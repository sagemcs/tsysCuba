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

public partial class Logged_Reports_AllReports : System.Web.UI.Page
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

    public Dictionary<int, string> Dict_type_anticipo()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Viaje" },
            { 2, "Gastos Extraordinarios" }
        };
        return dict;
    }   

    public List<AdvanceReport2DTO> LoadDataAdvance(int status_id, int user_id, DateTime? created)
    {
        List<AdvanceReport2DTO> gastos = new List<AdvanceReport2DTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select e.Amount as 'Importe', ISNULL(convert(varchar, e.DepartureDate, 3) , '') as 'FechaSalida', ISNULL(convert(varchar, e.ArrivalDate, 3) , '') as 'FechaLLegada', ISNULL(convert(varchar, e.CheckDate, 3) , '') as 'FechaComprobacion', e.ImmediateBoss as 'JefeInmediato', e.Status as 'Estado', e.Currency as 'Moneda', ISNULL(convert(varchar, e.CreateDate, 3) , '') as 'FechaCreado', e.UpdateUserKey, UserName from Advance e inner join Users u on e.UpdateUserKey = u.UserKey;";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                if (user_id != 0 && dataReader.GetInt32(9) == user_id)
                {
                    var advance = new AdvanceReport2DTO();
                    advance.Importe = dataReader.GetDecimal(0);
                    advance.FechaSalida = dataReader.GetString(1);
                    advance.FechaLLegada = dataReader.GetString(2);
                    advance.FechaComprobacion = dataReader.GetString(3);
                    advance.JefeInmediato = dataReader.GetString(4);
                    advance.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(5)).Value;
                    advance.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(6)).Value;
                    advance.FechaCreado = dataReader.GetString(7);
                    advance.Username = dataReader.GetString(9);
                    gastos.Add(advance);
                }
                else
                {
                    var advance = new AdvanceReport2DTO();
                    advance.Importe = dataReader.GetDecimal(0);
                    advance.FechaSalida = dataReader.GetString(1);
                    advance.FechaLLegada = dataReader.GetString(2);
                    advance.FechaComprobacion = dataReader.GetString(3);
                    advance.JefeInmediato = dataReader.GetString(4);
                    advance.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(5)).Value;
                    advance.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(6)).Value;
                    advance.FechaCreado = dataReader.GetString(7);
                    advance.Username = dataReader.GetString(9);
                    gastos.Add(advance);
                }
                                   
            }
        }
        if (status_id != 0)
        {
            gastos = gastos.Where(g => g.Estado == Doc_Tools.Dict_status().First(x => x.Key == status_id).Value).ToList();
        }
        if (created != null)
        {
            gastos = gastos.Where(g => g.FechaCreado == created.ToString()).ToList();
        }
        return gastos;
    }

    public List<ExpenseReport2DTO> LoadDataReembolso(int status_id, int user_id, DateTime? created)
    {
        List<ExpenseReport2DTO> gastos = new List<ExpenseReport2DTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select e.Amount as 'Importe', e.Status as 'Estado', e.Currency as 'Moneda', ISNULL(convert(varchar, e.Date, 3) , '') as 'Fecha', e.UpdateUserKey, UserName from Expense e inner join Users u on e.UpdateUserKey = u.UserKey";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {

                if (user_id != 0 && dataReader.GetInt32(5) == user_id)
                {
                    var expense = new ExpenseReport2DTO();
                    expense.Importe = dataReader.GetDecimal(0);
                    expense.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    expense.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                    expense.FechaCreado = dataReader.GetString(3);
                    expense.Username = dataReader.GetString(5);
                    gastos.Add(expense);
                }
                else
                {
                    var expense = new ExpenseReport2DTO();
                    expense.Importe = dataReader.GetDecimal(0);
                    expense.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    expense.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                    expense.FechaCreado = dataReader.GetString(3);
                    expense.Username = dataReader.GetString(5);
                    gastos.Add(expense);
                }
                
            }
        }
        if (status_id != 0)
        {
            gastos = gastos.Where(g => g.Estado == Doc_Tools.Dict_status().First(x => x.Key == status_id).Value).ToList();
        }
        if (created != null)
        {
            gastos = gastos.Where(g => g.FechaCreado == created.ToString()).ToList();
        }
        return gastos;
    }

    public List<MinorMedicalExpenseReport2DTO> LoadDataMedical(int status_id, int user_id, DateTime? created)
    {
        List<MinorMedicalExpenseReport2DTO> gastos = new List<MinorMedicalExpenseReport2DTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select e.Amount as 'Importe', e.Status as 'Estado', ISNULL(convert(varchar, e.Date, 3) , '') as 'Fecha', e.UpdateUserKey, UserName from MinorMedicalExpense e inner join Users u on e.UpdateUserKey = u.UserKey";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                if (user_id != 0 && dataReader.GetInt32(3) == user_id)
                {
                    var medical = new MinorMedicalExpenseReport2DTO();
                    medical.Importe = dataReader.GetDecimal(0);
                    medical.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    medical.Fecha = dataReader.GetString(2);
                    medical.Username = dataReader.GetString(4);
                    gastos.Add(medical);
                }
                else
                {
                    var medical = new MinorMedicalExpenseReport2DTO();
                    medical.Importe = dataReader.GetDecimal(0);
                    medical.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    medical.Fecha = dataReader.GetString(2);
                    medical.Username = dataReader.GetString(4);
                    gastos.Add(medical);
                }
                    
            }
        }
        if (status_id != 0)
        {
            gastos = gastos.Where(g => g.Estado == Doc_Tools.Dict_status().First(x => x.Key == status_id).Value).ToList();
        }
        if (created != null)
        {
            gastos = gastos.Where(g => g.Fecha == created.ToString()).ToList();
        }
        return gastos;
    }

    public List<CorporateCardReport2DTO> LoadDataTarjeta(int status_id, int user_id, DateTime? created)
    {
        List<CorporateCardReport2DTO> gastos = new List<CorporateCardReport2DTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select e.Amount as 'Importe', e.Status as 'Estado', e.Currency as 'Moneda', ISNULL(convert(varchar, e.CreateDate, 3) , '') as 'FechaCreado', e.UpdateUserKey, UserName from CorporateCard  e inner join Users u on e.UpdateUserKey = u.UserKey";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                if (user_id != 0 && dataReader.GetInt32(5) == user_id)
                {
                    var corpCard = new CorporateCardReport2DTO();
                    corpCard.Importe = dataReader.GetDecimal(0);
                    corpCard.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    corpCard.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                    corpCard.FechaCreado = dataReader.GetString(3);
                    corpCard.Username = dataReader.GetString(5);
                    gastos.Add(corpCard);
                }
                else
                {
                    var corpCard = new CorporateCardReport2DTO();
                    corpCard.Importe = dataReader.GetDecimal(0);
                    corpCard.Estado = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(1)).Value;
                    corpCard.Moneda = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                    corpCard.FechaCreado = dataReader.GetString(3);
                    corpCard.Username = dataReader.GetString(5);
                    gastos.Add(corpCard);
                }
            }
        }

        if(status_id != 0)
        {
            gastos = gastos.Where(g => g.Estado == Doc_Tools.Dict_status().First(x => x.Key == status_id).Value).ToList();
        }
        if (created != null)
        {
            gastos = gastos.Where(g => g.FechaCreado == created.ToString()).ToList();
        }
        return gastos;
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
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
        if (!IsPostBack)
        {
            BindEmpleados();
        }
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

    protected void btn_back_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Logged/Administradores/AnticipoEmpleados");
    }

    protected void btn_filtrar_Click(object sender, EventArgs e)
    {
        tbx_fecha_inicio.Text = string.Empty;
        drop_empleados.SelectedIndex = -1;
        drop_docs.SelectedIndex = -1;
        drop_status.SelectedIndex = -1;
        Reporte_Anticipos.Visible = false;
    }

    protected void btn_generar_Click(object sender, EventArgs e)
    {
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        int user_id = int.Parse(drop_empleados.SelectedItem.Value);
        int type = int.Parse(drop_docs.SelectedItem.Value);
        DateTime? created = !string.IsNullOrEmpty(tbx_fecha_inicio.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_inicio.Text) : null;
        try
        {
            List<Invoice> list = new List<Invoice>();

            if (type == 0)
            {
                List<ExpenseReport2DTO> list_advance = LoadDataReembolso(status_id, user_id, created);
                report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ReembolsosInnerReport.rpt"));
                report_document.SetDataSource(list_advance);
                report_document.SetParameterValue("titulo", "Reporte de Reembolso de Gastos");
            } else if(type == 1)
            {
                List<AdvanceReport2DTO> list_advance = LoadDataAdvance(status_id, user_id, created);
                report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "AnticiposInnerReport.rpt"));
                report_document.SetDataSource(list_advance);
                report_document.SetParameterValue("titulo", "Reporte de Anticipo de Gastos");
            }
            else if (type == 2)
            {
                List<MinorMedicalExpenseReport2DTO> list_advance = LoadDataMedical(status_id, user_id, created);
                report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "MinorMedicalExpenseInnerReport.rpt"));
                report_document.SetDataSource(list_advance);
                report_document.SetParameterValue("titulo", "Reporte de Gastos Médicos Menores");
            } else if(type == 3)
            {
                List<CorporateCardReport2DTO> list_tarjeta = LoadDataTarjeta(status_id, user_id, created);
                report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "TarjetaEmpleadoInnerReport.rpt"));
                report_document.SetDataSource(list_tarjeta);
                report_document.SetParameterValue("titulo", "Reporte de Gastos de Tarjeta Corporativa");
            }
            

            Company company = Tools.EmpresaAutenticada();
            //if (company == null)
            //    return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            //report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Anticipos.ReportSource = report_document;
            Reporte_Anticipos.SeparatePages = false;
            Reporte_Anticipos.Visible = true;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de anticipo de gastos");
        }
    }

    private void BindEmpleados()
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        List<RolDTO> roles = Tools.get_RolesValidadores().ToList();
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        empleados = Tools.GetEmpleados(pUserKey, level, Tools.DocumentType.Advance);

        empleados.Add(new EmpleadoDTO() { UserKey = 0, Nombre = "" });
        drop_empleados.DataSource = empleados.Select(x => new { Id = x.UserKey, Nombre = x.Nombre }).OrderBy(o => o.Id).ToList();
        drop_empleados.DataTextField = "Nombre";
        drop_empleados.DataValueField = "Id";
        drop_empleados.DataBind();
        drop_empleados.SelectedIndex = -1;
    }

    protected void drop_empleados_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
    }
}