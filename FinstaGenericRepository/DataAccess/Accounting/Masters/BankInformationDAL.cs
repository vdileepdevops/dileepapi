using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using HelperManager;
using System.Data;
using FinstaRepository.DataAccess.Accounting.Transactions;
namespace FinstaRepository.DataAccess.Accounting.Masters
{
    public class BankInformationDAL : SettingsDAL, IBankInformation
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public List<BankdebitcarddtlsDTO> lstBankdebitcarddtls { get; set; }
        public List<BankInformationAddressDTO> lstBankInformationAddress { get; set; }
        public List<BankUPI> lstBankUpi { get; set; }
        AccountingTransactionsDAL objJournalVoucher = new AccountingTransactionsDAL();
        JournalVoucherDTO objJournalVoucherDTO = new JournalVoucherDTO();
        PaymentsDTO objPaymentsDTO = null;
        Int64 BankAccountId = 0;
        Int64 OpeningBalanceAccountID;
        StringBuilder strSave = new StringBuilder();
        #region Save BankInformation
        public bool SaveBankInformation(BankInformationDTO lstBankInformation, string ConnectionString)
        {
            bool isSaved = false;
            //objJournalVoucher = new AccountingTransactionsDAL();
            //objJournalVoucherDTO = new JournalVoucherDTO();
            Int64 RecordId = 0;
            Int64 BankParentId;
            string P_Recordid = string.Empty;
            string bankaccountname = string.Empty;
            StringBuilder query = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                BankParentId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts where accountname='CURRENT ACCOUNTS WITH BANKS'  and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                if (!string.IsNullOrEmpty(lstBankInformation.pAccountnumber.Trim()))
                {
                    bankaccountname = lstBankInformation.pBankname.ToUpper().Trim() + "_" + ManageQuote(lstBankInformation.pAccountnumber.Trim());
                }
                else
                {
                    bankaccountname = lstBankInformation.pBankname.Trim();
                }


                //OpeningBalanceAccountID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('OPENING BALANCE')  and statusid=" + Convert.ToInt32(Status.Active) + " and chracctype='2';"));
              

                Int64 OpeningBalanceparentAccountID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('OTHER CURRENT LIABILITIES')  and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                OpeningBalanceAccountID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('OPENING BALANCE', " + OpeningBalanceparentAccountID + ", '2'," + lstBankInformation.pCreatedby + ")"));

                objJournalVoucherDTO.pCreatedby = lstBankInformation.pCreatedby;
                if (lstBankInformation != null)
                {

                    if (lstBankInformation.ptypeofoperation.ToUpper() == "CREATE")
                    {

                        BankAccountId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + bankaccountname + "', " + BankParentId + ", '2'," + lstBankInformation.pCreatedby + ")"));
                        //Int64 bankcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tblmstbank  where bankname='" + ManageQuote(lstBankInformation.pBankname.Trim()) + "' and accountnumber='" + ManageQuote(lstBankInformation.pAccountnumber.Trim()) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                        //if (bankcount == 0)
                        //{
                        RecordId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tblmstbank(bankname, bankbranch, accountname, acctountype, accountnumber, bankdate, overdraft, bankaccountid, ifsccode,  statusid, createdby, createddate,openingbalance,isdebitcardapplicable, isupiapplicable) VALUES ('" + ManageQuote(lstBankInformation.pBankname) + "','" + ManageQuote(lstBankInformation.pBankbranch) + "','" + ManageQuote(lstBankInformation.pAccountname) + "','" + ManageQuote(lstBankInformation.pAcctountype) + "','" + ManageQuote(lstBankInformation.pAccountnumber) + "','" + FormatDate(lstBankInformation.pBankdate) + "'," + lstBankInformation.pOverdraft + "," + BankAccountId + ",'" + ManageQuote(lstBankInformation.pIfsccode) + "'," + getStatusid(lstBankInformation.pStatusname, ConnectionString) + "," + lstBankInformation.pCreatedby + ",current_timestamp," + lstBankInformation.pOpeningBalance + "," + lstBankInformation.pIsdebitcardapplicable + "," + lstBankInformation.pIsupiapplicable + ") returning recordid;"));
                        if (lstBankInformation.pOpeningBalance != 0)
                        {
                            objJournalVoucherDTO.ptypeofoperation = "CREATE";
                            lstBankInformation.pRecordid = RecordId;
                            SaveOpeningJV(lstBankInformation);
                        }
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                    }
                    if (lstBankInformation.ptypeofoperation.ToUpper() == "UPDATE")
                    {
                         BankAccountId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select bankaccountid from tblmstbank  where recordid=" + lstBankInformation.pRecordid + "  and statusid=" + Convert.ToInt32(Status.Active) + ";"));

                        string _paccountnumber = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountnumber from tblmstbank  where recordid=" + lstBankInformation.pRecordid + "  and statusid=" + Convert.ToInt32(Status.Active) + ";").ToString();

                        strSave.Append("select fn_updatebankaccountname(" + lstBankInformation.pRecordid + " ,'" + _paccountnumber + "' ,'" + ManageQuote(lstBankInformation.pAccountnumber) + "');");

                        strSave.Append("UPDATE tblmstbank   SET   acctountype='" + ManageQuote(lstBankInformation.pAcctountype) + "', accountnumber='" + ManageQuote(lstBankInformation.pAccountnumber) + "', accountname='" + ManageQuote(lstBankInformation.pAccountname) + "', bankname='" + ManageQuote(lstBankInformation.pBankname) + "', bankbranch='" + ManageQuote(lstBankInformation.pBankbranch) + "', ifsccode='" + ManageQuote(lstBankInformation.pIfsccode) + "', overdraft=" + lstBankInformation.pOverdraft + ", openingbalance=" + lstBankInformation.pOpeningBalance + ", isdebitcardapplicable=" + lstBankInformation.pIsdebitcardapplicable + ", isupiapplicable=" + lstBankInformation.pIsupiapplicable + ", bankaccountid=" + BankAccountId + ",statusid=" + getStatusid(lstBankInformation.pStatusname, ConnectionString) + ", modifiedby=" + lstBankInformation.pCreatedby + ", modifieddate=current_timestamp WHERE recordid=" + lstBankInformation.pRecordid + ";");
                        DataSet ds = new DataSet();
                        ds = NPGSqlHelper.ExecuteDataset(con, CommandType.Text, "select recordid,jvaccountid,ledgeramount,accounttranstype from tbltransjournalvoucherdetails  where jvnumber  ='" + lstBankInformation.popeningjvno + "' and jvaccountid=" + BankAccountId + "");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if ((Convert.ToInt64(ds.Tables[0].Rows[0]["ledgeramount"]) != lstBankInformation.pOpeningBalance) || (ds.Tables[0].Rows[0]["accounttranstype"].ToString() != lstBankInformation.pOpeningBalanceType.ToUpper()))
                            {
                                objJournalVoucherDTO.ptypeofoperation = "UPDATE";
                                objJournalVoucherDTO.pjvnumber = lstBankInformation.popeningjvno;
                                SaveOpeningJV(lstBankInformation);
                            }
                        }
                        else if (lstBankInformation.pOpeningBalance != 0)
                        {
                            objJournalVoucherDTO.ptypeofoperation = "CREATE";
                            SaveOpeningJV(lstBankInformation);
                        }
                    }

                    if (lstBankInformation.lstBankInformationAddressDTO != null)
                    {
                        for (int i = 0; i < lstBankInformation.lstBankInformationAddressDTO.Count; i++)
                        {
                            long count = 0;
                            count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbankaddressdtls  where bankid =" + lstBankInformation.pRecordid + ""));
                            if (lstBankInformation.lstBankInformationAddressDTO[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                lstBankInformation.pRecordid = RecordId;
                            }
                            if (count == 0)
                            {
                                strSave.Append("INSERT INTO tblmstbankaddressdtls(bankid, address1, address2, city, district, state, country, pincode, statusid, createdby, createddate) VALUES (" + lstBankInformation.pRecordid + ",'" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pAddress1) + "','" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pAddress2) + "','" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pcity) + "','" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pDistrict) + "','" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pState) + "','" + lstBankInformation.lstBankInformationAddressDTO[i].pCountry + "'," + lstBankInformation.lstBankInformationAddressDTO[i].pPincode + "," + getStatusid(lstBankInformation.lstBankInformationAddressDTO[i].pStatusname, ConnectionString) + "," + lstBankInformation.lstBankInformationAddressDTO[i].pCreatedby + ",current_timestamp);");
                            }
                            else if (count > 0)
                            {
                                strSave.Append("UPDATE tblmstbankaddressdtls SET  address1='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pAddress1) + "', address2='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pAddress2) + "', city='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pcity) + "' , district='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pDistrict) + "',state='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pState) + "', country='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pCountry) + "', pincode='" + ManageQuote(lstBankInformation.lstBankInformationAddressDTO[i].pPincode) + "', statusid=" + getStatusid(lstBankInformation.lstBankInformationAddressDTO[i].pStatusname, ConnectionString) + ", modifiedby=" + lstBankInformation.lstBankInformationAddressDTO[i].pCreatedby + ", modifieddate=current_timestamp WHERE recordid=" + lstBankInformation.lstBankInformationAddressDTO[i].pRecordid + ";");
                            }
                        }
                    }
                    if (lstBankInformation.lstBankdebitcarddtlsDTO != null)
                    {
                        for (int i = 0; i < lstBankInformation.lstBankdebitcarddtlsDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom))
                            {
                                lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom = "null";
                            }
                            else
                            {
                                lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom = "'" + FormatDate(Convert.ToDateTime(lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom).ToString("01-MM-yyyy")) + "'";
                            }
                            if (string.IsNullOrEmpty(lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto))
                            {
                                lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto = "null";
                            }
                            else
                            {
                                lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto = "'" + FormatDate(Convert.ToDateTime(lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto).ToString("01-MM-yyyy")) + "'";
                            }
                            long count = 0;
                            count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbankdebitcarddtls  where bankid =" + lstBankInformation.pRecordid + ""));
                            if (lstBankInformation.lstBankdebitcarddtlsDTO[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                lstBankInformation.pRecordid = RecordId;
                            }
                            if (count == 0)
                            {
                                strSave.Append("INSERT INTO tblmstbankdebitcarddtls(bankid, cardnumber, cardname, validfrom, validto, statusid, createdby, createddate) VALUES (" + lstBankInformation.pRecordid + "," + lstBankInformation.lstBankdebitcarddtlsDTO[i].pCardNo + ",'" + ManageQuote(lstBankInformation.lstBankdebitcarddtlsDTO[i].pCardName) + "'," + (lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom) + "," + (lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto) + "," + getStatusid(lstBankInformation.lstBankdebitcarddtlsDTO[i].pStatusname, ConnectionString) + "," + lstBankInformation.lstBankdebitcarddtlsDTO[i].pCreatedby + ",current_timestamp);");
                            }
                            else if (count > 0)
                            {
                                strSave.Append("UPDATE tblmstbankdebitcarddtls SET cardnumber=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pCardNo + ", cardname='" + ManageQuote(lstBankInformation.lstBankdebitcarddtlsDTO[i].pCardName) + "', validfrom=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidfrom + ", validto=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pValidto + ", statusid=" + getStatusid(lstBankInformation.lstBankdebitcarddtlsDTO[i].pStatusname, ConnectionString) + ",  modifiedby=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pCreatedby + ", modifieddate=current_timestamp WHERE recordid=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pRecordid + ";");
                            }
                            strSave.Append("update tblmstbank set isdebitcardapplicable =" + lstBankInformation.pIsdebitcardapplicable + " where recordid=" + lstBankInformation.pRecordid + ";");
                        }
                        //if (lstBankInformation.pIsdebitcardapplicable == false)
                        //{
                        //    strSave.Append("UPDATE tblmstbankdebitcarddtls set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + lstBankInformation.pCreatedby + ", modifieddate=current_timestamp WHERE bankid=" + lstBankInformation.pRecordid + " ;");
                        //}

                    }
                    if (lstBankInformation.lstBankUPI != null)
                    {
                        for (int i = 0; i < lstBankInformation.lstBankUPI.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(lstBankInformation.lstBankUPI[i].ptypeofoperation))
                            {
                                if (lstBankInformation.lstBankUPI[i].ptypeofoperation.ToUpper() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(P_Recordid))
                                    {
                                        P_Recordid = lstBankInformation.lstBankUPI[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        P_Recordid = P_Recordid + "," + lstBankInformation.lstBankUPI[i].pRecordid.ToString();
                                    }
                                }
                                if (lstBankInformation.ptypeofoperation.ToUpper() == "CREATE")
                                {
                                    lstBankInformation.pRecordid = RecordId;
                                }
                                if (lstBankInformation.lstBankUPI[i].ptypeofoperation.ToUpper() == "CREATE")
                                {
                                    strSave.Append("INSERT INTO tblmstbankupidtls(bankid, upiid, upiname, statusid, createdby, createddate) VALUES (" + lstBankInformation.pRecordid + ",'" + ManageQuote(lstBankInformation.lstBankUPI[i].pUpiid) + "','" + ManageQuote(lstBankInformation.lstBankUPI[i].pUpiname) + "'," + getStatusid(lstBankInformation.lstBankUPI[i].pStatusname, ConnectionString) + "," + lstBankInformation.lstBankUPI[i].pCreatedby + ",current_timestamp);");
                                }
                                if (lstBankInformation.lstBankUPI[i].ptypeofoperation.ToUpper() == "UPDATE")
                                {
                                    strSave.Append("UPDATE tblmstbankupidtls SET upiid='" + ManageQuote(lstBankInformation.lstBankUPI[i].pUpiid) + "', upiname='" + ManageQuote(lstBankInformation.lstBankUPI[i].pUpiname) + "', statusid=" + getStatusid(lstBankInformation.lstBankUPI[i].pStatusname, ConnectionString) + ", modifiedby=" + lstBankInformation.lstBankUPI[i].pCreatedby + ", modifieddate=current_timestamp WHERE recordid=" + lstBankInformation.lstBankUPI[i].pRecordid + ";");
                                }
                                strSave.Append("update tblmstbank set isupiapplicable =" + lstBankInformation.pIsupiapplicable + " where recordid=" + lstBankInformation.pRecordid + ";");
                            }
                        }
                        if (!string.IsNullOrEmpty(P_Recordid))
                        {
                            query.Append("UPDATE tblmstbankupidtls set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + lstBankInformation.pCreatedby + ", modifieddate=current_timestamp WHERE bankid=" + lstBankInformation.pRecordid + " and recordid not in(" + P_Recordid + ")  AND statusid<>2;");
                        }
                        else
                        {
                            query.Append("UPDATE tblmstbankupidtls set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + lstBankInformation.pCreatedby + ", modifieddate=current_timestamp WHERE bankid=" + lstBankInformation.pRecordid + "  AND statusid<>2 ;");
                        }

                    }
                }

                if (strSave.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query + "" + strSave.ToString());
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

        #endregion
        public BankInformationDTO SaveOpeningJV(BankInformationDTO lstBankInformation)
        {
            string refjvnumber = "";
            List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();
            string JvDate = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select transactionopeningdate from tblmstcompany where statusid=" + Convert.ToInt32(Status.Active) + ";").ToString();
            objJournalVoucherDTO.pjvdate = JvDate;
            objJournalVoucherDTO.pnarration = "OPENING BALANCE OF BANK ACCOUNT";
            objJournalVoucherDTO.pmodoftransaction = "AUTO";
            if (lstBankInformation.pOpeningBalanceType.ToUpper() == "C")
            {
                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ptranstype = "C";
                objPaymentsDTO.psubledgerid = BankAccountId;
                objPaymentsDTO.pamount = lstBankInformation.pOpeningBalance;
                _Paymentslist.Add(objPaymentsDTO);
                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ptranstype = "D";
                objPaymentsDTO.psubledgerid = OpeningBalanceAccountID;
                objPaymentsDTO.pamount = lstBankInformation.pOpeningBalance;
                _Paymentslist.Add(objPaymentsDTO);
            }
            else if (lstBankInformation.pOpeningBalanceType.ToUpper() == "D")
            {
                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ptranstype = "D";
                objPaymentsDTO.psubledgerid = BankAccountId;
                objPaymentsDTO.pamount = lstBankInformation.pOpeningBalance;
                _Paymentslist.Add(objPaymentsDTO);
                objPaymentsDTO = new PaymentsDTO();
                objPaymentsDTO.ptranstype = "C";
                objPaymentsDTO.psubledgerid = OpeningBalanceAccountID;
                objPaymentsDTO.pamount = lstBankInformation.pOpeningBalance;
                _Paymentslist.Add(objPaymentsDTO);

            }
            objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
            if (objJournalVoucherDTO.pJournalVoucherlist.Count > 0)
            {
                objJournalVoucher.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
            }
            strSave.Append("UPDATE tblmstbank SET openingjvno='" + refjvnumber + "'  WHERE recordid=" + lstBankInformation.pRecordid + ";");
            return lstBankInformation;
        }

        public List<BankUPI> GetBankUPIDetails(string con)
        {
            List<BankUPI> lstBankUPI = new List<BankUPI>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,upiname from tblmstbankupi t join tblmststatus t1 on t.statusid=t1.statusid where t1.statusname='Active';"))
                {
                    while (dr.Read())
                    {
                        BankUPI obj = new BankUPI();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pUpiname = Convert.ToString(dr["upiname"]);
                        lstBankUPI.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankUPI;
        }

        public List<BankInformationDTO> GetBAnkDetails(string con)
        {
            List<BankInformationDTO> lstBankInformation = new List<BankInformationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,case when (accountnumber is null or accountnumber='') then  coalesce(bankname,'') else (coalesce(bankname,'')||' - '||coalesce(accountnumber,'')) end as bankname from tblmstbank t1 join tblmststatus ts on t1.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' order by bankname"))
                {
                    while (dr.Read())
                    {
                        BankInformationDTO obj = new BankInformationDTO();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pBankname = Convert.ToString(dr["bankname"]);
                        lstBankInformation.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankInformation;
        }

        public List<BankInformationDTO> GetBankNames(string con)
        {
            List<BankInformationDTO> lstBankInformation = new List<BankInformationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select bank_id ,bank_name from bank_names where  status='t'"))
                {
                    while (dr.Read())
                    {
                        BankInformationDTO obj = new BankInformationDTO();
                        obj.pRecordid = Convert.ToInt64(dr["bank_id"]);
                        obj.pBankname = Convert.ToString(dr["bank_name"]);
                        lstBankInformation.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankInformation;
        }

        public Int64 GenerateBookId(NpgsqlTransaction trans)
        {
            Int64 BookId;
            try
            {
                BookId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select (coalesce(max(chqbookid),0)+1) bookid from tblmstchequemanagement;"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return BookId;
        }

        public long GetExistingChequeCount(string con, long bankId, long chqFromNo, long chqToNo)
        {
            Int64 chqcount = 0;
            try
            {
                chqcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstchequemanagement where bankid= " + bankId + " and (chequefrom between " + chqFromNo + " and " + chqToNo + " or chequeto between " + chqFromNo + " and " + chqToNo + ")"));

                return chqcount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveChequeManagement(BankInformationDTO lstChequemanagement, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder strChqSave = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                Int64 ChqBookId;
                for (int i = 0; i < lstChequemanagement.lstChequemanagementDTO.Count; i++)
                {
                    if (!string.IsNullOrEmpty(lstChequemanagement.lstChequemanagementDTO[i].ptypeofoperation))
                    {
                        if (lstChequemanagement.lstChequemanagementDTO[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            ChqBookId = GenerateBookId(trans);
                            Int64 count = GetExistingChequeCount(ConnectionString, lstChequemanagement.lstChequemanagementDTO[i].pBankId, lstChequemanagement.lstChequemanagementDTO[i].pChequefrom, lstChequemanagement.lstChequemanagementDTO[i].pChequeto);
                            if (count > 0)
                            {
                                return false;
                            }
                            else
                            {
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO tblmstchequemanagement(bankid,  noofcheques, chequefrom, chequeto, chqbookid, chqegeneratestatus,statusid, createdby, createddate) VALUES (" + lstChequemanagement.lstChequemanagementDTO[i].pBankId + "," + lstChequemanagement.lstChequemanagementDTO[i].pNoofcheques + "," + lstChequemanagement.lstChequemanagementDTO[i].pChequefrom + "," + lstChequemanagement.lstChequemanagementDTO[i].pChequeto + "," + ChqBookId + ",'" + lstChequemanagement.pChqegeneratestatus + "'," + getStatusid(lstChequemanagement.lstChequemanagementDTO[i].pStatusname, ConnectionString) + "," + lstChequemanagement.lstChequemanagementDTO[i].pCreatedby + ",current_timestamp);");
                            }
                        }
                        else if (lstChequemanagement.lstChequemanagementDTO[i].ptypeofoperation.ToUpper() == "UPDATE")
                        {
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "update tblmstchequemanagement set chqegeneratestatus='" + lstChequemanagement.lstChequemanagementDTO[i].pChqegeneratestatus + "',modifiedby=" + lstChequemanagement.lstChequemanagementDTO[i].pCreatedby + ",modifieddate=current_timestamp where chqbookid=" + lstChequemanagement.lstChequemanagementDTO[i].pChqbookid + "");
                        }
                    }
                }
                //if (strChqSave.Length > 0)
                //{
                //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strChqSave.ToString());
                //}
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
        public List<string> GetCheckDuplicateDebitCardNo(string con, BankInformationDTO lstBankInformation)
        {
            Int64 DebtCardCount = 0;
            Int64 UPIIdCount = 0;
            Int64 bankcount = 0;
            List<string> lstdata = new List<string>();
            try
            {
                if (lstBankInformation != null)
                {

                    if (lstBankInformation.lstBankdebitcarddtlsDTO != null)
                    {
                        for (int i = 0; i < lstBankInformation.lstBankdebitcarddtlsDTO.Count; i++)
                        {
                            DebtCardCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbankdebitcarddtls t1 join tblmststatus t2 on t1.statusid=t2.statusid where t1.statusid=" + Convert.ToInt32(Status.Active) + " and  cardnumber=" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pCardNo + " and recordid not in(" + lstBankInformation.lstBankdebitcarddtlsDTO[i].pRecordid + ");"));
                        }
                    }
                    if (lstBankInformation.lstBankUPI != null)
                    {
                        for (int i = 0; i < lstBankInformation.lstBankUPI.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(lstBankInformation.lstBankUPI[i].pUpiid))
                            {
                                UPIIdCount = UPIIdCount + Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbankupidtls t1 join tblmststatus t2 on t1.statusid=t2.statusid where t1.statusid=" + Convert.ToInt32(Status.Active) + " and upper(upiid)='" + lstBankInformation.lstBankUPI[i].pUpiid.ToUpper() + "' and recordid not in(" + lstBankInformation.lstBankUPI[i].pRecordid + ") ;"));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(lstBankInformation.pBankname))
                    {
                        bankcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbank  where bankname='" + ManageQuote(lstBankInformation.pBankname.Trim()) + "' and accountnumber='" + ManageQuote(lstBankInformation.pAccountnumber) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and recordid not in(" + lstBankInformation.pRecordid + ");"));
                    }
                    // lstdata.Add(DebtCardCount.ToString() + ',' + UPIIdCount.ToString() + ',' + bankcount.ToString());
                    if (DebtCardCount == 0 && UPIIdCount == 0 && bankcount == 0)
                    {
                        lstdata.Add("TRUE");
                    }
                    else if (DebtCardCount > 0 && UPIIdCount > 0 && bankcount > 0)
                    {
                        lstdata.Add("FALSE");
                    }
                    else if (bankcount > 0)
                    {
                        lstdata.Add("Bank Already Exist");
                    }
                    else if (DebtCardCount > 0)
                    {
                        lstdata.Add("Debit Card Already Exist");
                    }
                    else if (UPIIdCount > 0)
                    {
                        lstdata.Add("UPI Id Already Exist");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstdata;
        }
        public long GetCheckDuplicateUPIId(string con, string uPIId)
        {
            Int64 UPIIdCount = 0;
            try
            {
                UPIIdCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tblmstbankupidtls t1 join tblmststatus t2 on t1.statusid=t2.statusid where t1.statusid=" + Convert.ToInt32(Status.Active) + " and upper(upiid)='" + uPIId.ToUpper() + "' ;"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return UPIIdCount;
        }
        public List<ChequemanagementDTO> ViewChequeManagementDetails(string con)
        {
            List<ChequemanagementDTO> lstChequemanagement = new List<ChequemanagementDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select t1.chqbookid,t1.noofcheques,t1.chequefrom,t1.chequeto,t1.chqegeneratestatus,bankname,accountnumber from tblmstchequemanagement t1 join tblmstbank t2 on t1.bankid=t2.recordid join tblmststatus ts on t1.statusid=ts.statusid and upper(statusname)='ACTIVE';"))
                {
                    while (dr.Read())
                    {
                        ChequemanagementDTO obj = new ChequemanagementDTO();
                        obj.pChqbookid = Convert.ToInt64(dr["chqbookid"]);
                        obj.pNoofcheques = Convert.ToInt64(dr["noofcheques"]);
                        obj.pChequefrom = Convert.ToInt64(dr["chequefrom"]);
                        obj.pChequeto = Convert.ToInt64(dr["chequeto"]);
                        obj.pChqegeneratestatus = Convert.ToString(dr["chqegeneratestatus"]);
                        obj.pBankname = Convert.ToString(dr["bankname"]);
                        obj.pAccountnumber = Convert.ToString(dr["accountnumber"]);
                        lstChequemanagement.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstChequemanagement;
        }

        public List<BankInformationDTO> ViewBankInformationDetails(string con)
        {
            List<BankInformationDTO> lstBankInformation = new List<BankInformationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,bankname,accountnumber,accountname,statusname,isdebitcardapplicable,isupiapplicable from tblmstbank t join tblmststatus ts on t.statusid=ts.statusid  ;"))
                {
                    while (dr.Read())
                    {
                        BankInformationDTO obj = new BankInformationDTO();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pBankname = Convert.ToString(dr["bankname"]);
                        obj.pAccountnumber = Convert.ToString(dr["accountnumber"]);
                        obj.pAccountname = Convert.ToString(dr["accountname"]);
                        obj.pStatusname = Convert.ToString(dr["statusname"]);
                        obj.pIsdebitcardapplicable = Convert.ToBoolean(dr["isdebitcardapplicable"]);
                        obj.pIsupiapplicable = Convert.ToBoolean(dr["isupiapplicable"]);
                        lstBankInformation.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankInformation;
        }

        public BankInformationDTO ViewBankInformation(Int64 _precordid, string con)
        {
            BankInformationDTO objBankInformationDTO = new BankInformationDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,bankdate,accountnumber,bankname,accountname,bankbranch,ifsccode,coalesce(overdraft,0) as overdraft,coalesce(openingbalance,0) as openingbalance,isdebitcardapplicable,isupiapplicable,statusname,'OLD' as typeofoperation,acctountype,openingjvno,(select accounttranstype from tbltransjournalvoucherdetails  where jvnumber  =t.openingjvno and jvaccountid=t.bankaccountid) as OpeningBalanceType from tblmstbank t join tblmststatus ts on t.statusid=ts.statusid and upper(statusname)=upper('active') where recordid=" + _precordid + ""))
                {
                    while (dr.Read())
                    {
                        objBankInformationDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        if ((dr["bankdate"].ToString()) != null || !string.IsNullOrEmpty(dr["bankdate"].ToString()))
                        {
                            objBankInformationDTO.pBankdate = Convert.ToDateTime(dr["bankdate"]).ToString("dd/MM/yyyy");
                        }
                        objBankInformationDTO.pAccountnumber = Convert.ToString(dr["accountnumber"]);
                        objBankInformationDTO.pBankname = Convert.ToString(dr["bankname"]);
                        objBankInformationDTO.pAccountname = Convert.ToString(dr["accountname"]);
                        objBankInformationDTO.pBankbranch = Convert.ToString(dr["bankbranch"]);
                        objBankInformationDTO.pIfsccode = Convert.ToString(dr["ifsccode"]);
                        objBankInformationDTO.pOverdraft = Convert.ToInt64(dr["overdraft"]);
                        objBankInformationDTO.pOpeningBalance = Convert.ToInt64(dr["openingbalance"]);
                        objBankInformationDTO.pIsdebitcardapplicable = Convert.ToBoolean(dr["isdebitcardapplicable"]);
                        objBankInformationDTO.pIsupiapplicable = Convert.ToBoolean(dr["isupiapplicable"]);
                        objBankInformationDTO.pStatusname = Convert.ToString(dr["statusname"]);
                        objBankInformationDTO.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        objBankInformationDTO.pAcctountype = Convert.ToString(dr["acctountype"]);
                        objBankInformationDTO.popeningjvno = Convert.ToString(dr["openingjvno"]);
                        objBankInformationDTO.pOpeningBalanceType = Convert.ToString(dr["OpeningBalanceType"]);
                    }
                }
                objBankInformationDTO.lstBankInformationAddressDTO = ViewBankAddressInformation(objBankInformationDTO.pRecordid, con);
                objBankInformationDTO.lstBankdebitcarddtlsDTO = ViewBankDebitCardDetails(objBankInformationDTO.pRecordid, con);
                objBankInformationDTO.lstBankUPI = ViewBankUpiDetails(objBankInformationDTO.pRecordid, con);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return objBankInformationDTO;
        }

        private List<BankUPI> ViewBankUpiDetails(long pRecordid, string con)
        {
            lstBankUpi = new List<BankUPI>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,bankid,upiid,upiname,statusname,'OLD' as typeofoperation from tblmstbankupidtls t join tblmststatus ts on t.statusid=ts.statusid and upper(statusname)=upper('active') where bankid=" + pRecordid + ""))
                {
                    while (dr.Read())
                    {
                        BankUPI obj = new BankUPI();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pBankId = Convert.ToInt64(dr["bankid"]);
                        obj.pUpiid = Convert.ToString(dr["upiid"]);
                        obj.pUpiname = Convert.ToString(dr["upiname"]);
                        obj.pStatusname = Convert.ToString(dr["statusname"]);
                        obj.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        lstBankUpi.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankUpi;
        }

        private List<BankdebitcarddtlsDTO> ViewBankDebitCardDetails(long pRecordid, string con)
        {
            lstBankdebitcarddtls = new List<BankdebitcarddtlsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,bankid,cardnumber,cardname,statusname,'OLD' as typeofoperation,validfrom,validto from tblmstbankdebitcarddtls t join tblmststatus ts on t.statusid=ts.statusid and upper(statusname)=upper('active') where bankid=" + pRecordid + ""))
                {
                    while (dr.Read())
                    {
                        BankdebitcarddtlsDTO obj = new BankdebitcarddtlsDTO();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pBankId = Convert.ToInt64(dr["bankid"]);
                        obj.pCardNo = Convert.ToInt64(dr["cardnumber"]);
                        obj.pCardName = Convert.ToString(dr["cardname"]);
                        obj.pStatusname = Convert.ToString(dr["statusname"]);
                        obj.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        if ((dr["validfrom"].ToString()) != "" || !string.IsNullOrEmpty(dr["validfrom"].ToString()))
                        {
                            obj.pValidfrom = Convert.ToDateTime(dr["validfrom"]).ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            obj.pValidfrom = "";
                        }
                        if ((dr["validto"].ToString()) != "" || !string.IsNullOrEmpty(dr["validto"].ToString()))
                        {
                            obj.pValidto = Convert.ToDateTime(dr["validto"]).ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            obj.pValidto = "";
                        }
                        lstBankdebitcarddtls.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankdebitcarddtls;

        }

        public List<BankInformationAddressDTO> ViewBankAddressInformation(Int64 pRecordid, string con)
        {
            lstBankInformationAddress = new List<BankInformationAddressDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, "select recordid,bankid,address1,address2,city,t.district,coalesce(t3.districtid,0) districtid,t.state,coalesce(t2.stateid,0) stateid,t.country,coalesce(t1.countryid,0) countryid,pincode,statusname,'OLD' as typeofoperation from tblmstbankaddressdtls t join tblmststatus ts on t.statusid=ts.statusid and upper(statusname)=upper('active') left join tblmstcountry t1 on t.country=t1.country left join tblmststate t2 on t.state=t2.state left join tblmstdistrict t3 on t.district=t3.district where bankid =" + pRecordid + ""))
                {
                    while (dr.Read())
                    {
                        BankInformationAddressDTO obj = new BankInformationAddressDTO();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        obj.pBankId = Convert.ToInt64(dr["bankid"]);
                        obj.pAddress1 = Convert.ToString(dr["address1"]);
                        obj.pAddress2 = Convert.ToString(dr["address2"]);
                        obj.pcity = Convert.ToString(dr["city"]);
                        obj.pDistrict = Convert.ToString(dr["district"]);
                        obj.pState = Convert.ToString(dr["state"]);
                        obj.pCountry = Convert.ToString(dr["country"]);
                        obj.pPincode = Convert.ToString(dr["pincode"]);
                        obj.pStatusname = Convert.ToString(dr["statusname"]);
                        obj.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        obj.pstateid = Convert.ToInt64(dr["stateid"]);
                        obj.pcountryid = Convert.ToInt64(dr["countryid"]);
                        obj.pdistrictid = Convert.ToInt64(dr["districtid"]);
                        lstBankInformationAddress.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstBankInformationAddress;
        }

        public bool DeleteBankInformation(BankInformationDTO lstChequemanagement, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder BankDetailsDelete = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                BankDetailsDelete.Append("UPDATE tblmstbank set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + lstChequemanagement.pCreatedby + ",modifieddate=current_timestamp where recordid=" + lstChequemanagement.pRecordid + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, BankDetailsDelete.ToString());
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


    }
}
