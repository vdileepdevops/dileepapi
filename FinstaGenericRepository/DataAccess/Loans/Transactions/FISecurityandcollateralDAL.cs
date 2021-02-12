using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public partial class FirstinformationDAL : SettingsDAL, IFirstinformation
    {
        public ApplicationSecurityandCollateralDTO ApplicationSecurityandCollateralDTO { set; get; }



        public void saveImmovablemovablepropertydetails(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO, string connectionstring, StringBuilder sbinsert, StringBuilder sbinsertfirst)
        {

            string Recordid = string.Empty;
            try
            {

                if (SecurityandCollateralDTO.pIsimmovablepropertyapplicable == true)
                {
                    if (SecurityandCollateralDTO.ImMovablePropertyDetailsList != null)
                    {
                        for (int i = 0; i < SecurityandCollateralDTO.ImMovablePropertyDetailsList.Count; i++)
                        {
                            if (SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pStatusname == null)
                            {
                                SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pStatusname = "ACTIVE";
                            }
                            if (string.IsNullOrEmpty(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate))
                            {
                                SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate = "null";
                            }
                            else
                            {
                                SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate = "'" + FormatDate(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate) + "'";
                            }

                            if (SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper() != "CREATE" || SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "OLD")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pRecordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pEstimatedmarketvalue.ToString()))
                            {
                                SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pEstimatedmarketvalue = 0;
                            }

                            if (SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                sbinsert.Append("INSERT INTO tabapplicationsecuritycollateralimmovablepropertydetails(applicationid, vchapplicationid, contactid, contactreferenceid,typeofproperty, titledeed, deeddate, propertyownername, addressofproperty,estimatedmarketvalue, propertydocpath,filename, statusid, createdby, createddate)VALUES(" + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "'," + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pContactreferenceid) + "','" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pTypeofproperty) + "', '" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pTitledeed) + "', " + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate + ", '" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertyownername) + "','" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pAddressofproperty) + "',coalesce(" + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pEstimatedmarketvalue + ",0), '" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath) + "','" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpathname) + "', " + getStatusid(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                            }
                            else if (SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "UPDATE")
                            {
                                if (SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pIsapplicable)
                                {
                                    sbinsert.Append("update tabapplicationsecuritycollateralimmovablepropertydetails set typeofproperty='" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pTypeofproperty) + "', titledeed='" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pTitledeed) + "', deeddate=" + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pDeeddate + ", propertyownername='" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertyownername) + "', addressofproperty='" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pAddressofproperty) + "',estimatedmarketvalue=coalesce(" + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pEstimatedmarketvalue + ",0), propertydocpath= '" + ManageQuote(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pPropertydocpath) + "', statusid=" + getStatusid(SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pStatusname, connectionstring) + ", modifiedby=" + SecurityandCollateralDTO.pCreatedby + ", modifieddate=current_timestamp where recordid=" + SecurityandCollateralDTO.ImMovablePropertyDetailsList[i].pRecordid + " and applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                                }

                            }


                        }
                    }
                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralimmovablepropertydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " and recordid not in (" + Recordid + ");");
                }
                else
                {
                    if (string.IsNullOrEmpty(SecurityandCollateralDTO.ImMovablePropertyDetailsList.ToString()) || SecurityandCollateralDTO.ImMovablePropertyDetailsList.Count == 0 || (Recordid == "" && SecurityandCollateralDTO.ImMovablePropertyDetailsList.Count > 0))
                    {
                        sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralimmovablepropertydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " AND statusid<>2;");
                    }
                }

                Recordid = string.Empty;
                if (SecurityandCollateralDTO.pismovablepropertyapplicable == true)
                {
                    if (SecurityandCollateralDTO.MovablePropertyDetailsList != null)
                    {
                        for (int i = 0; i < SecurityandCollateralDTO.MovablePropertyDetailsList.Count; i++)
                        {
                            if (SecurityandCollateralDTO.MovablePropertyDetailsList[i].pStatusname == null)
                            {
                                SecurityandCollateralDTO.MovablePropertyDetailsList[i].pStatusname = "ACTIVE";
                            }
                            if (SecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper() != "CREATE" || SecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "OLD")
                            {
                                if (string.IsNullOrEmpty(Recordid))
                                {
                                    Recordid = SecurityandCollateralDTO.MovablePropertyDetailsList[i].pRecordid.ToString();
                                }
                                else
                                {
                                    Recordid = Recordid + "," + SecurityandCollateralDTO.MovablePropertyDetailsList[i].pRecordid.ToString();
                                }
                            }
                            if (string.IsNullOrEmpty(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pEstimatedmarketvalue.ToString()))
                            {
                                SecurityandCollateralDTO.MovablePropertyDetailsList[i].pEstimatedmarketvalue = 0;
                            }

                            if (SecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "CREATE")
                            {
                                sbinsert.Append("INSERT INTO tabapplicationsecuritycollateralmovablepropertydetails(applicationid, vchapplicationid, contactid, contactreferenceid,typeofvehicle, vehicleownername,vehiclemodelandmake, registrationno, estimatedmarketvalue, vehicledocpath,filename, statusid, createdby, createddate)VALUES(" + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', " + SecurityandCollateralDTO.MovablePropertyDetailsList[i].pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pContactreferenceid) + "', '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].PTypeofvehicle) + "', '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicleownername) + "', '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehiclemodelandmake) + "', '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pRegistrationno) + "', coalesce(" + SecurityandCollateralDTO.MovablePropertyDetailsList[i].pEstimatedmarketvalue + ",0), '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath) + "', '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpathname) + "'," + getStatusid(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                            }
                            else if (SecurityandCollateralDTO.MovablePropertyDetailsList[i].ptypeofoperation.ToUpper() == "UPDATE")
                            {
                                if (SecurityandCollateralDTO.MovablePropertyDetailsList[i].pIsapplicable)
                                {
                                    sbinsert.Append("update tabapplicationsecuritycollateralmovablepropertydetails set typeofvehicle= '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].PTypeofvehicle) + "', vehicleownername= '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicleownername) + "',vehiclemodelandmake='" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehiclemodelandmake) + "', registrationno='" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pRegistrationno) + "', estimatedmarketvalue=coalesce(" + SecurityandCollateralDTO.MovablePropertyDetailsList[i].pEstimatedmarketvalue + ",0), vehicledocpath= '" + ManageQuote(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pVehicledocpath) + "', statusid=" + getStatusid(SecurityandCollateralDTO.MovablePropertyDetailsList[i].pStatusname, connectionstring) + ", modifiedby= " + SecurityandCollateralDTO.pCreatedby + ", modifieddate=current_timestamp where recordid=" + SecurityandCollateralDTO.MovablePropertyDetailsList[i].pRecordid + " and applicationid=" + SecurityandCollateralDTO.pApplicationid + " ;");
                                }

                            }

                        }
                    }
                }
                if (!string.IsNullOrEmpty(Recordid))
                {
                    sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralmovablepropertydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " and recordid not in (" + Recordid + ");");
                }
                else
                {
                    if (string.IsNullOrEmpty(SecurityandCollateralDTO.MovablePropertyDetailsList.ToString()) || SecurityandCollateralDTO.MovablePropertyDetailsList.Count == 0 || (Recordid == "" && SecurityandCollateralDTO.MovablePropertyDetailsList.Count > 0))
                    {
                        sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralmovablepropertydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool saveApplicationSecurityCollateral(ApplicationSecurityandCollateralDTO SecurityandCollateralDTO, string connectionstring)
        {
            bool isSaved = false;
            string Recordid = string.Empty;
            int countsecuritysectiondataexists = 0;
            StringBuilder sbinsert = new StringBuilder();
            StringBuilder sbinsertfirst = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (!string.IsNullOrEmpty(SecurityandCollateralDTO.pVchapplicationid))
                {
                    SecurityandCollateralDTO.pApplicationid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select applicationid from tabapplication where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';"));
                }

                sbinsert.Append("UPDATE tabapplication set issecurityandcolletralapplicable= " + SecurityandCollateralDTO.pissecurityandcolletralapplicable + " where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';");

                if (SecurityandCollateralDTO.pissecurityandcolletralapplicable == true)
                {
                    countsecuritysectiondataexists = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, "select count(*) from tabapplicationsecurityapplicablesections where vchapplicationid = '" + SecurityandCollateralDTO.pVchapplicationid + "';"));
                    if (SecurityandCollateralDTO.pStatusname == null)
                    {
                        SecurityandCollateralDTO.pStatusname = "ACTIVE";
                    }
                    if (countsecuritysectiondataexists == 0)
                    {
                        sbinsert.Append("INSERT INTO tabapplicationsecurityapplicablesections(applicationid, vchapplicationid, contactid, contactreferenceid,isimmovablepropertyapplicable, ismovablepropertyapplicable, issecuritychequesapplicable,            isdepositslienapplicable, issecuritylienapplicable, statusid,createdby, createddate)VALUES( " + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', " + SecurityandCollateralDTO.pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.pContactreferenceid) + "'," + SecurityandCollateralDTO.pIsimmovablepropertyapplicable + ", " + SecurityandCollateralDTO.pismovablepropertyapplicable + " , " + SecurityandCollateralDTO.pissecuritychequesapplicable + " , " + SecurityandCollateralDTO.pisdepositslienapplicable + ", " + SecurityandCollateralDTO.pissecuritylienapplicable + ", " + getStatusid(SecurityandCollateralDTO.pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                    }
                    else
                    {
                        sbinsert.Append("update tabapplicationsecurityapplicablesections set isimmovablepropertyapplicable=" + SecurityandCollateralDTO.pIsimmovablepropertyapplicable + ", ismovablepropertyapplicable=" + SecurityandCollateralDTO.pismovablepropertyapplicable + ", issecuritychequesapplicable= " + SecurityandCollateralDTO.pissecuritychequesapplicable + ",isdepositslienapplicable=" + SecurityandCollateralDTO.pisdepositslienapplicable + ", issecuritylienapplicable=" + SecurityandCollateralDTO.pissecuritylienapplicable + ", statusid=" + getStatusid(SecurityandCollateralDTO.pStatusname, connectionstring) + ",modifiedby= " + SecurityandCollateralDTO.pCreatedby + ",modifieddate= current_timestamp where vchapplicationid='" + SecurityandCollateralDTO.pVchapplicationid + "' ;");
                    }

                    saveImmovablemovablepropertydetails(SecurityandCollateralDTO, connectionstring, sbinsert, sbinsertfirst);
                    if (SecurityandCollateralDTO.pissecuritychequesapplicable == true)
                    {
                        if (SecurityandCollateralDTO.SecuritychequesList != null)
                        {
                            for (int i = 0; i < SecurityandCollateralDTO.SecuritychequesList.Count; i++)
                            {
                                if (SecurityandCollateralDTO.SecuritychequesList[i].pStatusname == null)
                                {
                                    SecurityandCollateralDTO.SecuritychequesList[i].pStatusname = "ACTIVE";
                                }
                                if (SecurityandCollateralDTO.SecuritychequesList[i].ptypeofoperation.ToUpper() != "CREATE" || SecurityandCollateralDTO.SecuritychequesList[i].ptypeofoperation.ToUpper() == "OLD")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = SecurityandCollateralDTO.SecuritychequesList[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + SecurityandCollateralDTO.SecuritychequesList[i].pRecordid.ToString();
                                    }
                                }

                                if (SecurityandCollateralDTO.SecuritychequesList[i].ptypeofoperation.ToUpper() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationsecuritycollateralsecuritycheques(applicationid, vchapplicationid, applicanttype, contactid,contactreferenceid, typeofsecurity, bankname, ifsccode, accountno,chequeno,securitychequesdocpath,filename, statusid, createdby, createddate)VALUES (" + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pApplicanttype) + "', " + SecurityandCollateralDTO.SecuritychequesList[i].pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pContactreferenceid) + "', '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pTypeofsecurity) + "',  '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pBankname) + "',  '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pIfsccode) + "',  '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pAccountno) + "',  '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pChequeno) + "', '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath) + "','" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpathname) + "'," + getStatusid(SecurityandCollateralDTO.SecuritychequesList[i].pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                                }
                                else if (SecurityandCollateralDTO.SecuritychequesList[i].ptypeofoperation.ToUpper() == "UPDATE")
                                {
                                    if (SecurityandCollateralDTO.SecuritychequesList[i].pIsapplicable)
                                    {
                                        sbinsert.Append("update tabapplicationsecuritycollateralsecuritycheques set applicanttype='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pApplicanttype) + "', contactid= " + SecurityandCollateralDTO.SecuritychequesList[i].pContactid + ",contactreferenceid='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pContactreferenceid) + "', typeofsecurity='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pTypeofsecurity) + "', bankname='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pBankname) + "', ifsccode='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pIfsccode) + "', accountno='" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pAccountno) + "',chequeno= '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pChequeno) + "',securitychequesdocpath= '" + ManageQuote(SecurityandCollateralDTO.SecuritychequesList[i].pSecuritychequesdocpath) + "', statusid=" + getStatusid(SecurityandCollateralDTO.SecuritychequesList[i].pStatusname, connectionstring) + ", modifiedby= " + SecurityandCollateralDTO.pCreatedby + ", modifieddate=current_timestamp  where recordid=" + SecurityandCollateralDTO.SecuritychequesList[i].pRecordid + " and applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                                    }

                                }

                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralsecuritycheques set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " and recordid not in (" + Recordid + ");");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SecurityandCollateralDTO.SecuritychequesList.ToString()) || SecurityandCollateralDTO.SecuritychequesList.Count == 0 || (Recordid == "" && SecurityandCollateralDTO.SecuritychequesList.Count > 0))
                        {
                            sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralsecuritycheques set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                        }
                    }
                    Recordid = string.Empty;

                    if (SecurityandCollateralDTO.pisdepositslienapplicable == true)
                    {
                        if (SecurityandCollateralDTO.DepositsasLienList != null)
                        {
                            for (int i = 0; i < SecurityandCollateralDTO.DepositsasLienList.Count; i++)
                            {

                                if (SecurityandCollateralDTO.DepositsasLienList[i].pStatusname == null)
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pStatusname = "ACTIVE";
                                }
                                if (string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate))
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate = "null";
                                }
                                else
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate = "'" + FormatDate(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate) + "'";
                                }


                                if (string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate))
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate = "null";
                                }
                                else
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate = "'" + FormatDate(SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate) + "'";
                                }

                                if (string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList[i].pDepositamount.ToString()))
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pDepositamount = 0;
                                }
                                if (string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList[i].pRateofinterest.ToString()))
                                {
                                    SecurityandCollateralDTO.DepositsasLienList[i].pRateofinterest = 0;
                                }

                                if (SecurityandCollateralDTO.DepositsasLienList[i].ptypeofoperation.ToUpper() != "CREATE" || SecurityandCollateralDTO.DepositsasLienList[i].ptypeofoperation.ToUpper() == "OLD")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = SecurityandCollateralDTO.DepositsasLienList[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + SecurityandCollateralDTO.DepositsasLienList[i].pRecordid.ToString();
                                    }
                                }


                                if (SecurityandCollateralDTO.DepositsasLienList[i].ptypeofoperation.ToUpper() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationsecuritycollateraldepositslien(applicationid, vchapplicationid, contactid, contactreferenceid,depositin, typeofdeposit, depositorbank, depositaccountno, depositamount,rateofinterest, depositdate, tenureofdeposit, deposittenuretype,maturitydate, depositdocpath,filename, statusid, createdby, createddate)VALUES (" + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', " + SecurityandCollateralDTO.DepositsasLienList[i].pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pContactreferenceid) + "','" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositin) + "','" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pTypeofdeposit) + "', '" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositorbank) + "', '" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositaccountno) + "',coalesce(" + SecurityandCollateralDTO.DepositsasLienList[i].pDepositamount + ",0),coalesce(" + SecurityandCollateralDTO.DepositsasLienList[i].pRateofinterest + ",0), " + (SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate) + ", '" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pTenureofdeposit) + "','" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDeposittenuretype) + "', " + SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate + ", '" + ManageQuote(Convert.ToString(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath)) + "', '" + ManageQuote(Convert.ToString(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpathname)) + "', " + getStatusid(SecurityandCollateralDTO.DepositsasLienList[i].pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                                }
                                else if (SecurityandCollateralDTO.DepositsasLienList[i].ptypeofoperation.ToUpper() == "UPDATE")
                                {
                                    if (SecurityandCollateralDTO.DepositsasLienList[i].pIsapplicable)
                                    {
                                        sbinsert.Append("update tabapplicationsecuritycollateraldepositslien set depositin='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositin) + ", typeofdeposit='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pTypeofdeposit) + "', depositorbank='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositorbank) + "', depositaccountno='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDepositaccountno) + "', depositamount=,coalesce(" + SecurityandCollateralDTO.DepositsasLienList[i].pDepositamount + ",0),rateofinterest=coalesce(" + SecurityandCollateralDTO.DepositsasLienList[i].pRateofinterest + ",0), depositdate=" + SecurityandCollateralDTO.DepositsasLienList[i].pDepositdate + ", tenureofdeposit='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pTenureofdeposit) + "', deposittenuretype='" + ManageQuote(SecurityandCollateralDTO.DepositsasLienList[i].pDeposittenuretype) + "',maturitydate=" + SecurityandCollateralDTO.DepositsasLienList[i].pMaturitydate + ", depositdocpath='" + ManageQuote(Convert.ToString(SecurityandCollateralDTO.DepositsasLienList[i].pDepositdocpath)) + "', statusid= " + getStatusid(SecurityandCollateralDTO.DepositsasLienList[i].pStatusname, connectionstring) + ", modifiedby= " + SecurityandCollateralDTO.pCreatedby + ", modifieddate=current_timestamp  where recordid=" + SecurityandCollateralDTO.DepositsasLienList[i].pRecordid + " and applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                                    }

                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateraldepositslien set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " and recordid not in (" + Recordid + ");");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SecurityandCollateralDTO.DepositsasLienList.ToString()) || SecurityandCollateralDTO.DepositsasLienList.Count == 0 || (Recordid == "" && SecurityandCollateralDTO.DepositsasLienList.Count > 0))
                        {
                            sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateraldepositslien set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                        }
                    }
                    Recordid = string.Empty;
                    if (SecurityandCollateralDTO.pissecuritylienapplicable == true)
                    {
                        if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList != null)
                        {
                            for (int i = 0; i < SecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Count; i++)
                            {
                                if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pStatusname == null)
                                {
                                    SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pStatusname = "ACTIVE";
                                }
                                if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].ptypeofoperation.ToUpper() != "CREATE" || SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].ptypeofoperation.ToUpper() == "OLD")
                                {
                                    if (string.IsNullOrEmpty(Recordid))
                                    {
                                        Recordid = SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pRecordid.ToString();
                                    }
                                    else
                                    {
                                        Recordid = Recordid + "," + SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pRecordid.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pEstimatedvalue.ToString()))
                                {
                                    SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pEstimatedvalue = 0;
                                }
                                if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].ptypeofoperation.ToUpper() == "CREATE")
                                {
                                    sbinsert.Append("INSERT INTO tabapplicationsecuritycollateralotherpropertyorsecuritydetails(applicationid, vchapplicationid, contactid, contactreferenceid,nameofthesecurity, estimatedvalue, securitydocpath,filename, statusid,createdby, createddate)VALUES (" + SecurityandCollateralDTO.pApplicationid + ",'" + ManageQuote(SecurityandCollateralDTO.pVchapplicationid) + "', " + SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pContactid + ", '" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pContactreferenceid) + "', '" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pNameofthesecurity) + "',coalesce(" + SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pEstimatedvalue + ",0), '" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath) + "','" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpathname) + "'," + getStatusid(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pStatusname, connectionstring) + ", " + SecurityandCollateralDTO.pCreatedby + ", current_timestamp);");
                                }
                                else if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].ptypeofoperation.ToUpper() == "UPDATE")
                                {
                                    if (SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pIsapplicable)
                                    {
                                        sbinsert.Append("update tabapplicationsecuritycollateralotherpropertyorsecuritydetails set nameofthesecurity='" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pNameofthesecurity) + "', estimatedvalue=coalesce(" + SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pEstimatedvalue + ",0), securitydocpath='" + ManageQuote(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pSecuritydocpath) + "', statusid=" + getStatusid(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pStatusname, connectionstring) + ", modifiedby= " + SecurityandCollateralDTO.pCreatedby + ", modifieddate=current_timestamp  where recordid=" + SecurityandCollateralDTO.otherPropertyorsecurityDetailsList[i].pRecordid + " and applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                                    }

                                }

                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Recordid))
                    {
                        sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralotherpropertyorsecuritydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + " and recordid not in (" + Recordid + ");");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SecurityandCollateralDTO.otherPropertyorsecurityDetailsList.ToString()) || SecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Count == 0 || (Recordid == "" && SecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Count > 0))
                        {
                            sbinsertfirst.Append("UPDATE tabapplicationsecuritycollateralotherpropertyorsecuritydetails set statusid=" + getStatusid("In-Active", connectionstring) + ",modifiedby=" + SecurityandCollateralDTO.pCreatedby + ",modifieddate=current_timestamp where applicationid=" + SecurityandCollateralDTO.pApplicationid + ";");
                        }
                    }

                    Recordid = string.Empty;
                }
                if (!string.IsNullOrEmpty(sbinsert.ToString()))
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsertfirst + "" + sbinsert.ToString());
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

        public ApplicationSecurityandCollateralDTO getSecurityCollateralDetails(long applicationid, string strapplicationid, string ConnectionString)
        {

            try
            {

                ApplicationSecurityandCollateralDTO = new ApplicationSecurityandCollateralDTO();

                bool issecurityandcolletralapplicable = Convert.ToBoolean(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select issecurityandcolletralapplicable from tabapplication where vchapplicationid = '" + strapplicationid + "';"));
                ApplicationSecurityandCollateralDTO.pissecurityandcolletralapplicable = issecurityandcolletralapplicable;

                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT  coalesce(isimmovablepropertyapplicable,false) as isimmovablepropertyapplicable,coalesce(ismovablepropertyapplicable,false) as ismovablepropertyapplicable,coalesce(issecuritychequesapplicable,false) as issecuritychequesapplicable,coalesce(isdepositslienapplicable,false) as isdepositslienapplicable,coalesce(issecuritylienapplicable,false) as issecuritylienapplicable  from tabapplicationsecurityapplicablesections t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE'  and vchapplicationid='" + strapplicationid + "'; "))
                {
                    while (dr.Read())
                    {
                        ApplicationSecurityandCollateralDTO.pIsimmovablepropertyapplicable = Convert.ToBoolean(dr["isimmovablepropertyapplicable"]);
                        ApplicationSecurityandCollateralDTO.pismovablepropertyapplicable = Convert.ToBoolean(dr["ismovablepropertyapplicable"]);
                        ApplicationSecurityandCollateralDTO.pissecuritychequesapplicable = Convert.ToBoolean(dr["issecuritychequesapplicable"]);
                        ApplicationSecurityandCollateralDTO.pisdepositslienapplicable = Convert.ToBoolean(dr["isdepositslienapplicable"]);
                        ApplicationSecurityandCollateralDTO.pissecuritylienapplicable = Convert.ToBoolean(dr["issecuritylienapplicable"]);
                    }
                }

                if (issecurityandcolletralapplicable == true)
                {

                    ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList = new List<ApplicationSecurityandCollateralImMovablePropertyDetails>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,contactid,contactreferenceid,typeofproperty,titledeed,deeddate,propertyownername,addressofproperty,coalesce(estimatedmarketvalue,0) as estimatedmarketvalue,propertydocpath,filename from tabapplicationsecuritycollateralimmovablepropertydetails t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE'  and vchapplicationid='" + strapplicationid + "'; "))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.ImMovablePropertyDetailsList.Add(new ApplicationSecurityandCollateralImMovablePropertyDetails
                            {

                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pTypeofproperty = Convert.ToString(dr["typeofproperty"]),
                                pTitledeed = Convert.ToString(dr["titledeed"]),
                                pDeeddate = Convert.ToString(dr["deeddate"]),
                                pPropertyownername = Convert.ToString(dr["propertyownername"]),
                                pAddressofproperty = Convert.ToString(dr["addressofproperty"]),
                                pEstimatedmarketvalue = Convert.ToDecimal(dr["estimatedmarketvalue"]),
                                pPropertydocpath = Convert.ToString(dr["propertydocpath"]),
                                pPropertydocpathname = Convert.ToString(dr["filename"]),
                                ptypeofoperation = "OLD"

                            });
                        }
                    }
                    ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList = new List<ApplicationSecurityandCollateralMovablePropertyDetails>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT 	recordid,contactid,contactreferenceid,typeofvehicle,vehicleownername,vehiclemodelandmake,registrationno,coalesce(estimatedmarketvalue,0) as estimatedmarketvalue,vehicledocpath,filename from tabapplicationsecuritycollateralmovablepropertydetails  t1 join tblmststatus t2 on t1.statusid=t2.statusid where upper(t2.statusname)='ACTIVE' and vchapplicationid='" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.MovablePropertyDetailsList.Add(new ApplicationSecurityandCollateralMovablePropertyDetails
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                PTypeofvehicle = Convert.ToString(dr["typeofvehicle"]),
                                pVehicleownername = Convert.ToString(dr["vehicleownername"]),
                                pVehiclemodelandmake = Convert.ToString(dr["vehiclemodelandmake"]),
                                pRegistrationno = Convert.ToString(dr["registrationno"]),
                                pEstimatedmarketvalue = Convert.ToDecimal(dr["estimatedmarketvalue"]),
                                pVehicledocpath = Convert.ToString(dr["vehicledocpath"]),
                                pVehicledocpathname = Convert.ToString(dr["filename"]),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }


                    ApplicationSecurityandCollateralDTO.SecuritychequesList = new List<ApplicationSecurityandCollateralSecuritycheques>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid, applicanttype, contactid, contactreferenceid, typeofsecurity, bankname, ifsccode, accountno, chequeno,securitychequesdocpath,filename from tabapplicationsecuritycollateralsecuritycheques t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE' and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.SecuritychequesList.Add(new ApplicationSecurityandCollateralSecuritycheques
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pApplicanttype = Convert.ToString(dr["applicanttype"]),
                                pTypeofsecurity = Convert.ToString(dr["typeofsecurity"]),
                                pBankname = Convert.ToString(dr["bankname"]),
                                pIfsccode = Convert.ToString(dr["ifsccode"]),
                                pAccountno = Convert.ToString(dr["accountno"]),
                                pChequeno = Convert.ToString(dr["chequeno"]),
                                pSecuritychequesdocpath = Convert.ToString(dr["securitychequesdocpath"]),
                                pSecuritychequesdocpathname = Convert.ToString(dr["filename"]),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }


                    ApplicationSecurityandCollateralDTO.DepositsasLienList = new List<ApplicationSecurityandCollateralDepositsasLien>();
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,contactid,contactreferenceid,depositin,typeofdeposit,depositorbank,depositaccountno,coalesce(depositamount,0) as depositamount,coalesce(rateofinterest,0) as rateofinterest,depositdate,tenureofdeposit,deposittenuretype,maturitydate,depositdocpath,filename from tabapplicationsecuritycollateraldepositslien  t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE'  and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.DepositsasLienList.Add(new ApplicationSecurityandCollateralDepositsasLien
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pDepositin = Convert.ToString(dr["depositin"]),
                                pTypeofdeposit = Convert.ToString(dr["typeofdeposit"]),
                                pDepositorbank = Convert.ToString(dr["depositorbank"]),
                                pDepositaccountno = Convert.ToString(dr["depositaccountno"]),
                                pDepositamount = Convert.ToDecimal(dr["depositamount"]),
                                pRateofinterest = Convert.ToDecimal(dr["rateofinterest"]),
                                pDepositdate = Convert.ToString(dr["depositdate"]),
                                pTenureofdeposit = Convert.ToString(dr["tenureofdeposit"]),
                                pDeposittenuretype = Convert.ToString(dr["deposittenuretype"]),
                                pMaturitydate = Convert.ToString(dr["maturitydate"]),
                                pDepositdocpath = Convert.ToString(dr["depositdocpath"]),
                                pDepositdocpathname = Convert.ToString(dr["filename"]),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }

                    ApplicationSecurityandCollateralDTO.otherPropertyorsecurityDetailsList = new List<ApplicationSecurityandCollateralOtherPropertyorsecurityDetails>();

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT recordid,contactid,contactreferenceid,nameofthesecurity,coalesce(estimatedvalue,0) as estimatedvalue, securitydocpath,filename from tabapplicationsecuritycollateralotherpropertyorsecuritydetails  t1 join tblmststatus t2 on t1.statusid = t2.statusid where upper(t2.statusname) = 'ACTIVE'  and vchapplicationid = '" + strapplicationid + "';"))
                    {
                        while (dr.Read())
                        {
                            ApplicationSecurityandCollateralDTO.otherPropertyorsecurityDetailsList.Add(new ApplicationSecurityandCollateralOtherPropertyorsecurityDetails
                            {
                                pIsapplicable = true,
                                pRecordid = Convert.ToInt64(dr["recordid"]),
                                pContactid = Convert.ToInt64(dr["contactid"]),
                                pContactreferenceid = Convert.ToString(dr["contactreferenceid"]),
                                pNameofthesecurity = Convert.ToString(dr["nameofthesecurity"]),
                                pEstimatedvalue = Convert.ToDecimal(dr["estimatedvalue"]),
                                pSecuritydocpath = Convert.ToString(dr["securitydocpath"]),
                                pSecuritydocpathname = Convert.ToString(dr["filename"]),
                                ptypeofoperation = "OLD"
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return ApplicationSecurityandCollateralDTO;
        }


    }
}
