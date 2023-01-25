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
        if (HttpContext.Current.Session["xml_file"] != null)
        {
            var xml_file = (ExpenseFilesDTO)HttpContext.Current.Session["xml_file"];
            tbx_xml.Text = xml_file.FileName;
        }

        if (HttpContext.Current.Session["pdf_file"] != null)
        {
            var pdf_file = (ExpenseFilesDTO)HttpContext.Current.Session["pdf_file"];
            tbx_pdf.Text = pdf_file.FileName;
        }

        if (HttpContext.Current.Session["voucher_file"] != null)
        {
            var voucher_file = (ExpenseFilesDTO)HttpContext.Current.Session["voucher_file"];
            tbx_voucher.Text = voucher_file.FileName;
        }

    }
    public void fill_filelists()
    {
        if (fu_xml.HasFile)
        {
            var xml_file = new ExpenseFilesDTO
            {
                Type = ExpenseFilesDTO.FileType.Xml,
                ExpenseType = Doc_Tools.DocumentType.Expense
            };
            byte[] byte_array = new byte[fu_xml.PostedFile.ContentLength];
            fu_xml.PostedFile.InputStream.Read(byte_array, 0, byte_array.Length);
            xml_file.ContentType = fu_xml.PostedFile.ContentType;
            xml_file.FileName = fu_xml.PostedFile.FileName;
            xml_file.FileBinary = byte_array;
            HttpContext.Current.Session["xml_file"] = xml_file;
        }
        if (fu_pdf.HasFile)
        {
            var pdf_file = new ExpenseFilesDTO
            {
                Type = ExpenseFilesDTO.FileType.Pdf,
                ExpenseType = Doc_Tools.DocumentType.Expense
            };
            byte[] byte_array = new byte[fu_pdf.PostedFile.ContentLength];
            fu_pdf.PostedFile.InputStream.Read(byte_array, 0, byte_array.Length);
            pdf_file.ContentType = fu_pdf.PostedFile.ContentType;
            pdf_file.FileName = fu_pdf.PostedFile.FileName;
            pdf_file.FileBinary = byte_array;
            HttpContext.Current.Session["pdf_file"] = pdf_file;
        }
        if (fu_voucher.HasFile)
        {
            var voucher_file = new ExpenseFilesDTO
            {
                Type = ExpenseFilesDTO.FileType.Voucher,
                ExpenseType = Doc_Tools.DocumentType.Expense
            };
            byte[] byte_array = new byte[fu_voucher.PostedFile.ContentLength];
            fu_voucher.PostedFile.InputStream.Read(byte_array, 0, byte_array.Length);
            voucher_file.ContentType = fu_voucher.PostedFile.ContentType;
            voucher_file.FileName = fu_voucher.PostedFile.FileName;
            voucher_file.FileBinary = byte_array;
            HttpContext.Current.Session["voucher_file"] = voucher_file;
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
                    //Si no es carga inicial
                    if (!IsPostBack)
                    {
                        get_anticipos(pUserKey);
                        get_items(pCompanyID);
                        get_taxes();
                        BindTipoGasto();
                        HttpContext.Current.Session["GridItems"] = null;
                        HttpContext.Current.Session["GridTaxes"] = null;
                        //Limpiar Variables de sesion del Gastos
                        HttpContext.Current.Session["fu_xml"] = null;
                        HttpContext.Current.Session["fu_pdf"] = null;
                        HttpContext.Current.Session["fu_voucher"] = null;
                        HttpContext.Current.Session["xml_files"] = null;
                        HttpContext.Current.Session["pdf_files"] = null;
                        HttpContext.Current.Session["voucher_files"] = null;
                        HttpContext.Current.Session["motivo"] = null;
                        HttpContext.Current.Session["is_valid"] = null;
                    }
                  
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
        int advance_id = 0;
        if (is_valid)
        {     
            if(HttpContext.Current.Session["motivo"]!=null &&  string.IsNullOrEmpty(tbx_motivo.Text))
            {
                tbx_motivo.Text = HttpContext.Current.Session["motivo"].ToString();
            }          
            if(drop_anticipos.SelectedValue!="0")
            {
                advance_id = int.Parse(drop_anticipos.SelectedValue);
                var advance = LoadAdvanceById(advance_id, pUserKey);
            }          

            int tipo_moneda = int.Parse(drop_currency.SelectedValue);
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);            
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);           
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

            int carga = WriteToDb(tipo_moneda, fecha_gasto, importe_gasto, pUserKey, pCompanyID, advance_id, lista_detalles, status, motivo_gasto);

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
            if(advance_id!=0)
            {
                Vencer_Advance(advance_id, pUserKey);
            }           

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

    private void BindTipoGasto()
    {
        var lista = Doc_Tools.Dict_tipos_gastos();
        lista.Add(0, string.Empty);
        STipoGasto.DataSource = lista.OrderBy(x=> x.Key).Select(x=> new {Id=x.Key, Name=x.Value}).ToList();
        STipoGasto.DataTextField = "Name";
        STipoGasto.DataValueField = "Id";
        STipoGasto.DataBind();
    }

    private void ClearControls()
    {
        tbx_fechagasto.Text = string.Empty;
        tbx_importe.Text = string.Empty;       
        STipoGasto.ClearSelection();
        drop_currency.ClearSelection();
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
        btnSage.Enabled = false;
        tbx_motivo.Text = string.Empty;
    }

    
    private int WriteToDb(int tipo_moneda, DateTime fecha_gasto, decimal importe_gasto, int userkey, string companyId, int AdvanceId, List<ExpenseDetailDTO> expenseDetails, int status, string motivo_gasto)
    {
        string val = string.Empty, d = string.Empty;
        int id = 0, detail_id = 0;
        int? approval_level = null;                 
        try
        {             
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
                if (approval_level == null)
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
                    cmd.CommandText = "INSERT INTO ExpenseDetail (ExpenseId,Type,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId,STaxCodeKey,TaxAmount) VALUES (@_ExpenseId,@_Type,@_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser,@_CompanyId, @_STaxCodeKey, @_TaxAmount); SELECT SCOPE_IDENTITY();";
                    cmd.Parameters.Add("@_ExpenseId", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@_Type", SqlDbType.Int).Value = detail.Type;
                    cmd.Parameters.Add("@_ItemKey", SqlDbType.Int).Value = detail.ItemKey;
                    cmd.Parameters.Add("@_Qty", SqlDbType.Decimal).Value = detail.Qty;
                    cmd.Parameters.Add("@_UnitCost", SqlDbType.Decimal).Value = detail.UnitCost;
                    cmd.Parameters.Add("@_Amount", SqlDbType.Decimal).Value = detail.Amount;
                    cmd.Parameters.Add("@_CreateDate", SqlDbType.DateTime).Value = detail.CreateDate;
                    cmd.Parameters.Add("@_UpdateDate", SqlDbType.DateTime).Value = detail.CreateDate;
                    cmd.Parameters.Add("@_CreateUser", SqlDbType.Int).Value = userkey;
                    cmd.Parameters.Add("@_CompanyId", SqlDbType.VarChar).Value = companyId;
                    cmd.Parameters.Add("@_STaxCodeKey", SqlDbType.Decimal).Value = detail.STaxCodeKey;
                    cmd.Parameters.Add("@_TaxAmount", SqlDbType.Decimal).Value = detail.TaxAmount;

                    var inserted = cmd.ExecuteScalar();
                    d = inserted.ToString();
                    detail_id = Convert.ToInt32(d);
                    cmd.Parameters.Clear();
                    if (detail.FileXml != null)
                    {
                        detail.FileXml.ExpenseId = id;
                        detail.FileXml.ExpenseDetailId = detail_id;
                        detail.FileXml.DateCreated = detail.CreateDate;
                        Doc_Tools.SaveFile(detail.FileXml);
                    }
                    if (detail.FilePdf != null)
                    {
                        detail.FilePdf.ExpenseId = id;
                        detail.FilePdf.ExpenseDetailId = detail_id;
                        detail.FilePdf.DateCreated = detail.CreateDate;
                        Doc_Tools.SaveFile(detail.FilePdf);
                    }
                    if (detail.FilePdfVoucher != null)
                    {
                        detail.FilePdfVoucher.ExpenseId = id;
                        detail.FilePdfVoucher.ExpenseDetailId = detail_id;
                        detail.FilePdfVoucher.DateCreated = detail.CreateDate;
                        Doc_Tools.SaveFile(detail.FilePdfVoucher);
                    }
                }
                cmd.Connection.Close();                        
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
                expense.Currency = dataReader.GetInt32(2);
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
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        //logica para enviar los correos
        //Logica para lanzar Reporte de Reembolsos
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
        Response.Redirect("~/Logged/Reports/Reembolsos");
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
                expense.Currency = dataReader.GetInt32(2);
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
                advance.AdvanceType = Doc_Tools.Dict_Advancetype().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
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
           
            HttpContext.Current.Session["Expense"] = expense;
            ClearControls();
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
                Button btnDelete = (Button)e.Row.Cells[7].Controls[1];
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
            cmd.CommandText = "SELECT AdvanceId , Folio FROM Advance where UpdateUserKey = @UpdateUserKey and Status = 2 ";
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
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int idx = e.Row.RowIndex;
            List<ExpenseDetailDTO> items = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            ExpenseDetailDTO item = items[idx];
            var img_xml = (System.Web.UI.WebControls.Image)e.Row.Cells[8].Controls[1];
            var img_pdf = (System.Web.UI.WebControls.Image)e.Row.Cells[9].Controls[1];
            var img_voucher = (System.Web.UI.WebControls.Image)e.Row.Cells[10].Controls[1];

            img_xml.ImageUrl = item.FileXml != null ? "/Img/Ok.png" : "/Img/X.png";
            img_pdf.ImageUrl = item.FilePdf != null ? "/Img/Ok.png" : "/Img/X.png";
            img_voucher.ImageUrl = item.FilePdfVoucher != null ? "/Img/Ok.png" : "/Img/X.png";
        }
    }   

    protected void btn_additem_Click(object sender, EventArgs e)
    {       
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        bool xml = false, voucher = false, pdf = false;
        fill_filelists();       

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
        //validacion de fecha de articulo
        if (string.IsNullOrEmpty(tbx_fecha_articulo.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //validacion de fecha de articulo
        if (string.IsNullOrEmpty(tbx_fecha_articulo.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        //validacion de fecha articulo vs fecha del gasto
        if (DateTime.Parse(tbx_fecha_articulo.Text) > DateTime.Parse(tbx_fechagasto.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B55").Value;
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
        //validacion de impuestos 
        if(drop_taxes.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B54").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //Si el archivo tiene XMLs    
        if (HttpContext.Current.Session["xml_file"] != null)
        {
            var xml_files = (ExpenseFilesDTO)HttpContext.Current.Session["xml_file"];
            //Validacion de tipo fichero
            if (xml_files.ContentType != "text/xml")
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB29").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
            //Validación del Tamaño
            if (xml_files.FileLength > 1000000 * 15)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB30").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
        }
        else { xml = true; }

        //Si el archivo tiene PDFs    
        if (HttpContext.Current.Session["pdf_file"] != null)
        {
            var pdf_file = (ExpenseFilesDTO)HttpContext.Current.Session["pdf_file"];
            if (pdf_file.ContentType != "application/pdf")
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }

            if (pdf_file.FileLength > 1000000 * 15)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB27").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
        }
        else { pdf = true; }

        //Si se subio archivo PDF Voucher
        if (HttpContext.Current.Session["voucher_file"] != null)
        {
            var voucher_file = (ExpenseFilesDTO)HttpContext.Current.Session["voucher_file"];
            if (voucher_file.ContentType != "application/pdf")
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }

            if (voucher_file.FileLength > 1000000 * 15)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB28").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
        }
        else { voucher = true; }
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
        DateTime fecha_articulo = DateTime.Parse(tbx_fecha_articulo.Text);
        detalle.CreateDate = fecha_articulo;
        
        if (drop_taxes.SelectedValue != "0")
        {
            detalle.STaxCodeKey = int.Parse(drop_taxes.SelectedItem.Value);
            detalle.STaxCodeID = drop_taxes.SelectedItem.Text;
            detalle.TaxAmount = taxes.FirstOrDefault(x => x.STaxCodeKey == detalle.STaxCodeKey).Rate * detalle.Amount;           
        }
        if (!xml)
        {
            var xml_file = (ExpenseFilesDTO)HttpContext.Current.Session["xml_file"];
            detalle.FileXml = xml_file;
        }
        if (!pdf)
        {
            var pdf_file = (ExpenseFilesDTO)HttpContext.Current.Session["pdf_file"];
            detalle.FilePdf = pdf_file;
        }
        if (!voucher)
        {
            var voucher_file = (ExpenseFilesDTO)HttpContext.Current.Session["voucher_file"];
            detalle.FilePdfVoucher = voucher_file;
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
        drop_taxes.ClearSelection();
        STipoGasto.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importe_item.Text = string.Empty;
        tbx_fecha_articulo.Text = string.Empty;
        drop_taxes.ClearSelection();
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
        // tbx_tax_amount.Text = string.Empty;       
        MultiView1.SetActiveView(View_General);

    }    

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //moneda
        if (drop_currency.SelectedValue == "")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;
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
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        tbx_fecha_articulo.Text = string.Empty;
        STipoGasto.ClearSelection();      
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
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
        tbx_fecha_articulo.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
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

        //int advance_id = int.Parse(drop_anticipos.SelectedValue);
        //if(advance_id!=0)
        //{
        //    var advance = LoadAdvanceById(advance_id, pUserKey);
        //    tbx_currency.Text = Doc_Tools.Dict_moneda().FirstOrDefault(x=> x.Key == advance.Currency).Value;
        //}
    }

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btnSage.Enabled = false;
        is_valid = false;

        int advance_id = 0;
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
        //validacion de moneda
        if (drop_currency.SelectedValue == "")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;
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

        if(drop_anticipos.SelectedValue != "0")
        {
            advance_id = int.Parse(drop_anticipos.SelectedValue);
            var advance = LoadAdvanceById(advance_id, pUserKey);
        }

        int tipo_moneda = int.Parse(drop_currency.SelectedValue);
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);        
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);             

        fill_fileUploads();
        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];       

        if (lista_detalles == null || lista_detalles.Count == 0)
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB41").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }       
        if(advance_id!=0)
        {
            if (!Check_Advance_Date(pUserKey, advance_id, fecha_gasto))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB43").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }
        
        if (Check_Exist(pUserKey, importe_gasto))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B57").Value;         
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Validaciones del Importe y Articulos - Impuestos
        if (importe_gasto != Decimal.Parse(lista_detalles.Sum(x => x.Amount + x.TaxAmount).ToString("0.00")))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB30").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Solo alertas sin retorno
        is_valid = true;               
        HttpContext.Current.Session["is_valid"] = is_valid;          

        if(advance_id!=0)
        {
            if (!Check_Advance_Amount(advance_id, importe_gasto, pUserKey))
            {
                Msj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB34").Value;
            }
        }
       
        ////anticipo
        if (drop_anticipos.SelectedValue == "0")
        {           
            Msj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B42").Value;           
        }

        if (!string.IsNullOrEmpty(Msj))
        {            
            tipo = "warning";            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);           
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
        //fill_fileUploads();
    }

    protected void View_General_Deactivate(object sender, EventArgs e)
    {
        HttpContext.Current.Session["motivo"] = tbx_motivo.Text;
        //fill_filelists();
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

        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex; 
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Delete")
        {
            int advance_id = int.Parse(row.Cells[0].Text);
            Doc_Tools.DeleteExpense(Doc_Tools.DocumentType.Expense, advance_id, pUserKey);
            BindGridView();
        }
    }

    protected void drop_currency_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }
   
    protected void btnVisualize_Command(object sender, CommandEventArgs e)
    {
        BindGridView();
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        HttpContext.Current.Session["Advance"] = null;

        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex;
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Visualize")
        {
            int expense_id = int.Parse(row.Cells[0].Text);
            HttpContext.Current.Session["expense_id_visualize"] = expense_id;
            HttpContext.Current.Session["expense_type_visualize"] = Doc_Tools.DocumentType.Expense;
            HttpContext.Current.Session["screen_type"] = 0;
            ClearControls();
            Response.Redirect("DocumentosGastos");
        }
    }

    protected void tbx_fecha_articulo_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
        MultiView1.SetActiveView(View_Articulos);
    }
}