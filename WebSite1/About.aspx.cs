using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class About : Page
{
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                {
   
                }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }

            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
            {
                Page.MasterPageFile = "MasterPageContb.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
        }
        catch (Exception ex)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }
}