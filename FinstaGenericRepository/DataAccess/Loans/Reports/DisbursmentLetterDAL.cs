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
    public class DisbursmentLetterDAL : SettingsDAL, IDisbursmentLetter
    {
        public DisbursmentLetterDTO _DisbursmentLetterDTO = null;
        public DisbursalLetterCount _DisbursalLetterCounts = null;
        public List<DisbursmentLetterDTO> _DisbursmentLetterListDTO = null;
        public List<DisbursalLetterCount> _DisbursalLetterCountListDTO = null;
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        public async Task<List<DisbursalLetterCount>> GetDisbursementLettersCount(string ConnectionString)
        {
            _DisbursalLetterCountListDTO = new List<DisbursalLetterCount>();
            await Task.Run(() =>
            {
                try
                {

                    //select 'NOT SENT' STATUS,count(*) totalcount from tbltransloandisbursement where vchapplicationid not in (select vchapplicationid from tabapplicationdisbursementletter) and voucherno not in (select voucherno from tabapplicationdisbursementletter) union select 'SENT' STATUS, count(*) from tbltransloandisbursement where vchapplicationid  in (select vchapplicationid from tabapplicationdisbursementletter) and voucherno in (select voucherno from tabapplicationdisbursementletter);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select 'NOT SENT' STATUS,count(*) totalcount from tbltransloandisbursement where voucherno not in (select voucherno from tabapplicationdisbursementletter) union select 'SENT' STATUS, count(*) from tbltransloandisbursement where vchapplicationid  in (select vchapplicationid from tabapplicationdisbursementletter) and voucherno in (select voucherno from tabapplicationdisbursementletter);"))
                    {
                        while (dr.Read())
                        {
                            _DisbursalLetterCounts = new DisbursalLetterCount
                            {
                                pStatus = Convert.ToString(dr["STATUS"]),
                                pCount = Convert.ToInt64(dr["totalcount"])
                            };
                            _DisbursalLetterCountListDTO.Add(_DisbursalLetterCounts);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DisbursalLetterCountListDTO;
        }
        public async Task<List<DisbursmentLetterDTO>> GetDisbursalLetterMainData(string ConnectionString, string Letterstatus)
        {
            _DisbursmentLetterListDTO = new List<DisbursmentLetterDTO>();
            await Task.Run(() =>
            {
                try
                {
                    string Strsanctionlettercondition = string.Empty;
                    if (Letterstatus.Trim().ToUpper() == "N")
                    {
                        Strsanctionlettercondition = "not in";
                    }
                    else if (Letterstatus.Trim().ToUpper() == "Y")
                    {
                        Strsanctionlettercondition = "in";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  ta.applicantid,tap.vchapplicationid,applicantname,approvedloanamount,businessentitycontactno as contactnumber,businessentityemailid as emailid,voucherno from tabapplication ta join tbltransapprovedapplications tap on ta.applicationid=tap.applicationid join tbltransloandisbursement td on td.vchapplicationid=tap.vchapplicationid join tblmstcontact tc on tc.contactid=ta.applicantid where  ta.vchapplicationid in (select vchapplicationid from tbltransloandisbursement) and  voucherno " + Strsanctionlettercondition + " (select voucherno from tabapplicationdisbursementletter where statusid = 1) order by ta.applicantid desc;  "))
                    {
                        while (dr.Read())
                        {
                            _DisbursmentLetterDTO = new DisbursmentLetterDTO
                            {
                                pVchapplicationID = Convert.ToString(dr["vchapplicationid"]),
                                pApplicantname = Convert.ToString(dr["applicantname"]),
                                pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pApplicantEmail = Convert.ToString(dr["emailid"]),
                                pApplicantMobileNo = Convert.ToString(dr["contactnumber"]),
                                pVoucherno = Convert.ToString(dr["voucherno"])
                            };
                            _DisbursmentLetterListDTO.Add(_DisbursmentLetterDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DisbursmentLetterListDTO;
        }
        public bool SavedisbursalLetter(DisbursmentLetterDTO _DisbursmentLetterDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string StrsaveDisbursalletter = string.Empty;
                StrsaveDisbursalletter = "INSERT INTO tabapplicationdisbursementletter(applicationid, vchapplicationid, transdate, disbursementlettersent,sentthrough, applicantname, loanname, loanamount, interesttype,rateofinterest, tenureofloan, tenuretype, downpayment, termsandconditions,statusid, createdby, createddate,dibursementamount,modeofpayment,installmentamount,voucherno) VALUES(" + _DisbursmentLetterDTO.pApplicationId + ", '" + ManageQuote(_DisbursmentLetterDTO.pVchapplicationID) + "', current_date, 'Y','','" + ManageQuote(_DisbursmentLetterDTO.pApplicantname) + "', '" + ManageQuote(_DisbursmentLetterDTO.pLoanname) + "', " + _DisbursmentLetterDTO.pApprovedloanamount + ", '" + ManageQuote(_DisbursmentLetterDTO.pInteresttype) + "', " + _DisbursmentLetterDTO.pInterestRate + ", " + _DisbursmentLetterDTO.pTenureofloan + ", '" + _DisbursmentLetterDTO.pLoanpayin + "', " + _DisbursmentLetterDTO.pDownpayment + ", '', " + Convert.ToInt32(Status.Active) + ", " + _DisbursmentLetterDTO.pCreatedby + ", current_timestamp," + _DisbursmentLetterDTO.pDisbursalamount + ",'" + ManageQuote(_DisbursmentLetterDTO.pModeofpayment) + "'," + _DisbursmentLetterDTO.pInstallmentamount + ",'" + ManageQuote(_DisbursmentLetterDTO.pVoucherno) + "'); ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, StrsaveDisbursalletter);
                trans.Commit();
                Issaved = true;
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
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
        public async Task<DisbursmentLetterDTO> GetDisbursalLetterData(string Connectionstring, string vchapplicationId, string Voucherno)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select applicationid,applicationdate,vchapplicationid,applicantname,loantype,loanname,approveddate,approvedloanamount,tenureofloan,loanpayin,interesttype,installmentamount,rateofinterest,downpayment,applicantid,totaldisburseamount,disbursementdate,voucherno,modeofpayment,disbursedby,contactnumber,address1,state,district,city,pincode,titlename from vwdisbursalletter where upper(vchapplicationid)='" + ManageQuote(vchapplicationId).Trim().ToUpper() + "' and upper(voucherno)='" + ManageQuote(Voucherno).Trim().ToUpper() + "';"))
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                _DisbursmentLetterDTO = new DisbursmentLetterDTO
                                {
                                    pApplicationId = Convert.ToInt64(dr["applicationid"]),
                                    pApplicationdate = dr["applicationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["applicationdate"]).ToString("dd/MM/yyyy"),
                                    pVchapplicationID = Convert.ToString(dr["vchapplicationid"]).Trim().ToUpper(),
                                    pApplicantname = Convert.ToString(dr["applicantname"]).Trim(),
                                    pLoantype = Convert.ToString(dr["loantype"]).Trim(),
                                    pLoanname = Convert.ToString(dr["loanname"]).Trim(),
                                    pApprovedDate = dr["approveddate"] == DBNull.Value ? null : Convert.ToDateTime(dr["approveddate"]).ToString("dd/MM/yyyy"),
                                    pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                    pTenureofloan = dr["tenureofloan"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["tenureofloan"]),
                                    pLoanpayin = Convert.ToString(dr["loanpayin"]),
                                    pInteresttype = Convert.ToString(dr["interesttype"]),
                                    pInstallmentamount = dr["installmentamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["installmentamount"]),
                                    pInterestRate = dr["rateofinterest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["rateofinterest"]),
                                    pDownpayment = dr["downpayment"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["downpayment"]),
                                    pApplicantId = Convert.ToInt64(dr["applicantid"]),
                                    pDisbursalamount = dr["totaldisburseamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),
                                    pDisbursedDate = Convert.ToDateTime(dr["disbursementdate"]).ToString("dd/MM/yyyy"),
                                    pVoucherno = Convert.ToString(dr["voucherno"]).Trim().ToUpper(),
                                    pModeofpayment = Convert.ToString(dr["modeofpayment"]).Trim().ToUpper(),
                                    pDisbursedby = Convert.ToString(dr["disbursedby"]).Trim().ToUpper(),
                                    pApplicantMobileNo = Convert.ToString(dr["contactnumber"]),
                                    pApplicantaddress1 = Convert.ToString(dr["address1"]).Trim(),
                                    pApplicantState = Convert.ToString(dr["state"]).Trim(),
                                    pApplicantdistrict = Convert.ToString(dr["district"]).Trim(),
                                    pApplicantcity = Convert.ToString(dr["city"]).Trim(),
                                    pApplicantpincode = Convert.ToString(dr["pincode"]),
                                    pTitlename = Convert.ToString(dr["titlename"])
                                };
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DisbursmentLetterDTO;
        }
    }
}
