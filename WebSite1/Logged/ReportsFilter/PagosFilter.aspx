<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="PagosFilter.aspx.cs" Inherits="Logged_ReportsFilter_PagosFilter" %>
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
                <asp:ServiceReference Path="~/Servicios/PagosWebService.asmx" /> 
         </Services>
        <Scripts>
            <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
            <asp:ScriptReference   Path="~/Scripts/data-tables-pagos.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Pagos</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-3">
                    
                        <div class="form-group">
                            
                         <asp:TextBox ID="inputFolio" MaxLength="40"  placeholder="Folio" runat="server" CssClass="form-control filter"  ToolTip="Folio"></asp:TextBox>   
                         
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            
                         <asp:TextBox ID="inputSerie" MaxLength="25"  placeholder="Serie" runat="server" CssClass="form-control filter"  ToolTip="Serie"></asp:TextBox>   
                         
                        </div>

                    </div>

                    <div class="col-md-3">
                        <div class="form-group">
                            
                              <asp:TextBox ID="inputFecha"  placeholder="Fecha" runat="server" ToolTip="Fecha" CssClass="form-control datepicker filter"></asp:TextBox>
                        
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:DropDownList ID="comboProveedores" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                           
                        </div>

                    </div>

                </div>
                <div class="row">
                    
                    <div class="col-md-3">
                        <div class="form-group">                          
                         <asp:TextBox ID="inputTotal"  placeholder="Total" runat="server" CssClass="form-control filter"  ToolTip="Total"></asp:TextBox>   
                         <span id="error_inputTotal" class="no-valid-message">Dato no válido</span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">                            
                         <asp:TextBox ID="inputUUID" MaxLength="36" placeholder="UUID" runat="server" CssClass="form-control filter"  ToolTip="UUID"></asp:TextBox>   
                            
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
                        
                        <th>Fecha</th>
                        <th>Proveedor</th>
                        <th>Subtotal</th>
                        <th>Total</th>
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

