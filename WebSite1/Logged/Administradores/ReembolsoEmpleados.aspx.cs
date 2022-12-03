//PORTAL DE PROVEDORES T|SYS|
//20 - JULIO, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA
//PANTALLA PARA REEMBOLSOS DE EMPLEADOS
//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using uCFDsLib.v33;
using uCFDsLib.v40;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Collections;
using ComprobanteComplemento = uCFDsLib.v33.ComprobanteComplemento;
using ComprobanteAddenda = uCFDsLib.v33.ComprobanteAddenda;
using ComprobanteConceptoImpuestos = uCFDsLib.v33.ComprobanteConceptoImpuestos;
using c_Impuesto = uCFDsLib.v33.c_Impuesto;
using Proveedores_Model;
using WebSite1;


public partial class Logged_Administradores_ReembolsoEmpleados : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private bool is_valid;
    private PortalProveedoresEntities _context = new PortalProveedoresEntities();   

    private List<ItemDTO> gridList = new List<ItemDTO>(); 
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
    string titulo = "T|SYS|", Msj, tipo;

    #endregion

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
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

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
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
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
    public void fill_fileUploads()
    {
        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            
            var f_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
            foreach (HttpPostedFile file in f_pdf.PostedFiles)
            {
                tbx_pdf.Text += file.FileName + " ";
            }
            
        }
        if (fu_pdf.HasFiles)
        {
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
        }


        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            var f_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
            foreach (HttpPostedFile file in f_xml.PostedFiles)
            {
                tbx_xml.Text += file.FileName + " ";
            }           
        }
        if (fu_xml.HasFiles)
        {
            HttpContext.Current.Session["fu_xml"] = fu_xml;
        }

        if (HttpContext.Current.Session["fu_voucher"] != null)
        {
            var f_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
            foreach (HttpPostedFile file in f_voucher.PostedFiles)
            {
                tbx_voucher.Text += file.FileName + " ";
            }           
        }
        if (fu_voucher.HasFiles)
        {
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        List<RolDTO> roles = Doc_Tools.get_Roles();

        try
        {
            if (!roles.Any(x=> x.ID == HttpContext.Current.Session["RolUser"].ToString()))
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                try
                {
                    //string Var = HttpContext.Current.Session["VendKey"].ToString();
                    //string Var2 = HttpContext.Current.Session["RolUser"].ToString();
                    //string Var3 = HttpContext.Current.Session["IDCompany"].ToString();

                    //pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    fill_fileUploads();
                    BindGridView();

                    if (HttpContext.Current.Session["is_valid"] != null)
                    {
                        is_valid = (bool)HttpContext.Current.Session["is_valid"];
                    }
                    else
                    {
                        HttpContext.Current.Session["is_valid"] = false;
                    }
                    if (HttpContext.Current.Session["motivo"] != null)
                    {
                        tbx_motivo.Text = HttpContext.Current.Session["motivo"].ToString();
                    }

                    btnSage.Enabled = is_valid;
                    pVendKey = 0;
                    MultiView1.SetActiveView(View_General);                    
                   
                    
                    //pLogKey = 0;
                    //pUserKey = 0;
                    //pCompanyID = "";

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;
                        //Sage();
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, "TSM");
                }
            }
            //Si no es carga inicial
            if (!IsPostBack)
            {
                get_anticipos(pUserKey);              
                get_items(pCompanyID);
                get_taxes();
                HttpContext.Current.Session["GridItems"] = null;
                HttpContext.Current.Session["GridTaxes"] = null;                
            }           

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
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

        }
        catch (Exception ex)
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

    private bool Check_Exist(int userKey, decimal importe)
    {
        decimal count = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*) FROM Expense where UpdateUserKey = @UpdateUserKey and Amount = @Amount and UpdateDate = @UpdateDate";
            cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userKey;
            cmd.Parameters.Add("@UpdateDate", SqlDbType.Date).Value = DateTime.Today.Date;
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                count = lector.GetInt32(0);
            }            
            if(count > 0)
            {
                return true;
            }            
        }
        return false;
    }

    private bool Check_Advance_Date(int userkey, int advance_id, DateTime fecha_gasto)
    {
        DateTime CheckDate = DateTime.Now;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CheckDate FROM Advance where UpdateUserKey = @UpdateUserKey and AdvanceId = @AdvanceId";
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;            
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                CheckDate = lector.GetDateTime(0);
            }
            if (CheckDate < fecha_gasto)
            {
                return false;
            }
        }
        return true;
    }

    private void Sage() 
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];          
           
            fu_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];                    

            fu_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];

            if(HttpContext.Current.Session["motivo"]!=null &&  string.IsNullOrEmpty(tbx_motivo.Text))
            {
                tbx_motivo.Text = HttpContext.Current.Session["motivo"].ToString();
            }
           
            
            int advance_id = int.Parse(drop_anticipos.SelectedValue);
            var advance = LoadAdvanceById(advance_id, pUserKey);

            int tipo_moneda = advance.Currency;
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);            
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);
            int AdvanceId = int.Parse(drop_anticipos.SelectedValue);
            string motivo_gasto = tbx_motivo.Text;
            int status = 1;
            var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            if (rol != "T|SYS| - Empleado")
            {
                List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
                if (roles.FirstOrDefault(x => x.ID == rol).Key == roles.Max(z => z.Key))
                {
                    status = 2;
                }
            }

            int carga = WriteToDb(tipo_moneda, fecha_gasto, importe_gasto, fu_xml, fu_pdf, fu_voucher, pUserKey, pCompanyID, AdvanceId, lista_detalles, status, motivo_gasto);

            if (carga == -1)
            {
                gvGastos.DataSource = null;               
                gvGastos.Visible = false;                           
                tbx_importe.Text = Math.Round(lista_detalles.Sum(x => x.Amount + x.TaxAmount), 2).ToString("0.00");
                HttpContext.Current.Session["is_valid"] = false;
                btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B4").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_General);
                return;
            }

            //Logica de vencer el anticipo
            Vencer_Advance(advance_id, pUserKey);

            EnviarCorreo();
            BindGridView();
            ClearControls();
            HttpContext.Current.Session["GridItems"] = null;
            HttpContext.Current.Session["GridTaxes"] = null;
            GvItems.DataSource = null;
            GvItems.DataBind();
            HttpContext.Current.Session["is_valid"] = false;
            btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
            Response.Redirect(Page.Request.RawUrl);
        }
        else
        {
            btnSage.Enabled = false;
        }      

    }

    private bool Check_Advance_Amount(int advance_id, decimal expense_amount, int user_key)
    {
        decimal advance_amount = 0;        
        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM Advance where AdvanceId = @AdvanceId and UpdateUserKey = @UpdateUserKey";
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_key;
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while(lector.Read())
            {
                advance_amount = lector.GetDecimal(0);
            }
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT sum(Amount) as Amount FROM Expense where AdvanceId = @AdvanceId and UpdateUserKey = @UpdateUserKey";
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_key;
            while (lector.Read())
            {
                expense_amount += lector.GetDecimal(0);
            }
            cmd.Connection.Close();

            if(expense_amount>advance_amount)
            {
                return false;
            }
            return true;
        }
    }

    private void Vencer_Advance(int advance_id, int user_key)
    {
        decimal advance_amount = 0, expense_amount = 0;

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM Advance where AdvanceId = @AdvanceId and UpdateUserKey = @UpdateUserKey";
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_key;
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                advance_amount = lector.GetDecimal(0);
            }

            cmd.Connection.Close();

            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT sum(Amount) as Amount FROM Expense where AdvanceId = @AdvanceId and UpdateUserKey = @UpdateUserKey";
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_key;
            cmd.Connection.Open();
            lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                expense_amount += lector.GetDecimal(0);
            }
            cmd.Connection.Close();

            if (expense_amount >= advance_amount)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE Advance SET Status = 4 where AdvanceId = @AdvanceId and UpdateUserKey = @UpdateUserKey";
                cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_key;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            
        }
    }

    private void BindGridView()
    {
        gvGastos.DataSource = null;       
        gvGastos.Visible = true;
        gvGastos.DataSource = ReadFromDb(pUserKey);
        gvGastos.DataBind();
    }

    private void ClearControls()
    {
        tbx_fechagasto.Text = string.Empty;
        tbx_importe.Text = string.Empty;       
        STipoGasto.ClearSelection();
        tbx_currency.Text = string.Empty;
        drop_anticipos.ClearSelection();        
        HttpContext.Current.Session["fu_xml"] = null;
        HttpContext.Current.Session["fu_pdf"] = null;
        HttpContext.Current.Session["fu_voucher"] = null;
        HttpContext.Current.Session["xml_files"] = null;
        HttpContext.Current.Session["pdf_files"] = null;
        HttpContext.Current.Session["voucher_files"] = null;
        HttpContext.Current.Session["motivo"] = null;
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        if (fu_pdf != null)
        {
            fu_pdf.Attributes.Clear();
        }    
        if(fu_xml!=null)
        {
            fu_xml.Attributes.Clear();
        }
        if(fu_voucher!=null)
        {
            fu_voucher.Attributes.Clear();
        }       
       
        btnSage.Enabled = false;
        tbx_motivo.Text = string.Empty;
    }

    /// <summary>
    /// Metodo para Guardar el Gasto en BD
    /// </summary>
    /// <returns>Entero</returns>
    private int WriteToDb(int tipo_moneda, DateTime fecha_gasto, decimal importe_gasto, FileUpload fu_xml, FileUpload fu_pdf, FileUpload fu_voucher, int userkey, string companyId, int AdvanceId, List<ExpenseDetailDTO> expenseDetails, int status, string motivo_gasto)
    {
        string val = "";
        int id = 0;
        int? approval_level = null;        
        var xml_files = new List<ExpenseFilesDTO>();      
        var pdf_files = new List<ExpenseFilesDTO>();      
        var voucher_files = new List<ExpenseFilesDTO>();      
        try
        {
          
            if(fu_xml!=null)
            {
                if (fu_xml.HasFiles && HttpContext.Current.Session["xml_files"]!=null)
                {
                    xml_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["xml_files"];
                }
            }
                   
            if (fu_pdf!=null)
            {
                if (fu_pdf.HasFiles && HttpContext.Current.Session["pdf_files"]!=null)
                {                   
                    pdf_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["pdf_files"];
                }
            }
          
            if (fu_voucher!=null)
            {
                if (fu_voucher.HasFiles && HttpContext.Current.Session["voucher_files"]!=null)
                {                    
                    voucher_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["voucher_files"];
                }
            }            

            //set Nivel
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            int role_key = Doc_Tools.get_Roles().FirstOrDefault(x => x.ID == rol).Key;
            if (role_key == 8)
            {
                approval_level = null;
            }
            else
            {
                approval_level = Doc_Tools.get_RolesValidadores().FirstOrDefault(x => x.ID == rol).Key;
            }


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
               SqlCommand cmd = conn.CreateCommand();               
               cmd.CommandText = "Insert Into Expense (Date, Currency, Amount, UpdateDate, UpdateUserKey, CompanyId, Status, AdvanceId, ApprovalLevel, ExpenseReason) VALUES (@Date, @Currency, @Amount,  @UpdateDate, @UpdateUserKey, @CompanyId, @Status, @AdvanceId, @ApprovalLevel, @ExpenseReason); SELECT SCOPE_IDENTITY();";
           
               cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = fecha_gasto;
               cmd.Parameters.Add("@Currency", SqlDbType.Int).Value = tipo_moneda;
               cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe_gasto;             
               cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
               cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;
               cmd.Parameters.Add("@CompanyId", SqlDbType.NVarChar).Value = companyId;
               cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status; //Estado inicial Pendiente
               cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = AdvanceId; 
               if(approval_level==null)
               {
                    cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = DBNull.Value;
               }
               else 
               {
                    cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = approval_level.Value;
               }
               cmd.Parameters.Add("@ExpenseReason", SqlDbType.VarChar).Value = motivo_gasto ?? (object)DBNull.Value;

               cmd.Connection.Open();
               var modified = cmd.ExecuteScalar();
               val = modified.ToString();
               id = Convert.ToInt32(val);

                foreach (ExpenseDetailDTO detail in expenseDetails)
                {                    
                    cmd.CommandText = "INSERT INTO ExpenseDetail (ExpenseId,Type,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId,STaxCodeKey,TaxAmount) VALUES (@_ExpenseId,@_Type,@_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser,@_CompanyId, @_STaxCodeKey, @_TaxAmount);";
                    cmd.Parameters.Add("@_ExpenseId", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@_Type", SqlDbType.Int).Value = detail.Type;
                    cmd.Parameters.Add("@_ItemKey", SqlDbType.Int).Value = detail.ItemKey;
                    cmd.Parameters.Add("@_Qty", SqlDbType.Decimal).Value = detail.Qty;
                    cmd.Parameters.Add("@_UnitCost", SqlDbType.Decimal).Value = detail.UnitCost;
                    cmd.Parameters.Add("@_Amount", SqlDbType.Decimal).Value = detail.Amount;
                    cmd.Parameters.Add("@_CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@_UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@_CreateUser", SqlDbType.Int).Value = userkey;
                    cmd.Parameters.Add("@_CompanyId", SqlDbType.VarChar).Value = companyId;                   
                    cmd.Parameters.Add("@_STaxCodeKey", SqlDbType.Decimal).Value = detail.STaxCodeKey;
                    cmd.Parameters.Add("@_TaxAmount", SqlDbType.Decimal).Value = detail.TaxAmount;
                    var inserted = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }                
               cmd.Connection.Close();

               foreach (ExpenseFilesDTO file in xml_files.Union(pdf_files).Union(voucher_files))
               {
                   file.ExpenseId = id;
                   int id_file = Doc_Tools.SaveFile(file);                 
               }               
            }

            return id;
        }
        catch (Exception ex)
        {
            if(id>0)
            {
                Doc_Tools.DeleteExpenseOnFail(Doc_Tools.DocumentType.Expense, id);
                Doc_Tools.DeleteDetailOnFail(Doc_Tools.DocumentType.Expense, id);
                Doc_Tools.DeleteFile(Doc_Tools.DocumentType.Expense,id);
            }
           
          
            LogError(pLogKey, pUserKey, "Insertar-Pago-Reembolso-Empleados:Insertar-Pago", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }       
    }
       
    public Dictionary<int, string> Dict_type()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Viaje" },
            { 2, "Compra Extraordinaria" }
        };
        return dict;
    }

    private List<ExpenseDTO> ReadFromDb(int user_id)
    {
        List<ExpenseDTO> gastos = new List<ExpenseDTO>();
        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ExpenseId ,Date ,Currency ,Amount, Status, AdvanceId FROM Expense where UpdateUserKey = @UpdateUserKey;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;               
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var expense = new ExpenseDTO();
                expense.ExpenseId = dataReader.GetInt32(0);              
                expense.Date = dataReader.GetDateTime(1);
                expense.Currency =  Doc_Tools.Dict_moneda().First(x=> x.Key== dataReader.GetInt32(2)).Value;
                expense.Amount = dataReader.GetDecimal(3);
                expense.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;
                expense.AdvanceId = dataReader.GetInt32(5);
                gastos.Add(expense);
            }
        }
        
        return gastos;

    }

    protected void btnSage_Click(object sender, EventArgs e)
    {
        Sage();        
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        //logica para enviar los correos
        //Logica para lanzar Reporte de Reembolsos
        Response.Redirect("~/Logged/Reports/Reembolsos");
    }    

    private bool CompruebaMontoFactura(MemoryStream fs, decimal importe)
    { 
        try
        {            
            StreamReader streamReader = null;
            TextReader reader = null;            
            XmlSerializer Xmls = null;

            uCFDsLib.v33.Comprobante Factura = new uCFDsLib.v33.Comprobante();
            uCFDsLib.v40.Comprobante Facturas = new uCFDsLib.v40.Comprobante();

            try
            {

                string xmlOutput = string.Empty;
                fs.Position = 0;
                streamReader = new StreamReader(fs);
                xmlOutput = streamReader.ReadToEnd();
                streamReader.Close();
                reader = new StringReader(xmlOutput);

                try
                {
                    Xmls = new XmlSerializer(Facturas.GetType());
                    Facturas = (uCFDsLib.v40.Comprobante)Xmls.Deserialize(reader);
                    if(Facturas.Total == importe)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        reader = new StringReader(xmlOutput);
                        Xmls = new XmlSerializer(Factura.GetType());
                        Factura = (uCFDsLib.v33.Comprobante)Xmls.Deserialize(reader);
                        if(Factura.Total == importe)
                        {
                            return true;
                        }
                    }
                    catch (Exception exs)
                    {
                        //LOG Err
                        HttpContext.Current.Session["Error"] = "Tu archivo no tiene la estructura valida por el SAT.";
                        string Mensaje = "Error al Deserializar el Archivo ";
                        Mensaje = Mensaje + exs.Message;
                        if (exs.InnerException != null)
                        {
                            Mensaje = Mensaje + " || " + exs.InnerException;
                        }
                        LogError(iLogKey, iUserKey, "Carga de Factura_CargarXML()", Mensaje, iCompanyID);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //LOG Err
                HttpContext.Current.Session["Error"] = "Tu archivo no tiene la estructura valida por el SAT.";
                string Mensaje = "Error al Deserializar el Archivo ";
                Mensaje = Mensaje + ex.Message;
                if (ex.InnerException != null)
                {
                    Mensaje = Mensaje + " || " + ex.InnerException;
                }
                LogError(iLogKey, iUserKey, "Carga de Factura_CargarXML()", Mensaje, iCompanyID);
                return false;
            }                     
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
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
        return false;
    }

    protected void STipoGasto_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        if (STipoGasto.SelectedValue!="")
        {
            tipo = "info";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == STipoGasto.SelectedValue).Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;

        }      
    }

    protected void tbx_importe_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //if (tbx_importe.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
        //{
        //    tbx_importe.Text = string.Empty;
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
        //    System.Threading.Thread.Sleep(5000);
        //    return;
        //}

        ////Validar el monto x tipo de Reembolso
        ////Alimentos, Hospedaje, Gastos Extraordinarios
        //string tipo_gasto = STipoGasto.SelectedValue;
        //switch (tipo_gasto)
        //{
        //    case "":
        //        if(tipo_gasto == string.Empty)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B13);", true);
        //            System.Threading.Thread.Sleep(5000);
        //            return;
        //        }               
        //        break;
        //    case "6": //Alimentos 200
        //        if(decimal.Parse(tbx_importe.Text) > 200)
        //        {                    
        //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B26);", true);
        //            System.Threading.Thread.Sleep(5000);                   
        //        }
        //        break;
        //    case "4":  //Gasolina  400
        //        if (decimal.Parse(tbx_importe.Text) > 400)
        //        {                    
        //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B27);", true);
        //            System.Threading.Thread.Sleep(5000);                   
        //        }
        //        break;
        //    case "8":  //Gastos Extraordinarios 2000
        //        if (decimal.Parse(tbx_importe.Text) > 2000)
        //        {                    
        //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B28);", true);
        //            System.Threading.Thread.Sleep(5000);                    
        //        }
        //        break;
        
       
    }
    
    private ExpenseDTO LoadExpenseById(int expense_id, int user_id)
    {
        var expense = new ExpenseDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ExpenseId ,Date ,Currency ,Amount, Status, AdvanceId, ExpenseReason FROM Expense where UpdateUserKey = @UpdateUserKey and ExpenseId = @ExpenseId and Status = 1;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {                
                expense.ExpenseId = dataReader.GetInt32(0);               
                expense.Date = dataReader.GetDateTime(1);
                expense.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                expense.Amount = dataReader.GetDecimal(3);
                expense.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;
                expense.AdvanceId = dataReader.GetInt32(5);                
                expense.ExpenseReason = dataReader.GetString(6);
            }
        }
        return expense;
    }

    private AdvanceDTO LoadAdvanceById(int advance_id, int user_id)
    {
        var advance = new AdvanceDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,UpdateDate,CompanyId,Status, Currency FROM Advance where UpdateUserKey = @UpdateUserKey and AdvanceId = @AdvanceId;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                advance.AdvanceId = dataReader.GetInt32(0);
                advance.AdvanceType = Dict_type().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
                advance.Folio = dataReader.GetString(2);
                advance.Amount = dataReader.GetDecimal(3);
                if (!dataReader.IsDBNull(4))
                {
                    advance.DepartureDate = dataReader.GetDateTime(4);
                }
                if (!dataReader.IsDBNull(5))
                {
                    advance.ArrivalDate = dataReader.GetDateTime(5);
                }
                advance.CheckDate = dataReader.GetDateTime(6);
                advance.ImmediateBoss = dataReader.GetString(7);
                advance.UpdateUserKey = dataReader.GetInt32(8);
                advance.UpdateDate = dataReader.GetDateTime(9);
                advance.CompanyId = dataReader.GetString(10);
                advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x => x.Key == dataReader.GetInt32(11)).Value;
                advance.Currency = dataReader.GetInt32(12);
            }
        }
        return advance;
    }

    protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        HttpContext.Current.Session["Expense"] = null;
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int expense_id = int.Parse(row.Cells[0].Text);
            var expense = LoadExpenseById(expense_id, pUserKey);
            expense.FileNameXml = Doc_Tools.LoadFilesbyExpense(Doc_Tools.DocumentType.Expense, ExpenseFilesDTO.FileType.Xml, expense_id);
            expense.FileNamePdf = Doc_Tools.LoadFilesbyExpense(Doc_Tools.DocumentType.Expense, ExpenseFilesDTO.FileType.Pdf, expense_id);
            expense.FileNamePdfVoucher = Doc_Tools.LoadFilesbyExpense(Doc_Tools.DocumentType.Expense, ExpenseFilesDTO.FileType.Voucher, expense_id);
            HttpContext.Current.Session["Expense"] = expense;
            Response.Redirect("EditReembolso");
        }

    }

    protected void gvGastos_RowDataBound(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[4].Text != "Pendiente")
            {
                Button btnEdit = (Button)e.Row.Cells[5].Controls[0];
                Button btnDelete = (Button)e.Row.Cells[6].Controls[1];
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }
        }
    }
    
    private void get_anticipos(int user_id)
    {
        var lista = new List<Tuple<int, string>>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AdvanceId , Folio FROM Advance where UpdateUserKey = @UpdateUserKey and status in (2,5);";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;            
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var advance_id = dataReader.GetInt32(0);
                var folio = dataReader.GetString(1);    
                lista.Add(new Tuple<int, string>(advance_id,folio));
               
            }
            lista.Add(new Tuple<int, string>(0, string.Empty));
            drop_anticipos.DataSource = lista.Select(d => new { id = d.Item1, folio = d.Item2 }).OrderBy(x=> x.id).ToList();
            drop_anticipos.DataTextField = "folio";
            drop_anticipos.DataValueField = "id";
            drop_anticipos.DataBind();
            drop_anticipos.SelectedIndex = -1;
         }       
    }

    private void get_items(string company_id)
    {
        List<ItemDTO> lista = Doc_Tools.get_items(company_id);                  
        drop_articulos.DataSource = lista;
        drop_articulos.DataTextField = "ItemId";
        drop_articulos.DataValueField = "ItemKey";
        drop_articulos.DataBind();
        drop_articulos.SelectedIndex = -1;        
        HttpContext.Current.Session["Items"] = null;
        HttpContext.Current.Session["Items"] = lista;
    }

    private void get_taxes()
    {        
        List<TaxesDTO>  lista = Doc_Tools.get_taxes(pCompanyID);
        drop_taxes.DataSource = lista;
        drop_taxes.DataTextField = "STaxCodeID";
        drop_taxes.DataValueField = "STaxCodeKey";
        drop_taxes.DataBind();
        drop_taxes.SelectedIndex = -1;
        HttpContext.Current.Session["Taxes"] = null;
        HttpContext.Current.Session["Taxes"] = lista;
    }    

    protected void GvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //Borrar articulo del grid   
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = GvItems.Rows[rowIndex];

        if (e.CommandName == "Select")
        {            
            int itemKey = int.Parse(row.Cells[0].Text);
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            if (lista.Count==1)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB50").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            var detail = lista.FirstOrDefault(x => x.ItemKey == itemKey);
            lista.Remove(detail);
            HttpContext.Current.Session["GridItems"] = lista;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = Math.Round(lista.Sum(x => x.Amount + x.TaxAmount), 2).ToString("0.00");
        }
    }

    protected void GvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }   

    protected void btn_additem_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //validacion de tipo Gasto
        if (string.IsNullOrEmpty(STipoGasto.SelectedValue.ToString()))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB26").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        //validaciones en articulos
        if (drop_articulos.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B40").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //validacion texto en importe
        if (tbx_importe_item.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        if (tbx_importe_item.Text == string.Empty || decimal.Parse(tbx_importe_item.Text) <= 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B41").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //validacion texto en cantidad
        if (tbx_cantidad.Text.Any(x => !char.IsNumber(x)))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B39").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        if (tbx_cantidad.Text == string.Empty || int.Parse(tbx_cantidad.Text) <= 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B39").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }           

        //Lista de articulos
        var items = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];

        //Agregar articulo al grid
        var detalle = new ExpenseDetailDTO();
        detalle.ItemKey = int.Parse(drop_articulos.SelectedItem.Value);
        detalle.Type = int.Parse(STipoGasto.SelectedValue);
        detalle.TipoGasto = STipoGasto.SelectedItem.Text;
        detalle.ItemId = drop_articulos.SelectedItem.Text;
        detalle.Qty = decimal.Parse(tbx_cantidad.Text);    
        detalle.UnitCost = decimal.Parse(tbx_importe_item.Text);
        if (drop_taxes.SelectedValue != "0")
        {
            detalle.STaxCodeKey = int.Parse(drop_taxes.SelectedItem.Value);
            detalle.STaxCodeID = drop_taxes.SelectedItem.Text;
            detalle.TaxAmount = taxes.FirstOrDefault(x => x.STaxCodeKey == detalle.STaxCodeKey).Rate * detalle.Amount;           
        }    


        if (HttpContext.Current.Session["GridItems"] !=null)
        {
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            lista.Add(detalle);
            HttpContext.Current.Session["GridItems"] = lista;
            GvItems.DataSource = null;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
        else
        {
            var lista = new List<ExpenseDetailDTO>
            {
                detalle
            };
            HttpContext.Current.Session["GridItems"] = lista;
            GvItems.DataSource = null;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }        

        //Limpiar controles
        drop_articulos.ClearSelection();
        STipoGasto.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importe_item.Text = string.Empty;
        drop_taxes.ClearSelection();
       // tbx_tax_amount.Text = string.Empty;       
        MultiView1.SetActiveView(View_General);

    }    

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //anticipo
        if (drop_anticipos.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B42").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);           
            return;
        }
        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        MultiView1.SetActiveView(View_Articulos);
    }       

    protected void btn_cancelar_item_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
        //Limpiar controles
        drop_articulos.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importe_item.Text = string.Empty;
        MultiView1.SetActiveView(View_General);
    }

    public void EnviarCorreo()
    {       
        var matrix = Doc_Tools.get_MatrizValidadores(pUserKey, 1);
        string server_address = ConfiguracionCorreoElectronico.server_address;
        int server_port = ConfiguracionCorreoElectronico.server_port;
        string user = ConfiguracionCorreoElectronico.user;
        string password = ConfiguracionCorreoElectronico.password;
        bool enable_ssl = ConfiguracionCorreoElectronico.enable_ssl;
        CorreoElectronico email = new CorreoElectronico(server_address, server_port, user, password, enable_ssl);

        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.Expense));
        var orden = jerarquia.Get_Orden(jerarquia);

        string from = Doc_Tools.getUserEmail(pUserKey);
        string to = string.Empty;
        foreach (var item in orden) if (to == string.Empty)
            {
                if (item.Value == "RecursosHumanos" && matrix.Rh != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Rh);
                }
                if (item.Value == "GerenciaArea" && matrix.Gerente != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Rh);
                }
                if (item.Value == "CuentasxPagar" && matrix.ValidadorCx != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.ValidadorCx);
                }
                if (item.Value == "Tesoreria" && matrix.Tesoreria != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Tesoreria);
                }
                if (item.Value == "Finanzas" && matrix.Finanzas != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Finanzas);
                }

            }

        string subject = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.Expense.ToString());
        string text = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.Expense.ToString());
        bool is_text_html = false;
        email.Enviar(from, to, subject, text, is_text_html);
    }

    protected void drop_anticipos_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        int advance_id = int.Parse(drop_anticipos.SelectedValue);
        if(advance_id!=0)
        {
            var advance = LoadAdvanceById(advance_id, pUserKey);
            tbx_currency.Text = Doc_Tools.Dict_moneda().FirstOrDefault(x=> x.Key == advance.Currency).Value;
        }
    }

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btnSage.Enabled = false;
        is_valid = false;
        
        //anticipo
        if (drop_anticipos.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B42").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {          
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //validacion de fecha (3 meses anteriores de limite)
        if (DateTime.Today.AddMonths(-3) > DateTime.Parse(tbx_fechagasto.Text))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB25").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }       

        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {         
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;           
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        
        HttpContext.Current.Session["motivo"] = tbx_motivo.Text;
        
        if (HttpContext.Current.Session["fu_xml"]!=null)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
        }
        //Si el archivo tiene XML
        if (fu_xml.HasFiles)
        {
            var xml_files = new List<ExpenseFilesDTO>();
            //Validación del Formato de los Archivos            
            if (fu_xml.PostedFiles.Any(x => x.ContentType != "text/xml"))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB29").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Validación del Tamaño
            if (fu_xml.PostedFiles.Any(x => x.ContentLength > 1000000 * 15))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB30").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
           
        }

        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            fu_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
        }
        //Si se subio archivo PDF
        if (fu_pdf.HasFiles)
        {
            var pdf_files = new List<ExpenseFilesDTO>();
            if (fu_pdf.PostedFiles.Any(x => x.ContentType.ToString() != "application/pdf"))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            if (fu_pdf.PostedFiles.Any(x => x.ContentLength > 1000000 * 15))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB27").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }            
        }

        if (HttpContext.Current.Session["fu_voucher"] != null)
        {
            fu_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
        }
        //Si se subio archivo PDF Voucher
        if (fu_voucher.HasFiles)
        {
            var voucher_files = new List<ExpenseFilesDTO>();
            if (fu_voucher.PostedFiles.Any(x => x.ContentType.ToString() != "application/pdf"))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            if (fu_voucher.PostedFiles.Any(x => x.ContentLength > 1000000 * 15))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB28").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }            
        }

        int advance_id = int.Parse(drop_anticipos.SelectedValue);
        var advance = LoadAdvanceById(advance_id, pUserKey);

        int tipo_moneda = advance.Currency;
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);        
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);
        int AdvanceId = int.Parse(drop_anticipos.SelectedValue);        

        fill_fileUploads();
        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];       

        if (lista_detalles == null || lista_detalles.Count == 0)
        {
           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB41").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }       

        if (!Check_Advance_Date(pUserKey, AdvanceId, fecha_gasto))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB43").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        if (Check_Exist(pUserKey, importe_gasto))
        {
            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB22").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Validaciones del Importe y Articulos - Impuestos
        if(importe_gasto != Decimal.Parse(lista_detalles.Sum(x=> x.Amount + x.TaxAmount).ToString("0.00")))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB30").Value;          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Solo alertas sin retorno
        is_valid = true;               
        HttpContext.Current.Session["is_valid"] = is_valid;
        string Msjj = string.Empty;

        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
        }

        if (!fu_xml.HasFile)
        {          
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB31").Value ;
        }

        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            fu_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
        }

        if (!fu_pdf.HasFile)
        {          
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB32").Value;
        }

        if (HttpContext.Current.Session["fu_voucher"] != null)
        {
            fu_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
        }
        
        if (!fu_voucher.HasFile)
        {
          
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB33").Value;
        }

        if (!Check_Advance_Amount(AdvanceId, importe_gasto, pUserKey))
        {
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB34").Value;
        }

        if (Msjj != "")
        {            
            tipo = "warning";            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msjj + "','" + tipo + "');", true);           
        }

        if (is_valid)
        {
            btnSage.Enabled = true;
        }
    }

    protected void View_General_Activate(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["motivo"]!=null)
        {
            tbx_motivo.Text = HttpContext.Current.Session["motivo"].ToString();
        }
        fill_fileUploads();
    }

    protected void View_General_Deactivate(object sender, EventArgs e)
    {
        HttpContext.Current.Session["motivo"] = tbx_motivo.Text;
        if (fu_xml.HasFiles)
        {
            var xml_files = new List<ExpenseFilesDTO>();
            foreach (HttpPostedFile xml_postedFile in fu_xml.PostedFiles)
            {               
                var xml_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Xml,
                    ExpenseType = Doc_Tools.DocumentType.Expense
                };
                byte[] byte_array = new byte[xml_postedFile.ContentLength];
                xml_postedFile.InputStream.Read(byte_array, 0, byte_array.Length);
                xml_file.FileName = xml_postedFile.FileName;
                xml_file.FileBinary = byte_array;
                xml_files.Add(xml_file);
            }
            HttpContext.Current.Session["xml_files"] = xml_files;
            HttpContext.Current.Session["fu_xml"] = fu_xml;           
        }
        if (fu_pdf.HasFiles)
        {
            var pdf_files = new List<ExpenseFilesDTO>();
            foreach (HttpPostedFile pdf_postedFile in fu_pdf.PostedFiles)
            {
                var pdf_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Pdf,
                    ExpenseType = Doc_Tools.DocumentType.Expense
                };
                byte[] byte_array = new byte[pdf_postedFile.ContentLength];
                pdf_postedFile.InputStream.Read(byte_array, 0, byte_array.Length);
                pdf_file.FileName = pdf_postedFile.FileName;
                pdf_file.FileBinary = byte_array;
                pdf_files.Add(pdf_file);
            }
            HttpContext.Current.Session["pdf_files"] = pdf_files;
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;           
        }
        if (fu_voucher.HasFiles)
        {
            var voucher_files = new List<ExpenseFilesDTO>();
            foreach (HttpPostedFile voucher_postedFile in fu_voucher.PostedFiles)
            {
                var voucher_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Voucher,
                    ExpenseType = Doc_Tools.DocumentType.Expense
                };
                byte[] byte_array = new byte[voucher_postedFile.ContentLength];
                voucher_postedFile.InputStream.Read(byte_array, 0, byte_array.Length);
                voucher_file.FileName = voucher_postedFile.FileName;
                voucher_file.FileBinary = byte_array;
                voucher_files.Add(voucher_file);
            }
            HttpContext.Current.Session["voucher_files"] = voucher_files;
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;            
        }        
    }

    protected void tbx_fechagasto_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }

    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        BindGridView();
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        HttpContext.Current.Session["Advance"] = null;

        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex; //int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Delete")
        {
            int advance_id = int.Parse(row.Cells[0].Text);
            Doc_Tools.DeleteExpense(Doc_Tools.DocumentType.Expense, advance_id, pUserKey);
            BindGridView();
        }
    }

}