/// <summary>
/// Summary description for ItemDTO
/// </summary>
public class AprobacionSolicitudChequeDTO
{

    public int InvcRcptKey { get; set; }
    public string VendorId { get; set; }
    public string VendName { get; set; }
    public string Solicitud { get; set; }
    public string Factura{ get; set; }
    public string CuentasXPagar { get; set; }
    public string DireccionTesorería { get; set; }
    public string DireccionFinanzas { get; set; }
    public AprobacionSolicitudChequeDTO()
    {
    }
}