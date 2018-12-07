<%@ Page Language="C#" Title="Administracion de Usuarios" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="AdmUsers.aspx.cs" Inherits="Logged_Administradores_AdmUsers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script>
        function alert(campo)
        {
         $(campo).show("slow").delay(3000).hide("slow")
         return true;
        }
    </script>
    <script>
        function alertme(titulo, mesaje, Tipo)
        {
        swal(titulo, mesaje, Tipo)
        }
    </script>

<div class="col-lg-12 col-sm-12 col-12" id="M1">
<br />
<h3>Administración de Usuarios</h3>
</div>

    <h4>Criterios de Búsqueda</h4>
    <br />
    <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
     <triggers>
       <asp:AsyncPostBackTrigger controlid="LinkButton1" eventname="Click" />
       <asp:AsyncPostBackTrigger controlid="LinkButton2" eventname="Click" />
       <asp:AsyncPostBackTrigger controlid="LinkButton3" eventname="Click" />
     </triggers>
    <contenttemplate>

    <div class="col-sx-12 col-md-4">
        <label >Correo Electronico</label>
        <asp:TextBox runat="server" type="text" ID="Email" MaxLength="90" class="form-control" placeholder="ejemplo@tsys.com" TabIndex="3"/>
    </div>

    <div class="col-sx-12 col-md-4">
        <label >Nombre / Razón Social</label>
        <asp:TextBox runat="server" type="text" ID="Nombre" MaxLength="120" class="form-control" placeholder="Nombre / Razón social" TabIndex="3"/>
    </div>

    <div class="col-sx-12 col-md-3">
        <div class="col-xs-10  col-md-8">
            <label>Status</label>
            <asp:DropDownList runat="server" ID="Cclientes" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged">
                <asp:ListItem Value="1">Activo</asp:ListItem>
                <asp:ListItem Value="2">Inactivo</asp:ListItem>
                <asp:ListItem Value="3">Pendiente</asp:ListItem>
            </asp:DropDownList>
        </div>


    </div>

    <div class="col-xs-12 md-3 col-lg-2">
        <br />
       <label>Desde</label>
       <asp:TextBox type="date" name="fecha" ID="F1" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
    </div>

    <div class="col-xs-12 md-3 col-lg-2">
        <br />
          <label>Hasta</label>
        <asp:TextBox type="date" name="fecha" ID="F2" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>

    <div class="col-sx-12 col-md-3">
        <br />
        <div class="col-xs-10  col-md-8">
            <label>Rol Asignado</label>
            <asp:DropDownList runat="server" ID="DRol" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged">
                <asp:ListItem Value="1">Seleccione Rol</asp:ListItem>
                <asp:ListItem Value="2">Proveedor</asp:ListItem>
                <asp:ListItem Value="3">T|SYS| - Admin</asp:ListItem>
                <asp:ListItem Value="4">T|SYS| - Validador</asp:ListItem>
                <asp:ListItem Value="5">T|SYS| - Consultas</asp:ListItem>
            </asp:DropDownList>
    </div>


    </div>

    <div class="col-xs-2 col-md-4">
        <br />
        <span aria-hidden="true">
            <br />
        </span>
        <asp:LinkButton runat="server" type="button" class="btn btn-success" OnClick="Buscar" ID="LinkButton1">                                        
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
        </asp:LinkButton>

        <asp:LinkButton runat="server" type="button" class="btn btn-primary" OnClick="Clean" ID="LinkButton3">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-erase"></span>
        </asp:LinkButton>
        <br />
        <br />
        <br />
        <br />
    </div>

    <div class="col-xs-10 col-sm-9">
     <div class="alert alert-block alert-success" id="AL" style="display: none">
        <h5>¡Exito!</h5>
        <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
      </div>
    </div>
    <div class="col-xs-10 col-sm-9">
     <div class="alert alert-block alert-danger" id="AL1" style="display: none">
        <h5>¡Error!</h5>
        <asp:Label ID="Labe5" runat="server" Text=""></asp:Label>
      </div>
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

    <div class="row">
    <br />

    <br />
    <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table"
        margin-left="auto" margin-right="auto" runat="server"
        AutoGenerateColumns="False"
        OnPageIndexChanging="GridView2_PageIndexChanging"
        onrowediting="GridView2_RowEditing" 
        OnRowCancelingEdit="GridView2_RowCancelingEdit"
        OnRowCreated="GridView2_RowCreated"
        OnRowUpdating="GridView2_RowUpdating"
        OnRowDataBound="GridView2_RowDataBound"
        Width="99%">

        <AlternatingRowStyle BackColor="White" />

        <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#ffffcc" />
        <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

        <Columns>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
             <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="Resset" CssClass="btn btn-warning" CommandName="Edit" />
                </ItemTemplate>
             <edititemtemplate>
                    <asp:Button ID="btnUpdate" runat="server" Text="Enviar" CssClass="btn btn-success" CommandName="Update" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="btn btn-danger" CommandName="Cancel" />
                </edititemtemplate>
            <HeaderStyle Width="80px"></HeaderStyle>

            <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField DataField="Nombre" HeaderText="Nombre" HeaderStyle-Width="70px" ReadOnly="True">
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Correo" HeaderText="Correo" HeaderStyle-Width="120px" ReadOnly="True">
                <HeaderStyle Width="75px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Fecha" HeaderText="Fecha Creado" HeaderStyle-Width="70px" ReadOnly="True">
                <HeaderStyle Width="140px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Creador" HeaderText="Rol" HeaderStyle-Width="70px" ReadOnly="True">
                <HeaderStyle Width="140px"></HeaderStyle>
            </asp:BoundField>

          <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="70px"  HeaderText="Status" ValidateRequestMode="Enabled">
          <ItemTemplate>
               <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Stat" >
                <asp:ListItem Value="1">Activo</asp:ListItem>
                <asp:ListItem Value="2">Inactivo</asp:ListItem>
<%--                <asp:ListItem Value="3">Pendiente</asp:ListItem>--%>
               </asp:DropDownList>
          </ItemTemplate> 
             <HeaderStyle Width="70px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

            <asp:BoundField DataField="Correo" HeaderText="Enviar A" HeaderStyle-Width="70px" ReadOnly="False">
                <HeaderStyle Width="70px"></HeaderStyle>
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

        <div class="col-xs-12 col-md-12">
        <span class="col-xs-12 col-md-11"></span>
        <div class="col-xs-12 col-md-1">
            <asp:Button runat="server" type="button" CssClass="btn btn-primary" OnClick="Save" Text ="Guardar" ID="LinkButton2"></asp:Button>
            <br />
        </div>
       </div>

    </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>