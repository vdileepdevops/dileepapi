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
    public class BranchDAL : SettingsDAL, IBranch
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;
        public BranchDTO BranchDTOdetails { set; get; }
        public bool SaveBranchDetails(BranchDTO BranchDTO, string connectionstring)
        {
            bool IsSave = false;
            string Recordid = string.Empty;
            StringBuilder saveBranchDetails = new StringBuilder();
            StringBuilder savebeforeinsert = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (BranchDTO != null)
                {
                    if (string.IsNullOrEmpty(BranchDTO.pbranchname))
                    {
                        BranchDTO.pbranchname = "";
                    }
                    if (string.IsNullOrEmpty(BranchDTO.pbranchcode))
                    {
                        BranchDTO.pbranchcode = "";
                    }
                    if (string.IsNullOrEmpty(BranchDTO.pemailid))
                    {
                        BranchDTO.pemailid = "";
                    }
                    if (string.IsNullOrEmpty(BranchDTO.pgstinnumber))
                    {
                        BranchDTO.pgstinnumber = "";
                    }
                    if (string.IsNullOrEmpty(BranchDTO.pestablishmentdate))
                    {
                        BranchDTO.pestablishmentdate = "null";
                    }
                    else
                    {
                        BranchDTO.pestablishmentdate = "'" + FormatDate(BranchDTO.pestablishmentdate) + "'";
                    }
                    if (string.IsNullOrEmpty(BranchDTO.pstatename))
                    {
                        BranchDTO.pstatename = "";
                    }
                    if (BranchDTO.ptypeofoperation.ToUpper() == "CREATE")
                    {
                        BranchDTO.pbranchid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "INSERT INTO tblmstbranch(branchname , branchcode , establishmentdate , contactnumber ,emailid , gstinnumber , statecode , statename ,stateid,statusid, createdby, createddate) VALUES ('" + ManageQuote(BranchDTO.pbranchname) + "','" + ManageQuote(BranchDTO.pbranchcode) + "'," + (BranchDTO.pestablishmentdate) + "," + BranchDTO.pcontactnumber + ",'" + ManageQuote(BranchDTO.pemailid) + "', '" + ManageQuote(BranchDTO.pgstinnumber) + "'," + BranchDTO.pstatecode + ",'" + ManageQuote(BranchDTO.pstatename) + "'," + BranchDTO.pstateid + "," + Convert.ToInt32(Status.Active) + "," + BranchDTO.pCreatedby + ", current_timestamp) returning branchid ;"));
                    }
                    else
                    {
                        saveBranchDetails.Append("UPDATE tblmstbranch  SET branchname ='" + ManageQuote(BranchDTO.pbranchname) + "', branchcode ='" + ManageQuote(BranchDTO.pbranchcode) + "', establishmentdate =" + (BranchDTO.pestablishmentdate) + ", contactnumber =" + BranchDTO.pcontactnumber + ", emailid ='" + ManageQuote(BranchDTO.pemailid) + "', gstinnumber ='" + ManageQuote(BranchDTO.pgstinnumber) + "', statusid =" + Convert.ToInt32(Status.Active) + ", modifiedby =" + BranchDTO.pCreatedby + ", modifieddate =current_timestamp WHERE branchid=" + BranchDTO.pbranchid + ";");

                    }
                    if (BranchDTO.lstBranchAddressDTO.Count > 0)
                    {
                        for (int i = 0; i < BranchDTO.lstBranchAddressDTO.Count; i++)
                        {
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pAddressType))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pAddressType = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].paddress1))
                            {
                                BranchDTO.lstBranchAddressDTO[i].paddress1 = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].paddress2))
                            {
                                BranchDTO.lstBranchAddressDTO[i].paddress2 = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pState))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pState = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pDistrict))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pDistrict = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pcity))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pcity = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pCountry))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pCountry = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO[i].pPriority))
                            {
                                BranchDTO.lstBranchAddressDTO[i].pPriority = "";
                            }
                            if (BranchDTO.lstBranchAddressDTO[i].ptypeofoperation.ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = BranchDTO.lstBranchAddressDTO[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + BranchDTO.lstBranchAddressDTO[i].pRecordid.ToString();
                                }
                            }
                            if (BranchDTO.lstBranchAddressDTO[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                saveBranchDetails.Append("INSERT INTO tblmstbranchaddressdetails(branchid,addresstype, address1, address2, state,stateid, district, districtid, city, country, countryid, pincode,priority, statusid, createdby, createddate)VALUES (" + BranchDTO.pbranchid + ",'" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pAddressType) + "','" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].paddress1) + "','" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].paddress2) + "','" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pState) + "'," + BranchDTO.lstBranchAddressDTO[i].pStateId + ",'" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pDistrict) + "'," + BranchDTO.lstBranchAddressDTO[i].pDistrictId + ",'" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pcity) + "','" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pCountry) + "'," + BranchDTO.lstBranchAddressDTO[i].pCountryId + "," + BranchDTO.lstBranchAddressDTO[i].pPincode + ",'" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pPriority) + "'," + Convert.ToInt32(Status.Active) + "," + BranchDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveBranchDetails.Append("update tblmstbranchaddressdetails set addresstype='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pAddressType) + "',address1='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].paddress1) + "',address2='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].paddress2) + "',state='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pState) + "',stateid=" + BranchDTO.lstBranchAddressDTO[i].pStateId + ",district='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pDistrict) + "',districtid=" + BranchDTO.lstBranchAddressDTO[i].pDistrictId + ",city='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pcity) + "',country='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pCountry) + "',countryid=" + BranchDTO.lstBranchAddressDTO[i].pCountryId + ",pincode=" + BranchDTO.lstBranchAddressDTO[i].pPincode + ",priority='" + ManageQuote(BranchDTO.lstBranchAddressDTO[i].pPriority) + "',modifiedby=" + BranchDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + BranchDTO.lstBranchAddressDTO[i].pRecordid + ";");
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        savebeforeinsert.Append("UPDATE tblmstbranchaddressdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + BranchDTO.pCreatedby + ",modifieddate=current_timestamp where branchid=" + BranchDTO.pbranchid + " and recordid not in (" + Recordid + ");");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BranchDTO.lstBranchAddressDTO.ToString()))
                        {
                            savebeforeinsert.Append("UPDATE tblmstbranchaddressdetails set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifieddate=current_timestamp where branchid=" + BranchDTO.pbranchid + " AND statusid<>2;");
                        }
                    }
                    Recordid = string.Empty;
                    if (BranchDTO.lstBranchDocStoreDTO.Count > 0)
                    {
                        for (int i = 0; i < BranchDTO.lstBranchDocStoreDTO.Count; i++)
                        {
                            if (BranchDTO.lstBranchDocStoreDTO[i].ptypeofoperation.ToUpper() != "CREATE")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = BranchDTO.lstBranchDocStoreDTO[i].pRecordId.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + BranchDTO.lstBranchDocStoreDTO[i].pRecordId.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchDocStoreDTO[i].pDOCREFERENCENO))
                            {
                                BranchDTO.lstBranchDocStoreDTO[i].pDOCREFERENCENO = "";
                            }
                            if (string.IsNullOrEmpty(BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH))
                            {
                                BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH = "";
                            }
                            if (BranchDTO.lstBranchDocStoreDTO[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                saveBranchDetails.Append("INSERT INTO tblmstbranchdocumentproofs(branchid ,documentid ,documentgroupid,documentgroupname,documentname ,docreferenceno ,docstorepath ,docfiletype,docfilename,docisdownloadable, statusid, createdby, createddate)VALUES (" + BranchDTO.pbranchid + "," + BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTID + "," + BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTGROUPID + ",'" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTGROUPNAME) + "','" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTNAME) + "','" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCREFERENCENO) + "','" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH) + "','" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCFILETYPE) + "','" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCFILENAME) + "','" + (BranchDTO.lstBranchDocStoreDTO[i].pDOCISDOWNLOADABLE) + "'," + Convert.ToInt32(Status.Active) + "," + BranchDTO.pCreatedby + ",current_timestamp);");
                            }
                            else
                            {
                                saveBranchDetails.Append("update tblmstbranchdocumentproofs set documentid=" + BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTID + ",documentgroupid=" + BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTGROUPID + ",documentgroupname='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTGROUPNAME) + "',documentname='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCUMENTNAME) + "',docreferenceno='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCREFERENCENO) + "',docstorepath='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH) + "',docfiletype='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCFILETYPE) + "',docfilename='" + ManageQuote(BranchDTO.lstBranchDocStoreDTO[i].pDOCFILENAME) + "',docisdownloadable='" + (BranchDTO.lstBranchDocStoreDTO[i].pDOCISDOWNLOADABLE) + "',modifiedby=" + BranchDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + BranchDTO.lstBranchDocStoreDTO[i].pRecordId + ";");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        savebeforeinsert.Append("UPDATE tblmstbranchdocumentproofs set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + BranchDTO.pCreatedby + ",modifieddate=current_timestamp where branchid=" + BranchDTO.pbranchid + " and recordid not in (" + Recordid + ");");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BranchDTO.lstBranchDocStoreDTO.ToString()) || BranchDTO.lstBranchDocStoreDTO.Count == 0)
                        {
                            savebeforeinsert.Append("UPDATE tblmstbranchdocumentproofs set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifieddate=current_timestamp where branchid=" + BranchDTO.pbranchid + " AND statusid<>2;");
                        }
                    }
                }
                if (Convert.ToString(saveBranchDetails) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, savebeforeinsert + "" + saveBranchDetails.ToString());
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

        public BranchDTO getBranchDetails(string ConnectionString)
        {
            try
            {
                BranchDTOdetails = new BranchDTO();
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT branchid, branchname, branchcode, establishmentdate, contactnumber,emailid, gstinnumber, statusid, createdby, createddate, modifiedby,      modifieddate, statecode, statename, stateid  FROM tblmstbranch; "))
                {
                    while (dr.Read())
                    {
                        BranchDTOdetails = new BranchDTO();
                        BranchDTOdetails.pbranchid = Convert.ToInt32(dr["branchid"]);
                        BranchDTOdetails.pbranchname = dr["branchname"].ToString();
                        BranchDTOdetails.pbranchcode = dr["branchcode"].ToString();
                        BranchDTOdetails.pcontactnumber = Convert.ToInt64(dr["contactnumber"]);
                        BranchDTOdetails.pemailid = Convert.ToString(dr["emailid"]);
                        BranchDTOdetails.pgstinnumber = Convert.ToString(dr["gstinnumber"]);
                        BranchDTOdetails.pestablishmentdate = dr["establishmentdate"] == DBNull.Value ? null : Convert.ToDateTime(dr["establishmentdate"]).ToString("dd/MM/yyyy");
                        BranchDTOdetails.ptypeofoperation = "UPDATE";

                        BranchDTOdetails.lstBranchAddressDTO = new List<BranchAddressDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, branchid, addresstype, address1, address2, state, stateid,district, districtid, city, country, countryid, pincode, priority,       statusid, createdby, createddate, modifiedby, modifieddate  FROM tblmstbranchaddressdetails where statusid =" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                BranchDTOdetails.lstBranchAddressDTO.Add(new BranchAddressDTO
                                {
                                    pRecordid = Convert.ToInt64(dr1["recordid"]),
                                    pbranchid = Convert.ToInt64(dr1["branchid"]),
                                    pAddressType = Convert.ToString(dr1["addresstype"]),
                                    paddress1 = Convert.ToString(dr1["address1"]),
                                    paddress2 = Convert.ToString(dr1["address2"]),
                                    pState = Convert.ToString(dr1["state"]),
                                    pStateId = Convert.ToInt64(dr1["stateid"]),
                                    pDistrict = Convert.ToString(dr1["district"]),
                                    pDistrictId = Convert.ToInt64(dr1["districtid"]),
                                    pcity = Convert.ToString(dr1["city"]),
                                    pCountry = Convert.ToString(dr1["country"]),
                                    pCountryId = Convert.ToInt64(dr1["countryid"]),
                                    pPincode = Convert.ToInt64(dr1["pincode"]),
                                    pPriority = Convert.ToString(dr1["priority"]),
                                    ptypeofoperation = "OLD"

                                });
                            }
                        }
                        BranchDTOdetails.lstBranchDocStoreDTO = new List<CompanyDocumentsDTO>();
                        using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, branchid, documentid, documentgroupid, documentgroupname,documentname, docstorepath, docfiletype, docfilename, docreferenceno,       docisdownloadable from tblmstbranchdocumentproofs where statusid=" + Convert.ToInt32(Status.Active) + ";"))
                        {
                            while (dr1.Read())
                            {
                                BranchDTOdetails.lstBranchDocStoreDTO.Add(new CompanyDocumentsDTO
                                {
                                    pRecordId = Convert.ToInt64(dr1["recordid"]),
                                    pCompanyId = Convert.ToInt64(dr1["branchid"]),
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

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return BranchDTOdetails;
        }

        public string checkbranchnameDuplicates(string branchname, string branchcode, int branchid, string connectionstring)
        {
            int count = 0;
            string errormessage = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(branchid.ToString()) || branchid == 0)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstbranch where upper(branchname)='" + ManageQuote(branchname.Trim().ToUpper()) + "'"));
                    if (count > 0)
                    {
                        errormessage = "Branch Name";
                    }
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstbranch where upper(branchcode)='" + ManageQuote(branchcode.Trim().ToUpper()) + "'"));
                    if (count > 0 && !string.IsNullOrEmpty(errormessage))
                    {
                        errormessage = errormessage + ",Branch Code";
                    }
                    else if (count > 0 && string.IsNullOrEmpty(errormessage))
                    {
                        errormessage = "Branch Code";
                    }
                }
                else
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstbranch where upper(branchname)='" + ManageQuote(branchname.Trim().ToUpper()) + "' and branchid!=" + branchid + ";"));
                    if (count > 0)
                    {
                        errormessage = "Branch Name";
                    }
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionstring, CommandType.Text, "select count(*) from tblmstbranch where upper(branchcode)='" + ManageQuote(branchcode.Trim().ToUpper()) + "' and branchid!=" + branchid + ";"));
                    if (count > 0 && !string.IsNullOrEmpty(errormessage))
                    {
                        errormessage = errormessage + ",Branch Code";
                    }
                    else if (count > 0 && string.IsNullOrEmpty(errormessage))
                    {
                        errormessage = "Branch Code";
                    }

                }
                if (!string.IsNullOrEmpty(errormessage))
                {
                    errormessage = errormessage + " Already Exists";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return errormessage;
        }
    }
}
