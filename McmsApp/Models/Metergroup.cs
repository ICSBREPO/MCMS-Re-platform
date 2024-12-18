using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Tnbwometer : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int tnbwometersid { get; set; }
        public int tnbwometergroupid { get; set; }
        public string tnbremarks { get; set; }
        public string tnblastreading { get; set; }
        public string tnbmeterlowerlimit { get; set; }
        public string description { get; set; }
        public string tnblastreadingdate { get; set; }
        public string tnbnewreadingdate { get; set; }
        public string tnblocation { get; set; }
        public string tnbmetername { get; set; }
        public string tnbresult { get; set; }
        public bool tnbmandatorychkbox { get; set; }
        public string tnbwodesc { get; set; }
        public string tnbmeterupperlimit { get; set; }
        public string tnbnewreading { get; set; }
        public string tnblastreadinginspector { get; set; }
        public string tnbmanualreading { get; set; }
        public string tnbwonum { get; set; }
        public string jobplanid { get; set; }
        public string tnbparentwonum { get; set; }
        public string tnbassetnum { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Meter> meter { get; set; }
        //custom
        public bool islookup { get; set; }
        public string domainid { get; set; }
        public string measureunitid { get; set; }
        public string meterdescription { get; set; }
        public string metertype { get; set; }
        //new added
        public int tnbsequence { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Metergroup : INotifyPropertyChanged
    {
        public string _rowstamp { get; set; }
        public string localref { get; set; }
        public string description { get; set; }
        public int metergroupid { get; set; }
        public string href { get; set; }
        public string groupname { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Tnbwometergroup : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int tnbwometergroupid { get; set; }
        public string tnbcbnumber { get; set; }
        public string description { get; set; }
        public string tnblocation { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwometer> tnbwometers { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Metergroup> metergroup { get; set; }
        public string tnbmetergroup { get; set; }
        public string tnbwodesc { get; set; }
        public string tnbwonum { get; set; }
        public string jobplanid { get; set; }
        public string tnbtasknum { get; set; }
        public double tnbper { get; set; }
        public int tnbsequence { get; set; }
        public string tnbparentwonum { get; set; }
        public string tnbassetnum { get; set; }
        //custom
        public bool pendingupload { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Meter
    {
        public string metertype { get; set; }
        public string localref { get; set; }
        public string alndomain_collectionref { get; set; }
        public string description { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<MxDomain> alndomain { get; set; }
        public string metertype_description { get; set; }
        public string domainid { get; set; }
        public bool tnbpropagatedefaultmod { get; set; }
        public string metername { get; set; }
        public int meterid { get; set; }
        public bool tnbautocreatecm { get; set; }
        public bool tnbautocalculate { get; set; }
        public string measureunitid { get; set; }
    }
}
