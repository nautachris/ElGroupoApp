using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class ViewAccountModel
    {

        public List<EditContactModel> Contacts { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Description = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


        [Display(Description = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }



        [Display(Description = "Zip Code")]
        [Required]
        public string ZipCode { get; set; }


        public Dictionary<long, string> ContactTypes { get; set; }

    }



}
