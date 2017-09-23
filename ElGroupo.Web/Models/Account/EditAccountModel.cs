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
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public bool IsAdminEditing { get; set; }
        public List<EditContactModel> Contacts { get; set; }

        public List<UserConnectionModel> Connections { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

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

    }



    public class EditContactModel
    {
        public long Id { get; set; }
        public string ContactTypeDescription { get; set; }
        public long ContactTypeId { get; set; }
        public string Value { get; set; }
    }
}
