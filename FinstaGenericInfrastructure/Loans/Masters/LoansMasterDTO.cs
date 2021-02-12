using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Masters
{
    public class LoansMasterDTO : CommonDTO
    {
        public List<DocumentsMasterDTO> getidentificationdocumentsList { get; set; }

        public List<instalmentdatedetails> instalmentdatedetailslist { get; set; }

        public List<loanconfigurationDTO> loanconfigurationlist { get; set; }

        public List<DocumentlistDto> identificationdocumentsList { get; set; }

        public List<ReferralCommissioLoanDTO> ReferralCommissioLoanList { get; set; }

        public List<PenaltyConfigurationDTO> PenaltyConfigurationList { get; set; }

        public Int64 pLoanid { get; set; }
        public string pLoanNmae { get; set; }

        public int pLoantypeid { get; set; }
        public string pLoantype { get; set; }
        public string pLoanname { get; set; }
        public string pLoancode { get; set; }

        public string pCompanycode { get; set; }
        public string pBranchcode { get; set; }

        public string pSeries { get; set; }

        public int pSerieslength { get; set; }
        public string pLoanidcode { get; set; }

    }
    public class instalmentdatedetails : CommonDTO
    {
        public Int64 pLoantypeId { get; set; }
        public Int64 pLoanid { get; set; }
        public string pTypeofInstalmentDay { get; set; }
        public Int64? pDisbursefromday { get; set; }
        public Int64? pDisbursetoday { get; set; }
        public Int64 pInstalmentdueday { get; set; }
        public int ploantypeid { get; set; }
        public string ploantype { get; set; }

    }

    public class loanconfigurationDTO : CommonDTO
    {
        public Int64 pLoanconfigid { get; set; }
        public Int64 pLoantypeId { get; set; }
        public Int64 pLoanid { get; set; }
        public string pContacttype { get; set; }

        public string pApplicanttype { get; set; }
        public string pLoanpayin { get; set; }
        public decimal? pMinloanamount { get; set; }
        public decimal? pMaxloanamount { get; set; }
        public Int64? pTenurefrom { get; set; }
        public Int64? pTenureto { get; set; }
        public string pInteresttype { get; set; }
        public decimal? pRateofinterest { get; set; }
        public string ptypeofoperation { get; set; }
        public bool pIsamountrangeapplicable { get; set; }
        public bool pIstenurerangeapplicable { get; set; }
    }



    public class DocumentlistDto : CommonDTO
    {
        public Int64 pLoantypeId { set; get; }

        public Int64 pLoanId { set; get; }

        public string pContactType { set; get; }

        public Int64 pDocumentId { set; get; }

        public Int64 pDocumentgroupId { set; get; }

        public Boolean pDocumentRequired { set; get; }

        public Boolean pDocumentMandatory { set; get; }

    }
    public class ReferralCommissioLoanDTO : CommonDTO
    {
        public Int64 pLoantypeId { get; set; }
        public Int64 pLoanid { get; set; }
        public bool pIsreferralcomexist { get; set; }
        public string pCommissionpayouttype { get; set; }
        public decimal? pCommissionpayout { get; set; }

    }

    public class PenaltyConfigurationDTO : CommonDTO
    {
        public Int64 ppenaltyid { get; set; }
        public Int64 pLoantypeId { get; set; }
        public Int64 pLoanid { get; set; }
        public string ptypeofpenalinterest { get; set; }
        public bool pisloanpayinmodeapplicable { get; set; }
        public string pduepenaltytype { get; set; }
        public decimal? pduepenaltyvalue { get; set; }
        public string poverduepenaltytype { get; set; }
        public decimal? poverduepenaltyvalue { get; set; }
        public int ppenaltygraceperiod { get; set; }
        public string pPenaltygraceperiodtype { get; set; }       

    }
}
