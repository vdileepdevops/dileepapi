using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{
    public class ContactAddressDTO : CommonDTO
    {
        public string pAddress1 { set; get; }
        public string pAddress2 { set; get; }
        public string pcity { set; get; }
        public string pCountry { set; get; }
        public string pState { set; get; }
        public string pDistrict { set; get; }
        public string pPincode { set; get; }
        public long pstateid { get; set; }
        public long pcountryid { get; set; }
        public long pdistrictid { get; set; }
    }
}
