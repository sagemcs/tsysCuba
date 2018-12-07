
var table;
$(document).ready(function () {
    hide_combo_proveedores(PagosWebService.get_path());
    var url_list = PagosWebService.get_path() + "/listar";
   
    table = $('#list').DataTable({
        language: window.datatableLang,
        "ajax": {
            "url": url_list,
            "type": "POST",
            "beforeSend": function (xhr) {
                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
            },
            "draw": 1,
            "data": function (data) {
                delete data.columns;
                //data.VendID = $('#MainContent_comboProveedores').val();
                data.Folio = $('#MainContent_inputFolio').val();
                data.Serie = $('#MainContent_inputSerie').val();
                data.Fecha = $('#MainContent_inputFecha').val();
                data.VendID = $('#MainContent_comboProveedores').val();
                data.Total = $('#MainContent_inputTotal').val();
                data.UUID = $('#MainContent_inputUUID').val();
                data.Status = $('#MainContent_comboEstado').val();
               
            }
        },
        searching: false,
        'aLengthMenu':[[10, 25, 50, 100, -1],[10, 25, 50, 100, "All"]],
        // 'aLengthMenu':[[2, 5, 10, 100, -1],[2, 5, 10, 100, "All"]],
        // "bStateSave": true,
        "processing": true,
        "serverSide": true,
        "stateSave": true,
        "columns": [
            { "data": "UUID" , 'className': "centrar-data", "orderable": false},
          
            { "data": "Folio" , 'className': "centrar-data text_align_left"},
            { "data": "Serie", 'className': "centrar-data" },
            { "data": "Fecha" , 'className': "centrar-data"},
            { "data": "Proveedor" , 'className': "centrar-data"},
            { "data": "Subtotal", 'className': "centrar-data" },
            { "data": "Total", 'className': "centrar-data" },
            { "data": "Estado", 'className': "centrar-data" }
        ],

        "columnDefs": [ {
            'targets': 0,
            'checkboxes': {
                'selectRow': true
            }
        } ],
        'select': {
            selector:'td:not(:last-child)',
            'style': 'multi'
        },
        'order': [[1, 'asc']]
    });


    $('#MainContent_inputTotal').mouseleave(function (e) {
        if (IsNumber('MainContent_inputTotal')) {
            $('#error_inputTotal').css('display', 'none');
        }
        else
            $('#error_inputTotal').css('display', 'block');
    });

    $('.buscar').click(function (e) {
        if (IsNumber('MainContent_inputTotal')) {
            e.preventDefault();
            table.ajax.reload();
        }
        else
            $('#error_inputTotal').css('display', 'block');
    });

    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputSerie').val('');
        $('#MainContent_inputFecha').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputUUID').val('');
 
        table.ajax.reload(  );
    });

    function getIds() {
        var rows_selected = table.column(0).data();
        var fechas = [];
        $.each(rows_selected, function (index, id) {

            fechas.push(id);
        });
        return fechas;
    }

    $('form').submit(function () {
        
        var ids = getIds();
        if (ids.length == 0) {
            showToastError("No existen registros para generar el reporte.");
            return false;
        }
redirectToReport(); return false;
        return true;
    });

    function redirectToReport() {
        var folio = $('#MainContent_inputFolio').val();
        var serie = $('#MainContent_inputSerie').val();
        var fecha = $('#MainContent_inputFecha').val();
        var provId = $('#MainContent_comboProveedores').val();
        var total = $('#MainContent_inputTotal').val();
        var uuid = $('#MainContent_inputUUID').val();
        var estado = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/Pagos";
        
        url_report += "?folio=" + folio;
        url_report += "&serie=" + serie;
        url_report += "&fecha=" + fecha;
        url_report += "&provId=" + provId;
        url_report += "&total=" + total;
        url_report += "&uuid=" + uuid;
        url_report += "&estado=" + estado;
        window.open(url_report, '_blank');
    }
} );

