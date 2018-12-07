using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proveedores_Model;

/// <summary>
/// Summary description for ErrorDTO
/// </summary>
public class ErrorDTO
{
    public string Usuario { get; set; }
    public string Compania { get; set; }
    public string Fecha { get; set; }
    public string Comentario { get; set; }

    public ErrorDTO()
    { }

    public ErrorDTO(string Usuario, string Compañía, string Fecha, string Comentario)
    {
        this.Usuario = Usuario;
        this.Compania = Compañía;
        this.Fecha = Fecha;
        this.Comentario = Comentario;
    }
}

public class Errores
{
    public Errores()
    {
    }

    public static List<ErrorDTO> ObtenerErrores(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            //throw new MulticonsultingException("No está autenticado por ninguna empresa");
            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;
            //throw new MulticonsultingException("No está autenticado");

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;
            //throw new MulticonsultingException("Acceso Denegado");

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<ErrorLog> list = db.ErrorLog.ToList();
            List<ErrorDTO> errores = new List<ErrorDTO>();

            foreach (ErrorLog error in list.Where(a => a.CompanyID == company.CompanyID))
                errores.Add(new ErrorDTO(error.Users.UserID, error.Users.Company.FirstOrDefault() != null ? error.Users.Company.First().CompanyID : string.Empty, error.ErrorDate != null ? error.ErrorDate.ToShortDateString() : string.Empty, error.Message));

            return errores;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ErrorDTO>();
        }
    }
}