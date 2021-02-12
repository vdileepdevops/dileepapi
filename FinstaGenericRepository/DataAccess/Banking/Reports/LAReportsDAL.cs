using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Banking.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Reports;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;
using static FinstaInfrastructure.Banking.Reports.LAReportsDTO;

namespace FinstaRepository.DataAccess.Banking.Reports
{
    public class LAReportsDAL : SettingsDAL, ILAReports
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        List<LAReportsDTO> lstAgentPoints { get; set; }

        #region Cash Flow Report ...
        public List<LAReportsDTO> GetCashFlowSummary(string date, string months, string Connectionstring)
        {
            List<LAReportsDTO> lstCashflow = new List<LAReportsDTO>();

            try
            {
                string Query = string.Empty;
                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select  FNCashflow('" + date + "'," + months + ");");


                Query = @"select sno,monthname,monthtotal from cashflowsummary order by sno;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objCashflow = new LAReportsDTO();
                        objCashflow.pCashflowSno = dr["sno"];
                        objCashflow.pperticulars = dr["monthname"];
                        objCashflow.pcashamtsummary = dr["monthtotal"];
                        lstCashflow.Add(objCashflow);

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCashflow;
        }
        public List<CashFlowDetailsDTO> GetCashFlowDetails(string Asonmonth, string month, string Connectionstring)
        {
            List<CashFlowDetailsDTO> lstCashflowdetails = new List<CashFlowDetailsDTO>();
            try
            {
                Asonmonth = "01-" + Asonmonth;
                string Query = string.Empty;
                if (month.ToUpper() == "ALL")
                {
                    Query = @"select fdaccountno,chitbranchname,membername,fdname,tenure,interestpayout,depositamount,interestpayable,maturityamount,monthlyint,depositdate,maturitydate,squareyard,caltype,interestrate,monthname from cashflowdetails order by chitbranchname,depositdate;";
                }
                else if (month.ToUpper().Contains("UPTO"))
                {
                    Query = @"select fdaccountno,chitbranchname,membername,fdname,tenure,interestpayout,depositamount,interestpayable,maturityamount,monthlyint,depositdate,maturitydate,squareyard,caltype,interestrate,monthname from cashflowdetails where CAST('01-'||monthname AS DATE)<'" + Asonmonth + "' order by chitbranchname,depositdate;";
                }
                else
                {
                    Query = @"select fdaccountno,chitbranchname,membername,fdname,tenure,interestpayout,depositamount,interestpayable,maturityamount,monthlyint,depositdate,maturitydate,squareyard,caltype,interestrate,monthname from cashflowdetails where monthname='" + month + "' order by chitbranchname,depositdate;";

                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        CashFlowDetailsDTO objCashflowdetails = new CashFlowDetailsDTO();
                        objCashflowdetails.pMonthname = dr["monthname"];
                        objCashflowdetails.pFdaccountno = dr["fdaccountno"];
                        objCashflowdetails.pChitbranchname = dr["chitbranchname"];
                        objCashflowdetails.pMembername = dr["membername"];
                        objCashflowdetails.pSchemename = dr["fdname"];
                        objCashflowdetails.pTenure = dr["tenure"];
                        objCashflowdetails.pInterestpayout = dr["interestpayout"];
                        objCashflowdetails.pDepositamount = dr["depositamount"];
                        objCashflowdetails.pInterestpayable = dr["interestpayable"];
                        objCashflowdetails.pMaturityamount = dr["maturityamount"];
                        objCashflowdetails.pMonthlyint = dr["monthlyint"];
                        objCashflowdetails.pDepositDate = dr["depositdate"];
                        objCashflowdetails.pMaturityDate = dr["maturitydate"];
                        objCashflowdetails.pSquareyard = dr["squareyard"];
                        objCashflowdetails.pCaltype = dr["caltype"];
                        objCashflowdetails.pInterestrate = dr["interestrate"];
                        lstCashflowdetails.Add(objCashflowdetails);

                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCashflowdetails;
        }

        public List<CashFlowPerticularDetailsDTO> GetCashFlowPerticularsDetails(string perticulars, string Asonmonth, string Connectionstring)
        {
            List<CashFlowPerticularDetailsDTO> lstCashflowdetails = new List<CashFlowPerticularDetailsDTO>();
            try
            {
                Asonmonth = "01-" + Asonmonth;
                string Query = string.Empty;
                //Cash
                if (perticulars == "Cash")
                {
                    Query = @"SELECT transactionno,transactiondate,Y.accountname,Y.parentaccountname,particulars,debitamount,creditamount FROM  (select * from tblmstaccounts where accountname='CASH ON HAND') x, tbltranstotaltransactions y where x.accountid=y.accountid AND to_char(CAST(transactiondate AS DATE),'DD-MON-YYYY')::DATE < '" + Asonmonth + "' order by transactiondate;";
                }
                else if (perticulars == "Cheques on Hand")
                {
                    Query = @"SELECT transactionno,transactiondate,Y.accountname,Y.parentaccountname,particulars,debitamount,creditamount FROM  (select * from tblmstaccounts where accountname='UNCLEARED CHEQUES A/C') x, tbltranstotaltransactions y where x.accountid=y.accountid AND to_char(CAST(transactiondate AS DATE),'DD-MON-YYYY')::DATE < '" + Asonmonth + "' order by transactiondate;";
                }
                else if (perticulars == "Fixed Deposit Receipts")
                {
                    Query = @"SELECT transactionno,transactiondate,Y.accountname,Y.parentaccountname,particulars,debitamount,creditamount FROM  (select * from tblmstaccounts where accountname='FIXED DEPOSIT RECEIPT') x, tbltranstotaltransactions y where x.accountid=y.accountid AND to_char(CAST(transactiondate AS DATE),'DD-MON-YYYY')::DATE < '" + Asonmonth + "' order by transactiondate;";

                }
                else if (perticulars == "Bank Balances")
                {
                    Query = @"SELECT transactionno,transactiondate,Y.accountname,Y.parentaccountname,particulars,debitamount,creditamount FROM  (select * from tblmstbank a, tblmstaccounts b where b.accountid=a.bankaccountid) x, tbltranstotaltransactions y where x.accountid=y.accountid AND to_char(CAST(transactiondate AS DATE),'DD-MON-YYYY')::DATE < '" + Asonmonth + "' order by transactiondate";
                }
                else if (perticulars == "Chits Details")
                {
                    Query = @"SELECT transactionno,transactiondate,Y.accountname,Y.parentaccountname,particulars,debitamount,creditamount FROM  (select * from tblmstaccounts where accountname='CHITS A/C') x, tbltranstotaltransactions y where x.accountid=y.accountid AND to_char(CAST(transactiondate AS DATE),'DD-MON-YYYY')::DATE < '" + Asonmonth + "' order by transactiondate;";

                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        CashFlowPerticularDetailsDTO objCashflowdetails = new CashFlowPerticularDetailsDTO();
                        objCashflowdetails.pTransactionno = dr["transactionno"];
                        objCashflowdetails.pTransdate = dr["transactiondate"];
                        objCashflowdetails.pAccountname = dr["accountname"];
                        objCashflowdetails.pParentaccountname = dr["parentaccountname"];
                        objCashflowdetails.pPerticulars = dr["particulars"];
                        objCashflowdetails.pDebitamount = dr["debitamount"];
                        objCashflowdetails.pCreditamt = dr["creditamount"];
                        lstCashflowdetails.Add(objCashflowdetails);

                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCashflowdetails;
        }
        #endregion

        #region Target Report ...
        public List<TargetReportDTO> GetTargetReportSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate, string Connectionstring)
        {
            List<TargetReportDTO> lstTargetReport = new List<TargetReportDTO>();

            try
            {
                string Query = string.Empty;


                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select FN_GET_TARGET('" + FormatDate(receiptfromdate) + "','" + FormatDate(receipttodate) + "','" + FormatDate(chequefromdate) + "','" + FormatDate(chequetodate) + "');");


                Query = @"select coalesce(sum(receipt_amount),0) as receipt_amount,coalesce(sum(premature_amount),0)as premature_amount,branch,coalesce(sum(receipt_amount),0) -coalesce(sum(premature_amount),0) AS achived from temp_target group by branch;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        TargetReportDTO objTargetReport = new TargetReportDTO();
                        objTargetReport.preceiptamountsummary = dr["receipt_amount"];
                        objTargetReport.pprematureamoutsummary = dr["premature_amount"];
                        objTargetReport.pbranchsummary = dr["branch"];
                        objTargetReport.pachivedsummary = dr["achived"];
                        lstTargetReport.Add(objTargetReport);

                    }

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstTargetReport;
        }
        public List<TargetReportDTO> GetTargetReportDetails(string branch, string Connectionstring)
        {
            List<TargetReportDTO> lstTargetreport = new List<TargetReportDTO>();
            try
            {
                string Query = string.Empty;
                if (branch.ToUpper() == "ALL")
                {
                    Query = @"select target_month, account_no, account_name, receipt_amount, premature_amount, branch ,  receipt_type,receipt_date,cleardate  from temp_target ORDER BY branch,account_no;";

                }
                else
                {
                    Query = @"select target_month, account_no, account_name, receipt_amount, premature_amount, branch ,  receipt_type,receipt_date,cleardate  from temp_target where branch='" + ManageQuote(branch) + "' ORDER BY branch,account_no;";

                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        TargetReportDTO objTargetreport = new TargetReportDTO();
                        objTargetreport.pTargetmonth = dr["target_month"];
                        objTargetreport.pAccountno = dr["account_no"];
                        objTargetreport.pAccountname = dr["account_name"];
                        objTargetreport.pReceiptamount = dr["receipt_amount"];
                        objTargetreport.pPreMaturityamount = dr["premature_amount"];
                        objTargetreport.pbranch = dr["branch"];
                        objTargetreport.pReceipttype = dr["receipt_type"];
                        objTargetreport.pReceiptDate = dr["receipt_date"];
                        objTargetreport.pClearDate = dr["cleardate"];
                        lstTargetreport.Add(objTargetreport);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstTargetreport;
        }
        #endregion


        #region Agent Points ...

        public List<LAReportsDTO> GetAgentPointsSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate, string Connectionstring)
        {
            lstAgentPoints = new List<LAReportsDTO>();

            try
            {
                string Query = string.Empty;

                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select FN_GET_AGENTPOINTS('" + FormatDate(receiptfromdate) + "','" + FormatDate(receipttodate) + "','" + FormatDate(chequefromdate) + "','" + FormatDate(chequetodate) + "')");
                Query = @"select agentid, agentenramtpts from agent_points_summary where agentenramtpts>0 order by agentid;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objAgentPoint = new LAReportsDTO();
                        objAgentPoint.pAgentnamesummary = dr["agentid"];
                        objAgentPoint.pPointsummary = dr["agentenramtpts"];
                        lstAgentPoints.Add(objAgentPoint);

                    }

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAgentPoints;
        }

        public List<AgentPointsDetailsDTO> GetAgentPointsDetails(string agentname, string Connectionstring)
        {
            List<AgentPointsDetailsDTO> lstAgentPoints = new List<AgentPointsDetailsDTO>();

            try
            {
                string Query = string.Empty;
                if (agentname == "ALL")
                {
                    Query = @"select x.referral_name,x.referralid,x.account_no,x.account_name,x.receipt_amount,x.branch,x.receipt_type,x.receipt_date,x.cleardate from
(select x.*,y.referralid||'-'||y.referralname as referral_name,referralid,to_char(cleardate,'MON-YYYY') as vchmonth from tempagent_points X LEFT JOIN
(select distinct fdaccountno,referralid,referralname from tbltransfdreferraldetails) y on x.account_no=y.fdaccountno) x,
(select * from agent_points_summary where agentenramtpts>0) y where  x.referral_name=y.agentid order by x.referral_name;";

                }
                else
                {
                    Query = @"select x.referral_name,x.referralid,x.account_no,x.account_name,x.receipt_amount,x.branch,x.receipt_type,x.receipt_date,x.cleardate from
(select x.*,y.referralid||'-'||y.referralname as referral_name,referralid,to_char(cleardate,'MON-YYYY') as vchmonth from tempagent_points X LEFT JOIN
(select distinct fdaccountno,referralid,referralname from tbltransfdreferraldetails) y on x.account_no=y.fdaccountno) x,
(select * from agent_points_summary where agentenramtpts>0) y where x.referral_name='" + agentname + "' and x.referral_name=y.agentid order by x.referral_name;";

                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        AgentPointsDetailsDTO objAgentPoint = new AgentPointsDetailsDTO();
                        objAgentPoint.pAgentname = dr["referral_name"];
                        objAgentPoint.pContactid = dr["referralid"];
                        objAgentPoint.pFdaccountno = dr["account_no"];
                        objAgentPoint.pAccountname = dr["account_name"];
                        objAgentPoint.pReceiptamount = dr["receipt_amount"];
                        objAgentPoint.pBranch = dr["branch"];
                        objAgentPoint.pReceipttype = dr["receipt_type"];
                        objAgentPoint.pReceiptDate = dr["receipt_date"];
                        objAgentPoint.pClearDate = dr["cleardate"];
                        lstAgentPoints.Add(objAgentPoint);

                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAgentPoints;
        }
        #endregion


        #region Interest Payment Report...
        public List<LAReportsDTO> GetInterestreportscheme(string Connectionstring)
        {

            List<LAReportsDTO> lstInterestscheme = new List<LAReportsDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select distinct fdconfigid as schemeid,fdname as schemename from tbltransfdcreation where fdaccountid in (select distinct trans_type_id from interest_payments) order by fdname;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objInterest = new LAReportsDTO();
                        objInterest.pSchemeId = dr["schemeid"];
                        objInterest.pSchemename = dr["schemename"];
                        lstInterestscheme.Add(objInterest);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestscheme;
        }

        public List<LAReportsDTO> GetInterestreportfdaccountnos(string paymenttype, string company, string brnach, long schemeid, string Connectionstring)
        {

            List<LAReportsDTO> lstInterestscheme = new List<LAReportsDTO>();
            try
            {
                string Query = string.Empty;

                if (paymenttype.ToUpper() == "SELF")
                {
                    if (schemeid == 0)
                    {
                        Query = @"select fdaccountno,fdaccountno||'-'||membername as membername from tbltransfdcreation where fdaccountid in (select distinct trans_type_id from interest_payments) order by membername;";

                    }
                    else
                    {
                        Query = @"select fdaccountno,fdaccountno||'-'||membername as membername from tbltransfdcreation where fdconfigid=" + schemeid + " order by membername;";

                    }

                }
                else if (paymenttype.ToUpper() == "ADJUSTMENT")
                {
                    if (schemeid == 0)
                    {
                        Query = @"select A.fdaccountno,A.fdaccountno||'-'||A.membername as membername from tbltransfdcreation A left join tabbranchcodes B 
on A.chitbranchid::numeric=B.code and A.chitbranchname=B.branchname
where A.fdaccountid in (select distinct trans_type_id from interest_payments) and A.chitbranchname='" + ManageQuote(brnach) + "' and B.companyname='" + ManageQuote(company) + "' order by membername;";

                    }
                    else
                    {
                        Query = @"select A.fdaccountno,A.fdaccountno||'-'||A.membername as membername from tbltransfdcreation A left join tabbranchcodes B 
on A.chitbranchid::numeric=B.code and A.chitbranchname=B.branchname
where A.chitbranchname='" + ManageQuote(brnach) + "' and B.companyname='" + ManageQuote(company) + "' and  fdconfigid=" + schemeid + " order by membername;";

                    }

                }


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objInterest = new LAReportsDTO();
                        objInterest.pFdAccNo = dr["fdaccountno"];
                        objInterest.pFdAccNoMembername = dr["membername"];
                        lstInterestscheme.Add(objInterest);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestscheme;
        }
        #endregion

        #region Commission Payment Report...


        #endregion

        #region Maturity Intimation Report...
        public List<LAReportsDTO> GetMaturityscheme(string Connectionstring)
        {

            List<LAReportsDTO> lstMaturityscheme = new List<LAReportsDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select distinct fdconfigid as schemeid,fdname as schemename from tbltransfdcreation where accountstatus='N' order by fdname;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objMaturity = new LAReportsDTO();
                        objMaturity.pSchemeId = dr["schemeid"];
                        objMaturity.pSchemename = dr["schemename"];
                        lstMaturityscheme.Add(objMaturity);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityscheme;
        }
        public List<LAReportsDTO> GetMaturitybrnach(long schemeid, string Connectionstring)
        {

            List<LAReportsDTO> lstMaturitybranch = new List<LAReportsDTO>();
            try
            {
                string Query = string.Empty;

                if (schemeid == 0)
                {
                    Query = @"select distinct chitbranchname as branch from tbltransfdcreation where accountstatus='N' order by chitbranchname;";
                }
                else
                {
                    Query = @"select distinct chitbranchname as branch from tbltransfdcreation where accountstatus='N' and fdconfigid=" + schemeid + " order by chitbranchname;";
                }


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objMaturity = new LAReportsDTO();
                        objMaturity.pbranch = dr["branch"];
                        lstMaturitybranch.Add(objMaturity);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturitybranch;
        }
        public bool SaveMaturityIntimationReport(LAReportsDTO _MaturityIntimationDTO, string Connectionstring)

        {
            bool IsSaved = false;
            string Query = string.Empty;
            try
            {
                if (_MaturityIntimationDTO.pmaturityintimationlist != null)
                {
                    for (int i = 0; i < _MaturityIntimationDTO.pmaturityintimationlist.Count; i++)
                    {
                        Query = "INSERT INTO maturityintimationreport(printdate, fdaccount_no) values (current_date,'" + _MaturityIntimationDTO.pmaturityintimationlist[i].pfdaccountno + "');";
                        NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, Query.ToString());

                    }
                    IsSaved = true;
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return IsSaved;

        }
        public List<MaturityIntimationDTO> ShowMaturityIntimationReport(long schemeid, string branchname, string fromdate, string todate, string Connectionstring)
        {

            List<MaturityIntimationDTO> lstMaturityDetails = new List<MaturityIntimationDTO>();
            try
            {
                string Query = string.Empty;

                if (schemeid == 0 && branchname.ToUpper() == "ALL")
                {
                    //Query = @"select membername, fdaccountno,to_char(transdate, 'dd/Mon/yyyy') as transdate,fdname as schemename,chitbranchname as branchname, tenor|| ' ' || tenortype as tenor,depositamount,interestrate,maturityamount,to_char(depositdate, 'dd/Mon/yyyy') as depositdate,to_char(maturitydate, 'dd/Mon/yyyy') as maturitydate from tbltransfdcreation where maturitydate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and accountstatus='N'";
                    Query = @"select membername, fdaccountno,to_char(transdate, 'dd/Mon/yyyy') as transdate,fdname as schemename,chitbranchname as branchname, tenor|| ' ' || tenortype as tenor,depositamount,interestrate,maturityamount,to_char(depositdate, 'dd/Mon/yyyy') as depositdate,to_char(maturitydate, 'dd/Mon/yyyy') as maturitydate from vwfdtransaction_details where balanceamount<=0 and  maturitydate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and accountstatus='N'";

                }
                else if (schemeid == 0 && branchname.ToUpper() != "ALL")
                {
                    Query = @"select membername, fdaccountno,to_char(transdate, 'dd/Mon/yyyy') as transdate,fdname as schemename,chitbranchname as branchname, tenor|| ' ' || tenortype as tenor,depositamount,interestrate,maturityamount,to_char(depositdate, 'dd/Mon/yyyy') as depositdate,to_char(maturitydate, 'dd/Mon/yyyy') as maturitydate from vwfdtransaction_details where balanceamount<=0   maturitydate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and chitbranchname = '" + ManageQuote(branchname) + "' and accountstatus='N'";

                }
                else if (schemeid != 0 && branchname.ToUpper() == "ALL")
                {
                    Query = @"select membername, fdaccountno,to_char(transdate, 'dd/Mon/yyyy') as transdate,fdname as schemename,chitbranchname as branchname, tenor|| ' ' || tenortype as tenor,depositamount,interestrate,maturityamount,to_char(depositdate, 'dd/Mon/yyyy') as depositdate,to_char(maturitydate, 'dd/Mon/yyyy') as maturitydate from vwfdtransaction_details where balanceamount<=0  maturitydate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and fdconfigid = " + schemeid + " and accountstatus='N'";

                }
                else if (schemeid != 0 && branchname.ToUpper() != "ALL")
                {
                    Query = @"select membername, fdaccountno,to_char(transdate, 'dd/Mon/yyyy') as transdate,fdname as schemename,chitbranchname as branchname, tenor|| ' ' || tenortype as tenor,depositamount,interestrate,maturityamount,to_char(depositdate, 'dd/Mon/yyyy') as depositdate,to_char(maturitydate, 'dd/Mon/yyyy') as maturitydate from vwfdtransaction_details where balanceamount<=0  maturitydate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and fdconfigid = " + schemeid + " and chitbranchname = '" + ManageQuote(branchname) + "' and accountstatus='N'";
                }

                // Start Letter purpose Code
                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "delete from tempmaturityintimationletter;");
                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "insert into tempmaturityintimationletter(" + Query + ")");
                // End Letter purpose

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityIntimationDTO objMaturity = new MaturityIntimationDTO();
                        objMaturity.pmembername = dr["membername"];
                        objMaturity.pfdaccountno = dr["fdaccountno"];
                        objMaturity.pschemename = dr["schemename"];
                        objMaturity.pbranchname = dr["branchname"];
                        objMaturity.ptenor = dr["tenor"];
                        objMaturity.pdepositamount = dr["depositamount"];
                        objMaturity.pinterestrate = dr["interestrate"];
                        objMaturity.pmaturityamount = dr["maturityamount"];
                        objMaturity.pdepositdate = dr["depositdate"];
                        objMaturity.pmaturitydate = dr["maturitydate"];
                        lstMaturityDetails.Add(objMaturity);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityDetails;
        }

        public List<MaturityIntimationDTO> GetMaturityIntimationLetter(string FdAccountNo, string Connectionstring)
        {

            List<MaturityIntimationDTO> lstMaturityDetails = new List<MaturityIntimationDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select schemename,membername,fdaccountno,branchname,depositamount,maturityamount,depositdate,maturitydate from tempmaturityintimationletter where fdaccountno in ('" + FdAccountNo + "') order by schemename;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityIntimationDTO objMaturity = new MaturityIntimationDTO();
                        objMaturity.pmembername = dr["membername"];
                        objMaturity.pfdaccountno = dr["fdaccountno"];
                        objMaturity.pschemename = dr["schemename"];
                        objMaturity.pbranchname = dr["branchname"];
                        objMaturity.pdepositamount = dr["depositamount"];
                        objMaturity.pmaturityamount = dr["maturityamount"];
                        objMaturity.pdepositdate = dr["depositdate"];
                        objMaturity.pmaturitydate = dr["maturitydate"];
                        lstMaturityDetails.Add(objMaturity);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityDetails;
        }
        #endregion

        #region Lien Release Report...
        public List<LAReportsDTO> GetLienbrnach(string Connectionstring)
        {
            List<LAReportsDTO> lstLienbranch = new List<LAReportsDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select distinct companybranch as branch from tbltranslienentry  where lienstatus='Y' order by companybranch;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objLien = new LAReportsDTO();
                        objLien.pbranch = dr["branch"];
                        lstLienbranch.Add(objLien);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLienbranch;
        }
        public List<LienReleaseDTO> ShowLienReleaseReport(string branchname, string fromdate, string todate, string Connectionstring)
        {
            List<LienReleaseDTO> lstLienReleaseDetails = new List<LienReleaseDTO>();
            try
            {
                string Query = string.Empty;
                if (branchname.ToUpper() == "ALL")
                {
                    Query = @"  select to_char(A.liendate, 'dd/Mon/yyyy') as liendate,A.fdaccountno,lienamount,c.membername,C.fdname as schemename,C.depositamount,C.maturityamount,to_char(C.depositdate, 'dd/Mon/yyyy') as depositdate,to_char(C.maturitydate, 'dd/Mon/yyyy') as maturitydate,C.interesttype,C.interestrate,C.tenor||' '||C.tenortype as tenor,A.companybranch,to_char(B.lienrealsedate, 'dd/Mon/yyyy') as lienrealsedate from (select * from tbltranslienentry where lienstatus='Y')A 
     left join  tbltranslienrealse B on A.lienid=B.lienid 
     left join  tbltransfdcreation C on A.fdaccountno=C.fdaccountno where B.lienrealsedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "';";
                }
                else
                {
                    Query = @"  select to_char(A.liendate, 'dd/Mon/yyyy') as liendate,A.fdaccountno,lienamount,c.membername,C.fdname as schemename,C.depositamount,C.maturityamount,to_char(C.depositdate, 'dd/Mon/yyyy') as depositdate,to_char(C.maturitydate, 'dd/Mon/yyyy') as maturitydate,C.interesttype,C.interestrate,C.tenor||' '||C.tenortype as tenor,A.companybranch,to_char(B.lienrealsedate, 'dd/Mon/yyyy') as lienrealsedate from (select * from tbltranslienentry where lienstatus='Y')A 
     left join  tbltranslienrealse B on A.lienid=B.lienid 
     left join  tbltransfdcreation C on A.fdaccountno=C.fdaccountno where B.lienrealsedate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and companybranch='" + ManageQuote(branchname) + "';";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LienReleaseDTO objLienRelease = new LienReleaseDTO();
                        objLienRelease.pliendate = dr["liendate"];
                        objLienRelease.plienreleasedate = dr["lienrealsedate"];
                        objLienRelease.plienamount = dr["lienamount"];
                        objLienRelease.pmembername = dr["membername"];
                        objLienRelease.pfdaccountno = dr["fdaccountno"];
                        objLienRelease.pschemename = dr["schemename"];
                        objLienRelease.pbranchname = dr["companybranch"];
                        objLienRelease.ptenor = dr["tenor"];
                        objLienRelease.pdepositamount = dr["depositamount"];
                        objLienRelease.pinterestrate = dr["interestrate"];
                        objLienRelease.pmaturityamount = dr["maturityamount"];
                        objLienRelease.pdepositdate = dr["depositdate"];
                        objLienRelease.pmaturitydate = dr["maturitydate"];
                        lstLienReleaseDetails.Add(objLienRelease);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLienReleaseDetails;
        }
        #endregion

        #region Self Adjustment Report...
        public List<SelfAdjustmentDTO> ShowSelfAdjustmentReport(string paymenttype, string companyname, string branchname, string fromdate, string todate, string Connectionstring)
        {

            List<SelfAdjustmentDTO> lstSelfAdjustmentDetails = new List<SelfAdjustmentDTO>();
            try
            {
                string Query = string.Empty;
                if (paymenttype.ToUpper() == "SELF")
                {

                    //Query = @"select distinct A.fdaccountno,A.membername,A.fdname as schemename,A.tenor||' '||A.tenortype as tenor,A.depositamount,to_char(A.depositdate, 'dd/Mon/yyyy') as depositdate,A.maturityamount,to_char(A.maturitydate, 'dd/Mon/yyyy') as maturitydate,A.chitbranchname as branch,A.interestrate,B.payment_type from tbltransfdcreation A join self_or_adjustment B on A.fdaccountid=B.fd_account_id where upper(B.payment_type)=upper('" + ManageQuote(paymenttype) + "') and A.accountstatus='N' and to_char(B.self_or_adjustment_date, 'Mon-yyyy') between '" + fromdate + "' and '" + todate + "';";

                    Query = @"select distinct A.fdaccountno,A.membername,A.fdname as schemename,A.tenor||' '||A.tenortype as tenor,A.depositamount,to_char(A.depositdate, 'dd/Mon/yyyy') as depositdate,A.maturityamount,to_char(A.maturitydate, 'dd/Mon/yyyy') as maturitydate,A.chitbranchname as branch,A.interestrate,B.payment_type from tbltransfdcreation A join self_or_adjustment B on A.fdaccountid=B.fd_account_id where upper(B.payment_type)=upper('" + ManageQuote(paymenttype) + "') and A.accountstatus='N' and to_char(B.self_or_adjustment_date, 'yyyy-MM-dd') between '" + fromdate + "' and '" + todate + "' ;";

                }
                else
                {
                    if (companyname.ToUpper() == "ALL" && branchname.ToUpper() == "ALL")
                    {
                        Query = @"select distinct A.fdaccountno,A.membername,A.fdname as schemename,A.tenor||' '||A.tenortype as tenor,A.depositamount,to_char(A.depositdate, 'dd/Mon/yyyy') as depositdate,A.maturityamount,to_char(A.maturitydate, 'dd/Mon/yyyy') as maturitydate,A.chitbranchname as branch,A.interestrate,B.payment_type from tbltransfdcreation A join self_or_adjustment B on A.fdaccountid=B.fd_account_id where upper(B.payment_type)=upper('" + ManageQuote(paymenttype) + "') and A.accountstatus='N' and to_char(B.self_or_adjustment_date, 'yyyy-MM-dd') between '" + fromdate + "' and '" + todate + "';";

                    }
                    else if (companyname.ToUpper() != "ALL" && branchname.ToUpper() == "ALL")
                    {
                        Query = @"select distinct A.fdaccountno,A.membername,A.fdname as schemename,A.tenor||' '||A.tenortype as tenor,A.depositamount,to_char(A.depositdate, 'dd/Mon/yyyy') as depositdate,A.maturityamount,to_char(A.maturitydate, 'dd/Mon/yyyy') as maturitydate,A.chitbranchname as branch,A.interestrate,B.payment_type from tbltransfdcreation A join self_or_adjustment B on A.fdaccountid=B.fd_account_id where upper(B.payment_type)=upper('" + ManageQuote(paymenttype) + "') and A.accountstatus='N' and to_char(B.self_or_adjustment_date, 'yyyy-MM-dd') between '" + fromdate + "' and '" + todate + "' and B.company_name= '" + ManageQuote(companyname) + "';";

                    }
                    else if (companyname.ToUpper() != "ALL" && branchname.ToUpper() != "ALL")
                    {
                        Query = @"select distinct A.fdaccountno,A.membername,A.fdname as schemename,A.tenor||' '||A.tenortype as tenor,A.depositamount,to_char(A.depositdate, 'dd/Mon/yyyy') as depositdate,A.maturityamount,to_char(A.maturitydate, 'dd/Mon/yyyy') as maturitydate,A.chitbranchname as branch,A.interestrate,B.payment_type from tbltransfdcreation A join self_or_adjustment B on A.fdaccountid=B.fd_account_id where upper(B.payment_type)=upper('" + ManageQuote(paymenttype) + "') and A.accountstatus='N' and to_char(B.self_or_adjustment_date, 'yyyy-MM-dd') between '" + fromdate + "' and '" + todate + "' and B.company_name= '" + ManageQuote(companyname) + "' and B.branch_name= '" + ManageQuote(branchname) + "';";

                    }


                }



                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        SelfAdjustmentDTO objSelfAdjustment = new SelfAdjustmentDTO();
                        objSelfAdjustment.pmembername = dr["membername"];
                        objSelfAdjustment.pfdaccountno = dr["fdaccountno"];
                        objSelfAdjustment.pschemename = dr["schemename"];
                        objSelfAdjustment.pbranchname = dr["branch"];
                        objSelfAdjustment.ptenor = dr["tenor"];
                        objSelfAdjustment.pdepositamount = dr["depositamount"];
                        objSelfAdjustment.pinterestrate = dr["interestrate"];
                        objSelfAdjustment.pmaturityamount = dr["maturityamount"];
                        objSelfAdjustment.pdepositdate = dr["depositdate"];
                        objSelfAdjustment.pmaturitydate = dr["maturitydate"];
                        lstSelfAdjustmentDetails.Add(objSelfAdjustment);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstSelfAdjustmentDetails;
        }
        public List<LAReportsDTO> GetSelfAdjustmentcompany(string Connectionstring)
        {
            List<LAReportsDTO> lstCompanyDetails = new List<LAReportsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct company_name from self_or_adjustment where upper(payment_type)='ADJUSTMENT'  order by company_name;"))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objCompanydetails = new LAReportsDTO();
                        objCompanydetails.pcompanyname = dr["company_name"];
                        lstCompanyDetails.Add(objCompanydetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstCompanyDetails;
        }

        public List<LAReportsDTO> GetSelfAdjustmentbrnach(string companyname, string Connectionstring)
        {
            List<LAReportsDTO> lstBranchDetails = new List<LAReportsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct branch_name from self_or_adjustment  where company_name='" + ManageQuote(companyname) + "'  order by branch_name;"))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objBranchdetails = new LAReportsDTO();
                        objBranchdetails.pbranch = dr["branch_name"];
                        lstBranchDetails.Add(objBranchdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstBranchDetails;
        }
        #endregion

        #region Maturity Trend Report...
        public List<MaturityTrendDTO> ShowMaturityTrendGridHeader(string Connectionstring)
        {
            List<MaturityTrendDTO> lstMaturityTrendDetails = new List<MaturityTrendDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"SELECT mthreport.schemename, 'Count' as Count,mthreport.curmn, mthreport.curmn1, mthreport.curmn2, mthreport.curmn3, mthreport.curmn4, mthreport.curmn5, mthreport.curmn6, mthreport.curmn7, mthreport.curmn8, mthreport.curmn9, mthreport.curmn10, mthreport.curmn11, mthreport.curmn12
FROM crosstab('SELECT ''Scheme Name'' As Monthname,monthname::text As row_name,monthname::text As Numunits FROM vwcurrentplus12months'::text, 'select * from vwcurrentplus12months'::text) mthreport(schemename text, curmn character varying(25), curmn1 character varying(25), curmn2 character varying(25), curmn3 character varying(25), curmn4 character varying(25), curmn5 character varying(25), curmn6 character varying(25), curmn7 character varying(25), curmn8 character varying(25), curmn9 character varying(25), curmn10 character varying(25), curmn11 character varying(25), curmn12 character varying(25));";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityTrendDTO objMaturityTrend = new MaturityTrendDTO();
                        objMaturityTrend.pschemename = dr["schemename"];
                        //objMaturityTrend.pCount = dr["Count"];
                        objMaturityTrend.pcurrentmonth = dr["curmn"];
                        objMaturityTrend.pcurrentmonth1 = dr["curmn1"];
                        objMaturityTrend.pcurrentmonth2 = dr["curmn2"];
                        objMaturityTrend.pcurrentmonth3 = dr["curmn3"];
                        objMaturityTrend.pcurrentmonth4 = dr["curmn4"];
                        objMaturityTrend.pcurrentmonth5 = dr["curmn5"];
                        objMaturityTrend.pcurrentmonth6 = dr["curmn6"];
                        objMaturityTrend.pcurrentmonth7 = dr["curmn7"];
                        objMaturityTrend.pcurrentmonth8 = dr["curmn8"];
                        objMaturityTrend.pcurrentmonth9 = dr["curmn9"];
                        objMaturityTrend.pcurrentmonth10 = dr["curmn10"];
                        objMaturityTrend.pcurrentmonth11 = dr["curmn11"];
                        objMaturityTrend.pcurrentmonth12 = dr["curmn12"];


                        lstMaturityTrendDetails.Add(objMaturityTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityTrendDetails;
        }
        public List<MaturityTrendDTO> ShowMaturityTrendReport(string Connectionstring)
        {
            List<MaturityTrendDTO> lstMaturityTrendDetails = new List<MaturityTrendDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"SELECT mthreport.schemename,Count, coalesce(mthreport.curmn, '0')::numeric as cummon ,coalesce(mthreport.curmn1, '0')::numeric as cummon1 ,
coalesce(mthreport.curmn2, '0')::numeric as cummon2 ,coalesce(mthreport.curmn3, '0')::numeric as cummon3 ,coalesce(mthreport.curmn4, '0')::numeric as cummon4 ,
coalesce(mthreport.curmn5, '0')::numeric as cummon5 ,coalesce(mthreport.curmn6, '0')::numeric as cummon6 ,coalesce(mthreport.curmn7, '0')::numeric as cummon7 ,
coalesce(mthreport.curmn8, '0')::numeric as cummon8 ,coalesce(mthreport.curmn9, '0')::numeric as cummon9 ,coalesce(mthreport.curmn10, '0')::numeric as cummon10 ,
coalesce(mthreport.curmn11, '0')::numeric as cummon11 ,coalesce(mthreport.curmn12, '0')::numeric as cummon12  FROM crosstab('SELECT A.fdname::text As row_name,
count(A.fdaccountid)as Count,MONTHNAME,coalesce(sum(A.maturityamount),0) As maturityamt FROM
(select *,TO_CHAR(CAST(A.maturitydate AS DATE),''MON-YYYY'') As Monthname from tbltransfdcreation A join vwfdreceiptsfinal B on A.fdaccountid = B.fd_account_id AND
A.maturitydate BETWEEN cast(''01-''||to_char(CURRENT_DATE,''MON-YYYY'') as date) and
(date_trunc(''month'', cast(''01-''||to_char(CURRENT_DATE,''MON-YYYY'') as date) + cast(''12 months'' as interval)) + interval ''1 months'' - interval ''1 day'' )::date and  accountstatus=''N''
) AS A group by A.fdname,monthname '::text, 'select* from vwcurrentplus12months'::text)
mthreport(schemename text, Count character varying(25), curmn character varying(25), curmn1 character varying(25), curmn2 character varying(25),
curmn3 character varying(25), curmn4 character varying(25), curmn5 character varying(25), curmn6 character varying(25), curmn7 character varying(25),
curmn8 character varying(25), curmn9 character varying(25), curmn10 character varying(25), curmn11 character varying(25), curmn12 character varying(25)) order by schemename;";

                //                Query = @"SELECT mthreport.schemename,Count, coalesce(mthreport.curmn,'0')::numeric as cummon ,coalesce(mthreport.curmn1,'0')::numeric as cummon1 ,coalesce(mthreport.curmn2,'0')::numeric as cummon2 ,coalesce(mthreport.curmn3,'0')::numeric as cummon3 ,coalesce(mthreport.curmn4,'0')::numeric as cummon4 ,coalesce(mthreport.curmn5,'0')::numeric as cummon5 ,coalesce(mthreport.curmn6,'0')::numeric as cummon6 ,coalesce(mthreport.curmn7,'0')::numeric as cummon7 ,coalesce(mthreport.curmn8,'0')::numeric as cummon8 ,coalesce(mthreport.curmn9,'0')::numeric as cummon9 ,coalesce(mthreport.curmn10,'0')::numeric as cummon10 ,coalesce(mthreport.curmn11,'0')::numeric as cummon11 ,coalesce(mthreport.curmn12,'0')::numeric as cummon12  FROM crosstab('SELECT fdname::text As row_name,count(fdaccountid)as Count, TO_CHAR(CAST(maturitydate AS DATE),''MON-YYYY'') As Monthname,coalesce(sum(maturityamount),0) As maturityamt FROM 
                //tbltransfdcreation where accountstatus=''N'' and maturitydate BETWEEN cast(CURRENT_DATE as date) and cast((CURRENT_DATE + interval ''13 months'') as date) group by fdname,TO_CHAR(maturitydate,''MON-YYYY'') order by fdname,cast(''01-''||TO_CHAR(CAST(maturitydate AS DATE),''MON-YYYY'') as date) desc'::text, 'select * from vwcurrentplus12months'::text) mthreport (schemename text, Count character varying(25), curmn character varying(25), curmn1 character varying(25), curmn2 character varying(25), curmn3 character varying(25), curmn4 character varying(25), curmn5 character varying(25), curmn6 character varying(25), curmn7 character varying(25), curmn8 character varying(25), curmn9 character varying(25), curmn10 character varying(25), curmn11 character varying(25), curmn12 character varying(25)) order by schemename;";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityTrendDTO objMaturityTrend = new MaturityTrendDTO();
                        objMaturityTrend.pschemename = dr["schemename"];
                        //objMaturityTrend.pCount = dr["Count"];
                        objMaturityTrend.pcurrentmonth = dr["cummon"];
                        objMaturityTrend.pcurrentmonth1 = dr["cummon1"];
                        objMaturityTrend.pcurrentmonth2 = dr["cummon2"];
                        objMaturityTrend.pcurrentmonth3 = dr["cummon3"];
                        objMaturityTrend.pcurrentmonth4 = dr["cummon4"];
                        objMaturityTrend.pcurrentmonth5 = dr["cummon5"];
                        objMaturityTrend.pcurrentmonth6 = dr["cummon6"];
                        objMaturityTrend.pcurrentmonth7 = dr["cummon7"];
                        objMaturityTrend.pcurrentmonth8 = dr["cummon8"];
                        objMaturityTrend.pcurrentmonth9 = dr["cummon9"];
                        objMaturityTrend.pcurrentmonth10 = dr["cummon10"];
                        objMaturityTrend.pcurrentmonth11 = dr["cummon11"];
                        objMaturityTrend.pcurrentmonth12 = dr["cummon12"];


                        lstMaturityTrendDetails.Add(objMaturityTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityTrendDetails;
        }

        public List<MaturityTrendDetailsDTO> ShowShemeAndDatewiseDetails(string schemename, string maturitydate, string Connectionstring)
        {
            List<MaturityTrendDetailsDTO> lstMaturityDetails = new List<MaturityTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"select fdname as schemename,fdaccountid,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount,maturitydate from tbltransfdcreation  A join vwfdreceiptsfinal B on A.fdaccountid = B.fd_account_id where accountstatus='N' and fdname='" + ManageQuote(schemename) + "' and TO_CHAR(CAST(maturitydate AS DATE),'MON-YYYY')='" + maturitydate + "' order by maturitydate;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityTrendDetailsDTO objMaturityTrend = new MaturityTrendDetailsDTO();
                        objMaturityTrend.pSchemename1 = dr["schemename"];
                        objMaturityTrend.pFdAccountNo = dr["fdaccountno"];
                        objMaturityTrend.pTransdate = dr["transdate"];
                        objMaturityTrend.pMembercode = dr["membercode"];
                        objMaturityTrend.pMembername = dr["membername"];
                        objMaturityTrend.pTenor = dr["tenor"];
                        objMaturityTrend.pDepositamount = dr["depositamount"];
                        objMaturityTrend.pInterestrate = dr["interestrate"];
                        objMaturityTrend.pMaturityamount = dr["maturityamount"];
                        objMaturityTrend.pMaturitydate = dr["maturitydate"];
                        lstMaturityDetails.Add(objMaturityTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityDetails;
        }
        public List<MaturityTrendDetailsDTO> ShowGrandTotalDatewiseDetails(string maturitydate, string Connectionstring)
        {
            List<MaturityTrendDetailsDTO> lstMaturityDetails = new List<MaturityTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"select fdname as schemename,fdaccountid,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount, maturitydate from tbltransfdcreation A join vwfdreceiptsfinal B on A.fdaccountid = B.fd_account_id where accountstatus='N' and  TO_CHAR(CAST(maturitydate AS DATE),'MON-YYYY')='" + maturitydate + "' order by maturitydate,fdname;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityTrendDetailsDTO objMaturityTrend = new MaturityTrendDetailsDTO();
                        objMaturityTrend.pSchemename1 = dr["schemename"];
                        objMaturityTrend.pFdAccountNo = dr["fdaccountno"];
                        objMaturityTrend.pTransdate = dr["transdate"];
                        objMaturityTrend.pMembercode = dr["membercode"];
                        objMaturityTrend.pMembername = dr["membername"];
                        objMaturityTrend.pTenor = dr["tenor"];
                        objMaturityTrend.pDepositamount = dr["depositamount"];
                        objMaturityTrend.pInterestrate = dr["interestrate"];
                        objMaturityTrend.pMaturityamount = dr["maturityamount"];
                        objMaturityTrend.pMaturitydate = dr["maturitydate"];
                        lstMaturityDetails.Add(objMaturityTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityDetails;
        }
        #endregion

        #region Interest Payment Trend Report...
        public List<InterestPaymentTrendDTO> ShowInterestPaymentReport(string Connectionstring)
        {
            List<InterestPaymentTrendDTO> lstInterestPaymentTrendDetails = new List<InterestPaymentTrendDTO>();
            try
            {

                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "SELECT fn_Interest_payment();");
                string Query = string.Empty;

                Query = @"select schemename,sum(cummon) as cummon,sum(cummon1) as cummon1,sum(cummon2) as cummon2,sum(cummon3) as cummon3,
sum(cummon4) as cummon4,sum(cummon5) as cummon5,sum(cummon6) as cummon6,sum(cummon7) as cummon7,sum(cummon8) as cummon8,
sum(cummon9) as cummon9,sum(cummon10) as cummon10,sum(cummon11) as cummon11,sum(cummon12) as cummon12 from(
SELECT mthreport.schemename, cast(coalesce(mthreport.curmn,'0') as numeric)as cummon ,
cast(coalesce(mthreport.curmn1,'0') as numeric)as cummon1 ,cast(coalesce(mthreport.curmn2,'0') as numeric)as cummon2 ,
cast(coalesce(mthreport.curmn3,'0') as numeric)as cummon3 ,cast(coalesce(mthreport.curmn4,'0') as numeric)as cummon4 ,
cast(coalesce(mthreport.curmn5,'0') as numeric)as cummon5 ,cast(coalesce(mthreport.curmn6,'0') as numeric)as cummon6 ,
cast(coalesce(mthreport.curmn7,'0') as numeric)as cummon7 ,cast(coalesce(mthreport.curmn8,'0') as numeric)as cummon8 ,
cast(coalesce(mthreport.curmn9,'0') as numeric)as cummon9 ,cast(coalesce(mthreport.curmn10,'0') as numeric)as cummon10 ,
cast(coalesce(mthreport.curmn11,'0') as numeric)as cummon11 ,cast(coalesce(mthreport.curmn12,'0') as numeric) as cummon12 
FROM crosstab('SELECT SCHEME_NAME::text As row_name,MONTH As Monthname,
			  coalesce(sum(TOTAL_INTEREST_AMOUNT),0) As TOTAL_INTEREST_AMOUNT FROM TEMPINTERESTPAYMENT 
			  group by SCHEME_NAME,MONTH order by MONTH desc'::text,
			  'select * from vwcurrentplus12months'::text) mthreport 
			  (schemename text, curmn character varying(25), curmn1 character varying(25), 
			   curmn2 character varying(25), curmn3 character varying(25), curmn4 character varying(25),
			   curmn5 character varying(25), curmn6 character varying(25), curmn7 character varying(25),
			   curmn8 character varying(25), curmn9 character varying(25),
curmn10 character varying(25), curmn11 character varying(25), curmn12 character varying(25)))g group by schemename;";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        InterestPaymentTrendDTO objInterestTrend = new InterestPaymentTrendDTO();
                        objInterestTrend.pschemename = dr["schemename"];
                        //objMaturityTrend.pCount = dr["Count"];
                        objInterestTrend.pcurrentmonth = dr["cummon"];
                        objInterestTrend.pcurrentmonth1 = dr["cummon1"];
                        objInterestTrend.pcurrentmonth2 = dr["cummon2"];
                        objInterestTrend.pcurrentmonth3 = dr["cummon3"];
                        objInterestTrend.pcurrentmonth4 = dr["cummon4"];
                        objInterestTrend.pcurrentmonth5 = dr["cummon5"];
                        objInterestTrend.pcurrentmonth6 = dr["cummon6"];
                        objInterestTrend.pcurrentmonth7 = dr["cummon7"];
                        objInterestTrend.pcurrentmonth8 = dr["cummon8"];
                        objInterestTrend.pcurrentmonth9 = dr["cummon9"];
                        objInterestTrend.pcurrentmonth10 = dr["cummon10"];
                        objInterestTrend.pcurrentmonth11 = dr["cummon11"];
                        objInterestTrend.pcurrentmonth12 = dr["cummon12"];


                        lstInterestPaymentTrendDetails.Add(objInterestTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestPaymentTrendDetails;
        }

        public List<InterestPaymentTrendDetailsDTO> ShowInterestTrendShemeAndDatewiseDetails(string schemename, string maturitydate, string Connectionstring)
        {
            List<InterestPaymentTrendDetailsDTO> lstInterestTrendDetails = new List<InterestPaymentTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"select scheme_name,maturitydate,month,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount ,interest_amount,tds_amount,total_interest_amount from TEMPINTERESTPAYMENT A join tbltransfdcreation B on A.fd_id=B.fdaccountid where accountstatus='N' and scheme_name='" + ManageQuote(schemename) + "' and month='" + maturitydate + "' order by maturitydate;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        InterestPaymentTrendDetailsDTO objInterestTrend = new InterestPaymentTrendDetailsDTO();
                        objInterestTrend.pSchemename1 = dr["scheme_name"];
                        objInterestTrend.pMonth = dr["month"];
                        objInterestTrend.pFdAccountNo = dr["fdaccountno"];
                        objInterestTrend.pTransdate = dr["transdate"];
                        objInterestTrend.pMembercode = dr["membercode"];
                        objInterestTrend.pMembername = dr["membername"];
                        objInterestTrend.pTenor = dr["tenor"];
                        objInterestTrend.pDepositamount = dr["depositamount"];
                        objInterestTrend.pInterestrate = dr["interestrate"];
                        objInterestTrend.pMaturityamount = dr["maturityamount"];
                        objInterestTrend.pMaturitydate = dr["maturitydate"];
                        objInterestTrend.pInterestamount = dr["interest_amount"];
                        objInterestTrend.pTdsamount = dr["tds_amount"];
                        objInterestTrend.pTotalinterestamount = dr["total_interest_amount"];
                        lstInterestTrendDetails.Add(objInterestTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestTrendDetails;
        }
        public List<InterestPaymentTrendDetailsDTO> ShowInterestTrendGrandTotalDatewiseDetails(string maturitydate, string Connectionstring)
        {
            List<InterestPaymentTrendDetailsDTO> lstInterestTrendDetails = new List<InterestPaymentTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"select scheme_name,maturitydate,month,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount ,interest_amount,tds_amount,total_interest_amount from TEMPINTERESTPAYMENT A join tbltransfdcreation B on A.fd_id=B.fdaccountid where accountstatus='N' and month='" + maturitydate + "' order by maturitydate;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        InterestPaymentTrendDetailsDTO objInterestTrend = new InterestPaymentTrendDetailsDTO();
                        objInterestTrend.pSchemename1 = dr["scheme_name"];
                        objInterestTrend.pMonth = dr["month"];
                        objInterestTrend.pFdAccountNo = dr["fdaccountno"];
                        objInterestTrend.pTransdate = dr["transdate"];
                        objInterestTrend.pMembercode = dr["membercode"];
                        objInterestTrend.pMembername = dr["membername"];
                        objInterestTrend.pTenor = dr["tenor"];
                        objInterestTrend.pDepositamount = dr["depositamount"];
                        objInterestTrend.pInterestrate = dr["interestrate"];
                        objInterestTrend.pMaturityamount = dr["maturityamount"];
                        objInterestTrend.pMaturitydate = dr["maturitydate"];
                        objInterestTrend.pInterestamount = dr["interest_amount"];
                        objInterestTrend.pTdsamount = dr["tds_amount"];
                        objInterestTrend.pTotalinterestamount = dr["total_interest_amount"];
                        lstInterestTrendDetails.Add(objInterestTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestTrendDetails;
        }
        #endregion

        #region Pre Maturity Report...

        public List<PreMaturityDetailsDTO> ShowPreMaturityReport(string fromdate, string todate, string type, string pdatecheked, string Connectionstring)
        {

            List<PreMaturityDetailsDTO> lstPrematurityDetails = new List<PreMaturityDetailsDTO>();
            try
            {
                string Query = string.Empty;
                if (pdatecheked == "ASON")
                {
                    if (type.ToUpper() == "ALL")
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
left join (select * from maturity_payments )A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid
where B.accountstatus in ('N','P','R') and C.trans_date<='" + FormatDate(todate) + "' union select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from tbltransfdcreation B join maturity_bonds C on B.memberid = C.member_id and B.fdaccountid = C.trans_type_id  join(select* from maturity_payments )A on A.member_id = B.memberid and A.trans_type_id = B.fdaccountid where B.accountstatus in ('N', 'P', 'R') and C.trans_date <= '" + FormatDate(todate) + "'; ";
                    }
                    else if (type == "Renewal")
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
join (select * from maturity_payments where payment_type='" + ManageQuote(type) + "')A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid where B.accountstatus in ('N','P','R') and C.trans_date<='" + FormatDate(todate) + "';";
                    }
                    else
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
left join (select * from maturity_payments where payment_type='" + ManageQuote(type) + "')A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid where B.accountstatus in ('N','P','R') and C.trans_date<='" + FormatDate(todate) + "';";
                    }

                }
                else if (pdatecheked == "BETWEEN")
                {
                    if (type.ToUpper() == "ALL")
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
left join (select * from maturity_payments )A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid
where B.accountstatus in ('N','P','R') and C.trans_date  between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' union select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from tbltransfdcreation B join maturity_bonds C on B.memberid = C.member_id and B.fdaccountid = C.trans_type_id  join(select* from maturity_payments )A on A.member_id = B.memberid and A.trans_type_id = B.fdaccountid where B.accountstatus in ('N', 'P', 'R') and C.trans_date between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'; ";


                    }
                    else if (type == "Renewal")
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
join (select * from maturity_payments where payment_type='" + ManageQuote(type) + "')A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid where B.accountstatus in ('N','P','R') and C.trans_date between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "';";
                    }
                    else
                    {
                        Query = @"select to_char(C.trans_date, 'dd/Mon/yyyy')  as prematuritydate,C.mature_amount as prematurityamount,B.accountstatus,A.maturity_payment_id,A.member_id,B.membername,B.fdaccountno,B.fdname as schemename,B.tenor||' '||tenortype as tenure,B.depositamount, to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate,B.maturityamount,to_char(B.maturitydate, 'dd/Mon/yyyy')  as maturitydate,B.interestrate,B.interestpayable,B.chitbranchname,A.trans_type,A.trans_type_id,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_jv_date,to_char(A.payment_date, 'dd/Mon/yyyy')  as maturity_payment_date,A.paid_amount,A.voucher_id from 
tbltransfdcreation B
join maturity_bonds C on B.memberid=C.member_id and B.fdaccountid=C.trans_type_id
join (select * from maturity_payments where payment_type='" + ManageQuote(type) + "')A  on A.member_id=B.memberid and A.trans_type_id=B.fdaccountid where B.accountstatus in ('N','P','R') and C.trans_date between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "';";

                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        PreMaturityDetailsDTO objPrematuritydetails = new PreMaturityDetailsDTO();
                        objPrematuritydetails.pMembername = dr["membername"].ToString();
                        objPrematuritydetails.pFdAccountNo = dr["fdaccountno"].ToString();
                        objPrematuritydetails.pSchemename = dr["schemename"].ToString();
                        objPrematuritydetails.pTenor = dr["tenure"];
                        objPrematuritydetails.pDepositamount = dr["depositamount"];
                        objPrematuritydetails.pDepositdate = dr["depositdate"];
                        objPrematuritydetails.pMaturityamount = dr["maturityamount"].ToString();
                        objPrematuritydetails.pMaturitydate = dr["maturitydate"].ToString();
                        objPrematuritydetails.pInterestrate = dr["interestrate"].ToString();
                        objPrematuritydetails.pInterestPayable = dr["interestpayable"];
                        objPrematuritydetails.pChitbranchname = dr["chitbranchname"];
                        objPrematuritydetails.pTranstype = dr["trans_type"];

                        objPrematuritydetails.pMaturityJVDate = dr["maturity_jv_date"].ToString();
                        objPrematuritydetails.pMaturityPaymentDate = dr["maturity_payment_date"].ToString();
                        objPrematuritydetails.pPaidAmount = dr["paid_amount"].ToString();

                        objPrematuritydetails.pPreMaturityDate = dr["prematuritydate"].ToString();
                        objPrematuritydetails.pPreMaturityAmt = dr["prematurityamount"].ToString();

                        lstPrematurityDetails.Add(objPrematuritydetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPrematurityDetails;
        }

        #endregion

        #region Member wise Receipts Report...

        public List<LAReportsDTO> GetMemberName(string Connectionstring)
        {
            List<LAReportsDTO> lstMemberDetails = new List<LAReportsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct memberid,membername from tbltransfdcreation  where memberid in(select distinct member_id from fd_receipt) order by membername;"))
                {
                    while (dr.Read())
                    {
                        LAReportsDTO objMemberdetails = new LAReportsDTO();
                        objMemberdetails.pmemberid = dr["memberid"];
                        objMemberdetails.pmembername = dr["membername"];
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
        public List<MemberwiseReceiptsDTO> ShowMemberwiseReceiptsReport(long memberid, string fromdate, string todate, string pdatecheked, string Connectionstring)
        {

            List<MemberwiseReceiptsDTO> lstMemberwiseDetails = new List<MemberwiseReceiptsDTO>();
            try
            {
                string Query = string.Empty;
                if (pdatecheked == "ASON")
                {
                    if (memberid == 0)
                    {
                        Query = @"select to_char(A.fd_receiptt_date, 'dd/Mon/yyyy') as fd_receiptt_date,A.fd_account_id,to_char(A.receipt_date, 'dd/Mon/yyyy') as receipt_date,A.deposit_type,A.instalment_amount,A.received_amount,A.mode_of_receipt,A.receipt_no,(case when D.depositstatus = 'C' then 'Canceled'  when D.depositstatus = 'R' then 'Rejected' when(D.depositstatus = 'P' and D.clearstatus = 'Y') then 'Cleared' when(D.depositstatus = 'P' and D.clearstatus = 'R') then 'Returned' when(D.depositstatus = 'P' and D.clearstatus = 'N') then 'Pending' when(D.depositstatus = 'N' and D.clearstatus = 'N') then 'Not Cleared' when(A.mode_of_receipt = 'CASH') then 'Cleared' when(A.mode_of_receipt = 'ADJUSTMENT') then 'ADJUSTMENT' end) as ChequeStatus,B.membername,B.fdaccountno,B.fdname as schemename,B.depositamount,to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate
,C.referralname as agentname,C.fdaccountid,C.commsssionvalue

 from fd_receipt A  join tbltransfdcreation B on A.fd_account_id = B.fdaccountid and A.member_id = B.memberid
left join  tbltransfdreferraldetails C on C.fdaccountid = A.fd_account_id
left join TBLTRANSRECEIPTREFERENCE D on D.receiptid = A.receipt_no where A.fd_receiptt_date <='" + FormatDate(todate) + "'";


                    }
                    else
                    {
                        Query = @"select to_char(A.fd_receiptt_date, 'dd/Mon/yyyy') as fd_receiptt_date,A.fd_account_id,to_char(A.receipt_date, 'dd/Mon/yyyy') as receipt_date,A.deposit_type,A.instalment_amount,A.received_amount,A.mode_of_receipt,A.receipt_no,(case when D.depositstatus='C' then 'Canceled'  when D.depositstatus='R' then 'Rejected' when (D.depositstatus='P' and D.clearstatus='Y') then 'Cleared' when (D.depositstatus='P' and D.clearstatus='R') then 'Returned' when (D.depositstatus='P' and D.clearstatus='N') then 'Pending'
when (A.mode_of_receipt='CASH') then 'Cleared' when (A.mode_of_receipt='ADJUSTMENT') then 'ADJUSTMENT' end) as ChequeStatus,B.membername,B.fdaccountno,B.fdname as schemename,B.depositamount,to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate
,C.referralname as agentname,C.fdaccountid,C.commsssionvalue
 from fd_receipt A  join tbltransfdcreation B on A.fd_account_id=B.fdaccountid and A.member_id=B.memberid

left join  tbltransfdreferraldetails C on C.fdaccountid=A.fd_account_id

left join TBLTRANSRECEIPTREFERENCE D on D.receiptid=A.receipt_no where  A.member_id=" + memberid + " and A.fd_receiptt_date<='" + FormatDate(todate) + "'";

                    }

                }
                else if (pdatecheked == "BETWEEN")
                {
                    if (memberid == 0)
                    {
                        Query = @"select to_char(A.fd_receiptt_date, 'dd/Mon/yyyy') as fd_receiptt_date,A.fd_account_id,to_char(A.receipt_date, 'dd/Mon/yyyy') as receipt_date,A.deposit_type,A.instalment_amount,A.received_amount,A.mode_of_receipt,A.receipt_no,(case when D.depositstatus='C' then 'Canceled'  when D.depositstatus='R' then 'Rejected' when (D.depositstatus='P' and D.clearstatus='Y') then 'Cleared' when (D.depositstatus='P' and D.clearstatus='R') then 'Returned' when (D.depositstatus='P' and D.clearstatus='N') then 'Pending'
when (A.mode_of_receipt='CASH') then 'Cleared' when (A.mode_of_receipt='ADJUSTMENT') then 'ADJUSTMENT' end) as ChequeStatus,B.membername,B.fdaccountno,B.fdname as schemename,B.depositamount,to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate
,C.referralname as agentname,C.fdaccountid,C.commsssionvalue
 from fd_receipt A  join tbltransfdcreation B on A.fd_account_id=B.fdaccountid and A.member_id=B.memberid
left join  tbltransfdreferraldetails C on  C.fdaccountid=A.fd_account_id
left join TBLTRANSRECEIPTREFERENCE D on D.receiptid=A.receipt_no where  A.fd_receiptt_date between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'";
                    }
                    else
                    {
                        Query = @"select to_char(A.fd_receiptt_date, 'dd/Mon/yyyy') as fd_receiptt_date,A.fd_account_id,to_char(A.receipt_date, 'dd/Mon/yyyy') as receipt_date,A.deposit_type,A.instalment_amount,A.received_amount,A.mode_of_receipt,A.receipt_no,(case when D.depositstatus='C' then 'Canceled'  when D.depositstatus='R' then 'Rejected' when (D.depositstatus='P' and D.clearstatus='Y') then 'Cleared' when (D.depositstatus='P' and D.clearstatus='R') then 'Returned' when (D.depositstatus='P' and D.clearstatus='N') then 'Pending'
when (A.mode_of_receipt='CASH') then 'Cleared' when (A.mode_of_receipt='ADJUSTMENT') then 'ADJUSTMENT' end) as ChequeStatus,B.membername,B.fdaccountno,B.fdname as schemename,B.depositamount,to_char(B.depositdate, 'dd/Mon/yyyy') as depositdate
,C.referralname as agentname,C.fdaccountid,C.commsssionvalue
 from fd_receipt A  join tbltransfdcreation B on A.fd_account_id=B.fdaccountid and A.member_id=B.memberid

left join  tbltransfdreferraldetails C on  C.fdaccountid=A.fd_account_id

left join TBLTRANSRECEIPTREFERENCE D on D.receiptid=A.receipt_no where  A.member_id=" + memberid + " and A.fd_receiptt_date between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'";

                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MemberwiseReceiptsDTO objMemberwise = new MemberwiseReceiptsDTO();
                        objMemberwise.pReceiptVoucherno = dr["receipt_no"].ToString();
                        objMemberwise.pChequeStatus = dr["ChequeStatus"].ToString();

                        objMemberwise.pFDReceiptDate = dr["fd_receiptt_date"].ToString();
                        objMemberwise.pReceiptDate = dr["receipt_date"].ToString();
                        objMemberwise.pDeposittype = dr["deposit_type"].ToString();
                        objMemberwise.pInstallmentamount = dr["instalment_amount"];
                        objMemberwise.pReceivedamount = dr["received_amount"];
                        objMemberwise.pModeofReceipt = dr["mode_of_receipt"];

                        objMemberwise.pMembername = dr["membername"].ToString();
                        objMemberwise.pFdAccountNo = dr["fdaccountno"].ToString();
                        objMemberwise.pSchemename = dr["schemename"];
                        objMemberwise.pDepositamount = dr["depositamount"];
                        objMemberwise.pDepositdate = dr["depositdate"];
                        objMemberwise.pAgentName = dr["agentname"].ToString();
                        objMemberwise.pCommissionValue = dr["commsssionvalue"].ToString();

                        lstMemberwiseDetails.Add(objMemberwise);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberwiseDetails;
        }

        #endregion

        #region Interest  Report...
        public List<InterestreportDTO> interestreport(string forthemonth, long Schemeid, string paymenttype, string companyname, string branchname, string Connectionstring)
        {
            NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "SELECT FN_INTEREST_REPORT('" + forthemonth + "');");
            List<InterestreportDTO> lstInterestReport = new List<InterestreportDTO>();
            try
            {
                string Query = string.Empty;
                if (paymenttype.ToUpper() == "SELF")
                {
                    Query = "select member_name,trans_type_no,interest_amount,interest_payable,tds_amount,interest_payable,pancard,interest_paid_month from TEMP_INTEREST_REPORT ti join self_or_adjustment sa on ti.trans_type_id=sa.fd_account_id where scheme_id=" + Schemeid + " and upper(sa.payment_type)=upper('" + paymenttype + "') order by member_name,trans_type_no;";
                }
                else if (paymenttype.ToUpper() == "ADJUSTMENT")
                {
                    Query = "select member_name,trans_type_no,interest_amount,interest_payable,tds_amount,interest_payable,pancard,interest_paid_month from TEMP_INTEREST_REPORT ti join self_or_adjustment sa on ti.trans_type_id=sa.fd_account_id where scheme_id=" + Schemeid + " and upper(sa.payment_type)=upper('" + paymenttype + "') and upper(sa.company_name) =upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by member_name,trans_type_no;";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        InterestreportDTO objinterestreport = new InterestreportDTO();
                        objinterestreport.pAdvanceacno = dr["trans_type_no"];
                        objinterestreport.pMembername = dr["member_name"];
                        objinterestreport.pInterestamount = dr["interest_payable"];
                        objinterestreport.pInterestPayable = dr["INTEREST_AMOUNT"];
                        objinterestreport.pTdsamount = dr["TDS_AMOUNT"];
                        objinterestreport.pPancard = dr["pancard"];
                        objinterestreport.pMonth = dr["INTEREST_PAID_MONTH"];
                        lstInterestReport.Add(objinterestreport);
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstInterestReport;
        }
        #endregion

        #region Print Maturity Trend Details Report...
        public List<PrintMaturityTrendDetailsDTO> PrintMaturityTrendDetailsReport(string maturitydate, string Connectionstring)
        {
            List<PrintMaturityTrendDetailsDTO> lstPrintMaturityDetails = new List<PrintMaturityTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select fdname as schemename,fdaccountid,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount, maturitydate,TO_CHAR(CAST(maturitydate AS DATE),'MON-YYYY') As Monthname from tbltransfdcreation A join vwfdreceiptsfinal B on A.fdaccountid = B.fd_account_id where accountstatus='N' and  maturitydate BETWEEN cast('01-'||to_char(CURRENT_DATE,'MON-YYYY') as date) and(date_trunc('month', cast('01-'||to_char(CURRENT_DATE,'MON-YYYY') as date) + cast('12 months' as interval)) + interval '1 months' - interval '1 day' )::date and  accountstatus='N' order by maturitydate;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        PrintMaturityTrendDetailsDTO objPrintMaturityTrend = new PrintMaturityTrendDetailsDTO();
                        objPrintMaturityTrend.pMonth = dr["Monthname"];
                        objPrintMaturityTrend.pSchemename1 = dr["schemename"];
                        objPrintMaturityTrend.pFdAccountNo = dr["fdaccountno"];
                        objPrintMaturityTrend.pTransdate = dr["transdate"];
                        objPrintMaturityTrend.pMembercode = dr["membercode"];
                        objPrintMaturityTrend.pMembername = dr["membername"];
                        objPrintMaturityTrend.pTenor = dr["tenor"];
                        objPrintMaturityTrend.pDepositamount = dr["depositamount"];
                        objPrintMaturityTrend.pInterestrate = dr["interestrate"];
                        objPrintMaturityTrend.pMaturityamount = dr["maturityamount"];
                        objPrintMaturityTrend.pMaturitydate = dr["maturitydate"];
                        lstPrintMaturityDetails.Add(objPrintMaturityTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPrintMaturityDetails;
        }
        #endregion

        #region Print Interest Trend Details Report...
        public List<PrintInterestPaymentTrendDetailsDTO> PrintInterestTrendDetailsReport(string maturitydate, string Connectionstring)
        {
            List<PrintInterestPaymentTrendDetailsDTO> lstPrintInterestDetails = new List<PrintInterestPaymentTrendDetailsDTO>();
            try
            {
                string Query = string.Empty;
                Query = @"select scheme_name,maturitydate,month,fdaccountno,transdate,membercode,membername,tenor||'-'||tenortype as tenor,depositamount,interestrate,maturityamount ,interest_amount,tds_amount,total_interest_amount from TEMPINTERESTPAYMENT A join tbltransfdcreation B on A.fd_id=B.fdaccountid where accountstatus='N' and cast('01-'||(month) as date)  BETWEEN cast('01-'||to_char(CURRENT_DATE,'MON-YYYY') as date) and(date_trunc('month', cast('01-'||to_char(CURRENT_DATE,'MON-YYYY') as date) + cast('12 months' as interval)) + interval '1 months' - interval '1 day' )::date order by cast('01-'||(month) as date) ;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        PrintInterestPaymentTrendDetailsDTO objPrintInterestTrend = new PrintInterestPaymentTrendDetailsDTO();
                        objPrintInterestTrend.pSchemename1 = dr["scheme_name"];
                        objPrintInterestTrend.pMonth = dr["month"];
                        objPrintInterestTrend.pFdAccountNo = dr["fdaccountno"];
                        objPrintInterestTrend.pTransdate = dr["transdate"];
                        objPrintInterestTrend.pMembercode = dr["membercode"];
                        objPrintInterestTrend.pMembername = dr["membername"];
                        objPrintInterestTrend.pTenor = dr["tenor"];
                        objPrintInterestTrend.pDepositamount = dr["depositamount"];
                        objPrintInterestTrend.pInterestrate = dr["interestrate"];
                        objPrintInterestTrend.pMaturityamount = dr["maturityamount"];
                        objPrintInterestTrend.pMaturitydate = dr["maturitydate"];
                        objPrintInterestTrend.pInterestamount = dr["interest_amount"];
                        objPrintInterestTrend.pTdsamount = dr["tds_amount"];
                        objPrintInterestTrend.pTotalinterestamount = dr["total_interest_amount"];
                        lstPrintInterestDetails.Add(objPrintInterestTrend);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPrintInterestDetails;
        }

        public List<ApplicationFormDTO> GetApplicationFdnames(string Connectionstring)
        {
            List<ApplicationFormDTO> lstApplicationFormFdnames = new List<ApplicationFormDTO>();
            try
            {
                string Query = string.Empty;
                Query = "select fdaccountid,fdaccountno||'-'||membername as name,mobileno from vwfdtransaction_details;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ApplicationFormDTO objApplicationForm = new ApplicationFormDTO();
                        objApplicationForm.pFdaccountid = Convert.ToInt64(dr["fdaccountid"]);
                        objApplicationForm.pFdAccountNo = Convert.ToString(dr["name"]);
                        objApplicationForm.pMobileNo = Convert.ToString(dr["mobileno"]);
                        lstApplicationFormFdnames.Add(objApplicationForm);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstApplicationFormFdnames;
        }
        public ApplicationFormDetailsDTO GetApplicationFormDetails(string FdAccountNo, string Connectionstring)
        {
            ApplicationFormDetailsDTO objApplicationForm = new ApplicationFormDetailsDTO();
            try
            {
                string Query = string.Empty;
                Query = "select tab.*,y.fd_receiptt_date,y.bankname,y.mode_of_receipt,y.receipt_no,y.branchname,tc.nameofenterprise as Companyname,(tcd.address1 || tcd.address2 || tcd.district ||','|| tcd.city||','||tcd.state||' - ' ||tcd.pincode)address from(select t.fdaccountno,c.titlename,t.membername,(EXTRACT(year FROM age(current_date,c.dob)))::int as Age,to_char(c.dob,'dd/mm/yyyy')dob,c.gender,t.mobileno,c.businessentityemailid as mailid,(case when  vc.address1='' then '' else (vc.address1 || vc.district ||','|| vc.city||','||vc.state||' - ' ||vc.pincode) end)member_address,t.chitbranchname,string_agg(nomineename||'('||relationship||')',  ',' )as  Nominee  from vwfdtransaction_details t join tblmstcontact c on t.contactid=c.contactid join vwcontactdataview vc on vc.contactid=c.contactid left join tabapplicationpersonalnomineedetails ta on t.fdaccountno=ta.vchapplicationid and t.memberid=ta.applicationid where t.fdaccountno='" + FdAccountNo + "' group by c.titlename,t.membername,c.dob,c.gender,t.mobileno,c.businessentityemailid,vc.address1,vc.district,vc.city,vc.state,vc.pincode,t.chitbranchname,t.fdaccountno)tab left join (select tm.membername,tm.membercode,ft.fdaccountno,to_char(fr.fd_receiptt_date, 'dd/Mon/yyyy')fd_receiptt_date,fr.received_amount,tt.bankname as bankname,tt.branchname as branchname,fr.mode_of_receipt,fr.receipt_no,(case when tt.clearstatus = 'Y' then 'Cleared' when tt.clearstatus = 'R' then 'Returned' when tt.depositstatus = 'C' then 'Cancelled'  when tt.clearstatus IS NULL then 'Cleared' else 'Un Cleared' end)as ChequeStatus from fd_receipt fr join tblmstmembers tm on fr.member_id = tm.memberid join tbltransfdcreation ft on fr.fd_account_id = ft.fdaccountid and fr.status = true left join tbltransreceiptreference tt on tt.receiptid=fr.receipt_no and tt.clearstatus <> 'R'   where ft.fdaccountno='" + FdAccountNo + "'   order by fd_receipt_id desc limit 1)as y on tab.fdaccountno=y.fdaccountno,tblmstcompany tc left join tblmstcompanyaddressdetails tcd on tcd.companyid=tc.companyid and tcd.priority='PRIMARY'; ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        objApplicationForm = new ApplicationFormDetailsDTO();
                        objApplicationForm.pFdAccountNO = Convert.ToString(dr["fdaccountno"]);
                        objApplicationForm.pTitleName = Convert.ToString(dr["titlename"]);
                        objApplicationForm.pMemberName = Convert.ToString(dr["membername"]);
                        objApplicationForm.pAge = Convert.ToString(dr["age"]);
                        objApplicationForm.pDob = Convert.ToString(dr["dob"]);
                        objApplicationForm.pGender = Convert.ToString(dr["gender"]);
                        objApplicationForm.pMobileNo = Convert.ToString(dr["mobileno"]);
                        objApplicationForm.pMailId = Convert.ToString(dr["mailid"]);
                        objApplicationForm.pMemberAddress = Convert.ToString(dr["member_address"]);
                        objApplicationForm.pBranchName = Convert.ToString(dr["chitbranchname"]);
                        objApplicationForm.pNominee = Convert.ToString(dr["nominee"]);
                        objApplicationForm.pReceiptDate = Convert.ToString(dr["fd_receiptt_date"]);
                        objApplicationForm.pBankName = Convert.ToString(dr["bankname"]);
                        objApplicationForm.pModeOfPayment = Convert.ToString(dr["mode_of_receipt"]);
                        objApplicationForm.pReceiptNo = Convert.ToString(dr["receipt_no"]);
                        objApplicationForm.pBankBranchName = Convert.ToString(dr["branchname"]);
                        objApplicationForm.pCompanyName = Convert.ToString(dr["companyname"]);
                        objApplicationForm.pCompanyAddress = Convert.ToString(dr["address"]);

                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return objApplicationForm;
        }
        #endregion
    }
}
