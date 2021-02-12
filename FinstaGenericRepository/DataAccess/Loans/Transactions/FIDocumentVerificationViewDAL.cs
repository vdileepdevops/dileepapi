using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
   public partial class FirstinformationDAL : SettingsDAL, IFirstinformation
    {
      
        public List<FirstinformationDTO> FIDocumentViewGetloandetails(string Applicationid, string ConnectionString)
        {
            lstFirstinformation = new List<FirstinformationDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,t2.verificationstatus,t2.fiverifierscomments,t2.fiverifiersrating,t2.verificationdate,t2.verificationtime,t2.username from (select applicationid, vchapplicationid, dateofapplication, applicantid, contactreferenceid, applicantname, contacttype, loantypeid, loantype, loanid, loanname, groupid, groupname, applicanttype, isschemesapplicable, schemename, schemecode, amountrequested, purposeofloan,loanpayin, interesttype, rateofinterest, tenureofloan, tenuretype, loaninstalmentpaymentmode, instalmentamount,  issurietypersonsapplicable, iskycapplicable, ispersonaldetailsapplicable, issecurityandcolletralapplicable, isexistingloansapplicable, isreferencesapplicable, isreferralapplicable, referralname, salespersonname, 'Applicant' as appcontacttype, 'Loan Application Details' as sectionname,coalesce(disbursementdate,current_date)as disbursementdate from tabapplication  where upper(vchapplicationid) = '" + ManageQuote(Applicationid.Trim().ToUpper()) + "') t1 left join(select x.*,coalesce(y.username,'') as username from tabapplicationFIverification x join tblmstusers y on x.createdby=y.userid  where upper(vchapplicationid) = '" + ManageQuote(Applicationid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and t1.appcontacttype = t2.contacttype and t1.sectionname = t2.verifiedsectionname;"))
                {
                    while (dr.Read())
                    {
                        FirstinformationDTO objFirstinformation = new FirstinformationDTO();
                        objFirstinformation.papplicationid = Convert.ToInt64(dr["applicationid"]);
                        objFirstinformation.pVchapplicationid = Convert.ToString(dr["vchapplicationid"]);
                        objFirstinformation.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                        objFirstinformation.pDateofDisbursement = dr["disbursementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["disbursementdate"]).ToString("dd/MM/yyyy");
                        objFirstinformation.pApplicantid = Convert.ToInt64(dr["Applicantid"]);
                        objFirstinformation.pContactreferenceid = Convert.ToString(dr["Contactreferenceid"]);
                        objFirstinformation.pApplicantname = Convert.ToString(dr["Applicantname"]);
                        objFirstinformation.pContacttype = Convert.ToString(dr["Contacttype"]);
                        objFirstinformation.pLoantypeid = Convert.ToInt64(dr["Loantypeid"]);
                        objFirstinformation.pLoantype = Convert.ToString(dr["Loantype"]);
                        objFirstinformation.pLoanid = Convert.ToInt64(dr["Loanid"]);
                        objFirstinformation.pLoanname = Convert.ToString(dr["Loanname"]);
                        objFirstinformation.pApplicanttype = Convert.ToString(dr["Applicanttype"]);
                        objFirstinformation.pIsschemesapplicable = Convert.ToBoolean(dr["Isschemesapplicable"]);
                        objFirstinformation.pSchemename = Convert.ToString(dr["Schemename"]);
                        objFirstinformation.pSchemecode = Convert.ToString(dr["Schemecode"]);
                        objFirstinformation.pAmountrequested = Convert.ToDecimal(dr["Amountrequested"]);
                        objFirstinformation.pPurposeofloan = Convert.ToString(dr["Purposeofloan"]);
                        objFirstinformation.pLoanpayin = Convert.ToString(dr["Loanpayin"]);
                        objFirstinformation.pInteresttype = Convert.ToString(dr["Interesttype"]);
                        objFirstinformation.pRateofinterest = Convert.ToDecimal(dr["Rateofinterest"]);
                        objFirstinformation.pTenureofloan = Convert.ToInt64(dr["Tenureofloan"]);
                        objFirstinformation.pTenuretype = Convert.ToString(dr["Tenuretype"]);
                        objFirstinformation.pLoaninstalmentpaymentmode = Convert.ToString(dr["Loaninstalmentpaymentmode"]);
                        objFirstinformation.pInstalmentamount = Convert.ToDecimal(dr["Instalmentamount"]);
                    
                        objFirstinformation.pIssurietypersonsapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsKYCapplicable = Convert.ToBoolean(dr["IsKYCapplicable"]);
                        objFirstinformation.pIspersonaldetailsapplicable = Convert.ToBoolean(dr["Ispersonaldetailsapplicable"]);
                        objFirstinformation.pIssecurityandcolletralapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsexistingloansapplicable = Convert.ToBoolean(dr["Isexistingloansapplicable"]);
                        objFirstinformation.pIsreferencesapplicable = Convert.ToBoolean(dr["Isreferencesapplicable"]);
                        objFirstinformation.pIsreferralapplicable = Convert.ToBoolean(dr["Isreferralapplicable"]);
                        objFirstinformation.pReferralname = Convert.ToString(dr["Referralname"]);
                        objFirstinformation.pSalespersonname = Convert.ToString(dr["Salespersonname"]);
                        objFirstinformation.SectionName = Convert.ToString(dr["sectionname"]);
                        objFirstinformation.pFIVerifierscomments= Convert.ToString(dr["fiverifierscomments"]);
                        objFirstinformation.pFIVerifiersrating = Convert.ToString(dr["fiverifiersrating"]);
                        objFirstinformation.pverificationdate= dr["verificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["verificationdate"]).ToString("dd/MM/yyyy");
                        objFirstinformation.pverificationtime = Convert.ToString(dr["verificationtime"]);
                        objFirstinformation.IsVerified = Convert.ToString(dr["verificationstatus"]);
                        objFirstinformation.pVerifierName = Convert.ToString(dr["username"]);
                        objFirstinformation.ptypeofoperation = "OLD";                      
                        lstFirstinformation.Add(objFirstinformation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFirstinformation;
        }
        public ApplicationKYCDocumentsDTO FIDocumentGetApplicantCreditandkycdetails(string Applicationid, long contactid, string ConnectionString)
        {
            ApplicationKYCDocumentsDTO = new ApplicationKYCDocumentsDTO();
            try
            {

                ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO = FIDocumentGetCreditScoreDetails(Applicationid, ConnectionString);
                ApplicationKYCDocumentsDTO.documentstorelist = FIDocumentgetDocumentstoreDetails(ConnectionString, contactid, Applicationid);
                // lstApplicationKYCDocuments.Add(objApplicationKYCDocuments);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ApplicationKYCDocumentsDTO;
        }
        public List<documentstoreDTO> FIDocumentgetDocumentstoreDetails(string connectionString, Int64 pContactId, string strapplicationid)
        {
            string strQuery = string.Empty;
            long applicationid = 0;
            documentstoredetails = new List<documentstoreDTO>();
            try
            {
                if (!string.IsNullOrEmpty(strapplicationid))
                {
                    applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + strapplicationid + "';"));
                }

                if (string.IsNullOrEmpty(strapplicationid))
                {
                    strQuery = "SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid=" + pContactId + " and upper(ts.statusname)='ACTIVE' and coalesce(loanid,0)=0;";
                }
                else
                {
                    //strQuery = "SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + " union all SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid=" + pContactId + " and upper(ts.statusname)='ACTIVE'; ";
                    strQuery = "select distinct t3.*,t4.verifiedsectionname,verificationstatus from (SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,'KYC DocumentsDTO Details' as sectionname,coalesce(t.contacttype, '')||'-'||coalesce(t1.name,'')||' '||coalesce(t1.surname,'') as subscetion FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + " )t3 left join(select* from tabapplicationFIverification where applicationid = " + applicationid + ") t4 on t3.applicationno = t4.applicationid and coalesce(t3.contacttype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and coalesce(trim(t3.subscetion),'') = coalesce(trim(t4.verifiedsubsectionname),'') and t3.docstoreid=t4.contactreferenceid::int;";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        documentstoreDTO objdocumentstore = new documentstoreDTO();
                        objdocumentstore.pDocstoreId = Convert.ToInt64(dr["docstoreid"]);
                        objdocumentstore.pContactId = Convert.ToInt64(dr["contactid"]);
                        objdocumentstore.pLoanId = Convert.ToInt64(dr["loanid"]);
                        objdocumentstore.pApplicationNo = Convert.ToInt64(dr["applicationno"]);
                        objdocumentstore.pDocumentId = Convert.ToInt64(dr["documentid"]);
                        objdocumentstore.pDocumentGroupId = Convert.ToInt64(dr["documentgroupid"]);
                        objdocumentstore.pDocumentGroup = Convert.ToString(dr["documentgroupname"]);
                        objdocumentstore.pDocumentName = Convert.ToString(dr["documentname"]);
                        objdocumentstore.pDocStorePath = Convert.ToString(dr["docstorepath"]);
                        objdocumentstore.pDocFileType = Convert.ToString(dr["docfiletype"]);
                        objdocumentstore.pDocReferenceno = Convert.ToString(dr["docreferenceno"]);
                        objdocumentstore.pDocIsDownloadable = Convert.ToBoolean(dr["docisdownloadable"]);
                        objdocumentstore.pStatusname = Convert.ToString(dr["statusname"]);
                        objdocumentstore.pName = Convert.ToString(dr["name"]);
                        objdocumentstore.pContactType = Convert.ToString(dr["contacttype"]).Trim();                     
                        objdocumentstore.SectionName = Convert.ToString(dr["sectionname"]).Trim();
                        objdocumentstore.psubsectionname = Convert.ToString(dr["subscetion"]).Trim();                     
                        objdocumentstore.IsVerified = Convert.ToString(dr["verificationstatus"]).Trim();
                        objdocumentstore.ptypeofoperation = "OLD";                        
                        documentstoredetails.Add(objdocumentstore);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return documentstoredetails;
        }
        private List<CreditScoreDetailsDTO> FIDocumentGetCreditScoreDetails(string Applicationid, string ConnectionString)
        {
            lstCreditScoreDetails = new List<CreditScoreDetailsDTO>();
            try
            {
               
                // using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select contactid,contactreferenceid,applicantype,iscreditscoreapplicable,ciccompany,creditscore,maxcreditscore,creditscorefilepath from tabapplicationancilbildetails where vchapplicationid= '" + ManageQuote(Applicationid) + "' and statusid=" + getStatusid("ACTIVE", ConnectionString) + ""))
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from (select t.vchapplicationid,t.contactid,t.contactreferenceid,coalesce(t.applicantype,'') as applicantype,t.iscreditscoreapplicable,t.ciccompany,t.creditscore,t.maxcreditscore,coalesce(t.creditscorefilepath,'') as creditscorefilepath,t.statusid,'Credit Score Details' as sectionname,coalesce(applicantype, '')||'-'||coalesce(m.name, '')||' '||coalesce(m.surname,'') as subscetion from tabapplicationancilbildetails t join tblmstcontact m on t.contactid=m.contactid  where upper(t.vchapplicationid)= '" + ManageQuote(Applicationid.Trim().ToUpper()) + "' and t.statusid=1)t1  left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(Applicationid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicantype,'') = coalesce(t2.contacttype,'') and coalesce(trim(t1.sectionname),'') = coalesce(trim(t2.verifiedsectionname),'') and coalesce(trim(t1.subscetion),'') = coalesce(trim(t2.verifiedsubsectionname),'') and t1.contactreferenceid=t2.contactreferenceid;"))
                {
                    while (dr.Read())
                    {
                        CreditScoreDetailsDTO objCreditScoreDetail = new CreditScoreDetailsDTO();
                        objCreditScoreDetail.pContactid = Convert.ToInt64(dr["contactid"]);
                        objCreditScoreDetail.pContactreferenceid= Convert.ToString(dr["contactreferenceid"]);
                        objCreditScoreDetail.pIscreditscoreapplicable = Convert.ToBoolean(dr["iscreditscoreapplicable"]);
                        objCreditScoreDetail.pCiccompany = Convert.ToString(dr["ciccompany"]);
                        objCreditScoreDetail.pCreditscore = Convert.ToInt64(dr["creditscore"]);
                        objCreditScoreDetail.pMaxcreditscore = Convert.ToInt64(dr["maxcreditscore"]);
                        objCreditScoreDetail.pCreditscorefilepath = Convert.ToString(dr["creditscorefilepath"]);
                        objCreditScoreDetail.SectionName = Convert.ToString(dr["sectionname"]);
                        objCreditScoreDetail.pApplicanttype= Convert.ToString(dr["applicantype"]);
                        objCreditScoreDetail.psubsectionname = Convert.ToString(dr["subscetion"]);                                                      
                        objCreditScoreDetail.IsVerified = Convert.ToString(dr["verificationstatus"]);
                        objCreditScoreDetail.ptypeofoperation  = "OLD";                                                                         
                        lstCreditScoreDetails.Add(objCreditScoreDetail);

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstCreditScoreDetails;
        }
     
        public List<ApplicationContactPersonalDetailsDTO> GetFIDocumentkycDetails(string strapplictionid, string ConnectionString)
        {
            List<ApplicationContactPersonalDetailsDTO> lstApplicationContactPersonalDetailsDTO = new List<ApplicationContactPersonalDetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t3.applicationid,t1.vchapplicationid,t2.contactid,t2.contactreferenceid,t1.residentialstatus,t1.maritalstatus,t1.placeofbirth, t1.countryofbirth, t1.nationality,t1.minoritycommunity,t1.applicantype,'Personal KYC Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion,t2.contacttype,t2.name||' '||t2.surname as name,t2.dob, t2.gender,t2.fathername,t2.typeofenterprise,t2.natureofbusiness, t2.businessentitycontactno,t2.businessentityemailid FROM tabapplication t3 join tblmstcontact t2 on t3.applicantid=t2.contactid left outer join tabapplicationpersonalbirthdetails t1 on t1.applicationid=t3.applicationid  where  upper(t3.vchapplicationid) ='" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and coalesce( t1.statusid,1)=" + Convert.ToInt32(Status.Active) + ")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and  coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname and t3.contactid=t4.contactid;"))
                {
                    while (dr.Read())
                    {
                        ApplicationContactPersonalDetailsDTO ApplicationContactPersonalDetailsDTO = new ApplicationContactPersonalDetailsDTO();
                        ApplicationContactPersonalDetailsDTO.pName = Convert.ToString(dr["name"]);
                        ApplicationContactPersonalDetailsDTO.pDob = dr["dob"] == DBNull.Value ? null : Convert.ToDateTime(dr["dob"]).ToString("dd/MM/yyyy");
                        if (Convert.ToString(dr["dob"]) != string.Empty)
                        {
                            ApplicationContactPersonalDetailsDTO.pAge = CalculateAgeCorrect(Convert.ToDateTime(dr["dob"]));
                        }                    
                        ApplicationContactPersonalDetailsDTO.pGender = Convert.ToString(dr["gender"]);
                        ApplicationContactPersonalDetailsDTO.pContactType = Convert.ToString(dr["contacttype"]);
                        ApplicationContactPersonalDetailsDTO.pBusinessEntityContactno = Convert.ToString(dr["businessentitycontactno"]);
                        ApplicationContactPersonalDetailsDTO.pBusinessEntityEmailId = Convert.ToString(dr["businessentityemailid"]);
                        ApplicationContactPersonalDetailsDTO.pFatherName = Convert.ToString(dr["fathername"]);
                        ApplicationContactPersonalDetailsDTO.pContactId =Convert.ToInt64(dr["contactid"].ToString());
                        ApplicationContactPersonalDetailsDTO.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        ApplicationContactPersonalDetailsDTO.presidentialstatus = Convert.ToString(dr["residentialstatus"]);
                        ApplicationContactPersonalDetailsDTO.pplaceofbirth = Convert.ToString(dr["placeofbirth"]);
                        ApplicationContactPersonalDetailsDTO.pcountryofbirth = Convert.ToString(dr["countryofbirth"]);
                        ApplicationContactPersonalDetailsDTO.pnationality = Convert.ToString(dr["nationality"]);
                        ApplicationContactPersonalDetailsDTO.pmaritalstatus = Convert.ToString(dr["maritalstatus"]);
                        ApplicationContactPersonalDetailsDTO.pminoritycommunity = Convert.ToString(dr["minoritycommunity"]);
                        ApplicationContactPersonalDetailsDTO.psubsectionname = Convert.ToString(dr["subscetion"]);
                        ApplicationContactPersonalDetailsDTO.SectionName = Convert.ToString(dr["sectionname"]);
                        ApplicationContactPersonalDetailsDTO.IsVerified = Convert.ToString(dr["verificationstatus"]);
                        ApplicationContactPersonalDetailsDTO.papplicanttype= Convert.ToString(dr["applicantype"]);
                        lstApplicationContactPersonalDetailsDTO.Add(ApplicationContactPersonalDetailsDTO);
                    }
                }
             }
            catch (Exception)
            {

                throw;
            }
            return lstApplicationContactPersonalDetailsDTO;
        }
        public List<contactPersonalAddressDTO> GetFIDocumentAddressDetails(string strapplictionid, string ConnectionString)
        {
            List<contactPersonalAddressDTO> lstcontactPersonalAddressDTO = new List<contactPersonalAddressDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(select x.*,y.addresstype, y.address1,y.address2,y.state,y.district,y.city,y.country,y.pincode from (SELECT t1.recordid,t1.applicationid,t1.vchapplicationid,t1.contactid,t1.contactreferenceid,t1.contacttype as applicantype,'Personal Address Details' AS sectionname,coalesce(t1.contacttype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalapplicablesections t1 join tblmstcontact t2 on t1.contactid=t2.contactid where t1.statusid=" + Convert.ToInt32(Status.Active)+") x join tblmstcontactaddressdetails y on x.contactid=y.contactid where upper(x.vchapplicationid) ='" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' )t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and  coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and trim(t3.subscetion) = trim(t4.verifiedsubsectionname) and t3.contactid=t4.contactid;"))
                {
                    while (dr.Read())
                    {
                        contactPersonalAddressDTO contactPersonalAddressDTO = new contactPersonalAddressDTO();
                        contactPersonalAddressDTO.pcontactid = Convert.ToInt64(dr["contactid"].ToString());
                        contactPersonalAddressDTO.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        contactPersonalAddressDTO.pAddressType = Convert.ToString(dr["addresstype"]);
                        contactPersonalAddressDTO.pAddress1 = Convert.ToString(dr["address1"]);
                        contactPersonalAddressDTO.pAddress2 = Convert.ToString(dr["address2"]);
                        contactPersonalAddressDTO.pState = Convert.ToString(dr["state"]);
                        contactPersonalAddressDTO.pDistrict = Convert.ToString(dr["district"]);
                        contactPersonalAddressDTO.pCity = Convert.ToString(dr["city"]);
                        contactPersonalAddressDTO.pCountry = Convert.ToString(dr["country"]);
                        contactPersonalAddressDTO.pPinCode = Convert.ToInt64(dr["pincode"].ToString());
                        contactPersonalAddressDTO.psubsectionname = Convert.ToString(dr["subscetion"]);
                        contactPersonalAddressDTO.SectionName = Convert.ToString(dr["sectionname"]);
                        contactPersonalAddressDTO.IsVerified = Convert.ToString(dr["verificationstatus"]);
                        contactPersonalAddressDTO.papplicanttype = Convert.ToString(dr["applicantype"]);
                        lstcontactPersonalAddressDTO.Add(contactPersonalAddressDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstcontactPersonalAddressDTO;
        }
        public ApplicationPersonalInformationDTO FIDocumentGetApplicationPersonalInformation(string strapplictionid, string ConnectionString)
        {
            ds = new DataSet();
            long applicationid = 0;
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
                applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "';")); ;
                ApplicationPersonalInformationDTO.papplicationid = applicationid;
                ApplicationPersonalInformationDTO.pvchapplicationid = strapplictionid;
               // ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select t1.*,t2.verifiedsectionname,verificationstatus from(SELECT recordid,applicationid,vchapplicationid,contactid,contactreferenceid,applicanttype, isemploymentapplicable, employmenttype, nameoftheorganization,natureoftheorganization, employmentrole, officeaddress, officephoneno,reportingto, employeeexp, employeeexptype, totalworkexp, dateofestablishment,dateofcommencement, gstinno, cinno, dinno, tradelicenseno,'Employement Details' as sectionname FROM tabapplicationpersonalemplymentdetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ") t1 left join(select * from tabapplicationFIverification  where vchapplicationid = '" + ManageQuote(strapplictionid) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and t1.applicanttype = t2.contacttype and t1.sectionname = t2.verifiedsectionname;SELECT recordid, applicationid,vchapplicationid, contactid,contactreferenceid,residentialstatus, maritalstatus, placeofbirth, countryofbirth, nationality,minoritycommunity FROM tabapplicationpersonalbirthdetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.applicationid, t1.vchapplicationid,t1.suritycontactreferenceid,surityapplicanttype,contactname,t2.recordid,t2.contactid,t2.contactreferenceid,totalnoofmembers,noofearningmembers,familytype,noofboyschild,noofgirlchild,houseownership,'Personal Details' AS sectionname FROM tabapplicationsurietypersonsdetails  t1 left join tabapplicationpersonalfamilydetails t2 on t1.applicationid=t2.applicationid and t1.suritycontactreferenceid=t2.contactreferenceid where t1.vchapplicationid = '" + ManageQuote(strapplictionid) + "')t3 left join(select * from tabapplicationFIverification  where vchapplicationid = '" + ManageQuote(strapplictionid) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and t3.surityapplicanttype = t4.contacttype and t3.sectionname = t4.verifiedsectionname; SELECT recordid, applicationid, vchapplicationid, contactid, contactreferenceid,nomineename, relationship, dateofbirth, contactno, idprooftype, idproofname, referencenumber, docidproofpath FROM tabapplicationpersonalnomineedetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";select t1.*,t2.verifiedsectionname,verificationstatus from(SELECT recordid, applicationid, vchapplicationid, contactid, contactreferenceid,bankname, accountno, ifsccode, branch, isprimarybank,'Bank Details' AS sectionname,coalesce(bankname,'')||' '||coalesce(accountno,'') as subscetion,'Applicant' as applicanttype FROM tabapplicationpersonalbankdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ") t1  left join(select vchapplicationid,verifiedsectionname,verifiedsubsectionname,contacttype,verificationstatus from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and t1.applicanttype = t2.contacttype and t1.sectionname = t2.verifiedsectionname and t1.subscetion=t2.verifiedsubsectionname; SELECT recordid, applicationid, vchapplicationid, contactid, contactreferenceid,grossannualincome, netannualincome, averageannualexpenses FROM tabapplicationpersonalincomedetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";SELECT recordid,applicationid,vchapplicationid,contactid,contactreferenceid,sourcename, grossannual FROM tabapplicationpersonalotherincomedetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";SELECT recordid, applicationid, vchapplicationid, contactid, contactreferenceid,qualification, nameofthecourseorprofession, occupation FROM tabapplicationpersonaleducationdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";SELECT recordid,applicationid,vchapplicationid,contactid,contactreferenceid,businessactivity, establishmentdate, commencementdate, cinnumber,gstinno FROM tabapplicationpersonalbusinessdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; SELECT recordid, applicationid, vchapplicationid, contactid, contactreferenceid, financialyear, turnoveramount, netprofitamount, docbalancesheetpath FROM tabapplicationpersonalbusinessfinancialdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; ");
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT x.recordid,x.applicationid,x.vchapplicationid,x.contactid,x.contactreferenceid,x.applicanttype, x.isemploymentapplicable,x. employmenttype, x.nameoftheorganization,x.natureoftheorganization,x. employmentrole, officeaddress, officephoneno,reportingto, employeeexp, employeeexptype, totalworkexp, dateofestablishment,dateofcommencement, gstinno, cinno, dinno, tradelicenseno,'Employement Details' as sectionname,coalesce(applicanttype, '')||'-'||coalesce(y.name,'')||' '||coalesce(y.surname,'') as subscetion FROM tabapplicationpersonalemplymentdetails x join TBLMSTCONTACT y on x.contactid=y.contactid where x.applicationid=" + applicationid + " and upper(x.vchapplicationid) ='" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and x.statusid="+Convert.ToInt32(Status.Active)+")t1 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype,'') = coalesce(t2.contacttype,'') and coalesce(t1.sectionname,'') = coalesce(t2.verifiedsectionname,'') and coalesce(trim(t1.subscetion),'') = coalesce(t2.verifiedsubsectionname,'');"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalEmployeementList.Add(new ApplicationPersonalEmployeementDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            papplicanttype = Convert.ToString(dr["applicanttype"]),
                            pisapplicable = Convert.ToBoolean(dr["isemploymentapplicable"]),
                            pemploymenttype = Convert.ToString(dr["employmenttype"]),
                            pnameoftheorganization = Convert.ToString(dr["nameoftheorganization"]),
                            pEnterpriseType = Convert.ToString(dr["natureoftheorganization"]),
                            pemploymentrole = Convert.ToString(dr["employmentrole"]),
                            pofficeaddress = Convert.ToString(dr["officeaddress"]),
                            pofficephoneno = Convert.ToString(dr["officephoneno"]),
                            preportingto = Convert.ToString(dr["reportingto"]),
                            pemployeeexp = Convert.ToInt32(dr["employeeexp"]),
                            pemployeeexptype = Convert.ToString(dr["employeeexptype"]),
                            ptotalworkexp = Convert.ToInt32(dr["totalworkexp"]),
                            pdateofestablishment = Convert.ToString(dr["dateofestablishment"]),
                            pdateofcommencement = Convert.ToString(dr["dateofcommencement"]),
                            pgstinno = Convert.ToString(dr["gstinno"]),
                            pcinno = Convert.ToString(dr["cinno"]),
                            pdinno = Convert.ToString(dr["dinno"]),
                            ptradelicenseno = Convert.ToString(dr["tradelicenseno"]),                         
                            SectionName = Convert.ToString(dr["sectionname"]),
                            psubsectionname= Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicationid,vchapplicationid, contactid,contactreferenceid,residentialstatus, maritalstatus, placeofbirth, countryofbirth, nationality,minoritycommunity FROM tabapplicationpersonalbirthdetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalDetailsList.Add(new ApplicationPersonalDetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToString(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            presidentialstatus = Convert.ToString(dr["residentialstatus"]),
                            pmaritalstatus = Convert.ToString(dr["maritalstatus"]),
                            pplaceofbirth = Convert.ToString(dr["placeofbirth"]),
                            pcountryofbirth = Convert.ToString(dr["countryofbirth"]),
                            pnationality = Convert.ToString(dr["nationality"]),
                            pminoritycommunity = Convert.ToString(dr["minoritycommunity"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(select t1.vchapplicationid,t1.applicantype,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(t1.totalnoofmembers,0) as totalnoofmembers,coalesce(t1.noofearningmembers,0) as noofearningmembers,coalesce(t1.familytype,'') as familytype,coalesce(t1.noofboyschild,0) as noofboyschild,coalesce(t1.noofgirlchild,0) as noofgirlchild,coalesce(t1.houseownership,'') as houseownership,'Personal Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalfamilydetails  t1  join TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid="+Convert.ToInt32(Status.Active)+")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and  coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname;"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalFamilyList.Add(new ApplicationPersonalFamilyDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            ptotalnoofmembers = Convert.ToInt32(dr["totalnoofmembers"]),
                            papplicanttype = Convert.ToString(dr["applicantype"]),
                            pnoofearningmembers = Convert.ToInt32(dr["noofearningmembers"]),
                            pfamilytype = Convert.ToString(dr["familytype"]),
                            pnoofboyschild = Convert.ToInt32(dr["noofboyschild"]),
                            pnoofgirlchild = Convert.ToInt32(dr["noofgirlchild"]),
                            phouseownership = Convert.ToString(dr["houseownership"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.vchapplicationid,t1.applicantype,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(t1.nomineename,'') as nomineename,coalesce(relationship,'') as relationship, dateofbirth,coalesce(contactno,'') as contactno,coalesce(idprooftype,'') as idprooftype,coalesce(idproofname,'') as idproofname,coalesce(referencenumber,'') as referencenumber,coalesce(docidproofpath,'') as docidproofpath,'Nominee Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalnomineedetails  t1  join  TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid="+Convert.ToInt32(Status.Active)+")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname;"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalNomineeList.Add(new ApplicationPersonalNomineeDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pnomineename = Convert.ToString(dr["nomineename"]),
                            prelationship = Convert.ToString(dr["relationship"]),
                            pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                            pcontactno = Convert.ToString(dr["contactno"]),
                            pidprooftype = Convert.ToString(dr["idprooftype"]),
                            pidproofname = Convert.ToString(dr["idproofname"]),
                            preferencenumber = Convert.ToString(dr["referencenumber"]),
                            pdocidproofpath = Convert.ToString(dr["docidproofpath"]),
                            papplicanttype = Convert.ToString(dr["applicantype"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),                     
                            ptypeofoperation = "OLD"

                        });
                    }
                }
               // select t1.*,t2.verifiedsectionname,verificationstatus from(SELECT recordid, applicantype, applicationid, vchapplicationid, contactid, contactreferenceid, bankname, accountno, ifsccode, branch, isprimarybank,'Bank Details' AS sectionname, coalesce(applicantype, '')|| '-' || coalesce(bankname, '') || '-' || coalesce(accountno, '') as subscetion FROM tabapplicationpersonalbankdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ") t1 left join(select vchapplicationid, verifiedsectionname, verifiedsubsectionname, contacttype, verificationstatus from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicantype,'') = coalesce(t2.contacttype, '') and coalesce(t1.sectionname,'') = coalesce(t2.verifiedsectionname, '') and coalesce(t1.subscetion,'')= coalesce(t2.verifiedsubsectionname, '')
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT x.recordid, x.applicantype,x.applicationid, x.vchapplicationid, x.contactid, x.contactreferenceid,x.bankname, x.accountno, x.ifsccode, x.branch, x.isprimarybank,'Bank Details' AS sectionname,coalesce(x.applicantype,'')||'-'||coalesce(y.name,'')||' '||coalesce(y.surname,'') as subscetion FROM tabapplicationpersonalbankdetails x  join TBLMSTCONTACT y on x.contactid=y.contactid where x.applicationid = " + applicationid + " and upper(x.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and x.statusid = " + Convert.ToInt32(Status.Active) + ") t1  left join(select vchapplicationid,verifiedsectionname,verifiedsubsectionname,contacttype,verificationstatus from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicantype,'') = coalesce(t2.contacttype,'') and coalesce(t1.sectionname,'') = coalesce(t2.verifiedsectionname,'') and coalesce(t1.subscetion,'')=coalesce(t2.verifiedsubsectionname,''); "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalBankList.Add(new ApplicationPersonalBankDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            //  pBankAccountname = dr["accountno"].ToString(),
                            pBankName = Convert.ToString(dr["bankname"]),
                            pBankAccountNo = Convert.ToString(dr["accountno"]),
                            pBankifscCode = Convert.ToString(dr["ifsccode"]),
                            pBankBranch = Convert.ToString(dr["branch"]),
                            pIsprimaryAccount = Convert.ToBoolean(dr["isprimarybank"].ToString()),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            papplicanttype= Convert.ToString(dr["applicantype"]),
                           // pContacttype = "Applicant",
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.applicationid,t1.applicantype, t1.vchapplicationid,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(grossannualincome,0) as grossannualincome,coalesce(netannualincome,0) as netannualincome,coalesce(averageannualexpenses,0) as averageannualexpenses,'Income Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalincomedetails  t1 left join  TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid="+Convert.ToInt32(Status.Active)+")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and trim(t3.subscetion) = t4.verifiedsubsectionname;"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalIncomeList.Add(new ApplicationPersonalIncomeDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pgrossannualincome = Convert.ToDecimal(dr["grossannualincome"])==0 ? 0 : Math.Round(Convert.ToDecimal(dr["grossannualincome"])/12),
                            paverageannualexpenses = Convert.ToDecimal(dr["averageannualexpenses"]) == 0 ? 0 : Math.Round(Convert.ToDecimal(dr["averageannualexpenses"])/12),
                            pnetannualincome= Convert.ToDecimal(dr["netannualincome"]) == 0 ? 0 : Math.Round(Convert.ToDecimal(dr["netannualincome"]) / 12),
                            pMonthlySavings =((Convert.ToDecimal(dr["netannualincome"]) == 0 ? 0 : Math.Round(Convert.ToDecimal(dr["netannualincome"]) / 12)) - (Convert.ToDecimal(dr["averageannualexpenses"]) == 0 ? 0 : Math.Round(Convert.ToDecimal(dr["averageannualexpenses"]) / 12))),
                            papplicanttype = Convert.ToString(dr["applicantype"]),
                            // pcontacttype= dr["surityapplicanttype"].ToString(),
                            SectionName = Convert.ToString(dr["sectionname"]),                          
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"

                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,applicationid,vchapplicationid,contactid,contactreferenceid,sourcename, grossannual FROM tabapplicationpersonalotherincomedetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalOtherIncomeList.Add(new ApplicationPersonalOtherIncomeDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            psourcename = Convert.ToString(dr["sourcename"]),
                            pgrossannual = Convert.ToDecimal(dr["grossannual"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.applicationid,t1.applicantype, t1.vchapplicationid,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(qualification,'') as qualification,coalesce(nameofthecourseorprofession,'') as nameofthecourseorprofession,coalesce(occupation,'') as occupation,'Education Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonaleducationdetails  t1  join  TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid="+Convert.ToInt32(Status.Active)+")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid  and coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname;"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.PersonalEducationList.Add(new ApplicationPersonalEducationDTO
                        {
                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pqualification = Convert.ToString(dr["qualification"]),
                            pnameofthecourseorprofession = Convert.ToString(dr["nameofthecourseorprofession"]),
                            poccupation = Convert.ToString(dr["occupation"]),
                            papplicanttype = Convert.ToString(dr["applicantype"]),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.applicationid,t1.applicantype, t1.vchapplicationid,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(t1.businessactivity,'') as businessactivity,coalesce(t1.establishmentdate,null) as establishmentdate,coalesce(t1.commencementdate,null) as commencementdate,coalesce(t1.cinnumber,'') as cinnumber,coalesce(t1.gstinno,'') as gstinno,'Business Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalbusinessdetails  t1  join  TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid=" + Convert.ToInt32(Status.Active) + ")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid  and coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname;"))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.BusinessDetailsDTOList.Add(new BusinessDetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pbusinessactivity = Convert.ToString(dr["businessactivity"]),
                            pestablishmentdate = dr["establishmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["establishmentdate"]).ToString("dd/MM/yyyy"),
                            pcommencementdate = dr["commencementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["commencementdate"]).ToString("dd/MM/yyyy"),
                            pcinnumber = Convert.ToString(dr["cinnumber"]),
                            papplicanttype= Convert.ToString(dr["applicantype"]),
                            pgstinno = Convert.ToString(dr["gstinno"]),
                            pcontacttype= Convert.ToString(dr["applicantype"]),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"
                        });
                    }
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, " select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT t1.applicationid,t1.applicantype, t1.vchapplicationid,coalesce(t1.recordid,0) as recordid,coalesce(t1.contactid,0) as contactid,coalesce(t1.contactreferenceid,'') as contactreferenceid,coalesce(t1.financialyear,'') as financialyear,coalesce(t1.turnoveramount,0) as turnoveramount,coalesce(t1.netprofitamount,0) as netprofitamount,coalesce(t1.docbalancesheetpath,'') as docbalancesheetpath,'Business Financial Details' AS sectionname,coalesce(t1.applicantype, '')||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as subscetion FROM tabapplicationpersonalbusinessfinancialdetails  t1  join  TBLMSTCONTACT t2 on t1.contactid=t2.contactid  where upper(t1.vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "' and t1.statusid=" + Convert.ToInt32(Status.Active) + ")t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid  and coalesce(t3.applicantype,'') = coalesce(t4.contacttype,'') and coalesce(t3.sectionname,'') = coalesce(t4.verifiedsectionname,'') and t3.subscetion = t4.verifiedsubsectionname; "))
                {
                    while (dr.Read())
                    {
                        ApplicationPersonalInformationDTO.businessfinancialdetailsDTOList.Add(new businessfinancialdetailsDTO
                        {

                            precordid = Convert.ToInt64(dr["recordid"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pfinancialyear = Convert.ToString(dr["financialyear"]),
                            pturnoveramount = Convert.ToDecimal(dr["turnoveramount"]),
                            pnetprofitamount = Convert.ToDecimal(dr["netprofitamount"]),
                            pdocbalancesheetpath = Convert.ToString(dr["docbalancesheetpath"]),
                            papplicanttype = Convert.ToString(dr["applicantype"]),
                            pcontacttype = Convert.ToString(dr["applicantype"]),
                            SectionName = Convert.ToString(dr["sectionname"]),
                            psubsectionname = Convert.ToString(dr["subscetion"]),
                            IsVerified = Convert.ToString(dr["verificationstatus"]),
                            ptypeofoperation = "OLD"
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
        public ApplicationSecurityandCollateralDTO FIDocumentgetSecurityCollateralDetails(long applicationid, string strapplicationid, string ConnectionString)
        {

            try
            {

                ApplicationSecurityandCollateralDTO = new ApplicationSecurityandCollateralDTO();

                bool issecurityandcolletralapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select issecurityandcolletralapplicable from tabapplication where upper(vchapplicationid) = '" +ManageQuote(strapplicationid.Trim().ToUpper()) + "';"));
                ApplicationSecurityandCollateralDTO.pissecurityandcolletralapplicable = issecurityandcolletralapplicable;
                if (issecurityandcolletralapplicable == true)
                {

                    ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList = new List<ApplicationSecurityandCollateralImMovablePropertyDetails>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT recordid,contactid,vchapplicationid,contactreferenceid,typeofproperty,titledeed,deeddate,propertyownername,addressofproperty,coalesce(estimatedmarketvalue,0) as estimatedmarketvalue,coalesce(propertydocpath,'') as propertydocpath,coalesce(filename,'')  as propertydocpathname,'Security and Collateral' AS sectionname,coalesce(TYPEOFPROPERTY,'') as subscetion, 'Applicant' as applicanttype from tabapplicationsecuritycollateralimmovablepropertydetails t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE'  and upper(vchapplicationid)='" + ManageQuote(strapplicationid.Trim().ToUpper()) + "') t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" +ManageQuote(strapplicationid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and t3.applicanttype = t4.contacttype and t3.sectionname = t4.verifiedsectionname and t3.subscetion=t4.verifiedsubsectionname; "))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList.Add(new ApplicationSecurityandCollateralImMovablePropertyDetails
                            {

                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pTypeofproperty = Convert.ToString(dr["typeofproperty"]),
                                pTitledeed = Convert.ToString(dr["titledeed"]),
                                pDeeddate = dr["deeddate"] == DBNull.Value ? null : Convert.ToDateTime(dr["deeddate"]).ToString("dd/MM/yyyy"),
                                pPropertyownername = Convert.ToString(dr["propertyownername"]),
                                pAddressofproperty = Convert.ToString(dr["addressofproperty"]),
                                pEstimatedmarketvalue = Convert.ToDecimal(dr["estimatedmarketvalue"]),
                                pPropertydocpath = Convert.ToString(dr["propertydocpath"]),
                                pPropertydocpathname = Convert.ToString(dr["propertydocpathname"]),
                                SectionName = Convert.ToString(dr["sectionname"]),
                                psubsectionname = Convert.ToString(dr["subscetion"]),
                                IsVerified = Convert.ToString(dr["verificationstatus"]),
                                pContactTYpe = "Applicant",
                                ptypeofoperation="OLD"


                            }) ;
                        }
                    }
                    ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList = new List<ApplicationSecurityandCollateralMovablePropertyDetails>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t3.*,t4.verifiedsectionname,verificationstatus from(SELECT recordid,vchapplicationid,contactid,contactreferenceid,typeofvehicle,vehicleownername,vehiclemodelandmake,registrationno,coalesce(estimatedmarketvalue,0) as estimatedmarketvalue,vehicledocpath,'Security and Collateral' AS sectionname,coalesce(typeofvehicle,'')||''||coalesce(registrationno,'') as subscetion, 'Applicant' as applicanttype from tabapplicationsecuritycollateralmovablepropertydetails  t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE' and upper(vchapplicationid)='" +ManageQuote(strapplicationid.Trim().ToUpper()) + "') t3 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" +ManageQuote(strapplicationid.Trim().ToUpper()) + "') t4 on t3.vchapplicationid = t4.vchapplicationid and t3.applicanttype = t4.contacttype and t3.sectionname = t4.verifiedsectionname and t3.subscetion=t4.verifiedsubsectionname;"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList.Add(new ApplicationSecurityandCollateralMovablePropertyDetails
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                PTypeofvehicle = Convert.ToString(dr["typeofvehicle"]),
                                pVehicleownername = Convert.ToString(dr["vehicleownername"]),
                                pVehiclemodelandmake = Convert.ToString(dr["vehiclemodelandmake"]),
                                pRegistrationno = Convert.ToString(dr["registrationno"]),
                                pEstimatedmarketvalue = Convert.ToDecimal(dr["estimatedmarketvalue"]),
                                pVehicledocpath = Convert.ToString(dr["vehicledocpath"]),
                                SectionName = Convert.ToString(dr["sectionname"]),
                                psubsectionname = Convert.ToString(dr["subscetion"]),
                                IsVerified = Convert.ToString(dr["verificationstatus"]),
                                pContactTYpe = "Applicant",
                                ptypeofoperation = "OLD"
                            });
                        }
                    }


                    ApplicationSecurityandCollateralDTO.SecuritychequesList = new List<ApplicationSecurityandCollateralSecuritycheques>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicanttype, contactid, contactreferenceid, typeofsecurity, bankname, ifsccode, accountno, chequeno,securitychequesdocpath from tabapplicationsecuritycollateralsecuritycheques t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE' and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.SecuritychequesList.Add(new ApplicationSecurityandCollateralSecuritycheques
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pApplicanttype = Convert.ToString(dr["applicanttype"]),
                                pTypeofsecurity = Convert.ToString(dr["typeofsecurity"]),
                                pBankname = Convert.ToString(dr["bankname"]),
                                pIfsccode = Convert.ToString(dr["ifsccode"]),
                                pAccountno = Convert.ToString(dr["accountno"]),
                                pChequeno = Convert.ToString(dr["chequeno"]),
                                pSecuritychequesdocpath = Convert.ToString(dr["securitychequesdocpath"])
                            });
                        }
                    }


                    ApplicationSecurityandCollateralDTO.DepositsasLienList = new List<ApplicationSecurityandCollateralDepositsasLien>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,contactid,contactreferenceid,depositin,typeofdeposit,depositorbank,depositaccountno,coalesce(depositamount,0) as depositamount,coalesce(rateofinterest,0) as rateofinterest,depositdate,tenureofdeposit,deposittenuretype,maturitydate,depositdocpath from tabapplicationsecuritycollateraldepositslien  t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE'  and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.DepositsasLienList.Add(new ApplicationSecurityandCollateralDepositsasLien
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pDepositin = Convert.ToString(dr["depositin"]),
                                pTypeofdeposit = Convert.ToString(dr["typeofdeposit"]),
                                pDepositorbank = Convert.ToString(dr["depositorbank"]),
                                pDepositaccountno = Convert.ToString(dr["depositaccountno"]),
                                pDepositamount = Convert.ToDecimal(dr["depositamount"]),
                                pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]),
                                pDepositdate = Convert.ToString(dr["depositdate"]),
                                pTenureofdeposit = Convert.ToString(dr["tenureofdeposit"]),
                                pDeposittenuretype = Convert.ToString(dr["deposittenuretype"]),
                                pMaturitydate = Convert.ToString(dr["maturitydate"]),
                                pDepositdocpath = Convert.ToString(dr["depositdocpath"])
                            });
                        }
                    }

                    ApplicationSecurityandCollateralDTO.otherPropertyorsecurityDetailsList = new List<ApplicationSecurityandCollateralOtherPropertyorsecurityDetails>();

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,contactid,contactreferenceid,nameofthesecurity,coalesce(estimatedvalue,0) as estimatedvalue, securitydocpath from tabapplicationsecuritycollateralotherpropertyorsecuritydetails  t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE'  and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Add(new ApplicationSecurityandCollateralOtherPropertyorsecurityDetails
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pNameofthesecurity = Convert.ToString(dr["nameofthesecurity"]),
                                pEstimatedvalue = Convert.ToDecimal(dr["estimatedvalue"]),
                                pSecuritydocpath = Convert.ToString(dr["securitydocpath"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return ApplicationSecurityandCollateralDTO;
        }
        public List<ApplicationExistingLoanDetailsDTO> FIDocumentGetApplicationExistingLoanDetails(string contactreferenceid, string vchapplicationid, string con)
        {
            lstApplicationExistingLoanDetails = new List<ApplicationExistingLoanDetailsDTO>();
            string Query = string.Empty;
            int count = 0;
            try
            {

                if (vchapplicationid != null)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tabapplicationexistingloans where upper(vchapplicationid) = '" +ManageQuote(vchapplicationid.Trim().ToUpper()) + "';"));
                }
                //if (count > 0)
                //{
                    Query = "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(select  recordid,contactid as applicantid,vchapplicationid,contactreferenceid,typeoflender,  bankorcreditfacilityname, loanno,loanname,tenureofloan, rateofinterest,loanpayin,instalmentamount,loanamount,loansanctiondate,coalesce(remainingtenureofloan,0) as remainingtenureofloan, coalesce(principleoutstanding,0) as principleoutstanding,'OLD' as typeofoperation,ts.statusname,'Existing Loan' AS sectionname,coalesce(loanno,'') as subscetion,'Applicant' as applicanttype from tabapplicationexistingloans t1 join tblmststatus ts on t1.statusid=ts.statusid where upper(vchapplicationid)='" +ManageQuote(vchapplicationid.Trim().ToUpper()) + "' and upper(ts.statusname) = 'ACTIVE')t1  left join(select vchapplicationid,verifiedsectionname,verifiedsubsectionname,contacttype,verificationstatus from tabapplicationFIverification  where upper(vchapplicationid) = '" +ManageQuote(vchapplicationid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and t1.applicanttype = t2.contacttype and t1.sectionname = t2.verifiedsectionname and t1.subscetion=t2.verifiedsubsectionname;";
                   // Query = "select  recordid,contactid as applicantid,contactreferenceid,typeoflender,  bankorcreditfacilityname, loanno,loanname,tenureofloan, rateofinterest,loanpayin,instalmentamount,loanamount,loansanctiondate,coalesce(remainingtenureofloan,0) as remainingtenureofloan, coalesce(principleoutstanding,0) as principleoutstanding,'OLD' as typeofoperation,ts.statusname from tabapplicationexistingloans t1 join tblmststatus ts on t1.statusid=ts.statusid where vchapplicationid='" + vchapplicationid + "' and upper(ts.statusname) = 'ACTIVE'";
                //}
                //else
                //{
                //    Query = "select  0 as recordid,applicantid,contactreferenceid,'company' as typeoflender,  null as bankorcreditfacilityname, vchapplicationid as loanno, loanname,tenureofloan, rateofinterest,loanpayin,instalmentamount,coalesce(amountrequested,0) as loanamount,dateofapplication as loansanctiondate, coalesce(tenureofloan,0) as remainingtenureofloan, amountrequested as principleoutstanding,'CREATE' as typeofoperation,ts.statusname from tabapplication t1 join tblmststatus ts on t1.statusid=ts.statusid where contactreferenceid='" + contactreferenceid + "' and upper(ts.statusname) = 'ACTIVE'";
                //}
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                    while (dr.Read())
                    {
                        ApplicationExistingLoanDetailsDTO obj = new ApplicationExistingLoanDetailsDTO();
                        obj.pRecordid = Convert.ToInt64(dr["recordid"]);
                        //obj.pApplicationid = Convert.ToInt64(dr["applicationid"]);
                        obj.pContactid = Convert.ToInt64(dr["applicantid"]);
                        obj.pContactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                        if (dr["bankorcreditfacilityname"] != null)
                        {
                            obj.pbankorcreditfacilityname = Convert.ToString(dr["bankorcreditfacilityname"]);
                        }
                        obj.pLoanno = Convert.ToString(dr["loanno"]);
                        obj.pLoanname = Convert.ToString(dr["loanname"]);
                        obj.pTenureofloan = Convert.ToInt64(dr["tenureofloan"]);
                        obj.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                        obj.pLoanpayin = Convert.ToString(dr["loanpayin"]);
                        obj.pContactType = "Applicant";
                        obj.pInstalmentamount = Convert.ToDecimal(dr["instalmentamount"]);
                        obj.ploanamount = Convert.ToDecimal(dr["loanamount"]);
                        obj.pLoansanctiondate = Convert.ToString(dr["loansanctiondate"]);
                        obj.premainingTenureofloan = Convert.ToInt64(dr["remainingtenureofloan"]);
                        obj.pPrincipleoutstanding = Convert.ToInt64(dr["principleoutstanding"]);
                        obj.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        obj.pTypeofLender = Convert.ToString(dr["typeoflender"]);
                        obj.pStatusname = Convert.ToString(dr["statusname"]);
                        obj.SectionName = Convert.ToString(dr["sectionname"]);
                        obj.psubsectionname = Convert.ToString(dr["subscetion"]);
                       
                            obj.IsVerified = Convert.ToString(dr["verificationstatus"]);
                            obj.ptypeofoperation = "OLD";
                        
                        lstApplicationExistingLoanDetails.Add(obj);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstApplicationExistingLoanDetails;
        }
        public ApplicationReferencesDTO FIDocumentGetApplicationReferenceData(long applicationId, string vchapplicationID, string connectionString)
        {
            List<ApplicationLoanReferencesDTO> lObjloanAppReference = new List<ApplicationLoanReferencesDTO>();
            ApplicationReferencesDTO objReferences = new ApplicationReferencesDTO();

            try
            {

                if (!string.IsNullOrEmpty(vchapplicationID))
                {
                    applicationId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select applicationid from tabapplication where upper(vchapplicationid) = '" +ManageQuote(vchapplicationID.Trim().ToUpper()) + "';"));
                }
                bool Isreferencesapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select isreferencesapplicable from tabapplication where upper(vchapplicationid) = '" +ManageQuote(vchapplicationID.Trim().Trim()) + "';"));
                objReferences.pIsreferencesapplicable = Isreferencesapplicable;

                if (Isreferencesapplicable == true)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT recordid, applicationid, vchapplicationid, firstname, lastname, coalesce(contactnumber, 0) as contactnumber, coalesce(alternatenumber, 0) as alternatenumber, emailid, alternateemailid, 'Reference' AS sectionname, 'Applicant' as applicanttype, coalesce(firstname, '') || ' ' || coalesce(lastname, '') as subscetion FROM tabapplicationreferences where  upper(vchapplicationid) = '" + ManageQuote(vchapplicationID).Trim().ToUpper() + "' and statusid ="+Convert.ToInt32(Status.Active)+")t1 left join(select vchapplicationid, verifiedsectionname, verifiedsubsectionname, contacttype, verificationstatus from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(vchapplicationID).Trim().ToUpper() + "') t2 on t1.vchapplicationid = t2.vchapplicationid and t1.applicanttype = t2.contacttype and t1.sectionname = t2.verifiedsectionname and t1.subscetion = t2.verifiedsubsectionname;"))
                    // using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT recordid,applicationid, vchapplicationid, firstname, lastname,coalesce( contactnumber,0) as contactnumber,coalesce( alternatenumber,0) as alternatenumber, emailid, alternateemailid FROM tabapplicationreferences where applicationId=" + applicationId + " and upper(vchapplicationid)='" + ManageQuote(vchapplicationID).Trim().ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            ApplicationLoanReferencesDTO ObjloanAppReference = new ApplicationLoanReferencesDTO();
                            ObjloanAppReference.pRefRecordId = Convert.ToInt64(dr["recordid"]);
                            ObjloanAppReference.papplicationId = Convert.ToInt64(dr["applicationid"]);
                            ObjloanAppReference.pvchapplicationId = Convert.ToString(dr["vchapplicationid"]);
                            ObjloanAppReference.pApplicanttype= Convert.ToString(dr["applicanttype"]);
                            ObjloanAppReference.pRefFirstname = Convert.ToString(dr["firstname"]);
                            ObjloanAppReference.pRefLastname = Convert.ToString(dr["lastname"]);
                            ObjloanAppReference.pRefcontactNo = Convert.ToDecimal(dr["contactnumber"]);
                            ObjloanAppReference.pRefalternatecontactNo = Convert.ToDecimal(dr["alternatenumber"]);
                            ObjloanAppReference.pRefEmailID = Convert.ToString(dr["emailid"]);
                            ObjloanAppReference.pRefAlternateEmailId = Convert.ToString(dr["alternateemailid"]);
                            ObjloanAppReference.SectionName= Convert.ToString(dr["sectionname"]);
                            ObjloanAppReference.psubsectionname = Convert.ToString(dr["subscetion"]);                           
                            ObjloanAppReference.IsVerified = Convert.ToString(dr["verificationstatus"]);
                            ObjloanAppReference.ptypeofoperation = "OLD";
                           
                            lObjloanAppReference.Add(ObjloanAppReference);
                        }
                    }
                    if (lObjloanAppReference.Count > 0)
                    {
                        objReferences.LobjAppReferences = lObjloanAppReference;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }


            return objReferences;
        }
        public async Task<FIDocumentViewDTO> GetFIDocumentView(string strapplictionid, string ConnectionString)
        {
            string ContactRefID = string.Empty;
            long Applicantid = 0;
            long applicationid = 0;
            FIDocumentViewDTO FIDocumentViewDTO = new FIDocumentViewDTO();
            FIDocumentViewDTO.FirstinformationDTO = new List<FirstinformationDTO>();
            FIDocumentViewDTO.ApplicationPersonal = new ApplicationPersonalInformationDTO();
            FIDocumentViewDTO.ApplicationSecurityandCollateralDTO = new ApplicationSecurityandCollateralDTO();
            FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO = new List<ApplicationExistingLoanDetailsDTO>();
            FIDocumentViewDTO.ApplicationReferencesDTO = new ApplicationReferencesDTO();
            FIDocumentViewDTO.ApplicationKYCDocumentsDTO = new ApplicationKYCDocumentsDTO();
            FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO = new List<ApplicationContactPersonalDetailsDTO>();
            FIDocumentViewDTO.lstcontactPersonalAddressDTO = new List<contactPersonalAddressDTO>();
            FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification = new ApplicationLoanSpecificDTOinVerification();
            await Task.Run(() =>
            {
                try
                {
                    FIDocumentViewDTO.FirstinformationDTO = FIDocumentViewGetloandetails(strapplictionid, ConnectionString);
                    if (FIDocumentViewDTO.FirstinformationDTO.Count > 0)
                    {
                        ContactRefID = FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid;
                        Applicantid = FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid;
                        applicationid = FIDocumentViewDTO.FirstinformationDTO[0].papplicationid;
                        FIDocumentViewDTO.pFIVerifierscomments = FIDocumentViewDTO.FirstinformationDTO[0].pFIVerifierscomments;
                        FIDocumentViewDTO.pFIVerifiersrating = FIDocumentViewDTO.FirstinformationDTO[0].pFIVerifiersrating;
                        FIDocumentViewDTO.pverificationdate = FIDocumentViewDTO.FirstinformationDTO[0].pverificationdate;
                        FIDocumentViewDTO.pverificationtime = FIDocumentViewDTO.FirstinformationDTO[0].pverificationtime;
                        FIDocumentViewDTO.ApplicationKYCDocumentsDTO = FIDocumentGetApplicantCreditandkycdetails(strapplictionid, Applicantid, ConnectionString);
                        FIDocumentViewDTO.ApplicationPersonal = FIDocumentGetApplicationPersonalInformation(strapplictionid, ConnectionString);
                        FIDocumentViewDTO.ApplicationSecurityandCollateralDTO = FIDocumentgetSecurityCollateralDetails(applicationid, strapplictionid, ConnectionString);
                        FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO = FIDocumentGetApplicationExistingLoanDetails(ContactRefID, strapplictionid, ConnectionString);
                        FIDocumentViewDTO.ApplicationReferencesDTO = FIDocumentGetApplicationReferenceData(Applicantid, strapplictionid, ConnectionString);
                        FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO = GetFIDocumentkycDetails(strapplictionid, ConnectionString);
                        FIDocumentViewDTO.lstcontactPersonalAddressDTO = GetFIDocumentAddressDetails(strapplictionid, ConnectionString);
                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification = GetApplicantLoanSpecificDetailsinVerification(strapplictionid, ConnectionString);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return FIDocumentViewDTO;

        }


        #region BindLoans - Loan Specific in Verification
        public ApplicationLoanSpecificDTOinVerification GetApplicantLoanSpecificDetailsinVerification(string strapplictionid, string ConnectionString)
        {
           
                _ApplicationLoanSpecificDTOinVerification = new ApplicationLoanSpecificDTOinVerification();
                string LoanType = string.Empty;
                string ContactRefId = string.Empty;
                Int64 Applicatid = 0;
                DataSet ds1 = new DataSet();
                long applicationid = 0;

                try
                {
                    ds1 = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select applicationid,loantype,applicantid,contactreferenceid from tabapplication  where  vchapplicationid = '" + strapplictionid + "';");
                    if (ds1 != null)
                    {
                        LoanType = ds1.Tables[0].Rows[0]["loantype"].ToString().ToUpper().Trim();
                        ContactRefId = ds1.Tables[0].Rows[0]["contactreferenceid"].ToString();
                        Applicatid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicantid"]);
                        applicationid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicationid"]);
                    }
                    _ApplicationLoanSpecificDTOinVerification.pLoantype = LoanType.ToUpper().Trim();
                    _ApplicationLoanSpecificDTOinVerification.pVchapplicationid = strapplictionid;
                    _ApplicationLoanSpecificDTOinVerification.pApplicationid = applicationid;
                    _ApplicationLoanSpecificDTOinVerification.pApplicantid = Applicatid;
                    _ApplicationLoanSpecificDTOinVerification.pContactreferenceid = ContactRefId;
                    if (LoanType == "CONSUMER LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO = new ConsumerLoanDTOVerification();
                        _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO = new List<ConsumerLoanDetailsDTOVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, totalproductcost, downpayment, balanceamount FROM tblapplicationconsumerloan where applicationid =" + applicationid + " and vchapplicationid='" + strapplictionid + "' AND STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.ptotalproductcost = Convert.ToDecimal(dr["totalproductcost"]);
                                _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.pdownpayment = Convert.ToDecimal(dr["downpayment"]);
                                _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.pbalanceamount = Convert.ToDecimal(dr["balanceamount"]);
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype,tl.recordid ,producttype, productname, manufacturer, productmodel, quantity,costofproduct,insurancecostoftheproduct, othercost, totalcostofproduct,iswarrantyapplicable, period,periodtype,'Product Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationconsumerloanproductdetails tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Add(new ConsumerLoanDetailsDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pproducttype = dr["producttype"].ToString(),
                                    pproductname = dr["productname"].ToString(),
                                    pmanufacturer = dr["manufacturer"].ToString(),
                                    pproductmodel = dr["productmodel"].ToString(),
                                    pquantity = Convert.ToInt64(dr["quantity"]),
                                    pcostofproduct = Convert.ToDecimal(dr["costofproduct"]),
                                    pinsurancecostoftheproduct = Convert.ToDecimal(dr["insurancecostoftheproduct"]),
                                    pothercost = Convert.ToDecimal(dr["othercost"]),
                                    ptotalcostofproduct = Convert.ToDecimal(dr["totalcostofproduct"]),
                                    piswarrantyapplicable = Convert.ToBoolean(dr["iswarrantyapplicable"].ToString()),
                                    pperiod = dr["period"].ToString(),
                                    pperiodtype = dr["periodtype"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    #region Loan Related Binding
                    if (LoanType == "VEHICLE LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO = new List<VehicleLoanDTOVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t1.verifiedsubsectionname,t2.verifiedsubsectionname,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid, 'Applicant' as applicanttype, showroomname, vehiclemanufacture, VehicleModel, actualvehiclecost,downpayment, onroadprice, requestedamount, engineno, chasisno, registrationno, yearofmake, remarks, 'Vehicle Details' as verifiedsectionname, 'Applicant' || '-' || coalesce(t2.name, '') || ' ' || coalesce(t2.surname, '') as verifiedsubsectionname FROM tabapplicationvehicleloan tl join  TBLMSTCONTACT t2 on tl.contactid = t2.contactid where tl.applicationid = " + applicationid + "  and tl.statusid = "+Convert.ToInt32(Status.Active)+")t1 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO.Add(new VehicleLoanDTOVerification
                                {
                                    pShowroomName = dr["showroomname"].ToString(),
                                    pVehicleManufacturer = dr["vehiclemanufacture"].ToString(),
                                    pVehicleModel = dr["VehicleModel"].ToString(),
                                    pActualcostofVehicle = Convert.ToDecimal(dr["actualvehiclecost"]),
                                    pDownPayment = Convert.ToDecimal(dr["downpayment"]),
                                    pOnroadprice = Convert.ToDecimal(dr["onroadprice"]),
                                    pRequestedLoanAmount = Convert.ToDecimal(dr["requestedamount"]),
                                    pEngineNo = dr["engineno"].ToString(),
                                    pChassisNo = dr["chasisno"].ToString(),
                                    pRegistrationNo = dr["registrationno"].ToString(),
                                    pYearofMake = dr["yearofmake"].ToString(),
                                    pAnyotherRemarks = dr["remarks"].ToString(),
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    if (LoanType == "GOLD LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO = new GoldLoanDTOVerification();
                        _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO = new List<GoldLoanDetailsDTOVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, totalappraisedvalue,appraisaldate, appraisorname FROM tblapplicationgoldloan  where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.pTotalAppraisedValue = Convert.ToDecimal(dr["totalappraisedvalue"]);
                                _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.pAppraisalDate = dr["appraisaldate"].ToString();

                                _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.pAppraisorName = dr["appraisorname"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t1.verifiedsubsectionname,t2.verifiedsubsectionname,t2.verifiedsectionname,verificationstatus from(SELECT t.vchapplicationid, 'Applicant' as applicanttype, tl.recordid, goldarticletype, detailsofarticle, carat, grossweight, netweight, appraisedvalueofarticle, observations, articledocpath, COALESCE(docname, '') as docname, 'Gold Details' as verifiedsectionname, 'Applicant' || '-' || 'Gold Details' as verifiedsubsectionname FROM tblapplicationgoldloan t join tabapplicationgoldloandetails tl on t.recordid = tl.detailsid join  TBLMSTCONTACT t2 on t.contactid = t2.contactid where t.applicationid = " + applicationid + " and tl.statusid = "+Convert.ToInt32(Status.Active)+")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + ManageQuote(strapplictionid.Trim().ToUpper()) + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Add(new GoldLoanDetailsDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pGoldArticleType = dr["goldarticletype"].ToString(),
                                    pDetailsofGoldArticle = dr["detailsofarticle"].ToString(),
                                    pCarat = dr["carat"].ToString(),
                                    pGrossweight = Convert.ToDecimal(dr["grossweight"]),
                                    pNetWeight = Convert.ToDecimal(dr["netweight"]),
                                    pAppraisedValue = Convert.ToDecimal(dr["appraisedvalueofarticle"]),
                                    pobservations = dr["observations"].ToString(),
                                    pUploadfilename = dr["docname"].ToString(),
                                    pGoldArticlePath = dr["articledocpath"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    if (LoanType == "EDUCATION LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO = new EducationLoanDTOVerification();
                        _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO = new List<EducationInstutiteAddressDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO = new List<EducationLoanFeeDetailsDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO = new List<EducationLoanyearwiseFeedetailsDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO = new List<EducationQualifcationDTOVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, nameoftheinstitution, nameofproposedcourse, reasonforselectionoftheinstitute,rankingofinstitution, durationofcourse, dateofcommencement, reasonforseatsecured,'Educational Institution and Course Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid  where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pNameoftheinstitution = dr["nameoftheinstitution"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pNameofProposedcourse = dr["nameofproposedcourse"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pselectionoftheinstitute = dr["reasonforselectionoftheinstitute"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pRankingofinstitution = dr["rankingofinstitution"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pDurationofCourse = dr["durationofcourse"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pDateofCommencement = dr["dateofcommencement"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofcommencement"]).ToString("dd/MM/yyyy");
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.pseatsecured = dr["reasonforseatsecured"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.SectionName = dr["verifiedsectionname"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.psubsectionname = dr["verifiedsubsectionname"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.IsVerified = dr["verificationstatus"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, address1, address2, city, state, district, country, pincode,stateid,districtid,countryid,'Address of Institution' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname   FROM tabapplicationeducationloaninstituteaddress tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO.Add(new EducationInstutiteAddressDTOVerification
                                {
                                    pAddress1 = dr["address1"].ToString(),
                                    pAddress2 = dr["address2"].ToString(),
                                    pCity = dr["city"].ToString(),
                                    pState = dr["state"].ToString(),
                                    pDistrict = dr["district"].ToString(),
                                    pCountry = dr["country"].ToString(),
                                    pPincode = dr["pincode"].ToString(),
                                    pStateid = dr["stateid"] == DBNull.Value ? 0 : Convert.ToInt32(dr["stateid"]),
                                    pDistrictid = dr["districtid"] == DBNull.Value ? 0 : Convert.ToInt32(dr["districtid"]),
                                    pCountryid = dr["countryid"] == DBNull.Value ? 0 : Convert.ToInt32(dr["countryid"]),
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*, t2.verifiedsectionname, verificationstatus from(SELECT vchapplicationid, 'Applicant' as applicanttype, tl.recordid, qualification, institute, yearofpassing, noofattempts, markspercentage, grade, isscholarshipsapplicable, scholarshiporprize, scholarshipname, 'Educational Qualification' as verifiedsectionname, 'Applicant' || '-' || coalesce(t2.name, '') || ' ' || coalesce(t2.surname, '') as verifiedsubsectionname FROM tabapplicationeducationloanqualificationdetails  tl join  TBLMSTCONTACT t2 on tl.contactid = t2.contactid  where tl.applicationid = " + applicationid + " and tl.statusid = " + Convert.ToInt32(Status.Active) + ")t1 left join(select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname),'');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO.Add(new EducationQualifcationDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pqualification = dr["qualification"].ToString(),
                                    pinstitute = dr["institute"].ToString(),
                                    pyearofpassing = dr["yearofpassing"].ToString(),
                                    pnoofattempts = dr["noofattempts"].ToString(),
                                    pmarkspercentage = dr["markspercentage"].ToString(),
                                    pgrade = dr["grade"].ToString(),
                                    pisscholarshipsapplicable = Convert.ToBoolean(dr["isscholarshipsapplicable"]),
                                    pscholarshiporprize = dr["scholarshiporprize"].ToString(),
                                    pscholarshipname = dr["scholarshipname"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,totalfundrequirement, nonrepayablescholarship, repayablescholarship,fundsavailablefromfamily,'Source of Funds' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloanfeedetails  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid  where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Add(new EducationLoanFeeDetailsDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    ptotalfundrequirement = Convert.ToDecimal(dr["totalfundrequirement"]),
                                    pnonrepayablescholarship = dr["nonrepayablescholarship"].ToString(),
                                    prepayablescholarship = dr["repayablescholarship"].ToString(),
                                    pfundsavailablefromfamily = Convert.ToDecimal(dr["fundsavailablefromfamily"]),
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,year, qualification, fee,'Fee Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloanyearwisefeedetails  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '') and t1.recordid = t2.contactreferenceid::int;"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Add(new EducationLoanyearwiseFeedetailsDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pyear = dr["year"].ToString(),
                                    pqualification = dr["qualification"].ToString(),
                                    pfee = Convert.ToDecimal(dr["fee"]),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    if (LoanType == "LOAN AGAINST DEPOSITS")
                    {
                        _ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO = new List<LoanagainstDepositDTOVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,deposittype, bankcreditfacility, depositaccountnumber, depositamount,depositinterestpercentage, depositdate, deposittenure, depositdocpath,filename,'Loan against Deposits' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationdepositloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), ''); "))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO.Add(new LoanagainstDepositDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pdeposittype = dr["deposittype"].ToString(),
                                    pbankcreditfacility = dr["bankcreditfacility"].ToString(),
                                    pdepositaccountnumber = dr["depositaccountnumber"].ToString(),
                                    pdepositamount = Convert.ToDecimal(dr["depositamount"]),
                                    pdepositinterestpercentage = Convert.ToDecimal(dr["depositinterestpercentage"]),
                                    pdepositdate = dr["depositdate"].ToString(),
                                    pdeposittenure = dr["deposittenure"].ToString(),
                                    pdepositdocpath = dr["depositdocpath"].ToString(),
                                    pUploadfilename = dr["filename"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    if (LoanType == "HOME LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst = new List<HomeLoanDTOVerification>();
                        // _HomeLoanDTOVerification.BalanceTransferDTO = new BalanceTransferDTO();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype,tl.recordid,initialpayment, propertylocation, propertyownershiptype, propertytype, purpose, propertystatus, address1, address2, city, state, district,country, pincode, buildertieup, projectname, ownername, selleraddress,buildingname, blockname, builtupareain, plotarea, undividedshare, plintharea, bookingdate, completiondate, occupancycertificatedate, actualcost, saleagreementvalue, stampdutycharges, otheramenitiescharges, otherincidentalexpenditure, totalvalueofproperty, ageofbuilding,originalcostofproperty, estimatedvalueofrepairs, amountalreadyspent,otherborrowings, totalvalue ,'Home Loan Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationhomeloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), ''); "))
                        {
                            while (dr.Read())
                            {
                                HomeLoanDTOVerification _HomeLoanDTOVerification = new HomeLoanDTOVerification();

                                _HomeLoanDTOVerification.pRecordid = Convert.ToInt64(dr["recordid"]);
                                _HomeLoanDTOVerification.pinitialpayment = Convert.ToDecimal(dr["initialpayment"]);
                                _HomeLoanDTOVerification.ppropertylocation = dr["propertylocation"].ToString();
                                _HomeLoanDTOVerification.ppropertyownershiptype = dr["propertyownershiptype"].ToString();
                                _HomeLoanDTOVerification.ppropertytype = dr["propertytype"].ToString();
                                _HomeLoanDTOVerification.ppurpose = dr["purpose"].ToString();
                                _HomeLoanDTOVerification.ppropertystatus = dr["propertystatus"].ToString();
                                _HomeLoanDTOVerification.paddress1 = dr["address1"].ToString();
                                _HomeLoanDTOVerification.paddress2 = dr["address2"].ToString();
                                _HomeLoanDTOVerification.pcity = dr["city"].ToString();
                                _HomeLoanDTOVerification.pstate = dr["state"].ToString();
                                _HomeLoanDTOVerification.pdistrict = dr["district"].ToString();
                                _HomeLoanDTOVerification.pcountry = dr["country"].ToString();
                                _HomeLoanDTOVerification.ppincode = dr["pincode"].ToString();
                                _HomeLoanDTOVerification.pbuildertieup = dr["buildertieup"].ToString();
                                _HomeLoanDTOVerification.pprojectname = dr["projectname"].ToString();
                                _HomeLoanDTOVerification.pownername = dr["ownername"].ToString();
                                _HomeLoanDTOVerification.pselleraddress = dr["selleraddress"].ToString();
                                _HomeLoanDTOVerification.pbuildingname = dr["buildingname"].ToString();
                                _HomeLoanDTOVerification.pblockname = dr["blockname"].ToString();
                                _HomeLoanDTOVerification.pbuiltupareain = Convert.ToDecimal(dr["builtupareain"]);
                                _HomeLoanDTOVerification.pplotarea = Convert.ToDecimal(dr["plotarea"]);
                                _HomeLoanDTOVerification.pundividedshare = Convert.ToDecimal(dr["undividedshare"]);
                                _HomeLoanDTOVerification.pplintharea = Convert.ToDecimal(dr["plintharea"]);
                                _HomeLoanDTOVerification.pbookingdate = dr["bookingdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["bookingdate"]).ToString("dd/MM/yyyy");
                                _HomeLoanDTOVerification.pcompletiondate = dr["completiondate"] == DBNull.Value ? null : Convert.ToDateTime(dr["completiondate"]).ToString("dd/MM/yyyy");
                                _HomeLoanDTOVerification.poccupancycertificatedate = dr["occupancycertificatedate"] == DBNull.Value ? null : Convert.ToDateTime(dr["occupancycertificatedate"]).ToString("dd/MM/yyyy");
                                _HomeLoanDTOVerification.pactualcost = Convert.ToDecimal(dr["actualcost"]);
                                _HomeLoanDTOVerification.psaleagreementvalue = Convert.ToDecimal(dr["saleagreementvalue"]);
                                _HomeLoanDTOVerification.pstampdutycharges = Convert.ToDecimal(dr["stampdutycharges"]);
                                _HomeLoanDTOVerification.potheramenitiescharges = Convert.ToDecimal(dr["otheramenitiescharges"]);
                                _HomeLoanDTOVerification.potherincidentalexpenditure = Convert.ToDecimal(dr["otherincidentalexpenditure"]);
                                _HomeLoanDTOVerification.ptotalvalueofproperty = Convert.ToDecimal(dr["totalvalueofproperty"]);
                                _HomeLoanDTOVerification.pageofbuilding = dr["ageofbuilding"].ToString();
                                _HomeLoanDTOVerification.poriginalcostofproperty = Convert.ToDecimal(dr["originalcostofproperty"]);
                                _HomeLoanDTOVerification.pestimatedvalueofrepairs = Convert.ToDecimal(dr["estimatedvalueofrepairs"]);
                                _HomeLoanDTOVerification.pamountalreadyspent = Convert.ToDecimal(dr["amountalreadyspent"]);
                                _HomeLoanDTOVerification.potherborrowings = Convert.ToDecimal(dr["otherborrowings"]);
                                _HomeLoanDTOVerification.ptotalvalue = Convert.ToDecimal(dr["totalvalue"]);
                                _HomeLoanDTOVerification.SectionName = dr["verifiedsectionname"].ToString();
                                _HomeLoanDTOVerification.psubsectionname = dr["verifiedsubsectionname"].ToString();
                                _HomeLoanDTOVerification.IsVerified = dr["verificationstatus"].ToString();

                                _ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst.Add(_HomeLoanDTOVerification);
                            }
                        }
                    }
                    if (LoanType == "BUSINESS LOAN")
                    {
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO = new BusinessLoanDTOVerification();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO = new List<BusinessfinancialperformanceDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO = new List<BusinesscredittrendpurchasesDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO = new List<BusinesscredittrendsalesDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO = new List<BusinessstockpositionDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO = new List<BusinesscostofprojectDTOVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO = new BusinessancillaryunitaddressdetailsDTOVerification();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails = new List<BusinessloanassociateconcerndetailsVerification>();
                        _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss = new List<BusinessloanturnoverandprofitorlossVerification>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,descriptionoftheactivity, isfinancialperformanceapplicable, iscredittrendforpurchasesapplicable,iscredittrendforsalesapplicable, isstockpositionapplicable, iscostofprojectapplicable,isancillaryunit, associateconcernsexist,'Business Loan Applicable Sections' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid  where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.pdescriptionoftheactivity = dr["descriptionoftheactivity"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.pisfinancialperformanceapplicable = Convert.ToBoolean(dr["isfinancialperformanceapplicable"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.piscredittrendforpurchasesapplicable = Convert.ToBoolean(dr["iscredittrendforpurchasesapplicable"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.piscredittrendforsalesapplicable = Convert.ToBoolean(dr["iscredittrendforsalesapplicable"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.pisstockpositionapplicable = Convert.ToBoolean(dr["isstockpositionapplicable"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.piscostofprojectapplicable = Convert.ToBoolean(dr["iscostofprojectapplicable"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.pisancillaryunit = Convert.ToBoolean(dr["isancillaryunit"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.passociateconcernsexist = Convert.ToBoolean(dr["associateconcernsexist"].ToString());
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.SectionName = dr["verifiedsectionname"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.psubsectionname = dr["verifiedsubsectionname"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.IsVerified = dr["verificationstatus"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,'Financial Performance' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessfinancialperformance tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid  where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Add(new BusinessfinancialperformanceDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pturnoveramount = Convert.ToDecimal(dr["turnoveramount"]),
                                    pnetprofitamount = Convert.ToDecimal(dr["netprofitamount"]),
                                    pbalancesheetdocpath = dr["balancesheetdocpath"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,finacialyear, suppliername, address, contactno, maxcreditreceived,mincreditreceived, avgtotalcreditreceived,'Credit Trend of Purchases' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname  FROM tblapplicationbusinesscredittrendpurchases tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Add(new BusinesscredittrendpurchasesDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    psuppliername = dr["suppliername"].ToString(),
                                    paddress = dr["address"].ToString(),
                                    pcontactno = dr["contactno"].ToString(),
                                    pmaxcreditreceived = Convert.ToDecimal(dr["maxcreditreceived"]),
                                    pmincreditreceived = Convert.ToDecimal(dr["mincreditreceived"]),
                                    pavgtotalcreditreceived = Convert.ToDecimal(dr["avgtotalcreditreceived"]),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales,'Credit Trend of Sales' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname  FROM tblapplicationbusinesscredittrendsales tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Add(new BusinesscredittrendsalesDTOVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pcustomername = dr["customername"].ToString(),
                                    paddress = dr["address"].ToString(),
                                    pcontactno = dr["contactno"].ToString(),
                                    pmaxcreditgiven = Convert.ToDecimal(dr["maxcreditgiven"]),
                                    pmincreditgiven = Convert.ToDecimal(dr["mincreditgiven"]),
                                    ptotalcreditsales = Convert.ToDecimal(dr["totalcreditsales"]),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,finacialyear, maxstockcarried, minstockcarried, avgtotalstockcarried,'Stock Position' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessstockposition tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO.Add(new BusinessstockpositionDTOVerification
                                {

                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pmaxstockcarried = Convert.ToDecimal(dr["maxstockcarried"]),
                                    pminstockcarried = Convert.ToDecimal(dr["minstockcarried"]),
                                    pavgtotalstockcarried = Convert.ToDecimal(dr["avgtotalstockcarried"]),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()

                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,costoflandincludingdevelopment, buildingandothercivilworks, plantandmachinery,equipmenttools, testingequipment, miscfixedassets, erectionorinstallationcharges,preliminaryorpreoperativeexpenses, provisionforcontingencies,marginforworkingcapital,'Cost of Project' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinesscostofproject  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO.Add(new BusinesscostofprojectDTOVerification
                                {

                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pcostoflandincludingdevelopment = Convert.ToDecimal(dr["costoflandincludingdevelopment"]),
                                    pbuildingandothercivilworks = Convert.ToDecimal(dr["buildingandothercivilworks"]),
                                    pplantandmachinery = Convert.ToDecimal(dr["plantandmachinery"]),
                                    pequipmenttools = Convert.ToDecimal(dr["equipmenttools"]),
                                    ptestingequipment = Convert.ToDecimal(dr["testingequipment"]),
                                    pmiscfixedassets = Convert.ToDecimal(dr["miscfixedassets"]),
                                    perectionorinstallationcharges = Convert.ToDecimal(dr["erectionorinstallationcharges"]),
                                    ppreliminaryorpreoperativeexpenses = Convert.ToDecimal(dr["preliminaryorpreoperativeexpenses"]),
                                    pprovisionforcontingencies = Convert.ToDecimal(dr["provisionforcontingencies"]),
                                    pmarginforworkingcapital = Convert.ToDecimal(dr["marginforworkingcapital"]),
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype,recordid,address1, address2, city, country, state, district, pincode,coalesce(stateid,0) as stateid,coalesce(districtid,0) as districtid,coalesce(countryid,0) as countryid,'Ancillary Unit Information' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationbusinessloanancillaryunitaddressdetails tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress1 = dr["address1"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress2 = dr["address2"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcity = dr["city"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pCountry = dr["country"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid = Convert.ToInt64(dr["countryid"]);
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pState = dr["state"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid = Convert.ToInt64(dr["stateid"]);
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pDistrict = dr["district"].ToString();
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid = Convert.ToInt64(dr["districtid"]);
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pPincode = dr["pincode"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,nameofassociateconcern, natureofassociation, natureofactivity,itemstradedormanufactured,'Associate Concern Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationbusinessloanassociateconcerndetails tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), '');"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Add(new BusinessloanassociateconcerndetailsVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pnameofassociateconcern = dr["nameofassociateconcern"].ToString(),
                                    pnatureofassociation = dr["natureofassociation"].ToString(),
                                    pnatureofactivity = dr["natureofactivity"].ToString(),
                                    pitemstradedormanufactured = dr["itemstradedormanufactured"].ToString(),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct t1.*,t2.verifiedsectionname,verificationstatus from(SELECT vchapplicationid,'Applicant' as applicanttype, tl.recordid,turnoveryear, turnoveramount, turnoverprofit,'Turn Over and Profit/Loss Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationbusinessloanturnoverandprofitorloss tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid where tl.applicationid=" + applicationid + " and tl.statusid=" + Convert.ToInt32(Status.Active) + ")t1 left join (select * from tabapplicationFIverification  where upper(vchapplicationid) = '" + strapplictionid + "') t2 on t1.vchapplicationid = t2.vchapplicationid and coalesce(t1.applicanttype, '') = coalesce(t2.contacttype, '') and coalesce(t1.verifiedsectionname, '') = coalesce(t2.verifiedsectionname, '') and coalesce(trim(t1.verifiedsubsectionname), '') = coalesce(trim(t2.verifiedsubsectionname), ''); "))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Add(new BusinessloanturnoverandprofitorlossVerification
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pturnoveryear = dr["turnoveryear"].ToString(),
                                    pturnoveramount = Convert.ToDecimal(dr["turnoveramount"]),
                                    pturnoverprofit = Convert.ToDecimal(dr["turnoverprofit"]),
                                    pTypeofoperation = "OLD",
                                    SectionName = dr["verifiedsectionname"].ToString(),
                                    psubsectionname = dr["verifiedsubsectionname"].ToString(),
                                    IsVerified = dr["verificationstatus"].ToString()
                                });
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception)
                {

                    throw;
                }
  

            return _ApplicationLoanSpecificDTOinVerification;

        }
        #endregion

    }
}
