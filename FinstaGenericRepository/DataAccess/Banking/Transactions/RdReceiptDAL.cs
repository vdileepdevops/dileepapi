using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using Finsta_Banking_Infrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using Finsta_Banking_Repository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;
using System.Threading.Tasks;
using FinstaRepository;

namespace Finsta_Banking_Repository.DataAccess.Banking.Transactions
{
    public class RdReceiptDAL : SettingsDAL, IRdReceipt
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region  Get Member Details
        public List<MemberDetailsDTO> GetMemberDetails(string MemberType, string Connectionstring)
        {
            List<MemberDetailsDTO> lstMemberDetails = new List<MemberDetailsDTO>();
            try
            {

                string query = "select distinct memberid,membercode,membername,tc.contactid,tc.contactreferenceid,membertype,businessentitycontactno as mobileno  from tblmstmembers tm join tblmstcontact tc on tc.contactid=tm.contactid where upper(membertype)='" + ManageQuote(MemberType.ToUpper()) + "' and tm.statusid =" + Convert.ToInt32(Status.Active) + " order by membername";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, query))
                {
                    while (dr.Read())
                    {
                        MemberDetailsDTO objMemberdetails = new MemberDetailsDTO();
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

        #region  Get Account Details
        public AccountDetailsDTO GetAccountDetails(string MemberCode, string Connectionstring)
        {
            AccountDetailsDTO AccountDetailsDTO = new AccountDetailsDTO();
            try
            {
                AccountDetailsDTO.RDAccountDetailsDTOList = new List<RDAccountDetailsDTO>();
                string query = "select rdaccountid,rdaccountno,membername, accountid from tbltransrdcreation where  upper(membercode) ='" + ManageQuote(MemberCode.ToUpper()) + "' and accountstatus='N' order by rdaccountno";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, query))
                {
                    while (dr.Read())
                    {
                        RDAccountDetailsDTO objAccountdetails = new RDAccountDetailsDTO();
                        objAccountdetails.paccountid = Convert.ToInt64(dr["rdaccountid"]);
                        objAccountdetails.paccountno = dr["rdaccountno"];
                        objAccountdetails.pMembername = dr["membername"];
                        objAccountdetails.pSubledgerid = Convert.ToInt64(dr["accountid"]);
                        AccountDetailsDTO.RDAccountDetailsDTOList.Add(objAccountdetails);
                    }

                }
                AccountDetailsDTO.RDSavingsAccountDetailsDTOList = GetAdjustmentDetils(MemberCode, Connectionstring);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return AccountDetailsDTO;
        }
        #endregion

        #region  Get Fd Details By ID
        public List<AccountDetailsByIdDTO> GetAccountDetailsByid(string AccountNo, string Connectionstring)
        {
            List<AccountDetailsByIdDTO> lstAccountDetailsbyid = new List<AccountDetailsByIdDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select rdname,rdaccountid,rdaccountno,membercode,membername,depositamount,maturityamount,instalmentamount,rdinstalmentpayin,tenor,interestrate,interestpayable,interestpayout,to_char(transdate, 'dd/Mon/yyyy')transdate,to_char(deposidate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy')maturitydate from tbltransrdcreation where rdaccountno ='" + AccountNo + "' order by rdaccountno;"))
                {
                    while (dr.Read())
                    {
                        AccountDetailsByIdDTO objdetailsbyid = new AccountDetailsByIdDTO();
                        objdetailsbyid.paccountid = dr["rdaccountid"];
                        objdetailsbyid.paccountno = dr["rdaccountno"];
                        objdetailsbyid.pMembername = dr["membername"];
                        objdetailsbyid.pDeposiamount = dr["depositamount"];
                        objdetailsbyid.pMaturityamount = dr["maturityamount"];
                        objdetailsbyid.pInstalmentamount = dr["instalmentamount"];
                        objdetailsbyid.pInstalmentPayin = dr["rdinstalmentpayin"];
                        objdetailsbyid.pInstalmentTenor = dr["tenor"];
                        objdetailsbyid.pDeposidate = dr["depositdate"];
                        objdetailsbyid.pMaturitydate = dr["maturitydate"];
                        objdetailsbyid.pInterestPayble = dr["interestpayable"];
                        objdetailsbyid.pInterestrate = dr["interestrate"];
                        objdetailsbyid.pInterestPayout = dr["interestpayout"];
                        objdetailsbyid.pschemename = dr["rdname"];
                        objdetailsbyid.pInterestrate = dr["interestrate"];
                        objdetailsbyid.pTransdate = dr["transdate"];



                        lstAccountDetailsbyid.Add(objdetailsbyid);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAccountDetailsbyid;
        }
        #endregion

        #region Get View Dues
        public List<ViewDuesDTO> GetViewDues(string AccountNo, string transdate, string Connectionstring)
        {
            List<ViewDuesDTO> lstDues = new List<ViewDuesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select instalmentno,instalmentdate,instalmentamount,0 as penalty from tabrdinstalments where rdaccountno='" + AccountNo + "' order by instalmentno;"))
                {
                    while (dr.Read())
                    {
                        ViewDuesDTO objDues = new ViewDuesDTO();
                        objDues.pInstalmentno = dr["instalmentno"];
                        objDues.pInstalmentdate = dr["instalmentdate"];
                        objDues.pinstalmentamount = dr["instalmentamount"];
                        objDues.pPenalty = dr["penalty"];
                        lstDues.Add(objDues);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstDues;
        }
        #endregion

        #region  Save Rd Receipt
        public bool Savegenaralreceipt(RdReceiptDTO ObjRdReceiptDTO, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = ObjRdReceiptDTO.pConid;
                Objgeneralreceipt.ppartyreferenceid = ObjRdReceiptDTO.pContactid;
                Objgeneralreceipt.ppartyreftype = "Recurring Deposit";
                Objgeneralreceipt.ppartyname = ObjRdReceiptDTO.pMembername;
                Objgeneralreceipt.preceiptdate = ObjRdReceiptDTO.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = ObjRdReceiptDTO.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(ObjRdReceiptDTO.pReceivedamount);
                Objgeneralreceipt.pnarration = ObjRdReceiptDTO.pNarration;
                Objgeneralreceipt.pbankname = ObjRdReceiptDTO.pBank;
                Objgeneralreceipt.pBankId = ObjRdReceiptDTO.pBankid;
                Objgeneralreceipt.pdepositbankid = ObjRdReceiptDTO.pdepositbankid;
                Objgeneralreceipt.pbranchname = ObjRdReceiptDTO.pBranch;
                Objgeneralreceipt.ptranstype = ObjRdReceiptDTO.pTranstype;
                Objgeneralreceipt.ptypeofpayment = ObjRdReceiptDTO.ptypeofpayment;
                Objgeneralreceipt.pChequenumber = ObjRdReceiptDTO.pChequenumber;
                if (ObjRdReceiptDTO.ptypeofpayment == "Debit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjRdReceiptDTO.pReceiptdate.ToString();
                }
                else if (ObjRdReceiptDTO.ptypeofpayment == "Credit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjRdReceiptDTO.pReceiptdate.ToString();
                }
                else
                {
                    Objgeneralreceipt.pchequedate = ObjRdReceiptDTO.pchequedate;
                }

                Objgeneralreceipt.pCardNumber = ObjRdReceiptDTO.pCardnumber;
                Objgeneralreceipt.pUpiid = ObjRdReceiptDTO.pUpiid;
                Objgeneralreceipt.pCreatedby = ObjRdReceiptDTO.pCreatedby;
                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();
                objpreceipts.psubledgerid = ObjRdReceiptDTO.pSubledgerid;
                objpreceipts.pledgername = "Recurring Deposit";
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(ObjRdReceiptDTO.pReceivedamount);
                objpreceipts.pamount = Convert.ToDecimal(ObjRdReceiptDTO.pReceivedamount);
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


        public bool SaveRdReceipt(RdReceiptDTO ObjRdReceiptDTO, string ConnectionString, out string OUTReceiptid)
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
                if (ObjRdReceiptDTO.pModeofreceipt != "ADJUSTMENT")
                {
                    if (Savegenaralreceipt(ObjRdReceiptDTO, trans, out OUTReceiptid))
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
                    objJournalVoucherDTO.pjvdate = ObjRdReceiptDTO.pReceiptdate;
                    objJournalVoucherDTO.pCreatedby = ObjRdReceiptDTO.pCreatedby;
                    objJournalVoucherDTO.pnarration = ObjRdReceiptDTO.pNarration;
                    objJournalVoucherDTO.pmodoftransaction = "MANUAL";
                    PaymentsNewDTO objPaymentsDTO = new PaymentsNewDTO
                    {

                        ppartyid = null,
                        ptranstype = "C",
                        psubledgerid = ObjRdReceiptDTO.pSubledgerid,
                        pamount = Convert.ToDecimal(ObjRdReceiptDTO.pReceivedamount)
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
                    objPaymentsDTO.psubledgerid = ObjRdReceiptDTO.pSavingsMemberAccountid;
                    objPaymentsDTO.pamount = Convert.ToDecimal(ObjRdReceiptDTO.pReceivedamount);
                    _Paymentslist.Add(objPaymentsDTO);
                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                    AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
                    if (Accontstrans.SaveJournalVoucherNew(objJournalVoucherDTO, trans, out adjustrefno))
                    {
                        Referenceno = adjustrefno;
                    }

                }
                if (string.IsNullOrEmpty(ObjRdReceiptDTO.pRecordid.ToString()) || ObjRdReceiptDTO.pRecordid == 0)
                {
                    sbInsert.Append("INSERT INTO Rd_receipt(rd_receipt_date,member_id,rd_account_id,deposit_type, instalment_amount,penalty_amount, received_amount, mode_of_receipt,receipt_no)VALUES('" + FormatDate(ObjRdReceiptDTO.pReceiptdate) + "'," + ObjRdReceiptDTO.pMemberid + "," + ObjRdReceiptDTO.pAccountid + ",'" + ObjRdReceiptDTO.pDeposittype + "'," + ObjRdReceiptDTO.pInstalmentamount + ", " + ObjRdReceiptDTO.pPenaltyamount + "," + ObjRdReceiptDTO.pReceivedamount + ",'" + ObjRdReceiptDTO.pModeofreceipt + "','" + Referenceno + "');");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }
                if (ObjRdReceiptDTO.pModeofreceipt == "CASH" || ObjRdReceiptDTO.pModeofreceipt == "ADJUSTMENT")
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "select fn_rd_instalments_update('" + ObjRdReceiptDTO.pAccountno + "','" + Referenceno + "')");
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "SELECT FN_PROMOTOR_SALARY_JV(" + ObjRdReceiptDTO.pAccountid + ",'" + FormatDate(ObjRdReceiptDTO.pReceiptdate) + "','RD')");
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

        //#region  Save Rd Receipt Adjustment
        //public bool SaveRDJournalVoucher(RdJournalVoucherDTO ObjRdVouchertDTO, NpgsqlTransaction trans, out string Voucher)
        //{
        //    bool IsSaved = false;
        //    StringBuilder sbQuery = new StringBuilder();
        //    AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
        //    try
        //    {
        //        JournalVoucherNewDTO objvoucher = new JournalVoucherNewDTO();
        //        objvoucher.pjvnumber = "";
        //        objvoucher.pjvdate = ObjRdVouchertDTO.pjvdate;
        //        objvoucher.ptotalpaidamount = ObjRdVouchertDTO.ptotalpaidamount;
        //        objvoucher.ptypeofoperation = ObjRdVouchertDTO.pmodoftransaction;
        //        if (Accontstrans.SaveJournalVoucherNew(objvoucher, trans, out Voucher))
        //        {
        //            IsSaved = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    return IsSaved;
        //}
        //public bool SaveRdAdjustment(RdJournalVoucherDTO ObjRdVouchertDTO, string ConnectionString, out string OUTVoucherid)
        //{
        //    bool Issaved = false;
        //    StringBuilder sbInsert = new StringBuilder();
        //    bool IsAccountSaved = false;
        //    try
        //    {
        //        con = new NpgsqlConnection(ConnectionString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();
        //        if (SaveRDJournalVoucher(ObjRdVouchertDTO, trans, out OUTVoucherid))
        //        {
        //            IsAccountSaved = true;
        //        }
        //        else
        //        {
        //            trans.Rollback();
        //            return IsAccountSaved;
        //        }
        //        string Receiptno = OUTVoucherid;
        //        if (string.IsNullOrEmpty(ObjRdVouchertDTO.pRecordid.ToString()) || ObjRdVouchertDTO.pRecordid == 0)
        //        {
        //            sbInsert.Append("INSERT INTO Rd_receipt(rd_receipt_date,member_id,rd_account_id,deposit_type, instalment_amount,penalty_amount, received_amount, mode_of_receipt,receipt_no)VALUES('" + FormatDate(ObjRdVouchertDTO.pReceiptdate) + "'," + ObjRdVouchertDTO.pMemberid + "," + ObjRdVouchertDTO.pAccountid + ",'" + ObjRdVouchertDTO.pDeposittype + "'," + ObjRdVouchertDTO.pInstalmentamount + ", " + ObjRdVouchertDTO.pPenaltyamount + "," + ObjRdVouchertDTO.pReceivedamount + ",'" + ObjRdVouchertDTO.pModeofreceipt + "','" + Receiptno + "');");
        //        }
        //        if (!string.IsNullOrEmpty(sbInsert.ToString()))
        //        {
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
        //        }
        //        if (ObjRdVouchertDTO.pModeofreceipt == "CASH")
        //        {
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "fn_rd_instalments_update(" + ObjRdVouchertDTO.pAccountno + ",'" + Receiptno + "')");
        //        }
        //        trans.Commit();
        //        Issaved = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        trans.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Dispose();
        //            con.Close();
        //            con.ClearPool();
        //            trans.Dispose();
        //        }
        //    }
        //    return Issaved;
        //}
        //#endregion 



        #region RDReceipt Details View

        public List<RdReceiptDetailsDTO> GetRdReceiptDetails(string FromDate, string Todate, string Connectionstring)
        {
            List<RdReceiptDetailsDTO> lstRdReceiptDetails = new List<RdReceiptDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select tm.membername,tm.membercode,ft.rdaccountno,to_char(fr.rd_receipt_date, 'dd/Mon/yyyy')rd_receipt_date,fr.received_amount,fr.mode_of_receipt,fr.receipt_no,(case when tt.clearstatus = 'R' OR tt.clearstatus = 'C' OR tt.depositstatus = 'C' then 'Cancelled' when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from rd_receipt fr join tblmstmembers tm on fr.member_id = tm.memberid join tbltransrdcreation ft on fr.rd_account_id = ft.rdaccountid left join tbltransreceiptreference tt ON tt.receiptid = fr.receipt_no where rd_receipt_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(Todate) + "' order by rd_receipt_id desc; "))
                {
                    while (dr.Read())
                    {
                        RdReceiptDetailsDTO objRDReceiptdetails = new RdReceiptDetailsDTO();
                        objRDReceiptdetails.pMembername = dr["membername"];
                        objRDReceiptdetails.pMembercode = dr["membercode"];
                        objRDReceiptdetails.pRdaccountno = dr["rdaccountno"];
                        objRDReceiptdetails.pReceiptdate = Convert.ToString(dr["rd_receipt_date"]);
                        objRDReceiptdetails.pReceivedAmount = dr["received_amount"];
                        objRDReceiptdetails.pModeOfReceipt = dr["mode_of_receipt"];
                        objRDReceiptdetails.pReceiptno = dr["receipt_no"];
                        objRDReceiptdetails.pChequestatus = dr["chequestatus"];
                        lstRdReceiptDetails.Add(objRDReceiptdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstRdReceiptDetails;
        }
        #endregion

        #region  Get Receipts List By Accountno
        public List<TransactionsDTO> GetTransactionslist(int AccountNo, string ConnectionString)
        {
            List<TransactionsDTO> lstTransactions = new List<TransactionsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from(select received_amount,receipt_no,rd_receipt_date,narration,Mode_of_receipt,(case when tt.clearstatus='Y' then 'cleared' when (tt.clearstatus='R' or  tt.clearstatus='C' or depositstatus='C') then 'Canceled' when tt.clearstatus is null then 'cleared' else 'Not Cleared' end ) as Chequestatus  from rd_receipt fr left join tbltransreceiptreference tt on fr.receipt_no=tt.receiptid where rd_account_id=" + AccountNo + " order by rd_receipt_id desc)tbl where Chequestatus<>'Canceled' order by rd_receipt_date;"))
                    while (dr.Read())
                    {
                        TransactionsDTO objTransactions = new TransactionsDTO();
                        objTransactions.pReceiptamount = dr["received_amount"];
                        objTransactions.pReceiptno = dr["receipt_no"];
                        objTransactions.pNarration = dr["narration"];
                        objTransactions.pModeofReceipt = dr["Mode_of_receipt"];
                        objTransactions.pChequestatus = dr["Chequestatus"];
                        if (dr["rd_receipt_date"] != DBNull.Value)
                        {
                            objTransactions.pReceiptdate = Convert.ToDateTime(dr["rd_receipt_date"]).ToString("dd/MM/yyyy");
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

        #region  Get Adjustment Details
        public List<RDSavingsAccountDetailsDTO> GetAdjustmentDetils(String Membercode, string ConnectionString)
        {
            List<RDSavingsAccountDetailsDTO> lstAdjustdetails = new List<RDSavingsAccountDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingaccountno,membername,membercode,accountid,(select coalesce(-1*accountbalance,0) from tblmstaccounts  where accountname  =savingaccountno||'-'||upper(membername)) as balance from tbltranssavingaccountcreation  where membercode='" + Membercode + "' order by savingaccountno;"))
                    while (dr.Read())
                    {
                        RDSavingsAccountDetailsDTO objAdjustdetails = new RDSavingsAccountDetailsDTO();
                        objAdjustdetails.pSavingsMemberAccountno = dr["savingaccountno"];
                        objAdjustdetails.pSavingsMemberAccountName = dr["membername"];
                        objAdjustdetails.pSavingsMemberAccountid = dr["accountid"];
                        objAdjustdetails.pSavingsMemberBalance = dr["balance"];
                        lstAdjustdetails.Add(objAdjustdetails);
                    }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstAdjustdetails;
        }
        #endregion



        //#region RD Receipt View
        //public List<RDReceiptDetailsDTO> GetRDReceiptDetails(string FromDate, string Todate, string Connectionstring)
        //{
        //    List<RDReceiptDetailsDTO> lstRDReceiptDetailsbyid = new List<RDReceiptDetailsDTO>();
        //    try
        //    {
        //        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select tm.membername,tm.membercode,rt.rdaccountno,to_char(rd_receipt_date, 'dd/Mon/yyyy')rd_receipt_date,rt.instalmentamount as instalmentamount,rr.received_amount,rr.mode_of_receipt,rr.receipt_no,(case when tt.clearstatus = 'R' OR tt.clearstatus = 'C' OR tt.depositstatus = 'C' then 'Cancelled' when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from rd_receipt rr join tblmstmembers tm on rr.member_id = tm.memberid join tbltransrdcreation rt on rr.rd_account_id = rt.rdaccountid left join tbltransreceiptreference tt ON tt.receiptid = rr.receipt_no where rd_receipt_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(Todate) + "' order by rd_receipt_id desc;"))
        //        {
        //            while (dr.Read())
        //            {
        //                RDReceiptDetailsDTO objRDReceiptdetailsbyid = new RDReceiptDetailsDTO();
        //                objRDReceiptdetailsbyid.pMembername = dr["membername"];
        //                objRDReceiptdetailsbyid.pMembercode = dr["membercode"];
        //                objRDReceiptdetailsbyid.pRdaccountno = dr["rdaccountno"];
        //                objRDReceiptdetailsbyid.pReceiptdate = Convert.ToString(dr["rdaccountno"]);
        //                objRDReceiptdetailsbyid.pInstalmentamount = dr["instalmentamount"];
        //                objRDReceiptdetailsbyid.pReceivedAmount = dr["received_amount"];
        //                objRDReceiptdetailsbyid.pModeOfReceipt = dr["mode_of_receipt"];
        //                objRDReceiptdetailsbyid.pReceiptno = dr["receipt_no"];
        //                objRDReceiptdetailsbyid.pChequestatus = dr["ChequeStatus"];
        //                lstRDReceiptDetailsbyid.Add(objRDReceiptdetailsbyid);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    return lstRDReceiptDetailsbyid;
        //}
        //#endregion

    }
}
