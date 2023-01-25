using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ExpenseFilesDTO;

public partial class Logged_Administradores_DocumentosGastos : System.Web.UI.Page
{
    #region Variables

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

    public Dictionary<string, string> Dict_rutas ()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {                       
            { "ReembolsoEmpleados", "Expense" },
            { "TarjetaEmpleado", "CorporateCard" },
            { "GastosMedicosMenoresEmpleados", "MinorMedicalExpense" }
        };
        return dict;
    }

    public Dictionary<string, string> Dict_rutas_validador()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "ValidadorReembolsos", "Expense" },
            { "ValidadorTarjetas", "CorporateCard" },
            { "ValidadorGastosMedicosMenores", "MinorMedicalExpense" }
        };
        return dict;
    }

    string eventName = String.Empty;

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

        try
        {
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            List<RolDTO> roles = Doc_Tools.get_Roles();
            if (!roles.Any(x=> x.ID == rol))
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

                    if (!IsPostBack)
                    {
                        BindGridView();
                    }

                    pVendKey = 0;                                   

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;                       
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, pCompanyID);
                }
            }

            if (!IsPostBack)
            {
               
                // HttpContext.Current.Session["Error"] = "";
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

    private List<ExpenseDetailDTO> Load_Articles_By_Expense(int expense_id,string company_id, Doc_Tools.DocumentType type)
    {
        var items = Doc_Tools.get_items(company_id);
        List<ExpenseDetailDTO> articles = new List<ExpenseDetailDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (type)
            {               
                case Doc_Tools.DocumentType.Expense:
                    cmd.CommandText = "SELECT DetailId,ExpenseId,Type FROM ExpenseDetail where ExpenseId = @ExpenseId and CompanyId = @CompanyId;";
                    break;
                case Doc_Tools.DocumentType.CorporateCard:
                    cmd.CommandText = "SELECT DetailId,CorporateCardId,Type FROM CorporateCardDetail where CorporateCardId = @ExpenseId and CompanyId = @CompanyId;";
                    break;
                case Doc_Tools.DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "SELECT DetailId,MinorMedicalExpenseId, Itemkey FROM MinorMedicalExpenseDetail where MinorMedicalExpenseId = @ExpenseId and CompanyId = @CompanyId;";
                    break;             
            }              
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
            cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var article = new ExpenseDetailDTO();
                article.DetailId = dataReader.GetInt32(0);
                article.ExpenseId = dataReader.GetInt32(1);
                article.ItemKey = type == Doc_Tools.DocumentType.MinorMedicalExpense ? dataReader.GetInt32(2) : 0;
                article.Type = type != Doc_Tools.DocumentType.MinorMedicalExpense ? dataReader.GetInt32(2) : 0;
                article.TipoGasto = type != Doc_Tools.DocumentType.MinorMedicalExpense ? Doc_Tools.Dict_tipos_gastos().FirstOrDefault(x => x.Key == article.Type).Value : items.FirstOrDefault(x=> x.ItemKey == article.ItemKey).ItemId;
                article.FileXml = Doc_Tools.LoadFilesbyExpense(type, ExpenseFilesDTO.FileType.Xml, article.ExpenseId, article.DetailId.Value);
                article.FilePdf = Doc_Tools.LoadFilesbyExpense(type, ExpenseFilesDTO.FileType.Pdf, article.ExpenseId, article.DetailId.Value);
                article.FilePdfVoucher = Doc_Tools.LoadFilesbyExpense(type, ExpenseFilesDTO.FileType.Voucher, article.ExpenseId, article.DetailId.Value);
                articles.Add(article);
            }
            return articles;
        }
    }

    private void BindGridView()
    {
        int expense_id = (int)HttpContext.Current.Session["expense_id_visualize"];
        Doc_Tools.DocumentType type = (Doc_Tools.DocumentType)HttpContext.Current.Session["expense_type_visualize"];
        
        List<ExpenseDetailDTO> lista_articulos = Load_Articles_By_Expense(expense_id, pCompanyID, type);
        HttpContext.Current.Session["GridItems"] = lista_articulos;
        var source = lista_articulos.Select(x => new 
        { 
            ExpenseType  = type != Doc_Tools.DocumentType.MinorMedicalExpense ? Doc_Tools.Dict_tipos_gastos().FirstOrDefault(d=> d.Key == x.Type).Value : x.TipoGasto,          
            FileNameXML = x.FileXml!=null ? x.FileXml.FileName: string.Empty,
            FileNamePDF = x.FilePdf!=null ? x.FilePdf.FileName : string.Empty,
            FileNameVoucher = x.FilePdfVoucher != null ? x.FilePdfVoucher.FileName : string.Empty
        }).ToList();
        GV_Gastos.DataSource = null;
        GV_Gastos.DataSource = source;
        GV_Gastos.Visible = true;      
        GV_Gastos.DataBind();
    }
    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        var doc_type = (Doc_Tools.DocumentType)HttpContext.Current.Session["expense_type_visualize"];
        int tipo_retorno = (int)HttpContext.Current.Session["screen_type"];
        if(tipo_retorno==0)
        {
            Response.Redirect(Dict_rutas().FirstOrDefault(x => x.Value == doc_type.ToString()).Key);
        }
        else 
        {
            Response.Redirect(Dict_rutas_validador().FirstOrDefault(x => x.Value == doc_type.ToString()).Key);
        }        
    }

    protected void GV_Gastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void GV_Gastos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int idx = e.Row.RowIndex;
            List<ExpenseDetailDTO> items = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
            ExpenseDetailDTO item = items[idx];
            var img_xml = (System.Web.UI.WebControls.Image)e.Row.Cells[1].Controls[1];
            var img_pdf = (System.Web.UI.WebControls.Image)e.Row.Cells[2].Controls[1];
            var img_voucher = (System.Web.UI.WebControls.Image)e.Row.Cells[3].Controls[1];

            img_xml.ImageUrl = item.FileXml != null ? "/Img/Ok.png" : "/Img/X.png";
            img_pdf.ImageUrl = item.FilePdf != null ? "/Img/Ok.png" : "/Img/X.png";
            img_voucher.ImageUrl = item.FilePdfVoucher != null ? "/Img/Ok.png" : "/Img/X.png";

            Button btn_xml = (System.Web.UI.WebControls.Button)e.Row.Cells[1].Controls[3];
            Button btn_pdf = (System.Web.UI.WebControls.Button)e.Row.Cells[2].Controls[3];
            Button btn_voucher = (System.Web.UI.WebControls.Button)e.Row.Cells[3].Controls[3];

            btn_xml.Visible = item.FileXml != null;
            btn_pdf.Visible = item.FilePdf != null;
            btn_voucher.Visible = item.FilePdfVoucher != null;
        }
    }

    protected void DownloadFile(int index, FileType fileType)
    {
        List<ExpenseDetailDTO> items = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridItems"];
        string archivo = string.Empty;
        MemoryStream memoryStream = new MemoryStream();
        ExpenseFilesDTO file = new ExpenseFilesDTO();
        switch (fileType)
        {
            case FileType.Xml:
                file = items[index].FileXml;
                HttpContext.Current.Response.ContentType = "text/xml";
                break;
            case FileType.Pdf:
                file = items[index].FilePdf;
                HttpContext.Current.Response.ContentType = "application/pdf";
                break;
            case FileType.Voucher:
                file = items[index].FilePdfVoucher;
                HttpContext.Current.Response.ContentType = "application/pdf";
                break;          
        }
      
        memoryStream.Write(file.FileBinary, 0, file.FileBinary.Length);
        archivo += file.FileName;        
        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
        HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
        HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
        HttpContext.Current.Response.ContentType = Page.ContentType;
        HttpContext.Current.Response.End();
    }

    protected void btn_xml_Command(object sender, CommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        DownloadFile(index, FileType.Xml);
    }

    protected void btn_pdf_Command(object sender, CommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        DownloadFile(index, FileType.Pdf);
    }

    protected void btn_voucher_Command(object sender, CommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        DownloadFile(index, FileType.Voucher);
    }
}