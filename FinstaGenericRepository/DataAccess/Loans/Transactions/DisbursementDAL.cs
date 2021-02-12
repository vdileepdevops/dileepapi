using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Accounting.Transactions;
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
    public class DisbursementDAL : SettingsDAL, IDisbursement
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        public DisbursementDTO _DisbursementDTO { get; set; }
        public List<ChargesDTO> padjustchargeslist { get; set; }
        public List<ChargesDTO> pchargereceiptslist { get; set; }

        public List<ExistingLoansDTO> pexistingloanslist { get; set; }
        public List<StageWisePaymentsDTO> pstagewisepaymentslist { get; set; }
        public List<PartPaymentsDTO> PartPaymentslist { get; set; }

        JournalVoucherDTO objJournalVoucherDTO = null;
        AccountingTransactionsDAL objJournalVoucher = null;
        PaymentsDTO objPaymentsDTO = null;


        public List<DisbursementReportDetailsDTO> pdisbursedlist { get; set; }
        public List<DisbursementReportDetailsDTO> ploantypeslist { get; set; }
        public List<DisbursementReportDetailsDTO> papplicanttypeslist { get; set; }
        public List<DisbursementReportDuesDetailsDTO> pdisbursementdueslist { get; set; }
        public DisbursementReportDTO pDisbursementReportDTO { get; set; }


        public async Task<DisbursementDTO> GetApprovedApplicationsByID(string vchapplicationid, string ConnectionString)
        {
            await Task.Run(() =>
            {

                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.applicantid,tap.loaninstalmentpaymentmode,tap.installmentamount,ta.applicationid,coalesce(tap.debtorsaccountid,0)debtorsaccountid,ta.contactreferenceid,ta.vchapplicationid,to_char(dateofapplication,'DD/MM/yyyy')dateofapplication,ta.contactreferenceid,ta.purposeofloan,ta.applicantname,tap.loantype,ta.loanname,ta.amountrequested,to_char(tap.approveddate,'DD/MM/yyyy') approveddate,tap.approvedby,tap.approvedloanamount,tap.loanpayin,tap.interesttype,tap.tenureofloan,tap.rateofinterest,tc.businessentitycontactno,tap.disbursementpayinmode,payinnature, payinduration , coalesce(approvedloanamount,0) -coalesce(totaldisburseamount,0) totaldisburseamount from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid join tblmstloanpayin t1 on  tap.loanpayin =t1.laonpayin left join (select vchapplicationid,sum(totaldisburseamount) totaldisburseamount from tbltransloandisbursement  group by vchapplicationid) t3 on ta.vchapplicationid=t3.vchapplicationid where tap.vchapplicationid='" + vchapplicationid + "'"))
                    {
                        if (dr.Read())
                        {
                            _DisbursementDTO = new DisbursementDTO
                            {
                                pdebtorsaccountid =
                                (dr["debtorsaccountid"] == null) ? 0 : Convert.ToInt64(dr["debtorsaccountid"]),
                                papplicantid =
                                (dr["applicantid"] == null) ? 0 : Convert.ToInt64(dr["applicantid"]),
                                papplicationid =
                                (dr["applicationid"] == null) ? 0 : Convert.ToInt64(dr["applicationid"]),
                                pvchapplicationid =
                                (dr["vchapplicationid"] == null) ? null : Convert.ToString(dr["vchapplicationid"]),
                                pdateofapplication =
                                (dr["Dateofapplication"] == null) ? null : Convert.ToString(dr["Dateofapplication"]),
                                pcontactreferenceid =
                                 (dr["contactreferenceid"] == null) ? null : Convert.ToString(dr["contactreferenceid"]),
                                pcontactreftype = "APPLICANT",
                                papplicantname =
                                (dr["applicantname"] == null) ? null : Convert.ToString(dr["applicantname"]),
                                ploanname =
                                (dr["loanname"] == null) ? null : Convert.ToString(dr["loanname"]),
                                ploantype =
                                (dr["loantype"] == null) ? null : Convert.ToString(dr["loantype"]),
                                ppurposeofloan =
                                 (dr["purposeofloan"] == null) ? null : Convert.ToString(dr["purposeofloan"]),
                                papproveddate =
                                  (dr["approveddate"] == null) ? null : Convert.ToString(dr["approveddate"]),
                                ploaninstalmentpaymentmode = Convert.ToString(dr["loaninstalmentpaymentmode"]),
                                papprovedloanamount =
                                  (dr["approvedloanamount"] == null) ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                ptenureofloan =
                                  (dr["Tenureofloan"] == null) ? 0 : Convert.ToDecimal(dr["Tenureofloan"]),
                                ploanpayin =
                                (dr["loanpayin"] == null) ? null : Convert.ToString(dr["loanpayin"]),
                                ppayinduration =
                                (dr["payinduration"] == null) ? null : Convert.ToString(dr["payinduration"]),
                                ppayinnature =
                                (dr["payinnature"] == null) ? null : Convert.ToString(dr["payinnature"]),
                                pinteresttype =
                                (dr["interesttype"] == null) ? null : Convert.ToString(dr["interesttype"]),
                                ploanpayoutamount =
                                (dr["totaldisburseamount"] == null) ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),
                                prateofinterest =
                                (dr["Rateofinterest"] == null) ? 0 : Convert.ToDecimal(dr["Rateofinterest"]),
                                pmobileno =
                                  (dr["businessentitycontactno"] == null) ? null : Convert.ToString(dr["businessentitycontactno"]),
                                pdisbursedtype =
                                    (dr["disbursementpayinmode"] == null) ? null : Convert.ToString(dr["disbursementpayinmode"]).ToUpper(),


                                ploaninstallment = (dr["installmentamount"] == null) ? 0 : Convert.ToDecimal(dr["installmentamount"]),
                                pInstallmentdatelist = GetApprovedApplicationsInstallmentdateData(vchapplicationid, ConnectionString),
                                padjustchargeslist = GetApprovedApplicationsChargesByID(vchapplicationid, ConnectionString),
                                pchargereceiptslist = GetApprovedApplicationsChargesReceiptsByID(vchapplicationid, ConnectionString),
                                pexistingloanslist = GetApprovedApplicatntExistingLoansDataByID(vchapplicationid, ConnectionString),
                                pstagewisepaymentslist = GetApprovedApplicationsStagePaymentsDataByID(vchapplicationid, ConnectionString),
                                pPartPaymentslist = GetApprovedApplicationsPartPaymentsDataByID(vchapplicationid, ConnectionString)
                            };
                        }
                    }

                    int existingcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tbltransloandisbursement  where  vchapplicationid ='" + vchapplicationid + "';"));
                    if (existingcount > 0)
                    {
                        _DisbursementDTO.pexistingdisbursestatus = "YES";
                        _DisbursementDTO.pinstallmentstartdate = Convert.ToDateTime(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select installmentstartdate from tbltransapprovedapplications  where  vchapplicationid ='" + vchapplicationid + "';")).ToString("dd/MM/yyyy");
                        _DisbursementDTO.ppreviousdisbursedtdate = Convert.ToDateTime(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select max(disbursementdate) from tbltransloandisbursement  where    vchapplicationid ='" + vchapplicationid + "'; ")).ToString("dd/MM/yyyy");
                    }
                    else
                        _DisbursementDTO.pexistingdisbursestatus = "NO";

                    string cancelchequesamount = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select coalesce(paidamount,0) from tbltranspaymentreference  t3 join tbltranspaymentvoucherdetails  t1 on t3.paymentid=t1.paymentid join tbltransapprovedapplications t2 on debtorsaccountid =debitaccountid where vchapplicationid =' " + vchapplicationid + "' and clearstatus in ('C','R')"));
                    if (string.IsNullOrEmpty(cancelchequesamount))
                        _DisbursementDTO.pcancelchequesamount = 0;
                    else

                        _DisbursementDTO.pcancelchequesamount = Convert.ToDecimal(cancelchequesamount);


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return _DisbursementDTO;
        }
        public List<InstallmentdateDTO> GetApprovedApplicationsInstallmentdateData(string vchapplicationid, string ConnectionString)
        {

            List<InstallmentdateDTO> pInstallmentdatelist = new List<InstallmentdateDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select upper(typeofinstallmentday)typeofinstallmentday ,disbursefromday  ,disbursetoday  ,installmentdueday  from tblmstloaninstallmentdateconfig t1 join tbltransapprovedapplications  t2 on t1.loanid=t2.loanid where vchapplicationid='" + vchapplicationid + "'"))
                {
                    while (dr.Read())
                    {
                        InstallmentdateDTO _InstallmentdateDTO = new InstallmentdateDTO
                        {
                            pinstallmentdatetype = (dr["typeofinstallmentday"] == null) ? null : Convert.ToString(dr["typeofinstallmentday"]),
                            pinstallmentdatedueno = (dr["installmentdueday"] == null) ? 0 : Convert.ToInt32(dr["installmentdueday"]),
                            pdisbursefromno = (dr["disbursefromday"] == null) ? 0 : Convert.ToInt32(dr["disbursefromday"]),
                            pdisbursetono = (dr["disbursetoday"] == null) ? 0 : Convert.ToInt32(dr["disbursetoday"]),
                        };
                        pInstallmentdatelist.Add(_InstallmentdateDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pInstallmentdatelist;
        }
        public List<ChargesDTO> GetApprovedApplicationsChargesByID(string vchapplicationid, string ConnectionString)
        {

            padjustchargeslist = new List<ChargesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct  t1.chargestype, t1.chargename, t1.totalchargeamount chargereceivableamount, t1.totalchargeamount as chargeamount,(coalesce(t1.totalchargeamount,0)-coalesce(receiptamount,0)) as chargesbalance, t1.chargepaymentmode ,t1.gsttype,t1.gstcalculationtype,coalesce(t1.gstpercentage,0)gstpercentage,coalesce(t3.cgstpercentage,0)cgstpercentage,coalesce(t3.sgstpercentage,0)sgstpercentage,coalesce(t3.utgstpercentage,0)utgstpercentage from tbltransapprovedloancharges t1 left join (select vchapplicationid,particularname,sum(receiptamount)receiptamount,sum(gstamount)gstamount from (select vchapplicationid,particularname, coalesce(sum(coalesce(waiveoffamount,0) + coalesce(detailsreceivedamount,0)),0) as receiptamount,sum(gstamount)gstamount from tbltransemireceiptdetails where   upper(particularstype)='CHARGES' and particularname <> 'PENAL INTEREST' and vchapplicationid ='" + vchapplicationid + "' group by vchapplicationid,particularname,gsttype,gstcalculationtype union all select vchapplicationid,chargename, coalesce(sum(coalesce(chargereceivableamount,0) + coalesce(chargewaiveoffamount,0)),0) as receiptamount,sum(gstamount)gstamount from tbltransloanchargesreceipt where   vchapplicationid ='" + vchapplicationid + "' group by vchapplicationid,chargename,gsttype,gstcalculationtype)x group by vchapplicationid,particularname) t2 on t1.vchapplicationid=t2.vchapplicationid and upper(t1.chargename)=upper(t2.particularname) left join tblmstgsttaxpercentage t3 on  t1.gstpercentage=t3.gstpercentage  where (coalesce(t1.totalchargeamount,0)-coalesce(receiptamount,0))>0 and t1.vchapplicationid ='" + vchapplicationid + "';"))
                {
                    while (dr.Read())
                    {
                        ChargesDTO _ChargesDTO = new ChargesDTO();

                        _ChargesDTO.pchargeid = 0;
                        _ChargesDTO.pchargetype = (dr["chargestype"] == null) ? "" : Convert.ToString(dr["chargestype"]);
                        _ChargesDTO.pchargename = Convert.ToString(dr["chargename"]);

                        _ChargesDTO.pgsttype = (dr["gsttype"] == null) ? null : Convert.ToString(dr["gsttype"]);
                        _ChargesDTO.pgstcalculationtype = (dr["gstcalculationtype"] == null) ? "" : Convert.ToString(dr["gstcalculationtype"]);
                        _ChargesDTO.pgstpercentage = (dr["gstpercentage"] == null) ? 0 : Convert.ToDecimal(dr["gstpercentage"]);

                        _ChargesDTO.pchargeamount = (dr["chargereceivableamount"] == null) ? 0 : Convert.ToDecimal(dr["chargereceivableamount"]);
                        _ChargesDTO.pchargebalance = (dr["chargesbalance"] == null) ? 0 : Convert.ToDecimal(dr["chargesbalance"]);
                        _ChargesDTO.pchargeactualamount = (dr["chargeamount"] == null) ? 0 : Convert.ToDecimal(dr["chargeamount"]);
                        _ChargesDTO.pchargeigstpercentage = (dr["gstpercentage"] == null) ? 0 : Convert.ToDecimal(dr["gstpercentage"]);
                        _ChargesDTO.pchargecgstpercentage = (dr["cgstpercentage"] == null) ? 0 : Convert.ToDecimal(dr["cgstpercentage"]);
                        _ChargesDTO.pchargesgstpercentage = (dr["sgstpercentage"] == null) ? 0 : Convert.ToDecimal(dr["sgstpercentage"]);
                        _ChargesDTO.pchargeutgstpercentage = (dr["utgstpercentage"] == null) ? 0 : Convert.ToDecimal(dr["utgstpercentage"]);

                        _ChargesDTO.pchargegstamount = 0;
                        _ChargesDTO.pchargecgstamount = 0;
                        _ChargesDTO.pchargesgstamount = 0;
                        _ChargesDTO.pchargeutgstamount = 0;
                        _ChargesDTO.pchargeigstamount = 0;


                        _ChargesDTO.pchargewaiveoffamount = 0;
                        if (Convert.ToString(dr["chargepaymentmode"]) == "R")
                        {
                            _ChargesDTO.pchargecollectstatus = "YES";
                            _ChargesDTO.pchargeadjuststatus = "NO";
                        }
                        else
                        {
                            _ChargesDTO.pchargecollectstatus = "NO";
                            _ChargesDTO.pchargeadjuststatus = "YES";
                        }

                        padjustchargeslist.Add(_ChargesDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return padjustchargeslist;
        }
        public List<ChargesDTO> GetApprovedApplicationsChargesReceiptsByID(string vchapplicationid, string ConnectionString)
        {

            padjustchargeslist = new List<ChargesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select particularname,coalesce(detailsreceivedamount,0)receiptamount,coalesce(waiveoffamount,0)waiveoffamount,t1.receiptno, detailsreceivabledate ,case when chrclearstatus='Y' then 'YES' else 'NO' end as chequestatus from tbltransemireceipt t3  join tbltransemireceiptdetails t1 on t3.emiid=t1.emiid join tbltransapprovedloancharges t2 on t1.vchapplicationid=t2.vchapplicationid and t1.particularname=t2.chargename where  (coalesce(detailsreceivedamount,0)+ coalesce(waiveoffamount,0))<>0 and  upper(particularstype)='CHARGES' and chrclearstatus in ('N','P','Y') and t1.vchapplicationid ='" + vchapplicationid + "'"))
                {
                    while (dr.Read())
                    {
                        ChargesDTO _ChargesDTO = new ChargesDTO();

                        _ChargesDTO.pchargename = (dr["particularname"] == null) ? "" : Convert.ToString(dr["particularname"]);

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["receiptamount"])))
                            _ChargesDTO.pchargeamount = Convert.ToDecimal(dr["receiptamount"]);
                        _ChargesDTO.pchargereceiptwaiveoffamount = (dr["waiveoffamount"] == null) ? 0 : Convert.ToDecimal(dr["waiveoffamount"]);
                        _ChargesDTO.pchargereceiptno = (dr["receiptno"] == null) ? "" : Convert.ToString(dr["receiptno"]);
                        _ChargesDTO.pchequestatus = (dr["chequestatus"] == null) ? "" : Convert.ToString(dr["chequestatus"]);
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["detailsreceivabledate"])))
                            _ChargesDTO.pchargereceiptdate = Convert.ToDateTime(dr["detailsreceivabledate"]).ToString("dd/MMM/yyyy");

                        padjustchargeslist.Add(_ChargesDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return padjustchargeslist;
        }
        public List<ExistingLoansDTO> GetApprovedApplicatntExistingLoansDataByID(string vchapplicationid, string ConnectionString)
        {

            pexistingloanslist = new List<ExistingLoansDTO>();
            try
            {
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  chargestype, chargename, chargereceivableamount, chargewaiveoffamount, chargepaymentmode from tbltransapprovedloancharges where vchapplicationid ='" + vchapplicationid + "'"))
                //{
                //    while (dr.Read())
                //    {
                //        ExistingLoansDTO _ExistingLoansDTO = new ExistingLoansDTO();



                //        _ExistingLoansDTO.pvchapplicationid = "";
                //        _ExistingLoansDTO.poutstandingamount = 0;
                //        _ExistingLoansDTO.padjustedamount = 0;

                //        pexistingloanslist.Add(_ExistingLoansDTO);

                //    }
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pexistingloanslist;
        }
        public List<StageWisePaymentsDTO> GetApprovedApplicationsStagePaymentsDataByID(string vchapplicationid, string ConnectionString)
        {

            pstagewisepaymentslist = new List<StageWisePaymentsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.stageno,t1.stagename  ,t1.paymentreleasepercentage ,t1.paymentreleaseamount,coalesce(t2.vchreceiptno,'')vchreceiptno,t2.disbursedate from tbltransapprovalpaymentstages  t1 left join ((select  stageno,stagename,vchapplicationid  ,string_agg (distinct to_char(disbursedate,'DD/MM/YYYY')::text,', ') disbursedate ,string_agg(distinct vchreceiptno,', ')vchreceiptno from tbltransdisbursestagewisepayments where vchapplicationid ='" + vchapplicationid + "' group by  stageno,stagename,vchapplicationid)) t2 on t1.vchapplicationid=t2.vchapplicationid and t1.stageno=t2.stageno  where t1.vchapplicationid ='" + vchapplicationid + "'"))
                {
                    while (dr.Read())
                    {
                        StageWisePaymentsDTO _StageWisePaymentsDTO = new StageWisePaymentsDTO();



                        _StageWisePaymentsDTO.pstageno = (dr["stageno"] == null) ? 0 : Convert.ToInt64(dr["stageno"]);
                        _StageWisePaymentsDTO.pstagename = (dr["stagename"] == null) ? "" : Convert.ToString(dr["stagename"]);
                        _StageWisePaymentsDTO.ppaymentreleasetype = "PERCENTAGE";

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["paymentreleasepercentage"])))
                            _StageWisePaymentsDTO.ppercentage = Convert.ToDecimal(dr["paymentreleasepercentage"]);

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["paymentreleaseamount"])))
                            _StageWisePaymentsDTO.pstageamount = Convert.ToDecimal(dr["paymentreleaseamount"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dr["vchreceiptno"])))
                        {
                            _StageWisePaymentsDTO.pstagepaidstatus = false;
                            _StageWisePaymentsDTO.pstagepreviouspaidstatus = "NO";
                            _StageWisePaymentsDTO.pstagestatus = false;
                        }
                        else
                        {
                            _StageWisePaymentsDTO.pstagepaidstatus = true;
                            _StageWisePaymentsDTO.pstagepreviouspaidstatus = "YES";
                            _StageWisePaymentsDTO.pstagestatus = true;
                        }

                        _StageWisePaymentsDTO.pstagepaidvouchernumber = (dr["vchreceiptno"] == null) ? "" : Convert.ToString(dr["vchreceiptno"]);

                        _StageWisePaymentsDTO.pstagepaiddate = (dr["disbursedate"] == null) ? "" : Convert.ToString(dr["disbursedate"]);



                        pstagewisepaymentslist.Add(_StageWisePaymentsDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pstagewisepaymentslist;
        }

        public List<PartPaymentsDTO> GetApprovedApplicationsPartPaymentsDataByID(string vchapplicationid, string ConnectionString)
        {

            PartPaymentslist = new List<PartPaymentsDTO>();
            PartPaymentsDTO _PartPaymentsDTO = null;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select vchapplicationid ,disburseamount totaldisburseamount , disbursementdate  ,disbursedby ,voucherno  ,modeofpayment  from tbltransloandisbursement  where   vchapplicationid ='" + vchapplicationid + "' union all SELECT  vchapplicationid,sum(receiptamount)receiptamount,receiptdate ,disbursedby ,vchreceiptno,chargepaymentmode from (select t1.vchapplicationid,receiptamount,receiptdate,disbursedby ,vchreceiptno,'CHARGES '|| chargepaymentmode chargepaymentmode from  tbltransloanchargesreceipt t1 join tbltransloandisbursement t2   on   voucherno=voucherrefno   where    t1.vchapplicationid ='" + vchapplicationid + "')x group by  vchapplicationid,disbursedby,receiptdate ,vchreceiptno,chargepaymentmode order by disbursementdate"))
                {
                    while (dr.Read())
                    {
                        _PartPaymentsDTO = new PartPaymentsDTO
                        {
                            pvchapplicationid = (dr["vchapplicationid"] == null) ? "" : Convert.ToString(dr["vchapplicationid"]),
                            pdisburedamount = (dr["totaldisburseamount"] == null) ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),
                            pdisburseddate = (dr["disbursementdate"] == null) ? "" : Convert.ToDateTime(dr["disbursementdate"]).ToString("dd-MM-yyyy"),
                            pdisbursedby = (dr["disbursedby"] == null) ? "" : Convert.ToString(dr["disbursedby"]),
                            pvoucheno = (dr["voucherno"] == null) ? "" : Convert.ToString(dr["voucherno"]),
                            pmodeofpayment = (dr["modeofpayment"] == null) ? "" : Convert.ToString(dr["modeofpayment"]),
                        };
                        PartPaymentslist.Add(_PartPaymentsDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return PartPaymentslist;
        }
        public bool SaveLoanDisbursement(DisbursementDTO _DisbursementDTO, string ConnectionString, out string _disbursementvoucherno)

        {
            bool IsSaved = false;
            bool IsAccountSaved = false;
            _disbursementvoucherno = string.Empty;
            long _disbursementid = 0;
            StringBuilder sbQuery = new StringBuilder();

            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (_DisbursementDTO.pmodofPayment.ToUpper() == "ADJUSTMENT")
                {
                    if (SaveAdjustmentVoucher(_DisbursementDTO, trans, out _disbursementvoucherno))
                    {
                        IsAccountSaved = true;

                    }
                    else
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                else
                {

                    if (SavePaymentVoucher(_DisbursementDTO, trans, out _disbursementvoucherno))
                    {
                        IsAccountSaved = true;

                    }
                    else
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                _DisbursementDTO.ppaymentid = _disbursementvoucherno;
                if (IsAccountSaved)
                {
                    _DisbursementDTO.pdisbursedby = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select employeename from tblmstusers  where userid  = " + _DisbursementDTO.pCreatedby + ";"));

                    if (string.IsNullOrEmpty(Convert.ToString(_DisbursementDTO.pchargesadjustedamount)))
                    {
                        _DisbursementDTO.pchargesadjustedamount = 0;
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(_DisbursementDTO.potherloansadjustedamount)))
                    {
                        _DisbursementDTO.potherloansadjustedamount = 0;
                    }
                    decimal? totaldisbursedamount = _DisbursementDTO.potherloansadjustedamount + _DisbursementDTO.pchargesadjustedamount + _DisbursementDTO.ploandisbusalamount;
                    string _query = "insert into tbltransloandisbursement(applicationid,vchapplicationid,disbursementdate,disbursedby,loansanctionamount,chargesadjustedamount,otherloansadjustedamount,disburseamount,totaldisburseamount,disbursementpayinmode,installmentstartdate,modeofpayment,voucherno,statusid,createdby,createddate) values(" + _DisbursementDTO.papplicationid + ",'" + ManageQuote(_DisbursementDTO.pvchapplicationid) + "','" + FormatDate(_DisbursementDTO.ppaymentdate) + "','" + ManageQuote(_DisbursementDTO.pdisbursedby) + "'," + _DisbursementDTO.papprovedloanamount + "," + _DisbursementDTO.pchargesadjustedamount + "," + _DisbursementDTO.potherloansadjustedamount + "," + _DisbursementDTO.ploandisbusalamount + "," + totaldisbursedamount + ",'" + ManageQuote(_DisbursementDTO.pdisbursedtype) + "','" + FormatDate(_DisbursementDTO.pinstallmentstartdate) + "','" + ManageQuote(_DisbursementDTO.pmodofPayment) + "','" + ManageQuote(_DisbursementDTO.ppaymentid) + "'," + Convert.ToInt32(Status.Active) + "," + _DisbursementDTO.pCreatedby + ",current_timestamp) returning disbursementid;";
                    _disbursementid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, _query));
                    _DisbursementDTO.pdisbursementid = _disbursementid;



                    sbQuery.Append(SaveLoanDisbursementStageWisePayments(_DisbursementDTO, trans));
                    sbQuery.Append(SaveLoanDisbursementAdjustedCharges(_DisbursementDTO, trans));
                    sbQuery.Append("SELECT fn_instalmentsinsert('" + _DisbursementDTO.ppaymentid + "')");

                    if (!string.IsNullOrEmpty(sbQuery.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());

                    }
                    trans.Commit();
                    IsSaved = true;
                }

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

        public bool SavePaymentVoucher(DisbursementDTO _DisbursementDTO, NpgsqlTransaction trans, out string _PaymentId)

        {
            bool IsSaved = false;

            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL _AccountingTransactionsDAL = new AccountingTransactionsDAL();
            try
            {
                PaymentVoucherDTO _PaymentVoucherDTO = new PaymentVoucherDTO();

                _PaymentVoucherDTO.ppaymentdate = _DisbursementDTO.ppaymentdate;
                _PaymentVoucherDTO.ptotalpaidamount = _DisbursementDTO.ploandisbusalamount;
                _PaymentVoucherDTO.pnarration = "LOAN AMOUNT DISBURSED TO " + _DisbursementDTO.pvchapplicationid;
                _PaymentVoucherDTO.pmodofPayment = _DisbursementDTO.pmodofPayment;
                _PaymentVoucherDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                _PaymentVoucherDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;
                _PaymentVoucherDTO.pbankname = _DisbursementDTO.pbankname;
                _PaymentVoucherDTO.pbankid = _DisbursementDTO.pbankid;
                _PaymentVoucherDTO.pbranchname = _DisbursementDTO.pbranchname;
                _PaymentVoucherDTO.ptranstype = _DisbursementDTO.ptranstype;
                _PaymentVoucherDTO.ptypeofpayment = _DisbursementDTO.ptypeofpayment;
                _PaymentVoucherDTO.pChequenumber = _DisbursementDTO.pChequenumber;
                _PaymentVoucherDTO.pCardNumber = _DisbursementDTO.pCardNumber;
                _PaymentVoucherDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                _PaymentVoucherDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;
                _PaymentVoucherDTO.pUpiid = _DisbursementDTO.pUpiid;
                _PaymentVoucherDTO.pUpiname = _DisbursementDTO.pUpiname;

                List<PaymentsDTO> ppaymentslist = new List<PaymentsDTO>();

                PaymentsDTO _PaymentsDTO = new PaymentsDTO();
                _PaymentsDTO.ppartyid = _DisbursementDTO.papplicantid;
                _PaymentsDTO.ppartyname = _DisbursementDTO.papplicantname;
                _PaymentsDTO.ppartyreferenceid = _DisbursementDTO.pcontactreferenceid;
                _PaymentsDTO.ppartyreftype = "APPLICANT";
                _PaymentsDTO.pisgstapplicable = false;
                _PaymentsDTO.pistdsapplicable = false;
                _PaymentsDTO.psubledgerid = _DisbursementDTO.pdebtorsaccountid;
                _PaymentsDTO.pledgername = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT parentaccountname from tblmstaccounts  where    accountid =" + _DisbursementDTO.pdebtorsaccountid + "; "));
                _PaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                _PaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;
                _PaymentsDTO.pamount = _DisbursementDTO.ploandisbusalamount;

                ppaymentslist.Add(_PaymentsDTO);

                _PaymentVoucherDTO.ppaymentslist = ppaymentslist;
                if (_AccountingTransactionsDAL.SavePaymentVoucher_ALL(_PaymentVoucherDTO, trans, "SAVE", out _PaymentId))
                {

                    IsSaved = true;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return IsSaved;

        }
        public string SaveLoanDisbursementStageWisePayments(DisbursementDTO _DisbursementDTO, NpgsqlTransaction trans)
        {
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                if (_DisbursementDTO.pstagewisepaymentslist != null)
                {
                    if (_DisbursementDTO.pstagewisepaymentslist.Count > 0)
                    {
                        for (int i = 0; i < _DisbursementDTO.pstagewisepaymentslist.Count; i++)
                        {
                            int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select  count(*)    from tbltransdisbursestagewisepayments  where    vchapplicationid ='" + ManageQuote(_DisbursementDTO.pvchapplicationid) + "' and stageno ='" + _DisbursementDTO.pstagewisepaymentslist[i].pstageno + "'"));
                            if (count == 0)
                                sbQuery.Append("insert into tbltransdisbursestagewisepayments(disbursementid,applicationid,vchapplicationid,vchreceiptno,disbursedate,stageno,stagename,paymentreleasetype,paymentreleasepercentage,paymentreleaseamount,statusid,createdby,createddate) values(" + _DisbursementDTO.pdisbursementid + "," + _DisbursementDTO.papplicationid + ",'" + ManageQuote(_DisbursementDTO.pvchapplicationid) + "','" + ManageQuote(_DisbursementDTO.ppaymentid) + "','" + FormatDate(_DisbursementDTO.ppaymentdate) + "'," + _DisbursementDTO.pstagewisepaymentslist[i].pstageno + ",'" + ManageQuote(_DisbursementDTO.pstagewisepaymentslist[i].ppaymentreleasetype) + "','" + ManageQuote(_DisbursementDTO.pstagewisepaymentslist[i].ppaymentreleasetype) + "'," + (_DisbursementDTO.pstagewisepaymentslist[i].ppercentage) + "," + (_DisbursementDTO.pstagewisepaymentslist[i].pstageamount) + "," + Convert.ToInt32(Status.Active) + "," + _DisbursementDTO.pCreatedby + ",current_timestamp);");
                        }
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }
            return Convert.ToString(sbQuery);
        }
        public string SaveLoanDisbursementAdjustedCharges(DisbursementDTO _DisbursementDTO, NpgsqlTransaction trans)
        {
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                if (_DisbursementDTO.padjustchargeslist != null)
                {
                    if (_DisbursementDTO.padjustchargeslist.Count > 0)
                    {
                        if (_DisbursementDTO.pchargesadjustedamount > 0)
                        {
                            objJournalVoucherDTO = new JournalVoucherDTO();
                            List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                            objJournalVoucherDTO.pjvdate = _DisbursementDTO.ppaymentdate;
                            objJournalVoucherDTO.pnarration = "BEING JV PASSED TOWARDS TDS AMOUNT";
                            objJournalVoucherDTO.pmodoftransaction = "AUTO";

                            objJournalVoucherDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                            objJournalVoucherDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                            objPaymentsDTO = new PaymentsDTO();
                            objPaymentsDTO.ppartyid = _DisbursementDTO.papplicantid;
                            objPaymentsDTO.ppartyname = _DisbursementDTO.papplicantname;
                            objPaymentsDTO.ppartyreferenceid = _DisbursementDTO.pcontactreferenceid;
                            objPaymentsDTO.ppartyreftype = _DisbursementDTO.pcontactreftype;

                            objPaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                            objPaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                            objPaymentsDTO.ptranstype = "D";
                            objPaymentsDTO.psubledgerid = _DisbursementDTO.pdebtorsaccountid;
                            objPaymentsDTO.pamount = _DisbursementDTO.pchargesadjustedamount;
                            _Paymentslist.Add(objPaymentsDTO);

                            for (int i = 0; i < _DisbursementDTO.padjustchargeslist.Count; i++)
                            {
                                long chargeaccountid = 0;

                                if (_DisbursementDTO.padjustchargeslist[i].pchargebalance > 0)
                                {
                                    objPaymentsDTO = new PaymentsDTO();
                                    objPaymentsDTO.ptranstype = "C";

                                    if (_DisbursementDTO.padjustchargeslist[i].pgsttype == "IGST")
                                    {
                                        _DisbursementDTO.padjustchargeslist[i].pchargeigstamount = Math.Round((_DisbursementDTO.padjustchargeslist[i].pchargebalance * _DisbursementDTO.padjustchargeslist[i].pchargeigstpercentage) / (100 + _DisbursementDTO.padjustchargeslist[i].pgstpercentage));
                                    }
                                    else if (_DisbursementDTO.padjustchargeslist[i].pgsttype == "CGST/SGST")
                                    {
                                        _DisbursementDTO.padjustchargeslist[i].pchargecgstamount = Math.Round((_DisbursementDTO.padjustchargeslist[i].pchargebalance * _DisbursementDTO.padjustchargeslist[i].pchargecgstpercentage) / (100 + _DisbursementDTO.padjustchargeslist[i].pgstpercentage));

                                        _DisbursementDTO.padjustchargeslist[i].pchargesgstamount = Math.Round((_DisbursementDTO.padjustchargeslist[i].pchargebalance * _DisbursementDTO.padjustchargeslist[i].pchargesgstpercentage) / (100 + _DisbursementDTO.padjustchargeslist[i].pgstpercentage));

                                    }
                                    else if (_DisbursementDTO.padjustchargeslist[i].pgsttype == "CGST/UTGST")
                                    {
                                        _DisbursementDTO.padjustchargeslist[i].pchargecgstamount = Math.Round((_DisbursementDTO.padjustchargeslist[i].pchargebalance * _DisbursementDTO.padjustchargeslist[i].pchargecgstpercentage) / (100 + _DisbursementDTO.padjustchargeslist[i].pgstpercentage));

                                        _DisbursementDTO.padjustchargeslist[i].pchargeutgstamount = Math.Round((_DisbursementDTO.padjustchargeslist[i].pchargebalance * _DisbursementDTO.padjustchargeslist[i].pchargeutgstpercentage) / (100 + _DisbursementDTO.padjustchargeslist[i].pgstpercentage));

                                    }

                                    _DisbursementDTO.padjustchargeslist[i].pchargeactualamount = _DisbursementDTO.padjustchargeslist[i].pchargebalance - (_DisbursementDTO.padjustchargeslist[i].pchargeigstamount + _DisbursementDTO.padjustchargeslist[i].pchargecgstamount + _DisbursementDTO.padjustchargeslist[i].pchargesgstamount + _DisbursementDTO.padjustchargeslist[i].pchargeutgstamount);
                                    _DisbursementDTO.padjustchargeslist[i].pchargegstamount = _DisbursementDTO.padjustchargeslist[i].pchargeigstamount + _DisbursementDTO.padjustchargeslist[i].pchargecgstamount + _DisbursementDTO.padjustchargeslist[i].pchargesgstamount + _DisbursementDTO.padjustchargeslist[i].pchargeutgstamount;



                                    chargeaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select chargesaccountid from tblmstchargestypes  where chargename ='" + _DisbursementDTO.padjustchargeslist[i].pchargename + "';"));

                                    string chargeaccountname = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountname from tblmstaccounts  where accountid  =" + _DisbursementDTO.pdebtorsaccountid + ";"));

                                    chargeaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + chargeaccountname + "'," + chargeaccountid + ",'3'," + _DisbursementDTO.pCreatedby + ")"));

                                    objPaymentsDTO.psubledgerid = chargeaccountid;
                                    objPaymentsDTO.pamount = Convert.ToDecimal(_DisbursementDTO.padjustchargeslist[i].pchargeactualamount);

                                    objPaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                                    objPaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                                    _Paymentslist.Add(objPaymentsDTO);

                                    decimal totalgstamount = _DisbursementDTO.padjustchargeslist[i].pchargeigstamount + _DisbursementDTO.padjustchargeslist[i].pchargecgstamount + _DisbursementDTO.padjustchargeslist[i].pchargesgstamount + _DisbursementDTO.padjustchargeslist[i].pchargeutgstamount;

                                    if (totalgstamount > 0)
                                    {
                                        objPaymentsDTO = new PaymentsDTO();
                                        objPaymentsDTO.ptranstype = "C";

                                        chargeaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  like 'GST OUTPUT';"));
                                        chargeaccountname = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountname from tblmstaccounts  where accountid  =" + _DisbursementDTO.pdebtorsaccountid + ";"));
                                        chargeaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + chargeaccountname + "'," + chargeaccountid + ",'3'," + _DisbursementDTO.pCreatedby + ")"));
                                        objPaymentsDTO.psubledgerid = chargeaccountid;
                                        objPaymentsDTO.pamount = totalgstamount;

                                        objPaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                                        objPaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                                        _Paymentslist.Add(objPaymentsDTO);
                                    }

                                }
                            }
                            objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                            string refjvnumber = "";
                            //objJournalVoucherDTO.pStatusid = Convert.ToInt32(Status.Active);
                            objJournalVoucher = new AccountingTransactionsDAL();
                            objJournalVoucher.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                            _DisbursementDTO.pvoucherrefno = refjvnumber;
                            for (int i = 0; i < _DisbursementDTO.padjustchargeslist.Count; i++)
                            {


                                sbQuery.Append("insert into tbltransloanchargesreceipt (applicationid,vchapplicationid,voucherrefno,chargename,chargereceivableamount,chargewaiveoffamount,chargepaymentmode,vchreceiptno,receiptamount,totalchargeamount,receiptdate,statusid,createdby,createddate,gsttype ,gstcalculationtype ,gstpercentage ,gstamount  ,igstamount , cgstamount ,sgstamount,utgstamount) values(" + _DisbursementDTO.papplicationid + ",'" + ManageQuote(_DisbursementDTO.pvchapplicationid) + "','" + ManageQuote(_DisbursementDTO.ppaymentid) + "','" + ManageQuote(_DisbursementDTO.padjustchargeslist[i].pchargename) + "'," + _DisbursementDTO.padjustchargeslist[i].pchargebalance + "," + (_DisbursementDTO.padjustchargeslist[i].pchargewaiveoffamount) +
                               ", 'ADUSTMENT', '" + refjvnumber + "'," + (_DisbursementDTO.padjustchargeslist[i].pchargebalance) + "," + (_DisbursementDTO.padjustchargeslist[i].pchargeactualamount) + ",'" + FormatDate(_DisbursementDTO.ppaymentdate) + "'," + Convert.ToInt32(Status.Active) + "," + _DisbursementDTO.pCreatedby + ",current_timestamp, '" + ManageQuote(_DisbursementDTO.padjustchargeslist[i].pgsttype) + "', '" + ManageQuote(_DisbursementDTO.padjustchargeslist[i].pgstcalculationtype) + "'," + (_DisbursementDTO.padjustchargeslist[i].pgstpercentage) +
                               "," + (_DisbursementDTO.padjustchargeslist[i].pchargegstamount) +
                               "," + (_DisbursementDTO.padjustchargeslist[i].pchargeigstamount) +
                               "," + (_DisbursementDTO.padjustchargeslist[i].pchargecgstamount) +
                               "," + (_DisbursementDTO.padjustchargeslist[i].pchargesgstamount) +
                               "," + (_DisbursementDTO.padjustchargeslist[i].pchargeutgstamount) +
                               ");");
                            }
                            ///////////////
                            ///
                            sbQuery.Append("insert into tbltransinstalmentsdetails(modeofpayment,instalmentid,servicerundate, applicationid, vchapplicationid,  particulars,     paidreceiptno, paidreceiptdate, particularstype, paidchargesamount, gsttype, gstcaltype, gstpercentage, paidgstamount,statusid, createdby, createddate) select 'Adjustment', 0,receiptdate,applicationid, vchapplicationid, upper(chargename)chargename,vchreceiptno,receiptdate,'CHARGES RECEIVABLE',totalchargeamount, gsttype, gstcalculationtype, gstpercentage, gstamount, statusid, createdby, createddate from tbltransloanchargesreceipt where voucherrefno ='" + ManageQuote(_DisbursementDTO.ppaymentid) + "'  union all select 'Adjustment', 0,receiptdate,applicationid, vchapplicationid, upper(chargename)chargename,vchreceiptno,receiptdate,'CHARGES RECEIVED',totalchargeamount, gsttype, gstcalculationtype, gstpercentage, gstamount, statusid, createdby, createddate from tbltransloanchargesreceipt where voucherrefno ='" + ManageQuote(_DisbursementDTO.ppaymentid) + "';");
                        }

                    }
                }


            }
            catch (Exception)
            {

                throw;
            }
            return Convert.ToString(sbQuery);
        }

        public bool SaveAdjustmentVoucher(DisbursementDTO _DisbursementDTO, NpgsqlTransaction trans, out string refjvnumber)
        {
            bool IsSaved = false;
            refjvnumber = "";
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                //if (_DisbursementDTO.padjustchargeslist != null)
                //{
                //    if (_DisbursementDTO.padjustchargeslist.Count > 0)
                //    {
                //        if (_DisbursementDTO.pchargesadjustedamount > 0)
                //        {
                objJournalVoucherDTO = new JournalVoucherDTO();
                List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                objJournalVoucherDTO.pjvdate = _DisbursementDTO.ppaymentdate;
                objJournalVoucherDTO.pnarration = "BEING JV PASSED TOWARDS ADJUSTMENT TO SHOW ROOM AMOUNT";
                objJournalVoucherDTO.pmodoftransaction = "AUTO";
                objJournalVoucherDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                objJournalVoucherDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ppartyid = _DisbursementDTO.papplicantid;
                objPaymentsDTO.ppartyname = _DisbursementDTO.papplicantname;
                objPaymentsDTO.ppartyreferenceid = _DisbursementDTO.pcontactreferenceid;
                objPaymentsDTO.ppartyreftype = _DisbursementDTO.pcontactreftype;
                objPaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                objPaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                objPaymentsDTO.ptranstype = "D";
                objPaymentsDTO.psubledgerid = _DisbursementDTO.pdebtorsaccountid;
                objPaymentsDTO.pamount = _DisbursementDTO.ploandisbusalamount;
                _Paymentslist.Add(objPaymentsDTO);

                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ppartyid = _DisbursementDTO.papplicantid;
                objPaymentsDTO.ppartyname = _DisbursementDTO.papplicantname;
                objPaymentsDTO.ppartyreferenceid = _DisbursementDTO.pcontactreferenceid;
                objPaymentsDTO.ppartyreftype = _DisbursementDTO.pcontactreftype;
                objPaymentsDTO.pCreatedby = _DisbursementDTO.pCreatedby;
                objPaymentsDTO.ptypeofoperation = _DisbursementDTO.ptypeofoperation;

                objPaymentsDTO.ptranstype = "C";
                objPaymentsDTO.psubledgerid = _DisbursementDTO.psubledgerid;
                objPaymentsDTO.pamount = _DisbursementDTO.ploandisbusalamount;
                _Paymentslist.Add(objPaymentsDTO);


                objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;

                //objJournalVoucherDTO.pStatusid = Convert.ToInt32(Status.Active);
                objJournalVoucher = new AccountingTransactionsDAL();
                if (objJournalVoucher.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber))
                {
                    _DisbursementDTO.pvoucherrefno = refjvnumber;
                    IsSaved = true;
                }

                ///////////////
                //        }

                //    }
                //}


            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        public async Task<DisbursementViewDTO> GetDisbursementViewData(string ConnectionString)
        {
            DisbursementViewDTO _DisbursementViewDTO = new DisbursementViewDTO();
            List<DisbursementViewDetailsDTO> papprovedloanslist = new List<DisbursementViewDetailsDTO>();
            List<DisbursementViewDetailsDTO> ptotaldisbusedlist = new List<DisbursementViewDetailsDTO>();
            List<DisbursementViewDetailsDTO> ppartdisbursedlist = new List<DisbursementViewDetailsDTO>();
            List<DisbursementViewDetailsDTO> pstagedisbursedlist = new List<DisbursementViewDetailsDTO>();
            await Task.Run(() =>
            {

                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.vchapplicationid,tap.loantype,tap.loanname,ta.applicantname,tc.businessentitycontactno,tap.approvedloanamount from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid join tblmstloanpayin t1 on  tap.loanpayin =t1.laonpayin  where ta.vchapplicationid not in (select vchapplicationid from tbltransloandisbursement  ) order by tap.loantype,tap.loanname,ta.vchapplicationid"))
                    {
                        while (dr.Read())
                        {
                            DisbursementViewDetailsDTO _DisbursementViewDetailsDTO = new DisbursementViewDetailsDTO
                            {
                                pvchapplicationid = Convert.ToString(dr["vchapplicationid"]),
                                ploantype = Convert.ToString(dr["loantype"]),
                                ploanname = Convert.ToString(dr["loanname"]),
                                papplicantname = Convert.ToString(dr["applicantname"]),
                                pcontactno = Convert.ToInt64(dr["businessentitycontactno"]),
                                ploanapprovedamount = Convert.ToDecimal(dr["approvedloanamount"]),
                            };
                            papprovedloanslist.Add(_DisbursementViewDetailsDTO);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.vchapplicationid,tap.loantype,tap.loanname,ta.applicantname,tc.businessentitycontactno,coalesce( approvedloanamount,0)approvedloanamount,sum(totaldisburseamount)totaldisburseamount from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid join tblmstloanpayin t1 on  tap.loanpayin =t1.laonpayin join  tbltransloandisbursement t3 on ta.vchapplicationid=t3.vchapplicationid  group by ta.vchapplicationid,tap.loantype,tap.loanname,ta.applicantname,tc.businessentitycontactno,coalesce( approvedloanamount,0) having  coalesce( approvedloanamount,0)=coalesce( sum(totaldisburseamount),0) order by tap.loantype,tap.loanname,ta.vchapplicationid"))
                    {
                        while (dr.Read())
                        {
                            DisbursementViewDetailsDTO _DisbursementViewDetailsDTO = new DisbursementViewDetailsDTO
                            {
                                pvchapplicationid = Convert.ToString(dr["vchapplicationid"]),
                                ploantype = Convert.ToString(dr["loantype"]),
                                ploanname = Convert.ToString(dr["loanname"]),
                                papplicantname = Convert.ToString(dr["applicantname"]),
                                pcontactno = Convert.ToInt64(dr["businessentitycontactno"]),
                                pdisbursementamount = Convert.ToDecimal(dr["totaldisburseamount"]),
                            };
                            ptotaldisbusedlist.Add(_DisbursementViewDetailsDTO);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.vchapplicationid,tap.loantype,tap.loanname,ta.applicantname,tc.businessentitycontactno,tap.approvedloanamount,totaldisburseamount,(tap.approvedloanamount-totaldisburseamount)disbursementbalance from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid join tblmstloanpayin t1 on  tap.loanpayin =t1.laonpayin join  (select vchapplicationid ,sum( totaldisburseamount)totaldisburseamount from tbltransloandisbursement  where   upper(disbursementpayinmode) <> 'STAGE PAYMENT'   group by vchapplicationid) t3 on ta.vchapplicationid=t3.vchapplicationid  where coalesce( approvedloanamount,0)<>coalesce( totaldisburseamount,0) order by tap.loantype,tap.loanname,ta.vchapplicationid;"))
                    {
                        while (dr.Read())
                        {
                            DisbursementViewDetailsDTO _DisbursementViewDetailsDTO = new DisbursementViewDetailsDTO
                            {
                                pvchapplicationid = Convert.ToString(dr["vchapplicationid"]),
                                ploantype = Convert.ToString(dr["loantype"]),
                                ploanname = Convert.ToString(dr["loanname"]),
                                papplicantname = Convert.ToString(dr["applicantname"]),
                                pcontactno = Convert.ToInt64(dr["businessentitycontactno"]),
                                ploanapprovedamount = Convert.ToDecimal(dr["approvedloanamount"]),
                                pdisbursementamount = Convert.ToDecimal(dr["totaldisburseamount"]),
                                pdisbursementbalance = Convert.ToDecimal(dr["disbursementbalance"]),
                            };
                            ppartdisbursedlist.Add(_DisbursementViewDetailsDTO);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ta.vchapplicationid,tap.loantype,tap.loanname,ta.applicantname,tc.businessentitycontactno,tap.approvedloanamount,totaldisburseamount,(tap.approvedloanamount-totaldisburseamount)disbursementbalance,balancestages from tabapplication ta join tbltransapprovedapplications tap on ta.vchapplicationid=tap.vchapplicationid  join  tblmstcontact tc on ta.contactreferenceid=tc.contactreferenceid join tblmstloanpayin t1 on  tap.loanpayin =t1.laonpayin join  (select vchapplicationid ,sum( totaldisburseamount)totaldisburseamount from tbltransloandisbursement  where   upper(disbursementpayinmode)= 'STAGE PAYMENT'   group by vchapplicationid) t3 on ta.vchapplicationid=t3.vchapplicationid join (select t1.vchapplicationid,approvedstages-coalesce(disbursedstages,0) balancestages from (select vchapplicationid ,count(*) as approvedstages   from tbltransapprovalpaymentstages  group by vchapplicationid ) t1 left join ( select vchapplicationid ,count(*) disbursedstages from ( select distinct vchapplicationid ,stageno    from tbltransdisbursestagewisepayments )x group by vchapplicationid )t2 on t1.vchapplicationid=t2.vchapplicationid)t4 on ta.vchapplicationid=t4.vchapplicationid  where  coalesce( approvedloanamount,0)<>coalesce( totaldisburseamount,0) order by tap.loantype,tap.loanname,ta.vchapplicationid ;"))
                    {
                        while (dr.Read())
                        {
                            DisbursementViewDetailsDTO _DisbursementViewDetailsDTO = new DisbursementViewDetailsDTO
                            {
                                pvchapplicationid = Convert.ToString(dr["vchapplicationid"]),
                                ploantype = Convert.ToString(dr["loantype"]),
                                ploanname = Convert.ToString(dr["loanname"]),
                                papplicantname = Convert.ToString(dr["applicantname"]),
                                pcontactno = Convert.ToInt64(dr["businessentitycontactno"]),
                                ploanapprovedamount = Convert.ToDecimal(dr["approvedloanamount"]),
                                pdisbursementamount = Convert.ToDecimal(dr["totaldisburseamount"]),
                                pdisbursementbalance = Convert.ToDecimal(dr["disbursementbalance"]),
                                pendingstatgesno = Convert.ToInt32(dr["balancestages"]),
                            };
                            pstagedisbursedlist.Add(_DisbursementViewDetailsDTO);
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });

            _DisbursementViewDTO.papprovedloanslist = papprovedloanslist;
            _DisbursementViewDTO.ptotaldisbusedlist = ptotaldisbusedlist;
            _DisbursementViewDTO.ppartdisbursedlist = ppartdisbursedlist;
            _DisbursementViewDTO.pstagedisbursedlist = pstagedisbursedlist;

            return _DisbursementViewDTO;
        }

        public async Task<EmiChartReportDTO> GetEmiChartReport(string vchapplicationid, string ConnectionString)
        {
            EmiChartReportDTO _EmiChartReportDTO = new EmiChartReportDTO();
            List<EmiInstalmentDetailsDTO> pEmichartlist = new List<EmiInstalmentDetailsDTO>();
            await Task.Run(() =>
            {
                string strQuery = "select vchapplicationid,instalmentno,instalmentdate,instalmentprinciple,instalmentinterest,instalmentamount from tbltransinstalments" +
                    "  where vchapplicationid ='" + vchapplicationid + "' and instalmentamount  <>0 order by instalmentno";

                string strQuery1 = "select applicantname,approvedloanamount,a.tenureofloan,a.rateofinterest,a.installmentamount,a.loanpayin,a.vchapplicationid,a.installmentstartdate, a.loanname,a.loantype,a.loaninstalmentpaymentmode,coalesce(a.partprinciplepaidinterval,0) as partprinciplepaidinterval,a.interesttype from tbltransapprovedapplications a join tabapplication b on a.vchapplicationid = b.vchapplicationid where a.vchapplicationid = '" + vchapplicationid + "'";
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            EmiInstalmentDetailsDTO _InstalmentsgenerationDTO = new EmiInstalmentDetailsDTO
                            {
                                pInstalmentno = Convert.ToInt32(dr["instalmentno"]),
                                pInstalmentdate = Convert.ToDateTime(dr["instalmentdate"]).ToString("dd-MMM-yyyy"),
                                //  Convert.ToString(dr["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(dr["parentid"]);
                                pInstalmentprinciple = Convert.ToDecimal(dr["instalmentprinciple"]),
                                pInstalmentinterest = Convert.ToDecimal(dr["instalmentinterest"]),
                                pInstalmentamount = Convert.ToDecimal(dr["instalmentamount"]),

                            };
                            pEmichartlist.Add(_InstalmentsgenerationDTO);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery1))
                    {
                        while (dr.Read())
                        {
                            _EmiChartReportDTO.pLoanname = Convert.ToString(dr["loanname"]);
                            _EmiChartReportDTO.pLoanamount = Convert.ToDecimal(dr["approvedloanamount"]);
                            _EmiChartReportDTO.pTenureofloan = Convert.ToInt32(dr["tenureofloan"]);
                            _EmiChartReportDTO.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                            _EmiChartReportDTO.pinstallmentamount = Convert.ToDecimal(dr["installmentamount"]);
                            _EmiChartReportDTO.pPayinmode = Convert.ToString(dr["loanpayin"]);
                            _EmiChartReportDTO.pEmistartdate = Convert.ToDateTime(dr["installmentstartdate"]).ToString("dd-MMM-yyyy");
                            _EmiChartReportDTO.pApplicationid = Convert.ToString(dr["vchapplicationid"]);
                            _EmiChartReportDTO.pApplicantname = Convert.ToString(dr["applicantname"]);
                            _EmiChartReportDTO.PLoantype = Convert.ToString(dr["loantype"]);
                            _EmiChartReportDTO.pInstalmentpaymentmode = Convert.ToString(dr["loaninstalmentpaymentmode"]);
                            _EmiChartReportDTO.ppartprinciplepaidinterval = Convert.ToInt64(dr["partprinciplepaidinterval"]);
                            _EmiChartReportDTO.pInteresttype = Convert.ToString(dr["interesttype"]);


                        }
                    }
                    _EmiChartReportDTO.pEmichartlist = pEmichartlist;
                }
                catch (Exception)
                {

                    throw;
                }

            });
            return _EmiChartReportDTO;
        }


        public async Task<DisbursementReportDTO> GetDisbursedReportDetails(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString)
        {
            //DisbursementReportDTO _DisbursementReportDTO = new DisbursementReportDTO();
            await Task.Run(() =>
            {

                try
                {
                    _DisbursementReportDTO.pdisbursedlist = GetDisbursedReport(_DisbursementReportDTO, ConnectionString);
                    _DisbursementReportDTO.ploantypeslist = GetDisbursedReportbyLoantype(_DisbursementReportDTO, ConnectionString);
                    _DisbursementReportDTO.papplicanttypeslist = GetDisbursedReportbyApplicantype(_DisbursementReportDTO, ConnectionString);

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return _DisbursementReportDTO;
        }


        public List<DisbursementReportDetailsDTO> GetDisbursedReport(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString)
        {

            pdisbursedlist = new List<DisbursementReportDetailsDTO>();

            try
            {
                string fromdate = _DisbursementReportDTO.pfromdate;
                string todate = _DisbursementReportDTO.ptodate;
                //string todate = Convert.ToDateTime(fromdate).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,type,coalesce(loancount,0)loancount,coalesce(disbursedamount,0)disbursedamount from (select 1 as recordid, 'No of Loans at the beginning of the month' as type,count(distinct t1.vchapplicationid) as loancount,sum(totaldisburseamount) as disbursedamount from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where loanstatusid in (13) and t1.disbursementdate <'" + FormatDate(fromdate) + "' union all select 2 as recordid, 'Loans disbursed during the month (+)' as type,count(distinct vchapplicationid),sum(totaldisburseamount) as disbursedamount from tbltransloandisbursement  where disbursementdate  between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' union all select 3 as recordid, 'Loans closed during the month (-)' as type,count(distinct t1.vchapplicationid),coalesce(sum(totaldisburseamount),0) as disbursedamount from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where loanstatusid  in (14,15) and loancloseddate   between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' union all select 4 as recordid,'Closing Loans',sum(count) as count,sum(disbursedamount) as disbursedamount from (select count(distinct t1.vchapplicationid)count,coalesce(sum(totaldisburseamount),0) as disbursedamount from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where loanstatusid in (13) and t1.disbursementdate <'" + FormatDate(fromdate) + "' union all select count(distinct vchapplicationid),coalesce(sum(totaldisburseamount),0) as disbursedamount from tbltransloandisbursement  where disbursementdate  between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' union all select - count(distinct t1.vchapplicationid),-coalesce(sum(totaldisburseamount),0) as disbursedamount from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where loanstatusid  in (14,15) and loancloseddate   between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "')x )x order by recordid"))
                {
                    while (dr.Read())
                    {
                        DisbursementReportDetailsDTO _DisbursementReportDetailsDTO = new DisbursementReportDetailsDTO();

                        _DisbursementReportDetailsDTO.precordid = (dr["recordid"] == null) ? 0 : Convert.ToInt32(dr["recordid"]);

                        _DisbursementReportDetailsDTO.ptype = (dr["type"] == null) ? "" : Convert.ToString(dr["type"]);

                        _DisbursementReportDetailsDTO.ploancount = (dr["loancount"] == null) ? 0 : Convert.ToInt32(dr["loancount"]);
                        _DisbursementReportDetailsDTO.ploanvalue = (dr["disbursedamount"] == null) ? 0 : Convert.ToDecimal(dr["disbursedamount"]);

                        pdisbursedlist.Add(_DisbursementReportDetailsDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pdisbursedlist;
        }
        public List<DisbursementReportDetailsDTO> GetDisbursedReportbyLoantype(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString)
        {

            ploantypeslist = new List<DisbursementReportDetailsDTO>();

            try
            {
                //string fromdate = "01-" + month;
                string fromdate = _DisbursementReportDTO.pfromdate;
                string todate = _DisbursementReportDTO.ptodate;
                //string todate = Convert.ToDateTime(fromdate).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  loantype as type,count(distinct t1.vchapplicationid) loancount,coalesce(sum(totaldisburseamount),0) as disbursedamount,round(coalesce(sum(totaldisburseamount),0)/count(distinct t1.vchapplicationid),2) as avgdisbursed from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where t1.disbursementdate   between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by loantype"))
                {
                    while (dr.Read())
                    {
                        DisbursementReportDetailsDTO _DisbursementReportDetailsDTO = new DisbursementReportDetailsDTO();



                        _DisbursementReportDetailsDTO.ptype = (dr["type"] == null) ? "" : Convert.ToString(dr["type"]);

                        _DisbursementReportDetailsDTO.ploancount = (dr["loancount"] == null) ? 0 : Convert.ToInt32(dr["loancount"]);
                        _DisbursementReportDetailsDTO.ploanvalue = (dr["disbursedamount"] == null) ? 0 : Convert.ToDecimal(dr["disbursedamount"]);
                        _DisbursementReportDetailsDTO.ploanaveragevalue = (dr["avgdisbursed"] == null) ? 0 : Convert.ToDecimal(dr["avgdisbursed"]);

                        ploantypeslist.Add(_DisbursementReportDetailsDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ploantypeslist;
        }

        public List<DisbursementReportDetailsDTO> GetDisbursedReportbyApplicantype(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString)
        {

            papplicanttypeslist = new List<DisbursementReportDetailsDTO>();

            try
            {
                string fromdate = _DisbursementReportDTO.pfromdate;
                string todate = _DisbursementReportDTO.ptodate;
                //string todate = Convert.ToDateTime(fromdate).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  applicanttype as type,count(distinct t1.vchapplicationid) loancount,coalesce(sum(totaldisburseamount),0) as disbursedamount,round(coalesce(sum(totaldisburseamount),0)/count(distinct t1.vchapplicationid),2) as avgdisbursed from tbltransloandisbursement t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid where t1.disbursementdate   between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' group by applicanttype"))
                {
                    while (dr.Read())
                    {
                        DisbursementReportDetailsDTO _DisbursementReportDetailsDTO = new DisbursementReportDetailsDTO();



                        _DisbursementReportDetailsDTO.ptype = (dr["type"] == null) ? "" : Convert.ToString(dr["type"]);

                        _DisbursementReportDetailsDTO.ploancount = (dr["loancount"] == null) ? 0 : Convert.ToInt32(dr["loancount"]);
                        _DisbursementReportDetailsDTO.ploanvalue = (dr["disbursedamount"] == null) ? 0 : Convert.ToDecimal(dr["disbursedamount"]);
                        _DisbursementReportDetailsDTO.ploanaveragevalue = (dr["avgdisbursed"] == null) ? 0 : Convert.ToDecimal(dr["avgdisbursed"]);

                        papplicanttypeslist.Add(_DisbursementReportDetailsDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return papplicanttypeslist;
        }

        public async Task<List<DisbursementReportDuesDetailsDTO>> GetDisbursedReportDuesDetails(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString)
        {
            pdisbursementdueslist = new List<DisbursementReportDuesDetailsDTO>();

            await Task.Run(() =>
           {

               try
               {
                   string fromdate = _DisbursementReportDTO.pfromdate;
                   string todate = _DisbursementReportDTO.ptodate;

                   //string todate = Convert.ToDateTime(fromdate).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
                   string query = "";
                   if (_DisbursementReportDTO.ptype.ToUpper() == "LOAN TYPE")
                       query = "select applicanttype,t1.loantype,applicantname,sum(t3.totaldisburseamount)totaldisburseamount,approvedloanamount,t1.tenureofloan,0 as paidampunt,0 as dueamount,0 as penaltyamount,0 as penaltypaid,0 as penaltydue, loanstatus,salespersonname,t1.vchapplicationid from tbltransapprovedapplications t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid join tbltransloandisbursement t3 on t1.vchapplicationid=t3.vchapplicationid where t3.disbursementdate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and case when '" + ManageQuote(_DisbursementReportDTO.pfieldname) + "' ='ALL' then t1.loantype <>'ALL' else t1.loantype ='" + ManageQuote(_DisbursementReportDTO.pfieldname) + "' end  group by applicanttype,t1.loantype,applicantname,approvedloanamount,t1.tenureofloan,loanstatus,salespersonname,t1.vchapplicationid;";
                   if (_DisbursementReportDTO.ptype.ToUpper() != "LOAN TYPE")
                       query = "select applicanttype,t1.loantype,applicantname,sum(t3.totaldisburseamount)totaldisburseamount,approvedloanamount,t1.tenureofloan,0 as paidampunt,0 as dueamount,0 as penaltyamount,0 as penaltypaid,0 as penaltydue, loanstatus,salespersonname,t1.vchapplicationid from tbltransapprovedapplications t1 join tabapplication t2 on t1.vchapplicationid=t2.vchapplicationid join tbltransloandisbursement t3 on t1.vchapplicationid=t3.vchapplicationid where t3.disbursementdate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' and   case when '" + ManageQuote(_DisbursementReportDTO.pfieldname) + "' ='ALL' then applicanttype <>'ALL' else applicanttype ='" + ManageQuote(_DisbursementReportDTO.pfieldname) + "' end  group by applicanttype,t1.loantype,applicantname,approvedloanamount,t1.tenureofloan,loanstatus,salespersonname,t1.vchapplicationid;";

                   using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                   {
                       while (dr.Read())
                       {
                           DisbursementReportDuesDetailsDTO _DisbursementReportDuesDetailsDTO = new DisbursementReportDuesDetailsDTO
                           {
                               papplicanttype = (dr["applicanttype"] == null) ? "" : Convert.ToString(dr["applicanttype"]),
                               ploantype = (dr["loantype"] == null) ? "" : Convert.ToString(dr["loantype"]),

                               pvchapplicationid = (dr["vchapplicationid"] == null) ? "" : Convert.ToString(dr["vchapplicationid"]),
                               papplicantname = (dr["applicantname"] == null) ? "" : Convert.ToString(dr["applicantname"]),
                               pdisbursedamount = (dr["totaldisburseamount"] == null) ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),
                               pinstallmentintrest = 0,
                               ploanamount = (dr["approvedloanamount"] == null) ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                               ptenureofloan = (dr["tenureofloan"] == null) ? 0 : Convert.ToInt32(dr["tenureofloan"]),
                               ppaidamount = (dr["paidampunt"] == null) ? 0 : Convert.ToDecimal(dr["paidampunt"]),
                               pdueamount = (dr["dueamount"] == null) ? 0 : Convert.ToDecimal(dr["dueamount"]),
                               ppenaltyamount = (dr["penaltyamount"] == null) ? 0 : Convert.ToDecimal(dr["penaltyamount"]),
                               ppenaltypaidamount = (dr["penaltypaid"] == null) ? 0 : Convert.ToDecimal(dr["penaltypaid"]),
                               ppenaltydueamount = (dr["penaltydue"] == null) ? 0 : Convert.ToDecimal(dr["penaltydue"]),
                               pstatus = (dr["loanstatus"] == null) ? "" : Convert.ToString(dr["loanstatus"]),
                               psalesexecutive = (dr["salespersonname"] == null) ? "" : Convert.ToString(dr["salespersonname"]),
                               pcollectionexecutive = "",
                           };
                           pdisbursementdueslist.Add(_DisbursementReportDuesDetailsDTO);

                       }
                   }

               }
               catch (Exception ex)
               {

                   throw ex;
               }
           });
            return pdisbursementdueslist;
        }

        public async Task<List<EmiChartViewDTO>> GetEmiChartViewData(string ConnectionString)
        {

            List<EmiChartViewDTO> EmiChartViewlist = new List<EmiChartViewDTO>();
            await Task.Run(() =>
            {
                string strQuery = "select  ta.applicantid,tap.vchapplicationid,applicantname,(approvedloanamount) as approvedloanamount,businessentitycontactno as contactnumber,businessentityemailid as emailid,sum(totaldisburseamount) as totaldisburseamount from tabapplication ta join tbltransapprovedapplications tap on ta.applicationid=tap.applicationid join tbltransloandisbursement td on td.vchapplicationid=tap.vchapplicationid join tblmstcontact tc on tc.contactid=ta.applicantid where  ta.vchapplicationid in (select vchapplicationid from tbltransloandisbursement)  group by ta.applicantid,approvedloanamount,tap.vchapplicationid,applicantname,contactnumber,emailid order by ta.applicantid desc";
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            EmiChartViewDTO _EmiChartViewDTO = new EmiChartViewDTO
                            {

                                pVchapplicationID = Convert.ToString(dr["vchapplicationid"]),
                                pApplicantname = Convert.ToString(dr["applicantname"]),
                                pApprovedloanamount = dr["approvedloanamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["approvedloanamount"]),
                                pApplicantEmail = Convert.ToString(dr["emailid"]),
                                pApplicantMobileNo = Convert.ToString(dr["contactnumber"]),
                                pDisbursalamount = dr["totaldisburseamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),

                            };
                            EmiChartViewlist.Add(_EmiChartViewDTO);
                        }
                    }


                }
                catch (Exception)
                {

                    throw;
                }

            });
            return EmiChartViewlist;
        }

        public async Task<DisbursementTrendDTO> GetDisbursementTrendReport(string monthname, string ConnectionString)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DisbursementTrendDTO _DisbursementTrendDTO = new DisbursementTrendDTO();
            _DisbursementTrendDTO.pmonthlist = new List<DisbursementTrendDeatilsDTO>();
            _DisbursementTrendDTO.pdisbursementtrendlist = new List<DisbursementTrendDeatilsDTO>();
            _DisbursementTrendDTO.pdisbursementtrenddailylist = new List<DisbursementTrendDeatilsDTO>();

            await Task.Run(() =>
                {
                    string strQuery = "select fn_Trendreport_Disbursement('" + monthname + "');select * from temptbltransmonthlist  order by ('01-'||particulars)::date; SELECT* from temptbltransdisbursed; SELECT * from temptbltransdisburseddaily; ";
                    try
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, strQuery);

                        dt = ds.Tables[1];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            _DisbursementTrendDTO.pmonthlist.Add(new DisbursementTrendDeatilsDTO
                            {
                                precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),
                                pparticulars = Convert.ToString(dt.Rows[i]["particulars"]),
                            });

                        }
                        dt = ds.Tables[2];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            _DisbursementTrendDTO.pdisbursementtrendlist.Add(new DisbursementTrendDeatilsDTO
                            {
                                precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),
                                pparticulars = Convert.ToString(dt.Rows[i]["particulars"]),

                                pcount1 = Convert.ToInt16(dt.Rows[i]["count1"]),
                                pamount1 = Convert.ToDecimal(dt.Rows[i]["amount1"]),

                                pcount2 = Convert.ToInt16(dt.Rows[i]["count2"]),
                                pamount2 = Convert.ToDecimal(dt.Rows[i]["amount2"]),

                                pcount3 = Convert.ToInt16(dt.Rows[i]["count3"]),
                                pamount3 = Convert.ToDecimal(dt.Rows[i]["amount3"]),

                                pcount4 = Convert.ToInt16(dt.Rows[i]["count4"]),
                                pamount4 = Convert.ToDecimal(dt.Rows[i]["amount4"]),

                                pcount5 = Convert.ToInt16(dt.Rows[i]["count5"]),
                                pamount5 = Convert.ToDecimal(dt.Rows[i]["amount5"]),

                                pcount6 = Convert.ToInt16(dt.Rows[i]["count6"]),
                                pamount6 = Convert.ToDecimal(dt.Rows[i]["amount6"]),

                                pcount7 = Convert.ToInt16(dt.Rows[i]["count7"]),
                                pamount7 = Convert.ToDecimal(dt.Rows[i]["amount7"]),

                                pcount8 = Convert.ToInt16(dt.Rows[i]["count8"]),
                                pamount8 = Convert.ToDecimal(dt.Rows[i]["amount8"]),

                                pcount9 = Convert.ToInt16(dt.Rows[i]["count9"]),
                                pamount9 = Convert.ToDecimal(dt.Rows[i]["amount9"]),

                                pcount10 = Convert.ToInt16(dt.Rows[i]["count10"]),
                                pamount10 = Convert.ToDecimal(dt.Rows[i]["amount10"]),

                                pcount11 = Convert.ToInt16(dt.Rows[i]["count11"]),
                                pamount11 = Convert.ToDecimal(dt.Rows[i]["amount11"]),

                                pcount12 = Convert.ToInt16(dt.Rows[i]["count12"]),
                                pamount12 = Convert.ToDecimal(dt.Rows[i]["amount12"])
                            });
                        }
                        dt = ds.Tables[3];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            _DisbursementTrendDTO.pdisbursementtrenddailylist.Add(new DisbursementTrendDeatilsDTO
                            {
                                precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),


                                pcount1 = Convert.ToInt16(dt.Rows[i]["count1"]),
                                pamount1 = Convert.ToDecimal(dt.Rows[i]["amount1"]),

                                pcount2 = Convert.ToInt16(dt.Rows[i]["count2"]),
                                pamount2 = Convert.ToDecimal(dt.Rows[i]["amount2"]),

                                pcount3 = Convert.ToInt16(dt.Rows[i]["count3"]),
                                pamount3 = Convert.ToDecimal(dt.Rows[i]["amount3"]),

                                pcount4 = Convert.ToInt16(dt.Rows[i]["count4"]),
                                pamount4 = Convert.ToDecimal(dt.Rows[i]["amount4"]),

                                pcount5 = Convert.ToInt16(dt.Rows[i]["count5"]),
                                pamount5 = Convert.ToDecimal(dt.Rows[i]["amount5"]),

                                pcount6 = Convert.ToInt16(dt.Rows[i]["count6"]),
                                pamount6 = Convert.ToDecimal(dt.Rows[i]["amount6"]),

                                pcount7 = Convert.ToInt16(dt.Rows[i]["count7"]),
                                pamount7 = Convert.ToDecimal(dt.Rows[i]["amount7"]),

                                pcount8 = Convert.ToInt16(dt.Rows[i]["count8"]),
                                pamount8 = Convert.ToDecimal(dt.Rows[i]["amount8"]),

                                pcount9 = Convert.ToInt16(dt.Rows[i]["count9"]),
                                pamount9 = Convert.ToDecimal(dt.Rows[i]["amount9"]),

                                pcount10 = Convert.ToInt16(dt.Rows[i]["count10"]),
                                pamount10 = Convert.ToDecimal(dt.Rows[i]["amount10"]),

                                pcount11 = Convert.ToInt16(dt.Rows[i]["count11"]),
                                pamount11 = Convert.ToDecimal(dt.Rows[i]["amount11"]),

                                pcount12 = Convert.ToInt16(dt.Rows[i]["count12"]),
                                pamount12 = Convert.ToDecimal(dt.Rows[i]["amount12"])
                            });
                        }


                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                });
            return _DisbursementTrendDTO;
        }

        public async Task<DisbursementTrendDTO> GetCollectionandDuesTrendReport(string monthname, string type, string ConnectionString)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DisbursementTrendDTO _DisbursementTrendDTO = new DisbursementTrendDTO();
            _DisbursementTrendDTO.pmonthlist = new List<DisbursementTrendDeatilsDTO>();
            _DisbursementTrendDTO.pdisbursementtrendlist = new List<DisbursementTrendDeatilsDTO>();
            _DisbursementTrendDTO.pdisbursementtrenddailylist = new List<DisbursementTrendDeatilsDTO>();

            await Task.Run(() =>
            {
                string strQuery = "select fn_Trendreport_collectionanddues('" + monthname + "','" + type + "');select * from temptbltransmonthlist  order by ('01-'||particulars)::date; select * from   temptbltranscollectionanddues order by recordid,sortorder; select * from   temptbltranscollectionandduesdetailed order by recordid,sortorder; ";
                try
                {
                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, strQuery);

                    dt = ds.Tables[1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _DisbursementTrendDTO.pmonthlist.Add(new DisbursementTrendDeatilsDTO
                        {
                            precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),
                            pparticulars = Convert.ToString(dt.Rows[i]["particulars"]),
                        });

                    }
                    dt = ds.Tables[2];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _DisbursementTrendDTO.pdisbursementtrendlist.Add(new DisbursementTrendDeatilsDTO
                        {
                            precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),
                            pparticulars = Convert.ToString(dt.Rows[i]["particulars"]),

                            pamount1 = Convert.ToDecimal(dt.Rows[i]["amount1"]),

                            pamount2 = Convert.ToDecimal(dt.Rows[i]["amount2"]),

                            pamount3 = Convert.ToDecimal(dt.Rows[i]["amount3"]),

                            pamount4 = Convert.ToDecimal(dt.Rows[i]["amount4"]),

                            pamount5 = Convert.ToDecimal(dt.Rows[i]["amount5"]),

                            pamount6 = Convert.ToDecimal(dt.Rows[i]["amount6"]),

                            pamount7 = Convert.ToDecimal(dt.Rows[i]["amount7"]),

                            pamount8 = Convert.ToDecimal(dt.Rows[i]["amount8"]),

                            pamount9 = Convert.ToDecimal(dt.Rows[i]["amount9"]),

                            pamount10 = Convert.ToDecimal(dt.Rows[i]["amount10"]),

                            pamount11 = Convert.ToDecimal(dt.Rows[i]["amount11"]),

                            pamount12 = Convert.ToDecimal(dt.Rows[i]["amount12"])
                        });
                    }
                    dt = ds.Tables[3];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _DisbursementTrendDTO.pdisbursementtrenddailylist.Add(new DisbursementTrendDeatilsDTO
                        {
                            precordid = Convert.ToInt16(dt.Rows[i]["recordid"]),
                            psortorder = Convert.ToInt16(dt.Rows[i]["sortorder"]),
                            ptype = Convert.ToString(dt.Rows[i]["type"]),

                            pparticulars = Convert.ToString(dt.Rows[i]["particulars"]),

                            pamount1 = Convert.ToDecimal(dt.Rows[i]["amount1"]),

                            pamount2 = Convert.ToDecimal(dt.Rows[i]["amount2"]),

                            pamount3 = Convert.ToDecimal(dt.Rows[i]["amount3"]),

                            pamount4 = Convert.ToDecimal(dt.Rows[i]["amount4"]),

                            pamount5 = Convert.ToDecimal(dt.Rows[i]["amount5"]),

                            pamount6 = Convert.ToDecimal(dt.Rows[i]["amount6"]),

                            pamount7 = Convert.ToDecimal(dt.Rows[i]["amount7"]),

                            pamount8 = Convert.ToDecimal(dt.Rows[i]["amount8"]),

                            pamount9 = Convert.ToDecimal(dt.Rows[i]["amount9"]),

                            pamount10 = Convert.ToDecimal(dt.Rows[i]["amount10"]),

                            pamount11 = Convert.ToDecimal(dt.Rows[i]["amount11"]),

                            pamount12 = Convert.ToDecimal(dt.Rows[i]["amount12"])
                        });
                    }


                }
                catch (Exception ex)
                {

                    throw;
                }

            });
            return _DisbursementTrendDTO;
        }
    }
}
