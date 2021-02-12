using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
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
  //  [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class VerificationController : ControllerBase
    {
        IVerification objverification = new Verification();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;      
        List<VerificationDetailsDTO> lstVerificationDetailsDTO { set; get; }
        public ApplicationLoanSpecificDTOinVerification _ApplicationLoanSpecificDTOinVerification { get; set; }
        public VerificationController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region Save verification
        /// <summary>
        ///Save Verification Details
        /// </summary>
        [Route("api/loans/Transactions/Verification/SaveVerificationDetails")]
        [HttpPost]
        public async Task<IActionResult> SaveVerificationDetails(TeleVerificationDTO TeleVerificationDTO)
        {
            bool isSaved=false;
            try
            {
                isSaved =await objverification.SaveVerficationDetails(TeleVerificationDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        /// <summary>
        ///Save FI Verification Details
        /// </summary>
        [Route("api/loans/Transactions/Verification/SaveFIVerificationDetails")]
        [HttpPost]
        public async Task<IActionResult> SaveFIVerificationDetails(FIDocumentViewDTO FIDocumentViewDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved =await objverification.SaveFIverification(FIDocumentViewDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        /// <summary>
        ///Save field verification Details
        /// </summary>
        [Route("api/loans/Transactions/Verification/Savefieldverification")]
        [HttpPost]
        public async Task<IActionResult> Savefieldverification(FieldverificationDTO FieldverificationDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved =await objverification.SaveFieldverification(FieldverificationDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        /// <summary>
        ///Bind Verification Details Based on strapplicationid
        /// </summary>
        [Route("api/loans/Transactions/Verification/GetVerificationDetails")]
        [HttpGet]
        public async Task<IActionResult> GetVerificationDetails(string strapplicationid)
        {
            TeleVerificationDTO TeleVerificationDTO = new TeleVerificationDTO();
            try
            {
                TeleVerificationDTO =await objverification.GetVerficationDetails(strapplicationid,Con);
                if (TeleVerificationDTO != null )
                {
                    return Ok(TeleVerificationDTO);
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
        ///Bind Field Verification Details Based on strapplicationid
        /// </summary>
        [Route("api/loans/Transactions/Verification/GetFieldVerificationDetails")]
        [HttpGet]
        public async Task<IActionResult> GetFieldVerificationDetails(string strapplicationid)
        {
            FieldverificationDTO FieldverificationDTO = new FieldverificationDTO();
            try
            {
                FieldverificationDTO =await objverification.GetFieldVerificationDetails(strapplicationid, Con);

                if (FieldverificationDTO != null)
                {
                    if(FieldverificationDTO.FieldVerifyneighbourcheckDTO.Count>0)
                    {
                        for (int i = 0; i < FieldverificationDTO.FieldVerifyneighbourcheckDTO.Count; i++)
                        {
                           if(!string.IsNullOrEmpty(FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince))
                            {
                                    string Value = FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince;
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingsince = Value.Split('-')[0];
                                    FieldverificationDTO.FieldVerifyneighbourcheckDTO[i].papplicantisstayingMontsOrYears = Value.Split('-')[1];                              
                            }
                        }
                    }
                    return Ok(FieldverificationDTO);
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
        /// Get All Applicant Verification Status Details
        /// </summary>
        [Route("api/loans/Transactions/Verification/GetAllApplicantVerificationDetails")]
        [HttpGet]
        public async Task<IActionResult> GetAllApplicantVerificationDetails()
        {
            lstVerificationDetailsDTO = new List<VerificationDetailsDTO>();
            try
            {
                lstVerificationDetailsDTO =await objverification.GetAllApplicantVerificationDetails(Con);
                if (lstVerificationDetailsDTO != null && lstVerificationDetailsDTO.Count > 0)
                {
                    return Ok(lstVerificationDetailsDTO);
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
        ///Bind Applicant Details Based On ContactRefID
        /// </summary>
        [Route("api/loans/Transactions/Verification/GetApplicantDetails")]
        [HttpGet]
        public async Task<IActionResult> GetApplicantDetails(string strcontactrefid)
        {
            AddressconfirmedDTO AddressconfirmedDTO = new AddressconfirmedDTO();
          
            try
            {
                AddressconfirmedDTO =await objverification.GetDetailsOfApplicant(strcontactrefid,Con);
                if (AddressconfirmedDTO != null)
                {
                    return Ok(AddressconfirmedDTO);
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


        #region Verification-Loan-Specific-Data

        /// <summary>
        /// Fetch Loan  details Passing  string strapplictionid
        /// </summary>
        /// <returns></returns>
        [Route("api/loans/Transactions/Verification/GetApplicantLoanSpecificDetailsinVerification")]
        [HttpGet]
        public async Task<IActionResult> GetApplicantLoanSpecificDetailsinVerification(string strapplictionid)
        {
            _ApplicationLoanSpecificDTOinVerification = new ApplicationLoanSpecificDTOinVerification();
            try
            {
                _ApplicationLoanSpecificDTOinVerification = await objverification.GetApplicantLoanSpecificDetailsinVerification(strapplictionid, Con);
                if (_ApplicationLoanSpecificDTOinVerification != null)
                {
                    return Ok(_ApplicationLoanSpecificDTOinVerification);
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
    }
}