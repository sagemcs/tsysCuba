﻿//PORTAL DE PROVEDORES T|SYS|
//20 - JULIO, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA
//PANTALLA PARA REEMBOLSOS DE EMPLEADOS
//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Linq;
using Proveedores_Model;
using WebSite1;
using System.Text;
using System.Web.Services;
using System.Diagnostics;

public partial class Logged_Administradores_ValidadorReembolsos : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;    
    public bool visible;
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
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();

        try
        {
            string rol = HttpContext.Current.Session["RolUser"].ToString();         
            int level = roles.FirstOrDefault(x => x.ID == rol).Key;
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
                    BindPackageInfo();
                   
                    upPackage.Visible = level <= 1;
                    //BindGridView();
                    if (!IsPostBack)
                    {
                        BindEmpleados();
                        BindStatus();
                        BindGridView(0, 0);
                    }
                   
                    pVendKey = 0;
                    //pLogKey = 0;
                    //pUserKey = 0;
                    //pCompanyID = "";

                    //Load_TiposGasto();

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

    private void BindGridView(int user_id, int? status_id)
    {
        var paquete = get_Package(pUserKey);
        gvGastos.DataSource = null;        
        gvGastos.Visible = true;        
        DateTime? final = !string.IsNullOrEmpty(tbx_fecha_fin.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_fin.Text) : null;
        DateTime? inicio = !string.IsNullOrEmpty(tbx_fecha_inicio.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_inicio.Text) : null;
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        List<ExpenseDTO> gastos = ReadFromDb().ToList();
        if(user_id != 0)
        {
            gastos = gastos.Where(x => x.UpdateUserKey == user_id).ToList();
        }
        if (status_id != 0)
        {
            gastos = gastos.Where(x => x.Status == Doc_Tools.Dict_status().FirstOrDefault(d => d.Key == status_id).Value).ToList();
        }
        if (inicio!=null)
        {
            gastos = gastos.Where(x => x.Date >= inicio.Value).ToList();
        }
        if(final!=null)
        {
            gastos = gastos.Where(x => x.Date <= final.Value).ToList();
        }
       
        gvGastos.DataSource = gastos.OrderByDescending(x => x.Date).ToList();
        gvGastos.DataBind();
    }

    private void BindEmpleados()
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        empleados = Doc_Tools.GetEmpleados(pUserKey, level, Doc_Tools.DocumentType.Expense);

        empleados.Add(new EmpleadoDTO() { UserKey = 0, Nombre = "Todos" });
        drop_empleados.DataSource = empleados.Select(x => new { Id = x.UserKey, Nombre = x.Nombre }).OrderBy(o => o.Id).ToList();
        drop_empleados.DataTextField = "Nombre";
        drop_empleados.DataValueField = "Id";
        drop_empleados.DataBind();
        drop_empleados.SelectedIndex = -1;
       
    }

    private void BindStatus()
    {
        var estados = Doc_Tools.Dict_status().Select((x) => new { Id = x.Key, Nombre = x.Value }).ToList();
        estados.Add(new { Id = 0, Nombre = "Todos" });
        drop_status.DataSource = estados.Where(x=> x.Id != 4).OrderBy(o => o.Id).ToList();
        drop_status.DataTextField = "Nombre";
        drop_status.DataValueField = "Id";
        drop_status.DataBind();
        drop_status.SelectedIndex = -1;
    }
    
    private void BindPackageInfo()
    {        
        var paquete = get_Package(pUserKey);
        tbx_no_paquete.Text = paquete.PackageId.ToString();
        tbx_cant_reembolsos.Text = get_expenses_package(paquete.PackageId).ToString();
        
    }   

    private List<ExpenseDTO> ReadFromDb()
    {
        List<ExpenseDTO> gastos = new List<ExpenseDTO>();
       
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();            
            cmd.CommandText = "SELECT ExpenseId ,Date ,Currency ,Amount, Status, Isnull(DeniedReason,''), Isnull(PackageId,0), Isnull(ApprovalLevel, 0), UpdateUserKey FROM Expense ;";
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
                expense.DeniedReason = dataReader.GetString(5);
                expense.PackageId = dataReader.GetInt32(6);
                expense.ApprovalLevel = dataReader.GetInt32(7);
                expense.UpdateUserKey = dataReader.GetInt32(8);
                gastos.Add(expense);
                          
            }
        }
        HttpContext.Current.Session["Gastos"] = gastos;
        return gastos;
    }

    private ExpenseDTO LoadExpenseById(int expense_id)
    {
        var expense = new ExpenseDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ExpenseId ,Date ,Currency ,Amount, Status, AdvanceId, CompanyId FROM Expense where ExpenseId = @ExpenseId;";
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
                expense.CompanyId = dataReader.GetString(6);
            }
        }
        return expense;
    }

    private List<ExpenseDetailDTO> Load_Articles_By_Expense(int expense_id, string company_id)
    {
        var lista = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        if(lista==null)
        {
            lista = Doc_Tools.get_items(company_id);
        }        
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        if (taxes==null)
        {
            taxes = Doc_Tools.get_taxes(company_id);
        }
            List<ExpenseDetailDTO> articles = new List<ExpenseDetailDTO>();        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DetailId,ExpenseId,ItemKey,Qty,UnitCost, STaxCodeKey,TaxAmount FROM ExpenseDetail where ExpenseId = @ExpenseId and CompanyId = @CompanyId;";
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
                article.STaxCodeID = taxes.FirstOrDefault(x => x.STaxCodeKey == article.STaxCodeKey) !=null ? taxes.FirstOrDefault(x => x.STaxCodeKey == article.STaxCodeKey).STaxCodeID : string.Empty;                
                articles.Add(article);               
            }
        }
        return articles;
    }

    protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {      
        int rowIndex = ((GridViewRow)((Control)e.CommandSource).BindingContainer).RowIndex;
        GridViewRow row = gvGastos.Rows[rowIndex];
        int expense_id = int.Parse(row.Cells[0].Text);
        var paquete = get_Package(pUserKey);
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores();
        int level_validador = roles.FirstOrDefault(x => x.ID == rol).Key;
        int user_id = 0;
        int status_id = 0;

        if (level_validador==1)
        {
            if (paquete.PackageId == 0)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B53").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;              
            }
        }       
       
        if (e.CommandName == "Deny")
        {
            int status = 3;
            TextBox motivo = (TextBox)(Control)row.Cells[8].Controls[1];
            if (string.IsNullOrEmpty(motivo.Text))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B31").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;              
            }
           
            Update_Expense(expense_id, paquete.PackageId, status, motivo.Text, level: level_validador);
            Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Expense, pUserKey, level_validador, Doc_Tools.NotificationType.Denegacion);
            BindPackageInfo();            
           
        }
        if (e.CommandName == "Coment")
        {
            HttpContext.Current.Session["ExpenseComentId"] = expense_id;
            HttpContext.Current.Session["DocumentType"] = Doc_Tools.DocumentType.Expense;
            Response.Redirect("ComentariosValidador.aspx");
        }
        if (e.CommandName == "Integrate")
        {
            //Coger los datos del documento y lanzar las api Integrar
            var expense = LoadExpenseById(expense_id);
            if(!expense.SageIntegration)
            {
                var detalles = Load_Articles_By_Expense(expense_id, pCompanyID);
                int oRetvalue = Doc_Tools.ExecuteVoucherApi("admin", expense, detalles, Doc_Tools.DocumentType.Expense);
                if (oRetvalue == 1)
                {
                    Doc_Tools.CheckSageIntegration(expense_id, Doc_Tools.DocumentType.Expense);

                    if (drop_empleados.SelectedItem != null)
                    {
                        user_id = int.Parse(drop_empleados.SelectedItem.Value);
                    }
                    status_id = int.Parse(drop_status.SelectedItem.Value);
                    BindGridView(user_id, status_id);

                    tipo = "success";
                    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B32").Value;
                    ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    return;
                }
                else
                {
                    tipo = "error";
                    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B33").Value;
                    ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    return;
                }
            }
            else
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B35").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            
        }

        
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        
        status_id = int.Parse(drop_status.SelectedValue);
        BindGridView(user_id, status_id);
    }

    protected void Create_Package(int user_id, string comment)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "update ExpensePackage set ClosedAt = GETDATE() where ClosedAt is null and UserId = @UserId";
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                cmd.CommandText = "INSERT INTO ExpensePackage (UserId, Description, CreatedAt) VALUES (@UserId, @Description, @CreatedAt); SELECT SCOPE_IDENTITY();";
                //cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = comment;
                cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }           
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Crear-Paquete-Reembolso-Empleados:Crear-Paquete", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;            
        }
    }

    protected void Update_Expense(int expense_id, int package_id, int status, string motivo, int level=1)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE Expense SET Status = @Status ");
                if(level==1)
                {
                    sb.Append(", PackageId = @PackageId");
                    cmd.Parameters.Add("@PackageId", SqlDbType.Int).Value = package_id;
                }
                
                sb.Append(", UpdateDate = @UpdateDate, DeniedReason = @DeniedReason, ApprovalLevel = @ApprovalLevel  WHERE ExpenseId = @ExpenseId;");
                cmd.CommandText = sb.ToString();
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;                
                cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Parameters.Add("@DeniedReason", SqlDbType.VarChar).Value = motivo;
                cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = level;
                cmd.Connection.Open();               
                var modified = cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Update-Gasto-Reembolso-Empleados:Update-Gasto", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;           
        }

    }

    protected void drop_empleados_SelectedIndexChanged(object sender, EventArgs e)
    {       
        int user_id;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        else
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B51").Value;
            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void btn_filtrar_Click(object sender, EventArgs e)
    {
        tbx_fecha_fin.Text = string.Empty;
        tbx_fecha_inicio.Text = string.Empty;
        drop_empleados.SelectedIndex = -1;
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void tbx_fecha_inicio_TextChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void tbx_fecha_fin_TextChanged(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(tbx_fecha_inicio.Text))
        {
            DateTime inicio = DateTime.Parse(tbx_fecha_inicio.Text);
            DateTime final = DateTime.Parse(tbx_fecha_fin.Text);
            if(final<inicio)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B56").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                tbx_fecha_fin.Text = string.Empty;
                return;
            }
        }
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected int get_expenses_package(int PackageId)
    {
        int expense_count = 0;
        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*)  FROM Expense where PackageId = @PackageId;";
            cmd.Parameters.Add("@PackageId", SqlDbType.Int).Value = PackageId;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                expense_count = dataReader.GetInt32(0);                
            }
        }
        return expense_count;
    }

    protected void btn_crear_paquete_Click(object sender, EventArgs e)
    {
        //Logica para crear un paquete
        var paquete = get_Package(pUserKey);
        //diferente de paquete inicial
        if(paquete.PackageId!=0)
        {
            if (get_expenses_package(paquete.PackageId) == 0)
            {                                   
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B30").Value;      
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }            
        }
        
        Create_Package(pUserKey, "comentario vacio");
        BindPackageInfo();        
        
    }

    protected Doc_Tools.Paquete get_Package(int user_id)
    {        
        int PackageId;
        DateTime CreatedAt;
        var paquete = new Doc_Tools.Paquete();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PackageId, CreatedAt FROM ExpensePackage where UserId = @UserId and ClosedAt is null order by CreatedAt desc;";
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {                
                PackageId = dataReader.GetInt32(0);
                CreatedAt = dataReader.GetDateTime(1);
                paquete.PackageId = PackageId;
                paquete.CreatedAt = CreatedAt;                          
            }
        }
        return paquete;
    }

    protected void drop_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }

        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void gvGastos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        List<ExpenseDTO> gastos = (List<ExpenseDTO>)HttpContext.Current.Session["Gastos"];
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        //7 - Aprobar
        //8 - Denegar
        //9 - Motivos Denegacion
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int expense_id = int.Parse(e.Row.Cells[0].Text);
            var gasto = gastos.FirstOrDefault(x => x.ExpenseId == expense_id);

            Button btn_aprobar = (Button)e.Row.Cells[6].Controls[1];
            Button btn_denegar = (Button)e.Row.Cells[7].Controls[1];
            Button btn_comentar = (Button)e.Row.Cells[9].Controls[1];
            TextBox tbx_motivo = (TextBox)e.Row.Cells[8].Controls[1];
            Button btn_integrar = (Button)e.Row.Cells[10].Controls[1];

            switch (e.Row.Cells[5].Text)
            {
                case "Pendiente":
                    if (level - gasto.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = true;
                        btn_denegar.Visible = true;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = false;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
                    }                    
                    else
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = false;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
                    }
                    break;
                case "Aprobado":
                    if (level - gasto.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = true;
                        btn_denegar.Visible = true;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = false;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
                    }
                    else if (gasto.ApprovalLevel == level)
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = false;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
                    }
                    else
                    {
                        btn_aprobar.Visible = false;
                        btn_comentar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.ReadOnly = true;
                        tbx_motivo.Visible = false;
                        btn_integrar.Visible = gasto.ApprovalLevel == roles.Max(x => x.Key) && level == 2;
                    }
                    break;
                case "Denegado":
                    if (level - gasto.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = true;
                    }
                    else if (gasto.ApprovalLevel == level)
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = true;
                    }
                    else
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = true;
                    }
                    break;
                case "Vencido":
                    btn_aprobar.Visible = false;
                    btn_denegar.Visible = false;
                    tbx_motivo.Visible = false;
                    tbx_motivo.ReadOnly = true;
                    btn_integrar.Visible = false;
                    btn_comentar.Visible = false;
                    break;
                case "Integrado":
                    btn_aprobar.Visible = false;
                    btn_denegar.Visible = false;
                    tbx_motivo.Visible = false;
                    tbx_motivo.ReadOnly = true;
                    btn_integrar.Visible = false;
                    btn_comentar.Visible = false;
                    break;
                default:
                    break;
            }        

        }
    }

    public void EnviarCorreo(bool accion)
    {
        string acction_text = accion ? "Aprobado" : "Denegado";
        //Segun el nivel del usuario logueado traer la matriz
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        var matrix = Doc_Tools.get_MatrizValidadores(pUserKey, level);
        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.Expense));
        var orden = jerarquia.Get_Orden(jerarquia);
        string server_address = ConfiguracionCorreoElectronico.server_address;
        int server_port = ConfiguracionCorreoElectronico.server_port;
        string user = ConfiguracionCorreoElectronico.user;
        string password = ConfiguracionCorreoElectronico.password;
        bool enable_ssl = ConfiguracionCorreoElectronico.enable_ssl;
        CorreoElectronico email = new CorreoElectronico(server_address, server_port, user, password, enable_ssl);

        //Tomar el nivel superior que corresponde
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

    protected void btnVisualize_Command(object sender, CommandEventArgs e)
    {     
        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex;
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Visualize")
        {
            int expense_id = int.Parse(row.Cells[0].Text);
            HttpContext.Current.Session["expense_id_visualize"] = expense_id;
            HttpContext.Current.Session["expense_type_visualize"] = Doc_Tools.DocumentType.Expense;
            HttpContext.Current.Session["screen_type"] = 1;
            Response.Redirect("DocumentosGastos");
        }
    }

    protected void btnAprobar_Command(object sender, CommandEventArgs e)
    {       
        int status_id = 0, user_id = 0;       
        int expense_id = int.Parse(hh1.Value);
        int status = 2;
        var paquete = get_Package(pUserKey);
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores();
        int level_validador = roles.FirstOrDefault(x => x.ID == rol).Key;
        Update_Expense(expense_id, paquete.PackageId, status, string.Empty, level: level_validador);
        Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Expense, pUserKey, level_validador, Doc_Tools.NotificationType.Aprobacion);
        BindPackageInfo();
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }

        status_id = int.Parse(drop_status.SelectedValue);
        BindGridView(user_id, status_id);

    }

    [WebMethod]
    public static string ResolveToken()
    {        
        Contrarecibos.CrearToken();
        string token_vigente = Doc_Tools.Get_token_from_db(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
        return token_vigente;
    }   

    [WebMethod]
    public static string ComparePassword(string pass)
    {
        int userkey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
        string result = Doc_Tools.CheckPassword(userkey, pass).ToString();
        return result;
    }
}