using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinstaApi.Common
{
    public class FinstaAppException : Exception
    {

        public FinstaAppException()
        { }

        public FinstaAppException(string message)
            : base(message)
        { }

        public FinstaAppException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
