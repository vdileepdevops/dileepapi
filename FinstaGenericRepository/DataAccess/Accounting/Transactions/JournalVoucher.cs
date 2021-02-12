using FinstaRepository.Interfaces.Accounting.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Accounting;
using Npgsql;
using HelperManager;
using System.Data;
using System.Threading.Tasks;
namespace FinstaRepository.DataAccess.Accounting.Transactions
{
    public partial class AccountingTransactionsDAL : IAccountingTransactions
    {
        public List<JournalVoucherReportDTO> lstJournalVoucherReport { get; private set; }
        public bool SaveJournalVoucherNew(JournalVoucherNewDTO _JournalVoucherDTO, NpgsqlTransaction trans, out string ptdsrefjvnumber)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(_JournalVoucherDTO.pjvnumber))
                {
                    _JournalVoucherDTO.pjvnumber = Convert.ToString(Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('JOURNAL VOUCHER','AUTO','" + FormatDate(_JournalVoucherDTO.pjvdate) + "')")));
                }

                ptdsrefjvnumber = _JournalVoucherDTO.pjvnumber;
                string query = "";
                long detailsid = 0;
                if (string.IsNullOrEmpty(_JournalVoucherDTO.ptypeofoperation))
                {
                    _JournalVoucherDTO.ptypeofoperation = "CREATE";
                }
                if (_JournalVoucherDTO.ptypeofoperation.ToUpper() == "CREATE")
                {
                    query = "insert into tbltransjournalvoucher( jvnumber, jvdate, narration, statusid, createdby, createddate,filename,filepath,fileformat,modeoftransaction)values('" + _JournalVoucherDTO.pjvnumber + "', '" + FormatDate(_JournalVoucherDTO.pjvdate) + "', '" + ManageQuote(_JournalVoucherDTO.pnarration) + "',  " + Convert.ToInt32(Status.Active) + ", " + _JournalVoucherDTO.pCreatedby + ", current_timestamp,'" + ManageQuote(_JournalVoucherDTO.pFilename) + "','" + ManageQuote(_JournalVoucherDTO.pFilepath) + "','" + ManageQuote(_JournalVoucherDTO.pFileformat) + "','" + ManageQuote(_JournalVoucherDTO.pmodoftransaction) + "') returning recordid";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));

                }
                else if (_JournalVoucherDTO.ptypeofoperation.ToUpper() == "UPDATE")
                {
                    query = "select recordid from tbltransjournalvoucher where jvnumber='" + _JournalVoucherDTO.pjvnumber + "';";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                    sbQuery.Append("update tbltransjournalvoucher set  modifiedby=" + _JournalVoucherDTO.pCreatedby + ", modifieddate=current_timestamp where jvnumber='" + _JournalVoucherDTO.pjvnumber + "'; ");
                    sbQuery.Append("delete from tbltransjournalvoucherdetails where jvnumber='" + _JournalVoucherDTO.pjvnumber + "'; ");
                    sbQuery.Append("delete from tbltranstotaltransactions where transactionno='" + _JournalVoucherDTO.pjvnumber + "' ;");

                }
                if (_JournalVoucherDTO.pJournalVoucherlist != null)
                {
                    for (int i = 0; i < _JournalVoucherDTO.pJournalVoucherlist.Count; i++)
                    {
                        if (string.IsNullOrEmpty(_JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation))
                        {
                            _JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation = "CREATE";
                        }
                        if (_JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            if (string.IsNullOrEmpty(_JournalVoucherDTO.pJournalVoucherlist[i].ppartyid))
                            {
                                _JournalVoucherDTO.pJournalVoucherlist[i].ppartyid = "0";
                            }
                            sbQuery.Append("insert into tbltransjournalvoucherdetails( detailsid, jvnumber, contactid,contactname,contactrefid,contactreftype, jvaccountid, ledgeramount,accounttranstype, gsttype, gstcalculationtype, tdssection, pannumber, tdscalculationtype, tdspercentage, tdsamount, gstpercentage, igstamount, cgstamount, sgstamount, utgstamount, istdsapplicable,gstnumber,tdsaccountid,stateid,statename)values (" + detailsid + ", '" + ManageQuote(_JournalVoucherDTO.pjvnumber) + "'," + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyid + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyname + "','" + ManageQuote(_JournalVoucherDTO.pJournalVoucherlist[i].ppartyreferenceid) + "','" + ManageQuote(_JournalVoucherDTO.pJournalVoucherlist[i].ppartyreftype) + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].psubledgerid + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].pamount + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].ptranstype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgsttype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstcalculationtype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].ptdssection + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartypannumber + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].ptdscalculationtype + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].ptdspercentage + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].ptdsamount + ", '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstpercentage + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pigstamount + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pcgstamount + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].psgstamount + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].putgstamount + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].pistdsapplicable + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstno + "'," + _JournalVoucherDTO.pJournalVoucherlist[i].ptdsaccountId + "," + _JournalVoucherDTO.pJournalVoucherlist[i].pStateId + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].pState + "');");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(_JournalVoucherDTO.pjvnumber))
                {
                    //sbQuery.AppendLine("SELECT fntotaltransactions('" + _JournalVoucherDTO.pjvnumber + "','JOURNALVOUCHER');select accountsupdate();");
                    sbQuery.AppendLine("SELECT fntotaltransactions('" + _JournalVoucherDTO.pjvnumber + "','JOURNALVOUCHER');");
                }

                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                }

                IsSaved = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return IsSaved;

        }
        public bool SaveJournalVoucher(JournalVoucherDTO _JournalVoucherDTO, NpgsqlTransaction trans, out string ptdsrefjvnumber)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(_JournalVoucherDTO.pjvnumber))
                {
                    _JournalVoucherDTO.pjvnumber = Convert.ToString(Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('JOURNAL VOUCHER','AUTO','" + FormatDate(_JournalVoucherDTO.pjvdate) + "')")));
                }

                ptdsrefjvnumber = _JournalVoucherDTO.pjvnumber;
                string query = "";
                long detailsid = 0;
                if (string.IsNullOrEmpty(_JournalVoucherDTO.ptypeofoperation))
                {
                    _JournalVoucherDTO.ptypeofoperation = "CREATE";
                }
                if (_JournalVoucherDTO.ptypeofoperation.ToUpper() == "CREATE")
                {
                    query = "insert into tbltransjournalvoucher( jvnumber, jvdate, narration, statusid, createdby, createddate,filename,filepath,fileformat,modeoftransaction)values('" + _JournalVoucherDTO.pjvnumber + "', '" + FormatDate(_JournalVoucherDTO.pjvdate) + "', '" + ManageQuote(_JournalVoucherDTO.pnarration) + "',  " + Convert.ToInt32(Status.Active) + ", " + _JournalVoucherDTO.pCreatedby + ", current_timestamp,'" + ManageQuote(_JournalVoucherDTO.pFilename) + "','" + ManageQuote(_JournalVoucherDTO.pFilepath) + "','" + ManageQuote(_JournalVoucherDTO.pFileformat) + "','"+ ManageQuote(_JournalVoucherDTO.pmodoftransaction) + "') returning recordid";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));

                }
                else if (_JournalVoucherDTO.ptypeofoperation.ToUpper() == "UPDATE")
                {
                    query = "select recordid from tbltransjournalvoucher where jvnumber='" + _JournalVoucherDTO.pjvnumber + "';";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                    sbQuery.Append("update tbltransjournalvoucher set  modifiedby=" + _JournalVoucherDTO.pCreatedby + ", modifieddate=current_timestamp where jvnumber='" + _JournalVoucherDTO.pjvnumber + "'; ");
                    sbQuery.Append("delete from tbltransjournalvoucherdetails where jvnumber='" + _JournalVoucherDTO.pjvnumber + "'; ");
                    sbQuery.Append("delete from tbltranstotaltransactions where transactionno='" + _JournalVoucherDTO.pjvnumber + "' ;");

                }
                if (_JournalVoucherDTO.pJournalVoucherlist != null)
                {
                    for (int i = 0; i < _JournalVoucherDTO.pJournalVoucherlist.Count; i++)
                    {
                        if (string.IsNullOrEmpty(_JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation))
                        {
                            _JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation = "CREATE";
                        }
                        if (_JournalVoucherDTO.pJournalVoucherlist[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            sbQuery.Append("insert into tbltransjournalvoucherdetails( detailsid, jvnumber, contactid,contactname,contactrefid,contactreftype, jvaccountid, ledgeramount,accounttranstype, gsttype, gstcalculationtype, tdssection, pannumber, tdscalculationtype, tdspercentage, tdsamount, gstpercentage, igstamount, cgstamount, sgstamount, utgstamount, istdsapplicable,gstnumber,tdsaccountid,stateid,statename)values (" + detailsid + ", '" + ManageQuote(_JournalVoucherDTO.pjvnumber) + "'," + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyid + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyname + "','" + ManageQuote(_JournalVoucherDTO.pJournalVoucherlist[i].ppartyreferenceid) + "','" + ManageQuote(_JournalVoucherDTO.pJournalVoucherlist[i].ppartyreftype) + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].psubledgerid + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].pamount + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].ptranstype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgsttype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstcalculationtype + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pTdsSection + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartypannumber + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].ptdscalculationtype + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].pTdsPercentage + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].ptdsamount + ", '" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstpercentage + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pigstamount + "', '" + _JournalVoucherDTO.pJournalVoucherlist[i].pcgstamount + "', " + _JournalVoucherDTO.pJournalVoucherlist[i].psgstamount + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].putgstamount + ", " + _JournalVoucherDTO.pJournalVoucherlist[i].pistdsapplicable + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].pgstno + "'," + _JournalVoucherDTO.pJournalVoucherlist[i].ptdsaccountId + "," + _JournalVoucherDTO.pJournalVoucherlist[i].pStateId + ",'" + _JournalVoucherDTO.pJournalVoucherlist[i].pState + "');");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(_JournalVoucherDTO.pjvnumber))
                {
                    sbQuery.AppendLine("SELECT fntotaltransactions('" + _JournalVoucherDTO.pjvnumber + "','JOURNALVOUCHER');");
                    //select accountsupdate();
                }
                
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                }

                IsSaved = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return IsSaved;

        }

        public bool SaveJournalVoucher_All(JournalVoucherDTO _JournalVoucherDTO, string ConnectionString, out string jvnumber)
        {
            long accountid = 0;
            bool isSaved = false;
            string refjvnumber = "";
            StringBuilder sbinsert = new StringBuilder();
            objJournalVoucherDTO = new JournalVoucherDTO();
            List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_JournalVoucherDTO.pJournalVoucherlist != null)
                {
                    objJournalVoucherDTO.pjvdate = _JournalVoucherDTO.pjvdate;
                    objJournalVoucherDTO.pnarration = _JournalVoucherDTO.pnarration.ToUpper();
                    objJournalVoucherDTO.pmodoftransaction = "MANUAL";
                    objJournalVoucherDTO.pCreatedby = _JournalVoucherDTO.pCreatedby ;
                    for (int i = 0; i < _JournalVoucherDTO.pJournalVoucherlist.Count; i++)
                    {



                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.ppartyid = _JournalVoucherDTO.pJournalVoucherlist[i].ppartyid;
                        objPaymentsDTO.ppartyname = _JournalVoucherDTO.pJournalVoucherlist[i].ppartyname;
                        objPaymentsDTO.ppartyreferenceid = _JournalVoucherDTO.pJournalVoucherlist[i].ppartyreferenceid;
                        objPaymentsDTO.ppartyreftype = _JournalVoucherDTO.pJournalVoucherlist[i].ppartyreftype;
                        if (_JournalVoucherDTO.pJournalVoucherlist[i].ptranstype.ToUpper() == "CREDIT")
                        {
                            if(_JournalVoucherDTO.pJournalVoucherlist[i].ptdsamount > 0)
                            {
                                accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='OTHER CURRENT LIABILITIES' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                                accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('TDS-" + _JournalVoucherDTO.pJournalVoucherlist[i].pTdsSection + " PAYABLE'," + accountid + ",'2',"+ objJournalVoucherDTO.pCreatedby + ")"));
                                accountid= Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyreferenceid + "_" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyname + "'," + accountid + ",'3',"+ objJournalVoucherDTO.pCreatedby + ")"));
                            }
                            objPaymentsDTO.ptranstype = "C";
                        }
                        if (_JournalVoucherDTO.pJournalVoucherlist[i].ptranstype.ToUpper() == "DEBIT") 
                        {
                            if (_JournalVoucherDTO.pJournalVoucherlist[i].ptdsamount > 0)
                            {
                                accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where  upper(accountname)='OTHER CURRENT ASSETS' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                                accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('TDS-" + _JournalVoucherDTO.pJournalVoucherlist[i].pTdsSection + " RECEIVABLE'," + accountid + ",'2',"+ objJournalVoucherDTO.pCreatedby + ")"));
                                accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyreferenceid + "_" + _JournalVoucherDTO.pJournalVoucherlist[i].ppartyname + "'," + accountid + ",'3',"+ objJournalVoucherDTO.pCreatedby + ")"));
                            }
                                objPaymentsDTO.ptranstype = "D";
                          

                        }
                       
                        objPaymentsDTO.psubledgerid = _JournalVoucherDTO.pJournalVoucherlist[i].psubledgerid;
                        objPaymentsDTO.pamount = _JournalVoucherDTO.pJournalVoucherlist[i].pamount;
                        objPaymentsDTO.pgstcalculationtype = _JournalVoucherDTO.pJournalVoucherlist[i].pgstcalculationtype;
                        objPaymentsDTO.pTdsSection = _JournalVoucherDTO.pJournalVoucherlist[i].pTdsSection;
                        objPaymentsDTO.ptdscalculationtype = _JournalVoucherDTO.pJournalVoucherlist[i].ptdscalculationtype;
                        objPaymentsDTO.pTdsPercentage = _JournalVoucherDTO.pJournalVoucherlist[i].pTdsPercentage;
                        objPaymentsDTO.ptdsamount = _JournalVoucherDTO.pJournalVoucherlist[i].ptdsamount;
                        objPaymentsDTO.pgstpercentage = _JournalVoucherDTO.pJournalVoucherlist[i].pgstpercentage;
                        objPaymentsDTO.pigstamount = _JournalVoucherDTO.pJournalVoucherlist[i].pigstamount;
                        objPaymentsDTO.pcgstamount = _JournalVoucherDTO.pJournalVoucherlist[i].pcgstamount;
                        objPaymentsDTO.psgstamount = _JournalVoucherDTO.pJournalVoucherlist[i].psgstamount;
                        objPaymentsDTO.putgstamount = _JournalVoucherDTO.pJournalVoucherlist[i].putgstamount;
                        objPaymentsDTO.pistdsapplicable = _JournalVoucherDTO.pJournalVoucherlist[i].pistdsapplicable;
                        objPaymentsDTO.pgstnumber = _JournalVoucherDTO.pJournalVoucherlist[i].pgstnumber;
                        objPaymentsDTO.pStateId = _JournalVoucherDTO.pJournalVoucherlist[i].pStateId;
                        objPaymentsDTO.pState = _JournalVoucherDTO.pJournalVoucherlist[i].pState;
                        objPaymentsDTO.pgstno = _JournalVoucherDTO.pJournalVoucherlist[i].pgstno;
                        objPaymentsDTO.pgsttype = _JournalVoucherDTO.pJournalVoucherlist[i].pgsttype;
                        objPaymentsDTO.ptdsaccountId = accountid;

                        _Paymentslist.Add(objPaymentsDTO);
                    }
                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                    
                    SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                    trans.Commit();
                 
                    isSaved = true;

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
            jvnumber = refjvnumber;
            return isSaved;
        }

        public async Task<List<JournalVoucherDTO>> GetJournalVoucherData(string ConnectionString)
        {
            await Task.Run(() =>
            {
                pJournalVoucherList = new List<JournalVoucherDTO>();
                List<PaymentsDTO> pPaymentList = new List<PaymentsDTO>();
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.recordid,to_char(jvdate,'DD-MM-YYYY')as jvdate,t1.jvnumber," +
                        "sum(ledgeramount)as Amount,t1.narration from tbltransjournalvoucher t1 join tbltransjournalvoucherdetails t2 on t1.jvnumber=t2.jvnumber where jvdate=current_date " +
                        "and t2.accounttranstype='D'  group by t1.recordid,t1.jvdate,t1.jvnumber,t1.narration order by t1.recordid desc "))

                    {
                        while (dr.Read())
                        {


                            JournalVoucherDTO _journalVoucherDTO = new JournalVoucherDTO
                            {
                                pjvdate = Convert.ToString(dr["jvdate"]),
                                pjvnumber = Convert.ToString(dr["jvnumber"]),
                                ptotalpaidamount = Convert.ToDecimal(dr["Amount"]),
                                pnarration = Convert.ToString(dr["narration"])
                            };
                            pJournalVoucherList.Add(_journalVoucherDTO);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return pJournalVoucherList;
        }

        public async Task<List<JournalVoucherReportDTO>> GetJournalVoucherReportData(string ConnectionString, string Jvnumber)
        {
            await Task.Run(() =>
            {
                lstJournalVoucherReport = new List<JournalVoucherReportDTO>();
                try
                {
                    string strQuery = "select jvnumber,jvdate,jvaccountid,accountname,case when chracctype='3' then parentaccountname||'('||accountname||')' else accountname end as particulars, narration, parentaccountname,case when accounttranstype='D' then ledgeramount else 0 end as debitamount,case when accounttranstype='C' then ledgeramount else 0 end as creditamount  from tbltransjournalvoucherdetails a join tblmstaccounts b on a.jvaccountid=b.accountid join tbltransjournalvoucher c using (jvnumber) where UPPER(jvnumber) ='" + Jvnumber.Trim().ToUpper() + "';";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            JournalVoucherReportDTO _JVReportDTO = new JournalVoucherReportDTO();
                            _JVReportDTO.pJvDate = dr["jvdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["jvdate"]).ToString("dd-MM-yyyy");
                            _JVReportDTO.pJvnumber = Convert.ToString(dr["jvnumber"]);
                            _JVReportDTO.pCreditAmount = dr["creditamount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["creditamount"]);
                            _JVReportDTO.pDebitamount = dr["debitamount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["debitamount"]);
                            _JVReportDTO.pParticulars = Convert.ToString(dr["particulars"]);
                            _JVReportDTO.pNarration = Convert.ToString(dr["narration"]);
                            lstJournalVoucherReport.Add(_JVReportDTO);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return lstJournalVoucherReport;
        }


    }
}
