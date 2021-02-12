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
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Transactions
{
    //[Authorize]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class FDTransactionController : ControllerBase
    {
        IFDTransaction FdtransactionDAL = new FDTransactionDAL();
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public FDTransactionController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// API for Binding Member and Applicant types based on Contact type
        /// api/Banking/Masters/FIMember/GetapplicantTypes

        /// API for Binding all FI Member for Joint Members
        /// api/Banking/Masters/GetallFIMembers
        ///  Changed to below API
        /// <summary>
        /// Get all FI Members Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/GetallJointMembers")]
        public async Task<IActionResult> GetallJointMembers(string membercode,string Contacttype)
        {
            try
            {
                List<FiMemberContactDetails> _FiMemberContactDetailsList = await FdtransactionDAL.GetallJointMembers(membercode, Contacttype,Con);
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

        /// <summary>
        /// Bind Members based on Contact and Member Types Names
        /// </summary>
        /// <param name="Contacttype"></param>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetallFDMembers")]
        public async Task<IActionResult> GetFDMembers(string Contacttype,string MemberType)
        {            
            try
            {
                List<FdMembersandContactDetails> _FdMembers = await FdtransactionDAL.GetFDMembers(Contacttype, MemberType, Con);
                if (_FdMembers != null && _FdMembers.Count>0)
                {
                    return Ok(_FdMembers);
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
        /// Get Nominee Details of Member Based on Member_Code
        /// </summary>
        /// <param name="MemberCode"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("api/Banking/GetFDMemberNomineeDetails")]
        //public async Task<IActionResult> GetFDMemberNomineeDetails(string MemberCode)
        //{
        //    try
        //    {
        //        List<FDMemberNomineeDetails> _FDMemberNomineeDetails = await FdtransactionDAL.GetFDMemberNomineeDetails(MemberCode,  Con);
        //        if (_FDMemberNomineeDetails != null && _FDMemberNomineeDetails.Count>0)
        //        {
        //            return Ok(_FDMemberNomineeDetails);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status204NoContent);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        /// <summary>
        /// Get Fd Schemes on Passing Membertype and ApplicantType Names
        /// </summary>
        /// <param name="ApplicantType"></param>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdSchemes")]
        public async Task<IActionResult> GetFdSchemes(string ApplicantType, string MemberType)
        {
            try
            {
                List<FdNameandCode> _FdNameandCodeList = await FdtransactionDAL.GetFdSchemes(ApplicantType, MemberType, Con);
                if (_FdNameandCodeList != null && _FdNameandCodeList.Count>0)
                {
                    return Ok(_FdNameandCodeList);
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
       /// Get scheme details
       /// </summary>
       /// <param name="ApplicantType"></param>
       /// <param name="MemberType"></param>
       /// <param name="FdconfigID"></param>
       /// <param name="Fdname"></param>
       /// <param name="Tenure"></param>
       /// <param name="Tenuremode"></param>
       /// <param name="Depositamount"></param>
       /// <param name="InterestRate"></param>
       /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdSchemeDetails")]
        public async Task<IActionResult> GetFdSchemeDetails(string ApplicantType, string MemberType,long FdconfigID,string Fdname,long Tenure,string Tenuremode,decimal Depositamount)
        {
            try
            {
                FDdetailsFromScheme _FDdetailsFromScheme = await FdtransactionDAL.GetFdSchemeDetails(ApplicantType, MemberType, FdconfigID, Fdname, Tenure, Tenuremode, Depositamount, Con);
                if (_FDdetailsFromScheme != null )
                {
                    return Ok(_FDdetailsFromScheme);
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
        /// Save Fd Transaction 1st and 2nd Tabs
        /// </summary>
        /// <param name="_FdMemberandSchemeData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/SaveFDMemberandSchemeData")]
        public IActionResult SaveFDMemberandSchemeData([FromBody] FdMemberandSchemeSave _FdMemberandSchemeData)
        {
            FdComfigurationIdandName _FdComfigurationIdandName = new FdComfigurationIdandName();
            try
            {
                long pFdAccountId = 0;
                string FdaccountNo = FdtransactionDAL.SaveFDMemberandSchemeData(_FdMemberandSchemeData, Con, out pFdAccountId);
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

        /// <summary>
        /// FD Joint Members and Nominees Save
        /// </summary>
        /// <param name="_FdJointandNomineeSave"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/SaveFDJointMembersandNomineeData")]
        //public IActionResult SaveFDJointMembersandNomineeData([FromBody] FdJointandNomineeSave _FdJointandNomineeSave)
        //{            
        //    try
        //    {
        //        if (_FdJointandNomineeSave.FDMemberNomineeDetailsList .Count > 0)
        //        {
        //            string OldFolder = "Upload";
        //            string NewFolder = "Original";
        //            string webRootPath = _hostingEnvironment.ContentRootPath;
        //            string OldPath = Path.Combine(webRootPath, OldFolder);
        //            string newPath = Path.Combine(webRootPath, NewFolder);
        //            if (!Directory.Exists(newPath))
        //            {
        //                Directory.CreateDirectory(newPath);
        //            }
        //            foreach (FDMemberNomineeDetails kycDoc in _FdJointandNomineeSave.FDMemberNomineeDetailsList)
        //            {
        //                if (!string.IsNullOrEmpty(kycDoc.pdocidproofpath))
        //                {
        //                    string OldFullPath = Path.Combine(OldPath, kycDoc.pdocidproofpath);
        //                    string NewFullPath = Path.Combine(newPath, kycDoc.pdocidproofpath);
        //                    kycDoc.pdocidproofpath = NewFullPath;
        //                    if (System.IO.File.Exists(OldFullPath))
        //                    {
        //                        System.IO.File.Move(OldFullPath, NewFullPath);
        //                    }
        //                }
        //            }
        //        }
        //        if (FdtransactionDAL.SaveFDJointMembersandNomineeData(_FdJointandNomineeSave, Con))
        //        {
        //            return Ok(true);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status304NotModified);
        //        }
        //    }
        //    catch (Exception eX)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, eX);
        //    }
        //}

        /// <summary>
        /// Save Fd Transaction Referral Details on passing Object
        /// </summary>
        /// <param name="_FdTransactionReferrals"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/SaveFDReferralData")]
        //public IActionResult SaveFDReferralData([FromBody] FdTransactionReferrals _FdTransactionReferrals)
        //{
        //    try
        //    {
        //        if (FdtransactionDAL.SaveFDReferralData(_FdTransactionReferrals, Con))
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
        /// Delete Fd Transaction on Passing FdAccountNo and userId
        /// </summary>
        /// <param name="FdAccountNo"></param>
        /// <param name="pCreatedby"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/DeleteFdTransactions")]
        public IActionResult DeleteFdTransactions(string FdAccountNo,long pCreatedby)
        {
            try
            {
                if (FdtransactionDAL.DeleteFdTransactions(FdAccountNo, pCreatedby, Con))
                {
                    return Ok(true);
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
        /// Fd View Data for Main Page with Active status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdTransactionData")]
        public async Task<IActionResult> GetFdTransactionData()
        {
            try
            {
                List< FdTransactionMainGridData> FdTransactionMainGridDataList = await FdtransactionDAL.GetFdTransactionData( Con);
                if (FdTransactionMainGridDataList != null && FdTransactionMainGridDataList.Count>0)
                {
                    return Ok(FdTransactionMainGridDataList);
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
        /// Get Insurance Member data on Passing Member Reference ID for Edit
        /// </summary>
        /// <param name="FdAccountNo"></param>        
        ///  <param name="FdAccountId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdTransactionDetailsforEdit")]
        public async Task< IActionResult> GetFdTransactionDetailsforEdit(string FdAccountNo, long FdAccountId, string accounttype)
        {
            //FdTransactionDataEdit _FdTransactionDataEdit = new FdTransactionDataEdit();
            try
            {
                FdTransactionDataEdit _FdTransactionDataEdit = await FdtransactionDAL.GetFdTransactionDetailsforEdit(FdAccountNo, FdAccountId, Con, accounttype);
                if(_FdTransactionDataEdit!=null)
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

        /// <summary>
        /// Gets Tenures based on Fd name and Fd configId
        /// </summary>
        /// <param name="FDName"></param>
        /// <param name="FdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdTenuresofTable")]
        public async Task< IActionResult> GetFdTenuresofTable(string FDName,long FdconfigId,string TenureMode, string MemberType)
        {          
            try
            {
                List< FdTransactinTenuresofTable> _FdTransactinTenuresofTableList =await FdtransactionDAL.GetFdTenuresofTable(FDName, FdconfigId, TenureMode, MemberType, Con);

                if(_FdTransactinTenuresofTableList!=null && _FdTransactinTenuresofTableList.Count>0)
                {
                    return Ok(_FdTransactinTenuresofTableList);
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
        /// Gets Deposit amounts based on Tenure and Fdname, ConfigId
        /// </summary>
        /// <param name="FDName"></param>
        /// <param name="FdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <param name="Tenure"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdDepositamountsofTable")]
        public async Task<IActionResult> GetFdDepositamountsofTable(string FDName, long FdconfigId, string TenureMode,long Tenure, string MemberType)
        {
            try
            {
                List<FdTransactinDepositAmountofTable> _FdTransactinDepositAmountofTableList = await FdtransactionDAL.GetFdDepositamountsofTable(FDName, FdconfigId, TenureMode, Tenure,MemberType, Con);

                if(_FdTransactinDepositAmountofTableList!=null && _FdTransactinDepositAmountofTableList.Count>0)
                {
                    return Ok(_FdTransactinDepositAmountofTableList);
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
        /// Gets Branch Details of Chits
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetchitBranchDetails")]
        public async Task<IActionResult> GetchitBranchDetails()
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await FdtransactionDAL.GetchitBranchDetails(Con);
                if(_ChitBranchDetailsList !=null && _ChitBranchDetailsList.Count>0)
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

        /// <summary>
        /// Get Chit branch status for hide/show of Chitbranch in Fd Transaction
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetchitBranchstatus")]
        public async Task<IActionResult> GetchitBranchstatus()
        {
            try
            {
                object Status = await FdtransactionDAL.GetchitBranchstatus(Con);              
                if(Convert.ToString(Status)=="Y")
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent, false);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);                
            }
        }

        /// <summary>
        /// Get Maturity amount of update table
        /// </summary>
        /// <param name="pInterestMode"></param>
        /// <param name="pInterestTenure"></param>
        /// <param name="pDepositAmount"></param>
        /// <param name="pInterestPayOut"></param>
        /// <param name="pCompoundorSimpleInterestType"></param>
        /// <param name="pInterestRate"></param>
        /// <param name="pCalType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetMaturityamount")]
        public IActionResult GetMaturityamount( string pInterestMode,long pInterestTenure,decimal pDepositAmount,string pInterestPayOut,string pCompoundorSimpleInterestType,decimal pInterestRate,string pCalType)
        {
            List<Matuerityamount> lstmaturity = new List<Matuerityamount>();
            try
            {
                lstmaturity  =  FdtransactionDAL.GetMaturityamount(pInterestMode, pInterestTenure, pDepositAmount, pInterestPayOut, pCompoundorSimpleInterestType, pInterestRate, pCalType, Con);
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

        /// <summary>
        /// Get Interest Amount of Update table
        /// </summary>
        /// <param name="FDName"></param>
        /// <param name="FdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <param name="Tenure"></param>
        /// <param name="Depositamount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InterestamountsofTable")]
        public async Task<IActionResult> GetInterestamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure,decimal Depositamount, string MemberType)
        {
            try
            {
                List<FdTransactinInterestAmountofTable> _FdTransactinMaturityAmountofTableList = await FdtransactionDAL.GetInterestamountsofTable(FDName, FdconfigId, TenureMode, Tenure, Depositamount, MemberType, Con);

                if (_FdTransactinMaturityAmountofTableList != null && _FdTransactinMaturityAmountofTableList.Count > 0)
                {
                    return Ok(_FdTransactinMaturityAmountofTableList);
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
        /// Get Maturity amount of Update Table
        /// </summary>
        /// <param name="FDName"></param>
        /// <param name="FdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <param name="Tenure"></param>
        /// <param name="Depositamount"></param>
        /// <param name="Interestamount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/MaturityamountsofTable")]
        public async Task<IActionResult> GetMaturityamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount,decimal Interestamount, string MemberType)
        {
            try
            {
                List<FdTransactinMaturityAmountofTable> _FdTransactinInterestAmountofTableList = await FdtransactionDAL.GetMaturityamountsofTable(FDName, FdconfigId, TenureMode, Tenure, Depositamount, Interestamount, MemberType, Con);

                if (_FdTransactinInterestAmountofTableList != null && _FdTransactinInterestAmountofTableList.Count > 0)
                {
                    return Ok(_FdTransactinInterestAmountofTableList);
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
        /// Get Interest Payouts of Update Table
        /// </summary>
        /// <param name="FDName"></param>
        /// <param name="FdconfigId"></param>
        /// <param name="TenureMode"></param>
        /// <param name="Tenure"></param>
        /// <param name="Depositamount"></param>
        /// <param name="Interestamount"></param>
        /// <param name="Maturityamount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/PayoutsofTable")]
        public async Task<IActionResult> GetPayoutsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, decimal Interestamount,decimal Maturityamount)
        {
            try
            {
                List<FdInterestPayout> _FdInterestPayoutList = await FdtransactionDAL.GetPayoutsofTable(FDName, FdconfigId, TenureMode, Tenure, Depositamount, Interestamount, Maturityamount, Con);

                if (_FdInterestPayoutList != null && _FdInterestPayoutList.Count > 0)
                {
                    return Ok(_FdInterestPayoutList);
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
        /// Gets count of Deposit amount on passing Fdname and Entered Deposit amount
        /// </summary>
        /// <param name="Fdname"></param>
        /// <param name="Depositamount"></param>       
        /// <returns></returns>
        [Route("api/Banking/GetDepositamountCountofInterestRate")]        
        [HttpGet]
        public IActionResult GetDepositamountCountofInterestRate(string Fdname, decimal Depositamount, string MemberType)
        {          
            try
            {
               int count = FdtransactionDAL.GetDepositamountCountofInterestRate(Fdname, Depositamount, MemberType, Con);
                if (count>0)
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
        /// <summary>
        /// Gets count and minimum interest rate of Tenure and Depositamount on passing Fdname , Entered Deposit amount,Tenure and Mode of Tenure 
        /// </summary>
        /// <param name="Fdname"></param>
        /// <param name="Depositamount"></param>
        /// <param name="Tenure"></param>
        /// <param name="TenureMode"></param>   
        /// <param name="InterestPayout"></param>   
        /// <returns></returns>
        [Route("api/Banking/GetTenureandMininterestRateofInterestRate")]
        [HttpGet]
        public IActionResult GetTenureandMininterestRateofInterestRate(string Fdname, decimal Depositamount,long Tenure, string TenureMode,string InterestPayout, string MemberType)
        {           
            try
            {
                FdInterestRateValidation TenureandInterstRateCount = FdtransactionDAL.GetTenureandMininterestRateofInterestRate(Fdname, Depositamount, Tenure, TenureMode, InterestPayout, MemberType, Con);
                if(TenureandInterstRateCount!=null)
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
               
        /// <summary>
        /// Gets Scheme Data for Grid Binding
        /// </summary>
        /// <param name="Fdname"></param>
        /// <param name="ApplicantType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdSchemeDetailsforGrid")]
        public async Task<IActionResult> GetFdSchemeDetailsforGrid(string Fdname, string ApplicantType, string MemberType)
        {
            try
            {
                List<FdSchemeData> _FdSchemeDataList = await FdtransactionDAL.GetFdSchemeDetailsforGrid(Fdname, ApplicantType, MemberType, Con);
                if (_FdSchemeDataList != null && _FdSchemeDataList.Count > 0)
                {
                    return Ok(_FdSchemeDataList);
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
        /// Gets Tenure Modes of Scheme
        /// </summary>
        /// <param name="Fdname"></param>
        /// <param name="ApplicantType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/GetFdSchemeTenureModes")]
        public async Task<IActionResult> GetFdSchemeTenureModes(string Fdname, string ApplicantType, string MemberType)
        {
            try
            {
                List<TenureModes> _TenureModesList = await FdtransactionDAL.GetFdSchemeTenureModes(Fdname, ApplicantType, MemberType, Con);
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



    }
}