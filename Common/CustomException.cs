using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CustomException : Exception
    {
        public CustomException(string message, int status = 400) : base(message) => StatusCode = status;
        public int StatusCode { get; }
    }
}
