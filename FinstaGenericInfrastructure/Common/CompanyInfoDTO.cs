using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{  
    public class CompanyInfoDTO
    {
        public string pCompanyName { get; set; }
        public string pAddress1 { set; get; }
        public string pAddress2 { set; get; }
        public string pcity { set; get; }
        public string pCountry { set; get; }
        public string pState { set; get; }
        public string pDistrict { set; get; }
        public string pPincode { set; get; }
        public string pCinNo { get; set; }
        public string pGstinNo { get; set; }
        public string pBranchname { get; set; }
        public bool pdatepickerenablestatus { get; set; }
    }    
}
