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

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class RdConfigarationDAL : SettingsDAL, IRdConfigaration
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region Get RDName Count
        public int GetRdNameCount(string RdName, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositConfig where upper(rdname)='" + ManageQuote(RdName.ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        #endregion

        #region Save's Recurring Deposit Configaration
        public bool SaverdNameAndCode(RdNameAndCodeDTO Rdnameandcode, string Connectionstring)
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
                if (string.IsNullOrEmpty(Rdnameandcode.pRdconfigid.ToString()) || Rdnameandcode.pRdconfigid == 0)
                {
                    sbInsert.Append("insert into tblmstrecurringdepositConfig (rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid,createdby,createddate)values('" + ManageQuote(Rdnameandcode.pRdname) + "','" + ManageQuote(Rdnameandcode.pRdcode) + "','" + ManageQuote(Rdnameandcode.pCompanycode) + "','" + ManageQuote(Rdnameandcode.pBranchcode) + "','" + ManageQuote(Rdnameandcode.pSeries) + "'," + Rdnameandcode.pSerieslength + ",'" + ManageQuote(Rdnameandcode.pRdnamecode) + "'," + Convert.ToInt32(Status.Active) + "," + Rdnameandcode.pCreatedby + ",current_timestamp);");

                }
                else
                {
                    sbInsert.Append("update tblmstrecurringdepositConfig set rdname='" + ManageQuote(Rdnameandcode.pRdname) + "', rdcode='" + ManageQuote(Rdnameandcode.pRdcode) + "', companycode='" + ManageQuote(Rdnameandcode.pCompanycode) + "', branchcode='" + ManageQuote(Rdnameandcode.pBranchcode) + "',series='" + ManageQuote(Rdnameandcode.pSeries) + "', serieslength=" + Rdnameandcode.pSerieslength + ", rdnamecode='" + ManageQuote(Rdnameandcode.pRdnamecode) + "' where rdconfigid=" + Rdnameandcode.pRdconfigid + ";");
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
        public bool Saverdconfigarationdetails(RdConfigartionDetails Rdconfiglist, string Connectionstring)
        {

            string Recordid = string.Empty;
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            StringBuilder queryUpdate = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (Rdconfiglist.lstRdConfigartionDetails != null)
                {
                    for (int i = 0; i < Rdconfiglist.lstRdConfigartionDetails.Count; i++)
                    {
                        if (Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode.Trim().ToUpper() != "TABLE")
                        {
                            if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
                                }
                            }

                            if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                            {
                                //sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode, mininstallmentamount, maxinstallmentamount, installmentpayin, investmentperiodfrom, investmentperiodto, interestpayout,interesttype,compoundinteresttype,interestratefrom, interestrateto, valueper100,tenure,tenuremode,Payindenomination,interestamount,depositamount,maturityamount,statusid, createdby, createddate)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + "," + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + "," + Rdconfiglist.lstRdConfigartionDetails[i].pTenure + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pTenuremode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp);");

                                sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode, mininstallmentamount, maxinstallmentamount, installmentpayin, investmentperiodfrom, investmentperiodto, interestpayout,interesttype,compoundinteresttype,isreferralcommissionapplicable,referralcommissiontype,commissionvalue,tdsaccountid,tdssection,tdspercentage,interestratefrom, interestrateto, valueper100,Payindenomination,interestamount,depositamount,maturityamount,multiplesof, statusid, createdby, createddate,caltype)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pIsreferralcommissionapplicable + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pReferralcommissiontype + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pCommissionValue + ",'" + Rdconfiglist.lstRdConfigartionDetails[i].pTdsaccountid + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pTdssection + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pTdspercentage + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + "," + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + "," + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMultiplesofamount + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp, '" + Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode + "');");
                            }
                            else if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                            {
                                sbInsert.Append("update tblmstrecurringdepositConfigdetails set membertypeid=" + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + "membertype ='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "', applicanttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "', rdcalculationmode='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "', mininstallmentamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + ", maxinstallmentamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ", installmentpayin='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "', investmentperiodfrom'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "', investmentperiodto='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "', interestpayout='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "', interesttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "',compoundinteresttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "',isreferralcommissionapplicable = '" + Rdconfiglist.lstRdConfigartionDetails[i].pIsreferralcommissionapplicable + "', referralcommissiontype = '" + Rdconfiglist.lstRdConfigartionDetails[i].pReferralcommissiontype + "', commissionvalue = " + Rdconfiglist.lstRdConfigartionDetails[i].pCommissionValue + ", tdsaccountid = '" + Rdconfiglist.lstRdConfigartionDetails[i].pTdsaccountid + "', tdssection = '" + Rdconfiglist.lstRdConfigartionDetails[i].pTdssection + "', tdspercentage = " + Rdconfiglist.lstRdConfigartionDetails[i].pTdspercentage + ", interestratefrom=" + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + ", interestrateto" + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + ", valueper100=" + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + ",Payindenomination=" + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + ",interestamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + ",depositamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + ",maturityamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + ",multiplesof = " + Rdconfiglist.lstRdConfigartionDetails[i].pMultiplesofamount + ", modifiedby =" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp, caltype = '" + Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode + "' where rdconfigid =" + Rdconfiglist.pRdconfigid + " and recordid = "+Rdconfiglist.lstRdConfigartionDetails[i].precordid+"; ");
                            }
                        }
                        else
                        {
                            if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
                                }
                            }
                            if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
                            {
                                //sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode, mininstallmentamount, maxinstallmentamount, installmentpayin, investmentperiodfrom, investmentperiodto, interestpayout,interesttype,compoundinteresttype,interestratefrom, interestrateto, valueper100,tenure,tenuremode,Payindenomination,interestamount,depositamount,maturityamount,statusid, createdby, createddate)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + "," + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + "," + Rdconfiglist.lstRdConfigartionDetails[i].pTenure + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pTenuremode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp);");

                                sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode,tenure,tenuremode, interestpayout, isreferralcommissionapplicable,referralcommissiontype,commissionvalue,tdsaccountid,tdssection,tdspercentage,multiplesof, Payindenomination,interestamount,depositamount,maturityamount,statusid, createdby, createddate, caltype)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "',"+ Rdconfiglist.lstRdConfigartionDetails[i].pTenure+ ", '" + Rdconfiglist.lstRdConfigartionDetails[i].pTenuremode + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pIsreferralcommissionapplicable + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pReferralcommissiontype + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pCommissionValue + ",'" + Rdconfiglist.lstRdConfigartionDetails[i].pTdsaccountid + "','" + Rdconfiglist.lstRdConfigartionDetails[i].pTdssection + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pTdspercentage + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMultiplesofamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp, '" + Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode + "');");
                            }

                            else if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
                            {
                                sbInsert.Append("update tblmstrecurringdepositConfigdetails set rdname = '" + ManageQuote(Rdconfiglist.pRdname) + "', membertypeid = " + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ", membertype = '" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "',applicanttype = '" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "', rdcalculationmode = '" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "', tenure = " + Rdconfiglist.lstRdConfigartionDetails[i].pTenure + ", tenuremode = '" + Rdconfiglist.lstRdConfigartionDetails[i].pTenuremode + "', interestpayout = '" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "', isreferralcommissionapplicable = '" + Rdconfiglist.lstRdConfigartionDetails[i].pIsreferralcommissionapplicable + "', referralcommissiontype = '" + Rdconfiglist.lstRdConfigartionDetails[i].pReferralcommissiontype + "', commissionvalue = " + Rdconfiglist.lstRdConfigartionDetails[i].pCommissionValue + ", tdsaccountid = '" + Rdconfiglist.lstRdConfigartionDetails[i].pTdsaccountid + "', tdssection = '" + Rdconfiglist.lstRdConfigartionDetails[i].pTdssection + "', tdspercentage = " + Rdconfiglist.lstRdConfigartionDetails[i].pTdspercentage + ", multiplesof = " + Rdconfiglist.lstRdConfigartionDetails[i].pMultiplesofamount + ", Payindenomination = " + Rdconfiglist.lstRdConfigartionDetails[i].pPayindenomination + ", interestamount = " + Rdconfiglist.lstRdConfigartionDetails[i].pInterestamount + ", depositamount = " + Rdconfiglist.lstRdConfigartionDetails[i].pDepositamount + ", maturityamount = " + Rdconfiglist.lstRdConfigartionDetails[i].pMaturityamount + " modifiedby =" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp, caltype = '" + Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode + "' where rdconfigid =" + Rdconfiglist.pRdconfigid + " and recordid = " + Rdconfiglist.lstRdConfigartionDetails[i].precordid + "; ");
                            }
                        }

                    }
                }

                if (!string.IsNullOrEmpty(Recordid))
                {
                    queryUpdate.Append("update tblmstrecurringdepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where rdconfigid=" + Rdconfiglist.pRdconfigid + "  and recordid not in(" + Recordid + ");");
                }
                else
                {
                    //if (Rdconfiglist.lstRdConfigartionDetails.Count == 0)
                    //{
                        queryUpdate.Append("update tblmstrecurringdepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where rdconfigid=" + Rdconfiglist.pRdconfigid + ";");
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
        public bool SaveRdloanfacilityDetails(RdloanfacilityDetailsDTO RdloanDetails, string Connectionstring)
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

                if (string.IsNullOrEmpty(RdloanDetails.precordid.ToString()) || RdloanDetails.precordid == 0)
                {
                    sbInsert.Append("insert into tblmstrecurringdepositLoansConfig (rdconfigid,rdname,isloanfacilityapplicable,eligiblepercentage,isloanageperiod,ageperiod,ageperiodtype,isprematuretylockingperiod,prematuretyageperiod,prematuretyageperiodtype,islatefeepayble,latefeepaybletype,latefeepayblevalue,latefeeapplicablefrom,latefeeapplicabletype,statusid,createdby,createddate)values(" + RdloanDetails.pRdconfigid + ",'" + ManageQuote(RdloanDetails.pRdname) + "','" + RdloanDetails.pIsloanfacilityapplicable + "'," + RdloanDetails.pEligiblepercentage + ",'" + RdloanDetails.pIsloanageperiod + "'," + RdloanDetails.pAgeperiod + ",'" + ManageQuote(RdloanDetails.pAgeperiodtype) + "','" + RdloanDetails.pIsprematuretylockingperiod + "'," + RdloanDetails.pPrematuretyageperiod + ",'" + ManageQuote(RdloanDetails.pPrematuretyageperiodtype) + "','" + RdloanDetails.pIslatefeepayble + "','" + ManageQuote(RdloanDetails.pLatefeepaybletype) + "'," + RdloanDetails.pLatefeepayblevalue + "," + RdloanDetails.pLatefeeapplicablefrom + ",'" + ManageQuote(RdloanDetails.pLatefeeapplicabletype) + "'," + Convert.ToInt32(Status.Active) + "," + RdloanDetails.pCreatedby + ",current_timestamp);");

                    // sbInsert.Append("insert into tblmstrecurringdepositLoansConfig (rdconfigid,rdname,isloanfacilityapplicable,eligiblepercentage,isloanageperiod,ageperiod,ageperiodtype,isprematuretylockingperiod,prematuretyageperiod,prematuretyageperiodtype,islatefeepayble,latefeepaybletype,latefeepayblevalue,latefeeapplicablefrom,latefeeapplicabletype,statusid,createdby,createddate)values(" + RdloanDetails.pRdconfigid + ",'" + ManageQuote(RdloanDetails.pRdname) + "','" + RdloanDetails.pIsloanfacilityapplicable + "'," + RdloanDetails.pEligiblepercentage + ",'" + RdloanDetails.pIsloanageperiod + "'," + RdloanDetails.pAgeperiod + ",'" + ManageQuote(RdloanDetails.pAgeperiodtype) + "','" + RdloanDetails.pIsprematuretylockingperiod + "'," + RdloanDetails.pPrematuretyageperiod + ",'" + ManageQuote(RdloanDetails.pPrematuretyageperiodtype) + "','" + RdloanDetails.pIslatefeepayble + "','" + ManageQuote(RdloanDetails.pLatefeeapplicabletype) + "'," + RdloanDetails.pLatefeepayblevalue + "," + RdloanDetails.pLatefeeapplicablefrom + ",'" + ManageQuote(RdloanDetails.pLatefeepaybletype) + "'," + Convert.ToInt32(Status.Active) + "," + RdloanDetails.pCreatedby + ",current_timestamp);");
                }
                else
                {
                    sbInsert.Append("update tblmstrecurringdepositLoansConfig set isloanfacilityapplicable='" + RdloanDetails.pIsloanfacilityapplicable + "',eligiblepercentage=" + RdloanDetails.pEligiblepercentage + ",isloanageperiod='" + RdloanDetails.pIsloanageperiod + "',ageperiod=" + RdloanDetails.pAgeperiod + ",ageperiodtype='" + ManageQuote(RdloanDetails.pAgeperiodtype) + "',isprematuretylockingperiod=" + RdloanDetails.pIsprematuretylockingperiod + ",prematuretyageperiod=" + RdloanDetails.pPrematuretyageperiod + ",prematuretyageperiodtype='" + ManageQuote(RdloanDetails.pPrematuretyageperiodtype) + "',islatefeepayble='" + RdloanDetails.pIslatefeepayble + "',latefeepaybletype='" + ManageQuote(RdloanDetails.pLatefeepaybletype) + "',latefeepayblevalue=" + RdloanDetails.pLatefeepayblevalue + ",latefeeapplicablefrom=" + RdloanDetails.pLatefeeapplicablefrom + ",latefeeapplicabletype='" + ManageQuote(RdloanDetails.pLatefeeapplicabletype) + "',modifiedby=" + RdloanDetails.pCreatedby + ",modifieddate=current_timestamp WHERE rdconfigid=" + RdloanDetails.pRdconfigid + ";");
                    //sbInsert.Append("update tblmstrecurringdepositLoansConfig set isloanfacilityapplicable='" + RdloanDetails.pIsloanfacilityapplicable + "',eligiblepercentage=" + RdloanDetails.pEligiblepercentage + ",isloanageperiod='" + RdloanDetails.pIsloanageperiod + "',ageperiod=" + RdloanDetails.pAgeperiod + ",ageperiodtype='" + ManageQuote(RdloanDetails.pAgeperiodtype) + "',isprematuretylockingperiod=" + RdloanDetails.pPrematuretyageperiod + ",prematuretyageperiod=" + RdloanDetails.pPrematuretyageperiod + ",prematuretyageperiodtype='" + ManageQuote(RdloanDetails.pPrematuretyageperiodtype) + "',islatefeepayble='" + RdloanDetails.pIslatefeepayble + "',latefeepaybletype='" + ManageQuote(RdloanDetails.pLatefeeapplicabletype) + "',latefeepayblevalue=" + RdloanDetails.pLatefeepayblevalue + ",latefeeapplicablefrom=" + RdloanDetails.pLatefeeapplicablefrom + ",latefeeapplicabletype='" + ManageQuote(RdloanDetails.pLatefeepaybletype) + "',modifiedby=" + RdloanDetails.pCreatedby + ",modifieddate=current_timestamp WHERE rdconfigid="+ RdloanDetails.pRdconfigid + ";");

                }
                string Recordid = string.Empty;
                if (RdloanDetails._RecurringDepositPrematurityInterestPercentages != null && RdloanDetails._RecurringDepositPrematurityInterestPercentages.Count > 0)
                {
                    for (int i = 0; i < RdloanDetails._RecurringDepositPrematurityInterestPercentages.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pTypeofOperation) && RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pTypeofOperation.Trim().ToUpper() != "CREATE"
                       )
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pRecordid.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pRecordid.ToString();
                            }
                        }
                        if (RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pTypeofOperation == "CREATE")
                        {
                            sbInsert.AppendLine("insert into tblmstrecurringdepositprematurityinterestpercentages(rdconfigid, rdname, minprematuritytoperiod, maxprematurityfromperiod, prematurityperiodtype,percentage, statusid, createdby, createddate) values ('" + RdloanDetails.pRdconfigid + "', '" + ManageQuote(RdloanDetails.pRdname) + "', " + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pminprematuritypercentage + ", " + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pmaxprematuritypercentage + ", '" + ManageQuote(RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pprematurityperiodtype) + "'," + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pPercentage + ", " + Convert.ToInt32(Status.Active) + ", '" + RdloanDetails.pCreatedby + "', current_timestamp);");
                        }
                        else
                        {
                            sbInsert.AppendLine("update tblmstrecurringdepositprematurityinterestpercentages set minprematuritytoperiod = " + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pminprematuritypercentage + ", maxprematurityfromperiod = " + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pmaxprematuritypercentage + ", prematurityperiodtype = '" + ManageQuote(RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pprematurityperiodtype) + "',statusid = " + Convert.ToInt32(Status.Active) + ", modifiedby = '" + RdloanDetails.pCreatedby + "', modifieddate = current_timestamp,percentage=" + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pPercentage + " where rdname = '" + ManageQuote(RdloanDetails.pRdname) + "' and rdconfigid = " + RdloanDetails.pRdconfigid + " and recordid = " + RdloanDetails._RecurringDepositPrematurityInterestPercentages[i].pRecordid + ";");
                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbUpdate.AppendLine("UPDATE tblmstrecurringdepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdloanDetails.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdname = '" + ManageQuote(RdloanDetails.pRdname) + "' and rdconfigid = " + RdloanDetails.pRdconfigid + "; ");
                        }
                        else
                        {
                            if (RdloanDetails._RecurringDepositPrematurityInterestPercentages == null || RdloanDetails._RecurringDepositPrematurityInterestPercentages.Count == 0)
                            {
                                sbUpdate.AppendLine("UPDATE tblmstrecurringdepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdloanDetails.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdname = '" + ManageQuote(RdloanDetails.pRdname) + "'; ");
                            }
                        }
                    }
                }
                else
                {
                    sbUpdate.AppendLine("UPDATE tblmstrecurringdepositprematurityinterestpercentages SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + RdloanDetails.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE rdname = '" + ManageQuote(RdloanDetails.pRdname) + "'; ");
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
        public bool SaveRdReferralDetails(RdReferralCommissionDTO RdReferralCommission, string Connectionstring)
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

                if (string.IsNullOrEmpty(RdReferralCommission.precordid.ToString()) || RdReferralCommission.precordid == 0)
                {
                    sbInsert.Append("insert into tblmstrecurringdepositConfigreferraldetails (rdconfigid,rdname,isreferralcommissionapplicable,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection,statusid,createdby,createddate)values(" + RdReferralCommission.pRdconfigid + ",'" + ManageQuote(RdReferralCommission.pRdname) + "','" + RdReferralCommission.pIsreferralcommissionapplicable + "','" + ManageQuote(RdReferralCommission.pReferralcommissiontype) + "'," + RdReferralCommission.pCommissionValue + ",'" + RdReferralCommission.pIstdsapplicable + "','" + ManageQuote(RdReferralCommission.pTdsaccountid) + "','" + ManageQuote(RdReferralCommission.pTdssection) + "'," + Convert.ToInt32(Status.Active) + "," + RdReferralCommission.pCreatedby + ",current_timestamp);");
                }
                else
                {
                    sbInsert.Append("update tblmstrecurringdepositConfigreferraldetails set isreferralcommissionapplicable='" + RdReferralCommission.pIsreferralcommissionapplicable + "',referralcommissiontype='" + ManageQuote(RdReferralCommission.pReferralcommissiontype) + "',commissionValue=" + RdReferralCommission.pCommissionValue + ",istdsapplicable='" + RdReferralCommission.pIstdsapplicable + "',tdsaccountid='" + ManageQuote(RdReferralCommission.pTdsaccountid) + "',tdssection='" + ManageQuote(RdReferralCommission.pTdssection) + "',modifiedby=" + RdReferralCommission.pCreatedby + ",modifieddate=current_timestamp WHERE rdconfigid=" + RdReferralCommission.pRdconfigid + ";");

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

        #region Get Rd View Details
        public List<RdViewDTO> GetRdViewDetails(string ConnectionString)
        {
            List<RdViewDTO> lstRdview = new List<RdViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where  statusid = " + Convert.ToInt32(Status.Active) + "  order by rdname;"))
                {
                    while (dr.Read())
                    {
                        RdViewDTO objRdview = new RdViewDTO();
                        objRdview.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        objRdview.pRdname = dr["rdname"].ToString();
                        objRdview.pRdnamecode = dr["rdnamecode"].ToString();
                        if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Active))
                        {
                            objRdview.pstatus = true;
                        }
                        if (Convert.ToInt32(dr["statusid"]) == Convert.ToInt32(Status.Inactive))
                        {
                            objRdview.pstatus = false;
                        }
                        lstRdview.Add(objRdview);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstRdview;
        }
        #endregion

        #region Get Rd Name And Code Details
        public RdNameAndCodeDTO GetRdNameAndCodeDetails(string RdName, string RdNameCode, string ConnectionString)
        {
            RdNameAndCodeDTO RdNameAndCodeDTO = new RdNameAndCodeDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where upper(rdname) = '" + ManageQuote(RdName.ToUpper()) + "' and upper(rdnamecode) ='" + ManageQuote(RdNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdNameAndCodeDTO.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        RdNameAndCodeDTO.pRdname = dr["rdname"].ToString();
                        RdNameAndCodeDTO.pRdcode = dr["rdcode"].ToString();
                        RdNameAndCodeDTO.pCompanycode = dr["companycode"].ToString();
                        RdNameAndCodeDTO.pBranchcode = dr["branchcode"].ToString();
                        RdNameAndCodeDTO.pSeries = dr["series"].ToString();
                        RdNameAndCodeDTO.pSerieslength = Convert.ToInt64(dr["serieslength"]);
                        RdNameAndCodeDTO.pRdnamecode = dr["rdnamecode"].ToString();
                        RdNameAndCodeDTO.ptypeofoperation = "OLD";
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RdNameAndCodeDTO;
        }
        #endregion

        #region Get Rd Configuration Details
        public RdConfigartionDetails GetRdConfigurationDetails(string RdName, string RdNameCode, string ConnectionString)
        {
            RdConfigartionDetails RdConfigDetails = new RdConfigartionDetails();
            RdConfigDetails.lstRdConfigartionDetails = new List<RdConfigartionDetailsDTO>();
            //   RdConfigDetails.lstDepositCalculationTables = new List<DepositCalculationTablesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where upper(rdname) = '" + ManageQuote(RdName.ToUpper()) + "' and upper(rdnamecode) ='" + ManageQuote(RdNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdConfigDetails.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        RdConfigDetails.pRdname = dr["rdname"].ToString();
                        RdConfigDetails.pRdnamecode = dr["rdnamecode"].ToString();
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,rdconfigid,rdname,membertypeid,membertype,applicanttype,rdcalculationmode,coalesce(mininstallmentamount,0) as mininstallmentamount,coalesce(maxinstallmentamount,0) as maxinstallmentamount,coalesce(installmentpayin,'') as installmentpayin,coalesce(investmentperiodfrom,'') as investmentperiodfrom,coalesce(investmentperiodto,'') as investmentperiodto,coalesce(interestpayout,'') as interestpayout,coalesce(interesttype,'') as interesttype,coalesce(compoundinteresttype,'') as compoundinteresttype,coalesce(interestratefrom,0) as interestratefrom,coalesce(interestrateto,0) as interestrateto,coalesce(valueper100,0) as valueper100,statusid,tenure,tenuremode,Payindenomination,interestamount,depositamount,maturityamount, isreferralcommissionapplicable, referralcommissiontype, commissionvalue,coalesce(istdsapplicable,false) as istdsapplicable, tdsaccountid, tdssection, tdspercentage, multiplesof from tblmstrecurringdepositconfigdetails where rdconfigid=" + RdConfigDetails.pRdconfigid + " and upper(rdname)='" + ManageQuote(RdName.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdConfigDetails.lstRdConfigartionDetails.Add(new RdConfigartionDetailsDTO
                        {
                            precordid = dr["recordid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["recordid"]),
                            pMembertypeid = dr["membertypeid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["membertypeid"]),
                            pMembertype = Convert.ToString(dr["membertype"]),
                            pApplicanttype = Convert.ToString(dr["applicanttype"]),
                            pRdcalucationmode = Convert.ToString(dr["rdcalculationmode"]),
                            pMininstalmentamount = dr["mininstallmentamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["mininstallmentamount"]),
                            pMaxinstalmentamount = dr["maxinstallmentamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["maxinstallmentamount"]),
                            pInvestmentperiodfrom = Convert.ToString(dr["investmentperiodfrom"]),
                            pInvestmentperiodto = Convert.ToString(dr["investmentperiodto"]),
                            pInterestpayuot = Convert.ToString(dr["interestpayout"]),
                            pInteresttype = Convert.ToString(dr["interesttype"]),
                            pCompoundInteresttype = Convert.ToString(dr["compoundinteresttype"]),
                            pInterestratefrom = dr["interestratefrom"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["interestratefrom"]),
                            pInterestrateto = dr["interestrateto"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["interestrateto"]),
                            pValueper100 = dr["valueper100"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["valueper100"]),
                            pTenure = dr["tenure"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["tenure"]),
                            pTenuremode = Convert.ToString(dr["tenuremode"]),
                            pPayindenomination = dr["Payindenomination"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Payindenomination"]),
                            pInterestamount = dr["interestamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["interestamount"]),
                            pDepositamount = dr["depositamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["depositamount"]),
                            pMaturityamount = dr["maturityamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["maturityamount"]),
                            pMultiplesofamount = dr["multiplesof"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["multiplesof"]),
                            pIsreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]),
                            pReferralcommissiontype = Convert.ToString(dr["referralcommissiontype"]),
                            pCommissionValue = dr["commissionvalue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["commissionvalue"]),
                            pIstdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]),
                            pTdsaccountid = Convert.ToString(dr["tdsaccountid"]),
                            pTdssection = Convert.ToString(dr["tdssection"]),
                            pTdspercentage = dr["tdspercentage"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["tdspercentage"]),

                            pTypeofOperation = "OLD",
                        });
                    }
                }
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,deposittype,depositconfigid,depositname,membertypeid,membertype,applicanttype,tenure,tenuremode,Payindenomination,interestcaltype,interestamount,depositamount,maturityamount from tblmstdepositcalculationtable where depositconfigid=" + RdConfigDetails.pRdconfigid + " and upper(depositname)='" + ManageQuote(RdConfigDetails.pRdname.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                //{
                //    while (dr.Read())
                //    {
                //        RdConfigDetails.lstDepositCalculationTables.Add(new DepositCalculationTablesDTO
                //        {
                //            precordid = Convert.ToInt64(dr["recordid"]),
                //            pMembertypeid = Convert.ToInt64(dr["membertypeid"]),
                //            pMembertype = Convert.ToString(dr["membertype"]),
                //            pApplicanttype = Convert.ToString(dr["applicanttype"]),
                //            pTenure = Convert.ToDecimal(dr["tenure"]),
                //            pTenuremode = Convert.ToString(dr["tenuremode"]),
                //            pPayindenomination = Convert.ToDecimal(dr["Payindenomination"]),
                //            pInterestcaltype = Convert.ToString(dr["interestcaltype"]),
                //            pInterestamount = Convert.ToDecimal(dr["interestamount"]),
                //            pDepositamount = Convert.ToDecimal(dr["depositamount"]),
                //            pMaturityamount = Convert.ToDecimal(dr["maturityamount"]),
                //            pTypeofOperation = "OLD"
                //        });
                //    }
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RdConfigDetails;
        }
        #endregion

        #region Get Rd Loan Facilty Details
        public RdloanfacilityDetailsDTO GetRdloanfacilityDetails(string RdName, string RdNameCode, string ConnectionString)
        {
            RdloanfacilityDetailsDTO RdLoanFacilty = new RdloanfacilityDetailsDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where upper(rdname) = '" + ManageQuote(RdName.ToUpper()) + "' and upper(rdnamecode) ='" + ManageQuote(RdNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdLoanFacilty.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        RdLoanFacilty.pRdname = dr["rdname"].ToString();
                        RdLoanFacilty.pRdnamecode = dr["rdnamecode"].ToString();
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,rdconfigid,rdname,isloanfacilityapplicable,eligiblepercentage,isloanageperiod,ageperiod,ageperiodtype,isprematuretylockingperiod,prematuretyageperiod,prematuretyageperiodtype,islatefeepayble,latefeepaybletype,latefeepayblevalue,latefeeapplicablefrom,latefeeapplicabletype,statusid,createdby,createddate from tblmstrecurringdepositLoansConfig where rdconfigid=" + RdLoanFacilty.pRdconfigid + " and upper(rdname)='" + ManageQuote(RdLoanFacilty.pRdname.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdLoanFacilty.precordid = Convert.ToInt64(dr["recordid"]);
                        RdLoanFacilty.pIsloanfacilityapplicable = Convert.ToBoolean(dr["isloanfacilityapplicable"]);
                        RdLoanFacilty.pEligiblepercentage = Convert.ToDecimal(dr["eligiblepercentage"]);
                        RdLoanFacilty.pIsloanageperiod = Convert.ToBoolean(dr["isloanageperiod"]);
                        RdLoanFacilty.pAgeperiod = Convert.ToDecimal(dr["ageperiod"]);
                        RdLoanFacilty.pAgeperiodtype = dr["ageperiodtype"].ToString();
                        RdLoanFacilty.pIsprematuretylockingperiod = Convert.ToBoolean(dr["isprematuretylockingperiod"]);
                        RdLoanFacilty.pPrematuretyageperiod = Convert.ToDecimal(dr["prematuretyageperiod"]);
                        RdLoanFacilty.pPrematuretyageperiodtype = dr["prematuretyageperiodtype"].ToString();
                        RdLoanFacilty.pIslatefeepayble = Convert.ToBoolean(dr["islatefeepayble"]);
                        RdLoanFacilty.pLatefeepaybletype = dr["latefeepaybletype"].ToString();
                        RdLoanFacilty.pLatefeepayblevalue = Convert.ToDecimal(dr["latefeepayblevalue"]);
                        RdLoanFacilty.pLatefeeapplicablefrom = Convert.ToInt64(dr["latefeeapplicablefrom"]);
                        RdLoanFacilty.pLatefeeapplicabletype = dr["latefeeapplicabletype"].ToString();
                        RdLoanFacilty.pTypeofOperation = "OLD";
                        RdLoanFacilty._RecurringDepositPrematurityInterestPercentages = GetRecurringDepositPrematurityInterestPercentagesData(RdLoanFacilty.pRdconfigid, ConnectionString);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RdLoanFacilty;
        }
        #endregion

        #region Get Rd Referral Details
        public RdReferralCommissionDTO GetRdReferralDetails(string RdName, string RdNameCode, string ConnectionString)
        {
            RdReferralCommissionDTO RdReferralDetails = new RdReferralCommissionDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where upper(rdname) = '" + ManageQuote(RdName.ToUpper()) + "' and upper(rdnamecode) ='" + ManageQuote(RdNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdReferralDetails.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        RdReferralDetails.pRdname = dr["rdname"].ToString();
                        RdReferralDetails.pRdnamecode = dr["rdnamecode"].ToString();
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  recordid,rdconfigid,rdname,isreferralcommissionapplicable,referralcommissiontype,commissionValue,istdsapplicable,tdsaccountid,tdssection,statusid,createdby,createddate from tblmstrecurringdepositConfigreferraldetails where rdconfigid=" + RdReferralDetails.pRdconfigid + " and upper(rdname)='" + ManageQuote(RdReferralDetails.pRdname.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RdReferralDetails.precordid = Convert.ToInt64(dr["recordid"]);
                        RdReferralDetails.pIsreferralcommissionapplicable = Convert.ToBoolean(dr["isreferralcommissionapplicable"]);
                        RdReferralDetails.pReferralcommissiontype = dr["referralcommissiontype"].ToString();
                        RdReferralDetails.pCommissionValue = Convert.ToDecimal(dr["commissionValue"]);
                        RdReferralDetails.pIstdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                        RdReferralDetails.pTdsaccountid = dr["tdsaccountid"].ToString();
                        RdReferralDetails.pTdssection = dr["tdssection"].ToString();
                      
                        RdReferralDetails.pTypeofOperation = "OLD";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RdReferralDetails;
        }
        #endregion

        #region Save Identification Documents RD
        public bool SaveIdentificationDocumentsRD(IdentificationDocumentsDt IdentificationDocumentsDto, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();


            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                sbInsert.Append("delete from tblmstloanwisedocumentproofs where loantypeid=" + IdentificationDocumentsDto.pRdconfigid + " and contacttype='RD';");
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
                                    sbInsert.Append("insert into tblmstloanwisedocumentproofs(loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory,statusid,createdby,createddate) values(" + IdentificationDocumentsDto.pRdconfigid + "," + IdentificationDocumentsDto.pRdconfigid + ",'RD'," + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentId + "," + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentgroupId + ",'" + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentRequired + "','" + IdentificationDocumentsDto.identificationdocumentsList[i].pDocumentsList[j].pDocumentMandatory + "'," + Convert.ToInt32(Status.Active) + "," + IdentificationDocumentsDto.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString() + "" + sbInsert.ToString());
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

        #region Get Identification Documents RD
        public IdentificationDocumentsDt GetIdentificationDocumentsRD(string RdName, string RdNameCode, string Connectionstring)
        {
            string Query = string.Empty;
            IdentificationDocumentsDt IdentificationDocumentsDto = new IdentificationDocumentsDt();
            IdentificationDocumentsDto.identificationdocumentsList = new List<FinstaInfrastructure.Loans.Masters.DocumentsMasterDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select rdconfigid,rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid from tblmstrecurringdepositconfig where upper(rdname) = '" + ManageQuote(RdName.ToUpper()) + "' and upper(rdnamecode) ='" + ManageQuote(RdNameCode.ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        IdentificationDocumentsDto.pRdconfigid = Convert.ToInt64(dr["rdconfigid"]);
                        IdentificationDocumentsDto.pRdname = dr["rdname"].ToString();
                        IdentificationDocumentsDto.pRdcode = dr["rdcode"].ToString();
                        IdentificationDocumentsDto.pRdnamecode = dr["rdnamecode"].ToString();

                    }
                }
                using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup"))
                {
                    if (IdentificationDocumentsDto.pRdconfigid > 0)
                    {

                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1 and documentid not in(select documentid from tblmstloanwisedocumentproofs where statusid=1 and  loanid=" + IdentificationDocumentsDto.pRdconfigid + " and contacttype ='RD') union select y.contacttype,x.documentid,x.documentgroupid,x.documentgroupname,x.documentname,y.isdocumentmandatory as mandatory,y.isdocumentrequired required from tblmstdocumentproofs x right join tblmstloanwisedocumentproofs y on x.documentid = y.documentid where y.statusid = 1 and y.loanid = " + IdentificationDocumentsDto.pRdconfigid + " and contacttype ='RD';";
                    }
                    else
                    {
                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required,ts.statusname from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = 1;";
                    }
                    while (dr1.Read())
                    {
                        DocumentsMasterDTO objdocumentidproofs = new DocumentsMasterDTO();
                        objdocumentidproofs.pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]);
                        objdocumentidproofs.pDocumentGroup = dr1["documentgroupname"].ToString();
                        objdocumentidproofs.pDocumentsList = new List<pIdentificationDocumentsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                        {
                            if (IdentificationDocumentsDto.pRdconfigid > 0)
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
                                            pLoanId = IdentificationDocumentsDto.pRdconfigid,
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
                                            pLoanId = IdentificationDocumentsDto.pRdconfigid,
                                            // pLoantypeId = pLoanId,
                                        });
                                    }
                                }
                            }
                        }
                        IdentificationDocumentsDto.identificationdocumentsList.Add(objdocumentidproofs);
                    }
                }
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory from tblmstloanwisedocumentproofs where loantypeid = " + IdentificationDocumentsDto.pFdconfigid + " and contacttype ='RD' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                //{
                //    while (dr.Read())
                //    {
                //        IdentificationDocumentsDto.identificationdocumentsList.Add(new FinstaInfrastructure.Loans.Masters.DocumentlistDto
                //        {
                //            pLoantypeId = Convert.ToInt64(dr["loantypeid"]),
                //            pLoanId = Convert.ToInt64(dr["loanid"]),
                //            pContactType = dr["contacttype"].ToString(),
                //            pDocumentId = Convert.ToInt64(dr["documentid"]),
                //            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                //            pDocumentRequired = Convert.ToBoolean(dr["isdocumentrequired"].ToString()),
                //            pDocumentMandatory = Convert.ToBoolean(dr["isdocumentmandatory"].ToString())
                //        });
                //    }
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return IdentificationDocumentsDto;
        }
        #endregion

        #region Delete Rd Configuration
        public bool DeleteRdConfiguration(long RdConfigId, string Connectionstring)
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
                if (!string.IsNullOrEmpty(RdConfigId.ToString()) && RdConfigId != 0)
                {
                    sbInsert.Append("update tblmstrecurringdepositConfig set statusid=" + Convert.ToInt32(Status.Inactive) + " where rdconfigid=" + RdConfigId + ";");
                    sbInsert.Append("update tblmstrecurringdepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where rdconfigid=" + RdConfigId + ";");
                    sbInsert.Append("update tblmstrecurringdepositLoansConfig set statusid=" + Convert.ToInt32(Status.Inactive) + " where rdconfigid=" + RdConfigId + ";");
                    sbInsert.Append("update tblmstrecurringdepositConfigreferraldetails set statusid=" + Convert.ToInt32(Status.Inactive) + " where rdconfigid=" + RdConfigId + ";");
                    sbInsert.Append("update tblmstloanwisedocumentproofs set statusid=" + Convert.ToInt32(Status.Inactive) + " where loantypeid=" + RdConfigId + " and contacttype='RD';");
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



        #region Comment Methods
        //public bool Saverdconfigarationdetails(RdConfigartionDetails Rdconfiglist, string Connectionstring)
        //{
        //    string Recordid = string.Empty;
        //    string Recordid1 = string.Empty;
        //    bool IsSaved = false;
        //    StringBuilder sbInsert = new StringBuilder();
        //    StringBuilder queryUpdate = new StringBuilder();
        //    try
        //    {
        //        con = new NpgsqlConnection(Connectionstring);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();

        //        if (Rdconfiglist.lstRdConfigartionDetails != null)
        //        {
        //            for (int i = 0; i < Rdconfiglist.lstRdConfigartionDetails.Count; i++)
        //            {
        //                if (Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode.Trim().ToUpper() != "TABLE")
        //                {
        //                    if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
        //                    {
        //                        if (string.IsNullOrEmpty(Recordid))
        //                        {
        //                            Recordid = Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
        //                        }
        //                        else
        //                        {
        //                            Recordid = Recordid + "," + Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
        //                        }
        //                    }

        //                    if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
        //                    {
        //                        sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode, mininstallmentamount, maxinstallmentamount, installmentpayin, investmentperiodfrom, investmentperiodto, interestpayout, interesttype,compoundinteresttype,interestratefrom, interestrateto, valueper100, statusid, createdby, createddate)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + "," + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + "," + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + "," + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp);");
        //                    }
        //                    else if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
        //                    {
        //                        sbInsert.Append("update tblmstrecurringdepositConfigdetails set membertypeid=" + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + "membertype ='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "', applicanttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "', rdcalculationmode='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "', mininstallmentamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pMininstalmentamount + ", maxinstallmentamount=" + Rdconfiglist.lstRdConfigartionDetails[i].pMaxinstalmentamount + ", installmentpayin='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInstalmentpayin) + "', investmentperiodfrom'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodfrom) + "', investmentperiodto='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInvestmentperiodto) + "', interestpayout='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "', interesttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInteresttype) + "',compoundinteresttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pCompoundInteresttype) + "', interestratefrom=" + Rdconfiglist.lstRdConfigartionDetails[i].pInterestratefrom + ", interestrateto" + Rdconfiglist.lstRdConfigartionDetails[i].pInterestrateto + ", valueper100=" + Rdconfiglist.lstRdConfigartionDetails[i].pValueper100 + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp");

        //                    }

        //                }
        //                else
        //                {
        //                    if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() != "CREATE")
        //                    {
        //                        if (string.IsNullOrEmpty(Recordid))
        //                        {
        //                            Recordid = Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
        //                        }
        //                        else
        //                        {
        //                            Recordid = Recordid + "," + Rdconfiglist.lstRdConfigartionDetails[i].precordid.ToString();
        //                        }
        //                    }
        //                    if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "CREATE")
        //                    {
        //                        sbInsert.Append("insert into tblmstrecurringdepositConfigdetails(rdconfigid, rdname, membertypeid, membertype, applicanttype, rdcalculationmode,interestpayout,statusid, createdby, createddate)values(" + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "','" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "'," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp);");
        //                    }
        //                    if (Rdconfiglist.lstRdConfigartionDetails[i].pTypeofOperation.Trim().ToUpper() == "UPDATE")
        //                    {
        //                        sbInsert.Append("Update tblmstrecurringdepositConfigdetails set membertypeid=" + Rdconfiglist.lstRdConfigartionDetails[i].pMembertypeid + "membertype ='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pMembertype) + "', applicanttype='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pApplicanttype) + "', rdcalculationmode='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pRdcalucationmode) + "',interestpayout='" + ManageQuote(Rdconfiglist.lstRdConfigartionDetails[i].pInterestpayuot) + "',modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where rdconfigid =" + Rdconfiglist.pRdconfigid + ";");
        //                    }

        //                    }

        //            }
        //        }
        //        if (Rdconfiglist.lstDepositCalculationTables != null)
        //        {
        //            for (int J = 0; J < Rdconfiglist.lstDepositCalculationTables.Count; J++)
        //            {

        //                if (Rdconfiglist.lstDepositCalculationTables[J].pTypeofOperation.Trim().ToUpper() != "CREATE")
        //                {
        //                    if (string.IsNullOrEmpty(Recordid1))
        //                    {
        //                        Recordid1 = Rdconfiglist.lstDepositCalculationTables[J].precordid.ToString();
        //                    }
        //                    else
        //                    {
        //                        Recordid1 = Recordid1 + "," + Rdconfiglist.lstDepositCalculationTables[J].precordid.ToString();
        //                    }
        //                }
        //                if (Rdconfiglist.lstDepositCalculationTables[J].pTypeofOperation.Trim().ToUpper() == "CREATE")
        //                {

        //                    sbInsert.Append("insert into tblmstdepositcalculationtable (deposittype,depositconfigid,depositname,membertypeid,membertype,applicanttype,tenure,tenuremode,Payindenomination,interestcaltype,interestamount,depositamount,maturityamount,statusid, createdby, createddate)values('RD'," + Rdconfiglist.pRdconfigid + ",'" + ManageQuote(Rdconfiglist.pRdname) + "'," + Rdconfiglist.lstDepositCalculationTables[J].pMembertypeid + ",'" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pMembertype) + "','" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pApplicanttype) + "'," + Rdconfiglist.lstDepositCalculationTables[J].pTenure + ",'" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pTenuremode) + "'," + Rdconfiglist.lstDepositCalculationTables[J].pPayindenomination + ",'" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pInterestcaltype) + "'," + Rdconfiglist.lstDepositCalculationTables[J].pInterestamount + "," + Rdconfiglist.lstDepositCalculationTables[J].pDepositamount + "," + Rdconfiglist.lstDepositCalculationTables[J].pMaturityamount + "," + Convert.ToInt32(Status.Active) + "," + Rdconfiglist.pCreatedby + ",current_timestamp);");
        //                }
        //                if (Rdconfiglist.lstDepositCalculationTables[J].pTypeofOperation.Trim().ToUpper() == "UPDATE")
        //                {
        //                    sbInsert.Append("update tblmstdepositcalculationtable set membertypeid=" + Rdconfiglist.lstDepositCalculationTables[J].pMembertypeid + ",membertype='" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pMembertype) + "',applicanttype='" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pApplicanttype) + "',tenure=" + Rdconfiglist.lstDepositCalculationTables[J].pTenure + ",tenuremode='" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pTenuremode) + "',Payindenomination=" + Rdconfiglist.lstDepositCalculationTables[J].pPayindenomination + ",interestcaltype='" + ManageQuote(Rdconfiglist.lstDepositCalculationTables[J].pInterestcaltype) + "',interestamount=" + Rdconfiglist.lstDepositCalculationTables[J].pInterestamount + ",depositamount=" + Rdconfiglist.lstDepositCalculationTables[J].pDepositamount + ",maturityamount=" + Rdconfiglist.lstDepositCalculationTables[J].pMaturityamount + "  where recordid=" + Rdconfiglist.lstDepositCalculationTables[J].precordid + ";");
        //                }

        //                }
        //        }
        //        if (!string.IsNullOrEmpty(Recordid))
        //        {
        //            queryUpdate.Append("update tblmstrecurringdepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where rdconfigid=" + Rdconfiglist.pRdconfigid + "  and recordid not in(" + Recordid + ");");
        //        }
        //        else
        //        {
        //            if (Rdconfiglist.lstRdConfigartionDetails.Count == 0)
        //            {
        //                queryUpdate.Append("update tblmstrecurringdepositConfigdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where rdconfigid=" + Rdconfiglist.pRdconfigid + ";");
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Recordid1))
        //        {
        //            queryUpdate.Append("update tblmstdepositcalculationtable set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where depositconfigid=" + Rdconfiglist.pRdconfigid + "  and recordid not in(" + Recordid1 + ");");
        //        }
        //        else
        //        {
        //            if (Rdconfiglist.lstDepositCalculationTables.Count == 0)
        //            {
        //                queryUpdate.Append("update tblmstdepositcalculationtable set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Rdconfiglist.pCreatedby + ",modifieddate=current_timestamp where depositconfigid=" + Rdconfiglist.pRdconfigid + ";");
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(sbInsert.ToString())|| !string.IsNullOrEmpty(queryUpdate.ToString()))
        //        {
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, queryUpdate.ToString() + "" + sbInsert.ToString());
        //        }
        //        trans.Commit();
        //        IsSaved = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        trans.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Dispose();
        //            con.Close();
        //            con.ClearPool();
        //            trans.Dispose();
        //        }
        //    }
        //    return IsSaved;
        //}

        //public string SaverdNameAndCode(RdNameAndCodeDTO Rdnameandcode, string Connectionstring, out long Rdconfigid, out long Checkcount)
        //{
        //    //  bool IsSaved = false;
        //    int count = 0;
        //    StringBuilder sbInsert = new StringBuilder();
        //    Rdnameandcode.pDuplicatecount = 0;
        //    try
        //    {
        //        con = new NpgsqlConnection(Connectionstring);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();




        //        if (!string.IsNullOrEmpty(Rdnameandcode.ptypeofoperation) && Rdnameandcode.ptypeofoperation.Trim().ToUpper() == "CREATE")
        //        {
        //            count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select count(*) from tblmstrecurringdepositConfig where upper(rdname)='" + ManageQuote(Rdnameandcode.pRdname.ToUpper()) + "';"));
        //            if (count == 0)
        //            {
        //                Rdnameandcode.pRdconfigid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstrecurringdepositConfig (rdname,rdcode,companycode,branchcode,series,serieslength,rdnamecode,statusid,createdby,createddate)values('" + ManageQuote(Rdnameandcode.pRdname) + "','" + ManageQuote(Rdnameandcode.pRdcode) + "','" + ManageQuote(Rdnameandcode.pCompanycode) + "','" + ManageQuote(Rdnameandcode.pBranchcode) + "','" + ManageQuote(Rdnameandcode.pSeries) + "'," + Rdnameandcode.pSerieslength + ",'" + ManageQuote(Rdnameandcode.pRdnamecode) + "'," + Convert.ToInt32(Status.Active) + "," + Rdnameandcode.pCreatedby + ",current_timestamp) returning Rdconfigid;"));

        //            }
        //            else
        //            {
        //                Rdnameandcode.pDuplicatecount = count;

        //            }
        //        }
        //        else
        //        {
        //            sbInsert.Append("update tblmstrecurringdepositConfig set rdname='" + ManageQuote(Rdnameandcode.pRdname) + "', rdcode='" + ManageQuote(Rdnameandcode.pRdcode) + "', companycode='" + ManageQuote(Rdnameandcode.pCompanycode) + "', branchcode='" + ManageQuote(Rdnameandcode.pBranchcode) + "',series='" + ManageQuote(Rdnameandcode.pSeries) + "', serieslength=" + Rdnameandcode.pSerieslength + ", sharenamecode='" + ManageQuote(Rdnameandcode.pRdnamecode) + "' where shareconfigid=" + Rdnameandcode.pRdconfigid + ";");
        //        }
        //        Rdconfigid = Rdnameandcode.pRdconfigid;
        //        Checkcount = Rdnameandcode.pDuplicatecount;
        //        if (!string.IsNullOrEmpty(sbInsert.ToString()))
        //        {
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
        //        }
        //        trans.Commit();


        //        //  IsSaved = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        trans.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Dispose();
        //            con.Close();
        //            con.ClearPool();
        //            trans.Dispose();
        //        }
        //    }
        //    return Rdnameandcode.pRdname;

        //}
        #endregion


        public List<RecurringDepositPrematurityInterestPercentages> GetRecurringDepositPrematurityInterestPercentagesData(long RdConfigId, string ConnectionString)
        {
            var _RecurringDepositPrematurityInterestPercentagesList = new List<RecurringDepositPrematurityInterestPercentages>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, coalesce(minprematuritytoperiod,0) as minprematuritytoperiod,coalesce( maxprematurityfromperiod,0) as maxprematurityfromperiod,  prematurityperiodtype,coalesce(percentage,0) as percentage  FROM tblmstrecurringdepositprematurityinterestpercentages where  rdconfigid = '" + RdConfigId + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        RecurringDepositPrematurityInterestPercentages _RdMaturityRates = new RecurringDepositPrematurityInterestPercentages
                        {
                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            pminprematuritypercentage = Convert.ToDecimal(dr["minprematuritytoperiod"]),
                            pmaxprematuritypercentage = Convert.ToDecimal(dr["maxprematurityfromperiod"]),
                            pprematurityperiodtype = Convert.ToString(dr["prematurityperiodtype"]),
                            pTypeofOperation = "OLD",
                            pPercentage = Convert.ToDecimal(dr["percentage"])
                        };
                        _RecurringDepositPrematurityInterestPercentagesList.Add(_RdMaturityRates);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _RecurringDepositPrematurityInterestPercentagesList;


        }

        public RdschemeandcodeCount GetRDNameCount(Int64 RDconfigid, string RDName, string RdCode, string ConnectionString)
        {
            RdschemeandcodeCount _RdschemeandcodeCount = new RdschemeandcodeCount();
            try
            {
                if(RDconfigid == 0)
                {
                    _RdschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositconfig where upper(rdname)='" + ManageQuote(RDName).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _RdschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositconfig where upper(rdcode)='" + ManageQuote(RdCode).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    _RdschemeandcodeCount.pSchemeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositconfig where upper(rdname)='" + ManageQuote(RDName).ToUpper() + "' and rdconfigid <> "+RDconfigid+" and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                    _RdschemeandcodeCount.pSchemeCodeCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstrecurringdepositconfig where upper(rdcode)='" + ManageQuote(RdCode).ToUpper() + "' and rdconfigid <> " + RDconfigid + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _RdschemeandcodeCount;
        }
    }
}
