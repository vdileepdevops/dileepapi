using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class FIMemberDAL : SettingsDAL, IFIMember
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        DataSet ds = null;
        public List<FIMembertypeDTO> _FIMembersList { get; set; }
        public List<FIApplicantTypeDTO> _FIApplicantTypesList { get; set; }
        public bool SaveFIMember(FIMemberDTO _FIMemberDTO, string ConnectionString)
        {
            StringBuilder sbSaveFIMember = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                _FIMemberDTO._FIMemberPersonalInformationDTO.pContactId = _FIMemberDTO._FiMemberContactDetailsDTO.pContactId;
                // Next Id-Generation of Member Reference Id

                if (string.IsNullOrEmpty(_FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate))
                {
                    _FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate = "null";
                }
                else
                {
                    _FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate = "'" + FormatDate(_FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate) + "'";
                }               
                if (!string.IsNullOrEmpty(_FIMemberDTO.ptypeofoperation) && _FIMemberDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                {                   
                    if (string.IsNullOrEmpty(_FIMemberDTO.pMemberReferenceid))
                    {
                            _FIMemberDTO.pMemberReferenceid = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('MEMBER','" + ManageQuote(_FIMemberDTO._FiMemberContactDetailsDTO.pMemberType) + "'," + _FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate + ")").ToString();
                    }
                    // Contact-Member Save
                    _FIMemberDTO.pMemberId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tblmstmembers   ( membercode,contactid, contacttype, contactreferenceid, membername,membertype, membertypeid, memberstatus, statusid,     createdby,createddate,ispersonaldetailsapplicable,applicanttype,transdate)  VALUES ('" + ManageQuote(_FIMemberDTO.pMemberReferenceid) + "', " + _FIMemberDTO._FiMemberContactDetailsDTO.pContactId + ", '" + _FIMemberDTO._FiMemberContactDetailsDTO.pContacttype + "', '" + _FIMemberDTO._FiMemberContactDetailsDTO.pContactReferenceId + "', '" + _FIMemberDTO._FiMemberContactDetailsDTO.pContactName + "','" + _FIMemberDTO._FiMemberContactDetailsDTO.pMemberType + "', " + _FIMemberDTO._FiMemberContactDetailsDTO.pMembertypeId + ",    '" + _FIMemberDTO._FiMemberContactDetailsDTO.pMemberStatus + "', " + Convert.ToInt32(Status.Active) + ", " + _FIMemberDTO.pCreatedby + ",current_timestamp,'" + _FIMemberDTO.ispersonaldetailsapplicable + "','" + ManageQuote(_FIMemberDTO._FiMemberContactDetailsDTO.pApplicantType) + "'," + _FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate + ") returning memberid; "));
                }
                else if(!string.IsNullOrEmpty(_FIMemberDTO.ptypeofoperation) && _FIMemberDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                {
                    sbSaveFIMember.AppendLine("Update tblmstmembers set applicanttype='" + ManageQuote(_FIMemberDTO._FiMemberContactDetailsDTO.pApplicantType) + "', transdate=" + _FIMemberDTO._FiMemberContactDetailsDTO.pFIMemberDate + ", membername='" + _FIMemberDTO._FiMemberContactDetailsDTO.pContactName + "', membertype='" + _FIMemberDTO._FiMemberContactDetailsDTO.pMemberType + "'," +
                        "membertypeid=" + _FIMemberDTO._FiMemberContactDetailsDTO.pMembertypeId + ",memberstatus='" + _FIMemberDTO._FiMemberContactDetailsDTO.pMemberStatus + "' , modifieddate=current_timestamp,modifiedby=" + _FIMemberDTO.pCreatedby + " where membercode='"+ _FIMemberDTO.pMemberReferenceid + "';");
                }

                // Kyc Details
                if (_FIMemberDTO._FIMemberKYCDocumentsDTO.documentstorelist != null && _FIMemberDTO._FIMemberKYCDocumentsDTO.documentstorelist.Count > 0)
                {
                    ReferralAdvocateDAL objReferralAdvocateDAL = new ReferralAdvocateDAL();
                    sbSaveFIMember.AppendLine(objReferralAdvocateDAL.UpdateStoreDetails(_FIMemberDTO._FIMemberKYCDocumentsDTO.documentstorelist, ConnectionString, 0, _FIMemberDTO._FiMemberContactDetailsDTO.pContactId));
                }
                // Personal Details
                
                sbSaveFIMember.AppendLine(SaveFIMemberPersonalDetails(_FIMemberDTO._FIMemberPersonalInformationDTO, ConnectionString, _FIMemberDTO.pMemberReferenceid, _FIMemberDTO.pMemberId));
                // Reference Details
                sbSaveFIMember.AppendLine(SaveFIMemberReferenceData(_FIMemberDTO.lobjAppReferences, _FIMemberDTO.pMemberId, _FIMemberDTO.pMemberReferenceid, _FIMemberDTO.pCreatedby));
                // Referral Details
                sbSaveFIMember.AppendLine(SaveFIMemberReferralData(_FIMemberDTO._FiMemberReferralDTO, _FIMemberDTO.pMemberId, _FIMemberDTO.pMemberReferenceid));
                if(Convert.ToString(sbSaveFIMember)!=string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbSaveFIMember.ToString());
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
        public string SaveFIMemberPersonalDetails(FIMemberPersonalInformationDTO _FIMemberPersonalInformationDTO,string Connectionstring,string MemberReferenceId,long pMemberId)
        {
            StringBuilder sbQuery = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();
            try
            {
                if (_FIMemberPersonalInformationDTO._FIMemberEmployeementList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberEmployeementList.Count; i++)
                    {                       
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment = "null";
                        }
                        else
                        {
                            _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment = "'" + FormatDate(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment) + "'";
                        }
                        if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement = "null";
                        }
                        else
                        {
                            _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement = "'" + FormatDate(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement) + "'";
                        }

                        long ApplicableSectionsCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select count(*) from tabapplicationpersonalapplicablesections where vchapplicationid='" + ManageQuote(MemberReferenceId) + "' and contactid = " + _FIMemberPersonalInformationDTO.pContactId + ""));

                        if(ApplicableSectionsCount<=0)
                        {
                            sbQuery.AppendLine("insert into tabapplicationpersonalapplicablesections( applicationid, vchapplicationid, contactid, contactreferenceid,isemplymentdetailsapplicable, contacttype,statusid,createdby,createddate ) values (" + pMemberId + ",'" + ManageQuote(MemberReferenceId) + "'," + _FIMemberPersonalInformationDTO.pContactId + ",'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "','" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable) + "','MEMBER'," + Convert.ToInt32(Status.Active) + "," + _FIMemberPersonalInformationDTO.pCreatedby + ",current_timestamp);");
                        }    
                        else
                        {
                            sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isemplymentdetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "';");
                        }

                        long ApplicableKycCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, " select count(*)  from tabapplicationkyccreditdetailsapplicablesections where contactid = " + _FIMemberPersonalInformationDTO.pContactId + " and vchapplicationid = '" + ManageQuote(MemberReferenceId) + "';"));

                        if (ApplicableKycCount <= 0)
                        {
                            sbQuery.AppendLine("insert into tabapplicationkyccreditdetailsapplicablesections(applicationid, vchapplicationid, contactid, contactreferenceid, contacttype, statusid, createdby, createddate) values (" + pMemberId + ",'" + ManageQuote(MemberReferenceId) + "'," + _FIMemberPersonalInformationDTO.pContactId + ",'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "','MEMBER'," + Convert.ToInt32(Status.Active) + "," + _FIMemberPersonalInformationDTO.pCreatedby + ",current_timestamp);");
                        }


                    if (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation == "CREATE")
                        {                          
                            if (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalemplymentdetails(applicationid,vchapplicationid,contactid,contactreferenceid,applicanttype,isemploymentapplicable,employmenttype,nameoftheorganization,natureoftheorganization,employmentrole,officeaddress,officephoneno,reportingto,employeeexp,employeeexptype,totalworkexp,dateofestablishment,dateofcommencement,gstinno,cinno,dinno,tradelicenseno,statusid,createdby,createddate)values('" + (pMemberId) + "','" + ManageQuote(MemberReferenceId) + "','" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactid) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].papplicanttype) + "','" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemploymenttype) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pnameoftheorganization) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pEnterpriseType) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemploymentrole) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pofficeaddress) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pofficephoneno) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].preportingto) + "','" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemployeeexp) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemployeeexptype) + "','" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptotalworkexp) + "'," + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment) + "," + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement) + ",'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pgstinno) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcinno) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdinno) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptradelicenseno) + "'," + Convert.ToInt32(Status.Active) + ",'" + (_FIMemberPersonalInformationDTO.pCreatedby) + "',current_timestamp); ");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation == "UPDATE" || _FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptypeofoperation == "OLD")
                        {                          

                            if (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalemplymentdetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "', applicanttype='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].papplicanttype) + "', isemploymentapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pisapplicable) + "', employmenttype='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemploymenttype) + "', nameoftheorganization='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pnameoftheorganization) + "', natureoftheorganization='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pEnterpriseType) + "', employmentrole='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemploymentrole) + "', officeaddress='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pofficeaddress) + "', officephoneno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pofficephoneno) + "', reportingto='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].preportingto) + "', employeeexp='" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemployeeexp) + "', employeeexptype='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pemployeeexptype) + "', totalworkexp='" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptotalworkexp) + "', dateofestablishment=" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofestablishment) + ", dateofcommencement=" + (_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdateofcommencement) + ", gstinno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pgstinno) + "', cinno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcinno) + "', dinno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pdinno) + "', tradelicenseno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].ptradelicenseno) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + _FIMemberPersonalInformationDTO.pCreatedby + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberEmployeementList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList.Count; i++)
                    {                       
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set ispersonalbirthdetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactreferenceid) + "';");

                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation == "CREATE")
                        {                          
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalbirthdetails( applicationid, vchapplicationid, contactid, contactreferenceid, residentialstatus, maritalstatus, placeofbirth, countryofbirth, nationality, minoritycommunity, statusid, createdby, createddate,applicantype) values('" + (pMemberId) + "','" + ManageQuote(MemberReferenceId) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactid) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactreferenceid) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].presidentialstatus) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pmaritalstatus) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pplaceofbirth) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcountryofbirth) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pnationality) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pminoritycommunity) + "'," + Convert.ToInt32(Status.Active) + ",'" + (_FIMemberPersonalInformationDTO.pCreatedby) + "',current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].papplicanttype) + "'); ");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation == "UPDATE" || _FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].ptypeofoperation == "OLD")
                        {                          
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalbirthdetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactreferenceid) + "', residentialstatus='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].presidentialstatus) + "', maritalstatus='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pmaritalstatus) + "', placeofbirth='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pplaceofbirth) + "', countryofbirth='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcountryofbirth) + "', nationality='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pnationality) + "', minoritycommunity='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pminoritycommunity) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalDetailsList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList.Count; i++)
                    {                       
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isfamilydetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactreferenceid) + "';");
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation == "CREATE")
                        {
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalfamilydetails( applicationid, vchapplicationid, contactid, contactreferenceid, totalnoofmembers, noofearningmembers, familytype, noofboyschild, noofgirlchild, houseownership, statusid, createdby, createddate,applicantype)values('" + (pMemberId) + "','" + ManageQuote(MemberReferenceId) + "','" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactid) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactreferenceid) + "','" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptotalnoofmembers) + "','" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofearningmembers) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pfamilytype) + "','" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofboyschild) + "','" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofgirlchild) + "','" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].phouseownership) + "'," + Convert.ToInt32(Status.Active) + ",'" + (_FIMemberPersonalInformationDTO.pCreatedby) + "',current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].papplicanttype) + "');");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation == "UPDATE" || _FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptypeofoperation == "OLD")
                        {                          
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalfamilydetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactreferenceid) + "', totalnoofmembers='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].ptotalnoofmembers) + "', noofearningmembers='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofearningmembers) + "', familytype='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pfamilytype) + "', noofboyschild='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofboyschild) + "', noofgirlchild='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pnoofgirlchild) + "', houseownership='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].phouseownership) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalFamilyList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
               string recordid1 = string.Empty;
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList.Count; i++)
                    {
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].precordid.ToString();
                            }
                        }                      
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth = "null";
                        }
                        else
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth = "'" + FormatDate(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth) + "'";
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isnomineedetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactreferenceid) + "';");
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation == "CREATE")
                        {                           
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee) values ('" + pMemberId + "', '" + ManageQuote(MemberReferenceId) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactreferenceid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pnomineename) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].prelationship) + "', " + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth) + ", '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactno) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pidprooftype) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pidproofname) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].preferencenumber) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdocidproofpath) + "', " + Convert.ToInt32(Status.Active) + ", '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].papplicanttype) + "'," + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pisprimarynominee) + ");");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].ptypeofoperation == "UPDATE")
                        {
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalnomineedetails set contactid = '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactid) + "', contactreferenceid = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactreferenceid) + "', nomineename = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pnomineename) + "', relationship = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].prelationship) + "', dateofbirth = " + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdateofbirth) + ", contactno = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pcontactno) + "', idprooftype = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pidprooftype) + "', idproofname = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pidproofname) + "', referencenumber = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].preferencenumber) + "', docidproofpath = '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pdocidproofpath) + "', statusid = " + Convert.ToInt32(Status.Active) + ", modifiedby = '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate = current_timestamp,isprimarynominee=" + (_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].pisprimarynominee) + " where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + pMemberId + " and recordid = " + _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList.ToString()) || _FIMemberPersonalInformationDTO._FIMemberPersonalNomineeList.Count == 0)
                    {

                        sbupdate.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "'; ");
                    }
                }
                recordid1 = string.Empty;
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList != null)
                {
                    //string recordid2 = string.Empty;
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalBankList.Count; i++)
                    {
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = _FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + _FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].precordid.ToString();
                            }
                        }                      
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isbankdetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pcontactreferenceid) + "';");
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation == "CREATE")
                        {                           
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalbankdetails(applicationid, vchapplicationid, contactid, contactreferenceid, bankname, accountno, ifsccode, branch, isprimarybank, statusid, createdby, createddate,applicantype) values ('" + (pMemberId) + "', '" + ManageQuote(MemberReferenceId) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pcontactid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pcontactreferenceid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankName) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankAccountNo) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankifscCode) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankBranch) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pIsprimaryAccount) + "', " + Convert.ToInt32(Status.Active) + ", '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].papplicanttype) + "');");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].ptypeofoperation == "UPDATE")
                        {
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalbankdetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pcontactreferenceid) + "', bankname='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankName) + "', accountno='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankAccountNo) + "', ifsccode='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankifscCode) + "', branch='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pBankBranch) + "', isprimarybank='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].pIsprimaryAccount) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and recordid =" + _FIMemberPersonalInformationDTO._FIMemberPersonalBankList[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.AppendLine("UPDATE tabapplicationpersonalbankdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalBankList.ToString()) || _FIMemberPersonalInformationDTO._FIMemberPersonalBankList.Count == 0)
                    {
                        sbupdate.AppendLine("UPDATE tabapplicationpersonalbankdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "'; ");
                    }
                }

                if (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList.Count; i++)
                    {                        
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactreferenceid) + "';");
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation == "CREATE")
                        {                           
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, grossannualincome, netannualincome, averageannualexpenses, statusid, createdby, createddate,applicantype) values ('" + (pMemberId) + "', '" + ManageQuote(MemberReferenceId) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactreferenceid) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pgrossannualincome) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pnetannualincome) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].paverageannualexpenses) + "', "+Convert.ToInt32(Status.Active)+", '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].papplicanttype) + "');");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation == "UPDATE" || _FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].ptypeofoperation == "OLD")
                        {                         
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalincomedetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactreferenceid) + "', grossannualincome='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pgrossannualincome) + "', netannualincome='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pnetannualincome) + "', averageannualexpenses='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].paverageannualexpenses) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalIncomeList[i].pcontactreferenceid) + "';");
                        }
                    }
                }

                recordid1 = string.Empty;
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList != null)
                {
                    //string recordid3 = string.Empty;
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList.Count; i++)
                    {
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].precordid.ToString();
                            }
                        }                       
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pcontactreferenceid) + "';");
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation == "CREATE")
                        {                          
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonalotherincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, sourcename, grossannual, statusid, createdby, createddate,applicantype) values ('" + (pMemberId) + "', '" + ManageQuote(MemberReferenceId) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pcontactid) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pcontactreferenceid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].psourcename) + "', '" + _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pgrossannual + "', " + Convert.ToInt32(Status.Active) + ", '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].papplicanttype) + "');");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].ptypeofoperation == "UPDATE")
                        {
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonalotherincomedetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pcontactreferenceid) + "', sourcename='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].psourcename) + "', grossannual='" + _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].pgrossannual + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and recordid =" + _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.AppendLine("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList.ToString()) || _FIMemberPersonalInformationDTO._FIMemberPersonalOtherIncomeList.Count == 0)
                    {
                        sbupdate.AppendLine("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + _FIMemberPersonalInformationDTO.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + pMemberId + " and vchapplicationid='" + ManageQuote(MemberReferenceId) + "'; ");
                    }
                }
                if (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList != null)
                {
                    for (int i = 0; i < _FIMemberPersonalInformationDTO._FIMemberPersonalEducationList.Count; i++)
                    {
                       
                        if (!string.IsNullOrEmpty(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation))
                        {
                            _FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation = _FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.AppendLine("update tabapplicationpersonalapplicablesections set iseducationdetailsapplicable='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pisapplicable) + "', modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and  contactreferenceid ='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactreferenceid) + "';");

                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation == "CREATE")
                        {                         
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pisapplicable == true)
                                sbQuery.AppendLine("insert into tabapplicationpersonaleducationdetails(applicationid, vchapplicationid, contactid, contactreferenceid, qualification, nameofthecourseorprofession, occupation, statusid, createdby, createddate,applicantype) values ('" + (pMemberId) + "', '" + ManageQuote(MemberReferenceId) + "', '" + (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactreferenceid) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pqualification) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pnameofthecourseorprofession) + "', '" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].poccupation) + "', " + Convert.ToInt32(Status.Active) + ", '" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', current_timestamp,'" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].papplicanttype) + "');");
                        }
                        if (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation == "UPDATE" || _FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].ptypeofoperation == "OLD")
                        {
                            if (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pisapplicable == true)
                                sbQuery.AppendLine("update tabapplicationpersonaleducationdetails set contactid='" + (_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactreferenceid) + "', qualification='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pqualification) + "', nameofthecourseorprofession='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pnameofthecourseorprofession) + "', occupation='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].poccupation) + "', statusid=" + Convert.ToInt32(Status.Active) + ", modifiedby='" + (_FIMemberPersonalInformationDTO.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(MemberReferenceId) + "' and applicationid = " + (pMemberId) + " and contactreferenceid='" + ManageQuote(_FIMemberPersonalInformationDTO._FIMemberPersonalEducationList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Convert.ToString(sbQuery) + " " + Convert.ToString(sbupdate);
        }
        public string SaveFIMemberReferenceData(List< FIMemberReferencesDTO> _FIMemberReferencesDTOlist, long MemberId, string MemberReferenceId,long UserId)
        {
            string Recordid = string.Empty;
            StringBuilder SbsaveReferences = new StringBuilder();
            StringBuilder sbDelete = new StringBuilder();
            try
            {
                if(_FIMemberReferencesDTOlist!=null && _FIMemberReferencesDTOlist.Count>0)
                {
                    for (int i = 0; i < _FIMemberReferencesDTOlist.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(_FIMemberReferencesDTOlist[i].ptypeofoperation))
                        {
                            if (_FIMemberReferencesDTOlist[i].ptypeofoperation.Trim().ToUpper() != "CREATE" && _FIMemberReferencesDTOlist[i].ptypeofoperation.Trim().ToUpper() != "OLD")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = _FIMemberReferencesDTOlist[i].pRefRecordId.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + _FIMemberReferencesDTOlist[i].pRefRecordId.ToString();
                                }
                        }
                        if (_FIMemberReferencesDTOlist[i].ptypeofoperation.Trim().ToUpper() == "UPDATE")
                        {
                                SbsaveReferences.AppendLine("UPDATE tblmstmemberreferences SET firstname ='" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefFirstname).Trim() + "', lastname ='" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefLastname).Trim() + "', contactnumber =" + _FIMemberReferencesDTOlist[i].pRefcontactNo + ", " +
                                    "alternatenumber =" + _FIMemberReferencesDTOlist[i].pRefalternatecontactNo + ", emailid ='" + ManageQuote( _FIMemberReferencesDTOlist[i].pRefEmailID ).Trim()+ "',alternateemailid='"+ ManageQuote(_FIMemberReferencesDTOlist[i].pRefEmailID).Trim() + "', modifiedby =" + UserId + ", modifieddate =current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " WHERE membercode='" + ManageQuote(MemberReferenceId) + "' and memberid=" + MemberId + " and recordid="+ _FIMemberReferencesDTOlist [i].pRefRecordId+ "; ");
                        }
                        else if (_FIMemberReferencesDTOlist[i].ptypeofoperation.Trim().ToUpper() == "CREATE")
                        {
                                SbsaveReferences.AppendLine("INSERT INTO tblmstmemberreferences(memberid,membercode, firstname, lastname, contactnumber, alternatenumber, emailid, alternateemailid, statusid, createdby, createddate)   VALUES (" + MemberId + ", '" + ManageQuote(MemberReferenceId) + "','" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefFirstname) + "', '" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefLastname) + "', " + _FIMemberReferencesDTOlist[i].pRefcontactNo + ", " + _FIMemberReferencesDTOlist[i].pRefalternatecontactNo + ", '" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefEmailID) + "', '" + ManageQuote(_FIMemberReferencesDTOlist[i].pRefAlternateEmailId) + "', " + Convert.ToInt32(Status.Active) + ", " + UserId + ", current_timestamp);");
                        }                       
                    }
                }

                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + UserId + ",modifieddate=current_timestamp where recordid not in(" + Recordid + ");");
                    }
                    else
                    {
                        sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + UserId + ",modifieddate=current_timestamp where memberid=" + MemberId + ";");
                    }                  
                }
                else
                {
                    sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + UserId + ",modifieddate=current_timestamp where memberid=" + MemberId + ";");
                }            
            }
            catch (Exception)
            {
                throw;
            }
            return Convert.ToString(sbDelete) + " " + Convert.ToString(SbsaveReferences);
        }
        public string SaveFIMemberReferralData(FiMemberReferralDTO _FiMemberReferralDTO,  long MemberId, string MemberReferenceId)
        {
            StringBuilder SbsaveReferrals = new StringBuilder();
            try
            {
                if (_FiMemberReferralDTO.pIsReferralApplicableorNot)
                {
                    if (!string.IsNullOrEmpty(_FiMemberReferralDTO.ptypeofoperation))
                    {
                        if (_FiMemberReferralDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                        {
                            SbsaveReferrals.AppendLine("INSERT INTO tblmstmemberreferraldetails(memberid,membercode, isreferralcomexist, commissionpayouttype,commissionpayout, istdsapplicable, tdsaccountid, tdssection,tdspercentage, statusid, createdby, createddate ) VALUES (" + MemberId + ", '" + ManageQuote(MemberReferenceId) + "','" + (_FiMemberReferralDTO.pIsReferralApplicableorNot) + "', '" + ManageQuote(_FiMemberReferralDTO.pCommisionPayoutType) + "', " + _FiMemberReferralDTO.pCommisionPayoutAmountorPercentile + ", '" + _FiMemberReferralDTO.pIsTdsapplicable + "', '" + ManageQuote(_FiMemberReferralDTO.pTDSAccountId) + "','" + ManageQuote(_FiMemberReferralDTO.pTdsSection) + "', " + _FiMemberReferralDTO.pTdsPercentage + ", " + Convert.ToInt32(Status.Active) + ", " + _FiMemberReferralDTO.pCreatedby + ", current_timestamp);");
                        }
                        else if (_FiMemberReferralDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                        {
                            SbsaveReferrals.AppendLine("update tblmstmemberreferraldetails set isreferralcomexist='" + (_FiMemberReferralDTO.pIsReferralApplicableorNot) + "',commissionpayouttype='" + ManageQuote(_FiMemberReferralDTO.pCommisionPayoutType) + "',commissionpayout=" + _FiMemberReferralDTO.pCommisionPayoutAmountorPercentile + ",istdsapplicable='" + _FiMemberReferralDTO.pIsTdsapplicable + "',tdsaccountid='" + ManageQuote(_FiMemberReferralDTO.pTDSAccountId) + "',tdssection='" + ManageQuote(_FiMemberReferralDTO.pTdsSection) + "',tdspercentage=" + _FiMemberReferralDTO.pTdsPercentage + ",statusid=" + Convert.ToInt32(Status.Active) + ",modifiedby=" + _FiMemberReferralDTO.pCreatedby + ",modifieddate=current_timestamp where  membercode='" + MemberReferenceId + "' and MemberId=" + MemberId + "; ");
                        }
                    }
                }
                else
                {
                    SbsaveReferrals.AppendLine("Delete from tblmstmemberreferraldetails where membercode='" + ManageQuote(MemberReferenceId) + "' and memberid=" + MemberId + ";");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Convert.ToString(SbsaveReferrals);
        }
        public FIMemberPersonalInformationDTO GetFIMemberPersonalInformation(string strapplictionid, string ConnectionString)
        {
            ds = new DataSet();
            FIMemberPersonalInformationDTO FIPersonalInformationDTO = new FIMemberPersonalInformationDTO();
            FIPersonalInformationDTO._FIMemberEmployeementList = new List<FIMemberEmployeementDTO>();
            FIPersonalInformationDTO._FIMemberPersonalDetailsList = new List<FIMemberPersonalDetailsDTO>();
            FIPersonalInformationDTO._FIMemberPersonalFamilyList = new List<FIMemberPersonalFamilyDTO>();
            FIPersonalInformationDTO._FIMemberPersonalNomineeList = new List<FIMemberPersonalNomineeDTO>();
            FIPersonalInformationDTO._FIMemberPersonalBankList = new List<FIMemberPersonalBankDTO>();
            FIPersonalInformationDTO._FIMemberPersonalIncomeList = new List<FIMemberPersonalIncomeDTO>();
            FIPersonalInformationDTO._FIMemberPersonalOtherIncomeList = new List<FIMemberPersonalOtherIncomeDTO>();
            FIPersonalInformationDTO._FIMemberPersonalEducationList = new List<FIMemberPersonalEducationDTO>();
            try
            {
                bool ispersonaldetailsapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select ispersonaldetailsapplicable from tblmstmembers where upper(membercode) = '" + ManageQuote(strapplictionid).ToUpper() + "';"));
                FIPersonalInformationDTO.pIspersonaldetailsapplicable = ispersonaldetailsapplicable;
                FIPersonalInformationDTO.pMemberReferenceId = strapplictionid;

                // Reads Data only if Personal Data Exists
                if (ispersonaldetailsapplicable)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,contactid,contactreferenceid,coalesce(applicanttype,'') as applicanttype, coalesce(isemploymentapplicable,false) as isemploymentapplicable, coalesce(employmenttype,'') as employmenttype,coalesce(nameoftheorganization,'') as nameoftheorganization,coalesce(natureoftheorganization,'') as natureoftheorganization,coalesce( employmentrole,'') as employmentrole,coalesce( officeaddress,'') as officeaddress, coalesce(officephoneno,'') as officephoneno,coalesce(reportingto,'') as reportingto,coalesce( employeeexp,0) as employeeexp,coalesce( employeeexptype,'') as employeeexptype, coalesce(totalworkexp,0) as totalworkexp,coalesce( dateofestablishment,null) as dateofestablishment,coalesce(dateofcommencement,null) as dateofcommencement, coalesce(gstinno,'') as gstinno,coalesce(cinno,'') as cinno,coalesce(dinno,'') as dinno,coalesce(tradelicenseno,'') as  tradelicenseno FROM tabapplicationpersonalemplymentdetails where  upper(vchapplicationid) ='" + ManageQuote(strapplictionid).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberEmployeementList.Add(new FIMemberEmployeementDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                papplicanttype = dr["applicanttype"].ToString(),
                                pisapplicable = true,
                                pemploymenttype = dr["employmenttype"].ToString(),
                                pnameoftheorganization = dr["nameoftheorganization"].ToString(),

                                pEnterpriseType = dr["natureoftheorganization"].ToString(),
                                pemploymentrole = dr["employmentrole"].ToString(),
                                pofficeaddress = dr["officeaddress"].ToString(),
                                pofficephoneno = dr["officephoneno"].ToString(),
                                preportingto = dr["reportingto"].ToString(),
                                pemployeeexp = Convert.ToInt32(dr["employeeexp"]),
                                pemployeeexptype = dr["employeeexptype"].ToString(),
                                ptotalworkexp = Convert.ToInt32(dr["totalworkexp"]),
                                pdateofestablishment = dr["dateofestablishment"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofestablishment"]).ToString("dd/MM/yyyy"),

                                pdateofcommencement = dr["dateofcommencement"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofcommencement"]).ToString("dd/MM/yyyy"),

                                pgstinno = dr["gstinno"].ToString(),
                                pcinno = dr["cinno"].ToString(),
                                pdinno = dr["dinno"].ToString(),
                                ptradelicenseno = dr["tradelicenseno"].ToString(),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype,vchapplicationid, contactid,contactreferenceid,coalesce(residentialstatus,'') as residentialstatus,coalesce(maritalstatus,'') as maritalstatus,coalesce(placeofbirth,'') as placeofbirth, coalesce(countryofbirth,'') as countryofbirth,coalesce(nationality,'') as nationality,coalesce(minoritycommunity,'') as minoritycommunity FROM tabapplicationpersonalbirthdetails where upper(vchapplicationid) ='" + ManageQuote(strapplictionid).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalDetailsList.Add(new FIMemberPersonalDetailsDTO
                            {

                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = dr["contactid"].ToString(),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                presidentialstatus = dr["residentialstatus"].ToString(),
                                papplicanttype = dr["applicantype"].ToString(),
                                pmaritalstatus = dr["maritalstatus"].ToString(),
                                pplaceofbirth = dr["placeofbirth"].ToString(),
                                pcountryofbirth = dr["countryofbirth"].ToString(),
                                pnationality = dr["nationality"].ToString(),
                                pminoritycommunity = dr["minoritycommunity"].ToString(),
                                ptypeofoperation = "OLD",
                                pisapplicable = true
                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,coalesce(totalnoofmembers,0) as totalnoofmembers,coalesce(noofearningmembers,0) as noofearningmembers,coalesce(familytype,'') as familytype,coalesce(noofboyschild,0) as noofboyschild,coalesce(noofgirlchild,0) as noofgirlchild,coalesce(houseownership,'') as houseownership FROM tabapplicationpersonalfamilydetails where  upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalFamilyList.Add(new FIMemberPersonalFamilyDTO
                            {

                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                ptotalnoofmembers = Convert.ToInt32(dr["totalnoofmembers"]),
                                pnoofearningmembers = Convert.ToInt32(dr["noofearningmembers"]),
                                pfamilytype = dr["familytype"].ToString(),
                                papplicanttype = dr["applicantype"].ToString(),
                                pnoofboyschild = Convert.ToInt32(dr["noofboyschild"]),
                                pnoofgirlchild = Convert.ToInt32(dr["noofgirlchild"]),
                                phouseownership = dr["houseownership"].ToString(),
                                ptypeofoperation = "OLD",
                                pisapplicable = true
                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,coalesce(nomineename,'') as nomineename,coalesce(relationship,'') as relationship, coalesce(dateofbirth,null) as dateofbirth, coalesce(contactno,'') as contactno, coalesce(idprooftype,'') as idprooftype, coalesce(idproofname,'') as idproofname,coalesce( referencenumber,'') as referencenumber, coalesce(docidproofpath,'') as docidproofpath,coalesce(isprimarynominee,false) as isprimarynominee FROM tabapplicationpersonalnomineedetails where  upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + " and applicantype='MEMBER';"))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalNomineeList.Add(new FIMemberPersonalNomineeDTO
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
                                ptypeofoperation = "OLD",
                                pisapplicable = true

                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,bankname, coalesce(accountno,'') as accountno, coalesce(ifsccode,'') as ifsccode, coalesce(branch,'') as branch, coalesce(isprimarybank,false) as isprimarybank FROM tabapplicationpersonalbankdetails where upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalBankList.Add(new FIMemberPersonalBankDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                papplicanttype = dr["applicantype"].ToString(),                                
                                pBankName = dr["bankname"].ToString(),
                                pBankAccountNo = dr["accountno"].ToString(),
                                pBankifscCode = dr["ifsccode"].ToString(),
                                pBankBranch = dr["branch"].ToString(),
                                pIsprimaryAccount = Convert.ToBoolean(dr["isprimarybank"].ToString()),
                                ptypeofoperation = "OLD",
                                pisapplicable = true
                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,coalesce(grossannualincome,0) as grossannualincome,coalesce(netannualincome,0) as netannualincome, coalesce(averageannualexpenses,0) as averageannualexpenses FROM tabapplicationpersonalincomedetails where  upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalIncomeList.Add(new FIMemberPersonalIncomeDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                papplicanttype = dr["applicantype"].ToString(),
                                pgrossannualincome = Convert.ToDecimal(dr["grossannualincome"]),
                                pnetannualincome = Convert.ToDecimal(dr["netannualincome"]),
                                paverageannualexpenses = Convert.ToDecimal(dr["averageannualexpenses"]),
                                ptypeofoperation = "OLD",
                                pisapplicable = true

                            }); ;
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,applicantype,vchapplicationid,contactid,contactreferenceid,coalesce(sourcename,'') as sourcename,coalesce(grossannual,0) as grossannual FROM tabapplicationpersonalotherincomedetails where upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalOtherIncomeList.Add(new FIMemberPersonalOtherIncomeDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                papplicanttype = dr["applicantype"].ToString(),
                                psourcename = dr["sourcename"].ToString(),
                                pgrossannual = Convert.ToDecimal(dr["grossannual"]),
                                ptypeofoperation = "OLD",
                                pisapplicable = true
                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,coalesce(qualification,'') as qualification, coalesce(nameofthecourseorprofession,'') as nameofthecourseorprofession, coalesce(occupation,'') as occupation FROM tabapplicationpersonaleducationdetails where  upper(vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FIPersonalInformationDTO._FIMemberPersonalEducationList.Add(new FIMemberPersonalEducationDTO
                            {
                                precordid = Convert.ToInt64(dr["recordid"]),
                                pcontactid = Convert.ToInt64(dr["contactid"]),
                                papplicanttype = dr["applicantype"].ToString(),

                                pcontactreferenceid = dr["contactreferenceid"].ToString(),
                                pqualification = dr["qualification"].ToString(),
                                pnameofthecourseorprofession = dr["nameofthecourseorprofession"].ToString(),
                                poccupation = dr["occupation"].ToString(),
                                ptypeofoperation = "OLD",
                                pisapplicable = true
                            });
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            return FIPersonalInformationDTO;

        }
        public List< FIMemberReferencesDTO> GetFIMemberReferenceInformation(string strapplictionid, string ConnectionString)
        {            
            var _FIMemberReferencesList = new List<FIMemberReferencesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, memberid,membercode, firstname, lastname, coalesce(contactnumber,0) as contactnumber, coalesce(alternatenumber,0) as alternatenumber, coalesce(emailid,'') as emailid,coalesce(alternateemailid,'') as alternateemailid FROM tblmstmemberreferences where  upper(membercode) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        FIMemberReferencesDTO _FIMemberReferencesDTO = new FIMemberReferencesDTO
                        {
                            pRefRecordId = Convert.ToInt64(dr["recordid"]),
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pMemberReferenceId = Convert.ToString(dr["membercode"]),                            
                            pRefFirstname = Convert.ToString( dr["firstname"]),
                            pRefLastname = Convert.ToString( dr["lastname"]),
                            pRefcontactNo= Convert.ToDecimal( dr["contactnumber"]),
                            pRefalternatecontactNo= Convert.ToDecimal(dr["alternatenumber"]),
                            pRefEmailID= Convert.ToString(dr["emailid"]),
                            pRefAlternateEmailId= Convert.ToString(dr["alternateemailid"]),                            
                            ptypeofoperation = "OLD"                                                 
                        };
                        _FIMemberReferencesList.Add(_FIMemberReferencesDTO);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FIMemberReferencesList;
        }
        public FiMemberReferralDTO GetFIMemberReferralInformation(string strapplictionid, string ConnectionString)
        {
            FiMemberReferralDTO _FIMemberReferralDTO = null;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, memberid,membercode, isreferralcomexist, commissionpayouttype, coalesce(commissionpayout,0) as commissionpayout, istdsapplicable, coalesce(tdsaccountid,'') as tdsaccountid,coalesce(tdssection,'') as tdssection,coalesce(tdspercentage,0) as tdspercentage, coalesce(isreferralapplicable,false) as isreferralapplicable from tblmstmemberreferraldetails where  upper(membercode) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        _FIMemberReferralDTO = new FiMemberReferralDTO
                        {
                            pRefRecordId = Convert.ToInt64(dr["recordid"]),
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pMemberReferenceId = Convert.ToString(dr["membercode"]),
                            pIsReferralApplicableorNot = Convert.ToBoolean(dr["isreferralapplicable"]),
                            pCommisionPayoutType = Convert.ToString(dr["commissionpayouttype"]),
                            pCommisionPayoutAmountorPercentile = Convert.ToDecimal(dr["commissionpayout"]),
                            pIsTdsapplicable = Convert.ToBoolean(dr["istdsapplicable"]),
                            pTDSAccountId = Convert.ToString(dr["tdsaccountid"]),
                            pTdsSection = Convert.ToString(dr["tdssection"]),
                            ptypeofoperation = "OLD",
                            pTdsPercentage = Convert.ToDecimal(dr["tdspercentage"]),
                            IsReferralCommisionpaidforthisLoan = Convert.ToBoolean(dr["isreferralcomexist"])
                        };                       
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FIMemberReferralDTO;
        }
        public FiMemberContactDetails GetFIMemberContactInformation(string strapplictionid, string ConnectionString)
        {
            FiMemberContactDetails _FIMemberContactDTO = null;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT memberid,membercode, contactid, contacttype, contactreferenceid, membername, coalesce(membertype,'') as membertype,coalesce(membertypeid,0) as membertypeid,coalesce(memberstatus,'') as memberstatus,applicanttype,transdate FROM tblmstmembers where upper(membercode) = '" + ManageQuote(strapplictionid).ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        _FIMemberContactDTO = new FiMemberContactDetails
                        {
                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pContacttype = Convert.ToString(dr["contacttype"]),
                            pContactName = Convert.ToString(dr["membername"]),
                            pMemberType = Convert.ToString(dr["membertype"]),
                            pMembertypeId = Convert.ToInt64(dr["membertypeid"]),
                            pMemberStatus = Convert.ToString(dr["memberstatus"]),
                            pContactReferenceId = Convert.ToString(dr["contactreferenceid"]),
                            pContactId = Convert.ToInt64(dr["contactid"]),
                            ptypeofoperation = "OLD",
                            pMemberReferenceId = strapplictionid,
                            pFIMemberDate = dr["transdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["transdate"]).ToString("dd/MM/yyyy"),
                            pApplicantType = Convert.ToString(dr["applicanttype"])                            
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FIMemberContactDTO;
        }
        public async Task<FIMemberDTO> GetFIMemberData(string strapplictionid, string ConnectionString)
        {
            var _FIMemberData = new FIMemberDTO();
            _FIMemberData._FIMemberKYCDocumentsDTO = new FIMemberKYCDocumentsDTO();
            await Task.Run(() =>
            {               
                try
                {
                    _FIMemberData.pMemberReferenceid = strapplictionid;
                    _FIMemberData._FiMemberContactDetailsDTO = GetFIMemberContactInformation(strapplictionid, ConnectionString);
                   // _FIMemberData._FIMemberPersonalInformationDTO = GetFIMemberPersonalInformation(strapplictionid, ConnectionString);
                   // _FIMemberData.lobjAppReferences = GetFIMemberReferenceInformation(strapplictionid, ConnectionString);
                    _FIMemberData._FiMemberReferralDTO = GetFIMemberReferralInformation(strapplictionid, ConnectionString);
                    if (_FIMemberData._FiMemberContactDetailsDTO != null)
                    {
                        _FIMemberData._FIMemberKYCDocumentsDTO.documentstorelist = getDocumentstoreDetails(ConnectionString, _FIMemberData._FiMemberContactDetailsDTO.pContactId, "");
                        _FIMemberData.pMemberId = _FIMemberData._FiMemberContactDetailsDTO.pMemberId;
                    }
                    else
                    {
                        _FIMemberData._FIMemberKYCDocumentsDTO.documentstorelist =null;
                    }                    
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FIMemberData;
        }
        public async Task<List<FiMemberContactDetails>> GetallFIMembersDetails( string ConnectionString)
        {
            var _FiMemberContactDetails = new List<FiMemberContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    //select memberid, membercode, te.contactid, contacttype, te.contactreferenceid, membername, coalesce(membertype, '') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, contactnumber, emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontactpersondetails tc on tc.contactid = te.contactid where upper(priority) = 'PRIMARY' and te.statusid = " + Convert.ToInt32(Status.Active) + "  order by memberid desc;

                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode, te.contactid, te.contacttype, te.contactreferenceid, membername, coalesce(membertype,'') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, tm.businessentitycontactno as contactnumber, businessentityemailid as emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tm on tm.contactid=te.contactid join  tblmstcontactpersondetails tc on tc.contactid = te.contactid where  upper(priority) = 'PRIMARY' and te.statusid=" + Convert.ToInt32(Status.Active) + " order by memberid desc"))
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode, te.contactid, te.contacttype, te.contactreferenceid, membername, coalesce(membertype,'') as membertype,coalesce(membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, tm.businessentitycontactno as contactnumber, businessentityemailid as emailid, te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tm on tm.contactid=te.contactid  and te.statusid=" + Convert.ToInt32(Status.Active) + " order by memberid desc"))
                    {
                        while (dr.Read())
                        {
                            var _FIMemberContactDTO = new FiMemberContactDetails
                            {
                                pMembertypeId = Convert.ToInt64(dr["memberid"]),
                                pContacttype = Convert.ToString(dr["contacttype"]),
                                pContactName = Convert.ToString(dr["membername"]),
                                pMemberType = Convert.ToString(dr["membertype"]),
                                pMemberStatus = Convert.ToString(dr["memberstatus"]),
                                pContactReferenceId = Convert.ToString(dr["contactreferenceid"]),
                                pContactId = Convert.ToInt64(dr["contactid"]),
                                ptypeofoperation = "OLD",
                                pMemberReferenceId = Convert.ToString(dr["membercode"]),
                                pContactNo = Convert.ToString(dr["contactnumber"]),
                                pEmailId = Convert.ToString(dr["emailid"]),
                                pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false
                            };
                            _FiMemberContactDetails.Add(_FIMemberContactDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FiMemberContactDetails;
        }
        public int checkMemberCountinMaster(string ContactReferenceID, string ConnectionString)
        {
            try
            {
                return Convert.ToString(ContactReferenceID) != string.Empty ? Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembers where upper(contactreferenceid)='" + ManageQuote(ContactReferenceID).ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";")) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteFIMember(string MemberReferenceID, long Userid, string ConnectionString)
        {
            StringBuilder SbDeleteFIMember = new StringBuilder();
            bool IsDeleted = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();                
                int Statusid = Convert.ToInt32(Status.Inactive);              
                long ContactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select contactid from tblmstmembers where upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "' ;"));

                // KYC Delete (Document Store)
                SbDeleteFIMember.AppendLine("UPDATE tblmstdocumentstore set statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp where upper(contacttype)= 'MEMBER' and contactid=" + ContactId + " AND statusid<>2 and coalesce(loanid,0)=0;");               

                // Personal Details

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalapplicablesections SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalemplymentdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalbirthdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalfamilydetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalnomineedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalbankdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalincomedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonalotherincomedetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).Trim().ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationpersonaleducationdetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                SbDeleteFIMember.AppendLine("UPDATE tabapplicationkyccreditdetailsapplicablesections SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(vchapplicationid)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                // Reference Details
                SbDeleteFIMember.AppendLine("UPDATE tblmstmemberreferences SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                // Referral Details
                SbDeleteFIMember.AppendLine("UPDATE tblmstmemberreferraldetails SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                // Member Details Master Data

                SbDeleteFIMember.AppendLine("UPDATE tblmstmembers SET statusid=" + Statusid + ",modifiedby=" + Userid + ",modifieddate=current_timestamp  where  upper(membercode)='" + ManageQuote(MemberReferenceID).ToUpper() + "'; ");

                if (Convert.ToString(SbDeleteFIMember) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbDeleteFIMember.ToString());
                    trans.Commit();
                    IsDeleted = true;
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
            return IsDeleted;           
        }
        public async Task<List<FIMembertypeDTO>> _GetFIMembersTypesListDetails(string ConnectionString)
        {
            _FIMembersList = new List<FIMembertypeDTO>();
            FIMembertypeDTO _FIMembertypeDTO = null;
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype,membertypecode from tblmstmembertypes where  statusid = " + Convert.ToInt32(Status.Active) + " order by membertype;"))
                    {
                        while (dr.Read())
                        {
                            _FIMembertypeDTO = new FIMembertypeDTO
                            {
                                pMembertypeId = Convert.ToInt64(dr["membertypeid"]),
                                pMemberType = Convert.ToString(dr["membertype"]),
                                pMemberTypeCode = Convert.ToString(dr["membertypecode"])
                            };
                            _FIMembersList.Add(_FIMembertypeDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FIMembersList;
        }
        public async Task<List<FIApplicantTypeDTO>> GetFIMembersApplicantListDetails(string ContactType, string ConnectionString)
        {
            _FIApplicantTypesList = new List<FIApplicantTypeDTO>();
            FIApplicantTypeDTO _FIApplicantTypeDTO = null;
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct applicanttype from tblmstapplicantcongiguration where upper(contacttype)='" + ManageQuote(ContactType).ToUpper() + "' and  statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            _FIApplicantTypeDTO = new FIApplicantTypeDTO
                            {
                                pApplicantType = Convert.ToString(dr["applicanttype"])
                            };
                            _FIApplicantTypesList.Add(_FIApplicantTypeDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FIApplicantTypesList;
        }
        public string SaveFIMemberMasterData(FiMemberContactDetails _FiMemberContactDetails,string ConnectionString, out long pMemberId,out string Dob)
        {
            StringBuilder sbSaveFIMember = new StringBuilder();           
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();               
                // Next Id-Generation of Member Reference Id

                if (string.IsNullOrEmpty(_FiMemberContactDetails.pFIMemberDate))
                {
                    _FiMemberContactDetails.pFIMemberDate = "null";
                }
                else
                {
                    _FiMemberContactDetails.pFIMemberDate = "'" + FormatDate(_FiMemberContactDetails.pFIMemberDate) + "'";
                }
                if (!string.IsNullOrEmpty(_FiMemberContactDetails.ptypeofoperation) && _FiMemberContactDetails.ptypeofoperation.Trim().ToUpper() == "CREATE")
                {
                    if (string.IsNullOrEmpty(_FiMemberContactDetails.pMemberReferenceId))
                    {
                        _FiMemberContactDetails.pMemberReferenceId = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('MEMBER','" + ManageQuote(_FiMemberContactDetails.pMemberType) + "'," + _FiMemberContactDetails.pFIMemberDate + ")").ToString();
                    }
                    // Contact-Member Save
                    _FiMemberContactDetails.pMemberId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tblmstmembers   ( membercode,contactid, contacttype, contactreferenceid, membername,membertype, membertypeid, memberstatus, statusid,     createdby,createddate,ispersonaldetailsapplicable,applicanttype,transdate,isreferencesapplicable)  VALUES ('" + ManageQuote(_FiMemberContactDetails.pMemberReferenceId) + "', " + _FiMemberContactDetails.pContactId + ", '" + _FiMemberContactDetails.pContacttype + "', '" + _FiMemberContactDetails.pContactReferenceId + "', '" + _FiMemberContactDetails.pContactName + "','" + _FiMemberContactDetails.pMemberType + "', " + _FiMemberContactDetails.pMembertypeId + ",    '" + _FiMemberContactDetails.pMemberStatus + "', " + Convert.ToInt32(Status.Active) + ", " + _FiMemberContactDetails.pCreatedby + ",current_timestamp,'false','" + ManageQuote(_FiMemberContactDetails.pApplicantType) + "'," + _FiMemberContactDetails.pFIMemberDate + ",'false') returning memberid; "));
                }
                else if (!string.IsNullOrEmpty(_FiMemberContactDetails.ptypeofoperation) && _FiMemberContactDetails.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                {
                    sbSaveFIMember.AppendLine("Update tblmstmembers set applicanttype='" + ManageQuote(_FiMemberContactDetails.pApplicantType) + "', transdate=" + _FiMemberContactDetails.pFIMemberDate + ", membername='" + _FiMemberContactDetails.pContactName + "', membertype='" + _FiMemberContactDetails.pMemberType + "'," +
                        "membertypeid=" + _FiMemberContactDetails.pMembertypeId + ",memberstatus='" + _FiMemberContactDetails.pMemberStatus + "' , modifieddate=current_timestamp,modifiedby=" + _FiMemberContactDetails.pCreatedby + " where membercode='" + _FiMemberContactDetails.pMemberReferenceId + "';");                   
                }
                pMemberId = _FiMemberContactDetails.pMemberId;

                long ApplicableSectionsCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select count(*) from tabapplicationpersonalapplicablesections where vchapplicationid='" + ManageQuote(_FiMemberContactDetails.pMemberReferenceId) + "' and contactid = " + _FiMemberContactDetails.pContactId + ""));

                if (ApplicableSectionsCount <= 0)
                {
                    sbSaveFIMember.AppendLine("insert into tabapplicationpersonalapplicablesections( applicationid, vchapplicationid, contactid, contactreferenceid,isemplymentdetailsapplicable, contacttype,statusid,createdby,createddate ) values (" + pMemberId + ",'" + ManageQuote(_FiMemberContactDetails.pMemberReferenceId) + "'," + _FiMemberContactDetails.pContactId + ",'" + ManageQuote(_FiMemberContactDetails.pContactReferenceId) + "','false','MEMBER'," + Convert.ToInt32(Status.Active) + "," + _FiMemberContactDetails.pCreatedby + ",current_timestamp);");                   
                }              

                long ApplicableKycCount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, " select count(*)  from tabapplicationkyccreditdetailsapplicablesections where contactid = " + _FiMemberContactDetails.pContactId + " and vchapplicationid = '" + ManageQuote(_FiMemberContactDetails.pMemberReferenceId) + "';"));

                if (ApplicableKycCount <= 0)
                {
                    sbSaveFIMember.AppendLine("insert into tabapplicationkyccreditdetailsapplicablesections(applicationid, vchapplicationid, contactid, contactreferenceid, contacttype, statusid, createdby, createddate) values (" + pMemberId + ",'" + ManageQuote(_FiMemberContactDetails.pMemberReferenceId) + "'," + _FiMemberContactDetails.pContactId + ",'" + ManageQuote(_FiMemberContactDetails.pContactReferenceId) + "','MEMBER'," + Convert.ToInt32(Status.Active) + "," + _FiMemberContactDetails.pCreatedby + ",current_timestamp);");                   
                }
                Dob = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select dob from tblmstcontact where contactid="+ _FiMemberContactDetails.pContactId + "").ToString();
                if (sbSaveFIMember.ToString()!=string.Empty)
                {
                    NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, sbSaveFIMember.ToString());
                }                
                trans.Commit();                
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
            return _FiMemberContactDetails.pMemberReferenceId;
        }
        public bool SaveFIMemberReferenceData(FIMembersaveReferences _FIMembersaveReferences, string ConnectionString)
        {
            string Recordid = string.Empty;
            StringBuilder SbsaveReferences = new StringBuilder();
            StringBuilder sbDelete = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_FIMembersaveReferences.lobjAppReferences != null && _FIMembersaveReferences.lobjAppReferences.Count > 0)
                {
                    for (int i = 0; i < _FIMembersaveReferences.lobjAppReferences.Count; i++)
                    {
                        _FIMembersaveReferences.lobjAppReferences[i].pRefalternatecontactNo = Convert.ToString(_FIMembersaveReferences.lobjAppReferences[i].pRefalternatecontactNo) == string.Empty ? 0 : _FIMembersaveReferences.lobjAppReferences[i].pRefalternatecontactNo;

                        if (!string.IsNullOrEmpty(_FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation))
                        {
                            if (_FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation.Trim().ToUpper() != "CREATE" && _FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation.Trim().ToUpper() != "OLD")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = _FIMembersaveReferences.lobjAppReferences[i].pRefRecordId.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + _FIMembersaveReferences.lobjAppReferences[i].pRefRecordId.ToString();
                                }
                            }
                            if (_FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation.Trim().ToUpper() == "UPDATE" || _FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation.Trim().ToUpper() == "OLD")
                            {
                                SbsaveReferences.AppendLine("UPDATE tblmstmemberreferences SET firstname ='" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefFirstname) + "', lastname ='" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefLastname) + "', contactnumber =" + _FIMembersaveReferences.lobjAppReferences[i].pRefcontactNo + ", " +
                                    "alternatenumber =" + _FIMembersaveReferences.lobjAppReferences[i].pRefalternatecontactNo + ", emailid ='" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefEmailID) + "',alternateemailid='" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefAlternateEmailId)+ "', modifiedby =" + _FIMembersaveReferences.pCreatedby + ", modifieddate =current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " WHERE membercode='" + ManageQuote(_FIMembersaveReferences.pMemberReferenceId) + "' and memberid=" + _FIMembersaveReferences.pMemberId + " and recordid=" + _FIMembersaveReferences.lobjAppReferences[i].pRefRecordId + "; ");
                            }
                            else if (_FIMembersaveReferences.lobjAppReferences[i].ptypeofoperation.Trim().ToUpper() == "CREATE")
                            {                              
                                SbsaveReferences.AppendLine("INSERT INTO tblmstmemberreferences(memberid,membercode, firstname, lastname, contactnumber, alternatenumber, emailid, alternateemailid, statusid, createdby, createddate)   VALUES (" + _FIMembersaveReferences.pMemberId + ", '" + ManageQuote(_FIMembersaveReferences.pMemberReferenceId) + "','" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefFirstname) + "', '" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefLastname) + "', " + _FIMembersaveReferences.lobjAppReferences[i].pRefcontactNo + ", " +  _FIMembersaveReferences.lobjAppReferences[i].pRefalternatecontactNo + ", '" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefEmailID) + "', '" + ManageQuote(_FIMembersaveReferences.lobjAppReferences[i].pRefAlternateEmailId) + "', " + Convert.ToInt32(Status.Active) + ", " + _FIMembersaveReferences.pCreatedby + ", current_timestamp);");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + _FIMembersaveReferences.pCreatedby + ",modifieddate=current_timestamp where recordid not in(" + Recordid + ");");
                    }
                    else
                    {
                        sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + _FIMembersaveReferences.pCreatedby + ",modifieddate=current_timestamp where memberid=" + _FIMembersaveReferences.pMemberId + ";");
                    }
                    SbsaveReferences.AppendLine("update tblmstmembers set isreferencesapplicable='" + _FIMembersaveReferences.pIsreferencesapplicable + "' where membercode='" + _FIMembersaveReferences.pMemberReferenceId + "';");
                }
                else
                {
                    sbDelete.AppendLine("update tblmstmemberreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + _FIMembersaveReferences.pCreatedby + ",modifieddate=current_timestamp where memberid=" + _FIMembersaveReferences.pMemberId + ";");
                }

                if(!string.IsNullOrEmpty( SbsaveReferences.ToString()) || !string.IsNullOrEmpty(sbDelete.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbDelete) + " " + Convert.ToString(SbsaveReferences));
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
        public bool SaveFIMemberReferralData(FiMemberReferralDTO _FiMemberReferralDTO, string ConnectionString)
        {
            StringBuilder SbsaveReferrals = new StringBuilder();
            bool IsSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_FiMemberReferralDTO.pIsReferralApplicableorNot)
                {
                    if (!string.IsNullOrEmpty(_FiMemberReferralDTO.ptypeofoperation))
                    {
                        _FiMemberReferralDTO.pTdsPercentage = Convert.ToString(_FiMemberReferralDTO.pTdsPercentage) == string.Empty ? 0 : _FiMemberReferralDTO.pTdsPercentage;

                        _FiMemberReferralDTO.pTDSAccountId = Convert.ToString(_FiMemberReferralDTO.pTDSAccountId) == string.Empty ? string.Empty : _FiMemberReferralDTO.pTDSAccountId;

                        _FiMemberReferralDTO. pCommisionPayoutAmountorPercentile= Convert.ToString(_FiMemberReferralDTO.pCommisionPayoutAmountorPercentile) == string.Empty ? 0 : _FiMemberReferralDTO.pCommisionPayoutAmountorPercentile;

                        if (_FiMemberReferralDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                        {
                            SbsaveReferrals.AppendLine("INSERT INTO tblmstmemberreferraldetails(memberid,membercode, isreferralapplicable, commissionpayouttype,commissionpayout, istdsapplicable, tdsaccountid, tdssection,tdspercentage, statusid, createdby, createddate,isreferralcomexist ) VALUES (" + _FiMemberReferralDTO.pMemberId + ", '" + ManageQuote(_FiMemberReferralDTO.pMemberReferenceId) + "','" + (_FiMemberReferralDTO.pIsReferralApplicableorNot) + "', '" + ManageQuote(_FiMemberReferralDTO.pCommisionPayoutType) + "', " + _FiMemberReferralDTO.pCommisionPayoutAmountorPercentile + ", '" + _FiMemberReferralDTO.pIsTdsapplicable + "', '" + ManageQuote(_FiMemberReferralDTO.pTDSAccountId) + "','" + ManageQuote(_FiMemberReferralDTO.pTdsSection) + "', " + _FiMemberReferralDTO.pTdsPercentage + ", " + Convert.ToInt32(Status.Active) + ", " + _FiMemberReferralDTO.pCreatedby + ", current_timestamp,'" +_FiMemberReferralDTO.IsReferralCommisionpaidforthisLoan + "');");
                        }
                        else if (_FiMemberReferralDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                        {
                            SbsaveReferrals.AppendLine("update tblmstmemberreferraldetails set isreferralcomexist='" + (_FiMemberReferralDTO.IsReferralCommisionpaidforthisLoan) + "',commissionpayouttype='" + ManageQuote(_FiMemberReferralDTO.pCommisionPayoutType) + "',commissionpayout=" + _FiMemberReferralDTO.pCommisionPayoutAmountorPercentile + ",istdsapplicable='" + _FiMemberReferralDTO.pIsTdsapplicable + "',tdsaccountid='" + ManageQuote(_FiMemberReferralDTO.pTDSAccountId) + "',tdssection='" + ManageQuote(_FiMemberReferralDTO.pTdsSection) + "',tdspercentage=" + _FiMemberReferralDTO.pTdsPercentage + ",statusid=" + Convert.ToInt32(Status.Active) + ",modifiedby=" + _FiMemberReferralDTO.pCreatedby + ",modifieddate=current_timestamp,isreferralapplicable='" + _FiMemberReferralDTO.pIsReferralApplicableorNot + "' where  membercode='" + ManageQuote(_FiMemberReferralDTO.pMemberReferenceId) + "' and MemberId=" + _FiMemberReferralDTO.pMemberId + "; ");
                        }
                    }
                }
                else
                {
                    SbsaveReferrals.AppendLine("Delete from tblmstmemberreferraldetails where membercode='" + ManageQuote(_FiMemberReferralDTO.pMemberReferenceId) + "' and memberid=" + _FiMemberReferralDTO.pMemberId + ";");
                }

                if(Convert.ToString(SbsaveReferrals)!=string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, SbsaveReferrals.ToString());
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
        public bool GetIsReferencesapplicableOrnot(string strapplictionid, string ConnectionString)
        {
            bool IsExists = false;
            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT coalesce(isreferencesapplicable,false) as isreferencesapplicable FROM tblmstmembers where  upper(membercode) = '" + ManageQuote(strapplictionid).ToUpper() + "';"))
            {
                if (dr.HasRows && dr.Read())
                {
                    IsExists = Convert.ToBoolean(dr["isreferencesapplicable"]);
                }                
            }            
            return IsExists;
        }
        public  async Task<List<FIMeberRefIdAndID>> GetFIMembersforasGuardians(string ConnectionString)
        {
            var _FIMeberRefIdAndIDList = new List<FIMeberRefIdAndID>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select memberid,membercode from tblmstmembers where statusid=" + Convert.ToInt32(Status.Active) + " and upper(applicanttype)!='MINOR' order by membername; "))
                    {
                        while (dr.Read())
                        {
                            var _FIMeberRefIdAndID = new FIMeberRefIdAndID
                            {
                                pMemberId = Convert.ToInt64(dr["memberid"]),
                                pMemberReferenceId = Convert.ToString(dr["membercode"])                              
                            };
                            _FIMeberRefIdAndIDList.Add(_FIMeberRefIdAndID);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _FIMeberRefIdAndIDList;
        }

        public List<ContactDetailsDTO> getContactDetails(string contactType, string ConnectionString)
        {
            string strQuery = string.Empty;
            List<ContactDetailsDTO>  lstContactDetails = new List<ContactDetailsDTO>();
            try
            {
                
                if (!string.IsNullOrEmpty(contactType))
                {
                   // strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + " and upper(contacttype)=upper('" + contactType + "') and CONTACTID not in(select contactid from tblmstmembers) ) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                    strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + " and upper(contacttype)=upper('" + contactType + "')  ) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                }
                else
                {
                    //strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + " and CONTACTID not in(select contactid from tblmstmembers)  ) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                    strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + "   ) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        ContactDetailsDTO objContactDetails = new ContactDetailsDTO();
                        objContactDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objContactDetails.pContactName = Convert.ToString(dr["name"]);
                        objContactDetails.pReferenceId = Convert.ToString(dr["contactreferenceid"]);
                        objContactDetails.pTitleName = Convert.ToString(dr["titlename"]);
                        objContactDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objContactDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objContactDetails.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objContactDetails.pFatherName = Convert.ToString(dr["fathername"]);
                        objContactDetails.pRoleid = dr["roleid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["roleid"]);
                        objContactDetails.pRolename = Convert.ToString(dr["rolename"]);
                        lstContactDetails.Add(objContactDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstContactDetails;
        }

    }
}
