using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Proveedores_Model;
using SAGE_Model;

public partial class Logged_Reports_ContrareciboB : System.Web.UI.Page
{
    PortalProveedoresEntities db = new PortalProveedoresEntities();
    sage500_appEntities db_sage = new sage500_appEntities();
    protected void Page_Load(object sender, EventArgs e)
    {
        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuth)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        try
        {
            if (Request.QueryString.HasKeys())
            {
                try
                {
                    int id = Convert.ToInt32(Request["id"]);

                    ReportDocument report_document = new ReportDocument();
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport.rpt"));
                    InvoiceReceipt receipt = db.InvoiceReceipt.Where(c => c.InvcRcptKey == id).FirstOrDefault();
                    List<FacturaDTO> facturas = new List<FacturaDTO>();
                    List<NotaDTO> Notas = new List<NotaDTO>();
                    decimal Total = 0;
                    foreach (var detail in receipt.InvcRcptDetails)
                    {
                        Invoice invoice = detail.Invoice;
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
                            Nota.Total = "$ " + Nota.Total;
                            Nota.Traslados = "$ " + Nota.Traslados;
                            Nota.Subtotal = "$ " + Nota.Subtotal;
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
                        factura.Total = "$ " + factura.Total;
                        factura.Traslados = "$ " + factura.Traslados;
                        factura.Subtotal = "$ " + factura.Subtotal;

                        facturas.Add(factura);
                    }
                    report_document.Database.Tables[0].SetDataSource(facturas);
                    report_document.Database.Tables[1].SetDataSource(Notas);

                    report_document.SetParameterValue("contrarecibo_no", receipt.Folio.ToString());
                    report_document.SetParameterValue("razon_social_compannia", receipt.Company.CompanyName);

                    ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(receipt.Vendors.VendorID);
                    string DBA = "Información de Proveedor no encontrada";
                    if (proveedor != null && !string.IsNullOrWhiteSpace(proveedor.RFC))
                        DBA = proveedor.RFC;

                    report_document.SetParameterValue("rfc", DBA);

                    report_document.SetParameterValue("fecha_pago", receipt.PaymentDate.ToShortDateString());
                    report_document.SetParameterValue("fecha_datos", Tools.FechaEnEspañol(receipt.CreateDate));
                    report_document.SetParameterValue("proveedor", receipt.Vendors.VendName);
                    report_document.SetParameterValue("total", Total);

                    Reporte_Contrarecibo.ReportSource = report_document;

                }
                catch (Exception ex)
                {
                    string eeeee = ex.ToString();
                }

            }
            else
            {
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de un contrarecibo");
        }
    }
}