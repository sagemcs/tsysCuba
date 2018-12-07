using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using WebSite1;

public partial class Account_Manage : System.Web.UI.Page
{
    string eventName = String.Empty;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["RolUser"] != null)
        {

            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
            {
                Page.MasterPageFile = "~/Logged/Administradores/MasterPageContb.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
            {
                Page.MasterPageFile = "~/Logged/Administradores/SiteVal.master";
            }

        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    protected string SuccessMessage
    {
        get;
        private set;
    }

    protected bool CanRemoveExternalLogins
    {
        get;
        private set;
    }

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, string.Empty).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool HasPassword(UserManager manager)
    {
        var user = manager.FindById(User.Identity.GetUserId());
        return (user != null && user.PasswordHash != null);
    }

    protected void Page_Load()
    {
        if (!IsPostBack)
        {
            // Determine the sections to render
            UserManager manager = new UserManager();
            if (HasPassword(manager))
            {
                changePasswordHolder.Visible = true;
            }
            else
            {
                setPassword.Visible = true;
                changePasswordHolder.Visible = false;
            }
            CanRemoveExternalLogins = manager.GetLogins(User.Identity.GetUserId()).Count() > 1;

            // Render success message
            var message = Request.QueryString["m"];
            if (message != null)
            {
                // Strip the query string from action
                Form.Action = ResolveUrl("~/Account/Manage");

                SuccessMessage =
                    message == "ChangePwdSuccess" ? "Your password has been changed."
                    : message == "SetPwdSuccess" ? "Your password has been set."
                    : message == "RemoveLoginSuccess" ? "The account was removed."
                    : String.Empty;
                successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);

                if (message.ToString() == "ChangePwdSuccess")
                {
                    string tipo = "success";
                    string Msj = "Su contraseña ha cambiado";
                    string titulo = "Actualización de Password Exitosa";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
        }
    }

    protected void ChangePassword_Click(object sender, EventArgs e)
    {

        if (Email_Ok(NewPassword.Text) == true)
        {
            if (IsValid)
            {
                string Var = User.Identity.GetUserId();
                UserManager manager = new UserManager();
                IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text);
                if (result.Succeeded)
                {
                    var user = manager.FindById(User.Identity.GetUserId());
                    IdentityHelper.SignIn(manager, user, isPersistent: false);
                    Response.Redirect("~/Account/Manage?m=ChangePwdSuccess");
                }
                else
                {
                    AddErrors(result);
                }
            }

        }
        else
        {    
            string Msj1 = "";
            Msj1 = Msj1 + "La Contraseña debe de contar con:" + "<br/>";
            Msj1 = Msj1 + "Al menos una letra mayúscula" + "<br/>";
            Msj1 = Msj1 + "Al menos una letra minúscula" + "<br/>";
            Msj1 = Msj1 + "Al menos un dígito" + "<br/>";
            Msj1 = Msj1 + "Al menos un caracter especial" + "<br/>";
            Msj1 = Msj1 + "Mínimo ocho digítos de largo" + "<br/>";

            Label2.Text = Msj1;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);

        }

    }

    protected void SetPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Create the local login info and link the local account to the user
            UserManager manager = new UserManager();
            IdentityResult result = manager.AddPassword(User.Identity.GetUserId(), password.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Account/Manage?m=SetPwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }

    public IEnumerable<UserLoginInfo> GetLogins()
    {
        UserManager manager = new UserManager();
        var accounts = manager.GetLogins(User.Identity.GetUserId());
        CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(manager);
        return accounts;
    }

    public void RemoveLogin(string loginProvider, string providerKey)
    {
        UserManager manager = new UserManager();
        var result = manager.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        string msg = String.Empty;
        if (result.Succeeded)
        {
            var user = manager.FindById(User.Identity.GetUserId());
            IdentityHelper.SignIn(manager, user, isPersistent: false);
            msg = "?m=RemoveLoginSuccess";
        }
        Response.Redirect("~/Account/Manage" + msg);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
}