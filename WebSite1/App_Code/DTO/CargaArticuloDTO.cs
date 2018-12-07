using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGE_Model;
using Proveedores_Model;


/// <summary>
/// Summary description for CargaArticuloDTO
/// </summary>
public class CargaArticuloDTO
{
    public string Articulo { get; set; }
    public string Cantidad { get; set; }
    public string Costo { get; set; }
    public string Monto { get; set; }
    public string Comentario { get; set; }
    public string Estado { get; set; }

    public CargaArticuloDTO()
    {
       
    }

    public CargaArticuloDTO(string Articulo, string Cantidad, string Costo, string Monto, string Comentario, string Status)
    {
        this.Articulo = Articulo;
        this.Cantidad = Cantidad;
        this.Costo = Costo;
        this.Monto = Monto;
        this.Comentario = Comentario;
        this.Estado = Status;
    }
    
}


public class CargaArticulo
{
    public CargaArticulo()
    {
    }

    public static List<CargaArticuloDTO> ObtenerCargaArticulos(bool directo_en_vista = false)
    {
        try
        {
            #region ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;

            #endregion ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<CargaArticuloDTO> list_dto = new List<CargaArticuloDTO>();
            List<SAGE_Model.tapLoadFilesAPDtl> list = db_sage.tapLoadFilesAPDtl.ToList();

            foreach (var item in list)
            {
                tapLFStatus status = item.status == null ? null : db.tapLFStatus.Where(s => s.Status == item.status).FirstOrDefault();
                list_dto.Add(new CargaArticuloDTO(item.ItemID, item.Qty != null ? item.Qty.ToString() : "0", item.UnitCost != null ? Math.Round(Convert.ToDecimal(item.UnitCost), 2).ToString() : "0",
                item.TranAmt != null ? Math.Round(Convert.ToDecimal(item.TranAmt), 2).ToString() : "0", item.DescError, status != null ? status.Descripcion : "No definido"));
            }

            return list_dto;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaArticuloDTO>();
        }
    }



    public static List<CargaArticuloDTO> ObtenerCargaArticulos(string ItemID, string Qty, string Status, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Status) && Status == "0")
                Status = "";

            List<CargaArticuloDTO> items = ObtenerCargaArticulos();

            if (items == null)
                return null;

            if (!string.IsNullOrWhiteSpace(ItemID) && ItemID != "null")
                items = items.Where(a => a.Articulo.ToUpper().Contains(ItemID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Qty) && Qty != "null")
                items = items.Where(a => a.Cantidad == Qty).ToList();
            if (!string.IsNullOrWhiteSpace(Status) && Status != "null")
                items = items.Where(a => a.Estado == Status).ToList();
            return items;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaArticuloDTO>();
        }
    }

}