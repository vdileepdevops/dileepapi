using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Masters;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FinstaRepository.DataAccess.Loans.Masters
{
    public class SchemeMasterDAL : SettingsDAL, ISchemeMaster
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        List<SchemeMasterDTO> lstSchemeMasterdetails { get; set; }
        public List<schemeConfigurationDTO> schemeconfigurationdetails { get; set; }
        public List<schemechargesconfigDTO> schemechargesconfigdetails { get; set; }
        public List<SchemeReferralCommissioLoanDTO> SchemeReferralCommissioLoandetails { get; set; }


        public bool saveSchemeMaster(SchemeMasterDTO schememasterlist, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            long schemeid;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (string.IsNullOrEmpty(schememasterlist.pEffectfromdate))
                {
                    schememasterlist.pEffectfromdate = "null";
                }
                else
                {
                    schememasterlist.pEffectfromdate = "'" + FormatDate(schememasterlist.pEffectfromdate) + "'";
                }
                if (string.IsNullOrEmpty(schememasterlist.pEffecttodate))
                {
                    schememasterlist.pEffecttodate = "null";
                }
                else
                {
                    schememasterlist.pEffecttodate = "'" + FormatDate(schememasterlist.pEffecttodate) + "'";
                }

                schemeid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstschemenamescodes(schemename,schemecode,statusid,createdby,createddate,loantypeid,loanid,effectfromdate,effecttodate)values('" + ManageQuote(schememasterlist.pSchemename.Trim()) + "','" + ManageQuote(schememasterlist.pSchemecode.Trim()) + "'," + getStatusid(schememasterlist.pStatusname, connectionstring) + "," + schememasterlist.pCreatedby + ",current_timestamp," + schememasterlist.pLoantypeid + "," + schememasterlist.pLoanid + "," + schememasterlist.pEffectfromdate + "," + schememasterlist.pEffecttodate + ") returning schemeid"));

                if (schememasterlist.schemeConfigurationList != null)
                {
                    for (int i = 0; i < schememasterlist.schemeConfigurationList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pEffectfromdate))
                        {

                            schememasterlist.schemeConfigurationList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.schemeConfigurationList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.schemeConfigurationList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pEffecttodate))
                        {

                            schememasterlist.schemeConfigurationList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.schemeConfigurationList[i].pEffecttodate = "'" + FormatDate(schememasterlist.schemeConfigurationList[i].pEffecttodate) + "'";
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pMinloanamount.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pMinloanamount = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pMaxloanamount.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pMaxloanamount = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pTenurefrom.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pTenurefrom = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pTenureto.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pTenureto = 0;
                        }
                        sbinsert.Append("insert into tblmstloanwiseschemeconfiguration(loantypeid, loanid,loanconfigid,schemeid, contacttype, applicanttype, loanpayin, minloanamount, maxloanamount, tenurefrom, tenureto, interesttype, actualrateofinterest, schemeinterest,effectfromdate,effecttodate,istenurerangeapplicable, isamountrangeapplicable, statusid, createdby, createddate)values(" + schememasterlist.schemeConfigurationList[i].pLoantypeid + "," + schememasterlist.schemeConfigurationList[i].pLoanid + "," + schememasterlist.schemeConfigurationList[i].pLoanconfigid + "," + schemeid + ",'" + ManageQuote(schememasterlist.schemeConfigurationList[i].pContacttype) + "','" + ManageQuote(schememasterlist.schemeConfigurationList[i].pApplicanttype) + "','" + ManageQuote(schememasterlist.schemeConfigurationList[i].pLoanpayin) + "',coalesce(" + schememasterlist.schemeConfigurationList[i].pMinloanamount + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pMaxloanamount + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pTenurefrom + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pTenureto + ",0),'" + ManageQuote(schememasterlist.schemeConfigurationList[i].PInteresttype) + "'," + schememasterlist.schemeConfigurationList[i].pActualrateofinterest + "," + schememasterlist.schemeConfigurationList[i].pSchemeinterest + "," + schememasterlist.schemeConfigurationList[i].pEffectfromdate + "," + schememasterlist.schemeConfigurationList[i].pEffecttodate + "," + schememasterlist.schemeConfigurationList[i].pIstenurerangeapplicable + "," + schememasterlist.schemeConfigurationList[i].pIsamountrangeapplicable + "," + getStatusid(schememasterlist.pStatusname, connectionstring) + "," + schememasterlist.schemeConfigurationList[i].pCreatedby + ",current_timestamp);");
                    }

                }

                if (schememasterlist.schemechargesconfigList != null)
                {
                    for (int i = 0; i < schememasterlist.schemechargesconfigList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pEffectfromdate))
                        {

                            schememasterlist.schemechargesconfigList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.schemechargesconfigList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.schemechargesconfigList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pEffecttodate))
                        {

                            schememasterlist.schemechargesconfigList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.schemechargesconfigList[i].pEffecttodate = "'" + FormatDate(schememasterlist.schemechargesconfigList[i].pEffecttodate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pminloanamountortenure.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pminloanamountortenure = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pschemechargespercentage.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pschemechargespercentage = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pchargesvalue = 0;
                        }


                        sbinsert.Append("INSERT INTO tblmstloanwiseschemechargesconfig(   schemeid, loantypeid, loanid,loanchargesdetailsid, loanchargeid, chargename, loanpayin, ischargeapplicableonloanrange, chargevaluefixedorpercentage, applicanttype, ischargerangeapplicableonvalueortenure, minloanamountortenure, maxloanamountortenure, chargepercentage, minchargesvalue, maxchargesvalue, chargecalonfield, gsttype, gstvalue, chargesvalue,effectfromdate, effecttodate, statusid, createdby, createddate)values(" + schemeid + "," + schememasterlist.schemechargesconfigList[i].pLoantypeid + "," + schememasterlist.schemechargesconfigList[i].pLoanid + "," + schememasterlist.schemechargesconfigList[i].pLoanchargesdetailsid + "," + schememasterlist.schemechargesconfigList[i].pLoanchargeid + ",'" + ManageQuote(schememasterlist.schemechargesconfigList[i].pChargename) + "','" + ManageQuote(schememasterlist.schemechargesconfigList[i].pLoanpayin) + "','" + schememasterlist.schemechargesconfigList[i].pischargeapplicableonloanrange + "','" + schememasterlist.schemechargesconfigList[i].pchargevaluefixedorpercentage + "','" + schememasterlist.schemechargesconfigList[i].papplicanttype + "','" + schememasterlist.schemechargesconfigList[i].pischargerangeapplicableonvalueortenure + "',coalesce(" + schememasterlist.schemechargesconfigList[i].pminloanamountortenure + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pschemechargespercentage + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue + ",0),'" + schememasterlist.schemechargesconfigList[i].pchargecalonfield + "','" + schememasterlist.schemechargesconfigList[i].pgsttype + "',coalesce(" + schememasterlist.schemechargesconfigList[i].pgstvalue + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pchargesvalue + ",0)," + schememasterlist.schemechargesconfigList[i].pEffectfromdate + "," + schememasterlist.schemechargesconfigList[i].pEffecttodate + ",1," + schememasterlist.schemechargesconfigList[i].pCreatedby + ",current_timestamp);");
                    }

                }
                if (schememasterlist.SchemeReferralCommissioLoanList != null)
                {
                    for (int i = 0; i < schememasterlist.SchemeReferralCommissioLoanList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate))
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate))
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate = "'" + FormatDate(schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate) + "'";
                        }
                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout.ToString()))
                        {
                            schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout = 0;
                        }
                        sbinsert.Append("INSERT INTO tblmstloanwiseschemereferralcommisionconfig(schemeid, loantypeid, loanid, isreferralcomexist, commissionpayouttype, actualcommissionpayout, schemecommissionpayout, effectfromdate, effecttodate, statusid, createdby, createddate,schemecommissionpayouttype)values(" + schemeid + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pLoantypeId + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pLoanid + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pIsreferralcomexist + ",'" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayouttype) + "','" + (schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayout) + "',coalesce(" + schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout + ",0)," + schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate + ",1," + schememasterlist.SchemeReferralCommissioLoanList[i].pCreatedby + ",current_timestamp,'" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayouttype) + "');");
                    }
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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
        public List<SchemeMasterDTO> getSchemeMasterDetails(string ConnectionString)
        {
            lstSchemeMasterdetails = new List<SchemeMasterDTO>();

            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tsc.loantypeid,tsc.loanid, loantype,loanname,schemeid,schemename,schemecode,statusname,effectfromdate,effecttodate from  tblmstschemenamescodes tsc join tblmstloans tl on tsc.loanid=tl.loanid join tblmstloantypes tt on tt.loantypeid = tl.loantypeid  join tblmststatus ts on tsc.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE'  order by loantype, loanname,schemeid;"))
                {
                    while (dr.Read())
                    {
                        SchemeMasterDTO objmasterdetails = new SchemeMasterDTO();
                        objmasterdetails.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                        objmasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objmasterdetails.pLoantype = Convert.ToString(dr["loantype"]);
                        objmasterdetails.pLoanname = Convert.ToString(dr["loanname"]);
                        objmasterdetails.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        objmasterdetails.pSchemename = Convert.ToString(dr["schemename"]);
                        objmasterdetails.pSchemecode = Convert.ToString(dr["schemecode"]);
                        objmasterdetails.pStatusname = Convert.ToString(dr["statusname"]);

                        if (dr["effectfromdate"] != null)
                        {
                            objmasterdetails.pEffectfromdate = Convert.ToDateTime(dr["effectfromdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objmasterdetails.pEffectfromdate = null;
                        }

                        if (dr["effecttodate"] != null)
                        {
                            objmasterdetails.pEffecttodate = Convert.ToDateTime(dr["effecttodate"]).ToString("dd/MM/yyyy");

                        }
                        else
                        {
                            objmasterdetails.pEffecttodate = null;
                        }

                        //objmasterdetails.schemeConfigurationList = getschemeconfigurationDetails(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        //objmasterdetails.schemechargesconfigList = getschemechargesconfigList(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        //objmasterdetails.SchemeReferralCommissioLoanList = getSchemeReferralCommissionList(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        lstSchemeMasterdetails.Add(objmasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstSchemeMasterdetails;
        }

        public List<SchemeMasterDTO> getSchemeMasterDetails(Int64 SCHEMEID, string ConnectionString)
        {
            lstSchemeMasterdetails = new List<SchemeMasterDTO>();

            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tsc.loantypeid,tsc.loanid, loantype,loanname,schemeid,schemename,schemecode,statusname,effectfromdate,effecttodate from  tblmstschemenamescodes tsc join tblmstloans tl on tsc.loanid=tl.loanid join tblmstloantypes tt on tt.loantypeid = tl.loantypeid  join tblmststatus ts on tl.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' and tsc.schemeid=" + SCHEMEID + " order by loantype, loanname,schemeid;"))
                {
                    while (dr.Read())
                    {
                        SchemeMasterDTO objmasterdetails = new SchemeMasterDTO();
                        objmasterdetails.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                        objmasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objmasterdetails.pLoantype = Convert.ToString(dr["loantype"]);
                        objmasterdetails.pLoanname = Convert.ToString(dr["loanname"]);
                        objmasterdetails.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        objmasterdetails.pSchemename = Convert.ToString(dr["schemename"]);
                        objmasterdetails.pSchemecode = Convert.ToString(dr["schemecode"]);
                        objmasterdetails.pStatusname = Convert.ToString(dr["statusname"]);

                        if (dr["effectfromdate"] != null)
                        {

                            objmasterdetails.pEffectfromdate = Convert.ToDateTime(dr["effectfromdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objmasterdetails.pEffectfromdate = null;
                        }

                        if (dr["effecttodate"] != null)
                        {
                            objmasterdetails.pEffecttodate = Convert.ToDateTime(dr["effecttodate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objmasterdetails.pEffecttodate = null;
                        }

                        objmasterdetails.schemeConfigurationList = getschemeconfigurationDetails(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        objmasterdetails.schemechargesconfigList = getschemechargesconfigList(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        objmasterdetails.SchemeReferralCommissioLoanList = getSchemeReferralCommissionList(ConnectionString, objmasterdetails.pLoanid, objmasterdetails.pSchemeid);
                        lstSchemeMasterdetails.Add(objmasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstSchemeMasterdetails;
        }

        public List<SchemeMasterDTO> getLoanspecificSchemeMasterDetails(string ConnectionString, Int64 loanid)
        {
            lstSchemeMasterdetails = new List<SchemeMasterDTO>();

            try
            {
                SchemeMasterDTO objmasterdetails = new SchemeMasterDTO();
                objmasterdetails.schemeConfigurationList = getschemeconfigurationDetails(ConnectionString, loanid, 0);
                objmasterdetails.schemechargesconfigList = getschemechargesconfigList(ConnectionString, loanid, 0);
                objmasterdetails.SchemeReferralCommissioLoanList = getSchemeReferralCommissionList(ConnectionString, loanid, 0);
                lstSchemeMasterdetails.Add(objmasterdetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSchemeMasterdetails;
        }

        private List<schemeConfigurationDTO> getschemeconfigurationDetails(string connectionString, Int64 loanid, Int64 schemeid)
        {
            string strQuery = string.Empty;
            schemeconfigurationdetails = new List<schemeConfigurationDTO>();
            try
            {
                if (schemeid > 0)
                {
                    //strQuery = "select distinct coalesce(schemeid,0) as schemeid, tc.loantypeid, tc.loanid ,tc.loanconfigid, tc.applicanttype, tc.contacttype, tc.loanpayin,coalesce( tc.minloanamount,0) as minloanamount,coalesce( tc.maxloanamount,0) as maxloanamount, coalesce(tc.tenurefrom,0) as tenurefrom, coalesce(tc.tenureto,0) as tenureto, tc.interesttype, coalesce(tc.rateofinterest,0) as rateofinterest, coalesce(tsc.schemeinterest,tc.rateofinterest) as schemeinterest,tc.istenurerangeapplicable,tc.isamountrangeapplicable, statusname,coalesce(tsc.recordid,0) as recordid from TBLMSTLOANCONFIGURATION tc left join tblmstloanwiseschemeconfiguration tsc on tc.loanid = tsc.loanid and  tc.loanconfigid = tsc.loanconfigid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and schemeid=" + schemeid + "  and upper(ts.statusname) = 'ACTIVE' order by tc.loanconfigid;";

                    strQuery = "select distinct coalesce(schemeid,0) as schemeid, tc.loantypeid, tc.loanid ,tc.loanconfigid, tc.applicanttype, tc.contacttype, tc.loanpayin,coalesce( tc.minloanamount,0) as minloanamount,coalesce( tc.maxloanamount,0) as maxloanamount, coalesce(tc.tenurefrom,0) as tenurefrom, coalesce(tc.tenureto,0) as tenureto, tc.interesttype, coalesce(tc.rateofinterest,0) as rateofinterest, coalesce(tsc.schemeinterest,tc.rateofinterest) as schemeinterest,tc.istenurerangeapplicable,tc.isamountrangeapplicable, statusname,coalesce(tsc.recordid,0) as recordid,'UPDATE' AS TYPEOFOPERATION from TBLMSTLOANCONFIGURATION tc left join tblmstloanwiseschemeconfiguration tsc on tc.loanid = tsc.loanid  and  tc.loanconfigid = tsc.loanconfigid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and schemeid=" + schemeid + " and upper(ts.statusname) = 'ACTIVE' union all select distinct coalesce(schemeid, 0) as schemeid, tc.loantypeid, tc.loanid ,tc.loanconfigid, tc.applicanttype, tc.contacttype, tc.loanpayin,coalesce(tc.minloanamount, 0) as minloanamount,coalesce(tc.maxloanamount, 0) as maxloanamount, coalesce(tc.tenurefrom, 0) as tenurefrom, coalesce(tc.tenureto, 0) as tenureto, tc.interesttype, coalesce(tc.rateofinterest, 0) as rateofinterest, coalesce(tc.rateofinterest, 0) as schemeinterest,tc.istenurerangeapplicable,tc.isamountrangeapplicable, statusname,0 as recordid,'CREATE' AS typeofoperation from TBLMSTLOANCONFIGURATION tc left join tblmstloanwiseschemeconfiguration tsc on tc.loanid = tsc.loanid   join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and schemeid = " + schemeid + " and upper(ts.statusname) = 'ACTIVE'  and tc.loanconfigid not in(select loanconfigid from tblmstloanwiseschemeconfiguration where  loanid = " + loanid + " and schemeid = " + schemeid + ")  order by loanconfigid; ";

                    //strQuery = "select distinct coalesce(schemeid,0) as schemeid, tc.loantypeid, tc.loanid ,tc.loanconfigid, tc.applicanttype, tc.contacttype, tc.loanpayin,coalesce( tc.minloanamount,0) as minloanamount,coalesce( tc.maxloanamount,0) as maxloanamount, coalesce(tc.tenurefrom,0) as tenurefrom, coalesce(tc.tenureto,0) as tenureto, tc.interesttype, coalesce(tc.rateofinterest,0) as rateofinterest, coalesce(tsc.schemeinterest,tc.rateofinterest) as schemeinterest,tc.istenurerangeapplicable,tc.isamountrangeapplicable, statusname,coalesce(tsc.recordid,0) as recordid,case when schemeid is null then 'CREATE' ELSE 'UPDATE' END AS TYPEOFOPERATION from TBLMSTLOANCONFIGURATION tc left join tblmstloanwiseschemeconfiguration tsc on tc.loanid = tsc.loanid  and  tc.loanconfigid = tsc.loanconfigid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and (schemeid=" + schemeid + " or schemeid is null) and upper(ts.statusname) = 'ACTIVE';";
                }
                else
                {
                    strQuery = "select distinct 0 as schemeid, tc.loantypeid, tc.loanid ,tc.loanconfigid, tc.applicanttype, tc.contacttype, tc.loanpayin,coalesce( tc.minloanamount,0) as minloanamount,coalesce( tc.maxloanamount,0) as maxloanamount, coalesce(tc.tenurefrom,0) as tenurefrom, coalesce(tc.tenureto,0) as tenureto, tc.interesttype, coalesce(tc.rateofinterest,0) as rateofinterest, coalesce(tc.rateofinterest,0) as schemeinterest,tc.istenurerangeapplicable,tc.isamountrangeapplicable, statusname,0 as recordid,'CREATE' AS typeofoperation from TBLMSTLOANCONFIGURATION tc  join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid  = " + loanid + " and upper(ts.statusname) = 'ACTIVE' order by tc.loanconfigid;";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        schemeConfigurationDTO objamasterdetails = new schemeConfigurationDTO();
                        objamasterdetails.pRecordid = Convert.ToInt32(dr["recordid"]);
                        objamasterdetails.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        objamasterdetails.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                        objamasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objamasterdetails.pLoanconfigid = Convert.ToInt64(dr["loanconfigid"]);
                        objamasterdetails.pContacttype = dr["contacttype"].ToString();
                        objamasterdetails.pApplicanttype = dr["applicanttype"].ToString();
                        objamasterdetails.pLoanpayin = dr["loanpayin"].ToString();
                        objamasterdetails.pMinloanamount = Convert.ToDecimal(dr["minloanamount"]);
                        objamasterdetails.pMaxloanamount = Convert.ToDecimal(dr["maxloanamount"]);
                        objamasterdetails.pTenurefrom = Convert.ToInt64(dr["tenurefrom"]);
                        objamasterdetails.pTenureto = Convert.ToInt64(dr["tenureto"]);
                        objamasterdetails.PInteresttype = dr["interesttype"].ToString();
                        objamasterdetails.pActualrateofinterest = Convert.ToDecimal(dr["rateofinterest"]);
                        objamasterdetails.pSchemeinterest = Convert.ToDecimal(dr["schemeinterest"]);
                        objamasterdetails.pStatusname = dr["statusname"].ToString();
                        objamasterdetails.pIsamountrangeapplicable = Convert.ToBoolean(dr["isamountrangeapplicable"]);
                        objamasterdetails.pIstenurerangeapplicable = Convert.ToBoolean(dr["istenurerangeapplicable"]);
                        objamasterdetails.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        schemeconfigurationdetails.Add(objamasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return schemeconfigurationdetails;
        }

        private List<schemechargesconfigDTO> getschemechargesconfigList(string connectionString, Int64 loanid, Int64 schemeid)
        {
            string strQuery = string.Empty;
            schemechargesconfigdetails = new List<schemechargesconfigDTO>();
            try
            {
                if (schemeid > 0)
                {
                    //strQuery = "select distinct coalesce(tsc.recordid,0) as recordid,coalesce(schemeid,0) as schemeid,tc.loantypeid, tc.loanid,tsc.loanchargesdetailsid ,tc.loanchargeid, tc.chargename, tc.loanpayin, coalesce(tc.minloanamountortenure, 0) as amountortenurefrom, coalesce(tc.maxloanamountortenure, 0) as amountortenureto, tc.ischargeapplicableonloanrange, tc.chargevaluefixedorpercentage,tc.applicanttype,tc.ischargerangeapplicableonvalueortenure,coalesce(tc.chargepercentage,0) AS chargepercentage,coalesce(tsc.chargepercentage,tc.chargepercentage) AS schemechargepercentage,coalesce(tc.minchargesvalue,0) as minchargevalue,coalesce(tc.maxchargesvalue,0) as maxchargevalue,coalesce(tsc.minchargesvalue, tc.minchargesvalue) as schememinchargevalue,coalesce(tsc.maxchargesvalue, tc.maxchargesvalue) as schememaxchargevalue,tc.chargecalonfield,tc.gsttype,tc.gstvalue,statusname from tblmstloanwisechargesconfig tc left join tblmstloanwiseschemechargesconfig tsc on tc.loanid = tsc.loanid and tc.recordid = tsc.loanchargesdetailsid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and schemeid = " + schemeid + "  and upper(ts.statusname) = 'ACTIVE' order by tc.loanchargeid;";


                    strQuery = "select distinct coalesce(tsc.recordid, 0) as recordid,coalesce(schemeid, 0) as schemeid,tc.loantypeid, tc.loanid,tsc.loanchargesdetailsid ,tc.loanchargeid, tc.chargename, tc.loanpayin, coalesce(tc.minloanamountortenure, 0) as amountortenurefrom, coalesce(tc.maxloanamountortenure, 0) as amountortenureto, tc.ischargeapplicableonloanrange, tc.chargevaluefixedorpercentage,tc.applicanttype,tc.ischargerangeapplicableonvalueortenure,coalesce(tc.chargepercentage, 0) AS chargepercentage, coalesce(tsc.chargepercentage, tc.chargepercentage) AS schemechargepercentage, coalesce(tc.minchargesvalue, 0) as minchargevalue,coalesce(tc.maxchargesvalue, 0) as maxchargevalue,coalesce(tsc.minchargesvalue, tc.minchargesvalue) as schememinchargevalue,coalesce(tsc.maxchargesvalue, tc.maxchargesvalue) as schememaxchargevalue,tc.chargecalonfield,tc.gsttype,tc.gstvalue,statusname,'UPDATE' AS TYPEOFOPERATION  from tblmstloanwisechargesconfig tc left join tblmstloanwiseschemechargesconfig tsc on tc.loanid = tsc.loanid and tc.recordid = tsc.loanchargesdetailsid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid =  " + loanid + " and schemeid = " + schemeid + "   and upper(ts.statusname) = 'ACTIVE' union all select distinct 0 as recordid,coalesce(schemeid, 0) as schemeid,tc.loantypeid, tc.loanid,tc.recordid as loanchargesdetailsid ,tc.loanchargeid, tc.chargename, tc.loanpayin, coalesce(tc.minloanamountortenure, 0) as amountortenurefrom, coalesce(tc.maxloanamountortenure, 0) as amountortenureto, tc.ischargeapplicableonloanrange, tc.chargevaluefixedorpercentage,tc.applicanttype,tc.ischargerangeapplicableonvalueortenure,coalesce(tc.chargepercentage, 0) AS chargepercentage, coalesce(tc.chargepercentage,0) AS schemechargepercentage, coalesce(tc.minchargesvalue, 0) as minchargevalue,coalesce(tc.maxchargesvalue, 0) as maxchargevalue,coalesce(tc.minchargesvalue,0) as schememinchargevalue,coalesce(tc.maxchargesvalue,0) as schememaxchargevalue,tc.chargecalonfield,tc.gsttype,tc.gstvalue,statusname,'CREATE' AS TYPEOFOPERATION  from tblmstloanwisechargesconfig tc left join tblmstloanwiseschemechargesconfig tsc on tc.loanid = tsc.loanid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid =  " + loanid + " and schemeid = " + schemeid + "   and upper(ts.statusname) = 'ACTIVE' and tc.recordid not in(select loanchargesdetailsid from tblmstloanwiseschemechargesconfig where  loanid =  " + loanid + " and schemeid = " + schemeid + " )  order by recordid;";

                }
                else
                {
                    strQuery = "select distinct 0 as recordid, 0 as schemeid,tc.loantypeid, tc.loanid,tc.recordid as loanchargesdetailsid,tc.loanchargeid, tc.chargename, tc.loanpayin, coalesce(tc.minloanamountortenure, 0) as amountortenurefrom, coalesce(tc.maxloanamountortenure, 0) as amountortenureto, tc.ischargeapplicableonloanrange, tc.chargevaluefixedorpercentage,tc.applicanttype,tc.ischargerangeapplicableonvalueortenure,coalesce(tc.chargepercentage,0) AS chargepercentage,coalesce(tc.chargepercentage,0) AS schemechargepercentage,coalesce(tc.minchargesvalue, 0) as minchargevalue,coalesce(tc.maxchargesvalue, 0) as maxchargevalue,coalesce(tc.minchargesvalue, 0) as schememinchargevalue,coalesce(tc.maxchargesvalue, 0) as schememaxchargevalue,tc.chargecalonfield,tc.gsttype,tc.gstvalue,statusname,'CREATE' AS TYPEOFOPERATION from tblmstloanwisechargesconfig tc  join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid = " + loanid + " and upper(ts.statusname) = 'ACTIVE' order by tc.loanchargeid;";
                }


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        schemechargesconfigDTO objamasterdetails = new schemechargesconfigDTO();
                        objamasterdetails.pRecordid = Convert.ToInt32(dr["recordid"]);
                        objamasterdetails.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        objamasterdetails.pLoantypeid = Convert.ToInt64(dr["loantypeid"]);
                        objamasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objamasterdetails.pLoanchargesdetailsid = Convert.ToInt64(dr["loanchargesdetailsid"]);
                        objamasterdetails.pLoanchargeid = Convert.ToInt64(dr["loanchargeid"]);
                        objamasterdetails.pChargename = Convert.ToString(dr["chargename"]);
                        objamasterdetails.pLoanpayin = Convert.ToString(dr["loanpayin"]);
                        objamasterdetails.pminloanamountortenure = Convert.ToDecimal(dr["amountortenurefrom"]);
                        objamasterdetails.pmaxloanamountortenure = Convert.ToDecimal(dr["amountortenureto"]);
                        objamasterdetails.pischargeapplicableonloanrange = Convert.ToString(dr["ischargeapplicableonloanrange"]);
                        objamasterdetails.pchargevaluefixedorpercentage = Convert.ToString(dr["chargevaluefixedorpercentage"]);
                        objamasterdetails.papplicanttype = Convert.ToString(dr["applicanttype"]);
                        objamasterdetails.pischargerangeapplicableonvalueortenure = Convert.ToString(dr["ischargerangeapplicableonvalueortenure"]);
                        objamasterdetails.pchargepercentage = Convert.ToDecimal(dr["chargepercentage"]);
                        objamasterdetails.pminchargesvalue = Convert.ToDecimal(dr["minchargevalue"]);
                        objamasterdetails.pmaxchargesvalue = Convert.ToDecimal(dr["maxchargevalue"]);
                        objamasterdetails.pchargecalonfield = Convert.ToString(dr["chargecalonfield"]);
                        objamasterdetails.pgsttype = Convert.ToString(dr["gsttype"]);
                        objamasterdetails.pgstvalue = Convert.ToDecimal(dr["gstvalue"]);
                        objamasterdetails.pStatusname = Convert.ToString(dr["statusname"]);
                        objamasterdetails.pschemechargespercentage = Convert.ToDecimal(dr["schemechargepercentage"]);
                        objamasterdetails.pSchememinchargesvalue = Convert.ToDecimal(dr["schememinchargevalue"]);
                        objamasterdetails.pSchememaxchargesvalue = Convert.ToDecimal(dr["schememaxchargevalue"]);
                        objamasterdetails.ptypeofoperation = Convert.ToString(dr["typeofoperation"]);
                        schemechargesconfigdetails.Add(objamasterdetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return schemechargesconfigdetails;
        }
        public List<SchemeReferralCommissioLoanDTO> getSchemeReferralCommissionList(string ConnectionString, Int64 loanid, Int64 schemeid)
        {
            string strQuery = string.Empty;
            SchemeReferralCommissioLoandetails = new List<SchemeReferralCommissioLoanDTO>();
            try
            {
                if (schemeid > 0)
                {
                    strQuery = "select coalesce(schemeid,0) as schemeid,t1.loantypeid,t1.loanid,t1.isreferralcomexist,t1.commissionpayouttype,t1.commissionpayout,coalesce(t2.schemecommissionpayout,t1.commissionpayout) as schemecommissionpayout,t2.schemecommissionpayouttype,ts.statusname,coalesce(t2.recordid,0) as recordid from tblmstloanwisereferralcommisionconfig t1 left join tblmstloanwiseschemereferralcommisionconfig t2 on t1.loanid=t2.loanid join tblmststatus ts on t1.statusid = ts.statusid where t1.loanid =  " + loanid + " and t2.schemeid=" + schemeid + " and upper(ts.statusname) = 'ACTIVE';";
                }
                else
                {
                    strQuery = "select 0 as schemeid,t1.loantypeid,t1.loanid,t1.isreferralcomexist,t1.commissionpayouttype,t1.commissionpayout,t1.commissionpayout ,t1.commissionpayout as schemecommissionpayout,'' as schemecommissionpayouttype,ts.statusname,0 as recordid from tblmstloanwisereferralcommisionconfig t1 join tblmststatus ts on t1.statusid = ts.statusid where t1.loanid = " + loanid + " and upper(ts.statusname) = 'ACTIVE';";
                }
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, strQuery))
                {
                    while (dr.Read())
                    {
                        SchemeReferralCommissioLoanDTO obj = new SchemeReferralCommissioLoanDTO();
                        obj.pRecordid = Convert.ToInt32(dr["recordid"]);
                        obj.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        obj.pLoantypeId = Convert.ToInt32(dr["loantypeid"]);
                        obj.pLoanid = Convert.ToInt32(dr["loanid"]);
                        obj.pIsreferralcomexist = Convert.ToBoolean(dr["isreferralcomexist"]);
                        obj.pCommissionpayouttype = Convert.ToString((dr["Commissionpayouttype"]));
                        obj.pCommissionpayout = Convert.ToDecimal(dr["Commissionpayout"]);
                        obj.pSchemecommissionpayout = Convert.ToDecimal(dr["schemecommissionpayout"]);
                        obj.pSchemecommissionpayouttype = Convert.ToString((dr["schemecommissionpayouttype"]));
                        obj.pStatusname = Convert.ToString((dr["statusname"]));
                        SchemeReferralCommissioLoandetails.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SchemeReferralCommissioLoandetails;
        }
        public SchemeMasterDTO getSchemeMasterDetailsbyId(Int64 schemeId, Int64 loanId, string ConnectionString)
        {

            SchemeMasterDTO SchemeMasterdetails = new SchemeMasterDTO();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tsc.loantypeid,tsc.loanid, loantype,loanname,schemeid,schemename,schemecode,effectfromdate,effecttodate,statusname from  tblmstschemenamescodes tsc join tblmstloans tl on tsc.loanid=tl.loanid join tblmstloantypes tt on tt.loantypeid = tl.loantypeid  join tblmststatus ts on tl.statusid = ts.statusid where upper(ts.statusname) = 'ACTIVE' and tsc.loanid=" + loanId + " and tsc.schemeid=" + schemeId + "  order by loantype, loanname;"))
                {
                    while (dr.Read())
                    {
                        SchemeMasterDTO objmasterdetails = new SchemeMasterDTO();
                        SchemeMasterdetails.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        SchemeMasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        SchemeMasterdetails.pLoantype = Convert.ToString(dr["loantype"]);
                        SchemeMasterdetails.pLoanname = Convert.ToString(dr["loanname"]);
                        SchemeMasterdetails.pSchemeid = Convert.ToInt64(dr["schemeid"]);
                        SchemeMasterdetails.pSchemename = Convert.ToString(dr["schemename"]);
                        SchemeMasterdetails.pEffectfromdate = Convert.ToDateTime(dr["effectfromdate"]).ToString("dd/MM/yyyy");
                        SchemeMasterdetails.pEffecttodate = Convert.ToDateTime(dr["effecttodate"]).ToString("dd/MM/yyyy");
                        SchemeMasterdetails.pSchemecode = Convert.ToString(dr["schemecode"]);
                        SchemeMasterdetails.pStatusname = Convert.ToString(dr["statusname"]);
                        SchemeMasterdetails.schemeConfigurationList = getschemeconfigurationDetails(ConnectionString, SchemeMasterdetails.pLoanid, SchemeMasterdetails.pSchemeid);
                        SchemeMasterdetails.schemechargesconfigList = getschemechargesconfigList(ConnectionString, SchemeMasterdetails.pLoanid, SchemeMasterdetails.pSchemeid);
                        SchemeMasterdetails.SchemeReferralCommissioLoanList = getSchemeReferralCommissionList(ConnectionString, SchemeMasterdetails.pLoanid, SchemeMasterdetails.pSchemeid);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return SchemeMasterdetails;
        }

        public int CheckDuplicateSchemeNames(string SchemeName, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstschemenamescodes where upper(schemename)='" + SchemeName.ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public int CheckDuplicateSchemeCodes(string Schemecode, string ConnectionString)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstschemenamescodes where upper(schemecode)='" + Schemecode.ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }
        public bool UpdateSchemeMaster(SchemeMasterDTO schememasterlist, string connectionstring)
        {
            bool isUpdated = false;
            StringBuilder sbUpdate = new StringBuilder();
            long schemeid;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                if (string.IsNullOrEmpty(schememasterlist.pEffectfromdate))
                {
                    schememasterlist.pEffectfromdate = "null";
                }
                else
                {
                    schememasterlist.pEffectfromdate = "'" + FormatDate(schememasterlist.pEffectfromdate) + "'";
                }
                if (string.IsNullOrEmpty(schememasterlist.pEffecttodate))
                {
                    schememasterlist.pEffecttodate = "null";
                }
                else
                {
                    schememasterlist.pEffecttodate = "'" + FormatDate(schememasterlist.pEffecttodate) + "'";
                }

                sbUpdate.Append("update tblmstschemenamescodes set effectfromdate=" + schememasterlist.pEffectfromdate + ", effecttodate=" + schememasterlist.pEffecttodate + ", modifiedby=" + schememasterlist.pCreatedby + " where schemeid=" + schememasterlist.pSchemeid + ";");
                if (schememasterlist.schemeConfigurationList != null)
                {
                    for (int i = 0; i < schememasterlist.schemeConfigurationList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pEffectfromdate))
                        {

                            schememasterlist.schemeConfigurationList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.schemeConfigurationList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.schemeConfigurationList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pEffecttodate))
                        {

                            schememasterlist.schemeConfigurationList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.schemeConfigurationList[i].pEffecttodate = "'" + FormatDate(schememasterlist.schemeConfigurationList[i].pEffecttodate) + "'";
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pMinloanamount.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pMinloanamount = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pMaxloanamount.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pMaxloanamount = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pTenurefrom.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pTenurefrom = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemeConfigurationList[i].pTenureto.ToString()))
                        {
                            schememasterlist.schemeConfigurationList[i].pTenureto = 0;
                        }
                        if (schememasterlist.schemeConfigurationList[i].ptypeofoperation == "UPDATE")
                        {
                            sbUpdate.Append("update tblmstloanwiseschemeconfiguration set applicanttype='" + ManageQuote(schememasterlist.schemeConfigurationList[i].pApplicanttype) + "',loanpayin='" + ManageQuote(schememasterlist.schemeConfigurationList[i].pLoanpayin) + "',minloanamount=coalesce(" + schememasterlist.schemeConfigurationList[i].pMinloanamount + ",0),maxloanamount=coalesce(" + schememasterlist.schemeConfigurationList[i].pMaxloanamount + ",0),tenurefrom=coalesce(" + schememasterlist.schemeConfigurationList[i].pTenurefrom + ",0),tenureto=coalesce(" + schememasterlist.schemeConfigurationList[i].pTenureto + ",0),interesttype='" + ManageQuote(schememasterlist.schemeConfigurationList[i].PInteresttype) + "',actualrateofinterest=" + schememasterlist.schemeConfigurationList[i].pActualrateofinterest + ",schemeinterest=" + schememasterlist.schemeConfigurationList[i].pSchemeinterest + ",effectfromdate=" + schememasterlist.schemeConfigurationList[i].pEffectfromdate + ",effecttodate=" + schememasterlist.schemeConfigurationList[i].pEffecttodate + ",modifiedby=" + schememasterlist.schemeConfigurationList[i].pCreatedby + ",modifieddate=current_timestamp where recordid=" + schememasterlist.schemeConfigurationList[i].pRecordid + " and loanid=" + schememasterlist.schemeConfigurationList[i].pLoanid + ";");
                        }
                        else if (schememasterlist.schemeConfigurationList[i].ptypeofoperation == "CREATE")
                        {
                            sbUpdate.Append("insert into tblmstloanwiseschemeconfiguration(loantypeid, loanid,loanconfigid,schemeid, contacttype, applicanttype, loanpayin, minloanamount, maxloanamount, tenurefrom, tenureto, interesttype, actualrateofinterest, schemeinterest,effectfromdate,effecttodate,  statusid, createdby, createddate)values(" + schememasterlist.schemeConfigurationList[i].pLoantypeid + "," + schememasterlist.schemeConfigurationList[i].pLoanid + "," + schememasterlist.schemeConfigurationList[i].pLoanconfigid + "," + schememasterlist.pSchemeid + ",'" + ManageQuote(schememasterlist.schemeConfigurationList[i].pContacttype) + "','" + ManageQuote(schememasterlist.schemeConfigurationList[i].pApplicanttype) + "','" + ManageQuote(schememasterlist.schemeConfigurationList[i].pLoanpayin) + "',coalesce(" + schememasterlist.schemeConfigurationList[i].pMinloanamount + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pMaxloanamount + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pTenurefrom + ",0),coalesce(" + schememasterlist.schemeConfigurationList[i].pTenureto + ",0),'" + ManageQuote(schememasterlist.schemeConfigurationList[i].PInteresttype) + "'," + schememasterlist.schemeConfigurationList[i].pActualrateofinterest + "," + schememasterlist.schemeConfigurationList[i].pSchemeinterest + "," + schememasterlist.schemeConfigurationList[i].pEffectfromdate + "," + schememasterlist.schemeConfigurationList[i].pEffecttodate + "," + getStatusid(schememasterlist.pStatusname, connectionstring) + "," + schememasterlist.schemeConfigurationList[i].pCreatedby + ",current_timestamp);");
                        }
                    }
                }
                if (schememasterlist.schemechargesconfigList != null)
                {
                    for (int i = 0; i < schememasterlist.schemechargesconfigList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pEffectfromdate))
                        {

                            schememasterlist.schemechargesconfigList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.schemechargesconfigList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.schemechargesconfigList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pEffecttodate))
                        {

                            schememasterlist.schemechargesconfigList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.schemechargesconfigList[i].pEffecttodate = "'" + FormatDate(schememasterlist.schemechargesconfigList[i].pEffecttodate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pminloanamountortenure.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pminloanamountortenure = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pchargepercentage.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pchargepercentage = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue = 0;
                        }
                        if (string.IsNullOrEmpty(schememasterlist.schemechargesconfigList[i].pchargesvalue.ToString()))
                        {
                            schememasterlist.schemechargesconfigList[i].pchargesvalue = 0;
                        }

                        if (schememasterlist.schemechargesconfigList[i].ptypeofoperation == "UPDATE")
                        {
                            sbUpdate.Append("update tblmstloanwiseschemechargesconfig set chargename='" + ManageQuote(schememasterlist.schemechargesconfigList[i].pChargename) + "',loanpayin='" + ManageQuote(schememasterlist.schemechargesconfigList[i].pLoanpayin) + "',ischargeapplicableonloanrange='" + schememasterlist.schemechargesconfigList[i].pischargeapplicableonloanrange + "',chargevaluefixedorpercentage='" + schememasterlist.schemechargesconfigList[i].pchargevaluefixedorpercentage + "',applicanttype='" + schememasterlist.schemechargesconfigList[i].papplicanttype + "',ischargerangeapplicableonvalueortenure='" + schememasterlist.schemechargesconfigList[i].pischargerangeapplicableonvalueortenure + "',minloanamountortenure=coalesce(" + schememasterlist.schemechargesconfigList[i].pminloanamountortenure + ",0),maxloanamountortenure=coalesce(" + schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure + ",0),chargepercentage=coalesce(" + schememasterlist.schemechargesconfigList[i].pschemechargespercentage + ",0),minchargesvalue=coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue + ",0),maxchargesvalue=coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue + ",0),chargecalonfield='" + schememasterlist.schemechargesconfigList[i].pchargecalonfield + "',gsttype='" + schememasterlist.schemechargesconfigList[i].pgsttype + "',gstvalue=coalesce(" + schememasterlist.schemechargesconfigList[i].pgstvalue + ",0),effectfromdate=" + schememasterlist.schemechargesconfigList[i].pEffectfromdate + ",effecttodate=" + schememasterlist.schemechargesconfigList[i].pEffecttodate + ",chargesvalue=coalesce(" + schememasterlist.schemechargesconfigList[i].pchargesvalue + ",0),modifiedby=" + schememasterlist.schemechargesconfigList[i].pCreatedby + ",modifieddate=current_timestamp where recordid=" + schememasterlist.schemechargesconfigList[i].pRecordid + " and loanid=" + schememasterlist.schemechargesconfigList[i].pLoanid + ";");
                        }
                        else if (schememasterlist.schemechargesconfigList[i].ptypeofoperation == "CREATE")
                        {
                            sbUpdate.Append("INSERT INTO tblmstloanwiseschemechargesconfig(schemeid, loantypeid, loanid, loanchargesdetailsid,loanchargeid, chargename, loanpayin, ischargeapplicableonloanrange, chargevaluefixedorpercentage, applicanttype, ischargerangeapplicableonvalueortenure, minloanamountortenure, maxloanamountortenure, chargepercentage, minchargesvalue, maxchargesvalue, chargecalonfield, gsttype, gstvalue, chargesvalue,effectfromdate, effecttodate, statusid, createdby, createddate)values(" + schememasterlist.pSchemeid + "," + schememasterlist.schemechargesconfigList[i].pLoantypeid + "," + schememasterlist.schemechargesconfigList[i].pLoanid + "," + schememasterlist.schemechargesconfigList[i].pLoanchargesdetailsid + "," + schememasterlist.schemechargesconfigList[i].pLoanchargeid + ",'" + ManageQuote(schememasterlist.schemechargesconfigList[i].pChargename) + "','" + ManageQuote(schememasterlist.schemechargesconfigList[i].pLoanpayin) + "','" + schememasterlist.schemechargesconfigList[i].pischargeapplicableonloanrange + "','" + schememasterlist.schemechargesconfigList[i].pchargevaluefixedorpercentage + "','" + schememasterlist.schemechargesconfigList[i].papplicanttype + "','" + schememasterlist.schemechargesconfigList[i].pischargerangeapplicableonvalueortenure + "',coalesce(" + schememasterlist.schemechargesconfigList[i].pminloanamountortenure + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pmaxloanamountortenure + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pchargepercentage + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememinchargesvalue + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pSchememaxchargesvalue + ",0),'" + schememasterlist.schemechargesconfigList[i].pchargecalonfield + "','" + schememasterlist.schemechargesconfigList[i].pgsttype + "',coalesce(" + schememasterlist.schemechargesconfigList[i].pgstvalue + ",0),coalesce(" + schememasterlist.schemechargesconfigList[i].pchargesvalue + ",0)," + schememasterlist.schemechargesconfigList[i].pEffectfromdate + "," + schememasterlist.schemechargesconfigList[i].pEffecttodate + "," + Convert.ToInt32(Status.Active) + "," + schememasterlist.schemechargesconfigList[i].pCreatedby + ",current_timestamp);");
                        }
                    }
                }
                if (schememasterlist.SchemeReferralCommissioLoanList != null)
                {
                    for (int i = 0; i < schememasterlist.SchemeReferralCommissioLoanList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate))
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate = "'" + FormatDate(schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate) + "'";
                        }

                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate))
                        {

                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate = "null";
                        }
                        else
                        {
                            schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate = "'" + FormatDate(schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate) + "'";
                        }
                        if (string.IsNullOrEmpty(schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout.ToString()))
                        {
                            schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout = 0;
                        }
                        //if (schememasterlist.SchemeReferralCommissioLoanList[i].ptypeofoperation == "UPDATE")
                        //{
                        sbUpdate.Append("update tblmstloanwiseschemereferralcommisionconfig set isreferralcomexist=" + schememasterlist.SchemeReferralCommissioLoanList[i].pIsreferralcomexist + ", commissionpayouttype='" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayouttype) + "', actualcommissionpayout='" + (schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayout) + "', schemecommissionpayout=coalesce(" + schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout + ",0), effectfromdate=" + schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate + ", effecttodate=" + schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate + ",schemecommissionpayouttype='" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayouttype) + "',modifiedby=" + schememasterlist.SchemeReferralCommissioLoanList[i].pCreatedby + ",modifieddate=current_timestamp where loanid=" + schememasterlist.SchemeReferralCommissioLoanList[i].pLoanid + " and statusid=" + Convert.ToInt32(Status.Active) + ";");
                        //}
                        //else if (schememasterlist.SchemeReferralCommissioLoanList[i].ptypeofoperation == "CREATE")
                        //{
                        //    sbUpdate.Append("INSERT INTO tblmstloanwiseschemereferralcommisionconfig(schemeid, loantypeid, loanid, isreferralcomexist, commissionpayouttype, actualcommissionpayout, schemecommissionpayout, effectfromdate, effecttodate, statusid, createdby, createddate,schemecommissionpayouttype)values(" + schememasterlist.pSchemeid + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pLoantypeId + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pLoanid + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pIsreferralcomexist + ",'" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayouttype) + "','" + (schememasterlist.SchemeReferralCommissioLoanList[i].pCommissionpayout) + "',coalesce(" + schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayout + ",0)," + schememasterlist.SchemeReferralCommissioLoanList[i].pEffectfromdate + "," + schememasterlist.SchemeReferralCommissioLoanList[i].pEffecttodate + ",1," + schememasterlist.SchemeReferralCommissioLoanList[i].pCreatedby + ",current_timestamp,'" + ManageQuote(schememasterlist.SchemeReferralCommissioLoanList[i].pSchemecommissionpayouttype) + "');");
                        //}
                    }
                }
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbUpdate.ToString());
                trans.Commit();
                isUpdated = true;
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
            return isUpdated;
        }

        public bool DeleteSchemeMaster(Int64 schemeid, int createdby, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbupdate = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                sbupdate.Append("UPDATE tblmstschemenamescodes set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + createdby + ",modifieddate=current_timestamp where schemeid=" + schemeid + "; ");
                sbupdate.Append("UPDATE tblmstloanwiseschemeconfiguration set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + createdby + ",modifieddate=current_timestamp where schemeid=" + schemeid + "; ");
                sbupdate.Append("UPDATE tblmstloanwiseschemechargesconfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + createdby + ",modifieddate=current_timestamp where schemeid=" + schemeid + "; ");
                sbupdate.Append("UPDATE tblmstloanwiseschemereferralcommisionconfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + createdby + ",modifieddate=current_timestamp where schemeid=" + schemeid + "; ");

                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString());
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
    }
}
