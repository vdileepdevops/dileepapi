using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Transactions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaRepository.DataAccess.Loans.Transactions
{
    public class PreclosureDAL : SettingsDAL, IPreclosure
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
    }
}
