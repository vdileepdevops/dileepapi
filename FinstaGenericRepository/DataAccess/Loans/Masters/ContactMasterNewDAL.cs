using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Masters;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FinstaInfrastructure.Loans.Transactions;
using System.IO;
using System.Drawing;
using FinstaInfrastructure;
using FinstaInfrastructure.Settings;
using System.Threading.Tasks;


namespace FinstaRepository.DataAccess.Loans.Masters
{
    public class ContactMasterNewDAL :SettingsDAL,IContactMasterNew
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlDataReader dr = null;
        DataSet ds = null;

        NpgsqlTransaction trans = null;
        public List<contactAddressNewDTO> lstAddressDetails { get; set; }
        public List<EnterpriseTypeNewDTO> lstEnterpriseType { get; set; }
        public List<BusinessTypeNewDTO> lstBusinessType { get; set; }
        public List<ContactViewNewDTO> lstContactViewDTO { get; set; }
        public List<FirstinformationDTO> lstFirstinformationDTO { set; get; }
        #region SaveContact
        public bool Savecontact(ContactMasterNewDTO contact, out string contactid, string ConnectionString)
        {
            bool isSaved = false;
            contactid = string.Empty;
            long ContactRefid;
            try
            {
                StringBuilder sb = new StringBuilder();



                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(contact.pDob))
                {

                    contact.pDob = "null";
                }
                else
                {

                    contact.pDob = "'" + FormatDate(contact.pDob) + "'";
                }
                if (Convert.ToString(contact.pBusinessEntityContactno) != string.Empty)
                {
                    contact.pBusinessEntityContactno = contact.pBusinessEntityContactno;
                }
                else
                {
                    contact.pBusinessEntityContactno = "null";
                }
                if (Convert.ToString(contact.pAlternativeNo) != string.Empty)
                {
                    contact.pAlternativeNo = contact.pAlternativeNo;
                }
                else
                {
                    contact.pAlternativeNo = "null";
                }
                contact.pReferenceId = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('CONTACT','" + ManageQuote(contact.pContactType) + "',CURRENT_DATE)").ToString();

                if (contact.pContactType == "Individual")
                {
                    ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstcontact(transdate,contactreferenceid,contacttype,titlename,name,surname,businessentityemailid  , businessentitycontactno, dob,gender,fathername,spousename,contactimagepath,statusid,createdby,createddate,secondary_contact_number,secondary_emailid,contact_mailing_name) values(CURRENT_DATE,'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactType) + "','" + ManageQuote(contact.pTitleName) + "','" + ManageQuote(contact.pName) + "','" + ManageQuote(contact.pSurName) + "','" + ManageQuote(contact.pBusinessEntityEmailid) + "','" + ManageQuote(contact.pBusinessEntityContactno) + "'," + (contact.pDob) + ",'" + ManageQuote(contact.pGender) + "','" + ManageQuote(contact.pFatherName) + "','" + ManageQuote(contact.pSpouseName) + "','" + ManageQuote(contact.pContactimagepath) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp," + contact.pAlternativeNo + ",'" + contact.pEmailId2 + "','" + contact.pContactName + "') returning CONTACTID;"));
                    for (var i = 0; i < contact.pAddressList.Count; i++)
                    {
                        sb.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate,longitude,latitude,isprimary) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + (contact.pAddressList[i].pAddressPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp,'" + contact.pAddressList[i].plongitude + "','" + contact.pAddressList[i].platitude + "'," + (contact.pAddressList[i].pPriorityCon) + ");");
                    }
                    if (contact.pEmailidsList != null)
                    {
                        for (var i = 0; i < contact.pEmailidsList.Count; i++)
                        {
                            if (contact.pEmailidsList[i].pContactNumber == "" || contact.pEmailidsList[i].pContactNumber == string.Empty || contact.pEmailidsList[i].pContactNumber == null)
                            {
                                contact.pEmailidsList[i].pContactNumber = "0";
                            }

                            sb.Append("insert into TBLMSTCONTACTPERSONDETAILS(contactid,contactreferenceid,contactname,contactnumber,emailid,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactName) + "'," + contact.pEmailidsList[i].pContactNumber + ",'" + ManageQuote(contact.pEmailidsList[i].pEmailId) + "','" + ManageQuote(contact.pEmailidsList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");
                        }
                    }
                }

                else
                {

                    ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstcontact(transdate,contactreferenceid,contacttype,name,typeofenterprise,natureofbusiness,businessentityemailid  , businessentitycontactno,contactimagepath,statusid,createdby,createddate,secondary_contact_number,secondary_emailid,contact_mailing_name) values(CURRENT_DATE,'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactType) + "','" + ManageQuote(contact.pName) + "','" + ManageQuote(contact.pEnterpriseType) + "','" + ManageQuote(contact.pBusinesstype) + "','" + ManageQuote(contact.pBusinessEntityEmailid) + "','" + ManageQuote(contact.pBusinessEntityContactno) + "','" + ManageQuote(contact.pContactimagepath) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp," + contact.pAlternativeNo + ",'" + contact.pEmailId2 + "','" + contact.pName + "') returning CONTACTID;"));
                    if (contact.pAddressList != null)
                    {
                        for (int i = 0; i < contact.pAddressList.Count; i++)
                        {

                            sb.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate,longitude,latitude,isprimary) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + (contact.pAddressList[i].pAddressPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp,'" + contact.pAddressList[i].plongitude + "','" + contact.pAddressList[i].platitude + "'," + (contact.pAddressList[i].pPriorityCon) + ");");

                        }
                        if (contact.pbusinessList.Count > 0)
                        {
                            for (int i = 0; i < contact.pbusinessList.Count; i++)
                            {
                                if (string.IsNullOrEmpty(contact.pbusinessList[i].pContactId.ToString()))
                                {

                                    contact.pbusinessList[i].pContactId = "null";
                                }
                                sb.Append("insert into tbl_mst_contact_business_entity(contact_id,business_entity_contact_id,desigination_id,isprimary,status) values (" + ContactRefid + "," + contact.pbusinessList[i].pContactId + "," + contact.pbusinessList[i].designationid + "," + contact.pbusinessList[i].pBusinessPriority + ",true);");
                            }
                        }
                    }

                    if (contact.pEmailidsList != null)
                    {
                        for (int i = 0; i < contact.pEmailidsList.Count; i++)
                        {
                            if (contact.pEmailidsList[i].pContactNumber == "" || contact.pEmailidsList[i].pContactNumber == string.Empty || contact.pEmailidsList[i].pContactNumber == null)
                            {
                                contact.pEmailidsList[i].pContactNumber = "0";
                            }

                            sb.Append("insert into TBLMSTCONTACTPERSONDETAILS(contactid,contactreferenceid,contactname,contactnumber,emailid,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactName) + "'," + contact.pEmailidsList[i].pContactNumber + ",'" + ManageQuote(contact.pEmailidsList[i].pEmailId) + "','" + ManageQuote(contact.pEmailidsList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");
                        }
                    }

                }
                ReferralAdvocateDAL _ReferralAdvocateDAL = new ReferralAdvocateDAL();
                if (contact.documentstorelist != null)
                {
                    if (contact.documentstorelist.Count > 0)
                    {
                        sb.Append(_ReferralAdvocateDAL.UpdateStoreDetails(contact.documentstorelist, ConnectionString, 0, ContactRefid));
                    }
                }
                if (contact.referralbankdetailslist != null)
                {
                    if (contact.referralbankdetailslist.Count > 0)
                    {
                        sb.Append(_ReferralAdvocateDAL.UpdateContactBankDetails(ConnectionString, contact.referralbankdetailslist, ContactRefid, trans));
                    }
                }

                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
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
            contactid = contact.pReferenceId;
            return isSaved;
        }
        #endregion

        #region ViewContact
        public ContactMasterNewDTO ViewContact(string refernceid, string ConnectionString)
        {
            string addressdetails = string.Empty;


            ContactMasterNewDTO obj = new ContactMasterNewDTO();
            ds = new DataSet();

            try
            {

                obj.pAddressList = new List<contactAddressNewDTO>();
                obj.pEmailidsList = new List<EmailidsNewDTO>();
                obj.pbusinessList = new List<ContactBunsineePersonDto>();
                ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select * from tblmstcontact where upper(contactreferenceid)='" + refernceid.ToUpper() + "';select * from tblmstcontactaddressdetails where statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE') and upper(contactreferenceid)='" + refernceid.ToUpper() + "';select * from tblmstcontactpersondetails where statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE') and upper(contactreferenceid)='" + refernceid.ToUpper() + "'");
                if (ds != null)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        obj.pContactId = Convert.ToInt64(dr["contactid"]);

                        obj.pContactType = Convert.ToString(dr["contacttype"]);
                        obj.pName = Convert.ToString(dr["name"]);
                        obj.pSurName = Convert.ToString(dr["surname"]);
                        obj.pContactType = Convert.ToString(dr["contacttype"]);
                        obj.pTitleName = Convert.ToString(dr["titlename"]);
                        obj.pName = Convert.ToString(dr["name"]);
                        obj.pSurName = Convert.ToString(dr["surname"]);
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["dob"])))
                            obj.pDob = Convert.ToDateTime(dr["dob"]).ToString("dd-MM-yyyy");
                        obj.pGender = Convert.ToString(dr["gender"]);
                        obj.pFatherName = Convert.ToString(dr["fathername"]);
                        obj.pSpouseName = Convert.ToString(dr["spousename"]);
                        obj.pEnterpriseType = Convert.ToString(dr["typeofenterprise"]);
                        obj.pBusinesstype = Convert.ToString(dr["NatureofBusiness"]);
                        obj.pBusinessEntityEmailid = Convert.ToString(dr["businessentityemailid"]);
                        obj.pBusinessEntityContactno = Convert.ToString(dr["businessentitycontactno"]);

                        obj.pEmailId2 = dr["secondary_emailid"];

                        obj.pAlternativeNo = dr["secondary_contact_number"];
                        obj.pContactName = Convert.ToString(dr["contact_mailing_name"]);
                        obj.pContactimagepath = dr["contactimagepath"].ToString();

                    }

                    obj.pcontactexistingstatus = Convert.ToBoolean(
                        NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT getcontactexistingdata(" + obj.pContactId + ");"));

                    foreach (DataRow dr1 in ds.Tables[1].Rows)
                    {
                        addressdetails = string.Empty;
                        if (dr1["Address1"].ToString() != "" && dr1["Address1"].ToString() != null && dr1["Address1"].ToString() != string.Empty)
                        {
                            addressdetails = addressdetails + dr1["Address1"].ToString();
                        }
                        if (dr1["Address2"].ToString() != "" && dr1["Address2"].ToString() != null && dr1["Address2"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["Address2"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr1["Address2"].ToString();
                            }
                        }
                        if (dr1["City"].ToString() != "" && dr1["City"].ToString() != null && dr1["City"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["City"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr1["City"].ToString();
                            }
                        }
                        if (dr1["District"].ToString() != "" && dr1["District"].ToString() != null && dr1["District"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["District"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr1["District"].ToString();
                            }
                        }
                        if (dr1["State"].ToString() != "" && dr1["State"].ToString() != null && dr1["State"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["State"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr1["State"].ToString();
                            }
                        }
                        if (dr1["Country"].ToString() != "" && dr1["Country"].ToString() != null && dr1["Country"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["Country"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr1["Country"].ToString();
                            }
                        }
                        if (dr1["PinCode"].ToString() != "" && dr1["PinCode"].ToString() != null && dr1["PinCode"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["PinCode"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "-" + Convert.ToInt64(dr1["PinCode"]);
                            }
                        }
                        if (Convert.ToString(dr1["longitude"]) != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["longitude"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "-" + dr1["longitude"].ToString();
                            }
                        }
                        if (Convert.ToString(dr1["latitude"]) != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr1["latitude"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "-" + dr1["latitude"].ToString();
                            }
                        }
                        obj.pAddressList.Add(new contactAddressNewDTO
                        {
                            pRecordId = Convert.ToInt64(dr1["recordid"]),
                            pAddressType = dr1["AddressType"].ToString(),
                            pAddress1 = dr1["Address1"].ToString(),
                            pAddress2 = dr1["Address2"].ToString(),
                            pState = dr1["State"].ToString(),
                            pStateId = Convert.ToInt64(dr1["stateid"]),
                            pDistrict = dr1["District"].ToString(),
                            pDistrictId = Convert.ToInt64(dr1["districtid"]),
                            pCity = dr1["City"].ToString(),
                            pCountry = dr1["Country"].ToString(),
                            pCountryId = Convert.ToInt64(dr1["countryid"]),
                            pPinCode = Convert.ToInt64(dr1["PinCode"]),
                            pPriorityCon = Convert.ToBoolean(dr1["isprimary"]),
                            pAddressPriority = dr1["priority"],
                            plongitude = dr1["longitude"],
                            platitude = dr1["latitude"],
                            pAddressDetails = addressdetails,
                            ptypeofoperation = "OLD"
                        });
                    }
                    obj.pbusinessList = getbusinesscontactpersons(ConnectionString, obj.pContactId);
                    ReferralAdvocateDAL _ReferralAdvocateDAL = new ReferralAdvocateDAL();

                    obj.documentstorelist = getDocumentstoreDetails(ConnectionString, obj.pContactId, "");
                    //obj.documentstorelist = _ReferralAdvocateDAL.GetContactExistingKycDetails(ConnectionString, obj.pContactId);
                    obj.referralbankdetailslist = getReferralbankDetails(ConnectionString, obj.pContactId);
                    //foreach (DataRow dr2 in ds.Tables[2].Rows)
                    //{
                    //    obj.pEmailidsList.Add(new EmailidsDTO
                    //    {
                    //        pRecordId = Convert.ToInt64(dr2["recordid"]),
                    //        pContactName = dr2["contactname"].ToString(),
                    //        pContactNumber = dr2["contactnumber"].ToString(),
                    //        pEmailId = dr2["emailid"].ToString(),
                    //        pPriority = dr2["Priority"].ToString()
                    //    });
                    //}
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return obj;
        }
        public List<ContactBunsineePersonDto> getbusinesscontactpersons(string ConnectionString, long refernceid)
        {
            List<ContactBunsineePersonDto> lstContactBunsineePersonDto = new List<ContactBunsineePersonDto>();
            try
            {
                DataSet ds = new DataSet();
                ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "SELECT t1.tbl_mst_contact_business_entity_id,t1.contact_id,t1.business_entity_contact_id,COALESCE((select designation_name from tbl_mst_designation where tbl_mst_designation_id=t1.desigination_id), '') designation_name,t1.desigination_id,t1.status,t1.isprimary,t2.name as contact_name FROM tbl_mst_contact_business_entity t1 left join tblmstcontact t2 on t1.business_entity_contact_id=t2.contactid  where t1.contact_id=" + refernceid + " and t1.status='true';");
                if (ds != null)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ContactBunsineePersonDto ContactBunsineePersonDto = new ContactBunsineePersonDto();
                        ContactBunsineePersonDto.precordid = dr["tbl_mst_contact_business_entity_id"];
                        ContactBunsineePersonDto.pContactId = dr["business_entity_contact_id"];
                        ContactBunsineePersonDto.pContactName = dr["contact_name"];
                        ContactBunsineePersonDto.designationid = dr["desigination_id"];
                        ContactBunsineePersonDto.designationname = dr["designation_name"];
                        ContactBunsineePersonDto.pBusinessPriority = dr["isprimary"];
                        ContactBunsineePersonDto.pstatus = dr["status"];
                        ContactBunsineePersonDto.ptypeofoperation = "OLD";
                        lstContactBunsineePersonDto.Add(ContactBunsineePersonDto);
                    }
                }

            }
            catch (Exception ex)
            {

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
            return lstContactBunsineePersonDto;
        }
        public List<referralbankdetailsDTO> getReferralbankDetails(string ConnectionString, object contactid)

        {
            List<referralbankdetailsDTO> banklist = new List<referralbankdetailsDTO>();


            try
            {
                string _query = "";

                _query = "select tbl_mst_contact_bank_id, bank_id, bank_branch_name, bank_account_number, bank_ifsc_code, isprimary  from tbl_mst_contact_bank where status=true and contact_id=" + contactid;

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, _query))
                {
                    while (dr.Read())
                    {
                        referralbankdetailsDTO _referralbankdetailsDTO = new referralbankdetailsDTO();

                        _referralbankdetailsDTO.pBankBranch = Convert.ToString(dr["bank_branch_name"]);
                        _referralbankdetailsDTO.pBankId = dr["bank_id"];
                        if (!string.IsNullOrEmpty(Convert.ToString(_referralbankdetailsDTO.pBankId)))
                            _referralbankdetailsDTO.pBankName = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select bank_name as bank_name from bank_names  where bank_id=" + _referralbankdetailsDTO.pBankId));
                        else
                            _referralbankdetailsDTO.pBankName = "";
                        _referralbankdetailsDTO.pBankAccountNo = Convert.ToString(dr["bank_account_number"]);
                        _referralbankdetailsDTO.pBankifscCode = Convert.ToString(dr["bank_ifsc_code"]);
                        _referralbankdetailsDTO.pIsprimaryAccount = Convert.ToBoolean(dr["isprimary"].ToString());
                        _referralbankdetailsDTO.precordid = dr["tbl_mst_contact_bank_id"];
                        _referralbankdetailsDTO.ptypeofoperation = "OLD";
                        banklist.Add(_referralbankdetailsDTO);

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }


            return banklist;
        }
        #endregion

        #region GetContactDetails
        public List<ContactMasterNewDTO> GetContactdetails(string ConnectionString, string Type)
        {
            string Query = string.Empty;
            List<ContactMasterNewDTO> lstcontactdetails = new List<ContactMasterNewDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL" || string.IsNullOrEmpty(Type))
                {
                    Query = "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,businessentityemailid , businessentitycontactno from tblmstcontact tc  ) x left join tblmststatus y on x.statusid=y.statusid order by x.contactid desc;";
                }
                else
                {
                    Query = "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,businessentityemailid , businessentitycontactno from tblmstcontact tc  ) x left join tblmststatus y on x.statusid = y.statusid where upper(x.name) like'%" + Type + "%' or upper(x.surname) like '%" + Type + "%' or upper(x.contactreferenceid) like '%" + Type + "%' or upper(x.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%'; ";
                }
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,tca.contactname,tca.contactnumber,tca.emailid,tca.priority from tblmstcontact tc  join TBLMSTCONTACTPERSONDETAILS tca on tc.contactreferenceid=tca.contactreferenceid where upper(tca.priority)='PRIMARY') x left join tblmststatus y on x.statusid=y.statusid"))
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ContactMasterNewDTO obj = new ContactMasterNewDTO();
                        obj.pContactId = Convert.ToInt64(dr["contactid"]);
                        obj.pReferenceId = dr["contactreferenceid"].ToString();
                        obj.pContactType = dr["contacttype"].ToString();
                        obj.pStatusid = dr["statusid"].ToString();
                        obj.pStatusname = dr["statusname"].ToString();
                        obj.pName = dr["name"].ToString() + " " + dr["surname"].ToString();

                        obj.pSurName = dr["surname"].ToString();
                        obj.pBusinessEntityEmailid = dr["businessentityemailid"].ToString();
                        obj.pBusinessEntityContactno = dr["businessentitycontactno"].ToString();
                        //obj.pEmailidsList = new List<EmailidsDTO>();
                        //obj.pEmailidsList.Add(new EmailidsDTO
                        //{
                        //    pContactNumber = dr["contactnumber"].ToString()
                        //,
                        //    pEmailId = dr["emailid"].ToString()
                        //});

                        lstcontactdetails.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstcontactdetails;
        }
        public List<SubscriberContactDTO> GetSubContactdetails(string ConnectionString, string Type)
        {
            string Query = string.Empty;
            List<SubscriberContactDTO> lstcontactdetails = new List<SubscriberContactDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL" || string.IsNullOrEmpty(Type))
                {
                    Query = "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,businessentityemailid , businessentitycontactno ,trim(contact_mailing_name) as contact_name from tblmstcontact tc  ) x left join tblmststatus y on x.statusid=y.statusid order by x.contactid desc;";
                }
                else
                {
                    Query = "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,businessentityemailid , businessentitycontactno ,trim(contact_mailing_name) as contact_name from tblmstcontact tc  ) x left join tblmststatus y on x.statusid = y.statusid where upper(x.contact_name) like '%" + Type + "%' or upper(x.contactreferenceid) like '%" + Type + "%' or upper(x.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%'; ";
                }
                //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,tca.contactname,tca.contactnumber,tca.emailid,tca.priority from tblmstcontact tc  join TBLMSTCONTACTPERSONDETAILS tca on tc.contactreferenceid=tca.contactreferenceid where upper(tca.priority)='PRIMARY') x left join tblmststatus y on x.statusid=y.statusid"))
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        SubscriberContactDTO obj = new SubscriberContactDTO();
                        obj.contactid = Convert.ToInt64(dr["contactid"]);
                        obj.contactreferenceid = dr["contactreferenceid"].ToString();
                        obj.contacttype = dr["contacttype"].ToString();
                        //obj.pStatusid = dr["statusid"].ToString();
                        //obj.pStatusname = dr["statusname"].ToString();
                        // obj.pName = dr["name"].ToString() + " " + dr["surname"].ToString();

                        obj.contactname = dr["contact_name"].ToString();
                        obj.contactemailid = dr["businessentityemailid"].ToString();
                        obj.contactmobilenumber = dr["businessentitycontactno"].ToString();
                        //obj.pEmailidsList = new List<EmailidsDTO>();
                        //obj.pEmailidsList.Add(new EmailidsDTO
                        //{
                        //    pContactNumber = dr["contactnumber"].ToString()
                        //,
                        //    pEmailId = dr["emailid"].ToString()
                        //});

                        lstcontactdetails.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstcontactdetails;
        }
        public List<ContactMasterNewDTO> GetContactdetailsByMobileNo(string ConnectionString, string pMobileNo)
        {
            string Query = string.Empty;
            List<ContactMasterNewDTO> lstcontactdetails = new List<ContactMasterNewDTO>();
            try
            {

                Query = "select x.*,y.statusname from (select tc.contacttype,tc.contactid,tc.contactreferenceid,tc.statusid, businessentitycontactno from tblmstcontact tc  ) x left join tblmststatus y on x.statusid = y.statusid where businessentitycontactno='" + pMobileNo + "' order by x.contactid desc;";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        ContactMasterNewDTO obj = new ContactMasterNewDTO();
                        obj.pContactId = Convert.ToInt64(dr["contactid"]);
                        obj.pReferenceId = dr["contactreferenceid"].ToString();
                        obj.pContactType = dr["contacttype"].ToString();
                        obj.pStatusid = dr["statusid"].ToString();
                        obj.pStatusname = dr["statusname"].ToString();
                        obj.pBusinessEntityContactno = dr["businessentitycontactno"].ToString();
                        lstcontactdetails.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstcontactdetails;
        }
        #endregion

        public List<DesignationDTO> GetDesignations(string ConnectionString)
        {
            string Query = string.Empty;
            List<DesignationDTO> _lstDesignation = new List<DesignationDTO>();
            try
            {

                Query = "select tbl_mst_designation_id,designation_name from tbl_mst_designation  where status=true ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        _lstDesignation.Add(new DesignationDTO
                        {
                            designationid = dr["tbl_mst_designation_id"],
                            designationname = dr["designation_name"],
                        });

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return _lstDesignation;
        }

        #region UpdateContact
        public bool UpdateContact(ContactMasterNewDTO contact, out string contactId, string ConnectionString)
        {
            bool isSaved = false;
            long ContactRefid;
            string Recordid = string.Empty;
            contactId = string.Empty;
            string RelativeRecordid = string.Empty;
            StringBuilder sbupdate = new StringBuilder();
            StringBuilder query1 = new StringBuilder();
            string query = "";
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (string.IsNullOrEmpty(contact.pDob))
                {

                    contact.pDob = "null";
                }
                else
                {

                    contact.pDob = "'" + FormatDate(contact.pDob) + "'";
                }
                if (Convert.ToString(contact.pAlternativeNo) != string.Empty)
                {
                    contact.pAlternativeNo = contact.pAlternativeNo;
                }
                else
                {
                    contact.pAlternativeNo = "null";
                }
                ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select CONTACTID from tblmstcontact  where contactreferenceid='" + contact.pReferenceId + "';"));
                if (contact.pContactType == "Individual")
                {
                    sbupdate.Append("update tblmstcontact set name='" + ManageQuote(contact.pName) + "',surname='" + ManageQuote(contact.pSurName) + "',dob=" + (contact.pDob) + ",gender='" + ManageQuote(contact.pGender) + "',gendercode='" + contact.pGenderCode + "',fathername='" + ManageQuote(contact.pFatherName) + "',spousename='" + ManageQuote(contact.pSpouseName) + "',businessentityemailid ='" + ManageQuote(contact.pBusinessEntityEmailid) + "' , businessentitycontactno='" + ManageQuote(contact.pBusinessEntityContactno) + "',contactimagepath='" + ManageQuote(contact.pContactimagepath) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp,secondary_contact_number=" + contact.pAlternativeNo + ",contact_mailing_name='" + ManageQuote(contact.pContactName) + "',secondary_emailid='" + contact.pEmailId2 + "' where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';");
                }
                else
                {
                    sbupdate.Append("update tblmstcontact set name='" + ManageQuote(contact.pName) + "',typeofenterprise='" + ManageQuote(contact.pEnterpriseType) + "',natureofbusiness='" + ManageQuote(contact.pBusinesstype) + "',businessentityemailid ='" + ManageQuote(contact.pBusinessEntityEmailid) + "' , businessentitycontactno='" + ManageQuote(contact.pBusinessEntityContactno) + "',contactimagepath='" + ManageQuote(contact.pContactimagepath) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp,secondary_contact_number=" + contact.pAlternativeNo + ",contact_mailing_name='" + ManageQuote(contact.pContactName) + "',secondary_emailid='" + contact.pEmailId2 + "' where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';");

                    if (contact.pbusinessList.Count > 0)
                    {
                        for (int i = 0; i < contact.pbusinessList.Count; i++)
                        {
                            if (Convert.ToString(contact.pbusinessList[i].ptypeofoperation) == "DELETE")
                            {
                                if (string.IsNullOrEmpty(RelativeRecordid))
                                {
                                    RelativeRecordid = contact.pbusinessList[i].precordid.ToString();
                                }
                                else
                                {
                                    RelativeRecordid = RelativeRecordid + "," + contact.pbusinessList[i].precordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(contact.pbusinessList[i].pContactId.ToString()))
                            {

                                contact.pbusinessList[i].pContactId = "null";
                            }
                            if (Convert.ToString(contact.pbusinessList[i].ptypeofoperation) == "CREATE")
                            {
                                sbupdate.Append("insert into tbl_mst_contact_business_entity(contact_id,business_entity_contact_id,desigination_id,isprimary,status) values (" + ContactRefid + "," + contact.pbusinessList[i].pContactId + "," + contact.pbusinessList[i].designationid + "," + contact.pbusinessList[i].pBusinessPriority + ",true);");
                            }
                            else if (Convert.ToString(contact.pbusinessList[i].ptypeofoperation) == "UPDATE")
                            {
                                sbupdate.Append("UPDATE tbl_mst_contact_business_entity SET business_entity_contact_id=" + contact.pbusinessList[i].pContactId + ",desigination_id=" + contact.pbusinessList[i].designationid + ", isprimary = " + contact.pbusinessList[i].pBusinessPriority + " WHERE tbl_mst_contact_business_entity_id=" + contact.pbusinessList[i].precordid + " AND contact_id=" + ContactRefid + ";");
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(RelativeRecordid))
                    {

                        query1.Append("update tbl_mst_contact_business_entity set status=false where contact_id=" + ContactRefid + "  and tbl_mst_contact_business_entity_id in(" + RelativeRecordid + ");");

                    }
                    else
                    {
                        if (contact.pbusinessList.Count == 0)
                        {
                            query1.Append("update tbl_mst_contact_business_entity set status=false where contact_id=" + ContactRefid + ";");

                        }
                    }
                }
                
                if (contact.pEmailidsList != null)
                {
                    for (var i = 0; i < contact.pEmailidsList.Count; i++)
                    {

                        sbupdate.Append("update TBLMSTCONTACTPERSONDETAILS set contactname='" + ManageQuote(contact.pContactName) + "',contactnumber=" + contact.pEmailidsList[i].pContactNumber + ",emailid='" + ManageQuote(contact.pEmailidsList[i].pEmailId) + "',priority='" + ManageQuote(contact.pEmailidsList[i].pPriority) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp   where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "' and recordid=" + contact.pEmailidsList[i].pRecordId + ";");
                    }
                }
                if (contact.pAddressList != null)
                {

                    for (int i = 0; i < contact.pAddressList.Count; i++)
                    {
                        if (contact.pAddressList[i].ptypeofoperation == "DELETE")
                        {
                            if (string.IsNullOrEmpty(Recordid))
                            {
                                Recordid = contact.pAddressList[i].pRecordId.ToString();
                            }
                            else
                            {
                                Recordid = Recordid + "," + contact.pAddressList[i].pRecordId.ToString();
                            }
                        }
                        if (contact.pAddressList[i].ptypeofoperation == "UPDATE")
                        {

                            sbupdate.Append("update tblmstcontactaddressdetails set addresstype='" + ManageQuote(contact.pAddressList[i].pAddressType) + "', address1='" + ManageQuote(contact.pAddressList[i].pAddress1) + "',address2='" + ManageQuote(contact.pAddressList[i].pAddress2) + "',state='" + ManageQuote(contact.pAddressList[i].pState) + "',stateid=" + contact.pAddressList[i].pStateId + ",district='" + ManageQuote(contact.pAddressList[i].pDistrict) + "',districtid=" + contact.pAddressList[i].pDistrictId + ",city='" + ManageQuote(contact.pAddressList[i].pCity) + "',country='" + ManageQuote(contact.pAddressList[i].pCountry) + "',countryid=" + contact.pAddressList[i].pCountryId + ",pincode=" + contact.pAddressList[i].pPinCode + ",priority='" + (contact.pAddressList[i].pPriority) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp,longitude='" + contact.pAddressList[i].plongitude + "',latitude='" + contact.pAddressList[i].platitude + "',isprimary=" + contact.pAddressList[i].pPriorityCon + " where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "'  and recordid=" + contact.pAddressList[i].pRecordId + ";");
                        }
                        else if (contact.pAddressList[i].ptypeofoperation == "CREATE")
                        {
                            sbupdate.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate,longitude,latitude,isprimary) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + ManageQuote(contact.pAddressList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp,'" + contact.pAddressList[i].plongitude + "','" + contact.pAddressList[i].platitude + "'," + (contact.pAddressList[i].pPriorityCon) + ");");
                        }
                        //else if (contact.pAddressList[i].ptypeofoperation == "DELETE")
                        //{
                        //    sbupdate.Append("update tblmstcontactaddressdetails set statusid=" + getStatusid(contact.pStatusname, ConnectionString) + ",modifiedby=" + contact.pAddressList[i].pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper()+ "' and upper(addresstype)='" + contact.pAddressList[i].pAddressType.ToUpper()+ "' and recordid=" + contact.pAddressList[i].pRecordId + ";");
                        //}
                    }
                }

                if (!string.IsNullOrEmpty(Recordid))
                {
                    query = "update tblmstcontactaddressdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "'  and recordid in(" + Recordid + ");";
                }
                //else
                //{
                //    query = "update tblmstcontactaddressdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';";
                //}
                ReferralAdvocateDAL _ReferralAdvocateDAL = new ReferralAdvocateDAL();
                if (contact.documentstorelist != null)
                {
                    if (contact.documentstorelist.Count > 0)
                    {
                       
                        sbupdate.Append(_ReferralAdvocateDAL.UpdateStoreDetails(contact.documentstorelist, ConnectionString, 0, contact.pContactId));
                        int referralcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tbl_mst_referral where contact_id=" + ContactRefid + ""));
                        if (referralcount > 0)
                        {
                            int getid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentgroupid,0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%';"));
                            int DocumentId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentid,0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%';"));
                            for (int i = 0; i < contact.documentstorelist.Count; i++)
                            {
                                if (contact.documentstorelist[i].pDocumentGroupId == getid && contact.documentstorelist[i].pDocumentId == DocumentId)
                                {
                                    sbupdate.Append("UPDATE tbl_mst_referral set pan_number='" + contact.documentstorelist[i].pDocReferenceno + "'  where contact_id=" + ContactRefid + ";");
                                }
                            }
                        }
                    }
                    else
                    {
                        sbupdate.Append("UPDATE tblmstdocumentstore set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where contactid=" + contact.pContactId + " AND statusid<>2;");

                    }
                }

                //store Contact Bank details
                if (contact.referralbankdetailslist != null)
                {
                    if (contact.referralbankdetailslist.Count > 0)
                    {
                        sbupdate.Append(_ReferralAdvocateDAL.UpdateContactBankDetails(ConnectionString, contact.referralbankdetailslist, ContactRefid, trans));

                    }
                    else
                    {
                        sbupdate.Append("UPDATE tbl_mst_contact_bank set status='false' where contact_id=" + ContactRefid + " AND status<>'false';");

                    }
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query + "" + sbupdate.ToString());
                if (!string.IsNullOrEmpty(query1.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query1.ToString());
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
            contactId = contact.pReferenceId;
            return isSaved;
        }
        #endregion

        #region DeleteContact
        public bool DeleteContact(ContactMasterNewDTO contact, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update tblmstcontact set statusid=" + getStatusid(contact.pStatusname, ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';");
                isSaved = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isSaved;
        }
        #endregion

        #region ContactAddressTypes       
        public bool SaveAddressType(contactAddressNewDTO addresstype, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstaddresstypes(contacttype,addresstype,statusid,createdby,createddate)values('" + ManageQuote(addresstype.pContactType) + "','" + ManageQuote(addresstype.pAddressType.Trim()) + "'," + getStatusid(addresstype.pStatusname, ConnectionString) + "," + addresstype.pCreatedby + ",current_timestamp);");
                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;
        }

        public List<contactAddressNewDTO> GetAddressType(string contactype, string ConnectionString)
        {
            lstAddressDetails = new List<contactAddressNewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ADDRESSTYPE from TBLMSTADDRESSTYPES where upper(contacttype)='" + contactype.ToUpper() + "' order by ADDRESSTYPE"))
                {
                    while (dr.Read())
                    {
                        contactAddressNewDTO objaddressdetails = new contactAddressNewDTO();
                        objaddressdetails.pAddressType = dr["ADDRESSTYPE"].ToString();
                        lstAddressDetails.Add(objaddressdetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstAddressDetails;

        }

        public int checkInsertAddressTypeDuplicates(string addresstype, string contactype, string connectionstring)
        {
            int count = -1;
            try
            {

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from TBLMSTADDRESSTYPES where upper(ADDRESSTYPE)='" + addresstype.ToUpper() + "' and  upper(contacttype)='" + contactype.ToUpper() + "'"));

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }

        #endregion

        #region ContactEnterpriseTypes  

        public List<EnterpriseTypeNewDTO> GetEnterpriseType(string ConnectionString)
        {

            lstEnterpriseType = new List<EnterpriseTypeNewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select enterprisetype from tblmstenterprisetypes order by enterprisetype"))
                {
                    while (dr.Read())
                    {
                        EnterpriseTypeNewDTO objenterprise = new EnterpriseTypeNewDTO();
                        objenterprise.pEnterpriseType = dr["enterprisetype"].ToString();
                        lstEnterpriseType.Add(objenterprise);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstEnterpriseType;

        }

        public bool SaveEnterpriseType(EnterpriseTypeNewDTO Enterprisetype, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstenterprisetypes(enterprisetype,statusid,createdby,createddate)values('" + ManageQuote(Enterprisetype.pEnterpriseType.Trim()) + "',1,14,current_timestamp);");

                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;
        }

        public int checkInsertEnterpriseTypeDuplicates(string enterprisetype, string connectionstring)
        {
            int count = -1;
            try
            {

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstenterprisetypes where upper(enterprisetype)='" + enterprisetype.ToUpper() + "'"));

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }
        #endregion

        #region ContactBusinessTypes       

        public List<BusinessTypeNewDTO> GetBusinessTypes(string ConnectionString)
        {
            lstBusinessType = new List<BusinessTypeNewDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select businesstype from tblmstbusinesstypes order by businesstype"))
                {
                    while (dr.Read())
                    {
                        BusinessTypeNewDTO objbusinesstype = new BusinessTypeNewDTO();
                        objbusinesstype.pBusinesstype = dr["businesstype"].ToString();
                        lstBusinessType.Add(objbusinesstype);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstBusinessType;

        }

        public bool SaveBusinessTypes(BusinessTypeNewDTO Businesstype, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstbusinesstypes(businesstype,statusid,createdby,createddate)values('" + ManageQuote(Businesstype.pBusinesstype.Trim()) + "',1, 14,current_timestamp);");

                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;
        }

        public int checkInsertBusinessTypesDuplicates(string businesstype, string connectionstring)
        {
            int count = -1;
            try
            {

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstbusinesstypes where upper(businesstype)='" + businesstype.ToUpper() + "'"));

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }

        #endregion

        #region Personcount

        public int GetPersoncount(ContactMasterNewDTO ContactDto, string ConnectionString)
        {
            int count = -1;
            try
            {

                if (string.IsNullOrEmpty(ContactDto.pReferenceId))
                {
                    ContactDto.pReferenceId = "";
                }
                else
                {
                    ContactDto.pReferenceId = ContactDto.pReferenceId.ToUpper();
                }

                //count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstcontact x where upper(contactreferenceid)<>'" + ContactDto.pReferenceId + "' and upper(x.name)='" + ManageQuote(ContactDto.pName.ToUpper()) + "' and upper(coalesce(x.surname,''))='" + ManageQuote(ContactDto.pSurName.ToUpper()) + "' and x.businessentitycontactno=" + ContactDto.pBusinessEntityContactno + ";"));
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstcontact x where upper(contactreferenceid)<>'" + ContactDto.pReferenceId + "' and x.businessentitycontactno=" + ContactDto.pBusinessEntityContactno + ";"));
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public List<ContactViewNewDTO> GetContactView(string ViewName, string endindex, string searchby, string ConnectionString)
        {

            string Query = string.Empty;
            string addressdetails = string.Empty;
            lstContactViewDTO = new List<ContactViewNewDTO>();
            string search = "";
            if (searchby != null)
            {
                search = searchby.ToUpper().Trim();
            }
            else
            {
                search = searchby;
            }
            try
            {
                if (ViewName.ToUpper().Trim() == "CONTACTS")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' order by contactid desc limit 10 offset " + endindex + ";";
                }
                if (ViewName.ToUpper().Trim() == "APPLICANTS")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactreferenceid in(select distinct contactreferenceid from tabapplication) limit 10 offset " + endindex + ";";
                }
                if (ViewName.ToUpper().Trim() == "PARTIES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstparty) order by contactid desc limit 10 offset " + endindex + ";";
                }
                if (ViewName.ToUpper().Trim() == "EMPLOYEES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstemployee) order by contactid desc limit 10 offset " + endindex + ";";
                }
                if (ViewName.ToUpper().Trim() == "REFERRALS")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstreferral) order by contactid desc limit 10 offset " + endindex + ";";
                }
                if (ViewName.ToUpper().Trim() == "ADVOCATES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid from tblmstadvocate) order by contactid desc limit 10 offset " + endindex + ";";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        addressdetails = string.Empty;
                        ContactViewNewDTO ContactViewDTO = new ContactViewNewDTO();
                        ContactViewDTO.pContactdId = Convert.ToInt64(dr["contactid"]);
                        ContactViewDTO.pContactType = dr["contacttype"].ToString();
                        ContactViewDTO.pContactName = dr["fullname"].ToString();
                        ContactViewDTO.pRefNo = dr["contactreferenceid"].ToString();
                        ContactViewDTO.pFatherName = dr["fathername"].ToString();
                        ContactViewDTO.pContactNumber = dr["businessentitycontactno"].ToString();
                        ContactViewDTO.pContactEmail = dr["businessentityemailid"].ToString();
                        ContactViewDTO.pImagePath = dr["contactimagepath"].ToString();
                        ContactViewDTO.pGender = dr["gender"];


                        if (dr["address1"].ToString() != "" && dr["address1"].ToString() != null && dr["address1"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["address1"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["address1"].ToString();
                            }
                        }
                        if (dr["address2"].ToString() != "" && dr["address2"].ToString() != null && dr["address2"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["address2"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["address2"].ToString();
                            }
                        }
                        if (dr["state"].ToString() != "" && dr["state"].ToString() != null && dr["state"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["state"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["state"].ToString();
                            }
                        }
                        if (dr["district"].ToString() != "" && dr["district"].ToString() != null && dr["district"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["district"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["district"].ToString();
                            }
                        }
                        if (dr["city"].ToString() != "" && dr["city"].ToString() != null && dr["city"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["city"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["city"].ToString();
                            }
                        }
                        if (dr["country"].ToString() != "" && dr["country"].ToString() != null && dr["country"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["country"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["country"].ToString();
                            }
                        }
                        if (dr["pincode"].ToString() != "" && dr["pincode"].ToString() != null && dr["pincode"].ToString() != string.Empty && dr["pincode"].ToString() != "0")
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["pincode"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "-" + Convert.ToInt64(dr["pincode"]);
                            }
                        }
                        ContactViewDTO.pAddresDetails = addressdetails;
                        lstContactViewDTO.Add(ContactViewDTO);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return lstContactViewDTO;
        }

        public List<ContactViewNewDTO> GetContactViewdata(string ViewName, string ConnectionString)
        {

            string Query = string.Empty;
            string addressdetails = string.Empty;
            lstContactViewDTO = new List<ContactViewNewDTO>();
            
            try
            {
                if (ViewName.ToUpper().Trim() == "CONTACTS")
                {
                    Query = "SELECT * FROM vwcontactdataview order by contactid desc;";
                }
                if (ViewName.ToUpper().Trim() == "APPLICANTS")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactreferenceid in(select distinct contactreferenceid from tabapplication);";
                }
                if (ViewName.ToUpper().Trim() == "PARTIES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstparty) order by contactid desc;";
                }
                if (ViewName.ToUpper().Trim() == "EMPLOYEES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstemployee) order by contactid desc;";
                }
                if (ViewName.ToUpper().Trim() == "REFERRALS")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstreferral) order by contactid desc;";
                }
                if (ViewName.ToUpper().Trim() == "ADVOCATES")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid from tblmstadvocate) order by contactid desc;";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        addressdetails = string.Empty;
                        ContactViewNewDTO ContactViewDTO = new ContactViewNewDTO();
                        ContactViewDTO.pContactdId = Convert.ToInt64(dr["contactid"]);
                        ContactViewDTO.pContactType = dr["contacttype"].ToString();
                        ContactViewDTO.pContactName = dr["fullname"].ToString();
                        ContactViewDTO.pRefNo = dr["contactreferenceid"].ToString();
                        ContactViewDTO.pFatherName = dr["fathername"].ToString();
                        ContactViewDTO.pContactNumber = dr["businessentitycontactno"].ToString();
                        ContactViewDTO.pContactEmail = dr["businessentityemailid"].ToString();
                        ContactViewDTO.pImagePath = dr["contactimagepath"].ToString();
                        ContactViewDTO.pGender = dr["gender"];


                        if (dr["address1"].ToString() != "" && dr["address1"].ToString() != null && dr["address1"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["address1"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["address1"].ToString();
                            }
                        }
                        if (dr["address2"].ToString() != "" && dr["address2"].ToString() != null && dr["address2"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["address2"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["address2"].ToString();
                            }
                        }
                        if (dr["state"].ToString() != "" && dr["state"].ToString() != null && dr["state"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["state"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["state"].ToString();
                            }
                        }
                        if (dr["district"].ToString() != "" && dr["district"].ToString() != null && dr["district"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["district"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["district"].ToString();
                            }
                        }
                        if (dr["city"].ToString() != "" && dr["city"].ToString() != null && dr["city"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["city"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["city"].ToString();
                            }
                        }
                        if (dr["country"].ToString() != "" && dr["country"].ToString() != null && dr["country"].ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["country"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "," + dr["country"].ToString();
                            }
                        }
                        if (dr["pincode"].ToString() != "" && dr["pincode"].ToString() != null && dr["pincode"].ToString() != string.Empty && dr["pincode"].ToString() != "0")
                        {
                            if (string.IsNullOrEmpty(addressdetails))
                            {
                                addressdetails = dr["pincode"].ToString();
                            }
                            else
                            {
                                addressdetails = addressdetails + "-" + Convert.ToInt64(dr["pincode"]);
                            }
                        }
                        ContactViewDTO.pAddresDetails = addressdetails;
                        lstContactViewDTO.Add(ContactViewDTO);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return lstContactViewDTO;
        }
        public int GetContactCount(string ViewName, string searchby, string ConnectionString)
        {
            string Query = string.Empty;
            int count = -1;
            string search = "";
            if (searchby != null)
            {
                search = searchby.ToUpper().Trim();
            }
            else
            {
                search = searchby;
            }
            try
            {
                if (ViewName.ToUpper().Trim() == "CONTACTS")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%';";
                }
                if (ViewName.ToUpper().Trim() == "APPLICANTS")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactreferenceid in(select distinct contactreferenceid from tabapplication);";
                }
                if (ViewName.ToUpper().Trim() == "PARTIES")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstparty);";
                }
                if (ViewName.ToUpper().Trim() == "EMPLOYEES")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstemployee);";
                }
                if (ViewName.ToUpper().Trim() == "REFERRALS")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid  from tblmstreferral);";
                }
                if (ViewName.ToUpper().Trim() == "ADVOCATES")
                {
                    Query = "SELECT count(*) FROM vwcontactdataview WHERE upper(fullname) like '%" + search + "%' and contactid in(select distinct contactid from tblmstadvocate);";
                }

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, Query));
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }
        #endregion

        public ContactViewNewDTO GetContactViewbyid(string ConnectionString, string refid)
        {
            long referralcount = 0;
            long Employeecount = 0;
            long contactId = 0;
            string Query = string.Empty;
            string addressdetails = string.Empty;
            ContactViewNewDTO lstContactViewDTO = new ContactViewNewDTO();

            try
            {
                Query = "select z.address1||','||z.address2||','||z.city||','||z.district||','||z.state||', '||z.pincode as contactaddress,contacttype,(case when contacttype='Individual' then contact_mailing_name else c.name end) as contact_name,COALESCE(surname,'') as contact_surname,gender,COALESCE(fathername,'') as fathername,c.contactid,c.contactreferenceid,COALESCE(businessentityemailid,'') as businessentityemailid,COALESCE(businessentitycontactno::text,'') as businessentitycontactno,COALESCE(contactimagepath,'') as contactimagepath,COALESCE(is_supplier_applicable,false) as  is_supplier_applicable,COALESCE(is_advocate_applicable,false) as  is_advocate_applicable from tblmstcontactaddressdetails z join tblmstcontact c on c.contactid=z.contactid where c.contactreferenceid='" + refid + "' and   z.isprimary='true' and c.statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');";

                //contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + refid + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));
                contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + refid + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));


                //Employeecount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select count(*) from tblmstemployee where contactid=" + contactId + " and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));
                Employeecount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select count(*) from tbl_mst_employee where contact_id=" + contactId + " and  status=true;"));
                //referralcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstreferral where contactid=" + contactId + " and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));
                referralcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tbl_mst_referral where contact_id=" + contactId + " and  status=true;"));
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {

                        lstContactViewDTO.pContactdId = dr["contactid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["contactid"]);
                        lstContactViewDTO.pContactType = Convert.ToString(dr["contacttype"]);
                        lstContactViewDTO.pContactName = Convert.ToString(dr["contact_name"]);
                        lstContactViewDTO.pRefNo = Convert.ToString(dr["contactreferenceid"]);
                        lstContactViewDTO.pFatherName = Convert.ToString(dr["fathername"]);
                        lstContactViewDTO.pContactNumber = Convert.ToString(dr["businessentitycontactno"]);
                        lstContactViewDTO.pContactEmail = Convert.ToString(dr["businessentityemailid"]);
                        lstContactViewDTO.pImagePath = Convert.ToString(dr["contactimagepath"]);
                        lstContactViewDTO.pAddresDetails = Convert.ToString(dr["contactaddress"]);
                        lstContactViewDTO.pissupplier = Convert.ToBoolean(dr["is_supplier_applicable"]);
                        lstContactViewDTO.pisadvocate = Convert.ToBoolean(dr["is_advocate_applicable"]);
                        lstContactViewDTO.pGender = Convert.ToString(dr["gender"]);
                        lstContactViewDTO.pContactimagepath = dr["contactimagepath"].ToString();
                        if (Employeecount > 0)
                        {
                            lstContactViewDTO.pisemployee = true;
                        }
                        else
                        {
                            lstContactViewDTO.pisemployee = false;

                        }
                        if (referralcount > 0)
                        {
                            lstContactViewDTO.pisreferral = true;
                        }
                        else
                        {
                            lstContactViewDTO.pisreferral = false;

                        }

                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstContactViewDTO;
        }
        public bool SaveContactEmployee(string ConnectionString, ContactEmployeeDTO EmployeeDTO)
        {
            bool isSaved = false;
            StringBuilder sb = new StringBuilder();
            StringBuilder query = new StringBuilder();
            string EmployeeRecordid = string.Empty;
            object formname = "EMPLOYEE";
            long EmployeeId;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                // long branchid = _commonDAL.getbranchId(ConnectionString, GlobalSchema, EmployeeDTO.samebranchcode);
                if (EmployeeDTO != null)
                {
                    if (Convert.ToString(EmployeeDTO.pEmploymentRoleId) == string.Empty)
                    {
                        EmployeeDTO.pEmploymentRoleId = "null";
                    }
                    if (Convert.ToString(EmployeeDTO.pdesignation) == string.Empty)
                    {
                        EmployeeDTO.pdesignation = "null";
                    }
                    //if (Convert.ToString(EmployeeDTO.pjoinedasid) == string.Empty)
                    //{
                    //    EmployeeDTO.pjoinedasid = "null";
                    //}

                    if (Convert.ToString(EmployeeDTO.pCountryId) == string.Empty)
                    {
                        EmployeeDTO.pCountryId = "null";
                    }
                    if (Convert.ToString(EmployeeDTO.pEmploymentJoiningDate) == string.Empty)
                    {

                        EmployeeDTO.pEmploymentJoiningDate = "null";
                    }
                    else
                    {
                        EmployeeDTO.pEmploymentJoiningDate = "'" + FormatDate(EmployeeDTO.pEmploymentJoiningDate.ToString()) + "'";
                    }
                    long contactId = 0;
                    contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + EmployeeDTO.pcontactid + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));

                    long count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select count(*) from tbl_mst_employee where contact_id=" + contactId + " and status='true';"));

                    if (count > 0)
                    {
                        EmployeeId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "Select tbl_mst_employee_id from tbl_mst_employee where contact_id=" + contactId + " and status='true';"));

                        sb.Append("update tbl_mst_employee set  basic_amount=" + EmployeeDTO.pEmploymentBasicSalary + ",allowance_amount=" + EmployeeDTO.pEmploymentAllowanceORvda + ",ctc_amount=" + EmployeeDTO.pEmploymentCTC + ",designation_id=" + EmployeeDTO.mdesignationid + ",role_id=" + EmployeeDTO.pEmploymentRoleId + ",date_of_joining=" + EmployeeDTO.pEmploymentJoiningDate + ",residential_status='" + EmployeeDTO.presidentialstatus + "', place_of_birth='" + EmployeeDTO.pplaceofbirth + "', country_id=" + EmployeeDTO.pCountryId + ", nationality='" + EmployeeDTO.pnationality + "', minority_community='" + EmployeeDTO.pminoritycommunity + "', martial_status='" + EmployeeDTO.pmaritalstatus + "',disciplinary_actions='" + EmployeeDTO.pdisciplinaryactions + "',extra_curricular_activities='" + EmployeeDTO.pextracurricularactivities + "' where tbl_mst_employee_id=" + EmployeeId + " and status='true';");
                    }
                    else
                    {
                        //EmployeeDTO.pemployeecode = objcommondal.GenerateNextID(trans,  Convert.ToString(formname), "1", Convert.ToString(System.DateTime.Now.Date));
                       // EmployeeDTO.pemployeecode = GenerateNextID(trans, "EMPLOYEE", "1", Convert.ToString(System.DateTime.Now.Date)).ToString();
                        EmployeeDTO.pemployeecode = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT generateemployeereferralnextid('EMPLOYEE','','" + FormatDate(Convert.ToString(System.DateTime.Now.Date)) + "')"));


                        EmployeeId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbl_mst_employee(contact_id,employee_code,basic_amount,allowance_amount,ctc_amount,designation_id,role_id,date_of_joining,residential_status, place_of_birth, country_id, nationality, minority_community, martial_status, status,disciplinary_actions,extra_curricular_activities) VALUES(" + contactId + ",'" + EmployeeDTO.pemployeecode + "'," + EmployeeDTO.pEmploymentBasicSalary + ", " + EmployeeDTO.pEmploymentAllowanceORvda + ", " + EmployeeDTO.pEmploymentCTC + "," + EmployeeDTO.mdesignationid + "," + EmployeeDTO.pEmploymentRoleId + "," + EmployeeDTO.pEmploymentJoiningDate + ",'" + EmployeeDTO.presidentialstatus + "','" + EmployeeDTO.pplaceofbirth + "'," + EmployeeDTO.pCountryId + ",'" + EmployeeDTO.pnationality + "','" + EmployeeDTO.pminoritycommunity + "','" + EmployeeDTO.pmaritalstatus + "','true','" + EmployeeDTO.pdisciplinaryactions + "','" + EmployeeDTO.pextracurricularactivities + "') returning tbl_mst_employee_id;"));
                    }

                    if (EmployeeDTO.plstemployess.Count > 0)
                    {
                        string Recordid = string.Empty;

                        for (int i = 0; i < EmployeeDTO.plstemployess.Count; i++)
                        {
                            if (Convert.ToString(EmployeeDTO.plstemployess[i].pdateofbirth) == string.Empty)
                            {

                                EmployeeDTO.plstemployess[i].pdateofbirth = "null";
                            }
                            else
                            {
                                EmployeeDTO.plstemployess[i].pdateofbirth = "'" + FormatDate(EmployeeDTO.plstemployess[i].pdateofbirth.ToString()) + "'";
                            }
                            if (Convert.ToString(EmployeeDTO.plstemployess[i].qualificationid) == string.Empty)
                            {
                                EmployeeDTO.plstemployess[i].qualificationid = "null";
                            }
                            if (Convert.ToString(EmployeeDTO.plstemployess[i].ptypeofoperation) != "CREATE")
                            {
                                if (string.IsNullOrEmpty(EmployeeRecordid))
                                {
                                    EmployeeRecordid = EmployeeDTO.plstemployess[i].precordid.ToString();
                                }
                                else
                                {
                                    EmployeeRecordid = EmployeeRecordid + "," + EmployeeDTO.plstemployess[i].precordid.ToString();
                                }
                            }
                            if (Convert.ToString(EmployeeDTO.plstemployess[i].ptypeofoperation) == "CREATE")
                            {

                                sb.Append("insert into tbl_mst_employee_family(tbl_mst_employee_id, relationship_id, relation_name, relation_dateofbirth, relation_age, relation_gender,relation_martial_status, relation_qualifcation_id, relation_occupation, relation_phone_number, status) values(" + EmployeeId + "," + EmployeeDTO.plstemployess[i].relationshipid + ",'" + EmployeeDTO.plstemployess[i].pname + "'," + EmployeeDTO.plstemployess[i].pdateofbirth + "," + EmployeeDTO.plstemployess[i].page + ",'" + EmployeeDTO.plstemployess[i].pgender + "','" + EmployeeDTO.plstemployess[i].pmaritialstatus + "'," + EmployeeDTO.plstemployess[i].qualificationid + ",'" + EmployeeDTO.plstemployess[i].poccupation + "','" + EmployeeDTO.plstemployess[i].pphoneno + "','true');");
                            }
                            if (Convert.ToString(EmployeeDTO.plstemployess[i].ptypeofoperation) == "UPDATE")
                            {

                                sb.Append("update tbl_mst_employee_family set  relationship_id=" + EmployeeDTO.plstemployess[i].relationshipid + ", relation_name='" + EmployeeDTO.plstemployess[i].pname + "', relation_dateofbirth=" + EmployeeDTO.plstemployess[i].pdateofbirth + ", relation_age=" + EmployeeDTO.plstemployess[i].page + ", relation_gender='" + EmployeeDTO.plstemployess[i].pgender + "',relation_martial_status='" + EmployeeDTO.plstemployess[i].pmaritialstatus + "', relation_qualifcation_id=" + EmployeeDTO.plstemployess[i].qualificationid + ", relation_occupation='" + EmployeeDTO.plstemployess[i].poccupation + "', relation_phone_number='" + EmployeeDTO.plstemployess[i].pphoneno + "' where tbl_mst_employee_family_id=" + EmployeeDTO.plstemployess[i].precordid + " and tbl_mst_employee_id=" + EmployeeId + ";");
                            }

                        }

                    }

                    if (!string.IsNullOrEmpty(EmployeeRecordid))
                    {

                        query.Append("update " + "tbl_mst_employee_family set status=false where tbl_mst_employee_id=" + EmployeeId + "  and tbl_mst_employee_family_id not in(" + EmployeeRecordid + ");");

                    }
                    else
                    {
                        if (EmployeeDTO.plstemployess.Count == 0)
                        {
                            query.Append("update " + "tbl_mst_employee_family set status=false where tbl_mst_employee_id=" + EmployeeId + ";");

                        }
                    }
                }
                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query.ToString() + "" + sb.ToString());
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

        public async Task<ContactEmployeeDTO> GetEmployeedeatils(string ConnectionString, string refernceid)
        {
            ContactEmployeeDTO EmployeeDTO = new ContactEmployeeDTO();
            EmployeeDTO.plstemployess = new List<familyDetailsDTO>();
            long contactId = 0;
            long EmployeeId;
            await Task.Run(() =>
            {
                try
                {
                    contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + refernceid + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));

                    EmployeeId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select tbl_mst_employee_id from tbl_mst_employee where contact_id=" + contactId + " and status='true';"));

                    string query = "SELECT tbl_mst_employee_id, contact_id,  employee_code,COALESCE(basic_amount::text,'') as basic_amount, COALESCE(allowance_amount::text,'') as allowance_amount,COALESCE(ctc_amount::text,'') as ctc_amount,COALESCE((select designation_name from tbl_mst_designation where tbl_mst_designation_id=t1.designation_id), '') designation_name, COALESCE(designation_id::text,'') as designation_id,COALESCE((select rolename as role_name from tblmstemployeerole where roleid=t1.role_id), '') role_name,COALESCE(role_id::text,'') as role_id,COALESCE(date_of_joining::text,'') as date_of_joining, residential_status, COALESCE(place_of_birth::text,'') as place_of_birth,COALESCE((select country from tblmstcountry where countryid=t1.country_id), '') country_name, COALESCE(country_id::text,'') as country_id,COALESCE(nationality,'') as nationality,COALESCE( minority_community,'') as minority_community, martial_status, status, COALESCE(disciplinary_actions,'') as disciplinary_actions,COALESCE(extra_curricular_activities,'') as extra_curricular_activities, COALESCE(list_of_copies::text,'') as list_of_copies FROM tbl_mst_employee t1 where t1.tbl_mst_employee_id = " + EmployeeId + " and t1.status = 'true';";


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())

                        {
                            EmployeeDTO.precordid = dr["tbl_mst_employee_id"];
                            EmployeeDTO.pEmploymentBasicSalary = dr["basic_amount"];
                            EmployeeDTO.pEmploymentAllowanceORvda = dr["allowance_amount"];
                            EmployeeDTO.pEmploymentCTC = dr["ctc_amount"];
                            EmployeeDTO.mdesignationid = dr["designation_id"];
                            EmployeeDTO.mdesignationname = dr["designation_name"];
                            //obj.pDob = Convert.ToDateTime(dr["date_of_joining"]).ToString("dd-MM-yyyy");
                            EmployeeDTO.pEmploymentJoiningDate = Convert.ToDateTime(dr["date_of_joining"]).ToString("dd/MM/yyyy");
                            EmployeeDTO.pEmploymentRoleId = dr["role_id"];
                            EmployeeDTO.pEmploymentRoleName = dr["role_name"];

                            EmployeeDTO.pplaceofbirth =Convert.ToString(dr["place_of_birth"]);
                            EmployeeDTO.presidentialstatus = Convert.ToString(dr["residential_status"]);
                            EmployeeDTO.pnationality = Convert.ToString(dr["nationality"]);
                            EmployeeDTO.pminoritycommunity = Convert.ToString(dr["minority_community"]);
                            EmployeeDTO.pCountryId = dr["country_id"];
                            EmployeeDTO.pCountry = dr["country_name"];
                            EmployeeDTO.pmaritalstatus = Convert.ToString(dr["martial_status"]);
                            EmployeeDTO.pdisciplinaryactions = dr["disciplinary_actions"];
                            EmployeeDTO.pextracurricularactivities = dr["extra_curricular_activities"];
                            //  EmployeeDTO.pdateofreporting = dr["nationality"];
                        }
                    }
                    string strfamily = "SELECT tbl_mst_employee_family_id,  tbl_mst_employee_id, COALESCE(relationship_id::text,'') as relationship_id,COALESCE((select relationship_name from tbl_mst_relationship where tbl_mst_relationship_id=t1.relationship_id), '') relationship_name, COALESCE(relation_name,'') as relation_name,COALESCE(relation_dateofbirth::text,'') as relation_dateofbirth,COALESCE(to_char(relation_dateofbirth, 'DD/MM/YYYY')::text,'') as dateofbirth,COALESCE(relation_age::text,'') as relation_age,CASE WHEN relation_gender ='M' THEN 'Male' WHEN relation_gender ='F' THEN 'Female' WHEN relation_gender ='T' THEN 'Third Gender' ELSE '' END as relation_gender,CASE WHEN relation_martial_status ='M' THEN 'Married' WHEN relation_martial_status ='U' THEN 'Un-married'  ELSE '' END as relation_martial_status,COALESCE((select qualification_name from tbl_mst_qualification where tbl_mst_qualification_id=t1.relation_qualifcation_id), '') qualification_name,COALESCE(relation_qualifcation_id::text,'') as relation_qualifcation_id, COALESCE(relation_occupation,'') as relation_occupation,COALESCE(relation_phone_number::text,'') as relation_phone_number, status FROM tbl_mst_employee_family t1 where tbl_mst_employee_id = " + EmployeeId + " and status = 'true';";
                    using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strfamily))
                    {
                        while (dr1.Read())

                        {
                            EmployeeDTO.plstemployess.Add(new familyDetailsDTO
                            {

                                precordid = dr1["tbl_mst_employee_family_id"],
                                relationshipid = dr1["relationship_id"],
                                relationshipname =Convert.ToString(dr1["relationship_name"]),
                                pname =Convert.ToString(dr1["relation_name"]),
                                pdateofbirth = string.IsNullOrEmpty(Convert.ToString(dr1["relation_dateofbirth"])) ? "" : Convert.ToDateTime(dr1["relation_dateofbirth"]).ToString("dd/MM/yyyy"),
                                pdob = Convert.ToString(dr1["dateofbirth"]),
                                page = Convert.ToInt32(dr1["relation_age"]) == 0 ? "" : dr1["relation_age"],
                                pmaritialstatus = dr1["relation_martial_status"],
                                qualificationid = dr1["relation_qualifcation_id"],
                                qualificationname = dr1["qualification_name"],
                                poccupation = dr1["relation_occupation"],
                                pphoneno = dr1["relation_phone_number"],
                                ptypeofoperation = "OLD"

                            });


                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return EmployeeDTO;
        }


        //public async Task<List<RelationShipDTO>> getRelationShip(string ConnectionString)
        //{
        //    List<RelationShipDTO> _lstRelationShip = new List<RelationShipDTO>();
        //    await Task.Run(() =>
        //    {

        //        try
        //        {

        //            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tbl_mst_relationship_id,relationship_name FROM tbl_mst_relationship where status=true"))
        //            {

        //                while (dr.Read())
        //                {
        //                    RelationShipDTO _RelationShip = new RelationShipDTO
        //                    {
        //                        relationshipid = dr["tbl_mst_relationship_id"],
        //                        relationshipname = dr["relationship_name"]
        //                    };

        //                    _lstRelationShip.Add(_RelationShip);
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //        finally
        //        {

        //        }

        //    });
        //    return _lstRelationShip;
        //}

        public async Task<List<documentstoreDTO>> GetContactExistingKycDetails(string ConnectionString, long contactid)

        {
            List<documentstoreDTO> _documetslist = new List<documentstoreDTO>();
            await Task.Run(() =>
            {

                try
                {
                    string _query = "";

                    _query = "select docstoreid as tbl_mst_contact_documents_id, documentgroupid as document_proofs_id,coalesce( docfiletype,'')document_file_type,coalesce( docreferenceno,'')document_reference_no, docisdownloadable as document_is_downloadable, coalesce(filename,'')document_file_name,coalesce( document_file_month,'')document_file_month,coalesce( document_file_year,'')document_file_year  from tblmstdocumentstore where statusid in(select statusid from tblmststatus where statusname='Active') and contactid=" + contactid;

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, _query))
                    {
                        while (dr.Read())
                        {

                            documentstoreDTO _KYCDocumentsDTO = new documentstoreDTO();

                            _KYCDocumentsDTO.pDocstoreId =Convert.ToInt64( dr["tbl_mst_contact_documents_id"]);
                            _KYCDocumentsDTO.pDocumentId = Convert.ToInt64(dr["document_proofs_id"]);

                            if (!string.IsNullOrEmpty(Convert.ToString(_KYCDocumentsDTO.pDocumentId)))
                            {
                                string details = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select documentgroupid||'@'||documentname from tblmstdocumentproofs  where documentid=" + _KYCDocumentsDTO.pDocumentId));

                                _KYCDocumentsDTO.pDocumentName = details.Split('@')[1];
                                _KYCDocumentsDTO.pDocumentGroupId = Convert.ToInt64(details.Split('@')[0]);
                            }
                            else
                            {
                                _KYCDocumentsDTO.pDocumentName = "";
                                _KYCDocumentsDTO.pDocumentGroupId = 0;
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(_KYCDocumentsDTO.pDocumentGroupId)))
                                _KYCDocumentsDTO.pDocumentGroup =Convert.ToString( NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select documentgroupname from tblmstdocumentgroup  where documentgroupid=" + _KYCDocumentsDTO.pDocumentGroupId));
                            else
                                _KYCDocumentsDTO.pDocumentGroup = "";

                            // _KYCDocumentsDTO.pDocStorePath = dr["document_path"];
                            _KYCDocumentsDTO.pDocFileType = dr["document_file_type"].ToString();
                            _KYCDocumentsDTO.pDocReferenceno = dr["document_reference_no"].ToString(); 
                            _KYCDocumentsDTO.pDocIsDownloadable =Convert.ToBoolean( dr["document_is_downloadable"]);
                            _KYCDocumentsDTO.pFilename = dr["document_file_name"].ToString(); 
                            _KYCDocumentsDTO.pDocumentReferenceMonth = Convert.ToString(dr["document_file_month"]);
                            _KYCDocumentsDTO.pDocumentReferenceYear = Convert.ToString(dr["document_file_year"]);
                            _KYCDocumentsDTO.ptypeofoperation = "OLD";
                            _KYCDocumentsDTO.pContactId = contactid;
                            //_KYCDocumentsDTO.pDocumentReferenceYear = Convert.ToString(dr["document_file_month_year"]).Split('-')[1];
                            _documetslist.Add(_KYCDocumentsDTO);

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {

                }

            });
            return _documetslist;
        }
        public async Task<ReferralDTO> GetReferraldeatils(string ConnectionString, string refernceid)
        {
            List<documentstoreDTO> lstdocuments = new List<documentstoreDTO>();
            DataSet ds1 = new DataSet();
            DataSet ds = new DataSet();
            long contactId = 0;
            ReferralDTO ReferralDTO = new ReferralDTO();
            await Task.Run(async () =>
            {
                try
                {
                    contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + refernceid + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));
                    //string ReferralQuery = "SELECT coalesce(tbl_mst_referral_id:: text,'') as tbl_mst_referral_id,coalesce(referral_code,'') as referral_code,coalesce(contact_id::text,'') as contact_id,coalesce(pan_number,'') as pan_number,COALESCE((select trim(coalesce(name,'')||' '||coalesce( surname,'')) as contact_name from tblmstcontact where contactid=t1.introduced_id), '') introducedname,coalesce(introduced_id::text,'') as introduced_id,COALESCE((select tdssection from tblmsttdssections where recordid=t1.tdssectionid), '') section_name,coalesce(tdssectionid::text,'') as tdssectionid FROM tbl_mst_referral t1 where t1.contact_id=" + contactId + " and status='true';";
                    string ReferralQuery = "SELECT coalesce(tbl_mst_referral_id:: text,'') as tbl_mst_referral_id,coalesce(referral_code,'') as referral_code,coalesce(contact_id::text,'') as contact_id,coalesce(pan_number,'') as pan_number,COALESCE((select trim(coalesce(name,'')||' '||coalesce( surname,'')) as contact_name from tblmstcontact where contactid=t1.introduced_id), '') introducedname,coalesce(introduced_id::text,'') as introduced_id,COALESCE((select tdssection from tblmsttdssections where recordid=t1.tdssectionid), '') section_name,coalesce(tdssectionid::text,'') as tdssectionid, filename FROM tbl_mst_referral t1 join tblmstdocumentstore td on t1.contact_id = td.contactid where t1.contact_id=" + contactId + " and status='true';";
                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, ReferralQuery);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in ds.Tables[0].Rows)
                        {
                            ReferralDTO.precordid = dr1["tbl_mst_referral_id"];
                            ReferralDTO.preferralcode = dr1["referral_code"];
                            ReferralDTO.pContactId = dr1["contact_id"];
                            ReferralDTO.pIsPanNoAvailable = true;
                            ReferralDTO.pPanNumber = dr1["pan_number"];
                            ReferralDTO.introducedname = dr1["introducedname"];
                            ReferralDTO.introducedid = dr1["introduced_id"];
                            ReferralDTO.ptdsSectionName = dr1["section_name"];
                            ReferralDTO.pFilename = dr1["filename"];
                        }
                    }

                    else
                    {
                        //  EasyChitConfigurationDAL _EasyChitConfigurationDAL = new EasyChitConfigurationDAL();

                        lstdocuments = await GetContactExistingKycDetails(ConnectionString, contactId);
                        if (lstdocuments != null)
                        {
                            int getid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentgroupid,0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%';"));
                            int DocumentId = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentid,0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%';"));
                            if (getid > 0)
                            {
                                string _query = "select docstoreid as tbl_mst_contact_documents_id, documentgroupid as document_proofs_id,coalesce( docfiletype,'')document_file_type,coalesce( docreferenceno,'')document_reference_no, docisdownloadable as document_is_downloadable, coalesce(filename,'')document_file_name,coalesce( document_file_month,'')document_file_month,coalesce( document_file_year,'')document_file_year  from tblmstdocumentstore where statusid in(select statusid from tblmststatus where statusname='Active') and contactid=" + contactId + " and documentgroupid=" + getid + " and documentid="+ DocumentId + "";
                                ds1 = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, _query);
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow dr in ds1.Tables[0].Rows)
                                    {
                                        ReferralDTO.pIsPanNoAvailable = true;
                                        ReferralDTO.pPanNumber = dr["document_reference_no"];
                                    }
                                }
                                else
                                {
                                    ReferralDTO.pIsPanNoAvailable = false;
                                    ReferralDTO.pPanNumber = "";
                                }
                            }
                            else
                            {
                                ReferralDTO.pIsPanNoAvailable = false;
                                ReferralDTO.pPanNumber = "";
                            }
                        }
                        else
                        {
                            ReferralDTO.pIsPanNoAvailable = false;
                            ReferralDTO.pPanNumber = "";
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return ReferralDTO;

            }
        public async Task<List<QualificationDTO>> ViewQualificationDetails(string ConnectionString)
        {
            List<QualificationDTO> _lstQualification = new List<QualificationDTO>();
            await Task.Run(() =>
            {
                try
                {
                    string _qualificationquery = "select tbl_mst_qualification_id,qualification_name,status from tbl_mst_qualification where status=true order by tbl_mst_qualification_id desc;";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, _qualificationquery))
                    {
                        while (dr.Read())
                        {
                            _lstQualification.Add(new QualificationDTO
                            {
                                qualificationid = dr["tbl_mst_qualification_id"],
                                qualificationname = dr["qualification_name"],
                                pStatusname = Convert.ToString(dr["status"])
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return _lstQualification;
        }
        public async Task<List<IntroducedDTO>> GetInterducedDetails(string ConnectionString)

        {
            List<IntroducedDTO> _lstInterduced = new List<IntroducedDTO>();
            await Task.Run(() =>
            {
                try
                {
                    string _query = "";
                    _query = "select 0 as contact_id,'Direct' as contact_name,'' as business_entity_contactno,'' as business_entity_emailid,'' as referral_code union all select t1.contact_id,trim(coalesce(name,'')||' '||coalesce(surname,'')) as contact_name,coalesce(businessentitycontactno::text,'')business_entity_contactno,coalesce(businessentityemailid,'')business_entity_emailid,coalesce(referral_code,'') referral_code   from tbl_mst_referral t1  join tblmstcontact   t3 on t1.contact_id =t3.contactid where t1.status=true;";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, _query))
                    {
                        while (dr.Read())
                        {
                            _lstInterduced.Add(new IntroducedDTO
                            {
                                introducedid = dr["contact_id"],
                                introducedcode = dr["referral_code"],
                                introducedname = dr["contact_name"],
                                introducedmobilenumber = dr["business_entity_contactno"],
                                introducedemailid = dr["business_entity_emailid"]
                            });

                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {

                }

            });
            return _lstInterduced;
        }
        public bool SaveContactReferral(string ConnectionString, ReferralDTO ReferralDTO)
        {
            bool isSaved = false;
            int count = 0;
            object formname = "AGENT";
            long contactId = 0;
            try
            {
                StringBuilder sb = new StringBuilder();
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                contactId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "Select contactid from tblmstcontact where contactreferenceid='" + ReferralDTO.pContactId + "' and  statusid in (select statusid from tblmststatus  where    upper(statusname)  ='ACTIVE');"));

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tbl_mst_referral where contact_id=" + contactId + " and status='true';"));
                if (ReferralDTO != null)
                {
                    if (Convert.ToString(ReferralDTO.ptdsSectionName) == string.Empty)
                    {
                        ReferralDTO.ptdsSectionID = "null";
                    }
                    else
                    {
                        ReferralDTO.ptdsSectionID = NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select recordid from tblmsttdssections where tdssection='" + ReferralDTO.ptdsSectionName + "';");// 1;
                    }
                    if (count == 0)
                    {
                        //object referralcode = GenerateNextID(trans,  GlobalSchema, Convert.ToString(formname), "", Convert.ToString(System.DateTime.Now.Date));

                        //object referralcode = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('" + Convert.ToString(formname) + "','',CURRENT_DATE)").ToString();
                        //object referralcode = GenerateNextID(trans, Convert.ToString(formname), "", Convert.ToString(System.DateTime.Now.Date)).ToString();
                        object referralcode = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT generateemployeereferralnextid('"+ Convert.ToString(formname) + "','','" + FormatDate(Convert.ToString(System.DateTime.Now.Date)) + "')"));

                        sb.Append("insert into tbl_mst_referral (contact_id,referral_code,pan_number,introduced_id,tdssectionid,status) values (" + contactId + ",'" + referralcode + "','" + (ReferralDTO.pPanNumber) + "'," + (ReferralDTO.introducedid) + "," + ReferralDTO.ptdsSectionID + ",'true');");
                        sb.Append("insert into tbl_mst_referral_log (user_id, ip_address, log_entry_date_time,activity_type,contact_id,referral_code,pan_number,introduced_id,tdssectionid,status) values (" + ReferralDTO.pCreatedby + ",'" + ReferralDTO.pipaddress + "',current_timestamp,'C'," + contactId + ",'" + referralcode + "','" + (ReferralDTO.pPanNumber) + "'," + (ReferralDTO.introducedid) + "," + ReferralDTO.ptdsSectionID + ",'true');");
                    }
                    else
                    {
                        sb.Append("update tbl_mst_referral set pan_number='" + (ReferralDTO.pPanNumber) + "',introduced_id=" + (ReferralDTO.introducedid) + ",tdssectionid=" + ReferralDTO.ptdsSectionID + " where contact_id=" + contactId + ";");
                        sb.Append("insert into tbl_mst_referral_log (user_id, ip_address, log_entry_date_time,activity_type,contact_id,pan_number,introduced_id,tdssectionid,status) values (" + ReferralDTO.pCreatedby + ",'" + ReferralDTO.pipaddress + "',current_timestamp,'U'," + contactId + ",'" + (ReferralDTO.pPanNumber) + "'," + (ReferralDTO.introducedid) + "," + ReferralDTO.ptdsSectionID + ",'true');");
                    }
                }
                if (ReferralDTO.pIsPanNoAvailable == false && Convert.ToString(ReferralDTO.pPanNumber) != string.Empty)
                {
                    //int dcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT count(*) as count  FROM  " + CommonDAL.AddDoubleQuotes(GlobalSchema) + ".tbl_mst_document_proofs where upper(document_name) like '%PAN%' ;"));

                    int dcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentgroupid,0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%';"));
                    object getid = 0;

                    if (dcount == 0)
                    {
                        getid = (NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce(documentgroupid,0) as count  FROM  tblmstdocumentgroup where upper(documentgroupname) like '%TAX DOCUMENTS%';"));

                        getid = (NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "insert into  tblmstdocumentproofs (documentgroupid,documentname,statusid,createdby,createddate)values(" + getid + ",'PAN CARD',1," + ReferralDTO.pCreatedby + ",current_timestamp) returning documentid"));
                    }
                    else
                    {
                        getid = (NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT coalesce((coalesce(documentid,0)),0) as count  FROM  tblmstdocumentproofs where upper(documentname) like '%PAN%' order by documentid desc  limit 1 ;"));
                    }

                    sb.Append("insert into tblmstdocumentstore (contactid,documentid,documentgroupid,documentgroupname,documentname,docreferenceno,statusid,filename,docisdownloadable,createdby,createddate) values (" + contactId + "," + getid + ",2,'Identification Documents','PAN Card','" + ReferralDTO.pPanNumber + "',1,'" + ReferralDTO.pFilename + "','f'," + ReferralDTO.pCreatedby + ",current_timestamp);");
                }

                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
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
        public async Task<List<RelationShipNewDTO>> getRelationShip(string ConnectionString)
        {
            List<RelationShipNewDTO> _lstRelationShip = new List<RelationShipNewDTO>();
            await Task.Run(() =>
            {

                try
                {

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tbl_mst_relationship_id,relationship_name FROM tbl_mst_relationship where status=true"))
                    {

                        while (dr.Read())
                        {
                            RelationShipNewDTO _RelationShip = new RelationShipNewDTO
                            {
                                relationshipid = dr["tbl_mst_relationship_id"],
                                relationshipname = dr["relationship_name"]
                            };

                            _lstRelationShip.Add(_RelationShip);
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
            return _lstRelationShip;
        }
        public bool SaveContactSupplier(string ConnectionString, SupplierDTO SupplierDTO)
        {
            bool isSaved = false;

            try
            {
                StringBuilder sb = new StringBuilder();
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sb.Append("update tblmstcontact set is_supplier_applicable='" + (SupplierDTO.pIsSupplier) + "' where contactreferenceid='" + SupplierDTO.pContactId + "';");

                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
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
        public bool SaveContactAdvocate(string ConnectionString, AdvocateDTO AdvocateDTO)
        {
            bool isSaved = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                sb.Append("update tblmstcontact set is_advocate_applicable='" + (AdvocateDTO.pIsAdvocate) + "' where contactreferenceid='" + AdvocateDTO.pContactId + "';");
                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
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

        public Int32 CheckDocumentExist(Int32 DocumentId, string ReferenceNo,string connectionstring)
        {
            int count = 0;
            try
            {
                if (!string.IsNullOrEmpty(ReferenceNo))
                {
                    
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstdocumentstore where   documentid ="+ DocumentId + " and docreferenceno='"+ ReferenceNo + "' and statusid=1"));

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;
        }
        public object GenerateNextID(NpgsqlTransaction trans, string FormName, string FieldName, string Trans_Date)
        {
            string _strQuery, strSubQuery, _yearField, _ID = string.Empty;
            int _year, _monthField;
            DateTime _fromDate, _toDate;
            long _nextid;
            DataRow dr;
            try
            {
                //con = new NpgsqlConnection(ConnectionString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}

                //trans = con.BeginTransaction();
                _strQuery = "SELECT formname,filedname,code,serice,tablename,columnname,concolunname,datecolumnname,finanicalyear,normalyear,suffix_year FROM tabgenerateidmaster WHERE formname='" + FormName + "' AND filedname='" + FieldName + "';";
                dr = NPGSqlHelper.ExecuteDataset(trans, CommandType.Text, _strQuery).Tables[0].Rows[0];
                _year = Convert.ToDateTime(Trans_Date).Year;
                _monthField = Convert.ToDateTime(Trans_Date).Month;
                if (dr != null)
                {
                    if (dr["finanicalyear"].ToString() == "Y" && dr["normalyear"].ToString() == "N")
                    {
                        _fromDate = _monthField <= 3 ? new DateTime(_year - 1, 04, 01) : new DateTime(_year, 04, 01);
                        _toDate = _monthField > 3 ? new DateTime(_year + 1, 03, 31) : new DateTime(_year, 03, 31);
                        _yearField = _fromDate.ToString("yy");
                    }
                    else if (dr["finanicalyear"].ToString() == "N" && dr["normalyear"].ToString() == "Y")
                    {
                        _fromDate = new DateTime(_year, 01, 01);
                        _toDate = new DateTime(_year, 12, 31);
                        _yearField = _fromDate.ToString("yy");
                    }
                    else
                    {
                        _fromDate = new DateTime(1990, 03, 01);
                        _toDate = DateTime.Now;
                        _yearField = "";
                    }

                    if (dr["suffix_year"].ToString() == "Y")
                    {
                        strSubQuery = "MAX(CAST(SPLIT_PART(SPLIT_PART(" + dr["columnname"].ToString() + ",'" + dr["code"].ToString() + "',2),'/" + _yearField + "',1)AS NUMERIC))";
                    }
                    else
                    {
                        strSubQuery = "MAX(CAST(SPLIT_PART(" + dr["columnname"].ToString() + ", '" + dr["code"].ToString() + "', 2)AS NUMERIC))";
                    }
                    if (dr["concolunname"].ToString() != "ACCOUNTING")
                    {

                        _strQuery = "SELECT " + strSubQuery + " AS MAXID FROM " + dr["tablename"].ToString() + " WHERE " + dr["concolunname"].ToString() + "='" + dr["filedname"].ToString() + "' AND " + dr["datecolumnname"].ToString() + " BETWEEN '" + FormatDate(_fromDate.ToShortDateString()) + "'::date  AND '" + FormatDate(_toDate.ToShortDateString()) + "'::date AND " + dr["columnname"].ToString() + " LIKE '" + dr["code"].ToString() + "%' ;";
                    }
                    else
                    {
                        _strQuery = "SELECT " + strSubQuery + " AS MAXID FROM " + dr["tablename"].ToString() + " WHERE " + dr["datecolumnname"].ToString() + " BETWEEN '" + FormatDate(_fromDate.ToShortDateString()) + "'::date  AND '" + FormatDate(_toDate.ToShortDateString()) + "'::date AND " + dr["columnname"].ToString() + " LIKE '" + dr["code"].ToString() + "%';";
                    }
                    _ID = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, _strQuery));
                    if (string.IsNullOrEmpty(_ID))
                    {
                        if (dr["suffix_year"].ToString() == "Y")
                        {
                            _ID = dr["code"].ToString() + dr["serice"].ToString() + "/" + _yearField;
                        }
                        else
                        {
                            _ID = dr["code"].ToString() + _yearField + dr["serice"].ToString();
                        }
                    }
                    else
                    {
                        _nextid = Convert.ToInt64(_ID);
                        var len = dr["serice"].ToString().Length;
                        if (dr["suffix_year"].ToString() == "Y")
                        {
                            _ID = dr["code"].ToString() + (_nextid + 1).ToString().PadLeft(len, '0') + "/" + _yearField;
                        }
                        else
                        {
                            _ID = dr["code"].ToString() + (_nextid + 1).ToString().PadLeft(len, '0');
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //trans.Dispose();
                //con.Dispose();
                //con.Close();
            }
            return _ID;
        }

        public List<SubscriberContactDTO> GetContactsList(string ConnectionString)
        {
            string Query = string.Empty;
            List<SubscriberContactDTO> lstcontactdetails = new List<SubscriberContactDTO>();

            try
            {

                Query = "select x.*,y.statusname from (select tc.contacttype,tc.name,tc.surname,tc.contactid,tc.contactreferenceid,tc.statusid,businessentityemailid , businessentitycontactno ,case when contact_mailing_name='' then trim(name) else trim(contact_mailing_name) end as contact_name from tblmstcontact tc  ) x left join tblmststatus y on x.statusid=y.statusid order by x.contactid desc;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        SubscriberContactDTO obj = new SubscriberContactDTO();
                        obj.contactid = Convert.ToInt64(dr["contactid"]);
                        obj.contactreferenceid = dr["contactreferenceid"].ToString();
                        obj.contacttype = dr["contacttype"].ToString();
                        //obj.pStatusid = dr["statusid"].ToString();
                        //obj.pStatusname = dr["statusname"].ToString();
                        // obj.pName = dr["name"].ToString() + " " + dr["surname"].ToString();

                        obj.contactname = dr["contact_name"].ToString();
                        obj.contactemailid = dr["businessentityemailid"].ToString();
                        obj.contactmobilenumber = dr["businessentitycontactno"].ToString();
                        //obj.pEmailidsList = new List<EmailidsDTO>();
                        //obj.pEmailidsList.Add(new EmailidsDTO
                        //{
                        //    pContactNumber = dr["contactnumber"].ToString()
                        //,
                        //    pEmailId = dr["emailid"].ToString()
                        //});

                        lstcontactdetails.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstcontactdetails;
        }
    }
}
