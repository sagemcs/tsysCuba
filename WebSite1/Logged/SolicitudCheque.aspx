<%@ Page Title="Generar Solicitud de Chueque" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="SolicitudCheque.aspx.cs" Inherits="Logged_SolicitudCheque" %>
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
        <asp:ServiceReference  Path="~/Servicios/ContrarecibosWebService.asmx" />
              
    </Services>
         <Scripts>
            <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
             <asp:ScriptReference   Path="~/Scripts/data-tables-solicitudcheque-contrarecibos.js" />
        </Scripts>
      
    </asp:ScriptManagerProxy>
    
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Generar solicitud de cheque</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-2">
                    
                        <div class="form-group">
                            
                            <asp:TextBox ID="inputFolio"  placeholder="Folio" runat="server" CssClass="form-control filter" ToolTip="Folio"></asp:TextBox>   
                         
                        </div>
                    </div>
                   <div class="col-md-2">
                    
                        <div class="form-group">
                            
                            <asp:DropDownList ID="comboProveedores" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                           
                        </div>
                    </div>
                   
                   <div class="col-md-2">
                    
                        <div class="form-group">
                            
                        <asp:TextBox ID="inputRFC"  placeholder="RFC" runat="server" CssClass="form-control filter" ToolTip="RFC"></asp:TextBox>   
                         
                        </div>
                    </div>
                    
                   <div class="col-md-2">
                    
                        <div class="form-group">
                            
                        <asp:TextBox ID="inputTotal"  placeholder="Total" runat="server" CssClass="form-control filter" ToolTip="Total"></asp:TextBox>   
                         
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            
                        
                              <asp:TextBox ID="inputFechaRecepcion"  placeholder="Fecha recepción" runat="server"  ToolTip="Fecha recepción" CssClass="form-control datepicker filter"></asp:TextBox>
                       
                        </div>
                    </div>
                    <div class="col-md-2">
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
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Folio</th>
                        <th>Proveedor</th>
                        <th>RFC</th>
                        <th>Condiciones</th>
                        <th>Fecha recepción</th>
                        <th>Fecha programada</th>
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
                    <button id="btn-generar" href="#" type="button" class="btn btn-primary generar" title="Generar solicitud de cheque" data-toggle="tooltip">
                        Generar solicitud
                    </button>
                </div>
            </div>
        </div>
       
        <asp:HiddenField ID="contrarecibos_seleccionados" runat="server" />
       
        
       <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel">Comentario</h4>
                    </div>
                    <div class="modal-body">

                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-md-12">
                                           <div class="form-group">
                                               <asp:TextBox ID="inputComentario" MaxLength="255" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                              
                                            </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger pull-left" data-dismiss="modal"></i>Cancelar
                            </button>
                           

                            <button class="btn btn-primary" title="Generar solicitud de cheque" data-toggle="tooltip">
                                Aceptar
                            </button>
                        </div>
                </div>
            </div>
        </div>
    </div>


     

</asp:Content>

