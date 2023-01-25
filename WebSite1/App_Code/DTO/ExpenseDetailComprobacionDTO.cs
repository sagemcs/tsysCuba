using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AdvanceDetailDTO
/// </summary>
public class ExpenseDetailComprobacionDTO
{
    public int? DetailId { get; set; }
    public int Type { get; set; }

    public string TipoGasto { get; set; }
    public int ItemKey { get; set; }
    public string ItemId { get; set; }
    public decimal Qty { get; set; }
    public decimal Amount
    {
        get { return Qty * UnitCost; }
        set { Amount = value; }
    }
    public decimal UnitCost { get; set; }
    public int STaxCodeKey { get; set; }

    public string STaxCodeID { get; set; }
    public decimal TaxAmount { get; set; }

    public ExpenseDetailComprobacionDTO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public enum Action { Insert, Delete, None }
}