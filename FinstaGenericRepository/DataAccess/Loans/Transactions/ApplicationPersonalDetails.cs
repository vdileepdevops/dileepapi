using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public partial class FirstinformationDAL : SettingsDAL, IFirstinformation
    {
        #region  ApplicationPersonalDetails

        public List<EmployeeRoleDTO> EmployeeRoleList { get; set; }



        public List<EmployeeRoleDTO> GetEmployementRoles(string ConnectionString)
        {
            EmployeeRoleList = new List<EmployeeRoleDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select roleid,rolename from tblmstemployeerole t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE' order by rolename"))
                {
                    while (dr.Read())
                    {
                        EmployeeRoleDTO _EmployeeRole = new EmployeeRoleDTO();
                        _EmployeeRole.pemploymentroleid = Convert.ToInt64(dr["roleid"]);
                        _EmployeeRole.pemploymentrole = Convert.ToString(dr["rolename"]);
                        EmployeeRoleList.Add(_EmployeeRole);


                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return EmployeeRoleList;
        }
        public bool SaveApplicationPersonalInformation(ApplicationPersonalInformationDTO PersonalInformation, string ConnectionString)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();
            string recordid1 = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (PersonalInformation.pvchapplicationid != null)
                {
                    long AvailableAppcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tabapplication where vchapplicationid = '" + PersonalInformation.pvchapplicationid + "';"));
                    if (AvailableAppcount > 0)
                    {
                        PersonalInformation.papplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + PersonalInformation.pvchapplicationid + "';"));

                        sbQuery.Append("UPDATE tabapplication set Ispersonaldetailsapplicable= " + PersonalInformation.pIspersonaldetailsapplicable + " where vchapplicationid = '" + PersonalInformation.pvchapplicationid + "';");
                    }
                    else
                    {
                        long AvailableMembercount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembers where membercode = '" + PersonalInformation.pvchapplicationid + "';"));
                        if (AvailableMembercount > 0)
                        {
                            PersonalInformation.papplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select memberid from tblmstmembers where membercode = '" + PersonalInformation.pvchapplicationid + "';"));
                        }
                    }
                }

                //sbQuery.Append("UPDATE tabapplication set Ispersonaldetailsapplicable= " + PersonalInformation.pIspersonaldetailsapplicable + " where vchapplicationid = '" + PersonalInformation.pvchapplicationid + "';");


                if (PersonalInformation.PersonalEmployeementList != null)
                {
                    sbupdate.Append("update tabapplicationpersonalemplymentdetails set statusid=2 where vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "';");
                    for (int i = 0; i < PersonalInformation.PersonalEmployeementList.Count; i++)
                    {
                        if (PersonalInformation.PersonalEmployeementList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalEmployeementList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalEmployeementList[i].ptypeofoperation = PersonalInformation.PersonalEmployeementList[i].ptypeofoperation.ToUpper();
                        }

                        if (string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].pdateofestablishment))
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofestablishment = "null";
                        }
                        else
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofestablishment = "'" + FormatDate(PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + "'";
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].pdateofcommencement))
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofcommencement = "null";
                        }
                        else
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofcommencement = "'" + FormatDate(PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + "'";
                        }
                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isemplymentdetailsapplicable='" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "';");
                       
                        if (PersonalInformation.PersonalEmployeementList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalEmployeementList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalemplymentdetails(applicationid,vchapplicationid,contactid,contactreferenceid,applicanttype,isemploymentapplicable,employmenttype,nameoftheorganization,natureoftheorganization,employmentrole,officeaddress,officephoneno,reportingto,employeeexp,employeeexptype,totalworkexp,dateofestablishment,dateofcommencement,gstinno,cinno,dinno,tradelicenseno,statusid,createdby,createddate)values('" + (PersonalInformation.papplicationid) + "','" + ManageQuote(PersonalInformation.pvchapplicationid) + "','" + (PersonalInformation.PersonalEmployeementList[i].pcontactid) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].papplicanttype) + "','" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymenttype) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pnameoftheorganization) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pEnterpriseType) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymentrole) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficeaddress) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficephoneno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].preportingto) + "','" + (PersonalInformation.PersonalEmployeementList[i].pemployeeexp) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemployeeexptype) + "','" + (PersonalInformation.PersonalEmployeementList[i].ptotalworkexp) + "'," + (PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + "," + (PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + ",'" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pgstinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pdinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].ptradelicenseno) + "'," + Convert.ToInt32(Status.Active) + ",'" + (PersonalInformation.pCreatedby) + "',current_timestamp); ");
                        }
                        if (PersonalInformation.PersonalEmployeementList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalEmployeementList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalEmployeementList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalemplymentdetails set contactid='" + (PersonalInformation.PersonalEmployeementList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "', applicanttype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].papplicanttype) + "', isemploymentapplicable='" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "', employmenttype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymenttype) + "', nameoftheorganization='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pnameoftheorganization) + "', natureoftheorganization='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pEnterpriseType) + "', employmentrole='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymentrole) + "', officeaddress='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficeaddress) + "', officephoneno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficephoneno) + "', reportingto='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].preportingto) + "', employeeexp='" + (PersonalInformation.PersonalEmployeementList[i].pemployeeexp) + "', employeeexptype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemployeeexptype) + "', totalworkexp='" + (PersonalInformation.PersonalEmployeementList[i].ptotalworkexp) + "', dateofestablishment=" + (PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + ", dateofcommencement=" + (PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + ", gstinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pgstinno) + "', cinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcinno) + "', dinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pdinno) + "', tradelicenseno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].ptradelicenseno) + "', statusid='" + getStatusid(PersonalInformation.PersonalEmployeementList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "';");


                        }
                    }
                }
                if (PersonalInformation.PersonalDetailsList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalDetailsList.Count; i++)
                    {
                        if (PersonalInformation.PersonalDetailsList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalDetailsList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalDetailsList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalDetailsList[i].ptypeofoperation = PersonalInformation.PersonalDetailsList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set ispersonalbirthdetailsapplicable='" + (PersonalInformation.PersonalDetailsList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalDetailsList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalDetailsList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalbirthdetails( applicationid, vchapplicationid, contactid, contactreferenceid, residentialstatus, maritalstatus, placeofbirth, countryofbirth, nationality, minoritycommunity, statusid, createdby, createddate,applicantype)values('" + (PersonalInformation.papplicationid) + "','" + ManageQuote(PersonalInformation.pvchapplicationid) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcontactid) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].presidentialstatus) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pmaritalstatus) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pplaceofbirth) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcountryofbirth) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pnationality) + "','" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pminoritycommunity) + "'," + Convert.ToInt32(Status.Active) + ",'" + (PersonalInformation.pCreatedby) + "',current_timestamp,'" + ManageQuote(PersonalInformation.PersonalDetailsList[i].papplicanttype) + "'); ");
                        }
                        if (PersonalInformation.PersonalDetailsList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalDetailsList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalDetailsList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbirthdetails set contactid='" + (PersonalInformation.PersonalDetailsList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcontactreferenceid) + "', residentialstatus='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].presidentialstatus) + "', maritalstatus='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pmaritalstatus) + "', placeofbirth='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pplaceofbirth) + "', countryofbirth='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcountryofbirth) + "', nationality='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pnationality) + "', minoritycommunity='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pminoritycommunity) + "', statusid='" + getStatusid(PersonalInformation.PersonalDetailsList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalDetailsList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                if (PersonalInformation.PersonalFamilyList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalFamilyList.Count; i++)
                    {
                        if (PersonalInformation.PersonalFamilyList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalFamilyList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalFamilyList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalFamilyList[i].ptypeofoperation = PersonalInformation.PersonalFamilyList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isfamilydetailsapplicable='" + (PersonalInformation.PersonalFamilyList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalFamilyList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalFamilyList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalfamilydetails( applicationid, vchapplicationid, contactid, contactreferenceid, totalnoofmembers, noofearningmembers, familytype, noofboyschild, noofgirlchild, houseownership, statusid, createdby, createddate,applicantype)values('" + (PersonalInformation.papplicationid) + "','" + ManageQuote(PersonalInformation.pvchapplicationid) + "','" + (PersonalInformation.PersonalFamilyList[i].pcontactid) + "','" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pcontactreferenceid) + "','" + (PersonalInformation.PersonalFamilyList[i].ptotalnoofmembers) + "','" + (PersonalInformation.PersonalFamilyList[i].pnoofearningmembers) + "','" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pfamilytype) + "','" + (PersonalInformation.PersonalFamilyList[i].pnoofboyschild) + "','" + (PersonalInformation.PersonalFamilyList[i].pnoofgirlchild) + "','" + ManageQuote(PersonalInformation.PersonalFamilyList[i].phouseownership) + "'," + Convert.ToInt32(Status.Active) + ",'" + (PersonalInformation.pCreatedby) + "',current_timestamp,'" + ManageQuote(PersonalInformation.PersonalFamilyList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalFamilyList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalFamilyList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalFamilyList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalfamilydetails set contactid='" + (PersonalInformation.PersonalFamilyList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pcontactreferenceid) + "', totalnoofmembers='" + (PersonalInformation.PersonalFamilyList[i].ptotalnoofmembers) + "', noofearningmembers='" + (PersonalInformation.PersonalFamilyList[i].pnoofearningmembers) + "', familytype='" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pfamilytype) + "', noofboyschild='" + (PersonalInformation.PersonalFamilyList[i].pnoofboyschild) + "', noofgirlchild='" + (PersonalInformation.PersonalFamilyList[i].pnoofgirlchild) + "', houseownership='" + ManageQuote(PersonalInformation.PersonalFamilyList[i].phouseownership) + "', statusid='" + getStatusid(PersonalInformation.PersonalFamilyList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalFamilyList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                recordid1 = string.Empty;
                if (PersonalInformation.PersonalNomineeList != null)
                {

                    for (int i = 0; i < PersonalInformation.PersonalNomineeList.Count; i++)
                    {
                        if (PersonalInformation.PersonalNomineeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = PersonalInformation.PersonalNomineeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + PersonalInformation.PersonalNomineeList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.PersonalNomineeList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalNomineeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalNomineeList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalNomineeList[i].ptypeofoperation = PersonalInformation.PersonalNomineeList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.PersonalNomineeList[i].pdateofbirth))
                        {

                            PersonalInformation.PersonalNomineeList[i].pdateofbirth = "null";
                        }
                        else
                        {

                            PersonalInformation.PersonalNomineeList[i].pdateofbirth = "'" + FormatDate(PersonalInformation.PersonalNomineeList[i].pdateofbirth) + "'";
                        }


                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isnomineedetailsapplicable='" + (PersonalInformation.PersonalNomineeList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalNomineeList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalNomineeList[i].pisapplicable == true && !string.IsNullOrEmpty(PersonalInformation.PersonalNomineeList[i].pnomineename))
                                sbQuery.Append("insert into tabapplicationpersonalnomineedetails(applicationid, vchapplicationid, contactid, contactreferenceid, nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath, statusid, createdby, createddate,applicantype,isprimarynominee) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalNomineeList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pcontactreferenceid) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pnomineename) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].prelationship) + "', " + (PersonalInformation.PersonalNomineeList[i].pdateofbirth) + ", '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pcontactno) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pidprooftype) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pidproofname) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].preferencenumber) + "', '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pdocidproofpath) + "', " + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalNomineeList[i].papplicanttype) + "'," + (PersonalInformation.PersonalNomineeList[i].pisprimarynominee) + ");");
                        }
                        if (PersonalInformation.PersonalNomineeList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalNomineeList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalNomineeList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalnomineedetails set contactid = '" + (PersonalInformation.PersonalNomineeList[i].pcontactid) + "', contactreferenceid = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pcontactreferenceid) + "', nomineename = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pnomineename) + "', relationship = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].prelationship) + "', dateofbirth = " + (PersonalInformation.PersonalNomineeList[i].pdateofbirth) + ", contactno = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pcontactno) + "', idprooftype = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pidprooftype) + "', idproofname = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pidproofname) + "', referencenumber = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].preferencenumber) + "', docidproofpath = '" + ManageQuote(PersonalInformation.PersonalNomineeList[i].pdocidproofpath) + "', statusid = '" + getStatusid(PersonalInformation.PersonalNomineeList[i].pStatusname, ConnectionString) + "', modifiedby = '" + (PersonalInformation.pCreatedby) + "', modifieddate = current_timestamp,isprimarynominee=" + (PersonalInformation.PersonalNomineeList[i].pisprimarynominee) + " where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid = " + PersonalInformation.PersonalNomineeList[i].precordid + ";");

                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.Append("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.PersonalNomineeList.ToString()) || PersonalInformation.PersonalNomineeList.Count == 0 || (recordid1 == "" && PersonalInformation.PersonalNomineeList.Count > 0))
                    {

                        sbupdate.Append("UPDATE tabapplicationpersonalnomineedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                }
                recordid1 = string.Empty;
                if (PersonalInformation.PersonalBankList != null)
                {
                    //string recordid2 = string.Empty;
                    for (int i = 0; i < PersonalInformation.PersonalBankList.Count; i++)
                    {
                        if (PersonalInformation.PersonalBankList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = PersonalInformation.PersonalBankList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + PersonalInformation.PersonalBankList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.PersonalBankList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalBankList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalBankList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalBankList[i].ptypeofoperation = PersonalInformation.PersonalBankList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isbankdetailsapplicable='" + (PersonalInformation.PersonalBankList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalBankList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalBankList[i].ptypeofoperation == "CREATE" && !string.IsNullOrEmpty(PersonalInformation.PersonalBankList[i].pBankName))
                        {
                            if (PersonalInformation.PersonalBankList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalbankdetails(applicationid, vchapplicationid, contactid, contactreferenceid, bankname, accountno, ifsccode, branch, isprimarybank, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalBankList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.PersonalBankList[i].pcontactreferenceid) + "', '" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankName) + "', '" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankAccountNo) + "', '" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankifscCode) + "', '" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankBranch) + "', '" + (PersonalInformation.PersonalBankList[i].pIsprimaryAccount) + "', " + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalBankList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalBankList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.PersonalBankList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbankdetails set contactid='" + (PersonalInformation.PersonalBankList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalBankList[i].pcontactreferenceid) + "', bankname='" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankName) + "', accountno='" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankAccountNo) + "', ifsccode='" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankifscCode) + "', branch='" + ManageQuote(PersonalInformation.PersonalBankList[i].pBankBranch) + "', isprimarybank='" + (PersonalInformation.PersonalBankList[i].pIsprimaryAccount) + "', statusid='" + getStatusid(PersonalInformation.PersonalBankList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid =" + PersonalInformation.PersonalBankList[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.Append("UPDATE tabapplicationpersonalbankdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.PersonalBankList.ToString()) || PersonalInformation.PersonalBankList.Count == 0 || (recordid1 == "" && PersonalInformation.PersonalBankList.Count > 0))
                    {
                        sbupdate.Append("UPDATE tabapplicationpersonalbankdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                }

                if (PersonalInformation.PersonalIncomeList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalIncomeList.Count; i++)
                    {
                        if (PersonalInformation.PersonalIncomeList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalIncomeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalIncomeList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalIncomeList[i].ptypeofoperation = PersonalInformation.PersonalIncomeList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (PersonalInformation.PersonalIncomeList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalIncomeList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalIncomeList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, grossannualincome, netannualincome, averageannualexpenses, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalIncomeList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "', '" + (PersonalInformation.PersonalIncomeList[i].pgrossannualincome) + "', '" + (PersonalInformation.PersonalIncomeList[i].pnetannualincome) + "', '" + (PersonalInformation.PersonalIncomeList[i].paverageannualexpenses) + "', " + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalIncomeList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalIncomeList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalIncomeList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalIncomeList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalincomedetails set contactid='" + (PersonalInformation.PersonalIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "', grossannualincome='" + (PersonalInformation.PersonalIncomeList[i].pgrossannualincome) + "', netannualincome='" + (PersonalInformation.PersonalIncomeList[i].pnetannualincome) + "', averageannualexpenses='" + (PersonalInformation.PersonalIncomeList[i].paverageannualexpenses) + "', statusid='" + getStatusid(PersonalInformation.PersonalIncomeList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "';");
                        }
                    }
                }

                recordid1 = string.Empty;
                if (PersonalInformation.PersonalOtherIncomeList != null)
                {
                    //string recordid3 = string.Empty;
                    for (int i = 0; i < PersonalInformation.PersonalOtherIncomeList.Count; i++)
                    {
                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = PersonalInformation.PersonalOtherIncomeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + PersonalInformation.PersonalOtherIncomeList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.PersonalOtherIncomeList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalOtherIncomeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation = PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable == true && !string.IsNullOrEmpty(PersonalInformation.PersonalOtherIncomeList[i].psourcename))
                                sbQuery.Append("insert into tabapplicationpersonalotherincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, sourcename, grossannual, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactid) + "', '" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "', '" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].psourcename) + "', '" + PersonalInformation.PersonalOtherIncomeList[i].pgrossannual + "', " + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalotherincomedetails set contactid='" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "', sourcename='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].psourcename) + "', grossannual='" + PersonalInformation.PersonalOtherIncomeList[i].pgrossannual + "', statusid='" + getStatusid(PersonalInformation.PersonalOtherIncomeList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid =" + PersonalInformation.PersonalOtherIncomeList[i].precordid + ";");

                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    if (recordid1 == "0")
                    {
                        sbupdate.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                    else
                    {
                        sbupdate.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid1 + "); ");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.PersonalOtherIncomeList.ToString()) || PersonalInformation.PersonalOtherIncomeList.Count == 0 || (recordid1 == "" && PersonalInformation.PersonalOtherIncomeList.Count > 0))
                    {
                        sbupdate.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                }
                if (PersonalInformation.PersonalEducationList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalEducationList.Count; i++)
                    {
                        if (PersonalInformation.PersonalEducationList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalEducationList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalEducationList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalEducationList[i].ptypeofoperation = PersonalInformation.PersonalEducationList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set iseducationdetailsapplicable='" + (PersonalInformation.PersonalEducationList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalEducationList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalEducationList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalEducationList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonaleducationdetails(applicationid, vchapplicationid, contactid, contactreferenceid, qualification, nameofthecourseorprofession, occupation, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalEducationList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.PersonalEducationList[i].pcontactreferenceid) + "', '" + ManageQuote(PersonalInformation.PersonalEducationList[i].pqualification) + "', '" + ManageQuote(PersonalInformation.PersonalEducationList[i].pnameofthecourseorprofession) + "', '" + ManageQuote(PersonalInformation.PersonalEducationList[i].poccupation) + "', " + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalEducationList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalEducationList[i].ptypeofoperation == "UPDATE" || PersonalInformation.PersonalEducationList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.PersonalEducationList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonaleducationdetails set contactid='" + (PersonalInformation.PersonalEducationList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEducationList[i].pcontactreferenceid) + "', qualification='" + ManageQuote(PersonalInformation.PersonalEducationList[i].pqualification) + "', nameofthecourseorprofession='" + ManageQuote(PersonalInformation.PersonalEducationList[i].pnameofthecourseorprofession) + "', occupation='" + ManageQuote(PersonalInformation.PersonalEducationList[i].poccupation) + "', statusid='" + getStatusid(PersonalInformation.PersonalEducationList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEducationList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                if (PersonalInformation.BusinessDetailsDTOList != null)
                {
                    for (int i = 0; i < PersonalInformation.BusinessDetailsDTOList.Count; i++)
                    {
                        if (PersonalInformation.BusinessDetailsDTOList[i].pStatusname == null)
                        {
                            PersonalInformation.BusinessDetailsDTOList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation))
                        {
                            PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation = PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate))
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate = "null";
                        }
                        else
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate = "'" + FormatDate(PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate) + "'";
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate))
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate = "null";
                        }
                        else
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate = "'" + FormatDate(PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate) + "'";
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isbusinessactivitydetailsapplicable='" + (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("INSERT INTO tabapplicationpersonalbusinessdetails(applicationid, vchapplicationid, contactid, contactreferenceid,businessactivity, establishmentdate,commencementdate,cinnumber,gstinno, statusid, createdby, createddate,applicantype)VALUES ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.BusinessDetailsDTOList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pbusinessactivity) + "'," + PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate + ", " + PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate + ",'" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcinnumber) + "','" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pgstinno) + "'," + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].papplicanttype) + "');");

                        }
                        if (PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation == "UPDATE" || PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation == "OLD")
                        {
                            if (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbusinessdetails set contactid='" + (PersonalInformation.BusinessDetailsDTOList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "', businessactivity='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pbusinessactivity) + "', establishmentdate=" + PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate + ", commencementdate=" + PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate + ", cinnumber='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcinnumber) + "', gstinno='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pgstinno) + "', statusid='" + getStatusid(PersonalInformation.BusinessDetailsDTOList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "';");
                        }
                    }
                }
                recordid1 = string.Empty;
                if (PersonalInformation.businessfinancialdetailsDTOList != null)
                {
                    //string Recordid = string.Empty;
                    for (int i = 0; i < PersonalInformation.businessfinancialdetailsDTOList.Count; i++)
                    {
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid1))
                            {
                                recordid1 = PersonalInformation.businessfinancialdetailsDTOList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid1 = recordid1 + "," + PersonalInformation.businessfinancialdetailsDTOList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname == null)
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation = PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount.ToString()))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount = 0;
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount.ToString()))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount = 0;
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isfinancialperformanceapplicable='" + (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "';");


                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("INSERT INTO tabapplicationpersonalbusinessfinancialdetails(applicationid, vchapplicationid, contactid, contactreferenceid, financialyear, turnoveramount, netprofitamount, docbalancesheetpath,statusid, createdby, createddate,applicantype)VALUES ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.businessfinancialdetailsDTOList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pfinancialyear) + "',coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount + ", 0),coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount + ", 0),'" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pdocbalancesheetpath) + "'," + Convert.ToInt32(Status.Active) + ", '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].papplicanttype) + "');");

                        }
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbusinessfinancialdetails set contactid='" + (PersonalInformation.businessfinancialdetailsDTOList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "', financialyear='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pfinancialyear) + "', turnoveramount=coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount + ", 0), netprofitamount=coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount + ", 0), docbalancesheetpath='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pdocbalancesheetpath) + "',statusid='" + getStatusid(PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid =" + PersonalInformation.businessfinancialdetailsDTOList[i].precordid + ";");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recordid1))
                {
                    sbupdate.Append("UPDATE tabapplicationpersonalbusinessfinancialdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid1 + "); ");
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList.ToString()) || PersonalInformation.businessfinancialdetailsDTOList.Count == 0 || (recordid1 == "" && PersonalInformation.businessfinancialdetailsDTOList.Count > 0))
                    {
                        sbupdate.Append("UPDATE tabapplicationpersonalbusinessfinancialdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                }

                //if (!string.IsNullOrEmpty(PersonalInformation.pTypeofapplicant))
                //{
                //    if (PersonalInformation.pTypeofapplicant.Trim().ToUpper() == "MEMBER")
                //    {
                //        sbQuery.AppendLine();
                //    }
                //}

                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbupdate) + "" + sbQuery.ToString());
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

        public bool SaveApplicationPersonalLOANInformation(ApplicationPersonalInformationDTO PersonalInformation, string ConnectionString)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            StringBuilder sbUpdateQuery = new StringBuilder();
            string recordid = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (PersonalInformation.pvchapplicationid != null)
                {
                    PersonalInformation.papplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where    vchapplicationid  ='" + PersonalInformation.pvchapplicationid + "';"));
                }
                if (PersonalInformation.PersonalEmployeementList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalEmployeementList.Count; i++)
                    {
                        if (PersonalInformation.PersonalEmployeementList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalEmployeementList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalEmployeementList[i].ptypeofoperation = PersonalInformation.PersonalEmployeementList[i].ptypeofoperation.ToUpper();
                        }

                        if (string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].pdateofestablishment))
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofestablishment = "null";
                        }
                        else
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofestablishment = "'" + FormatDate(PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + "'";
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.PersonalEmployeementList[i].pdateofcommencement))
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofcommencement = "null";
                        }
                        else
                        {

                            PersonalInformation.PersonalEmployeementList[i].pdateofcommencement = "'" + FormatDate(PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + "'";
                        }
                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isemplymentdetailsapplicable='" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalEmployeementList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalEmployeementList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalemplymentdetails(applicationid,vchapplicationid,contactid,contactreferenceid,applicanttype,isemploymentapplicable,employmenttype,nameoftheorganization,natureoftheorganization,employmentrole,officeaddress,officephoneno,reportingto,employeeexp,employeeexptype,totalworkexp,dateofestablishment,dateofcommencement,gstinno,cinno,dinno,tradelicenseno,statusid,createdby,createddate)values('" + (PersonalInformation.papplicationid) + "','" + ManageQuote(PersonalInformation.pvchapplicationid) + "','" + (PersonalInformation.PersonalEmployeementList[i].pcontactid) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].papplicanttype) + "','" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymenttype) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pnameoftheorganization) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pEnterpriseType) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymentrole) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficeaddress) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficephoneno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].preportingto) + "','" + (PersonalInformation.PersonalEmployeementList[i].pemployeeexp) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemployeeexptype) + "','" + (PersonalInformation.PersonalEmployeementList[i].ptotalworkexp) + "'," + (PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + "," + (PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + ",'" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pgstinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pdinno) + "','" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].ptradelicenseno) + "','" + getStatusid(PersonalInformation.PersonalEmployeementList[i].pStatusname, ConnectionString) + "','" + (PersonalInformation.pCreatedby) + "',current_timestamp); ");
                        }
                        if (PersonalInformation.PersonalEmployeementList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.PersonalEmployeementList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalemplymentdetails set contactid='" + (PersonalInformation.PersonalEmployeementList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "', applicanttype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].papplicanttype) + "', isemploymentapplicable='" + (PersonalInformation.PersonalEmployeementList[i].pisapplicable) + "', employmenttype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymenttype) + "', nameoftheorganization='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pnameoftheorganization) + "', natureoftheorganization='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pEnterpriseType) + "', employmentrole='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemploymentrole) + "', officeaddress='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficeaddress) + "', officephoneno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pofficephoneno) + "', reportingto='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].preportingto) + "', employeeexp='" + (PersonalInformation.PersonalEmployeementList[i].pemployeeexp) + "', employeeexptype='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pemployeeexptype) + "', totalworkexp='" + (PersonalInformation.PersonalEmployeementList[i].ptotalworkexp) + "', dateofestablishment=" + (PersonalInformation.PersonalEmployeementList[i].pdateofestablishment) + ", dateofcommencement=" + (PersonalInformation.PersonalEmployeementList[i].pdateofcommencement) + ", gstinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pgstinno) + "', cinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcinno) + "', dinno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pdinno) + "', tradelicenseno='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].ptradelicenseno) + "', statusid='" + getStatusid(PersonalInformation.PersonalEmployeementList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalEmployeementList[i].pcontactreferenceid) + "' and applicanttype='Applicant';");


                        }
                    }
                }

                if (PersonalInformation.PersonalIncomeList != null)
                {
                    for (int i = 0; i < PersonalInformation.PersonalIncomeList.Count; i++)
                    {
                        if (PersonalInformation.PersonalIncomeList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalIncomeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalIncomeList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalIncomeList[i].ptypeofoperation = PersonalInformation.PersonalIncomeList[i].ptypeofoperation.ToUpper();
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (PersonalInformation.PersonalIncomeList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalIncomeList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalIncomeList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, grossannualincome, netannualincome, averageannualexpenses, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalIncomeList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "', '" + (PersonalInformation.PersonalIncomeList[i].pgrossannualincome) + "', '" + (PersonalInformation.PersonalIncomeList[i].pnetannualincome) + "', '" + (PersonalInformation.PersonalIncomeList[i].paverageannualexpenses) + "', '" + getStatusid(PersonalInformation.PersonalIncomeList[i].pStatusname, ConnectionString) + "', '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalIncomeList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalIncomeList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.PersonalIncomeList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalincomedetails set contactid='" + (PersonalInformation.PersonalIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "', grossannualincome='" + (PersonalInformation.PersonalIncomeList[i].pgrossannualincome) + "', netannualincome='" + (PersonalInformation.PersonalIncomeList[i].pnetannualincome) + "', averageannualexpenses='" + (PersonalInformation.PersonalIncomeList[i].paverageannualexpenses) + "', statusid='" + getStatusid(PersonalInformation.PersonalIncomeList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.PersonalIncomeList[i].pcontactreferenceid) + "' and applicantype='Applicant';");
                        }
                    }
                }
                recordid = string.Empty;
                if (PersonalInformation.PersonalOtherIncomeList != null)
                {

                    for (int i = 0; i < PersonalInformation.PersonalOtherIncomeList.Count; i++)
                    {
                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid))
                            {
                                recordid = PersonalInformation.PersonalOtherIncomeList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid = recordid + "," + PersonalInformation.PersonalOtherIncomeList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.PersonalOtherIncomeList[i].pStatusname == null)
                        {
                            PersonalInformation.PersonalOtherIncomeList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation))
                        {
                            PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation = PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation.ToUpper();
                        }
                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isincomedetailsapplicable='" + (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable == true)
                                sbQuery.Append("insert into tabapplicationpersonalotherincomedetails(applicationid, vchapplicationid, contactid, contactreferenceid, sourcename, grossannual, statusid, createdby, createddate,applicantype) values ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactid) + "', '" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "', '" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].psourcename) + "', '" + PersonalInformation.PersonalOtherIncomeList[i].pgrossannual + "', '" + getStatusid(PersonalInformation.PersonalOtherIncomeList[i].pStatusname, ConnectionString) + "', '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].papplicanttype) + "');");
                        }
                        if (PersonalInformation.PersonalOtherIncomeList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.PersonalOtherIncomeList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalotherincomedetails set contactid='" + (PersonalInformation.PersonalOtherIncomeList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].pcontactreferenceid) + "', sourcename='" + ManageQuote(PersonalInformation.PersonalOtherIncomeList[i].psourcename) + "', grossannual='" + PersonalInformation.PersonalOtherIncomeList[i].pgrossannual + "', statusid='" + getStatusid(PersonalInformation.PersonalOtherIncomeList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid =" + PersonalInformation.PersonalOtherIncomeList[i].precordid + "  and applicantype='Applicant';");

                        }
                    }

                }

                if (!string.IsNullOrEmpty(recordid))
                {
                    if (recordid == "0")
                    {
                        sbUpdateQuery.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "'; ");
                    }
                    else
                    {
                        sbUpdateQuery.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid + ") and applicantype='Applicant'; ");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.PersonalOtherIncomeList.ToString()) || PersonalInformation.PersonalOtherIncomeList.Count == 0 || (recordid == "" && PersonalInformation.PersonalOtherIncomeList.Count > 0))
                    {
                        sbUpdateQuery.Append("UPDATE tabapplicationpersonalotherincomedetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicantype='Applicant'; ");
                    }
                }
                if (PersonalInformation.BusinessDetailsDTOList != null)
                {
                    for (int i = 0; i < PersonalInformation.BusinessDetailsDTOList.Count; i++)
                    {
                        if (PersonalInformation.BusinessDetailsDTOList[i].pStatusname == null)
                        {
                            PersonalInformation.BusinessDetailsDTOList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation))
                        {
                            PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation = PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate))
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate = "null";
                        }
                        else
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate = "'" + FormatDate(PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate) + "'";
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate))
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate = "null";
                        }
                        else
                        {

                            PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate = "'" + FormatDate(PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate) + "'";
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isbusinessactivitydetailsapplicable='" + (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "';");

                        if (PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("INSERT INTO tabapplicationpersonalbusinessdetails(applicationid, vchapplicationid, contactid, contactreferenceid,businessactivity, establishmentdate,commencementdate,cinnumber,gstinno, statusid, createdby, createddate,applicantype)VALUES ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.BusinessDetailsDTOList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pbusinessactivity) + "'," + PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate + ", " + PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate + ",'" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcinnumber) + "','" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pgstinno) + "','" + getStatusid(PersonalInformation.BusinessDetailsDTOList[i].pStatusname, ConnectionString) + "', '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].papplicanttype) + "');");

                        }
                        if (PersonalInformation.BusinessDetailsDTOList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.BusinessDetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbusinessdetails set contactid='" + (PersonalInformation.BusinessDetailsDTOList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "', businessactivity='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pbusinessactivity) + "', establishmentdate=" + PersonalInformation.BusinessDetailsDTOList[i].pestablishmentdate + ", commencementdate=" + PersonalInformation.BusinessDetailsDTOList[i].pcommencementdate + ", cinnumber='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcinnumber) + "', gstinno='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pgstinno) + "', statusid='" + getStatusid(PersonalInformation.BusinessDetailsDTOList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and contactreferenceid='" + ManageQuote(PersonalInformation.BusinessDetailsDTOList[i].pcontactreferenceid) + "' and applicantype='Applicant';");
                        }
                    }
                }
                recordid = string.Empty;
                if (PersonalInformation.businessfinancialdetailsDTOList != null)
                {
                    for (int i = 0; i < PersonalInformation.businessfinancialdetailsDTOList.Count; i++)
                    {
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid))
                            {
                                recordid = PersonalInformation.businessfinancialdetailsDTOList[i].precordid.ToString();
                            }
                            else
                            {
                                recordid = recordid + "," + PersonalInformation.businessfinancialdetailsDTOList[i].precordid.ToString();
                            }
                        }
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname == null)
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname = "ACTIVE";
                        }
                        if (!string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation = PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation.ToUpper();
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount.ToString()))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount = 0;
                        }
                        if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount.ToString()))
                        {
                            PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount = 0;
                        }

                        sbQuery.Append("update tabapplicationpersonalapplicablesections set isfinancialperformanceapplicable='" + (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "';");


                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation == "CREATE")
                        {
                            if (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("INSERT INTO tabapplicationpersonalbusinessfinancialdetails(applicationid, vchapplicationid, contactid, contactreferenceid, financialyear, turnoveramount, netprofitamount, docbalancesheetpath,statusid, createdby, createddate,applicantype)VALUES ('" + (PersonalInformation.papplicationid) + "', '" + ManageQuote(PersonalInformation.pvchapplicationid) + "', '" + (PersonalInformation.businessfinancialdetailsDTOList[i].pcontactid) + "', '" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "','" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pfinancialyear) + "',coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount + ", 0),coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount + ", 0),'" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pdocbalancesheetpath) + "','" + getStatusid(PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname, ConnectionString) + "', '" + (PersonalInformation.pCreatedby) + "', current_timestamp,'" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].papplicanttype) + "');");

                        }
                        if (PersonalInformation.businessfinancialdetailsDTOList[i].ptypeofoperation == "UPDATE")
                        {
                            if (PersonalInformation.businessfinancialdetailsDTOList[i].pisapplicable == true)
                                sbQuery.Append("update tabapplicationpersonalbusinessfinancialdetails set contactid='" + (PersonalInformation.businessfinancialdetailsDTOList[i].pcontactid) + "', contactreferenceid='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "', financialyear='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pfinancialyear) + "', turnoveramount=coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pturnoveramount + ", 0), netprofitamount=coalesce(" + PersonalInformation.businessfinancialdetailsDTOList[i].pnetprofitamount + ", 0), docbalancesheetpath='" + ManageQuote(PersonalInformation.businessfinancialdetailsDTOList[i].pdocbalancesheetpath) + "',statusid='" + getStatusid(PersonalInformation.businessfinancialdetailsDTOList[i].pStatusname, ConnectionString) + "', modifiedby='" + (PersonalInformation.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicationid = " + (PersonalInformation.papplicationid) + " and recordid =" + PersonalInformation.businessfinancialdetailsDTOList[i].precordid + " and applicantype='Applicant';");
                        }
                    }

                }
                if (!string.IsNullOrEmpty(recordid))
                {
                    sbUpdateQuery.Append("UPDATE tabapplicationpersonalbusinessfinancialdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' AND RECORDID not in(" + recordid + ") and applicantype='Applicant'; ");
                }
                else
                {
                    if (string.IsNullOrEmpty(PersonalInformation.businessfinancialdetailsDTOList.ToString()) || PersonalInformation.businessfinancialdetailsDTOList.Count == 0 || (recordid == "" && PersonalInformation.businessfinancialdetailsDTOList.Count > 0))
                    {
                        sbUpdateQuery.Append("UPDATE tabapplicationpersonalbusinessfinancialdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + PersonalInformation.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + PersonalInformation.papplicationid + " and vchapplicationid='" + ManageQuote(PersonalInformation.pvchapplicationid) + "' and applicantype='Applicant'; ");
                    }
                }
                if (!string.IsNullOrEmpty(sbQuery.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbUpdateQuery) + "" + sbQuery.ToString());
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
        public ApplicationPersonalInformationDTO GetApplicationPersonalInformation(string strapplictionid, string ConnectionString)
        {
            ds = new DataSet();
            bool ispersonaldetailsapplicable = false;
            ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO = new ApplicationPersonalInformationDTO();
            ApplicationPersonalInformationDTO.PersonalEmployeementList = new List<ApplicationPersonalEmployeementDTO>();
            ApplicationPersonalInformationDTO.PersonalDetailsList = new List<ApplicationPersonalDetailsDTO>();
            ApplicationPersonalInformationDTO.PersonalFamilyList = new List<ApplicationPersonalFamilyDTO>();
            ApplicationPersonalInformationDTO.PersonalNomineeList = new List<ApplicationPersonalNomineeDTO>();
            ApplicationPersonalInformationDTO.PersonalBankList = new List<ApplicationPersonalBankDTO>();
            ApplicationPersonalInformationDTO.PersonalIncomeList = new List<ApplicationPersonalIncomeDTO>();
            ApplicationPersonalInformationDTO.PersonalOtherIncomeList = new List<ApplicationPersonalOtherIncomeDTO>();
            ApplicationPersonalInformationDTO.PersonalEducationList = new List<ApplicationPersonalEducationDTO>();
            ApplicationPersonalInformationDTO.BusinessDetailsDTOList = new List<BusinessDetailsDTO>();
            ApplicationPersonalInformationDTO.businessfinancialdetailsDTOList = new List<businessfinancialdetailsDTO>();
            try
            {
                long Applicationcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tabapplication where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';"));
                if (Applicationcount > 0)
                {
                    ispersonaldetailsapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select ispersonaldetailsapplicable from tabapplication where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';"));
                }
                else
                {
                    long Membercount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstmembers where upper(membercode) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';"));
                    if (Membercount > 0)
                    {
                        ispersonaldetailsapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select ispersonaldetailsapplicable from tblmstmembers where upper(membercode) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';"));
                    }
                }


                ApplicationPersonalInformationDTO.pIspersonaldetailsapplicable = ispersonaldetailsapplicable;
                ApplicationPersonalInformationDTO.pvchapplicationid = strapplictionid;

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid,ti.applicationid,ti.contactid,ti.contactreferenceid,coalesce(applicanttype,'') as applicanttype, coalesce(isemploymentapplicable,false) as isemploymentapplicable, coalesce(employmenttype,'') as employmenttype,coalesce(nameoftheorganization,'') as nameoftheorganization,coalesce(natureoftheorganization,'') as natureoftheorganization,coalesce( employmentrole,'') as employmentrole,coalesce( officeaddress,'') as officeaddress, coalesce(officephoneno,'') as officephoneno,coalesce(reportingto,'') as reportingto,coalesce( employeeexp,0) as employeeexp,coalesce( employeeexptype,'') as employeeexptype, coalesce(totalworkexp,0) as totalworkexp,coalesce( dateofestablishment,null) as dateofestablishment,coalesce(dateofcommencement,null) as dateofcommencement, coalesce(gstinno,'') as gstinno,coalesce(cinno,'') as cinno,coalesce(dinno,'') as dinno,coalesce(tradelicenseno,'') as  tradelicenseno,ta.isemplymentdetailsapplicable  FROM tabapplicationpersonalemplymentdetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid=ta.vchapplicationid and ti.contactid=ta.contactid  where  upper(ti.vchapplicationid) ='" + ManageQuote(strapplictionid).ToUpper() + "' and ti.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalEmployeementList.Add(new ApplicationPersonalEmployeementDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            papplicanttype = dr["applicanttype"].ToString(),
                            pisapplicable = Convert.ToBoolean(dr["isemplymentdetailsapplicable"]),
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
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, ti.applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(residentialstatus, '') as residentialstatus, coalesce(maritalstatus, '') as maritalstatus, coalesce(placeofbirth, '') as placeofbirth, coalesce(countryofbirth, '') as countryofbirth, coalesce(nationality, '') as nationality, coalesce(minoritycommunity, '') as minoritycommunity,ispersonalbirthdetailsapplicable FROM tabapplicationpersonalbirthdetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid).ToUpper() + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalDetailsList.Add(new ApplicationPersonalDetailsDTO
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
                            pisapplicable = Convert.ToBoolean(dr["ispersonalbirthdetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text,
                    "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(totalnoofmembers, 0) as totalnoofmembers, coalesce(noofearningmembers, 0) as noofearningmembers, coalesce(familytype, '') as familytype, coalesce(noofboyschild, 0) as noofboyschild, coalesce(noofgirlchild, 0) as noofgirlchild, coalesce(houseownership, '') as houseownership, isfamilydetailsapplicable FROM tabapplicationpersonalfamilydetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid  where  upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalFamilyList.Add(new ApplicationPersonalFamilyDTO
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
                            pisapplicable = Convert.ToBoolean(dr["isfamilydetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(nomineename, '') as nomineename, coalesce(relationship, '') as relationship, coalesce(dateofbirth, null) as dateofbirth, coalesce(contactno, '') as contactno, coalesce(idprooftype, '') as idprooftype, coalesce(idproofname, '') as idproofname, coalesce(referencenumber, '') as referencenumber, coalesce(docidproofpath, '') as docidproofpath, coalesce(isprimarynominee, false) as isprimarynominee, isnomineedetailsapplicable FROM tabapplicationpersonalnomineedetails  ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where  upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalNomineeList.Add(new ApplicationPersonalNomineeDTO
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
                            pisapplicable = Convert.ToBoolean(dr["isnomineedetailsapplicable"])

                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, bankname, coalesce(accountno, '') as accountno, coalesce(ifsccode, '') as ifsccode, coalesce(branch, '') as branch, coalesce(isprimarybank, false) as isprimarybank, isbankdetailsapplicable FROM tabapplicationpersonalbankdetails  ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where  upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalBankList.Add(new ApplicationPersonalBankDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            papplicanttype = dr["applicantype"].ToString(),
                            //  pBankAccountname = dr["accountno"].ToString(),
                            pBankName = dr["bankname"].ToString(),
                            pBankAccountNo = dr["accountno"].ToString(),
                            pBankifscCode = dr["ifsccode"].ToString(),
                            pBankBranch = dr["branch"].ToString(),
                            pIsprimaryAccount = Convert.ToBoolean(dr["isprimarybank"].ToString()),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["isbankdetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(grossannualincome, 0) as grossannualincome, coalesce(netannualincome, 0) as netannualincome, coalesce(averageannualexpenses, 0) as averageannualexpenses, isincomedetailsapplicable FROM tabapplicationpersonalincomedetails  ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where  upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalIncomeList.Add(new ApplicationPersonalIncomeDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            papplicanttype = dr["applicantype"].ToString(),
                            pgrossannualincome = Convert.ToDecimal(dr["grossannualincome"]),
                            pnetannualincome = Convert.ToDecimal(dr["netannualincome"]),
                            paverageannualexpenses = Convert.ToDecimal(dr["averageannualexpenses"]),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["isincomedetailsapplicable"])

                        }); ;
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(sourcename, '') as sourcename, coalesce(grossannual, 0) as grossannual, isincomedetailsapplicable FROM tabapplicationpersonalotherincomedetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalOtherIncomeList.Add(new ApplicationPersonalOtherIncomeDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            papplicanttype = dr["applicantype"].ToString(),
                            psourcename = dr["sourcename"].ToString(),
                            pgrossannual = Convert.ToDecimal(dr["grossannual"]),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["isincomedetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid,applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid,coalesce(qualification,'') as qualification, coalesce(nameofthecourseorprofession,'') as nameofthecourseorprofession, coalesce(occupation,'') as occupation,iseducationdetailsapplicable FROM tabapplicationpersonaleducationdetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid=ta.vchapplicationid and ti.contactid=ta.contactid where  upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalEducationList.Add(new ApplicationPersonalEducationDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            papplicanttype = dr["applicantype"].ToString(),

                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pqualification = dr["qualification"].ToString(),
                            pnameofthecourseorprofession = dr["nameofthecourseorprofession"].ToString(),
                            poccupation = dr["occupation"].ToString(),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["iseducationdetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(businessactivity, '') as businessactivity, coalesce(establishmentdate, null) as establishmentdate, coalesce(commencementdate, null) as commencementdate, coalesce(cinnumber, '') as cinnumber, coalesce(gstinno, '') as gstinno, isbusinessactivitydetailsapplicable FROM tabapplicationpersonalbusinessdetails ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.BusinessDetailsDTOList.Add(new BusinessDetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            papplicanttype = dr["applicantype"].ToString(),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pbusinessactivity = dr["businessactivity"].ToString(),
                            pestablishmentdate = dr["establishmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["establishmentdate"]).ToString("dd/MM/yyyy"),
                            pcommencementdate = dr["commencementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["commencementdate"]).ToString("dd/MM/yyyy"),
                            pcinnumber = dr["cinnumber"].ToString(),
                            pgstinno = dr["gstinno"].ToString(),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["isbusinessactivitydetailsapplicable"])
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ti.recordid, ti.applicationid, applicantype, ti.vchapplicationid, ti.contactid, ti.contactreferenceid, coalesce(financialyear, '') as financialyear, coalesce(turnoveramount, 0) as turnoveramount, coalesce(netprofitamount, 0) as netprofitamount, coalesce(docbalancesheetpath, '') as docbalancesheetpath,isfinancialperformanceapplicable FROM tabapplicationpersonalbusinessfinancialdetails  ti join tabapplicationpersonalapplicablesections ta on ti.vchapplicationid = ta.vchapplicationid and ti.contactid = ta.contactid where upper(ti.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and ti.statusid = " + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.businessfinancialdetailsDTOList.Add(new businessfinancialdetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            papplicanttype = dr["applicantype"].ToString(),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pfinancialyear = dr["financialyear"].ToString(),
                            pturnoveramount = Convert.ToDecimal(dr["turnoveramount"].ToString()),
                            pnetprofitamount = Convert.ToDecimal(dr["netprofitamount"].ToString()),
                            pdocbalancesheetpath = dr["docbalancesheetpath"].ToString(),
                            ptypeofoperation = "OLD",
                            pisapplicable = Convert.ToBoolean(dr["isfinancialperformanceapplicable"])
                        });
                    }
                }



            }
            catch (Exception)
            {

                throw;
            }
            return ApplicationPersonalInformationDTO;

        }

        public ApplicationPersonalInformationDTO GetApplicationPersonalLoanInformation(string strapplictionid, string ConnectionString)
        {
            ds = new DataSet();
            bool ispersonaldetailsapplicable = false;
            ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO = new ApplicationPersonalInformationDTO();
            ApplicationPersonalInformationDTO.PersonalEmployeementList = new List<ApplicationPersonalEmployeementDTO>();
            ApplicationPersonalInformationDTO.PersonalIncomeList = new List<ApplicationPersonalIncomeDTO>();
            ApplicationPersonalInformationDTO.PersonalOtherIncomeList = new List<ApplicationPersonalOtherIncomeDTO>();
            ApplicationPersonalInformationDTO.BusinessDetailsDTOList = new List<BusinessDetailsDTO>();
            ApplicationPersonalInformationDTO.businessfinancialdetailsDTOList = new List<businessfinancialdetailsDTO>();

            try
            {
                ispersonaldetailsapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select ispersonaldetailsapplicable from tabapplication where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';"));
                ApplicationPersonalInformationDTO.pIspersonaldetailsapplicable = ispersonaldetailsapplicable;
                ApplicationPersonalInformationDTO.pvchapplicationid = strapplictionid;

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,contactid,contactreferenceid,coalesce(applicanttype,'') as applicanttype, coalesce(isemploymentapplicable,false) as isemploymentapplicable, coalesce(employmenttype,'') as employmenttype,coalesce(nameoftheorganization,'') as nameoftheorganization,coalesce(natureoftheorganization,'') as natureoftheorganization,coalesce( employmentrole,'') as employmentrole,coalesce( officeaddress,'') as officeaddress, coalesce(officephoneno,'') as officephoneno,coalesce(reportingto,'') as reportingto,coalesce( employeeexp,0) as employeeexp,coalesce( employeeexptype,'') as employeeexptype, coalesce(totalworkexp,0) as totalworkexp,coalesce( dateofestablishment,null) as dateofestablishment,coalesce(dateofcommencement,null) as dateofcommencement, coalesce(gstinno,'') as gstinno,coalesce(cinno,'') as cinno,coalesce(dinno,'') as dinno,coalesce(tradelicenseno,'') as  tradelicenseno FROM tabapplicationpersonalemplymentdetails where  upper(vchapplicationid) ='" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and applicanttype='Applicant';"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalEmployeementList.Add(new ApplicationPersonalEmployeementDTO
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
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid,coalesce(grossannualincome,0) as grossannualincome,coalesce(netannualincome,0) as netannualincome, coalesce(averageannualexpenses,0) as averageannualexpenses FROM tabapplicationpersonalincomedetails where  upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + " and applicantype='Applicant';"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalIncomeList.Add(new ApplicationPersonalIncomeDTO
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
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,applicantype,vchapplicationid,contactid,contactreferenceid,coalesce(sourcename,'') as sourcename,coalesce(grossannual,0) as grossannual FROM tabapplicationpersonalotherincomedetails where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + " and applicantype='Applicant'; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalOtherIncomeList.Add(new ApplicationPersonalOtherIncomeDTO
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
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype, vchapplicationid, contactid, contactreferenceid, coalesce(businessactivity,'') as businessactivity, coalesce(establishmentdate,null) as establishmentdate, coalesce(commencementdate,null) as commencementdate, coalesce(cinnumber,'') as cinnumber, coalesce(gstinno,'') as gstinno FROM tabapplicationpersonalbusinessdetails where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + " and applicantype='Applicant'; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.BusinessDetailsDTOList.Add(new BusinessDetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            papplicanttype = dr["applicantype"].ToString(),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pbusinessactivity = dr["businessactivity"].ToString(),
                            pestablishmentdate = dr["establishmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["establishmentdate"]).ToString("dd/MM/yyyy"),
                            pcommencementdate = dr["commencementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["commencementdate"]).ToString("dd/MM/yyyy"),
                            pcinnumber = dr["cinnumber"].ToString(),
                            pgstinno = dr["gstinno"].ToString(),
                            ptypeofoperation = "OLD",
                            pisapplicable = true
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,applicantype,vchapplicationid, contactid, contactreferenceid, coalesce(financialyear,'') as financialyear, coalesce(turnoveramount,0) as turnoveramount, coalesce(netprofitamount,0) as netprofitamount, coalesce(docbalancesheetpath,'') as docbalancesheetpath FROM tabapplicationpersonalbusinessfinancialdetails where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and statusid = " + Convert.ToInt32(Status.Active) + " and applicantype='Applicant';"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.businessfinancialdetailsDTOList.Add(new businessfinancialdetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            papplicanttype = dr["applicantype"].ToString(),
                            pcontactreferenceid = dr["contactreferenceid"].ToString(),
                            pfinancialyear = dr["financialyear"].ToString(),
                            pturnoveramount = Convert.ToDecimal(dr["turnoveramount"].ToString()),
                            pnetprofitamount = Convert.ToDecimal(dr["netprofitamount"].ToString()),
                            pdocbalancesheetpath = dr["docbalancesheetpath"].ToString(),
                            ptypeofoperation = "OLD",
                            pisapplicable = true
                        });
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }
            return ApplicationPersonalInformationDTO;

        }

        public bool SendAcknowledgementDetails(string con, AcknowledgementDTO acknowlwdgement)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
