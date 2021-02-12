using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{
    public class ContactDTO : CommonDTO
    {
        public Int64 pContactId { get; set; }
        public string pTitleName { get; set; }
        public string pName { get; set; }
        public string pSurName { get; set; }
        public string pBusinessEntitycontactNo { get; set; }
        public string pBusinessEntityEmailId { get; set; }
        public string pContactimagepath { get; set; }
        public string pContactName { get; set; }
        public string pReferenceId { get; set; }
        public string pPhoto { get; set; }
        public string pFatherName { get; set; }
        public long? pRoleid { get; set; }
        public string pRolename { get; set; }
    }
}
