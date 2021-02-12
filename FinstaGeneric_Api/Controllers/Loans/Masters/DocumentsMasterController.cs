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
    public class DocumentsMasterController : ControllerBase
    {
        IDocumentsMaster obj = new DocumentsMasterDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        public List<DocumentsMasterDTO> lstDocumentsMasterDTO { set; get; }
        public DocumentsMasterController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/loans/masters/documentsmaster/SaveDocumentGroup")]
        [HttpPost]
        public async Task<IActionResult> SaveDocumentGroup(DocumentsMasterDTO Documents)
        {
            bool isSaved = false;
            try
            {
                isSaved =await obj.SaveDocumentGroup(Documents,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/loans/masters/documentsmaster/SaveIdentificationDocuments")]
        [HttpPost]
        public async Task<IActionResult> SaveIdentificationDocuments(DocumentsMasterDTO Documents)
        {
            bool isSaved = false;
            try
            {
                isSaved = await obj.SaveIdentificationDocuments(Documents,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/loans/masters/documentsmaster/GetdocumentidproffDetails")]
        [HttpPost]
        public IActionResult GetdocumentidproffDetails(Int64 pLoanId)
        {
            List<pIdentificationDocumentsDTO> lstpIdentificationDocumentsDTO = new List<pIdentificationDocumentsDTO>();
            try
            {
                lstpIdentificationDocumentsDTO = obj.GetdocumentidproffDetails(Con, pLoanId);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstpIdentificationDocumentsDTO);

        }

        [Route("api/loans/masters/documentsmaster/Getdocumentidprofftypes")]
        [HttpPost]
        public IActionResult Getdocumentidprofftypes(Int64 pLoanId)
        {
            lstDocumentsMasterDTO = new List<DocumentsMasterDTO>();
            try
            {
                lstDocumentsMasterDTO= obj.Getdocumentidprofftypes(Con, pLoanId);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstDocumentsMasterDTO);

        }

        [Route("api/loans/masters/documentsmaster/GetDocumentGroupNames")]
        [HttpGet]
        public async Task<IActionResult> GetDocumentGroupNames()
        {
            lstDocumentsMasterDTO = new List<DocumentsMasterDTO>();
            try
            {
                lstDocumentsMasterDTO = await obj.GetDocumentGroupNames(Con);
            }
            catch (Exception ex)
            {

                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstDocumentsMasterDTO);
        }

        #region CheckDuplicateGroupNames
        [Route("api/loans/masters/documentsmaster/CheckDuplicateGroupNames")]
       [HttpGet]
        public int CheckDuplicateGroupNames(string DocGroupName)
        {
            try
            {
                return obj.CheckDuplicateDocGroupNames(DocGroupName,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion
        #region CheckDuplicatedocNamesBasedonGroupName
        [Route("api/loans/masters/documentsmaster/CheckDuplicateDocNamesBasedonGroupName")]
        [HttpGet]
        public int CheckDuplicateDocNamesBasedonGroupName(string DocGroupName,string DocName, Int64 DocumenId)
        {
            try
            {
                return obj.CheckDuplicateDocbasedonGroupNames(DocGroupName,DocName,DocumenId,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        [Route("api/loans/masters/documentsmaster/UpdateIdentificationDocuments")]
        [HttpPost]
        public async Task<IActionResult> UpdateIdentificationDocuments(DocumentsMasterDTO Documents)
        {
            bool isSaved = false;
            try
            {
                isSaved = await obj.UpdateIdentificationDocuments(Documents, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/loans/masters/documentsmaster/DeleteIdentificationDocuments")]
        [HttpPost]
        public async Task<IActionResult> DeleteIdentificationDocuments(DocumentsMasterDTO Documents)
        {
            bool isSaved = false;
            try
            {
                isSaved = await obj.DeleteIdentificationDocuments(Documents, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
    }
}