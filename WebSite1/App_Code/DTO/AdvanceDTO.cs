using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AdvanceDTO
/// </summary>
public class AdvanceDTO : Document
{
    public int AdvanceId { get; set; }
    public string AdvanceType { get; set; }
    public string Folio { get; set; }
    public int Currency { get; set; }    
    public DateTime? DepartureDate { get; set; }
    public DateTime? ArrivalDate { get; set; }
    public DateTime CheckDate { get; set; }
    public string ImmediateBoss { get; set; }       
    public string DeniedReason { get; set; }
    public int? PackageId { get; set; }   
    public string ExpenseReason { get; set; }
    public bool SageIntegration { get; set; }
    public AdvanceDTO()
    {
        
      
    }

}
public class AdvanceReportDTO
{
    public string Tipo { get; set; }
    public decimal Importe { get; set; }
    public string FechaSalida { get; set; }
    public string FechaLLegada { get; set; }
    public string FechaComprobacion { get; set; }
    public string JefeInmediato { get; set; }
    public string Estado { get; set; }

    public AdvanceReportDTO()
    {


    }
}

public class AdvanceReport2DTO : Document
{
    public string Tipo { get; set; }
    public decimal Importe { get; set; }
    public string FechaSalida { get; set; }
    public string FechaLLegada { get; set; }
    public string FechaComprobacion { get; set; }
    public string JefeInmediato { get; set; }
    public string Estado { get; set; }
    public string Moneda { get; set; }
    public string FechaCreado { get; set; }
    public string Username { get; set; }

    public AdvanceReport2DTO()
    {


    }

}

public class AdvanceReportGroupedDTO
{
    private decimal _total;
    public string FechaSalida { get; set; }
    public string FechaLLegada { get; set; }
    public string FechaComprobacion { get; set; }
    public decimal Viaje { get; set; }
    public decimal GExtra { get; set; }
    public decimal Total { get { return get_total(); } set { _total = value; } }

    public decimal get_total()
    {
        return this.Viaje + this.GExtra;
    }

    public AdvanceReportGroupedDTO()
    {


    }


}

public class AdvanceValidatorReportDTO
{
    public string Nombre { get; set; }
    public string Folio { get; set; }
    public string Tipo { get; set; }
    public string FechaSalida { get; set; }
    public string FechaLLegada { get; set; }
    public string FechaComprobacion { get; set; }
    public decimal Importe { get; set; }
    public AdvanceValidatorReportDTO()
    {


    }
}
