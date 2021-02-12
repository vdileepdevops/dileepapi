using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;
using System.Threading.Tasks;
namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class FDReceiptDAL : SettingsDAL, IFDReceipt
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region  Get Member Details
        public List<FDMemberDetailsDTO> GetMemberDetails(string MemberType, string BranchName, string Connectionstring)
        {
            List<FDMemberDetailsDTO> lstMemberDetails = new List<FDMemberDetailsDTO>();
            try
            {
                //select distinct tm.memberid,tm.Membername, tm.membercode, tc.contactid, tc.contactreferenceid, tm.membertype, tc.name, tc.businessentitycontactno from tblmstmembers tm join tblmstcontact tc on tm.contactid = tc.contactid join tbltransfdcreation tf on tm.memberid = tf.memberid where upper(tm.membertype) = '" + ManageQuote(MemberType.ToUpper()) + "'  and tf.chitbranchname = '"+ BranchName + "' and tm.statusid = 1 and tf.statusid = 1
                //string query = "select distinct memberid,membercode,membername,contactid,contactreferenceid,membertype,mobileno from vwfdtransaction_details where upper(membertype)='" + ManageQuote(MemberType.ToUpper()) + "' and chitbranchname ='" + BranchName + "' and balanceamount>0 order by membername";
                string query = "select * from (select distinct memberid,membercode,membername,fd.contactid,fd.contactreferenceid,membertype,mobileno,count(case when contacttype='Business Entity' then '1' else tn.vchapplicationid end)count from vwfdtransaction_details fd left join tabapplicationpersonalnomineedetails tn on tn.vchapplicationid=fd.fdaccountno  where upper(membertype)='" + ManageQuote(MemberType.ToUpper()) + "'  and balanceamount>0 and accountstatus='N'  group by memberid,membercode,membername,fd.contactid,fd.contactreferenceid,membertype,mobileno order by membername)tbl where count>0";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, query))
                {
                    while (dr.Read())
                    {
                        FDMemberDetailsDTO objMemberdetails = new FDMemberDetailsDTO();
                        objMemberdetails.pMemberid = dr["memberid"];
                        objMemberdetails.pMembercode = dr["membercode"];
                        objMemberdetails.pName = dr["Membername"];
                        objMemberdetails.pConid = dr["contactid"];
                        objMemberdetails.pContactreferenceid = dr["contactreferenceid"];
                        objMemberdetails.pMembertype = dr["membertype"];
                        objMemberdetails.pBusinessentitycontactno = dr["mobileno"];
                        lstMemberDetails.Add(objMemberdetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberDetails;
        }
        #endregion

        #region  Get Fd Details
        public List<FDDetailsDTO> GetFdDetails(string MemberCode, string ChitBranch, string Connectionstring)
        {
            List<FDDetailsDTO> lstFDDetails = new List<FDDetailsDTO>();
            try
            {
                //select fdaccountid, fdaccountno, membername, chitbranchname, depositamount, accountid from tbltransfdcreation  where upper(membercode) = '" + ManageQuote(MemberCode.ToUpper()) + "' and upper(chitbranchname)= '"+ManageQuote(ChitBranch.ToUpper())+"'
                //string query = "select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid from vwfdtransaction_details where upper(membercode) ='" + ManageQuote(MemberCode.ToUpper()) + "' and upper(chitbranchname)='" + ManageQuote(ChitBranch.ToUpper()) + "' and balanceamount>0 ";
                string query = "select * from (select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,count(case when contacttype='Business Entity' then '1' else tn.vchapplicationid end)count from vwfdtransaction_details fd left join tabapplicationpersonalnomineedetails tn on tn.vchapplicationid=fd.fdaccountno where upper(membercode) ='" + ManageQuote(MemberCode.ToUpper()) + "' and balanceamount>0 group by  fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid)tbl where count>0 ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, query))
                {
                    while (dr.Read())
                    {
                        FDDetailsDTO objFDdetails = new FDDetailsDTO();
                        objFDdetails.pFdaccountid = Convert.ToInt64(dr["fdaccountid"]);
                        objFDdetails.pFdaccountno = dr["fdaccountno"];
                        objFDdetails.pMembername = dr["membername"];
                        objFDdetails.pChitbranchname = dr["chitbranchname"];
                        //objFDdetails.pDeposiamount = dr["depositamount"];
                        //objFDdetails.pAccountno = dr["accountid"];
                        lstFDDetails.Add(objFDdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstFDDetails;
        }
        #endregion

        #region  Get Fd Details By ID
        public List<FDDetailsByIdDTO> GetFdDetailsByid(string FdAccountNo, string Connectionstring)
        {
            List<FDDetailsByIdDTO> lstFDDetailsbyid = new List<FDDetailsByIdDTO>();
            try
            {
                //string query = "select fdaccountid,fdaccountno,membercode,membername,depositamount,maturityamount,accountid,tenortype,tenor,interesttype,interestrate,interestpayable,interestpayout,to_char(transdate, 'dd/Mon/yyyy')transdate,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy')maturitydate  from tbltransfdcreation where fdaccountno='" + FdAccountNo + "'";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select fdname,fdaccountid,fdaccountno,membercode,membername,depositamount,maturityamount,paidamount,clearedmount,pendingchequeamount,balanceamount,accountid,tenortype,tenor,interesttype,interestrate,interestpayable,interestpayout,to_char(transdate, 'dd/Mon/yyyy')transdate,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy')maturitydate from vwfdtransaction_details where fdaccountno ='" + FdAccountNo + "';"))
                {
                    while (dr.Read())
                    {
                        FDDetailsByIdDTO objFDdetailsbyid = new FDDetailsByIdDTO();
                        objFDdetailsbyid.pFdaccountid = dr["fdaccountid"];
                        objFDdetailsbyid.pFdaccountno = dr["fdaccountno"];
                        objFDdetailsbyid.pMembername = dr["membername"];
                        objFDdetailsbyid.pDeposiamount = dr["depositamount"];
                        objFDdetailsbyid.pMaturityamount = dr["maturityamount"];
                        objFDdetailsbyid.pAccountno = dr["accountid"];
                        objFDdetailsbyid.pTenortype = dr["tenortype"];
                        objFDdetailsbyid.pTenor = dr["tenor"];
                        objFDdetailsbyid.pInteresttype = dr["interesttype"];
                        objFDdetailsbyid.pInterestrate = dr["interestrate"];
                        objFDdetailsbyid.pInterestPayble = dr["interestpayable"];
                        objFDdetailsbyid.pInterestPayout = dr["interestpayout"];
                        objFDdetailsbyid.pDeposidate = dr["depositdate"];
                        objFDdetailsbyid.pMaturitydate = dr["maturitydate"];
                        objFDdetailsbyid.pTransdate= dr["transdate"];
                        objFDdetailsbyid.pPaidamount = dr["paidamount"];
                        objFDdetailsbyid.pClearedmount = dr["clearedmount"];
                        objFDdetailsbyid.pPendingchequeamount = dr["pendingchequeamount"];
                        objFDdetailsbyid.pBalanceamount = dr["balanceamount"];
                        objFDdetailsbyid.pFDName = dr["fdname"];
                        lstFDDetailsbyid.Add(objFDdetailsbyid);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstFDDetailsbyid;
        }
        #endregion

        #region  Get Transactions list
        public List<TransactionsDTO> GetTransactionslist(int FdAccountNo, string ConnectionString)
        {
            List<TransactionsDTO> lstTransactions = new List<TransactionsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, " select * from(select received_amount,receipt_no,fd_receiptt_date,narration,Mode_of_receipt,(case when tt.clearstatus='Y' then 'cleared' when (tt.clearstatus='R' or  tt.clearstatus='C' or depositstatus='C') then 'Canceled' when tt.clearstatus is null then 'cleared' else 'Not Cleared' end ) as Chequestatus  from fd_receipt fr left join tbltransreceiptreference tt on fr.receipt_no=tt.receiptid where fd_account_id=" + FdAccountNo + " order by fd_receipt_id desc)tbl where Chequestatus<>'Canceled';"))
                    while (dr.Read())
                    {
                        TransactionsDTO objTransactions = new TransactionsDTO();
                        objTransactions.pReceiptamount = dr["received_amount"];
                        objTransactions.pReceiptno = dr["receipt_no"];
                        objTransactions.pNarration = dr["narration"];
                        objTransactions.pModeofReceipt = dr["Mode_of_receipt"];
                        objTransactions.pChequestatus = dr["Chequestatus"];
                        if (dr["fd_receiptt_date"] != DBNull.Value)
                        {
                            objTransactions.pReceiptdate = Convert.ToDateTime(dr["fd_receiptt_date"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objTransactions.pReceiptdate = "";

                        }
                        //objTransactions.pChequestatus = dr["chequestatus"].ToString();
                        lstTransactions.Add(objTransactions);
                    }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTransactions;
        }
        #endregion

        #region  Save FD Receipt
        public bool Savegenaralreceipt(FDReceiptDTO ObjFdReceiptDTO, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = ObjFdReceiptDTO.pConid;
                Objgeneralreceipt.ppartyreferenceid = ObjFdReceiptDTO.pContactid;
                Objgeneralreceipt.ppartyreftype = "Fixed Deposit";
                Objgeneralreceipt.ppartyname = ObjFdReceiptDTO.pMembername;
                Objgeneralreceipt.preceiptdate = ObjFdReceiptDTO.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = ObjFdReceiptDTO.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(ObjFdReceiptDTO.pReceivedamount);
                Objgeneralreceipt.pnarration = ObjFdReceiptDTO.pNarration;
                Objgeneralreceipt.pbankname = ObjFdReceiptDTO.pBank;
                Objgeneralreceipt.pBankId = ObjFdReceiptDTO.pBankid;
                Objgeneralreceipt.pdepositbankid = ObjFdReceiptDTO.pdepositbankid;
                Objgeneralreceipt.pbranchname = ObjFdReceiptDTO.pBranch;
                Objgeneralreceipt.ptranstype = ObjFdReceiptDTO.pTranstype;
                Objgeneralreceipt.ptypeofpayment = ObjFdReceiptDTO.ptypeofpayment;
                Objgeneralreceipt.pChequenumber = ObjFdReceiptDTO.pChequenumber;
                if(ObjFdReceiptDTO.ptypeofpayment== "Debit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjFdReceiptDTO.pReceiptdate.ToString();
                }
               else if (ObjFdReceiptDTO.ptypeofpayment == "Credit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjFdReceiptDTO.pReceiptdate.ToString();
                }
                else
                {
                    Objgeneralreceipt.pchequedate = ObjFdReceiptDTO.pchequedate;
                }
                
                Objgeneralreceipt.pCardNumber = ObjFdReceiptDTO.pCardnumber;
                Objgeneralreceipt.pUpiid = ObjFdReceiptDTO.pUpiid;
                Objgeneralreceipt.pCreatedby = ObjFdReceiptDTO.pCreatedby;
                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();
                objpreceipts.psubledgerid = ObjFdReceiptDTO.pSubledgerid;
                objpreceipts.pledgername = "FIXED DEPOSIT";
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(ObjFdReceiptDTO.pReceivedamount);
                objpreceipts.pamount = Convert.ToDecimal(ObjFdReceiptDTO.pReceivedamount);
                preceiptslist.Add(objpreceipts);
                Objgeneralreceipt.preceiptslist = preceiptslist;
                if (Accontstrans.CallsaveGeneralReceipt(Objgeneralreceipt, trans, out Receiptid))
                {
                    IsSaved = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return IsSaved;
        }
        public bool SaveFdReceipt(FDReceiptDTO ObjFdReceiptDTO, string ConnectionString, out string OUTReceiptid)
        {
            bool Issaved = false;
            StringBuilder sbInsert = new StringBuilder();
            bool IsAccountSaved = false;
            string Referenceno = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (ObjFdReceiptDTO.pModeofreceipt != "ADJUSTMENT")
                {
                    if (Savegenaralreceipt(ObjFdReceiptDTO, trans, out OUTReceiptid))
                    {
                        IsAccountSaved = true;
                        Referenceno = OUTReceiptid;
                    }
                }
                else
                {
                    string adjustrefno = string.Empty;
                    JournalVoucherNewDTO objJournalVoucherDTO = new JournalVoucherNewDTO();
                    List<PaymentsNewDTO> _Paymentslist = new List<PaymentsNewDTO>();
                    objJournalVoucherDTO.pjvdate = ObjFdReceiptDTO.pReceiptdate;
                    objJournalVoucherDTO.pCreatedby = ObjFdReceiptDTO.pCreatedby;
                    objJournalVoucherDTO.pnarration = ObjFdReceiptDTO.pNarration;
                    objJournalVoucherDTO.pmodoftransaction = "MANUAL";
                    PaymentsNewDTO objPaymentsDTO = new PaymentsNewDTO
                    {

                        ppartyid = null,
                        ptranstype = "C",
                        psubledgerid = ObjFdReceiptDTO.pSubledgerid,
                        pamount = Convert.ToDecimal(ObjFdReceiptDTO.pReceivedamount)
                    };
                    _Paymentslist.Add(objPaymentsDTO);
                    objPaymentsDTO = new PaymentsNewDTO
                    {
                        ptranstype = "D",
                        ppartyid = null,
                        ppartyname = "",
                        ppartyreferenceid = "",
                        ppartyreftype = ""
                    };
                    //  long creditaccountid = memberaccountid;
                    objPaymentsDTO.psubledgerid = ObjFdReceiptDTO.pSavingsMemberAccountid;
                    objPaymentsDTO.pamount = Convert.ToDecimal(ObjFdReceiptDTO.pReceivedamount);
                    _Paymentslist.Add(objPaymentsDTO);
                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                    AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
                    if (Accontstrans.SaveJournalVoucherNew(objJournalVoucherDTO, trans, out adjustrefno))
                    {
                        Referenceno = adjustrefno;
                    }
                }
          
                if (string.IsNullOrEmpty(ObjFdReceiptDTO.pRecordid.ToString()) || ObjFdReceiptDTO.pRecordid == 0)
                {
                    sbInsert.Append("INSERT INTO public.fd_receipt(fd_receiptt_date,member_id, fd_account_id,deposit_type, instalment_amount, received_amount, mode_of_receipt,receipt_no,narration,status)VALUES('" + FormatDate(ObjFdReceiptDTO.pReceiptdate) + "'," + ObjFdReceiptDTO.pMemberid + "," + ObjFdReceiptDTO.pFdaccountid + ",'" + ObjFdReceiptDTO.pDeposittype + "'," + ObjFdReceiptDTO.pInstalmentamount + "," + ObjFdReceiptDTO.pReceivedamount + ",'" + ObjFdReceiptDTO.pModeofreceipt + "','" + Referenceno + "','" + ObjFdReceiptDTO.pNarration + "'," + ObjFdReceiptDTO.pStatus + ");");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }
                if(ObjFdReceiptDTO.pModeofreceipt== "CASH")
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "SELECT FN_PROMOTOR_SALARY_JV("+ ObjFdReceiptDTO.pFdaccountid + ",'"+ FormatDate(ObjFdReceiptDTO.pReceiptdate) + "','FD')");
                }
                trans.Commit();
                Issaved = true;
                OUTReceiptid = Referenceno;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return Issaved;
        }
        #endregion

        #region FDReceipt Details
        public List<FDReceiptDetailsDTO> GetFDReceiptDetails(string FromDate,string Todate, string Connectionstring)
        {
            List<FDReceiptDetailsDTO> lstFDReceiptDetailsbyid = new List<FDReceiptDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select tm.membername,tm.membercode,ft.fdaccountno,to_char(fr.fd_receiptt_date, 'dd/Mon/yyyy')fd_receiptt_date,ft.depositamount as dueamount,fr.received_amount,fr.mode_of_receipt,fr.receipt_no,(case when tt.clearstatus = 'R' OR tt.clearstatus = 'C' OR tt.depositstatus = 'C' then 'Cancelled' when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from fd_receipt fr join tblmstmembers tm on fr.member_id = tm.memberid join tbltransfdcreation ft on fr.fd_account_id = ft.fdaccountid and fr.status = true left join tbltransreceiptreference tt ON tt.receiptid = fr.receipt_no where fd_receiptt_date between '" + FormatDate(FromDate)+"' and '"+FormatDate(Todate)+"' order by fd_receipt_id desc; "))
                {
                    while (dr.Read())
                    {
                        FDReceiptDetailsDTO objFDReceiptdetailsbyid = new FDReceiptDetailsDTO();
                        objFDReceiptdetailsbyid.pMembername = dr["membername"];
                        objFDReceiptdetailsbyid.pMembercode = dr["membercode"];
                        objFDReceiptdetailsbyid.pFdaccountno = dr["fdaccountno"];
                        objFDReceiptdetailsbyid.pReceiptdate = Convert.ToString(dr["fd_receiptt_date"]);
                        objFDReceiptdetailsbyid.pDueamount = dr["dueamount"];
                        objFDReceiptdetailsbyid.pReceivedAmount = dr["received_amount"];
                        objFDReceiptdetailsbyid.pModeOfReceipt = dr["mode_of_receipt"];
                        objFDReceiptdetailsbyid.pReceiptno = dr["receipt_no"];
                        objFDReceiptdetailsbyid.pChequestatus = dr["chequestatus"];
                        lstFDReceiptDetailsbyid.Add(objFDReceiptdetailsbyid);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstFDReceiptDetailsbyid;
        }
        #endregion

        #region GetFDBranchDetails
        public async Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct tc.code, tc.branchname, tc.vchregion, tc.vchzone from tabbranchcodes tc join tbltransfdcreation tf on tc.code=tf.chitbranchid::numeric(9,0) where tf.statusid=" + Convert.ToInt32(Status.Active) + " order by tc.branchname;"))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["code"]),
                                pBranchname = dr["branchname"],
                                pVchRegion = dr["vchregion"],
                                pVchZone = dr["vchzone"]
                            };
                            _ChitBranchDetailsList.Add(_ChitBranchDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _ChitBranchDetailsList;
        }

        public async Task<List<ChitBranchDetails>> GetFDReceiptBranchDetails(string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from (select distinct chitbranchid,chitbranchname,count(case when contacttype='Business Entity' then '1' else tn.vchapplicationid end)count from vwfdtransaction_details fd left join tabapplicationpersonalnomineedetails tn on tn.vchapplicationid=fd.fdaccountno where   balanceamount >0  group by chitbranchid,chitbranchname order by chitbranchname)tbl where count>0"))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["chitbranchid"]),
                                pBranchname = dr["chitbranchname"],
                               
                            };
                            _ChitBranchDetailsList.Add(_ChitBranchDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _ChitBranchDetailsList;
        }
        #endregion

       

    }
}
