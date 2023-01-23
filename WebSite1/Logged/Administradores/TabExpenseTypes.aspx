<%@ Page title="Tabulador Tipos de Gastos" Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.master" CodeFile="TabExpenseTypes.aspx.cs" Inherits="TabExpenseTypes" %>

<script runat="server">

   
</script>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 16-Octubre-2019 By Luis Angel Garcia P
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // We can attach the `fileselect` event to all file inputs on the page
            $(document).on('change', ':file', function () {
                var input = $(this),
                    numFiles = input.get(0).files ? input.get(0).files.length : 1,
                    label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                input.trigger('fileselect', [numFiles, label]);
            });

            // We can watch for our custom `fileselect` event like this
            $(document).ready(function () {
                $(':file').on('fileselect', function (event, numFiles, label) {

                    var input = $(this).parents('.input-group').find(':text'),
                        log = numFiles > 1 ? numFiles + ' files selected' : label;

                    if (input.length) {
                        input.val(log);
                    } else {
                        if (log) alert(log);
                    }
                });
            });
            return true;
        })
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
            unblockUI();
        }
    </script>

    <script>
        function alerte(campo) {
        $(campo).show("slow").delay(2000).hide("slow")
        }
    </script>

    <script>
        function Alerta(Doc) {
            alert(Doc);
            unblockUI();
        }
    </script>

    <div class="Titulo">
        <div class="col-lg-12 col-sm-12 col-12" id="M1">
            <h3>Tabulador de Tipos de Gastos</h3>
        </div>
        <br />
        <div class="row">
            <h4>Agregar nuevo Tipo de Gasto</h4>
             <div class="col-xs-6 col-sm-6 col-md-6">                         
                <h5>Descripcion</h5>
                <div class="col-xs-12 col-sm-12 col-md-12">                    
                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="tbx_tipogasto" class="form-control" TabIndex="1" />                                   
                </div>
              </div>            
        </div>
        <br />
        <br />
        <div class="row">
            <div class="col-xs-3 col-sm-3 col-md-3">                              
                 <div class="col-xs-12 col-sm-12 col-md-12">                
                    <asp:Button Text="Guardar" runat="server" ID="btn_uardar" autoposback="True" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_uardar_Click" />
                </div>                            
              </div>  
              <div class="col-xs-3 col-sm-3 col-md-3">                       
                    <div class="col-xs-12 col-sm-12 col-md-12">                
                        <asp:Button Text="Limpiar" runat="server" ID="btn_limpiar" autoposback="True" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_limpiar_Click" />
                    </div>                            
               </div> 
        </div>                 
       
     </div>
        <br />      
        <div class="row" style="padding-left:50px">
            <asp:GridView Caption="Tipos de Gasto"
                ID="gv_nomenclador"
                runat="server"
                CssClass="table table-bordered bs-table"
                margin-left="auto" margin-right="auto"
                AutoGenerateColumns="False"
                OnRowCommand="gv_nomenclador_RowCommand"
                OnRowDataBound="gv_nomenclador_RowDataBound"
                CellPadding="4"
                ForeColor="#333333" GridLines="None" Width="50%">

                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Width="20px" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" HeaderStyle-Width="10%" ReadOnly="True"/>                              
                    <asp:BoundField DataField="Name" HeaderText="Tipo de Gasto" HeaderStyle-Width="10%" ReadOnly="True"/> 
                    <asp:CommandField ShowSelectButton="True"  ControlStyle-CssClass="btn-success" ButtonType="Button" SelectText="Editar" CancelText="Editar" ShowCancelButton="True" >
                         <ControlStyle CssClass="btn-success"></ControlStyle>
                    </asp:CommandField>
                   <asp:TemplateField >
                   <ItemTemplate>
                      <asp:Button ID="btnDelete" runat="server" CssClass="btn-warning" CommandName="Delete" Text="Eliminar"  OnCommand="btnDelete_Command"></asp:Button>
                   </ItemTemplate>
                 </asp:TemplateField> 
                   
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
        </div>


    </div>

</asp:Content>