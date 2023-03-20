//PORTAL DE PROVEDORES T|SYS|
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

public partial class Logged_Administradores_ValidadorAnticipos : System.Web.UI.Page
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

        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().Where(x => x.Key != 4).ToList();

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
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    BindPackageInfo();
                   
                    upPackage.Visible = roles.Min(x => x.Key) == level;
                    //BindGridView();
                    if (!IsPostBack)
                    {
                        BindEmpleados();
                        BindStatus();
                        BindType();
                        BindGridView(0, 0, 0);
                    }
                   
                    pVendKey = 0;                   
                  
                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;                       
                    }
                }
                catch (Exception xD)
                {
                    LogError(0, 1, "ValidadorAnticipos_Load", "Error al Cargar Variables de Sesion : " + xD.Message, "TSM");
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

    private void BindGridView(int user_id, int? status_id, int? type)
    {
        gvAnticipos.DataSource = null;
        gvAnticipos.Visible = true;
        DateTime? final = !string.IsNullOrEmpty(tbx_fecha_fin.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_fin.Text) : null;
        DateTime? inicio = !string.IsNullOrEmpty(tbx_fecha_inicio.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_inicio.Text) : null;
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores().Where(x => x.Key != 4).ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        List<AdvanceDTO> anticipos = ReadFromDb();
        if (user_id != 0)
        {
            anticipos = anticipos.Where(x => x.UpdateUserKey == user_id).ToList();
        }
        else
        {
            var empleados = Doc_Tools.GetEmpleados(pUserKey, level, Doc_Tools.DocumentType.Advance).Select(x=> x.UserKey).ToList();
            anticipos = anticipos.Where(x => empleados.Contains(x.UpdateUserKey)).ToList();
        }
        if(status_id!=0)
        {
            anticipos = anticipos.Where(x => x.Status == Doc_Tools.Dict_status().FirstOrDefault(d => d.Key == status_id).Value).ToList();
        }  
        if(type!=0)
        {
            anticipos = anticipos.Where(x => x.AdvanceType == Dict_type().FirstOrDefault(d => d.Key == type).Value).ToList();
        }
        if (inicio!=null)
        {
            anticipos = anticipos.Where(x => x.CheckDate >= inicio).ToList();
        }
        if(final!=null)
        {
            anticipos = anticipos.Where(x => x.CheckDate <= final).ToList();
        }

        
        gvAnticipos.DataSource = anticipos.OrderByDescending(x => x.CheckDate).ToList();
        gvAnticipos.DataBind();
    }

    private void BindEmpleados()
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().Where(x => x.Key != 4).ToList();
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;       
        empleados = Doc_Tools.GetEmpleados(pUserKey, level, Doc_Tools.DocumentType.Advance);     

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
        estados.Add(new { Id=0, Nombre="Todos" });
        drop_status.DataSource = estados.OrderBy(o => o.Id).ToList();
        drop_status.DataTextField = "Nombre";
        drop_status.DataValueField = "Id";
        drop_status.DataBind();
        drop_status.SelectedIndex = -1;
    }

    private void BindType()
    {
        var tipos = Dict_type().Select((x) => new { Id = x.Key, Nombre = x.Value }).ToList();
        tipos.Add(new { Id = 0, Nombre = "Todos" });
        drop_tipo.DataSource = tipos.OrderBy(o => o.Id).ToList();
        drop_tipo.DataTextField = "Nombre";
        drop_tipo.DataValueField = "Id";
        drop_tipo.DataBind();
        drop_tipo.SelectedIndex = -1;
    }

    private void BindPackageInfo()
    {        
        var paquete = get_Package(pUserKey);
        tbx_no_paquete.Text = paquete.PackageId.ToString();
        tbx_cant_reembolsos.Text = get_expenses_package(paquete.PackageId).ToString();        
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

    private List<AdvanceDTO> ReadFromDb()
    {
        List<AdvanceDTO> anticipos = new List<AdvanceDTO>();       

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();    
            cmd.CommandText = "SELECT AdvanceId, AdvanceType, Folio,Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey, CreateDate, UpdateDate,CompanyId,Status, Isnull(PackageId,0), IsNull(DeniedReason, ''), Isnull(ApprovalLevel , 0) FROM Advance;";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var advance = new AdvanceDTO();
                advance.AdvanceId = dataReader.GetInt32(0);
                advance.AdvanceType = Dict_type().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
                advance.Folio = dataReader.GetString(2);
                advance.Amount = dataReader.GetDecimal(3);
                if(!dataReader.IsDBNull(4))
                {
                    advance.DepartureDate = dataReader.GetDateTime(4);
                }
                if(!dataReader.IsDBNull(5))
                {
                    advance.ArrivalDate = dataReader.GetDateTime(5);
                }                
                advance.CheckDate = dataReader.GetDateTime(6);
                advance.ImmediateBoss = dataReader.GetString(7);
                advance.UpdateUserKey = dataReader.GetInt32(8);
                advance.CreateDate = dataReader.GetDateTime(9);
                advance.UpdateDate = dataReader.GetDateTime(10);
                advance.CompanyId = dataReader.GetString(11);
                advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x => x.Key == dataReader.GetInt32(12)).Value;
                advance.PackageId = dataReader.GetInt32(13);
                advance.DeniedReason = dataReader.GetString(14);
                advance.ApprovalLevel = dataReader.GetInt32(15);
                advance.Causante = Doc_Tools.get_causante(advance.UpdateUserKey);
                anticipos.Add(advance);                                                
            }
        }
        HttpContext.Current.Session["Anticipos"] = anticipos;
        return anticipos;
    }     

    protected void gvAnticipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int rowIndex = ((GridViewRow)((Control)e.CommandSource).BindingContainer).RowIndex;
        GridViewRow row = gvAnticipos.Rows[rowIndex];
        int advance_id = int.Parse(row.Cells[0].Text);
        var paquete = get_Package(pUserKey);
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores().Where(x => x.Key != 4).ToList();
        int level_validador = roles.FirstOrDefault(x => x.ID == rol).Key;
        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        if(drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        if (level_validador == 1)
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
            TextBox motivo = (TextBox)(Control)row.Cells[12].Controls[1];
            if (string.IsNullOrEmpty(motivo.Text))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B31").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            Update_Advance(advance_id, paquete.PackageId, status, motivo.Text, level: level_validador);
            var advance = LoadAdvanceById(advance_id);            
            Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Advance, advance.UpdateUserKey, level_validador, Doc_Tools.NotificationType.Denegacion, pUserKey, motivo.Text);
            BindPackageInfo();
            BindGridView(user_id, status_id, type);
        }

        if (e.CommandName == "Coment")
        {
            var advance = LoadAdvanceById(advance_id);
           // Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Advance, advance.UpdateUserKey, level_validador);
            HttpContext.Current.Session["ExpenseComentId"] = advance_id;
            HttpContext.Current.Session["DocumentType"] = Doc_Tools.DocumentType.Advance;
            Response.Redirect("ComentariosValidador.aspx");
        }  

        if(e.CommandName=="Integrate")
        {
            //Coger los datos del documento y lanzar las api Integrar
            var advance = LoadAdvanceById(advance_id);
            if(!advance.SageIntegration)
            {
                int oRetvalue = Doc_Tools.ExecuteVoucherApi("admin", advance, new List<ExpenseDetailDTO>(), Doc_Tools.DocumentType.Advance);
                if (oRetvalue == 1)
                {
                    Doc_Tools.CheckSageIntegration(advance_id, Doc_Tools.DocumentType.Advance);

                    if (drop_empleados.SelectedItem != null)
                    {
                        user_id = int.Parse(drop_empleados.SelectedItem.Value);
                    }
                    status_id = int.Parse(drop_status.SelectedItem.Value);

                    BindGridView(user_id, status_id, type);

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
        BindGridView(user_id, status_id, type);
        BindPackageInfo();
    }

    private AdvanceDTO LoadAdvanceById(int advance_id)
    {
        var advance = new AdvanceDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,UpdateDate,CompanyId,Status, CreateDate FROM Advance where AdvanceId = @AdvanceId;";
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
                advance.CreateDate = dataReader.GetDateTime(12);
            }
        }
        return advance;
    }

    protected void Create_Package(int user_id, string comment)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "update AdvancePackage set ClosedAt = GETDATE() where ClosedAt is null and UserId = @UserId";
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                cmd.CommandText = "INSERT INTO AdvancePackage (UserId, Description, CreatedAt) VALUES (@UserId, @Description, @CreatedAt); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = comment;
                cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }           
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Crear-Paquete-Anticipo-Empleados:Crear-Paquete", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;            
        }
    }

    protected void Update_Advance(int advance_id, int package_id, int status, string motivo = "", int level=1)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE Advance SET UpdateDate = @UpdateDate , Status = @Status ");
                if (level == 1)
                {
                    sb.Append(", PackageId = @PackageId");
                    cmd.Parameters.Add("@PackageId", SqlDbType.Int).Value = package_id;
                }               
                sb.Append(", DeniedReason = @DeniedReason, ApprovalLevel = @ApprovalLevel  where AdvanceId = @AdvanceId;");
                cmd.CommandText = sb.ToString();
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;              
                cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
                cmd.Parameters.Add("@DeniedReason", SqlDbType.VarChar).Value = motivo;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = level;
                
                cmd.Connection.Open();               
                var modified = cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Update-Gasto-Anticipo-Empleados:Update-Anticipo", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;           
        }
    }

    protected void drop_empleados_SelectedIndexChanged(object sender, EventArgs e)
    {    
        int user_id = 0;
        int type = 0;
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
        if(drop_tipo.SelectedItem!=null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }       
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id, type);
    }

    protected void btn_filtrar_Click(object sender, EventArgs e)
    {
        tbx_fecha_fin.Text = string.Empty;
        tbx_fecha_inicio.Text = string.Empty;
        drop_empleados.SelectedIndex = -1;
        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id,type);
    }

    protected void tbx_fecha_inicio_TextChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id, type);
    }

    protected void tbx_fecha_fin_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(tbx_fecha_inicio.Text))
        {
            DateTime inicio = DateTime.Parse(tbx_fecha_inicio.Text);
            DateTime final = DateTime.Parse(tbx_fecha_fin.Text);
            if (final < inicio)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B56").Value;
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                tbx_fecha_fin.Text = string.Empty;
                return;
            }
        }

        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id, type);
    }

    protected int get_expenses_package(int PackageId)
    {
        int expense_count = 0;
        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*)  FROM Advance where PackageId = @PackageId;";
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
        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id, type);

    }

    protected Doc_Tools.Paquete get_Package(int user_id)
    {                
        var paquete = new Doc_Tools.Paquete();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PackageId, CreatedAt FROM AdvancePackage where UserId = @UserId and ClosedAt is null order by CreatedAt desc;";
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                paquete.PackageId = dataReader.GetInt32(0);
                paquete.CreatedAt = dataReader.GetDateTime(1);                                     
            }
        }
        return paquete;
    }

    protected void drop_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        int type = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        
        BindGridView(user_id, status_id, type);
    }

    protected void drop_tipo_SelectedIndexChanged(object sender, EventArgs e)
    {
        int type = 0;
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
      
        BindGridView(user_id, status_id, type);
    }

    protected void gvAnticipos_RowDataBound(object sender, GridViewRowEventArgs e)
    {       
        List<AdvanceDTO> anticipos = (List<AdvanceDTO>)HttpContext.Current.Session["Anticipos"];
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().Where(x => x.Key != 4).ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;

        var paquete = get_Package(pUserKey);
        if (level == 1)
        {
            if (paquete.PackageId == 0)
            {
                paquete = null;
            }
        }
       
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int advance_id = int.Parse(e.Row.Cells[0].Text);
            var anticipo = anticipos.FirstOrDefault(x => x.AdvanceId == advance_id);
            
            Button btn_aprobar = (Button)e.Row.Cells[10].Controls[1];
            Button btn_denegar = (Button)e.Row.Cells[11].Controls[1];
            TextBox tbx_motivo = (TextBox)e.Row.Cells[12].Controls[1];
            Button btn_comentar = (Button)e.Row.Cells[13].Controls[1];
            Button btn_integrar = (Button)e.Row.Cells[14].Controls[1];
            
            switch (e.Row.Cells[9].Text)
            {
                case "Pendiente":
                    if (level - anticipo.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = paquete != null ? true : false;//true;
                        btn_denegar.Visible = paquete != null ? true : false; //true;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = false;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
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
                case "Aprobado":
                    if (level - anticipo.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = paquete != null ? true : false; //true;
                        btn_denegar.Visible = paquete != null ? true : false; // true;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = false;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = false;
                    }
                    else if(anticipo.ApprovalLevel == level)
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
                        btn_integrar.Visible = anticipo.ApprovalLevel == roles.Max(x => x.Key) && level == 2;
                    }
                    break;
                case "Denegado":
                    if(level - anticipo.ApprovalLevel == 1)
                    {
                        btn_aprobar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.Visible = true;
                        tbx_motivo.ReadOnly = true;
                        btn_integrar.Visible = false;
                        btn_comentar.Visible = true;
                    }
                    else if (anticipo.ApprovalLevel == level)
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

    protected void btnAprobar_Command(object sender, CommandEventArgs e)
    {
        int status_id = 0, user_id = 0, type = 0;
        int expense_id = int.Parse(hh1.Value);
        int status = 2;
        var paquete = get_Package(pUserKey);
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores().Where(x=> x.Key != 4).ToList();
        int level_validador = roles.FirstOrDefault(x => x.ID == rol).Key;
        var advance = LoadAdvanceById(expense_id);
        if (drop_tipo.SelectedItem != null)
        {
            type = int.Parse(drop_tipo.SelectedItem.Value);
        }

        Update_Advance(expense_id, paquete.PackageId, status, string.Empty, level: level_validador);      
        BindPackageInfo();
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }

        status_id = int.Parse(drop_status.SelectedValue);
       
        if (roles.FirstOrDefault(x => x.ID == rol).Key == roles.Max(z => z.Key))
        {
            Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Advance, advance.UpdateUserKey, level_validador, Doc_Tools.NotificationType.Integracion, pUserKey);
        }
        else 
        {
            Doc_Tools.EnviarCorreo(Doc_Tools.DocumentType.Advance, advance.UpdateUserKey, level_validador, Doc_Tools.NotificationType.Aprobacion, pUserKey);
        }
        
        BindGridView(user_id, status_id, type);

    }



}