﻿<%@ Page Title="Administrar Usuario" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Manage.aspx.cs" Inherits="Account_Manage" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="Cache-Control" content="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <script src="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script>
        //function alertme(titulo, mesaje, Tipo) {
        //    swal(titulo, mesaje, Tipo)
        //}
        function alertme(titulo, mesaje, Tipo) {
            swal(titulo, mesaje, Tipo)
            unblockUI();
            return true;
        }
    </script>
    <script>
        function alert(campo) {
            $(campo).toggle();
        }
    </script>
    <br />
    <br />
    <br />
    <br />
    <div>
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>

    </div>
    <br />
    <br />
    <br />

    <ul class="nav nav-pills">
        <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Cambiar contraseña</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Seguridad</a>
        </li>
    </ul>

    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane container active" id="home">
            <div class="row">
                <div class="col-md-12">
                    <section id="passwordForm">
                        <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
                            <p>
                                You do not have a local password for this site. Add a local
                        password so you can log in without an external login.
                            </p>
                            <div class="form-horizontal">
                                <hr />
                                <br />
                                <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="password" CssClass="col-md-2 control-label">Password</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="password" TextMode="Password" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="password"
                                            CssClass="text-danger" ErrorMessage="The password field is required."
                                            Display="Dynamic" ValidationGroup="SetPassword" />
                                        <asp:ModelErrorMessage runat="server" ModelStateKey="NewPassword" AssociatedControlID="password"
                                            CssClass="text-danger" SetFocusOnError="true" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="confirmPassword" CssClass="col-md-2 control-label">Confirm password</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="confirmPassword" TextMode="Password" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="confirmPassword"
                                            CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required."
                                            ValidationGroup="SetPassword" />
                                        <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="confirmPassword"
                                            CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match."
                                            ValidationGroup="SetPassword" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-md-offset-2 col-md-10">
                                        <asp:Button runat="server" Text="Set Password" ValidationGroup="SetPassword" OnClick="SetPassword_Click" CssClass="btn btn-default" />
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <div class="col-lg-12 col-sm-12 col-xs-12">
                            <label class="col-form-label col-lg-5 col-sm-3 col-xs-1"></label>
                            <div class="form-group col-lg-6 col-sm-8 col-xs-10">
                                <h3>Administración de Usuario</h3>
                            </div>
                        </div>


                        <asp:PlaceHolder runat="server" ID="changePasswordHolder" Visible="false">
                            <p>Actualmente estás Loggueado como : <strong><%: User.Identity.GetUserName() %></strong>.</p>
                            <div class="form-horizontal">
                                <br />
                                <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                                <div class="form-group">
                                    <asp:Label runat="server" ID="CurrentPasswordLabel" AssociatedControlID="CurrentPassword" CssClass="col-md-2 control-label">Contraseña Actual</asp:Label>
                                    <div class="col-md-4">
                                        <asp:TextBox runat="server" ID="CurrentPassword" TextMode="Password" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="CurrentPassword"
                                            CssClass="text-danger" ErrorMessage="Este campo es requerido"
                                            ValidationGroup="ChangePassword" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" ID="NewPasswordLabel" AssociatedControlID="NewPassword" CssClass="col-md-2 control-label">Contraseña Nueva</asp:Label>
                                    <div class="col-md-4">
                                        <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="NewPassword"
                                            CssClass="text-danger" ErrorMessage="Este campo es Requerido"
                                            ValidationGroup="ChangePassword" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" ID="ConfirmNewPasswordLabel" AssociatedControlID="ConfirmNewPassword" CssClass="col-md-2 control-label">Confirme la nueva Contraseña</asp:Label>
                                    <div class="col-md-4">
                                        <asp:TextBox runat="server" ID="ConfirmNewPassword" TextMode="Password" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmNewPassword"
                                            CssClass="text-danger" Display="Dynamic" ErrorMessage="La Confirmacion de Contraseña es Requerido."
                                            ValidationGroup="ChangePassword" />
                                        <asp:CompareValidator runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                            CssClass="text-danger" Display="Dynamic" ErrorMessage="La nueva contraseña y la confirmación no coinciden, verificalo"
                                            ValidationGroup="ChangePassword" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-offset-2 col-md-10">
                                        <asp:Button runat="server" Text="Guardar Cambios" OnClick="ChangePassword_Click" CssClass="btn btn-primary" ValidationGroup="ChangePassword" />
                                    </div>
                                </div>
                            </div>

                            <div class="alert alert-danger" id="AL" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                                <h5>¡Error!</h5>
                                <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                            </div>
                        </asp:PlaceHolder>
                    </section>
                </div>
            </div>
        </div>

        <div class="tab-pane container fade" id="menu1">
            <br />
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BntToken" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div class="row" id="Seguridad" runat="server">
                        <div class="form-group row">
                            <label class="col-md-2 control-label">Generar Token:</label>
                            <div class="col-md-4 col-md-2">
                                <asp:Button runat="server" ID="BntToken" Text="Generar token" CssClass="btn btn-primary" OnClick="GenerarToken_Click" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
