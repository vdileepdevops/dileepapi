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


        public bool SavePaymentVoucher(PaymentVoucherDTO _PaymentVoucherDTO, string ConnectionString, out string _PaymentId)

        {
            bool IsSaved = false;
            _PaymentId = string.Empty;
            StringBuilder sbQuery = new StringBuilder();

            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (SavePaymentVoucher_ALL(_PaymentVoucherDTO, trans, "SAVE", out _PaymentId))
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
        public bool SavePaymentVoucher_ALL(PaymentVoucherDTO _PaymentVoucherDTO, NpgsqlTransaction trans, string TransType, out string _PaymentId)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {

                if (string.IsNullOrEmpty(_PaymentVoucherDTO.ppaymentid))
                {

                    _PaymentVoucherDTO.ppaymentid = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('PAYMENT VOUCHER','CASH','" + FormatDate(_PaymentVoucherDTO.ppaymentdate) + "')"));
                }
                long creditaccountid = 0;
                long tdsaccountid = 0;
                string query = "";
                long detailsid = 0;
                if (_PaymentVoucherDTO.pmodofPayment == "CASH")
                {
                    if (TransType == "SAVE")
                        creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='CASH ON HAND' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                    if (TransType == "CANCEL")
                        creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='CHEQUE ON HAND' and statusid=" + Convert.ToInt32(Status.Active) + ";"));


                }
                else
                {
                    creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select bankaccountid from tblmstbank  where recordid = " + _PaymentVoucherDTO.pbankid));
                }
                if (string.IsNullOrEmpty(_PaymentVoucherDTO.ptypeofoperation))
                {
                    _PaymentVoucherDTO.ptypeofoperation = "CREATE";
                }
                if (_PaymentVoucherDTO.ptypeofoperation.ToUpper() == "CREATE")
                {
                    query = "insert into tbltranspaymentvoucher( paymentid, paymentdate, modeofpayment, totalpaidamount, narration, creditaccountid, statusid, createdby, createddate,filename,filepath,fileformat)values('" + _PaymentVoucherDTO.ppaymentid + "', '" + FormatDate(_PaymentVoucherDTO.ppaymentdate) + "', '" + ManageQuote(_PaymentVoucherDTO.pmodofPayment) + "', " + _PaymentVoucherDTO.ptotalpaidamount + ", '" + ManageQuote(_PaymentVoucherDTO.pnarration) + "', " + creditaccountid + ", " + Convert.ToInt32(Status.Active) + ", " + _PaymentVoucherDTO.pCreatedby + ", current_timestamp,'" + ManageQuote(_PaymentVoucherDTO.pFilename) + "','" + ManageQuote(_PaymentVoucherDTO.pFilepath) + "','" + ManageQuote(_PaymentVoucherDTO.pFileformat) + "') returning recordid";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                }

                if (_PaymentVoucherDTO.ppaymentslist != null)
                {
                    for (int i = 0; i < _PaymentVoucherDTO.ppaymentslist.Count; i++)
                    {
                        if (string.IsNullOrEmpty(_PaymentVoucherDTO.ppaymentslist[i].ptypeofoperation))
                        {
                            _PaymentVoucherDTO.ppaymentslist[i].ptypeofoperation = "CREATE";
                        }

                        if (_PaymentVoucherDTO.ppaymentslist[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            if (!string.IsNullOrEmpty(_PaymentVoucherDTO.ppaymentslist[i].pState))
                            {
                                if (_PaymentVoucherDTO.ppaymentslist[i].pState.Contains('-'))
                                {
                                    string[] details = _PaymentVoucherDTO.ppaymentslist[i].pState.Split('-');
                                    _PaymentVoucherDTO.ppaymentslist[i].pState = details[0].Trim();
                                    _PaymentVoucherDTO.ppaymentslist[i].pgstno = details[1].Trim();
                                }
                            }
                            if (_PaymentVoucherDTO.ppaymentslist[i].ptdsamount > 0)
                            {
                                objJournalVoucherDTO = new JournalVoucherDTO();
                                List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                                objJournalVoucherDTO.pjvdate = _PaymentVoucherDTO.ppaymentdate;
                                objJournalVoucherDTO.pnarration = "BEING JV PASSED TOWARDS TDS AMOUNT";
                                objJournalVoucherDTO.pmodoftransaction = "AUTO";
                                objJournalVoucherDTO.pCreatedby = _PaymentVoucherDTO.pCreatedby;
                                objPaymentsDTO = new PaymentsDTO();

                                objPaymentsDTO.ppartyid = _PaymentVoucherDTO.ppaymentslist[i].ppartyid;
                                objPaymentsDTO.ppartyname = _PaymentVoucherDTO.ppaymentslist[i].ppartyname;
                                objPaymentsDTO.ppartyreferenceid = _PaymentVoucherDTO.ppaymentslist[i].ppartyreferenceid;
                                objPaymentsDTO.ppartyreftype = _PaymentVoucherDTO.ppaymentslist[i].ppartyreftype;


                                objPaymentsDTO.ptranstype = "C";
                                objPaymentsDTO.psubledgerid = _PaymentVoucherDTO.ppaymentslist[i].psubledgerid;
                                objPaymentsDTO.pamount = _PaymentVoucherDTO.ppaymentslist[i].ptdsamount;
                                _Paymentslist.Add(objPaymentsDTO);

                                objPaymentsDTO = new PaymentsDTO();
                                objPaymentsDTO.ptranstype = "D";

                                objPaymentsDTO.ppartyid = _PaymentVoucherDTO.ppaymentslist[i].ppartyid;
                                objPaymentsDTO.ppartyname = _PaymentVoucherDTO.ppaymentslist[i].ppartyname;
                                objPaymentsDTO.ppartyreferenceid = _PaymentVoucherDTO.ppaymentslist[i].ppartyreferenceid;
                                objPaymentsDTO.ppartyreftype = _PaymentVoucherDTO.ppaymentslist[i].ppartyreftype;

                                 creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='OTHER CURRENT LIABILITIES' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                                creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('TDS-" + _PaymentVoucherDTO.ppaymentslist[i].pTdsSection + " PAYABLE'," + creditaccountid + ",'2'," + _PaymentVoucherDTO.pCreatedby + ")"));

                                creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + _PaymentVoucherDTO.ppaymentslist[0].ppartyreferenceid + "_" + _PaymentVoucherDTO.ppaymentslist[0].ppartyname.ToUpper() + "'," + creditaccountid + ",'3'," + _PaymentVoucherDTO.pCreatedby + ")"));

                                objPaymentsDTO.psubledgerid = creditaccountid;
                                objPaymentsDTO.pamount = _PaymentVoucherDTO.ppaymentslist[i].ptdsamount;
                                _Paymentslist.Add(objPaymentsDTO);


                                objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                                string refjvnumber = "";
                                //objJournalVoucherDTO.pStatusid = Convert.ToInt32(Status.Active);
                                SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                                _PaymentVoucherDTO.ppaymentslist[i].ptdsrefjvnumber = refjvnumber;
                                tdsaccountid = creditaccountid;
                            }


                            sbQuery.Append("insert into tbltranspaymentvoucherdetails( detailsid, paymentid, contactid,contactname,contactrefid,contactreftype, debitaccountid, ledgeramount, isgstapplicable,gsttype,gstcalculationtype, gstpercentage, igstamount, cgstamount, sgstamount, utgstamount, tdssection,  tdscalculationtype, tdspercentage, tdsamount, istdsapplicable,pannumber,gstnumber,stateid,statename,tdsrefjvnumber,tdsaccountid)values (" + detailsid + ", '" + _PaymentVoucherDTO.ppaymentid + "'," + _PaymentVoucherDTO.ppaymentslist[i].ppartyid + ", '" + _PaymentVoucherDTO.ppaymentslist[i].ppartyname + "', '" + _PaymentVoucherDTO.ppaymentslist[i].ppartyreferenceid + "', '" + _PaymentVoucherDTO.ppaymentslist[i].ppartyreftype + "', " + _PaymentVoucherDTO.ppaymentslist[i].psubledgerid + ", " + _PaymentVoucherDTO.ppaymentslist[i].pamount + ", '" +
                                _PaymentVoucherDTO.ppaymentslist[i].pisgstapplicable + "', '" + _PaymentVoucherDTO.ppaymentslist[i].pgsttype + "', '" + _PaymentVoucherDTO.ppaymentslist[i].pgstcalculationtype + "', " + _PaymentVoucherDTO.ppaymentslist[i].pgstpercentage + ", " + _PaymentVoucherDTO.ppaymentslist[i].pigstamount + ", " + _PaymentVoucherDTO.ppaymentslist[i].pcgstamount + ", " + _PaymentVoucherDTO.ppaymentslist[i].psgstamount + ", " + _PaymentVoucherDTO.ppaymentslist[i].putgstamount + ", '" + _PaymentVoucherDTO.ppaymentslist[i].pTdsSection + "', '" + _PaymentVoucherDTO.ppaymentslist[i].ptdscalculationtype + "', " + _PaymentVoucherDTO.ppaymentslist[i].pTdsPercentage + ", " + _PaymentVoucherDTO.ppaymentslist[i].ptdsamount + ", " + _PaymentVoucherDTO.ppaymentslist[i].pistdsapplicable + ", '" + _PaymentVoucherDTO.ppaymentslist[i].ppartypannumber + "','" + ManageQuote(_PaymentVoucherDTO.ppaymentslist[i].pgstno) + "'," + _PaymentVoucherDTO.ppaymentslist[i].pStateId + ",'" + ManageQuote(_PaymentVoucherDTO.ppaymentslist[i].pState) + "','" + ManageQuote(_PaymentVoucherDTO.ppaymentslist[i].ptdsrefjvnumber) + "'," + tdsaccountid + ");");



                        }
                    }
                }

                if (_PaymentVoucherDTO.pmodofPayment != "CASH")
                {
                    string particulars = "";

                    if (_PaymentVoucherDTO.ppaymentslist.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(_PaymentVoucherDTO.ppaymentslist[0].pledgername))
                        {
                            particulars = _PaymentVoucherDTO.ppaymentslist[0].pledgername.ToUpper() + "(" + _PaymentVoucherDTO.ppaymentslist[0].ppartyreferenceid + "_" + _PaymentVoucherDTO.ppaymentslist[0].ppartyname.ToUpper() + ")";
                        }
                        else
                        {
                            particulars = "";
                        }
                    }
                    if (_PaymentVoucherDTO.ppaymentslist.Count > 1)
                    {
                        particulars = particulars + "AND OTHER";
                    }

                    if (string.IsNullOrEmpty(_PaymentVoucherDTO.ptypeofpayment))
                    {
                        _PaymentVoucherDTO.ptypeofpayment = _PaymentVoucherDTO.ptranstype;
                    }
                    if (_PaymentVoucherDTO.pbankname.Contains('-'))
                    {

                        _PaymentVoucherDTO.pbankname = _PaymentVoucherDTO.pbankname.Split('-')[0].Trim();
                    }
                    sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _PaymentVoucherDTO.ppaymentid + "', '" + _PaymentVoucherDTO.pbankname + "', '" + _PaymentVoucherDTO.pbranchname + "', '" + _PaymentVoucherDTO.ptranstype + "', '" + _PaymentVoucherDTO.ptypeofpayment + "', " + _PaymentVoucherDTO.pbankid + ", '" + _PaymentVoucherDTO.pChequenumber + "', '" + _PaymentVoucherDTO.pCardNumber + "', '" + _PaymentVoucherDTO.pUpiid + "', '" + _PaymentVoucherDTO.pUpiname + "', '" + FormatDate(_PaymentVoucherDTO.ppaymentdate) + "', " + _PaymentVoucherDTO.ptotalpaidamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + ", " + _PaymentVoucherDTO.pCreatedby + ", current_timestamp);");
                    if (_PaymentVoucherDTO.ptypeofpayment == "CHEQUE")
                    {
                        sbQuery.Append("update  tblmstcheques set   statusid =(SELECT  statusid from tblmststatus  where upper(statusname)  ='USED-CHEQUES') where bankid =" + _PaymentVoucherDTO.pbankid + " and chequenumber=" + _PaymentVoucherDTO.pChequenumber + ";");
                    }
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {


                    sbQuery.Append("select fntotaltransactions('" + _PaymentVoucherDTO.ppaymentid + "','PAYMENT VOUCHER');");
                    //+"select accountsupdate();"
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                    IsSaved = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            _PaymentId = _PaymentVoucherDTO.ppaymentid;
            return IsSaved;

        }
        public async Task<List<PaymentVoucherDTO>> GetPaymentVoucherExistingData(string ConnectionString)
        {
            await Task.Run(() =>
            {
                ppaymentslist = new List<PaymentVoucherDTO>();
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select to_char(t1.paymentdate,'DD-MM-YYYY') paymentdate , t1.paymentid  ,modeofpayment, bankname, chequenumber ,totalpaidamount  from tbltranspaymentvoucher t1 left join  tbltranspaymentreference t2   on t1.paymentid=t2.paymentid where t1.paymentdate =current_date order by t1.createddate desc ;"))

                    {
                        while (dr.Read())
                        {
                            PaymentVoucherDTO _PaymentsDTO = new PaymentVoucherDTO
                            {
                                ppaymentdate = Convert.ToString(dr["paymentdate"]),
                                ppaymentid = Convert.ToString(dr["paymentid"]),
                                pmodofPayment = Convert.ToString(dr["modeofpayment"]),
                                ptotalpaidamount = Convert.ToDecimal(dr["totalpaidamount"]),
                                pbankname = Convert.ToString(dr["bankname"]),
                                pChequenumber = Convert.ToString(dr["chequenumber"]),
                            };
                            ppaymentslist.Add(_PaymentsDTO);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return ppaymentslist;
        }

        public async Task<PaymentVoucherDTO> GetPaymentVoucherReportDataById(string paymentId, string Connectionstring)
        {
            await Task.Run(() =>
            {
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct tr.paymentid,tr.paymentdate,narration,modeofpayment,chequenumber,trr.typeofpayment from tbltranspaymentvoucher tr join tbltranspaymentvoucherdetails  trd on tr.paymentid=trd.paymentid left join tbltranspaymentreference trr on trr.paymentid=tr.paymentid where tr.paymentid='" + paymentId + "';"))
                    {
                        while (dr.Read())
                        {
                            pPaymentVoucherDTO = new PaymentVoucherDTO
                            {
                                ppaymentdate = Convert.ToString(dr["paymentdate"]),
                                ppaymentid = Convert.ToString(dr["paymentid"]),


                                pnarration = Convert.ToString(dr["narration"]),
                                pmodofPayment = Convert.ToString(dr["modeofpayment"]),
                                pChequenumber = Convert.ToString(dr["chequenumber"]),
                                ptypeofpayment = Convert.ToString(dr["typeofpayment"]),
                                ppaymentslist = GetPaymentVoucherDetailsReportDataById(paymentId, Connectionstring)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return pPaymentVoucherDTO;
        }
        public List<PaymentsDTO> GetPaymentVoucherDetailsReportDataById(string paymentId, string Connectionstring)
        {
            Paymentsdetailslist = new List<PaymentsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "SELECT contactname,accountname,SUM(ledgeramount)ledgeramount FROM (select contactname,case when chracctype ='2' then accountname else parentaccountname end accountname,(coalesce(ledgeramount,0) ) ledgeramount from tbltranspaymentvoucherdetails tr join tblmstaccounts tac on  tac.accountid=tr.debitaccountid  where  paymentid='" + paymentId + "')X GROUP BY contactname,accountname ORDER BY contactname"))
                {
                    while (dr.Read())
                    {
                        objPaymentsDTO = new PaymentsDTO
                        {
                            ppartyname = Convert.ToString(dr["contactname"]),
                            pamount = Convert.ToDecimal(dr["ledgeramount"]),
                            pledgername = Convert.ToString(dr["accountname"])
                        };
                        Paymentsdetailslist.Add(objPaymentsDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Paymentsdetailslist;
        }

        public async Task<List<PaymentVoucherReportDTO>> GetPaymentVoucherReportData(string paymentId, string Connectionstring)
        {
            List<PaymentVoucherReportDTO> PaymentVoucherReportlist = new List<PaymentVoucherReportDTO>();
            await Task.Run(() =>
            {

                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct tr.paymentid,tr.paymentdate,narration,contactid,contactname,modeofpayment,chequenumber,coalesce(trr.typeofpayment,'CASH')typeofpayment,transtype,employeename from tbltranspaymentvoucher tr join tbltranspaymentvoucherdetails  trd on tr.paymentid=trd.paymentid left join tbltranspaymentreference trr on trr.paymentid=tr.paymentid join tblmstusers t1 on tr.createdby=t1.userid where tr.paymentid='" + paymentId + "';"))
                    {
                        while (dr.Read())
                        {
                            pPaymentVoucherReportDTO = new PaymentVoucherReportDTO
                            {
                                ppaymentdate = Convert.ToDateTime(dr["paymentdate"]).ToString("dd-MMM-yyyy"),
                                ppaymentid = Convert.ToString(dr["paymentid"]),

                                pcontactid = Convert.ToString(dr["contactid"]),
                                pcontactname = Convert.ToString(dr["contactname"]),

                                pnarration = Convert.ToString(dr["narration"]),
                                pmodofPayment = Convert.ToString(dr["modeofpayment"]),
                                pChequenumber = Convert.ToString(dr["chequenumber"]),
                                ptypeofpayment = Convert.ToString(dr["transtype"]),
                                pemployeename = Convert.ToString(dr["employeename"]),
                                ppaymentslist = GetPaymentVoucherDetailsReportData(paymentId, Convert.ToString(dr["contactname"]), Connectionstring)
                            };
                            PaymentVoucherReportlist.Add(pPaymentVoucherReportDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return PaymentVoucherReportlist;
        }
        public List<GeneralReceiptSubDetails> GetPaymentVoucherDetailsReportData(string paymentId, string contactname, string Connectionstring)
        {
            GeneralReceiptlist = new List<GeneralReceiptSubDetails>();
            try
            {
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "SELECT contactname,accountname,SUM(ledgeramount)ledgeramount FROM (select contactname,case when chracctype ='2' then accountname else parentaccountname end accountname,(coalesce(ledgeramount,0) ) ledgeramount from tbltranspaymentvoucherdetails tr join tblmstaccounts tac on  tac.accountid=tr.debitaccountid  where  paymentid='" + paymentId + "' and upper(contactname)='" + contactname.ToUpper() + "')X GROUP BY contactname,accountname ORDER BY contactname"))
                string Query = string.Empty ;
                if (string.IsNullOrEmpty(contactname))
                {
                    Query = "SELECT contactname,accountname,SUM(ledgeramount)ledgeramount,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount FROM (select contactname,case when chracctype ='2' then accountname else parentaccountname end accountname,(coalesce(ledgeramount,0) ) ledgeramount,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount from tbltranspaymentvoucherdetails tr join tblmstaccounts tac on  tac.accountid=tr.debitaccountid  where  paymentid='" + paymentId + "')X GROUP BY contactname,accountname,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount ORDER BY contactname";
                }
                else
                {
                    Query = "SELECT contactname,accountname,SUM(ledgeramount)ledgeramount,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount FROM (select contactname,case when chracctype ='2' then accountname else parentaccountname end accountname,(coalesce(ledgeramount,0) ) ledgeramount,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount from tbltranspaymentvoucherdetails tr join tblmstaccounts tac on  tac.accountid=tr.debitaccountid  where  paymentid='" + paymentId + "' and upper(contactname)='" + contactname.ToUpper() + "')X GROUP BY contactname,accountname,gstcalculationtype,cgstamount,sgstamount,igstamount,tdscalculationtype,tdsamount ORDER BY contactname";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text,Query))
                {
                    while (dr.Read())
                    {
                        _GeneralReceipt = new GeneralReceiptSubDetails();
                        _GeneralReceipt.pLedgeramount = Convert.ToDecimal(dr["ledgeramount"]);
                        _GeneralReceipt.pAccountname = Convert.ToString(dr["accountname"]);
                        _GeneralReceipt.pgstcalculationtype = Convert.ToString(dr["gstcalculationtype"]);
                        if (!string.IsNullOrEmpty(dr["cgstamount"].ToString()))
                        {
                            _GeneralReceipt.pcgstamount = Convert.ToDecimal(dr["cgstamount"]);
                        }
                        if (!string.IsNullOrEmpty(dr["sgstamount"].ToString()))
                        {
                            _GeneralReceipt.psgstamount = Convert.ToDecimal(dr["sgstamount"]);
                        }
                        if (!string.IsNullOrEmpty(dr["igstamount"].ToString()))
                        {
                            _GeneralReceipt.pigstamount = Convert.ToDecimal(dr["igstamount"]);
                        }
                        _GeneralReceipt.ptdscalculationtype = Convert.ToString(dr["tdscalculationtype"]);
                        if (!string.IsNullOrEmpty(dr["tdsamount"].ToString()))
                        {
                            _GeneralReceipt.ptdsamount = Convert.ToDecimal(dr["tdsamount"]);
                        }
                        GeneralReceiptlist.Add(_GeneralReceipt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GeneralReceiptlist;
        }
    }
}
