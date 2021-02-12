
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Transactions;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class LienReleaseTransactionDAL : SettingsDAL, ILienReleaseTransaction
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        public List<LienRelaseBranchrDTO> GetBranches(string connectionstring)
        {
            List<LienRelaseBranchrDTO> lstlienreleseBranches = new List<LienRelaseBranchrDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct chitbranchid,chitbranchname from tbltranslienentry ln join tbltransfdcreation tf on ln.fdaccountno=tf.fdaccountno where lienstatus='N' order by chitbranchname; "))
                {
                    while (dr.Read())
                    {
                        LienRelaseBranchrDTO objlienrelaseBranches = new LienRelaseBranchrDTO()
                        {
                            pBranchname=dr["chitbranchname"],
                            pBranchcode=dr["chitbranchid"]

                        };
                        lstlienreleseBranches.Add(objlienrelaseBranches);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstlienreleseBranches;
        }
        public List<LienRelasegetmemberDTO> LienReleasemembercode(string Branchname,string connectionstring)
        {
            List<LienRelasegetmemberDTO> lstlienreleasemember = new List<LienRelasegetmemberDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct tm.memberid,tl.membercode,tm.membername,businessentitycontactno as contactnumber from tbltranslienentry tl  join tbltransfdcreation tf on tl.fdaccountno=tf.fdaccountno join tblmstmembers tm on tl.membercode=tm.membercode join tblmstcontact tc on tc.contactid = tm.contactid where  tl.lienstatus = 'N' and tl.statusid=" + Convert.ToInt32(Status.Active) + " and tf.statusid=" + Convert.ToInt32(Status.Active) + "  and tf.chitbranchname='" + Branchname+"'; "))
                {
                    while (dr.Read())
                    {
                        LienRelasegetmemberDTO objlienrelasemember = new LienRelasegetmemberDTO()
                        {
                            pMemberid=dr["memberid"],
                            pMembercode=dr["membercode"],
                            pMembername=dr["membername"],
                            pContactnumber=dr["contactnumber"]

                        };
                        lstlienreleasemember.Add(objlienrelasemember);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstlienreleasemember;
        }

        public async Task<List<LienRelasegetmemberDTO>> Getlienreleasefd(string Membercode,string BranchName, string connectionstring)
        {
            var _Lienrelasefd = new List<LienRelasegetmemberDTO>();
            await Task.Run(() =>
            {
                try
                {
                    NpgsqlDataReader dr;

                    using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select distinct tl.fdaccountno from tbltranslienentry tl join tbltransfdcreation tf on tl.fdaccountno=tf.fdaccountno and tf.statusid=" + Convert.ToInt32(Status.Active) + " where tl.membercode  = '" + Membercode + "' and lienstatus = 'N' and tf.chitbranchname='"+ BranchName + "' and tl.statusid = " + Convert.ToInt32(Status.Active) + ""))
                    {
                        while (dr.Read())
                        {
                            var _Lienrelasefd1 = new LienRelasegetmemberDTO
                            {

                                pFdaccountno = dr["fdaccountno"]
                                

                            };
                            _Lienrelasefd.Add(_Lienrelasefd1);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _Lienrelasefd;
        }

        public async Task<List<LienReleasememberfdDTO>> GetLienrelasedata(string Membercode, string Fdraccountno,string LienDate, string connectionstring)
        {
           List<LienReleasememberfdDTO> _Lienreleaselist = new List<LienReleasememberfdDTO>();
            await Task.Run(() =>
            {
                try
                {
                    NpgsqlDataReader dr;    
                    using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select to_char(b.liendate,'dd/Mon/yyyy')liendate,a.membername,a.tenor,a.depositamount,lienamount,b.companybranch,b.lienadjuestto,b.lienid from tbltransfdcreation a,tbltranslienentry b  where a.MEMBERCODE=b.MEMBERCODE AND  a.fdaccountno = b.fdaccountno and a.MEMBERCODE = '" + Membercode + "' and a.fdaccountno = '" + Fdraccountno + "' and b.liendate<='"+FormatDate(LienDate)+"' and b.lienid not in(select lienid from tbltranslienrealse c where c.lienid=b.lienid  and c.statusid=1 ) and b.lienstatus = 'N'  and b.statusid = " + Convert.ToInt32(Status.Active) + ""))
                    {
                        while (dr.Read())
                        {
                            LienReleasememberfdDTO _Lienreleasedata = new LienReleasememberfdDTO
                            {
                                pLiendate = dr["liendate"],
                                pMembername = dr["membername"],
                                pTenor = dr["tenor"],
                                pDepositamount = dr["depositamount"],
                                pLienamount = dr["lienamount"],
                                pCompanybranch = dr["companybranch"],
                                pLienadjuestto = dr["lienadjuestto"],
                                pLienid = dr["lienid"]

                            };
                            _Lienreleaselist.Add(_Lienreleasedata);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            return _Lienreleaselist;
        }


        public bool SaveLienreleaseentry(LienreleaseSaveDTO _LienreleasesaveDTO, string connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            string qry = string.Empty;
            try
            {

                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_LienreleasesaveDTO.ListLienreleaseDTO != null)
                {
                    for(int i=0;i< _LienreleasesaveDTO.ListLienreleaseDTO.Count; i++)
                    {
                        if (Convert.ToString(_LienreleasesaveDTO.ListLienreleaseDTO[i].ptypeofoperation) == "CREATE")
                        {
                            sbinsert.Append("insert into tbltranslienrealse(lienid,lienrealsedate,statusid,createdby,createddate)values(" + _LienreleasesaveDTO.ListLienreleaseDTO[i].pLienid + ",'" + FormatDate(_LienreleasesaveDTO.ListLienreleaseDTO[i].pLienrealsedate.ToString()) + "'," + Convert.ToInt32(Status.Active) + ", " + _LienreleasesaveDTO.ListLienreleaseDTO[i].pCreatedby + ", current_timestamp);");

                            sbinsert.Append("update tbltranslienentry set lienstatus='Y' where lienid= " + _LienreleasesaveDTO.ListLienreleaseDTO[i].pLienid + ";");
                        }

                        else
                        {
                            sbinsert.Append("Update tbltranslienrealse set lienid = " + _LienreleasesaveDTO.ListLienreleaseDTO[i].pLienid + ",lienrealsedate = '" + FormatDate(_LienreleasesaveDTO.ListLienreleaseDTO[i].pLienrealsedate.ToString()) + "',modifiedby = '" + _LienreleasesaveDTO.ListLienreleaseDTO[i].pCreatedby + "',modifieddate= current_timestamp where lienid = " + _LienreleasesaveDTO.ListLienreleaseDTO[i].pLienid + ";");

                        }
                    }
                }
               

                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
                }

                trans.Commit();
                IsSaved = true;
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


        public bool DeleteLienreleaseentry(long Lienid, string connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            try
            {

                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(Lienid.ToString()) && Lienid != 0)
                {
                    sbInsert.Append("update tbltranslienrealse set statusid=" + Convert.ToInt32(Status.Inactive) + " where lienid=" + Lienid + ";");
                    sbInsert.Append("update tbltranslienentry set lienstatus = 'N' where lienid=" + Lienid + ";");

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

        public LienReleaseviewDTO Lienreleaseviewdata(string connectionstring)
        {
            List<LienReleasememberfdDTO> lstleinreleaseview = new List<LienReleasememberfdDTO>();
            try
            {


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select a.fdaccountno, a.membername,a.tenor,a.depositamount,lienamount,b.companybranch,b.lienadjuestto,b.lienid,to_char(b.liendate, 'dd/Mon/yyyy')liendate,c.lienrealsedate from tbltransfdcreation a,tbltranslienentry b,tbltranslienrealse c  where a.MEMBERCODE = b.membercode and a.fdaccountno = b.fdaccountno and b.lienstatus = 'Y' and b.lienid = c.lienid  and  c.statusid = " + Convert.ToInt32(Status.Active) + " order by c.recordid desc;"))
                {
                    while (dr.Read())
                    {
                        LienReleasememberfdDTO objlienrelaseview = new LienReleasememberfdDTO();
                        objlienrelaseview.pFdaccountno = dr["fdaccountno"];
                        objlienrelaseview.pMembername = dr["membername"];
                        objlienrelaseview.pTenor = dr["tenor"];
                        objlienrelaseview.pDepositamount = dr["depositamount"];
                        objlienrelaseview.pLienamount = dr["lienamount"];
                        objlienrelaseview.pCompanybranch = dr["companybranch"];
                        objlienrelaseview.pLienadjuestto = dr["lienadjuestto"];
                        objlienrelaseview.pLienid = dr["lienid"];
                        objlienrelaseview.pLiendate = dr["liendate"];
                        objlienrelaseview.pLienrealsedate = dr["lienrealsedate"];


                        lstleinreleaseview.Add(objlienrelaseview);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            LienReleaseviewDTO listlienrelease = new LienReleaseviewDTO();
            listlienrelease.LienReleaselist = lstleinreleaseview;
            return listlienrelease;
        }


    }

}
