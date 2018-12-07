using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class _Default : Page
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
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                  RevisaPagos();
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

            Global.Docs();

            if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
            {
                Page.MasterPageFile = "MenuP.master";
            }

            else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
            {
                string titulo, Msj, tipo;
                int Dias = RevDocs();
                if (Dias == 0)
                {
                    HttpContext.Current.Session["UpDoc"] = "0";
                    Page.MasterPageFile = "MenuPreP.master";
                }
                else if (Dias == 11)
                {
                    HttpContext.Current.Session["UpDoc"] = "1";
                    tipo = "error";
                    Msj = "Has superado el tiempo límite para la actualización de Opinión de Cumplimiento de Obligaciones Fiscales, Deberás cargarlo cuanto antes para seguir teniendo acceso a las actividades del portal";
                    titulo = "Notificación de actualización de documentos";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    Page.MasterPageFile = "MenuP.master";
                }
                else if (Dias == -1)
                {
                    tipo = "error";
                    Msj = "Error el Obtener datos";
                    titulo = "Notificación de actualización de documentos";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    HttpContext.Current.Session["UpDoc"] = "0";
                    tipo = "warning";
                    Msj = "Te recordamos tienes " + Dias + " para actualizar tu Opinión de Cumplimiento de Obligaciones Fiscales, ya que de lo contrario no podrás tener acceso a las actividades del portal";
                    titulo = "Notificación de actualización de documentos";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    Page.MasterPageFile = "MenuPreP.master";
                }
                
            }
        }
        catch(Exception ex)
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

    protected void RevisaPagos()
    {
        int VendK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
        string IDCom = HttpContext.Current.Session["IDCompany"].ToString();
        //int Logkey = Convert.ToInt16(HttpContext.Current.Session["LogKey"].ToString());
        int Logkey = 1;
        int Uskey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
        int Cont = 0;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = VendK });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = IDCom });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Fech = string.Empty;
                    string Pago = string.Empty;
                    string Fac = string.Empty;
                    string Cta = string.Empty;
                    string Banco = string.Empty;
                    string RFCB = string.Empty;

                    Fech = (rdr["FechaPago"].ToString());
                    Pago = (rdr["Monto"].ToString());
                    Fac = (rdr["Factura"].ToString());
                    Cta = (rdr["NoCta"].ToString());
                    Banco = (rdr["Banco"].ToString());
                    RFCB = (rdr["RFC_Banco"].ToString());

                    string Msj = string.Empty;
                    string Destinartario = User.Identity.Name.ToString();
                    Msj = EmailPagos(Destinartario, Fech, Pago, Fac, Cta, Banco, RFCB).ToString();
                    if (Msj == "Ok")
                    {
                        Update(VendK, IDCom, Fac);
                        Cont = Cont + 1;
                    }
                    else
                    {
                        LogError(Logkey, Uskey, "Notificación de Pago", Msj, IDCom);
                    }
                }
            }
            if (Cont >= 1)
            {
                string titulo, Msj, tipo;
                if (Cont == 1) { Msj = "Se ha realizado un nuevo pago a tu cuenta, Revisa tu correo en el encontraras los datos para a generación del Complemento de pago"; }
                else { Msj = "Se han realizado nuevos pagos a tu cuenta, Revisa tu correo en el encontraras los datos para a generación del Complemento de pago "; }

                tipo = "success";
                titulo = "Notificación de Pago";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch (Exception ex)
        {
            LogError(Logkey, Uskey, "Notificación de Pago", ex.Message, IDCom);
        }
    } 

    private string EmailPagos(string Destinatario,string Fecha, string Monto, string Fac, string Cta, string Bank, string RFC)
    {
        string Resut = "Error";
        try
        {

          string body = string.Empty;

          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/NPagos.html")))
          {
            body = reader.ReadToEnd();
            body = body.Replace("{Date_Fecha}", Fecha);
            body = body.Replace("{Date_Monto}", Monto);
            body = body.Replace("{Date_Fac}", Fac);
            body = body.Replace("{Date_Cta}", Cta);
            body = body.Replace("{Date_Banco}", Bank);
            body = body.Replace("{Date_RFC}", RFC);
         }
          if (Global.EmailGlobal(Destinatario, body, "NOTIFICACIÓN DE PAGO") == true) { Resut = "Ok"; }
        }
        catch (Exception b)
        {
            Resut = b.Message;
        }
        return Resut;
    }

    private void Update(int vendK, string Company,string Folio)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = vendK });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = Company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = Folio });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception Bv)
        {


        }
    }

    private int RevDocs()
    {
        int Msj;
        try
        {
            string sql;
            string Cuenta;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendk = HttpContext.Current.Session["VendKey"].ToString();
            string company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();
            sql = @"Select UpdateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 
            DateTime UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización

            if (Hoy >= UpdateL)
            {
                if (UpdateD(Vendk, company) == true)
                {Msj = 11;} else {Msj = -1; }           
            }
            else
            {
                DateTime DateDays = UpdateL.AddDays(-10);
                TimeSpan Days = UpdateL - Hoy;
                int Dias = Days.Days;
                if (Hoy >= DateDays)
                {
                    if (UpdateD(Vendk, company) == true)
                    { Msj = Dias;}
                    else { Msj = -1; }   
                }
                else
                {
                    Msj = 0;
                }
           }
          sqlConnection1.Close();
        }
        catch (Exception)
        {
            Msj = -1;
        }
        return Msj;
    }

    private bool UpdateD(string Vendk,string Company)
    {
        bool ret = false;
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE Documents SET Status = 1 where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + Company + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
                ret = true;
            }

           
        }
        catch
        {

        }
        return ret;
    }

    private void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
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