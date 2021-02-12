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
    public class FDTransactionDAL : SettingsDAL, IFDTransaction
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        ShareApplicationDAL objshareApplicationDAL { get; set; }
        public async Task<List<FDMemberNomineeDetails>> GetFDMemberNomineeDetails(string MemberCode, string ConnectionString)
        {
            List<FDMemberNomineeDetails> _FDNomineeDetailsList = new List<FDMemberNomineeDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select vchapplicationid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, statusid,coalesce(percentage,0) as percentage from tabapplicationpersonalnomineedetails where vchapplicationid='" + ManageQuote(MemberCode) + "' and (applicantype='MEMBER' or applicantype='Member') and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _FdNomineeDetails = new FDMemberNomineeDetails
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
                            _FDNomineeDetailsList.Add(_FdNomineeDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FDNomineeDetailsList;
        }

        public async Task<List<FdMembersandContactDetails>> GetFDMembers(string Contacttype, string MemberType, string ConnectionString)
        {
            List<FdMembersandContactDetails> _FdMembersList = new List<FdMembersandContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    //"select memberid,membercode,membername,te.contactid, te.contactreferenceid,contactnumber from tblmstmembers te join tblmstcontactpersondetails tc  on tc.contactid = te.contactid where  contacttype ='" + Contacttype + "' and membertype = '" + MemberType + "' and upper(priority) = 'PRIMARY' and  te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername; "
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,te.contactid, te.contactreferenceid,businessentitycontactno as contactnumber from tblmstmembers te join tblmstcontact tc  on tc.contactid = te.contactid where  te.contacttype ='" + Contacttype + "' and membertype = '" + MemberType + "' and  te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername; "))
                    {
                        while (dr.Read())
                        {
                            var _FdMembersandContactDetails = new FdMembersandContactDetails
                            {
                                pMemberCode = dr["membercode"],
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberName = dr["membername"],
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactrefid = dr["contactreferenceid"],
                                pContactnumber=dr["contactnumber"]
                            };
                            _FdMembersList.Add(_FdMembersandContactDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdMembersList;
        }
        
        public async Task< FDdetailsFromScheme> GetFdSchemeDetails(string ApplicantType, string MemberType, long FdconfigID, string Fdname, long Tenure, string Tenuremode, decimal Depositamount, string ConnectionString)
        {
            FDdetailsFromScheme _FDdetailsFromScheme = new FDdetailsFromScheme();
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

                    //ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select recordid,investmentperiodfrom,investmentperiodto from tblmstfixeddepositConfigdetails where applicanttype = '" + ApplicantType + "' and membertype='" + MemberType + "' and fdconfigid=" + FdconfigID + " and fdname='" + Fdname + "' and statusid = " + Convert.ToInt32(Status.Active) + "  and (" + Depositamount + " between mindepositamount and maxdepositamount) and (" + InterestRate + " between interestratefrom and interestrateto) ;");

                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select recordid,investmentperiodfrom,investmentperiodto from tblmstfixeddepositConfigdetails where applicanttype = '" + ApplicantType + "' and membertype='" + MemberType + "' and fdconfigid=" + FdconfigID + " and fdname='" + Fdname + "' and statusid = " + Convert.ToInt32(Status.Active) + "  and (" + Depositamount + " between mindepositamount and maxdepositamount);");
                    if (ds!=null && ds.Tables[0].Rows.Count>0)
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
                        if(string.IsNullOrEmpty(strRecordids))
                        {
                            strRecordids= Convert.ToString(dr["recordid"]);
                        }
                        else
                        {
                            strRecordids = strRecordids + "," + Convert.ToString(dr["recordid"]);
                        }
                    }

                    if (Recordid > 0)
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdcalculationmode, coalesce(mindepositamount,0) as mindepositamount,coalesce(maxdepositamount,0) as maxdepositamount,  investmentperiodfrom, investmentperiodto, interestpayout, interesttype, compoundinteresttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) interestrateto, coalesce(valuefor100,0) as valuefor100, coalesce(tenure,0) as tenure, tenuremode, coalesce(interestamount,0) as interestamount, coalesce(depositamount,0) as depositamount, coalesce(maturityamount,0) as maturityamount, coalesce(payindenomination,0) as payindenomination,tc.fdname,tcd.fdcode,tcd.fdnamecode,case when coalesce(valuefor100,0) <=0 then 'Interestrate' else 'Valueper100' end as InterestorValue,coalesce(multiplesof,0) as multiplesof,tc.referralcommissiontype,coalesce(tc.commissionvalue,0) as commissionvalue,caltype from tblmstfixeddepositConfigdetails  tc join tblmstfixeddepositConfig tcd on tc.fdconfigid=tcd.fdconfigid where tc.recordid = " + Recordid + " and tc.statusid = " + Convert.ToInt32(Status.Active) + " ; "))
                        {
                            if (dr.Read())
                            {
                                _FDdetailsFromScheme.pReferralcommisiontype = dr["referralcommissiontype"];
                                _FDdetailsFromScheme.pReferralCommsionvalue = Convert.ToDecimal(dr["commissionvalue"]);
                                _FDdetailsFromScheme.pFdAmount = Convert.ToDecimal(dr["depositamount"]);
                                _FDdetailsFromScheme.pMinDepositAmount = Convert.ToInt64(dr["mindepositamount"]);
                                _FDdetailsFromScheme.pMaxdepositAmount = Convert.ToDecimal(dr["maxdepositamount"]);
                                _FDdetailsFromScheme.pInvestmentPeriodFrom = dr["investmentperiodfrom"];
                                _FDdetailsFromScheme.pInvestmentPeriodTo = dr["investmentperiodto"];
                                _FDdetailsFromScheme.pInterestType = dr["interesttype"];
                                _FDdetailsFromScheme.pInterestPayOut = dr["interestpayout"];
                                _FDdetailsFromScheme.pInterestRateFrom = Convert.ToDecimal(dr["interestratefrom"]);
                                _FDdetailsFromScheme.pInterestRateTo = Convert.ToDecimal(dr["interestrateto"]);
                                _FDdetailsFromScheme.pInterestOrValueForHundred = Convert.ToDecimal(dr["valuefor100"]);
                                _FDdetailsFromScheme.pInterestTenureMode = dr["tenuremode"];
                                _FDdetailsFromScheme.pInterestTenure = Convert.ToInt64(dr["tenure"]);
                                _FDdetailsFromScheme.pInterestAmount = Convert.ToDecimal(dr["interestamount"]);
                                _FDdetailsFromScheme.pDepositAmount = Convert.ToDecimal(dr["depositamount"]);
                                _FDdetailsFromScheme.pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]);
                                _FDdetailsFromScheme.pPayinDenomination = Convert.ToDecimal(dr["payindenomination"]);
                                _FDdetailsFromScheme.pFdnameCode = dr["fdnamecode"];
                                _FDdetailsFromScheme.pFdname = dr["fdname"];
                                _FDdetailsFromScheme.pFdcode = dr["fdcode"];
                                _FDdetailsFromScheme.pValueORInterestratelabel = dr["InterestorValue"];
                                _FDdetailsFromScheme.pMultiplesof = Convert.ToDecimal(dr["multiplesof"]);

                                _FDdetailsFromScheme.FdInterestPayoutList = GetInterestPayoutsofScheme(strRecordids,FdconfigID, ConnectionString);

                                _FDdetailsFromScheme.pCaltype = Convert.ToString(dr["caltype"]);
                               // _FDdetailsFromScheme.FdInterestPayoutList = GetInterestPayoutsofScheme(FdconfigID,ConnectionString);

                            }
                        }
                        //if (_FDdetailsFromScheme != null)
                        //{
                        //    _FDdetailsFromScheme.FdInterestPayoutList = new List<FdInterestPayout>();
                        //    string strInterestPay = Convert.ToString(_FDdetailsFromScheme.pInterestPayOut);
                        //    if (strInterestPay.Contains(','))
                        //    {
                        //        for (int i = 0; i < strInterestPay.Split(',').Length; i++)
                        //        {
                        //            FdInterestPayout _FdInterestPayout = new FdInterestPayout();
                        //            _FdInterestPayout.pInterestPayOut = strInterestPay.Split(',')[i];
                        //            _FDdetailsFromScheme.FdInterestPayoutList.Add(_FdInterestPayout);
                        //        }
                        //    }
                        //    else if (!string.IsNullOrEmpty(strInterestPay))
                        //    {
                        //        FdInterestPayout _FdInterestPayout = new FdInterestPayout();
                        //        _FdInterestPayout.pInterestPayOut = strInterestPay;
                        //        _FDdetailsFromScheme.FdInterestPayoutList.Add(_FdInterestPayout);
                        //    }
                        //    else
                        //    {
                        //        _FDdetailsFromScheme.FdInterestPayoutList = null;
                        //    }
                        //}
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FDdetailsFromScheme;
        }

        public List<FdInterestPayout>  GetInterestPayoutsofScheme(string strRecordids,long FdconfigId,string Connectionstring)
        {
            List<FdInterestPayout>  FdInterestPayoutList = new List<FdInterestPayout>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct interestpayout from tblmstfixeddepositconfigdetails where fdconfigid=" + FdconfigId + " and  statusid=" + Convert.ToInt32(Status.Active) + " and recordid in ("+ strRecordids + ");"))

                //select distinct interestpayout from tblmstfixeddepositconfigdetails where fdconfigid = " + FdconfigId + " and statusid = " + Convert.ToInt32(Status.Active) + " and coalesce(valuefor100,0)= 0

                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct interestpayout from tblmstfixeddepositconfigdetails where fdconfigid=" + FdconfigId + " and  statusid=" + Convert.ToInt32(Status.Active) + ";"))

                {
                    while (dr.Read())
                    {
                        var _FdInterestPayoutdata = new FdInterestPayout
                        {
                            pInterestPayOut = dr["interestpayout"]                        
                        };
                        FdInterestPayoutList.Add(_FdInterestPayoutdata);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return FdInterestPayoutList;
        }

        public async Task<List<FdNameandCode>> GetFdSchemes(string ApplicantType, string MemberType, string ConnectionString)
        {
            List<FdNameandCode> _FdNameandCodeList = new List<FdNameandCode>();
            await Task.Run(() =>
            {
                try
                {
                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tcd.recordid,tc.fdconfigid,tc.fdname,tc.fdcode,tc.fdnamecode,fdcalculationmode from tblmstfixeddepositConfig tc join tblmstfixeddepositConfigdetails tcd on tc.fdconfigid=tcd.fdconfigid where membertype='" + MemberType + "' and applicanttype='" + ApplicantType + "' and tc.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    //{
                    //    while (dr.Read())
                    //    {
                    //        var _FdNameandCode = new FdNameandCode
                    //        {
                    //            pFdConfigId = Convert.ToInt64(dr["fdconfigid"]),
                    //            pFdname = dr["fdname"],
                    //            pFdcode = dr["fdcode"],
                    //            pFdnameCode = dr["fdnamecode"],
                    //            pFdCalculationmode = dr["fdcalculationmode"],
                    //            pFdDetailsRecordid= Convert.ToInt64(dr["recordid"])
                    //        };
                    //        _FdNameandCodeList.Add(_FdNameandCode);
                    //    }
                    //}


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct tc.fdconfigid,tc.fdname,tc.fdcode,tc.fdnamecode,fdcalculationmode from tblmstfixeddepositConfig tc join tblmstfixeddepositConfigdetails tcd on tc.fdconfigid=tcd.fdconfigid where membertype='" + MemberType + "' and applicanttype='" + ApplicantType + "' and tc.statusid=" + Convert.ToInt32(Status.Active) + " order by tc.fdname; "))
                    {
                        while (dr.Read())
                        {
                            var _FdNameandCode = new FdNameandCode
                            {
                                pFdConfigId = Convert.ToInt64(dr["fdconfigid"]),
                                pFdname = dr["fdname"],
                                pFdcode = dr["fdcode"],
                                pFdnameCode = dr["fdnamecode"],
                                pFdCalculationmode = dr["fdcalculationmode"]
                            };
                            _FdNameandCodeList.Add(_FdNameandCode);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdNameandCodeList;
        }

        public bool SaveFDJointMembersandNomineeData(FdJointandNomineeSave FdJointandNomineeSave, string ConnectionString)
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

                SbsaveReferences.AppendLine("Update tbltransfdcreation set isjointapplicable='" + FdJointandNomineeSave.pIsjointMembersapplicableorNot + "',isnomineesapplicable='" + FdJointandNomineeSave.pIsNomineesApplicableorNot + "' where fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "';");

                if (FdJointandNomineeSave.FdMembersandContactDetailsList != null && FdJointandNomineeSave.FdMembersandContactDetailsList.Count > 0)
                {
                    for (int i = 0; i < FdJointandNomineeSave.FdMembersandContactDetailsList.Count; i++)
                    {
                        if (Convert.ToString(FdJointandNomineeSave.FdMembersandContactDetailsList[i].pTypeofOperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(JointRecordid))
                            {
                                JointRecordid = Convert.ToString(FdJointandNomineeSave.FdMembersandContactDetailsList[i].precordid);
                            }
                            else
                            {
                                JointRecordid = JointRecordid + "," + Convert.ToString(FdJointandNomineeSave.FdMembersandContactDetailsList[i].precordid);
                            }
                        }
                    }

                    for (int i = 0; i < FdJointandNomineeSave.FdMembersandContactDetailsList.Count; i++)
                    {
                        if (Convert.ToString(FdJointandNomineeSave.FdMembersandContactDetailsList[i].pTypeofOperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("INSERT INTO tbltransfdjointdetails(fdaccountid, fdaccountno, memberid, membercode, membername,contactid, contacttype, contactreferenceid, statusid, createdby, createddate) VALUES (" + FdJointandNomineeSave.pFdAccountId + ", '" + FdJointandNomineeSave.pFdaccountNo + "', " + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberId + ", '" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberCode + "', '" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberName + "', " + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContactid + ",'" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContacttype + "', '" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContactrefid + "', " + Convert.ToInt32(Status.Active) + ", " + FdJointandNomineeSave.pCreatedby + ", current_timestamp);");
                        }
                        else if (Convert.ToString(FdJointandNomineeSave.FdMembersandContactDetailsList[i].pTypeofOperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tbltransfdjointdetails set memberid=" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberId + ",membercode='" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberCode + "',membername='" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pMemberName + "',contactid=" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContactid + ",contacttype='" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContacttype + "',contactreferenceid='" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].pContactrefid + "',statusid=" + Convert.ToInt32(Status.Active) + " where fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "' and recordid=" + FdJointandNomineeSave.FdMembersandContactDetailsList[i].precordid + ";");
                        }
                    }
                    if (!string.IsNullOrEmpty(JointRecordid))
                    {
                        sbUpdate.AppendLine("UPDATE tbltransfdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "' AND RECORDID not in(" + JointRecordid + ") ; ");
                    }
                    else
                    {
                        if (FdJointandNomineeSave.FdMembersandContactDetailsList == null || FdJointandNomineeSave.FdMembersandContactDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tbltransfdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "' ; ");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(JointRecordid))
                {
                    sbUpdate.AppendLine("UPDATE tbltransfdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "' AND RECORDID not in(" + JointRecordid + ") ; ");
                }
                else
                {
                    if (FdJointandNomineeSave.FdMembersandContactDetailsList == null || FdJointandNomineeSave.FdMembersandContactDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tbltransfdjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdaccountid=" + FdJointandNomineeSave.pFdAccountId + " and fdaccountno='" + FdJointandNomineeSave.pFdaccountNo + "'; ");
                    }
                }

                // Nominee Details
                if (FdJointandNomineeSave.FDMemberNomineeDetailsList != null && FdJointandNomineeSave.FDMemberNomineeDetailsList.Count > 0)
                {
                    for (int i = 0; i < FdJointandNomineeSave.FDMemberNomineeDetailsList.Count; i++)
                    {
                        if (Convert.ToString(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].ptypeofoperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(RecordId))
                            {
                                RecordId = Convert.ToString(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].precordid);
                            }
                            else
                            {
                                RecordId = RecordId + "," + Convert.ToString(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].precordid);
                            }
                        }
                    }
                    for (int i = 0; i < FdJointandNomineeSave.FDMemberNomineeDetailsList.Count; i++)
                    {                       
                        if (string.IsNullOrEmpty(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth))
                        {
                            FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth = "null";
                        }
                        else
                        {
                            FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth = "'" + FormatDate(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth) + "'";
                        }


                        if (Convert.ToString(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].ptypeofoperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee,percentage) values ('" + FdJointandNomineeSave.pMemberId + "', '" + FdJointandNomineeSave.pFdaccountNo + "', " + FdJointandNomineeSave.pContactid + ", '" + FdJointandNomineeSave.pContactrefid + "', '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pnomineename + "', '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].prelationship + "', " + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth + ", '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pcontactno + "', '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pidprooftype + "', '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pidproofname + "', '" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].preferencenumber + "', '" + 
                                
                                FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdocidproofpath + "', " + Convert.ToInt32(Status.Active) + ", '" + FdJointandNomineeSave.pCreatedby + "', current_timestamp,'FD-MEMBER'," + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pisprimarynominee + "," + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pPercentage + ");");
                        }
                        else if (Convert.ToString(FdJointandNomineeSave.FDMemberNomineeDetailsList[i].ptypeofoperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tabapplicationpersonalnomineedetails set contactid=" + FdJointandNomineeSave.pContactid + ",contactreferenceid='" + FdJointandNomineeSave.pContactrefid + "',nomineename='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pnomineename + "',relationship='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].prelationship + "',dateofbirth=" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdateofbirth + ",contactno='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pcontactno + "',idprooftype='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pidprooftype + "',idproofname='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pidproofname + "',referencenumber='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].preferencenumber + "',docidproofpath='" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pdocidproofpath + "',statusid=" + Convert.ToInt32(Status.Active) + ",modifieddate=current_timestamp,modifiedby=" + FdJointandNomineeSave.pCreatedby + ",percentage=" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pPercentage + ",isprimarynominee=" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].pisprimarynominee + " where applicantype='FD-MEMBER' and applicationid=" + FdJointandNomineeSave.pMemberId + " and recordid=" + FdJointandNomineeSave.FDMemberNomineeDetailsList[i].precordid + ";  ");
                        }
                    }
                    if (!string.IsNullOrEmpty(RecordId))
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + FdJointandNomineeSave.pMemberId + " and vchapplicationid='" + FdJointandNomineeSave.pFdaccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='FD-MEMBER'; ");
                    }
                    else
                    {
                        if (FdJointandNomineeSave.FDMemberNomineeDetailsList == null || FdJointandNomineeSave.FDMemberNomineeDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + FdJointandNomineeSave.pMemberId + " and vchapplicationid='" + FdJointandNomineeSave.pFdaccountNo + "' and applicantype='FD-MEMBER'; ");
                        }
                    }

                }

                if (!string.IsNullOrEmpty(RecordId))
                {
                    sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + FdJointandNomineeSave.pMemberId + " and vchapplicationid='" + FdJointandNomineeSave.pFdaccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='FD-MEMBER'; ");
                }
                else
                {
                    if (FdJointandNomineeSave.FDMemberNomineeDetailsList == null || FdJointandNomineeSave.FDMemberNomineeDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FdJointandNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + FdJointandNomineeSave.pMemberId + " and vchapplicationid='" + FdJointandNomineeSave.pFdaccountNo + "' and applicantype='FD-MEMBER'; ");
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

        public string SaveFDMemberandSchemeData(FdMemberandSchemeSave FdMemberandSchemeData, string ConnectionString, out long pFdAccountId)
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
                if(FdMemberandSchemeData.pSquareyard==null)
                {
                    FdMemberandSchemeData.pSquareyard = 0;
                }
                if (string.IsNullOrEmpty(FdMemberandSchemeData.pTransDate))
                {
                    FdMemberandSchemeData.pTransDate = "null";
                }
                else
                {
                    FdMemberandSchemeData.pTransDate = "'" + FormatDate(FdMemberandSchemeData.pTransDate) + "'";
                }
                if (string.IsNullOrEmpty(FdMemberandSchemeData.pMaturityDate))
                {
                    FdMemberandSchemeData.pMaturityDate = "null";
                }
                else
                {
                    FdMemberandSchemeData.pMaturityDate = "'" + FormatDate(FdMemberandSchemeData.pMaturityDate) + "'";
                }
                if (string.IsNullOrEmpty(FdMemberandSchemeData.pDepositDate))
                {
                    FdMemberandSchemeData.pDepositDate = "null";
                }
                else
                {
                    FdMemberandSchemeData.pDepositDate = "'" + FormatDate(FdMemberandSchemeData.pDepositDate) + "'";
                }
                if (Convert.ToString(FdMemberandSchemeData.pTypeofOperation) == "CREATE")
                {
                    if (string.IsNullOrEmpty(Convert.ToString(FdMemberandSchemeData.pFdAccountNo)) || Convert.ToString(FdMemberandSchemeData.pFdAccountNo)=="0")
                    {
                        FdMemberandSchemeData.pFdAccountNo = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('FIXED DEPOSIT','" + FdMemberandSchemeData.pFdname + "'," + FdMemberandSchemeData.pTransDate + ")").ToString();
                    }
                    // Fd Transaction Save 1st and 2nd Tab
                    FdMemberandSchemeData.pFdAccountId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltransfdcreation(fdaccountno,transdate, membertypeid, membertype, memberid, applicanttype, membercode, membername, contactid,contacttype,contactreferenceid, fdconfigid, fdname, tenortype, tenor, depositamount,interesttype, compoundinteresttype, interestrate, maturityamount, interestpayable, depositdate, maturitydate, isinterestdepositinsaving,isautorenew, renewonlyprinciple, renewonlyprincipleinterest,bondprintstatus, accountstatus, tokenno,statusid, createdby,createddate,isjointapplicable,isreferralapplicable,chitbranchid,chitbranchname,fdcalculationmode,interestpayout,isinterestdepositinbank,squareyard,caltype) VALUES ('" + FdMemberandSchemeData.pFdAccountNo + "', " + FdMemberandSchemeData.pTransDate + ", " + FdMemberandSchemeData.pMembertypeId + ", '" + FdMemberandSchemeData.pMemberType + "'," + FdMemberandSchemeData.pMemberId + ", '" + FdMemberandSchemeData.pApplicantType + "', '" + FdMemberandSchemeData.pMemberCode + "', '" + FdMemberandSchemeData.pMemberName + "', " + FdMemberandSchemeData.pContactid + ", '" + FdMemberandSchemeData.pContacttype + "',  '" + FdMemberandSchemeData.pContactrefid + "', " + FdMemberandSchemeData.pFdConfigId + ", '" + FdMemberandSchemeData.pFdname + "', '" + FdMemberandSchemeData.pInterestTenureMode + "', " + FdMemberandSchemeData.pInterestTenure + ", " + FdMemberandSchemeData.pDepositAmount + ",'" + FdMemberandSchemeData.pInterestType + "', '" + FdMemberandSchemeData.pCompoundInterestType + "', " + FdMemberandSchemeData.pInterestRate + ", " + FdMemberandSchemeData.pMaturityAmount + "," + FdMemberandSchemeData.pInterestAmount + ", " + FdMemberandSchemeData.pDepositDate + ", " + FdMemberandSchemeData.pMaturityDate + ", " + FdMemberandSchemeData.pIsinterestDepositinSaving + "," + FdMemberandSchemeData.pIsAutoRenew + ", " + FdMemberandSchemeData.pIsRenewOnlyPrinciple + ", " + FdMemberandSchemeData.pIsRenewOnlyPrincipleandInterest + ",'N','N', '" + FdMemberandSchemeData.pTokenNo + "'," + Convert.ToInt32(Status.Active) + ", " + FdMemberandSchemeData.pCreatedby + ",current_timestamp," + FdMemberandSchemeData.pIsJointMembersapplicable + "," + FdMemberandSchemeData.pIsReferralsapplicable + "," + FdMemberandSchemeData.pChitbranchId + ",'" + FdMemberandSchemeData.pChitbranchname + "','" + FdMemberandSchemeData.pFdCalculationmode + "','" + FdMemberandSchemeData.pInterestPayOut + "'," + FdMemberandSchemeData.pIsinterestDepositinBank + ","+ FdMemberandSchemeData.pSquareyard+ ",'"+ FdMemberandSchemeData.pCaltype + "') returning fdaccountid; "));
                }
                else if (Convert.ToString(FdMemberandSchemeData.pTypeofOperation) == "UPDATE")
                {
                    string strUpdate = "UPDATE tbltransfdcreation SET   tenortype ='" + FdMemberandSchemeData.pInterestTenureMode + "', tenor =" + FdMemberandSchemeData.pInterestTenure + ", depositamount =" + FdMemberandSchemeData.pDepositAmount + ", interesttype ='" + FdMemberandSchemeData.pInterestType + "', compoundinteresttype ='" + FdMemberandSchemeData.pCompoundInterestType + "', interestrate =" + FdMemberandSchemeData.pInterestRate + ", maturityamount =" + FdMemberandSchemeData.pMaturityAmount + ", interestpayable =" + FdMemberandSchemeData.pInterestAmount + ", depositdate =" + FdMemberandSchemeData.pDepositDate + ",maturitydate =" + FdMemberandSchemeData.pMaturityDate + ", isinterestdepositinsaving =" + FdMemberandSchemeData.pIsinterestDepositinSaving + ", isautorenew =" + FdMemberandSchemeData.pIsAutoRenew + ", renewonlyprinciple =" + FdMemberandSchemeData.pIsRenewOnlyPrinciple + ", renewonlyprincipleinterest =" + FdMemberandSchemeData.pIsRenewOnlyPrincipleandInterest + ", isjointapplicable =" + FdMemberandSchemeData.pIsJointMembersapplicable + ", isreferralapplicable =" + FdMemberandSchemeData.pIsReferralsapplicable + ", bondprintstatus ='N', accountstatus ='N', tokenno ='" + FdMemberandSchemeData.pTokenNo + "', statusid =" + Convert.ToInt32(Status.Active) + ",  modifiedby =" + FdMemberandSchemeData.pCreatedby + ", modifieddate =current_timestamp,chitbranchid=" + FdMemberandSchemeData.pChitbranchId + ",chitbranchname='" + FdMemberandSchemeData.pChitbranchname + "',fdcalculationmode='" + FdMemberandSchemeData.pFdCalculationmode + "',interestpayout='" + FdMemberandSchemeData.pInterestPayOut + "',isinterestdepositinbank=" + FdMemberandSchemeData.pIsinterestDepositinBank + ",squareyard="+ FdMemberandSchemeData.pSquareyard+ ",caltype='"+ FdMemberandSchemeData.pCaltype + "'  WHERE fdaccountid =" + FdMemberandSchemeData.pFdAccountId + " and fdaccountno ='" + FdMemberandSchemeData.pFdAccountNo + "'; ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strUpdate);
                }
                pFdAccountId = FdMemberandSchemeData.pFdAccountId;
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
            return Convert.ToString(FdMemberandSchemeData.pFdAccountNo);
        }

        public bool SaveFDReferralData(FdTransactionReferrals FdTransactionReferrals, string ConnectionString)
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

                SbsaveReferences.AppendLine("update tbltransfdcreation set isreferralapplicable='" + FdTransactionReferrals.pIsReferralsapplicable + "' where fdaccountno='" + FdTransactionReferrals.pFdaccountNo + "';");

                if (FdTransactionReferrals.pIsReferralsapplicable)
                {
                    if (Convert.ToString(FdTransactionReferrals.pTypeofOperation) == "CREATE")
                    {
                        SbsaveReferences.AppendLine("INSERT INTO tbltransfdreferraldetails(fdaccountid, fdaccountno, referralid, referralname,salespersonname, commsssionvalue, statusid, createdby, createddate,commissiontype,referralcode,contactid) VALUES ( " + FdTransactionReferrals.pFdAccountId + ", '" + FdTransactionReferrals.pFdaccountNo + "', '" + FdTransactionReferrals.pReferralId + "', '" + FdTransactionReferrals.pAdvocateName + "', '" + FdTransactionReferrals.pSalesPersonName + "'," + FdTransactionReferrals.pCommisionValue + ", " + Convert.ToInt32(Status.Active) + ", " + FdTransactionReferrals.pCreatedby + ", current_timestamp,'"+ FdTransactionReferrals.pCommissionType + "','"+ FdTransactionReferrals.pReferralCode + "',"+ FdTransactionReferrals.pContactId + ");");
                    }
                    else if (Convert.ToString(FdTransactionReferrals.pTypeofOperation) == "UPDATE")
                    {
                        SbsaveReferences.AppendLine("Update tbltransfdreferraldetails set referralid='" + FdTransactionReferrals.pReferralId + "',referralname='" + FdTransactionReferrals.pAdvocateName + "',salespersonname='" + FdTransactionReferrals.pSalesPersonName + "',commsssionvalue=" + FdTransactionReferrals.pCommisionValue + ",modifiedby=" + FdTransactionReferrals.pCreatedby + ",modifieddate=current_timestamp,commissiontype='" + FdTransactionReferrals.pCommissionType + "',referralcode='"+ FdTransactionReferrals.pReferralCode + "',contactid="+ FdTransactionReferrals.pContactId + " where fdaccountid=" + FdTransactionReferrals.pFdAccountId + " and fdaccountno='" + FdTransactionReferrals.pFdaccountNo + "';");
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

        public bool DeleteFdTransactions(string FdaccountNo, long Userid, string ConnectionString)
        {
            StringBuilder SbDeleteFdTransaction = new StringBuilder();
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
                long FdAccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select fdaccountid from tbltransfdcreation where fdaccountno='" + FdaccountNo + "' ;"));

                // Nominee Details 
                SbDeleteFdTransaction.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  vchapplicationid='" + FdaccountNo + "' and applicationid=" + FdAccountid + " and upper(applicantype)='FD-MEMBER'; ");

                // Insurance Member Details
                SbDeleteFdTransaction.AppendLine("UPDATE tbltransfdjointdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where fdaccountno='" + FdaccountNo + "'; ");

                SbDeleteFdTransaction.AppendLine("UPDATE tbltransfdreferraldetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where fdaccountno='" + FdaccountNo + "'; ");

                SbDeleteFdTransaction.AppendLine("UPDATE tbltransfdcreation SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where fdaccountno='" + FdaccountNo + "'; ");

                if (Convert.ToString(SbDeleteFdTransaction) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbDeleteFdTransaction.ToString());
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

        public async Task<List<FdTransactionMainGridData>> GetFdTransactionData(string ConnectionString)
        {
            List<FdTransactionMainGridData> _FdTransactionMainGridData = new List<FdTransactionMainGridData>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdaccountid,fdaccountno,membertype,applicanttype,fdname,membername, coalesce(depositamount,0) as depositamount, coalesce(maturityamount,0) as maturityamount,to_char(depositdate, 'dd/Mon/yyyy')depositdate,to_char(maturitydate, 'dd/Mon/yyyy') maturitydate,chitbranchname,(tenor ||' '|| tenortype)Tenure,interestpayout,(select (case when count(fd_account_id)> 0 then true else false end)ReceiptStatus from fd_receipt tt where tt.fd_account_id=t.fdaccountid and status=true)  from tbltransfdcreation t where statusid=" + Convert.ToInt32(Status.Active) + " order by fdaccountid desc;;"))
                    {
                        while (dr.Read())
                        {
                            var _FdTransactionMainGridDataDetails = new FdTransactionMainGridData
                            {
                                pFdAccountId = Convert.ToInt64(dr["fdaccountid"]),
                                pFdaccountNo = dr["fdaccountno"],
                                pMemberType = dr["membertype"],
                                pApplicantType = dr["applicanttype"],
                                pFdname = dr["fdname"],
                                pMemberName = dr["membername"],
                                pReceiptStatus = dr["ReceiptStatus"],
                                pDepositAmount = Convert.ToDecimal(dr["depositamount"]),
                                pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]),
                                pDepositDate = Convert.ToString(dr["depositdate"]),
                                pMaturityDate = Convert.ToString(dr["maturitydate"]),
                                pChitbranchname = dr["chitbranchname"],
                                pTenure = dr["Tenure"],
                                pInterestPayout=dr["interestpayout"]
                            };
                            _FdTransactionMainGridData.Add(_FdTransactionMainGridDataDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactionMainGridData;
        }

        public async Task<FdTransactionDataEdit> GetFdTransactionDetailsforEdit(string FdAccountNo, long FdAccountId, string ConnectionString,string accounttype)
        {
            objshareApplicationDAL = new ShareApplicationDAL();
            FdTransactionDataEdit _FdTransactionDataEdit = new FdTransactionDataEdit();
            await Task.Run(() =>
            {
                try
                {
                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT transdate,ta.fdaccountid,ta.fdaccountno, membertypeid, membertype, memberid, applicanttype, membercode, membername, contactid, contacttype,contactreferenceid, ta.fdconfigid, tc.fdname, tenortype, tenor,coalesce( depositamount,0) as depositamount, interesttype, compoundinteresttype,coalesce( interestrate,0) as interestrate,coalesce( maturityamount,0) as maturityamount , interestpayable, depositdate, maturitydate, isinterestdepositinsaving, isautorenew, renewonlyprinciple, renewonlyprincipleinterest, isjointapplicable, isreferralapplicable, bondprintstatus, accountstatus, tokenno, referralid,referralname,salespersonname,coalesce( commsssionvalue,0) as commsssionvalue,chitbranchid,chitbranchname,fdcalculationmode,tc.fdnamecode,tc.fdcode,ta.interestpayout, coalesce(isinterestdepositinbank,'false')as isinterestdepositinbank,coalesce(isnomineesapplicable,false)as isnomineesapplicable,interestpayout,commissiontype,(case when ta.squareyard is null then 0 else ta.squareyard end)squareyard,caltype FROM tbltransfdcreation ta join tblmstfixeddepositconfig tc on ta.fdconfigid=tc.fdconfigid left join tbltransfdreferraldetails trd on ta.fdaccountid=trd.fdaccountid where ta.statusid = " + Convert.ToInt32(Status.Active) + " and ta.fdaccountid=" + FdAccountId + " and ta.fdaccountno='" + FdAccountNo + "' order by applicanttype desc;"))
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT distinct transdate,ta.fdaccountid,ta.fdaccountno, ta.membertypeid, ta.membertype, memberid, ta.applicanttype, membercode, membername, ta.contactid, contacttype,contactreferenceid, ta.fdconfigid, tc.fdname, tenortype, tenor,coalesce( ta.depositamount,0) as depositamount, ta.interesttype, ta.compoundinteresttype,coalesce( interestrate,0) as interestrate,coalesce( ta.maturityamount,0) as maturityamount , interestpayable, depositdate, maturitydate, isinterestdepositinsaving, isautorenew, renewonlyprinciple, renewonlyprincipleinterest, isjointapplicable, isreferralapplicable, bondprintstatus, accountstatus, tokenno, coalesce( tcd.commissionvalue,0) as commsssionvalue,chitbranchid,chitbranchname,ta.fdcalculationmode,tc.fdnamecode,tc.fdcode,ta.interestpayout, coalesce(isinterestdepositinbank,'false')as isinterestdepositinbank,coalesce(isnomineesapplicable,false)as isnomineesapplicable,ta.interestpayout,referralcommissiontype as commissiontype,(case when ta.squareyard is null then 0 else ta.squareyard end)squareyard,ta.caltype FROM tbltransfdcreation ta join tblmstfixeddepositconfig tc on ta.fdconfigid=tc.fdconfigid join tblmstfixeddepositconfigdetails tcd on tc.fdconfigid=tcd.fdconfigid and ta.interestpayout=tcd.interestpayout  where ta.statusid = " + Convert.ToInt32(Status.Active) + " and ta.fdaccountid=" + FdAccountId + "  and ta.fdaccountno='" + FdAccountNo + "' order by applicanttype desc; "))
                    {
                        if (dr.Read())
                        {
                            _FdTransactionDataEdit.pCommissionType = dr["commissiontype"];
                            _FdTransactionDataEdit.pTransDate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy");
                            _FdTransactionDataEdit.pFdAccountId = Convert.ToInt64(dr["fdaccountid"]);
                            _FdTransactionDataEdit.pFdAccountNo = dr["fdaccountno"];
                            _FdTransactionDataEdit.pMembertypeId = Convert.ToInt64(dr["membertypeid"]);
                            _FdTransactionDataEdit.pMemberType = dr["membertype"];
                            _FdTransactionDataEdit.pMemberId = Convert.ToInt64(dr["memberid"]);
                            _FdTransactionDataEdit.pApplicantType = dr["applicanttype"];
                            _FdTransactionDataEdit.pMemberCode = dr["membercode"];
                            _FdTransactionDataEdit.pMemberName = dr["membername"];
                            _FdTransactionDataEdit.pContactid = Convert.ToInt64(dr["contactid"]);
                            _FdTransactionDataEdit.pContacttype = dr["contacttype"];
                            _FdTransactionDataEdit.pContactrefid = dr["contactreferenceid"];
                            _FdTransactionDataEdit.pFdConfigId = Convert.ToInt64(dr["fdconfigid"]);
                            _FdTransactionDataEdit.pFdname = dr["fdname"];
                            _FdTransactionDataEdit.pFdnameCode = dr["fdnamecode"];
                            _FdTransactionDataEdit.pFdcode = dr["fdcode"];
                            _FdTransactionDataEdit.pInterestTenureMode = dr["tenortype"];
                            _FdTransactionDataEdit.pInterestTenure = Convert.ToInt64(dr["tenor"]);
                            _FdTransactionDataEdit.pDepositAmount = Convert.ToDecimal(dr["depositamount"]);
                            _FdTransactionDataEdit.pInterestType = dr["interesttype"];
                            _FdTransactionDataEdit.pCompoundInterestType = dr["compoundinteresttype"];
                            _FdTransactionDataEdit.pInterestRate = Convert.ToDecimal(dr["interestrate"]);
                            _FdTransactionDataEdit.pMaturityAmount = Convert.ToDecimal(dr["maturityamount"]);
                            _FdTransactionDataEdit.pInterestPayOut = dr["interestpayout"];

                            if(Convert.ToString(dr["depositdate"])!=string.Empty)
                            {
                                _FdTransactionDataEdit.pDepositDate = Convert.ToDateTime(dr["depositdate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                _FdTransactionDataEdit.pDepositDate = null;
                            }
                            if (Convert.ToString(dr["maturitydate"]) != string.Empty)
                            {
                                _FdTransactionDataEdit.pMaturityDate = Convert.ToDateTime(dr["maturitydate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                _FdTransactionDataEdit.pMaturityDate = null;
                            }                           
                            _FdTransactionDataEdit.pIsinterestDepositinSaving = Convert.ToBoolean(dr["isinterestdepositinsaving"]);
                            _FdTransactionDataEdit.pIsAutoRenew = Convert.ToBoolean(dr["isautorenew"]);
                            _FdTransactionDataEdit.pIsRenewOnlyPrinciple = Convert.ToBoolean(dr["renewonlyprinciple"]);
                            _FdTransactionDataEdit.pIsJointMembersapplicable = Convert.ToBoolean(dr["isjointapplicable"]);
                            _FdTransactionDataEdit.pIsReferralsapplicable = Convert.ToBoolean(dr["isreferralapplicable"]);
                            if(Convert.ToString( dr["bondprintstatus"])=="false" || Convert.ToString(dr["bondprintstatus"]) == "N")
                            {
                                _FdTransactionDataEdit.pBondPrintStatus = false;
                            }
                            else
                            {
                                _FdTransactionDataEdit.pBondPrintStatus = false;
                            }                           
                            _FdTransactionDataEdit.pAccountStatus = dr["accountstatus"];
                            _FdTransactionDataEdit.pTokenNo = dr["tokenno"];
                            //_FdTransactionDataEdit.pReferralId = dr["referralid"];
                            //_FdTransactionDataEdit.pAdvocateName = dr["referralname"];
                            //_FdTransactionDataEdit.pSalesPersonName = dr["salespersonname"];
                            _FdTransactionDataEdit.pCommisionValue = Convert.ToDecimal(dr["commsssionvalue"]);
                            _FdTransactionDataEdit.JointMembersandContactDetailsList = objshareApplicationDAL.GetJointMembersListInEdit(FdAccountNo, accounttype, ConnectionString);
                            _FdTransactionDataEdit.MemberNomineeDetailsList = objshareApplicationDAL.GetNomineesListInEdit(FdAccountNo, accounttype, ConnectionString);
                            _FdTransactionDataEdit.MemberReferalDetails = objshareApplicationDAL.getReferralDetails(ConnectionString, FdAccountNo, accounttype);
                            _FdTransactionDataEdit.pChitbranchId = Convert.ToInt64(dr["chitbranchid"]);
                            _FdTransactionDataEdit.pChitbranchname = dr["chitbranchname"];
                            _FdTransactionDataEdit.pFdCalculationmode = dr["fdcalculationmode"];
                            _FdTransactionDataEdit.pInterestPayOut = dr["interestpayout"];
                            _FdTransactionDataEdit.pIsinterestDepositinBank = Convert.ToBoolean(dr["isinterestdepositinbank"]);
                            _FdTransactionDataEdit.pIsNomineesapplicable = Convert.ToBoolean(dr["isnomineesapplicable"]);
                            _FdTransactionDataEdit.pInterestAmount = Convert.ToDecimal(dr["interestpayable"]);
                            _FdTransactionDataEdit.pSquareyard = Convert.ToDecimal(dr["squareyard"]);
                            _FdTransactionDataEdit.pCaltype = Convert.ToString(dr["caltype"]);
                            //_FdTransactionDataEdit.pReferralCode = dr["referralcode"];
                            //_FdTransactionDataEdit.pReferralContactId = dr["referralcontactid"];

                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactionDataEdit;
        }
        
        public List<FdMembersandContactDetails> GetJointMembersListofFdInEdit(string FdAccountNo, string ConnectionString)
        {
            var _FdMemberJointDetailsList = new List<FdMembersandContactDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, memberid,membercode,membername,contactid,contacttype,contactreferenceid from tbltransfdjointdetails  where  fdaccountno = '" + FdAccountNo + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        FdMembersandContactDetails _FdMemberJointDetails = new FdMembersandContactDetails
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
                        _FdMemberJointDetailsList.Add(_FdMemberJointDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FdMemberJointDetailsList;
        }

        public List<FDMemberNomineeDetails> GetNomineesListofFdInEdit(string FdAccountNo, string ConnectionString)
        {
            var _FdMemberNomineeDetailsList = new List<FDMemberNomineeDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,vchapplicationid,contactid,contactreferenceid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, applicantype,coalesce(percentage,0) as percentage,statusid from tabapplicationpersonalnomineedetails where vchapplicationid='" + FdAccountNo + "' and applicantype='FD-MEMBER' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        var _FdTransNomineeDetails = new FDMemberNomineeDetails
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
                        _FdMemberNomineeDetailsList.Add(_FdTransNomineeDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FdMemberNomineeDetailsList;
        }

        public async Task<List<FdTransactinTenuresofTable>> GetFdTenuresofTable(string FDName, long FdconfigId, string TenureMode, string MemberType, string ConnectionString)
        {
            List<FdTransactinTenuresofTable> _FdTransactionMainGridDataList = new List<FdTransactinTenuresofTable>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct tenure from tblmstfixeddepositConfigdetails where tenuremode='" + TenureMode + "' and fdconfigid=" + FdconfigId + " and fdname='" + FDName + "' and membertype='" + MemberType + "'  and statusid=" + Convert.ToInt32(Status.Active) + " and coalesce(tenure,0)>0;"))
                    {
                        while (dr.Read())
                        {
                            var _FdTransactionMainGridData = new FdTransactinTenuresofTable
                            {
                                pInterestTenure = Convert.ToInt64(dr["tenure"])
                            };
                            _FdTransactionMainGridDataList.Add(_FdTransactionMainGridData);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactionMainGridDataList;
        }

        public async Task<List<FdTransactinDepositAmountofTable>> GetFdDepositamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, string MemberType, string ConnectionString)
        {
            List<FdTransactinDepositAmountofTable> _FdTransactinDepositAmountofTableList = new List<FdTransactinDepositAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select depositamount from tblmstfixeddepositConfigdetails where tenuremode='" + TenureMode + "' and fdconfigid=" + FdconfigId + " and fdname='" + FDName + "' and membertype='" + MemberType + "'  and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + " and coalesce(depositamount,0)>0;"))
                    {
                        while (dr.Read())
                        {
                            var _FdTransactinDepositAmountofTable = new FdTransactinDepositAmountofTable
                            {
                                pDepositAmount = Convert.ToDecimal(dr["depositamount"])
                            };
                            _FdTransactinDepositAmountofTableList.Add(_FdTransactinDepositAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactinDepositAmountofTableList;
        }

        public async Task<List<ChitBranchDetails>> GetchitBranchDetails(string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select code,branchname,vchregion,vchzone from tabbranchcodes  order by branchname;"))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["code"]),
                                pBranchname = dr["branchname"],
                                pVchRegion = dr["vchregion"],
                                pVchZone = dr["vchzone"]
                            };
                            _ChitBranchDetailsList.Add(_ChitBranchDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _ChitBranchDetailsList;
        }

        public async Task<object> GetchitBranchstatus(string Connectionstring)
        {
            object BranchStatus = null;
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select coalesce(chitbranchstatus,'N') as chitbranchstatus from tblmstcompany where statusid=" + Convert.ToInt32(Status.Active) + " ;"))
                    {
                        if (dr.Read())
                        {
                            BranchStatus = dr["chitbranchstatus"];
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return BranchStatus;
        }

        public List<Matuerityamount> GetMaturityamount(string pInterestMode, long pInterestTenure, decimal pDepositAmount, string pInterestPayOut, string pCompoundorSimpleInterestType, decimal pInterestRate,string pCalType, string Connectionstring)
        {
            List<Matuerityamount> lstmaturity = new List<Matuerityamount>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from  FN_CALUCATE_FD_RD_INTEREST('FD','" + pInterestPayOut + "','" + pCompoundorSimpleInterestType + "',''," + pInterestTenure + ",'" + pInterestMode + "'," + pInterestRate + "," + pDepositAmount + ",'"+ pCalType + "') ;"))
                    // Maturityamount = NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select * from  FN_CALUCATE_FD_RD_INTEREST('FD','"+ pInterestPayOut + "','"+ pCompoundorSimpleInterestType + "','',"+ pInterestTenure+ ",'"+ pInterestMode + "',"+ pInterestRate+ ","+ pDepositAmount+ ") ;").ToString();
                    while (dr.Read())
                    {
                        Matuerityamount objmamturityamount = new Matuerityamount();
                        objmamturityamount.pMatueritytAmount= Convert.ToDecimal(dr["MATURITYAMOUNT"]);
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

        public async Task<List<FdTransactinInterestAmountofTable>> GetInterestamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, string MemberType, string ConnectionString)
        {
            List<FdTransactinInterestAmountofTable> _FdTransactinDepositAmountofTableList = new List<FdTransactinInterestAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(interestamount,0) as interestamount from tblmstfixeddepositConfigdetails where tenuremode='" + TenureMode + "' and fdconfigid=" + FdconfigId + " and fdname='" + FDName + "' and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + "  and membertype='" + MemberType + "'  and coalesce(maturityamount,0)>0 and depositamount=" + Depositamount + ";"))
                    {
                        while (dr.Read())
                        {
                            var _FdTransactinInterestAmountofTable = new FdTransactinInterestAmountofTable
                            {
                                pInterestAmount = Convert.ToDecimal(dr["interestamount"])
                            };
                            _FdTransactinDepositAmountofTableList.Add(_FdTransactinInterestAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactinDepositAmountofTableList;
        }

        public  async Task<List<FdTransactinMaturityAmountofTable>> GetMaturityamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, decimal Interestamount, string MemberType, string ConnectionString)
        {
            List<FdTransactinMaturityAmountofTable> _FdTransactinInterestAmountofTableList = new List<FdTransactinMaturityAmountofTable>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(maturityamount,0) as maturityamount from tblmstfixeddepositConfigdetails where tenuremode='" + TenureMode + "' and fdconfigid=" + FdconfigId + " and fdname='" + FDName + "' and statusid=" + Convert.ToInt32(Status.Active) + " and tenure=" + Tenure + " and coalesce(maturityamount,0)>0 and depositamount=" + Depositamount + " and interestamount="+ Interestamount + " and membertype='" + MemberType + "';"))
                    {
                        while (dr.Read())
                        {
                            var _FdTransactinInterestAmountofTable = new FdTransactinMaturityAmountofTable
                            {
                                pMaturityAmount = Convert.ToDecimal(dr["maturityamount"])
                            };
                            _FdTransactinInterestAmountofTableList.Add(_FdTransactinInterestAmountofTable);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FdTransactinInterestAmountofTableList;
        }

        public async Task<List<FdInterestPayout>> GetPayoutsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, decimal Interestamount, decimal Maturityamount, string ConnectionString)
        {
            object InterestPayout = null;
            List<FdInterestPayout> _FdInterestPayoutDetails = new List<FdInterestPayout>();
            await Task.Run(() =>
            {
                try
                {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  interestpayout from tblmstfixeddepositConfigdetails  where   statusid = " + Convert.ToInt32(Status.Active) + " and tenuremode='" + TenureMode + "' and fdconfigid=" + FdconfigId + " and fdname='" + FDName + "' and tenure=" + Tenure + " and coalesce(maturityamount,0)>0 and depositamount=" + Depositamount + " and interestamount=" + Interestamount + " and maturityamount="+ Maturityamount + "; "))
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
                            FdInterestPayout _FdInterestPayout = new FdInterestPayout();
                            _FdInterestPayout.pInterestPayOut = strInterestPay.Split(',')[i];
                            _FdInterestPayoutDetails.Add(_FdInterestPayout);
                        }
                    }
                    else if (!string.IsNullOrEmpty(strInterestPay))
                    {
                        FdInterestPayout _FdInterestPayout = new FdInterestPayout();
                        _FdInterestPayout.pInterestPayOut = strInterestPay;
                        _FdInterestPayoutDetails.Add(_FdInterestPayout);
                    }
                    else
                    {
                        _FdInterestPayoutDetails = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        });
            return _FdInterestPayoutDetails;
        }
        
        public int GetDepositamountCountofInterestRate(string Fdname, decimal Depositamount, string MemberType, string ConnectionString)
        {
            int count;
            List<decimal> MultiplesList = new List<decimal>();
            int FinalCount=0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstfixeddepositConfigdetails where fdname='" + Fdname + "' and (" + Depositamount + " between mindepositamount and maxdepositamount) and statusid = " + Convert.ToInt32(Status.Active) + " and membertype='" + MemberType + "';"));
                if(count >0)
                {                                    
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(multiplesof,0) as multiplesof from tblmstfixeddepositConfigdetails where fdname='" + Fdname + "' and (" + Depositamount + " between mindepositamount and maxdepositamount) and statusid = " + Convert.ToInt32(Status.Active) + " and membertype='" + MemberType + "'; "))
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
                            if(MultiplesList[i] > 0)
                            {
                                FinalCount = Depositamount % MultiplesList[i] == 0 ? FinalCount + 1 : FinalCount;
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

        public FdInterestRateValidation GetTenureandMininterestRateofInterestRate(string Fdname, decimal Depositamount,long Tenure,string TenureMode,string InterestPayout, string MemberType, string ConnectionString)
        {           
            FdInterestRateValidation _FdInterestRateValidation = new FdInterestRateValidation();
            try
            {
                DataSet ds = new DataSet();
                DataTable ComputedTable = new DataTable();
                ComputedTable.Columns.Add("Recordid");
                ComputedTable.Columns.Add("Fromdays");
                ComputedTable.Columns.Add("Todays");
                ComputedTable.Columns.Add("Valid");
                int ValidCount = 0;
                ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select recordid,investmentperiodfrom,investmentperiodto from tblmstfixeddepositConfigdetails where  fdname='" + Fdname + "' and statusid = " + Convert.ToInt32(Status.Active) + "  and membertype='" + MemberType + "'  and (" + Depositamount + " between mindepositamount and maxdepositamount) and interestpayout like '%" + InterestPayout + "%' ;");
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
                        TenureParam = TenureMode == "Days" ? TenureParam : TenureMode == "Months" ? TenureParam * 30 : TenureMode == "Years" ? TenureParam * 365 : TenureParam;

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
                    _FdInterestRateValidation.pTenureCount = 0;
                }
                long Recordid = 0;
                DataRow[] rows = ComputedTable.Select("Valid = true");
                foreach (DataRow dr in rows)
                {
                    Recordid = Convert.ToInt64(dr["recordid"]);
                }
                if (Recordid > 0)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,referralcommissiontype,coalesce(commissionvalue,0) as commissionvalue,coalesce(rate_per_square_yard,0) as rate_per_square_yard,coalesce(valuefor100,0)as valuefor100,caltype from tblmstfixeddepositConfigdetails  where recordid = " + Recordid + " and statusid = " + Convert.ToInt32(Status.Active) + " ; "))
                    {
                        if (dr.Read())
                        {
                            _FdInterestRateValidation.pMinInterestRate = Convert.ToDecimal(dr["interestratefrom"]);  // Minimum Interest Rate
                            _FdInterestRateValidation.pMaxInterestRate = Convert.ToDecimal(dr["interestrateto"]);     // Maximum Interest Rate
                            _FdInterestRateValidation.pReferralCommisionvalue = Convert.ToDecimal(dr["commissionvalue"]);//ReferralCommission value
                            _FdInterestRateValidation.pReferralcommisiontype = dr["referralcommissiontype"];
                            _FdInterestRateValidation.pRatePerSquareYard =Convert.ToDecimal(dr["rate_per_square_yard"]);
                            _FdInterestRateValidation.pValuefor100 = Convert.ToDecimal(dr["valuefor100"]);
                            _FdInterestRateValidation.pCaltype = Convert.ToString(dr["caltype"]);
                        }
                    }
                    _FdInterestRateValidation.pTenureCount = ValidCount;
                    //strTenureandInterstRateCount = ValidCount.ToString() + "-" + InterestRateFrom.ToString() + "-" + InterestRateTo.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }       
            return _FdInterestRateValidation;
        }

        public async Task<List<FdSchemeData>> GetFdSchemeDetailsforGrid(string Fdname, string ApplicantType, string MemberType, string ConnectionString)
        {
            List<FdSchemeData> FdSchemeDataList = new List<FdSchemeData>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdcalculationmode,coalesce( mindepositamount,0) as mindepositamount, coalesce( maxdepositamount,0) as maxdepositamount,investmentperiodfrom,investmentperiodto,interestpayout,interesttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,coalesce(multiplesof,0) as multiplesof,coalesce(valuefor100,0) as valuefor100,caltype,coalesce(tenure,0) as tabletenure,coalesce(maturityamount,0) as tablematurityamount,  coalesce(depositamount,0) tabledepositamount , coalesce(interestamount,0) as tableinterestamount,  coalesce(payindenomination,0) as  tablepayindenomination, tenuremode as  tabletenuremode from tblmstfixeddepositconfigdetails where fdname='" + Fdname + "' and applicanttype='" + ApplicantType + "' and membertype='" + MemberType + "' and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FdSchemeData _FdSchemeData = new FdSchemeData();
                            List<TenureModes> _Tenuremodeslist = new List<TenureModes>();
                            //table start
                            _FdSchemeData.pFdtabletenure = dr["tabletenure"];
                            _FdSchemeData.pFdtablematurityamount = dr["tablematurityamount"];
                            _FdSchemeData.pFdtabledepositamount = dr["tabledepositamount"];
                            _FdSchemeData.pFdtableinterestamount = dr["tableinterestamount"];
                            _FdSchemeData.pFdtablepayindenomination = dr["tablepayindenomination"];
                            _FdSchemeData.pFdtabletenuremode = dr["tabletenuremode"];

                            //table end


                            _FdSchemeData.pFdCalculationmode = dr["fdcalculationmode"];
                            _FdSchemeData.pMinDepositAmount = Convert.ToDecimal(dr["mindepositamount"]);
                            _FdSchemeData.pMaxdepositAmount = Convert.ToDecimal(dr["maxdepositamount"]);
                            _FdSchemeData.pInvestmentPeriodFrom = dr["investmentperiodfrom"];
                            _FdSchemeData.pInvestmentPeriodTo = dr["investmentperiodto"];
                            _FdSchemeData.pInterestPayOut = dr["interestpayout"];
                            _FdSchemeData.pInterestType = dr["interesttype"];
                            _FdSchemeData.pInterestRateFrom = Convert.ToDecimal(dr["interestratefrom"]);
                            _FdSchemeData.pInterestRateTo = Convert.ToDecimal(dr["interestrateto"]);
                            _FdSchemeData.pMultiplesof = Convert.ToDecimal(dr["multiplesof"]);
                            _FdSchemeData.pValuefor100 = Convert.ToDecimal(dr["valuefor100"]);
                            _FdSchemeData.pCaltype = Convert.ToString(dr["caltype"]);
                            if (Convert.ToString(_FdSchemeData.pInvestmentPeriodTo) != string.Empty)
                            {
                                if (Convert.ToString(_FdSchemeData.pInvestmentPeriodTo).Contains(' '))
                                {
                                    string[] strTenures = Convert.ToString(_FdSchemeData.pInvestmentPeriodTo).Split(' ');
                                    if (strTenures.Length > 0)
                                    {
                                        for (int k = 0; k < strTenures.Length; k++)
                                        {
                                            TenureModes _Tenuremodes = new TenureModes();
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
                                    TenureModes _Tenuremodes = new TenureModes();
                                    if (Convert.ToString(_FdSchemeData.pInvestmentPeriodTo).Contains('Y'))
                                    {
                                        _Tenuremodes.pTenurename = "Years";
                                    }
                                    else if (Convert.ToString(_FdSchemeData.pInvestmentPeriodTo).Contains('M'))
                                    {
                                        _Tenuremodes.pTenurename = "Months";
                                    }
                                    else if (Convert.ToString(_FdSchemeData.pInvestmentPeriodTo).Contains('D'))
                                    {
                                        _Tenuremodes.pTenurename = "Days";
                                    }
                                    _Tenuremodeslist.Add(_Tenuremodes);
                                }
                            }
                            _FdSchemeData.pTenureModesList = _Tenuremodeslist;
                            FdSchemeDataList.Add(_FdSchemeData);
                        }
                        
                        //if (FdSchemeDataList!=null && FdSchemeDataList.Count>0)
                        //{                            
                        //    for (int i=0;i< FdSchemeDataList.Count;i++)
                        //    {
                        //        if (Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo) != string.Empty)
                        //        {
                        //            if (Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo).Contains(' '))
                        //            {
                        //                string[] strTenures = Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo).Split(' ');
                        //                if (strTenures.Length > 0)
                        //                {
                        //                    for (int k = 0; k < strTenures.Length; k++)
                        //                    {
                        //                        TenureModes _Tenuremodes = new TenureModes();
                        //                        if (Convert.ToString(FdSchemeDataList[k].pInvestmentPeriodTo).Contains('Y'))
                        //                        {
                        //                            _Tenuremodes.pTenurename = "Years";
                        //                        }
                        //                        else if (Convert.ToString(FdSchemeDataList[k].pInvestmentPeriodTo).Contains('M'))
                        //                        {
                        //                            _Tenuremodes.pTenurename = "Months";
                        //                        }
                        //                        else if (Convert.ToString(FdSchemeDataList[k].pInvestmentPeriodTo).Contains('D'))
                        //                        {
                        //                            _Tenuremodes.pTenurename = "Days";
                        //                        }
                        //                        _Tenuremodeslist.Add(_Tenuremodes);
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                TenureModes _Tenuremodes = new TenureModes();
                        //                if (Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo).Contains('Y'))
                        //                {
                        //                    _Tenuremodes.pTenurename = "Years";
                        //                }
                        //                else if (Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo).Contains('M'))
                        //                {
                        //                    _Tenuremodes.pTenurename = "Months";
                        //                }
                        //                else if (Convert.ToString(FdSchemeDataList[i].pInvestmentPeriodTo).Contains('D'))
                        //                {
                        //                    _Tenuremodes.pTenurename = "Days";
                        //                }
                        //                _Tenuremodeslist.Add(_Tenuremodes);
                        //            }
                        //        }
                        //    }
                        //}
                       
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return FdSchemeDataList;
        }

        public async Task<List<FiMemberContactDetails>> GetallJointMembers(string membercode,string Contacttype,string ConnectionString)
        {
            var _FiMemberContactDetails = new List<FiMemberContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode, te.contactid, te.contacttype, te.contactreferenceid, membername, coalesce(membertype,'') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, businessentitycontactno as contactnumber, businessentityemailid as emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tc on tc.contactid = te.contactid where   te.statusid=" + Convert.ToInt32(Status.Active) + " and membercode !='"+ membercode + "' and te.contacttype='"+ Contacttype + "' order by membername; "))
                    {
                        while (dr.Read())
                        {
                            var _FIMemberContactDTO = new FiMemberContactDetails
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

        public async Task<List<TenureModes>> GetFdSchemeTenureModes(string Fdname, string ApplicantType, string MemberType, string ConnectionString)
        {
            List<TenureModes> _Tenuremodeslist = new List<TenureModes>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select investmentperiodfrom,investmentperiodto from tblmstfixeddepositconfigdetails where fdname='" + Fdname + "' and applicanttype='" + ApplicantType + "' and membertype='" + MemberType + "'  and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                           
                            if (Convert.ToString(dr["investmentperiodto"]) != string.Empty)
                            {
                                if (Convert.ToString(dr["investmentperiodto"]).Contains(' '))
                                {
                                    string[] strTenures = Convert.ToString(dr["investmentperiodto"]).Split(' ');
                                    if (strTenures.Length > 0)
                                    {
                                        for (int k = 0; k < strTenures.Length; k++)
                                        {
                                            TenureModes _Tenuremodes = new TenureModes();
                                            if (Convert.ToString(strTenures[k]).Contains('Y'))
                                            {
                                                _Tenuremodes.pTenurename = "Years";
                                                _Tenuremodes.pSortorder = 3;
                                            }
                                            else if (Convert.ToString(strTenures[k]).Contains('M'))
                                            {
                                                _Tenuremodes.pTenurename = "Months";
                                                _Tenuremodes.pSortorder = 2;
                                            }
                                            else if (Convert.ToString(strTenures[k]).Contains('D'))
                                            {
                                                _Tenuremodes.pTenurename = "Days";
                                                _Tenuremodes.pSortorder = 1;
                                            }
                                            int count = 0;
                                            //if (_Tenuremodeslist.Count <= 0)
                                            //{
                                            //    _Tenuremodeslist.Add(_Tenuremodes);
                                            //}
                                            for (int i = 0; i < _Tenuremodeslist.Count; i++)
                                            {
                                                if (_Tenuremodeslist[i].pTenurename == _Tenuremodes.pTenurename)
                                                {
                                                    count++;
                                                }                                               
                                            }
                                            if (count == 0)
                                            {
                                                _Tenuremodeslist.Add(_Tenuremodes);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    TenureModes _Tenuremodes = new TenureModes();
                                    if (Convert.ToString(dr["investmentperiodto"]).Contains('Y'))
                                    {
                                        _Tenuremodes.pTenurename = "Years";
                                        _Tenuremodes.pSortorder = 3;
                                    }
                                    else if (Convert.ToString(dr["investmentperiodto"]).Contains('M'))
                                    {
                                        _Tenuremodes.pTenurename = "Months";
                                        _Tenuremodes.pSortorder = 2;
                                    }
                                    else if (Convert.ToString(dr["investmentperiodto"]).Contains('D'))
                                    {
                                        _Tenuremodes.pTenurename = "Days";
                                        _Tenuremodes.pSortorder = 1;
                                    }
                                    int count = 0;
                                    for(int i=0;i< _Tenuremodeslist.Count;i++)
                                    {
                                        if (_Tenuremodeslist[i].pTenurename == _Tenuremodes.pTenurename)
                                        {
                                            count++;
                                        }                                       
                                    }
                                    if (count == 0)
                                    {
                                        _Tenuremodeslist.Add(_Tenuremodes);
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
            });          
            return _Tenuremodeslist.OrderBy (x => x.pSortorder).ToList(); 
        }
    }
}
