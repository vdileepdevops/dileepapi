using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;

namespace FinstaInfrastructure.Settings
{
    public class BranchDTO : CommonDTO
    {
        public Int64 pbranchid { set; get; }
        public Int64 pRecordId { set; get; }
        public string pbranchname { set; get; }
        public string pbranchcode { set; get; }
        public string pestablishmentdate { set; get; }
        public Int64 pcontactnumber { set; get; }
        public string pemailid { set; get; }
        public string pgstinnumber { set; get; }
        public string pstatename { set; get; }
        public Int64 pstatecode { set; get; }
        public Int64 pstateid { set; get; }
        public List<BranchAddressDTO> lstBranchAddressDTO { set; get; }
        public List<CompanyDocumentsDTO> lstBranchDocStoreDTO { set; get; }
    }
    public class BranchAddressDTO
    {
        public Int64 pbranchid { set; get; }
        public Int64 pRecordid { set; get; }
        public string pAddressType { get; set; }
        public string paddress1 { get; set; }
        public string paddress2 { get; set; }
        public string pState { get; set; }
        public Int64 pStateId { get; set; }
        public string pDistrict { get; set; }
        public Int64 pDistrictId { get; set; }
        public string pcity { get; set; }
        public int pCityId { get; set; }
        public string pCountry { get; set; }
        public Int64 pCountryId { get; set; }
        public Int64 pPincode { get; set; }
        public string pPriority { get; set; }
        public string ptypeofoperation { get; set; }
    }

    //public class BranchDocStoreDTO
    //{
    //    public Int64 pbranchid { set; get; }
    //    public Int64 pRecordId { set; get; }
    //    public Int64 pdocumentid { get; set; }
    //    public Int64 pdocumentgroupid { get; set; }
    //    public string preferencenumber { get; set; }
    //    public string pprooffilepath { get; set; }
    //}
}
