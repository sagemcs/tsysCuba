using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Carga_Doc : System.Web.UI.Page
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
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    if (UsuarioP.Items.Count == 0)
                    {
                        User_Empresas(UsuarioP);
                        Comprueba();
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


            Global.Docs();
        if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
        {
            Page.MasterPageFile = "MenuP.master";
        }
        else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
        {
                if (HttpContext.Current.Session["UpDoc"].ToString() == "1") { Page.MasterPageFile = "MenuP.master"; }
                else {  Page.MasterPageFile = "MenuPreP.master"; }
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

    protected void Upload()
    {
        string titulo, Msj, tipo, Guardar, Company, Prove;
        Msj = string.Empty;
        Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
        string Vedn = HttpContext.Current.Session["VendKey"].ToString();
        try
        {
            int Contt = 0;
            Company = UsuarioP.SelectedItem.ToString();
            Prove = User.Identity.Name.ToString();

            Guardar = Execute("5", FileUpload1, Company, Prove,"").ToString();  //Acta Constitutiva   5
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar ACTA CONSTITUTIVA , Verifica el Archivo";
            }
            Guardar = Execute("6", FileUpload2,Company, Prove,"").ToString(); //RFC   6
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar REGISTRO FEDEREAL DEL CONTRIBUYENTE, Verifica el Archivo";
            }
            Guardar = Execute("7", FileUpload3, Company, Prove,"").ToString(); //Poder Notarial  7
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar PODER NOTARIAL , Verifica el Archivo";
            }
            Guardar = Execute("8", FileUpload4, Company, Prove,"").ToString(); //IFE 8
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar IDENTIFICACIÓN OFICIAL , Verifica el Archivo";
            }
            Guardar = Execute("9", FileUpload5, Company, Prove,"").ToString(); //Obligaciones Fiscales   9
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar OPINIÓN DE OBLIGACIONES FÍSCALES , Verifica el Archivo";
            }
            Guardar = Execute("10", FileUpload6, Company, Prove,"").ToString(); //Carta Instruccion      10
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar CARTA INSTRUCCIÓN , Verifica el Archivo";
            }
            Guardar = Execute("11", FileUpload7, Company, Prove,"").ToString(); //Contrato       11
            if (Guardar != "1")
            {
                Contt = Contt + 1;
                Msj = "Error al cargar CONTRATO Y/O CARTA DE PRESTACIÓN , Verifica el Archivo";
            }

            if (Contt >= 1)
            {
                tipo = "error";
                titulo = "Error Al Cargar Documentos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                ListEm();
                tipo = "success";
                Msj = "Has completado la carga de documentos exitosamente, en cuanto el administrador los apruebe, se te notificará por correo electrónico para seguir con el proceso de registro, Gracias";
                titulo = "Carga de Documentos Exitosa";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch
        {

        }
    }

    public string Execute(string KeyFile,FileUpload Caja,string Company,string Prove,string Desc)
    {
        string res;
        
        try
        {
            string Filename = Caja.FileName.ToString();

            //Fabian
            Stream fsr = Caja.PostedFile.InputStream;

            System.IO.BinaryReader br3 = new System.IO.BinaryReader(fsr);
            Byte[] bytes3 = br3.ReadBytes((Int32)fsr.Length);

            string Ext = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Cades = "spInsertDoc";
                string Result = string.Empty;
                SqlCommand cmd = new SqlCommand(Cades, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@FKey", Value = KeyFile });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File", Value = bytes3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@FileNAme", Value = Filename });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@EmpresaC", Value = HttpContext.Current.Session["IDComTran"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Vendedor", Value = HttpContext.Current.Session["VendKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = HttpContext.Current.Session["UserKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext", Value = Ext });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Desc", Value = Desc });


                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "1")
                    {
                        Result = rdr["Resultado"].ToString();
                    }
                    else
                    {
                        Result = rdr["Resultado"].ToString();
                        throw new Exception(Result);
                    }
                }
                res = Result;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return res;
        }
        catch(Exception Rt)
        {
            res = Rt.Message;
            return res;
        }  
    }

    protected void Revisa()
    {
        try
        {
            string VB = ID.Text;
            string Cadena = string.Empty;

            string Flw = FileUpload1.FileName.ToString();
            
            if (FileUpload1.FileName.ToString() == "" || FileUpload2.FileName.ToString() == "" || FileUpload3.FileName.ToString() == "" || FileUpload4.FileName.ToString() == "" || FileUpload5.FileName.ToString() == "" || FileUpload6.FileName.ToString() == "" || FileUpload7.FileName.ToString() == "")
            {
              Cadena = "B1";
            }
            else if (FileUpload1.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B2";
            }
            else if (FileUpload2.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B3";
            }
            else if (FileUpload3.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B4";
            }
            else if (FileUpload4.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B5";
            }
            else if (FileUpload5.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B6";
            }
            else if (FileUpload6.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B7";
            }
            else if (FileUpload7.PostedFile.ContentLength > 1000000 * 15)
            {
                Cadena = "B8";
            }

        if (Cadena != "")
        {
           ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(" + Cadena + ");", true);
        }
        else
        {
          Upload();
         }
        }
        catch
        {
            //lblError.Text = "Database connection error. Please try again.";
        }

    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {
            Revisa();
            Comprueba();
        }
        catch
        {
            //lblError.Text = "Database connection error. Please try again.";
        }

    }

    protected void Variables()
    {
        try
        { 
            if (UsuarioP.SelectedItem.ToString() != "")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetvar", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@EmpresaV",
                        Value = UsuarioP.SelectedItem.ToString()
                    });

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@User",
                        Value = HttpContext.Current.Session["UserKey"].ToString()
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    string Prueba = HttpContext.Current.Session["UserKey"].ToString();
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    System.Data.DataTable GV = new System.Data.DataTable();
                    GV.Load(rdr);

                    if (GV.Rows.Count == 1)
                    {
                        DataRow Fila = GV.Rows[0];
                        HttpContext.Current.Session["VendKey"] = Fila["VendKey"].ToString();
                        HttpContext.Current.Session["IDComTran"] = Fila["Company"].ToString();
                    }
                    else
                    {
                        HttpContext.Current.Session["VendKey"] = "";
                        HttpContext.Current.Session["IDComTran"] = "";
                    }
                    conn.Close();
                }

            }
        }
        catch
        {

        }
    }

    protected void User_Empresas(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            string Userkey = User.Identity.Name.ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompanyLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = Userkey });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Empresas"].ToString() != "")
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Empresas"].ToString());
                        Caja.Items.Insert(0, Linea);
                        Caja.Visible = true;
                        Variables();
                    }
                    else
                    {
                        throw new Exception(Errores);
                    }

                    
                }
                if (Caja.Items.Count == 0)
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = ("N/D");
                    Caja.Items.Insert(0, Linea);
                    Caja.Visible = true;
                    HttpContext.Current.Session["IDComTran"] = "";
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {
            Caja.Items.Clear();
        }
    }

    protected void Comprueba()
    {
        try
        {
            if (UsuarioP.SelectedItem.ToString() != "")
            {
                Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
                int Cot = 0;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spComprDoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@User", Value = HttpContext.Current.Session["VendKey"].ToString() });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Company", Value = HttpContext.Current.Session["IDCompany"].ToString() });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    string Errores = string.Empty;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt.Columns.Add("Nombre");
                    dt.Columns.Add("NombreFile");
                    dt.Columns.Add("Fecha");
                    dt.Columns.Add("Fecha2");
                    dt.Columns.Add("Status");
                    //dt.Columns.Add("Coment");

                        while (rdr.Read())
                        {

                            DataRow row = dt.NewRow();
                            row["Nombre"] = rdr["Nombre"].ToString();
                            row["NombreFile"] = rdr["NombreFile"].ToString();
                            row["Fecha"] = rdr["Fecha"].ToString();
                            if (rdr["Fecha2"].ToString() == null)
                            {
                                row["Fecha2"] = "No Disponible";
                            }
                            else
                            {
                                row["Fecha2"] = rdr["Fecha2"].ToString();
                            }
                            row["Status"] = rdr["status"].ToString();
                            if (rdr["status"].ToString()== "Aprobado") { Cot = Cot + 1; }
                            if (rdr["status"].ToString() == "En Revisión") { Cot = 0; }
                        dt.Rows.Add(row);
                        }

                    if (dt.Rows.Count >= 1)
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        Carga.Visible = false;
                        Info.Visible = true;
                    }
                    else
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        Carga.Visible = true;
                        Info.Visible = false;
                    }

                    string Var = GridView1.Rows[0].Cells[4].Text.ToString();

                    if (Cot >= 1)
                    {
                        foreach (GridViewRow gvr in GridView1.Rows)
                        {
                            FileUpload Carga = (FileUpload)gvr.Cells[5].FindControl("BDoc");
                            System.Web.UI.WebControls.TextBox Caja = (System.Web.UI.WebControls.TextBox)gvr.Cells[5].FindControl("CDoc");

                            if (gvr.Cells[4].Text.ToString() == "Aprobado")
                            {
                                Caja.Visible = false;
                                Carga.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        btnEnv.Visible = false;
                        GridView1.Columns[5].Visible = false;
                    }

                }
            }
        }
        catch(Exception ex)
        {

        }

    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            String archivo;

            GridViewRow row = GridView1.Rows[index];
            archivo = HttpUtility.HtmlDecode(row.Cells[1].Text);


             if (e.CommandName == "Documento_1")
            {
                //FileUpload Caja = new FileUpload();
                //Caja.ID = "CajaV";
                //Page.Controls.Add(Caja);
                
                //InsertSage(row.Cells[0].Text, row.Cells[1].Text);
            }

        }
        catch (Exception ex)
        {
            
        }
    }

    protected void Nuevos(object sender,EventArgs e)
    {
        try
        {
            string Msj = string.Empty;
            string titulo, tipo;
            int Cont = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                if (gvr.Cells[4].Text.ToString() == "Pendiente")
                {
                    Cont = Cont + 1;
                    FileUpload Carga = (FileUpload)gvr.Cells[5].FindControl("BDoc");
                    string Archivo = gvr.Cells[0].Text.ToString();
                    string var = Carga.FileName.ToString();
                    int tam_var = var.Length;
                    string Var_Sub = var.Substring((tam_var - 3), 3);

                    if (Var_Sub == "pdf")
                    {
                        if (Carga.FileName.ToString() != "")
                        {

                            string Guardar = string.Empty;
                            
                            string Company = UsuarioP.SelectedItem.ToString();
                            string Prove = User.Identity.Name.ToString();

                            Guardar = Execute("7", Carga, Company, Prove,Archivo).ToString(); //Poder Notarial  7
                            if (Guardar != "1")
                            {
                                Msj = "Error al cargar " + Archivo + " , Verifica el Archivo";
                            }
                        }
                    }
                    else
                    {
                        Msj = "Error al cargar " + Archivo + " , No es Archivo '.pdf'";
                    }

                }

            }

            if (Cont >= 1)
            {
                if (Msj != "")
                    {
                        tipo = "error";
                        titulo = "Error Al Cargar Documentos";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                else
                    { 
                    ListEm();
                    tipo = "success";
                    Msj = "Has completado la carga de documentos exitosamente, en cuanto el administrador los apruebe, se te notificará por correo electrónico para seguir con el proceso de registro, Gracias";
                    titulo = "Carga de Documentos Exitosa";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    Comprueba();
                }
            }

        }
        catch
        {

        }
    }


    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
        //    string HyperLinkValue = e.Row.Cells[1].Text;

        //    string Doc = e.Row.Cells[0].Text;

        //    HyperLink myLink = new HyperLink();
        //    string Location = ResolveUrl("~/ViewPdfPage.aspx") + "?ColumName=" + HyperLinkValue;
        //    myLink.Text = HyperLinkValue;
        //    e.Row.Style["cursor"] = "pointer";
        //    e.Row.Attributes["onClick"] = string.Format("javascript:window.open('" + HyperLinkValue + "', '_blank');", Location);
        //    e.Row.Attributes.Add("onclick", ClientScript.RegisterStartupScript(GetType(), "ramdomtext", "alertme('Hola');"));
        //    Doc = "{name: }";
        //    e.Row.Attributes.Add("OnClientClick", "Web()");
        //    e.Row.Cells[1].Controls.Add(myLink);
        }
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void UsuarioP_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
            Comprueba();
        }
        catch
        {

        }
    }

    private bool EmailC(int Opcion, string Destinatario)
    {
        bool Resut = false;
        try
        {
                string PassNew = "<div ID='Tabla'><div class='panel panel-success'><div class='table-responsive'><table class='table table-striped' id='table-to-result-ass-indic'><tbody> ";
                if (Opcion == 2)
                {
                    PassNew = PassNew + "<tr class='table-primary'><th><strong>Proveedor :  </strong>     </th><td> " + User.Identity.Name.ToString() + "</td></tr><br/>";
                }

                PassNew = PassNew + "</tbody></ table ></ div ></ div ></ div > ";

                string body = string.Empty;

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionDocs.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }
            Global.EmailGlobal(Destinatario, body, "CARGA DE DOCUMENTOS");  
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected bool ListEm()
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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Carga de Documentos" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Desti = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());

                    if (Desti != "" && Desti != "Usuario")
                    {
                        EmailC(2, Desti);
                    }
                }
            }
        }
        catch
        {

        }
        return uno;
    }




}