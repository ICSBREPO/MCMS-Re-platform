using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace mcms.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Plusgaudline : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? id { get; set; }
        public int? sequence { get; set; }
        public bool no { get; set; }
        public string tnbsqaremarks { get; set; }
        public int? sqaid { get; set; }
        public bool? template { get; set; }
        public string response_longdescription { get; set; }
        public bool yes { get; set; }
        public int? plusgaudlineid { get; set; }
        public string description { get; set; }
        public bool select3 { get; set; }
        public bool select2 { get; set; }
        public bool select1 { get; set; }
        public bool likertscale { get; set; }
        public int? plusgauditid { get; set; }
        public int? linenum { get; set; }
        public string tnblinenum { get; set; }
        public bool notapplicable { get; set; }
        public bool reversescored { get; set; }
        public int? linescore { get; set; }
        public bool select5 { get; set; }
        public bool select4 { get; set; }
        public string alertcolor { get; set; }
        public string _action { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Plusgaudit : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string _action { get; set; }
        public bool pendingupload { get; set; }
        public string tnbwonum { get; set; }
        public bool certreqd { get; set; }
        public bool climatesurvey { get; set; }
        public bool template { get; set; }
        public string status_description { get; set; }
        public bool historyflag { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Plusgaudline> plusgaudline { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbsignature> tnbsignature { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Doclinks> doclinks { get; set; }
        public string auditnum { get; set; }
        public string tnbauditnumber { get; set; }
        public int? totalscore { get; set; }
        public int? createdbyid { get; set; }
        public string description { get; set; }
        public bool surveyformat { get; set; }
        public int? tnbpercentage { get; set; }
        public DateTime? statusdate { get; set; }
        public bool externalaudit { get; set; }
        public string createdby { get; set; }
        public int? plusgauditid { get; set; }
        public bool vendoraudit { get; set; }
        public DateTime? createdbydate { get; set; }
        public DateTime? approvedbydate { get; set; }
        public string status { get; set; }
        public string completedby { get; set; }
        public string approvedby { get; set; }
        public string tnbremarks { get; set; }
        public string tnbcontractor { get; set; }
        public string tnbteam { get; set; }
        public string tnbscope { get; set; }
        public string company { get; set; }
        public string tnbreceivedby { get; set; }
        public string tnbstaff { get; set; }
        public string tnbimname { get; set; }
        public int totalSignature { get; set; }
        public string tnbimstaff { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class ResSQA
    {
        public List<Plusgaudit> member { get; set; }
        public ErrorResponse Error { get; set; }
    }


}