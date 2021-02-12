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
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class RdTransactionController : ControllerBase
    {
        IRDTransaction RdtransactionDAL = new RDTransactionDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public RdTransactionController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get Nominee Details of Member Based on Member_Code
        /// </summary>
        /// <param name="MemberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetMemberNomineeDetails")]
        public async Task<IActionResult> GetMemberNomineeDetails(string MemberCode)
        {
            try
            {
                List<NomineeDetails> _RDNomineeDetails = await RdtransactionDAL.GetMemberNomineeDetails(MemberCode, Con);
                if (_RDNomineeDetails != null)
                {
                    return Ok(_RDNomineeDetails);
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

        /// <summary>
        /// Save Rd Transaction 1st and 2nd Tabs
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/SaveRDMemberandSchemeData")]
        public IActionResult SaveRDMemberandSchemeData([FromBody] RdMemberandSchemeSave _RdTransSaveDTO)
        {
            RdComfigurationIdandName _RdComfigurationIdandName = new RdComfigurationIdandName();
            try
            {
                long pRdAccountId = 0;
                string RdaccountNo = RdtransactionDAL.SaveRDMemberandSchemeData(_RdTransSaveDTO, Con, out pRdAccountId);
                if (!string.IsNullOrEmpty(RdaccountNo))
                {
                    _RdComfigurationIdandName.pRdaccountNo = RdaccountNo;
                    _RdComfigurationIdandName.pRdAccountId = pRdAccountId;
                    return Ok(_RdComfigurationIdandName);
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
        /// <summary>
        /// Get Particular Rd Scheme Details on Passing Recordid of the Rd Name
        /// </summary>

        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetRdSchemeDetails")]
        public async Task<IActionResult> GetRdSchemeDetails(string ApplicantType, string MemberType, long RdconfigID, string Rdname, long Tenure, string Tenuremode, decimal instalmentamount)
        {
            try
            {
                RDdetailsFromScheme _RDdetailsFromScheme = await RdtransactionDAL.GetRdSchemeDetails(ApplicantType, MemberType, RdconfigID, Rdname, Tenure, Tenuremode, instalmentamount, Con);
                if (_RDdetailsFromScheme != null)
                {
                    return Ok(_RDdetailsFromScheme);
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
        /// <summary>
        /// Bind Members based on Contact and Member Types
        /// </summary>
        /// <param name="Contacttype"></param>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetallRDMembers")]
        public async Task<IActionResult> GetRDMembers(string Contacttype, string MemberType)
        {
            try
            {
                List<RdMembersandContactDetails> _RdMembers = await RdtransactionDAL.GetRDMembers(Contacttype, MemberType, Con);
                if (_RdMembers != null)
                {
                    return Ok(_RdMembers);
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
        /// <summary>
        /// Get Rd Schemes on Passing Membertype and ApplicantType Names
        /// </summary>
        /// <param name="ApplicantType"></param>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetRdSchemes")]
        public async Task<IActionResult> GetRdSchemes(string ApplicantType, string MemberType)
        {
            try
            {
                List<RdNameandCode> _RdNameandCodeList = await RdtransactionDAL.GetRdSchemes(ApplicantType, MemberType, Con);
                if (_RdNameandCodeList != null)
                {
                    return Ok(_RdNameandCodeList);
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



        /// <summary>
        /// RD Joint Members and Nominees Save
        /// </summary>

        /// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/SaveRDJointMembersandNomineeData")]
        //public IActionResult SaveRDJointMembersandNomineeData([FromBody] RdJointandNomineeSave _RdJointandNomineeSave)
        //{
        //    try
        //    {
        //        if (RdtransactionDAL.SaveRDJointMembersandNomineeData(_RdJointandNomineeSave, Con))
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

        /// <summary>
        /// Save Rd Transaction Referral Details on passing Object
        /// </summary>

        /// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/SaveRDReferralData")]
        //public IActionResult SaveRDReferralData([FromBody] RdTransactionReferrals _RdTransactionReferrals)
        //{
        //    try
        //    {
        //        if (RdtransactionDAL.SaveRDReferralData(_RdTransactionReferrals, Con))
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

        [HttpGet]
        [Route("api/Banking/GetRdTransactionData")]
        public async Task<IActionResult> GetRdTransactionData()
        {
            try
            {
                List<RdTransactionMainGridData> RRdTransactionMainGridDataList = await RdtransactionDAL.GetRdTransactionData(Con);
                if (RRdTransactionMainGridDataList != null && RRdTransactionMainGridDataList.Count > 0)
                {
                    return Ok(RRdTransactionMainGridDataList);
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
        [Route("api/Banking/GetRdTenuresofTable")]
        public async Task<IActionResult> GetRdTenuresofTable(string RDName, long RdconfigId, string TenureMode,string MemberType)
        {
            try
            {
                List<RdTransactinTenuresofTable> _RdTransactinTenuresofTableList = await RdtransactionDAL.GetRdTenuresofTable(RDName, RdconfigId, TenureMode, MemberType, Con);

                if (_RdTransactinTenuresofTableList != null && _RdTransactinTenuresofTableList.Count > 0)
                {
                    return Ok(_RdTransactinTenuresofTableList);
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


        /// <summary>
        /// Gets Deposit amounts based on Tenure and Rdname, ConfigId
        /// </summary>
        /// <param name=RDName"></param>
        /// <param name="RdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <param name="Tenure"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetRdInstallmentsamountsofTable")]
        public async Task<IActionResult> GetRdInstallmentsamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure,string MemberType)
        {
            try
            {
                List<RdTransactinInstallmentsAmountofTable> _RdTransactinDepositAmountofTableList = await RdtransactionDAL.GetRdInstallmentsamountsofTable(RDName, RdconfigId, TenureMode, Tenure, MemberType, Con);

                if (_RdTransactinDepositAmountofTableList != null && _RdTransactinDepositAmountofTableList.Count > 0)
                {
                    return Ok(_RdTransactinDepositAmountofTableList);
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



        [Route("api/Banking/GetRDTenureandMininterestRateofInterestRate")]
        [HttpGet]
        public IActionResult GetRDTenureandMininterestRateofInterestRate(string Rdname, decimal instalmentamount, long Tenure, string TenureMode, string InterestPayout,string MemberType)
        {
            try
            {
                RdInterestRateValidation TenureandInterstRateCount = RdtransactionDAL.GetRDTenureandMininterestRateofInterestRate(Rdname, instalmentamount, Tenure, TenureMode, InterestPayout,MemberType, Con);
                if (TenureandInterstRateCount != null)
                {
                    return Ok(TenureandInterstRateCount); // Format is "TenureCount-Minimum Interest Rate-Maximum Interest Rate"
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
        [Route("api/Banking/GetRDMaturityamount")]
        public IActionResult GetRDMaturityamount(string pInterestMode, long pInterestTenure, decimal instalmentamount, string pInterestPayOut, string pCompoundorSimpleInterestType, decimal pInterestRate, string pCalType)
        {
            List<RDMatuerityamount> lstmaturity = new List<RDMatuerityamount>();
            try
            {
                lstmaturity = RdtransactionDAL.GetRDMaturityamount(pInterestMode, pInterestTenure, instalmentamount, pInterestPayOut, pCompoundorSimpleInterestType, pInterestRate, pCalType, Con);
                if (lstmaturity != null && lstmaturity.Count > 0)
                {
                    return Ok(lstmaturity);
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
        [Route("api/Banking/GetRdSchemeDetailsforGrid")]
        public async Task<IActionResult> GetRdSchemeDetailsforGrid(string Rdname, string ApplicantType, string MemberType)
        {
            try
            {
                List<RdSchemeData> _RdSchemeDataList = await RdtransactionDAL.GetRdSchemeDetailsforGrid(Rdname, ApplicantType,MemberType, Con);
                if (_RdSchemeDataList != null && _RdSchemeDataList.Count > 0)
                {
                    return Ok(_RdSchemeDataList);
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
        [Route("api/Banking/Masters/GetRDallJointMembers")]
        public async Task<IActionResult> GetRDallJointMembers(string membercode, string Contacttype)
        {
            try
            {
                List<RDFiMemberContactDetails> _FiMemberContactDetailsList = await RdtransactionDAL.GetallJointMembers(membercode, Contacttype, Con);
                if (_FiMemberContactDetailsList != null && _FiMemberContactDetailsList.Count > 0)
                {
                    return Ok(_FiMemberContactDetailsList);
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
        [Route("api/Banking/GetRdSchemeTenureModes")]
        public async Task<IActionResult> GetRdSchemeTenureModes(string Rdname, string ApplicantType,string MemberType)
        {
            try
            {
                List<RDTenureModes> _TenureModesList = await RdtransactionDAL.GetRdSchemeTenureModes(Rdname, ApplicantType, MemberType, Con);
                if (_TenureModesList != null && _TenureModesList.Count > 0)
                {
                    return Ok(_TenureModesList);
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

        [Route("api/Banking/GetRDDepositamountCountofInterestRate")]
        [HttpGet]
        public IActionResult GetRDDepositamountCountofInterestRate(string Rdname, decimal instalmentamount,string MemberType)
        {
            try
            {
                int count = RdtransactionDAL.GetRDDepositamountCountofInterestRate(Rdname, instalmentamount, MemberType, Con);
                if (count > 0)
                {
                    return Ok(count);
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
        [Route("api/Banking/GetRDInterestamountsofTable")]
        public async Task<IActionResult> GetRDInterestamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount,string MemberType)
        {
            try
            {
                List<RdTransactinInterestAmountofTable> _RdTransactinMaturityAmountofTableList = await RdtransactionDAL.GetRDInterestamountsofTable(RDName, RdconfigId, TenureMode, Tenure, instalmentamount, MemberType, Con);

                if (_RdTransactinMaturityAmountofTableList != null && _RdTransactinMaturityAmountofTableList.Count > 0)
                {
                    return Ok(_RdTransactinMaturityAmountofTableList);
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
        [Route("api/Banking/GetRdTransactionDetailsforEdit")]
        public async Task<IActionResult> GetRdTransactionDetailsforEdit(string RdAccountNo, long RdAccountId, string accounttype)
        {
            //RdTransactionDataEdit _RdTransactionDataEdit = new RdTransactionDataEdit();
            try
            {
                RdTransactionDataEdit _RdTransactionDataEdit = await RdtransactionDAL.GetRdTransactionDetailsforEdit(RdAccountNo, RdAccountId, accounttype, Con);
                if (_RdTransactionDataEdit != null)
                {
                    return Ok(_RdTransactionDataEdit);
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
        [Route("api/Banking/GetRDMaturityamountsofTable")]
        public async Task<IActionResult> GetRDMaturityamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount,string MemberType)
        {
            try
            {
                List<RdTransactinMaturityAmountofTable> _RdTransactinInterestAmountofTableList = await RdtransactionDAL.GetRDMaturityamountsofTable(RDName, RdconfigId, TenureMode, Tenure, instalmentamount, Interestamount, MemberType, Con);

                if (_RdTransactinInterestAmountofTableList != null && _RdTransactinInterestAmountofTableList.Count > 0)
                {
                    return Ok(_RdTransactinInterestAmountofTableList);
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
        [Route("api/Banking/GetRDPayoutsofTable")]
        public async Task<IActionResult> GetRDPayoutsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount, decimal Maturityamount)
        {
            try
            {
                List<RdInterestPayout> _RdInterestPayoutList = await RdtransactionDAL.GetRDPayoutsofTable(RDName, RdconfigId, TenureMode, Tenure, instalmentamount, Interestamount, Maturityamount, Con);

                if (_RdInterestPayoutList != null && _RdInterestPayoutList.Count > 0)
                {
                    return Ok(_RdInterestPayoutList);
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
        [Route("api/Banking/GetRdInstallmentchart")]
        public async Task<IActionResult> GetRdInstallmentchart(string Rdaccountno)
        {
            try
            {
                List<RDInstallmentchart> _RDInstallmentchartList = await RdtransactionDAL.GetRdInstallmentchart(Rdaccountno,Con);
                if (_RDInstallmentchartList != null && _RDInstallmentchartList.Count > 0)
                {
                    return Ok(_RDInstallmentchartList);
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

    }
}