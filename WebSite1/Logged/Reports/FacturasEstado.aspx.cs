using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using Proveedores_Model;


public partial class Logged_Reports_FacturasEstadoB : System.Web.UI.Page
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
            List<FacturaDTO> list_dto = new List<FacturaDTO>();
            List<Invoice> list = new List<Invoice>();

            if (Request.QueryString.HasKeys())
            {

                string folio = Request["folio"];
                string fecha = Request["fecha"];
                string proveedor = Request["provId"];
                string contrarecibo = Request["contrarecibo"];
                string solicitud = Request["solicitud"];
                string estado = Request["estado"];

                fecha = Tools.ObtenerFechaEnFormato(fecha);

                list_dto = Facturas.ObtenerFacturas(proveedor, folio, fecha, contrarecibo, solicitud, estado, true);
            }
            else
                list_dto = Facturas.ObtenerFacturas(true);

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "FacturasEstadoReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Estado de Facturas");


            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            string path = Server.MapPath("~/Img/estados/");
            report_document.SetParameterValue("img_base", path);
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_FacturasEstado.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte estado de facturas");
        }

    }
}