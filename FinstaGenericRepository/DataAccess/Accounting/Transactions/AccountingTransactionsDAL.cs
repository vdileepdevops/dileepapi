using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Accounting.Transactions
{
    public partial class AccountingTransactionsDAL : SettingsDAL, IAccountingTransactions
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        public List<TypeofPaymentDTO> typeofpaymentlist { get; set; }
        JournalVoucherDTO objJournalVoucherDTO = null;
        AccountingTransactionsDAL objJournalVoucher = null;
        PaymentsDTO objPaymentsDTO = null;
      
        public List<BankDTO> banklist { get; set; }
        public List<AccountsDTO> accountslist { get; set; }
        public List<PartyDTO> partylist { get; set; }

        public List<GstDTo> GstList { get; set; }
        public List<Modeoftransaction> Modeoftransactionlist { get; set; }
        public List<BankDTO> bankdebitcardslist { get; set; }
        public List<BankUPI> bankupilist { get; set; }
        public List<ChequesDTO> chequeslist { get; set; }
        public List<GstDTo> statelist { get; set; }
        public List<PaymentVoucherDTO> ppaymentslist { get; set; }
        public PaymentVoucherDTO pPaymentVoucherDTO { get; set; }
        public List<GeneralReceiptSubDetails> GeneralReceiptlist { get; set; }
        public GeneralReceiptSubDetails _GeneralReceipt { get; set; }
        public List<PaymentsDTO> Paymentsdetailslist { get; set; }
        public PaymentVoucherReportDTO pPaymentVoucherReportDTO { get; set; }
        public List<GeneralreceiptDTO> pGeneralReceiptList { get; set; }
        public List<GeneralReceiptReportDTO> pGeneralReceiptReportDataList { get; set; }
        
        public GeneralReceiptReportDTO pGeneralReceiptData { get; set; }
        public async Task<List<TypeofPaymentDTO>> GetTypeofpaymentList(string ConnectionString)
        {
            await Task.Run(() =>
            {
                typeofpaymentlist = new List<TypeofPaymentDTO>();
                try
                {
                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select roleid,rolename from tblmstemployeerole t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE' order by rolename"))
                    //{
                    //    while (dr.Read())
                    //    {
                    //        TypeofPaymentDTO _EmployeeRole = new TypeofPaymentDTO();                        
                    //        _EmployeeRole.typeofpayment = Convert.ToString(dr["rolename"]);
                    //        typeofpaymentlist.Add(_EmployeeRole);


                    //    }
                    //}
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return typeofpaymentlist;
        }
        public async Task<List<BankDTO>> GetBankntList(string ConnectionString)
        {
            await Task.Run(() =>
            {
                banklist = new List<BankDTO>();
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.recordid,case when (accountnumber is null or coalesce(accountnumber,'')='') then bankname else bankname||' - '|| accountnumber end as bankname,bankbranch,sum(coalesce(bankbookbalance,0)) as bankbookbalance,sum(coalesce(bankbookbalance,0))+sum(coalesce(passbookbalance,0)) passbookbalance from tblmstbank t1 left join (select t1.recordid,coalesce(sum( coalesce(debitamount,0)-coalesce(creditamount,0)),0) as bankbookbalance from tblmstbank t1 join tbltranstotaltransactions t2 on t1.bankaccountid=t2.parentid group by t1.recordid)t2 on t1.recordid=t2.recordid left join (select depositedbankid,sum(passbookbalance)passbookbalance from (select bankid depositedbankid,paidamount as passbookbalance  from tbltranspaymentreference  where clearstatus ='N' union all select depositedbankid,-totalreceivedamount as passbookbalance from  tbltransreceiptreference  where depositstatus  ='P' and clearstatus='N' )x group by depositedbankid)t3 on t1.recordid=t3.depositedbankid where statusid=" + Convert.ToInt32(Status.Active) + " group by t1.recordid,bankname||' - '|| accountnumber ,bankbranch,t1.accountnumber,t1.bankname order by bankname;"))
                    {
                        while (dr.Read())
                        {
                            BankDTO _BankDTO = new BankDTO();
                            _BankDTO.pdepositbankname = _BankDTO.pbankname = Convert.ToString(dr["bankname"]);
                            _BankDTO.pdepositbankid = _BankDTO.pbankid = Convert.ToInt64(dr["recordid"]);
                            _BankDTO.pbranchname = Convert.ToString(dr["bankbranch"]);
                            _BankDTO.pbankbalance = Convert.ToDecimal(dr["bankbookbalance"]);
                            _BankDTO.pbankpassbookbalance = Convert.ToDecimal(dr["passbookbalance"]);
                            banklist.Add(_BankDTO);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return banklist;
        }
        public async Task<List<AccountsDTO>> GetLedgerAccountList(string ConnectionString, string formname)
        {
            string query = string.Empty;
            await Task.Run(() =>
            {
                accountslist = new List<AccountsDTO>();
                try
                {
                    if (formname == "DISBURSEMENT")
                    {
                        query = "select t1.accountid,t1.accountname,sum( coalesce(debitamount,0)-coalesce(creditamount,0)) as balance," +
                        " (case when acctype='L' then 'EQUITY AND LIABILITIES' when acctype='A' then 'ASSETS' when acctype='I' then 'INCOME' when acctype='E' then 'EXPENSES' end)acctype from tblmstaccounts t1 left join tbltranstotaltransactions t2 on t1.accountid=t2.parentid  where chracctype ='2' and t1.accountid not in (select accountid from tblmstuntransactionaccounts  where formname ='PAYMENT VOUCHER') and acctype  ='A' and t1.accountname='TRADE ADVANCE TO SHOWROOMS' and t1.statusid= " + Convert.ToInt32(Status.Active) + " group by t1.accountid,t1.accountname,acctype order by accountname;";

                    }
                    else
                    {
                        query = "select t1.accountid,t1.accountname,sum( coalesce(debitamount,0)-coalesce(creditamount,0)) as balance," +
                        " (case when acctype='L' then 'EQUITY AND LIABILITIES' when acctype='A' then 'ASSETS' when acctype='I' then 'INCOME' when acctype='E' then 'EXPENSES' end)acctype from tblmstaccounts t1 left join tbltranstotaltransactions t2 on t1.accountid=t2.parentid  where chracctype ='2' and t1.accountid not in (select accountid from tblmstuntransactionaccounts  where formname ='" + formname + "') and t1.statusid= " + Convert.ToInt32(Status.Active) + " group by t1.accountid,t1.accountname,acctype order by accountname;";
                    }

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            AccountsDTO _AccountsDTO = new AccountsDTO();
                            _AccountsDTO.pledgerid = Convert.ToInt64(dr["accountid"]);
                            _AccountsDTO.pledgername = Convert.ToString(dr["accountname"]);
                            _AccountsDTO.accountbalance = Convert.ToDecimal(dr["balance"]);

                            _AccountsDTO.id = Convert.ToInt64(dr["accountid"]);
                            _AccountsDTO.text = Convert.ToString(dr["accountname"]);
                            _AccountsDTO.pAccounttype = Convert.ToString(dr["acctype"]);

                            accountslist.Add(_AccountsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return accountslist;
        }

        public async Task<List<AccountsDTO>> GetSubLedgerAccountList(long ledgerid, string ConnectionString)
        {
            await Task.Run(() =>
            {
                accountslist = new List<AccountsDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.accountid,t1.accountname,sum(coalesce(debitamount, 0) - coalesce(creditamount, 0)) as balance from tblmstaccounts t1 left join tbltranstotaltransactions t2 on t1.accountid = t2.accountid  where t1.parentid = " + ledgerid + " and t1.chracctype = '3' and t1.statusid = " + Convert.ToInt32(Status.Active) + " group by t1.accountid,t1.accountname order by accountname;"))
                    {
                        while (dr.Read())
                        {
                            AccountsDTO _AccountsDTO = new AccountsDTO();
                            _AccountsDTO.psubledgerid = Convert.ToInt64(dr["accountid"]);
                            _AccountsDTO.psubledgername = Convert.ToString(dr["accountname"]);
                            _AccountsDTO.accountbalance = Convert.ToDecimal(dr["balance"]);
                            accountslist.Add(_AccountsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return accountslist;
        }

        public async Task<List<PartyDTO>> GetPartyList(string ConnectionString)
        {
            await Task.Run(() =>
            {
                partylist = new List<PartyDTO>();
                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t3.contactid,(coalesce(t3.name,'')||' '||coalesce(t3.surname,'')) as name,t3.titlename,contactreferenceid,t1.businessentitycontactno,t1.businessentityemailid,'PARTY' as partyreftype from tblmstparty t3 join  tblmstcontact t1 on t1.contactid=t3.contactid join tblmststatus t2 on t1.statusid=t2.statusid where   t3.statusid= " + Convert.ToInt32(Status.Active) + " order by name;"))
                    {
                        while (dr.Read())
                        {
                            PartyDTO _PartyDTO = new PartyDTO();
                            _PartyDTO.ppartyid = Convert.ToInt64(dr["contactid"]);
                            _PartyDTO.ppartyname = Convert.ToString(dr["name"]);
                            _PartyDTO.ppartyreferenceid = Convert.ToString(dr["contactreferenceid"]);
                            _PartyDTO.ppartyreftype = Convert.ToString(dr["partyreftype"]);
                            _PartyDTO.ppartycontactno = Convert.ToInt64(dr["businessentitycontactno"]);
                            _PartyDTO.ppartyemailid = Convert.ToString(dr["businessentityemailid"]);
                            _PartyDTO.ppartypannumber = Convert.ToString("");
                            partylist.Add(_PartyDTO);


                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return partylist;
        }

        public List<JournalVoucherDTO> pJournalVoucherList { get; set; }
        public async Task<List<GstDTo>> GetGstPercentages(string ConnectionString)
        {
            await Task.Run(() =>
            {
                GstList = new List<GstDTo>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid, coalesce(gstpercentage,0) gstpercentage,coalesce( cgstpercentage,0)cgstpercentage,coalesce( sgstpercentage,0)sgstpercentage,coalesce( utgstpercentage,0) utgstpercentage from tblmstgsttaxpercentage where statusid=" + Convert.ToInt32(Status.Active) + " order by gstpercentage;"))
                    {
                        while (dr.Read())
                        {
                            GstDTo GstDTO = new GstDTo
                            {
                                pRecordId = Convert.ToInt64(dr["recordid"]),
                                pgstpercentage = Convert.ToDecimal(dr["gstpercentage"]),
                                pigstpercentage = Convert.ToDecimal(dr["gstpercentage"]),
                                psgstpercentage = Convert.ToDecimal(dr["cgstpercentage"]),
                                pcgstpercentage = Convert.ToDecimal(dr["sgstpercentage"]),
                                putgstpercentage = Convert.ToDecimal(dr["utgstpercentage"])
                            };
                            GstList.Add(GstDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return GstList;
        }

        public async Task<List<Modeoftransaction>> GetModeoftransactions(string ConnectionString)
        {
            await Task.Run(() =>
            {
                Modeoftransactionlist = new List<Modeoftransaction>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,transmode,modeoftype,submodeoftype,chqonhand,chqinbank,chqclear from tblmstmodeoftransaction where statusid=" + Convert.ToInt32(Status.Active) + " order by transmode;"))
                    {
                        while (dr.Read())
                        {
                            Modeoftransaction ModeoftransactionDTO = new Modeoftransaction
                            {
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pmodofPayment = Convert.ToString(dr["transmode"]),
                                pmodofreceipt = Convert.ToString(dr["transmode"]),
                                ptranstype = Convert.ToString(dr["modeoftype"]),
                                ptypeofpayment = Convert.ToString(dr["submodeoftype"]),
                                pchqonhandstatus = Convert.ToString(dr["chqonhand"]),
                                pchqinbankstatus = Convert.ToString(dr["chqinbank"]),
                                pchqclearstatus = Convert.ToString(dr["chqclear"])
                            };
                            Modeoftransactionlist.Add(ModeoftransactionDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return Modeoftransactionlist;
        }
        public async Task<List<BankDTO>> GetDebitCardNumbers(string ConnectionString)
        {
            await Task.Run(() =>
            {
                bankdebitcardslist = new List<BankDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t2.bankid,bankname,cardnumber from tblmstbank t1 join tblmstbankdebitcarddtls t2 on t1.recordid=t2.bankid where t2.statusid=" + Convert.ToInt32(Status.Active) + " order by cardnumber;"))
                    {
                        while (dr.Read())
                        {
                            BankDTO _BankDebitCardsDTO = new BankDTO
                            {
                                pCardNumber = Convert.ToString(dr["cardnumber"]),
                                pbankname = Convert.ToString(dr["bankname"]),
                                pbankid = Convert.ToInt64(dr["bankid"]),
                            };
                            bankdebitcardslist.Add(_BankDebitCardsDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return bankdebitcardslist;
        }
        public async Task<List<BankUPI>> GetUpiNames(long bankid, string ConnectionString)
        {
            await Task.Run(() =>
            {
                bankupilist = new List<BankUPI>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select upiid,upiname from tblmstbankupidtls where bankid=" + bankid + " and statusid=" + Convert.ToInt32(Status.Active) + " order by upiname;"))
                    {
                        while (dr.Read())
                        {
                            BankUPI _BankUPI = new BankUPI
                            {
                                pUpiid = Convert.ToString(dr["upiid"]),
                                pUpiname = Convert.ToString(dr["upiname"])
                            };
                            bankupilist.Add(_BankUPI);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return bankupilist;
        }
        public async Task<List<ChequesDTO>> GetChequeNumbers(long bankid, string ConnectionString)
        {
            await Task.Run(() =>
            {
                chequeslist = new List<ChequesDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  chqbookid , chequenumber  from tblmstcheques  where  bankid =" + bankid + " and statusid in (select statusid from tblmststatus  where    upper(statusname )='UNUSED-CHEQUES'  ) order by chequenumber;"))
                    {
                        while (dr.Read())
                        {
                            ChequesDTO _ChequesDTO = new ChequesDTO
                            {
                                pChequenumber = Convert.ToInt64(dr["chequenumber"]),
                                pChqbookid = Convert.ToInt64(dr["chqbookid"])
                            };
                            chequeslist.Add(_ChequesDTO);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return chequeslist;
        }

        public async Task<List<GstDTo>> getStatesbyPartyid(long ppartyid, string ConnectionString, int id)
        {
            await Task.Run(() =>
            {
                statelist = new List<GstDTo>();
                string query = "";
                try
                {
                    if (Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstparty t1 join  tblmstpartytaxdetails  t2 on t1.partiid =t2.partiid where isgstapplicable =true and  contactid =" + ppartyid + "; ")))
                    {
                        query = "select stateid,state,case when statecode<>branchstatcode and sgsttype='SGST' then 'IGST' else 'CGST,'||sgsttype end as gsttype from (select t1.stateid,t1.state,t1.statecode,case when isstateunionterritory=false then 'SGST' else 'UTGST' end as sgsttype,t2.statecode as branchstatcode from (select t1.statusid,t3.stateid,t3.state||'-'|| gstno state,statecode,isstateunionterritory from tblmstparty t1 join tblmstpartytaxdetails  t2 on t1.partiid=t2.partiid join tblmststate t3 on t3.state=t2.statename  where t1.contactid =" + ppartyid + ") t1,tblmstbranch t2 where t1.statusid  =" + Convert.ToInt32(Status.Active) + ")x order by state;";
                    }
                    else
                    {
                        query = "select stateid,state,case when statecode<>branchstatcode and sgsttype='SGST' then 'IGST' else 'CGST,'||sgsttype end as gsttype from (select t1.stateid,t1.state,t1.statecode,case when isstateunionterritory=false then 'SGST' else 'UTGST' end as sgsttype,t2.statecode as branchstatcode from tblmststate t1,tblmstbranch t2 where t1.statusid  =" + Convert.ToInt32(Status.Active) + ")x order by state;";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            GstDTo obGstDTo = new GstDTo();
                            obGstDTo.pState = dr["state"].ToString();
                            obGstDTo.pStateId = Convert.ToInt32(dr["stateid"]);
                            obGstDTo.pgsttype = Convert.ToString(dr["gsttype"]);
                            statelist.Add(obGstDTo);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return statelist;

        }
        public async Task<List<TdsSectionDTO>> getTdsSectionsbyPartyid(long ppartyid, string ConnectionString)
        {
            await Task.Run(() =>
            {
                lstTdsSectionDetails = new List<TdsSectionDTO>();
                string query = "";
                try
                {
                    if (Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstparty t1 join  tblmstpartytaxdetails  t2 on t1.partiid =t2.partiid where istdsapplicable =true and  contactid =" + ppartyid + "; ")))
                    {
                        query = "select distinct istdsapplicable,tdssectionname tdssection,coalesce( tdspercentage,0) tdspercentage from tblmstparty t1 join tblmstpartytaxdetails  t2 on t1.partiid=t2.partiid join tblmsttdssections t3 on t2.tdssectionname=t3.tdssection where t1.contactid =" + ppartyid;

                    }
                    else
                    {
                        query = "select distinct tdssection,false as istdsapplicable ,coalesce( tdspercentage,0) tdspercentage from tblmsttdssections where statusid=" + Convert.ToInt32(Status.Active) + "  order by tdssection;";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            TdsSectionDTO objTdsSectionDetails = new TdsSectionDTO();
                            
                            objTdsSectionDetails.pTdsSection = Convert.ToString(dr["tdssection"]);
                            objTdsSectionDetails.pTdsPercentage = Convert.ToDecimal(dr["tdspercentage"]);
                            objTdsSectionDetails.istdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                            lstTdsSectionDetails.Add(objTdsSectionDetails);
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return lstTdsSectionDetails;
        }

        public async Task<decimal> getpartyAccountbalance(long ppartyid, string ConnectionString)
        {
            decimal accountbalance = 0;
            await Task.Run(() =>
            {
                lstTdsSectionDetails = new List<TdsSectionDTO>();
                string query = "";
                try
                {

                    query = "select coalesce( sum( coalesce(debitamount,0)-coalesce(creditamount,0)),0) as balance from tbltranstotaltransactions where contactid=" + ppartyid + "; ";

                    accountbalance = Convert.ToDecimal(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, query));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return accountbalance;
        }
        public async Task<decimal> getcashbalance(string ConnectionString)
        {
            decimal accountbalance = 0;
            await Task.Run(() =>
            {
                lstTdsSectionDetails = new List<TdsSectionDTO>();
                string query = "";
                try
                {

                    query = "select coalesce(sum(balance),0) from (select coalesce(accountbalance,0)balance from   tblmstaccounts    where  accountname='CASH ON HAND')x; ";


                    //query = "select coalesce(sum(balance),0) from (select coalesce(accountbalance,0)balance from   tblmstaccounts    where accountname='MAIN CASH' or  accountname='MAINCASH' union all select coalesce(totalreceivedamount,0) from tbltransreceiptreference where   clearstatus  ='N' and depositstatus  ='N')x; ";

                    accountbalance = Convert.ToDecimal(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, query));
                }
                catch (Exception ex)
                {{
                }
                throw ex;
                }
            });
            return accountbalance;
        }

        public decimal GetBankBalance(long recordid, string con)
        {
            try
            {
                recordid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select coalesce(sum( coalesce(debitamount,0)-coalesce(creditamount,0)),0) as balance from tbltranstotaltransactions where parentid in (select bankaccountid from tblmstbank  where case when '" + recordid + "'=0 then recordid>0 else recordid=" + recordid + " end);"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return recordid;
        }

    }
}
