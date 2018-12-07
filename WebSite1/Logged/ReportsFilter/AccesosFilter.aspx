<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AccesosFilter.aspx.cs" Inherits="Logged_ReportsFilter_AccesosFilter" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<asp:Content ID="CustomStyles" ContentPlaceHolderID="CustomStyles" runat="server">
    <link href="../../Css/reports-filter.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
         <Services>
                <asp:ServiceReference Path="~/Servicios/AccesosWebService.asmx" />              
            </Services>
         <Scripts>
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
             <asp:ScriptReference   Path="~/Scripts/data-tables-accesos.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
   
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Accesos</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-3">
                    
                        <div class="form-group">
                            
                            <asp:TextBox ID="inputUserID"  placeholder="UserID" runat="server" CssClass="form-control filter" ToolTip="UserID" MaxLength="256"></asp:TextBox>   
                            
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            
                        <asp:TextBox ID="inputUsername"  placeholder="Usuario" runat="server" CssClass="form-control filter" ToolTip="Usuario" MaxLength="256"></asp:TextBox>   
                            
                        </div>

                    </div>

                    <div class="col-md-3">
                        <div class="form-group">
                            
                            <asp:TextBox ID="inputIP"  placeholder="IP" runat="server" CssClass="form-control filter" ToolTip="IP" MaxLength="39"></asp:TextBox>
                        </div>
                    </div>


          
                    <div class="col-md-3">
                        <div class="form-group pull-left">
                            <a href="#" class="btn btn-success buscar" title="Buscar" data-toggle="tooltip">
                               Buscar
                            </a>
                            <a href="#" class="btn btn-primary limpiar" title="Limpiar filtro" data-toggle="tooltip">
                                Limpiar
                            </a>
          
                        </div>
                    </div>

                </div>
          
            </div>


        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th>Usuario</th>
                        <th>Nombre</th>
                        <th>Fecha</th>
                        <th>IP</th>
                    </tr>
                    </thead>
                    <tbody>

                    </tbody>
                </table>
            </div>
        </div>
        <div class="row" style="margin-top: 15px;">
            <div class="col-md-12">
                <div class="pull-right">
                    <button href="#" class="btn btn-primary generar" title="Generar reporte" data-toggle="tooltip">
                        Generar reporte
                    </button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>