using System;
using System.Collections.Generic;
using FinstaRepository.Interfaces.Accounting.Masters;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;
using Npgsql;
using HelperManager;
using System.Data;
using System.Text;
using System.Threading.Tasks;



namespace FinstaRepository.DataAccess.Accounting.Masters
{
    public class AccountMasterDAL : IAccountMaster
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        List<AccountTreeDTO> accounttreeList { get; set; }
        AccountingTransactionsDAL objJournalVoucher = null;
        JournalVoucherDTO objJournalVoucherDTO = null;
        JournalVoucherNewDTO objJournalVoucherNewDTO = null;
        PaymentsDTO objPaymentsDTO = null;
        PaymentsNewDTO objPaymentsNewDTO = null;
        List<AccountTreeNewDTO> _lstAccountsMasterDTO { set; get; }
        List<AccountsMasterTwotypeDTO> _lstAccountsMasterTwotypeDTO { set; get; }
        List<AccountsMasterThreetypeDTO> _lstAccountsMasterThreeTypeDTO { set; get; }
        List<AccountsMasterFourtypeDTO> _lstAccountsMasterFourTypeDTO { set; get; }
        List<AccountsMasterFivetypeDTO> _lstAccountsMasterFiveTypeDTO { set; get; }
        List<AccountsMasterTwotypeNewDTO> _lstAccountsMasterTwotypeNewDTO { set; get; }
        List<AccountsMasterThreetypeNewDTO> _lstAccountsMasterThreeTypeNewDTO { set; get; }
        List<AccountsMasterFourtypeNewDTO> _lstAccountsMasterFourTypeNewDTO { set; get; }
        List<AccountsMasterFivetypeNewDTO> _lstAccountsMasterFiveTypeNewDTO { set; get; }
        public List<AccountTreeDTO> GetAccountTreeDetails(string ConnectionString)
        {
            accounttreeList = new List<AccountTreeDTO>();
            try
            {
          
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails"))
                {

                    while (dr.Read())
                    {
                        AccountTreeDTO objaccounttreelist = new AccountTreeDTO();
                        objaccounttreelist.pRecordId = Convert.ToInt64(dr["recordid"]);
                        objaccounttreelist.pAccountid = Convert.ToInt64(dr["accountid"]);
                        objaccounttreelist.pAccountname = Convert.ToString(dr["accountname"]);
                        objaccounttreelist.pParentId = Convert.ToString(dr["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(dr["parentid"]);
                        objaccounttreelist.pParrentAccountname = Convert.ToString(dr["parentaccountname"]);
                        objaccounttreelist.pAccountBalance = Convert.ToDecimal(dr["accountbalance"]);
                        objaccounttreelist.pChracctype = Convert.ToString(dr["chracctype"]);
                        objaccounttreelist.pOpeningdate = Convert.ToString(dr["openingdate"]);
                        objaccounttreelist.pFontcolor = Convert.ToString(dr["fcolor"]);
                        objaccounttreelist.pAmounttype = Convert.ToString(dr["Amounttype"]);
                        objaccounttreelist.pRownum = Convert.ToInt64(dr["rowid"]);
                        objaccounttreelist.pLevel = Convert.ToInt64(dr["level"]);
                        objaccounttreelist.pHaschild = Convert.ToBoolean(dr["haschild"]);
                        accounttreeList.Add(objaccounttreelist);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return accounttreeList;
        }

        #region Account Tree By Sai Mahesh
        public async Task<List<AccountsTreeDTO>> GetAccountTree(string ConnectionString)
        {
            decimal seconchildsum = 0;
            decimal thirdchildsum = 0;
            decimal fourthchildsum = 0;
            decimal fifthchildsum = 0;
            List<AccountsTreeDTO> _lstAccountsMasterDTO = new List<AccountsTreeDTO>();
            await Task.Run(() =>
            {

                _lstAccountsMasterDTO = new List<AccountsTreeDTO>();
                string Parentid = string.Empty;

                try
                {
                    string Query = "SELECT recordid, accountid, accountname, parentid,parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status,openingdate, haschild FROM vwaccounttree where parentid is null order by accountid;";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {

                        while (dr.Read())
                        {
                            _lstAccountsMasterTwotypeNewDTO = new List<AccountsMasterTwotypeNewDTO>();
                            AccountsTreeDTO _objAccountTree = new AccountsTreeDTO();

                            _objAccountTree.parent_id = dr["parentid"].ToString();
                            if (_objAccountTree.parent_id == "") { Parentid = null; } else { }
                            _objAccountTree.account_id = dr["accountid"];
                            _objAccountTree.account_name = dr["accountname"];
                            _objAccountTree.account_balance = dr["accountbalance"].ToString();
                            _objAccountTree.Main_accountname = dr["accountname"];
                            if (_objAccountTree.account_balance == "") { _objAccountTree.account_balance = 0; }
                            _objAccountTree.chracc_type = dr["chracctype"];
                            _objAccountTree.Subcategorycreationstatus = dr["subcategory_creation_status"];

                            string Query2 = "SELECT recordid, accountid, accountname, parentid,parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status,openingdate, haschild FROM vwaccounttree where parentid = '" + _objAccountTree.account_id + "' order by accountid; ";


                            using (NpgsqlDataReader drTwo = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query2))
                            {

                                while (drTwo.Read())
                                {
                                    AccountsMasterTwotypeNewDTO _objAccountTwoTree = new AccountsMasterTwotypeNewDTO();
                                    _lstAccountsMasterThreeTypeNewDTO = new List<AccountsMasterThreetypeNewDTO>();
                                    _objAccountTwoTree.parent_id = drTwo["parentid"];
                                    _objAccountTwoTree.parent_account_name = dr["accountname"];
                                    _objAccountTwoTree.Main_accountname = dr["accountname"];
                                    _objAccountTwoTree.account_id = drTwo["accountid"];
                                    _objAccountTwoTree.account_name = drTwo["accountname"];
                                    _objAccountTwoTree.account_balance = drTwo["accountbalance"];
                                    _objAccountTwoTree.chracc_type = drTwo["chracctype"];
                                    _objAccountTwoTree.Subcategorycreationstatus = drTwo["subcategory_creation_status"];
                                    seconchildsum += Convert.ToDecimal(drTwo["accountbalance"]);


                                    string Query3 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status,openingdate, haschild FROM vwaccounttree where parentid = '" + _objAccountTwoTree.account_id + "' order by accountid;";

                                    using (NpgsqlDataReader drThree = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query3))
                                    {

                                        while (drThree.Read())
                                        {
                                            AccountsMasterThreetypeNewDTO _objAccountThreeTree = new AccountsMasterThreetypeNewDTO();
                                            _lstAccountsMasterFourTypeNewDTO = new List<AccountsMasterFourtypeNewDTO>();
                                            _objAccountThreeTree.parent_id = drThree["parentid"];
                                            _objAccountThreeTree.parent_account_name = drTwo["accountname"];
                                            _objAccountThreeTree.account_id = drThree["accountid"];
                                            _objAccountThreeTree.account_name = drThree["accountname"];
                                            _objAccountThreeTree.Main_accountname = dr["accountname"];
                                            _objAccountThreeTree.account_balance = drThree["accountbalance"];
                                            _objAccountThreeTree.chracc_type = drThree["chracctype"];
                                            _objAccountThreeTree.Subcategorycreationstatus = drThree["subcategory_creation_status"];

                                            thirdchildsum += Convert.ToDecimal(drThree["accountbalance"]);


                                            string Query4 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status,openingdate, haschild FROM vwaccounttree where parentid = '" + _objAccountThreeTree.account_id + "' order by accountid;";

                                            using (NpgsqlDataReader drFour = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query4))
                                            {

                                                while (drFour.Read())
                                                {
                                                    AccountsMasterFourtypeNewDTO _objAccountFourtype = new AccountsMasterFourtypeNewDTO();
                                                    _lstAccountsMasterFiveTypeNewDTO = new List<AccountsMasterFivetypeNewDTO>();
                                                    _objAccountFourtype.parent_id = drFour["parentid"];
                                                    _objAccountFourtype.parent_account_name = drThree["accountname"];
                                                    _objAccountFourtype.account_id = drFour["accountid"];
                                                    _objAccountFourtype.account_name = drFour["accountname"];
                                                    _objAccountFourtype.Main_accountname = dr["accountname"];
                                                    _objAccountFourtype.account_balance = drFour["accountbalance"];
                                                    _objAccountFourtype.chracc_type = drFour["chracctype"];
                                                    _objAccountFourtype.Subcategorycreationstatus = drFour["subcategory_creation_status"];

                                                    fourthchildsum += Convert.ToDecimal(drFour["accountbalance"]);

                                                    string Query5 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status,openingdate, haschild FROM vwaccounttree where parentid = '" + _objAccountFourtype.account_id + "' order by accountid;";


                                                    using (NpgsqlDataReader drFive = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query5))
                                                    {

                                                        while (drFive.Read())
                                                        {
                                                            AccountsMasterFivetypeNewDTO _objAccountFivetype = new AccountsMasterFivetypeNewDTO();
                                                            _objAccountFivetype.parent_id = drFive["parentid"];
                                                            _objAccountFivetype.parent_account_name = drFour["accountname"];
                                                            _objAccountFivetype.account_id = drFive["accountid"];
                                                            _objAccountFivetype.account_name = drFive["accountname"];
                                                            _objAccountFivetype.Main_accountname = dr["accountname"];
                                                            _objAccountFivetype.account_balance = drFive["accountbalance"];
                                                            _objAccountFivetype.chracc_type = drFive["chracctype"];
                                                            _objAccountFivetype.Subcategorycreationstatus = drFive["subcategory_creation_status"];
                                                            fifthchildsum += Convert.ToDecimal(drFive["accountbalance"]);
                                                            _lstAccountsMasterFiveTypeNewDTO.Add(_objAccountFivetype);
                                                            _objAccountFourtype.Children = _lstAccountsMasterFiveTypeNewDTO;

                                                        }

                                                    }
                                                    _objAccountFourtype.Children = _lstAccountsMasterFiveTypeNewDTO;
                                                    _lstAccountsMasterFourTypeNewDTO.Add(_objAccountFourtype);
                                                }

                                            }
                                            _objAccountThreeTree.Children = _lstAccountsMasterFourTypeNewDTO;
                                            _lstAccountsMasterThreeTypeNewDTO.Add(_objAccountThreeTree);
                                        }

                                    }
                                    _objAccountTwoTree.Children = _lstAccountsMasterThreeTypeNewDTO;
                                    _lstAccountsMasterTwotypeNewDTO.Add(_objAccountTwoTree);
                                }
                            }
                            _objAccountTree.Children = _lstAccountsMasterTwotypeNewDTO;
                            _objAccountTree.AccountHeadSum = seconchildsum + thirdchildsum + fourthchildsum + fifthchildsum;
                            _lstAccountsMasterDTO.Add(_objAccountTree);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {

                }
            });
            return _lstAccountsMasterDTO;
        }

        public async Task<List<AccountsTreeDTO>> AccountTreeSearch(string ConnectionString, string searchterm)
        {
            decimal seconchildsum = 0;
            decimal thirdchildsum = 0;
            decimal fourthchildsum = 0;
            decimal fifthchildsum = 0;
            List<AccountsTreeDTO> _lstAccountsMasterDTO = new List<AccountsTreeDTO>();
            string search_string = searchterm;
            await Task.Run(() =>
            {
                _lstAccountsMasterDTO = new List<AccountsTreeDTO>();
                string Parentid = string.Empty;

                try
                {
                    con = new NpgsqlConnection(ConnectionString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();

                    string strQuery = "select fn_accounttree_search('" + search_string + "');";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);

                    string Query = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status FROM temptblmstaccount where parentid is null order by accountid";

                    DataSet ds = new DataSet();
                    ds = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, Query);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        _lstAccountsMasterTwotypeNewDTO = new List<AccountsMasterTwotypeNewDTO>();
                        AccountsTreeDTO _objAccountTree = new AccountsTreeDTO();

                        _objAccountTree.parent_id = dr["parentid"].ToString();
                        if (_objAccountTree.parent_id == "") { Parentid = null; } else { }
                        _objAccountTree.account_id = dr["accountid"];
                        _objAccountTree.account_name = dr["accountname"];
                        _objAccountTree.account_balance = dr["accountbalance"].ToString();
                        _objAccountTree.parent_account_name = dr["parentaccountname"];
                        _objAccountTree.Main_accountname = dr["accountname"];
                        if (_objAccountTree.account_balance == "") { _objAccountTree.account_balance = 0; }
                        _objAccountTree.chracc_type = dr["chracctype"];
                        _objAccountTree.Subcategorycreationstatus = dr["subcategory_creation_status"];

                        string Query2 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status FROM temptblmstaccount where parentid = '" + _objAccountTree.account_id + "' order by accountid; ";

                        DataSet dsTwo = new DataSet();
                        dsTwo = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, Query2);
                        foreach (DataRow drTwo in dsTwo.Tables[0].Rows)
                        // while (drTwo.Read())
                        {
                            AccountsMasterTwotypeNewDTO _objAccountTwoTree = new AccountsMasterTwotypeNewDTO();
                            _lstAccountsMasterThreeTypeNewDTO = new List<AccountsMasterThreetypeNewDTO>();
                            _objAccountTwoTree.parent_id = drTwo["parentid"];
                            _objAccountTwoTree.parent_account_name = dr["accountname"];
                            _objAccountTwoTree.Main_accountname = dr["accountname"];
                            _objAccountTwoTree.account_id = drTwo["accountid"];
                            _objAccountTwoTree.account_name = drTwo["accountname"];
                            _objAccountTwoTree.account_balance = drTwo["accountbalance"];
                            _objAccountTwoTree.chracc_type = drTwo["chracctype"];
                            _objAccountTwoTree.Subcategorycreationstatus = drTwo["subcategory_creation_status"];
                            seconchildsum += Convert.ToDecimal(drTwo["accountbalance"]);


                            string Query3 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status FROM temptblmstaccount where parentid = '" + _objAccountTwoTree.account_id + "' order by accountid;";
                            DataSet dsThree = new DataSet();
                            dsThree = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, Query3);

                            foreach (DataRow drThree in dsThree.Tables[0].Rows)
                            //while (drThree.Read())
                            {
                                AccountsMasterThreetypeNewDTO _objAccountThreeTree = new AccountsMasterThreetypeNewDTO();
                                _lstAccountsMasterFourTypeNewDTO = new List<AccountsMasterFourtypeNewDTO>();
                                _objAccountThreeTree.parent_id = drThree["parentid"];
                                _objAccountThreeTree.parent_account_name = drTwo["accountname"];
                                _objAccountThreeTree.account_id = drThree["accountid"];
                                _objAccountThreeTree.account_name = drThree["accountname"];
                                _objAccountThreeTree.Main_accountname = dr["accountname"];
                                _objAccountThreeTree.account_balance = drThree["accountbalance"];
                                _objAccountThreeTree.chracc_type = drThree["chracctype"];
                                _objAccountThreeTree.Subcategorycreationstatus = drThree["subcategory_creation_status"];

                                thirdchildsum += Convert.ToDecimal(drThree["accountbalance"]);


                                string Query4 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status FROM temptblmstaccount where parentid = '" + _objAccountThreeTree.account_id + "' order by accountid;" +
                                "";
                                DataSet dsFour = new DataSet();
                                dsFour = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, Query4);
                                foreach (DataRow drFour in dsFour.Tables[0].Rows)
                                // while (drFour.Read())
                                {
                                    AccountsMasterFourtypeNewDTO _objAccountFourtype = new AccountsMasterFourtypeNewDTO();
                                    _lstAccountsMasterFiveTypeNewDTO = new List<AccountsMasterFivetypeNewDTO>();
                                    _objAccountFourtype.parent_id = drFour["parentid"];
                                    _objAccountFourtype.parent_account_name = drThree["accountname"];
                                    _objAccountFourtype.account_id = drFour["accountid"];
                                    _objAccountFourtype.account_name = drFour["accountname"];
                                    _objAccountFourtype.Main_accountname = dr["accountname"];
                                    _objAccountFourtype.account_balance = drFour["accountbalance"];
                                    _objAccountFourtype.chracc_type = drFour["chracctype"];
                                    _objAccountFourtype.Subcategorycreationstatus = drFour["subcategory_creation_status"];

                                    fourthchildsum += Convert.ToDecimal(drFour["accountbalance"]);

                                    string Query5 = "SELECT recordid, accountid, accountname, parentid, parentaccountname, acctype, chracctype, accountbalance, subcategory_creation_status FROM temptblmstaccount where parentid = '" + _objAccountFourtype.account_id + "' order by accountid";


                                    DataSet dsFive = new DataSet();
                                    dsFive = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, Query5);

                                    foreach (DataRow drFive in dsFive.Tables[0].Rows)
                                    {
                                        AccountsMasterFivetypeNewDTO _objAccountFivetype = new AccountsMasterFivetypeNewDTO();
                                        _objAccountFivetype.parent_id = drFive["parentid"];
                                        _objAccountFivetype.parent_account_name = drFour["accountname"];
                                        _objAccountFivetype.account_id = drFive["accountid"];
                                        _objAccountFivetype.account_name = drFive["accountname"];
                                        _objAccountFivetype.Main_accountname = dr["accountname"];
                                        _objAccountFivetype.account_balance = drFive["accountbalance"];
                                        _objAccountFivetype.chracc_type = drFive["chracctype"];
                                        _objAccountFivetype.Subcategorycreationstatus = drFive["subcategory_creation_status"];
                                        fifthchildsum += Convert.ToDecimal(drFive["accountbalance"]);
                                        _lstAccountsMasterFiveTypeNewDTO.Add(_objAccountFivetype);
                                        _objAccountFourtype.Children = _lstAccountsMasterFiveTypeNewDTO;
                                    }

                                    _objAccountFourtype.Children = _lstAccountsMasterFiveTypeNewDTO;
                                    _lstAccountsMasterFourTypeNewDTO.Add(_objAccountFourtype);
                                }

                                //}
                                _objAccountThreeTree.Children = _lstAccountsMasterFourTypeNewDTO;
                                _lstAccountsMasterThreeTypeNewDTO.Add(_objAccountThreeTree);
                            }

                            //}
                            _objAccountTwoTree.Children = _lstAccountsMasterThreeTypeNewDTO;
                            _lstAccountsMasterTwotypeNewDTO.Add(_objAccountTwoTree);
                        }
                        //}
                        _objAccountTree.Children = _lstAccountsMasterTwotypeNewDTO;
                        _objAccountTree.AccountHeadSum = seconchildsum + thirdchildsum + fourthchildsum + fifthchildsum;
                        _lstAccountsMasterDTO.Add(_objAccountTree);
                    }
                    // }
                    trans.Commit();
                }

                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Dispose();
                    throw;
                }
                finally
                {

                }
            });
            return _lstAccountsMasterDTO;
        }

        public bool SaveAccountHeads(AccountCreationNewDTO accountcreate, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            objJournalVoucher = new AccountingTransactionsDAL();
            objJournalVoucherNewDTO = new JournalVoucherNewDTO();
            string Jvdate = string.Empty;
            string Query = string.Empty;
            Int64 Accountid;
            Int64 OpeningBalanceAccountID;
            Int32 Openingbalance;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();



                OpeningBalanceAccountID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('OPENING BALANCE');"));





                //Accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select " + AddDoubleQuotes(GlobalSchema) + ".insertaccounts('" + GlobalSchema + "','" + Schema + "','" + accountcreate.pAccountname.ToString().ToUpper() + "', " + accountcreate.pParentId + ", '" + accountcreate.pChracctype + "','false');"));



                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pRecordId)))
                {

                    accountcreate.pRecordId = "0";
                }

                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pgstpercentage)))
                {

                    accountcreate.pgstpercentage = "0";

                }

                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pgsttype)))
                {

                    accountcreate.pgsttype = "";


                }
                //Accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + accountcreate.pAccountname.ToString().ToUpper() + "', " + accountcreate.pParentId + ", '" + accountcreate.pChracctype + "',false," + accountcreate.pisgstapplicable + "," + accountcreate.pgstpercentage + ",'" + accountcreate.pgsttype + "'," + accountcreate.pistdsapplicable + "," + accountcreate.pRecordId + ",'" + accountcreate.ptdscalculationtype + "');"));

                //               character varying,
                //                  bigint,
                //                  character varying,
                //bigint,
                //boolean,
                //integer,
                //character,
                //boolean,
                //integer,
                //character


                Accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + accountcreate.pAccountname.ToString().ToUpper() + "', " + accountcreate.pParentId + ", '" + accountcreate.pChracctype + "'," + accountcreate.pCreatedby + "," + accountcreate.pisgstapplicable + "," + accountcreate.pgstpercentage + ",'" + accountcreate.pgsttype + "'," + accountcreate.pistdsapplicable + "," + accountcreate.pRecordId + ",'" + accountcreate.ptdscalculationtype + "');"));
                //select insertaccounts('" + accountcreate.pAccountname.ToUpper() + "', " + accountcreate.pParentId + ", '" + accountcreate.pChracctype + "'," + accountcreate.pCreatedby + "," + accountcreate.pisgstapplicable + "," + accountcreate.pgstpercentage + ",'" + accountcreate.pgsttype + "'," + accountcreate.pistdsapplicable + "," + accountcreate.pRecordId + ",'" + accountcreate.ptdscalculationtype + "');
                //Query = "update " + AddDoubleQuotes(Schema) + ".tbl_mst_account set is_gst_applicable='" + accountcreate.pisgstapplicable + "', gst_percentage=" + accountcreate.pgstpercentage + ", gst_calculation_type='" + accountcreate.pgsttype + "', is_tds_applicable='" + accountcreate.pistdsapplicable + "', tds_section_id=" + accountcreate.pRecordId + ", tds_calculation_type='" + accountcreate.ptdscalculationtype + "' where account_id ='" + Accountid + "'";
                // NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);

                if (accountcreate.pOpeningamount.ToString() != "") { Openingbalance = Convert.ToInt32(accountcreate.pOpeningamount.ToString()); }
                else
                {
                    accountcreate.pOpeningamount = 0;
                    Openingbalance = Convert.ToInt32(accountcreate.pOpeningamount.ToString());
                }

                if (Openingbalance > 0)
                {
                    List<PaymentsNewDTO> _Paymentslist = new List<PaymentsNewDTO>();
                    accountcreate.pOpeningdate = accountcreate.pOpeningdate.ToString();
                    objJournalVoucherNewDTO.pjvdate = Convert.ToDateTime(accountcreate.pOpeningdate).ToString("dd-MM-yyyy");
                    objJournalVoucherNewDTO.ptypeofoperation = "CREATE";
                    objJournalVoucherNewDTO.pnarration = "OPENING JV";
                    objJournalVoucherNewDTO.pmodoftransaction = "A";
                    objJournalVoucherNewDTO.pCreatedby = accountcreate.pCreatedby;
                    //objJournalVoucherNewDTO.pbranchid = //getbranchId(connectionstring, GlobalSchema, Schema);
                    if (accountcreate.pOpeningBalanceType.ToString().ToUpper() == "CREDIT")
                    {
                        objPaymentsNewDTO = new PaymentsNewDTO();
                        objPaymentsNewDTO.ptranstype = "C";
                        objPaymentsNewDTO.ppartyid = null;
                        objPaymentsNewDTO.psubledgerid = Accountid;
                        objPaymentsNewDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsNewDTO);
                        objPaymentsNewDTO = new PaymentsNewDTO();
                        objPaymentsNewDTO.ptranstype = "D";
                        objPaymentsNewDTO.psubledgerid = OpeningBalanceAccountID;
                        objPaymentsNewDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsNewDTO);
                    }
                    else if (accountcreate.pOpeningBalanceType.ToString().ToUpper() == "DEBIT")
                    {
                        objPaymentsNewDTO = new PaymentsNewDTO();
                        objPaymentsNewDTO.ptranstype = "D";
                        objPaymentsNewDTO.ppartyid = null;
                        objPaymentsNewDTO.psubledgerid = Accountid;
                        objPaymentsNewDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsNewDTO);
                        objPaymentsNewDTO = new PaymentsNewDTO();
                        objPaymentsNewDTO.ptranstype = "C";
                        objPaymentsNewDTO.psubledgerid = OpeningBalanceAccountID;
                        objPaymentsNewDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsNewDTO);
                    }

                    objJournalVoucherNewDTO.pJournalVoucherlist = _Paymentslist;
                    string refjvnumber = "";
                    objJournalVoucher.SaveJournalVoucherNew(objJournalVoucherNewDTO, trans, out refjvnumber);

                    //   objJournalVoucher.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);


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
                    trans.Dispose();
                }
            }
            return isSaved;
        }
        #endregion

        public bool SaveAccountMaster(AccountCreationDTO accountcreate, string connectionstring)
        {
            bool isSaved = false;
            
            objJournalVoucher = new AccountingTransactionsDAL();
            objJournalVoucherDTO = new JournalVoucherDTO();
            string Jvdate = string.Empty;
            string Query = string.Empty;
            Int64 Accountid;
            Int64 OpeningBalanceAccountID;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                OpeningBalanceAccountID = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select accountid from tblmstaccounts  where accountname  in('OPENING BALANCE');"));
                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pRecordId)))
                {

                    accountcreate.pRecordId =0;
                }

                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pgstpercentage)))
                {

                    accountcreate.pgstpercentage = "0";

                }

                if (string.IsNullOrEmpty(Convert.ToString(accountcreate.pgsttype)))
                {

                    accountcreate.pgsttype = "";


                }
                Accountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select insertaccounts('" + accountcreate.pAccountname.ToUpper() + "', " + accountcreate.pParentId + ", '" + accountcreate.pChracctype + "'," + accountcreate.pCreatedby + "," + accountcreate.pisgstapplicable + "," + accountcreate.pgstpercentage + ",'" + accountcreate.pgsttype + "'," + accountcreate.pistdsapplicable + "," + accountcreate.pRecordId + ",'" + accountcreate.ptdscalculationtype + "');"));
                //Query = "select insertaccounts('" + accountcreate.pAccountname.ToUpper() + "', " + accountcreate.pParentId + ", '"+accountcreate.pChracctype+"'," + accountcreate.pCreatedby + ");";
                //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);

                if (accountcreate.pOpeningamount > 0)
                {
                    List<PaymentsDTO> _Paymentslist = new List<PaymentsDTO>();

                    objJournalVoucherDTO.pjvdate =Convert.ToDateTime(accountcreate.pOpeningdate).ToString("dd-MM-yyyy");
                    objJournalVoucherDTO.ptypeofoperation = "CREATE";
                    objJournalVoucherDTO.pnarration = "OPENING JV";
                    objJournalVoucherDTO.pmodoftransaction = "AUTO";
                    objJournalVoucherDTO.pCreatedby = accountcreate.pCreatedby;
                    if (accountcreate.pOpeningBalanceType.ToUpper() == "CREDIT")
                    {
                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.ptranstype = "C";
                        objPaymentsDTO.psubledgerid = Accountid;
                        objPaymentsDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsDTO);
                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.ptranstype = "D";
                        objPaymentsDTO.psubledgerid = OpeningBalanceAccountID;
                        objPaymentsDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsDTO);
                    }
                    else if (accountcreate.pOpeningBalanceType.ToUpper() == "DEBIT")
                    {
                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.ptranstype = "D";
                        objPaymentsDTO.psubledgerid = Accountid;
                        objPaymentsDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsDTO);
                        objPaymentsDTO = new PaymentsDTO();
                        objPaymentsDTO.ptranstype = "C";
                        objPaymentsDTO.psubledgerid = OpeningBalanceAccountID;
                        objPaymentsDTO.pamount = accountcreate.pOpeningamount;
                        _Paymentslist.Add(objPaymentsDTO);
                    }

                    objJournalVoucherDTO.pJournalVoucherlist = _Paymentslist;
                    string refjvnumber = "";
                    objJournalVoucher.SaveJournalVoucher(objJournalVoucherDTO, trans, out refjvnumber);
                    
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

        public int checkAccountnameDuplicates(string Accountname, string AccountType, int Parentid,string connectionstring)
        {
            int count = 0;
            try
            {
                if (!string.IsNullOrEmpty(Accountname))
                {
                    if (AccountType != "3") {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstaccounts where upper(accountname)='" + Accountname.ToUpper() + "'"));
                    }
                    else if(AccountType == "3")
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstaccounts where upper(accountname)='" + Accountname.ToUpper() + "' and parentid="+Parentid+""));
                    }

                   
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;
        }
        #region Account Tree New
        public async Task<List<AccountTreeNewDTO>> GetAccountTreeNewDetails(string ConnectionString)
        {
            List<AccountTreeNewDTO> accounttreeList = new List<AccountTreeNewDTO>();
            await Task.Run(() =>
            {
                
            try
            {

                    DataTable dt = new DataTable();
                    dt = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails").Tables[0];
                    DataTable dt1 = new DataTable();
                    DataRow[] dtrow = dt.Select("parentid is null");
                    if(dtrow.Length>0)
                     dt1 = dtrow.CopyToDataTable();
                    using (DataTableReader dr = new DataTableReader(dt1))
                    {

                        while (dr.Read())
                        {

                            //NpgsqlDataReader dr2=dr.to
                            _lstAccountsMasterTwotypeDTO = new List<AccountsMasterTwotypeDTO>();
                            AccountTreeNewDTO objaccounttreelist = new AccountTreeNewDTO();
                            objaccounttreelist.pRecordId = Convert.ToInt64(dr["recordid"]);
                            objaccounttreelist.pAccountid = Convert.ToInt64(dr["accountid"]);
                            objaccounttreelist.pAccountname = Convert.ToString(dr["accountname"]);
                            objaccounttreelist.Main_accountname = dr["accountname"];
                            objaccounttreelist.pParentId = Convert.ToString(dr["parentid"]) == "" ? null : dr["parentid"];
                            objaccounttreelist.pParrentAccountname = Convert.ToString(dr["parentaccountname"]);
                            objaccounttreelist.pAccountBalance = Convert.ToDecimal(dr["accountbalance"]);
                            objaccounttreelist.pChracctype = Convert.ToString(dr["chracctype"]);
                            objaccounttreelist.pOpeningdate = Convert.ToString(dr["openingdate"]);
                            objaccounttreelist.pFontcolor = Convert.ToString(dr["fcolor"]);
                            objaccounttreelist.pAmounttype = Convert.ToString(dr["Amounttype"]);
                            objaccounttreelist.pRownum = Convert.ToInt64(dr["rowid"]);
                            objaccounttreelist.pLevel = Convert.ToInt64(dr["level"]);
                            objaccounttreelist.pHaschild = Convert.ToBoolean(dr["haschild"]);
                            DataTable dtTwo = new DataTable();
                            DataRow[] dtrowTwo = dt.Select("parentid=" + objaccounttreelist.pAccountid + "");
                            if(dtrowTwo.Length>0)
                             dtTwo = dtrowTwo.CopyToDataTable();
                            using (DataTableReader drTwo = new DataTableReader(dtTwo))
                            {
                                while (drTwo.Read())
                                {
                                    AccountsMasterTwotypeDTO _objAccountTwoTree = new AccountsMasterTwotypeDTO();
                                    _lstAccountsMasterThreeTypeDTO = new List<AccountsMasterThreetypeDTO>();
                                    _objAccountTwoTree.pRecordId = Convert.ToInt64(drTwo["recordid"]);
                                    _objAccountTwoTree.pAccountid = Convert.ToInt64(drTwo["accountid"]);
                                    _objAccountTwoTree.pAccountname = Convert.ToString(drTwo["accountname"]);
                                    _objAccountTwoTree.Main_accountname = dr["accountname"];
                                    _objAccountTwoTree.pParentId = Convert.ToString(drTwo["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drTwo["parentid"]);
                                    _objAccountTwoTree.pParrentAccountname = Convert.ToString(drTwo["parentaccountname"]);
                                    _objAccountTwoTree.pAccountBalance = Convert.ToDecimal(drTwo["accountbalance"]);
                                    _objAccountTwoTree.pChracctype = Convert.ToString(drTwo["chracctype"]);
                                    _objAccountTwoTree.pOpeningdate = Convert.ToString(drTwo["openingdate"]);
                                    _objAccountTwoTree.pFontcolor = Convert.ToString(drTwo["fcolor"]);
                                    _objAccountTwoTree.pAmounttype = Convert.ToString(drTwo["Amounttype"]);
                                    _objAccountTwoTree.pRownum = Convert.ToInt64(drTwo["rowid"]);
                                    _objAccountTwoTree.pLevel = Convert.ToInt64(drTwo["level"]);
                                    _objAccountTwoTree.pHaschild = Convert.ToBoolean(drTwo["haschild"]);
                                    DataTable dtThree = new DataTable();
                                    DataRow[] dtrowThree = dt.Select("parentid=" + _objAccountTwoTree.pAccountid + "");
                                    if(dtrowThree.Length>0)
                                     dtThree = dtrowThree.CopyToDataTable();
                                    using (DataTableReader drThree = new DataTableReader(dtThree))
                                    {
                                    //    using (NpgsqlDataReader drThree = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountTwoTree.pAccountid + ""))
                                    //{
                                        while (drThree.Read())
                                        {
                                            AccountsMasterThreetypeDTO _objAccountThreeTree = new AccountsMasterThreetypeDTO();
                                            _lstAccountsMasterFourTypeDTO = new List<AccountsMasterFourtypeDTO>();
                                            _objAccountThreeTree.pRecordId = Convert.ToInt64(drThree["recordid"]);
                                            _objAccountThreeTree.pAccountid = Convert.ToInt64(drThree["accountid"]);
                                            _objAccountThreeTree.pAccountname = Convert.ToString(drThree["accountname"]);
                                            _objAccountThreeTree.Main_accountname = dr["accountname"];
                                            _objAccountThreeTree.pParentId = Convert.ToString(drThree["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drThree["parentid"]);
                                            _objAccountThreeTree.pParrentAccountname = Convert.ToString(drThree["parentaccountname"]);
                                            _objAccountThreeTree.pAccountBalance = Convert.ToDecimal(drThree["accountbalance"]);
                                            _objAccountThreeTree.pChracctype = Convert.ToString(drThree["chracctype"]);
                                            _objAccountThreeTree.pOpeningdate = Convert.ToString(drThree["openingdate"]);
                                            _objAccountThreeTree.pFontcolor = Convert.ToString(drThree["fcolor"]);
                                            _objAccountThreeTree.pAmounttype = Convert.ToString(drThree["Amounttype"]);
                                            _objAccountThreeTree.pRownum = Convert.ToInt64(drThree["rowid"]);
                                            _objAccountThreeTree.pLevel = Convert.ToInt64(drThree["level"]);
                                            _objAccountThreeTree.pHaschild = Convert.ToBoolean(drThree["haschild"]);
                                            DataTable dtFour = new DataTable();
                                            DataRow[] dtrowFour = dt.Select("parentid=" + _objAccountThreeTree.pAccountid + "");
                                            if(dtrowFour.Length>0)
                                             dtFour = dtrowFour.CopyToDataTable();
                                            using (DataTableReader drFour = new DataTableReader(dtFour))
                                            {
                                            //    using (NpgsqlDataReader drFour = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountThreeTree.pAccountid + ""))
                                            //{
                                                while (drFour.Read())
                                                {
                                                    AccountsMasterFourtypeDTO _objAccountFourtype = new AccountsMasterFourtypeDTO();
                                                    _lstAccountsMasterFiveTypeDTO = new List<AccountsMasterFivetypeDTO>();
                                                    _objAccountFourtype.pRecordId = Convert.ToInt64(drFour["recordid"]);
                                                    _objAccountFourtype.pAccountid = Convert.ToInt64(drFour["accountid"]);
                                                    _objAccountFourtype.pAccountname = Convert.ToString(drFour["accountname"]);
                                                    _objAccountFourtype.Main_accountname = dr["accountname"];
                                                    _objAccountFourtype.pParentId = Convert.ToString(drFour["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drFour["parentid"]);
                                                    _objAccountFourtype.pParrentAccountname = Convert.ToString(drFour["parentaccountname"]);
                                                    _objAccountFourtype.pAccountBalance = Convert.ToDecimal(drFour["accountbalance"]);
                                                    _objAccountFourtype.pChracctype = Convert.ToString(drFour["chracctype"]);
                                                    _objAccountFourtype.pOpeningdate = Convert.ToString(drFour["openingdate"]);
                                                    _objAccountFourtype.pFontcolor = Convert.ToString(drFour["fcolor"]);
                                                    _objAccountFourtype.pAmounttype = Convert.ToString(drFour["Amounttype"]);
                                                    _objAccountFourtype.pRownum = Convert.ToInt64(drFour["rowid"]);
                                                    _objAccountFourtype.pLevel = Convert.ToInt64(drFour["level"]);
                                                    _objAccountFourtype.pHaschild = Convert.ToBoolean(drFour["haschild"]);
                                                    DataTable dtFive = new DataTable();
                                                    DataRow[] dtrowFive = dt.Select("parentid=" + _objAccountFourtype.pAccountid + "");
                                                    if(dtrowFive.Length>0)
                                                     dtFive = dtrowFive.CopyToDataTable();
                                                    using (DataTableReader drFive = new DataTableReader(dtFive))
                                                    {
                                                        //using (NpgsqlDataReader drFive = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountFourtype.pAccountid + ""))
                                                        //{
                                                        while (drFive.Read())
                                                        {
                                                            AccountsMasterFivetypeDTO _objAccountFivetype = new AccountsMasterFivetypeDTO();
                                                            _objAccountFivetype.pRecordId = Convert.ToInt64(drFive["recordid"]);
                                                            _objAccountFivetype.pAccountid = Convert.ToInt64(drFive["accountid"]);
                                                            _objAccountFivetype.pAccountname = Convert.ToString(drFive["accountname"]);
                                                            _objAccountFivetype.Main_accountname = dr["accountname"];
                                                            _objAccountFivetype.pParentId = Convert.ToString(drFive["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drFive["parentid"]);
                                                            _objAccountFivetype.pParrentAccountname = Convert.ToString(drFive["parentaccountname"]);
                                                            _objAccountFivetype.pAccountBalance = Convert.ToDecimal(drFive["accountbalance"]);
                                                            _objAccountFivetype.pChracctype = Convert.ToString(drFive["chracctype"]);
                                                            _objAccountFivetype.pOpeningdate = Convert.ToString(drFive["openingdate"]);
                                                            _objAccountFivetype.pFontcolor = Convert.ToString(drFive["fcolor"]);
                                                            _objAccountFivetype.pAmounttype = Convert.ToString(drFive["Amounttype"]);
                                                            _objAccountFivetype.pRownum = Convert.ToInt64(drFive["rowid"]);
                                                            _objAccountFivetype.pLevel = Convert.ToInt64(drFive["level"]);
                                                            _objAccountFivetype.pHaschild = Convert.ToBoolean(drFive["haschild"]);
                                                            _lstAccountsMasterFiveTypeDTO.Add(_objAccountFivetype);
                                                            _objAccountFourtype.Children = _lstAccountsMasterFiveTypeDTO;
                                                        }
                                                    }
                                                    _objAccountFourtype.Children = _lstAccountsMasterFiveTypeDTO;
                                                    _lstAccountsMasterFourTypeDTO.Add(_objAccountFourtype);
                                                }
                                            }
                                            _objAccountThreeTree.Children = _lstAccountsMasterFourTypeDTO;
                                            _lstAccountsMasterThreeTypeDTO.Add(_objAccountThreeTree);
                                        }
                                    }
                                    _objAccountTwoTree.Children = _lstAccountsMasterThreeTypeDTO;
                                    _lstAccountsMasterTwotypeDTO.Add(_objAccountTwoTree);
                                }
                            }
                            objaccounttreelist.Children = _lstAccountsMasterTwotypeDTO;
                            accounttreeList.Add(objaccounttreelist);
                        }
                    }
                    //        DataTable dt = new DataTable();
                    //         dt = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails").Tables[0];
                    //        DataRow[] drd = dt.Select("parentid is null");
                    //        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid is null"))
                    //    {

                    //        while (dr.Read())
                    //        {

                    //                //NpgsqlDataReader dr2=dr.to
                    //                _lstAccountsMasterTwotypeDTO = new List<AccountsMasterTwotypeDTO>();
                    //            AccountTreeNewDTO objaccounttreelist = new AccountTreeNewDTO();
                    //            objaccounttreelist.pRecordId = Convert.ToInt64(dr["recordid"]);
                    //            objaccounttreelist.pAccountid = Convert.ToInt64(dr["accountid"]);
                    //            objaccounttreelist.pAccountname = Convert.ToString(dr["accountname"]);
                    //                objaccounttreelist.Main_accountname = dr["accountname"];
                    //                objaccounttreelist.pParentId = Convert.ToString(dr["parentid"]) == "" ?null :dr["parentid"];
                    //            objaccounttreelist.pParrentAccountname = Convert.ToString(dr["parentaccountname"]);
                    //            objaccounttreelist.pAccountBalance = Convert.ToDecimal(dr["accountbalance"]);
                    //            objaccounttreelist.pChracctype = Convert.ToString(dr["chracctype"]);
                    //            objaccounttreelist.pOpeningdate = Convert.ToString(dr["openingdate"]);
                    //            objaccounttreelist.pFontcolor = Convert.ToString(dr["fcolor"]);
                    //            objaccounttreelist.pAmounttype = Convert.ToString(dr["Amounttype"]);
                    //            objaccounttreelist.pRownum = Convert.ToInt64(dr["rowid"]);
                    //            objaccounttreelist.pLevel = Convert.ToInt64(dr["level"]);
                    //            objaccounttreelist.pHaschild = Convert.ToBoolean(dr["haschild"]);

                    //                using (NpgsqlDataReader drTwo = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid="+ objaccounttreelist.pAccountid + ""))
                    //            {
                    //                while (drTwo.Read())
                    //                {
                    //                    AccountsMasterTwotypeDTO _objAccountTwoTree = new AccountsMasterTwotypeDTO();
                    //                        _lstAccountsMasterThreeTypeDTO = new List<AccountsMasterThreetypeDTO>();
                    //                        _objAccountTwoTree.pRecordId = Convert.ToInt64(drTwo["recordid"]);
                    //                    _objAccountTwoTree.pAccountid = Convert.ToInt64(drTwo["accountid"]);
                    //                    _objAccountTwoTree.pAccountname = Convert.ToString(drTwo["accountname"]);
                    //                        _objAccountTwoTree.Main_accountname = dr["accountname"];
                    //                        _objAccountTwoTree.pParentId = Convert.ToString(drTwo["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drTwo["parentid"]);
                    //                    _objAccountTwoTree.pParrentAccountname = Convert.ToString(drTwo["parentaccountname"]);
                    //                    _objAccountTwoTree.pAccountBalance = Convert.ToDecimal(drTwo["accountbalance"]);
                    //                    _objAccountTwoTree.pChracctype = Convert.ToString(drTwo["chracctype"]);
                    //                    _objAccountTwoTree.pOpeningdate = Convert.ToString(drTwo["openingdate"]);
                    //                    _objAccountTwoTree.pFontcolor = Convert.ToString(drTwo["fcolor"]);
                    //                    _objAccountTwoTree.pAmounttype = Convert.ToString(drTwo["Amounttype"]);
                    //                    _objAccountTwoTree.pRownum = Convert.ToInt64(drTwo["rowid"]);
                    //                    _objAccountTwoTree.pLevel = Convert.ToInt64(drTwo["level"]);
                    //                    _objAccountTwoTree.pHaschild = Convert.ToBoolean(drTwo["haschild"]);
                    //                    using (NpgsqlDataReader drThree = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountTwoTree.pAccountid + ""))
                    //                    {
                    //                        while (drThree.Read())
                    //                        {
                    //                            AccountsMasterThreetypeDTO _objAccountThreeTree = new AccountsMasterThreetypeDTO();
                    //                                _lstAccountsMasterFourTypeDTO = new List<AccountsMasterFourtypeDTO>();
                    //                                _objAccountThreeTree.pRecordId = Convert.ToInt64(drThree["recordid"]);
                    //                            _objAccountThreeTree.pAccountid = Convert.ToInt64(drThree["accountid"]);
                    //                            _objAccountThreeTree.pAccountname = Convert.ToString(drThree["accountname"]);
                    //                                _objAccountThreeTree.Main_accountname = dr["accountname"];
                    //                                _objAccountThreeTree.pParentId = Convert.ToString(drThree["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drThree["parentid"]);
                    //                            _objAccountThreeTree.pParrentAccountname = Convert.ToString(drThree["parentaccountname"]);
                    //                            _objAccountThreeTree.pAccountBalance = Convert.ToDecimal(drThree["accountbalance"]);
                    //                            _objAccountThreeTree.pChracctype = Convert.ToString(drThree["chracctype"]);
                    //                            _objAccountThreeTree.pOpeningdate = Convert.ToString(drThree["openingdate"]);
                    //                            _objAccountThreeTree.pFontcolor = Convert.ToString(drThree["fcolor"]);
                    //                            _objAccountThreeTree.pAmounttype = Convert.ToString(drThree["Amounttype"]);
                    //                            _objAccountThreeTree.pRownum = Convert.ToInt64(drThree["rowid"]);
                    //                            _objAccountThreeTree.pLevel = Convert.ToInt64(drThree["level"]);
                    //                            _objAccountThreeTree.pHaschild = Convert.ToBoolean(drThree["haschild"]);
                    //                            using (NpgsqlDataReader drFour = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountThreeTree.pAccountid + ""))
                    //                            {
                    //                                while (drFour.Read())
                    //                                {
                    //                                    AccountsMasterFourtypeDTO _objAccountFourtype = new AccountsMasterFourtypeDTO();
                    //                                        _lstAccountsMasterFiveTypeDTO = new List<AccountsMasterFivetypeDTO>();
                    //                                        _objAccountFourtype.pRecordId = Convert.ToInt64(drFour["recordid"]);
                    //                                    _objAccountFourtype.pAccountid = Convert.ToInt64(drFour["accountid"]);
                    //                                    _objAccountFourtype.pAccountname = Convert.ToString(drFour["accountname"]);
                    //                                        _objAccountFourtype.Main_accountname = dr["accountname"];
                    //                                        _objAccountFourtype.pParentId = Convert.ToString(drFour["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drFour["parentid"]);
                    //                                    _objAccountFourtype.pParrentAccountname = Convert.ToString(drFour["parentaccountname"]);
                    //                                    _objAccountFourtype.pAccountBalance = Convert.ToDecimal(drFour["accountbalance"]);
                    //                                    _objAccountFourtype.pChracctype = Convert.ToString(drFour["chracctype"]);
                    //                                    _objAccountFourtype.pOpeningdate = Convert.ToString(drFour["openingdate"]);
                    //                                    _objAccountFourtype.pFontcolor = Convert.ToString(drFour["fcolor"]);
                    //                                    _objAccountFourtype.pAmounttype = Convert.ToString(drFour["Amounttype"]);
                    //                                    _objAccountFourtype.pRownum = Convert.ToInt64(drFour["rowid"]);
                    //                                    _objAccountFourtype.pLevel = Convert.ToInt64(drFour["level"]);
                    //                                    _objAccountFourtype.pHaschild = Convert.ToBoolean(drFour["haschild"]);
                    //                                    using (NpgsqlDataReader drFive = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from vwaccounttreedetails where parentid=" + _objAccountFourtype.pAccountid + ""))
                    //                                    {
                    //                                        while (drFive.Read())
                    //                                        {
                    //                                            AccountsMasterFivetypeDTO _objAccountFivetype = new AccountsMasterFivetypeDTO();
                    //                                            _objAccountFivetype.pRecordId = Convert.ToInt64(drFive["recordid"]);
                    //                                            _objAccountFivetype.pAccountid = Convert.ToInt64(drFive["accountid"]);
                    //                                            _objAccountFivetype.pAccountname = Convert.ToString(drFive["accountname"]);
                    //                                                _objAccountFivetype.Main_accountname = dr["accountname"];
                    //                                                _objAccountFivetype.pParentId = Convert.ToString(drFive["parentid"]) == "" ? (Int64?)null : Convert.ToInt64(drFive["parentid"]);
                    //                                            _objAccountFivetype.pParrentAccountname = Convert.ToString(drFive["parentaccountname"]);
                    //                                            _objAccountFivetype.pAccountBalance = Convert.ToDecimal(drFive["accountbalance"]);
                    //                                            _objAccountFivetype.pChracctype = Convert.ToString(drFive["chracctype"]);
                    //                                            _objAccountFivetype.pOpeningdate = Convert.ToString(drFive["openingdate"]);
                    //                                            _objAccountFivetype.pFontcolor = Convert.ToString(drFive["fcolor"]);
                    //                                            _objAccountFivetype.pAmounttype = Convert.ToString(drFive["Amounttype"]);
                    //                                            _objAccountFivetype.pRownum = Convert.ToInt64(drFive["rowid"]);
                    //                                            _objAccountFivetype.pLevel = Convert.ToInt64(drFive["level"]);
                    //                                            _objAccountFivetype.pHaschild = Convert.ToBoolean(drFive["haschild"]);
                    //                                            _lstAccountsMasterFiveTypeDTO.Add(_objAccountFivetype);
                    //                                            _objAccountFourtype.Children = _lstAccountsMasterFiveTypeDTO;
                    //                                        }
                    //                                        }
                    //                                    _objAccountFourtype.Children = _lstAccountsMasterFiveTypeDTO;
                    //                                    _lstAccountsMasterFourTypeDTO.Add(_objAccountFourtype);
                    //                                }
                    //                                }
                    //                            _objAccountThreeTree.Children = _lstAccountsMasterFourTypeDTO;
                    //                            _lstAccountsMasterThreeTypeDTO.Add(_objAccountThreeTree);
                    //                        }
                    //                        }
                    //                    _objAccountTwoTree.Children = _lstAccountsMasterThreeTypeDTO;
                    //                    _lstAccountsMasterTwotypeDTO.Add(_objAccountTwoTree);
                    //                }
                    //                }
                    //            objaccounttreelist.Children = _lstAccountsMasterTwotypeDTO;
                    //            accounttreeList.Add(objaccounttreelist);
                    //        }
                    //    }
                }
            catch (Exception)
            {

                throw;
            }
            });
            return accounttreeList;
        }

        public List<TdsSectionNewDTO> getTdsSectionNo(string ConnectionString)
        {
            List<TdsSectionNewDTO> lstTdsSectionDetails = new List<TdsSectionNewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,tdssection,tdspercentage from tblmsttdssections where statusid =1 order by tdssection"))
                {
                    while (dr.Read())
                    {
                        TdsSectionNewDTO objTdsSectionDetails = new TdsSectionNewDTO();
                        objTdsSectionDetails.pRecordid = dr["recordid"];
                        objTdsSectionDetails.pTdsSection = dr["tdssection"];
                        objTdsSectionDetails.pTdsPercentage = dr["tdspercentage"];
                        lstTdsSectionDetails.Add(objTdsSectionDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstTdsSectionDetails;
        }
        #endregion
    }
}
