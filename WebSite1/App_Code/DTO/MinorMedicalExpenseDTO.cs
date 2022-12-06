using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MinorMedicalExpenseDTO
/// </summary>

    public class MinorMedicalExpenseDTO : Document
    {
        public int MinorMedicalExpenseId { get; set; }
        public DateTime Date { get; set; }             
        public string DeniedReason { get; set; }
        public int? PackageId { get; set; }
        public List<string> FileNameXml { get; set; }
        public List<string> FileNamePdf { get; set; }
        public bool SageIntegration { get; set; }
        public MinorMedicalExpenseDTO()
            {
                //
                // TODO: Add constructor logic here
                //
            }
        }

    public class MinorMedicalExpenseReportDTO
    {
        public string Fecha { get; set; }
        public decimal Importe { get; set; }
        public string Estado { get; set; }
        public MinorMedicalExpenseReportDTO()
        {


        }
    }

public class MinorMedicalExpenseReport2DTO : Document
{
    public decimal Importe { get; set; }
    public string Estado { get; set; }
    public string Fecha { get; set; }
    public string Username { get; set; }

    public MinorMedicalExpenseReport2DTO()
    {


    }

}
