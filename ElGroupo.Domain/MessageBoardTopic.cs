using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class MessageBoardTopic:ClassBase
    {
        [Required]
        public User StartedBy { get; set; }
        public DateTime StartedDate { get; set; }
        public string Subject { get; set; }

        [Required]
        public virtual Event Event { get; set; }
        public virtual ICollection<MessageBoardItem> Messages { get; set; }
    }
}
