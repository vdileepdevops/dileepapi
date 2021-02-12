using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Reports;
namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface ICollectionReport
    {
        #region GetColletionsummary
        List<CollectionReportDTO> GetColletionsummary(string fromdate, string todate,string ConnectionString, int recordid, string fieldname, string fieldtype);
        #endregion 

        #region GetColletiondetails
        List<CollectionReportDetailsDTO> GetColletiondetails(string fromdate, string todate, string Applicationid, string ConnectionString);
        #endregion 
    }
}
