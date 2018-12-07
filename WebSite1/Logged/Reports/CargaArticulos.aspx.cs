using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;


public partial class Logged_Reports_CargaArticulosB : System.Web.UI.Page
{
    PortalProveedoresEntities db = new PortalProveedoresEntities();
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
            List<CargaArticuloDTO> list_dto = new List<CargaArticuloDTO>();

            if (Request.QueryString.HasKeys())
            {
                string ItemID = Request["ItemID"];
                string Qty = Request["Qty"];
                string Status = Request["Status"];

                list_dto = CargaArticulo.ObtenerCargaArticulos(ItemID, Qty, Status, true);
            }
            else
                list_dto = CargaArticulo.ObtenerCargaArticulos(true);
            
            if(list_dto != null && list_dto.Count > 0)
                for(int i = 0; i < list_dto.Count; i++)
                {
                    list_dto[i].Monto = "$ " + list_dto[i].Monto;
                    list_dto[i].Costo = "$ " + list_dto[i].Costo;
                }

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "CargaArticuloReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Carga de Artículo");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");


            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_CargaArticulos.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de carga de artículos");
        }
    }
}