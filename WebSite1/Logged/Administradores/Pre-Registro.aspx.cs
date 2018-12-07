using Microsoft.AspNet.Identity;
using System;
using System.Web.UI;
using WebSite1;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using ClosedXML.Excel;
using System.Collections.Generic;

public partial class Pre_Registro : System.Web.UI.Page
{
    private readonly Page Me;

    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                {

                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        if (Cclientes.Items.Count == 0)
                        {
                            Cclientes.Visible = false;
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

    protected void VerificaCorreo(object sender, EventArgs e)
    {
        try
        {
            string Resulta = string.Empty;
            int err = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Resulta = (rdr["Resultado"].ToString());
                }
            }


            if (Resulta == "0")
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (Email.Text == gvr.Cells[4].Text.ToString())
                    {
                        err = err + 1;
                    }
                }
            }

            else
            {
                CMail.Text = "Este Correo ya Fue Registrado";
            }



            if (err >= 1)
            {

            }



        }
        catch
        {


        }
    }

    protected void Campos(object sender, EventArgs e)
    {
        string Caja = string.Empty;
        string VRazon = string.Empty;
        string VEmpresa = string.Empty;

        if (Cclientes.Visible == true)
        {
            VRazon = Cclientes.SelectedItem.ToString();
        }
        else
        {
            VRazon = Razon.Text;
        }

        if (VRazon == "" && IDSAGE.Text == "" && RFC.Text == "" && Email.Text == "" && VEmpresa == "")
        {
            Caja = "ATD";
        }
        else if (RevisaPro() == false)
        {
            Caja = "AID2";
        }
        else if (VRazon == "")
        {
            Caja = "AID";
        }
        else if (IDSAGE.Text == "")
        {
            Caja = "AID";
        }
        else if (RFC.Text == "")
        {
            Caja = "ARF2";
        }
        else if (RFC.Text != "" && RFC.Text.Length < 12)
        {
            Caja = "ARF1";
        }
        else if (Email.Text == "")
        {
            Caja = "AEM";
        }
        else if (Email.Text == "")
        {
            Caja = "AEM";
        }
        else if (EmpresaP.SelectedItem.ToString() == "")
        {
            Caja = "AEM";
        }

        if (Caja != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
        }
        else
        {
            VEmpresa = EmpresaP.SelectedItem.ToString();
            llenado();
        }


    }

    protected void Unnamed1_Clean(object sender, EventArgs e)
    {
        Razon.Text = "";
        IDSAGE.Text = "";
        RFC.Text = "";
        Email.Text = "";
        Razon.Visible = true;
        Cclientes.Visible = false;
        Cclientes.Items.Clear();
        EmpresaP.Items.Clear();
        Email.Focus();
        //EmailC(GridView2, 2);
    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {
            Razon.Text = "";
            RFC.Text = "";
            IDSAGE.Text = "";
            EmpresaP.Items.Clear();
            Razon.Visible = true;
            Cclientes.Visible = false;
            Cclientes.Items.Clear();

            if (Email_Ok(Email.Text) == true)
            {
                if (Email.Text != "")
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Email",
                            Value = Email.Text
                        });

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        DataTable Registros = new DataTable();
                        Registros.Load(rdr);
                        int Filas = Registros.Rows.Count;

                        if (Filas == 0)
                        {
                            conn.Close();
                            string Caja = "AID1";
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                        }
                        else if (Filas == 1)
                        {
                            DataRow Fila = Registros.Rows[0];
                            Razon.Visible = true;
                            Cclientes.Visible = false;
                            Razon.Text = Fila["Nombre"].ToString();
                            RFC.Text = Fila["RFC"].ToString();
                            IDSAGE.Text = Fila["VendID"].ToString();
                            EmpresaP.Items.Clear();
                            ListItem Lin = new ListItem();
                            Lin.Value = (Fila["Empresa"].ToString());
                            EmpresaP.Items.Insert(0, Lin);
                        }
                        else if (Filas >= 2)
                        {
                            foreach (DataRow row in Registros.Rows)
                            {
                                RFC.Text = "";
                                IDSAGE.Text = "";
                                ListItem Linea = new ListItem();
                                Linea.Value = (row["Nombre"].ToString());
                                Cclientes.Items.Insert(0, Linea);
                                RFC.Text = (row["RFC"].ToString());
                                IDSAGE.Text = (row["VendID"].ToString());
                                Razon.Visible = false;
                                Cclientes.Visible = true;
                                Empres();
                            }
                        }
                        conn.Close();
                    }
                }
                else
                {
                    string Caja = "AID";
                    CMail.Text = "Email No Encontrado, Verificalo con SAGE";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                }
            }
            else
            {
                string Caja = "AID";
                CMail.Text = "Formato de Email Invalido";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
            }
        }
        catch (Exception a)
        {
            string Eror = a.Message.ToString();
        }

    }

    protected void btn_Enviar(object sender, EventArgs e)
    {
        try
        {   
            string ID, Razon, RFC, Email, Empresa;
            int Contador = 0;
            int Total = GridView2.Rows.Count;
            if (GridView2.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                   
                    ID = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    Razon = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    RFC = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    Email = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    Empresa = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());

                    string key = User.Identity.Name.ToString();
                    string Rol = "Proveedor";


                    // Generate a new 12-character password with at least 1 non-alphanumeric character.
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    string password = Membership.GeneratePassword(12, 1);
                    password = CrearPassword(8);
                    string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);

                    //Generate User
                    var manager = new UserManager();
                    var newuser = new ApplicationUser() { UserName = Email };

                    IdentityResult result = manager.Create(newuser, password);
                    if (result.Succeeded)
                    {
                        manager.AddToRole(newuser.Id.ToString(), Rol);
                        string userid = newuser.Id.ToString();

                        if (Registro(ID, Razon, Email, hashedNewPassword, key, Empresa, Rol, userid) == true)
                        {
                            Contador = Contador + 1;
                        }
                    }
                    else
                    {
                        string userid = "UPDATE";
                        if (Registro(ID, Razon, Email, hashedNewPassword, key, Empresa, Rol, userid) == true)
                        {
                            Contador = Contador + 1;
                        }

                    }
                }

                if (Contador == Total)
                {
                    ListEm(GridView2, 2);
                    string titulo, Msj, tipo;
                    DataTable dt = new DataTable();
                    GridView2.DataSource = dt;
                    GridView2.DataBind();
                    tipo = "success";
                    Msj = "Registro Exitoso";
                    titulo = "T|SYS|";
                    ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
        }
        catch (Exception)
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = "Algunos Datos son Invalidos, Validalos con SAGE,En caso de persistir el problema comunicate con el Area de Sistemas ";
            titulo = "T|SYS| - Error de Ejecucion SP";
            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    public string CrearPassword(int longitud)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&#!@";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < longitud--)
        {
            res.Append(caracteres[rnd.Next(caracteres.Length)]);
        }
        return res.ToString();
    }

    protected bool Registro(string vID, string vRazon, string vEmail, string Pass, string vKey, string Empresa, string vRol, string vUserid)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@VendID", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Razon", Value = vRazon });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Empresa });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@KeyId", Value = vUserid });

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

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView2.PageIndex = e.NewPageIndex;
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

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            DataTable dt = new DataTable();

            dt.Columns.Add("ID SAGE");
            dt.Columns.Add("Razon Social");
            dt.Columns.Add("R.F.C.");
            dt.Columns.Add("Email");
            //dt.Columns.Add("Empresa");

            x = GridView2.Rows.Count;

            if (x == 1)
            {
                GridView2.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ID SAGE"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["R.F.C."] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
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

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
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

    protected void llenado() {

        DataTable dt = new DataTable();

        dt.Columns.Add("ID SAGE");
        dt.Columns.Add("Razon Social");
        dt.Columns.Add("R.F.C.");
        dt.Columns.Add("Email");
        dt.Columns.Add("Empresa");


        if (GridView2.Rows.Count >= 1)
        {
            foreach (GridViewRow gvr in GridView2.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["ID SAGE"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["R.F.C."] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                dt.Rows.Add(dr);
            }

        }
        string s = string.Empty;

        if (Razon.Visible == true && Razon.Text != "")
        {
            s = Razon.Text;
        }
        else if (Cclientes.Visible == true && Cclientes.SelectedItem.ToString() != "")
        {
            s = Cclientes.SelectedItem.ToString();
        }
        else
        {
            s = "";
        }

        DataRow row = dt.NewRow();
        row["ID SAGE"] = IDSAGE.Text;
        row["Razon Social"] = s;
        row["R.F.C."] = RFC.Text;
        row["Email"] = Email.Text;
        row["Empresa"] = EmpresaP.SelectedItem.ToString();

        dt.Rows.Add(row);
        GridView2.DataSource = dt;
        GridView2.DataBind();

        Razon.Text = "";
        IDSAGE.Text = "";
        RFC.Text = "";
        Email.Text = "";
        EmpresaP.Items.Clear();
        Cclientes.Items.Clear();
        Cclientes.Visible = false;
        Razon.Visible = true;
        Email.Focus();


    }

    protected void Empres()
    {
        try
        {
            if (Cclientes.SelectedItem.ToString() != "")
            {
                EmpresaP.Items.Clear();
                IDSAGE.Text = "";
                RFC.Text = "";
                string Razon = Cclientes.SelectedItem.ToString();
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetVendorsCompany", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Razon",
                        Value = Razon
                    });

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Email",
                        Value = Email.Text
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ListItem Lin = new ListItem();
                        Lin.Value = (rdr["Empresa"].ToString());
                        EmpresaP.Items.Insert(0, Lin);

                        RFC.Text = (rdr["RFC"].ToString());
                        IDSAGE.Text = (rdr["VendID"].ToString());
                    }
                }
            }
        }
        catch
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

    protected void Cclientes_SelectedIndexChanged(object sender, EventArgs e)
    {
        Empres();
    }

    protected void LlenadoTsys()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("Nombre");
        dt.Columns.Add("Correo");
        dt.Columns.Add("Rol");

        if (GridView1.Rows.Count >= 1)
        {
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dt.Rows.Add(dr);
            }

        }

        DataRow row = dt.NewRow();
        row["Nombre"] = NombreT.Text;
        row["Correo"] = EmailT.Text;
        row["Rol"] = UsersR.SelectedItem.ToString();

        dt.Rows.Add(row);
        GridView1.DataSource = dt;
        GridView1.DataBind();

        NombreT.Text = "";
        EmailT.Text = "";
        NombreT.Focus();
    }

    protected void Unnamed2_Click(object sender, EventArgs e)
    {
        string Caja = string.Empty;
        try
        {
            if (NombreT.Text == "" && EmailT.Text == "")
            {
                Caja = "ATT";
            }
            else if (NombreT.Text == "")
            {
                Caja = "AUT";
            }
            else if (Revisa() == false)
            {
                CMT.Text = "El Campo Email Ingresado Ya se Encuentra Registrado o ya está dentro de la Tabla, Verificalo.";
                Caja = "AET";
            }
            else if (EmailT.Text == "")
            {
                CMT.Text = "El Campo Email es Obligatorio.";
                Caja = "AET";
            }
            else if (Email_Ok(EmailT.Text) == false)
            {
                CMT.Text = "Formato de Email Invalido.";
                Caja = "AET";
            }

            if (Caja == "")
            {
                LlenadoTsys();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
            }
        }
        catch
        {

        }
    }

    protected bool Revisa()
    {
        bool rev = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortalT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = EmailT.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Company",
                    Value = ""
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = 2
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "0")
                    {
                        rev = true;
                    }
                }
                conn.Close();
            }

            if (GridView1.Rows.Count >=1)
            {
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                //if (gvr.Cells[2].Text.ToString() == EmailT.Text && gvr.Cells[4].Text.ToString() == EmpersasT.SelectedItem.ToString())
                if (gvr.Cells[2].Text.ToString() == EmailT.Text)
                {
                    rev = false;
                }
            }
          }
        }
        catch (Exception b)
        {
            rev = false;
        }

        return rev;
    }

    protected bool RevisaPro()
    {
        bool rev = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortalT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Company",
                    Value = EmpresaP.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = 1
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "0")
                    {
                        rev = true;
                    }
                }
                conn.Close();
            }

            if (GridView2.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.Cells[4].Text.ToString() == Email.Text && gvr.Cells[5].Text.ToString() == EmpresaP.SelectedItem.ToString())
                    {
                        rev = false;
                    }
                }
            }
        }
        catch (Exception b)
        {
            rev = false;
        }

        return rev;
    }

    protected void Unnamed2_Clean(object sender, EventArgs e)
    {
        NombreT.Text = "";
        EmailT.Text = "";
        NombreT.Focus();
        //EmailC(GridView1,1);
    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        GridView1.PageIndex = e.NewPageIndex;

        // Rebind the GridView control to  
        // show data in the new page. 
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

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Rol");
            //dt.Columns.Add("Empresa");

            x = GridView1.Rows.Count;

            if (x == 1)
            {
                GridView1.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
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

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {

        }
    }

    protected bool RegistroTsys(string vID,string vEmail, string Pass, string vKey, string vRol, string vUserid)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInUserTsys", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Nombre", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@Company", Value = Empresa });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@KeyId", Value = vUserid });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                DataTable Tbl = new DataTable();
                Tbl.Load(rdr);
                DataRow Fila = Tbl.Rows[0];

                if (Fila["Resultado"].ToString() == "")
                {
                    Res = true;
                }
                conn.Close();
            }
        }
        catch
        {
            Res = false;
        }
        return Res;
    }

    protected void btn_EnviarTsy(object sender, EventArgs e)
    {
        try
        {  
            string Nombre,Email,RolT;
            int Contador = 0;
            int Total = GridView1.Rows.Count;
                if (GridView1.Rows.Count >= 1)
                {
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                    Nombre = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    RolT = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //Empresa = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());

                    //string key = User.Identity.Name.ToString();
                    string key = HttpContext.Current.Session["UserKey"].ToString();

                    // Generate a new 12-character password with at least 1 non-alphanumeric character.
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    string password = CrearPassword(8);
                    string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);


                    //Generate User
                    var manager = new UserManager();
                        var newuser = new ApplicationUser() { UserName = Email };

                        IdentityResult result = manager.Create(newuser, password);
                        if (result.Succeeded)
                        {
                            manager.AddToRole(newuser.Id.ToString(), RolT);
                            string userid = newuser.Id.ToString();

                            if (RegistroTsys(Nombre, Email, password, key, RolT, userid) == true)
                            {
                                Contador = Contador + 1;
                            }
                        }
                        else
                        {
                            string userid = "UPDATE";
                            if (RegistroTsys(Nombre, Email, password, key, RolT, userid) == true)
                            {
                                Contador = Contador + 1;
                            }

                        }
                    }

                    if (Contador == Total)
                    {
                        ListEm(GridView1, 1);
                        string titulo, Msj, tipo;
                        DataTable dts = new DataTable();
                        GridView1.DataSource = dts;
                        GridView1.DataBind();
                        tipo = "success";
                        Msj = "Registro Exitoso";
                        titulo = "T|SYS|";
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                    else
                    {
                        string titulo, Msj, tipo;
                        tipo = "error";
                        Msj = "Algunos Datos son Invalidos, Verifica que los Datos Como Rol Asignado o Empresa Asiganda Esten Vigentes ";
                        titulo = "T|SYS| - Error de Ejecucion SP";

                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                }
        }
        catch (Exception)
        {
            
        }
    }

    protected void Empresa(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //Caja.Items.Add(rdr["Empresas"].ToString());
                    ListItem Lin = new ListItem();
                    Lin.Value = (rdr["Empresas"].ToString());
                    Caja.Items.Insert(0, Lin);
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {

        }

    }

    private bool EmailC(GridView Tabla,int Opcion,string Destinatario)
    {
        bool Resut = false;
        try
        {
         string PassNew = "<div ID='Tabla'><div class='panel panel-success'><div class='table-responsive'><table class='table table-striped' id='table-to-result-ass-indic'><tbody> ";

         foreach (GridViewRow gvr in Tabla.Rows)
         {
              if (Opcion == 2)
              {
               PassNew = PassNew + "<tr class='table-primary'><th>Clase : Proveedor     </th><td>  Email Registrado :  " + HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) +"</td></tr><br/>";
              }
              if (Opcion == 1)
              {
              PassNew = PassNew + "<tr class='table-primary'><th>Clase : Usuario Tsys     </th><td>     " + HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString()) + "</td>  Rol Asignado : " + HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) + "<td></td>  </tr><br/>";
              }
         }
          PassNew = PassNew + "</tbody></ table ></ div ></ div ></ div > ";
          string body = string.Empty;

          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionNO.html")))
          {
            body = reader.ReadToEnd();
            body = body.Replace("{PassTemp}", PassNew);
          }

          Resut = Global.EmailGlobal(Destinatario, body, "REGISTRO DE USUARIOS NUEVOS");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected bool ListEm(GridView Tabla, int Opcion)
    {
        bool uno;
        uno = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spAddMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = "xx" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Usuario Nuevo" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Desti = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());

                    if (Desti != "" && Desti != "Usuario")
                    {
                        EmailC(Tabla, Opcion,Desti);
                    } 
                }
            }
        }
        catch
        {

        }
        return uno;
    }

    protected bool RegistroEx(string vID, string vRazon, string vEmail, string Pass, string vKey, string Empresa, string vRol, string vUserid)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spExcel", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@Opcion", Value = 1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@VendID", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Razon", Value = vRazon });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Empresa });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@KeyId", Value = vUserid });

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


    protected void Excel(object sender, EventArgs e)
    {

        try
        { 

        if (FileUpload1.HasFile)
        {

            DataTable dts = new DataTable();
            dts.Columns.Add("No");
            dts.Columns.Add("ID");
            dts.Columns.Add("Nombre");
            dts.Columns.Add("Company");
            dts.Columns.Add("Email");
            dts.Columns.Add("Notif");
            dts.Columns.Add("Anot");

            using (XLWorkbook workBook = new XLWorkbook(FileUpload1.PostedFile.InputStream))
            {
                var workbook = new XLWorkbook(FileUpload1.PostedFile.InputStream);
                IXLWorksheet workSheet = workBook.Worksheet(1);
                DataTable dt = new DataTable();

                bool firstRow = true;
                string Col = string.Empty;
                int Cont = 0;
                foreach (IXLRow row in workSheet.Rows())
                {
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }

                        if (dt.Columns[0].ColumnName != "PrimaryCntctKey") { Col = "Celda A1 debe ser columna PrimaryCntctKey"; }
                        else if (dt.Columns[1].ColumnName != "VendID") { Col = "Celda A2 debe ser columna VendID"; }
                        else if (dt.Columns[2].ColumnName != "CompanyID") { Col = "Celda A3 debe ser columna CompanyID"; }
                        else if (dt.Columns[3].ColumnName != "VendName") { Col = "Celda A4 debe ser columna VendName"; }
                        else if (dt.Columns[4].ColumnName != "Correo") { Col = "Celda A5 debe ser columna Correo"; }

                        if (Col != "")
                        {
                            dt = null;
                            string titulo, Msj, tipo;
                            tipo = "error";
                            Msj = Col;
                            titulo = "T|SYS| - Error de Lectura Excel";
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                            return;
                        }
                        else
                        {
                            firstRow = false;
                        }
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        Cont = Cont + 1;
                        int Fila = 0;
                        DataRow rowt = dts.NewRow();


                        foreach (IXLCell cell in row.Cells())
                        {
                            Fila = dt.Rows.Count - 1;
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }

                        string Primary = dt.Rows[Fila][0].ToString();
                        string VendID = dt.Rows[Fila][1].ToString();
                        string Company = dt.Rows[Fila][2].ToString();
                        string VendName = dt.Rows[Fila][3].ToString();
                        string Correo = dt.Rows[Fila][4].ToString();

                        if (Correo != "")
                        {
                            if (Email_Ok(Correo) == true)
                            {
                                ApplicationDbContext context = new ApplicationDbContext();
                                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                                string password = Membership.GeneratePassword(12, 1);
                                password = CrearPassword(8);
                                string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);
                                string key = User.Identity.Name.ToString();
                                string Rol = "Proveedor";

                                var manager = new UserManager();
                                var newuser = new ApplicationUser() { UserName = Correo };

                                IdentityResult result = manager.Create(newuser, password);
                                if (result.Succeeded)
                                {
                                    manager.AddToRole(newuser.Id.ToString(), Rol);
                                    string userid = newuser.Id.ToString();

                                    if (RegistroEx(VendID, VendName, Correo, hashedNewPassword, key, Company, Rol, userid) == true)
                                    {
                                        string SenM = Emailexc(Correo, Company);
                                        string Not = "OK";
                                        string Anot = "Registro exitoso";

                                        if (SenM != "Ok") { Not = "X"; Anot = "Registro exitoso, error al enviar Correo"; }
                                        rowt["No"] = Cont;
                                        rowt["ID"] = VendID;
                                        rowt["Nombre"] = VendName;
                                        rowt["Company"] = Company;
                                        rowt["Email"] = Correo;
                                        rowt["Notif"] = Not;
                                        rowt["Anot"] = Anot;
                                        dts.Rows.Add(rowt);
                                    }
                                }
                                else
                                {

                                    rowt["No"] = Cont;
                                    rowt["ID"] = VendID;
                                    rowt["Nombre"] = VendName;
                                    rowt["Company"] = Company;
                                    rowt["Email"] = Correo;
                                    rowt["Notif"] = "X";
                                    rowt["Anot"] = "Error al registrar , usuario ya registrado";
                                    dts.Rows.Add(rowt);
                                }
                            }
                            else
                            {
                                rowt["No"] = Cont;
                                rowt["ID"] = VendID;
                                rowt["Nombre"] = VendName;
                                rowt["Company"] = Company;
                                rowt["Email"] = Correo;
                                rowt["Notif"] = "X";
                                rowt["Anot"] = "Formato de Email Inocrrecto";
                                dts.Rows.Add(rowt);
                            }
                        }


                    }  // Cierra Else
                }
            }

            if (dts.Rows.Count >= 1)
            {
                GridView3.DataSource = dts;
                GridView3.DataBind();

            }
        }
        else
        {
                GridView3.DataSource = "";
                GridView3.DataBind();
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Selecciona un archivo para iniciar la Carga ";
                titulo = "T|SYS| - Error de Ejecucion SP";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }

    }
        catch (Exception ex)
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = "Algunos datos son Invalidos, verificalos con SAGE,en caso de persistir el problema comúnicate con el Area de Sistemas ";
            titulo = "T|SYS| - Error de Ejecucion SP";
            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
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

    private string Emailexc(string CorreoUs, string Company)
    {
        string Resut = "Error al enviar correo de confirmación a " + CorreoUs;
        try
        {
          string PassNew = Membership.GeneratePassword(8, 1);
          string PassHAs = PassNew.ToString();
          ApplicationDbContext context = new ApplicationDbContext();
          UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
          UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
          string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
          bool UpdatePass = UpdatePAss(CorreoUs, hashedNewPassword);
          bool send = false;

          string body = string.Empty;
          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOkUser.html")))
          {
             body = reader.ReadToEnd();
             body = body.Replace("{UserTemp}", CorreoUs);
             body = body.Replace("{PassTemp}", PassHAs);
           }

            send = Global.EmailGlobal(CorreoUs, body, "BIENVENIDO PORTAL T|SYS|");
            if (send == true) { Resut = "Ok"; }
            else
            {
             int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
             int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
             LogError(pLogKey, pUserKey, "Envio de Correo de Registro de Usuario Excel" , Resut, Company);
           }
        }
        catch (Exception b)
        {
            Resut = b.Message;
        }
        return Resut;
    }

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
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

        }
    }

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

}