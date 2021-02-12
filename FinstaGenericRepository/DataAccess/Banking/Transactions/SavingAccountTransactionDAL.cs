using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Transactions;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Transactions;
namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class SavingAccountTransactionDAL : SettingsDAL, ISavingAccountTransaction
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public ContactDetails _ContactDetailsList { get; set; }
        public List<MemberDetails> _MemberDetails { set; get; }
        public List<SavingAccountConfigBind> _SavingAccountConfigList { set; get; }
        public List<SavingAccountConfigDetailsBind> _SavingAccountConfigDetailsList { set; get; }
        public List<SavingAccountTransactionMainGrid> _SavingAccountTransactionMainGridList { get; set; }
        public List<SavingAccountTransactionSave> _SavingAccountTransactionList { set; get; }
        public List<SavingAccountTransactionDTO> listSavingAccountTransaction { get; set; }

        ShareApplicationDAL objshareApplicationDAL { get; set; }

        public async Task<List<SavingAccountConfigBind>> GetSavingAccountDetails(long Membertypeid, string Applicanttype, string ConnectionString)
        {

            await Task.Run(() =>
            {
                try
                {
                    _SavingAccountConfigList = new List<SavingAccountConfigBind>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t.savingconfigid, t.savingaccname, t.savingaccnamecode  from tblmstSavingAccountConfig t join tblmstSavingAccountConfigdetails tt on t.savingconfigid=tt.savingconfigid where membertypeid=" + Membertypeid + " and applicanttype='" + Applicanttype + "' and t.statusid=" + Convert.ToInt32(Status.Active) + " and tt.statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            var _SavingAccountConfigBind = new SavingAccountConfigBind
                            {
                                pSavingconfigid = Convert.ToInt64(dr["savingconfigid"]),
                                pSavingaccname = Convert.ToString(dr["savingaccname"]),
                                pSavingaccnamecode = Convert.ToString(dr["savingaccnamecode"])
                            };
                            _SavingAccountConfigList.Add(_SavingAccountConfigBind);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            });
            return _SavingAccountConfigList;
        }

        public async Task<List<SavingAccountConfigDetailsBind>> GetSavingAccountConfigDetails(long Savingconfigid, long Membertypeid, string Applicanttype, string ConnectionString)
        {

            await Task.Run(() =>
            {
                try
                {
                    _SavingAccountConfigDetailsList = new List<SavingAccountConfigDetailsBind>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingaccnamecode,minopenamount,interestpayout,issavingspayinapplicable,savingspayinmode,savingmindepositamount,savingmaxdepositamount,iswithdrawallimitapplicable,maxwithdrawallimit,minmaintainbalance  from tblmstSavingAccountConfig t join tblmstSavingAccountConfigdetails tt on t.savingconfigid=tt.savingconfigid where t.savingconfigid=" + Savingconfigid + " and membertypeid=" + Membertypeid + " and applicanttype='" + Applicanttype + "' and t.statusid=" + Convert.ToInt32(Status.Active) + " and tt.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _SavingAccountConfigDetailsBind = new SavingAccountConfigDetailsBind
                            {
                                pSavingaccnamecode = Convert.ToString(dr["savingaccnamecode"]),
                                pMinopenamount = Convert.ToDecimal(dr["minopenamount"]),
                                pInterestpayout = Convert.ToString(dr["interestpayout"]),
                                pIssavingspayinapplicable = Convert.ToBoolean(dr["issavingspayinapplicable"]),
                                pSavingspayinmode = Convert.ToString(dr["savingspayinmode"]),
                                pSavingmindepositamount = Convert.ToDecimal(dr["savingmindepositamount"]),
                                pSavingmaxdepositamount = Convert.ToDecimal(dr["savingmaxdepositamount"]),
                                pIswithdrawallimitapplicable = Convert.ToBoolean(dr["iswithdrawallimitapplicable"]),
                                pMaxwithdrawallimit = Convert.ToDecimal(dr["maxwithdrawallimit"]),
                                pMinmaintainbalance = Convert.ToDecimal(dr["minmaintainbalance"]),
                            };
                            _SavingAccountConfigDetailsList.Add(_SavingAccountConfigDetailsBind);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            });
            return _SavingAccountConfigDetailsList;
        }

        public async Task<ContactDetails> GetContactDetails(long MemberID, string ConnectionString)
        {

            await Task.Run(() =>
            {
                try
                {

                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid, membercode, te.contactid, contacttype, te.contactreferenceid, membername, coalesce(membertype, '') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, contactnumber, emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontactpersondetails tc on tc.contactid = te.contactid where upper(priority) = 'PRIMARY' and memberid = " + MemberID + " and te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername;"))
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid, membercode, te.contactid, te.contacttype, te.contactreferenceid, membername, coalesce(membertype, '') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, businessentitycontactno as contactnumber, businessentityemailid as emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tc on tc.contactid=te.contactid where memberid = " + MemberID + " and te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername; "))
                    {
                        while (dr.Read())
                        {
                            _ContactDetailsList = new ContactDetails
                            {
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContacttype = Convert.ToString(dr["contacttype"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pContactno = Convert.ToString(dr["contactnumber"])
                            };

                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            });
            return _ContactDetailsList;
        }

        public async Task<List<MemberDetails>> GetMemberDetails(long MemberTypeId, string contactType, string ConnectionString)
        {

            await Task.Run(() =>
            {
                try
                {
                    _MemberDetails = new List<MemberDetails>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,contacttype from tblmstmembers where contacttype='" + contactType + "' and membertypeid=" + MemberTypeId + " and statusid=" + Convert.ToInt32(Status.Active) + ""))
                    {
                        while (dr.Read())
                        {
                            var _MemberDetailsBind = new MemberDetails
                            {
                                pMemberid = Convert.ToInt64(dr["memberid"]),
                                pMembercode = Convert.ToString(dr["membercode"]),
                                pMembername = Convert.ToString(dr["membername"]),
                                pContacttype = Convert.ToString(dr["contacttype"])
                            };
                            _MemberDetails.Add(_MemberDetailsBind);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            });
            return _MemberDetails;
        }

        public string SaveSavingAccountTransaction(SavingAccountTransactionSave _SavingAccountTransactionsave, string ConnectionString, out long pSavingaccountid)
        {
            StringBuilder sbSavingAccountTransaction = new StringBuilder();


            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();


                if (!string.IsNullOrEmpty(_SavingAccountTransactionsave.ptypeofoperation))
                {
                    if (string.IsNullOrEmpty(_SavingAccountTransactionsave.pSavingsamount.ToString()))
                    {
                        _SavingAccountTransactionsave.pSavingsamount = 0;
                    }

                    _SavingAccountTransactionsave.pTransdate = "'" + FormatDate(_SavingAccountTransactionsave.pTransdate) + "'";

                    if (ManageQuote(_SavingAccountTransactionsave.ptypeofoperation).ToUpper() == "CREATE")
                    {
                        if (!string.IsNullOrEmpty(_SavingAccountTransactionsave.pSavingaccname))
                        {
                            _SavingAccountTransactionsave.pSavingaccountno = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('SAVING ACCOUNT','" + _SavingAccountTransactionsave.pSavingaccname + "'," + _SavingAccountTransactionsave.pTransdate + ")").ToString();
                        }
                        _SavingAccountTransactionsave.pSavingaccountid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltranssavingaccountcreation(savingaccountno , transdate, membertypeid, membertype ,memberid, applicanttype, membercode, membername,contactid, contacttype, contactreferenceid, savingconfigid,savingaccname,savingsamount, savingsamountpayin,interestpayout,isjointapplicable,isreferralapplicable,statusid, createdby, createddate,isnomineedetailsapplicable) VALUES ('" + ManageQuote(_SavingAccountTransactionsave.pSavingaccountno) + "'," + _SavingAccountTransactionsave.pTransdate + "," + _SavingAccountTransactionsave.pMembertypeid + ",'" + ManageQuote(_SavingAccountTransactionsave.pMembertype) + "'," + _SavingAccountTransactionsave.pMemberid + ",'" + ManageQuote(_SavingAccountTransactionsave.pApplicanttype) + "','" + ManageQuote(_SavingAccountTransactionsave.pMembercode) + "','" + ManageQuote(_SavingAccountTransactionsave.pMembername) + "'," + _SavingAccountTransactionsave.pContactid + ",'" + ManageQuote(_SavingAccountTransactionsave.pContacttype) + "','" + ManageQuote(_SavingAccountTransactionsave.pContactreferenceid) + "'," + _SavingAccountTransactionsave.pSavingconfigid + ",'" + ManageQuote(_SavingAccountTransactionsave.pSavingaccname) + "'," + _SavingAccountTransactionsave.pSavingsamount + ",'" + ManageQuote(_SavingAccountTransactionsave.pSavingsamountpayin) + "','" + ManageQuote(_SavingAccountTransactionsave.pInterestpayout) + "'," + _SavingAccountTransactionsave.pIsjointapplicable + "," + _SavingAccountTransactionsave.pIsreferralapplicable + "," + Convert.ToInt32(Status.Active) + ", '" + _SavingAccountTransactionsave.pCreatedby + "', current_timestamp," + _SavingAccountTransactionsave.pIsNomineesapplicable + ") returning savingaccountid; "));

                        trans.Commit();

                    }

                    else if (ManageQuote(_SavingAccountTransactionsave.ptypeofoperation).ToUpper() == "UPDATE")
                    {
                        sbSavingAccountTransaction.AppendLine("UPDATE tbltranssavingaccountcreation SET  transdate=" + _SavingAccountTransactionsave.pTransdate + ", membertypeid=" + _SavingAccountTransactionsave.pMembertypeid + ", membertype='" + ManageQuote(_SavingAccountTransactionsave.pMembertype) + "',applicanttype='" + ManageQuote(_SavingAccountTransactionsave.pApplicanttype) + "',savingconfigid=" + _SavingAccountTransactionsave.pSavingconfigid + ",savingaccname='" + _SavingAccountTransactionsave.pSavingaccname + "',savingsamount=" + _SavingAccountTransactionsave.pSavingsamount + ",savingsamountpayin='" + ManageQuote(_SavingAccountTransactionsave.pSavingsamountpayin) + "',interestpayout='" + ManageQuote(_SavingAccountTransactionsave.pInterestpayout) + "' WHERE savingaccountid=" + _SavingAccountTransactionsave.pSavingaccountid + ";");

                    }

                }
                pSavingaccountid = _SavingAccountTransactionsave.pSavingaccountid;
                string Recordid = string.Empty;

                if (Convert.ToString(sbSavingAccountTransaction) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbSavingAccountTransaction.ToString());
                    trans.Commit();

                }
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
            return _SavingAccountTransactionsave.pSavingaccountno;
        }

        public bool SaveJointmemberAndNominee(JointmemberAndNomineeSave _JointmemberAndNomineeSave, string ConnectionString)
        {
            bool IsSaved = false;
            StringBuilder sbjointmember = new StringBuilder();
            StringBuilder sbNominee = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();
            string Recordid = string.Empty;
            string query = "";
            string recordid1 = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();


                if (_JointmemberAndNomineeSave.JointMembersList != null)
                {
                    for (int i = 0; i < _JointmemberAndNomineeSave.JointMembersList.Count; i++)
                    {
                        if (_JointmemberAndNomineeSave.JointMembersList[i].ptypeofoperation != "CREATE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = _JointmemberAndNomineeSave.JointMembersList[i].pRecordid.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + _JointmemberAndNomineeSave.JointMembersList[i].pRecordid.ToString();
                            }
                        }
                        if (_JointmemberAndNomineeSave.JointMembersList[i].ptypeofoperation == "CREATE")
                        {
                            sbjointmember.Append("insert into tbltranssavingaccountjointdetails(savingaccountid,savingaccountno ,memberid,membercode,membername,contactid,contacttype,contactreferenceid,statusid, createdby, createddate)values(" + _JointmemberAndNomineeSave.JointMembersList[i].pSavingaccountid + ",'" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pSavingaccountno) + "'," + _JointmemberAndNomineeSave.JointMembersList[i].pMemberid + ",'" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pMembercode) + "','" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pMembername) + "'," + _JointmemberAndNomineeSave.JointMembersList[i].pContactid + ",'" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pContacttype) + "','" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pContactreferenceid) + "'," + Convert.ToInt32(Status.Active) + ", '" + _JointmemberAndNomineeSave.JointMembersList[i].pCreatedby + "', current_timestamp);");


                        }
                        else
                        {
                            sbjointmember.Append("update tbltranssavingaccountjointdetails set memberid=" + _JointmemberAndNomineeSave.JointMembersList[i].pMemberid + ",membercode='" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pMembercode) + "',membername='" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pMembername) + "',contactid=" + _JointmemberAndNomineeSave.JointMembersList[i].pContactid + ",contacttype='" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pContacttype) + "',contactreferenceid='" + ManageQuote(_JointmemberAndNomineeSave.JointMembersList[i].pContactreferenceid) + "' where recordid=" + _JointmemberAndNomineeSave.JointMembersList[i].pRecordid + " and savingaccountid=" + _JointmemberAndNomineeSave.JointMembersList[i].pSavingaccountid + ";");


                        }
                    }
                    sbjointmember.Append("update tbltranssavingaccountcreation set isjointapplicable=" + _JointmemberAndNomineeSave.pIsjointapplicable + " where savingaccountid=" + _JointmemberAndNomineeSave.papplicationid + ";");
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        query = "update tbltranssavingaccountjointdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + _JointmemberAndNomineeSave.pCreatedby + ",modifieddate=current_timestamp where savingaccountid=" + _JointmemberAndNomineeSave.papplicationid + "  and recordid not in(" + Recordid + ");";
                    }
                    else
                    {
                        query = "update tbltranssavingaccountjointdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + _JointmemberAndNomineeSave.pCreatedby + ",modifieddate=current_timestamp where savingaccountid=" + _JointmemberAndNomineeSave.papplicationid + ";";
                    }
                }
                recordid1 = string.Empty;
                if (_JointmemberAndNomineeSave.PersonalNomineeList != null)
                {

                    for (int i = 0; i < _JointmemberAndNomineeSave.PersonalNomineeList.Count; i++)
                    {
                        if (_JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = _JointmemberAndNomineeSave.PersonalNomineeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + _JointmemberAndNomineeSave.PersonalNomineeList[i].precordid.ToString();
                            }
                        }
                        if (_JointmemberAndNomineeSave.PersonalNomineeList[i].pStatusname == null)
                        {
                            _JointmemberAndNomineeSave.PersonalNomineeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(_JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation))
                        {
                            _JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation = _JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(_JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth))
                        {

                            _JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth = "null";
                        }
                        else
                        {

                            _JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth = "'" + FormatDate(_JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth) + "'";
                        }


                        sbNominee.Append("update tabapplicationpersonalapplicablesections set isnomineedetailsapplicable='" + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pisapplicable) + "', modifiedby='" + (_JointmemberAndNomineeSave.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(_JointmemberAndNomineeSave.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactreferenceid) + "';");

                        if (_JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation == "CREATE")
                        {
                            if (_JointmemberAndNomineeSave.PersonalNomineeList[i].pisapplicable == true && !string.IsNullOrEmpty(_JointmemberAndNomineeSave.PersonalNomineeList[i].pnomineename))
                                sbNominee.Append("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee) values ('" + (_JointmemberAndNomineeSave.papplicationid) + "', '" + ManageQuote(_JointmemberAndNomineeSave.pvchapplicationid) + "', '" + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactid) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactreferenceid) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pnomineename) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].prelationship) + "', " + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth) + ", '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactno) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pidprooftype) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pidproofname) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].preferencenumber) + "', '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pdocidproofpath) + "', " + Convert.ToInt32(Status.Active) + ", '" + (_JointmemberAndNomineeSave.pCreatedby) + "', current_timestamp,'" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].papplicanttype) + "'," + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pisprimarynominee) + ");");
                        }
                        if (_JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation == "UPDATE" || _JointmemberAndNomineeSave.PersonalNomineeList[i].ptypeofoperation == "OLD")
                        {
                            if (_JointmemberAndNomineeSave.PersonalNomineeList[i].pisapplicable == true)
                                sbNominee.Append("update tabapplicationpersonalnomineedetails set contactid = '" + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactid) + "', contactreferenceid = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactreferenceid) + "', nomineename = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pnomineename) + "', relationship = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].prelationship) + "', dateofbirth = " + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pdateofbirth) + ", contactno = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pcontactno) + "', idprooftype = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pidprooftype) + "', idproofname = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pidproofname) + "', referencenumber = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].preferencenumber) + "', docidproofpath = '" + ManageQuote(_JointmemberAndNomineeSave.PersonalNomineeList[i].pdocidproofpath) + "', statusid = '" + getStatusid(_JointmemberAndNomineeSave.PersonalNomineeList[i].pStatusname, ConnectionString) + "', modifiedby = '" + (_JointmemberAndNomineeSave.pCreatedby) + "', modifieddate = current_timestamp,isprimarynominee=" + (_JointmemberAndNomineeSave.PersonalNomineeList[i].pisprimarynominee) + " where vchapplicationid = '" + ManageQuote(_JointmemberAndNomineeSave.pvchapplicationid) + "' and applicationid = " + (_JointmemberAndNomineeSave.papplicationid) + " and recordid = " + _JointmemberAndNomineeSave.PersonalNomineeList[i].precordid + ";");

                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.Append("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _JointmemberAndNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + _JointmemberAndNomineeSave.papplicationid + " and vchapplicationid='" + ManageQuote(_JointmemberAndNomineeSave.pvchapplicationid) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (_JointmemberAndNomineeSave.PersonalNomineeList == null || _JointmemberAndNomineeSave.PersonalNomineeList.Count == 0)
                    {

                        sbupdate.Append("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _JointmemberAndNomineeSave.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + _JointmemberAndNomineeSave.papplicationid + " and vchapplicationid='" + ManageQuote(_JointmemberAndNomineeSave.pvchapplicationid) + "'; ");
                    }
                }

                if (!string.IsNullOrEmpty(query))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query);
                }
                if (!string.IsNullOrEmpty(sbjointmember.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbupdate) + "" + Convert.ToString(sbjointmember) + "" + Convert.ToString(sbNominee));
                }



                trans.Commit();
                IsSaved = true;
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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

        public bool SaveReferral(ReferralSave _ReferralSave, string ConnectionString)
        {
            bool IsSaved = false;
            StringBuilder sbSaveReferral = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();

            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sbSaveReferral.Append("update tbltranssavingaccountcreation set isreferralapplicable=" + _ReferralSave.pIsreferralapplicable + " where savingaccountid=" + _ReferralSave.pSavingaccountid + ";");
                if (!string.IsNullOrEmpty(_ReferralSave.ptypeofoperation))
                {
                    int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tbltranssavingaccountreferraldetails where savingaccountid=" + _ReferralSave.pSavingaccountid + " and statusid =" + Convert.ToInt32(Status.Active) + ""));

                    if (count == 0)
                    {

                        if (_ReferralSave.pIsreferralapplicable == true)
                            sbSaveReferral.Append("INSERT INTO tbltranssavingaccountreferraldetails(savingaccountid , savingaccountno, referralid , referralname ,salespersonname ,statusid, createdby, createddate) VALUES (" + _ReferralSave.pSavingaccountid + ",'" + ManageQuote(_ReferralSave.pSavingaccountno) + "'," + _ReferralSave.pReferralid + ",'" + ManageQuote(_ReferralSave.pReferralname) + "','" + ManageQuote(_ReferralSave.pSalespersonname) + "'," + Convert.ToInt32(Status.Active) + ", '" + _ReferralSave.pCreatedby + "', current_timestamp);");

                    }
                    else if (ManageQuote(_ReferralSave.ptypeofoperation).ToUpper() == "UPDATE")
                    {
                        if (_ReferralSave.pIsreferralapplicable == true)
                        {
                            sbSaveReferral.AppendLine("UPDATE tbltranssavingaccountreferraldetails SET  referralid=" + _ReferralSave.pReferralid + ",referralname='" + ManageQuote(_ReferralSave.pReferralname) + "',salespersonname='" + ManageQuote(_ReferralSave.pSalespersonname) + "' WHERE savingaccountid=" + _ReferralSave.pSavingaccountid + ";");
                        }
                        else
                        {
                            sbSaveReferral.AppendLine("UPDATE tbltranssavingaccountreferraldetails SET  statusid=" + Convert.ToInt32(Status.Inactive) + " WHERE savingaccountid=" + _ReferralSave.pSavingaccountid + ";");
                        }

                    }


                }

                string Recordid = string.Empty;

                if (!string.IsNullOrEmpty(sbSaveReferral.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbSaveReferral.ToString());
                    trans.Commit();
                    IsSaved = true;
                }
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
        public int CheckMemberDuplicates(long Memberid, long savingaccountid, string connectionstring)
        {
            int count = 0;
            try
            {
                if (savingaccountid == 0)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tbltranssavingaccountcreation where memberid=" + Memberid + " and statusid=" + Convert.ToInt32(Status.Active) + ""));
                }
                else
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tbltranssavingaccountcreation where memberid=" + Memberid + " and savingaccountid!=" + savingaccountid + "  and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;
        }

        public async Task<List<SavingAccountTransactionMainGrid>> GetSavingAccountTransactionMainGrid(string ConnectionString)
        {
            _SavingAccountTransactionMainGridList = new List<SavingAccountTransactionMainGrid>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingaccountid,savingaccountno,membertype,applicanttype,transdate,contacttype,membername,savingaccname from tbltranssavingaccountcreation where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            var _SavingAccountTransactionMainGrid = new SavingAccountTransactionMainGrid
                            {
                                pSavingaccountid = Convert.ToInt64(dr["savingaccountid"]),
                                pSavingaccountno = Convert.ToString(dr["savingaccountno"]),
                                pMembertype = Convert.ToString(dr["membertype"]),
                                pApplicanttype = Convert.ToString(dr["applicanttype"]),
                                pTransdate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy"),
                                pContacttype = Convert.ToString(dr["contacttype"]),
                                pMembername = Convert.ToString(dr["membername"]),
                                pSavingaccname = Convert.ToString(dr["savingaccname"])

                                //  pNomineeName = Convert.ToString(dr["nomineename"])
                            };
                            _SavingAccountTransactionMainGridList.Add(_SavingAccountTransactionMainGrid);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _SavingAccountTransactionMainGridList;
        }
        public async Task<SavingsTransactionDataEdit> GetSavingAccountTransactionEditDetails(string ConnectionString, Int64 SavingAccountId, string accounttype, string savingsaccountNo)
        {
            SavingsTransactionDataEdit _SavingsTransactionDataEdit = new SavingsTransactionDataEdit();
            objshareApplicationDAL = new ShareApplicationDAL();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingaccountid,savingaccountno,membertypeid,membertype,applicanttype,transdate,contactid,contacttype,memberid,membername,savingconfigid,savingaccname,savingsamount,isjointapplicable,isreferralapplicable,isnomineedetailsapplicable from tbltranssavingaccountcreation where  savingaccountid=" + SavingAccountId + " and statusid=" + Convert.ToInt32(Status.Active) + ""))

                    {
                        while (dr.Read())
                        {

                            _SavingsTransactionDataEdit.pSavingaccountid = Convert.ToInt64(dr["savingaccountid"]);
                            _SavingsTransactionDataEdit.pSavingaccountno = Convert.ToString(dr["savingaccountno"]);
                            _SavingsTransactionDataEdit.pMembertypeid = Convert.ToInt64(dr["membertypeid"]);
                            _SavingsTransactionDataEdit.pMembertype = Convert.ToString(dr["membertype"]);
                            _SavingsTransactionDataEdit.pApplicanttype = Convert.ToString(dr["applicanttype"]);
                            _SavingsTransactionDataEdit.pTransdate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy");
                            _SavingsTransactionDataEdit.pContactid = Convert.ToInt64(dr["contactid"]);
                            _SavingsTransactionDataEdit.pContacttype = Convert.ToString(dr["contacttype"]);
                            _SavingsTransactionDataEdit.pMemberid = Convert.ToInt64(dr["memberid"]);
                            _SavingsTransactionDataEdit.pMembername = Convert.ToString(dr["membername"]);
                            _SavingsTransactionDataEdit.pSavingconfigid = Convert.ToInt64(dr["savingconfigid"]);
                            _SavingsTransactionDataEdit.pSavingaccname = Convert.ToString(dr["savingaccname"]);
                            _SavingsTransactionDataEdit.pSavingsamount = Convert.ToDecimal(dr["savingsamount"]);
                            _SavingsTransactionDataEdit.pIsjointapplicable = Convert.ToBoolean(dr["isjointapplicable"]);
                            _SavingsTransactionDataEdit.pIsreferralapplicable = Convert.ToBoolean(dr["isreferralapplicable"]);
                            _SavingsTransactionDataEdit.pIsNomineesapplicable = Convert.ToBoolean(dr["isnomineedetailsapplicable"]);
                            _SavingsTransactionDataEdit.ptypeofoperation = Convert.ToString("UPDATE");
                            _SavingsTransactionDataEdit.JointMembersandContactDetailsList = objshareApplicationDAL.GetJointMembersListInEdit(savingsaccountNo, accounttype, ConnectionString);
                            _SavingsTransactionDataEdit.MemberNomineeDetailsList = objshareApplicationDAL.GetNomineesListInEdit(savingsaccountNo, accounttype, ConnectionString);
                            _SavingsTransactionDataEdit.MemberReferalDetails = objshareApplicationDAL.getReferralDetails(ConnectionString, savingsaccountNo, accounttype);

                        }
                    }



                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return _SavingsTransactionDataEdit;
        }

        //public SavingAccountTransactionSave getSavingAccountTransactionDetails(string ConnectionString, Int64 SavingAccountId)
        //{
        //    SavingAccountTransactionSave _SavingAccountTransaction = new SavingAccountTransactionSave();

        //    try
        //    {

        //        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select savingaccountid,savingaccountno,membertypeid,membertype,applicanttype,transdate,contactid,contacttype,memberid,membername,savingconfigid,savingaccname,savingsamount,isjointapplicable,isreferralapplicable,isnomineedetailsapplicable from tbltranssavingaccountcreation where  savingaccountid=" + SavingAccountId + " and statusid=" + Convert.ToInt32(Status.Active) + ""))

        //        {
        //            while (dr.Read())
        //            {
        //                _SavingAccountTransaction = new SavingAccountTransactionSave()
        //                {
        //                    pSavingaccountid = Convert.ToInt64(dr["savingaccountid"]),
        //                    pSavingaccountno = Convert.ToString(dr["savingaccountno"]),
        //                    pMembertypeid = Convert.ToInt64(dr["membertypeid"]),
        //                    pMembertype = Convert.ToString(dr["membertype"]),
        //                    pApplicanttype = Convert.ToString(dr["applicanttype"]),
        //                    pTransdate = Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy"),
        //                    pContactid = Convert.ToInt64(dr["contactid"]),
        //                    pContacttype = Convert.ToString(dr["contacttype"]),
        //                    pMemberid = Convert.ToInt64(dr["memberid"]),
        //                    pMembername = Convert.ToString(dr["membername"]),
        //                    pSavingconfigid = Convert.ToInt64(dr["savingconfigid"]),
        //                    pSavingaccname = Convert.ToString(dr["savingaccname"]),
        //                    pSavingsamount = Convert.ToDecimal(dr["savingsamount"]),
        //                    pIsjointapplicable = Convert.ToBoolean(dr["isjointapplicable"]),
        //                    pIsreferralapplicable = Convert.ToBoolean(dr["isreferralapplicable"]),
        //                    pIsNomineesapplicable = Convert.ToBoolean(dr["isnomineedetailsapplicable"]),
        //                    ptypeofoperation = Convert.ToString("UPDATE")



        //                };

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return _SavingAccountTransaction;
        //}

        public List<JointMembers> getJointMembersDetails(string ConnectionString, Int64 SavingAccountId)
        {
            List<JointMembers> JointMembersList = new List<JointMembers>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,savingaccountid,savingaccountno,memberid,membercode,membername,contactid,contactreferenceid,contacttype from tbltranssavingaccountjointdetails where savingaccountid=" + SavingAccountId + " and statusid=" + Convert.ToInt32(Status.Active) + ""))

                {
                    while (dr.Read())
                    {
                        JointMembers obJointMembersDetails = new JointMembers()
                        {
                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            pSavingaccountid = Convert.ToInt64(dr["savingaccountid"]),
                            pSavingaccountno = Convert.ToString(dr["savingaccountno"]),
                            pMemberid = Convert.ToInt64(dr["memberid"]),
                            pMembercode = Convert.ToString(dr["membercode"]),
                            pMembername = Convert.ToString(dr["membername"]),
                            pContactid = Convert.ToInt64(dr["contactid"]),
                            pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pContacttype = Convert.ToString(dr["contacttype"]),

                            ptypeofoperation = Convert.ToString("UPDATE")

                        };




                        JointMembersList.Add(obJointMembersDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JointMembersList;
        }

        public List<ApplicationPersonalNomineeDTO> getApplicationPersonalNominee(string ConnectionString, Int64 SavingAccountId)
        {
            List<ApplicationPersonalNomineeDTO> ApplicationPersonalNomineeList = new List<ApplicationPersonalNomineeDTO>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(nomineename, '') as nomineename, coalesce(relationship, '') as relationship, coalesce(dateofbirth, null) as dateofbirth, coalesce(contactno, '') as contactno, coalesce(idprooftype, '') as idprooftype, coalesce(idproofname, '') as idproofname, coalesce(referencenumber, '') as referencenumber, coalesce(docidproofpath, '') as docidproofpath, coalesce(isprimarynominee, false) as isprimarynominee FROM tabapplicationpersonalnomineedetails  ti where  ti.applicationid = " + SavingAccountId + " and ti.statusid = " + Convert.ToInt32(Status.Active) + ";"))

                {
                    while (dr.Read())
                    {
                        ApplicationPersonalNomineeDTO obJointMembersDetails = new ApplicationPersonalNomineeDTO()
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pnomineename = dr["nomineename"].ToString(),
                            prelationship = dr["relationship"].ToString(),
                            pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),

                            pcontactno = dr["contactno"].ToString(),
                            papplicanttype = dr["applicantype"].ToString(),

                            pidprooftype = dr["idprooftype"].ToString(),
                            pidproofname = dr["idproofname"].ToString(),
                            preferencenumber = dr["referencenumber"].ToString(),
                            pdocidproofpath = dr["docidproofpath"].ToString(),
                            pisprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                            ptypeofoperation = "OLD"


                        };




                        ApplicationPersonalNomineeList.Add(obJointMembersDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ApplicationPersonalNomineeList;
        }

        public ReferralSave getReferralDetails(string ConnectionString, Int64 SavingAccountId)
        {
            ReferralSave _ReferralSave = new ReferralSave();

            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,savingaccountid,savingaccountno,referralid,referralname,salespersonname from tbltranssavingaccountreferraldetails where savingaccountid=" + SavingAccountId + " and statusid =" + Convert.ToInt32(Status.Active) + ""))

                {
                    if (dr.Read())
                    {
                        _ReferralSave = new ReferralSave()
                        {

                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            pSavingaccountid = Convert.ToInt64(dr["savingaccountid"]),
                            pSavingaccountno = Convert.ToString(dr["savingaccountno"]),
                            pReferralid = Convert.ToInt64(dr["referralid"]),
                            pReferralname = Convert.ToString(dr["referralname"]),
                            pSalespersonname = Convert.ToString(dr["salespersonname"]),

                            ptypeofoperation = Convert.ToString("UPDATE")



                        };

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ReferralSave;
        }
        public bool DeleteSavingTransaction(Int64 savingAccountid, long modifiedby, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbupdate = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sbupdate.Append("UPDATE tbltranssavingaccountcreation set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingaccountid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tbltranssavingaccountjointdetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingaccountid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tbltranssavingaccountreferraldetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where savingaccountid=" + savingAccountid + "; ");
                sbupdate.Append("UPDATE tabapplicationpersonalnomineedetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where applicationid=" + savingAccountid + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString());
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
