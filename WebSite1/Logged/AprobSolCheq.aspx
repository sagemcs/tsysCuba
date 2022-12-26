<%@ Page Title="Aprobacion Solicitud Cheques" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AprobSolCheq.aspx.cs" Inherits="Logged_AprobSolCheq" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!--Version 10-JUNIO-2019 By Luis Angel-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Last-Modified" content="0">
    <meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
    <meta http-equiv="Cache-Control" content="no-cache, mustrevalidate">
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type ="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type ="text/javascript"></script>
    <script src ="../../Css/sweetalert2.all.min.js" type ="text/javascript"></script>
<%--    <script src ="../../Scripts/AdmFac.js" type ="text/javascript"></script>--%>

    <style>
        #Update{
        position :fixed;
        top:40%;
        bottom :40%;
        left : 40%;
        right: 40%;
        position: absolute;
        z-index : 1001;
        overflow : hidden;
        margin :0;
        padding :0;
        background-color : #fcfcfc;
        filter :alpha(opacity =99);
        opacity : 2.50;
        border: 1px solid gray;
        border:none;
        background-image :url("../../Img/mail_success.gif");
        background-repeat :no-repeat;
        background-size:cover;
        background-position:center;
        border-radius: 20px 20px 20px 20px;
        -moz-border-radius: 20px 20px 20px 20px;
        -webkit-border-radius: 20px 20px 20px 20px;
        /*border: 0px none #000000;*/
}
    </style>

    <script>
        $(document).ready(function () {

            var ctrlKeyDown = false;            
            $(document).on("keydown", keydown);
            $(document).on("keyup", keyup);

            function keydown(e) {

                if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                    // Pressing F5 or Ctrl+R
                    VariableG();
                    e.preventDefault();                    
                    setTimeout("location.href='AprobSolCheq'", 10);
                } else if ((e.which || e.keyCode) == 17) {
                    // Pressing  only Ctrl
                    ctrlKeyDown = true;
                }
            };

            function keyup(e) {
                // Key up Ctrlupdate
                if ((e.which || e.keyCode) == 17)
                    ctrlKeyDown = false;
            };

          $('#btn_send_comments').click(function (e) {
              $('#myModal').modal('hide');
              $('#Update').modal('show');
              Proceso(e, "/enviar_notificacion_de_rechazo")
              return false;
          });

          $('#btn_No_Send').click(function (e) {
              Proceso(e, "/NOenviar_notificacion_de_rechazo");
              VariableG();
              setTimeout("location.href='AprobSolCheq'", 1);
              return false;
          });

          function VariableG()
          {
              var Proce = "/Actualizar_Variable2";
              var Act = 'Java';
              var Prv_Adm = $('#MainContent_IdProveedor').val();
              var Fol_Adm = $('#MainContent_Folio').val();
              var NOC_Adm = $('#MainContent_FechaR').val(); 
              var Sts_Adm = $('#MainContent_FolioC').val();
              var Chk_Adm = $('#MainContent_FechaAp').val();
              var Fe1_Adm = $('#MainContent_FechaPago').val();
              var Fe2_Adm = $('#MainContent_total_').val();
              var url_list = NotificacionesWebService.get_path() + Proce;
              $.ajax({
                  type: "POST",
                  "beforeSend": function (xhr) {
                      xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                  },
                  url: url_list,
                  dataType: "json",
                  data: {
                      bts: Act,
                      Prv: Prv_Adm,
                      Fol: Fol_Adm,
                      NOC: NOC_Adm,
                      Sts: Sts_Adm,
                      Chk: Chk_Adm,
                      Fe1: Fe1_Adm,
                      Fe2: Fe2_Adm
                  }
              });
          }

          function Proceso(e,Proce)
          {
              e.preventDefault();
              var comentario = $('#MainContent_inputComentario').val();
              var folio_factura = $('#inputFolio').val();
              var folio_llave = $('#inputllave').val();
              var Fila = $('#inputFila').val();
              var url_list = NotificacionesWebService.get_path() + Proce;
              $.ajax({
                  type: "POST",
                  "beforeSend": function (xhr) {
                      xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                  },
                  url: url_list,
                  dataType: "json",
                  data: {
                      folio: folio_factura,
                      texto: comentario,
                      llave: folio_llave
                  },
                  success: function (respuesta) {
                      $('#myModal').modal('hide');
                      $('#Update').modal('hide');
                      if (respuesta.success) {
                          $('#MainContent_inputComentario').val('');
                          $('#inputFolio').attr('value', '');
                          $('#inputllave').attr('value', '');
                          var table = document.getElementById("<%= gvFacturas.ClientID %>");
                          table.deleteRow(Fila);
                          swal({
                              title: "Notificaciones T|SYS|",
                              text: respuesta.msg,
                              icon: "success",
                              buttons: ["OK"],
                              confirmButtonColor: "#FFFFFF",
                              closeModal: true,
                              closeOnConfirm: false,
                              dangerMode: true,
                             })
                            .then((willDelete) => {
                                if (willDelete) {
                                    VariableG();
                                    setTimeout("location.href='AprobSolCheq'", 1);
                                }
                                else {
                                    VariableG();
                                    setTimeout("location.href='AprobSolCheq'", 1);
                                }
                            });
                      }
                      else
                      {
                          $('#myModal').modal('hide');
                          $('#Update').modal('hide');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                          VariableG();
                          setTimeout("location.href='AprobSolCheq'", 1);
                      }
   
                  },
                  error: function (respuesta) {
                      $('#Update').modal('close');
                      $('#myModal').modal('close');
                      swal("La notificación de rechazo de Solicitud de Cheque no ha podido ser enviada");
                      VariableG();
                      setTimeout("location.href='AprobSolCheq'", 1);
                  }
              });

              return false;
          }
     });

      function alert(campo) {
          $(campo).show("slow").delay(2000).hide("slow");
          return true;
      }

        function showModal() {
            var inputVal = $("#tokenValue").val();
            var tokenVal = $("#tbx_token").val();
            if (inputVal == tokenVal) {
                $("#Button1").show()
                $("#validarToken").hide()
            }
            else {
                $("#errorMsj").show()
                setTimeout(() => {
                    $("#errorMsj").hide()
                }, 2000);
            }
            return false;
        }
    </script>

    <script>

            function VariableGs() {
                var Proce = "/Actualizar_Variable2";
                var Act = 'Java';
                var Prv_Adm = $('#MainContent_IdProveedor').val();
                var Fol_Adm = $('#MainContent_Folio').val();
                var NOC_Adm = $('#MainContent_FechaR').val();
                var Sts_Adm = $('#MainContent_FolioC').val();
                var Chk_Adm = $('#MainContent_FechaAp').val();
                var Fe1_Adm = $('#MainContent_FechaPago').val();
                var Fe2_Adm = $('#MainContent_total_').val();
                var url_list = NotificacionesWebService.get_path() + Proce;
                $.ajax({
                    type: "POST",
                    "beforeSend": function (xhr) {
                        xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                    },
                    url: url_list,
                    dataType: "json",
                    data: {
                        bts: Act,
                        Prv: Prv_Adm,
                        Fol: Fol_Adm,
                        NOC: NOC_Adm,
                        Sts: Sts_Adm,
                        Chk: Chk_Adm,
                        Fe1: Fe1_Adm,
                        Fe2: Fe2_Adm
                    }
                });
            }

            function Pregunta(folio, llave, Fila) {

                swal.fire({
                    title: "¿Desea Rechazar esta Solicitud de Cheque?",
                    text: "Una vez cancelada, no podrá volver actualizar el status de la Solicitud",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    dangerMode: true,
                })
                .then((willDelete) => {
                        if (willDelete) {
                            $('#myModal').modal('show');
                            $('#inputFolio').attr('value', folio);
                            $('#inputllave').attr('value', llave);
                            $('#inputFila').attr('value', Fila);
                        }
                        else {
                            folio = "";
                            llave = "";
                            Fila = "";
                            VariableGs();
                            setTimeout("location.href='AprobSolCheq'", 1);
                        }
                    });

                $('#Update').modal('close');
                $('#myModal').modal('close');
                return true;
            }

            function Pregunta2(folio, llave, Fila) {

                swal.fire({
                    title: "Confirmacion de Autorización",
                    text: "Ingrese su Token para confirmar",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    dangerMode: true,
                }).then((willDelete) => {
                        if (willDelete) {
                            NuevoTok(folio, llave, Fila);
                        }
                    });

                $('#Update').modal('close');
                $('#myModal').modal('close');
                return true;
            }

            async function NuevoTok(folio, llave, Fila) {
                try {

                    let variables;
                    variables = await obtenervariables();

                    if (variables.token == "enviado") {
                        Preguntas(folio, llave, Fila);
                    }
                    else {
                        swal.fire({
                            title: "Confirmacion de Autorizacion",
                            text: "Se ha generado un error al intentar obtener el token",
                            icon: "error",
                            confirmButtonText: 'Ok',
                            cancelButtonText: 'Cancelar',
                            confirmButtonColor: "#FF6600",
                            customClass: 'swal-wide',
                            closeModal: true,
                            closeOnConfirm: false,
                            showCancelButton: false,
                            dangerMode: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                            }
                        })
                    }

                }
                catch (err) {
                    return err.message;
                }
                return true;
            }

            async function obtenervariables() {
                var variablas = [];
                let variables;
                try {

                    var url_generar_solicitud = ContrarecibosWebService.get_path() + "/getrol";
                    variables = await $.ajax({
                        type: "POST",
                        beforeSend: function (xhr) {   //Include the bearer token in header
                            xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                        },
                        url: url_generar_solicitud,
                        dataType: "json",
                        data: {
                            ids: JSON.stringify(1),
                            comentario: ""
                        },
                        success: function (respuesta) {
                            if (respuesta.success == true) {
                                variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                                return variablas;
                            } else {
                                //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                            }
                        },
                        error: function (respuesta) {
                            //showToastError("No se pudo crear la solicitud de cheque.");
                            Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                        }
                    });

                }
                catch (err) {
                    return err.message;
                }
                return variables;
            }

            async function Preguntas(folio, llave, Fila) {

                const { value: password } = await swal.fire({
                    title: "Confirmacion de Autorizacion",
                    input: 'password',
                    inputLabel: 'Se ha enviado un Token a tu correo ,Ingresa el Token recibido',
                    inputPlaceholder: 'Ingresa Token',
                    customClass: 'swal-wide',
                    inputAttributes: {
                        maxlength: 20,
                        autocapitalize: 'off',
                        autocorrect: 'off'
                    },
                    icon: "success",
                    buttons: ["Cancelar", "Enviar"],
                    confirmButtonText: 'Enviar',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    allowOutsideClick: false,
                    closeOnConfirm: false,
                    showCancelButton: true,
                    dangerMode: true,
                })
                if (password) {
                    validatoken(password, folio, llave, Fila);
                }
                else {
                    swal.fire({
                        title: "Confirmacion de Autorizacion",
                        text: "Ingresa el token enviado a tu correo",
                        icon: "error",
                        confirmButtonText: 'Ok',
                        cancelButtonText: 'Cancelar',
                        confirmButtonColor: "#FF6600",
                        customClass: 'swal-wide',
                        closeModal: true,
                        closeOnConfirm: false,
                        showCancelButton: false,
                        dangerMode: true,
                    }).then((result) => {
                        if (result.isConfirmed) {
                        }
                    })
                }

                return true;
            }

            async function revisaSolicitud(ids) {
                var variablas = [];
                let variables;
                try {

                    var url_generar_solicitud = ContrarecibosWebService.get_path() + "/RevToken";
                    variables = await $.ajax({
                        type: "POST",
                        beforeSend: function (xhr) {   //Include the bearer token in header
                            xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                        },
                        url: url_generar_solicitud,
                        dataType: "json",
                        data: {
                            ids: ids,
                            comentario: ""
                        },
                        success: function (respuesta) {
                            if (respuesta.success == true) {
                                variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                                return variablas;
                            } else {
                                //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                            }
                        },
                        error: function (respuesta) {
                            //showToastError("No se pudo crear la solicitud de cheque.");
                            Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                        }
                    });

                }
                catch (err) {
                    return err.message;
                }
                return variables;
            }


            async function validatoken(password, folio, llave, Fila) {
                try {
                    let variables;
                    variables = await revisaSolicitud(password);
                    if (variables.token == "correcto") {
                        update(folio, llave, Fila);
                    }
                    else {
                        swal.fire({
                            title: "Confirmacion de Autorizacion",
                            text: "El token ingresado es incorrecto o ya expiro,intenta de nuevo o genera uno nuevo.",
                            icon: "error",
                            confirmButtonText: 'Generar Nuevo',
                            cancelButtonText: 'Cancelar',
                            confirmButtonColor: "#FF6600",
                            customClass: 'swal-wide',
                            closeModal: true,
                            closeOnConfirm: false,
                            showCancelButton: true,
                            dangerMode: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                swal.close();
                                nuevotoken(folio, llave, Fila);
                            }
                        })
                    }
                }
                catch (err) {
                    return err.message;
                }

                return true;
            }

            async function nuevotoken() {
                let variables
                variables = await nuevasvariables();
                if (variables.token == "enviado") {
                    Preguntas();
                }
                else {
                    swal.fire({
                        title: "Confirmacion de Autorizacion",
                        text: "Se ha generado un error al intentar obtener el token",
                        icon: "error",
                        confirmButtonText: 'Ok',
                        cancelButtonText: 'Cancelar',
                        confirmButtonColor: "#FF6600",
                        customClass: 'swal-wide',
                        closeModal: true,
                        closeOnConfirm: false,
                        showCancelButton: false,
                        dangerMode: true,
                    }).then((result) => {
                        if (result.isConfirmed) {
                        }
                    })
                }
        }
    
        async function update(lista_cheques) {            
            var folio, llave, Fila;        
            let variables;
            var i = 1;
            for (const cheque of lista_cheques) {
                folio = cheque.folio;
                llave = cheque.llave;
                Fila = i;
                try {
                    variables = await actualizarSolicitud(folio, llave, Fila);
                } catch (e) {
                    return e.message;
                }
                i++;
            }
        
                try {                   
                    if (variables.token == "actualizado") {
                        swal.fire({
                            title: "Confirmacion de Autorizacion",
                            text: "El token ingresado es correcto, haz autorizado la solicitud.",
                            icon: "success",
                            confirmButtonText: 'Ok',
                            cancelButtonText: 'Cancelar',
                            customClass: 'swal-wide',
                            closeModal: true,
                            closeOnConfirm: false,
                            showCancelButton: false,
                            allowOutsideClick: false,
                            dangerMode: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                VariableGs();
                                setTimeout("location.href='AprobSolCheq'", 1);
                            }
                        })
                    }
                    else {
                        swal.fire({
                            title: "Error de Autorizacion",
                            text: "Se ha generado un error al intentar actualizar el estado a la solicitud de cheque, intentalo de nuevo",
                            icon: "error",
                            confirmButtonText: 'Ok',
                            cancelButtonText: 'Ok',
                            confirmButtonColor: "#FF6600",
                            customClass: 'swal-wide',
                            closeModal: true,
                            closeOnConfirm: true,
                            showCancelButton: false,
                            dangerMode: true,
                        }).then((result) => {
                            if (result.isConfirmed) {

                            }
                        })
                    }
                }
                catch (err) {
                    return err.message;
                }
        }

            async function actualizarSolicitud(folio, llave, fila) {
            
                var variablas = [];
                let variables;
                try {

                    var url_generar_solicitud = ContrarecibosWebService.get_path() + "/updateSOL2";
                    variables = await $.ajax({
                        type: "POST",
                        beforeSend: function (xhr) {   //Include the bearer token in header
                            xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                        },
                        url: url_generar_solicitud,
                        dataType: "json",
                        data: {
                            folio: folio,
                            llave: llave,
                            fila: fila
                        },
                        success: function (respuesta) {
                            if (respuesta.success == true) {
                                variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                                return variablas;
                            } else {
                                //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                            }
                        },
                        error: function (respuesta) {
                            //showToastError("No se pudo crear la solicitud de cheque.");
                            Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                        }
                    });

                }
                catch (err) {
                    return err.message;
                }
            return variables;
            }
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo);
        }
    </script>
    <br />
    <br />
    <br />
    <br />
    <div class="col-lg-11 col-sm-11 col-xs-11">
        <label class="col-form-label col-lg-5 col-sm-3 col-xs-1"></label>
        <div class="form-group col-lg-6 col-sm-8 col-xs-10">
            <h3>Aprobacion Solicitud de Cheques</h3>
        </div>
    </div>

    <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
    <ContentTemplate>   

  <%-- 
    <div class="col-xs-12 md-3 col-lg-3">
        <label>Token Vigente</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" ClientIDMode="Static" AutoCompleteType="Disabled" ID="tbx_token" class="form-control" TabIndex="3"  ReadOnly="true" BackColor="White"/>
    </div--%>>        
   

    <div class="col-xs-12 md-3 col-lg-3">
    <label>Proveedor</label>
     <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="IdProveedor" class="form-control" TabIndex="3" />
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Folio Factura</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="Folio" class="form-control" TabIndex="3" />
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Fecha Recepcion</label>
        <asp:TextBox type="date" name="fecha" ID="FechaR" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Folio Contrarrecibo</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="FolioC" class="form-control" TabIndex="3" />
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Fecha Aprobacion</label>
        <asp:TextBox type="date" name="fecha" ID="FechaAp" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Fecha Programada de Pago</label>
        <asp:TextBox type="date" name="fecha" ID="FechaPago" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
    </div>

    <div class="col-xs-12 md-3 col-lg-3">
        <label>Total</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="total_" class="form-control" TabIndex="3" />
    </div>


    <div class="col-xs-11 col-sm-11 col-md-11">
        <div class="form-group row">
            <label class="col-form-label col-xs-12 col-sm-12 col-md-12"></label>
            <div class="col-xs-4 col-sm-2 col-md-1">
                <asp:Button ID="Buscar" runat="server" Text="Buscar" OnClick="Buscar_Click1" CssClass="btn btn-primary" title="Generar Busqueda" />
            </div>

            <div class="col-xs-4 col-sm-2 col-md-1">
                <asp:Button ID="Button2" runat="server" Text="Limpiar" OnClick="Limpiar_Click1" CssClass="btn btn-tsys" title="Limpiar Busqueda" />
            </div>

            <div class="col-xs-4 col-sm-2 col-md-1">                
                <asp:Button ID="btn_carga_masiva" runat="server" Text="Carga Masiva" OnClientClick="$('#passwordValue').val(''); $('#myModal_Aprobar').modal(); $('#validarToken').show(); $('#Button1').hide();" CssClass="btn btn-tsys" title="Carga Masiva" />
            </div>
        </div>
    </div>

    <div class="col-sm-9">
        <div class="alert alert-block alert-danger" id="AL" style="display: none">
          <h5>¡Error!</h5>
          <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
        </div>
    </div>
 
    <div class="col-sm-9">
       <div class="alert alert-block alert-danger" id="AL1" style="display: none">
        <h5>¡Error!</h5>
        <asp:Label ID="lblMsj" runat="server" Text=""></asp:Label>
       </div>
    </div>

    <div class="col-sm-9">
       <div class="alert alert-block alert-danger" id="AL3" style="display: none">
        <h5>¡Se ha Rechazado la Factura!</h5>
<%--        <asp:Label ID="lblMsj2" runat="server" Text=""></asp:Label>--%>
       </div>
    </div>


    <div class="col-sm-9">
       <div class="alert alert-block alert-success" id="AL2" style="display: none">
         <h5>¡Exito!</h5>
         <asp:Label ID="lblMsj1" runat="server" Text=""></asp:Label>
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



<%--    <asp:Panel ID="Panel1" runat="server" BorderWidth="0">--%>

    <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto"  AutoGenerateColumns="False" CellPadding="4" OnRowCommand="GridView1_RowCommand"  >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <Columns>    
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px" HeaderText="Seleccionar">
                  <ItemTemplate>
                    <asp:CheckBox ID="Check" CssClass="ChkBoxClass" Style="width: 20px; height: 20px;" runat="server" AutoPostBack="true"/>
                  </ItemTemplate>
                </asp:TemplateField>
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID.Factura" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="Vendor" HeaderText="Proveedor" ReadOnly="True" SortExpression="VendId" />
         <asp:BoundField DataField="RFC" HeaderText="RFC" ReadOnly="True" SortExpression="OC" />
         <asp:BoundField DataField="Terminos" HeaderText="Terminos de Pago" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="FolioF" HeaderText="Folio de Factura" ReadOnly="True" SortExpression="FechaTransaccion" />
         <asp:BoundField DataField="FechaR" HeaderText="Fecha de Recepción" ReadOnly="True" SortExpression="FechaCarga" DataFormatString = "{0:dd/MM/yyyy}" />
         <asp:BoundField DataField="FolioC" HeaderText="Folio Contrarrecibo" ReadOnly="True" SortExpression="Status" />
         <asp:BoundField DataField="FechaAP" HeaderText="Fecha de Aprobacion" ReadOnly="True" SortExpression="Status" DataFormatString = "{0:dd/MM/yyyy}" />
         <asp:BoundField DataField="FechaPago" HeaderText="Fecha Programada Pago" ReadOnly="True" SortExpression="Status" DataFormatString = "{0:dd/MM/yyyy}" />
         <asp:BoundField DataField="Total" HeaderText="Total" ReadOnly="True" SortExpression="Status" DataFormatString = "{0:C2}" />
         <asp:BoundField DataField="Aprobador" HeaderText="Aprobador" ReadOnly="True" SortExpression="Status" />
         
<%--      <asp:TemplateField>
          <ItemTemplate>
           <asp:Button ID="Documento_1" CssClass="btn btn-default" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" Visible='<%# Eval("Tipo").ToString() == "Extranjero" ? false : true %>' />
          </ItemTemplate> 
           </asp:TemplateField>--%>
            
             <asp:TemplateField>
              <ItemTemplate>
            <asp:Button ID="Documento_2" CssClass="btn btn-default" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Comprobante PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Documento_3" CssClass="btn btn-default" runat="server" CommandName="Documento_3" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Anexo PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         <%-- <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Aprobar" CssClass="btn btn-default cargar" runat="server" CommandName="Aprobar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Aprobar" />
          </ItemTemplate>
         </asp:TemplateField>--%>
             <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Cancelar" CssClass="btn btn-default" runat="server" CommandName="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Rechazar" />
          </ItemTemplate>
         </asp:TemplateField> 

        <asp:BoundField DataField="Tipo" HeaderText="Tipo" ReadOnly="True" SortExpression="Tipo" Visible="false" />
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

    <br />

    <asp:GridView ID="gvValidacion" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto" CellPadding="4" >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="Tomato" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <RowStyle BackColor="White" ForeColor="DarkRed" /> 
        <Columns>
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="folio" HeaderText="folio" ReadOnly="True" SortExpression="folio" />
         <asp:BoundField DataField="UUID" HeaderText="UUID" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="NodeOc" HeaderText="O.C." ReadOnly="True" SortExpression="NodeOc" />
         <asp:BoundField DataField="FechaTransaccion" HeaderText="Fecha" ReadOnly="True" SortExpression="FechaError" />
         <asp:BoundField DataField="ErrorValidacion" HeaderText="ErrorValidacion" ReadOnly="True" SortExpression="ErrorValidacion" />
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


   </ContentTemplate>
   <Triggers>
    <asp:AsyncPostBackTrigger controlid="Buscar" eventname="Click" />
    <asp:AsyncPostBackTrigger controlid="Buscar" eventname="Click" />
<%--    <asp:AsyncPostBackTrigger ControlID="gvFacturas" eventname="RowCommand"  />--%>
    <asp:PostBackTrigger ControlID="gvFacturas" />
    </Triggers>
  </asp:UpdatePanel>

            <div id="Update" style="display:none">
            </div>

            <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
                <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel">¿Desea Anexar comentario de Rechazo para  el Empleado?</h4>
                    </div>
                    <div class="modal-body">

                            <div class="container-fluid">
                                <div class="row">
                                    Agregar Comentario:
                                    <div class="col-md-12">
                                           <div class="form-group">
                                               <asp:TextBox ID="inputComentario" MaxLength="200" runat="server" CssClass="form-control" AutoComplete = "off" AutoCompleteType="Disabled"  ></asp:TextBox>
                                               <input type="hidden" id="inputFolio" /> 
                                               <input type="hidden" id="inputllave" /> 
                                               <input type="hidden" id="inputFila" /> 
                                            </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button id="btn_No_Send" type="button" title="No Enviar" class="btn btn-tsys pull-left" data-dismiss="modal">No Enviar
                            </button>
                           

                            <button id="btn_send_comments" class="btn btn-primary" title="Enviar" data-toggle="tooltip">
                                Si Enviar
                            </button>
                        </div>
                </div>
            </div>
            </div>

            <%--  Modal para generar token --%>
            <div class="modal fade" id="myModal_Aprobar" tabindex="-1" role="dialog" aria-labelledby="myModalLabel1">
                <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel1">Teclee el password para verificar su identidad</h4>
                    </div>
                    <div class="modal-body">
                            <div class="container-fluid">
                                <div class="row">
                                    Inserte el token:
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <asp:TextBox runat="server" type="text" AutoComplete = "off" ClientIDMode="Static" AutoCompleteType="Disabled" ID="passwordValue" class="form-control" TabIndex="3" />
                                            <p id="errorMsj" style="color: red;" hidden>Password Incorrecto</p>                                              
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button id="btn_No_Send1" type="button" title="No Enviar" class="btn btn-tsys pull-left" data-dismiss="modal">Cancelar
                            </button>                           
                            <asp:Button ID="validarToken" runat="server" Text="Validar token" ClientIDMode="Static" OnClientClick="return showModal()" CssClass="btn btn-primary" title="validar Token" />
                            <asp:Button ID="Button1" runat="server" Text="Aprobar" ClientIDMode="Static" OnClick="btn_carga_masiva_Click" CssClass="btn btn-primary cargaMasiva" title="Aprobar" data-toggle="tooltip"/>
                        </div>
                </div>
            </div>
            </div>
        

            

</asp:Content>