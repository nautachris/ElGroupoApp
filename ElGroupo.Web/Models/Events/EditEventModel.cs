using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ElGroupo.Web.Models.Events
{
    public class EditEventModel
    {
        public long Id { get; set; }


        [Required]
        [Display(Description = "Event Name")]
        public string Name { get; set; }

        [Display(Description = "Event Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string LocationName { get; set; }

        [Required]
        [Display(Description = "Event Date")]
        public DateTime EventDate { get; set; }
        [Required]
        [Display(Description = "Start Hour")]
        public int StartHour { get; set; }
        [Required]

        [Display(Description = "Start Minute")]
        public int StartMinute { get; set; }
        [Required]
        public Enums.AMPM StartAMPM { get; set; }
        [Required]
        [Display(Description = "End Hour")]
        public int EndHour { get; set; }
        [Required]
        [Display(Description = "End Minute")]
        public int EndMinute { get; set; }
        [Required]
        public Enums.AMPM EndAMPM { get; set; }

        [Required]
        [Display(Description = "Address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        [Display(Description = "City")]
        public string City { get; set; }

        [Required]
        [Display(Description = "State")]
        public string State { get; set; }
        public string ZipCode { get; set; }

        [Required]
        public double XCoord { get; set; }

        [Required]
        public double YCoord { get; set; }

        public string GooglePlaceId { get; set; }
    }
}
