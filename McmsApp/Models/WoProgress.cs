﻿using System;
using System.ComponentModel;
using PropertyChanged;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;

namespace mcms.Models
{
    [AddINotifyPropertyChangedInterface]
    public class WoProgress : INotifyPropertyChanged
    {
        internal ImageSource image;
        public string Title { get; set; }
        public string Description { get; set; }
        public StepStatus Status { get; set; }
        public int ProgressValue { get; set; }
        public ImageSource ProgressImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}