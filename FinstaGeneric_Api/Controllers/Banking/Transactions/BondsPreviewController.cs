using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Transactions
{
    // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BondsPreviewController : ControllerBase
    {
        IBondsPreview objBondprview= new BondsPreviewDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public BondsPreviewController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get Bonds Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/BondsPreview/GetBondsDetails")]
        public async Task<IActionResult> GetBondsDetails()
        {
            List<GetPreviewDetailsDTO> lstBondsDetails = new List<GetPreviewDetailsDTO>();
            try
            {
                lstBondsDetails = await objBondprview.GetBondsDetails(Con);
                return lstBondsDetails != null ? Ok(lstBondsDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        
        /// <summary>
        /// Get Bond Preview details list data
        /// </summary>
        /// <param name="fdaccountnos"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/BondsPreview/GetBondsPreviewDetails")]
        public async Task<IActionResult> GetBondsPreviewDetails(string fdaccountnos)
        {           
            try
            {
                BondspreviewMain BondsPreviewDetails = await objBondprview.GetBondsPreviewDetails(fdaccountnos, Con);
                return BondsPreviewDetails != null ? Ok(BondsPreviewDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Save Bonds Print
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Transactions/BondsPreview/SaveBondsPrint")]
        public IActionResult SaveBondsPrint([FromBody] Bonds_PrintDTO ObjBondsprint)
        {
            try
            {
                if (objBondprview.SaveBondsPrint(ObjBondsprint, Con))
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
    }
}