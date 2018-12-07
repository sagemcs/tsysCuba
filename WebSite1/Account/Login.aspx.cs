using Microsoft.AspNet.Identity;
using System;
using System.Text;
using System.Web.UI;
using WebSite1;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.IO;
using EASendMail;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web;
using System.Security.Cryptography;
using Microsoft.AspNet.Identity.EntityFramework;

public partial class Account_Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["UserKey"] != null)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        else
        {
            if (EmpresaID.Items.Count == 0)
            {
                EmpresaID.Visible = false;
                LEmpresas.Visible = false;
            }
        }


    }

    protected void LogIn(object sender, EventArgs e)
    {
        try
        {
            if (Email_Ok(UserName1.Text) == true)
            {
                var manager = new UserManager();
                ApplicationUser Busca = manager.Find(UserName1.Text, Password1.Text);
                if (Busca != null)
                {
                    Global.LoginStatus(UserName1.Text);
                    if (HttpContext.Current.Session["Status"].ToString() != "Activo")
                    {
                        UserName1.Text = "";
                        Password1.Text = "";
                        EmpresaID.Items.Clear();
                        LEmpresas.Visible = false;
                        EmpresaID.Visible = false;
                        string titulo, Msj, tipo;
                        tipo = "error";
                        Msj = "Usuario No Activo, Verificalo con el Administrador";
                        titulo = "Error";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                    else
                    {
                        if (EmpresaID.Visible == true && EmpresaID.SelectedItem.Value != "N/D")
                        {
                            if (IsValid)
                            {
                                using (ApplicationDbContext db = new ApplicationDbContext())
                                {
                                    Global.VarSesion(Busca.UserName.ToString(), EmpresaID.SelectedItem.ToString());
                                    DatosLogin();
                                    if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, RememberMe.Checked);
                                        Response.Redirect("~/Logged/Proveedores/Default.aspx");
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, RememberMe.Checked);
                                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, RememberMe.Checked);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx");
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, RememberMe.Checked);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx");
                                    }
                                }

                            }
                        }///
                        else
                        {
                            User_Empresas(EmpresaID, Busca.UserName.ToString());
                        }
                    }
                }
                else
                {
                    string titulo, Msj, tipo;
                    tipo = "error";
                    Msj = "El Correo y/o Contraseña Invalido, Favor de Verificar";
                    titulo = "Error";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
            else
            {
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Formato de Email Invalido";
                titulo = "Error";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }

        }
        catch(Exception ex)
        {

        }

    }

    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "Reset();", true);
    }

    public string CrearPassword(int longitud)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&#!@";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < longitud--)
        {
            res.Append(caracteres[rnd.Next(caracteres.Length)]);
        }
        return res.ToString();
    }


    protected void btnVer_Click(object sender, EventArgs e)
    {
        string titulo, Msj, tipo;
        bool blnValidUser;
        bool SendEmail;
        bool UpdatePass;
        if (ctrlGoogleReCaptcha.Validate())
        {

        blnValidUser = IsValidUser();
        if (blnValidUser == true)
        {
            string PassNew = CrearPassword(8);    
            string PassHAs = PassNew.ToString();
            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            //PassHAs = "12345678";
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            UpdatePass = UpdatePAss(UserName.Text, hashedNewPassword);

            if (UpdatePass == true)
            {

                string Body;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
                {
                 Body = reader.ReadToEnd();
                 Body = Body.Replace("{PassTemp}", PassNew);
                }

                 SendEmail = Global.EmailGlobal(UserName.Text, Body, "Recuperación de Contraseña");
                if (SendEmail == true)
                {
                    tipo = "success";
                    Msj = "Se ha enviado un Correo Electronico, Sigue las Instrucciones para restablecer la contraseña";
                    titulo = "Correo de Restablecimiento de Contraseña";
                }
                else
                {
                    tipo = "error";
                    Msj = "Se ha generado un error al intentar enviar el Correo, comunícate con el área de sistemas para darte una solución";
                    titulo = "Error";
                }
            }
            else
            {
                tipo = "error";
                Msj = "Se Ha Generado un error al intentar Obtener Los Datos";
                titulo = "Error";
            }
        }
        else
        {
            tipo = "error";
            Msj = "El Correo Ingresado, No Existe dentro de los Registros, Favor de Verificar";
            titulo = "Error";
        }

        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
        else
        {
            string Caja = "Al";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
        }

    }

    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ctrlGoogleReCaptcha.PublicKey = "6LfVDgMTAAAAAJnH9GV0i7r_Pl3FfC_hyfh2Cgnq";
        ctrlGoogleReCaptcha.PrivateKey = "6LfVDgMTAAAAAPfTlH1n7z72CvS46c2C_qkTmFsZ";
        //this.Panel1.Controls.Add(ctrlGoogleReCaptcha);
    }

    private bool IsValidUser()
    {
        bool cont = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {

                SqlCommand cmd = new SqlCommand("spValUserPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = UserName.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                cont = false;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Result = rdr["Resultado"].ToString();
                    if (Result != "0")
                    {
                        cont = true;
                    }
                }
                conn.Close();
            }
        }
        catch
        {
            return cont;
        }
        return cont;
    }

    private bool Email_Ok(string email)
    {
        String expresion;
        expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, String.Empty).Length == 0)
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

    private bool Email(string PassNew)
    {
        bool Resut = false;
        try
        {
           string body = string.Empty;                   
           using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
           {
             body = reader.ReadToEnd();
             body = body.Replace("{PassTemp}", PassNew);
          }
            Global.EmailGlobal(UserName.Text, body, "RECUPERACIÓN DE CONTRASEÑA");
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    private bool EmailTsys(string Destinatario, string PassNew)
    {
        bool rest = false;
        SmtpMail oMail = new SmtpMail("TryIt");
        EASendMail.SmtpClient oClient = new EASendMail.SmtpClient();

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSetingEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "General" });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                string Errores = string.Empty;
                string UsuarioE, PassEm, Puerto1, Host;
                string Emailportal = string.Empty;
                string smtp = string.Empty;
                string PassPortal = string.Empty;
                string PPortal = string.Empty;
                int puerto;
                Host = string.Empty;
                Puerto1 = string.Empty;
                PassEm = string.Empty;
                UsuarioE = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UsuarioE = rdr["UserMail"].ToString();
                    PassEm = rdr["Pass"].ToString();
                    Puerto1 = rdr["Puerto"].ToString();
                    Host = rdr["Host"].ToString();
                }


                Emailportal = UsuarioE;
                smtp = Host;
                PassPortal = PassEm;
                PPortal = Puerto1;
                puerto = Convert.ToInt32(PPortal);

                string Body;

                using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
                {
                    Body = reader.ReadToEnd();
                    Body = Body.Replace("{PassTemp}", PassNew);

                }

                StringBuilder emailHtml = new StringBuilder(Server.MapPath("~/Account/Templates Email/ResetPass.html"));

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtp, puerto);
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                MailMessage mensaje = new MailMessage(Destinatario, Emailportal, "Recuperación de Contraseña", Body);

                try
                {
                    client.Send(mensaje);
                    rest = true;
                }
                catch(Exception ex)
                {
                    // Error al Enviar Correo
                }

            }
        }

        catch
        {
            //Seccion Errores
        }
        return rest;
    }

    protected void User_Empresas(DropDownList Caja, string Usuario)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompanyLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = Usuario });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Empresas"].ToString() != "")
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Empresas"].ToString());
                        Caja.Items.Insert(0, Linea);
                        EmpresaID.Visible = true;
                        LEmpresas.Visible = true;
                    }
                    else
                    {
                        throw new Exception(Errores);
                    }
                }
                if (EmpresaID.Items.Count == 0)
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = ("N/D");
                    Caja.Items.Insert(0, Linea);
                    EmpresaID.Visible = true;
                    LEmpresas.Visible = true;
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {
            EmpresaID.Items.Clear();
            EmpresaID.Visible = false;
            LEmpresas.Visible = false;
        }
    }

    protected bool UpdatePAss(string Email, string NewPass)
    {
        bool Resultado = false;
        try
        {
            string oldPass = string.Empty;
            string IdUser = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                        SqlCommand cmd1 = new SqlCommand("spResetPass", conn);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Pass", Value = NewPass });

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Email", Value = Email });

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Opcion", Value = 2 });

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();
                        SqlDataReader rdr1 = cmd1.ExecuteReader();
                        Resultado = true;
                        conn.Close();
                    }
            }

        catch (Exception ex)
        {
            string Error = ex.Message;
            Resultado = false;
        }
        return Resultado;
    }

    public static string GetDireccionIp(HttpRequest request)
    {
        string ipAddress1 = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        string ipAddress2 = request.ServerVariables["REMOTE_ADDR"];
        string ipAddress = string.IsNullOrEmpty(ipAddress1) ? ipAddress2 : ipAddress1;
        return ipAddress;
    }

    protected void DatosLogin()
    {
        try
        {
            string IPP = GetDireccionIp(Request);

            string IP = Request.UserHostAddress;
            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spAccessLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Userkey", Value = User });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@IP", Value = IP });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    HttpContext.Current.Session["LogKey"] = Convert.ToInt16(rdr["LogKey"].ToString());
                }
                HttpContext.Current.Session["JWTKey"] = DateTime.Now.Ticks.ToString();
            }
        }
        catch(Exception ex)
        {
            string rest = ex.Message;
        }

    }



}