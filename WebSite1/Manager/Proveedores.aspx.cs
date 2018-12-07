using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Enable the GridView paging option and  
            // specify the page size. 

            string name = Profile.UserName;

            Session["Proveedor"] = "000001";

            gvProveedores.AllowPaging = true;
            gvProveedores.PageSize = 15;


            // Enable the GridView sorting option. 
            gvProveedores.AllowSorting = true;


            // Initialize the sorting expression. 
            //ViewState["SortExpression"] = "PersonID ASC";


            // Populate the GridView. 
            BindGridView();
        }
    }

    private void BindGridView()
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            // Create a DataSet object. 
            DataSet dsProveedores = new DataSet();


            // Create a SELECT query. 
            string strSelectCmd = "SELECT VendID,VendName,TaxPayerID,email,Habilitado,Doc1,Doc2,Doc3,Doc4,Doc5,Doc6,Doc7 FROM tapVendorPortal";


            // Create a SqlDataAdapter object 
            // SqlDataAdapter represents a set of data commands and a  
            // database connection that are used to fill the DataSet and  
            // update a SQL Server database.  
            SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);


            // Open the connection 
            conn.Open();


            // Fill the DataTable named "Person" in DataSet with the rows 
            // returned by the query.new n 
            da.Fill(dsProveedores, "tapVendorPortal");


            // Get the DataView from Person DataTable. 
            DataView dvPerson = dsProveedores.Tables["tapVendorPortal"].DefaultView;


            // Set the sort column and sort order. 
            //dvPerson.Sort = ViewState["SortExpression"].ToString();


            // Bind the GridView control. 
            gvProveedores.DataSource = dvPerson;
            gvProveedores.DataBind();
            pnlAdd.Visible = false;
        }

    }

    protected void gvProveedores_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            // Create a command object. 
            SqlCommand cmd = new SqlCommand();


            // Assign the connection to the command. 
            cmd.Connection = conn;


            // Set the command text 
            // SQL statement or the name of the stored procedure  
            cmd.CommandText = "DELETE FROM tapVendorPortal WHERE VendID = @VendID";


            // Set the command type 
            // CommandType.Text for ordinary SQL statements;  
            // CommandType.StoredProcedure for stored procedures. 
            cmd.CommandType = CommandType.Text;


            // Get the PersonID of the selected row. 
            string strPersonID = gvProveedores.Rows[e.RowIndex].Cells[1].Text;


            // Append the parameter. 
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID;


            // Open the connection. 
            conn.Open();



            // Execute the command. 
            cmd.ExecuteNonQuery();
        }


        // Rebind the GridView control to show data after deleting. 
        BindGridView();
    }

    // GridView.PageIndexChanging Event 
    protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        gvProveedores.PageIndex = e.NewPageIndex;


        // Rebind the GridView control to  
        // show data in the new page. 
        BindGridView();
    }

    // GridView.RowCancelingEdit Event 
    protected void gvProveedores_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        gvProveedores.EditIndex = -1;


        // Rebind the GridView control to show data in view mode. 
        BindGridView();


        // Show the Add button. 
        //lbtnAdd.Visible = true;
    }

    // GridView.RowUpdating Event 
    protected void gvProveedores_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {

            String savePath;
            String doc1= " ";
            String doc2 = " ";
            String doc3 = " ";
            String doc4 = " ";
            String doc5 = " ";
            String doc6 = " ";
            String doc7 = " ";
            int Habilitado=0;


            savePath = Server.MapPath("./") + @"Archivos\Proveedores\";

            // Before attempting to perform operations

            if (FileUpload1.HasFile)
            {
                doc1 = FileUpload1.FileName;
                FileUpload1.SaveAs(savePath + FileUpload1.FileName);               
            }
            else
            {
                doc1 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label1")).Text;
            }
            if (FileUpload2.HasFile)
            {
                doc2 = FileUpload2.FileName;
                FileUpload2.SaveAs(savePath + FileUpload2.FileName);
            }
            else
            {
                doc2 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label2")).Text;
            }

            if (FileUpload3.HasFile)
            {
                doc3 = FileUpload3.FileName;
                FileUpload3.SaveAs(savePath + FileUpload3.FileName);
            }
            else
            {
                doc3 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label3")).Text;
            }
            if (FileUpload4.HasFile)
            {
                doc4 = FileUpload4.FileName;
                FileUpload4.SaveAs(savePath + FileUpload4.FileName);
            }
            else
            {
                doc4 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label4")).Text;
            }
            if (FileUpload5.HasFile)
            {
                doc5 = FileUpload5.FileName;
                FileUpload5.SaveAs(savePath + FileUpload5.FileName);
            }
            else
            {
                doc5 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label5")).Text;
            }
            if (FileUpload6.HasFile)
            {
                doc6 = FileUpload6.FileName;
                FileUpload6.SaveAs(savePath + FileUpload6.FileName);
            }
            else
            {
                doc6 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label6")).Text;
            }
            if (FileUpload7.HasFile)
            {
                doc7 = FileUpload7.FileName;
                FileUpload7.SaveAs(savePath + FileUpload7.FileName);
            }
            else
            {
                doc7 = ((Label)gvProveedores.Rows[e.RowIndex].FindControl("Label7")).Text;
            }

            CheckBox chkUpdate = (CheckBox)gvProveedores.Rows[e.RowIndex].FindControl("CheckBox1");

            if (chkUpdate != null)
            {
                if (chkUpdate.Checked)
                {
                    Habilitado = 1;
                }
                else
                {
                    Habilitado = 0;
                }
            }

            // Create a command object. 
            SqlCommand cmd = new SqlCommand();
            
            // Assign the connection to the command. 
              cmd.Connection = conn;
            
             cmd.CommandText = "UPDATE tapVendorPortal SET Habilitado=@Habilitado,Doc1=@Doc1,Doc2=@Doc2,Doc3=@Doc3,Doc4=@Doc4,Doc5=@Doc5,Doc6=@Doc6,Doc7=@Doc7 WHERE VendID = @VendID";
             cmd.CommandType = CommandType.Text;


            // Get the PersonID of the selected row. 
            string strVendID = gvProveedores.Rows[e.RowIndex].Cells[2].Text;
            //string strVendName = ((TextBox)gvProveedores.Rows[e.RowIndex].FindControl("TextBox1")).Text;
          

                    // Append the parameters. 
            cmd.Parameters.Add("@VendID", SqlDbType.VarChar, 12).Value = strVendID;
            cmd.Parameters.Add("@Habilitado", SqlDbType.Bit).Value = Habilitado;
            cmd.Parameters.Add("@Doc1", SqlDbType.VarChar, 2000).Value = doc1;
            cmd.Parameters.Add("@Doc2", SqlDbType.VarChar, 2000).Value = doc2;
            cmd.Parameters.Add("@Doc3", SqlDbType.VarChar, 2000).Value = doc3;
            cmd.Parameters.Add("@Doc4", SqlDbType.VarChar, 2000).Value = doc4;
            cmd.Parameters.Add("@Doc5", SqlDbType.VarChar, 2000).Value = doc5;
            cmd.Parameters.Add("@Doc6", SqlDbType.VarChar, 2000).Value = doc6;
            cmd.Parameters.Add("@Doc7", SqlDbType.VarChar, 2000).Value = doc7;
            // Open the connection. 
            conn.Open();
            
            // Execute the command. 
            cmd.ExecuteNonQuery();

            conn.Close();
        }


        // Exit edit mode. 
        gvProveedores.EditIndex = -1;
        pnlAdd.Visible = false;


        // Rebind the GridView control to show data after updating. 
        BindGridView();


        // Show the Add button. 
        //lbtnAdd.Visible = true;
    }

    protected void gvProveedores_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Make sure the current GridViewRow is a data row. 
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Make sure the current GridViewRow is either  
            // in the normal state or an alternate row. 
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                // Add client-side confirmation when deleting. 
                ((LinkButton)e.Row.Cells[1].Controls[0]).Attributes["onclick"] = "if(!confirm('Are you certain you want to delete this person ?')) return false;";
            }
        }
    }

    // GridView.RowEditing Event 
    protected void gvProveedores_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Make the GridView control into edit mode  
        // for the selected row.  
        gvProveedores.EditIndex = e.NewEditIndex;

        


        // Rebind the GridView control to show data in edit mode. 
        BindGridView();


        // Hide the Add button. 
        //lbtnAdd.Visible = false;
        pnlAdd.Visible = true;
    }

 

}

  
