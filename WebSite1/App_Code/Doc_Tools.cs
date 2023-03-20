//PORTAL DE PROVEDORES T|SYS|
//2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA

//REFERENCIAS UTILIZADAS

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Proveedores_Model;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using static ExpenseFilesDTO;
using Microsoft.AspNet.Identity;
using WebSite1;
using System.Activities.Statements;
using System.IO;

/// <summary>
/// Summary description for Doc_Tools
/// </summary>
public static class Doc_Tools
{
    //Api Sage-Portal
    public static void Insert_tapApiLogErrorCgGst(int Batchkey, int vKey, int i_dtlkey, string errores)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into tapApiLogErrorCgGst values (@Batchkey,@vKey,@i_dtlkey,@errores)";
            cmd.Parameters.Add("@Batchkey", SqlDbType.Int).Value = Batchkey;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = vKey;
            cmd.Parameters.Add("@i_dtlkey", SqlDbType.Int).Value = i_dtlkey;
            cmd.Parameters.Add("@errores", SqlDbType.VarChar).Value = errores;

            cmd.Connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
        }
    }
    public static int Insert_tapBatchCExtGst(tapBatchCExtGst tap)
    {
        int iLote = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapBatchCExtGst (iBatchNo, iBatchKey, iBatchCmnt, iBatchOvrdSegVal, iBatchType, iInterCompany, iInterCoBatchKey, iOrigUserID, iPostDate, iSourceCompanyID, iTranCtrl, iHold, iHoldReason, iPrivate, iCashAcctKey, iCashAcctID, iImportLogKey, iLogSuccessful, oRetVal, oBatchKey, eliminado, status, FechaCreacion, FechaModificado, CompanyID, UserID, oSpid) VALUES" +
                                                             " (@iBatchNo, @iBatchKey, @iBatchCmnt, @iBatchOvrdSegVal, @iBatchType, @iInterCompany, @iInterCoBatchKey, @iOrigUserID, @iPostDate, @iSourceCompanyID, @iTranCtrl, @iHold, @iHoldReason, @iPrivate, @iCashAcctKey, @iCashAcctID, @iImportLogKey, @iLogSuccessful, @oRetVal, @oBatchKey, @eliminado, @status, @FechaCreacion, @FechaModificado, @CompanyID, @UserID, @oSpid); SELECT SCOPE_IDENTITY();";

            cmd.Parameters.Add("@iBatchNo", SqlDbType.Int).Value = tap.iBatchNo != null ? tap.iBatchNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchKey", SqlDbType.Int).Value = tap.iBatchKey != null ? tap.iBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchCmnt", SqlDbType.VarChar).Value = tap.iBatchCmnt != null ? tap.iBatchCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchOvrdSegVal", SqlDbType.VarChar).Value = tap.iBatchOvrdSegVal != null ? tap.iBatchOvrdSegVal : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchType", SqlDbType.Int).Value = tap.iBatchType != null ? tap.iBatchType : (object)DBNull.Value;
            cmd.Parameters.Add("@iInterCompany", SqlDbType.Int).Value = tap.iInterCompany != null ? tap.iInterCompany : (object)DBNull.Value;
            cmd.Parameters.Add("@iInterCoBatchKey", SqlDbType.Int).Value = tap.iInterCoBatchKey != null ? tap.iInterCoBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iOrigUserID", SqlDbType.VarChar).Value = tap.iOrigUserID != null ? tap.iOrigUserID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPostDate", SqlDbType.DateTime).Value = tap.iPostDate != null ? tap.iPostDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iSourceCompanyID", SqlDbType.VarChar).Value = tap.iSourceCompanyID != null ? tap.iSourceCompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranCtrl", SqlDbType.Decimal).Value = tap.iTranCtrl != null ? tap.iTranCtrl : (object)DBNull.Value;
            cmd.Parameters.Add("@iHold", SqlDbType.Int).Value = tap.iHold != null ? tap.iHold : (object)DBNull.Value;
            cmd.Parameters.Add("@iHoldReason", SqlDbType.VarChar).Value = tap.iHoldReason != null ? tap.iHoldReason : (object)DBNull.Value;
            cmd.Parameters.Add("@iPrivate", SqlDbType.Int).Value = tap.iPrivate != null ? tap.iPrivate : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctKey", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctID", SqlDbType.VarChar).Value = tap.iCashAcctID != null ? tap.iCashAcctID : (object)DBNull.Value;
            cmd.Parameters.Add("@iImportLogKey", SqlDbType.Int).Value = tap.iImportLogKey != null ? tap.iImportLogKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iLogSuccessful", SqlDbType.Int).Value = tap.iLogSuccessful != null ? tap.iLogSuccessful : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;
            cmd.Parameters.Add("@oBatchKey", SqlDbType.Int).Value = tap.oBatchKey != null ? tap.oBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@eliminado", SqlDbType.Bit).Value = tap.eliminado != null ? tap.eliminado : (object)DBNull.Value;
            cmd.Parameters.Add("@status", SqlDbType.Int).Value = tap.status != null ? tap.status : (object)DBNull.Value;
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = tap.FechaCreacion != null ? tap.FechaCreacion : (object)DBNull.Value;
            cmd.Parameters.Add("@FechaModificado", SqlDbType.DateTime).Value = tap.FechaModificado != null ? tap.FechaModificado : (object)DBNull.Value;
            cmd.Parameters.Add("@CompanyID", SqlDbType.VarChar).Value = tap.CompanyID != null ? tap.CompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = tap.UserID != null ? tap.UserID : (object)DBNull.Value;
            cmd.Parameters.Add("@oSpid", SqlDbType.Int).Value = tap.oSpid != null ? tap.oSpid : (object)DBNull.Value;
            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                iLote = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }

        }
        return iLote;
    }
    public static int Insert_tapVoucherCExGst(tapVoucherCExGst tap)
    {
        int vkey = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapVoucherCExGst (iLote, iApplySeqNo, iBatchKey, iRemitToVendAddrKey, iRemitToVendAddrID, iRemitToCopyKey, iRemitToCopyID, iRemitToAddrName, iRTAddrLine1, iRTAddrLine2, iRTAddrLine3, iRTAddrLine4, iRTAddrLine5,  iRTCity, iRTState, iRTCountryID, iRTPostalCode, iCashAcctKey, iCashAcctID, iCntcKey, iCntcName, iCurrExchRate, iCurrExchSchdKey, iCurrExchSchdID, iCurrID, iHoldPmt, iVendClassKey, iVendClassID, iVendKey, iVendID, iFOBKey, iFOBID, iImportLogKey, iPmtTermsKey, iPmtTermsID, iReasonCodeKey, iReasonCodeID, iFreightAmt, iSeparateCheck, iShipMethKey, iShipMethID, iPurchFromVendAddrKey, iPurchFromVendAddrID, iPurchToCopyKey, iPurchToCopyID, iPurchFromAddrName, iPurchAddrLine1, iPurchAddrLine2, iPurchAddrLine3, iPurchAddrLine4, iPurchAddrLine5, iPurchCity, iPurchState, iPurchCountryID, iPurchPostalCode, iShipZoneKey, iShipZoneID, iTranCmnt, iTranDate, iTranNo, iTranType, iUniqueID, iDefaultIfNull, iUserID, iV1099Box, iV1099BoxText, iV1099Form, iRetntAmt, iVouchNo, iInvcRcptDate, oVouchKey, oTranID, iDueDate, iDiscAmt, iDiscDate, iExemptKey, iSTaxAmt, iPurchAmt, iTranAmt, oRetVal) VALUES " +
                "(@iLote, @iApplySeqNo, @iBatchKey, @iRemitToVendAddrKey, @iRemitToVendAddrID, @iRemitToCopyKey, @iRemitToCopyID, @iRemitToAddrName, @iRTAddrLine1, @iRTAddrLine2, @iRTAddrLine3, @iRTAddrLine4, @iRTAddrLine5, @iRTCity, @iRTState, @iRTCountryID, @iRTPostalCode, @iCashAcctKey, @iCashAcctID, @iCntcKey, @iCntcName, @iCurrExchRate, @iCurrExchSchdKey, @iCurrExchSchdID, @iCurrID, @iHoldPmt, @iVendClassKey, @iVendClassID, @iVendKey, @iVendID, @iFOBKey, @iFOBID, @iImportLogKey, @iPmtTermsKey, @iPmtTermsID, @iReasonCodeKey, @iReasonCodeID, @iFreightAmt, @iSeparateCheck, @iShipMethKey, @iShipMethID, @iPurchFromVendAddrKey, @iPurchFromVendAddrID, @iPurchToCopyKey, @iPurchToCopyID, @iPurchFromAddrName, @iPurchAddrLine1, @iPurchAddrLine2, @iPurchAddrLine3, @iPurchAddrLine4, @iPurchAddrLine5, @iPurchCity, @iPurchState, @iPurchCountryID, @iPurchPostalCode, @iShipZoneKey, @iShipZoneID, @iTranCmnt, @iTranDate, @iTranNo, @iTranType, @iUniqueID, @iDefaultIfNull, @iUserID, @iV1099Box, @iV1099BoxText, @iV1099Form, @iRetntAmt, @iVouchNo, @iInvcRcptDate, @oVouchKey, @oTranID, @iDueDate, @iDiscAmt, @iDiscDate, @iExemptKey, @iSTaxAmt, @iPurchAmt, @iTranAmt, @oRetVal); SELECT SCOPE_IDENTITY();";

            cmd.Parameters.Add("@iLote", SqlDbType.Int).Value = tap.iLote != null ? tap.iLote : (object)DBNull.Value;
            cmd.Parameters.Add("@iApplySeqNo", SqlDbType.Int).Value = tap.iApplySeqNo != null ? tap.iApplySeqNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchKey", SqlDbType.Int).Value = tap.iBatchKey != null ? tap.iBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToVendAddrKey", SqlDbType.Int).Value = tap.iRemitToVendAddrKey != null ? tap.iRemitToVendAddrKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToVendAddrID", SqlDbType.Int).Value = tap.iRemitToVendAddrID != null ? tap.iRemitToVendAddrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToCopyKey", SqlDbType.Int).Value = tap.iRemitToCopyKey != null ? tap.iRemitToCopyKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToCopyID", SqlDbType.VarChar).Value = tap.iRemitToCopyID != null ? tap.iRemitToCopyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToAddrName", SqlDbType.VarChar).Value = tap.iRemitToAddrName != null ? tap.iRemitToAddrName : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine1", SqlDbType.VarChar).Value = tap.iRTAddrLine1 != null ? tap.iRTAddrLine1 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine2", SqlDbType.VarChar).Value = tap.iRTAddrLine2 != null ? tap.iRTAddrLine2 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine3", SqlDbType.VarChar).Value = tap.iRTAddrLine3 != null ? tap.iRTAddrLine3 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine4", SqlDbType.VarChar).Value = tap.iRTAddrLine4 != null ? tap.iRTAddrLine4 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine5", SqlDbType.VarChar).Value = tap.iRTAddrLine5 != null ? tap.iRTAddrLine5 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTCity", SqlDbType.VarChar).Value = tap.iRTCity != null ? tap.iRTCity : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTState", SqlDbType.VarChar).Value = tap.iRTState != null ? tap.iRTState : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTCountryID", SqlDbType.VarChar).Value = tap.iRTCountryID != null ? tap.iRTCountryID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTPostalCode", SqlDbType.VarChar).Value = tap.iRTPostalCode != null ? tap.iRTPostalCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctKey", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctID", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCntcKey", SqlDbType.Int).Value = tap.iCntcKey != null ? tap.iCntcKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCntcName", SqlDbType.VarChar).Value = tap.iCntcName != null ? tap.iCntcName : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchRate", SqlDbType.Decimal).Value = tap.iCurrExchRate != null ? tap.iCurrExchRate : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchSchdKey", SqlDbType.Int).Value = tap.iCurrExchSchdKey != null ? tap.iCurrExchSchdKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchSchdID", SqlDbType.VarChar).Value = tap.iCurrExchSchdID != null ? tap.iCurrExchSchdID : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrID", SqlDbType.VarChar).Value = tap.iCurrID != null ? tap.iCurrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iHoldPmt", SqlDbType.Int).Value = tap.iHoldPmt != null ? tap.iHoldPmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendClassKey", SqlDbType.Int).Value = tap.iVendClassKey != null ? tap.iVendClassKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendClassID", SqlDbType.VarChar).Value = tap.iVendClassID != null ? tap.iVendClassID : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendKey", SqlDbType.Int).Value = tap.iVendKey != null ? tap.iVendKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendID", SqlDbType.VarChar).Value = tap.iVendID != null ? tap.iVendID : (object)DBNull.Value;
            //iFOBKey, iFOBID, iImportLogKey, iPmtTermsKey, iPmtTermsID, iReasonCodeKey, iReasonCodeID, iFreightAmt, iSeparateCheck, iShipMethKey, iShipMethID, iPurchFromVendAddrKey, iPurchFromVendAddrID, iPurchToCopyKey, 
            cmd.Parameters.Add("@iFOBKey", SqlDbType.Int).Value = tap.iFOBKey != null ? tap.iFOBKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBID", SqlDbType.VarChar).Value = tap.iFOBID != null ? tap.iFOBID : (object)DBNull.Value;
            cmd.Parameters.Add("@iImportLogKey", SqlDbType.Int).Value = tap.iImportLogKey != null ? tap.iImportLogKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPmtTermsKey", SqlDbType.Int).Value = tap.iPmtTermsKey != null ? tap.iPmtTermsKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPmtTermsID", SqlDbType.VarChar).Value = tap.iPmtTermsID != null ? tap.iPmtTermsID : (object)DBNull.Value;
            cmd.Parameters.Add("@iReasonCodeKey", SqlDbType.Int).Value = tap.iReasonCodeKey != null ? tap.iReasonCodeKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iReasonCodeID", SqlDbType.VarChar).Value = tap.iReasonCodeID != null ? tap.iReasonCodeID : (object)DBNull.Value;
            cmd.Parameters.Add("@iFreightAmt", SqlDbType.Decimal).Value = tap.iFreightAmt != null ? tap.iFreightAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iSeparateCheck", SqlDbType.Int).Value = tap.iSeparateCheck != null ? tap.iSeparateCheck : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethKey", SqlDbType.Int).Value = tap.iShipMethKey != null ? tap.iShipMethKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethID", SqlDbType.VarChar).Value = tap.iShipMethID != null ? tap.iShipMethID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromVendAddrKey", SqlDbType.Int).Value = tap.iPurchFromVendAddrKey != null ? tap.iPurchFromVendAddrKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromVendAddrID", SqlDbType.VarChar).Value = tap.iPurchFromVendAddrID != null ? tap.iPurchFromVendAddrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchToCopyKey", SqlDbType.Int).Value = tap.iPurchToCopyKey != null ? tap.iPurchToCopyKey : (object)DBNull.Value;
            //iPurchToCopyID, iPurchFromAddrName, iPurchAddrLine1, iPurchAddrLine2, iPurchAddrLine3, iPurchAddrLine4, iPurchAddrLine5, iPurchCity, iPurchState, iPurchCountryID, iPurchPostalCode, iShipZoneKey, iShipZoneID, 
            cmd.Parameters.Add("@iPurchToCopyID", SqlDbType.VarChar).Value = tap.iPurchToCopyID != null ? tap.iPurchToCopyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromAddrName", SqlDbType.VarChar).Value = tap.iPurchFromAddrName != null ? tap.iPurchFromAddrName : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine1", SqlDbType.VarChar).Value = tap.iPurchAddrLine1 != null ? tap.iPurchAddrLine1 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine2", SqlDbType.VarChar).Value = tap.iPurchAddrLine2 != null ? tap.iPurchAddrLine2 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine3", SqlDbType.VarChar).Value = tap.iPurchAddrLine3 != null ? tap.iPurchAddrLine3 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine4", SqlDbType.VarChar).Value = tap.iPurchAddrLine4 != null ? tap.iPurchAddrLine4 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine5", SqlDbType.VarChar).Value = tap.iPurchAddrLine5 != null ? tap.iPurchAddrLine5 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchCity", SqlDbType.VarChar).Value = tap.iPurchCity != null ? tap.iPurchCity : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchState", SqlDbType.VarChar).Value = tap.iPurchState != null ? tap.iPurchState : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchCountryID", SqlDbType.VarChar).Value = tap.iPurchCountryID != null ? tap.iPurchCountryID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchPostalCode", SqlDbType.VarChar).Value = tap.iPurchPostalCode != null ? tap.iPurchPostalCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneKey", SqlDbType.Int).Value = tap.iShipZoneKey != null ? tap.iShipZoneKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneID", SqlDbType.VarChar).Value = tap.iShipZoneID != null ? tap.iShipZoneID : (object)DBNull.Value;
            //iTranCmnt, iTranDate, iTranNo, iTranType, iUniqueID, iDefaultIfNull, iUserID, iV1099Box, iV1099BoxText, iV1099Form, iRetntAmt,
            //iVouchNo, iInvcRcptDate, oVouchKey, oTranID, iDueDate, iDiscAmt, iDiscDate, iExemptKey, 
            cmd.Parameters.Add("@iTranCmnt", SqlDbType.VarChar).Value = tap.iTranCmnt != null ? tap.iTranCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranDate", SqlDbType.DateTime).Value = tap.iTranDate != null ? tap.iTranDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranNo", SqlDbType.VarChar).Value = tap.iTranNo != null ? tap.iTranNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranType", SqlDbType.Int).Value = tap.iTranType != null ? tap.iTranType : (object)DBNull.Value;
            cmd.Parameters.Add("@iUniqueID", SqlDbType.VarChar).Value = tap.iUniqueID != null ? tap.iUniqueID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDefaultIfNull", SqlDbType.Int).Value = tap.iDefaultIfNull != null ? tap.iDefaultIfNull : (object)DBNull.Value;
            cmd.Parameters.Add("@iUserID", SqlDbType.VarChar).Value = tap.iUserID != null ? tap.iUserID : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099Box", SqlDbType.VarChar).Value = tap.iV1099Box != null ? tap.iV1099Box : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099BoxText", SqlDbType.VarChar).Value = tap.iV1099BoxText != null ? tap.iV1099BoxText : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099Form", SqlDbType.Int).Value = tap.iV1099Form != null ? tap.iV1099Form : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntAmt", SqlDbType.Decimal).Value = tap.iRetntAmt != null ? tap.iRetntAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iVouchNo", SqlDbType.VarChar).Value = tap.iVouchNo != null ? tap.iVouchNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iInvcRcptDate", SqlDbType.DateTime).Value = tap.iInvcRcptDate != null ? tap.iInvcRcptDate : (object)DBNull.Value;
            cmd.Parameters.Add("@oVouchKey", SqlDbType.Int).Value = tap.oVouchKey != null ? tap.oVouchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@oTranID", SqlDbType.VarChar).Value = tap.oTranID != null ? tap.oTranID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDueDate", SqlDbType.DateTime).Value = tap.iDueDate != null ? tap.iDueDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iDiscAmt", SqlDbType.Decimal).Value = tap.iDiscAmt != null ? tap.iDiscAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iDiscDate", SqlDbType.DateTime).Value = tap.iDiscDate != null ? tap.iDiscDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iExemptKey", SqlDbType.Int).Value = tap.iExemptKey != null ? tap.iExemptKey : (object)DBNull.Value;
            //iSTaxAmt, iPurchAmt, iTranAmt, oRetVal
            cmd.Parameters.Add("@iSTaxAmt", SqlDbType.Decimal).Value = tap.iSTaxAmt != null ? tap.iSTaxAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAmt", SqlDbType.Decimal).Value = tap.iPurchAmt != null ? tap.iPurchAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranAmt", SqlDbType.Decimal).Value = tap.iTranAmt != null ? tap.iTranAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;
           
            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                vkey = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
        }
        return vkey;
    }
    public static int Insert_tapVoucherDtlCExGst(tapVoucherDtlCExGst tap)
    {
        //  
        int dtlKey = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapVoucherDtlCExGst (iLote, vKey, iTranNo, iTargetCompanyID, iVouchKey, iCmntOnly, iDescription, iExtAmt, iRetntAmt, iRetntRate, iExtCmnt, iItemKey, iItemID, iQuantity, iGLAcctKey, iGLAcctNo, iSTaxClassKey, iSTaxClassID, iUnitCost, iUnitMeasKey, iUnitMeasID, iAcctRefKey, iAcctRefCode, iFOBKey, iFOBID, iFreightAmt, iShipMethKey, iShipMethID, iShipZoneKey, iShipZoneID, iSTaxSchdKey, iSTaxSchdID, iDefaultIfNull, iPerformOverride, iSpid,  iCommissionFlag, iPOKey, iPONo, iPOLineKey, iPOLineNo, iRcvrLineKey, iMatchStatus, iReturnType, oRetVal) VALUES" +
                " (@iLote, @vKey, @iTranNo, @iTargetCompanyID, @iVouchKey, @iCmntOnly, @iDescription, @iExtAmt, @iRetntAmt, @iRetntRate, @iExtCmnt, @iItemKey, @iItemID, @iQuantity, @iGLAcctKey, @iGLAcctNo, @iSTaxClassKey, @iSTaxClassID, @iUnitCost, @iUnitMeasKey, @iUnitMeasID, @iAcctRefKey, @iAcctRefCode, @iFOBKey, @iFOBID, @iFreightAmt, @iShipMethKey, @iShipMethID, @iShipZoneKey, @iShipZoneID, @iSTaxSchdKey, @iSTaxSchdID, @iDefaultIfNull, @iPerformOverride, @iSpid,  @iCommissionFlag, @iPOKey, @iPONo, @iPOLineKey, @iPOLineNo, @iRcvrLineKey, @iMatchStatus, @iReturnType, @oRetVal); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.Add("@iLote", SqlDbType.Int).Value = tap.iLote != null ? tap.iLote : (object)DBNull.Value;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = tap.vKey != null ? tap.vKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranNo", SqlDbType.VarChar).Value = tap.iTranNo != null ? tap.iTranNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iTargetCompanyID", SqlDbType.VarChar).Value = tap.iTargetCompanyID != null ? tap.iTargetCompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iVouchKey", SqlDbType.Int).Value = tap.iVouchKey != null ? tap.iVouchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCmntOnly", SqlDbType.Int).Value = tap.iCmntOnly != null ? tap.iCmntOnly : (object)DBNull.Value;
            cmd.Parameters.Add("@iDescription", SqlDbType.VarChar).Value = tap.iDescription != null ? tap.iDescription : (object)DBNull.Value;
            cmd.Parameters.Add("@iExtAmt", SqlDbType.Decimal).Value = tap.iExtAmt != null ? tap.iExtAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntAmt", SqlDbType.Decimal).Value = tap.iRetntAmt != null ? tap.iRetntAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntRate", SqlDbType.Decimal).Value = tap.iRetntRate != null ? tap.iRetntRate : (object)DBNull.Value;

            cmd.Parameters.Add("@iExtCmnt", SqlDbType.VarChar).Value = tap.iExtCmnt != null ? tap.iExtCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iItemKey", SqlDbType.Int).Value = tap.iItemKey != null ? tap.iItemKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iItemID", SqlDbType.VarChar).Value = tap.iItemID != null ? tap.iItemID : (object)DBNull.Value;
            cmd.Parameters.Add("@iQuantity", SqlDbType.Decimal).Value = tap.iQuantity != null ? tap.iQuantity : (object)DBNull.Value;
            cmd.Parameters.Add("@iGLAcctKey", SqlDbType.Int).Value = tap.iGLAcctKey != null ? tap.iGLAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iGLAcctNo", SqlDbType.VarChar).Value = tap.iGLAcctNo != null ? tap.iGLAcctNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxClassKey", SqlDbType.Int).Value = tap.iSTaxClassKey != null ? tap.iSTaxClassKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxClassID", SqlDbType.VarChar).Value = tap.iSTaxClassID != null ? tap.iSTaxClassID : (object)DBNull.Value;
            cmd.Parameters.Add("@iUnitCost", SqlDbType.Decimal).Value = tap.iUnitCost != null ? tap.iUnitCost : (object)DBNull.Value;
            cmd.Parameters.Add("@iUnitMeasKey", SqlDbType.Int).Value = tap.iUnitMeasKey != null ? tap.iUnitMeasKey : (object)DBNull.Value;

            cmd.Parameters.Add("@iUnitMeasID", SqlDbType.VarChar).Value = tap.iUnitMeasID != null ? tap.iUnitMeasID : (object)DBNull.Value;
            cmd.Parameters.Add("@iAcctRefKey", SqlDbType.Int).Value = tap.iAcctRefKey != null ? tap.iAcctRefKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iAcctRefCode", SqlDbType.VarChar).Value = tap.iAcctRefCode != null ? tap.iAcctRefCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBKey", SqlDbType.Int).Value = tap.iFOBKey != null ? tap.iFOBKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBID", SqlDbType.VarChar).Value = tap.iFOBID != null ? tap.iFOBID : (object)DBNull.Value;
            cmd.Parameters.Add("@iFreightAmt", SqlDbType.Decimal).Value = tap.iFreightAmt != null ? tap.iFreightAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethKey", SqlDbType.Int).Value = tap.iShipMethKey != null ? tap.iShipMethKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethID", SqlDbType.VarChar).Value = tap.iShipMethID != null ? tap.iShipMethID : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneKey", SqlDbType.Int).Value = tap.iShipZoneKey != null ? tap.iShipZoneKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneID", SqlDbType.VarChar).Value = tap.iShipZoneID != null ? tap.iShipZoneID : (object)DBNull.Value;

            cmd.Parameters.Add("@iSTaxSchdKey", SqlDbType.Int).Value = tap.iSTaxSchdKey != null ? tap.iSTaxSchdKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxSchdID", SqlDbType.VarChar).Value = tap.iSTaxSchdID != null ? tap.iSTaxSchdID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDefaultIfNull", SqlDbType.Int).Value = tap.iDefaultIfNull != null ? tap.iDefaultIfNull : (object)DBNull.Value;
            cmd.Parameters.Add("@iPerformOverride", SqlDbType.Int).Value = tap.iPerformOverride != null ? tap.iPerformOverride : (object)DBNull.Value;
            cmd.Parameters.Add("@iSpid", SqlDbType.Int).Value = tap.iSpid != null ? tap.iSpid : (object)DBNull.Value;
            cmd.Parameters.Add("@iCommissionFlag", SqlDbType.VarChar).Value = tap.iCommissionFlag != null ? tap.iCommissionFlag : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOKey", SqlDbType.Int).Value = tap.iPOKey != null ? tap.iPOKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPONo", SqlDbType.VarChar).Value = tap.iPONo != null ? tap.iPONo : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOLineKey", SqlDbType.Int).Value = tap.iPOLineKey != null ? tap.iPOLineKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOLineNo", SqlDbType.Int).Value = tap.iPOLineNo != null ? tap.iPOLineNo : (object)DBNull.Value;

            cmd.Parameters.Add("@iRcvrLineKey", SqlDbType.Int).Value = tap.iRcvrLineKey != null ? tap.iRcvrLineKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iMatchStatus", SqlDbType.Int).Value = tap.iMatchStatus != null ? tap.iMatchStatus : (object)DBNull.Value;
            cmd.Parameters.Add("@iReturnType", SqlDbType.Int).Value = tap.iReturnType != null ? tap.iReturnType : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;
            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                dtlKey = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
        }
        return dtlKey;
    }
    public static int Exec_sppaVoucherAPIGst(int Batchkey, int vKey, string Company)
    {
        int RetVal = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "sppaVoucherAPIGst";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Batchkey", SqlDbType.Int).Value = Batchkey;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = vKey;
            cmd.Parameters.Add("@Company", SqlDbType.VarChar).Value = Company;
            //cmd.Parameters.Add("@RetVal", SqlDbType.Int).Value = RetVal;
            cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Connection.Open();
            cmd.CommandTimeout = 300;
            try
            {
                cmd.ExecuteNonQuery();
                RetVal = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                RetVal = 3;
                cmd.Connection.Close();
            }
        }
        return RetVal;
    }

    public static int ExecuteVoucherApi(string username, Document document, List<ExpenseDetailDTO> items, DocumentType type)
    {
        int iLote;
        int vkey = 0;
        int dtlKey = 0;
        int RetVal = 0;
        var taxes = get_taxes(document.CompanyId);
        var articulos = get_items(document.CompanyId);

        //Si el tipo de Documento es Anticipo
        if (type == DocumentType.Advance)
        {
            tapBatchCExtGst tapBatch = new tapBatchCExtGst
            {
                iBatchType = 401,
                iOrigUserID = username,
                iSourceCompanyID = document.CompanyId,
                iHold = 1,
                iPrivate = 1,
                status = 0,
                FechaCreacion = DateTime.Now,
                FechaModificado = DateTime.Now,
                CompanyID = document.CompanyId,
                UserID = username
            };

            iLote = Insert_tapBatchCExtGst(tapBatch);
            tapVoucherCExGst tapVoucher = new tapVoucherCExGst
            {
                iLote = iLote,
                iRemitToVendAddrKey = 144830,
                iRemitToCopyKey = 144830,
                iCurrID = ((Currencys)document.GetCurrency(type)).ToString(), //"MXN", //Moneda del gasto
                iHoldPmt = 0,
                iVendKey = 1652,
                iVendID = "G778",
                iPmtTermsKey = 2,
                iSeparateCheck = 0,
                iPurchFromVendAddrKey = 144830,
                iPurchToCopyKey = 144830,
                iTranDate = document.GetDate(type),
                iTranNo = iLote.ToString(),
                iTranType = 401,
                iUserID = username,
                iInvcRcptDate = document.GetDate(type),
                iDueDate = document.GetDate(type),
                iDiscDate = null,
                iDiscAmt = 0,
                iSTaxAmt =  0,
                iPurchAmt = document.Amount,
                iTranAmt = document.Amount               
            };

            if(tapVoucher.iCurrID != Currencys.MNX.ToString())
            {
                decimal? tranAmount = tapVoucher.iTranAmt;
                double rate = 1;

                var ChangeRates = get_ChangeRates().FirstOrDefault(x => x.EffectiveDate == tapVoucher.iTranDate);
                if (ChangeRates != null)
                {
                   rate = ChangeRates.CurrExchRate;
                }        
                    
               tapVoucher.iTranAmt = decimal.Multiply(tranAmount.Value, Convert.ToDecimal(rate));
            }

            vkey = Insert_tapVoucherCExGst(tapVoucher);

            //articulo de pruaba para el Anticipo, pendiente a definir
            ItemDTO item = articulos.First();

            tapVoucherDtlCExGst tapVoucherDtl = new tapVoucherDtlCExGst
            {
                iLote = iLote,
                vKey = vkey,
                iTranNo = iLote.ToString(),
                iTargetCompanyID = document.CompanyId,
                iCmntOnly = 0,
                iExtAmt = document.Amount,
                iItemKey = item.ItemKey,
                iQuantity = 1,
                iGLAcctKey = articulos.FirstOrDefault(x => x.ItemKey == item.ItemKey).ExpAcctKey,
                iSTaxClassKey = articulos.FirstOrDefault(x => x.ItemKey == item.ItemKey).STaxClassKey,
                iSTaxClassID = taxes.FirstOrDefault(x => x.STaxCodeKey == articulos.FirstOrDefault(d => d.ItemKey == item.ItemKey).STaxClassKey).STaxClassID, 
                iUnitCost = document.Amount,
                iUnitMeasID = "Serv",
                iFreightAmt = 0,
                iSTaxSchdKey = 2,
                iSTaxSchdID = "IVA 16",
                iDefaultIfNull = 1,
                iMatchStatus = 1,
                iReturnType = 1,
                iPOLineNo = null,
                iPOKey = null,
                iPONo = null,
                iPOLineKey = null,
                oRetVal = null
            };
            dtlKey = Insert_tapVoucherDtlCExGst(tapVoucherDtl);

            RetVal = Exec_sppaVoucherAPIGst(iLote, vkey, document.CompanyId.Trim());
            if (RetVal != 1)
            {
                Insert_tapApiLogErrorCgGst(iLote, vkey, dtlKey, "Ha ocurrido un error en la ejecucion del procedimiento Sage");               
            }
        }

        //Si el tipo de Documento es Reembolso, Tarjeta, Gasto Medico Menor
        if (type != DocumentType.Advance)
        {
            //Recorrido por los articulos del documento
            foreach (ExpenseDetailDTO item in items)
            {
                tapBatchCExtGst tapBatch = new tapBatchCExtGst
                {
                    iBatchType = 401,
                    iOrigUserID = username,
                    iSourceCompanyID = document.CompanyId,
                    iHold = 1,
                    iPrivate = 1,
                    status = 0,
                    FechaCreacion = DateTime.Now,
                    FechaModificado = DateTime.Now,
                    CompanyID = document.CompanyId,
                    UserID = username
                };

                iLote = Insert_tapBatchCExtGst(tapBatch);
                tapVoucherCExGst tapVoucher = new tapVoucherCExGst
                {
                    iLote = iLote,
                    iRemitToVendAddrKey = 144830,
                    iRemitToCopyKey = 144830,
                    iCurrID = ((Currencys)document.GetCurrency(type)).ToString(), 
                    iHoldPmt = 0,
                    iVendKey = 1652,
                    iVendID = "G778",
                    iPmtTermsKey = 2,
                    iSeparateCheck = 0,
                    iPurchFromVendAddrKey = 144830,
                    iPurchToCopyKey = 144830,
                    iTranDate = document.GetDate(type),
                    iTranNo = iLote.ToString(),
                    iTranType = 401,
                    iUserID = username,
                    iInvcRcptDate = document.GetDate(type),
                    iDueDate = document.GetDate(type),
                    iDiscDate = null,
                    iDiscAmt = 0,
                    iSTaxAmt =  item.TaxAmount, 
                    iPurchAmt =  item.Amount,
                    iTranAmt =  item.Amount + item.TaxAmount,                   
                };
                vkey = Insert_tapVoucherCExGst(tapVoucher);
                tapVoucherDtlCExGst tapVoucherDtl = new tapVoucherDtlCExGst
                {
                    iLote = iLote,
                    vKey = vkey,
                    iTranNo = iLote.ToString(),
                    iTargetCompanyID = document.CompanyId,
                    iCmntOnly = 0,
                    iExtAmt = item.Amount,
                    iItemKey = item.ItemKey,
                    iQuantity = item.Qty,
                    iGLAcctKey = articulos.FirstOrDefault(x => x.ItemKey == item.ItemKey).ExpAcctKey,
                    iSTaxClassKey = articulos.FirstOrDefault(x => x.ItemKey == item.ItemKey).STaxClassKey,
                    iSTaxClassID = taxes.FirstOrDefault(x => x.STaxCodeKey == item.STaxCodeKey).STaxClassID,
                    iUnitCost = item.UnitCost,
                    iUnitMeasID = "Serv",
                    iFreightAmt = 0,
                    iSTaxSchdKey = 2,
                    iSTaxSchdID = "IVA 16",
                    iDefaultIfNull = 1,
                    iMatchStatus = 1,
                    iReturnType = 1,
                    iPOLineNo = null,
                    iPOKey = null,
                    iPONo = null,
                    iPOLineKey = null,
                    oRetVal = null
                };
                dtlKey = Insert_tapVoucherDtlCExGst(tapVoucherDtl);

                RetVal = Exec_sppaVoucherAPIGst(iLote, vkey, document.CompanyId.Trim());
                if (RetVal != 1)
                {
                    Insert_tapApiLogErrorCgGst(iLote, vkey, dtlKey, string.Format("{0} - {1} - {2}", "Ha ocurrido un error en la ejecucion del procedimiento Sage",type.ToString(), DateTime.Now.ToShortDateString()));                 
                }
            }
        }   
       
        return RetVal;

    }

    public static void DeleteVoucherOnFail(int ilote)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "delete from tapBatchCExtGst where ilote = @ilote";
            cmd.Parameters.Add("@ilote", SqlDbType.Int).Value = ilote;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "delete from tapVoucherCExGst where ilote = @ilote";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "delete from tapVoucherDtlCExGst where ilote = @ilote";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }

    public static List<EmpleadoDTO> GetEmpleados(int logged_user, int level, DocumentType type)
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();

            switch (type)
            {
                case DocumentType.Advance:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM Advance;";
                    break;
                case DocumentType.Expense:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM Expense;";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM CorporateCard;";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM MinorMedicalExpense;";
                    break;
            }

            List<int> keys = new List<int>();
            List<int> employees_keys = new List<int>();
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                int employee_key = dataReader.GetInt32(0);
                keys.Add(employee_key);
            }
            cmd.Connection.Close();

            //switcheo por niveles patra obtener usuario inferiores en la matriz
            string rol = HttpContext.Current.Session["RolUser"].ToString();

            switch (rol)
            {
                //si el usuario es Cuentas por Cobrar 
                case "T|SYS| - Validador":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserValidadorCX = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_1 = cmd.ExecuteReader();
                    while (dataReader_1.Read())
                    {
                        int employee_key = dataReader_1.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Tesoreria --> traer Cuentas por Cobrar
                case "T|SYS| - Tesoreria":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserTesoreria = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_2 = cmd.ExecuteReader();
                    while (dataReader_2.Read())
                    {
                        int employee_key = dataReader_2.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Finanzas":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserFinanzas = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerente":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserGerente = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_4 = cmd.ExecuteReader();
                    while (dataReader_4.Read())
                    {
                        int employee_key = dataReader_4.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerencia de Capital Humano":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserRH = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_5 = cmd.ExecuteReader();
                    while (dataReader_5.Read())
                    {
                        int employee_key = dataReader_5.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
            }


            foreach (int employee_key in employees_keys.Distinct())
            {
                if (keys.Contains(employee_key))
                {
                    cmd.CommandText = "SELECT UserKey ,UserID ,UserName FROM Users where UserKey = @userkey;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@userkey", SqlDbType.Int).Value = employee_key;
                    cmd.Connection.Open();
                    SqlDataReader Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        var empleado = new EmpleadoDTO();
                        empleado.UserKey = Reader.GetInt32(0);
                        empleado.Correo = Reader.GetString(1);
                        empleado.Nombre = Reader.GetString(2);
                        empleados.Add(empleado);
                    }
                    cmd.Connection.Close();
                }
            }
        }

        return empleados;
    }

    public static List<TaxesDTO> get_taxes(string CompanyID)
    {
        var lista = new List<TaxesDTO>();
        //agregar item vacio 
        lista.Add(new TaxesDTO { STaxCodeID = "", STaxCodeKey = 0, Rate = 0 });
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT tciSTaxClass.*,dbo.tciSTaxSubjClassDt.STaxCodeKey,dbo.tciSTaxSubjClassDt.STaxClassKey, dbo.tciSTaxCode.STaxCodeID, dbo.tciSTaxCode.Description, dbo.tciSTaxSubjClassDt.Rate  FROM dbo.tciSTaxSubjClassDt INNER JOIN dbo.tciSTaxCode ON dbo.tciSTaxSubjClassDt.STaxCodeKey = dbo.tciSTaxCode.STaxCodeKey  INNER JOIN  dbo.tciSTaxCodeCompany ON dbo.tciSTaxSubjClassDt.STaxCodeKey = dbo.tciSTaxCodeCompany.STaxCodeKey  inner join dbo.tciSTaxSubjClass on dbo.tciSTaxSubjClassDt.STaxClassKey = dbo.tciSTaxSubjClass.STaxClassKey  inner join dbo.tciSTaxClass on dbo.tciSTaxClass.STaxClassKey = dbo.tciSTaxSubjClass.STaxClassKey  inner join tciSTaxSchdCodes on tciSTaxSchdCodes.STaxCodeKey = tciSTaxCode.STaxCodeKey WHERE(dbo.tciSTaxCodeCompany.CompanyID = @CompanyID) and tciSTaxSubjClass.UseWithPurch = 1";
            cmd.Parameters.Add("@CompanyID", SqlDbType.VarChar).Value = CompanyID.Trim();
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                //STaxClassKey(0)	STaxClassID(1)	STaxCodeClass(2)	UpdateCounter(3)	STaxCodeKey(4)	STaxClassKey(5)	STaxCodeID(6)	Description(7)	Rate(8)
                var item = new TaxesDTO();
                item.STaxClassKey = dataReader.GetInt32(0);
                item.STaxClassID = dataReader.GetString(1);
                item.STaxCodeClass = dataReader.GetInt16(2);
                item.UpdateCounter = dataReader.GetInt32(3);
                item.STaxCodeKey = dataReader.GetInt32(4);              
                item.STaxCodeID = dataReader.GetString(6);
                item.Description = dataReader.GetString(7);
                item.Rate = dataReader.GetDecimal(8);
                lista.Add(item);
            }
            cmd.Connection.Close();
        }
        return lista;

    }

    public static List<ItemDTO> get_items(string company_id)
    {
        var lista = new List<ItemDTO>();
        //agregar item vacio 
        lista.Add(new ItemDTO { ItemId = "", ItemKey = 0 });
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT dbo.vluItem.ItemKey, dbo.vluItem.LongDesc, dbo.vluItem.CompanyID, dbo.vluItem.PriceUnitMeasID, dbo.vluItem.STaxClassKey, dbo.timItem.ExpAcctKey,dbo.timItem.STaxClassKey FROM dbo.vluItem WITH(NOLOCK) INNER JOIN dbo.timItem ON dbo.vluItem.ItemKey = dbo.timItem.ItemKey WHERE (@CompanyID = dbo.vluItem.CompanyID) AND (dbo.vluItem.Status = 1) AND (dbo.vluItem.ItemType = 1 OR dbo.vluItem.ItemType = 3 OR dbo.vluItem.ItemType = 4) ORDER BY dbo.vluItem.ItemID, dbo.vluItem.ShortDesc;";
            cmd.Parameters.Add("@CompanyID", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var item = new ItemDTO();
                item.ItemKey = dataReader.GetInt32(0);
                item.ItemId = dataReader.GetString(1);
                item.CompanyID = dataReader.GetString(2);
                item.PriceUnitMeasId = dataReader.GetString(3);
                item.ExpAcctKey = dataReader.GetInt32(4);
                item.STaxClassKey = dataReader.GetInt32(5);
                lista.Add(item);
            }
            cmd.Connection.Close();
        }
        return lista;

    }

    public static void CheckSageIntegration(int id, DocumentType type)
    {
        string query_text = string.Empty;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            switch (type)
            {
                case DocumentType.Advance:
                    query_text = "UPDATE Advance SET SageIntegration = 1, Status = 5 WHERE AdvanceId = @id";
                    break;
                case DocumentType.Expense:
                    query_text = "UPDATE Expense SET SageIntegration = 1, Status = 5 WHERE ExpenseId = @id";
                    break;
                case DocumentType.CorporateCard:
                    query_text = "UPDATE CorporateCard SET SageIntegration = 1, Status = 5 WHERE CorporateCardId = @id";
                    break;
                case DocumentType.MinorMedicalExpense:
                    query_text = "UPDATE MinorMedicalExpense SET SageIntegration = 1, Status = 5 WHERE MinorMedicalExpenseId = @id";
                    break;
            }
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = query_text;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            cmd.CommandTimeout = 300;
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
            }
        }

    }

    public static ValidatingUserDTO get_MatrizValidadoresAnticipos(int pUserKey, int level)
    {

        ValidatingUserDTO matriz = new ValidatingUserDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (level)
            {
                case 1:
                    cmd.CommandText = "SELECT UserKey, Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserKey = @UserKey";
                    break;
                case 2:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserValidadorCX = @UserKey";
                    break;
                case 3:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserGerente = @UserKey";
                    break;
                case 4:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserRH = @UserKey";
                    break;
                case 5:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserTesoreria = @UserKey";
                    break;
                case 6:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserFinanzas = @UserKey";
                    break;
                default:
                    break;
            }

            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = pUserKey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                matriz.UserKey = dataReader.GetInt32(0);
                matriz.ValidadorCx = dataReader.GetInt32(1);
                matriz.Gerente = dataReader.GetInt32(2);
                matriz.Rh = dataReader.GetInt32(3);
                matriz.Tesoreria = dataReader.GetInt32(4);
                matriz.Finanzas = dataReader.GetInt32(5);
            }
        }
        return matriz;
    }

    public static string GetAprovalLevel(int level)
    {
        string nivel = string.Empty;
        switch (level)
        {
            case 0:                
                break;
            case 1:
                nivel = "Gerente / RH";
                break;
            case 2:
                nivel = "Cuentas Por Pagar";
                break;
            case 3:
                nivel = "Tesoreria";
                break;
            case 4:
                nivel = "Finanzas";
                break;
        }
        return nivel;
    }

    public static ValidatingUserDTO get_MatrizValidadores(int pUserKey)
    {        
        ValidatingUserDTO matriz = new ValidatingUserDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserKey, Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserKey = @UserKey";
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = pUserKey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                matriz.UserKey = dataReader.GetInt32(0);
                matriz.ValidadorCx = dataReader.GetInt32(1);
                matriz.Gerente = dataReader.GetInt32(2);
                matriz.Rh = dataReader.GetInt32(3);
                matriz.Tesoreria = dataReader.GetInt32(4);
                matriz.Finanzas = dataReader.GetInt32(5);
            }
        }
        return matriz;
    }    

    public static ValidatingTree get_JerarquiaValidadores(int document_type)
    {
        ValidatingTree jerarquia = new ValidatingTree();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RecursosHumanos,GerenciaArea,CuentasxPagar,Tesoreria,Finanzas FROM ValidatingTree where TypeKey = @TypeKey";
            cmd.Parameters.Add("@TypeKey", SqlDbType.Int).Value = document_type;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                jerarquia.RecursosHumanos = dataReader.GetBoolean(0);
                jerarquia.GerenciaArea = dataReader.GetBoolean(1);
                jerarquia.CuentasxPagar = dataReader.GetBoolean(2);
                jerarquia.Tesoreria = dataReader.GetBoolean(3);
                jerarquia.Finanzas = dataReader.GetBoolean(4);
            }
        }
        return jerarquia;
    }

    public enum DocumentType
    {
        Advance = 1, Expense = 2, CorporateCard = 3, MinorMedicalExpense = 4
    }

    public static Dictionary<int,string> TiposDocumentos()
    {
        var tipos = new Dictionary<int, string>
        {
            { 1, "Anticipo" },
            { 2, "Reembolso" },
            { 3, "Tarjeta Corporativa" },
            { 4, "Gasto Médico Menor" }
        };
        return tipos;
    }

    public enum Currencys
    {
        MNX = 1, USD = 2, EUR = 3        
    }

    public enum NotificationType
    {
        Aprobacion, Denegacion, Revision, Integracion
    }
   
    public  class Paquete
    {
        public int PackageId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public static Dictionary<int, string> Dict_status()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Pendiente" },
            { 2, "Aprobado" },
            { 3, "Denegado" },
            { 4, "Vencido" },
            { 5, "Integrado" },
        };
        return dict;
    }

    public static Dictionary<int, string> Dict_moneda()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Pesos" },
            { 2, "Dolar" },
            { 3, "Euros" }
        };
        return dict;
    }
    

    public static Dictionary<int, string> Dict_tipos_gastos()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT [Id] ,[Name] FROM [dbo].[ExpenseType]";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {               
                var id = dataReader.GetInt32(0);
                var name = dataReader.GetString(1);
                dict.Add(id, name);
            }
        }       
        
        return dict;
    }

    public static List<RolDTO> get_Roles()
    {
        List<RolDTO> roles = new List<RolDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RoleKey, RoleID FROM Roles where RoleKey in (7,8,9,10,11,12,13,14,15)";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var rol = new RolDTO();
                rol.Key = dataReader.GetInt32(0);
                rol.ID = dataReader.GetString(1);
                roles.Add(rol);
            }
        }
        return roles;
    }

    public static List<CurrExchSchdDtlDTO> get_ChangeRates()
    {
        List<CurrExchSchdDtlDTO> rates = new List<CurrExchSchdDtlDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from tmcCurrExchSchdDtl";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                //[CurrExchSchdKey]     ,[EffectiveDate]      ,[SourceCurrID]      ,[TargetCurrID]      ,[CurrExchRate]      ,[ExpirationDate]
                var rate = new CurrExchSchdDtlDTO();
                rate.CurrExchSchdKey = dataReader.GetInt32(0);
                rate.EffectiveDate = dataReader.GetDateTime(1);
                rate.SourceCurrID = dataReader.GetString(2);
                rate.TargetCurrID = dataReader.GetString(3);
                rate.CurrExchRate = dataReader.GetDouble(4);
                if(!dataReader.IsDBNull(5))
                {
                    rate.ExpirationDate = dataReader.GetDateTime(5);
                }
               
                rates.Add(rate);
            }
        }
        return rates;
    }     

    public static List<RolDTO> get_RolesValidadores()
    {
        List<RolDTO> roles = new List<RolDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Level, RoleID FROM ValidatingRoles";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var rol = new RolDTO();
                rol.Key = dataReader.GetInt32(0);
                rol.ID = dataReader.GetString(1);
                roles.Add(rol);
            }
        }
        return roles;

    }

    public static string getUserEmail(int userkey)
    {
        string email = string.Empty;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserName FROM AspNetUsers where UserKey = @UserKey";
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = userkey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                email = dataReader.GetString(0);
            }
        }
        return email;
    }        
     
    public static Dictionary<string, string> get_msg()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {           
            { "B20", "El importe supera el máximo permitido segun los datos facilitados." },
            { "B19", "La fecha de salida no puede ser anterior a la fecha actual, favor de verificar e intentar nuevamente." },
            { "B18", "El usuario cuenta con dos anticipos pendientes, favor de verificar e intentar nuevamente." },
            { "B17", "Jefe Inmediato requerido, favor de verificar e intentar nuevamente." },
            { "B16", "El importe solo debe contener números, punto o coma, favor de verificar e intentar nuevamente." },
            { "B15", "Importe del gasto requerido, favor de verificar e intentar nuevamente." },
            { "B14", "Tipo de moneda requerido, favor de verificar e intentar nuevamente." },
            { "B13", "Tipo del anticipo requerido, favor de verificar e intentar nuevamente." },
            { "B12", "Fecha del anticipo requerida, favor de verificar e intentar nuevamente." },
            { "B11", "No coindice el monto declarado con el total de la factura, favor de verificar e intentar nuevamente." },
            { "B10", "Folio requerido, favor de verificar e intentar nuevamente." },
            { "B9", "Uno de los archivos PDF Anexo no cuenta con formato PDF, favor de verificar el archivo e intentar nuevamente." },
            { "B8", "Uno de los archivos PDF FACTURA no cuenta con formato PDF, favor de verificar el archivo e intentar nuevamente." },
            { "B7", "Uno de los archivos XML Factura no cuenta con formato XML, favor de verificar el archivo e intentar nuevamente." },
            { "B5", "Factura Procesada." },
            { "B4", "Al intentar cargar los documentos en el servidor vuelva a intentarlo, en caso de persistir el problema comunicate con el área de sistemas." },
            { "B3", "La fecha de llegada debe ser posterior a la fecha de salida, favor de verificar e intentar nuevamente." },
            { "B2", "Fecha de llegada requerida, favor de verificar e intentar nuevamente." },
            { "B1", "Fecha de salida requerida, favor de verificar e intentar nuevamente." },
            { "B30", "El paquete actual no contiene pagos, favor de verificar e intentar nuevamente." },
            { "B31", "Motivo de la cancelacion requerido, favor de verificar e intentar nuevamente." },
            { "B32", "El gasto se ha integrado satisfactoriamente en Sage." },
            { "B33", "El gasto no se ha podido integrar a Sage, favor de verificar e intentar nuevamente." },
            { "B34", "El gasto se ha integrado satisfactoriamente en Sage." },
            { "B35", "El gasto se ha integrado previamente en Sage." },
            { "B36", "Tipo de gasto requerido, favor de verificar e intentar nuevamente." },
            { "B37", "El gasto se ha integrado previamente en Sage." },
            { "B38", "El gasto se ha integrado previamente en Sage." },
            { "B39", "La cantidad solo debe contener números y ser superior a cero, favor de verificar e intentar nuevamente." },
            { "B40", "Debe especificar el gasto, favor de verificar e intentar nuevamente." },
            { "B41", "El importe debe ser superior a cero, favor de verificar e intentar nuevamente." },
            { "B42", "Está introduciendo un reembolso sin anticipo." },
            { "MB42", "Uno de los archivos PDF VOUCHER supera el tamaño máximo permitido (15 MB)." },
            { "MB41", "La lista de gastos esta vacía, por favor inserte al menos uno." },
            { "MB40", "No coincide el importe total del gasto con los artículos e impuestos declarados." },
            { "MB39", "Ya existe una tarjeta con ese monto el dia actual." },
            { "MB38", "Solo se admiten gastos del mes actual." },
            { "MB37", "No coincide el importe total del gasto con los artículos e impuestos declarados." },
            { "MB36", "Ya existe un Gasto Medico Menor con ese monto el dia actual." },
            { "MB35", "Debe especificar los elementos del gasto." },
            { "MB34", "La suma de los reembolsos rebasa el importe del anticipo." },
            { "MB33", "Seleccione un Anexo en PDF para Cargar." },
            { "MB32", "Seleccione una Factura en PDF para Cargar." },
            { "MB31", "Seleccione una Factura en XML para Cargar." },
            { "MB30", "Uno de los archivos XML FACTURA supera el tamaño máximo permitido (15 MB)." },
            { "MB29", "Uno de los archivos XML Factura no cuenta con formato XML." },
            { "MB28", "Uno de los archivos PDF Voucher supera el tamaño máximo permitido (15 MB)." },
            { "MB27", "Uno de los archivos PDF FACTURA supera el tamaño máximo permitido (15 MB)." },
            { "MB26", "Tipo de gasto requerido, favor de verificar e intentar nuevamente." },
            { "MB25", "No se admiten gastos con más de 3 meses de anterioridad, favor de verificar e intentar nuevamente." },
            { "MB24", "Fecha del gasto requerida, favor de verificar e intentar nuevamente." },
            { "MB23", "El cantidad solo debe contener números, punto o coma y ser superior a cero, favor de verificar e intentar nuevamente." },
            { "MB22", "El importe solo debe contener números, punto o coma y ser superior a cero, favor de verificar e intentar nuevamente." },
            { "MB21", "Debe especificar el artículo, favor de verificar e intentar nuevamente." },
            { "MB43", "No puede subir un rembolso posterior a la fecha de comprobacion del anticipo." },
            { "MB50", "El gasto debe contener al menos un artículo" },
            { "1", "Política de Gastos de Transporte Aéreo" },
            { "2", "Política de Gastos de Transporte Terrestre" },
            { "3", "Política de Gastos de Casetas" },
            { "4", "El importe máximo para el tipo de gasto Gasolina es de 400" },
            { "5", "Política de Gastos de Estacionamiento" },
            { "6", "Política de Gastos de Alimentos" },
            { "7", "El importe máximo para el tipo de gasto Hospedaje es de 200" },
            { "8", "El importe máximo para el tipo de gasto Gastos Exraordinarios es de 2000" },
            { "B51", "El rol actual no puede validar documentos de este tipo" },
            { "B52", "Debe especificar el empleado, favor de verificar e intentar nuevamente." },
            { "B53", "Número de Paquete requerido, favor de verificar e intentar nuevamente." },
            { "B54", "Debe seleccionar el tipo de impuesto, favor de verificar e intentar nuevamente." },
            { "B55", "La fecha del artículo no debe ser anterior a la fecha del gasto." },
            { "B56", "La fecha del de final debe ser mayor que la de inicio." },
            { "B57", "Ya existe un pago con esa fecha e importe." },
            { "B58", "Ya existe ese tipo de gasto en base de datos." },
            { "B59", "El importe del reembolso según tipo de cambio excede el importe del anticipo en la moneda original." },
            { "B60", "No existe un tipo de cambio definido para esa moneda en la fecha señalada del gasto ." },


        };
        return dict;
    }

    public static Dictionary<int, string> Dict_Advancetype()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT [Id] ,[Name] FROM [dbo].[AdvanceType]";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var id = dataReader.GetInt32(0);
                var name = dataReader.GetString(1);
                dict.Add(id, name);
            }
        }
        return dict;        
    }

    public static ExpenseFilesDTO LoadFilesbyExpense(DocumentType type, ExpenseFilesDTO.FileType fileType, int expense_id, int detail_id)
    {
        var file = new ExpenseFilesDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.Clear();
            cmd.CommandText = "select Id,ExpenseType,ExpenseId,ExpenseDetailId,FileName,FileType,FileBinary  from Files Where FileType = @FileType and ExpenseId = @ExpenseId  and ExpenseDetailId = @ExpenseDetailId and ExpenseType = @ExpenseType";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
            cmd.Parameters.Add("@ExpenseDetailId", SqlDbType.Int).Value = detail_id;
            cmd.Parameters.Add("@ExpenseType", SqlDbType.Int).Value = (int)type;
            cmd.Parameters.Add("@FileType", SqlDbType.Int).Value = (int)fileType;
            cmd.Connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {               
                file.Id = reader.GetInt32(0);
                file.ExpenseType = (DocumentType)reader.GetInt32(1);
                file.ExpenseId = reader.GetInt32(2);
                file.ExpenseDetailId = reader.GetInt32(3);
                file.FileName = reader.GetString(4);
                file.Type = (FileType)reader.GetInt32(5);
                file.FileBinary = (byte[])reader.GetSqlBinary(6);               
            }
            
            cmd.Connection.Close();
        }
        return file.Id != 0 ? file :null ;
    }

    public static bool CheckFileExist(ExpenseFilesDTO file)
    {
        var val = string.Empty;
        int id = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select count(*)  from Files Where FileType = @FileType and ExpenseId = @ExpenseId and FileName = @FileName";            
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = file.ExpenseId;
            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = file.FileName;
            cmd.Parameters.Add("@FileType", SqlDbType.Int).Value = file.Type;           
            cmd.Connection.Open();
            var inserted = cmd.ExecuteScalar();
            val = inserted.ToString();
            id = Convert.ToInt32(val);
            cmd.Connection.Close();
        }
        return id > 0 ? true:false;
    }

    public static int SaveFile(ExpenseFilesDTO file)
    {
        var val = string.Empty;
        int id = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Files (ExpenseType,ExpenseId,ExpenseDetailId,FileName,FileType,FileBinary) VALUES (@ExpenseType,@ExpenseId,@ExpenseDetailId,@FileName,@FileType,@FileBinary); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.Add("@ExpenseType", SqlDbType.Int).Value = file.ExpenseType;
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = file.ExpenseId;
            cmd.Parameters.Add("@ExpenseDetailId", SqlDbType.Int).Value = file.ExpenseDetailId;
            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = file.FileName;
            cmd.Parameters.Add("@FileType", SqlDbType.Int).Value = file.Type;
            cmd.Parameters.Add("@FileBinary", SqlDbType.VarBinary, file.FileLength).Value = file.FileBinary;            
            cmd.Connection.Open();          
            var inserted = cmd.ExecuteScalar();
            val = inserted.ToString();
            id = Convert.ToInt32(val);
            cmd.Connection.Close();
        }
        return id;
        
    }

    public static void DeleteFile(DocumentType type ,int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (type)
            {               
                case DocumentType.Expense:
                    cmd.CommandText = "Delete from Files where ExpenseType = 2 and ExpenseId = @Id ";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "Delete from Files where ExpenseType = 3 and ExpenseId = @Id ";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "Delete from Files where ExpenseType = 4 and ExpenseId = @Id ";
                    break;
            }           
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;          
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }

    public static void DeleteExpenseOnFail(DocumentType type, int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (type)
            {
                case DocumentType.Advance:
                    cmd.CommandText = "Delete from Advance where AdvanceId = @Id";
                    break;
                case DocumentType.Expense:
                    cmd.CommandText = "Delete from Expense where ExpenseId = @Id";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "Delete from CorporateCard where CorporateCardId = @Id";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "Delete from MinorMedicalExpense where MinorMedicalExpenseId = @Id";
                    break;
            }
            
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
       
    }

    public static void DeleteDetailOnFail(DocumentType type, int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (type)
            {               
                case DocumentType.Expense:
                    cmd.CommandText = "Delete from ExpenseDetail where ExpenseId = @Id";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "Delete from MinorMedicalExpenseDetail where MinorMedicalExpenseId = @Id";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "Delete from CorporateCardDetail where CorporateCardId = @Id";
                    break;
            }

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }

    public static void DeleteExpense(DocumentType type,int expense_id, int user_id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (type)
            {
                case DocumentType.Advance:
                    cmd.CommandText = "Delete FROM Advance where UpdateUserKey = @UpdateUserKey and AdvanceId = @AdvanceId;";
                    break;
                case DocumentType.Expense:
                    cmd.CommandText = "Delete FROM Expense where UpdateUserKey = @UpdateUserKey and ExpenseId = @AdvanceId;";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "Delete FROM CorporateCard where UpdateUserKey = @UpdateUserKey and CorporateCardId = @AdvanceId;";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "Delete FROM MinorMedicalExpense where UpdateUserKey = @UpdateUserKey and MinorMedicalExpenseId = @AdvanceId;";
                    break;               
            }           
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = expense_id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            if(type != DocumentType.Advance)
            {
                DeleteDetailOnFail(type, expense_id);
                DeleteFile(type, expense_id);
            }
           
            
        }
    }   

    public static void EnviarCorreo(Doc_Tools.DocumentType type, int userkey, int aproval_level, NotificationType notification, int logged_user, string denial_reason = null)
    {      
        var matrix = Doc_Tools.get_MatrizValidadores(userkey);
        string tipo_documento = TiposDocumentos().FirstOrDefault(x => x.Key == (int)type).Value;
        List<string> to = new List<string>();
        string from = string.Empty;
        string subject = string.Empty;
        switch (type)
        {
            case Doc_Tools.DocumentType.Advance:
                switch (aproval_level)
                {
                    case 1:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                break;                            
                                
                        }                        
                        break;
                    case 2:                       
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;                           
                        }
                        break;
                    case 3:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Integracion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }                        
                        break;                   
                }
                break;
            case Doc_Tools.DocumentType.Expense:
                switch (aproval_level)
                {
                    case 1:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                break;
                        }
                        break;
                    case 2:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                    case 3:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                        }
                        break;
                    case 4:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Gerente));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Integracion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                }
                break;
            case Doc_Tools.DocumentType.CorporateCard:
                switch (aproval_level)
                {                   
                    case 2:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                    case 3:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                        }
                        break;
                    case 4:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Integracion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                }
                break;
            case Doc_Tools.DocumentType.MinorMedicalExpense:
                switch (aproval_level)
                {
                    case 1:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Rh));
                                break;
                        }
                        break;
                    case 2:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Rh));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                    case 3:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Rh));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                        }
                        break;
                    case 4:
                        switch (notification)
                        {
                            case NotificationType.Aprobacion:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Denegacion:
                                to.Add(Doc_Tools.getUserEmail(userkey));
                                to.Add(Doc_Tools.getUserEmail(matrix.Rh));
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                to.Add(Doc_Tools.getUserEmail(matrix.Tesoreria));
                                break;
                            case NotificationType.Revision:
                                to.Add(Doc_Tools.getUserEmail(matrix.Finanzas));
                                break;
                            case NotificationType.Integracion:
                                to.Add(Doc_Tools.getUserEmail(matrix.ValidadorCx));
                                break;
                        }
                        break;
                }
                break;
        }
        string texto = string.Empty;       
        
        switch (notification)
        {
            case NotificationType.Aprobacion:
                from = Doc_Tools.getUserEmail(logged_user);
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Account/Templates Email/AprobacionPago.html"))
                {
                    texto = reader.ReadToEnd();
                    texto = texto.Replace("{empleado}", from).Replace("{documento}", tipo_documento);
                }
                subject = string.Format("El usuario {0} ha aprobado un {1} para su revisión", from, tipo_documento);
                break;
            case NotificationType.Denegacion:
                from = Doc_Tools.getUserEmail(logged_user);
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory +  "/Account/Templates Email/DenegacionPago.html"))
                {
                    texto = reader.ReadToEnd();
                    texto = texto.Replace("{empleado}", from).Replace("{documento}", tipo_documento).Replace("{motivo}", denial_reason);
                }
                subject = string.Format("El usuario {0} ha denegado un {1} para su revisión", from, tipo_documento);
                break;
            case NotificationType.Revision:
                from = Doc_Tools.getUserEmail(userkey);
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Account/Templates Email/RevisionPago.html"))
                {
                    texto = reader.ReadToEnd();
                    texto = texto.Replace("{empleado}", from ).Replace("{documento}", tipo_documento);
                }
                subject = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, tipo_documento);
                break;
            case NotificationType.Integracion:
                from = Doc_Tools.getUserEmail(userkey);
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Account/Templates Email/IntegracionPago.html"))
                {
                    texto = reader.ReadToEnd();
                    texto = texto.Replace("{empleado}", from).Replace("{documento}", tipo_documento);
                }
                subject = string.Format("El usuario {0} ha aprobado un {1} para su integracion", from, tipo_documento);
                break;
        }
        foreach (var direccion in to)
        {
            bool SendEmail = Global.EmailGlobal(direccion, texto, subject);
        }       

    }

    public static int GetGerenteToNotify(int userkey)
    {
        int gerente_key = 0;
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Isnull([UserGerente],0) FROM [dbo].[validatingUser] where UserKey = @UserKey";
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = userkey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                gerente_key = dataReader.GetInt32(0);
            }
        }
        return gerente_key;
    }

    public static bool CheckPassword(int pUserKey, string password)
    {
        string UserName = string.Empty;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();            
            cmd.CommandText = "SELECT UserID  FROM Users where UserKey = @UserKey;";                    
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = pUserKey;          
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while(lector.Read())
            {
                UserName = lector.GetString(0);
            }
            cmd.Connection.Close();
        }
        var manager = new UserManager();
        var user = manager.FindByName(UserName);       
        return manager.CheckPassword(user, password);
    }

    public static string Get_token_from_db(int userkey)
    {
        string token = string.Empty;  
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select token from SecurityToken Where Userkey = @userkey and Activo = 1;";
                cmd.Parameters.AddWithValue("@Userkey", SqlDbType.Int).Value = userkey;
                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    token = reader.GetString(0);
                }
            }
           
        }
        catch (Exception exp)
        {

        }
        return token;
    }

    public static void VerificarAnticiposPendientes()
    {
        List<AdvanceDTO> anticipos = new List<AdvanceDTO>();
        int count = 0;
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = " SELECT count(*) FROM dbo.Pendant_Documents where [Current_date] = CAST( GETDATE() AS Date ) ;";            
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {               
                count = dataReader.GetInt32(0);                
            }
        }
        if(count==0)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Currency, Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,CreateDate, UpdateDate,CompanyId,Status FROM Advance where Status = @Status;";
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Connection.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var advance = new AdvanceDTO();
                    advance.AdvanceId = dataReader.GetInt32(0);
                    advance.AdvanceType = Dict_Advancetype().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
                    advance.Folio = dataReader.GetString(2);
                    advance.Currency = dataReader.GetInt32(3);
                    advance.Amount = dataReader.GetDecimal(4);
                    if (!dataReader.IsDBNull(5))
                    {
                        advance.DepartureDate = dataReader.GetDateTime(5);
                    }
                    if (!dataReader.IsDBNull(6))
                    {
                        advance.ArrivalDate = dataReader.GetDateTime(6);
                    }
                    advance.CheckDate = dataReader.GetDateTime(7);
                    advance.ImmediateBoss = dataReader.GetString(8);
                    advance.UpdateUserKey = dataReader.GetInt32(9);
                    advance.CreateDate = dataReader.GetDateTime(10);
                    advance.UpdateDate = dataReader.GetDateTime(11);
                    advance.CompanyId = dataReader.GetString(12);
                    advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x => x.Key == dataReader.GetInt32(13)).Value;
                    anticipos.Add(advance);
                }
            }

            var grouped_user = anticipos.GroupBy(x => x.UpdateUserKey);
            foreach (var user in grouped_user)
            {
                foreach (AdvanceDTO anticipo_pendients in user.ToList())
                {                    
                    EnviarCorreo(DocumentType.Advance, user.Key, 1, NotificationType.Revision, user.Key);                
                   
                }
            }
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[Pendant_Documents] ([Current_date])  VALUES (GETDATE()) ;";                
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();                
            }
        }
        

    }

    public static void SetDefaultArticle(int itemkey, string companyId)
    {      
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "if exists (SELECT [ItemKey] FROM [dbo].[DefaultItemMME] where [CompanyID] = @CompanyID) begin  UPDATE[dbo].[DefaultItemMME]  SET [ItemKey] = @ItemKey, [CompanyID] = @CompanyID, [DateUpdated] = GETDATE()  WHERE [CompanyID] = @CompanyID end else  begin INSERT INTO[dbo].[DefaultItemMME]([ItemKey],[CompanyID],[DateUpdated])  VALUES(@ItemKey, @CompanyID, Getdate()) end ";
            cmd.Parameters.Add("@ItemKey", SqlDbType.Int).Value = itemkey;
            cmd.Parameters.Add("@CompanyID", SqlDbType.NVarChar).Value = companyId;          
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();            
            cmd.Connection.Close();
        }
    }

    public static ItemDTO GetDefaultItem(string companyId)
    {
        int itemKey = 0;
        ItemDTO item = new ItemDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IsNull([ItemKey],0) FROM [dbo].[DefaultItemMME] where [CompanyID] = @CompanyID";
            cmd.Parameters.Add("@CompanyID", SqlDbType.NVarChar).Value = companyId;
            cmd.Connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                itemKey = reader.GetInt32(0);
            }
            cmd.Connection.Close();
        }
        if (itemKey != 0)
        {
            item = get_items(companyId).FirstOrDefault(x => x.ItemKey == itemKey);
            return item;
        }        
        return null;
    }

    public static string get_causante(int userkey)
    {
        string empleado = string.Empty;
        ItemDTO item = new ItemDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT isnull(UserID, '') FROM [dbo].[Users] where UserKey = @UserKey";
            cmd.Parameters.Add("@UserKey", SqlDbType.NVarChar).Value = userkey;
            cmd.Connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                empleado = reader.GetString(0);
            }
            cmd.Connection.Close();
        }        
        return empleado;
    }
   

}
