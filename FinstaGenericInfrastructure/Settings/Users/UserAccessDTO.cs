using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Settings.Users
{
   public class UserAccessDTO
    {
        public long pUserID { set; get; }
        public string pContactRefID { set; get; }
        public string pUserName { set; get; }
        public long pRoleid { set; get; }
        public int pRoleFunctionsCOunt { set; get; }
        public bool pOtpAuthentication { get; set; }
        public string pMobile{ set; get; }
        public string pEmail { set; get; }
        public string pOtp { set; get; }
        public string pRoleName { set; get; }
        public string pPassword { set; get; }
        public string pUserType { set; get; }
        public int pstatusid { set; get; }
        public int pCreatedby { set; get; }
        public string pSaltKey { set; get; }
        public string pEmployeeName { set; get; }
        public string pToken { set; get; }
        public string pStatus { set; get; }
        public string PUserorDesignation { set; get; }
        public bool pActiveorInactive { set; get; }
        public string pRefreshToken { set; get; }
        public string pMessage { set; get; }

    }

    public class ContactEmployeeDTO
    {
        public long pContactID { set; get; }
        public string pContactRefID { set; get; }
        public long pEmployeeid { set; get; }
        public string pEmployeeName { set; get; }
        public long pRoleid { set; get; }
        public string pRoleName { set; get; }
    }
    public class ValidateResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
   public class ValidateDTO
    {
        public string pMobile { set; get; }
        public long pUserId { set; get; }
        public string pOtp { set; get; }

    }
}
