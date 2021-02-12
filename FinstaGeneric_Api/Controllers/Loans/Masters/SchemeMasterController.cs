using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Loans.Masters;
using FinstaRepository.Interfaces.Loans.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class SchemeMasterController : ControllerBase
    {
        ISchemeMaster obschememaster = new SchemeMasterDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public List<SchemeMasterDTO> lstSchememasterdetails { get; set; }
        public SchemeMasterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/loans/masters/schememaster/getSchemeMasterDetailsgrid")]
        [HttpGet]
        public IActionResult getSchemeMasterDetailsgrid()
        {
            lstSchememasterdetails = new List<SchemeMasterDTO>();
            try
            {
                lstSchememasterdetails = obschememaster.getSchemeMasterDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstSchememasterdetails);
        }


        [Route("api/loans/masters/schememaster/getSchemeMasterDetails")]
        [HttpGet]
        public IActionResult getSchemeMasterDetails(Int64 schemeid)
        {
            lstSchememasterdetails = new List<SchemeMasterDTO>();
            try
            {
                lstSchememasterdetails = obschememaster.getSchemeMasterDetails(schemeid,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstSchememasterdetails);
        }
        [Route("api/loans/masters/schememaster/getSchemeMasterDetailsbyId")]
        [HttpGet]
        public IActionResult getSchemeMasterDetailsbyId(Int64 schemeId, Int64 loanId)
        {
            SchemeMasterDTO Schememasterdetails = new SchemeMasterDTO();
            try
            {
                Schememasterdetails = obschememaster.getSchemeMasterDetailsbyId(schemeId, loanId, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(Schememasterdetails);
        }
        [Route("api/loans/masters/schememaster/getLoanspecificSchemeMasterDetails")]
        [HttpGet]
        public IActionResult getLoanspecificSchemeMasterDetails(Int64 loanid)
        {
            lstSchememasterdetails = new List<SchemeMasterDTO>();
            try
            {
                lstSchememasterdetails = obschememaster.getLoanspecificSchemeMasterDetails(Con, loanid);
                if (lstSchememasterdetails!=null && lstSchememasterdetails.Count>0)
                {
                    return Ok(lstSchememasterdetails);
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
        [Route("api/loans/masters/schememaster/saveSchemeMaster")]
        [HttpPost]
        public IActionResult saveSchemeMaster(SchemeMasterDTO schememasterlist)
        {
            bool isSaved = false;

            try
            {
                isSaved = obschememaster.saveSchemeMaster(schememasterlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #region CheckDuplicatechargeNames
        [Route("api/loans/masters/schememaster/CheckDuplicateSchemeNames")]
        [HttpGet]
        public int CheckDuplicateSchemeNames(string SchemeName)
        {     
            try
            {

               return obschememaster.CheckDuplicateSchemeNames(SchemeName, Con);          

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }


        [Route("api/loans/masters/schememaster/CheckDuplicateSchemeCodes")]
        [HttpGet]
        public int CheckDuplicateSchemeCodes(string schemecode)
        {
            try
            {

                return obschememaster.CheckDuplicateSchemeCodes(schemecode, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        [Route("api/loans/masters/schememaster/UpdateSchemeMaster")]
        [HttpPost]
        public IActionResult UpdateSchemeMaster(SchemeMasterDTO schememasterlist)
        {
            bool isSaved = false;

            try
            {
                isSaved = obschememaster.UpdateSchemeMaster(schememasterlist, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [Route("api/loans/masters/schememaster/DeleteSchemeMaster")]
        [HttpPost]
        public IActionResult DeleteSchemeMaster(SchemeMasterDTO schememasterlist)
        {
            bool isSaved = false;
            // lstLoanMasterdetails = new List<LoansMasterDTO>();

            Int64 schemeid = schememasterlist.pSchemeid;
            int createdby = schememasterlist.pCreatedby;
            try
            {
                isSaved = obschememaster.DeleteSchemeMaster(schemeid, createdby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
    }
}