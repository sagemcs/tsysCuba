using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSite1;

public partial class Logged_Reports_Comprobacion_Gastos_Tarjeta : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    ReportDocument report_document = new ReportDocument();
    private int iUserKey;
    private string iCompanyID;
    private int iLogKey;
    public int pUserKey
    {
        get
        {
            return this.iUserKey;
        }
        set
        {
            this.iUserKey = value;
        }
    }
    public string pCompanyID
    {
        get
        {
            return this.iCompanyID;
        }
        set
        {
            this.iCompanyID = value;
        }
    }
    public int pLogKey
    {
        get
        {
            return this.iLogKey;
        }
        set
        {
            this.iLogKey = value;
        }
    }

    public List<ExpenseDetailComprobacionDTO> LoadData(int docKey, int createUser, string companyId)
    {
        var lista = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        List<ExpenseDetailComprobacionDTO> articles = new List<ExpenseDetailComprobacionDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpReportComprobacionGastosTarjeta";
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@docKey",
                Value = docKey
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@createUser",
                Value = createUser
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@companyId",
                Value = companyId
            });

            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var article = new ExpenseDetailComprobacionDTO();
                article.DetailId = dataReader.GetInt32(0);
                article.ItemKey = dataReader.GetInt32(2);
                article.Qty = dataReader.GetDecimal(3);
                article.UnitCost = dataReader.GetDecimal(4);
                article.ItemId = lista.FirstOrDefault(x => x.ItemKey == article.ItemKey).ItemId;
                article.TaxAmount = dataReader.GetDecimal(6);
                article.Type = dataReader.GetInt32(7);
                article.STaxCodeKey = dataReader.GetInt32(5);
                article.STaxCodeID = taxes.FirstOrDefault(x => x.STaxCodeKey == article.STaxCodeKey).STaxCodeID;
                article.TipoGasto = Doc_Tools.Dict_tipos_gastos().FirstOrDefault(x => x.Key == article.Type).Value;
                if (!articles.Any(x => x.DetailId == article.DetailId))
                {
                    articles.Add(article);
                }
            }
        }
        return articles;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["IDCompany"] == null || HttpContext.Current.Session["UserKey"] == null)
        {
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
            return;
        }

        pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

        int docKey = Convert.ToInt32(HttpContext.Current.Session["DocKey"].ToString());
        int createUser = Convert.ToInt32(HttpContext.Current.Session["CreateUser"].ToString());

        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuth)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null || HttpContext.Current.Session["UserKey"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }
        try
        {
            List<ExpenseDetailComprobacionDTO> list_dto = new List<ExpenseDetailComprobacionDTO>();
            List<Invoice> list = new List<Invoice>();

            if (Request.QueryString.HasKeys())
            {

                string folio = Request["folio"];
                string serie = Request["serie"];
                string fecha = Request["fecha"];
                string fechaR = Request["fechaR"];
                string proveedor = Request["provId"];
                string total = Request["total"];
                string uuid = Request["uuid"];
                string estado = Request["estado"];

                fecha = Tools.ObtenerFechaEnFormatoNew(fecha);
                fechaR = Tools.ObtenerFechaEnFormatoNew(fechaR);

                list_dto = LoadData(docKey, createUser, pCompanyID).ToList();
            }
            else
                list_dto = LoadData(docKey, createUser, pCompanyID).ToList();

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ComprobacionReport.rpt"));
            var anomlist = list_dto.Select(x => new { x.ItemKey, x.Qty, x.UnitCost, x.ItemId, x.TaxAmount, x.Type, x.STaxCodeKey, x.STaxCodeID, x.TipoGasto, x.Amount }).ToList();

            report_document.SetDataSource(anomlist);
            report_document.SetParameterValue("titulo", "Comprobación de Gastos de Tarjeta Corporativa");

            Company company = Tools.EmpresaAutenticada();
            //if (company == null)
            //    return;
            Users user = Tools.UsuarioAutenticado();
            Tuple<string, string> data = Tools.GetUsuarioPuestoArea(pUserKey);
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("beneficiario", user != null ? user.UserName : "Beneficiario");
            report_document.SetParameterValue("puesto", user != null ? data.Item1 : "Puesto");
            report_document.SetParameterValue("area", user != null ? data.Item2 : "Área");
            report_document.SetParameterValue("folio", "");

            //report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            if (list_dto.Count > 0)
                Reporte_Reembolsos.ReportSource = report_document;
            Reporte_Reembolsos.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de reembolsos");
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
    }
    private void Page_Unload(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["IDCompany"] == null || HttpContext.Current.Session["UserKey"] == null)
        {
            Context.GetOwinContext().Authentication.SignOut();
            return;
        }
        CloseReports(report_document);
        report_document.Dispose();
        report_document.Dispose();
        report_document = null;

    }
    private void CloseReports(ReportDocument reportDocument)
    {
        Sections sections = reportDocument.ReportDefinition.Sections;
        foreach (Section section in sections)
        {
            ReportObjects reportObjects = section.ReportObjects;
            foreach (ReportObject reportObject in reportObjects)
            {
                if (reportObject.Kind == ReportObjectKind.SubreportObject)
                {
                    SubreportObject subreportObject = (SubreportObject)reportObject;
                    ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                    subReportDocument.Close();
                }
            }
        }
        reportDocument.Close();
    }

    protected void btn_back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Logged/Administradores/TarjetaEmpleado");
    }
}