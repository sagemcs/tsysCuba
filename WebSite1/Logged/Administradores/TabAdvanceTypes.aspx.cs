//PORTAL DE PROVEDORES T|SYS|
//08 ABRIL, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA P
//PANTALLA PARA CARGA DE DOCUMENTOS DE PROVEEDORES

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TabAdvanceTypes : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private bool is_valid;
   
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

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        List<RolDTO> roles = Doc_Tools.get_Roles();
       

        try
        {
            if ("T|SYS| - Admin" != HttpContext.Current.Session["RolUser"].ToString())
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
                    pVendKey = 0;

                   

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
                LoadData(pCompanyID);     
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

    private void LoadData(string company)
    {
        var lista = Doc_Tools.Dict_Advancetype().Select(x => new { Id = x.Key, Name = x.Value }).ToList();
        gv_nomenclador.DataSource = lista;
        gv_nomenclador.DataBind();
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

    protected void gv_nomenclador_RowCommand(object sender, GridViewCommandEventArgs e)
    {       
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gv_nomenclador.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int typegasto_id = int.Parse(row.Cells[0].Text);
            tbx_tipogasto.Text = Doc_Tools.Dict_Advancetype().FirstOrDefault(x => x.Key == typegasto_id).Value;
            HttpContext.Current.Session["typegasto_id"] = typegasto_id;
        }
    }

    protected void gv_nomenclador_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void btn_uardar_Click(object sender, EventArgs e)
    {
        string tipo_gasto = tbx_tipogasto.Text;
        if(string.IsNullOrEmpty(tipo_gasto))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B13").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        if(HttpContext.Current.Session["typegasto_id"]!=null)
        {
            int id_gasto = (int)HttpContext.Current.Session["typegasto_id"];
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE [dbo].[AdvanceType] SET [Name] = @Name WHERE Id = @Id;";
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = tipo_gasto;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id_gasto; 
                cmd.Connection.Open();
                var inserted = cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
        }
        else 
        {
            if (Doc_Tools.Dict_Advancetype().Any(x => x.Value == tipo_gasto))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B58").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[AdvanceType] ([Name]) VALUES (@Name);";
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = tipo_gasto; 
                cmd.Connection.Open();
                var inserted = cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
        }       

        tbx_tipogasto.Text = string.Empty;
        LoadData(pCompanyID);
        HttpContext.Current.Session["typegasto_id"] = null;
    }

    private void Delete_AdvanceType(int tipo_gasto)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM [dbo].[AdvanceType] WHERE Id = @Id;";
            cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = tipo_gasto; ;
            cmd.Connection.Open();
            var inserted = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }  

    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {   
        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex;
        GridViewRow row = gv_nomenclador.Rows[rowIndex];

        if (e.CommandName == "Delete")
        {
            int typegasto_id = int.Parse(row.Cells[0].Text);
            Delete_AdvanceType(typegasto_id);

        }
        LoadData(pCompanyID);
    }

    protected void btn_limpiar_Click(object sender, EventArgs e)
    {
        tbx_tipogasto.Text = string.Empty;
        LoadData(pCompanyID);
        HttpContext.Current.Session["typegasto_id"] = null;
    }
}