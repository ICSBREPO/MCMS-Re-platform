using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mcms.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Tnbwochecklist : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int tnbwochecklistid { get; set; }
        public int tnbwochecklisttypeid { get; set; }
        public string tnbtype { get; set; }
        public bool tnbyes { get; set; }
        public string tnbdescription { get; set; }
        public string tnbwonum { get; set; }
        public int tnbsequence { get; set; }
        public bool tnbna { get; set; }
        public string tnbvalue { get; set; }
        public string tnbdefectdescription { get; set; }
        public string tnbremarks { get; set; }
        public DateTime? tnbdefectrectdate { get; set; }
        public bool tnbpasswithcondition { get; set; }
        public bool tnbno { get; set; }
        public bool xamvisible { get; set; }
        public string alertcolor { get; set; }
        public int totalIndicator { get; set; }
        public int totalAttachment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    [AddINotifyPropertyChangedInterface]
    public class Tnbwochecklisttype : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int tnbwochecklisttypeid { get; set; }
        public int tnbpsiwochecklisttypeid { get; set; }
        public string tnbchecklisttype { get; set; }
        public string tnbdescription { get; set; }
        public string tnbwonum { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwochecklist> tnbwochecklist { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwochecklistsignature> tnbwochecklistsignature { get; set; }
        public double tnbpassper { get; set; }
        public bool tnballpass { get; set; }
        public bool tnbsignature { get; set; }
        public int tnbsequence { get; set; }
        public int totalIndicator { get; set; }
        public int totalAttachment { get; set; }
        public int totalSignature { get; set; }
        public bool pendingupdate { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    [AddINotifyPropertyChangedInterface]
    public class Tnbpsiwochecklisttype : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int tnbpsiwochecklisttypeid { get; set; }
        public int tnbwochecklisthdrid { get; set; }
        public string tnbchecklisttype { get; set; }
        public string tnbdescription { get; set; }
        public string tnbwonum { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwochecklisttype> tnbwochecklisttype { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwochecklisthdr> tnbwochecklisthdr { get; set; }
        public double tnbpassper { get; set; }
        public bool tnballpass { get; set; }
        public int tnbsequence { get; set; }
        public int totalIndicator { get; set; }
        public bool pendingupdate { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    [AddINotifyPropertyChangedInterface]
    public class Tnbwochecklisthdr : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int? tnbwochecklisthdrid { get; set; }
        public int tnbpsiwochecklisttypeid { get; set; }
        public string tnbwonum { get; set; }
        public string tnbdescription { get; set; }
        public string tnbpreparedby { get; set; }
        public string tnbvalidatedby { get; set; }
        public string tnbapprovedby { get; set; }
        public DateTime? tnbprepareddate { get; set; }
        public DateTime? tnbvalidateddate { get; set; }
        public DateTime? tnbapproveddate { get; set; }
        public DateTime tnbchangedate { get; set; }
        public string tnbprepcomments { get; set; }
        public string tnbvalcomments { get; set; }
        public string tnbapprcomments { get; set; }
        public bool pendingupdate { get; set; }
        public string tnbchangedby { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
