using FinstaInfrastructure.Settings.Users;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings.Users;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FinstaRepository;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace FinstaRepository.DataAccess.Settings.Users
{
    public class UserAccessDAL : SettingsDAL, IUserAccess
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        List<ContactEmployeeDTO> lstContactEmployeeDTO { set; get; }
        List<UserAccessDTO> lstUserAccessDTO { set; get; }

        public async Task<int> CheckUserName(string UserName, string connectionString)
        {

            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstusers  where upper(username)='" + ManageQuote(UserName.ToUpper().Trim()) + "';").ToString());
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return count;

        }
        public async Task<int> CheckUsercontactRefID(string Contactrefid, string connectionString)
        {

            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstusers  where upper(contactrefid)='" + ManageQuote(Contactrefid.ToUpper().Trim()) + "';").ToString());
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return count;

        }
        public UserAccessDTO CheckUser(string UserName, string PassWord, string connectionString)
        {


            UserAccessDTO userDTO = new UserAccessDTO();

            try
            {
                string Query = "select * from tblmstusers  where upper(username)='" + ManageQuote(UserName.ToUpper().Trim()) + "' and statusid=1";
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
                {
                    while (dr.Read())
                    {
                        userDTO.pUserID = Convert.ToInt64(dr["userid"]);
                        userDTO.pUserName = dr["username"].ToString();
                        userDTO.pContactRefID = dr["contactrefid"].ToString();
                        userDTO.pRoleid = Convert.ToInt64(dr["roleid"]);
                        userDTO.pSaltKey = dr["saltkey"].ToString();
                        userDTO.pPassword = dr["password"].ToString();
                        userDTO.pOtpAuthentication = Convert.ToBoolean(dr["otp_authentication"]);
                        userDTO.pMobile = dr["mobileno"].ToString();
                        userDTO.pEmail = dr["email"].ToString();
                    }
                }

            }

            catch (Exception ex)
            {

                throw ex;
            }

            return userDTO;
        }

        public async Task<List<ContactEmployeeDTO>> GetContactDetails(string ConnectionString)
        {
            lstContactEmployeeDTO = new List<ContactEmployeeDTO>();
            await Task.Run(() =>
            {
                try
                {
                    string Query = "select t1.contactid,t1.contactreferenceid,t2.tbl_mst_employee_id,coalesce(t2.role_id,0) as roleid,t1.name,(select rolename  from tblmstemployeerole where roleid=t2.role_id) from   tblmstcontact t1 join tbl_mst_employee t2 on t1.contactid=t2.contact_id where t1.statusid=1 and  t1.contactreferenceid not in(select coalesce(contactrefid,'') as contactrefid from tblmstusers);";
                    //string Query = "select x.*,coalesce(y.rolename,'') as rolename,coalesce(y.roleid,0) as roleid from (select t1.contactid,t1.contactreferenceid,t2.employeeid,t2.name from   tblmstcontact t1 join tblmstemployee t2 on t1.contactid=t2.contactid where t1.statusid=" + Convert.ToInt32(Status.Active) + ") x join tblmstemployeeemploymentdetails y on x.employeeid=y.employeeid where x.contactreferenceid not in(select coalesce(contactrefid,'') as contactrefid from tblmstusers);";

                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            ContactEmployeeDTO ContactEmployeeDTO = new ContactEmployeeDTO();
                            ContactEmployeeDTO.pContactID = Convert.ToInt64(dr["contactid"]);
                            ContactEmployeeDTO.pContactRefID = dr["contactreferenceid"].ToString();
                            ContactEmployeeDTO.pEmployeeid = Convert.ToInt64(dr["tbl_mst_employee_id"]);
                            ContactEmployeeDTO.pEmployeeName = dr["name"].ToString();
                            ContactEmployeeDTO.pRoleid = Convert.ToInt64(dr["roleid"]);
                            ContactEmployeeDTO.pRoleName = Convert.ToString(dr["rolename"]);
                            lstContactEmployeeDTO.Add(ContactEmployeeDTO);
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstContactEmployeeDTO;
        }
        public async Task<string> GetRoleName(long EmployeeID, string ConnectionString)
        {
            string RoleName = string.Empty;
            await Task.Run(() =>
            {
                return RoleName = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select rolename from tblmstemployeeemploymentdetails  where employeeid=" + EmployeeID + " and statusid=1;"));
            });
            return RoleName;
        }
        public bool SaveUserAccess(UserAccessDTO UserAccessDTO, string connectionString)
        {
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;

            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(UserAccessDTO.pRoleid.ToString()))
                {
                    UserAccessDTO.pRoleid = 0;
                }

                sbinsert.Append("INSERT INTO tblmstusers(username,password, statusid,usertype,designation,createdby,createddate,saltkey,roleid,contactrefid,employeename )VALUES ('" + ManageQuote(UserAccessDTO.pUserName.Trim()) + "','" + UserAccessDTO.pPassword + "'," + Convert.ToInt32(Status.Active) + ",'" + ManageQuote(UserAccessDTO.pUserType) + "','" + ManageQuote(UserAccessDTO.pRoleName) + "'," + UserAccessDTO.pCreatedby + ",current_timestamp,'" + UserAccessDTO.pSaltKey + "',coalesce(" + UserAccessDTO.pRoleid + ",0),'" + ManageQuote(UserAccessDTO.pContactRefID) + "','" + ManageQuote(UserAccessDTO.pEmployeeName) + "');");

                if (Convert.ToString(sbinsert) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
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

        public async Task<List<UserAccessDTO>> GetAllUsersView(string connectionString)
        {
            lstUserAccessDTO = new List<UserAccessDTO>();
            await Task.Run(() =>
            {
                try
                {
                    // string Query = "select x.userid,coalesce(x.employeename,x.username) as employeename,coalesce(x.designation,'') as designation,coalesce(x.roleid,0) as roleid,coalesce(x.contactrefid,'') as contactrefid,x.username,x.usertype,x.statusid,y.statusname from tblmstusers x left join tblmststatus y on x.statusid=y.statusid order by x.userid;";
                    string Query = "select m.*,coalesce(n.count,0) as count from (select x.userid,coalesce(x.employeename,x.username) as employeename,coalesce(x.designation,'') as designation,coalesce(x.roleid,0) as roleid,coalesce(x.contactrefid,'') as contactrefid,x.username,x.usertype,x.statusid,y.statusname from tblmstusers x left join tblmststatus y on x.statusid=y.statusid order by x.userid) m left join(select  coalesce(userid,0) as userid,count(*) as count from tblmstrolefunctions group by userid) n on m.userid=n.userid where upper(m.employeename) not in('ADMIN');";
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
                    {
                        while (dr.Read())
                        {
                            UserAccessDTO UserAccessDTO = new UserAccessDTO();
                            UserAccessDTO.pUserID = Convert.ToInt64(dr["userid"]);
                            UserAccessDTO.pEmployeeName = dr["employeename"].ToString();
                            UserAccessDTO.pUserName = dr["username"].ToString();
                            UserAccessDTO.pRoleName = dr["designation"].ToString();
                            UserAccessDTO.pRoleid = Convert.ToInt64(dr["roleid"]);
                            UserAccessDTO.pRoleFunctionsCOunt = Convert.ToInt32(dr["count"]);
                            if (UserAccessDTO.pRoleid != 0 && UserAccessDTO.pRoleFunctionsCOunt == 0)
                            {
                                UserAccessDTO.PUserorDesignation = "Designation";
                            }
                            else
                            {
                                UserAccessDTO.PUserorDesignation = "User";
                            }
                            UserAccessDTO.pUserType = dr["usertype"].ToString();
                            UserAccessDTO.pstatusid = Convert.ToInt32(dr["statusid"]);
                            if (dr["statusname"].ToString() == "Active")
                            {
                                UserAccessDTO.pActiveorInactive = true;
                            }
                            if (dr["statusname"].ToString() == "In-Active")
                            {
                                UserAccessDTO.pActiveorInactive = false;
                            }
                            UserAccessDTO.pStatus = dr["statusname"].ToString();
                            lstUserAccessDTO.Add(UserAccessDTO);
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return lstUserAccessDTO;
        }



        public bool UpdateuserPassword(string Username, string password, string connectionString)
        {

            bool IsSaved = false;

            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string PassQuery = "update tblmstusers set password='" + password + "' where upper(username)='" + Username.Trim().ToUpper() + "' and statusid=1";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, PassQuery);

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
        public async Task<bool> DeleteUser(long Userid, bool status, string connectionString)
        {
            bool Issaved = false;
            int statusid = 0;
            await Task.Run(() =>
            {
                try
                {
                    if (Userid != 0)
                    {
                        if (status == true || status == false)
                        {
                            if (status == false)
                            {
                                statusid = Convert.ToInt32(Status.Inactive);
                            }
                            if (status == true)
                            {
                                statusid = Convert.ToInt32(Status.Active); ;
                            }
                        }
                    }
                    if (statusid != 0)
                    {
                        NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "update tblmstusers set statusid=" + statusid + " where userid=" + Userid + ";");
                        Issaved = true;
                    }


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
            return Issaved;


        }

        public string GetDeafultPassword(string ConnectionString)
        {
            string Password = string.Empty;

            try
            {
                Password = Convert.ToString(NPGSqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select coalesce(defaultpassword,'') as defaultpassword from tblmstcompany;"));
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Password;
        }

        #region OTP Authentication
        public bool SendOTP(string connectionString, string strMobileNo, string struserid, string email)
        {

            bool IssendOtp = false;
            string strInsertOTP = string.Empty;
            string strCheckOTP = string.Empty;
            string strUpdateOTP = string.Empty;
            string strOTP = string.Empty;
            string strSmsMessage = string.Empty;
            string strMailMessage = string.Empty;
            string strSmtpClient = string.Empty;
            string strFromMail = string.Empty;
            string strFromMailPassword = string.Empty;
            StringBuilder strinsert = new StringBuilder();
            int OTPCount = 0;

            try
            {
                if (!string.IsNullOrEmpty(strMobileNo))
                {
                    OTPCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tabwebotp where mobile='" + strMobileNo + "' and Userid='" + struserid + "' and isactive='Y' "));
                    if (OTPCount == 0)
                    {
                        strOTP = OTPGenerator();
                        strInsertOTP = "INSERT INTO tabwebotp(Userid,mobile,otp,senddatetime,isactive,statusid) VALUES ('" + struserid + "','" + strMobileNo + "','" + strOTP + "',current_timestamp,'Y',1);";
                        NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, strInsertOTP);

                    }
                    else
                    {
                        strCheckOTP = "select senddatetime from tabwebotp where mobile='" + strMobileNo + "' and Userid='" + struserid + "' and isactive='Y';";

                        DateTime otpsentdate = Convert.ToDateTime(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, strCheckOTP));

                        TimeSpan diff = DateTime.Now - otpsentdate;

                        if (diff.Minutes >= 15)
                        {
                            strOTP = OTPGenerator();
                            strinsert.Append("update tabwebotp set isactive='N' where mobile='" + strMobileNo + "' and Userid='" + struserid + "' and isactive='Y';");
                            strinsert.Append("INSERT INTO tabwebotp(Userid,mobile,otp,senddatetime,isactive,statusid) VALUES ('" + struserid + "','" + strMobileNo + "','" + strOTP + "',current_timestamp,'Y',1);");
                            NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, strinsert.ToString());
                        }
                        else
                        {
                            string otpavail = "select otp from tabwebotp where mobile='" + strMobileNo + "' and Userid='" + struserid + "' and isactive='Y';";

                            strOTP = Convert.ToString(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, otpavail));
                        }
                    }
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select otp_authentication,sendotpemail_applicable,sendotpsms_applicable,employeename from tblmstusers where  userid=" + struserid + ""))
                    {
                        if (dr.Read())
                        {
                            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select * from fn_getotpdetails('" + strOTP + "','" + strMobileNo + "','" + dr["employeename"] + "','" + email + "')"))
                            {
                                if (dr1.Read())
                                {
                                    strSmsMessage = Convert.ToString(dr1["Sms_Message"]);
                                    strMailMessage = Convert.ToString(dr1["Mail_Message"]);
                                    strSmtpClient = Convert.ToString(dr1["Smtp_Client"]);
                                    strFromMail = Convert.ToString(dr1["From_mail"]);
                                    strFromMailPassword = Convert.ToString(dr1["From_mail_password"]);
                                }
                            }
                            if (Convert.ToBoolean(dr["sendotpsms_applicable"]))
                            {
                                Thread Messagethread = new Thread(() => SendMessage(strOTP, strMobileNo, strSmsMessage));
                                Messagethread.Start();
                            }
                            //if (Convert.ToBoolean(dr["sendotpemail_applicable"]))
                            //{
                            //    Thread emailthread = new Thread(() => SendEmail(strOTP, Convert.ToString(dr["employeename"]), email, strMailMessage, strSmtpClient, strFromMail, strFromMailPassword));
                            //    emailthread.Start();
                            //}
                        }
                    }

                }

                IssendOtp = true;
            }

            catch (Exception ex)
            {
                IssendOtp = false;
                throw ex;
            }

            return IssendOtp;
        }
        public void SendMessage(string strOTP, string strMobileNo, string strSmsMessage)
        {
            string strMessage = string.Empty;
            // strMessage = "Your verification code is : " + strOTP;
            // String url = strSmsMessage;
            WebClient wc = new WebClient();
            Stream responseStream = wc.OpenRead(strSmsMessage);
            StreamReader sr = new StreamReader(responseStream);
            string responseString = sr.ReadToEnd();
            sr.Close();
            responseStream.Close();
        }

        public void SendEmail(string strOTP, string employeename, string email, string strMailMessage, string strSmtpClient, string strFromMail, string strFromMailPassword)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(strSmtpClient);
            mail.From = new MailAddress(strFromMail);
            mail.To.Add(email);
            mail.Subject = "One-time password (OTP) for LAND ADVANCE ";
            mail.IsBodyHtml = true;
            // string htmlBody;
            // string message;
            //message = "Your OTP is " + strOTP + ". The OTP is valid for 15 minutes and will be used for verification";
            //htmlBody = " <body style='background:#f9f9f9;'><table style='margin: 100px auto 15px auto; color: #696666; font-size: 20px; font-family: Arial, sans-serif;'><tr><td> Dear " + employeename + "</td></tr></table><table width='600px' style='margin: 0 auto; text-align: center; border: 1px solid #dadada; background: #ffffff; padding: 30px;'><tr><th style='padding-bottom: 50px;'><img src='http://202.53.15.13:9014/assets/images/Leadfront.png'></th></tr><tr><td style='font-size: 16px; color: #696666; font-family: Arial, sans-serif;'>" + message + "</td></tr><tr><td style='padding: 20px 0;'></td></tr></table><table style='margin: 20px auto 15px auto; text-align: left; color: #696666; font-size: 11px; font-family: Arial, sans-serif;'><tr><td style='text-align: center;'>This email was been sent by Admin, (LeadFront)</td></tr><tr><td>This is an automatically generated email, please do not reply.</td></tr></table></body>";
            mail.Body = strMailMessage;
            SmtpServer.Port = 587;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("rajendar1210@gmail.com", "01234@56789");
            SmtpServer.Credentials = new System.Net.NetworkCredential(strFromMail, strFromMailPassword);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
        public String OTPGenerator()
        {
            string OTP = string.Empty;
            try
            {
                Random objran = new Random();
                OTP = objran.Next(1000000).ToString();
                if (OTP.Length != 6)
                {
                    OTP = OTPGenerator();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return OTP;
        }

        public ValidateResponse ValidateOTP(ValidateDTO objValidateOTP, string connectionString)
        {
            string strQuery = string.Empty;
            string updateotp = string.Empty;
            string updateactivation = string.Empty;
            string strOTPStatus = string.Empty;
            int OTPCount = 0;
            DateTime otpdate = new DateTime();
            DataSet ds = new DataSet();
            ValidateResponse objValidateResponse = new ValidateResponse();
            DataSet dataset = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                try
                {

                    OTPCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(otp) from tabwebotp where Userid='" + objValidateOTP.pUserId + "' and mobile='" + objValidateOTP.pMobile + "' and otp='" + objValidateOTP.pOtp + "';"));
                    if (OTPCount != 0)
                    {
                        strQuery = "select otp,senddatetime from tabwebotp where Userid='" + objValidateOTP.pUserId + "' and mobile='" + objValidateOTP.pMobile + "' and otp='" + objValidateOTP.pOtp + "';";
                        using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, strQuery))
                        {
                            if (dr.Read())
                            {
                                otpdate = Convert.ToDateTime(dr["senddatetime"]);
                            }
                        }




                        TimeSpan diff = DateTime.Now - otpdate;

                        if (diff.Minutes >= 15)
                        {
                            objValidateResponse.status = false;
                            objValidateResponse.message = "OTP has Expired";
                        }
                        else
                        {
                            strOTPStatus = "select isactive from tabwebotp where otp='" + objValidateOTP.pOtp + "';";

                            strOTPStatus = Convert.ToString(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, strOTPStatus));
                            if (strOTPStatus == "N")
                            {
                                objValidateResponse.status = false;
                                objValidateResponse.message = "OTP has Expired";
                            }
                            else
                            {

                                updateotp = "update tabwebotp set isactive='N' where Userid='" + objValidateOTP.pUserId + "'";
                                NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, updateotp);
                                objValidateResponse.status = true;
                                objValidateResponse.message = "success";

                            }
                        }

                    }
                    else
                    {
                        objValidateResponse.status = false;
                        objValidateResponse.message = "Invalid OTP";
                    }
                }
                catch (Exception ex)
                {
                    objValidateResponse.status = false;
                    objValidateResponse.message = "Try Again";
                    Console.WriteLine(ex.Message);
                }


            }
            catch (Exception ex)
            {
                objValidateResponse.status = false;
                objValidateResponse.message = "Server Issue";
                //throw ex;
            }

            return objValidateResponse;

        }
        #endregion
    }
}