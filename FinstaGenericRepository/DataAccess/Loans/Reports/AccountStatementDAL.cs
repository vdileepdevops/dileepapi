using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Reports;
using HelperManager;
using Npgsql;

namespace FinstaRepository.DataAccess.Loans.Reports
{
    public class AccountStatementDAL : SettingsDAL, IAccountStatement
    {
        public AccountStatementDTO _AccountStatementDTO = null;
        List<Transactiondetails> _TransactiondetailsList { get; set; }
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlDataReader dr = null;
        NpgsqlTransaction trans = null;
        public async Task<AccountStatementDTO> GetAccountstatementReport(string ConnectionString, string VchapplicationID)
        {
            await Task.Run(() =>
            {
                try
                {
                    VchapplicationID = ManageQuote(VchapplicationID).Trim().ToUpper();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select applicationid, fathername,contactnumber,emailid,contactaddress,loanaccountno,applicantname,loanstatus,approvedloanamount,installmentstartdate,loantype,loanname,tenureofloan,loanpayin,rateofinterest,lastinstalmentdate,installmentamount,disbursementdate,aadharno,panno ,titlename,contactimagepath,loaninstalmentpaymentmode,loancloseddate from VWACCOUNTSTATEMENT_LOAN_CONTACT_DETAILS where upper(loanaccountno)='" + VchapplicationID + "';"))
                    {
                        while (dr.Read())
                        {
                            _AccountStatementDTO = new AccountStatementDTO
                            {
                                pApplicationid = Convert.ToInt64(dr["applicationid"]),
                                pFathername = Convert.ToString(dr["fathername"]),
                                pMobileno = Convert.ToString(dr["contactnumber"]),
                                pEmailid = Convert.ToString(dr["emailid"]),
                                pAddress = Convert.ToString(dr["contactaddress"]),
                                pVchapplicationid = Convert.ToString(dr["loanaccountno"]),
                                pName = Convert.ToString(dr["applicantname"]),
                                pCurrentStatus = Convert.ToString(dr["loanstatus"]),
                                pLoanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pFirstinstalmentdate = dr["installmentstartdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["installmentstartdate"]).ToString("dd/MM/yyyy"),
                                pLoantype = Convert.ToString(dr["loantype"]),
                                pLoanname = Convert.ToString(dr["loanname"]),
                                pTenure = Convert.ToString(dr["tenureofloan"]),
                                pLoanpayin = Convert.ToString(dr["loanpayin"]),
                                pInterest = Convert.ToString(dr["rateofinterest"]),
                                pLastinstalmentdate = dr["lastinstalmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["lastinstalmentdate"]).ToString("dd/MM/yyyy"),
                                pInstalmentamount = Convert.ToDecimal(dr["installmentamount"]),
                                pDisbursedDate = dr["disbursementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["disbursementdate"]).ToString("dd/MM/yyyy"),
                                pAadharno = Convert.ToString(dr["aadharno"]),
                                pPanNo = Convert.ToString(dr["panno"]),
                                pTitlename = Convert.ToString(dr["titlename"]),
                                pContactImagePath = Convert.ToString(dr["contactimagepath"]),
                                ploaninstalmentpaymentmode = Convert.ToString(dr["loaninstalmentpaymentmode"]),
                                pLoanclosedDate = dr["loancloseddate"] == DBNull.Value ? null : Convert.ToDateTime(dr["loancloseddate"]).ToString("dd/MM/yyyy"),
                                pDocumentstoredetails = getDocumentstoreDetails(ConnectionString, 0, VchapplicationID),
                                pTransactionListDetails = GetaccountstatementtransactionDetails(ConnectionString, VchapplicationID)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return _AccountStatementDTO;
        }
        public List<Transactiondetails> GetaccountstatementtransactionDetails(string ConnectionString, string VchapplicationID)
        {
            decimal Balance = 0;
            _TransactiondetailsList = new List<Transactiondetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccountstatement_transactions_details where upper(vchapplicationid)='" + VchapplicationID + "';"))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            var _Transactiondetails = new Transactiondetails();

                            if (dr["transdate"] != DBNull.Value)
                            {
                                _Transactiondetails.pTransDate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy");
                            }
                            _Transactiondetails.pTransno = Convert.ToString(dr["transactionno"]);
                            _Transactiondetails.pDebitorCredit = Convert.ToString(dr["debitorcredit"]);
                            _Transactiondetails.pDisburseamount = Convert.ToDecimal(dr["disburseamount"]);
                            _Transactiondetails.pParticulars = Convert.ToString(dr["transactiontype"]);
                            _Transactiondetails.pDebitamount = Convert.ToDecimal(dr["debitamount"]);
                            _Transactiondetails.pCreditamount = Convert.ToDecimal(dr["creditamount"]);
                            _TransactiondetailsList.Add(_Transactiondetails);
                        }

                        if (_TransactiondetailsList != null && _TransactiondetailsList.Count > 0)
                        {
                            for (int i = 0; i < _TransactiondetailsList.Count; i++)
                            {
                                if (i == 0)
                                {
                                    _TransactiondetailsList[i].pBalance = Balance = Balance + _TransactiondetailsList[i].pDisburseamount;
                                    _TransactiondetailsList[i].pBalanceCreditorDebit = "Dr";
                                }
                                else
                                {
                                    if (_TransactiondetailsList[i].pDebitorCredit == "Cr")
                                    {
                                        Balance = Balance + _TransactiondetailsList[i].pBalance - _TransactiondetailsList[i].pDisburseamount;
                                        _TransactiondetailsList[i].pBalance = Math.Abs(Balance);
                                        _TransactiondetailsList[i].pBalanceCreditorDebit = Balance > 0 ? "Dr" : "Cr";
                                    }
                                    else if (_TransactiondetailsList[i].pDebitorCredit == "Dr")
                                    {
                                        Balance = Balance + _TransactiondetailsList[i].pBalance + _TransactiondetailsList[i].pDisburseamount;
                                        _TransactiondetailsList[i].pBalance = Math.Abs(Balance);
                                        _TransactiondetailsList[i].pBalanceCreditorDebit = Balance > 0 ? "Dr" : "Cr";
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return _TransactiondetailsList;
        }
    }
}
