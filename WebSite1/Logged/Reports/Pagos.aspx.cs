using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;

public partial class Logged_Reports_PagosB : System.Web.UI.Page
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
            List<Payment> list = db.Payment.ToList();
            List<PagoDTO> list_dto = new List<PagoDTO>();

            if (Request.QueryString.HasKeys())
            {
                string folio = Request["folio"];
                string serie = Request["serie"];
                string fecha = Request["fecha"];
                string proveedor = Request["provId"];
                string total = Request["total"];
                string uuid = Request["uuid"];
                string estado = Request["estado"];
                fecha = Tools.ObtenerFechaEnFormato(fecha);

                list_dto = Pagos.ObtenerPagos(folio, serie, fecha, proveedor, total, uuid, estado, true);
            }
            else
            {
                list_dto = Pagos.ObtenerPagos(true);
            }


            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "PagosReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Pagos");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Pagos.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de pagos");
        }
    }
}