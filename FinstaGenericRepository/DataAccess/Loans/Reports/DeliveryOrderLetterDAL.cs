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
    public class DeliveryOrderLetterDAL : SettingsDAL,IDeliveryOrderLetter
    {
        public DeliveryOrderLetterDTO _DeliveryOrderLetterDTO = null;
        public List<DeliveryOrderLetterDTO> _DeliveryOrderLetterListDTO = null;
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);      
        NpgsqlTransaction trans = null;
        public DeliveryOrdersCount _DeliveryOrdersCount = null;
        public List<DeliveryOrdersCount> _DeliveryOrdersCountListDTO = null;

        public  async Task<DeliveryOrderLetterDTO> GetDeliveryOrderLetterData(string ConnectionString, string VchapplicationID)
        {            
            await Task.Run(() =>
            {
                try
                {
                    VchapplicationID = ManageQuote(VchapplicationID).Trim().ToUpper();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select applicationid,applicationdate,vchapplicationid,applicantname,loantype,loanname,approveddate,approvedloanamount,tenureofloan,loanpayin,interesttype,installmentamount,rateofinterest,downpayment,onroadprice,requestedamount,Vehiclemodel,engineno,chasisno,registrationno,yearofmake,fathername,address1,address2,state,district,pincode , titlename,contacttype from vwdeliveryorderletterdetails where upper(vchapplicationid)='" + VchapplicationID + "';"))
                    {
                        while (dr.Read())
                        {
                            _DeliveryOrderLetterDTO = new DeliveryOrderLetterDTO
                            {
                                pApplicationId = Convert.ToInt64(dr["applicationid"]),
                                pVchapplicationID = Convert.ToString(dr["vchapplicationid"]).Trim().ToUpper(),
                                pApplicationdate = dr["applicationdate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["applicationdate"]).ToString("dd/MM/yyyy"),
                                pApplicantname = Convert.ToString(dr["applicantname"]).Trim().ToUpper(),
                                pLoantype = Convert.ToString(dr["loantype"]).Trim().ToUpper(),
                                pLoanname = Convert.ToString(dr["loanname"]).Trim().ToUpper(),                                
                                pApprovedDate = dr["approveddate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["approveddate"]).ToString("dd/MM/yyyy"),
                                pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pTenureofloan = dr["tenureofloan"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["tenureofloan"]),
                                pLoanpayin = Convert.ToString(dr["loanpayin"]).Trim().ToUpper(),
                                pInteresttype = Convert.ToString(dr["interesttype"]),                              
                                pInstallmentamount = dr["installmentamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["installmentamount"]),
                                pInterestRate = Convert.ToDecimal(dr["rateofinterest"]),
                                pDownpayment = Convert.ToDecimal(dr["downpayment"]),
                                pOnroadprice = Convert.ToDecimal(dr["onroadprice"]),
                                pRequestedamount = Convert.ToDecimal(dr["requestedamount"]),
                                pVehiclemodel = Convert.ToString(dr["Vehiclemodel"]).Trim().ToUpper(),
                                pengineno = Convert.ToString(dr["engineno"]).Trim().ToUpper(),
                                pchasisno = Convert.ToString(dr["chasisno"]).Trim().ToUpper(),
                                pregistrationno = Convert.ToString(dr["registrationno"]).Trim().ToUpper(),
                                pyearofmake = Convert.ToString(dr["yearofmake"]),
                                pfathername = Convert.ToString(dr["fathername"]).Trim().ToUpper(),
                                paddress1 = Convert.ToString(dr["address1"]).Trim(),
                                paddress2 = Convert.ToString(dr["address2"]).Trim(),
                                pstate = Convert.ToString(dr["state"]).Trim(),
                                pdistrict = Convert.ToString(dr["district"]).Trim(),
                                ppincode = Convert.ToString(dr["pincode"]),
                                pTitlename = Convert.ToString(dr["titlename"]),
                                pContacttype = Convert.ToString(dr["contacttype"]),

                            };
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DeliveryOrderLetterDTO;
        }

        public async Task<List<DeliveryOrderLetterDTO>> GetDeliveryOrderLetterMainData(string ConnectionString, string Letterstatus)
        {
            _DeliveryOrderLetterListDTO = new List<DeliveryOrderLetterDTO>();
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
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.applicantid,tap.vchapplicationid,applicantname,approvedloanamount,businessentitycontactno as contactnumber,businessentityemailid as emailid from tabapplication ta join tbltransapprovedapplications tap on ta.applicationid=tap.applicationid join tblmstcontact tc on tc.contactid=ta.applicantid  join tblmstloantypes tm on tm.loantypeid = ta.loantypeid JOIN tabapplicationvehicleloan tp ON tp.applicationid = ta.applicationid where  ta.vchapplicationid in (select vchapplicationid from tbltransapprovedapplications) and tap.vchapplicationid " + Strsanctionlettercondition + "(select vchapplicationid from tabapplicationdeliveryorderdetails where statusid = "+Convert.ToInt32(Status.Active)+ ") order by ta.applicantid desc; ; "))
                    {
                        while (dr.Read())
                        {
                            _DeliveryOrderLetterDTO = new DeliveryOrderLetterDTO
                            {
                                pVchapplicationID = Convert.ToString(dr["vchapplicationid"]),
                                pApplicantname = Convert.ToString(dr["applicantname"]),
                                pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pApplicantEmail = Convert.ToString(dr["emailid"]),
                                pApplicantMobileNo = Convert.ToString(dr["contactnumber"])
                            };
                            _DeliveryOrderLetterListDTO.Add(_DeliveryOrderLetterDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DeliveryOrderLetterListDTO;
        }

        public bool Savedeliveryorderletter(DeliveryOrderLetterDTO __DeliveryOrderLetter, string Connectionstring)
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
                Strsavesanctionletter = "INSERT INTO tabapplicationdeliveryorderdetails(applicationid, vchapplicationid, transdate, deliveryordersent,sentthrough, applicantname, loanname, loanamount, interesttype,rateofinterest, tenureofloan, tenuretype, downpayment, termsandconditions,statusid, createdby, createddate) VALUES(" + __DeliveryOrderLetter.pApplicationId + ", '" + ManageQuote(__DeliveryOrderLetter.pVchapplicationID) + "', current_date, 'Y','','" + ManageQuote(__DeliveryOrderLetter.pApplicantname) + "', '" + ManageQuote(__DeliveryOrderLetter.pLoanname) + "', " + __DeliveryOrderLetter.pApprovedloanamount + ", '" + ManageQuote(__DeliveryOrderLetter.pInteresttype) + "', " + __DeliveryOrderLetter.pInterestRate + ", " + __DeliveryOrderLetter.pTenureofloan + ", '" + __DeliveryOrderLetter.pLoanpayin + "', " + __DeliveryOrderLetter.pDownpayment + ", '', " + Convert.ToInt32(Status.Active) + ", " + __DeliveryOrderLetter.pCreatedby + ", current_timestamp); ";
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

        public async Task<List<DeliveryOrdersCount>> GetDeliveryOrdersCount(string ConnectionString)
        {
            _DeliveryOrdersCountListDTO = new List<DeliveryOrdersCount>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select 'NOT SENT' STATUS,count(*) totalcount from tbltransapprovedapplications tap join tabapplicationvehicleloan tv on tap.vchapplicationid=tv.vchapplicationid where tap.vchapplicationid not in (select vchapplicationid from tabapplicationdeliveryorderdetails) union select 'SENT' STATUS, count(*) from tbltransapprovedapplications tap join tabapplicationvehicleloan tv on tap.vchapplicationid = tv.vchapplicationid  where tap.vchapplicationid  in (select vchapplicationid from tabapplicationdeliveryorderdetails);"))
                    {
                        while (dr.Read())
                        {
                            _DeliveryOrdersCount = new DeliveryOrdersCount
                            {
                                pStatus = Convert.ToString(dr["STATUS"]),
                                pCount = Convert.ToInt64(dr["totalcount"])
                            };
                            _DeliveryOrdersCountListDTO.Add(_DeliveryOrdersCount);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _DeliveryOrdersCountListDTO;
        }
    }
}
