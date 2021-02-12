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



namespace FinstaRepository.DataAccess.Loans.Masters
{
    public class ContactMasterDAL : SettingsDAL, IContactMaster
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlDataReader dr = null;
        DataSet ds = null;

        NpgsqlTransaction trans = null;
        public List<contactAddressDTO> lstAddressDetails { get; set; }
        public List<EnterpriseTypeDTO> lstEnterpriseType { get; set; }
        public List<BusinessTypeDTO> lstBusinessType { get; set; }
        public List<ContactViewDTO> lstContactViewDTO { get; set; }
        public List<FirstinformationDTO> lstFirstinformationDTO { set; get; }
        #region SaveContact
        public bool Savecontact(ContactMasterDTO contact, string ConnectionString)
        {
            bool isSaved = false;

            try
            {
                StringBuilder sb = new StringBuilder();
                long ContactRefid;


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

                contact.pReferenceId = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('CONTACT','" + ManageQuote(contact.pContactType) + "',CURRENT_DATE)").ToString();

                if (contact.pContactType == "Individual")
                {
                    ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstcontact(transdate,contactreferenceid,contacttype,titlename,name,surname,businessentityemailid  , businessentitycontactno, dob,gender,gendercode,fathername,spousename,contactimagepath,statusid,createdby,createddate) values(CURRENT_DATE,'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactType) + "','" + ManageQuote(contact.pTitleName) + "','" + ManageQuote(contact.pName) + "','" + ManageQuote(contact.pSurName) + "','" + ManageQuote(contact.pBusinessEntityEmailid) + "','" + ManageQuote(contact.pBusinessEntityContactno) + "'," + (contact.pDob) + ",'" + ManageQuote(contact.pGender) + "','" + contact.pGenderCode + "','" + ManageQuote(contact.pFatherName) + "','" + ManageQuote(contact.pSpouseName) + "','" + ManageQuote(contact.pContactimagepath) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp) returning CONTACTID;"));
                    for (var i = 0; i < contact.pAddressList.Count; i++)
                    {
                        sb.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + ManageQuote(contact.pAddressList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");
                    }

                    for (var i = 0; i < contact.pEmailidsList.Count; i++)
                    {
                        if (contact.pEmailidsList[i].pContactNumber == "" || contact.pEmailidsList[i].pContactNumber == string.Empty || contact.pEmailidsList[i].pContactNumber == null)
                        {
                            contact.pEmailidsList[i].pContactNumber = "0";
                        }

                        sb.Append("insert into TBLMSTCONTACTPERSONDETAILS(contactid,contactreferenceid,contactname,contactnumber,emailid,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactName) + "'," + contact.pEmailidsList[i].pContactNumber + ",'" + ManageQuote(contact.pEmailidsList[i].pEmailId) + "','" + ManageQuote(contact.pEmailidsList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");
                    }
                }

                else
                {

                    ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstcontact(transdate,contactreferenceid,contacttype,name,typeofenterprise,natureofbusiness,businessentityemailid  , businessentitycontactno,contactimagepath,statusid,createdby,createddate) values(CURRENT_DATE,'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pContactType) + "','" + ManageQuote(contact.pName) + "','" + ManageQuote(contact.pEnterpriseType) + "','" + ManageQuote(contact.pBusinesstype) + "','" + ManageQuote(contact.pBusinessEntityEmailid) + "','" + ManageQuote(contact.pBusinessEntityContactno) + "','" + ManageQuote(contact.pContactimagepath) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp) returning CONTACTID;"));
                    if (contact.pAddressList != null)
                    {
                        for (int i = 0; i < contact.pAddressList.Count; i++)
                        {

                            sb.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + ManageQuote(contact.pAddressList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");

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
            return isSaved;
        }
        #endregion

        #region ViewContact
        public ContactMasterDTO ViewContact(string refernceid, string ConnectionString)
        {
            string addressdetails = string.Empty;


            ContactMasterDTO obj = new ContactMasterDTO();
            ds = new DataSet();

            try
            {

                obj.pAddressList = new List<contactAddressDTO>();
                obj.pEmailidsList = new List<EmailidsDTO>();
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
                        obj.pAddressList.Add(new contactAddressDTO
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
                            pPriority = dr1["Priority"].ToString(),
                            pAddressDetails = addressdetails
                        });
                    }
                    foreach (DataRow dr2 in ds.Tables[2].Rows)
                    {
                        obj.pEmailidsList.Add(new EmailidsDTO
                        {
                            pRecordId = Convert.ToInt64(dr2["recordid"]),
                            pContactName = dr2["contactname"].ToString(),
                            pContactNumber = dr2["contactnumber"].ToString(),
                            pEmailId = dr2["emailid"].ToString(),
                            pPriority = dr2["Priority"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return obj;
        }
        #endregion


        #region GetContactDetails
        public List<ContactMasterDTO> GetContactdetails(string ConnectionString, string Type)
        {
            string Query = string.Empty;
            List<ContactMasterDTO> lstcontactdetails = new List<ContactMasterDTO>();
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
                        ContactMasterDTO obj = new ContactMasterDTO();
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
        #endregion

        #region UpdateContact
        public bool UpdateContact(ContactMasterDTO contact, string ConnectionString)
        {
            bool isSaved = false;
            long ContactRefid;
            string Recordid = string.Empty;
            StringBuilder sbupdate = new StringBuilder();
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

                if (contact.pContactType == "Individual")
                {
                    sbupdate.Append("update tblmstcontact set name='" + ManageQuote(contact.pName) + "',surname='" + ManageQuote(contact.pSurName) + "',dob=" + (contact.pDob) + ",gender='" + ManageQuote(contact.pGender) + "',gendercode='" + contact.pGenderCode + "',fathername='" + ManageQuote(contact.pFatherName) + "',spousename='" + ManageQuote(contact.pSpouseName) + "',businessentityemailid ='" + ManageQuote(contact.pBusinessEntityEmailid) + "' , businessentitycontactno='" + ManageQuote(contact.pBusinessEntityContactno) + "',contactimagepath='" + ManageQuote(contact.pContactimagepath) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';");
                }
                else
                {
                    sbupdate.Append("update tblmstcontact set name='" + ManageQuote(contact.pName) + "',typeofenterprise='" + ManageQuote(contact.pEnterpriseType) + "',natureofbusiness='" + ManageQuote(contact.pBusinesstype) + "',businessentityemailid ='" + ManageQuote(contact.pBusinessEntityEmailid) + "' , businessentitycontactno='" + ManageQuote(contact.pBusinessEntityContactno) + "',contactimagepath='" + ManageQuote(contact.pContactimagepath) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';");
                }
                ContactRefid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select CONTACTID from tblmstcontact  where contactreferenceid='" + contact.pReferenceId + "';"));
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
                        if (contact.pAddressList[i].ptypeofoperation != "CREATE")
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

                            sbupdate.Append("update tblmstcontactaddressdetails set addresstype='" + ManageQuote(contact.pAddressList[i].pAddressType) + "', address1='" + ManageQuote(contact.pAddressList[i].pAddress1) + "',address2='" + ManageQuote(contact.pAddressList[i].pAddress2) + "',state='" + ManageQuote(contact.pAddressList[i].pState) + "',stateid=" + contact.pAddressList[i].pStateId + ",district='" + ManageQuote(contact.pAddressList[i].pDistrict) + "',districtid=" + contact.pAddressList[i].pDistrictId + ",city='" + ManageQuote(contact.pAddressList[i].pCity) + "',country='" + ManageQuote(contact.pAddressList[i].pCountry) + "',countryid=" + contact.pAddressList[i].pCountryId + ",pincode=" + contact.pAddressList[i].pPinCode + ",priority='" + ManageQuote(contact.pAddressList[i].pPriority) + "',modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "' and recordid=" + contact.pAddressList[i].pRecordId + ";");
                        }
                        else if (contact.pAddressList[i].ptypeofoperation == "CREATE")
                        {
                            sbupdate.Append("insert into tblmstcontactaddressdetails(CONTACTID,contactreferenceid,addresstype,address1,address2,state,stateid,district,districtid,city,country,countryid,pincode,priority,statusid,createdby,createddate) values(" + ContactRefid + ",'" + ManageQuote(contact.pReferenceId) + "','" + ManageQuote(contact.pAddressList[i].pAddressType) + "','" + ManageQuote(contact.pAddressList[i].pAddress1) + "','" + ManageQuote(contact.pAddressList[i].pAddress2) + "','" + ManageQuote(contact.pAddressList[i].pState) + "'," + contact.pAddressList[i].pStateId + ",'" + ManageQuote(contact.pAddressList[i].pDistrict) + "'," + contact.pAddressList[i].pDistrictId + ",'" + ManageQuote(contact.pAddressList[i].pCity) + "','" + ManageQuote(contact.pAddressList[i].pCountry) + "'," + contact.pAddressList[i].pCountryId + "," + contact.pAddressList[i].pPinCode + ",'" + ManageQuote(contact.pAddressList[i].pPriority) + "'," + getStatusid(contact.pStatusname, ConnectionString) + "," + contact.pCreatedby + ",current_timestamp);");
                        }
                        //else if (contact.pAddressList[i].ptypeofoperation == "DELETE")
                        //{
                        //    sbupdate.Append("update tblmstcontactaddressdetails set statusid=" + getStatusid(contact.pStatusname, ConnectionString) + ",modifiedby=" + contact.pAddressList[i].pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper()+ "' and upper(addresstype)='" + contact.pAddressList[i].pAddressType.ToUpper()+ "' and recordid=" + contact.pAddressList[i].pRecordId + ";");
                        //}
                    }
                }

                if (!string.IsNullOrEmpty(Recordid))
                {
                    query = "update tblmstcontactaddressdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "'  and recordid not in(" + Recordid + ");";
                }
                else
                {
                    query = "update tblmstcontactaddressdetails set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + contact.pCreatedby + ",modifieddate=current_timestamp where upper(contactreferenceid)='" + contact.pReferenceId.ToUpper() + "';";
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query + "" + sbupdate.ToString());
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

        #region DeleteContact
        public bool DeleteContact(ContactMasterDTO contact, string ConnectionString)
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
        public bool SaveAddressType(contactAddressDTO addresstype, string ConnectionString)
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

        public List<contactAddressDTO> GetAddressType(string contactype, string ConnectionString)
        {
            lstAddressDetails = new List<contactAddressDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select ADDRESSTYPE from TBLMSTADDRESSTYPES where upper(contacttype)='" + contactype.ToUpper() + "' order by ADDRESSTYPE"))
                {
                    while (dr.Read())
                    {
                        contactAddressDTO objaddressdetails = new contactAddressDTO();
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

        public List<EnterpriseTypeDTO> GetEnterpriseType(string ConnectionString)
        {

            lstEnterpriseType = new List<EnterpriseTypeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select enterprisetype from tblmstenterprisetypes order by enterprisetype"))
                {
                    while (dr.Read())
                    {
                        EnterpriseTypeDTO objenterprise = new EnterpriseTypeDTO();
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

        public bool SaveEnterpriseType(EnterpriseTypeDTO Enterprisetype, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstenterprisetypes(enterprisetype,statusid,createdby,createddate,entrytype)values('" + ManageQuote(Enterprisetype.pEnterpriseType.Trim()) + "'," + getStatusid(Enterprisetype.pStatusname, ConnectionString) + "," + Enterprisetype.pCreatedby + ",current_timestamp,'OTHER');");

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

        public List<BusinessTypeDTO> GetBusinessTypes(string ConnectionString)
        {
            lstBusinessType = new List<BusinessTypeDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select businesstype from tblmstbusinesstypes order by businesstype"))
                {
                    while (dr.Read())
                    {
                        BusinessTypeDTO objbusinesstype = new BusinessTypeDTO();
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

        public bool SaveBusinessTypes(BusinessTypeDTO Businesstype, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstbusinesstypes(businesstype,statusid,createdby,createddate,entrytype)values('" + ManageQuote(Businesstype.pBusinesstype.Trim()) + "'," + getStatusid(Businesstype.pStatusname, ConnectionString) + "," + Businesstype.pCreatedby + ",current_timestamp,'OTHER');");

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

        public int GetPersoncount(ContactMasterDTO ContactDto, string ConnectionString)
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

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstcontact x where upper(contactreferenceid)<>'" + ContactDto.pReferenceId + "' and upper(x.name)='" + ManageQuote(ContactDto.pName.ToUpper()) + "' and upper(coalesce(x.surname,''))='" + ManageQuote(ContactDto.pSurName.ToUpper()) + "' and x.businessentitycontactno=" + ContactDto.pBusinessEntityContactno + ";"));

            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public List<ContactViewDTO> GetContactView(string ViewName, string ConnectionString)
        {

            string Query = string.Empty;
            string addressdetails = string.Empty;
            lstContactViewDTO = new List<ContactViewDTO>();

            try
            {
                if (ViewName == "Contacts")
                {
                    Query = "SELECT * FROM vwcontactdataview order by createddate desc;";
                }
                else if (ViewName == "Applicants")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactreferenceid in(select distinct contactreferenceid from tabapplication);";
                }
                //if (ViewName.ToUpper().Trim() == "MEMBERS")
                //{
                //    Query = "SELECT * FROM vwcontactdataview WHERE contactreferenceid in(select distinct contactreferenceid from tabapplication);";
                //}
                else if (ViewName == "Parties")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstparty);";
                }
                else if (ViewName == "Employees")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstemployee);";
                }
                else if (ViewName == "Referrals")
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid  from tblmstreferral);";
                }
                else
                {
                    Query = "SELECT * FROM vwcontactdataview WHERE contactid in(select distinct contactid from tblmstadvocate);";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        lstContactViewDTO.Add(new ContactViewDTO
                        {
                            pContactdId = dr["contactid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["contactid"]),
                            pContactType = Convert.ToString(dr["contacttype"]),
                            pContactName = Convert.ToString(dr["fullname"]),
                            pRefNo = Convert.ToString(dr["contactreferenceid"]),
                            pFatherName = Convert.ToString(dr["fathername"]),
                            pContactNumber = Convert.ToString(dr["businessentitycontactno"]),
                            pContactEmail = Convert.ToString(dr["businessentityemailid"]),
                            pImagePath = Convert.ToString(dr["contactimagepath"]),
                            pAddresDetails = Convert.ToString(dr["contactaddress"]),
                        });

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstContactViewDTO;
        }





        #endregion
    }
}
