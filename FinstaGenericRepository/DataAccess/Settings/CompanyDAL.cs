using System;
using System.Collections.Generic;
using System.Text;
using HelperManager;
using Npgsql;
using System.Data;
using FinstaRepository.Interfaces.Settings;
using FinstaInfrastructure.Settings;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Settings
{
    public class CompanyDAL : SettingsDAL, ICompany
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        public CompanyDTO CompanyDTOdetails { set; get; }

        public bool SaveCompanyDetails(CompanyDTO CompanyDTO, string connectionstring)
        {
            bool IsSave = false;

            StringBuilder saveCompanyDetails = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (CompanyDTO != null)
                {
                    if (string.IsNullOrEmpty(CompanyDTO.pnameofenterprise))
                    {
                        CompanyDTO.pnameofenterprise = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.penterprisecode))
                    {
                        CompanyDTO.penterprisecode = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.ppancard))
                    {
                        CompanyDTO.ppancard = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.ptypeofenterprise))
                    {
                        CompanyDTO.ptypeofenterprise = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pestablishmentdate))
                    {
                        CompanyDTO.pestablishmentdate = "null";
                    }
                    else
                    {
                        CompanyDTO.pestablishmentdate = "'" + FormatDate(CompanyDTO.pestablishmentdate) + "'";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pcommencementdate))
                    {
                        CompanyDTO.pcommencementdate = "null";
                    }
                    else
                    {
                        CompanyDTO.pcommencementdate = "'" + FormatDate(CompanyDTO.pcommencementdate) + "'";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pcinnumber))
                    {
                        CompanyDTO.pcinnumber = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pgstinnumber))
                    {
                        CompanyDTO.pgstinnumber = "";
                    }

                    if (string.IsNullOrEmpty(CompanyDTO.pcompanyumrn))
                    {
                        CompanyDTO.pcompanyumrn = "";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.ptransactionopeningdate))
                    {
                        CompanyDTO.ptransactionopeningdate = "null";
                    }
                    else
                    {
                        CompanyDTO.ptransactionopeningdate = "'" + FormatDate(CompanyDTO.ptransactionopeningdate) + "'";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pdatepickerenableenddate))
                    {
                        CompanyDTO.pdatepickerenableenddate = "null";
                    }
                    else
                    {
                        CompanyDTO.pdatepickerenableenddate = "'" + FormatDate(CompanyDTO.pdatepickerenableenddate) + "'";
                    }
                    if (string.IsNullOrEmpty(CompanyDTO.pdefaultpassword))
                    {
                        CompanyDTO.pdefaultpassword = "";
                    }
                    if (CompanyDTO.ptypeofoperation == "CREATE")
                    {
                        CompanyDTO.pCompanyId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "INSERT INTO tblmstcompany(nameofenterprise, enterprisecode, pancard, typeofenterprise,establishmentdate, commencementdate, cinnumber, gstinnumber,transactionlock, datepickerenablestatus, companyumrn,statusid, createdby, createddate,transactionopeningdate,defaultpassword,datepickerenableenddate) VALUES ('" + ManageQuote(CompanyDTO.pnameofenterprise) + "','" + ManageQuote(CompanyDTO.penterprisecode) + "','" + ManageQuote(CompanyDTO.ppancard) + "','" + ManageQuote(CompanyDTO.ptypeofenterprise) + "'," + (CompanyDTO.pestablishmentdate) + "," + (CompanyDTO.pcommencementdate) + ",'" + ManageQuote(CompanyDTO.pcinnumber) + "', '" + ManageQuote(CompanyDTO.pgstinnumber) + "','" + CompanyDTO.ptransactionlock + "','" + CompanyDTO.pdatepickerenablestatus + "','" + ManageQuote(CompanyDTO.pcompanyumrn) + "'," + Convert.ToInt32(Status.Active) + "," + CompanyDTO.pCreatedby + ", current_timestamp, " + (CompanyDTO.ptransactionopeningdate) + ", '" + ManageQuote(CompanyDTO.pdefaultpassword) + "', " + CompanyDTO.ptransactionopeningdate + ") returning companyid;"));
                    }
                    else
                    {
                        saveCompanyDetails.Append("UPDATE tblmstcompany SET nameofenterprise='" + ManageQuote(CompanyDTO.pnameofenterprise) + "',enterprisecode='" + ManageQuote(CompanyDTO.penterprisecode) + "', pancard='" + ManageQuote(CompanyDTO.ppancard) + "',   typeofenterprise='" + ManageQuote(CompanyDTO.ptypeofenterprise) + "',establishmentdate=" + (CompanyDTO.pestablishmentdate) + ",commencementdate=" + (CompanyDTO.pcommencementdate) + ",cinnumber='" + ManageQuote(CompanyDTO.pcinnumber) + "',gstinnumber='" + ManageQuote(CompanyDTO.pgstinnumber) + "',transactionlock='" + CompanyDTO.ptransactionlock + "',datepickerenablestatus='" + CompanyDTO.pdatepickerenablestatus + "',companyumrn='" + ManageQuote(CompanyDTO.pcompanyumrn) + "',modifiedby=" + CompanyDTO.pCreatedby + ",modifieddate=current_timestamp,transactionopeningdate=" + (CompanyDTO.ptransactionopeningdate) + ",defaultpassword='" + ManageQuote(CompanyDTO.pdefaultpassword) + "',datepickerenableenddate=" + (CompanyDTO.ptransactionopeningdate) + ";");
                    }
                    if (CompanyDTO.lstCompanyContactDTO.Count > 0)
                    {
                        for (int i = 0; i < CompanyDTO.lstCompanyContactDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyContactDTO[i].pcontactnumber.ToString()))
                            {
                                CompanyDTO.lstCompanyContactDTO[i].pcontactnumber = 0;
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyContactDTO[i].pemailid))
                            {
                                CompanyDTO.lstCompanyContactDTO[i].pemailid = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyContactDTO[i].ppriority))
                            {
                                CompanyDTO.lstCompanyContactDTO[i].ppriority = "";
                            }
                            if (CompanyDTO.lstCompanyContactDTO[i].ptypeofoperation == "CREATE")
                            {
                                saveCompanyDetails.Append("INSERT INTO tblmstcompanycontactdetails(companyid,contactnumber,emailid,priority,statusid,createdby,createddate)VALUES (" + CompanyDTO.pCompanyId + "," + CompanyDTO.lstCompanyContactDTO[i].pcontactnumber + ",'" + ManageQuote(CompanyDTO.lstCompanyContactDTO[i].pemailid) + "','" + ManageQuote(CompanyDTO.lstCompanyContactDTO[i].ppriority) + "'," + Convert.ToInt32(Status.Active) + "," + CompanyDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveCompanyDetails.Append("update tblmstcompanycontactdetails set contactnumber=" + CompanyDTO.lstCompanyContactDTO[i].pcontactnumber + ",emailid='" + ManageQuote(CompanyDTO.lstCompanyContactDTO[i].pemailid) + "',priority='" + ManageQuote(CompanyDTO.lstCompanyContactDTO[i].ppriority) + "',modifiedby=" + CompanyDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + CompanyDTO.lstCompanyContactDTO[i].pRecordId + "; ");
                            }
                        }

                    }
                    if (CompanyDTO.lstCompanyAddressDTO.Count > 0)
                    {

                        for (int i = 0; i < CompanyDTO.lstCompanyAddressDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pAddressType))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pAddressType = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pAddress1))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pAddress1 = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pAddress2))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pAddress2 = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pState))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pState = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pDistrict))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pDistrict = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pCity))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pCity = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pCountry))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pCountry = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyAddressDTO[i].pPriority))
                            {
                                CompanyDTO.lstCompanyAddressDTO[i].pPriority = "";
                            }
                            if (CompanyDTO.lstCompanyAddressDTO[i].ptypeofoperation == "CREATE")
                            {
                                saveCompanyDetails.Append("INSERT INTO tblmstcompanyaddressdetails(companyid, addresstype, address1, address2, state,stateid, district, districtid, city, country, countryid, pincode,priority, statusid, createdby, createddate)VALUES (" + CompanyDTO.pCompanyId + ",'" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddressType) + "','" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddress1) + "','" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddress2) + "','" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pState) + "'," + CompanyDTO.lstCompanyAddressDTO[i].pStateId + ",'" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pDistrict) + "'," + CompanyDTO.lstCompanyAddressDTO[i].pDistrictId + ",'" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pCity) + "','" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pCountry) + "'," + CompanyDTO.lstCompanyAddressDTO[i].pCountryId + "," + CompanyDTO.lstCompanyAddressDTO[i].pPinCode + ",'" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pPriority) + "'," + Convert.ToInt32(Status.Active) + "," + CompanyDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveCompanyDetails.Append("update tblmstcompanyaddressdetails set addresstype='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddressType) + "',address1='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddress1) + "',address2='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pAddress2) + "',state='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pState) + "',stateid=" + CompanyDTO.lstCompanyAddressDTO[i].pStateId + ",district='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pDistrict) + "',districtid=" + CompanyDTO.lstCompanyAddressDTO[i].pDistrictId + ",city='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pCity) + "',country='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pCountry) + "',countryid=" + CompanyDTO.lstCompanyAddressDTO[i].pCountryId + ",pincode=" + CompanyDTO.lstCompanyAddressDTO[i].pPinCode + ",priority='" + ManageQuote(CompanyDTO.lstCompanyAddressDTO[i].pPriority) + "',modifiedby=" + CompanyDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + CompanyDTO.lstCompanyAddressDTO[i].pRecordId + ";");
                            }
                        }
                    }
                    if (CompanyDTO.lstCompanyDocumentsDTO.Count > 0)
                    {
                        for (int i = 0; i < CompanyDTO.lstCompanyDocumentsDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPNAME))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPNAME = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTNAME))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTNAME = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILETYPE))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILETYPE = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILENAME))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILENAME = "";
                            }
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCREFERENCENO))
                            {
                                CompanyDTO.lstCompanyDocumentsDTO[i].pDOCREFERENCENO = "";
                            }

                            if (CompanyDTO.lstCompanyDocumentsDTO[i].ptypeofoperation == "CREATE")
                            {
                                saveCompanyDetails.Append("INSERT INTO tblmstcompanydocuments(companyid,documentid,documentgroupid,documentgroupname,documentname, docstorepath, docfiletype, docfilename, docreferenceno,docisdownloadable, statusid, createdby, createddate)VALUES (" + CompanyDTO.pCompanyId + "," + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTID + "," + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPID + ",'" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPNAME) + "','" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTNAME) + "','" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH) + "','" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILETYPE) + "','" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILENAME) + "','" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCREFERENCENO) + "','" + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCISDOWNLOADABLE + "'," + Convert.ToInt32(Status.Active) + "," + CompanyDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveCompanyDetails.Append("UPDATE tblmstcompanydocuments set documentid=" + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTID + ",documentgroupid=" + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPID + ",documentgroupname='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTGROUPNAME) + "',documentname='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCUMENTNAME) + "',docstorepath='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH) + "',docfiletype='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILETYPE) + "',docfilename='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCFILENAME) + "',docreferenceno='" + ManageQuote(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCREFERENCENO) + "',docisdownloadable='" + CompanyDTO.lstCompanyDocumentsDTO[i].pDOCISDOWNLOADABLE + "',modifiedby=" + CompanyDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + CompanyDTO.lstCompanyDocumentsDTO[i].pRecordId + ";");
                            }
                        }
                    }
                    if (CompanyDTO.lstCompanyPromotersDTO.Count > 0)
                    {
                        for (int i = 0; i < CompanyDTO.lstCompanyPromotersDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(CompanyDTO.lstCompanyPromotersDTO[i].pContactId.ToString()))
                            {
                                CompanyDTO.lstCompanyPromotersDTO[i].pContactId = 0;
                            }
                            if (CompanyDTO.lstCompanyPromotersDTO[i].ptypeofoperation == "CREATE")
                            {
                                saveCompanyDetails.Append("INSERT INTO tblmstcompanypromoters(companyid,contactid, statusid, createdby, createddate) VALUES (" + CompanyDTO.pCompanyId + "," + CompanyDTO.lstCompanyPromotersDTO[i].pContactId + "," + Convert.ToInt32(Status.Active) + "," + CompanyDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveCompanyDetails.Append("update tblmstcompanypromoters set contactid=" + CompanyDTO.lstCompanyPromotersDTO[i].pContactId + ",modifiedby=" + CompanyDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + CompanyDTO.lstCompanyPromotersDTO[i].pRecordId + ";");
                            }
                        }
                    }
                }
                if (Convert.ToString(saveCompanyDetails) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, saveCompanyDetails.ToString());
                    trans.Commit();
                    IsSave = true;
                }
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
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
            return IsSave;
        }

        public CompanyDTO getCompanyDetails(string ConnectionString)
        {
            try
            {
                CompanyDTOdetails = new CompanyDTO();
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select  companyid, nameofenterprise, enterprisecode, pancard, typeofenterprise,establishmentdate, commencementdate, cinnumber, gstinnumber,transactionlock, locktime, datepickerenablestatus, companyumrn,statusid, createdby, createddate, modifiedby, modifieddate, transactionopeningdate,       defaultpassword, datepickerenableenddate  FROM tblmstcompany; "))
                {
                    while (dr.Read())
                    {
                        CompanyDTOdetails = new CompanyDTO();
                        CompanyDTOdetails.pCompanyId = Convert.ToInt32(dr["companyid"]);
                        CompanyDTOdetails.pnameofenterprise = dr["nameofenterprise"].ToString();
                        CompanyDTOdetails.penterprisecode = dr["enterprisecode"].ToString();
                        CompanyDTOdetails.ppancard = dr["pancard"].ToString();
                        CompanyDTOdetails.ptypeofenterprise = Convert.ToString(dr["typeofenterprise"]);

                        CompanyDTOdetails.pestablishmentdate = dr["establishmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["establishmentdate"]).ToString("dd/MM/yyyy");
                        CompanyDTOdetails.pcommencementdate = dr["commencementdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["commencementdate"]).ToString("dd/MM/yyyy");
                        CompanyDTOdetails.pcinnumber = dr["cinnumber"].ToString();
                        CompanyDTOdetails.pgstinnumber = dr["gstinnumber"].ToString();
                        CompanyDTOdetails.ptransactionlock = Convert.ToBoolean(dr["transactionlock"]);
                        CompanyDTOdetails.plocktime = dr["locktime"] == DBNull.Value ? null : Convert.ToDateTime(dr["locktime"]).ToString("dd/MM/yyyy");
                        CompanyDTOdetails.pdatepickerenablestatus = Convert.ToBoolean(dr["datepickerenablestatus"]);
                        CompanyDTOdetails.pcompanyumrn = Convert.ToString(dr["companyumrn"]);
                        CompanyDTOdetails.ptransactionopeningdate = dr["transactionopeningdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["transactionopeningdate"]).ToString("dd/MM/yyyy");
                        CompanyDTOdetails.pdefaultpassword = Convert.ToString(dr["defaultpassword"]);
                        CompanyDTOdetails.pdatepickerenableenddate = dr["datepickerenableenddate"] == DBNull.Value ? null : Convert.ToDateTime(dr["datepickerenableenddate"]).ToString("dd/MM/yyyy");

                        CompanyDTOdetails.lstCompanyContactDTO = new List<CompanyContactDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, companyid, contactnumber, emailid, priority  FROM tblmstcompanycontactdetails where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                CompanyDTOdetails.lstCompanyContactDTO.Add(new CompanyContactDTO
                                {
                                    pRecordId = Convert.ToInt64(dr1["recordid"]),
                                    pCompanyId = Convert.ToInt64(dr1["companyid"]),
                                    pcontactnumber = Convert.ToInt64(dr1["contactnumber"]),
                                    pemailid = Convert.ToString(dr1["emailid"]),
                                    ppriority = Convert.ToString(dr1["priority"]),
                                    ptypeofoperation = "OLD"
                                });
                            }
                        }
                        CompanyDTOdetails.lstCompanyAddressDTO = new List<CompanyAddressDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, companyid, addresstype, address1, address2, state,stateid, district, districtid, city, country, countryid, pincode,priority from tblmstcompanyaddressdetails where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                CompanyDTOdetails.lstCompanyAddressDTO.Add(new CompanyAddressDTO
                                {
                                    pRecordId = Convert.ToInt64(dr1["recordid"]),
                                    pCompanyId = Convert.ToInt64(dr1["companyid"]),
                                    pAddressType = Convert.ToString(dr1["addresstype"]),
                                    pAddress1 = Convert.ToString(dr1["address1"]),
                                    pAddress2 = Convert.ToString(dr1["address2"]),
                                    pState = Convert.ToString(dr1["state"]),
                                    pStateId = Convert.ToInt64(dr1["stateid"]),
                                    pDistrict = Convert.ToString(dr1["district"]),
                                    pDistrictId = Convert.ToInt64(dr1["districtid"]),
                                    pCity = Convert.ToString(dr1["city"]),
                                    pCountry = Convert.ToString(dr1["country"]),
                                    pCountryId = Convert.ToInt64(dr1["countryid"]),
                                    pPinCode = Convert.ToInt64(dr1["pincode"]),
                                    pPriority = Convert.ToString(dr1["priority"]),
                                    ptypeofoperation = "OLD"
                                });
                            }
                        }
                        CompanyDTOdetails.lstCompanyDocumentsDTO = new List<CompanyDocumentsDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, companyid, documentid, documentgroupid, documentgroupname,documentname, docstorepath, docfiletype, docfilename, docreferenceno,       docisdownloadable from tblmstcompanydocuments where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                CompanyDTOdetails.lstCompanyDocumentsDTO.Add(new CompanyDocumentsDTO
                                {
                                    pRecordId = Convert.ToInt64(dr1["recordid"]),
                                    pCompanyId = Convert.ToInt64(dr1["companyid"]),
                                    pDOCUMENTID = Convert.ToInt64(dr1["documentid"]),
                                    pDOCUMENTGROUPID = Convert.ToInt64(dr1["documentgroupid"]),
                                    pDOCUMENTGROUPNAME = Convert.ToString(dr1["documentgroupname"]),
                                    pDOCUMENTNAME = Convert.ToString(dr1["documentname"]),
                                    pDOCSTOREPATH = Convert.ToString(dr1["docstorepath"]),
                                    pDOCFILETYPE = Convert.ToString(dr1["docfiletype"]),
                                    pDOCFILENAME = Convert.ToString(dr1["docfilename"]),
                                    pDOCREFERENCENO = Convert.ToString(dr1["docreferenceno"]),
                                    pDOCISDOWNLOADABLE = Convert.ToBoolean(dr1["docisdownloadable"]),
                                    ptypeofoperation = "OLD"
                                });
                            }
                        }
                        CompanyDTOdetails.lstCompanyPromotersDTO = new List<CompanyPromotersDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, companyid, contactid from tblmstcompanypromoters where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                CompanyDTOdetails.lstCompanyPromotersDTO.Add(new CompanyPromotersDTO
                                {
                                    pRecordId = Convert.ToInt64(dr1["recordid"]),
                                    pCompanyId = Convert.ToInt64(dr1["companyid"]),
                                    pContactId = Convert.ToInt64(dr1["contactid"]),
                                    ptypeofoperation = "OLD"
                                });
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return CompanyDTOdetails;
        }
    }
}
