using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;

public partial class Logged_ReportsFilter_ContrarecibosFilter : System.Web.UI.Page
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
        }
        Page.Title = "Contrarecibos";
        try
        {
            List<ProveedorDTO> list_dto = Proveedores.ObtenerProveedores(true);
            if (list_dto != null && list_dto.Count != 0)
            {
                ProveedorDTO vacio = new ProveedorDTO();
                vacio.Social = "[-Seleccione proveedor-]";
                vacio.ID = "";
                list_dto.Insert(0, vacio);

                comboProveedores.DataSource = list_dto;
                comboProveedores.DataTextField = "Social";
                comboProveedores.DataValueField = "ID";
                comboProveedores.DataBind();
            }
            else
                return;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar los contrarecibos");
        }
    }
}