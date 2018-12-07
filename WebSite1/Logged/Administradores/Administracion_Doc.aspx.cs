using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.UI;
using System.Collections.Generic;

public partial class Logged_Administrar : System.Web.UI.Page
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
                       CargaProv();
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
    string eventName = String.Empty;
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

    protected void BindGridInvoices(string Id, string EMail)
    {
        string sSQL = "";

        SqlConnection sqlConnection1 = new SqlConnection();
        sqlConnection1 = SqlConnectionDB("PortalConnection");
        sqlConnection1.Open();

        using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
        {
            try
            {
                sSQL = "spRevDoc";

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@ID", Id));
                parsT.Add(new SqlParameter("@Email", EMail));

                SqlParameter[] sqlParameter = parsT.ToArray();
                foreach (SqlParameter par in sqlParameter)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                SqlDataAdapter Data = new SqlDataAdapter(Cmd);
                DataTable Table = new DataTable();

                Data.Fill(Table);
                gvFacturas.DataSource = Table;
                gvFacturas.DataBind();


            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar procedimiento almacenado, Error: " + ex.ToString());
            }
            finally
            {
                if (sqlConnection1.State == ConnectionState.Open)
                {
                    sqlConnection1.Close();
                }
            }
        }


    }

    protected void Comprueba(string Id,string EMail)
    {
        try
        {
            if (IDSAGE.Text != "" && Mail.Text != "")
            {
                //Global.VarSesion(User.Identity.Name.ToString(), SelProv.SelectedItem.ToString());

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spRevDoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ID", Value = Id});

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Email", Value = EMail});

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    string Errores = string.Empty;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Nombre");
                    dt.Columns.Add("NombreFile");
                    dt.Columns.Add("Fecha");
                    dt.Columns.Add("Status");
                    dt.Columns.Add("Coment");

                    while (rdr.Read())
                    {

                        DataRow row = dt.NewRow();
                        row["Nombre"] = HttpUtility.HtmlDecode(rdr["Nombre"].ToString());
                        row["NombreFile"] = HttpUtility.HtmlDecode(rdr["NombreFile"].ToString());
                        row["Fecha"] = rdr["Fecha"].ToString();
                        row["Status"] = rdr["status"].ToString();
                        row["Coment"] = HttpUtility.HtmlDecode(rdr["Comentarios"].ToString());
                        dt.Rows.Add(row);
                    }

                    if (dt.Rows.Count >= 5)
                    {
                        gvFacturas.DataSource = dt;
                        gvFacturas.DataBind();
                    }
                    else
                    {
                        gvFacturas.DataSource = dt;
                        gvFacturas.DataBind();
                    }

                }
            }
        }
        catch
        {

        }

    }

    protected void CargaProv()
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = "" });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SelProv.Items.Clear();
                SelComp.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length  >1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        SelProv.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }
                    
                }
                
                conn.Close();
            }

            if (Cuenta == 0)
            {
                gvFacturas.DataSource = dt;
                gvFacturas.DataBind();
                Button1.Visible = false;
                Button2.Visible = false;
                Button3.Visible = false;
                Comentarios.Visible = false;
                lc.Visible = false;
                DatosV.Visible = true;
                Mail.Text = "";
                Comentarios.Text = "";
                IDSAGE.Text = "";
            }
            else
            {
                DatosV.Visible = false;
                Comentarios.Text = "";
                lc.Visible = true;
                Button1.Visible = true;
                Button2.Visible = true;
                Button3.Visible = true;
                Comentarios.Visible = true;

                Datos(SelProv.SelectedItem.ToString());

                foreach (GridViewRow gvr in gvFacturas.Rows)
                {
                    CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                    string  status = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    if (status == "Aprobado")
                    {
                        Checka.Checked = true;
                        Checka.Enabled = false;
                    }
                }


            } 
        }
        catch (Exception b)
        {

        }
    }

    protected void Datos(string ID)
    {

        try
        {
            int Contador = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 2 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = ID });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SelComp.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = (rdr["Fecha"].ToString());
                    SelComp.Items.Insert(0, Linea);
                    IDSAGE.Text = rdr["ID"].ToString();
                    Mail.Text = rdr["Email"].ToString();
                    Contador = Contador + 1;
                }

                if (Contador >= 1)
                {
                 BindGridInvoices(IDSAGE.Text, Mail.Text);
                }
            }
        }
        catch
        {

        }
 }

    protected void SelProv_SelectedIndexChanged1(object sender, EventArgs e)
    {
        try
        {
            if (IDSAGE.Text != "" && Mail.Text != "")
            {
                string Prov = SelProv.SelectedItem.ToString();
                Datos(SelProv.SelectedItem.ToString());
                foreach (GridViewRow gvr in gvFacturas.Rows)
                {
                    CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                    string status = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    if (status == "Aprobado")
                    {
                        Checka.Checked = true;
                        Checka.Enabled = false;
                    }
                }
            }
        }
        catch
        {

        }
    }

    protected void Aceptar(object sender, EventArgs e)
    {
        try
        {
            int cont = 0;
            int i =0;
            int x = 0;
            string docs = string.Empty;
            string Prov = SelProv.SelectedItem.ToString();
            string Com = SelComp.SelectedItem.ToString();

            foreach (GridViewRow gvr in gvFacturas.Rows)
            {
                CheckBox Checka = (CheckBox)gvFacturas.Rows[i].Cells[1].FindControl("Check");
                docs = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());

                if (Checka.Checked)
                {
                    if (Alta(2, docs,Prov,Com) == false)
                    {
                        cont = cont + 1;
                    }
                }
                else
                {
                    if (Alta(1, docs, Prov, Com) == false)
                    {
                        cont = cont + 1;
                    }
                    x = x + 1;
                }
                i = i + 1;
            }

            if (cont >= 1)
            {
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Se Generaron Errores al Guardar Cambios en Base de Datos";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                CargaProv();
            }
            else
            {
                Email("Aprobado");
                string titulo, Msj, tipo;
                tipo = "success";
                Msj = "Notificacion de Proveedor Exitosa";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                CargaProv();
            }

        }
        catch
        {

        }
    }

    protected void Del()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDocD", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@ID", Value = IDSAGE.Text });
                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@Name", Value = SelProv.SelectedItem.ToString()});
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = SelComp.SelectedItem.ToString()});

                if (conn.State == ConnectionState.Open) {conn.Close();}

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                conn.Close();
            }
        }
        catch (Exception b)
        {

        }

    }


    protected bool Alta(int status,string Doc,string Prov,string Com)
    {
        bool alt = false;
        try
        {
            string Key = HttpContext.Current.Session["UserKey"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@ID",
                    Value = IDSAGE.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Name",
                    Value = Prov
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserKey",
                    Value = Key
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Status",
                    Value = status
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Doc",
                    Value = Doc
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Compan",
                    Value = Com
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                alt = true;
                conn.Close();
            }
        }
        catch (Exception b)
        {
            alt = false;
        }
        return alt;
    }

    private bool Email(string PassNew)
    {
        bool Resut = false;
        try
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOk.html")))
            {
                body = reader.ReadToEnd();
                string OK = "='https://www.multiconsulting.com.mx/Ok.png'";
                string NO = "='https://www.multiconsulting.com.mx/X.png'";
                string Coment = Comentarios.Text;

                CheckBox Checka1 = (CheckBox)gvFacturas.Rows[0].Cells[1].FindControl("Check");
                CheckBox Checka2 = (CheckBox)gvFacturas.Rows[1].Cells[1].FindControl("Check");
                CheckBox Checka3 = (CheckBox)gvFacturas.Rows[2].Cells[1].FindControl("Check");
                CheckBox Checka4 = (CheckBox)gvFacturas.Rows[3].Cells[1].FindControl("Check");
                CheckBox Checka5 = (CheckBox)gvFacturas.Rows[4].Cells[1].FindControl("Check");
                CheckBox Checka6 = (CheckBox)gvFacturas.Rows[5].Cells[1].FindControl("Check");
                CheckBox Checka7 = (CheckBox)gvFacturas.Rows[6].Cells[1].FindControl("Check");

                if (Checka1.Checked == true) { body = body.Replace("{Status1}", "Aprobado"); body = body.Replace("{PassTemp1}", OK); } else { body = body.Replace("{Status1}", "Rechazado"); body = body.Replace("{PassTemp1}", NO); }
                if (Checka2.Checked == true) { body = body.Replace("{Status2}", "Aprobado"); body = body.Replace("{PassTemp2}", OK); } else { body = body.Replace("{Status2}", "Rechazado"); body = body.Replace("{PassTemp2}", NO); }
                if (Checka3.Checked == true) { body = body.Replace("{Status3}", "Aprobado"); body = body.Replace("{PassTemp3}", OK); } else { body = body.Replace("{Status3}", "Rechazado"); body = body.Replace("{PassTemp3}", NO); }
                if (Checka4.Checked == true) { body = body.Replace("{Status4}", "Aprobado"); body = body.Replace("{PassTemp4}", OK); } else { body = body.Replace("{Status4}", "Rechazado"); body = body.Replace("{PassTemp4}", NO); }
                if (Checka5.Checked == true) { body = body.Replace("{Status5}", "Aprobado"); body = body.Replace("{PassTemp5}", OK); } else { body = body.Replace("{Status5}", "Rechazado"); body = body.Replace("{PassTemp5}", NO); }
                if (Checka6.Checked == true) { body = body.Replace("{Status6}", "Aprobado"); body = body.Replace("{PassTemp6}", OK); } else { body = body.Replace("{Status6}", "Rechazado"); body = body.Replace("{PassTemp6}", NO); }
                if (Checka7.Checked == true) { body = body.Replace("{Status7}", "Aprobado"); body = body.Replace("{PassTemp7}", OK); } else { body = body.Replace("{Status7}", "Rechazado"); body = body.Replace("{PassTemp7}", NO); }

                 body = body.Replace("{Comentarios}", Coment);

            }

            Resut = Global.EmailGlobal(Mail.Text, body, "CARGA DE DOCUMENTOS PORTAL TSYS");
            
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //int i = e.Row.RowIndex;
                //Image imagen = (Image)gvFacturas.Rows[i].Cells[7].FindControl("imgestado");
                //string EstadoImg = HttpUtility.HtmlDecode(gvFacturas.Rows[i].Cells[2].Text.ToString());

                //if (EstadoImg == "Aprobado")
                //{
                //    imagen.ImageUrl = "~/Img/Ok.png";
                //}
                //else
                //{
                //    imagen.ImageUrl = "~/Img/Red.png";
                //}
            }
    }

    protected void OkAll(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvFacturas.Rows)
        {
            CheckBox check = row.FindControl("Check") as CheckBox;

            if (check.Checked != true)
            {
                check.Checked = true;
            }
        }

    }

    protected void NoAll(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvFacturas.Rows)
        {
            CheckBox check = row.FindControl("Check") as CheckBox;
            if (check.Enabled == true)
            { 
            if (check.Checked == true)
            {
                check.Checked = false;
            }
            }
        }
        //Descargar();
    }

    protected void Check(int index)
    {
        CheckBox Checka = (CheckBox)gvFacturas.Rows[index].Cells[1].FindControl("Check");
        DataTable dt = new DataTable();

        dt.Columns.Add("Status");
        dt.Columns.Add("Nombre");
        dt.Columns.Add("Archivo");
        dt.Columns.Add("Fecha");

        foreach (GridViewRow gvr in gvFacturas.Rows)
        {
            if (Checka.Checked == true)
            {
                DataRow dr = dt.NewRow();        
                dr["Status"] = "Aprobado";
                dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["Archivo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dr["Fecha"] = gvr.Cells[4].Text.ToString();
                dt.Rows.Add(dr);
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr["Status"] = "No Aprobado";
                dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["Archivo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dr["Fecha"] = gvr.Cells[4].Text.ToString();
                dt.Rows.Add(dr);

            }
        }

        gvFacturas.DataSource = dt;
        gvFacturas.DataBind();

    }

    protected void Descargar()
    {
        try
        {
            //MemoryStream memoryStream = new MemoryStream();
            //ZipFile ElZip = new ZipFile();

            //int index = 1;

            //GridViewRow row = gvFacturas.Rows[index];
     

            //    using (ZipFile Comp = new ZipFile())
            //    {
            //        foreach (GridViewRow gvr in gvFacturas.Rows)
            //        {
            //        string Nombre = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
            //        memoryStream = databaseFileRead("InvoiceKey", gvr.Cells[0].Text, "DocFile");
            //        Comp.AddEntry(Nombre, memoryStream.ToArray());
            //        }

            //    string Descarga = "MultiConsulting.zip";
            //    using (MemoryStream Out = new MemoryStream())
            //    {
            //        Comp.Save(Out);
            //        //return File(Out.ToArray(), "application/zip", Descarga);
            //    }

            //}
            }
        catch
        {

        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            string archivo,ID,Razon,Company,Email2,IDSAG;

            GridViewRow row = gvFacturas.Rows[index];
            ID = HttpUtility.HtmlDecode(row.Cells[3].Text.ToString()); // Archivo
            Razon = HttpUtility.HtmlDecode(SelProv.SelectedItem.ToString()); //Razon
            Company = HttpUtility.HtmlDecode(SelComp.SelectedItem.ToString());
            Email2 = HttpUtility.HtmlDecode(Mail.Text);
            IDSAG = HttpUtility.HtmlDecode(IDSAGE.Text);

            archivo = ID + " - " + Razon;

            if (e.CommandName == "Documento_1")
            {
                if (e.CommandName == "Documento_1")
                {
                    memoryStream = databaseFileRead(IDSAG,Razon,ID,Email2,Company);
                }

                archivo = archivo.Replace(".", "").Replace(",","");
                archivo = archivo + ".pdf";
                Response.ContentType = "text/plain";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + archivo);
                Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();

            }
            else if (e.CommandName == "Check")
            {
              
            }

        }
        catch (Exception ex)
        {
            string respuesta = ex.Message;
            int pLogKey = 1;
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
            LogError(pLogKey, pUserKey, "Aprobacion de Documentos:GridView1_RowCommand", ex.Message, pCompanyID);
        }
    }

    protected void chkview_CheckedChanged(object sender, EventArgs e)
    {
        IDSAGE.Text = "";
        //Define your code here.
    }


    private MemoryStream databaseFileRead(string ID, string Razon, string Doc,string Email,string Company)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("DowFil", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@ID",Value = ID});
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Name",Value = Razon});
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Doc", Value = Doc });
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Email", Value = Email });
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@compan", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var blob = new Byte[(rdr.GetBytes(0, 0, null, 0, int.MaxValue))];
                    rdr.GetBytes(0, 0, blob, 0, blob.Length);
                    memoryStream.Write(blob, 0, blob.Length);
                }
            }
            return memoryStream;
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;

            return null;
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
            //Label4.Text = err;
        }
    }

    
}