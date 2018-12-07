﻿<%@ Page Title="Compañias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="CompaniesFilter.aspx.cs" Inherits="Logged_ReportsFilter_CompaniesFilter" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
      <script src="/Scripts/dataTable-1.10.16/datatables.min.js"></script>
    <script src="/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js"></script>
    <script src="/Scripts/data-tables-empresas.js"></script>
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Compañias</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    
                    <div class="col-md-5">
                        <div class="form-group">
                            
                        <asp:TextBox ID="inputNombre" runat="server" CssClass="form-control" ToolTip="NOmbre"></asp:TextBox>   
                         
                        </div>

                    </div>

                    <div class="col-md-5">
                        <div class="form-group">
                            
                            <asp:TextBox ID="inputRFC" runat="server" CssClass="form-control" ToolTip="RFC"></asp:TextBox>   
                         
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
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Nombre</th>
                        <th>RFC</th>
                        <th>Fecha</th>
                        <th>Estado</th>
                    </tr>
                    </thead>
                    <tbody>

                    </tbody>
                </table>
            </div>
        </div>
        <div class="row">
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