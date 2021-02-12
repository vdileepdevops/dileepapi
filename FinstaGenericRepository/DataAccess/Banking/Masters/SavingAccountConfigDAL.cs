using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Masters;
namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class SavingAccountConfigDAL: SettingsDAL,ISavingAccountConfig
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public List<SavingAccountConfigDTO> listsavingAccountConfig { get; set; }
        public List<SavingAccountNameandCodeDTO> listSavingAccountNameandCodeDTO { get; set; }
        public List<SavingAccountConfigDetailsDTO> listSavingAccountConfigDetails { get; set; }
        public int checkInsertAccNameandCodeDuplicates(string checkparamtype, string Accname, string Acccode, Int64 SavingAccid, string connectionstring)
        {
            int count = 0;
            try
            {
                if (checkparamtype.ToUpper() == "ACCNAME")
                {
                    if (string.IsNullOrEmpty(SavingAccid.ToString()) || SavingAccid == 0)
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingaccname)='" + ManageQuote(Accname.Trim().ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ""));
                    }
                    else
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingaccname)='" + ManageQuote(Accname.Trim().ToUpper()) + "' and savingconfigid!=" + SavingAccid + "  and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(SavingAccid.ToString()) || SavingAccid == 0)
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingacccode)='" + ManageQuote(Acccode.Trim().ToUpper()) + "'  and statusid=" + Convert.ToInt32(Status.Active) + ""));
                    }
                    else
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingacccode)='" + ManageQuote(Acccode.Trim().ToUpper()) + "' and savingconfigid!=" + SavingAccid + "  and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }

        public SavingschemeandcodeCount GetSavingAccNameCodeCount(Int64 SavingAccid, string SavingAccName, string SavingAccCode, string ConnectionString)
        {
            SavingschemeandcodeCount _SavingschemeandcodeCount = new SavingschemeandcodeCount();
            try
            {
                if(SavingAccid == 0)
                {
                    _SavingschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingaccname)='" + ManageQuote(SavingAccName).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _SavingschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingacccode)='" + ManageQuote(SavingAccCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _SavingschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingaccname)='" + ManageQuote(SavingAccName).ToUpper() + "' and savingconfigid <> "+ SavingAccid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _SavingschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstSavingAccountConfig where upper(savingacccode)='" + ManageQuote(SavingAccCode).ToUpper() + "' and savingconfigid <> " + SavingAccid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _SavingschemeandcodeCount;
        }

        public int SaveSavingAccountNameAndCode(SavingAccountNameandCodeDTO objSavingAccountNameandCode, string connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            int SavingConfigid =0;
          
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
               
              
                if (objSavingAccountNameandCode.ptypeofoperation == "CREATE")
                {
                    SavingConfigid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstSavingAccountConfig(savingaccname,savingacccode,companycode,branchcode,series,serieslength,savingaccnamecode,statusid,createdby,createddate)values('" + ManageQuote(objSavingAccountNameandCode.pSavingAccountname) + "','" + ManageQuote(objSavingAccountNameandCode.pSavingAccountcode.Trim()) + "','" + ManageQuote(objSavingAccountNameandCode.pCompanycode.Trim()) + "','" + ManageQuote(objSavingAccountNameandCode.pBranchcode) + "','" + ManageQuote(objSavingAccountNameandCode.pSeries) + "'," + objSavingAccountNameandCode.pSerieslength + ",'" + ManageQuote(objSavingAccountNameandCode.pSavingaccnamecode) + "'," + Convert.ToInt32(Status.Active) + "," + objSavingAccountNameandCode.pCreatedby + ",current_timestamp) returning savingconfigid"));
                }
                trans.Commit();
               
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
            return SavingConfigid;
        }

        public bool SaveSavingAccountConfiguration(SavingAccountConfigDTO objsavingAccountConfig, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            string Recordid = string.Empty;
            string query = "";
            int createdby = 0;
            Int64 savingConfigid = 0;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                for (int i = 0; i < objsavingAccountConfig.savingAccountConfiglist.Count; i++) {
                    if (objsavingAccountConfig.savingAccountConfiglist[i].ptypeofoperation != "CREATE")
                    {
                      
                        if (string.IsNullOrEmpty(Recordid))
                        {
                            Recordid = objsavingAccountConfig.savingAccountConfiglist[i].pRecordid.ToString();
                        }
                        else
                        {
                            Recordid = Recordid + "," + objsavingAccountConfig.savingAccountConfiglist[i].pRecordid.ToString();
                        }
                    }
                    if (objsavingAccountConfig.savingAccountConfiglist != null)
                    {
                        if (string.IsNullOrEmpty(objsavingAccountConfig.savingAccountConfiglist[i].pSavingmindepositamount.ToString()))
                        {
                            objsavingAccountConfig.savingAccountConfiglist[i].pSavingmindepositamount = 0;
                        }
                        if (string.IsNullOrEmpty(objsavingAccountConfig.savingAccountConfiglist[i].pSavingmaxdepositamount.ToString()))
                        {
                            objsavingAccountConfig.savingAccountConfiglist[i].pSavingmaxdepositamount = 0;
                        }
                        if (string.IsNullOrEmpty(objsavingAccountConfig.savingAccountConfiglist[i].pMaxwithdrawllimit.ToString()))
                        {
                            objsavingAccountConfig.savingAccountConfiglist[i].pMaxwithdrawllimit = 0;
                        }
                        if (string.IsNullOrEmpty(objsavingAccountConfig.savingAccountConfiglist[i].pPenaltyvalue.ToString()))
                        {
                            objsavingAccountConfig.savingAccountConfiglist[i].pPenaltyvalue = 0;
                        }
                        if (objsavingAccountConfig.savingAccountConfiglist[i].ptypeofoperation == "UPDATE")
                        {
                            sbinsert.Append("update tblmstSavingAccountConfigdetails set membertypeid=" + objsavingAccountConfig.savingAccountConfiglist[i].pMembertypeid + ",membertype='" + objsavingAccountConfig.savingAccountConfiglist[i].pMembertype + "',applicanttype='" + objsavingAccountConfig.savingAccountConfiglist[i].pApplicanttype + "',minopenamount=" + objsavingAccountConfig.savingAccountConfiglist[i].pMinopenamount + ",minmaintainbalance =" + objsavingAccountConfig.savingAccountConfiglist[i].pMinbalance + ",interestpayout='" + objsavingAccountConfig.savingAccountConfiglist[i].pInterestpayout + "',interestrate=" + objsavingAccountConfig.savingAccountConfiglist[i].pInterestrate + ",iswithdrawallimitapplicable=" + objsavingAccountConfig.savingAccountConfiglist[i].pIswithdrawallimitapplicable + ",withdrawallimitpayout='" + objsavingAccountConfig.savingAccountConfiglist[i].pWithdrawallimitpayout + "',maxwithdrawallimit=" + objsavingAccountConfig.savingAccountConfiglist[i].pMaxwithdrawllimit + ",ispenaltyapplicableonminbal=" + objsavingAccountConfig.savingAccountConfiglist[i].pIspenaltyapplicableonminbal + ",penaltycaltype='" + objsavingAccountConfig.savingAccountConfiglist[i].pPenaltycaltype + "',penaltyvalue=" + objsavingAccountConfig.savingAccountConfiglist[i].pPenaltyvalue + ",issavingspayinapplicable=" + objsavingAccountConfig.savingAccountConfiglist[i].pIssavingspayinapplicable + "," +
                                "savingspayinmode='" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingspayinmode + "',savingmindepositamount=" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingmindepositamount + ",savingmaxdepositamount=" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingmaxdepositamount + ",modifiedby =" + objsavingAccountConfig.pCreatedby + ",modifieddate=current_timestamp where savingconfigid =" + objsavingAccountConfig.pSavingAccountid + ";");
                        }
                        else if (objsavingAccountConfig.savingAccountConfiglist[i].ptypeofoperation == "CREATE")
                        {
                            sbinsert.Append("insert into tblmstSavingAccountConfigdetails(savingconfigid, savingaccname, membertypeid, membertype, applicanttype, minopenamount, minmaintainbalance, interestpayout , interestrate, iswithdrawallimitapplicable, withdrawallimitpayout, maxwithdrawallimit ,ispenaltyapplicableonminbal,penaltycaltype,penaltyvalue,issavingspayinapplicable,savingspayinmode,savingmindepositamount,savingmaxdepositamount, statusid, createdby, createddate)values(" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingConfigid + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingAccname + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pMembertypeid + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pMembertype + "','" + objsavingAccountConfig.savingAccountConfiglist[i].pApplicanttype + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pMinopenamount + "," + objsavingAccountConfig.savingAccountConfiglist[i].pMinbalance + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pInterestpayout + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pInterestrate + "," + objsavingAccountConfig.savingAccountConfiglist[i].pIswithdrawallimitapplicable + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pWithdrawallimitpayout + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pMaxwithdrawllimit + "," + objsavingAccountConfig.savingAccountConfiglist[i].pIspenaltyapplicableonminbal + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pPenaltycaltype + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pPenaltyvalue + "," + objsavingAccountConfig.savingAccountConfiglist[i].pIssavingspayinapplicable + ",'" + objsavingAccountConfig.savingAccountConfiglist[i].pSavingspayinmode + "'," + objsavingAccountConfig.savingAccountConfiglist[i].pSavingmindepositamount + "," + objsavingAccountConfig.savingAccountConfiglist[i].pSavingmaxdepositamount + "," + Convert.ToInt32(Status.Active) + "," + objsavingAccountConfig.savingAccountConfiglist[i].pCreatedby + ",current_timestamp);");
                        }


                       
                    }
                }

                if (!string.IsNullOrEmpty(Recordid))
                {
                    query = "update tblmstSavingAccountConfigdetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + objsavingAccountConfig.pCreatedby + ",modifieddate=current_timestamp where savingconfigid=" + objsavingAccountConfig.pSavingAccountid + "  and recordid not in(" + Recordid + ");";
                }
                else
                {
                    query = "update tblmstSavingAccountConfigdetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + objsavingAccountConfig.pCreatedby + ",modifieddate=current_timestamp where savingconfigid=" + objsavingAccountConfig.pSavingAccountid + ";";
                }
                if (!string.IsNullOrEmpty(query))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query);
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

        public bool SaveLoanFacility(LoanFacilityDTO objLoanFacility, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
               
                    if (string.IsNullOrEmpty(objLoanFacility.pEligiblepercentage.ToString()))
                    {
                    objLoanFacility.pEligiblepercentage = 0;
                    }
                    if (string.IsNullOrEmpty(objLoanFacility.pAgeperiod.ToString()))
                    {
                    objLoanFacility.pAgeperiod = 0;
                    }
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tblmstSavingAccountLoansConfig where savingconfigid=" + objLoanFacility.pSavingConfigid + " and statusid=" + Convert.ToInt32(Status.Active) + ""));
                    if (count==0)
                    {
                        sbinsert.Append("insert into tblmstSavingAccountLoansConfig(savingconfigid, savingaccname, isloanfacilityapplicable, eligiblepercentage, ageperiod, ageperiodtype, statusid, createdby, createddate)values(" + objLoanFacility.pSavingConfigid+ ",'" + objLoanFacility.pSavingAccname + "'," + objLoanFacility.pIsloanfacilityapplicable + "," + objLoanFacility.pEligiblepercentage + "," + objLoanFacility.pAgeperiod + ",'" + objLoanFacility.pAgeperiodtype + "'," + Convert.ToInt32(Status.Active) + "," + objLoanFacility.pCreatedby + ",current_timestamp);");
                    }
                    if (count != 0)
                    {
                        sbinsert.Append("update tblmstSavingAccountLoansConfig set  isloanfacilityapplicable=" + objLoanFacility.pIsloanfacilityapplicable + ", eligiblepercentage=" + objLoanFacility.pEligiblepercentage + ", ageperiod=" + objLoanFacility.pAgeperiod + ", ageperiodtype='" + objLoanFacility.pAgeperiodtype + "',modifiedby =" + objLoanFacility.pCreatedby + ",modifieddate=current_timestamp where savingconfigid =" + objLoanFacility.pSavingConfigid + ";");
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

        public bool SaveIdentificationdocuments(IdentificationDocumentsDTO obIdentificationDocuments, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (obIdentificationDocuments.identificationdocumentsList != null)
                {
                    //if (obIdentificationDocuments.ptypeofoperation == "UPDATE")
                    //{
                    //if (obIdentificationDocuments.identificationdocumentsList.Count > 0)
                    //{
                    
                    sbinsert.Append("delete from  tblmstloanwisedocumentproofs where loanid=" + obIdentificationDocuments.pSavingConfigid + ";");
                        //}
                    //}
                    for (int i = 0; i < obIdentificationDocuments.identificationdocumentsList.Count; i++)
                    {
                        if (obIdentificationDocuments.identificationdocumentsList[i].pDocumentRequired == true || obIdentificationDocuments.identificationdocumentsList[i].pDocumentMandatory == true)
                        {
                            sbinsert.Append("insert into tblmstloanwisedocumentproofs(loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory,statusid,createdby,createddate) values(" + obIdentificationDocuments.pSavingConfigid + "," + obIdentificationDocuments.pSavingConfigid + ",'" + ManageQuote(obIdentificationDocuments.identificationdocumentsList[i].pContactType) + "'," + obIdentificationDocuments.identificationdocumentsList[i].pDocumentId + "," + obIdentificationDocuments.identificationdocumentsList[i].pDocumentgroupId + ",'" + obIdentificationDocuments.identificationdocumentsList[i].pDocumentRequired + "','" + obIdentificationDocuments.identificationdocumentsList[i].pDocumentMandatory + "'," + Convert.ToInt32(Status.Active) + "," + obIdentificationDocuments.identificationdocumentsList[i].pCreatedby + ",current_timestamp);");
                        }
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

        public bool SaveReferralCommission(ReferralCommissionDTO objReferralCommission,string ConnectionString)
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
               
                    if (string.IsNullOrEmpty(objReferralCommission.pCommissionValue.ToString()))
                    {
                    objReferralCommission.pCommissionValue = 0;
                    }
                  if (string.IsNullOrEmpty(objReferralCommission.pTdspercentage.ToString()))
                {
                    objReferralCommission.pTdspercentage = 0;
                }
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tblmstSavingAccountConfigreferraldetails where savingconfigid=" + objReferralCommission.pSavingConfigid + " and statusid=" + Convert.ToInt32(Status.Active) + ""));
                if (count != 0)
                    {
                        sbinsert.Append("Update tblmstSavingAccountConfigreferraldetails set isreferralcommissionapplicable="+objReferralCommission.pIsreferralcommissionapplicable + ", referralcommissioncalfield='" + ManageQuote(objReferralCommission.pReferralcommissioncalfield) + "',referralcommissiontype='" + objReferralCommission.pReferralcommissiontype + "' ,commissionValue=" + objReferralCommission.pCommissionValue + ",istdsapplicable="+ objReferralCommission.pIstdsapplicable + ",tdsaccountid='" + ManageQuote(objReferralCommission.pTdsaccountid) + "',tdssection='" + ManageQuote(objReferralCommission.ptdssection) + "',tdspercentage=" + objReferralCommission.pTdspercentage + ",modifiedby =" + objReferralCommission.pCreatedby + ",modifieddate=current_timestamp where savingconfigid =" + objReferralCommission.pSavingConfigid + ";");
                    }
                    if (count==0)
                    {
                        sbinsert.Append("insert into tblmstSavingAccountConfigreferraldetails(savingconfigid ,savingaccname,isreferralcommissionapplicable,referralcommissioncalfield,referralcommissiontype ,commissionValue,istdsapplicable,tdsaccountid,tdssection,tdspercentage,statusid,createdby,createddate) values(" + objReferralCommission.pSavingConfigid + ",'" + objReferralCommission.pSavingAccname + "',"+ objReferralCommission.pIsreferralcommissionapplicable + ",'" + ManageQuote(objReferralCommission.pReferralcommissioncalfield) + "','" + objReferralCommission.pReferralcommissiontype + "'," + objReferralCommission.pCommissionValue + ","+ objReferralCommission.pIstdsapplicable + ",'"+ ManageQuote(objReferralCommission.pTdsaccountid) + "','"+ ManageQuote(objReferralCommission.ptdssection) + "',"+ objReferralCommission.pTdspercentage + "," + Convert.ToInt32(Status.Active) + "," + objReferralCommission.pCreatedby + ",current_timestamp);");
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
        public async Task<List<SavingAccountNameandCodeDTO>> GetSavingAccountConfigData(string ConnectionString)
        {
            await Task.Run(() =>
            {
                listSavingAccountNameandCodeDTO = new List<SavingAccountNameandCodeDTO>();
             
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select Savingconfigid,savingaccname,savingaccnamecode,createddate,case when statusid=" + Convert.ToInt32(Status.Active) + " then 'Active' else 'In-Active' end as status from tblmstSavingAccountConfig where statusid=" + Convert.ToInt32(Status.Active) + " order by savingaccname"))

                    {
                        while (dr.Read())
                        {


                            SavingAccountNameandCodeDTO _SavingAccountNameandCodeDTO = new SavingAccountNameandCodeDTO
                            {
                                pSavingAccountid = Convert.ToInt64(dr["Savingconfigid"]),
                                pSavingAccountname = Convert.ToString(dr["savingaccname"]),
                                pSavingaccnamecode = Convert.ToString(dr["savingaccnamecode"]),
                                pStatusid= Convert.ToString(dr["status"])

                            };
                            listSavingAccountNameandCodeDTO.Add(_SavingAccountNameandCodeDTO);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return listSavingAccountNameandCodeDTO;
        }

        public async Task<List<SavingAccountConfigDTO>> GetSavingAccountConfigMasterDetails(string ConnectionString, Int64 SavingAccountId)
        {
            await Task.Run(() =>
            {
                listsavingAccountConfig = new List<SavingAccountConfigDTO>();

                try
                {
                            SavingAccountConfigDTO _objSavingAccountConfig = new SavingAccountConfigDTO
                            {
                              SavingAccountNameandCodelist=getSavingAccountNameandCode(ConnectionString, SavingAccountId),
                                savingAccountConfiglist=getSavingAccountConfigDetails(ConnectionString, SavingAccountId),
                                getidentificationdocumentsList = Getdocumentidprofftypes(ConnectionString, SavingAccountId),
                                ReferralCommissionList=getReferralcommissionDetails(ConnectionString, SavingAccountId),
                                LoanFacilityList=getLoanFacilityDetails(ConnectionString, SavingAccountId)
                            };
                            listsavingAccountConfig.Add(_objSavingAccountConfig);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return listsavingAccountConfig;
        }

        public List<SavingAccountConfigDetailsDTO> getSavingAccountConfigDetails(string ConnectionString, Int64 SavingAccountId)
        {
            listSavingAccountConfigDetails = new List<SavingAccountConfigDetailsDTO>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ts.recordid, ts.Savingconfigid,membertypeid,membertype,applicanttype,minopenamount,minmaintainbalance,interestpayout,interestrate,iswithdrawallimitapplicable,withdrawallimitpayout,maxwithdrawallimit,ispenaltyapplicableonminbal,penaltycaltype,penaltyvalue,issavingspayinapplicable,savingspayinmode,savingmindepositamount,savingmaxdepositamount from tblmstSavingAccountConfigdetails ts join tblmstSavingAccountConfig tt on ts.Savingconfigid=tt.Savingconfigid where ts.Savingconfigid="+ SavingAccountId + " and ts.statusid=" + Convert.ToInt32(Status.Active) + ""))

                {
                    while (dr.Read())
                    {
                        SavingAccountConfigDetailsDTO objSavingAccountConfigDetails = new SavingAccountConfigDetailsDTO()
                        {
                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            pSavingConfigid = Convert.ToInt64(dr["Savingconfigid"]),
                            pMembertypeid = Convert.ToInt64(dr["membertypeid"]),
                            pMembertype = Convert.ToString(dr["membertype"]),
                            pApplicanttype = Convert.ToString(dr["applicanttype"]),
                            pMinopenamount = Convert.ToDecimal(dr["minopenamount"]),
                            pMinbalance = Convert.ToDecimal(dr["minmaintainbalance"]),
                            pInterestpayout = Convert.ToString(dr["interestpayout"]),
                            pInterestrate = Convert.ToDecimal(dr["interestrate"]),
                            pIswithdrawallimitapplicable = Convert.ToBoolean(dr["iswithdrawallimitapplicable"]),
                            pWithdrawallimitpayout = Convert.ToString(dr["withdrawallimitpayout"]),
                            pMaxwithdrawllimit = Convert.ToDecimal(dr["maxwithdrawallimit"]),
                            pIspenaltyapplicableonminbal = Convert.ToBoolean(dr["ispenaltyapplicableonminbal"]),
                            pPenaltycaltype = Convert.ToString(dr["penaltycaltype"]),
                            pPenaltyvalue = Convert.ToDecimal(dr["penaltyvalue"]),
                            pIssavingspayinapplicable = Convert.ToBoolean(dr["issavingspayinapplicable"]),
                            pSavingspayinmode = Convert.ToString(dr["savingspayinmode"]),
                            pSavingmindepositamount = Convert.ToDecimal(dr["savingmindepositamount"]),
                            pSavingmaxdepositamount = Convert.ToDecimal(dr["savingmaxdepositamount"]),
                            ptypeofoperation = Convert.ToString("UPDATE")
                            


                        };




                        listSavingAccountConfigDetails.Add(objSavingAccountConfigDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listSavingAccountConfigDetails;
        }

        public List<DocumentsMasterDTO> Getdocumentidprofftypes(string ConnectionString, Int64 SavingAccountId)
        {
            List<DocumentsMasterDTO> lstdocumentidprofftypes = new List<DocumentsMasterDTO>();
            string Query = string.Empty;

            try
            {
                using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup"))
                {
                    if (SavingAccountId > 0)
                    {

                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1 and documentid not in(select documentid from tblmstloanwisedocumentproofs where statusid=1 and  loanid=" + SavingAccountId + ") union select y.contacttype,x.documentid,x.documentgroupid,x.documentgroupname,x.documentname,y.isdocumentmandatory as mandatory,y.isdocumentrequired required from tblmstdocumentproofs x right join tblmstloanwisedocumentproofs y on x.documentid = y.documentid where y.statusid = " + Convert.ToInt32(Status.Active) + " and y.loanid = " + SavingAccountId + ";";
                    }
                    else
                    {
                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required,ts.statusname from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = " + Convert.ToInt32(Status.Active) + ";";
                    }
                    while (dr1.Read())
                    {
                        DocumentsMasterDTO objdocumentidproofs = new DocumentsMasterDTO();
                        objdocumentidproofs.pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]);
                        objdocumentidproofs.pDocumentGroup = dr1["documentgroupname"].ToString();
                        objdocumentidproofs.pDocumentsList = new List<pIdentificationDocumentsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                        {
                            if (SavingAccountId > 0)
                            {
                                while (dr.Read())
                                {

                                    if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
                                    {
                                        objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO
                                        {
                                            pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]),
                                            pDocumentGroup = dr1["documentgroupname"].ToString(),
                                            pContactType = dr["contacttype"].ToString(),
                                            pDocumentId = Convert.ToInt64(dr["documentid"]),
                                            pDocumentName = dr["documentname"].ToString(),
                                            pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                            pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                            pLoanId = SavingAccountId,
                                            // pLoantypeId = pLoanId,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                while (dr.Read())
                                {

                                    if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
                                    {
                                        objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO
                                        {
                                            pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]),
                                            pDocumentGroup = dr1["documentgroupname"].ToString(),
                                            pContactType = dr["contacttype"].ToString(),
                                            pDocumentId = Convert.ToInt64(dr["documentid"]),
                                            pDocumentName = dr["documentname"].ToString(),
                                            pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                            pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                            pStatusname = dr["statusname"].ToString(),
                                            pLoanId = SavingAccountId,
                                            // pLoantypeId = pLoanId,
                                        });
                                    }
                                }
                            }
                        }
                        lstdocumentidprofftypes.Add(objdocumentidproofs);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return lstdocumentidprofftypes;
        }
        public SavingAccountNameandCodeDTO getSavingAccountNameandCode(string ConnectionString, Int64 SavingAccountId)
        {
            SavingAccountNameandCodeDTO objSavingAccountNameandCode = new SavingAccountNameandCodeDTO();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select Savingconfigid,savingaccname,savingacccode,companycode,branchcode,series,savingaccnamecode,createddate,case when statusid=1 then true else false end as status from tblmstSavingAccountConfig where Savingconfigid=" + SavingAccountId + " and statusid=" + Convert.ToInt32(Status.Active) + ""))

                {
                    if (dr.Read())
                    {
                        objSavingAccountNameandCode = new SavingAccountNameandCodeDTO()
                        {
                            pSavingAccountid = Convert.ToInt64(dr["Savingconfigid"]),
                            pSavingAccountname = Convert.ToString(dr["savingaccname"]),
                            pSavingAccountcode = Convert.ToString(dr["savingacccode"]),
                            pCompanycode = Convert.ToString(dr["companycode"]),
                            pBranchcode = Convert.ToString(dr["branchcode"]),
                            pSeries = Convert.ToString(dr["series"]),
                            pSavingaccnamecode = Convert.ToString(dr["savingaccnamecode"]),
                            pStatusid = Convert.ToString(dr["status"])



                        };
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objSavingAccountNameandCode;
        }
        public LoanFacilityDTO getLoanFacilityDetails(string ConnectionString, Int64 SavingAccountId)
        {
            LoanFacilityDTO objLoanFacility = new LoanFacilityDTO();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tl.savingconfigid,isloanfacilityapplicable,eligiblepercentage,ageperiod,ageperiodtype from tblmstSavingAccountLoansConfig tl join tblmstSavingAccountConfig tt on tl.Savingconfigid=tt.Savingconfigid where tl.savingconfigid="+ SavingAccountId + " and tl.statusid=1"))

                {
                    if (dr.Read())
                    {
                         objLoanFacility = new LoanFacilityDTO()
                        {
                            pSavingConfigid = Convert.ToInt64(dr["Savingconfigid"]),
                            pIsloanfacilityapplicable = Convert.ToBoolean(dr["isloanfacilityapplicable"]),
                            pEligiblepercentage = Convert.ToDecimal(dr["eligiblepercentage"]),
                            pAgeperiod = Convert.ToDecimal(dr["ageperiod"]),
                            pAgeperiodtype = Convert.ToString(dr["ageperiodtype"]),
                            


                        };
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objLoanFacility;
        }

        public ReferralCommissionDTO getReferralcommissionDetails(string ConnectionString, Int64 SavingAccountId)
        {
            ReferralCommissionDTO objReferralCommission = new ReferralCommissionDTO();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ts.Savingconfigid,isreferralcommissionapplicable,referralcommissioncalfield,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection,tdspercentage from tblmstSavingAccountConfigreferraldetails ts join tblmstSavingAccountConfig tt on ts.Savingconfigid=tt.Savingconfigid where ts.Savingconfigid=" + SavingAccountId + " and ts.statusid=1"))

                {
                    if (dr.Read())
                    {
                        
                        objReferralCommission = new ReferralCommissionDTO()
                        {
                            pSavingConfigid = Convert.ToInt64(dr["Savingconfigid"]),
                            pIsreferralcommissionapplicable = dr["isreferralcommissionapplicable"] == DBNull.Value ?false: Convert.ToBoolean(dr["isreferralcommissionapplicable"]),
                            pReferralcommissioncalfield = Convert.ToString(dr["referralcommissioncalfield"]),
                            pReferralcommissiontype = Convert.ToString(dr["referralcommissiontype"]),
                            pCommissionValue = Convert.ToDecimal(dr["commissionValue"]),
                            pIstdsapplicable=Convert.ToBoolean(dr["istdsapplicable"]),
                            pTdsaccountid=Convert.ToString(dr["tdsaccountid"]),
                            ptdssection=Convert.ToString(dr["tdssection"]),
                            pTdspercentage=Convert.ToDecimal(dr["tdspercentage"])
                         


                        };
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReferralCommission;
        }


        public bool DeleteSavingAccountConfig(Int64 savingAccountid, int modifiedby, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbupdate = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sbupdate.Append("UPDATE tblmstSavingAccountConfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingconfigid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tblmstSavingAccountConfigdetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingconfigid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tblmstSavingAccountLoansConfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingconfigid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tblmstSavingAccountConfigreferraldetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingconfigid=" + savingAccountid + "; ");
                
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString());
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

    }
}
