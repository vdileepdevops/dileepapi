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

namespace FinstaApi.Controllers.Loans.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ChargesMasterController : ControllerBase
    {
        IChargesMaster objChargemaster = new ChargesMasterDAL();
        public List<ChargesMasterDTO> lstchargesmaster { get; set; }
        public List<LoanchargetypesDTO> lstloantypecharges { get; set; }
        public List<LoanchargetypesConfigDTO> lstLoantypechargesconfig { get; set; }
        public List<PreclouserchargesDTO> lstLoanWisepreclousercharges { get; set; }

        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public ChargesMasterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region SaveChargesName

        [Route("api/loans/masters/ChargesMaster/SaveChargesName")]
        [HttpPost]
        public IActionResult SaveChargesName(ChargesMasterDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.SaveChargesName(_Charges, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region GetChargesName
        [Route("api/loans/masters/ChargesMaster/GetChargesName")]
        [HttpGet]
        public IActionResult GetChargesName(string chargeStatus)
        {
            lstchargesmaster = new List<ChargesMasterDTO>();
            try
            {
                lstchargesmaster = objChargemaster.GetChargesName(Con, chargeStatus);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstchargesmaster);

        }
        #endregion

        #region UpdateChargesName
        [Route("api/loans/masters/ChargesMaster/UpdateChargeName")]
        [HttpPost]
        public IActionResult UpdateChargesName(ChargesMasterDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.UpdateChargesName(_Charges, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region DeleteChargesName
        [Route("api/loans/masters/ChargesMaster/DeleteChargesName")]
        [HttpPost]
        public IActionResult DeleteChargesName(ChargesMasterDTO _Charges)
        {

            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.DeleteChargesName(_Charges, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion
        //Loan Wise Charges Assgining
        #region SaveLoanWiseChargesName

        [Route("api/loans/masters/ChargesMaster/SaveLoanWiseChargesName")]
        [HttpPost]
        public IActionResult SaveLoanWiseChargesName(ChargesMasterDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.SaveLoanWiseChargesName(_Charges, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region GetLoanWiseChargesName
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/GetLoanWiseChargesName")]
        public IActionResult GetLoanWiseChargesName(string loanid)
        {
            lstloantypecharges = new List<LoanchargetypesDTO>();
            try
            {
                lstloantypecharges = objChargemaster.GetLoanWiseChargesName(Con, loanid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstloantypecharges);

        }
        #endregion


        //Charges Configaration

        #region SaveLoanWiseChargeConfig
        [Route("api/loans/masters/ChargesMaster/SaveLoanWiseChargeConfig")]
        [HttpPost]
        public IActionResult SaveLoanWiseChargeConfig(ChargesMasterDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.SaveLoanWiseChargeConfig(_Charges, Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region GetLoanWiseApplicantTypes
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/GetLoanWiseApplicantTypes")]
        public IActionResult GetLoanWiseApplicantTypes(Int64 loanid)
        {
            lstLoantypechargesconfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                lstLoantypechargesconfig = objChargemaster.GetLoanWiseApplicantTypes(Con, loanid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoantypechargesconfig);

        }
        #endregion

        #region GetLoanWiseChargeConfig
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/GetLoanWiseChargeConfig")]
        public IActionResult GetLoanWiseChargeConfig(Int64 loanChargeid)
        {
            lstLoantypechargesconfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                lstLoantypechargesconfig = objChargemaster.GetLoanWiseChargeConfig(Con, loanChargeid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoantypechargesconfig);

        }
        #endregion

        #region ViewLoanWiseChargeConfig
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/ViewLoanWiseChargeConfig")]
        public IActionResult ViewLoanWiseChargeConfig()
        {
            lstLoantypechargesconfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                lstLoantypechargesconfig = objChargemaster.ViewLoanWiseChargeConfig(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoantypechargesconfig);

        }
        #endregion

        #region UpdateLoanWiseChargeConfig
        [Route("api/loans/masters/ChargesMaster/UpdateLoanWiseChargeConfig")]
        [HttpPost]
        public IActionResult UpdateLoanWiseChargeConfig(ChargesMasterDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.UpdateLoanWiseChargeConfig(_Charges, Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region DeleteLoanWiseChargeConfig
        [Route("api/loans/masters/ChargesMaster/DeleteLoanWiseChargeConfig")]
        [HttpPost]
        public IActionResult DeleteLoanWiseChargeConfig(LoanchargetypesConfigDTO _Charges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.DeleteLoanWiseChargeConfig(_Charges, Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion
        //PreClouser Charges
        #region CheckDuplicateLoanid
            
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/CheckDuplicateLoanid")]
        public int CheckDuplicateLoanid(long Loanid)
        {
            try
            {
                return objChargemaster.CheckDuplicateLoanid(Con, Loanid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region SavePreclouserCharges
        [Route("api/loans/masters/ChargesMaster/SavePreclouserCharges")]
        [HttpPost]
        public IActionResult SavePreclouserCharges(PreclouserchargesDTO _preclouserCharges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.SavePreclouserCharges(_preclouserCharges, Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region UpdatePreclouserCharges
        [Route("api/loans/masters/ChargesMaster/UpdatePreclouserCharges")]
        [HttpPost]
        public IActionResult UpdatePreclouserCharges(PreclouserchargesDTO _preclouserCharges)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.UpdatePreclouserCharges(_preclouserCharges, Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region DeletePreclouserCharges
        [Route("api/loans/masters/ChargesMaster/DeletePreclouserCharges")]
        [HttpPost]
        public IActionResult DeletePreclouserCharges(long Loanid, long Recordid,long userid)
        {
            bool isSaved = false;
            try
            {
                isSaved = objChargemaster.DeletePreclouserCharges(Con, Loanid,Recordid,userid);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion

        #region GetePreclouserCharges
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/GetePreclouserCharges")]
        public IActionResult GetePreclouserCharges(long Loanid, long Recordid)
        {
            lstLoanWisepreclousercharges = new List<PreclouserchargesDTO>();
            try
            {
                lstLoanWisepreclousercharges = objChargemaster.GetePreclouserCharges(Con, Loanid, Recordid);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanWisepreclousercharges);

        }
        #endregion

        #region ViewPreclouserCharges
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/ViewPreclouserCharges")]

        public IActionResult ViewPreclouserCharges()
        {
            lstLoanWisepreclousercharges = new List<PreclouserchargesDTO>();
            try
            {
                lstLoanWisepreclousercharges = objChargemaster.ViewPreclouserCharges(Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanWisepreclousercharges);
        }
        #endregion

        #region GetLoanWiseLoanpayin
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/GetLoanWiseLoanpayin")]
        public IActionResult GetLoanWiseLoanpayin(Int64 loanid,string applicanttype)
        {
            lstLoantypechargesconfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                lstLoantypechargesconfig = objChargemaster.GetLoanWiseLoanPayin(Con, loanid, applicanttype);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoantypechargesconfig);

        }


        #endregion

        #region CheckDuplicatechargeNames
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/CheckDuplicateChargeNames")]
        public int CheckDuplicateChargeNames(string ChargeName, Int64 chargeid)
        {
            try
            {
                return objChargemaster.CheckDuplicateChargeNames(ChargeName, chargeid,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region CheckDuplicatechargeNamesByLoanid
        [HttpGet]
        [Route("api/loans/masters/ChargesMaster/CheckDuplicateChargeNamesByLoanid")]
        public int CheckDuplicateChargeNamesByLoanid(string ChargeName,Int64 loanid)
        {
            try
            {
                return objChargemaster.CheckDuplicateChargeNamesByLoanid(ChargeName,loanid,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

    }
}