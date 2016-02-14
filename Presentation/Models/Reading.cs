using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Development.Web.Models
{
    public class Reading
    {
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StationCode { get; set; }
        [Required]
        public int ActualSpeed { get; set; }
        [Required]
        public int PredictedSpeed { get; set; }
        [Required]
        public DateTime ReadingDate { get; set; }
        [Required]
        public int Variance { get; set; }
        [Required]
        public string Captcha { get; set; }
    }
}