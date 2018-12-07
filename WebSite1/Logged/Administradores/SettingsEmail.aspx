﻿<%@ Page Language="C#" Title="Configuración Correos" AutoEventWireup="true"  MasterPageFile="~/Site.Master" CodeFile="SettingsEmail.aspx.cs" Inherits="Logged_Administradores_SettingsEmail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<link href ="../../Css/HojaProv.css" rel="stylesheet" type ="text/css" />

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
        }
    </script>
    <script>
        function alert(campo) {
            $(campo).show("slow").delay(2000).hide("slow")
        }
    </script>
    <script>
        function Reset() {
            document.getElementById("tab-2").checked = true;
        }
    </script>

<SCRIPT language="JavaScript">
  function numbersonly(e)
  {
    var unicode=e.charCode? e.charCode : e.keyCode
    if (unicode!=8 && unicode!=44)
    {
      if (unicode<48||unicode>57) //if not a number
      { return false} //disable key press    
    }  
  }  
</SCRIPT>

   <div class="col-lg-12 col-sm-5 col-12" id="M1">
      <h3>Configuración Cuenta de Envio de Correos Tsys</h3>
    </div>

    <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
     <triggers>
<%--       <asp:AsyncPostBackTrigger controlid="SelProv" EventName="SelectedIndexChanged" />--%>
<%--       <asp:AsyncPostBackTrigger controlid="SolCont" eventname="SelectedIndexChanged" />--%>
<%--       <asp:AsyncPostBackTrigger controlid="BtnSave" eventname="Click" />--%>
     </triggers>
    <contenttemplate>


<%--      <div class="col-xs-12 md-12 col-lg-12">
    <div class="col-xs-12 md-4 col-lg-4">
        <label></label>
        <asp:DropDownList ID="SelProv" class="selectpicker show-tick form-control" AutoPostBack ="true" OnSelectedIndexChanged="SelUno" enable="false" data-live-search="true" data-style="btn-primary" runat="server" >
        <asp:ListItem Value="1">General</asp:ListItem>
        </asp:DropDownList>
    </div>
    </div>--%>

      <br />

      <div class="col-xs-12 md-12 col-lg-12">
    <br />
    <br />
    <h4>Perfil de Envío</h4>
    </div>

      <div class="col-xs-12 md-3 col-lg-4">
        <label>Correo Electronico</label>
        <asp:TextBox runat="server" type="text" ID="EmailC" MaxLength="80" class="form-control" placeholder="ejemplo@tsys.com" TabIndex="3" />
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
        <label>Contraseña</label>
        <asp:TextBox runat="server" type="password" ID="Pass" MaxLength="30" placeholder="Contraseña" class="form-control" TabIndex="3" />
      </div>

      <div class="col-xs-6 md-3 col-lg-3">
        <label>Host</label>
        <asp:TextBox runat="server" type="text" ID="Host"  MaxLength="30" placeholder="smtp.tsys.com" class="form-control" TabIndex="3" />
      </div>

      <div class="col-xs-6 md-3 col-lg-2">
        <label>Puerto</label>
        <asp:TextBox runat="server" type="text" ID="Puert" MaxLength="5" onkeypress="return numbersonly(event);" placeholder="Puerto" class="form-control" TabIndex="3" />
      </div>

       <div class="col-xs-12 md-4 col-lg-4">
       <br />

<%--      <div class="col-xs-10 md-10 col-lg-10">
        <label>Copiar Perfil De:</label>
        <asp:DropDownList ID="SolCont" class="selectpicker show-tick form-control" data-live-search="true" AutoPostBack ="true" OnSelectedIndexChanged="SelDos" data-style="btn-primary" runat="server" >
        <asp:ListItem Value="1">Usuario Nuevo</asp:ListItem>
        <asp:ListItem Value="3">Carga de Documentos</asp:ListItem>
        <asp:ListItem Value="4">Notificación Facturas</asp:ListItem>
        <asp:ListItem Value="6">Notificación Pagos</asp:ListItem>
        </asp:DropDownList>
        </div>--%>
       <div class="col-xs-1 col-sm-2 btn-group">
        <span ><br /></span>
        <asp:LinkButton runat="server" type="button" class="btn btn-primary" ID="BtnSave" OnClick="Save">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span>
        </asp:LinkButton>
       </div>
     </div>

    </ContentTemplate>
    </asp:UpdatePanel>


     <asp:UpdatePanel runat="server" id="UpdatePanel1" updatemode="Conditional">
     <triggers>
<%--       <asp:AsyncPostBackTrigger controlid="SelProv" EventName="SelectedIndexChanged" />--%>
<%--       <asp:AsyncPostBackTrigger controlid="SolCont" eventname="SelectedIndexChanged" />--%>
       <asp:AsyncPostBackTrigger controlid="LinkButton1" eventname="Click" />
     </triggers>
    <contenttemplate>

      <div class="col-xs-12 md-11 col-lg-11">
       <br />
       <br />
      <div class="col-xs-12 md-4 col-lg-4">
        <label>Enviar Email de Prueba a:</label>
        <asp:TextBox runat ="server" ID="test" placeholder="Ejemplo@tsys.com" class="form-control"></asp:TextBox>
    </div>            
      <div class="col-xs-1 col-sm-2 btn-group">
        <span ><br /></span>
        <asp:LinkButton runat="server" type="button" class="btn btn-primary" ID="LinkButton1" OnClick="EmailTest1">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-send"></span>
        </asp:LinkButton>
<%--        <asp:LinkButton runat="server" type="button" class="btn btn-primary" ID="BtnSave" OnClick="Save">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-floppy-disk"></span>
        </asp:LinkButton>--%>
       </div>
     </div>

    </ContentTemplate>
    </asp:UpdatePanel>


<%--    <asp:UpdateProgress ID="UpdatePross" runat ="server" AssociatedUpdatePanelID ="">
        <ProgressTemplate>
            <div id="backGround"></div>
            <div id="Update">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>


<%--    <asp:Panel runat ="server">
  <span class ="col-xs-12 col-lg-12 col-md-12"></span>
      <div class="col-xs-12 md-12 col-lg-12">
      <br />
       <br />
       <br />
       <br />
       <br />
       <br />
       <br />
      <h3>Recepcion de Notificaciones</h3>
      <br />
    <div class="col-xs-12 md-12 col-lg-12">
    <div class="col-xs-12 md-4 col-lg-4">
        <label></label>
        <asp:DropDownList ID="SelProv" class="selectpicker show-tick form-control" AutoPostBack ="true" OnSelectedIndexChanged="SelUno" enable="false" data-live-search="true" data-style="btn-primary" runat="server" >
        <asp:ListItem Value="1">Usuario Nuevo</asp:ListItem>
        <asp:ListItem Value="2">Notificación Pagos</asp:ListItem>
        <asp:ListItem Value="3">Notificación Facturas</asp:ListItem>
        <asp:ListItem Value="4">Carga de Documentos</asp:ListItem>  
        </asp:DropDownList>
    </div>
    </div>
    </div>
        <div class="col-xs-12 md-12 col-lg-12">
         <br />
        <asp:Label runat="server">A continuación, ingrese las direcciones de correo a las que se les notificará cuando se ejecute esta Actividad</asp:Label>
      </div>
      <div class="row" id="Cam">
        <br />
            <div class="col-xs-12 col-sm-7 col-md-5">
            <br />
                <div class="form-group row">
                    <label class="col-form-label col-xs-12 col-sm-3">Correo Electronico:</label>
                    <div class="col-xs-10 col-sm-9">
                        <asp:TextBox runat="server" type="text" MaxLength="80" ID="Email" class="form-control" TabIndex="1" />
                    </div>
                </div>
            </div>
            <div class="btn-toolbar" role="toolbar">
                <div class="col-xs-1 col-sm-2 btn-group">
                <br />
                <br />
                    <asp:LinkButton runat="server" type="button" class="btn btn-success" ID="BtnBusc" OnClick="AddU">                                        
                      <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>
       

      <div class="row">
            <br />
          
             <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table" 
                    margin-left="auto" margin-right="auto" runat="server" 
                    AutoGenerateColumns="False" 
                    onpageindexchanging="GridView1_PageIndexChanging" 
                    onrowcancelingedit="GridView1_RowCancelingEdit" 
                    OnRowCreated = "GridView1_RowCreated" 
                    onrowupdating="GridView1_RowUpdating" 
                    OnRowDataBound="GridView1_RowDataBound"
                    width="50%">

                    <AlternatingRowStyle BackColor="White" />

                    <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#ffffcc" />
                    <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

                    <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                <ItemTemplate>
                                    <asp:Button ID="btnCancel" runat="server" Text="Eliminar" CssClass="btn btn-danger" CommandName="Cancel"   />
                                </ItemTemplate>
                                <EditItemTemplate>
                                </EditItemTemplate>

                                <HeaderStyle Width="80px"></HeaderStyle>

                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Email" HeaderText="Destinatarios" HeaderStyle-Width="140px" ReadOnly="True">
                                <HeaderStyle Width="75px"></HeaderStyle>
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

    </asp:Panel>--%>

</asp:Content>