using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSite1;

public partial class Logged_Administradores_AdmUsers : System.Web.UI.Page
{
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {   
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                {

                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        BindGridView();
                    }
                    else
                    {
                        HttpContext.Current.Session.RemoveAll();
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/Account/Login.aspx");
                    }

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

            if (HttpContext.Current.Session["RolUser"].ToString() != "T|SYS| - Admin")
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
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

    protected void Buscar(object sender,EventArgs e)
    {
        try
        {
            BindGridView();
        }
        catch (Exception ex)
        {
            
        }
    }

    private void BindGridView()
    {
        DatosV.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            string SpFiltros;
            SpFiltros = Filtros();
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand(SpFiltros, conn);

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();

            string Errores = string.Empty;
            SqlDataReader rdr = cmd.ExecuteReader();
            
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            while (rdr.Read())
            {
                DataRow row = dt.NewRow();

                row["Nombre"] = Convert.ToString(rdr.GetValue(0));
                row["Correo"] = Convert.ToString(rdr.GetValue(1));
                row["Fecha"] = Convert.ToString(rdr.GetValue(2));
                row["Creador"] = Convert.ToString(rdr.GetValue(4));
                dt.Rows.Add(row);
                
            }
        }

        GridView1.DataSource = dt;
        GridView1.DataBind();



        if (dt.Rows.Count == 0) { DatosV.Visible = true; }
        else
        {
            int cont = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string SpFiltros;
                SpFiltros = Filtros();
                DataSet dsProveedores = new DataSet();
                SqlCommand cmd = new SqlCommand(SpFiltros, conn);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DropDownList Status = GridView1.Rows[cont].Cells[5].FindControl("Stat") as DropDownList;
                    Status.SelectedIndex = Convert.ToInt16(rdr.GetValue(5)) - 1;
                    cont = cont + 1;
                }
            }
        }
    }

    private string Filtros()
    {
        string Cadena = string.Empty;
        Cadena = "Select a.UserName,a.UserID,a.CreateDate,b.Descripcion,d.RoleID,a.Status From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey Where b.Descripcion = '";
        Cadena = Cadena + Cclientes.SelectedItem.ToString() + "' ";

        if (Email.Text != "") { Cadena = Cadena + " AND a.UserID LIKE '%" + Email.Text + "%' "; }
        if (Nombre.Text != "") { Cadena = Cadena + " AND a.UserName LIKE '%" + Nombre.Text + "%' "; }
        if (DRol.SelectedItem.ToString() != "Seleccione Rol") { Cadena = Cadena + " AND d.RoleID LIKE '%" + DRol.SelectedItem.ToString() + "%' "; }
        if (F1.Text != ""){Cadena = Cadena + " AND a.CreateDate >= '" + F1.Text + "' ";}if (F2.Text != ""){Cadena = Cadena + " AND a.CreateDate <= '" + F2.Text + " 23:59:59' "; }
        return Cadena;

    }

    protected void Clean(object sender,EventArgs e)
    {
        try
        {
            DRol.SelectedValue = "1";
            Cclientes.SelectedValue = "1";
            F1.Text = "";
            F2.Text = "";
            Email.Text = "";
            Nombre.Text = "";
            BindGridView();
        }
        catch (Exception ex)
        {

        }
    }

    protected void Roles(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectRol", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Rol"].ToString() == "T|SYS| - Admin" || rdr["Rol"].ToString() == "T|SYS| - Validador" || rdr["Rol"].ToString() == "T|SYS| - Consultas")
                    {
                        ListItem Rol = new ListItem();
                        Rol.Value = (rdr["Rol"].ToString());
                        Caja.Items.Insert(0, Rol);
                    }
                }
                conn.Close();
            }
        }
        catch
        {

        }
    }

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, string.Empty).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //GridView1.PageIndex = e.NewPageIndex;
        //BindGridView();
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected void GridView2_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        try
        {
            int i, x, z;
            z = 0;
            i = e.NewEditIndex;
            int[] num = new int[80];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                  DataRow dr = dt.NewRow();
                  DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
                  dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                  dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                  dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                  dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                  num[z] = Status.SelectedIndex +1 ;
                  z = z + 1;
                  dt.Rows.Add(dr);
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            z =0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
              DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
              Status.SelectedIndex = num[z]-1;
              z = z + 1;
            }
            

        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x,z;
            z = 0;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            int[] num = new int[80];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                 DataRow dr = dt.NewRow();
                 DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
                 dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                 dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                 dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                 dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                 num[z] = Status.SelectedIndex + 1;
                 z = z + 1;
                 dt.Rows.Add(dr);
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

        }
        catch (Exception v)
        {

        }

    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x,z;
            z = 0;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            int[] num = new int[80];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            string strName = ((TextBox)GridView1.Rows[e.RowIndex].Cells[6].Controls[0]).Text;
            string EmailUs = HttpUtility.HtmlDecode(GridView1.Rows[e.RowIndex].Cells[2].Text.ToString());

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    z = z + 1;
                    dt.Rows.Add(dr);
                }
            }



            if (Email_Ok(strName) == true)
            {
                if (EmailT(strName, EmailUs) == true)
                {
                    Label4.Text = "Actualizacion de password exitosa,Se ha enviado un correo de restablecimiento de contraseña a " + strName;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                }
                else
                {
                    Labe5.Text = "Se Generaron errores al cargar la configuracion para el restablecimiento de contraseña, Verifica la conexion SQL";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                }
            }
            else
            {
                Labe5.Text = "Formato de correo Invalido, Revisalo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
            }


            GridView1.DataSource = dt;
            GridView1.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }
        }
        catch (Exception v)
        {
            string err = v.Message;
        }
    }

    protected void Cclientes_SelectedIndexChanged(object sender, EventArgs e)
    {
      BindGridView();
    }

    protected bool UpdatePAss(string Email, string NewPass)
    {
        bool Resultado = false;
        try
        {
            string oldPass = string.Empty;
            string IdUser = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd1 = new SqlCommand("spResetPass", conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Pass", Value = NewPass });

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = Email });

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 2 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                Resultado = true;
                conn.Close();
            }
        }

        catch (Exception ex)
        {
            string Error = ex.Message;
            Resultado = false;
        }
        return Resultado;
    }

    private bool EmailT(string CorreoUs,string UserEmail)
    {
        bool Resut = false;
        try
        {
            string PassNew = Membership.GeneratePassword(8, 1);
            string PassHAs = PassNew.ToString();
            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            bool UpdatePass = UpdatePAss(UserEmail, hashedNewPassword);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
            {
                body = reader.ReadToEnd();
                body = body.Replace("{PassTemp}", PassHAs);

            }

            Resut = Global.EmailGlobal(CorreoUs, body, "RECUPERACIÓN DE CONTRASEÑA");
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            if (GridView1.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    int Ukey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
                    DropDownList Status = gvr.Cells[5].FindControl("Stat") as DropDownList;

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spUpdateUserPass", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Email });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = Ukey });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Valor", Value = Status.SelectedItem.ToString() });

                        if (conn.State == ConnectionState.Open) { conn.Close(); }

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        conn.Close();
                    }
                }

                Label4.Text = "Actualizacion Exitosa ";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                BindGridView();
            }
            else
            {

            }
        }
        catch (Exception ex)
        {
            Labe5.Text = "Se generaron errores al intentar actualizar al usuario, verifica la conexion y datos SQL ";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
        }
    }
}