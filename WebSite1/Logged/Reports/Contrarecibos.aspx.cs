using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;


public partial class Logged_Reports_ContrarecibosB : System.Web.UI.Page
{
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
        }
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();
            if (Request.QueryString.HasKeys())
            {
                string folio = Request["folio"];
                string proveedor = Request["provId"];
                string total = Request["total"];
                string rfc = Request["rfc"];
                string fecha = Request["&fecha_r="];
                fecha = Tools.ObtenerFechaEnFormato(fecha);

                contrarecibos = Contrarecibos.ObtenerContrarecibos(folio, proveedor, rfc, total, fecha, false, true);
            }
            else
                contrarecibos = Contrarecibos.ObtenerContrarecibos(false, true);

            if (contrarecibos == null)
                throw new MulticonsultingException("No hay contrarecibos que mostrar");

            foreach (var c in contrarecibos)
                c.Total = "$ " + c.Total;

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrarecibosReport.rpt"));
            report_document.SetDataSource(contrarecibos);

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            report_document.SetParameterValue("titulo", "Reporte de Contrarecibos");
            if (company != null)
            {
                report_document.SetParameterValue("compannia", company.CompanyName);
            }
            Reporte_Contrarecibos.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de contrarecibos");
        }

    }
}