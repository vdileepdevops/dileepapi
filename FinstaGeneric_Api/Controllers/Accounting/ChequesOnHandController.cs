using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.Accounting
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ChequesOnHandController : ControllerBase
    {
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        IChequesOnHand _ChequesOnHand = new ChequesOnHandDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public List<BankDTO> banklist { get; set; }
        public ChequesOnHandDTO _ChequesOnHandDTO { get; set; }
        public ChequesOnHandDTO _ChequesInBankDTO { get; set; }

        public ChequesOnHandController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [HttpGet]
        [Route("api/Accounting/ChequesOnHand/GetBanksList")]
        public async Task<IActionResult> GetBanksList()
        {

            banklist = new List<BankDTO>();
            try
            {
                banklist = await _AccountingTransactionsDAL.GetBankntList(Con);
                return Ok(banklist);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [Route("api/Accounting/ChequesOnHand/GetChequesOnHandData_New")]
        [HttpGet]
        public async Task<IActionResult> GetChequesOnHandData_New(string BrsFromDate, string BrsTodate, Int64 _BankId)
        {
            _ChequesOnHandDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesOnHandDTO.pchequesclearreturnlist = await _ChequesOnHand.GetDepositedCancelledCheques(Con, BrsFromDate, BrsTodate, _BankId);

                return Ok(_ChequesOnHandDTO);
            }
            catch (Exception ex)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError);
                //throw new FinstaAppException(ex.ToString());
                return Ok(ex.ToString());
            }

        }

        [Route("api/Accounting/ChequesOnHand/GetChequesOnHandData")]
        [HttpGet]
        public async Task<IActionResult> GetChequesOnHandData(Int64 _BankId)
        {
            _ChequesOnHandDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesOnHandDTO.pchequesOnHandlist = await _ChequesOnHand.GetChequesOnHandDetails(Con);
                _ChequesOnHandDTO.pchequesclearreturnlist = await _ChequesOnHand.GetDepositedCancelledCheques(Con, null, null, _BankId);

                return Ok(_ChequesOnHandDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/Accounting/ChequesOnHand/SaveChequesOnHand")]
        public IActionResult SaveChequesOnHand(ChequesOnHandDTO ChequesOnHanddto)
        {
            try
            {

                if (_ChequesOnHand.SaveChequesOnHand(ChequesOnHanddto, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        [HttpGet]
        [Route("api/Accounting/ChequesOnHand/GetBankBalance")]
        public IActionResult GetBankBalance(Int64 _recordid)
        {
            _ChequesOnHandDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesOnHandDTO = _ChequesOnHand.GetBankBalance(_recordid, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(_ChequesOnHandDTO);
        }
        [Route("api/Accounting/ChequesOnHand/GetChequesIssued")]
        [HttpGet]
        public async Task<IActionResult> GetChequesIssued(Int64 _BankId)
        {
            _ChequesOnHandDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesOnHandDTO.pchequesOnHandlist = await _ChequesOnHand.GetIssuedDetails(Con, _BankId);
                _ChequesOnHandDTO.pchequesclearreturnlist = await _ChequesOnHand.GetIssuedCancelledCheques(Con, null, null, _BankId);

                return Ok(_ChequesOnHandDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        [Route("api/Accounting/ChequesOnHand/GetIssuedCancelledCheques_New")]
        [HttpGet]
        public async Task<IActionResult> GetIssuedCancelledCheques_New(string BrsFromDate, string BrsTodate, Int64 _BankId)
        {
            _ChequesOnHandDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesOnHandDTO.pchequesclearreturnlist = await _ChequesOnHand.GetIssuedCancelledCheques(Con, BrsFromDate, BrsTodate, _BankId);

                return Ok(_ChequesOnHandDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        [HttpPost]
        [Route("api/Accounting/ChequesOnHand/SaveChequesIssued")]
        public IActionResult SaveChequesIssued(ChequesOnHandDTO ChequesOnHanddto)
        {
            try
            {

                if (_ChequesOnHand.SaveChequesIssued(ChequesOnHanddto, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #region ChequesInBank
        [HttpGet]
        [Route("api/Accounting/ChequesOnHand/GetChequesInBankData")]
        public async Task<IActionResult> GetChequesInBankData(long depositedBankid)
        {
            _ChequesInBankDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesInBankDTO.pchequesOnHandlist = await _ChequesOnHand.GetChequesInBankData(Con, depositedBankid);
                _ChequesInBankDTO.pchequesclearreturnlist = await _ChequesOnHand.GetClearedReturnedCheques(Con, null, null, depositedBankid);

                return Ok(_ChequesInBankDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/Accounting/ChequesOnHand/GetClearedReturnedCheques_New")]
        public async Task<IActionResult> GetClearedReturnedCheques_New(string BrsFromDate, string BrsTodate, long depositedBankid)
        {
            _ChequesInBankDTO = new ChequesOnHandDTO();
            try
            {
                _ChequesInBankDTO.pchequesclearreturnlist = await _ChequesOnHand.GetClearedReturnedCheques(Con, BrsFromDate, BrsTodate, depositedBankid);

                return Ok(_ChequesInBankDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpPost]
        [Route("api/Accounting/ChequesOnHand/SaveChequesInBank")]
        public IActionResult SaveChequesInBank(ChequesOnHandDTO ChequesOnHanddto)
        {
            try
            {

                if (_ChequesOnHand.SaveChequesInBank(ChequesOnHanddto, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion
    }
}