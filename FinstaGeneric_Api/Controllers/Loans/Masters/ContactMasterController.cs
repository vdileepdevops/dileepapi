using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Loans.Masters;
using FinstaRepository.Interfaces.Loans.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using FinstaInfrastructure.Loans.Transactions;

namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ContactMasterController : ControllerBase
    {

        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;

        public ContactMasterController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }


        IContactMaster obj = new ContactMasterDAL();
        List<contactAddressDTO> lstcontactaddress { get; set; }
        List<EnterpriseTypeDTO> lstEnterprisetype { get; set; }
        List<BusinessTypeDTO> lstBusinessType { get; set; }
        List<ContactViewDTO> lstContactViewDTO { get; set; }
        ContactMasterDTO lstcontactMasterDetails { get; set; }
        public List<FirstinformationDTO> lstFirstinformationDTO { set; get; }

        [Route("api/loans/masters/contactmaster/FileUpload")]

        [HttpPost, DisableRequestSizeLimit]
        public bool Uploadphoto(ContactMasterDTO contact)
        {
            try
            {
                var bytes = Convert.FromBase64String(contact.pPhoto);// a.base64image 
                                                                     //or full path to file in temp location
                                                                     //var filePath = Path.GetTempFileName();

                // full path to file in current project location
                string filedir = Path.Combine(Directory.GetCurrentDirectory(), "ContactImages");
                Debug.WriteLine(filedir);
                Debug.WriteLine(Directory.Exists(filedir));
                if (!Directory.Exists(filedir))
                { //check if the folder exists;
                    Directory.CreateDirectory(filedir);
                }
                Guid obj = Guid.NewGuid();
                string file1 = Path.Combine(filedir, obj.ToString() + contact.pBusinesscontactName + ".jpg");
                contact.pContactimagepath = file1;
                Debug.WriteLine(file1);
                //Debug.WriteLine(File.Exists(file));
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file1, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Internal server error");
                //throw ex;
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/loans/masters/contactmaster/viewphoto")]
        [HttpGet]
        public IActionResult viewphoto(ContactMasterDTO contact)
        {
            string base64String = string.Empty;

            string filePath = string.Empty;
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(contact.pContactimagepath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes1 = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes1);
                        contact.pPhoto = base64String;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(base64String);
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

        [Route("api/loans/masters/contactmaster/ConvertImagepathtobase64")]
        [HttpGet]
        public IActionResult ConvertImagepathtobase64(string strPath)
        {
            string base64String = string.Empty;
            List<string> imagebase64stringlist = new List<string>();
            string filePath = string.Empty;
            try
            {
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
                //throw new FinstaAppException(ex.ToString());
                throw ex;
            }
            return Ok(imagebase64stringlist);
        }

        [Route("api/loans/masters/contactmaster/Savecontact")]
        [HttpPost]
        public bool Savecontact([FromBody]ContactMasterDTO contact)
        {
            bool issaved = false;
            bool isfileupload = false;
            try
            {
                if (!string.IsNullOrEmpty(contact.pPhoto))
                {
                    isfileupload = Uploadphoto(contact);

                }
                else
                {
                    isfileupload = true;
                }
                if (isfileupload == true)
                {
                    issaved = obj.Savecontact(contact, Con);
                }

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return issaved;
        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/ViewContact")]
        public ContactMasterDTO ViewContact(string refernceid)
        {
            lstcontactMasterDetails = new ContactMasterDTO();
            try
            {
                lstcontactMasterDetails = obj.ViewContact(refernceid, Con);
                if (!string.IsNullOrEmpty(lstcontactMasterDetails.pContactimagepath))
                    viewphoto(lstcontactMasterDetails);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return lstcontactMasterDetails;
        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/GetContactDetails")]

        public List<ContactMasterDTO> GetContactdetails(string Type)
        {
            try
            {
                return obj.GetContactdetails(Con, Type);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/loans/masters/contactmaster/UpdateContact")]
        public bool UpdateContact(ContactMasterDTO contact)
        {
            bool issaved = false;
            bool isfileupload = false;
            try
            {
                if (!string.IsNullOrEmpty(contact.pPhoto))
                {
                    isfileupload = Uploadphoto(contact);
                }
                else
                {
                    isfileupload = true;
                }
                if (isfileupload == true)
                {
                    issaved = obj.UpdateContact(contact, Con);
                }

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return issaved;
        }
        [Route("api/loans/masters/contactmaster/DeleteContact")]
        [HttpPost]
        public bool DeleteContact(ContactMasterDTO contact)
        {
            try
            {
                return obj.DeleteContact(contact, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/loans/masters/contactmaster/SaveAddressType")]
        [HttpPost]
        public bool SaveAddressType(contactAddressDTO _address)
        {
            try
            {
                return obj.SaveAddressType(_address, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/loans/masters/contactmaster/GetAddressType")]
        [HttpGet]
        public IActionResult GetAddressType(string contactype)
        {
            lstcontactaddress = new List<contactAddressDTO>();
            try
            {
                lstcontactaddress = obj.GetAddressType(contactype, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstcontactaddress);

        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/checkInsertAddressTypeDuplicates")]
        public IActionResult checkInsertAddressTypeDuplicates(string addresstype, string contactype)
        {
            int count = 0;

            try
            {
                count = obj.checkInsertAddressTypeDuplicates(addresstype, contactype, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }





        [Route("api/loans/masters/contactmaster/SaveEnterpriseType")]
        [HttpPost]
        public bool SaveEnterpriseType(EnterpriseTypeDTO _Enterprise)
        {
            try
            {
                return obj.SaveEnterpriseType(_Enterprise, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/GetEnterpriseType")]
        public IActionResult GetEnterpriseType()
        {
            lstEnterprisetype = new List<EnterpriseTypeDTO>();
            try
            {
                lstEnterprisetype = obj.GetEnterpriseType(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstEnterprisetype);

        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/checkInsertEnterpriseTypeDuplicates")]
        public IActionResult checkInsertEnterpriseTypeDuplicates(string enterprisetype)
        {
            int count = 0;
            try
            {
                count = obj.checkInsertEnterpriseTypeDuplicates(enterprisetype, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }




        [Route("api/loans/masters/contactmaster/SaveBusinessTypes")]
        [HttpPost]
        public bool SaveNatureofbusiness(BusinessTypeDTO _type)
        {
            try
            {
                return obj.SaveBusinessTypes(_type, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/loans/masters/contactmaster/GetBusinessTypes")]
        [HttpGet]
        public IActionResult GetBusinessTypes()
        {
            lstBusinessType = new List<BusinessTypeDTO>();
            try
            {
                lstBusinessType = obj.GetBusinessTypes(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstBusinessType);

        }



        [Route("api/loans/masters/contactmaster/checkInsertBusinessTypesDuplicates")]
        [HttpGet]
        public IActionResult checkInsertBusinessTypesDuplicates(string businesstype)
        {
            int count = 0;
            try
            {
                count = obj.checkInsertBusinessTypesDuplicates(businesstype, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        [Route("api/loans/masters/contactmaster/GetPersonCount")]
        [HttpPost]
        public int GetPersonCount(ContactMasterDTO ContactDto)
        {

            try
            {
                return obj.GetPersoncount(ContactDto, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        /// <summary>
        ///Get All Contact Details like CONTACTS,APPLICANTS,PARTIES,EMPLOYEES,REFERRALS,ADVOCATES
        /// </summary>
        [HttpGet]
        [Route("api/loans/masters/contactmaster/GetcontactviewByName")]
        public IActionResult GetcontactviewByName(string ViewName)
        {

            lstContactViewDTO = new List<ContactViewDTO>();
            try
            {
                lstContactViewDTO = obj.GetContactView(ViewName, Con);
                if (lstContactViewDTO.Count > 0)
                {
                    for (int i = 0; i < lstContactViewDTO.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstContactViewDTO[i].pImagePath))
                        {
                           
                            lstContactViewDTO[i].pImage= getConvertImagepathtobase64(lstContactViewDTO[i].pImagePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstContactViewDTO);

        }


    }
}