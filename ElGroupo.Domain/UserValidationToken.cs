using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Domain
{
    public class UserValidationToken:ClassBase
    {
        public User User { get; set; }
        public Guid Token { get; set; }
        public TokenTypes TokenType { get; set; }
    }
}
