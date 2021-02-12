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
    public class SanctionLetterController : ControllerBase
    {
        readonly string Con = string.Empty;        
        static IConfiguration _iconfiguration;
        ISanctionLetter _SanctionLetterAccess = new SanctionLetterDAL();
         public SanctionLetter _SanctionLetterDTO { get; set; }
        public SanctionLetterCounts _SanctionLetterCounts { get; set; }

        public List<SanctionLetter> _SanctionLetterDTOList { get; set; }

        public List<SanctionLetterCounts> _SanctionLetterCountsListDTO { get; set; }

        public SanctionLetterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;            
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Gets Sanction Letter Data on Passing Loan Account No.
        /// </summary>
        /// <param name="VchapplicationID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetSanctionLetterData")]
        public async Task<IActionResult> GetSanctionLetterData(string VchapplicationID)
        {
            _SanctionLetterDTO = new SanctionLetter();
            try
            {
                if (!string.IsNullOrEmpty(VchapplicationID))
                {
                    _SanctionLetterDTO = await _SanctionLetterAccess.GetSanctionLetterData(Con, VchapplicationID);
                    return _SanctionLetterDTO != null ? Ok(_SanctionLetterDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
        /// Gets Main Grid Data of Sanction Letter
        /// </summary>        
        /// <param name="Letterstatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetSanctionLetterMainData")]
        public async Task<IActionResult> GetSanctionLetterMainData(string Letterstatus)
        {
            _SanctionLetterDTOList = new List<SanctionLetter>();
            try
            {
                if (!string.IsNullOrEmpty(Letterstatus))
                {
                    _SanctionLetterDTOList = await _SanctionLetterAccess.GetSanctionLetterMainData(Con,  Letterstatus);
                    return _SanctionLetterDTOList != null  && _SanctionLetterDTOList .Count>0? Ok(_SanctionLetterDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
        /// Save's Sanction Letter on Paassing  SanctionLetter Model Object
        /// </summary>
        /// <param name="_SanctionLetter"></param>
        /// <returns></returns>
        [Route("api/Loans/Reports/Savesanctionletter")]
        [HttpPost]
        public IActionResult Savesanctionletter(SanctionLetter _SanctionLetter)
        {            
            try
            {
               if(_SanctionLetterAccess.Savesanctionletter(_SanctionLetter, Con))
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
        /// Gets Count of Sanction Letters Sent and Not Sent
        /// </summary>      
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetSanctionLettersCount")]
        public async Task<IActionResult> GetSanctionLettersCount()
        {
            _SanctionLetterDTOList = new List<SanctionLetter>();
            try
            {
                _SanctionLetterCountsListDTO = await _SanctionLetterAccess.GetSanctionLettersCount(Con);
                    return _SanctionLetterCountsListDTO != null && _SanctionLetterCountsListDTO.Count > 0 ? Ok(_SanctionLetterCountsListDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);                
               
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }
}