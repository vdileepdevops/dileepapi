using FinstaInfrastructure.Loans.Masters;
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
    public partial class FirstinformationDAL : SettingsDAL, IFirstinformation
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        public ApplicationLoanSpecificDTOinVerification _ApplicationLoanSpecificDTOinVerification { get; set; }
        public List<LoansMasterDTO> lstLoanMasterdetails { get; set; }
        public FirstinformationDTO FirstinformationDTO { get; set; }

        public ApplicationApplicantandOthersDTO ApplicationApplicantandOthersDTO { set; get; }
        public List<SurityApplicantsDTO> lstsuritypplicants { get; set; }
        public List<FirstinformationDTO> lstFirstinformation { get; set; }
        public List<PropertylocationDTO> lstPropertylocationDTO { get; set; }
        public List<PropertyownershiptypeDTO> lstPropertyownershiptypeDTO { get; set; }
        public List<propertytypeDTO> lstpropertytypeDTO { get; set; }
        public List<purposeDTO> lstpurposeDTO { get; set; }
        public List<propertystatusDTO> lstpropertystatusDTO { get; set; }
        public ApplicationLoanSpecificDTO ApplicationLoanSpecificDTO { set; get; }
        public List<ApplicationExistingLoanDetailsDTO> lstApplicationExistingLoanDetails { get; set; }
        public ExistingLoanDetailsDTO ExistingLoanDetailsDTO { get; set; }
        public List<EducationalQualificationDTO> lstEducationalQualificationDTO { get; set; }
        public List<EducationalFeeYearDTO> lstEducationalFeeYearDTO { get; set; }
        public List<EducationalFeeQualificationDTO> lstEducationalFeeQualificationDTO { get; set; }
        public List<ApplicantIdsDTO> lstApplicantIdsDTO { set; get; }

        public List<ApplicationApplicantandOthersDTO> lstApplicantotherdetailsDTO { set; get; }
        public List<CreditScoreDetailsDTO> lstCreditScoreDetails { get; set; }
        public List<KYCDcumentsDetailsDTO> lstKycDocumentDetails { get; set; }
        public List<GetLoandetailsDTO> lstGetLoandetails { get; set; }
        public List<ApplicationKYCDocumentsDTO> lstApplicationKYCDocuments { get; set; }

        public ApplicationKYCDocumentsDTO ApplicationKYCDocumentsDTO { get; set; }

        public List<ProducttypeDTO> lstProducttype { get; set; }
        public List<AcknowledgementDTO> lstAcknowledgement { get; set; }

        #region  GetSchemenamescodes
        public List<GetLoandetailsDTO> GetSchemenamescodes(string ConnectionString, long Loanid)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select schemeid,schemename,schemecode from tblmstschemenamescodes tc join tblmststatus ts on tc.statusid=ts.statusid where loanid=" + Loanid + " and upper(statusname)='ACTIVE' order by schemename;"))
                {
                    while (dr.Read())
                    {
                        GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                        objGetLoandetails.pschemeid = Convert.ToInt32(dr["schemeid"]);
                        objGetLoandetails.pSchemename = dr["schemename"].ToString();
                        objGetLoandetails.pSchemecode = dr["schemecode"].ToString();
                        lstGetLoandetails.Add(objGetLoandetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }
        #endregion

        #region Get Configured LoanMaster data

        public List<LoansMasterDTO> getfiLoanTypes(string ConnectionString)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT distinct tl.loantypeid,tl.loantype from  tblmstloanconfiguration tc  join  tblmstloantypes  tl on tc.loantypeid=tl.loantypeid join tblmststatus ts on tc.statusid=ts.statusid where upper(ts.statusname)='ACTIVE' order by loantype;"))
                {
                    while (dr.Read())
                    {
                        LoansMasterDTO objamasterdetails = new LoansMasterDTO();
                        objamasterdetails.pLoantype = dr["loantype"].ToString();
                        objamasterdetails.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        lstLoanMasterdetails.Add(objamasterdetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanMasterdetails;
        }

        public List<GetLoandetailsDTO> GetLoanpayin(string ConnectionString, long Loanid, string Contacttype, string Applicanttype, int schemeid)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            string strquery = string.Empty;
            try
            {

                if (schemeid > 0)
                {
                    strquery = "select distinct loanpayin from tblmstloanwiseschemeconfiguration tc join tblmststatus ts on tc.statusid = ts.statusid where loanid = " + Loanid + " and upper(contacttype)= '" + Contacttype.ToUpper() + "' and upper(applicanttype)= '" + Applicanttype.ToUpper() + "' and upper(statusname)= 'ACTIVE' and schemeid=" + schemeid + "  order by loanpayin;";
                }
                else
                {
                    strquery = "select distinct loanpayin from tblmstloanconfiguration where loanid=" + Loanid + " and upper(contacttype)='" + Contacttype.ToUpper() + "' and upper(applicanttype)='" + Applicanttype.ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + " order by loanpayin; ";

                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                        objGetLoandetails.pLoanpayin = dr["loanpayin"].ToString();
                        //objGetLoandetails.pMinloanamount = Convert.ToDecimal(dr["minloanamount"]);
                        //objGetLoandetails.pMaxloanamount = Convert.ToDecimal(dr["maxloanamount"]);
                        //objGetLoandetails.pTenorfrom = Convert.ToDecimal(dr["tenurefrom"]);
                        //objGetLoandetails.pTenorto = Convert.ToDecimal(dr["tenureto"]);
                        //objGetLoandetails.pInteresttype = dr["interesttype"].ToString();
                        //objGetLoandetails.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                        lstGetLoandetails.Add(objGetLoandetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }
        public List<GetLoandetailsDTO> GetLoanInterestTypes(string ConnectionString, long Loanid, int schemeid, string Contacttype, string Applicanttype, string Loanpayin)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            string strquery = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Loanpayin))
                {
                    Loanpayin = "";
                }

                if (schemeid > 0)
                {
                    strquery = "select distinct  interesttype from tblmstloanwiseschemeconfiguration tc join tblmststatus ts on tc.statusid = ts.statusid where loanid = " + Loanid + " and upper(statusname)= 'ACTIVE' and schemeid=" + schemeid + " and upper(contacttype)='" + Contacttype.ToUpper() + "' and upper(applicanttype)='" + Applicanttype.ToUpper() + "' and upper(loanpayin)='" + Loanpayin.ToUpper() + "'  order by interesttype desc;";
                }
                else
                {
                    strquery = "select distinct interesttype from tblmstloanconfiguration tc join tblmststatus ts on tc.statusid = ts.statusid  where loanid=" + Loanid + "  and upper(statusname)= 'ACTIVE' and upper(contacttype)='" + Contacttype.ToUpper() + "' and upper(applicanttype)='" + Applicanttype.ToUpper() + "' and upper(loanpayin)='" + Loanpayin.ToUpper() + "' order by interesttype desc; ";

                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                        objGetLoandetails.pInteresttype = dr["interesttype"].ToString();
                        lstGetLoandetails.Add(objGetLoandetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }

        public List<GetLoandetailsDTO> getLoaninstalmentmodes(string ConnectionString, string loanpayin, string interesttype)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            string strquery = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(loanpayin) && !string.IsNullOrEmpty(interesttype))
                {

                    strquery = "select distinct loaninstalmentpaymode,loaninstalmentpaymodecode from tabloaninstalmentmodes where upper(loanpaymode)='" + loanpayin.ToUpper() + "' and upper(emitype)='" + interesttype.ToUpper() + "' and vchstatus='Y';";


                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                    {
                        while (dr.Read())
                        {
                            GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                            objGetLoandetails.pLoaninstalmentpaymentmode = dr["loaninstalmentpaymode"].ToString();
                            objGetLoandetails.pLoaninstalmentpaymentmodecode = dr["loaninstalmentpaymodecode"].ToString();
                            lstGetLoandetails.Add(objGetLoandetails);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }

        #endregion

        #region  GetInterestRateBasedOnLoanpayin
        public List<GetLoandetailsDTO> GetInterestRates(string ConnectionString, FirstinformationDTO FirstinformationDTO)
        {
            decimal interestrate = 0;
            string strquery = string.Empty;
            bool istenurerangeapplicable = false;
            bool isamountrangeapplicable = false;
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {

                if (FirstinformationDTO.pschemeid > 0)
                {
                    strquery = "select istenurerangeapplicable,  isamountrangeapplicable  from tblmstloanwiseschemeconfiguration tl join tblmststatus ts on tl.statusid=ts.statusid where  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and upper(ts.statusname)='ACTIVE';";


                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, strquery);
                    strquery = string.Empty;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        istenurerangeapplicable = Convert.ToBoolean(ds.Tables[0].Rows[0]["istenurerangeapplicable"]);
                        isamountrangeapplicable = Convert.ToBoolean(ds.Tables[0].Rows[0]["isamountrangeapplicable"]);
                        if (istenurerangeapplicable == true && isamountrangeapplicable == true)
                        {
                            strquery = "select coalesce(schemeinterest,0) as rateofinterest  from tblmstloanwiseschemeconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pTenureofloan + " between tenurefrom and tenureto and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date) and schemeid=" + FirstinformationDTO.pschemeid + "; ";
                        }
                        else if (istenurerangeapplicable == true && isamountrangeapplicable == false)
                        {
                            strquery = "select coalesce(schemeinterest,0) as rateofinterest  from tblmstloanwiseschemeconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pTenureofloan + " between tenurefrom and tenureto and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date) and schemeid=" + FirstinformationDTO.pschemeid + ";";
                        }
                        else if (istenurerangeapplicable == false && isamountrangeapplicable == true)
                        {

                            strquery = "select coalesce(schemeinterest,0) as rateofinterest  from tblmstloanwiseschemeconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pAmountrequested + " between coalesce(minloanamount,0) and coalesce(maxloanamount,0) and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date) and schemeid=" + FirstinformationDTO.pschemeid + ";";
                        }
                        else if (istenurerangeapplicable == false && isamountrangeapplicable == false)
                        {

                            strquery = "select coalesce(schemeinterest,0) as rateofinterest  from tblmstloanwiseschemeconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "'  and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date) and schemeid=" + FirstinformationDTO.pschemeid + ";";
                        }
                    }
                }
                else
                {

                    strquery = "select istenurerangeapplicable,  isamountrangeapplicable  from tblmstloanconfiguration tl join tblmststatus ts on tl.statusid=ts.statusid where  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and upper(ts.statusname)='ACTIVE';";

                    ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, strquery);
                    strquery = string.Empty;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        istenurerangeapplicable = Convert.ToBoolean(ds.Tables[0].Rows[0]["istenurerangeapplicable"]);
                        isamountrangeapplicable = Convert.ToBoolean(ds.Tables[0].Rows[0]["isamountrangeapplicable"]);
                        if (istenurerangeapplicable == true && isamountrangeapplicable == true)
                        {
                            strquery = "select coalesce(rateofinterest,0) as rateofinterest  from tblmstloanconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pTenureofloan + " between tenurefrom and tenureto and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date);";
                        }
                        else if (istenurerangeapplicable == true && isamountrangeapplicable == false)
                        {
                            strquery = "select coalesce(rateofinterest,0) as rateofinterest  from tblmstloanconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pTenureofloan + " between tenurefrom and tenureto and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date);";
                        }
                        else if (istenurerangeapplicable == false && isamountrangeapplicable == true)
                        {

                            strquery = "select coalesce(rateofinterest,0) as rateofinterest  from tblmstloanconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and  " + FirstinformationDTO.pAmountrequested + " between coalesce(minloanamount,0) and coalesce(maxloanamount,0) and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date);";
                        }
                        else if (istenurerangeapplicable == false && isamountrangeapplicable == false)
                        {

                            strquery = "select coalesce(rateofinterest,0) as rateofinterest  from tblmstloanconfiguration  tl join tblmststatus ts on tl.statusid=ts.statusid  where  upper(ts.statusname)='ACTIVE' and  loanid=" + FirstinformationDTO.pLoanid + " and upper(contacttype)='" + FirstinformationDTO.pContacttype.ToUpper() + "' and upper(applicanttype)='" + FirstinformationDTO.pApplicanttype.ToUpper() + "' and loanpayin='" + FirstinformationDTO.pLoanpayin + "'  and upper(interesttype)='" + FirstinformationDTO.pInteresttype.ToUpper() + "' and '" + FormatDate(FirstinformationDTO.pDateofapplication.ToString()) + "' between effectfromdate and coalesce(effecttodate,current_date);";
                        }
                    }
                }

                // interestrate = Convert.ToDecimal(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, strquery));
                if (!string.IsNullOrEmpty(strquery))
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                    {
                        while (dr.Read())
                        {
                            GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                            objGetLoandetails.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                            lstGetLoandetails.Add(objGetLoandetails);
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }


        public List<GetLoandetailsDTO> GetLoanMinandmaxAmounts(string ConnectionString, long Loanid, string Contacttype, string Applicanttype, string Loanpayin, int schemeid, string interesttype)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            string strquery = string.Empty;
            try
            {
                if (schemeid > 0)
                {
                    strquery = "select coalesce(min(coalesce(minloanamount,0)),0) as minloanamount,coalesce(max(coalesce(maxloanamount,0)),0) as maxloanamount from tblmstloanwiseschemeconfiguration tl JOIN tblmststatus ts ON tl.statusid = ts.statusid   where loanid = " + Loanid + " and upper(contacttype)= '" + Contacttype.ToUpper() + "' and upper(applicanttype)= '" + Applicanttype.ToUpper() + "' and upper(statusname)= 'ACTIVE' and schemeid=" + schemeid + "  and upper(loanpayin)='" + Loanpayin.ToUpper() + "' and upper(interesttype)='" + interesttype.ToUpper() + "';";
                }
                else
                {
                    strquery = "select coalesce(min(coalesce(minloanamount,0)),0) as minloanamount,coalesce(max(coalesce(maxloanamount,0)),0) as maxloanamount from tblmstloanconfiguration tl JOIN tblmststatus ts ON tl.statusid = ts.statusid  where loanid=" + Loanid + " and upper(contacttype)='" + Contacttype.ToUpper() + "' and upper(applicanttype)='" + Applicanttype.ToUpper() + "' and loanpayin='" + Loanpayin + "' and upper(interesttype)='" + interesttype.ToUpper() + "';";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strquery))
                {
                    while (dr.Read())
                    {
                        GetLoandetailsDTO objGetLoandetails = new GetLoandetailsDTO();
                        objGetLoandetails.pMinloanamount = Convert.ToDecimal(dr["minloanamount"]);
                        objGetLoandetails.pMaxloanamount = Convert.ToDecimal(dr["maxloanamount"]);
                        lstGetLoandetails.Add(objGetLoandetails);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstGetLoandetails;
        }
        #endregion

        #region  GetAllLoandetails
        public List<FirstinformationDTO> GetAllLoandetails(string Applicationid, string ConnectionString)
        {
            lstFirstinformation = new List<FirstinformationDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * FROM TABAPPLICATION WHERE VCHAPPLICATIONID='" + ManageQuote(Applicationid) + "'"))
                {
                    while (dr.Read())
                    {
                        FirstinformationDTO objFirstinformation = new FirstinformationDTO();
                        objFirstinformation.pVchapplicationid = Convert.ToString(dr["vchapplicationid"]);
                        objFirstinformation.papplicationid = Convert.ToInt64(dr["applicationid"]);
                        objFirstinformation.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                        objFirstinformation.pApplicantid = Convert.ToInt64(dr["Applicantid"]);
                        objFirstinformation.pContactreferenceid = dr["Contactreferenceid"].ToString();
                        objFirstinformation.pApplicantname = dr["Applicantname"].ToString();
                        objFirstinformation.pContacttype = dr["Contacttype"].ToString();
                        objFirstinformation.pLoantypeid = Convert.ToInt64(dr["Loantypeid"]);
                        objFirstinformation.pLoantype = dr["Loantype"].ToString();
                        objFirstinformation.pLoanid = Convert.ToInt64(dr["Loanid"]);
                        objFirstinformation.pLoanname = dr["Loanname"].ToString();
                        objFirstinformation.pApplicanttype = dr["Applicanttype"].ToString();
                        objFirstinformation.pIsschemesapplicable = Convert.ToBoolean(dr["Isschemesapplicable"]);
                        objFirstinformation.pSchemename = dr["Schemename"].ToString();
                        objFirstinformation.pSchemecode = dr["Schemecode"].ToString();
                        objFirstinformation.pAmountrequested = Convert.ToDecimal(dr["Amountrequested"]);
                        objFirstinformation.pPurposeofloan = dr["Purposeofloan"].ToString();
                        objFirstinformation.pLoanpayin = dr["Loanpayin"].ToString();
                        objFirstinformation.pInteresttype = dr["Interesttype"].ToString();
                        objFirstinformation.pRateofinterest = Convert.ToDecimal(dr["Rateofinterest"]);
                        objFirstinformation.pTenureofloan = Convert.ToInt64(dr["Tenureofloan"]);
                        objFirstinformation.pTenuretype = dr["Tenuretype"].ToString();
                        objFirstinformation.pLoaninstalmentpaymentmode = dr["Loaninstalmentpaymentmode"].ToString();
                        objFirstinformation.pPartprinciplepaidinterval = dr["partprinciplepaidinterval"] == DBNull.Value ? 0 : Convert.ToInt32(dr["partprinciplepaidinterval"]);
                        objFirstinformation.pInstalmentamount = Convert.ToDecimal(dr["Instalmentamount"]);

                        objFirstinformation.pIssurietypersonsapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsKYCapplicable = Convert.ToBoolean(dr["IsKYCapplicable"]);
                        objFirstinformation.pIspersonaldetailsapplicable = Convert.ToBoolean(dr["Ispersonaldetailsapplicable"]);
                        objFirstinformation.pIssecurityandcolletralapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsexistingloansapplicable = Convert.ToBoolean(dr["Isexistingloansapplicable"]);
                        objFirstinformation.pIsreferencesapplicable = Convert.ToBoolean(dr["Isreferencesapplicable"]);
                        objFirstinformation.pIsreferralapplicable = Convert.ToBoolean(dr["Isreferralapplicable"]);
                        objFirstinformation.pReferralname = dr["Referralname"].ToString();
                        objFirstinformation.pSalespersonname = dr["Salespersonname"].ToString();
                        objFirstinformation.preferralcontactrefid = Convert.ToString(dr["referralcontactrefid"]);
                        objFirstinformation.psalespersoncontactrefid = Convert.ToString(dr["salespersoncontactrefid"]);
                        objFirstinformation.ptypeofoperation = "UPDATE";
                        // objFirstinformation.lstsurityapplicantsDTO = Getsurietypersondetails(Applicationid, ConnectionString);
                        objFirstinformation.lstCreditscoreDetailsDTO = GetCreditScoreDetails(Applicationid, ConnectionString);
                        //   objFirstinformation.lstKYCDcumentsDetailsDTO = GetKYCDcumentsDetails(Applicationid, ConnectionString);
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
        #endregion

        #region  GetLoanDetails
        public List<FirstinformationDTO> Getloandetails(string Applicationid, string ConnectionString)
        {
            lstFirstinformation = new List<FirstinformationDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * FROM TABAPPLICATION WHERE VCHAPPLICATIONID='" + ManageQuote(Applicationid) + "';"))
                {
                    while (dr.Read())
                    {
                        FirstinformationDTO objFirstinformation = new FirstinformationDTO();
                        objFirstinformation.papplicationid = Convert.ToInt64(dr["applicationid"]);
                        objFirstinformation.pVchapplicationid = dr["vchapplicationid"].ToString();
                        objFirstinformation.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                        objFirstinformation.pApplicantid = Convert.ToInt64(dr["Applicantid"]);
                        objFirstinformation.pContactreferenceid = dr["Contactreferenceid"].ToString();
                        objFirstinformation.pApplicantname = dr["Applicantname"].ToString();
                        objFirstinformation.pContacttype = dr["Contacttype"].ToString();
                        objFirstinformation.pLoantypeid = Convert.ToInt64(dr["Loantypeid"]);
                        objFirstinformation.pLoantype = dr["Loantype"].ToString();
                        objFirstinformation.pLoanid = Convert.ToInt64(dr["Loanid"]);
                        objFirstinformation.pLoanname = dr["Loanname"].ToString();
                        objFirstinformation.pApplicanttype = dr["Applicanttype"].ToString();
                        objFirstinformation.pIsschemesapplicable = Convert.ToBoolean(dr["Isschemesapplicable"]);
                        objFirstinformation.pSchemename = dr["Schemename"].ToString();
                        objFirstinformation.pSchemecode = dr["Schemecode"].ToString();
                        objFirstinformation.pAmountrequested = Convert.ToDecimal(dr["Amountrequested"]);
                        objFirstinformation.pPurposeofloan = dr["Purposeofloan"].ToString();
                        objFirstinformation.pLoanpayin = dr["Loanpayin"].ToString();
                        objFirstinformation.pInteresttype = dr["Interesttype"].ToString();
                        objFirstinformation.pRateofinterest = Convert.ToDecimal(dr["Rateofinterest"]);
                        objFirstinformation.pTenureofloan = Convert.ToInt64(dr["Tenureofloan"]);
                        objFirstinformation.pTenuretype = dr["Tenuretype"].ToString();
                        objFirstinformation.pLoaninstalmentpaymentmode = dr["Loaninstalmentpaymentmode"].ToString();
                        objFirstinformation.pInstalmentamount = Convert.ToDecimal(dr["Instalmentamount"]);

                        objFirstinformation.pIssurietypersonsapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsKYCapplicable = Convert.ToBoolean(dr["IsKYCapplicable"]);
                        objFirstinformation.pIspersonaldetailsapplicable = Convert.ToBoolean(dr["Ispersonaldetailsapplicable"]);
                        objFirstinformation.pIssecurityandcolletralapplicable = Convert.ToBoolean(dr["Issecurityandcolletralapplicable"]);
                        objFirstinformation.pIsexistingloansapplicable = Convert.ToBoolean(dr["Isexistingloansapplicable"]);
                        objFirstinformation.pIsreferencesapplicable = Convert.ToBoolean(dr["Isreferencesapplicable"]);
                        objFirstinformation.pIsreferralapplicable = Convert.ToBoolean(dr["Isreferralapplicable"]);
                        objFirstinformation.pReferralname = dr["Referralname"].ToString();
                        objFirstinformation.pSalespersonname = dr["Salespersonname"].ToString();
                        objFirstinformation.preferralcontactrefid = Convert.ToString(dr["referralcontactrefid"]);
                        objFirstinformation.psalespersoncontactrefid = Convert.ToString(dr["salespersoncontactrefid"]);
                        objFirstinformation.ptypeofoperation = "UPDATE";
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
        #endregion

        #region  GetAcknowledgementDetails
        public List<AcknowledgementDTO> GetAcknowledgementDetails(string ConnectionString)
        {
            lstAcknowledgement = new List<AcknowledgementDTO>();

            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  dateofapplication,titlename,applicationid,vchapplicationid,applicantname,loanid,loanname,amountrequested,businessentitycontactno as contactnumber,businessentityemailid as emailid,case when sentcount>0 then 'Sent' else 'Not Sent' end as sentstatus,sentto,sentthrough,interesttype,rateofinterest,tenureofloan,tenuretype,transdate from(select dateofapplication,titlename,ta.interesttype,ta.rateofinterest,ta.tenureofloan,ta.tenuretype,ta.applicationid, ta.vchapplicationid, ta.applicantname, loanid, loanname, amountrequested, businessentitycontactno, businessentityemailid,sentto,sentthrough,taa.transdate, count(taa.applicationid) as sentcount from tabapplication ta join tblmstcontact tc on ta.contactreferenceid = tc.contactreferenceid  left join tabapplicationacknowledgementdetails taa on ta.applicationid = taa.applicationid group by ta.applicationid, ta.vchapplicationid, ta.applicantname, loanid, loanname, amountrequested,businessentitycontactno, businessentityemailid,taa.sentto,taa.sentthrough,taa.transdate,titlename) g"))
                {
                    while (dr.Read())
                    {
                        AcknowledgementDTO objAcknowledgement = new AcknowledgementDTO();
                        objAcknowledgement.pApplicantid = Convert.ToInt64(dr["applicationid"]);
                        objAcknowledgement.pVchapplicationid = dr["vchapplicationid"].ToString();
                        objAcknowledgement.pApplicantname = dr["applicantname"].ToString();
                        objAcknowledgement.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objAcknowledgement.pLoanname = dr["loanname"].ToString();
                        objAcknowledgement.pLoanamount = Convert.ToDecimal(dr["amountrequested"]);
                        objAcknowledgement.pMobileno = dr["contactnumber"].ToString();
                        objAcknowledgement.pEmail = dr["emailid"].ToString();
                        objAcknowledgement.pSentstatus = dr["sentstatus"].ToString();
                        objAcknowledgement.psentthrough = dr["sentthrough"].ToString();
                        objAcknowledgement.pSentto = dr["sentto"].ToString();
                        if (dr["transdate"] != DBNull.Value)
                        {
                            objAcknowledgement.pTransDate = Convert.ToDateTime(dr["transdate"]);
                        }

                        if (dr["dateofapplication"] != DBNull.Value)
                        {
                            objAcknowledgement.pApplicationdate = Convert.ToDateTime(dr["dateofapplication"]).ToString();
                        }
                        objAcknowledgement.pInteresttype = Convert.ToString(dr["interesttype"]);
                        if (dr["rateofinterest"] != DBNull.Value)
                        {
                            objAcknowledgement.pInterestRate = Convert.ToInt64(dr["rateofinterest"]);
                        }
                        if (dr["tenureofloan"] != DBNull.Value)
                        {
                            objAcknowledgement.pTenureofloan = Convert.ToInt64(dr["tenureofloan"]);
                        }
                        objAcknowledgement.pTenuretype = Convert.ToString(dr["tenuretype"]);
                        objAcknowledgement.pTitlename = Convert.ToString(dr["titlename"]);
                        lstAcknowledgement.Add(objAcknowledgement);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstAcknowledgement;
        }

        public bool SaveAcknowledgementDetails(string ConnectionString, AcknowledgementDTO acknowledgement)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "INSERT INTO tabapplicationacknowledgementdetails( applicantname,applicationid, vchapplicationid, transdate, acknowledgementsent,sentthrough,loanamount,sentto, statusid, createdby, createddate,tenureofloan,tenuretype,interesttype,rateofinterest)VALUES('" + ManageQuote(acknowledgement.pApplicantname) + "'," + acknowledgement.pApplicantid + ",'" + acknowledgement.pVchapplicationid + "','" + FormatDate(acknowledgement.pTransDate.ToString()) + "','true','" + acknowledgement.psentthrough + "'," + acknowledgement.pLoanamount + ",'" + acknowledgement.pSentto + "',1," + acknowledgement.pCreatedby + ",current_timestamp," + acknowledgement.pTenureofloan + ",'" + ManageQuote(acknowledgement.pTenuretype) + "','" + ManageQuote(acknowledgement.pInteresttype) + "'," + acknowledgement.pInterestRate + "); ");

                isSaved = true;


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isSaved;
        }
        #endregion

        #region  GetApplicantCreditandkycdetails
        public ApplicationKYCDocumentsDTO GetApplicantCreditandkycdetails(string Applicationid, long contactid, string ConnectionString)
        {
            bool isKYCapplicable = false;
            ApplicationKYCDocumentsDTO = new ApplicationKYCDocumentsDTO();
            try
            {
                isKYCapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select iskycapplicable from tabapplication where vchapplicationid = '" + ManageQuote(Applicationid) + "';"));
                ApplicationKYCDocumentsDTO.pisKYCapplicable = isKYCapplicable;

                ApplicationKYCDocumentsDTO.lstCreditscoreDetailsDTO = GetCreditScoreDetails(Applicationid, ConnectionString);
                ApplicationKYCDocumentsDTO.documentstorelist = getDocumentstoreDetails(ConnectionString, contactid, Applicationid);
                // lstApplicationKYCDocuments.Add(objApplicationKYCDocuments);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ApplicationKYCDocumentsDTO;
        }
        #endregion

        #region  Getsurietypersondetails
        public ApplicationApplicantandOthersDTO Getsurietypersondetails(string Applicationid, string ConnectionString)
        {
            ApplicationApplicantandOthersDTO = new ApplicationApplicantandOthersDTO();

            try
            {
                bool issurietypersonsapplicable = false;
                issurietypersonsapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select issurietypersonsapplicable from tabapplication where vchapplicationid = '" + Applicationid + "';"));
                ApplicationApplicantandOthersDTO.pissurietypersonsapplicable = issurietypersonsapplicable;
                if (issurietypersonsapplicable == true)
                {
                    ApplicationApplicantandOthersDTO.lstsurityapplicantsDTO = new List<SurityApplicantsDTO>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT ts.recordid ,suritycontactreferenceid,surityapplicanttype,ts.Contactid,ts.Contactname ,tc.businessentitycontactno FROM TABAPPLICATIONSURIETYPERSONSDETAILS ts  JOIN tblmstcontact tc on ts.suritycontactreferenceid=tc.contactreferenceid where  VCHAPPLICATIONID='" + Applicationid + "' and ts.statusid=" + getStatusid("ACTIVE", ConnectionString) + ";"))
                    {
                        while (dr.Read())
                        {
                            ApplicationApplicantandOthersDTO.lstsurityapplicantsDTO.Add(new SurityApplicantsDTO
                            {
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactreferenceid = dr["suritycontactreferenceid"].ToString(),
                                pSurityapplicanttype = dr["surityapplicanttype"].ToString(),
                                pContactid = Convert.ToInt64(dr["Contactid"]),
                                pContactname = dr["Contactname"].ToString(),
                                psuritycontactno = dr["businessentitycontactno"].ToString(),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ApplicationApplicantandOthersDTO;
        }
        #endregion

        #region  GetCreditScoreDetails
        private List<CreditScoreDetailsDTO> GetCreditScoreDetails(string Applicationid, string ConnectionString)
        {
            lstCreditScoreDetails = new List<CreditScoreDetailsDTO>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,tc.contactid,tc.contactreferenceid,(coalesce(name,'')||''||coalesce(surname,'')) as name,applicantype,iscreditscoreapplicable,ciccompany,creditscore,maxcreditscore,creditscorefilepath,filename from tabapplicationancilbildetails tc join tblmststatus ts on tc.statusid = ts.statusid join tblmstcontact t1 on tc.contactid=t1.contactid where vchapplicationid= '" + ManageQuote(Applicationid) + "' and tc.statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        CreditScoreDetailsDTO objCreditScoreDetail = new CreditScoreDetailsDTO();
                        objCreditScoreDetail.pRecordid = Convert.ToInt64(dr["recordid"]);
                        objCreditScoreDetail.pContactid = Convert.ToInt64(dr["contactid"]);
                        objCreditScoreDetail.pApplicantname = Convert.ToString(dr["name"]);
                        objCreditScoreDetail.pApplicanttype = Convert.ToString(dr["applicantype"]);
                        objCreditScoreDetail.pContactreferenceid = dr["contactreferenceid"].ToString();
                        objCreditScoreDetail.pIscreditscoreapplicable = Convert.ToBoolean(dr["iscreditscoreapplicable"]);
                        objCreditScoreDetail.pCiccompany = dr["ciccompany"].ToString();
                        objCreditScoreDetail.pCreditscore = Convert.ToInt64(dr["creditscore"]);
                        objCreditScoreDetail.pMaxcreditscore = Convert.ToInt64(dr["maxcreditscore"]);
                        objCreditScoreDetail.pCreditscorefilepath = dr["creditscorefilepath"].ToString();
                        objCreditScoreDetail.pFilename = Convert.ToString(dr["filename"]);
                        objCreditScoreDetail.pisapplicable = true;
                        objCreditScoreDetail.ptypeofoperation = "OLD";
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
        #endregion

        #region SaveApplication
        public string Saveapplication(ApplicationDTO Applicationlist, string Connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            string IsSaved = "";
            string query = string.Empty;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                //if (!string.IsNullOrEmpty(Applicationlist.FirstinformationDTO.pVchapplicationid))
                //{
                if (Applicationlist.FirstinformationDTO.ptypeofoperation == "CREATE")
                {

                    Applicationlist.FirstinformationDTO.pVchapplicationid = NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "SELECT FN_GENERATENEXTID('APPLICATION','" + ManageQuote(Applicationlist.FirstinformationDTO.pLoanname) + "','" + FormatDate(Applicationlist.FirstinformationDTO.pDateofapplication.ToString()) + "')").ToString();

                    query = "insert into tabapplication(vchapplicationid,dateofapplication,applicantid,contactreferenceid,applicantname,contacttype,loantypeid,loantype,loanid,loanname,applicanttype,isschemesapplicable,schemename,schemecode,amountrequested,purposeofloan,loanpayin,interesttype,rateofinterest,tenureofloan,tenuretype,loaninstalmentpaymentmode,instalmentamount,issurietypersonsapplicable,isKYCapplicable,ispersonaldetailsapplicable,issecurityandcolletralapplicable,isexistingloansapplicable,isreferencesapplicable, isreferralapplicable,partprinciplepaidinterval,loanstatus,loanstatusid, statusid, createdby, createddate)values('" + Applicationlist.FirstinformationDTO.pVchapplicationid + "','" + FormatDate(Applicationlist.FirstinformationDTO.pDateofapplication.ToString()) + "'," + Applicationlist.FirstinformationDTO.pApplicantid + ",'" + ManageQuote(Applicationlist.FirstinformationDTO.pContactreferenceid) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pApplicantname) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pContacttype) + "'," + Applicationlist.FirstinformationDTO.pLoantypeid + ",'" + ManageQuote(Applicationlist.FirstinformationDTO.pLoantype) + "'," + Applicationlist.FirstinformationDTO.pLoanid + ",'" + ManageQuote(Applicationlist.FirstinformationDTO.pLoanname) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pApplicanttype) + "','" + Applicationlist.FirstinformationDTO.pIsschemesapplicable + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pSchemename) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pSchemecode) + "'," + Applicationlist.FirstinformationDTO.pAmountrequested + ",'" + ManageQuote(Applicationlist.FirstinformationDTO.pPurposeofloan) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pLoanpayin) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pInteresttype) + "'," + Applicationlist.FirstinformationDTO.pRateofinterest + "," + Applicationlist.FirstinformationDTO.pTenureofloan + ",'" + ManageQuote(Applicationlist.FirstinformationDTO.pTenuretype) + "','" + ManageQuote(Applicationlist.FirstinformationDTO.pLoaninstalmentpaymentmode) + "'," + Applicationlist.FirstinformationDTO.pInstalmentamount + ",'" + Applicationlist.FirstinformationDTO.pIssurietypersonsapplicable + "','" + Applicationlist.FirstinformationDTO.pIsKYCapplicable + "','" + Applicationlist.FirstinformationDTO.pIspersonaldetailsapplicable + "','" + Applicationlist.FirstinformationDTO.pIssecurityandcolletralapplicable + "','" + Applicationlist.FirstinformationDTO.pIsexistingloansapplicable + "','" + Applicationlist.FirstinformationDTO.pIsreferencesapplicable + "','" + Applicationlist.FirstinformationDTO.pIsreferencesapplicable + "'," + Applicationlist.FirstinformationDTO.pPartprinciplepaidinterval + ",'FI Saved'," + Convert.ToInt32(Status.FI_Saved) + "," + getStatusid(Applicationlist.FirstinformationDTO.pStatusname, Connectionstring) + "," + Applicationlist.FirstinformationDTO.pCreatedby + ",current_timestamp) returning applicationid;";
                    // Applicantid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                }
                if (Applicationlist.FirstinformationDTO.ptypeofoperation == "UPDATE")
                {
                    query = "UPDATE  tabapplication SET dateofapplication= '" + FormatDate(Applicationlist.FirstinformationDTO.pDateofapplication.ToString()) + "', contactreferenceid='" + ManageQuote(Applicationlist.FirstinformationDTO.pContactreferenceid) + "',applicantname='" + ManageQuote(Applicationlist.FirstinformationDTO.pApplicantname) + "',contacttype='" + ManageQuote(Applicationlist.FirstinformationDTO.pContacttype) + "',applicanttype='" + ManageQuote(Applicationlist.FirstinformationDTO.pApplicanttype) + "',isschemesapplicable='" + Applicationlist.FirstinformationDTO.pIsschemesapplicable + "',schemename='" + ManageQuote(Applicationlist.FirstinformationDTO.pSchemename) + "',schemecode='" + ManageQuote(Applicationlist.FirstinformationDTO.pSchemecode) + "',amountrequested=" + Applicationlist.FirstinformationDTO.pAmountrequested + ",purposeofloan='" + ManageQuote(Applicationlist.FirstinformationDTO.pPurposeofloan) + "',loanpayin='" + ManageQuote(Applicationlist.FirstinformationDTO.pLoanpayin) + "',interesttype='" + ManageQuote(Applicationlist.FirstinformationDTO.pInteresttype) + "',rateofinterest=" + Applicationlist.FirstinformationDTO.pRateofinterest + ",tenureofloan=" + Applicationlist.FirstinformationDTO.pTenureofloan + ",tenuretype='" + ManageQuote(Applicationlist.FirstinformationDTO.pTenuretype) + "',loaninstalmentpaymentmode='" + ManageQuote(Applicationlist.FirstinformationDTO.pLoaninstalmentpaymentmode) + "',instalmentamount=" + Applicationlist.FirstinformationDTO.pInstalmentamount + ", isreferralapplicable='" + Applicationlist.FirstinformationDTO.pIsreferralapplicable + "',Referralname='" + Applicationlist.FirstinformationDTO.pReferralname + "',Salespersonname='" + Applicationlist.FirstinformationDTO.pSalespersonname + "', referralcontactrefid='" + ManageQuote(Applicationlist.FirstinformationDTO.preferralcontactrefid) + "', salespersoncontactrefid='" + ManageQuote(Applicationlist.FirstinformationDTO.psalespersoncontactrefid) + "',partprinciplepaidinterval=" + Applicationlist.FirstinformationDTO.pPartprinciplepaidinterval + " where vchapplicationid='" + Applicationlist.FirstinformationDTO.pVchapplicationid + "'";
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query);
                trans.Commit();
                IsSaved = Applicationlist.FirstinformationDTO.pVchapplicationid;
                // }
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
        #endregion

        #region Saveapplicationsurityapplicantdetails
        public bool Saveapplicationsurityapplicantdetails(ApplicationApplicantandOthersDTO Applicationlist, string Connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            string query = string.Empty;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(Applicationlist.pvchapplicationid))
                {
                    Applicationlist.pApplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + Applicationlist.pvchapplicationid + "';"));
                }
                sbinsert.Append("UPDATE tabapplication set issurietypersonsapplicable= " + Applicationlist.pissurietypersonsapplicable + " where vchapplicationid = '" + Applicationlist.pvchapplicationid + "';");

                if (Applicationlist.pissurietypersonsapplicable == true)
                {
                    if (Applicationlist.lstsurityapplicantsDTO != null)
                    {
                        for (int i = 0; i < Applicationlist.lstsurityapplicantsDTO.Count; i++)
                        {
                            if (Applicationlist.lstsurityapplicantsDTO[i].ptypeofoperation == "CREATE")
                            {
                                sbinsert.Append("INSERT INTO tabapplicationsurietypersonsdetails(applicationid,vchapplicationid, suritycontactreferenceid, surityapplicanttype,contactid, contactname, statusid, createdby, createddate)VALUES (" + Applicationlist.pApplicationid + ",'" + Applicationlist.pvchapplicationid + "', '" + Applicationlist.lstsurityapplicantsDTO[i].pContactreferenceid + "', '" + Applicationlist.lstsurityapplicantsDTO[i].pSurityapplicanttype + "', " + Applicationlist.lstsurityapplicantsDTO[i].pContactid + ", '" + Applicationlist.lstsurityapplicantsDTO[i].pContactname + "', " + getStatusid(Applicationlist.lstsurityapplicantsDTO[i].pStatusname, Connectionstring) + ", " + Applicationlist.lstsurityapplicantsDTO[i].pCreatedby + ",current_timestamp);");
                            }
                            if (Applicationlist.lstsurityapplicantsDTO[i].ptypeofoperation == "UPDATE")
                            {
                                sbinsert.Append("UPDATE tabapplicationsurietypersonsdetails SET  suritycontactreferenceid ='" + Applicationlist.lstsurityapplicantsDTO[i].pContactreferenceid + "', surityapplicanttype = '" + Applicationlist.lstsurityapplicantsDTO[i].pSurityapplicanttype + "', contactid = " + Applicationlist.lstsurityapplicantsDTO[i].pContactid + ", contactname = '" + Applicationlist.lstsurityapplicantsDTO[i].pContactname + "' WHERE RECORDID = " + Applicationlist.lstsurityapplicantsDTO[i].pRecordid + ";");
                            }
                        }
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
            return IsSaved;
        }
        #endregion

        #region Savekycandidentificationdocuments
        public bool Savekycandidentificationdocuments(ApplicationKYCDocumentsDTO Applicationlist, string Connectionstring)
        {
            string recordid = string.Empty;
            long ContactId = 0;
            bool IsSaved = false;
            string query = string.Empty;
            StringBuilder sbfirstupdate = new StringBuilder();
            StringBuilder sbinsert = new StringBuilder();
            string updatedquery = string.Empty;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (!string.IsNullOrEmpty(Applicationlist.pVchapplicationid))
                {
                    long AvailableAppcount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select count(*) from tabapplication where vchapplicationid = '" + Applicationlist.pVchapplicationid + "';"));
                    if (AvailableAppcount > 0)
                    {
                        Applicationlist.pApplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + Applicationlist.pVchapplicationid + "';"));

                        sbinsert.Append("UPDATE tabapplication set isKYCapplicable= " + Applicationlist.pisKYCapplicable + " where vchapplicationid = '" + Applicationlist.pVchapplicationid + "';");
                    }
                    else
                    {
                        long AvailableMembercount = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select count(*) from tblmstmembers where membercode = '" + Applicationlist.pVchapplicationid + "';"));
                        if (AvailableMembercount > 0)
                        {
                            Applicationlist.pApplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(Connectionstring, CommandType.Text, "select memberid from tblmstmembers where membercode = '" + Applicationlist.pVchapplicationid + "';"));
                        }
                    }
                }

                //sbinsert.Append("UPDATE tabapplication set isKYCapplicable= " + Applicationlist.pisKYCapplicable + " where vchapplicationid = '" + Applicationlist.pVchapplicationid + "';");

                if (Applicationlist.lstCreditscoreDetailsDTO != null)
                {
                    for (int i = 0; i < Applicationlist.lstCreditscoreDetailsDTO.Count; i++)
                    {
                        sbinsert.Append("update tabapplicationkyccreditdetailsapplicablesections set iscreditscoredetailsapplicable='" + (Applicationlist.lstCreditscoreDetailsDTO[i].pisapplicable) + "', modifiedby='" + (Applicationlist.pCreatedby) + "', modifieddate=current_timestamp where vchapplicationid = '" + ManageQuote(Applicationlist.pVchapplicationid) + "' and  contactreferenceid ='" + ManageQuote(Applicationlist.lstCreditscoreDetailsDTO[i].pContactreferenceid) + "';");

                        if (Applicationlist.lstCreditscoreDetailsDTO[i].ptypeofoperation.ToUpper().Trim() != "CREATE")
                        {
                            if (string.IsNullOrEmpty(recordid))
                            {
                                recordid = Applicationlist.lstCreditscoreDetailsDTO[i].pRecordid.ToString();
                            }
                            else
                            {
                                recordid = recordid + "," + Applicationlist.lstCreditscoreDetailsDTO[i].pRecordid.ToString();
                            }
                        }

                        if (Applicationlist.lstCreditscoreDetailsDTO[i].ptypeofoperation == "CREATE")
                        {
                            if (Applicationlist.lstCreditscoreDetailsDTO[i].pisapplicable == true)
                                sbinsert.Append("INSERT INTO tabapplicationancilbildetails(applicationid,vchapplicationid, contactid, contactreferenceid, applicantype,iscreditscoreapplicable, ciccompany, creditscore,maxcreditscore, creditscorefilepath,filename, statusid, createdby, createddate) VALUES(" + Applicationlist.pApplicationid + ",'" + Applicationlist.pVchapplicationid + "', " + Applicationlist.lstCreditscoreDetailsDTO[i].pContactid + ",'" + Applicationlist.lstCreditscoreDetailsDTO[i].pContactreferenceid + "','" + Applicationlist.lstCreditscoreDetailsDTO[i].pApplicanttype + "'," + Applicationlist.lstCreditscoreDetailsDTO[i].pIscreditscoreapplicable + ", '" + Applicationlist.lstCreditscoreDetailsDTO[i].pCiccompany + "',  " + Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscore + ", " + Applicationlist.lstCreditscoreDetailsDTO[i].pMaxcreditscore + ", '" + ManageQuote(Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath) + "','" + ManageQuote(Applicationlist.lstCreditscoreDetailsDTO[i].pFilename) + "', " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ",current_timestamp);");
                        }
                        if (Applicationlist.lstCreditscoreDetailsDTO[i].ptypeofoperation == "UPDATE")
                        {
                            if (Applicationlist.lstCreditscoreDetailsDTO[i].pisapplicable == true)
                                sbinsert.Append("update  tabapplicationancilbildetails set contactid=" + Applicationlist.lstCreditscoreDetailsDTO[i].pContactid + ", contactreferenceid='" + Applicationlist.lstCreditscoreDetailsDTO[i].pContactreferenceid + "', applicantype='" + Applicationlist.lstCreditscoreDetailsDTO[i].pApplicanttype + "',iscreditscoreapplicable=" + Applicationlist.lstCreditscoreDetailsDTO[i].pIscreditscoreapplicable + ", ciccompany='" + Applicationlist.lstCreditscoreDetailsDTO[i].pCiccompany + "', creditscore= " + Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscore + ",maxcreditscore=" + Applicationlist.lstCreditscoreDetailsDTO[i].pMaxcreditscore + ", creditscorefilepath= '" + ManageQuote(Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath) + "' where recordid =" + Applicationlist.lstCreditscoreDetailsDTO[i].pRecordid + "");
                        }

                    }
                    if (!string.IsNullOrEmpty(recordid))
                    {
                        sbfirstupdate.Append("UPDATE tabapplicationancilbildetails set  statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + Applicationlist.pCreatedby + ", modifieddate = current_timestamp WHERE  vchapplicationid='" + ManageQuote(Applicationlist.pVchapplicationid) + "' AND RECORDID not in(" + recordid + "); ");
                    }
                    else
                    {
                        sbfirstupdate.Append("UPDATE tabapplicationancilbildetails set  statusid=" + Convert.ToInt32(Status.Inactive) + ", modifiedby = " + Applicationlist.pCreatedby + ", modifieddate = current_timestamp WHERE  vchapplicationid='" + ManageQuote(Applicationlist.pVchapplicationid) + "'; ");
                    }
                }
                ReferralAdvocateDAL objReferralAdvocateDAL = new ReferralAdvocateDAL();
                if (Applicationlist.documentstorelist != null)
                {
                    ContactId = Applicationlist.documentstorelist[0].pContactId;
                }
                updatedquery = objReferralAdvocateDAL.UpdateStoreDetails(Applicationlist.documentstorelist, Connectionstring, Applicationlist.pApplicationid, ContactId);

                if (!string.IsNullOrEmpty(updatedquery))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbfirstupdate + "" + sbinsert.ToString() + "" + updatedquery);
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
        #endregion

        #region Deletesueritydetails
        public bool Deletesueritydetails(string strapplictionid, string strconrefid, int Createdby, string Connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            string query = string.Empty;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (strapplictionid != null && strconrefid != null)
                {
                    sbinsert.Append("update tabapplicationsurietypersonsdetails set STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  suritycontactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationancilbildetails SET STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    //sbinsert.Append("update tabapplicationkycdocumentsdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalbankdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalbirthdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonaleducationdetails set STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalemplymentdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalfamilydetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalincomedetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalnomineedetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalotherincomedetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationpersonalemplymentdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationsecuritycollateralimmovablepropertydetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationsecuritycollateralmovablepropertydetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationsecuritycollateralsecuritycheques set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationsecuritycollateraldepositslien set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationsecuritycollateralotherpropertyorsecuritydetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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
        #endregion

        #region Deletecreditandkycdetails
        public bool Deletecreditandkycdetails(string strapplictionid, string strconrefid, int Createdby, string Connectionstring)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            string query = string.Empty;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (strapplictionid != null && strconrefid != null)
                {
                    sbinsert.Append("update tabapplicationancilbildetails SET STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");
                    sbinsert.Append("update tabapplicationkycdocumentsdetails set  STATUSID=" + getStatusid("In-Active", Connectionstring) + ",modifiedby=" + Createdby + ",modifieddate=current_timestamp where vchapplicationid ='" + strapplictionid + "'AND  contactreferenceid ='" + strconrefid + "';");

                    if (!string.IsNullOrEmpty(sbinsert.ToString()))
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
                    }
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
        #endregion

        #region SURITYAPPLICANTNAMESMaster       
        public bool SaveSurityApplicantnames(SurityApplicantsDTO surityApplicantname, string ConnectionString)
        {
            bool isSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstsurityapplicantnamecongiguration(surityapplicanttype,statusid,createdby,createddate)values('" + ManageQuote(surityApplicantname.pSurityapplicanttype.Trim()) + "'," + getStatusid(surityApplicantname.pStatusname, ConnectionString) + "," + surityApplicantname.pCreatedby + ",current_timestamp);");
                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;
        }

        public List<SurityApplicantsDTO> GetSurityapplicants(string ConnectionString, string contacttype)
        {
            lstsuritypplicants = new List<SurityApplicantsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select surityapplicanttype from tblmstsurityapplicantnamecongiguration where upper(contacttype)='" + contacttype.ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + " order by surityapplicanttype"))
                {
                    while (dr.Read())
                    {
                        SurityApplicantsDTO objsuritypplicants = new SurityApplicantsDTO();
                        objsuritypplicants.pSurityapplicanttype = dr["surityapplicanttype"].ToString();
                        lstsuritypplicants.Add(objsuritypplicants);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstsuritypplicants;

        }

        public int checkInsertSuritypplicantsDuplicates(string Suritypplicantstype, string connectionstring)
        {
            int count = 0;
            try
            {

                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstsurityapplicantnamecongiguration where upper(surityapplicanttype)='" + Suritypplicantstype.ToUpper() + "'"));

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }

        #endregion

        #region SaveLoans
        private void SaveApplicantLoan(long applicationid, string strapplictionid, ApplicationLoanSpecificDTO Applicationlist, string ConnectionString, StringBuilder sbinsert)
        {
            ds = new DataSet();
            string query = string.Empty;

            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (Applicationlist.pLoantype.ToUpper().Trim() == "VEHICLE LOAN")
                {
                    if (Applicationlist.lstVehicleLoanDTO.Count > 0)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationvehicleloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        for (int i = 0; i < Applicationlist.lstVehicleLoanDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pDownPayment.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pDownPayment = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pOnroadprice.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pOnroadprice = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount = 0;
                            }

                            if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                            {
                                sbinsert.Append("update tabapplicationvehicleloan set showroomname='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pShowroomName) + "',vehiclemanufacture='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleManufacturer) + "',vehiclemodel='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleModel) + "',actualvehiclecost=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle + ",0),downpayment=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0),onroadprice=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pOnroadprice + ",0),requestedamount=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount + ",0),engineno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pEngineNo) + "',chasisno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pChassisNo) + "',registrationno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pRegistrationNo) + "',yearofmake='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pYearofMake) + "',remarks='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pAnyotherRemarks) + "',modifiedby=1,modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationvehicleloan(applicationid,vchapplicationid,contactid,contactreferenceid,showroomname,vehiclemanufacture,vehiclemodel,actualvehiclecost,downpayment,onroadprice,requestedamount,engineno,chasisno,registrationno,yearofmake,remarks,statusid,createdby,createddate) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pShowroomName) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleManufacturer) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleModel) + "',coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pOnroadprice + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount + ",0),'" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pEngineNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pChassisNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pRegistrationNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pYearofMake) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pAnyotherRemarks) + "',1,1,current_timestamp);");
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "GOLD LOAN")
                {
                    if (Applicationlist.lstGoldLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationgoldloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;SELECT COUNT(*) FROM tabapplicationgoldloandetails WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND STATUSID=1;");

                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pAppraisalDate))
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "null";
                            }
                            else
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "'" + FormatDate(Applicationlist.lstGoldLoanDTO.pAppraisalDate) + "'";
                            }
                            sbinsert.Append("update tblapplicationgoldloan set TotalAppraisedValue=coalesce(" + Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue + ", 0),AppraisalDate=" + Applicationlist.lstGoldLoanDTO.pAppraisalDate + ",AppraisorName='" + ManageQuote(Applicationlist.lstGoldLoanDTO.pAppraisorName) + "',modifiedby=1,modifieddate=CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + ",vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pAppraisalDate))
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "null";
                            }
                            else
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "'" + FormatDate(Applicationlist.lstGoldLoanDTO.pAppraisalDate) + "'";
                            }
                            query = "insert into tblapplicationgoldloan(applicationid,vchapplicationid,contactid,contactreferenceid,TotalAppraisedValue,AppraisalDate,AppraisorName,statusid,createdby,createddate) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue + ", 0)," + Applicationlist.lstGoldLoanDTO.pAppraisalDate + ",'" + ManageQuote(Applicationlist.lstGoldLoanDTO.pAppraisorName) + "',1,1,current_timestamp) returning recordid;";
                            Applicationlist.lstGoldLoanDTO.pRecordid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                        }

                        for (int i = 0; i < Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue = 0;
                            }

                            if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                            {
                                if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {

                                    sbinsert.Append("UPDATE tabapplicationgoldloandetails SET goldarticletype='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "',detailsofarticle='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "',carat='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "',grossweight=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + ",netweight=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",AppraisedValueofArticle=coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),observations='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "',articledocpath='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "',modifiedby=1,modifieddate=CURRENT_TIMESTAMP WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND recordid=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid + ";");

                                }
                                if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {

                                    sbinsert.Append("insert into tabapplicationgoldloandetails(detailsid,vchapplicationid,goldarticletype,detailsofarticle,carat,grossweight,netweight,AppraisedValueofArticle,observations,articledocpath,statusid,createdby,createddate) values(" + Applicationlist.lstGoldLoanDTO.pRecordid + ",'" + ManageQuote(strapplictionid) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "'," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + "," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "',1,1,current_timestamp);");


                                }
                                if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                {
                                    sbinsert.Append("UPDATE tabapplicationgoldloandetails SET STATUSID=2 WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND recordid=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid + ";");
                                }
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationgoldloandetails(detailsid,vchapplicationid,goldarticletype,detailsofarticle,carat,grossweight,netweight,AppraisedValueofArticle,observations,articledocpath,statusid,createdby,createddate) values(" + Applicationlist.lstGoldLoanDTO.pRecordid + ",'" + ManageQuote(strapplictionid) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "'," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + "," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "',1,1,current_timestamp);");
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "EDUCATION LOAN")
                {
                    if (Applicationlist.EducationLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationeducationloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;select count(*) as count from tabapplicationeducationloaninstituteaddress where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;select count(*) as count from tabapplicationeducationloanqualificationdetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;select count(*) as count from tabapplicationeducationloanfeedetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;select count(*) as count from tabapplicationeducationloanyearwisefeedetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.pDateofCommencement))
                        {
                            Applicationlist.EducationLoanDTO.pDateofCommencement = "null";
                        }
                        else
                        {
                            Applicationlist.EducationLoanDTO.pDateofCommencement = "'" + FormatDate(Applicationlist.EducationLoanDTO.pDateofCommencement) + "'";
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tabapplicationeducationloan set nameoftheinstitution='" + ManageQuote(Applicationlist.EducationLoanDTO.pNameoftheinstitution) + "',nameofproposedcourse='" + ManageQuote(Applicationlist.EducationLoanDTO.pNameofProposedcourse) + "',reasonforselectionoftheinstitute='" + ManageQuote(Applicationlist.EducationLoanDTO.pselectionoftheinstitute) + "',rankingofinstitution='" + ManageQuote(Applicationlist.EducationLoanDTO.pRankingofinstitution) + "',durationofcourse='" + ManageQuote(Applicationlist.EducationLoanDTO.pDurationofCourse) + "',dateofcommencement=" + Applicationlist.EducationLoanDTO.pDateofCommencement + ",reasonforseatsecured='" + ManageQuote(Applicationlist.EducationLoanDTO.pseatsecured) + "',modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        }
                        else
                        {
                            sbinsert.Append("insert into tabapplicationeducationloan(applicationid,vchapplicationid,contactid,contactreferenceid,nameoftheinstitution,nameofproposedcourse,reasonforselectionoftheinstitute,rankingofinstitution,durationofcourse,dateofcommencement,reasonforseatsecured,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pNameoftheinstitution) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pNameofProposedcourse) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pselectionoftheinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pRankingofinstitution) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pDurationofCourse) + "'," + Applicationlist.EducationLoanDTO.pDateofCommencement + ",'" + ManageQuote(Applicationlist.EducationLoanDTO.pseatsecured) + "',1,1,current_timestamp);");
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count; i++)
                            {
                                if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tabapplicationeducationloaninstituteaddress set address1='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress1) + "',address2='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress2) + "',city='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCity) + "',state='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pState) + "',district='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrict) + "',country='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountry) + "',pincode='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pPincode) + "',modifiedby=1,modifieddate=current_timestamp,stateid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pStateid) + ",districtid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrictid) + ",countryid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountryid) + "  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloaninstituteaddress(applicationid,vchapplicationid,contactid,contactreferenceid,address1,address2,city,state,district,country,pincode,statusid,createdby,createddate,stateid,districtid,countryid) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress1) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress2) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCity) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pState) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrict) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountry) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pPincode) + "',1,1,current_timestamp," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pStateid) + "," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrictid) + "," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountryid) + ");");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.Count; i++)
                            {

                                if (Convert.ToInt32(ds.Tables[2].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("insert into tabapplicationeducationloanqualificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,qualification,institute,yearofpassing,noofattempts,markspercentage,grade,isscholarshipsapplicable,scholarshiporprize,scholarshipname,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "','" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "',1,1,current_timestamp);");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanqualificationdetails set qualification='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "',institute='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "',yearofpassing='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "',noofattempts='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "',markspercentage='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "',grade='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "',isscholarshipsapplicable='" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "',scholarshiporprize='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "',scholarshipname='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "',modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanqualificationdetails set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid + ";");
                                    }
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloanqualificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,qualification,institute,yearofpassing,noofattempts,markspercentage,grade,isscholarshipsapplicable,scholarshiporprize,scholarshipname,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "','" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "',1,1,current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily = 0;
                                }

                                if (Convert.ToInt32(ds.Tables[3].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tabapplicationeducationloanfeedetails set totalfundrequirement=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement + ", 0),nonrepayablescholarship='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pnonrepayablescholarship) + "',repayablescholarship='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].prepayablescholarship) + "',fundsavailablefromfamily=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily + ", 0),modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloanfeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,totalfundrequirement,nonrepayablescholarship,repayablescholarship,fundsavailablefromfamily,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement + ", 0),'" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pnonrepayablescholarship) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].prepayablescholarship) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily + ", 0),1,1,current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee = 0;
                                }

                                if (Convert.ToInt32(ds.Tables[4].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("insert into tabapplicationeducationloanyearwisefeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,year,qualification,fee,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0),1,1,current_timestamp);");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanyearwisefeedetails set year='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "',qualification='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',fee=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0),modifiedby=1,modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanyearwisefeedetails set statusid=2,modifiedby=1,modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid + ";");
                                    }
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloanyearwisefeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,year,qualification,fee,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0),1,1,current_timestamp);");
                                }
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "LOAN AGAINST DEPOSITS")
                {
                    if (Applicationlist.lstLoanagainstDepositDTO.Count > 0)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationdepositloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        for (int i = 0; i < Applicationlist.lstLoanagainstDepositDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount.ToString()))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage.ToString()))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate = "null";
                            }
                            else
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate = "'" + FormatDate(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate) + "'";
                            }
                            if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                            {
                                if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("insert into tabapplicationdepositloan(applicationid,vchapplicationid,contactid,contactreferenceid,deposittype,bankcreditfacility,depositaccountnumber,depositamount,depositinterestpercentage,depositdate,deposittenure,depositdocpath,statusid,createdby,createddate) values (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0)," + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",'" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "',1,1,current_timestamp);");
                                }
                                if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {
                                    sbinsert.Append("update tabapplicationdepositloan set deposittype='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "',bankcreditfacility='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "',depositaccountnumber='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',depositamount=coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),depositinterestpercentage=coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0),depositdate=" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",deposittenure='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "',depositdocpath='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "',modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.lstLoanagainstDepositDTO[i].pRecordid + ";");
                                }
                                if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                {
                                    sbinsert.Append("update tabapplicationdepositloan set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.lstLoanagainstDepositDTO[i].pRecordid + ";");
                                }
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationdepositloan(applicationid,vchapplicationid,contactid,contactreferenceid,deposittype,bankcreditfacility,depositaccountnumber,depositamount,depositinterestpercentage,depositdate,deposittenure,depositdocpath,statusid,createdby,createddate) values (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0)," + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",'" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "',1,1,current_timestamp);");
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "HOME LOAN")
                {
                    if (Applicationlist.HomeLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationhomeloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pbookingdate))
                        {
                            Applicationlist.HomeLoanDTO.pbookingdate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.pbookingdate = "'" + FormatDate(Applicationlist.HomeLoanDTO.pbookingdate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pcompletiondate))
                        {
                            Applicationlist.HomeLoanDTO.pcompletiondate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.pcompletiondate = "'" + FormatDate(Applicationlist.HomeLoanDTO.pcompletiondate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.poccupancycertificatedate))
                        {
                            Applicationlist.HomeLoanDTO.poccupancycertificatedate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.poccupancycertificatedate = "'" + FormatDate(Applicationlist.HomeLoanDTO.poccupancycertificatedate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pinitialpayment.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pinitialpayment = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pbuiltupareain.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pbuiltupareain = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pplotarea.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pplotarea = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pundividedshare.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pundividedshare = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pplintharea.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pplintharea = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pactualcost.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pactualcost = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.psaleagreementvalue.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.psaleagreementvalue = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pstampdutycharges.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pstampdutycharges = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potheramenitiescharges.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potheramenitiescharges = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potherincidentalexpenditure.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potherincidentalexpenditure = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.ptotalvalueofproperty.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.ptotalvalueofproperty = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.poriginalcostofproperty.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.poriginalcostofproperty = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pamountalreadyspent.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pamountalreadyspent = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potherborrowings.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potherborrowings = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.ptotalvalue.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.ptotalvalue = 0;
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tabapplicationhomeloan set initialpayment=coalesce(" + Applicationlist.HomeLoanDTO.pinitialpayment + ", 0), propertylocation='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertylocation) + "', propertyownershiptype='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertyownershiptype) + "', propertytype='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertytype) + "',purpose='" + ManageQuote(Applicationlist.HomeLoanDTO.ppurpose) + "', propertystatus='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertystatus) + "', address1='" + ManageQuote(Applicationlist.HomeLoanDTO.paddress1) + "', address2='" + ManageQuote(Applicationlist.HomeLoanDTO.paddress2) + "', city='" + ManageQuote(Applicationlist.HomeLoanDTO.pcity) + "', state='" + ManageQuote(Applicationlist.HomeLoanDTO.pstate) + "', district='" + ManageQuote(Applicationlist.HomeLoanDTO.pdistrict) + "',country='" + ManageQuote(Applicationlist.HomeLoanDTO.pcountry) + "', pincode='" + ManageQuote(Applicationlist.HomeLoanDTO.ppincode) + "', buildertieup='" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildertieup) + "', projectname='" + ManageQuote(Applicationlist.HomeLoanDTO.pprojectname) + "', ownername='" + ManageQuote(Applicationlist.HomeLoanDTO.pownername) + "', selleraddress='" + ManageQuote(Applicationlist.HomeLoanDTO.pselleraddress) + "',buildingname='" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildingname) + "', blockname='" + ManageQuote(Applicationlist.HomeLoanDTO.pblockname) + "', builtupareain=coalesce(" + Applicationlist.HomeLoanDTO.pbuiltupareain + ", 0), plotarea=coalesce(" + Applicationlist.HomeLoanDTO.pplotarea + ", 0), undividedshare=coalesce(" + Applicationlist.HomeLoanDTO.pundividedshare + ", 0),plintharea=coalesce(" + Applicationlist.HomeLoanDTO.pplintharea + ", 0), bookingdate=" + Applicationlist.HomeLoanDTO.pbookingdate + ", completiondate=" + Applicationlist.HomeLoanDTO.pcompletiondate + ", occupancycertificatedate=" + Applicationlist.HomeLoanDTO.poccupancycertificatedate + ",actualcost=coalesce(" + Applicationlist.HomeLoanDTO.pactualcost + ", 0), saleagreementvalue=coalesce(" + Applicationlist.HomeLoanDTO.psaleagreementvalue + ", 0), stampdutycharges=coalesce(" + Applicationlist.HomeLoanDTO.pstampdutycharges + ", 0), otheramenitiescharges=coalesce(" + Applicationlist.HomeLoanDTO.potheramenitiescharges + ", 0),otherincidentalexpenditure=coalesce(" + Applicationlist.HomeLoanDTO.potherincidentalexpenditure + ", 0), totalvalueofproperty=coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalueofproperty + ", 0), ageofbuilding='" + ManageQuote(Applicationlist.HomeLoanDTO.pageofbuilding) + "',originalcostofproperty=coalesce(" + Applicationlist.HomeLoanDTO.poriginalcostofproperty + ", 0), estimatedvalueofrepairs=coalesce(" + Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs + ", 0), amountalreadyspent=coalesce(" + Applicationlist.HomeLoanDTO.pamountalreadyspent + ", 0),otherborrowings=coalesce(" + Applicationlist.HomeLoanDTO.potherborrowings + ", 0), totalvalue=coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalue + ", 0),modifiedby=1,modifieddate=current_timestamp,districtid= " + Applicationlist.HomeLoanDTO.pDistrictId + ",stateid= " + Applicationlist.HomeLoanDTO.pStateId + ",countryid=" + Applicationlist.HomeLoanDTO.pCountryId + " where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tabapplicationhomeloan(applicationid, vchapplicationid, contactid, contactreferenceid,initialpayment, propertylocation, propertyownershiptype, propertytype,purpose, propertystatus, address1, address2, city, state, district,country, pincode, buildertieup, projectname, ownername, selleraddress,buildingname, blockname, builtupareain, plotarea, undividedshare,plintharea, bookingdate, completiondate, occupancycertificatedate,actualcost, saleagreementvalue, stampdutycharges, otheramenitiescharges,otherincidentalexpenditure, totalvalueofproperty, ageofbuilding,originalcostofproperty, estimatedvalueofrepairs, amountalreadyspent,otherborrowings, totalvalue, statusid, createdby, createddate,districtid,stateid,countryid)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.HomeLoanDTO.pinitialpayment + ", 0),'" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertylocation) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertyownershiptype) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertytype) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppurpose) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertystatus) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.paddress1) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.paddress2) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pcity) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pstate) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pdistrict) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pcountry) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppincode) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildertieup) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pprojectname) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pownername) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pselleraddress) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildingname) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pblockname) + "', coalesce(" + Applicationlist.HomeLoanDTO.pbuiltupareain + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pplotarea + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pundividedshare + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pplintharea + ", 0)," + Applicationlist.HomeLoanDTO.pbookingdate + "," + Applicationlist.HomeLoanDTO.pcompletiondate + "," + Applicationlist.HomeLoanDTO.poccupancycertificatedate + ",coalesce(" + Applicationlist.HomeLoanDTO.pactualcost + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.psaleagreementvalue + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pstampdutycharges + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potheramenitiescharges + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potherincidentalexpenditure + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalueofproperty + ", 0),'" + ManageQuote(Applicationlist.HomeLoanDTO.pageofbuilding) + "',coalesce(" + Applicationlist.HomeLoanDTO.poriginalcostofproperty + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pamountalreadyspent + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potherborrowings + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalue + ", 0),1,1,current_timestamp, " + Applicationlist.HomeLoanDTO.pDistrictId + ", " + Applicationlist.HomeLoanDTO.pStateId + ", " + Applicationlist.HomeLoanDTO.pCountryId + ");");
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "BALANCE TRANSFER")
                {
                    if (Applicationlist.BalanceTransferDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationbalancetransfer where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ploandate))
                        {
                            Applicationlist.BalanceTransferDTO.ploandate = "null";
                        }
                        else
                        {
                            Applicationlist.BalanceTransferDTO.ploandate = "'" + FormatDate(Applicationlist.BalanceTransferDTO.ploandate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.poutstandingdate))
                        {
                            Applicationlist.BalanceTransferDTO.poutstandingdate = "null";
                        }
                        else
                        {
                            Applicationlist.BalanceTransferDTO.poutstandingdate = "'" + FormatDate(Applicationlist.BalanceTransferDTO.poutstandingdate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ploanamount.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.ploanamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.pinstallmentamount.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.pinstallmentamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ptotaltenureofloan.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.ptotaltenureofloan = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.pbalancetenureofloan.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.pbalancetenureofloan = 0;
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tblapplicationbalancetransfer set bankorcreditfacilityname='" + ManageQuote(Applicationlist.BalanceTransferDTO.pbankorcreditfacilityname) + "',loandate=" + Applicationlist.BalanceTransferDTO.ploandate + ",loanaccountno='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanaccountno) + "',loanamount=coalesce(" + Applicationlist.BalanceTransferDTO.ploanamount + ", 0),outstandingdate=" + Applicationlist.BalanceTransferDTO.poutstandingdate + ",installmentamount=" + Applicationlist.BalanceTransferDTO.pinstallmentamount + ",loanpayin='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanpayin) + "',totaltenureofloan=" + Applicationlist.BalanceTransferDTO.ptotaltenureofloan + ",balancetenureofloan=" + Applicationlist.BalanceTransferDTO.pbalancetenureofloan + ",loansanctiondocpath='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploansanctiondocpath) + "',emichartdocpath='" + ManageQuote(Applicationlist.BalanceTransferDTO.pemichartdocpath) + "',modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tblapplicationbalancetransfer(applicationid,vchapplicationid,contactid,contactreferenceid,bankorcreditfacilityname,loandate,loanaccountno,loanamount,outstandingdate,installmentamount,loanpayin,totaltenureofloan,balancetenureofloan,loansanctiondocpath,emichartdocpath,statusid,createdby,createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BalanceTransferDTO.pbankorcreditfacilityname) + "'," + Applicationlist.BalanceTransferDTO.ploandate + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanaccountno) + "',coalesce(" + Applicationlist.BalanceTransferDTO.ploanamount + ", 0)," + Applicationlist.BalanceTransferDTO.poutstandingdate + "," + Applicationlist.BalanceTransferDTO.pinstallmentamount + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanpayin) + "'," + Applicationlist.BalanceTransferDTO.ptotaltenureofloan + "," + Applicationlist.BalanceTransferDTO.pbalancetenureofloan + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploansanctiondocpath) + "','" + ManageQuote(Applicationlist.BalanceTransferDTO.pemichartdocpath) + "',1,1,current_timestamp);");
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "BUSINESS LOAN")
                {
                    if (Applicationlist.BusinessLoanDTO != null)
                    {

                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationbusinessloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;select count(*) as count from tblapplicationbusinessfinancialperformance where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1; select count(*) as count from tblapplicationbusinesscredittrendpurchases where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1; select count(*) as count from tblapplicationbusinesscredittrendsales where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1; select count(*) as count from tblapplicationbusinessstockposition where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1; select count(*) as count from tblapplicationbusinesscostofproject where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1;");
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tblapplicationbusinessloan set descriptionoftheactivity='" + ManageQuote(Applicationlist.BusinessLoanDTO.pdescriptionoftheactivity) + "', isfinancialperformanceapplicable='" + Applicationlist.BusinessLoanDTO.pisfinancialperformanceapplicable + "', iscredittrendforpurchasesapplicable='" + Applicationlist.BusinessLoanDTO.piscredittrendforpurchasesapplicable + "',iscredittrendforsalesapplicable='" + Applicationlist.BusinessLoanDTO.piscredittrendforsalesapplicable + "', isstockpositionapplicable='" + Applicationlist.BusinessLoanDTO.pisstockpositionapplicable + "', iscostofprojectapplicable='" + Applicationlist.BusinessLoanDTO.piscostofprojectapplicable + "',isancillaryunit='" + Applicationlist.BusinessLoanDTO.pisancillaryunit + "', associateconcernsexist='" + Applicationlist.BusinessLoanDTO.passociateconcernsexist + "',modifiedby=1,modifieddate=current_timestamp  where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = 1;");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tblapplicationbusinessloan(applicationid, vchapplicationid, contactid, contactreferenceid,descriptionoftheactivity, isfinancialperformanceapplicable, iscredittrendforpurchasesapplicable,iscredittrendforsalesapplicable, isstockpositionapplicable, iscostofprojectapplicable,isancillaryunit, associateconcernsexist, statusid, createdby,createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.pdescriptionoftheactivity) + "','" + Applicationlist.BusinessLoanDTO.pisfinancialperformanceapplicable + "','" + Applicationlist.BusinessLoanDTO.piscredittrendforpurchasesapplicable + "','" + Applicationlist.BusinessLoanDTO.piscredittrendforsalesapplicable + "','" + Applicationlist.BusinessLoanDTO.pisstockpositionapplicable + "','" + Applicationlist.BusinessLoanDTO.piscostofprojectapplicable + "','" + Applicationlist.BusinessLoanDTO.pisancillaryunit + "','" + Applicationlist.BusinessLoanDTO.passociateconcernsexist + "',1,1,current_timestamp);");
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinessfinancialperformance(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,statusid, createdby, createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0),'" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "',1,1,current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessfinancialperformance set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "', turnoveramount=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0), netprofitamount=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0), balancesheetdocpath='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "',modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessfinancialperformance set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid + ";");
                                    }
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinessfinancialperformance(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,statusid, createdby, createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0),'" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "',1,1,current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[2].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendpurchases(applicationid, vchapplicationid, contactid, contactreferenceid, finacialyear, suppliername, address, contactno, maxcreditreceived, mincreditreceived, avgtotalcreditreceived, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0), 1, 1, current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendpurchases set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', suppliername='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', address='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', contactno='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', maxcreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), mincreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), avgtotalcreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0),modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendpurchases set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid + ";");
                                    }

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendpurchases(applicationid, vchapplicationid, contactid, contactreferenceid, finacialyear, suppliername, address, contactno, maxcreditreceived, mincreditreceived, avgtotalcreditreceived, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0), 1, 1, current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[3].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendsales(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0),1,1,current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendsales set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "', customername='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "', address='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "', contactno='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "', maxcreditgiven=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0),mincreditgiven=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0), totalcreditsales=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0),modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendsales set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid + ";");
                                    }

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendsales(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0),1,1,current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[4].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinessstockposition(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear,maxstockcarried,minstockcarried, avgtotalstockcarried, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0),1,1,current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessstockposition set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',maxstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0),minstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0), avgtotalstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0),modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid + ";");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessstockposition set statusid=2,modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid + ";");
                                    }
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinessstockposition(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear,maxstockcarried,minstockcarried, avgtotalstockcarried, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0),1,1,current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[5].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tblapplicationbusinesscostofproject set costoflandincludingdevelopment=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment + ", 0), buildingandothercivilworks=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks + ", 0), plantandmachinery=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery + ", 0),equipmenttools=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools + ", 0), testingequipment=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment + ", 0), miscfixedassets=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets + ", 0), erectionorinstallationcharges=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges + ", 0),preliminaryorpreoperativeexpenses=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses + ", 0), provisionforcontingencies=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies + ", 0),marginforworkingcapital=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital + ", 0),modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscostofproject(applicationid, vchapplicationid, contactid, contactreferenceid,costoflandincludingdevelopment, buildingandothercivilworks, plantandmachinery,equipmenttools, testingequipment, miscfixedassets, erectionorinstallationcharges,preliminaryorpreoperativeexpenses, provisionforcontingencies,marginforworkingcapital, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital + ", 0), 1, 1, current_timestamp);");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region save HomeLoan masters

        public bool Savepropertylocation(PropertylocationDTO PropertylocationDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "INSERT INTO tblmstpropertylocation(locationname,statusid,createdby,createddate) VALUES('" + ManageQuote(PropertylocationDTO.plocationname) + "'," + PropertylocationDTO.pStatusid + "," + PropertylocationDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        public bool Savepropertyownershiptype(PropertyownershiptypeDTO PropertyownershiptypeDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstpropertyownershiptype(ownershipname,statusid,createdby,createddate) values('" + ManageQuote(PropertyownershiptypeDTO.pownershipname) + "'," + PropertyownershiptypeDTO.pStatusid + "," + PropertyownershiptypeDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        public bool Savepropertytype(propertytypeDTO propertytypeDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstpropertytype(propertytypename,statusid,createdby,createddate) values('" + ManageQuote(propertytypeDTO.ppropertytype) + "'," + propertytypeDTO.pStatusid + "," + propertytypeDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        public bool Savepurpose(purposeDTO purposeDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstpurpose(purpose,statusid,createdby,createddate) values('" + ManageQuote(purposeDTO.ppurpose) + "'," + purposeDTO.pStatusid + "," + purposeDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        public bool Savepropertystatus(propertystatusDTO propertystatusDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstpropertystatus(propertystatus,statusid,createdby,createddate) values('" + ManageQuote(propertystatusDTO.propertystatus) + "'," + propertystatusDTO.pStatusid + "," + propertystatusDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }
        #endregion

        #region Bind HomeLoan masters
        public List<PropertylocationDTO> BindPropertylocation(string ConnectionString)
        {
            lstPropertylocationDTO = new List<PropertylocationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select locationid,locationname from tblmstpropertylocation where statusid=1 order by locationid;"))
                {
                    while (dr.Read())
                    {
                        PropertylocationDTO PropertylocationDTO = new PropertylocationDTO();
                        PropertylocationDTO.plocationid = Convert.ToInt64(dr["locationid"]);
                        PropertylocationDTO.plocationname = dr["locationname"].ToString();
                        lstPropertylocationDTO.Add(PropertylocationDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstPropertylocationDTO;
        }

        public List<PropertyownershiptypeDTO> BindPropertyownershiptype(string ConnectionString)
        {
            lstPropertyownershiptypeDTO = new List<PropertyownershiptypeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,ownershipname from tblmstpropertyownershiptype where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        PropertyownershiptypeDTO PropertyownershiptypeDTO = new PropertyownershiptypeDTO();
                        PropertyownershiptypeDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        PropertyownershiptypeDTO.pownershipname = dr["ownershipname"].ToString();
                        lstPropertyownershiptypeDTO.Add(PropertyownershiptypeDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstPropertyownershiptypeDTO;
        }

        public List<propertytypeDTO> Bindpropertytype(string ConnectionString)
        {
            lstpropertytypeDTO = new List<propertytypeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,propertytypename from tblmstpropertytype where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        propertytypeDTO propertytypeDTO = new propertytypeDTO();
                        propertytypeDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        propertytypeDTO.ppropertytype = dr["propertytypename"].ToString();
                        lstpropertytypeDTO.Add(propertytypeDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstpropertytypeDTO;
        }

        public List<purposeDTO> Bindpurpose(string ConnectionString)
        {
            lstpurposeDTO = new List<purposeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,purpose from tblmstpurpose where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        purposeDTO purposeDTO = new purposeDTO();
                        purposeDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        purposeDTO.ppurpose = dr["purpose"].ToString();
                        lstpurposeDTO.Add(purposeDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstpurposeDTO;
        }

        public List<propertystatusDTO> Bindpropertystatus(string ConnectionString)
        {
            lstpropertystatusDTO = new List<propertystatusDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,propertystatus from tblmstpropertystatus where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        propertystatusDTO propertystatusDTO = new propertystatusDTO();
                        propertystatusDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        propertystatusDTO.propertystatus = dr["propertystatus"].ToString();
                        lstpropertystatusDTO.Add(propertystatusDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstpropertystatusDTO;
        }
        #endregion

        #region bindLoans
        public async Task<ApplicationLoanSpecificDTO> GetApplicantLoanSpecificDetails(string strapplictionid, string ConnectionString)
        {
            await Task.Run(() =>
            {
                ApplicationLoanSpecificDTO = new ApplicationLoanSpecificDTO();
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
                    ApplicationLoanSpecificDTO.pLoantype = LoanType.ToUpper().Trim();
                    ApplicationLoanSpecificDTO.pVchapplicationid = strapplictionid;
                    ApplicationLoanSpecificDTO.pApplicationid = applicationid;
                    ApplicationLoanSpecificDTO.pApplicantid = Applicatid;
                    ApplicationLoanSpecificDTO.pContactreferenceid = ContactRefId;
                    if (LoanType == "CONSUMER LOAN")
                    {
                        ApplicationLoanSpecificDTO.ConsumerLoanDTO = new ConsumerLoanDTO();
                        ApplicationLoanSpecificDTO.ConsumerLoanDTO.lstConsumerLoanDetailsDTO = new List<ConsumerLoanDetailsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, totalproductcost, downpayment, balanceamount FROM tblapplicationconsumerloan where applicationid =" + applicationid + " and vchapplicationid='" + strapplictionid + "' AND STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.ConsumerLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.ConsumerLoanDTO.ptotalproductcost = Convert.ToDecimal(dr["totalproductcost"]);
                                ApplicationLoanSpecificDTO.ConsumerLoanDTO.pdownpayment = Convert.ToDecimal(dr["downpayment"]);
                                ApplicationLoanSpecificDTO.ConsumerLoanDTO.pbalanceamount = Convert.ToDecimal(dr["balanceamount"]);
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid ,producttype, productname, manufacturer, productmodel, quantity,costofproduct,insurancecostoftheproduct, othercost, totalcostofproduct,iswarrantyapplicable, period,periodtype FROM tblapplicationconsumerloanproductdetails where applicationid =" + applicationid + " and vchapplicationid='" + strapplictionid + "' AND STATUSID=" + Convert.ToInt32(Status.Active) + "; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Add(new ConsumerLoanDetailsDTO
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
                                    pTypeofoperation = "OLD"
                                });
                            }
                        }
                    }

                    #region Loan Related Binding
                    if (LoanType == "VEHICLE LOAN")

                    {
                        ApplicationLoanSpecificDTO.lstVehicleLoanDTO = new List<VehicleLoanDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT showroomname, vehiclemanufacture, VehicleModel, actualvehiclecost,downpayment, onroadprice, requestedamount, engineno, chasisno, registrationno, yearofmake, remarks FROM tabapplicationvehicleloan where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.lstVehicleLoanDTO.Add(new VehicleLoanDTO
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
                                    pAnyotherRemarks = dr["remarks"].ToString()

                                });
                            }
                        }

                    }
                    if (LoanType == "GOLD LOAN")
                    {
                        ApplicationLoanSpecificDTO.lstGoldLoanDTO = new GoldLoanDTO();
                        ApplicationLoanSpecificDTO.lstGoldLoanDTO.lstGoldLoanDetailsDTO = new List<GoldLoanDetailsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, totalappraisedvalue,appraisaldate, appraisorname FROM tblapplicationgoldloan where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {

                                ApplicationLoanSpecificDTO.lstGoldLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.lstGoldLoanDTO.pTotalAppraisedValue = Convert.ToDecimal(dr["totalappraisedvalue"]);
                                ApplicationLoanSpecificDTO.lstGoldLoanDTO.pAppraisalDate = dr["appraisaldate"].ToString();

                                ApplicationLoanSpecificDTO.lstGoldLoanDTO.pAppraisorName = dr["appraisorname"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,goldarticletype, detailsofarticle, carat, grossweight, netweight,appraisedvalueofarticle, observations, articledocpath,COALESCE (docname,'') as docname FROM tabapplicationgoldloandetails where  vchapplicationid='" + strapplictionid + "' and statusid=1;"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Add(new GoldLoanDetailsDTO
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
                                    pTypeofoperation = "OLD"

                                });
                            }
                        }
                    }
                    if (LoanType == "EDUCATION LOAN")
                    {
                        ApplicationLoanSpecificDTO.EducationLoanDTO = new EducationLoanDTO();
                        ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationInstutiteAddressDTO = new List<EducationInstutiteAddressDTO>();
                        ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationLoanFeeDetailsDTO = new List<EducationLoanFeeDetailsDTO>();
                        ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO = new List<EducationLoanyearwiseFeedetailsDTO>();
                        ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationQualifcationDTO = new List<EducationQualifcationDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT nameoftheinstitution, nameofproposedcourse, reasonforselectionoftheinstitute,rankingofinstitution, durationofcourse, dateofcommencement, reasonforseatsecured FROM tabapplicationeducationloan where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pNameoftheinstitution = dr["nameoftheinstitution"].ToString();
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pNameofProposedcourse = dr["nameofproposedcourse"].ToString();
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pselectionoftheinstitute = dr["reasonforselectionoftheinstitute"].ToString();
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pRankingofinstitution = dr["rankingofinstitution"].ToString();
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pDurationofCourse = dr["durationofcourse"].ToString();
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pDateofCommencement = dr["dateofcommencement"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofcommencement"]).ToString("dd/MM/yyyy");
                                ApplicationLoanSpecificDTO.EducationLoanDTO.pseatsecured = dr["reasonforseatsecured"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT address1, address2, city, state, district, country, pincode,stateid,districtid,countryid   FROM tabapplicationeducationloaninstituteaddress WHERE applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationInstutiteAddressDTO.Add(new EducationInstutiteAddressDTO
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
                                    pCountryid = dr["countryid"] == DBNull.Value ? 0 : Convert.ToInt32(dr["countryid"])
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,qualification, institute, yearofpassing, noofattempts, markspercentage,grade, isscholarshipsapplicable, scholarshiporprize, scholarshipname FROM tabapplicationeducationloanqualificationdetails where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationQualifcationDTO.Add(new EducationQualifcationDTO
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
                                    pTypeofoperation = "OLD"
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,totalfundrequirement, nonrepayablescholarship, repayablescholarship,fundsavailablefromfamily FROM tabapplicationeducationloanfeedetails where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Add(new EducationLoanFeeDetailsDTO
                                {
                                    ptotalfundrequirement = Convert.ToDecimal(dr["totalfundrequirement"]),
                                    pnonrepayablescholarship = dr["nonrepayablescholarship"].ToString(),
                                    prepayablescholarship = dr["repayablescholarship"].ToString(),
                                    pfundsavailablefromfamily = Convert.ToDecimal(dr["fundsavailablefromfamily"])
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,year, qualification, fee FROM tabapplicationeducationloanyearwisefeedetails where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Add(new EducationLoanyearwiseFeedetailsDTO
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pyear = dr["year"].ToString(),
                                    pqualification = dr["qualification"].ToString(),
                                    pfee = Convert.ToDecimal(dr["fee"]),
                                    pTypeofoperation = "OLD"

                                });
                            }
                        }
                    }
                    if (LoanType == "LOAN AGAINST DEPOSITS")
                    {
                        ApplicationLoanSpecificDTO.lstLoanagainstDepositDTO = new List<LoanagainstDepositDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,deposittype, bankcreditfacility, depositaccountnumber, depositamount,depositinterestpercentage, depositdate, deposittenure, depositdocpath,filename FROM tabapplicationdepositloan where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "' and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.lstLoanagainstDepositDTO.Add(new LoanagainstDepositDTO
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
                                    pTypeofoperation = "OLD"

                                });
                            }
                        }

                    }
                    if (LoanType == "BALANCE TRANSFER")
                    {
                        ApplicationLoanSpecificDTO.BalanceTransferDTO = new BalanceTransferDTO();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,bankorcreditfacilityname, loandate, loanaccountno, loanamount,outstandingdate, installmentamount, loanpayin, totaltenureofloan, balancetenureofloan, loansanctiondocpath, emichartdocpath FROM tblapplicationbalancetransfer where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.pbankorcreditfacilityname = dr["bankorcreditfacilityname"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ploandate = dr["loandate"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ploanaccountno = dr["loanaccountno"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ploanamount = Convert.ToDecimal(dr["loanamount"]);
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.poutstandingdate = dr["outstandingdate"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.pinstallmentamount = Convert.ToDecimal(dr["installmentamount"]);
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ploanpayin = dr["loanpayin"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ptotaltenureofloan = Convert.ToInt64(dr["totaltenureofloan"]);
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.pbalancetenureofloan = Convert.ToInt64(dr["balancetenureofloan"]);
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.ploansanctiondocpath = dr["loansanctiondocpath"].ToString();
                                ApplicationLoanSpecificDTO.BalanceTransferDTO.pemichartdocpath = dr["emichartdocpath"].ToString();
                            }
                        }

                    }
                    if (LoanType == "HOME LOAN")
                    {
                        ApplicationLoanSpecificDTO.HomeLoanDTO = new HomeLoanDTO();
                        // ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO = new BalanceTransferDTO();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,initialpayment, propertylocation, propertyownershiptype, propertytype, purpose, propertystatus, address1, address2, city, state, district,country, pincode, buildertieup, projectname, ownername, selleraddress,buildingname, blockname, builtupareain, plotarea, undividedshare, plintharea, bookingdate, completiondate, occupancycertificatedate, actualcost, saleagreementvalue, stampdutycharges, otheramenitiescharges, otherincidentalexpenditure, totalvalueofproperty, ageofbuilding,originalcostofproperty, estimatedvalueofrepairs, amountalreadyspent,otherborrowings, totalvalue,districtid,stateid,countryid FROM tabapplicationhomeloan where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pinitialpayment = Convert.ToDecimal(dr["initialpayment"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppropertylocation = dr["propertylocation"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppropertyownershiptype = dr["propertyownershiptype"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppropertytype = dr["propertytype"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppurpose = dr["purpose"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppropertystatus = dr["propertystatus"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.paddress1 = dr["address1"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.paddress2 = dr["address2"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pcity = dr["city"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pstate = dr["state"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pdistrict = dr["district"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pcountry = dr["country"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ppincode = dr["pincode"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pbuildertieup = dr["buildertieup"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pprojectname = dr["projectname"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pownername = dr["ownername"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pselleraddress = dr["selleraddress"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pbuildingname = dr["buildingname"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pblockname = dr["blockname"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pbuiltupareain = Convert.ToDecimal(dr["builtupareain"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pplotarea = Convert.ToDecimal(dr["plotarea"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pundividedshare = Convert.ToDecimal(dr["undividedshare"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pplintharea = Convert.ToDecimal(dr["plintharea"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pbookingdate = dr["bookingdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["bookingdate"]).ToString("dd/MM/yyyy");
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pcompletiondate = dr["completiondate"] == DBNull.Value ? null : Convert.ToDateTime(dr["completiondate"]).ToString("dd/MM/yyyy");
                                ApplicationLoanSpecificDTO.HomeLoanDTO.poccupancycertificatedate = dr["occupancycertificatedate"] == DBNull.Value ? null : Convert.ToDateTime(dr["occupancycertificatedate"]).ToString("dd/MM/yyyy");
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pactualcost = Convert.ToDecimal(dr["actualcost"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.psaleagreementvalue = Convert.ToDecimal(dr["saleagreementvalue"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pstampdutycharges = Convert.ToDecimal(dr["stampdutycharges"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.potheramenitiescharges = Convert.ToDecimal(dr["otheramenitiescharges"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.potherincidentalexpenditure = Convert.ToDecimal(dr["otherincidentalexpenditure"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ptotalvalueofproperty = Convert.ToDecimal(dr["totalvalueofproperty"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pageofbuilding = dr["ageofbuilding"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.poriginalcostofproperty = Convert.ToDecimal(dr["originalcostofproperty"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pestimatedvalueofrepairs = Convert.ToDecimal(dr["estimatedvalueofrepairs"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pamountalreadyspent = Convert.ToDecimal(dr["amountalreadyspent"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.potherborrowings = Convert.ToDecimal(dr["otherborrowings"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.ptotalvalue = Convert.ToDecimal(dr["totalvalue"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pDistrictId = Convert.ToInt64(dr["districtid"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pStateId =Convert.ToInt64(dr["stateid"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.pCountryId =Convert.ToInt64(dr["countryid"]);
                            }
                        }

                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,bankorcreditfacilityname, loandate, loanaccountno, loanamount,outstandingdate, installmentamount, loanpayin, totaltenureofloan, balancetenureofloan, loansanctiondocpath, emichartdocpath FROM tblapplicationbalancetransfer where applicationid=" + applicationid + " and vchapplicationid='" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.pbankorcreditfacilityname = dr["bankorcreditfacilityname"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ploandate = dr["loandate"] == DBNull.Value ? null : Convert.ToDateTime(dr["loandate"]).ToString("dd/MM/yyyy");

                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ploanaccountno = dr["loanaccountno"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ploanamount = Convert.ToDecimal(dr["loanamount"]);

                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.poutstandingdate = dr["outstandingdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["outstandingdate"]).ToString("dd/MM/yyyy");
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.pinstallmentamount = Convert.ToDecimal(dr["installmentamount"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ploanpayin = dr["loanpayin"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ptotaltenureofloan = Convert.ToInt64(dr["totaltenureofloan"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.pbalancetenureofloan = Convert.ToInt64(dr["balancetenureofloan"]);
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.ploansanctiondocpath = dr["loansanctiondocpath"].ToString();
                                ApplicationLoanSpecificDTO.HomeLoanDTO.BalanceTransferDTO.pemichartdocpath = dr["emichartdocpath"].ToString();
                            }
                        }


                    }
                    if (LoanType == "BUSINESS LOAN")
                    {
                        ApplicationLoanSpecificDTO.BusinessLoanDTO = new BusinessLoanDTO();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessfinancialperformanceDTO = new List<BusinessfinancialperformanceDTO>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO = new List<BusinesscredittrendpurchasesDTO>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscredittrendsalesDTO = new List<BusinesscredittrendsalesDTO>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessstockpositionDTO = new List<BusinessstockpositionDTO>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscostofprojectDTO = new List<BusinesscostofprojectDTO>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO = new BusinessancillaryunitaddressdetailsDTO();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessloanassociateconcerndetails = new List<Businessloanassociateconcerndetails>();
                        ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss = new List<Businessloanturnoverandprofitorloss>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,descriptionoftheactivity, isfinancialperformanceapplicable, iscredittrendforpurchasesapplicable,iscredittrendforsalesapplicable, isstockpositionapplicable, iscostofprojectapplicable,isancillaryunit, associateconcernsexist FROM tblapplicationbusinessloan where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.pdescriptionoftheactivity = dr["descriptionoftheactivity"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.pisfinancialperformanceapplicable = Convert.ToBoolean(dr["isfinancialperformanceapplicable"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.piscredittrendforpurchasesapplicable = Convert.ToBoolean(dr["iscredittrendforpurchasesapplicable"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.piscredittrendforsalesapplicable = Convert.ToBoolean(dr["iscredittrendforsalesapplicable"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.pisstockpositionapplicable = Convert.ToBoolean(dr["isstockpositionapplicable"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.piscostofprojectapplicable = Convert.ToBoolean(dr["iscostofprojectapplicable"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.pisancillaryunit = Convert.ToBoolean(dr["isancillaryunit"].ToString());
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.passociateconcernsexist = Convert.ToBoolean(dr["associateconcernsexist"].ToString());
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath FROM tblapplicationbusinessfinancialperformance where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Add(new BusinessfinancialperformanceDTO
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pturnoveramount = Convert.ToDecimal(dr["turnoveramount"]),
                                    pnetprofitamount = Convert.ToDecimal(dr["netprofitamount"]),
                                    pbalancesheetdocpath = dr["balancesheetdocpath"].ToString(),
                                    pTypeofoperation = "OLD"
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,finacialyear, suppliername, address, contactno, maxcreditreceived,mincreditreceived, avgtotalcreditreceived FROM tblapplicationbusinesscredittrendpurchases where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Add(new BusinesscredittrendpurchasesDTO
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    psuppliername = dr["suppliername"].ToString(),
                                    paddress = dr["address"].ToString(),
                                    pcontactno = dr["contactno"].ToString(),
                                    pmaxcreditreceived = Convert.ToDecimal(dr["maxcreditreceived"]),
                                    pmincreditreceived = Convert.ToDecimal(dr["mincreditreceived"]),
                                    pavgtotalcreditreceived = Convert.ToDecimal(dr["avgtotalcreditreceived"]),
                                    pTypeofoperation = "OLD"
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales FROM tblapplicationbusinesscredittrendsales where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Add(new BusinesscredittrendsalesDTO
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pcustomername = dr["customername"].ToString(),
                                    paddress = dr["address"].ToString(),
                                    pcontactno = dr["contactno"].ToString(),
                                    pmaxcreditgiven = Convert.ToDecimal(dr["maxcreditgiven"]),
                                    pmincreditgiven = Convert.ToDecimal(dr["mincreditgiven"]),
                                    ptotalcreditsales = Convert.ToDecimal(dr["totalcreditsales"]),
                                    pTypeofoperation = "OLD"



                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,finacialyear, maxstockcarried, minstockcarried, avgtotalstockcarried FROM tblapplicationbusinessstockposition where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessstockpositionDTO.Add(new BusinessstockpositionDTO
                                {

                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pfinacialyear = dr["finacialyear"].ToString(),
                                    pmaxstockcarried = Convert.ToDecimal(dr["maxstockcarried"]),
                                    pminstockcarried = Convert.ToDecimal(dr["minstockcarried"]),
                                    pavgtotalstockcarried = Convert.ToDecimal(dr["avgtotalstockcarried"]),
                                    pTypeofoperation = "OLD"

                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,costoflandincludingdevelopment, buildingandothercivilworks, plantandmachinery,equipmenttools, testingequipment, miscfixedassets, erectionorinstallationcharges,preliminaryorpreoperativeexpenses, provisionforcontingencies,marginforworkingcapital FROM tblapplicationbusinesscostofproject where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "';"))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinesscostofprojectDTO.Add(new BusinesscostofprojectDTO
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
                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, " SELECT recordid,address1, address2, city, country, state, district, pincode,coalesce(stateid,0) as stateid,coalesce(districtid,0) as districtid,coalesce(countryid,0) as countryid FROM tabapplicationbusinessloanancillaryunitaddressdetails where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "'; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress1 = dr["address1"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress2 = dr["address2"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcity = dr["city"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pCountry = dr["country"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid = Convert.ToInt64(dr["countryid"]);
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pState = dr["state"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid = Convert.ToInt64(dr["stateid"]);
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pDistrict = dr["district"].ToString();
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid = Convert.ToInt64(dr["districtid"]);
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pPincode = dr["pincode"].ToString();
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,nameofassociateconcern, natureofassociation, natureofactivity,itemstradedormanufactured FROM tabapplicationbusinessloanassociateconcerndetails where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "' AND STATUSID=1; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Add(new Businessloanassociateconcerndetails
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pnameofassociateconcern = dr["nameofassociateconcern"].ToString(),
                                    pnatureofassociation = dr["natureofassociation"].ToString(),
                                    pnatureofactivity = dr["natureofactivity"].ToString(),
                                    pitemstradedormanufactured = dr["itemstradedormanufactured"].ToString(),
                                    pTypeofoperation = "OLD"

                                });
                            }
                        }
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,turnoveryear, turnoveramount, turnoverprofit FROM tabapplicationbusinessloanturnoverandprofitorloss where applicationid = " + applicationid + " and vchapplicationid = '" + strapplictionid + "' AND STATUSID=1; "))
                        {
                            while (dr.Read())
                            {
                                ApplicationLoanSpecificDTO.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Add(new Businessloanturnoverandprofitorloss
                                {
                                    pRecordid = Convert.ToInt64(dr["recordid"]),
                                    pturnoveryear = dr["turnoveryear"].ToString(),
                                    pturnoveramount = Convert.ToDecimal(dr["turnoveramount"]),
                                    pturnoverprofit = Convert.ToDecimal(dr["turnoverprofit"]),
                                    pTypeofoperation = "OLD"
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

            return ApplicationLoanSpecificDTO;

        }
        #endregion

        //FirstinformationDTO IFirstinformation.GetApplicantDetails(long applicationid, string strapplictionid, string ConnectionString)
        //{
        //    throw new NotImplementedException();
        //}

        #region ApplicantExisting Loans
        public bool SaveApplicationexistingloansDetails(ExistingLoanDetailsDTO objApplicationExistingLoanDetails, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sb = new StringBuilder();
            string Recordid = string.Empty;
            string applicationId = string.Empty;
            string query = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sb.Append("UPDATE tabapplication set isexistingloansapplicable= " + objApplicationExistingLoanDetails.pIsexistingloansapplicable + " where vchapplicationid = '" + objApplicationExistingLoanDetails.pVchapplicationid + "';");


                if (objApplicationExistingLoanDetails.pIsexistingloansapplicable)
                {
                    if (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails.Count > 0)
                    {
                        for (int i = 0; i < objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails.Count; i++)
                        {

                            if (string.IsNullOrEmpty(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate))
                            {
                                objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate = "null";
                            }
                            else
                            {
                                objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate = "'" + FormatDate(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate) + "'";
                            }
                            if (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ptypeofoperation.ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pRecordid.ToString();
                                }
                            }
                            if (objApplicationExistingLoanDetails.pVchapplicationid != null)
                            {
                                applicationId = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + objApplicationExistingLoanDetails.pVchapplicationid + "';"));
                            }



                            if (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                sb.Append("INSERT INTO tabapplicationexistingloans(applicationid, vchapplicationid, contactid, contactreferenceid, typeoflender, bankorcreditfacilityname, loanno, loanname, loansanctiondate, tenureofloan, rateofinterest, loanpayin, instalmentamount, loanamount, principleoutstanding,remainingtenureofloan, statusid, createdby, createddate,LoanSanctionDocumentpath,LoanSanctionDocumentfilename,EmichartDocumentpath ,EmichartDocumentfilename,isbalancetransferapplicable)VALUES (" + applicationId + ",'" + ManageQuote(objApplicationExistingLoanDetails.pVchapplicationid)
                                    + "'," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pContactid + ",'" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pContactreferenceid) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pTypeofLender) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pbankorcreditfacilityname) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanno) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanname) + "'," + (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate) + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pTenureofloan + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pRateofinterest + ",'" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanpayin) + "'," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pInstalmentamount + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ploanamount + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pPrincipleoutstanding + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].premainingTenureofloan + "," + Convert.ToInt32(Status.Active) + "," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pCreatedby + ",current_timestamp,'" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanSanctionDocumentpath) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanSanctionDocumentfilename) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pEmichartDocumentpath) + "','" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pEmichartDocumentfilename) + "'," + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pisbalancetransferapplicable + ");");
                            }
                            else if (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ptypeofoperation.ToUpper() == "UPDATE" || objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ptypeofoperation.ToUpper() == "OLD")
                            {
                                sb.Append("UPDATE tabapplicationexistingloans SET applicationid=" + applicationId + ", vchapplicationid='" + ManageQuote(objApplicationExistingLoanDetails.pVchapplicationid)
                                    + "', contactid=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pContactid + ", contactreferenceid='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pContactreferenceid) + "', typeoflender='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pTypeofLender) + "', bankorcreditfacilityname='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pbankorcreditfacilityname) + "', loanno='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanno) + "', loanname='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanname) + "', loansanctiondate=" + (objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoansanctiondate) + ", tenureofloan=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pTenureofloan + ", rateofinterest=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pRateofinterest + ", loanpayin='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanpayin) + "', instalmentamount=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pInstalmentamount + ", loanamount=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].ploanamount + ", principleoutstanding=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pPrincipleoutstanding + ", modifiedby=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pCreatedby + ", modifieddate=current_timestamp, remainingtenureofloan=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].premainingTenureofloan + ",LoanSanctionDocumentpath='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanSanctionDocumentpath) + "',LoanSanctionDocumentfilename='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pLoanSanctionDocumentfilename) + "',EmichartDocumentpath='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pEmichartDocumentpath) + "' ,EmichartDocumentfilename='" + ManageQuote(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pEmichartDocumentfilename) + "',isbalancetransferapplicable=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pisbalancetransferapplicable + " WHERE recordid=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[i].pRecordid + "; ");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        query = "UPDATE tabapplicationexistingloans set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[0].pCreatedby + ",modifieddate=current_timestamp where applicationid=" + applicationId + " and recordid not in (" + Recordid + ") AND statusid<>2;";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails)))
                        {
                            query = "UPDATE tabapplicationexistingloans set statusid=" + getStatusid("In-Active", ConnectionString) + ",modifiedby=" + objApplicationExistingLoanDetails.lstApplicationExistingLoanDetails[0].pCreatedby + ",modifieddate=current_timestamp where applicationid=" + applicationId + " AND statusid<>2;";
                        }
                    }
                }
                if (sb.Length > 0 || (!string.IsNullOrEmpty(query)))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, query + "" + sb.ToString());
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
                con.Dispose();
                con.Close();
                con.ClearPool();
                trans.Dispose();
            }
            return isSaved;
        }
        public ExistingLoanDetailsDTO GetApplicationExistingLoanDetails(string contactreferenceid, string vchapplicationid, string con)
        {
            ExistingLoanDetailsDTO = new ExistingLoanDetailsDTO();
            string Query = string.Empty;
            int count = 0;
            bool isexistingloansapplicable = false;
            try
            {
                if (vchapplicationid != null)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tabapplicationexistingloans where vchapplicationid = '" + vchapplicationid + "';"));
                }
                isexistingloansapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select isexistingloansapplicable from tabapplication where vchapplicationid = '" + ManageQuote(vchapplicationid) + "';"));
                if (count > 0)
                {
                    Query = "select  recordid,contactid as applicantid,contactreferenceid,typeoflender,  bankorcreditfacilityname, loanno,loanname,tenureofloan, rateofinterest,loanpayin,instalmentamount,loanamount,loansanctiondate,coalesce(remainingtenureofloan,0) as remainingtenureofloan, coalesce(principleoutstanding,0) as principleoutstanding,'OLD' as typeofoperation,ts.statusname, LoanSanctionDocumentpath,LoanSanctionDocumentfilename,EmichartDocumentpath ,EmichartDocumentfilename,isbalancetransferapplicable from tabapplicationexistingloans t1 join tblmststatus ts on t1.statusid=ts.statusid where vchapplicationid='" + vchapplicationid + "' and upper(ts.statusname) = 'ACTIVE'";
                }
                else
                {
                    //Query = "select  0 as recordid,applicantid,contactreferenceid,'company' as typeoflender,  null as bankorcreditfacilityname, vchapplicationid as loanno, loanname,tenureofloan, rateofinterest,loanpayin,instalmentamount,coalesce(amountrequested,0) as loanamount,dateofapplication as loansanctiondate, coalesce(tenureofloan,0) as remainingtenureofloan, amountrequested as principleoutstanding,'CREATE' as typeofoperation,ts.statusname from tabapplication t1 join tblmststatus ts on t1.statusid=ts.statusid where contactreferenceid='" + contactreferenceid + "' and upper(ts.statusname) = 'ACTIVE'";

                    Query = "select  0 as recordid,applicantid,contactreferenceid,'company' as typeoflender,  'Internal' as bankorcreditfacilityname, t1.vchapplicationid as loanno, ta.loanname,t1.tenureofloan, t1.rateofinterest,t1.loanpayin,t1.installmentamount as instalmentamount,coalesce(t1.amountrequested, 0) as loanamount,dateofapplication as loansanctiondate, coalesce(t1.tenureofloan, 0) as remainingtenureofloan, t1.amountrequested as principleoutstanding,'CREATE' as typeofoperation,ts.statusname,'' as LoanSanctionDocumentpath,'' as  LoanSanctionDocumentfilename,'' as  EmichartDocumentpath ,'' as  EmichartDocumentfilename,false as isbalancetransferapplicable from tbltransapprovedapplications t1  join tabapplication ta on ta.vchapplicationid = t1.vchapplicationid join tblmststatus ts on ta.loanstatusid = ts.statusid where contactreferenceid =  '" + contactreferenceid + "' and upper(ts.statusname) in('DISBURSED');";

                }
                ExistingLoanDetailsDTO.pIsexistingloansapplicable = isexistingloansapplicable;
                ExistingLoanDetailsDTO.lstApplicationExistingLoanDetails = new List<ApplicationExistingLoanDetailsDTO>();
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(con, CommandType.Text, Query))
                {

                    while (dr.Read())
                    {
                        ExistingLoanDetailsDTO.lstApplicationExistingLoanDetails.Add(new ApplicationExistingLoanDetailsDTO
                        {
                            pRecordid = Convert.ToInt64(dr["recordid"]),
                            //obj.pApplicationid = Convert.ToInt64(dr["applicationid"]),
                            pContactid = Convert.ToInt64(dr["applicantid"]),
                            pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pbankorcreditfacilityname = Convert.ToString(dr["bankorcreditfacilityname"]),
                            pLoanno = Convert.ToString(dr["loanno"]),
                            pLoanname = Convert.ToString(dr["loanname"]),
                            pTenureofloan = Convert.ToInt64(dr["tenureofloan"]),
                            pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]),
                            pLoanpayin = Convert.ToString(dr["loanpayin"]),
                            pInstalmentamount = Convert.ToDecimal(dr["instalmentamount"]),
                            ploanamount = Convert.ToDecimal(dr["loanamount"]),
                            pLoansanctiondate = Convert.ToString(dr["loansanctiondate"]),
                            premainingTenureofloan = Convert.ToInt64(dr["remainingtenureofloan"]),
                            pPrincipleoutstanding = Convert.ToInt64(dr["principleoutstanding"]),
                            ptypeofoperation = Convert.ToString(dr["typeofoperation"]),
                            pTypeofLender = Convert.ToString(dr["typeoflender"]),
                            pStatusname = Convert.ToString(dr["statusname"]),
                            pLoanSanctionDocumentpath = Convert.ToString(dr["LoanSanctionDocumentpath"]),
                            pLoanSanctionDocumentfilename = Convert.ToString(dr["LoanSanctionDocumentfilename"]),
                            pEmichartDocumentpath = Convert.ToString(dr["EmichartDocumentpath"]),
                            pEmichartDocumentfilename = Convert.ToString(dr["EmichartDocumentfilename"]),
                            pisbalancetransferapplicable = Convert.ToBoolean(dr["isbalancetransferapplicable"])
                        });
                    }
                }
                if (ExistingLoanDetailsDTO.lstApplicationExistingLoanDetails.Count > 0)
                {
                    ExistingLoanDetailsDTO.pIsexistingloansapplicable = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ExistingLoanDetailsDTO;
        }
        #endregion

        #region Save Education Loan Masters
        public bool SaveEducationalFeeQualification(EducationalFeeQualificationDTO EducationalFeeQualificationDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {

                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmsteducationalfeequalification(educationalqualification,statusid,createdby,createddate) values('" + ManageQuote(EducationalFeeQualificationDTO.peducationalfeequalification) + "'," + EducationalFeeQualificationDTO.pStatusid + "," + EducationalFeeQualificationDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }

        public bool SaveEducationalFeeYear(EducationalFeeYearDTO EducationalFeeYearDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmsteducationalfeeyear(year,statusid,createdby,createddate) values('" + ManageQuote(EducationalFeeYearDTO.pYear) + "'," + EducationalFeeYearDTO.pStatusid + "," + EducationalFeeYearDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }

        public bool SaveEducationalQualification(EducationalQualificationDTO EducationalQualificationDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmsteducationalqualification(educationalqualification,statusid,createdby,createddate) values('" + ManageQuote(EducationalQualificationDTO.peducationalqualification) + "'," + EducationalQualificationDTO.pStatusid + "," + EducationalQualificationDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }

        public List<EducationalFeeQualificationDTO> BindEducationalFeeQualification(string ConnectionString)
        {
            lstEducationalFeeQualificationDTO = new List<EducationalFeeQualificationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,educationalqualification from tblmsteducationalfeequalification where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        EducationalFeeQualificationDTO EducationalFeeQualificationDTO = new EducationalFeeQualificationDTO();
                        EducationalFeeQualificationDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        EducationalFeeQualificationDTO.peducationalfeequalification = dr["educationalqualification"].ToString();
                        lstEducationalFeeQualificationDTO.Add(EducationalFeeQualificationDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lstEducationalFeeQualificationDTO;

        }

        public List<EducationalFeeYearDTO> BindEducationalFeeYear(string ConnectionString)
        {
            lstEducationalFeeYearDTO = new List<EducationalFeeYearDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,year from tblmsteducationalfeeyear where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        EducationalFeeYearDTO EducationalFeeYearDTO = new EducationalFeeYearDTO();
                        EducationalFeeYearDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        EducationalFeeYearDTO.pYear = dr["year"].ToString();
                        lstEducationalFeeYearDTO.Add(EducationalFeeYearDTO);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return lstEducationalFeeYearDTO;
        }

        public List<EducationalQualificationDTO> BindEducationalQualification(string ConnectionString)
        {
            lstEducationalQualificationDTO = new List<EducationalQualificationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,educationalqualification from tblmsteducationalqualification where statusid=1 order by recordid;"))
                {
                    while (dr.Read())
                    {
                        EducationalQualificationDTO EducationalQualificationDTO = new EducationalQualificationDTO();
                        EducationalQualificationDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        EducationalQualificationDTO.peducationalqualification = dr["educationalqualification"].ToString();
                        lstEducationalQualificationDTO.Add(EducationalQualificationDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstEducationalQualificationDTO;

        }
        public List<ApplicantIdsDTO> BindApplicantIds(string LoanType, string ConnectionString)
        {
            lstApplicantIdsDTO = new List<ApplicantIdsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select applicationid,vchapplicationid from tabapplication where upper(loantype)='" + LoanType.Trim().ToUpper() + "' and statusid=1 order by applicationid;"))
                {
                    while (dr.Read())
                    {
                        ApplicantIdsDTO ApplicantIdsDTO = new ApplicantIdsDTO();
                        ApplicantIdsDTO.pApplicantID = Convert.ToInt64(dr["applicationid"]);
                        ApplicantIdsDTO.pApplicationID = dr["vchapplicationid"].ToString();
                        lstApplicantIdsDTO.Add(ApplicantIdsDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstApplicantIdsDTO;
        }
        public bool SaveLoanSpecificDetails(string strapplictionid, ApplicationLoanSpecificDTO Applicationlist, string ConnectionString)
        {
            ds = new DataSet();
            StringBuilder sbinsert = new StringBuilder();
            StringBuilder sbUPDATEGRID = new StringBuilder();
            string Recordid = string.Empty;
            string recordid1 = string.Empty;
            string recordid2 = string.Empty;
            string recordid3 = string.Empty;
            string recordid4 = string.Empty;
            string recordid5 = string.Empty;
            string query = string.Empty;
            bool IsSaved = false;
            long applicationid = 0;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + ManageQuote(strapplictionid) + "';"));
                if (Applicationlist.pLoantype.ToUpper().Trim() == "VEHICLE LOAN")
                {
                    if (Applicationlist.lstVehicleLoanDTO.Count > 0)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationvehicleloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        for (int i = 0; i < Applicationlist.lstVehicleLoanDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pDownPayment.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pDownPayment = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pOnroadprice.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pOnroadprice = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount.ToString()))
                            {
                                Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount = 0;
                            }

                            if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                            {
                                sbinsert.Append("update tabapplicationvehicleloan set showroomname='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pShowroomName) + "',vehiclemanufacture='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleManufacturer) + "',vehiclemodel='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleModel) + "',actualvehiclecost=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle + ",0),downpayment=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0),onroadprice=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pOnroadprice + ",0),requestedamount=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount + ",0),engineno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pEngineNo) + "',chasisno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pChassisNo) + "',registrationno='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pRegistrationNo) + "',yearofmake='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pYearofMake) + "',remarks='" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pAnyotherRemarks) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");

                                sbinsert.Append("update tabapplication set downpayment=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0) where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationvehicleloan(applicationid,vchapplicationid,contactid,contactreferenceid,showroomname,vehiclemanufacture,vehiclemodel,actualvehiclecost,downpayment,onroadprice,requestedamount,engineno,chasisno,registrationno,yearofmake,remarks,statusid,createdby,createddate) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pShowroomName) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleManufacturer) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pVehicleModel) + "',coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pActualcostofVehicle + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pOnroadprice + ",0),coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pRequestedLoanAmount + ",0),'" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pEngineNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pChassisNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pRegistrationNo) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pYearofMake) + "','" + ManageQuote(Applicationlist.lstVehicleLoanDTO[i].pAnyotherRemarks) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");

                                sbinsert.Append("update tabapplication set downpayment=coalesce(" + Applicationlist.lstVehicleLoanDTO[i].pDownPayment + ",0) where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }
                        }
                    }
                }

                if (Applicationlist.pLoantype.ToUpper().Trim() == "CONSUMER LOAN")
                {
                    if (Applicationlist.ConsumerLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationconsumerloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";SELECT COUNT(*) FROM tblapplicationconsumerloanproductdetails WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND STATUSID=" + Convert.ToInt32(Status.Active) + ";");
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.ptotalproductcost.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.ptotalproductcost = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.pdownpayment.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.pdownpayment = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.pbalanceamount.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.pbalanceamount = 0;
                            }
                            sbinsert.Append("UPDATE tblapplicationconsumerloan SET totalproductcost=coalesce(" + Applicationlist.ConsumerLoanDTO.ptotalproductcost + ", 0),downpayment=coalesce(" + Applicationlist.ConsumerLoanDTO.pdownpayment + ", 0),balanceamount=coalesce(" + Applicationlist.ConsumerLoanDTO.pbalanceamount + ", 0),modifiedby=" + Applicationlist.pCreatedby + ", modifieddate=current_timestamp WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            sbinsert.Append("UPDATE tabapplication SET downpayment=coalesce(" + Applicationlist.ConsumerLoanDTO.pdownpayment + ", 0)WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.ptotalproductcost.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.ptotalproductcost = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.pdownpayment.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.pdownpayment = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.pbalanceamount.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.pbalanceamount = 0;
                            }
                            sbinsert.Append("INSERT INTO tblapplicationconsumerloan(applicationid, vchapplicationid, contactid, contactreferenceid,totalproductcost, downpayment, balanceamount, statusid, createdby, createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.ConsumerLoanDTO.ptotalproductcost + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.pdownpayment + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.pbalanceamount + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                            sbinsert.Append("UPDATE tabapplication SET downpayment=coalesce(" + Applicationlist.ConsumerLoanDTO.pdownpayment + ", 0)WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                        }



                        for (int i = 0; i < Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Count; i++)
                        {
                            if (Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pRecordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pquantity.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pquantity = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pcostofproduct.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pcostofproduct = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pinsurancecostoftheproduct.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pinsurancecostoftheproduct = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pothercost.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pothercost = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].ptotalcostofproduct.ToString()))
                            {
                                Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].ptotalcostofproduct = 0;
                            }
                            if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                            {

                                if (Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationconsumerloanproductdetails(applicationid, vchapplicationid, contactid, contactreferenceid, producttype, productname, manufacturer, productmodel, quantity, costofproduct, insurancecostoftheproduct, othercost, totalcostofproduct, iswarrantyapplicable, period,periodtype, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproducttype) + "','" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductname) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pmanufacturer) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductmodel) + "', coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pquantity + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pcostofproduct + ", 0),coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pinsurancecostoftheproduct + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pothercost + ", 0),coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].ptotalcostofproduct + ", 0), " + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].piswarrantyapplicable + ",'" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiod) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiodtype) + "'," + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                }
                                if (Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {
                                    sbinsert.Append("UPDATE tblapplicationconsumerloanproductdetails SET  producttype = '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproducttype) + "', productname = '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductname) + "', manufacturer = '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pmanufacturer) + "',productmodel = '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductmodel) + "', quantity = coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pquantity + ", 0), costofproduct = coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pcostofproduct + ", 0), insurancecostoftheproduct = coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pinsurancecostoftheproduct + ", 0),othercost = coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pothercost + ", 0), totalcostofproduct = coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].ptotalcostofproduct + ", 0), iswarrantyapplicable = " + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].piswarrantyapplicable + ", period = '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiod) + "',periodtype='" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiodtype) + "', modifiedby =" + Applicationlist.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pRecordid + "; ");
                                }
                                //if (Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                //{
                                //    sbinsert.Append("UPDATE tblapplicationconsumerloanproductdetails SET  STATUSID="+Convert.ToInt32(Status.Inactive)+", modifiedby = 1, modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pRecordid + "; ");
                                //}
                            }
                            else
                            {
                                sbinsert.Append("INSERT INTO tblapplicationconsumerloanproductdetails(applicationid, vchapplicationid, contactid, contactreferenceid, producttype, productname, manufacturer, productmodel, quantity, costofproduct, insurancecostoftheproduct, othercost, totalcostofproduct, iswarrantyapplicable, period,periodtype, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproducttype) + "','" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductname) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pmanufacturer) + "', '" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pproductmodel) + "', coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pquantity + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pcostofproduct + ", 0),coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pinsurancecostoftheproduct + ", 0), coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pothercost + ", 0),coalesce(" + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].ptotalcostofproduct + ", 0), " + Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].piswarrantyapplicable + ",'" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiod) + "','" + ManageQuote(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO[i].pperiodtype) + "', " + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ", current_timestamp);");
                            }

                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbUPDATEGRID.Append("UPDATE tblapplicationconsumerloanproductdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + Applicationlist.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID not in(" + Recordid + "); ");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.ToString()) || Applicationlist.ConsumerLoanDTO.lstConsumerLoanDetailsDTO.Count == 0)
                            {

                                sbUPDATEGRID.Append("UPDATE tblapplicationconsumerloanproductdetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ", modifiedby =" + Applicationlist.pCreatedby + ", modifieddate = CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "'; ");
                            }
                        }
                    }

                }



                if (Applicationlist.pLoantype.ToUpper().Trim() == "GOLD LOAN")
                {
                    if (Applicationlist.lstGoldLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationgoldloan where  vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";SELECT COUNT(*) FROM tabapplicationgoldloandetails WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND STATUSID=" + Convert.ToInt32(Status.Active) + ";");

                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pAppraisalDate))
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "null";
                            }
                            else
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "'" + FormatDate(Applicationlist.lstGoldLoanDTO.pAppraisalDate) + "'";
                            }
                            sbinsert.Append("update tblapplicationgoldloan set TotalAppraisedValue=coalesce(" + Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue + ", 0),AppraisalDate=" + Applicationlist.lstGoldLoanDTO.pAppraisalDate + ",AppraisorName='" + ManageQuote(Applicationlist.lstGoldLoanDTO.pAppraisorName) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=CURRENT_TIMESTAMP WHERE applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.pAppraisalDate))
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "null";
                            }
                            else
                            {
                                Applicationlist.lstGoldLoanDTO.pAppraisalDate = "'" + FormatDate(Applicationlist.lstGoldLoanDTO.pAppraisalDate) + "'";
                            }
                            query = "insert into tblapplicationgoldloan(applicationid,vchapplicationid,contactid,contactreferenceid,TotalAppraisedValue,AppraisalDate,AppraisorName,statusid,createdby,createddate) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.lstGoldLoanDTO.pTotalAppraisedValue + ", 0)," + Applicationlist.lstGoldLoanDTO.pAppraisalDate + ",'" + ManageQuote(Applicationlist.lstGoldLoanDTO.pAppraisorName) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp) returning recordid;";
                            Applicationlist.lstGoldLoanDTO.pRecordid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, query));
                        }


                        for (int i = 0; i < Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue.ToString()))
                            {
                                Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue = 0;
                            }
                            if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid.ToString();
                                }
                            }
                            if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                            {




                                if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {

                                    sbinsert.Append("UPDATE tabapplicationgoldloandetails SET goldarticletype='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "',detailsofarticle='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "',carat='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "',grossweight=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + ",netweight=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",AppraisedValueofArticle=coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),observations='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "',articledocpath='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "',docname='" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pUploadfilename) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=CURRENT_TIMESTAMP WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND recordid=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid + ";");
                                }
                                if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("insert into tabapplicationgoldloandetails(detailsid,vchapplicationid,goldarticletype,detailsofarticle,carat,grossweight,netweight,AppraisedValueofArticle,observations,articledocpath,statusid,createdby,createddate,docname) values(" + Applicationlist.lstGoldLoanDTO.pRecordid + ",'" + ManageQuote(strapplictionid) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "'," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + "," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp,'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pUploadfilename) + "');");
                                }
                                //if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                //{
                                //    sbinsert.Append("UPDATE tabapplicationgoldloandetails SET STATUSID=" + Convert.ToInt32(Status.Inactive) + " WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND recordid=" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pRecordid + ";");
                                //}
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationgoldloandetails(detailsid,vchapplicationid,goldarticletype,detailsofarticle,carat,grossweight,netweight,AppraisedValueofArticle,observations,articledocpath,statusid,createdby,createddate,docname) values(" + Applicationlist.lstGoldLoanDTO.pRecordid + ",'" + ManageQuote(strapplictionid) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticleType) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pDetailsofGoldArticle) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pCarat) + "'," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGrossweight + "," + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pNetWeight + ",coalesce(" + Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pAppraisedValue + ", 0),'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pobservations) + "','" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp,'" + ManageQuote(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pUploadfilename) + "');");
                            }

                        }

                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbUPDATEGRID.Append("UPDATE tabapplicationgoldloandetails SET STATUSID=" + Convert.ToInt32(Status.Inactive) + " WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "' AND  RECORDID not in(" + Recordid + ");");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO.ToString()) || Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationgoldloandetails SET STATUSID=" + Convert.ToInt32(Status.Inactive) + " WHERE vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "EDUCATION LOAN")
                {
                    if (Applicationlist.EducationLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationeducationloan where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationeducationloaninstituteaddress where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationeducationloanqualificationdetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationeducationloanfeedetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationeducationloanyearwisefeedetails where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.pDateofCommencement))
                        {
                            Applicationlist.EducationLoanDTO.pDateofCommencement = "null";
                        }
                        else
                        {
                            Applicationlist.EducationLoanDTO.pDateofCommencement = "'" + FormatDate(Applicationlist.EducationLoanDTO.pDateofCommencement) + "'";
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tabapplicationeducationloan set nameoftheinstitution='" + ManageQuote(Applicationlist.EducationLoanDTO.pNameoftheinstitution) + "',nameofproposedcourse='" + ManageQuote(Applicationlist.EducationLoanDTO.pNameofProposedcourse) + "',reasonforselectionoftheinstitute='" + ManageQuote(Applicationlist.EducationLoanDTO.pselectionoftheinstitute) + "',rankingofinstitution='" + ManageQuote(Applicationlist.EducationLoanDTO.pRankingofinstitution) + "',durationofcourse='" + ManageQuote(Applicationlist.EducationLoanDTO.pDurationofCourse) + "',dateofcommencement=" + Applicationlist.EducationLoanDTO.pDateofCommencement + ",reasonforseatsecured='" + ManageQuote(Applicationlist.EducationLoanDTO.pseatsecured) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                        }
                        else
                        {
                            sbinsert.Append("insert into tabapplicationeducationloan(applicationid,vchapplicationid,contactid,contactreferenceid,nameoftheinstitution,nameofproposedcourse,reasonforselectionoftheinstitute,rankingofinstitution,durationofcourse,dateofcommencement,reasonforseatsecured,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pNameoftheinstitution) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pNameofProposedcourse) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pselectionoftheinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pRankingofinstitution) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.pDurationofCourse) + "'," + Applicationlist.EducationLoanDTO.pDateofCommencement + ",'" + ManageQuote(Applicationlist.EducationLoanDTO.pseatsecured) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO.Count; i++)
                            {
                                if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tabapplicationeducationloaninstituteaddress set address1='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress1) + "',address2='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress2) + "',city='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCity) + "',state='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pState) + "',district='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrict) + "',country='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountry) + "',pincode='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pPincode) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp,stateid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pStateid) + ",districtid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrictid) + ",countryid=" + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountryid) + "  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloaninstituteaddress(applicationid,vchapplicationid,contactid,contactreferenceid,address1,address2,city,state,district,country,pincode,statusid,createdby,createddate,stateid,districtid,countryid) values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress1) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pAddress2) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCity) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pState) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrict) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountry) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pPincode) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pStateid) + "," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pDistrictid) + "," + (Applicationlist.EducationLoanDTO.lstEducationInstutiteAddressDTO[i].pCountryid) + ");");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.Count; i++)
                            {
                                if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (Convert.ToInt32(ds.Tables[2].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("insert into tabapplicationeducationloanqualificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,qualification,institute,yearofpassing,noofattempts,markspercentage,grade,isscholarshipsapplicable,scholarshiporprize,scholarshipname,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "','" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanqualificationdetails set qualification='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "',institute='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "',yearofpassing='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "',noofattempts='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "',markspercentage='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "',grade='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "',isscholarshipsapplicable='" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "',scholarshiporprize='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "',scholarshipname='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and recordid=" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tabapplicationeducationloanqualificationdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pRecordid + ";");
                                    //}
                                }
                                else
                                {

                                    sbinsert.Append("insert into tabapplicationeducationloanqualificationdetails(applicationid,vchapplicationid,contactid,contactreferenceid,qualification,institute,yearofpassing,noofattempts,markspercentage,grade,isscholarshipsapplicable,scholarshiporprize,scholarshipname,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pqualification) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pinstitute) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pyearofpassing) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pnoofattempts) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pmarkspercentage) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pgrade) + "','" + Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pisscholarshipsapplicable + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshiporprize) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO[i].pscholarshipname) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }

                            if (!string.IsNullOrEmpty(Recordid))
                            {
                                sbUPDATEGRID.Append("update tabapplicationeducationloanqualificationdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and recordid not in(" + Recordid + ");");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.ToString()) || Applicationlist.EducationLoanDTO.lstEducationQualifcationDTO.Count == 0)
                                {
                                    sbUPDATEGRID.Append("update tabapplicationeducationloanqualificationdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily = 0;
                                }

                                if (Convert.ToInt32(ds.Tables[3].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tabapplicationeducationloanfeedetails set totalfundrequirement=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement + ", 0),nonrepayablescholarship='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pnonrepayablescholarship) + "',repayablescholarship='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].prepayablescholarship) + "',fundsavailablefromfamily=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloanfeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,totalfundrequirement,nonrepayablescholarship,repayablescholarship,fundsavailablefromfamily,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].ptotalfundrequirement + ", 0),'" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pnonrepayablescholarship) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].prepayablescholarship) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanFeeDetailsDTO[i].pfundsavailablefromfamily + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count > 0)
                        {

                            for (int i = 0; i < Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count; i++)
                            {
                                if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid1))
                                    {
                                        recordid1 = Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid1 = recordid1 + "," + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee.ToString()))
                                {
                                    Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee = 0;
                                }

                                if (Convert.ToInt32(ds.Tables[4].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("insert into tabapplicationeducationloanyearwisefeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,year,qualification,fee,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                    }
                                    if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tabapplicationeducationloanyearwisefeedetails set year='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "',qualification='" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',fee=coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0),modifiedby=1,modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Applicationlist.pCreatedby + " and recordid=" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tabapplicationeducationloanyearwisefeedetails set statusid=" + Convert.ToInt32(Status.Active) + ",modifiedby=1,modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pRecordid + ";");
                                    //}
                                }
                                else
                                {
                                    sbinsert.Append("insert into tabapplicationeducationloanyearwisefeedetails(applicationid,vchapplicationid,contactid,contactreferenceid,year,qualification,fee,statusid,createdby,createddate)values(" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pyear) + "','" + ManageQuote(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pqualification) + "',coalesce(" + Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO[i].pfee + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }


                            if (!string.IsNullOrEmpty(recordid1))
                            {
                                sbUPDATEGRID.Append("update tabapplicationeducationloanyearwisefeedetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and recordid not in(" + recordid1 + ");");

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.ToString()) || Applicationlist.EducationLoanDTO.lstEducationLoanyearwiseFeedetailsDTO.Count == 0)
                                {
                                    sbUPDATEGRID.Append("update tabapplicationeducationloanyearwisefeedetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                }
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "LOAN AGAINST DEPOSITS")
                {
                    if (Applicationlist.lstLoanagainstDepositDTO.Count > 0)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationdepositloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        for (int i = 0; i < Applicationlist.lstLoanagainstDepositDTO.Count; i++)
                        {
                            if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = Applicationlist.lstLoanagainstDepositDTO[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + Applicationlist.lstLoanagainstDepositDTO[i].pRecordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount.ToString()))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage.ToString()))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate))
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate = "null";
                            }
                            else
                            {
                                Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate = "'" + FormatDate(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate) + "'";
                            }
                            if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                            {
                                if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                {
                                    sbinsert.Append("insert into tabapplicationdepositloan(applicationid,vchapplicationid,contactid,contactreferenceid,deposittype,bankcreditfacility,depositaccountnumber,depositamount,depositinterestpercentage,depositdate,deposittenure,depositdocpath,filename,statusid,createdby,createddate) values (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0)," + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",'" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pUploadfilename) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                                if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                {
                                    sbinsert.Append("update tabapplicationdepositloan set deposittype='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "',bankcreditfacility='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "',depositaccountnumber='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',depositamount=coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),depositinterestpercentage=coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0),depositdate=" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",deposittenure='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "',depositdocpath='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "',filename='" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pUploadfilename) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.lstLoanagainstDepositDTO[i].pRecordid + ";");
                                }
                                //if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                //{
                                //    sbinsert.Append("update tabapplicationdepositloan set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.lstLoanagainstDepositDTO[i].pRecordid + ";");
                                //}
                            }
                            else
                            {
                                sbinsert.Append("insert into tabapplicationdepositloan(applicationid,vchapplicationid,contactid,contactreferenceid,deposittype,bankcreditfacility,depositaccountnumber,depositamount,depositinterestpercentage,depositdate,deposittenure,depositdocpath,filename,statusid,createdby,createddate) values (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittype) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pbankcreditfacility) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositaccountnumber) + "',coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositamount + ", 0),coalesce(" + Applicationlist.lstLoanagainstDepositDTO[i].pdepositinterestpercentage + ", 0)," + Applicationlist.lstLoanagainstDepositDTO[i].pdepositdate + ",'" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdeposittenure) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath) + "','" + ManageQuote(Applicationlist.lstLoanagainstDepositDTO[i].pUploadfilename) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                            }
                        }

                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbUPDATEGRID.Append("update tabapplicationdepositloan set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and  recordid not in(" + Recordid + ");");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO.ToString()) || Applicationlist.lstLoanagainstDepositDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("update tabapplicationdepositloan set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                            }
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "HOME LOAN")
                {
                    if (Applicationlist.HomeLoanDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tabapplicationhomeloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pbookingdate))
                        {
                            Applicationlist.HomeLoanDTO.pbookingdate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.pbookingdate = "'" + FormatDate(Applicationlist.HomeLoanDTO.pbookingdate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pcompletiondate))
                        {
                            Applicationlist.HomeLoanDTO.pcompletiondate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.pcompletiondate = "'" + FormatDate(Applicationlist.HomeLoanDTO.pcompletiondate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.poccupancycertificatedate))
                        {
                            Applicationlist.HomeLoanDTO.poccupancycertificatedate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.poccupancycertificatedate = "'" + FormatDate(Applicationlist.HomeLoanDTO.poccupancycertificatedate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pinitialpayment.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pinitialpayment = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pbuiltupareain.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pbuiltupareain = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pplotarea.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pplotarea = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pundividedshare.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pundividedshare = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pplintharea.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pplintharea = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pactualcost.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pactualcost = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.psaleagreementvalue.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.psaleagreementvalue = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pstampdutycharges.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pstampdutycharges = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potheramenitiescharges.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potheramenitiescharges = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potherincidentalexpenditure.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potherincidentalexpenditure = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.ptotalvalueofproperty.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.ptotalvalueofproperty = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.poriginalcostofproperty.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.poriginalcostofproperty = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.pamountalreadyspent.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.pamountalreadyspent = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.potherborrowings.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.potherborrowings = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.ptotalvalue.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.ptotalvalue = 0;
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tabapplicationhomeloan set initialpayment=coalesce(" + Applicationlist.HomeLoanDTO.pinitialpayment + ", 0), propertylocation='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertylocation) + "', propertyownershiptype='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertyownershiptype) + "', propertytype='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertytype) + "',purpose='" + ManageQuote(Applicationlist.HomeLoanDTO.ppurpose) + "', propertystatus='" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertystatus) + "', address1='" + ManageQuote(Applicationlist.HomeLoanDTO.paddress1) + "', address2='" + ManageQuote(Applicationlist.HomeLoanDTO.paddress2) + "', city='" + ManageQuote(Applicationlist.HomeLoanDTO.pcity) + "', state='" + ManageQuote(Applicationlist.HomeLoanDTO.pstate) + "', district='" + ManageQuote(Applicationlist.HomeLoanDTO.pdistrict) + "',country='" + ManageQuote(Applicationlist.HomeLoanDTO.pcountry) + "', pincode='" + ManageQuote(Applicationlist.HomeLoanDTO.ppincode) + "', buildertieup='" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildertieup) + "', projectname='" + ManageQuote(Applicationlist.HomeLoanDTO.pprojectname) + "', ownername='" + ManageQuote(Applicationlist.HomeLoanDTO.pownername) + "', selleraddress='" + ManageQuote(Applicationlist.HomeLoanDTO.pselleraddress) + "',buildingname='" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildingname) + "', blockname='" + ManageQuote(Applicationlist.HomeLoanDTO.pblockname) + "', builtupareain=coalesce(" + Applicationlist.HomeLoanDTO.pbuiltupareain + ", 0), plotarea=coalesce(" + Applicationlist.HomeLoanDTO.pplotarea + ", 0), undividedshare=coalesce(" + Applicationlist.HomeLoanDTO.pundividedshare + ", 0),plintharea=coalesce(" + Applicationlist.HomeLoanDTO.pplintharea + ", 0), bookingdate=" + Applicationlist.HomeLoanDTO.pbookingdate + ", completiondate=" + Applicationlist.HomeLoanDTO.pcompletiondate + ", occupancycertificatedate=" + Applicationlist.HomeLoanDTO.poccupancycertificatedate + ",actualcost=coalesce(" + Applicationlist.HomeLoanDTO.pactualcost + ", 0), saleagreementvalue=coalesce(" + Applicationlist.HomeLoanDTO.psaleagreementvalue + ", 0), stampdutycharges=coalesce(" + Applicationlist.HomeLoanDTO.pstampdutycharges + ", 0), otheramenitiescharges=coalesce(" + Applicationlist.HomeLoanDTO.potheramenitiescharges + ", 0),otherincidentalexpenditure=coalesce(" + Applicationlist.HomeLoanDTO.potherincidentalexpenditure + ", 0), totalvalueofproperty=coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalueofproperty + ", 0), ageofbuilding='" + ManageQuote(Applicationlist.HomeLoanDTO.pageofbuilding) + "',originalcostofproperty=coalesce(" + Applicationlist.HomeLoanDTO.poriginalcostofproperty + ", 0), estimatedvalueofrepairs=coalesce(" + Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs + ", 0), amountalreadyspent=coalesce(" + Applicationlist.HomeLoanDTO.pamountalreadyspent + ", 0),otherborrowings=coalesce(" + Applicationlist.HomeLoanDTO.potherborrowings + ", 0), totalvalue=coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalue + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp,districtid= " + Applicationlist.HomeLoanDTO.pDistrictId + ",stateid= " + Applicationlist.HomeLoanDTO.pStateId + ",countryid=" + Applicationlist.HomeLoanDTO.pCountryId + "  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tabapplicationhomeloan(applicationid, vchapplicationid, contactid, contactreferenceid,initialpayment, propertylocation, propertyownershiptype, propertytype,purpose, propertystatus, address1, address2, city, state, district,country, pincode, buildertieup, projectname, ownername, selleraddress,buildingname, blockname, builtupareain, plotarea, undividedshare,plintharea, bookingdate, completiondate, occupancycertificatedate,actualcost, saleagreementvalue, stampdutycharges, otheramenitiescharges,otherincidentalexpenditure, totalvalueofproperty, ageofbuilding,originalcostofproperty, estimatedvalueofrepairs, amountalreadyspent,otherborrowings, totalvalue, statusid, createdby, createddate,districtid,stateid,countryid)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "',coalesce(" + Applicationlist.HomeLoanDTO.pinitialpayment + ", 0),'" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertylocation) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertyownershiptype) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertytype) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppurpose) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppropertystatus) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.paddress1) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.paddress2) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pcity) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pstate) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pdistrict) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pcountry) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.ppincode) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildertieup) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pprojectname) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pownername) + "', '" + ManageQuote(Applicationlist.HomeLoanDTO.pselleraddress) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pbuildingname) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.pblockname) + "', coalesce(" + Applicationlist.HomeLoanDTO.pbuiltupareain + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pplotarea + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pundividedshare + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pplintharea + ", 0)," + Applicationlist.HomeLoanDTO.pbookingdate + "," + Applicationlist.HomeLoanDTO.pcompletiondate + "," + Applicationlist.HomeLoanDTO.poccupancycertificatedate + ",coalesce(" + Applicationlist.HomeLoanDTO.pactualcost + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.psaleagreementvalue + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pstampdutycharges + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potheramenitiescharges + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potherincidentalexpenditure + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalueofproperty + ", 0),'" + ManageQuote(Applicationlist.HomeLoanDTO.pageofbuilding) + "',coalesce(" + Applicationlist.HomeLoanDTO.poriginalcostofproperty + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pestimatedvalueofrepairs + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.pamountalreadyspent + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.potherborrowings + ", 0),coalesce(" + Applicationlist.HomeLoanDTO.ptotalvalue + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp, " + Applicationlist.HomeLoanDTO.pDistrictId + ", " + Applicationlist.HomeLoanDTO.pStateId + ", " + Applicationlist.HomeLoanDTO.pCountryId + ");");
                        }
                    }
                    if (Applicationlist.HomeLoanDTO.BalanceTransferDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationbalancetransfer where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate = "'" + FormatDate(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate = "null";
                        }
                        else
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate = "'" + FormatDate(Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanamount.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pinstallmentamount.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.pinstallmentamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ptotaltenureofloan.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.ptotaltenureofloan = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbalancetenureofloan.ToString()))
                        {
                            Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbalancetenureofloan = 0;
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tblapplicationbalancetransfer set bankorcreditfacilityname='" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbankorcreditfacilityname) + "',loandate=" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate + ",loanaccountno='" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanaccountno) + "',loanamount=coalesce(" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanamount + ", 0),outstandingdate=" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate + ",installmentamount=" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.pinstallmentamount + ",loanpayin='" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanpayin) + "',totaltenureofloan=" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ptotaltenureofloan + ",balancetenureofloan=" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbalancetenureofloan + ",loansanctiondocpath='" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploansanctiondocpath) + "',emichartdocpath='" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pemichartdocpath) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tblapplicationbalancetransfer(applicationid,vchapplicationid,contactid,contactreferenceid,bankorcreditfacilityname,loandate,loanaccountno,loanamount,outstandingdate,installmentamount,loanpayin,totaltenureofloan,balancetenureofloan,loansanctiondocpath,emichartdocpath,statusid,createdby,createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbankorcreditfacilityname) + "'," + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploandate + ",'" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanaccountno) + "',coalesce(" + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanamount + ", 0)," + Applicationlist.HomeLoanDTO.BalanceTransferDTO.poutstandingdate + "," + Applicationlist.HomeLoanDTO.BalanceTransferDTO.pinstallmentamount + ",'" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploanpayin) + "'," + Applicationlist.HomeLoanDTO.BalanceTransferDTO.ptotaltenureofloan + "," + Applicationlist.HomeLoanDTO.BalanceTransferDTO.pbalancetenureofloan + ",'" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.ploansanctiondocpath) + "','" + ManageQuote(Applicationlist.HomeLoanDTO.BalanceTransferDTO.pemichartdocpath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "BALANCE TRANSFER")
                {
                    if (Applicationlist.BalanceTransferDTO != null)
                    {
                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationbalancetransfer where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ploandate))
                        {
                            Applicationlist.BalanceTransferDTO.ploandate = "null";
                        }
                        else
                        {
                            Applicationlist.BalanceTransferDTO.ploandate = "'" + FormatDate(Applicationlist.BalanceTransferDTO.ploandate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.poutstandingdate))
                        {
                            Applicationlist.BalanceTransferDTO.poutstandingdate = "null";
                        }
                        else
                        {
                            Applicationlist.BalanceTransferDTO.poutstandingdate = "'" + FormatDate(Applicationlist.BalanceTransferDTO.poutstandingdate) + "'";
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ploanamount.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.ploanamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.pinstallmentamount.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.pinstallmentamount = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.ptotaltenureofloan.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.ptotaltenureofloan = 0;
                        }
                        if (string.IsNullOrEmpty(Applicationlist.BalanceTransferDTO.pbalancetenureofloan.ToString()))
                        {
                            Applicationlist.BalanceTransferDTO.pbalancetenureofloan = 0;
                        }
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tblapplicationbalancetransfer set bankorcreditfacilityname='" + ManageQuote(Applicationlist.BalanceTransferDTO.pbankorcreditfacilityname) + "',loandate=" + Applicationlist.BalanceTransferDTO.ploandate + ",loanaccountno='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanaccountno) + "',loanamount=coalesce(" + Applicationlist.BalanceTransferDTO.ploanamount + ", 0),outstandingdate=" + Applicationlist.BalanceTransferDTO.poutstandingdate + ",installmentamount=" + Applicationlist.BalanceTransferDTO.pinstallmentamount + ",loanpayin='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanpayin) + "',totaltenureofloan=" + Applicationlist.BalanceTransferDTO.ptotaltenureofloan + ",balancetenureofloan=" + Applicationlist.BalanceTransferDTO.pbalancetenureofloan + ",loansanctiondocpath='" + ManageQuote(Applicationlist.BalanceTransferDTO.ploansanctiondocpath) + "',emichartdocpath='" + ManageQuote(Applicationlist.BalanceTransferDTO.pemichartdocpath) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid ='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tblapplicationbalancetransfer(applicationid,vchapplicationid,contactid,contactreferenceid,bankorcreditfacilityname,loandate,loanaccountno,loanamount,outstandingdate,installmentamount,loanpayin,totaltenureofloan,balancetenureofloan,loansanctiondocpath,emichartdocpath,statusid,createdby,createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BalanceTransferDTO.pbankorcreditfacilityname) + "'," + Applicationlist.BalanceTransferDTO.ploandate + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanaccountno) + "',coalesce(" + Applicationlist.BalanceTransferDTO.ploanamount + ", 0)," + Applicationlist.BalanceTransferDTO.poutstandingdate + "," + Applicationlist.BalanceTransferDTO.pinstallmentamount + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploanpayin) + "'," + Applicationlist.BalanceTransferDTO.ptotaltenureofloan + "," + Applicationlist.BalanceTransferDTO.pbalancetenureofloan + ",'" + ManageQuote(Applicationlist.BalanceTransferDTO.ploansanctiondocpath) + "','" + ManageQuote(Applicationlist.BalanceTransferDTO.pemichartdocpath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                        }
                    }
                }
                if (Applicationlist.pLoantype.ToUpper().Trim() == "BUSINESS LOAN")
                {
                    if (Applicationlist.BusinessLoanDTO != null)
                    {

                        ds = NPGSqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select count(*) as count from tblapplicationbusinessloan where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";select count(*) as count from tblapplicationbusinessfinancialperformance where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; select count(*) as count from tblapplicationbusinesscredittrendpurchases where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; select count(*) as count from tblapplicationbusinesscredittrendsales where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; select count(*) as count from tblapplicationbusinessstockposition where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + "; select count(*) as count from tblapplicationbusinesscostofproject where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationBusinessloanancillaryunitaddressdetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationBusinessloanassociateconcerndetails where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";select count(*) as count from tabapplicationBusinessloanturnoverandprofitorloss where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";");
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["count"]) > 0)
                        {
                            sbinsert.Append("update tblapplicationbusinessloan set descriptionoftheactivity='" + ManageQuote(Applicationlist.BusinessLoanDTO.pdescriptionoftheactivity) + "', isfinancialperformanceapplicable='" + Applicationlist.BusinessLoanDTO.pisfinancialperformanceapplicable + "', iscredittrendforpurchasesapplicable='" + Applicationlist.BusinessLoanDTO.piscredittrendforpurchasesapplicable + "',iscredittrendforsalesapplicable='" + Applicationlist.BusinessLoanDTO.piscredittrendforsalesapplicable + "', isstockpositionapplicable='" + Applicationlist.BusinessLoanDTO.pisstockpositionapplicable + "', iscostofprojectapplicable='" + Applicationlist.BusinessLoanDTO.piscostofprojectapplicable + "',isancillaryunit='" + Applicationlist.BusinessLoanDTO.pisancillaryunit + "', associateconcernsexist='" + Applicationlist.BusinessLoanDTO.passociateconcernsexist + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid = " + applicationid + " and vchapplicationid = '" + ManageQuote(strapplictionid) + "' and statusid = " + Convert.ToInt32(Status.Active) + ";");
                        }
                        else
                        {
                            sbinsert.Append("INSERT INTO tblapplicationbusinessloan(applicationid, vchapplicationid, contactid, contactreferenceid,descriptionoftheactivity, isfinancialperformanceapplicable, iscredittrendforpurchasesapplicable,iscredittrendforsalesapplicable, isstockpositionapplicable, iscostofprojectapplicable,isancillaryunit, associateconcernsexist, statusid, createdby,createddate)VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.pdescriptionoftheactivity) + "','" + Applicationlist.BusinessLoanDTO.pisfinancialperformanceapplicable + "','" + Applicationlist.BusinessLoanDTO.piscredittrendforpurchasesapplicable + "','" + Applicationlist.BusinessLoanDTO.piscredittrendforsalesapplicable + "','" + Applicationlist.BusinessLoanDTO.pisstockpositionapplicable + "','" + Applicationlist.BusinessLoanDTO.piscostofprojectapplicable + "','" + Applicationlist.BusinessLoanDTO.pisancillaryunit + "','" + Applicationlist.BusinessLoanDTO.passociateconcernsexist + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[1].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinessfinancialperformance(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,statusid, createdby, createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0),'" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessfinancialperformance set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "', turnoveramount=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0), netprofitamount=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0), balancesheetdocpath='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tblapplicationbusinessfinancialperformance set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pRecordid + ";");
                                    //}
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinessfinancialperformance(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, turnoveramount, netprofitamount, balancesheetdocpath,statusid, createdby, createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pnetprofitamount + ", 0),'" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath) + "'," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }



                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            sbUPDATEGRID.Append("update tblapplicationbusinessfinancialperformance set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and  recordid not in(" + Recordid + ");");

                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("update tblapplicationbusinessfinancialperformance set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " ;");
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count > 0)
                        {

                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid1))
                                    {
                                        recordid1 = Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid1 = recordid1 + "," + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[2].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendpurchases(applicationid, vchapplicationid, contactid, contactreferenceid, finacialyear, suppliername, address, contactno, maxcreditreceived, mincreditreceived, avgtotalcreditreceived, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0), " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendpurchases set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', suppliername='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', address='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', contactno='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', maxcreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), mincreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), avgtotalcreditreceived=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tblapplicationbusinesscredittrendpurchases set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pRecordid + ";");
                                    //}

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendpurchases(applicationid, vchapplicationid, contactid, contactreferenceid, finacialyear, suppliername, address, contactno, maxcreditreceived, mincreditreceived, avgtotalcreditreceived, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pfinacialyear) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].psuppliername) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].paddress) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pcontactno) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmaxcreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pmincreditreceived + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO[i].pavgtotalcreditreceived + ", 0), " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                }
                            }


                        }
                        if (!string.IsNullOrEmpty(recordid1))
                        {
                            sbUPDATEGRID.Append("update tblapplicationbusinesscredittrendpurchases set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " and recordid not in(" + recordid1 + ");");
                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendpurchasesDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("update tblapplicationbusinesscredittrendpurchases set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + " ;");
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count > 0)
                        {

                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid2))
                                    {
                                        recordid2 = Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid2 = recordid2 + "," + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[3].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendsales(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinesscredittrendsales set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "', customername='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "', address='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "', contactno='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "', maxcreditgiven=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0),mincreditgiven=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0), totalcreditsales=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tblapplicationbusinesscredittrendsales set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pRecordid + ";");
                                    //}

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscredittrendsales(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear, customername, address, contactno, maxcreditgiven,mincreditgiven, totalcreditsales, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pfinacialyear) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcustomername) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].paddress) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pcontactno) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmaxcreditgiven + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].pmincreditgiven + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO[i].ptotalcreditsales + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }


                        }
                        if (!string.IsNullOrEmpty(recordid2))
                        {
                            sbUPDATEGRID.Append("update tblapplicationbusinesscredittrendsales set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid not in(" + recordid2 + ");");
                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinesscredittrendsalesDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("update tblapplicationbusinesscredittrendsales set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO.Count > 0)
                        {

                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid3))
                                    {
                                        recordid3 = Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid3 = recordid3 + "," + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[4].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tblapplicationbusinessstockposition(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear,maxstockcarried,minstockcarried, avgtotalstockcarried, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("update tblapplicationbusinessstockposition set finacialyear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',maxstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0),minstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0), avgtotalstockcarried=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid + ";");
                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("update tblapplicationbusinessstockposition set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid=" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pRecordid + ";");
                                    //}
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinessstockposition(applicationid, vchapplicationid, contactid, contactreferenceid,finacialyear,maxstockcarried,minstockcarried, avgtotalstockcarried, statusid, createdby,createddate) VALUES (" + applicationid + ",'" + ManageQuote(strapplictionid) + "'," + Applicationlist.pApplicantid + ",'" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pfinacialyear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pmaxstockcarried + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pminstockcarried + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO[i].pavgtotalstockcarried + ", 0)," + Convert.ToInt32(Status.Active) + "," + Applicationlist.pCreatedby + ",current_timestamp);");
                                }
                            }


                        }
                        if (!string.IsNullOrEmpty(recordid3))
                        {
                            sbUPDATEGRID.Append("update tblapplicationbusinessstockposition set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1 and recordid not in(" + recordid3 + ");");

                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinessstockpositionDTO.Count == 0)
                            {
                                sbUPDATEGRID.Append("update tblapplicationbusinessstockposition set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count > 0)
                        {
                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO.Count; i++)
                            {
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital = 0;
                                }
                                if (Convert.ToInt32(ds.Tables[5].Rows[0]["count"]) > 0)
                                {
                                    sbinsert.Append("update tblapplicationbusinesscostofproject set costoflandincludingdevelopment=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment + ", 0), buildingandothercivilworks=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks + ", 0), plantandmachinery=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery + ", 0),equipmenttools=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools + ", 0), testingequipment=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment + ", 0), miscfixedassets=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets + ", 0), erectionorinstallationcharges=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges + ", 0),preliminaryorpreoperativeexpenses=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses + ", 0), provisionforcontingencies=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies + ", 0),marginforworkingcapital=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=1;");
                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tblapplicationbusinesscostofproject(applicationid, vchapplicationid, contactid, contactreferenceid,costoflandincludingdevelopment, buildingandothercivilworks, plantandmachinery,equipmenttools, testingequipment, miscfixedassets, erectionorinstallationcharges,preliminaryorpreoperativeexpenses, provisionforcontingencies,marginforworkingcapital, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pcostoflandincludingdevelopment + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pbuildingandothercivilworks + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pplantandmachinery + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pequipmenttools + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ptestingequipment + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmiscfixedassets + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].perectionorinstallationcharges + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].ppreliminaryorpreoperativeexpenses + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pprovisionforcontingencies + ", 0), coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinesscostofprojectDTO[i].pmarginforworkingcapital + ", 0)," + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                }
                            }
                        }
                        if (Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO != null)
                        {
                            if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid.ToString()))
                            {
                                Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid.ToString()))
                            {
                                Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid = 0;
                            }
                            if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid.ToString()))
                            {
                                Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid = 0;
                            }
                            if (Convert.ToInt32(ds.Tables[6].Rows[0]["count"]) > 0)
                            {
                                sbinsert.Append("UPDATE tabapplicationbusinessloanancillaryunitaddressdetails SET  address1='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress1) + "' , address2='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress2) + "', city='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcity) + "', country='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pCountry) + "', state='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pState) + "', district='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pDistrict) + "', pincode='" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pPincode) + "',stateid=" + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid + ",districtid=" + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid + ",countryid=" + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' and statusid=" + Convert.ToInt32(Status.Active) + ";");
                            }
                            else
                            {
                                sbinsert.Append("INSERT INTO tabapplicationbusinessloanancillaryunitaddressdetails(applicationid, vchapplicationid, contactid, contactreferenceid, address1, address2, city, country, state, district, pincode,statusid, createdby, createddate,stateid,districtid,countryid)VALUES (" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress1) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pAddress2) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcity) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pCountry) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pState) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pDistrict) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pPincode) + "'," + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp," + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pstateid + "," + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pdistrictid + "," + Applicationlist.BusinessLoanDTO.BusinessancillaryunitaddressdetailsDTO.pcountryid + ");");
                            }
                        }

                        if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Count > 0)
                        {

                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid4))
                                    {
                                        recordid4 = Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid4 = recordid4 + "," + Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pRecordid.ToString();
                                    }
                                }
                                if (Convert.ToInt32(ds.Tables[7].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tabapplicationbusinessloanassociateconcerndetails(applicationid, vchapplicationid, contactid, contactreferenceid,nameofassociateconcern, natureofassociation, natureofactivity, itemstradedormanufactured, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnameofassociateconcern) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofassociation) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofactivity) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pitemstradedormanufactured) + "', " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("UPDATE tabapplicationbusinessloanassociateconcerndetails SET  nameofassociateconcern='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnameofassociateconcern) + "', natureofassociation='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofassociation) + "',natureofactivity='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofactivity) + "', itemstradedormanufactured='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pitemstradedormanufactured) + "',modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pRecordid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");

                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationbusinessloanassociateconcerndetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pRecordid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                    //}

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationbusinessloanassociateconcerndetails(applicationid, vchapplicationid, contactid, contactreferenceid,nameofassociateconcern, natureofassociation, natureofactivity, itemstradedormanufactured, statusid, createdby, createddate)VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnameofassociateconcern) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofassociation) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pnatureofactivity) + "', '" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails[i].pitemstradedormanufactured) + "', " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                }
                            }


                        }
                        if (!string.IsNullOrEmpty(recordid4))
                        {
                            sbUPDATEGRID.Append("UPDATE tabapplicationbusinessloanassociateconcerndetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID not in(" + recordid4 + ");");


                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinessloanassociateconcerndetails.Count == 0)
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationbusinessloanassociateconcerndetails SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }

                        }

                        if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Count > 0)
                        {


                            for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Count; i++)
                            {
                                if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pTypeofoperation.ToUpper().Trim() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(recordid5))
                                    {
                                        recordid5 = Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        recordid5 = recordid5 + "," + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveramount.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveramount = 0;
                                }
                                if (string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoverprofit.ToString()))
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoverprofit = 0;
                                }

                                if (Convert.ToInt32(ds.Tables[8].Rows[0]["count"]) > 0)
                                {
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pTypeofoperation.ToUpper().Trim() == "CREATE")
                                    {
                                        sbinsert.Append("INSERT INTO tabapplicationbusinessloanturnoverandprofitorloss(applicationid, vchapplicationid, contactid, contactreferenceid,turnoveryear, turnoveramount, turnoverprofit, statusid, createdby,createddate)VALUES (" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveryear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoverprofit + ", 0), " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                    }
                                    if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pTypeofoperation.ToUpper().Trim() == "UPDATE")
                                    {
                                        sbinsert.Append("UPDATE tabapplicationbusinessloanturnoverandprofitorloss SET turnoveryear='" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveryear) + "', turnoveramount=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveramount + ", 0), turnoverprofit=coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoverprofit + ", 0),modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pRecordid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");

                                    }
                                    //if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pTypeofoperation.ToUpper().Trim() == "DELETE")
                                    //{
                                    //    sbinsert.Append("UPDATE tabapplicationbusinessloanturnoverandprofitorloss SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=1,modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID=" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pRecordid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");
                                    //}

                                }
                                else
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationbusinessloanturnoverandprofitorloss(applicationid, vchapplicationid, contactid, contactreferenceid,turnoveryear, turnoveramount, turnoverprofit, statusid, createdby,createddate)VALUES (" + applicationid + ", '" + ManageQuote(strapplictionid) + "', " + Applicationlist.pApplicantid + ", '" + ManageQuote(Applicationlist.pContactreferenceid) + "','" + ManageQuote(Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveryear) + "',coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoveramount + ", 0),coalesce(" + Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss[i].pturnoverprofit + ", 0), " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ", current_timestamp);");
                                }
                            }


                        }
                        if (!string.IsNullOrEmpty(recordid5))
                        {
                            sbUPDATEGRID.Append("UPDATE tabapplicationbusinessloanturnoverandprofitorloss SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "' AND RECORDID not in(" + recordid5 + ");");
                        }
                        else
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinessloanturnoverandprofitorloss.Count == 0)
                            {
                                sbUPDATEGRID.Append("UPDATE tabapplicationbusinessloanturnoverandprofitorloss SET  STATUSID=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp  where applicationid=" + applicationid + " and vchapplicationid='" + ManageQuote(strapplictionid) + "';");
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sbinsert.ToString()) || !string.IsNullOrEmpty(sbUPDATEGRID.ToString()))
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
            return IsSaved;

        }

        #endregion

        #region ApplicationReferrenceDetails

        public bool SaveApplicationReferenceData(long applicationid, string strapplictionid, ApplicationReferencesDTO Applicationlist, string ConnectionString)
        {
            int SavedCount = 0;
            StringBuilder sbInsertReferenceData = new StringBuilder();
            string Recordid = string.Empty;
            string Query = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (!string.IsNullOrEmpty(strapplictionid))
                {
                    applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + strapplictionid + "';"));
                }

                sbInsertReferenceData.Append("UPDATE tabapplication set isreferencesapplicable= " + Applicationlist.pIsreferencesapplicable + " where vchapplicationid = '" + strapplictionid + "';");
                if (Applicationlist.pIsreferencesapplicable)
                {
                    if (Applicationlist.LobjAppReferences.Count > 0 && Applicationlist.LobjAppReferences != null)
                    {
                        foreach (ApplicationLoanReferencesDTO objReferenceData in Applicationlist.LobjAppReferences)
                        {
                            if (!string.IsNullOrEmpty(objReferenceData.ptypeofoperation))
                            {
                                if (objReferenceData.ptypeofoperation.Trim().ToUpper() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = objReferenceData.pRefRecordId.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + objReferenceData.pRefRecordId.ToString();
                                    }
                                }
                                if (Convert.ToString(objReferenceData.pRefcontactNo) == string.Empty)
                                {
                                    objReferenceData.pRefcontactNo = 0;
                                }
                                if (Convert.ToString(objReferenceData.pRefalternatecontactNo) == string.Empty)
                                {
                                    objReferenceData.pRefalternatecontactNo = 0;
                                }
                                if (objReferenceData.ptypeofoperation.Trim().ToUpper() == "CREATE")
                                {
                                    sbInsertReferenceData.Append("INSERT INTO tabapplicationreferences(applicationid, vchapplicationid, firstname, lastname,        contactnumber, alternatenumber, emailid, alternateemailid, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "','" + ManageQuote(objReferenceData.pRefFirstname) + "' , '" + ManageQuote(objReferenceData.pRefLastname) + "', " + objReferenceData.pRefcontactNo + "," + objReferenceData.pRefalternatecontactNo + ", '" + ManageQuote(objReferenceData.pRefEmailID) + "', '" + ManageQuote(objReferenceData.pRefAlternateEmailId) + "', " + Convert.ToInt32(Status.Active) + ", " + Applicationlist.pCreatedby + ",current_timestamp" + ");");
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            Query = "update tabapplicationreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where recordid not in(" + Recordid + ")" +
                                "and upper(vchapplicationid)='" + ManageQuote(strapplictionid).Trim().ToUpper() + "' and applicationid=" + applicationid + ";";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Applicationlist.LobjAppReferences.ToString()))
                            {
                                Query = "update tabapplicationreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where upper(vchapplicationid)='" + ManageQuote(strapplictionid).Trim().ToUpper() + "' and applicationid=" + applicationid + ";";
                            }
                        }
                    }
                }
                if (Convert.ToString(sbInsertReferenceData) != string.Empty)
                {
                    SavedCount = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query + "" + sbInsertReferenceData.ToString());
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                trans.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    trans.Dispose();
                }
            }
            return SavedCount > 0 ? true : false;
        }

        public ApplicationReferencesDTO GetApplicationReferenceData(long applicationId, string vchapplicationID, string connectionString)
        {
            List<ApplicationLoanReferencesDTO> lObjloanAppReference = new List<ApplicationLoanReferencesDTO>();
            ApplicationReferencesDTO objReferences = new ApplicationReferencesDTO();

            try
            {

                if (!string.IsNullOrEmpty(vchapplicationID))
                {
                    applicationId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + vchapplicationID + "';"));
                }
                bool Isreferencesapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select isreferencesapplicable from tabapplication where vchapplicationid = '" + vchapplicationID + "';"));
                objReferences.pIsreferencesapplicable = Isreferencesapplicable;

                if (Isreferencesapplicable == true)
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT recordid,applicationid, vchapplicationid, firstname, lastname,coalesce( contactnumber,0) as contactnumber,coalesce( alternatenumber,0) as alternatenumber, emailid, alternateemailid FROM tabapplicationreferences where applicationId=" + applicationId + " and upper(vchapplicationid)='" + ManageQuote(vchapplicationID).Trim().ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + "; "))
                    {
                        while (dr.Read())
                        {
                            ApplicationLoanReferencesDTO ObjloanAppReference = new ApplicationLoanReferencesDTO();
                            ObjloanAppReference.pRefRecordId = Convert.ToInt64(dr["recordid"]);
                            ObjloanAppReference.papplicationId = Convert.ToInt64(dr["applicationid"]);
                            ObjloanAppReference.pvchapplicationId = Convert.ToString(dr["vchapplicationid"]);
                            ObjloanAppReference.pRefFirstname = Convert.ToString(dr["firstname"]);
                            ObjloanAppReference.pRefLastname = Convert.ToString(dr["lastname"]);
                            ObjloanAppReference.pRefcontactNo = Convert.ToDecimal(dr["contactnumber"]);
                            ObjloanAppReference.pRefalternatecontactNo = Convert.ToDecimal(dr["alternatenumber"]);
                            ObjloanAppReference.pRefEmailID = Convert.ToString(dr["emailid"]);
                            ObjloanAppReference.pRefAlternateEmailId = Convert.ToString(dr["alternateemailid"]);
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

        public bool UpdateApplicationReferenceData(long applicationid, string strapplictionid, ApplicationReferencesDTO Applicationlist, string ConnectionString)
        {
            int SavedCount = 0;
            StringBuilder sbInsertReferenceData = new StringBuilder();
            string Recordid = string.Empty;
            string Query = string.Empty;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (!string.IsNullOrEmpty(strapplictionid))
                {
                    applicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + strapplictionid + "';"));
                }

                if (Applicationlist.pIsreferencesapplicable == true)
                {
                    if (Applicationlist.LobjAppReferences != null && Applicationlist.LobjAppReferences.Count > 0)
                    {
                        foreach (ApplicationLoanReferencesDTO objReferenceData in Applicationlist.LobjAppReferences)
                        {
                            if (!string.IsNullOrEmpty(objReferenceData.ptypeofoperation))
                            {
                                if (objReferenceData.ptypeofoperation.Trim().ToUpper() != "CREATE")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = objReferenceData.pRefRecordId.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + objReferenceData.pRefRecordId.ToString();
                                    }
                                }
                                if (objReferenceData.ptypeofoperation.Trim().ToUpper() == "CREATE")
                                {
                                    sbInsertReferenceData.Append("INSERT INTO tabapplicationreferences(applicationid, vchapplicationid, firstname, lastname,        contactnumber, alternatenumber, emailid, alternateemailid, statusid, createdby, createddate) VALUES(" + applicationid + ", '" + ManageQuote(strapplictionid) + "','" + ManageQuote(objReferenceData.pRefFirstname) + "' , '" + ManageQuote(objReferenceData.pRefLastname) + "', " + objReferenceData.pRefcontactNo + "," + objReferenceData.pRefalternatecontactNo + ", '" + ManageQuote(objReferenceData.pRefEmailID) + "', '" + ManageQuote(objReferenceData.pRefAlternateEmailId) + "', " + Convert.ToInt32(Status.Active) + ", " + objReferenceData.pCreatedby + ",current_timestamp" + ");");
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Recordid))
                        {
                            Query = "update tabapplicationreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where recordid not in(" + Recordid + ")" +
                                "and upper(vchapplicationid)='" + ManageQuote(strapplictionid).Trim().ToUpper() + "' and applicationid=" + applicationid + ";";
                        }
                        else
                        {
                            Query = "update tabapplicationreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where upper(vchapplicationid)='" + ManageQuote(strapplictionid).Trim().ToUpper() + "' and applicationid=" + applicationid + ";";
                        }
                    }
                    else
                    {
                        Query = "update tabapplicationreferences set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + Applicationlist.pCreatedby + ",modifieddate=current_timestamp where upper(vchapplicationid)='" + ManageQuote(strapplictionid).Trim().ToUpper() + "' and applicationid=" + applicationid + ";";
                    }
                }
                if (Convert.ToString(sbInsertReferenceData) != string.Empty)
                {
                    SavedCount = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query + "" + sbInsertReferenceData.ToString());
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                trans.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    trans.Dispose();
                }
            }
            return SavedCount > 0 ? true : false;
        }

        #endregion

        #region Consumer Loan
        public bool SaveConsumableproduct(ProducttypeDTO ProducttypeDTO, string ConnectionString)
        {
            bool IsSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstconsumableproducttypes(producttype,statusid,createdby,createddate) values('" + ManageQuote(ProducttypeDTO.pproducttype) + "'," + Convert.ToInt32(Status.Active) + "," + ProducttypeDTO.pCreatedby + ",CURRENT_TIMESTAMP);");
                IsSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return IsSaved;
        }

        public List<ProducttypeDTO> BindConsumableproduct(string ConnectionString)
        {
            lstProducttype = new List<ProducttypeDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,producttype from tblmstconsumableproducttypes where statusid=" + Convert.ToInt32(Status.Active) + " order by recordid;"))
                {
                    while (dr.Read())
                    {
                        ProducttypeDTO ProducttypeDTO = new ProducttypeDTO();
                        ProducttypeDTO.pRecordid = Convert.ToInt64(dr["recordid"]);
                        ProducttypeDTO.pproducttype = dr["producttype"].ToString();
                        lstProducttype.Add(ProducttypeDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lstProducttype;

        }
        #endregion

        #region LOANAGAINSTPROPERTYLOANSPECIFICFIELDS

        public bool saveApplicationLoanAgainstpropertyLoanspecificfiels(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO, string connectionstring)
        {
            bool isSaved = false;
            string Recordid = string.Empty;
            StringBuilder sbinsert = new StringBuilder();
            StringBuilder sbinsertfirst = new StringBuilder();
            int countsecuritysectiondataexists = 0;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(SecurityandCollateralDTO.pVchapplicationid))
                {
                    SecurityandCollateralDTO.pApplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';"));
                }
                if (SecurityandCollateralDTO.ImMovablePropertyDetailsList != null || SecurityandCollateralDTO.MovablePropertyDetailsList != null)
                {
                    SecurityandCollateralDTO.pissecurityandcolletralapplicable = true;
                    if (SecurityandCollateralDTO.ImMovablePropertyDetailsList != null)
                    {
                        SecurityandCollateralDTO.pIsimmovablepropertyapplicable = true;
                    }
                    else
                    {
                        SecurityandCollateralDTO.pIsimmovablepropertyapplicable = false;

                    }
                    if (SecurityandCollateralDTO.MovablePropertyDetailsList != null)
                    {
                        SecurityandCollateralDTO.pismovablepropertyapplicable = true;
                    }
                    else
                    {
                        SecurityandCollateralDTO.pismovablepropertyapplicable = false;

                    }
                    sbinsert.Append("UPDATE tabapplication set issecurityandcolletralapplicable= " + SecurityandCollateralDTO.pissecurityandcolletralapplicable + " where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';");
                    if (SecurityandCollateralDTO.pissecurityandcolletralapplicable == true)
                    {
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select applicantid,contactreferenceid  from tabapplication where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';"))
                        {
                            while (dr.Read())
                            {
                                SecurityandCollateralDTO.pContactid = Convert.ToInt64(dr["applicantid"]);
                                SecurityandCollateralDTO.pContactreferenceid = dr["contactreferenceid"].ToString();
                            }
                        }
                        countsecuritysectiondataexists = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tabapplicationsecurityapplicablesections where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';"));
                        if (SecurityandCollateralDTO.pStatusname == null)
                        {
                            SecurityandCollateralDTO.pStatusname = "ACTIVE";
                        }
                        if (countsecuritysectiondataexists == 0)
                        {
                            sbinsert.Append("INSERT INTO tabapplicationsecurityapplicablesections(applicationid, vchapplicationid, contactid, contactreferenceid,isimmovablepropertyapplicable, ismovablepropertyapplicable,statusid,createdby, createddate)VALUES( " + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', " + SecurityandCollateralDTO.pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.pContactreferenceid) + "'," + SecurityandCollateralDTO.pIsimmovablepropertyapplicable + ", " + SecurityandCollateralDTO.pismovablepropertyapplicable + " , " + Convert.ToInt32(Status.Active) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                        }
                        else
                        {
                            sbinsert.Append("update tabapplicationsecurityapplicablesections set isimmovablepropertyapplicable=" + SecurityandCollateralDTO.pIsimmovablepropertyapplicable + ", ismovablepropertyapplicable=" + SecurityandCollateralDTO.pismovablepropertyapplicable + ", statusid=" + Convert.ToInt32(Status.Active) + ",modifiedby= " + SecurityandCollateralDTO.pCreatedby + ",modifieddate= current_timestamp where vchapplicationid='" + SecurityandCollateralDTO.pVchapplicationid + "' ;");
                        }
                    }
                    saveImmovablemovablepropertydetails(SecurityandCollateralDTO, connectionstring, sbinsert, sbinsertfirst);
                }

                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsertfirst + "" + sbinsert.ToString());
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

        #region PERSONALLOANSPECIFICFIELDS

        #endregion

        #region FI View Details
        public async Task<List<FirstinformationDTO>> GetFirstInformationView(string connectionString)
        {
            lstFirstinformation = new List<FirstinformationDTO>();

            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select loantypeid,loanid,applicationid,vchapplicationid,applicantid,contactreferenceid, dateofapplication,applicantname,loanname,loantype,amountrequested,tenureofloan,rateofinterest,statusname,loanpayin from tabapplication ta join tblmststatus ts on ta.loanstatusid=ts.statusid where ts.statusid in(" + Convert.ToInt32(Status.FI_Saved) + "," + Convert.ToInt32(Status.FI_Partial_Saved) + "," + Convert.ToInt32(Status.Field_Verification) + "," + Convert.ToInt32(Status.Tele_Verification) + "," + Convert.ToInt32(Status.Document_Verification) + ") order by applicationid desc;"))
                    {
                        while (dr.Read())
                        {
                            FirstinformationDTO objfirstinformationview = new FirstinformationDTO();
                            objfirstinformationview.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                            objfirstinformationview.pLoanid = Convert.ToInt64(dr["loanid"]);
                            objfirstinformationview.pVchapplicationid = Convert.ToString(dr["vchapplicationid"]);
                            objfirstinformationview.pApplicantid = Convert.ToInt64(dr["applicantid"]);
                            objfirstinformationview.pContactreferenceid = Convert.ToString(dr["contactreferenceid"]);
                            objfirstinformationview.pDateofapplication = Convert.ToDateTime(dr["Dateofapplication"]).ToString("dd/MM/yyyy");
                            objfirstinformationview.pApplicantname = Convert.ToString(dr["applicantname"]);
                            objfirstinformationview.pLoanname = Convert.ToString(dr["loanname"]);
                            objfirstinformationview.pLoantype = Convert.ToString(dr["loantype"]);
                            objfirstinformationview.pAmountrequested = Convert.ToDecimal(dr["amountrequested"]);
                            objfirstinformationview.pTenureofloan = Convert.ToInt64(dr["tenureofloan"]);
                            objfirstinformationview.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                            objfirstinformationview.pLoanpayin = Convert.ToString(dr["loanpayin"]);
                            objfirstinformationview.pStatusname = Convert.ToString(dr["statusname"]);
                            objfirstinformationview.ptypeofoperation = "OLD";
                            lstFirstinformation.Add(objfirstinformationview);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            return lstFirstinformation;
        }

        #endregion

        #region Fi Emi Schesule view

        public async Task<FirstinformationDTO> GetFiEmiSchesuleview(decimal loanamount, string interesttype, string loanpayin, decimal interestrate, int tenureofloan, string Loaninstalmentmode, int emiprincipalpayinterval, string connectionString)
        {
            FirstinformationDTO = new FirstinformationDTO();

            await Task.Run(() =>
            {
                try
                {
                    FirstinformationDTO.lstInstalmentsgenerationDTO = new List<InstalmentsgenerationDTO>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT installmentno1,installmentprinciple1,installmentinterest1,installmentamount1 from fn_emichartgeneration(" + loanamount + ", '" + interesttype + "', '" + loanpayin + "'," + interestrate + ", " + tenureofloan + ", '" + Loaninstalmentmode + "', " + emiprincipalpayinterval + ",'SINGLE PAYMENT'," + loanamount + ",1,CURRENT_DATE,CURRENT_DATE) order by installmentno1;"))
                    {
                        while (dr.Read())
                        {
                            FirstinformationDTO.lstInstalmentsgenerationDTO.Add(new InstalmentsgenerationDTO
                            {
                                pInstalmentno = Convert.ToInt32(dr["installmentno1"]),
                                pInstalmentprinciple = Convert.ToDecimal(dr["installmentprinciple1"]),
                                pInstalmentinterest = Convert.ToDecimal(dr["installmentinterest1"]),
                                pInstalmentamount = Convert.ToDecimal(dr["installmentamount1"])
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            return FirstinformationDTO;
        }
        #endregion
    }
}
