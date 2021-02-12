using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class RDTransactionDAL : SettingsDAL, IRDTransaction
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        ShareApplicationDAL objshareApplicationDAL { get; set; }
        public async Task<List<NomineeDetails>> GetMemberNomineeDetails(string MemberCode, string ConnectionString)
        {
            List<NomineeDetails> _NomineeDetailsList = new List<NomineeDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select vchapplicationid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, statusid,coalesce(percentage,0) as percentage from tabapplicationpersonalnomineedetails where vchapplicationid='" + ManageQuote(MemberCode) + "' and (applicantype='MEMBER' or applicantype='Member') and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _NomineeDetails = new NomineeDetails
                            {
                                pnomineename = Convert.ToString(dr["nomineename"]),
                                prelationship = Convert.ToString(dr["relationship"]),
                                pMemberrefcode = Convert.ToString(dr["vchapplicationid"]),
                                pcontactno = Convert.ToString(dr["contactno"]),
                                pidproofname = Convert.ToString(dr["idproofname"]),
                                pdocidproofpath = Convert.ToString(dr["docidproofpath"]),
                                preferencenumber = Convert.ToString(dr["referencenumber"]),
                                pisprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                                pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false,
                                pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                                pAge = dr["dateofbirth"] != DBNull.Value ? CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"])) : 0,
                                ptypeofoperation = "CREATE",
                                pidprooftype = Convert.ToString(dr["idprooftype"]),
                                pPercentage = Convert.ToDecimal(dr["percentage"])
                                // pRecordid=Convert.ToInt64(dr["recordid"])
                            };
                            _NomineeDetailsList.Add(_NomineeDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _NomineeDetailsList;
        }
        public async Task<List<RdConfigartionSchemesDTO>> GetRDConfigurationDetails(long RdRecordid, string ConnectionString)
        {
            List<RdConfigartionSchemesDTO> lstRdConfigartionDetails = new List<RdConfigartionSchemesDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,rdconfigid,rdname,membertypeid,membertype,applicanttype,rdcalculationmode,coalesce(mininstallmentamount,0) as mininstallmentamount,coalesce(maxinstallmentamount,0) as maxinstallmentamount,coalesce(installmentpayin,'') as installmentpayin,coalesce(investmentperiodfrom,'') as investmentperiodfrom,coalesce(investmentperiodto,'') as investmentperiodto,coalesce(interestpayout,'') as interestpayout,coalesce(interesttype,'') as interesttype,coalesce(compoundinteresttype,'') as compoundinteresttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,coalesce(valueper100,0) as valueper100,statusid,tenure,tenuremode,Payindenomination,interestamount,depositamount,maturityamount from tblmstrecurringdepositconfigdetails where  recordid=" + RdRecordid + " and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            RdConfigartionSchemesDTO RdConfigartionDetailsDTO = new RdConfigartionSchemesDTO();
                            RdConfigartionDetailsDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            RdConfigartionDetailsDTO.pMembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            RdConfigartionDetailsDTO.pMembertype = Convert.ToString(dr["membertype"]);
                            RdConfigartionDetailsDTO.pApplicanttype = Convert.ToString(dr["applicanttype"]);
                            RdConfigartionDetailsDTO.pRdcalucationmode = Convert.ToString(dr["rdcalculationmode"]);
                            RdConfigartionDetailsDTO.pMininstalmentamount = Convert.ToDecimal(dr["mininstallmentamount"]);
                            RdConfigartionDetailsDTO.pMaxinstalmentamount = Convert.ToDecimal(dr["maxinstallmentamount"]);
                            RdConfigartionDetailsDTO.pInvestmentperiodfrom = Convert.ToString(dr["investmentperiodfrom"]);
                            RdConfigartionDetailsDTO.pInvestmentperiodto = Convert.ToString(dr["investmentperiodto"]);
                            RdConfigartionDetailsDTO.pInterestpayuot = Convert.ToString(dr["interestpayout"]);
                            RdConfigartionDetailsDTO.pInteresttype = Convert.ToString(dr["interesttype"]);
                            RdConfigartionDetailsDTO.pCompoundInteresttype = Convert.ToString(dr["compoundinteresttype"]);
                            RdConfigartionDetailsDTO.pInterestratefrom = Convert.ToDecimal(dr["interestratefrom"]);
                            RdConfigartionDetailsDTO.pInterestrateto = Convert.ToDecimal(dr["interestrateto"]);
                            RdConfigartionDetailsDTO.pValueper100 = Convert.ToDecimal(dr["valueper100"]);
                            RdConfigartionDetailsDTO.pTenure = Convert.ToDecimal(dr["tenure"]);
                            RdConfigartionDetailsDTO.pTenuremode = Convert.ToString(dr["tenuremode"]);
                            RdConfigartionDetailsDTO.pPayindenomination = Convert.ToDecimal(dr["Payindenomination"]);
                            RdConfigartionDetailsDTO.pInterestamount = Convert.ToDecimal(dr["interestamount"]);
                            RdConfigartionDetailsDTO.pDepositamount = Convert.ToDecimal(dr["depositamount"]);
                            RdConfigartionDetailsDTO.pMaturityamount = Convert.ToDecimal(dr["maturityamount"]);
                            lstRdConfigartionDetails.Add(RdConfigartionDetailsDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstRdConfigartionDetails;
        }
        public async Task<List<RdMembersandContactDetails>> GetRDMembers(string Contacttype, string MemberType, string ConnectionString)
        {
            List<RdMembersandContactDetails> _RdMembersList = new List<RdMembersandContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,TC.contactid,TC.contactreferenceid, businessentitycontactno as mobileno from tblmstmembers TM join tblmstcontact TC on TC.contactid = TM.contactid where TC.contacttype ='" + Contacttype + "' and membertype = '" + MemberType + "' and TC.statusid = " + Convert.ToInt32(Status.Active) + " order by membername;"))
                    {
                        while (dr.Read())
                        {
                            var _RdMembersandContactDetails = new RdMembersandContactDetails
                            {
                                pMemberCode = dr["membercode"],
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberName = dr["membername"],
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactrefid = dr["contactreferenceid"],
                                pContactnumber = dr["mobileno"]
                            };
                            _RdMembersList.Add(_RdMembersandContactDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdMembersList;
        }
        private string Getpayinnatureoftenuremode(string Tenuremode, string ConnectionString)
        {
            string payinnature = string.Empty;
            try
            {
                payinnature = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT initcap(payinnature) from tblmstloanpayin where upper(laonpayin) ='" + Tenuremode.ToUpper() + "';"));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return payinnature;
        }
        public async Task<RDdetailsFromScheme> GetRdSchemeDetails(string ApplicantType, string MemberType, long RdconfigID, string Rdname, long Tenure, string Tenuremode, decimal instalmentamount, string ConnectionString)
        {
            RDdetailsFromScheme _RDdetailsFromScheme = new RDdetailsFromScheme();
            await Task.Run(() =>
            {
                try
                {
                    DataSet ds = new DataSet();

                    DataTable ComputedTable = new DataTable();
                    ComputedTable.Columns.Add("Recordid");
                    ComputedTable.Columns.Add("Fromdays");
                    ComputedTable.Columns.Add("Todays");
                    ComputedTable.Columns.Add("Valid");

                    Tenuremode = Getpayinnatureoftenuremode(Tenuremode, ConnectionString);
                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select recordid,investmentperiodfrom,investmentperiodto from tblmstrecurringdepositConfigdetails where applicanttype = '" + ApplicantType + "' and membertype='" + MemberType + "' and rdconfigid=" + RdconfigID + " and rdname='" + Rdname + "' and statusid = " + Convert.ToInt32(Status.Active) + "  and (" + instalmentamount + " between mininstallmentamount and maxinstallmentamount);");
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string Strfrom = Convert.ToString(ds.Tables[0].Rows[i]["investmentperiodfrom"]);
                            string Strto = Convert.ToString(ds.Tables[0].Rows[i]["investmentperiodto"]);
                            double FromDays = 0;
                            double ToDays = 0;
                            bool IsValid = false;
                            long TenureParam = Tenure;
                            if (!string.IsNullOrEmpty(Strfrom) && Strfrom.Contains(' '))
                            {
                                string[] StrfromDays = Strfrom.Split(' ');
                                if (StrfromDays.Length > 0)
                                {
                                    for (int fromdays = 0; fromdays < StrfromDays.Length; fromdays++)
                                    {
                                        if (StrfromDays[fromdays].Contains('Y'))
                                        {
                                            FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", "")) * 365;
                                        }
                                        else if (StrfromDays[fromdays].Contains('M'))
                                        {
                                            FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", "")) * 30;
                                        }
                                        else if (StrfromDays[fromdays].Contains('D'))
                                        {
                                            FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", ""));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Strfrom.Contains('Y'))
                                {
                                    FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", "")) * 365;
                                }
                                else if (Strfrom.Contains('M'))
                                {
                                    FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", "")) * 30;
                                }
                                else if (Strfrom.Contains('D'))
                                {
                                    FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", ""));
                                }
                            }
                            if (!string.IsNullOrEmpty(Strto) && Strto.Contains(' '))
                            {
                                string[] StrtoDays = Strto.Split(' ');
                                if (StrtoDays.Length > 0)
                                {
                                    for (int todays = 0; todays < StrtoDays.Length; todays++)
                                    {
                                        if (StrtoDays[todays].Contains('Y'))
                                        {
                                            ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", "")) * 365;
                                        }
                                        else if (StrtoDays[todays].Contains('M'))
                                        {
                                            ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", "")) * 30;
                                        }
                                        else if (StrtoDays[todays].Contains('D'))
                                        {
                                            ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", ""));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Strto.Contains('Y'))
                                {
                                    ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", "")) * 365;
                                }
                                else if (Strto.Contains('M'))
                                {
                                    ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", "")) * 30;
                                }
                                else if (Strto.Contains('D'))
                                {
                                    ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", ""));
                                }
                            }
                            TenureParam = Tenuremode == "Days" ? TenureParam : Tenuremode == "Months" ? TenureParam * 30 : Tenuremode == "Years" ? TenureParam * 365 : TenureParam;

                            if (TenureParam >= FromDays && TenureParam <= ToDays)
                            {
                                IsValid = true;
                            }
                            else
                            {
                                IsValid = false;
                            }
                            ComputedTable.Rows.Add(ds.Tables[0].Rows[i]["recordid"], FromDays, ToDays, IsValid);
                        }
                    }

                    long Recordid = 0;

                    string strRecordids = string.Empty;
                    DataRow[] rows = ComputedTable.Select("Valid = true");
                    foreach (DataRow dr in rows)
                    {
                        Recordid = Convert.ToInt64(dr["recordid"]);
                        if (string.IsNullOrEmpty(strRecordids))
                        {
                            strRecordids = Convert.ToString(dr["recordid"]);
                        }
                        else
                        {
                            strRecordids = strRecordids + "," + Convert.ToString(dr["recordid"]);
                        }
                    }

                    if (Recordid > 0)
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdcalculationmode, coalesce(mininstallmentamount,0) as mininstallmentamount,coalesce(maxinstallmentamount,0) as maxinstallmentamount,  investmentperiodfrom, investmentperiodto, interestpayout, interesttype, compoundinteresttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) interestrateto, coalesce(valueper100,0) as valueper100, coalesce(tenure,0) as tenure, tenuremode, coalesce(interestamount,0) as interestamount, coalesce(depositamount,0) as depositamount, coalesce(maturityamount,0) as maturityamount, coalesce(payindenomination,0) as payindenomination,tc.rdname,tcd.rdcode,tcd.rdnamecode,case when coalesce(valueper100,0) <=0 then 'Interestrate' else 'Valueper100' end as InterestorValue,coalesce(multiplesof,0) as multiplesof,tc.referralcommissiontype,coalesce(tc.commissionvalue,0) as commissionvalue,caltype from tblmstrecurringdepositConfigdetails  tc join tblmstrecurringdepositconfig tcd on tc.rdconfigid=tcd.rdconfigid  where tc.recordid = " + Recordid + " and tc.statusid = " + Convert.ToInt32(Status.Active) + " order by recordid ; "))
                        {
                            if (dr.Read())
                            {
                                _RDdetailsFromScheme.pReferralcommisiontype = dr["referralcommissiontype"];
                                _RDdetailsFromScheme.pReferralCommsionvalue = Convert.ToDecimal(dr["commissionvalue"]);
                                _RDdetailsFromScheme.pRdAmount = Convert.ToDecimal(dr["depositamount"]);
                                _RDdetailsFromScheme.pMinInstallmentAmount = Convert.ToInt64(dr["mininstallmentamount"]);
                                _RDdetailsFromScheme.pMaxInstallmentAmount = Convert.ToDecimal(dr["maxinstallmentamount"]);
                                _RDdetailsFromScheme.pInvestmentPeriodFrom = dr["investmentperiodfrom"];
                                _RDdetailsFromScheme.pInvestmentPeriodTo = dr["investmentperiodto"];
                                _RDdetailsFromScheme.pInterestType = dr["interesttype"];
                                _RDdetailsFromScheme.pInterestPayOut = dr["interestpayout"];
                                _RDdetailsFromScheme.pInterestRateFrom = Convert.ToDecimal(dr["interestratefrom"]);
                                _RDdetailsFromScheme.pInterestRateTo = Convert.ToDecimal(dr["interestrateto"]);
                                _RDdetailsFromScheme.pInterestOrValueForHundred = Convert.ToDecimal(dr["valueper100"]);
                                _RDdetailsFromScheme.pInterestTenureMode = dr["tenuremode"];
                                _RDdetailsFromScheme.pInterestTenure = Convert.ToInt64(dr["tenure"]);
                                _RDdetailsFromScheme.pInterestAmount = Convert.ToDecimal(dr["interestamount"]);
                                _RDdetailsFromScheme.pDepositAmount = Convert.ToDecimal(dr["depositamount"]);
                                _RDdetailsFromScheme.pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]);
                                _RDdetailsFromScheme.pPayinDenomination = Convert.ToDecimal(dr["payindenomination"]);
                                _RDdetailsFromScheme.pRdnameCode = dr["rdnamecode"];
                                _RDdetailsFromScheme.pRdname = dr["rdname"];
                                _RDdetailsFromScheme.pRdcode = dr["rdcode"];
                                _RDdetailsFromScheme.pValueORInterestratelabel = dr["InterestorValue"];
                                _RDdetailsFromScheme.pMultiplesof = Convert.ToDecimal(dr["multiplesof"]);

                                _RDdetailsFromScheme.RdInterestPayoutList = GetInterestPayoutsofScheme(strRecordids, RdconfigID, ConnectionString);

                                _RDdetailsFromScheme.pCaltype = Convert.ToString(dr["caltype"]);


                            }
                        }

                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RDdetailsFromScheme;
        }

        public List<RdInterestPayout> GetInterestPayoutsofScheme(string strRecordids, long RdconfigId, string Connectionstring)
        {
            List<RdInterestPayout> RdInterestPayoutList = new List<RdInterestPayout>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct interestpayout from tblmstrecurringdepositConfigdetails where Rdconfigid=" + RdconfigId + " and  statusid=" + Convert.ToInt32(Status.Active) + " and recordid in (" + strRecordids + ");"))

                {
                    while (dr.Read())
                    {
                        var _RdInterestPayoutdata = new RdInterestPayout
                        {
                            pInterestPayOut = dr["interestpayout"]
                        };
                        RdInterestPayoutList.Add(_RdInterestPayoutdata);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RdInterestPayoutList;
        }

        public async Task<List<RdNameandCode>> GetRdSchemes(string ApplicantType, string MemberType, string ConnectionString)
        {
            List<RdNameandCode> _RdNameandCodeList = new List<RdNameandCode>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct tc.rdconfigid,tc.rdname,tc.rdcode,tc.rdnamecode,rdcalculationmode from tblmstrecurringdepositconfig tc join tblmstrecurringdepositconfigdetails tcd on tc.rdconfigid=tcd.rdconfigid  where tcd.membertype='" + MemberType + "' and tcd.applicanttype='" + ApplicantType + "' and tc.statusid=" + Convert.ToInt32(Status.Active) + " order by tc.rdname; "))
                    {
                        while (dr.Read())
                        {
                            var _RdNameandCode = new RdNameandCode
                            {
                                pRdConfigId = Convert.ToInt64(dr["rdconfigid"]),
                                pRdname = dr["rdname"],
                                pRdcode = dr["rdcode"],
                                pRdnameCode = dr["rdnamecode"],
                                pRdCalculationmode = dr["rdcalculationmode"],
                                // pRdDetailsRecordid = Convert.ToInt64(dr["recordid"])
                            };
                            _RdNameandCodeList.Add(_RdNameandCode);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdNameandCodeList;
        }

        public bool SaveRDJointMembersandNomineeData(RdJointandNomineeSave RdJointandNomineeSave, string ConnectionString)
        {

            StringBuilder SbsaveReferences = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                StringBuilder sbUpdate = new StringBuilder();
                string RecordId = string.Empty;
                string JointRecordid = string.Empty;

                SbsaveReferences.AppendLine("Update tbltransrdcreation set isjointapplicable='" + RdJointandNomineeSave.pIsjointMembersapplicableorNot + "',isnomineesapplicable='" + RdJointandNomineeSave.pIsNomineesApplicableorNot + "' where rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "';");

                if (RdJointandNomineeSave.RdMembersandContactDetailsList != null && RdJointandNomineeSave.RdMembersandContactDetailsList.Count > 0)
                {
                    for (int i = 0; i < RdJointandNomineeSave.RdMembersandContactDetailsList.Count; i++)
                    {
                        if (Convert.ToString(RdJointandNomineeSave.RdMembersandContactDetailsList[i].pTypeofOperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(JointRecordid))
                            {
                                JointRecordid = Convert.ToString(RdJointandNomineeSave.RdMembersandContactDetailsList[i].precordid);
                            }
                            else
                            {
                                JointRecordid = JointRecordid + "," + Convert.ToString(RdJointandNomineeSave.RdMembersandContactDetailsList[i].precordid);
                            }
                        }
                    }

                    for (int i = 0; i < RdJointandNomineeSave.RdMembersandContactDetailsList.Count; i++)
                    {
                        if (Convert.ToString(RdJointandNomineeSave.RdMembersandContactDetailsList[i].pTypeofOperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("INSERT INTO tbltransrdjointdetails(rdaccountid, rdaccountno, memberid, membercode, membername,contactid, contacttype, contactreferenceid, statusid, createdby, createddate) VALUES (" + RdJointandNomineeSave.pRdAccountId + ", '" + RdJointandNomineeSave.pRdaccountNo + "', " + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberId + ", '" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberCode + "', '" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberName + "', " + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContactid + ",'" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContacttype + "', '" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContactrefid + "', " + Convert.ToInt32(Status.Active) + ", " + RdJointandNomineeSave.pCreatedby + ", current_timestamp);");
                            //SbsaveReferences.AppendLine("INSERT INTO tbltransjointdetails(accounttype, accountid, accountno, memberid, statusid, createdby, createddate) VALUES ('" + RdJointandNomineeSave.pAccountype + "', " + RdJointandNomineeSave.pRdAccountId + ", '" + RdJointandNomineeSave.pRdaccountNo + "', " + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberId + ", " + Convert.ToInt32(Status.Active) + ", " + RdJointandNomineeSave.pCreatedby + ", current_timestamp);");
                        }
                        else if (Convert.ToString(RdJointandNomineeSave.RdMembersandContactDetailsList[i].pTypeofOperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tbltransrdjointdetails set memberid=" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberId + ",membercode='" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberCode + "',membername='" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pMemberName + "',contactid=" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContactid + ",contacttype='" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContacttype + "',contactreferenceid='" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].pContactrefid + "',statusid=" + Convert.ToInt32(Status.Active) + " where rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "' and recordid=" + RdJointandNomineeSave.RdMembersandContactDetailsList[i].precordid + ";");

                        }
                    }
                    if (!string.IsNullOrEmpty(JointRecordid))
                    {
                        sbUpdate.AppendLine("UPDATE tbltransrdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "' AND RECORDID not in(" + JointRecordid + ") ; ");
                    }
                    else
                    {
                        if (RdJointandNomineeSave.RdMembersandContactDetailsList == null || RdJointandNomineeSave.RdMembersandContactDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tbltransrdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "' ; ");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(JointRecordid))
                {
                    sbUpdate.AppendLine("UPDATE tbltransrdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "' AND RECORDID not in(" + JointRecordid + ") ; ");
                }
                else
                {
                    if (RdJointandNomineeSave.RdMembersandContactDetailsList == null || RdJointandNomineeSave.RdMembersandContactDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tbltransrdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdaccountid=" + RdJointandNomineeSave.pRdAccountId + " and rdaccountno='" + RdJointandNomineeSave.pRdaccountNo + "'; ");
                    }
                }

                // Nominee Details
                if (RdJointandNomineeSave.RDMemberNomineeDetailsList != null && RdJointandNomineeSave.RDMemberNomineeDetailsList.Count > 0)
                {
                    for (int i = 0; i < RdJointandNomineeSave.RDMemberNomineeDetailsList.Count; i++)
                    {
                        if (Convert.ToString(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].ptypeofoperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(RecordId))
                            {
                                RecordId = Convert.ToString(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].precordid);
                            }
                            else
                            {
                                RecordId = RecordId + "," + Convert.ToString(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].precordid);
                            }
                        }
                    }
                    for (int i = 0; i < RdJointandNomineeSave.RDMemberNomineeDetailsList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth))
                        {
                            RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth = "null";
                        }
                        else
                        {
                            RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth = "'" + FormatDate(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth) + "'";
                        }


                        if (Convert.ToString(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].ptypeofoperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee,percentage) values ('" + RdJointandNomineeSave.pMemberId + "', '" + RdJointandNomineeSave.pRdaccountNo + "', " + RdJointandNomineeSave.pContactid + ", '" + RdJointandNomineeSave.pContactrefid + "', '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pnomineename + "', '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].prelationship + "', " + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth + ", '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pcontactno + "', '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pidprooftype + "', '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pidproofname + "', '" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].preferencenumber + "', '" +

                                RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdocidproofpath + "', " + Convert.ToInt32(Status.Active) + ", '" + RdJointandNomineeSave.pCreatedby + "', current_timestamp,'RD-MEMBER'," + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pisprimarynominee + "," + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pPercentage + ");");
                        }
                        else if (Convert.ToString(RdJointandNomineeSave.RDMemberNomineeDetailsList[i].ptypeofoperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tabapplicationpersonalnomineedetails set contactid=" + RdJointandNomineeSave.pContactid + ",contactreferenceid='" + RdJointandNomineeSave.pContactrefid + "',nomineename='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pnomineename + "',relationship='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].prelationship + "',dateofbirth=" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdateofbirth + ",contactno='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pcontactno + "',idprooftype='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pidprooftype + "',idproofname='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pidproofname + "',referencenumber='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].preferencenumber + "',docidproofpath='" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pdocidproofpath + "',statusid=" + Convert.ToInt32(Status.Active) + ",modifieddate=current_timestamp,modifiedby=" + RdJointandNomineeSave.pCreatedby + ",percentage=" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pPercentage + ",isprimarynominee=" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].pisprimarynominee + " where applicantype='RD-MEMBER' and applicationid=" + RdJointandNomineeSave.pMemberId + " and recordid=" + RdJointandNomineeSave.RDMemberNomineeDetailsList[i].precordid + ";  ");
                        }
                    }
                    if (!string.IsNullOrEmpty(RecordId))
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + RdJointandNomineeSave.pMemberId + " and vchapplicationid='" + RdJointandNomineeSave.pRdaccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='RD-MEMBER'; ");
                    }
                    else
                    {
                        if (RdJointandNomineeSave.RDMemberNomineeDetailsList == null || RdJointandNomineeSave.RDMemberNomineeDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + RdJointandNomineeSave.pMemberId + " and vchapplicationid='" + RdJointandNomineeSave.pRdaccountNo + "' and applicantype='RD-MEMBER'; ");
                        }
                    }

                }

                if (!string.IsNullOrEmpty(RecordId))
                {
                    sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + RdJointandNomineeSave.pMemberId + " and vchapplicationid='" + RdJointandNomineeSave.pRdaccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='RD-MEMBER'; ");
                }
                else
                {
                    if (RdJointandNomineeSave.RDMemberNomineeDetailsList == null || RdJointandNomineeSave.RDMemberNomineeDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + RdJointandNomineeSave.pMemberId + " and vchapplicationid='" + RdJointandNomineeSave.pRdaccountNo + "' and applicantype='RD-MEMBER'; ");
                    }
                }

                if (!string.IsNullOrEmpty(SbsaveReferences.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbUpdate) + " " + Convert.ToString(SbsaveReferences));
                    trans.Commit();
                    IsSaved = true;
                }
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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

        public string SaveRDMemberandSchemeData(RdMemberandSchemeSave RdMemberandSchemeData, string ConnectionString, out long pRdAccountId)
        {
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                // Next Id-Generation of Member Reference Id
                if (RdMemberandSchemeData.pSquareyard == null)
                {
                    RdMemberandSchemeData.pSquareyard = 0;
                }
                if (RdMemberandSchemeData.pInterestRate == null)
                {
                    RdMemberandSchemeData.pInterestRate = 0;
                }
                if (string.IsNullOrEmpty(RdMemberandSchemeData.pTransDate))
                {
                    RdMemberandSchemeData.pTransDate = "null";
                }
                else
                {
                    RdMemberandSchemeData.pTransDate = "'" + FormatDate(RdMemberandSchemeData.pTransDate) + "'";
                }
                if (string.IsNullOrEmpty(RdMemberandSchemeData.pMaturityDate))
                {
                    RdMemberandSchemeData.pMaturityDate = "null";
                }
                else
                {
                    RdMemberandSchemeData.pMaturityDate = "'" + FormatDate(RdMemberandSchemeData.pMaturityDate) + "'";
                }
                if (string.IsNullOrEmpty(RdMemberandSchemeData.pDepositDate))
                {
                    RdMemberandSchemeData.pDepositDate = "null";
                }
                else
                {
                    RdMemberandSchemeData.pDepositDate = "'" + FormatDate(RdMemberandSchemeData.pDepositDate) + "'";
                }
                if (Convert.ToString(RdMemberandSchemeData.pTypeofOperation) == "CREATE")
                {
                    if (string.IsNullOrEmpty(Convert.ToString(RdMemberandSchemeData.pRdAccountNo)) || Convert.ToString(RdMemberandSchemeData.pRdAccountNo) == "0")
                    {
                        RdMemberandSchemeData.pRdAccountNo = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('RECURRING DEPOSIT','" + RdMemberandSchemeData.pRdname + "'," + RdMemberandSchemeData.pTransDate + ")").ToString();
                    }
                    // Rd Transaction Save 1st and 2nd Tab
                    RdMemberandSchemeData.pRdAccountId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltransrdcreation(rdaccountno,transdate, membertypeid, membertype, memberid, applicanttype, membercode, membername, contactid,contacttype,contactreferenceid, rdconfigid, rdname, rdinstalmentpayin, tenor, depositamount,interesttype, compoundinteresttype, interestrate, maturityamount, interestpayable, deposidate, maturitydate, isinterestdepositinsaving,isautorenew, renewonlyprinciple, renewonlyprincipleinterest,bondprintstatus, accountstatus, tokenno,statusid, createdby,createddate,isjointapplicable,isreferralapplicable,instalmentamount,interestpayout , caltype , rdcalculationmode,tenortype) VALUES ('" + RdMemberandSchemeData.pRdAccountNo + "', " + RdMemberandSchemeData.pTransDate + ", " + RdMemberandSchemeData.pMembertypeId + ", '" + RdMemberandSchemeData.pMemberType + "'," + RdMemberandSchemeData.pMemberId + ", '" + RdMemberandSchemeData.pApplicantType + "', '" + RdMemberandSchemeData.pMemberCode + "', '" + RdMemberandSchemeData.pMemberName + "', " + RdMemberandSchemeData.pContactid + ", '" + RdMemberandSchemeData.pContacttype + "',  '" + RdMemberandSchemeData.pContactrefid + "', " + RdMemberandSchemeData.pRdConfigId + ", '" + RdMemberandSchemeData.pRdname + "', '" + RdMemberandSchemeData.pInterestTenureMode + "', " + RdMemberandSchemeData.pInterestTenure + ", " + RdMemberandSchemeData.pDepositAmount + ",'" + RdMemberandSchemeData.pInterestType + "', '" + RdMemberandSchemeData.pCompoundInterestType + "', " + RdMemberandSchemeData.pInterestRate + ", " + RdMemberandSchemeData.pMaturityAmount + "," + RdMemberandSchemeData.pInterestAmount + ", " + RdMemberandSchemeData.pDepositDate + ", " + RdMemberandSchemeData.pMaturityDate + ", " + RdMemberandSchemeData.pIsinterestDepositinSaving + "," + RdMemberandSchemeData.pIsAutoRenew + ", " + RdMemberandSchemeData.pIsRenewOnlyPrinciple + ", " + RdMemberandSchemeData.pIsRenewOnlyPrincipleandInterest + ",'N','N', '" + RdMemberandSchemeData.pTokenNo + "'," + Convert.ToInt32(Status.Active) + ", " + RdMemberandSchemeData.pCreatedby + ",current_timestamp," + RdMemberandSchemeData.pIsJointMembersapplicable + "," + RdMemberandSchemeData.pIsReferralsapplicable + "," + RdMemberandSchemeData.pinstallmentAmount + ",'" + RdMemberandSchemeData.pInterestPayOut + "','" + RdMemberandSchemeData.pRdCalculationmode + "','" + RdMemberandSchemeData.pRdCalculationmode + "','" + RdMemberandSchemeData.pTenortype + "') returning rdaccountid; "));
                }
                else if (Convert.ToString(RdMemberandSchemeData.pTypeofOperation) == "UPDATE")
                {
                    string strUpdate = "UPDATE tbltransrdcreation SET   rdinstalmentpayin ='" + RdMemberandSchemeData.pInterestTenureMode + "', tenor =" + RdMemberandSchemeData.pInterestTenure + ", depositamount =" + RdMemberandSchemeData.pDepositAmount + ", interesttype ='" + RdMemberandSchemeData.pInterestType + "', compoundinteresttype ='" + RdMemberandSchemeData.pCompoundInterestType + "', interestrate =" + RdMemberandSchemeData.pInterestRate + ", maturityamount =" + RdMemberandSchemeData.pMaturityAmount + ", interestpayable =" + RdMemberandSchemeData.pInterestAmount + ", deposidate =" + RdMemberandSchemeData.pDepositDate + ",maturitydate =" + RdMemberandSchemeData.pMaturityDate + ", isinterestdepositinsaving =" + RdMemberandSchemeData.pIsinterestDepositinSaving + ", isautorenew =" + RdMemberandSchemeData.pIsAutoRenew + ", renewonlyprinciple =" + RdMemberandSchemeData.pIsRenewOnlyPrinciple + ", renewonlyprincipleinterest =" + RdMemberandSchemeData.pIsRenewOnlyPrincipleandInterest + ",bondprintstatus ='N', accountstatus ='N', tokenno ='" + RdMemberandSchemeData.pTokenNo + "', statusid =" + Convert.ToInt32(Status.Active) + ",  modifiedby =" + RdMemberandSchemeData.pCreatedby + ", modifieddate =current_timestamp ,instalmentamount=" + RdMemberandSchemeData.pinstallmentAmount + ",interestpayout='" + RdMemberandSchemeData.pInterestPayOut + "' , caltype='" + RdMemberandSchemeData.pRdCalculationmode + "' , rdcalculationmode='" + RdMemberandSchemeData.pRdCalculationmode + "',tenortype='" + RdMemberandSchemeData.pTenortype + "' WHERE rdaccountid =" + RdMemberandSchemeData.pRdAccountId + " and rdaccountno ='" + RdMemberandSchemeData.pRdAccountNo + "'; ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strUpdate);
                }
                pRdAccountId = RdMemberandSchemeData.pRdAccountId;
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "select fn_rdinstalmentsinsert('" + RdMemberandSchemeData.pRdAccountNo + "');");
                trans.Commit();
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
            return Convert.ToString(RdMemberandSchemeData.pRdAccountNo);
        }

        public bool SaveRDReferralData(RdTransactionReferrals RdTransactionReferrals, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                StringBuilder SbsaveReferences = new StringBuilder();

                SbsaveReferences.AppendLine("update tbltransrdcreation set isreferralapplicable='" + RdTransactionReferrals.pIsReferralsapplicable + "' where rdaccountno='" + RdTransactionReferrals.pRdaccountNo + "';");

                if (RdTransactionReferrals.pIsReferralsapplicable)
                {
                    if (Convert.ToString(RdTransactionReferrals.pTypeofOperation) == "CREATE")
                    {
                        SbsaveReferences.AppendLine("INSERT INTO tbltransrdreferraldetails(rdaccountid, rdaccountno, referralid, referralname,salespersonname, commsssionvalue, statusid, createdby, createddate,commissiontype,referralcode,contactid) VALUES ( " + RdTransactionReferrals.pRdAccountId + ", '" + RdTransactionReferrals.pRdaccountNo + "', '" + RdTransactionReferrals.pReferralId + "', '" + RdTransactionReferrals.pAdvocateName + "', '" + RdTransactionReferrals.pSalesPersonName + "'," + RdTransactionReferrals.pCommisionValue + ", " + Convert.ToInt32(Status.Active) + ", " + RdTransactionReferrals.pCreatedby + ", current_timestamp,'" + RdTransactionReferrals.pCommissionType + "','" + RdTransactionReferrals.pReferralCode + "'," + RdTransactionReferrals.pContactId + ");");
                    }
                    else if (Convert.ToString(RdTransactionReferrals.pTypeofOperation) == "UPDATE")
                    {
                        SbsaveReferences.AppendLine("Update tbltransrdreferraldetails set referralid='" + RdTransactionReferrals.pReferralId + "',referralname='" + RdTransactionReferrals.pAdvocateName + "',salespersonname='" + RdTransactionReferrals.pSalesPersonName + "',commsssionvalue=" + RdTransactionReferrals.pCommisionValue + ",modifiedby=" + RdTransactionReferrals.pCreatedby + ",modifieddate=current_timestamp,commissiontype='" + RdTransactionReferrals.pCommissionType + "',referralcode='" + RdTransactionReferrals.pReferralCode + "',contactid=" + RdTransactionReferrals.pContactId + " where rdaccountid=" + RdTransactionReferrals.pRdAccountId + " and rdaccountno='" + RdTransactionReferrals.pRdaccountNo + "';");
                    }
                }
                if (!string.IsNullOrEmpty(SbsaveReferences.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbsaveReferences.ToString());
                    trans.Commit();
                    IsSaved = true;
                }
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
            return IsSaved;
        }


        public bool DeleteRdTransactions(string RdaccountNo, long Userid, string ConnectionString)
        {
            StringBuilder SbDeleteRdTransaction = new StringBuilder();
            bool IsDeleted = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                int Statusid = Convert.ToInt32(Status.Inactive);
                long RdAccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select Rdaccountid from tbltransRdcreation where Rdaccountno='" + RdaccountNo + "' ;"));

                // Nominee Details 
                SbDeleteRdTransaction.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  vchapplicationid='" + RdaccountNo + "' and applicationid=" + RdAccountid + " and upper(applicantype)='RD-MEMBER'; ");

                // Insurance Member Details
                SbDeleteRdTransaction.AppendLine("UPDATE tbltransRdjointdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where Rdaccountno='" + RdaccountNo + "'; ");

                SbDeleteRdTransaction.AppendLine("UPDATE tbltransRdreferraldetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where Rdaccountno='" + RdaccountNo + "'; ");

                SbDeleteRdTransaction.AppendLine("UPDATE tbltransRdcreation SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where Rdaccountno='" + RdaccountNo + "'; ");

                if (Convert.ToString(SbDeleteRdTransaction) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbDeleteRdTransaction.ToString());
                    trans.Commit();
                    IsDeleted = true;
                }
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
            return IsDeleted;
        }
        public async Task<List<RdTransactionMainGridData>> GetRdTransactionData(string ConnectionString)
        {
            List<RdTransactionMainGridData> _RdTransactionMainGridData = new List<RdTransactionMainGridData>();
            await Task.Run(() =>
            {
                try
                {
                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdaccountid,rdaccountno,membertype,applicanttype,rdname,membername, coalesce(depositamount,0) as depositamount, coalesce(maturityamount,0) as maturityamount,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy') maturitydate,(tenor ||' '|| tenortype)Tenure,interestpayout,(select (case when count(rd_account_id)> 0 then true else false end)ReceiptStatus from rd_receipt tt where tt.rd_account_id=t.rdaccountid and status=true)  from tbltransrdcreation t where statusid=" + Convert.ToInt32(Status.Active) + " order by rdaccountid desc;;"))

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdaccountid,rdaccountno,membertype,applicanttype,rdname,membername, coalesce(depositamount,0) as depositamount, coalesce(maturityamount,0) as maturityamount,to_char(deposidate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy') maturitydate,(tenor ||' '|| tenortype)Tenure,rdinstalmentpayin,(select (case when count(rd_account_id)> 0 then true else false end) ReceiptStatus from rd_receipt tt where tt.rd_account_id=t.rdaccountid)  from tbltransrdcreation t where statusid=" + Convert.ToInt32(Status.Active) + " order by rdaccountid desc;"))
                    {
                        while (dr.Read())
                        {
                            var _RdTransactionMainGridDataDetails = new RdTransactionMainGridData
                            {
                                pRdAccountId = Convert.ToInt64(dr["rdaccountid"]),
                                pRdaccountNo = dr["rdaccountno"],
                                pMemberType = dr["membertype"],
                                pApplicantType = dr["applicanttype"],
                                pRdname = dr["rdname"],
                                pMemberName = dr["membername"],
                                pReceiptStatus = dr["ReceiptStatus"],
                                pInstallmentAmount = Convert.ToDecimal(dr["depositamount"]),
                                pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]),
                                pDepositDate = Convert.ToString(dr["depositdate"]),
                                pMaturityDate = Convert.ToString(dr["maturitydate"]),
                                pTenure = dr["Tenure"],
                                //  pInterestPayout = dr["interestpayout"]
                            };
                            _RdTransactionMainGridData.Add(_RdTransactionMainGridDataDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactionMainGridData;
        }


        public async Task<RdTransactionDataEdit> GetRdTransactionDetailsforEdit(string RdAccountNo, long RdAccountId, string accounttype, string ConnectionString)
        {
            objshareApplicationDAL = new ShareApplicationDAL();
            RdTransactionDataEdit _RdTransactionDataEdit = new RdTransactionDataEdit();
            await Task.Run(() =>
            {
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT distinct transdate,ta.rdaccountid,ta.rdaccountno, ta.membertypeid, ta.membertype, memberid, ta.applicanttype, membercode, membername, ta.contactid, contacttype,contactreferenceid, ta.rdconfigid, tc.rdname, rdinstalmentpayin as tenortype, tenor,coalesce( ta.depositamount,0) as depositamount, ta.interesttype, ta.compoundinteresttype,coalesce( interestrate,0) as interestrate,coalesce( ta.maturityamount,0) as maturityamount , interestpayable,deposidate depositdate, maturitydate, isinterestdepositinsaving, isautorenew, renewonlyprinciple, renewonlyprincipleinterest, isjointapplicable, isreferralapplicable, bondprintstatus, accountstatus, tokenno,coalesce( tcd.commissionvalue,0) as commsssionvalue,ta.rdcalculationmode,tc.rdnamecode,tc.rdcode,ta.interestpayout, coalesce(isnomineesapplicable,false)as isnomineesapplicable,ta.interestpayout,referralcommissiontype as commissiontype,ta.caltype,instalmentamount,ta.tenortype FROM tbltransrdcreation ta join tblmstrecurringdepositConfig tc on ta.rdconfigid=tc.rdconfigid join tblmstrecurringdepositConfigdetails tcd on tc.rdconfigid=tcd.rdconfigid  where ta.statusid = " + Convert.ToInt32(Status.Active) + " and ta.rdaccountid=" + RdAccountId + "  and ta.rdaccountno='" + RdAccountNo + "' order by applicanttype desc; "))
                    {
                        if (dr.Read())
                        {
                            _RdTransactionDataEdit.pCommissionType = dr["commissiontype"];
                            _RdTransactionDataEdit.pTransDate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy");
                            _RdTransactionDataEdit.pRdAccountId = Convert.ToInt64(dr["rdaccountid"]);
                            _RdTransactionDataEdit.pRdAccountNo = dr["rdaccountno"];
                            _RdTransactionDataEdit.pMembertypeId = Convert.ToInt64(dr["membertypeid"]);
                            _RdTransactionDataEdit.pMemberType = dr["membertype"];
                            _RdTransactionDataEdit.pMemberId = Convert.ToInt64(dr["memberid"]);
                            _RdTransactionDataEdit.pApplicantType = dr["applicanttype"];
                            _RdTransactionDataEdit.pMemberCode = dr["membercode"];
                            _RdTransactionDataEdit.pMemberName = dr["membername"];
                            _RdTransactionDataEdit.pContactid = Convert.ToInt64(dr["contactid"]);
                            _RdTransactionDataEdit.pContacttype = dr["contacttype"];
                            _RdTransactionDataEdit.pContactrefid = dr["contactreferenceid"];
                            _RdTransactionDataEdit.pRdConfigId = Convert.ToInt64(dr["rdconfigid"]);
                            _RdTransactionDataEdit.pRdname = dr["rdname"];
                            _RdTransactionDataEdit.pRdnameCode = dr["rdnamecode"];
                            _RdTransactionDataEdit.pRdcode = dr["rdcode"];
                            _RdTransactionDataEdit.pInterestTenureMode = dr["tenortype"];
                            _RdTransactionDataEdit.pInterestTenure = Convert.ToInt64(dr["tenor"]);
                            _RdTransactionDataEdit.pDepositAmount = Convert.ToDecimal(dr["depositamount"]);
                            _RdTransactionDataEdit.pinstallmentAmount = Convert.ToDecimal(dr["instalmentamount"]);
                            _RdTransactionDataEdit.pInterestType = dr["interesttype"];
                            _RdTransactionDataEdit.pCompoundInterestType = dr["compoundinteresttype"];
                            _RdTransactionDataEdit.pInterestRate = Convert.ToDecimal(dr["interestrate"]);
                            _RdTransactionDataEdit.pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]);
                            _RdTransactionDataEdit.pInterestPayOut = dr["interestpayout"];

                            if (Convert.ToString(dr["depositdate"]) != string.Empty)
                            {
                                _RdTransactionDataEdit.pDepositDate = Convert.ToDateTime(dr["depositdate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                _RdTransactionDataEdit.pDepositDate = null;
                            }
                            if (Convert.ToString(dr["maturitydate"]) != string.Empty)
                            {
                                _RdTransactionDataEdit.pMaturityDate = Convert.ToDateTime(dr["maturitydate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                _RdTransactionDataEdit.pMaturityDate = null;
                            }
                            _RdTransactionDataEdit.pIsinterestDepositinSaving = Convert.ToBoolean(dr["isinterestdepositinsaving"]);
                            _RdTransactionDataEdit.pIsAutoRenew = Convert.ToBoolean(dr["isautorenew"]);
                            _RdTransactionDataEdit.pIsRenewOnlyPrinciple = Convert.ToBoolean(dr["renewonlyprinciple"]);
                            _RdTransactionDataEdit.pIsJointMembersapplicable = Convert.ToBoolean(dr["isjointapplicable"]);
                            _RdTransactionDataEdit.pIsReferralsapplicable = Convert.ToBoolean(dr["isreferralapplicable"]);
                            if (Convert.ToString(dr["bondprintstatus"]) == "false" || Convert.ToString(dr["bondprintstatus"]) == "N")
                            {
                                _RdTransactionDataEdit.pBondPrintStatus = false;
                            }
                            else
                            {
                                _RdTransactionDataEdit.pBondPrintStatus = false;
                            }
                            _RdTransactionDataEdit.pAccountStatus = dr["accountstatus"];
                            _RdTransactionDataEdit.pTokenNo = dr["tokenno"];
                            //_RdTransactionDataEdit.pReferralId = dr["referralid"];
                            //_RdTransactionDataEdit.pAdvocateName = dr["referralname"];
                            //_RdTransactionDataEdit.pSalesPersonName = dr["salespersonname"];
                            _RdTransactionDataEdit.pCommisionValue = Convert.ToDecimal(dr["commsssionvalue"]);
                            _RdTransactionDataEdit.JointMembersandContactDetailsList = objshareApplicationDAL.GetJointMembersListInEdit(RdAccountNo, accounttype, ConnectionString);
                            _RdTransactionDataEdit.MemberNomineeDetailsList = objshareApplicationDAL.GetNomineesListInEdit(RdAccountNo, accounttype, ConnectionString);
                            _RdTransactionDataEdit.MemberReferalDetails = objshareApplicationDAL.getReferralDetails(ConnectionString, RdAccountNo, accounttype);


                            _RdTransactionDataEdit.pChitbranchId = 0;
                            _RdTransactionDataEdit.pChitbranchname = "";
                            _RdTransactionDataEdit.pRdCalculationmode = dr["rdcalculationmode"];
                            _RdTransactionDataEdit.pTenortype = dr["tenortype"];
                            _RdTransactionDataEdit.pInterestPayOut = dr["interestpayout"];
                            _RdTransactionDataEdit.pIsinterestDepositinBank = false;
                            _RdTransactionDataEdit.pIsNomineesapplicable = Convert.ToBoolean(dr["isnomineesapplicable"]);
                            _RdTransactionDataEdit.pInterestAmount = Convert.ToDecimal(dr["interestpayable"]);
                            // _RdTransactionDataEdit.pSquareyard = Convert.ToDecimal(dr["squareyard"]);
                            _RdTransactionDataEdit.pCaltype = Convert.ToString(dr["caltype"]);
                            //_RdTransactionDataEdit.pReferralCode = dr["referralcode"];
                            //_RdTransactionDataEdit.pReferralContactId = dr["referralcontactid"];

                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactionDataEdit;
        }

        public List<RdMembersandContactDetails> GetJointMembersListofRdInEdit(string RdAccountNo, string ConnectionString)
        {
            var _RdMemberJointDetailsList = new List<RdMembersandContactDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, memberid,membercode,membername,contactid,contacttype,contactreferenceid from tbltransrdjointdetails  where  rdaccountno = '" + RdAccountNo + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdMembersandContactDetails _RdMemberJointDetails = new RdMembersandContactDetails
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pMemberCode = dr["membercode"],
                            pMemberName = dr["membername"],
                            pContactid = Convert.ToInt64(dr["contactid"]),
                            pContacttype = dr["contacttype"],
                            pContactrefid = dr["contactreferenceid"],
                            pTypeofOperation = "OLD"
                        };
                        _RdMemberJointDetailsList.Add(_RdMemberJointDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _RdMemberJointDetailsList;
        }

        public List<RDMemberNomineeDetails> GetNomineesListofRdInEdit(string RdAccountNo, string ConnectionString)
        {
            var _RdMemberNomineeDetailsList = new List<RDMemberNomineeDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,vchapplicationid,contactid,contactreferenceid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, applicantype,coalesce(percentage,0) as percentage,statusid from tabapplicationpersonalnomineedetails where vchapplicationid='" + RdAccountNo + "' and applicantype='RD-MEMBER' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        var _RdTransNomineeDetails = new RDMemberNomineeDetails
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pnomineename = Convert.ToString(dr["nomineename"]),
                            prelationship = Convert.ToString(dr["relationship"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pMemberrefcode = Convert.ToString(dr["vchapplicationid"]),
                            pcontactno = Convert.ToString(dr["contactno"]),
                            pidproofname = Convert.ToString(dr["idproofname"]),
                            pdocidproofpath = Convert.ToString(dr["docidproofpath"]),
                            preferencenumber = Convert.ToString(dr["referencenumber"]),
                            pisprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                            pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false,
                            pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                            pAge = dr["dateofbirth"] != DBNull.Value ? CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"])) : 0,
                            ptypeofoperation = "OLD",
                            pidprooftype = Convert.ToString(dr["idprooftype"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pPercentage = Convert.ToDecimal(dr["percentage"])
                        };
                        _RdMemberNomineeDetailsList.Add(_RdTransNomineeDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _RdMemberNomineeDetailsList;
        }


        public async Task<List<RdTransactinTenuresofTable>> GetRdTenuresofTable(string RDName, long RdconfigId, string TenureMode, string MemberType, string ConnectionString)
        {
            List<RdTransactinTenuresofTable> _RdTransactionMainGridDataList = new List<RdTransactinTenuresofTable>();
            await Task.Run(() =>
            {
                try
                {
                    //TenureMode = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct tenure from tblmstrecurringdepositconfigdetails where tenuremode='" + TenureMode + "' and rdconfigid=" + RdconfigId + " and rdname='" + RDName + "' and membertype='" + MemberType + "' and statusid=" + Convert.ToInt32(Status.Active) + " and coalesce(tenure,0)>0 order by tenure;"))
                    {
                        while (dr.Read())
                        {
                            var _RdTransactionMainGridData = new RdTransactinTenuresofTable
                            {
                                pInterestTenure = Convert.ToInt64(dr["tenure"])
                            };
                            _RdTransactionMainGridDataList.Add(_RdTransactionMainGridData);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactionMainGridDataList;
        }

        public async Task<List<RdTransactinInstallmentsAmountofTable>> GetRdInstallmentsamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, string MemberType, string ConnectionString)
        {
            List<RdTransactinInstallmentsAmountofTable> _RdTransactinInstallmentsAmountofTable = new List<RdTransactinInstallmentsAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    //TenureMode = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select payindenomination from tblmstrecurringdepositConfigdetails where tenuremode='" + TenureMode + "' and rdconfigid=" + RdconfigId + " and rdname='" + RDName + "' and membertype='" + MemberType + "'  and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + " and coalesce(depositamount,0)>0 order by payindenomination;"))
                    {
                        while (dr.Read())
                        {
                            var _rdTransactinDepositAmountofTable = new RdTransactinInstallmentsAmountofTable
                            {
                                pInstallmentAmount = Convert.ToDecimal(dr["payindenomination"])
                            };
                            _RdTransactinInstallmentsAmountofTable.Add(_rdTransactinDepositAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactinInstallmentsAmountofTable;
        }


        public List<RDMatuerityamount> GetRDMaturityamount(string pInterestMode, long pInterestTenure, decimal instalmentamount, string pInterestPayOut, string pCompoundorSimpleInterestType, decimal pInterestRate, string pCalType, string Connectionstring)
        {
            List<RDMatuerityamount> lstmaturity = new List<RDMatuerityamount>();
            try
            {
                pInterestMode = Getpayinnatureoftenuremode(pInterestMode, Connectionstring);
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from  FN_CALUCATE_FD_RD_INTEREST('RD','" + pInterestPayOut + "','" + pCompoundorSimpleInterestType + "',''," + pInterestTenure + ",'" + pInterestMode.ToUpper() + "'," + pInterestRate + "," + instalmentamount + ",'" + pCalType + "') ;"))
                    // Maturityamount = NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select * from  FN_CALUCATE_FD_RD_INTEREST('RD','"+ pInterestPayOut + "','"+ pCompoundorSimpleInterestType + "','',"+ pInterestTenure+ ",'"+ pInterestMode + "',"+ pInterestRate+ ","+ pDepositAmount+ ") ;").ToString();
                    while (dr.Read())
                    {
                        RDMatuerityamount objmamturityamount = new RDMatuerityamount();
                        objmamturityamount.pMatueritytAmount = Convert.ToDecimal(dr["MATURITYAMOUNT"]);
                        objmamturityamount.pInterestamount = Convert.ToDecimal(dr["INTERESTPAYBLE"]);
                        lstmaturity.Add(objmamturityamount);

                    }

            }
            catch (Exception)
            {
                throw;
            }

            return lstmaturity;
        }

        public async Task<List<RdTransactinInterestAmountofTable>> GetRDInterestamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, string MemberType, string ConnectionString)
        {
            List<RdTransactinInterestAmountofTable> _RdTransactinDepositAmountofTableList = new List<RdTransactinInterestAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    // TenureMode = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(interestamount,0) as interestamount from tblmstrecurringdepositConfigdetails where tenuremode='" + TenureMode + "' and rdconfigid=" + RdconfigId + " and rdname='" + RDName + "' and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + "  and membertype='" + MemberType + "'  and coalesce(maturityamount,0)>0 and payindenomination=" + instalmentamount + ";"))
                    {
                        while (dr.Read())
                        {
                            var _RdTransactinInterestAmountofTable = new RdTransactinInterestAmountofTable
                            {
                                pInterestAmount = Convert.ToDecimal(dr["interestamount"])
                            };
                            _RdTransactinDepositAmountofTableList.Add(_RdTransactinInterestAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactinDepositAmountofTableList;
        }


        public async Task<List<RdTransactinMaturityAmountofTable>> GetRDMaturityamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount, string MemberType, string ConnectionString)
        {
            List<RdTransactinMaturityAmountofTable> _RdTransactinInterestAmountofTableList = new List<RdTransactinMaturityAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    //TenureMode = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(maturityamount,0) as maturityamount,coalesce(depositamount,0) as depositamount  from tblmstrecurringdepositconfigdetails where tenuremode='" + TenureMode + "' and rdconfigid=" + RdconfigId + " and rdname='" + RDName + "' and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + " and coalesce(maturityamount,0)>0 and payindenomination=" + instalmentamount + " and interestamount=" + Interestamount + "  and membertype='" + MemberType + "' ;"))
                    {
                        while (dr.Read())
                        {
                            var _RdTransactinInterestAmountofTable = new RdTransactinMaturityAmountofTable
                            {
                                pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]),
                                pDepositamount = Convert.ToDecimal(dr["depositamount"])
                            };
                            _RdTransactinInterestAmountofTableList.Add(_RdTransactinInterestAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdTransactinInterestAmountofTableList;
        }

        public async Task<List<RdInterestPayout>> GetRDPayoutsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount, decimal Maturityamount, string ConnectionString)
        {
            object InterestPayout = null;
            List<RdInterestPayout> _RdInterestPayoutDetails = new List<RdInterestPayout>();
            await Task.Run(() =>
            {
                try
                {
                    // TenureMode = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct interestpayout from tblmstrecurringdepositconfigdetails  where   statusid = " + Convert.ToInt32(Status.Active) + " and tenuremode='" + TenureMode + "' and rdconfigid=" + RdconfigId + " and rdname='" + RDName + "' and tenure=" + Tenure + " and coalesce(maturityamount,0)>0 and payindenomination=" + instalmentamount + " and interestamount=" + Interestamount + " and maturityamount=" + Maturityamount + "; "))
                    {
                        if (dr.Read())
                        {
                            InterestPayout = dr["interestpayout"];
                        }
                    }
                    if (InterestPayout != null)
                    {
                        string strInterestPay = Convert.ToString(InterestPayout);
                        if (strInterestPay.Contains(','))
                        {
                            for (int i = 0; i < strInterestPay.Split(',').Length; i++)
                            {
                                RdInterestPayout _RdInterestPayout = new RdInterestPayout();
                                _RdInterestPayout.pInterestPayOut = strInterestPay.Split(',')[i];
                                _RdInterestPayoutDetails.Add(_RdInterestPayout);
                            }
                        }
                        else if (!string.IsNullOrEmpty(strInterestPay))
                        {
                            RdInterestPayout _RdInterestPayout = new RdInterestPayout();
                            _RdInterestPayout.pInterestPayOut = strInterestPay;
                            _RdInterestPayoutDetails.Add(_RdInterestPayout);
                        }
                        else
                        {
                            _RdInterestPayoutDetails = null;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RdInterestPayoutDetails;
        }

        public int GetRDDepositamountCountofInterestRate(string Rdname, decimal instalmentamount, string MemberType, string ConnectionString)
        {
            int count;
            List<decimal> MultiplesList = new List<decimal>();
            int FinalCount = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositConfigdetails where rdname='" + Rdname + "' and (" + instalmentamount + " between mininstallmentamount and maxinstallmentamount) and statusid = " + Convert.ToInt32(Status.Active) + " and membertype='" + MemberType + "';"));
                if (count > 0)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(multiplesof,0) as multiplesof from tblmstrecurringdepositConfigdetails where rdname='" + Rdname + "' and (" + instalmentamount + " between mininstallmentamount and maxinstallmentamount) and statusid = " + Convert.ToInt32(Status.Active) + "  and membertype='" + MemberType + "' ; "))
                    {
                        while (dr.Read())
                        {
                            MultiplesList.Add(Convert.ToDecimal(dr["multiplesof"]));
                        }
                    }
                    if (MultiplesList.Count > 0)
                    {
                        for (int i = 0; i < MultiplesList.Count; i++)
                        {
                            if (MultiplesList[i] > 0)
                            {
                                FinalCount = instalmentamount % MultiplesList[i] == 0 ? FinalCount + 1 : FinalCount;
                            }
                            else
                            {
                                FinalCount = count;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return FinalCount;
        }


        public RdInterestRateValidation GetRDTenureandMininterestRateofInterestRate(string Rdname, decimal instalmentamount, long Tenure, string TenureMode, string InterestPayout, string MemberType, string ConnectionString)
        {
            RdInterestRateValidation _RdInterestRateValidation = new RdInterestRateValidation();
            try
            {
                DataSet ds = new DataSet();
                DataTable ComputedTable = new DataTable();
                ComputedTable.Columns.Add("Recordid");
                ComputedTable.Columns.Add("Fromdays");
                ComputedTable.Columns.Add("Todays");
                ComputedTable.Columns.Add("Valid");
                int ValidCount = 0;
               string TenureNature = Getpayinnatureoftenuremode(TenureMode, ConnectionString);
                ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select recordid,investmentperiodfrom,investmentperiodto from tblmstrecurringdepositConfigdetails where  Rdname='" + Rdname + "' and statusid = " + Convert.ToInt32(Status.Active) + "  and membertype='" + MemberType + "'  and (" + instalmentamount + " between mininstallmentamount and maxinstallmentamount) and interestpayout like '%" + InterestPayout + "%' and installmentpayin='"+ TenureMode + "' ;");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string Strfrom = Convert.ToString(ds.Tables[0].Rows[i]["investmentperiodfrom"]);
                        string Strto = Convert.ToString(ds.Tables[0].Rows[i]["investmentperiodto"]);
                        double FromDays = 0;
                        double ToDays = 0;
                        bool IsValid = false;
                        long TenureParam = Tenure;
                        if (!string.IsNullOrEmpty(Strfrom) && Strfrom.Contains(' '))
                        {
                            string[] StrfromDays = Strfrom.Split(' ');
                            if (StrfromDays.Length > 0)
                            {
                                for (int fromdays = 0; fromdays < StrfromDays.Length; fromdays++)
                                {
                                    if (StrfromDays[fromdays].Contains('Y'))
                                    {
                                        FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", "")) * 365;
                                    }
                                    else if (StrfromDays[fromdays].Contains('M'))
                                    {
                                        FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", "")) * 30;
                                    }
                                    else if (StrfromDays[fromdays].Contains('D'))
                                    {
                                        FromDays += Convert.ToInt32(Regex.Replace(StrfromDays[fromdays], "[^0-9]", ""));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Strfrom.Contains('Y'))
                            {
                                FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", "")) * 365;
                            }
                            else if (Strfrom.Contains('M'))
                            {
                                FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", "")) * 30;
                            }
                            else if (Strfrom.Contains('D'))
                            {
                                FromDays += Convert.ToInt32(Regex.Replace(Strfrom, "[^0-9]", ""));
                            }
                        }
                        if (!string.IsNullOrEmpty(Strto) && Strto.Contains(' '))
                        {
                            string[] StrtoDays = Strto.Split(' ');
                            if (StrtoDays.Length > 0)
                            {
                                for (int todays = 0; todays < StrtoDays.Length; todays++)
                                {
                                    if (StrtoDays[todays].Contains('Y'))
                                    {
                                        ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", "")) * 365;
                                    }
                                    else if (StrtoDays[todays].Contains('M'))
                                    {
                                        ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", "")) * 30;
                                    }
                                    else if (StrtoDays[todays].Contains('D'))
                                    {
                                        ToDays += Convert.ToInt32(Regex.Replace(StrtoDays[todays], "[^0-9]", ""));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Strto.Contains('Y'))
                            {
                                ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", "")) * 365;
                            }
                            else if (Strto.Contains('M'))
                            {
                                ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", "")) * 30;
                            }
                            else if (Strto.Contains('D'))
                            {
                                ToDays += Convert.ToInt32(Regex.Replace(Strto, "[^0-9]", ""));
                            }
                        }
                        TenureParam = TenureNature == "Days" ? TenureParam : TenureNature == "Months" ? TenureParam * 30 : TenureNature == "Years" ? TenureParam * 365 : TenureParam;

                        if (TenureParam >= FromDays && TenureParam <= ToDays)
                        {
                            IsValid = true;
                            ValidCount++;
                        }
                        else
                        {
                            IsValid = false;
                        }
                        ComputedTable.Rows.Add(ds.Tables[0].Rows[i]["recordid"], FromDays, ToDays, IsValid);
                    }
                }
                else
                {
                    _RdInterestRateValidation.pTenureCount = 0;
                }
                long Recordid = 0;
                DataRow[] rows = ComputedTable.Select("Valid = true");
                foreach (DataRow dr in rows)
                {
                    Recordid = Convert.ToInt64(dr["recordid"]);
                }
                if (Recordid > 0)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,referralcommissiontype,coalesce(commissionvalue,0) as commissionvalue,coalesce(valueper100,0)as valuefor100,caltype from tblmstrecurringdepositConfigdetails  where recordid = " + Recordid + " and statusid = " + Convert.ToInt32(Status.Active) + " ; "))
                    {
                        if (dr.Read())
                        {
                            _RdInterestRateValidation.pMinInterestRate = Convert.ToDecimal(dr["interestratefrom"]);  // Minimum Interest Rate
                            _RdInterestRateValidation.pMaxInterestRate = Convert.ToDecimal(dr["interestrateto"]);     // Maximum Interest Rate
                            _RdInterestRateValidation.pReferralCommisionvalue = Convert.ToDecimal(dr["commissionvalue"]);//ReferralCommission value
                            _RdInterestRateValidation.pReferralcommisiontype = dr["referralcommissiontype"];
                            //  _RdInterestRateValidation.pRatePerSquareYard = Convert.ToDecimal(dr["rate_per_square_yard"]);
                            _RdInterestRateValidation.pValuefor100 = Convert.ToDecimal(dr["valuefor100"]);
                            _RdInterestRateValidation.pCaltype = Convert.ToString(dr["caltype"]);
                        }
                    }
                    _RdInterestRateValidation.pTenureCount = ValidCount;
                    //strTenureandInterstRateCount = ValidCount.ToString() + "-" + InterestRateFrom.ToString() + "-" + InterestRateTo.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _RdInterestRateValidation;
        }

        public async Task<List<RdSchemeData>> GetRdSchemeDetailsforGrid(string Rdname, string ApplicantType, string MemberType, string ConnectionString)
        {
            List<RdSchemeData> RdSchemeDataList = new List<RdSchemeData>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdcalculationmode,coalesce( mininstallmentamount,0) as mininstallmentamount, coalesce( maxinstallmentamount,0) as maxinstallmentamount,investmentperiodfrom,investmentperiodto,interestpayout,interesttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,coalesce(multiplesof,0) as multiplesof,coalesce(valueper100,0) as valueper100,caltype,coalesce(tenure,0) as tabletenure,coalesce(maturityamount,0) as tablematurityamount,  coalesce(depositamount,0) tabledepositamount , coalesce(interestamount,0) as tableinterestamount,  coalesce(payindenomination,0) as  tablepayindenomination, tenuremode as  tabletenuremode,installmentpayin from tblmstrecurringdepositconfigdetails where rdname='" + Rdname + "' and applicanttype='" + ApplicantType + "' and membertype='" + MemberType + "'  and statusid=" + Convert.ToInt32(Status.Active) + " order by recordid; "))
                    {
                        while (dr.Read())
                        {
                            RdSchemeData _RdSchemeData = new RdSchemeData();
                            List<RDTenureModes> _Tenuremodeslist = new List<RDTenureModes>();
                            //table start
                            _RdSchemeData.pRdtabletenure = dr["tabletenure"];
                            _RdSchemeData.pRdtablematurityamount = dr["tablematurityamount"];
                            _RdSchemeData.pRdtabledepositamount = dr["tabledepositamount"];
                            _RdSchemeData.pRdtableinterestamount = dr["tableinterestamount"];
                            _RdSchemeData.pRdtablepayindenomination = dr["tablepayindenomination"];
                            _RdSchemeData.pRdtabletenuremode = dr["tabletenuremode"];

                            //table end

                            _RdSchemeData.pRdCalculationmode = dr["rdcalculationmode"];
                            _RdSchemeData.pMinInstallmentAmount = Convert.ToDecimal(dr["mininstallmentamount"]);
                            _RdSchemeData.pMaxInstallmentAmount = Convert.ToDecimal(dr["maxinstallmentamount"]);
                            _RdSchemeData.pInvestmentPeriodFrom = dr["investmentperiodfrom"];
                            _RdSchemeData.pInvestmentPeriodTo = dr["investmentperiodto"];
                            _RdSchemeData.pInterestPayOut = dr["interestpayout"];
                            _RdSchemeData.pInterestType = dr["interesttype"];
                            _RdSchemeData.pInterestRateFrom = Convert.ToDecimal(dr["interestratefrom"]);
                            _RdSchemeData.pInterestRateTo = Convert.ToDecimal(dr["interestrateto"]);
                            _RdSchemeData.pMultiplesof = Convert.ToDecimal(dr["multiplesof"]);
                            _RdSchemeData.pValuefor100 = Convert.ToDecimal(dr["valueper100"]);
                            _RdSchemeData.pCaltype = Convert.ToString(dr["caltype"]);

                            _RdSchemeData.pinstallmentpayin = Convert.ToString(dr["installmentpayin"]); 

                            if (Convert.ToString(_RdSchemeData.pInvestmentPeriodTo) != string.Empty)
                            {
                                if (Convert.ToString(_RdSchemeData.pInvestmentPeriodTo).Contains(' '))
                                {
                                    string[] strTenures = Convert.ToString(_RdSchemeData.pInvestmentPeriodTo).Split(' ');
                                    if (strTenures.Length > 0)
                                    {
                                        for (int k = 0; k < strTenures.Length; k++)
                                        {
                                            RDTenureModes _Tenuremodes = new RDTenureModes();
                                            if (Convert.ToString(strTenures[k]).Contains('Y'))
                                            {
                                                _Tenuremodes.pTenurename = "Years";
                                            }
                                            else if (Convert.ToString(strTenures[k]).Contains('M'))
                                            {
                                                _Tenuremodes.pTenurename = "Months";
                                            }
                                            else if (Convert.ToString(strTenures[k]).Contains('D'))
                                            {
                                                _Tenuremodes.pTenurename = "Days";
                                            }
                                            _Tenuremodeslist.Add(_Tenuremodes);
                                        }
                                    }
                                }
                                else
                                {
                                    RDTenureModes _Tenuremodes = new RDTenureModes();
                                    if (Convert.ToString(_RdSchemeData.pInvestmentPeriodTo).Contains('Y'))
                                    {
                                        _Tenuremodes.pTenurename = "Years";
                                    }
                                    else if (Convert.ToString(_RdSchemeData.pInvestmentPeriodTo).Contains('M'))
                                    {
                                        _Tenuremodes.pTenurename = "Months";
                                    }
                                    else if (Convert.ToString(_RdSchemeData.pInvestmentPeriodTo).Contains('D'))
                                    {
                                        _Tenuremodes.pTenurename = "Days";
                                    }
                                    _Tenuremodeslist.Add(_Tenuremodes);
                                }
                            }
                            _RdSchemeData.pTenureModesList = _Tenuremodeslist;
                            RdSchemeDataList.Add(_RdSchemeData);
                        }


                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return RdSchemeDataList;
        }


        public async Task<List<RDFiMemberContactDetails>> GetallJointMembers(string membercode, string Contacttype, string ConnectionString)
        {
            var _FiMemberContactDetails = new List<RDFiMemberContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode, te.contactid, te.contacttype, te.contactreferenceid, membername, coalesce(membertype,'') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, businessentitycontactno as contactnumber, businessentityemailid as emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tc on tc.contactid = te.contactid where   te.statusid=" + Convert.ToInt32(Status.Active) + " and membercode !='" + membercode + "' and te.contacttype='" + Contacttype + "' order by membername; "))
                    {
                        while (dr.Read())
                        {
                            var _FIMemberContactDTO = new RDFiMemberContactDetails
                            {
                                pMembertypeId = Convert.ToInt64(dr["memberid"]),
                                pContacttype = Convert.ToString(dr["contacttype"]),
                                pContactName = Convert.ToString(dr["membername"]),
                                pMemberType = Convert.ToString(dr["membertype"]),
                                pMemberStatus = Convert.ToString(dr["memberstatus"]),
                                pContactReferenceId = Convert.ToString(dr["contactreferenceid"]),
                                pContactId = Convert.ToInt64(dr["contactid"]),
                                ptypeofoperation = "OLD",
                                pMemberReferenceId = Convert.ToString(dr["membercode"]),
                                pContactNo = Convert.ToString(dr["contactnumber"]),
                                pEmailId = Convert.ToString(dr["emailid"]),
                                pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false
                            };
                            _FiMemberContactDetails.Add(_FIMemberContactDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FiMemberContactDetails;
        }

        public async Task<List<RDTenureModes>> GetRdSchemeTenureModes(string Rdname, string ApplicantType, string MemberType, string ConnectionString)
        {
            List<RDTenureModes> _Tenuremodeslist = new List<RDTenureModes>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct installmentpayin,payinnature,payinduration from tblmstrecurringdepositConfigdetails a join tblmstloanpayin b on a.installmentpayin=b.laonpayin where rdname='" + Rdname + "' and applicanttype='" + ApplicantType + "' and membertype='" + MemberType + "' and a.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            RDTenureModes _Tenuremodes = new RDTenureModes();
                            _Tenuremodes.pTenurename = dr["installmentpayin"];
                            _Tenuremodes.pTenurenature = dr["payinnature"];
                            _Tenuremodes.pPayinduration = dr["payinduration"];
                            _Tenuremodeslist.Add(_Tenuremodes);

                            //if (Convert.ToString(dr["investmentperiodto"]) != string.Empty)
                            //{
                            //    if (Convert.ToString(dr["investmentperiodto"]).Contains(' '))
                            //    {
                            //        string[] strTenures = Convert.ToString(dr["investmentperiodto"]).Split(' ');
                            //        if (strTenures.Length > 0)
                            //        {
                            //            for (int k = 0; k < strTenures.Length; k++)
                            //            {
                            //                RDTenureModes _Tenuremodes = new RDTenureModes();
                            //                if (Convert.ToString(strTenures[k]).Contains('Y'))
                            //                {
                            //                    _Tenuremodes.pTenurename = "Years";
                            //                    _Tenuremodes.pSortorder = 3;
                            //                }
                            //                else if (Convert.ToString(strTenures[k]).Contains('M'))
                            //                {
                            //                    _Tenuremodes.pTenurename = "Months";
                            //                    _Tenuremodes.pSortorder = 2;
                            //                }
                            //                else if (Convert.ToString(strTenures[k]).Contains('D'))
                            //                {
                            //                    _Tenuremodes.pTenurename = "Days";
                            //                    _Tenuremodes.pSortorder = 1;
                            //                }
                            //                int count = 0;
                            //                //if (_Tenuremodeslist.Count <= 0)
                            //                //{
                            //                //    _Tenuremodeslist.Add(_Tenuremodes);
                            //                //}
                            //                for (int i = 0; i < _Tenuremodeslist.Count; i++)
                            //                {
                            //                    if (_Tenuremodeslist[i].pTenurename == _Tenuremodes.pTenurename)
                            //                    {
                            //                        count++;
                            //                    }
                            //                }
                            //                if (count == 0)
                            //                {
                            //                    _Tenuremodeslist.Add(_Tenuremodes);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        RDTenureModes _Tenuremodes = new RDTenureModes();
                            //        if (Convert.ToString(dr["investmentperiodto"]).Contains('Y'))
                            //        {
                            //            _Tenuremodes.pTenurename = "Years";
                            //            _Tenuremodes.pSortorder = 3;
                            //        }
                            //        else if (Convert.ToString(dr["investmentperiodto"]).Contains('M'))
                            //        {
                            //            _Tenuremodes.pTenurename = "Months";
                            //            _Tenuremodes.pSortorder = 2;
                            //        }
                            //        else if (Convert.ToString(dr["investmentperiodto"]).Contains('D'))
                            //        {
                            //            _Tenuremodes.pTenurename = "Days";
                            //            _Tenuremodes.pSortorder = 1;
                            //        }
                            //        int count = 0;
                            //        for (int i = 0; i < _Tenuremodeslist.Count; i++)
                            //        {
                            //            if (_Tenuremodeslist[i].pTenurename == _Tenuremodes.pTenurename)
                            //            {
                            //                count++;
                            //            }
                            //        }
                            //        if (count == 0)
                            //        {
                            //            _Tenuremodeslist.Add(_Tenuremodes);
                            //        }
                            //    }
                            //}
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _Tenuremodeslist.OrderBy(x => x.pSortorder).ToList();
        }



        public async Task<List<RDInstallmentchart>> GetRdInstallmentchart(string Rdaccountno, string ConnectionString)
        {
            List<RDInstallmentchart> __RDInstallmentchartlist = new List<RDInstallmentchart>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from tabrdinstalments where rdaccountno='" + Rdaccountno + "' order by instalmentno;"))
                    {
                        while (dr.Read())
                        {
                            RDInstallmentchart _RDInstallmentchart = new RDInstallmentchart();
                            _RDInstallmentchart.prdaccountno = dr["rdaccountno"];
                            _RDInstallmentchart.pinstalmentno = dr["instalmentno"];
                            _RDInstallmentchart.pinstalmentdate = dr["instalmentdate"];
                            _RDInstallmentchart.pinstalmentamount = dr["instalmentamount"];
                            __RDInstallmentchartlist.Add(_RDInstallmentchart);

                          
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return __RDInstallmentchartlist;
        }



    }

   
}
