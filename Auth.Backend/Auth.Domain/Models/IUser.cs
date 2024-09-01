using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.Models
{
    public interface IUser
    {
        string? FirstName { get; set; }
        string? LastName { get; set; }
        DateTime? Birthdate { get; set; }
        string? Refresh_Token { get; set; }
        string? JWT_Token { get; set; }
        string? Expires_At { get; set; }
    }
}