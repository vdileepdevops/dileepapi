using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Banking.Reports;
using FinstaRepository.Interfaces.Banking.Reports;
using FinstaRepository.DataAccess.Banking.Reports;
using System.IO;
using FinstaApi.Common;

namespace FinstaApi.Controllers.Banking.Report
{

    [ApiController]
    [EnableCors("CorsPolicy")]
    public class MemberEnquiryController : ControllerBase
    {
        IMemberEnquiry objMemberEnquiry = new MemberEnquiryDAL();
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        string Con = string.Empty;
        public MemberEnquiryController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberDetails")]
        public IActionResult GetMemberDetails()
        {
            List<MemberDetailsDTO> ListMemberDetails = new List<MemberDetailsDTO>();
            try
            {
                ListMemberDetails = objMemberEnquiry.GetMemberDetails(Con);
                return ListMemberDetails != null ? Ok(ListMemberDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberTransactions")]
        public async Task<IActionResult> GetMemberTransactions(long Memberid)
        {
            try
            {
                List<MemberTransactionDTO> _ListMemberTransactions = await objMemberEnquiry.GetMemberTransactions(Memberid, Con);
                if (!string.IsNullOrEmpty(_ListMemberTransactions[0].pContactImagePath))
                {
                    string ThumbnailFolder = "Upload\\";
                    string webRootPath = _hostingEnvironment.ContentRootPath;
                    string ThumabnailsFolderpath = Path.Combine(webRootPath, ThumbnailFolder);
                    string ThumbnailFullPath = Path.Combine(ThumabnailsFolderpath, Convert.ToString(_ListMemberTransactions[0].pContactImagePath));
                    if (System.IO.File.Exists(ThumbnailFullPath))
                    {
                        _ListMemberTransactions[0].pImage = getConvertImagepathtobase64(ThumbnailFullPath);
                    }

                }
                if (_ListMemberTransactions != null && _ListMemberTransactions.Count > 0)
                {
                    return Ok(_ListMemberTransactions);
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
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberReceiptDetails")]
        public IActionResult GetMemberReceiptDetails(string FdAccountNo)
        {
            List<MemberReceiptDTO> ListMemberReceipts = new List<MemberReceiptDTO>();
            try
            {
                ListMemberReceipts = objMemberEnquiry.GetMemberReceiptDetails(FdAccountNo, Con);
                return ListMemberReceipts != null ? Ok(ListMemberReceipts) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberNomineeDetails")]
        public IActionResult GetMemberNomineeDetails(string FdAccountNo)
        {
            List<MemberNomineeDetailsDTO> MemberNomineeDetailsList = new List<MemberNomineeDetailsDTO>();
            try
            {
                MemberNomineeDetailsList = objMemberEnquiry.GetMemberNomineeDetails(FdAccountNo, Con);
                return MemberNomineeDetailsList != null ? Ok(MemberNomineeDetailsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberInterestPaymentDetails")]
        public IActionResult GetMemberInterestPaymentDetails(string FdAccountNo)
        {
            List<MemberInterestPaymentDTO> MemberInterestPaymentsList = new List<MemberInterestPaymentDTO>();
            try
            {
                MemberInterestPaymentsList = objMemberEnquiry.GetMemberInterestPaymentDetails(FdAccountNo, Con);
                return MemberInterestPaymentsList != null ? Ok(MemberInterestPaymentsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberPromoterSalaryDetails")]
        public IActionResult GetMemberPromoterSalaryDetails(string FdAccountNo)
        {
            List<MemberPromoterSalaryDTO> MemberPromoterSalaryList = new List<MemberPromoterSalaryDTO>();
            try
            {
                MemberPromoterSalaryList = objMemberEnquiry.GetMemberPromoterSalaryDetails(FdAccountNo, Con);
                return MemberPromoterSalaryList != null ? Ok(MemberPromoterSalaryList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberLiensDetails")]
        public IActionResult GetMemberLiensDetails(string FdAccountNo)
        {
            List<MemberLiensDTO> MemberLiensList = new List<MemberLiensDTO>();
            try
            {
                MemberLiensList = objMemberEnquiry.GetMemberLiensDetails(FdAccountNo, Con);
                return MemberLiensList != null ? Ok(MemberLiensList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberMaturityBondsDetails")]
        public IActionResult GetMemberMaturityBondsDetails(string FdAccountNo)
        {
            List<MemberMaturityBondDTO> MemberMaturityBondsList = new List<MemberMaturityBondDTO>();
            try
            {
                MemberMaturityBondsList = objMemberEnquiry.GetMemberMaturityBondsDetails(FdAccountNo, Con);
                return MemberMaturityBondsList != null ? Ok(MemberMaturityBondsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberMaturityPaymentsDetails")]
        public IActionResult GetMemberMaturityPaymentsDetails(string FdAccountNo)
        {
            List<MaturityPaymentsDTO> MaturityPaymentsList = new List<MaturityPaymentsDTO>();
            try
            {
                MaturityPaymentsList = objMemberEnquiry.GetMemberMaturityPaymentsDetails(FdAccountNo, Con);
                return MaturityPaymentsList != null ? Ok(MaturityPaymentsList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }



        #region Member Enquiry Details Report ...
        [HttpGet]
        [Route("api/Banking/Report/MemberEnquiry/GetMemberEnquiryDetailsReport")]
        public IActionResult GetMemberEnquiryDetailsReport(string FdAccountNo)
        {
            List<MemberEnquiryDTO> LstMemberEnquiryDetails = new List<MemberEnquiryDTO>();
            try
            {
                LstMemberEnquiryDetails = objMemberEnquiry.GetMemberEnquiryDetailsReport(FdAccountNo, Con);
                return LstMemberEnquiryDetails != null ? Ok(LstMemberEnquiryDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion
    }
}