using PropertyChanged;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Companies : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int companiesid { get; set; }
        public string company { get; set; }
        public string name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ResCompanies
    {
        public List<Companies> member { get; set; }
        public ErrorResponse Error { get; set; }
    }
}
