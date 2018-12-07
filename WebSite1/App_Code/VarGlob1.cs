using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;


/// <summary>
/// Contains my site's global variables.
/// </summary>
public static class Global
{
    /// <summary>
    /// Global variable storing important stuff.
    /// </summary>
    static string _importantData;

    /// <summary>
    /// Get or set the static important data.
    /// </summary>
    public static string ImportantData
    {
        get
        {
            return _importantData;
        }
        set
        {
            _importantData = value;
        }
    }

    public static void Docs()
    {   
        string User = HttpContext.Current.Session["UserKey"].ToString();

        using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("spDocsU", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Key",
                Value = User
            });

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
             HttpContext.Current.Session["Docs"] = rdr["Docs"].ToString();
            }
        }   

    }

    public static void VarSesion(string user,string Empresa)
    {

        HttpContext.Current.Session["VendKey"] = "";
        HttpContext.Current.Session["UserKey"] = "";
        HttpContext.Current.Session["IDCompany"] = "";
        HttpContext.Current.Session["IDComTran"] = "";
        HttpContext.Current.Session["RolUser"] = "";
        HttpContext.Current.Session["Status"] = "";

        try
        {
            using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectVarGlobal", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ususario", Value = user });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Empresa", Value = Empresa });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (HttpContext.Current.Session["IDCompany"].ToString() == "")
                    {
                     HttpContext.Current.Session["IDCompany"] = rdr["IDCompany"].ToString();
                    }
                    HttpContext.Current.Session["VendKey"] = rdr["VendKey"].ToString();
                    HttpContext.Current.Session["UserKey"] = rdr["UserKey"].ToString();
                    HttpContext.Current.Session["IDComTran"] = rdr["IDCompany"].ToString();
                    HttpContext.Current.Session["RolUser"] = rdr["Rol"].ToString();
                    HttpContext.Current.Session["Status"] = rdr["Status"].ToString();
                }

                conn.Close();
                //KeyLog();
            }
        }
        catch (Exception b)
        {

        }
    }

    public static void LoginStatus(string user)
    {
        HttpContext.Current.Session["Status"] = "";

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SpLoginStatus", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ususario", Value = user });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                  HttpContext.Current.Session["Status"] = rdr["Status"].ToString();
                }

                conn.Close();
            }
        }
        catch (Exception b)
        {

        }


    }

    public static void KeyLog()
    {
        int Vkey = 0;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("Select MAX(LogKey) As Fkey From AccessLog", conn);

                if (conn.State == ConnectionState.Open)
                { conn.Close(); }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Vkey = Convert.ToInt16(rdr["Fkey"].ToString());
                }
            }

            HttpContext.Current.Session["LogKey"] = Vkey;

        }
        catch (Exception ex)
        {

        }
    }

    public static bool EmailGlobal(string Destinatario, string Body, string Subjet)
    {
        bool rest = false;
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
                PassPortal = PassEm.Replace(" ","");
                PPortal = Puerto1;
                puerto = Convert.ToInt32(PPortal);
                

                //CORREO
                MailMessage Correo = new MailMessage();
                Correo.From = new System.Net.Mail.MailAddress(Emailportal);
                Correo.To.Add(Destinatario);
                Correo.Subject = (Subjet);
                Correo.Body = Body;
                Correo.Priority = System.Net.Mail.MailPriority.Normal;
                Correo.IsBodyHtml = true;

                //SMPT
                System.Net.Mail.SmtpClient ServerMail = new System.Net.Mail.SmtpClient();
                ServerMail.Port = Convert.ToInt16(puerto);
                ServerMail.Host = smtp;

                if (PassPortal != "")
                {
                    ServerMail.Credentials = new NetworkCredential(Emailportal, PassPortal);
                    ServerMail.EnableSsl = true;
                }
                else
                {
                    ServerMail.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                try
                {
                    ServerMail.Send(Correo);
                    rest = true;
                }
                catch (Exception ex)
                {
                    ServerMail.Dispose();
                    int SesKey = Convert.ToInt16(HttpContext.Current.Session["LogKey"].ToString());
                    int UsKey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
                    string Company = HttpContext.Current.Session["IDCompany"].ToString();
                    LogError(SesKey, UsKey, "Error al enviar correo electronico", ex.Message, Company);
                }

            }
        }

        catch(Exception b)
        {
            int SesKey = Convert.ToInt16(HttpContext.Current.Session["LogKey"].ToString());
            int UsKey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(SesKey, UsKey, "Error al cargar la configuración del correo electronico", b.Message, Company);
        }
        return rest;
    }

    public static bool EmailGlobalAdd(string Destinatario, string Body, string Subjet,Stream File,string FileName)
    {
        bool rest = false;
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
                PassPortal = PassEm.Replace(" ", "");
                PPortal = Puerto1;
                puerto = Convert.ToInt32(PPortal);

                //Memory
                File.Seek(0, SeekOrigin.Begin);
                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Application.Octet;
                contentType.Name = FileName;
                Attachment attachment = new Attachment(File, contentType);

                //CORREO
                MailMessage Correo = new MailMessage();
                Correo.From = new System.Net.Mail.MailAddress(Emailportal);
                Correo.To.Add(Destinatario);
                Correo.Subject = (Subjet);
                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);
                Correo.Attachments.Add(attachment);
                Correo.Body = Body;
                Correo.Priority = System.Net.Mail.MailPriority.Normal;
                Correo.IsBodyHtml = true;

                //SMPT
                System.Net.Mail.SmtpClient ServerMail = new System.Net.Mail.SmtpClient();
                ServerMail.Port = Convert.ToInt16(puerto);
                ServerMail.Host = smtp;

                if (PassPortal != "")
                {
                    ServerMail.Credentials = new NetworkCredential(Emailportal, PassPortal);
                    ServerMail.EnableSsl = true;
                }
                else
                {
                    ServerMail.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                try
                {
                    ServerMail.Send(Correo);
                    rest = true;
                }
                catch (Exception ex)
                {
                    ServerMail.Dispose();
                    int SesKey = Convert.ToInt16(HttpContext.Current.Session["LogKey"].ToString());
                    int UsKey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
                    string Company = HttpContext.Current.Session["IDCompany"].ToString();
                    LogError(SesKey, UsKey, "Error al enviar correo electronico", ex.Message, Company);
                }

            }
        }

        catch (Exception b)
        {
            int SesKey = Convert.ToInt16(HttpContext.Current.Session["LogKey"].ToString());
            int UsKey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(SesKey, UsKey, "Error al cargar la configuración del correo electronico", b.Message, Company);
        }
        return rest;
    }

    //Rutina Manejar Errores
    public static void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                foreach (System.Data.SqlClient.SqlParameter par in parsT)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                System.Data.SqlClient.SqlDataReader rdr = null;

                rdr = Cmd.ExecuteReader();

                while (rdr.Read())
                {
                    val1 = rdr.GetInt32(0); // 0 ok
                }

                sqlConnection1.Close();

            }


        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;

        }
    }

    //Rutina de Conexión
    public static SqlConnection SqlConnectionDB(string cnx)
    {
        try
        {
            SqlConnection SqlConnectionDB = new SqlConnection();
            ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings[cnx];
            if ((connSettings != null) && (connSettings.ConnectionString != null))
            {
                SqlConnectionDB.ConnectionString = ConfigurationManager.ConnectionStrings[cnx].ConnectionString;
            }

            return SqlConnectionDB;
        }
        catch (Exception ex)
        {

            return null;
        }
    }

}