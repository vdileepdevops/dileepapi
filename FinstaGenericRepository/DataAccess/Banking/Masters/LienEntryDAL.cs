using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Masters;

namespace FinstaRepository.DataAccess.Banking.Masters
{
    public class LienEntryDAL : SettingsDAL, ILienEntry 
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;

        public async Task<List<GetfdraccountnoDTO>> Getfddetails(long Memberid,string chitbranchname,string type, string connectionstring)
        {
            var _Fdrdetails = new List<GetfdraccountnoDTO>();
            string query = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    if(type== "Edit")
                    {
                        query = "select * from (select td.fdaccountno,fdaccountid,membername,memberid,chitbranchname,COALESCE(depositamount,0) as depositamount,COALESCE(sum(lienamount),0)totalLienAmount,((depositamount-(depositamount*10/100))-COALESCE(sum(lienamount),0)) as Balance from vwfdtransaction_details td left join tbltranslienentry tl on td.fdaccountno=tl.fdaccountno and tl.lienstatus='N' and statusid=" + Convert.ToInt32(Status.Active) + "  where memberid = '" + Memberid + "' and chitbranchname='" + chitbranchname + "' and td.fdaccountid not in(select trans_type_id from maturity_bonds) group by td.fdaccountno,fdaccountid,membername,memberid,chitbranchname,depositamount)as tbl";
                    }
                    else
                    {
                        query = "select * from (select td.fdaccountno,fdaccountid,membername,memberid,chitbranchname,COALESCE(depositamount,0) as depositamount,COALESCE(sum(lienamount),0)totalLienAmount,((depositamount-(depositamount*10/100))-COALESCE(sum(lienamount),0)) as Balance from vwfdtransaction_details td left join tbltranslienentry tl on td.fdaccountno=tl.fdaccountno and tl.lienstatus='N' and statusid=" + Convert.ToInt32(Status.Active) + "  where memberid = '" + Memberid + "' and chitbranchname='" + chitbranchname + "' and td.fdaccountid not in(select trans_type_id from maturity_bonds) group by td.fdaccountno,fdaccountid,membername,memberid,chitbranchname,depositamount)as tbl where tbl.Balance>0";
                    }
                    NpgsqlDataReader dr;
                    //string qry = "SELECT fdaccountno,fdaccountid,membername,memberid,chitbranchname FROM tbltransfdcreation where memberid = '" + Memberid + "' and chitbranchname='" + chitbranchname + "' and statusid = " + Convert.ToInt32(Status.Active) + "";
                    using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, query))
                    {
                        while (dr.Read())
                        {
                            var _Fdrdetails1 = new GetfdraccountnoDTO
                            {

                                pFdaccountno = Convert.ToString(dr["fdaccountno"]),
                                pFdacctid = Convert.ToInt64(dr["fdaccountid"]),
                                pMembername = Convert.ToString(dr["membername"]),
                                pMemberid = Convert.ToInt64(dr["memberid"])

                            };
                            _Fdrdetails.Add(_Fdrdetails1);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _Fdrdetails;
        }
        public async Task<List<FiMemberContactDetails>> GetMemberDetails(string chitbranchname, string connectionstring)
        {
            var _FiMemberContactDetails = new List<FiMemberContactDetails>();
            await Task.Run(() =>
            {
                try
                {
                    NpgsqlDataReader dr;
                    // string qry = "select te.memberid,te.membercode, te.contactid, te.contacttype, te.contactreferenceid, te.membername, coalesce(te.membertype,'') as membertype,coalesce(te.membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, businessentitycontactno,te.contactreferenceid,te.statusid from tblmstmembers te join tblmstcontact tc on tc.contactid = te.contactid join tbltransfdcreation tf on tf.memberid=te.memberid where te.statusid=" + Convert.ToInt32(Status.Active) + "  and tf.statusid=" + Convert.ToInt32(Status.Active) + " and chitbranchname='" + chitbranchname + "' order by membername";
                    //string qry = "select *,(depositamount-paidamount)balance from (select te.memberid,te.membercode, te.contactid, te.contacttype, te.contactreferenceid, te.membername, coalesce(te.membertype,'') as membertype,coalesce(te.membertypeid, 0) as membertypeid, coalesce(memberstatus, '') as memberstatus, businessentitycontactno,te.contactreferenceid,te.statusid,tf.depositamount,(select sum(received_amount)-sum(case when tr.clearstatus='Y' then 0  when tr.clearstatus is null then 0 else totalreceivedamount end )paidamount from fd_receipt fr left join tbltransreceiptreference tr on fr.receipt_no=tr.receiptid where fr.fd_account_id=tf.fdaccountid) from tblmstmembers te join tblmstcontact tc on tc.contactid = te.contactid join tbltransfdcreation tf on tf.memberid = te.memberid where te.statusid = " + Convert.ToInt32(Status.Active) + "  and tf.statusid = " + Convert.ToInt32(Status.Active) + " and chitbranchname = '" + chitbranchname + "' order by membername) as d where(depositamount - paidamount) <= 0;";
                    string qry = "select distinct memberid,membercode,contactid,contacttype,contactreferenceid,membername,membertype,membertypeid,mobileno from vwfdtransaction_details where chitbranchname='"+ chitbranchname + "' and  balanceamount<=0 and fdaccountid not in(select trans_type_id from maturity_bonds) order by membername";
                    using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, qry))
                    
                    {
                        while (dr.Read())
                        {
                            var _FIMemberContactDTO = new FiMemberContactDetails
                            {
                                pMembertypeId = Convert.ToInt64(dr["memberid"]),
                                pContacttype = Convert.ToString(dr["contacttype"]),
                                pContactName = Convert.ToString(dr["membername"]),
                                pMemberType = Convert.ToString(dr["membertype"]),                             
                                pContactReferenceId = Convert.ToString(dr["contactreferenceid"]),
                                pContactId = Convert.ToInt64(dr["contactid"]),
                                ptypeofoperation = "OLD",
                                pMemberReferenceId = Convert.ToString(dr["membercode"]),
                                pContactNo = Convert.ToString(dr["mobileno"])
                               
                                
                            };
                            _FiMemberContactDetails.Add(_FIMemberContactDTO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
            return _FiMemberContactDetails;
        }


        public GetmemberfddetailsDTO Getmemberfddetails(long Memberid, string Fdraccountno, string connectionstring)
        {
            GetmemberfddetailsDTO _Memberfdrdata1 = new GetmemberfddetailsDTO();

            try
                {
                    NpgsqlDataReader dr;

                    using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "SELECT membername,depositamount,tenor,transdate,tf.fdaccountno,tf.tenortype,sum(COALESCE(lienamount,0))lienamount, ((depositamount-(depositamount*5/100))-COALESCE(sum(lienamount),0)) as Balance FROM tbltransfdcreation tf left join tbltranslienentry te on tf.fdaccountno=te.fdaccountno and te.statusid = " + Convert.ToInt32(Status.Active) + "  and lienstatus='N' where memberid = " + Memberid + " and tf.fdaccountno ='"+ Fdraccountno + "'  and tf.statusid = "+ Convert.ToInt32(Status.Active) + "  group by membername,depositamount,tenor,transdate,tf.fdaccountno,tf.tenortype"))
                    {
                        if  (dr.Read())
                        {
                            _Memberfdrdata1 = new GetmemberfddetailsDTO
                            {

                                pMembername =dr["membername"],
                                pDepositamount = dr["depositamount"],
                                pTenor = dr["tenor"], 
                                pTransdate = dr["transdate"],
                                pFdaccountno=dr["fdaccountno"],
                                pTenortype=dr["tenortype"],
                                pBalance=dr["Balance"]

                            };
                           
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
          
            return _Memberfdrdata1;
        }


        public bool SaveLienentry(LienEntryDTO _lienentryDTO, string connectionstring)
        {
            bool IsSaved = false;
            string qry = string.Empty;
            try
            {

                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (_lienentryDTO.ptypeofoperation == "CREATE") {
                     qry = "insert into tbltranslienentry(liendate,membercode,fdaccountno,lienamount,liencount,companyname,companybranch,lienadjuestto,receipttype,lienstatus,statusid,createdby,createddate)values('" + FormatDate(_lienentryDTO.pLiendate.ToString()) + "','" + _lienentryDTO.pMembercode + "','" + _lienentryDTO.pFdaccountno + "'," + _lienentryDTO.pLienamount + "," + _lienentryDTO.pLiencount + ",'" + _lienentryDTO.pCompanyname + "','" + _lienentryDTO.pCompanybranch + "','" + _lienentryDTO.pLienadjuestto + "','" + _lienentryDTO.pReceipttype + "','N'," + Convert.ToInt32(Status.Active) + ", " + _lienentryDTO.pCreatedby + ", current_timestamp);";
                }

                else
                {
                     qry = "Update tbltranslienentry set liendate = '" + FormatDate( _lienentryDTO.pLiendate.ToString()) + "',membercode = '" + _lienentryDTO.pMembercode + "',fdaccountno ='" + _lienentryDTO.pFdaccountno + "',lienamount =" + _lienentryDTO.pLienamount + ",liencount = " + _lienentryDTO.pLiencount + ",companyname = '" + _lienentryDTO.pCompanyname + "',companybranch = '" + _lienentryDTO.pCompanybranch + "',lienadjuestto = '" + _lienentryDTO.pLienadjuestto + "',receipttype = '" + _lienentryDTO.pReceipttype + "',modifiedby = '" + _lienentryDTO.pCreatedby + "',modifieddate= current_timestamp where lienid = " + _lienentryDTO.pLienid + ";";

                }

                if (!string.IsNullOrEmpty(qry))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, qry);
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

        public List<LienEntryViewDTO> Lienviewdata(string connectionstring)
        {
            List<LienEntryViewDTO> lstfdview = new List<LienEntryViewDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select lienid,to_char(liendate, 'dd/Mon/yyyy')liendate,td.membername,to_char(td.depositdate,'dd/Mon/yyyy')depositdate,td.depositamount,tl.membercode,tl.fdaccountno,companybranch,lienadjuestto,lienamount,receipttype from vwfdtransaction_details td join tbltranslienentry tl on td.fdaccountno=tl.fdaccountno where  statusid = "+ Convert.ToInt32(Status.Active) + " order by lienid desc; "))
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

        public List<LienEntryDTO> GetLiendata(string Fdraccountno,string connectionstring)
        {
            List<LienEntryDTO> lstfdview = new List<LienEntryDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "select lienid,to_char(liendate, 'dd/Mon/yyyy')liendate,membercode,fdaccountno,companybranch,lienadjuestto,lienamount,receipttype,lienstatus from tbltranslienentry where  statusid = " + Convert.ToInt32(Status.Active) + " and fdaccountno='"+ Fdraccountno + "' order by liendate desc;"))
                {
                    while (dr.Read())
                    {
                        LienEntryDTO objfdview = new LienEntryDTO();

                        objfdview.pLienid = Convert.ToInt64(dr["lienid"]);
                        objfdview.pLiendate =Convert.ToString(dr["liendate"]);
                        objfdview.pMembercode = Convert.ToString(dr["membercode"]);
                        objfdview.pFdaccountno = Convert.ToString(dr["fdaccountno"]);
                        objfdview.pCompanybranch = Convert.ToString(dr["companybranch"]);
                        objfdview.pLienadjuestto = Convert.ToString(dr["lienadjuestto"]);
                        objfdview.pLienamount = Convert.ToInt64(dr["lienamount"]);
                        objfdview.pReceipttype = Convert.ToString(dr["receipttype"]);
                        objfdview.pLienstatus = Convert.ToString(dr["lienstatus"]);


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
        public bool DeleteLienentry(long Lienid, string connectionstring)
        {
            bool IsSaved = false;
            StringBuilder sbInsert = new StringBuilder();
            try
            {

                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(Lienid.ToString()) && Lienid != 0)
                {
                    sbInsert.Append("update tbltranslienentry set statusid=" + Convert.ToInt32(Status.Inactive) + " where lienid=" + Lienid + ";");
                  
                }
                if (!string.IsNullOrEmpty(sbInsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbInsert.ToString());
                }

                trans.Commit();
                IsSaved = true;
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
            return IsSaved;
        }



        public LienEntryDetailsForEdit GetLienentryforEdit(long lienid, string connectionstring)
        {
            LienEntryDetailsForEdit _lienentrydataforedit = new LienEntryDetailsForEdit();

            try
            {
                NpgsqlDataReader dr;

                using (dr = NPGSqlHelper.ExecuteReader(connectionstring, CommandType.Text, "SELECT tl.lienid,tl.liendate,tm.memberid,tl.membercode,tf.fdaccountid,tl.fdaccountno,tl.lienamount,tl.companybranch as tobranch,tf.chitbranchname as branchname,tl.lienadjuestto FROM tbltranslienentry tl join tblmstmembers tm on tl.membercode=tm.membercode join tbltransfdcreation tf on tf.fdaccountno=tl.fdaccountno where lienid = " + lienid+" and tl.statusid = "+ Convert.ToInt32(Status.Active) + ""))
                {
                    if (dr.Read())
                    {
                        _lienentrydataforedit = new LienEntryDetailsForEdit
                        {
                            pLienid=dr["lienid"],
                            pLienDate = dr["liendate"],
                            pMemberId= dr["memberid"],
                            pMemberCode = dr["membercode"],
                            pFdAccountid = dr["fdaccountid"],
                            pFdaccountno = dr["fdaccountno"],
                            pLienamount = dr["lienamount"],
                            pTobranch= dr["tobranch"],
                            pBranchname=dr["branchname"],
                            pLienadjuestto = dr["lienadjuestto"],
                        };

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return _lienentrydataforedit;
        }

        public async Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring)
        {
            List<ChitBranchDetails> _ChitBranchDetailsList = new List<ChitBranchDetails>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select distinct chitbranchid,chitbranchname from vwfdtransaction_details where  balanceamount<=0 and fdaccountid not in(select trans_type_id from maturity_bonds) order by chitbranchname;"))
                    {
                        while (dr.Read())
                        {
                            var _ChitBranchDetails = new ChitBranchDetails
                            {
                                pBranchId = Convert.ToInt64(dr["chitbranchid"]),
                                pBranchname = dr["chitbranchname"]
                                
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

    }


        }
    

