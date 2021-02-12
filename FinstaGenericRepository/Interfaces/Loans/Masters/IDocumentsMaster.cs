using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Masters;
using System.Threading.Tasks;


namespace FinstaRepository.Interfaces.Loans.Masters
{
   public interface IDocumentsMaster
    {
        Task<bool> SaveDocumentGroup(DocumentsMasterDTO Documents,string ConnectionString);
        Task<bool> SaveIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString);
        List<DocumentsMasterDTO> Getdocumentidprofftypes(string ConnectionString, Int64 pLoanId);
        List<pIdentificationDocumentsDTO> GetdocumentidproffDetails(string ConnectionString, Int64 pLoanId);
        Task<List<DocumentsMasterDTO>> GetDocumentGroupNames(string ConnectionString);
        #region checkduplicategroupnamesanddocuments
        int CheckDuplicateDocGroupNames(string DocGroupName,string ConnectionString);
        int CheckDuplicateDocbasedonGroupNames(string DocGroupName,string DocumentName, Int64 DocumenId,string ConnectionString);
        #endregion
        #region UpdateDocumentNames
        Task<bool> UpdateIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString);
        #endregion
        #region DeleteDocumentNames
        Task<bool> DeleteIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString);
        #endregion

    }
}
