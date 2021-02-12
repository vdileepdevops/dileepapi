using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Masters;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class InsurenceConfigDAL : SettingsDAL, IInsurenceConfig
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region Get Insurence Name Count
        public int GetInsurenceNameCount(string InsurenceName, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(InsuranceSchemeName)='" + ManageQuote(InsurenceName.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public InsuranceschemeandcodeCount GetInsuranceNameCount(Int64 Insuranceid, string InsuranceName, string InsuranceCode, string ConnectionString)
        {
            InsuranceschemeandcodeCount _InsuranceschemeandcodeCount = new InsuranceschemeandcodeCount();
            try
            {
                if (Insuranceid == 0)
                {
                    _InsuranceschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(InsuranceSchemeName)='" + ManageQuote(InsuranceName).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _InsuranceschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(insuranceschemecode)='" + ManageQuote(InsuranceCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _InsuranceschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(InsuranceSchemeName)='" + ManageQuote(InsuranceName).ToUpper() + "' and insuranceschemeconfigid <> " + Insuranceid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _InsuranceschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(insuranceschemecode)='" + ManageQuote(InsuranceCode).ToUpper() + "' and insuranceschemeconfigid <> " + Insuranceid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _InsuranceschemeandcodeCount;
        }

        public int GetInsurenceCodeCount(string InsurenceCode, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstinsuranceconfig where upper(insuranceschemecode)='" + ManageQuote(InsurenceCode.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        #endregion

        #region Save Insurence Configaration
        public string SaveInsurenceNameAndCode(InsurenceNameAndCodeDTO InsurenceNameAndCode, string ConnectionString, out long insurenceconfigid)
        {
            StringBuilder Sbinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(InsurenceNameAndCode.ptypeofoperation) && InsurenceNameAndCode.ptypeofoperation.Trim().ToUpper() == "CREATE")
                {
                    InsurenceNameAndCode.pInsurenceconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstinsuranceconfig (InsuranceSchemeName,InsuranceSchemecode,companycode,branchcode,series,serieslength,InsuranceSchemenamecode,statusid,createdby,createddate)values('" + ManageQuote(InsurenceNameAndCode.pInsurencename) + "','" + ManageQuote(InsurenceNameAndCode.pInsurencecode) + "','" + ManageQuote(InsurenceNameAndCode.pCompanycode) + "','" + ManageQuote(InsurenceNameAndCode.pBranchcode) + "','" + ManageQuote(InsurenceNameAndCode.pSeries) + "'," + InsurenceNameAndCode.pSerieslength + ",'" + ManageQuote(InsurenceNameAndCode.pInsurencenamecode) + "'," + Convert.ToInt32(Status.Active) + "," + InsurenceNameAndCode.pCreatedby + ",current_timestamp) returning InsuranceSchemeconfigid;"));
                }
                else if (!string.IsNullOrEmpty(InsurenceNameAndCode.ptypeofoperation) && InsurenceNameAndCode.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                {
                    Sbinsert.Append("update tblmstinsuranceconfig set InsuranceSchemeName='" + ManageQuote(InsurenceNameAndCode.pInsurencename) + "',InsuranceSchemecode='" + ManageQuote(InsurenceNameAndCode.pInsurencecode) + "',companycode='" + ManageQuote(InsurenceNameAndCode.pCompanycode) + "',branchcode='" + ManageQuote(InsurenceNameAndCode.pBranchcode) + "',series='" + ManageQuote(InsurenceNameAndCode.pSeries) + "',serieslength=" + InsurenceNameAndCode.pSerieslength + ",InsuranceSchemenamecode='" + ManageQuote(InsurenceNameAndCode.pInsurencenamecode) + "',statusid=" + Convert.ToInt32(Status.Active) + ",modifiedby=" + InsurenceNameAndCode.pCreatedby + ",modifieddate=current_timestamp where InsuranceSchemeconfigid=" + InsurenceNameAndCode.pInsurenceconfigid + ";");
                }
                if (!string.IsNullOrEmpty(Sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Sbinsert.ToString());
                }
                trans.Commit();
                insurenceconfigid = InsurenceNameAndCode.pInsurenceconfigid;
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
            return InsurenceNameAndCode.pInsurencename;

        }
        public bool SaveInsurenceConfigDetails(InsurenceConfigDTO InsurenceConfigDetails, string ConnectionString)
        {
            StringBuilder Sbinsert = new StringBuilder();
            StringBuilder QueryUpdate = new StringBuilder();
            bool IsSaved = false;
            string Recordid = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (InsurenceConfigDetails.lstInsurenceConfigartionDetails != null)
                {
                    for (int i = 0; i < InsurenceConfigDetails.lstInsurenceConfigartionDetails.Count; i++)
                    {
                        if (InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].precordid.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].precordid.ToString();
                            }
                        }
                        if (InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                        {
                            Sbinsert.Append("insert into tblmstinsuranceconfigdetails(insuranceschemeconfigid,insuranceschemename,membertypeid,membertype,applicanttype,agefrom,ageto,insuranceduration,premiumamountpayable,premiumpayin,insuranceclaimpayoutevent,insuranceclaimamount,premiumrefund,statusid,createdby,createddate)VALUES(" + InsurenceConfigDetails.pInsurenceconfigid + ",'" + ManageQuote(InsurenceConfigDetails.pInsurencename) + "'," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pMembertype) + "','" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pApplicanttype) + "'," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pAgefrom + "," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pAgeto + ",'" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceduration) + "'," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumamountpayable + ",'" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumpayin) + "','" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceclaimpayoutevent) + "'," + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceclaimamount + ","+ InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumrefund + "," + Convert.ToInt32(Status.Active) + "," + InsurenceConfigDetails.pCreatedby + ",current_timestamp);");
                        }
                        else if (InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                        {
                            Sbinsert.Append("update tblmstinsuranceconfigdetails set membertypeid=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pMembertypeid + ",membertype='" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pMembertype) + "',applicanttype='" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pApplicanttype) + "',agefrom=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pAgefrom + ",ageto=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pAgeto + ",insuranceduration='" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceduration) + "',premiumamountpayable=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumamountpayable + ",premiumpayin='" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumpayin) + "',insuranceclaimpayoutevent='" + ManageQuote(InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceclaimpayoutevent) + "',insuranceclaimamount=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pInsuranceclaimamount + ",premiumrefund=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].pPremiumrefund + ",modifiedby=" + InsurenceConfigDetails.pCreatedby + ",modifieddate=current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " where recordid=" + InsurenceConfigDetails.lstInsurenceConfigartionDetails[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    QueryUpdate.Append("update tblmstinsuranceconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + InsurenceConfigDetails.pCreatedby + ",modifieddate=current_timestamp where insuranceschemeconfigid=" + InsurenceConfigDetails.pInsurenceconfigid + "  and recordid not in(" + Recordid + ");");
                }
                else
                {
                    if (InsurenceConfigDetails.lstInsurenceConfigartionDetails.Count == 0)
                    {
                        QueryUpdate.Append("update tblmstinsuranceconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + InsurenceConfigDetails.pCreatedby + ",modifieddate=current_timestamp where insuranceschemeconfigid=" + InsurenceConfigDetails.pInsurenceconfigid + ";");
                    }
                }

                if (!string.IsNullOrEmpty(Sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, QueryUpdate.ToString() + "" + Sbinsert.ToString());
                }
                trans.Commit();
                IsSaved = true;
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
            return IsSaved;
        }
        public bool SaveInsurenceReferralDetails(insurenceReferralCommissionDTO InsurenceReferralDetails, string ConnectionString)
        {
            StringBuilder Sbinsert = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(InsurenceReferralDetails.ptypeofoperation) && InsurenceReferralDetails.ptypeofoperation.Trim().ToUpper() == "CREATE")
                {
                    Sbinsert.Append("insert into tblmstinsuranceconfigreferraldetails(insuranceschemeconfigid,insuranceSchemename,isreferralcommissionapplicable,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection,tdspercentage,statusid,createdby,createddate)values(" + InsurenceReferralDetails.pInsurenceconfigid + ",'" + ManageQuote(InsurenceReferralDetails.pInsurencename) + "'," + InsurenceReferralDetails.pIsreferralcommissionapplicable + ",'" + ManageQuote(InsurenceReferralDetails.pReferralcommissiontype) + "'," + InsurenceReferralDetails.pCommissionValue + "," + InsurenceReferralDetails.pIstdsapplicable + ",'" + ManageQuote(InsurenceReferralDetails.pTdsaccountid) + "','" + ManageQuote(InsurenceReferralDetails.pTdssection) + "'," + InsurenceReferralDetails.pTdspercentage + "," + Convert.ToInt32(Status.Active) + "," + InsurenceReferralDetails.pCreatedby + ",current_timestamp);");

                }
                else if (!string.IsNullOrEmpty(InsurenceReferralDetails.ptypeofoperation) && InsurenceReferralDetails.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                {

                }
                if (!string.IsNullOrEmpty(Sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Sbinsert.ToString());
                }
                trans.Commit();
                IsSaved = true;
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
            return IsSaved;
        }
        #endregion

        #region Get Insurence View Details
        public List<InsurenceConfigViewDTO> GetInsurenceViewDetails(string ConnectionString)
        {
            List<InsurenceConfigViewDTO> lstInsurenceView = new List<InsurenceConfigViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select insuranceschemeconfigid,insuranceschemename,insuranceschemecode,companycode,branchcode,series,serieslength,insuranceschemenamecode,statusid from tblmstinsuranceconfig where statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        InsurenceConfigViewDTO objlstInsurenceView = new InsurenceConfigViewDTO();
                        objlstInsurenceView.pInsurenceconfigid = Convert.ToInt64(dr["insuranceschemeconfigid"]);
                        objlstInsurenceView.pInsurencename = dr["insuranceschemename"].ToString();
                        objlstInsurenceView.pInsurencenamecode = dr["insuranceschemenamecode"].ToString();
                        if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                        {
                            objlstInsurenceView.pstatus = true;
                        }
                        if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                        {
                            objlstInsurenceView.pstatus = false;
                        }
                        lstInsurenceView.Add(objlstInsurenceView);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInsurenceView;
        }
        #endregion

        #region Get Insurence Name And Code Details
        public InsurenceNameAndCodeDTO GetInsurenceNameAndCodeDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString)
        {
            InsurenceNameAndCodeDTO objInsurenceNameAndCode = new InsurenceNameAndCodeDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select insuranceschemeconfigid,insuranceschemename,insuranceschemecode,companycode,branchcode,series,serieslength,insuranceschemenamecode,statusid from tblmstinsuranceconfig where upper(insuranceschemename) = '" + ManageQuote(InsurenceName.ToUpper()) + "' and upper(insuranceschemenamecode) ='" + ManageQuote(InsurenceNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        objInsurenceNameAndCode.pInsurenceconfigid = Convert.ToInt64(dr["insuranceschemeconfigid"]);
                        objInsurenceNameAndCode.pInsurencename = dr["insuranceschemename"].ToString();
                        objInsurenceNameAndCode.pInsurencecode = dr["insuranceschemecode"].ToString();
                        objInsurenceNameAndCode.pCompanycode = dr["companycode"].ToString();
                        objInsurenceNameAndCode.pBranchcode = dr["branchcode"].ToString();
                        objInsurenceNameAndCode.pSeries = dr["series"].ToString();
                        objInsurenceNameAndCode.pSerieslength = Convert.ToInt64(dr["serieslength"]);
                        objInsurenceNameAndCode.pInsurencenamecode = dr["insuranceschemenamecode"].ToString();
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return objInsurenceNameAndCode;
        }
        #endregion

        #region Get Insurence Configuration Details
        public InsurenceConfigDTO GetInsurenceConfigurationDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString)
        {
            InsurenceConfigDTO InsurenceConfigDetails = new InsurenceConfigDTO();
            InsurenceConfigDetails.lstInsurenceConfigartionDetails = new List<InsurenceConfigartionDetailsDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select insuranceschemeconfigid,insuranceschemename,insuranceschemecode,companycode,branchcode,series,serieslength,insuranceschemenamecode,statusid from tblmstinsuranceconfig where upper(insuranceschemename) = '" + ManageQuote(InsurenceName.ToUpper()) + "' and upper(insuranceschemenamecode) ='" + ManageQuote(InsurenceNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        InsurenceConfigDetails.pInsurenceconfigid = Convert.ToInt64(dr["insuranceschemeconfigid"]);
                        InsurenceConfigDetails.pInsurencename = dr["insuranceschemename"].ToString();
                        InsurenceConfigDetails.pInsurencenamecode = dr["insuranceschemenamecode"].ToString();
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,insuranceschemeconfigid,insuranceschemename,membertypeid,membertype,applicanttype,agefrom,ageto,insuranceduration,premiumamountpayable,premiumpayin,insuranceclaimpayoutevent,insuranceclaimamount,premiumrefund from tblmstinsuranceconfigdetails where insuranceschemeconfigid=" + InsurenceConfigDetails.pInsurenceconfigid + " and upper(insuranceschemename)='" + ManageQuote(InsurenceName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {

                        InsurenceConfigDetails.lstInsurenceConfigartionDetails.Add(new InsurenceConfigartionDetailsDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),                        
                            pMembertypeid = Convert.ToInt64(dr["membertypeid"]),
                            pMembertype = dr["membertype"].ToString(),
                            pApplicanttype= dr["applicanttype"].ToString(),
                            pAgefrom = Convert.ToInt64(dr["agefrom"]),
                            pAgeto = Convert.ToInt64(dr["ageto"]),
                            pInsuranceduration = dr["insuranceduration"].ToString(),
                            pPremiumamountpayable = Convert.ToDecimal(dr["premiumamountpayable"]),
                            pPremiumpayin = dr["premiumpayin"].ToString(),
                            pInsuranceclaimpayoutevent = dr["insuranceclaimpayoutevent"].ToString(),
                            pInsuranceclaimamount = Convert.ToDecimal(dr["insuranceclaimamount"]),
                            pPremiumrefund = Convert.ToBoolean(dr["premiumrefund"]),
                            pTypeofOperation = "OLD"
                        });
                    }
                }
            }



            catch (Exception ex)
            {

                throw ex;
            }
            return InsurenceConfigDetails;
        }
        #endregion

        #region Get Insurence Referral Details
        public insurenceReferralCommissionDTO GetInsurenceReferralDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString)
        {
            insurenceReferralCommissionDTO InsurenceReferralDetails = new insurenceReferralCommissionDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select insuranceschemeconfigid,insuranceschemename,insuranceschemecode,companycode,branchcode,series,serieslength,insuranceschemenamecode,statusid from tblmstinsuranceconfig where upper(insuranceschemename) = '" + ManageQuote(InsurenceName.ToUpper()) + "' and upper(insuranceschemenamecode) ='" + ManageQuote(InsurenceNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        InsurenceReferralDetails.pInsurenceconfigid = Convert.ToInt64(dr["insuranceschemeconfigid"]);
                        InsurenceReferralDetails.pInsurencename = dr["insuranceschemename"].ToString();
                        InsurenceReferralDetails.pInsurencenamecode = dr["insuranceschemenamecode"].ToString();
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,insuranceschemeconfigid,insuranceschemename,isreferralcommissionapplicable,referralcommissiontype,commissionvalue,istdsapplicable,tdsaccountid,tdssection,tdspercentage from tblmstinsuranceconfigreferraldetails where insuranceschemeconfigid=" + InsurenceReferralDetails.pInsurenceconfigid + " and upper(insuranceschemename)='" + ManageQuote(InsurenceName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        InsurenceReferralDetails.precordid = Convert.ToInt64(dr["recordid"]);
                        InsurenceReferralDetails.pIsreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]);
                        InsurenceReferralDetails.pReferralcommissiontype = dr["referralcommissiontype"].ToString();
                        InsurenceReferralDetails.pCommissionValue = Convert.ToDecimal(dr["commissionvalue"]);
                        InsurenceReferralDetails.pIstdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                        InsurenceReferralDetails.pTdsaccountid = dr["tdsaccountid"].ToString();
                        InsurenceReferralDetails.pTdssection = dr["tdssection"].ToString();
                        InsurenceReferralDetails.pTdspercentage = Convert.ToDecimal(dr["tdspercentage"]);
                        InsurenceReferralDetails.pTypeofOperation = "OLD";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return InsurenceReferralDetails;
        }
        #endregion

        #region Delete Insurence Configuration 
        public bool DeleteInsurenceConfiguration(long InsurenceConfigId, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(InsurenceConfigId.ToString()) && InsurenceConfigId != 0)
                {
                    sbInsert.Append("update tblmstinsuranceconfig set statusid=" + Convert.ToInt32(Status.Inactive) + " where insuranceschemeconfigid=" + InsurenceConfigId + ";");
                    sbInsert.Append("update tblmstinsuranceconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where insuranceschemeconfigid=" + InsurenceConfigId + ";");
                    sbInsert.Append("update tblmstinsuranceconfigreferraldetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where insuranceschemeconfigid=" + InsurenceConfigId + ";");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }

                trans.Commit();
                IsSaved = true;
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
            return IsSaved;
        }
        #endregion
    }
}
