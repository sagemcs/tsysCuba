<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"CodeFile="FacturasEstadoFilter.aspx.cs" Inherits="Logged_ReportsFilter_FacturasEstadoFilter" %>
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
                <asp:ServiceReference Path="~/Servicios/FacturasWebService.asmx" /> 
         </Services>
        <Scripts>
            <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
            <asp:ScriptReference   Path="~/Scripts/data-tables-facturas-estado.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
  
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Estado de Facturas</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">          
                <div class="row">
                    <div class="col-md-3">                    
                        <div class="form-group">                            
                       <asp:TextBox ID="inputFolio" MaxLength="40"  placeholder="Folio" runat="server" CssClass="form-control filter" ToolTip="Folio"></asp:TextBox>   
                         </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">                            
                            <asp:DropDownList ID="comboProveedores" placeholder="Proveedor"  runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                           
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                              <asp:TextBox ID="inputFecha"  placeholder="Fecha de recepción" runat="server"  ToolTip="Fecha" CssClass="form-control datepicker filter"></asp:TextBox>                       
                        </div>
                    </div>
                </div>
                <div class="row">                    
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:TextBox ID="inputContrarecibo" MaxLength="10"  placeholder="Contrarecibo" runat="server" CssClass="form-control filter"  ToolTip="Contrarecibo"></asp:TextBox>      
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:TextBox ID="inputSolicitud" MaxLength="25"  placeholder="Solicitud de Cheque" runat="server" CssClass="form-control filter" ToolTip="Solicitud"></asp:TextBox>                          
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:DropDownList ID="comboEstado" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                        </div>
                    </div>
          
                    <div class="col-md-3" >
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
                        <th></th>
                        <th>Folio</th>
                        <th>Serie</th>    
                        <th>Proveedor</th>
                        <th>Fecha de factura</th>
                        <th>Fecha de recepción</th>
                        <th>Total</th>
                        <th>Contrarecibo</th>
                        <th>Solicitud</th>
                        <th>Estado</th>
                        <th>Estado</th>
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
