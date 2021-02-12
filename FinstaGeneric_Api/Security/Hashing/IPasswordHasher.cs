using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinstaApi.Security.Hashing
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool PasswordMatches(string providedPassword, string passwordHash);
        string encrypt(string password);
        string Decrypt(string password);

    }
}
