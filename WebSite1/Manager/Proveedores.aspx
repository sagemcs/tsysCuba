<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.Master"  CodeFile="Proveedores.aspx.cs" Inherits="Default2" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" onpageindexchanging="gvProveedores_PageIndexChanging" onrowcancelingedit="gvProveedores_RowCancelingEdit" onrowdatabound="gvProveedores_RowDataBound" onrowdeleting="gvProveedores_RowDeleting" onrowediting="gvProveedores_RowEditing" onrowupdating="gvProveedores_RowUpdating">
        <RowStyle BackColor="White" ForeColor="#003399" /> 
    <Columns> 
        <asp:CommandField ShowEditButton="True" /> 
        <asp:CommandField ShowDeleteButton="True" /> 
        <asp:BoundField DataField="VendID" HeaderText="ProveedorID" ReadOnly="True" SortExpression="VendID" /> 
        <asp:BoundField DataField="VendName" HeaderText="Nombre" ReadOnly="True" SortExpression="VendName" /> 
        <asp:BoundField DataField="TaxPayerID" HeaderText="RFC" ReadOnly="True" SortExpression="TaxPayerID" />
        <asp:BoundField DataField="email" HeaderText="email" ReadOnly="True" SortExpression="email" />
        <asp:TemplateField HeaderText="Habilitado"> 
            <EditItemTemplate> 
                <asp:CheckBox ID="CheckBox1" runat="server" Enabled="True" Checked='<%# Bind("Habilitado") %>'></asp:CheckBox> 
            </EditItemTemplate> 
            <ItemTemplate> 
                <asp:CheckBox ID="CheckBox2" runat="server" Enabled="False" Checked='<%# Bind("Habilitado") %>'></asp:CheckBox>
            </ItemTemplate> 
        </asp:TemplateField>  
        <asp:TemplateField HeaderText="Doc1"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Doc1") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView" Text='<%# Bind("Doc1") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc1")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>  
        <asp:TemplateField HeaderText="Doc2"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Doc2") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView2" Text='<%# Bind("Doc2") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc2")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField> 
        <asp:TemplateField HeaderText="Doc3"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Doc3") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView3" Text='<%# Bind("Doc3") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc3")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Doc4"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Doc4") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView4" Text='<%# Bind("Doc4") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc4")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Doc5"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Doc5") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView5" Text='<%# Bind("Doc5") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc5")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Doc6"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Doc6") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView6" Text='<%# Bind("Doc6") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc6")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Doc7"> 
            <EditItemTemplate>                 
                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Doc7") %>'></asp:Label>
            </EditItemTemplate> 
            <ItemTemplate> 
               <asp:HyperLink ID="lnkView7" Text='<%# Bind("Doc7") %>' NavigateUrl='<%# String.Format("~/ViewPdfPage.aspx?fileName={0}",Eval("Doc7")) %>' runat="server" Target="_blank" />
            </ItemTemplate> 
        </asp:TemplateField>
         <asp:BoundField DataField="Doc3" HeaderText="Doc.3" ReadOnly="True" SortExpression="Doc3" />
         <asp:BoundField DataField="Doc4" HeaderText="Doc.4" ReadOnly="True" SortExpression="Doc4" />
         <asp:BoundField DataField="Doc5" HeaderText="Doc.5" ReadOnly="True" SortExpression="Doc5" />
         <asp:BoundField DataField="Doc6" HeaderText="Doc.6" ReadOnly="True" SortExpression="Doc6" />
         <asp:BoundField DataField="Doc7" HeaderText="Doc.7" ReadOnly="True" SortExpression="Doc7" />
    </Columns> 
    <FooterStyle BackColor="#99CCCC" ForeColor="#003399" /> 
    <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" /> 
    <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" /> 
    <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" /> 
    </asp:GridView>
    <asp:Panel ID="pnlAdd" runat="server" Visible="False">
    <table border="1">
        <caption>Carga de Archivos de Proveedor</caption>
        <tr>
            <th> Acta constitutiva: </th>
            <th><asp:FileUpload ID="FileUpload1" runat="server" /></th>            
        </tr>
        <tr>
            <th>RFC:</th>
            <th><asp:FileUpload ID="FileUpload2" runat="server" /></th>            
        </tr>
        <tr>
            <th>Poder notarial:</th>
            <th>
                <asp:FileUpload ID="FileUpload3" runat="server" />
            </th>            
        </tr>
        <tr>
            <th>Identificación oficial representante legal:</th>
            <th>
                <asp:FileUpload ID="FileUpload4" runat="server" />
            </th>            
        </tr>
         <tr>
            <th>Opinión de cumplimiento de Obligaciones Fiscales:</th>
            <th>
                <asp:FileUpload ID="FileUpload5" runat="server" />
             </th>            
        </tr>
         <tr>
            <th>Carta Instrucción:</th>
            <th>
                <asp:FileUpload ID="FileUpload6" runat="server" />
             </th>            
        </tr>
         <tr>
            <th>Contrato y/o carta de presentación</th>
            <th>
                <asp:FileUpload ID="FileUpload7" runat="server" />
             </th>             
        </tr>

    </table>
    </asp:Panel>    
</asp:Content>
