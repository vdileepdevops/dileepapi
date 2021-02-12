using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Settings;
using FinstaRepository.Interfaces.Settings;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FinstaInfrastructure.Common;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Settings
{
    public class SettingsDAL : ISettings
    {


        public List<SettingsDTO> lstlocations { get; set; }
        public List<SettingsDTO> lsttitles { get; set; }
        List<SettingsDTO> lstCompanyDetails { get; set; }
        List<SettingsDTO> lstApplicantypes { get; set; }
        List<SettingsDTO> lstContacttypes { get; set; }
        List<GenerateidMasterDTO> lstFormnames { get; set; }
        List<GenerateidMasterDTO> lstmodeoftransactions { get; set; }
        public List<documentstoreDTO> documentstoredetails { get; set; }
        public List<TdsSectionDTO> lstTdsSectionDetails { get; set; }
        //public static string FormatDate1(string strDate)
        //{
        //    string Date = null;
        //    if (!string.IsNullOrEmpty(strDate))
        //    {
        //        //strDate = Convert.ToDateTime(strDate).ToString("dd-MM-yyyy");

        //        string[] dat = null;
        //        if (strDate != null)
        //        {
        //            if (strDate.Contains("/"))
        //            {
        //                dat = strDate.Split('/');
        //            }
        //            else if (strDate.Contains("-"))
        //            {
        //                dat = strDate.Split('-');
        //            }
        //            Date = dat[2] + "-" + dat[1] + "-" + dat[0];
        //        }
        //    }
        //    return Date;
        //}
        public int CalculateAgeCorrect(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
        public static string FormatDate(string strDate)
        {
            string Date = null;
            if (!string.IsNullOrEmpty(strDate))
            {
                strDate = Convert.ToDateTime(strDate).ToString("dd-MM-yyyy");

                string[] dat = null;
                if (strDate != null)
                {
                    if (strDate.Contains("/"))
                    {
                        dat = strDate.Split('/');
                    }
                    else if (strDate.Contains("-"))
                    {
                        dat = strDate.Split('-');
                    }
                    Date = dat[2] + "-" + dat[1] + "-" + dat[0];
                }
            }
            return Date;
        }
        protected string ManageQuote(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("'", "''");
            }
            else if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }
            return str.Trim();
        }

        protected int getStatusid(string name, string ConnectionString)
        {

            return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select statusid from tblmststatus where upper(statusname)='" + name.ToUpper().Trim() + "';"));
        }
        public List<SettingsDTO> getContacttitles(string ConnectionString)
        {
            lsttitles = new List<SettingsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT titlename from tblmstcontacttitles order by titlename;"))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objtiltles = new SettingsDTO();
                        objtiltles.pTitleName = dr["titlename"].ToString();
                        lsttitles.Add(objtiltles);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lsttitles;

        }


        public List<SettingsDTO> getCountries(string ConnectionString)
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT countryid,country from tblmstcountry order by country ;"))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objcountries = new SettingsDTO();
                        objcountries.pCountry = dr["country"].ToString();
                        objcountries.pCountryId = Convert.ToInt32(dr["countryid"]);
                        lstlocations.Add(objcountries);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstlocations;

        }


        public List<SettingsDTO> getStates(string ConnectionString, int id)
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT stateid,state from tblmststate where countryid=" + id + " order by state;"))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objstates = new SettingsDTO();
                        objstates.pState = dr["state"].ToString();
                        objstates.pStateId = Convert.ToInt32(dr["stateid"]);
                        lstlocations.Add(objstates);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstlocations;

        }


        public List<SettingsDTO> getDistricts(string ConnectionString, int id)
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT districtid,district from tblmstdistrict	 where stateid=" + id + " order by district;"))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objDistricts = new SettingsDTO();
                        objDistricts.pDistrict = dr["district"].ToString();
                        objDistricts.pDistrictId = Convert.ToInt32(dr["districtid"]);
                        lstlocations.Add(objDistricts);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstlocations;

        }

        public List<SettingsDTO> getCompanyandbranchdetails(string ConnectionString)
        {
            lstCompanyDetails = new List<SettingsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT enterprisecode, branchcode from(SELECT row_number() over(partition by companyid) as cid, enterprisecode from tblmstcompany) t1 join(SELECT row_number() over(partition by branchid) as bid, branchcode from tblmstbranch )t2  on t1.cid = t2.bid "))
                {
                    while (dr.Read())
                    {
                        SettingsDTO obj = new SettingsDTO();
                        obj.PEnterprisecode = dr["enterprisecode"].ToString();
                        obj.PBranchcode = dr["branchcode"].ToString();
                        lstCompanyDetails.Add(obj);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstCompanyDetails;

        }

        public List<SettingsDTO> getApplicanttypes(string contacttype, long loanid, string ConnectionString)
        {
            lstApplicantypes = new List<SettingsDTO>();
            string strquery = string.Empty;
            try
            {
                if (loanid > 0)
                {
                    strquery = "select distinct applicanttype from tblmstloanconfiguration where loanid=" + loanid + " and upper(contacttype)='" + contacttype.ToUpper() + "' order by applicanttype;";
                }
                else
                {
                    strquery = "select distinct applicanttype from tblmstapplicantcongiguration where upper(contacttype)='" + contacttype.ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + "  order by applicanttype";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objapplicantypes = new SettingsDTO();
                        objapplicantypes.pApplicanttype = dr["applicanttype"].ToString();
                        objapplicantypes.id = dr["applicanttype"].ToString();
                        objapplicantypes.text = dr["applicanttype"].ToString();
                        lstApplicantypes.Add(objapplicantypes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstApplicantypes;

        }

        public List<SettingsDTO> getContacttypes(int loanid, string ConnectionString)
        {
            string strquery = string.Empty;

            lstContacttypes = new List<SettingsDTO>();
            try
            {
                if (loanid > 0)
                {
                    strquery = "select distinct contacttype from tblmstloanconfiguration where loanid=" + loanid + " and  statusid=" + Convert.ToInt32(Status.Active) + " order by contacttype desc;";
                }
                else
                    strquery = "select distinct contacttype from TBLMSTAPPLICANTCONGIGURATION order by contacttype desc;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        SettingsDTO objContacttypes = new SettingsDTO();
                        objContacttypes.pContacttype = dr["contacttype"].ToString();
                        objContacttypes.id = dr["contacttype"].ToString();
                        objContacttypes.text = dr["contacttype"].ToString();
                        lstContacttypes.Add(objContacttypes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstContacttypes;

        }
        public List<documentstoreDTO> getDocumentstoreDetails(string connectionString, Int64 pContactId, string strapplicationid)
        {
            string strQuery = string.Empty;
            long applicationid = 0;
            long Memberid = 0;
            documentstoredetails = new List<documentstoreDTO>();
            try
            {
                if (!string.IsNullOrEmpty(strapplicationid))
                {
                    applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + strapplicationid + "';"));

                    if (applicationid <= 0)
                    {
                        Memberid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select memberid from tblmstmembers where membercode = '" + strapplicationid + "';"));
                    }
                }

                if (string.IsNullOrEmpty(strapplicationid))
                {
                    strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid=" + pContactId + " and upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=0;";
                }
                else
                {
                    int? applicationdocstorecount = 0;

                    if (applicationid > 0)
                    {
                        applicationdocstorecount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstdocumentstore where coalesce(applicationno,0)=" + applicationid + " and upper(contacttype)!='MEMBER';"));
                    }
                    else if (Memberid > 0)
                    {
                        applicationdocstorecount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstdocumentstore where coalesce(applicationno,0)=" + Memberid + " and upper(contacttype)='MEMBER';"));
                    }
                    if (applicationid > 0)
                    {
                        if (applicationdocstorecount > 0)
                        {
                            strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + "  and upper(t.contacttype)!='MEMBER' union all SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'CREATE' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid  in(select contactid from  tabapplicationkyccreditdetailsapplicablesections where applicationid=" + applicationid + " and  statusid=" + Convert.ToInt32(Status.Active) + " EXCEPT select contactid from tblmstdocumentstore where coalesce(applicationno,0)=" + applicationid + " and upper(contacttype)!='MEMBER') and coalesce(applicationno,0)=0 and upper(ts.statusname)='ACTIVE';";
                        }
                        else
                        {
                            strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'CREATE' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid in(select contactid from  tabapplicationkyccreditdetailsapplicablesections where applicationid=" + applicationid + " and  upper(contacttype)!='MEMBER' and  statusid=" + Convert.ToInt32(Status.Active) + ") and coalesce(applicationno,0)=0 and upper(ts.statusname)='ACTIVE';";
                        }
                    }
                    else if (Memberid > 0)
                    {
                        if (applicationdocstorecount > 0)
                        {

                            strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + Memberid + " and upper(t.contacttype)='MEMBER' union all SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'CREATE' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid  in (select contactid from  tabapplicationkyccreditdetailsapplicablesections where applicationid=" + Memberid + " and  statusid=" + Convert.ToInt32(Status.Active) + " EXCEPT select contactid from tblmstdocumentstore where coalesce(applicationno,0)=" + Memberid + " and upper(contacttype)='MEMBER') and coalesce(applicationno,0)=" + Memberid + " and upper(ts.statusname)='ACTIVE';";

                        }
                        else
                        {
                            strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||' '||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno,coalesce(documentid,0)documentid,coalesce(documentgroupid,0)documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, (case when docisdownloadable is null then false else docisdownloadable end) docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'CREATE' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid in(select contactid from  tabapplicationkyccreditdetailsapplicablesections where applicationid=" + Memberid + " and  upper(contacttype)='MEMBER' and  statusid=" + Convert.ToInt32(Status.Active) + ") and coalesce(applicationno,0)=0 and upper(ts.statusname)='ACTIVE';";
                        }
                    }

                    //strquery= "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + " union all SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid in(select contactid from  tabapplicationkyccreditdetailsapplicablesections where applicationid=" + applicationid + " and  statusid=" + Convert.ToInt32(Status.Active) + ") and coalesce(applicationno,0)=0 and upper(ts.statusname)='ACTIVE';";

                    //strQuery = "SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + " union all SELECT docstoreid,t1.contactreferenceid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,filename FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid=" + pContactId + "  and coalesce(applicationno,0)!=" + applicationid + " and upper(ts.statusname)='ACTIVE'; ";

                    // strQuery = "select t3.*,t4.verifiedsectionname,verificationstatus from (SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,'KYC DocumentsDTO Details' as sectionname FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where upper(ts.statusname)='ACTIVE' and coalesce(applicationno,0)=" + applicationid + " union all SELECT docstoreid,t.contactid,(coalesce(name,'')||''||coalesce(surname,'')) as name, coalesce(loanid,0) loanid, coalesce(applicationno,0) applicationno, documentid, documentgroupid,documentgroupname, documentname, docstorepath, docfiletype, docreferenceno, docisdownloadable, statusname,t.statusid,t.createdby,t.createddate,t.contacttype,'OLD' as typeofoperation,'KYC DocumentsDTO Details' as sectionname FROM tblmstdocumentstore t join tblmststatus ts on t.statusid = ts.statusid join tblmstcontact t1 on t.contactid=t1.contactid  where t.contactid=" + pContactId + " and upper(ts.statusname)='ACTIVE')t3 left join(select* from tabapplicationFIverification where contactid = " + pContactId + ") t4 on t3.contactid = t4.contactid and t3.contacttype = t4.contacttype and t3.sectionname = t4.verifiedsectionname;";

                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        documentstoreDTO objdocumentstore = new documentstoreDTO();
                        objdocumentstore.pDocstoreId = Convert.ToInt64(dr["docstoreid"]);
                        objdocumentstore.pContactId = Convert.ToInt64(dr["contactid"]);
                        objdocumentstore.pContactreferenceid = Convert.ToString(dr["contactreferenceid"]);
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
                        objdocumentstore.pContactType = Convert.ToString(dr["contacttype"]);
                        objdocumentstore.pisapplicable = true;
                        objdocumentstore.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        objdocumentstore.pFilename = Convert.ToString(dr["filename"]);

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

        //public bool saveRelationShip(RelationShipDTO objRelation, string connectionString)
        //{
        //    try
        //    {
        //        return NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "") > 0 ? true : false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



        #region GetTdsSectionNo

        public List<TdsSectionDTO> getTdsSectionNo(string ConnectionString)
        {
            lstTdsSectionDetails = new List<TdsSectionDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,tdssection,coalesce( tdspercentage,0) tdspercentage from tblmsttdssections where statusid=" + Convert.ToInt32(Status.Active) + "  order by tdssection;"))
                {
                    while (dr.Read())
                    {
                        TdsSectionDTO objTdsSectionDetails = new TdsSectionDTO();
                        objTdsSectionDetails.pRecordid = Convert.ToInt64(dr["recordid"]);
                        objTdsSectionDetails.pTdsSection = Convert.ToString(dr["tdssection"]);
                        objTdsSectionDetails.pTdsPercentage = Convert.ToDecimal(dr["tdspercentage"]);
                        lstTdsSectionDetails.Add(objTdsSectionDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstTdsSectionDetails;
        }

        #endregion

        #region CompanyNameand Address

        public CompanyInfoDTO GetcompanyNameandaddressDetails(string Connectionstring)
        {
            CompanyInfoDTO _CompanyInfoDTO = new CompanyInfoDTO();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from vwcompanydetails  where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        _CompanyInfoDTO.pCompanyName = Convert.ToString(dr["nameofenterprise"]);
                        _CompanyInfoDTO.pAddress1 = Convert.ToString(dr["address1"]);
                        _CompanyInfoDTO.pAddress2 = Convert.ToString(dr["address2"]);
                        _CompanyInfoDTO.pState = Convert.ToString(dr["state"]);
                        _CompanyInfoDTO.pDistrict = Convert.ToString(dr["district"]);
                        _CompanyInfoDTO.pcity = Convert.ToString(dr["city"]);
                        _CompanyInfoDTO.pPincode = Convert.ToString(dr["pincode"]);
                        _CompanyInfoDTO.pCinNo = Convert.ToString(dr["cinnumber"]);
                        _CompanyInfoDTO.pGstinNo = Convert.ToString(dr["gstinnumber"]);
                        _CompanyInfoDTO.pBranchname = Convert.ToString(dr["branchname"]);
                        _CompanyInfoDTO.pdatepickerenablestatus = Convert.ToBoolean(dr["datepickerenablestatus"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _CompanyInfoDTO;
        }

        #endregion

        public async Task<bool> GetDatepickerEnableStatus(string con)
        {
            bool DateStatus = false;
            await Task.Run(() =>
            {
                try
                {
                    DateStatus = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select case when datepickerenableenddate<current_date then false else datepickerenablestatus end as datepickerenablestatus from tblmstcompany;"));

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return DateStatus;
        }

        #region Id generation
        public List<GenerateidMasterDTO> GetFormNames(string ConnectionString)
        {
            string strquery = string.Empty;

            lstFormnames = new List<GenerateidMasterDTO>();
            try
            {

                strquery = "select distinct Formname from tabgenerateidmaster WHERE formname <>'APPLICATION' and status='Y'";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GenerateidMasterDTO objGenerateId = new GenerateidMasterDTO();
                        objGenerateId.pFormName = dr["Formname"].ToString();

                        lstFormnames.Add(objGenerateId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFormnames;

        }

        public List<GenerateidMasterDTO> GetModeofTransaction(string Formname, string ConnectionString)
        {
            string strquery = string.Empty;

            lstmodeoftransactions = new List<GenerateidMasterDTO>();
            try
            {


                strquery = "select recordid,filedname from tabgenerateidmaster WHERE formname ='" + Formname.ToUpper() + "' and filedname<> '' and status='Y'";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GenerateidMasterDTO objGenerateId = new GenerateidMasterDTO();
                        objGenerateId.pFieldname = dr["filedname"].ToString();
                        objGenerateId.pTransactionModeId = Convert.ToInt32(dr["recordid"]);
                        lstmodeoftransactions.Add(objGenerateId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstmodeoftransactions;

        }

        public int checkTransactionCodeExist(string TransactionCode, string connectionstring)
        {
            int count = 0;
            try
            {
                if (!string.IsNullOrEmpty(TransactionCode.ToString()))
                {

                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tabgenerateidmaster where code like '" + TransactionCode + "%' and status='Y'"));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;
        }

        public bool SaveGenerateIdMaster(GenerateIdDTO _GenerateIdDTO, string ConnectionString)
        {
            NpgsqlConnection con = null;
            NpgsqlTransaction trans = null;
            string tablename = string.Empty;
            string columnname = string.Empty;
            string concolunname = string.Empty;
            string datecolumnname = string.Empty;
            bool IsSaved = false;
            string finanicalyear = string.Empty;
            string normalyear = string.Empty;
            StringBuilder sbQuery = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_GenerateIdDTO.pGenerateidMasterlist != null)
                {
                    for (int i = 0; i < _GenerateIdDTO.pGenerateidMasterlist.Count; i++)
                    {
                        if (_GenerateIdDTO.pGenerateidMasterlist[i].pSericeReset == "Financial Year")
                        {
                            finanicalyear = "Y";
                            normalyear = "N";
                        }
                        if (_GenerateIdDTO.pGenerateidMasterlist[i].pSericeReset == "Calendar Year")
                        {
                            normalyear = "Y";
                            finanicalyear = "N";

                        }
                        else
                        {
                            finanicalyear = "N";
                            normalyear = "N";
                        }

                        if (string.IsNullOrEmpty(_GenerateIdDTO.pGenerateidMasterlist[i].pFieldname))
                        {
                            NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(trans, CommandType.Text, "select formname,filedname,code,tablename,columnname,concolunname,datecolumnname from tabgenerateidmaster where upper(formname)='" + _GenerateIdDTO.pGenerateidMasterlist[i].pFormName.ToUpper() + "' and status='Y'", null);
                            while (dr.Read())
                            {
                                if (dr != null)
                                {
                                    tablename = Convert.ToString(dr["tablename"]);
                                    columnname = Convert.ToString(dr["columnname"]);
                                    concolunname = Convert.ToString(dr["concolunname"]);
                                    datecolumnname = Convert.ToString(dr["datecolumnname"]);
                                }
                            }

                        }
                        else
                        {
                            NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(trans, CommandType.Text, "select formname,filedname,code,tablename,columnname,concolunname,datecolumnname from tabgenerateidmaster where upper(formname)='" + _GenerateIdDTO.pGenerateidMasterlist[i].pFormName.ToUpper() + "' and upper(filedname)='" + _GenerateIdDTO.pGenerateidMasterlist[i].pFieldname.ToUpper() + "' and status='Y'", null);
                            while (dr.Read())
                            {
                                if (dr != null)
                                {
                                    tablename = Convert.ToString(dr["tablename"]);
                                    columnname = Convert.ToString(dr["columnname"]);
                                    concolunname = Convert.ToString(dr["concolunname"]);
                                    datecolumnname = Convert.ToString(dr["datecolumnname"]);
                                }
                            }
                        }
                        sbQuery.Append("update tabgenerateidmaster set status='N' where recordid=" + _GenerateIdDTO.pGenerateidMasterlist[i].pRecordid + ";");

                        sbQuery.Append("insert into tabgenerateidmaster(Formname,filedname,code,serice,tablename,columnname,concolunname,datecolumnname,finanicalyear,normalyear,status)values('" + ManageQuote(_GenerateIdDTO.pGenerateidMasterlist[i].pFormName.ToUpper()) + "','" + ManageQuote(_GenerateIdDTO.pGenerateidMasterlist[i].pFieldname) + "','" + ManageQuote(_GenerateIdDTO.pGenerateidMasterlist[i].pTransactionCode.ToUpper()) + "','" + ManageQuote(_GenerateIdDTO.pGenerateidMasterlist[i].pTransactionSerice) + "','" + ManageQuote(tablename) + "','" + ManageQuote(columnname) + "','" + ManageQuote(concolunname) + "','" + ManageQuote(datecolumnname) + "','" + ManageQuote(finanicalyear) + "','" + ManageQuote(normalyear) + "','Y');");
                    }
                    if (!string.IsNullOrEmpty(sbQuery.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbQuery.ToString());
                    }
                    trans.Commit();
                    IsSaved = true;
                }



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

        public List<GenerateidMasterDTO> GetGenerateidmasterList(string ConnectionString)
        {
            string strquery = string.Empty;

            lstmodeoftransactions = new List<GenerateidMasterDTO>();
            try
            {

                strquery = "select Recordid,formname,filedname,code,Serice,finanicalyear,normalyear," +
                    "(case when finanicalyear='Y' then 'Financial Year' when normalyear='Y' then 'Calendar Year' when  finanicalyear='N' and normalyear='N' then 'NO' end )as sericereset from tabgenerateidmaster WHERE formname <>'APPLICATION' and status='Y'";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GenerateidMasterDTO objGenerateId = new GenerateidMasterDTO();
                        objGenerateId.pRecordid = Convert.ToInt32(dr["Recordid"]);
                        objGenerateId.pFormName = Convert.ToString(dr["formname"]);
                        objGenerateId.pFieldname = Convert.ToString(dr["filedname"]);
                        objGenerateId.pTransactionCode = Convert.ToString(dr["code"]);
                        objGenerateId.pTransactionSerice = Convert.ToString(dr["Serice"]);
                        objGenerateId.pFinanicalyear = Convert.ToString(dr["finanicalyear"]);
                        objGenerateId.pNormalyear = Convert.ToString(dr["normalyear"]);
                        objGenerateId.pSericeReset = Convert.ToString(dr["sericereset"]);
                        lstmodeoftransactions.Add(objGenerateId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstmodeoftransactions;

        }
        #endregion


        #region All-Party-Details
        public List<ReferralAdvocateDTO> GetAllPartyDetails(string Type, string ConnectionString)
        {
            string Query = string.Empty;
            var lstReferalContactdetails = new List<ReferralAdvocateDTO>();
            if (!string.IsNullOrEmpty(Type))
            {
                Type = Type.ToUpper();
            }
            try
            {
                if (Type == "ALL")
                {
                    Query = "select t1.partiid,t1.contactid,contactreferenceid,(coalesce(t1.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstparty t1 join tblmstcontact t2 on t1.contactid=t2.contactid where t1.statusid=1 order by advocatename ;";
                }
                else
                {
                    Query = "select t1.partiid,t1.contactid,contactreferenceid,(coalesce(t.name,'')||' '||coalesce(t1.surname,''))  as advocatename,t2.businessentitycontactno,t2.businessentityemailid,contacttype from tblmstparty t1 join tblmstcontact t2 on t1.contactid=t2.contactid   where upper(t1.name) like'%" + Type + "%' or upper(t1.surname) like '%" + Type + "%' or upper(t2.businessentityemailid) like '%" + Type + "%' or upper(businessentitycontactno::text) like '%" + Type + "%' or contacttype like '%" + Type + "%' order by advocatename; ";
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
        #endregion

        #region Date Lock
        public bool GetDateLockStatus(string ConnectionString)
        {
            bool Status = false;
            try
            {

                
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select datepickerenablestatus from tblmstcompany where statusid=1 "))
                {
                    while (dr.Read())
                    {
                        Status = Convert.ToBoolean(dr["datepickerenablestatus"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Status;

        }
        #endregion
    }
}
