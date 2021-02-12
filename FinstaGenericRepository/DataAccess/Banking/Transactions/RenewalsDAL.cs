using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Loans.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class RenewalsDAL : SettingsDAL, IRenewals
    {
        #region Get Scheme Type
        public async Task<List<SchemeTypeDTO>> GetSchemeType(string ConnectionString)
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct fdname,fdconfigid from tblmstfixeddepositConfig  where statusid = " + Convert.ToInt32(Status.Active) + " order by fdname;"))
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

        #region Get Fd Accounts
        public async Task<List<FDAccountsDTO>> GetFdaccounts(string ConnectionString)
        {
            List<FDAccountsDTO> lstFdAccounts = new List<FDAccountsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdaccountid,fdaccountno,membername,membercode,memberid from tbltransfdcreation where accountstatus='N' and maturitydate<current_date;"))
                    {
                        while (dr.Read())
                        {
                            FDAccountsDTO objFdAccounts = new FDAccountsDTO();
                            objFdAccounts.pFdaccountid = dr["fdaccountid"];
                            objFdAccounts.pFdaccountno = dr["fdaccountno"];
                            objFdAccounts.pMembername = dr["membername"];
                            objFdAccounts.pMembercode = dr["membercode"];
                            objFdAccounts.pMemberid = dr["memberid"];
                            lstFdAccounts.Add(objFdAccounts);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstFdAccounts;
        }
        #endregion

        #region Get Fd Account Details
        public async Task<List<FDAccountDetailsDTO>> GetFdAccountDetails(int fdid,string ConnectionString)
        {
            List<FDAccountDetailsDTO> lstFdAccountDetails = new List<FDAccountDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdaccountid,fdaccountno,membername,membercode,memberid,contactreferenceid,fdname,tenor,tenortype,interestrate,depositamount,maturityamount,depositdate,maturitydate from tbltransfdcreation   where fdaccountid="+ fdid + ";"))
                    {
                        while (dr.Read())
                        {
                            FDAccountDetailsDTO objFdAccountdetails = new FDAccountDetailsDTO();
                            objFdAccountdetails.pFdaccountid = dr["fdaccountid"];
                            objFdAccountdetails.pFdaccountno = dr["fdaccountno"];
                            objFdAccountdetails.pMembername = dr["membername"];
                            objFdAccountdetails.pMembercode = dr["membercode"];
                            objFdAccountdetails.pMemberid = dr["memberid"];

                            objFdAccountdetails.pContactreferenceid = dr["contactreferenceid"];
                            objFdAccountdetails.pFdname = dr["fdname"];
                            objFdAccountdetails.pTenor = dr["tenor"];
                            objFdAccountdetails.pTenortype = dr["tenortype"];
                            objFdAccountdetails.pInterestrate = dr["interestrate"];
                            objFdAccountdetails.pDepositamount = dr["depositamount"];
                            objFdAccountdetails.pMaturityamount = dr["maturityamount"];
                            objFdAccountdetails.pDepositdate = dr["depositdate"];
                            objFdAccountdetails.pMaturitydate = dr["maturitydate"];                           
                            lstFdAccountDetails.Add(objFdAccountdetails);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstFdAccountDetails;
        }
        #endregion
    }
}
