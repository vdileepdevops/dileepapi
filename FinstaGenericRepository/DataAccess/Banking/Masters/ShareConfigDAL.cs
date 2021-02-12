using FinstaInfrastructure.Settings.Users;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings.Users;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class ShareConfigDAL : SettingsDAL, IShareConfig
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;       
        public async Task<List<ShareviewDTO>> GetShareview(string ConnectionString)
        {
            List<ShareviewDTO> lstShareviewDTO = new List<ShareviewDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select shareconfigid,sharename,sharecode,sharenamecode,statusid from tblmstshareconfig where statusid = " + Convert.ToInt32(Status.Active) + "  order by sharename;"))
                    {
                        while (dr.Read())
                        {
                            ShareviewDTO ShareviewDTO = new ShareviewDTO();
                            ShareviewDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareviewDTO.psharename = Convert.ToString(dr["sharename"]);
                            ShareviewDTO.psharecode = Convert.ToString(dr["sharecode"]);
                            ShareviewDTO.psharenamecode = Convert.ToString(dr["sharenamecode"]);
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                ShareviewDTO.pstatus = "Active";
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                ShareviewDTO.pstatus = "In-Active";
                            }
                            lstShareviewDTO.Add(ShareviewDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstShareviewDTO;
        }
     
        public bool DeleteShareConfiguration(long ShareconfigID, string ShareName, string ConnectionString)
        {
            bool Issaved = false;
            StringBuilder SbDelete = new StringBuilder();

            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                SbDelete.Append("update tblmstshareconfig set statusid="+Convert.ToInt32(Status.Inactive)+" where shareconfigid="+ ShareconfigID + " and upper(sharename)='"+ManageQuote(ShareName.ToUpper())+ "';");
                SbDelete.Append("update tblmstshareconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where shareconfigid=" + ShareconfigID + " and upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "';");
                SbDelete.Append("update tblmstshareconfigreferraldetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where shareconfigid=" + ShareconfigID + " and upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "';");
                if (Convert.ToString(SbDelete) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbDelete.ToString());
                }
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

        public ShareschemeandcodeCount GetShareNameCodeCount(Int64 shareid, string ShareName, string ShareCode, string ConnectionString)
        {
            ShareschemeandcodeCount _ShareschemeandcodeCount = new ShareschemeandcodeCount();
            try
            {
                if (shareid == 0)
                {
                    _ShareschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharename)='" + ManageQuote(ShareName).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _ShareschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _ShareschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharename)='" + ManageQuote(ShareName).ToUpper() + "' and shareconfigid <> " + shareid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _ShareschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode).ToUpper() + "' and shareconfigid <> " + shareid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _ShareschemeandcodeCount;
        }
        public int GetShareNameCount(string ShareName, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
            return count;
        }

        public int GetShareCodeCount(string ShareCode, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
            return count;
        }

        public bool SaveShareNameANdcode(ShareConfigDTO ShareConfigDTO, string ConnectionString)
        {
            bool Issaved = false;
         
            StringBuilder SbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (ShareConfigDTO != null)
                {
                    if (string.IsNullOrEmpty(ShareConfigDTO.psharename))
                    {
                        ShareConfigDTO.psharename = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.psharecode))
                    {
                        ShareConfigDTO.psharecode = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.pcompanycode))
                    {
                        ShareConfigDTO.pcompanycode = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.pbranchcode))
                    {
                        ShareConfigDTO.pbranchcode = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.pseries))
                    {
                        ShareConfigDTO.pseries = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.psharenamecode))
                    {
                        ShareConfigDTO.psharenamecode = "";
                    }
                    if (string.IsNullOrEmpty(ShareConfigDTO.pshareconfigid.ToString()) || ShareConfigDTO.pshareconfigid == 0)
                    {
                        SbInsert.Append("INSERT INTO tblmstshareconfig(sharename, sharecode, companycode, branchcode,series, serieslength, sharenamecode, statusid, createdby, createddate) VALUES ('" + ManageQuote(ShareConfigDTO.psharename) + "','" + ManageQuote(ShareConfigDTO.psharecode) + "','" + ManageQuote(ShareConfigDTO.pcompanycode) + "','" + ManageQuote(ShareConfigDTO.pbranchcode) + "','" + ManageQuote(ShareConfigDTO.pseries) + "'," + ShareConfigDTO.pserieslength + ",'" + ManageQuote(ShareConfigDTO.psharenamecode) + "'," + Convert.ToInt32(Status.Active) + "," + ShareConfigDTO.pCreatedby + ",current_timestamp);");
                       
                    }
                    else
                    {
                        SbInsert.Append("update tblmstshareconfig set sharename='" + ManageQuote(ShareConfigDTO.psharename) + "', sharecode='" + ManageQuote(ShareConfigDTO.psharecode) + "', companycode='" + ManageQuote(ShareConfigDTO.pcompanycode) + "', branchcode='" + ManageQuote(ShareConfigDTO.pbranchcode) + "',series='" + ManageQuote(ShareConfigDTO.pseries) + "', serieslength=" + ShareConfigDTO.pserieslength + ", sharenamecode='" + ManageQuote(ShareConfigDTO.psharenamecode) + "' where shareconfigid=" + ShareConfigDTO.pshareconfigid + ";");
                    }
                }
                if (Convert.ToString(SbInsert) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbInsert.ToString());
                }
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

        public async Task<ShareConfigDTO> GetShareNameANdcode(string ShareName, string ShareCode, string ConnectionString)
        {
            ShareConfigDTO ShareConfigDTO = new ShareConfigDTO();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT shareconfigid, sharename, sharecode, companycode, branchcode, series, serieslength, sharenamecode FROM tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode.ToUpper()) + "' and upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareConfigDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareConfigDTO.psharename = Convert.ToString(dr["sharename"]);
                            ShareConfigDTO.psharecode = Convert.ToString(dr["sharecode"]);
                            ShareConfigDTO.pcompanycode = Convert.ToString(dr["companycode"]);
                            ShareConfigDTO.pbranchcode = Convert.ToString(dr["branchcode"]);
                            ShareConfigDTO.pseries = Convert.ToString(dr["series"]);
                            ShareConfigDTO.pserieslength = Convert.ToInt64(dr["serieslength"]);
                            ShareConfigDTO.psharenamecode = Convert.ToString(dr["sharenamecode"]);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return ShareConfigDTO;
        }

        public bool SaveShareConfigDetails(ShareConfigDetails ShareConfigDetails, string ConnectionString)
        {
            bool Issaved = false;
            string Recordid = string.Empty;
            string query = "";
            StringBuilder SbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();


                    if (ShareConfigDetails.lstShareconfigDetailsDTO.Count > 0)
                    {
                        for (int i = 0; i < ShareConfigDetails.lstShareconfigDetailsDTO.Count; i++)
                        {
                            if (ShareConfigDetails.lstShareconfigDetailsDTO[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = ShareConfigDetails.lstShareconfigDetailsDTO[i].precordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].precordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertype))
                            {
                            ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertype = "";
                            }
                            if (string.IsNullOrEmpty(ShareConfigDetails.lstShareconfigDetailsDTO[i].papplicanttype))
                            {
                            ShareConfigDetails.lstShareconfigDetailsDTO[i].papplicanttype = "";
                            }
                            if (string.IsNullOrEmpty(ShareConfigDetails.lstShareconfigDetailsDTO[i].pdivedendpayout))
                            {
                            ShareConfigDetails.lstShareconfigDetailsDTO[i].pdivedendpayout = "";
                            }
                            if (ShareConfigDetails.lstShareconfigDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                            {
                                SbInsert.Append("INSERT INTO tblmstshareconfigdetails(shareconfigid, sharename, membertypeid, membertype,applicanttype, facevalue, minshares, maxshares, ismultipleshares,multipleshares, isdivedend, divedendpayout, sharetransfer,statusid,createdby, createddate) VALUES (" + ShareConfigDetails.pshareconfigid + ",'" + ManageQuote(ShareConfigDetails.psharename) + "'," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertypeid + ",'" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertype) + "','" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].papplicanttype) + "'," + Convert.ToDecimal(ShareConfigDetails.lstShareconfigDetailsDTO[i].pfacevalue) + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pminshares + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmaxshares + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pismultipleshares + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmultipleshares + "," + ShareConfigDetails.lstShareconfigDetailsDTO[i].pisdivedend + ",'" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].pdivedendpayout) + "'," + ShareConfigDetails.lstShareconfigDetailsDTO[i].psharetransfer + "," + Convert.ToInt32(Status.Active) + "," + ShareConfigDetails.pCreatedby + ",current_timestamp);");
                            }
                            if (ShareConfigDetails.lstShareconfigDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                            {
                                SbInsert.Append("UPDATE tblmstshareconfigdetails SET shareconfigid=" + ShareConfigDetails.pshareconfigid + ", sharename='" + ManageQuote(ShareConfigDetails.psharename) + "', membertypeid=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertypeid + ", membertype='" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].pmembertype) + "',applicanttype='" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].papplicanttype) + "', facevalue=" + Convert.ToDecimal(ShareConfigDetails.lstShareconfigDetailsDTO[i].pfacevalue) + ", minshares=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pminshares + ", maxshares=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmaxshares + ", ismultipleshares=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pismultipleshares + ",multipleshares=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pmultipleshares + ", isdivedend=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].pisdivedend + ", divedendpayout='" + ManageQuote(ShareConfigDetails.lstShareconfigDetailsDTO[i].pdivedendpayout) + "', sharetransfer=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].psharetransfer + " WHERE RECORDID=" + ShareConfigDetails.lstShareconfigDetailsDTO[i].precordid + ";");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        query = "update tblmstshareconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + ShareConfigDetails.pCreatedby + ",modifieddate=current_timestamp where shareconfigid=" + ShareConfigDetails.pshareconfigid + "  and recordid not in(" + Recordid + ");";
                    }
                    else
                    {
                        //if (ShareConfigDetails.lstShareconfigDetailsDTO.Count == 0)
                        //{
                            query = "update tblmstshareconfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + ShareConfigDetails.pCreatedby + ",modifieddate=current_timestamp where shareconfigid=" + ShareConfigDetails.pshareconfigid + ";";
                        //}
                    }

                if (!string.IsNullOrEmpty(SbInsert.ToString()) || !string.IsNullOrEmpty(query))
                {                  
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query + "" + SbInsert.ToString());
                }
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

        public async Task<ShareConfigDetails> GetShareConfigDetails(string ShareName, string ShareCode, string ConnectionString)
        {
            ShareConfigDetails ShareConfigDetails = new ShareConfigDetails();
            ShareConfigDetails.lstShareconfigDetailsDTO = new List<ShareconfigDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select shareconfigid,sharename,sharecode,statusid from tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode.ToUpper()) + "' and upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareConfigDetails.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareConfigDetails.psharename = Convert.ToString(dr["sharename"]);
                            ShareConfigDetails.psharecode = Convert.ToString(dr["sharecode"]);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, shareconfigid, sharename, membertypeid, membertype,applicanttype, facevalue, minshares, maxshares, ismultipleshares,multipleshares, isdivedend, divedendpayout, sharetransfer FROM tblmstshareconfigdetails where shareconfigid=" + ShareConfigDetails.pshareconfigid + " and upper(sharename)='" + ManageQuote(ShareConfigDetails.psharename.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareConfigDetails.lstShareconfigDetailsDTO.Add(new ShareconfigDetailsDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pshareconfigid = Convert.ToInt64(dr["shareconfigid"]),
                                psharename = Convert.ToString(dr["sharename"]),
                                pmembertypeid = Convert.ToInt64(dr["membertypeid"]),
                                pmembertype = Convert.ToString(dr["membertype"]),
                                papplicanttype = Convert.ToString(dr["applicanttype"]),
                                pfacevalue = Convert.ToDecimal(dr["facevalue"]),
                                pminshares = Convert.ToInt64(dr["minshares"]),
                                pmaxshares = Convert.ToInt64(dr["maxshares"]),
                                pismultipleshares = Convert.ToBoolean(dr["ismultipleshares"]),
                                pmultipleshares = Convert.ToInt64(dr["multipleshares"]),
                                pisdivedend = Convert.ToBoolean(dr["isdivedend"]),
                                pdivedendpayout = Convert.ToString(dr["divedendpayout"]),
                                psharetransfer = Convert.ToBoolean(dr["sharetransfer"]),
                                pTypeofOperation = "OLD"

                            });
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return ShareConfigDetails;
        }

        public bool SaveShareConfigReferral(ShareconfigReferralDTO ShareconfigReferralDTO, string ConnectionString)
        {
            bool Issaved = false;

            StringBuilder SbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();


                    if (ShareconfigReferralDTO != null)
                    {
                        if (string.IsNullOrEmpty(ShareconfigReferralDTO.preferralcommissiontype))
                        {
                        ShareconfigReferralDTO.preferralcommissiontype = "";
                        }
                        if (string.IsNullOrEmpty(ShareconfigReferralDTO.ptdsaccountid))
                        {
                        ShareconfigReferralDTO.ptdsaccountid = "";
                        }
                        if (string.IsNullOrEmpty(ShareconfigReferralDTO.ptdssection))
                        {
                        ShareconfigReferralDTO.ptdssection = "";
                        }
                        if (string.IsNullOrEmpty(ShareconfigReferralDTO.precordid.ToString()) || ShareconfigReferralDTO.precordid == 0)
                        {
                            SbInsert.Append("INSERT INTO tblmstshareconfigreferraldetails(shareconfigid, sharename, referralcommissiontype, commissionvalue, istdsapplicable, tdsaccountid, tdssection, tdspercentage, statusid,createdby, createddate,isreferralcommissionapplicable) VALUES (" + ShareconfigReferralDTO.pshareconfigid + ",'" + ManageQuote(ShareconfigReferralDTO.psharename) + "','" + ManageQuote(ShareconfigReferralDTO.preferralcommissiontype) + "'," + Convert.ToDecimal(ShareconfigReferralDTO.pcommissionValue) + "," + ShareconfigReferralDTO.pistdsapplicable + ",'" + ManageQuote(ShareconfigReferralDTO.ptdsaccountid) + "','" + ManageQuote(ShareconfigReferralDTO.ptdssection) + "'," + ShareconfigReferralDTO.ptdspercentage + "," + Convert.ToInt32(Status.Active) + "," + ShareconfigReferralDTO.pCreatedby + ",current_timestamp," + ShareconfigReferralDTO.pisreferralcommissionapplicable + ");");
                        }
                        else
                        {
                            SbInsert.Append("update tblmstshareconfigreferraldetails set shareconfigid=" + ShareconfigReferralDTO.pshareconfigid + ", sharename='" + ManageQuote(ShareconfigReferralDTO.psharename) + "',isreferralcommissionapplicable="+ShareconfigReferralDTO.pisreferralcommissionapplicable+ ", referralcommissiontype='" + ManageQuote(ShareconfigReferralDTO.preferralcommissiontype) + "', commissionvalue=" + Convert.ToDecimal(ShareconfigReferralDTO.pcommissionValue) + ", istdsapplicable=" + ShareconfigReferralDTO.pistdsapplicable + ", tdsaccountid='" + ManageQuote(ShareconfigReferralDTO.ptdsaccountid) + "', tdssection='" + ManageQuote(ShareconfigReferralDTO.ptdssection) + "', tdspercentage=" + ShareconfigReferralDTO.ptdspercentage + " where recordid=" + ShareconfigReferralDTO.precordid + ";");
                        }
                    }
            
                if (Convert.ToString(SbInsert) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text,SbInsert.ToString());
                }
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
        public async Task<ShareconfigReferralDTO> GetShareConfigReferralDetails(string ShareName, string ShareCode, string ConnectionString)
        {
            ShareconfigReferralDTO ShareconfigReferralDTO = new ShareconfigReferralDTO();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select shareconfigid,sharename,sharecode,statusid from tblmstshareconfig where upper(sharecode)='" + ManageQuote(ShareCode.ToUpper()) + "' and upper(sharename)='" + ManageQuote(ShareName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareconfigReferralDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareconfigReferralDTO.psharename = Convert.ToString(dr["sharename"]);
                            ShareconfigReferralDTO.psharecode = Convert.ToString(dr["sharecode"]);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, shareconfigid, sharename, referralcommissiontype, commissionvalue,istdsapplicable, tdsaccountid, tdssection,tdspercentage,isreferralcommissionapplicable FROM tblmstshareconfigreferraldetails where shareconfigid=" + ShareconfigReferralDTO.pshareconfigid + " and upper(sharename)='" + ManageQuote(ShareconfigReferralDTO.psharename.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareconfigReferralDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            ShareconfigReferralDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareconfigReferralDTO.psharename = Convert.ToString(dr["sharename"]);
                            ShareconfigReferralDTO.preferralcommissiontype = Convert.ToString(dr["referralcommissiontype"]);
                            ShareconfigReferralDTO.pcommissionValue = Convert.ToDecimal(dr["commissionvalue"]);
                            ShareconfigReferralDTO.pistdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                            ShareconfigReferralDTO.ptdsaccountid = Convert.ToString(dr["tdsaccountid"]);
                            ShareconfigReferralDTO.ptdssection = Convert.ToString(dr["tdssection"]);
                            ShareconfigReferralDTO.ptdspercentage = Convert.ToInt64(dr["tdspercentage"]);
                            ShareconfigReferralDTO.pisreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]);

                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return ShareconfigReferralDTO;
        }
    }
}
