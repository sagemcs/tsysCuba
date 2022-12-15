//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SAGE_Model;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

public class FacturaDTO
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateR { get; set; }
    public DateTime DateA { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Double { get; set; }
    public string Retenciones { get; set; }
    public double Retenciones_In_Double { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }
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
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.Date = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateR = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.DateA = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            //this.Date = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(Fecha_Recepcion);
            //this.DateRx = string.IsNullOrWhiteSpace(this.Fecha_Recepcion) ? DateTime.MinValue : Convert.ToDateTime(this.Fecha_Recepcion);
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.Proveedor_Nombre = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.VendKey = factura.Vendors != null ? factura.Vendors.VendorKey : -1;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Retenciones = factura.ImpuestoImporteRtn != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteRtn), 2).ToString() : "0";
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";                
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Subtotal_In_Double = Convert.ToDouble(factura.Subtotal);
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Retenciones_In_Double = Convert.ToDouble(factura.ImpuestoImporteRtn);
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;
            
            Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            //this.Usuario = user != null ? user.UserID : string.Empty;
            this.Usuario = user != null ? user.UserName : string.Empty;

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
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }
                this.Contrarecibo_Folio = invoiceReceipt != null ? invoiceReceipt.Folio : "";
                if (invoiceReceipt != null && invoiceReceipt.ChkReqDetail != null && invoiceReceipt.ChkReqDetail.Count > 0) {
                    ChkReqDetail chkReqDetail = invoiceReceipt.ChkReqDetail.FirstOrDefault();
                    CheckRequest checkRequest = null;
                    if (chkReqDetail != null)
                    {
                        checkRequest = chkReqDetail.CheckRequest;
                    }
                    this.Solicitud_Folio = checkRequest != null ? checkRequest.Serie : "";
                }
                   
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

public class FacturaDTOs
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateR { get; set; }
    public DateTime DateA { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Double { get; set; }
    public string Retenciones { get; set; }
    public double Retenciones_In_Double { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }
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

    public FacturaDTOs()
    { }

    public FacturaDTOs(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();

        if (factura != null)
        {
            this.Key = Key;
            this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.Date = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateR = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.DateA = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.Proveedor_Nombre = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.VendKey = factura.Vendors != null ? factura.Vendors.VendorKey : -1;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Retenciones = factura.ImpuestoImporteRtn != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteRtn), 2).ToString() : "0";
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Subtotal_In_Double = Convert.ToDouble(factura.Subtotal);
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Retenciones_In_Double = Convert.ToDouble(factura.ImpuestoImporteRtn);
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;

            Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            this.Usuario = user != null ? user.UserName : string.Empty;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;
            this.Tipo = factura.TranType;


            if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
            {
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }
                this.Contrarecibo_Folio = invoiceReceipt != null ? invoiceReceipt.Folio : "";
                if (invoiceReceipt != null && invoiceReceipt.ChkReqDetail != null && invoiceReceipt.ChkReqDetail.Count > 0)
                {
                    ChkReqDetail chkReqDetail = invoiceReceipt.ChkReqDetail.FirstOrDefault();
                    CheckRequest checkRequest = null;
                    if (chkReqDetail != null)
                    {
                        checkRequest = chkReqDetail.CheckRequest;
                    }
                    this.Solicitud_Folio = checkRequest != null ? checkRequest.Serie : "";
                }

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
            throw new MulticonsultingException(ex.Message);

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

    private static List<FacturaDTOs> ObtenerDeTablaInvoice20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, string document_type, Expression<Func<Invoice, bool>> predicate, string order_col, string order_dir)
    {
        try
        {
            List<FacturaDTOs> facturas = new List<FacturaDTOs>();

            try
            {

                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();

                if (VendorId == "[-Seleccione proveedor-]") { VendorId = "";}
                if (Estado == "0") { Estado = ""; }

                string sSQL = "spSelectInv2020";
                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@Vendor", VendorId));
                parsT.Add(new SqlParameter("@Folio", Folio));
                parsT.Add(new SqlParameter("@Serie", Serie));
                parsT.Add(new SqlParameter("@Fecha", Fecha));
                parsT.Add(new SqlParameter("@FechaR", FechaR));
                parsT.Add(new SqlParameter("@Total", Total));
                parsT.Add(new SqlParameter("@UUID", UUID));
                parsT.Add(new SqlParameter("@Estatus", Estado));
                parsT.Add(new SqlParameter("@Orden", order_col));
                parsT.Add(new SqlParameter("@Dir", order_dir));

                using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
                {

                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = sSQL;

                    foreach (SqlParameter par in parsT)
                    {
                        Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                    }

                    SqlDataReader rdr = null;
                    rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr.GetInt32(0) != null)
                        {
                            FacturaDTOs factura = new FacturaDTOs(rdr.GetInt32(0));
                            facturas.Add(factura);
                        }
                    }

                    sqlConnection1.Close();

                }


            }
            catch (Exception ex)
            {
                string err;
                err = ex.Message;
                HttpContext.Current.Session["Error"] = err;
                string Msj = string.Empty;
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(st.FrameCount - 1);
                int LogKey2, Userk;
                string Company = string.Empty;
                if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
                if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
                if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
                Msj = Msj + ex.Message;
                string nombreMetodo = frame.GetMethod().Name.ToString();
                int linea = frame.GetFileLineNumber();
                Msj = Msj + " || Metodo : FacturaDTOs.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
                throw new MulticonsultingException(Msj);
            }
            
            return facturas;
        }
        catch
        {
            return new List<FacturaDTOs>();
        }
    }

    public static List<FacturaDTO> ObtenerPorFolio(int folio, bool directo_en_vista = false)
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
                predicate = (a => a.InvoiceKey == folio);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.InvoiceKey == folio && VendorsIds.Contains(a.VendorKey));
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

    private static List<FacturaDTOs> Obtener20(string order_col, string order_dir,string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado,string document_type, bool directo_en_vista = false)
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

            return ObtenerDeTablaInvoice20(VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado, "IN",predicate, order_col, order_dir);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTOs>();
        }
    }
    public static List<FacturaDTO> ObtenerFacturas(bool directo_en_vista = false)
    {
        return Obtener("IN", directo_en_vista);
    }

    public static List<FacturaDTOs> ObtenerFacturas20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, string order_col, string order_dir)
    {
        return Obtener20(order_col, order_dir,VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado,"IN");
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
        catch(Exception ex)
        {
            string exstring = ex.ToString();
            return new List<FacturaDTO>();
        }
    }
    
    public static List<FacturaDTO> ObtenerFacturasSinContrarecibo(string VendorId, string Folio, string Serie, string Fecha, string FechaAp,string Total, string UUID, string Estado)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturasSinContrarecibo();
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();

            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == Fecha).ToList();

            if (!string.IsNullOrWhiteSpace(FechaAp) && FechaAp != "null")
                facturas = facturas.Where(f => f.Fecha_Aprobacion == FechaAp).ToList();

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

    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Fecha, string FechaR, string contrarecibo, string solicitud, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio!= "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(contrarecibo) && contrarecibo != "null")
                facturas = facturas.Where(f => f.Contrarecibo_Folio.ToUpper().Contains(contrarecibo.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(solicitud) && solicitud != "null")
                facturas = facturas.Where(f => f.Solicitud_Folio.ToUpper().Contains(solicitud.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();
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


    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie!= "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();
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

    public static List<FacturaDTOs> ObtenerFacturas20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";

            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";


            List<FacturaDTOs> facturas = ObtenerFacturas20(VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado);



            //if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
            //    facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
            //    facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
            //    facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            //    facturas = facturas.Where(f => f.Fecha == Fecha).ToList();


            //if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
            //    facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();


            //if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            //{
            //    try
            //    {
            //        double total = Convert.ToDouble(Total.Replace(",", "."));
            //        facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
            //    }
            //    catch
            //    {
            //        throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
            //    }
            //}


            //if (!string.IsNullOrWhiteSpace(UUID))
            //    facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Estado))
            //{
            //    Estado = Tools.GetDocumentStatusDescription(Estado);
            //    facturas = facturas.Where(p => p.Estado == Estado).ToList();
            //}

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTOs>();
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
                if (factura.Status < 7) // No se analizan las facturas en proceso de pago o pagadas (¿o eliminadas?)
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


    
    public static void ActualizarEstadoFacturasSql(bool directo_en_vista = false)
    {
        try
        {
            string Rst = string.Empty;
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();

            if (!UpdateFacs(Company)) { Rst = "1"; }

            if (!UpdatePagos(Company)) { Rst = "2"; }

        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
        }
    }

    public static bool UpdateFacs(string Company)
    {
        bool Cancel = true;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;

            sSQL = "spUpdateInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Company", Company));

            using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
            {

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                foreach (SqlParameter par in parsT)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }
                SqlDataReader rdr = null;
                rdr = Cmd.ExecuteReader();
                sqlConnection1.Close();

            }
        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
    }

    public static bool UpdatePagos(string Company)
    {
        bool Cancel = true;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 6 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = Company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
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
            throw new MulticonsultingException(ex.Message);

        }
    }
}

