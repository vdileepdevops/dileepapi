using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{

    public class BondspreviewMain
    {
        public List<BondsPreviewDTO> _BondsPreviewDTOLst { get; set; }
    }


    public class BondsPreviewDTO
    {

        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pInterestpayable { get; set; }
        public object pDepositdate { get; set; }
        public object pInterestpayout { get; set; }
        public object pFdname { get; set; }
        public object pMaturitydate { get; set; }
        public object pFathername { get; set; }
        public object pAddress { get; set; }
        public object pTenor { get; set; }
        public object pTenortype { get; set; }
        public object pSquareyard { get; set; }
        public List<NomieeDetailsDTO> plstNomieeDetails { get; set; }
        public List<LienDetailsDTO> plstLienDetails { get; set; }

    }
    public class NomieeDetailsDTO
    {
        public object pNomieename { get; set; }
        public object pReletaion { get; set; }
        public object pProportion { get; set; }

    }
    public class LienDetailsDTO
    {
        public object pLiento { get; set; }
        public object pCompanybranch { get; set; }
        public object pCompanyname { get; set; }


    }
    public class GetPreviewDetailsDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pInterestpayable { get; set; }
        public object pDepositdate { get; set; }
        public object pInterestpayout { get; set; }
        public object pFdname { get; set; }
        public object pMaturitydate { get; set; }
        public object pTenor { get; set; }
        public object pTenortype { get; set; }
        public object psquareyard { get; set; }



    }
    public class Bonds_PrintDTO

    {
        public List<Bonds_Print> lstBonds_Print { get; set; }

    }
    public class Bonds_Print
    {
        public long pDeposit_id { get; set; }
        public string pPrint_Date { get; set; }
        public string pPrint_Status { get; set; }

    }
}
