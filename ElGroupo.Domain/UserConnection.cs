using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class UserConnection: ClassBase
    {
        //[Required]
        //public long UserId { get; set; }

        [Required]
        public User User { get; set; }

        //[Required]
        //public long ConnectedUserId { get; set; }
        [Required]
        public User ConnectedUser { get; set; }
    }
}
