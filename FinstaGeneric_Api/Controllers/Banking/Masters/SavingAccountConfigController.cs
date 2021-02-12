using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using FinstaInfrastructure.Banking.Masters;
using Microsoft.Extensions.Configuration;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Banking.Masters;
using FinstaApi.Common;
namespace FinstaApi.Controllers.Banking.Masters
{
    [Authorize]
    [ApiController]

    [EnableCors("CorsPolicy")]
    public class SavingAccountConfigController : ControllerBase
    {
        public List<SavingAccountConfigDTO> lstSavingAccConfigdetails { get; set; }
        public List<SavingAccountNameandCodeDTO> listSavingAccountNameandCodeDTO { get; set; }

        ISavingAccountConfig objSavingAccountConfig = new SavingAccountConfigDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;

        public SavingAccountConfigController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }


        [Route("api/banking/masters/savingAccountConfig/checkInsertAccNameandCodeDuplicates")]
        [HttpGet]
        public IActionResult checkInsertAccNameandCodeDuplicates(string checkparamtype, string Accname, string Acccode, Int64 SavingAccid)
        {
            int count = 0;
            lstSavingAccConfigdetails = new List<SavingAccountConfigDTO>();
            try
            {
                count = objSavingAccountConfig.checkInsertAccNameandCodeDuplicates(checkparamtype, Accname, Acccode, SavingAccid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        [HttpGet]
        [Route("api/banking/masters/savingAccountConfig/GetSavingAccNameCodeCount")]
        public IActionResult GetSavingAccNameCodeCount(Int64 SavingAccid, string SavingAccName, string SavingAccCode)
        {
            try
            {
                SavingschemeandcodeCount _SavingschemeandcodeCount = objSavingAccountConfig.GetSavingAccNameCodeCount(SavingAccid, SavingAccName, SavingAccCode, Con);
                if (_SavingschemeandcodeCount != null)
                {
                    return Ok(_SavingschemeandcodeCount);
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

        [Route("api/banking/masters/savingAccountConfig/saveAccountConfigNameAndCode")]
        [HttpPost]
        public IActionResult saveAccountConfigNameAndCode(SavingAccountNameandCodeDTO SavingAccountNameandCodelist)
        {
            int isSaved = 0;

            try
            {
                isSaved = objSavingAccountConfig.SaveSavingAccountNameAndCode(SavingAccountNameandCodelist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/banking/masters/savingAccountConfig/saveAccountConfig")]
        [HttpPost]
        public IActionResult saveAccountConfig(SavingAccountConfigDTO savingaccountconfiglist)
        {
            bool isSaved = false;

            try
            {
                isSaved = objSavingAccountConfig.SaveSavingAccountConfiguration(savingaccountconfiglist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/banking/masters/savingAccountConfig/SaveLoanFacility")]
        [HttpPost]
        public IActionResult SaveLoanFacility(LoanFacilityDTO LoanFacilitylist)
        {
            bool isSaved = false;

            try
            {
                isSaved = objSavingAccountConfig.SaveLoanFacility(LoanFacilitylist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/banking/masters/savingAccountConfig/SaveIdentificationdocuments")]
        [HttpPost]
        public IActionResult SaveIdentificationdocuments(IdentificationDocumentsDTO IdentificationDocumentslist)
        {
            bool isSaved = false;

            try
            {
                isSaved = objSavingAccountConfig.SaveIdentificationdocuments(IdentificationDocumentslist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/banking/masters/savingAccountConfig/SaveReferralCommission")]
        [HttpPost]
        public IActionResult SaveReferralCommission(ReferralCommissionDTO ReferralCommissionlist)
        {
            bool isSaved = false;

            try
            {
                isSaved = objSavingAccountConfig.SaveReferralCommission(ReferralCommissionlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }


        [HttpGet]
        [Route("api/banking/masters/savingAccountConfig/GetSavingAccountConfigData")]

        public async Task<IActionResult> GetSavingAccountConfigData()
        {
            listSavingAccountNameandCodeDTO = new List<SavingAccountNameandCodeDTO>();

            try
            {
                listSavingAccountNameandCodeDTO = await objSavingAccountConfig.GetSavingAccountConfigData(Con);
                if (listSavingAccountNameandCodeDTO.Count > 0)
                {
                    return Ok(listSavingAccountNameandCodeDTO);
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

        [Route("api/banking/masters/savingAccountConfig/GetAccountConfigDetails")]
        [HttpGet]
        public async Task<IActionResult>  GetAccountConfigDetails(Int64 SavingAccountId)
        {
            lstSavingAccConfigdetails = new List<SavingAccountConfigDTO>();
            try
            {
                lstSavingAccConfigdetails = await objSavingAccountConfig.GetSavingAccountConfigMasterDetails(Con, SavingAccountId);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstSavingAccConfigdetails);


        }
        [Route("api/banking/masters/savingAccountConfig/DeleteSavingAccountConfig")]
        [HttpPost]
        public IActionResult DeleteSavingAccountConfig(SavingAccountConfigDTO savingaccountconfiglist)
        {
            bool isSaved = false;
            // lstLoanMasterdetails = new List<LoansMasterDTO>();

            Int64 savingAccountid = savingaccountconfiglist.pSavingAccountid;
            int modifiedby = savingaccountconfiglist.pModifiedby;
            try
            {
                isSaved = objSavingAccountConfig.DeleteSavingAccountConfig(savingAccountid, modifiedby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
    }
}