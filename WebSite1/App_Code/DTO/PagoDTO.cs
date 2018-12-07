using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proveedores_Model;

public class PagoDTO
{
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public string Proveedor { get; set; }
    public string Subtotal { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string UUID { get; set; }
    public string Usuario { get; set; }
    public string Estado { get; set; }

    public PagoDTO()
    { }

    public PagoDTO(string Folio, string Serie, string Fecha, string Proveedor, string Subotal, string Total, string UUID, string Usuario, string Estado)
    {
        this.Folio = Folio;
        this.Serie = Serie;
        this.Fecha = Fecha;
        this.Proveedor = Proveedor;
        this.Subtotal = Subotal;
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);
        this.UUID = UUID;
        this.Usuario = Usuario;
        this.Estado = Estado;
    }
}

public class Pagos
{

    public Pagos()
    {
    }

    public static List<PagoDTO> ObtenerPagos(bool directo_en_vista = false)
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
            List<Payment> list = db.Payment.ToList();
            List<PagoDTO> pagos = new List<PagoDTO>();
            foreach (Payment pago in list.Where(a => a.CompanyID == company.CompanyID))
            {
                int vendor_key = Convert.ToInt32(pago.VendorKey);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                Int32 AprovUserKey = Convert.ToInt32(pago.AprovUserKey);
                Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
                StatusDocs status = null;
                status = db.StatusDocs.Where(s => s.Status == company.Status).FirstOrDefault();
                pagos.Add(new PagoDTO(pago.Folio, pago.Serie, pago.PaymentDate != null ? pago.PaymentDate.ToShortDateString() : string.Empty, vendor != null ? vendor.VendorID : string.Empty, pago.Subtotal != null ? Math.Round(Convert.ToDecimal(pago.Subtotal), 2).ToString() : "0",
                  Math.Round(Convert.ToDecimal(pago.Total), 2).ToString(), pago.UUID, user != null ? user.UserID : string.Empty, status != null ? status.Descripcion : "No definido"));
            }

            return pagos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<PagoDTO>();
        }
    }

    public static List<PagoDTO> ObtenerPagos(string Folio, string Serie, string Fecha, string Proveedor, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<PagoDTO> pagos = ObtenerPagos();

            if (pagos == null)
                return null;

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                pagos = pagos.Where(p => p.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                pagos = pagos.Where(p => p.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                pagos = pagos.Where(p => p.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                pagos = pagos.Where(p => p.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    pagos = pagos.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(UUID) && UUID != "null")
                pagos = pagos.Where(p => p.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                pagos = pagos.Where(p => p.Estado == Estado).ToList();
            }

            return pagos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<PagoDTO>();
        }
    }

}

