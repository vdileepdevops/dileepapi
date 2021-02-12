using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings;
using Npgsql;
using HelperManager;
using System.Data;

namespace FinstaRepository.DataAccess.Settings
{
    public class ReferralAdvocateDAL : SettingsDAL, IReferralAdvocate
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        List<ReferralAdvocateDTO> lstReferalContactdetails = null;
        public List<documentstoreDTO> documentstoredetails { get; set; }
        public List<referralbankdetailsDTO> referralbankdetails { get; set; }

        #region GetReferral/Agent Name
        public List<ReferralAdvocateDTO> getContactDetails(string contactType, string ConnectionString)
        {
            string strQuery = string.Empty;
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            try
            {
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,t1.businessentitycontactno,t1.businessentityemailid,contactimagepath,fathername from tblmstcontact t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname) = 'ACTIVE' and upper(contacttype)=upper('" + contactType + "') order by name;"))
                if (!string.IsNullOrEmpty(contactType))
                {
                    strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + " and upper(contacttype)=upper('" + contactType + "')) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                }
                else
                {
                    strQuery = "SELECT distinct A.*,ROLEID,ROLENAME FROM(select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,contactreferenceid,businessentitycontactno,businessentityemailid,contactimagepath,fathername from tblmstcontact where STATUSID=" + Convert.ToInt32(Status.Active) + " ) A LEFT JOIN (SELECT coalesce(ROLEID,0) ROLEID,DESIGNATION ROLENAME,TE.CONTACTID FROM tblmstemployeeemploymentdetails TR JOIN tblmstemployee TE ON TE.EMPLOYEEID=TR.EMPLOYEEID JOIN tblmstcontact TC ON TC.CONTACTID=TE.EMPLOYEEID where te.statusid=" + Convert.ToInt32(Status.Active) + ") B ON A.CONTACTID=B.CONTACTID order by name;";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalContactDetails = new ReferralAdvocateDTO();
                        objReferalContactDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferalContactDetails.pContactName = Convert.ToString(dr["name"]);
                        objReferalContactDetails.pReferenceId = Convert.ToString(dr["contactreferenceid"]);
                        objReferalContactDetails.pTitleName = Convert.ToString(dr["titlename"]);
                        objReferalContactDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferalContactDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferalContactDetails.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objReferalContactDetails.pFatherName = Convert.ToString(dr["fathername"]);
                        objReferalContactDetails.pRoleid = dr["roleid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["roleid"]);
                        objReferalContactDetails.pRolename = Convert.ToString(dr["rolename"]);
                        lstReferalContactdetails.Add(objReferalContactDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }
        #endregion
        #region GetReferral/Agent Details for Main Grid
        public List<ReferralAdvocateDTO> getReferralAgentDetails(string Type, string ConnectionString)
        {
            string Query = string.Empty;
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL")
                {
                    //Query = "select t1.referralid,t1.contactid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,'')) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype,t2.contactreferenceid from tblmstreferral t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid where  upper(ts.statusname) = 'ACTIVE' order by t1.referralid";
                    Query = "select t1.tbl_mst_referral_id as referralid,t1.contact_id as contactid,(coalesce(t2.name,'')||' '||coalesce(t2.surname,'')) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype,t2.contactreferenceid from tbl_mst_referral t1 join tblmstcontact t2 on t1.contact_id=t2.contactid where status='t' order by t1.tbl_mst_referral_id";
                }
                else
                {
                    //Query = "select t1.referralid,t1.contactid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype,t2.contactreferenceid from tblmstreferral t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid  where upper(t1.name) like'%" + Type + "%' or upper(t1.surname) like '%" + Type + "%' or upper(t2.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%' or upper(contacttype) like '%" + Type.ToUpper() + "%' and upper(ts.statusname) = 'ACTIVE'; ";
                    Query = "select t1.tbl_mst_referral_id as referralid,t1.contact_id as contactid,(coalesce(t2.name, '') || ' ' || coalesce(t2.surname, '')) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype,t2.contactreferenceid from tbl_mst_referral t1 join tblmstcontact t2 on t1.contact_id=t2.contactid  where upper(t2.name) like'%" + Type + "%' or upper(t2.surname) like '%" + Type + "%' or upper(t2.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%' or upper(contacttype) like '%" + Type.ToUpper() + "%' ; ";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferalAgentDetails.pReferralId = Convert.ToInt64(dr["referralid"]);
                        objReferalAgentDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferalAgentDetails.pReferenceId = Convert.ToString(dr["contactreferenceid"]);
                        objReferalAgentDetails.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferalAgentDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferalAgentDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        lstReferalContactdetails.Add(objReferalAgentDetails);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }
        public List<ReferralAdvocateDTO> getReferralDetails(string ConnectionString)
        {
            string Query = string.Empty;
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
           
            try
            {
               
                    Query = "select t1.tbl_mst_referral_id as referralid,t1.contact_id as contactid,referral_code,(coalesce(t2.name,'')||' '||coalesce(t2.surname,'')) as advocatename,(coalesce(t2.name,'')||' '||coalesce(t2.surname,'')||' - ' || referral_code) as referralname ,t2.businessentitycontactno,t2.businessentityemailid,contacttype,t2.contactreferenceid from tbl_mst_referral t1 join tblmstcontact t2 on t1.contact_id=t2.contactid where status='t' order by t1.tbl_mst_referral_id";
               
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferalAgentDetails.pReferralId = Convert.ToInt64(dr["referralid"]);
                        objReferalAgentDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferalAgentDetails.pReferenceId = Convert.ToString(dr["contactreferenceid"]);
                        objReferalAgentDetails.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferalAgentDetails.pReferralName = Convert.ToString(dr["referralname"]);
                        objReferalAgentDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferalAgentDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferalAgentDetails.pReferralCode = Convert.ToString(dr["referral_code"]);
                        lstReferalContactdetails.Add(objReferalAgentDetails);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }
        #endregion
        public ReferralAdvocateDTO GetContactDetailsbyId(Int64 ContactId, string ConnectionString)
        {
            ReferralAdvocateDTO objReferralAdvocateDTO = null;
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name,titlename,t2.businessentitycontactno,t2.businessentityemailid,contactimagepath,contactreferenceid,fathername from tblmstcontact t2  join tblmststatus ts on t2.statusid = ts.statusid where contactid=" + ContactId + " "))
                {
                    while (dr.Read())
                    {
                        objReferralAdvocateDTO = new ReferralAdvocateDTO();
                        objReferralAdvocateDTO.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferralAdvocateDTO.pContactName = Convert.ToString(dr["name"]);
                        objReferralAdvocateDTO.pReferenceId = Convert.ToString(dr["contactreferenceid"]);
                        objReferralAdvocateDTO.pTitleName = Convert.ToString(dr["titlename"]);
                        objReferralAdvocateDTO.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objReferralAdvocateDTO.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferralAdvocateDTO.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferralAdvocateDTO.pFatherName = Convert.ToString(dr["fathername"]);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return objReferralAdvocateDTO;
        }
        #region Get Referral/Agent Data For Edit
        public ReferralAdvocateDTO ViewReferralAgentDetails(Int64 refid, string ConnectionString)
        {
            ReferralAdvocateDTO objReferralAdvocateDTO = new ReferralAdvocateDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.referralid,t1.contactid,t1.name,t1.surname,(coalesce(t1.name,'')||' '||coalesce(t1.surname,'')) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contactimagepath,statusname,t1.statusid,t1.createdby,t1.createddate from tblmstreferral t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t2.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' and t1.referralid=" + refid + " "))
                {
                    while (dr.Read())
                    {
                        //ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferralAdvocateDTO.pReferralId = Convert.ToInt64(dr["referralid"]);
                        objReferralAdvocateDTO.pName = Convert.ToString(dr["name"]);
                        objReferralAdvocateDTO.pSurName = Convert.ToString(dr["surname"]);
                        objReferralAdvocateDTO.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferralAdvocateDTO.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferralAdvocateDTO.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objReferralAdvocateDTO.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferralAdvocateDTO.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferralAdvocateDTO.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
                objReferralAdvocateDTO.documentstorelist = getDocumentstoreDetails(ConnectionString, objReferralAdvocateDTO.pContactId, "");
                objReferralAdvocateDTO.referralbankdetailslist = getReferralbankDetails(ConnectionString, refid);
                objReferralAdvocateDTO.referraltaxdetailslist = getReferralTaxDetails(ConnectionString, refid);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return objReferralAdvocateDTO;
        }
        private referraltaxdetailsDTO getReferralTaxDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referraltaxdetailsDTO objreferraltaxdetail = null;

            try
            {

                strquery = "SELECT  reftaxid,referralid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, t.statusid,statusname ,t.statusid,t.createdby,t.createddate FROM tblmstreferraltaxdetails t join tblmststatus ts on t.statusid = ts.statusid where referralid=" + pReferralId + " and upper(ts.statusname) = 'ACTIVE';";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        objreferraltaxdetail = new referraltaxdetailsDTO();
                        objreferraltaxdetail.pRefTaxId = Convert.ToInt64(dr["reftaxid"]);
                        objreferraltaxdetail.pReferralId = Convert.ToInt64(dr["referralid"]);
                        objreferraltaxdetail.pIstdsApplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                        objreferraltaxdetail.ptdsSectionName = Convert.ToString(dr["tdssectionname"]);
                        objreferraltaxdetail.pIsgstApplicable = Convert.ToBoolean(dr["isgstapplicable"]);
                        objreferraltaxdetail.pStateName = Convert.ToString(dr["statename"]);
                        objreferraltaxdetail.pGstType = Convert.ToString(dr["gsttype"]);
                        objreferraltaxdetail.pGstNo = Convert.ToString(dr["gstno"]);
                        objreferraltaxdetail.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objreferraltaxdetail;
        }
        private List<referralbankdetailsDTO> getReferralbankDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referralbankdetails = new List<referralbankdetailsDTO>();
            try
            {
                strquery = "SELECT  refbankid,referralid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, t.statusid,statusname,t.createdby,t.createddate,'OLD' as typeofoperation FROM tblmstreferralbankdetails t join tblmststatus ts on t.statusid = ts.statusid and upper(ts.statusname) = 'ACTIVE'  where referralid=" + pReferralId + ";";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        referralbankdetailsDTO objreferralbankdetail = new referralbankdetailsDTO();
                        objreferralbankdetail.pRefbankId = Convert.ToInt64(dr["refbankid"]);
                        objreferralbankdetail.pReferralId = Convert.ToInt64(dr["referralid"]);
                        objreferralbankdetail.pBankAccountname = Convert.ToString(dr["bankaccountname"]);
                        objreferralbankdetail.pBankName = Convert.ToString(dr["bankname"]);
                        objreferralbankdetail.pBankAccountNo = Convert.ToString(dr["bankaccountno"]);
                        objreferralbankdetail.pBankifscCode = Convert.ToString(dr["bankifsccode"]);
                        objreferralbankdetail.pBankBranch = Convert.ToString(dr["bankbranch"]);
                        objreferralbankdetail.pIsprimaryAccount = Convert.ToBoolean(dr["isprimaryaccount"]);
                        objreferralbankdetail.pStatusname = Convert.ToString(dr["statusname"]);
                        objreferralbankdetail.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        referralbankdetails.Add(objreferralbankdetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return referralbankdetails;

        }
        #endregion

        #region Save ReferralAgent
        public bool saveReferral(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder ReferralDetailsinsert = new StringBuilder();
            long ReferralId;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId ));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];

                ReferralId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstreferral (contactid,titlename,name,surname,statusid,createdby,createddate) values (" + referralavocatelist.pContactId + ",'" + ManageQuote(referralavocatelist.pTitleName.Trim()) + "','" + ManageQuote(referralavocatelist.pName.Trim()) + "','" + ManageQuote(referralavocatelist.pSurName.Trim()) + "'," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp) returning referralid;"));
                string str = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);
                //if (referralavocatelist.documentstorelist != null)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceMonth) && !string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceYear))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceMonth + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceYear;
                //        }
                //        else if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceMonth))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceMonth;
                //        }
                //        else if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceYear))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceYear;
                //        }
                //        else
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno.Trim();
                //        }
                //        Int64 count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(docstoreid) from tblmstdocumentstore where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and documentid=" + referralavocatelist.documentstorelist[i].pDocumentId + " and documentgroupid=" + referralavocatelist.documentstorelist[i].pDocumentGroupId + " and coalesce(loanid,0)=0"));
                //        if (count == 0)
                //        {
                //            ReferralDetailsinsert.Append("insert into tblmstdocumentstore (contactid ,applicationno,documentid,documentgroupid,documentgroupname,documentname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate) values (" + referralavocatelist.documentstorelist[i].pContactId + "," + referralavocatelist.documentstorelist[i].pApplicationNo + "," + referralavocatelist.documentstorelist[i].pDocumentId + "," + referralavocatelist.documentstorelist[i].pDocumentGroupId + ",'" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentGroup) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentName) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocStorePath) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocFileType) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocReferenceno) + "'," + referralavocatelist.documentstorelist[i].pDocIsDownloadable + "," + getStatusid(referralavocatelist.documentstorelist[i].pStatusname, ConnectionString) + "," + referralavocatelist.documentstorelist[i].pCreatedby + ",current_timestamp);");
                //        }
                //    }
                //}
                if (referralavocatelist.referralbankdetailslist != null && referralavocatelist.referralbankdetailslist.Count > 0)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        ReferralDetailsinsert.Append("INSERT INTO tblmstreferralbankdetails(referralid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + ReferralId + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.referralbankdetailslist[i].pStatusname, ConnectionString) + "," + referralavocatelist.referralbankdetailslist[i].pCreatedby + ",current_timestamp); ");
                    }
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    ReferralDetailsinsert.Append("INSERT INTO tblmstreferraltaxdetails(referralid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + ReferralId + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                }
                if (ReferralDetailsinsert.Length > 0 || str.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, str + "" + ReferralDetailsinsert.ToString());
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
        public bool DeleteReferralAgent(DeleteDTO objDeleteDTO, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder ReferralDetailsDelete = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                ReferralDetailsDelete.Append("UPDATE tblmstreferral set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where referralid=" + objDeleteDTO.pReferralId + "; ");
                //if (referralavocatelist.documentstorelist.Count > 0)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        ReferralDetailsDelete.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + referralavocatelist.documentstorelist[i].pCreatedby + ",modifieddate=current_timestamp where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and docstoreid=" + referralavocatelist.documentstorelist[i].pDocstoreId + "; ");
                //    }

                //}
                ReferralDetailsDelete.Append("UPDATE tblmstreferralbankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where referralid=" + objDeleteDTO.pReferralId + "; ");
                ReferralDetailsDelete.Append("UPDATE tblmstreferraltaxdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where referralid=" + objDeleteDTO.pReferralId + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, ReferralDetailsDelete.ToString());
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
        public string UpdateStoreDetails(List<documentstoreDTO> documentstorelist, string ConnectionString, Int64 applicationid, long numcontactid)
        {
            StringBuilder query = new StringBuilder();
            StringBuilder strUpdate = new StringBuilder();
            string StoreRecordid = string.Empty;
            if (documentstorelist != null && documentstorelist.Count > 0)
            {
                for (int i = 0; i < documentstorelist.Count; i++)
                {
                    if (numcontactid == 0)
                    {
                        numcontactid = documentstorelist[i].pContactId;
                    }
                    if (documentstorelist[i].ptypeofoperation.ToUpper() != "CREATE")
                    {
                        if (!string.IsNullOrEmpty(documentstorelist[i].pDocumentReferenceMonth) && !string.IsNullOrEmpty(documentstorelist[i].pDocumentReferenceYear))
                        {
                            documentstorelist[i].pDocReferenceno = documentstorelist[i].pDocReferenceno + "~" + documentstorelist[i].pDocumentReferenceMonth + "~" + documentstorelist[i].pDocumentReferenceYear;
                        }
                        else if (!string.IsNullOrEmpty(documentstorelist[i].pDocumentReferenceMonth))
                        {
                            documentstorelist[i].pDocReferenceno = documentstorelist[i].pDocReferenceno + "~" + documentstorelist[i].pDocumentReferenceMonth;
                        }
                        else if (!string.IsNullOrEmpty(documentstorelist[i].pDocumentReferenceYear))
                        {
                            documentstorelist[i].pDocReferenceno = documentstorelist[i].pDocReferenceno + "~" + documentstorelist[i].pDocumentReferenceYear;
                        }
                        else
                        {
                            documentstorelist[i].pDocReferenceno = documentstorelist[i].pDocReferenceno.Trim();
                        }
                        if (documentstorelist[i].ptypeofoperation.ToUpper() != "DELETE")
                        {
                            if (string.IsNullOrEmpty(StoreRecordid))
                            {
                                StoreRecordid = documentstorelist[i].pDocstoreId.ToString();
                            }
                            else
                            {
                                StoreRecordid = StoreRecordid + "," + documentstorelist[i].pDocstoreId.ToString();
                            }
                        }
                    }
                    Int64 count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(docstoreid) from tblmstdocumentstore t join tblmststatus t1 on t.statusid=t1.statusid where contactid=" + documentstorelist[i].pContactId + " and documentid=" + documentstorelist[i].pDocumentId + " and documentgroupid=" + documentstorelist[i].pDocumentGroupId + " and coalesce(applicationno,0)=0 and t1.statusname='Active'"));
                    //if (count == 0)
                    //{

                    strUpdate.Append("update tabapplicationkyccreditdetailsapplicablesections set iskycdocumentsdetailsapplicable='" + (documentstorelist[i].pisapplicable) + "', modifiedby=" + (documentstorelist[i].pCreatedby) + ", modifieddate=current_timestamp where applicationid = " + applicationid + " and  contactid =" + numcontactid + ";");
                    string strext = string.Empty;
                    if (!string.IsNullOrEmpty(documentstorelist[i].pDocStorePath) && documentstorelist[i].pDocStorePath.Contains('.'))
                    {
                         strext = documentstorelist[i].pDocStorePath.Substring(documentstorelist[i].pDocStorePath.LastIndexOf('.') + 1);
                    }
                    if (!string.IsNullOrEmpty(strext))
                    {
                        documentstorelist[i].pDocFileType = strext;
                    }
                    if (documentstorelist[i].ptypeofoperation.ToUpper() == "CREATE")
                    {
                        if (documentstorelist[i].pisapplicable == true)
                            strUpdate.Append("insert into tblmstdocumentstore (contactid ,applicationno,contacttype,documentid,documentgroupid,documentgroupname,documentname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate,filename) values (" + numcontactid + ",0,'" + ManageQuote(documentstorelist[i].pContactType) + "' ," + documentstorelist[i].pDocumentId + "," + documentstorelist[i].pDocumentGroupId + ",'" + ManageQuote(documentstorelist[i].pDocumentGroup) + "','" + ManageQuote(documentstorelist[i].pDocumentName) + "','" + ManageQuote(documentstorelist[i].pDocStorePath) + "','" + ManageQuote(documentstorelist[i].pDocFileType) + "','" + ManageQuote(documentstorelist[i].pDocReferenceno) + "'," + documentstorelist[i].pDocIsDownloadable + "," + Convert.ToInt32(Status.Active) + "," + documentstorelist[i].pCreatedby + ",current_timestamp,'" + ManageQuote(documentstorelist[i].pFilename) + "');");
                    }
                    else if (documentstorelist[i].ptypeofoperation.ToUpper() == "UPDATE")
                    {
                        if (documentstorelist[i].pisapplicable == true)
                            strUpdate.Append("UPDATE tblmstdocumentstore SET statusid=" + Convert.ToInt32(Status.Active) + ",contacttype='" + ManageQuote(documentstorelist[i].pContactType) + "', documentid=" + documentstorelist[i].pDocumentId + ", documentgroupid=" + documentstorelist[i].pDocumentGroupId + ", documentgroupname='" + ManageQuote(documentstorelist[i].pDocumentGroup) + "', documentname='" + ManageQuote(documentstorelist[i].pDocumentName) + "', docstorepath='" + ManageQuote(documentstorelist[i].pDocStorePath) + "', docfiletype='" + ManageQuote(documentstorelist[i].pDocFileType) + "', docreferenceno='" + ManageQuote(documentstorelist[i].pDocReferenceno) + "', docisdownloadable=" + documentstorelist[i].pDocIsDownloadable + ", modifiedby=" + documentstorelist[i].pCreatedby + ", modifieddate=current_timestamp,filename='" + ManageQuote(documentstorelist[i].pFilename) + "' WHERE contactid=" + documentstorelist[i].pContactId + " and docstoreid=" + documentstorelist[i].pDocstoreId + ";");
                    }
                    // }

                }
                if (!string.IsNullOrEmpty(StoreRecordid))
                {
                    //query.Append("UPDATE tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + documentstorelist[0].pContactId + ",modifieddate=current_timestamp where contactid=" + documentstorelist[0].pContactId + " and docstoreid not in (" + StoreRecordid + ") AND statusid<>2 and coalesce(loanid,0)=0 ;");

                    if (applicationid > 0)
                    {
                        query.Append("UPDATE tblmstdocumentstore SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + documentstorelist[0].pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationno=" + applicationid + " AND docstoreid in(" + StoreRecordid + "); ");
                    }
                    else
                    {
                        query.Append("UPDATE tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + documentstorelist[0].pCreatedby + ",modifieddate=current_timestamp where contactid=" + numcontactid + " and docstoreid not in (" + StoreRecordid + ") AND statusid<>2 and coalesce(applicationno,0)=0 ;");
                    }
                }
                else
                {
                    query.Append("UPDATE tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + documentstorelist[0].pCreatedby + ",modifieddate=current_timestamp where contactid=" + numcontactid + " AND statusid<>" + Convert.ToInt32(Status.Inactive) + " and coalesce(applicationno,0)=0;");
                }
            }
            return query + "" + strUpdate;
        }
        public bool UpdatedReferral(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isUpdated = false;
            string Recordid = string.Empty;
            string StoreRecordid = string.Empty;
            StringBuilder strUpdate = new StringBuilder();
            //string query = "";
            StringBuilder query = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];


                strUpdate.Append("UPDATE tblmstreferral SET  titlename='" + ManageQuote(referralavocatelist.pTitleName) + "', name='" + ManageQuote(referralavocatelist.pName) + "', surname='" + ManageQuote(referralavocatelist.pSurName) + "',  modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE referralid=" + referralavocatelist.pReferralId + " and contactid=" + referralavocatelist.pContactId + ";");

                string updatedquery = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);


                if (referralavocatelist.referralbankdetailslist != null)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                        }
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            strUpdate.Append("INSERT INTO tblmstreferralbankdetails(referralid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + referralavocatelist.referralbankdetailslist[i].pReferralId + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp); ");
                        }
                        else if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() == "UPDATE")
                        {
                            strUpdate.Append("UPDATE tblmstreferralbankdetails SET  bankaccountname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "', bankname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "', bankaccountno='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "', bankifsccode='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "', bankbranch='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "', isprimaryaccount=" + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + ", modifiedby=" + referralavocatelist.referralbankdetailslist[i].pCreatedby + ", modifieddate=current_timestamp WHERE referralid=" + referralavocatelist.referralbankdetailslist[i].pReferralId + " and refbankid= " + referralavocatelist.referralbankdetailslist[i].pRefbankId + ";");
                        }

                    }

                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    query.Append("UPDATE tblmstreferralbankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE referralid=" + referralavocatelist.pReferralId + " and refbankid not in(" + Recordid + ")  AND statusid<>2;");
                }
                else
                {
                    query.Append("UPDATE tblmstreferralbankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE referralid=" + referralavocatelist.pReferralId + "  AND statusid<>2 ;");
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "CREATE")
                    {
                        strUpdate.Append("INSERT INTO tblmstreferraltaxdetails(referralid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + referralavocatelist.pReferralId + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                    }
                    else if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "UPDATE")
                    {
                        strUpdate.Append("UPDATE tblmstreferraltaxdetails SET  istdsapplicable=" + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ", tdssectionname='" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "', isgstapplicable=" + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ", statename='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "', gsttype='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "', gstno='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "', modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE referralid=" + referralavocatelist.pReferralId + " and reftaxid=" + referralavocatelist.referraltaxdetailslist.pRefTaxId + "; ");
                    }
                }
                if (referralavocatelist.referraltaxdetailslist == null)
                {
                    strUpdate.Append("UPDATE tblmstreferraltaxdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ",modifieddate=current_timestamp where referralid=" + referralavocatelist.pReferralId + "; ");
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updatedquery + "" + query + "" + strUpdate.ToString());
                trans.Commit();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                con.Dispose();
                con.Close();
                con.ClearPool();
                trans.Dispose();
            }
            return isUpdated;
        }
        public int CheckContactDuplicate(Int64 contactId, Int64 RefId, string ConnectionString)
        {
            int count = 0;
            try
            {
                string Query = "select count(*) from tblmstreferral t1 join tblmststatus ts on t1.statusid = ts.statusid where contactid =" + contactId + " and referralid<>" + RefId + " and upper(ts.statusname) = 'ACTIVE';";
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, Query));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public List<ReferralAdvocateDTO> GetDocumentProofs(Int64 docId, string ConnectionString)
        {
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            try
            {
                string Query = "select distinct documentid,documentname from  tblmstdocumentproofs where documentgroupid=" + docId + " and statusid=" + Convert.ToInt32(Status.Active) + " order by documentname";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalContactDetails = new ReferralAdvocateDTO();
                        objReferalContactDetails.pDocumentId = Convert.ToInt64(dr["documentid"]);
                        objReferalContactDetails.pDocumentName = Convert.ToString(dr["documentname"]);
                        lstReferalContactdetails.Add(objReferalContactDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }

        #region SaveTdsSectionNo
        public bool SaveTdsSectionNo(TdsSectionDTO tdsSectionNo, string con)
        {
            bool isSaved = false;

            try
            {
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "insert into tblmsttdssections(tdssection,statusid,createdby,createddate)values('" + ManageQuote(tdsSectionNo.pTdsSection.Trim()) + "'," + getStatusid(tdsSectionNo.pStatusname, con) + "," + tdsSectionNo.pCreatedby + ",current_timestamp);");
                isSaved = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isSaved;
        }
        #endregion
        #region GetGstType
        public List<GstTyeDTO> getGstType(string ConnectionString)
        {
            List<GstTyeDTO> lstGstTyedetails = new List<GstTyeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,gsttype from tblmstgsttype t1 join tblmststatus t2 on t1.statusid=t2.statusid  order by gsttype;"))
                {
                    while (dr.Read())
                    {
                        GstTyeDTO objGstTyedetails = new GstTyeDTO();
                        objGstTyedetails.pRecordid = Convert.ToInt64(dr["recordid"]);
                        objGstTyedetails.pGstType = Convert.ToString(dr["gsttype"]);
                        lstGstTyedetails.Add(objGstTyedetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstGstTyedetails;
        }
        #endregion
        #region SaveGstType
        public bool SaveGstType(GstTyeDTO gstType, string con)
        {
            bool isSaved = false;

            try
            {
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, "insert into tblmstgsttype(gsttype,statusid,createdby,createddate)values('" + ManageQuote(gstType.pGstType.Trim()) + "'," + getStatusid(gstType.pStatusname, con) + "," + gstType.pCreatedby + ",current_timestamp);");
                isSaved = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isSaved;
        }
        #endregion

        #region Save Advocate
        public bool saveAdvocate(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder ReferralDetailsinsert = new StringBuilder();
            long advocateid;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];

                advocateid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstadvocate (contactid,titlename,name,surname,statusid,createdby,createddate) values (" + referralavocatelist.pContactId + ",'" + ManageQuote(referralavocatelist.pTitleName.Trim()) + "','" + ManageQuote(referralavocatelist.pName.Trim()) + "','" + ManageQuote(referralavocatelist.pSurName.Trim()) + "'," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp) returning advocateid;"));
                //if (referralavocatelist.documentstorelist != null)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceMonth) && !string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceYear))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceMonth + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceYear;
                //        }
                //        else if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceMonth))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceMonth;
                //        }
                //        else if (!string.IsNullOrEmpty(referralavocatelist.documentstorelist[i].pDocumentReferenceYear))
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno + "~" + referralavocatelist.documentstorelist[i].pDocumentReferenceYear;
                //        }
                //        else
                //        {
                //            referralavocatelist.documentstorelist[i].pDocReferenceno = referralavocatelist.documentstorelist[i].pDocReferenceno.Trim();
                //        }
                //        Int64 count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(docstoreid) from tblmstdocumentstore where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and documentid=" + referralavocatelist.documentstorelist[i].pDocumentId + " and documentgroupid=" + referralavocatelist.documentstorelist[i].pDocumentGroupId + "and coalesce(loanid,0)=0"));
                //        if (count == 0)
                //        {
                //            ReferralDetailsinsert.Append("insert into tblmstdocumentstore (contactid ,applicationno,documentid,documentgroupid,documentgroupname,documentname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate) values (" + referralavocatelist.documentstorelist[i].pContactId + "," + referralavocatelist.documentstorelist[i].pApplicationNo + "," + referralavocatelist.documentstorelist[i].pDocumentId + "," + referralavocatelist.documentstorelist[i].pDocumentGroupId + ",'" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentGroup) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentName) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocStorePath) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocFileType) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocReferenceno) + "'," + referralavocatelist.documentstorelist[i].pDocIsDownloadable + "," + getStatusid(referralavocatelist.documentstorelist[i].pStatusname, ConnectionString) + "," + referralavocatelist.documentstorelist[i].pCreatedby + ",current_timestamp);");
                //        }
                //    }
                //}
                string updatedquery = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);
                if (referralavocatelist.referralbankdetailslist != null && referralavocatelist.referralbankdetailslist.Count > 0)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        ReferralDetailsinsert.Append("INSERT INTO tblmstadvocatebankdetails(advocateid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + advocateid + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.referralbankdetailslist[i].pStatusname, ConnectionString) + "," + referralavocatelist.referralbankdetailslist[i].pCreatedby + ",current_timestamp); ");
                    }
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    ReferralDetailsinsert.Append("INSERT INTO tblmstadvocatetaxdetails(advocateid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + advocateid + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                }
                if (ReferralDetailsinsert.Length > 0 || updatedquery.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updatedquery + "" + ReferralDetailsinsert.ToString());
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
        #region GetAdvocate/Lawyer Details for Main Grid
        public List<ReferralAdvocateDTO> getAdvocateLawterDetails(string Type, string ConnectionString)
        {
            string Query = string.Empty;
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL")
                {
                    Query = "select t1.advocateid,t1.contactid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstadvocate t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' order by t1.advocateid";
                }
                else
                {
                    Query = "select t1.advocateid,t1.contactid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstadvocate t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid  where upper(t1.name) like'%" + Type + "%' or upper(t1.surname) like '%" + Type + "%' or upper(t2.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%' or contacttype like '%" + Type + "%' and upper(ts.statusname) = 'ACTIVE'; ";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferalAgentDetails.pReferralId = Convert.ToInt64(dr["advocateid"]);
                        objReferalAgentDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferalAgentDetails.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferalAgentDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferalAgentDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        lstReferalContactdetails.Add(objReferalAgentDetails);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }
        #region Get Advocate/Lawer Data For Edit
        public ReferralAdvocateDTO ViewAdvocateLawerDetails(Int64 refid, string ConnectionString)
        {
            ReferralAdvocateDTO objReferralAdvocateDTO = new ReferralAdvocateDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.advocateid,t1.contactid,t1.name,t1.surname,(t1.name||' '||t1.surname) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contactimagepath,statusname from tblmstadvocate t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t2.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' and t1.advocateid=" + refid + " "))
                {
                    while (dr.Read())
                    {
                        objReferralAdvocateDTO.pReferralId = Convert.ToInt64(dr["advocateid"]);
                        objReferralAdvocateDTO.pName = Convert.ToString(dr["name"]);
                        objReferralAdvocateDTO.pSurName = Convert.ToString(dr["surname"]);
                        objReferralAdvocateDTO.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferralAdvocateDTO.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferralAdvocateDTO.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objReferralAdvocateDTO.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferralAdvocateDTO.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferralAdvocateDTO.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
                objReferralAdvocateDTO.documentstorelist = getDocumentstoreDetails(ConnectionString, objReferralAdvocateDTO.pContactId, "");
                objReferralAdvocateDTO.referralbankdetailslist = getAdvocatebankDetails(ConnectionString, refid);
                objReferralAdvocateDTO.referraltaxdetailslist = getAdvocateTaxDetails(ConnectionString, refid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReferralAdvocateDTO;
        }
        #endregion
        private List<referralbankdetailsDTO> getAdvocatebankDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referralbankdetails = new List<referralbankdetailsDTO>();
            try
            {
                strquery = "SELECT  advbankid,advocateid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, t.statusid,statusname,'OLD' as typeofoperation FROM tblmstadvocatebankdetails t join tblmststatus ts on t.statusid = ts.statusid and upper(ts.statusname) = 'ACTIVE' where advocateid=" + pReferralId + ";";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        referralbankdetailsDTO objreferralbankdetail = new referralbankdetailsDTO();
                        objreferralbankdetail.pRefbankId = Convert.ToInt64(dr["advbankid"]);
                        objreferralbankdetail.pReferralId = Convert.ToInt64(dr["advocateid"]);
                        objreferralbankdetail.pBankAccountname = Convert.ToString(dr["bankaccountname"]);
                        objreferralbankdetail.pBankName = Convert.ToString(dr["bankname"]);
                        objreferralbankdetail.pBankAccountNo = Convert.ToString(dr["bankaccountno"]);
                        objreferralbankdetail.pBankifscCode = Convert.ToString(dr["bankifsccode"]);
                        objreferralbankdetail.pBankBranch = Convert.ToString(dr["bankbranch"]);
                        objreferralbankdetail.pIsprimaryAccount = Convert.ToBoolean(dr["isprimaryaccount"]);
                        objreferralbankdetail.pStatusname = Convert.ToString(dr["statusname"]);
                        objreferralbankdetail.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        referralbankdetails.Add(objreferralbankdetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return referralbankdetails;
        }
        private referraltaxdetailsDTO getAdvocateTaxDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referraltaxdetailsDTO objreferraltaxdetail = null;

            try
            {
                strquery = "SELECT  advtaxid,advocateid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, t.statusid,statusname  FROM tblmstadvocatetaxdetails t join tblmststatus ts on t.statusid = ts.statusid where advocateid=" + pReferralId + " and upper(ts.statusname) = 'ACTIVE';";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        objreferraltaxdetail = new referraltaxdetailsDTO();
                        objreferraltaxdetail.pRefTaxId = Convert.ToInt64(dr["advtaxid"]);
                        objreferraltaxdetail.pReferralId = Convert.ToInt64(dr["advocateid"]);
                        objreferraltaxdetail.pIstdsApplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                        objreferraltaxdetail.ptdsSectionName = Convert.ToString(dr["tdssectionname"]);
                        objreferraltaxdetail.pIsgstApplicable = Convert.ToBoolean(dr["isgstapplicable"]);
                        objreferraltaxdetail.pStateName = Convert.ToString(dr["statename"]);
                        objreferraltaxdetail.pGstType = Convert.ToString(dr["gsttype"]);
                        objreferraltaxdetail.pGstNo = Convert.ToString(dr["gstno"]);
                        objreferraltaxdetail.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objreferraltaxdetail;
        }
        public int CheckAdvocateDuplicate(Int64 contactId, Int64 refId, string ConnectionString)
        {
            int count = 0;
            try
            {
                string Query = "select count(*) from tblmstadvocate t1 join tblmststatus ts on t1.statusid = ts.statusid where contactid =" + contactId + " and advocateid<>" + refId + " and upper(ts.statusname) = 'ACTIVE';";
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, Query));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public bool DeleteAdvocateLawer(DeleteDTO objDeleteDTO, string ConnectionString)
        {
            bool isSaved = false;

            StringBuilder ReferralDetailsDelete = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                ReferralDetailsDelete.Append("UPDATE tblmstadvocate set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where advocateid=" + objDeleteDTO.pReferralId + "; ");
                //if (referralavocatelist.documentstorelist.Count > 0)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        ReferralDetailsDelete.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + referralavocatelist.documentstorelist[i].pCreatedby + ",modifieddate=current_timestamp where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and docstoreid=" + referralavocatelist.documentstorelist[i].pDocstoreId + "; ");
                //    }
                //}
                ReferralDetailsDelete.Append("UPDATE tblmstadvocatebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where advocateid=" + objDeleteDTO.pReferralId + "; ");
                ReferralDetailsDelete.Append("UPDATE tblmstadvocatetaxdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where advocateid=" + objDeleteDTO.pReferralId + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, ReferralDetailsDelete.ToString());
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
        public bool UpdatedAdvocateLawer(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isUpdated = false;
            string Recordid = string.Empty;
            StringBuilder strUpdate = new StringBuilder();
            //string query = "";
            StringBuilder query = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];

                strUpdate.Append("UPDATE tblmstadvocate SET  titlename='" + ManageQuote(referralavocatelist.pTitleName) + "', name='" + ManageQuote(referralavocatelist.pName) + "', surname='" + ManageQuote(referralavocatelist.pSurName) + "',  modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE advocateid=" + referralavocatelist.pReferralId + " and contactid=" + referralavocatelist.pContactId + ";");
                string str = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);
                //if (referralavocatelist.documentstorelist != null)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        //if (referralavocatelist.documentstorelist[i].ptypeofoperation != "CREATE")
                //        //{
                //        //    if (string.IsNullOrEmpty(Recordid))
                //        //    {
                //        //        Recordid = referralavocatelist.documentstorelist[i].pDocstoreId.ToString();
                //        //    }
                //        //    else
                //        //    {
                //        //        Recordid = Recordid + "," + referralavocatelist.documentstorelist[i].pDocstoreId.ToString();
                //        //    }
                //        //}
                //        Int64 count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(docstoreid) from tblmstdocumentstore where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and documentid=" + referralavocatelist.documentstorelist[i].pDocumentId + " and documentgroupid=" + referralavocatelist.documentstorelist[i].pDocumentGroupId + " and coalesce(loanid,0)=0"));
                //        if (count == 0)
                //        {
                //            if (referralavocatelist.documentstorelist[i].ptypeofoperation == "CREATE")
                //            {
                //                strUpdate.Append("insert into tblmstdocumentstore (contactid ,applicationno,documentid,documentgroupid,documentgroupname,documentname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate) values (" + referralavocatelist.documentstorelist[i].pContactId + "," + referralavocatelist.documentstorelist[i].pApplicationNo + "," + referralavocatelist.documentstorelist[i].pDocumentId + "," + referralavocatelist.documentstorelist[i].pDocumentGroupId + ",'" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentGroup) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentName) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocStorePath) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocFileType) + "','" + ManageQuote(referralavocatelist.documentstorelist[i].pDocReferenceno) + "'," + referralavocatelist.documentstorelist[i].pDocIsDownloadable + "," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp);");
                //            }
                //        }
                //        else if (referralavocatelist.documentstorelist[i].ptypeofoperation == "UPDATE")
                //        {
                //            strUpdate.Append("UPDATE tblmstdocumentstore SET   applicationno=" + referralavocatelist.documentstorelist[i].pApplicationNo + ", documentid=" + referralavocatelist.documentstorelist[i].pDocumentId + ", documentgroupid=" + referralavocatelist.documentstorelist[i].pDocumentGroupId + ", documentgroupname='" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentGroup) + "', documentname='" + ManageQuote(referralavocatelist.documentstorelist[i].pDocumentName) + "', docstorepath='" + ManageQuote(referralavocatelist.documentstorelist[i].pDocStorePath) + "', docfiletype='" + ManageQuote(referralavocatelist.documentstorelist[i].pDocFileType) + "', docreferenceno='" + ManageQuote(referralavocatelist.documentstorelist[i].pDocReferenceno) + "', docisdownloadable=" + referralavocatelist.documentstorelist[i].pDocIsDownloadable + ", modifiedby=" + referralavocatelist.documentstorelist[i].pCreatedby + ", modifieddate=current_timestamp WHERE contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and docstoreid=" + referralavocatelist.documentstorelist[i].pDocstoreId + ";");
                //        }


                //    }

                //}
                //if (!string.IsNullOrEmpty(Recordid))
                //{
                //    query.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + referralavocatelist.pCreatedby + ",modifieddate=current_timestamp where contactid=" + referralavocatelist.pContactId + " and docstoreid not in (" + Recordid + ") ;");
                //}
                //else
                //{
                //    query.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + referralavocatelist.pCreatedby + ",modifieddate=current_timestamp where contactid=" + referralavocatelist.pContactId + ";");
                //}
                if (referralavocatelist.referralbankdetailslist != null)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation != "CREATE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                        }
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation == "CREATE")
                        {
                            strUpdate.Append("INSERT INTO tblmstadvocatebankdetails(advocateid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + referralavocatelist.referralbankdetailslist[i].pReferralId + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp); ");
                        }
                        else if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation == "UPDATE")
                        {
                            strUpdate.Append("UPDATE tblmstadvocatebankdetails SET  bankaccountname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "', bankname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "', bankaccountno='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "', bankifsccode='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "', bankbranch='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "', isprimaryaccount=" + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + ", modifiedby=" + referralavocatelist.referralbankdetailslist[i].pCreatedby + ", modifieddate=current_timestamp WHERE advocateid=" + referralavocatelist.referralbankdetailslist[i].pReferralId + " and advbankid= " + referralavocatelist.referralbankdetailslist[i].pRefbankId + ";");
                        }

                    }

                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    query.Append("UPDATE tblmstadvocatebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE advocateid=" + referralavocatelist.pReferralId + " and advbankid not in(" + Recordid + ")  AND statusid<>2;");
                }
                else
                {
                    query.Append("UPDATE tblmstadvocatebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE advocateid=" + referralavocatelist.pReferralId + "  AND statusid<>2 ;");
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "CREATE")
                    {
                        strUpdate.Append("INSERT INTO tblmstadvocatetaxdetails(advocateid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + referralavocatelist.pReferralId + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                    }
                    else if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "UPDATE")
                    {
                        strUpdate.Append("UPDATE tblmstadvocatetaxdetails SET  istdsapplicable=" + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ", tdssectionname='" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "', isgstapplicable=" + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ", statename='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "', gsttype='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "', gstno='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "', modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE advocateid=" + referralavocatelist.pReferralId + " and advtaxid=" + referralavocatelist.referraltaxdetailslist.pRefTaxId + "; ");
                    }
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, str + "" + query + "" + strUpdate.ToString());
                trans.Commit();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                con.Dispose();
                con.Close();
                con.ClearPool();
                trans.Dispose();
            }
            return isUpdated;
        }


        #endregion
        public int CheckTdsSectionDuplicate(string tdsSecName, string con)
        {
            int count = 0;
            try
            {
                string Query = "select count(*) from tblmsttdssections where upper(tdssection) =upper('" + tdsSecName + "');";
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, Query));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int CheckGstTypeDuplicate(string strGstType, string con)
        {
            int count = 0;
            try
            {
                string Query = "select count(*) from tblmstgsttype  where upper(gsttype) =upper('" + strGstType + "');";
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, Query));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        #region Party 

        public bool saveParty(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder ReferralDetailsinsert = new StringBuilder();
            long ReferralId;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];

                ReferralId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstparty (contactid,titlename,name,surname,statusid,createdby,createddate) values (" + referralavocatelist.pContactId + ",'" + ManageQuote(referralavocatelist.pTitleName.Trim()) + "','" + ManageQuote(referralavocatelist.pName.Trim()) + "','" + ManageQuote(referralavocatelist.pSurName.Trim()) + "'," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp) returning partiid;"));

                string str = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);
                if (referralavocatelist.referralbankdetailslist != null && referralavocatelist.referralbankdetailslist.Count > 0)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        ReferralDetailsinsert.Append("INSERT INTO tblmstpartybankdetails(partiid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + ReferralId + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.referralbankdetailslist[i].pStatusname, ConnectionString) + "," + referralavocatelist.referralbankdetailslist[i].pCreatedby + ",current_timestamp); ");
                    }
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    ReferralDetailsinsert.Append("INSERT INTO tblmstpartytaxdetails(partiid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + ReferralId + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                }
                if (ReferralDetailsinsert.Length > 0 || str.Length > 0)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, str + "" + ReferralDetailsinsert.ToString());
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
        public List<ReferralAdvocateDTO> getPartyDetails(string Type, string ConnectionString)
        {
            string Query = string.Empty;
            lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL")
                {
                    Query = "select t1.partiid,t1.contactid,contactreferenceid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstparty t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid where  upper(ts.statusname) = 'ACTIVE' order by t1.partiid";
                }
                else
                {
                    Query = "select t1.partiid,t1.contactid,contactreferenceid,(coalesce(t.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstparty t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t1.statusid = ts.statusid  where upper(t1.name) like'%" + Type + "%' or upper(t1.surname) like '%" + Type + "%' or upper(t2.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%' or contacttype like '%" + Type + "%' and upper(ts.statusname) = 'ACTIVE'; ";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferalAgentDetails.pReferralId = Convert.ToInt64(dr["partiid"]);
                        objReferalAgentDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferalAgentDetails.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferalAgentDetails.pContactReferanceId = Convert.ToString(dr["contactreferenceid"]);
                        objReferalAgentDetails.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferalAgentDetails.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        lstReferalContactdetails.Add(objReferalAgentDetails);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstReferalContactdetails;
        }
        public ReferralAdvocateDTO ViewPartyDetails(Int64 refid, string ConnectionString)
        {
            ReferralAdvocateDTO objReferralAdvocateDTO = new ReferralAdvocateDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select t1.partiid,t1.contactid,t1.name,t1.surname,(coalesce(t1.name,'')||' '||coalesce(t1.surname,'')) as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contactimagepath,statusname,t1.statusid,t1.createdby,t1.createddate from tblmstparty t1 join tblmstcontact t2 on t1.contactid=t2.contactid join tblmststatus ts on t2.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' and t1.partiid=" + refid + " "))
                {
                    while (dr.Read())
                    {
                        //ReferralAdvocateDTO objReferalAgentDetails = new ReferralAdvocateDTO();
                        objReferralAdvocateDTO.pReferralId = Convert.ToInt64(dr["partiid"]);
                        objReferralAdvocateDTO.pName = Convert.ToString(dr["name"]);
                        objReferralAdvocateDTO.pSurName = Convert.ToString(dr["surname"]);
                        objReferralAdvocateDTO.pContactId = Convert.ToInt64(dr["contactid"]);
                        objReferralAdvocateDTO.pAdvocateName = Convert.ToString(dr["advocatename"]);
                        objReferralAdvocateDTO.pContactimagepath = Convert.ToString(dr["contactimagepath"]);
                        objReferralAdvocateDTO.pBusinessEntitycontactNo = Convert.ToString(dr["businessentitycontactno"]);
                        objReferralAdvocateDTO.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        objReferralAdvocateDTO.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
                objReferralAdvocateDTO.documentstorelist = getDocumentstoreDetails(ConnectionString, objReferralAdvocateDTO.pContactId, "");
                objReferralAdvocateDTO.referralbankdetailslist = getPartybankDetails(ConnectionString, refid);
                objReferralAdvocateDTO.referraltaxdetailslist = getPartyTaxDetails(ConnectionString, refid);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return objReferralAdvocateDTO;
        }
        private List<referralbankdetailsDTO> getPartybankDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referralbankdetails = new List<referralbankdetailsDTO>();
            try
            {
                strquery = "SELECT  refbankid,partiid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, t.statusid,statusname,t.createdby,t.createddate,'OLD' as typeofoperation FROM tblmstpartybankdetails t join tblmststatus ts on t.statusid = ts.statusid and upper(ts.statusname) = 'ACTIVE'  where partiid=" + pReferralId + ";";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        referralbankdetailsDTO objreferralbankdetail = new referralbankdetailsDTO();
                        objreferralbankdetail.pRefbankId = Convert.ToInt64(dr["refbankid"]);
                        objreferralbankdetail.pReferralId = Convert.ToInt64(dr["partiid"]);
                        objreferralbankdetail.pBankAccountname = Convert.ToString(dr["bankaccountname"]);
                        objreferralbankdetail.pBankName = Convert.ToString(dr["bankname"]);
                        objreferralbankdetail.pBankAccountNo = Convert.ToString(dr["bankaccountno"]);
                        objreferralbankdetail.pBankifscCode = Convert.ToString(dr["bankifsccode"]);
                        objreferralbankdetail.pBankBranch = Convert.ToString(dr["bankbranch"]);
                        objreferralbankdetail.pIsprimaryAccount = Convert.ToBoolean(dr["isprimaryaccount"]);
                        objreferralbankdetail.pStatusname = Convert.ToString(dr["statusname"]);
                        objreferralbankdetail.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        referralbankdetails.Add(objreferralbankdetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return referralbankdetails;

        }
        private referraltaxdetailsDTO getPartyTaxDetails(string connectionString, Int64 pReferralId)
        {
            string strquery = string.Empty;
            referraltaxdetailsDTO objreferraltaxdetail = null;

            try
            {

                strquery = "SELECT  reftaxid,partiid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, t.statusid,statusname ,t.statusid,t.createdby,t.createddate FROM tblmstpartytaxdetails t join tblmststatus ts on t.statusid = ts.statusid where partiid=" + pReferralId + " and upper(ts.statusname) = 'ACTIVE';";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        objreferraltaxdetail = new referraltaxdetailsDTO();
                        objreferraltaxdetail.pRefTaxId = Convert.ToInt64(dr["reftaxid"]);
                        objreferraltaxdetail.pReferralId = Convert.ToInt64(dr["partiid"]);
                        objreferraltaxdetail.pIstdsApplicable = Convert.ToBoolean(dr["istdsapplicable"]);
                        objreferraltaxdetail.ptdsSectionName = Convert.ToString(dr["tdssectionname"]);
                        objreferraltaxdetail.pIsgstApplicable = Convert.ToBoolean(dr["isgstapplicable"]);
                        objreferraltaxdetail.pStateName = Convert.ToString(dr["statename"]);
                        objreferraltaxdetail.pGstType = Convert.ToString(dr["gsttype"]);
                        objreferraltaxdetail.pGstNo = Convert.ToString(dr["gstno"]);
                        objreferraltaxdetail.pStatusname = Convert.ToString(dr["statusname"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objreferraltaxdetail;
        }
        public bool DeleteParty(DeleteDTO objDeleteDTO, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder ReferralDetailsDelete = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                ReferralDetailsDelete.Append("UPDATE tblmstparty set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where partiid=" + objDeleteDTO.pReferralId + "; ");
                //if (referralavocatelist.documentstorelist.Count > 0)
                //{
                //    for (int i = 0; i < referralavocatelist.documentstorelist.Count; i++)
                //    {
                //        ReferralDetailsDelete.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + referralavocatelist.documentstorelist[i].pCreatedby + ",modifieddate=current_timestamp where contactid=" + referralavocatelist.documentstorelist[i].pContactId + " and docstoreid=" + referralavocatelist.documentstorelist[i].pDocstoreId + "; ");
                //    }

                //}
                ReferralDetailsDelete.Append("UPDATE tblmstpartybankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where partiid=" + objDeleteDTO.pReferralId + "; ");
                ReferralDetailsDelete.Append("UPDATE tblmstpartytaxdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objDeleteDTO.pCreatedby + ",modifieddate=current_timestamp where partiid=" + objDeleteDTO.pReferralId + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, ReferralDetailsDelete.ToString());
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
        public bool UpdatedParty(ReferralAdvocateDTO referralavocatelist, string ConnectionString)
        {
            bool isUpdated = false;
            string Recordid = string.Empty;
            string StoreRecordid = string.Empty;
            StringBuilder strUpdate = new StringBuilder();
            //string query = "";
            StringBuilder query = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + referralavocatelist.pContactId));
                referralavocatelist.pName = contactdetails.Split('@')[0];
                referralavocatelist.pSurName = contactdetails.Split('@')[1];

                strUpdate.Append("UPDATE tblmstparty SET  titlename='" + ManageQuote(referralavocatelist.pTitleName) + "', name='" + ManageQuote(referralavocatelist.pName) + "', surname='" + ManageQuote(referralavocatelist.pSurName) + "',  modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE partiid=" + referralavocatelist.pReferralId + " and contactid=" + referralavocatelist.pContactId + ";");

                string updatedquery = UpdateStoreDetails(referralavocatelist.documentstorelist, ConnectionString, 0, referralavocatelist.pContactId);


                if (referralavocatelist.referralbankdetailslist != null)
                {
                    for (int i = 0; i < referralavocatelist.referralbankdetailslist.Count; i++)
                    {
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + referralavocatelist.referralbankdetailslist[i].pRefbankId.ToString();
                            }
                        }
                        if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() == "CREATE")
                        {
                            strUpdate.Append("INSERT INTO tblmstpartybankdetails(partiid, bankaccountname, bankname, bankaccountno, bankifsccode, bankbranch, isprimaryaccount, statusid, createdby, createddate) VALUES (" + referralavocatelist.referralbankdetailslist[i].pReferralId + ",'" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "','" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "'," + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + "," + getStatusid(referralavocatelist.pStatusname, ConnectionString) + "," + referralavocatelist.pCreatedby + ",current_timestamp); ");
                        }
                        else if (referralavocatelist.referralbankdetailslist[i].ptypeofoperation.ToUpper() == "UPDATE")
                        {
                            strUpdate.Append("UPDATE tblmstpartybankdetails SET  bankaccountname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountname) + "', bankname='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankName) + "', bankaccountno='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankAccountNo) + "', bankifsccode='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankifscCode) + "', bankbranch='" + ManageQuote(referralavocatelist.referralbankdetailslist[i].pBankBranch) + "', isprimaryaccount=" + referralavocatelist.referralbankdetailslist[i].pIsprimaryAccount + ", modifiedby=" + referralavocatelist.referralbankdetailslist[i].pCreatedby + ", modifieddate=current_timestamp WHERE partiid=" + referralavocatelist.referralbankdetailslist[i].pReferralId + " and refbankid= " + referralavocatelist.referralbankdetailslist[i].pRefbankId + ";");
                        }

                    }

                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    query.Append("UPDATE tblmstpartybankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE partiid=" + referralavocatelist.pReferralId + " and refbankid not in(" + Recordid + ")  AND statusid<>2;");
                }
                else
                {
                    query.Append("UPDATE tblmstpartybankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE partiid=" + referralavocatelist.pReferralId + "  AND statusid<>2 ;");
                }
                if (referralavocatelist.referraltaxdetailslist != null)
                {
                    if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "CREATE")
                    {
                        strUpdate.Append("INSERT INTO tblmstpartytaxdetails(partiid, istdsapplicable, tdssectionname, isgstapplicable, statename, gsttype, gstno, statusid, createdby, createddate) VALUES (" + referralavocatelist.pReferralId + "," + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "'," + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ",'" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "','" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "'," + getStatusid(referralavocatelist.referraltaxdetailslist.pStatusname, ConnectionString) + "," + referralavocatelist.referraltaxdetailslist.pCreatedby + ",current_timestamp);");
                    }
                    else if (referralavocatelist.referraltaxdetailslist.ptypeofoperation.ToUpper() == "UPDATE")
                    {
                        strUpdate.Append("UPDATE tblmstpartytaxdetails SET  istdsapplicable=" + referralavocatelist.referraltaxdetailslist.pIstdsApplicable + ", tdssectionname='" + ManageQuote(referralavocatelist.referraltaxdetailslist.ptdsSectionName) + "', isgstapplicable=" + referralavocatelist.referraltaxdetailslist.pIsgstApplicable + ", statename='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pStateName) + "', gsttype='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstType) + "', gstno='" + ManageQuote(referralavocatelist.referraltaxdetailslist.pGstNo) + "', modifiedby=" + referralavocatelist.pCreatedby + ", modifieddate=current_timestamp WHERE partiid=" + referralavocatelist.pReferralId + " and reftaxid=" + referralavocatelist.referraltaxdetailslist.pRefTaxId + "; ");
                    }
                }
                if (referralavocatelist.referraltaxdetailslist == null)
                {
                    strUpdate.Append("UPDATE tblmstpartytaxdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + referralavocatelist.pCreatedby + ",modifieddate=current_timestamp where partiid=" + referralavocatelist.pReferralId + "; ");
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updatedquery + "" + query + "" + strUpdate.ToString());
                trans.Commit();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                con.Dispose();
                con.Close();
                con.ClearPool();
                trans.Dispose();
            }
            return isUpdated;
        }
        public int CheckPartyDuplicate(Int64 contactId, Int64 RefId, string ConnectionString)
        {
            int count = 0;
            try
            {
                string Query = "select count(*) from tblmstparty t1 join tblmststatus ts on t1.statusid = ts.statusid where contactid =" + contactId + " and partiid<>" + RefId + " and upper(ts.statusname) = 'ACTIVE';";
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, Query));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public string UpdateContactBankDetails(string ConnectionString, List<referralbankdetailsDTO> banklistlist, object numcontactid, NpgsqlTransaction trans)
        {
            StringBuilder _sb = new StringBuilder();
            string _query = string.Empty;
            string Recordid = string.Empty;
            object count = 0;
            for (int i = 0; i < banklistlist.Count; i++)
            {
                if (Convert.ToString(banklistlist[i].pBankId) == string.Empty)
                {

                    banklistlist[i].pBankId = "null";
                }
                if (Convert.ToString(banklistlist[i].ptypeofoperation) == "DELETE")
                {
                    if (string.IsNullOrEmpty(Recordid))
                    {
                        Recordid = banklistlist[i].precordid.ToString();
                    }
                    else
                    {
                        Recordid = Recordid + "," + banklistlist[i].precordid.ToString();
                    }
                }
                //_query = "select count(*) from " + AddDoubleQuotes(GlobalSchema) + ".tbl_mst_contact_bank  where tbl_mst_contact_bank_id=" + banklistlist[i].pContactbankId;
                //count = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, _query);
                if (Convert.ToString(banklistlist[i].ptypeofoperation) == "CREATE")
                {
                    _query = "INSERT INTO tbl_mst_contact_bank(contact_id,bank_id, bank_branch_name, bank_account_number, bank_ifsc_code,isprimary,status)VALUES(" + numcontactid + "," + banklistlist[i].pBankId + ",'" + banklistlist[i].pBankBranch + "','" + banklistlist[i].pBankAccountNo + "','" + banklistlist[i].pBankifscCode + "','" + banklistlist[i].pIsprimaryAccount + "','true') returning tbl_mst_contact_bank_id";
                    banklistlist[i].pContactbankId = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, _query);

                }
                else if (Convert.ToString(banklistlist[i].ptypeofoperation) == "UPDATE")
                {
                    _sb.Append("UPDATE tbl_mst_contact_bank   SET  bank_id=" + banklistlist[i].pBankId + ",bank_branch_name='" + banklistlist[i].pBankBranch + "', bank_account_number='" + banklistlist[i].pBankAccountNo + "', bank_ifsc_code='" + banklistlist[i].pBankifscCode + "',isprimary='" + banklistlist[i].pIsprimaryAccount + "', status='true' WHERE tbl_mst_contact_bank_id=" + banklistlist[i].precordid + " and contact_id=" + numcontactid + ";");


                }

            }
            if (!string.IsNullOrEmpty(Recordid))
            {
                _sb.Append("update tbl_mst_contact_bank set status='false' where contact_id=" + numcontactid + " and tbl_mst_contact_bank_id in(" + Recordid + ");");
            }
            return Convert.ToString(_sb);
        }




        #endregion
    }
}
