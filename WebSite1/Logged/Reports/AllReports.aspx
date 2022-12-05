<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AllReports.aspx.cs" Inherits="Logged_Reports_AllReports" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<asp:Content ID="Content3" ContentPlaceHolderID="CustomStyles" Runat="Server">
    <!--Version 31-Mayo-2019 By Luis Angel-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <link href ="../../Css/reports.css" rel="stylesheet" type ="text/css" />
    <link rel="stylesheet" href="~/Css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Css/bootstrap-theme.min.css" />
    <script src="../../Scripts/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/print-report.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <br/> <br/> <br/> <br /> <br/>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
        <ContentTemplate>
            <%--Fila de filtros--%>
            <div class="row">

                <%-- Estado--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Estado:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:DropDownList ID="drop_status" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="False">
                            <asp:ListItem Value="0">Todos </asp:ListItem>
                            <asp:ListItem Value="1">Pendiente</asp:ListItem>
                            <asp:ListItem Value="2">Aprobado</asp:ListItem>
                            <asp:ListItem Value="3">Cancelado</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%-- Documento--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Tipo de documento:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:DropDownList ID="drop_docs" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="False">
                            <asp:ListItem Value="0">Reembolso de Gastos </asp:ListItem>
                            <asp:ListItem Value="1">Anticipo de Gastos</asp:ListItem>
                            <asp:ListItem Value="2">Gastos Medicos Menores</asp:ListItem>
                            <asp:ListItem Value="3">Gasto Tarjeta Corporativa</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%-- Empleados--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Empleados:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:DropDownList ID="drop_empleados" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drop_empleados_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <%--Fecha de Inicio--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Fecha de Creación:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:TextBox type="date" name="fecha" ID="tbx_fecha_inicio" AutoComplete="off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" AutoPostBack="False" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="drop_empleados" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <br/>

    <%--<div class="col-xs-11 col-sm-11 col-md-11">--%>
        <div class="form-group row">
            <label class="col-form-label col-xs-12 col-sm-12 col-md-12"></label>
            <div class="col-xs-4 col-sm-2 col-md-2">
                <asp:Button ID="btn_filtrar" runat="server" Class="btn btn-tsys cargar" Text="Borrar Filtros" OnClick="btn_filtrar_Click" />
            </div>

            <div class="col-xs-4 col-sm-2 col-md-2">
                <asp:Button ID="btn_generar" runat="server" Class="btn btn-primary" Text="Generar Reporte" OnClick="btn_generar_Click" />
            </div>
        </div>
    <%--</div>--%>
    <%--<hr/>--%>

    
    <div id = "dvReport"  style="position: relative; left: 5%;" > 
        <CR:CrystalReportViewer ID="Reporte_Anticipos" runat="server" AutoDataBind="true" BestFitPage="False" DisplayStatusbar="False" HasCrystalLogo="False" HasDrilldownTabs="False" HasToggleGroupTreeButton="False" HasToggleParameterPanelButton="False" ToolPanelView="None" ClientIDMode="Inherit" PrintMode="pdf" HasPrintButton="True" />
    </div>
    
<script>
        $(document).ready(function () {
            $('#Reporte_Facturas_toptoolbar_print').click(function (e) {
                e.preventDefault();
                Print();
            });

            $('#btn_print').click(function (e) {
                e.preventDefault();
                Print();
            });
        });
</script>
</asp:Content>
