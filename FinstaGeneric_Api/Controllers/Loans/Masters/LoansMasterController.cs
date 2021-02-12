using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Loans.Masters;
using FinstaRepository.Interfaces.Loans.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [ApiController]

    [EnableCors("CorsPolicy")]
    public class LoansMasterController : ControllerBase
    {
        public List<LoansMasterDTO> lstLoanMasterdetails { get; set; }

        public List<loanconfigurationDTO> lstLoanpayins { get; set; }
        public List<loanconfigurationDTO> lstLoanIneterstratetypes { get; set; }

        ILoansMaster objLoanmaster = new LoansMasterDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public LoansMasterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/loans/masters/loanmaster/getLoanTypes")]
        [HttpGet]
        public IActionResult getLoanTypes()
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                lstLoanMasterdetails = objLoanmaster.getLoanTypes(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanMasterdetails);


        }

        [Route("api/loans/masters/loanmaster/getLoanNames")]
        [HttpGet]
        public IActionResult getLoanNames(int loanTypeId)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                lstLoanMasterdetails = objLoanmaster.getLoanNames(Con,loanTypeId);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanMasterdetails);


        }
        [Route("api/loans/masters/loanmaster/getLoanpayins")]
        [HttpGet]
        public IActionResult getLoanpayins()
        {
            lstLoanpayins = new List<loanconfigurationDTO>();
            try
            {
                lstLoanpayins = objLoanmaster.getLoanpayins(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanpayins);


        }
        [Route("api/loans/masters/loanmaster/getLoanInterestratetypes")]
        [HttpGet]
        public IActionResult getLoanInterestratetypes()
        {
            lstLoanIneterstratetypes = new List<loanconfigurationDTO>();
            try
            {
                lstLoanIneterstratetypes = objLoanmaster.getLoanInterestratetypes(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanIneterstratetypes);


        }

        [Route("api/loans/masters/loanmaster/getLoanMasterDetailsgrid")]
        [HttpGet]
        public IActionResult getLoanMasterDetailsgrid()
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                lstLoanMasterdetails = objLoanmaster.getLoanMasterDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanMasterdetails);


        }
        [Route("api/loans/masters/loanmaster/getLoanMasterDetails")]
        [HttpGet]
        public IActionResult getLoanMasterDetails(Int64 loanid)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                lstLoanMasterdetails = objLoanmaster.getLoanMasterDetails(Con, loanid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanMasterdetails);


        }

       


        [Route("api/loans/masters/loanmaster/checkInsertLoanNameandCodeDuplicates")]
        //[HttpPost]
        //[HttpGet("{checkparamtype}/{loanname}/{loancode}")]
        [HttpGet]
        public IActionResult checkInsertLoanNameandCodeDuplicates(string checkparamtype,string loanname,string loancode, Int64 loanid)
        {
            int count = 0;
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                count = objLoanmaster.checkInsertLoanNameandCodeDuplicates(checkparamtype,loanname, loancode,loanid,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        [Route("api/loans/masters/loanmaster/saveLoanMaster")]
        [HttpPost]
        public IActionResult saveLoanMaster(LoansMasterDTO loanmasterlist)
        {
            bool isSaved = false;
            
            try
            {
                isSaved = objLoanmaster.saveLoanMaster(loanmasterlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/loans/masters/loanmaster/updateLoanMaster")]
        [HttpPost]
        public IActionResult updateLoanMaster(LoansMasterDTO loanmasterlist)
        {
            bool isSaved = false;
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                isSaved = objLoanmaster.updateLoanMaster(loanmasterlist, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/loans/masters/loanmaster/DeleteLoanMaster")]
        [HttpPost]
        public IActionResult DeleteLoanMaster(LoansMasterDTO loanmasterlist)
        {
            bool isSaved = false;
           // lstLoanMasterdetails = new List<LoansMasterDTO>();

            Int64 loanid = loanmasterlist.pLoanid;
            int modifiedby = loanmasterlist.pModifiedby;
            try
            {
                isSaved = objLoanmaster.DeleteLoanMaster(loanid, modifiedby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

    }
}