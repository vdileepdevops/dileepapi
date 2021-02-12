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
    public class BondsPreviewDAL : SettingsDAL, IBondsPreview
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public async Task<List<GetPreviewDetailsDTO>> GetBondsDetails(string ConnectionString)
        {
            List<GetPreviewDetailsDTO> lstBondsDetails = new List<GetPreviewDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdaccountid,fdaccountno,membername,membercode,fdname,depositamount,interestrate,maturityamount,interestpayable,depositdate,maturitydate,interestpayout,tenor,tenortype,tf.squareyard from tbltransfdcreation tf join receivedamount fr on tf.fdaccountid=fr.fd_account_id where bondprintstatus='N' and depositamount=paidamount and tf.fdaccountid not in(select trans_type_id from maturity_bonds) and tf.fdaccountid not in(select fd_account_id from Bonds_Print);"))
                    {
                        while (dr.Read())
                        {
                            GetPreviewDetailsDTO objBondsDetails = new GetPreviewDetailsDTO();
                            objBondsDetails.pFdaccountid= dr["fdaccountid"];
                            objBondsDetails.pFdaccountno = dr["fdaccountno"];
                            objBondsDetails.pMembername = dr["membername"];
                            objBondsDetails.pMembercode = dr["membercode"];
                            objBondsDetails.pFdname = dr["fdname"];
                            objBondsDetails.pDepositamount = dr["depositamount"];
                            objBondsDetails.pInterestrate = dr["interestrate"];
                            objBondsDetails.pMaturityamount = dr["maturityamount"];
                            objBondsDetails.pInterestpayable = dr["interestpayable"];
                            objBondsDetails.pDepositdate = Convert.ToDateTime(dr["depositdate"]).ToString("dd/MMM/yyyy");
                            objBondsDetails.pMaturitydate = Convert.ToDateTime(dr["maturitydate"]).ToString("dd/MMM/yyyy");
                            objBondsDetails.pInterestpayout = dr["interestpayout"];
                            objBondsDetails.pTenor = dr["tenor"];
                            objBondsDetails.pTenortype = dr["tenortype"];
                            objBondsDetails.psquareyard = dr["squareyard"];
                            lstBondsDetails.Add(objBondsDetails);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstBondsDetails;
        }
        public async Task<BondspreviewMain> GetBondsPreviewDetails(string fdaccountnos, string ConnectionString)
        {
            BondspreviewMain _BondspreviewMain = new BondspreviewMain();
            List<BondsPreviewDTO> lstBondsPreviewDetails = new List<BondsPreviewDTO>();
            _BondspreviewMain._BondsPreviewDTOLst = new List<BondsPreviewDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tf.contactid,fdaccountid,fdaccountno,membername,depositamount,interestrate,maturityamount,interestpayable,depositdate,maturitydate,interestpayout,tenor,tenortype,case when tc.fathername='' then tc.spousename else tc.fathername end as fathername ,tca.address1 || ',' || tca.address2 || ',' || tca.city || ',' || tca.district || ',' || tca.state || '-' || tca.pincode as address,tf.squareyard  from tbltransfdcreation tf left join tblmstcontact tc  on tf.contactid = tc.contactid left join tblmstcontactaddressdetails tca on tf.contactid = tca.contactid   where bondprintstatus = 'N'   and fdaccountno in(" + fdaccountnos + ");"))
                    {
                        while (dr.Read())
                        {
                            BondsPreviewDTO objBondsPreviewDetails = new BondsPreviewDTO();
                            objBondsPreviewDetails.pFdaccountid = dr["fdaccountid"];
                            objBondsPreviewDetails.pFdaccountno = dr["fdaccountno"];
                            objBondsPreviewDetails.pDepositamount = dr["depositamount"];
                            objBondsPreviewDetails.pInterestrate = dr["interestrate"];
                            objBondsPreviewDetails.pMaturityamount = dr["maturityamount"];
                            objBondsPreviewDetails.pInterestpayable = dr["interestpayable"];
                            objBondsPreviewDetails.pDepositdate = Convert.ToDateTime(dr["depositdate"]).ToString("dd/MMM/yyyy");
                            objBondsPreviewDetails.pMaturitydate = Convert.ToDateTime(dr["maturitydate"]).ToString("dd/MMM/yyyy");
                            objBondsPreviewDetails.pInterestpayout = dr["interestpayout"];
                            objBondsPreviewDetails.pFathername = dr["fathername"];
                            objBondsPreviewDetails.pMembername = dr["membername"];
                            objBondsPreviewDetails.pAddress = dr["address"];
                            objBondsPreviewDetails.pTenor = dr["tenor"];
                            objBondsPreviewDetails.pTenortype = dr["tenortype"];
                            objBondsPreviewDetails.pSquareyard = dr["squareyard"];
                            objBondsPreviewDetails.plstNomieeDetails = new List<NomieeDetailsDTO>();
                            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select nomineename,relationship,percentage from tabapplicationpersonalnomineedetails where vchapplicationid='" + objBondsPreviewDetails.pFdaccountno + "' and percentage>0; "))
                            {
                                while (dr1.Read())
                                {
                                    NomieeDetailsDTO objNomieeDetails = new NomieeDetailsDTO();
                                    objNomieeDetails.pNomieename = dr1["nomineename"];
                                    objNomieeDetails.pReletaion = dr1["relationship"];
                                    objNomieeDetails.pProportion = dr1["percentage"];
                                    objBondsPreviewDetails.plstNomieeDetails.Add(objNomieeDetails);
                                }
                            }

                            objBondsPreviewDetails.plstLienDetails = new List<LienDetailsDTO>();
                            using (NpgsqlDataReader dr2 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select lienadjuestto,companybranch,companyname from tbltranslienentry where lienstatus='N' and fdaccountno='" + objBondsPreviewDetails.pFdaccountno + "'; "))
                            {
                                while (dr2.Read())
                                {
                                    LienDetailsDTO objliendetails = new LienDetailsDTO();
                                    objliendetails.pLiento = dr2["lienadjuestto"];
                                    objliendetails.pCompanybranch = dr2["companybranch"];
                                    objliendetails.pCompanyname = dr2["companyname"];
                                    objBondsPreviewDetails.plstLienDetails.Add(objliendetails);
                                }
                            }
                            _BondspreviewMain._BondsPreviewDTOLst.Add(objBondsPreviewDetails);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return _BondspreviewMain;
        }
        public bool SaveBondsPrint(Bonds_PrintDTO ObjBondsprint, string Connectionstring)
        {
            bool Issaved = false;
            StringBuilder sbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (ObjBondsprint.lstBonds_Print != null)
                {
                    for (int i = 0; i < ObjBondsprint.lstBonds_Print.Count; i++)
                    {
                        sbInsert.Append("insert into Bonds_Print(fd_account_id, print_date, print_status)values(" + ObjBondsprint.lstBonds_Print[i].pDeposit_id + ",'" + FormatDate(ObjBondsprint.lstBonds_Print[i].pPrint_Date) + "','N');");
                    }
                }
                
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }
                trans.Commit();
                Issaved = true;
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
            return Issaved;
        }
    }
}
