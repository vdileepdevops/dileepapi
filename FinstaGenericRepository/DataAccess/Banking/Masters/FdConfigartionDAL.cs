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
   public class FdConfigartionDAL:SettingsDAL,IFdConfigartion
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;     
        #region GetFdViewDetails
        public async Task<List<FDViewDTO>> GetFdViewDetails(string ConnectionString)
        {
            List<FDViewDTO> lstFdview = new List<FDViewDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdconfigid,fdname,fdcode,companycode,branchcode,series,serieslength,fdnamecode,statusid from tblmstfixeddepositConfig where statusid = " + Convert.ToInt32(Status.Active) + " order by fdcode;"))
                    {
                        while (dr.Read())
                        {
                            FDViewDTO objFDview = new FDViewDTO();
                            objFDview.pFDconfigid = Convert.ToInt64(dr["fdconfigid"]);
                            objFDview.pFDname = Convert.ToString(dr["fdname"]);
                            objFDview.pFdnamecode = Convert.ToString(dr["fdnamecode"]);
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                            {
                                objFDview.pstatus = true;
                            }
                            if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                            {
                                objFDview.pstatus = false;
                            }
                            lstFdview.Add(objFDview);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstFdview;
        }

        public async Task<FdNameAndCodeDTO> GetFdNameAndCodeDetails(string FDName,string FdCode,string ConnectionString)
        {
            FdNameAndCodeDTO FdNameAndCodeDTO = new FdNameAndCodeDTO();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select fdconfigid,fdname,fdcode,companycode,branchcode,series,serieslength,fdnamecode from tblmstfixeddepositConfig where upper(fdname) = '" + ManageQuote(FDName.ToUpper()) + "' and upper(fdnamecode) ='" + ManageQuote(FdCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        if (dr.Read())
                        {
                            FdNameAndCodeDTO.pFdconfigid = Convert.ToInt64(dr["fdconfigid"]);
                            FdNameAndCodeDTO.pFdname = Convert.ToString(dr["fdname"]);
                            FdNameAndCodeDTO.pFdcode = Convert.ToString(dr["fdcode"]);
                            FdNameAndCodeDTO.pCompanycode = Convert.ToString(dr["companycode"]);
                            FdNameAndCodeDTO.pBranchcode = Convert.ToString(dr["branchcode"]);
                            FdNameAndCodeDTO.pSeries = Convert.ToString(dr["series"]);
                            FdNameAndCodeDTO.pSerieslength = Convert.ToInt64(dr["serieslength"]);
                            FdNameAndCodeDTO.pFdnamecode = Convert.ToString(dr["fdnamecode"]);
                        }

                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return FdNameAndCodeDTO;
        }

        public bool SaveFdNameAndCode(FdNameAndCodeDTO FdNameAndCodeDTO, string Connectionstring)
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
                if (string.IsNullOrEmpty(FdNameAndCodeDTO.pFdconfigid.ToString()) || FdNameAndCodeDTO.pFdconfigid == 0)
                {
                    sbInsert.Append("insert into tblmstfixeddepositConfig (fdname,fdcode,companycode,branchcode,series,serieslength,fdnamecode,statusid,createdby,createddate)values('" + ManageQuote(FdNameAndCodeDTO.pFdname) + "','" + ManageQuote(FdNameAndCodeDTO.pFdcode) + "','" + ManageQuote(FdNameAndCodeDTO.pCompanycode) + "','" + ManageQuote(FdNameAndCodeDTO.pBranchcode) + "','" + ManageQuote(FdNameAndCodeDTO.pSeries) + "'," + FdNameAndCodeDTO.pSerieslength + ",'" + ManageQuote(FdNameAndCodeDTO.pFdnamecode) + "'," + Convert.ToInt32(Status.Active) + "," + FdNameAndCodeDTO.pCreatedby + ",current_timestamp);");
                    
                }
                else
                {
                    sbInsert.Append("update tblmstfixeddepositConfig set fdname='" + ManageQuote(FdNameAndCodeDTO.pFdname) + "', fdcode='" + ManageQuote(FdNameAndCodeDTO.pFdcode) + "', companycode='" + ManageQuote(FdNameAndCodeDTO.pCompanycode) + "', branchcode='" + ManageQuote(FdNameAndCodeDTO.pBranchcode) + "',series='" + ManageQuote(FdNameAndCodeDTO.pSeries) + "', serieslength=" + FdNameAndCodeDTO.pSerieslength + ", fdnamecode='" + ManageQuote(FdNameAndCodeDTO.pFdnamecode) + "' where fdconfigid=" + FdNameAndCodeDTO.pFdconfigid + ";");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text,sbInsert.ToString());
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

        public bool SaveFDConfigurationDetails(FdConfigDeatails FdConfigDeatails, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            string Recordid = string.Empty;
            string Recordid1 = string.Empty;
            StringBuilder queryUpdate = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (FdConfigDeatails.lstFDConfigartionDetailsDTO.Count > 0)
                {
                    for (int i = 0; i < FdConfigDeatails.lstFDConfigartionDetailsDTO.Count; i++)
                    {
                        if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode.Trim().ToUpper() != "TABLE")
                        {
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = FdConfigDeatails.lstFDConfigartionDetailsDTO[i].precordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].precordid.ToString();
                                }
                            }
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                            {
                                sbInsert.Append("insert into tblmstfixeddepositConfigdetails(fdconfigid, fdname, membertypeid, membertype, applicanttype, fdcalculationmode, mindepositamount, maxdepositamount, investmentperiodfrom, investmentperiodto, interestpayout, interesttype, interestratefrom, interestrateto, valuefor100, statusid, createdby, createddate,compoundinteresttype,isreferralcommissionapplicable,referralcommissiontype,commissionvalue,tdsaccountid,tdssection,tdspercentage,multiplesof,caltype)values(" + FdConfigDeatails.pFdconfigid + ",'" + ManageQuote(FdConfigDeatails.pFdname) + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertypeid + ",'" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertype) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pApplicanttype) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode) + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMininstalmentamount + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMaxinstalmentamount + ",'" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInvestmentperiodfrom) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInvestmentperiodto) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestpayuot) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInteresttype) + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestratefrom + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestrateto + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pValueper100 + "," + Convert.ToInt32(Status.Active) + "," + FdConfigDeatails.pCreatedby + ",current_timestamp,'"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestCompunding+ "','"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pIsreferralcommissionapplicable + "','" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pReferralcommissiontype + "',"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i] .pCommissionValue+ ",'"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i]. pTdsaccountid + "','" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + "',"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdspercentage + ","+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMultiplesofamount + ",'"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pCaltype + "');");
                            }
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                            {
                                sbInsert.Append("update tblmstfixeddepositConfigdetails set membertypeid=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertypeid + "membertype ='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertype) + "', applicanttype='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pApplicanttype) + "', fdcalculationmode='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode) + "', mindepositamount=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMininstalmentamount + ", maxdepositamount=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMaxinstalmentamount + ", investmentperiodfrom'" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInvestmentperiodfrom) + "', investmentperiodto='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInvestmentperiodto) + "', interestpayout='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestpayuot) + "', interesttype='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInteresttype) + "', interestratefrom=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestratefrom + ", interestrateto" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestrateto + ", valuefor100=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pValueper100 + ",compoundinteresttype='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestCompunding + "',modifiedby=" + FdConfigDeatails.pCreatedby + ",modifieddate=current_timestamp,isreferralcommissionapplicable='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pIsreferralcommissionapplicable + "',referralcommissiontype='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pReferralcommissiontype + "',commissionvalue=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pCommissionValue + ",tdsaccountid='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdsaccountid + "',tdssection='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + "',tdspercentage=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + ",multiplesof=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMultiplesofamount + ", caltype = '" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pCaltype + "' where fdconfigid =" + FdConfigDeatails.pFdconfigid + ";");
                            }
                        }
                        else
                        {
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = FdConfigDeatails.lstFDConfigartionDetailsDTO[i].precordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].precordid.ToString();
                                }
                            }
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                            {
                                sbInsert.Append("insert into tblmstfixeddepositConfigdetails(fdconfigid, fdname, membertypeid, membertype, applicanttype, fdcalculationmode,interestpayout,tenure,tenuremode,interestamount,depositamount,maturityamount,payindenomination, statusid, createdby, createddate,isreferralcommissionapplicable,referralcommissiontype,commissionvalue,tdsaccountid,tdssection,tdspercentage,multiplesof, caltype)values(" + FdConfigDeatails.pFdconfigid + ",'" + ManageQuote(FdConfigDeatails.pFdname) + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertypeid + ",'" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertype) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pApplicanttype) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode) + "','" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestpayuot) + "',"+Convert.ToInt32(FdConfigDeatails.lstFDConfigartionDetailsDTO[i] .ptenure)+ ",'"+ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].ptenuremode)+ "',"+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pinterestamount + ","+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pdepositamount + ","+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pmaturityamount + ","+ FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pPayindenomination + "," + Convert.ToInt32(Status.Active) + "," + FdConfigDeatails.pCreatedby + ",current_timestamp,'" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pIsreferralcommissionapplicable + "','" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pReferralcommissiontype + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pCommissionValue + ",'" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdsaccountid + "','" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + "'," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdspercentage + "," + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMultiplesofamount + ",'" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode + "');");
                            }
                            if (FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                            {
                                sbInsert.Append("Update tblmstfixeddepositConfigdetails set membertypeid=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertypeid + "membertype ='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMembertype) + "', applicanttype='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pApplicanttype) + "', fdcalculationmode='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode) + "',interestpayout='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pInterestpayuot) + "',tenure=" + Convert.ToInt32(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].ptenure) + ",tenuremode='" + ManageQuote(FdConfigDeatails.lstFDConfigartionDetailsDTO[i].ptenuremode) + "',interestamount=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pinterestamount + ",depositamount=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pdepositamount + ",maturityamount=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pmaturityamount + ",payindenomination=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pPayindenomination + ",modifiedby=" + FdConfigDeatails.pCreatedby + ",modifieddate=current_timestamp,isreferralcommissionapplicable='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pIsreferralcommissionapplicable + "',referralcommissiontype='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pReferralcommissiontype + "',commissionvalue=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pCommissionValue + ",tdsaccountid='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdsaccountid + "',tdssection='" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + "',tdspercentage=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pTdssection + ",multiplesof=" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pMultiplesofamount + ", caltype = '" + FdConfigDeatails.lstFDConfigartionDetailsDTO[i].pFDcalucationmode + "' where fdconfigid =" + FdConfigDeatails.pFdconfigid + ";");
                            }

                        }
                    }
                }
               
                if (!string.IsNullOrEmpty(Recordid))
                {
                    queryUpdate.Append("update tblmstfixeddepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FdConfigDeatails.pCreatedby + ",modifieddate=current_timestamp where fdconfigid=" + FdConfigDeatails.pFdconfigid + "  and recordid not in(" + Recordid + ");");
                }
                else
                {
                    //if (FdConfigDeatails.lstFDConfigartionDetailsDTO.Count != 0)
                    //{
                        queryUpdate.Append("update tblmstfixeddepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FdConfigDeatails.pCreatedby + ",modifieddate=current_timestamp where fdconfigid=" + FdConfigDeatails.pFdconfigid + ";");
                    //}
                }
               
                if (!string.IsNullOrEmpty(sbInsert.ToString()) || !string.IsNullOrEmpty(queryUpdate.ToString()))
                {                
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, queryUpdate.ToString() + "" + sbInsert.ToString());
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

        public async Task<FdConfigDeatails> GetFDConfigurationDetails(string FdName, string FDCode, string Connectionstring)
        {
            FdConfigDeatails FdConfigDeatails = new FdConfigDeatails();
            FdConfigDeatails.lstFDConfigartionDetailsDTO = new List<FDConfigartionDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    FdConfigDeatails.pFdname = FdName.ToUpper();
                    FdConfigDeatails.pFdnamecode = FDCode.ToUpper();
                    FdConfigDeatails.pFdconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select fdconfigid  from tblmstfixeddepositConfig where upper(fdname) = '" + ManageQuote(FdConfigDeatails.pFdname) + "' and fdnamecode='" + ManageQuote(FdConfigDeatails.pFdnamecode) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select recordid,fdconfigid, fdname, membertypeid, membertype, applicanttype, fdcalculationmode, coalesce(mindepositamount,0) as mindepositamount, coalesce(maxdepositamount,0) as maxdepositamount, coalesce(investmentperiodfrom,'') as investmentperiodfrom, coalesce(investmentperiodto,'') as investmentperiodto, coalesce(interestpayout,'') as interestpayout, coalesce(interesttype,'') as interesttype,coalesce(interestratefrom,0) as interestratefrom, coalesce(interestrateto,0) as interestrateto, coalesce(valuefor100,0) as valuefor100,coalesce(compoundinteresttype,'') as compoundinteresttype,coalesce(tenure,0) as tenure,coalesce(tenuremode,'') as tenuremode,coalesce(interestamount,0) as interestamount,coalesce(depositamount,0) as depositamount,coalesce(maturityamount,0) as maturityamount,coalesce(payindenomination,0) as payindenomination,coalesce( isreferralcommissionapplicable,false) as isreferralcommissionapplicable,referralcommissiontype,coalesce(commissionvalue,0) as commissionvalue,tdsaccountid,tdssection,coalesce(tdspercentage,0) as tdspercentage,coalesce(rate_per_square_yard,0) as rate_per_square_yard,coalesce(multiplesof,0) as multiplesof,caltype  from tblmstfixeddepositConfigdetails where  upper(fdname)='" + ManageQuote(FdConfigDeatails.pFdname) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FdConfigDeatails.lstFDConfigartionDetailsDTO.Add(new FDConfigartionDetailsDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pFdconfigid = Convert.ToInt64(dr["fdconfigid"]),
                                pMembertypeid = Convert.ToInt64(dr["membertypeid"]),
                                pMembertype = Convert.ToString(dr["membertype"]),
                                pApplicanttype = Convert.ToString(dr["applicanttype"]),
                                pFDcalucationmode = Convert.ToString(dr["fdcalculationmode"]),
                                pMininstalmentamount = Convert.ToDecimal(dr["mindepositamount"]),
                                pMaxinstalmentamount = Convert.ToDecimal(dr["maxdepositamount"]),
                                pInvestmentperiodfrom = Convert.ToString(dr["investmentperiodfrom"]),
                                pInvestmentperiodto = Convert.ToString(dr["investmentperiodto"]),
                                pInterestpayuot = Convert.ToString(dr["interestpayout"]),
                                pInteresttype = Convert.ToString(dr["interesttype"]),
                                pInterestCompunding = Convert.ToString(dr["compoundinteresttype"]),
                                pInterestratefrom = Convert.ToDecimal(dr["interestratefrom"]),
                                pInterestrateto = Convert.ToDecimal(dr["interestrateto"]),
                                pValueper100 = Convert.ToDecimal(dr["valuefor100"]),
                                ptenure = Convert.ToInt32(dr["tenure"]),
                                ptenuremode = Convert.ToString(dr["tenuremode"]),
                                pinterestamount = Convert.ToDecimal(dr["interestamount"]),
                                pdepositamount = Convert.ToDecimal(dr["depositamount"]),
                                pmaturityamount = Convert.ToDecimal(dr["maturityamount"]),
                                pPayindenomination = Convert.ToDecimal(dr["payindenomination"]),
                                pTypeofOperation = "OLD",
                                pIsreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]),
                                pReferralcommissiontype = Convert.ToString(dr["referralcommissiontype"]),
                                pCommissionValue = Convert.ToDecimal(dr["commissionvalue"]),
                                pTdsaccountid = Convert.ToString(dr["tdsaccountid"]),
                                pTdssection = Convert.ToString(dr["tdssection"]),
                                pTdspercentage = Convert.ToDecimal(dr["tdspercentage"]),
                                pRatepersquareyard=Convert.ToDecimal(dr["rate_per_square_yard"]),
                                pMultiplesofamount=Convert.ToDecimal(dr["multiplesof"]),
                                pCaltype=Convert.ToString(dr["caltype"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return FdConfigDeatails;
        }

        public bool SaveLoanFacility(FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            StringBuilder sbUpdate = new StringBuilder();


            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (FDloanfacilityDetailsDTO != null)
                {                    
                    if (string.IsNullOrEmpty(FDloanfacilityDetailsDTO.precordid.ToString()) || FDloanfacilityDetailsDTO.precordid == 0)
                    {
                        sbInsert.Append("insert into tblmstfixeddepositLoansConfig (fdconfigid,fdname,isloanfacility,loanpercentage,ageperiod,ageperiodtype,isprematuretylockingperiod,prematuretyageperiod,prematuretyageperiodtype,islatefeepayble,latefeepaybletype,latefeepayblevalue,latefeeapplicablefrom,latefeeapplicabletype,statusid,createdby,createddate)values(" + FDloanfacilityDetailsDTO.pFdconfigid + ",'" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "','" + FDloanfacilityDetailsDTO.pIsloanfacilityapplicable + "'," + FDloanfacilityDetailsDTO.pEligiblepercentage + "," + FDloanfacilityDetailsDTO.pAgeperiod + ",'" + ManageQuote(FDloanfacilityDetailsDTO.pAgeperiodtype) + "','" + FDloanfacilityDetailsDTO.pIsprematuretylockingperiod + "'," + FDloanfacilityDetailsDTO.pPrematuretyageperiod + ",'" + ManageQuote(FDloanfacilityDetailsDTO.pPrematuretyageperiodtype) + "','" + FDloanfacilityDetailsDTO.pIslatefeepayble + "','" + ManageQuote(FDloanfacilityDetailsDTO.pLatefeepaybletype) + "'," + FDloanfacilityDetailsDTO.pLatefeepayblevalue + "," + FDloanfacilityDetailsDTO.pLatefeeapplicablefrom + ",'" + ManageQuote(FDloanfacilityDetailsDTO.pLatefeeapplicabletype) + "'," + Convert.ToInt32(Status.Active) + "," + FDloanfacilityDetailsDTO.pCreatedby + ",current_timestamp);");                       
                    }
                    else
                    {
                        sbInsert.Append("update tblmstfixeddepositLoansConfig set isloanfacility='" + FDloanfacilityDetailsDTO.pIsloanfacilityapplicable + "',loanpercentage=" + FDloanfacilityDetailsDTO.pEligiblepercentage + ",ageperiod=" + FDloanfacilityDetailsDTO.pAgeperiod + ",ageperiodtype='" + ManageQuote(FDloanfacilityDetailsDTO.pAgeperiodtype) + "',isprematuretylockingperiod=" + FDloanfacilityDetailsDTO.pIsprematuretylockingperiod + ",prematuretyageperiod=" + FDloanfacilityDetailsDTO.pPrematuretyageperiod + ",prematuretyageperiodtype='" + ManageQuote(FDloanfacilityDetailsDTO.pPrematuretyageperiodtype) + "',islatefeepayble='" + FDloanfacilityDetailsDTO.pIslatefeepayble + "',latefeepaybletype='" + ManageQuote(FDloanfacilityDetailsDTO.pLatefeepaybletype) + "',latefeepayblevalue=" + FDloanfacilityDetailsDTO.pLatefeepayblevalue + ",latefeeapplicablefrom=" + FDloanfacilityDetailsDTO.pLatefeeapplicablefrom + ",latefeeapplicabletype='" + ManageQuote(FDloanfacilityDetailsDTO.pLatefeeapplicabletype) + "',modifiedby=" + FDloanfacilityDetailsDTO.pCreatedby + ",modifieddate=current_timestamp where fdconfigid=" + FDloanfacilityDetailsDTO.pFdconfigid + ";");                        
                    }
                    string Recordid =string.Empty;
                    if(FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages!=null && FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages.Count>0)
                    {
                        for(int i=0;i< FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages.Count;i++)
                        {                           
                            if (!string.IsNullOrEmpty(FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pTypeofOperation) && FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pTypeofOperation.Trim().ToUpper() != "CREATE"
                           )
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pRecordid.ToString();
                                }
                            }
                            if (FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pTypeofOperation == "CREATE")
                            {
                                sbInsert.AppendLine("insert into tblmstfixeddepositprematurityinterestpercentages(fdconfigid, fdname, minprematuritytoperiod, maxprematurityfromperiod, prematurityperiodtype,percentage, statusid, createdby, createddate) values ('" + FDloanfacilityDetailsDTO.pFdconfigid + "', '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "', " + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pminprematuritypercentage + ", " + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pmaxprematuritypercentage + ", '" + ManageQuote(FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pprematurityperiodtype) + "',"+ FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pPercentage + ", " + Convert.ToInt32(Status.Active) + ", '" + FDloanfacilityDetailsDTO.pCreatedby + "', current_timestamp);");
                            }
                            else
                            {
                                sbInsert.AppendLine("update tblmstfixeddepositprematurityinterestpercentages set minprematuritytoperiod = " + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pminprematuritypercentage + ", maxprematurityfromperiod = " + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pmaxprematuritypercentage + ", prematurityperiodtype = '" + ManageQuote(FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pprematurityperiodtype) + "',statusid = " + Convert.ToInt32(Status.Active) + ", modifiedby = '" + FDloanfacilityDetailsDTO.pCreatedby + "', modifieddate = current_timestamp,percentage=" + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pPercentage + " where fdname = '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "' and fdconfigid = " + FDloanfacilityDetailsDTO.pFdconfigid + " and recordid = " + FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages[i].pRecordid + ";");
                            }
                            if (!string.IsNullOrEmpty(Recordid))
                            {
                                sbUpdate.AppendLine("UPDATE tblmstfixeddepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FDloanfacilityDetailsDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdname = '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "' and fdconfigid = " + FDloanfacilityDetailsDTO.pFdconfigid + "; ");
                            }
                            else
                            {
                                if (FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages == null || FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages.Count == 0)
                                {
                                    sbUpdate.AppendLine("UPDATE tblmstfixeddepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FDloanfacilityDetailsDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdname = '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "' and fdconfigid = " + FDloanfacilityDetailsDTO.pFdconfigid + "; ");
                                }
                            }
                        }
                    }
                    else
                    {
                        sbUpdate.AppendLine("UPDATE tblmstfixeddepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + FDloanfacilityDetailsDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE fdname = '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "'; ");
                    }
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbUpdate.ToString() + " " + sbInsert.ToString());
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

        public async Task<FDloanfacilityDetailsDTO> GetLoanFacilityDetails(string FdName, string FDCode, string Connectionstring)
        {
            FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO = new FDloanfacilityDetailsDTO();
            DataSet ds = new DataSet();
            await Task.Run(() =>
            {
                try
                {
                    FDloanfacilityDetailsDTO.pFdname = FdName.ToUpper();
                    FDloanfacilityDetailsDTO.pFdnamecode = FDCode.ToUpper();
                    FDloanfacilityDetailsDTO.pFdconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select fdconfigid  from tblmstfixeddepositConfig where upper(fdname) = '" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "' and fdnamecode='" + ManageQuote(FDloanfacilityDetailsDTO.pFdnamecode) + "';"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select recordid,fdconfigid,fdname,isloanfacility,loanpercentage,ageperiod,ageperiodtype,isprematuretylockingperiod,prematuretyageperiod,prematuretyageperiodtype,islatefeepayble,latefeepaybletype,latefeepayblevalue,latefeeapplicablefrom,latefeeapplicabletype from tblmstfixeddepositLoansConfig where  upper(fdname)='" + ManageQuote(FDloanfacilityDetailsDTO.pFdname) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        if (dr.Read())
                        {
                            FDloanfacilityDetailsDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            FDloanfacilityDetailsDTO.pIsloanfacilityapplicable = Convert.ToBoolean(dr["isloanfacility"]);
                            FDloanfacilityDetailsDTO.pEligiblepercentage = Convert.ToDecimal(dr["loanpercentage"]);
                           // FDloanfacilityDetailsDTO.pIsloanageperiod = Convert.ToBoolean(dr["isloanageperiod"]);
                            FDloanfacilityDetailsDTO.pAgeperiod = Convert.ToDecimal(dr["ageperiod"]);
                            FDloanfacilityDetailsDTO.pAgeperiodtype = Convert.ToString(dr["ageperiodtype"]);
                            FDloanfacilityDetailsDTO.pIsprematuretylockingperiod = Convert.ToBoolean(dr["isprematuretylockingperiod"]);
                            FDloanfacilityDetailsDTO.pPrematuretyageperiod = Convert.ToDecimal(dr["prematuretyageperiod"]);
                            FDloanfacilityDetailsDTO.pPrematuretyageperiodtype = Convert.ToString(dr["prematuretyageperiodtype"]);
                            FDloanfacilityDetailsDTO.pIslatefeepayble = Convert.ToBoolean(dr["islatefeepayble"]);
                            FDloanfacilityDetailsDTO.pLatefeepaybletype = Convert.ToString(dr["latefeepaybletype"]);
                            FDloanfacilityDetailsDTO.pLatefeepayblevalue = Convert.ToDecimal(dr["latefeepayblevalue"]);
                            FDloanfacilityDetailsDTO.pLatefeeapplicablefrom = Convert.ToInt64(dr["latefeeapplicablefrom"]);
                            FDloanfacilityDetailsDTO.pLatefeeapplicabletype = Convert.ToString(dr["latefeeapplicabletype"]);
                            FDloanfacilityDetailsDTO.pTypeofOperation = "OLD";
                            FDloanfacilityDetailsDTO._FixedDepositPrematurityInterestPercentages = GetFixedDepositPrematurityInterestPercentagesData(FDloanfacilityDetailsDTO.pFdconfigid,Connectionstring);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return FDloanfacilityDetailsDTO;
        }

        public bool SaveFdReferralDeatils(FDReferralCommissionDTO FDReferralCommissionDTO, string Connectionstring)
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
                if (FDReferralCommissionDTO != null)
                {                  
                    if (string.IsNullOrEmpty(FDReferralCommissionDTO.precordid.ToString()) || FDReferralCommissionDTO.precordid == 0)
                    {
                        sbInsert.Append("insert into tblmstfixeddepositConfigreferraldetails (fdconfigid,fdname,isreferralcommissionapplicable,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection,statusid,createdby,createddate)values(" + FDReferralCommissionDTO.pFdconfigid + ",'" + ManageQuote(FDReferralCommissionDTO.pFdname) + "','" + FDReferralCommissionDTO.pIsreferralcommissionapplicable + "','" + ManageQuote(FDReferralCommissionDTO.pReferralcommissiontype) + "'," + FDReferralCommissionDTO.pCommissionValue + ",'" + FDReferralCommissionDTO.pIstdsapplicable + "','" + ManageQuote(FDReferralCommissionDTO.pTdsaccountid) + "','" + ManageQuote(FDReferralCommissionDTO.pTdssection) + "'," + Convert.ToInt32(Status.Active) + "," + FDReferralCommissionDTO.pCreatedby + ",current_timestamp);");
                    }
                    else
                    {
                        sbInsert.Append("update tblmstfixeddepositConfigreferraldetails set isreferralcommissionapplicable='" + FDReferralCommissionDTO.pIsreferralcommissionapplicable + "',referralcommissiontype='" + ManageQuote(FDReferralCommissionDTO.pReferralcommissiontype) + "',commissionValue=" + FDReferralCommissionDTO.pCommissionValue + ",istdsapplicable='" + FDReferralCommissionDTO.pIstdsapplicable + "',tdsaccountid='" + ManageQuote(FDReferralCommissionDTO.pTdsaccountid) + "',tdssection='" + ManageQuote(FDReferralCommissionDTO.pTdssection) + "',modifiedby=" + FDReferralCommissionDTO.pCreatedby + ",modifieddate=current_timestamp where fdconfigid=" + FDReferralCommissionDTO.pFdconfigid + ";");
                    }
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

        public async Task<FDReferralCommissionDTO> GetFDReferralCommission(string FdName, string FDCode, string Connectionstring)
        {
            FDReferralCommissionDTO FDReferralCommissionDTO = new FDReferralCommissionDTO();
            await Task.Run(() =>
            {
                try
                {
                    FDReferralCommissionDTO.pFdname = FdName.ToUpper();
                    FDReferralCommissionDTO.pFdnamecode = FDCode.ToUpper();
                    FDReferralCommissionDTO.pFdconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select fdconfigid  from tblmstfixeddepositConfig where upper(fdname) = '" + ManageQuote(FDReferralCommissionDTO.pFdname) + "' and fdnamecode='" + ManageQuote(FDReferralCommissionDTO.pFdnamecode) + "';"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select recordid,fdconfigid,fdname,isreferralcommissionapplicable,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection from tblmstfixeddepositConfigreferraldetails where  upper(fdname)='" + ManageQuote(FDReferralCommissionDTO.pFdname) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        if (dr.Read())
                        {
                            FDReferralCommissionDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            FDReferralCommissionDTO.pIsreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]);
                            FDReferralCommissionDTO.pReferralcommissiontype = Convert.ToString(dr["referralcommissiontype"]);
                            FDReferralCommissionDTO.pCommissionValue = Convert.ToDecimal(dr["commissionValue"]);
                            FDReferralCommissionDTO.pIstdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                            FDReferralCommissionDTO.pTdsaccountid = Convert.ToString(dr["tdsaccountid"]);
                            FDReferralCommissionDTO.pTdssection = Convert.ToString(dr["tdssection"]);
                            FDReferralCommissionDTO.pTypeofOperation = "OLD";
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return FDReferralCommissionDTO;
        }

        public bool SaveIdentificationDocumentsFD(IdentificationDocumentsDto IdentificationDocumentsDto, string Connectionstring)
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
                sbInsert.Append("delete from tblmstloanwisedocumentproofs where loantypeid="+ IdentificationDocumentsDto.pFdconfigid + " and contacttype='FD';");
                if (IdentificationDocumentsDto.identificationdocumentsList.Count > 0)
                {
                    for (int i = 0; i < IdentificationDocumentsDto.identificationdocumentsList.Count; i++)
                    {
                        if (IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList.Count > 0)
                        {
                            for (int j = 0; j < IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList.Count; j++)
                            {
                                if (IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentRequired == true || IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentMandatory == true)
                                {
                                    sbInsert.Append("insert into tblmstloanwisedocumentproofs(loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory,statusid,createdby,createddate) values(" + IdentificationDocumentsDto.pFdconfigid + "," + IdentificationDocumentsDto.pFdconfigid + ",'FD'," + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentId + "," + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentgroupId + ",'" + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentRequired + "','" + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentMandatory + "'," + Convert.ToInt32(Status.Active) + "," + IdentificationDocumentsDto.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }
                    }
                }             
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text,sbInsert.ToString());
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

        public async Task<IdentificationDocumentsDto> GetIdentificationDocumentsFD(string FdName, string FDCode, string Connectionstring)
        {
            string Query = string.Empty;
           

            IdentificationDocumentsDto IdentificationDocumentsDto = new IdentificationDocumentsDto();
            IdentificationDocumentsDto.identificationdocumentsList = new List<DocumentsMasterDTO>();
            await Task.Run(() =>
            {
                try
                {
                    IdentificationDocumentsDto.pFdname = FdName.ToUpper();
                    IdentificationDocumentsDto.pFdnamecode = FDCode.ToUpper();
                    IdentificationDocumentsDto.pFdconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select fdconfigid  from tblmstfixeddepositConfig where upper(fdname) = '" + ManageQuote(IdentificationDocumentsDto.pFdname) + "' and fdnamecode='" + ManageQuote(IdentificationDocumentsDto.pFdnamecode) + "';"));
                    using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup"))
                    {
                        if (IdentificationDocumentsDto.pFdconfigid > 0)
                        {

                            Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1 and documentid not in(select documentid from tblmstloanwisedocumentproofs where statusid=1 and  loanid=" + IdentificationDocumentsDto.pFdconfigid + " and contacttype ='FD') union select y.contacttype,x.documentid,x.documentgroupid,x.documentgroupname,x.documentname,y.isdocumentmandatory as mandatory,y.isdocumentrequired required from tblmstdocumentproofs x right join tblmstloanwisedocumentproofs y on x.documentid = y.documentid where y.statusid = 1 and y.loanid = " + IdentificationDocumentsDto.pFdconfigid + " and contacttype ='FD';";
                        }
                        else
                        {
                            Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required,ts.statusname from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = 1;";
                        }
                        while (dr1.Read())
                        {
                            DocumentsMasterDTO objdocumentidproofs = new DocumentsMasterDTO();
                            objdocumentidproofs.pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]);
                            objdocumentidproofs.pDocumentGroup = Convert.ToString(dr1["documentgroupname"]);
                            objdocumentidproofs.pDocumentsList = new List<pIdentificationDocumentsDTO>();
                            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                            {
                                if (IdentificationDocumentsDto.pFdconfigid > 0)
                                {
                                    while (dr.Read())
                                    {

                                        if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
                                        {
                                            objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO
                                            {
                                                pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]),
                                                pDocumentGroup = Convert.ToString(dr1["documentgroupname"]),
                                                pContactType = Convert.ToString(dr["contacttype"]),
                                                pDocumentId = Convert.ToInt64(dr["documentid"]),
                                                pDocumentName = dr["documentname"].ToString(),
                                                pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                                pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                                pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                                pLoanId = IdentificationDocumentsDto.pFdconfigid,
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
                                                pDocumentGroup = Convert.ToString(dr1["documentgroupname"]),
                                                pContactType = Convert.ToString(dr["contacttype"]),
                                                pDocumentId = Convert.ToInt64(dr["documentid"]),
                                                pDocumentName = Convert.ToString(dr["documentname"]),
                                                pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                                pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                                pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                                pStatusname = dr["statusname"].ToString(),
                                                pLoanId = IdentificationDocumentsDto.pFdconfigid,
                                                // pLoantypeId = pLoanId,
                                            });
                                        }
                                    }
                                }
                            }
                            IdentificationDocumentsDto.identificationdocumentsList.Add(objdocumentidproofs);
                        }
                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return IdentificationDocumentsDto;
        }

        public bool DeleteFdConfiguration(long FdConfigId, string Connectionstring)
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
                if(!string.IsNullOrEmpty(FdConfigId.ToString()) && FdConfigId!=0)
                {
                    sbInsert.Append("update tblmstfixeddepositConfig set statusid=" + Convert.ToInt32(Status.Inactive) + " where fdconfigid=" + FdConfigId + ";");
                    sbInsert.Append("update tblmstfixeddepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where fdconfigid=" + FdConfigId + ";");
                    sbInsert.Append("update tblmstfixeddepositLoansConfig set statusid=" + Convert.ToInt32(Status.Inactive) + " where fdconfigid=" + FdConfigId + ";");
                   // sbInsert.Append("update tblmstfixeddepositConfigreferraldetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where fdconfigid=" + FdConfigId + ";");
                    sbInsert.Append("update tblmstloanwisedocumentproofs set statusid="+Convert.ToInt32(Status.Inactive)+" where loantypeid="+ FdConfigId + " and contacttype='FD';");
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

        public FdschemeandcodeCount GetFDNameCount(Int64 FDConfigid, string FDName, string FdnameCode,string ConnectionString)
        {            
            FdschemeandcodeCount _FdschemeandcodeCount = new FdschemeandcodeCount();
            try
            {
                if(FDConfigid == 0)
                {
                    _FdschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstfixeddepositConfig where upper(fdname)='" + ManageQuote(FDName).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _FdschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstfixeddepositConfig where upper(fdcode)='" + ManageQuote(FdnameCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _FdschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstfixeddepositConfig where upper(fdname)='" + ManageQuote(FDName).ToUpper() + "' and fdconfigid <> "+FDConfigid+" and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _FdschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstfixeddepositConfig where upper(fdcode)='" + ManageQuote(FdnameCode).ToUpper() + "' and fdconfigid <> " + FDConfigid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FdschemeandcodeCount;
        }

        public List<FixedDepositPrematurityInterestPercentages> GetFixedDepositPrematurityInterestPercentagesData(long FdConfigId,string ConnectionString)
        {          
            var _FixedDepositPrematurityInterestPercentagesList = new List<FixedDepositPrematurityInterestPercentages>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, coalesce(minprematuritytoperiod,0) as minprematuritytoperiod,coalesce( maxprematurityfromperiod,0) as maxprematurityfromperiod,  prematurityperiodtype,coalesce(percentage,0) as percentage  FROM tblmstfixeddepositprematurityinterestpercentages where  fdconfigid = '" + FdConfigId+ "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        FixedDepositPrematurityInterestPercentages _FdMaturityRates = new FixedDepositPrematurityInterestPercentages
                        {
                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            pminprematuritypercentage = Convert.ToDecimal(dr["minprematuritytoperiod"]),
                            pmaxprematuritypercentage = Convert.ToDecimal(dr["maxprematurityfromperiod"]),
                            pprematurityperiodtype = Convert.ToString(dr["prematurityperiodtype"]),
                            pTypeofOperation = "OLD",
                            pPercentage = Convert.ToDecimal(dr["percentage"])
                        };
                        _FixedDepositPrematurityInterestPercentagesList.Add(_FdMaturityRates);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }           
            return _FixedDepositPrematurityInterestPercentagesList;


        }
    }
}
