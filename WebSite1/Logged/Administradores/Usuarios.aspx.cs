using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNet.Identity;
using WebSite1;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

public partial class Pagos : System.Web.UI.Page
{
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
                        GridView1.AllowPaging = true;
                        GridView1.PageSize = 15;
                        GridView2.AllowPaging = true;
                        GridView2.PageSize = 15;
                        GridView1.AllowSorting = true;
                        GridView2.AllowSorting = true;
                        BindGridView(); //Proveedores
                        BindGridView2(); //TSYS
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

    private void BindGridView()
    {
        DatosV.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            // Create a DataSet object. 
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand("spSelectUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Opcion",
                Value = 1
            });

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                DatosV.Visible = true;
            }
            while (rdr.Read())
            {
                DataRow row = dt.NewRow();
                row["Fecha"] = (rdr["Fecha"].ToString());
                row["UserID"] = HttpUtility.HtmlDecode(rdr["UserID"].ToString());
                row["UserName"] = HttpUtility.HtmlDecode(rdr["UserName"].ToString());
                row["Empresa"] = HttpUtility.HtmlDecode(rdr["Empresa"].ToString());
                row["Estado"] = HttpUtility.HtmlDecode(rdr["Status"].ToString());

                dt.Rows.Add(row);
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
              
        }

    }

    string eventName = String.Empty;

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        GridView1.PageIndex = e.NewPageIndex;


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
        GridView1.EditIndex = e.NewEditIndex;
        try
        {
            int x;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

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
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                    //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                    // HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dt.Rows.Add(dr);
                 }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
            //GridView1.EditIndex = -1;
            //BindGridView();
        }
        catch (Exception v)
        {

        }

        // Rebind the GridView control to show data in edit mode. 
        //BindGridView();
    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

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
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "No Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
            //
            //BindGridView();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        try
        {   
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

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
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }
            
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        catch (Exception v)
        {

        }
         
    }



    private void BindGridView2()
    {
        DatosT.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand("spSelectUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Opcion",
                Value = 2
            });

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                DatosT.Visible = true;
            }
            while (rdr.Read())
            {
                DataRow row = dt.NewRow();
                row["Fecha"] = HttpUtility.HtmlDecode(rdr["Fecha"].ToString());
                row["UserID"] = HttpUtility.HtmlDecode(rdr["UserID"].ToString());
                row["UserName"] = HttpUtility.HtmlDecode(rdr["UserName"].ToString());
                //row["Empresa"] = HttpUtility.HtmlDecode(rdr["Empresa"].ToString());
                row["Estado"] = HttpUtility.HtmlDecode(rdr["Status"].ToString());

                ///dr["Fecha"] = HttpUtility.HtmlDecode(rdr["Fecha"].ToString());

                dt.Rows.Add(row);
            }
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView2.PageIndex = e.NewPageIndex;
        BindGridView2();
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
        GridView2.EditIndex = e.NewEditIndex;
        try
        {
            int x;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                    //
                    dt.Rows.Add(dr);
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView2.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "No Aprobado";
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
            //
            //BindGridView();
        }
        catch (Exception v)
        {

        }

    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView2.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void Aceptar()
    {
        try
        {
            int cont = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                if (gvr.Cells[5].Text.ToString() == "Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    Alta(Email,Razon,Company,1,2);
                    if (EmailT(Email) == true)
                    {

                    }
                    cont = cont + 1;
                }
                else if (gvr.Cells[5].Text.ToString() == "No Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    Alta(Email, Razon, Company, 2,2);
                    cont = cont + 1;
                }
            }

            if (cont >= 1)
            {
                string titulo, Msj, tipo;
                tipo = "success";
                Msj = " Se Completo el Proceso Exitosamente!!";
                titulo = "Aprobacion de Usuario";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                string titulo, Msj, tipo;
                tipo = "info";
                Msj = "No se Ha Detectado Ningun Cambio";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch
        {

        }
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


    private bool EmailT(string CorreoUs)
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
         bool UpdatePass = UpdatePAss(CorreoUs, hashedNewPassword);

         string body = string.Empty;
         using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOkUser.html")))
         {
           body = reader.ReadToEnd();
           body = body.Replace("{UserTemp}", CorreoUs);
           body = body.Replace("{PassTemp}", PassHAs);
         }

            Resut = Global.EmailGlobal(CorreoUs, body, "BIENVENIDO AL PORTAL T|SYS|");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected void AceptarTsys()
    {
        try
        {
            int cont = 0;
            int errs = 0;
            foreach (GridViewRow gvr in GridView2.Rows)
            {
                if (gvr.Cells[4].Text.ToString() == "Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon," ", 1, 1) == true)
                    {
                        cont = cont + 1;
                    }
                    else
                    {
                        errs = errs + 1;
                    }
                    if (EmailT(Email) == true)
                    {

                    }

                }
                else if (gvr.Cells[4].Text.ToString() == "No Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon, " ", 2,1) == true)
                    {
                        cont = cont + 1;
                    }
                    else
                    {
                        errs = errs + 1;
                    }
                }
            }

            if (cont >= 1)
            {
                string titulo, Msj, tipo;
                tipo = "success";
                Msj = " Se Completo el Proceso Exitosamente!!";
                titulo = "Aprobacion de Usuario";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else if (errs >= 1)
            {
                string titulo, Msj, tipo;
                tipo = "info";
                Msj = "Se Encontraron Errores al Ejecutar Procedimiento";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                string titulo, Msj, tipo;
                tipo = "info";
                Msj = "No se Ha Detectado Ningun Cambio";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch
        {

        }
    }

    protected bool Alta(string Email,string Razon,string Company,int Valor,int Opcion)
    {
        bool val = false;
        try
        {
            string Key = HttpContext.Current.Session["UserKey"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@ID",
                    Value = Razon
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Companys",
                    Value = Company
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserKey",
                    Value = Key
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Valor",
                    Value = Valor
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = Opcion
                });


                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                val = true;
                conn.Close();
            }
        }
        catch (Exception b)
        {
            string text = b.Message;
        }
        return val;
    }

    protected void btn_Enviar(object sender, EventArgs e)
    {
        try
        {
            if (GridView1.Rows.Count >= 1)
            {
                Aceptar();
                BindGridView();
                BindGridView2();
            }
            else
            {
                string tipo, Msj, titulo;
                tipo = "error";
                Msj = "No hay Datos Para Enviar";
                titulo = "Aprobacion de Usuarios";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

            }
            
        }
        catch
        {
            string tipo, Msj, titulo;
            tipo = "error";
            Msj = "Error al Ejecutar el Procedimiento de Guardado";
            titulo = "Aprobacion de Usuarios";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void btn_EnviarTsys(object sender, EventArgs e)
    {
        try
        {
            if (GridView2.Rows.Count >= 1)
            {
                AceptarTsys();
                BindGridView();
                BindGridView2();
            }
            else
            {
                string tipo, Msj, titulo;
                tipo = "error";
                Msj = "No hay Datos Para Enviar";
                titulo = "Aprobacion de Usuarios";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

            }
        }
        catch
        {
            string tipo, Msj, titulo;
            tipo = "error";
            Msj = "Error al Ejecutar el Procedimiento de Guardado";
            titulo = "Aprobacion de Usuarios";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected bool Registro(string CrtMail, string UserMaill, string Empresa)
    {
        bool Res = false;
        try
        {
            string Company = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@CreateMail", Value = CrtMail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserMail", Value = UserMaill });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 1 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                Res = true;
                conn.Close();
            }
        }
        catch
        {
            Res = false;
        }
        return Res;
    }

}