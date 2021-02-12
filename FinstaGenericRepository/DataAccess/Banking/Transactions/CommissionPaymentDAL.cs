using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;


namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class CommissionPaymentDAL : SettingsDAL, ICommissionPayment
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        public List<CommissionPaymentDTO> GetAgentDetails(string Connectionstring)
        {
            List<CommissionPaymentDTO> lstAgentDetails = new List<CommissionPaymentDTO>();
            try
            {
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select name as agentname,contactreferenceid,businessentitycontactno as mobileno from tblmstcontact where contactid in (select distinct agent_id from commission_payment) and  contactid in (select contactid from tblmstreferral)order by name; "))

                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactid as agentid,concat(name||' '||surname,'-',contactid) as agentname from tblmstreferral where contactid in (select distinct agent_id from commission_payment)  "))

                    // For Table name change in Contact Form tbl_mst_referal purpose change the query
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select tbl_mst_referral_id as agentid,concat(name||' '||surname,'-',tbl_mst_referral_id) as agentname,(coalesce(name,'')||' '||coalesce(surname,'')||' - ' || referral_code) as referralname,referral_code from tblmstcontact A join tbl_mst_referral B on A.contactid=B.contact_id where tbl_mst_referral_id in (select distinct agent_id from commission_payment)"))
                {
                    while (dr.Read())
                    {
                        CommissionPaymentDTO objAgentdetails = new CommissionPaymentDTO();
                        objAgentdetails.pagentid = dr["agentid"];
                        objAgentdetails.pagentname = dr["referralname"].ToString();

                        lstAgentDetails.Add(objAgentdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAgentDetails;
        }
        public bool SaveCommisionPayment(CommissionPaymentDTO _CommissionPaymentDTO, string Connectionstring, out string _PaymentId)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {

                //if (string.IsNullOrEmpty(_CommissionPaymentDTO.ppaymentid.ToString()))
                //{

                _CommissionPaymentDTO.ppaymentid = Convert.ToString(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "SELECT FN_GENERATENEXTID('PAYMENT VOUCHER','CASH','" + FormatDate(_CommissionPaymentDTO.pCommissionpaymentDate) + "')"));
                //}
                long creditaccountid = 0;
                string query = "";
                long detailsid = 0;


                creditaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select bankaccountid from tblmstbank  where recordid = " + _CommissionPaymentDTO.pbankid));


                query = "insert into tbltranspaymentvoucher( paymentid, paymentdate, modeofpayment, totalpaidamount, narration, creditaccountid, statusid, createdby, createddate)values('" + _CommissionPaymentDTO.ppaymentid + "', '" + FormatDate(_CommissionPaymentDTO.pCommissionpaymentDate) + "', '" + ManageQuote(_CommissionPaymentDTO.ptranstype) + "', " + _CommissionPaymentDTO.pTotalpaymentamount + ", '" + ManageQuote(_CommissionPaymentDTO.pNarration) + "', " + creditaccountid + ", " + Convert.ToInt32(Status.Active) + "," + _CommissionPaymentDTO.pCreatedby + ", current_timestamp) returning recordid";
                detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, query));


                if (_CommissionPaymentDTO.pcommissionpaymentlist != null)
                {
                    for (int i = 0; i < _CommissionPaymentDTO.pcommissionpaymentlist.Count; i++)
                    {
                        sbQuery.Append("insert into tbltranspaymentvoucherdetails( detailsid, paymentid, contactid,contactname, ledgeramount,debitaccountid)values (" + detailsid + ", '" + _CommissionPaymentDTO.ppaymentid + "'," + _CommissionPaymentDTO.pagentid + ",(select name||' '||surname as name from tbl_mst_referral tr join tblmstcontact tc on tr.contact_id=tc.contactid where tbl_mst_referral_id=" + _CommissionPaymentDTO.pagentid +"), '" + _CommissionPaymentDTO.pcommissionpaymentlist[i].ptotalamount + "'," + _CommissionPaymentDTO.pcommissionpaymentlist[i].pdebitaccountid + ");");

                        sbQuery.Append("update commission_payment set voucher_id=(select recordid from tbltranspaymentvoucher where paymentid='" + _CommissionPaymentDTO.ppaymentid + "') where  commission_payment_id=" + _CommissionPaymentDTO.pcommissionpaymentlist[i].pcommissionaymentid + ";");
                    }
                }

                if (_CommissionPaymentDTO.ptranstype != "CASH")
                {
                    string particulars = "";

                    if (string.IsNullOrEmpty(_CommissionPaymentDTO.ptypeofpayment))
                    {
                        _CommissionPaymentDTO.ptypeofpayment = _CommissionPaymentDTO.ptypeofpayment;
                    }
                    if (_CommissionPaymentDTO.pbankname.Contains('-'))
                    {

                        _CommissionPaymentDTO.pbankname = _CommissionPaymentDTO.pbankname.Split('-')[0].Trim();
                    }
                   

                    if (_CommissionPaymentDTO.ptranstype == "CHEQUE")
                    {
                        sbQuery.Append("update  tblmstcheques set   statusid =(SELECT  statusid from tblmststatus  where upper(statusname)  ='USED-CHEQUES') where bankid =" + _CommissionPaymentDTO.pbankid + " and chequenumber=" + _CommissionPaymentDTO.pchequeno + ";");

                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _CommissionPaymentDTO.ppaymentid + "', '" + _CommissionPaymentDTO.pbankname + "', '" + _CommissionPaymentDTO.pbranchname + "', '" + _CommissionPaymentDTO.ptranstype + "', '" + _CommissionPaymentDTO.ptranstype + "', " + _CommissionPaymentDTO.pbankid + ", '" + _CommissionPaymentDTO.pchequeno + "', '" + _CommissionPaymentDTO.pdebitcard + "', '" + _CommissionPaymentDTO.pUpiid + "', '" + _CommissionPaymentDTO.pUpiname + "', '" + FormatDate(_CommissionPaymentDTO.pCommissionpaymentDate) + "', " + _CommissionPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _CommissionPaymentDTO.pCreatedby + ", current_timestamp);");
                    }
                    else if (_CommissionPaymentDTO.ptranstype == "ONLINE")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _CommissionPaymentDTO.ppaymentid + "', '" + _CommissionPaymentDTO.pBankname + "', '" + _CommissionPaymentDTO.pBankbranchname + "', '" + _CommissionPaymentDTO.ptranstype + "', '" + _CommissionPaymentDTO.ptypeofpayment + "', " + _CommissionPaymentDTO.pbankid + ", '" + _CommissionPaymentDTO.preferencenoonline + "', '" + _CommissionPaymentDTO.pdebitcard + "', '" + _CommissionPaymentDTO.pUpiid + "', '" + _CommissionPaymentDTO.pUpiname + "', '" + FormatDate(_CommissionPaymentDTO.pCommissionpaymentDate) + "', " + _CommissionPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _CommissionPaymentDTO.pCreatedby + ", current_timestamp);");
                    }
                    else if (_CommissionPaymentDTO.ptranstype == "DEBIT CARD")
                    {
                        sbQuery.Append("insert into tbltranspaymentreference(  paymentid, bankname, branchname, transtype, typeofpayment, bankid, chequenumber,cardnumber , upiid  , upiname, paymentdate, paidamount, clearstatus, particulars, statusid, createdby, createddate   ) values('" + _CommissionPaymentDTO.ppaymentid + "', '" + _CommissionPaymentDTO.pBankname + "', '" + _CommissionPaymentDTO.pBankbranchname + "', '" + _CommissionPaymentDTO.ptranstype + "', '" + _CommissionPaymentDTO.ptranstype + "', " + _CommissionPaymentDTO.pbankid + ", '" + _CommissionPaymentDTO.preferencenodcard + "', '" + _CommissionPaymentDTO.pdebitcard + "', '" + _CommissionPaymentDTO.pUpiid + "', '" + _CommissionPaymentDTO.pUpiname + "', '" + FormatDate(_CommissionPaymentDTO.pCommissionpaymentDate) + "', " + _CommissionPaymentDTO.pTotalpaymentamount + ", 'N', '" + particulars + "',  " + Convert.ToInt32(Status.Active) + "," + _CommissionPaymentDTO.pCreatedby + ", current_timestamp);");

                    }
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {


                    sbQuery.Append("select fntotaltransactions('" + _CommissionPaymentDTO.ppaymentid + "','PAYMENT VOUCHER');select accountsupdate();");
                    NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, sbQuery.ToString());
                    IsSaved = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            _PaymentId = _CommissionPaymentDTO.ppaymentid;
            return IsSaved;

        }
        public List<CommissionPaymentAgentViewDTO> GetAgentContactDetails(long agentid, string Connectionstring)
        {
            List<CommissionPaymentAgentViewDTO> lstAgentDetails = new List<CommissionPaymentAgentViewDTO>();
            try
            {
                //                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, @"select C.name,A.fdaccountno,commsssionvalue,A.transdate,membername,tenortype,tenor,depositamount,interesttype,interestrate,maturityamount,interestpayable,depositdate,maturitydate from tbltransfdcreation A join tbltransfdreferraldetails B on A.fdaccountno=B.fdaccountno
                //left join tblmstcontact C on A.contactid=C.contactid where A.contactid = " + agentid + ";"))


                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, @"select c.name || ' ' || coalesce(c.surname,'') as name,A.fdaccountno,commsssionvalue,A.transdate,membername,tenortype,tenor||' '||tenortype as tenor,depositamount,interesttype,interestrate,maturityamount,interestpayable,depositdate,maturitydate from tbltransfdcreation A join tbltransfdreferraldetails B on A.fdaccountno = B.fdaccountno join tblmstcontact c on B.contactid = c.contactid and referralid ='" + agentid + "';"))
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, @"select c.name || ' ' || coalesce(c.surname,'') as name,A.fdaccountno,coalesce(commsssionvalue,0)commsssionvalue,A.transdate,membername,tenortype,tenor||' '||tenortype as tenor,depositamount,interesttype,interestrate,maturityamount,interestpayable,depositdate,maturitydate from tbltransfdcreation A join tbltransfdreferraldetails B on A.fdaccountno = B.fdaccountno join tbl_mst_referral tr on tr.tbl_mst_referral_id =b.referralid::bigint join tblmstcontact c on tr.contact_id = c.contactid and referralid ='" + agentid + "';"))
                {
                    while (dr.Read())
                    {
                        CommissionPaymentAgentViewDTO objAgentdetails = new CommissionPaymentAgentViewDTO();
                        objAgentdetails.pagentname = dr["name"];
                        objAgentdetails.pmembername = dr["membername"];
                        objAgentdetails.pfdaccountno = dr["fdaccountno"];
                        objAgentdetails.pcommsssionvalue =dr["commsssionvalue"];
                        objAgentdetails.ptransdate = dr["transdate"];
                        //objAgentdetails.pfdname = dr["fdname"];
                        //objAgentdetails.ptenortype = dr["tenortype"];
                        objAgentdetails.ptenor = dr["tenor"];
                        objAgentdetails.pdepositamount = dr["depositamount"];
                        objAgentdetails.pinteresttype = dr["interesttype"];
                        objAgentdetails.pinterestrate = dr["interestrate"];
                        objAgentdetails.pmaturityamount = dr["maturityamount"];
                        objAgentdetails.pinterestpayable = dr["interestpayable"];
                        objAgentdetails.pdepositdate = dr["depositdate"];
                        objAgentdetails.pmaturitydate = dr["maturitydate"];
                        lstAgentDetails.Add(objAgentdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAgentDetails;
        }
        public List<AgentBankDetailsDTO> GetAgentBankDetails(long agentid, string Connectionstring)
        {
            List<AgentBankDetailsDTO> lstAgentBankDetails = new List<AgentBankDetailsDTO>();
            try
            {
                
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, @"select tb.bankaccountname,tb.bankname,tb.bankaccountno,tb.bankifsccode,tb.bankbranch from tblmstreferral tr join tblmstreferralbankdetails tb on tr.referralid = tb.referralid where contactid = "+ agentid + " and tb.statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        AgentBankDetailsDTO objAgentBankdetails = new AgentBankDetailsDTO();
                        objAgentBankdetails.pAgentName = dr["bankaccountname"];
                        objAgentBankdetails.pBankName = dr["bankname"];
                        objAgentBankdetails.pBankAccountNo = dr["bankaccountno"];
                        objAgentBankdetails.pBankIfsccode = dr["bankifsccode"];
                        objAgentBankdetails.pBankBranch = dr["bankbranch"];
                        lstAgentBankDetails.Add(objAgentBankdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAgentBankDetails;
        }
        public List<CommissionPaymentDetailsDTO> ShowPromoterSalaryDetails(long agentid, string asondate, string Connectionstring)
        {
            List<CommissionPaymentDetailsDTO> lstPromoterDetails = new List<CommissionPaymentDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select trans_type,account_id,membername,fdaccountno,commission_payment_id,depositamount,interestrate,maturityamount,commssion_amount,tds_amount,round((commssion_amount-coalesce(tds_amount,0))) as totalamount from commission_payment cp  join tbltransfdcreation fd on fd.fdaccountid=cp.trans_type_id where agent_id=" + agentid + "  and commission_trans_date <='" + (asondate) + "' and (voucher_id is null or voucher_id=0) union all select trans_type,account_id,membername,rdaccountno,commission_payment_id,depositamount,interestrate,maturityamount,commssion_amount,tds_amount,round((commssion_amount-coalesce(tds_amount,0))) as totalamount from commission_payment cp  join tbltransrdcreation fd on fd.rdaccountid=cp.trans_type_id where agent_id=" + agentid + "  and commission_trans_date <= '" + (asondate) + "' and (voucher_id is null or voucher_id=0) ;"))


                {
                    while (dr.Read())
                    {
                        CommissionPaymentDetailsDTO objPromoterdetails = new CommissionPaymentDetailsDTO();
                        objPromoterdetails.pdebitaccountid = dr["account_id"];
                        objPromoterdetails.pMembername = dr["membername"].ToString();
                        objPromoterdetails.pSchemeAccountno = dr["fdaccountno"].ToString();
                        objPromoterdetails.pcommissionaymentid = Convert.ToInt64(dr["commission_payment_id"]);
                        objPromoterdetails.pDepositamount = dr["depositamount"];
                        objPromoterdetails.pInterestrate = dr["interestrate"];
                        objPromoterdetails.pMaturityamount = dr["maturityamount"];
                        objPromoterdetails.pCommissionamount = dr["commssion_amount"];
                        objPromoterdetails.pTdsamount = dr["tds_amount"];
                        objPromoterdetails.ptotalamount = dr["totalamount"];
                        objPromoterdetails.pbankingtranstype = dr["trans_type"];
                        lstPromoterDetails.Add(objPromoterdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPromoterDetails;
        }

        public List<CommissionPaymentDetailsDTO> ShowPromoterSalaryReport(long agentid, string frommonthof, string tomonthof, string type, string pdatecheked, string Connectionstring)
        {

            List<CommissionPaymentDetailsDTO> lstPromoterDetails = new List<CommissionPaymentDetailsDTO>();
            try
            {
                string Query = string.Empty;
                if (agentid == 0)
                {
                    if (type.ToUpper() == "ALL")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,
to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
         join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where  commission_trans_date<='" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left      join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }

                    }
                    else if (type.ToUpper() == "PAYMENT")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                                          join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                       join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0))X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                    }
                    else if (type.ToUpper() == "NONPAYMENT")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select 'Non Payments' as paymentstatus,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                        join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select 'Non Payments' as paymentstatus,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
         join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                        }
                    }
                }
                else
                {

                    if (type.ToUpper() == "ALL")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                           join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                               join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where  agent_id='" + agentid + "' and commission_trans_date<='" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                                          join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                    join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where  agent_id='" + agentid + "' and commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }

                    }
                    else if (type.ToUpper() == "PAYMENT")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                                                                                                                                                     join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where agent_id ='" + agentid + "' and  commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
(select voucher_id,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                                                                                                                                                     join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where agent_id=" + agentid + " and  commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                        }
                    }
                    else if (type.ToUpper() == "NONPAYMENT")
                    {
                        if (pdatecheked.ToUpper() == "ASON")
                        {
                            Query = @"select 'Non Payments' as paymentstatus,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left
                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                                                                                                                                                     join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where agent_id=" + agentid + " and  commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                        }
                        else if (pdatecheked.ToUpper() == "BETWEEN")
                        {
                            Query = @"select 'Non Payments' as paymentstatus,name||' '||surname as agentname,
fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left
                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                                                                                                                                                                     join tbl_mst_referral c on c.tbl_mst_referral_id = cp.agent_id  join tblmstcontact tc on tc.contactid=c.contact_id where agent_id=" + agentid + " and  commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                        }
                    }
                }

                //                if (agentid == 0)
                //                {
                //                    if (type.ToUpper() == "ALL")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,
                //to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //         join tblmstreferral c on c.contactid = cp.agent_id where  commission_trans_date<='" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left      join tblmstreferral c on c.contactid = cp.agent_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }

                //                    }
                //                    else if (type.ToUpper() == "PAYMENT")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                                          join tblmstreferral c on c.contactid = cp.agent_id where commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                       join tblmstreferral c on c.contactid = cp.agent_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0))X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                    }
                //                    else if (type.ToUpper() == "NONPAYMENT")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select 'Non Payments' as paymentstatus,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                        join tblmstreferral c on c.contactid = cp.agent_id where commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select 'Non Payments' as paymentstatus,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left  join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //         join tblmstreferral c on c.contactid = cp.agent_id where commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                //                        }
                //                    }
                //                }
                //                else
                //                {

                //                    if (type.ToUpper() == "ALL")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                //                           join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                               join tblmstreferral c on c.contactid = cp.agent_id where  agent_id='" + agentid + "' and commission_trans_date<='" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select case when paymentid is null then 'Non Payments' else 'Payments' end as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                //                                          join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                    join tblmstreferral c on c.contactid = cp.agent_id where  agent_id='" + agentid + "' and commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "')X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }

                //                    }
                //                    else if (type.ToUpper() == "PAYMENT")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                //                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                                                                                                                                                     join tblmstreferral c on c.contactid = cp.agent_id where agent_id ='" + agentid + "' and  commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select 'Payments' as paymentstatus,X.agentname,X.contactid,X.account_id,X.membername,X.fdaccountno,X.commission_payment_id,X.commssion_amount,X.tds_amount,X.totalamount,Y.paymentid,to_char(Y.paymentdate, 'dd/Mon/yyyy') as paymentdate,Y.modeofpayment,Y.paybank,Y.paybankbranch,Y.chequenumber FROM 
                //(select voucher_id,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount from commission_payment cp  left
                //                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                                                                                                                                                     join tblmstreferral c on c.contactid = cp.agent_id where agent_id=" + agentid + " and  commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is not null or voucher_id<>0) )X left join (select B.recordid, A.paymentid, A.paymentdate, A.transtype as modeofpayment,A.bankname as paybank,A.branchname as paybankbranch,A.chequenumber from tbltranspaymentreference A left join tbltranspaymentvoucher B on A.paymentid = B.paymentid)Y on X.voucher_id = Y.recordid; ";
                //                        }
                //                    }
                //                    else if (type.ToUpper() == "NONPAYMENT")
                //                    {
                //                        if (pdatecheked.ToUpper() == "ASON")
                //                        {
                //                            Query = @"select 'Non Payments' as paymentstatus,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left
                //                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                                                                                                                                                     join tblmstreferral c on c.contactid = cp.agent_id where agent_id=" + agentid + " and  commission_trans_date<='" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                //                        }
                //                        else if (pdatecheked.ToUpper() == "BETWEEN")
                //                        {
                //                            Query = @"select 'Non Payments' as paymentstatus,c.name||' '||c.surname as agentname,
                //fd.contactid, account_id,membername,fdaccountno,commission_payment_id,commssion_amount,tds_amount,round((commssion_amount - coalesce(tds_amount, 0))) as totalamount,null as paymentid,null as paymentdate,null as modeofpayment, null as paybank,null as paybankbranch,null as chequenumber from commission_payment cp  left
                //                                                                                                                                                                     join tbltransfdcreation fd on fd.fdaccountid = cp.trans_type_id left
                //                                                                                                                                                                     join tblmstreferral c on c.contactid = cp.agent_id where agent_id=" + agentid + " and  commission_trans_date between '" + FormatDate(frommonthof) + "' and '" + FormatDate(tomonthof) + "' and (voucher_id is null or voucher_id=0) ;";
                //                        }
                //                    }
                //                }


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {                 
                    while (dr.Read())
                    {
                        CommissionPaymentDetailsDTO objPromoterdetails = new CommissionPaymentDetailsDTO();
                        objPromoterdetails.pPaymentstatus = dr["paymentstatus"];
                        objPromoterdetails.pAgentname = dr["agentname"].ToString();
                        objPromoterdetails.pMembername = dr["membername"].ToString();
                        objPromoterdetails.pSchemeAccountno = dr["fdaccountno"].ToString();
                        objPromoterdetails.pCommissionamount = dr["commssion_amount"];
                        objPromoterdetails.pTdsamount = dr["tds_amount"];
                        objPromoterdetails.ptotalamount = dr["totalamount"];

                        objPromoterdetails.pvoucherno = dr["paymentid"].ToString();
                        objPromoterdetails.ppaymentdate = dr["paymentdate"].ToString();
                        objPromoterdetails.pmodeofpayment = dr["modeofpayment"].ToString();
                        objPromoterdetails.ppaybankname = dr["paybank"];
                        objPromoterdetails.ppaybankbranchname = dr["paybankbranch"];
                        objPromoterdetails.ppaychequeno = dr["chequenumber"];

                        lstPromoterDetails.Add(objPromoterdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPromoterDetails;
        }
        public List<CommissionPaymentDetailsDTO> GetViewCommisionpaymentdetails(string Connectionstring)
        {

            List<CommissionPaymentDetailsDTO> lstPromoterDetails = new List<CommissionPaymentDetailsDTO>();
            try
            {
                string Query = string.Empty;

                Query = @"select to_char(fd.transdate, 'dd/Mon/yyyy')transdate,membername,fd.fdaccountno,commission_payment_id,commssion_amount,tds_amount,(commssion_amount-coalesce(tds_amount,0)) as totalamount,fd.depositamount,fd.tenor||'-'||fd.tenortype as tenor,c.referralname as agentname,c.commsssionvalue from commission_payment cp  join tbltransfdcreation fd on fd.fdaccountid=cp.trans_type_id 
left join tbltransfdreferraldetails c on c.fdaccountno=fd.fdaccountno where  (voucher_id is not null or voucher_id<>0) 
and voucher_id in (select recordid from tbltranspaymentvoucher where paymentdate = current_date) ;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        CommissionPaymentDetailsDTO objPromoterdetails = new CommissionPaymentDetailsDTO();
                        objPromoterdetails.pMembername = dr["membername"].ToString();
                        objPromoterdetails.pSchemeAccountno = dr["fdaccountno"].ToString();
                        objPromoterdetails.pcommissionaymentid = Convert.ToInt64(dr["commission_payment_id"]);
                        objPromoterdetails.pCommissionamount = dr["commssion_amount"];
                        objPromoterdetails.pTdsamount = dr["tds_amount"];
                        objPromoterdetails.ptotalamount = dr["totalamount"];

                        objPromoterdetails.ptransdate = dr["transdate"].ToString();
                        objPromoterdetails.pDepositamount = dr["depositamount"].ToString();
                        objPromoterdetails.pTenor = dr["tenor"].ToString();
                        objPromoterdetails.pAgentname = dr["agentname"].ToString();
                        objPromoterdetails.pCommissionvalue = dr["commsssionvalue"].ToString();



                        lstPromoterDetails.Add(objPromoterdetails);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPromoterDetails;
        }




        //------------------END--------------------------------------------------
    }
}
