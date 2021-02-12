using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Settings;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Loans.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class SelfAdjustmentDAL : SettingsDAL, ISelfAdjustment
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region Get Company Names
        public async Task<List<CompanyNamesDTO>> GetCompanyname(string ConnectionString)
        {
            List<CompanyNamesDTO> lstCompanyname = new List<CompanyNamesDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct companyname from tabbranchcodes;"))
                    {
                        while (dr.Read())
                        {
                            CompanyNamesDTO objCompanyname = new CompanyNamesDTO();
                            objCompanyname.pcompanyname = dr["companyname"];
                            lstCompanyname.Add(objCompanyname);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstCompanyname;
        }
        #endregion

        #region Get Branch Names
        public async Task<List<BranchNamesDTO>> GetBranchName(string Companyname, string ConnectionString)
        {
            List<BranchNamesDTO> lstBranchname = new List<BranchNamesDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  code,branchname from tabbranchcodes where companyname='" + Companyname + "' order by branchname;"))
                    {
                        while (dr.Read())
                        {
                            BranchNamesDTO objBranchname = new BranchNamesDTO();
                            objBranchname.pBranchid = dr["code"];
                            objBranchname.pBranchname = dr["branchname"];
                            lstBranchname.Add(objBranchname);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstBranchname;
        }
        public async Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct chitbranchid,chitbranchname from vwfdtransaction_details where balanceamount<=0 and interestpayout<>'On Maturity' and fdaccountid not in(select trans_type_id from maturity_bonds) and fdaccountid not in(select fd_account_id from self_or_adjustment)  order by chitbranchname;"))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["chitbranchid"]),
                                pBranchname = dr["chitbranchname"],

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
        #endregion

        #region Get Scheme Type
        public async Task<List<SchemeTypeDTO>> GetSchemeType(string BranchName, string ConnectionString)
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            string query = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    //query = "select distinct tc.fdname,tc.fdconfigid from tblmstfixeddepositConfig tc join tbltransfdcreation  tf on tc.fdconfigid=tf.fdconfigid and tf.statusid=" + Convert.ToInt32(Status.Active) + "  where chitbranchname='"+ BranchName + "' AND tc.statusid=" + Convert.ToInt32(Status.Active) + " order by fdname";
                    query = "select distinct fdconfigid,fdname from vwfdtransaction_details  where chitbranchname='" + BranchName + "' and balanceamount<=0 and interestpayout<>'On Maturity' and fdaccountid not in(select trans_type_id from maturity_bonds) and fdaccountid not in(select fd_account_id from self_or_adjustment) order by fdname";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            SchemeTypeDTO objSchemetypes = new SchemeTypeDTO();
                            objSchemetypes.pSchemeid = dr["fdconfigid"];
                            objSchemetypes.pSchemeName = dr["fdname"];
                            lstSchemetype.Add(objSchemetypes);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstSchemetype;
        }
        #endregion

        #region Get Member Names
        public async Task<List<MembersDTO>> GetMembers(string branchname, Int64 fdconfigid, string ConnectionString)
        {
            List<MembersDTO> lstMembers = new List<MembersDTO>();
            await Task.Run(() =>
            {
                try
                {
                    //select distinct contactid,memberid,membercode,membername,mobileno from vwfddata where chitbranchname = '" + branchname + "' and fdconfigid = "+ fdconfigid + " order by membername
                    string query = "select distinct memberid,membercode,membername,mobileno,contactid from vwfdtransaction_details where chitbranchname='" + branchname + "' and fdconfigid=" + fdconfigid + " and balanceamount<=0 and interestpayout<>'On Maturity' and fdaccountid not in(select trans_type_id from maturity_bonds) and fdaccountid not in(select fd_account_id from self_or_adjustment) order by membername";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            MembersDTO objMembers = new MembersDTO();
                            objMembers.pMemberid = dr["memberid"];
                            objMembers.pMembercode = dr["membercode"];
                            objMembers.pMembername = dr["membername"];
                            objMembers.pMobileno = dr["mobileno"];
                            objMembers.pContactid = dr["contactid"];
                            lstMembers.Add(objMembers);
                        }


                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstMembers;
        }
        #endregion

        #region Get Fd Account Numbers
        public async Task<List<FdAccountDTO>> GetFdAcnumbers(string branchname, int memberid, Int64 fdconfigid, string ConnectionString)
        {
            List<FdAccountDTO> lstFDAccountno = new List<FdAccountDTO>();
            await Task.Run(() =>
            {
                try
                {
                    //select fdaccountid, fdaccountno, membername from vwfddata where chitbranchname = '" + branchname + "' and memberid = " + memberid + " and fdconfigid = "+fdconfigid+" and fdaccountid not in(select fd_account_id from self_or_adjustment)
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdaccountid,fdaccountno,membername from vwfdtransaction_details where chitbranchname='" + branchname + "' and memberid=" + memberid + " and fdconfigid=" + fdconfigid + " and balanceamount<=0 and interestpayout<>'On Maturity'  and fdaccountid not in(select trans_type_id from maturity_bonds) and fdaccountid not in(select fd_account_id from self_or_adjustment) ;"))
                    {
                        while (dr.Read())
                        {
                            FdAccountDTO objFDAccountno = new FdAccountDTO();
                            objFDAccountno.pFdaccountid = dr["fdaccountid"];
                            objFDAccountno.pFdaccountnumber = dr["fdaccountno"];
                            objFDAccountno.pMembername = dr["membername"];
                            lstFDAccountno.Add(objFDAccountno);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstFDAccountno;
        }
        #endregion

        #region Get Bank Details
        public async Task<List<SelfBankDetailsDTO>> GetBankDetails(int Contactid, string ConnectionString)
        {
            List<SelfBankDetailsDTO> lstBankdetails = new List<SelfBankDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select bankname,accountno,ifsccode,branch from tabapplicationpersonalbankdetails where contactid=" + Contactid + " and isprimarybank=true ;"))
                    {
                        while (dr.Read())
                        {
                            SelfBankDetailsDTO objBankdetails = new SelfBankDetailsDTO();
                            objBankdetails.pBankname = dr["bankname"];
                            objBankdetails.pAccountno = dr["accountno"];
                            objBankdetails.pIfsccode = dr["ifsccode"];
                            objBankdetails.pBranhname = dr["branch"];
                            lstBankdetails.Add(objBankdetails);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstBankdetails;
        }
        #endregion


        public bool SaveSelforAdjustment(SelfAdjustmentConfigDTO Selfadjustment, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder SbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(Selfadjustment.pRecordid.ToString()) || Selfadjustment.pRecordid == 0)
                {
                    SbInsert.Append("insert into self_or_adjustment(self_or_adjustment_date,member_id,fd_account_id,company_name,branch_name,group_code_ticket_no,chit_person_name,Payment_type,bank_name,bank_branch,bank_account_no,ifsc_code,status)values('" + Selfadjustment.pTransdate + "'," + Selfadjustment.pMemberid + "," + Selfadjustment.pFdaccountid + ",'" + Selfadjustment.pCompnayname + "','" + Selfadjustment.pBranchname + "','" + Selfadjustment.pGroupcodeticketno + "','" + Selfadjustment.pChitpersonname + "','" + Selfadjustment.pPaymenttype + "','" + Selfadjustment.pBankname + "','" + Selfadjustment.pBranchname + "','" + Selfadjustment.pBankaccountno + "','" + Selfadjustment.pIfsccode + "',true);");
                }
                else
                {
                    SbInsert.Append("update table self_or_adjustment set self_or_adjustment_date='" + Selfadjustment.pTransdate + "',company_name='" + ManageQuote(Selfadjustment.pCompnayname) + "',branch_name='" + ManageQuote(Selfadjustment.pBranchname) + ",member_id=" + Selfadjustment.pMemberid + ",fd_account_id=" + Selfadjustment.pFdaccountid + ",group_code_ticket_no='" + ManageQuote(Selfadjustment.pGroupcodeticketno) + "',chit_person_name='" + ManageQuote(Selfadjustment.pChitpersonname) + "',Payment_type='" + ManageQuote(Selfadjustment.pPaymenttype) + "',bank_name='" + ManageQuote(Selfadjustment.pBankname) + "',bank_branch='" + ManageQuote(Selfadjustment.pBankbranch) + "',bank_account_no='" + ManageQuote(Selfadjustment.pBankaccountno) + "',ifsc_code='" + ManageQuote(Selfadjustment.pIfsccode) + "' where recordid=" + Selfadjustment.pRecordid + ")");

                }
                if (!string.IsNullOrEmpty(SbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbInsert.ToString());
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

        public async Task<List<SelfAdjustmentConfigDTO>> ViewSelfAdjustmendtetails(string ConnectionString)
        {
            List<SelfAdjustmentConfigDTO> lstSelfAdjustment = new List<SelfAdjustmentConfigDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select self_or_adjustment_id,to_char(self_or_adjustment_date, 'dd/Mon/yyyy') self_or_adjustment_date,member_id,tm.membername,fd_account_id,fdaccountno,company_name,branch_name,group_code_ticket_no,chit_person_name,Payment_type,bank_name,bank_branch,bank_account_no,ifsc_code,status from self_or_adjustment sa join tbltransfdcreation tf on sa.fd_account_id=tf.fdaccountid join tblmstmembers tm on sa.member_id=tm.memberid  where status=true order by self_or_adjustment_id  desc"))
                    {
                        while (dr.Read())
                        {
                            SelfAdjustmentConfigDTO objSelfAdjustment = new SelfAdjustmentConfigDTO();
                            objSelfAdjustment.pRecordid = Convert.ToInt64(dr["self_or_adjustment_id"]);
                            objSelfAdjustment.pTransdate = Convert.ToString(dr["self_or_adjustment_date"]);
                            objSelfAdjustment.pMemberid = Convert.ToInt64(dr["member_id"]);
                            objSelfAdjustment.pFdaccountid = Convert.ToInt64(dr["fd_account_id"]);
                            objSelfAdjustment.pCompnayname = Convert.ToString(dr["company_name"]);
                            objSelfAdjustment.pBranchname = Convert.ToString(dr["branch_name"]);
                            objSelfAdjustment.pChitpersonname = Convert.ToString(dr["chit_person_name"]);
                            objSelfAdjustment.pPaymenttype = Convert.ToString(dr["Payment_type"]);
                            objSelfAdjustment.pBankname = Convert.ToString(dr["bank_name"]);
                            objSelfAdjustment.pBankbranch = Convert.ToString(dr["bank_branch"]);
                            objSelfAdjustment.pBankaccountno = Convert.ToString(dr["bank_account_no"]);
                            objSelfAdjustment.pIfsccode = Convert.ToString(dr["ifsc_code"]);
                            objSelfAdjustment.pGroupcodeticketno = Convert.ToString(dr["group_code_ticket_no"]);
                            objSelfAdjustment.pMemberName = Convert.ToString(dr["membername"]);
                            objSelfAdjustment.pFdAccountno = Convert.ToString(dr["fdaccountno"]);
                            lstSelfAdjustment.Add(objSelfAdjustment);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstSelfAdjustment;
        }
        public async Task<List<SelfAdjustmentConfigDTO>> GetSelfAdjustmendtetailsByid(int memberid, int fdid, string ConnectionString)
        {
            List<SelfAdjustmentConfigDTO> lstSelfAdjustment = new List<SelfAdjustmentConfigDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select self_or_adjustment_id,self_or_adjustment_date,member_id,fd_account_id,company_name,branch_name,group_code_ticket_no,chit_person_name,Payment_type,bank_name,bank_branch,bank_account_no,ifsc_code,status from self_or_adjustment where member_id=" + memberid + " and fd_account_id=" + fdid + " and status=true"))
                    {
                        while (dr.Read())
                        {
                            SelfAdjustmentConfigDTO objSelfAdjustment = new SelfAdjustmentConfigDTO();
                            objSelfAdjustment.pRecordid = Convert.ToInt64(dr["self_or_adjustment_id"]);
                            objSelfAdjustment.pTransdate = Convert.ToString(dr["self_or_adjustment_date"]);
                            objSelfAdjustment.pMemberid = Convert.ToInt64(dr["member_id"]);
                            objSelfAdjustment.pFdaccountid = Convert.ToInt64(dr["fd_account_id"]);
                            objSelfAdjustment.pCompnayname = Convert.ToString(dr["company_name"]);
                            objSelfAdjustment.pBranchname = Convert.ToString(dr["branch_name"]);
                            objSelfAdjustment.pChitpersonname = Convert.ToString(dr["chit_person_name"]);
                            objSelfAdjustment.pPaymenttype = Convert.ToString(dr["Payment_type"]);
                            objSelfAdjustment.pBankname = Convert.ToString(dr["bank_name"]);
                            objSelfAdjustment.pBankbranch = Convert.ToString(dr["bank_branch"]);
                            objSelfAdjustment.pBankaccountno = Convert.ToString(dr["bank_account_no"]);
                            objSelfAdjustment.pIfsccode = Convert.ToString(dr["ifsc_code"]);
                            objSelfAdjustment.pGroupcodeticketno = Convert.ToString(dr["group_code_ticket_no"]);
                            lstSelfAdjustment.Add(objSelfAdjustment);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstSelfAdjustment;
        }
    }
}
