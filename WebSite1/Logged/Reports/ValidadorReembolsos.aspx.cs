﻿using CrystalDecisions.CrystalReports.Engine;
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

public partial class Logged_Reports_ValidadorReembolsos : System.Web.UI.Page
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

    public Dictionary<int, string> Dict_type()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Viaje" },
            { 2, "Compra Extraordinaria" }
        };
        return dict;
    }

    public List<ExpenseValidatorReportDTO> LoadData(int user_id)
    {
        List<ExpenseValidatorReportDTO> gastos = new List<ExpenseValidatorReportDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            int role = 0;
            switch (rol)
            {
                //si el usuario es Cuentas por Cobrar 
                case "T|SYS| - Validador":
                    role = 0;
                    break;
                //si el usuario es Tesoreria --> traer Cuentas por Cobrar
                case "T|SYS| - Tesoreria":
                    role = 3;
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Finanzas":
                    role = 4;
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerente":
                    role = 1;
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerencia de Capital Humano":
                    role = 2;
                    break;
            }
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpReportReembolsoGastosValidador";
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@UpdateUserKey",
                Value = user_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@role",
                Value = role
            });
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var expense = new ExpenseValidatorReportDTO();
                expense.Nombre = dataReader.GetString(0);
                expense.Fecha = dataReader.GetString(1);
                expense.Tipo = Dict_type().First(x => x.Key == dataReader.GetInt32(2)).Value;
                expense.Importe = dataReader.GetDecimal(3);
                gastos.Add(expense);
            }
        }
        return gastos;
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
        try
        {
            List<ExpenseValidatorReportDTO> list_dto = new List<ExpenseValidatorReportDTO>();
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

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ValidadorAnticiposReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Reembolsos");

            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            //report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_ValidadorReembolsos.ReportSource = report_document;
            Reporte_ValidadorReembolsos.SeparatePages = false;
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
        Response.Redirect("~/");
    }
}