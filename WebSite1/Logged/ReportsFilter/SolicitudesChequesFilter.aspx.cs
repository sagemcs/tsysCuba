using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_ReportsFilter_SolicitudesChequesFilter : System.Web.UI.Page
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
        Page.Title = "Solicitudes cheques";
        try
        {
            List<ProveedorDTO> list_dto_proveedores = Proveedores.ObtenerProveedores(true);
            List<UsuarioDTO> list_dto_solicitantes = Usuarios.ObtenerUsuarios(true);

            if (list_dto_proveedores != null && list_dto_proveedores.Count != 0)
            {
                ProveedorDTO vacio_p = new ProveedorDTO();
                vacio_p.Social = "[-Seleccione proveedor-]";
                vacio_p.ID = "";
                list_dto_proveedores.Insert(0, vacio_p);
                comboProveedores.DataSource = list_dto_proveedores;
                comboProveedores.DataTextField = "Social";
                comboProveedores.DataValueField = "ID";
                comboProveedores.DataBind();
            }
            if (list_dto_solicitantes != null && list_dto_solicitantes.Count != 0)
            {
                UsuarioDTO vacio_s = new UsuarioDTO();
                vacio_s.Nombre = "[-Seleccione solicitante-]";
                vacio_s.Correo = "";
                list_dto_solicitantes.Insert(0, vacio_s);
                comboSolicitantes.DataSource = list_dto_solicitantes;
                comboSolicitantes.DataTextField = "Nombre";
                comboSolicitantes.DataValueField = "Correo";
                comboSolicitantes.DataBind();
            }
            else
                return;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar las solicitudes de cheque");
        }
    }
}