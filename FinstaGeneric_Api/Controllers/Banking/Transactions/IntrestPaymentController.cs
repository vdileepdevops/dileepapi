using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace FinstaApi.Controllers.Banking.Transactions
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class IntrestPaymentController : ControllerBase
    {
        IIntrestPayments objIntrestPayment = new IntrestPaymentDAL();


        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public IntrestPaymentController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/Banking/Transactions/IntrestPayment/SaveInterestPayment")]
        [HttpPost]
        public IActionResult SaveInterestPayment([FromBody] IntrestPaymentDTO _IntrestPaymentDTO)
        {
            bool isSaved = false;
            InterestPaymentSaveDTO objSave = new InterestPaymentSaveDTO();
            try
            {
                string paymentId = string.Empty;
                isSaved = objIntrestPayment.SaveInterestPayment(_IntrestPaymentDTO, Con, out paymentId);

                if (!string.IsNullOrEmpty(paymentId))
                {
                    objSave.pvoucherid = paymentId;


                }
                // return Ok(paymentId);
                return Ok(objSave);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }



        }

        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetSchemename")]
        public IActionResult GetSchemename()
        {
            List<IntrestPaymentDTO> lstscheme = new List<IntrestPaymentDTO>();
            try
            {
                lstscheme = objIntrestPayment.GetSchemename(Con);
                return lstscheme != null ? Ok(lstscheme) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetCompany")]
        public IActionResult GetCompany()
        {
            List<IntrestPaymentDTO> lstCompany = new List<IntrestPaymentDTO>();
            try
            {
                lstCompany = objIntrestPayment.GetCompany(Con);
                return lstCompany != null ? Ok(lstCompany) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetBranchName")]
        public IActionResult GetBranchName(string companyname)
        {
            List<IntrestPaymentDTO> lstBranchname = new List<IntrestPaymentDTO>();
            try
            {
                lstBranchname = objIntrestPayment.GetBranchName(companyname, Con);
                return lstBranchname != null ? Ok(lstBranchname) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/RunInterestPaymentFunction")]
        public IActionResult RunInterestPaymentFunction(string Connectionstring)
        {
            bool IsFunctionRun=false;
            try
            {
                IsFunctionRun=objIntrestPayment.RunInterestPaymentFunction(Con);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetMemberPaymenthistory")]
        public IActionResult GetMemberPaymenthistory(long schemeid,string paymenttype, string companyname, string branchname, string forthemonth)
        {
            List<IntrestPaymentDetailsDTO> lstMemberdetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                lstMemberdetails = objIntrestPayment.GetMemberPaymenthistory(schemeid,paymenttype, companyname, branchname, forthemonth, Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetShowInterestPaymentReport")]
        public IActionResult GetShowInterestPaymentReport(long schemeid, string fdaccountno, string paymenttype, string companyname, string branchname, string  pdatecheked, string frommonthof, string tomonthof, string type)
        {
            List<IntrestPaymentDetailsDTO> lstMemberdetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                lstMemberdetails = objIntrestPayment.GetShowInterestPaymentReport(schemeid, fdaccountno , paymenttype, companyname, branchname, pdatecheked,  frommonthof,  tomonthof,  type, Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


     

        [HttpGet]
        [Route("api/Banking/Transactions/IntrestPayment/GetShowInterestpaymentdetailsforview")]
        public IActionResult GetShowInterestpaymentdetailsforview()
        {
            List<IntrestPaymentDetailsDTO> lstMemberdetails = new List<IntrestPaymentDetailsDTO>();
            try
            {
                lstMemberdetails = objIntrestPayment.GetShowInterestpaymentdetailsforview(Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }







    }
}