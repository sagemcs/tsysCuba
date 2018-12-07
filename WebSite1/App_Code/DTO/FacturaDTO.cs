using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using SAGE_Model;

using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

public class FacturaDTO
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public string Retenciones { get; set; }
    public string Traslados { get; set; }
    
    public string Descuento { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string UUID { get; set; }
    public string Usuario { get; set; }
    public string Estado { get; set; }
    public string Estado_Id { get; set; }
    public string Estado_Img { get; set; }

    public string Moneda { get; set; }

    public string Tipo { get; set; }
    public string Orden_de_Compra { get; set; }

    public List<NotaDTO> Notas { get; set; }
    
    public int VendKey { get; set; } 
    public string Contrarecibo_Folio { get; set; }

    public string Solicitud_Folio { get; set; }

    public FacturaDTO()
    { }

    public FacturaDTO(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();
        if(factura != null)
        {
            this.Key = Key;
            this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.UpdateDate != null ? factura.UpdateDate.Value.ToShortDateString() : "";
            this.Fecha_Recepcion = factura.UpdateDate != null ? Tools.ObtenerFechaRecepcion(Convert.ToDateTime(factura.UpdateDate)).ToShortDateString() : "";
            this.Proveedor = factura.Vendors.VendorID;
            this.Proveedor_Nombre = factura.Vendors.VendName;
            this.VendKey = factura.Vendors.VendorKey;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Retenciones = factura.ImpuestoImporteRtn != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteRtn), 2).ToString() : "0";
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";                
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;
            
            Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            this.Usuario = user != null ? user.UserID : string.Empty;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            //this.Estado_Img = status != null ? "<img src='/Img/" + status.Status.ToString() + ".png'>" : "<img src='/Img/0.png'>";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;
            this.Tipo = factura.TranType;


            if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
            {
                this.Contrarecibo_Folio = factura.InvcRcptDetails.First().InvoiceReceipt.Folio;
                if (factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail != null && factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.Count > 0)
                    this.Solicitud_Folio = factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.First().CheckRequest.Serie;
                else
                    this.Solicitud_Folio = "-";
            }
            else
            {
                this.Contrarecibo_Folio = "-";
                this.Solicitud_Folio = "-";
            }

            this.Notas = new List<NotaDTO>();
            if (this.Tipo == "IN")
            {
                foreach (Invoice nota in factura.Invoice1.Where(i => i.InvoiceKey != factura.InvoiceKey)) // Recorrido por notas de débito y crédito
                    this.Notas.Add(new NotaDTO(nota.InvoiceKey, factura.InvoiceKey));
            }
            else
                this.Notas = null;
        }
    }
}

public class NotaDTO : FacturaDTO
{

    public int ApplyToInvcKey { get; set; }

    public NotaDTO()
    { }

    public NotaDTO(int Key, int ApplyToInvcKey) :base(Key)
    {
        this.ApplyToInvcKey = ApplyToInvcKey;
    }

}


public class Facturas
{
 
    public Facturas()
    {
        

    }

    private static List<FacturaDTO> ObtenerDeTablaInvoice(Expression<Func<Invoice, bool>> predicate)
    {
        try
        {
            List<FacturaDTO> facturas = new List<FacturaDTO>();
            PortalProveedoresEntities db = new PortalProveedoresEntities();

            List<Invoice> list = db.Invoice.Where(predicate).ToList();

            foreach (Invoice invoice in list)
            {
                FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey);
                facturas.Add(factura);
            }
            return facturas;
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    private static List<FacturaDTO> Obtener(string document_type, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Invoice, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.TranType == document_type);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == document_type && VendorsIds.Contains(a.VendorKey));
            }
            
            return ObtenerDeTablaInvoice(predicate);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }
    public static List<FacturaDTO> ObtenerFacturas(bool directo_en_vista = false)
    {
        return Obtener("IN", directo_en_vista);
    }
    public static List<FacturaDTO> ObtenerNotasDeCredito()
    {
        return Obtener("CM");
    }
    public static List<FacturaDTO> ObtenerNotasDeDebito()
    {
        return Obtener("DM");
    }

    public static List<FacturaDTO> ObtenerFacturas(List<string> UUIDs)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Invoice, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.TranType == "IN" && UUIDs.Contains(a.UUID));
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == "IN" && a.CompanyID == company.CompanyID && UUIDs.Contains(a.UUID) && VendorsIds.Contains(a.VendorKey));
            }

            return ObtenerDeTablaInvoice(predicate);
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTO> ObtenerFacturasSinContrarecibo()
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Invoice, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.TranType == "IN" && a.InvcRcptDetails.Count == 0);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == "IN" && a.CompanyID == company.CompanyID && a.InvcRcptDetails.Count == 0 && VendorsIds.Contains(a.VendorKey));
            }

            List<FacturaDTO> facturas_sincontrarecibo = new List<FacturaDTO>();
            facturas_sincontrarecibo = ObtenerDeTablaInvoice(predicate);

            List<FacturaDTO> facturas = new List<FacturaDTO>();
            sage500_appEntities db_sage = new sage500_appEntities();

            foreach (FacturaDTO factura in facturas_sincontrarecibo)
                if (db_sage.tapPendVoucher.Where(p => p.CompanyID == factura.Compania && p.VendKey == factura.VendKey && p.TranNo == factura.Folio).FirstOrDefault() == null
                    && db_sage.tapVoucher.Where(p => p.CompanyID == factura.Compania && p.VendKey == factura.VendKey && p.TranNo == factura.Folio).FirstOrDefault() != null)
                    facturas.Add(factura);
            return facturas;
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }
    
    public static List<FacturaDTO> ObtenerFacturasSinContrarecibo(string VendorId, string Folio, string Serie, string Fecha, string Total, string UUID, string Estado)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturasSinContrarecibo();
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                double total = Convert.ToDouble(Total.Replace(",", "."));
                facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
            }
            if (!string.IsNullOrWhiteSpace(UUID) && UUID != "null")
                facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Fecha, string contrarecibo, string solicitud, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio!= "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(contrarecibo) && contrarecibo != "null")
                facturas = facturas.Where(f => f.Contrarecibo_Folio.ToUpper().Contains(contrarecibo.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(solicitud) && solicitud != "null")
                facturas = facturas.Where(f => f.Solicitud_Folio.ToUpper().Contains(solicitud.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }


    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Serie, string Fecha, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie!= "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(UUID))
                facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado))
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }
    
    public static void ActualizarEstadoFacturas(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            sage500_appEntities db_sage = new sage500_appEntities();
            PortalProveedoresEntities db = new PortalProveedoresEntities();

            for (int i = 0; i < db.Invoice.Where(f => f.CompanyID == company.CompanyID).ToList().Count(); i++)
            {
                var factura = db.Invoice.ToList()[i];
                if (factura.Status < 8) // No se analizan las facturas en proceso de pago o pagadas (¿o eliminadas?)
                {
                    if (db_sage.tapVoucher.Where(p => p.CompanyID == factura.CompanyID && p.VendKey == factura.VendorKey && p.TranNo == factura.Folio).FirstOrDefault() != null)
                        factura.Status = 4;
                    if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
                    {
                        factura.Status = 5;
                        if (factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail != null && factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.Count > 0)
                            factura.Status = 6;
                    }
                }
            }
            db.SaveChanges();
        }
        catch(Exception exp) {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
        }
    }
}

