﻿<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Contrarecibo.aspx.cs" Inherits="Logged_Contrarecibo" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<asp:Content ID="CustomStyles" ContentPlaceHolderID="CustomStyles" runat="server">
    <link href="../Css/reports-filter.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  <asp:ScriptManagerProxy  ID="ScriptManagerProxy1" runat="server">
      <Services>
        <asp:ServiceReference  Path="~/Servicios/FacturasWebService.asmx" />
              
    </Services>
         <Scripts>
            <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
             <asp:ScriptReference   Path="~/Scripts/data-tables-contrarecibo-facturas.js" />
        </Scripts>
      
    </asp:ScriptManagerProxy>
    
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Generar contrarecibo</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                           
                            <asp:DropDownList ID="comboProveedores" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                           
                        </div>

                    </div>
                   <div class="col-md-3">
                        <div class="form-group">
                          
                            <asp:TextBox ID="inputFecha"  placeholder="Fecha" runat="server" CssClass="form-control datepicker filter"></asp:TextBox>
                       
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
        <div class="row" style="margin-top: 15px;">
            <div class="col-md-12">
               <span  class="pull-right" style="color: green;">
                  Importe facturas seleccionadas $<strong id="importe-total"style="margin-left:3px;width:80px;font-size: 16px;">0.00</strong>

               </span> 
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Compañía</th>
                        <th>Folio</th>
                        <th>Serie</th>
                        
                        <th>Fecha</th>
                        <th>Proveedor</th>
                        <th>Subtotal</th>
                        <th>Retenciones</th>
                        <th>Traslados</th>
                        <th>Total</th>
                         <th>Crédito/Débitos</th>
                        <th>Importe</th>
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
                    <button href="#" class="btn btn-primary generar" title="Generar contrarecibo" data-toggle="tooltip">
                        Generar contrarecibo
                    </button>
                </div>
            </div>
        </div>
       
        <asp:HiddenField ID="facturas_seleccionadas" runat="server" />

       <div class="modal fade" id="modalDetallesNota" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel">Notas</h4>
                    </div>
                    <div class="modal-body">

                            <div class="container-fluid">
                                <div class="row">
                                    <div id="modal-content-table" class="col-md-12">
                                          
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"></i>Cerrar
                            </button>
                           

                        </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
