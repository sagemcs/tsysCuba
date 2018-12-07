using System;
using System.IO;
using uCFDsLib.v33;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using ConsultaSAT; // SAT
using System.Web;
using System.Web.UI;
using System.Text;
using System.Net.Mail;
using System.Net;

public partial class Facturas : System.Web.UI.Page
{
    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;

    public int pVendKey
    {
        get
        {
            return this.iVendKey;
        }
        set
        {
            this.iVendKey = value;
        }
    }
    public string pCompanyID
    {
        get
        {
            return this.iCompanyID;
        }
        set
        {
            this.iCompanyID = value;
        }
    }
    public int pLogKey
    {
        get
        {
            return this.iLogKey;
        }
        set
        {
            this.iLogKey = value;
        }
    }
    public int pUserKey
    {
        get
        {
            return this.iUserKey;
        }
        set
        {
            this.iUserKey = value;
        }
    }
    string eventName = String.Empty;
    //Rutina Manejar Errores
    private void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
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
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

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
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try {


            if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {


                string Var = HttpContext.Current.Session["VendKey"].ToString();
                string Var2 = HttpContext.Current.Session["RolUser"].ToString();
                string Var3 = HttpContext.Current.Session["IDCompany"].ToString();

                pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

            }

            if (!IsPostBack)
            {

                //gvFacturas.AllowPaging = true;
                // gvFacturas.PageSize = 15;           
                // gvFacturas.AllowSorting = true;
                // HttpContext.Current.Session["Error"] = "";
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }

            Global.Docs();
            if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
            {
                if (HttpContext.Current.Session["UpDoc"].ToString() == "1") { Page.MasterPageFile = "MenuP.master"; }
                else { Page.MasterPageFile = "MenuPreP.master"; }
            }
        }
        catch
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    private void BindGridView(int InvoiceKey, int vendKey, string CompanyId)
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        try {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                // Create a DataSet object. 
                DataSet dsFacturas = new DataSet();

                // Create a SELECT query. 
                string strSelectCmd = "select Invoice.InvoiceKey,UUID,FechaTransaccion from Invoice inner join InvoiceFile on Invoice.InvoiceKey = InvoiceFile.InvoiceKey WHERE Invoice.VendorKey =" + vendKey + " and Invoice.InvoiceKey=" + InvoiceKey + " and Invoice.CompanyID='" + CompanyId + "'";

                // Create a SqlDataAdapter object 
                // SqlDataAdapter represents a set of data commands and a  
                // database connection that are used to fill the DataSet and  
                // update a SQL Server database.  
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);

                // Open the connection 
                conn.Open();

                // Fill the DataTable named "Person" in DataSet with the rows 
                // returned by the query.new n 
                da.Fill(dsFacturas, "Invoice");


                // Get the DataView from Person DataTable. 
                DataView dvInvoice = dsFacturas.Tables["Invoice"].DefaultView;


                // Set the sort column and sort order. 
                //dvPerson.Sort = ViewState["SortExpression"].ToString();


                // Bind the GridView control. 
                gvFacturas.DataSource = dvInvoice;
                gvFacturas.DataBind();


            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    private void BindLogGridView(int InvoiceKey, int vendKey, string CompanyId)
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        try
        {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                // Create a DataSet object. 
                DataSet dsFacturas = new DataSet();

                // Create a SELECT query. 
                string strSelectCmd = "select Invoice.InvoiceKey,invoice.folio,Invoice.uuid,Invoice.NodeOc,ErrorValidacion,FechaError from Invoice inner join InvcLogValidacion on InvcLogValidacion.InvoiceKey = Invoice.InvoiceKey WHERE Invoice.VendorKey =" + vendKey + " and Invoice.InvoiceKey=" + InvoiceKey + " and Invoice.CompanyID='" + CompanyId + "'";

                // Create a SqlDataAdapter object 
                // SqlDataAdapter represents a set of data commands and a  
                // database connection that are used to fill the DataSet and  
                // update a SQL Server database.  
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);

                // Open the connection 
                conn.Open();

                // Fill the DataTable named "Person" in DataSet with the rows 
                // returned by the query.new n 
                da.Fill(dsFacturas, "Invoice");


                // Get the DataView from Person DataTable. 
                DataView dvInvoice = dsFacturas.Tables["Invoice"].DefaultView;


                // Set the sort column and sort order. 
                //dvPerson.Sort = ViewState["SortExpression"].ToString();


                // Bind the GridView control. 
                gvValidacion.DataSource = dvInvoice;
                gvValidacion.DataBind();


            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            String archivo;

            GridViewRow row = gvFacturas.Rows[index];
            archivo = row.Cells[1].Text;


            if (e.CommandName == "Documento_1")
            {
                memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary1");
                archivo += ".xml";
            }
            if (e.CommandName == "Documento_2")
            {
                memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary2");
                archivo += ".pdf";
            }
            if (e.CommandName == "Documento_3")
            {
                memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary3");
                archivo += ".pdf";
            }

            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + archivo);
            Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void btnSage_Click(object sender, EventArgs e)
    {
        try {

            HttpContext.Current.Session["Error"] = "";
            Label4.Text = "";
            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;

            if (!FileUpload1.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvFacturas.DataBind();
                gvValidacion.DataBind();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B1);", true);

                return;
            }
            if (!FileUpload2.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvFacturas.DataBind();
                gvValidacion.DataBind();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B2);", true);
                return;
            }
            if (!FileUpload3.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvFacturas.DataBind();
                gvValidacion.DataBind();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B3);", true);
                return;
            }

            int carga = databaseFilePutDB(FileUpload1.PostedFile.InputStream, FileUpload2.PostedFile.InputStream, FileUpload3.PostedFile.InputStream, pVendKey, 0, pCompanyID);

            if (carga == -1)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvFacturas.DataBind();
                gvValidacion.DataBind();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                return;
            }

            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = databaseFileRead("InvcFileKey", Convert.ToString(carga), "FileBinary1");


            if (CargarXML(memoryStream, carga))
            {   ///Notificacion SAGE
                ListEm();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B5);", true);
            }
            else
            {
                gvFacturas.DataBind();
                gvValidacion.DataBind();
                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
            }



        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }

    }

    private bool EmailC(int Opcion, string Destinatario)
    {
        bool Resut = false;
        try
        {

                string PassNew = "<div ID='Tabla'><div class='panel panel-success'><div class='table-responsive'><table class='table table-striped' id='table-to-result-ass-indic'><tbody> ";

                if (Opcion == 2)
                {
                    PassNew = PassNew + "<tr class='table-primary'><th><strong>Proveedor :  </strong>     </th><td> " + User.Identity.Name.ToString() + "</td></tr><br/>";
                }

                PassNew = PassNew + "</tbody></ table ></ div ></ div ></ div > ";

                    string body = string.Empty;

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionFacts.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }

            Global.EmailGlobal(Destinatario, body, "CARGA DE FACTURAS");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected bool ListEm()
    {
        bool uno;
        uno = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spAddMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = "xx" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Notificación Facturas" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Desti = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());

                    if (Desti != "" && Desti != "Usuario")
                    {
                        EmailC(2, Desti);
                    }
                }
            }
        }
        catch
        {

        }
        return uno;
    }

    private bool CargarXML(MemoryStream fs, int InvcFileKey)
    {
        try
        {
            bool valido = false;
            bool complementovalido = false;
            bool adendavalida = false;

            int VendKey;
            int LogKey;
            int UserKey;
            String CompanyID;

            VendKey = pVendKey;
            LogKey = pLogKey;
            UserKey = pUserKey;
            CompanyID = pCompanyID;


            String folio = "";
            String serie = "";
            DateTime FechaTransaccion = DateTime.Now;
            DateTime FechaTimbre = DateTime.Now;
            String Moneda = "";
            String UsoCFDI = "";
            String TipoComprobante = "";
            String FormaPago = "";
            String MetodoPago = "";
            String UUID = "";

            String NombreEmisor = "";
            String RFCEmisor = "";
            String NombreReceptor = "";
            String RFCReceptor = "";
            String complemento = "";
            String OdeC = "";
            String tipo = "";
            String UUIDRel = "";

            decimal ImpuestoImporteTraslado = 0;
            decimal ImpuestoImporteRetenido = 0;
            decimal Descuento = 0;
            decimal Subtotal = 0;
            decimal Total = 0;

            decimal importimp = 0;
            decimal tasaimp = 0;
            decimal baseimp = 0;
            String tipoImp = "";

            int vkey;
            int vkeydtl;
            int Cancelado = 0;
            int Timbrado = 0;
            int Status = 1;
            int resultado = 0;
            int cuentaAdenda = 0;
            int validacion = 0;

            DataTable orderDetailTable;

            orderDetailTable = new DataTable("OrderDetail");
            DataColumn[] cols ={
                                  new DataColumn("number",typeof(Int32)),
                                  new DataColumn("noIdVnd",typeof(String)),
                                  new DataColumn("noIdTS",typeof(String)),
                                  new DataColumn("qty",typeof(Decimal)),
                                  new DataColumn("amt",typeof(Decimal))
                              };

            orderDetailTable.Columns.AddRange(cols);

            StreamReader objStreamReader;

            StreamReader streamReader = null;
            string xmlOutput = string.Empty;

            fs.Position = 0;
            streamReader = new StreamReader(fs);
            xmlOutput = streamReader.ReadToEnd();
            streamReader.Close();

            TextReader reader = new StringReader(xmlOutput);
            Comprobante Factura = new Comprobante();
            XmlSerializer Xml = new XmlSerializer(Factura.GetType());
            Factura = (Comprobante)Xml.Deserialize(reader);

            if (Factura.Emisor.Nombre == null) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Emisor."; return false; }
            if (Factura.Emisor.Rfc == null) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Emisor."; return false; }
            if (Factura.Receptor.Nombre == null) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Receptor."; return false; }
            if (Factura.Receptor.Rfc == null) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Receptor."; return false; }

            if (Factura.Folio == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Folio."; return false; }
            if (Factura.Serie == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Serie"; return false; }

            NombreEmisor = Factura.Emisor.Nombre.ToString();
            RFCEmisor = Factura.Emisor.Rfc.ToString();
            NombreReceptor = Factura.Receptor.Nombre.ToString();
            RFCReceptor = Factura.Receptor.Rfc.ToString();
            UsoCFDI = Factura.Receptor.UsoCFDI.ToString();

            folio = Factura.Folio.ToString();
            serie = Factura.Serie.ToString();
            
            FechaTransaccion = Factura.Fecha;

            Moneda = Factura.Moneda.ToString();
            TipoComprobante = Factura.TipoDeComprobante.ToString();

            if (TipoComprobante == "E")
            {
                var cta = Factura.CfdiRelacionados.CfdiRelacionado.Length;

                for (int r = 0; r < cta; r++)
                {
                    UUIDRel = Factura.CfdiRelacionados.CfdiRelacionado[r].UUID.ToString();
                }
            }

            FormaPago = Factura.FormaPago.ToString();
            MetodoPago = Factura.MetodoPago.ToString();

            if (Factura.Impuestos.TotalImpuestosTrasladadosSpecified)
            {
                ImpuestoImporteTraslado = Factura.Impuestos.TotalImpuestosTrasladados;
            }

            if (Factura.Impuestos.TotalImpuestosRetenidosSpecified)
            {
                ImpuestoImporteRetenido = Factura.Impuestos.TotalImpuestosRetenidos;
            }

            Descuento = Factura.Descuento;
            Subtotal = Factura.SubTotal;
            Total = Factura.Total;

            ////Validación de Valores
            if (NombreEmisor.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Emisor.";
                return false;
            }

            if (RFCEmisor.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Emisor.";
                return false;
            }

            if (NombreEmisor.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Receptor.";
                return false;
            }

            if (RFCEmisor.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Receptor.";
                return false;
            }

            if (folio.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene un número de Folio.";
                return false;
            }

            if (FechaTransaccion.ToString().Length == 0)
            {

                HttpContext.Current.Session["Error"] = "El XML no contiene una fecha válida.";
                return false;
            }

            if (Moneda.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene una moneda válida.";
                return false;
            }
            else
            {
                if (!Enum.IsDefined(typeof(c_Moneda), Moneda.ToString()))
                {
                    HttpContext.Current.Session["Error"] = "El XML no contiene una moneda válida.";
                    return false;
                }

            }

            if (TipoComprobante.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene un tipo de Comprobante válido.";
                return false;
            }
            else
            {
                if (!Enum.IsDefined(typeof(c_TipoDeComprobante), TipoComprobante.ToString()))
                {
                    HttpContext.Current.Session["Error"] = "El XML no contiene un tipo de Comprobante válido.";
                    return false;
                }
            }

            if (FormaPago.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene una forma de pago válida.";
                return false;
            }
            else
            {
                if (!Enum.IsDefined(typeof(c_FormaPago), FormaPago.ToString()))
                {
                    HttpContext.Current.Session["Error"] = "El XML no contiene una forma de pago válida.";
                    return false;
                }
            }

            if (MetodoPago.Length == 0)
            {
                HttpContext.Current.Session["Error"] = "El XML no contiene un Método de Pago válido.";
                return false;
            }
            else
            {
                if (!Enum.IsDefined(typeof(c_MetodoPago), MetodoPago.ToString()))
                {
                    HttpContext.Current.Session["Error"] = "El XML no contiene un Método de Pago válido.";
                    return false;
                }
            }

            if (Subtotal == 0)
            {
                HttpContext.Current.Session["Error"] = "El Subtotal debe ser mayor a cero.";
                return false;
            }

            if (Total == 0)
            {
                HttpContext.Current.Session["Error"] = "El Total debe ser mayor a cero.";
                return false;
            }


            if (Factura.Complemento[0].Any != null && Factura.Complemento[0].Any.Length > 0)
            {
                int longitud;

                longitud = Factura.Complemento[0].Any.Length;
                ComprobanteComplemento cmp;
                cmp = Factura.Complemento[0];

                for (int i = 0; i < longitud; i++)
                {
                    XmlElement xe;
                    String nombre;

                    xe = cmp.Any[i];
                    nombre = xe.Name.ToString();

                    if (nombre == "tfd:TimbreFiscalDigital")
                    {
                        complemento = cmp.Any[i].OuterXml;

                        Stream s = new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(complemento));
                        uCFDsLib.v33.TimbreFiscalDigital Base = new uCFDsLib.v33.TimbreFiscalDigital();
                        objStreamReader = new StreamReader(s);
                        Xml = new XmlSerializer(Base.GetType());
                        Base = (uCFDsLib.v33.TimbreFiscalDigital)Xml.Deserialize(objStreamReader);
                        objStreamReader.Close();

                        UUID = Base.UUID.ToString();
                        FechaTimbre = Base.FechaTimbrado;

                        if (UUID.ToString().Length == 0)
                        {
                            HttpContext.Current.Session["Error"] = "El folio UUID del comprobante no es correcto.";
                            return false;
                        }

                        complementovalido = true;

                    }
                    if (nombre == "cfdi:Addenda")
                    {
                        var adenda = cmp.Any[i].OuterXml;
                        XmlDocument doc = new XmlDocument(); doc.LoadXml(adenda); XmlNode myNode = doc.DocumentElement;
                        OdeC = doc.ChildNodes[0].ChildNodes[0].Attributes["OdeC"].Value;
                        //tipo = doc.ChildNodes[0].ChildNodes[0].Attributes["tipo"].Value;

                        //int cuenta = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes.Count;


                        //for (int c = 0; c < cuenta; c++)
                        //{
                        //    var number = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[c].Attributes["number"].Value;
                        //    var noIdVnd = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[c].Attributes["noIdVnd"].Value;
                        //    var noIdTS = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[c].Attributes["noIdTS"].Value;
                        //    var qty = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[c].Attributes["qty"].Value;
                        //    var amt = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[c].Attributes["amt"].Value;

                        //    orderDetailTable.Rows.Add(number, noIdVnd, noIdTS, qty, amt);
                        //}

                        if (OdeC.Length == 0)
                        {
                            HttpContext.Current.Session["Error"] = "El número de compra no es correcto.";
                            return false;
                        }

                        adendavalida = true;
                    }
                }
            }
            else
            {
                HttpContext.Current.Session["Error"] = "El Xml no tiene complemento.";
                return false;
            }

            if (!complementovalido)
            {
                HttpContext.Current.Session["Error"] = "El Xml no tiene complemento de timbrado.";
                return false;
            }

            if (!adendavalida)
            {
                HttpContext.Current.Session["Error"] = "El Xml no tiene una adenda válida.";
                return false;
            }


            if (!ConsultaSAT(Factura, UUID))
            {
                Timbrado = 1;
            }
            else
                Timbrado = 1;


            if (ConsultaFacturaDB(UUID))
            {
                HttpContext.Current.Session["Error"] = "El Comprobante ya se encuentra registrado.";
                return false;
            }

            if (TipoComprobante == "E")
            {
                if (!ConsultaFacturaDB(UUIDRel))
                {
                    HttpContext.Current.Session["Error"] = "El Comprobante al que hace referencia la nota de crédito, no está registrado.";
                    return false;
                }
            }


            for (int i = 0; i < Factura.Conceptos.Length; i++)
            {
                string strDescr = "";
                string strUnidad = "";
                string strItemId = "";
                string strItemIdSAT = "";

                decimal UnitCost = 0;
                decimal Monto = 0;
                decimal Cantidad = 0;
                decimal DescuentoItm = 0;
                int longImpTsl = 0;
                int longImpRtn = 0;

                strItemId = Factura.Conceptos[i].NoIdentificacion.ToString();
                strItemIdSAT = Factura.Conceptos[i].ClaveProdServ.ToString();
                strDescr = Factura.Conceptos[i].Descripcion.ToString();
                strUnidad = Factura.Conceptos[i].ClaveUnidad.ToString();
                UnitCost = Factura.Conceptos[i].ValorUnitario;
                Monto = Factura.Conceptos[i].Importe;
                Cantidad = Factura.Conceptos[i].Cantidad;

                //foreach (DataRow row in orderDetailTable.Rows)
                //{
                //    var number = row[0].ToString();
                //    var noIdVnd = row[1].ToString();
                //    var noIdTS = row[2].ToString();
                //    var qty = Convert.ToDecimal(row[3].ToString());
                //    var amt = Convert.ToDecimal(row[4].ToString());

                //    if (noIdVnd == strItemId && qty == Cantidad && amt == Monto)
                //    {
                //        cuentaAdenda += 1;
                //    }
                //}


                if (Factura.Conceptos[i].DescuentoSpecified)
                {
                    DescuentoItm = Factura.Conceptos[i].Descuento;
                }

                if (strItemIdSAT.ToString().Length == 0)
                {

                    HttpContext.Current.Session["Error"] = "El artículo no tiene " + strItemIdSAT + " una clave del SAT válido.";
                    return false;
                }

                if (strDescr.ToString().Length == 0)
                {

                    HttpContext.Current.Session["Error"] = "El artículo" + strItemIdSAT + " no tiene una descripción válida.";
                    return false;
                }

                if (strUnidad.ToString().Length == 0)
                {

                    HttpContext.Current.Session["Error"] = "El artículo" + strItemIdSAT + " no tiene una clave de unidad válida.";
                    return false;
                }
                else {
                    if (!Enum.IsDefined(typeof(c_ClaveUnidad), strUnidad.ToString()))
                    {

                        HttpContext.Current.Session["Error"] = "El artículo" + strItemIdSAT + " no tiene una clave de unidad válida.";
                        return false;
                    }
                }

                if (UnitCost == 0)
                {

                    HttpContext.Current.Session["Error"] = "El costo unitario del artículo" + strItemIdSAT + " debe ser mayor a cero.";
                    return false;
                }
                if (Monto == 0)
                {

                    HttpContext.Current.Session["Error"] = "El Monto del artículo" + strItemIdSAT + " debe ser mayor a cero.";
                    return false;
                }
                if (Cantidad == 0)
                {
                    HttpContext.Current.Session["Error"] = "La cantidad del artículo" + strItemIdSAT + " debe ser mayor a cero.";
                    return false;
                }




                if (Factura.Conceptos[i].Impuestos != null)
                {
                    ComprobanteConceptoImpuestos Imp = Factura.Conceptos[i].Impuestos;

                    if (Imp.Traslados != null)
                    {
                        longImpTsl = Imp.Traslados.Length;

                        if (longImpTsl > 0)
                        {
                            for (int j = 0; j < longImpTsl; j++)
                            {

                                importimp = Imp.Traslados[j].Importe;
                                tasaimp = Imp.Traslados[j].TasaOCuota;
                                baseimp = Imp.Traslados[j].Base;
                                tipoImp = Imp.Traslados[j].Impuesto.ToString();

                                if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                {

                                    HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                    return false;
                                }

                                if (tasaimp > 0)
                                {
                                    if (importimp == 0)
                                    {

                                        HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                        return false;
                                    }
                                }
                            }
                        }
                    }


                    if (Imp.Retenciones != null)
                    {

                        longImpRtn = Imp.Retenciones.Length;

                        if (longImpTsl > 0)
                        {
                            for (int j = 0; j < longImpRtn; j++)
                            {
                                importimp = Imp.Retenciones[j].Importe;
                                tasaimp = Imp.Retenciones[j].TasaOCuota;
                                tipoImp = Imp.Retenciones[j].Impuesto.ToString();

                                if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                {

                                    HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                    return false;
                                }

                                if (tasaimp > 0)
                                {
                                    if (importimp == 0)
                                    {
                                        HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                        return false;
                                    }
                                }

                            }
                        }
                    }
                }
            }



            //if (Factura.Conceptos.Length != cuentaAdenda)
            //{
            //    HttpContext.Current.Session["Error"] = "Los artículos de la adenda no coinciden con los artículos del comprobante fiscal. ";
            //    // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

            //    return false;
            //}



            //Insertar Encabezado en BD
            Tuple<int, int> resutl = InsertInvoiceDB(VendKey, RFCEmisor, RFCReceptor, NombreEmisor, NombreReceptor, folio, serie, OdeC, UUID, FechaTransaccion, FechaTimbre, Moneda, UsoCFDI, TipoComprobante, FormaPago, MetodoPago, ImpuestoImporteTraslado, ImpuestoImporteRetenido, Descuento, Subtotal, Total, Cancelado, Timbrado, Status, UserKey, CompanyID, LogKey);
            vkey = resutl.Item1;
            resultado = resutl.Item2;

            if (vkey > 0 & resultado == 1)
            {
                //Inserta Detalle en BS
                for (int k = 0; k < Factura.Conceptos.Length; k++)
                {
                    string strDescr = "";
                    string strUnidad = "";
                    string strItemId = "";
                    string strItemIdSAT = "";

                    decimal UnitCost = 0;
                    decimal Monto = 0;
                    decimal Cantidad = 0;
                    decimal DescuentoItm = 0;
                    int longImpTsl = 0;
                    int longImpRtn = 0;

                    strItemId = Factura.Conceptos[k].NoIdentificacion.ToString();
                    strItemIdSAT = Factura.Conceptos[k].ClaveProdServ.ToString().Substring(Factura.Conceptos[k].ClaveProdServ.ToString().Length - 8, 8);
                    strDescr = Factura.Conceptos[k].Descripcion.ToString();
                    strUnidad = Factura.Conceptos[k].ClaveUnidad.ToString();
                    UnitCost = Factura.Conceptos[k].ValorUnitario;
                    Monto = Factura.Conceptos[k].Importe;
                    Cantidad = Factura.Conceptos[k].Cantidad;
                    strItemId = strDescr.Trim().Split(' ')[0];

                    if (Factura.Conceptos[k].DescuentoSpecified)
                    {
                        DescuentoItm = Factura.Conceptos[k].Descuento;
                    }

                    //foreach (DataRow row in orderDetailTable.Rows)
                    //{
                    //    var noIdVnd = row[1].ToString();
                    //    var noIdTS = row[2].ToString();
                    //    var qty = Convert.ToDecimal(row[3].ToString());
                    //    var amt = Convert.ToDecimal(row[4].ToString());

                    //    if (noIdVnd == strItemId && qty == Cantidad && amt == Monto)
                    //    {
                    //        strItemId = noIdTS;
                    //    }
                    //}
                                     

                    Tuple<int, int> resutlDtl = InsertInvoiceDtlDB(vkey, strItemId, strItemIdSAT, strUnidad, strDescr, UnitCost, Cantidad, Monto, DescuentoItm, Status, UserKey, CompanyID, LogKey);
                    vkeydtl = resutlDtl.Item1;
                    resultado = resutlDtl.Item2;

                    if (vkeydtl > 0 & resultado == 1)
                    {

                        if (Factura.Conceptos[k].Impuestos != null)
                        {
                            ComprobanteConceptoImpuestos Imp = Factura.Conceptos[k].Impuestos;

                            if (Imp.Traslados != null)
                            {
                                longImpTsl = Imp.Traslados.Length;

                                if (longImpTsl > 0)
                                {
                                    for (int j = 0; j < longImpTsl; j++)
                                    {
                                        importimp = Imp.Traslados[j].Importe;
                                        tasaimp = Imp.Traslados[j].TasaOCuota;
                                        tipoImp = Imp.Traslados[j].Impuesto.ToString().Substring(Imp.Traslados[j].Impuesto.ToString().Length - 3, 3);

                                        Tuple<int, int> resutlDtlTaxT = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "T", Status, UserKey, CompanyID, LogKey);
                                        int vkeydtltx = resutlDtlTaxT.Item1;
                                        resultado = resutlDtlTaxT.Item2;

                                        if (!(vkeydtltx > 0 & resultado == 1))
                                        {
                                            HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                            return false;
                                        }

                                    }
                                }
                            }

                            if (Imp.Retenciones != null)
                            {

                                longImpRtn = Imp.Retenciones.Length;

                                if (longImpTsl > 0)
                                {
                                    for (int j = 0; j < longImpRtn; j++)
                                    {
                                        importimp = Imp.Retenciones[j].Importe;
                                        tasaimp = Imp.Retenciones[j].TasaOCuota;
                                        tipoImp = Imp.Retenciones[j].Impuesto.ToString().Substring(Imp.Retenciones[j].Impuesto.ToString().Length - 3, 3);

                                        Tuple<int, int> resutlDtlTaxR = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "R", Status, UserKey, CompanyID, LogKey);
                                        int vkeydtltx = resutlDtlTaxR.Item1;
                                        resultado = resutlDtlTaxR.Item2;

                                        if (!(vkeydtltx > 0 & resultado == 1))
                                        {
                                            HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                            return false;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session["Error"] = "Error al Insertar artículo de Factura";
                        return false;
                    }
                }//for detalle
            }
            else
            {
                HttpContext.Current.Session["Error"] = "Error al Insertar Encabezado de Factura.";           
                return false;
            }


            if (TipoComprobante == "I")
            {
                SincNcInv(vkey, 0, "IN");
                validacion = ValidatetInvoiceDB(VendKey, vkey, CompanyID);
            }
            else if (TipoComprobante == "E")
            {
                int invk = ConsultaFacturaKeyDB(UUIDRel);
                SincNcInv(vkey, invk, "CM");
                validacion = ValidateNCDB(VendKey, vkey, CompanyID);
            }

            //validacion = 0;

            if (validacion == 0)
            {

                if (SincInvFiles(vkey, InvcFileKey))
                {
                    BindGridView(vkey, pVendKey, pCompanyID);


                    return true;
                }
                else
                {
                    DelInvFiles(vkey, InvcFileKey);
                    return false;
                }
            }
            else
            {
                BindLogGridView(vkey, pVendKey, pCompanyID);
                DelInvFiles(vkey, InvcFileKey);
                return false;
            }

        }
        catch (Exception ex)
        {
            string err;
            err = "XML Invalido ";
            if (HttpContext.Current.Session["Error"].ToString() == "")
            {
                err = err + HttpContext.Current.Session["Error"].ToString();
            }
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", err, pCompanyID);

            if (HttpContext.Current.Session["Error"].ToString() == "")
            {
              HttpContext.Current.Session["Error"] = err;
            }
            return false;
        }
    }

    private bool ConsultaSAT(Comprobante Factura, string UUID)
    {

        try
        {
            string titulo, Msj, tipo;
            bool timbrado = false;

            string querySend = "?re=" + Factura.Emisor.Rfc.ToString() + "&rr=" + Factura.Receptor.Rfc.ToString() + "&tt=" + Factura.Total.ToString() + "&id=" + UUID;

            ConsultaCFDIServiceClient consulta = new ConsultaCFDIServiceClient();
            ConsultaSAT.Acuse Acuse = new ConsultaSAT.Acuse();

            Acuse = consulta.Consulta(querySend);
            consulta.Close();

            if (Acuse.Estado.ToLower() == "vigente")
            {
                tipo = "success";
                Msj = Acuse.Estado;
                timbrado = true;
            }
            else if (Acuse.Estado.ToLower() == "cancelado")
            {
                tipo = "error";
                Msj = Acuse.Estado;
                timbrado = false;
            }
            else
            {
                tipo = "info";
                Msj = Acuse.Estado;

            }

            titulo = "Servicio SAT";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

            return timbrado;
        }

        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaSAT", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }
    }

    private bool ConsultaFacturaDB(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM Invoice WHERE UUID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }

    }

    private int ConsultaFacturaKeyDB(string UUID)
    {

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();


            sql = @"SELECT InvoiceKey FROM Invoice WHERE UUID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }


            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return Convert.ToInt32(Cuenta);
            else
                return Convert.ToInt32(Cuenta);
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return 0;
        }

    }

    private bool SincInvFiles(int InvoiceKey, int InvcFileKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE InvoiceFile SET InvoiceKey= '" + InvoiceKey + "' WHERE InvcFileKey = '" + InvcFileKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }
    }

    private void SincNcInv(int InvoiceKey, int invk, String TranType)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            if (TranType == "IN")
                invk = InvoiceKey;


            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE Invoice SET ApplyToInvcKey= " + invk + ", TranType='" + TranType + "' WHERE InvoiceKey = " + InvoiceKey;
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }


        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();

        }
    }

    private bool DelInvFiles(int InvoiceKey, int InvcFileKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Delete from InvoiceFile WHERE InvcFileKey = '" + InvcFileKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from InvoiceTaxLine WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from InvoiceLines WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from Invoice WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }
    }
    
    private Tuple<int, int> InsertInvoiceDB(int VendorKey, String RFCEmisor, String RFCReceptor, String NombreEmisor, String NombreReceptor, String Folio, String Serie, String NodeOc, String UUID, DateTime FechaTransaccion, DateTime FechaTimbrado, String Moneda, String UsoCFDi, String TipoComprobante, String FormaPago, String MetodoPago, Decimal ImpuestoImporteTrs, Decimal ImpuestoImporteRtn, Decimal Descuento, Decimal Subtotal, Decimal Total, int Cancelado, int Timbrado, int Status, int UpdateUserKey, String CompanyID, int LogKey)
    {
        try {


            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapInsertInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
            parsT.Add(new SqlParameter("@RFCEmisor", RFCEmisor));
            parsT.Add(new SqlParameter("@RFCReceptor", RFCReceptor));
            parsT.Add(new SqlParameter("@NombreEmisor", NombreEmisor));
            parsT.Add(new SqlParameter("@NombreReceptor", NombreReceptor));
            parsT.Add(new SqlParameter("@Folio", Folio));
            parsT.Add(new SqlParameter("@Serie", Serie));
            parsT.Add(new SqlParameter("@NodeOc", NodeOc));
            parsT.Add(new SqlParameter("@UUID", UUID));
            parsT.Add(new SqlParameter("@FechaTransaccion", FechaTransaccion));
            parsT.Add(new SqlParameter("@FechaTimbrado", FechaTimbrado));
            parsT.Add(new SqlParameter("@Moneda", Moneda));
            parsT.Add(new SqlParameter("@UsoCFDi", UsoCFDi));
            parsT.Add(new SqlParameter("@TipoComprobante", TipoComprobante));
            parsT.Add(new SqlParameter("@FormaPago", FormaPago));
            parsT.Add(new SqlParameter("@MetodoPago", MetodoPago));
            parsT.Add(new SqlParameter("@ImpuestoImporteTrs", ImpuestoImporteTrs));
            parsT.Add(new SqlParameter("@ImpuestoImporteRtn", ImpuestoImporteRtn));
            parsT.Add(new SqlParameter("@Descuentos", Descuento));
            parsT.Add(new SqlParameter("@Subtotal", Subtotal));
            parsT.Add(new SqlParameter("@Total", Total));
            parsT.Add(new SqlParameter("@Timbrado", Timbrado));
            parsT.Add(new SqlParameter("@Cancelado", Cancelado));
            parsT.Add(new SqlParameter("@Status", Status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));
            parsT.Add(new SqlParameter("@LogKey", LogKey));

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
                    vkey = rdr.GetInt32(0); //> 0 ok
                    val1 = rdr.GetInt32(1); //= 1-OK

                }

                sqlConnection1.Close();

            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private Tuple<int, int> InsertInvoiceDtlDB(int InvoiceKey, String Codigo, String ClaveProd, String ClaveUni, String Descripcion, decimal ValorUnitario, decimal Cantidad, decimal Importe, decimal Descuento, int status, int UpdateUserKey, String CompanyID, int LogKey)
    {
        try
        {
            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapInsertInvoiceDtl";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@Codigo", Codigo));
            parsT.Add(new SqlParameter("@ClaveProd", ClaveProd));
            parsT.Add(new SqlParameter("@ClaveUni", ClaveUni));
            parsT.Add(new SqlParameter("@Descripcion", Descripcion));
            parsT.Add(new SqlParameter("@ValorUnitario", ValorUnitario));
            parsT.Add(new SqlParameter("@Cantidad", Cantidad));
            parsT.Add(new SqlParameter("@Importe", Importe));
            parsT.Add(new SqlParameter("@Descuento", Descuento));
            parsT.Add(new SqlParameter("@status", status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));
            parsT.Add(new SqlParameter("@LogKey", LogKey));


            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {
                try
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
                        vkey = rdr.GetInt32(0); //> 0 ok
                        val1 = rdr.GetInt32(1); //= 1-OK

                    }
                }


                catch (Exception ex)
                {
                    string err;
                    err = ex.InnerException.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);

            sqlConnection1.Close();

            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private Tuple<int, int> InsertInvoiceDtlTaxDB(int InvoiceKey, int InvoicedtlKey, Decimal TaxAmt, String TaxID, decimal rate, String Tipo, int status, int UpdateUserKey, String CompanyID, int LogKey)
    {
        try
        {
            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapInsertInvoiceDtlTax";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@InvoicedtlKey", InvoicedtlKey));
            parsT.Add(new SqlParameter("@TaxAmt", TaxAmt));
            parsT.Add(new SqlParameter("@TaxID", TaxID));
            parsT.Add(new SqlParameter("@rate", rate));
            parsT.Add(new SqlParameter("@Tipo", Tipo));
            parsT.Add(new SqlParameter("@status", status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {
                try
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
                        vkey = rdr.GetInt32(0); //> 0 ok
                        val1 = rdr.GetInt32(1); //= 1-OK

                    }
                }


                catch (Exception ex)
                {
                    string err;
                    err = ex.InnerException.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private int ValidatetInvoiceDB(int VendorKey, int InvoiceKey, String CompanyID)
    {
        try
        {

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapiValidateInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {
                try
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


                catch (Exception ex)
                {
                    string err;
                    err = ex.InnerException.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return val1;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return -1;
        }
    }

    private int ValidateNCDB(int VendorKey, int InvoiceKey, String CompanyID)
    {
        try
        {

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapiValidateCN";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {
                try
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


                catch (Exception ex)
                {
                    string err;
                    err = ex.InnerException.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return val1;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.InnerException.Message, pCompanyID);
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return -1;
        }
    }

    private int databaseFilePutDB(Stream fs, Stream fs2, Stream fs3, int VendorKey, int InvoiceKey, String CompanyID)
    {
        try
        {

            string val = "";
            int id = 0;

            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);

            System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs2);
            Byte[] bytes2 = br2.ReadBytes((Int32)fs2.Length);

            System.IO.BinaryReader br3 = new System.IO.BinaryReader(fs3);
            Byte[] bytes3 = br3.ReadBytes((Int32)fs3.Length);

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (var sqlWrite = new SqlCommand("Insert Into InvoiceFile (InvoiceKey,VendorKey,FileBinary1,FileBinary2,FileBinary3,FileType1,FileType2,FileType3,UpdateDate,UpdateUserKey,CompanyId) VALUES (@InvoiceKey,@VendorKey,@FileBinary1,@FileBinary2,@FileBinary3,@FileType1,@FileType2,@FileType3,@UpdateDate,@UpdateUserKey,@CompanyId);SELECT SCOPE_IDENTITY();", sqlConnection1))
            {
                sqlWrite.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;
                sqlWrite.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
                sqlWrite.Parameters.Add("@FileBinary1", SqlDbType.VarBinary, bytes.Length).Value = bytes;
                sqlWrite.Parameters.Add("@FileBinary2", SqlDbType.VarBinary, bytes2.Length).Value = bytes2;
                sqlWrite.Parameters.Add("@FileBinary3", SqlDbType.VarBinary, bytes3.Length).Value = bytes3;
                sqlWrite.Parameters.Add("@FileType1", SqlDbType.VarChar, 5).Value = "xml";
                sqlWrite.Parameters.Add("@FileType2", SqlDbType.VarChar, 5).Value = "pdf";
                sqlWrite.Parameters.Add("@FileType3", SqlDbType.VarChar, 5).Value = "pdf";
                sqlWrite.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                sqlWrite.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = pUserKey;
                sqlWrite.Parameters.Add("@CompanyId", SqlDbType.VarChar, 3).Value = CompanyID;

                var modified = sqlWrite.ExecuteScalar();
                val = modified.ToString();
                id = Convert.ToInt32(val);

                sqlConnection1.Close();
            }

            return id;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = "Error de conexión SQL BD Portal";
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return -1;
        }
    }

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna)
    {
        try
        {
            string sql = "";
            MemoryStream memoryStream = new MemoryStream();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            if (consulta == "InvoiceKey")
                sql = @"SELECT " + columna + " FROM InvoiceFile WHERE InvoiceKey = @varID";
            else
                sql = @"SELECT " + columna + " FROM InvoiceFile WHERE InvcFileKey = @varID";


            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        //using (var fs = new MemoryStream(memoryStream, FileMode.Create, FileAccess.Write)) {
                        memoryStream.Write(blob, 0, blob.Length);
                        //}
                    }
            }

            sqlConnection1.Close();


            return memoryStream;
        }
        catch (Exception ex)
        {
            string err;
            err = ex.InnerException.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return null;
        }
    }
      

}