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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSite1;

public partial class Logged_Reports_AprobacionSolicitudCheque : System.Web.UI.Page
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

    public Dictionary<int, string> Dict_status()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Pendiente" },
            { 2, "Aprobado" },
            { 3, "Cancelado" },
            { 4, "Vencido" }
        };
        return dict;
    }

    public List<AprobacionSolicitudChequeDTO> LoadData(int user_id)
    {
        List<AprobacionSolicitudChequeDTO> aprobaciones = new List<AprobacionSolicitudChequeDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpReportAprobacionSolCheque";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var aprob = new AprobacionSolicitudChequeDTO();
                aprob.InvcRcptKey = dataReader.GetInt32(0);
                aprob.VendorId = dataReader.GetString(1);
                aprob.VendName = dataReader.GetString(2);
                aprob.Solicitud = dataReader.GetString(3);
                aprob.Factura = dataReader.GetString(4);
                aprob.CuentasXPagar = dataReader.GetString(5);
                aprob.DireccionTesorería = dataReader.GetString(6);
                aprob.DireccionFinanzas = dataReader.GetString(7);
                aprobaciones.Add(aprob);
            }
        }
        return aprobaciones;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

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
            if (HttpContext.Current.Session["IDCompany"] == null)
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
            List<AprobacionSolicitudChequeDTO> list_dto = new List<AprobacionSolicitudChequeDTO>();
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

                list_dto = LoadData(pUserKey);
            }
            else
                list_dto = LoadData(pUserKey);

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "AprobacionSolChequeReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de aprobación de solicitudes de cheque");

            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            //report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_AprobacionSolCheque.ReportSource = report_document;
            Reporte_AprobacionSolCheque.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de anticipo de gastos");
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
        Response.Redirect("~/");
    }
}