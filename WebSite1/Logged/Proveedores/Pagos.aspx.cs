using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using uCFDsLib.v33;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ConsultaSAT; // SAT
using System.Web;
using System.Web.UI;
using System.Net.Mail;
using System.Net;

public partial class Pagos : System.Web.UI.Page
{
    int err = 0;
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        //string Variable = HttpContext.Current.Session["IDCompany"].ToString();
     
        if (!IsPostBack)
        {
            //Session["Proveedor"] = "000001";
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    if (UsuarioP.Items.Count == 0)
                    {
                        User_Empresas(UsuarioP);
                    }
                    Panel2.Visible = false;
                    GridView2.Visible = false;
                }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
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
            //Page.MasterPageFile = "MenuP.master";
            Response.Redirect("~/Logged/Proveedores/Default.aspx");

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

    private void BindGridView()
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            // Create a DataSet object. 
            DataSet dsProveedores = new DataSet();


            //// Create a SELECT query. 
            //string strSelectCmd = "select [vdvVendorPayment].[BankAcctID] AS [Cuenta Bancaria],[vdvVendorPayment].[VendID] AS [ID Prov], " +
            //"[vdvVendorPayment].[TranID] AS[Transacción],[vdvVendorPayment].[VendPmtMethDesc] AS [Metodo De Pago] ,[vdvVendorPayment].[PostDate] AS[Fecha Aplic.] " +
            //",[vdvVendorPayment].[TranAmt] AS[Importe Transacción],[vdvVendorPayment].[TranAmtHC] AS[Importe Moneda Local] " +
            //" FROM vdvVendorPayment where VendID='" + Session["Proveedor"].ToString() +"'" ;

            // Create a SELECT query. 
            string strSelectCmd = "select [vdvVendorPayment].[BankAcctID] AS [Cuenta Bancaria],[vdvVendorPayment].[VendID] AS [ID Prov], " +
            "[vdvVendorPayment].[TranID] AS[Transacción],[vdvVendorPayment].[VendPmtMethDesc] AS [Metodo De Pago] ,[vdvVendorPayment].[PostDate] AS[Fecha Aplic.] " +
            ",[vdvVendorPayment].[TranAmt] AS[Importe Transacción],[vdvVendorPayment].[TranAmtHC] AS[Importe Moneda Local] " +
            " FROM vdvVendorPayment ";

            string strSelectCmd2 = "SELECT tapVoucher.VouchNo AS Voucher,CONVERT(VARCHAR(10), tapVoucher.TranDate, 105) As Fecha,tapVoucher.TranCmnt AS Detalles,tapVoucher.TranID As Referencia,tapVoucher.TranAmt As Importe,tapVoucher.TranAmtHC As ImporteLocal "+
                                   "FROM tapVoucher WITH(NOLOCK),tapVendor WITH(NOLOCK) " +
                                   "WHERE tapVendor.VendID = 'XIKAR' AND tapVendor.CompanyID = 'IEP' AND " +
                                   "tapVoucher.CompanyID = 'IEP' AND tapVoucher.CreateDate > '2018-09-01' AND TranDate >= '2018-09-01' " +
                                   "AND((tapVoucher.Status = 1) OR(tapVoucher.Status = 2))";



            // Create a SqlDataAdapter object 
            // SqlDataAdapter represents a set of data commands and a  
            // database connection that are used to fill the DataSet and  
            // update a SQL Server database.  
            SqlDataAdapter da = new SqlDataAdapter(strSelectCmd2, conn);


            // Open the connection 
            conn.Open();


            // Fill the DataTable named "Person" in DataSet with the rows 
            // returned by the query.new n 
            da.Fill(dsProveedores, "Tabla");

            // Get the DataView from Person DataTable. 
            DataView dvPerson = dsProveedores.Tables["Tabla"].DefaultView;


            // Set the sort column and sort order. 
            //dvPerson.Sort = ViewState["SortExpression"].ToString();


            // Bind the GridView control. 
            //GridView1.DataSource = dvPerson;
            //GridView1.DataBind();  
        }

    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        //GridView1.PageIndex = e.NewPageIndex;


        // Rebind the GridView control to  
        // show data in the new page. 
        BindGridView();
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Make the GridView control into edit mode  
        // for the selected row.  
        //GridView1.EditIndex = e.NewEditIndex;

        //Panel1.Visible = true;
        //Panel2.Visible = true;
        //GridView2.Visible = true;


        // Rebind the GridView control to show data in edit mode. 
        BindGridView();


        // Hide the Add button. 
        //Panel1.Visible = true;
    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        //GridView1.EditIndex = -1;


        // Rebind the GridView control to show data in view mode. 
        BindGridView();
        //Panel1.Visible = false;
        //Panel2.Visible = false;
        GridView2.DataSource = "";
        GridView2.DataBind();
        GridView2.Visible = false;


        // Show the Add button. 
        //lbtnAdd.Visible = true;
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            
            // Rebind the GridView control to show data after updating. 
            //BindGridView();

        }
    }

    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        Verifica();
        //cargarArchivos(); 
        //ConsultaSAT(); //SAT
        //ValidaSAT();  //IEPT
    }

    protected void Verifica()
    {

        txt1.Text = "";
        txt2.Text = "";
        txt3.Text = "";
        txt4.Text = "";
        txt5.Text = "";
        txt6.Text = "";
        txt7.Text = "";
        txt8.Text = "";
        txt9.Text = "";
        txt10.Text = "";
        DataTable dt = new DataTable();
        GridView2.DataSource = dt;
        GridView2.DataBind();
        Panel2.Visible = false;


        string text= string .Empty;
        if (!FileUpload2.HasFile && !FileUpload1.HasFile)
        {
            text = "No se Encontraron Archivos";
        }
        else if (!FileUpload2.HasFile)
        {
            text = "Ingresa un XML";
        }

        else if (!FileUpload1.HasFile)
        {
            text = "Ingresa un PDF";
        }
        if (text != "")
        {
            Mensajes.Text = text;
            string Caja = "B1";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(" + Caja + ");", true);
        }
        else
        {
            string tipo, Msj, titulo;
            string Msj1 = cargarArchivos();
            if (err >=1)
            {
                tipo = "error";
                Msj = Msj1;
                titulo = "Carga De Pagos";
                //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                
                Label2.Text = Msj1;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
            }
            else
            {
                tipo = "success";
                Msj = Msj1;
                titulo = "Carga de Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }

        }
    }

    protected void Variables()
    {
        try
        {
            if (UsuarioP.SelectedItem.ToString() != "")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetvar", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@EmpresaV",
                        Value = UsuarioP.SelectedItem.ToString()
                    });

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@User",
                        Value = HttpContext.Current.Session["UserKey"].ToString()
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    string Prueba = HttpContext.Current.Session["UserKey"].ToString();
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable GV = new DataTable();
                    GV.Load(rdr);

                    if (GV.Rows.Count == 1)
                    {
                        DataRow Fila = GV.Rows[0];
                        HttpContext.Current.Session["VendKey"] = Fila["VendKey"].ToString();
                        HttpContext.Current.Session["IDComTran"] = Fila["Company"].ToString();
                    }
                    else
                    {
                        HttpContext.Current.Session["VendKey"] = "";
                        HttpContext.Current.Session["IDComTran"] = "";
                    }
                    conn.Close();
                }

            }
        }
        catch
        {

        }
    }

    protected void User_Empresas(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            string Userkey = User.Identity.Name.ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompanyLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = Userkey });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Empresas"].ToString() != "")
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Empresas"].ToString());
                        Caja.Items.Insert(0, Linea);
                        Caja.Visible = true;
                        Variables();
                    }
                    else
                    {
                        throw new Exception(Errores);
                    }


                }
                if (Caja.Items.Count == 0)
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = ("N/D");
                    Caja.Items.Insert(0, Linea);
                    Caja.Visible = true;
                    HttpContext.Current.Session["IDComTran"] = "";
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {
            Caja.Items.Clear();
        }
    }

    protected string cargarArchivos()
    {
        string Resultado = string.Empty;
        try
        {
            string complemento = "";
            string CompleT = "";

            string archivo;
            err = 0;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FileUpload2.PostedFile.InputStream);
            StringReader reader = new StringReader(xmlDoc.InnerXml);
            Comprobante Factura = new Comprobante();
            XmlSerializer Xml = new XmlSerializer(Factura.GetType());
            Factura = (Comprobante)Xml.Deserialize(reader);

            StreamReader objStreamReader;

            if (Factura.Complemento[0].Any != null && Factura.Complemento[0].Any.Length > 0)
            {
                if (Factura.Complemento[0].Any[0].Name == "pago10:Pagos") {complemento = Factura.Complemento[0].Any[0].OuterXml; } else {throw new Exception("El comprobante No cuenta con nodo de Pagos, verificar."); }
                if (Factura.Complemento[0].Any[1].Name == "tfd:TimbreFiscalDigital") {CompleT = Factura.Complemento[0].Any[1].OuterXml;} else {throw new Exception("El comprobante No Cuenta con Sello Digital, verificar.");}

                Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(complemento));
                Stream x = new MemoryStream(ASCIIEncoding.Default.GetBytes(CompleT));

                uCFDsLib.v33.Pagos Pagos = new uCFDsLib.v33.Pagos();
                objStreamReader = new StreamReader(s);
                Xml = new XmlSerializer(Pagos.GetType());
                Pagos = (uCFDsLib.v33.Pagos)Xml.Deserialize(objStreamReader);
                objStreamReader.Close();

                uCFDsLib.v33.TimbreFiscalDigital timbre = new uCFDsLib.v33.TimbreFiscalDigital();
                objStreamReader = new StreamReader(x);
                Xml = new XmlSerializer(timbre.GetType());
                timbre = (uCFDsLib.v33.TimbreFiscalDigital)Xml.Deserialize(objStreamReader);
                objStreamReader.Close();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.InnerXml = complemento;


                string Feca = Pagos.Pago.FechaPago.ToString();
                string fechaD = Factura.Fecha.ToString();
                DateTime fechaF = Convert.ToDateTime(Feca).Date;
                DateTime fechaDoc = Convert.ToDateTime(fechaD).Date;
                DateTime FechAc = DateTime.Now.Date.AddMonths(-1);

                if (fechaF <= FechAc) {Resultado = "LA fechad de pago excede de 1 mes de antiguedad"; err = err + 1; return Resultado; }

                if (fechaDoc <= FechAc) { Resultado = "La fecha del XML excede a mas de 1 mes de Antiguedad"; err = err + 1; return Resultado; }

                //string SAT = ConsultaSAT(Factura.Emisor.Rfc, Factura.Receptor.Rfc, Pagos.Pago.Monto, timbre.UUID);
                //if (SAT != "Vigente")
                //{
                //    Resultado = "El Comprobante Se Encuentra Como " + SAT + "  Ante el SAT, Verificalo";
                //    err = err + 1;
                //    return Resultado;
                //}

                string ReSAGE = RevisaSAGE(Factura,Pagos);
                if (ReSAGE != "") {err = err + 1; return ReSAGE;}

                string UUID, Fol, Serie, F1, F2, Ver, Totl;
                if (timbre.UUID == null) {UUID = "";} else {UUID = timbre.UUID.ToString();}
                if (Factura.Folio == null) {Fol = "";} else {Fol = Factura.Folio.ToString();}
                if (Factura.Serie == null) {Serie = "";} else {Serie = Factura.Serie.ToString();}
                if (Factura.Fecha == null) {F1 = "";} else {F1 = Factura.Fecha.ToShortDateString();}
                if (Pagos.Pago.FechaPago == null) {F2 = "";} else {F2 = Factura.Fecha.ToShortDateString();}
                if (Factura.Version == null) {Ver = "";} else {Ver = Factura.Version.ToString();}
                if (Pagos.Pago.Monto.ToString() == null){Totl = "";} else {Totl = Pagos.Pago.Monto.ToString(); }

                string Respues = Execute(FileUpload2, FileUpload1, UUID, Fol, Serie, F1, F2, Ver, Totl).ToString();
                if (Respues != "1")
                {   ResetPago(UUID);
                    Resultado = "Se encontrarón problemas al cargar los datos generales del comprobante, Verifica que estén correctos y/o que no se haya cargado anteriormente este documento ";
                    err = err + 1;
                    return Resultado;
                }


                DoctoRelacionado Docu1 = new DoctoRelacionado();
                for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
                {
                    Docu1 = Pagos.Pago.DoctoRelacionado[j];
                    string UUIP = Docu1.IdDocumento;
                    string FolioP = Docu1.Folio;
                    string Mon = Docu1.MonedaDR.ToString();
                    string Meto = Docu1.MetodoDePagoDR;
                    string PArc = Docu1.NumParcialidad;
                    string SalT = Docu1.ImpSaldoAnt.ToString();
                    string PAgo = Docu1.ImpPagado.ToString();
                    string Saldo = Docu1.ImpSaldoInsoluto.ToString();
                    if (Desglose(UUIP, UUID, PAgo, PArc, SalT, Saldo, FolioP, Mon, Meto) == false)
                    {
                        ResetPago(UUID);
                        Resultado = "Se encontrarón problemas al cargar el desglose del comprobante, Verifica que los datos estén correctos";
                        err = err + 1;
                        return Resultado;
                    }
                }

                if (Pagos.Pago.FechaPago == null) {txt2.Text = "N/D";} else {txt2.Text = Pagos.Pago.FechaPago.ToShortDateString();}
                if (Pagos.Pago.MonedaP.ToString() == "") {txt3.Text = "N/D";} else {txt3.Text = Pagos.Pago.MonedaP.ToString();}
                if (Pagos.Pago.FormaDePagoP.ToString() == null) {txt5.Text = "N/D";} else {txt5.Text = Pagos.Pago.FormaDePagoP.ToString();}
                if (Pagos.Pago.Monto.ToString("C") == null) {txt1.Text = "N/D";} else {txt1.Text = Pagos.Pago.Monto.ToString("C");}
                if (Factura.Fecha == null) {txt6.Text = "N/D";} else {txt6.Text = Factura.Fecha.ToShortDateString();}
                if (Factura.Serie == null) {txt8.Text = "";} else {txt8.Text = Factura.Serie.ToString();}
                if (timbre.UUID == null) { txt7.Text = "N/D";} else { txt7.Text = timbre.UUID.ToString();}
                if (Factura.Folio == null) { txt9.Text = "N/D";} else { txt9.Text = Factura.Folio.ToString();}
                if (Factura.Version == null) {txt10.Text = "";} else {txt10.Text = Factura.Version.ToString();}
                if (Pagos.Pago.TipoCambioP.ToString() != "0") {txt4.Text = Pagos.Pago.TipoCambioP.ToString();} else {txt4.Text = "1";}

                if (Pagos.Pago.NumOperacion == null) { TextBox1.Text = "N/D"; } else { TextBox1.Text = Pagos.Pago.NumOperacion.ToString(); }
                if (Pagos.Pago.RfcEmisorCtaOrd == null) { TextBox2.Text = "N/D"; } else { TextBox2.Text = Pagos.Pago.RfcEmisorCtaOrd.ToString(); }
                if (Pagos.Pago.NomBancoOrdExt == null) { TextBox3.Text = "N/D"; } else { TextBox3.Text = Pagos.Pago.NomBancoOrdExt.ToString(); }
                if (Pagos.Pago.CtaOrdenante == null) { TextBox4.Text = "N/D"; } else { TextBox4.Text = Pagos.Pago.CtaOrdenante.ToString(); }
                if (Pagos.Pago.RfcEmisorCtaBen == null) { TextBox5.Text = "N/D"; } else { TextBox5.Text = Pagos.Pago.RfcEmisorCtaBen.ToString(); }


                //Vaciado de Documentos Relacionados

                DataTable dt = new DataTable();
                dt.Columns.Add("UUID");
                dt.Columns.Add("Folio");
                dt.Columns.Add("Moneda");
                dt.Columns.Add("Metodo Pago");
                dt.Columns.Add("Parcialidad");
                dt.Columns.Add("SaldoAnt");
                dt.Columns.Add("Pago");
                dt.Columns.Add("NvoSaldo");

                DoctoRelacionado Docu = new DoctoRelacionado();

                for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
                {
                    DataRow row = dt.NewRow();
                    Docu = Pagos.Pago.DoctoRelacionado[j];

                    row["UUID"] = Docu.IdDocumento;
                    row["Folio"] = Docu.Folio;
                    row["Moneda"] = Docu.MonedaDR;
                    row["Metodo Pago"] = Docu.MetodoDePagoDR;
                    row["Parcialidad"] = Docu.NumParcialidad;
                    row["SaldoAnt"] = Docu.ImpSaldoAnt.ToString("C");
                    row["Pago"] = Docu.ImpPagado.ToString("C");
                    row["NvoSaldo"] = Docu.ImpSaldoInsoluto.ToString("C");
                    dt.Rows.Add(row);
                }
                Panel2.Visible = true;
                GridView2.DataSource = dt;
                GridView2.DataBind();
                GridView2.Visible = true;

                err = 0;
                Resultado = "Carga de Complemento de Pago Exitosa";
            }
            else
            {
                Resultado = "Se Generaron Problemas al Cargar el Complemento, verificar.";
            }
        }
        catch (Exception ex)
        {   
            err = err + 1;
            if (ex.Message == null)
            {
                Resultado = "Documento Invalido";
            }
            else
            {
                if (ex.Message != null) { Resultado = ex.Message; }
            }
            
        }

        return Resultado;
    }

    private bool Desglose(string UUIDP,string Fac,string pago, string Parcial,string SalT, string SAlN,string FolP, string Mon,string Meto)
    {   
        bool Res = false;
        int key = 0;
        int UUID = 0;
        string  Fk = string.Empty;
        string Pk = string.Empty;

        try
        {
            // foreach (GridViewRow gvr in GridView2.Rows)
            //{
            SalT = SalT.Replace(",", ".");
            SAlN = SAlN.Replace(",", ".");
            pago = pago.Replace(",", ".");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                    string Caden = "spGetPayAppl";
                    SqlCommand cmd1 = new SqlCommand(Caden, conn);
                    cmd1.CommandType = CommandType.StoredProcedure;

                    cmd1.Parameters.Add(new SqlParameter()
                    { ParameterName = "@UPago", Value = UUIDP });

                    cmd1.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Factura", Value = Fac });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr1 = cmd1.ExecuteReader();

                    while (rdr1.Read())
                    {
                        Pk = rdr1["PKey"].ToString();
                        Fk = rdr1["FKey"].ToString();
                    }

                    key = int.Parse(Pk.ToString());
                    UUID = int.Parse(Fk.ToString());


                    if (UUID != 0)
                    {
                    /////////////////////////////////////////////////////////////////////////////////

                    string Cades = "spInsertPayAppl";
                    string Result = string.Empty;
                    SqlCommand cmd = new SqlCommand(Cades, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymentKey", Value = key });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplUUID", Value = UUIDP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplFolio", Value = FolP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymtApplied", Value = pago });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PartialNumber", Value = Parcial });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceAnt", Value = SalT });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceOut", Value = SAlN });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Moneda", Value = Mon });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@MetodoPago", Value = Meto });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Folio", Value = UUID });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr["Resultado"].ToString() == "1")
                        {
                           Res = true;
                        }
                        else
                        {
                            Res = false;
                        }
                    }
                 }
               }
            //}
        }
        catch(Exception ex)
        {
            string Error = ex.Message;
            Res = false;
        }
        return Res;
    }

    protected string ConsultaSAT(string RFC1,string RFC2,decimal Total,string UUID)
    {
        string SAT = string.Empty;
        try
        {
            //string querySend = "?re=" + "IEP911010UG5" + "&rr=" + "XAXX010101000" + "&tt=" + "5400.00" + "&id=" + "82D4BB4B-3D0A-2548-B72B-9D19D22CF6D7";
            string querySend = "?re=" + RFC1 + "&rr=" + RFC2 + "&tt=" + Total + "&id=" + UUID;
            
            ConsultaCFDIServiceClient consulta = new ConsultaCFDIServiceClient();
            ConsultaSAT.Acuse Acuse = new ConsultaSAT.Acuse();

            Acuse = consulta.Consulta(querySend);
            consulta.Close();

            if (Acuse.Estado.ToLower() == "vigente")
            {
                SAT = "vigente";
            }
            else if (Acuse.Estado.ToLower() == "cancelado")
            {
                SAT = "Cancelado";
            }
            else
            {
                SAT = "Desconocido";
            }
        }

        catch
        {
            SAT = "3";  
        }
        return SAT;
    }

    protected void BtnEnviar(object sender,EventArgs e)
    {
        try
        {
            string tipo, Msj, titulo;

            if (Execute(FileUpload1,FileUpload2,"","","","","","","") == "1")
            {
                txt1.Text = "";
                txt2.Text = "";
                txt3.Text = "";
                txt4.Text = "";
                txt5.Text = "";
                txt6.Text = "";
                txt7.Text = "";
                txt8.Text = "";
                txt9.Text = "";
                txt10.Text = "";
                ListEm();
                DataTable dt = new DataTable();
                GridView2.DataSource = dt;
                GridView2.DataBind();
                Panel2.Visible = false;
                tipo = "succes";
                Msj = "Carga Exitosa";
                titulo = "Carga de Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                
            }
            else
            {
                tipo = "error";
                Msj = "Carga Un Archivo";
                titulo = "Carga De Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    protected void Carga()
    {
        string tipo, Msj, titulo;
        string Respues = string.Empty;


        if (FileUpload1.FileName.ToString() != "" && FileUpload2.FileName.ToString() != "")
        {           
            if (Respues != "1")
            {   
               
                tipo = "succes";
                Msj = "Carga Exitosa";
                titulo = "Carga de Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                tipo = "error";
                Msj = "Se encontraron Errores al Subir el Archivo, Validalos";
                titulo = "Carga De Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        else
        {
            tipo = "error";
            Msj = "Se encontraron Errores al Subir el Archivo, Verifica que se haya Cargado el Archivo";
            titulo = "Carga De Pagos";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }




    }

    public string Execute(FileUpload Caja,FileUpload Caja2,string UUID,string Folio,string Serie,string F1,string F2,string Version,string Total)
     {
        string res;

        try
        {
            string Filename = Caja.FileName.ToString();

            //Fabian
            Stream fsr = Caja.PostedFile.InputStream;
            System.IO.BinaryReader br3 = new System.IO.BinaryReader(fsr);
            Byte[] bytes3 = br3.ReadBytes((Int32)fsr.Length);

            Stream fsr1 = Caja2.PostedFile.InputStream;
            System.IO.BinaryReader br4 = new System.IO.BinaryReader(fsr1);
            Byte[] bytes4 = br4.ReadBytes((Int32)fsr1.Length);

            //}
            string Ext = ".XML";
            string Ext2 = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();

            Total = Total.Replace(",", ".");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Cades = "spInsertPay";
                string Result = string.Empty;
                SqlCommand cmd = new SqlCommand(Cades, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File", Value = bytes3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File2", Value = bytes4 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Vendedor", Value = HttpContext.Current.Session["VendKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = UsuarioP.SelectedItem.ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = HttpContext.Current.Session["UserKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@F1", Value = F1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@F2", Value = F2 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UUID", Value = UUID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Serie", Value = Serie });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Folio", Value = Folio });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Version", Value = Version });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Total", Value = Total });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext", Value = Ext });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext2", Value = Ext2 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "1")
                    {
                        Result = rdr["Resultado"].ToString();
                    }
                    else
                    {
                        Result = rdr["Resultado"].ToString();
                        throw new Exception(Result);
                    }
                }
                res = Result;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return res;
        }
        catch (Exception Rt)
        {
            res = Rt.Message;
            return res;
        }
    }

    protected void ResetPago(string UU)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spResetPay";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@UPago", Value = UU });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
            }
        }
        catch
        {

        }

    }

    protected string RevisaSAGE(Comprobante xml, uCFDsLib.v33.Pagos Pagos)
    {
        string Res = string.Empty;
        try
        {   


            /// DATOS GENERALES XML
            if (xml.TipoDeComprobante.ToString() != "P") { Res = "El tipo de comprobante debe ser 'P'" + "<br/>"; }
            if (Convert.ToDecimal(xml.Total.ToString()) >0) { Res = Res + "El Total de comprobante debe ser 0" + "<br/>"; }
            if (xml.SubTotal.ToString() != "0") { Res = Res + "El Subtotal de comprobante esta mal" + "<br/>"; }
            if (xml.Conceptos.Count() > 1) { Res = Res + "Demasiados conceptos dentro del Nodo 'Conceptos'" + "<br/>"; }

            ComprobanteConcepto Conceptos = new ComprobanteConcepto();
            Conceptos = xml.Conceptos[0];
            string strItemIdSAT = xml.Conceptos[0].ClaveProdServ.ToString().Substring(xml.Conceptos[0].ClaveProdServ.ToString().Length - 8, 8);
            //Conceptos
            if (Convert.ToDecimal(Conceptos.Importe.ToString()) > 1) { Res = Res + "La Cantidad en el concepto del Comprobante debe ser 1" + "<br/>"; }
            if (Convert.ToDecimal(Conceptos.Importe.ToString()) > 0) { Res = Res + "El importe en el concepto del Comprobante debe ser 0" + "<br/>"; }
            if (Convert.ToDecimal(Conceptos.ValorUnitario.ToString()) > 0) { Res = Res + "El valor unitario en el concepto del Comprobante debe ser 0" + "<br/>"; }
            if (Conceptos.Descripcion.ToString() != "Pago") { Res = Res + "La descripcion en el concepto del Comprobante debe ser 'Pago'" + "<br/>"; }
            if (Conceptos.ClaveUnidad.ToString() != "ACT") { Res = Res + "La Clave Unidad en el concepto del Comprobante debe ser 'ACT'" + "<br/>"; }
            if (strItemIdSAT != "84111506") { Res = Res + "La Clave Producto Servicio en el concepto del Comprobante debe ser '84111506'" + "<br/>"; }

            //Facturas Pagadas
            DoctoRelacionado Docu1 = new DoctoRelacionado();
            if (Pagos.Pago.CtaOrdenante == null) { Res = Res + "El comprobante no cuenta con el atributo CtaOrdenante en el nodo de pagos" + "<br/>"; }
            if (Pagos.Pago.NomBancoOrdExt == null) { Res = Res + "El comprobante no cuenta con el atributo NomBancoOrdExt en el nodo de pagos" + "<br/>"; }
            if (Pagos.Pago.NumOperacion == null) { Res = Res + "El comprobante no cuenta con el atributo NumOperacion en el nodo de pagos" + "<br/>"; }
            if (Pagos.Pago.RfcEmisorCtaBen == null) { Res = Res + "El comprobante no cuenta con el atributo RfcEmisorCtaBen en el nodo de pagos" + "<br/>"; }
            if (Pagos.Pago.RfcEmisorCtaOrd == null) { Res = Res + "El comprobante no cuenta con el atributo RfcEmisorCtaOrd en el nodo de pagos" + "<br/>"; }

            if (ReDoc(Pagos.Pago.CtaOrdenante, Pagos.Pago.RfcEmisorCtaOrd, Pagos.Pago.Monto, Pagos.Pago.FechaPago.ToString()) == false) { Res = Res + "Los datos Bancarios no coiciden con los registrados en T|SYS| o la Empresa destino no coincide con los datos del pago,Revisalos con los proporcionados en el Email de Confirmación" + "<br/>"; }


            if (Pagos.Pago.DoctoRelacionado.Count() == 0) { Res = Res + "El comprobante no cuenta Documententos relacionados en el nodo de pagos" + "<br/>"; }
            else { 
            for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
            {  
                Docu1 = Pagos.Pago.DoctoRelacionado[j];
                string UUIP = Docu1.IdDocumento;
                string FolioP = Docu1.Folio;
                string Mon = Docu1.MonedaDR.ToString();
                string Cta = Pagos.Pago.CtaOrdenante.ToString();
                string Meto = Docu1.MetodoDePagoDR;
                string SalT = Docu1.ImpSaldoAnt.ToString();
                string SalN = Docu1.ImpSaldoInsoluto.ToString();
                string PAgo = Docu1.ImpPagado.ToString();
                decimal Pag = (Convert.ToDecimal(PAgo.ToString()));
                decimal SaldoN = (Convert.ToDecimal(SalN.ToString()));

                string RFCE = xml.Emisor.Rfc;
                string RFCR = xml.Receptor.Rfc;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                  string Caden = "spGetPmtDetail";
                  SqlCommand cmd1 = new SqlCommand(Caden, conn);
                  cmd1.CommandType = CommandType.StoredProcedure;
                  cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUIP });
                  if (conn.State == ConnectionState.Open) { conn.Close(); }
                  conn.Open();
                  SqlDataReader rdr1 = cmd1.ExecuteReader();
                  if (rdr1.HasRows)
                  {
                   while (rdr1.Read())
                    {
                     if (rdr1["RFCEmisor"].ToString() != RFCE) { Res = Res + "El RFC Emisor del comprobante No coincide con el RFC Emisor Registrado para el comprobante con Folio  " + FolioP + "<br/>"; }
                     if (rdr1["RFCReceptor"].ToString() != RFCR) { Res = Res + "El RFC Receptor del comprobante No coincide con el RFC Receptor Registrado para el comprobante con Folio " + FolioP + "<br/>"; }
                     int Sta = Convert.ToInt16(rdr1["Status"].ToString());
                     if (Sta >= 6) { Res = Res + "El comprobante no está habilitado para recibir complemento de Pago." + "<br/>";}
                     if (Sta == 9) { Res = Res + "El comprobante ya se encuentra registrado como pagado." + "<br/>";}
                     if (rdr1["Folio"].ToString() != FolioP) { Res = Res + "El Folio ingresado para el comprobante con Folio  " + FolioP + " no coincide con el registrado en T|SYS|" + "<br/>";}
                     if (rdr1["MetodoPago"].ToString() != Meto) { Res = Res + "El Metodo de Pago ingresado para el comprobante con Folio  " + FolioP + " no coincide con el registrado en T|SYS|" + "<br/>"; }
                     if (rdr1["Moneda"].ToString() != Mon.ToString()) { Res = Res + "El tipo de Moneda registrado para el comprobante con Folio " + FolioP + " no coincide con el registrado en T|SYS|" + "<br/>"; }
                     decimal Sal = (Convert.ToDecimal(rdr1["Total"].ToString()));
                     decimal Resta = (Sal - Pag);
                     if (Resta < 0) { Res = Res + "El importe registrado para el comprobante con Folio " + FolioP + " excede contra el Total registrado en T|SYS|" + "<br/>"; }
                     if (Resta != SaldoN) { Res = Res + "El Nuevo Saldo del comprobante con Folio " + FolioP + " No coincide con el Nuevo Saldo Aplicando al Pago" + "<br/>"; }

                   }
                 }
                 else
                 {
                  Res = Res + "No se encontro ningún registro de alguna factura con el UUID " + UUIP + "<br/>";
                  conn.Close();
                  return Res;
                 }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Res = ex.InnerException.Message;
        }
        return Res;
    }

    protected bool ReDoc(string Cuenta,string RFCB,decimal Pago,string Fecha)
    {
        bool Doc = false;
        try
        {
            Fecha = Fecha.Substring(0, 10);
            Fecha = Fecha.Replace("/", "-");
            string UserKey = HttpContext.Current.Session["UserKey"].ToString();
            string Company = UsuarioP.SelectedItem.ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter(){ ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Cuenta", Value = Cuenta });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@RFCB", Value = RFCB });
                if (conn.State == ConnectionState.Open){conn.Close();}
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = true;}
            }
        }
        catch(Exception ex)
        {
            string mess = ex.Message;
        }
        return Doc;

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

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionPagos.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }

            Global.EmailGlobal(Destinatario, body, "CARGA DE COMPLEMENTO DE PAGOS");

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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Notificación Pagos" });
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

}