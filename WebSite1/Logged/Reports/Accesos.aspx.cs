using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;

public partial class Logged_Reports_AccesosB : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();

    protected void Page_Init(object sender, EventArgs e)
    {
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
            List<AccessLog> list = db.AccessLog.ToList();
            List<AccesoDTO> list_dto = new List<AccesoDTO>();

            if (Request.QueryString.HasKeys())
            {
                string userId = Request["userId"];
                string username = Request["username"];
                string ip = Request["ip"];

                list_dto = Accesos.ObtenerAccesos(userId, username, ip, true);
            }
            else
                list_dto = Accesos.ObtenerAccesos(true);

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "AccesosReport.rpt"));

            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Accesos");
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");

            Reporte_Accesos.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de accesos al sistema");
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        //    if (HttpContext.Current.Session["IDCompany"] == null)
        //    {
        //        Context.GetOwinContext().Authentication.SignOut();
        //        Response.Redirect("~/Account/Login.aspx");
        //    }
        //    else
        //    {
        //        if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
        //        {
        //            HttpContext.Current.Session.RemoveAll();
        //            Context.GetOwinContext().Authentication.SignOut();
        //            Response.Redirect("~/Account/Login.aspx");
        //        }
        //    }
        //}
        //try
        //{
        //    List<AccessLog> list = db.AccessLog.ToList();
        //    List<AccesoDTO> list_dto = new List<AccesoDTO>();

        //    if (Request.QueryString.HasKeys())
        //    {
        //        string userId = Request["userId"];
        //        string username = Request["username"];
        //        string ip = Request["ip"];

        //        list_dto = Accesos.ObtenerAccesos(userId, username, ip, true);
        //    }
        //    else
        //        list_dto = Accesos.ObtenerAccesos(true);

        //    var company_id = HttpContext.Current.Session["IDCompany"];
        //    if (company_id == null)
        //        return;
        //    Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

        //    ReportDocument report_document = new ReportDocument();
        //    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "AccesosReport.rpt"));

        //    report_document.SetDataSource(list_dto);
        //    report_document.SetParameterValue("titulo", "Reporte de Accesos");
        //    report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
        //    report_document.SetParameterValue("logo", "~/Img/TSYS.png");

        //    Reporte_Accesos.ReportSource = report_document;
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de accesos al sistema");
        //}
    }
}