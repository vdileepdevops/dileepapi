using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings;
using FinstaApi.Common;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaApi.Controllers.Loans.Masters;
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace FinstaApi.Controllers.Settings
{
    [Authorize]
    [ApiController]

    [EnableCors("CorsPolicy")]
    public class ReferralAdvocateController : ControllerBase
    {
        string Con = string.Empty;
        private IHostingEnvironment _hostingEnvironment;
        static IConfiguration _iconfiguration;
        IReferralAdvocate obReferralAdvocate = new ReferralAdvocateDAL();
        public List<ReferralAdvocateDTO> lstReferalContactDetails { get; set; }
        public List<TdsSectionDTO> lstTdsSectionDetails { get; set; }
        public ReferralAdvocateController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region Referral/Agent
        [Route("api/Settings/ReferralAdvocate/getContactDetails")]
        [HttpGet]
        public IActionResult getContactDetails(string contactType)
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getContactDetails(contactType, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/saveReferral")]
        [HttpPost]
        public IActionResult saveReferral([FromBody] ReferralAdvocateDTO referralavocatelist)
        {

            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.saveReferral(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
                //isSaved = obReferralAdvocate.saveReferral(referralavocatelist, Con);
                //}
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/getReferralAgentDetails")]
        [HttpGet]
        public IActionResult getReferralAgentDetails(string Type)
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getReferralAgentDetails(Type, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/getReferralDetails")]
        [HttpGet]
        public IActionResult getReferralDetails()
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getReferralDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/ViewReferralAgentDetails")]
        [HttpGet]
        public IActionResult ViewReferralAgentDetails(Int64 Refid)
        {
            ReferralAdvocateDTO lstViewReferralAgentDetails = new ReferralAdvocateDTO();
            string base64String = string.Empty;
            try
            {
                lstViewReferralAgentDetails = obReferralAdvocate.ViewReferralAgentDetails(Refid, Con);
                if (!string.IsNullOrEmpty(lstViewReferralAgentDetails.pContactimagepath))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(lstViewReferralAgentDetails.pContactimagepath))
                    {
                        using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes1 = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes1);
                            lstViewReferralAgentDetails.pPhoto = base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewReferralAgentDetails);
        }
        [Route("api/Settings/ReferralAdvocate/DeleteReferralAgent")]
        [HttpPost]
        public IActionResult DeleteReferralAgent(DeleteDTO objDeleteDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = obReferralAdvocate.DeleteReferralAgent(objDeleteDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/UpdatedReferral")]
        [HttpPost]
        public IActionResult UpdatedReferral(ReferralAdvocateDTO referralavocatelist)
        {

            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.UpdatedReferral(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
                //isUpdated = obReferralAdvocate.UpdatedReferral(referralavocatelist, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isUpdated);
        }
        [Route("api/Settings/ReferralAdvocate/CheckContactDuplicate")]
        [HttpGet]
        public IActionResult CheckContactDuplicate(Int64 ContactId, Int64 RefId)
        {
            int count = 0;
            try
            {
                count = obReferralAdvocate.CheckContactDuplicate(ContactId, RefId, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(count);
        }
        [Route("api/Settings/ReferralAdvocate/GetDocumentProofs")]
        [HttpGet]
        public IActionResult GetDocumentProofs(Int64 DocId)
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.GetDocumentProofs(DocId, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/getTdsSectionNo")]
        [HttpGet]
        public IActionResult getTdsSectionNo()
        {
            lstTdsSectionDetails = new List<TdsSectionDTO>();
            try
            {
                lstTdsSectionDetails = obReferralAdvocate.getTdsSectionNo(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstTdsSectionDetails);
        }
        [Route("api/Settings/ReferralAdvocate/SaveTdsSectionNo")]
        [HttpPost]
        public IActionResult SaveTdsSectionNo(TdsSectionDTO tdsSectionNo)
        {
            bool isSaved = false;
            try
            {
                isSaved = obReferralAdvocate.SaveTdsSectionNo(tdsSectionNo, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/CheckTdsSectionDuplicate")]
        [HttpGet]
        public IActionResult CheckTdsSectionDuplicate(string TdsSecName)
        {
            int count = 0;
            try
            {
                count = obReferralAdvocate.CheckTdsSectionDuplicate(TdsSecName, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(count);
        }
        [Route("api/Settings/ReferralAdvocate/getGstType")]
        [HttpGet]
        public IActionResult getGstType()
        {
            List<GstTyeDTO> lstReferalContactDetails = new List<GstTyeDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getGstType(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/SaveGstType")]
        [HttpPost]
        public IActionResult SaveGstType(GstTyeDTO GstType)
        {
            bool isSaved = false;
            try
            {
                isSaved = obReferralAdvocate.SaveGstType(GstType, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/CheckGstTypeDuplicate")]
        [HttpGet]
        public IActionResult CheckGstTypeDuplicate(string strGstType)
        {
            int count = 0;
            try
            {
                count = obReferralAdvocate.CheckGstTypeDuplicate(strGstType, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(count);
        }
        [Route("api/Settings/ReferralAdvocate/UploadFile")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadFile(IFormFile objDocStore)
        {
            try
            {
                //var physicalFile = new FileInfo("C:\\Users\\Public\\Pictures\\Sample Pictures\\Chrysanthemum.png");
                //objDocStore.pUploadFile = physicalFile.AsMockIFormFile();
                // var filePath = Path.GetTempFileName();
                if (objDocStore != null)
                {
                    //foreach (IFormFile doc in objDocStore.pUploadFile)
                    //{
                    //string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string pDocFileType = System.IO.Path.GetExtension(objDocStore.FileName).Trim('"');
                    string filedir = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
                    Debug.WriteLine(filedir);
                    Debug.WriteLine(Directory.Exists(filedir));
                    if (!Directory.Exists(filedir))
                    { //check if the folder exists;
                        Directory.CreateDirectory(filedir);
                    }
                    Guid obj = Guid.NewGuid();
                    string file1 = Path.Combine(filedir, obj.ToString());
                    string pDocStorePath = file1;
                    Debug.WriteLine(file1);
                    objDocStore.CopyTo(new FileStream(file1, FileMode.Create));
                    //}
                }
                // var file = Request.Form.Files[0];
                //// var file = "C:\\Users\\Public\\Pictures\\Sample Pictures\\Chrysanthemum.jpg";
                // string folderName = "Upload";
                // //string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                // //string pDocFileType = System.IO.Path.GetExtension(file.FileName).Trim('"');
                // string filedir = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                // Debug.WriteLine(filedir);
                // Debug.WriteLine(Directory.Exists(filedir));
                // if (!Directory.Exists(filedir))
                // { //check if the folder exists;
                //     Directory.CreateDirectory(filedir);
                // }
                // Guid obj = Guid.NewGuid();
                // string file1 = Path.Combine(filedir, obj.ToString());
                // string pDocStorePath = file1;
                // Debug.WriteLine(file1);
                //if (doc.Length > 0)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return Ok("");
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/Settings/ReferralAdvocate/GetContactDetailsbyId")]
        [HttpGet]
        public IActionResult GetContactDetailsbyId(Int64 ContactId)
        {
            ReferralAdvocateDTO lstReferalContactDetails = new ReferralAdvocateDTO();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.GetContactDetailsbyId(ContactId, Con);
                if (!string.IsNullOrEmpty(lstReferalContactDetails.pContactimagepath))
                {
                    string ThumbnailFolder = "Upload\\";
                    string webRootPath = _hostingEnvironment.ContentRootPath;
                    string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                    string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(lstReferalContactDetails.pContactimagepath));
                    lstReferalContactDetails.pContactimagepath = ThumbnailFullPath;
                }
               
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        #endregion
        #region Advocate/Lawyer
        [Route("api/Settings/ReferralAdvocate/saveAdvocate")]
        [HttpPost]
        public IActionResult saveAdvocate(ReferralAdvocateDTO referralavocatelist)
        {

            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.saveAdvocate(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }

                //isSaved = obReferralAdvocate.saveAdvocate(referralavocatelist, Con);

            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/getAdvocateLawterDetails")]
        [HttpGet]
        public IActionResult getAdvocateLawterDetails(string Type)
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getAdvocateLawterDetails(Type, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/ViewAdvocateLawerDetails")]
        [HttpGet]
        public IActionResult ViewAdvocateLawerDetails(Int64 Refid)
        {
            ReferralAdvocateDTO lstViewReferralAgentDetails = new ReferralAdvocateDTO();
            string base64String = string.Empty;
            try
            {
                lstViewReferralAgentDetails = obReferralAdvocate.ViewAdvocateLawerDetails(Refid, Con);
                if (!string.IsNullOrEmpty(lstViewReferralAgentDetails.pContactimagepath))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(lstViewReferralAgentDetails.pContactimagepath))
                    {
                        using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes1 = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes1);
                            lstViewReferralAgentDetails.pPhoto = base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewReferralAgentDetails);
        }
        [Route("api/Settings/ReferralAdvocate/DeleteAdvocateLawer")]
        [HttpPost]
        public IActionResult DeleteAdvocateLawer(DeleteDTO objDeleteDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = obReferralAdvocate.DeleteAdvocateLawer(objDeleteDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/UpdatedAdvocateLawer")]
        [HttpPost]
        public IActionResult UpdatedAdvocateLawer(ReferralAdvocateDTO referralavocatelist)
        {
            //bool isUpdated = false;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.UpdatedAdvocateLawer(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }

                //isUpdated = obReferralAdvocate.UpdatedAdvocateLawer(referralavocatelist, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isUpdated);
        }
        [Route("api/Settings/ReferralAdvocate/CheckAdvocateDuplicate")]
        [HttpGet]
        public IActionResult CheckAdvocateDuplicate(Int64 ContactId, Int64 RefId)
        {
            int count = 0;
            try
            {
                count = obReferralAdvocate.CheckAdvocateDuplicate(ContactId, RefId, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(count);
        }
        #endregion
        #region Party
        [Route("api/Settings/ReferralAdvocate/saveParty")]
        [HttpPost]
        public IActionResult saveParty(ReferralAdvocateDTO referralavocatelist)
        {

            try
            {

                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.saveParty(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }

                //isSaved = obReferralAdvocate.saveParty(referralavocatelist, Con);

            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/getPartyDetails")]
        [HttpGet]
        public IActionResult getPartyDetails(string Type)
        {
            lstReferalContactDetails = new List<ReferralAdvocateDTO>();
            try
            {
                lstReferalContactDetails = obReferralAdvocate.getPartyDetails(Type, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReferalContactDetails);
        }
        [Route("api/Settings/ReferralAdvocate/ViewPartyDetails")]
        [HttpGet]
        public IActionResult ViewPartyDetails(Int64 Refid)
        {
            ReferralAdvocateDTO lstViewReferralAgentDetails = new ReferralAdvocateDTO();
            string base64String = string.Empty;
            try
            {
                lstViewReferralAgentDetails = obReferralAdvocate.ViewPartyDetails(Refid, Con);
                if (!string.IsNullOrEmpty(lstViewReferralAgentDetails.pContactimagepath))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(lstViewReferralAgentDetails.pContactimagepath))
                    {
                        using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes1 = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes1);
                            lstViewReferralAgentDetails.pPhoto = base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewReferralAgentDetails);
        }
        [Route("api/Settings/ReferralAdvocate/DeleteParty")]
        [HttpPost]
        public IActionResult DeleteParty(DeleteDTO objDeleteDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = obReferralAdvocate.DeleteParty(objDeleteDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Settings/ReferralAdvocate/UpdatedParty")]
        [HttpPost]
        public IActionResult UpdatedParty(ReferralAdvocateDTO referralavocatelist)
        {
            //bool isUpdated = false;
            try
            {

                if (!string.IsNullOrEmpty(Convert.ToString(referralavocatelist.documentstorelist)))
                {
                    if (referralavocatelist.documentstorelist.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in referralavocatelist.documentstorelist)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (obReferralAdvocate.UpdatedParty(referralavocatelist, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }

                //isUpdated = obReferralAdvocate.UpdatedParty(referralavocatelist, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            //return Ok(isUpdated);
        }
        [Route("api/Settings/ReferralAdvocate/CheckPartyDuplicate")]
        [HttpGet]
        public IActionResult CheckPartyDuplicate(Int64 ContactId, Int64 RefId)
        {
            int count = 0;
            try
            {
                count = obReferralAdvocate.CheckPartyDuplicate(ContactId, RefId, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(count);
        }
        #endregion

    }
}