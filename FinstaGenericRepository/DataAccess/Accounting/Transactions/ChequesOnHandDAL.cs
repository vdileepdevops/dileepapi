using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaRepository.DataAccess.Accounting.Transactions;

namespace FinstaRepository.DataAccess.Accounting.Transactions
{
    public partial class ChequesOnHandDAL : SettingsDAL, IChequesOnHand
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        PaymentsDTO objPaymentsDTO = null;
        public List<ReceiptReferenceDTO> lstreceipts { get; set; }
        JournalVoucherDTO objJournalVoucherDTO = null;
        AccountingTransactionsDAL objAccountTransDal = null;
        List<PaymentsDTO> _Paymentslist = null;
        DataSet ds = null;
        string strTransNo = "";
        public async Task<List<ReceiptReferenceDTO>> GetChequesOnHandDetails(string ConnectionString)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select a.receiptid,date(a.receiptdate)receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,date(chequedate)chequedate,depositstatus,(select STRING_AGG(distinct parentaccountname,',') from tbltransgeneralreceiptdetails t1 join tblmstaccounts t2 on t1.creditaccountid=t2.accountid where receiptid =a.receiptid)as ledger from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid where depositstatus='N';"))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.pdepositstatus = false;
                            _receiptsDTO.pcancelstatus = false;
                            _receiptsDTO.preceiptid = Convert.ToString(dr["receiptid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["referencenumber"]);
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyid = Convert.ToInt64(dr["contactid"]);
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalreceivedamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]);
                            if (!string.IsNullOrEmpty(dr["chequedate"].ToString()))
                            {
                                _receiptsDTO.pchequedate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyname = Convert.ToString(dr["contactname"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["depositstatus"]);
                            _receiptsDTO.pledger = Convert.ToString(dr["ledger"]);
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstreceipts;
        }

        public async Task<List<ReceiptReferenceDTO>> GetDepositedCancelledCheques(string ConnectionString, string BrsFromDate, string BrsTodate, Int64 _BankId)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                string Query = string.Empty;
                try
                {
                    if ((string.IsNullOrEmpty(BrsFromDate) && string.IsNullOrEmpty(BrsTodate)) || (BrsFromDate == null && BrsTodate == null))
                    {
                        //Query = "select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('P') and clearstatus='N' and case when '" + _BankId + "'=0 then depositedbankid>0 else depositedbankid=" + _BankId + " end union all select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('C') and clearstatus='N';";
                        Query = "select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('P','C') and case when '" + _BankId + "'=0 then (depositedbankid>0 or depositedbankid is null) else depositedbankid=" + _BankId + " end; ";
                    }
                    else
                    {
                        //Query = "select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('P') and clearstatus='N' and coalesce(cleardate,depositeddate) between '" + FormatDate(BrsFromDate) + "' and '" + FormatDate(BrsTodate) + "' and case when '" + _BankId + "'=0 then depositedbankid>0 else depositedbankid=" + _BankId + " end union all select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('C') and clearstatus='N' and coalesce(cleardate,depositeddate) between '" + FormatDate(BrsFromDate) + "' and '" + FormatDate(BrsTodate) + "';";
                        Query = "select a.receiptid,a.receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,chequedate,depositedbankid,c.accountname as depositedbankname,depositstatus,depositeddate from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid left join tblmstbank c on b.depositedbankid=c.recordid where depositstatus in ('P','C')  and depositeddate between '" + FormatDate(BrsFromDate) + "' and '" + FormatDate(BrsTodate) + "' and case when '" + _BankId + "'=0 then depositedbankid>0 else depositedbankid=" + _BankId + " end ;";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.preceiptid = Convert.ToString(dr["receiptid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["referencenumber"]);
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyid = Convert.ToInt64(dr["contactid"]);
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalreceivedamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]);
                            if (!string.IsNullOrEmpty(dr["chequedate"].ToString()))
                            {
                                _receiptsDTO.pchequedate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyname = Convert.ToString(dr["contactname"]);
                            //  _receiptsDTO.pdepositbankid = Convert.ToInt64(dr["depositedbankid"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["depositstatus"]);
                            _receiptsDTO.pdepositbankname = Convert.ToString(dr["depositedbankname"]);
                            if (!string.IsNullOrEmpty(dr["depositeddate"].ToString()))
                            {
                                _receiptsDTO.pdepositeddate = Convert.ToDateTime(dr["depositeddate"]).ToString("dd/MM/yyyy");
                            }
                            if (dr["depositedbankid"] == DBNull.Value)
                            {

                            }
                            else
                            {
                                _receiptsDTO.pdepositbankid = Convert.ToInt64(dr["depositedbankid"]);
                            }
                            //  _receiptsDTO.pdepositedBankName = Convert.ToString(dr["depositedbankname"]);
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreceipts;
        }

        public bool SaveChequesOnHand(ChequesOnHandDTO ChequesOnHanddto, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sbChqonhand = new StringBuilder();
            string _PaymentId = string.Empty;

            objAccountTransDal = new AccountingTransactionsDAL();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                for (int i = 0; i < ChequesOnHanddto.pchequesOnHandlist.Count; i++)
                {
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "P")
                    {
                        GeneralreceiptDTO modelGeneralReceipt = new GeneralreceiptDTO();
                        modelGeneralReceipt.pchequedate = ChequesOnHanddto.ptransactiondate;
                        if (string.IsNullOrEmpty(modelGeneralReceipt.pchequedate))
                        {
                            modelGeneralReceipt.pchequedate = "null";
                        }
                        else
                        {
                            modelGeneralReceipt.pchequedate = "'" + FormatDate(ChequesOnHanddto.ptransactiondate) + "'";
                        }
                        modelGeneralReceipt.pdepositbankid = ChequesOnHanddto.pchequesOnHandlist[i].pdepositbankid;
                        modelGeneralReceipt.preceiptid = ChequesOnHanddto.pchequesOnHandlist[i].preceiptid;
                        modelGeneralReceipt.pChequenumber = ChequesOnHanddto.pchequesOnHandlist[i].pChequenumber;
                        modelGeneralReceipt.ptotalreceivedamount = ChequesOnHanddto.pchequesOnHandlist[i].ptotalreceivedamount;
                        modelGeneralReceipt.preceiptdate = ChequesOnHanddto.ptransactiondate;
                        modelGeneralReceipt.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;
                        //if (string.IsNullOrEmpty(modelGeneralReceipt.preceiptdate))
                        //{
                        //    modelGeneralReceipt.preceiptdate = "null";
                        //}
                        //else
                        //{
                        //    modelGeneralReceipt.preceiptdate = "'" + FormatDate(modelGeneralReceipt.preceiptdate) + "'";
                        //}
                        sbChqonhand.Append(objAccountTransDal.SaveGeneralReceiptTransactions_chequesinbank(trans, modelGeneralReceipt, "DEPOSIT",out strTransNo));
                    }
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "C")
                    {
                        Int64 UncleardChequesID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('UNCLEARED CHEQUES A/C') and statusid=" + Convert.ToInt32(Status.Active) + " and chracctype='2';"));
                        DataSet dsReceiptDetails = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, "select contactid,contactname,contactrefid,contactreftype from tbltransgeneralreceipt where receiptid  ='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");

                        PaymentVoucherDTO _PaymentVoucherDTO = new PaymentVoucherDTO();
                        _Paymentslist = new List<PaymentsDTO>();
                        _PaymentVoucherDTO.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;
                        _PaymentVoucherDTO.ppaymentdate = ChequesOnHanddto.ptransactiondate;
                        _PaymentVoucherDTO.ptotalpaidamount = ChequesOnHanddto.pchequesOnHandlist[i].ptotalreceivedamount;
                        _PaymentVoucherDTO.pmodofPayment = "CASH";
                        if (string.IsNullOrEmpty(_PaymentVoucherDTO.ppaymentdate))
                        {
                            _PaymentVoucherDTO.ppaymentdate = "null";
                        }
                        else
                        {
                            _PaymentVoucherDTO.ppaymentdate = ChequesOnHanddto.ptransactiondate;
                        }
                        _PaymentVoucherDTO.pnarration = "Cheque Cancel JV";
                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.psubledgerid = UncleardChequesID;
                        objPaymentsDTO.pamount = ChequesOnHanddto.pchequesOnHandlist[i].ptotalreceivedamount;
                        objPaymentsDTO.ptypeofoperation = "CREATE";
                        objPaymentsDTO.pisgstapplicable = false;
                        if (dsReceiptDetails.Tables[0].Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dsReceiptDetails.Tables[0].Rows[0]["contactid"].ToString()))
                            {
                                objPaymentsDTO.ppartyid = Convert.ToInt64(dsReceiptDetails.Tables[0].Rows[0]["contactid"]);
                            }
                            objPaymentsDTO.ppartyname = Convert.ToString(dsReceiptDetails.Tables[0].Rows[0]["contactname"]);
                            objPaymentsDTO.ppartyreferenceid = Convert.ToString(dsReceiptDetails.Tables[0].Rows[0]["contactrefid"]);
                            objPaymentsDTO.ppartyreftype = Convert.ToString(dsReceiptDetails.Tables[0].Rows[0]["contactreftype"]);
                        }
                        _Paymentslist.Add(objPaymentsDTO);
                        _PaymentVoucherDTO.ppaymentslist = _Paymentslist;
                        objAccountTransDal.SavePaymentVoucher_ALL(_PaymentVoucherDTO, trans, "CANCEL", out _PaymentId);
                        sbChqonhand.Append("update tbltransreceiptreference set depositstatus='" + ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() + "',depositeddate='" + FormatDate(ChequesOnHanddto.ptransactiondate) + "', depositmodifiedby=" + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ", depositmodifieddate=current_timestamp where receiptid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "';");

                        sbChqonhand.Append("update tbltransgeneralreceipt set returnreceiptid='" + _PaymentId + "', modifiedby=" + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ", modifieddate=current_timestamp where receiptid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "';");

                        if (ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges > 0)
                        {
                            Int64 BankParentId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where accountname='BANK CHARGES' and statusid=" + Convert.ToInt32(Status.Active) + " and chracctype='2';"));

                            Int64 BankCancelChargesID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('CHQ-RETURN-CHARGES', " + BankParentId + ", '3'," + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ")"));

                            Int64 PartyAccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select creditaccountid from tbltransgeneralreceiptdetails where receiptid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "' limit 1; "));
                            objJournalVoucherDTO = new JournalVoucherDTO();
                            _Paymentslist = new List<PaymentsDTO>();
                            objJournalVoucherDTO.pjvdate = ChequesOnHanddto.ptransactiondate;
                            objJournalVoucherDTO.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;
                            objJournalVoucherDTO.pnarration = "BANK RETURN CHARGES";
                            objJournalVoucherDTO.pmodoftransaction = "AUTO";
                            objPaymentsDTO = new PaymentsDTO();
                            objPaymentsDTO.ptranstype = "C";
                            objPaymentsDTO.psubledgerid = BankCancelChargesID;
                            objPaymentsDTO.pamount = ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges;
                            _Paymentslist.Add(objPaymentsDTO);
                            objPaymentsDTO = new PaymentsDTO();
                            objPaymentsDTO.ptranstype = "D";
                            objPaymentsDTO.psubledgerid = PartyAccountid;
                            objPaymentsDTO.pamount = ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges;
                            _Paymentslist.Add(objPaymentsDTO);


                            objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                            string refjvnumber = "";
                            objAccountTransDal.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                        }
                    }

                }
                if (sbChqonhand.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbChqonhand.ToString());
                    trans.Commit();
                    isSaved = true;
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
            return isSaved;
        }

        public ChequesOnHandDTO GetBankBalance(long recordid, string con)
        {
            ChequesOnHandDTO obj = new ChequesOnHandDTO();
            objAccountTransDal = new AccountingTransactionsDAL();
            DataSet ds = new DataSet();
            string brsfromdate = string.Empty;
            string brstodate = string.Empty;
            string fromdate = string.Empty;
            string todate = string.Empty;
            try
            {
                obj._BankBalance = objAccountTransDal.GetBankBalance(recordid, con);
                ds = NPGSqlHelper.ExecuteDataset(con, CommandType.Text, "select depositmodifieddate::date as modifieddate from tbltransreceiptreference where depositmodifieddate is not null union all select modifieddate::date from tbltranspaymentreference where modifieddate is not null order by  modifieddate desc limit 2;");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        brsfromdate = ds.Tables[0].Rows[0]["modifieddate"].ToString();
                        brstodate = ds.Tables[0].Rows[0]["modifieddate"].ToString();
                    }
                    else
                    {
                        brstodate = ds.Tables[0].Rows[0]["modifieddate"].ToString();
                        brsfromdate = ds.Tables[0].Rows[1]["modifieddate"].ToString();
                    }
                    fromdate = NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select max(cleardate) createddate from (select coalesce(cleardate,depositeddate) as cleardate from tbltransreceiptreference where createddate ::date='" + FormatDate(brstodate) + "' union all select cleardate from tbltranspaymentreference where createddate ::date='" + FormatDate(brstodate) + "' )x;").ToString();
                    if (!string.IsNullOrEmpty(fromdate))
                    {
                        obj.ptobrsdate = Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy");
                    }
                    todate = NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select min(cleardate) createddate from (select coalesce(cleardate,depositeddate) as cleardate from tbltransreceiptreference where createddate ::date='" + FormatDate(brsfromdate) + "' union all select cleardate from tbltranspaymentreference where createddate ::date='" + FormatDate(brstodate) + "' )x;").ToString();
                    if (!string.IsNullOrEmpty(todate))
                    {
                        obj.pfrombrsdate = Convert.ToDateTime(todate).ToString("dd/MM/yyyy");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }
        public async Task<List<ReceiptReferenceDTO>> GetIssuedDetails(string ConnectionString, Int64 _BankId)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                try
                {
                    string Query = "select a.paymentid,a.paymentdate,a.totalpaidamount,typeofpayment,chequenumber,bankname,clearstatus,cleardate from tbltranspaymentvoucher a join tbltranspaymentreference b on a.paymentid = b.paymentid where clearstatus ='N'";
                    if (_BankId != 0)
                    {
                        Query = Query + " and bankid=" + _BankId;
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.preceiptid = Convert.ToString(dr["paymentid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["chequenumber"]);
                            if (!string.IsNullOrEmpty(dr["paymentdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["paymentdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalpaidamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]);
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _receiptsDTO.pchequedate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.pdepositbankname = Convert.ToString(dr["bankname"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["clearstatus"]);
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreceipts;
        }
        public async Task<List<ReceiptReferenceDTO>> GetIssuedCancelledCheques(string ConnectionString, string BrsFromDate, string BrsTodate, Int64 _BankId)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                string Query = string.Empty;
                try
                {
                    if ((string.IsNullOrEmpty(BrsFromDate) && string.IsNullOrEmpty(BrsTodate)) || (BrsFromDate == null && BrsTodate == null))
                    {
                        Query = "select a.paymentid,a.paymentdate,a.totalpaidamount,typeofpayment,chequenumber,bankname,clearstatus,cleardate  from tbltranspaymentvoucher a join tbltranspaymentreference b on a.paymentid = b.paymentid  where clearstatus in ('P','C','R') and case when '" + _BankId + "'=0 then creditaccountid>0 else creditaccountid=" + _BankId + " end; ";
                    }
                    else
                    {
                        Query = "select a.paymentid,a.paymentdate,a.totalpaidamount,typeofpayment,chequenumber,bankname,clearstatus,cleardate  from tbltranspaymentvoucher a join tbltranspaymentreference b on a.paymentid = b.paymentid  where clearstatus in ('P','C','R') and cleardate between '" + FormatDate(BrsFromDate) + "' and '" + FormatDate(BrsTodate) + "' and case when '" + _BankId + "'=0 then creditaccountid>0 else creditaccountid=" + _BankId + " end; ";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.preceiptid = Convert.ToString(dr["paymentid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["chequenumber"]);
                            if (!string.IsNullOrEmpty(dr["paymentdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["paymentdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalpaidamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]);
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _receiptsDTO.pdepositeddate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.pdepositbankname = Convert.ToString(dr["bankname"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["clearstatus"]);
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreceipts;
        }
        public bool SaveChequesIssued(ChequesOnHandDTO ChequesOnHanddto, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sbChqIssuedhand = new StringBuilder();
            string _PaymentId = string.Empty;

            objAccountTransDal = new AccountingTransactionsDAL();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                for (int i = 0; i < ChequesOnHanddto.pchequesOnHandlist.Count; i++)
                {
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "P")
                    {
                        sbChqIssuedhand.Append("update tbltranspaymentreference set clearstatus='" + ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() + "',cleardate='" + FormatDate(ChequesOnHanddto.ptransactiondate) + "', modifiedby=" + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ", modifieddate=current_timestamp where paymentid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "';");
                    }
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "C" || ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "R")
                    {
                        sbChqIssuedhand.Append("update tbltranspaymentreference set clearstatus='" + ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() + "',cleardate='" + FormatDate(ChequesOnHanddto.ptransactiondate) + "', modifiedby=" + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ", modifieddate=current_timestamp where paymentid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "';");

                        GeneralreceiptDTO modelGeneralReceipt = new GeneralreceiptDTO();
                        modelGeneralReceipt.pchequedate = ChequesOnHanddto.ptransactiondate;
                        modelGeneralReceipt.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;
                        if (string.IsNullOrEmpty(modelGeneralReceipt.pchequedate))
                        {
                            modelGeneralReceipt.pchequedate = "null";
                        }
                        else
                        {
                            modelGeneralReceipt.pchequedate = "'" + FormatDate(ChequesOnHanddto.ptransactiondate) + "'";
                        }
                        modelGeneralReceipt.preceiptid = ChequesOnHanddto.pchequesOnHandlist[i].preceiptid;
                        modelGeneralReceipt.pChequenumber = ChequesOnHanddto.pchequesOnHandlist[i].pChequenumber;
                        modelGeneralReceipt.ptotalreceivedamount = ChequesOnHanddto.pchequesOnHandlist[i].ptotalreceivedamount;
                        ds = new DataSet();
                        string refjvnumber = "";
                        ds = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, "select recordid,debitaccountid,coalesce(tdsamount,0) as tdsamount,coalesce(tdsaccountid,0) as tdsaccountid  from tbltranspaymentvoucherdetails where paymentid  in('" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "');");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                            {
                                if (Convert.ToInt32(ds.Tables[0].Rows[j]["tdsamount"]) > 0)
                                {
                                    List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
                                    objJournalVoucherDTO = new JournalVoucherDTO();
                                    objJournalVoucherDTO.pjvdate = ChequesOnHanddto.ptransactiondate;
                                    objJournalVoucherDTO.pnarration = "BE JV PASSED TOWARDS TDS AMOUNT";
                                    objJournalVoucherDTO.pmodoftransaction = "AUTO";
                                    objJournalVoucherDTO.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[0].pCreatedby;
                                    objPaymentsDTO = new PaymentsDTO();
                                    objPaymentsDTO.ptranstype = "C";
                                    objPaymentsDTO.psubledgerid = Convert.ToInt64(ds.Tables[0].Rows[j]["tdsaccountid"]);
                                    objPaymentsDTO.pamount = Convert.ToInt64(ds.Tables[0].Rows[j]["tdsamount"]);
                                    _Paymentslist.Add(objPaymentsDTO);

                                    objPaymentsDTO = new PaymentsDTO();
                                    objPaymentsDTO.ptranstype = "D";
                                    objPaymentsDTO.psubledgerid = Convert.ToInt64(ds.Tables[0].Rows[j]["debitaccountid"]);
                                    objPaymentsDTO.pamount = Convert.ToInt32(ds.Tables[0].Rows[j]["tdsamount"]);
                                    _Paymentslist.Add(objPaymentsDTO);
                                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;

                                    objAccountTransDal.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                                    sbChqIssuedhand.Append("update tbltranspaymentvoucherdetails set tdscancelrefjvnumber='" + refjvnumber + "' where recordid=" + Convert.ToInt32(ds.Tables[0].Rows[j]["recordid"]) + ";");
                                }
                            }
                        }
                        SaveBankInformation(modelGeneralReceipt, trans);
                    }
                }
                if (sbChqIssuedhand.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbChqIssuedhand.ToString());
                }
                trans.Commit();
                isSaved = true;
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
            return isSaved;
        }

        public bool SaveBankInformation(GeneralreceiptDTO _GeneralreceiptDTO, NpgsqlTransaction trans)

        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                string BankaccountTransid = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "SELECT FN_GENERATENEXTID('CHEQUES ON HAND',''," + (_GeneralreceiptDTO.pchequedate) + ")"));

                string query = "";
                long detailsid = 0;
                if (string.IsNullOrEmpty(_GeneralreceiptDTO.ptypeofoperation))
                {
                    _GeneralreceiptDTO.ptypeofoperation = "CREATE";
                }
                if (_GeneralreceiptDTO.ptypeofoperation.ToUpper() == "CREATE")
                {
                    query = "insert into tbltransbankinformation (transactionno,transactiondate,receiptid,referencenumber,statusid,createdby,createddate) values('" + BankaccountTransid + "', " + (_GeneralreceiptDTO.pchequedate) + ", '" + ManageQuote(_GeneralreceiptDTO.preceiptid) + "','" + ManageQuote(_GeneralreceiptDTO.pChequenumber) + "',  " + Convert.ToInt32(Status.Active) + ", " + _GeneralreceiptDTO.pCreatedby + ", current_timestamp) returning recordid;";
                    detailsid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));

                    sbQuery.Append("insert into tbltransbankinformationdetails (detailsid,transactionid,accountid,ledgeramount,accounttype,contactid,contactname,contactrefid,contactreftype)  select detailsid,transid,creditaccountid,sum(totalpaidamount) as totalpaidamount,transtype,contactid,contactname,contactrefid,contactreftype from (select " + detailsid + " as detailsid,'" + ManageQuote(BankaccountTransid) + "' as transid ,creditaccountid,totalpaidamount,'D' as transtype,0 as contactid,'' as contactname,'' as contactrefid,'' as contactreftype from tbltranspaymentvoucher where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') union all select " + detailsid + ",'" + ManageQuote(BankaccountTransid) + "', debitaccountid,ledgeramount+coalesce(tdsamount,0),'C',contactid,contactname,contactrefid,contactreftype from tbltranspaymentvoucherdetails where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') union all select " + detailsid + ",'" + ManageQuote(BankaccountTransid) + "', cgstaccountid,cgstamount,'C',0 as contactid,'' as contactname,'' as contactrefid,'' as contactreftype from tbltranspaymentvoucherdetails where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') and coalesce(cgstamount,0)<>0 union all select " + detailsid + ",'" + ManageQuote(BankaccountTransid) + "', igstaccountid,igstamount,'C',0 as contactid,'' as contactname,'' as contactrefid,'' as contactreftype from tbltranspaymentvoucherdetails where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') and coalesce(igstamount,0)<>0 union all select " + detailsid + ",'" + ManageQuote(BankaccountTransid) + "', sgstaccountid,sgstamount,'C',0 as contactid,'' as contactname,'' as contactrefid,'' as contactreftype from tbltranspaymentvoucherdetails where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') and coalesce(sgstamount,0)<>0 union all select " + detailsid + ",'" + ManageQuote(BankaccountTransid) + "', utgstaccountid,utgstamount,'C',0 as contactid,'' as contactname,'' as contactrefid,'' as contactreftype from tbltranspaymentvoucherdetails where paymentid  in('" + _GeneralreceiptDTO.preceiptid + "') and coalesce(utgstamount,0)<>0) x group by detailsid,transid,creditaccountid,transtype,contactid,contactname,contactrefid,contactreftype;");
                }
                if (!string.IsNullOrEmpty(BankaccountTransid))
                {
                    sbQuery.AppendLine("SELECT fntotaltransactions('" + BankaccountTransid + "','CHEQUESONHAND');");
                    //select accountsupdate();
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                }
                IsSaved = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsSaved;
        }

        #region ChequesInBank

        public async Task<List<ReceiptReferenceDTO>> GetChequesInBankData(string ConnectionString, long DepositedBankId)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                try
                {
                    string strQuery = "select a.receiptid,date(a.receiptdate)receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,referencenumber,date(chequedate)chequedate,clearstatus as depositstatus,c.recordid as depositedbankid,date(depositeddate)depositeddate,(select STRING_AGG(distinct parentaccountname,',') from tbltransgeneralreceiptdetails t1 join tblmstaccounts t2 on t1.creditaccountid=t2.accountid where receiptid =a.receiptid)as ledger from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid join tblmstbank c on b.depositedbankid=c.recordid where depositstatus='P' and clearstatus='N' ";
                    if (DepositedBankId != 0)
                    {
                        strQuery = strQuery + "and c.recordid=" + DepositedBankId;
                    }


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.pdepositstatus = false;
                            _receiptsDTO.pcancelstatus = false;
                            _receiptsDTO.preceiptid = Convert.ToString(dr["receiptid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["referencenumber"]);
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyid = Convert.ToInt64(dr["contactid"]);
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalreceivedamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]).ToUpper();
                            if (!string.IsNullOrEmpty(dr["chequedate"].ToString()))
                            {
                                _receiptsDTO.pchequedate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            }

                            _receiptsDTO.ppartyname = Convert.ToString(dr["contactname"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["depositstatus"]);
                            _receiptsDTO.pledger = Convert.ToString(dr["ledger"]);
                            if (!string.IsNullOrEmpty(dr["depositeddate"].ToString()))
                            {
                                _receiptsDTO.pdepositeddate = Convert.ToDateTime(dr["depositeddate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.pdepositbankid = Convert.ToInt64(dr["depositedbankid"]);
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreceipts;
        }

        public async Task<List<ReceiptReferenceDTO>> GetClearedReturnedCheques(string ConnectionString, string BrsFromDate, string BrsTodate, long DepositedBankId)
        {
            await Task.Run(() =>
            {
                lstreceipts = new List<ReceiptReferenceDTO>();
                string Query = string.Empty;
                try
                {
                    if ((string.IsNullOrEmpty(BrsFromDate) && string.IsNullOrEmpty(BrsTodate)) || (BrsFromDate == null && BrsTodate == null) || (BrsFromDate == "null" && BrsTodate == "null"))
                    {
                        Query = "select a.receiptid,date(a.receiptdate)receiptdate,a.totalreceivedamount,contactid,contactname,typeofpayment,depositedbankid,referencenumber,date(chequedate)chequedate,clearstatus as depositstatus,date(depositeddate)depositeddate,date(cleardate) as cleardate,(select STRING_AGG(distinct parentaccountname,',') from tbltransgeneralreceiptdetails t1 join tblmstaccounts t2 on t1.creditaccountid=t2.accountid where receiptid =a.receiptid)as ledger from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid join tblmstbank c on b.depositedbankid=c.recordid where depositstatus='P' and clearstatus in ('Y','R') ";
                    }
                    else
                    {
                        Query = "select a.receiptid,date(a.receiptdate)receiptdate,a.totalreceivedamount,contactid,contactname,depositedbankid,typeofpayment,referencenumber,date(chequedate)chequedate,clearstatus as depositstatus,date(depositeddate)depositeddate,date(cleardate) as cleardate,(select STRING_AGG(distinct parentaccountname,',') from tbltransgeneralreceiptdetails t1 join tblmstaccounts t2 on t1.creditaccountid=t2.accountid where receiptid =a.receiptid)as ledger from tbltransgeneralreceipt a join tbltransreceiptreference b on a.receiptid = b.receiptid join tblmstbank c on b.depositedbankid=c.recordid where depositstatus='P' and clearstatus in ('Y','R') and cleardate between '" + FormatDate(BrsFromDate) + "' and '" + FormatDate(BrsTodate) + "' ";
                    }
                    if (DepositedBankId != 0)
                    {
                        Query = Query + "and c.recordid=" + DepositedBankId + ";";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ReceiptReferenceDTO _receiptsDTO = new ReceiptReferenceDTO();
                            _receiptsDTO.preceiptid = Convert.ToString(dr["receiptid"]);
                            _receiptsDTO.pChequenumber = Convert.ToString(dr["referencenumber"]);
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _receiptsDTO.preceiptdate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyid = Convert.ToInt64(dr["contactid"]);
                            _receiptsDTO.ptotalreceivedamount = Convert.ToDecimal(dr["totalreceivedamount"]);
                            _receiptsDTO.ptypeofpayment = Convert.ToString(dr["typeofpayment"]).ToUpper();
                            if (!string.IsNullOrEmpty(dr["chequedate"].ToString()))
                            {
                                _receiptsDTO.pchequedate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            }
                            _receiptsDTO.ppartyname = Convert.ToString(dr["contactname"]);
                            _receiptsDTO.pchequestatus = Convert.ToString(dr["depositstatus"]);
                            //_receiptsDTO.pdepositbankname = Convert.ToString(dr["depositedbankname"]);

                            if (dr["depositedbankid"] == DBNull.Value)
                            {
                            }
                            else
                            {
                                _receiptsDTO.pdepositbankid = Convert.ToInt64(dr["depositedbankid"]);
                            }
                            _receiptsDTO.pledger = Convert.ToString(dr["ledger"]);
                            if (!string.IsNullOrEmpty(dr["depositeddate"].ToString()))
                            {
                                _receiptsDTO.pdepositeddate = Convert.ToDateTime(dr["depositeddate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _receiptsDTO.pcleardate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            lstreceipts.Add(_receiptsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreceipts;
        }

        public bool SaveChequesInBank(ChequesOnHandDTO ChequesOnHanddto, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sbChqinbank = new StringBuilder();
            string _PaymentId = string.Empty;
            objJournalVoucherDTO = new JournalVoucherDTO();
            objAccountTransDal = new AccountingTransactionsDAL();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                for (int i = 0; i < ChequesOnHanddto.pchequesOnHandlist.Count; i++)
                {
                    GeneralreceiptDTO modelGeneralReceipt = new GeneralreceiptDTO();
                    List<GeneralreceiptDTO> _lstGeneralreceiptDTO = new List<GeneralreceiptDTO>();

                    modelGeneralReceipt.pchequedate = ChequesOnHanddto.ptransactiondate;
                    if (string.IsNullOrEmpty(modelGeneralReceipt.pchequedate))
                    {
                        modelGeneralReceipt.pchequedate = "null";
                    }
                    else
                    {
                        modelGeneralReceipt.pchequedate = "'" + FormatDate(ChequesOnHanddto.ptransactiondate) + "'";
                    }
                    modelGeneralReceipt.pdepositbankid = ChequesOnHanddto.pchequesOnHandlist[i].pdepositbankid;
                    modelGeneralReceipt.preceiptid = ChequesOnHanddto.pchequesOnHandlist[i].preceiptid;
                    modelGeneralReceipt.pChequenumber = ChequesOnHanddto.pchequesOnHandlist[i].pChequenumber;
                    modelGeneralReceipt.ptotalreceivedamount = ChequesOnHanddto.pchequesOnHandlist[i].ptotalreceivedamount;
                    modelGeneralReceipt.preceiptdate = ChequesOnHanddto.ptransactiondate;
                    modelGeneralReceipt.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;

                    //if (string.IsNullOrEmpty(modelGeneralReceipt.preceiptdate))
                    //{
                    //    modelGeneralReceipt.preceiptdate = "null";
                    //}
                    //else
                    //{
                    //    modelGeneralReceipt.preceiptdate = "'" + FormatDate(ChequesOnHanddto.ptransactiondate) + "'";
                    //    //modelGeneralReceipt.preceiptdate =(ChequesOnHanddto.ptransactiondate);
                    //}
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "Y")
                    {

                        List<ReceiptsDTO> _lstreceipt = new List<ReceiptsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select contactid as partyid,contactname as partyname,contactrefid as partyreferenceid,contactreftype as partyreftype,coalesce(istdsapplicable,false) as istdsapplicable,coalesce(a.tdsamount,0) as tdsamount,creditaccountid as subledgerid,b.tdssection,coalesce(b.tdsamount,0) as tdsamountindividual from tbltransgeneralreceipt a join  tbltransgeneralreceiptdetails b on a.receiptid=b.receiptid where a.receiptid ='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "'"))
                        {
                            while (dr.Read())
                            {
                                ReceiptsDTO objreceipts = new ReceiptsDTO();
                                modelGeneralReceipt.ppartyid = Convert.ToInt64(dr["partyid"]);
                                modelGeneralReceipt.ppartyname = Convert.ToString(dr["partyname"]);
                                modelGeneralReceipt.ppartyreferenceid = Convert.ToString(dr["partyreferenceid"]);
                                modelGeneralReceipt.ppartyreftype = Convert.ToString(dr["partyreftype"]);
                                modelGeneralReceipt.pistdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                                modelGeneralReceipt.ptdsamount = Convert.ToDecimal(dr["tdsamount"]);

                                objreceipts.psubledgerid = Convert.ToInt64(dr["subledgerid"]);
                                objreceipts.ptdssection = Convert.ToString(dr["tdssection"]);
                                objreceipts.ptdsamountindividual = Convert.ToDecimal(dr["tdsamountindividual"]);
                                _lstreceipt.Add(objreceipts);
                                modelGeneralReceipt.preceiptslist = _lstreceipt;
                            }

                        }
                       
                       // if (ChequesOnHanddto.pchequesOnHandlist[i].preceiptid.StartsWith("CHQ"))
                       // {
                            sbChqinbank.Append(objAccountTransDal.SaveGeneralReceiptTransactions_chequesclear(trans, modelGeneralReceipt));
                       // }
                    }
                    if (ChequesOnHanddto.pchequesOnHandlist[i].pchequestatus.ToUpper() == "R")
                    {
                        
                        sbChqinbank.Append(objAccountTransDal.SaveGeneralReceiptTransactions_chequesinbank(trans, modelGeneralReceipt, "RETURN", out strTransNo));

                        sbChqinbank.Append("update tbltransgeneralreceipt set returnreceiptid='" + strTransNo + "', modifiedby=" + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ", modifieddate=current_timestamp where receiptid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "';");

                        List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();

                        if (ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges > 0)
                        {
                            Int64 Bankchargesid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where accountname='FINANCE COST AND BANK CHARGES';"));

                            Int64 BankParentId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('BANK CHARGES', " + Bankchargesid + ", '2'," + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ")"));

                            //Int64 BankParentId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where accountname='BANK CHARGES';"));

                            Int64 BankCancelChargesID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('CHQ-RETURN-CHARGES', " + BankParentId + ", '3'," + ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby + ")"));

                            Int64 PartyAccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select creditaccountid from tbltransgeneralreceiptdetails where receiptid='" + ChequesOnHanddto.pchequesOnHandlist[i].preceiptid + "' limit 1; "));
                            objJournalVoucherDTO.pjvdate = ChequesOnHanddto.ptransactiondate;
                            objJournalVoucherDTO.pCreatedby = ChequesOnHanddto.pchequesOnHandlist[i].pCreatedby;
                            objJournalVoucherDTO.pnarration = "BANK RETURN CHARGES";
                            objJournalVoucherDTO.pmodoftransaction = "AUTO";
                            objPaymentsDTO = new PaymentsDTO();
                            objPaymentsDTO.ptranstype = "C";
                            objPaymentsDTO.psubledgerid = BankCancelChargesID;
                            objPaymentsDTO.pamount = ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges;
                            _Paymentslist.Add(objPaymentsDTO);
                            objPaymentsDTO = new PaymentsDTO();
                            objPaymentsDTO.ptranstype = "D";
                            objPaymentsDTO.psubledgerid = PartyAccountid;
                            objPaymentsDTO.pamount = ChequesOnHanddto.pchequesOnHandlist[i].pactualcancelcharges;
                            _Paymentslist.Add(objPaymentsDTO);

                            objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                            string jvrefnumber = string.Empty;
                            objAccountTransDal.SaveJournalVoucher(objJournalVoucherDTO, trans, out jvrefnumber);
                        }
                    }
                }
                if (sbChqinbank.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbChqinbank.ToString());
                    trans.Commit();
                    isSaved = true;
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
            return isSaved;
        }
        #endregion
    }
}
