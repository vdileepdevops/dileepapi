using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FinstaInfrastructure.Banking.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Reports;
using FinstaRepository.Interfaces.Banking.Transactions;
using System.Threading.Tasks;
using HelperManager;
using Npgsql;

namespace FinstaRepository.DataAccess.Banking.Reports
{
    public class MemberEnquiryDAL : SettingsDAL, IMemberEnquiry
    {

        public List<MemberDetailsDTO> GetMemberDetails(string Connectionstring)
        {
            List<MemberDetailsDTO> lstMemberDetails = new List<MemberDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select memberid,membercode,membername,tc.fathername,businessentitycontactno as phoneno,(case when ta.contactid is not null then address1||','||city||','||district||','||state||','||country||'-'||pincode else null  end )::text as Address from tblmstmembers tm join tblmstcontact tc on tm.contactid=tc.contactid left join tblmstcontactaddressdetails ta on tm.contactid=ta.contactid and ta.priority='PRIMARY'  where tm.statusid=" + Convert.ToInt32(Status.Active) + " order by membername";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberDetailsDTO objMemberDetailsDTO = new MemberDetailsDTO();
                        objMemberDetailsDTO.pMemberid = dr["memberid"];
                        objMemberDetailsDTO.pMembername = dr["membername"];
                        objMemberDetailsDTO.pMembercode = dr["membercode"];
                        objMemberDetailsDTO.pMobileno = dr["phoneno"];
                        objMemberDetailsDTO.pAddress = dr["Address"];
                        objMemberDetailsDTO.pFathername = dr["fathername"];
                        lstMemberDetails.Add(objMemberDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberDetails;
        }
        public async Task<List<MemberTransactionDTO>> GetMemberTransactions(long Memberid, string ConnectionString)
        {
            List<MemberTransactionDTO> LstMemberTransactions = new List<MemberTransactionDTO>();
            await Task.Run(() =>
            {

                try
                {
                    MemberTransactionDTO _objMemberTransactionDTO = new MemberTransactionDTO
                    {
                        // using
                        pContactImagePath = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select contactimagepath from tblmstcontact tc join tblmstmembers tm on tc.contactid=tm.contactid where tm.memberid=" + Memberid + "; ")),
                        LstMemberBankDetails = GetMemberBankDetails(Memberid, ConnectionString),
                        LstMemberTransactionDetails = GetMemberTransactionDetails(Memberid, ConnectionString)

                    };
                    LstMemberTransactions.Add(_objMemberTransactionDTO);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return LstMemberTransactions;
        }
        public List<MemberBankDetailsDTO> GetMemberBankDetails(long Memberid, string Connectionstring)
        {
            List<MemberBankDetailsDTO> lstMemberBankDetails = new List<MemberBankDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select bankname,branch,accountno,ifsccode from tabapplicationpersonalbankdetails where applicationid =" + Memberid + " and statusid=" + Convert.ToInt32(Status.Active) + "";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberBankDetailsDTO objMemberBankDetailsDTO = new MemberBankDetailsDTO();
                        objMemberBankDetailsDTO.pBankname = dr["bankname"];
                        objMemberBankDetailsDTO.pBranch = dr["branch"];
                        objMemberBankDetailsDTO.pAccountno = dr["accountno"];
                        objMemberBankDetailsDTO.pIfsccode = dr["ifsccode"];
                        lstMemberBankDetails.Add(objMemberBankDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberBankDetails;
        }

        public List<MemberTransactionDetailsDTO> GetMemberTransactionDetails(long Memberid, string Connectionstring)
        {
            List<MemberTransactionDetailsDTO> lstMemberTransactionDetails = new List<MemberTransactionDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select fdaccountid,fdaccountno,membername,contacttype,applicanttype,fdconfigid as schemeid,fdname as Schemename,chitbranchid,chitbranchname,depositamount,tenor||' '||tenortype as tenure,interesttype,interestrate,maturityamount,interestpayable,fdcalculationmode,interestpayout,paidamount,clearedmount as receivedamount,balanceamount,pendingchequeamount,to_char(transdate, 'dd/Mon/yyyy')transactiondate,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy')maturitydate from vwfdtransaction_details where memberid=" + Memberid + "";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberTransactionDetailsDTO objMemberTransactionDetailsDTO = new MemberTransactionDetailsDTO();
                        objMemberTransactionDetailsDTO.pFdaccountid = dr["fdaccountid"];
                        objMemberTransactionDetailsDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberTransactionDetailsDTO.pMembername = dr["membername"];
                        objMemberTransactionDetailsDTO.pContacttype = dr["contacttype"];
                        objMemberTransactionDetailsDTO.pApplicanttype = dr["applicanttype"];
                        objMemberTransactionDetailsDTO.pSchemeid = dr["schemeid"];
                        objMemberTransactionDetailsDTO.pSchemename = dr["Schemename"];
                        objMemberTransactionDetailsDTO.pChitbranchid = dr["chitbranchid"];
                        objMemberTransactionDetailsDTO.pChitbranchname = dr["chitbranchname"];
                        objMemberTransactionDetailsDTO.pDepositamount = dr["depositamount"];
                        objMemberTransactionDetailsDTO.pTenure = dr["tenure"];
                        objMemberTransactionDetailsDTO.pInteresttype = dr["interesttype"];
                        objMemberTransactionDetailsDTO.pInterestrate = dr["interestrate"];
                        objMemberTransactionDetailsDTO.pMaturityamount = dr["maturityamount"];
                        objMemberTransactionDetailsDTO.pInterestpayable = dr["interestpayable"];
                        objMemberTransactionDetailsDTO.pFdcalculationmode = dr["fdcalculationmode"];
                        objMemberTransactionDetailsDTO.pInterestpayout = dr["interestpayout"];
                        objMemberTransactionDetailsDTO.pPaidamount = dr["paidamount"];
                        objMemberTransactionDetailsDTO.pReceivedamount = dr["receivedamount"];
                        objMemberTransactionDetailsDTO.pBalanceamount = dr["balanceamount"];
                        objMemberTransactionDetailsDTO.pPendingchequeamount = dr["pendingchequeamount"];
                        objMemberTransactionDetailsDTO.pTransactiondate = dr["transactiondate"];
                        objMemberTransactionDetailsDTO.pDepositdate = dr["depositdate"];
                        objMemberTransactionDetailsDTO.pMaturitydate = dr["maturitydate"];


                        lstMemberTransactionDetails.Add(objMemberTransactionDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberTransactionDetails;
        }
        public List<MemberReceiptDTO> GetMemberReceiptDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberReceiptDTO> lstMemberReceipts = new List<MemberReceiptDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select tm.membername,tm.membercode,ft.fdaccountno,to_char(fr.fd_receiptt_date, 'dd/Mon/yyyy')fd_receiptt_date,ft.depositamount,fr.instalment_amount as dueamount,fr.received_amount,fr.mode_of_receipt,fr.receipt_no,(case when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus = 'R' then 'Returned' when tt.depositstatus = 'C' then 'Cancelled'  when tt.clearstatus IS NULL then 'Cleared' else 'Un Cleared' end)as ChequeStatus from fd_receipt fr join tblmstmembers tm on fr.member_id = tm.memberid join tbltransfdcreation ft on fr.fd_account_id = ft.fdaccountid and fr.status = true left join tbltransreceiptreference tt on tt.receiptid=fr.receipt_no where ft.fdaccountno='" + FdAccountNo + "'   order by fd_receipt_id desc;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberReceiptDTO objMemberReceiptDTO = new MemberReceiptDTO();
                        objMemberReceiptDTO.pMembername = dr["membername"];
                        objMemberReceiptDTO.pMembercode = dr["membercode"];
                        objMemberReceiptDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberReceiptDTO.pFdreceipttdate = dr["fd_receiptt_date"];
                        objMemberReceiptDTO.pAdvanceamount = dr["depositamount"];
                        objMemberReceiptDTO.pReceivedamount = dr["received_amount"];
                        objMemberReceiptDTO.pModeofreceipt = dr["mode_of_receipt"];
                        objMemberReceiptDTO.pReceiptno = dr["receipt_no"];
                        objMemberReceiptDTO.pChequestatus = dr["chequestatus"];
                        lstMemberReceipts.Add(objMemberReceiptDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberReceipts;
        }
        public List<MemberNomineeDetailsDTO> GetMemberNomineeDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberNomineeDetailsDTO> lstMemberNomineeDetails = new List<MemberNomineeDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select nomineename,contactno,relationship,idproofname,referencenumber,percentage from tabapplicationpersonalnomineedetails where vchapplicationid='" + FdAccountNo + "' and percentage>0 and  statusid=" + Convert.ToInt32(Status.Active) + ";";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberNomineeDetailsDTO objMemberNomineeDetailsDTO = new MemberNomineeDetailsDTO();
                        objMemberNomineeDetailsDTO.pNomineename = dr["nomineename"];
                        objMemberNomineeDetailsDTO.pContactno = dr["contactno"];
                        objMemberNomineeDetailsDTO.pRelationship = dr["relationship"];
                        objMemberNomineeDetailsDTO.pReferencenumber = dr["referencenumber"];
                        objMemberNomineeDetailsDTO.pIdproofname = dr["idproofname"];
                        objMemberNomineeDetailsDTO.pProportion = dr["percentage"];

                        lstMemberNomineeDetails.Add(objMemberNomineeDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberNomineeDetails;
        }
        public List<MemberInterestPaymentDTO> GetMemberInterestPaymentDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberInterestPaymentDTO> MemberInterestPaymentList = new List<MemberInterestPaymentDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select paymentid,to_char(paymentdate, 'dd/Mon/yyyy')paymentdate,fdaccountno,fdname,payment_type,depositamount,interest_amount,tds_amount,total_amount,interest_paid_month from vwinterest_payment_report_paid where fdaccountno='" + FdAccountNo + "';";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberInterestPaymentDTO objMemberInterestPaymentDTO = new MemberInterestPaymentDTO();
                        objMemberInterestPaymentDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberInterestPaymentDTO.pSchemeName = dr["fdname"];
                        objMemberInterestPaymentDTO.pPaymentType = dr["payment_type"];
                        objMemberInterestPaymentDTO.pDepositamount = dr["depositamount"];
                        objMemberInterestPaymentDTO.pInterestAmount = dr["interest_amount"];
                        objMemberInterestPaymentDTO.pTdsAmount = dr["tds_amount"];
                        objMemberInterestPaymentDTO.pTotalAmount = dr["total_amount"];
                        objMemberInterestPaymentDTO.pInterestpaidmonth = dr["interest_paid_month"];
                        objMemberInterestPaymentDTO.pPaymentdate = dr["paymentdate"];
                        objMemberInterestPaymentDTO.pPaymentid = dr["paymentid"];

                        MemberInterestPaymentList.Add(objMemberInterestPaymentDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return MemberInterestPaymentList;
        }
        public List<MemberPromoterSalaryDTO> GetMemberPromoterSalaryDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberPromoterSalaryDTO> MemberPromoterSalaryList = new List<MemberPromoterSalaryDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select x.*,y.paymentid,to_char(y.paymentdate, 'dd/Mon/yyyy')paymentdate from(select voucher_id,to_char(fd.transdate, 'dd/Mon/yyyy')transdate,to_char(commission_trans_date::date, 'dd/Mon/yyyy')commission_trans_date,membername,fd.fdaccountno,commission_payment_id,commssion_amount,tds_amount,(commssion_amount-coalesce(tds_amount,0)) as totalamount,fd.depositamount,fd.tenor||' '||fd.tenortype as tenor,c.referralname as agentname,c.commsssionvalue from commission_payment cp  join tbltransfdcreation fd on fd.fdaccountid=cp.trans_type_id left join tbltransfdreferraldetails c on c.fdaccountno=fd.fdaccountno where  (voucher_id is not null or voucher_id<>0)and fd.fdaccountno='" + FdAccountNo + "')x JOIN ( SELECT b.recordid,a.paymentid,a.paymentdate,a.transtype AS modeofpayment, a.bankname AS paybank,a.branchname AS paybankbranch,a.chequenumber FROM tbltranspaymentreference a LEFT JOIN tbltranspaymentvoucher b ON a.paymentid::text = b.paymentid::text) y ON x.voucher_id = y.recordid; ; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberPromoterSalaryDTO objMemberPromoterSalaryDTO = new MemberPromoterSalaryDTO();
                        objMemberPromoterSalaryDTO.pTransdate = dr["transdate"];
                        objMemberPromoterSalaryDTO.pCommissiontransdate = dr["commission_trans_date"];
                        objMemberPromoterSalaryDTO.pMembername = dr["membername"];
                        objMemberPromoterSalaryDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberPromoterSalaryDTO.pCommissionpaymentid = dr["commission_payment_id"];
                        objMemberPromoterSalaryDTO.pCommssionAmount = dr["commssion_amount"];
                        objMemberPromoterSalaryDTO.pTdsAmount = dr["tds_amount"];
                        objMemberPromoterSalaryDTO.pTotalAmount = dr["totalamount"];
                        objMemberPromoterSalaryDTO.pDepositamount = dr["depositamount"];
                        objMemberPromoterSalaryDTO.pTenure = dr["tenor"];
                        objMemberPromoterSalaryDTO.pAgentname = dr["agentname"];
                        objMemberPromoterSalaryDTO.pCommsssionvalue = dr["commsssionvalue"];
                        objMemberPromoterSalaryDTO.pPaymentid = dr["paymentid"];
                        objMemberPromoterSalaryDTO.pPaymentdate = dr["paymentdate"];


                        MemberPromoterSalaryList.Add(objMemberPromoterSalaryDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return MemberPromoterSalaryList;
        }
        public List<MemberLiensDTO> GetMemberLiensDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberLiensDTO> MemberLiensList = new List<MemberLiensDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select lienid,to_char(liendate, 'dd/Mon/yyyy')liendate,td.membername,to_char(td.depositdate,'dd/Mon/yyyy')depositdate,td.depositamount,lienamount,td.tenor||' '||tenortype as tenure,td.interestrate,td.interestpayable,td.interesttype,tl.membercode,tl.fdaccountno,companybranch,lienadjuestto,(case when lienstatus='N' then 'Not Released' else 'Lien Released' end)as lienstatus from vwfdtransaction_details td join tbltranslienentry tl on td.fdaccountno=tl.fdaccountno  where  statusid =1 and tl.fdaccountno='" + FdAccountNo + "'; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberLiensDTO objMemberLiensDTO = new MemberLiensDTO();
                        objMemberLiensDTO.pLiendate = dr["liendate"];
                        objMemberLiensDTO.pMembername = dr["membername"];
                        objMemberLiensDTO.pDepositdate = dr["depositdate"];
                        objMemberLiensDTO.pDepositAmount = dr["depositamount"];
                        objMemberLiensDTO.pTenure = dr["tenure"];
                        objMemberLiensDTO.pInterestrate = dr["interestrate"];
                        objMemberLiensDTO.pInterestpayable = dr["interestpayable"];
                        objMemberLiensDTO.pInteresttype = dr["depositamount"];
                        objMemberLiensDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberLiensDTO.pCompanybranch = dr["companybranch"];
                        objMemberLiensDTO.pLienAdjustTo = dr["lienadjuestto"];
                        objMemberLiensDTO.pLienAmount = dr["lienamount"];
                        objMemberLiensDTO.pLienstatus = dr["lienstatus"];



                        MemberLiensList.Add(objMemberLiensDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return MemberLiensList;
        }
        public List<MemberMaturityBondDTO> GetMemberMaturityBondsDetails(string FdAccountNo, string Connectionstring)
        {
            List<MemberMaturityBondDTO> MemberMaturityBondsList = new List<MemberMaturityBondDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select tm.membername,(case when t.trans_type='Fd' then 'Advance' else t.trans_type end) trans_type,tf.fdaccountno,to_char(t.trans_date, 'dd/Mon/yyyy')trans_date,depositamount,(tenor||' '||tenortype)tenure,fdname,t.mature_amount,t.interest_payble,agent_commssion_payable,damages,interes_paid,net_payable,maturity_type from maturity_bonds t join tblmstmembers tm on t.member_id=tm.memberid join tbltransfdcreation tf on tf.fdaccountid=t.trans_type_id  where status=true and tf.fdaccountno='" + FdAccountNo + "'; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberMaturityBondDTO objMemberMaturityBondDTO = new MemberMaturityBondDTO();
                        objMemberMaturityBondDTO.pMembername = dr["membername"];
                        objMemberMaturityBondDTO.pTranstype = dr["trans_type"];
                        objMemberMaturityBondDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberMaturityBondDTO.pTransdate = dr["trans_date"];
                        objMemberMaturityBondDTO.pMatureamount = dr["mature_amount"];
                        objMemberMaturityBondDTO.pInterestpayble = dr["interest_payble"];
                        objMemberMaturityBondDTO.pAgentCommssionPayable = dr["agent_commssion_payable"];
                        objMemberMaturityBondDTO.pDamages = dr["damages"];
                        objMemberMaturityBondDTO.pInteresPaid = dr["interes_paid"];
                        objMemberMaturityBondDTO.pNetpayable = dr["net_payable"];
                        objMemberMaturityBondDTO.pMaturitytype = dr["maturity_type"];
                        objMemberMaturityBondDTO.pDepositamount = dr["depositamount"];
                        objMemberMaturityBondDTO.pTenure = dr["tenure"];
                        objMemberMaturityBondDTO.pSchemeName = dr["fdname"];



                        MemberMaturityBondsList.Add(objMemberMaturityBondDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return MemberMaturityBondsList;
        }
        public List<MaturityPaymentsDTO> GetMemberMaturityPaymentsDetails(string FdAccountNo, string Connectionstring)
        {
            List<MaturityPaymentsDTO> MaturityPaymentsList = new List<MaturityPaymentsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select distinct y.paymentid as voucherid,to_char(y.paymentdate, 'dd/Mon/yyyy')paymentdate,y.modeofpayment,tm.membername,tm.membercode,tf.fdaccountno,to_char(tf.depositdate, 'dd/Mon/yyyy')depositdate,depositamount,(tenor||' '||tenortype)tenure,tf.maturityamount,tf.interestpayout,to_char(tp.payment_date,'dd/Mon/yyyy')maturity_payment_date,tp.paid_amount,payment_type from maturity_payments tp join tblmstmembers tm on tm.memberid=tp.member_id join tbltransfdcreation tf on tf.fdaccountid=tp.trans_type_id left join tbltranspaymentvoucher y on tp.voucher_id=y.recordid where tf.fdaccountno='" + FdAccountNo + "'; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityPaymentsDTO objMaturityPaymentsDTO = new MaturityPaymentsDTO();
                        objMaturityPaymentsDTO.pMembername = dr["membername"];
                        objMaturityPaymentsDTO.pMembercode = dr["membercode"];
                        objMaturityPaymentsDTO.pFdaccountno = dr["fdaccountno"];
                        objMaturityPaymentsDTO.pMaturitypaymentdate = dr["maturity_payment_date"];
                        objMaturityPaymentsDTO.pDepositdate = dr["depositdate"];
                        objMaturityPaymentsDTO.pDepositamount = dr["depositamount"];
                        objMaturityPaymentsDTO.pTenure = dr["tenure"];
                        objMaturityPaymentsDTO.pMaturityamount = dr["maturityamount"];
                        objMaturityPaymentsDTO.pInterestpayout = dr["interestpayout"];
                        objMaturityPaymentsDTO.pPaymentType = dr["payment_type"];
                        objMaturityPaymentsDTO.pPaidAmount = dr["paid_amount"];
                        objMaturityPaymentsDTO.pVoucherid = dr["voucherid"];
                        objMaturityPaymentsDTO.pPaymentdate = dr["paymentdate"];
                        objMaturityPaymentsDTO.pModeofpayment = dr["modeofpayment"];



                        MaturityPaymentsList.Add(objMaturityPaymentsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return MaturityPaymentsList;
        }




        #region Member Enquiry Details Report ...

        public List<MemberDetailsDTO> GetMemberDetailsByid(long Memberid, string Connectionstring)
        {
            List<MemberDetailsDTO> lstMemberDetails = new List<MemberDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select memberid,membercode,membername,tc.fathername,businessentitycontactno as phoneno,(case when ta.contactid is not null then address1||','||city||','||district||','||state||','||country||'-'||pincode else null  end )::text as Address from tblmstmembers tm join tblmstcontact tc on tm.contactid=tc.contactid left join tblmstcontactaddressdetails ta on tm.contactid=ta.contactid and ta.priority='PRIMARY'  where memberid=" + Memberid + ";";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberDetailsDTO objMemberDetailsDTO = new MemberDetailsDTO();
                        objMemberDetailsDTO.pMemberid = dr["memberid"];
                        objMemberDetailsDTO.pMembername = dr["membername"];
                        objMemberDetailsDTO.pMembercode = dr["membercode"];
                        objMemberDetailsDTO.pMobileno = dr["phoneno"];
                        objMemberDetailsDTO.pAddress = dr["Address"];
                        objMemberDetailsDTO.pFathername = dr["fathername"];
                        lstMemberDetails.Add(objMemberDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberDetails;
        }
        public List<MemberTransactionDetailsDTO> GetMemberTransactionDetailsByid(string FdAccountNo, string Connectionstring)
        {
            List<MemberTransactionDetailsDTO> lstMemberTransactionDetails = new List<MemberTransactionDetailsDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select fdaccountid,fdaccountno,membername,contacttype,applicanttype,fdconfigid as schemeid,fdname as Schemename,chitbranchid,chitbranchname,depositamount,tenor||' '||tenortype as tenure,interesttype,interestrate,maturityamount,interestpayable,fdcalculationmode,interestpayout,paidamount,clearedmount as receivedamount,balanceamount,pendingchequeamount,to_char(transdate, 'dd/Mon/yyyy')transactiondate,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy')maturitydate from vwfdtransaction_details where fdaccountno='"+FdAccountNo+"'";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberTransactionDetailsDTO objMemberTransactionDetailsDTO = new MemberTransactionDetailsDTO();
                        objMemberTransactionDetailsDTO.pFdaccountid = dr["fdaccountid"];
                        objMemberTransactionDetailsDTO.pFdaccountno = dr["fdaccountno"];
                        objMemberTransactionDetailsDTO.pMembername = dr["membername"];
                        objMemberTransactionDetailsDTO.pContacttype = dr["contacttype"];
                        objMemberTransactionDetailsDTO.pApplicanttype = dr["applicanttype"];
                        objMemberTransactionDetailsDTO.pSchemeid = dr["schemeid"];
                        objMemberTransactionDetailsDTO.pSchemename = dr["Schemename"];
                        objMemberTransactionDetailsDTO.pChitbranchid = dr["chitbranchid"];
                        objMemberTransactionDetailsDTO.pChitbranchname = dr["chitbranchname"];
                        objMemberTransactionDetailsDTO.pDepositamount = dr["depositamount"];
                        objMemberTransactionDetailsDTO.pTenure = dr["tenure"];
                        objMemberTransactionDetailsDTO.pInteresttype = dr["interesttype"];
                        objMemberTransactionDetailsDTO.pInterestrate = dr["interestrate"];
                        objMemberTransactionDetailsDTO.pMaturityamount = dr["maturityamount"];
                        objMemberTransactionDetailsDTO.pInterestpayable = dr["interestpayable"];
                        objMemberTransactionDetailsDTO.pFdcalculationmode = dr["fdcalculationmode"];
                        objMemberTransactionDetailsDTO.pInterestpayout = dr["interestpayout"];
                        objMemberTransactionDetailsDTO.pPaidamount = dr["paidamount"];
                        objMemberTransactionDetailsDTO.pReceivedamount = dr["receivedamount"];
                        objMemberTransactionDetailsDTO.pBalanceamount = dr["balanceamount"];
                        objMemberTransactionDetailsDTO.pPendingchequeamount = dr["pendingchequeamount"];
                        objMemberTransactionDetailsDTO.pTransactiondate = dr["transactiondate"];
                        objMemberTransactionDetailsDTO.pDepositdate = dr["depositdate"];
                        objMemberTransactionDetailsDTO.pMaturitydate = dr["maturitydate"];


                        lstMemberTransactionDetails.Add(objMemberTransactionDetailsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberTransactionDetails;
        }



        public List<MemberEnquiryDTO> GetMemberEnquiryDetailsReport(string FdAccountNo, string ConnectionString)
        {
            long Memberid = 0;
            List<MemberEnquiryDTO> LstMemberEnquiryDetails = new List<MemberEnquiryDTO>();
            Memberid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select memberid from tbltransfdcreation  where fdaccountno  ='" + FdAccountNo + "'"));
            try
            {

                MemberEnquiryDTO _objMemberEnquiryDetailsDTO = new MemberEnquiryDTO
                {
                    pContactImagePath = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select contactimagepath from tblmstcontact tc join tblmstmembers tm on tc.contactid=tm.contactid where tm.memberid=" + Memberid + "; ")),
                    LstMemberDetails = GetMemberDetailsByid(Memberid, ConnectionString),
                    LstMemberBankDetails = GetMemberBankDetails(Memberid, ConnectionString),
                    LstMemberTransactionDetails = GetMemberTransactionDetailsByid(FdAccountNo, ConnectionString),                   
                    LstMemberReceiptDetails = GetMemberReceiptDetails(FdAccountNo, ConnectionString),
                    LstMemberNomieeDetails = GetMemberNomineeDetails(FdAccountNo, ConnectionString),
                    LstMemberInterestPaymentDetails = GetMemberInterestPaymentDetails(FdAccountNo, ConnectionString),
                    LstMemberPromoterSalarytDetails = GetMemberPromoterSalaryDetails(FdAccountNo, ConnectionString),
                    LstMemberLientDetails = GetMemberLiensDetails(FdAccountNo, ConnectionString),
                    LstMemberMaturityBondDetails = GetMemberMaturityBondsDetails(FdAccountNo, ConnectionString),
                    LstMemberMaturityPaymentsDetails = GetMemberMaturityPaymentsDetails(FdAccountNo, ConnectionString),
                };
                LstMemberEnquiryDetails.Add(_objMemberEnquiryDetailsDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return LstMemberEnquiryDetails;
        }

        #endregion


    }
}
