using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public class ReceiptsDAL : SettingsDAL, IReceipts
    {
        // public ReceiptsDTO ReceiptsDTO { get; set; }
        public List<EmiReceiptsDTO> lstReceipts { get; set; }
        public List<ParticularsDTO> lstParticulars { get; set; }
        public List<OutstandingbalDTO> lstOutstandingbal { get; set; }
        public List<TransactionsDTO> lstTransactions { get; set; }
        public List<LoannamesDTO> lstLoannames { get; set; }
        public List<LoandetailsDTO> lstLoandetails { get; set; }
        public List<ViewParticularsDTO> lstParticularsDetails { get; set; }
        public List<ViewtodayreceiptsDTO> lstViewtodayreceipts { get; set; }

        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;

        #region  GetLoannames
        public List<LoannamesDTO> GetLoannames(string ConnectionString)
        {
            lstLoannames = new List<LoannamesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT loantypeid,loantype FROM tblmstloantypes  WHERE STATUSID=1 order by loantype"))
                    while (dr.Read())
                    {
                        LoannamesDTO objLoannames = new LoannamesDTO();
                        objLoannames.pLoanid = Convert.ToInt64(dr["loantypeid"]);
                        objLoannames.pLoanname = dr["loantype"].ToString();
                        lstLoannames.Add(objLoannames);

                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoannames;
        }
        #endregion

        #region  GetApplicantionid
        public List<EmiReceiptsDTO> GetApplicantionid(string loanname, string ConnectionString, string formname)
        {
            lstReceipts = new List<EmiReceiptsDTO>();
            string strquery = string.Empty;
            try
            {
                if (formname.ToUpper() == "RECEIPT")
                {
                    strquery = "select ta.loanname,ta.applicationid,ta.vchapplicationid,applicantname,debtorsaccountid,ta.contactreferenceid,ta.applicantid,ta.contacttype from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid where upper(ta.loantype)=upper('" + ManageQuote(loanname) + "') and ta.loanstatus in('Loan Approved','Disbursed')  order by ta.vchapplicationid;";
                }
                else if (formname.ToUpper() == "PRECLOSURE")
                {
                    strquery = "select ta.loanname,ta.applicationid,ta.vchapplicationid,applicantname,debtorsaccountid,ta.contactreferenceid,ta.applicantid,ta.contacttype from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid where upper(ta.loantype)=upper('" + ManageQuote(loanname) + "') and ta.loanstatus in('Loan Approved','Disbursed')  order by ta.vchapplicationid;";

                }
                else
                {
                    strquery = "select ta.loanname,ta.applicationid,ta.vchapplicationid,applicantname,debtorsaccountid,ta.contactreferenceid,ta.applicantid,ta.contacttype from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid where upper(ta.loantype)=upper('" + ManageQuote(loanname) + "') and ta.loanstatus in('Loan Approved','Disbursed','EMI Closed','Pre Closed')  order by ta.vchapplicationid;";

                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                    while (dr.Read())
                    {
                        EmiReceiptsDTO objreceipts = new EmiReceiptsDTO();
                        objreceipts.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                        objreceipts.pVchapplicationid = dr["Vchapplicationid"].ToString();
                        objreceipts.pApplicantname = dr["applicantname"].ToString();
                        objreceipts.pAccountid = Convert.ToInt64(dr["debtorsaccountid"]);
                        objreceipts.pContactid = dr["contactreferenceid"].ToString();
                        objreceipts.pConid = Convert.ToInt64(dr["applicantid"]);
                        objreceipts.pContacttype = dr["contacttype"].ToString();
                        objreceipts.pLoanname = dr["loanname"].ToString();
                        lstReceipts.Add(objreceipts);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReceipts;
        }
        #endregion

        #region  GetParticulars
        public List<ParticularsDTO> GetParticulars(string Loanid, string transdate, string ConnectionString, string formname)
        {
            lstParticulars = new List<ParticularsDTO>();
            try
            {
                if (formname == "RECEIPT")
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * from FN_GETLOANRECEIPTDETAILS('" + Loanid + "','" + FormatDate(transdate) + "')"))
                        while (dr.Read())
                        {
                            ParticularsDTO objParticulars = new ParticularsDTO();
                            objParticulars.pParticularstype = dr["PARTICULARSTYPE"].ToString();
                            objParticulars.pParticularsname = dr["PARTICULARSNAME"].ToString();
                            objParticulars.pPrinciplereceivable = Convert.ToDecimal(dr["PRINCIPLE"]);
                            objParticulars.pIntrestreceivable = Convert.ToDecimal(dr["INTEREST"]);
                            objParticulars.pAmount = Convert.ToDecimal(dr["Amount"]);
                            objParticulars.pInstalmentdues = Convert.ToInt64(dr["PINSTALMENTSDUE"]);
                            objParticulars.pEmiamount = Convert.ToDecimal(dr["EMIAMOUNT"]);

                            if (dr["LASTRECEIPTDATE"] != DBNull.Value)
                            {
                                objParticulars.pLastreceiptdate = Convert.ToDateTime(dr["LASTRECEIPTDATE"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                objParticulars.pLastreceiptdate = "";
                            }
                            objParticulars.pLastreceiptamount = Convert.ToDecimal(dr["LASTPAIDAMOUNT"]);
                            objParticulars.pGsttype = dr["GSTTYPE"].ToString();
                            objParticulars.pGstcaltype = dr["GSTCALCULATIONTYPE"].ToString();
                            objParticulars.pGstpercentage = Convert.ToDecimal(dr["GSTPERCENTAGE"]);
                            lstParticulars.Add(objParticulars);
                        }
                }
                else if (formname == "Preclosure")
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * from fn_getpreclosure_forrecieptdata('" + Loanid + "','" + FormatDate(transdate) + "')"))
                        while (dr.Read())
                        {
                            ParticularsDTO objParticulars = new ParticularsDTO();
                            objParticulars.pParticularstype = dr["PARTICULARSTYPE"].ToString();
                            objParticulars.pParticularsname = dr["PARTICULARSNAME"].ToString();
                            //objParticulars.pPrinciplereceivable = Convert.ToDecimal(dr["PRINCIPLE"]);
                            //objParticulars.pIntrestreceivable = Convert.ToDecimal(dr["INTEREST"]);
                            objParticulars.pAmount = Convert.ToDecimal(dr["Amount"]);
                            objParticulars.pInstalmentdues = Convert.ToInt64(dr["PINSTALMENTSDUEcount"]);
                            objParticulars.pEmiamount = Convert.ToDecimal(dr["EMIAMOUNT"]);

                            if (dr["LASTRECEIPTDATE"] != DBNull.Value)
                            {
                                objParticulars.pLastreceiptdate = Convert.ToDateTime(dr["LASTRECEIPTDATE"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                objParticulars.pLastreceiptdate = "";
                            }
                            objParticulars.pLastreceiptamount = dr["LASTPAIDAMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["LASTPAIDAMOUNT"]);
                            objParticulars.pGsttype = dr["GSTTYPE"].ToString();
                            objParticulars.pGstcaltype = dr["GSTCALCULATIONTYPE"].ToString();
                            objParticulars.pGstpercentage = Convert.ToDecimal(dr["GSTPERCENTAGE"]);
                            objParticulars.pFutureprinciple = dr["Futureprinciple"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Futureprinciple"]);
                            objParticulars.pFutureinterest = dr["Futureinterest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Futureinterest"]);
                            objParticulars.pFutureduecount = dr["Futureduecount"] == DBNull.Value ? 0 : Convert.ToInt64(dr["Futureduecount"]);
                            lstParticulars.Add(objParticulars);
                        }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstParticulars;
        }
        #endregion

        #region  ViewParticularsDetails      
        public List<ViewParticularsDTO> ViewParticularsDetails(string Loanid, string transdate, string ConnectionString, string todate, string duestype)
        {
            lstParticularsDetails = new List<ViewParticularsDTO>();
            try
            {
                if (duestype == null)
                    duestype = "ASON";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  vl.particulars1, ti.vchapplicationid,vl.instalmentno1,vl.instalmentdate1,vl.receivableprinciple,vl.receivableinterest,vl.receivableamount,vl.receivablepenalty,ti.instalmentamount  from fn_dues_duesdetailedreport('" + ManageQuote(Loanid) + "','" + FormatDate(transdate.ToString()) + "','" + FormatDate(transdate.ToString()) + "','" + duestype + "') vl join tbltransinstalments ti on vl.instalmentno1=ti.instalmentno and vl.vchapplicationid1=ti.vchapplicationid   where vl.vchapplicationid1='" + ManageQuote(Loanid) + "'  and vl.instalmentdate1<='" + FormatDate(transdate.ToString()) + "' and (receivableamount>0 or receivablepenalty>0) order by particulars1,instalmentno1;"))
                    while (dr.Read())
                    {
                        ViewParticularsDTO objParticularsDetails = new ViewParticularsDTO();
                        objParticularsDetails.pParticularsname = dr["particulars1"].ToString();
                        objParticularsDetails.pApplicationid = dr["vchapplicationid"].ToString();
                        objParticularsDetails.pInstalmentno = Convert.ToInt16(dr["instalmentno1"]);
                        if (dr["instalmentdate1"] != DBNull.Value)
                        {
                            objParticularsDetails.pInstalmentdate = Convert.ToDateTime(dr["instalmentdate1"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objParticularsDetails.pInstalmentdate = "";
                        }
                        objParticularsDetails.pPrinciplereceivable = Convert.ToDecimal(dr["receivableprinciple"]);
                        objParticularsDetails.pIntrestreceivable = Convert.ToDecimal(dr["receivableinterest"]);
                        objParticularsDetails.pInstalmentdue = Convert.ToDecimal(dr["receivableamount"]);
                        objParticularsDetails.pPenaltyreceivable = Convert.ToDecimal(dr["receivablepenalty"]);
                        objParticularsDetails.pInstalmentamount = Convert.ToDecimal(dr["instalmentamount"]);
                        objParticularsDetails.pAmountadusted = (Convert.ToDecimal(dr["instalmentamount"]) - Convert.ToDecimal(dr["receivableamount"])) <= 0 ? 0 : (Convert.ToDecimal(dr["instalmentamount"]) - Convert.ToDecimal(dr["receivableamount"]));
                        lstParticularsDetails.Add(objParticularsDetails);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstParticularsDetails;
        }
        #endregion


        #region  GetTransactions
        public List<OutstandingbalDTO> GetTransactions(string Loanid, string ConnectionString)
        {
            lstOutstandingbal = new List<OutstandingbalDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT  COALESCE(SUM(INSTALMENTPRINCIPLE),0)-COALESCE(SUM(PAIDPRINCIPLE),0) as principle,COALESCE(SUM(INSTALMENTINTEREST),0)-COALESCE(SUM(PAIDINTEREST),0) as interest FROM TBLTRANSINSTALMENTS WHERE vchapplicationid='" + Loanid + "' ;"))
                    while (dr.Read())
                    {
                        OutstandingbalDTO objOutstandingbal = new OutstandingbalDTO();
                        objOutstandingbal.pPrincipal = Convert.ToDecimal(dr["principle"]);
                        objOutstandingbal.pInterest = Convert.ToDecimal(dr["interest"]);
                        objOutstandingbal.lstTransactions = GetTransactionslist(Loanid, ConnectionString);
                        lstOutstandingbal.Add(objOutstandingbal);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstOutstandingbal;
        }

        private List<TransactionsDTO> GetTransactionslist(string loanid, string ConnectionString)
        {
            lstTransactions = new List<TransactionsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, " select totalreceived,receiptno,receiptdate,narration,case when chrclearstatus in('N','P') then 'Not Cleared'  when chrclearstatus='R' then 'Return' else 'Cleared' end as chequestatus  from tbltransemireceipt where vchapplicationid='" + ManageQuote(loanid) + "' and chrclearstatus in('N','P','Y','') order by emiid desc;;"))
                    while (dr.Read())
                    {
                        TransactionsDTO objTransactions = new TransactionsDTO();
                        objTransactions.pReceiptamount = Convert.ToDecimal(dr["totalreceived"]);
                        objTransactions.pReceiptno = dr["receiptno"].ToString();
                        objTransactions.pNarration = dr["narration"].ToString();
                        if (dr["receiptdate"] != DBNull.Value)
                        {
                            objTransactions.pReceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objTransactions.pReceiptdate = "";

                        }
                        objTransactions.pChequestatus = dr["chequestatus"].ToString();
                        lstTransactions.Add(objTransactions);
                    }
            }
            catch (Exception)
            {
                throw;
            }
            return lstTransactions;
        }
        #endregion

        #region  GetLoandetails
        public List<LoandetailsDTO> GetLoandetails(string Loanid, string ConnectionString)
        {
            lstLoandetails = new List<LoandetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  ta.loanstatus , ta.vchapplicationid, ta.applicantname, ta.dateofapplication, ta.loantype, ta.loanname, ta.purposeofloan, tap.approvedloanamount, tap.tenureofloan, tap.loanpayin, tap.rateofinterest, tap.interesttype, tap.installmentamount from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid = tap.vchapplicationid where ta.vchapplicationid = '" + ManageQuote(Loanid) + "'; "))
                    while (dr.Read())
                    {
                        LoandetailsDTO objLoandetails = new LoandetailsDTO();
                        objLoandetails.pApplicantname = dr["applicantname"].ToString();
                        if (dr["dateofapplication"] != DBNull.Value)
                        {
                            objLoandetails.pDateofapplication = Convert.ToDateTime(dr["dateofapplication"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objLoandetails.pDateofapplication = "";

                        }
                        objLoandetails.pLoantype = dr["loantype"].ToString();
                        objLoandetails.pLoanname = dr["loanname"].ToString();
                        objLoandetails.pPurposeofloan = dr["purposeofloan"].ToString();
                        objLoandetails.pApprovedamount = Convert.ToDecimal(dr["approvedloanamount"]);
                        objLoandetails.pTenureofloan = Convert.ToDecimal(dr["tenureofloan"]);
                        objLoandetails.pLoanpayin = dr["loanpayin"].ToString();
                        objLoandetails.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                        objLoandetails.pInteresttype = dr["interesttype"].ToString();
                        objLoandetails.pEmiAmount = Convert.ToDecimal(dr["installmentamount"]);
                        objLoandetails.pLoanstatus = dr["loanstatus"].ToString();
                        lstLoandetails.Add(objLoandetails);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstLoandetails;
        }
        #endregion

        #region  SaveEmiReceipt
        public bool Savegenaralreceipt(SaveEmireceiptsDTO SaveEmireceiptslist, NpgsqlTransaction trans, out string Receiptid)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                GeneralreceiptDTO Objgeneralreceipt = new GeneralreceiptDTO();
                Objgeneralreceipt.preceiptid = "";
                Objgeneralreceipt.ppartyid = SaveEmireceiptslist.pConid;
                Objgeneralreceipt.ppartyreftype = "APPLICANT";
                Objgeneralreceipt.ppartyreferenceid = SaveEmireceiptslist.pContactid;
                Objgeneralreceipt.ppartyname = SaveEmireceiptslist.pApplicantname;
                Objgeneralreceipt.preceiptdate = SaveEmireceiptslist.pReceiptdate.ToString();
                Objgeneralreceipt.pmodofreceipt = SaveEmireceiptslist.pModeofreceipt;
                Objgeneralreceipt.ptotalreceivedamount = Convert.ToDecimal(SaveEmireceiptslist.pTotalreceived);
                Objgeneralreceipt.pnarration = SaveEmireceiptslist.pNarration;
                Objgeneralreceipt.pbankname = SaveEmireceiptslist.pBank;
                Objgeneralreceipt.pBankId = SaveEmireceiptslist.pBankid;
                Objgeneralreceipt.pdepositbankid = SaveEmireceiptslist.pDeposibankid;
                Objgeneralreceipt.pbranchname = SaveEmireceiptslist.pBranch;
                Objgeneralreceipt.ptranstype = SaveEmireceiptslist.pTranstype;
                Objgeneralreceipt.ptypeofpayment = SaveEmireceiptslist.pTypeofpaymentonline;
                Objgeneralreceipt.pChequenumber = SaveEmireceiptslist.pChequeno;
                Objgeneralreceipt.pchequedate = SaveEmireceiptslist.pTransdate;
                Objgeneralreceipt.pCardNumber = SaveEmireceiptslist.pVchcardnumber;
                Objgeneralreceipt.pUpiid = SaveEmireceiptslist.pUpiid;
                Objgeneralreceipt.pCreatedby = SaveEmireceiptslist.pCreatedby;

                List<ReceiptsDTO> preceiptslist = new List<ReceiptsDTO>();
                ReceiptsDTO objpreceipts = new ReceiptsDTO();
                objpreceipts.psubledgerid = SaveEmireceiptslist.pSubledgerid;
                objpreceipts.pledgername = "SUNDRY DEBITORS" + "_" + SaveEmireceiptslist.pLoanname;
                objpreceipts.ptdsamountindividual = Convert.ToDecimal(SaveEmireceiptslist.pTotalreceived);
                objpreceipts.pamount = Convert.ToDecimal(SaveEmireceiptslist.pTotalreceived);
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
        public bool SaveEmiReceipt(SaveEmireceiptsDTO SaveEmireceiptslist, string ConnectionString, out string OUTReceiptid)
        {
            StringBuilder sbinsert = new StringBuilder();
            string Query = string.Empty;
            bool IsSaved = false;
            bool IsAccountSaved = false;
            string Receiptid = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (Savegenaralreceipt(SaveEmireceiptslist, trans, out OUTReceiptid))
                {
                    IsAccountSaved = true;
                }
                else
                {
                    trans.Rollback();
                    return IsAccountSaved;
                }

                Query = "insert into tbltransemireceipt(loanno, loanname, applicationid, vchapplicationid, receiptdate, receiptno, principlereceivable, intrestreceivable, installmentreceivable, penaltyreceivable, chargesreceivable, installmentreceived, penaltyreceived, chargesreceived, penaltywaiveoffamount, chargeswaiveoffamount, loanadvanceamount, totalreceived, modeofpayment, referencenumber,bank, branch, dattransdate, narration, typeofpaymentonline, upiid, chrclearstatus, vchcardissuebank, vchcardnumber, vchcardtype, vchcardholdername, statusid, createdby,createddate,formname)values(" + SaveEmireceiptslist.pLoanno + ",'" + ManageQuote(SaveEmireceiptslist.pLoanname) + "'," + SaveEmireceiptslist.pApplicationid + ",'" + ManageQuote(SaveEmireceiptslist.pVchapplicationid) + "','" + FormatDate(SaveEmireceiptslist.pReceiptdate.ToString()) + "','" + ManageQuote(OUTReceiptid) + "'," + SaveEmireceiptslist.pPrincipalreceivable + "," + SaveEmireceiptslist.pInterestreceivable + "," + SaveEmireceiptslist.pInstalmentreceivable + "," + SaveEmireceiptslist.pPenaltyreceivable + "," + SaveEmireceiptslist.pChargesreceivable + "," + SaveEmireceiptslist.pInstallmentreceived + "," + SaveEmireceiptslist.pPenaltyreceived + "," + SaveEmireceiptslist.pChargesreceived + "," + SaveEmireceiptslist.pPenaltywaiveoffamount + "," + SaveEmireceiptslist.pChargeswaiveoffamount + "," + SaveEmireceiptslist.pLoanadvanceamount + "," + SaveEmireceiptslist.pTotalreceived + ",'" + ManageQuote(SaveEmireceiptslist.pModeofreceipt) + "','" + ManageQuote(SaveEmireceiptslist.pChequeno) + "','" + ManageQuote(SaveEmireceiptslist.pBank) + "','" + ManageQuote(SaveEmireceiptslist.pBranch) + "','" + FormatDate(SaveEmireceiptslist.pTransdate.ToString()) + "','" + ManageQuote(SaveEmireceiptslist.pNarration) + "','" + ManageQuote(SaveEmireceiptslist.pTypeofpaymentonline) + "','" + ManageQuote(SaveEmireceiptslist.pUpiid) + "','" + ManageQuote(SaveEmireceiptslist.pChrclearstatus) + "','" + ManageQuote(SaveEmireceiptslist.pVchcardissuebank) + "','" + ManageQuote(SaveEmireceiptslist.pVchcardnumber) + "','" + ManageQuote(SaveEmireceiptslist.pVchcardtype) + "','" + ManageQuote(SaveEmireceiptslist.pVchcardholdername) + "',1," + SaveEmireceiptslist.pCreatedby + ",current_timestamp,'" + SaveEmireceiptslist.pFormname + "') returning emiid";

                if (!string.IsNullOrEmpty(Query))
                {
                    SaveEmireceiptslist.pEmiid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, Query));
                }
                if (SaveEmireceiptslist.lstSaveEmireceiptdetails != null)
                {
                    if (SaveEmireceiptslist.pFormname == "RECEIPT")
                    {
                        for (int i = 0; i < SaveEmireceiptslist.lstSaveEmireceiptdetails.Count; i++)
                        {
                            if (SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pParticularstype == "CHARGES")
                            {
                                if (SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivedamount > 0 || SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pWaiveoffamount > 0)
                                {
                                    sbinsert.Append(" insert into tbltransemireceiptdetails(emiid,receiptno,applicationid,vchapplicationid,particularstype,particularname,detailsreceivabledate,detailsreceivableamount,waiveoffamount,detailsreceivedamount,statusid,createdby,createddate,gsttype,gstcalculationtype,gstpercentage,formname)values(" + SaveEmireceiptslist.pEmiid + ",'" + ManageQuote(OUTReceiptid) + "'," + SaveEmireceiptslist.pApplicationid + ",'" + ManageQuote(SaveEmireceiptslist.pVchapplicationid) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pParticularstype) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pParticularname) + "','" + FormatDate(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivabledate.ToString()) + "'," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivableamount + "," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pWaiveoffamount + "," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivedamount + ",1," + SaveEmireceiptslist.pCreatedby + ",current_timestamp,'" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGsttype) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGstcaltype) + "'," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGstpercentage + ",'" + SaveEmireceiptslist.pFormname + "');");
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < SaveEmireceiptslist.lstSaveEmireceiptdetails.Count; i++)
                        {
                            if (SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivedamount > 0 || SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pWaiveoffamount > 0)
                            {
                                sbinsert.Append(" insert into tbltransemireceiptdetails(emiid,receiptno,applicationid,vchapplicationid,particularstype,particularname,detailsreceivabledate,detailsreceivableamount,waiveoffamount,detailsreceivedamount,statusid,createdby,createddate,gsttype,gstcalculationtype,gstpercentage,formname)values(" + SaveEmireceiptslist.pEmiid + ",'" + ManageQuote(OUTReceiptid) + "'," + SaveEmireceiptslist.pApplicationid + ",'" + ManageQuote(SaveEmireceiptslist.pVchapplicationid) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pParticularstype) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pParticularname) + "','" + FormatDate(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivabledate.ToString()) + "'," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivableamount + "," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pWaiveoffamount + "," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pDetailsreceivedamount + ",1," + SaveEmireceiptslist.pCreatedby + ",current_timestamp,'" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGsttype) + "','" + ManageQuote(SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGstcaltype) + "'," + SaveEmireceiptslist.lstSaveEmireceiptdetails[i].pGstpercentage + ",'" + SaveEmireceiptslist.pFormname + "');");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
                    }
                }
                if (SaveEmireceiptslist.pModeofreceipt.ToUpper() == "CASH")
                {

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "SELECT FN_EMI_CREDITJV('" + ManageQuote(SaveEmireceiptslist.pVchapplicationid) + "','" + OUTReceiptid + "','" + SaveEmireceiptslist.pFormname + "')");

                }

                trans.Commit();
                IsSaved = true;
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
            return IsSaved;

        }
        #endregion

        #region  Viewtodayreceipts
        public List<ViewtodayreceiptsDTO> Viewtodayreceipts(string fromdate, string todate, string ConnectionString, string formname)
        {
            lstViewtodayreceipts = new List<ViewtodayreceiptsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select receiptdate,receiptno,te.vchapplicationid,ta.applicantname,modeofpayment,bank,referencenumber,totalreceived,narration from tbltransemireceipt te  join tabapplication ta on te.vchapplicationid=ta.vchapplicationid  where receiptdate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and upper(formname)='" + formname + "' order by emiid desc"))
                    while (dr.Read())
                    {
                        ViewtodayreceiptsDTO objtodayreceipts = new ViewtodayreceiptsDTO();
                        if (dr["receiptdate"] != DBNull.Value)
                        {
                            objtodayreceipts.pReceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objtodayreceipts.pReceiptdate = "";
                        }
                        objtodayreceipts.pReceiptno = dr["receiptno"].ToString();
                        objtodayreceipts.pLoanno = dr["vchapplicationid"].ToString();
                        objtodayreceipts.pCustomername = dr["applicantname"].ToString();
                        objtodayreceipts.pModeofreceipt = dr["modeofpayment"].ToString();
                        objtodayreceipts.pBankname = dr["bank"].ToString();
                        objtodayreceipts.pChequeno = dr["referencenumber"].ToString();
                        objtodayreceipts.pTotalreceived = Convert.ToDecimal(dr["totalreceived"]);
                        objtodayreceipts.pNarration = dr["narration"].ToString();
                        lstViewtodayreceipts.Add(objtodayreceipts);
                    }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstViewtodayreceipts;
        }
        #endregion
    }
}
