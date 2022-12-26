//PORTAL DE PROVEDORES T|SYS|
//2 - AGOSTO, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA
//PANTALLA PARA TARJETAS CORPORATIVAS
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
using c_Impuesto = uCFDsLib.v33.c_Impuesto;
using Proveedores_Model;
using WebSite1;

public partial class Logged_Administradores_TarjetaEmpleado : System.Web.UI.Page
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

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        List<RolDTO> roles = Doc_Tools.get_Roles();

        try
        {
            if (!roles.Any(x => x.ID == HttpContext.Current.Session["RolUser"].ToString()))
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

                    if (!IsPostBack)
                    {
                        get_taxes();
                        get_items(pCompanyID);
                        HttpContext.Current.Session["GridList"] = null;
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

                    //fill_filelists();
                    fill_fileUploads();
                    BindGridView();

                    if(HttpContext.Current.Session["is_valid"] != null)
                    {
                        is_valid = (bool)HttpContext.Current.Session["is_valid"];
                    }
                    else 
                    {
                        HttpContext.Current.Session["is_valid"] = false;                        
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

    private void get_taxes()
    {
        List<TaxesDTO> lista =Doc_Tools.get_taxes(pCompanyID);       
        drop_taxes.DataSource = lista.ToList();
        drop_taxes.DataTextField = "STaxCodeID";
        drop_taxes.DataValueField = "STaxCodeKey";
        drop_taxes.DataBind();
        drop_taxes.SelectedIndex = -1;      
        HttpContext.Current.Session["Taxes"] = null;
        HttpContext.Current.Session["Taxes"] = lista;

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

    protected void Page_LoadComplete(object sender, EventArgs e)
    {

        //sesiones para el modo edicion     

    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
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
                ExpenseType = Doc_Tools.DocumentType.CorporateCard
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
                ExpenseType = Doc_Tools.DocumentType.CorporateCard
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
                ExpenseType = Doc_Tools.DocumentType.CorporateCard
            };
            byte[] byte_array = new byte[fu_voucher.PostedFile.ContentLength];
            fu_voucher.PostedFile.InputStream.Read(byte_array, 0, byte_array.Length);
            voucher_file.ContentType = fu_voucher.PostedFile.ContentType;
            voucher_file.FileName = fu_voucher.PostedFile.FileName;
            voucher_file.FileBinary = byte_array;
            HttpContext.Current.Session["voucher_file"] = voucher_file;
        }
    }

    private bool Check_Exist(int userKey, decimal importe)
    {
        decimal count = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*) FROM CorporateCard where UpdateUserKey = @UpdateUserKey and Amount = @Amount and UpdateDate = @UpdateDate";
            cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userKey;
            cmd.Parameters.Add("@UpdateDate", SqlDbType.Date).Value = DateTime.Today.Date;
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                count = lector.GetInt32(0);
            }
            if (count > 0)
            {
                return true;
            }
        }
        return false;
    }
    private void Sage()
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            //Escribir info en BD
            int tipo_moneda = int.Parse(drop_currency.SelectedValue);
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
            //int tipo_gasto = int.Parse(STipoGasto.SelectedValue);
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);
            string motivo_gasto = tbx_motivo.Text;
            int status = 1;
            var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            if (rol != "T|SYS| - Empleado")
            {
                List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
                if (roles.FirstOrDefault(x => x.ID == rol).Key == roles.Max(z => z.Key))
                {
                    status = 2;
                }
            }
            int carga = WriteToDb(tipo_moneda, fecha_gasto, importe_gasto, pUserKey, pCompanyID, lista_detalles,status, motivo_gasto);

            if (carga == -1)
            {
                gvGastos.DataSource = null;              
                gvGastos.Visible = false;
                tbx_importe.Text = Math.Round(lista_detalles.Sum(x => x.Amount + x.TaxAmount), 2).ToString("0.00");
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B4").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);                
                ClearControls();
                MultiView1.SetActiveView(View_General);
                return;
            }

            EnviarCorreo();
            BindGridView();
            ClearControls();
            HttpContext.Current.Session["GridList"] = null;
            HttpContext.Current.Session["GridTaxes"] = null;
            HttpContext.Current.Session["is_valid"] = false;
            btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
            GvItems.DataSource = null;
            GvItems.DataBind();
        }
        
    }

    private void BindGridView()
    {
        gvGastos.DataSource = null;
        //gvValidacion.DataSource = null;
        //gvValidacion.Visible = false;
        gvGastos.Visible = true;
        gvGastos.DataSource = ReadFromDb(pUserKey);
        gvGastos.DataBind();
    }    

    private void ClearControls()
    {
        tbx_fechagasto.Text = string.Empty;
        tbx_importe.Text = string.Empty;      
        STipoGasto.ClearSelection();
        drop_currency.ClearSelection();
        HttpContext.Current.Session["fu_xml"] = null;
        HttpContext.Current.Session["fu_pdf"] = null;
        HttpContext.Current.Session["fu_voucher"] = null;
        HttpContext.Current.Session["xml_files"] = null;
        HttpContext.Current.Session["pdf_files"] = null;
        HttpContext.Current.Session["voucher_files"] = null;
        HttpContext.Current.Session["motivo"] = null;
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        //if (fu_xml!=null)
        //{
        //    fu_xml.Attributes.Clear();
        //}
        //if(fu_pdf!=null)
        //{
        //    fu_pdf.Attributes.Clear();
        //}
        //if(fu_voucher!=null)
        //{
        //    fu_voucher.Attributes.Clear();
        //}          
        
        btnSage.Enabled = false;
        tbx_motivo.Text = string.Empty;
        HttpContext.Current.Session["is_valid"] = false;
    }

    /// <summary>
    /// Metodo para Guardar el Gasto en BD
    /// </summary>
    /// <returns>Entero</returns>
    private int WriteToDb(int tipo_moneda, DateTime fecha_gasto, decimal importe_gasto, int userkey, string companyId, List<ExpenseDetailDTO> expenseDetails,int status, string motivo_gasto)
    {
        string val = string.Empty, d=string.Empty;      
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
                cmd.CommandText = "Insert Into CorporateCard (Date, Currency, Amount, CreateDate, UpdateDate, UpdateUserKey, CompanyId, Status, ApprovalLevel, ExpenseReason) VALUES ( @Date, @Currency, @Amount, @CreateDate, @UpdateDate, @UpdateUserKey, @CompanyId, @Status, @ApprovalLevel, @ExpenseReason); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = fecha_gasto;
                cmd.Parameters.Add("@Currency", SqlDbType.Int).Value = tipo_moneda;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe_gasto;
              
                cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;
                cmd.Parameters.Add("@CompanyId", SqlDbType.NVarChar).Value = companyId;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status; //Estado inicial Pendiente
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
                    cmd.CommandText = "INSERT INTO CorporateCardDetail (CorporateCardId,Type,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId,STaxCodeKey,TaxAmount) VALUES (@_CorporateCardId,@_Type, @_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser,@_CompanyId,@_STaxCodeKey,@_TaxAmount); SELECT SCOPE_IDENTITY();";
                    cmd.Parameters.Add("@_CorporateCardId", SqlDbType.Int).Value = id;
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
                    d = inserted.ToString();
                    detail_id = Convert.ToInt32(d);
                    cmd.Parameters.Clear();
                    if(detail.FileXml!=null)
                    {
                        detail.FileXml.ExpenseId = id;
                        detail.FileXml.ExpenseDetailId = detail_id;
                        Doc_Tools.SaveFile(detail.FileXml);
                    }
                    if(detail.FilePdf!=null)
                    {
                        detail.FilePdf.ExpenseId = id;
                        detail.FilePdf.ExpenseDetailId = detail_id;
                        Doc_Tools.SaveFile(detail.FilePdf);
                    }
                    if(detail.FilePdfVoucher!=null)
                    {
                        detail.FilePdfVoucher.ExpenseId = id;
                        detail.FilePdfVoucher.ExpenseDetailId = detail_id;
                        Doc_Tools.SaveFile(detail.FilePdfVoucher);
                    }                 
                    
                }               
                cmd.Connection.Close();
               
            }

            return id;
        }
        catch (Exception ex)
        {
            if (id > 0)
            {
                Doc_Tools.DeleteExpenseOnFail(Doc_Tools.DocumentType.CorporateCard, id);
                Doc_Tools.DeleteDetailOnFail(Doc_Tools.DocumentType.CorporateCard, id);
                Doc_Tools.DeleteFile(Doc_Tools.DocumentType.CorporateCard, id);
            }
            LogError(pLogKey, pUserKey, "Insertar-Pago-Tarjeta-Empleados:Insertar-Pago", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }
          
    private List<CorporateCardDTO> ReadFromDb(int user_id)
    {
        List<CorporateCardDTO> gastos = new List<CorporateCardDTO>();

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status FROM CorporateCard where UpdateUserKey = @UpdateUserKey;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var tarjeta = new CorporateCardDTO();
                tarjeta.CorporateCardId = dataReader.GetInt32(0);              
                tarjeta.Date = dataReader.GetDateTime(1);
                tarjeta.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                tarjeta.Amount = dataReader.GetDecimal(3);
                tarjeta.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;               
                gastos.Add(tarjeta);
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
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
        Response.Redirect("~/Logged/Reports/TarjetaEmpleado");
    }    

    protected void STipoGasto_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
        fill_filelists();
        if (STipoGasto.SelectedValue != "")
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
        if(tbx_importe.Text.Any(d => !char.IsDigit(d) && (d != '.') && (d != ',')))
        {
            tbx_importe.Text = string.Empty;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
            System.Threading.Thread.Sleep(5000);
            return;
        }       
    }
   
    private CorporateCardDTO LoadCardById(int expense_id, int user_id)
    {
        var card = new CorporateCardDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status, ExpenseReason FROM CorporateCard where UpdateUserKey = @UpdateUserKey and CorporateCardId = @CorporateCardId and Status = 1;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@CorporateCardId", SqlDbType.Int).Value = expense_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                card.CorporateCardId = dataReader.GetInt32(0);              
                card.Date = dataReader.GetDateTime(1);
                card.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                card.Amount = dataReader.GetDecimal(3);
                card.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;               
                card.ExpenseReason = dataReader.GetString(5);
            }
        }
        return card;
    }

    protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        HttpContext.Current.Session["CorporateCard"] = null;
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int card_id = int.Parse(row.Cells[0].Text);
            var card = LoadCardById(card_id, pUserKey);            
            HttpContext.Current.Session["CorporateCard"] = card;
            Response.Redirect("EditTarjeta");
        }   

    }

    protected void gvGastos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int card_id =  int.Parse(e.Row.Cells[0].Text);
            if (e.Row.Cells[4].Text != "Pendiente")
            {
                Button btnEdit = (Button)e.Row.Cells[5].Controls[0];
                Button btnDelete = (Button)e.Row.Cells[7].Controls[1];
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }           

        }
    }
   
    protected void GvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        is_valid = false;       
        HttpContext.Current.Session["is_valid"] = is_valid;

        //Borrar articulo del grid   
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = GvItems.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int itemKey = int.Parse(row.Cells[0].Text);
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            if (lista.Count == 1)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB50").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            var detail = lista.FirstOrDefault(x => x.ItemKey == itemKey);
            lista.Remove(detail);
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
       
    }

    protected void GvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int idx = e.Row.RowIndex;
            List <ExpenseDetailDTO> items = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            ExpenseDetailDTO item = items[idx];
            var img_xml = (System.Web.UI.WebControls.Image)e.Row.Cells[8].Controls[1];
            var img_pdf = (System.Web.UI.WebControls.Image)e.Row.Cells[9].Controls[1];
            var img_voucher = (System.Web.UI.WebControls.Image)e.Row.Cells[10].Controls[1];
                        
            img_xml.ImageUrl = item.FileXml != null ? "/Img/Ok.png" : "/Img/X.png";
            img_pdf.ImageUrl = item.FilePdf != null ? "/Img/Ok.png" : "/Img/X.png";
            img_voucher.ImageUrl = item.FilePdfVoucher != null ? "/Img/Ok.png" : "/Img/X.png";   
        }
    }   

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        is_valid = false;
        HttpContext.Current.Session["is_valid"] = is_valid;
        STipoGasto.ClearSelection();
        if(drop_currency.SelectedValue =="")
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
        tbx_xml.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
        MultiView1.SetActiveView(View_Articulos);
    }

    protected void btn_additem_Click(object sender, EventArgs e)
    {
        bool xml = false, voucher=false, pdf=false;
        fill_filelists();
        //validacion de tipo Gasto
        if (string.IsNullOrEmpty(STipoGasto.SelectedValue.ToString()))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB26").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //validaciones
        if (drop_articulos.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B40").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        //validacion texto en importe
        if (tbx_importegasto.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        //validacion del importe del articulo
        if (tbx_importegasto.Text == string.Empty || decimal.Parse(tbx_importegasto.Text) <= 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B41").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        else 
        {         
            if (tbx_importegasto.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }           
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
        if (drop_taxes.SelectedValue == "0")
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
        detalle.UnitCost = decimal.Parse(tbx_importegasto.Text);
        if (drop_taxes.SelectedValue != "0")
        {
            detalle.STaxCodeKey = int.Parse(drop_taxes.SelectedItem.Value);
            detalle.STaxCodeID = drop_taxes.SelectedItem.Text;
            detalle.TaxAmount = taxes.FirstOrDefault(x => x.STaxCodeKey == detalle.STaxCodeKey).Rate * detalle.Amount;
        }
        if(!xml)
        {
            var xml_file = (ExpenseFilesDTO)HttpContext.Current.Session["xml_file"];
            detalle.FileXml = xml_file;
        }
        if(!pdf)
        {
            var pdf_file = (ExpenseFilesDTO)HttpContext.Current.Session["pdf_file"];
            detalle.FilePdf = pdf_file;
        }
        if(!voucher)
        {
            var voucher_file = (ExpenseFilesDTO)HttpContext.Current.Session["voucher_file"];
            detalle.FilePdfVoucher = voucher_file;
        }

        if (HttpContext.Current.Session["GridList"] != null)
        {
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            lista.Add(detalle);
            HttpContext.Current.Session["GridList"] = lista;
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
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = null;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }

        //Limpiar controles
        drop_articulos.ClearSelection();
        drop_taxes.ClearSelection();
        tbx_importegasto.Text = string.Empty;
        tbx_cantidad.Text = string.Empty;
        drop_taxes.ClearSelection();
        tbx_xml.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
        HttpContext.Current.Session["voucher_file"] = null;
        HttpContext.Current.Session["pdf_file"] = null;
        HttpContext.Current.Session["xml_file"] = null;
        MultiView1.SetActiveView(View_General);       

    }       
     
    protected void btn_cancelar_item_Click(object sender, EventArgs e)
    {
        is_valid = false;
        HttpContext.Current.Session["is_valid"] = is_valid;
        //Limpiar controles
        drop_articulos.ClearSelection();
        STipoGasto.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        tbx_voucher.Text = string.Empty;
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

        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.CorporateCard));
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

        string subject = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.CorporateCard.ToString());
        string text = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.CorporateCard.ToString());
        bool is_text_html = false;
        email.Enviar(from, to, subject, text, is_text_html);
    }

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btnSage.Enabled = false;
        is_valid = false;       

        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;          
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Solo se admiten gastos del mes en curso
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        if (DateTime.Today.Year == fecha_gasto.Year)
        {
            if (DateTime.Today.Month != fecha_gasto.Month)
            {                
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB38").Value;              
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }
        else
        {         
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB38").Value;          
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Validacion de moneda
        if (string.IsNullOrEmpty(drop_currency.SelectedValue.ToString()))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;         
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

       
        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;          
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Escribir info en BD
        int tipo_moneda = int.Parse(drop_currency.SelectedValue);
        fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        int tipo_gasto = int.Parse(STipoGasto.SelectedValue);
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);   

        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];     

        if (lista_detalles == null || lista_detalles.Count == 0)
        {         
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB35").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
      
        if (Check_Exist(pUserKey, importe_gasto))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB39").Value;          
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }      

        //Solo alertas sin retorno
        is_valid = true;                  
        HttpContext.Current.Session["is_valid"] = is_valid;
      
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
        //fill_fileUploads();
    }

    protected void View_General_Deactivate(object sender, EventArgs e)
    {
        //fill_filelists();
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
            Doc_Tools.DeleteExpense(Doc_Tools.DocumentType.CorporateCard,advance_id, pUserKey);
            BindGridView();
        }
    }

    protected void btnVisualize_Command(object sender, CommandEventArgs e)
    {
        BindGridView();
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];    

        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex;
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Visualize")
        {
            int expense_id = int.Parse(row.Cells[0].Text);
            HttpContext.Current.Session["expense_id_visualize"] = expense_id;
            HttpContext.Current.Session["expense_type_visualize"] = Doc_Tools.DocumentType.CorporateCard;
            HttpContext.Current.Session["screen_type"] = 0;
            ClearControls();
            Response.Redirect("DocumentosGastos");
        }
    }

}