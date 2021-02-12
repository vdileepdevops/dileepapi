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


    public class LoansMasterDAL : SettingsDAL, ILoansMaster
    {

        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        public List<LoansMasterDTO> lstLoanMasterdetails { get; set; }
        public List<loanconfigurationDTO> loanconfigurationdetails { get; set; }
        public List<instalmentdatedetails> loaninstalmentdatedetails { get; set; }
        public List<ReferralCommissioLoanDTO> ReferralCommissioLoandetails { get; set; }
        public List<PenaltyConfigurationDTO> PenaltyConfigurationLoandetails { get; set; }
        public List<loanconfigurationDTO> lstLoanpayins { get; set; }
        public List<loanconfigurationDTO> lstLoanIneterstratetypes { get; set; }

        public List<LoansMasterDTO> getLoanTypes(string ConnectionString)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT loantypeid,loantype from tblmstloantypes where statusid=" + Convert.ToInt32(Status.Active) + " order by loantype "))
                {
                    while (dr.Read())
                    {
                        LoansMasterDTO objamasterdetails = new LoansMasterDTO();
                        objamasterdetails.pLoantype = dr["loantype"].ToString();
                        objamasterdetails.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        lstLoanMasterdetails.Add(objamasterdetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanMasterdetails;
        }

        public List<LoansMasterDTO> getLoanNames(string ConnectionString, int loanTypeId)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT loanid,loanName from tblmstloans t1 join tblmststatus t2 on t1.statusid=t2.statusid where loantypeid=" + loanTypeId + " and loanid in(select loanid from tblmstloanconfiguration where statusid=" + Convert.ToInt32(Status.Active) + ") and t1.statusid=" + Convert.ToInt32(Status.Active) + " order by loanName "))
                {
                    while (dr.Read())
                    {
                        LoansMasterDTO objamasterdetails = new LoansMasterDTO();
                        objamasterdetails.pLoanNmae = dr["loanName"].ToString();
                        objamasterdetails.pLoanid = Convert.ToInt32(dr["loanid"]);
                        lstLoanMasterdetails.Add(objamasterdetails);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanMasterdetails;
        }

        public List<loanconfigurationDTO> getLoanpayins(string ConnectionString)
        {
            lstLoanpayins = new List<loanconfigurationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select laonpayin from tblmstloanpayin where statusid=1 order by recordid"))
                {
                    while (dr.Read())
                    {
                        loanconfigurationDTO obj = new loanconfigurationDTO();
                        obj.pLoanpayin = dr["laonpayin"].ToString();
                        lstLoanpayins.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanpayins;
        }

        public List<loanconfigurationDTO> getLoanInterestratetypes(string ConnectionString)
        {
            lstLoanIneterstratetypes = new List<loanconfigurationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select interestratetype from tblmstinterestratetypes where statusid = 1 order by recordid"))
                {
                    while (dr.Read())
                    {
                        loanconfigurationDTO obj = new loanconfigurationDTO();
                        obj.pInteresttype = dr["interestratetype"].ToString();
                        lstLoanIneterstratetypes.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanIneterstratetypes;
        }

        public List<loanconfigurationDTO> getLoanconfigurationDetails(string ConnectionString, Int64 loanid)
        {
            loanconfigurationdetails = new List<loanconfigurationDTO>();
            try
            {

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select loanconfigid, loantype, loanname, loancode, LOANIDCODE,tl.loanid, applicanttype, contacttype, loanpayin, minloanamount, maxloanamount, tenurefrom, tenureto, interesttype, rateofinterest, effectfromdate,effecttodate, statusname from tblmstloantypes tt join tblmstloans tl on tt.loantypeid = tl.loantypeid join TBLMSTLOANCONFIGURATION tc on tl.loanid = tc.loanid join tblmststatus ts on tc.statusid = ts.statusid where tc.loanid=" + loanid + " and upper(ts.statusname)='ACTIVE' order by loantype, loanname; "))

                {
                    while (dr.Read())
                    {
                        loanconfigurationDTO objamasterdetails = new loanconfigurationDTO();
                        objamasterdetails.pLoanconfigid = Convert.ToInt64(dr["loanconfigid"]);
                        objamasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objamasterdetails.pContacttype = dr["contacttype"].ToString();
                        objamasterdetails.pApplicanttype = dr["applicanttype"].ToString();
                        objamasterdetails.pLoanpayin = dr["loanpayin"].ToString();
                        objamasterdetails.pMinloanamount = Convert.ToDecimal(dr["minloanamount"]);
                        objamasterdetails.pMaxloanamount = Convert.ToDecimal(dr["maxloanamount"]);
                        objamasterdetails.pTenurefrom = Convert.ToInt64(dr["tenurefrom"]);
                        objamasterdetails.pTenureto = Convert.ToInt64(dr["tenureto"]);
                        objamasterdetails.pInteresttype = dr["interesttype"].ToString();
                        objamasterdetails.pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]);

                        if (dr["effectfromdate"] != DBNull.Value)
                        {
                            objamasterdetails.pEffectfromdate = Convert.ToDateTime(dr["effectfromdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objamasterdetails.pEffectfromdate = "";
                        }

                        if (dr["effecttodate"] != DBNull.Value)
                        {
                            objamasterdetails.pEffecttodate = Convert.ToDateTime(dr["effecttodate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            objamasterdetails.pEffecttodate = "";
                        }

                        if (dr["effectfromdate"] != DBNull.Value && dr["effecttodate"] != DBNull.Value)
                        {
                            objamasterdetails.ptypeofoperation = "NO EDIT";
                        }
                        else
                        {
                            objamasterdetails.ptypeofoperation = "OLD";
                        }
                        loanconfigurationdetails.Add(objamasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return loanconfigurationdetails;
        }

        public List<instalmentdatedetails> getinstalmentsdateslist(string ConnectionString, Int64 Loanid)
        {
            loaninstalmentdatedetails = new List<instalmentdatedetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from TBLMSTLOANINSTALLMENTDATECONFIG where loanid=" + Loanid + " "))
                {
                    while (dr.Read())
                    {
                        instalmentdatedetails objinstalmentsdateslist = new instalmentdatedetails();
                        // objinstalmentsdateslis.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        objinstalmentsdateslist.pTypeofInstalmentDay = dr["typeofinstallmentday"].ToString();
                        objinstalmentsdateslist.pDisbursefromday = Convert.ToInt32(dr["disbursefromday"]);
                        objinstalmentsdateslist.pDisbursetoday = Convert.ToInt32(dr["Disbursetoday"]);
                        objinstalmentsdateslist.pInstalmentdueday = Convert.ToInt32(dr["installmentdueday"]);
                        objinstalmentsdateslist.pInstalmentdueday = Convert.ToInt32(dr["installmentdueday"]);
                        loaninstalmentdatedetails.Add(objinstalmentsdateslist);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return loaninstalmentdatedetails;
        }

        public List<ReferralCommissioLoanDTO> getReferralCommissionlist(string ConnectionString, Int64 Loanid)
        {
            ReferralCommissioLoandetails = new List<ReferralCommissioLoanDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from TBLMSTLOANWISEREFERRALCOMMISIONCONFIG where loanid=" + Loanid + " "))
                {
                    while (dr.Read())
                    {
                        ReferralCommissioLoanDTO obj = new ReferralCommissioLoanDTO();
                        obj.pLoanid = Convert.ToInt32(dr["loanid"]);
                        obj.pIsreferralcomexist = Convert.ToBoolean(dr["isreferralcomexist"]);
                        obj.pCommissionpayouttype = Convert.ToString((dr["Commissionpayouttype"]));
                        obj.pCommissionpayout = Convert.ToDecimal(dr["Commissionpayout"]);
                        ReferralCommissioLoandetails.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ReferralCommissioLoandetails;
        }

        public List<PenaltyConfigurationDTO> getPenaltyConfigurationlist(string ConnectionString, Int64 Loanid)
        {
            PenaltyConfigurationLoandetails = new List<PenaltyConfigurationDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select * from tblmstpenaltyconfiguration where loanid=" + Loanid + " "))
                {
                    while (dr.Read())
                    {
                        PenaltyConfigurationDTO obj = new PenaltyConfigurationDTO();
                        obj.pLoanid = Convert.ToInt32(dr["loanid"]);
                        obj.ptypeofpenalinterest = Convert.ToString(dr["typeofpenalinterest"]);
                        obj.pisloanpayinmodeapplicable = Convert.ToBoolean(dr["isloanpayinmodeapplicable"]);
                        obj.pduepenaltytype = Convert.ToString(dr["duepenaltytype"]);
                        obj.pduepenaltyvalue = Convert.ToDecimal(dr["duepenaltyvalue"]);
                        obj.poverduepenaltytype = Convert.ToString(dr["overduepenaltytype"]);
                        obj.poverduepenaltyvalue = Convert.ToDecimal(dr["overduepenaltyvalue"]);
                        obj.ppenaltygraceperiod = Convert.ToInt32(dr["penaltygraceperiod"]);
                        obj.pPenaltygraceperiodtype = Convert.ToString(dr["penaltygraceperiodtype"]);


                        if (dr["effectfromdate"] != DBNull.Value)
                        {
                            obj.pEffectfromdate = Convert.ToDateTime(dr["effectfromdate"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            obj.pEffectfromdate = "";
                        }
                        //obj.pEffectfromdate = Convert.ToString(dr["effectfromdate"]);
                        PenaltyConfigurationLoandetails.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return PenaltyConfigurationLoandetails;
        }
        public List<LoansMasterDTO> getLoanMasterDetails(string ConnectionString)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            DocumentsMasterDAL daDocumentsMasterDAL = new DocumentsMasterDAL();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tt.loantypeid,loanid, loantype,loanname,loancode,companycode,branchcode,series,serieslength,loanidcode,statusname from tblmstloantypes tt join tblmstloans  tl on tt.loantypeid = tl.loantypeid join tblmststatus ts on tl.statusid = ts.statusid where tl.statusid=" + Convert.ToInt32(Status.Active) + " order by loantype, loanname;"))
                {
                    while (dr.Read())
                    {
                        LoansMasterDTO objamasterdetails = new LoansMasterDTO();
                        objamasterdetails.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        objamasterdetails.pCompanycode = dr["companycode"].ToString();
                        objamasterdetails.pBranchcode = dr["branchcode"].ToString();
                        objamasterdetails.pSeries = dr["series"].ToString();
                        objamasterdetails.pSerieslength = Convert.ToInt32(dr["serieslength"]);

                        objamasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objamasterdetails.pLoantype = dr["loantype"].ToString();
                        objamasterdetails.pLoanname = dr["loanname"].ToString();
                        objamasterdetails.pLoancode = dr["loancode"].ToString();
                        objamasterdetails.pLoanidcode = dr["loanidcode"].ToString();
                        objamasterdetails.pStatusname = dr["statusname"].ToString();
                        //objamasterdetails.loanconfigurationlist = getLoanconfigurationDetails(ConnectionString, objamasterdetails.pLoanid);
                        //objamasterdetails.instalmentdatedetailslist = getinstalmentsdateslist(ConnectionString, objamasterdetails.pLoanid);
                        //objamasterdetails.getidentificationdocumentsList = daDocumentsMasterDAL.Getdocumentidprofftypes(ConnectionString, objamasterdetails.pLoanid);
                        //objamasterdetails.ReferralCommissioLoanList = getReferralCommissionlist(ConnectionString, objamasterdetails.pLoanid);
                        lstLoanMasterdetails.Add(objamasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanMasterdetails;
        }

        public List<LoansMasterDTO> getLoanMasterDetails(string ConnectionString, Int64 LOANID)
        {
            lstLoanMasterdetails = new List<LoansMasterDTO>();
            DocumentsMasterDAL daDocumentsMasterDAL = new DocumentsMasterDAL();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select tt.loantypeid,loanid, loantype,loanname,loancode,companycode,branchcode,series,serieslength,loanidcode,statusname from tblmstloantypes tt join tblmstloans  tl on tt.loantypeid = tl.loantypeid join tblmststatus ts on tl.statusid = ts.statusid WHERE tl.LOANID=" + LOANID + " order by loantype, loanname;"))
                {
                    while (dr.Read())
                    {
                        LoansMasterDTO objamasterdetails = new LoansMasterDTO();
                        objamasterdetails.pLoantypeid = Convert.ToInt32(dr["loantypeid"]);
                        objamasterdetails.pCompanycode = dr["companycode"].ToString();
                        objamasterdetails.pBranchcode = dr["branchcode"].ToString();
                        objamasterdetails.pSeries = dr["series"].ToString();
                        objamasterdetails.pSerieslength = Convert.ToInt32(dr["serieslength"]);

                        objamasterdetails.pLoanid = Convert.ToInt64(dr["loanid"]);
                        objamasterdetails.pLoantype = dr["loantype"].ToString();
                        objamasterdetails.pLoanname = dr["loanname"].ToString();
                        objamasterdetails.pLoancode = dr["loancode"].ToString();
                        objamasterdetails.pLoanidcode = dr["loanidcode"].ToString();
                        objamasterdetails.pStatusname = dr["statusname"].ToString();
                        objamasterdetails.loanconfigurationlist = getLoanconfigurationDetails(ConnectionString, objamasterdetails.pLoanid);
                        objamasterdetails.instalmentdatedetailslist = getinstalmentsdateslist(ConnectionString, objamasterdetails.pLoanid);
                        objamasterdetails.getidentificationdocumentsList = daDocumentsMasterDAL.Getdocumentidprofftypes(ConnectionString, objamasterdetails.pLoanid);
                        objamasterdetails.ReferralCommissioLoanList = getReferralCommissionlist(ConnectionString, objamasterdetails.pLoanid);
                        objamasterdetails.PenaltyConfigurationList = getPenaltyConfigurationlist(ConnectionString, objamasterdetails.pLoanid);
                        lstLoanMasterdetails.Add(objamasterdetails);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstLoanMasterdetails;
        }

        public int checkInsertLoanNameandCodeDuplicates(string checkparamtype, string loanname, string loancode, Int64 loanid, string connectionstring)
        {
            int count = 0;
            try
            {
                if (checkparamtype.ToUpper() == "LOANNAME")
                {
                    if (string.IsNullOrEmpty(loanid.ToString()) || loanid == 0)
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstloans where upper(loanname)='" + ManageQuote(loanname.Trim().ToUpper()) + "'"));
                    }
                    else
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstloans where upper(loanname)='" + ManageQuote(loanname.Trim().ToUpper()) + "' and loanid!=" + loanid + ";"));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(loanid.ToString()) || loanid == 0)
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstloans where upper(loancode)='" + ManageQuote(loancode.Trim().ToUpper()) + "'"));
                    }
                    else
                    {
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstloans where upper(loancode)='" + ManageQuote(loancode.Trim().ToUpper()) + "' and loanid!=" + loanid + ";"));
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return count;

        }

        public bool saveLoanMaster(LoansMasterDTO loanmasterlist, string connectionstring)
        {
            bool isSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            long loanid;
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                loanid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstloans(loantypeid,loanname,loancode,companycode,branchcode,series,serieslength,loanidcode,statusid,createdby,createddate)values(" + loanmasterlist.pLoantypeid + ",'" + ManageQuote(loanmasterlist.pLoanname.Trim()) + "','" + ManageQuote(loanmasterlist.pLoancode.Trim()) + "','" + ManageQuote(loanmasterlist.pCompanycode) + "','" + ManageQuote(loanmasterlist.pBranchcode) + "','" + ManageQuote(loanmasterlist.pSeries.Trim()) + "'," + loanmasterlist.pSerieslength + ",'" + ManageQuote(loanmasterlist.pLoanidcode.Trim()) + "'," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp) returning loanid"));

                if (loanmasterlist.loanconfigurationlist != null)
                {
                    for (int i = 0; i < loanmasterlist.loanconfigurationlist.Count; i++)
                    {

                        if (string.IsNullOrEmpty(loanmasterlist.loanconfigurationlist[i].pTenurefrom.ToString()))
                        {
                            loanmasterlist.loanconfigurationlist[i].pTenurefrom = 0;
                        }
                        if (string.IsNullOrEmpty(loanmasterlist.loanconfigurationlist[i].pTenureto.ToString()))
                        {
                            loanmasterlist.loanconfigurationlist[i].pTenureto = 0;
                        }
                        sbinsert.Append("insert into tblmstloanconfiguration(loantypeid, loanid, contacttype, applicanttype, loanpayin, minloanamount, maxloanamount, tenurefrom, tenureto, interesttype, rateofinterest, effectfromdate,isamountrangeapplicable,istenurerangeapplicable,  statusid, createdby, createddate)values(" + loanmasterlist.pLoantypeid + "," + loanid + ",'" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pContacttype) + "','" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pApplicanttype) + "','" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pLoanpayin) + "',coalesce(" + loanmasterlist.loanconfigurationlist[i].pMinloanamount + ",0),coalesce(" + loanmasterlist.loanconfigurationlist[i].pMaxloanamount + ",0),coalesce(" + loanmasterlist.loanconfigurationlist[i].pTenurefrom + ",0),coalesce(" + loanmasterlist.loanconfigurationlist[i].pTenureto + ",0),'" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pInteresttype) + "'," + loanmasterlist.loanconfigurationlist[i].pRateofinterest + ",'" + FormatDate(loanmasterlist.loanconfigurationlist[i].pEffectfromdate.ToString()) + "'," + loanmasterlist.loanconfigurationlist[i].pIsamountrangeapplicable + "," + loanmasterlist.loanconfigurationlist[i].pIstenurerangeapplicable + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");
                    }
                }

                if (loanmasterlist.instalmentdatedetailslist != null)
                {
                    for (int i = 0; i < loanmasterlist.instalmentdatedetailslist.Count; i++)
                    {
                        sbinsert.Append("insert into tblmstloaninstallmentdateconfig (loantypeid,loanid,typeofinstallmentday,disbursefromday,disbursetoday,installmentdueday,statusid,createdby,createddate)values('" + loanmasterlist.pLoantypeid + "'," + loanid + ",'" + ManageQuote(loanmasterlist.instalmentdatedetailslist[i].pTypeofInstalmentDay) + "',coalesce(" + loanmasterlist.instalmentdatedetailslist[i].pDisbursefromday + ",0),coalesce(" + loanmasterlist.instalmentdatedetailslist[i].pDisbursetoday + ",0)," + loanmasterlist.instalmentdatedetailslist[i].pInstalmentdueday + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");

                    }
                }
                if (loanmasterlist.identificationdocumentsList != null)
                {
                    for (int i = 0; i < loanmasterlist.identificationdocumentsList.Count; i++)
                    {
                        if (loanmasterlist.identificationdocumentsList[i].pDocumentRequired == true || loanmasterlist.identificationdocumentsList[i].pDocumentMandatory == true)
                        {
                            sbinsert.Append("insert into tblmstloanwisedocumentproofs(loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory,statusid,createdby,createddate) values(" + loanmasterlist.pLoantypeid + "," + loanid + ",'" + ManageQuote(loanmasterlist.identificationdocumentsList[i].pContactType) + "'," + loanmasterlist.identificationdocumentsList[i].pDocumentId + "," + loanmasterlist.identificationdocumentsList[i].pDocumentgroupId + ",'" + loanmasterlist.identificationdocumentsList[i].pDocumentRequired + "','" + loanmasterlist.identificationdocumentsList[i].pDocumentMandatory + "'," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");
                        }

                    }
                }
                if (loanmasterlist.ReferralCommissioLoanList != null)
                {
                    for (int i = 0; i < loanmasterlist.ReferralCommissioLoanList.Count; i++)
                    {
                        sbinsert.Append("insert into tblmstloanwisereferralcommisionconfig(loantypeid,loanid,ISREFERRALCOMEXIST,COMMISSIONPAYOUTTYPE,COMMISSIONPAYOUT,statusid,createdby,createddate)values('" + loanmasterlist.pLoantypeid + "'," + loanid + "," + loanmasterlist.ReferralCommissioLoanList[i].pIsreferralcomexist + ",'" + ManageQuote(loanmasterlist.ReferralCommissioLoanList[i].pCommissionpayouttype) + "'," + loanmasterlist.ReferralCommissioLoanList[i].pCommissionpayout + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");

                    }
                }
                if (loanmasterlist.PenaltyConfigurationList != null)
                {
                    for (int i = 0; i < loanmasterlist.PenaltyConfigurationList.Count; i++)
                    {

                        if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate))
                        {

                            loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate = "null";
                        }
                        else
                        {

                            loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate = "'" + FormatDate(loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate) + "'";
                        }
                        if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue.ToString()))
                        {
                            loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue = 0;
                        }
                        if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue.ToString()))
                        {
                            loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue = 0;
                        }
                        if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod.ToString()))
                        {
                            loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod = 0;
                        }

                        sbinsert.Append("insert into tblmstpenaltyconfiguration(loantypeid, loanid, typeofpenalinterest, isloanpayinmodeapplicable,duepenaltytype, duepenaltyvalue, overduepenaltytype, overduepenaltyvalue,penaltygraceperiod, penaltygraceperiodtype, effectfromdate, statusid, createdby, createddate)VALUES ('" + loanmasterlist.pLoantypeid + "'," + loanid + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].ptypeofpenalinterest) + "', " + loanmasterlist.PenaltyConfigurationList[i].pisloanpayinmodeapplicable + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].pduepenaltytype) + "'," + loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].poverduepenaltytype) + "'," + loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue + ", " + loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].pPenaltygraceperiodtype) + "', " + loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");

                    }
                }
                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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

        public bool updateLoanMaster(LoansMasterDTO loanmasterlist, string connectionstring)
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
                if (loanmasterlist != null)
                {
                    sbupdate.Append("UPDATE tblmstloans set loanname ='" + ManageQuote(loanmasterlist.pLoanname) + "',loancode='" + ManageQuote(loanmasterlist.pLoancode) + "',statusid=" + getStatusid(loanmasterlist.pStatusname, connectionstring) + ",modifiedby=" + loanmasterlist.pCreatedby + ",modifieddate=current_timestamp where loanid=" + loanmasterlist.pLoanid + "; ");
                }

                if (loanmasterlist.loanconfigurationlist != null)
                {

                    for (int i = 0; i < loanmasterlist.loanconfigurationlist.Count; i++)
                    {
                        if (loanmasterlist.loanconfigurationlist[i].ptypeofoperation == "UPDATE")
                        {
                            sbupdate.Append("UPDATE tblmstloanconfiguration set applicanttype ='" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pApplicanttype) + "',loanpayin='" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pLoanpayin) + "',minloanamount=" + loanmasterlist.loanconfigurationlist[i].pMinloanamount + ",maxloanamount=" + loanmasterlist.loanconfigurationlist[i].pMaxloanamount + ",tenurefrom=" + loanmasterlist.loanconfigurationlist[i].pTenurefrom + ",tenureto=" + loanmasterlist.loanconfigurationlist[i].pTenureto + ",interesttype='" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pInteresttype) + "',rateofinterest=" + loanmasterlist.loanconfigurationlist[i].pRateofinterest + ",effectfromdate='" + FormatDate(Convert.ToString(loanmasterlist.loanconfigurationlist[i].pEffectfromdate)) + "',effecttodate='" + FormatDate(Convert.ToString(loanmasterlist.loanconfigurationlist[i].pEffecttodate)) + "',modifiedby=" + loanmasterlist.pCreatedby + ",modifieddate=current_timestamp,isamountrangeapplicable= " + loanmasterlist.loanconfigurationlist[i].pIsamountrangeapplicable + ",istenurerangeapplicable= " + loanmasterlist.loanconfigurationlist[i].pIstenurerangeapplicable + " where loanconfigid=" + loanmasterlist.loanconfigurationlist[i].pLoanconfigid + " and loanid=" + loanmasterlist.pLoanid + ";");
                        }
                        else if (loanmasterlist.loanconfigurationlist[i].ptypeofoperation == "CREATE")
                        {
                            sbupdate.Append("insert into tblmstloanconfiguration(loantypeid, loanid, contacttype, applicanttype, loanpayin, minloanamount, maxloanamount, tenurefrom, tenureto, interesttype, rateofinterest,isamountrangeapplicable,istenurerangeapplicable, effectfromdate,  statusid, createdby, createddate)values(" + loanmasterlist.pLoantypeid + "," + loanmasterlist.pLoanid + ",'" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pContacttype) + "','" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pApplicanttype) + "','" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pLoanpayin) + "'," + loanmasterlist.loanconfigurationlist[i].pMinloanamount + "," + loanmasterlist.loanconfigurationlist[i].pMaxloanamount + "," + loanmasterlist.loanconfigurationlist[i].pTenurefrom + "," + loanmasterlist.loanconfigurationlist[i].pTenureto + ",'" + ManageQuote(loanmasterlist.loanconfigurationlist[i].pInteresttype) + "'," + loanmasterlist.loanconfigurationlist[i].pRateofinterest + "," + loanmasterlist.loanconfigurationlist[i].pIsamountrangeapplicable + "," + loanmasterlist.loanconfigurationlist[i].pIstenurerangeapplicable + ",'" + FormatDate(Convert.ToString(loanmasterlist.loanconfigurationlist[i].pEffectfromdate)) + "'," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");
                        }
                        else if (loanmasterlist.loanconfigurationlist[i].ptypeofoperation == "DELETE")
                        {
                            sbupdate.Append("update tblmstloanconfiguration set statusid=" + getStatusid("In-Active", connectionstring) + ", modifiedby= " + loanmasterlist.pModifiedby + ", modifieddate = current_timestamp where loanid = " + loanmasterlist.pLoanid + " and loanconfigid = " + loanmasterlist.loanconfigurationlist[i].pLoanconfigid + "; ");
                        }

                    }
                }
                if (loanmasterlist.instalmentdatedetailslist != null)
                {
                    if (loanmasterlist.instalmentdatedetailslist.Count > 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from TBLMSTLOANINSTALLMENTDATECONFIG where loanid=" + loanmasterlist.pLoanid + "");
                        for (var i = 0; i < loanmasterlist.instalmentdatedetailslist.Count; i++)
                        {
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "insert into TBLMSTLOANINSTALLMENTDATECONFIG(loantypeid,loanid,typeofinstallmentday,disbursefromday,disbursetoday,installmentdueday,createdby,statusid,createddate)values('" + loanmasterlist.pLoantypeid + "'," + loanmasterlist.pLoanid + ",'" + ManageQuote(loanmasterlist.instalmentdatedetailslist[i].pTypeofInstalmentDay) + "'," + loanmasterlist.instalmentdatedetailslist[i].pDisbursefromday + "," + loanmasterlist.instalmentdatedetailslist[i].pDisbursetoday + "," + loanmasterlist.instalmentdatedetailslist[i].pInstalmentdueday + "," + loanmasterlist.pCreatedby + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + ",current_timestamp);");
                        }
                    }
                }
                if (loanmasterlist.identificationdocumentsList != null)
                {
                    if (loanmasterlist.identificationdocumentsList.Count > 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from  tblmstloanwisedocumentproofs where loanid=" + loanmasterlist.pLoanid + "");

                        for (int i = 0; i < loanmasterlist.identificationdocumentsList.Count; i++)
                        {
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "insert into tblmstloanwisedocumentproofs(loantypeid,loanid,contacttype,documentid,documentgroupid,isdocumentrequired,isdocumentmandatory,statusid,createdby,createddate) values(" + loanmasterlist.pLoantypeid + "," + loanmasterlist.pLoanid + ",'" + ManageQuote(loanmasterlist.identificationdocumentsList[i].pContactType) + "'," + loanmasterlist.identificationdocumentsList[i].pDocumentId + "," + loanmasterlist.identificationdocumentsList[i].pDocumentgroupId + ",'" + loanmasterlist.identificationdocumentsList[i].pDocumentRequired + "','" + loanmasterlist.identificationdocumentsList[i].pDocumentMandatory + "'," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");
                        }
                    }
                }

                if (loanmasterlist.ReferralCommissioLoanList != null)
                {
                    if (loanmasterlist.ReferralCommissioLoanList.Count > 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from  tblmstloanwisereferralcommisionconfig where loanid=" + loanmasterlist.pLoanid + "");

                        for (int i = 0; i < loanmasterlist.ReferralCommissioLoanList.Count; i++)
                        {
                            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "insert into tblmstloanwisereferralcommisionconfig(loantypeid,loanid,ISREFERRALCOMEXIST,COMMISSIONPAYOUTTYPE,COMMISSIONPAYOUT,statusid,createdby,createddate)values('" + loanmasterlist.pLoantypeid + "'," + loanmasterlist.pLoanid + "," + loanmasterlist.ReferralCommissioLoanList[i].pIsreferralcomexist + ",'" + ManageQuote(loanmasterlist.ReferralCommissioLoanList[i].pCommissionpayouttype) + "'," + loanmasterlist.ReferralCommissioLoanList[i].pCommissionpayout + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");

                        }
                    }
                }
                if (loanmasterlist.PenaltyConfigurationList != null)
                {
                    if (loanmasterlist.PenaltyConfigurationList.Count > 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "delete from  tblmstpenaltyconfiguration where loanid=" + loanmasterlist.pLoanid + "");
                        for (int i = 0; i < loanmasterlist.PenaltyConfigurationList.Count; i++)
                        {

                            if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate))
                            {

                                loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate = "null";
                            }
                            else
                            {

                                loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate = "'" + FormatDate(loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate) + "'";
                            }
                            if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue.ToString()))
                            {
                                loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue = 0;
                            }
                            if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue.ToString()))
                            {
                                loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue = 0;
                            }
                            if (string.IsNullOrEmpty(loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod.ToString()))
                            {
                                loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod = 0;
                            }

                            sbupdate.Append("insert into tblmstpenaltyconfiguration(loantypeid, loanid, typeofpenalinterest, isloanpayinmodeapplicable,duepenaltytype, duepenaltyvalue, overduepenaltytype, overduepenaltyvalue,penaltygraceperiod, penaltygraceperiodtype, effectfromdate, statusid, createdby, createddate)VALUES ('" + loanmasterlist.pLoantypeid + "'," + loanmasterlist.pLoanid + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].ptypeofpenalinterest) + "', " + loanmasterlist.PenaltyConfigurationList[i].pisloanpayinmodeapplicable + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].pduepenaltytype) + "'," + loanmasterlist.PenaltyConfigurationList[i].pduepenaltyvalue + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].poverduepenaltytype) + "'," + loanmasterlist.PenaltyConfigurationList[i].poverduepenaltyvalue + ", " + loanmasterlist.PenaltyConfigurationList[i].ppenaltygraceperiod + ", '" + ManageQuote(loanmasterlist.PenaltyConfigurationList[i].pPenaltygraceperiodtype) + "', " + loanmasterlist.PenaltyConfigurationList[i].pEffectfromdate + "," + getStatusid(loanmasterlist.pStatusname, connectionstring) + "," + loanmasterlist.pCreatedby + ",current_timestamp);");

                        }
                    }
                }
                if (!string.IsNullOrEmpty(sbupdate.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbupdate.ToString());
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

        public bool DeleteLoanMaster(Int64 loanid, int modifiedby, string connectionstring)
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

                sbupdate.Append("UPDATE tblmstloans set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
                sbupdate.Append("UPDATE tblmstloanconfiguration set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
                sbupdate.Append("UPDATE tblmstloaninstallmentdateconfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
                sbupdate.Append("UPDATE tblmstloanwisedocumentproofs set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
                sbupdate.Append("UPDATE tblmstloanwisereferralcommisionconfig set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
                sbupdate.Append("UPDATE tblmstpenaltyconfiguration set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + modifiedby + ",modifieddate=current_timestamp where loanid=" + loanid + "; ");
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







