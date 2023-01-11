<%@ Page title="Establecer Artículo" Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.master" CodeFile="ArticleMME.aspx.cs" Inherits="ArticleMME" %>

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
            <h3>Establecer artículo para Gastos Médicos Menores</h3>
        </div>
        <div class="row">
            <div class="col-lg-4 col-sm-4 col-xs-4">
                 <h4>Articulo predeterminado:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:Label runat="server" ID="lbl_article"></asp:Label>
                    </div>
                </div>
        </div>
        <br />
        <br />
        <div class="row">
             <%--Articulos--%>
                <div class="col-lg-4 col-sm-4 col-xs-4">
                    <h4>Gastos:</h4>
                    <span class="dropdown-header">
                        <asp:DropDownList ID="drop_articulos" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" >
                        </asp:DropDownList>
                    </span>
                </div>
                <%--Agregar articulo--%>
                <div class="col-lg-4 col-sm-4 col-xs-4">
                    <h4>Guardar</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:Button ID="btn_setitem" runat="server" Class="btn btn-tsys cargar" title="Establecer Artículo" Text="Establecer Artículo" OnClick="btn_setitem_Click" CausesValidation="true" />
                    </div>
                </div>
        </div>   
        

    </div>

</asp:Content>