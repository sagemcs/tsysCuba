using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Proveedores_Model;
using SAGE_Model;

/// <summary>
/// Summary description for ContrareciboDTO
/// </summary>
public class ContrareciboDTO
{
    public int Id { get; set; }
    public string Folio{ get; set; }
    public string Proveedor { get; set; }
    public string RFC { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Condiciones{ get; set; }
    public string Fecha_Programada_Pago { get; set; }
    //public List<FacturaDTO> facturas { get; set; }

    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public ContrareciboDTO()
    { }
    public ContrareciboDTO(int Key, string Folio, string Proveedor, string RFC, string Condiciones, string Fecha_Recepcion, string Fecha_Programada_Pago, string Total)
    {
        this.Id = Key;
        this.Folio = Folio;
        this.Proveedor = Proveedor;
        this.RFC = RFC;
        this.Condiciones = Condiciones;
        this.Fecha_Recepcion = Fecha_Recepcion;
        this.Fecha_Programada_Pago = Fecha_Programada_Pago;
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);

        PortalProveedoresEntities db = new PortalProveedoresEntities();
        List<Invoice> list = db.Invoice.ToList();
        Vendors vendor = db.Vendors.Where(v => v.VendorID == this.Proveedor).FirstOrDefault();
    }
}

public class Contrarecibos
{
    public Contrarecibos()
    { }

    public static List<ContrareciboDTO> ObtenerContrarecibos(bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<InvoiceReceipt, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<InvoiceReceipt> list = (sin_solicitud) ? db.InvoiceReceipt.Where(predicate).Where(r => r.ChkReqDetail.Count == 0).ToList() : db.InvoiceReceipt.Where(predicate).ToList();
            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();

            foreach (InvoiceReceipt contra in list.Where(a => a.CompanyID == company.CompanyID))
            {
                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(contra.Vendors.VendorID);

                string DBA = "Información de Proveedor no encontrada";
                short? condiciones = 0;
                if (proveedor != null)
                {
                    if (!string.IsNullOrWhiteSpace(proveedor.RFC))
                        DBA = proveedor.RFC;
                    condiciones = Convert.ToInt16(proveedor.Condiciones);
                }
                contrarecibos.Add(new ContrareciboDTO(contra.InvcRcptKey, contra.Folio.ToString(), contra.Vendors.VendorID, DBA, condiciones != null ? proveedor.Condiciones_Descripcion /*condiciones.ToString()*/ : "Información no encontrada", contra.RcptDate.ToShortDateString(), contra.PaymentDate.ToShortDateString(), Math.Round(contra.Total, 2).ToString()));
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO> ObtenerContrarecibos(List<int> Keys_list, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<InvoiceReceipt, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<InvoiceReceipt> list = (sin_solicitud) ? db.InvoiceReceipt.Where(predicate).Where(r => r.ChkReqDetail.Count == 0).ToList() : db.InvoiceReceipt.Where(predicate).ToList();

            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();

            foreach (InvoiceReceipt contra in list.Where(a => Keys_list.Contains(a.InvcRcptKey)))
            {
                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(contra.Vendors.VendorID);
                string DBA = "Información de Proveedor no encontrada";
                if (proveedor != null && !string.IsNullOrWhiteSpace(proveedor.RFC))
                    DBA = proveedor.RFC;
                contrarecibos.Add(new ContrareciboDTO(contra.InvcRcptKey, contra.Folio.ToString(), contra.Vendors.VendorID, DBA, proveedor != null ? proveedor.Condiciones_Descripcion : "Información no encontrada" /*contra.PaymentTerms*/, contra.RcptDate.ToShortDateString(), contra.PaymentDate.ToShortDateString(), Math.Round(contra.Total, 2).ToString()));
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO> ObtenerContrarecibos(string Folio, string Proveedor, string RFC, string Total, string Fecha, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            List<ContrareciboDTO> contrarecibos = ObtenerContrarecibos(sin_solicitud);

            if (!string.IsNullOrWhiteSpace(Folio) && Folio !="null")
            {
                contrarecibos = contrarecibos.Where(c => c.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    contrarecibos = contrarecibos.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Fecha_Recepcion == Fecha).ToList();
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }
    

    public static int GenerarContrarecibo(List<string> UUIDs, string rpt_path)
    {
        try
        {
            sage500_appEntities db_sage = new sage500_appEntities();

            string company_id = HttpContext.Current.Session["IDCompany"].ToString();
            tsmCompany company = db_sage.tsmCompany.Where(c => c.CompanyID == company_id).FirstOrDefault();
            if (company == null)
                throw new MulticonsultingException("No está autenticado por ninguna empresa");

            PortalProveedoresEntities db = new PortalProveedoresEntities();

            ReportDocument report_document = new ReportDocument();
            report_document.Load(rpt_path);
            List<Invoice> list = db.Invoice.Where(i => i.CompanyID == company.CompanyID && UUIDs.Contains(i.UUID)).ToList();
            if (list.Count > 1 && list.Where(l => l.UpdateDate.Value.Date != list.First().UpdateDate.Value.Date).FirstOrDefault() != null)
                throw new MulticonsultingException("Hay facturas que son de diferentes fechas"); // Son de diferentes fechas
            if (list.Count > 1 && list.Where(l => l.VendorKey != list.First().VendorKey).FirstOrDefault() != null)
                throw new MulticonsultingException("Hay facturas que son de diferentes proveedores"); // Son de diferentes proveedores
            List<FacturaDTO> facturas = new List<FacturaDTO>();
            List<NotaDTO> Notas = new List<NotaDTO>();
            decimal Total = 0;

            foreach (Invoice invoice in list)
            {
                try
                {
                    Int32 AprovUserKey = Convert.ToInt32(invoice.AprovUserKey);
                    Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
                    Total += invoice.Total;

                    //List<NotaDTO> Notas = new List<NotaDTO>();

                    bool sin_notas = true;

                    foreach (Invoice nota in invoice.Invoice1.Where(i => i.TranType == "CM" || i.TranType == "DM"))
                    {
                        sin_notas = false;
                        Total += nota.TranType == "CM" ? -(nota.Total) : nota.Total; // La nota afectando el valor del contrarecibo

                        NotaDTO Nota = new NotaDTO(nota.InvoiceKey, invoice.InvoiceKey);
                        //Nota.Total = "$ " + Nota.Total;
                        //Nota.Traslados = "$ " + Nota.Traslados;
                        //Nota.Subtotal = "$ " + Nota.Subtotal;
                        Nota.Tipo = "Nota de " + (nota.TranType == "CM" ? "Crédito" : "Débito");
                        Notas.Add(Nota);
                    }
                    if (sin_notas)
                    {
                        NotaDTO Nota_Vacia = new NotaDTO();
                        Nota_Vacia.ApplyToInvcKey = invoice.InvoiceKey;
                        Notas.Add(Nota_Vacia);
                    }

                    FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey);
                    //factura.Total = "$ " + factura.Total;
                    //factura.Traslados = "$ " + factura.Traslados;
                    //factura.Subtotal = "$ " + factura.Subtotal;

                    facturas.Add(factura);
                }
                catch { }
            }

            if (list.Count > 0)
            {
                int folio = 1;
                DateTime Fecha_Recepcion = DateTime.MinValue;
                DateTime Fecha_Programada_Pago = DateTime.MinValue;
                string Terminos_de_Pago = "Información de proveedor no encontrada";
                InvoiceReceipt ultimo_contrarecibo = db.InvoiceReceipt.OrderByDescending(c => c.CreateDate).FirstOrDefault();
                if (ultimo_contrarecibo != null)
                    folio = Convert.ToInt32(ultimo_contrarecibo.Folio) + 1;

                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(list.First().Vendors.VendorID);
                if (proveedor != null)
                {
                    Terminos_de_Pago = proveedor.Condiciones;
                    Fecha_Recepcion = Tools.ObtenerFechaRecepcion(Convert.ToDateTime(list.First().UpdateDate));
                    if (!string.IsNullOrWhiteSpace(Terminos_de_Pago))
                        Fecha_Programada_Pago = Tools.ObtenerFechaProgramadaPago(Fecha_Recepcion, Convert.ToInt32(proveedor.Condiciones));
                }
                else
                    throw new MulticonsultingException("No existe el proveedor"); // Vendor no encontrado en SAGE

                DateTime CreateDate = DateTime.Now;
                InvoiceReceipt nuevo_contrarecibo = new InvoiceReceipt()
                {
                    UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    VendorKey = list.First().Vendors.VendorKey,
                    CompanyID = company.CompanyID,
                    CreateDate = CreateDate,
                    CreateUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    UpdateDate = CreateDate,
                    UpdateUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    Folio = Tools.RellenoConCeros(folio),
                    RcptDate = Fecha_Recepcion,
                    PaymentDate = Fecha_Programada_Pago,
                    PaymentTerms = Terminos_de_Pago,
                    Total = Total
                };

                db.InvoiceReceipt.Add(nuevo_contrarecibo);
                db.SaveChanges();
                foreach (FacturaDTO factura in facturas)
                {
                    InvcRcptDetails details = new InvcRcptDetails()
                    {
                        InvcRcptKey = nuevo_contrarecibo.InvcRcptKey,
                        InvoiceKey = factura.Key,
                        UUID = factura.UUID,
                        Folio = factura.Folio,
                        Tipo = factura.Tipo,
                        Moneda = factura.Moneda,
                        IVA = Math.Round(Convert.ToDecimal(factura.Traslados), 2),
                        ISR = Math.Round(Convert.ToDecimal(factura.Retenciones), 2),
                        TotalTax = Convert.ToDecimal(factura.Retenciones) + Convert.ToDecimal(factura.Traslados),
                        Total = Convert.ToDecimal(factura.Total)
                    };
                    db.InvcRcptDetails.Add(details);
                }

                for (int i = 0; i < facturas.Count; i++)
                {
                    facturas[i].Total = "$ " + facturas[i].Total;
                    facturas[i].Traslados = "$ " + facturas[i].Traslados;
                    facturas[i].Subtotal = "$ " + facturas[i].Subtotal;
                }
                for (int i = 0; i < Notas.Count; i++)
                {
                    Notas[i].Total = "$ " + Notas[i].Total;
                    Notas[i].Traslados = "$ " + Notas[i].Traslados;
                    Notas[i].Subtotal = "$ " + Notas[i].Subtotal;
                }

                report_document.Database.Tables[0].SetDataSource(facturas);
                report_document.Database.Tables[1].SetDataSource(Notas);

                report_document.SetParameterValue("contrarecibo_no", nuevo_contrarecibo.Folio.ToString());
                report_document.SetParameterValue("razon_social_compannia", company.CompanyName);
                report_document.SetParameterValue("rfc", company.FedID);
                report_document.SetParameterValue("fecha_pago", nuevo_contrarecibo.PaymentDate.ToShortDateString());
                report_document.SetParameterValue("fecha_datos", Tools.FechaEnEspañol(nuevo_contrarecibo.CreateDate));
                report_document.SetParameterValue("proveedor", proveedor.Social);
                report_document.SetParameterValue("total", Total);

                Stream stream = report_document.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                BinaryReader br = new BinaryReader(stream);

                InvcRcptFile invcRcptFile = new InvcRcptFile()
                {
                    InvcRcptkey = nuevo_contrarecibo.InvcRcptKey,
                    FileType = "pdf",
                    FileBinary = br.ReadBytes(Convert.ToInt32(stream.Length)),
                    Counter = (db.InvcRcptFile.Count() + 1).ToString()
                };
                db.InvcRcptFile.Add(invcRcptFile);
                db.SaveChanges();

                bool Envio;
                Envio = Global.EmailGlobalAdd(proveedor.Correo, "Se adjunta fichero de contrarecibo", "Contrarecibo T|SYS|", stream, "Contrarecibo.pdf");
                //if (Envio == false)
                //{
                //    Facturas.ActualizarEstadoFacturas();
                //    throw new MulticonsultingException("Contra recibo generado, error al enviar comprobante");
                //}
                //else
                //{
                //    Facturas.ActualizarEstadoFacturas();
                //}

                //CorreoElectronico correo = new CorreoElectronico(ConfiguracionCorreoElectronico.server_address,
                //      ConfiguracionCorreoElectronico.server_port,
                //      ConfiguracionCorreoElectronico.user,
                //      ConfiguracionCorreoElectronico.password);
                //correo.Enviar(company.EMailAddr, proveedor.Correo, "Contrarecibo", "Se adjunta fichero de contrarecibo", false, stream, "Contrarecibo.pdf");

                Facturas.ActualizarEstadoFacturas();

                return nuevo_contrarecibo.InvcRcptKey;
            }
        }
        catch (Exception e)
        {
            throw new MulticonsultingException(e.Message, e.InnerException); 
        }
        return -1;
    }    
}