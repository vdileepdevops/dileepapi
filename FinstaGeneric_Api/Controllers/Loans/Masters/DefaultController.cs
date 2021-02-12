using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class DefaultController : ControllerBase
    {

        private IHostingEnvironment _hostingEnvironment;

        public DefaultController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public class Employee
        {
            public int ID { get; set; }
            public string Fname { get; set; }
            public string Lname { get; set; }
            public string email { get; set; }
        }

        [Route("api/loans/masters/contact/PostTestNew")]
        [HttpPost]
        public Employee PostTest([FromBody]Employee _Test)
        {
            Console.WriteLine("Hit");
            _Test.email = "Nag@gmail.com";
            return _Test;
        }


        [Route("api/loans/masters/contact/FileUpload")]

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok("");
                }
                else
                {
                    return BadRequest();

                }

            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Internal server error");
                throw ex;
            }
        }

        [Route("api/loans/masters/contact/MultiFileUpload")]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public IActionResult Uploadmultiple()
        {
            try
            {
                List<string> UploadFilepathList = new List<string>();
                var i = 0;
                var files = Request.Form.Files;
                var filescount = Request.Form.Files.Count;
                for (i = 0; i < filescount; i++)
                {
                   

                    var file = Request.Form.Files[i];
                    string UploadfolderName = "Upload";

                    string webRootPath = _hostingEnvironment.ContentRootPath;
                    string UploadPath = Path.Combine(webRootPath, UploadfolderName);
                    //string[] files123 = System.IO.Directory.GetFiles(UploadPath);


                    if (!Directory.Exists(UploadPath))
                    {
                        Directory.CreateDirectory(UploadPath);

                    }
                    if (file.Length > 0)
                    {
                        //var NewFileName = Request.Form["NewFileName"].ToString();
                        //ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        string extension = Path.GetExtension(fileName);
                        string fullPath = Path.Combine(UploadPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        UploadFilepathList.Add(fullPath);
                        UploadFilepathList.Add(fileName);
                    }
                }


                return Ok(UploadFilepathList);

            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Internal server error");
                throw ex;
            }
        }



    }
}