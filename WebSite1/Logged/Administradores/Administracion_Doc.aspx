<%@ Page Title="Administrar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Administracion_Doc.aspx.cs" Inherits="Logged_Administrar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
        <link href ="../../Css/HojaProv.css" rel="stylesheet" type ="text/css" />
        
        <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
            return true;
        }
    </script>

    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Administracion de Documentos</h3>
    </div>

  <asp:UpdatePanel runat="server" id="UpdatePanel1">
  <ContentTemplate>

    <div class="col-xs-12 md-4 col-lg-4">
        <label>Seleccione Proveedor</label>
        <asp:DropDownList ID="SelProv" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" OnSelectedIndexChanged="SelProv_SelectedIndexChanged1"></asp:DropDownList>
    </div>   


        <div class="col-xs-12 md-4 col-lg-4">
        <label>Seleccione Empresa</label>
        <asp:DropDownList ID="SelComp" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" ></asp:DropDownList>
        </div>

        <div class="col-xs-12 md-4 col-lg-4">
        <label>ID SAGE:</label>
         <asp:TextBox runat="server" type="text" MaxLength="200" ID="IDSAGE" class="form-control" TabIndex="3" readonly="true" />
        </div>

        <div class="col-xs-12 md-4 col-lg-4">
        <label>Email:</label>
         <asp:TextBox runat="server" type="text" MaxLength="200" ID="Mail" class="form-control" TabIndex="2" ReadOnly="true" />
        </div>
        

        <div class="col-xs-12 md-12 col-lg-12">
            <br />
            <br />
        </div>
   
     <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto"  OnRowDataBound="GridView2_RowDataBound" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="GridView2_RowCommand" GridLines="Horizontal" HorizontalAlign="Center" >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <Columns>        

         <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5px">
          <ItemTemplate>
             <asp:LinkButton ID="Documento_1" Widht="5px" CssClass="btn btn-info" title="Descargar Documento" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"><span aria-hidden="true" class="glyphicon glyphicon-download-alt"></span></asp:LinkButton>
          </ItemTemplate> 
             <HeaderStyle Width="5px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

          <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px"  HeaderText="Aprobado" ValidateRequestMode="Enabled">
          <ItemTemplate>
                <asp:CheckBox ID="Check" CssClass="ChkBoxClass" style="width: 20px; height: 20px;" runat ="server" CommandName="Check" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
          </ItemTemplate> 
             <HeaderStyle Width="15px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

            <asp:BoundField DataField="status" HeaderText="Status" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100px">
                <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="100px"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="Nombre" HeaderText="Documento" ReadOnly="True" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="300px">
                <HeaderStyle Width="300px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="300px"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="Archivo" HeaderText="Archivo Recibido" ReadOnly="False" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px" Visible ="false">
                <HeaderStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="60px"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="Fecha" HeaderText="Fecha Recepción" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="180px">
                <HeaderStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="180px"></ItemStyle>
            </asp:BoundField>


         </Columns>
        <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" /> 
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" /> 
        <RowStyle CssClass="table table-bordered" BackColor="#EFF3FB" /> 
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" /> 

        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>

     <div class="row">
          <div class="col-xs-11 col-sm-3 col-md-3">
              <div class="form-group row">
                  <div class="col-xs-11 col-sm-6 col-md-6">
                      <asp:Button Text="Aprobar Todos" ID="Button2" runat="server" CssClass="btn btn-success" title="Seleccionar Todo"  OnClick="OkAll" />
                  </div>

                   <div class="col-xs-11 col-sm-6 col-md-6">
                      <asp:Button Text="Cancelar Todo" ID="Button3" runat="server" CssClass="btn btn-danger" title="Deseleccionar Todo" OnClick="NoAll" />
                  </div>
              </div>

          </div>

          <label class="col-form-label col-xs-11 col-sm-6 col-md-6"></label>

          <div class="col-xs-11 col-sm-3 col-md-3">
              <div class="form-group row">
                  <label class="col-form-label col-sm-11"></label>
                  <div class="col-sm-1">
                      <asp:Button Text="Guardar" ID="Button1" runat="server" CssClass="btn btn-primary" title="Guardar Datos" OnClick="Aceptar" />
                  </div>
              </div>
          </div>
   </div>
   
<%--   </ContentTemplate>
  </asp:UpdatePanel>--%>


          <div class="col-xs-12 md-11 col-lg-11">
      <label id="lc" runat ="server">Comentarios</label>
      <asp:TextBox runat="server" type="text" MaxLength="200" TextMode ="MultiLine" Maxheight="300px" MaxWidht="700px" class="form-control" ID="Comentarios"></asp:TextBox>
      </div>
          <asp:Panel runat="server" ID="DatosV" Width="90%" Height=" 300px">
        <div class="col-xs-12">
            <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
        </div>
        <div class="col-xs-12" id="Timg">
            <center>     
                    <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
                   </center>

        </div>
    </asp:Panel>

     </ContentTemplate>
     <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />
        <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
        <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
        <asp:PostBackTrigger ControlID="gvFacturas"/>
    </Triggers>
  </asp:UpdatePanel>
    <br />
    <br />
    <br />
    <br />


</asp:Content>