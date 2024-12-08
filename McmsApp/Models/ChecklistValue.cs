using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace mcms.Models
{
    public class Tnbchecklistvalue : INotifyPropertyChanged
    {
        public string _rowstamp { get; set; }
        public string tnbchecklist { get; set; }
        public string tnbvalue { get; set; }
        public string tnborgid { get; set; }
        public string href { get; set; }
        public int tnbsequence { get; set; }
        public int tnbchecklistvalueid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Restnbchecklistval
    {
        public List<Tnbchecklistvalue> member { get; set; }
        public ResponseInfo responseInfo { get; set; }
    }
}
