using System;
using System.Collections.Generic;
using System.Linq;
namespace Auth.Domain.Token
{
    public class SecurityTokenException : Exception
    {
        public SecurityTokenException(string message) : base(message) { }
    }
}
