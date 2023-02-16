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
using static Tools;

public partial class Logged_Reports_AprobacionSolicitudCheque : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    ReportDocument report_document = new ReportDocument();
    int user_id = 0;
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

    public List<RechazoSolicitudChequeDTO> LoadDataRechazo(int user_id)
    {
        List<RechazoSolicitudChequeDTO> rechazos = new List<RechazoSolicitudChequeDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpReportRechazoSolCheque";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var aprob = new RechazoSolicitudChequeDTO();
                aprob.InvcRcptKey = dataReader.GetInt32(0);
                aprob.VendorId = dataReader.GetString(1);
                aprob.VendName = dataReader.GetString(2);
                aprob.Solicitud = dataReader.GetString(3);
                aprob.Factura = dataReader.GetString(4);
                aprob.Rechazo = dataReader.GetString(5);
                aprob.Motivo = dataReader.GetString(6);
                aprob.Fecha = dataReader.GetString(7);
                rechazos.Add(aprob);
            }
        }
        return rechazos;
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
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
        //if (!IsPostBack)
        //{
        //    BindEmpleados();
        //    BindStatus();
        //}
    }
    //private void Page_Unload(object sender, EventArgs e)
    //{
    //    //CloseReports(report_document);
    //    report_document.Dispose();
    //    report_document.Dispose();
    //    report_document = null;
    //}
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

    //private void BindStatus()
    //{
    //    var estados = Doc_Tools.Dict_status().Select((x) => new { Id = x.Key, Nombre = x.Value }).ToList();
    //    estados.Add(new { Id = 0, Nombre = "Todos" });
    //    drop_status.DataSource = estados.OrderBy(o => o.Id).ToList();
    //    drop_status.DataTextField = "Nombre";
    //    drop_status.DataValueField = "Id";
    //    drop_status.DataBind();
    //    drop_status.SelectedIndex = -1;
    //}
    protected void btn_back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/");
    }

    //protected void btn_filtrar_Click(object sender, EventArgs e)
    //{
    //    tbx_fecha_inicio.Text = string.Empty;
    //    drop_empleados.SelectedIndex = -1;
    //    drop_docs.SelectedIndex = -1;
    //    drop_status.SelectedIndex = -1;
    //    Reporte_Anticipos.Visible = false;
    //}

    protected void btn_generar_Click(object sender, EventArgs e)
    {
        int status_id = int.Parse(drop_docs.SelectedItem.Value);
        var count = 0;
        try
        {
            switch (status_id)
            {
                case 1:
                    var list_dto = LoadData(user_id);
                    count = list_dto.Count;
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "AprobacionSolChequeReport.rpt"));
                    report_document.SetDataSource(list_dto);
                    report_document.SetParameterValue("titulo", "Reporte de aprobación de solicitudes de cheque");
                    break;
                case 2:
                    var list_dtoR = LoadDataRechazo(user_id);
                    count = list_dtoR.Count;
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "RechazoSolChequeReport.rpt"));
                    report_document.SetDataSource(list_dtoR);
                    report_document.SetParameterValue("titulo", "Reporte de rechazo de solicitudes de cheque");
                    break;
               
            }          
            
            Company company = Tools.EmpresaAutenticada();
            //if (company == null)
            //    return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            //report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            if (count > 0)
                Reporte_AprobacionSolCheque.ReportSource = report_document;
            Reporte_AprobacionSolCheque.SeparatePages = false;
            Reporte_AprobacionSolCheque.Visible = true;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de anticipo de gastos");
        }
    }

    //private void BindEmpleados()
    //{
    //    List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
    //    List<RolDTO> roles = Tools.get_RolesValidadores().ToList();
    //    string rol = HttpContext.Current.Session["RolUser"].ToString();
    //    int level = roles.FirstOrDefault(x => x.ID == rol).Key;
    //    empleados = Tools.GetEmpleados(pUserKey, level, Tools.DocumentType.Advance);

    //    empleados.Add(new EmpleadoDTO() { UserKey = 0, Nombre = "" });
    //    drop_empleados.DataSource = empleados.Select(x => new { Id = x.UserKey, Nombre = x.Nombre }).OrderBy(o => o.Id).ToList();
    //    drop_empleados.DataTextField = "Nombre";
    //    drop_empleados.DataValueField = "Id";
    //    drop_empleados.DataBind();
    //    drop_empleados.SelectedIndex = -1;
    //}

    //protected void drop_empleados_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (drop_empleados.SelectedItem != null)
    //    {
    //        user_id = int.Parse(drop_empleados.SelectedItem.Value);
    //    }
    //}
}