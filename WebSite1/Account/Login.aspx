﻿<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" Async="true" %>
<%@ Register Assembly="GoogleReCaptcha" Namespace="GoogleReCaptcha" TagPrefix="cc1" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
<script src ="../Css/sweetalert.min.js" type="text/javascript"></script>
<link href="../Css/SLoging.css" rel="stylesheet" type ="text/css" />
<link href ="../Css/Normalize.css" rel="stylesheet" type ="text/css" />


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


    <div class="Margen">



        <div class="Base">


            <input id="tab-1" type="radio" name="tab" class="Login" checked><label for="tab-1" class="tab">Log In</label>
            <input id="tab-2" type="radio" name="tab" class="Registro"><label for="tab-2" class="tab" style="display: none">Registrarme</label>

            <div id="Formulario1" class="Formulario">

                <div class="VLogin">





                    <div class="Bloque">
                        <label for="user" class="Etiqueta">Usuario</label>
                        <asp:TextBox runat="server" ID="UserName1" type="text" CssClass="form-control " MaxLength="80" name="Nombre" />
                    </div>
                    <div class="Bloque">
                        <label for="pass" class="Etiqueta">Contraseña</label>
                        <asp:TextBox runat="server" ID="Password1" type="password" MaxLength="14" CssClass="form-control " data-type="password" />
                    </div>




                    <div class="Bloque">
                        <asp:label runat ="server" ID="LEmpresas" class="Etiqueta">Seleccione Empresa:</asp:label>
                        <asp:DropDownList ID="EmpresaID" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="False"></asp:DropDownList>
                    </div>



                    <div class="Bloque">
                        <asp:CheckBox runat="server" ID="RememberMe" type="checkbox" class="switch" Checked="false" />
                        <label for="check">Recordar Usuario</label>
                    </div>



<%--                  <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
                  <Triggers>
                      <asp:AsyncPostBackTrigger controlid="Uno" eventname="Click" />
                      <asp:AsyncPostBackTrigger controlid="Reset" eventname="Click" />
                       <asp:AsyncPostBackTrigger ControlID="EmpresaID" EventName="selectedindexchanged" />
                    </Triggers>
          
                  <ContentTemplate>--%>


                    <div class="Bloque">
                        <asp:Button runat="server" ID="Uno" class="Btn" Text="Iniciar Sesion" OnClick="LogIn" />
                    </div>

<%--                    </ContentTemplate>
                    </asp:UpdatePanel>--%>


                    <div class="hr"></div>
                    <div class="foot-lnk">
                        <asp:Button runat="server" Text="Olvidó su Contraseña?" id="Reset"  AutoPostBack="False" CssClass="btn btn-link" OnClick="Unnamed1_Click" />
                    </div>




                </div>
                <div class="VRegistro">
                    <div class="Bloque">
                        <h3>Restablecer Contraseña</h3>
                    </div>
                    <div class="Bloque">
                        <p>
                            Escriba su dirección de Correo electronico para iniciar la recuperación de la contraseña
                        </p>
                    </div>
                    <div class="Bloque">
                        <label for="pass" class="help-block">Usuario</label>
                        <asp:TextBox runat="server" ID="UserName" type="text" MaxLength="100" CssClass="form-control input-lg" name="Contra" />
                    </div>

                    <div class="Bloque" id="Capcha">
                        <cc1:GoogleReCaptcha ID="ctrlGoogleReCaptcha" runat="server" PublicKey="6LfVDgMTAAAAAJnH9GV0i7r_Pl3FfC_hyfh2Cgnq" PrivateKey="6LfVDgMTAAAAAPfTlH1n7z72CvS46c2C_qkTmFsZ" />
                    </div>
                    <asp:Label runat="server" ID="Capt" Style="display: none">Captcha Invalido!</asp:Label>

                    <div class="Bloque" id="BRst">
                        <div class="col-xs-6 col-md-6">
                            <asp:Button runat="server" Text="Regresar" class="btn btn-link" />
                        </div>
                        <div class="col-xs-6 col-md-6">
                            <asp:Button runat="server" Text="Enviar" class="btn btn-primary btn-block" ID="btnEnv" OnClick="btnVer_Click" />
                        </div>
                    </div>
                </div>

                <div class="alert alert-block alert-danger" id="Al" style="display: none">
                    <h5>¡Error!</h5>
                    Captcha Invalido!!
                </div>

            </div>


        </div>


    </div>



</asp:Content>


