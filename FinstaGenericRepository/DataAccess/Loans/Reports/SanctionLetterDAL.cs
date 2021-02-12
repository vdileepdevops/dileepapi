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
    public class SanctionLetterDAL : SettingsDAL, ISanctionLetter
    {
        public SanctionLetter _SanctionLetter = null;
        public SanctionLetterCounts _SanctionLetterCounts = null;
        public List<SanctionLetter> _SanctionLetterListDTO = null;
        public List<SanctionLetterCounts> _SanctionLetterCountsListDTO = null;

        public List<SanctionLetterCoapplicants> _SanctionLetterCoapplicantsList { get; set; }
        public List<SanctionLetterguarantors> _SanctionLetterguarantorsList { get; set; }
        public List<SanctionLetterPromoters> _SanctionLetterPromotersList { get; set; }
        public List<SanctionLetterPartners> _SanctionLetterPartnersList { get; set; }
        public List<SanctionLetterGuardianorParent> _SanctionLetterGuardianorParentList { get; set; }
        public List<SanctionLetterJointowners> _SanctionLetterJointownersList { get; set; }

        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        public async Task<SanctionLetter> GetSanctionLetterData(string ConnectionString, string VchapplicationID)
        {
            await Task.Run(() =>
             {
                 try
                 {
                     VchapplicationID = ManageQuote(VchapplicationID).Trim().ToUpper();
                     using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select applicationid,applicationdate,vchapplicationid,applicantname,loantype,loanname,loantypeandcode,approveddate,approvedloanamount,tenureofloan,loanpayin,interesttype,installmentamount,rateofinterest,downpayment,titlename from vwsanctionletterloandetails where upper(vchapplicationid)='" + VchapplicationID + "';"))
                     {
                         while (dr.Read())
                         {
                             _SanctionLetter = new SanctionLetter
                             {
                                 pApplicationId = Convert.ToInt64(dr["applicationid"]),
                                 pVchapplicationID = Convert.ToString(dr["vchapplicationid"]).Trim().ToUpper(),
                                 pApplicationdate = dr["applicationdate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["applicationdate"]).ToString("dd/MM/yyyy"),
                                 pApplicantname = Convert.ToString(dr["applicantname"]),
                                 pLoantype = Convert.ToString(dr["loantype"]).Trim(),
                                 pLoanname = Convert.ToString(dr["loanname"]).Trim(),
                                 pLoantypeandCode = Convert.ToString(dr["loantypeandcode"]),
                                 pApprovedDate = dr["approveddate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["approveddate"]).ToString(),
                                 pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                 pTenureofloan = dr["tenureofloan"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["tenureofloan"]),
                                 pLoanpayin = Convert.ToString(dr["loanpayin"]).Trim().ToUpper(),
                                 pInteresttype = Convert.ToString(dr["interesttype"]),
                                 pInstallmentamount = dr["installmentamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["installmentamount"]),
                                 pInterestRate = Convert.ToDecimal(dr["rateofinterest"]),
                                 pDownpayment = Convert.ToDecimal(dr["downpayment"]),
                                 pChargesList = GetSanctionletterCharges(VchapplicationID, ConnectionString),
                                 pCoapplicantslist = GetSanctionLetterCoapplicants(ConnectionString, VchapplicationID),
                                 pGuarantorslist = GetSanctionLetterGuarantors(ConnectionString, VchapplicationID),
                                 pGuardianOrParentlist = GetSanctionLetterGuardianorParents(ConnectionString, VchapplicationID),
                                 pPromoterslist = GetSanctionLetterPromoters(ConnectionString, VchapplicationID),
                                 pPartnerslist = GetSanctionLetterPartners(ConnectionString, VchapplicationID),
                                 pJointOwnersList = GetSanctionLetterJointOwners(ConnectionString, VchapplicationID),
                                 pTitlename = Convert.ToString(dr["titlename"])
                             };
                         }
                     }
                 }
                 catch (Exception)
                 {
                     throw;
                 }
             });
            return _SanctionLetter;
        }
        public List<SanctionletterCharges> GetSanctionletterCharges(string vchapplicationid, string Connectionstring)
        {
            var pSanctionletterChargesList = new List<SanctionletterCharges>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select chargename,coalesce(chargesamount,0) chargesamount,chargestype from vwsanctionletterChargesdetails where upper(vchapplicationid) = '" + vchapplicationid + "'; "))
                {
                    while (dr.Read())
                    {
                        var _SanctionletterChargest = new SanctionletterCharges
                        {
                            pChargeAmount = dr["chargesamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["chargesamount"]),
                            pChargename = Convert.ToString(dr["chargename"]),
                            pChargetype = Convert.ToString(dr["chargestype"])
                        };
                        pSanctionletterChargesList.Add(_SanctionletterChargest);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pSanctionletterChargesList;
        }
        public async Task<List<SanctionLetter>> GetSanctionLetterMainData(string ConnectionString, string Letterstatus)
        {
            _SanctionLetterListDTO = new List<SanctionLetter>();
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
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tap.vchapplicationid,applicantname,approvedloanamount,businessentitycontactno as contactnumber,businessentityemailid as emailid,ta.applicantid from tabapplication ta join tbltransapprovedapplications tap on ta.applicationid=tap.applicationid join tblmstcontact tc on tc.contactid=ta.applicantid where ta.vchapplicationid in (select vchapplicationid from tbltransapprovedapplications) and tap.vchapplicationid " + Strsanctionlettercondition + " (select vchapplicationid from tabapplicationsanctionletter where statusid = " + Convert.ToInt32(Status.Active) + ") order by ta.applicantid desc; "))
                    {
                        while (dr.Read())
                        {
                            _SanctionLetter = new SanctionLetter
                            {
                                pVchapplicationID = Convert.ToString(dr["vchapplicationid"]).Trim().ToUpper(),
                                pApplicantname = Convert.ToString(dr["applicantname"]).Trim(),
                                pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pApplicantEmail = Convert.ToString(dr["emailid"]),
                                pApplicantMobileNo = Convert.ToString(dr["contactnumber"])
                            };
                            _SanctionLetterListDTO.Add(_SanctionLetter);
                        }
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _SanctionLetterListDTO;
        }
        public bool Savesanctionletter(SanctionLetter _SanctionLetter, string Connectionstring)
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
                string Strsavesanctionletter = string.Empty;
                Strsavesanctionletter = "INSERT INTO tabapplicationsanctionletter(applicationid, vchapplicationid, transdate, sanctionlettersent,sentthrough, applicantname, loanname, loanamount, interesttype,rateofinterest, tenureofloan, tenuretype, downpayment, termsandconditions,statusid, createdby, createddate) VALUES(" + _SanctionLetter.pApplicationId + ", '" + ManageQuote(_SanctionLetter.pVchapplicationID) + "', current_date, 'Y','','" + ManageQuote(_SanctionLetter.pApplicantname) + "', '" + ManageQuote(_SanctionLetter.pLoanname) + "', " + _SanctionLetter.pApprovedloanamount + ", '" + ManageQuote(_SanctionLetter.pInteresttype) + "', " + _SanctionLetter.pInterestRate + ", " + _SanctionLetter.pTenureofloan + ", '" + _SanctionLetter.pLoanpayin + "', " + _SanctionLetter.pDownpayment + ", '', " + Convert.ToInt32(Status.Active) + ", " + _SanctionLetter.pCreatedby + ", current_timestamp); ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Strsavesanctionletter);
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
        public async Task<List<SanctionLetterCounts>> GetSanctionLettersCount(string ConnectionString)
        {
            _SanctionLetterCountsListDTO = new List<SanctionLetterCounts>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select 'NOT SENT' STATUS,count(*) totalcount from tbltransapprovedapplications where vchapplicationid not in (select vchapplicationid from tabapplicationsanctionletter) union select 'SENT' STATUS, count(*) from tbltransapprovedapplications where vchapplicationid  in (select vchapplicationid from tabapplicationsanctionletter);"))
                    {
                        while (dr.Read())
                        {
                            _SanctionLetterCounts = new SanctionLetterCounts
                            {
                                pStatus = Convert.ToString(dr["STATUS"]),
                                pCount = Convert.ToInt64(dr["totalcount"])
                            };
                            _SanctionLetterCountsListDTO.Add(_SanctionLetterCounts);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _SanctionLetterCountsListDTO;
        }

        public List<SanctionLetterCoapplicants> GetSanctionLetterCoapplicants(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterCoapplicantsList = new List<SanctionLetterCoapplicants>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'CO%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var Coapps = new SanctionLetterCoapplicants
                        {
                            pTitle = "Co-applicant Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterCoapplicantsList.Add(Coapps);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterCoapplicantsList;
        }

        public List<SanctionLetterguarantors> GetSanctionLetterGuarantors(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterguarantorsList = new List<SanctionLetterguarantors>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'GUARANTOR%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var Guarantors = new SanctionLetterguarantors
                        {
                            pTitle = "Guarantor Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterguarantorsList.Add(Guarantors);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterguarantorsList;
        }

        public List<SanctionLetterPromoters> GetSanctionLetterPromoters(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterPromotersList = new List<SanctionLetterPromoters>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'PROMOTER%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var Promoters = new SanctionLetterPromoters
                        {
                            pTitle = "Promoter Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterPromotersList.Add(Promoters);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterPromotersList;
        }

        public List<SanctionLetterPartners> GetSanctionLetterPartners(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterPartnersList = new List<SanctionLetterPartners>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'PROMOTER%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var Partners = new SanctionLetterPartners
                        {
                            pTitle = "Promoter Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterPartnersList.Add(Partners);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterPartnersList;
        }

        public List<SanctionLetterGuardianorParent> GetSanctionLetterGuardianorParents(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterGuardianorParentList = new List<SanctionLetterGuardianorParent>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'GUARDIAN%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var GuardianorParents = new SanctionLetterGuardianorParent
                        {
                            pTitle = "Guardian/Parent Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterGuardianorParentList.Add(GuardianorParents);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterGuardianorParentList;
        }

        public List<SanctionLetterJointowners> GetSanctionLetterJointOwners(string Connectionstring, string LoanaccountNo)
        {
            _SanctionLetterJointownersList = new List<SanctionLetterJointowners>();
            int count = 0;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactname,fathername from tabapplicationsurietypersonsdetails ts join tblmstcontact tc on ts.contactid=tc.contactid where  upper(vchapplicationid) = '" + LoanaccountNo + "' and upper(surityapplicanttype) like 'JOINT%' and ts.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        count = count + 1;
                        var GuardianorParents = new SanctionLetterJointowners
                        {
                            pTitle = "Joint Owner Name " + Convert.ToString(count),
                            pName = Convert.ToString(dr["contactname"]).Trim(),
                            pFathername = Convert.ToString(dr["fathername"]).Trim()
                        };
                        _SanctionLetterJointownersList.Add(GuardianorParents);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SanctionLetterJointownersList;
        }
    }
}
