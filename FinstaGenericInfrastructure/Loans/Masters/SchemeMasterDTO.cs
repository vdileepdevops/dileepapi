using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Masters
{
    public class SchemeMasterDTO : CommonDTO
    {

        public List<schemeConfigurationDTO> schemeConfigurationList { get; set; }
        public List<schemechargesconfigDTO> schemechargesconfigList { get; set; }
        public List<SchemeReferralCommissioLoanDTO> SchemeReferralCommissioLoanList { get; set; }

        public Int64 pLoantypeid { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoantype { get; set; }
        public string pLoanname { get; set; }
        public Int64 pSchemeid { get; set; }
        public string pSchemename { get; set; }
        public string pSchemecode { get; set; }



    }
    public class schemeConfigurationDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pSchemeid { get; set; }
        public Int64 pLoantypeid { get; set; }
        public Int64 pLoanid { get; set; }
        public Int64 pLoanconfigid { get; set; }
        public string pContacttype { get; set; }
        public string pApplicanttype { get; set; }
        public string pLoanpayin { get; set; }

        public decimal? pMinloanamount { get; set; }
        public decimal? pMaxloanamount { get; set; }
        public Int64 pTenurefrom { get; set; }
        public Int64 pTenureto { get; set; }
        public string PInteresttype { get; set; }
        public decimal? pActualrateofinterest { get; set; }
        public decimal? pSchemeinterest { get; set; }
        public string ptypeofoperation { get; set; }

        public bool pIsamountrangeapplicable { get; set; }
        public bool pIstenurerangeapplicable { get; set; }

    }
    public class schemechargesconfigDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pLoantypeid { get; set; }
        public Int64 pLoanid { get; set; }
        public Int64 pSchemeid { get; set; }
        public Int64 pLoanchargesdetailsid { get; set; }
        public Int64 pLoanchargeid { get; set; }
        public string pChargename { get; set; }
        public string pLoanpayin { get; set; }
        public decimal pminloanamountortenure { get; set; }
        public decimal pmaxloanamountortenure { get; set; }
        public string pischargeapplicableonloanrange { get; set; }
        public string pchargevaluefixedorpercentage { get; set; }
        public string papplicanttype { get; set; }
        public string pischargerangeapplicableonvalueortenure { get; set; }
        public decimal? pschemechargespercentage { get; set; }
        
        public decimal? pchargepercentage { get; set; }
        public decimal? pminchargesvalue { get; set; }
        public decimal? pmaxchargesvalue { get; set; }
        public decimal? pchargesvalue { get; set; }
        public string pchargecalonfield { get; set; }
        public string pgsttype { get; set; }
        public decimal? pgstvalue { get; set; }

        public string pIschargewaivedoff { get; set; }

        public decimal? pSchememinchargesvalue { get; set; }
        public decimal? pSchememaxchargesvalue { get; set; }
        public string ptypeofoperation { get; set; }

    }
    public class SchemeReferralCommissioLoanDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pSchemeid { get; set; }
        public Int64 pLoantypeId { get; set; }
        public Int64 pLoanid { get; set; }
        public bool pIsreferralcomexist { get; set; }
        public string pCommissionpayouttype { get; set; }
        public decimal? pCommissionpayout { get; set; }
        public decimal? pSchemecommissionpayout { get; set; }

        public string pSchemecommissionpayouttype { get; set; }
        public string ptypeofoperation { get; set; }

    }
}
