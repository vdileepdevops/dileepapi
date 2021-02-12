using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaRepository.DataAccess.Loans.Transactions;
using FinstaRepository.Interfaces.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Loans.Transactions;
using FinstaApi.Common;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Loans.Transactions
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class DisbursementController : ControllerBase
    {
        IDisbursement _DisbursementDAL = new DisbursementDAL();
        public DisbursementDTO _DisbursementDTO { get; set; }
        public List<DisbursementReportDetailsDTO> pdisbursedlist { get; set; }
        public List<DisbursementReportDuesDetailsDTO> pdisbursementdueslist { get; set; }
        ISettings obj = new SettingsDAL();

        string Con = string.Empty;
        static IConfiguration _iconfiguration;

        public DisbursementController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetApprovedApplicationsByID")]
        public async Task<IActionResult> GetApprovedApplicationsByID(string vchapplicationid)
        {

            _DisbursementDTO = new DisbursementDTO();
            try
            {
                _DisbursementDTO = await _DisbursementDAL.GetApprovedApplicationsByID(vchapplicationid, Con);
                return Ok(_DisbursementDTO);
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
                //throw new FinstaAppException(ex.ToString());

                //return StatusCode(StatusCodes.Status500InternalServerError);
                //throw new FinstaAppException(ex.ToString());
            }



        }

        [HttpPost]
        [Route("api/loans/Transactions/Disbursement/GetDisbursedReportDetails")]
        public async Task<IActionResult> GetDisbursedReportDetails(DisbursementReportDTO _DisbursementReportDTO)
        {

            //DisbursementReportDTO _DisbursementReportDTO = new DisbursementReportDTO();



            try
            {
                _DisbursementReportDTO = await _DisbursementDAL.GetDisbursedReportDetails(_DisbursementReportDTO, Con);
                return Ok(_DisbursementReportDTO);
            }
            catch (Exception ex)
            {

                return Ok(ex);

                //return StatusCode(StatusCodes.Status500InternalServerError);
                //throw new FinstaAppException(ex.ToString());
            }



        }
        [HttpPost]
        [Route("api/loans/Transactions/Disbursement/GetDisbursedReportDuesDetails")]
        public async Task<IActionResult> GetDisbursedReportDuesDetails(DisbursementReportDTO _DisbursementReportDTO)
        {

            pdisbursementdueslist = new List<DisbursementReportDuesDetailsDTO>();
            try
            {
                pdisbursementdueslist = await _DisbursementDAL.GetDisbursedReportDuesDetails(_DisbursementReportDTO, Con);
                return Ok(pdisbursementdueslist);
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
                //throw new FinstaAppException(ex.ToString());

                //return StatusCode(StatusCodes.Status500InternalServerError);
                //throw new FinstaAppException(ex.ToString());
            }



        }

        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetDisbursementViewData")]
        public async Task<IActionResult> GetDisbursementViewData(string vchapplicationid)
        {

            DisbursementViewDTO _DisbursementViewDTO = new DisbursementViewDTO();
            try
            {
                _DisbursementViewDTO = await _DisbursementDAL.GetDisbursementViewData(Con);
                return Ok(_DisbursementViewDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }



        }

        [Route("api/loans/Transactions/Disbursement/SaveLoanDisbursement")]
        [HttpPost]
        public IActionResult SaveLoanDisbursement(DisbursementDTO _DisbursementDTO)
        {
            bool isSaved = false;
            List<string> lstdata = new List<string>();
            try
            {
                string paymentId = string.Empty;
                isSaved = _DisbursementDAL.SaveLoanDisbursement(_DisbursementDTO, Con, out paymentId);
                lstdata.Add(isSaved.ToString().ToUpper());
                lstdata.Add(paymentId);
                return Ok(lstdata);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());

            }
        }
        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetEmiChartReport")]
        public async Task<IActionResult> GetEmiChartReport(string vchapplicationid)
        {
            EmiChartReportDTO _EmiChartReportDTO = new EmiChartReportDTO();
            try
            {
                _EmiChartReportDTO = await _DisbursementDAL.GetEmiChartReport(vchapplicationid, Con);
                return Ok(_EmiChartReportDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }


        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetEmiChartView")]
        public async Task<IActionResult> GetEmiChartView()
        {
            List<EmiChartViewDTO> EmiChartViewlist = new List<EmiChartViewDTO>();
            EmiChartViewDTO _EmiChartViewDTO = new EmiChartViewDTO();
            try
            {
                EmiChartViewlist = await _DisbursementDAL.GetEmiChartViewData(Con);
                return Ok(EmiChartViewlist);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetDisbursementTrendReport")]
        public async Task<IActionResult> GetDisbursementTrendReport(string monthname)
        {
            DisbursementTrendDTO _DisbursementTrendDTO = new DisbursementTrendDTO();

            try
            {
                _DisbursementTrendDTO = await _DisbursementDAL.GetDisbursementTrendReport(monthname, Con);
                return Ok(_DisbursementTrendDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/loans/Transactions/Disbursement/GetCollectionandDuesTrendReport")]
        public async Task<IActionResult> GetCollectionandDuesTrendReport(string monthname, string type)
        {
            DisbursementTrendDTO _DisbursementTrendDTO = new DisbursementTrendDTO();

            try
            {
                _DisbursementTrendDTO = await _DisbursementDAL.GetCollectionandDuesTrendReport(monthname, type, Con);
                return Ok(_DisbursementTrendDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
    }
}