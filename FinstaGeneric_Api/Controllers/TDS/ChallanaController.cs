using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.TDS;
using FinstaRepository.DataAccess.TDS;
using FinstaRepository.Interfaces.TDS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.TDS
{
    [ApiController]
    [EnableCors("CorsPolicy")]
   
    public class ChallanaController : ControllerBase
    {
        IChallana objChallana = new ChallanaDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ChallanaController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [HttpGet]
        [Route("api/TDS/Challana/GetChallanaDetails")]
        public async Task<IActionResult> GetChallanaDetails(string FromDate, string ToDate, string SectionName, string CompanyType, string PanType)
        {
            try
            {
                List<ChallanaDTO> _ChallanaDetails = await objChallana.GetChallanaDetails(Con, FromDate, ToDate, SectionName, CompanyType, PanType);
                if (_ChallanaDetails != null && _ChallanaDetails.Count > 0)
                {
                    return Ok(_ChallanaDetails);
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
        [HttpPost]
        [Route("api/TDS/Challana/SaveChallanaEntry")]
        public IActionResult SaveChallanaEntry(ChallanaEntryDTO _ChallanaEntryDTO)
        {
            bool isSaved = false;

            try
            {
                isSaved = objChallana.SaveChallanaEntry(_ChallanaEntryDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/TDS/Challana/GetChallanaNumbers")]
        [HttpGet]

        public IActionResult GetChallanaNumbers()
        {
            List<ChallanaNoDTO> ChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                ChallanaNumbers = objChallana.GetChallanaNumbers(Con);
                return ChallanaNumbers != null ? Ok(ChallanaNumbers) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [Route("api/TDS/Challana/GetChallanaEntryDetails")]
        [HttpGet]

        public IActionResult GetChallanaEntryDetails(string ChallanaNO)
        {
            List<ChallanaDetailsDTO> ChallanaDetailsList = new List<ChallanaDetailsDTO>();
            try
            {
                ChallanaDetailsList = objChallana.GetChallanaEntryDetails(Con, ChallanaNO);
                return ChallanaDetailsList != null ? Ok(ChallanaDetailsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [Route("api/TDS/Challana/SaveChallanaPayment")]
        [HttpPost]
        public IActionResult SaveChallanaPayment([FromBody] ChallanaPaymentDTO _ChallanaPaymentDTO)
        {
            bool isSaved = false;
            ChallanaPaymentSaveDTO objSave = new ChallanaPaymentSaveDTO();
            try
            {
                string paymentId = string.Empty;
                isSaved = objChallana.SaveChallanaPayment(_ChallanaPaymentDTO, Con, out paymentId);
                if (!string.IsNullOrEmpty(paymentId))
                {
                    objSave.pvoucherid = paymentId;


                }
                return Ok(objSave);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }



        }

        #region CIN Entry
        [Route("api/TDS/Challana/GetChallanaPaymentNumbers")]
        [HttpGet]

        public IActionResult GetChallanaPaymentNumbers()
        {
            List<ChallanaNoDTO> ChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                ChallanaNumbers = objChallana.GetChallanaPaymentNumbers(Con);
                return ChallanaNumbers != null ? Ok(ChallanaNumbers) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [Route("api/TDS/Challana/GetChallanaPaymentDetails")]
        [HttpGet]

        public IActionResult GetChallanaPaymentDetails(string ChallanaNO)
        {
            GetChallanaPaymentsDTO ChallanaDetailsList = new GetChallanaPaymentsDTO();
            try
            {
                ChallanaDetailsList = objChallana.GetChallanaPaymentDetails(Con, ChallanaNO);
                return ChallanaDetailsList != null ? Ok(ChallanaDetailsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }

        [Route("api/TDS/Challana/GetChallanaPaymentReport")]
        [HttpGet]

        public IActionResult GetChallanaPaymentReport(string ChallanaNO)
        {
            ChallanaPaymentReportDTO ChallanaPaymentReportList = new ChallanaPaymentReportDTO();
            try
            {
                ChallanaPaymentReportList = objChallana.GetChallanaPaymentReport(Con, ChallanaNO);
                return ChallanaPaymentReportList != null ? Ok(ChallanaPaymentReportList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/TDS/Challana/SaveCinEntry")]
        public IActionResult SaveCinEntry(CinEntryDTO _CinEntryDTO)
        {
            bool isSaved = false;

            try
            {
                isSaved = objChallana.SaveCinEntry(_CinEntryDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion
        #region reports
        [HttpGet]
        [Route("api/TDS/Challana/GetCinEntryReportByChallanaNo")]
        public async Task<IActionResult> GetCinEntryReportByChallanaNo(string ChallanaNO)
        {
            try
            {
                List<CinEntryReportDTO> CinEntryReportList = await objChallana.GetCinEntryReportByChallanaNo(Con, ChallanaNO);
                if (CinEntryReportList != null && CinEntryReportList.Count > 0)
                {
                    return Ok(CinEntryReportList);
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
        [HttpGet]
        [Route("api/TDS/Challana/GetCinEntryReportsBetweenDates")]
        public async Task<IActionResult> GetCinEntryReportsBetweenDates(string FromDate, string ToDate)
        {
            try
            {
                List<CinEntryReportDTO> CinEntryReportList = await objChallana.GetCinEntryReportsBetweenDates(Con, FromDate, ToDate);
                if (CinEntryReportList != null && CinEntryReportList.Count > 0)
                {
                    return Ok(CinEntryReportList);
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

        [Route("api/TDS/Challana/GetCinEntryChallanaNumbers")]
        [HttpGet]

        public IActionResult GetCinEntryChallanaNumbers()
        {
            List<ChallanaNoDTO> ChallanaNumbers = new List<ChallanaNoDTO>();
            try
            {
                ChallanaNumbers = objChallana.GetCinEntryChallanaNumbers(Con);
                return ChallanaNumbers != null ? Ok(ChallanaNumbers) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        #endregion
    }
}