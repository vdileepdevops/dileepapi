using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaRepository.DataAccess.Loans.Masters;
using FinstaRepository.Interfaces.Loans.Masters;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaInfrastructure.Settings;
using FinstaInfrastructure.Common;
using Microsoft.AspNetCore.Cors;
using System.Net;
using System.Net.Http;
using FinstaApi.Common;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Settings

{
    [Authorize]

    [ApiController]
    [EnableCors("CorsPolicy")]
    public class SettingsController : ControllerBase
    {
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public SettingsController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        ISettings obj = new SettingsDAL();
        List<SettingsDTO> lstlocations { get; set; }
        List<SettingsDTO> lsttitles { get; set; }

        List<SettingsDTO> lstCompanyDetails { get; set; }

        List<SettingsDTO> lstBranchDetails { get; set; }
        List<SettingsDTO> lstApplciantypes { get; set; }

        List<SettingsDTO> lstContacttypes { get; set; }
        List<documentstoreDTO> lstKYCDocuments { get; set; }
        List<GenerateidMasterDTO> lstFormnames { get; set; }
        List<GenerateidMasterDTO> lstmodeoftransactions { get; set; }

        List<GenerateidMasterDTO> lstGetGenerateidmasterList { get; set; }


        List<ReferralAdvocateDTO> lstReferralAdvocateDTO { get; set; }

        CompanyInfoDTO _CompanyInfoDTO { get; set; }

        [Route("api/Settings/getContacttitles")]
        [HttpGet]

        public List<SettingsDTO> getContacttitles()
        {
            lsttitles = new List<SettingsDTO>();
            try
            {
                lsttitles = obj.getContacttitles(Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lsttitles;

        }

        [Route("api/Settings/getCountries")]
        [HttpGet]

        public List<SettingsDTO> getCountries()
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                lstlocations = obj.getCountries(Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstlocations;

        }

        [Route("api/Settings/getStates")]
        [HttpGet]
        public List<SettingsDTO> getStates(int id)
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                lstlocations = obj.getStates(Con, id);

            }
            catch (Exception)
            {
                throw;
            }
            return lstlocations;

        }


        [Route("api/Settings/getDistricts")]
        [HttpGet]
        public List<SettingsDTO> getDistricts(int id)
        {
            lstlocations = new List<SettingsDTO>();
            try
            {
                lstlocations = obj.getDistricts(Con, id);

            }
            catch (Exception)
            {
                throw;
            }
            return lstlocations;

        }

        [Route("api/Settings/getCompanyandbranchdetails")]
        [HttpGet]

        public List<SettingsDTO> getCompanyandbranchdetails()
        {
            lstCompanyDetails = new List<SettingsDTO>();
            try
            {
                lstCompanyDetails = obj.getCompanyandbranchdetails(Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstCompanyDetails;

        }

        [Route("api/Settings/getApplicanttypes")]
        [HttpGet]
        public List<SettingsDTO> getApplicanttypes(string contacttype, long loanid)
        {
            lstApplciantypes = new List<SettingsDTO>();
            try
            {
                lstApplciantypes = obj.getApplicanttypes(contacttype, loanid, Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstApplciantypes;
        }

        /// <summary>
        /// To get contacttypes and loanid as 0 incase of no dependency
        /// </summary>
        /// <param name="loanid"></param>
        /// <returns></returns>
        [Route("api/Settings/getContacttypes")]
        [HttpGet]
        public List<SettingsDTO> getContacttypes(int loanid)
        {
            lstApplciantypes = new List<SettingsDTO>();
            try
            {
                lstContacttypes = obj.getContacttypes(loanid, Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstContacttypes;
        }

        /// <summary>
        /// Not Implemented As of now
        /// </summary>
        /// <param name="relationObj"></param>
        /// <returns></returns>
        //[Route("api/Settings/saveRelationShip")]
        //[HttpPost]
        //public IActionResult saveRelationShip(RelationShipDTO relationObj)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(relationObj.pRelationShipName))
        //        {
        //            //if (obj.saveRelationShip(relationObj, Con))
        //            //{
        //            //    return Ok(true);
        //            //}
        //            //else
        //            //{
        //            //    return StatusCode(StatusCodes.Status304NotModified);
        //            //}
        //            return StatusCode(StatusCodes.Status304NotModified);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status204NoContent);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //        throw;
        //    }
        //}
        [Route("api/Settings/getDocumentstoreDetails")]
        [HttpGet]
        public IActionResult getDocumentstoreDetails(Int64 pContactId, string strapplicationid)
        {
            lstKYCDocuments = new List<documentstoreDTO>();
            try
            {
                lstKYCDocuments = obj.getDocumentstoreDetails(Con, pContactId, strapplicationid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstKYCDocuments);
        }

        /// <summary>
        /// Get Company Name and Address Details (Address,Branch name,Gstin,Cin no) (for Reports)
        /// </summary>
        /// <returns></returns>
        [Route("api/Settings/GetcompanyNameandaddressDetails")]
        [HttpGet]
        public IActionResult GetcompanyNameandaddressDetails()
        {
            _CompanyInfoDTO = new CompanyInfoDTO();
            try
            {
                _CompanyInfoDTO = obj.GetcompanyNameandaddressDetails(Con);
                if (_CompanyInfoDTO != null)
                {
                    return Ok(_CompanyInfoDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }

        #region GetDatepickerEnableStatus
        [Route("api/Accounting/AccountingReports/GetDatepickerEnableStatus")]
        [HttpGet]
        public async Task<IActionResult> GetDatepickerEnableStatus()
        {
            Boolean DateStatus;
            try
            {
                DateStatus = await obj.GetDatepickerEnableStatus(Con);
                return Ok(DateStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        #endregion

        #region IdGeneration
        [Route("api/Settings/GetFormnames")]
        [HttpGet]
        public List<GenerateidMasterDTO> GetFormnames()
        {
            lstFormnames = new List<GenerateidMasterDTO>();
            try
            {
                lstFormnames = obj.GetFormNames(Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstFormnames;
        }


        [Route("api/Settings/GetModeofTransaction")]
        [HttpGet]
        public List<GenerateidMasterDTO> GetModeofTransaction(string Formname)
        {
            lstmodeoftransactions = new List<GenerateidMasterDTO>();
            try
            {
                lstmodeoftransactions = obj.GetModeofTransaction(Formname, Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstmodeoftransactions;
        }

        [Route("api/Settings/checkTransactionCodeExist")]
        //[HttpPost]
        //[HttpGet("{checkparamtype}/{loanname}/{loancode}")]
        [HttpGet]
        public IActionResult checkTransactionCodeExist(string TransactionCode, int Parentid)
        {
            int count = 0;

            try
            {
                count = obj.checkTransactionCodeExist(TransactionCode.ToUpper(), Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count);
        }

        [Route("api/Settings/SaveGenerateIdMaster")]
        [HttpPost]
        public IActionResult SaveGenerateIdMaster(GenerateIdDTO _GenerateIdDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = obj.SaveGenerateIdMaster(_GenerateIdDTO, Con);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/Settings/GetGenerateidmasterList")]
        [HttpGet]
        public List<GenerateidMasterDTO> GetGenerateidmasterList()
        {
            lstmodeoftransactions = new List<GenerateidMasterDTO>();
            try
            {
                lstmodeoftransactions = obj.GetGenerateidmasterList(Con);

            }
            catch (Exception)
            {
                throw;
            }
            return lstmodeoftransactions;
        }
        #endregion



        #region All-Party-Details
        /// <summary>
        /// Gets all Party Details on passing Party Type
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [Route("api/Settings/GetAllPartyDetails")]
        [HttpGet]
        public IActionResult GetAllPartyDetails(string Type)
        {
            lstReferralAdvocateDTO = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferralAdvocateDTO = obj.GetAllPartyDetails(Type, Con);
                if (lstReferralAdvocateDTO != null && lstReferralAdvocateDTO.Count > 0)
                {
                    return Ok(lstReferralAdvocateDTO);
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
        #endregion

        #region Date Lock
        [Route("api/Settings/GetDateLockStatus")]
        [HttpGet]
        public IActionResult GetDateLockStatus()
        {

            bool isSaved = false;
            try
            {
                isSaved = obj.GetDateLockStatus(Con);
                return Ok(isSaved);
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