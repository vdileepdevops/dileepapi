using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Banking.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class ShareApplicationDAL : SettingsDAL, IShareApplication
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region Members Binding Based On Receipttype
        public async Task<List<shareMembersDetails>> GetshareMembers(string MemberType, string Receipttype, string ConnectionString)
        {
            List<shareMembersDetails> shareMembersList = new List<shareMembersDetails>();
            await Task.Run(() =>
            {
                try
                {
                    if (Receipttype.ToUpper() == "TRUE")
                    {
                        //and memberid not in(select memberid from tbltransshareapplication)
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,te.contactid, te.contactreferenceid,businessentitycontactno as contactnumber from tblmstmembers te join tblmstcontact tc  on tc.contactid = te.contactid where membertype = '" + MemberType + "' and  te.statusid = " + Convert.ToInt32(Status.Active) + "  and  memberid in(select member_id from Member_receipt)order by membername; "))
                        {
                            while (dr.Read())
                            {
                                var shareMembersDetails = new shareMembersDetails
                                {
                                    pMemberCode = dr["membercode"],
                                    pMemberId = Convert.ToInt64(dr["memberid"]),
                                    pMemberName = dr["membername"],
                                    pContactid = Convert.ToInt64(dr["contactid"]),
                                    pContactrefid = dr["contactreferenceid"],
                                    pContactnumber = dr["contactnumber"]
                                };
                                shareMembersList.Add(shareMembersDetails);
                            }
                        }


                    }
                    else
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode,membername,te.contactid, te.contactreferenceid,businessentitycontactno as contactnumber from tblmstmembers te join tblmstcontact tc  on tc.contactid = te.contactid where  membertype = '" + MemberType + "' and  te.statusid = " + Convert.ToInt32(Status.Active) + " order by membername; "))
                        {
                            while (dr.Read())
                            {
                                var shareMembersDetails = new shareMembersDetails
                                {
                                    pMemberCode = dr["membercode"],
                                    pMemberId = Convert.ToInt64(dr["memberid"]),
                                    pMemberName = dr["membername"],
                                    pContactid = Convert.ToInt64(dr["contactid"]),
                                    pContactrefid = dr["contactreferenceid"],
                                    pContactnumber = dr["contactnumber"]
                                };
                                shareMembersList.Add(shareMembersDetails);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return shareMembersList;
        }
        #endregion

        #region Share Names Binding Based On Applicanttype
        public async Task<List<ShareviewDTO>> GetSharNames(string Membertype, string Applicanttype, string ConnectionString)
        {
            List<ShareviewDTO> lstShareviewDTO = new List<ShareviewDTO>();

            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct shareconfigid,sharename from tblmstshareconfigdetails where membertype='" + ManageQuote(Membertype) + "' and applicanttype='" + ManageQuote(Applicanttype) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareviewDTO ShareviewDTO = new ShareviewDTO();
                            ShareviewDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            ShareviewDTO.psharename = Convert.ToString(dr["sharename"]);
                            lstShareviewDTO.Add(ShareviewDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return lstShareviewDTO;
        }
        #endregion

        #region Share Configaration Details Based On Applicanttype
        public async Task<List<shareconfigDetails>> GetSharconfigdetails(long shareconfigid, string Applicanttype, string Membertype, string ConnectionString)
        {
            List<shareconfigDetails> lstShareconfigdetails = new List<shareconfigDetails>();

            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select shareconfigid,sharename,applicanttype,facevalue,minshares,maxshares from  tblmstshareconfigdetails where shareconfigid =" + shareconfigid + " and applicanttype='" + ManageQuote(Applicanttype) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and membertype='" + Membertype + "';"))
                    {
                        while (dr.Read())
                        {
                            shareconfigDetails objshareconfigDetails = new shareconfigDetails();
                            objshareconfigDetails.pShareconfigid = Convert.ToInt64(dr["shareconfigid"]);
                            objshareconfigDetails.pSharename = Convert.ToString(dr["sharename"]);
                            objshareconfigDetails.pApplicanttype = Convert.ToString(dr["applicanttype"]);
                            objshareconfigDetails.pFacevalue = Convert.ToInt64(dr["facevalue"]);
                            objshareconfigDetails.pMinshare = Convert.ToInt64(dr["minshares"]);
                            objshareconfigDetails.pMaxshare = Convert.ToInt64(dr["maxshares"]);
                            lstShareconfigdetails.Add(objshareconfigDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return lstShareconfigdetails;
        }
        #endregion

        #region Save Share Application First Tab

        public string SaveshareApplication(ShareApplicationDTO ShareApplicationDTO, string ConnectionString, out long pShareaccountid)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            Int64 Maxsharetovalue = 0;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(ShareApplicationDTO.pshareapplicationid.ToString()) || ShareApplicationDTO.pshareapplicationid == 0)
                {
                    Maxsharetovalue = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select coalesce(max(distinctive_to),0) from Share_Account_creation;"));
                    ShareApplicationDTO.pdistinctivefrom = Maxsharetovalue + 1;
                    //    ShareApplicationDTO.pdistinctiveto = (Maxsharetovalue + ShareApplicationDTO.pnoofsharesissued) - 1;
                    ShareApplicationDTO.pdistinctiveto = (ShareApplicationDTO.pdistinctivefrom + ShareApplicationDTO.pnoofsharesissued);



                    ShareApplicationDTO.pShareAccountNo = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('SHARE CAPITAL','" + ShareApplicationDTO.psharename + "','" + ShareApplicationDTO.pTransdate + "')").ToString();
                    //ShareApplicationDTO.pShareAccountNo = "sd0001";

                    ShareApplicationDTO.pshareapplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into Share_Account_creation(share_Account_number,share_config_id,share_name,member_type_id,member_id,member_name,applicant_type,Trans_date,reference_no,face_value,no_of_shares_issued,distinctive_from,distinctive_to,total_amount,shares_issue_date,is_member_fee_applicable,Bond_print_status,statusid,createdby,createddate)values('" + ShareApplicationDTO.pShareAccountNo + "'," + ShareApplicationDTO.pshareconfigid + ",'" + ManageQuote(ShareApplicationDTO.psharename) + "'," + Convert.ToInt64(ShareApplicationDTO.pmembertypeid) + "," + Convert.ToInt64(ShareApplicationDTO.pmemberid) + ", '" + ManageQuote(ShareApplicationDTO.pmembername) + "','" + ManageQuote(ShareApplicationDTO.pApplicanttype) + "','" + FormatDate(ShareApplicationDTO.pTransdate) + "','" + ManageQuote(ShareApplicationDTO.preferenceno) + "'," + Convert.ToDecimal(ShareApplicationDTO.pfacevalue) + "," + Convert.ToInt64(ShareApplicationDTO.pnoofsharesissued) + "," + Convert.ToInt64(ShareApplicationDTO.pdistinctivefrom) + "," + Convert.ToInt64(ShareApplicationDTO.pdistinctiveto) + "," + Convert.ToDecimal(ShareApplicationDTO.ptotalamount) + ",'" + FormatDate(ShareApplicationDTO.pshareissuedate) + "','" + ShareApplicationDTO.pismemberfeeapplicable + "','N'," + Convert.ToInt32(Status.Active) + "," + ShareApplicationDTO.pCreatedby + ",current_timestamp) returning share_account_id;"));

                }
                else
                {
                    sbInsert.Append("update Share_Account_creation set is_member_fee_applicable='" + ShareApplicationDTO.pismemberfeeapplicable + "', member_type_id=" + Convert.ToInt64(ShareApplicationDTO.pmembertypeid) + ", member_id=" + Convert.ToInt64(ShareApplicationDTO.pmemberid) + ", share_config_id=" + Convert.ToInt64(ShareApplicationDTO.pshareconfigid) + ",reference_no='" + ManageQuote(ShareApplicationDTO.preferenceno) + "', face_value=" + Convert.ToDecimal(ShareApplicationDTO.pfacevalue) + ",no_of_shares_issued=" + Convert.ToInt64(ShareApplicationDTO.pnoofsharesissued) + ", distinctive_from=" + Convert.ToInt64(ShareApplicationDTO.pdistinctivefrom) + ", distinctive_to=" + Convert.ToInt64(ShareApplicationDTO.pdistinctiveto) + ", total_amount=" + Convert.ToDecimal(ShareApplicationDTO.ptotalamount) + ",Share_Joining_date='" + FormatDate(ShareApplicationDTO.pTransdate) + "', shares_issue_date='" + FormatDate(ShareApplicationDTO.pshareissuedate) + "', Bond_print_status='N',modifiedby=" + ShareApplicationDTO.pCreatedby + ",modifieddate=current_timestamp where share_account_id=" + ShareApplicationDTO.pShareaccountid + " and share_account_number=" + ShareApplicationDTO.pShareAccountNo + ";");
                    if (!string.IsNullOrEmpty(sbInsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                    }
                }
                pShareaccountid = ShareApplicationDTO.pshareapplicationid;
                trans.Commit();
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
            return Convert.ToString(ShareApplicationDTO.pShareAccountNo);
        }
        #endregion

        #region Save Share Application Nomiee and joint Details Second Tab
        public bool SaveshareJointMembersandNomineeData(savejointandnomiee JointandNomineeSaveDTO, string ConnectionString)
        {

            StringBuilder SbsaveReferences = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                StringBuilder sbUpdate = new StringBuilder();
                string RecordId = string.Empty;
                string JointRecordid = string.Empty;

                if (JointandNomineeSaveDTO.paccounttype == "share")
                {
                    SbsaveReferences.AppendLine("Update Share_Account_creation set isjointapplicable='" + JointandNomineeSaveDTO.pIsjointapplicableorNot + "',isnomineesapplicable='" + JointandNomineeSaveDTO.pIsNomineesApplicableorNot + "' where share_Account_id=" + JointandNomineeSaveDTO.pAccountId + " and share_Account_number='" + JointandNomineeSaveDTO.paccountNo + "';");
                }
                else if (JointandNomineeSaveDTO.paccounttype == "RD Transaction")
                {
                    SbsaveReferences.AppendLine("Update tbltransrdcreation set isjointapplicable='" + JointandNomineeSaveDTO.pIsjointapplicableorNot + "',isnomineesapplicable='" + JointandNomineeSaveDTO.pIsNomineesApplicableorNot + "' where rdaccountid=" + JointandNomineeSaveDTO.pAccountId + " and rdaccountno='" + JointandNomineeSaveDTO.paccountNo + "';");
                }
                else if (JointandNomineeSaveDTO.paccounttype == "FD Transaction")
                {
                    SbsaveReferences.AppendLine("Update tbltransfdcreation set isjointapplicable='" + JointandNomineeSaveDTO.pIsjointapplicableorNot + "',isnomineesapplicable='" + JointandNomineeSaveDTO.pIsNomineesApplicableorNot + "' where fdaccountid=" + JointandNomineeSaveDTO.pAccountId + " and fdaccountno='" + JointandNomineeSaveDTO.paccountNo + "';");
                }
                else if (JointandNomineeSaveDTO.paccounttype == "Savings Transaction")
                {
                    SbsaveReferences.Append("update tbltranssavingaccountcreation set isjointapplicable=" + JointandNomineeSaveDTO.pIsjointapplicableorNot + ",isnomineedetailsapplicable='" + JointandNomineeSaveDTO.pIsNomineesApplicableorNot + "' where savingaccountid=" + JointandNomineeSaveDTO.pAccountId + " and savingaccountno='" + JointandNomineeSaveDTO.paccountNo + "';");
                }

                if (JointandNomineeSaveDTO.JointDetailsList != null && JointandNomineeSaveDTO.JointDetailsList.Count > 0)
                {
                    for (int i = 0; i < JointandNomineeSaveDTO.JointDetailsList.Count; i++)
                    {
                        if (Convert.ToString(JointandNomineeSaveDTO.JointDetailsList[i].pTypeofOperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(JointRecordid))
                            {
                                JointRecordid = Convert.ToString(JointandNomineeSaveDTO.JointDetailsList[i].precordid);
                            }
                            else
                            {
                                JointRecordid = JointRecordid + "," + Convert.ToString(JointandNomineeSaveDTO.JointDetailsList[i].precordid);
                            }
                        }
                    }

                    for (int i = 0; i < JointandNomineeSaveDTO.JointDetailsList.Count; i++)
                    {
                        if (Convert.ToString(JointandNomineeSaveDTO.JointDetailsList[i].pTypeofOperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("INSERT INTO tbltransjointdetails(accounttype, accountid, accountno, memberid, statusid, createdby, createddate) VALUES ( '" + JointandNomineeSaveDTO.paccounttype + "'," + JointandNomineeSaveDTO.pAccountId + ", '" + JointandNomineeSaveDTO.paccountNo + "', " + JointandNomineeSaveDTO.JointDetailsList[i].pMemberId + "," + Convert.ToInt32(Status.Active) + ", " + JointandNomineeSaveDTO.pCreatedby + ", current_timestamp);");
                        }
                        else if (Convert.ToString(JointandNomineeSaveDTO.JointDetailsList[i].pTypeofOperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tbltransjointdetails set memberid=" + JointandNomineeSaveDTO.JointDetailsList[i].pMemberId + ",statusid=" + Convert.ToInt32(Status.Active) + " where accountid=" + JointandNomineeSaveDTO.pAccountId + " and accountno='" + JointandNomineeSaveDTO.paccountNo + "' and jointdetailsid=" + JointandNomineeSaveDTO.JointDetailsList[i].precordid + ";");
                        }
                    }
                    if (!string.IsNullOrEmpty(JointRecordid))
                    {
                        sbUpdate.AppendLine("UPDATE tbltransjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE accountid=" + JointandNomineeSaveDTO.pAccountId + " and accountno='" + JointandNomineeSaveDTO.paccountNo + "' and accounttype='" + JointandNomineeSaveDTO.paccounttype + "' AND jointdetailsid not in(" + JointRecordid + ") ; ");
                    }
                    else
                    {
                        if (JointandNomineeSaveDTO.JointDetailsList == null || JointandNomineeSaveDTO.JointDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tbltransjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE accountid=" + JointandNomineeSaveDTO.pAccountId + " and accounttype='" + JointandNomineeSaveDTO.paccounttype + "' and accountno='" + JointandNomineeSaveDTO.paccountNo + "' ; ");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(JointRecordid))
                {
                    sbUpdate.AppendLine("UPDATE tbltransjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE accountid=" + JointandNomineeSaveDTO.pAccountId + " and accountno='" + JointandNomineeSaveDTO.paccountNo + "' and accounttype='" + JointandNomineeSaveDTO.paccounttype + "' AND jointdetailsid not in(" + JointRecordid + ") ; ");
                }
                else
                {
                    if (JointandNomineeSaveDTO.JointDetailsList == null || JointandNomineeSaveDTO.JointDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tbltransjointdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE accountid=" + JointandNomineeSaveDTO.pAccountId + " and accounttype='" + JointandNomineeSaveDTO.paccounttype + "' and accountno='" + JointandNomineeSaveDTO.paccountNo + "'; ");
                    }
                }

                // Nominee Details
                if (JointandNomineeSaveDTO.NomineeDetailsList != null && JointandNomineeSaveDTO.NomineeDetailsList.Count > 0)
                {
                    for (int i = 0; i < JointandNomineeSaveDTO.NomineeDetailsList.Count; i++)
                    {
                        if (Convert.ToString(JointandNomineeSaveDTO.NomineeDetailsList[i].ptypeofoperation) != "CREATE")
                        {
                            if (string.IsNullOrEmpty(RecordId))
                            {
                                RecordId = Convert.ToString(JointandNomineeSaveDTO.NomineeDetailsList[i].precordid);
                            }
                            else
                            {
                                RecordId = RecordId + "," + Convert.ToString(JointandNomineeSaveDTO.NomineeDetailsList[i].precordid);
                            }
                        }
                    }
                    for (int i = 0; i < JointandNomineeSaveDTO.NomineeDetailsList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth))
                        {
                            JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth = "null";
                        }
                        else
                        {
                            JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth = "'" + FormatDate(JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth) + "'";
                        }


                        if (Convert.ToString(JointandNomineeSaveDTO.NomineeDetailsList[i].ptypeofoperation) == "CREATE")
                        {
                            SbsaveReferences.AppendLine("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee,percentage) values ('" + JointandNomineeSaveDTO.pAccountId + "', '" + JointandNomineeSaveDTO.paccountNo + "','" + JointandNomineeSaveDTO.NomineeDetailsList[i].pcontactno + "', '" + JointandNomineeSaveDTO.NomineeDetailsList[i].pnomineename + "', '" + JointandNomineeSaveDTO.NomineeDetailsList[i].prelationship + "', " + JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth + ", '" + JointandNomineeSaveDTO.NomineeDetailsList[i].pcontactno + "', '" + JointandNomineeSaveDTO.NomineeDetailsList[i].pidprooftype + "', '" + JointandNomineeSaveDTO.NomineeDetailsList[i].pidproofname + "', '" + JointandNomineeSaveDTO.NomineeDetailsList[i].preferencenumber + "', '" +

                                JointandNomineeSaveDTO.NomineeDetailsList[i].pdocidproofpath + "', " + Convert.ToInt32(Status.Active) + ", '" + JointandNomineeSaveDTO.pCreatedby + "', current_timestamp,'" + JointandNomineeSaveDTO.paccounttype + "'," + JointandNomineeSaveDTO.NomineeDetailsList[i].pisprimarynominee + "," + JointandNomineeSaveDTO.NomineeDetailsList[i].pPercentage + ");");
                        }
                        else if (Convert.ToString(JointandNomineeSaveDTO.NomineeDetailsList[i].ptypeofoperation) == "UPDATE")
                        {
                            SbsaveReferences.AppendLine("Update tabapplicationpersonalnomineedetails set contactreferenceid='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pcontactno + "',nomineename='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pnomineename + "',relationship='" + JointandNomineeSaveDTO.NomineeDetailsList[i].prelationship + "',dateofbirth=" + JointandNomineeSaveDTO.NomineeDetailsList[i].pdateofbirth + ",contactno='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pcontactno + "',idprooftype='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pidprooftype + "',idproofname='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pidproofname + "',referencenumber='" + JointandNomineeSaveDTO.NomineeDetailsList[i].preferencenumber + "',docidproofpath='" + JointandNomineeSaveDTO.NomineeDetailsList[i].pdocidproofpath + "',statusid=" + Convert.ToInt32(Status.Active) + ",modifieddate=current_timestamp,modifiedby=" + JointandNomineeSaveDTO.pCreatedby + ",percentage=" + JointandNomineeSaveDTO.NomineeDetailsList[i].pPercentage + ",isprimarynominee=" + JointandNomineeSaveDTO.NomineeDetailsList[i].pisprimarynominee + " where applicantype='" + JointandNomineeSaveDTO.paccounttype + "' and applicationid=" + JointandNomineeSaveDTO.pAccountId + " and recordid=" + JointandNomineeSaveDTO.NomineeDetailsList[i].precordid + ";  ");
                        }
                    }
                    if (!string.IsNullOrEmpty(RecordId))
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + JointandNomineeSaveDTO.pAccountId + " and vchapplicationid='" + JointandNomineeSaveDTO.paccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='" + JointandNomineeSaveDTO.paccounttype + "'; ");
                    }
                    else
                    {
                        if (JointandNomineeSaveDTO.NomineeDetailsList == null || JointandNomineeSaveDTO.NomineeDetailsList.Count == 0)
                        {
                            sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + JointandNomineeSaveDTO.pAccountId + " and vchapplicationid='" + JointandNomineeSaveDTO.paccountNo + "' and applicantype='" + JointandNomineeSaveDTO.paccounttype + "'; ");
                        }
                    }

                }

                if (!string.IsNullOrEmpty(RecordId))
                {
                    sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + JointandNomineeSaveDTO.pAccountId + " and vchapplicationid='" + JointandNomineeSaveDTO.paccountNo + "' AND RECORDID not in(" + RecordId + ") and applicantype='" + JointandNomineeSaveDTO.paccounttype + "'; ");
                }
                else
                {
                    if (JointandNomineeSaveDTO.NomineeDetailsList == null || JointandNomineeSaveDTO.NomineeDetailsList.Count == 0)
                    {
                        sbUpdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + JointandNomineeSaveDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + JointandNomineeSaveDTO.pAccountId + " and vchapplicationid='" + JointandNomineeSaveDTO.paccountNo + "' and applicantype='" + JointandNomineeSaveDTO.paccounttype + "'; ");
                    }
                }

                if (!string.IsNullOrEmpty(SbsaveReferences.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbUpdate) + " " + Convert.ToString(SbsaveReferences));
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
        #endregion

        #region Save Share Application Referral Details Third Tab
        public bool SaveReferralData(Referrals ReferralDetails, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                StringBuilder SbsaveReferences = new StringBuilder();


                if (ReferralDetails.paccounttype == "share")
                {
                    SbsaveReferences.AppendLine("update Share_Account_creation set isreferralapplicable='" + ReferralDetails.pIsReferralsapplicable + "' where share_Account_number='" + ReferralDetails.paccountNo + "';");
                }
                else if (ReferralDetails.paccounttype == "RD Transaction")
                {
                    SbsaveReferences.AppendLine("update tbltransrdcreation set isreferralapplicable='" + ReferralDetails.pIsReferralsapplicable + "' where rdaccountid=" + ReferralDetails.pAccountId + " and rdaccountno='" + ReferralDetails.paccountNo + "';");

                }
                else if (ReferralDetails.paccounttype == "FD Transaction")
                {
                    SbsaveReferences.AppendLine("update tbltransfdcreation set isreferralapplicable='" + ReferralDetails.pIsReferralsapplicable + "' where fdaccountid=" + ReferralDetails.pAccountId + " and fdaccountno='" + ReferralDetails.paccountNo + "';");

                }
                else if (ReferralDetails.paccounttype == "Savings Transaction")
                {
                    SbsaveReferences.AppendLine("update tbltranssavingaccountcreation set isreferralapplicable='" + ReferralDetails.pIsReferralsapplicable + "' where savingaccountid=" + ReferralDetails.pAccountId + " and savingaccountno='" + ReferralDetails.paccountNo + "';");
                }

                if (ReferralDetails.pIsReferralsapplicable)
                {
                    if (Convert.ToString(ReferralDetails.pTypeofOperation) == "CREATE")
                    {
                        SbsaveReferences.AppendLine("INSERT INTO tbltransreferraldetails(accounttype,accountid, accountno, referralid, referralcode,contactid,referralname,employeeid,salespersonname, commsssionvalue, commsssiontype,statusid, createdby, createddate) VALUES (  '" + ReferralDetails.paccounttype + "'," + ReferralDetails.pAccountId + ", '" + ReferralDetails.paccountNo + "', '" + ReferralDetails.pReferralId + "','" + ReferralDetails.pReferralCode + "'," + ReferralDetails.pContactId + ", '" + ReferralDetails.pReferralname + "', '" + ReferralDetails.pEmployeeidId + "', '" + ReferralDetails.pSalesPersonName + "'," + ReferralDetails.pCommisionValue + ",'" + ReferralDetails.pCommissionType + "', " + Convert.ToInt32(Status.Active) + "," + ReferralDetails.pCreatedby + ", current_timestamp);");
                    }
                    else if (Convert.ToString(ReferralDetails.pTypeofOperation) == "UPDATE")
                    {
                        SbsaveReferences.AppendLine("Update tbltransreferraldetails set referralid='" + ReferralDetails.pReferralId + "',referralcode='" + ReferralDetails.pReferralCode + "',contactid=" + ReferralDetails.pContactId + ",referralname='" + ReferralDetails.pReferralname + "',employeeid='" + ReferralDetails.pEmployeeidId + "',salespersonname='" + ReferralDetails.pSalesPersonName + "',commsssionvalue=" + ReferralDetails.pCommisionValue + ",modifiedby=" + ReferralDetails.pCreatedby + ",modifieddate=current_timestamp where accountid=" + ReferralDetails.pAccountId + " and accountno='" + ReferralDetails.paccountNo + "';");
                    }
                }
                if (!string.IsNullOrEmpty(SbsaveReferences.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbsaveReferences.ToString());
                    trans.Commit();
                    IsSaved = true;
                }
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
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
        #endregion

        #region share View Data
        public async Task<List<ShareApplicationDTO>> BindShareApplicationView(string ConnectionString)
        {
            List<ShareApplicationDTO> lstShareApplicationDTO = new List<ShareApplicationDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT share_account_id,share_Account_number,share_config_id, member_type_id,member_id,trans_date,reference_no,face_value, no_of_shares_issued, distinctive_from,distinctive_to, total_amount,is_member_fee_applicable,applicant_type,member_name ,(select (case when count(share_account_id)> 0 then true else false end) ReceiptStatus from Share_receipt tt where tt.share_account_id=t.share_account_id)from Share_Account_creation t where statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            ShareApplicationDTO objShareApplicationDTO = new ShareApplicationDTO();
                            objShareApplicationDTO.pshareapplicationid = Convert.ToInt64(dr["share_account_id"]);
                            objShareApplicationDTO.pShareAccountNo = Convert.ToString(dr["share_Account_number"]);
                            objShareApplicationDTO.pshareconfigid = Convert.ToInt64(dr["share_config_id"]);
                            objShareApplicationDTO.pmembertypeid = Convert.ToInt64(dr["member_type_id"]);
                            objShareApplicationDTO.pmemberid = Convert.ToInt64(dr["member_id"]);
                            objShareApplicationDTO.pTransdate = Convert.ToString(dr["trans_date"]);
                            objShareApplicationDTO.preferenceno = Convert.ToString(dr["reference_no"]);
                            objShareApplicationDTO.pfacevalue = Convert.ToDecimal(dr["face_value"]);
                            objShareApplicationDTO.pnoofsharesissued = Convert.ToInt64(dr["no_of_shares_issued"]);
                            objShareApplicationDTO.pdistinctivefrom = Convert.ToInt64(dr["distinctive_from"]);
                            objShareApplicationDTO.pdistinctiveto = Convert.ToInt64(dr["distinctive_to"]);
                            objShareApplicationDTO.ptotalamount = Convert.ToDecimal(dr["total_amount"]);
                            objShareApplicationDTO.pismemberfeeapplicable = Convert.ToBoolean(dr["is_member_fee_applicable"]);
                            objShareApplicationDTO.pApplicanttype = Convert.ToString(dr["applicant_type"]);
                            objShareApplicationDTO.pmembername = Convert.ToString(dr["member_name"]);
                            objShareApplicationDTO.pReceiptStatus = dr["ReceiptStatus"];
                            lstShareApplicationDTO.Add(objShareApplicationDTO);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstShareApplicationDTO;
        }

        #endregion

        public ShareApplicationDTO BindShareDetailsBasedonApplicationid(long ShareApplicationid, string ConnectionString)
        {
            ShareApplicationDTO objShareApplicationDTO = new ShareApplicationDTO();
            //try
            //{
            //    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT shareapplicationid,ismemberfeeapplicable,membertypeid, membertype,memberid,membercode,membername,contactid, contacttype, contactreferenceid,sharenamecode, shareconfigid,sharename,referenceno, facevalue,noofsharesissued, distinctivefrom, distinctiveto, totalamount,sharedate, shareissuedate, printstatus, sharestatus, tokenno FROM tbltransshareapplication where shareapplicationid=" + ShareApplicationid + ";"))
            //    {
            //        while (dr.Read())
            //        {

            //            objShareApplicationDTO.pshareapplicationid = Convert.ToInt64(dr["shareapplicationid"]);
            //            objShareApplicationDTO.pismemberfeeapplicable = Convert.ToBoolean(dr["ismemberfeeapplicable"]);
            //            objShareApplicationDTO.pmembertypeid = Convert.ToInt64(dr["membertypeid"]);
            //            objShareApplicationDTO.pmembertype = Convert.ToString(dr["membertype"]);
            //            objShareApplicationDTO.pmemberid = Convert.ToInt64(dr["memberid"]);
            //            objShareApplicationDTO.pmembercode = Convert.ToString(dr["membercode"]);
            //            objShareApplicationDTO.pmembername = Convert.ToString(dr["membername"]);
            //            objShareApplicationDTO.pcontactid = Convert.ToInt64(dr["contactid"]);
            //            objShareApplicationDTO.pcontacttype = Convert.ToString(dr["contacttype"]);
            //            objShareApplicationDTO.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
            //            objShareApplicationDTO.psharenamecode = Convert.ToString(dr["sharenamecode"]);
            //            objShareApplicationDTO.pshareconfigid = Convert.ToInt64(dr["shareconfigid"]);
            //            objShareApplicationDTO.psharename = Convert.ToString(dr["sharename"]);
            //            objShareApplicationDTO.preferenceno = Convert.ToString(dr["referenceno"]);
            //            objShareApplicationDTO.pfacevalue = Convert.ToDecimal(dr["facevalue"]);
            //            objShareApplicationDTO.pnoofsharesissued = Convert.ToInt64(dr["noofsharesissued"]);
            //            objShareApplicationDTO.pdistinctivefrom = Convert.ToInt64(dr["distinctivefrom"]);
            //            objShareApplicationDTO.pdistinctiveto = Convert.ToInt64(dr["distinctiveto"]);
            //            objShareApplicationDTO.ptotalamount = Convert.ToDecimal(dr["totalamount"]);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            return objShareApplicationDTO;
        }


        public List<jointdetails> GetJointMembersListInEdit(string AccountNo, string Accounttype, string ConnectionString)
        {
            var _MemberJointDetailsList = new List<jointdetails>();
            try
            {
                //Changed By Sai Mahesh 15 JAN 2021
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, memberid,membercode,membername,contactid,contacttype,contactreferenceid from tbltransrdjointdetails  where  rdaccountno = '" + AccountNo + "' and accounttype='" + Accounttype + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select jointdetailsid, tj.memberid, membercode, membername, contactid, contacttype, contactreferenceid from tbltransjointdetails tj join tblmstmembers tm on tm.memberid = tj.memberid where accountno = '" + AccountNo + "' and accounttype='" + Accounttype + "' and tj.statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        jointdetails _MemberJointDetails = new jointdetails
                        {
                            precordid = Convert.ToInt64(dr["jointdetailsid"]),
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pMemberCode = Convert.ToString(dr["membercode"]),
                            pMemberName = Convert.ToString(dr["membername"]),
                            pContactid = Convert.ToInt64(dr["contactid"]),
                            pContacttype = Convert.ToString(dr["contacttype"]),
                            pContactrefid = Convert.ToString(dr["contactreferenceid"]),
                            pTypeofOperation = "OLD"
                        };
                        _MemberJointDetailsList.Add(_MemberJointDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _MemberJointDetailsList;
        }

        public List<NomineeDetails> GetNomineesListInEdit(string AccountNo, string Accounttype, string ConnectionString)
        {
            var _MemberNomineeDetailsList = new List<NomineeDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,vchapplicationid,contactid,contactreferenceid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, applicantype,coalesce(percentage,0) as percentage,statusid from tabapplicationpersonalnomineedetails where vchapplicationid='" + AccountNo + "' and applicantype='" + Accounttype + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        var _TransNomineeDetails = new NomineeDetails
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pnomineename = Convert.ToString(dr["nomineename"]),
                            prelationship = Convert.ToString(dr["relationship"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pMemberrefcode = Convert.ToString(dr["vchapplicationid"]),
                            pcontactno = Convert.ToString(dr["contactno"]),
                            pidproofname = Convert.ToString(dr["idproofname"]),
                            pdocidproofpath = Convert.ToString(dr["docidproofpath"]),
                            preferencenumber = Convert.ToString(dr["referencenumber"]),
                            pisprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                            pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false,
                            pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                            pAge = dr["dateofbirth"] != DBNull.Value ? CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"])) : 0,
                            ptypeofoperation = "UPDATE",
                            pidprooftype = Convert.ToString(dr["idprooftype"]),
                            //  pcontactid = Convert.ToInt64(dr["contactid"]),
                            pPercentage = Convert.ToDecimal(dr["percentage"])
                        };
                        _MemberNomineeDetailsList.Add(_TransNomineeDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _MemberNomineeDetailsList;
        }

        public bool DeleteShareDetails(long ShareApplicationId, string Connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(ShareApplicationId.ToString()) && ShareApplicationId != 0)
                {
                    sbInsert.Append("update Share_Account_creation set statusid=" + Convert.ToInt32(Status.Inactive) + " where share_account_id=" + ShareApplicationId + ";");
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }

                trans.Commit();
                IsSaved = true;
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

        public Referrals getReferralDetails(string ConnectionString, string accountno, string Accounttype)
        {
            Referrals _Referraldata = new Referrals();

            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT referraldetailsid, accounttype, accountid, accountno, referralid,referralcode, contactid, referralname, employeeid, salespersonname,commsssionvalue, commsssiontype FROM tbltransreferraldetails where accountno ='" + accountno + "' and statusid =" + Convert.ToInt32(Status.Active) + " and accounttype='" + Accounttype + "';"))

                {
                    if (dr.Read())
                    {
                        _Referraldata = new Referrals()
                        {
                            preferaldetailsid = Convert.ToInt64(dr["referraldetailsid"]),
                            pAccountId = Convert.ToInt64(dr["accountid"]),
                            paccounttype = Convert.ToString(dr["accounttype"]),
                            paccountNo = Convert.ToString(dr["accountno"]),
                            pReferralId = Convert.ToInt64(dr["referralid"]),
                            pReferralCode = Convert.ToString(dr["referralcode"]),
                            pContactId = Convert.ToInt64(dr["contactid"]),
                            pReferralname = Convert.ToString(dr["referralname"]),
                            pEmployeeidId = Convert.ToInt64(dr["employeeid"]),
                            pSalesPersonName = Convert.ToString(dr["salespersonname"]),
                            pCommisionValue = Convert.ToDecimal(dr["commsssionvalue"]),
                            pCommissionType = Convert.ToString(dr["commsssiontype"]),
                            pTypeofOperation = Convert.ToString("UPDATE")



                        };

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Referraldata;
        }


    }
}
