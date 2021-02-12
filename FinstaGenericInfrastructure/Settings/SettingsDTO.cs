using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Settings
{
    public class SettingsDTO : CommonDTO
    {
        public string pCountry { get; set; }
        public int pCountryId { get; set; }

        public string pState { get; set; }
        public int pStateId { get; set; }

        public string pDistrict { get; set; }
        public int pDistrictId { get; set; }

        public string pTitleName { get; set; }

        public string PEnterprisecode { get; set; }

        public string PBranchcode { get; set; }

        public string pContacttype { get; set; }
        public string pApplicanttype { get; set; }

        public string id { get; set; }

        public string text { get; set; }


    }

    public class GenerateidMasterDTO:CommonDTO
    {
        public string pFormName { set; get; }
        public int pFormId { set; get; }
        public int pTransactionModeId { set; get; }
        public string pFieldname { set; get; }
        public string pTransactionCode { set; get; }
        public string pTransactionSerice { set; get; }
        public string pFinanicalyear { set; get; }
        public string pNormalyear { set; get; }
        public string pSericeReset { set; get; }
        public int pRecordid { set; get; }
    }
    public class GenerateIdDTO : CommonDTO
    {
        public List<GenerateidMasterDTO> pGenerateidMasterlist { set; get; }
    }

}
