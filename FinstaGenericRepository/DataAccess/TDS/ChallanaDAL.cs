using FinstaInfrastructure.TDS;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.TDS;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.TDS
{
   public class ChallanaDAL: SettingsDAL, IChallana
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        public async Task<List<ChallanaDTO>> GetChallanaDetails(string Connectionstring, string FromDate, string ToDate, string SectionName,string CompanyType,string PanType)
        {
            List<ChallanaDTO> ChallanaDetailsList = new List<ChallanaDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from(select *,(case when SUBSTR(panno,4,1)='C' then 'Company' else 'Non-Company' end)as CompanyType,(case when LENGTH(TRIM(panno))=10 then 'With Pan' else 'Without Pan' end) as PanType,tt.accountname as accountname from tds_vouchers  t join  tblmstaccounts tt on t.account_id=tt.accountid where section='" + SectionName + "' and trans_date between '" + FormatDate(FromDate) + "' and '" + FormatDate(ToDate) + "')tabl where CompanyType='"+ CompanyType + "' and PanType='"+ PanType + "' and tds_voucher_id not in(select tds_voucher_id from challana_entry_details); "))
                    {
                        while (dr.Read())
                        {
                            var ChallanaDetails = new ChallanaDTO
                            {
                                pParentId = dr["parent_id"],
                                pAccountId = dr["account_id"],
                                pAccountName = dr["accountname"],
                                pPanNo = dr["panno"],
                                pAmount = dr["amount"],
                                pTdsAmount = dr["cal_tds"],
                                pActualTdsAmount = dr["actual_tds"],
                                pBalance = dr["balance"],
                                pTransDate = Convert.ToDateTime(dr["trans_date"]).ToString("dd/MMM/yyyy"),
                                pTdsSection = dr["section"],
                                pSectionName = dr["section_name"],
                                pPaidAmount = dr["paid_amount"],
                                pCompanyType = dr["companytype"],
                                pPanType = dr["pantype"],
                                pTdsVoucherId = dr["tds_voucher_id"],
                            };
                            ChallanaDetailsList.Add(ChallanaDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return ChallanaDetailsList;
        }
        public bool SaveChallanaEntry(ChallanaEntryDTO _ChallanaEntryDTO, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string query = "";
                long ChallanaId = 0;
                if (string.IsNullOrEmpty(Convert.ToString(_ChallanaEntryDTO.pChallanaNo)))
                {
                    _ChallanaEntryDTO.pChallanaNo = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('CHALLANA ENTRY', 'CHALLANA', current_date)").ToString();
                    //_ChallanaEntryDTO.pChallanaNo =Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('CHALLANA ENTRY','CHALLANA',current_date);"));
                }
                if (string.IsNullOrEmpty(_ChallanaEntryDTO.ptypeofoperation.ToString()))
                {
                    _ChallanaEntryDTO.ptypeofoperation = "CREATE";
                }
                if (_ChallanaEntryDTO.ptypeofoperation.ToString().ToUpper() == "CREATE")
                {
                    query = "insert into challana_entry( challana_no, from_date, to_date, company_type, tds_percent,calc_tds,actual_tds,paid_Amount,companyid)values('" + _ChallanaEntryDTO.pChallanaNo + "', '" + FormatDate(_ChallanaEntryDTO.pFromDate.ToString()) + "',  '" + FormatDate(_ChallanaEntryDTO.pToDate.ToString()) + "', '"+ _ChallanaEntryDTO.pCompanyType+ "','"+ _ChallanaEntryDTO.pTdsType + "',"+ _ChallanaEntryDTO.pTotalTdsAmount + "," + _ChallanaEntryDTO.pActualTotalTdsAmount + "," + _ChallanaEntryDTO.pTotalPaidAmount + "," + _ChallanaEntryDTO.pCompanyId + ") returning challana_id";
                    ChallanaId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));

                }
                if (_ChallanaEntryDTO._ChallanaEntryDetails != null)
                {
                    for (int i = 0; i < _ChallanaEntryDTO._ChallanaEntryDetails.Count; i++)
                    {
                        sbinsert.Append("insert into challana_entry_details( challana_id, tds_voucher_id,parent_id,account_id,panno,amount,calc_tds,actual_tds,balance,paidamt,status)values (" + ChallanaId + ","+ _ChallanaEntryDTO._ChallanaEntryDetails[i].pTdsVoucherId + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pParentId + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pAccountId + ",'" + _ChallanaEntryDTO._ChallanaEntryDetails[i].pPanNo + "'," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pAmount + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pTdsAmount + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pActualTdsAmount + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pBalance + "," + _ChallanaEntryDTO._ChallanaEntryDetails[i].pPaidAmount + ",'true');");
                    }
                }
                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
                }
                trans.Commit();
                isSaved = true;
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
            return isSaved;
        }
        public List<ChallanaNoDTO> GetChallanaNumbers(string connectionstring)
        {
            List<ChallanaNoDTO> lstChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct challana_no ,ce.challana_id,from_date,to_date ,section from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id join tds_vouchers t on  ced.tds_voucher_id=t.tds_voucher_id where voucher_id is null; "))
                {
                    while (dr.Read())
                    {
                        ChallanaNoDTO objChallanaNumbers = new ChallanaNoDTO()
                        {
                            pChallanaId = dr["challana_id"],
                            pChallanaNo = dr["challana_no"],
                            pFromdate= Convert.ToDateTime(dr["from_date"]).ToString("dd/MMM/yyyy"),
                            pTodate= Convert.ToDateTime(dr["to_date"]).ToString("dd/MMM/yyyy"),
                           pSection=Convert.ToString(dr["section"])

                        };
                        lstChallanaNumbers.Add(objChallanaNumbers);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaNumbers;
        }
        public List<ChallanaDetailsDTO> GetChallanaEntryDetails(string connectionstring,string ChallanaNO)
        {
            List<ChallanaDetailsDTO> lstChallanaNumbers = new List<ChallanaDetailsDTO>();
            try
            {
                //query = "select  ced.challana_details_id,ce.challana_no ,tds_voucher_id,parent_id,account_id,panno,ced.amount,ced.calc_tds,ced.actual_tds,ced.balance,ced.paidamt from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id where voucher_id is null and challana_no='" + ChallanaNO + "' and status='true'; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select  ced.challana_details_id,ce.challana_no ,ced.tds_voucher_id,ced.parent_id,ced.account_id,ced.panno,ced.amount,ced.calc_tds,ced.actual_tds,ced.balance,ced.paidamt,tt.accountname as account_name from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id join tds_vouchers  t on ced.tds_voucher_id=t.tds_voucher_id join  tblmstaccounts tt on t.account_id=tt.accountid  where voucher_id is null and challana_no='" + ChallanaNO + "' and ced.status='true'; "))
                {
                    while (dr.Read())
                    {
                        ChallanaDetailsDTO objChallanaDetails = new ChallanaDetailsDTO()
                        {
                            pChallanaDetailsId = dr["challana_details_id"],
                            pChallanaNo = dr["challana_no"],
                            pTdsVoucherId = dr["tds_voucher_id"],
                            pParentId = dr["parent_id"],
                            pAccountId = dr["account_id"],
                            pAccountName = dr["account_name"],
                            pPanNo = dr["panno"],
                            pAmount = dr["amount"],
                            pTdsAmount = dr["calc_tds"],
                            pActualTdsAmount = dr["actual_tds"],
                            pBalance = dr["balance"],
                            pPaidAmount = dr["paidamt"]

                        };
                        lstChallanaNumbers.Add(objChallanaDetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaNumbers;
        }
        public bool SaveChallanaPayment(ChallanaPaymentDTO _ChallanaPaymentDTO, string Connectionstring, out string _PaymentId)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                //if (string.IsNullOrEmpty(_CommissionPaymentDTO.ppaymentid.ToString()))
                //{
                
                _ChallanaPaymentDTO.ppaymentid = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('PAYMENT VOUCHER','CASH','" + FormatDate(_ChallanaPaymentDTO.pCommissionpaymentDate) + "')"));
                //}
                long creditaccountid = 0;
                string query = "";
                long detailsid = 0;
                string accountName = string.Empty;

                creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select bankaccountid from tblmstbank  where recordid = " + _ChallanaPaymentDTO.pbankid));
               

                query = "insert into tbltranspaymentvoucher( paymentid, paymentdate, modeofpayment, totalpaidamount, narration, creditaccountid, statusid, createdby, createddate)values('" + _ChallanaPaymentDTO.ppaymentid + "', '" + FormatDate(_ChallanaPaymentDTO.pCommissionpaymentDate) + "', '" + ManageQuote(_ChallanaPaymentDTO.ptranstype) + "', " + _ChallanaPaymentDTO.pTotalpaymentamount + ", '" + ManageQuote(_ChallanaPaymentDTO.pNarration) + "', " + creditaccountid + ", " + Convert.ToInt32(Status.Active) + "," + _ChallanaPaymentDTO.pCreatedby + ", current_timestamp) returning recordid";
                detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));


                if (_ChallanaPaymentDTO.pcommissionpaymentlist != null)
                {
                    for (int i = 0; i < _ChallanaPaymentDTO.pcommissionpaymentlist.Count; i++)
                    {
                        accountName = Convert.ToString(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select distinct accountname from tblmstaccounts where  accountid ="+ _ChallanaPaymentDTO.pcommissionpaymentlist[i].pdebitaccountid + ""));
                        sbQuery.Append("insert into tbltranspaymentvoucherdetails( detailsid, paymentid,ledgeramount,contactname,debitaccountid)values (" + detailsid + ", '" + _ChallanaPaymentDTO.ppaymentid + "', '" + _ChallanaPaymentDTO.pcommissionpaymentlist[i].ptotalamount + "','"+ accountName + "'," + _ChallanaPaymentDTO.pcommissionpaymentlist[i].pdebitaccountid + ");");

                        sbQuery.Append("update challana_entry_details set voucher_id=(select recordid from tbltranspaymentvoucher where paymentid='" + _ChallanaPaymentDTO.ppaymentid + "') where  challana_details_id=" + _ChallanaPaymentDTO.pcommissionpaymentlist[i].pChallanaDetailsId + ";");
                    }
                }

                if (_ChallanaPaymentDTO.ptranstype != "CASH")
                {
                    string particulars = "";

                    if (string.IsNullOrEmpty(_ChallanaPaymentDTO.ptypeofpayment))
                    {
                        _ChallanaPaymentDTO.ptypeofpayment = _ChallanaPaymentDTO.ptypeofpayment;
                    }
                    if (_ChallanaPaymentDTO.pbankname.Contains('-'))
                    {

                        _ChallanaPaymentDTO.pbankname = _ChallanaPaymentDTO.pbankname.Split('-')[0].Trim();
                    }


                    if (_ChallanaPaymentDTO.ptranstype == "CHEQUE")
                    {
                        sbQuery.Append("update  tblmstcheques set   statusid =(SELECT  statusid from tblmststatus  where upper(statusname)  ='USED-CHEQUES') where bankid =" + _ChallanaPaymentDTO.pbankid + " and chequenumber=" + _ChallanaPaymentDTO.pchequeno + ";");

                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _ChallanaPaymentDTO.ppaymentid + "', '" + _ChallanaPaymentDTO.pbankname + "', '" + _ChallanaPaymentDTO.pbranchname + "', '" + _ChallanaPaymentDTO.ptranstype + "', '" + _ChallanaPaymentDTO.ptranstype + "', " + _ChallanaPaymentDTO.pbankid + ", '" + _ChallanaPaymentDTO.pchequeno + "', '" + _ChallanaPaymentDTO.pdebitcard + "', '" + _ChallanaPaymentDTO.pUpiid + "', '" + _ChallanaPaymentDTO.pUpiname + "', '" + FormatDate(_ChallanaPaymentDTO.pCommissionpaymentDate) + "', " + _ChallanaPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _ChallanaPaymentDTO.pCreatedby + ", current_timestamp);");
                    }
                    else if (_ChallanaPaymentDTO.ptranstype == "ONLINE")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _ChallanaPaymentDTO.ppaymentid + "', '" + _ChallanaPaymentDTO.pBankname + "', '" + _ChallanaPaymentDTO.pBankbranchname + "', '" + _ChallanaPaymentDTO.ptranstype + "', '" + _ChallanaPaymentDTO.ptypeofpayment + "', " + _ChallanaPaymentDTO.pbankid + ", '" + _ChallanaPaymentDTO.preferencenoonline + "', '" + _ChallanaPaymentDTO.pdebitcard + "', '" + _ChallanaPaymentDTO.pUpiid + "', '" + _ChallanaPaymentDTO.pUpiname + "', '" + FormatDate(_ChallanaPaymentDTO.pCommissionpaymentDate) + "', " + _ChallanaPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _ChallanaPaymentDTO.pCreatedby + ", current_timestamp);");
                    }
                    else if (_ChallanaPaymentDTO.ptranstype == "DEBIT CARD")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _ChallanaPaymentDTO.ppaymentid + "', '" + _ChallanaPaymentDTO.pBankname + "', '" + _ChallanaPaymentDTO.pBankbranchname + "', '" + _ChallanaPaymentDTO.ptranstype + "', '" + _ChallanaPaymentDTO.ptranstype + "', " + _ChallanaPaymentDTO.pbankid + ", '" + _ChallanaPaymentDTO.preferencenodcard + "', '" + _ChallanaPaymentDTO.pdebitcard + "', '" + _ChallanaPaymentDTO.pUpiid + "', '" + _ChallanaPaymentDTO.pUpiname + "', '" + FormatDate(_ChallanaPaymentDTO.pCommissionpaymentDate) + "', " + _ChallanaPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _ChallanaPaymentDTO.pCreatedby + ", current_timestamp);");

                    }
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {


                    sbQuery.Append("select fntotaltransactions('" + _ChallanaPaymentDTO.ppaymentid + "','PAYMENT VOUCHER');");
                   // select accountsupdate();
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                    trans.Commit();
                    IsSaved = true;
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
            _PaymentId = _ChallanaPaymentDTO.ppaymentid;
            return IsSaved;

        }
        #region Cin Entry
        public List<ChallanaNoDTO> GetChallanaPaymentNumbers(string connectionstring)
        {
            List<ChallanaNoDTO> lstChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct challana_no ,ce.challana_id,from_date,to_date from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id where voucher_id is not null and ce.challana_id not in(select challana_id from cin_entry ); "))
                {
                    while (dr.Read())
                    {
                        ChallanaNoDTO objChallanaNumbers = new ChallanaNoDTO()
                        {
                            pChallanaId = dr["challana_id"],
                            pChallanaNo = dr["challana_no"],
                            pFromdate = Convert.ToDateTime(dr["from_date"]).ToString("dd/MMM/yyyy"),
                            pTodate = Convert.ToDateTime(dr["to_date"]).ToString("dd/MMM/yyyy"),


                        };
                        lstChallanaNumbers.Add(objChallanaNumbers);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaNumbers;
        }
        public GetChallanaPaymentsDTO GetChallanaPaymentDetails(string connectionstring, string ChallanaNO)
        {
            GetChallanaPaymentsDTO lstChallanaPayments = new GetChallanaPaymentsDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct pv.paymentid as voucherno,pv.paymentdate,tt.bankname,tt.chequenumber from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id join tbltranspaymentvoucher pv on pv.recordid=ced.voucher_id left join tbltranspaymentreference tt on  tt.paymentid=pv.paymentid where ce.challana_no ='"+ ChallanaNO + "' and ced.status='true' and ce.challana_id not in(select challana_id from cin_entry ); "))
                {
                    while (dr.Read())
                    {
                        lstChallanaPayments = new GetChallanaPaymentsDTO()
                        {
                           pVoucherno=dr["voucherno"],
                            pPaymentdate=dr["paymentdate"],
                            pBankname=dr["bankname"],
                            pChequenumber=dr["chequenumber"],
                            ChallanaPaymentList= GetChallanaPaymentDetailsList(connectionstring, ChallanaNO)

                        };
                       
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaPayments;
        }
        public List<ChallanaDetailsDTO> GetChallanaPaymentDetailsList(string connectionstring, string ChallanaNO)
        {
            List<ChallanaDetailsDTO> lstChallanaNumbers = new List<ChallanaDetailsDTO>();
            try
            {
                //"select  ced.challana_details_id,ce.challana_no ,tds_voucher_id,parent_id,account_id,panno,ced.amount,ced.calc_tds,ced.actual_tds,ced.balance,ced.paidamt,tp.paymentid as voucher_id from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id left join tbltranspaymentvoucher tp on tp.recordid=ced.voucher_id where voucher_id is not null and challana_no='" + ChallanaNO + "' and status='true' and ce.challana_id not in(select challana_id from cin_entry ); "
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select  ced.challana_details_id,ce.challana_no ,ced.tds_voucher_id,ced.parent_id,ced.account_id,ced.panno,ced.amount,ced.calc_tds,ced.actual_tds,ced.balance,ced.paidamt,tp.paymentid as voucher_id,tt.accountname as account_name from challana_entry ce join challana_entry_details ced on ce.challana_id=ced.challana_id join tds_vouchers  t on ced.tds_voucher_id=t.tds_voucher_id join  tblmstaccounts tt on t.account_id=tt.accountid  left join tbltranspaymentvoucher tp on tp.recordid=ced.voucher_id where voucher_id is not null and challana_no='" + ChallanaNO + "' and ced.status='true' and ce.challana_id not in(select challana_id from cin_entry ); "))
                {
                    while (dr.Read())
                    {
                        ChallanaDetailsDTO objChallanaDetails = new ChallanaDetailsDTO()
                        {
                            pChallanaDetailsId = dr["challana_details_id"],
                            pChallanaNo = dr["challana_no"],
                            pTdsVoucherId = dr["tds_voucher_id"],
                            pParentId = dr["parent_id"],
                            pAccountId = dr["account_id"],
                            pAccountName= dr["account_name"],
                            pPanNo = dr["panno"],
                            pAmount = dr["amount"],
                            pTdsAmount = dr["calc_tds"],
                            pActualTdsAmount = dr["actual_tds"],
                            pBalance = dr["balance"],
                            pPaidAmount = dr["paidamt"],
                            pVoucherId = dr["voucher_id"]

                        };
                        lstChallanaNumbers.Add(objChallanaDetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaNumbers;
        }

        public bool SaveCinEntry(CinEntryDTO _CinEntryDTO, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string query = "";

                Int64 VoucherId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select recordid from tbltranspaymentvoucher where paymentid = '" + _CinEntryDTO.pVoucherId + "'"));
                
                
                if (string.IsNullOrEmpty(_CinEntryDTO.ptypeofoperation.ToString()))
                {
                    _CinEntryDTO.ptypeofoperation = "CREATE";
                }
                if (_CinEntryDTO.ptypeofoperation.ToString().ToUpper() == "CREATE")
                {
                    query = "insert into cin_entry( challana_id, voucher_id, reference_no, paid_date, paid_bank,bsr_code,challana_sl_no,challana_bank,challana_date)values("+ _CinEntryDTO.pChallanaId+ ","+ VoucherId + ",'"+ _CinEntryDTO .pReferenceNo+ "','" + FormatDate( _CinEntryDTO.pPaidDate.ToString()) + "','" + _CinEntryDTO.pPaidBank + "','" + _CinEntryDTO.pBsrCode + "','" + _CinEntryDTO.pChallanaSNO + "','" + _CinEntryDTO.pChallanaBank + "','" + FormatDate(_CinEntryDTO.pChallanaDate.ToString()) + "') ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query);

                }
               
               
                trans.Commit();
                isSaved = true;
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
            return isSaved;
        }


        #endregion

        #region Reports
        public async Task<List<CinEntryReportDTO>> GetCinEntryReportByChallanaNo(string Connectionstring, string ChallanaNO)
        {
            List<CinEntryReportDTO> _CinEntryReportDTO = new List<CinEntryReportDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select c.challana_no,b.panno,d.account_name as name,a.paid_date,b.amount,b.actual_tds,a.challana_date,a.bsr_code,a.challana_sl_no,a.challana_bank,a.reference_no,a.paid_bank,c.from_date,c.to_date,d.section from cin_entry a join challana_entry c on a.challana_id=c.challana_id join challana_entry_details b  on b.challana_id=c.challana_id join tds_vouchers d on d.tds_voucher_id=b.tds_voucher_id  where c.challana_no='" + ChallanaNO + "' "))
                    {
                        while (dr.Read())
                        {
                            var _CinEntryReportDTO1 = new CinEntryReportDTO
                            {
                                pChallanaNo = dr["challana_no"],
                                pPanNo = dr["panno"],
                                pName = dr["name"],
                                pPaidDate = Convert.ToDateTime(dr["paid_date"]).ToString("dd/MMM/yyyy"),
                                pAmount =Convert.ToDecimal(dr["amount"]),
                                pActualTdsAmount = Convert.ToDecimal(dr["actual_tds"]),
                                pChallanaDate = Convert.ToDateTime(dr["challana_date"]).ToString("dd/MMM/yyyy"),
                                pBsrCode = dr["bsr_code"],
                                pChallanaSNO = dr["challana_sl_no"],
                                pChallanaBank = dr["challana_bank"],
                                pReferenceNo = dr["reference_no"],
                                pPaidBank = dr["paid_bank"],
                                pFromdate = dr["from_date"],
                                pTodate = dr["to_date"],
                                pSection= dr["section"],
                            };
                            _CinEntryReportDTO.Add(_CinEntryReportDTO1);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _CinEntryReportDTO;
        }
        public async Task<List<CinEntryReportDTO>> GetCinEntryReportsBetweenDates(string Connectionstring, string FromDate,string ToDate)
        {
            List<CinEntryReportDTO> _CinEntryReportDTO = new List<CinEntryReportDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select c.challana_no,b.panno,d.account_name as name,a.paid_date,b.amount,b.calc_tds,b.actual_tds,a.challana_date,a.bsr_code,a.challana_sl_no,a.challana_bank,a.reference_no,a.paid_bank,c.from_date,c.to_date,d.section from cin_entry a join challana_entry c on a.challana_id=c.challana_id join challana_entry_details b  on b.challana_id=c.challana_id join tds_vouchers d on d.tds_voucher_id=b.tds_voucher_id  where c.from_date>='" + FormatDate(FromDate) + "' and c.to_date<='" + FormatDate(ToDate) + "'"))
                    {
                        while (dr.Read())
                        {
                            var _CinEntryReportDTO1 = new CinEntryReportDTO
                            {
                                pChallanaNo = dr["challana_no"],
                                pPanNo = dr["panno"],
                                pName = dr["name"],
                                pPaidDate = Convert.ToDateTime(dr["paid_date"]).ToString("dd/MMM/yyyy"),
                                pAmount = Convert.ToDecimal(dr["amount"]),
                                pActualTdsAmount = Convert.ToDecimal(dr["actual_tds"]),
                                pChallanaDate = Convert.ToDateTime(dr["challana_date"]).ToString("dd/MMM/yyyy"),
                                pBsrCode = dr["bsr_code"],
                                pChallanaSNO = dr["challana_sl_no"],
                                pChallanaBank = dr["challana_bank"],
                                pReferenceNo = dr["reference_no"],
                                pPaidBank = dr["paid_bank"],
                                pFromdate = dr["from_date"],
                                pTodate = dr["to_date"],
                                pSection = dr["section"],
                                pTdsAmount=Convert.ToDecimal(dr["calc_tds"])
                            };
                            _CinEntryReportDTO.Add(_CinEntryReportDTO1);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _CinEntryReportDTO;
        }

        public List<ChallanaNoDTO> GetCinEntryChallanaNumbers(string connectionstring)
        {
            List<ChallanaNoDTO> lstChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct  b.challana_no,b.challana_id from cin_entry a join challana_entry b on a.challana_id=b.challana_id order by b.challana_id"))
                {
                    while (dr.Read())
                    {
                        ChallanaNoDTO objChallanaNumbers = new ChallanaNoDTO()
                        {
                            pChallanaId = dr["challana_id"],
                            pChallanaNo = dr["challana_no"],
                            
                        };
                        lstChallanaNumbers.Add(objChallanaNumbers);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaNumbers;
        }

        public ChallanaPaymentReportDTO GetChallanaPaymentReport(string connectionstring, string ChallanaNO)
        {
            ChallanaPaymentReportDTO lstChallanaPaymentReport = new ChallanaPaymentReportDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct c.challana_no,tc.tan_number,c.company_type,tc.nameofenterprise as Companyname,(tcd.address1 || tcd.address2 || tcd.district ||','|| tcd.city||','||tcd.state||' - ' ||tcd.pincode)address,tv.section,c.actual_tds,date_part('year',current_date) as currentyear,date_part('year',current_date)+1 as nextyear from challana_entry c  join tblmstcompany tc on tc.companyid=c.companyid join challana_entry_details cd on cd.challana_id= c.challana_id left join tblmstcompanyaddressdetails tcd on tcd.companyid=tc.companyid and tcd.priority='PRIMARY' join tds_vouchers tv on tv.tds_voucher_id=cd.tds_voucher_id where c.challana_no='"+ ChallanaNO + "' "))
                {
                    while (dr.Read())
                    {
                        lstChallanaPaymentReport = new ChallanaPaymentReportDTO()
                        {
                            pChallanaNo = dr["challana_no"],
                            pTanNo = dr["tan_number"],
                            pCompanyType = dr["company_type"],
                            pCompanyName = dr["Companyname"],
                            pAddress = dr["address"],
                            pSection = dr["section"],
                            pActualTdsAmount =Convert.ToDecimal(dr["actual_tds"]),
                            pCurrentYear = dr["currentyear"],
                            pNextYear = dr["nextyear"],

                        };

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstChallanaPaymentReport;
        }
        #endregion
    }
}
