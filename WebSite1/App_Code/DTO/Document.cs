using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Document
/// </summary>
public class Document
{
    public decimal Amount { get; set; }
    public int UpdateUserKey { get; set; }
    public string CompanyId { get; set; }
    public string Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public int? ApprovalLevel { get; set; }
    public int Currency { get; set; }   
   
    public int GetCurrency(Doc_Tools.DocumentType type)
    {
        int currendyid = 0;
        switch (type)
        {
            case Doc_Tools.DocumentType.Advance:
                var anticipo = (AdvanceDTO)this;
                currendyid = anticipo.Currency;
                break;
            case Doc_Tools.DocumentType.Expense:
                var reembolso = (ExpenseDTO)this;
                currendyid = reembolso.Currency;
                break;
            case Doc_Tools.DocumentType.CorporateCard:
                var tarjeta = (CorporateCardDTO)this;
                currendyid = tarjeta.Currency;
                break;
            case Doc_Tools.DocumentType.MinorMedicalExpense:
                var medico = (MinorMedicalExpenseDTO)this;
                currendyid = 1;
                break;
        }
        return currendyid;
    }
    public DateTime GetDate(Doc_Tools.DocumentType type)
    {
        var date = new DateTime();
        switch (type)
        {
            case Doc_Tools.DocumentType.Advance:
               var anticipo =  (AdvanceDTO)this;
                date = anticipo.CreateDate;
                break;
            case Doc_Tools.DocumentType.Expense:
                var reembolso = (ExpenseDTO)this;
                date = reembolso.Date;
                break;
            case Doc_Tools.DocumentType.CorporateCard:
                var tarjeta = (CorporateCardDTO)this;
                date = tarjeta.Date;
                break;
            case Doc_Tools.DocumentType.MinorMedicalExpense:
                var medico = (MinorMedicalExpenseDTO)this;
                date = medico.Date;
                break;           
        }
        return date;
    }
}