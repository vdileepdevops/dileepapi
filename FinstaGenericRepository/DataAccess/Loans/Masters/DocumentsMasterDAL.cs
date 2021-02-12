using FinstaInfrastructure.Loans.Masters;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Masters;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Text;

namespace FinstaRepository.DataAccess.Loans.Masters
{
    public class DocumentsMasterDAL : SettingsDAL, IDocumentsMaster
    {
        NpgsqlDataReader dr = null;
        NpgsqlDataReader dr1 = null;


        #region SaveDocumentionGroup
        public async Task<bool> SaveDocumentGroup(DocumentsMasterDTO Documents, string ConnectionString)
        {
            bool isSaved = false;
            await Task.Run(() =>
            {
                try
                {

                    NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstdocumentgroup(documentgroupname,statusid,createdby,createddate)values('" + ManageQuote(Documents.pDocumentGroup.Trim()) + "'," + getStatusid(Documents.pStatusname, ConnectionString) + "," + Documents.pCreatedby + ",current_timestamp);");
                    isSaved = true;
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return isSaved;
        }
        #endregion

        #region SaveIdentificationDocumention 
        public async Task<bool> SaveIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString)
        {
            bool isSaved = false;
            await Task.Run(() =>
            {
                try
                {
                    NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "insert into tblmstdocumentproofs(documentgroupid,documentgroupname,documentname,statusid,createdby,createddate)values(" + Documents.pDocumentGroupId + ",'" + ManageQuote(Documents.pDocumentGroup.Trim()) + "','" + ManageQuote(Documents.pDocumentName.Trim()) + "'," + getStatusid(Documents.pStatusname, ConnectionString) + "," + Documents.pCreatedby + ",current_timestamp);");
                    isSaved = true;
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return isSaved;
        }
        #endregion

        //#region Getdocumentidprofftypes
        //public List<DocumentsMasterDTO> Getdocumentidprofftypes(string ConnectionString, LoanIdDTO Documents)
        //{
        //    List<DocumentsMasterDTO> lstdocumentidprofftypes = new List<DocumentsMasterDTO>();
        //    try
        //    {
        //        dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup");
        //        if (Documents.pLoanId> 0)
        //        {

        //            dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1 and documentid not in(select documentid from tblmstloanwisedocumentproofs where statusid=1 and  loanid=" + Documents.pLoanId + ") union select y.contacttype,x.documentid,x.documentgroupid,x.documentgroupname,x.documentname,y.isdocumentrequired as mandatory,y.isdocumentrequired required from tblmstdocumentproofs x right join tblmstloanwisedocumentproofs y on x.documentid = y.documentid where y.statusid = 1 and y.loanid = " + Documents.pLoanId + ";");
        //        }
        //        else
        //        {

        //            dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1");
        //        }
        //        while (dr1.Read())
        //        {
        //            DocumentsMasterDTO objdocumentidproofs = new DocumentsMasterDTO();
        //            objdocumentidproofs.pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]);
        //            objdocumentidproofs.pDocumentGroup = dr1["documentgroupname"].ToString();
        //            objdocumentidproofs.pDocumentsList = new List<pIdentificationDocumentsDTO>();
        //            while (dr.Read())
        //            {

        //                if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
        //                {
        //                    objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO {

        //                       pContactType= dr["contacttype"].ToString(),

        //                        pDocumentId =Convert.ToInt64(dr["documentid"]), pDocumentName = dr["documentname"].ToString(),
        //                        pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
        //                        pDocumentRequired = Convert.ToBoolean(dr["required"]) ,pDocumentgroupId= Convert.ToInt64(dr["documentgroupid"]),
        //                        pLoantypeId=Documents.pLoanId
        //                    });
        //                }
        //            }
        //            lstdocumentidprofftypes.Add(objdocumentidproofs);
        //        }             
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    return lstdocumentidprofftypes;
        //}
        //#endregion



        #region Getdocumentidprofftypes

        public List<pIdentificationDocumentsDTO> GetdocumentidproffDetails(string ConnectionString, Int64 pLoanId)
        {

            List<pIdentificationDocumentsDTO> pDocumentsList = new List<pIdentificationDocumentsDTO>();
            string Query = string.Empty;

            try
            {

                Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required,ts.statusname from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = 1;";

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        pDocumentsList.Add(new pIdentificationDocumentsDTO
                        {
                            pDocumentGroupId = Convert.ToInt64(dr["documentgroupid"]),
                            pDocumentGroup = dr["documentgroupname"].ToString(),
                            pContactType = dr["contacttype"].ToString(),
                            pDocumentId = Convert.ToInt64(dr["documentid"]),
                            pDocumentName = dr["documentname"].ToString(),
                            pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                            pDocumentRequired = Convert.ToBoolean(dr["required"]),
                            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                            pStatusname = dr["statusname"].ToString(),
                            pLoanId = pLoanId,
                            // pLoantypeId = pLoanId,
                        });

                        //pIdentificationDocumentsDTO _pIdentificationDocumentsDTO = new pIdentificationDocumentsDTO();

                        //_pIdentificationDocumentsDTO.pDocumentGroupId = Convert.ToInt64(dr["documentgroupid"]);
                        //_pIdentificationDocumentsDTO.pDocumentGroup = dr["documentgroupname"].ToString();
                        //_pIdentificationDocumentsDTO.pContactType = dr["contacttype"].ToString();
                        //_pIdentificationDocumentsDTO.pDocumentId = Convert.ToInt64(dr["documentid"]);
                        //_pIdentificationDocumentsDTO.pDocumentName = dr["documentname"].ToString();
                        //_pIdentificationDocumentsDTO.pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]);
                        //_pIdentificationDocumentsDTO.pDocumentRequired = Convert.ToBoolean(dr["required"]);
                        //_pIdentificationDocumentsDTO.pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]);
                        //_pIdentificationDocumentsDTO.pStatusname = dr["statusname"].ToString();

                        //pDocumentsList.Add(_pIdentificationDocumentsDTO);
                        //pLoanId = pLoanId,
                        // pLoantypeId = pLoanId,


                    }

                }



            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pDocumentsList;
        }
        public List<DocumentsMasterDTO> Getdocumentidprofftypes(string ConnectionString, Int64 pLoanId)
        {
            List<DocumentsMasterDTO> lstdocumentidprofftypes = new List<DocumentsMasterDTO>();
            string Query = string.Empty;

            try
            {
                using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup where documentgroupid in(select distinct documentgroupid from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = 1)"))
                {
                    if (pLoanId > 0)
                    {

                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required from tblmstdocumentproofs where statusid=1 and documentid not in(select documentid from tblmstloanwisedocumentproofs where statusid=1 and  loanid=" + pLoanId + ") union select y.contacttype,x.documentid,x.documentgroupid,x.documentgroupname,x.documentname,y.isdocumentmandatory as mandatory,y.isdocumentrequired required from tblmstdocumentproofs x right join tblmstloanwisedocumentproofs y on x.documentid = y.documentid where y.statusid = 1 and y.loanid = " + pLoanId + ";";
                    }
                    else
                    {
                        Query = "select '' as contacttype,documentid,documentgroupid,documentgroupname,documentname,'false'::BOOLEAN as mandatory,'false'::BOOLEAN as required,ts.statusname from tblmstdocumentproofs tc join tblmststatus ts on tc.statusid = ts.statusid where tc.statusid = 1;";
                    }
                    while (dr1.Read())
                    {
                        DocumentsMasterDTO objdocumentidproofs = new DocumentsMasterDTO();
                        objdocumentidproofs.pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]);
                        objdocumentidproofs.pDocumentGroup = dr1["documentgroupname"].ToString();
                        objdocumentidproofs.pDocumentsList = new List<pIdentificationDocumentsDTO>();
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                        {
                            if (pLoanId > 0)
                            {
                                while (dr.Read())
                                {

                                    if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
                                    {
                                        objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO
                                        {
                                            pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]),
                                            pDocumentGroup = dr1["documentgroupname"].ToString(),
                                            pContactType = dr["contacttype"].ToString(),
                                            pDocumentId = Convert.ToInt64(dr["documentid"]),
                                            pDocumentName = dr["documentname"].ToString(),
                                            pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                            pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                            pLoanId = pLoanId,
                                            // pLoantypeId = pLoanId,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                while (dr.Read())
                                {

                                    if (dr1["documentgroupname"].ToString() == dr["documentgroupname"].ToString())
                                    {
                                        objdocumentidproofs.pDocumentsList.Add(new pIdentificationDocumentsDTO
                                        {
                                            pDocumentGroupId = Convert.ToInt64(dr1["documentgroupid"]),
                                            pDocumentGroup = dr1["documentgroupname"].ToString(),
                                            pContactType = dr["contacttype"].ToString(),
                                            pDocumentId = Convert.ToInt64(dr["documentid"]),
                                            pDocumentName = dr["documentname"].ToString(),
                                            pDocumentMandatory = Convert.ToBoolean(dr["mandatory"]),
                                            pDocumentRequired = Convert.ToBoolean(dr["required"]),
                                            pDocumentgroupId = Convert.ToInt64(dr["documentgroupid"]),
                                            pStatusname = dr["statusname"].ToString(),
                                            pLoanId = pLoanId,
                                            // pLoantypeId = pLoanId,
                                        });
                                    }
                                }
                            }
                        }
                        lstdocumentidprofftypes.Add(objdocumentidproofs);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return lstdocumentidprofftypes;
        }
        #endregion

        #region GetDocumentGroup

        public async Task<List<DocumentsMasterDTO>> GetDocumentGroupNames(string ConnectionString)
        {
            List<DocumentsMasterDTO> lstdocumentGroups = new List<DocumentsMasterDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select documentgroupid,documentgroupname from tblmstdocumentgroup"))
                    {
                        while (dr.Read())
                        {
                            DocumentsMasterDTO objGroupNames = new DocumentsMasterDTO();
                            objGroupNames.pDocumentGroupId = Convert.ToInt32(dr["documentgroupid"]);
                            objGroupNames.pDocumentGroup = dr["documentgroupname"].ToString();
                            lstdocumentGroups.Add(objGroupNames);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            });
            return lstdocumentGroups;
        }
        #endregion

        #region checkduplicategroupnamesanddocuments
        public int CheckDuplicateDocGroupNames(string DocGroupName, string ConnectionString)
        {
            int count = 0;
            try
            {
                if (string.IsNullOrEmpty(DocGroupName))
                {
                    DocGroupName = "";
                }
                else
                {
                    DocGroupName = DocGroupName.ToUpper();
                }
                if (!string.IsNullOrEmpty(DocGroupName))
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstdocumentgroup where upper(documentgroupname)='" + DocGroupName.Trim() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }

            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public int CheckDuplicateDocbasedonGroupNames(string DocGroupName, string DocumentName, Int64 DocumenId, string ConnectionString)
        {
            int count = 0;
            try
            {
                if (string.IsNullOrEmpty(DocGroupName))
                {
                    DocGroupName = "";
                }
                else
                {
                    DocGroupName = DocGroupName.ToUpper();
                }
                if (string.IsNullOrEmpty(DocumentName))
                {
                    DocumentName = "";
                }
                else
                {
                    DocumentName = DocumentName.ToUpper();
                }
                if (string.IsNullOrEmpty(DocumenId.ToString()) || DocumenId == 0)
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstdocumentproofs where upper(documentgroupname)='" + DocGroupName.Trim() + "' and upper(documentname)='" + DocumentName.Trim() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
                else
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(*) from tblmstdocumentproofs where upper(documentgroupname)='" + DocGroupName.Trim() + "' and upper(documentname)='" + DocumentName.Trim() + "' and documentid!=" + DocumenId + " and statusid=" + Convert.ToInt32(Status.Active) + ";"));
                }
            }
            catch (Exception)
            {

                throw;
            }
            return count;
        }

        public async Task<bool> UpdateIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString)
        {
            bool isUpdate = false;
            await Task.Run(() =>
            {
                try
                {
                    NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update tblmstdocumentproofs set documentgroupid=" + Documents.pDocumentGroupId + ",documentgroupname='" + ManageQuote(Documents.pDocumentGroup.Trim()) + "',documentname='" + ManageQuote(Documents.pDocumentName.Trim()) + "',modifiedby=" + Documents.pCreatedby + " where documentid=" + Documents.pDocumentId + ";");
                    isUpdate = true;
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return isUpdate;
        }

        public async Task<bool> DeleteIdentificationDocuments(DocumentsMasterDTO Documents, string ConnectionString)
        {
            bool isDelete = false;
            await Task.Run(() =>
            {
                try
                {
                    NPGSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update tblmstdocumentproofs set statusid= " + getStatusid(Documents.pStatusname, ConnectionString) + "   where documentid=" + Documents.pDocumentId + ";");
                    isDelete = true;
                }
                catch (Exception)
                {

                    throw;
                }
            });
            return isDelete;
        }
        #endregion

    }
}
