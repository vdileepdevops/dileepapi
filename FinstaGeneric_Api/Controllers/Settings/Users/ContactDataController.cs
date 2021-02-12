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
using FinstaInfrastructure.Loans.Transactions;

namespace FinstaApi.Controllers.Settings.Users
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ContactDataController : ControllerBase
    {
        IContactwiseData ContactWiseDAL = new ContactwiseDataDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ContactDataDTO ContactDataDTO { set; get; }
        public List<FirstinformationDTO> lstApplicatLoans { set; get; }
        public List<FirstinformationDTO> lstcoApplicatLoans { set; get; }
        public List<FirstinformationDTO> lstgurantorsApplicatLoans { set; get; }

        public ContactDataController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [NonAction]
        public List<string> getConvertImagepathtobase64(string strPath)
        {
            string base64String = string.Empty;
            List<string> imagebase64stringlist = new List<string>();
            string filePath = string.Empty;
            try
            {

                // strPath = "D:\\GITHUB\\FINSTA\\FinstaGeneric_Api\\ContactImages\\03d81568-f89a-469a-8d34-92f97ca4f433.jpg";
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(strPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes1 = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes1);
                        imagebase64stringlist.Add(base64String);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return imagebase64stringlist;
        }
        /// <summary>
        /// Gets  Contact Data Based On Contact Reference ID.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/ContactData/GetContactData")]
        public async Task<IActionResult> GetContactData(string ContactRefID)
        {
            ContactDataDTO = new ContactDataDTO();
            try
            {
                ContactDataDTO = await ContactWiseDAL.GetContactData(ContactRefID, Con);
                if (ContactDataDTO != null)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString( ContactDataDTO.ContactViewDTO.pImagePath)))
                    {
                        ContactDataDTO.ContactViewDTO.pImage = getConvertImagepathtobase64(Convert.ToString(ContactDataDTO.ContactViewDTO.pImagePath));
                    }
                }
                return ContactDataDTO != null ? Ok(ContactDataDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Gets  Contact Loans Based On Contact Reference ID.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/ContactData/GetContactDataDetails")]
        public async Task<IActionResult> GetContactDataDetails(string loaddataType, string ContactRefID)
        {
            ContactDataDTO = new ContactDataDTO();
            try
            {
                ContactDataDTO = await ContactWiseDAL.GetContactDataDetails(loaddataType, ContactRefID, Con);               
                return ContactDataDTO != null ? Ok(ContactDataDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }
}