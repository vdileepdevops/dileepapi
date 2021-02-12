using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaRepository.Interfaces.Settings;
using FinstaInfrastructure.Settings;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Settings
{
    public class EmployeeDAL : SettingsDAL, IEmployee
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        public List<EmployeeDTO> employeeList = null;
        List<EmployeeBankDetails> employeeBankdetailsList = null;
        List<familyDetailsDTO> employeeFamilydetailsList = null;
        EmployeeDTO objEmployeeDetails = new EmployeeDTO();

        #region Employee     
        public bool SaveEmployeeRole(EmployeeRole modelRole, string connectionString)
        {
            int savedCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(modelRole.pEmployeeRoleName))
                {
                    savedCount = NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "insert into tblmstemployeerole(rolename,statusid,createdby,createddate) values('" + ManageQuote(modelRole.pEmployeeRoleName) + "'," + Convert.ToInt32(Status.Active) + "," + modelRole.pCreatedby + ",current_timestamp);");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                }
            }
            return savedCount > 0 ? true : false;
        }
        public bool SaveEmployeeDetails(EmployeeDTO employeeDetails, string connectionString)
        {
            int EmployeeSaveCount = 0;
            long employeeId;
            StringBuilder saveEmployee = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                // Master Data

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  =" + employeeDetails.pContactId));
                employeeDetails.pEmployeeName = contactdetails.Split('@')[0];
                employeeDetails.pEmployeeSurName = contactdetails.Split('@')[1];

                employeeId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstemployee(contactid,titlename,name,surname,statusid,createdby,createddate) values(" + employeeDetails.pContactId + ",'" + ManageQuote(employeeDetails.pEmployeeTitleName).Trim() + "','" + ManageQuote(employeeDetails.pEmployeeName).Trim() + "','" + ManageQuote(employeeDetails.pEmployeeSurName).Trim() + "'," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp) returning employeeid;"));


                // Employement Details
                if (string.IsNullOrEmpty(employeeDetails.pEmploymentJoiningDate))
                {
                    employeeDetails.pEmploymentJoiningDate = "null";
                }
                else
                {
                    employeeDetails.pEmploymentJoiningDate = "'" + FormatDate(employeeDetails.pEmploymentJoiningDate) + "'";
                }
                employeeDetails.pEmploymentBasicSalary = Convert.ToString(employeeDetails.pEmploymentBasicSalary) == string.Empty ? 0 : employeeDetails.pEmploymentBasicSalary < 0 ? 0 : employeeDetails.pEmploymentBasicSalary;
                employeeDetails.pEmploymentAllowanceORvda = Convert.ToString(employeeDetails.pEmploymentAllowanceORvda) == string.Empty ? 0 : employeeDetails.pEmploymentAllowanceORvda < 0 ? 0 : employeeDetails.pEmploymentAllowanceORvda;
                employeeDetails.pEmploymentCTC = Convert.ToString(employeeDetails.pEmploymentCTC) == string.Empty ? 0 : employeeDetails.pEmploymentCTC < 0 ? 0 : employeeDetails.pEmploymentCTC;


                // KYC Details
                //if (employeeDetails.pListEmpKYC != null && employeeDetails.pListEmpKYC.Count > 0)
                //{
                //    foreach (documentstoreDTO kycDetails in employeeDetails.pListEmpKYC)
                //    {
                //        if (!string.IsNullOrEmpty(kycDetails.pDocumentReferenceMonth) && !string.IsNullOrEmpty(kycDetails.pDocumentReferenceYear))
                //        {
                //            kycDetails.pDocReferenceno = kycDetails.pDocReferenceno + "~" + kycDetails.pDocumentReferenceMonth + "~" + kycDetails.pDocumentReferenceYear;
                //        }
                //        else if (!string.IsNullOrEmpty(kycDetails.pDocumentReferenceMonth))
                //        {
                //            kycDetails.pDocReferenceno = kycDetails.pDocReferenceno + "~" + kycDetails.pDocumentReferenceMonth;
                //        }
                //        else if (!string.IsNullOrEmpty(kycDetails.pDocumentReferenceYear))
                //        {
                //            kycDetails.pDocReferenceno = kycDetails.pDocReferenceno + "~" + kycDetails.pDocumentReferenceYear;
                //        }
                //        else
                //        {
                //            kycDetails.pDocReferenceno = kycDetails.pDocReferenceno.Trim();
                //        }

                //        saveEmployee.Append("update tabapplicationkyccreditdetailsapplicablesections set iskycdocumentsdetailsapplicable='" + (kycDetails.pisapplicable) + "', modifiedby=" + (kycDetails.pCreatedby) + ", modifieddate=current_timestamp where applicationid = " + 0 + " and  contactid =" + kycDetails.pContactId + ";");
                //        if (!string.IsNullOrEmpty(kycDetails.pDocStorePath) && kycDetails.pDocStorePath.Contains('.'))
                //        {
                //            string strext = kycDetails.pDocStorePath.Substring(kycDetails.pDocStorePath.LastIndexOf('.') + 1);
                //            if (!string.IsNullOrEmpty(strext))
                //            {
                //                kycDetails.pDocFileType = strext.ToString();
                //            }
                //        }
                //        Int64 count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(docstoreid) from tblmstdocumentstore where contactid=" + kycDetails.pContactId + " and documentid=" + kycDetails.pDocumentId + " and documentgroupid=" + kycDetails.pDocumentGroupId + " and coalesce(loanid,0)=0"));
                //        if (count == 0)
                //        {
                //            saveEmployee.Append("insert into tblmstdocumentstore(contactid,documentid,documentname,documentgroupid,documentgroupname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate) values(" + employeeDetails.pContactId + "," + kycDetails.pDocumentId + ",'" + ManageQuote(kycDetails.pDocumentName) + "'," + kycDetails.pDocumentGroupId + ",'" + ManageQuote(kycDetails.pDocumentGroup) + "','" + ManageQuote(kycDetails.pDocStorePath) + "','" + ManageQuote(kycDetails.pDocFileType) + "','" + ManageQuote(kycDetails.pDocReferenceno).Trim() + "','" + kycDetails.pDocIsDownloadable + "'," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp);");
                //        }
                //        else
                //        {
                //            if (kycDetails.pisapplicable == true)
                //                saveEmployee.Append("UPDATE tblmstdocumentstore SET   applicationno=" + 0 + ",contacttype='" + ManageQuote(kycDetails.pContactType) + "', documentid=" + kycDetails.pDocumentId + ", documentgroupid=" + kycDetails.pDocumentGroupId + ", documentgroupname='" + ManageQuote(kycDetails.pDocumentGroup) + "', documentname='" + ManageQuote(kycDetails.pDocumentName) + "', docstorepath='" + ManageQuote(kycDetails.pDocStorePath) + "', docfiletype='" + ManageQuote(kycDetails.pDocFileType) + "', docreferenceno='" + ManageQuote(kycDetails.pDocReferenceno) + "', docisdownloadable=" + kycDetails.pDocIsDownloadable + ", modifiedby=" + kycDetails.pCreatedby + ", modifieddate=current_timestamp,filename='" + ManageQuote(kycDetails.pFilename) + "' WHERE contactid=" + kycDetails.pContactId + " and docstoreid=" + kycDetails.pDocstoreId + ";");
                //        }
                //    }
                //}

                // End KYC



                if (employeeDetails.pListEmpKYC != null && employeeDetails.pListEmpKYC.Count > 0)
                {
                    ReferralAdvocateDAL objReferralAdvocateDAL = new ReferralAdvocateDAL();
                    saveEmployee.Append(objReferralAdvocateDAL.UpdateStoreDetails(employeeDetails.pListEmpKYC, connectionString, 0, employeeDetails.pContactId));
                }

                // Employement Details
                string roleid = Convert.ToString(employeeDetails.pEmploymentRoleId);
                if (employeeDetails.pEmploymentRoleId == 0 || employeeDetails.pEmploymentRoleId==null)
                {
                    roleid = "null";
                }
                saveEmployee.Append("insert into tblmstemployeeemploymentdetails(employeeid,roleid,rolename,designation,dateofjoining,basicsalary,allowanceorvariablepay,totalcosttocompany,statusid,createdby,createddate) values(" + employeeId + "," + roleid + ",'" + ManageQuote(employeeDetails.pEmploymentRoleName) + "','" + ManageQuote(employeeDetails.pEmploymentDesignation) + "'," + (employeeDetails.pEmploymentJoiningDate) + "," + employeeDetails.pEmploymentBasicSalary + "," + employeeDetails.pEmploymentAllowanceORvda + "," + employeeDetails.pEmploymentCTC + "," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp);");
                // Personal Details
                saveEmployee.Append("insert into tblmstemployeepersonalbirthdetails(employeeid,residentialstatus,maritalstatus,placeofbirth,countryofbirth,nationality,minoritycommunity,statusid,createdby,createddate) values(" + employeeId + ",'" + ManageQuote(employeeDetails.presidentialstatus) + "','" + ManageQuote(employeeDetails.pmaritalstatus) + "','" + ManageQuote(employeeDetails.pplaceofbirth).Trim() + "','" + ManageQuote(employeeDetails.pcountryofbirth).Trim() + "','" + ManageQuote(employeeDetails.pnationality) + "','" + ManageQuote(employeeDetails.pminoritycommunity) + "'," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp);");

                if (employeeDetails.pListFamilyDetails != null && employeeDetails.pListFamilyDetails.Count > 0)
                {
                    foreach (familyDetailsDTO familyDetails in employeeDetails.pListFamilyDetails)
                    {
                        // Personal Details
                        familyDetails.pTotalnoofmembers = Convert.ToString(familyDetails.pTotalnoofmembers) == string.Empty ? 0 : familyDetails.pTotalnoofmembers < 0 ? 0 : familyDetails.pTotalnoofmembers;
                        saveEmployee.Append("insert into tblmstemployeepersonalfamilydetails(employeeid,totalnoofmembers,contactpersonname,relationwithemployee,contactnumber,statusid,createdby,createddate) values(" + employeeId + "," + familyDetails.pTotalnoofmembers + ",'" + familyDetails.pContactpersonname+ "','" + familyDetails.pRelationwithemployee+ "','" + familyDetails.pContactnumber+ "'," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp);");
                    }
                }
                // Bank Details
                if (employeeDetails.pListEmpBankDetails != null && employeeDetails.pListEmpBankDetails.Count > 0)
                {
                    foreach (EmployeeBankDetails bankDetails in employeeDetails.pListEmpBankDetails)
                    {
                        saveEmployee.Append("insert into tblmstemployeebankdetails(employeeid,bankaccountname,bankname,bankaccountno,bankifsccode,bankbranch,isprimaryaccount,statusid,createdby,createddate) values(" + employeeId + ",'" + ManageQuote(bankDetails.pBankAccountname).Trim() + "','" + ManageQuote(bankDetails.pBankName).Trim() + "','" + ManageQuote(bankDetails.pBankAccountNo).Trim() + "','" + ManageQuote(bankDetails.pBankifscCode).Trim() + "','" + ManageQuote(bankDetails.pBankBranch).Trim() + "','" + bankDetails.pIsprimaryAccount + "'," + Convert.ToInt32(Status.Active) + "," + employeeDetails.pCreatedby + ",current_timestamp);");
                    }
                }
                if (Convert.ToString(saveEmployee) != string.Empty)
                {
                    EmployeeSaveCount = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, saveEmployee.ToString());
                    trans.Commit();
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
            return EmployeeSaveCount > 0 ? true : false;
        }
        public async Task<List<EmployeeDTO>> GetallEmployeeDetails(string connectionString)
        {
            await Task.Run(() =>
             {
                 employeeList = new List<EmployeeDTO>();
                 try
                 {
                     //using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select te.employeeid,coalesce(name,'')||' '||coalesce(surname,'') as name,surname,designation,rolename,contactnumber,emailid,contactreferenceid from tblmstemployee te left join tblmstemployeeemploymentdetails temp on te.employeeid=temp.employeeid left join tblmstcontactpersondetails tc on tc.contactid = te.contactid where te.statusid="+Convert.ToInt32(Status.Active)+"  and  upper(priority) = 'PRIMARY' order by name "))
                     using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select  te.tbl_mst_employee_id as employeeid,coalesce(tc.name,'')||' '||coalesce(tc.surname,'') as name,tc.surname,td.designation_name as designation,tr.rolename,tc.businessentitycontactno as contactnumber,tc.businessentityemailid as emailid,tc.contactreferenceid from tbl_mst_employee te join tblmstcontact tc on te.contact_id=tc.contactid left join tbl_mst_designation td on td.tbl_mst_designation_id=te.designation_id join tblmstemployeerole tr on tr.roleid=te.role_id where te.status='t' order by tc.name "))
                     {
                         while (dataReader.Read())
                         {
                             EmployeeDTO employeeObj = new EmployeeDTO
                             {
                                 pEmployeeId = Convert.ToInt64(dataReader["employeeid"]),
                                 pEmployeeName = Convert.ToString(dataReader["name"]),
                                 pEmployeeSurName = Convert.ToString(dataReader["surname"]),
                                 pEmploymentDesignation = Convert.ToString(dataReader["designation"]),
                                 pEmploymentRoleName = Convert.ToString(dataReader["rolename"]),
                                 pEmployeeContactNo = Convert.ToString(dataReader["contactnumber"]),
                                 pEmployeeContactEmail = Convert.ToString(dataReader["emailid"]),
                                 pContactRefNo = Convert.ToString(dataReader["contactreferenceid"])
                             };
                             employeeList.Add(employeeObj);
                         }
                     }
                 }
                 catch (Exception)
                 {
                     throw;
                 }
                 finally
                 {
                     if (con.State == ConnectionState.Open)
                     {
                         con.Dispose();
                         con.Close();
                         con.ClearPool();
                     }
                 }

             });
            return employeeList;
        }
        public int checkEmployeeCountinMaster(EmployeeDTO employee, string connectionString)
        {
            try
            {
                return employee.pContactId > 0 ? Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstemployee where contactid=" + employee.pContactId + " and statusid = " + Convert.ToInt32(Status.Active) + ";")) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<EmployeeDTO> GetEmployeeDetailsOnId(long employeeId, string connectionString)
        {
            await Task.Run(() =>
            {
                objEmployeeDetails = new EmployeeDTO();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select te.employeeid,te.contactid, te.name||' '||te.surname as employeename,tcp.contactnumber,tcp.contactreferenceid,coalesce( basicsalary,0) as basicsalary,coalesce(allowanceorvariablepay, 0) as allowanceorvariablepay, coalesce(totalcosttocompany, 0) as totalcosttocompany, designation, rolename,coalesce( roleid,0) as roleid,  dateofjoining, residentialstatus,maritalstatus,placeofbirth,countryofbirth,nationality,minoritycommunity from tblmstemployee te join tblmstcontact tc on te.contactid=tc.contactid join tblmstcontactpersondetails tcp on tcp.contactid=te.contactid left join tblmstemployeeemploymentdetails tmp on tmp.employeeid=te.employeeid left join tblmstemployeepersonalbirthdetails tp on tp.employeeid=te.employeeid where te.statusid = " + Convert.ToInt32(Status.Active) + "  and te.employeeid=" + employeeId + " and tcp.priority='PRIMARY' order by employeeid, employeename;"))
                    {
                        while (dr.Read())
                        {
                            // EmployeeDTO objEmployeeDetails = new EmployeeDTO();
                            objEmployeeDetails.pEmployeeId = Convert.ToInt64(dr["employeeid"]);
                            objEmployeeDetails.pContactId = Convert.ToInt64(dr["contactid"]);
                            objEmployeeDetails.pEmployeeName = Convert.ToString(dr["employeename"]);
                            objEmployeeDetails.pEmployeeContactNo = Convert.ToString(dr["contactnumber"]);
                            objEmployeeDetails.pContactRefNo = Convert.ToString(dr["contactreferenceid"]);
                            objEmployeeDetails.pEmploymentBasicSalary = Convert.ToDecimal(dr["basicsalary"]);
                            objEmployeeDetails.pEmploymentAllowanceORvda = Convert.ToDecimal(dr["allowanceorvariablepay"]);
                            objEmployeeDetails.pEmploymentCTC = Convert.ToDecimal(dr["totalcosttocompany"]);
                            objEmployeeDetails.pEmploymentDesignation = Convert.ToString(dr["designation"]);
                            objEmployeeDetails.pEmploymentRoleName = Convert.ToString(dr["rolename"]);
                            objEmployeeDetails.pEmploymentRoleId = Convert.ToInt64(dr["roleid"]);
                            if (dr["dateofjoining"] == DBNull.Value || Convert.ToString(dr["dateofjoining"]) == string.Empty)
                            {
                                objEmployeeDetails.pEmploymentJoiningDate = Convert.ToDateTime(null).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                objEmployeeDetails.pEmploymentJoiningDate = Convert.ToDateTime(dr["dateofjoining"]).ToString("dd/MM/yyyy");
                            }
                            objEmployeeDetails.presidentialstatus = Convert.ToString(dr["residentialstatus"]);
                            objEmployeeDetails.pmaritalstatus = Convert.ToString(dr["maritalstatus"]);
                            objEmployeeDetails.pplaceofbirth = Convert.ToString(dr["placeofbirth"]);
                            objEmployeeDetails.pcountryofbirth = Convert.ToString(dr["countryofbirth"]);
                            objEmployeeDetails.pnationality = Convert.ToString(dr["nationality"]);
                            objEmployeeDetails.pminoritycommunity = Convert.ToString(dr["minoritycommunity"]);
                            objEmployeeDetails.pListEmpKYC = getDocumentstoreDetails(connectionString, objEmployeeDetails.pContactId, "");
                            objEmployeeDetails.pListEmpBankDetails = getEmployeeBankDetails(connectionString, employeeId);
                            objEmployeeDetails.pListFamilyDetails = getEmployeeFamilyDetails(connectionString, employeeId);
                            //employeeList.Add(objEmployeeDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return objEmployeeDetails;
        }
        private List<EmployeeBankDetails> getEmployeeBankDetails(string connectionString, long pContactId)
        {
            employeeBankdetailsList = new List<EmployeeBankDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT empbankid,employeeid, bankaccountname, bankaccountno,bankbranch, bankifsccode, bankname, isprimaryaccount, statusid FROM tblmstemployeebankdetails  where employeeid=" + pContactId + " and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        EmployeeBankDetails employeeKYC = new EmployeeBankDetails
                        {
                            pBankRecordid = Convert.ToInt64(dr["empbankid"]),
                            pEmployeeID = Convert.ToInt64(dr["employeeid"]),
                            pBankAccountname = Convert.ToString(dr["bankaccountname"]),
                            pBankAccountNo = Convert.ToString(dr["bankaccountno"]),
                            pBankBranch = Convert.ToString(dr["bankbranch"]),
                            pBankifscCode = Convert.ToString(dr["bankifsccode"]),
                            pBankName = Convert.ToString(dr["bankname"]),
                            pIsprimaryAccount = Convert.ToBoolean(dr["isprimaryaccount"]),
                            ptypeofoperation = "UPDATE"
                        };
                        employeeBankdetailsList.Add(employeeKYC);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return employeeBankdetailsList;
        }
        private List<familyDetailsDTO> getEmployeeFamilyDetails(string connectionString, long pContactId)
        {
            employeeFamilydetailsList = new List<familyDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT recordid,employeeid,coalesce( totalnoofmembers,0) as totalnoofmembers, contactpersonname,relationwithemployee, contactnumber FROM tblmstemployeepersonalfamilydetails  where employeeid=" + pContactId + " and  statusid=" + Convert.ToInt32(Status.Active) + "; "))
                {
                    while (dr.Read())
                    {
                        familyDetailsDTO employeeKYC = new familyDetailsDTO
                        {
                            pfamilyrecordid = Convert.ToInt64(dr["recordid"]),
                            pEmployeeid = Convert.ToInt64(dr["employeeid"]),
                            pTotalnoofmembers = Convert.ToInt64(dr["totalnoofmembers"]),
                            pContactpersonname = Convert.ToString(dr["contactpersonname"]),
                            pRelationwithemployee = Convert.ToString(dr["relationwithemployee"]),
                            pContactnumber = Convert.ToString(dr["contactnumber"]),
                            ptypeofoperation = "UPDATE"
                        };
                        employeeFamilydetailsList.Add(employeeKYC);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return employeeFamilydetailsList;
        }
        public bool UpdateEmployeeData(EmployeeDTO empUpdateDTO, string connectionString)
        {
            bool IsUpdated = false;
            StringBuilder sbUpdateEmployee = new StringBuilder();
            string Recordid = string.Empty;
            // string query = string.Empty;
            StringBuilder sbDelete = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string contactdetails = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select coalesce(name,'')||'@'|| coalesce( surname,'') from tblmstcontact  where contactid  in (select contactid from tblmstemployee  where   employeeid =" + empUpdateDTO.pEmployeeId +")"));
                empUpdateDTO.pEmployeeName = contactdetails.Split('@')[0];
                empUpdateDTO.pEmployeeSurName = contactdetails.Split('@')[1];


                if (string.IsNullOrEmpty(empUpdateDTO.pMainTransactionType) || empUpdateDTO.pMainTransactionType.Trim().ToUpper() != "DELETE")
                {
                    // Employement Details
                    if (string.IsNullOrEmpty(empUpdateDTO.pEmploymentJoiningDate))
                    {
                        empUpdateDTO.pEmploymentJoiningDate = "null";
                    }
                    else
                    {
                        empUpdateDTO.pEmploymentJoiningDate = "'" + FormatDate(empUpdateDTO.pEmploymentJoiningDate) + "'";
                    }
                    empUpdateDTO.pEmploymentBasicSalary = Convert.ToString(empUpdateDTO.pEmploymentBasicSalary) == string.Empty ? 0 : empUpdateDTO.pEmploymentBasicSalary < 0 ? 0 : empUpdateDTO.pEmploymentBasicSalary;
                    empUpdateDTO.pEmploymentAllowanceORvda = Convert.ToString(empUpdateDTO.pEmploymentAllowanceORvda) == string.Empty ? 0 : empUpdateDTO.pEmploymentAllowanceORvda < 0 ? 0 : empUpdateDTO.pEmploymentAllowanceORvda;
                    empUpdateDTO.pEmploymentCTC = Convert.ToString(empUpdateDTO.pEmploymentCTC) == string.Empty ? 0 : empUpdateDTO.pEmploymentCTC < 0 ? 0 : empUpdateDTO.pEmploymentCTC;

                    string roleid = Convert.ToString(empUpdateDTO.pEmploymentRoleId);
                    if (empUpdateDTO.pEmploymentRoleId == 0)
                    {
                        roleid = "null";
                    }

                    sbUpdateEmployee.Append("update tblmstemployee set modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("update tblmstemployeeemploymentdetails set totalcosttocompany=" + empUpdateDTO.pEmploymentCTC + ", allowanceorvariablepay=" + empUpdateDTO.pEmploymentAllowanceORvda + ", basicsalary=" + empUpdateDTO.pEmploymentBasicSalary + ", dateofjoining=" + empUpdateDTO.pEmploymentJoiningDate + ",designation='" + ManageQuote(empUpdateDTO.pEmploymentDesignation) + "', rolename='" + ManageQuote(empUpdateDTO.pEmploymentRoleName) + "', roleid=" + roleid + ",statusid=" + getStatusid(empUpdateDTO.pStatusname, connectionString) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("update tblmstemployeepersonalbirthdetails set residentialstatus='" + ManageQuote(empUpdateDTO.presidentialstatus) + "', maritalstatus='" + ManageQuote(empUpdateDTO.pmaritalstatus) + "', placeofbirth='" + ManageQuote(empUpdateDTO.pplaceofbirth) + "', countryofbirth='" + ManageQuote(empUpdateDTO.pcountryofbirth) + "',nationality='" + ManageQuote(empUpdateDTO.pnationality) + "', minoritycommunity='" + ManageQuote(empUpdateDTO.pminoritycommunity) + "',modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");


                    // KYC Data

                    //if (empUpdateDTO.pListEmpKYC != null && empUpdateDTO.pListEmpKYC.Count > 0)
                    //{
                    //    foreach (documentstoreDTO docStore in empUpdateDTO.pListEmpKYC)
                    //    {
                    //        if (!string.IsNullOrEmpty(docStore.pDocumentReferenceMonth) && !string.IsNullOrEmpty(docStore.pDocumentReferenceYear))
                    //        {
                    //            docStore.pDocReferenceno = docStore.pDocReferenceno + "~" + docStore.pDocumentReferenceMonth + "~" + docStore.pDocumentReferenceYear;
                    //        }
                    //        else if (!string.IsNullOrEmpty(docStore.pDocumentReferenceMonth))
                    //        {
                    //            docStore.pDocReferenceno = docStore.pDocReferenceno + "~" + docStore.pDocumentReferenceMonth;
                    //        }
                    //        else if (!string.IsNullOrEmpty(docStore.pDocumentReferenceYear))
                    //        {
                    //            docStore.pDocReferenceno = docStore.pDocReferenceno + "~" + docStore.pDocumentReferenceYear;
                    //        }
                    //        else
                    //        {
                    //            docStore.pDocReferenceno = docStore.pDocReferenceno;
                    //        }
                    //        if (!string.IsNullOrEmpty(docStore.ptypeofoperation))
                    //        {
                    //            if (docStore.ptypeofoperation.Trim().ToUpper() != "CREATE")
                    //            {
                    //                if (string.IsNullOrEmpty(Recordid))
                    //                {
                    //                    Recordid = docStore.pDocstoreId.ToString();
                    //                }
                    //                else
                    //                {
                    //                    Recordid = Recordid + "," + docStore.pDocstoreId.ToString();
                    //                }
                    //            }
                    //            if(docStore.ptypeofoperation.Trim().ToUpper() == "OLD")
                    //            {
                    //                docStore.ptypeofoperation = "UPDATE";
                    //            }
                    //            if (docStore.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                    //            {
                    //                sbUpdateEmployee.Append("UPDATE tblmstdocumentstore SET  documentid=" + docStore.pDocumentId + ", documentgroupid=" + docStore.pDocumentGroupId + ", documentgroupname='" + ManageQuote(docStore.pDocumentGroup).Trim() + "', documentname='" + ManageQuote(docStore.pDocumentName).Trim() + "', docstorepath='" + ManageQuote(docStore.pDocStorePath).Trim() + "', docfiletype='" + ManageQuote(docStore.pDocFileType).Trim() + "', docreferenceno='" + ManageQuote(docStore.pDocReferenceno).Trim() + "', docisdownloadable=" + docStore.pDocIsDownloadable + ", modifiedby=" + empUpdateDTO.pCreatedby + ", modifieddate=current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " WHERE contactid=" + docStore.pContactId + " and docstoreid=" + docStore.pDocstoreId + ";");
                    //            }
                    //            else if (docStore.ptypeofoperation.Trim().ToUpper() == "CREATE")
                    //            {
                    //                sbUpdateEmployee.Append("insert into tblmstdocumentstore (contactid ,documentid,documentgroupid,documentgroupname,documentname,docstorepath,docfiletype,docreferenceno,docisdownloadable,statusid,createdby,createddate) values (" + docStore.pContactId + "," + docStore.pDocumentId + "," + docStore.pDocumentGroupId + ",'" + ManageQuote(docStore.pDocumentGroup).Trim() + "','" + ManageQuote(docStore.pDocumentName).Trim() + "','" + ManageQuote(docStore.pDocStorePath).Trim() + "','" + ManageQuote(docStore.pDocFileType).Trim() + "','" + ManageQuote(docStore.pDocReferenceno).Trim() + "'," + docStore.pDocIsDownloadable + "," + Convert.ToInt32(Status.Active) + "," + empUpdateDTO.pCreatedby + ",current_timestamp);");
                    //            }
                    //            //else if (docStore.ptypeofoperation.Trim().ToUpper() == "DELETE")
                    //            //{
                    //            //    sbUpdateEmployee.Append("UPDATE tblmstdocumentstore SET  modifiedby=" + empUpdateDTO.pCreatedby + ", modifieddate=current_timestamp,statusid=" + getStatusid("In-Active", connectionString) + " WHERE contactid=" + docStore.pContactId + " and docstoreid=" + docStore.pDocstoreId + ";");
                    //            //}
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(Recordid))
                    //    {
                    //        sbDelete.Append("update tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where docstoreid not in(" + Recordid + ")" +
                    //            " and contactid in (select contactid from tblmstemployee where employeeid =" + empUpdateDTO.pEmployeeId + ")" +
                    //            "and coalesce(loanid,0)=0;");
                    //    }
                    //    else
                    //    {
                    //       // sbDelete.Append("update tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where contactid in (select contactid from tblmstemployee where employeeid =" + empUpdateDTO.pEmployeeId + ") and coalesce(loanid,0)=0;");

                    //        sbDelete.Append("update tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where contactid in (select contactid from tblmstemployee where employeeid =" + empUpdateDTO.pEmployeeId + ") and coalesce(loanid,0)=0;");
                    //    }
                    //}
                    //else
                    //{
                    //    sbDelete.Append("update tblmstdocumentstore set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where contactid in (select contactid from tblmstemployee where employeeid =" + empUpdateDTO.pEmployeeId + ") and coalesce(loanid,0)=0;");
                    //}


                    if (empUpdateDTO.pListEmpKYC != null && empUpdateDTO.pListEmpKYC.Count > 0)
                    {
                        ReferralAdvocateDAL objReferralAdvocateDAL = new ReferralAdvocateDAL();
                        sbDelete.Append(objReferralAdvocateDAL.UpdateStoreDetails(empUpdateDTO.pListEmpKYC, connectionString, 0, empUpdateDTO.pContactId));
                    }
                    if (empUpdateDTO.pListEmpBankDetails != null && empUpdateDTO.pListEmpBankDetails.Count > 0)
                    {
                        Recordid = string.Empty;
                        foreach (EmployeeBankDetails empBank in empUpdateDTO.pListEmpBankDetails)
                        {
                            if (!string.IsNullOrEmpty(empBank.ptypeofoperation))
                            {
                                if (empBank.ptypeofoperation.Trim().ToUpper() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = empBank.pBankRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + empBank.pBankRecordid.ToString();
                                    }
                                }
                                if (empBank.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                                {
                                    sbUpdateEmployee.Append("UPDATE tblmstemployeebankdetails SET bankaccountname ='" + ManageQuote(empBank.pBankAccountname).Trim() + "', bankname ='" + ManageQuote(empBank.pBankName).Trim() + "', bankaccountno ='" + ManageQuote(empBank.pBankAccountNo).Trim() + "',bankifsccode ='" + ManageQuote(empBank.pBankifscCode).Trim() + "', bankbranch ='" + ManageQuote(empBank.pBankBranch).Trim() + "', isprimaryaccount ='" + empBank.pIsprimaryAccount + "', modifiedby =" + empUpdateDTO.pCreatedby + ", modifieddate =current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " WHERE empbankid=" + empBank.pBankRecordid + " and employeeid=" + empUpdateDTO.pEmployeeId + "; ");
                                }
                                else if (empBank.ptypeofoperation.Trim().ToUpper() == "CREATE")
                                {
                                    sbUpdateEmployee.Append("insert into tblmstemployeebankdetails(employeeid,bankaccountname,bankname,bankaccountno,bankifsccode,bankbranch,isprimaryaccount,statusid,createdby,createddate) values(" + empUpdateDTO.pEmployeeId + ",'" + ManageQuote(empBank.pBankAccountname).Trim() + "','" + ManageQuote(empBank.pBankName).Trim() + "','" + ManageQuote(empBank.pBankAccountNo).Trim() + "','" + ManageQuote(empBank.pBankifscCode).Trim() + "','" + ManageQuote(empBank.pBankBranch).Trim() + "','" + empBank.pIsprimaryAccount + "'," + Convert.ToInt32(Status.Active) + "," + empUpdateDTO.pCreatedby + ",current_timestamp);");
                                }
                                //else if (empBank.ptypeofoperation.Trim().ToUpper() == "DELETE")
                                //{
                                //    sbUpdateEmployee.Append("UPDATE tblmstemployeebankdetails SET modifiedby=" + empUpdateDTO.pCreatedby + ", modifieddate=current_timestamp,statusid=" + getStatusid("In-Active", connectionString) + " WHERE empbankid=" + empBank.pBankRecordid + " and employeeid=" + empUpdateDTO.pEmployeeId + ";");
                                //}
                            }
                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbDelete.Append("update tblmstemployeebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where empbankid not in(" + Recordid + ");");
                        }
                        else
                        {
                            sbDelete.Append("update tblmstemployeebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                        }
                    }
                    else
                    {
                        sbDelete.Append("update tblmstemployeebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    }
                    if (empUpdateDTO.pListFamilyDetails != null && empUpdateDTO.pListFamilyDetails.Count > 0)
                    {
                        Recordid = string.Empty;
                        foreach (familyDetailsDTO empFamily in empUpdateDTO.pListFamilyDetails)
                        {
                            // Personal Details
                            empFamily.pTotalnoofmembers = Convert.ToString(empFamily.pTotalnoofmembers) == string.Empty ? 0 : empFamily.pTotalnoofmembers < 0 ? 0 : empFamily.pTotalnoofmembers;
                            if (!string.IsNullOrEmpty(empFamily.ptypeofoperation))
                            {
                                if (empFamily.ptypeofoperation.Trim().ToUpper() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = empFamily.pfamilyrecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + empFamily.pfamilyrecordid.ToString();
                                    }
                                }
                                if (empFamily.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                                {
                                    sbUpdateEmployee.Append("UPDATE tblmstemployeepersonalfamilydetails SET totalnoofmembers =" + (empFamily.pTotalnoofmembers) + ", contactpersonname ='" + empFamily.pContactpersonname+ "', relationwithemployee ='" + ManageQuote(empFamily.pRelationwithemployee).Trim() + "',contactnumber ='" + ManageQuote(empFamily.pContactnumber).Trim() + "',modifiedby =" + empUpdateDTO.pCreatedby + ", modifieddate =current_timestamp,statusid=" + Convert.ToInt32(Status.Active) + " WHERE recordid=" + empFamily.pfamilyrecordid + " and employeeid=" + empUpdateDTO.pEmployeeId + "; ");
                                }
                                else if (empFamily.ptypeofoperation.Trim().ToUpper() == "CREATE")
                                {
                                    sbUpdateEmployee.Append("insert into tblmstemployeepersonalfamilydetails(employeeid,totalnoofmembers,contactpersonname,relationwithemployee,contactnumber,statusid,createdby,createddate) values(" + empUpdateDTO.pEmployeeId + "," + empFamily.pTotalnoofmembers + ",'" + ManageQuote(empFamily.pContactpersonname).Trim() + "','" + ManageQuote(empFamily.pRelationwithemployee).Trim() + "','" + ManageQuote(empFamily.pContactnumber).Trim() + "'," + Convert.ToInt32(Status.Active) + "," + empUpdateDTO.pCreatedby + ",current_timestamp);");
                                }
                                //else if (empFamily.ptypeofoperation.Trim().ToUpper() == "DELETE")
                                //{
                                //    sbUpdateEmployee.Append("UPDATE tblmstemployeepersonalfamilydetails SET modifiedby =" + empUpdateDTO.pCreatedby + ", modifieddate=current_timestamp,statusid=" + getStatusid("In-Active", connectionString) + " WHERE recordid=" + empFamily.pfamilyrecordid + " and employeeid=" + empUpdateDTO.pEmployeeId + ";");
                                //}
                            }
                        }

                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbDelete.Append("update tblmstemployeepersonalfamilydetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where recordid not in(" + Recordid + ");");
                        }
                        else
                        {
                            sbDelete.Append("update tblmstemployeepersonalfamilydetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                        }
                    }
                    else
                    {
                        sbDelete.Append("update tblmstemployeepersonalfamilydetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    }
                }
                else if (empUpdateDTO.pMainTransactionType.Trim().ToUpper() == "DELETE")
                {
                    sbUpdateEmployee.Append("update tblmstemployee set statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("UPDATE tblmstdocumentstore SET  modifiedby=" + empUpdateDTO.pCreatedby + ", modifieddate=current_timestamp,statusid=" + Convert.ToInt32(Status.Inactive) + " WHERE contactid=" + empUpdateDTO.pContactId + " and docstoreid in (select distinct docstoreid from tblmstdocumentstore td join tblmstemployee tm on  tm.contactid=td.contactid  where tm.contactid=" + empUpdateDTO.pContactId + ");");
                    sbUpdateEmployee.Append("update tblmstemployeeemploymentdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("update tblmstemployeepersonalbirthdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("update tblmstemployeepersonalfamilydetails set statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                    sbUpdateEmployee.Append("update tblmstemployeebankdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby=" + empUpdateDTO.pCreatedby + ",modifieddate=current_timestamp where employeeid=" + empUpdateDTO.pEmployeeId + ";");
                }
                _ = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbDelete) + "" + Convert.ToString(sbUpdateEmployee));
                trans.Commit();
                IsUpdated = true;
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            return IsUpdated;
        }
        public int checkEmployeeRoleExistsOrNot(string Rolename, string connectionString)
        {
            try
            {
                return Convert.ToString(Rolename) != string.Empty ? Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstemployeerole where upper(rolename)='" + ManageQuote(Rolename).Trim().ToUpper() + "' and statusid = " + Convert.ToInt32(Status.Active) + ";")) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
