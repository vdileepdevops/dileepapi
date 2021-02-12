using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Loans.Transactions;
using FinstaRepository.Interfaces.Loans.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace FinstaApi.Controllers.Loans.Transactions
{
   // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class FirstinformationController : ControllerBase
    {
        IFirstinformation Objfirstinformation = new FirstinformationDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public List<LoansMasterDTO> lstLoanMasterdetails { get; set; }
        FirstinformationDTO FirstinformationDTO { set; get; }
        ApplicationLoanSpecificDTO ApplicationLoanSpecificDTO { set; get; }
        public List<SurityApplicantsDTO> lstSurityapplicants { get; set; }
        public List<FirstinformationDTO> lstFirstinformation { get; set; }
        public List<PropertylocationDTO> lstPropertylocationDTO { get; set; }
        public List<PropertyownershiptypeDTO> lstPropertyownershiptypeDTO { get; set; }
        public List<propertytypeDTO> lstpropertytypeDTO { get; set; }
        public List<purposeDTO> lstpurposeDTO { get; set; }
        public List<propertystatusDTO> lstpropertystatusDTO { get; set; }
        ExistingLoanDetailsDTO lstApplicationExistingLoanDetailsDTO { get; set; }
        public List<EducationalQualificationDTO> lstEducationalQualificationDTO { get; set; }
        public List<EducationalFeeYearDTO> lstEducationalFeeYearDTO { get; set; }

        public List<EducationalFeeQualificationDTO> lstEducationalFeeQualificationDTO { get; set; }
        public List<ApplicantIdsDTO> lstApplicantIdsDTO { set; get; }
        ApplicationSecurityandCollateralDTO ApplicationSecurityandCollateralDTO { get; set; }

        ApplicationApplicantandOthersDTO ApplicationApplicantandOthersDTO { get; set; }
        public List<ApplicationKYCDocumentsDTO> lstApplicationKYCDocuments { get; set; }

        ApplicationKYCDocumentsDTO ApplicationKYCDocumentsDTO { get; set; }
        public List<ProducttypeDTO> lstProducttype { get; set; }
        public List<AcknowledgementDTO> lstAcknowledgement { get; set; }

        public FirstinformationController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        public List<EmployeeRoleDTO> EmployeeRoleList { get; set; }
        public List<GetLoandetailsDTO> lstGetLoandetails { get; set; }

        ApplicationReferencesDTO objReference = null;

        #region SaveApplication
        [Route("api/loans/Transactions/Firstinformation/Saveapplication")]
        [HttpPost]
        public IActionResult Saveapplication(ApplicationDTO Applicationlist)
        {
            string applicationid;
            List<string> lstdata = new List<string>();
            try
            {
                applicationid = Objfirstinformation.Saveapplication(Applicationlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            lstdata.Add(applicationid);
            return Ok(lstdata);
        }
        #endregion


        [Route("api/loans/masters/loanmaster/getfiLoanTypes")]
        [HttpGet]
        public IActionResult getLoanTypes()
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                lstLoanMasterdetails = Objfirstinformation.getfiLoanTypes(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanMasterdetails);


        }

        #region GetSchemenamescodes
        [Route("api/loans/Transactions/Firstinformation/GetSchemenamescodes")]
        [HttpGet]
        public IActionResult GetSchemenamescodes(long Loanid)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.GetSchemenamescodes(Con, Loanid);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }

        #endregion

        #region GetLoanpayin
        [Route("api/loans/Transactions/Firstinformation/GetLoanpayin")]
        [HttpGet]
        public IActionResult GetLoanpayin(long Loanid, string Contacttype, string Applicanttype, int schemeid)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.GetLoanpayin(Con, Loanid, Contacttype, Applicanttype, schemeid);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }

        [Route("api/loans/Transactions/Firstinformation/GetLoanInterestTypes")]
        [HttpGet]
        public IActionResult GetLoanInterestTypes(long Loanid, int schemeid, string Contacttype, string Applicanttype, string Loanpayin)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.GetLoanInterestTypes(Con, Loanid, schemeid, Contacttype, Applicanttype, Loanpayin);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }

        #endregion

        #region  GetInterestRateBasedOnLoanpayin
        [Route("api/loans/Transactions/Firstinformation/GetInterestRates")]
        [HttpPost]
        public IActionResult GetInterestRates(FirstinformationDTO firstinformationDTO)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.GetInterestRates(Con, firstinformationDTO);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }


        [Route("api/loans/Transactions/Firstinformation/getLoaninstalmentmodes")]
        [HttpGet]
        public IActionResult getLoaninstalmentmodes(string loanpayin, string interesttype)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.getLoaninstalmentmodes(Con, loanpayin, interesttype);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }

        [Route("api/loans/Transactions/Firstinformation/GetSurityapplicants")]
        [HttpGet]
        public IActionResult GetSurityapplicants(string contacttype)
        {
            lstSurityapplicants = new List<SurityApplicantsDTO>();
            try
            {
                lstSurityapplicants = Objfirstinformation.GetSurityapplicants(Con, contacttype);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstSurityapplicants);

        }

        [Route("api/loans/Transactions/Firstinformation/GetLoanMinandmaxAmounts")]
        [HttpGet]
        public IActionResult GetLoanMinandmaxAmounts(long Loanid, string Contacttype, string Applicanttype, string Loanpayin, int schemeid, string interesttype)
        {
            lstGetLoandetails = new List<GetLoandetailsDTO>();
            try
            {
                lstGetLoandetails = Objfirstinformation.GetLoanMinandmaxAmounts(Con, Loanid, Contacttype, Applicanttype, Loanpayin, schemeid, interesttype);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetLoandetails);

        }
        #endregion 

        #region GetAllLoandetails
        [Route("api/loans/Transactions/Firstinformation/GetAllLoandetails")]
        [HttpGet]
        public IActionResult GetAllLoandetails(string Applicationid)
        {
            lstFirstinformation = new List<FirstinformationDTO>();
            try
            {
                lstFirstinformation = Objfirstinformation.GetAllLoandetails(Applicationid, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstFirstinformation);

        }

        #endregion

        #region Getloandetails
        [Route("api/loans/Transactions/Firstinformation/Getloandetails")]
        [HttpGet]
        public IActionResult Getloandetails(string Applicationid)
        {
            lstFirstinformation = new List<FirstinformationDTO>();
            try
            {
                lstFirstinformation = Objfirstinformation.Getloandetails(Applicationid, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstFirstinformation);

        }

        #endregion


        #region AcknowledgementDetails
        [Route("api/loans/Transactions/Firstinformation/GetAcknowledgementDetails")]
        [HttpGet]
        public IActionResult GetAcknowledgementDetails()
        {
            lstAcknowledgement = new List<AcknowledgementDTO>();
            try
            {
                lstAcknowledgement = Objfirstinformation.GetAcknowledgementDetails(Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstAcknowledgement);

        }

        [Route("api/loans/Transactions/Firstinformation/SaveAcknowledgementDetails")]
        [HttpPost]
        public IActionResult SaveAcknowledgementDetails(AcknowledgementDTO acknowlwdgement)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.SaveAcknowledgementDetails(Con, acknowlwdgement);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

        #endregion


        #region GetApplicantCreditandkycdetails
        [Route("api/loans/Transactions/Firstinformation/GetApplicantCreditandkycdetails")]
        [HttpGet]
        public IActionResult GetApplicantCreditandkycdetails(string Applicationid, Int64 contactid)
        {
            ApplicationKYCDocumentsDTO = new ApplicationKYCDocumentsDTO();
            try
            {
                ApplicationKYCDocumentsDTO = Objfirstinformation.GetApplicantCreditandkycdetails(Applicationid, contactid, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(ApplicationKYCDocumentsDTO);

        }

        #endregion

        #region Saveapplicationsurityapplicantdetails
        [Route("api/loans/Transactions/Firstinformation/Saveapplicationsurityapplicantdetails")]
        [HttpPost]
        public IActionResult Saveapplicationsurityapplicantdetails(ApplicationApplicantandOthersDTO Applicationlist)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.Saveapplicationsurityapplicantdetails(Applicationlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion
        [Route("api/loans/Transactions/Firstinformation/Getsurietypersondetails")]
        [HttpGet]
        public IActionResult Getsurietypersondetails(string Applicationid)
        {
            ApplicationApplicantandOthersDTO = new ApplicationApplicantandOthersDTO();
            try
            {
                ApplicationApplicantandOthersDTO = Objfirstinformation.Getsurietypersondetails(Applicationid, Con);
                if (ApplicationApplicantandOthersDTO != null)
                {
                    return Ok(ApplicationApplicantandOthersDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #region Deletesueritydetails
        [Route("api/loans/Transactions/Firstinformation/Deletesueritydetails")]
        [HttpPost]
        public IActionResult Deletesueritydetails(string strapplictionid, string strconrefid, int Createdby)
        {
            bool isSaved;

            try
            {

                isSaved = Objfirstinformation.Deletesueritydetails(strapplictionid, strconrefid, Createdby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region Savekycandidentificationdocuments
        [Route("api/loans/Transactions/Firstinformation/Savekycandidentificationdocuments")]
        [HttpPost]
        public IActionResult Savekycandidentificationdocuments(ApplicationKYCDocumentsDTO Applicationlist)
        {
            string OldFullPath = string.Empty;
            string NewFullPath = string.Empty;
            bool isSaved;

            try
            {
                string OldFolder = "Upload";
                string NewFolder = "Original";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string OldPath = Path.Combine(webRootPath, OldFolder);
                string newPath = Path.Combine(webRootPath, NewFolder);
                if (!Directory.Exists(newPath))
                {

                    Directory.CreateDirectory(newPath);
                }
                for (int i = 0; i < Applicationlist.documentstorelist.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Applicationlist.documentstorelist[i].pDocStorePath))
                    {
                        OldFullPath = Path.Combine(OldPath, Applicationlist.documentstorelist[i].pDocStorePath);
                        NewFullPath = Path.Combine(newPath, Applicationlist.documentstorelist[i].pDocStorePath);
                        Applicationlist.documentstorelist[i].pDocStorePath = NewFullPath;
                        if (System.IO.File.Exists(OldFullPath))
                        {
                            System.IO.File.Move(OldFullPath, NewFullPath);
                        }

                        //System.IO.File.Copy(OldFullPath, NewFullPath, true);
                        //System.IO.File.Delete(OldFullPath);
                    }
                }
                for (int i = 0; i < Applicationlist.lstCreditscoreDetailsDTO.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath))
                    {
                        OldFullPath = Path.Combine(OldPath, Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath);
                        NewFullPath = Path.Combine(newPath, Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath);
                        Applicationlist.lstCreditscoreDetailsDTO[i].pCreditscorefilepath = NewFullPath;
                        if (System.IO.File.Exists(OldFullPath))
                        {
                            System.IO.File.Move(OldFullPath, NewFullPath);
                        }
                        //System.IO.File.Copy(OldFullPath, NewFullPath, true);
                        //System.IO.File.Delete(OldFullPath);
                    }
                }

                isSaved = Objfirstinformation.Savekycandidentificationdocuments(Applicationlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion



        #region Deletesueritydetails
        [Route("api/loans/Transactions/Firstinformation/Deletecreditandkycdetails")]
        [HttpPost]
        public IActionResult Deletecreditandkycdetails(string strapplictionid, string strconrefid, int Createdby)
        {
            bool isSaved;

            try
            {

                isSaved = Objfirstinformation.Deletecreditandkycdetails(strapplictionid, strconrefid, Createdby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion



        #region Security and Collateral
        [Route("api/loans/Transactions/Firstinformation/saveApplicationSecurityCollateral")]
        [HttpPost]
        public IActionResult saveApplicationSecurityCollateral(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO)
        {
            bool isSaved;
            try
            {
                string OldFullPath = string.Empty;
                string NewFullPath = string.Empty;
                string OldFolder = "Upload";
                string NewFolder = "Original";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string OldPath = Path.Combine(webRootPath, OldFolder);
                string newPath = Path.Combine(webRootPath, NewFolder);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                for (int i = 0; i < SecurityandCollateralDTO.ImMovablePropertyDetailsList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath))
                    {
                        OldFullPath = Path.Combine(OldPath, SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath);
                        NewFullPath = Path.Combine(newPath, SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath);
                        SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                for (int i = 0; i < SecurityandCollateralDTO.MovablePropertyDetailsList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath))
                    {
                        OldFullPath = Path.Combine(OldPath, SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath);
                        NewFullPath = Path.Combine(newPath, SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath);
                        SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                for (int i = 0; i < SecurityandCollateralDTO.SecuritychequesList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath))
                    {
                        OldFullPath = Path.Combine(OldPath, SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath);
                        NewFullPath = Path.Combine(newPath, SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath);
                        SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                for (int i = 0; i < SecurityandCollateralDTO.DepositsasLienList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath))
                    {
                        OldFullPath = Path.Combine(OldPath, SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath);
                        NewFullPath = Path.Combine(newPath, SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath);
                        SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                for (int i = 0; i < SecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath))
                    {
                        OldFullPath = Path.Combine(OldPath, SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath);
                        NewFullPath = Path.Combine(newPath, SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath);
                        SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                isSaved = Objfirstinformation.saveApplicationSecurityCollateral(SecurityandCollateralDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }


        [Route("api/loans/Transactions/Firstinformation/getSecurityCollateralDetails")]
        [HttpGet]
        public IActionResult getSecurityCollateralDetails(long applicationid, string strapplicationid)
        {
            ApplicationSecurityandCollateralDTO = new ApplicationSecurityandCollateralDTO();
            try
            {
                ApplicationSecurityandCollateralDTO = Objfirstinformation.getSecurityCollateralDetails(applicationid, strapplicationid, Con);
                if (ApplicationSecurityandCollateralDTO != null)
                {
                    return Ok(ApplicationSecurityandCollateralDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        #endregion

        #region SURITYAPPLICANTNAMESMaster    
        [Route("api/loans/Transactions/Firstinformation/SaveSurityApplicantnames")]
        [HttpPost]
        public bool SaveSurityApplicantnames(SurityApplicantsDTO _Surityapplicants)
        {
            try
            {
                return Objfirstinformation.SaveSurityApplicantnames(_Surityapplicants, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/loans/Transactions/Firstinformation/checkInsertSurityapplicantsDuplicates")]
        [HttpGet]
        public IActionResult checkInsertSurityapplicantsDuplicates(string Surityapplicanttype)
        {
            int count;
            try
            {
                count = Objfirstinformation.checkInsertSuritypplicantsDuplicates(Surityapplicanttype, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }
        #endregion

        #region HomeLoan masters
        /// <summary>
        /// Home Loan Masters Save Property Location
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveSavepropertylocation")]
        [HttpPost]
        public IActionResult SaveSavepropertylocation(PropertylocationDTO PropertylocationDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.Savepropertylocation(PropertylocationDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Save Property ownership Type
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Savepropertyownershiptype")]
        [HttpPost]
        public IActionResult Savepropertyownershiptype(PropertyownershiptypeDTO PropertyownershiptypeDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.Savepropertyownershiptype(PropertyownershiptypeDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Save Property  Type
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Savepropertytype")]
        [HttpPost]
        public IActionResult Savepropertytype(propertytypeDTO propertytypeDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.Savepropertytype(propertytypeDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Save Purpose
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Savepurpose")]
        [HttpPost]
        public IActionResult Savepurpose(purposeDTO purposeDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.Savepurpose(purposeDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Save Property Status
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Savepropertystatus")]
        [HttpPost]
        public IActionResult Savepropertystatus(propertystatusDTO propertystatusDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = Objfirstinformation.Savepropertystatus(propertystatusDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region Bind HomeLoan masters
        /// <summary>
        /// Home Loan Masters Bind Property Location
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Getpropertylocation")]
        [HttpGet]
        public IActionResult Getpropertylocation()
        {
            lstPropertylocationDTO = new List<PropertylocationDTO>();
            try
            {
                lstPropertylocationDTO = Objfirstinformation.BindPropertylocation(Con);
                if (lstPropertylocationDTO != null && lstPropertylocationDTO.Count > 0)
                {
                    return Ok(lstPropertylocationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Bind Property ownership Type
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetPropertyownershiptype")]
        [HttpGet]
        public IActionResult GetPropertyownershiptype()
        {
            lstPropertyownershiptypeDTO = new List<PropertyownershiptypeDTO>();
            try
            {
                lstPropertyownershiptypeDTO = Objfirstinformation.BindPropertyownershiptype(Con);
                if (lstPropertyownershiptypeDTO != null && lstPropertyownershiptypeDTO.Count > 0)
                {
                    return Ok(lstPropertyownershiptypeDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Bind Property Type
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Getpropertytype")]
        [HttpGet]
        public IActionResult Getpropertytype()
        {
            lstpropertytypeDTO = new List<propertytypeDTO>();
            try
            {
                lstpropertytypeDTO = Objfirstinformation.Bindpropertytype(Con);
                if (lstpropertytypeDTO != null && lstpropertytypeDTO.Count > 0)
                {
                    return Ok(lstpropertytypeDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Bind purpose
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Getpurpose")]
        [HttpGet]
        public IActionResult Getpurpose()
        {
            lstpurposeDTO = new List<purposeDTO>();
            try
            {
                lstpurposeDTO = Objfirstinformation.Bindpurpose(Con);
                if (lstpurposeDTO != null && lstpurposeDTO.Count > 0)
                {
                    return Ok(lstpurposeDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Home Loan Masters Bind Property Status
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/Getpropertystatus")]
        [HttpGet]
        public IActionResult Getpropertystatus()
        {
            lstpropertystatusDTO = new List<propertystatusDTO>();
            try
            {
                lstpropertystatusDTO = Objfirstinformation.Bindpropertystatus(Con);
                if (lstpropertystatusDTO != null && lstpropertystatusDTO.Count > 0)
                {
                    return Ok(lstpropertystatusDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion



        #region Bind Applicant Loan Details
        /// <summary>
        /// Fetch Loan  details Passing  string strapplictionid
        /// </summary>
        /// <returns></returns>
        [Route("api/loans/Transactions/Firstinformation/GetApplicantLoanSpecificDetails")]
        [HttpGet]
        public async Task<IActionResult> GetApplicantLoanSpecificDetails(string strapplictionid)
        {
            ApplicationLoanSpecificDTO = new ApplicationLoanSpecificDTO();
            try
            {
                ApplicationLoanSpecificDTO = await Objfirstinformation.GetApplicantLoanSpecificDetails(strapplictionid, Con);
                if (ApplicationLoanSpecificDTO != null)
                {
                    return Ok(ApplicationLoanSpecificDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion

        #region Existing Loan
        [Route("api/loans/Transactions/Firstinformation/SaveApplicationexistingloansDetails")]
        [HttpPost]
        public IActionResult SaveApplicationexistingloansDetails(ExistingLoanDetailsDTO objApplicationExistingLoanDetails)
        {
            bool isSaved;
            try
            {
                isSaved = Objfirstinformation.SaveApplicationexistingloansDetails(objApplicationExistingLoanDetails, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/loans/Transactions/Firstinformation/GetApplicationExistingLoanDetails")]
        [HttpGet]
        public IActionResult GetApplicationExistingLoanDetails(string contactreferenceid, string strapplicationid)
        {
            lstApplicationExistingLoanDetailsDTO = new ExistingLoanDetailsDTO();
            try
            {
                lstApplicationExistingLoanDetailsDTO = Objfirstinformation.GetApplicationExistingLoanDetails(contactreferenceid, strapplicationid, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstApplicationExistingLoanDetailsDTO);
        }
        #endregion

        #region Application Personal Details

        [Route("api/loans/Transactions/Firstinformation/GetEmployementRoles")]
        [HttpGet]
        public IActionResult GetEmployementRoles()
        {
            EmployeeRoleList = new List<EmployeeRoleDTO>();
            try
            {
                EmployeeRoleList = Objfirstinformation.GetEmployementRoles(Con);
                if (EmployeeRoleList != null)
                {
                    return Ok(EmployeeRoleList);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception)
            {
                // throw new FinstaAppException(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("api/loans/Transactions/Firstinformation/SaveApplicationPersonalInformation")]
        [HttpPost]
        public IActionResult SaveApplicationPersonalInformation(ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO)
        {
            string OldFullPath = string.Empty;
            string NewFullPath = string.Empty;
            bool isSaved = false;

            try
            {

                string OldFolder = "Upload";
                string NewFolder = "Original";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string OldPath = Path.Combine(webRootPath, OldFolder);
                string newPath = Path.Combine(webRootPath, NewFolder);
                if (!Directory.Exists(newPath))
                {

                    Directory.CreateDirectory(newPath);
                }
                for (int i = 0; i < ApplicationPersonalInformationDTO.PersonalNomineeList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ApplicationPersonalInformationDTO.PersonalNomineeList[i].pdocidproofpath))
                    {
                        OldFullPath = Path.Combine(OldPath, ApplicationPersonalInformationDTO.PersonalNomineeList[i].pdocidproofpath);
                        NewFullPath = Path.Combine(newPath, ApplicationPersonalInformationDTO.PersonalNomineeList[i].pdocidproofpath);
                        ApplicationPersonalInformationDTO.PersonalNomineeList[i].pdocidproofpath = NewFullPath;
                        System.IO.File.Move(OldFullPath, NewFullPath);
                        //System.IO.File.Copy(OldFullPath, NewFullPath, true);
                        //System.IO.File.Delete(OldFullPath);
                    }
                }


                isSaved = Objfirstinformation.SaveApplicationPersonalInformation(ApplicationPersonalInformationDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());

            }

        }
        /// <summary>
        /// Get Application Personal Information Based on  strapplictionid
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetApplicationPersonalInformation")]
        [HttpGet]
        public IActionResult GetApplicationPersonalInformation(string strapplictionid)
        {
            ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO = new ApplicationPersonalInformationDTO();
            try
            {
                ApplicationPersonalInformationDTO = Objfirstinformation.GetApplicationPersonalInformation(strapplictionid, Con);
                if (ApplicationPersonalInformationDTO != null)
                {
                    return Ok(ApplicationPersonalInformationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }




        /// <summary>
        /// Get Applicant Personal Loan Information Based on  strapplictionid
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetApplicationPersonalLoanInformation")]
        [HttpGet]
        public IActionResult GetApplicationPersonalLoanInformation(string strapplictionid)
        {
            ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO = new ApplicationPersonalInformationDTO();
            try
            {
                ApplicationPersonalInformationDTO = Objfirstinformation.GetApplicationPersonalLoanInformation(strapplictionid, Con);
                if (ApplicationPersonalInformationDTO != null)
                {
                    return Ok(ApplicationPersonalInformationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region Save Education Loan Masters
        /// <summary>
        /// Education Loan Masters Save Educational Qualification
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveEducationQualification")]
        [HttpPost]
        public IActionResult SaveEducationQualification(EducationalQualificationDTO EducationalQualificationDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.SaveEducationalQualification(EducationalQualificationDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        /// <summary>
        /// Education Loan Masters Fee   Save Educational Qualification
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveEducationFeeQualification")]
        [HttpPost]
        public IActionResult SaveEducationFeeQualification(EducationalFeeQualificationDTO EducationalFeeQualificationDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.SaveEducationalFeeQualification(EducationalFeeQualificationDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        /// <summary>
        /// Education Loan Masters Fee   Save Educational Year
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveEducationFeeYear")]
        [HttpPost]
        public IActionResult SaveEducationFeeYear(EducationalFeeYearDTO EducationalFeeYearDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.SaveEducationalFeeYear(EducationalFeeYearDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        /// <summary>
        /// Education Loan Masters   Bind Educational Qualification
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetEducationQualification")]
        [HttpGet]
        public IActionResult GetEducationQualification()
        {
            lstEducationalQualificationDTO = new List<EducationalQualificationDTO>();
            try
            {
                lstEducationalQualificationDTO = Objfirstinformation.BindEducationalQualification(Con);
                if (lstEducationalQualificationDTO != null && lstEducationalQualificationDTO.Count > 0)
                {
                    return Ok(lstEducationalQualificationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Education Loan Masters FEE  Bind YEAR
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetEducationalFeeYear")]
        [HttpGet]
        public IActionResult GetEducationalFeeYear()
        {
            lstEducationalFeeYearDTO = new List<EducationalFeeYearDTO>();
            try
            {
                lstEducationalFeeYearDTO = Objfirstinformation.BindEducationalFeeYear(Con);
                if (lstEducationalFeeYearDTO != null && lstEducationalFeeYearDTO.Count > 0)
                {
                    return Ok(lstEducationalFeeYearDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Education Loan Masters FEE  Bind Educational Qualification
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetEducationalFeeQualification")]
        [HttpGet]
        public IActionResult GetEducationalFeeQualification()
        {
            lstEducationalFeeQualificationDTO = new List<EducationalFeeQualificationDTO>();
            try
            {
                lstEducationalFeeQualificationDTO = Objfirstinformation.BindEducationalFeeQualification(Con);
                if (lstEducationalFeeQualificationDTO != null && lstEducationalFeeQualificationDTO.Count > 0)
                {
                    return Ok(lstEducationalFeeQualificationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Bind Application Ids Based on LoanType
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetApplicationIds")]
        [HttpGet]
        public IActionResult GetApplicationIds(string LoanType)
        {
            lstApplicantIdsDTO = new List<ApplicantIdsDTO>();
            try
            {
                lstApplicantIdsDTO = Objfirstinformation.BindApplicantIds(LoanType, Con);
                if (lstApplicantIdsDTO != null && lstApplicantIdsDTO.Count > 0)
                {
                    return Ok(lstApplicantIdsDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        /// Save Loan  details Passing  string strapplictionid
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveLoanSpecificDetails")]
        [HttpPost]
        public IActionResult SaveLoanSpecificDetails(string strapplictionid, ApplicationLoanSpecificDTO Applicationlist)
        {
            bool isSaved;
            string OldFullPath = string.Empty;
            string NewFullPath = string.Empty;

            try
            {
                if (Applicationlist.pLoantype.ToUpper().Trim() == "GOLD LOAN" || Applicationlist.pLoantype.ToUpper().Trim() == "LOAN AGAINST DEPOSITS" || Applicationlist.pLoantype.ToUpper().Trim() == "BUSINESS LOAN")
                {
                    string OldFolder = "Upload";
                    string NewFolder = "Original";
                    string webRootPath = _hostingEnvironment.ContentRootPath;
                    string OldPath = Path.Combine(webRootPath, OldFolder);
                    string newPath = Path.Combine(webRootPath, NewFolder);
                    if (!Directory.Exists(newPath))
                    {

                        Directory.CreateDirectory(newPath);
                    }
                    if (Applicationlist.pLoantype.ToUpper().Trim() == "GOLD LOAN")
                    {
                        for (int i = 0; i < Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO.Count; i++)
                        {
                            if (Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pTypeofoperation.ToUpper() == "CREATE")
                            {
                                if (!string.IsNullOrEmpty(Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath))
                                {
                                    OldFullPath = Path.Combine(OldPath, Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath);
                                    NewFullPath = Path.Combine(newPath, Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath);
                                    Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath = NewFullPath;
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                                else
                                {
                                    Applicationlist.lstGoldLoanDTO.lstGoldLoanDetailsDTO[i].pGoldArticlePath = NewFullPath;
                                }
                            }
                        }
                    }
                    if (Applicationlist.pLoantype.ToUpper().Trim() == "LOAN AGAINST DEPOSITS")
                    {

                        for (int i = 0; i < Applicationlist.lstLoanagainstDepositDTO.Count; i++)
                        {
                            if (Applicationlist.lstLoanagainstDepositDTO[i].pTypeofoperation.ToUpper() == "CREATE")
                            {
                                if (!string.IsNullOrEmpty(Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath))
                                {
                                    OldFullPath = Path.Combine(OldPath, Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath);
                                    NewFullPath = Path.Combine(newPath, Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath);
                                    Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath = NewFullPath;
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                                else
                                {
                                    Applicationlist.lstLoanagainstDepositDTO[i].pdepositdocpath = NewFullPath;
                                }
                            }
                        }
                    }
                    if (Applicationlist.pLoantype.ToUpper().Trim() == "BUSINESS LOAN")
                    {

                        for (int i = 0; i < Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO.Count; i++)
                        {
                            if (Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pTypeofoperation.ToUpper() == "CREATE")
                            {
                                if (!string.IsNullOrEmpty(Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath))
                                {
                                    OldFullPath = Path.Combine(OldPath, Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath);
                                    NewFullPath = Path.Combine(newPath, Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath);
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath = NewFullPath;
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                                else
                                {
                                    Applicationlist.BusinessLoanDTO.lstBusinessfinancialperformanceDTO[i].pbalancesheetdocpath = NewFullPath;
                                }
                            }
                        }
                    }

                }

                isSaved = Objfirstinformation.SaveLoanSpecificDetails(strapplictionid, Applicationlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        #endregion



        #region ApplicationReferences
        /// <summary>
        /// Save Application References
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveApplicationReferenceData")]
        [HttpPost]
        public IActionResult SaveApplicationReferenceData(long applicationid, string strapplictionid, ApplicationReferencesDTO LstobjReferenceData)
        {
            try
            {
                if (Objfirstinformation.SaveApplicationReferenceData(applicationid, strapplictionid, LstobjReferenceData, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Gets References data of an Application based on vchApplicationId and applicationid
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="vchapplicationID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/loans/Transactions/Firstinformation/GetApplicationReferenceData")]
        public IActionResult GetApplicationReferenceData(long applicationId, string vchapplicationID)
        {
            objReference = new ApplicationReferencesDTO();
            try
            {
                objReference = Objfirstinformation.GetApplicationReferenceData(applicationId, vchapplicationID, Con);
                if (objReference.LobjAppReferences != null && objReference.LobjAppReferences.Count > 0)
                {
                    return Ok(objReference);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Deletes or Creates References on Edit (no Update)
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="vchapplicationID"></param>
        /// <param name="objReferenceData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/loans/Transactions/Firstinformation/UpdateApplicationReferenceData")]
        public IActionResult UpdateApplicationReferenceData(long applicationId, string vchapplicationID, ApplicationReferencesDTO objReferenceData)
        {
            try
            {
                if (Objfirstinformation.UpdateApplicationReferenceData(applicationId, vchapplicationID, objReferenceData, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Consumer Loan
        /// <summary>
        /// Consumer Loan Master Save Product Types
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/SaveConsumableproductTypes")]
        [HttpPost]
        public IActionResult SaveConsumableproductTypes(ProducttypeDTO ProducttypeDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.SaveConsumableproduct(ProducttypeDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

        /// <summary>
        /// Consumer Loan Masters Product Type  Bind 
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetConsumableproductTypes")]
        [HttpGet]
        public IActionResult GetConsumableproductTypes()
        {
            lstProducttype = new List<ProducttypeDTO>();
            try
            {
                lstProducttype = Objfirstinformation.BindConsumableproduct(Con);
                if (lstProducttype != null && lstProducttype.Count > 0)
                {
                    return Ok(lstProducttype);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region LOANAGAINSTPROPERTYLOANSPECIFICFIELDS
        /// <summary>
        /// Consumer Loan Master Save Product Types
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/saveApplicationLoanAgainstpropertyLoanspecificfiels")]
        [HttpPost]
        public IActionResult saveApplicationLoanAgainstpropertyLoanspecificfiels(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.saveApplicationLoanAgainstpropertyLoanspecificfiels(SecurityandCollateralDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        #endregion

        #region PresonalLoanapi
        /// <summary>
        /// save Presonal Loan Loan Specific Api
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/saveApplicatntPersonalLoan")]
        [HttpPost]
        public IActionResult saveApplicatntPersonalLoan(ApplicationPersonalInformationDTO ApplicationPersonalInformationDTO)
        {
            bool isSaved;

            try
            {
                isSaved = Objfirstinformation.SaveApplicationPersonalLOANInformation(ApplicationPersonalInformationDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        #endregion

        /// <summary>
        /// Get Fi Document Verification Details
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetFiDocumentlDetails")]
        [HttpGet]
        public async Task<IActionResult> GetFiDocumentlDetails(string strapplicationid)
        {
            FIDocumentViewDTO FIDocumentViewDTO = new FIDocumentViewDTO();

            try
            {
                FIDocumentViewDTO = await Objfirstinformation.GetFIDocumentView(strapplicationid, Con);
                if (FIDocumentViewDTO != null)
                {
                    return Ok(FIDocumentViewDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }


        /// <summary>
        /// Get Firstinformation View Details
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetFirstInformationView")]
        [HttpGet]
        public async Task<IActionResult> GetFirstInformationView()
        {
            lstFirstinformation = new List<FirstinformationDTO>();
            try
            {
                lstFirstinformation = await Objfirstinformation.GetFirstInformationView(Con);
                if (lstFirstinformation != null)
                {
                    return Ok(lstFirstinformation);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        /// <summary>
        /// Get Firstinformation caluculate EMI
        /// </summary>
        [Route("api/loans/Transactions/Firstinformation/GetFiEmiSchesuleview")]
        [HttpGet]
        public async Task<IActionResult> GetFiEmiSchesuleview(decimal loanamount, string interesttype, string loanpayin, decimal interestrate, int tenureofloan, string Loaninstalmentmode, int emiprincipalpayinterval)
        {
            FirstinformationDTO = new FirstinformationDTO();
            try
            {
                FirstinformationDTO = await Objfirstinformation.GetFiEmiSchesuleview(loanamount, interesttype, loanpayin, interestrate, tenureofloan, Loaninstalmentmode, emiprincipalpayinterval, Con);
                if (FirstinformationDTO != null)
                {
                    return Ok(FirstinformationDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
    }
}