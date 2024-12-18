using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Workorderspec : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int classspecid { get; set; }
        public string classstructureid { get; set; }
        public string changeby { get; set; }
        public DateTime changedate { get; set; }
        public string alnvalue { get; set; }
        public bool mandatory { get; set; }
        public string refobjectname { get; set; }
        public string orgid { get; set; }
        public string assetattrid { get; set; }
        public int refobjectid { get; set; }
        public int displaysequence { get; set; }
        public string href { get; set; }
        public int workorderspecid { get; set; }
        public bool pendingupload { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Asset : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string href { get; set; }
        public string wonum { get; set; }
        public int assetuid { get; set; }
        public int assetid { get; set; }
        public string assetnum { get; set; }
        public string tnbmodelnum { get; set; }
        public string description { get; set; }
        public string serialnum { get; set; }
        public string tnbmanufacturerpart { get; set; }
        public string manufacturer { get; set; }
        public string tnbmanufacturingcountry { get; set; }
        public string tnbbrand { get; set; }
        public string tnbbrand_description { get; set; }
        public string tnbitemnum { get; set; }
        public string itemnum { get; set; }
        public string tnbvoltagelevel { get; set; }
        public string tnbassetcondition { get; set; }
        public string _action { get; set; }
        public DateTime? tnbdeliverydate { get; set; }
        public DateTime? tnbmanufdate { get; set; }
        
        public bool pendingupload { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<AssetSpec> assetspec { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        
    }

    [AddINotifyPropertyChangedInterface]
    public class Locations : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int locationsid { get; set; }
        public string description { get; set; }
        public string wonum { get; set; }
        public bool pendingupload { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LocationSpec> locationspec { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class AssetAttribute
    {
        public string datatype { get; set; }
        public string description { get; set; }
        public string domainid { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<MxDomain> alndomain { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class AssetSpec : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int assetspecid { get; set; }
        public string classstructureid { get; set; }
        public string tnbclass { get; set; }
        public int linearassetspecid { get; set; }
        public bool mandatory { get; set; }
        public string orgid { get; set; }
        public string assetattrid { get; set; }
        public bool itemspecvalchanged { get; set; }
        public bool inheritedfromitem { get; set; }
        public string assetnum { get; set; }
        public bool continuous { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<AssetAttribute> assetattribute { get; set; }
        public string domainid { get; set; }
        public string datatype { get; set; }
        public string alnvalue { get; set; }
        public string description { get; set; }
        public int? numvalue { get; set; }
        public string measureunitid { get; set; }
        public int displaysequence { get; set; }
        public int workorderid { get; set; }
        public bool pendingupload { get; set; }
        public bool islookup { get; set; }
        public string xamtype { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class LocationSpec : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int locationspecid { get; set; }
        public int locationsid { get; set; }
        public string classstructureid { get; set; }
        public string tnbclass { get; set; }
        public int linearassetspecid { get; set; }
        public bool mandatory { get; set; }
        public string orgid { get; set; }
        public string assetattrid { get; set; }
        public bool itemspecvalchanged { get; set; }
        public bool inheritedfromitem { get; set; }
        public string location   { get; set; }
        public bool continuous { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<AssetAttribute> assetattribute { get; set; }
        public string domainid { get; set; }
        public string datatype { get; set; }
        public string description { get; set; }
        public string alnvalue { get; set; }
        public int? numvalue { get; set; }
        public string measureunitid { get; set; }
        public int displaysequence { get; set; }
        public int workorderid { get; set; }
        public bool pendingupload { get; set; }
        public bool islookup { get; set; }
        public string xamtype { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
