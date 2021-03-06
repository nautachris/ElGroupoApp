﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace ElGroupo.Web.Models.Events
{
    public class CreateEventModel: EventDateModel
    {
        public string GoogleApiKey { get; set; }
        public bool IsRecurring { get; set; } = false;
        public EventRecurrenceModel Recurrence { get; set; } = new EventRecurrenceModel();

        [Required]
        [Display(Description="Event Name")]
        public string Name { get; set; }

        [Display(Description = "Event Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string LocationName { get; set; }



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

        public AttendanceVerificationMethods AttendanceVerificationMethod { get; set; } = AttendanceVerificationMethods.None;
        public string VerificationCode { get; set; }
        public int? LocationTolerance { get; set; }

        //public bool RSVPRequired { get; set; } = false;
    }
}
