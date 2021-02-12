using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaRepository.Interfaces.Loans.Masters
{
    public interface ISchemeMaster
    {
        List<SchemeMasterDTO> getSchemeMasterDetails(string ConnectionString);
        List<SchemeMasterDTO> getSchemeMasterDetails(Int64 schemeid,string ConnectionString);
        SchemeMasterDTO getSchemeMasterDetailsbyId(Int64 schemeId,Int64 loanId,string ConnectionString);

        List<SchemeMasterDTO> getLoanspecificSchemeMasterDetails(string ConnectionString, Int64 loanid);

        bool saveSchemeMaster(SchemeMasterDTO schememasterlist, string connectionstring);

        #region CheckDuplicateSchemeNames
        int CheckDuplicateSchemeNames(string SchemeName,string ConnectionString);

        int CheckDuplicateSchemeCodes(string Schemecode, string ConnectionString);
        #endregion
        bool UpdateSchemeMaster(SchemeMasterDTO schememasterlist, string connectionstring);

        bool DeleteSchemeMaster(Int64 schemeid, int createdby, string con);
    }
}
