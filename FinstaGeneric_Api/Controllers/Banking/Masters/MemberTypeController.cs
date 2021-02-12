using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings.Users;
using FinstaRepository.Interfaces.Settings.Users;
using FinstaApi.Common;
using FinstaRepository.DataAccess.Settings.Users;
using System.Net;
using System.Net.Http;
using FinstaInfrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Banking.Masters;

namespace FinstaApi.Controllers.Banking.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class MemberTypeController : ControllerBase
    {
        IMemberType objmemtype = new MemberTypeDAL();       
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public MemberTypeController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Save's Member Type.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/MemberType/SaveMemberType")]
        public IActionResult SaveMemberType([FromBody]  MemberTypeDTO MemberTypeDTO)
        {
            try
            {              
                    if (objmemtype.SaveMemberType(MemberTypeDTO, Con))
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
                throw;
            }
        }
        /// <summary>
        /// Get Member Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/MemberType/GetMemberDetails")]
        public async Task<IActionResult> GetMemberDetails()
        {
            List<MemberTypeDTO> lstMemberTypeDTO = new List<MemberTypeDTO>();
            try
            {
                lstMemberTypeDTO =await objmemtype.GetMemberTypeDetails(Con);
                return lstMemberTypeDTO != null && lstMemberTypeDTO.Count > 0 ? Ok(lstMemberTypeDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Masters/MemberType/GetSavingMemberTypeDetails")]
        public async Task<IActionResult> GetSavingMemberTypeDetails()
        {
            List<MemberTypeDTO> lstMemberTypeDTO = new List<MemberTypeDTO>();
            try
            {
                lstMemberTypeDTO = await objmemtype.GetSavingMemberTypeDetails(Con);
                return lstMemberTypeDTO != null && lstMemberTypeDTO.Count > 0 ? Ok(lstMemberTypeDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Masters/MemberType/GetShareMemberTypeDetails")]
        public async Task<IActionResult> GetShareMemberTypeDetails()
        {
            List<MemberTypeDTO> lstMemberTypeDTO = new List<MemberTypeDTO>();
            try
            {
                lstMemberTypeDTO = await objmemtype.GetShareMemberTypeDetails(Con);
                return lstMemberTypeDTO != null && lstMemberTypeDTO.Count > 0 ? Ok(lstMemberTypeDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        ///Bind Member Types.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/MemberType/BindMemberTypes")]
        public async Task<IActionResult> BindMemberTypes()
        {
            List<MemberTypeDTO> lstMemberTypeDTO = new List<MemberTypeDTO>();
            try
            {
                lstMemberTypeDTO =await objmemtype.BindMemberType(Con);
                return lstMemberTypeDTO != null && lstMemberTypeDTO.Count > 0 ? Ok(lstMemberTypeDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        ///Update Member Type.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/MemberType/UpdateMemberType")]
        public IActionResult UpdateMemberType([FromBody]  MemberTypeDTO MemberTypeDTO)
        {
            try
            {
                if (objmemtype.UpdateMemberType(MemberTypeDTO, Con))
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
                throw;
            }
        }

        /// <summary>
        /// Delete Member Type.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/MemberType/DeleteMemberType")]
        public IActionResult DeleteMemberType(int MemberID)
        {
            try
            {
                if (objmemtype.DeleteMemberType(MemberID, Con))
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
                throw;
            }
        }

        /// <summary>
        /// Check Duplicate Member Type 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/MemberType/CheckDuplicateMemberType")]
        public IActionResult CheckDuplicateMemberType(string MemberType)
        {
            try
            {
                int count = objmemtype.GetMemberTypeCount(MemberType, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
           
        }
        [HttpGet]
        [Route("api/Banking/Masters/MemberType/CheckDuplicateMemberTypeCode")]
        public IActionResult CheckDuplicateMemberTypeCode(string MemberTypeCode)
        {
            try
            {
                int count = objmemtype.GetMembercodeCount(MemberTypeCode, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Masters/MemberType/CheckDuplicateMemberNameCode")]
        public IActionResult GetMemberNameCount(Int64 memberid, string MemberType, string MemberTypeCode)
        {
            try
            {
                MemberschemeandcodeCount _MemberschemeandcodeCount = objmemtype.GetMemberNameCount(memberid, MemberType, MemberTypeCode, Con);
                if (_MemberschemeandcodeCount != null)
                {
                    return Ok(_MemberschemeandcodeCount);
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
    }
}