using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ElGroupo.Domain.Enums
{
    [Flags]
    public enum DaysOfWeek
    {
        None = 0,
        [Display(Description = "M")]
        Monday = 1,
        [Display(Description = "T")]
        Tuesday = 2,
        [Display(Description = "W")]
        Wednesday = 4,
        [Display(Description = "Th")]
        Thursday = 8,
        [Display(Description = "F")]
        Friday = 16,
        [Display(Description = "Sa")]
        Saturday = 32,
        [Display(Description = "Su")]
        Sunday = 64
    }
}
