using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;

namespace FinstaInfrastructure.Settings
{
    public class CompanyDTO : CommonDTO
    {
        public Int64 pCompanyId { set; get; }
        public Int64 pRecordId { set; get; }
        public string pnameofenterprise { set; get; }
        public string penterprisecode { set; get; }
        public string ppancard { set; get; }
        public string ptypeofenterprise { set; get; }
        public string pestablishmentdate { set; get; }
        public string pcommencementdate { set; get; }
        public string pcinnumber { set; get; }
        public string pgstinnumber { set; get; }
        public bool ptransactionlock { set; get; }
        public string plocktime { set; get; }
        public bool pdatepickerenablestatus { set; get; }
        public string pcompanyumrn { set; get; }
        public string ptransactionopeningdate { set; get; }
        public string pdefaultpassword { set; get; }
        public string pdatepickerenableenddate { set; get; }
        public List<CompanyAddressDTO> lstCompanyAddressDTO { set; get; }
        public List<CompanyContactDTO> lstCompanyContactDTO { set; get; }
        public List<CompanyDocumentsDTO> lstCompanyDocumentsDTO { set; get; }
        public List<CompanyPromotersDTO> lstCompanyPromotersDTO { set; get; }
    }
    public class CompanyAddressDTO : CommonDTO
    {
        public Int64 pCompanyId { set; get; }
        public Int64 pRecordId { set; get; }
        public string pAddressType { get; set; }
        public string pAddress1 { get; set; }
        public string pAddress2 { get; set; }
        public string pState { get; set; }
        public Int64 pStateId { get; set; }
        public string pDistrict { get; set; }
        public Int64 pDistrictId { get; set; }
        public string pCity { get; set; }
        public int pCityId { get; set; }
        public string pCountry { get; set; }
        public Int64 pCountryId { get; set; }
        public Int64 pPinCode { get; set; }
        public string pPriority { get; set; }
    }
    public class CompanyContactDTO : CommonDTO
    {
        public Int64 pCompanyId { set; get; }
        public Int64 pRecordId { set; get; }
        public Int64 pcontactnumber { set; get; }
        public string pemailid { set; get; }
        public string ppriority { set; get; }
    }
    public class CompanyDocumentsDTO : CommonDTO
    {
        public Int64 pCompanyId { set; get; }
        public Int64 pRecordId { set; get; }
        public Int64 pDOCUMENTID { set; get; }
        public Int64 pDOCUMENTGROUPID { set; get; }
        public string pDOCUMENTGROUPNAME { set; get; }
        public string pDOCUMENTNAME { set; get; }
        public string pDOCSTOREPATH { set; get; }
        public string pDOCFILETYPE { set; get; }
        public string pDOCFILENAME { set; get; }
        public string pDOCREFERENCENO { set; get; }
        public bool pDOCISDOWNLOADABLE { set; get; }

    }
    public class CompanyPromotersDTO : CommonDTO
    {
        public Int64 pCompanyId { set; get; }
        public Int64 pRecordId { set; get; }
        public Int64 pContactId { set; get; }

    }
}
