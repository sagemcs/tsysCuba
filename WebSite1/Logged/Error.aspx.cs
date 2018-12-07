using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Error : System.Web.UI.Page
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
        if (Request.QueryString.HasKeys())
        {
            try {
                ErrorLabel.Text = "Lo sentimos. " + Request["error"] + ". Le recomendamos ponerse en contacto con los desarrolladores del sistema.";
            }
            catch {
                Response.Redirect("~/Logged/Error");
            }
        }
    }
}