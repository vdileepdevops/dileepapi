using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Settings;
using Npgsql;
using HelperManager;
using System.Data;
using System.Threading.Tasks;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaInfrastructure.Accounting;

namespace FinstaRepository.DataAccess.Banking.Transactions
{
    public class MaturityPaymentDAL : SettingsDAL, IMaturityPayment
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        #region maturity Bonds
        public List<MemberTypesDTO> GetMemberTypes(string ConnectionString)
        {
            List<MemberTypesDTO> lstMembertypes = new List<MemberTypesDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select membertypeid,membertype from tblmstmembertypes where statusid=1 order by membertype;"))
                {
                    while (dr.Read())
                    {
                        MemberTypesDTO objMemberTypes = new MemberTypesDTO();
                        objMemberTypes.pMembertypeId = dr["membertypeid"];
                        objMemberTypes.pMemberType = dr["membertype"];
                        lstMembertypes.Add(objMemberTypes);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMembertypes;
        }
        public List<MemberIdsDTO> GetMemberIds(string Membertype, string Connectionstring)
        {
            List<MemberIdsDTO> lstMemberIdsDTO = new List<MemberIdsDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select contactreferenceid,(membercode||'-'||membername)membertypeids from tblmstmembers where membertype='" + Membertype + "';"))
                {
                    while (dr.Read())
                    {
                        MemberIdsDTO objMemberIdsDTO = new MemberIdsDTO();
                        objMemberIdsDTO.pContactreferenceid = dr["contactreferenceid"];
                        objMemberIdsDTO.pMembertypeids = dr["membertypeids"];
                        lstMemberIdsDTO.Add(objMemberIdsDTO);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMemberIdsDTO;
        }
        public List<DepositIdsDTO> GetDepositIds(string BranchName, string MaturityType, long Schemeid, string Connectionstring)
        {
            List<DepositIdsDTO> lstDepositIdsDTO = new List<DepositIdsDTO>();
            string Query = string.Empty;
            try
            {
                if (MaturityType == "Maturity")
                {
                    //Query = "select fdaccountid,fdaccountno,membercode,membername,businessentitycontactno,memberid from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + " and  maturitydate < current_date and fdaccountid not in(select trans_type_id from maturity_bonds where status=true) and tf.statusid=" + Convert.ToInt32(Status.Active) + " order by fdaccountno;";
                    Query = "select fdaccountid,fdaccountno,membercode,membername,mobileno as businessentitycontactno,fdconfigid,chitbranchname,memberid from vwfdtransaction_details where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + " and  maturitydate < current_date and fdaccountid not in(select trans_type_id from maturity_bonds where status=true) and balanceamount<=0  order by fdaccountno";
                }
                else
                {
                    //Query = "select fdaccountid,fdaccountno,membercode,membername,businessentitycontactno from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + "  order by fdaccountno;";
                    //Query = "select fdaccountid,fdaccountno,membercode,membername,businessentitycontactno,fdconfigid,chitbranchname,memberid from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + " AND Fdaccountid not in(select fdaccountid from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + " and  maturitydate < current_date order by fdaccountno) and  fdaccountid not in(select trans_type_id from maturity_bonds where status=true) and tf.statusid=" + Convert.ToInt32(Status.Active) + "  order by fdaccountno";
                    Query = "select fdaccountid,fdaccountno,membercode,membername,mobileno as businessentitycontactno,fdconfigid,chitbranchname,memberid from vwfdtransaction_details where chitbranchname = '" + BranchName + "' and fdconfigid=" + Schemeid + " and Fdaccountid not in(select fdaccountid from vwfdtransaction_details where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + " and  maturitydate < current_date order by fdaccountno) and fdaccountid not in(select trans_type_id from maturity_bonds where status=true) and balanceamount<=0   and depositdate<=depositdate+cast((select prematuretyageperiod::TEXT||' '||prematuretyageperiodtype from  tblmstfixeddepositLoansConfig where fdconfigid=" + Schemeid + ") as INTERVAL) order by fdaccountno";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        DepositIdsDTO objDepositIdsDTO = new DepositIdsDTO();
                        objDepositIdsDTO.pFdaccountid = dr["fdaccountid"];
                        objDepositIdsDTO.pFdaccountno = dr["fdaccountno"];
                        objDepositIdsDTO.pMemberid = dr["memberid"];
                        objDepositIdsDTO.pMembercode = dr["membercode"];
                        objDepositIdsDTO.pMembername = dr["membername"];
                        objDepositIdsDTO.pContactno = dr["businessentitycontactno"];
                        lstDepositIdsDTO.Add(objDepositIdsDTO);
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstDepositIdsDTO;
        }
        public PreMaturedetailsDTO GetPreMaturityDetails(string FDAccountno, string Date, string type, string Connectionstring)
        {
            PreMaturedetailsDTO _LstPreMaturedetails = new PreMaturedetailsDTO();
            try
            {
                NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select FN_PREMATURE_DETAILS('" + FDAccountno + "', '" + FormatDate(Date) + "','"+type+"')");

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select numid,vchbranch,vchmember," +
                    "bondamount,to_char(bonddate, 'dd/Mon/yyyy')bonddate,to_char(maturedate, 'dd/Mon/yyyy')maturedate," +
                    "to_char(prematuredate, 'dd/Mon/yyyy')prematuredate,vchperiod,numpredays,periodmode,rateofinterest,actualrateinterest," +
                    "interestpaidamt,calcinterestamt,promotorsalary,subtotal,netpay,numactualtds,suspenceamount,numaftertdsintamt,paidpromotorsalary from temptabprematuredetails;"))
                {
                    if (dr.Read())
                    {
                        _LstPreMaturedetails.Pnumid = dr["numid"];
                        _LstPreMaturedetails.pBranch = dr["vchbranch"];
                        _LstPreMaturedetails.pMember = dr["vchmember"];
                        _LstPreMaturedetails.pBondamount = dr["bondamount"];
                        _LstPreMaturedetails.pBondDate = dr["bonddate"];
                        _LstPreMaturedetails.pMatureDate = dr["maturedate"];
                        _LstPreMaturedetails.pPrematureDate = dr["prematuredate"];
                        _LstPreMaturedetails.pPeriod = dr["vchperiod"];
                        _LstPreMaturedetails.pPredays = dr["numpredays"];
                        _LstPreMaturedetails.pPeriodmode = dr["periodmode"];
                        _LstPreMaturedetails.pRateofinterest = dr["rateofinterest"];
                        _LstPreMaturedetails.pActualrateinterest = dr["actualrateinterest"];
                        _LstPreMaturedetails.pInterestpaidamt = dr["interestpaidamt"];
                        _LstPreMaturedetails.pCalcinterestamt = dr["calcinterestamt"];
                        _LstPreMaturedetails.pPromotorsalary = dr["promotorsalary"];
                        _LstPreMaturedetails.pSubtotal = dr["subtotal"];
                        _LstPreMaturedetails.pNetpay = dr["netpay"];
                        _LstPreMaturedetails.pTdsPayble = dr["numactualtds"];
                        _LstPreMaturedetails.pIntrestPayble = dr["numaftertdsintamt"];
                        _LstPreMaturedetails.pCommissionPaid = dr["paidpromotorsalary"];
                        _LstPreMaturedetails.pSuspenceAmount = dr["suspenceamount"];



                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return _LstPreMaturedetails;
        }
        public bool SaveMaturitybond(MaturitybondsSave _MaturitybondsSaveDTO, string connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbinsert = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();
            string qry = string.Empty;
            try
            {

                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (Convert.ToString(_MaturitybondsSaveDTO.ptypeofoperation) == "CREATE")
                {
                    sbinsert.Append("insert into maturity_bonds( member_id,trans_type,trans_type_id,trans_date,mature_amount,net_payable,pre_interest_rate," +
                        "interest_payble,agent_commssion_value,agent_commssion_Payable,damages,interes_paid,Commission_paid,maturity_type," +
                        " narration,payment_status,status,TDS_AMOUNT,SUSPENCE_AMOUNT)values(" + _MaturitybondsSaveDTO.pMemberid + ",'" + _MaturitybondsSaveDTO.pTranstype + "'," + _MaturitybondsSaveDTO.pTranstypeid + "," +
                        "'" + FormatDate(_MaturitybondsSaveDTO.pTransdate.ToString()) + "'," + Convert.ToDecimal(Convert.ToString
                        (_MaturitybondsSaveDTO.pMatureamount)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pNetpayble)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pPreinterestrate)) + "," +
                        "" + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pInterestpayble)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pAgentcommssionvalue)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pAgentcommssionPayable)) + "," +
                        "" + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pDamages)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pInterestpaid)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.pCommissionpaid)) + "," +
                        "'" + _MaturitybondsSaveDTO.pMaturityType + "','" + _MaturitybondsSaveDTO.pNarration + "','N','true'," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.ptdsamount)) + "," + Convert.ToDecimal(Convert.ToString(_MaturitybondsSaveDTO.psuspenceamount)) + ");");

                    sbupdate.Append("update tbltransfdcreation set accountstatus='P' where fdaccountid=" + _MaturitybondsSaveDTO.pTranstypeid + ";");

                }


                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString() + sbupdate.ToString());
                }

                trans.Commit();
                IsSaved = true;
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
            return IsSaved;
        }
        public List<MaturityBondViewDTO> GetMaturityBondView(string Connectionstring)
        {
            List<MaturityBondViewDTO> lstMaturityBondViewDTO = new List<MaturityBondViewDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select tm.membername,t.trans_type,tf.fdaccountno,to_char(t.trans_date, 'dd/Mon/yyyy')trans_date,depositamount,(tenor||' '||tenortype)tenure,fdname,t.mature_amount,t.interest_payble,net_payable,maturity_type from maturity_bonds t join tblmstmembers tm on t.member_id=tm.memberid join tbltransfdcreation tf on tf.fdaccountid=t.trans_type_id where status=true order by t.trans_date desc";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityBondViewDTO objMaturityBondViewDTO = new MaturityBondViewDTO();
                        objMaturityBondViewDTO.pMembername = dr["membername"];
                        objMaturityBondViewDTO.pTranstype = dr["trans_type"];
                        objMaturityBondViewDTO.pFdaccountno = dr["fdaccountno"];
                        objMaturityBondViewDTO.pTransdate = dr["trans_date"];
                        objMaturityBondViewDTO.pMatureamount = dr["mature_amount"];
                        objMaturityBondViewDTO.pInterestpayble = dr["interest_payble"];
                        objMaturityBondViewDTO.pNetpayable = dr["net_payable"];
                        objMaturityBondViewDTO.pMaturitytype = dr["maturity_type"];
                        objMaturityBondViewDTO.pDepositamount = dr["depositamount"];
                        objMaturityBondViewDTO.pTenure = dr["tenure"];
                        objMaturityBondViewDTO.pFdname = dr["fdname"];

                        lstMaturityBondViewDTO.Add(objMaturityBondViewDTO);
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityBondViewDTO;
        }
        #endregion

        #region maturity Payments
        public async Task<List<SchemeTypeDTO>> GetSchemeType(string BranchName, string MaturityType, string ConnectionString)
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            string Query = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    if (MaturityType == "Maturity")
                    {
                        //Query = "select distinct tc.fdname,tc.fdconfigid from tbltransfdcreation tf join tblmstfixeddepositConfig tc on tf.fdconfigid=tc.fdconfigid where chitbranchname = '" + BranchName + "'  and  maturitydate < current_date and tf.statusid=" + Convert.ToInt32(Status.Active) + " order by fdname;";
                        Query = "select distinct  fdname,fdconfigid from vwfdtransaction_details where chitbranchname= '" + BranchName + "' and  maturitydate < current_date and balanceamount<=0 order by fdname;";
                    }
                    else
                    {
                        //Query = "select fdaccountid,fdaccountno,membercode,membername,businessentitycontactno from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + "  order by fdaccountno;";
                        //Query = "select distinct tc.fdname,tc.fdconfigid from tbltransfdcreation tf join tblmstfixeddepositConfig tc on tf.fdconfigid=tc.fdconfigid where chitbranchname = '" + BranchName + "'  AND Fdaccountid not in(select fdaccountid from tbltransfdcreation tf join tblmstfixeddepositConfig tc on tf.fdconfigid=tc.fdconfigid where chitbranchname = '" + BranchName + "' and  maturitydate < current_date ) and tf.statusid=" + Convert.ToInt32(Status.Active) + "  order by fdname";

                        Query = "select distinct fdname,fdconfigid from vwfdtransaction_details where chitbranchname = '" + BranchName + "'  AND Fdaccountid not in(select fdaccountid from vwfdtransaction_details where chitbranchname = '" + BranchName + "' and  maturitydate < current_date ) and balanceamount<=0  order by fdname";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            SchemeTypeDTO objSchemetypes = new SchemeTypeDTO();
                            objSchemetypes.pSchemeid = dr["fdconfigid"];
                            objSchemetypes.pSchemeName = dr["fdname"];
                            lstSchemetype.Add(objSchemetypes);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstSchemetype;
        }
        public async Task<List<ChitBranchDetails>> GetMaturityBranchDetails(string MaturityType, string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    string Query = string.Empty;
                    if (MaturityType == "Maturity")
                    {
                        //Query = "select distinct tc.code, tc.branchname, tc.vchregion, tc.vchzone from tbltransfdcreation tf join tabbranchcodes tc on tc.code=tf.chitbranchid::numeric(9,0) where  maturitydate < current_date and tf.statusid=1  order by tc.branchname";
                        Query = "select distinct chitbranchid as code,chitbranchname as branchname from vwfdtransaction_details where  maturitydate < current_date and balanceamount<=0 order by branchname";
                    }
                    else
                    {
                        //Query = "select fdaccountid,fdaccountno,membercode,membername,businessentitycontactno from tbltransfdcreation tf join  tblmstcontact tc on tf.contactid=tc.contactid where chitbranchname = '" + BranchName + "' and  fdconfigid=" + Schemeid + "  order by fdaccountno;";
                        //Query = "select distinct tc.code, tc.branchname, tc.vchregion, tc.vchzone from tbltransfdcreation tf join tabbranchcodes tc  on  tc.code=tf.chitbranchid::numeric(9,0) where  Fdaccountid not in(select fdaccountid from tbltransfdcreation where maturitydate < current_date )  and tf.statusid=1 order by tc.branchname";
                        Query = "select distinct chitbranchid as code,chitbranchname as branchname from vwfdtransaction_details where  Fdaccountid not in(select fdaccountid from vwfdtransaction_details where maturitydate < current_date ) and balanceamount<=0 order by branchname";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["code"]),
                                pBranchname = dr["branchname"],
                                // pVchRegion = dr["vchregion"],
                                // pVchZone = dr["vchzone"]
                            };
                            _ChitBranchDetailsList.Add(_ChitBranchDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _ChitBranchDetailsList;
        }
        public async Task<List<MaturityMembersDTO>> GetMaturityMembers(string PaymentType, string ConnectionString)
        {
            List<MaturityMembersDTO> lstMembers = new List<MaturityMembersDTO>();
            string query = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    if (PaymentType == "Payment")
                    {
                        //query = "select distinct memberid,membercode,membername,businessentitycontactno as contactno,tm.contactid,tm.contactreferenceid from tblmstmembers tm join maturity_bonds tb on tm.memberid=tb.member_id join tblmstcontact tc on tc.contactid=tm.contactid and tb.status=true and tb.payment_status='N'";
                        query = "select distinct memberid,membercode,membername, contactno,contactid,contactreferenceid from (select distinct memberid,membercode,membername,businessentitycontactno as contactno,tm.contactid,tm.contactreferenceid,(coalesce(tp.out_statnding,tb.net_payable)- coalesce(tp.paid_amount,0))pending_Amount from tblmstmembers tm join maturity_bonds tb on tm.memberid=tb.member_id left join (select coalesce(sum(paid_amount),0)paid_amount,trans_type_id,out_statnding from maturity_payments group by trans_type_id,out_statnding)tp on tp.trans_type_id=tb.trans_type_id  join tblmstcontact tc on tc.contactid=tm.contactid and tb.status=true)as tbl where pending_Amount>0 ";
                    }
                    else
                    {
                        //query = "select distinct memberid,membercode,membername,businessentitycontactno as contactno,tm.contactid,tm.contactreferenceid from tblmstmembers tm join maturity_bonds tb on tm.memberid=tb.member_id join tblmstcontact tc on tc.contactid=tm.contactid where tb.status=true and tb.payment_status='N' and tb.maturity_type <>'Pre-Maturity'";
                        //query = "select * from (select distinct memberid,membercode,membername,businessentitycontactno as contactno,tm.contactid,tm.contactreferenceid,(coalesce(tp.out_statnding,tb.net_payable)- coalesce(tp.paid_amount,0))pending_Amount from tblmstmembers tm join maturity_bonds tb on tm.memberid=tb.member_id left join (select coalesce(sum(paid_amount),0)paid_amount,trans_type_id,out_statnding from maturity_payments group by trans_type_id,out_statnding)tp on tp.trans_type_id=tb.trans_type_id  join tblmstcontact tc on tc.contactid=tm.contactid where tb.status=true  and tb.maturity_type <>'Pre-Maturity')as tbl where pending_Amount>0";
                        query = "select distinct memberid,membercode,membername, contactno,contactid,contactreferenceid from (select distinct memberid,membercode,membername,businessentitycontactno as contactno,tm.contactid,tm.contactreferenceid,(coalesce(tp.out_statnding,tb.net_payable)- coalesce(tp.paid_amount,0))pending_Amount from tblmstmembers tm join maturity_bonds tb on tm.memberid=tb.member_id left join (select coalesce(sum(paid_amount),0)paid_amount,trans_type_id,out_statnding from maturity_payments group by trans_type_id,out_statnding)tp on tp.trans_type_id=tb.trans_type_id  join tblmstcontact tc on tc.contactid=tm.contactid where tb.status=true  and tb.maturity_type <>'Pre-Maturity')as tbl where pending_Amount>0";
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            MaturityMembersDTO objMembers = new MaturityMembersDTO();
                            objMembers.pMemberid = dr["memberid"];
                            objMembers.pMembercode = dr["membercode"];
                            objMembers.pMembername = dr["membername"];
                            objMembers.pMobileno = dr["contactno"];
                            objMembers.pContactid = dr["contactid"];
                            objMembers.pContactrefid = dr["contactreferenceid"];
                            lstMembers.Add(objMembers);
                        }


                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstMembers;
        }
        public List<MaturityBondstList> GetMaturityFdDetails(string PaymentType, long Memberid, string Date, string Connectionstring)
        {
            List<MaturityBondstList> lstMaturityBondstList = new List<MaturityBondstList>();
            string Query = string.Empty;
            try
            {
                if (PaymentType == "Payment")
                {
                    NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select fn_maturity_payment_details(" + Memberid + ",'" + FormatDate(Date) + "')");
                    //Query = "select tf.fdaccountid,tf.fdaccountno,trans_date,depositamount,mature_amount,pre_interest_rate,interest_payble,agent_commssion_value,agent_commssion_payable,interes_paid,commission_paid,tf.accountid,tb.net_payable,maturity_type,damages from tbltransfdcreation tf join maturity_bonds tb on tf.fdaccountid=tb.trans_type_id  where tb.member_id=" + Memberid + " and  tb.payment_status='N'";
                    Query = "select * from temptabmaturitypaymentdetails;";
                }
                else
                {
                    //Query = "select tf.fdaccountid,tf.fdaccountno,trans_date,depositamount,mature_amount,pre_interest_rate,interest_payble,agent_commssion_value,agent_commssion_payable,interes_paid,commission_paid,tf.accountid,tb.net_payable,maturity_type,damages,0 as latefeedays,0 as interest_amount_perday,0 as latefeeAmount,0 as payamount from tbltransfdcreation tf join maturity_bonds tb on tf.fdaccountid=tb.trans_type_id  where tb.member_id=" + Memberid + " and tb.maturity_type<> 'Pre-Maturity' and  tb.payment_status='N'";
                    //Query = "select * from(select tf.fdaccountid, tf.fdaccountno, trans_date, depositamount, mature_amount, pre_interest_rate, interest_payble, agent_commssion_value, agent_commssion_payable, interes_paid, commission_paid, tf.accountid, tb.net_payable, maturity_type, damages,0 as latefeedays,0 as interest_amount_perday,0 as latefeeAmount,0 as payamount,coalesce(tp.paid_amount,0)paid_amount,coalesce(tp.out_statnding,tb.net_payable)- coalesce(tp.paid_amount,0)as pending_Amount from tbltransfdcreation tf join maturity_bonds tb on tf.fdaccountid = tb.trans_type_id left join (select coalesce(sum(paid_amount),0)paid_amount,trans_type_id,out_statnding from maturity_payments group by trans_type_id,out_statnding)tp on tp.trans_type_id=tb.trans_type_id  where tb.member_id = " + Memberid + " and tb.maturity_type <> 'Pre-Maturity')as tbl where pending_Amount> 0";
                    NPGSqlHelper.ExecuteNonQuery(Connectionstring, CommandType.Text, "select fn_maturity_Renewal_details(" + Memberid + ",'" + FormatDate(Date) + "')");

                    Query = "select * from temptabmaturitypaymentdetails;";
                }

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityBondstList objMaturityBondsDTO = new MaturityBondstList();
                        objMaturityBondsDTO.pFdaccountid = dr["fdaccountid"];
                        objMaturityBondsDTO.pFdaccountno = dr["fdaccountno"];
                        objMaturityBondsDTO.pTransdate = dr["trans_date"];
                        objMaturityBondsDTO.pMatureAmount = dr["mature_amount"];
                        objMaturityBondsDTO.pPreinterestrate = dr["pre_interest_rate"];
                        objMaturityBondsDTO.pInterestpayble = dr["interest_payble"];
                        objMaturityBondsDTO.pAgentcommssionvalue = dr["agent_commssion_value"];
                        objMaturityBondsDTO.pAgentcommssionpayable = dr["agent_commssion_payable"];
                        objMaturityBondsDTO.pInterespaid = dr["interes_paid"];
                        objMaturityBondsDTO.pCommissionpaid = dr["commission_paid"];
                        objMaturityBondsDTO.pNetPayable = dr["net_payable"];
                        objMaturityBondsDTO.pAccountno = dr["accountid"];
                        objMaturityBondsDTO.pMaturityType = dr["maturity_type"];
                        objMaturityBondsDTO.pDamages = dr["damages"];
                        objMaturityBondsDTO.pDepositamount = dr["depositamount"];
                        objMaturityBondsDTO.pLatefeedays = dr["latefeedays"];
                        objMaturityBondsDTO.pInterestAmountPerday = dr["interest_amount_perday"];
                        objMaturityBondsDTO.pLateFeeAmount = dr["latefeeAmount"];
                        objMaturityBondsDTO.pPayamount = dr["payamount"];
                        objMaturityBondsDTO.pPaid_Amount = dr["paid_amount"];
                        objMaturityBondsDTO.pPending_Amount = dr["pending_Amount"];
                        lstMaturityBondstList.Add(objMaturityBondsDTO);
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityBondstList;
        }
        public bool SaveMaturityPayment(MaturityPaymentSaveDTO ObjMaturityPaymentDTO, string ConnectionString, out string OUTPaymentId)
        {
            bool Issaved = false;
            StringBuilder sbInsert = new StringBuilder();
            StringBuilder sbupdate = new StringBuilder();
            bool IsAccountSaved = false;
            try
            {
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (SavePaymentVoucher(ObjMaturityPaymentDTO, trans, out OUTPaymentId))
                {
                    IsAccountSaved = true;
                }
                else
                {
                    trans.Rollback();
                    return IsAccountSaved;
                }
                string Paymentid = OUTPaymentId;
                for (int i = 0; i < ObjMaturityPaymentDTO.MaturityPaymentsList.Count; i++)
                {

                    int voucherid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select recordid from tbltranspaymentvoucher where paymentid ='" + Paymentid + "'"));
                    sbInsert.Append("INSERT INTO maturity_payments(member_id,trans_type,trans_type_id,payment_date,out_statnding,paid_amount,voucher_id,payment_type,narration,status,latefee)VALUES" +
                        "(" + ObjMaturityPaymentDTO.pMemberid + ",'Fd'," + ObjMaturityPaymentDTO.MaturityPaymentsList[i].pTransTypeid + ",'" + FormatDate(ObjMaturityPaymentDTO.pMaturitypaymentdate) + "'," +
                        "" + ObjMaturityPaymentDTO.MaturityPaymentsList[i].pOutstandingAmount + "," +
                        "" + ObjMaturityPaymentDTO.MaturityPaymentsList[i].pPaidAmount + "," + voucherid + ",'" + ObjMaturityPaymentDTO.pPaymentType + "','" + ObjMaturityPaymentDTO.pnarration + "'," + ObjMaturityPaymentDTO.pStatus + "," + ObjMaturityPaymentDTO.MaturityPaymentsList[i].pLateFeeAmount + ");");
                    sbupdate.Append("update maturity_bonds set payment_status='P' where trans_type_id=" + ObjMaturityPaymentDTO.MaturityPaymentsList[i].pTransTypeid + ";");
                }


                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString() + sbupdate.ToString());
                }

                trans.Commit();
                Issaved = true;
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
            return Issaved;
        }
        public bool SavePaymentVoucher(MaturityPaymentSaveDTO ObjMaturityPaymentDTO, NpgsqlTransaction trans, out string PaymentId)
        {
            bool IsSaved = false;
            StringBuilder sbQuery = new StringBuilder();
            AccountingTransactionsDAL Accontstrans = new AccountingTransactionsDAL();
            try
            {
                PaymentVoucherDTO ObjPaymentVoucher = new PaymentVoucherDTO();
                ObjPaymentVoucher.ppaymentid = "";
                ObjPaymentVoucher.ppaymentdate = ObjMaturityPaymentDTO.pMaturitypaymentdate;
                ObjPaymentVoucher.ptotalpaidamount = ObjMaturityPaymentDTO.ptotalpaidamount;
                ObjPaymentVoucher.pnarration = ObjMaturityPaymentDTO.pnarration;
                ObjPaymentVoucher.pmodofPayment = ObjMaturityPaymentDTO.pmodofpayment;
                ObjPaymentVoucher.ptypeofoperation = ObjMaturityPaymentDTO.ptypeofoperation;
                ObjPaymentVoucher.pCreatedby = ObjMaturityPaymentDTO.pCreatedby;
                ObjPaymentVoucher.pbankid = ObjMaturityPaymentDTO.pbankid;
                ObjPaymentVoucher.pbankname = ObjMaturityPaymentDTO.pbankname;
                ObjPaymentVoucher.ptypeofpayment = ObjMaturityPaymentDTO.ptypeofpayment;
                ObjPaymentVoucher.pbranchname = ObjMaturityPaymentDTO.pbranchname;
                ObjPaymentVoucher.pChequenumber = ObjMaturityPaymentDTO.pChequenumber;
                ObjPaymentVoucher.ptranstype = ObjMaturityPaymentDTO.ptranstype;
                ObjPaymentVoucher.pCardNumber = ObjMaturityPaymentDTO.pCardNumber;
                ObjPaymentVoucher.pUpiid = ObjMaturityPaymentDTO.pUpiid;
                ObjPaymentVoucher.pUpiname = ObjMaturityPaymentDTO.pUpiname;


                List<PaymentsDTO> ppaymentslist = new List<PaymentsDTO>();
                for (int i = 0; i < ObjMaturityPaymentDTO.MaturityPaymentsList.Count; i++)
                {
                    PaymentsDTO objPayments = new PaymentsDTO();
                    objPayments.ppartyid = ObjMaturityPaymentDTO.pMemberid;
                    objPayments.ppartyname = ObjMaturityPaymentDTO.pMembername;
                    objPayments.ppartyreferenceid = "";
                    objPayments.psubledgerid = ObjMaturityPaymentDTO.MaturityPaymentsList[i].pAccountno;
                    objPayments.pledgername = "FIXED DEPOSIT";
                    objPayments.pamount = ObjMaturityPaymentDTO.MaturityPaymentsList[i].pPaidAmount;
                    objPayments.pCreatedby = ObjMaturityPaymentDTO.pCreatedby;
                    objPayments.ptypeofoperation = ObjMaturityPaymentDTO.ptypeofoperation;
                    ppaymentslist.Add(objPayments);
                }
                ObjPaymentVoucher.ppaymentslist = ppaymentslist;
                if (Accontstrans.SavePaymentVoucher_ALL(ObjPaymentVoucher, trans, "SAVE", out PaymentId))
                {
                    IsSaved = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return IsSaved;
        }
        public List<MaturityPaymentDetailsViewDTO> GetMaturityPaymentDetailsView(string Connectionstring)
        {
            List<MaturityPaymentDetailsViewDTO> lstMaturityPaymentDetailsViewDTO = new List<MaturityPaymentDetailsViewDTO>();
            string Query = string.Empty;
            try
            {
                Query = "select distinct tm.membername,tm.membercode,tf.fdaccountno,to_char(tf.depositdate, 'dd/Mon/yyyy')depositdate,depositamount,(tenor||' '||tenortype)tenure,tf.maturityamount,tf.interestpayout,to_char(tp.payment_date,'dd/Mon/yyyy')maturity_payment_date,tp.paid_amount,payment_type from maturity_payments tp join tblmstmembers tm on tm.memberid=tp.member_id join tbltransfdcreation tf on tf.fdaccountid=tp.trans_type_id";


                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        MaturityPaymentDetailsViewDTO objMaturityPaymentDetailsViewDTO = new MaturityPaymentDetailsViewDTO();
                        objMaturityPaymentDetailsViewDTO.pMembername = dr["membername"];
                        objMaturityPaymentDetailsViewDTO.pMembercode = dr["membercode"];
                        objMaturityPaymentDetailsViewDTO.pFdaccountno = dr["fdaccountno"];
                        objMaturityPaymentDetailsViewDTO.pMaturitypaymentdate = dr["maturity_payment_date"];
                        objMaturityPaymentDetailsViewDTO.pDepositdate = dr["depositdate"];
                        objMaturityPaymentDetailsViewDTO.pDepositamount = dr["depositamount"];
                        objMaturityPaymentDetailsViewDTO.pTenure = dr["tenure"];
                        objMaturityPaymentDetailsViewDTO.pMaturityamount = dr["maturityamount"];
                        objMaturityPaymentDetailsViewDTO.pInterestpayout = dr["interestpayout"];
                        objMaturityPaymentDetailsViewDTO.pPaymentType = dr["payment_type"];
                        objMaturityPaymentDetailsViewDTO.pPaidAmount = dr["paid_amount"];


                        lstMaturityPaymentDetailsViewDTO.Add(objMaturityPaymentDetailsViewDTO);
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstMaturityPaymentDetailsViewDTO;
        }
        #endregion

        #region Renewal
        public async Task<FdTransactionDataEdit> GetFdTransactionDetails(string FdAccountNos, string ConnectionString)
        {
            FdTransactionDataEdit _FdTransactionDataEdit = new FdTransactionDataEdit();
            await Task.Run(() =>
            {
                try
                {
                    _FdTransactionDataEdit.FdMembersandContactDetailsList = GetJointMembersListofFdInEdit(FdAccountNos, ConnectionString);
                    _FdTransactionDataEdit.FDMemberNomineeDetailsList = GetNomineesListofFdInEdit(FdAccountNos, ConnectionString);
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return _FdTransactionDataEdit;
        }
        public List<FdMembersandContactDetails> GetJointMembersListofFdInEdit(string FdAccountNos, string ConnectionString)
        {
            var _FdMemberJointDetailsList = new List<FdMembersandContactDetails>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT distinct memberid,membercode,membername,contactid,contacttype,contactreferenceid from tbltransfdjointdetails  where  fdaccountno in(" + FdAccountNos + ") and statusid = " + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        FdMembersandContactDetails _FdMemberJointDetails = new FdMembersandContactDetails
                        {

                            pMemberId = Convert.ToInt64(dr["memberid"]),
                            pMemberCode = dr["membercode"],
                            pMemberName = dr["membername"],
                            pContactid = Convert.ToInt64(dr["contactid"]),
                            pContacttype = dr["contacttype"],
                            pContactrefid = dr["contactreferenceid"],
                            pTypeofOperation = "CREATE"
                        };
                        _FdMemberJointDetailsList.Add(_FdMemberJointDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FdMemberJointDetailsList;
        }
        public List<FDMemberNomineeDetails> GetNomineesListofFdInEdit(string FdAccountNos, string ConnectionString)
        {
            var _FdMemberNomineeDetailsList = new List<FDMemberNomineeDetails>();
            try
            {
                string MemberCode = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select distinct membercode from vwfdtransaction_details where   fdaccountno in(" + FdAccountNos + ")").ToString();
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select recordid,vchapplicationid,contactid,contactreferenceid,nomineename,relationship,dateofbirth,contactno,idprooftype,idproofname,referencenumber, docidproofpath, isprimarynominee, applicantype,coalesce(percentage,0) as percentage,statusid from tabapplicationpersonalnomineedetails where vchapplicationid in('" + MemberCode + "')  and statusid=" + Convert.ToInt32(Status.Active) + ";"))
                {
                    while (dr.Read())
                    {
                        var _FdTransNomineeDetails = new FDMemberNomineeDetails
                        {
                            // precordid = Convert.ToInt64(dr["recordid"]),
                            pnomineename = Convert.ToString(dr["nomineename"]),
                            prelationship = Convert.ToString(dr["relationship"]),
                            pcontactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                            pMemberrefcode = Convert.ToString(dr["vchapplicationid"]),
                            pcontactno = Convert.ToString(dr["contactno"]),
                            pidproofname = Convert.ToString(dr["idproofname"]),
                            pdocidproofpath = Convert.ToString(dr["docidproofpath"]),
                            preferencenumber = Convert.ToString(dr["referencenumber"]),
                            pisprimarynominee = Convert.ToBoolean(dr["isprimarynominee"]),
                            pStatus = Convert.ToInt32(dr["statusid"]) == 1 ? true : false,
                            pdateofbirth = dr["dateofbirth"] == DBNull.Value ? null : Convert.ToDateTime(dr["dateofbirth"]).ToString("dd/MM/yyyy"),
                            pAge = dr["dateofbirth"] != DBNull.Value ? CalculateAgeCorrect(Convert.ToDateTime(dr["dateofbirth"])) : 0,
                            ptypeofoperation = "CREATE",
                            pidprooftype = Convert.ToString(dr["idprooftype"]),
                            pcontactid = Convert.ToInt64(dr["contactid"]),
                            pPercentage = Convert.ToDecimal(dr["percentage"])
                        };
                        _FdMemberNomineeDetailsList.Add(_FdTransNomineeDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _FdMemberNomineeDetailsList;
        }
        public List<LienEntryViewDTO> GetLienDetails(string FdAccountNo, string ConnectionString)
        {
            List<LienEntryViewDTO> lstfdview = new List<LienEntryViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select lienid,to_char(liendate, 'dd/Mon/yyyy')liendate,td.membername,to_char(td.depositdate,'dd/Mon/yyyy')depositdate,td.depositamount,tl.membercode,tl.fdaccountno,companybranch,lienadjuestto,lienamount,receipttype from vwfdtransaction_details td join tbltranslienentry tl on td.fdaccountno=tl.fdaccountno where  statusid =" + Convert.ToInt32(Status.Active) + " and lienstatus='N' and tl.fdaccountno='" + FdAccountNo + "' order by lienid desc;"))
                {
                    while (dr.Read())
                    {
                        LienEntryViewDTO objfdview = new LienEntryViewDTO();

                        objfdview.pLienid = Convert.ToInt64(dr["lienid"]);
                        objfdview.pLiendate = Convert.ToString(dr["liendate"]);
                        objfdview.pMembercode = Convert.ToString(dr["membercode"]);
                        objfdview.pFdaccountno = Convert.ToString(dr["fdaccountno"]);
                        objfdview.pCompanybranch = Convert.ToString(dr["companybranch"]);
                        objfdview.pLienadjuestto = Convert.ToString(dr["lienadjuestto"]);
                        objfdview.pLienamount = Convert.ToInt64(dr["lienamount"]);
                        objfdview.pReceipttype = Convert.ToString(dr["receipttype"]);
                        objfdview.pMembername = Convert.ToString(dr["membername"]);
                        objfdview.pDepositdate = Convert.ToString(dr["depositdate"]);
                        objfdview.pDepositamount = Convert.ToInt64(dr["depositamount"]);



                        lstfdview.Add(objfdview);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstfdview;
        }
        public string SaveMaturityRenewal(MaturityRenewalSaveDTO _MaturityRenewalSaveDTO, string ConnectionString, out long pFdAccountId)
        {
            try
            {
                StringBuilder sbInsert = new StringBuilder();
                StringBuilder sbupdate = new StringBuilder();
                con = new NpgsqlConnection(ConnectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                // Next Id-Generation of Member Reference Id

                if (string.IsNullOrEmpty(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate))
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate = "null";
                }
                else
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate = "'" + FormatDate(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate) + "'";
                }
                if (string.IsNullOrEmpty(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate))
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate = "null";
                }
                else
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate = "'" + FormatDate(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate) + "'";
                }
                if (string.IsNullOrEmpty(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate))
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate = "null";
                }
                else
                {
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate = "'" + FormatDate(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate) + "'";
                }
                if (Convert.ToString(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTypeofOperation) == "CREATE")
                {
                    if (string.IsNullOrEmpty(Convert.ToString(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo)) || Convert.ToString(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo) == "0")
                    {
                        _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo = NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT FN_GENERATENEXTID('FIXED DEPOSIT','" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdname + "'," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate + ")").ToString();
                    }
                    // Fd Transaction Save 1st and 2nd Tab
                    _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tbltransfdcreation(fdaccountno,transdate, membertypeid, membertype, memberid, applicanttype, membercode, membername, contactid,contacttype,contactreferenceid, fdconfigid, fdname, tenortype, tenor, depositamount,interesttype, compoundinteresttype, interestrate, maturityamount, interestpayable, depositdate, maturitydate, isinterestdepositinsaving,isautorenew, renewonlyprinciple, renewonlyprincipleinterest,bondprintstatus, accountstatus, tokenno,statusid, createdby,createddate,isjointapplicable,isreferralapplicable,chitbranchid,chitbranchname,fdcalculationmode,interestpayout,isinterestdepositinbank,squareyard,caltype) VALUES ('" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo + "', " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMembertypeId + ", '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMemberType + "'," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMemberId + ", '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pApplicantType + "', '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMemberCode + "', '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMemberName + "', " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pContactid + ", '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pContacttype + "',  '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pContactrefid + "', " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdConfigId + ", '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdname + "', '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestTenureMode + "', " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestTenure + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositAmount + ",'" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestType + "', '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCompoundInterestType + "', " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestRate + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityAmount + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestAmount + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsinterestDepositinSaving + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsAutoRenew + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsRenewOnlyPrinciple + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsRenewOnlyPrincipleandInterest + ",'N','N', '" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTokenNo + "'," + Convert.ToInt32(Status.Active) + ", " + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCreatedby + ",current_timestamp," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsJointMembersapplicable + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsReferralsapplicable + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pChitbranchId + ",'" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pChitbranchname + "','" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdCalculationmode + "','" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestPayOut + "'," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsinterestDepositinBank + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pSquareyard + ",'" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCaltype + "') returning fdaccountid; "));
                }
                else if (Convert.ToString(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTypeofOperation) == "UPDATE")
                {
                    string strUpdate = "UPDATE tbltransfdcreation SET   tenortype ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestTenureMode + "', tenor =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestTenure + ", depositamount =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositAmount + ", interesttype ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestType + "', compoundinteresttype ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCompoundInterestType + "', interestrate =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestRate + ", maturityamount =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityAmount + ", interestpayable =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestAmount + ", depositdate =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pDepositDate + ",maturitydate =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMaturityDate + ", isinterestdepositinsaving =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsinterestDepositinSaving + ", isautorenew =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsAutoRenew + ", renewonlyprinciple =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsRenewOnlyPrinciple + ", renewonlyprincipleinterest =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsRenewOnlyPrincipleandInterest + ", isjointapplicable =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsJointMembersapplicable + ", isreferralapplicable =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsReferralsapplicable + ", bondprintstatus =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pBondPrintStatus + ", accountstatus ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pAccountStatus + "', tokenno ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTokenNo + "', statusid =" + Convert.ToInt32(Status.Active) + ",  modifiedby =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCreatedby + ", modifieddate =current_timestamp,chitbranchid=" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pChitbranchId + ",chitbranchname='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pChitbranchname + "',fdcalculationmode='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdCalculationmode + "',interestpayout='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pInterestPayOut + "',isinterestdepositinbank=" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pIsinterestDepositinBank + ",squareyard=" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pSquareyard + ",caltype='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pCaltype + "' WHERE fdaccountid =" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountId + " and fdaccountno ='" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo + "'; ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strUpdate);
                }
                pFdAccountId = _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountId;

                for (int i = 0; i < _MaturityRenewalSaveDTO.MaturityPaymentsList.Count; i++)
                {

                    // int voucherid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select recordid from tbltranspaymentvoucher where paymentid ='" + Paymentid + "'"));
                    sbInsert.Append("INSERT INTO public.maturity_payments(member_id,trans_type,trans_type_id,payment_date,out_statnding,paid_amount,voucher_id,payment_type,narration,renewal_trans_type_id,status,latefee)VALUES" +
                        "(" + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pMemberId + ",'Fd'," + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pTransTypeid + "," + _MaturityRenewalSaveDTO._FdMemberandSchemeSave.pTransDate + "," +
                        "" + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pOutstandingAmount + "," + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pPaidAmount + "," + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pJvid + ",'" + _MaturityRenewalSaveDTO.pPaymentType + "','" + _MaturityRenewalSaveDTO.pnarration + "'," + pFdAccountId + "," + _MaturityRenewalSaveDTO.pStatus + "," + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pLateFeeAmount + ");");
                    sbupdate.Append("update maturity_bonds set payment_status='R' where trans_type_id=" + _MaturityRenewalSaveDTO.MaturityPaymentsList[i].pTransTypeid + ";");
                }

                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString() + sbupdate.ToString());
                }
                // if (_MaturityRenewalSaveDTO.pPaymentType=='Renewal')
                //{
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "SELECT FN_FUN_MATURITY_RENEWALS_JV(" + pFdAccountId + ")");
                //}
                trans.Commit();
            }
            catch (Exception Ex)
            {
                trans.Rollback();
                throw Ex;
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
            return Convert.ToString(_MaturityRenewalSaveDTO._FdMemberandSchemeSave.pFdAccountNo);
        }
        #endregion


    }
}
