using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class InsuranceMemberDAL : SettingsDAL, IInsuranceMember
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public List<InsuranceMemberBind> _InsuranceMemberList { get; set; }
        public List<InsuranceMembersDataforMainGrid> _InsuranceMembersDataforMainGridList { get; set; }
        public Viewmemberdetails _Viewmemberdetails { get; set; }
        public InsuranceschemeDetails _InsuranceschemeDetails { get; set; }
        public GetInsuranceMemberDataforEdit _GetInsuranceMemberDataforEdit { get; set; }   
        public List<InsuranceSchemes> _InsuranceSchemesList { get; set; }
        public List<Applicanttypesdata> _ApplicanttypesdataList { get; set; }
        public async Task<List<InsuranceMemberBind>> GetallInsuranceMembers(long MembertypeID,string ConnectionString)
        {
            _InsuranceMemberList = new List<InsuranceMemberBind>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membercode||'_'||membername as member,memberid,contactid,contactreferenceid from tblmstmembers where membertypeid="+ MembertypeID + " and statusid=" + Convert.ToInt32(Status.Active) + " order by membername desc;"))
                    {
                        while (dr.Read())
                        {
                            var _InsuranceMemberBind = new InsuranceMemberBind
                            {
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberCodeandName = Convert.ToString(dr["member"]),
                                Contactid= Convert.ToInt64(dr["contactid"]),
                                pContactrefid= Convert.ToString(dr["contactreferenceid"])
                            };
                            _InsuranceMemberList.Add(_InsuranceMemberBind);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _InsuranceMemberList;
        }
        public async Task<Viewmemberdetails> GetMemberDetailsforview(long MemberID, string ConnectionString)
        {
            await Task.Run(() =>
            { 
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select a.memberid,a.membername,a.contactreferenceid,a.membercode,a.transdate,a.membertype,a.dob,b.nomineename,b.relationship from (select memberid, membername, tm.contactreferenceid, membercode, tm.transdate, membertype, dob from tblmstmembers tm join tblmstcontact tc on tc.contactreferenceid = tm.contactreferenceid where memberid = " + MemberID + ") a left join (select vchapplicationid, nomineename, relationship from tabapplicationpersonalnomineedetails tp join tblmstmembers tm on tm.membercode = tp.vchapplicationid where tp.isprimarynominee = true and tm.memberid="+ MemberID + ") b on a.membercode = b.vchapplicationid;"))
                {
                    while (dr.Read())
                    {
                        _Viewmemberdetails = new Viewmemberdetails
                        {
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pAccountholdername = Convert.ToString(dr["membername"]),
                            pContactrefid = Convert.ToString(dr["contactreferenceid"]),
                            pMemberrefcode = Convert.ToString(dr["membercode"]),
                            pJoiningDate = dr["transdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy"),
                            pMembertype = Convert.ToString(dr["membertype"]),
                            pNomineeName = Convert.ToString(dr["nomineename"]),
                            pDateofbirth = dr["dob"] == DBNull.Value ? null : Convert.ToDateTime(dr["dob"]).ToString("dd/MM/yyyy"),
                            pAge = dr["dob"] == DBNull.Value ? null :Convert.ToString(CalculateAgeCorrect(Convert.ToDateTime(dr["dob"]))),
                            pNomimneeRelation = Convert.ToString(dr["relationship"])
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            });
            return _Viewmemberdetails;
        }
        public async Task<InsuranceschemeDetails> GetInsuranceSchemeDetails(long InsuranceSchemeId, string ConnectionString)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct insuranceschemecode,tii.insuranceschemeconfigid,tii.insuranceschemename,insuranceduration,coalesce( premiumamountpayable,0)as premiumamountpayable ,premiumpayin,insuranceclaimpayoutevent,coalesce( insuranceclaimamount,0) as insuranceclaimamount,coalesce( premiumrefund,false) as premiumrefund  from tblmstinsuranceconfigdetails ti join tblmstinsuranceconfig tii on ti.insuranceschemeconfigid=tii.insuranceschemeconfigid where tii.insuranceschemeconfigid=" + InsuranceSchemeId + " and tii.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            _InsuranceschemeDetails = new InsuranceschemeDetails
                            {
                                InsuranceschemeId = Convert.ToInt64(dr["insuranceschemeconfigid"]),
                                InsuranceschemeCode = Convert.ToString(dr["insuranceschemecode"]),
                                Claimamount = Convert.ToDecimal(dr["insuranceclaimamount"]),
                                ClaimType = Convert.ToString(dr["insuranceclaimpayoutevent"]),
                                Premiumamount = Convert.ToDecimal(dr["premiumamountpayable"]),
                                PremiumPayin = Convert.ToString(dr["premiumpayin"]),
                                InsuranceschemeDuration = Convert.ToString(dr["insuranceduration"]),
                                IspremiumRefund = Convert.ToBoolean(dr["premiumrefund"]),
                                
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _InsuranceschemeDetails;
        }
        public async Task<List<InsuranceMemberNomineeDetails>> GetInsuranceMemberNomineeDetails(string MemberReferenceId, string ConnectionString)
        {
            var _InsuranceMemberNomineeDetailslist = new List<InsuranceMemberNomineeDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,vchapplicationid,contactid,contactreferenceid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, applicantype,statusid from tabapplicationpersonalnomineedetails where vchapplicationid='" + ManageQuote(MemberReferenceId) + "' and upper(applicantype)='INSURANCE' and statusid="+Convert.ToInt32(Status.Active)+";"))
                    {
                        while (dr.Read())
                        {
                            var _InsuranceMemberNomineeDetails = new InsuranceMemberNomineeDetails
                            {
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pNomineeName = Convert.ToString(dr["nomineename"]),
                                pNomimneeRelation = Convert.ToString(dr["relationship"]),
                                pContactrefid = Convert.ToString(dr["contactreferenceid"]),
                                pMemberrefcode = Convert.ToString(dr["vchapplicationid"]),
                                ContactNo = Convert.ToString(dr["contactno"]),
                                IdProofname = Convert.ToString(dr["idproofname"]),
                                Idproofpath = Convert.ToString(dr["docidproofpath"]),
                                IdproofReferenceNo = Convert.ToString(dr["referencenumber"]),
                                Isprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                                pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false,
                                Dateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                                pAge = dr["dateofbirth"] != DBNull.Value ? CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"])) : 0,
                                ptypeofoperation = "OLD",
                                IdproofType = Convert.ToString(dr["idprooftype"]),
                                Contactid = Convert.ToInt64(dr["contactid"])
                            };
                            _InsuranceMemberNomineeDetailslist.Add(_InsuranceMemberNomineeDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _InsuranceMemberNomineeDetailslist;
        }
        public bool SaveInsuranceMemberData(InsuranceMembersave _InsuranceMembersave, string ConnectionString)
        {
            StringBuilder sbSaveInsuranceMember = new StringBuilder();            
            StringBuilder sbupdate = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();              
                if (string.IsNullOrEmpty(_InsuranceMembersave.pTransdate))
                {
                    _InsuranceMembersave.pTransdate = "null";
                }
                else
                {
                    _InsuranceMembersave.pTransdate = "'" + FormatDate(_InsuranceMembersave.pTransdate) + "'";
                }
                if (string.IsNullOrEmpty(_InsuranceMembersave.pPolicystartdate))
                {
                    _InsuranceMembersave.pPolicystartdate = "null";
                }
                else
                {
                    _InsuranceMembersave.pPolicystartdate = "'" + FormatDate(_InsuranceMembersave.pPolicystartdate) + "'";
                }
                if (string.IsNullOrEmpty(_InsuranceMembersave.pPolicyenddate))
                {
                    _InsuranceMembersave.pPolicyenddate = "null";
                }
                else
                {
                    _InsuranceMembersave.pPolicyenddate = "'" + FormatDate(_InsuranceMembersave.pPolicyenddate) + "'";
                }        
                if(Convert.ToString(_InsuranceMembersave.pPremiumamount)==string.Empty)
                {
                    _InsuranceMembersave.pPremiumamount = 0;
                }              

                if( !string.IsNullOrEmpty ( _InsuranceMembersave.ptypeofoperation))
                {
                    if (ManageQuote(_InsuranceMembersave.ptypeofoperation).ToUpper() == "CREATE")
                    {
                        sbSaveInsuranceMember.AppendLine("INSERT INTO tbltransinsurancemember(transdate, insurancetype, membertypeid, membertype,membercode, membername, insuranceschemeconfigid, insuranceschemename,policystartdate, policyenddate, policycoverageperiod, statusid,createdby, createddate,premiumamount,applicanttype) VALUES (" + _InsuranceMembersave.pTransdate + ",'" + _InsuranceMembersave.pInsuranceType + "'," + _InsuranceMembersave.pMembertypeId + ",'" + _InsuranceMembersave.pMembertype + "','" + _InsuranceMembersave.pMemberCodeandName.Split('_')[0] + "','" + _InsuranceMembersave.pMemberCodeandName.Split('_')[1] + "'," + _InsuranceMembersave.pSchemeId + " ,'" + _InsuranceMembersave.pSchemeName + "'," + _InsuranceMembersave.pPolicystartdate + "," + _InsuranceMembersave.pPolicyenddate + ",'" + _InsuranceMembersave.pPolicycoveragePeriod + "'," + Convert.ToInt32(Status.Active) + "," + _InsuranceMembersave.pCreatedby + ",current_timestamp," + _InsuranceMembersave.pPremiumamount + " ,'" + ManageQuote(_InsuranceMembersave.pApplicanttype) + "');");
                    }

                    else if (ManageQuote(_InsuranceMembersave.ptypeofoperation).ToUpper() == "UPDATE")
                    {
                        sbSaveInsuranceMember.AppendLine("UPDATE tbltransinsurancemember SET  transdate=" + _InsuranceMembersave.pTransdate + ", insurancetype='" + _InsuranceMembersave.pInsuranceType + "', membertypeid=" + _InsuranceMembersave.pMembertypeId + ", membertype='" + _InsuranceMembersave.pMembertype + "',insuranceschemeconfigid=" + _InsuranceMembersave.pSchemeId + ", insuranceschemename='" + _InsuranceMembersave.pSchemeName + "', policystartdate=" + _InsuranceMembersave.pPolicystartdate + ", policyenddate=" + _InsuranceMembersave.pPolicyenddate + ", policycoverageperiod='" + _InsuranceMembersave.pPolicycoveragePeriod + "',  modifiedby=" + _InsuranceMembersave.pCreatedby + ", modifieddate=current_timestamp,premiumamount=" + _InsuranceMembersave.pPremiumamount + ",applicanttype='" + ManageQuote(_InsuranceMembersave.pApplicanttype) + "' WHERE membercode='" + _InsuranceMembersave.pMemberCodeandName.Split('_')[0] + "' and insurancetype='" + _InsuranceMembersave.pInsuranceType + "';");

                    }                 
                }               
                string Recordid = string.Empty;
                if (_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave != null && _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave.Count > 0)
                {
                    for (int i = 0; i < _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation) && _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation.Trim().ToUpper() != "CREATE"
                            )
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pRecordid.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pRecordid.ToString();
                            }
                        }
                        if (!string.IsNullOrEmpty(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation))
                        {
                            _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation = _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth))
                        {
                            _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth = "null";
                        }
                        else
                        {
                            _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth = "'" + FormatDate(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth) + "'";
                        }
                        if (_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation == "CREATE")
                        {
                            sbSaveInsuranceMember.AppendLine("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee) values ('" + _InsuranceMembersave.pMemberId + "', '" + ManageQuote(_InsuranceMembersave.pMemberCodeandName.Split('_')[0]) + "', '" + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Contactid + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pContactrefid) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pNomineeName) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pNomimneeRelation) + "', " + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth + ", '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ContactNo) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdproofType) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdProofname) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdproofReferenceNo) + "', '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Idproofpath) + "', " + Convert.ToInt32(Status.Active) + ", '" + _InsuranceMembersave.pCreatedby + "', current_timestamp,'INSURANCE'," + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Isprimarynominee + ");");
                        }
                        if (_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ptypeofoperation != "CREATE")
                        {
                            sbSaveInsuranceMember.AppendLine("update tabapplicationpersonalnomineedetails set contactid = '" + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Contactid + "', contactreferenceid = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pContactrefid) + "', nomineename = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pNomineeName) + "', relationship = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pNomimneeRelation) + "', dateofbirth = " + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Dateofbirth + ", contactno = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].ContactNo) + "', idprooftype = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdproofType) + "', idproofname = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdProofname) + "', referencenumber = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].IdproofReferenceNo) + "', docidproofpath = '" + ManageQuote(_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Idproofpath) + "', statusid = " + Convert.ToInt32(Status.Active) + ", modifiedby = '" + _InsuranceMembersave.pCreatedby + "', modifieddate = current_timestamp,isprimarynominee=" + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].Isprimarynominee + " where vchapplicationid = '" + ManageQuote(_InsuranceMembersave.pMemberCodeandName.Split('_')[0]) + "' and applicationid = " + _InsuranceMembersave.pMemberId + " and recordid = " + _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave[i].pRecordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    sbupdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _InsuranceMembersave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" +_InsuranceMembersave.pMemberId + " and vchapplicationid='" + ManageQuote(_InsuranceMembersave.pMemberCodeandName.Split('_')[0]) + "' AND RECORDID not in(" + Recordid + ") and upper(applicantype)='INSURANCE'; ");
                }
                else
                {
                    if (_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave == null || _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave.Count == 0)
                    {
                        sbupdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _InsuranceMembersave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + _InsuranceMembersave.pMemberId + " and vchapplicationid='" + ManageQuote(_InsuranceMembersave.pMemberCodeandName.Split('_')[0]) + "' and upper(applicantype)='INSURANCE'; ");
                    }
                }
                if (Convert.ToString(sbSaveInsuranceMember) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString() + " " + sbSaveInsuranceMember.ToString());
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
        public async Task<List<InsuranceMembersDataforMainGrid>> GetInsuranceMembersMainGrid(string ConnectionString)
        {
            _InsuranceMembersDataforMainGridList = new List<InsuranceMembersDataforMainGrid>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ti.recordid, memberid, ti.membername, ti.membercode, coalesce( premiumamount,0) as premiumamount,to_char(policystartdate,'DD/MM/YYYY') policystartdate, to_char(policyenddate,'DD/MM/YYYY') policyenddate, policycoverageperiod,ti.membercode || '_' || ti.membername as membercodeandname from tbltransinsurancemember ti  join tblmstmembers tm on tm.membercode = ti.membercode where ti.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _InsuranceMembersDataforMainGrid = new InsuranceMembersDataforMainGrid
                            {
                                pRecordid= Convert.ToInt64(dr["recordid"]),
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberCodeandName = Convert.ToString(dr["membercodeandname"]),
                                pMembername = Convert.ToString(dr["membername"]),
                                pMembercode = Convert.ToString(dr["membercode"]),
                                Premiumamount = Convert.ToDecimal(dr["premiumamount"]),
                                pPolicystartdate = Convert.ToString(dr["policystartdate"]),
                                pPolicyenddate = Convert.ToString(dr["policyenddate"]),
                                pPolicycoveragePeriod = Convert.ToString(dr["policycoverageperiod"]),
                              //  pNomineeName = Convert.ToString(dr["nomineename"])
                            };
                            _InsuranceMembersDataforMainGridList.Add(_InsuranceMembersDataforMainGrid);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _InsuranceMembersDataforMainGridList;
        }
        public  async Task<GetInsuranceMemberDataforEdit> GetMemberDetailsforEdit( long Recordid, string ConnectionString)
        {           
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ti.recordid, coalesce(ti.applicanttype,'') as applicanttype, to_char(ti.transdate,'DD/MM/YYYY')transdate,insurancetype,tm.membertypeid,tm.membertype, memberid, ti.membername, ti.membercode,ti.membercode || '_' || ti.membername as membercodeandname,ti.insuranceschemeconfigid,ti.insuranceschemename,to_char(policystartdate,'DD/MM/YYYY') policystartdate, to_char(policyenddate,'DD/MM/YYYY') policyenddate, policycoverageperiod,tm.contactid,tm.contactreferenceid from tbltransinsurancemember ti  join tblmstmembers tm on tm.membercode = ti.membercode where  ti.recordid="+Recordid+"  and ti.statusid=" + Convert.ToInt32(Status.Active)+";"))
                    {
                        while (dr.Read())
                        {
                            _GetInsuranceMemberDataforEdit = new GetInsuranceMemberDataforEdit()
                            {
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberCodeandName = Convert.ToString(dr["membercodeandname"]),
                                pInsuranceType = Convert.ToString(dr["insurancetype"]),
                                pTransdate = Convert.ToString(dr["transdate"]),
                                pMembertype = Convert.ToString(dr["membertype"]),
                                pMembertypeId = Convert.ToInt64(dr["membertypeid"]),
                                pSchemeId = Convert.ToInt64(dr["insuranceschemeconfigid"]),
                                pSchemeName = Convert.ToString(dr["insuranceschemename"]),
                                pPolicystartdate = dr["policystartdate"] == DBNull.Value ? null : Convert.ToString(dr["policystartdate"]),
                                pPolicyenddate = dr["policyenddate"] == DBNull.Value ? null : Convert.ToString(dr["policyenddate"]),
                                pPolicycoveragePeriod = Convert.ToString(dr["policycoverageperiod"]),
                                pApplicanttype = Convert.ToString(dr["applicanttype"]),
                                Contactid = Convert.ToInt64(dr["contactid"]),
                                pContactrefid= Convert.ToString(dr["contactreferenceid"])
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _GetInsuranceMemberDataforEdit;
        }
        public bool DeleteInsuranceMember(string MemberReferenceID, long Userid, string ConnectionString)
        {
            StringBuilder SbDeleteInsuranceMember = new StringBuilder();
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
                long Memberid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select memberid from tblmstmembers where upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "' ;"));

                // Nominee Details 
                SbDeleteInsuranceMember.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "' and applicationid=" + Memberid + " and upper(applicantype)='INSURANCE'; ");

                // Insurance Member Details
                SbDeleteInsuranceMember.AppendLine("UPDATE tbltransinsurancemember SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");                

                if (Convert.ToString(SbDeleteInsuranceMember) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbDeleteInsuranceMember.ToString());
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
        public async Task<List<InsuranceSchemes>> GetInsuranceSchemes(string Membertype, string Applicanttype, string ConnectionString)
        {
            _InsuranceSchemesList = new List<InsuranceSchemes>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select insuranceschemeconfigid,insuranceschemename from tblmstinsuranceconfigdetails where upper(membertype) = '" + ManageQuote(Membertype).ToUpper() + "' and upper(applicanttype) ='" + ManageQuote(Applicanttype).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _InsuranceSchemes = new InsuranceSchemes
                            {
                                pInsurenceconfigid = Convert.ToInt64(dr["insuranceschemeconfigid"]),
                                pInsurencename = Convert.ToString(dr["insuranceschemename"])                               
                            };
                            _InsuranceSchemesList.Add(_InsuranceSchemes);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _InsuranceSchemesList;
        }
        public async Task<List<Applicanttypesdata>> GetApplicants(string Connectionstring)
        {
            _ApplicanttypesdataList = new List<Applicanttypesdata>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct applicanttype from tblmstapplicantcongiguration where statusid = " + Convert.ToInt32(Status.Active) + " order by applicanttype desc;"))
                    {
                        while (dr.Read())
                        {
                            var _ApplicantsData = new Applicanttypesdata
                            {
                                pApplicanttype = Convert.ToString(dr["applicanttype"])
                            };
                            _ApplicanttypesdataList.Add(_ApplicantsData);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _ApplicanttypesdataList;
        }
        public int checkMemberCountinMaster(string ContactReferenceId, long InsuranceID, string Connectionstring,string InsuranceType,long Recordid)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select count(*) from tbltransinsurancemember where upper(insurancetype)='" + ManageQuote(InsuranceType).ToUpper() + "' and  insuranceschemeconfigid = " + InsuranceID + " and upper(membercode)='" + ManageQuote(ContactReferenceId).ToUpper() + "' and recordid!=" + Recordid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
