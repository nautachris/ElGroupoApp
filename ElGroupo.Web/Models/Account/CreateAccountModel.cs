using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ElGroupo.Web.Models.Account
{
    public class CreateAccountModel
    {
        public List<OrganizationListModel> Organizations { get; set; } = new List<OrganizationListModel>();

        public int OrganizationId { get; set; }
        public bool InvitedFromEvent { get; set; }
        public string InviteName { get; set; }
        public string EventName { get; set; }

        public string InvitedEmail { get; set; }
        public Guid? InviteId { get; set; }

        public List<string> Titles
        {
            get
            {
                return new List<string> { "Mr", "Ms", "Mrs", "Dr", "", "Sir", "Lady" };
            }
        }
        public string Title { get; set; }

        [Display(Description = "User Name")]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Description = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Description ="Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Description = "Zip Code")]
        [Required]
        public string ZipCode { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
    }
}
