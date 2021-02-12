using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class IntrestPaymentDAL : SettingsDAL, IIntrestPayments
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        public bool SaveInterestPayment(IntrestPaymentDTO _IntrestPaymentDTO, string Connectionstring, out string _PaymentId)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {

                if (string.IsNullOrEmpty(_IntrestPaymentDTO.ppaymentid.ToString()))
                {

                    _IntrestPaymentDTO.ppaymentid = Convert.ToString(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "SELECT FN_GENERATENEXTID('PAYMENT VOUCHER','CASH','" + FormatDate(_IntrestPaymentDTO.pInterestPaymentDate) + "')"));
                }
                long creditaccountid = 0;
                string query = "";
                long detailsid = 0;


                creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select bankaccountid from tblmstbank  where recordid = " + _IntrestPaymentDTO.pbankid));


                query = "insert into tbltranspaymentvoucher( paymentid, paymentdate, modeofpayment, totalpaidamount, narration, creditaccountid, statusid, createdby, createddate)values('" + _IntrestPaymentDTO.ppaymentid + "', '" + FormatDate(_IntrestPaymentDTO.pInterestPaymentDate) + "', '" + ManageQuote(_IntrestPaymentDTO.ptranstype) + "', " + _IntrestPaymentDTO.pTotalpaymentamount + ", '" + ManageQuote(_IntrestPaymentDTO.pNarration) + "', " + creditaccountid + ", " + Convert.ToInt32(Status.Active) + "," + _IntrestPaymentDTO.pCreatedby + ", current_timestamp) returning recordid";
                detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, query));


                if (_IntrestPaymentDTO.pintrestpaymentlist != null)
                {
                    for (int i = 0; i < _IntrestPaymentDTO.pintrestpaymentlist.Count; i++)
                    {
                        sbQuery.Append("insert into tbltranspaymentvoucherdetails( detailsid, paymentid, contactname, ledgeramount,debitaccountid )values (" + detailsid + ", '" + _IntrestPaymentDTO.ppaymentid + "','" + _IntrestPaymentDTO.pintrestpaymentlist[i].pMembername + "', '" + _IntrestPaymentDTO.pintrestpaymentlist[i].ptotalamount + "'," + _IntrestPaymentDTO.pintrestpaymentlist[i].pdebitaccountid + ");");

                        sbQuery.Append("update interest_payments set voucher_id=(select recordid from tbltranspaymentvoucher where paymentid='" + _IntrestPaymentDTO.ppaymentid + "') where  interest_payment_id=" + _IntrestPaymentDTO.pintrestpaymentlist[i].pInterestpaymentid + ";");
                    }
                }

                if (_IntrestPaymentDTO.ptranstype != "CASH")
                {
                    string particulars = "";

                    if (string.IsNullOrEmpty(_IntrestPaymentDTO.ptypeofpayment))
                    {
                        _IntrestPaymentDTO.ptypeofpayment = _IntrestPaymentDTO.ptypeofpayment;
                    }
                    if (_IntrestPaymentDTO.pbankname.Contains('-'))
                    {

                        _IntrestPaymentDTO.pbankname = _IntrestPaymentDTO.pbankname.Split('-')[0].Trim();
                    }


                    if (_IntrestPaymentDTO.ptranstype == "CHEQUE")
                    {
                        sbQuery.Append("update  tblmstcheques set   statusid =(SELECT  statusid from tblmststatus  where upper(statusname)  ='USED-CHEQUES') where bankid =" + _IntrestPaymentDTO.pbankid + " and chequenumber=" + _IntrestPaymentDTO.pchequeno + ";");

                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _IntrestPaymentDTO.ppaymentid + "', '" + _IntrestPaymentDTO.pbankname + "', '" + _IntrestPaymentDTO.pbranchname + "', '" + _IntrestPaymentDTO.ptranstype + "', '" + _IntrestPaymentDTO.ptypeofpayment + "', " + _IntrestPaymentDTO.pbankid + ", '" + _IntrestPaymentDTO.pchequeno + "', '" + _IntrestPaymentDTO.pdebitcard + "', '" + _IntrestPaymentDTO.pUpiid + "', '" + _IntrestPaymentDTO.pUpiname + "', '" + FormatDate(_IntrestPaymentDTO.pInterestPaymentDate) + "', " + _IntrestPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _IntrestPaymentDTO.pCreatedby + ", current_timestamp);");


                    }

                    else if (_IntrestPaymentDTO.ptranstype == "ONLINE")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _IntrestPaymentDTO.ppaymentid + "', '" + _IntrestPaymentDTO.pbankname + "', '" + _IntrestPaymentDTO.pbranchname + "', '" + _IntrestPaymentDTO.ptranstype + "', '" + _IntrestPaymentDTO.ptypeofpayment + "', " + _IntrestPaymentDTO.pbankid + ", '" + _IntrestPaymentDTO.preferencenoonline + "', '" + _IntrestPaymentDTO.pdebitcard + "', '" + _IntrestPaymentDTO.pUpiid + "', '" + _IntrestPaymentDTO.pUpiname + "', '" + FormatDate(_IntrestPaymentDTO.pInterestPaymentDate) + "', " + _IntrestPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _IntrestPaymentDTO.pCreatedby + ", current_timestamp);");
                    }
                    else if (_IntrestPaymentDTO.ptranstype == "DEBIT CARD")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _IntrestPaymentDTO.ppaymentid + "', '" + _IntrestPaymentDTO.pbankname + "', '" + _IntrestPaymentDTO.pbranchname + "', '" + _IntrestPaymentDTO.ptranstype + "', '" + _IntrestPaymentDTO.ptypeofpayment + "', " + _IntrestPaymentDTO.pbankid + ", '" + _IntrestPaymentDTO.preferencenodcard + "', '" + _IntrestPaymentDTO.pdebitcard + "', '" + _IntrestPaymentDTO.pUpiid + "', '" + _IntrestPaymentDTO.pUpiname + "', '" + FormatDate(_IntrestPaymentDTO.pInterestPaymentDate) + "', " + _IntrestPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _IntrestPaymentDTO.pCreatedby + ", current_timestamp);");

                    }
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {


                    sbQuery.Append("select fntotaltransactions('" + _IntrestPaymentDTO.ppaymentid + "','PAYMENT VOUCHER');");
                    NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, sbQuery.ToString());
                    IsSaved = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            _PaymentId = _IntrestPaymentDTO.ppaymentid;
            return IsSaved;

        }
        public List<IntrestPaymentDTO> GetSchemename(string Connectionstring)
        {
            List<IntrestPaymentDTO> lstSchemeDetails = new List<IntrestPaymentDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select fdconfigid,fdname from tblmstfixeddepositconfig order by fdname;"))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDTO objSchemedetails = new IntrestPaymentDTO();
                        objSchemedetails.pSchemeId = dr["fdconfigid"];
                        objSchemedetails.pSchemename = dr["fdname"];
                        lstSchemeDetails.Add(objSchemedetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstSchemeDetails;
        }
        public List<IntrestPaymentDTO> GetCompany(string Connectionstring)
        {
            List<IntrestPaymentDTO> lstCompanyDetails = new List<IntrestPaymentDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select DISTINCT companyname from tabbranchcodes tb join self_or_adjustment sa on tb.companyname=sa.company_name   order by companyname;"))
  //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select DISTINCT company_name from self_or_adjustment where company_name!=''  order by company_name;"))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDTO objCompanydetails = new IntrestPaymentDTO();
                        objCompanydetails.pCompanyname = dr["companyname"];
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

        public List<IntrestPaymentDTO> GetBranchName(string companyname, string Connectionstring)
        {
            List<IntrestPaymentDTO> lstBranchDetails = new List<IntrestPaymentDTO>();
            try
            {
                 using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct code,branchname from tabbranchcodes tb join self_or_adjustment sa on tb.branchname=sa.branch_name  where companyname='" + ManageQuote(companyname) + "'  order by branchname;"))
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct '0001' as code,branch_name from self_or_adjustment  where company_name='"+ companyname + "' and branch_name!='' order by branch_name;"))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDTO objBranchdetails = new IntrestPaymentDTO();
                        objBranchdetails.pCode = dr["code"];
                        objBranchdetails.pBranchname = dr["branchname"];
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
        public bool RunInterestPaymentFunction(string Connectionstring)
        {
            bool IsFunctionRun = false;
            try
            {

                //  NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "SELECT fn_Interest_payment_jv();");
                IsFunctionRun = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return IsFunctionRun;

        }

        public List<IntrestPaymentDetailsDTO> GetMemberPaymenthistory(long schemeid, string paymenttype, string companyname, string branchname, string forthemonth, string Connectionstring)
        {

            List<IntrestPaymentDetailsDTO> lstmemberDetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                string Query = string.Empty;
                //NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "SELECT fn_Interest_payment_jv();");
                if (paymenttype.ToUpper() == "SELF")
                {
                    Query = "select  distinct ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,ip.interst_payable as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,ip.interst_payable,tf.depositdate,tf.maturitydate from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid=sa.fd_account_id join (select * from Interest_payments where voucher_id is  null or voucher_id=0) ip on tf.fdaccountid= ip.trans_type_id where tf.fdconfigid=" + schemeid + " and upper(sa.payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month)=upper('" + forthemonth + "') order by tf.membername,tf.fdaccountno;";
                }
                else if (paymenttype.ToUpper() == "ADJUSTMENT")
                {
                    Query = "select distinct ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,ip.interst_payable as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,ip.interst_payable,tf.depositdate,tf.maturitydate from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join(select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid =" + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)= upper('" + forthemonth + "') and upper(sa.company_name) =upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by tf.membername,tf.fdaccountno;";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDetailsDTO objMembershowdetails = new IntrestPaymentDetailsDTO();
                        objMembershowdetails.pSchemename = dr["fdname"];
                        objMembershowdetails.pdebitaccountid = dr["account_id"];
                        objMembershowdetails.pInterestpaymentid = dr["interest_payment_id"];
                        objMembershowdetails.pFdaccountno = dr["fdaccountno"];
                        objMembershowdetails.pMembername = dr["membername"];
                        objMembershowdetails.pIntrestamount = dr["interst_payable"];
                        objMembershowdetails.pTdsamount = dr["tds_amount"];
                        objMembershowdetails.ptotalamount = dr["total_amount"];
                        objMembershowdetails.pbankname = dr["bank_name"];
                        objMembershowdetails.pbankbranch = dr["bank_branch"];
                        objMembershowdetails.pdepositeamount = dr["depositamount"];
                        objMembershowdetails.pinterestrate = dr["interestrate"];
                        objMembershowdetails.pmaturiryamount = dr["maturityamount"];
                        objMembershowdetails.pinterestpayable = dr["interest_amount"];
                        objMembershowdetails.pdepositedate = dr["depositdate"];
                        objMembershowdetails.pmaturitydate = dr["maturitydate"];

                        lstmemberDetails.Add(objMembershowdetails);
                    }

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstmemberDetails;
        }


        public List<IntrestPaymentDetailsDTO> GetShowInterestPaymentReport(long schemeid, string fdaccountno, string paymenttype, string companyname, string branchname, string pdatecheked, string frommonthof, string tomonthof, string type, string Connectionstring)
        {

            List<IntrestPaymentDetailsDTO> lstmemberDetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                string Query = string.Empty;

                if (schemeid == 0)
                {
                    if (paymenttype.ToUpper() == "SELF")
                    {
                        if (type.ToUpper() == "ALL")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }

                        }
                        else if (type.ToUpper() == "PAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }
                        else if (type.ToUpper() == "NONPAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }



                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }



                    }
                    else if (paymenttype.ToUpper() == "ADJUSTMENT")
                    {
                        if (type.ToUpper() == "ALL")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }

                        }
                        else if (type.ToUpper() == "PAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "')  and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }
                        else if (type.ToUpper() == "NONPAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname ;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }

                    }
                }
                else
                {
                    if (paymenttype.ToUpper() == "SELF")
                    {
                        if (type.ToUpper() == "ALL")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }

                        }
                        else if (type.ToUpper() == "PAYMENT")
                        {

                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month)<=upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid=" + schemeid + " and upper(payment_type)=upper('" + ManageQuote(paymenttype) + "') and upper(interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }
                        else if (type.ToUpper() == "NONPAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select *from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid =" + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select *from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid =" + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select *from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid =" + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select *from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid =" + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "') and upper(ip.interest_paid_month) between upper('" + frommonthof + "') and upper('" + tomonthof + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }



                    }
                    else if (paymenttype.ToUpper() == "ADJUSTMENT")
                    {
                        if (type.ToUpper() == "ALL")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }

                        }
                        else if (type.ToUpper() == "PAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname ;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month)<= upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select 'Payments' as paymentstatus,fdconfigid,account_id,interest_payment_id,fdaccountno,fdname,membername,payment_type,interest_amount,tds_amount,round(interest_amount-coalesce(tds_amount,0)) as total_amount,interest_paid_month,bank_name,bank_branch,depositamount,interestrate,maturityamount,interestpayable,TO_CHAR(depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(maturitydate,'dd/Mon/yyyy') as maturitydate,company_name,branch_name,paymentid,TO_CHAR(paymentdate,'dd/Mon/yyyy') as paymentdate,modeofpayment,paybank,paybankbranch,chequenumber from vwInterest_Payment_Report_Paid where fdconfigid =" + schemeid + " and upper(payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(interest_paid_month) between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(company_name) =upper('" + ManageQuote(companyname) + "') and upper(branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }
                        else if (type.ToUpper() == "NONPAYMENT")
                        {
                            if (pdatecheked.ToUpper() == "ASON")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid = " + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid = " + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)<= upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }

                            }
                            else if (pdatecheked.ToUpper() == "BETWEEN")
                            {
                                if (fdaccountno.ToUpper() == "ALL")
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid = " + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') order by fdname;";

                                }
                                else
                                {
                                    Query = "select distinct 'Non Payments' as paymentstatus,ip.account_id,ip.interest_payment_id,tf.fdaccountno,tf.fdname,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,round(ip.interest_amount - coalesce(ip.tds_amount, 0)) as total_amount,ip.interest_paid_month,sa.bank_name,sa.bank_branch,tf.depositamount,tf.interestrate,tf.maturityamount,tf.interestpayable,TO_CHAR(tf.depositdate,'dd/Mon/yyyy') as depositdate ,TO_CHAR(tf.maturitydate,'dd/Mon/yyyy') as maturitydate,null as paymentid,null as paymentdate,null as modeofpayment,null as paybank,null as paybankbranch,null as chequenumber from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join (select * from Interest_payments where voucher_id is null or voucher_id = 0) ip on tf.fdaccountid = ip.trans_type_id where tf.fdconfigid = " + schemeid + " and upper(sa.payment_type)= upper('" + ManageQuote(paymenttype) + "')  and upper(ip.interest_paid_month)between  upper('" + frommonthof + "') and upper('" + tomonthof + "') and upper(sa.company_name) = upper('" + ManageQuote(companyname) + "') and upper(sa.branch_name) = upper('" + ManageQuote(branchname) + "') and fdaccountno='" + ManageQuote(fdaccountno) + "' order by fdname;";

                                }


                            }
                        }

                    }
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDetailsDTO objMembershowdetails = new IntrestPaymentDetailsDTO();
                        objMembershowdetails.pPaymentstatus = dr["paymentstatus"];
                        objMembershowdetails.pSchemename = dr["fdname"];
                        objMembershowdetails.pdebitaccountid = dr["account_id"];
                        objMembershowdetails.pInterestpaymentid = dr["interest_payment_id"];
                        objMembershowdetails.pFdaccountno = dr["fdaccountno"];
                        objMembershowdetails.pMembername = dr["membername"];
                        objMembershowdetails.pIntrestamount = dr["interest_amount"];
                        objMembershowdetails.pTdsamount = dr["tds_amount"];
                        objMembershowdetails.ptotalamount = dr["total_amount"];

                        objMembershowdetails.pbankname = dr["bank_name"];
                        objMembershowdetails.pbankbranch = dr["bank_branch"];
                        objMembershowdetails.pdepositeamount = dr["depositamount"];
                        objMembershowdetails.pinterestrate = dr["interestrate"];
                        objMembershowdetails.pmaturiryamount = dr["maturityamount"];
                        objMembershowdetails.pinterestpayable = dr["interestpayable"];
                        objMembershowdetails.pdepositedate = dr["depositdate"];
                        objMembershowdetails.pmaturitydate = dr["maturitydate"];

                        objMembershowdetails.pvoucherno = dr["paymentid"];
                        objMembershowdetails.ppaymentdate = dr["paymentdate"];
                        objMembershowdetails.pmodeofpayment = dr["modeofpayment"];
                        objMembershowdetails.ppaybankname = dr["paybank"];
                        objMembershowdetails.ppaybankbranchname = dr["paybankbranch"];
                        objMembershowdetails.ppaychequeno = dr["chequenumber"];


                        lstmemberDetails.Add(objMembershowdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstmemberDetails;
        }


        public List<IntrestPaymentDetailsDTO> GetShowInterestpaymentdetailsforview(string Connectionstring)
        {

            List<IntrestPaymentDetailsDTO> lstmemberDetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                string Query = string.Empty;

                //Query = "select distinct tf.fdaccountno,tf.membername,sa.payment_type,ip.interest_amount,ip.tds_amount,(ip.interest_amount-ip.tds_amount) as total_amount,ip.interest_paid_month from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid=sa.fd_account_id join (select * from Interest_payments where (voucher_id is not null or voucher_id<>0) and voucher_id in (select recordid from tbltranspaymentvoucher where paymentdate=current_date) ) ip on tf.fdaccountid= ip.trans_type_id;";


                Query = @"select X.fdaccountno,X.membername,X.payment_type,X.interest_amount,X.tds_amount,X.total_amount,X.interest_paid_month,X.depositamount,X.interestrate,Y.paymentid,Y.paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber from
(select distinct tf.fdaccountno, tf.membername, sa.payment_type, ip.interest_amount, ip.tds_amount, (ip.interest_amount-ip.tds_amount) as total_amount,ip.interest_paid_month,tf.depositamount,tf.interestrate,ip.voucher_id from tbltransfdcreation tf join self_or_adjustment sa on tf.fdaccountid = sa.fd_account_id join(select * from Interest_payments where (voucher_id is not null or voucher_id <> 0) and voucher_id in (select recordid from tbltranspaymentvoucher where paymentdate = current_date) ) ip on tf.fdaccountid = ip.trans_type_id)X left join(select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment, A.bankname as paybank, A.branchname as paybankbranch, A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        IntrestPaymentDetailsDTO objMembershowdetails = new IntrestPaymentDetailsDTO();
                        objMembershowdetails.pFdaccountno = dr["fdaccountno"];
                        objMembershowdetails.pPaymenttype = dr["payment_type"];
                        objMembershowdetails.pMembername = dr["membername"];
                        objMembershowdetails.pIntrestamount = dr["interest_amount"];
                        objMembershowdetails.pTdsamount = dr["tds_amount"];
                        objMembershowdetails.ptotalamount = dr["total_amount"];

                        objMembershowdetails.pInterestpaidmonth = dr["interest_paid_month"];
                        objMembershowdetails.pdepositeamount = dr["depositamount"];
                        objMembershowdetails.pinterestrate = dr["interestrate"];
                        objMembershowdetails.pvoucherno = dr["paymentid"];
                        objMembershowdetails.ppaymentdate = dr["paymentdate"];
                        objMembershowdetails.pmodeofpayment = dr["modeofpayment"];
                        objMembershowdetails.ppaybankname = dr["paybank"];
                        objMembershowdetails.ppaybankbranchname = dr["paybankbranch"];
                        objMembershowdetails.ppaychequeno = dr["chequenumber"];

                        lstmemberDetails.Add(objMembershowdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstmemberDetails;
        }






    }
}
