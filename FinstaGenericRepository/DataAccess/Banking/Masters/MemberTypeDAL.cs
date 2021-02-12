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
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class MemberTypeDAL : SettingsDAL, IMemberType
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        List<MemberTypeDTO> lstMemberTypeDTO { set; get; }

        public async Task<List<MemberTypeDTO>> BindMemberType(string ConnectionString)
        {
            lstMemberTypeDTO = new List<MemberTypeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype,membertypecode,companycode,branchcode,series,serieslength,membercode,coalesce(issharesissueapplicable,false) as issharesissueapplicable,statusid,coalesce(isaccounttypecreationapplicable,false) as isaccounttypecreationapplicable ,coalesce(ismembershipfeeapplicable,false) as ismembershipfeeapplicable from tblmstmembertypes where statusid = " + Convert.ToInt32(Status.Active) + "  order by membertype;"))
                    {
                        while (dr.Read())
                        {
                            MemberTypeDTO MemberTypeDTO = new MemberTypeDTO();
                            MemberTypeDTO.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            MemberTypeDTO.pmembertype = Convert.ToString(dr["membertype"]);
                            MemberTypeDTO.pmembertypecode = Convert.ToString(dr["membertypecode"]);
                            MemberTypeDTO.pcompanycode = Convert.ToString(dr["companycode"]);
                            MemberTypeDTO.pbranchcode = Convert.ToString(dr["branchcode"]);
                            MemberTypeDTO.pseries = Convert.ToString(dr["series"]);
                            MemberTypeDTO.pserieslength = Convert.ToInt64(dr["serieslength"]);
                            MemberTypeDTO.pmembercode = Convert.ToString(dr["membercode"]);
                            MemberTypeDTO.pissharesissueapplicable = Convert.ToBoolean(dr["issharesissueapplicable"]);
                            MemberTypeDTO.pisaccounttypecreationapplicable = Convert.ToBoolean(dr["isaccounttypecreationapplicable"]);
                            MemberTypeDTO.pismembershipfeeapplicable = Convert.ToBoolean(dr["ismembershipfeeapplicable"]);

                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                MemberTypeDTO.pstatus = true;
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                MemberTypeDTO.pstatus = false;
                            }
                            lstMemberTypeDTO.Add(MemberTypeDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstMemberTypeDTO;
        }

        public bool DeleteMemberType(int MemberID, string ConnectionString)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tblmstmembertypes set statusid= (select case when statusid=" + Convert.ToInt32(Status.Active) + " then " + Convert.ToInt32(Status.Inactive) + " when statusid=" + Convert.ToInt32(Status.Inactive) + " then " + Convert.ToInt32(Status.Active) + " end from tblmstmembertypes where membertypeid=" + MemberID + ") where membertypeid=" + MemberID + ";");
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

        public MemberschemeandcodeCount GetMemberNameCount(Int64 memberid, string MemberType, string MemberTypeCode, string ConnectionString)
        {
            MemberschemeandcodeCount _memberschemeandcodeCount = new MemberschemeandcodeCount();
            try
            {
                if(memberid == 0)
                {
                    _memberschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertype)='" + ManageQuote(MemberType).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _memberschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertypecode)='" + ManageQuote(MemberTypeCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _memberschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertype)='" + ManageQuote(MemberType).ToUpper() + "' and membertypeid <> "+memberid+" and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _memberschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertypecode)='" + ManageQuote(MemberTypeCode).ToUpper() + "' and membertypeid <> " + memberid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _memberschemeandcodeCount;
        }

        public int GetMemberTypeCount(string MemberType, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertype)='" + ManageQuote(MemberType.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
            return count;
        }

        public int GetMembercodeCount(string MemberTypeCode, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembertypes where upper(membertypecode)='" + ManageQuote(MemberTypeCode.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
            return count;
        }
        public async Task<List<MemberTypeDTO>> GetMemberTypeDetails(string ConnectionString)
        {
            lstMemberTypeDTO = new List<MemberTypeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype,membertypecode,companycode,branchcode,series,serieslength,membercode,issharesissueapplicable,isaccounttypecreationapplicable,ismembershipfeeapplicable,statusid from tblmstmembertypes where statusid = " + Convert.ToInt32(Status.Active) + " order by membertype;"))
                    {
                        while (dr.Read())
                        {
                            MemberTypeDTO MemberTypeDTO = new MemberTypeDTO();
                            MemberTypeDTO.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            MemberTypeDTO.pmembertype = Convert.ToString(dr["membertype"]);
                            MemberTypeDTO.pmembertypecode = Convert.ToString(dr["membertypecode"]);
                            MemberTypeDTO.pcompanycode = Convert.ToString(dr["companycode"]);
                            MemberTypeDTO.pbranchcode = Convert.ToString(dr["branchcode"]);
                            MemberTypeDTO.pseries = Convert.ToString(dr["series"]);
                            MemberTypeDTO.pserieslength = Convert.ToInt64(dr["serieslength"]);
                            MemberTypeDTO.pmembercode = Convert.ToString(dr["membercode"]);
                            MemberTypeDTO.pissharesissueapplicable = Convert.ToBoolean(dr["issharesissueapplicable"]);
                            MemberTypeDTO.pisaccounttypecreationapplicable = Convert.ToBoolean(dr["isaccounttypecreationapplicable"]);
                            MemberTypeDTO.pismembershipfeeapplicable = Convert.ToBoolean(dr["ismembershipfeeapplicable"]);
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                MemberTypeDTO.pstatus = true;
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                MemberTypeDTO.pstatus = false;
                            }
                            lstMemberTypeDTO.Add(MemberTypeDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstMemberTypeDTO;
        }

        public async Task<List<MemberTypeDTO>> GetSavingMemberTypeDetails(string ConnectionString)
        {
            lstMemberTypeDTO = new List<MemberTypeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype,membertypecode,companycode,branchcode,series,serieslength,membercode,issharesissueapplicable,statusid from tblmstmembertypes where Coalesce(isaccounttypecreationapplicable, false) = true and statusid = " + Convert.ToInt32(Status.Active) + " order by membertype;"))
                    {
                        while (dr.Read())
                        {
                            MemberTypeDTO MemberTypeDTO = new MemberTypeDTO();
                            MemberTypeDTO.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            MemberTypeDTO.pmembertype = Convert.ToString(dr["membertype"]);
                            MemberTypeDTO.pmembertypecode = Convert.ToString(dr["membertypecode"]);
                            MemberTypeDTO.pcompanycode = Convert.ToString(dr["companycode"]);
                            MemberTypeDTO.pbranchcode = Convert.ToString(dr["branchcode"]);
                            MemberTypeDTO.pseries = Convert.ToString(dr["series"]);
                            MemberTypeDTO.pserieslength = Convert.ToInt64(dr["serieslength"]);
                            MemberTypeDTO.pmembercode = Convert.ToString(dr["membercode"]);
                            MemberTypeDTO.pissharesissueapplicable = Convert.ToBoolean(dr["issharesissueapplicable"]);
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                MemberTypeDTO.pstatus = true;
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                MemberTypeDTO.pstatus = false;
                            }
                            lstMemberTypeDTO.Add(MemberTypeDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstMemberTypeDTO;
        }
        public async Task<List<MemberTypeDTO>> GetShareMemberTypeDetails(string ConnectionString)
        {
            lstMemberTypeDTO = new List<MemberTypeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype,membertypecode,companycode,branchcode,series,serieslength,membercode,issharesissueapplicable,statusid from tblmstmembertypes where Coalesce(issharesissueapplicable, false) = true and statusid = " + Convert.ToInt32(Status.Active) + " order by membertype;"))
                    {
                        while (dr.Read())
                        {
                            MemberTypeDTO MemberTypeDTO = new MemberTypeDTO();
                            MemberTypeDTO.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            MemberTypeDTO.pmembertype = Convert.ToString(dr["membertype"]);
                            MemberTypeDTO.pmembertypecode = Convert.ToString(dr["membertypecode"]);
                            MemberTypeDTO.pcompanycode = Convert.ToString(dr["companycode"]);
                            MemberTypeDTO.pbranchcode = Convert.ToString(dr["branchcode"]);
                            MemberTypeDTO.pseries = Convert.ToString(dr["series"]);
                            MemberTypeDTO.pserieslength = Convert.ToInt64(dr["serieslength"]);
                            MemberTypeDTO.pmembercode = Convert.ToString(dr["membercode"]);
                            MemberTypeDTO.pissharesissueapplicable = Convert.ToBoolean(dr["issharesissueapplicable"]);
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                MemberTypeDTO.pstatus = true;
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                MemberTypeDTO.pstatus = false;
                            }
                            lstMemberTypeDTO.Add(MemberTypeDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstMemberTypeDTO;
        }

        public bool SaveMemberType(MemberTypeDTO MemberTypeDTO, string ConnectionString)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembertype))
                {
                    MemberTypeDTO.pmembertype = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembertypecode))
                {
                    MemberTypeDTO.pmembertypecode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pcompanycode))
                {
                    MemberTypeDTO.pcompanycode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pbranchcode))
                {
                    MemberTypeDTO.pbranchcode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pseries))
                {
                    MemberTypeDTO.pseries = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembercode))
                {
                    MemberTypeDTO.pmembercode = "";
                }
                trans = con.BeginTransaction();
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO tblmstmembertypes(membertype,membertypecode,companycode,branchcode,series,serieslength,membercode,issharesissueapplicable,statusid,createdby,createddate,isaccounttypecreationapplicable,ismembershipfeeapplicable) VALUES('" + ManageQuote(MemberTypeDTO.pmembertype) + "','" + ManageQuote(MemberTypeDTO.pmembertypecode) + "','" + ManageQuote(MemberTypeDTO.pcompanycode) + "','" + ManageQuote(MemberTypeDTO.pbranchcode) + "','" + ManageQuote(MemberTypeDTO.pseries) + "'," + MemberTypeDTO.pserieslength + ",'" + ManageQuote(MemberTypeDTO.pmembercode) + "'," + MemberTypeDTO.pissharesissueapplicable + "," + Convert.ToInt32(Status.Active) + "," + MemberTypeDTO.pCreatedby + ",CURRENT_TIMESTAMP," + MemberTypeDTO.pisaccounttypecreationapplicable + "," + MemberTypeDTO.pismembershipfeeapplicable + ");");
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

        public bool UpdateMemberType(MemberTypeDTO MemberTypeDTO, string ConnectionString)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembertype))
                {
                    MemberTypeDTO.pmembertype = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembertypecode))
                {
                    MemberTypeDTO.pmembertypecode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pcompanycode))
                {
                    MemberTypeDTO.pcompanycode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pbranchcode))
                {
                    MemberTypeDTO.pbranchcode = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pseries))
                {
                    MemberTypeDTO.pseries = "";
                }
                if (string.IsNullOrEmpty(MemberTypeDTO.pmembercode))
                {
                    MemberTypeDTO.pmembercode = "";
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tblmstmembertypes set membertype='" + ManageQuote(MemberTypeDTO.pmembertype) + "',membertypecode='" + ManageQuote(MemberTypeDTO.pmembertypecode) + "',companycode='" + ManageQuote(MemberTypeDTO.pcompanycode) + "',branchcode='" + ManageQuote(MemberTypeDTO.pbranchcode) + "',series='" + ManageQuote(MemberTypeDTO.pseries) + "',serieslength=" + MemberTypeDTO.pserieslength + ",membercode='" + ManageQuote(MemberTypeDTO.pmembercode) + "',issharesissueapplicable=" + MemberTypeDTO.pissharesissueapplicable + ",isaccounttypecreationapplicable=" + MemberTypeDTO.pisaccounttypecreationapplicable + ",ismembershipfeeapplicable=" + MemberTypeDTO.pismembershipfeeapplicable + "  where membertypeid=" + MemberTypeDTO.pmembertypeid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");
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


    }
}
