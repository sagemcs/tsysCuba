using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;
using SAGE_Model;

public partial class Logged_Reports_ProveedoresB : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    private sage500_appEntities db_sage = new sage500_appEntities();
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
            List<Vendors> list = db.Vendors.ToList();
            List<ProveedorDTO> list_dto = new List<ProveedorDTO>();

            if (Request.QueryString.HasKeys())
            {
                string VendorId = Request["id"];
                string UserId = Request["nombre"];
                string VendorName = Request["social"];
                string RFC = Request["rfc"];
                string Estado = Request["estado"];

                list_dto = Proveedores.ObtenerProveedores(VendorId, UserId, VendorName, RFC, Estado, true);
            }
            else
            {
                list_dto = Proveedores.ObtenerProveedores(true);
            }

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ProveedoresReport.rpt"));

            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Proveedores");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            Reporte_Proveedores.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de proveedores");
        }
    }
}