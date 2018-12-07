using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;

public partial class Logged_Reports_UsuariosB : System.Web.UI.Page
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
            List<Users> list = db.Users.ToList();
            List<UsuarioDTO> list_dto = new List<UsuarioDTO>();

            if (Request.QueryString.HasKeys())
            {
                string nombre = Request["nombre"].ToString();
                string proveedor = Request["provId"].ToString();
                string interno = Request["interno"].ToString();
                string estado = Request["estado"].ToString().Replace(",", string.Empty);

                list_dto = Usuarios.ObtenerUsuarios(nombre, proveedor, interno, estado, true);
            }
            else
            {
                list_dto = Usuarios.ObtenerUsuarios(true);
            }

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "UsuariosReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Usuarios");
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Usuarios.ReportSource = report_document;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de usuarios");
        }

    }
}