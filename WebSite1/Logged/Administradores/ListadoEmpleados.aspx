<%@ Page title="Listado de Empleados" Language="C#" AutoEventWireup="true"  MasterPageFile="~/Logged/Administradores/SiteVal.master" CodeFile="ListadoEmpleados.aspx.cs" Inherits="ListadoEmpleados" %>

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
            <h3>Listado de Empleados por Validador</h3>
        </div>
       
        <br />
        <br />
        <div class="row">
             <div class="col-lg-6 col-sm-6 col-6" >
             <asp:ListBox runat="server" ID="listEmpleados" Width="50%"></asp:ListBox>                
            </div>   
        </div>   
        

    </div>

</asp:Content>