using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Accounting.Transactions
{
    public partial class AccountingTransactionsDAL : SettingsDAL, IAccountingTransactions
    {
        public GeneralreceiptDTO GeneralreceiptDTO { get; set; }
        public bool SaveGeneralReceipt(GeneralreceiptDTO GeneralreceiptDTO, string Connectionstring, out string pGeneralReceiptId)
        {
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (CallsaveGeneralReceipt(GeneralreceiptDTO, trans, out pGeneralReceiptId))
                {
                    trans.Commit();
                    IsSaved = true;
                }
                else
                {
                    trans.Rollback();
                }
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
        public bool CallsaveGeneralReceipt(GeneralreceiptDTO GeneralreceiptDTO, NpgsqlTransaction trans, out string pGeneralReceiptId)
        {
            StringBuilder sbGeneralReceipt = new StringBuilder();
            bool Savedstatus = false;
            long Debitaccountid = 0;
            string Maxreceiptid = string.Empty;
            try
            {
                // Generate Next Receipt Id
                if (string.IsNullOrEmpty(GeneralreceiptDTO.preceiptid))
                {
                    GeneralreceiptDTO.preceiptid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('GENERAL VOUCHER','" + ManageQuote(GeneralreceiptDTO.pmodofreceipt).Trim().ToUpper() + "','" + FormatDate(GeneralreceiptDTO.preceiptdate) + "')").ToString();
                }
                pGeneralReceiptId = GeneralreceiptDTO.preceiptid;
                if (string.IsNullOrEmpty(GeneralreceiptDTO.preceiptdate))
                {
                    GeneralreceiptDTO.preceiptdate = "null";
                }
                else
                {
                    //GeneralreceiptDTO.preceiptdate = "'" + FormatDate(GeneralreceiptDTO.preceiptdate) + "'";
                    // Not Formats date here due to dependency in JV Save
                    GeneralreceiptDTO.preceiptdate = GeneralreceiptDTO.preceiptdate;
                }
                if (string.IsNullOrEmpty(GeneralreceiptDTO.pchequedate))
                {
                    GeneralreceiptDTO.pchequedate = "null";
                }
                else
                {
                    GeneralreceiptDTO.pchequedate = "'" + FormatDate(GeneralreceiptDTO.pchequedate) + "'";
                }
                GeneralreceiptDTO.ptotalreceivedamount = Convert.ToString(GeneralreceiptDTO.ptotalreceivedamount) == string.Empty ? 0 : GeneralreceiptDTO.ptotalreceivedamount < 0 ? 0 : GeneralreceiptDTO.ptotalreceivedamount;

                if (GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "CASH")
                {
                    Debitaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='CASH ON HAND' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else if (GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "BANK" || GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "WALLET")
                {
                    Debitaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='UNCLEARED CHEQUES A/C' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                if (string.IsNullOrEmpty(GeneralreceiptDTO.ptypeofpayment))
                {
                    GeneralreceiptDTO.ptypeofpayment = GeneralreceiptDTO.ptranstype;
                }
                long GeneralReceiptRecordid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltransgeneralreceipt(receiptid, receiptdate, totalreceivedamount,narration, debitaccountid,contactid,statusid,createdby,createddate,istdsapplicable, modeofreceipt,tdssection, pannumber, tdscalculationtype,tdspercentage, tdsamount,contactname,contactreftype,contactrefid,filename,filepath,fileformat) VALUES ('" + ManageQuote(GeneralreceiptDTO.preceiptid) + "', '" + FormatDate(GeneralreceiptDTO.preceiptdate) + "', " + GeneralreceiptDTO.ptotalreceivedamount + ", '" + ManageQuote(GeneralreceiptDTO.pnarration) + "', " + Debitaccountid + ", " + GeneralreceiptDTO.ppartyid + ", " + Convert.ToInt32(Status.Active) + ", " + GeneralreceiptDTO.pCreatedby + ", current_timestamp, " + GeneralreceiptDTO.pistdsapplicable + ", '" + ManageQuote(GeneralreceiptDTO.pmodofreceipt) + "', '" + ManageQuote(GeneralreceiptDTO.pTdsSection) + "', '" + ManageQuote(GeneralreceiptDTO.ppartypannumber) + "', '" + ManageQuote(GeneralreceiptDTO.ptdscalculationtype) + "'," + GeneralreceiptDTO.pTdsPercentage + ", " + GeneralreceiptDTO.ptdsamount + ",'" + ManageQuote(GeneralreceiptDTO.ppartyname) + "','" + ManageQuote(GeneralreceiptDTO.ppartyreftype) + "','" + ManageQuote(GeneralreceiptDTO.ppartyreferenceid) + "','" + ManageQuote(GeneralreceiptDTO.pFilename) + "','" + ManageQuote(GeneralreceiptDTO.pFilepath) + "','" + ManageQuote(GeneralreceiptDTO.pFileformat) + "') returning recordid;"));

                if (GeneralreceiptDTO.preceiptslist != null && GeneralreceiptDTO.preceiptslist.Count > 0)
                {
                    for (int i = 0; i < GeneralreceiptDTO.preceiptslist.Count; i++)
                    {

                        if (GeneralreceiptDTO.pistdsapplicable && GeneralreceiptDTO.ptdsamount > 0)
                        {
                            if (GeneralreceiptDTO.ptdscalculationtype == "INCLUDE")
                            {
                                GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual = Math.Round((GeneralreceiptDTO.preceiptslist[i].pamount * GeneralreceiptDTO.pTdsPercentage) / (100 + GeneralreceiptDTO.pTdsPercentage));

                                GeneralreceiptDTO.preceiptslist[i].pamount = GeneralreceiptDTO.preceiptslist[i].pamount - GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual;
                            }
                            else if (GeneralreceiptDTO.ptdscalculationtype == "EXCLUDE")
                            {
                                GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual = Math.Round((GeneralreceiptDTO.preceiptslist[i].pamount * GeneralreceiptDTO.pTdsPercentage) / 100);
                            }
                        }
                        else
                        {
                            GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual = 0;
                        }

                        if (!string.IsNullOrEmpty(GeneralreceiptDTO.preceiptslist[i].pState) && GeneralreceiptDTO.preceiptslist[i].pState.Contains('-'))
                        {
                            string[] Stateandgst = GeneralreceiptDTO.preceiptslist[i].pState.Split('-');
                            GeneralreceiptDTO.preceiptslist[i].pState = Stateandgst[0].Trim();
                            GeneralreceiptDTO.preceiptslist[i].pgstno = Stateandgst[1].Trim();
                        }
                        sbGeneralReceipt.AppendLine("INSERT INTO tbltransgeneralreceiptdetails( detailsid, receiptid, creditaccountid, ledgeramount,gsttype, gstcalculationtype, gstpercentage, igstamount, cgstamount, sgstamount, utgstamount,gstnumber,stateid,statename,isgstapplicable,tdssection,tdspercentage,tdsamount) VALUES (" + GeneralReceiptRecordid + ", '" + ManageQuote(GeneralreceiptDTO.preceiptid) + "', " + GeneralreceiptDTO.preceiptslist[i].psubledgerid + ", " + GeneralreceiptDTO.preceiptslist[i].pamount + ", '" + ManageQuote(GeneralreceiptDTO.preceiptslist[i].pgsttype) + "', '" + ManageQuote(GeneralreceiptDTO.preceiptslist[i].pgstcalculationtype) + "', " + GeneralreceiptDTO.preceiptslist[i].pgstpercentage + ", " + GeneralreceiptDTO.preceiptslist[i].pigstamount + ", " + GeneralreceiptDTO.preceiptslist[i].pcgstamount + ", " + GeneralreceiptDTO.preceiptslist[i].psgstamount + ", " + GeneralreceiptDTO.preceiptslist[i].putgstamount + ",'" + ManageQuote(GeneralreceiptDTO.preceiptslist[i].pgstno) + "'," + GeneralreceiptDTO.preceiptslist[i].pStateId + ",'" + ManageQuote(GeneralreceiptDTO.preceiptslist[i].pState) + "'," + GeneralreceiptDTO.preceiptslist[i].IsGstapplicable + ",'" + ManageQuote(GeneralreceiptDTO.pTdsSection) + "'," + GeneralreceiptDTO.pTdsPercentage + "," + GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual + ");");
                    }
                }
                string particulars = "";
                if (GeneralreceiptDTO.preceiptslist.Count > 0)
                {
                    particulars = GeneralreceiptDTO.preceiptslist[0].pledgername.ToUpper() + "(" + GeneralreceiptDTO.ppartyreferenceid + "_" + GeneralreceiptDTO.ppartyname.ToUpper() + ")";
                }
                if (GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() != "CASH" && !string.IsNullOrEmpty(GeneralreceiptDTO.pmodofreceipt))
                {
                    sbGeneralReceipt.AppendLine("INSERT INTO tbltransreceiptreference( receiptid,bankname,branchname,transtype,typeofpayment,referencenumber,particulars,depositeddate,totalreceivedamount,depositstatus, clearstatus,statusid, createdby, createddate,cardnumber,cleardate,upiid,upiname,chequedate) VALUES ('" + ManageQuote(GeneralreceiptDTO.preceiptid) + "', '" + ManageQuote(GeneralreceiptDTO.pbankname).Trim().ToUpper() + "', '" + ManageQuote(GeneralreceiptDTO.pbranchname).Trim().ToUpper() + "', '" + ManageQuote(GeneralreceiptDTO.ptranstype).Trim().ToUpper() + "', '" + ManageQuote(GeneralreceiptDTO.ptypeofpayment).Trim().ToUpper() + "', '" + ManageQuote(GeneralreceiptDTO.pChequenumber).Trim().ToUpper() + "', '" + ManageQuote(particulars).Trim().ToUpper() + "', " + GeneralreceiptDTO.pchequedate + ", " + GeneralreceiptDTO.ptotalreceivedamount + ",'N', 'N', " + Convert.ToInt32(Status.Active) + "," + GeneralreceiptDTO.pCreatedby + ",current_timestamp,'" + ManageQuote(GeneralreceiptDTO.pCardNumber) + "'," + GeneralreceiptDTO.pchequedate + ",'" + ManageQuote(GeneralreceiptDTO.pUpiid) + "','" + ManageQuote(GeneralreceiptDTO.pUpiname).Trim().ToUpper() + "'," + GeneralreceiptDTO.pchequedate + ");");
                }
                if (GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "CASH")
                {
                    // JV Insert for Main Cash
                    if (GeneralreceiptDTO.pistdsapplicable && GeneralreceiptDTO.ptdsamount > 0)
                    {
                        if (GeneralreceiptDTO.preceiptslist != null)
                        {
                            for (int i = 0; i < GeneralreceiptDTO.preceiptslist.Count; i++)
                            {
                                if (GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual > 0)
                                {
                                    objJournalVoucherDTO = new JournalVoucherDTO();
                                    List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                                    objJournalVoucherDTO.pjvdate = GeneralreceiptDTO.preceiptdate;
                                    objJournalVoucherDTO.pnarration = "BEING JV PASSED TOWARDS TDS AMOUNT";
                                    objJournalVoucherDTO.pmodoftransaction = "AUTO";
                                    objJournalVoucherDTO.pCreatedby = GeneralreceiptDTO.pCreatedby;
                                    objPaymentsDTO = new PaymentsDTO
                                    {
                                        ppartyid = GeneralreceiptDTO.ppartyid,
                                        ppartyname = GeneralreceiptDTO.ppartyname,
                                        ppartyreferenceid = GeneralreceiptDTO.ppartyreferenceid,
                                        ppartyreftype = GeneralreceiptDTO.ppartyreftype,
                                        ptranstype = "D",
                                        psubledgerid = GeneralreceiptDTO.preceiptslist[i].psubledgerid,
                                        pamount = GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual
                                    };
                                    _Paymentslist.Add(objPaymentsDTO);
                                    objPaymentsDTO = new PaymentsDTO
                                    {
                                        ptranstype = "C",
                                        ppartyid = GeneralreceiptDTO.ppartyid,
                                        ppartyname = GeneralreceiptDTO.ppartyname,
                                        ppartyreferenceid = GeneralreceiptDTO.ppartyreferenceid,
                                        ppartyreftype = GeneralreceiptDTO.ppartyreftype
                                    };
                                    long creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='CURRENT ASSETS' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                                    

                                    creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('TDS-" + GeneralreceiptDTO.pTdsSection + " RECEIVABLE'," + creditaccountid + ",'2'," + GeneralreceiptDTO.pCreatedby + ")"));

                                    creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + GeneralreceiptDTO.ppartyreferenceid + "_" + GeneralreceiptDTO.ppartyname.ToUpper() + "'," + creditaccountid + ",'3'," + GeneralreceiptDTO.pCreatedby + ")"));

                                    objPaymentsDTO.psubledgerid = creditaccountid;
                                    objPaymentsDTO.pamount = GeneralreceiptDTO.preceiptslist[i].ptdsamountindividual;
                                    _Paymentslist.Add(objPaymentsDTO);
                                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                                    string refjvnumber = "";
                                    SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                                    sbGeneralReceipt.AppendLine("update tbltransgeneralreceiptdetails set tdsrefjvnumber='" + refjvnumber + "',tdsaccountid=" + creditaccountid + " where receiptid='" + ManageQuote(GeneralreceiptDTO.preceiptid) + "' and  creditaccountid=" + GeneralreceiptDTO.preceiptslist[i].psubledgerid + ";");
                                }
                            }
                        }
                    }
                }
                else if (GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "BANK" || GeneralreceiptDTO.pmodofreceipt.Trim().ToUpper() == "WALLET")
                {
                    if (!string.IsNullOrEmpty(GeneralreceiptDTO.ptranstype) && !string.IsNullOrEmpty(GeneralreceiptDTO.ptypeofpayment))
                    {
                        sbGeneralReceipt.Append(SaveGeneralReceiptTransactions(trans, GeneralreceiptDTO));
                    }
                }
                if (!string.IsNullOrEmpty(GeneralreceiptDTO.preceiptid))
                {
                    sbGeneralReceipt.AppendLine("SELECT fntotaltransactions('" + GeneralreceiptDTO.preceiptid + "','GENERAL RECEIPT');");
                    //  sbGeneralReceipt.AppendLine("select accountsupdate();");
                }
                if (Convert.ToString(sbGeneralReceipt) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbGeneralReceipt.ToString());
                    Savedstatus = true;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            return Savedstatus;
        }
        public string SaveGeneralReceiptTransactions(NpgsqlTransaction trans, GeneralreceiptDTO modelGeneralReceipt)
        {
            StringBuilder SaveChequeTrans = new StringBuilder();
            try
            {
                string Chequesonhandstatus = string.Empty;
                string Chequesinbankstatus = string.Empty;
                string Chequesclearstatus = string.Empty;

                if (modelGeneralReceipt != null)
                {
                    if (!string.IsNullOrEmpty(modelGeneralReceipt.ptypeofpayment))
                    {
                        NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(trans, CommandType.Text, "select chqonhand,chqinbank,chqclear from tblmstModeofTransaction where upper(transmode)='" + ManageQuote(modelGeneralReceipt.pmodofreceipt).Trim().ToUpper() + "' and upper(modeoftype)='" + ManageQuote(modelGeneralReceipt.ptranstype).Trim().ToUpper() + "' and upper(submodeoftype)='" + ManageQuote(modelGeneralReceipt.ptypeofpayment).Trim().ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";", null);
                        while (dr.Read())
                        {
                            if (dr != null)
                            {
                                Chequesonhandstatus = Convert.ToString(dr["chqonhand"]);
                                Chequesinbankstatus = Convert.ToString(dr["chqinbank"]);
                                Chequesclearstatus = Convert.ToString(dr["chqclear"]);
                            }
                        }
                    }
                    if (Chequesonhandstatus == "Y")
                    {
                        SaveChequeTrans.AppendLine(SaveGeneralReceiptTransactions_chequesonhand(modelGeneralReceipt));
                    }
                    else
                    {
                        //if (string.IsNullOrEmpty(modelGeneralReceipt.preceiptdate))
                        //{
                        //    modelGeneralReceipt.preceiptdate = "null";
                        //}
                        //else
                        //{
                        //    modelGeneralReceipt.preceiptdate = "'" + FormatDate(modelGeneralReceipt.preceiptdate) + "'";
                        //}
                        string strTransNo = "";
                        if (Chequesinbankstatus == "Y")
                        {
                            SaveChequeTrans.AppendLine(SaveGeneralReceiptTransactions_chequesinbank(trans, modelGeneralReceipt, "DEPOSIT", out strTransNo));
                        }
                        else if (Chequesclearstatus == "Y")
                        {
                            SaveChequeTrans.AppendLine(SaveGeneralReceiptTransactions_chequesinbank(trans, modelGeneralReceipt, "DEPOSIT", out strTransNo));
                            SaveChequeTrans.AppendLine(SaveGeneralReceiptTransactions_chequesclear(trans, modelGeneralReceipt));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Convert.ToString(SaveChequeTrans);
        }
        public string SaveGeneralReceiptTransactions_chequesinbank(NpgsqlTransaction trans, GeneralreceiptDTO modelGeneralReceipt, string Type, out string strTransNo)
        {
            string maincashType;
            string bankType;
            long Maincashaccountid;
            if (Type.ToUpper() == "DEPOSIT")
            {
                maincashType = "C";
                bankType = "D";
            }
            else
            {
                maincashType = "D";
                bankType = "C";
            }
            StringBuilder Sbbankonlinetrans = new StringBuilder();
            try
            {
                long Bankaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select bankaccountid from tblmstbank where recordid=" + modelGeneralReceipt.pdepositbankid + " and statusid=" + Convert.ToInt32(Status.Active) + " ;"));

                if (Type.ToUpper() == "DEPOSIT")
                {
                    string Chequedate = "null";
                    Sbbankonlinetrans.AppendLine("update tbltransreceiptreference set depositeddate=" + modelGeneralReceipt.pchequedate + ",depositstatus='P',clearstatus='N',cleardate=" + Chequedate + ", depositedbankid=" + modelGeneralReceipt.pdepositbankid + ",depositmodifiedby=" + modelGeneralReceipt.pCreatedby + ",depositmodifieddate=current_timestamp where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "';");
                }
                else
                {
                    Sbbankonlinetrans.AppendLine("update tbltransreceiptreference set cleardate=" + modelGeneralReceipt.pchequedate + ",clearstatus='R',depositmodifiedby=" + modelGeneralReceipt.pCreatedby + ",depositmodifieddate=current_timestamp where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "';");
                }
                // TransactionId Generation
                string Banktransaction_transactionid = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('CHEQUES ON HAND','','" + FormatDate(modelGeneralReceipt.preceiptdate) + "')").ToString();
                strTransNo = Banktransaction_transactionid;
                long Banktransid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltransbankinformation(transactionno,transactiondate,receiptid,referencenumber, statusid, createdby, createddate) VALUES ('" + ManageQuote(Banktransaction_transactionid).Trim() + "', " + modelGeneralReceipt.pchequedate + ",'" + ManageQuote(modelGeneralReceipt.preceiptid) + "','" + ManageQuote(modelGeneralReceipt.pChequenumber) + "'," + Convert.ToInt32(Status.Active) + ", " + modelGeneralReceipt.pCreatedby + ", current_timestamp) returning recordid ;"));

                Sbbankonlinetrans.AppendLine("INSERT INTO tbltransbankinformationdetails (detailsid,transactionid,accountid,ledgeramount,accounttype) VALUES (" + Banktransid + ", '" + Banktransaction_transactionid + "', " + Bankaccountid + ", " + modelGeneralReceipt.ptotalreceivedamount + ", '" + ManageQuote(bankType) + "');");
                if (Type.ToUpper() == "DEPOSIT")
                {
                    Maincashaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='CHEQUE ON HAND' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    Maincashaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='UNCLEARED CHEQUES A/C' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                Sbbankonlinetrans.AppendLine("INSERT INTO tbltransbankinformationdetails (detailsid,transactionid,accountid,ledgeramount,accounttype) VALUES (" + Banktransid + ", '" + Banktransaction_transactionid + "', " + Maincashaccountid + ", " + modelGeneralReceipt.ptotalreceivedamount + ", '" + ManageQuote(maincashType) + "');");

                if (!string.IsNullOrEmpty(Banktransaction_transactionid))
                {
                    Sbbankonlinetrans.AppendLine("SELECT fntotaltransactions('" + Banktransaction_transactionid + "','CHEQUESONHAND');");
                }

                return Convert.ToString(Sbbankonlinetrans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveGeneralReceiptTransactions_chequesclear(NpgsqlTransaction trans, GeneralreceiptDTO modelGeneralReceipt)
        {
            StringBuilder Sbbankonlinetrans = new StringBuilder();
            try
            {
                long Bankaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select bankaccountid from tblmstbank where recordid=" + modelGeneralReceipt.pdepositbankid + " and statusid=" + Convert.ToInt32(Status.Active) + " ;"));

                Sbbankonlinetrans.AppendLine("update tbltransreceiptreference set depositstatus='P',clearstatus='Y',cleardate=" + modelGeneralReceipt.pchequedate + ", depositmodifiedby=" + modelGeneralReceipt.pCreatedby + ",depositmodifieddate=current_timestamp,clearedmodifiedby=" + modelGeneralReceipt.pCreatedby + ",clearedmodifieddate=current_timestamp where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "';");

                // Gen cheques Clear Transaction Id Generation   
                if (modelGeneralReceipt.preceiptid.StartsWith("CHQ"))
                {
                    string Genchequescleared_transactionno = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('CHEQUES IN BANK','','" + FormatDate(modelGeneralReceipt.preceiptdate) + "')").ToString();

                    long Genchequestransid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tbltransgenchequecleared(transactionno, transactiondate, receiptid, referencenumber, contactid, contactname, contactrefid, contactreftype, totalreceivedamount, tdssection, pannumber, tdscalculationtype, tdspercentage, tdsamount, tdsaccountid, statusid, createdby, createddate )  select  '" + Genchequescleared_transactionno + "', " + modelGeneralReceipt.pchequedate + ", receiptid, '" + ManageQuote(modelGeneralReceipt.pChequenumber) + "', contactid, contactname, contactrefid, contactreftype, totalreceivedamount, tdssection, pannumber, tdscalculationtype, tdspercentage, tdsamount, tdsaccountid, " + Convert.ToInt32(Status.Active) + ", " + modelGeneralReceipt.pCreatedby + ", current_timestamp from tbltransgeneralreceipt where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "' returning recordid;"));

                    Sbbankonlinetrans.AppendLine("insert into tbltransgenchequecleareddetails( detailsid, transactionno, creditaccountid, ledgeramount, isgstapplicable, gsttype, gstcalculationtype, gstpercentage, gstnumber, stateid, statename, cgstaccountid, sgstaccountid, igstaccountid, utgstaccountid, igstamount, cgstamount, sgstamount, utgstamount,tdsaccountid, tdssection, tdspercentage, tdsamount) select  " + Genchequestransid + ", '" + Genchequescleared_transactionno + "', creditaccountid, ledgeramount, isgstapplicable, gsttype, gstcalculationtype, gstpercentage, gstnumber, stateid, statename, cgstaccountid, sgstaccountid, igstaccountid, utgstaccountid, igstamount, cgstamount, sgstamount, utgstamount, tdsaccountid, tdssection, tdspercentage, tdsamount from tbltransgeneralreceiptdetails where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "' ;");

                    if (!string.IsNullOrEmpty(Genchequescleared_transactionno))
                    {
                        Sbbankonlinetrans.AppendLine("SELECT fntotaltransactions('" + Genchequescleared_transactionno + "','CHEQUESINBANK');");
                    }

                    // JV Insert
                    if (modelGeneralReceipt.pistdsapplicable && modelGeneralReceipt.ptdsamount > 0)
                    {
                        if (modelGeneralReceipt.preceiptslist != null)
                        {
                            for (int i = 0; i < modelGeneralReceipt.preceiptslist.Count; i++)
                            {
                                if (modelGeneralReceipt.preceiptslist[i].ptdsamountindividual > 0)
                                {
                                    objJournalVoucherDTO = new JournalVoucherDTO();
                                    List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                                    objJournalVoucherDTO.pjvdate = modelGeneralReceipt.preceiptdate;
                                    objJournalVoucherDTO.pCreatedby = modelGeneralReceipt.pCreatedby;
                                    objJournalVoucherDTO.pnarration = "BE JV PASSED TOWARDS TDS AMOUNT";
                                    objJournalVoucherDTO.pmodoftransaction = "AUTO";
                                    objPaymentsDTO = new PaymentsDTO
                                    {
                                        ppartyid = modelGeneralReceipt.ppartyid,
                                        ppartyname = modelGeneralReceipt.ppartyname,
                                        ppartyreferenceid = modelGeneralReceipt.ppartyreferenceid,
                                        ppartyreftype = modelGeneralReceipt.ppartyreftype,
                                        ptranstype = "D",
                                        psubledgerid = modelGeneralReceipt.preceiptslist[i].psubledgerid,
                                        pamount = modelGeneralReceipt.preceiptslist[i].ptdsamountindividual
                                    };
                                    _Paymentslist.Add(objPaymentsDTO);
                                    objPaymentsDTO = new PaymentsDTO
                                    {
                                        ptranstype = "C",
                                        ppartyid = modelGeneralReceipt.ppartyid,
                                        ppartyname = modelGeneralReceipt.ppartyname,
                                        ppartyreferenceid = modelGeneralReceipt.ppartyreferenceid,
                                        ppartyreftype = modelGeneralReceipt.ppartyreftype
                                    };

                                    long creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='OTHER CURRENT LIABILITIES' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                                    creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('TDS-" + modelGeneralReceipt.preceiptslist[i].ptdssection + " PAYABLE'," + creditaccountid + ",'2'," + modelGeneralReceipt.pCreatedby + ")"));

                                    creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + modelGeneralReceipt.ppartyreferenceid + "_" + modelGeneralReceipt.ppartyname + "'," + creditaccountid + ",'3'," + modelGeneralReceipt.pCreatedby + ")"));

                                    objPaymentsDTO.psubledgerid = creditaccountid;
                                    objPaymentsDTO.pamount = modelGeneralReceipt.preceiptslist[i].ptdsamountindividual;
                                    _Paymentslist.Add(objPaymentsDTO);
                                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                                    string refjvnumber = "";
                                    SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);

                                    Sbbankonlinetrans.AppendLine("update tbltransgeneralreceiptdetails set tdsrefjvnumber='" + refjvnumber + "',tdsaccountid=" + creditaccountid + " where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "'" +
                                        "and creditaccountid=" + modelGeneralReceipt.preceiptslist[i].psubledgerid + ";");

                                    Sbbankonlinetrans.AppendLine("update tbltransgenchequecleareddetails set tdsaccountid=" + creditaccountid + " where transactionno='" + ManageQuote(Genchequescleared_transactionno) + "' and creditaccountid=" + modelGeneralReceipt.preceiptslist[i].psubledgerid + ";");
                                }
                            }
                        }
                    }
                }
                return Convert.ToString(Sbbankonlinetrans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveGeneralReceiptTransactions_chequesonhand(GeneralreceiptDTO modelGeneralReceipt)
        {
            StringBuilder Sbchequesonhand = new StringBuilder();
            try
            {
                modelGeneralReceipt.pchequedate = "null";
                Sbchequesonhand.AppendLine("update tbltransreceiptreference set depositstatus='N',clearstatus='N',depositeddate= " + modelGeneralReceipt.pchequedate + ", cleardate=" + modelGeneralReceipt.pchequedate + " where receiptid='" + ManageQuote(modelGeneralReceipt.preceiptid) + "';");
            }
            catch (Exception)
            {
                throw;
            }
            return Convert.ToString(Sbchequesonhand);
        }
        public async Task<List<GeneralreceiptDTO>> GetReceiptsData(string ConnectionString)
        {
            await Task.Run(() =>
            {
                pGeneralReceiptList = new List<GeneralreceiptDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1. receiptdate,t1.receiptid,modeofreceipt,bankname,referencenumber,coalesce( t1.totalreceivedamount ,0) totalreceivedamount ,narration,upper(contactname) contactname,contactrefid,istdsapplicable,tdssection,pannumber,tdscalculationtype,coalesce(tdspercentage,0) tdspercentage,t2.typeofpayment,t1.filename,t1.fileformat,t1.filepath,t2.cleardate,t2.chequedate,t2.depositeddate from tbltransgeneralreceipt t1 left join tbltransreceiptreference t2 on t1.receiptid=t2.receiptid where t1.receiptdate=current_date order by receiptdate,receiptid desc;"))
                    {
                        while (dr.Read())
                        {
                            GeneralreceiptDTO _GeneralReceipt = new GeneralreceiptDTO
                            {
                                preceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy"),
                                preceiptid = Convert.ToString(dr["receiptid"]),
                                pmodofreceipt = Convert.ToString(dr["modeofreceipt"]),
                                ptotalreceivedamount = Convert.ToDecimal(dr["totalreceivedamount"]),
                                pbankname = Convert.ToString(dr["bankname"]),
                                pChequenumber = Convert.ToString(dr["referencenumber"]),
                                pnarration = Convert.ToString(dr["narration"]),
                                ppartyname = Convert.ToString(dr["contactname"]),
                                ppartyreferenceid = Convert.ToString(dr["contactrefid"]),
                                pTdsSection = Convert.ToString(dr["tdssection"]),
                                ppartypannumber = Convert.ToString(dr["pannumber"]),
                                ptdscalculationtype = Convert.ToString(dr["tdscalculationtype"]),
                                pTdsPercentage = Convert.ToDecimal(dr["tdspercentage"]),
                                pistdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]),
                                ptypeofpayment = Convert.ToString(dr["typeofpayment"]),
                                pFilename = Convert.ToString(dr["filename"]),
                                pFileformat = Convert.ToString(dr["fileformat"]),
                                pFilepath = Convert.ToString(dr["filepath"]),
                                pCleardate = Convert.ToString(dr["cleardate"]),
                                pchequedate = Convert.ToString(dr["chequedate"]),
                                pDepositeddate = Convert.ToString(dr["depositeddate"])
                            };
                            pGeneralReceiptList.Add(_GeneralReceipt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return pGeneralReceiptList;
        }
        public async Task<List<GeneralReceiptReportDTO>> GetgeneralreceiptReportData(string ReceiptId, string Connectionstring)
        {
            await Task.Run(() =>
            {
                pGeneralReceiptReportDataList = new List<GeneralReceiptReportDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select tr.receiptid,receiptdate,creditaccountid,ledgeramount,upper(gstnumber) GSTNO,contactname ,narration,accountname,trim(substring(cash_words(ledgeramount::money), 1, position('dollars' in cash_words(ledgeramount::money))-1)) as amountwords from tbltransgeneralreceipt tr join tbltransgeneralreceiptdetails  trd on tr.receiptid=trd.receiptid  left join tblmstaccounts tac on tac.accountid=trd.creditaccountid where upper(TR.receiptid)='" + ManageQuote(ReceiptId).Trim().ToUpper() + "';"))
                    {
                        while (dr.Read())
                        {
                            GeneralReceiptReportDTO _GeneralReceipt = new GeneralReceiptReportDTO
                            {
                                pReceiptdate = Convert.ToString(dr["receiptdate"]),
                                pReceiptId = Convert.ToString(dr["receiptid"]),
                                pCrdAccountId = Convert.ToInt64(dr["creditaccountid"]),
                                pLedgeramount = Convert.ToDecimal(dr["ledgeramount"]),
                                pGstno = Convert.ToString(dr["GSTNO"]),
                                pContactname = Convert.ToString(dr["contactname"]),
                                pNarration = Convert.ToString(dr["narration"]),
                                pAccountname = Convert.ToString(dr["accountname"]),
                                pAmountinWords = Convert.ToString(dr["amountwords"])
                            };
                            pGeneralReceiptReportDataList.Add(_GeneralReceipt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return pGeneralReceiptReportDataList;
        }
        public async Task<GeneralReceiptReportDTO> GetgeneralreceiptReportData1(string ReceiptId, string Connectionstring)
        {
            await Task.Run(() =>
            {
                try
                {
                    ReceiptId = ManageQuote(ReceiptId).Trim().ToUpper();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct tr.receiptid,receiptdate,coalesce(upper(gstnumber),'') as gstno,coalesce(upper(contactname),'') as contactname ,coalesce(narration,'') as narration,modeofreceipt,coalesce(referencenumber,'')as referencenumber,trr.typeofpayment,coalesce(tu.employeename,'')as employeename,cleardate,chequedate,depositeddate from tbltransgeneralreceipt tr join tbltransgeneralreceiptdetails  trd on tr.receiptid=trd.receiptid left join tbltransreceiptreference trr on trr.receiptid=tr.receiptid left join tblmstusers tu on tr.createdby=tu.userid where upper(TR.receiptid)='" + ReceiptId + "';"))
                    {
                        while (dr.Read())
                        {
                            pGeneralReceiptData = new GeneralReceiptReportDTO
                            {
                                pReceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd-MMM-yyyy"),
                                pReceiptId = Convert.ToString(dr["receiptid"]),
                                pGstno = Convert.ToString(dr["gstno"]),
                                pContactname = Convert.ToString(dr["contactname"]),
                                pNarration = Convert.ToString(dr["narration"]),
                                pModeofreceipt = Convert.ToString(dr["modeofreceipt"]),
                                pReferenceorChequeNo = Convert.ToString(dr["referencenumber"]),
                                pTypeofpayment = Convert.ToString(dr["typeofpayment"]),
                                pPostedby = Convert.ToString(dr["employeename"]),
                                pGeneralReceiptSubDetailsList = GetGeneralreceiptParticulars(ReceiptId, Connectionstring),
                                pCleardate = Convert.ToString(dr["cleardate"]),
                                pChequedate = dr["chequedate"] != DBNull.Value ? Convert.ToDateTime(dr["chequedate"]).ToString("dd-MMM-yyyy") : Convert.ToString(dr["chequedate"]),
                                pDepositeddate = Convert.ToString(dr["depositeddate"])
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return pGeneralReceiptData;
        }
        public List<GeneralReceiptSubDetails> GetGeneralreceiptParticulars(string ReceiptId, string Connectionstring)
        {
            var pGeneralReceiptSubDetailsList = new List<GeneralReceiptSubDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "SELECT accountname,SUM(ledgeramount)ledgeramount,coalesce(SUM(cgstamount+sgstamount+igstamount),0) as gstamount ,coalesce(SUM(tdsamount),0) tdsamount FROM (select case when chracctype = '2' then accountname else parentaccountname end accountname ,coalesce(ledgeramount, 0) +coalesce(igstamount,0)+coalesce(cgstamount,0)+coalesce(sgstamount,0)+coalesce(utgstamount,0)+coalesce(tdsamount,0) ledgeramount,cgstamount,sgstamount,igstamount,tdsamount from tbltransgeneralreceiptdetails tr join tblmstaccounts tac on  tac.accountid = tr.creditaccountid where receiptid = '" + ReceiptId + "') X GROUP BY accountname; "))
                {
                    while (dr.Read())
                    {
                        var _GeneralReceipt = new GeneralReceiptSubDetails
                        {
                            pLedgeramount = Convert.ToDecimal(dr["ledgeramount"]),
                            pAccountname = Convert.ToString(dr["accountname"]),
                            pcgstamount = Convert.ToDecimal(dr["gstamount"]),
                            //psgstamount = Convert.ToDecimal(dr["sgstamount"]),
                            //pigstamount = Convert.ToDecimal(dr["igstamount"]),
                            ptdsamount = Convert.ToDecimal(dr["tdsamount"])
                        };
                        pGeneralReceiptSubDetailsList.Add(_GeneralReceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pGeneralReceiptSubDetailsList;
        }
    }
}
