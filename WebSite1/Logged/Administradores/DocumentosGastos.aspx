<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteVal.master" AutoEventWireup="true" CodeFile="DocumentosGastos.aspx.cs" Inherits="Logged_Administradores_DocumentosGastos" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 20-07-2022 By Rafael Boza
    <script src ="../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>   
    <script src ="../../Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <link rel="stylesheet" href="../../Scripts/bootstrap-datepicker/css/datepicker.css">
    <style>

            #Update1{
            position :fixed;
            top:40%;
            bottom :40%;
            left : 40%;
            right: 40%;
            z-index : 1001;
            overflow : hidden;
            margin :0;
            padding :0;
            background-color : #999;
            filter :alpha(opacity =50);
            opacity : 0.80;
            border: 1px solid gray;
            border:none;
            background-image :url("../../Img/aguarde.gif");
            background-repeat :no-repeat;
            background-size:cover;
            background-position:center;
            border-radius: 15px 15px 15px 15px;
            -moz-border-radius: 15px 15px 15px 15px;
            -webkit-border-radius: 15px 15px 15px 15px;}
        
            #backGround1{
            position :fixed;
            top:0px;
            bottom : 0px;
            left : 0px;
            right: 0px;
            overflow : hidden;
            margin :0;
            padding :0;
            background-color : #999999;
            filter :alpha(opacity =80);
            opacity : 0.80;
            z-index : 1000;}

            #Testdbd {
            width:50px;
            height:50px;
            background-color:red;}
        .auto-style1 {
            left: 0px;
            top: 0px;
        }
        </style>

    <script type="text/javascript">
        $(function () {            
           

            // We can attach the `fileselect` event to all file inputs on the page
            $(document).on('change', ':file', function () {
                var input = $(this);
                var caja = $(this).parents('.input-group').find(':text'),
                numFiles = input.get(0).files ? input.get(0).files.length : 1;
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                //input.trigger('fileselect', [numFiles, label]);
                caja.val(label);
                //swal('Angel', label, 'success')
            });

            // We can watch for our custom `fileselect` event like this
            $(document).ready(function () {

                var ctrlKeyDown = false;
                $(document).on("keydown", keydown);
                $(document).on("keyup", keyup);


                $('#STipoGasto').change(function () {

                    console.log('cambio');

                });

                function keydown(e) {

                    if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                        // Pressing F5 or Ctrl+R
                        //VariableG();
                        e.preventDefault();
                        setTimeout("location.href='Facturas'", 10);
                    } else if ((e.which || e.keyCode) == 17) {
                        // Pressing  only Ctrl
                        ctrlKeyDown = true;
                    }
                };

                function keyup(e) {
                    // Key up Ctrl
                    if ((e.which || e.keyCode) == 17)
                        ctrlKeyDown = false;
                };
                //$(':file').on('fileselect', function (event, numFiles, label) {

                //    var input = $(this).parents('.input-group').find(':text'),
                //    log = numFiles > 1 ? numFiles + ' files selected' : label;
                //    if (input.length) {
                //        input.val(log);
                //    } else {
                //        input.val(log);
                //        //if (log) alert(log);
                //    }

                //});

            });
        })
    </script>
    
    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo);
            unblockUI();
        }
    </script>

    <script>
        function alertB(campo) {
            $(campo).toggle();
            unblockUI();
        }
    </script>

    <script >
        function hideshow() {
            document.getElementById('UpdatePross').style.display = "none";
            unblockUI();
        }
    </script>

    <script>
        function alert(campo) {
            $(campo).show("slow").delay(2000).hide("slow");
            unblockUI();
        }
    </script>  
        
            <br />
            <br />
            <br />
            <div class="col-lg-12 col-sm-12 col-12" id="m4">
                <h4>Documentos del Gasto</h4>
            </div>
            <br />
            <div class="row">
                 <div class="col-xs-3 col-md-3 col-xs-3">
                            <asp:Button ID="btn_cancel" runat="server" Class="btn btn-success" title="Regresar" Text="Regresar" OnClick="btn_cancel_Click" />
                        </div>
            </div>
            <br />
            <%--Fila del Comentarios--%>
            <div  class="row">
               <%--Grid Articulos--%>
                    <asp:GridView Caption="Articulos"
                        ID="GV_Gastos"
                        runat="server"
                        CssClass="table table-bordered bs-table"
                        margin-left="auto" margin-right="auto"
                        AutoGenerateColumns="False"
                        OnRowCommand="GV_Gastos_RowCommand"
                        OnRowDataBound="GV_Gastos_RowDataBound"
                        CellPadding="4"                       
                        ForeColor="#333333" GridLines="None">

                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Width="20px" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <Columns>
                            <asp:BoundField DataField="ExpenseType" HeaderText="Tipo de Gasto" ReadOnly="True" SortExpression="ExpenseType" >  
                             <HeaderStyle Width="70%" />
                            </asp:BoundField>
                             <asp:TemplateField HeaderText="Xml" ItemStyle-Wrap="true">
                                <ItemTemplate >
                                    <asp:Image ID="img_xml" runat="server" Width="10px" Height="10px" />
                                    <asp:Button ID="btn_xml" runat="server" CssClass="btn-success" CommandName="Delete" Text="Descargar"  OnCommand="btn_xml_Command" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:Button>
                                </ItemTemplate>
                                 <HeaderStyle HorizontalAlign="Center" Width="10%" />

<ItemStyle Wrap="True"></ItemStyle>
                           </asp:TemplateField>                            
                             <asp:TemplateField HeaderText="Pdf">
                                <ItemTemplate>
                                    <asp:Image ID="img_pdf" runat="server" Width="10px" Height="10px" />
                                       <asp:Button ID="btn_pdf" runat="server" CssClass="btn-success" CommandName="Delete" Text="Descargar"  OnCommand="btn_pdf_Command" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:Button>
                                </ItemTemplate>
                                 <HeaderStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>                             
                             <asp:TemplateField HeaderText="Voucher">
                                <ItemTemplate>
                                    <asp:Image ID="img_voucher" runat="server" Width="10px" Height="10px" />
                                    <asp:Button ID="btn_voucher" runat="server" CssClass="btn-success" CommandName="Delete" Text="Descargar"  OnCommand="btn_voucher_Command" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:Button>
                                </ItemTemplate>
                                 <HeaderStyle HorizontalAlign="Center" Width="10%" />
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
    
   
</asp:Content>

