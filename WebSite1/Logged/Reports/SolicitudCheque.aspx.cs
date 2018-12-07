using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;
using SAGE_Model;

public partial class Logged_Reports_SolicitudChequeB : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
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
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }
        try
        {
            if (Request.QueryString.HasKeys())
            {
                try
                {
                    Users usuario = Tools.UsuarioAutenticado();
                    if (usuario == null)
                        return;
                    Company company = Tools.EmpresaAutenticada();
                    if (company == null)
                        return;

                    int id = Convert.ToInt32(Request["id"]);

                    PortalProveedoresEntities db = new PortalProveedoresEntities();
                    sage500_appEntities db_sage = new sage500_appEntities();
                    ReportDocument report_document = new ReportDocument();
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudChequeReport.rpt"));
                    CheckRequest solicitud_de_cheque = db.CheckRequest.Where(c => c.ChkReqKey == id).FirstOrDefault();
                    List<FacturaDTO> facturas = new List<FacturaDTO>();

                    if (solicitud_de_cheque == null)
                        return;
                    ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(solicitud_de_cheque.Vendors.VendorID);

                    decimal total = Math.Round(Convert.ToDecimal(solicitud_de_cheque.Total), 2);

                    NumLetra nl = new NumLetra();

                    string total_en_letras = nl.Convertir(total.ToString(), true);

                    //for(int i = 0; i < 20; i++)// Solo activar esta línea cuando se quiera ver comportamiento del documento con muchos datos cargados
                    foreach (ChkReqDetail detail in solicitud_de_cheque.ChkReqDetail)
                    {
                        foreach (InvcRcptDetails fac in detail.InvoiceReceipt.InvcRcptDetails)
                        {
                            FacturaDTO factura = new FacturaDTO()
                            {
                                Folio = fac.Invoice.Folio,
                                Compania = fac.Invoice.CompanyID,
                                Orden_de_Compra = fac.Invoice.NodeOc,
                                Tipo = fac.Invoice.TipoComprobante,
                                Total = Math.Round(fac.Invoice.Total, 2).ToString()
                            };
                            facturas.Add(factura);
                        }
                    }
                    report_document.SetDataSource(facturas);
                    report_document.SetParameterValue("razon_social_empresa", company.CompanyName);
                    report_document.SetParameterValue("direccion_gerencia", "Dirección de Finanzas");
                    report_document.SetParameterValue("condiciones_pago", "PAGO A " + proveedor.Condiciones_Descripcion.ToUpper());
                    report_document.SetParameterValue("proveedor", proveedor.Social.ToUpper());
                    report_document.SetParameterValue("importe", total.ToString());
                    report_document.SetParameterValue("folio", solicitud_de_cheque.Serie);
                    report_document.SetParameterValue("solicitante", usuario.UserName);
                    report_document.SetParameterValue("concepto", solicitud_de_cheque.Concept);
                    report_document.SetParameterValue("comentarios", solicitud_de_cheque.Comment);
                    report_document.SetParameterValue("importe_letras", total_en_letras.ToUpper());
                    report_document.SetParameterValue("fecha_programada_pago", solicitud_de_cheque.ChkReqPmtDate.Value.ToShortDateString());
                    report_document.SetParameterValue("logo", "~/Img/TSYS.png");
                    report_document.SetParameterValue("title", "Solicitud de Cheque");
                    Reporte_Solicitud_Cheque.ReportSource = report_document;
                }
                catch
                { }

            }
            else
            {
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de una solicitud de cheque");
        }
    }
}