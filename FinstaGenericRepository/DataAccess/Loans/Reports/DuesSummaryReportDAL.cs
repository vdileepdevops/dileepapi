using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Reports;
using HelperManager;
using Npgsql;

namespace FinstaRepository.DataAccess.Loans.Reports
{
    public class DuesSummaryReportDAL : SettingsDAL, IDuesSummaryReport
    {
        public DuesSummaryReportDTO _DuesSummaryReportDTO { get; set; }
        public List<DuesSummaryReportDTO> __DuesSummaryReportDTOList { get; set; }

        public async Task<List<DuesSummaryReportDTO>> GetDuesSummaryReport(string Connectionstring, string FromDate, string ToDate, int recordid, string fieldname, string fieldtype, string duestype)
        {
            __DuesSummaryReportDTOList = new List<DuesSummaryReportDTO>();
            string query = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(FromDate))
                    {
                        FromDate = "null";
                    }
                    else
                    {
                        FromDate = "'" + FormatDate(FromDate) + "'";
                    }
                    if (string.IsNullOrEmpty(ToDate))
                    {
                        ToDate = "null";
                    }
                    else
                    {
                        ToDate = "'" + FormatDate(ToDate) + "'";
                    }
                   

                    if (fieldname.ToUpper() == "ALL")
                    {
                        if (recordid == 0 || recordid == 1)
                        {
                            query = "SELECT  0 as paidamount,0 as currentdue,* from fn_duessummaryreport(" + FromDate + "," + ToDate + ",'" + duestype + "');";
                        }
                        else
                        {
                            query = " select paidamount,paidamount+dueamount as currentdue,t1.loantype , t1.loanname , t1.applicanttype , t1.applicantname , loanaccountno , disbursedloanamount , dueamount , totalnoofdues1 , duepenalty , totalduewithpenalty , outstandingprinciple , futureinstalments , loanstatus , salesexecutive , noofinstalmentspaid , lastpaidamount , lastpenaltypaid , lastpaiddate, tenureofloan , loanpayin , disburseddate, loanstartdate, loanenddate  from (SELECT* from fn_duessummaryreport(" + FromDate + ", " + ToDate + ", '" + duestype + "')) t1 left join(select applicanttype, vchapplicationid, applicantname, loantype, loanname, principal+inetrest + penalty + charges as paidamount from(SELECT applicationid, sum(coalesce(paidprincipleamount, 0))principal, sum(coalesce(paidinterestamount, 0)) inetrest, sum(coalesce(paidpenaltyamount, 0))penalty, sum(coalesce(paidchargesamount, 0))charges   from tbltransinstalmentsdetails  where particularstype like '%RECEIVED' and servicerundate between " + (FromDate.ToString()) + " and " + (ToDate.ToString()) + " and coalesce(instalmentdate, servicerundate) >= " + (FromDate.ToString()) + " group by applicationid) t1 join tabapplication t2 on t1.applicationid = t2.applicationid)t2 on loanaccountno = vchapplicationid; ";
                        }
                    }
                    else if (fieldtype.ToUpper() == "LOAN TYPE")
                    {
                        if (recordid == 0 || recordid == 1)
                        {
                            query = "SELECT  0 as paidamount,0 as currentdue,* from fn_duessummaryreport(" + FromDate + "," + ToDate + ",'" + duestype + "') where loantype='" + fieldname + "';";
                        }
                        else
                        {
                            query = " select paidamount,paidamount+dueamount as currentdue,t1.loantype , t1.loanname , t1.applicanttype , t1.applicantname , loanaccountno , disbursedloanamount , dueamount , totalnoofdues1 , duepenalty , totalduewithpenalty , outstandingprinciple , futureinstalments , loanstatus , salesexecutive , noofinstalmentspaid , lastpaidamount , lastpenaltypaid , lastpaiddate, tenureofloan , loanpayin , disburseddate, loanstartdate, loanenddate  from (SELECT* from fn_duessummaryreport(" + FromDate + ", " + ToDate + ", '" + duestype + "') where loantype='" + fieldname + "') t1 left join(select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal+ inetrest+penalty+ charges as paidamount from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between " + (FromDate.ToString()) + " and " + (ToDate.ToString()) + " and coalesce(instalmentdate,servicerundate)>=" + (FromDate.ToString()) + " group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  loantype='" + fieldname + "'  order by vchapplicationid)t2 on loanaccountno = vchapplicationid; ";
                        }
                    }
                    else if (fieldtype.ToUpper() != "LOAN TYPE")
                    {
                        if (recordid == 0 || recordid == 1)
                        {
                            query = "SELECT 0 as paidamount,0 as currentdue,* from fn_duessummaryreport(" + FromDate + "," + ToDate + ",'" + duestype + "') where applicanttype='" + fieldname + "';";
                        }
                        else
                        {
                            query = " select paidamount,paidamount+dueamount as currentdue,t1.loantype , t1.loanname , t1.applicanttype , t1.applicantname , loanaccountno , disbursedloanamount , dueamount , totalnoofdues1 , duepenalty , totalduewithpenalty , outstandingprinciple , futureinstalments , loanstatus , salesexecutive , noofinstalmentspaid , lastpaidamount , lastpenaltypaid , lastpaiddate, tenureofloan , loanpayin , disburseddate, loanstartdate, loanenddate  from (SELECT* from fn_duessummaryreport(" + FromDate + ", " + ToDate + ", '" + duestype + "') where applicanttype='" + fieldname + "') t1 left join(select applicanttype,vchapplicationid,applicantname,loantype,loanname,principal+ inetrest+penalty+ charges as paidamount from (SELECT applicationid,sum( coalesce(paidprincipleamount,0))principal,  sum( coalesce(paidinterestamount,0)) inetrest,sum(coalesce(paidpenaltyamount,0))penalty,sum(coalesce(paidchargesamount,0))charges   from tbltransinstalmentsdetails  where  particularstype like '%RECEIVED' and servicerundate between " + (FromDate.ToString()) + " and " + (ToDate.ToString()) + " and coalesce(instalmentdate,servicerundate)>=" + (FromDate.ToString()) + " group by applicationid) t1 join tabapplication t2 on t1.applicationid =t2.applicationid where  applicanttype='" + fieldname + "'  order by vchapplicationid)t2 on loanaccountno = vchapplicationid; ";
                        }
                    }

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            _DuesSummaryReportDTO = new DuesSummaryReportDTO
                            {
                                pcollection =  dr["paidamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["paidamount"]),
                                pcurrentdues = dr["currentdue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["currentdue"]),
                                pLoantype = Convert.ToString(dr["loantype"]).Trim().ToUpper(),
                                pLoanname = Convert.ToString(dr["loanname"]).Trim().ToUpper(),
                                pApplicantname = Convert.ToString(dr["applicantname"]).Trim().ToUpper(),
                                pLoanaccountno = Convert.ToString(dr["loanaccountno"]).Trim().ToUpper(),
                                pDisbursedamount = dr["disbursedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["disbursedloanamount"]),
                                pPresentDueamount = dr["dueamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["dueamount"]),
                                pTotalDues = dr["totalnoofdues1"] == DBNull.Value ? 0 : Convert.ToInt64(dr["totalnoofdues1"]),
                                pDuesPenalty = dr["duepenalty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["duepenalty"]),
                                pTotaldueamountwithpenalty = dr["totalduewithpenalty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["totalduewithpenalty"]),
                                pOutstandingPrinciple = dr["outstandingprinciple"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["outstandingprinciple"]),
                                pFutureinstallments = dr["futureinstalments"] == DBNull.Value ? 0 : Convert.ToInt64(dr["futureinstalments"]),
                                pLoanstatus = Convert.ToString(dr["loanstatus"]),
                                psalesExecutive = Convert.ToString(dr["salesexecutive"]),
                                pNoofInstallmentsPaid = dr["noofinstalmentspaid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["noofinstalmentspaid"]),
                                pLastPaidamount = dr["lastpaidamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["lastpaidamount"]),
                                pLastpaidDate = dr["lastpaiddate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["lastpaiddate"]).ToString("dd/MM/yyyy"),
                                pTenure = Convert.ToString(dr["tenureofloan"]),
                                pLoanPayin = Convert.ToString(dr["loanpayin"]),
                                pDisbursedDate = dr["disburseddate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["disburseddate"]).ToString("dd/MM/yyyy"),
                                pLoanstartdate = dr["loanstartdate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["loanstartdate"]).ToString("dd/MM/yyyy"),
                                pLoanenddate = dr["loanenddate"] == DBNull.Value ? Convert.ToDateTime(null).ToString("dd/MM/yyyy") : Convert.ToDateTime(dr["loanenddate"]).ToString("dd/MM/yyyy")
                            };
                            __DuesSummaryReportDTOList.Add(_DuesSummaryReportDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return __DuesSummaryReportDTOList;
        }
    }
}
