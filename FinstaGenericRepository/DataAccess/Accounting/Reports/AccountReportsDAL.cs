using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Reports;
using FinstaRepository.Interfaces.Accounting.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Accounting.Reports
{
    public partial class AccountReportsDAL : SettingsDAL, IAccountingReports
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public List<AccountReportsDTO> lstcashbook { get; set; }
        public List<BankBookDTO> lstbankbook { get; set; }
        public List<BRSDto> lstBRS { get; set; }
        public List<DayBookDto> lstdaybook { get; set; }
        public List<ComparisionTb> lstCompTb { get; set; }
        public List<ProfitAndLossDTO> lstProfitAndLoss { get; set; }
        public List<BalanceSheetDTO> lstBalanceSheet { get; set; }
        public List<AccountReportsDTO> lstreportdtl { get; set; }
        public List<ChequeEnquiryDTO> lstChequeEnquiry { get; set; }
        public List<JvListDTO> lstJvList { get; set; }

        public List<IssuedChequeDTO> lstIssuedCheque { get; set; }

        public List<ledgerExtractDTO> lstLedgerExtract { get; set; }
        public List<subAccountLedgerDTO> lstSubAccountLedger { get; set; }
        public List<subLedgerSummaryDTO> lstSubLedgerSummary { get; set; }

        #region Bank Book
        public async Task<List<BankBookDTO>> GetBankNames(string con)
        {
            await Task.Run(() =>
            {
                lstbankbook = new List<BankBookDTO>();
                try
                {
                    string Query = "select recordid bankaccountid,case when (accountnumber is null or accountnumber='') then coalesce(bankname,'') else coalesce(bankname,'')|| ' - ' || coalesce(accountnumber) end as bankname from  tblmstbank t1 join tblmststatus ts on t1.statusid=ts.statusid where ts.statusid=" + Convert.ToInt32(Status.Active) + " order by bankname;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            BankBookDTO _Obj = new BankBookDTO();
                            _Obj.pbankaccountid = Convert.ToInt64(dr["bankaccountid"]);
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            lstbankbook.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstbankbook;
        }
        public async Task<List<AccountReportsDTO>> GetBankBookDetails(string con, string fromDate, string toDate, long _pBankAccountId)
        {
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    long BankId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select bankaccountid from  tblmstbank where recordid=" + _pBankAccountId + ""));
                    string Query = "select row_number() over (order by transactiondate) as recordid,* from (select transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,DEBITAMOUNT,abs(CREDITAMOUNT)CREDITAMOUNT,abs(balance)balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from (select *,sum(DEBITAMOUNT+CREDITAMOUNT) OVER(ORDER BY transactiondate,RECORDID)as BALANCE from(SELECT 0 AS RECORDID,CAST('" + FormatDate(fromDate) + "' AS DATE) AS transactiondate,'0' AS TRANSACTIONNO,'Opening Balance' AS PARTICULARS,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)>0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)<0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END  AS CREDITAMOUNT,'' DESCRIPTION  FROM tbltranstotaltransactions WHERE transactiondate < '" + FormatDate(fromDate) + "'  AND  accountid=" + BankId + " UNION ALL SELECT RECORDID, transactiondate,TRANSACTIONNO,PARTICULARS,COALESCE(DEBITAMOUNT,0.00) as  DEBITAMOUNT,-COALESCE(CREDITAMOUNT,0.00) as CREDITAMOUNT,DESCRIPTION FROM tbltranstotaltransactions WHERE transactiondate BETWEEN '" + FormatDate(fromDate) + "' AND '" + FormatDate(toDate) + "' AND accountid=" + BankId + " AND ( debitamount<>0 or creditamount<>0) order by transactiondate,RECORDID) as D) x union all select transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,DEBITAMOUNT,abs(CREDITAMOUNT) CREDITAMOUNT,abs(balance) balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from (SELECT 0 AS RECORDID,CAST('" + FormatDate(toDate) + "' AS DATE) AS transactiondate,'0' AS TRANSACTIONNO,'Closing Balance' AS PARTICULARS,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)>0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)<0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END  AS CREDITAMOUNT,'' DESCRIPTION, COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) AS balance  FROM tbltranstotaltransactions WHERE transactiondate <='" + FormatDate(toDate) + "'  AND  accountid=" + BankId + " order by recordid,transactiondate ) x) x order by recordid; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _ObjBank = new AccountReportsDTO();
                            _ObjBank.precordid = Convert.ToInt64(dr["recordid"]);
                            _ObjBank.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString("dd/MM/yyyy");
                            _ObjBank.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                            _ObjBank.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                            _ObjBank.pdescription = Convert.ToString(dr["DESCRIPTION"]);
                            _ObjBank.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                            _ObjBank.ptransactionno = Convert.ToString(dr["TRANSACTIONNO"]);
                            _ObjBank.popeningbal = Convert.ToDouble(dr["balance"]);
                            _ObjBank.pBalanceType = Convert.ToString(dr["balancetype"]);
                            lstcashbook.Add(_ObjBank);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstcashbook;
        }
        #endregion
        #region Cashbook        
        public async Task<List<AccountReportsDTO>> getCashbookData(string ConnectionString, string fromdate, string todate, string transType)
        {
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    string accountid = string.Empty;
                    accountid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(accountid::varchar,',') from tblmstaccounts where case when '" + transType.ToUpper() + "'='CASH' THEN accountname='CASH ON HAND' WHEN '" + transType.ToUpper() + "'='CHEQUE' THEN  accountname='CHEQUE ON HAND'  ELSE accountname in ('CASH ON HAND','CHEQUE ON HAND')  END  and chracctype='2'"));
                    if (!string.IsNullOrEmpty(accountid))
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text,"select transactiondate, transactionno, particulars, description, debitamount, creditamount, ABS(bal) as balance,case when bal >= 0 then 'Dr' else 'Cr' END AS balancetype from(select t.*,sum(debitamount - creditamount) over(order by transactiondate, recordid) as bal from(SELECT 0 as recordid, '" + FormatDate(fromdate) + "' as transactiondate, null TRANSACTIONNO, 'OPENING BALANCE' particulars, ''description,case when  COALESCE(SUM(DEBITAMOUNT) - SUM(CREDITAMOUNT), 0) >= 0 then COALESCE(SUM(DEBITAMOUNT) - SUM(CREDITAMOUNT), 0) else 0 end debitamount,case when COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) < 0 then ABS(COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)) else 0 end creditamount   FROM tbltranstotaltransactions WHERE transactiondate< '" + FormatDate(fromdate) + "' and PARENTID in (" + accountid + ") union all SELECT recordid, transactiondate, TRANSACTIONNO, PARTICULARS, DESCRIPTION, COALESCE(DEBITAMOUNT, 0.00) AS DEBITAMOUNT, COALESCE(CREDITAMOUNT, 0.00) AS CREDITAMOUNT FROM tbltranstotaltransactions  WHERE PARENTID in (" + accountid + ") AND transactiondate BETWEEN '" + FormatDate(fromdate) + "' AND '" + FormatDate(todate) + "' order by transactiondate,recordid)t)t1 where(debitamount<>0 or creditamount<>0) ;"))
                        {
                            while (dr.Read())
                            {
                                AccountReportsDTO _cashbookDTO = new AccountReportsDTO();
                                _cashbookDTO.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString("dd/MM/yyyy");
                                _cashbookDTO.ptransactionno = Convert.ToString(dr["transactionno"]);
                                _cashbookDTO.pdescription = Convert.ToString(dr["description"]);
                                _cashbookDTO.pparticulars = Convert.ToString(dr["particulars"]);
                                _cashbookDTO.pcreditamount = Convert.ToDouble(dr["creditamount"]);
                                _cashbookDTO.pdebitamount = Convert.ToDouble(dr["debitamount"]);
                                _cashbookDTO.pclosingbal = Convert.ToDouble(dr["balance"]);
                                _cashbookDTO.pBalanceType = Convert.ToString(dr["balancetype"]);
                                lstcashbook.Add(_cashbookDTO);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstcashbook;
        }

        public async Task<List<AccountReportsDTO>> getCashbookDataTotals(string ConnectionString, string fromdate, string todate)
        {
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                Int64 maincashaccid = 117;
                try
                {
                    string strquery = "SELECT sum(debitamount)debitamount,sum(creditamount)creditamount FROM tbltranstotaltransactions  WHERE PARENTID=" + maincashaccid + " AND TRANSACTIONDATE BETWEEN '" + FormatDate(fromdate) + "' AND '" + FormatDate(todate) + "';";
                    string chequebal = "SELECT COALESCE(SUM(totalreceivedamount),0) FROM tbltransreceiptreference  WHERE depositstatus ='N';";
                    string closingbal = "select COALESCE(accountbalance,0) from tblmstaccounts  where accountid=" + maincashaccid + ";";
                    string directcash = "SELECT COALESCE(sum(AB.ledgeramount),0) FROM (SELECT COALESCE(X.RECEIPTid,y.RECEIPTID)as VCHRECEIPTNO,Y.GRDATE,Y.ledgeramount,Y.PARENTACCOUNTNAME,Y.ACCOUNTNAME,Y.NARRATION,X.referencenumber, X.BANKNAME,X.BRANCHNAME FROM (SELECT RECEIPTid,chequedate,referencenumber,BANKNAME,BRANCHNAME FROM tbltransreceiptreference WHERE chequedate>='" + FormatDate(fromdate) + "' AND chequedate<='" + FormatDate(todate) + "') X    RIGHT JOIN  (SELECT TGR.RECEIPTID,TGR.receiptdate AS GRDATE,TGR.NARRATION,TGD.ledgeramount,TAT.PARENTACCOUNTNAME,TAT.ACCOUNTNAME FROM tbltransgeneralreceiptdetails TGD   JOIN tbltransgeneralreceipt TGR ON TGR.receiptdate>='" + FormatDate(fromdate) + "' and TGR.receiptdate<='" + FormatDate(todate) + "'  AND TGD.DETAILSID=TGR.RECORDID JOIN tblmstaccounts TAT ON TGR.debitaccountid=TAT.ACCOUNTID) AS Y ON X.RECEIPTid=Y.RECEIPTID ORDER BY Y.RECEIPTID) AS AB where AB.VCHRECEIPTNO  NOT like 'CHQ%' and AB.GRDATE>='" + FormatDate(fromdate) + "' AND AB.GRDATE<='" + FormatDate(todate) + "';";
                    DataSet ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, strquery + chequebal + closingbal + directcash);
                    double pchqbal = Convert.ToDouble(ds.Tables[1].Rows[0][0]);
                    double ptotaldebt = Convert.ToDouble(ds.Tables[0].Rows[0]["debitamount"]);
                    double pclosingbal = Convert.ToDouble(ds.Tables[2].Rows[0][0]);
                    double pdirectcash = Convert.ToDouble(ds.Tables[3].Rows[0][0]);
                    AccountReportsDTO _cashbookDTO = new AccountReportsDTO();
                    _cashbookDTO.pmodeoftransaction = "CHEQUE";
                    _cashbookDTO.pcashtotal = ptotaldebt - pdirectcash;
                    _cashbookDTO.pclosingbal = pchqbal;
                    lstcashbook.Add(_cashbookDTO);
                    _cashbookDTO = new AccountReportsDTO();
                    _cashbookDTO.pmodeoftransaction = "CASH";
                    _cashbookDTO.pcashtotal = pdirectcash;
                    _cashbookDTO.pclosingbal = pclosingbal - pchqbal;
                    lstcashbook.Add(_cashbookDTO);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstcashbook;
        }

        #endregion

        #region AccountLedger
        public async Task<List<AccountReportsDTO>> GetAccountLedgerDetails(string con, string fromDate, string toDate, long pAccountId, long pSubAccountId)
        {
            string Query = string.Empty;
            string pQuery = string.Empty;
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    if (pSubAccountId > 0)
                    {
                        pQuery = " and accountid=" + pSubAccountId;
                    }
                    //else if (pAccountId > 0)
                    //{
                    //    pQuery = " and parentid = " + pAccountId;
                    //}
                    Query = "select rownumber as recordid,parentid,accountid,formname,transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,DEBITAMOUNT,abs(CREDITAMOUNT)as CREDITAMOUNT,abs(balance) as balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from (select row_number() over (order by transactiondate,RECORDID) as rownumber,*,sum(DEBITAMOUNT+CREDITAMOUNT) OVER(ORDER BY transactiondate,RECORDID)as BALANCE from(SELECT 0 AS RECORDID,'' as formname,0 parentid,0 accountid,CAST('" + FormatDate(fromDate) + "' AS DATE) AS transactiondate,'0' AS TRANSACTIONNO,'Opening Balance' AS PARTICULARS,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)>0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)<0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END  AS CREDITAMOUNT,'' DESCRIPTION  FROM tbltranstotaltransactions WHERE transactiondate < '" + FormatDate(fromDate) + "'  and parentid = " + pAccountId + " ";
                    Query = Query + pQuery + " UNION ALL select distinct x.RECORDID,formname,parentid,accountid, transactiondate,TRANSACTIONNO,PARTICULARS,COALESCE(DEBITAMOUNT,0.00) as  DEBITAMOUNT,COALESCE(CREDITAMOUNT,0.00) as CREDITAMOUNT,DESCRIPTION from (SELECT row_number() over (order by transactiondate,TRANSACTIONNO) RECORDID,parentid,accountid, transactiondate,TRANSACTIONNO,PARTICULARS,COALESCE(DEBITAMOUNT,0.00) as  DEBITAMOUNT,-COALESCE(CREDITAMOUNT,0.00) as CREDITAMOUNT,DESCRIPTION,split_part(REGEXP_REPLACE(TRANSACTIONNO,'[[:digit:]]','','g'),'/',1) as code FROM tbltranstotaltransactions WHERE transactiondate BETWEEN '" + FormatDate(fromDate) + "' AND '" + FormatDate(toDate) + "' and parentid = " + pAccountId + " ";
                    Query = Query + pQuery + " AND ( debitamount<>0 or creditamount<>0)) x join tabgenerateidmaster y on x.code=y.code) as D order by transactiondate,RECORDID )x order by RECORDID ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _ObjBank = new AccountReportsDTO();
                            _ObjBank.precordid = Convert.ToInt64(dr["RECORDID"]);
                            _ObjBank.pparentid = Convert.ToInt64(dr["parentid"]);
                            _ObjBank.paccountid = Convert.ToInt64(dr["accountid"]);
                            _ObjBank.pFormName = Convert.ToString(dr["formname"]);
                            _ObjBank.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString("dd/MM/yyyy");
                            _ObjBank.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                            _ObjBank.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                            _ObjBank.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                            _ObjBank.pdescription = Convert.ToString(dr["DESCRIPTION"]);
                            _ObjBank.ptransactionno = Convert.ToString(dr["TRANSACTIONNO"]);
                            _ObjBank.popeningbal = Convert.ToDouble(dr["BALANCE"]);
                            _ObjBank.pBalanceType = Convert.ToString(dr["balancetype"]);
                            lstcashbook.Add(_ObjBank);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstcashbook;
        }
        #endregion


        #region Day Book

        public async Task<List<DayBookDto>> getDaybook(string ConnectionString, string fromdate, string todate)
        {
            await Task.Run(() =>
            {
                lstdaybook = new List<DayBookDto>();
                try
                {
                    string accountid = string.Empty;
                    accountid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(accountid::varchar,',')accountid from tblmstaccounts where accountname in ('CASH ON HAND','CHEQUE ON HAND') and chracctype='2'"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(ptransactiondate,transactiondate)datdate, db.RAccname, db.ReceiptNo, db.DebitAmount, date(coalesce(db.transactiondate,tot.ptransactiondate))transactiondate, db.RParticulars, tot.PParticulars, tot.PaymentNo, date(coalesce(tot.ptransactiondate,db.transactiondate))ptransactiondate, tot.CREDITAMOUNT, tot.PAccname from(select tot.TRANSACTIONNO AS ReceiptNo, tot.PARTICULARS as RParticulars, tot.ACCOUNTNAME AS RAccname, tot.DEBITAMOUNT AS DebitAmount, tot.transactiondate, row_number() over(partition by transactiondate order by transactiondate) as seqnum from tbltranstotaltransactions tot where transactiondate between  '" + FormatDate(fromdate) + "' and  '" + FormatDate(todate) + "' and debitamount <> 0  and(accountid in ( " + accountid + " ) or parentid  in (select bankaccountid from tblmstbank)) )db full outer join( select tot.PARTICULARS as PParticulars,tot.ACCOUNTNAME as PAccname,tot.CREDITAMOUNT,tot.DESCRIPTION,tot.TRANSACTIONNO as PaymentNo,tot.transactiondate as ptransactiondate,row_number() over(partition by transactiondate order by transactiondate) as seqnum  from tbltranstotaltransactions tot where transactiondate  between  '" + FormatDate(fromdate) + "' and  '" + FormatDate(todate) + "'  and creditamount<>0 and(accountid in ( " + accountid + " ) or parentid  in (select bankaccountid from tblmstbank)) )tot on db.seqnum = tot.seqnum and transactiondate=ptransactiondate order by 1,db.seqnum; "))
                    {
                        while (dr.Read())
                        {
                            DayBookDto dayBookdto = new DayBookDto();
                            dayBookdto.prcptaccountname = dr["RAccname"] == DBNull.Value ? null : Convert.ToString(dr["RAccname"]);
                            dayBookdto.prcpttransactionno = dr["ReceiptNo"] == DBNull.Value ? null : Convert.ToString(dr["ReceiptNo"]);
                            dayBookdto.prcptdebitamount = dr["DebitAmount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["DebitAmount"]);
                            dayBookdto.prcpttransactiondate = dr["transactiondate"] == DBNull.Value ? null : Convert.ToDateTime(dr["transactiondate"]).ToString("dd/MM/yyyy");
                            dayBookdto.prcptparticulars = dr["RParticulars"] == DBNull.Value ? null : Convert.ToString(dr["RParticulars"]);
                            dayBookdto.ptransactionno = dr["PaymentNo"] == DBNull.Value ? null : Convert.ToString(dr["PaymentNo"]);
                            dayBookdto.ptransactiondate = dr["ptransactiondate"] == DBNull.Value ? null : Convert.ToDateTime(dr["ptransactiondate"]).ToString("dd/MM/yyyy");
                            dayBookdto.pcreditamount = dr["CREDITAMOUNT"] == DBNull.Value ? 0 : Convert.ToDouble(dr["CREDITAMOUNT"]);
                            dayBookdto.paccountname = dr["PAccname"] == DBNull.Value ? null : Convert.ToString(dr["PAccname"]);
                            dayBookdto.pparticulars = dr["PParticulars"] == DBNull.Value ? null : Convert.ToString(dr["PParticulars"]);
                            dayBookdto.pdatdate = dr["datdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["datdate"]).ToString("dd/MM/yyyy");

                            lstdaybook.Add(dayBookdto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstdaybook;
        }

        public async Task<List<DayBookDto>> getDaybookTotals(string ConnectionString, string fromdate, string todate)
        {
            await Task.Run(() =>
            {
                lstdaybook = new List<DayBookDto>();
                try
                {
                    string accountid = string.Empty;
                    accountid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(accountid::varchar,',')accountid from tblmstaccounts where accountname in ('CASH ON HAND','CHEQUE ON HAND') and chracctype='2'"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select accountname,SUM(openingbal)openingbal,SUM(debitamount)debitamount,SUM(creditamount)creditamount,SUM(openingbal+debitamount-creditamount)closingbal from (select accountname,sum(coalesce(debitamount,0)-coalesce(creditamount,0))Openingbal,0 AS debitamount, 0 AS creditamount from tbltranstotaltransactions where transactiondate < '" + FormatDate(fromdate) + "'  and (parentid in (" + accountid + " ) or  parentid  in (select bankaccountid from tblmstbank)) group by accountname UNION ALL select accountname,0 AS Openingbal,sum(debitamount)debitamount,sum(creditamount)creditamount from tbltranstotaltransactions where transactiondate between  '" + FormatDate(fromdate) + "' and  '" + FormatDate(todate) + "'   and (parentid in (" + accountid + " ) or parentid  in (select bankaccountid from tblmstbank)) group by accountname)t GROUP BY accountname"))
                    {
                        while (dr.Read())
                        {
                            DayBookDto dayBookdto = new DayBookDto();
                            dayBookdto.paccountname = Convert.ToString(dr["accountname"]);
                            dayBookdto.popeningbal = dr["openingbal"] == DBNull.Value ? 0 : Convert.ToDouble(dr["openingbal"]);
                            dayBookdto.pdebitamount = dr["debitamount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["debitamount"]);
                            dayBookdto.pcreditamount = dr["creditamount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["creditamount"]);
                            dayBookdto.pclosingbal = dr["closingbal"] == DBNull.Value ? 0 : Convert.ToDouble(dr["closingbal"]);
                            lstdaybook.Add(dayBookdto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstdaybook;
        }

        #endregion

        #region PartyLedger
        public async Task<List<AccountReportsDTO>> GetPartyLedgerDetails(string con, string fromDate, string toDate, long pAccountId, long pSubAccountId, string pPartyRefId)
        {
            string Query = string.Empty;
            string pQuery = string.Empty;
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    if (pSubAccountId > 0)
                    {
                        pQuery = "and parentid = " + pAccountId + " and accountid=" + pSubAccountId;
                    }
                    else if (pAccountId > 0)
                    {
                        pQuery = " and parentid = " + pAccountId;
                    }
                    Query = "select recordid,transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,contactname,DEBITAMOUNT,abs(CREDITAMOUNT)as CREDITAMOUNT,abs(balance) as balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from (select *,sum(DEBITAMOUNT+CREDITAMOUNT) OVER(ORDER BY TRANSACTIONDATE,RECORDID)as BALANCE from(SELECT 0 AS RECORDID,CAST('" + FormatDate(fromDate) + "' AS DATE) AS transactiondate,'0' AS TRANSACTIONNO,'Opening Balance' AS PARTICULARS,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)>0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)<0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END  AS CREDITAMOUNT,'' DESCRIPTION,'' contactname FROM tbltranstotaltransactions WHERE transactiondate < '" + FormatDate(fromDate) + "' ";
                    Query = Query + pQuery + " and contactid='" + pPartyRefId + "' UNION ALL SELECT row_number() over (order by transactiondate,TRANSACTIONNO) as recordid, transactiondate,TRANSACTIONNO,PARTICULARS,sum(COALESCE(DEBITAMOUNT,0.00)) as  DEBITAMOUNT,-sum(COALESCE(CREDITAMOUNT,0.00)) as CREDITAMOUNT,DESCRIPTION,contactname FROM tbltranstotaltransactions WHERE transactiondate BETWEEN '" + FormatDate(fromDate) + "' AND '" + FormatDate(toDate) + "' ";
                    Query = Query + pQuery + " and contactid='" + pPartyRefId + "' group by  transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,contactname ) as D  )x where ( debitamount<>0 or creditamount<>0); ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _ObjBank = new AccountReportsDTO();
                            _ObjBank.precordid = Convert.ToInt64(dr["RECORDID"]);
                            _ObjBank.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString("dd/MM/yyyy");
                            _ObjBank.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                            _ObjBank.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                            _ObjBank.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                            _ObjBank.pdescription = Convert.ToString(dr["DESCRIPTION"]);
                            _ObjBank.ptransactionno = Convert.ToString(dr["TRANSACTIONNO"]);
                            _ObjBank.popeningbal = Convert.ToDouble(dr["BALANCE"]);
                            _ObjBank.pBalanceType = Convert.ToString(dr["balancetype"]);
                            lstcashbook.Add(_ObjBank);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstcashbook;
        }
        #endregion

        #region BRS
        public async Task<List<BRSDto>> GetBrs(string con, string fromDate, long pBankAccountId)
        {
            string Query = string.Empty;
            await Task.Run(() =>
            {
                lstBRS = new List<BRSDto>();
                try
                {
                    Query = "select coalesce(sum( coalesce(debitamount,0)-coalesce(creditamount,0)),0) as bankbookbalance from tblmstbank t1 join tbltranstotaltransactions t2 on t1.bankaccountid=t2.parentid where t1.recordid=" + pBankAccountId + " group by t1.recordid;";
                    Int64 BankBookBalance = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, Query));

                    Query = "SELECT BS.recordid,cd.depositeddate as chequedate,CD.referencenumber,CD.PARTICULARS,CD.totalreceivedamount,CD.BANKNAME,CD.BRANCHname,'CHEQUES DEPOSITED BUT NOT CREDITED' groupType FROM tbltransreceiptreference CD JOIN tblmstbank BS ON CD.depositedbankid=BS.RECORDID  AND  CD.depositstatus='P' AND CD.CLEARSTATUS='N' AND depositeddate<='" + FormatDate(fromDate) + "' and BS.recordid=" + pBankAccountId + " UNION ALL SELECT P.BANKID,P.paymentDATE,P.CHEQUENUMBER,P.particulars,MC.totalpaidamount,P.BANKNAME,p.branchname,'CHEQUES ISSUED BUT NOT CLEARED' as groupType FROM tbltranspaymentreference AS P JOIN tbltranspaymentvoucher MC ON P.paymentid=MC.paymentid AND P.paymentDATE<='" + FormatDate(fromDate) + "' AND P.CLEARSTATUS='N' and bankid=" + pBankAccountId + ";";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            BRSDto _ObjBank = new BRSDto();
                            _ObjBank.precordid = Convert.ToInt64(dr["RECORDID"]);
                            _ObjBank.ptransactiondate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            _ObjBank.pChequeNumber = Convert.ToString(dr["referencenumber"]);
                            _ObjBank.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                            _ObjBank.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            _ObjBank.pBankName = Convert.ToString(dr["BANKNAME"]);
                            _ObjBank.pBranchName = Convert.ToString(dr["BRANCHname"]);
                            _ObjBank.pGroupType = Convert.ToString(dr["groupType"]);
                            _ObjBank.pBankBookBalance = BankBookBalance;
                            lstBRS.Add(_ObjBank);
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstBRS;
        }
        #endregion

        #region BRS1
        public async Task<List<BRSDto>> GetBrs1(string con, string fromDate, long pBankAccountId)
        {
            string Query = string.Empty;
            await Task.Run(() =>
            {
                lstBRS = new List<BRSDto>();
                try
                {
                    Query = "select coalesce(sum( coalesce(debitamount,0)-coalesce(creditamount,0)),0) as bankbookbalance from tblmstbank t1 join tbltranstotaltransactions t2 on t1.bankaccountid=t2.parentid where t1.recordid=" + pBankAccountId + " group by t1.recordid;";
                    Int64 BankBookBalance = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, Query));

                    Query = "SELECT BS.recordid,cd.chequedate,CD.referencenumber,CD.PARTICULARS,CD.totalreceivedamount,CD.BANKNAME,CD.BRANCHname,'CHEQUES DEPOSITED BUT NOT CREDITED' groupType FROM tbltransreceiptreference CD JOIN tblmstbank BS ON CD.depositedbankid=BS.RECORDID  AND  CD.depositstatus='P' AND CD.CLEARSTATUS='N' AND depositeddate<='" + FormatDate(fromDate) + "' and BS.recordid=" + pBankAccountId + " UNION ALL SELECT P.BANKID,P.paymentDATE,P.CHEQUENUMBER,P.particulars,MC.totalpaidamount,P.BANKNAME,p.branchname,'CHEQUES ISSUED BUT NOT CLEARED' as groupType FROM tbltranspaymentreference AS P JOIN tbltranspaymentvoucher MC ON P.paymentid=MC.paymentid AND P.paymentDATE<='" + FormatDate(fromDate) + "' AND P.CLEARSTATUS='N' and bankid=" + pBankAccountId + ";";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            BRSDto _ObjBank = new BRSDto();
                            _ObjBank.precordid = Convert.ToInt64(dr["RECORDID"]);
                            _ObjBank.ptransactiondate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            _ObjBank.pChequeNumber = Convert.ToString(dr["referencenumber"]);
                            _ObjBank.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                            _ObjBank.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            _ObjBank.pBankName = Convert.ToString(dr["BANKNAME"]);
                            _ObjBank.pBranchName = Convert.ToString(dr["BRANCHname"]);
                            _ObjBank.pGroupType = Convert.ToString(dr["groupType"]);
                            _ObjBank.pBankBookBalance = BankBookBalance;
                            lstBRS.Add(_ObjBank);
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstBRS;
        }
        #endregion

        #region Trialbalance
        public async Task<List<AccountReportsDTO>> GetTrialBalance(string con, string fromDate, string todate, string groupType)
        {
            string Query = string.Empty;
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    //Query = "select accountname,case when parentname='E' then 'EXPENSES' when parentname='A' then 'ASSETS' when parentname='L' then 'EQUITY AND LIABILITIES' when parentname='I' then 'INCOME' end as parentname,DEBITAMOUNT,CREDITAMOUNT,case when parentname='E' then 2 when parentname='A' then 3 when parentname='L' then 4 when parentname='I' then 1 end as sortorder from (SELECT parentaccountname as accountname,getaccounttype(parentid) as parentname,coalesce(sum(COALESCE(DEBITAMOUNT,0.00)),0) as  DEBITAMOUNT,-coalesce(sum(COALESCE(CREDITAMOUNT,0.00))) as CREDITAMOUNT FROM tbltranstotaltransactions WHERE case when upper('" + groupType + "')='ASON' then transactiondate<= '" + FormatDate(todate) + "' else transactiondate between '" + FormatDate(fromDate) + "' and '" + FormatDate(todate) + "' end group by parentaccountname,parentid order by parentaccountname )x where ( debitamount<>0 or creditamount<>0) order by sortorder,accountname;";

                    Query = "select accountname,accountid,case when parentname='E' then 'EXPENSES' when parentname='A' then 'ASSETS' when parentname='L' then 'EQUITY AND LIABILITIES' when parentname='I' then 'INCOME' end as parentname,DEBITAMOUNT,CREDITAMOUNT,case when parentname='E' then 2 when parentname='A' then 3 when parentname='L' then 4 when parentname='I' then 1 end as sortorder from (select accountname,accountid,parentname,case when balamt>=0 then abs(balamt) else 0 end as debitamount, case when balamt<0 then abs(balamt) else 0 end as creditamount from (SELECT parentaccountname as accountname,parentid as accountid,getaccounttype(parentid) as parentname,coalesce(sum(DEBITAMOUNT),0) as  DEBITAMOUNT,-coalesce(sum(CREDITAMOUNT)) as CREDITAMOUNT,coalesce(sum(DEBITAMOUNT-CREDITAMOUNT),0) as balamt FROM tbltranstotaltransactions WHERE case when upper('" + groupType + "')='ASON' then transactiondate<= '" + FormatDate(todate) + "' else transactiondate between '" + FormatDate(fromDate) + "' and '" + FormatDate(todate) + "' end group by parentaccountname,parentid) t   order by accountname)x where ( debitamount<>0 or creditamount<>0) order by sortorder,accountname;";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _ObjTrialBalance = new AccountReportsDTO();
                            _ObjTrialBalance.precordid = Convert.ToInt64(dr["sortorder"]);
                            _ObjTrialBalance.paccountid = Convert.ToInt64(dr["accountid"]);
                            _ObjTrialBalance.paccountname = Convert.ToString(dr["accountname"]);
                            _ObjTrialBalance.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                            _ObjTrialBalance.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                            _ObjTrialBalance.pparentname = Convert.ToString(dr["parentname"]);
                            lstcashbook.Add(_ObjTrialBalance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstcashbook;
        }
        #endregion

        #region ledgerSummary
        public async Task<List<AccountReportsDTO>> GetLedgerSummary(string con, string fromDate, string todate, string pAccountId)
        {
            string Query = string.Empty;
            await Task.Run(() =>
            {
                lstcashbook = new List<AccountReportsDTO>();
                try
                {
                    if (fromDate == todate)
                    {
                        Query = "SELECT coalesce(x.parentaccountname,a.parentaccountname) as parentaccountname,coalesce(x.parentid,a.parentid) as parentid,coalesce(x.ACCOUNTNAME,a.ACCOUNTNAME) as ACCOUNTNAME,coalesce(x.accountid,a.accountid) as accountid,coalesce(OPENINGBALANCE,0) OPENINGBALANCE,coalesce(DEBITAMOUNT,0) as DEBITAMOUNT,coalesce(CREDITAMOUNT,0) as CREDITAMOUNT,0 as balance FROM (SELECT tt.parentaccountname,parentid,TT.ACCOUNTNAME,accountid,SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT,0)) AS OPENINGBALANCE FROM tbltranstotaltransactions TT  WHERE TT.transactiondate<'" + FormatDate(fromDate) + "' AND parentid in(" + pAccountId + ") GROUP BY parentaccountname,ACCOUNTNAME,parentid,accountid ORDER BY ACCOUNTNAME) x FULL OUTER JOIN (SELECT tt.parentaccountname,parentid,TT.ACCOUNTNAME,accountid,CASE WHEN COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT,0)))>0 THEN abs(COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT,0)))) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT,0)))<0 THEN abs(COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT,0)))) ELSE 0 END  AS CREDITAMOUNT FROM tbltranstotaltransactions TT  WHERE TT.transactiondate <='" + FormatDate(fromDate) + "' AND parentid in(" + pAccountId + ") GROUP BY parentaccountname,ACCOUNTNAME,parentid,accountid ORDER BY ACCOUNTNAME)as a  on x.parentaccountname=a.parentaccountname and x.ACCOUNTNAME=a.ACCOUNTNAME order by ACCOUNTNAME;  ";
                    }
                    else
                    {
                        Query = "SELECT coalesce(x.parentaccountname,a.parentaccountname) as parentaccountname,coalesce(x.parentid,a.parentid) as parentid,coalesce(x.ACCOUNTNAME,a.ACCOUNTNAME) as ACCOUNTNAME,coalesce(x.accountid,a.accountid) as accountid,coalesce(OPENINGBALANCE,0) OPENINGBALANCE,coalesce(DEBITAMOUNT,0) as DEBITAMOUNT,coalesce(CREDITAMOUNT,0) as CREDITAMOUNT,coalesce(OPENINGBALANCE,0)+coalesce(DEBITAMOUNT,0)-coalesce(CREDITAMOUNT,0) as balance FROM(SELECT tt.parentaccountname,parentid, TT.ACCOUNTNAME,accountid, SUM(coalesce(TT.DEBITAMOUNT, 0)) - SUM(coalesce(TT.CREDITAMOUNT, 0)) AS OPENINGBALANCE FROM tbltranstotaltransactions TT  WHERE TT.transactiondate < '" + FormatDate(fromDate) + "' AND parentid in (" + pAccountId + ") GROUP BY parentaccountname, ACCOUNTNAME,parentid,accountid ORDER BY ACCOUNTNAME) x FULL OUTER JOIN(SELECT tt.parentaccountname,parentid, TT.ACCOUNTNAME,accountid, CASE WHEN COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT, 0)))> 0 THEN abs(COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT, 0)))) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(coalesce(TT.DEBITAMOUNT, 0)) - SUM(coalesce(TT.CREDITAMOUNT, 0))) < 0 THEN abs(COALESCE(SUM(coalesce(TT.DEBITAMOUNT,0))-SUM(coalesce(TT.CREDITAMOUNT, 0)))) ELSE 0 END AS CREDITAMOUNT FROM tbltranstotaltransactions TT  WHERE TT.transactiondate between '" + FormatDate(fromDate) + "' and '" + FormatDate(todate) + "' AND parentid in(" + pAccountId + ") GROUP BY parentaccountname,ACCOUNTNAME,parentid,accountid ORDER BY ACCOUNTNAME )as a  on x.parentaccountname = a.parentaccountname and x.ACCOUNTNAME = a.ACCOUNTNAME order by ACCOUNTNAME;";

                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _ObjTrialBalance = new AccountReportsDTO();
                            _ObjTrialBalance.pparentname = Convert.ToString(dr["parentaccountname"]);
                            _ObjTrialBalance.paccountname = Convert.ToString(dr["accountname"]);
                            _ObjTrialBalance.pparentid = Convert.ToInt64(dr["parentid"]);
                            _ObjTrialBalance.paccountid = Convert.ToInt64(dr["accountid"]);
                            _ObjTrialBalance.popeningbal = Convert.ToDouble(dr["OPENINGBALANCE"]);
                            _ObjTrialBalance.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                            _ObjTrialBalance.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                            _ObjTrialBalance.pclosingbal = Convert.ToDouble(dr["balance"]);
                            lstcashbook.Add(_ObjTrialBalance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstcashbook;
        }
        #endregion

        #region ComtarisionTb
        public async Task<List<ComparisionTb>> GetComparisionTB(string con, string fromDate, string todate)
        {
            string Query = string.Empty;
            await Task.Run(() =>
            {
                lstCompTb = new List<ComparisionTb>();
                try
                {
                    Query = "SELECT case when vchparentname1='I' then 'INCOME' WHEN vchparentname1='E' then 'EXPENSES' WHEN vchparentname1='A' then 'ASSETS' WHEN vchparentname1='L' then 'EQUITY AND LIABILITIES' end as parentaccountName,vchaccname1 as accountName,case when debit1 is null then 0.00 else round(debit1) end as debitamount1,case when credit1 is null then 0.00 else round(credit1) end as creditamount1,case when debit2 is null then 0.00 else round(debit2) end as debitamount2,case when credit2 is null then 0.00 else round(credit2) END as creditamount2,round(debittotal)debittotal,round(abs(credittotal))  credittotal FROM getcomptb('" + FormatDate(fromDate) + "','" + FormatDate(todate) + "')  where debit1!=0 or credit1!=0 or debit2!=0 or credit2!=0 or debittotal!=0 or credittotal!=0  ORDER BY sortorder1,vchparentname1,vchaccname1 ; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ComparisionTb _ObjCompTb = new ComparisionTb();
                            _ObjCompTb.pparentaccountName = Convert.ToString(dr["parentaccountName"]);
                            _ObjCompTb.paccountName = Convert.ToString(dr["accountName"]);
                            if (!string.IsNullOrEmpty(dr["debitamount1"].ToString()))
                            {
                                _ObjCompTb.pdebitamount1 = Convert.ToDecimal(dr["debitamount1"]);
                            }
                            if (!string.IsNullOrEmpty(dr["creditamount1"].ToString()))
                            {
                                _ObjCompTb.pcreditamount1 = Convert.ToDecimal(dr["creditamount1"]);
                            }
                            if (!string.IsNullOrEmpty(dr["debitamount2"].ToString()))
                            {
                                _ObjCompTb.pdebitamount2 = Convert.ToDecimal(dr["debitamount2"]);
                            }
                            if (!string.IsNullOrEmpty(dr["creditamount2"].ToString()))
                            {
                                _ObjCompTb.pcreditamount2 = Convert.ToDecimal(dr["creditamount2"]);
                            }
                            if (!string.IsNullOrEmpty(dr["debittotal"].ToString()))
                            {
                                _ObjCompTb.pdebittotal = Convert.ToDecimal(dr["debittotal"]);
                            }
                            if (!string.IsNullOrEmpty(dr["credittotal"].ToString()))
                            {
                                _ObjCompTb.pcredittotal = Convert.ToDecimal(dr["credittotal"]);
                            }
                            lstCompTb.Add(_ObjCompTb);
                        }


                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstCompTb;
        }
        #endregion

        #region Profit And Loss
        public async Task<List<ProfitAndLossDTO>> GetProfitAndLossData(string ConnectionString, string fromdate, string todate, string groupType)
        {
            await Task.Run(() =>
            {
                lstProfitAndLoss = new List<ProfitAndLossDTO>();
                try
                {

                    //string query = "select accountid,accountname,(case when amount<0 then '('||abs(amount)||')' else amount::text end)as amount,parentid,chracctype,(case when parentid is null then 'text-danger'  when chracctype::int= 1 then 'text-primary' " +
                    //"when chracctype::int= 2 then 'text-success' end)colorcode from (SELECT t.accountid,t.accountname,(WITH RECURSIVE cte AS" +
                    //"(SELECT tblmstaccounts.accountid AS parentid, tblmstaccounts.accountname, balamount, 1 AS LEVEL FROM tblmstaccounts " +
                    //"left join(select accountid, sum(debitamount - creditamount) as balamount from tbltranstotaltransactions where " +
                    //" case when upper('" + groupType + "')='ASON' then transactiondate<= '" + FormatDate(fromdate) + "' else transactiondate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' end group by accountid)  tt on tt.accountid = tblmstaccounts.accountid WHERE " +
                    //"tblmstaccounts.accountid = t.accountid UNION ALL SELECT p.accountid, p.accountname, tt.balamount, 2 AS LEVEL FROM cte cte_1" +
                    //" JOIN tblmstaccounts p USING(parentid) left join(select accountid, sum(debitamount - creditamount) as balamount from" +
                    //" tbltranstotaltransactions where case when upper('" + groupType + "')='ASON' then transactiondate<= '" + FormatDate(fromdate) + "' else transactiondate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "' end group by accountid) tt on tt.accountid = p.accountid)" +
                    //"SELECT COALESCE(sum(cte.balamount),0) AS balance FROM cte) AS Amount, t.parentid,t.chracctype FROM tblmstaccounts" +
                    //" t where t.acctype in('I', 'E') AND t.chracctype not in('2','3') ORDER BY t.accountid)tt where amount <> 0";
                    string query = "select parentid1 as parentid,accid as accountid,accname as accountname,balanceamount as amount,(case when level=1 then 'text-danger'" +
                    " when level=2 then 'text-primary' when level=3 then 'text-success' end) as colorcode,level from ( WITH RECURSIVE cte AS (select *,1 as level from" +
                    " getprofitandloss('" + FormatDate(fromdate) + "','" + FormatDate(todate) + "','" + groupType + "') where parentid1 is null UNION ALL   select t.accid,t.accname,t.balanceamount,t.parentid1,t.chracctype1," +
                    "c.level + 1 from  cte c JOIN getprofitandloss('" + FormatDate(fromdate) + "','" + FormatDate(todate) + "','" + groupType + "') t ON t.parentid1 = c.accid) SELECT * FROM cte ) as d";


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {

                            ProfitAndLossDTO profitAndLossDTO = new ProfitAndLossDTO();
                            profitAndLossDTO.pParentid = Convert.ToString(dr["parentid"]) == "" ? (long?)null : Convert.ToInt64(dr["parentid"]);
                            profitAndLossDTO.pAccountid = Convert.ToInt64(dr["accountid"]);
                            profitAndLossDTO.pAccountname = Convert.ToString(dr["accountname"]);
                            profitAndLossDTO.pAmount = Convert.ToString(dr["amount"]);
                            profitAndLossDTO.pColorcode = Convert.ToString(dr["colorcode"]);
                            profitAndLossDTO.pLevel = Convert.ToInt64(dr["level"]);

                            lstProfitAndLoss.Add(profitAndLossDTO);

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstProfitAndLoss;
        }
        #endregion

        #region BalanceSheet
        public async Task<List<BalanceSheetDTO>> GetBalanceSheetDetails(string ConnectionString, string fromdate)
        {
            await Task.Run(() =>
            {
                lstBalanceSheet = new List<BalanceSheetDTO>();
                try
                {
                    //string strQuery = "select  accountid,accountname,(case when amount<0 then '('||abs(amount)||')' else amount::text end)as amount,parentid,chracctype,(case when parentid is null then 'text-danger'  when chracctype::int= 1 then 'text-primary' when chracctype::int= 2 then 'text-success' end)colorcode from (SELECT t.accountid,t.accountname,(WITH RECURSIVE cte AS (SELECT tblmstaccounts.accountid AS parentid,tblmstaccounts.accountname,balamount,1 AS LEVEL FROM tblmstaccounts left join (select accountid,sum(debitamount-creditamount)as balamount from tbltranstotaltransactions where transactiondate<='" + FormatDate(fromdate) + "' group by accountid)  tt on tt.accountid = tblmstaccounts.accountid WHERE tblmstaccounts.accountid = t.accountid  UNION ALL SELECT p.accountid,p.accountname,tt.balamount,2 AS LEVEL FROM cte cte_1 JOIN tblmstaccounts p USING(parentid) left join (select accountid,sum(debitamount-creditamount)as balamount from tbltranstotaltransactions where transactiondate<='" + FormatDate(fromdate) + "' group by accountid) tt on tt.accountid = p.accountid )SELECT COALESCE(sum(cte.balamount),0) AS balance FROM cte) AS Amount, t.parentid,t.chracctype FROM tblmstaccounts t where acctype in ('A','L') AND chracctype not in ('3')  ORDER BY t.accountid)tt where amount<>0";
                    //string strQuery = "select *,(case when parentid1 is null then 'text-danger'  when chracctype1::int= 1 then 'text-primary' when chracctype1::int= 2 then 'text-success' end)colorcode from fn_balanceprofitandloss('" + FormatDate(fromdate) + "')  order by 1;";
                    string strQuery = "select accountid1,accountname1,(case when amount1<0 then '('||abs(round(amount1))||')' else round(amount1)::text end)as amount1," +
                    "parentid1,chracctype1,acctype1,levels,(case when levels=1 then 'text-danger' when levels=2 then 'text-primary' when levels=3 then 'text-success'" +
                    "  when levels=4 then 'text-body'end) as colorcode from fn_balanceprofitandloss('" + FormatDate(fromdate) + "')";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            BalanceSheetDTO _BalanceSheetDTO = new BalanceSheetDTO();
                            _BalanceSheetDTO.pAccountid = Convert.ToInt64(dr["accountid1"]);
                            _BalanceSheetDTO.pParentid = Convert.ToString(dr["parentid1"]) == "" ? (long?)null : Convert.ToInt64(dr["parentid1"]);
                            _BalanceSheetDTO.pAccountname = Convert.ToString(dr["accountname1"]);
                            _BalanceSheetDTO.pAmount = Convert.ToString(dr["amount1"]);
                            _BalanceSheetDTO.pColorcode = Convert.ToString(dr["colorcode"]);
                            _BalanceSheetDTO.pLevel = Convert.ToInt64(dr["levels"]);
                            lstBalanceSheet.Add(_BalanceSheetDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstBalanceSheet;
        }
        #endregion

        #region Reprint
        public async Task<long> GetReprintCount(string ConnectionString, string receiptno, long recordid)
        {
            long count = 0;
            string tblname = string.Empty;
            string strQuery = string.Empty;
            string colname = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    tblname = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select tablename from tblmstreprint where recordid=" + recordid));
                    if (tblname.Contains("receipt"))
                    {
                        colname = "receiptid";
                    }
                    else if (tblname.Contains("payment"))
                    {
                        colname = "paymentid";
                    }
                    else if (tblname.Contains("journal"))
                    {
                        colname = "jvnumber";
                    }

                    strQuery = "select count(1) from " + tblname + " where " + colname + "='" + receiptno.Trim().ToUpper() + "';";

                    count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, strQuery));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return count;
        }
        public async Task<List<AccountReportsDTO>> GetReprintBindDetails(string ConnectionString)
        {
            await Task.Run(() =>
            {
                lstreportdtl = new List<AccountReportsDTO>();
                try
                {
                    string strQuery = "selecT recordid,formname from tblmstreprint";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                    {
                        while (dr.Read())
                        {
                            AccountReportsDTO _reportdto = new AccountReportsDTO();
                            _reportdto.precordid = Convert.ToInt64(dr["recordid"]);
                            _reportdto.paccountname = Convert.ToString(dr["formname"]);
                            lstreportdtl.Add(_reportdto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstreportdtl;
        }
        #endregion
        #region GetRecerenceNo
        public async Task<string> GetReferenceNo(string con, string formName, string transactionNo)
        {
            string TransReferenceNo = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    if (formName == "CHEQUES IN BANK")
                    {
                        TransReferenceNo = Convert.ToString(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select receiptid from tbltransgenchequecleared  where transactionno ='" + transactionNo.ToUpper() + "';"));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return TransReferenceNo;
        }
        #endregion
        #region ChequesEnquiry
        public async Task<List<ChequeEnquiryDTO>> GetReceivedChequesData(string con, string pChequeNo)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select referencenumber,particulars,receiptid,totalreceivedamount from tbltransreceiptreference tcdr  where referencenumber like '" + pChequeNo + "%';";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["referencenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["receiptid"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }

        public async Task<List<ChequeEnquiryDTO>> GetReceivedChequesDetails(string con, string pChequeNo)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select  referencenumber,particulars,receiptid,(select receiptdate from tbltransgeneralreceipt where receiptid=tcdr.receiptid) chequedate,bankname,depositeddate,cleardate,totalreceivedamount,case  when clearstatus ='R' then 'Returned' when (depositstatus ='P' and clearstatus='N') then 'Passed' when depositstatus ='C' then 'Cancelled' when (depositstatus='P' and clearstatus='Y') then 'Cleared' else 'Not-Deposited' end as chequesstatus,(select case when coalesce(accountnumber,'')='' then bankname else bankname ||'-'|| accountnumber end as depositbankname from tblmstbank  where recordid =tcdr.depositedbankid) as depositbankname from tbltransreceiptreference tcdr  where referencenumber like '" + pChequeNo + "%'   order by referencenumber; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["referencenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["receiptid"]);
                            if (!string.IsNullOrEmpty(dr["chequedate"].ToString()))
                            {
                                _Obj.pchequedate = Convert.ToDateTime(dr["chequedate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["depositeddate"].ToString()))
                            {
                                _Obj.pdepositeddate = Convert.ToDateTime(dr["depositeddate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _Obj.pcleardate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            _Obj.pchequesstatus = Convert.ToString(dr["chequesstatus"]);
                            _Obj.pdepositbankname = Convert.ToString(dr["depositbankname"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }
        public async Task<List<ChequeEnquiryDTO>> GetIssuedChequesData(string con, string pChequeNo)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select chequenumber,particulars,paymentid,paidamount from tbltranspaymentreference  where chequenumber like '" + pChequeNo + "%';";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["chequenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["paymentid"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["paidamount"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }
        public async Task<List<ChequeEnquiryDTO>> GetIssuedChequesDetails(string con, string pChequeNo)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select  chequenumber,particulars,paymentid,paymentdate,bankname,cleardate,paidamount,case  when clearstatus ='R' then 'Returned' when clearstatus ='C' then 'Cancelled' when  clearstatus='P' then 'Cleared' else 'Not-Cleared' end as chequesstatus from tbltranspaymentreference tcdr  where chequenumber like '" + pChequeNo + "%'    order by chequenumber; ";


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["chequenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["paymentid"]);
                            if (!string.IsNullOrEmpty(dr["paymentdate"].ToString()))
                            {
                                _Obj.pchequedate = Convert.ToDateTime(dr["paymentdate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _Obj.pcleardate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["paidamount"]);
                            _Obj.pchequesstatus = Convert.ToString(dr["chequesstatus"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }
        #endregion

        #region JVList
        public async Task<List<JvListDTO>> GetJvListDetails(string con, string fromDate, string toDate, string modeOfTransaction)
        {
            await Task.Run(() =>
            {
                lstJvList = new List<JvListDTO>();
                string strQuery = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        if (modeOfTransaction.ToUpper() != "ALL")
                        {
                            strQuery = " upper(COALESCE(modeoftransaction,'AUTO')) = '" + modeOfTransaction.ToUpper() + "' and ";
                        }

                        string Query = "select transactionno,transactiondate,particulars,description,debitamount,creditamount from tbltranstotaltransactions t where exists (select 1 from tbltransjournalvoucher where " + strQuery + " jvnumber=t.transactionno) and transactiondate between '" + FormatDate(fromDate) + "' and '" + FormatDate(toDate) + "' order by transactiondate ;";
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                        {
                            while (dr.Read())
                            {
                                JvListDTO _Obj = new JvListDTO();
                                _Obj.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString();
                                _Obj.ptransactionno = Convert.ToString(dr["transactionno"]);
                                _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                                _Obj.pcreditamount = Convert.ToDouble(dr["creditamount"]);
                                _Obj.pdebitamount = Convert.ToDouble(dr["debitamount"]);
                                _Obj.pdescription = Convert.ToString(dr["description"]);
                                lstJvList.Add(_Obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstJvList;
        }
        #endregion
        #region Cheque Cancel
        public async Task<List<ChequeEnquiryDTO>> GetChequeCancelDetails(string con, string fromdate, string todate)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select depositeddate,referencenumber,totalreceivedamount,bankname,receiptid,particulars,(select receiptdate from tbltransgeneralreceipt where receiptid=tcdr.receiptid) as receiptdate from tbltransreceiptreference tcdr where depositstatus='C' and depositeddate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["referencenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["receiptid"]);
                            if (!string.IsNullOrEmpty(dr["depositeddate"].ToString()))
                            {
                                _Obj.pdepositeddate = Convert.ToDateTime(dr["depositeddate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _Obj.pchequedate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }
        #endregion

        #region Cheque Return
        public async Task<List<ChequeEnquiryDTO>> GetChequeReturnDetails(string con, string fromdate, string todate)
        {
            await Task.Run(() =>
            {
                lstChequeEnquiry = new List<ChequeEnquiryDTO>();
                try
                {
                    string Query = "select cleardate,referencenumber,totalreceivedamount,bankname,receiptid,particulars,(select receiptdate from tbltransgeneralreceipt where receiptid=tcdr.receiptid) as receiptdate from tbltransreceiptreference tcdr where clearstatus='R' and cleardate between '" + FormatDate(fromdate) + "' and '" + FormatDate(todate) + "'; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ChequeEnquiryDTO _Obj = new ChequeEnquiryDTO();
                            _Obj.preferencenumber = Convert.ToString(dr["referencenumber"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            _Obj.preceiptid = Convert.ToString(dr["receiptid"]);
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _Obj.pcleardate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["receiptdate"].ToString()))
                            {
                                _Obj.pchequedate = Convert.ToDateTime(dr["receiptdate"]).ToString("dd/MM/yyyy");
                            }
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            _Obj.ptotalreceivedamount = Convert.ToInt64(dr["totalreceivedamount"]);
                            lstChequeEnquiry.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstChequeEnquiry;
        }
        #endregion
        #region Issued Cheque
        public async Task<List<IssuedChequeDTO>> GetIssuedChequeNumbers(string con, long bankId)
        {
            await Task.Run(() =>
            {
                lstIssuedCheque = new List<IssuedChequeDTO>();
                try
                {
                    string Query = "select distinct chqbookid,chequefrom,chequeto,(chequefrom ||'-'|| chequeto) as chqfromto from tblmstchequemanagement tc join tblmstbank ts on tc.bankid=ts.recordid where ts.recordid=" + bankId + " order by chequefrom; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            IssuedChequeDTO _Obj = new IssuedChequeDTO();
                            _Obj.pchkBookId = Convert.ToInt64(dr["chqbookid"]);
                            _Obj.pchequeNoFrom = Convert.ToInt64(dr["chequefrom"]);
                            _Obj.pchequeNoTo = Convert.ToInt64(dr["chequeto"]);
                            _Obj.pchqfromto = Convert.ToString(dr["chqfromto"]);
                            lstIssuedCheque.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return lstIssuedCheque;
        }
        public async Task<List<IssuedChequeDTO>> GetIssuedChequeDetails(string con, long _BankId, long _ChqBookId, long _ChqFromNo, long _ChqToNo)
        {
            await Task.Run(() =>
            {
                lstIssuedCheque = new List<IssuedChequeDTO>();
                try
                {
                    string Query = "SELECT CD.chequenumber,CD.paymentid,CD.particulars,CD.paymentdate,CD.cleardate,tb.chqbookid,(CASE  WHEN CD.clearstatus='P'  THEN 'Cleared' WHEN CD.clearstatus='N'  THEN 'Not Cleared' WHEN CD.clearstatus='C'  THEN 'Cancelled' WHEN CD.clearstatus='R' THEN 'Returned'  end) as Status ,cd.paidamount,cd.bankname,'Used-Cheques' as ChequeStatus  FROM tbltranspaymentreference CD  JOIN tblmstchequemanagement TB ON CD.BANKID=TB.bankid AND TB.bankid=" + _BankId + " AND tb.chqbookid=" + _ChqBookId + " AND CAST(CD.chequenumber AS INTEGER)>=" + _ChqFromNo + " AND CAST(CD.chequenumber AS INTEGER)<=" + _ChqToNo + " union all select tc.chequenumber::text,''paymentid,'' particulars,null as paymentdate,tc.modifieddate::date as cleardate,tc.chqbookid,'' Status,0 as paidamount,tb.bankname,ts.statusname from tblmstcheques tc join tblmststatus ts on tc.statusid=ts.statusid join tblmstbank tb on tb.recordid=tc.bankid and ts.statusname in('UnUsed-Cheques','Cancelled') and TB.recordid=" + _BankId + " and tc.chqbookid=" + _ChqBookId + " and  tc.chequenumber>=" + _ChqFromNo + "  AND tc.chequenumber<=" + _ChqToNo + "; ";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            IssuedChequeDTO _Obj = new IssuedChequeDTO();
                            _Obj.pchequenumber = Convert.ToString(dr["chequenumber"]);
                            _Obj.ppaymentid = Convert.ToString(dr["paymentid"]);
                            _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                            if (!string.IsNullOrEmpty(dr["paymentdate"].ToString()))
                            {
                                _Obj.ppaymentdate = Convert.ToDateTime(dr["paymentdate"]).ToString("dd/MM/yyyy");
                            }
                            if (!string.IsNullOrEmpty(dr["cleardate"].ToString()))
                            {
                                _Obj.pcleardate = Convert.ToDateTime(dr["cleardate"]).ToString("dd/MM/yyyy");
                            }
                            _Obj.pchkBookId = Convert.ToInt64(dr["chqbookid"]);
                            _Obj.pstatus = Convert.ToString(dr["Status"]);
                            _Obj.ppaidamount = Convert.ToInt64(dr["paidamount"]);
                            _Obj.pbankname = Convert.ToString(dr["bankname"]);
                            _Obj.pchequestatus = Convert.ToString(dr["ChequeStatus"]);
                            lstIssuedCheque.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return lstIssuedCheque;
        }
        public async Task<bool> UnusedhequeCancel(string ConnectionString, IssuedChequeDTO issuedChequeDTO)
        {
            bool isSaved = false;
            StringBuilder Query = new StringBuilder();
            await Task.Run(() =>
            {
                try
                {
                    con = new NpgsqlConnection(ConnectionString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    if (issuedChequeDTO.lstIssuedCheque.Count > 0)
                    {
                        for (int i = 0; i < issuedChequeDTO.lstIssuedCheque.Count; i++)
                        {
                            Query.Append("update tblmstcheques set statusid =(select statusid from tblmststatus where statusname='Cancelled'),modifiedby=" + issuedChequeDTO.lstIssuedCheque[i].pCreatedby + ",modifieddate=current_timestamp where bankid=" + issuedChequeDTO.lstIssuedCheque[i].pbankaccountid + "  and chqbookid=" + issuedChequeDTO.lstIssuedCheque[i].pchkBookId + " and chequenumber=" + issuedChequeDTO.lstIssuedCheque[i].pchequenumber + "; ");
                        }
                    }
                    if (Query.Length > 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query.ToString());
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
            });

            return isSaved;
        }
        #endregion
        #region Ledger Extract
        public async Task<List<ledgerExtractDTO>> GetLedgerExtractDetails(string con, string fromDate, string toDate)
        {
            await Task.Run(() =>
            {
                lstLedgerExtract = new List<ledgerExtractDTO>();
                string strQuery = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        string Query = "select TRANSACTIONDATE,TRANSACTIONNO,ACCOUNTNAME,parentaccountname,PARTICULARS,NARRATION,debit,credit,abs(balance)as balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from( SELECT TRANSACTIONDATE,TRANSACTIONNO,T2.ACCOUNTNAME,T1.ACCOUNTID, T1.PARENTID,case when t2.chracctype='2' then T2.ACCOUNTNAME else T2.parentaccountname end as parentaccountname,t2.chracctype, PARTICULARS as PARTICULARS,DESCRIPTION AS NARRATION,COALESCE(DEBITAMOUNT,0) AS DEBIT, COALESCE(CREDITAMOUNT,0) AS CREDIT,ROW_NUMBER() OVER( PARTITION BY T1.ACCOUNTID,T1.PARENTID  ORDER BY TRANSACTIONDATE,T1.ACCOUNTID,T1.PARENTID) AS SID,T1.RECORDID,sum(debitamount-creditamount) over(partition by t1.parentid,t1.accountid order by transactiondate,t1.recordid) as balance FROM tbltranstotaltransactions  T1 JOIN tblmstaccounts T2  ON T1.accountid=T2.accountid WHERE TRANSACTIONDATE BETWEEN '" + FormatDate(fromDate) + "' AND '" + FormatDate(toDate) + "' ORDER BY parentaccountname,ACCOUNTID, SID,RECORDID)t ;";
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                        {
                            while (dr.Read())
                            {
                                ledgerExtractDTO _Obj = new ledgerExtractDTO();
                                _Obj.ptransactiondate = Convert.ToDateTime(dr["transactiondate"]).ToString("dd-MMM-yyyy");
                                _Obj.ptransactionno = Convert.ToString(dr["transactionno"]);
                                _Obj.paccountname = Convert.ToString(dr["ACCOUNTNAME"]);
                                _Obj.pparentname = Convert.ToString(dr["parentaccountname"]);
                                _Obj.pparticulars = Convert.ToString(dr["particulars"]);
                                _Obj.pdescription = Convert.ToString(dr["narration"]);
                                _Obj.pcreditamount = Convert.ToDouble(dr["credit"]);
                                _Obj.pdebitamount = Convert.ToDouble(dr["debit"]);
                                _Obj.pclosingbal = Convert.ToDouble(dr["balance"]);
                                _Obj.pBalanceType = Convert.ToString(dr["balancetype"]);
                                lstLedgerExtract.Add(_Obj);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstLedgerExtract;

        }

        #endregion

        #region Sub Account Ledger
        public async Task<List<subAccountLedgerDTO>> GetSubAccountLedgerDetails(string con)
        {
            await Task.Run(() =>
            {
                lstSubAccountLedger = new List<subAccountLedgerDTO>();
                string strQuery = string.Empty;
                try
                {
                    string Query = "SELECT distinct accountname FROM tblmstaccounts  WHERE chracctype = '3' and statusid != 2 ORDER BY accountname ;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            subAccountLedgerDTO _Obj = new subAccountLedgerDTO();
                            _Obj.paccountname = Convert.ToString(dr["accountname"]);

                            lstSubAccountLedger.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstSubAccountLedger;
        }
        public async Task<List<subAccountLedgerDTO>> GetAccountLedgerData(string con, string SubLedgerName)
        {
            await Task.Run(() =>
            {
                lstSubAccountLedger = new List<subAccountLedgerDTO>();
                string strQuery = string.Empty;
                try
                {
                    string Query = "select parentid ,parentaccountname ,accountbalance from tblmstaccounts where chracctype = '3' and statusid != 2 and accountname ='" + SubLedgerName + "' order by parentaccountname ;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            subAccountLedgerDTO _Obj = new subAccountLedgerDTO();
                            _Obj.pparentname = Convert.ToString(dr["parentaccountname"]);
                            _Obj.pparentid = Convert.ToInt64(dr["parentid"]);

                            lstSubAccountLedger.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstSubAccountLedger;
        }
        public async Task<List<subAccountLedgerDTO>> GetSubLedgerReportData(string con, string SubLedgerName, long parentid, string fromDate, string toDate)
        {
            await Task.Run(() =>
            {
                lstSubAccountLedger = new List<subAccountLedgerDTO>();
                string strQuery = string.Empty;
                string strSubQuery = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        if (parentid > 0)
                        {
                            strSubQuery = "  AND  t1.accountname ='" + ManageQuote(SubLedgerName) + "' and t1.parentid=" + parentid;
                        }
                        else
                        {
                            strSubQuery = "  AND  t1.accountname ='" + ManageQuote(SubLedgerName) + "'";
                        }
                        string Query = "select parentaccname,accountname,RECORDID,transactiondate,TRANSACTIONNO,PARTICULARS,DEBITAMOUNT, ABS(CREDITAMOUNT) as creditamount,DESCRIPTION,abs(balance)as balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from(select parentaccname,accountname,RECORDID,transactiondate,TRANSACTIONNO, PARTICULARS,DEBITAMOUNT, CREDITAMOUNT,DESCRIPTION,sum(DEBITAMOUNT+CREDITAMOUNT) OVER(PARTITION BY parentaccname ORDER BY parentaccname,transactiondate,RECORDID)as BALANCE from(SELECT t2.accountname as parentaccname,t1.accountname,0 AS RECORDID,CAST('" + FormatDate(fromDate) + "' AS DATE)-1 AS transactiondate,'0' AS TRANSACTIONNO,'Opening Balance' AS PARTICULARS,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)>0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END AS  DEBITAMOUNT,CASE WHEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0)<0 THEN COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) ELSE 0 END  AS CREDITAMOUNT,'' DESCRIPTION,COALESCE(SUM(DEBITAMOUNT)-SUM(CREDITAMOUNT),0) AS BALANCE  FROM tbltranstotaltransactions t1 join tblmstaccounts t2 on t1.parentid =t2.accountid WHERE transactiondate < '" + FormatDate(fromDate) + "' " + strSubQuery + " group by t2.accountname,t1.accountname UNION ALL SELECT t2.accountname,t1.accountname, t1.RECORDID, transactiondate,TRANSACTIONNO,PARTICULARS,COALESCE(DEBITAMOUNT,0.00) as  DEBITAMOUNT,-COALESCE(CREDITAMOUNT,0.00) as CREDITAMOUNT,DESCRIPTION,0 BALANCE FROM tbltranstotaltransactions t1 join tblmstaccounts t2 on t1.parentid =t2.accountid WHERE transactiondate BETWEEN '" + FormatDate(fromDate) + "' AND '" + FormatDate(toDate) + "'" + strSubQuery + " AND ( debitamount<>0 or creditamount<>0))as D  order by parentaccname,accountname, transactiondate, RECORDID, TRANSACTIONNO)E; ";

                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                        {
                            while (dr.Read())
                            {
                                subAccountLedgerDTO _Obj = new subAccountLedgerDTO();
                                _Obj.pparentname = Convert.ToString(dr["parentaccname"]);
                                _Obj.paccountname = Convert.ToString(dr["accountname"]);
                                _Obj.ptransactiondate = Convert.ToString(dr["transactiondate"]);
                                _Obj.ptransactionno = Convert.ToString(dr["TRANSACTIONNO"]);
                                _Obj.pparticulars = Convert.ToString(dr["PARTICULARS"]);
                                _Obj.pdescription = Convert.ToString(dr["DESCRIPTION"]);
                                _Obj.pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]);
                                _Obj.pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]);
                                _Obj.pclosingbal = Convert.ToDouble(dr["BALANCE"]);
                                _Obj.pBalanceType = Convert.ToString(dr["balancetype"]);

                                lstSubAccountLedger.Add(_Obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstSubAccountLedger;
        }
        #endregion

        #region Sub Ledger Summary
        public async Task<List<subLedgerSummaryDTO>> GetMainAccountHeads(string con)
        {
            await Task.Run(() =>
            {
                lstSubLedgerSummary = new List<subLedgerSummaryDTO>();
                string strQuery = string.Empty;
                try
                {
                    string Query = "select accountname,accountid,acctype From tblmstaccounts where parentid is null order by recordid;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            subLedgerSummaryDTO _Obj = new subLedgerSummaryDTO();
                            _Obj.paccountname = Convert.ToString(dr["accountname"]);
                            _Obj.paccountid = Convert.ToInt64(dr["accountid"]);
                            _Obj.pAccountType = Convert.ToString(dr["acctype"]);
                            lstSubLedgerSummary.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstSubLedgerSummary;
        }
        public async Task<List<subLedgerSummaryDTO>> GetSubLedgerAccountNames(string con,string acctype)
        {
            await Task.Run(() =>
            {
                lstSubLedgerSummary = new List<subLedgerSummaryDTO>();
                string strQuery = string.Empty;
                try
                {
                    string Query = "with recursive rel_tree as (select * from (select accountid, accountname::text, parentid,acctype,chracctype,accountbalance, 1 as level, array[accountid] as path_info from tblmstaccounts where parentid is null )t union all select c.accountid, /*rpad('_', p.level * 2) ||*/ c.accountname, c.parentid,c.acctype, c.chracctype,c.accountbalance, p.level + 1,p.path_info||c.accountid from tblmstaccounts c join rel_tree p on c.parentid = p.accountid )select accountid, accountname,accountbalance,chracctype from rel_tree where acctype in('" + acctype + "') and chracctype in ('2') order by path_info;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            subLedgerSummaryDTO _Obj = new subLedgerSummaryDTO();
                            _Obj.paccountname = Convert.ToString(dr["accountname"]);
                            _Obj.paccountid = Convert.ToInt64(dr["accountid"]);
                            
                            lstSubLedgerSummary.Add(_Obj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstSubLedgerSummary;
        }
        public async Task<DataSet> GetSubLedgerSummaryReportData(string con, long mainAccountid, string parentids, string fromDate, string toDate)
        {
            DataSet ds = new DataSet();
            await Task.Run(() =>
            {
                lstSubLedgerSummary = new List<subLedgerSummaryDTO>();

                string SubQuery = string.Empty;
                string reportType = "SUMMARY";
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate) && mainAccountid > 0 && !string.IsNullOrEmpty(parentids))
                    {
                        string Query = " fnsubledgerdatasummerymonths(" + mainAccountid + ",'" + FormatDate(fromDate) + "','" + FormatDate(toDate) + "','" + parentids + "','" + reportType + "'); ";
                         
                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, Query, null);
                        SubQuery = "select * from TEMPTABSUBLEDGERDATA order by  ACCOUNTNAME;select column_name as field ,case when column_name='accountname' then 'Particulars' else initcap(column_name) end as title,case when data_type='character varying' then 'text' else data_type end as type,case when data_type='date' then '{0:d}' when data_type= 'numeric' then '{0:n2}'else 'string' end as format FROM information_schema.columns where table_schema='public' and table_name='temptabsubledgerdata';";
                        using (ds = NPGSqlHelper.ExecuteDataset(con, CommandType.Text, SubQuery))
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return ds;
        }
        public async Task<DataSet> GetMonthlyComparisionReportData(string con, long mainAccountid, string parentids, string fromDate, string toDate)
        {
            DataSet ds = new DataSet();
            await Task.Run(() =>
            {
                lstSubLedgerSummary = new List<subLedgerSummaryDTO>();
                string SubQuery = string.Empty;
                string reportType = "MONTHS";
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate) && mainAccountid > 0 && !string.IsNullOrEmpty(parentids))
                    {
                        string Query = " fnsubledgerdatasummerymonths(" + mainAccountid + ",'" + FormatDate(fromDate) + "','" + FormatDate(toDate) + "','" + parentids + "','" + reportType + "'); ";

                        NPGSqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, Query, null);
                        SubQuery = "select * from TEMPTABSUBLEDGERDATA order by accountname; select column_name as field ,case when column_name='accountname' then 'Particulars' else initcap(column_name) end as title,case when data_type='character varying' then 'text' else data_type end as type,case when data_type='date' then '{0:d}' when data_type= 'numeric' then '{0:n2}'else 'string' end as format FROM information_schema.columns where table_schema='public' and table_name='temptabsubledgerdata' ;";
                        using (ds = NPGSqlHelper.ExecuteDataset(con, CommandType.Text, SubQuery))
                        {
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return ds;
        }

        #endregion
        #region MTDYTD Profit and Loss
        public async Task<MtdytdpandlDTO> GetMTDYTDPANDL(string con, string ReportType, string fromDate,string[] reportSubType1)
        {
            MtdytdpandlDTO objmypl = new MtdytdpandlDTO();
            objmypl.plstMtdytdpandl = new List<MtdytdpandlDetailsDTO>();
            objmypl.plstMtdytdpandldetails = new List<MtdytdpandlDetailsDTO>();
            await Task.Run(() =>
            {
                string strQuery = string.Empty;
                string Query = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ReportType))
                    {
                        if (ReportType.ToUpper() == "MONTH" || ReportType.ToUpper() == "MTD")
                        {
                            ReportType = "MTD";
                        }
                        else
                        {
                            ReportType = "YTD";
                        }
                        Int64 mnth = Convert.ToInt64(Convert.ToDateTime(FormatDate(fromDate)).ToString("yyyy"));
                        string mnth2 = Convert.ToDateTime(FormatDate(fromDate)).ToString("MMM");
                        if (mnth2 == "Jan" || mnth2 == "Feb" || mnth2 == "Mar")
                        {
                            mnth = mnth - 1;
                        }

                        Int64 mnth1 = mnth + 1;
                        foreach (string reportSubType in reportSubType1)
                        {
                            objmypl.plstmtdytdColumnslist = new List<MtdytdpandlDetailsDTO>();
                            if (reportSubType.Trim().ToUpper() == "SUMMARY")
                            {
                                Query = "select * from (SELECT vchlevel1,vchlevel2,vchlevel3,accountid1,accountname1,round(jan1) as jan1,round(feb1) as feb1,round(mar1) as mar1,round(apr1) as apr1,round(may1) as may1,round(jun1) as jun1,round(jul1) as jul1,round(aug1) as aug1,round(sep1) as sep1,round(oct1) as oct1,round(nov1) as nov1,round(dec1) as dec1 FROM GETMTDYTDPANDL('" + FormatDate(fromDate) + "','" + ReportType + "') t1 join vwchartofacciunttree t2 on t1.accountname1=t2.vchlevel4 where t2.vchlevel1='EXPENSES' or t2.vchlevel1='INCOME' order by  numlevel1,vchlevel2,vchlevel3,vchlevel4)x ;select *,case when title='Particulars' then 'paccountname' else 'p'||initcap(substring(title,1,3)) end as field from (select 'Particulars' as title,'text' as type,'string' as format union all select to_char(generate_series('" + mnth + "-04-01'::date,'" + mnth1 + "-03-31'::date,'1 MONTH')::date,'MON-YYYY')as title,'numeric' as type ,'{0:n2}' as format)t;";
                            }
                            else
                            {
                                Query = "select* from(SELECT vchlevel1 vchlevel0, vchlevel2 vchlevel1, vchlevel3 vchlevel2, PARENTACCOUNTNAME1 vchlevel3, accountid1, accountname1, round(jan1) as jan1,round(feb1) as feb1,round(mar1) as mar1,round(apr1) as apr1,round(may1) as may1,round(jun1) as jun1,round(jul1) as jul1,round(aug1) as aug1,round(sep1) as sep1,round(oct1) as oct1,round(nov1) as nov1,round(dec1) as dec1 FROM getmtdytdpandldetails('" + FormatDate(fromDate) + "','" + ReportType + "') t1 join vwchartofacciunttree t2 on t1.PARENTACCOUNTNAME1 = t2.vchlevel4 where t2.vchlevel1 = 'EXPENSES' or t2.vchlevel1 = 'INCOME' order by 1 desc,2,3,4)x ;select *,case when title='Particulars' then 'paccountname' else 'p'||initcap(substring(title,1,3)) end as field from (select 'Particulars' as title,'text' as type,'string' as format union all select to_char(generate_series('" + mnth + "-04-01'::date,'" + mnth1 + "-03-31'::date,'1 MONTH')::date,'MON-YYYY')as title,'numeric' as type ,'{0:n2}' as format)t;";
                            }
                            DataSet ds = NPGSqlHelper.ExecuteDataset(con, CommandType.Text, Query);

                            DataTable dt = ds.Tables[1];
                            //for dynamic columns list generation
                            foreach (DataRow dr in dt.Rows)
                            {
                                MtdytdpandlDetailsDTO _Obj = new MtdytdpandlDetailsDTO();
                                _Obj.pfield = Convert.ToString(dr["field"]);
                                _Obj.ptitle = Convert.ToString(dr["title"]);
                                _Obj.pformat = Convert.ToString(dr["format"]);
                                _Obj.ptype = Convert.ToString(dr["type"]);
                                objmypl.plstmtdytdColumnslist.Add(_Obj);
                            }

                            dt = ds.Tables[0];
                            //for griddata
                            foreach (DataRow dr in dt.Rows)
                            {
                                MtdytdpandlDetailsDTO _Obj = new MtdytdpandlDetailsDTO();
                                
                                if (reportSubType.Trim().ToUpper() != "SUMMARY")
                                {
                                    _Obj.plevel0 = Convert.ToString(dr["vchlevel0"]);
                                }
                                _Obj.plevel1 = Convert.ToString(dr["vchlevel1"]);
                                _Obj.plevel2 = Convert.ToString(dr["vchlevel2"]);
                                _Obj.plevel3 = Convert.ToString(dr["vchlevel3"]);
                                _Obj.paccountname = Convert.ToString(dr["accountname1"]);
                                if (_Obj.plevel1 == "INCOME" || _Obj.plevel0 == "INCOME")
                                {
                                    _Obj.pJan = -(Convert.ToDouble(dr["jan1"]));
                                    _Obj.pJan = -(Convert.ToDouble(dr["jan1"]));
                                    _Obj.pFeb = -(Convert.ToDouble(dr["feb1"]));
                                    _Obj.pMar = -(Convert.ToDouble(dr["mar1"]));
                                    _Obj.pApr = -(Convert.ToDouble(dr["apr1"]));
                                    _Obj.pMay = -(Convert.ToDouble(dr["may1"]));
                                    _Obj.pJun = -(Convert.ToDouble(dr["jun1"]));
                                    _Obj.pJul = -(Convert.ToDouble(dr["jul1"]));
                                    _Obj.pAug = -(Convert.ToDouble(dr["aug1"]));
                                    _Obj.pSep = -(Convert.ToDouble(dr["sep1"]));
                                    _Obj.pOct = -(Convert.ToDouble(dr["oct1"]));
                                    _Obj.pNov = -(Convert.ToDouble(dr["nov1"]));
                                    _Obj.pDec = -(Convert.ToDouble(dr["dec1"]));
                                }
                                else
                                {
                                    _Obj.pJan = Convert.ToDouble(dr["jan1"]);
                                    _Obj.pFeb = Convert.ToDouble(dr["feb1"]);
                                    _Obj.pMar = Convert.ToDouble(dr["mar1"]);
                                    _Obj.pApr = Convert.ToDouble(dr["apr1"]);
                                    _Obj.pMay = Convert.ToDouble(dr["may1"]);
                                    _Obj.pJun = Convert.ToDouble(dr["jun1"]);
                                    _Obj.pJul = Convert.ToDouble(dr["jul1"]);
                                    _Obj.pAug = Convert.ToDouble(dr["aug1"]);
                                    _Obj.pSep = Convert.ToDouble(dr["sep1"]);
                                    _Obj.pOct = Convert.ToDouble(dr["oct1"]);
                                    _Obj.pNov = Convert.ToDouble(dr["nov1"]);
                                    _Obj.pDec = Convert.ToDouble(dr["dec1"]);
                                }
                                
                                if (reportSubType.Trim().ToUpper() == "SUMMARY")
                                {
                                    objmypl.plstMtdytdpandl.Add(_Obj);
                                }
                                else
                                {
                                    objmypl.plstMtdytdpandldetails.Add(_Obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return objmypl;
        }
        #endregion
    }
}
