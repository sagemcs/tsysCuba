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

public partial class Logged_Reports_SolicitudesChequeB : System.Web.UI.Page
{
    PortalProveedoresEntities db = new PortalProveedoresEntities();
    sage500_appEntities db_sage = new sage500_appEntities();
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
            List<CheckRequest> list = db.CheckRequest.ToList();
            List<ChequeSolicitudDTO> list_dto = new List<ChequeSolicitudDTO>();

            if (Request.QueryString.HasKeys())
            {
                string Serie = Request["serie"];
                string Proveedor = Request["provId"];
                string Solicitante = Request["userId"];
                string Total = Request["total"];
                string fecha = Request["fecha"];
                fecha = Tools.ObtenerFechaEnFormato(fecha);

                list_dto = ChequeSolicitudes.ObtenerSolicitudesCheque(Serie, Proveedor, Solicitante, Total, fecha, true);
            }
            else
            {
                list_dto = ChequeSolicitudes.ObtenerSolicitudesCheque(true);
            }

            if (list_dto == null)
                throw new MulticonsultingException("No hay solicitudes de cheque que mostrar");

            foreach (var c in list_dto)
                c.Total = "$ " + c.Total;

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudesCheque.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Solicitud de Cheque");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Solicitudes_Cheque.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de solicitudes de cheque");
        }
    }
}