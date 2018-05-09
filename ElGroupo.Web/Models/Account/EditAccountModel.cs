using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class EditAccountModel
    {
        public bool ShowSaveConfirmation { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string apiKey { get; set; }
        public bool IsAdminEditing { get; set; }
        public List<EditContactModel> Contacts { get; set; }

        public List<string> Titles
        {
            get
            {
                return new List<string> { "Mr", "Ms", "Mrs", "Dr", "", "Sir", "Lady" };
            }
        }

        public List<UserConnectionModel> Connections { get; set; }

                public List<AttendeeGroupListModel> AttendeeGroups { get; set; }
        public long Id { get; set; }

        public string Title { get; set; }
        public string Specialty { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Description = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


        [Display(Description = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile UpdatedPhoto { get; set; }

        [Display(Description = "Zip Code")]
        [Required]
        public string ZipCode { get; set; }

        public bool HasPhoto { get; set; }

        public Dictionary<long, string> ContactTypes { get; set; }

        public string TimeZoneId { get; set; }
        public Dictionary<string, string> TimeZones { get; set; }

    }



    public class EditContactModel
    {
        public long Id { get; set; }
        public string ContactTypeDescription { get; set; }
        public long ContactTypeId { get; set; }
        public string Value { get; set; }
    }
}
