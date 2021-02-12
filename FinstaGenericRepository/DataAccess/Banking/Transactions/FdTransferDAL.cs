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
    public class FdTransferDAL : SettingsDAL, IFdTransfer
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public async Task<List<FdschemeNameandCode>> GetFdSchemes( string ConnectionString)
        {
            List<FdschemeNameandCode> _FdNameandCodeList = new List<FdschemeNameandCode>();
            await Task.Run(() =>
            {
                try
                {
                   
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct tc.fdconfigid,tc.fdname,tc.fdcode,tc.fdnamecode,fdcalculationmode from tblmstfixeddepositConfig tc join tblmstfixeddepositConfigdetails tcd on tc.fdconfigid=tcd.fdconfigid where tc.statusid=" + Convert.ToInt32(Status.Active) + " and tc.fdname in (select fdname from tbltransfdcreation); "))
                    {
                        while (dr.Read())
                        {
                            var _FdNameandCode = new FdschemeNameandCode
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

        #region  Get Fd Details

        public List<FDDetailsDTO> GetFdFromDetails( string Connectionstring)
        {
            List<FDDetailsDTO> lstFDDetails = new List<FDDetailsDTO>();
            try
            {

                //  string   Query = "select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,memberid from tbltransfdcreation  where statusid=" + Convert.ToInt32(Status.Active) + " and fdaccountid in (select fd_account_id from fd_receipt);";

                // string Query = "select a.*,receivedamount from (select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,memberid from tbltransfdcreation  where statusid=" + Convert.ToInt32(Status.Active) + " and fdaccountid in (select fd_account_id from fd_receipt)) a join (select fd_account_id, sum(received_amount) as receivedamount from fd_receipt group by fd_account_id) b on a.fdaccountid=b.fd_account_id where a.depositamount=b.receivedamount order by membername;";

                string Query = "select a.*,receivedamount,'' as receipt_no from (select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,memberid from tbltransfdcreation  where statusid=" + Convert.ToInt32(Status.Active) + " and fdaccountid in (select fd_account_id from fd_receipt)) a join (select fd_account_id, sum(received_amount) as receivedamount from fd_receipt where mode_of_receipt='CASH' group by fd_account_id) b on a.fdaccountid=b.fd_account_id where a.depositamount=b.receivedamount union select a.*,receivedamount,receipt_no from (select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,memberid from tbltransfdcreation  where statusid=" + Convert.ToInt32(Status.Active) + " and fdaccountid in (select fd_account_id from fd_receipt)) a join (select fd_account_id, sum(received_amount) as receivedamount,receipt_no from fd_receipt where mode_of_receipt='BANK' group by fd_account_id,receipt_no) b on a.fdaccountid=b.fd_account_id join tbltransreceiptreference tr on tr.receiptid=b.receipt_no where transtype = 'CHEQUE' and clearstatus = 'Y'; ";
                
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        FDDetailsDTO objFDdetails = new FDDetailsDTO();
                        objFDdetails.pFdaccountid =Convert.ToInt64( dr["fdaccountid"]);
                        objFDdetails.pFdaccountno = dr["fdaccountno"];
                        objFDdetails.pMembername = dr["membername"];
                        objFDdetails.pChitbranchname = dr["chitbranchname"];
                        objFDdetails.pMemberId = Convert.ToInt64(dr["memberid"]);
                        lstFDDetails.Add(objFDdetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFDDetails;
        }

        public List<FDDetailsDTO> GetFdToDetails( string Branchname, string Membercode, string Connectionstring)
        {
            List<FDDetailsDTO> lstFDDetails = new List<FDDetailsDTO>();
            try
            {
                string Query = string.Empty;
               
                
                    Query = "select fdaccountid,fdaccountno,membername,chitbranchname,depositamount,accountid,memberid from tbltransfdcreation  where statusid=" + Convert.ToInt32(Status.Active) + " and upper(membercode) ='" + ManageQuote(Membercode).ToUpper() + "' and upper(chitbranchname)='" + ManageQuote(Branchname).ToUpper() + "'  and fdaccountid not in (select fd_account_id from fd_receipt) ;";
                
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        FDDetailsDTO objFDdetails = new FDDetailsDTO();
                        objFDdetails.pFdaccountid =Convert.ToInt64( dr["fdaccountid"]);
                        objFDdetails.pFdaccountno = dr["fdaccountno"];
                        objFDdetails.pMembername = dr["membername"];
                        objFDdetails.pChitbranchname = dr["chitbranchname"];
                        objFDdetails.pMemberId = Convert.ToInt64(dr["memberid"]);
                        lstFDDetails.Add(objFDdetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFDDetails;
        }
        #endregion

        #region  Get Fd Details By ID
        public List<FDDetailsByIdDTO> GetFromFdDetailsByid(string FdAccountNo, string Connectionstring)
        {
            List<FDDetailsByIdDTO> lstFDDetailsbyid = new List<FDDetailsByIdDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select fdaccountid,fdaccountno,membercode,membername,depositamount,maturityamount,accountid,tenortype,tenor,interesttype,interestrate,interestpayable,interestpayout,to_char(depositdate, 'dd-mon-yyyy')depositdate,to_char(maturitydate, 'dd-mon-yyyy')maturitydate  from tbltransfdcreation where upper(fdaccountno)='" +ManageQuote( FdAccountNo ).ToUpper()+ "';"))
                {
                    while (dr.Read())
                    {
                        FDDetailsByIdDTO objFDdetailsbyid = new FDDetailsByIdDTO();
                        objFDdetailsbyid.pFdaccountid = dr["fdaccountid"];
                        objFDdetailsbyid.pFdaccountno = dr["fdaccountno"];
                        objFDdetailsbyid.pMembername = dr["membername"];
                        objFDdetailsbyid.pDeposiamount = dr["depositamount"];
                        objFDdetailsbyid.pMaturityamount = dr["maturityamount"];
                        objFDdetailsbyid.pAccountno = dr["accountid"];
                        objFDdetailsbyid.pTenortype = dr["tenortype"];
                        objFDdetailsbyid.pTenor = dr["tenor"];
                        objFDdetailsbyid.pInteresttype = dr["interesttype"];
                        objFDdetailsbyid.pInterestrate = dr["interestrate"];
                        objFDdetailsbyid.pInterestPayble = dr["interestpayable"];
                        objFDdetailsbyid.pInterestPayout = dr["interestpayout"];
                        objFDdetailsbyid.pDeposidate = dr["depositdate"];
                        objFDdetailsbyid.pMaturitydate = dr["maturitydate"];
                        lstFDDetailsbyid.Add(objFDdetailsbyid);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstFDDetailsbyid;
        }
        public List<FDDetailsByIdDTO> GetToFdDetailsByid(string FdAccountNo, string Connectionstring)
        {
            List<FDDetailsByIdDTO> lstFDDetailsbyid = new List<FDDetailsByIdDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select fdaccountid,fdaccountno,membercode,membername,depositamount,maturityamount,accountid,tenortype,tenor,interesttype,interestrate,interestpayable,interestpayout,to_char(depositdate, 'dd-mon-yyyy')depositdate,to_char(maturitydate, 'dd-mon-yyyy') maturitydate  from tbltransfdcreation where upper(fdaccountno)='" +ManageQuote( FdAccountNo).ToUpper() + "';"))
                {
                    while (dr.Read())
                    {
                        FDDetailsByIdDTO objFDdetailsbyid = new FDDetailsByIdDTO();
                        objFDdetailsbyid.pFdaccountid = dr["fdaccountid"];
                        objFDdetailsbyid.pFdaccountno = dr["fdaccountno"];
                        objFDdetailsbyid.pMembername = dr["membername"];
                        objFDdetailsbyid.pDeposiamount = dr["depositamount"];
                        objFDdetailsbyid.pMaturityamount = dr["maturityamount"];
                        objFDdetailsbyid.pAccountno = dr["accountid"];
                        objFDdetailsbyid.pTenortype = dr["tenortype"];
                        objFDdetailsbyid.pTenor = dr["tenor"];
                        objFDdetailsbyid.pInteresttype = dr["interesttype"];
                        objFDdetailsbyid.pInterestrate = dr["interestrate"];
                        objFDdetailsbyid.pInterestPayble = dr["interestpayable"];
                        objFDdetailsbyid.pInterestPayout = dr["interestpayout"];
                        objFDdetailsbyid.pDeposidate = dr["depositdate"];
                        objFDdetailsbyid.pMaturitydate = dr["maturitydate"];
                        lstFDDetailsbyid.Add(objFDdetailsbyid);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstFDDetailsbyid;
        }

        public bool SaveFdTransfer(Fdtransfersave _Fdtransfersave, string Connectionstring)
        {
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(_Fdtransfersave.pTransferdate))
                {
                    _Fdtransfersave.pTransferdate = "null";
                }
                else
                {
                    _Fdtransfersave.pTransferdate = "'" + FormatDate(_Fdtransfersave.pTransferdate) + "'";
                }
                string SbsaveReferences = string.Empty;
                SbsaveReferences = "INSERT INTO Transfer(from_member_id, to_member_id, from_fd_account_id, to_fd_account_id,transfer_date, status) VALUES ( " + _Fdtransfersave.pFromMemberId + ", '" + _Fdtransfersave.pToMemberId + "', '" + _Fdtransfersave.pFromAccountId + "', '" + _Fdtransfersave.pToAccountId + "', " + _Fdtransfersave.pTransferdate + ",'true');";                                 
                
                if (!string.IsNullOrEmpty(SbsaveReferences))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbsaveReferences);
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

        public List<FDMemberDetailsDTO> GetMemberDetails(string BranchName, string Connectionstring)
        {
            List<FDMemberDetailsDTO> lstMemberDetails = new List<FDMemberDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select  distinct tm.memberid,tm.Membername, tm.membercode, tc.contactid, tc.contactreferenceid, tm.membertype, tc.name, tc.businessentitycontactno from tblmstmembers tm join tblmstcontact tc on tm.contactid = tc.contactid join tbltransfdcreation tf on tm.memberid = tf.memberid where  tf.chitbranchname ='" + BranchName + "' and tm.statusid=" + Convert.ToInt32(Status.Active
                    ) + " and tf.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        FDMemberDetailsDTO objMemberdetails = new FDMemberDetailsDTO();
                        objMemberdetails.pMemberid = dr["memberid"];
                        objMemberdetails.pMembercode = dr["membercode"];
                        objMemberdetails.pName = dr["Membername"];
                        objMemberdetails.pConid = dr["contactid"];
                        objMemberdetails.pContactreferenceid = dr["contactreferenceid"];
                        objMemberdetails.pMembertype = dr["membertype"];
                        objMemberdetails.pBusinessentitycontactno = dr["businessentitycontactno"];
                        lstMemberDetails.Add(objMemberdetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberDetails;
        }
        #endregion
    }
}
