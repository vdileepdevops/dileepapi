using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using System.Data;
using FinstaRepository.DataAccess.Banking.Transactions;
using Finsta_Banking_Infrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;
using FinstaRepository;
using Finsta_Banking_Repository.Interfaces.Banking.Transactions;

namespace Finsta_Banking_Repository.DataAccess.Banking.Transactions
{
    public class S_DReciptDAL : SettingsDAL, IS_DReceipt
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region Save Member Receipt
        public bool Savegenaralreceipt(MemberReceiptDTO ObjMemberReceiptDTO, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = ObjMemberReceiptDTO.pConid;
                Objgeneralreceipt.ppartyreferenceid = ObjMemberReceiptDTO.pContactid;
                Objgeneralreceipt.ppartyreftype = "Member Receipt";
                Objgeneralreceipt.ppartyname = ObjMemberReceiptDTO.pMembername;
                Objgeneralreceipt.preceiptdate = ObjMemberReceiptDTO.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = ObjMemberReceiptDTO.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(ObjMemberReceiptDTO.pReceivedamount);
                Objgeneralreceipt.pnarration = ObjMemberReceiptDTO.pNarration;
                Objgeneralreceipt.pbankname = ObjMemberReceiptDTO.pBank;
                Objgeneralreceipt.pBankId = ObjMemberReceiptDTO.pBankid;
                Objgeneralreceipt.pdepositbankid = ObjMemberReceiptDTO.pdepositbankid;
                Objgeneralreceipt.pbranchname = ObjMemberReceiptDTO.pBranch;
                Objgeneralreceipt.ptranstype = ObjMemberReceiptDTO.pTranstype;
                Objgeneralreceipt.ptypeofpayment = ObjMemberReceiptDTO.ptypeofpayment;
                Objgeneralreceipt.pChequenumber = ObjMemberReceiptDTO.pChequenumber;
                if (ObjMemberReceiptDTO.ptypeofpayment == "Debit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjMemberReceiptDTO.pReceiptdate.ToString();
                }
                else if (ObjMemberReceiptDTO.ptypeofpayment == "Credit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjMemberReceiptDTO.pReceiptdate.ToString();
                }
                else
                {
                    Objgeneralreceipt.pchequedate = ObjMemberReceiptDTO.pchequedate;
                }

                Objgeneralreceipt.pCardNumber = ObjMemberReceiptDTO.pCardnumber;
                Objgeneralreceipt.pUpiid = ObjMemberReceiptDTO.pUpiid;
                Objgeneralreceipt.pCreatedby = ObjMemberReceiptDTO.pCreatedby;
                string membername = ObjMemberReceiptDTO.pMembername;
                string membercode = ObjMemberReceiptDTO.pMembercode;
                string membernamecode = membercode + "_" + membername;
                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();

                Int64 membershipaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('MEMBERSHIP FEE') AND chracctype='2' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                int subledgerid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + membernamecode + "', " + membershipaccountid + ", '3'," + ObjMemberReceiptDTO.pCreatedby + ")"));


                objpreceipts.psubledgerid = subledgerid;
                objpreceipts.pledgername = membernamecode;
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(ObjMemberReceiptDTO.pReceivedamount);
                objpreceipts.pamount = Convert.ToDecimal(ObjMemberReceiptDTO.pReceivedamount);
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

        public bool SaveMemberReceipt(MemberReceiptDTO ObjMemberReceiptDTO, string ConnectionString, out string OUTReceiptid)
        {
            bool Issaved = false;
            StringBuilder sbInsert = new StringBuilder();
            bool IsAccountSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (Savegenaralreceipt(ObjMemberReceiptDTO, trans, out OUTReceiptid))
                {
                    IsAccountSaved = true;
                }
                else
                {
                    trans.Rollback();
                    return IsAccountSaved;
                }
                string Receiptno = OUTReceiptid;
                ObjMemberReceiptDTO.pReceiptno = Receiptno;
                if (string.IsNullOrEmpty(ObjMemberReceiptDTO.pRecordid.ToString()) || ObjMemberReceiptDTO.pRecordid == 0)
                {
                    sbInsert.Append("insert into Member_receipt (member_id, receipt_date, received_amount, mode_of_receipt, receipt_no, narration, status) values (" + ObjMemberReceiptDTO.pMemberid + ", '" + FormatDate(ObjMemberReceiptDTO.pReceiptdate) + "', " + ObjMemberReceiptDTO.pReceivedamount + ", '" + ObjMemberReceiptDTO.pModeofreceipt + "', '" + ObjMemberReceiptDTO.pReceiptno + "', '" + ObjMemberReceiptDTO.pNarration + "', true);");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }
                trans.Commit();
                Issaved = true;
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


        #region Member Receipt View
        public List<MemberreceiptViewDTO> GetMemberReceiptView(string FromDate, string Todate, string Connectionstring)
        {
            List<MemberreceiptViewDTO> lstmemberReceiptView = new List<MemberreceiptViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select memberid, membername, membercode, membertypeid, membertype, contactid, contacttype, contactreferenceid,to_char(receipt_date, 'dd/Mon/yyyy') receipt_date, received_amount, mode_of_receipt, receipt_no, narration, (case when tt.clearstatus = 'R' OR tt.clearstatus = 'C' OR tt.depositstatus = 'C' then 'Cancelled' when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from Member_receipt MR join tblmstmembers TM on TM.memberid = MR.member_id left join tbltransreceiptreference TT on TT.receiptid = MR.receipt_no where MR.status = true and receipt_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(Todate) + "' order by memberid desc;"))
                {
                    while (dr.Read())
                    {
                        MemberreceiptViewDTO objMemberReceipt = new MemberreceiptViewDTO();
                        objMemberReceipt.pmemberid = Convert.ToInt64(dr["memberid"]);
                        objMemberReceipt.pmembername = Convert.ToString(dr["membername"]);
                        objMemberReceipt.pmembercode = Convert.ToString(dr["membercode"]);
                        objMemberReceipt.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                        objMemberReceipt.pmembertype = Convert.ToString(dr["membertype"]);
                        objMemberReceipt.pcontactid = Convert.ToInt64(dr["contactid"]);
                        objMemberReceipt.pcontacttype = Convert.ToString(dr["contacttype"]);
                        objMemberReceipt.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        objMemberReceipt.preceiptdate = dr["receipt_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        objMemberReceipt.preceivedamount = Convert.ToDecimal(dr["received_amount"]);
                        objMemberReceipt.pmodeofreceipt = Convert.ToString(dr["mode_of_receipt"]);
                        objMemberReceipt.preceiptno = Convert.ToString(dr["receipt_no"]);
                        objMemberReceipt.pnarration = Convert.ToString(dr["narration"]);
                        objMemberReceipt.pChequeStatus = Convert.ToString(dr["ChequeStatus"]);
                        lstmemberReceiptView.Add(objMemberReceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstmemberReceiptView;
        }
        #endregion
        #region Get Member Details
        public List<MembersandContactDetails> GetMembers(string Contacttype, string MemberType, string ConnectionString)
        {
            List<MembersandContactDetails> _MembersList = new List<MembersandContactDetails>();

            try
            {
                //"select memberid,membercode,membername,te.contactid, te.contactreferenceid,contactnumber from tblmstmembers te join tblmstcontactpersondetails tc  on tc.contactid = te.contactid where  contacttype ='" + Contacttype + "' and membertype = '" + MemberType + "' and upper(priority) = 'PRIMARY' and  te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername; "

                // and coalesce(upper(documentname), 'PAN CARD') like '%PAN%'
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,te.contactid, te.contacttype,te.contactreferenceid,businessentitycontactno as contactnumber, case when z.address2 <> '' then z.address1||','||z.address2||','||z.city||','||z.district||','||z.state||', '||z.pincode else z.address1||','||z.city||','||z.district||','||z.state||','||z.pincode end as contactaddress from tblmstmembers te join tblmstcontact tc  on tc.contactid = te.contactid left join tblmstcontactaddressdetails z on z.contactid = te.contactid where memberid not in (select distinct member_id from Member_receipt where status = true) and te.contacttype ='" + Contacttype + "' and membertype = '" + MemberType + "' and  coalesce(te.statusid, " + Convert.ToInt32(Status.Active) + ") = " + Convert.ToInt32(Status.Active) + " and coalesce(z.statusid, " + Convert.ToInt32(Status.Active) + ") = " + Convert.ToInt32(Status.Active) + " order by membername;"))
                {
                    while (dr.Read())
                    {
                        MembersandContactDetails _MembersandContactDetails = new MembersandContactDetails();

                        _MembersandContactDetails.pMemberCode = dr["membercode"];
                        _MembersandContactDetails.pMemberId = Convert.ToInt64(dr["memberid"]);
                        _MembersandContactDetails.pMemberName = dr["membername"];
                        _MembersandContactDetails.pContactid = Convert.ToInt64(dr["contactid"]);
                        _MembersandContactDetails.pContacttype = dr["contacttype"];
                        _MembersandContactDetails.pContactrefid = dr["contactreferenceid"];
                        _MembersandContactDetails.pContactnumber = dr["contactnumber"];
                        _MembersandContactDetails.pcontactaddress = dr["contactaddress"];
                        Int64 contactid = _MembersandContactDetails.pContactid;
                        //_MembersandContactDetails.pdocumentname = dr["documentname"];
                        //_MembersandContactDetails.pdocreferenceno = dr["docreferenceno"];
                        _MembersandContactDetails.MemberDocumentsDetailsDTO = new List<DocumentsDetailslist>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select documentname, docreferenceno from tblmstdocumentstore where statusid = 1 and contactid = " + contactid + " order by documentname desc;"))
                        {
                            while (dr1.Read())
                            {
                                DocumentsDetailslist DocumentsDetails = new DocumentsDetailslist();
                                DocumentsDetails.pdocreferenceno = dr1["docreferenceno"];
                                DocumentsDetails.pdocumentname = dr1["documentname"];
                                _MembersandContactDetails.MemberDocumentsDetailsDTO.Add(DocumentsDetails);
                            }
                        }
                        _MembersList.Add(_MembersandContactDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return _MembersList;
        }
        #endregion

        #region Saving Account Receipt
        public List<SavingAccNameDetails> GetSavingAccountNameDetails(string ConnectionString)
        {
            List<SavingAccNameDetails> _SavingAccNameDetails = new List<SavingAccNameDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingconfigid, savingaccname, savingaccnamecode from tblmstsavingaccountconfig where savingconfigid in (select distinct savingconfigid from tbltranssavingaccountcreation where statusid = " + Convert.ToInt32(Status.Active) + ") order by savingaccname;"))
                {
                    while (dr.Read())
                    {
                        SavingAccNameDetails _objSavingAccNameDetails = new SavingAccNameDetails();

                        _objSavingAccNameDetails.pSavingConfigid = Convert.ToInt64(dr["savingconfigid"]);
                        _objSavingAccNameDetails.pSavingAccname = Convert.ToString(dr["savingaccname"]);
                        _objSavingAccNameDetails.pSavingAccNameCode = Convert.ToString(dr["savingaccnamecode"]);
                        _SavingAccNameDetails.Add(_objSavingAccNameDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _SavingAccNameDetails;
        }
        public List<SavingAccDetails> GetSavingAccountNumberDetails(Int64 SavingConfigid, string ConnectionString)
        {
            List<SavingAccDetails> _SavingAccDetails = new List<SavingAccDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct savingaccountid, savingaccountno, ts.transdate, ts.membertypeid, ts.membertype, memberid, ts.applicanttype, membercode, membername, ts.contactid, ts.contacttype, ts.contactreferenceid, ts.savingconfigid, ts.savingaccname, coalesce(savingsamount,0) savingsamount, savingsamountpayin, interestcompound, coalesce(minsavingamount,0) minsavingamount, coalesce(minopenamount,0) minopenamount, coalesce(minmaintainbalance,0) minmaintainbalance, ta.interestpayout, coalesce(ta.interestrate,0) interestrate, coalesce(maxwithdrawallimit,0) maxwithdrawallimit,coalesce(penaltyvalue,0) penaltyvalue, savingspayinmode, coalesce(savingmindepositamount,0) savingmindepositamount, coalesce(savingmaxdepositamount,0) savingmaxdepositamount, businessentitycontactno, accountid from tbltranssavingaccountcreation ts join tblmstsavingaccountconfigdetails ta on ts.savingconfigid = ta.savingconfigid and ts.applicanttype = ta.applicanttype join tblmstcontact tc on tc.contactid = ts.contactid where ts.savingconfigid = " + SavingConfigid + " and ts.statusid = " + Convert.ToInt32(Status.Active) + " order by savingaccountno;"))
                {
                    while (dr.Read())
                    {
                        SavingAccDetails _objSavingAccDetails = new SavingAccDetails();

                        _objSavingAccDetails.psavingaccountid = Convert.ToInt64(dr["savingaccountid"]);
                        _objSavingAccDetails.psavingaccountno = Convert.ToString(dr["savingaccountno"]);
                        _objSavingAccDetails.ptransdate = dr["transdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy");
                        _objSavingAccDetails.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                        _objSavingAccDetails.pmembertype = Convert.ToString(dr["membertype"]);
                        _objSavingAccDetails.pmemberid = Convert.ToInt64(dr["memberid"]);
                        _objSavingAccDetails.papplicanttype = Convert.ToString(dr["applicanttype"]);
                        _objSavingAccDetails.pmembercode = Convert.ToString(dr["membercode"]);
                        _objSavingAccDetails.pmembername = Convert.ToString(dr["membername"]);
                        _objSavingAccDetails.pcontactid = Convert.ToInt64(dr["contactid"]);
                        _objSavingAccDetails.pcontacttype = Convert.ToString(dr["contacttype"]);
                        _objSavingAccDetails.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        _objSavingAccDetails.pcontactno = Convert.ToString(dr["businessentitycontactno"]);
                        _objSavingAccDetails.psavingconfigid = Convert.ToInt64(dr["savingconfigid"]);
                        _objSavingAccDetails.psavingaccname = Convert.ToString(dr["savingaccname"]);
                        _objSavingAccDetails.psavingsamount = Convert.ToDecimal(dr["savingsamount"]);
                        _objSavingAccDetails.psavingsamountpayin = Convert.ToString(dr["savingsamountpayin"]);
                        _objSavingAccDetails.pinterestcompound = Convert.ToString(dr["interestcompound"]);
                        _objSavingAccDetails.pminsavingamount = Convert.ToDecimal(dr["minsavingamount"]);
                        _objSavingAccDetails.pminopenamount = Convert.ToDecimal(dr["minopenamount"]);
                        _objSavingAccDetails.pminmaintainbalance = Convert.ToDecimal(dr["minmaintainbalance"]);
                        _objSavingAccDetails.pinterestpayout = Convert.ToString(dr["interestpayout"]);
                        _objSavingAccDetails.pinterestrate = Convert.ToDecimal(dr["interestrate"]);
                        _objSavingAccDetails.pmaxwithdrawallimit = Convert.ToDecimal(dr["maxwithdrawallimit"]);
                        _objSavingAccDetails.ppenaltyvalue = Convert.ToDecimal(dr["penaltyvalue"]);
                        _objSavingAccDetails.psavingspayinmode = Convert.ToString(dr["savingspayinmode"]);
                        _objSavingAccDetails.psavingmindepositamount = Convert.ToDecimal(dr["savingmindepositamount"]);
                        _objSavingAccDetails.psavingmaxdepositamount = Convert.ToDecimal(dr["savingmaxdepositamount"]);
                        _objSavingAccDetails.pAccountid = Convert.ToInt64(dr["accountid"]);
                        _SavingAccDetails.Add(_objSavingAccDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _SavingAccDetails;
        }

        public bool SaveSAgenaralreceipt(SAReceiptDTO ObjSAReceiptDTO, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = ObjSAReceiptDTO.pConid;
                Objgeneralreceipt.ppartyreferenceid = ObjSAReceiptDTO.pContactid;
                Objgeneralreceipt.ppartyreftype = "Saving Account Receipt";
                Objgeneralreceipt.ppartyname = ObjSAReceiptDTO.pMembername;
                Objgeneralreceipt.preceiptdate = ObjSAReceiptDTO.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = ObjSAReceiptDTO.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(ObjSAReceiptDTO.pReceivedamount);
                Objgeneralreceipt.pnarration = ObjSAReceiptDTO.pNarration;
                Objgeneralreceipt.pbankname = ObjSAReceiptDTO.pBank;
                Objgeneralreceipt.pBankId = ObjSAReceiptDTO.pBankid;
                Objgeneralreceipt.pdepositbankid = ObjSAReceiptDTO.pdepositbankid;
                Objgeneralreceipt.pbranchname = ObjSAReceiptDTO.pBranch;
                Objgeneralreceipt.ptranstype = ObjSAReceiptDTO.pTranstype;
                Objgeneralreceipt.ptypeofpayment = ObjSAReceiptDTO.ptypeofpayment;
                Objgeneralreceipt.pChequenumber = ObjSAReceiptDTO.pChequenumber;
                if (ObjSAReceiptDTO.ptypeofpayment == "Debit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjSAReceiptDTO.pReceiptdate.ToString();
                }
                else if (ObjSAReceiptDTO.ptypeofpayment == "Credit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjSAReceiptDTO.pReceiptdate.ToString();
                }
                else
                {
                    Objgeneralreceipt.pchequedate = ObjSAReceiptDTO.pchequedate;
                }

                Objgeneralreceipt.pCardNumber = ObjSAReceiptDTO.pCardnumber;
                Objgeneralreceipt.pUpiid = ObjSAReceiptDTO.pUpiid;
                Objgeneralreceipt.pCreatedby = ObjSAReceiptDTO.pCreatedby;
                string membername = ObjSAReceiptDTO.pMembername;
                string membercode = ObjSAReceiptDTO.pMembercode;
                string membernamecode = membercode + "_" + membername;
                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();

                objpreceipts.psubledgerid = ObjSAReceiptDTO.pSubledgerid;
                objpreceipts.pledgername = membername;
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(ObjSAReceiptDTO.pReceivedamount);
                objpreceipts.pamount = Convert.ToDecimal(ObjSAReceiptDTO.pReceivedamount);
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
        public bool SaveSavingsReceipt(SAReceiptDTO ObjSAReceiptDTO, string ConnectionString, out string OUTReceiptid)
        {
            bool Issaved = false;
            StringBuilder sbInsert = new StringBuilder();
            bool IsAccountSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (SaveSAgenaralreceipt(ObjSAReceiptDTO, trans, out OUTReceiptid))
                {
                    IsAccountSaved = true;
                }
                else
                {
                    trans.Rollback();
                    return IsAccountSaved;
                }
                string Receiptno = OUTReceiptid;
                ObjSAReceiptDTO.pReceiptno = Receiptno;
                if (string.IsNullOrEmpty(ObjSAReceiptDTO.pRecordid.ToString()) || ObjSAReceiptDTO.pRecordid == 0)
                {
                    sbInsert.Append("insert into Saving_Account_receipt (Trans_date, member_id, saving_account_id, received_amount, penalty_amount, mode_of_receipt, receipt_no, narration, status) values ('" + FormatDate(ObjSAReceiptDTO.pReceiptdate) + "', " + ObjSAReceiptDTO.pMemberid + ", " + ObjSAReceiptDTO.psavingaccountid + ", " + ObjSAReceiptDTO.pReceivedamount + ", " + ObjSAReceiptDTO.ppenaltyamount + ", '" + ObjSAReceiptDTO.pModeofreceipt + "', '" + ObjSAReceiptDTO.pReceiptno + "', '" + ObjSAReceiptDTO.pNarration + "', true);");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }
                trans.Commit();
                Issaved = true;
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

        public List<SavingreceiptViewDTO> GetSavingReceiptView(string FromDate, string Todate, string Connectionstring)
        {
            List<SavingreceiptViewDTO> lstSavingReceiptView = new List<SavingreceiptViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select saving_account_receipt_id, savingaccountno, TM.memberid, TM.membername, TM.membercode, TM.membertypeid, TM.membertype, TM.contactid, TM.contacttype, TM.contactreferenceid, to_char(trans_date, 'dd/Mon/yyyy') trans_date, received_amount, mode_of_receipt, receipt_no, narration, (case when TT.clearstatus = 'R' OR TT.clearstatus = 'C' OR TT.depositstatus = 'C' then 'Cancelled' when TT.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from saving_account_receipt SR join tblmstmembers TM on TM.memberid = SR.member_id left join tbltransreceiptreference TT on TT.receiptid = SR.receipt_no left join tbltranssavingaccountcreation TS on TS.savingaccountid = SR.saving_account_id where SR.status = true and trans_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(Todate) + "' order by saving_account_receipt_id desc;"))
                {
                    while (dr.Read())
                    {
                        SavingreceiptViewDTO objSavingReceipt = new SavingreceiptViewDTO();
                        objSavingReceipt.psavingaccountreceiptid = Convert.ToInt64(dr["saving_account_receipt_id"]);
                        objSavingReceipt.psavingaccountno = Convert.ToString(dr["savingaccountno"]);
                        objSavingReceipt.pmemberid = Convert.ToInt64(dr["memberid"]);
                        objSavingReceipt.pmembername = Convert.ToString(dr["membername"]);
                        objSavingReceipt.pmembercode = Convert.ToString(dr["membercode"]);
                        objSavingReceipt.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                        objSavingReceipt.pmembertype = Convert.ToString(dr["membertype"]);
                        objSavingReceipt.pcontactid = Convert.ToInt64(dr["contactid"]);
                        objSavingReceipt.pcontacttype = Convert.ToString(dr["contacttype"]);
                        objSavingReceipt.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        objSavingReceipt.preceiptdate = dr["trans_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["trans_date"]).ToString("dd/MM/yyyy");
                        objSavingReceipt.preceivedamount = Convert.ToDecimal(dr["received_amount"]);
                        objSavingReceipt.pmodeofreceipt = Convert.ToString(dr["mode_of_receipt"]);
                        objSavingReceipt.preceiptno = Convert.ToString(dr["receipt_no"]);
                        objSavingReceipt.pnarration = Convert.ToString(dr["narration"]);
                        objSavingReceipt.pChequeStatus = Convert.ToString(dr["ChequeStatus"]);
                        lstSavingReceiptView.Add(objSavingReceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSavingReceiptView;
        }

        public List<SavingTransactionDTO> GetSavingTransaction(Int64 SavingAccountId, string Connectionstring)
        {
            List<SavingTransactionDTO> lstSavingTransaction = new List<SavingTransactionDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select trans_date, receipt_no, received_amount, mode_of_receipt from saving_account_receipt where saving_account_id = " + SavingAccountId + " and status = true;"))
                {
                    while (dr.Read())
                    {
                        SavingTransactionDTO objSavingTransaction = new SavingTransactionDTO();
                        objSavingTransaction.ptransdate = dr["trans_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["trans_date"]).ToString("dd/MM/yyyy");
                        objSavingTransaction.preceiptno = Convert.ToString(dr["receipt_no"]);
                        objSavingTransaction.preceivedamount = Convert.ToDecimal(dr["received_amount"]);
                        objSavingTransaction.pmodeofreceipt = Convert.ToString(dr["mode_of_receipt"]);
                        lstSavingTransaction.Add(objSavingTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSavingTransaction;
        }
        #endregion

        #region Share Receipt
        public List<ShareAccNameDetails> GetShareAccountNameDetails(string ConnectionString)
        {
            List<ShareAccNameDetails> _ShareAccNameDetails = new List<ShareAccNameDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select shareconfigid, sharename, sharenamecode from tblmstshareconfig where shareconfigid in (select distinct share_config_id from Share_Account_creation where statusid = " + Convert.ToInt32(Status.Active) + ") order by sharename;"))
                {
                    while (dr.Read())
                    {
                        ShareAccNameDetails _objShareAccNameDetails = new ShareAccNameDetails();

                        _objShareAccNameDetails.pShareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                        _objShareAccNameDetails.pSharename = Convert.ToString(dr["sharename"]);
                        _objShareAccNameDetails.pShareNameCode = Convert.ToString(dr["sharenamecode"]);
                        _ShareAccNameDetails.Add(_objShareAccNameDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ShareAccNameDetails;
        }
        public List<ShareAccDetails> GetShareAccountNumberDetails(Int64 ShareConfigid, string ConnectionString)
        {
            List<ShareAccDetails> _ShareAccDetails = new List<ShareAccDetails>();
            try
            {
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct share_account_id, share_account_number, trans_date, member_type_id, TM.membertype, member_id, member_name, membercode, applicant_type, TM.contactid, TM.contacttype, TM.contactreferenceid, share_config_id, share_name, reference_no, coalesce(facevalue,0) facevalue, coalesce(total_amount,0) total_amount, no_of_shares_issued, minshares, maxshares, distinctive_from, distinctive_to, shares_issue_date, businessentitycontactno, divedendpayout, (select nomineename from tabapplicationpersonalnomineedetails where upper(applicantype) = 'SHARE' and isprimarynominee = true and applicationid = SA.share_account_id), accountid from tblmstshareconfigdetails TS join Share_Account_creation SA on SA.share_config_id = TS.shareconfigid and SA.applicant_type = TS.applicanttype join tblmstmembers TM on SA.member_id = TM.memberid join tblmstcontact TC on TC.contactid = TM.contactid where share_config_id = " + ShareConfigid + " and SA.statusid = " + Convert.ToInt32(Status.Active) + " and share_account_id not in (select distinct share_account_id from share_receipt where status = true) order by share_account_number;"))

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct share_account_id, share_account_number, trans_date, member_type_id, TM.membertype, member_id, member_name, membercode, applicant_type, TM.contactid, TM.contacttype, TM.contactreferenceid, share_config_id, share_name, reference_no, coalesce(face_value,0) facevalue,distinctive_from,distinctive_to, coalesce(total_amount,0) total_amount, no_of_shares_issued, shares_issue_date, businessentitycontactno,  (select nomineename from tabapplicationpersonalnomineedetails where upper(applicantype) = 'SHARE' and isprimarynominee = true and applicationid = SA.share_account_id), accountid from  Share_Account_creation SA join tblmstmembers TM on SA.member_id = TM.memberid join tblmstcontact TC on TC.contactid = TM.contactid where share_config_id = " + ShareConfigid + " and SA.statusid = " + Convert.ToInt32(Status.Active) + " and share_account_id not in (select distinct share_account_id from share_receipt where status = true) order by share_account_number;"))


                {
                    while (dr.Read())
                    {
                        ShareAccDetails _objShareAccDetails = new ShareAccDetails();
                        _objShareAccDetails.pshareaccountid = Convert.ToInt64(dr["share_account_id"]);
                        _objShareAccDetails.pshareaccountnumber = Convert.ToString(dr["share_account_number"]);
                        _objShareAccDetails.ptransdate = dr["trans_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["trans_date"]).ToString("dd/MM/yyyy");
                        _objShareAccDetails.pmembertypeid = Convert.ToInt64(dr["member_type_id"]);
                        _objShareAccDetails.pmembertype = Convert.ToString(dr["membertype"]);
                        _objShareAccDetails.pmemberid = Convert.ToInt64(dr["member_id"]);
                        _objShareAccDetails.papplicanttype = Convert.ToString(dr["applicant_type"]);
                        _objShareAccDetails.pmembercode = Convert.ToString(dr["membercode"]);
                        _objShareAccDetails.pmembername = Convert.ToString(dr["member_name"]);
                        _objShareAccDetails.pcontactid = Convert.ToInt64(dr["contactid"]);
                        _objShareAccDetails.pcontacttype = Convert.ToString(dr["contacttype"]);
                        _objShareAccDetails.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        _objShareAccDetails.pcontactno = Convert.ToString(dr["businessentitycontactno"]);
                        _objShareAccDetails.pshareconfigid = Convert.ToInt64(dr["share_config_id"]);
                        _objShareAccDetails.psharename = Convert.ToString(dr["share_name"]);
                        _objShareAccDetails.preferenceno = Convert.ToString(dr["reference_no"]);
                        _objShareAccDetails.pfacevalue = Convert.ToDecimal(dr["facevalue"]);
                        _objShareAccDetails.ptotalamount = Convert.ToDecimal(dr["total_amount"]);
                        _objShareAccDetails.pnoofsharesissued = Convert.ToInt64(dr["no_of_shares_issued"]);
                        //_objShareAccDetails.pminshares = Convert.ToInt64(dr["minshares"]);
                        //_objShareAccDetails.pmaxshares = Convert.ToInt64(dr["maxshares"]);
                        _objShareAccDetails.pdistinctivefrom = Convert.ToInt64(dr["distinctive_from"]);
                        _objShareAccDetails.pdistinctiveto = Convert.ToInt64(dr["distinctive_to"]);
                        _objShareAccDetails.psharesissuedate = dr["shares_issue_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["shares_issue_date"]).ToString("dd/MM/yyyy");
                        //  _objShareAccDetails.pdivedendpayout = Convert.ToString(dr["divedendpayout"]);
                        _objShareAccDetails.pnomineename = Convert.ToString(dr["nomineename"]);
                        _objShareAccDetails.pAccountid = Convert.ToInt64(dr["accountid"]);
                        _ShareAccDetails.Add(_objShareAccDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ShareAccDetails;
        }
        public bool SaveSharegenaralreceipt(ShareReceiptDTO ObjShareReceiptDTO, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = ObjShareReceiptDTO.pConid;
                Objgeneralreceipt.ppartyreferenceid = ObjShareReceiptDTO.pContactid;
                Objgeneralreceipt.ppartyreftype = "Saving Account Receipt";
                Objgeneralreceipt.ppartyname = ObjShareReceiptDTO.pMembername;
                Objgeneralreceipt.preceiptdate = ObjShareReceiptDTO.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = ObjShareReceiptDTO.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(ObjShareReceiptDTO.pReceivedamount);
                Objgeneralreceipt.pnarration = ObjShareReceiptDTO.pNarration;
                Objgeneralreceipt.pbankname = ObjShareReceiptDTO.pBank;
                Objgeneralreceipt.pBankId = ObjShareReceiptDTO.pBankid;
                Objgeneralreceipt.pdepositbankid = ObjShareReceiptDTO.pdepositbankid;
                Objgeneralreceipt.pbranchname = ObjShareReceiptDTO.pBranch;
                Objgeneralreceipt.ptranstype = ObjShareReceiptDTO.pTranstype;
                Objgeneralreceipt.ptypeofpayment = ObjShareReceiptDTO.ptypeofpayment;
                Objgeneralreceipt.pChequenumber = ObjShareReceiptDTO.pChequenumber;
                if (ObjShareReceiptDTO.ptypeofpayment == "Debit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjShareReceiptDTO.pReceiptdate.ToString();
                }
                else if (ObjShareReceiptDTO.ptypeofpayment == "Credit Card")
                {
                    Objgeneralreceipt.pchequedate = ObjShareReceiptDTO.pReceiptdate.ToString();
                }
                else
                {
                    Objgeneralreceipt.pchequedate = ObjShareReceiptDTO.pchequedate;
                }

                Objgeneralreceipt.pCardNumber = ObjShareReceiptDTO.pCardnumber;
                Objgeneralreceipt.pUpiid = ObjShareReceiptDTO.pUpiid;
                Objgeneralreceipt.pCreatedby = ObjShareReceiptDTO.pCreatedby;
                string membername = ObjShareReceiptDTO.pMembername;
                string membercode = ObjShareReceiptDTO.pMembercode;
                string membernamecode = membercode + "_" + membername;
                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();

                objpreceipts.psubledgerid = ObjShareReceiptDTO.pSubledgerid;
                objpreceipts.pledgername = membername;
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(ObjShareReceiptDTO.pReceivedamount);
                objpreceipts.pamount = Convert.ToDecimal(ObjShareReceiptDTO.pReceivedamount);
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
        public bool SaveShareReceipt(ShareReceiptDTO ObjShareReceiptDTO, string ConnectionString, out string OUTReceiptid)
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
                if (ObjShareReceiptDTO.pModeofreceipt.ToUpper() != "ADJUSTMENT")
                {
                    if (SaveSharegenaralreceipt(ObjShareReceiptDTO, trans, out OUTReceiptid))
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
                    objJournalVoucherDTO.pjvdate = ObjShareReceiptDTO.pReceiptdate;
                    objJournalVoucherDTO.pCreatedby = ObjShareReceiptDTO.pCreatedby;
                    objJournalVoucherDTO.pnarration = ObjShareReceiptDTO.pNarration;
                    objJournalVoucherDTO.pmodoftransaction = "MANUAL";
                    PaymentsNewDTO objPaymentsDTO = new PaymentsNewDTO
                    {

                        ppartyid = null,
                        ptranstype = "C",
                        psubledgerid = ObjShareReceiptDTO.pSubledgerid,
                        pamount = Convert.ToDecimal(ObjShareReceiptDTO.pReceivedamount)
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
                    objPaymentsDTO.psubledgerid = ObjShareReceiptDTO.pSavingsMemberAccountid;
                    objPaymentsDTO.pamount = Convert.ToDecimal(ObjShareReceiptDTO.pReceivedamount);
                    _Paymentslist.Add(objPaymentsDTO);
                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                    AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
                    if (Accontstrans.SaveJournalVoucherNew(objJournalVoucherDTO, trans, out adjustrefno))
                    {
                        Referenceno = adjustrefno;
                    }

                }

                if (string.IsNullOrEmpty(ObjShareReceiptDTO.pRecordid.ToString()) || ObjShareReceiptDTO.pRecordid == 0)
                {
                    sbInsert.Append("insert into Share_receipt (Share_receipt_date, member_id, Share_account_id, received_amount, mode_of_receipt, receipt_no, narration, status) values ('" + FormatDate(ObjShareReceiptDTO.pReceiptdate) + "', " + ObjShareReceiptDTO.pMemberid + ", " + ObjShareReceiptDTO.pshareaccountid + ", " + ObjShareReceiptDTO.pReceivedamount + ", '" + ObjShareReceiptDTO.pModeofreceipt + "', '" + Referenceno + "', '" + ObjShareReceiptDTO.pNarration + "', true);");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
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
        public List<ShareTransactionDTO> GetShareTransaction(Int64 ShareAccountId, string Connectionstring)
        {
            List<ShareTransactionDTO> lstShareTransaction = new List<ShareTransactionDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select trans_date, receipt_no, received_amount, mode_of_receipt from saving_account_receipt where saving_account_id = " + ShareAccountId + " and status = true;"))
                {
                    while (dr.Read())
                    {
                        ShareTransactionDTO objShareTransactionDTO = new ShareTransactionDTO();
                        objShareTransactionDTO.ptransdate = dr["trans_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["trans_date"]).ToString("dd/MM/yyyy");
                        objShareTransactionDTO.preceiptno = Convert.ToString(dr["receipt_no"]);
                        objShareTransactionDTO.preceivedamount = Convert.ToDecimal(dr["received_amount"]);
                        objShareTransactionDTO.pmodeofreceipt = Convert.ToString(dr["mode_of_receipt"]);
                        lstShareTransaction.Add(objShareTransactionDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstShareTransaction;
        }

        public List<SharereceiptViewDTO> GetShareReceiptView(string FromDate, string Todate, string Connectionstring)
        {
            List<SharereceiptViewDTO> lstSharereceiptView = new List<SharereceiptViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select Share_receipt_id, SR.Share_account_id, to_char(share_receipt_date, 'dd/Mon/yyyy') share_receipt_date, share_account_number, TM.memberid, TM.membername, TM.membercode, TM.membertypeid, TM.membertype, TM.contactid, TM.contacttype, TM.contactreferenceid, trans_date, received_amount, mode_of_receipt, receipt_no, narration, (case when TT.clearstatus = 'R' OR TT.clearstatus = 'C' OR TT.depositstatus = 'C' then 'Cancelled' when TT.clearstatus = 'Y' then 'Cleared' when tt.clearstatus IS NULL then 'Cleared' else 'Un-Cleared' end)as ChequeStatus from Share_receipt SR join tblmstmembers TM on TM.memberid = SR.member_id left join tbltransreceiptreference TT on TT.receiptid = SR.receipt_no join Share_Account_creation TS on TS.Share_account_id = SR.Share_account_id and SR.status = true where SR.status = true and Share_receipt_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(Todate) + "' order by Share_receipt_id desc;"))
                {
                    while (dr.Read())
                    {
                        SharereceiptViewDTO objSharereceipt = new SharereceiptViewDTO();
                        objSharereceipt.psharereceiptid = Convert.ToInt64(dr["Share_receipt_id"]);
                        objSharereceipt.pshareaccountno = Convert.ToString(dr["share_account_number"]);
                        objSharereceipt.pmemberid = Convert.ToInt64(dr["memberid"]);
                        objSharereceipt.pmembername = Convert.ToString(dr["membername"]);
                        objSharereceipt.pmembercode = Convert.ToString(dr["membercode"]);
                        objSharereceipt.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                        objSharereceipt.pmembertype = Convert.ToString(dr["membertype"]);
                        objSharereceipt.pcontactid = Convert.ToInt64(dr["contactid"]);
                        objSharereceipt.pcontacttype = Convert.ToString(dr["contacttype"]);
                        objSharereceipt.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        objSharereceipt.preceiptdate = dr["share_receipt_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["share_receipt_date"]).ToString("dd/MM/yyyy");
                        objSharereceipt.preceivedamount = Convert.ToDecimal(dr["received_amount"]);
                        objSharereceipt.pmodeofreceipt = Convert.ToString(dr["mode_of_receipt"]);
                        objSharereceipt.preceiptno = Convert.ToString(dr["receipt_no"]);
                        objSharereceipt.pnarration = Convert.ToString(dr["narration"]);
                        objSharereceipt.pChequeStatus = Convert.ToString(dr["ChequeStatus"]);
                        lstSharereceiptView.Add(objSharereceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSharereceiptView;
        }
        #endregion
    }
}
