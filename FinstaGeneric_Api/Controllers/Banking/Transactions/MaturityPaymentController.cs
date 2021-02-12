using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;
using FinstaApi.Common;

namespace FinstaApi.Controllers.Banking.Transactions
{
    // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class MaturityPaymentController : ControllerBase
    {

        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        IMaturityPayment objMaturityPayment = new MaturityPaymentDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public MaturityPaymentController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Get Memeber Type Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMemberTypes")]
        public IActionResult GetMemberTypes()
        {
            List<MemberTypesDTO> lstMembertypes = new List<MemberTypesDTO>();
            try
            {
                lstMembertypes = objMaturityPayment.GetMemberTypes(Con);
                return lstMembertypes != null ? Ok(lstMembertypes) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get Maturity MemeberID Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMemeberIds")]
        public IActionResult GetMemberIds(string MemberType)
        {
            List<MemberIdsDTO> lstMemberIdsDTO = new List<MemberIdsDTO>();
            try
            {
                lstMemberIdsDTO = objMaturityPayment.GetMemberIds(MemberType, Con);
                return lstMemberIdsDTO != null ? Ok(lstMemberIdsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }
        /// <summary>
        /// Get Maturity DepositIDs
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetDepositIds")]
        public IActionResult GetDepositIds(string BranchName, string MaturityType, long Schemeid)
        {
            List<DepositIdsDTO> lstDepositIdsDTO = new List<DepositIdsDTO>();
            try
            {
                lstDepositIdsDTO = objMaturityPayment.GetDepositIds(BranchName, MaturityType, Schemeid, Con);
                return lstDepositIdsDTO != null ? Ok(lstDepositIdsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMaturityBondView")]
        public IActionResult GetMaturityBondView()
        {
            List<MaturityBondViewDTO> lstMaturityBondViewDTO = new List<MaturityBondViewDTO>();
            try
            {
                lstMaturityBondViewDTO = objMaturityPayment.GetMaturityBondView(Con);
                return lstMaturityBondViewDTO != null ? Ok(lstMaturityBondViewDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetPreMaturityDetails")]
        public IActionResult GetPreMaturityDetails(string FDAccountno, string Date, string type)
        {
            PreMaturedetailsDTO PreMaturedetails = new PreMaturedetailsDTO();
            try
            {
                PreMaturedetails = objMaturityPayment.GetPreMaturityDetails(FDAccountno, Date, type, Con);
                return PreMaturedetails != null ? Ok(PreMaturedetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [Route("api/Banking/Transactions/MaturityPayment/SaveMaturitybond")]
        [HttpPost]
        public IActionResult SaveMaturitybond(MaturitybondsSave _MaturitybondsSaveDTO)
        {
            bool isSaved = false;

            try
            {
                isSaved = objMaturityPayment.SaveMaturitybond(_MaturitybondsSaveDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetSchemeType")]
        public async Task<IActionResult> GetSchemeType(string BranchName, string MaturityType)
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            try
            {
                lstSchemetype = await objMaturityPayment.GetSchemeType(BranchName, MaturityType, Con);
                return lstSchemetype != null ? Ok(lstSchemetype) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMaturityBranchDetails")]
        public async Task<IActionResult> GetMaturityBranchDetails(string MaturityType)
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await objMaturityPayment.GetMaturityBranchDetails(MaturityType, Con);
                if (_ChitBranchDetailsList != null && _ChitBranchDetailsList.Count > 0)
                {
                    return Ok(_ChitBranchDetailsList);
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
        #region Maturity Payments
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMaturityMembers")]
        public async Task<IActionResult> GetMaturityMembers(string PaymentType)
        {
            List<MaturityMembersDTO> lstMembers = new List<MaturityMembersDTO>();
            try
            {
                lstMembers = await objMaturityPayment.GetMaturityMembers(PaymentType,Con);
                return lstMembers != null ? Ok(lstMembers) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMaturityFdDetails")]
        public IActionResult GetMaturityFdDetails(string PaymentType,long Memberid, string Date)
        {
            List<MaturityBondstList> lstDepositIdsDTO = new List<MaturityBondstList>();
            try
            {
                lstDepositIdsDTO = objMaturityPayment.GetMaturityFdDetails(PaymentType,Memberid, Date, Con);
                return lstDepositIdsDTO != null ? Ok(lstDepositIdsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [HttpPost]
        [Route("api/Banking/Transactions/MaturityPayment/SaveMaturityPayment")]
        public async Task<IActionResult> SaveMaturityPayment([FromBody]  MaturityPaymentSaveDTO ObjMaturityPaymentDTO)
        {

            List<string> lstdata = new List<string>();
            try
            {
                string OUTPaymentId = string.Empty;
                if (objMaturityPayment.SaveMaturityPayment(ObjMaturityPaymentDTO, Con, out OUTPaymentId))
                {
                    if (!string.IsNullOrEmpty(OUTPaymentId))
                    {
                        List<PaymentVoucherReportDTO> _PaymentVoucherReportDTO = new List<PaymentVoucherReportDTO>();
                        _PaymentVoucherReportDTO = await _AccountingTransactionsDAL.GetPaymentVoucherReportData(OUTPaymentId, Con);

                        return Ok(_PaymentVoucherReportDTO);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetMaturityPaymentView")]
        public IActionResult GetMaturityPaymentView()
        {
            List<MaturityPaymentDetailsViewDTO> lstMaturityPaymentDetailsViewDTO = new List<MaturityPaymentDetailsViewDTO>();
            try
            {
                lstMaturityPaymentDetailsViewDTO = objMaturityPayment.GetMaturityPaymentDetailsView(Con);
                return lstMaturityPaymentDetailsViewDTO != null ? Ok(lstMaturityPaymentDetailsViewDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetFdTransactionDetails")]
        public async Task<IActionResult> GetFdTransactionDetails(string FdAccountNo)
        {
            //FdTransactionDataEdit _FdTransactionDataEdit = new FdTransactionDataEdit();
            try
            {
                FdTransactionDataEdit _FdTransactionDataEdit = await objMaturityPayment.GetFdTransactionDetails(FdAccountNo, Con);
                if (_FdTransactionDataEdit != null)
                {
                    return Ok(_FdTransactionDataEdit);
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
        [Route("api/Banking/Transactions/MaturityPayment/SaveMaturityRenewal")]
        public IActionResult SaveMaturityRenewal([FromBody] MaturityRenewalSaveDTO _MaturityRenewalSaveDTO)
        {
            FdComfigurationIdandName _FdComfigurationIdandName = new FdComfigurationIdandName();
            try
            {
                long pFdAccountId = 0;
                string FdaccountNo = objMaturityPayment.SaveMaturityRenewal(_MaturityRenewalSaveDTO, Con, out pFdAccountId);
                if (!string.IsNullOrEmpty(FdaccountNo))
                {
                    _FdComfigurationIdandName.pFdaccountNo = FdaccountNo;
                    _FdComfigurationIdandName.pFdAccountId = pFdAccountId;
                    return Ok(_FdComfigurationIdandName);
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
        [HttpGet]
        [Route("api/Banking/Transactions/MaturityPayment/GetLienDetails")]
        public IActionResult GetLienDetails(string FdAccountNo)
        {
            List<LienEntryViewDTO> lstfdview = new List<LienEntryViewDTO>();
            try
            {
                lstfdview = objMaturityPayment.GetLienDetails(FdAccountNo,Con);
                return lstfdview != null ? Ok(lstfdview) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }

        #endregion
    }
}