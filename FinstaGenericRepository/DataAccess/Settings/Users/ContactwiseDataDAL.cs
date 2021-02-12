using FinstaInfrastructure.Settings.Users;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings.Users;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Transactions;
using FinstaInfrastructure.Accounting;

namespace FinstaRepository.DataAccess.Settings.Users
{
    public class ContactwiseDataDAL : SettingsDAL, IContactwiseData
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        public ContactDataDTO ContactDataDTO { set; get; }
        public List<FirstinformationDTO> lstFirstinformationDTO { set; get; }

        public List<FirstinformationDTO> GetcoapplicantLoansBasedOnContactRefID(string contactrefID, string ConnectionString)
        {
            string Query = string.Empty;
            lstFirstinformationDTO = new List<FirstinformationDTO>();
            string applicationid = string.Empty;

            try
            {
                applicationid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(upper(vchapplicationid) ,''',''') as vchapplicationid from tabapplicationsurietypersonsdetails where (suritycontactreferenceid)='" + ManageQuote(contactrefID.Trim()) + "' and surityapplicanttype='Co-Applicant' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                if (!string.IsNullOrEmpty(applicationid))
                {
                    lstFirstinformationDTO = GetLoansData(applicationid, ConnectionString);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lstFirstinformationDTO;
        }

        public async Task<ContactDataDTO> GetContactData(string ContactRefID, string ConnectionString)
        {
            long Contactid = 0;
            ContactDataDTO = new ContactDataDTO();
            ContactDataDTO.ContactViewDTO = new ContactViewDTO();



            string addressdetails = string.Empty;
            string query = string.Empty;
            await Task.Run(() =>
            {
                try
                {

                    Contactid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select contactid from tblmstcontact where upper(contactreferenceid)='" + ManageQuote(ContactRefID.Trim().ToUpper()) + "';"));

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select to_char( datdate,'dd/MM/yyyy') datdate, contactid, contactreferenceid, contacttype, fullname, fathername, businessentitycontactno, businessentityemailid, contactimagepath, addresstype,contactaddress,loanscount,guratorcount,coapplicantcount,partystatus, referralstatus, employeestatus,advacatestatus from vwcontactdataview WHERE contactreferenceid='" + ManageQuote(ContactRefID.Trim()) + "';"))
                    {
                        if (dr.Read())
                        {
                            ContactViewDTO _ContactViewDTO = new ContactViewDTO
                            {
                                pContactDate = dr["datdate"],
                                pContactdId = dr["contactid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["contactid"]),
                                pContactType = dr["contacttype"],
                                pContactName = dr["fullname"],
                                pRefNo = dr["contactreferenceid"],
                                pFatherName = dr["fathername"],
                                pContactNumber = dr["businessentitycontactno"],
                                pContactEmail = dr["businessentityemailid"],
                                pImagePath = dr["contactimagepath"],
                                pAddresDetails = dr["contactaddress"],
                            };
                            ContactDataDTO _ContactDataDTO = new ContactDataDTO
                            {
                                pLoansCount = dr["loanscount"] == DBNull.Value ? 0 : Convert.ToInt64(dr["loanscount"]),
                                pGuratorCount = dr["guratorcount"] == DBNull.Value ? 0 : Convert.ToInt64(dr["guratorcount"]),
                                pCoApplicantCount = dr["coapplicantcount"] == DBNull.Value ? 0 : Convert.ToInt64(dr["coapplicantcount"]),
                                pPartyStatus = dr["partystatus"],
                                pReferralStatus = dr["referralstatus"],
                                pEmployeeStatus = dr["employeestatus"],
                                pAdvacateStatus = dr["advacatestatus"],
                            };
                            ContactDataDTO = _ContactDataDTO;
                            ContactDataDTO.ContactViewDTO = _ContactViewDTO;
                        }
                    }

                    ContactDataDTO.lstContactBankDetaisDTO = new List<ContactBankDetaisDTO>();
                    ContactDataDTO.lstKycDocDTO = new List<KycDocDTO>();
                    ContactDataDTO.contactpersonaldetailslist = new List<ContactPersonalDetailsDTO>();
                    ContactDataDTO.contactnomineedetailslist = new List<ContactNomineeDetailsDTO>();

                    ContactDataDTO.applicantdetailslist = new List<FirstinformationDTO>();
                    //ContactDataDTO.gurantordetailslist = new List<FirstinformationDTO>();
                    //ContactDataDTO.coapplicantdetailslist = new List<FirstinformationDTO>();
                    //ContactDataDTO.partydetailslist = new List<AccountReportsDTO>();
                    //ContactDataDTO.referraldetails = new RefferalDetailsDTO();
                    //ContactDataDTO.referralloansdetailslist = new List<FirstinformationDTO>();
                    //ContactDataDTO.advacatedetailslist = new List<FirstinformationDTO>();
                    //ContactDataDTO.employeedetailslist = new EmployeeDetailsDTO();
                    ContactDataDTO.cicdetailslist = new List<CicScoreDetailsDTO>();

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(recordid,0)as recordid,coalesce(applicantype,'')as applicantype,coalesce(applicationid,0)as applicationid, coalesce(vchapplicationid,'')as vchapplicationid,coalesce(contactid,0)as contactid,coalesce(contactreferenceid,'')as contactreferenceid,coalesce(bankname,'')as bankname,coalesce(accountno,'')as accountno,coalesce(ifsccode,'')as ifsccode,coalesce(branch,'') as branch,coalesce(isprimarybank,false) as isprimarybank from tabapplicationpersonalbankdetails where  isprimarybank =true and (contactreferenceid)='" + ManageQuote(ContactRefID.ToUpper()) + "'  and statusid=" + Convert.ToInt32(Status.Active) + "  order by createddate desc limit 1;"))
                    {
                        while (dr.Read())
                        {
                            ContactDataDTO.lstContactBankDetaisDTO.Add(new ContactBankDetaisDTO
                            {
                                pRecordid = dr["recordid"] == DBNull.Value ? 0 : Convert.ToInt64(dr["recordid"]),
                                pBankName = Convert.ToString(dr["bankname"]),
                                pBankAccountNo = Convert.ToString(dr["accountno"]),
                                pBankifscCode = Convert.ToString(dr["ifsccode"]),
                                pBankBranch = Convert.ToString(dr["branch"]),
                                pIsprimaryAccount = dr["isprimarybank"] == DBNull.Value ? false : Convert.ToBoolean(dr["isprimarybank"]),
                            });

                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select initcap(documentname)documentname,docreferenceno,docstorepath,docfiletype,docisdownloadable FROM (SELECT distinct upper( documentname)documentname,docreferenceno,docstorepath,docfiletype,docisdownloadable FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and t.contactid=" + Contactid + ")x;"))
                    {
                        while (dr.Read())
                        {
                            ContactDataDTO.lstKycDocDTO.Add(new KycDocDTO
                            {
                                pDocumentName = Convert.ToString(dr["documentname"]),
                                pDocStorePath = Convert.ToString(dr["docstorepath"]),
                                pDocFileType = Convert.ToString(dr["docfiletype"]),
                                pDocReferenceno = Convert.ToString(dr["docreferenceno"]),
                                pDocIsDownloadable = dr["docisdownloadable"] == DBNull.Value ? false : Convert.ToBoolean(dr["docisdownloadable"]),
                            });
                        }
                    }

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from (select row_number() over(partition by ciccompany order by ciccompany,coalesce(modifieddate,createddate) desc) as recordid, ciccompany ,creditscore  ,maxcreditscore,coalesce(modifieddate,createddate) createddate  from tabapplicationancilbildetails where contactid  =" + Contactid + "  and statusid=" + Convert.ToInt32(Status.Active) + " )x where recordid =1 order by ciccompany"))
                    {
                        while (dr.Read())
                        {

                            ContactDataDTO.cicdetailslist.Add(new CicScoreDetailsDTO
                            {
                                pciccompanyName = dr["ciccompany"],
                                //pcicscore = dr["creditscore"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["creditscore"]),
                                //pcicmaxscore = dr["maxcreditscore"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["maxcreditscore"]),
                                pcicscorepercentage = dr["maxcreditscore"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["maxcreditscore"]),

                            });

                        }
                    }

                    if (Convert.ToString(ContactDataDTO.ContactViewDTO.pContactType) == "Individual")
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from( select  residentialstatus , maritalstatus , placeofbirth , countryofbirth , nationality , minoritycommunity, createddate  from tabapplicationpersonalbirthdetails where (coalesce(residentialstatus,'')!='' or coalesce(maritalstatus,'')!='' or coalesce(placeofbirth,'')!='' or coalesce(countryofbirth,'')!='' or coalesce(nationality,'')!='' or coalesce(minoritycommunity,'')!='')   and contactid  =" + Contactid + "  and statusid=" + Convert.ToInt32(Status.Active) + ")x order by createddate desc limit 1 ; "))
                        {
                            while (dr.Read())
                            {
                                ContactDataDTO.contactpersonaldetailslist.Add(new ContactPersonalDetailsDTO
                                {
                                    presidentialstatus = dr["residentialstatus"],
                                    pmaritalstatus = dr["maritalstatus"],
                                    pplaceofbirth = dr["placeofbirth"],
                                    pcountryofbirth = dr["countryofbirth"],
                                    pnationality = dr["nationality"],
                                    pminoritycommunity = dr["minoritycommunity"],
                                });
                            }
                        }

                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from(SELECT nomineename ,relationship  ,to_char(dateofbirth,'dd/MM/yyyy') dateofbirth,contactno,createddate  FROM tabapplicationpersonalnomineedetails  where isprimarynominee =true  and statusid=" + Convert.ToInt32(Status.Active) + " and contactid  = " + Contactid + " and (coalesce(nomineename,'')!='' or coalesce(relationship,'')!=''  or dateofbirth != null::date  or coalesce(contactno,'')!=''))x order by createddate desc limit 1  "))
                        {
                            while (dr.Read())
                            {
                                ContactDataDTO.contactnomineedetailslist.Add(new ContactNomineeDetailsDTO
                                {
                                    pnomineename = dr["nomineename"],
                                    prelationship = dr["relationship"],
                                    pdateofbirth = dr["dateofbirth"],
                                    pcontactno = dr["contactno"],
                                });
                            }
                        }
                        string incomedata = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select vchapplicationid||'@'||coalesce(netannualincome,0) from tabapplicationpersonalincomedetails  where   contactid  = " + Contactid + " and coalesce(netannualincome,0)>0  and statusid=" + Convert.ToInt32(Status.Active) + " order by createddate desc limit 1 "));
                        if (!string.IsNullOrEmpty(incomedata))
                        {
                            string applicationid = Convert.ToString(incomedata.Split('@')[0]).Trim();

                            ContactDataDTO.ContactViewDTO.pnetannualincome = Convert.ToDecimal(incomedata.Split('@')[1]);
                            if (!string.IsNullOrEmpty(applicationid))
                            {
                                ContactDataDTO.ContactViewDTO.potherincome = Convert.ToDecimal(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select coalesce(sum(coalesce(grossannual,0)),0) from tabapplicationpersonalotherincomedetails  where  vchapplicationid ='" + applicationid + "'  and statusid=" + Convert.ToInt32(Status.Active) + "; ")); ;
                            }
                        }

                    }
                    else
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from(SELECT  businessactivity ,to_char(establishmentdate,'dd/MM/yyyy') establishmentdate ,to_char(commencementdate,'dd/MM/yyyy')  commencementdate ,cinnumber ,gstinno,createddate  from tabapplicationpersonalbusinessdetails  where ( coalesce(businessactivity,'')!='' or (establishmentdate)!= null::date or coalesce(commencementdate)!= null::date or coalesce(cinnumber,'')!='' or coalesce(gstinno,'')!='' ) and contactid  =" + Contactid + "  and statusid=" + Convert.ToInt32(Status.Active) + ")x order by createddate desc limit 1 ; "))
                        {
                            while (dr.Read())
                            {
                                ContactDataDTO.contactpersonaldetailslist.Add(new ContactPersonalDetailsDTO
                                {
                                    pbusinessactivity = dr["businessactivity"],
                                    pestablishmentdate = dr["establishmentdate"],
                                    pcommencementdate = dr["commencementdate"],
                                    pcinnumber = dr["cinnumber"],
                                    pgstinno = dr["gstinno"],
                                });

                            }
                        }
                    }

                    ContactDataDTO.applicantdetailslist = GetLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    //ContactDataDTO.gurantordetailslist = GetGurantorLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    //ContactDataDTO.coapplicantdetailslist = GetcoapplicantLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    //ContactDataDTO.referralloansdetailslist = GetRefferalLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    //ContactDataDTO.referraldetails = GetRefferalDetails(ContactRefID, ConnectionString, Contactid);
                    //ContactDataDTO.employeedetailslist = GetEmployeeDetails(ConnectionString, Contactid);
                    //ContactDataDTO.partydetailslist = GetPartyLedgerDetails(ConnectionString, Contactid);
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return ContactDataDTO;
        }

        public async Task<ContactDataDTO> GetContactDataDetails(string loaddataType, string ContactRefID, string ConnectionString)
        {
            long Contactid = 0;
            ContactDataDTO = new ContactDataDTO();
            ContactDataDTO.ContactViewDTO = new ContactViewDTO();



            string addressdetails = string.Empty;
            string query = string.Empty;
            await Task.Run(() =>
            {
                try
                {

                    Contactid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select contactid from tblmstcontact where upper(contactreferenceid)='" + ManageQuote(ContactRefID.Trim().ToUpper()) + "';"));
                    if (loaddataType == "APPLICANT")
                    {
                        ContactDataDTO.applicantdetailslist = new List<FirstinformationDTO>();
                        ContactDataDTO.applicantdetailslist = GetLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    }
                    else if (loaddataType == "GUARANTOR")
                    {
                        ContactDataDTO.gurantordetailslist = new List<FirstinformationDTO>();
                        ContactDataDTO.gurantordetailslist = GetGurantorLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    }
                    else if (loaddataType == "COAPPLICANT")
                    {
                        ContactDataDTO.coapplicantdetailslist = new List<FirstinformationDTO>();
                        ContactDataDTO.coapplicantdetailslist = GetcoapplicantLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                    }
                    else if (loaddataType == "PARTY")
                    {
                        ContactDataDTO.partydetailslist = new List<AccountReportsDTO>();
                        ContactDataDTO.partydetailslist = GetPartyLedgerDetails(ConnectionString, Contactid);
                    }
                    else if (loaddataType == "REFERRAL")
                    {
                        ContactDataDTO.referraldetails = new RefferalDetailsDTO();
                        ContactDataDTO.referralloansdetailslist = new List<FirstinformationDTO>();

                        ContactDataDTO.referralloansdetailslist = GetRefferalLoansBasedOnContactRefID(ContactRefID, ConnectionString);
                        ContactDataDTO.referraldetails = GetRefferalDetails(ContactRefID, ConnectionString, Contactid);
                    }
                    else if (loaddataType == "EMPLOYEE")
                    {
                        ContactDataDTO.employeedetailslist = new EmployeeDetailsDTO();
                        ContactDataDTO.employeedetailslist = GetEmployeeDetails(ConnectionString, Contactid);
                    }
                    else
                    {
                        ContactDataDTO.advacatedetailslist = new List<FirstinformationDTO>();
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            });
            return ContactDataDTO;
        }

        public List<FirstinformationDTO> GetGurantorLoansBasedOnContactRefID(string contactrefID, string ConnectionString)
        {
            string Query = string.Empty;
            lstFirstinformationDTO = new List<FirstinformationDTO>();
            string applicationid = string.Empty;

            try
            {
                applicationid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(upper(vchapplicationid) ,''',''') as vchapplicationid from tabapplicationsurietypersonsdetails where upper(suritycontactreferenceid)='" + ManageQuote(contactrefID.Trim().ToUpper()) + "' and surityapplicanttype='Guarantor';"));

                if (!string.IsNullOrEmpty(applicationid))
                {
                    lstFirstinformationDTO = GetLoansData(applicationid, ConnectionString);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lstFirstinformationDTO;
        }

        public List<FirstinformationDTO> GetLoansBasedOnContactRefID(string contactrefID, string ConnectionString)
        {
            string Query = string.Empty;
            lstFirstinformationDTO = new List<FirstinformationDTO>();
            string applicationid = string.Empty;

            try
            {

                applicationid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(upper(vchapplicationid) ,''',''') as vchapplicationid from tabapplication where upper(contactreferenceid)='" + ManageQuote(contactrefID.Trim().ToUpper()) + "';"));

                if (!string.IsNullOrEmpty(applicationid))
                {
                    lstFirstinformationDTO = GetLoansData(applicationid, ConnectionString);
                }


            }
            catch (Exception)
            {

                throw;
            }

            return lstFirstinformationDTO;


        }


        public List<FirstinformationDTO> GetRefferalLoansBasedOnContactRefID(string contactrefID, string ConnectionString)
        {
            string Query = string.Empty;
            lstFirstinformationDTO = new List<FirstinformationDTO>();
            string applicationid = string.Empty;

            try
            {

                applicationid = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select string_agg(upper(vchapplicationid) ,''',''') as vchapplicationid from tabapplication where upper(referralcontactrefid)='" + ManageQuote(contactrefID.Trim().ToUpper()) + "';"));

                if (!string.IsNullOrEmpty(applicationid))
                {
                    lstFirstinformationDTO = GetLoansData(applicationid, ConnectionString);
                }


            }
            catch (Exception)
            {

                throw;
            }

            return lstFirstinformationDTO;
        }

        public RefferalDetailsDTO GetRefferalDetails(string contactrefID, string ConnectionString, long contactid)
        {
            string Query = string.Empty;
            RefferalDetailsDTO _RefferalDetailsDTO = new RefferalDetailsDTO();
            string applicationid = string.Empty;

            try
            {
                _RefferalDetailsDTO.pStatusName = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT statusname from tblmstreferral  t1 join tblmststatus t2 on t1.statusid=t2.statusid where contactid=" + contactid));

                _RefferalDetailsDTO.ploansamount = Convert.ToDecimal(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select coalesce( sum(coalesce(approvedloanamount,0)),0) from tbltransapprovedapplications  where    vchapplicationid in (select vchapplicationid from tabapplication  where referralcontactrefid  ='" + (contactrefID.Trim().ToUpper()) + "')"));
                _RefferalDetailsDTO.pcommissionpaidamount = 0;
                _RefferalDetailsDTO.pcommissindueamount = 0;
                _RefferalDetailsDTO.pfdamount = 0;
                _RefferalDetailsDTO.prdamount = 0;
                _RefferalDetailsDTO.psdamount = 0;
                _RefferalDetailsDTO.pbusinessamount = _RefferalDetailsDTO.ploansamount + _RefferalDetailsDTO.pfdamount + _RefferalDetailsDTO.prdamount + _RefferalDetailsDTO.psdamount;


            }
            catch (Exception)
            {

                throw;
            }

            return _RefferalDetailsDTO;
        }

        public EmployeeDetailsDTO GetEmployeeDetails(string ConnectionString, long contactid)
        {
            string Query = string.Empty;
            EmployeeDetailsDTO _EmployeeDetailsDTO = new EmployeeDetailsDTO();
            string applicationid = string.Empty;
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select contactid,rolename  ,designation ,to_char(dateofjoining,'dd/MM/yyyy') dateofjoining , basicsalary ,  allowanceorvariablepay ,totalcosttocompany,statusname from tblmstemployeeemploymentdetails t1 join tblmstemployee t2 on t1.employeeid =t2.employeeid join tblmststatus t3 on t2.statusid=t3.statusid  where t2.statusid=1 and contactid=" + contactid + "; "))
                {
                    if (dr.Read())
                    {
                        _EmployeeDetailsDTO = new EmployeeDetailsDTO
                        {
                            prolename = dr["rolename"],
                            pStatusName = dr["statusname"],
                            pdesignation = dr["designation"],
                            pdateofjoining = dr["dateofjoining"],
                            pbasicsalary = Convert.ToDecimal(dr["basicsalary"]),
                            pallowanceorvariablepay = dr["allowanceorvariablepay"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["allowanceorvariablepay"]),
                            ptotalcosttocompany = dr["totalcosttocompany"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["totalcosttocompany"]),
                        };

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return _EmployeeDetailsDTO;
        }

        public List<AccountReportsDTO> GetPartyLedgerDetails(string ConnectionString, long contactid)
        {
            string Query = string.Empty;
            string pQuery = string.Empty;

            List<AccountReportsDTO> lstcashbook = new List<AccountReportsDTO>();
            try
            {

                Query = "select recordid,to_char( transactiondate,'dd/MM/yyyy') transactiondate,TRANSACTIONNO,PARTICULARS,DESCRIPTION,contactname,DEBITAMOUNT,abs(CREDITAMOUNT)as CREDITAMOUNT,abs(balance) as balance,case when balance>0 then 'Dr' else 'Cr' end as balancetype from (select *,sum(DEBITAMOUNT+CREDITAMOUNT) OVER(ORDER BY TRANSACTIONDATE,RECORDID)as BALANCE from(SELECT row_number() over (order by transactiondate,TRANSACTIONNO) as recordid, transactiondate,TRANSACTIONNO,parentaccountname PARTICULARS,sum(COALESCE(DEBITAMOUNT,0.00)) as  DEBITAMOUNT,-sum(COALESCE(CREDITAMOUNT,0.00)) as CREDITAMOUNT,DESCRIPTION,contactname FROM tbltranstotaltransactions WHERE  transactiondate <= current_date  and contactid=" + contactid + " group by  transactiondate,TRANSACTIONNO,parentaccountname,DESCRIPTION,contactname ) as D  )x where ( debitamount<>0 or creditamount<>0); ";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {

                        lstcashbook.Add(new AccountReportsDTO
                        {
                            precordid = Convert.ToInt64(dr["RECORDID"]),
                            ptransactiondate = Convert.ToString(dr["transactiondate"]),
                            pdebitamount = Convert.ToDouble(dr["DEBITAMOUNT"]),
                            pcreditamount = Convert.ToDouble(dr["CREDITAMOUNT"]),
                            pparticulars = Convert.ToString(dr["PARTICULARS"]),
                            pdescription = Convert.ToString(dr["DESCRIPTION"]),
                            ptransactionno = Convert.ToString(dr["TRANSACTIONNO"]),
                            popeningbal = Convert.ToDouble(dr["BALANCE"]),
                            pBalanceType = Convert.ToString(dr["balancetype"]),
                        });


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstcashbook;
        }
        public List<FirstinformationDTO> GetLoansData(string contactrefID, string ConnectionString)
        {
            string Query = string.Empty;
            lstFirstinformationDTO = new List<FirstinformationDTO>();

            try
            {
                Query = "select applicationid, vchapplicationid,to_char(dateofapplication,'dd/MM/yyyy')  dateofapplication, applicantid, contactreferenceid, applicantname, contacttype, loantypeid, loantype, loanid, loanname, applicanttype, amountrequested, purposeofloan, loanpayin, interesttype, rateofinterest, tenureofloan, loaninstalmentpaymentmode, instalmentamount, vchapplicantstatus,to_char(approvaldate,'dd/MM/yyyy') approvaldate,to_char(firstdisbursementdate,'dd/MM/yyyy')  firstdisbursementdate,to_char(lastdisbursementdate,'dd/MM/yyyy') lastdisbursementdate, totaldisburseamount,to_char(loanstartddate,'dd/MM/yyyy') loanstartddate,to_char(nextinstalmentdate,'dd/MM/yyyy') nextinstalmentdate,to_char(loancloseddate,'dd/MM/yyyy') loancloseddate, instalmentprinciple, instalmentinterest, actualpenalty, coalesce(paidprinciple,0)paidprinciple,coalesce( paidinterest,0)paidinterest,coalesce( paidpenalty,0)paidpenalty, coalesce(waiveofpenalty,0)waiveofpenalty, coalesce(principledue,0)principledue,coalesce( interestdue,0)interestdue,coalesce( penaltydue,0)penaltydue,coalesce( futureprincipledue,0)futureprincipledue,coalesce( futureinterestdue,0) futureinterestdue, statusid, statusname,statementstatus from vwcontact_loans where upper(vchapplicationid) in('" + contactrefID + "');";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {

                        lstFirstinformationDTO.Add(new FirstinformationDTO
                        {
                            papplicationid = Convert.ToInt64(dr["applicationid"]),
                            pVchapplicationid = Convert.ToString(dr["vchapplicationid"]),
                            pDateofapplication = Convert.ToString(dr["Dateofapplication"]),
                            pApplicantid = Convert.ToInt64(dr["applicantid"]),
                            pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pApplicantname = Convert.ToString(dr["applicantname"]),
                            pContacttype = Convert.ToString(dr["contacttype"]),
                            pLoantypeid = Convert.ToInt64(dr["loantypeid"]),
                            pLoantype = Convert.ToString(dr["loantype"]),
                            pLoanid = Convert.ToInt64(dr["loanid"]),
                            pLoanname = Convert.ToString(dr["loanname"]),
                            pApplicanttype = Convert.ToString(dr["applicanttype"]),
                            pAmountrequested = Convert.ToDecimal(dr["amountrequested"]),
                            pPurposeofloan = Convert.ToString(dr["purposeofloan"]),
                            pLoanpayin = Convert.ToString(dr["loanpayin"]),
                            pInteresttype = Convert.ToString(dr["interesttype"]),
                            pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]),
                            pTenureofloan = Convert.ToInt64(dr["tenureofloan"]),
                            pLoaninstalmentpaymentmode = Convert.ToString(dr["loaninstalmentpaymentmode"]),
                            pInstalmentamount = Convert.ToDecimal(dr["instalmentamount"]),
                            pVchapplicantstatus = Convert.ToString(dr["vchapplicantstatus"]),
                            pstatementstatus = Convert.ToString(dr["statementstatus"]),
                            papprovaldate = Convert.ToString(dr["approvaldate"]),
                            pfirstdisbursementdate = Convert.ToString(dr["firstdisbursementdate"]),
                            plastdisbursementdate = Convert.ToString(dr["lastdisbursementdate"]),
                            ptotaldisburseamount = dr["totaldisburseamount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["totaldisburseamount"]),
                            ploanstartddate = Convert.ToString(dr["loanstartddate"]),
                            pNextEmiDate = Convert.ToString(dr["nextinstalmentdate"]),
                            ploancloseddate = Convert.ToString(dr["loancloseddate"]),
                            pstatusname = Convert.ToString(dr["statusname"]),
                            pinstalmentprinciple = dr["instalmentprinciple"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["instalmentprinciple"]),
                            pinstalmentinterest = dr["instalmentinterest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["instalmentinterest"]),
                            pactualpenalty = dr["actualpenalty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["actualpenalty"]),
                            ppaidprinciple = dr["paidprinciple"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["paidprinciple"]),
                            ppaidinterest = dr["paidinterest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["paidinterest"]),
                            ppaidpenalty = dr["paidpenalty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["paidpenalty"]),
                            pwaiveofpenalty = dr["waiveofpenalty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["waiveofpenalty"]),
                            pprincipledue = dr["principledue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["principledue"]),
                            pinterestdue = dr["interestdue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["interestdue"]),
                            ppenaltydue = dr["penaltydue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["penaltydue"]),
                            pfutureprincipledue = dr["futureprincipledue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["futureprincipledue"]),
                            pfutureinterestdue = dr["futureinterestdue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["futureinterestdue"]),
                            ptotalpaidamount = Convert.ToDecimal(dr["paidprinciple"]) + Convert.ToDecimal(dr["paidinterest"]),
                            ptotaldueamount = (Convert.ToDecimal(dr["principledue"])) + (Convert.ToDecimal(dr["interestdue"])),
                        });

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lstFirstinformationDTO;
        }
    }
}
