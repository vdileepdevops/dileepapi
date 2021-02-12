using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;


namespace FinstaApi.Controllers.Banking.Transactions
{

    [ApiController]
    [EnableCors("CorsPolicy")]
    public class CommissionPaymentController : ControllerBase
    {
        ICommissionPayment objcommissionPayment =new CommissionPaymentDAL();

        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public CommissionPaymentController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        
        [Route("api/Banking/Transactions/CommissionPayment/SaveCommisionPayment")]
        [HttpPost]
        public IActionResult SaveCommisionPayment([FromBody] CommissionPaymentDTO _CommissionPaymentDTO)
        {
            bool isSaved = false;
            CommissionPaymentSaveDTO objSave = new CommissionPaymentSaveDTO();
            try
            {
                string paymentId = string.Empty;
                isSaved = objcommissionPayment.SaveCommisionPayment(_CommissionPaymentDTO, Con, out paymentId);
                if (!string.IsNullOrEmpty(paymentId))
                {
                    objSave.pvoucherid = paymentId;


                }
                return Ok(objSave);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }



        }


        [HttpGet]
        [Route("api/Banking/Transactions/CommissionPayment/GetAgentDetails")]
        public IActionResult GetAgentDetails()
        {
            List<CommissionPaymentDTO> lstagent = new List<CommissionPaymentDTO>();
            try
            {
                lstagent = objcommissionPayment.GetAgentDetails(Con);
                return lstagent != null ? Ok(lstagent) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/CommissionPayment/GetAgentContactDetails")]
        public IActionResult GetAgentContactDetails(long agentid)
        {
            List<CommissionPaymentAgentViewDTO> lstagentdetails = new List<CommissionPaymentAgentViewDTO>();
            try
            {
                lstagentdetails = objcommissionPayment.GetAgentContactDetails(agentid, Con);
                return lstagentdetails != null ? Ok(lstagentdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/CommissionPayment/GetAgentBankDetails")]
        public IActionResult GetAgentBankDetails(long agentid)
        {
            List<AgentBankDetailsDTO> lstagentbankdetails = new List<AgentBankDetailsDTO>();
            try
            {
                lstagentbankdetails = objcommissionPayment.GetAgentBankDetails(agentid, Con);
                return lstagentbankdetails != null ? Ok(lstagentbankdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]     
        [Route("api/Banking/Transactions/CommissionPayment/ShowPromoterSalaryDetails")]
        public IActionResult ShowPromoterSalaryDetails(long agentid, string asondate)
        {
            List<CommissionPaymentDetailsDTO> lstpromotersalary = new List<CommissionPaymentDetailsDTO>();
            try
            {
                lstpromotersalary = objcommissionPayment.ShowPromoterSalaryDetails(agentid, asondate, Con);
                return lstpromotersalary != null ? Ok(lstpromotersalary) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/CommissionPayment/ShowPromoterSalaryReport")]
        public IActionResult ShowPromoterSalaryReport(long agentid, string frommonthof, string tomonthof, string type, string pdatecheked)
        {
            List<CommissionPaymentDetailsDTO> lstpromotersalary = new List<CommissionPaymentDetailsDTO>();
            try
            {
                lstpromotersalary = objcommissionPayment.ShowPromoterSalaryReport(agentid,frommonthof,tomonthof,type, pdatecheked, Con);
                return lstpromotersalary != null ? Ok(lstpromotersalary) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/CommissionPayment/GetViewCommisionpaymentdetails")]
        public IActionResult GetViewCommisionpaymentdetails()
        {
            List<CommissionPaymentDetailsDTO> lstviewMemberdetails = new List<CommissionPaymentDetailsDTO>();
            try
            {
                lstviewMemberdetails = objcommissionPayment.GetViewCommisionpaymentdetails(Con);
                return lstviewMemberdetails != null ? Ok(lstviewMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }





        //-----------------------END-------------------------------------------

    }
}