using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Loans.Reports;
using FinstaRepository.Interfaces.Loans.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.Loans.Reports
{
    [Authorize]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class DisbursmentLetterController : ControllerBase
    {
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;
        IDisbursmentLetter _DisbursmentLetterAccess = new DisbursmentLetterDAL();
        public DisbursmentLetterDTO _DisbursmentLetterDTO { get; set; }
        public DisbursalLetterCount _DisbursalLetterCount { get; set; }

        public List<DisbursmentLetterDTO> _DisbursmentLetterDTOList { get; set; }

        public List<DisbursalLetterCount> _DisbursalLetterCountListDTO { get; set; }

        public DisbursmentLetterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Gets Main Grid Data of Disbursal Letter
        /// </summary>        
        /// <param name="Letterstatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDisbursalLetterMainData")]
        public async Task<IActionResult> GetDisbursalLetterMainData(string Letterstatus)
        {
            _DisbursmentLetterDTOList = new List<DisbursmentLetterDTO>();
            try
            {
                if (!string.IsNullOrEmpty(Letterstatus))
                {
                    _DisbursmentLetterDTOList = await _DisbursmentLetterAccess.GetDisbursalLetterMainData(Con, Letterstatus);
                    return _DisbursmentLetterDTOList != null && _DisbursmentLetterDTOList.Count > 0 ? Ok(_DisbursmentLetterDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Gets Count of Disbursal Letters Status Sent and Not Sent Count
        /// </summary>      
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDisbursementLettersCount")]
        public async Task<IActionResult> GetDisbursementLettersCount()
        {
            _DisbursalLetterCountListDTO = new List<DisbursalLetterCount>();
            try
            {
                _DisbursalLetterCountListDTO = await _DisbursmentLetterAccess.GetDisbursementLettersCount(Con);
                return _DisbursalLetterCountListDTO != null && _DisbursalLetterCountListDTO.Count > 0 ? Ok(_DisbursalLetterCountListDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Save's Disbursal Letter on Paassing  Disbursal Model Object
        /// </summary>
        /// <param name="_DisbursmentLetterDTO"></param>
        /// <returns></returns>
        [Route("api/Loans/Reports/SaveDisbursalLetter")]
        [HttpPost]
        public IActionResult SaveDisbursalLetter(DisbursmentLetterDTO _DisbursmentLetterDTO)
        {
            try
            {
                if (_DisbursmentLetterAccess.SavedisbursalLetter(_DisbursmentLetterDTO, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }


        /// <summary>
        /// Gets Disbursal Letters Data on passing Loan Account No.
        /// </summary>        
        /// <param name="VchapplicationId"></param>
        /// <param name="Voucherno"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDisbursalLetterData")]
        public async Task<IActionResult> GetDisbursalLetterData(string VchapplicationId, string Voucherno)
        {
            _DisbursmentLetterDTO = new DisbursmentLetterDTO();
            try
            {
                if (!string.IsNullOrEmpty(VchapplicationId))
                {
                    _DisbursmentLetterDTO = await _DisbursmentLetterAccess.GetDisbursalLetterData(Con, VchapplicationId, Voucherno);
                    return _DisbursmentLetterDTO != null ? Ok(_DisbursmentLetterDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);                
            }
        }
    }
}