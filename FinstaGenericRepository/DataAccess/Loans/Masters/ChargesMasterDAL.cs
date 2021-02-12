using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Masters;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FinstaInfrastructure.Loans.Masters;

namespace FinstaRepository.DataAccess.Loans.Masters
{
    public class ChargesMasterDAL : SettingsDAL, IChargesMaster
    {
        ChargesMasterDTO objcharges = new ChargesMasterDTO();
        PreclouserchargesDTO objpreclousercharegs = new PreclouserchargesDTO();
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        NpgsqlTransaction trans = null;
        public List<ChargesMasterDTO> lstChargesType { get; set; }
        public List<PreclouserchargesDTO> lstPreclousercharges { get; set; }
        public List<LoanchargetypesDTO> lstLoanchargetypes { get; set; }
        public List<LoanchargetypesConfigDTO> lstLoanchargesConfig { get; set; }

        #region SaveChargeName
        public bool SaveChargesName(ChargesMasterDTO charges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into TBLMSTCHARGESTYPES(CHARGENAME,statusid,createdby,createddate)values('" + ManageQuote(charges.pChargename) + "'," + getStatusid(charges.pStatusname, ConnectionString) + "," + charges.pCreatedby + ",current_timestamp);");
                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;
        }
        #endregion

        #region GetChargesName
        public List<ChargesMasterDTO> GetChargesName(string ConnectionString, string type)
        {
            lstChargesType = new List<ChargesMasterDTO>();
            string strQuery = string.Empty;
            try
            {
                if (type == "All")
                {
                    strQuery = "SELECT * FROM TBLMSTCHARGESTYPES";
                }
                else
                {
                    strQuery = "SELECT * FROM TBLMSTCHARGESTYPES WHERE STATUSID = 1";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        ChargesMasterDTO objcharges = new ChargesMasterDTO();
                        objcharges.pChargename = dr["CHARGENAME"].ToString();
                        objcharges.pChargeid = Convert.ToInt64(dr["chargeid"]);
                        lstChargesType.Add(objcharges);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lstChargesType;

        }
        #endregion

        #region UpdateChargesName
        public bool UpdateChargesName(ChargesMasterDTO Charges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                string loanchargename = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select chargename from tblmstloanwisechargestypes where chargeid=" + Charges.pChargeid + ";"));


                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update tblmstloanwisechargestypes set chargename='" + ManageQuote(Charges.pChargename) + "' where chargeid=" + Charges.pChargeid + ";Update TBLMSTCHARGESTYPES Set CHARGENAME='" + ManageQuote(Charges.pChargename) + "' where CHARGEID=" + Charges.pChargeid + ";update tblmstloanwisechargesconfig set chargename='" + ManageQuote(Charges.pChargename) + "' where chargename='" + loanchargename + "';");
                isSaved = true;
            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;

        }
        #endregion

        #region DeleteChargesName
        public bool DeleteChargesName(ChargesMasterDTO charges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update TBLMSTCHARGESTYPES set statusid=" + getStatusid(charges.pStatusname, ConnectionString) + ",modifiedby=" + charges.pCreatedby + ",modifieddate=current_timestamp where chargeid=" + charges.pChargeid + ";");
                isSaved = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isSaved;
        }
        #endregion
        //Loan Wise Charges Assgining
        #region SaveLoanWiseChargesName
        public bool SaveLoanWiseChargesName(ChargesMasterDTO LoanWiseCharges, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sb = new StringBuilder();
            con = new NpgsqlConnection(ConnectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                for (var i = 0; i < LoanWiseCharges.pLoanchargetypes.Count; i++)
                {
                    sb.Append("insert into TBLMSTLOANWISECHARGESTYPES(LOANTYPEID,LOANID,CHARGEID,CHARGENAME,LEDGERNAME,PARENTGROUPLEDGERNAME,PARENTGROUPLEDGERID,STATUSID,CREATEDBY,CREATEDDATE) values(" + LoanWiseCharges.pLoanchargetypes[i].pLoantypeid + "," + LoanWiseCharges.pLoanchargetypes[i].pLoanid + "," + LoanWiseCharges.pLoanchargetypes[i].pChargeid + ",'" + ManageQuote(LoanWiseCharges.pLoanchargetypes[i].pChargename) + "','" + ManageQuote(LoanWiseCharges.pLoanchargetypes[i].pLedgername) + "','" + ManageQuote(LoanWiseCharges.pLoanchargetypes[i].pParentgroupledgername) + "','" + ManageQuote(LoanWiseCharges.pLoanchargetypes[i].pParentgroupledgerid) + "'," + getStatusid(LoanWiseCharges.pStatusname, ConnectionString) + ", " + LoanWiseCharges.pCreatedby + ", current_timestamp)");
                }
                if (Convert.ToString(sb) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
                }
                trans.Commit();

                isSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();

                }
            }
            return isSaved;
        }
        #endregion

        #region GetLoanWiseChargesName
        public List<LoanchargetypesDTO> GetLoanWiseChargesName(string ConnectionString, string loanid)
        {
            lstLoanchargetypes = new List<LoanchargetypesDTO>();
            string Query = string.Empty;

            try
            {
                if (loanid == "ALL")
                {
                    Query = "SELECT TBL.LOANCHARGEID, TBL.LOANTYPEID, TLP.LOANTYPE, TBL.LOANID, TML.LOANNAME, TBL.CHARGEID, TBL.CHARGENAME, TBL.LEDGERNAME, TBL.PARENTGROUPLEDGERNAME, TBL.PARENTGROUPLEDGERID, TBL.STATUSID,statusname FROM TBLMSTLOANWISECHARGESTYPES TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID join TBLMSTSTATUS ts on ts.statusid = tbl.statusid and upper(statusname)='ACTIVE'   ORDER BY coalesce( TBL.modifieddate  , TBL.createddate)  desc ,TBL.LOANCHARGEID";
                }
                else
                {
                    Query = "SELECT TBL.LOANCHARGEID, TBL.LOANTYPEID, TLP.LOANTYPE, TBL.LOANID, TML.LOANNAME, TBL.CHARGEID, TBL.CHARGENAME, TBL.LEDGERNAME, TBL.PARENTGROUPLEDGERNAME, TBL.PARENTGROUPLEDGERID, TBL.STATUSID,statusname FROM TBLMSTLOANWISECHARGESTYPES TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID join TBLMSTSTATUS ts on ts.statusid = tbl.statusid where TBL.loanid=" + Convert.ToInt64(loanid) + " and upper(statusname)='ACTIVE' ORDER BY  coalesce( TBL.modifieddate  , TBL.createddate)  desc ,TBL.LOANCHARGEID";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        LoanchargetypesDTO objloanchargetypes = new LoanchargetypesDTO();
                        objloanchargetypes.pLoantypeid = Convert.ToInt64(dr["Loantypeid"]);
                        objloanchargetypes.pLoantype = dr["LOANTYPE"].ToString();
                        objloanchargetypes.pLoanid = Convert.ToInt64(dr["Loanid"]);
                        objloanchargetypes.pLoanNmae = dr["LOANNAME"].ToString();
                        objloanchargetypes.pChargeid = Convert.ToInt64(dr["Chargeid"]);
                        objloanchargetypes.pLoanChargeid = Convert.ToInt64(dr["LoanChargeid"]);
                        objloanchargetypes.pChargename = dr["Chargename"].ToString();
                        objloanchargetypes.pLedgername = dr["Ledgername"].ToString();
                        objloanchargetypes.pParentgroupledgername = dr["Parentgroupledgername"].ToString();
                        objloanchargetypes.pParentgroupledgerid = dr["Parentgroupledgerid"].ToString();
                        objloanchargetypes.pStatusname = dr["statusname"].ToString();
                        lstLoanchargetypes.Add(objloanchargetypes);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstLoanchargetypes;

        }
        #endregion

        //Charges Configaration

        #region SaveLoanWiseChargesConfig
        public bool SaveLoanWiseChargeConfig(ChargesMasterDTO charges, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sb = new StringBuilder();
            con = new NpgsqlConnection(ConnectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                if (charges.pLoanchargetypesConfig != null)
                {
                    for (var i = 0; i < charges.pLoanchargetypesConfig.Count; i++)
                    {

                        // FormatDate(charges.pLoanchargetypesConfig[i].pChargeEffectFrom) + "','" + FormatDate(charges.pLoanchargetypesConfig[i].pChargeEffectTo)
                        if (string.IsNullOrEmpty(charges.pLoanchargetypesConfig[i].pChargeEffectFrom))
                        {

                            charges.pLoanchargetypesConfig[i].pChargeEffectFrom = "null";
                        }
                        else
                        {

                            charges.pLoanchargetypesConfig[i].pChargeEffectFrom = "'" + FormatDate(charges.pLoanchargetypesConfig[i].pChargeEffectFrom) + "'";
                        }

                        if (string.IsNullOrEmpty(charges.pLoanchargetypesConfig[i].pChargeEffectTo))
                        {

                            charges.pLoanchargetypesConfig[i].pChargeEffectTo = "null";
                        }
                        else
                        {
                            charges.pLoanchargetypesConfig[i].pChargeEffectTo = "'" + FormatDate(charges.pLoanchargetypesConfig[i].pChargeEffectTo) + "'";
                        }
                        sb.Append("insert into tblmstloanwisechargesconfig(loantypeid,loanid,loanchargeid,chargename,loanpayin,ischargeapplicableonloanrange,chargevaluefixedorpercentage,applicanttype,ischargerangeapplicableonvalueortenure,minloanamountortenure,maxloanamountortenure,chargepercentage,minchargesvalue,maxchargesvalue,chargecalonfield,gsttype,gstvalue,effectfromdate,effecttodate,ischargewaivedoff,lockingperiod,statusid,createdby,createddate)values(" + charges.pLoanchargetypesConfig[i].pLoantypeid + "," + charges.pLoanchargetypesConfig[i].pLoanid + "," + charges.pLoanchargetypesConfig[i].pLoanChargeid + ",'" + ManageQuote(charges.pLoanchargetypesConfig[i].pChargename) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pLoanpayin) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pIsChargedependentOnLoanRange) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pChargevaluefixedorpercentage) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pApplicanttype) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pIschargerangeapplicableonvalueortenure) + "'," + charges.pLoanchargetypesConfig[i].pMinLoanAmountorTenure + "," + charges.pLoanchargetypesConfig[i].pMaxLoanAmountorTenure + "," + charges.pLoanchargetypesConfig[i].pChargePercentage + "," + charges.pLoanchargetypesConfig[i].pMinchargesvalue + "," + charges.pLoanchargetypesConfig[i].pMaxchargesvalue + ",'" + ManageQuote(charges.pLoanchargetypesConfig[i].pChargecalonfield) + "','" + ManageQuote(charges.pLoanchargetypesConfig[i].pGsttype) + "'," + charges.pLoanchargetypesConfig[i].pGstvalue + "," + charges.pLoanchargetypesConfig[i].pChargeEffectFrom + "," + charges.pLoanchargetypesConfig[i].pChargeEffectTo + ",'" + ManageQuote(charges.pLoanchargetypesConfig[i].pIschargewaivedoff) + "'," + charges.pLoanchargetypesConfig[i].pLockingperiod + ",1," + charges.pCreatedby + ",current_timestamp);");
                    }
                }
                //if (Convert.ToString(sb) != string.Empty)
                //{
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
                // }
                trans.Commit();
                isSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();

                }
            }
            return isSaved;
        }
        #endregion

        #region GetLoanWiseApplicantTypes
        public List<LoanchargetypesConfigDTO> GetLoanWiseApplicantTypes(string ConnectionString, Int64 loanid)
        {
            lstLoanchargesConfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct applicanttype from tblmstloanconfiguration where Loanid=" + loanid + "");
                while (dr.Read())
                {
                    LoanchargetypesConfigDTO objloanchargesConfig = new LoanchargetypesConfigDTO();
                    objloanchargesConfig.pApplicanttype = dr["applicanttype"].ToString();
                    lstLoanchargesConfig.Add(objloanchargesConfig);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanchargesConfig;

        }
        #endregion

        #region GetLoanWiseChargeConfig
        public List<LoanchargetypesConfigDTO> GetLoanWiseChargeConfig(string ConnectionString, Int64 loanChargeid)
        {
            lstLoanchargesConfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT  TBL.RECORDID,TBL.LOANTYPEID,TLP.LOANTYPE,TBL.LOANID,TML.LOANNAME,TBL.LOANCHARGEID,TBL.CHARGENAME,TBL.LOANPAYIN, case when TBL.ISCHARGEAPPLICABLEONLOANRANGE ='Y' then 'YES' else 'NO' end as ISCHARGEAPPLICABLEONLOANRANGE,TBL.CHARGEVALUEFIXEDORPERCENTAGE,TBL.APPLICANTTYPE,TBL.ISCHARGERANGEAPPLICABLEONVALUEORTENURE,TBL.MINLOANAMOUNTORTENURE,TBL.MAXLOANAMOUNTORTENURE,TBL.CHARGEPERCENTAGE,TBL.MINCHARGESVALUE,TBL.MAXCHARGESVALUE,TBL.CHARGECALONFIELD,TBL.GSTTYPE,TBL.GSTVALUE,to_char(TBL.EFFECTFROMDATE,'dd/MM/yyyy') EFFECTFROMDATE, to_char(TBL.EFFECTTODATE,'dd/MM/yyyy') EFFECTTODATE,TBL.ISCHARGEWAIVEDOFF,TBL.LOCKINGPERIOD FROM TBLMSTLOANWISECHARGESCONFIG TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID join TBLMSTSTATUS ts on ts.statusid = tbl.statusid  where TBL.LOANCHARGEID=" + loanChargeid + " and statusname='Active'  order by CHARGENAME");
                while (dr.Read())
                {
                    LoanchargetypesConfigDTO objloanchargesConfig = new LoanchargetypesConfigDTO();
                    objloanchargesConfig.precordid = Convert.ToInt64(dr["RECORDID"]);
                    objloanchargesConfig.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                    objloanchargesConfig.pLoantype = dr["LOANTYPE"].ToString();
                    objloanchargesConfig.pLoanid = Convert.ToInt64(dr["Loanid"]);
                    objloanchargesConfig.pLoanNmae = dr["LOANNAME"].ToString();
                    objloanchargesConfig.pLoanChargeid = Convert.ToInt64(dr["loanchargeid"]);

                    objloanchargesConfig.pChargename = Convert.ToString(dr["CHARGENAME"]);
                    objloanchargesConfig.pLoanpayin = dr["loanpayin"].ToString();
                    objloanchargesConfig.pIsChargedependentOnLoanRange = dr["ischargeapplicableonloanrange"].ToString();
                    objloanchargesConfig.pChargevaluefixedorpercentage = dr["chargevaluefixedorpercentage"].ToString();
                    objloanchargesConfig.pApplicanttype = dr["applicanttype"].ToString();
                    objloanchargesConfig.pIschargerangeapplicableonvalueortenure = dr["ischargerangeapplicableonvalueortenure"].ToString();
                    objloanchargesConfig.pMinLoanAmountorTenure = Convert.ToInt64(dr["minloanamountortenure"]);
                    objloanchargesConfig.pMaxLoanAmountorTenure = Convert.ToInt64(dr["maxloanamountortenure"]);
                    objloanchargesConfig.pChargePercentage = Convert.ToDecimal(dr["chargepercentage"]);
                    objloanchargesConfig.pMinchargesvalue = Convert.ToInt64(dr["minchargesvalue"]);
                    objloanchargesConfig.pMaxchargesvalue = Convert.ToInt64(dr["maxchargesvalue"]);
                    objloanchargesConfig.pChargecalonfield = dr["chargecalonfield"].ToString();
                    objloanchargesConfig.pGsttype = dr["gsttype"].ToString();
                    objloanchargesConfig.pGstvalue = Convert.ToInt64(dr["gstvalue"]);
                    objloanchargesConfig.pChargeEffectFrom = dr["effectfromdate"].ToString();

                    objloanchargesConfig.pChargeEffectTo = Convert.ToString(dr["effecttodate"]);
                    if (string.IsNullOrEmpty(objloanchargesConfig.pChargeEffectTo))
                    {
                        objloanchargesConfig.peditstatus = "YES";
                    }
                    else
                    {
                        objloanchargesConfig.peditstatus = "NO";
                    }
                    objloanchargesConfig.pIschargewaivedoff = dr["ischargewaivedoff"].ToString();
                    objloanchargesConfig.pLockingperiod = Convert.ToInt64(dr["lockingperiod"]);
                    objloanchargesConfig.ptypeofoperation = "OLD";
                    lstLoanchargesConfig.Add(objloanchargesConfig);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstLoanchargesConfig;
        }
        #endregion

        #region ViewLoanWiseChargeConfig
        public List<LoanchargetypesConfigDTO> ViewLoanWiseChargeConfig(string ConnectionString)
        {
            lstLoanchargesConfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT  TBL.LOANTYPEID,TLP.LOANTYPE,TBL.LOANID,TML.LOANNAME,TBL.LOANCHARGEID,TBL.CHARGENAME,TBL.LOANPAYIN,TBL.ISCHARGEAPPLICABLEONLOANRANGE,TBL.CHARGEVALUEFIXEDORPERCENTAGE,TBL.APPLICANTTYPE,TBL.ISCHARGERANGEAPPLICABLEONVALUEORTENURE,TBL.MINLOANAMOUNTORTENURE,TBL.MAXLOANAMOUNTORTENURE,TBL.CHARGEPERCENTAGE,TBL.MINCHARGESVALUE,TBL.MAXCHARGESVALUE,TBL.CHARGECALONFIELD,TBL.GSTTYPE,TBL.GSTVALUE,TBL.EFFECTFROMDATE,TBL.EFFECTTODATE,TBL.ISCHARGEWAIVEDOFF,TBL.LOCKINGPERIOD FROM TBLMSTLOANWISECHARGESCONFIG TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID order by CHARGENAME");
                while (dr.Read())
                {
                    LoanchargetypesConfigDTO objloanchargesConfig = new LoanchargetypesConfigDTO();
                    objloanchargesConfig.pLoantypeid = Convert.ToInt16(dr["loantypeid"]);
                    objloanchargesConfig.pLoantype = dr["LOANTYPE"].ToString();
                    objloanchargesConfig.pLoanid = Convert.ToInt16(dr["Loanid"]);
                    objloanchargesConfig.pLoanNmae = dr["LOANNAME"].ToString();
                    objloanchargesConfig.pLoanChargeid = Convert.ToInt16(dr["loanchargeid"]);
                    objloanchargesConfig.pLoanpayin = dr["loanpayin"].ToString();
                    objloanchargesConfig.pIsChargedependentOnLoanRange = dr["ischargeapplicableonloanrange"].ToString();
                    objloanchargesConfig.pChargevaluefixedorpercentage = dr["chargevaluefixedorpercentage"].ToString();
                    objloanchargesConfig.pApplicanttype = dr["applicanttype"].ToString();
                    objloanchargesConfig.pIschargerangeapplicableonvalueortenure = dr["ischargerangeapplicableonvalueortenure"].ToString();
                    objloanchargesConfig.pMinLoanAmountorTenure = Convert.ToInt16(dr["minloanamountortenure"]);
                    objloanchargesConfig.pMaxLoanAmountorTenure = Convert.ToInt16(dr["maxloanamountortenure"]);
                    objloanchargesConfig.pChargePercentage = Convert.ToInt16(dr["chargepercentage"]);
                    objloanchargesConfig.pMinchargesvalue = Convert.ToInt16(dr["minchargesvalue"]);
                    objloanchargesConfig.pMaxchargesvalue = Convert.ToInt16(dr["maxchargesvalue"]);
                    objloanchargesConfig.pChargecalonfield = dr["chargecalonfield"].ToString();
                    objloanchargesConfig.pGsttype = dr["gsttype"].ToString();
                    objloanchargesConfig.pGstvalue = Convert.ToInt16(dr["gstvalue"]);
                    objloanchargesConfig.pChargeEffectFrom = dr["effectfromdate"].ToString();
                    objloanchargesConfig.pChargeEffectTo = dr["effecttodate"].ToString();
                    objloanchargesConfig.pIschargewaivedoff = dr["ischargewaivedoff"].ToString();
                    objloanchargesConfig.pLockingperiod = Convert.ToInt16(dr["lockingperiod"]);
                    lstLoanchargesConfig.Add(objloanchargesConfig);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstLoanchargesConfig;
        }
        #endregion

        #region UpdateLoanWiseChargeConfig
        public bool UpdateLoanWiseChargeConfig(ChargesMasterDTO Charges, string ConnectionString)
        {
            bool isSaved = false;
            StringBuilder sb = new StringBuilder();
            con = new NpgsqlConnection(ConnectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                if (Charges.pLoanchargetypesConfig != null)
                {
                    for (var i = 0; i < Charges.pLoanchargetypesConfig.Count; i++)
                    {
                        if (string.IsNullOrEmpty(Charges.pLoanchargetypesConfig[i].pChargeEffectFrom))
                        {

                            Charges.pLoanchargetypesConfig[i].pChargeEffectFrom = "null";
                        }
                        else
                        {

                            Charges.pLoanchargetypesConfig[i].pChargeEffectFrom = "'" + FormatDate(Charges.pLoanchargetypesConfig[i].pChargeEffectFrom) + "'";
                        }

                        if (string.IsNullOrEmpty(Charges.pLoanchargetypesConfig[i].pChargeEffectTo))
                        {

                            Charges.pLoanchargetypesConfig[i].pChargeEffectTo = "null";
                        }
                        else
                        {
                            Charges.pLoanchargetypesConfig[i].pChargeEffectTo = "'" + FormatDate(Charges.pLoanchargetypesConfig[i].pChargeEffectTo) + "'";
                        }

                        if (Charges.pLoanchargetypesConfig[i].ptypeofoperation.ToUpper() == "UPDATE")
                        {
                            sb.Append("Update TBLMSTLOANWISECHARGESCONFIG Set loanpayin='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pLoanpayin) + "',ischargeapplicableonloanrange='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIsChargedependentOnLoanRange) + "',chargevaluefixedorpercentage='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pChargevaluefixedorpercentage) + "',applicanttype='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pApplicanttype) + "',ischargerangeapplicableonvalueortenure='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIschargerangeapplicableonvalueortenure) + "',minloanamountortenure=" + Charges.pLoanchargetypesConfig[i].pMinLoanAmountorTenure + ",maxloanamountortenure=  " + Charges.pLoanchargetypesConfig[i].pMaxLoanAmountorTenure + ",chargepercentage=  " + Charges.pLoanchargetypesConfig[i].pChargePercentage + ",minchargesvalue=" + Charges.pLoanchargetypesConfig[i].pMinchargesvalue + ",maxchargesvalue=" + Charges.pLoanchargetypesConfig[i].pMaxchargesvalue + ",chargecalonfield='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pChargecalonfield) + "',gsttype='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pGsttype) + "',gstvalue=" + Charges.pLoanchargetypesConfig[i].pGstvalue + ",effectfromdate= " + (Charges.pLoanchargetypesConfig[i].pChargeEffectFrom) + ",effecttodate=" + (Charges.pLoanchargetypesConfig[i].pChargeEffectTo) + ",ischargewaivedoff='" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIschargewaivedoff) + "',lockingperiod=" + Charges.pLoanchargetypesConfig[i].pLockingperiod + "  where loanid=" + Charges.pLoanchargetypesConfig[i].pLoanid + " and RECORDID=" + Charges.pLoanchargetypesConfig[i].precordid + ";");
                        }
                        else if (Charges.pLoanchargetypesConfig[i].ptypeofoperation == "CREATE")
                        {

                            sb.Append("insert into tblmstloanwisechargesconfig(loantypeid,loanid,loanchargeid,chargename,loanpayin,ischargeapplicableonloanrange,chargevaluefixedorpercentage,applicanttype,ischargerangeapplicableonvalueortenure,minloanamountortenure,maxloanamountortenure,chargepercentage,minchargesvalue,maxchargesvalue,chargecalonfield,gsttype,gstvalue,effectfromdate,effecttodate,ischargewaivedoff,lockingperiod,statusid,createdby,createddate)values(" + Charges.pLoanchargetypesConfig[i].pLoantypeid + "," + Charges.pLoanchargetypesConfig[i].pLoanid + "," + Charges.pLoanchargetypesConfig[i].pLoanChargeid + ",'" + ManageQuote(Charges.pLoanchargetypesConfig[i].pChargename) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pLoanpayin) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIsChargedependentOnLoanRange) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pChargevaluefixedorpercentage) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pApplicanttype) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIschargerangeapplicableonvalueortenure) + "'," + Charges.pLoanchargetypesConfig[i].pMinLoanAmountorTenure + "," + Charges.pLoanchargetypesConfig[i].pMaxLoanAmountorTenure + "," + Charges.pLoanchargetypesConfig[i].pChargePercentage + "," + Charges.pLoanchargetypesConfig[i].pMinchargesvalue + "," + Charges.pLoanchargetypesConfig[i].pMaxchargesvalue + ",'" + ManageQuote(Charges.pLoanchargetypesConfig[i].pChargecalonfield) + "','" + ManageQuote(Charges.pLoanchargetypesConfig[i].pGsttype) + "'," + Charges.pLoanchargetypesConfig[i].pGstvalue + "," + Charges.pLoanchargetypesConfig[i].pChargeEffectFrom + "," + Charges.pLoanchargetypesConfig[i].pChargeEffectTo + ",'" + ManageQuote(Charges.pLoanchargetypesConfig[i].pIschargewaivedoff) + "'," + Charges.pLoanchargetypesConfig[i].pLockingperiod + ",1," + Charges.pCreatedby + ",current_timestamp);");
                        }
                    }

                }
                if (!string.IsNullOrEmpty(sb.ToString()))
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sb.ToString());
                trans.Commit();
                isSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();

                }
            }
            return isSaved;
        }
        #endregion

        #region DeleteLoanWiseChargeConfig
        public bool DeleteLoanWiseChargeConfig(LoanchargetypesConfigDTO Charges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                string query = "";


                if (Charges.precordid == 0)
                {
                    if (Charges.pStatusname.ToUpper() == "IN-ACTIVE")
                        query = "Update tblmstloanwisechargestypes Set statusid=" + getStatusid(Charges.pStatusname, ConnectionString) + ",modifiedby=" + Charges.pCreatedby + ",modifieddate=current_timestamp where loanid=" + Charges.pLoanid + " and loanchargeid=" + Charges.pLoanChargeid + "; Update TBLMSTLOANWISECHARGESCONFIG Set statusid=" + getStatusid(Charges.pStatusname, ConnectionString) + ",modifiedby=" + Charges.pCreatedby + ",modifieddate=current_timestamp where loanid=" + Charges.pLoanid + " and loanchargeid=" + Charges.pLoanChargeid + ";";
                    else
                        query = "Update tblmstloanwisechargestypes Set statusid=" + getStatusid(Charges.pStatusname, ConnectionString) + ",modifiedby=" + Charges.pCreatedby + ",modifieddate=current_timestamp where loanid=" + Charges.pLoanid + " and loanchargeid=" + Charges.pLoanChargeid + "; ";
                }
                else
                    query = "Update TBLMSTLOANWISECHARGESCONFIG Set effecttodate  =current_date, statusid = " + getStatusid(Charges.pStatusname, ConnectionString) + ",modifiedby = " + Charges.pCreatedby + ",modifieddate = current_timestamp where loanid = " + Charges.pLoanid + " and loanchargeid = " + Charges.pLoanChargeid + " and recordid = " + Charges.precordid + "; ";
                if (query != "")
                {
                    NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, query);
                    isSaved = true;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isSaved;

        }

        #endregion

        //PreClouser Charges
        #region CheckDuplicateLoanid
        public int CheckDuplicateLoanid(string ConnectionString, long Loanid)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstloanpreclousercharges where loanid=" + Loanid + " and STATUSID=1;"));
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }
        #endregion

        #region SavePreclouserCharges
        public bool SavePreclouserCharges(PreclouserchargesDTO PreclouserCharges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into TBLMSTLOANPRECLOUSERCHARGES(LOANTYPEID,LOANID,ISCHARGEAPPLICABLE,CHARGECALTYPE,CHARGECALONFIELD,CHARGESVALUE,ISTAXAPPLICABLE,TAXTYPE,TAXPERCENTAGE,LOCKINGPERIOD,LOCKINGPERIODTYPE,STATUSID,CREATEDBY,CREATEDDATE)values(" + PreclouserCharges.pLoantypeid + "," + PreclouserCharges.pLoanid + ",'" + ManageQuote(PreclouserCharges.pIschargeapplicable) + "','" + ManageQuote(PreclouserCharges.pChargecaltype) + "','" + ManageQuote(PreclouserCharges.pChargecalonfield) + "'," + PreclouserCharges.pChargesvalue + ",'" + ManageQuote(PreclouserCharges.pIstaxapplicable) + "','" + ManageQuote(PreclouserCharges.pTaxtype) + "'," + PreclouserCharges.pTaxpercentage + "," + PreclouserCharges.pLockingperiod + ",'" + ManageQuote(PreclouserCharges.pLockingperiodtype) + "',1,"+PreclouserCharges.pCreatedby+",current_timestamp);");
                isSaved = true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isSaved;

        }
        #endregion

        #region UpdatePreclouserCharges
        public bool UpdatePreclouserCharges(PreclouserchargesDTO PreclouserCharges, string ConnectionString)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Update TBLMSTLOANPRECLOUSERCHARGES Set ISCHARGEAPPLICABLE='" + ManageQuote(PreclouserCharges.pIschargeapplicable) + "',CHARGECALTYPE='" + ManageQuote(PreclouserCharges.pChargecaltype) + "',CHARGECALONFIELD='" + ManageQuote(PreclouserCharges.pChargecalonfield) + "',CHARGESVALUE=" + PreclouserCharges.pChargesvalue + ",ISTAXAPPLICABLE='" + ManageQuote(PreclouserCharges.pIstaxapplicable) + "', TAXTYPE='" + ManageQuote(PreclouserCharges.pTaxtype) + "',TAXPERCENTAGE=" + PreclouserCharges.pTaxpercentage + ",LOCKINGPERIOD=" + PreclouserCharges.pLockingperiod + ",LOCKINGPERIODTYPE='" + ManageQuote(PreclouserCharges.pLockingperiodtype) + "',MODIFIEDBY=" + PreclouserCharges.pCreatedby + ",MODIFIEDDATE=current_timestamp where RECORDID=" + PreclouserCharges.pRecordid + " ;");
                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;

        }
        #endregion

        #region DeletePreclouserCharges
        public bool DeletePreclouserCharges(string ConnectionString, long Loanid, long Recordid,long userid)
        {
            bool isSaved = false;
            try
            {
                NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Update TBLMSTLOANPRECLOUSERCHARGES Set STATUSID=2,MODIFIEDBY="+ userid +",MODIFIEDDATE=current_timestamp where loanid=" + Loanid + " and recordid=" + Recordid + " ;");
                isSaved = true;

            }
            catch (Exception)
            {

                throw;
            }
            return isSaved;

        }
        #endregion

        #region GetePreclouserCharges
        public List<PreclouserchargesDTO> GetePreclouserCharges(string ConnectionString, long Loanid, long Recordid)
        {
            lstPreclousercharges = new List<PreclouserchargesDTO>();
            try
            {
                dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT TBL.RECORDID,TBL.LOANTYPEID,TLP.LOANTYPE,TBL.LOANID,TML.LOANNAME,TBL.ISCHARGEAPPLICABLE,TBL.CHARGECALTYPE,TBL.CHARGECALONFIELD,TBL.CHARGESVALUE,TBL.ISTAXAPPLICABLE,TBL.TAXTYPE,TBL.TAXPERCENTAGE,TBL.LOCKINGPERIOD,TBL.LOCKINGPERIODTYPE,STATUSNAME FROM TBLMSTLOANPRECLOUSERCHARGES TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID  JOIN TBLMSTSTATUS TS ON TBL.STATUSID = TS.STATUSID  where tbl.loanid=" + Loanid + " and  TBL.RECORDID=" + Recordid + "");
                while (dr.Read())
                {
                    objpreclousercharegs.pRecordid = Convert.ToInt64(dr["RECORDID"]);
                    objpreclousercharegs.pLoantypeid = Convert.ToInt64(dr["LOANTYPEID"]);
                    objpreclousercharegs.pLoantype = dr["LOANTYPE"].ToString();
                    objpreclousercharegs.pLoanid = Convert.ToInt64(dr["Loanid"]);
                    objpreclousercharegs.pLoanname = dr["LOANNAME"].ToString();
                    objpreclousercharegs.pIschargeapplicable = dr["Ischargeapplicable"].ToString();
                    objpreclousercharegs.pChargecaltype = dr["Chargecaltype"].ToString();
                    objpreclousercharegs.pChargecalonfield = dr["Chargecalonfield"].ToString();
                    objpreclousercharegs.pChargesvalue = Convert.ToDecimal(dr["Chargesvalue"]);
                    objpreclousercharegs.pIstaxapplicable = dr["Istaxapplicable"].ToString();
                    objpreclousercharegs.pTaxtype = dr["Taxtype"].ToString();
                    objpreclousercharegs.pTaxpercentage = Convert.ToDecimal(dr["Taxpercentage"]);
                    objpreclousercharegs.pLockingperiod = Convert.ToInt64(dr["Lockingperiod"]);
                    objpreclousercharegs.pLockingperiodtype = dr["Lockingperiodtype"].ToString();
                    objpreclousercharegs.pStatusname = dr["STATUSNAME"].ToString();
                    lstPreclousercharges.Add(objpreclousercharegs);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPreclousercharges;
        }
        #endregion

        #region ViewPreclouserCharges
        public List<PreclouserchargesDTO> ViewPreclouserCharges(string ConnectionString)
        {
            lstPreclousercharges = new List<PreclouserchargesDTO>();
            try
            {

                dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT TBL.RECORDID,TBL.LOANTYPEID,TLP.LOANTYPE,TBL.LOANID,TML.LOANNAME,TBL.ISCHARGEAPPLICABLE,TBL.CHARGECALTYPE,TBL.CHARGECALONFIELD,TBL.CHARGESVALUE,TBL.ISTAXAPPLICABLE,TBL.TAXTYPE,TBL.TAXPERCENTAGE,TBL.LOCKINGPERIOD,TBL.LOCKINGPERIODTYPE,STATUSNAME FROM TBLMSTLOANPRECLOUSERCHARGES TBL JOIN TBLMSTLOANTYPES TLP ON TBL.LOANTYPEID = TLP.LOANTYPEID JOIN TBLMSTLOANS TML ON TBL.LOANID = TML.LOANID  JOIN TBLMSTSTATUS TS ON TBL.STATUSID = TS.STATUSID where TBL.STATUSID=1 ORDER BY TBL.RECORDID");


                while (dr.Read())
                {
                    objpreclousercharegs = new PreclouserchargesDTO();
                    objpreclousercharegs.pRecordid = Convert.ToInt64(dr["RECORDID"]);
                    objpreclousercharegs.pLoantypeid = Convert.ToInt64(dr["LOANTYPEID"]);
                    objpreclousercharegs.pLoantype = dr["LOANTYPE"].ToString();
                    objpreclousercharegs.pLoanid = Convert.ToInt64(dr["Loanid"]);
                    objpreclousercharegs.pLoanname = dr["LOANNAME"].ToString();
                    objpreclousercharegs.pIschargeapplicable = dr["Ischargeapplicable"].ToString();
                    objpreclousercharegs.pChargecaltype = dr["Chargecaltype"].ToString();
                    objpreclousercharegs.pChargecalonfield = dr["Chargecalonfield"].ToString();
                    objpreclousercharegs.pChargesvalue = Convert.ToInt64(dr["Chargesvalue"]);
                    objpreclousercharegs.pIstaxapplicable = dr["Istaxapplicable"].ToString();
                    objpreclousercharegs.pTaxtype = dr["Taxtype"].ToString();
                    objpreclousercharegs.pTaxpercentage = Convert.ToInt64(dr["Taxpercentage"]);
                    objpreclousercharegs.pLockingperiod = Convert.ToInt64(dr["Lockingperiod"]);
                    objpreclousercharegs.pLockingperiodtype = dr["Lockingperiodtype"].ToString();
                    objpreclousercharegs.pStatusname = dr["STATUSNAME"].ToString();
                    lstPreclousercharges.Add(objpreclousercharegs);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstPreclousercharges;
        }
        #endregion

        #region GetLoanWiseLoanPayin
        public List<LoanchargetypesConfigDTO> GetLoanWiseLoanPayin(string ConnectionString, long loanid,string applicanttype)
        {
            lstLoanchargesConfig = new List<LoanchargetypesConfigDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select distinct loanpayin from tblmstloanconfiguration where Loanid=" + loanid + " and applicanttype='"+ applicanttype + "'"))
                {
                    while (dr.Read())
                    {
                        LoanchargetypesConfigDTO objloanchargesConfig = new LoanchargetypesConfigDTO();
                        objloanchargesConfig.pLoanpayin = dr["loanpayin"].ToString();
                        lstLoanchargesConfig.Add(objloanchargesConfig);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return lstLoanchargesConfig;
        }
        #endregion

        #region CheckChargeNames
        public int CheckDuplicateChargeNames(string ChargeName, Int64 chargeid, string ConnectionString)
        {
            int count = 0;
            try
            {
                if (string.IsNullOrEmpty(ChargeName))
                {
                    ChargeName = "";
                }
                else
                {
                    ChargeName = ChargeName.ToUpper();
                }
                if (!string.IsNullOrEmpty(ChargeName) && !string.IsNullOrEmpty(chargeid.ToString()) && chargeid != 0)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from TBLMSTCHARGESTYPES where upper(chargename)='" + ChargeName + "' and chargeid!=" + chargeid + " AND STATUSID=1;"));
                }
                else
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from TBLMSTCHARGESTYPES where upper(chargename)='" + ChargeName + "' AND STATUSID=1;"));
                }
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public int CheckDuplicateChargeNamesByLoanid(string ChargeName, long loanid, string ConnectionString)
        {
            int count = 0;
            try
            {
                if (string.IsNullOrEmpty(ChargeName))
                {
                    ChargeName = "";
                }
                else
                {
                    ChargeName = ChargeName.ToUpper();
                }
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from TBLMSTLOANWISECHARGESTYPES where (chargeid)='" + ChargeName + "' and loanid =" + loanid + " and statusid=" + getStatusid("ACTIVE", ConnectionString) + ";"));

            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }
        #endregion


        #region Referral


        #endregion



    }
}
