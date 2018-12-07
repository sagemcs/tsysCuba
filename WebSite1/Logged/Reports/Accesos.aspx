﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Accesos.aspx.cs" Inherits="Logged_Reports_AccesosB" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomStyles" Runat="Server">
    <link href ="../../Css/reports.css" rel="stylesheet" type ="text/css" />
    <link rel="stylesheet" href="../../Css/bootstrap.min.css" />
    <link rel="stylesheet" href="../../Css/bootstrap-theme.min.css" />
    <script src="../../Scripts/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/print-report.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <br/> <br/> <br/> <br/><div>
        <input id="btn_print" class="btn btn-primary" type="button" value="Imprimir" onclick="Print()" style="position: relative; left: 45%;"  /> 
        </div><br/>
        <div id = "dvReport"  style="position: relative; left: 10%;" >
            <CR:CrystalReportViewer ID="Reporte_Accesos" runat="server" AutoDataBind="true" BestFitPage="False" DisplayStatusbar="False" HasCrystalLogo="False" HasDrilldownTabs="False" HasToggleGroupTreeButton="False" HasToggleParameterPanelButton="False" ToolPanelView="None" ClientIDMode="Inherit" PrintMode="pdf" HasPrintButton="True" />
        </div>
    <script>
        $(document).ready(function () {
            $('#Reporte_Accesos_toptoolbar_print').click(function (e) {
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
