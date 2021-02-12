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

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public class Verification : SettingsDAL, IVerification
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        List<VerificationDetailsDTO> lstVerificationDetailsDTO { set; get; }

        public ApplicationLoanSpecificDTOinVerification _ApplicationLoanSpecificDTOinVerification { get; set; }
        public async Task<List<VerificationDetailsDTO>> GetAllApplicantVerificationDetails(string ConnectonString)
        {
            ds = new DataSet();
            lstVerificationDetailsDTO = new List<VerificationDetailsDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "select z.*,t1.loantype,t1.loanname,t1.dateofapplication,t1.televerificationdate,t1.addressverificationdate,t1.documentverificationdate,t1.amountrequested,t1.instalmentamount from(select x.applicationid, x.vchapplicationid, x.applicantid, x.contactreferenceid,coalesce(y.name,'')||' '||coalesce(y.surname,'') as applicantname,y.businessentitycontactno,x.isteleverification,x.isphysicalverification,x.isdocumentverified from(select applicationid, vchapplicationid, applicantid, contactreferenceid,false as isteleverification,false as isphysicalverification,false as isdocumentverified from tabapplication where vchapplicationid not in (select vchapplicationid from tabapplicationverificationdetails)union select applicationid, vchapplicationid, contactid as applicantid,contactreferenceid,CASE WHEN televerifiersrating is null  THEN false WHEN televerifiersrating=''  THEN false ELSE true end as isteleverification,CASE WHEN fieldverificationstatus is null  THEN false WHEN fieldverificationstatus=''  THEN false ELSE true END as isphysicalverification, CASE WHEN fiverificationstatus is null  THEN false WHEN fiverificationstatus=''  THEN false ELSE true END as isdocumentverified from tabapplicationverificationdetails) x left join tblmstcontact y on x.applicantid=y.contactid order by x.applicationid)z join tabapplication t1 on z.applicationid=t1.applicationid where t1.loanstatusid in(5,10,7,8,9) order by applicationid desc; "))
                    {
                        while (dr.Read())
                        {
                            VerificationDetailsDTO VerificationDetailsDTO = new VerificationDetailsDTO();
                            VerificationDetailsDTO.papplicationid = Convert.ToInt64(dr["applicationid"]);
                            VerificationDetailsDTO.pvchapplicationid = Convert.ToString(dr["vchapplicationid"]);
                            VerificationDetailsDTO.pcontactid = Convert.ToInt64(dr["applicantid"]);
                            VerificationDetailsDTO.pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                            VerificationDetailsDTO.PApplicantName = Convert.ToString(dr["applicantname"]);
                            VerificationDetailsDTO.pbusinessentitycontactno = Convert.ToInt64(dr["businessentitycontactno"]);
                            VerificationDetailsDTO.pisteleverification = Convert.ToBoolean(dr["isteleverification"].ToString());
                            VerificationDetailsDTO.pisphysicalverification = Convert.ToBoolean(dr["isphysicalverification"].ToString());
                            VerificationDetailsDTO.pisdocumentverified = Convert.ToBoolean(dr["isdocumentverified"].ToString());
                            VerificationDetailsDTO.pLoantype = Convert.ToString(dr["loantype"]);
                            VerificationDetailsDTO.pLoanName = Convert.ToString(dr["loanname"]);

                            VerificationDetailsDTO.pdateofapplication = dr["dateofapplication"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofapplication"]).ToString("dd/MM/yyyy");
                            VerificationDetailsDTO.pteleverificationdate = dr["televerificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["televerificationdate"]).ToString("dd/MM/yyyy");
                            VerificationDetailsDTO.paddressverificationdate = dr["addressverificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["addressverificationdate"]).ToString("dd/MM/yyyy");
                            VerificationDetailsDTO.pdocumentverificationdate = dr["documentverificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["documentverificationdate"]).ToString("dd/MM/yyyy");
                            VerificationDetailsDTO.pLoanAmount = Convert.ToDecimal(dr["amountrequested"]);
                            VerificationDetailsDTO.pinstalmentAmount = Convert.ToDecimal(dr["instalmentamount"]);
                            lstVerificationDetailsDTO.Add(VerificationDetailsDTO);
                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            });
            return lstVerificationDetailsDTO;
        }

        public async Task<FieldverificationDTO> GetFieldVerificationDetails(string strapplicatonid, string ConnectonString)
        {
            strapplicatonid = strapplicatonid.ToUpper().Trim();
            FieldverificationDTO FieldverificationDTO = new FieldverificationDTO();
            FieldverificationDTO.AddressconfirmedDTO = new AddressconfirmedDTO();
            FieldverificationDTO.lstaddrestype = new List<AddressType>();
            FieldverificationDTO.lstProffType = new List<ProffType>();

            FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO = new List<FielddocumentverificationDTO>();
            FieldverificationDTO.FieldVerifiersobservationDTO = new FieldVerifiersobservationDTO();
            FieldverificationDTO.FieldVerifyneighbourcheckDTO = new List<FieldVerifyneighbourcheckDTO>();
            ds = new DataSet();
            DataSet ds1 = new DataSet();
            await Task.Run(() =>
            {
                try
                {
                    ds1 = NPGSqlHelper.ExecuteDataset(ConnectonString, CommandType.Text, "select x.applicationid,x.vchapplicationid,x.applicantid,x.contactreferenceid,x.applicantname,x.contacttype,x.loantype,x.loanname,x.dateofapplication,coalesce(x.disbursementdate,current_date)as disbursementdate,y.businessentitycontactno from tabapplication x join tblmstcontact y on x.applicantid=y.contactid  where upper(x.vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and x.statusid=" + Convert.ToInt32(Status.Active) + ";");
                    if (ds1 != null)
                    {
                        FieldverificationDTO.papplicationid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicationid"]);
                        FieldverificationDTO.pvchapplicationid = Convert.ToString(ds1.Tables[0].Rows[0]["vchapplicationid"]);
                        FieldverificationDTO.pcontactid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicantid"]);
                        FieldverificationDTO.pcontactreferenceid = Convert.ToString(ds1.Tables[0].Rows[0]["contactreferenceid"]);
                        FieldverificationDTO.pApplicantName = Convert.ToString(ds1.Tables[0].Rows[0]["applicantname"]);
                        FieldverificationDTO.Pcontacttype = Convert.ToString(ds1.Tables[0].Rows[0]["contacttype"]);
                        FieldverificationDTO.pDateofApplication = ds1.Tables[0].Rows[0]["dateofapplication"] == DBNull.Value ? null : Convert.ToDateTime(ds1.Tables[0].Rows[0]["dateofapplication"]).ToString("dd/MM/yyyy");
                        FieldverificationDTO.pDateofDisbursement = ds1.Tables[0].Rows[0]["disbursementdate"] == DBNull.Value ? null : Convert.ToDateTime(ds1.Tables[0].Rows[0]["disbursementdate"]).ToString("dd/MM/yyyy");
                        FieldverificationDTO.pLoantype = Convert.ToString(ds1.Tables[0].Rows[0]["loantype"]);
                        FieldverificationDTO.pLoanName = Convert.ToString(ds1.Tables[0].Rows[0]["loanname"]);
                        FieldverificationDTO.Pconctano = Convert.ToInt64(ds1.Tables[0].Rows[0]["businessentitycontactno"]);
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "select ADDRESSTYPE from TBLMSTADDRESSTYPES where upper(contacttype)='" + FieldverificationDTO.Pcontacttype.ToUpper() + "' order by ADDRESSTYPE;"))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.lstaddrestype.Add(new AddressType
                            {

                                pAddresType = Convert.ToString(dr["ADDRESSTYPE"])

                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,'KYC DocumentsDTO Details' as sectionname,coalesce(t.contacttype, '')||'-'||coalesce(t1.name,'')||' '||coalesce(t1.surname,'') as subscetion FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + FieldverificationDTO.papplicationid + " and t.contacttype='Applicant';"))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.lstProffType.Add(new ProffType
                            {
                                pdocumentgroupname = Convert.ToString(dr["documentgroupname"]),
                                pProfname = Convert.ToString(dr["documentname"])
                            });
                        }
                    }
                    // ds = NPGSqlHelper.ExecuteDataset(ConnectonString, CommandType.Text, "SELECT recordid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, addresstype, isaddressconfirmed,uploadlocationdocpath, longitude, latitude, noofyearsatpresentaddress, houseownership, personmet, personname, relationshipwithapplicant,dateofbirth, maritalstatus, totalnoofmembersinfamily, earningmembers,children, employmentorbusinessdetails, monthlyincome, fieldverifierscomments,fieldverifiersrating FROM tabapplicationfieldverification where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";SELECT recordid,docprooftype, docproofname, isdocumentverified FROM tabapplicationfielddocumentverification where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";SELECT recordid,addresslocalitydescription, accessability, typeofaccomodation,approxarea, visiblepoliticalaffiliation, affiliationremarks, addressfurnishing, visibleassets, customercooperation, customeravailability,fieldverifierscomments, fieldverifiersrating FROM tabapplicationfieldverifiersobservation where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; SELECT recordid, nameoftheneighbour, addressofneighbour, isapplicantstayhere, houseownership, applicantisstayingsince, fieldverifierscomments, fieldverifiersrating FROM tabapplicationfieldverifyneighbourcheck where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; ");
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT recordid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, addresstype, isaddressconfirmed,uploadlocationdocpath, longitude, latitude, noofyearsatpresentaddress, houseownership, personmet, personname, relationshipwithapplicant,dateofbirth, maritalstatus, totalnoofmembersinfamily, earningmembers,children, employmentorbusinessdetails, monthlyincome, fieldverifierscomments,fieldverifiersrating,coalesce(t1.createdby,0) as createdby,coalesce(t2.username,'') as username FROM tabapplicationfieldverification t1 join tblmstusers t2 on t1.createdby=t2.userid where upper(t1.vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and t1.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            FieldverificationDTO.pverificationdate = dr["verificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["verificationdate"]).ToString("dd/MM/yyyy");
                            FieldverificationDTO.pverificationtime = Convert.ToString(dr["verificationtime"]);
                            FieldverificationDTO.pInvestigationexecutiveid = Convert.ToInt64(dr["investigationexecutiveid"]);
                            FieldverificationDTO.pInvestigationexecutivename = Convert.ToString(dr["investigationexecutivename"]);
                            FieldverificationDTO.pVerifierName = Convert.ToString(dr["username"]);
                            FieldverificationDTO.paddresstype = Convert.ToString(dr["addresstype"]);
                            FieldverificationDTO.AddressconfirmedDTO.pisaddressconfirmed = Convert.ToBoolean(dr["isaddressconfirmed"].ToString());
                            FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath = Convert.ToString(dr["uploadlocationdocpath"]);
                            FieldverificationDTO.AddressconfirmedDTO.plongitude = Convert.ToString(dr["longitude"]);
                            FieldverificationDTO.AddressconfirmedDTO.pLatitude = Convert.ToString(dr["latitude"]);
                            FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress = Convert.ToInt32(dr["noofyearsatpresentaddress"]);
                            FieldverificationDTO.AddressconfirmedDTO.pHouseownership = Convert.ToString(dr["houseownership"]);
                            FieldverificationDTO.AddressconfirmedDTO.pPersonmet = Convert.ToString(dr["personmet"]);
                            FieldverificationDTO.AddressconfirmedDTO.pPersonname = Convert.ToString(dr["personname"]);
                            FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant = Convert.ToString(dr["relationshipwithapplicant"]);
                            if (Convert.ToString(dr["dateofbirth"]) != string.Empty)
                            {
                                FieldverificationDTO.AddressconfirmedDTO.pDateofbirth = Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                FieldverificationDTO.AddressconfirmedDTO.pDateofbirth = "";
                            }
                            if (Convert.ToString(dr["dateofbirth"]) != string.Empty)
                            {
                                FieldverificationDTO.AddressconfirmedDTO.pAge = CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"]));
                            }
                            FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus = Convert.ToString(dr["maritalstatus"]);
                            FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily = Convert.ToInt32(dr["totalnoofmembersinfamily"]);
                            FieldverificationDTO.AddressconfirmedDTO.pEarningmembers = Convert.ToInt32(dr["earningmembers"]);
                            FieldverificationDTO.AddressconfirmedDTO.pChildren = Convert.ToInt32(dr["children"]);
                            FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails = Convert.ToString(dr["employmentorbusinessdetails"]);
                            FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome = Convert.ToDecimal(dr["monthlyincome"]);
                            FieldverificationDTO.pFieldVerifiersComments = Convert.ToString(dr["fieldverifierscomments"]);
                            FieldverificationDTO.pFieldVerifiersRating = Convert.ToString(dr["fieldverifiersrating"]);
                            FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersComments = Convert.ToString(dr["fieldverifierscomments"]);
                            FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersRating = Convert.ToString(dr["fieldverifiersrating"]);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT recordid, docprooftype, docproofname, isdocumentverified FROM tabapplicationfielddocumentverification where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO.Add(new FielddocumentverificationDTO
                            {
                                Precordid = Convert.ToInt64(dr["recordid"]),
                                Pdocprooftype = Convert.ToString(dr["docprooftype"]),
                                Pdocproofname = Convert.ToString(dr["docproofname"]),
                                pisdocumentverified = Convert.ToBoolean(dr["isdocumentverified"]),
                                ptypeofoperation = "OLD"

                            });
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT recordid, addresslocalitydescription, accessability, typeofaccomodation, approxarea, visiblepoliticalaffiliation, affiliationremarks, addressfurnishing, visibleassets, customercooperation, customeravailability, fieldverifierscomments, fieldverifiersrating FROM tabapplicationfieldverifiersobservation where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.FieldVerifiersobservationDTO.Precordid = Convert.ToInt64(dr["recordid"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pAddressLocalityDescription = Convert.ToString(dr["addresslocalitydescription"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pAccessability = Convert.ToString(dr["accessability"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pTypeofAccomodation = Convert.ToString(dr["typeofaccomodation"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pApproxArea = Convert.ToString(dr["approxarea"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pVisiblePoliticalAffiliation = Convert.ToBoolean(dr["visiblepoliticalaffiliation"].ToString());
                            FieldverificationDTO.FieldVerifiersobservationDTO.pAffiliationRemarks = Convert.ToString(dr["affiliationremarks"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pAddressFurnishing = Convert.ToString(dr["addressfurnishing"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pVisibleAssets = Convert.ToString(dr["visibleassets"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerCooperation = Convert.ToString(dr["customercooperation"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerAvailability = Convert.ToBoolean(dr["customeravailability"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersComments = Convert.ToString(dr["fieldverifierscomments"]);
                            FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersRating = Convert.ToString(dr["fieldverifiersrating"]);
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT recordid, coalesce(nameoftheneighbour,'') as nameoftheneighbour, coalesce(addressofneighbour,'') as addressofneighbour, coalesce(isapplicantstayhere,false) as isapplicantstayhere, coalesce(houseownership,'') as houseownership, coalesce(applicantisstayingsince,'') as applicantisstayingsince , coalesce(fieldverifierscomments,'') as fieldverifierscomments, coalesce(fieldverifiersrating,'') as fieldverifiersrating FROM tabapplicationfieldverifyneighbourcheck where upper(vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            FieldverificationDTO.FieldVerifyneighbourcheckDTO.Add(new FieldVerifyneighbourcheckDTO
                            {
                                Precordid = Convert.ToInt64(dr["recordid"]),
                                pNameoftheNeighbour = Convert.ToString(dr["nameoftheneighbour"]),
                                pAddressofNeighbour = Convert.ToString(dr["addressofneighbour"]),
                                pisApplicantstayhere = Convert.ToBoolean(dr["isapplicantstayhere"]),
                                papplicantisstayingMontsOrYears = "",
                                pHouseOwnership = Convert.ToString(dr["houseownership"]),
                                papplicantisstayingsince = Convert.ToString(dr["applicantisstayingsince"]),
                                pFieldVerifiersComments = Convert.ToString(dr["fieldverifierscomments"]),
                                pFieldVerifiersRating = Convert.ToString(dr["fieldverifiersrating"]),
                                pTypeofoperation = "OLD"
                            });
                        }
                    }


                }
                catch (Exception)
                {

                    throw;
                }
            });
            return FieldverificationDTO;
        }

        public async Task<TeleVerificationDTO> GetVerficationDetails(string strapplicatonid, string ConnectonString)
        {
            strapplicatonid = strapplicatonid.ToUpper().Trim();
            TeleVerificationDTO TeleVerificationDTO = new TeleVerificationDTO();
            ds = new DataSet();
            DataSet ds1 = new DataSet();
            await Task.Run(() =>
            {
                try
                {
                    ds1 = NPGSqlHelper.ExecuteDataset(ConnectonString, CommandType.Text, "select x.applicationid,x.vchapplicationid,x.applicantid,x.contactreferenceid,x.applicantname,x.loantype,x.loanname,x.dateofapplication,coalesce(x.disbursementdate,current_date) as disbursementdate,y.businessentitycontactno from tabapplication x join tblmstcontact y on x.applicantid=y.contactid  where upper(x.vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and x.statusid=" + Convert.ToInt32(Status.Active) + ";");
                    if (ds1 != null)
                    {
                        TeleVerificationDTO.papplicationid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicationid"]);
                        TeleVerificationDTO.pvchapplicationid = Convert.ToString(ds1.Tables[0].Rows[0]["vchapplicationid"]);
                        TeleVerificationDTO.pcontactid = Convert.ToInt64(ds1.Tables[0].Rows[0]["applicantid"]);
                        TeleVerificationDTO.pcontactreferenceid = Convert.ToString(ds1.Tables[0].Rows[0]["contactreferenceid"]);
                        TeleVerificationDTO.pApplicantName = Convert.ToString(ds1.Tables[0].Rows[0]["applicantname"]);
                        TeleVerificationDTO.pDateofApplication = ds1.Tables[0].Rows[0]["dateofapplication"] == DBNull.Value ? null : Convert.ToDateTime(ds1.Tables[0].Rows[0]["dateofapplication"]).ToString("dd/MM/yyyy");
                        TeleVerificationDTO.pDateofDisbursement = ds1.Tables[0].Rows[0]["disbursementdate"] == DBNull.Value ? null : Convert.ToDateTime(ds1.Tables[0].Rows[0]["disbursementdate"]).ToString("dd/MM/yyyy");
                        TeleVerificationDTO.pLoantype = Convert.ToString(ds1.Tables[0].Rows[0]["loantype"]);
                        TeleVerificationDTO.pLoanName = Convert.ToString(ds1.Tables[0].Rows[0]["loanname"]);
                        TeleVerificationDTO.Pconctano = Convert.ToInt64(ds1.Tables[0].Rows[0]["businessentitycontactno"]);
                    }
                    TeleVerificationDTO.CustomerAvailabilityDTO = new CustomerAvailabilityDTO();
                    TeleVerificationDTO.spoketoDTO = new spoketoDTO();
                    TeleVerificationDTO.spoketoDTO.spoketoOtherDTO = new spoketoOtherDTO();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "SELECT recordid,applicationid,vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, investigationexecutiveid, investigationexecutivename, customeravailability, contactto,  spoketo, nameoftheperson, relationshipwithapplicant, televerifierscomments, televerifiersrating,coalesce(t1.createdby,0) as createdby,coalesce(t2.username,'') as username FROM tabapplicationteleverification t1 join tblmstusers t2 on t1.createdby=t2.userid where upper(t1.vchapplicationid) = '" + ManageQuote(strapplicatonid) + "' and t1.statusid =" + Convert.ToInt32(Status.Active) + ";"))
                    {
                        while (dr.Read())
                        {
                            TeleVerificationDTO.precordid = Convert.ToInt64(dr["recordid"]);
                            TeleVerificationDTO.pverificationdate = dr["verificationdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["verificationdate"]).ToString("dd/MM/yyyy");
                            TeleVerificationDTO.pverificationtime = Convert.ToString(dr["verificationtime"]);
                            TeleVerificationDTO.pinvestigationexecutiveid = Convert.ToInt64(dr["investigationexecutiveid"]);
                            TeleVerificationDTO.pinvestigationexecutivename = Convert.ToString(dr["investigationexecutivename"]);
                            TeleVerificationDTO.pVerifierName = Convert.ToString(dr["username"]);
                            TeleVerificationDTO.CustomerAvailabilityDTO.pcustomeravailability = Convert.ToBoolean(dr["customeravailability"].ToString());
                            TeleVerificationDTO.CustomerAvailabilityDTO.pcontacttype = Convert.ToString(dr["contactto"]);
                            TeleVerificationDTO.spoketoDTO.pspoketo = Convert.ToString(dr["spoketo"]);
                            TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.pnameoftheperson = Convert.ToString(dr["nameoftheperson"]);
                            TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.prelationshipwithapplicant = Convert.ToString(dr["relationshipwithapplicant"]);
                            TeleVerificationDTO.pteleverifierscomments = Convert.ToString(dr["televerifierscomments"]);
                            TeleVerificationDTO.pteleverifiersrating = Convert.ToString(dr["televerifiersrating"]);
                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            });
            return TeleVerificationDTO;
        }

        public async Task<bool> SaveFieldverification(FieldverificationDTO FieldverificationDTO, string ConnectonString)
        {
            StringBuilder sbinsert = new StringBuilder();
            StringBuilder sbUPDATEGRID = new StringBuilder();
            bool IsSaved = false;
            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    con = new NpgsqlConnection(ConnectonString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();


                    if (FieldverificationDTO != null)
                    {
                        if (string.IsNullOrEmpty(FieldverificationDTO.pverificationdate))
                        {
                            FieldverificationDTO.pverificationdate = "null";
                        }
                        else
                        {
                            FieldverificationDTO.pverificationdate = "'" + FormatDate(FieldverificationDTO.pverificationdate) + "'";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.pverificationtime))
                        {
                            FieldverificationDTO.pverificationtime = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.pInvestigationexecutivename))
                        {
                            FieldverificationDTO.pInvestigationexecutivename = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.paddresstype))
                        {
                            FieldverificationDTO.paddresstype = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.plongitude))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.plongitude = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pLatitude))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pLatitude = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pHouseownership))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pHouseownership = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pPersonmet))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pPersonmet = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pPersonname))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pPersonname = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pDateofbirth))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pDateofbirth = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersComments))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersComments = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersRating))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersRating = "";
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome.ToString()))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome = 0;
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pChildren.ToString()))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pChildren = 0;
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pEarningmembers.ToString()))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pEarningmembers = 0;
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily.ToString()))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily = 0;
                        }
                        if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress.ToString()))
                        {
                            FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress = 0;
                        }

                        if (string.IsNullOrEmpty(FieldverificationDTO.precordid.ToString()) || FieldverificationDTO.precordid == 0)
                        {
                            sbinsert.Append("INSERT INTO tabapplicationfieldverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, addresstype, isaddressconfirmed,uploadlocationdocpath, longitude, latitude, noofyearsatpresentaddress,houseownership, personmet, personname, relationshipwithapplicant,dateofbirth, maritalstatus, totalnoofmembersinfamily, earningmembers,children, employmentorbusinessdetails, monthlyincome, fieldverifierscomments,fieldverifiersrating, statusid, createdby, createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "'," + FieldverificationDTO.pverificationdate + ",'" + ManageQuote(FieldverificationDTO.pverificationtime) + "'," + FieldverificationDTO.pInvestigationexecutiveid + ",'" + ManageQuote(FieldverificationDTO.pInvestigationexecutivename) + "','" + ManageQuote(FieldverificationDTO.paddresstype) + "'," + FieldverificationDTO.AddressconfirmedDTO.pisaddressconfirmed + " ,'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.plongitude) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pLatitude) + "',coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress + ",0),'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pHouseownership) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonmet) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonname) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pDateofbirth) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus) + "',coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily + ",0),coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pEarningmembers + ",0),coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pChildren + ",0),'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails) + "',coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome + ",0),'" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "','" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "'," + Convert.ToInt32(Status.Active) + "," + FieldverificationDTO.pCreatedby + ",current_timestamp);");

                            //if (FieldverificationDTO.pisaddressconfirmed == false)
                            //{
                            //    sbinsert.Append("INSERT INTO tabapplicationfieldverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, addresstype, isaddressconfirmed,statusid, createdby, createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "'," + FieldverificationDTO.pverificationdate + ",'" + ManageQuote(FieldverificationDTO.pverificationtime) + "'," + FieldverificationDTO.pInvestigationexecutiveid + ",'" + ManageQuote(FieldverificationDTO.pInvestigationexecutivename) + "','" + ManageQuote(FieldverificationDTO.paddresstype) + "'," + FieldverificationDTO.pisaddressconfirmed + " ," + FieldverificationDTO.pCreatedby + "," + Convert.ToInt32(Status.Active) + ",current_timestamp);");
                            //}
                            //else
                            //{
                            // sbinsert.Append("INSERT INTO tabapplicationfieldverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, addresstype, isaddressconfirmed,uploadlocationdocpath, longitude, latitude, noofyearsatpresentaddress,houseownership, personmet, personname, relationshipwithapplicant,dateofbirth, maritalstatus, totalnoofmembersinfamily, earningmembers,children, employmentorbusinessdetails, monthlyincome, fieldverifierscomments,fieldverifiersrating, statusid, createdby, createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "'," + FieldverificationDTO.pverificationdate + ",'" + ManageQuote(FieldverificationDTO.pverificationtime) + "'," + FieldverificationDTO.pInvestigationexecutiveid + ",'" + ManageQuote(FieldverificationDTO.pInvestigationexecutivename) + "','" + ManageQuote(FieldverificationDTO.paddresstype) + "'," + FieldverificationDTO.pisaddressconfirmed + " ,'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.plongitude) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pLatitude) + "'," + FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress + ",'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pHouseownership) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonmet) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonname) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pDateofbirth) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus) + "'," + FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily + "," + FieldverificationDTO.AddressconfirmedDTO.pEarningmembers + "," + FieldverificationDTO.AddressconfirmedDTO.pChildren + ",'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails) + "'," + FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome + ",'" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersComments) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersRating) + "'," + FieldverificationDTO.pCreatedby + "," + Convert.ToInt32(Status.Active) + ",current_timestamp);");
                            //}
                        }
                        else
                        {
                            sbinsert.Append("UPDATE tabapplicationfieldverification set verificationdate=" + FieldverificationDTO.pverificationdate + ", verificationtime='" + ManageQuote(FieldverificationDTO.pverificationtime) + "',investigationexecutiveid = " + FieldverificationDTO.pInvestigationexecutiveid + ", investigationexecutivename = '" + ManageQuote(FieldverificationDTO.pInvestigationexecutivename) + "', addresstype = '" + ManageQuote(FieldverificationDTO.paddresstype) + "',isaddressconfirmed = " + FieldverificationDTO.AddressconfirmedDTO.pisaddressconfirmed + ", uploadlocationdocpath = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath) + "', longitude = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.plongitude) + "', latitude = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pLatitude) + "',noofyearsatpresentaddress = coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress + ",0), houseownership = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pHouseownership) + "', personmet = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonmet) + "', personname = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonname) + "',relationshipwithapplicant = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant) + "', dateofbirth = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pDateofbirth) + "', maritalstatus = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus) + "',totalnoofmembersinfamily = coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily + ",0), earningmembers = coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pEarningmembers + ",0), children = coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pChildren + ",0), employmentorbusinessdetails = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails) + "',monthlyincome = coalesce(" + FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome + ",0), fieldverifierscomments = '" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "', fieldverifiersrating = '" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "',modifiedby = " + FieldverificationDTO.pCreatedby + ", modifieddate = current_timestamp where vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.precordid + ";");
                            // sbinsert.Append("UPDATE tabapplicationfieldverification set verificationdate=" + FieldverificationDTO.pverificationdate + ", verificationtime='" + ManageQuote(FieldverificationDTO.pverificationtime) + "',investigationexecutiveid = " + FieldverificationDTO.pInvestigationexecutiveid + ", investigationexecutivename = '" + ManageQuote(FieldverificationDTO.pInvestigationexecutivename) + "', addresstype = '" + ManageQuote(FieldverificationDTO.paddresstype) + "',isaddressconfirmed = " + FieldverificationDTO.pisaddressconfirmed + ", uploadlocationdocpath = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.puploadlocationdocpath) + "', longitude = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.plongitude) + "', latitude = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pLatitude) + "',noofyearsatpresentaddress = " + FieldverificationDTO.AddressconfirmedDTO.pNoofyearsatpresentaddress + ", houseownership = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pHouseownership) + "', personmet = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonmet) + "', personname = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pPersonname) + "',relationshipwithapplicant = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pRelationshipwithapplicant) + "', dateofbirth = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pDateofbirth) + "', maritalstatus = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pMaritalStatus) + "',totalnoofmembersinfamily = " + FieldverificationDTO.AddressconfirmedDTO.pTotalnoofmembersinfamily + ", earningmembers = " + FieldverificationDTO.AddressconfirmedDTO.pEarningmembers + ", children = " + FieldverificationDTO.AddressconfirmedDTO.pChildren + ", employmentorbusinessdetails = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pEmploymentorbusinessdetails) + "',monthlyincome = " + FieldverificationDTO.AddressconfirmedDTO.pMonthlyincome + ", fieldverifierscomments = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersComments) + "', fieldverifiersrating = '" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.pFieldVerifiersRating) + "',modifiedby = " + FieldverificationDTO.pCreatedby + ", modifieddate = current_timestamp where vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.precordid + "); ");
                        }
                        if (FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO.Count > 0)
                        {
                            string Recordid = string.Empty;
                            for (int i = 0; i < FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocprooftype))
                                {
                                    FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocprooftype = "";
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocproofname))
                                {
                                    FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocproofname = "";
                                }
                                if (FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Precordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Precordid.ToString();
                                    }
                                }
                                if (FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationfielddocumentverification(applicationid, vchapplicationid, contactid, contactreferenceid, docprooftype, docproofname,isdocumentverified,statusid, createdby,createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocprooftype) + "','" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocproofname) + "'," + FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].pisdocumentverified + "," + Convert.ToInt32(Status.Active) + "," + FieldverificationDTO.pCreatedby + ",current_timestamp);");
                                }
                                if (FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                {
                                    sbinsert.Append("UPDATE tabapplicationfielddocumentverification SET docprooftype='" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocprooftype) + "',docproofname='" + ManageQuote(FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Pdocproofname) + "',isdocumentverified=" + FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].pisdocumentverified + ",modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.AddressconfirmedDTO.lstFielddocumentverificationDTO[i].Precordid + ";");
                                }
                            }

                            if (!string.IsNullOrEmpty(Recordid))
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationfielddocumentverification SET STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' AND  RECORDID not in(" + Recordid + ");");
                            }
                            else
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationfielddocumentverification SET STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';");
                            }
                        }
                        if (FieldverificationDTO.FieldVerifiersobservationDTO != null)
                        {
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressLocalityDescription))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pAddressLocalityDescription = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pAccessability))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pAccessability = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pTypeofAccomodation))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pTypeofAccomodation = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pApproxArea))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pApproxArea = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pAffiliationRemarks))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pAffiliationRemarks = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressFurnishing))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pAddressFurnishing = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pVisibleAssets))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pVisibleAssets = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerCooperation))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerCooperation = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersComments))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersComments = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersRating))
                            {
                                FieldverificationDTO.FieldVerifiersobservationDTO.pFieldVerifiersRating = "";
                            }
                            if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifiersobservationDTO.Precordid.ToString()) || FieldverificationDTO.FieldVerifiersobservationDTO.Precordid == 0)
                            {
                                sbinsert.Append("INSERT INTO tabapplicationfieldverifiersobservation(applicationid, vchapplicationid, contactid, contactreferenceid,addresslocalitydescription, accessability, typeofaccomodation, approxarea, visiblepoliticalaffiliation, affiliationremarks,addressfurnishing, visibleassets, customercooperation, customeravailability, fieldverifierscomments, fieldverifiersrating, statusid, createdby,createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressLocalityDescription) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAccessability) + "','" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pTypeofAccomodation) + "','" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pApproxArea) + "', " + FieldverificationDTO.FieldVerifiersobservationDTO.pVisiblePoliticalAffiliation + ",'" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAffiliationRemarks) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressFurnishing) + "','" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pVisibleAssets) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerCooperation) + "', " + FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerAvailability + ", '" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "','" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "'," + Convert.ToInt32(Status.Active) + "," + FieldverificationDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                sbinsert.Append("UPDATE tabapplicationfieldverifiersobservation SET addresslocalitydescription='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressLocalityDescription) + "', accessability='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAccessability) + "', typeofaccomodation='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pTypeofAccomodation) + "', approxarea='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pApproxArea) + "', visiblepoliticalaffiliation=" + FieldverificationDTO.FieldVerifiersobservationDTO.pVisiblePoliticalAffiliation + ", affiliationremarks='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAffiliationRemarks) + "', addressfurnishing='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pAddressFurnishing) + "', visibleassets='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pVisibleAssets) + "', customercooperation='" + ManageQuote(FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerCooperation) + "', customeravailability=" + FieldverificationDTO.FieldVerifiersobservationDTO.pCustomerAvailability + ", fieldverifierscomments='" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "', fieldverifiersrating='" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "' ,modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.FieldVerifiersobservationDTO.Precordid + ";");
                            }
                        }
                        if (FieldverificationDTO.FieldVerifyneighbourcheckDTO.Count > 0)
                        {
                            string rECORDID1 = string.Empty;
                            for (int i = 0; i < FieldverificationDTO.FieldVerifyneighbourcheckDTO.Count; i++)
                            {
                                if (FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(rECORDID1))
                                    {
                                        rECORDID1 = FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid.ToString();
                                    }
                                    else
                                    {
                                        rECORDID1 = rECORDID1 + "," + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pNameoftheNeighbour))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pNameoftheNeighbour = "";
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pAddressofNeighbour))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pAddressofNeighbour = "";
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pHouseOwnership))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pHouseOwnership = "";
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince = "";
                                }
                                if (!string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince))
                                {
                                    if (!string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingMontsOrYears))
                                    {
                                        FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince = FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince + "-" + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingMontsOrYears;
                                    }
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersComments))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersComments = "";
                                }
                                if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersRating))
                                {
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersRating = "";
                                }
                                if (FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationfieldverifyneighbourcheck(applicationid, vchapplicationid, contactid, contactreferenceid,nameoftheneighbour, addressofneighbour, isapplicantstayhere, houseownership, applicantisstayingsince, fieldverifierscomments,fieldverifiersrating, statusid, createdby, createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pNameoftheNeighbour) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pAddressofNeighbour) + "'," + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pisApplicantstayhere + ", '" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pHouseOwnership) + "', '" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince) + "','" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "','" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "'," + Convert.ToInt32(Status.Active) + "," + FieldverificationDTO.pCreatedby + ",current_timestamp);");
                                }
                                if (FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {
                                    sbinsert.Append("UPDATE tabapplicationfieldverifyneighbourcheck SET nameoftheneighbour='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pNameoftheNeighbour) + "', addressofneighbour='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pAddressofNeighbour) + "',isapplicantstayhere=" + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pisApplicantstayhere + ", houseownership='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pHouseOwnership) + "', applicantisstayingsince='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince) + "', fieldverifierscomments='" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "', fieldverifiersrating='" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "',modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid + ";");
                                }
                                //    if (string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid.ToString()) || FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid == 0)
                                //{

                                //}
                                //else
                                //{
                                //    sbinsert.Append("UPDATE tabapplicationfieldverifyneighbourcheck SET nameoftheneighbour='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pNameoftheNeighbour) + "', addressofneighbour='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pAddressofNeighbour) + "',isapplicantstayhere=" + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pisApplicantstayhere + ", houseownership='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pHouseOwnership) + "', applicantisstayingsince='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince) + "', fieldverifierscomments='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersComments) + "', fieldverifiersrating='" + ManageQuote(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].pFieldVerifiersRating) + "',modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' and recordid=" + FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].Precordid + ";");
                                //}

                            }
                            if (!string.IsNullOrEmpty(rECORDID1))
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationfieldverifyneighbourcheck SET STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "' AND  RECORDID not in(" + rECORDID1 + ");");
                            }
                            else
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationfieldverifyneighbourcheck SET STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + FieldverificationDTO.pCreatedby + ",modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';");
                            }

                        }
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabapplicationverificationdetails where  vchapplicationid = '" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';"));
                        if (!string.IsNullOrEmpty(FieldverificationDTO.pFieldVerifiersComments) && !string.IsNullOrEmpty(FieldverificationDTO.pFieldVerifiersRating))
                        {
                            if (count > 0)
                            {
                                sbinsert.Append("update tabapplicationverificationdetails set fieldverificationstatus='Y',fieldverifierscomments='" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "',fieldverifiersrating='" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "',modifiedby=" + FieldverificationDTO.pCreatedby + ", modifieddate=current_timestamp where vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';");

                            }
                            else
                            {
                                sbinsert.Append("INSERT INTO tabapplicationverificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,fieldverificationstatus, fieldverifierscomments, fieldverifiersrating, statusid, createdby,createddate)VALUES (" + FieldverificationDTO.papplicationid + ",'" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "'," + FieldverificationDTO.pcontactid + ",'" + ManageQuote(FieldverificationDTO.pcontactreferenceid) + "','Y','" + ManageQuote(FieldverificationDTO.pFieldVerifiersComments) + "','" + ManageQuote(FieldverificationDTO.pFieldVerifiersRating) + "'," + Convert.ToInt32(Status.Active) + "," + FieldverificationDTO.pCreatedby + ",current_timestamp);");
                            }

                        }

                        string Getloanstatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select loanstatus from tabapplication where  vchapplicationid = '" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';"));
                        if (Getloanstatus == "Loan Approved")
                        {

                            sbinsert.Append("update tabapplication set addressverificationdate =" + FieldverificationDTO.pverificationdate + " where vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';");

                        }
                        else
                        {
                            sbinsert.Append("update tabapplication set addressverificationdate =" + FieldverificationDTO.pverificationdate + ",loanstatus='Field Verification',loanstatusid=" + Convert.ToInt32(Status.Field_Verification) + " where vchapplicationid='" + ManageQuote(FieldverificationDTO.pvchapplicationid) + "';");
                        }
                    }
                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Convert.ToString(sbUPDATEGRID) + "" + sbinsert.ToString());
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
            });
            return IsSaved;
        }

        public async Task<bool> SaveFIverification(FIDocumentViewDTO FIDocumentViewDTO, string ConnectonString)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    con = new NpgsqlConnection(ConnectonString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    sbinsert.Append("INSERT INTO temptabapplicationfiverification(detailsrecordid,applicationid, vchapplicationid, contactid,contactreferenceid, verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname, contacttype, verificationstatus, fiverifierscomments,fiverifiersrating, statusid, createdby, createddate) select recordid,applicationid, vchapplicationid, contactid,contactreferenceid, verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname, contacttype, verificationstatus, fiverifierscomments,fiverifiersrating, statusid, createdby, createddate from tabapplicationfiverification where vchapplicationid='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';");
                    sbinsert.Append("delete from tabapplicationfiverification where vchapplicationid='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';");
                    if (FIDocumentViewDTO.FirstinformationDTO.Count > 0)
                    {
                        if (string.IsNullOrEmpty(FIDocumentViewDTO.pverificationdate))
                        {
                            FIDocumentViewDTO.pverificationdate = "null";
                        }
                        else
                        {
                            FIDocumentViewDTO.pverificationdate = "'" + FormatDate(FIDocumentViewDTO.pverificationdate) + "'";
                        }
                        if (string.IsNullOrEmpty(FIDocumentViewDTO.pverificationtime))
                        {
                            FIDocumentViewDTO.pverificationtime = "";
                        }

                        for (int i = 0; i < FIDocumentViewDTO.FirstinformationDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(FIDocumentViewDTO.FirstinformationDTO[i].IsVerified))
                            {
                                FIDocumentViewDTO.FirstinformationDTO[i].IsVerified = "Not Verified";
                            }
                            else
                            {
                                FIDocumentViewDTO.FirstinformationDTO[i].IsVerified = FIDocumentViewDTO.FirstinformationDTO[i].IsVerified;
                            }
                            //if (FIDocumentViewDTO.FirstinformationDTO[i].ptypeofoperation.ToUpper().Trim()== "CREATE")
                            //{
                            sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[i].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[i].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                            //}
                            //if (FIDocumentViewDTO.FirstinformationDTO[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                            //{
                            //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].psubsectionname) + "', contacttype='Applicant',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[i].pVchapplicationid) + "'; ");
                            //}
                        }

                        if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO != null)
                        {
                            if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified = FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].pContactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].pApplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].pApplicantype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist != null)
                        {
                            if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified = FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].pContactId + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].pDocstoreId.ToString()) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].pContactType) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].pContactType) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationKYCDocumentsDTO.documentstorelist[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.lstcontactPersonalAddressDTO != null)
                        {
                            if (FIDocumentViewDTO.lstcontactPersonalAddressDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.lstcontactPersonalAddressDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].IsVerified = FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].IsVerified;
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.lstcontactPersonalAddressDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }
                        if (FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO != null)
                        {
                            if (FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].IsVerified = FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].IsVerified;
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].pContactId + ",'" + ManageQuote(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.lstApplicationContactPersonalDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalFamilyList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEmployeementList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalIncomeList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.BusinessDetailsDTOList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.businessfinancialdetailsDTOList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalEducationList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalNomineeList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationPersonal.PersonalBankList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationPersonal.PersonalBankList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationPersonal.PersonalBankList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified = FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].pcontactid + ",'" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].pcontactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].papplicanttype) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].psubsectionname) + "', contacttype='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].papplicanttype) + "',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationPersonal.PersonalBankList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }

                        }
                        if (FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences != null)
                        {
                            if (FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified = FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].psubsectionname) + "', contacttype='Applicant',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationReferencesDTO.LobjAppReferences[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO != null)
                        {
                            if (FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified = FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].pContactType) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].psubsectionname) + "', contacttype='Applicant',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationExistingLoanDetailsDTO[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified = FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pContactTYpe) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].psubsectionname) + "', contacttype='Applicant',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }
                        if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList != null)
                        {
                            if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified))
                                    {
                                        FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified = "Not Verified";
                                    }
                                    else
                                    {
                                        FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified = FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified;
                                    }
                                    //if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper().Trim() == "CREATE")
                                    //{
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].psubsectionname) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].pContactTYpe) + "','" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                    //}
                                    //if (FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper().Trim() == "UPDATE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationfiverification SET verificationdate=" + FIDocumentViewDTO.pverificationdate + ", verificationtime='" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "',verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].SectionName) + "', verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].psubsectionname) + "', contacttype='Applicant',verificationstatus = '" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].IsVerified) + "', fiverifierscomments = '" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "', fiverifiersrating = '" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "', modifiedby = " + FIDocumentViewDTO.pCreatedby + ", modifieddate = current_timestamp WHERE vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "' and verifiedsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].SectionName) + "' and verifiedsubsectionname='" + ManageQuote(FIDocumentViewDTO.ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList[i].psubsectionname) + "'; ");
                                    //}
                                }
                            }
                        }

                        //if(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BalanceTransferDTO!=null)
                        //{

                        //}

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstVehicleLoanDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO != null)
                        {
                            if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.IsVerified))
                            {
                                FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.IsVerified = "Not Verified";
                            }
                            sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");

                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationQualifcationDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.lstLoanagainstDepositDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst.Count; i++)
                                {

                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification._HomeLoanDTOLst[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }


                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }

                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }


                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }


                        if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO != null)
                        {
                            if (FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO.Count > 0)
                            {
                                for (int i = 0; i < FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO[i].IsVerified))
                                    {
                                        FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO[i].IsVerified = "Not Verified";
                                    }
                                    sbinsert.Append("INSERT INTO tabapplicationfiverification(applicationid, vchapplicationid, contactid, contactreferenceid,verificationdate, verificationtime, verifiedsectionname, verifiedsubsectionname,contacttype, verificationstatus, fiverifierscomments, fiverifiersrating, statusid, createdby, createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "'," + FIDocumentViewDTO.pverificationdate + ",'" + ManageQuote(FIDocumentViewDTO.pverificationtime) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO[i].SectionName) + "','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO[i].psubsectionname) + "','Applicant','" + ManageQuote(FIDocumentViewDTO._ApplicationLoanSpecificDTOinVerification.BusinessLoanDTO.lstBusinessstockpositionDTO[i].IsVerified) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }




                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabapplicationverificationdetails where  vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';"));
                        if (!string.IsNullOrEmpty(FIDocumentViewDTO.pFIVerifierscomments) && !string.IsNullOrEmpty(FIDocumentViewDTO.pFIVerifiersrating))
                        {
                            if (count > 0)
                            {
                                sbinsert.Append("update tabapplicationverificationdetails set fiverificationstatus='Y',fiverifierscomments='" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "',fiverifiersrating='" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "',modifiedby=" + FIDocumentViewDTO.pCreatedby + ", modifieddate=current_timestamp where vchapplicationid='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';");

                            }
                            else
                            {
                                sbinsert.Append("INSERT INTO tabapplicationverificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,fiverificationstatus,fiverifierscomments,fiverifiersrating, statusid, createdby,createddate)VALUES (" + FIDocumentViewDTO.FirstinformationDTO[0].papplicationid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "'," + FIDocumentViewDTO.FirstinformationDTO[0].pApplicantid + ",'" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pContactreferenceid) + "','Y','" + ManageQuote(FIDocumentViewDTO.pFIVerifierscomments) + "','" + ManageQuote(FIDocumentViewDTO.pFIVerifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + FIDocumentViewDTO.pCreatedby + ",current_timestamp);");
                            }

                        }
                        string Getloanstatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select loanstatus from tabapplication where  vchapplicationid = '" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';"));
                        if (Getloanstatus == "Loan Approved")
                        {
                            sbinsert.Append("update tabapplication set documentverificationdate =" + FIDocumentViewDTO.pverificationdate + " where vchapplicationid='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';");

                        }
                        else
                        {
                            sbinsert.Append("update tabapplication set documentverificationdate =" + FIDocumentViewDTO.pverificationdate + ",loanstatus='Document Verification',loanstatusid=" + Convert.ToInt32(Status.Document_Verification) + " where vchapplicationid='" + ManageQuote(FIDocumentViewDTO.FirstinformationDTO[0].pVchapplicationid) + "';");
                        }
                        // }

                    }
                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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
            });
            return IsSaved;
        }

        public async Task<bool> SaveVerficationDetails(TeleVerificationDTO TeleVerificationDTO, string ConnectonString)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    con = new NpgsqlConnection(ConnectonString);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    if (TeleVerificationDTO != null)
                    {
                        if (string.IsNullOrEmpty(TeleVerificationDTO.pverificationdate))
                        {
                            TeleVerificationDTO.pverificationdate = "null";
                        }
                        else
                        {
                            TeleVerificationDTO.pverificationdate = "'" + FormatDate(TeleVerificationDTO.pverificationdate) + "'";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.pverificationtime))
                        {
                            TeleVerificationDTO.pverificationtime = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.pinvestigationexecutivename))
                        {
                            TeleVerificationDTO.pinvestigationexecutivename = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.pteleverifierscomments))
                        {
                            TeleVerificationDTO.pteleverifierscomments = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.pteleverifiersrating))
                        {
                            TeleVerificationDTO.pteleverifiersrating = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.CustomerAvailabilityDTO.pcontacttype))
                        {
                            TeleVerificationDTO.CustomerAvailabilityDTO.pcontacttype = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.spoketoDTO.pspoketo))
                        {
                            TeleVerificationDTO.spoketoDTO.pspoketo = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.pnameoftheperson))
                        {
                            TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.pnameoftheperson = "";
                        }
                        if (string.IsNullOrEmpty(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.prelationshipwithapplicant))
                        {
                            TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.prelationshipwithapplicant = "";
                        }
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabapplicationverificationdetails where  vchapplicationid = '" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "';"));
                        if (string.IsNullOrEmpty(TeleVerificationDTO.precordid.ToString()) || TeleVerificationDTO.precordid == 0)
                        {
                            sbinsert.Append("INSERT INTO tabapplicationteleverification(applicationid,vchapplicationid,contactid, contactreferenceid,verificationdate, verificationtime, investigationexecutiveid,investigationexecutivename, customeravailability, contactto,spoketo, nameoftheperson, relationshipwithapplicant, televerifierscomments,televerifiersrating, statusid, createdby, createddate)VALUES (" + TeleVerificationDTO.papplicationid + ",'" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "'," + TeleVerificationDTO.pcontactid + ",'" + ManageQuote(TeleVerificationDTO.pcontactreferenceid) + "'," + TeleVerificationDTO.pverificationdate + ",'" + ManageQuote(TeleVerificationDTO.pverificationtime) + "'," + TeleVerificationDTO.pinvestigationexecutiveid + ",'" + ManageQuote(TeleVerificationDTO.pinvestigationexecutivename) + "'," + TeleVerificationDTO.CustomerAvailabilityDTO.pcustomeravailability + ",'" + ManageQuote(TeleVerificationDTO.CustomerAvailabilityDTO.pcontacttype) + "','" + ManageQuote(TeleVerificationDTO.spoketoDTO.pspoketo) + "','" + ManageQuote(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.pnameoftheperson) + "','" + ManageQuote(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.prelationshipwithapplicant) + "','" + ManageQuote(TeleVerificationDTO.pteleverifierscomments) + "','" + ManageQuote(TeleVerificationDTO.pteleverifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + TeleVerificationDTO.pCreatedby + ",current_timestamp);");

                        }
                        else
                        {
                            sbinsert.Append("UPDATE tabapplicationteleverification SET  verificationdate=" + TeleVerificationDTO.pverificationdate + ", verificationtime='" + ManageQuote(TeleVerificationDTO.pverificationtime) + "',investigationexecutiveid=" + TeleVerificationDTO.pinvestigationexecutiveid + ", investigationexecutivename='" + ManageQuote(TeleVerificationDTO.pinvestigationexecutivename) + "', customeravailability=" + TeleVerificationDTO.CustomerAvailabilityDTO.pcustomeravailability + ",contactto='" + ManageQuote(TeleVerificationDTO.CustomerAvailabilityDTO.pcontacttype) + "', spoketo='" + ManageQuote(TeleVerificationDTO.spoketoDTO.pspoketo) + "', nameoftheperson='" + ManageQuote(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.pnameoftheperson) + "', relationshipwithapplicant='" + ManageQuote(TeleVerificationDTO.spoketoDTO.spoketoOtherDTO.prelationshipwithapplicant) + "',televerifierscomments='" + ManageQuote(TeleVerificationDTO.pteleverifierscomments) + "', televerifiersrating='" + ManageQuote(TeleVerificationDTO.pteleverifiersrating) + "', modifiedby=" + TeleVerificationDTO.pCreatedby + ", modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "' and recordid=" + TeleVerificationDTO.precordid + ";");

                        }
                        if (count > 0)
                        {
                            sbinsert.Append("UPDATE tabapplicationverificationdetails SET televerificationstatus='Y',televerifierscomments='" + ManageQuote(TeleVerificationDTO.pteleverifierscomments) + "',televerifiersrating='" + ManageQuote(TeleVerificationDTO.pteleverifiersrating) + "',modifiedby=" + TeleVerificationDTO.pCreatedby + ", modifieddate=current_timestamp WHERE vchapplicationid='" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "';");

                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tabapplicationverificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,televerificationstatus, televerifierscomments, televerifiersrating, statusid, createdby,createddate)VALUES (" + TeleVerificationDTO.papplicationid + ",'" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "'," + TeleVerificationDTO.pcontactid + ",'" + ManageQuote(TeleVerificationDTO.pcontactreferenceid) + "','Y','" + ManageQuote(TeleVerificationDTO.pteleverifierscomments) + "','" + ManageQuote(TeleVerificationDTO.pteleverifiersrating) + "'," + Convert.ToInt32(Status.Active) + "," + TeleVerificationDTO.pCreatedby + ",current_timestamp);");
                        }
                        string Getloanstatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select loanstatus from tabapplication where  vchapplicationid = '" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "';"));
                        if (Getloanstatus == "Loan Approved")
                        {

                            sbinsert.Append("update tabapplication set televerificationdate =" + TeleVerificationDTO.pverificationdate + " where vchapplicationid='" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "';");

                        }
                        else
                        {
                            sbinsert.Append("update tabapplication set televerificationdate =" + TeleVerificationDTO.pverificationdate + ",loanstatus='Tele Verification',loanstatusid=" + Convert.ToInt32(Status.Tele_Verification) + " where vchapplicationid='" + ManageQuote(TeleVerificationDTO.pvchapplicationid) + "';");
                        }
                    }

                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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
            });
            return IsSaved;
        }


        public async Task<AddressconfirmedDTO> GetDetailsOfApplicant(string ContactRefID, string ConnectonString)
        {
            AddressconfirmedDTO AddressconfirmedDTO = new AddressconfirmedDTO();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectonString, CommandType.Text, "select m.*,n.maritalstatus from (select x.*,y.dob from (select distinct t1.contactreferenceid,coalesce(t1.totalnoofmembers,0) as totalnoofmembers,coalesce(t1.noofearningmembers,0) as noofearningmembers,coalesce(t1.noofboyschild,0)+coalesce(t1.noofgirlchild,0) as noofchild,t2.employmenttype from tabapplicationpersonalfamilydetails t1 left join tabapplicationpersonalemplymentdetails t2 on t1.contactreferenceid=t2.contactreferenceid)x join tblmstcontact y on x.contactreferenceid=y.contactreferenceid ) m join tabapplicationpersonalbirthdetails n on m.contactreferenceid=n.contactreferenceid where m.contactreferenceid='" + ManageQuote(ContactRefID) + "';"))
                    {
                        while (dr.Read())
                        {
                            AddressconfirmedDTO.pDateofbirth = Convert.ToString(dr["dob"]);
                            if (Convert.ToString(dr["dob"]) != string.Empty)
                            {
                                AddressconfirmedDTO.pAge = CalculateAgeCorrect(Convert.ToDateTime(dr["dob"]));
                            }
                            AddressconfirmedDTO.pTotalnoofmembersinfamily = Convert.ToInt32(dr["totalnoofmembers"]);
                            AddressconfirmedDTO.pEarningmembers = Convert.ToInt32(dr["noofearningmembers"]);
                            AddressconfirmedDTO.pChildren = Convert.ToInt32(dr["noofchild"]);
                            AddressconfirmedDTO.pEmploymentorbusinessdetails = Convert.ToString(dr["employmenttype"]);
                            AddressconfirmedDTO.pMaritalStatus = Convert.ToString(dr["maritalstatus"]);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return AddressconfirmedDTO;
        }

        #region BindLoans - Loan Specific in Verification
        public async Task<ApplicationLoanSpecificDTOinVerification> GetApplicantLoanSpecificDetailsinVerification(string strapplictionid, string ConnectionString)
        {

            await Task.Run(() =>
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid ,producttype, productname, manufacturer, productmodel, quantity,costofproduct,insurancecostoftheproduct, othercost, totalcostofproduct,iswarrantyapplicable, period,periodtype,verificationstatus,'Product Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationconsumerloanproductdetails tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid =" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "' AND tl.STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT showroomname, vehiclemanufacture, VehicleModel, actualvehiclecost,downpayment, onroadprice, requestedamount, engineno, chasisno, registrationno, yearofmake, remarks,verificationstatus,'Vehicle Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationvehicleloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "'; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,goldarticletype, detailsofarticle, carat, grossweight, netweight,appraisedvalueofarticle, observations, articledocpath,COALESCE (docname,'') as docname,verificationstatus,'Gold Details' as verifiedsectionname,'Applicant'||'-'|| 'Gold Details' as  verifiedsubsectionname FROM tabapplicationgoldloandetails tl left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where  tl.vchapplicationid='" + strapplictionid + "' and tl.statusid=" + Convert.ToInt32(Status.Active) + ";"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT nameoftheinstitution, nameofproposedcourse, reasonforselectionoftheinstitute,rankingofinstitution, durationofcourse, dateofcommencement, reasonforseatsecured,verificationstatus,'Educational Institution and Course Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "'; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT address1, address2, city, state, district, country, pincode,stateid,districtid,countryid,verificationstatus,'Address of Institution' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname   FROM tabapplicationeducationloaninstituteaddress tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid WHERE tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,qualification, institute, yearofpassing, noofattempts, markspercentage,grade, isscholarshipsapplicable, scholarshiporprize, scholarshipname,verificationstatus,'Educational Qualification' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloanqualificationdetails  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "' and tl.statusid=" + Convert.ToInt32(Status.Active) + ";"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,totalfundrequirement, nonrepayablescholarship, repayablescholarship,fundsavailablefromfamily,verificationstatus,'Source of Funds' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloanfeedetails  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                _ApplicationLoanSpecificDTOinVerification.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Add(new EducationLoanFeeDetailsDTOVerification
                                {
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,year, qualification, fee,verificationstatus,'Fee Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationeducationloanyearwisefeedetails  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "' and tl.statusid=" + Convert.ToInt32(Status.Active) + ";"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,deposittype, bankcreditfacility, depositaccountnumber, depositamount,depositinterestpercentage, depositdate, deposittenure, depositdocpath,filename,verificationstatus,'Loan against Deposits' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationdepositloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid=" + applicationid + " and tl.vchapplicationid='" + strapplictionid + "' and tl.statusid=" + Convert.ToInt32(Status.Active) + "; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,initialpayment, propertylocation, propertyownershiptype, propertytype, purpose, propertystatus, address1, address2, city, state, district,country, pincode, buildertieup, projectname, ownername, selleraddress,buildingname, blockname, builtupareain, plotarea, undividedshare, plintharea, bookingdate, completiondate, occupancycertificatedate, actualcost, saleagreementvalue, stampdutycharges, otheramenitiescharges, otherincidentalexpenditure, totalvalueofproperty, ageofbuilding,originalcostofproperty, estimatedvalueofrepairs, amountalreadyspent,otherborrowings, totalvalue ,verificationstatus,'Home Loan Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationhomeloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "'; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,descriptionoftheactivity, isfinancialperformanceapplicable, iscredittrendforpurchasesapplicable,iscredittrendforsalesapplicable, isstockpositionapplicable, iscostofprojectapplicable,isancillaryunit, associateconcernsexist,verificationstatus,'Business Loan Applicable Sections' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessloan tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,verificationstatus,'Financial Performance' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessfinancialperformance tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,finacialyear, suppliername, address, contactno, maxcreditreceived,mincreditreceived, avgtotalcreditreceived,verificationstatus,'Credit Trend of Purchases' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname  FROM tblapplicationbusinesscredittrendpurchases tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales,verificationstatus,'Credit Trend of Sales' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname  FROM tblapplicationbusinesscredittrendsales tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,finacialyear, maxstockcarried, minstockcarried, avgtotalstockcarried,verificationstatus,'Stock Position' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinessstockposition tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,costoflandincludingdevelopment, buildingandothercivilworks, plantandmachinery,equipmenttools, testingequipment, miscfixedassets, erectionorinstallationcharges,preliminaryorpreoperativeexpenses, provisionforcontingencies,marginforworkingcapital,verificationstatus,'Cost of Project' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tblapplicationbusinesscostofproject  tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid  where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "';"))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, " SELECT recordid,address1, address2, city, country, state, district, pincode,coalesce(stateid,0) as stateid,coalesce(districtid,0) as districtid,coalesce(countryid,0) as countryid FROM tabapplicationbusinessloanancillaryunitaddressdetails where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "'; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,nameofassociateconcern, natureofassociation, natureofactivity,itemstradedormanufactured,verificationstatus,'Associate Concern Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationbusinessloanassociateconcerndetails tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "' AND tl.STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
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
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT tl.recordid,turnoveryear, turnoveramount, turnoverprofit,verificationstatus,'Turn Over and Profit/Loss Details' as verifiedsectionname,'Applicant'||'-'|| coalesce(t2.name, '')||' '||coalesce(t2.surname,'') as  verifiedsubsectionname FROM tabapplicationbusinessloanturnoverandprofitorloss tl join  TBLMSTCONTACT t2 on tl.contactid=t2.contactid left join tabapplicationfiverification tf on tl.vchapplicationid=tf.vchapplicationid where tl.applicationid = " + applicationid + " and tl.vchapplicationid = '" + strapplictionid + "' AND tl.STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
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
            });

            return _ApplicationLoanSpecificDTOinVerification;

        }
        #endregion
    }
}
