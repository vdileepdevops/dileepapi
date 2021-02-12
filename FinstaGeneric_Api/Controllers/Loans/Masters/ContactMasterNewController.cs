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
using System.Net;
using FinstaInfrastructure;
using FinstaInfrastructure.Settings;

namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ContactMasterNewController : ControllerBase
    {
        IContactMasterNew obj = new ContactMasterNewDAL();
        ContactMasterNewDTO lstcontactMasterDetails { get; set; }
        List<contactAddressNewDTO> lstcontactaddress { get; set; }
        List<EnterpriseTypeNewDTO> lstEnterprisetype { get; set; }
        List<BusinessTypeNewDTO> lstBusinessType { get; set; }
        List<ContactViewNewDTO> lstContactViewDTO { get; set; }
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ContactMasterNewController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [Route("api/loans/masters/contactmasterNew/FileUpload")]

        [HttpPost, DisableRequestSizeLimit]
        public bool Uploadphoto(ContactMasterNewDTO contact)
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

        [Route("api/loans/masters/contactmasterNew/viewphoto")]
        [HttpGet]
        public IActionResult viewphoto(ContactMasterNewDTO contact)
        {
            string base64String = string.Empty;

            string filePath = string.Empty;
            try
            {
                string ThumbnailFolder = "Upload\\";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(contact.pContactimagepath));
                if (System.IO.File.Exists(ThumbnailFullPath)) {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(ThumbnailFullPath))
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
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(base64String);
        }
        [NonAction]
        public IActionResult viewphoto1(ContactViewNewDTO contact)
        {
            string base64String = string.Empty;

            string filePath = string.Empty;
            try
            {
                string ThumbnailFolder = "Upload\\";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(contact.pImagePath));
                if (System.IO.File.Exists(ThumbnailFullPath))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(ThumbnailFullPath))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes1 = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes1);
                            contact.pImage = base64String;
                        }
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

                // strPath = "D:\\GITHUB\\Growork\\GroworkGeneric_Api\\ContactImages\\03d81568-f89a-469a-8d34-92f97ca4f433.jpg";
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

        [Route("api/loans/masters/contactmasterNew/ConvertImagepathtobase64")]
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
                //throw new GroworkAppException(ex.ToString());
            }
            return Ok(imagebase64stringlist);
        }

        [Route("api/loans/masters/contactmasterNew/Savecontact")]
        [HttpPost]
        public IActionResult Savecontact([FromBody]ContactMasterNewDTO contact)
        {
            bool issaved = false;
            //bool isfileupload = false;
            List<string> lstdata = new List<string>();
            try
            {
              
                string contactId = string.Empty;

                issaved = obj.Savecontact(contact, out contactId, Con);
                lstdata.Add(issaved.ToString().ToUpper());
                lstdata.Add(contactId);
                return Ok(lstdata);
                // return Ok(issaved);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }


        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/ViewContact")]
        public ContactMasterNewDTO ViewContact(string refernceid)
        {
            lstcontactMasterDetails = new ContactMasterNewDTO();
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
        [Route("api/loans/masters/contactmasterNew/GetContactDetails")]

        public List<ContactMasterNewDTO> GetContactdetails(string Type)
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

        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetSubContactdetails")]

        public List<SubscriberContactDTO> GetSubContactdetails(string Type)
        {
            try
            {
                return obj.GetSubContactdetails(Con, Type);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/loans/masters/contactmasterNew/UpdateContact")]
        public IActionResult UpdateContact([FromBody]ContactMasterNewDTO contact)
        {
            bool issaved = false;
            
            List<string> lstdata = new List<string>();
            try
            {
             
                string contactId = string.Empty;

                issaved = obj.UpdateContact(contact, out contactId, Con);
                lstdata.Add(issaved.ToString().ToUpper());
                lstdata.Add(contactId);
                return Ok(lstdata);
                // return Ok(issaved);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/loans/masters/contactmaster/GetContactdetailsByMobileNo")]

        public List<ContactMasterNewDTO> GetContactdetailsByMobileNo(string pMobileNo)
        {
            try
            {
                return obj.GetContactdetailsByMobileNo(Con, pMobileNo);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetDesignations")]

        public List<DesignationDTO> GetDesignations()
        {
            List<DesignationDTO> _lstDesignation = new List<DesignationDTO>();
            try
            {
                _lstDesignation = obj.GetDesignations(Con);
                return _lstDesignation;
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }

        [Route("api/loans/masters/contactmasterNew/DeleteContact")]
        [HttpPost]
        public bool DeleteContact(ContactMasterNewDTO contact)
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

        [Route("api/loans/masters/contactmasterNew/SaveAddressType")]
        [HttpPost]
        public bool SaveAddressType(contactAddressNewDTO _address)
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
       
        [Route("api/loans/masters/contactmasterNew/GetAddressType")]
        [HttpGet]
        public IActionResult GetAddressType(string contactype)
        {
            lstcontactaddress = new List<contactAddressNewDTO>();
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
        [Route("api/loans/masters/contactmasterNew/checkInsertAddressTypeDuplicates")]
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
            return Ok(count);
        }


        [Route("api/loans/masters/contactmasterNew/SaveEnterpriseType")]
        [HttpPost]
        public bool SaveEnterpriseType(EnterpriseTypeNewDTO _Enterprise)
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
        [Route("api/loans/masters/contactmasterNew/GetEnterpriseType")]
        public IActionResult GetEnterpriseType()
        {
            lstEnterprisetype = new List<EnterpriseTypeNewDTO>();
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
        [Route("api/loans/masters/contactmasterNew/checkInsertEnterpriseTypeDuplicates")]
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
            return Ok(count); 
        }


        [Route("api/loans/masters/contactmasterNew/SaveBusinessTypes")]
        [HttpPost]
        public bool SaveNatureofbusiness(BusinessTypeNewDTO _type)
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

        [Route("api/loans/masters/contactmasterNew/GetBusinessTypes")]
        [HttpGet]
        public IActionResult GetBusinessTypes()
        {
            lstBusinessType = new List<BusinessTypeNewDTO>();
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



        [Route("api/loans/masters/contactmasterNew/checkInsertBusinessTypesDuplicates")]
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
        [Route("api/loans/masters/contactmasterNew/GetPersonCount")]
        [HttpPost]
        public int GetPersonCount(ContactMasterNewDTO ContactDto)
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
        [Route("api/loans/masters/contactmasterNew/GetContactCount")]
        [HttpGet]
        public int GetContactCount(string ViewName, string searchby)
        {

            try
            {
                return obj.GetContactCount(ViewName, searchby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetcontactviewByName")]
        public IActionResult GetcontactviewByName(string ViewName, string endindex, string searchby)
        {

            lstContactViewDTO = new List<ContactViewNewDTO>();
            try
            {
                lstContactViewDTO = obj.GetContactView(ViewName, endindex, searchby, Con);
                if (lstContactViewDTO.Count > 0)
                {
                    for (int i = 0; i < lstContactViewDTO.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstContactViewDTO[i].pImagePath))
                        {
                            string ThumbnailFolder = "Upload\\";
                            string webRootPath = _hostingEnvironment.ContentRootPath;
                            string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                            string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(lstContactViewDTO[i].pImagePath));
                            if (System.IO.File.Exists(ThumbnailFullPath))
                            {
                                lstContactViewDTO[i].pImage = getConvertImagepathtobase64(ThumbnailFullPath);
                            }
                          
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

        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/Getcontactview")]
        public IActionResult Getcontactview(string ViewName)
        {

            lstContactViewDTO = new List<ContactViewNewDTO>();
            try
            {
                lstContactViewDTO = obj.GetContactViewdata(ViewName, Con);
                if (lstContactViewDTO.Count > 0)
                {
                    for (int i = 0; i < lstContactViewDTO.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstContactViewDTO[i].pImagePath))
                        {
                            string ThumbnailFolder = "Upload\\";
                            string webRootPath = _hostingEnvironment.ContentRootPath;
                            string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                            string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(lstContactViewDTO[i].pImagePath));
                            if (System.IO.File.Exists(ThumbnailFullPath))
                            {
                                lstContactViewDTO[i].pImage = getConvertImagepathtobase64(ThumbnailFullPath);
                            }

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
        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetContactViewbyid")]
        public IActionResult GetContactViewbyid(string refid)
        {

            ContactViewNewDTO lstContactViewDTO = new ContactViewNewDTO();
            try
            {
                lstContactViewDTO = obj.GetContactViewbyid(Con, refid);
                if (!string.IsNullOrEmpty(lstContactViewDTO.pContactimagepath))
                    viewphoto1(lstContactViewDTO);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstContactViewDTO);

        }
        [Route("api/loans/masters/contactmasterNew/SaveContactEmployee")]
        [HttpPost]
        public IActionResult SaveContactEmployee([FromBody]ContactEmployeeDTO EmployeeDTO)
        {
            try
            {
                if (obj.SaveContactEmployee(Con, EmployeeDTO))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/ViewEmployeeContactDetails")]
        public async Task<ContactEmployeeDTO> ViewEmployeeContactDetails(string refernceid)
        {

            try
            {
                ContactEmployeeDTO EmployeeDTO = new ContactEmployeeDTO();
                EmployeeDTO = await obj.GetEmployeedeatils(Con, refernceid);
                return EmployeeDTO;

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/ViewReferralContactDetails")]
        public async Task<ReferralDTO> ViewReferralContactDetails(string refernceid)
        {

            try
            {
                ReferralDTO ReferralDTO = new ReferralDTO();
                ReferralDTO = await obj.GetReferraldeatils(Con, refernceid);

                return ReferralDTO;

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/loans/masters/contactmasterNew/ViewQualificationDetails")]
        [HttpGet]
        public async Task<IActionResult> ViewQualificationDetails()
        {
            List<QualificationDTO> QualificationList = new List<QualificationDTO>();
            try
            {
                QualificationList = await obj.ViewQualificationDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(QualificationList);
        }
        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetInterducedDetails")]
        public async Task<IActionResult> GetInterducedDetails(string searchtype)
        {
            List<IntroducedDTO> _lstIntroduced = new List<IntroducedDTO>();

            try
            {
                _lstIntroduced = await obj.GetInterducedDetails(Con);
                return _lstIntroduced != null ? Ok(_lstIntroduced) : (IActionResult)StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }
        [Route("api/loans/masters/contactmasterNew/SaveContactReferral")]
        [HttpPost]
        public IActionResult SaveContactReferral([FromBody]ReferralDTO ReferralDTO)
        {
            try
            {
                if (obj.SaveContactReferral(Con, ReferralDTO))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/loans/masters/contactmasterNew/SaveContactSupplier")]
        [HttpPost]
        public IActionResult SaveContactSupplier([FromBody]SupplierDTO SupplierDTO)
        {
            try
            {
                if (obj.SaveContactSupplier(Con, SupplierDTO))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/loans/masters/contactmaster/getRelationShip")]
        [HttpGet]
        public async Task<IActionResult> getRelationShip()
        {
            try
            {
                List<RelationShipNewDTO> _lstRelationShip = await obj.getRelationShip(Con);
                return Ok(_lstRelationShip);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/loans/masters/contactmasterNew/SaveContactAdvocate")]
        [HttpPost]
        public IActionResult SaveContactAdvocate([FromBody]AdvocateDTO AdvocateDTO)
        {
            try
            {
                if (obj.SaveContactAdvocate(Con, AdvocateDTO))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }

        [Route("api/loans/masters/contactmasterNew/CheckDocumentExist")]
        //[HttpPost]
        //[HttpGet("{checkparamtype}/{loanname}/{loancode}")]
        [HttpGet]
        public IActionResult CheckDocumentExist(Int32 DocumentId, string ReferenceNo)
        {
            int count = 0;

            try
            {
                count = obj.CheckDocumentExist(DocumentId, ReferenceNo, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        [HttpGet]
        [Route("api/loans/masters/contactmasterNew/GetContactsList")]

        public List<SubscriberContactDTO> GetContactsList()
        {
            try
            {
                return obj.GetContactsList(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }

        }
    }
}