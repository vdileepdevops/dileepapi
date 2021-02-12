using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public class ApprovalDAL : SettingsDAL, IApproval
    {
        public ApprovalDTO ApprovedetailsDTO { get; set; }
        public ViewapplicationsDTO Viewapplicants { get; set; }
        public List<ApprovalDTO> lstApprovedetails { get; set; }
        public List<approvalpaymentstagesDTO> lstApprovestages { get; set; }
        public List<approvedloanchargesDTO> lstApprovalCharges { get; set; }
        public List<ViewapplicationsDTO> lstViewapplications { get; set; }
        public List<LoanwisechargeDTO> lstLoanwisecharges { get; set; }
        public List<CashflowDTO> lstCashflow { get; set; }
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;

        #region Saveapprovedapplications
        public bool Saveapprovedapplications(ApprovalDTO ApprovalList, string Connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            string Approvestatus = string.Empty;
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                //Approvestatus = NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select approvedpersonstatus from tblmstapprovalpersons t1 join tblmstusers t2 on contactrefid=contactreferenceid where userid=" + ApprovalList.pCreatedby + ";").ToString();

                Approvestatus = NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select approvedpersonstatus from tblmstapprovalpersons;").ToString();
                if (Approvestatus == "Approved")
                {
                    if (ApprovalList.papprovestatus == "Approve")
                    {
                        if (ApprovalList.lstStagewisepayments.Count > 0 && ApprovalList.pDisbursementpayinmode.ToUpper() == "SINGLE PAYMENT")
                            ApprovalList.pDisbursementpayinmode = "Stage Payment";

                        sbinsert.Append("insert into tbltransapprovedapplications(applicationid,vchapplicationid,approveddate,approvedby,amountrequested,approvedloanamount,loanpayin,interesttype,rateofinterest,tenureofloan,tenuretype,vchaccintrest,loaninstalmentpaymentmode,Installmentamount,holidayperiodpayin,holidayperiodvalue,moratoriumperiodpayin,moratoriumperiodvalue,remarks,disbursementpayinmode,graceperiod,statusid, createdby, createddate,loantypeid,loantype,loanid,loanname,partprinciplepaidinterval)values(" + ApprovalList.pApplicationid + ",'" + ApprovalList.pVchapplicationid + "','" + FormatDate(ApprovalList.pApproveddate.ToString()) + "'," + ApprovalList.pApprovedby + "," + ApprovalList.pAmountrequested + "," + ApprovalList.pApprovedloanamount + ",'" + ManageQuote(ApprovalList.pLoanpayin) + "','" + ManageQuote(ApprovalList.pInteresttype) + "'," + ApprovalList.pRateofinterest + "," + ApprovalList.pTenureofloan + ",'" + ManageQuote(ApprovalList.pTenuretype) + "'," + ApprovalList.pVchaccintrest + ",'" + ManageQuote(ApprovalList.pLoaninstalmentpaymentmode) + "'," + ApprovalList.pInstallmentamount + ",'" + ManageQuote(ApprovalList.pHolidayperiodpayin) + "'," + ApprovalList.pHolidayperiodvalue + ",'" + ManageQuote(ApprovalList.pMoratoriumperiodpayin) + "'," + ApprovalList.pMoratoriumperiodvalue + ",'" + ManageQuote(ApprovalList.pRemarks) + "','" + ManageQuote(ApprovalList.pDisbursementpayinmode) + "'," + ApprovalList.pGraceperiod + ",1," + ApprovalList.pCreatedby + ",current_timestamp," + ApprovalList.pLoantypeid + ",'" + ManageQuote(ApprovalList.pLoantype) + "'," + ApprovalList.pLoanid + ",'" + ManageQuote(ApprovalList.pLoanname) + "'," + ApprovalList.pInterevels + ");");
                        Saveacceptedorrejectedapplications(ApprovalList, Connectionstring, sbinsert);
                        Saveapprovalpaymentstages(ApprovalList, Connectionstring, sbinsert);
                        SaveApprovelcharges(ApprovalList, Connectionstring, sbinsert);
                        sbinsert.Append("update tabapplication set loanstatus='Loan Approved',loanstatusid=" + Convert.ToInt32(Status.Loan_Approved) + ",approvaldate='" + FormatDate(ApprovalList.pApproveddate.ToString()) + "' where vchapplicationid='" + ApprovalList.pVchapplicationid + "'");

                    }
                    if (ApprovalList.papprovestatus == "Reject")
                    {
                        Saveacceptedorrejectedapplications(ApprovalList, Connectionstring, sbinsert);
                        sbinsert.Append("update tabapplication set loanstatus='Loan Rejected',loanstatusid=" + Convert.ToInt32(Status.Loan_Rejected) + ",approvaldate='" + FormatDate(ApprovalList.pApproveddate.ToString()) + "' where vchapplicationid='" + ApprovalList.pVchapplicationid + "'");
                    }
                }
                if (Approvestatus == "Accepted")
                {
                    Saveacceptedorrejectedapplications(ApprovalList, Connectionstring, sbinsert);
                    sbinsert.Append("update tabapplication set loanstatus='Loan Accepted',loanstatusid=11,approvaldate='" + FormatDate(ApprovalList.pApproveddate.ToString()) + "' where vchapplicationid='" + ApprovalList.pVchapplicationid + "'");
                }

                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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
        private void Saveacceptedorrejectedapplications(ApprovalDTO ApprovalList, string Connectionstring, StringBuilder sbinsert)
        {
            sbinsert.Append("insert into tbltransacceptedorrejectedapplications(applicationid,vchapplicationid,approveddate,approvedby,amountrequested,approvedloanamount,loanpayin,interesttype,rateofinterest,tenureofloan,tenuretype,vchaccintrest,loaninstalmentpaymentmode,oldInstallmentstartdate,Installmentamount,Installmentstartdate,holidayperiodpayin,holidayperiodvalue,moratoriumperiodpayin,moratoriumperiodvalue,remarks,disbursementpayinmode,graceperiod,statusid, createdby, createddate)values(" + ApprovalList.pApplicationid + ",'" + ApprovalList.pVchapplicationid + "','" + FormatDate(ApprovalList.pApproveddate.ToString()) + "'," + ApprovalList.pApprovedby + "," + ApprovalList.pAmountrequested + "," + ApprovalList.pApprovedloanamount + ",'" + ManageQuote(ApprovalList.pLoanpayin) + "','" + ManageQuote(ApprovalList.pInteresttype) + "'," + ApprovalList.pRateofinterest + "," + ApprovalList.pTenureofloan + ",'" + ManageQuote(ApprovalList.pTenuretype) + "'," + ApprovalList.pVchaccintrest + ",'" + ManageQuote(ApprovalList.pLoaninstalmentpaymentmode) + "','" + FormatDate(ApprovalList.pOldInstallmentstartdate.ToString()) + "'," + ApprovalList.pInstallmentamount + ",'" + FormatDate(ApprovalList.pInstallmentstartdate.ToString()) + "','" + ManageQuote(ApprovalList.pHolidayperiodpayin) + "'," + ApprovalList.pHolidayperiodvalue + ",'" + ManageQuote(ApprovalList.pMoratoriumperiodpayin) + "'," + ApprovalList.pMoratoriumperiodvalue + ",'" + ManageQuote(ApprovalList.pRemarks) + "','" + ManageQuote(ApprovalList.pDisbursementpayinmode) + "'," + ApprovalList.pGraceperiod + ",1," + ApprovalList.pCreatedby + ",current_timestamp);");
        }
        private void SaveApprovelcharges(ApprovalDTO ApprovalList, string Connectionstring, StringBuilder sbinsert)
        {
            try
            {
                if (ApprovalList.lstApprovedloancharges != null)
                {
                    for (int i = 0; i < ApprovalList.lstApprovedloancharges.Count; i++)
                    {
                        if (ApprovalList.lstApprovedloancharges[i].ptypeofoperation == "CREATE")
                        {
                            sbinsert.Append(" insert into tbltransapprovedloancharges(applicationid, vchapplicationid, chargestype, chargename, chargereceivableamount, chargewaiveoffamount, chargepaymentmode, statusid, createdby, createddate,gsttype,gstcalculationtype,gstpercentage,gstamount,totalchargeamount,igstamount,cgstamount,sgstamount,utgstamount,chargeamount)values(" + ApprovalList.pApplicationid + ",'" + ApprovalList.pVchapplicationid + "','" + ManageQuote(ApprovalList.lstApprovedloancharges[i].pChargestype) + "','" + ManageQuote(ApprovalList.lstApprovedloancharges[i].pChargename) + "'," + ApprovalList.lstApprovedloancharges[i].pChargereceivableamount + "," + ApprovalList.lstApprovedloancharges[i].pChargewaiveoffamount + ",'" + ManageQuote(ApprovalList.lstApprovedloancharges[i].pChargepaymentmode) + "',1," + ApprovalList.pCreatedby + ",current_timestamp,'" + ManageQuote(ApprovalList.lstApprovedloancharges[i].pGsttype) + "','" + ManageQuote(ApprovalList.lstApprovedloancharges[i].pGstcaltype) + "'," + ApprovalList.lstApprovedloancharges[i].pGstpercentage + "," + ApprovalList.lstApprovedloancharges[i].pTotalgstamount + "," + ApprovalList.lstApprovedloancharges[i].pTotalchargeamount + "," + ApprovalList.lstApprovedloancharges[i].pIgstamount + "," + ApprovalList.lstApprovedloancharges[i].pCgstamount + "," + ApprovalList.lstApprovedloancharges[i].pSgstamount + "," + ApprovalList.lstApprovedloancharges[i].pUtgstamount + "," + ApprovalList.lstApprovedloancharges[i].pActualchargeamount + ");");
                        }
                        if (ApprovalList.lstApprovedloancharges[i].ptypeofoperation == "UPDATE")
                        {
                            //sbinsert.Append("update tbltransapprovedloancharges");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void Saveapprovalpaymentstages(ApprovalDTO ApprovalList, string Connectionstring, StringBuilder sbinsert)
        {
            try
            {
                if (ApprovalList.lstStagewisepayments != null)
                {
                    for (int i = 0; i < ApprovalList.lstStagewisepayments.Count; i++)
                    {
                        if (ApprovalList.lstStagewisepayments[i].ptypeofoperation == "CREATE")
                        {
                            sbinsert.Append("insert into tbltransapprovalpaymentstages(applicationid,vchapplicationid,stageno,stagename,paymentreleasetype,paymentreleasepercentage,paymentreleaseamount,statusid,createdby,createddate)values(" + ApprovalList.pApplicationid + ",'" + ApprovalList.pVchapplicationid + "','" + ManageQuote(ApprovalList.lstStagewisepayments[i].pStageno) + "','" + ManageQuote(ApprovalList.lstStagewisepayments[i].pStagename) + "','" + ManageQuote(ApprovalList.lstStagewisepayments[i].pPaymentreleasetype) + "'," + ApprovalList.lstStagewisepayments[i].pPaymentreleasepercentage + "," + ApprovalList.lstStagewisepayments[i].pPaymentreleaseamount + ",1," + ApprovalList.pCreatedby + ",current_timestamp);");
                        }
                        if (ApprovalList.lstStagewisepayments[i].ptypeofoperation == "UPDATE")
                        {
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region Getapprovedapplications
        public List<ApprovalDTO> Getapprovedapplications(string Applicationid, string ConnectionString)
        {
            ApprovalDTO objApprovedetails = new ApprovalDTO();
            lstApprovedetails = new List<ApprovalDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from tbltransapprovedapplications WHERE VCHAPPLICATIONID='" + ManageQuote(Applicationid) + "'"))
                {
                    while (dr.Read())
                    {
                        objApprovedetails.pApplicationid = Convert.ToInt64(dr["Applicationid"]);
                        objApprovedetails.pVchapplicationid = dr["Vchapplicationid"].ToString();
                        objApprovedetails.pApproveddate = Convert.ToDateTime(dr["Approveddate"]);
                        objApprovedetails.pApprovedby = Convert.ToInt64(dr["Approvedby"]);
                        objApprovedetails.pAmountrequested = Convert.ToDecimal(dr["Amountrequested"]);
                        objApprovedetails.pApprovedloanamount = Convert.ToDecimal(dr["Approvedloanamount"]);
                        objApprovedetails.pLoanpayin = dr["Loanpayin"].ToString();
                        objApprovedetails.pInteresttype = dr["Interesttype"].ToString();
                        objApprovedetails.pRateofinterest = Convert.ToDecimal(dr["Rateofinterest"]);
                        objApprovedetails.pTenureofloan = Convert.ToDecimal(dr["Tenureofloan"]);
                        objApprovedetails.pTenuretype = dr["Tenuretype"].ToString();
                        objApprovedetails.pVchaccintrest = Convert.ToDecimal(dr["Vchaccintrest"]);
                        objApprovedetails.pLoaninstalmentpaymentmode = dr["Loaninstalmentpaymentmode"].ToString();
                        objApprovedetails.pOldInstallmentstartdate = Convert.ToDateTime(dr["OldInstallmentstartdate"]);
                        objApprovedetails.pInstallmentamount = Convert.ToDecimal(dr["Vchaccintrest"]);
                        objApprovedetails.pInstallmentstartdate = Convert.ToDateTime(dr["Installmentstartdate"]);
                        objApprovedetails.pHolidayperiodpayin = dr["Holidayperiodpayin"].ToString();
                        objApprovedetails.pHolidayperiodvalue = Convert.ToDecimal(dr["Holidayperiodvalue"]);
                        objApprovedetails.pMoratoriumperiodpayin = dr["Moratoriumperiodpayin"].ToString();
                        objApprovedetails.pMoratoriumperiodvalue = Convert.ToDecimal(dr["Moratoriumperiodvalue"]);
                        objApprovedetails.pRemarks = dr["Remarks"].ToString();
                        objApprovedetails.pDisbursementpayinmode = dr["Disbursementpayinmode"].ToString();
                        objApprovedetails.pGraceperiod = Convert.ToDecimal(dr["Graceperiod"]);
                        objApprovedetails.lstStagewisepayments = GetApprovelsStagewise(Applicationid, ConnectionString);
                        objApprovedetails.lstApprovedloancharges = GetApprovelscharges(Applicationid, ConnectionString);
                        lstApprovedetails.Add(objApprovedetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstApprovedetails;
        }

        private List<approvalpaymentstagesDTO> GetApprovelsStagewise(string Applicationid, string ConnectionString)
        {
            lstApprovestages = new List<approvalpaymentstagesDTO>();
            try
            {

                using (dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * FROM tbltransapprovalpaymentstages WHERE VCHAPPLICATIONID='" + Applicationid + "' and statusid=" + getStatusid("ACTIVE", ConnectionString) + ";"))
                    while (dr.Read())
                    {
                        approvalpaymentstagesDTO objStagewise = new approvalpaymentstagesDTO();
                        objStagewise.pRecordid = Convert.ToInt64(dr["recordid"]);
                        objStagewise.pStageno = dr["Stageno"].ToString();
                        objStagewise.pStagename = dr["Stagename"].ToString();
                        objStagewise.pPaymentreleasetype = dr["Paymentreleasetype"].ToString();
                        objStagewise.pPaymentreleasepercentage = Convert.ToDecimal(dr["Paymentreleasepercentage"]);
                        objStagewise.pPaymentreleaseamount = Convert.ToDecimal(dr["Paymentreleaseamount"]);
                        lstApprovestages.Add(objStagewise);
                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstApprovestages;
        }

        private List<approvedloanchargesDTO> GetApprovelscharges(string Applicationid, string ConnectionString)
        {
            lstApprovalCharges = new List<approvedloanchargesDTO>();
            try
            {
                using (dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * FROM tbltransapprovedloancharges WHERE VCHAPPLICATIONID='" + Applicationid + "' and statusid=" + getStatusid("ACTIVE", ConnectionString) + ";"))
                    while (dr.Read())
                    {
                        approvedloanchargesDTO objchargetypes = new approvedloanchargesDTO();
                        objchargetypes.pRecordid = Convert.ToInt64(dr["recordid"]);
                        objchargetypes.pChargestype = dr["Chargestype"].ToString();
                        objchargetypes.pChargename = dr["Chargename"].ToString();
                        objchargetypes.pChargereceivableamount = Convert.ToDecimal(dr["Chargereceivableamount"]);
                        objchargetypes.pChargewaiveoffamount = Convert.ToDecimal(dr["Chargewaiveoffamount"]);
                        objchargetypes.pChargepaymentmode = dr["Chargepaymentmode"].ToString();
                        lstApprovalCharges.Add(objchargetypes);
                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstApprovalCharges;
        }

        #endregion

        #region ViewApplications
        public List<ViewapplicationsDTO> ViewApplications(string Viewtype, string ConnectionString)
        {
            lstViewapplications = new List<ViewapplicationsDTO>();

            try
            {
                if (Viewtype == "New")
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.contacttype,ta.contactreferenceid,loantypeid,loantype,loanid,loanname,Applicanttype,applicationid,vchapplicationid, applicantname, dateofapplication,purposeofloan, amountrequested, tenureofloan, rateofinterest, interesttype, loanpayin,loaninstalmentpaymentmode,coalesce(instalmentamount,0)as instalmentamount,coalesce(partprinciplepaidinterval,0) as partprinciplepaidinterval , tc.businessentitycontactno from tabapplication ta join tblmstcontact tc on ta.contactreferenceid = tc.contactreferenceid where upper(loanstatus) in('FI SAVED','FI PARTIAL SAVED','TELE VERIFICATION','FIELD VERIFICATION','DOCUMENT VERIFICATION','LOAN ACCEPTED') and ta.statusid = 1 order by applicationid desc"))
                    {
                        while (dr.Read())
                        {
                            ViewapplicationsDTO objViewapplications = new ViewapplicationsDTO();
                            objViewapplications.pContacttype = dr["contacttype"].ToString();
                            objViewapplications.pContactreferenceid = dr["contactreferenceid"].ToString();
                            objViewapplications.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                            objViewapplications.pLoantype = dr["loantype"].ToString();
                            objViewapplications.pLoanid = Convert.ToInt64(dr["loanid"]);
                            objViewapplications.pLoanname = dr["loanname"].ToString();
                            objViewapplications.pApplicanttype = dr["Applicanttype"].ToString();
                            objViewapplications.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                            objViewapplications.pVchapplicationid = dr["vchapplicationid"].ToString();
                            objViewapplications.pApplicantname = dr["applicantname"].ToString();
                            objViewapplications.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                            objViewapplications.pPurposeofloan = dr["purposeofloan"].ToString();
                            objViewapplications.pAmountrequested = Convert.ToDecimal(dr["amountrequested"]);
                            objViewapplications.pTenureofloan = Convert.ToDecimal(dr["tenureofloan"]);
                            objViewapplications.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                            objViewapplications.pInteresttype = dr["interesttype"].ToString();
                            objViewapplications.pLoanpayin = dr["loanpayin"].ToString();
                            objViewapplications.pLoaninstalmentpaymentmode = dr["Loaninstalmentpaymentmode"].ToString();
                            objViewapplications.pMobileno = dr["businessentitycontactno"].ToString();
                            objViewapplications.pInstalmentamount = Convert.ToDecimal(dr["Instalmentamount"]);
                            objViewapplications.pInterevels = Convert.ToInt16(dr["partprinciplepaidinterval"]);
                            lstViewapplications.Add(objViewapplications);
                        }
                    }
                }
                if (Viewtype == "Approved")
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.contacttype,ta.contactreferenceid,ta.loantypeid,ta.loantype,ta.loanid,ta.loanname,ta.Applicanttype,ta.applicationid,ta.vchapplicationid,ta.dateofapplication,ta.contactreferenceid,ta.purposeofloan,ta.applicantname,ta.amountrequested,tap.approveddate,tap.approvedby,tap.approvedloanamount,tap.loanpayin,tap.interesttype,tap.tenureofloan,tap.rateofinterest,tc.businessentitycontactno from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid  where loanstatus = 'Loan Approved' order by recordid desc"))
                    {
                        while (dr.Read())
                        {
                            ViewapplicationsDTO objViewapplications = new ViewapplicationsDTO();
                            objViewapplications.pContacttype = dr["contacttype"].ToString();
                            objViewapplications.pContactreferenceid = dr["contactreferenceid"].ToString();
                            objViewapplications.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                            objViewapplications.pLoantype = dr["loantype"].ToString();
                            objViewapplications.pLoanid = Convert.ToInt64(dr["loanid"]);
                            objViewapplications.pLoanname = dr["loanname"].ToString();
                            objViewapplications.pApplicanttype = dr["Applicanttype"].ToString();
                            objViewapplications.pVchapplicationid = dr["vchapplicationid"].ToString();
                            objViewapplications.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                            objViewapplications.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                            objViewapplications.pContactreferenceid = dr["contactreferenceid"].ToString();
                            objViewapplications.pApplicantname = dr["applicantname"].ToString();
                            objViewapplications.pPurposeofloan = dr["purposeofloan"].ToString();
                            objViewapplications.pAmountrequested = Convert.ToDecimal(dr["amountrequested"]);
                            objViewapplications.pAproveddate = Convert.ToDateTime(dr["approveddate"]).ToString("dd/MM/yyyy");
                            objViewapplications.pApprovedloanamount = Convert.ToDecimal(dr["approvedloanamount"]);
                            objViewapplications.pLoanpayin = dr["loanpayin"].ToString();
                            objViewapplications.pInteresttype = dr["interesttype"].ToString();
                            objViewapplications.pTenureofloan = Convert.ToDecimal(dr["Tenureofloan"]);
                            objViewapplications.pRateofinterest = Convert.ToDecimal(dr["Rateofinterest"]);
                            objViewapplications.pMobileno = dr["businessentitycontactno"].ToString();
                            lstViewapplications.Add(objViewapplications);
                        }
                    }
                }
                if (Viewtype == "Rejected")
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.contacttype,ta.contactreferenceid,loantypeid,loantype,loanid,loanname,Applicanttype,applicationid,vchapplicationid, applicantname, dateofapplication,purposeofloan, amountrequested, tenureofloan, rateofinterest, interesttype, loanpayin, tc.businessentitycontactno,ta.approvaldate from tabapplication ta join tblmstcontact tc on ta.contactreferenceid = tc.contactreferenceid where loanstatus = 'Loan Rejected' and ta.statusid = 1 order by applicationid desc"))
                    {
                        while (dr.Read())
                        {
                            ViewapplicationsDTO objViewapplications = new ViewapplicationsDTO();
                            objViewapplications.pContacttype = dr["contacttype"].ToString();
                            objViewapplications.pContactreferenceid = dr["contactreferenceid"].ToString();
                            objViewapplications.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                            objViewapplications.pLoantype = dr["loantype"].ToString();
                            objViewapplications.pLoanid = Convert.ToInt64(dr["loanid"]);
                            objViewapplications.pLoanname = dr["loanname"].ToString();
                            objViewapplications.pApplicanttype = dr["Applicanttype"].ToString();
                            objViewapplications.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                            objViewapplications.pVchapplicationid = dr["vchapplicationid"].ToString();
                            objViewapplications.pApplicantname = dr["applicantname"].ToString();
                            objViewapplications.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                            objViewapplications.pAproveddate = Convert.ToDateTime(dr["approvaldate"]).ToString("dd/MM/yyyy");
                            objViewapplications.pPurposeofloan = dr["purposeofloan"].ToString();
                            objViewapplications.pAmountrequested = Convert.ToDecimal(dr["amountrequested"]);
                            objViewapplications.pTenureofloan = Convert.ToDecimal(dr["tenureofloan"]);
                            objViewapplications.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                            objViewapplications.pInteresttype = dr["interesttype"].ToString();
                            objViewapplications.pLoanpayin = dr["loanpayin"].ToString();
                            objViewapplications.pMobileno = dr["businessentitycontactno"].ToString();
                            lstViewapplications.Add(objViewapplications);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstViewapplications;
        }
        #endregion

        #region GetLoanwisecharges
        public List<LoanwisechargeDTO> GetLoanwisecharges(string Loanname, decimal Amount, decimal tenor, string applicanttype, string Loanpayin, string ConnectionString, string tranddate, Int32 schemeid)
        {
            lstLoanwisecharges = new List<LoanwisechargeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from FN_GETLOANWISECHARGES('" + ManageQuote(Loanname) + "'," + Amount + "," + tenor + ",'" + ManageQuote(applicanttype) + "','" + ManageQuote(Loanpayin) + "','" + tranddate + "'," + schemeid + ")"))
                {
                    while (dr.Read())
                    {
                        LoanwisechargeDTO objloanwisecharges = new LoanwisechargeDTO();
                        objloanwisecharges.pChargename = dr["Chargename"].ToString();
                        objloanwisecharges.pChargeamount = Convert.ToDecimal(dr["Chargeamount"]);
                        objloanwisecharges.pGsttype = dr["GSTTYPE"].ToString();
                        objloanwisecharges.pGstcaltype = dr["GSTCALTYPE"].ToString();
                        objloanwisecharges.pGstpercentage = Convert.ToDecimal(dr["GSTPERCENTAGE1"]);
                        objloanwisecharges.pCgstpercentage = Convert.ToDecimal(dr["CGSTPERCENTAGE1"]);
                        objloanwisecharges.pSgstpercentage = Convert.ToDecimal(dr["SGSTPERCENTAGE1"]);
                        objloanwisecharges.pCgstamount = Convert.ToDecimal(dr["CGSTAMOUNT"]);
                        objloanwisecharges.pSgstamount = Convert.ToDecimal(dr["SGSTAMOUNT"]);
                        objloanwisecharges.pTotalgstamount = Convert.ToDecimal(dr["TOTALGSTAMOUNT"]);
                        objloanwisecharges.pTotalchargeamount = Convert.ToDecimal(dr["TOTALCHARGEAMOUNT"]);
                        objloanwisecharges.pUtsgtpercentage = Convert.ToDecimal(dr["UTGSTPERCENTAGE1"]);
                        objloanwisecharges.pIgstpercentage = Convert.ToDecimal(dr["IGSTPERCENTAGE1"]);
                        objloanwisecharges.pUtgstamount = Convert.ToDecimal(dr["UTGSTAMOUNT"]);
                        objloanwisecharges.pIgstamount = Convert.ToDecimal(dr["IGSTAMOUNT"]);
                        objloanwisecharges.pActualchargeamount = Convert.ToDecimal(dr["ACTUALCHARGEAMOUNT"]);
                        lstLoanwisecharges.Add(objloanwisecharges);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanwisecharges;
        }
        #endregion

        #region ViewApplicationsbyid
        public List<ViewapplicationsDTO> ViewApplicationsbyid(string applicationid, string ConnectionString)
        {
            lstViewapplications = new List<ViewapplicationsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.contacttype,ta.contactreferenceid,ta.loantypeid,loantype,ta.loanid,loanname,Applicanttype, applicationid,vchapplicationid, applicantname, dateofapplication,purposeofloan, amountrequested, tenureofloan, rateofinterest, interesttype, loanpayin,loaninstalmentpaymentmode,coalesce(instalmentamount,0)as instalmentamount,coalesce(partprinciplepaidinterval,0) as partprinciplepaidinterval ,tc.businessentitycontactno,penaltygraceperiod,(select schemeid from  tblmstschemenamescodes where upper(schemename)=upper(ta.schemename) and statusid=" + Convert.ToInt32(Status.Active) + ") as schemeid  from tabapplication ta join tblmstcontact tc on ta.contactreferenceid = tc.contactreferenceid left join tblmstpenaltyconfiguration tp on ta.loanid=tp.loanid where upper(loanstatus) in('FI SAVED','FI PARTIAL SAVED','TELE VERIFICATION','FIELD VERIFICATION','DOCUMENT VERIFICATION','LOAN ACCEPTED') and ta.statusid = " + Convert.ToInt32(Status.Active) + " and vchapplicationid='" + applicationid + "';"))
                    while (dr.Read())
                    {
                        ViewapplicationsDTO objViewapplications = new ViewapplicationsDTO();
                        objViewapplications.pContacttype = dr["contacttype"].ToString();
                        objViewapplications.pContactreferenceid = dr["contactreferenceid"].ToString();
                        objViewapplications.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                        objViewapplications.pLoantype = dr["loantype"].ToString();
                        objViewapplications.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objViewapplications.pLoanname = dr["loanname"].ToString();
                        objViewapplications.pApplicanttype = dr["Applicanttype"].ToString();
                        objViewapplications.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                        objViewapplications.pVchapplicationid = dr["vchapplicationid"].ToString();
                        objViewapplications.pApplicantname = dr["applicantname"].ToString();
                        objViewapplications.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                        objViewapplications.pPurposeofloan = dr["purposeofloan"].ToString();
                        objViewapplications.pAmountrequested = Convert.ToDecimal(dr["amountrequested"]);
                        objViewapplications.pTenureofloan = Convert.ToDecimal(dr["tenureofloan"]);
                        objViewapplications.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                        objViewapplications.pInteresttype = dr["interesttype"].ToString();
                        objViewapplications.pLoanpayin = dr["loanpayin"].ToString();
                        objViewapplications.pLoaninstalmentpaymentmode = dr["Loaninstalmentpaymentmode"].ToString();
                        objViewapplications.pMobileno = dr["businessentitycontactno"].ToString();
                        objViewapplications.pInstalmentamount = Convert.ToDecimal(dr["Instalmentamount"]);
                        objViewapplications.pInterevels = Convert.ToInt16(dr["partprinciplepaidinterval"]);
                        objViewapplications.pGraceperiod = Convert.ToDecimal(dr["penaltygraceperiod"]);
                        objViewapplications.pschemeid = dr["schemeid"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["schemeid"]);
                        lstViewapplications.Add(objViewapplications);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstViewapplications;
        }
        #endregion

        #region GetcashflowStatements
        public List<CashflowDTO> GetSavingdetails(string applicationid, string ConnectionString)
        {
            lstCashflow = new List<CashflowDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tci.applicantype||'-'||Tms.name||' '||tms.surname AS NAME,(COALESCE(netannualincome,0)-COALESCE(AVERAGEANNUALEXPENSES,0)) AS SAVINGS from TABAPPLICATIONPERSONALINCOMEDETAILS tci join tblmstcontact tms on tci.contactreferenceid=tms.contactreferenceid WHERE VCHAPPLICATIONID = '" + applicationid + "'"))
                    while (dr.Read())
                    {
                        CashflowDTO objSavingdetails = new CashflowDTO();
                        objSavingdetails.pName = dr["NAME"].ToString();
                        objSavingdetails.pSavingsamount = Convert.ToDecimal(dr["SAVINGS"]);
                        lstCashflow.Add(objSavingdetails);
                    }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCashflow;
        }
        public List<CashflowDTO> GetExstingloandetails(string applicationid, string ConnectionString)
        {
            lstCashflow = new List<CashflowDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT LOANNAME,CASE WHEN LOANPAYIN='Daily' THEN INSTALMENTAMOUNT*365 WHEN LOANPAYIN='Weakly' THEN INSTALMENTAMOUNT*58 WHEN LOANPAYIN='Monthly' THEN INSTALMENTAMOUNT*12 WHEN LOANPAYIN='Queatrly' THEN INSTALMENTAMOUNT*4 WHEN LOANPAYIN='Half Yearly' THEN INSTALMENTAMOUNT*2 ELSE INSTALMENTAMOUNT  END AS INSTALMENTAMOUNT FROM TABAPPLICATIONEXISTINGLOANS  WHERE VCHAPPLICATIONID='" + applicationid + "' and statusid=1 ;"))
                    while (dr.Read())
                    {
                        CashflowDTO objSavingdetails = new CashflowDTO();
                        objSavingdetails.pLoanname = dr["LOANNAME"].ToString();
                        objSavingdetails.pInstalmentamount = Convert.ToDecimal(dr["INSTALMENTAMOUNT"]);
                        lstCashflow.Add(objSavingdetails);
                    }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCashflow;
        }
        #endregion
    }
}
