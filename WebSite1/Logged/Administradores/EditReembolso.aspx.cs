using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using WebSite1;

public partial class Logged_Administradores_EditReembolso : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private bool is_valid;
    private PortalProveedoresEntities _context = new PortalProveedoresEntities();
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
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    fill_fileUploads();
                    pVendKey = 0;
                    MultiView1.SetActiveView(View_General);

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;                      
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, "TSM");
                }
            }
            
            if (!IsPostBack)
            {
                get_items(pCompanyID);
                get_taxes();
                HttpContext.Current.Session["GridItems"] = null;
                HttpContext.Current.Session["GridTaxes"] = null;

                if (HttpContext.Current.Session["Expense"] != null)
                {
                    var expense = (ExpenseDTO)HttpContext.Current.Session["Expense"];
                    tbx_importe.Text = expense.Amount.ToString("0.00");
                    tbx_currency.Text = expense.Currency;
                    tbx_fechagasto.Text = expense.Date.ToString("yyyy-MM-dd");                 
                    Load_Articles_By_Expense(expense.ExpenseId, pUserKey, pCompanyID);
                    if(expense.FileNamePdf!=null)
                    {
                        expense.FileNamePdf.ForEach(x => tbx_pdf.Text += x.ToString());
                    }
                    if (expense.FileNameXml!=null)
                    {
                        expense.FileNameXml.ForEach(x => tbx_xml.Text += x.ToString());
                    }
                    if(expense.FileNamePdfVoucher!=null)
                    {
                        expense.FileNamePdfVoucher.ForEach(x => tbx_voucher.Text += x.ToString());
                    }                  
                   
                    tbx_motivo.Text = expense.ExpenseReason;
                }
            }

            if (HttpContext.Current.Session["Expense"] != null)
            {
                var expense = (ExpenseDTO)HttpContext.Current.Session["Expense"];
                if (HttpContext.Current.Session["fecha_gasto"] != null)
                {
                    DateTime fechagasto = new DateTime();
                    fechagasto = (DateTime)HttpContext.Current.Session["fecha_gasto"];
                    tbx_fechagasto.Text = fechagasto.ToString("yyyy-MM-dd");
                }
                
                //tbx_importe.Text = expense.Amount.ToString();
                //tbx_currency.Text = expense.Currency;
                //tbx_fechagasto.Text = expense.Date.ToString("yyyy-MM-dd");
                //STipoGasto.SelectedValue = Dict_tipos_gastos().FirstOrDefault(x => x.Value == expense.Type).Key.ToString();
                //tbx_policy.Text = Dict_policy().FirstOrDefault(x => x.Key == STipoGasto.SelectedIndex).Value;
                //
                //tbx_pdf.Text = expense.FileNamePdf;
                //tbx_xml.Text = expense.FileNameXml;
                //tbx_voucher.Text = expense.FileNamePdfVoucher;
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
   
    private void Load_Articles_By_Expense(int expense_id, int user_id, string company_id)
    {
        var lista = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        List<ExpenseDetailDTO> articles = new List<ExpenseDetailDTO>();
        if (HttpContext.Current.Session["GridList"] != null)
        {
            articles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
        }
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DetailId,ExpenseId,ItemKey,Qty,UnitCost, STaxCodeKey,TaxAmount, Type FROM ExpenseDetail where ExpenseId = @ExpenseId and CreateUser = @CreateUser and CompanyId = @CompanyId;";
            cmd.Parameters.Add("@CreateUser", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
            cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var article = new ExpenseDetailDTO();
                article.DetailId = dataReader.GetInt32(0);                
                article.ExpenseId = dataReader.GetInt32(1);
                article.ItemKey = dataReader.GetInt32(2);
                article.Qty = dataReader.GetDecimal(3);
                article.UnitCost = dataReader.GetDecimal(4);
                article.ItemId = lista.FirstOrDefault(x => x.ItemKey == article.ItemKey).ItemId;
                article.STaxCodeKey = dataReader.GetInt32(5);
                article.TaxAmount = dataReader.GetDecimal(6);
                article.STaxCodeID = taxes.FirstOrDefault(x => x.STaxCodeKey == article.STaxCodeKey).STaxCodeID;
                article.Type = dataReader.GetInt32(7);
                article.TipoGasto = Doc_Tools.Dict_tipos_gastos().FirstOrDefault(x => x.Key == article.Type).Value;
                
                articles.Add(article);
               
            }
        }
        GvItems.DataSource = null;
        GvItems.DataSource = articles;
        HttpContext.Current.Session["GridItems"] = articles;
        GvItems.DataBind();
    }
    private void get_taxes()
    {
        List<TaxesDTO> lista = Doc_Tools.get_taxes(pCompanyID);
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
    
    protected void STipoGasto_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        if (STipoGasto.SelectedValue != "")
        {
            tipo = "info";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == STipoGasto.SelectedValue).Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;

        }
    }

    private int Update_Reembolso(int expense_id, DateTime fecha_gasto, decimal importe_gasto, int userkey, string companyId, List<ExpenseDetailDTO> expenseDetails, string motivo_gasto)
    {
        var xml_files = new List<ExpenseFilesDTO>();
        var pdf_files = new List<ExpenseFilesDTO>();
        var voucher_files = new List<ExpenseFilesDTO>();
        int id = 0;
        try
        {
            if(HttpContext.Current.Session["xml_files"]!=null)
            {
                xml_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["xml_files"];
            }     

            if (HttpContext.Current.Session["pdf_files"]!=null)
            {
                pdf_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["pdf_files"];
            }           

            if (HttpContext.Current.Session["voucher_files"] != null)
            {               
               voucher_files = (List<ExpenseFilesDTO>)HttpContext.Current.Session["voucher_files"];              
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                //Cadena Inicial
                sb.Append("UPDATE dbo.Expense  SET Date = @Date,Amount = @Amount");                    
                sb.Append(", UpdateDate = @UpdateDate, ExpenseReason = @ExpenseReason WHERE ExpenseId = @ExpenseId;");
                cmd.CommandText = sb.ToString();              
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = fecha_gasto;               
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe_gasto;                
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;                    
                cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id; //Estado inicial Aprobado
                cmd.Parameters.Add("@ExpenseReason", SqlDbType.VarChar).Value = motivo_gasto ?? (object)DBNull.Value;
                cmd.Connection.Open();
                var modified = cmd.ExecuteScalar();

                foreach (ExpenseDetailDTO detail in expenseDetails)
                {
                    if(detail.DetailId == null)
                    {                        
                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO ExpenseDetail (ExpenseId,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId, STaxCodeKey,TaxAmount ,Type) VALUES (@_ExpenseId,@_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser, @_CompanyId, @_STaxCodeKey, @_TaxAmount, @_Type);";
                        cmd.Parameters.Add("@_ExpenseId", SqlDbType.Int).Value = expense_id;
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
                }                
                cmd.Connection.Close();
                foreach (ExpenseFilesDTO file in xml_files.Union(pdf_files).Union(voucher_files))
                {
                    file.ExpenseId = expense_id;
                    if(!Doc_Tools.CheckFileExist(file))
                    {
                        int id_file = Doc_Tools.SaveFile(file);
                    }                    
                }
            }

            return id;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Actualizar-Pago-Reembolso-Empleados:Actualizar-Pago", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }
    private void ClearControls()
    {
        tbx_fechagasto.Text = string.Empty;
        tbx_importe.Text = string.Empty;
        STipoGasto.ClearSelection();
        tbx_currency.Text = string.Empty;        
        HttpContext.Current.Session["fu_xml"] = null;
        HttpContext.Current.Session["fu_pdf"] = null;
        HttpContext.Current.Session["fu_voucher"] = null;
        HttpContext.Current.Session["xml_files"] = null;
        HttpContext.Current.Session["pdf_files"] = null;
        HttpContext.Current.Session["voucher_files"] = null;
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        tbx_pdf.Text = string.Empty;
        if (fu_pdf != null)
        {
            fu_pdf.Attributes.Clear();
        }
        if (fu_xml != null)
        {
            fu_xml.Attributes.Clear();
        }
        if (fu_voucher != null)
        {
            fu_voucher.Attributes.Clear();
        }

        btn_Guardar.Enabled = false;
        tbx_motivo.Text = string.Empty;
    }

    protected void btn_Guardar_Click(object sender, EventArgs e)
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            //Escribir info en BD
            string motivo_gasto;
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
            //int tipo_gasto = int.Parse(STipoGasto.SelectedValue);
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);           
            motivo_gasto = tbx_motivo.Text;
            //Atualizar Reembolso de gasto
            var expense = (ExpenseDTO)HttpContext.Current.Session["Expense"];
            var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
           
            var result = Update_Reembolso(expense.ExpenseId, fecha_gasto, importe_gasto,  pUserKey, pCompanyID, lista_detalles, motivo_gasto);
            if (result == -1)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B4").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_General);
                return;
            }
            ClearControls();
            Response.Redirect("ReembolsoEmpleados");
        }
        else 
        {
            btn_Guardar.Enabled = false;
        }
       
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
                    if (Facturas.Total == importe)
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
                        if (Factura.Total == importe)
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
   
    protected void GvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //Borrar articulo del grid   
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = GvItems.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int itemKey = int.Parse(row.Cells[0].Text);
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            if (lista.Count == 1)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB50").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            var detail = lista.FirstOrDefault(x => x.ItemKey == itemKey);
            lista.Remove(detail);
            if (detail.DetailId != null)
            {
                if (check_article(detail.DetailId.Value))
                {
                    delete_article(detail.DetailId.Value);
                }
            }
            HttpContext.Current.Session["GridItems"] = lista;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
    }

    protected void GvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void btn_additem_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //validacion de tipo Gasto
        if (string.IsNullOrEmpty(STipoGasto.SelectedValue.ToString()))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB26").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_General);
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
        if (tbx_importegasto.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        if (tbx_importegasto.Text == string.Empty || decimal.Parse(tbx_importegasto.Text) <= 0)
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
        detalle.UnitCost = decimal.Parse(tbx_importegasto.Text);
        if (drop_taxes.SelectedValue != "0")
        {
            detalle.STaxCodeKey = int.Parse(drop_taxes.SelectedItem.Value);
            detalle.STaxCodeID = drop_taxes.SelectedItem.Text;
            detalle.TaxAmount = taxes.FirstOrDefault(x => x.STaxCodeKey == detalle.STaxCodeKey).Rate * detalle.Amount;
        }

        if (HttpContext.Current.Session["GridItems"] != null)
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
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        drop_taxes.ClearSelection();
        // tbx_tax_amount.Text = string.Empty;       
        MultiView1.SetActiveView(View_General);

    }

    protected void tbx_fechagasto_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //Solo se admiten gastos del mes en curso
        //DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        //if (DateTime.Today.Year == fecha_gasto.Year)
        //{
        //    if (DateTime.Today.Month != fecha_gasto.Month)
        //    {
        //        tbx_fechagasto.Text = string.Empty;
        //        tipo = "error";
        //        Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB25").Value;
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        //        MultiView1.SetActiveView(View_General);
        //        return;
               
        //    }
        //}
        //else
        //{
        //    tbx_fechagasto.Text = string.Empty;
        //    tipo = "error";
        //    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB25").Value;
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        //    MultiView1.SetActiveView(View_General);
        //    return;
        //}
        HttpContext.Current.Session["fecha_gasto"] = DateTime.Parse(tbx_fechagasto.Text);
    }

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        MultiView1.SetActiveView(View_Articulos);
    }   

    protected bool check_article(int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Select count(*) FROM ExpenseDetail WHERE DetailId = @ExpenseId;";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            int count = (int)cmd.ExecuteScalar();
            cmd.Connection.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }

    protected void delete_article(int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM ExpenseDetail WHERE DetailId = @ExpenseId;";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }                 

    protected void btn_cancelar_item_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //Limpiar controles
        drop_articulos.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        MultiView1.SetActiveView(View_General);
    }

    protected void btn_Cancelar_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        Response.Redirect("ReembolsoEmpleados");
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

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btn_Guardar.Enabled = false;
        is_valid = false;
        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;            
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_General);
            return;
        }            

        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_General);
            return;
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
            foreach (HttpPostedFile postedFile in fu_xml.PostedFiles)
            {
                var xml_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Xml,
                    ExpenseType = Doc_Tools.DocumentType.Expense,
                    FileName = postedFile.FileName
                };
                byte[] byte_array = new byte[postedFile.ContentLength];
                postedFile.InputStream.Read(byte_array, 0, postedFile.ContentLength);
                xml_file.FileBinary = byte_array;
                xml_files.Add(xml_file);
            }
            HttpContext.Current.Session["xml_files"] = xml_files;
            HttpContext.Current.Session["fu_xml"] = fu_xml;

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
            foreach (HttpPostedFile postedFile in fu_pdf.PostedFiles)
            {
                var pdf_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Pdf,
                    ExpenseType = Doc_Tools.DocumentType.Expense,
                    FileName = postedFile.FileName
                };
                byte[] byte_array = new byte[postedFile.ContentLength];
                postedFile.InputStream.Read(byte_array, 0, postedFile.ContentLength);
                pdf_file.FileBinary = byte_array;
                pdf_files.Add(pdf_file);
            }
            HttpContext.Current.Session["pdf_files"] = pdf_files;
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
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
            foreach (HttpPostedFile postedFile in fu_pdf.PostedFiles)
            {
                var voucher_file = new ExpenseFilesDTO
                {
                    Type = ExpenseFilesDTO.FileType.Voucher,
                    ExpenseType = Doc_Tools.DocumentType.Expense,
                    FileName = postedFile.FileName
                };
                byte[] byte_array = new byte[postedFile.ContentLength];
                postedFile.InputStream.Read(byte_array, 0, postedFile.ContentLength);
                voucher_file.FileBinary = byte_array;
                voucher_files.Add(voucher_file);
            }
            HttpContext.Current.Session["voucher_files"] = voucher_files;
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;
        }

        //Escribir info en BD       
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);       
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);        

        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
      
        if (lista_detalles == null || lista_detalles.Count == 0)
        {          
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB41").Value;          
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_General);
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
       

        var expense = (ExpenseDTO)HttpContext.Current.Session["Expense"];
        if (!Check_Advance_Date(pUserKey, expense.AdvanceId, fecha_gasto))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB43").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //Solo alertas sin retorno
        is_valid = true;       
        tipo = "warning";

        string Msjj = string.Empty;
        HttpContext.Current.Session["is_valid"] = is_valid;

        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
        }

        if (!fu_xml.HasFile)
        {
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB31").Value;
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

        if (Msjj != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msjj + "','" + tipo + "');", true);
        }

        if (is_valid)
        {
            btn_Guardar.Enabled = true;
        }

    }

    public void fill_fileUploads()
    {
        if (fu_pdf.HasFiles)
        {
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
        }
        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            var f_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
            foreach (HttpPostedFile file in f_pdf.PostedFiles)
            {
                tbx_pdf.Text += file.FileName + " ";
            }
        }


        if (fu_xml.HasFiles)
        {
            HttpContext.Current.Session["fu_xml"] = fu_xml;
        }
        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            var f_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
            foreach (HttpPostedFile file in f_xml.PostedFiles)
            {
                tbx_xml.Text += file.FileName + " ";
            }
        }

        if (fu_voucher.HasFiles)
        {
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;

            if (HttpContext.Current.Session["fu_voucher"] != null)
            {
                var f_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
                foreach (HttpPostedFile file in f_voucher.PostedFiles)
                {
                    tbx_voucher.Text += file.FileName + " ";
                }
            }

        }
    }
    protected void View_General_Activate(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["fecha_gasto"] != null)
        {
            DateTime fechagasto = new DateTime();
            fechagasto = (DateTime)HttpContext.Current.Session["fecha_gasto"];
            tbx_fechagasto.Text = fechagasto.ToString("yyyy-MM-dd");
        }

        fill_fileUploads();
    }

    protected void View_General_Deactivate(object sender, EventArgs e)
    {
        if (fu_xml.HasFiles)
        {
            HttpContext.Current.Session["fu_xml"] = fu_xml;
        }
        if (fu_pdf.HasFiles)
        {
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
        }
        if (fu_voucher.HasFiles)
        {
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;
        }
        HttpContext.Current.Session["fecha_gasto"] = DateTime.Parse(tbx_fechagasto.Text);

    }

    
}