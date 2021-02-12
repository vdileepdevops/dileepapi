using FinstaInfrastructure.TDS;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.TDS;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.TDS
{
   public class TdsReportDAL: SettingsDAL, ITdsReport
    {
        public async Task<List<TdsReportDTO>> GetTdsReportDetails(string Connectionstring,string FromDate,string ToDate,string SectionName)
        {
            List<TdsReportDTO> TdsReportDetailsList = new List<TdsReportDTO>();
            await Task.Run(() =>
            {
                try
                {
                    //using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select * from tds_vouchers where section ='" + SectionName + "' and trans_date between '" + FormatDate(FromDate) + "' and '" +FormatDate(ToDate) + "';"))
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select parentaccountname,parent_id,account_id,accountname,panno,amount,cal_tds,actual_tds,balance,trans_date,section,section_name,paid_amount,c.jvnumber as jv_id from tds_vouchers a join tblmstaccounts b on a.account_id=b.accountid left join tbltransjournalvoucher c on c.recordid=a.jv_id where section ='" + SectionName + "' and trans_date between '" + FormatDate(FromDate) + "' and '" +FormatDate(ToDate) + "';"))
                    {
                        while (dr.Read())
                        {
                            var TdsReportDetails = new TdsReportDTO
                            {


                                pParentId=dr["parent_id"],
                                pParentAccountName=dr["parentaccountname"],
                                pAccountId = dr["account_id"],
                                pAccountName = dr["accountname"],
                                pPanNo = dr["panno"],
                                pAmount = dr["amount"],
                                pTdsAmount = dr["cal_tds"],
                                pActualTdsAmount = dr["actual_tds"],
                                pBalance = dr["balance"],
                                pTransDate = Convert.ToDateTime(dr["trans_date"]).ToString("dd/MMM/yyyy"),
                                pTdsSection = dr["section"],
                                pSectionName = dr["section_name"],
                                pPaidAmount = dr["paid_amount"],
                                pJvNo = dr["jv_id"]
                            };
                            TdsReportDetailsList.Add(TdsReportDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
            return TdsReportDetailsList;
        }
    }
}
