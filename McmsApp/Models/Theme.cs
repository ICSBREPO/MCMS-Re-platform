using System;
using System.Collections.Generic;
using SQLite;

namespace McmsApp.Models
{
    public class Theme
    {
        [PrimaryKey]
        public int id { get; set; }
        public string title { get; set; }
        [MaxLength(255)]
        public string description { get; set; }
        public string colorOne { get; set; }
        public string colorTwo { get; set; }
        public string colorThree { get; set; }
        public string colorFour { get; set; }
        public Boolean applied { get; set; }
        public string appliedLabel { get; set; }
    }
}
