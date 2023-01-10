using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CurrExchSchdDtl
/// </summary>
public class CurrExchSchdDtlDTO
{
	public int CurrExchSchdKey { get; set; }
	public DateTime EffectiveDate { get; set; }
	public string SourceCurrID { get; set; }
	public string TargetCurrID { get; set; }
	public double CurrExchRate { get; set; }
	public DateTime? ExpirationDate { get; set; }
	public CurrExchSchdDtlDTO()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}