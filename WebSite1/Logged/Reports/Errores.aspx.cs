using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;

public partial class Logged_Reports_ErroresB : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    protected void Page_Load(object sender, EventArgs e)
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
            List<ErrorLog> list = db.ErrorLog.ToList();
            List<ErrorDTO> list_dto = new List<ErrorDTO>();

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            //if (Request.QueryString.HasKeys())
            //{
            //    //list_dto = Errores.ObtenerErrores();
            //}
            //else
            //{
                list_dto = list_dto = Errores.ObtenerErrores(true);
            //}

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ErroresReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Errores");
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Errores.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de errores");
        }
    }
}