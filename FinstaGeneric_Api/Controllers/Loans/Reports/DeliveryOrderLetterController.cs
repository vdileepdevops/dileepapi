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
    public class DeliveryOrderLetterController : ControllerBase
    {
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;
        IDeliveryOrderLetter _DeliveryOrderLetterAccess = new DeliveryOrderLetterDAL();
        public DeliveryOrderLetterDTO _DeliveryOrderLetterDTO { get; set; }
        public List<DeliveryOrderLetterDTO> _DeliveryOrderLetterDTOList { get; set; }
        public List<DeliveryOrdersCount> _DeliveryOrdersCountListDTO { get; set; }
        public DeliveryOrdersCount _DeliveryOrdersCount { get; set; }
        public DeliveryOrderLetterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Gets Main Grid Data of Delivery Order Letter
        /// </summary>        
        /// <param name="Letterstatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDeliveryOrderLetterMainData")]
        public async Task<IActionResult> GetDeliveryOrderLetterMainData(string Letterstatus)
        {
            _DeliveryOrderLetterDTOList = new List<DeliveryOrderLetterDTO>();
            try
            {
                if (!string.IsNullOrEmpty(Letterstatus))
                {
                    _DeliveryOrderLetterDTOList = await _DeliveryOrderLetterAccess.GetDeliveryOrderLetterMainData(Con, Letterstatus);
                    return _DeliveryOrderLetterDTOList != null && _DeliveryOrderLetterDTOList.Count > 0 ? Ok(_DeliveryOrderLetterDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
        /// Save's Delivery Order Letter on Paassing  Model Object
        /// </summary>
        /// <param name="_SanctionLetter"></param>
        /// <returns></returns>
        [Route("api/Loans/Reports/Savedeliveryorderletter")]
        [HttpPost]
        public IActionResult Savedeliveryorderletter(DeliveryOrderLetterDTO _SanctionLetter)
        {
            try
            {
                if (_DeliveryOrderLetterAccess.Savedeliveryorderletter(_SanctionLetter, Con))
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
        /// Gets Delivery Order Letter Data on Passing Loan account No.
        /// </summary>
        /// <param name="VchapplicationID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDeliveryOrderLetterData")]
        public async Task<IActionResult> GetDeliveryOrderLetterData(string VchapplicationID)
        {
            _DeliveryOrderLetterDTO = new DeliveryOrderLetterDTO();
            try
            {
                if (!string.IsNullOrEmpty(VchapplicationID))
                {
                    _DeliveryOrderLetterDTO = await _DeliveryOrderLetterAccess.GetDeliveryOrderLetterData(Con, VchapplicationID);
                    return _DeliveryOrderLetterDTO != null ? Ok(_DeliveryOrderLetterDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
        /// Gets Count of Sanction Letters Sent and Not Sent Count
        /// </summary>      
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDeliveryOrdersCount")]
        public async Task<IActionResult> GetDeliveryOrdersCount()
        {
            _DeliveryOrdersCountListDTO = new List<DeliveryOrdersCount>();
            try
            {
                _DeliveryOrdersCountListDTO = await _DeliveryOrderLetterAccess.GetDeliveryOrdersCount(Con);
                return _DeliveryOrdersCountListDTO != null && _DeliveryOrdersCountListDTO.Count > 0 ? Ok(_DeliveryOrdersCountListDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }
}