using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure
{
    public class CommonDTO
    {
//        public int createdby = 1;
//        public int modifiedby = 1;

//        public int pCreatedby
//        {
//            get;
//            //{
//            //    return createdby;
//            //}
//            set;
//            //{
//            //    createdby = 1;
//            //}
//        }

//        public int pModifiedby
//        {
//            get
//            {
//                return modifiedby;
//            }
//            set
//            {
//                modifiedby = 1;
//            }
//        }

        public int createdby { set; get; }
        public int modifiedby { set; get; }

        public int pCreatedby { set; get; }



        public int pModifiedby { set; get; }


      //  public DateTime? pCreateddate { get; set; }
      //  public DateTime? pModifieddate { get; set; }
        public string pStatusid { get; set; }
        public string pStatusname { get; set; }

        public string pEffectfromdate { get; set; }

        public string pEffecttodate { get; set; }
        public string ptypeofoperation { get; set; }

      

    }
    public class DesignationDTO : CommonDTO
    {
        public object designationid { get; set; }
        public object designationname { get; set; }
    }
}
