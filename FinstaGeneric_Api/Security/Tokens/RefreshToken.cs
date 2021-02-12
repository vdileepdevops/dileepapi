using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinstaApi.Security.Tokens
{
    public class RefreshToken : JsonWebToken
    {
        public RefreshToken(string token, long expiration) : base(token, expiration)
        {
        }
    }
}
