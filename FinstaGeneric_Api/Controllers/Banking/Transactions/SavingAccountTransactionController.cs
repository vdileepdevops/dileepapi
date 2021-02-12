using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FinstaApi.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using FinstaInfrastructure.Banking.Transactions;

namespace FinstaApi.Controllers.Banking.Transactions
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    
    public class SavingAccountTransactionController : ControllerBase
    {
        ISavingAccountTransaction _SavingAccountTransaction = new SavingAccountTransactionDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public List<SavingAccountConfigBind> _SavingAccountConfigList { get; set; }
        public List<SavingAccountConfigDetailsBind> _SavingAccountConfigDetailsList { get; set; }
        public List<SavingAccountTransactionMainGrid> _SavingAccountTransactionMainGridList { get; set; }
        public List<MemberDetails> _MemberDetailsList { get; set; }

        public SavingAccountTransactionController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [HttpGet]
        [Route("api/Banking/Transactions/SavingTransaction/GetSavingAccountDetails")]
        public async Task<IActionResult> GetSavingAccountDetails(long Membertypeid, string Applicanttype)
        {
            
            try
            {
                _SavingAccountConfigList = new List<SavingAccountConfigBind>();
                _SavingAccountConfigList = await _SavingAccountTransaction.GetSavingAccountDetails(Membertypeid, Applicanttype,Con);
                if (_SavingAccountConfigList != null)
                {
                    return Ok(_SavingAccountConfigList);
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

        [HttpGet]
        [Route("api/Banking/Transactions/SavingTransaction/GetSavingAccountConfigDetails")]
        public async Task<IActionResult> GetSavingAccountConfigDetails(long Savingconfigid, long Membertypeid, string Applicanttype)
        {

            try
            {
                _SavingAccountConfigDetailsList = new List<SavingAccountConfigDetailsBind>();
                _SavingAccountConfigDetailsList = await _SavingAccountTransaction.GetSavingAccountConfigDetails(Savingconfigid, Membertypeid, Applicanttype,Con);
                if (_SavingAccountConfigDetailsList != null)
                {
                    return Ok(_SavingAccountConfigDetailsList);
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
        [HttpGet]
        [Route("api/Banking/Transactions/SavingTransaction/GetContactDetails")]
        public async Task<IActionResult> GetContactDetails(long MemberID)
        {
           
            try
            {
                ContactDetails _ContactDetailsList = await _SavingAccountTransaction.GetContactDetails(MemberID, Con);
                if (_ContactDetailsList != null)
                {
                    return Ok(_ContactDetailsList);
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

        [HttpGet]
        [Route("api/Banking/Transactions/SavingTransaction/GetMemberDetails")]
        public async Task<IActionResult> GetMemberDetails(long MemberTypeId, string contactType)
        {
           
            try
            {
                _MemberDetailsList = new List<MemberDetails>();
                _MemberDetailsList = await _SavingAccountTransaction.GetMemberDetails(MemberTypeId,contactType, Con);
                if (_MemberDetailsList != null)
                {
                    return Ok(_MemberDetailsList);
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

        [HttpPost]
        [Route("api/Banking/Transactions/SavingTransaction/SaveSavingAccountTransaction")]
        public IActionResult SaveSavingAccountTransaction(SavingAccountTransactionSave _SavingAccountTransactionDetails)
        {
            SavingAccountTransactionIdAndNo _SavingAccountTransactionIdAndNo = new SavingAccountTransactionIdAndNo();
            try
            {
                long Savingaccountid = 0;
                string Savingaccountno = _SavingAccountTransaction.SaveSavingAccountTransaction(_SavingAccountTransactionDetails, Con, out Savingaccountid);
                if (!string.IsNullOrEmpty(Savingaccountno))
                {
                    _SavingAccountTransactionIdAndNo.pSavingaccountno = Savingaccountno;
                    _SavingAccountTransactionIdAndNo.pSavingaccountid = Savingaccountid;
                    return Ok(_SavingAccountTransactionIdAndNo);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        //[HttpPost]
        //[Route("api/Banking/Transactions/SavingTransaction/SaveJointmemberAndNominee")]
        //public IActionResult SaveJointmemberAndNominee([FromBody] JointmemberAndNomineeSave _JointmemberAndNomineeSave)
        //{
        //    try
        //    {
               
        //        if (_SavingAccountTransaction.SaveJointmemberAndNominee(_JointmemberAndNomineeSave, Con))
        //        {
        //            return Ok(true);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status304NotModified);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpPost]
        //[Route("api/Banking/Transactions/SavingTransaction/SaveReferral")]
        //public IActionResult SaveReferral([FromBody] ReferralSave _ReferralSave)
        //{
        //    try
        //    {

        //        if (_SavingAccountTransaction.SaveReferral(_ReferralSave, Con))
        //        {
        //            return Ok(true);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status304NotModified);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
        [Route("api/Banking/Transactions/SavingTransaction/CheckMemberDuplicates")]
        [HttpGet]
        public IActionResult CheckMemberDuplicates(long Memberid, long savingaccountid)
        {
            int count = 0;
           
            try
            {
                count = _SavingAccountTransaction.CheckMemberDuplicates(Memberid, savingaccountid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }
        [HttpGet]
        [Route("api/Banking/Transactions/SavingTransaction/GetSavingAccountTransactionMainGrid")]
        public async Task<IActionResult> GetSavingAccountTransactionMainGrid()
        {
            _SavingAccountTransactionMainGridList = new List<SavingAccountTransactionMainGrid>();
            try
            {
                _SavingAccountTransactionMainGridList = await _SavingAccountTransaction.GetSavingAccountTransactionMainGrid(Con);
                if (_SavingAccountTransactionMainGridList != null)
                {
                    return Ok(_SavingAccountTransactionMainGridList);
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
        [Route("api/Banking/Transactions/SavingTransaction/GetSavingAccountTransDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSavingAccountTransDetails(Int64 SavingAccountId, string accounttype, string savingsaccountNo)
        {
           SavingsTransactionDataEdit lstSavingAccTransdetails = new SavingsTransactionDataEdit();
            try
            {
                lstSavingAccTransdetails = await _SavingAccountTransaction.GetSavingAccountTransactionEditDetails(Con, SavingAccountId, accounttype, savingsaccountNo);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstSavingAccTransdetails);


        }

        [Route("api/Banking/Transactions/SavingTransaction/DeleteSavingAccountTrans")]
        [HttpPost]
        public IActionResult DeleteSavingAccountTrans(SavingAccountTransactionSave savingAccountTransaction)
        {
            bool isSaved = false;
            // lstLoanMasterdetails = new List<LoansMasterDTO>();

            Int64 savingAccountid = savingAccountTransaction.pSavingaccountid;
            long modifiedby = savingAccountTransaction.pCreatedby;
            try
            {
                isSaved = _SavingAccountTransaction.DeleteSavingTransaction(savingAccountid, modifiedby, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
    }
}