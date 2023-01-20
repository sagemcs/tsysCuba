/// <summary>
/// Summary description for ItemDTO
/// </summary>
public class RechazoSolicitudChequeDTO
{

    public int InvcRcptKey { get; set; }
    public string VendorId { get; set; }
    public string VendName { get; set; }
    public string Solicitud { get; set; }
    public string Factura{ get; set; }
    public string Rechazo { get; set; }
    public string Motivo { get; set; }
    public string Fecha  { get; set; }
    public RechazoSolicitudChequeDTO()
    {
    }
}