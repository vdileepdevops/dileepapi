using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Loans.Transactions
{
    public interface IFirstinformation
    {
        #region SavesaveApplication
        string Saveapplication(ApplicationDTO Applicationlist, string Connectionstring);
        #endregion

        #region Configured LoanMaster data
        List<LoansMasterDTO> getfiLoanTypes(string ConnectionString);
        List<GetLoandetailsDTO> GetLoanpayin(string ConnectionString, long Loanid, string Contacttype, string Applicanttype, int schemeid);
        List<GetLoandetailsDTO> GetLoanInterestTypes(string ConnectionString, long Loanid, int schemeid,string Contacttype, string Applicanttype, string Loanpayin);

        List<GetLoandetailsDTO> getLoaninstalmentmodes(string ConnectionString, string loanpayin, string interesttype);
        #endregion
        #region  GetSchemenamescodes
        List<GetLoandetailsDTO> GetSchemenamescodes(string ConnectionString, long Loanid);
        #endregion

        #region  GetInterestRateBasedOnLoanpayin
        List<GetLoandetailsDTO> GetInterestRates(string ConnectionString, FirstinformationDTO FirstinformationDTO);
        List<GetLoandetailsDTO> GetLoanMinandmaxAmounts(string ConnectionString, long Loanid, string Contacttype, string Applicanttype, string Loanpayin, int schemeid, string interesttype);
        #endregion

        #region  GetApplicantSureitytypes
        //List<SurityApplicantsDTO> GetApplicantSureitytypes(string Strapplicationid, string ConnectionString);
        ApplicationApplicantandOthersDTO Getsurietypersondetails(string Applicationid, string ConnectionString);
        List<SurityApplicantsDTO> GetSurityapplicants(string ConnectionString, string contacttype);
        #endregion

        #region  GetApplicantCreditandkycdetails
        ApplicationKYCDocumentsDTO GetApplicantCreditandkycdetails(string Strapplicationid, Int64 contactid, string ConnectionString);
        #endregion

        #region  GetAcknowledgementDetails
        List<AcknowledgementDTO> GetAcknowledgementDetails(string ConnectionString);

        bool SaveAcknowledgementDetails(string con, AcknowledgementDTO acknowlwdgement);
        #endregion


        #region  Saveapplicationsurityapplicantdetails
        bool Saveapplicationsurityapplicantdetails(ApplicationApplicantandOthersDTO Applicationlist, string Connectionstring);
        #endregion

        #region  Savekycandidentificationdocuments
        bool Savekycandidentificationdocuments(ApplicationKYCDocumentsDTO Applicationlist, string Connectionstring);
        #endregion
        #region  GetAllLoandetails
        List<FirstinformationDTO> GetAllLoandetails(string Applicationid, string ConnectionString);
        #endregion

        #region  Getloandetails
        List<FirstinformationDTO> Getloandetails(string Applicationid, string ConnectionString);
        #endregion

        #region  Deletesueritydetails 
        bool Deletesueritydetails(string strapplictionid, string strconrefid, int Createdby, string Connectionstring);
        #endregion

        #region  Deletecreditandkycdetails 
        bool Deletecreditandkycdetails(string strapplictionid, string strconrefid, int Createdby, string Connectionstring);
        #endregion


        bool SaveSurityApplicantnames(SurityApplicantsDTO surityapplicants, string con);
        int checkInsertSuritypplicantsDuplicates(string surityapplicanttype, string con);

        #region Masters for homeloan
        bool Savepropertylocation(PropertylocationDTO PropertylocationDTO, string ConnectionString);
        bool Savepropertyownershiptype(PropertyownershiptypeDTO PropertyownershiptypeDTO, string ConnectionString);
        bool Savepropertytype(propertytypeDTO propertytypeDTO, string ConnectionString);
        bool Savepurpose(purposeDTO purposeDTO, string ConnectionString);
        bool Savepropertystatus(propertystatusDTO propertystatusDTO, string ConnectionString);

        #endregion

        #region Bind HomeLoan masters
        List<PropertylocationDTO> BindPropertylocation(string ConnectionString);
        List<PropertyownershiptypeDTO> BindPropertyownershiptype(string ConnectionString);
        List<propertytypeDTO> Bindpropertytype(string ConnectionString);
        List<purposeDTO> Bindpurpose(string ConnectionString);
        List<propertystatusDTO> Bindpropertystatus(string ConnectionString);
        #endregion

        bool SaveApplicationPersonalInformation(ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO, string ConnectionString);
        ApplicationPersonalInformationDTO GetApplicationPersonalInformation(string strapplictionid, string ConnectionString);

        #region Bind Applicant Details
        Task<ApplicationLoanSpecificDTO> GetApplicantLoanSpecificDetails(string strapplictionid, string ConnectionString);

        #endregion

        #region  Application Personal Details
        List<EmployeeRoleDTO> GetEmployementRoles(string ConnectionString);
        bool SaveApplicationexistingloansDetails(ExistingLoanDetailsDTO objApplicationExistingLoanDetails, string con);

        #endregion

        #region Education Loan Masters
        bool SaveEducationalQualification(EducationalQualificationDTO EducationalQualificationDTO, string ConnectionString);
        bool SaveEducationalFeeQualification(EducationalFeeQualificationDTO EducationalFeeQualificationDTO, string ConnectionString);
        bool SaveEducationalFeeYear(EducationalFeeYearDTO EducationalFeeYearDTO, string ConnectionString);
        List<EducationalQualificationDTO> BindEducationalQualification(string ConnectionString);
        List<EducationalFeeYearDTO> BindEducationalFeeYear(string ConnectionString);
        List<EducationalFeeQualificationDTO> BindEducationalFeeQualification(string ConnectionString);
        List<ApplicantIdsDTO> BindApplicantIds(string LoanType, string ConnectionString);
        bool SaveLoanSpecificDetails(string strapplictionid, ApplicationLoanSpecificDTO Applicationlist, string ConnectionString);

        #endregion

        #region Security and Collateral
        bool saveApplicationSecurityCollateral(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO, string connectionstring);
        ApplicationSecurityandCollateralDTO getSecurityCollateralDetails(long applicationid, string strapplicationid, string ConnectionString);
        #endregion




        #region Application Reference Data
        bool SaveApplicationReferenceData(long applicationid, string strapplictionid, ApplicationReferencesDTO Applicationlist, string ConnectionString);


        ApplicationReferencesDTO GetApplicationReferenceData(long applicationId, string vchapplicationID, string connectionString);

        bool UpdateApplicationReferenceData(long applicationid, string strapplictionid, ApplicationReferencesDTO Applicationlist, string ConnectionString);

        #endregion

        #region Consumer Loan
        bool SaveConsumableproduct(ProducttypeDTO ProducttypeDTO, string ConnectionString);
        List<ProducttypeDTO> BindConsumableproduct(string ConnectionString);
        #endregion

        #region LOANAGAINSTPROPERTYLOANSPECIFICFIELDS
        bool saveApplicationLoanAgainstpropertyLoanspecificfiels(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO, string connectionstring);
        #endregion

        #region PERSONALLOANSPECIFICFIELDS
        bool SaveApplicationPersonalLOANInformation(ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO, string ConnectionString);
        #endregion
        #region ApplicationExistingLoanDetails
        ExistingLoanDetailsDTO GetApplicationExistingLoanDetails(string contactreferenceid, string vchapplicationid, string con);
        #endregion

        #region FI View Details
        Task<List<FirstinformationDTO>> GetFirstInformationView(string connectionString);
        #endregion
        #region Fi Emi Schesule view
        Task<FirstinformationDTO> GetFiEmiSchesuleview(decimal loanamount, string interesttype, string loanpayin, decimal interestrate, int tenureofloan, string Loaninstalmentmode, int emiprincipalpayinterval, string connectionString);
        #endregion
        Task<FIDocumentViewDTO> GetFIDocumentView(string strapplictionid, string ConnectionString);

        ApplicationPersonalInformationDTO GetApplicationPersonalLoanInformation(string strapplictionid, string ConnectionString);

    }
}
