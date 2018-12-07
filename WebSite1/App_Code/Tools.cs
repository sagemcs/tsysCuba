using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CrystalDecisions.ReportAppServer.ReportDefModel;
using Proveedores_Model;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Text;

/// <summary>
/// Summary description for Tools
/// </summary>
public static class Tools
{
    public static bool EsTokenValido(string supuesto_token)
    {
        try
        {
            var payload = new Dictionary<string, object>{
                    { "iss", ConfigurationManager.AppSettings["as:Issuer"]},
                    { "aud", ConfigurationManager.AppSettings["as:AudienceId"]}
                };
            //var secret = ConfigurationManager.AppSettings["as:AudienceSecret"];
            var secret =  HttpContext.Current.Session["JWTKey"].ToString();
            IJwtEncoder encoder = new JwtEncoder(new HMACSHA256Algorithm(), new JsonNetSerializer(), new JwtBase64UrlEncoder());

            string real_token = "Bearer " + encoder.Encode(payload, secret);

            if (supuesto_token == real_token)
                return true;
        }
        catch { }
        return false;
    }
    public static string GetDocumentStatusDescription(int id)
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            StatusDocs status = db.StatusDocs.Where(s => s.Status == id).FirstOrDefault();
            return status != null ? status.Descripcion : "No definido";
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetDocumentStatusDescription(string id)
    {
        try
        {
            return GetDocumentStatusDescription(Convert.ToInt32(id));
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetUserStatusDescription(int id)
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            StatusUsers status = db.StatusUsers.Where(s => s.Status == id).FirstOrDefault();
            return status != null ? status.Descripcion : "No definido";
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetUserStatusDescription(string id)
    {
        try
        {
            return GetUserStatusDescription(Convert.ToInt32(id));
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static Company EmpresaAutenticada()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            if (HttpContext.Current.Session["IDCompany"] == null)
                return null;
            string company_id = HttpContext.Current.Session["IDCompany"].ToString();
            return db.Company.Where(c => c.CompanyID == company_id).FirstOrDefault();
        }
        catch { }
        return null;
    }

    public static Users UsuarioAutenticado()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            if (HttpContext.Current.Session["UserKey"] == null)
                return null;
            int user_key = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            return db.Users.Where(c => c.UserKey == user_key).FirstOrDefault();
        }
        catch { }
        return null;
    }

    public static string GetMasterPage()
    {
        try
        {
            if (UsuarioAutenticado() != null)
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    return "~/Logged/Proveedores/MenuPreP.Master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                {
                    return "~/Site.Master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                {
                    return "~/Logged/Administradores/MasterPageContb.master";
                }
        }
        catch { }
        return "~/Logged/Proveedores/MenuPreP.Master";
    }

    public static bool EsProveedor()
    {
        try
        {
            if ((UsuarioAutenticado() != null) && ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")))
                return true;
            return false;
        }
        catch
        { }
        return true;
    }

    public static List<string> Mensaje_Estado_de_Facturas()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {
                    if (vendor.Invoice != null && vendor.Invoice.Count > 0)
                    {
                        int facturas_pendientes = vendor.Invoice.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (facturas_pendientes > 0)
                            mensajes.Add("Tiene " + (facturas_pendientes == 1 ? " una factura pendiente" : facturas_pendientes.ToString() + " facturas pendientes"));
                        int facturas_canceladas = vendor.Invoice.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (facturas_canceladas > 0)
                            mensajes.Add("Tiene " + (facturas_canceladas == 1 ? " una factura cancelada" : facturas_canceladas.ToString() + " facturas canceladas"));
                        int facturas_eliminadas = vendor.Invoice.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (facturas_eliminadas > 0)
                            mensajes.Add("Tiene " + (facturas_eliminadas == 1 ? " una factura eliminada" : facturas_eliminadas.ToString() + " facturas eliminadas"));
                        int facturas_complemento_pago_pendiente = vendor.Invoice.Where(f => f.Status == 7).AsEnumerable().Count();
                        if (facturas_complemento_pago_pendiente > 0)
                            //mensajes.Add("Tiene " + (facturas_complemento_pago_pendiente == 1 ? " una factura eliminada" : facturas_complemento_pago_pendiente.ToString() + " facturas eliminadas"));
                            mensajes.Add("Tiene complementos de pago pendientes por subir");
                    }

                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int facturas_pendientes = db.Invoice.Where(f => f.Status == 1).AsEnumerable().Count();
                if (facturas_pendientes > 0)
                    mensajes.Add("Existe" + (facturas_pendientes == 1 ? " una factura pendiente" : "n " + facturas_pendientes.ToString() + " facturas pendientes"));
            }
            catch
            { }
        return mensajes;
    }

    public static List<string> Mensaje_Estado_de_Pagos()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {           
                    if (vendor.Payment != null && vendor.Payment.Count > 0)
                    {
                        int pagos_pendientes = vendor.Payment.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (pagos_pendientes > 0)
                            mensajes.Add("Tiene " + (pagos_pendientes == 1 ? " un pago pendiente" : pagos_pendientes.ToString() + " pagos pendientes"));
                        int pagos_cancelados = vendor.Payment.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (pagos_cancelados > 0)
                            mensajes.Add("Tiene " + (pagos_cancelados == 1 ? " un pago cancelado" : pagos_cancelados.ToString() + " pagos cancelados"));
                        int pagos_eliminados = vendor.Payment.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (pagos_eliminados > 0)
                            mensajes.Add("Tiene " + (pagos_eliminados == 1 ? " un pago eliminado" : pagos_eliminados.ToString() + " pagos eliminados"));
                        
                    }
                   
                }
            }
            catch
            {
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int pagos_pendientes = db.Payment.Where(f => f.Status == 1).AsEnumerable().Count();
                if (pagos_pendientes > 0)
                    mensajes.Add("Existe" + (pagos_pendientes == 1 ? " un pago pendiente" : "n " + pagos_pendientes.ToString() + " pagos pendientes"));
            }
            catch
            { }

        return mensajes;
    }
    
    public static List<string> Mensaje_Estado_de_Documentos_de_Proveedor()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {
                   
                    if (vendor.Documents != null && vendor.Documents.Count > 0)
                    {
                        int documentos_pendientes = vendor.Documents.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (documentos_pendientes > 0)
                            mensajes.Add("Tiene " + (documentos_pendientes == 1 ? " un documento pendiente" : documentos_pendientes.ToString() + " documentos pendientes"));
                        int documentos_cancelados = vendor.Documents.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (documentos_cancelados > 0)
                            mensajes.Add("Tiene " + (documentos_cancelados == 1 ? " un documento cancelado" : documentos_cancelados.ToString() + " documentos eliminados"));
                        int documentos_eliminados = vendor.Documents.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (documentos_eliminados > 0)
                            mensajes.Add("Tiene " + (documentos_eliminados == 1 ? " un documento eliminado" : documentos_eliminados.ToString() + " documentos eliminados"));
                    }
                }
            }
            catch
            {
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int documentos_pendientes = db.Documents.Where(f => f.Status == 1).AsEnumerable().Count();
                if (documentos_pendientes > 0)
                    mensajes.Add("Existe" + (documentos_pendientes == 1 ? " un documento de proveedor pendiente" : "n " + documentos_pendientes.ToString() + " documentos de proveedor pendientes"));
            }
            catch
            { }

        return mensajes;
    }

    public static string ObtenerFechaEnFormato(string fecha)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(fecha))
            {
                string[] camps = fecha.Split('/');
                DateTime date = new DateTime(Convert.ToInt32(camps[2]), Convert.ToInt32(camps[1]), Convert.ToInt32(camps[0]));
                fecha = date.ToShortDateString();
            }
        }
        catch
        { }
        return fecha;
    }

    public static string RellenoConCeros(int número, int dígitos = 5)
    {
        string rellenado = número.ToString();
        while (rellenado.Length < dígitos)
            rellenado = "0" + rellenado;
        return rellenado;
    }
    
    public static string FechaEnEspañol(DateTime date)
    {
        string fecha = string.Empty;

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                fecha = "Lunes";
                break;
            case DayOfWeek.Tuesday:
                fecha = "Martes";
                break;
            case DayOfWeek.Wednesday:
                fecha = "Miércoles";
                break;
            case DayOfWeek.Thursday:
                fecha = "Jueves";
                break;
            case DayOfWeek.Friday:
                fecha = "Viernes";
                break;
            case DayOfWeek.Saturday:
                fecha = "Sábado";
                break;
            case DayOfWeek.Sunday:
                fecha = "Domingo";
                break;
        }

        fecha += ", " + (date.Day == 1 ? "1ro" : date.Day.ToString()) + " de ";

        switch (date.Month)
        {
            case 1:
                fecha += "Enero";
                break;
            case 2:
                fecha += "Febrero";
                break;
            case 3:
                fecha += "Marzo";
                break;
            case 4:
                fecha += "Abril";
                break;
            case 5:
                fecha += "Mayo";
                break;
            case 6:
                fecha += "Junio";
                break;
            case 7:
                fecha += "Julio";
                break;
            case 8:
                fecha += "Agosto";
                break;
            case 9:
                fecha += "Septiembre";
                break;
            case 10:
                fecha += "Octubre";
                break;
            case 11:
                fecha += "Noviembre";
                break;
            case 12:
                fecha += "Diciembre";
                break;
        }

        fecha += " del " + date.Year;

        return fecha;
    }

    public static DateTime ObtenerFechaProgramadaPago(DateTime FechaRecepcion, int CondicionesPago)
    {
        FechaRecepcion = FechaRecepcion.AddDays(CondicionesPago);
        while (FechaRecepcion.DayOfWeek != DayOfWeek.Monday)
            FechaRecepcion = FechaRecepcion.AddDays(1);
        return FechaRecepcion; // Ya en este momento es la fecha Programada de Pago
    }

    public static DateTime ObtenerFechaRecepcion(DateTime FechaTransacción)
    {
        while (FechaTransacción.DayOfWeek != DayOfWeek.Monday)
            FechaTransacción = FechaTransacción.AddDays(1);
        return FechaTransacción;
    }
    private static SqlConnection SqlConnectionDB(string cnx)
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
    public static void LogError(String proceso, String mensaje)
    {
        try
        {
           
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"]);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            string pCompanyID = HttpContext.Current.Session["IDCompany"].ToString();

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", pLogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", pUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", pCompanyID));

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

}