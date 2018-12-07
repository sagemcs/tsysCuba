using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proveedores_Model;

/// <summary>
/// Summary description for AccesoDTO
/// </summary>
public class AccesoDTO
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Nombre { get; set; }
    public string Compania { get; set; }
    public string Fecha { get; set; }
    public string IP { get; set; }

    public AccesoDTO()
    { }

    public AccesoDTO(int Key, string Usuario, string Nombre, string Compania, string Fecha, string IP)
    {
        this.Id = Key;
        this.Usuario = Usuario;
        this.Nombre = Nombre;
        this.Compania = Compania;
        this.Fecha = Fecha;
        this.IP = IP;
    }
}

public class Accesos
{
    public Accesos()
    {
    }

    public static List<AccesoDTO> ObtenerAccesos(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;

            #region ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;

            #endregion ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<AccessLog> list = db.AccessLog.ToList();
            List<AccesoDTO> accesos = new List<AccesoDTO>();

            foreach (AccessLog access in list.Where(a => a.CompanyID == company.CompanyID))
                accesos.Add(new AccesoDTO(access.LogKey, access.Users.UserID, access.Users.UserName, access.Users.Company.FirstOrDefault() != null ? access.Users.Company.First().CompanyName : string.Empty, access.LoginDate != null ? access.LoginDate.ToShortDateString() : string.Empty, access.IP));

            return accesos;
        }
        catch(Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<AccesoDTO>();
        }
    }

    public static List<AccesoDTO> ObtenerAccesos(string Usuario, string Nombre, string IP, bool directo_en_vista = false)
    {
        try
        {  
            List<AccesoDTO> accesos = ObtenerAccesos();

            if (accesos == null)
                return null;

            if (!string.IsNullOrWhiteSpace(Usuario) && Usuario != "null")
                accesos = accesos.Where(a => a.Usuario.ToUpper().Contains(Usuario.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Nombre) && Nombre != "null")
                accesos = accesos.Where(a => a.Nombre.ToUpper().Contains(Nombre.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(IP) && IP != "null")
                accesos = accesos.Where(a => a.IP == IP).ToList();
            return accesos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<AccesoDTO>();
        }
    }
}