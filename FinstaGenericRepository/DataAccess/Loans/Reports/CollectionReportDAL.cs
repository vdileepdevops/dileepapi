using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Reports;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace FinstaRepository.DataAccess.Loans.Reports
{
    public class CollectionReportDAL : SettingsDAL, ICollectionReport
    {
        public List<CollectionReportDTO> lstColletionsummary { get; set; }
        public List<CollectionReportDetailsDTO> lstColletiondetails { get; set; }


        #region GetColletionsummary
        public List<CollectionReportDTO> GetColletionsummary(string fromdate, string todate, string ConnectionString, int recordid, string fieldname, string fieldtype)
        {
            lstColletionsummary = new List<CollectionReportDTO>();
            string query = string.Empty;
            try
            {
                // recordid = 0(Collection Report) or 6(Trend Report Collection Detaild) Total Collection
                // recordid =  4 Previous Dues Collection
                // recordid = 5 Current Period Dues Collection
                
                if (fieldname.ToUpper() == "ALL")
                {
                    if (recordid == 0 || recordid == 6)
                    {

                        query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,sum(principal) as principal,sum(inetrest) as inetrest,sum(penalty) as penalty,sum(charges) as charges from vwcollectionsummary where receiptdate between  '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "'  group by applicanttype, vchapplicationid,applicantname,loantype,loanname order by vchapplicationid";
                    }
                    else if (recordid == 4)
                    {
                        query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)<'" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid";
                    }
                    else if (recordid == 5)
                    {
                        query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)>='" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid";
                    }
                }
                else
                {
                    if (fieldtype.ToUpper() == "LOAN TYPE")
                    {
                        if (recordid == 0 || recordid == 6)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,sum(principal) as principal,sum(inetrest) as inetrest,sum(penalty) as penalty,sum(charges) as charges from vwcollectionsummary where receiptdate between  '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "'  and loantype='" + fieldname + "'   group by applicanttype,vchapplicationid,applicantname,loantype,loanname order by vchapplicationid";
                        }
                        else if (recordid == 4)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)<'" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  loantype='" + fieldname + "'  order by vchapplicationid";
                        }
                        else if (recordid == 5)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)>='" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  loantype='" + fieldname + "'  order by vchapplicationid";
                        }
                    }

                    if (fieldtype.ToUpper() != "LOAN TYPE")
                    {
                        if (recordid == 0 || recordid == 6)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,sum(principal) as principal,sum(inetrest) as inetrest,sum(penalty) as penalty,sum(charges) as charges from vwcollectionsummary where receiptdate between  '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and applicanttype='" + fieldname + "'  group by applicanttype,vchapplicationid,applicantname,loantype,loanname order by vchapplicationid";
                        }
                        else if (recordid == 4)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)<'" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  applicanttype='" + fieldname + "'  order by vchapplicationid";
                        }
                        else if (recordid == 5)
                        {
                            query = "select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal, inetrest,penalty, charges from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and coalesce(instalmentdate,servicerundate)>='" + FormatDate(fromdate.ToString()) + "' group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  applicanttype='" + fieldname + "'  order by vchapplicationid";
                        }
                    }
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                {
                    while (dr.Read())
                    {
                        CollectionReportDTO objcollectionsummary = new CollectionReportDTO();
                        objcollectionsummary.pApplicanttype = dr["applicanttype"].ToString();
                        objcollectionsummary.pApplicantname = dr["applicantname"].ToString();
                        objcollectionsummary.pLoantype = dr["loantype"].ToString();
                        objcollectionsummary.pVchapplicationid = dr["vchapplicationid"].ToString();
                        objcollectionsummary.pPrinciple = Convert.ToDecimal(dr["principal"]);
                        objcollectionsummary.pInterest = Convert.ToDecimal(dr["inetrest"]);
                        objcollectionsummary.pPenality = Convert.ToDecimal(dr["penalty"]);
                        objcollectionsummary.pCharges = Convert.ToDecimal(dr["charges"]);
                        objcollectionsummary.pTotalamount = Convert.ToDecimal(dr["principal"]) + Convert.ToDecimal(dr["inetrest"]) + Convert.ToDecimal(dr["penalty"]) + Convert.ToDecimal(dr["charges"]);
                        lstColletionsummary.Add(objcollectionsummary);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstColletionsummary;
        }
        #endregion

        #region GetColletiondetails
        public List<CollectionReportDetailsDTO> GetColletiondetails(string fromdate, string todate, string Applicationid, string ConnectionString)
        {
            lstColletiondetails = new List<CollectionReportDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwcollectiondetails where receiptdate between '" + FormatDate(fromdate.ToString()) + "' and '" + FormatDate(todate.ToString()) + "' and vchapplicationid= '" + Applicationid + "' order by receiptdate desc;"))
                {
                    while (dr.Read())
                    {
                        CollectionReportDetailsDTO objcollectiondetails = new CollectionReportDetailsDTO();
                        objcollectiondetails.pApplicantname = dr["applicantname"].ToString();
                        objcollectiondetails.pLoantype = dr["loantype"].ToString();
                        objcollectiondetails.pVchapplicationid = dr["vchapplicationid"].ToString();
                        if (dr["receiptdate"] != DBNull.Value)
                        {
                            objcollectiondetails.pReciptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objcollectiondetails.pReciptdate = "";

                        }

                        objcollectiondetails.pReciptNo = dr["receiptno"].ToString();
                        objcollectiondetails.pModeofpayment = dr["MODEOFPAYMENT"].ToString();
                        objcollectiondetails.pParticulars = dr["Particulars"].ToString();
                        objcollectiondetails.pReceiptamount = Convert.ToDecimal(dr["total"]);
                        objcollectiondetails.pNarration = dr["narration"].ToString();
                        lstColletiondetails.Add(objcollectiondetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstColletiondetails;
        }
        #endregion
    }
}
