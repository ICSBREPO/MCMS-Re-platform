using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyChanged;
using SQLite;

namespace McmsApp.Models
{
    public class Apikey
    {
        public string apikey { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class ErrorResponse
    {
        public string message { get; set; }
        public string statusCode { get; set; }
        public string reasonCode { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class UserProfile : INotifyPropertyChanged
    {
        public bool queryWithSite { get; set; }
        public string country { get; set; }
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int personuid { get; set; }
        public string personid { get; set; }
        public DateTime lastupdate { get; set; }
        public string defaultSiteDescription { get; set; }
        public bool canUseInactiveSites { get; set; }
        public string defaultOrg { get; set; }
        public string langcode { get; set; }
        public string displayyes { get; set; }
        public string href { get; set; }
        public string defaultStoreroom { get; set; }
        public string _imagelibref { get; set; }
        public string loginID { get; set; }
        public string defaultLcale { get; set; }
        public string stateprovince { get; set; }
        public string lastname { get; set; }
        public string loginUserName { get; set; }
        public string defaultTimeZone { get; set; }
        public string apicachekey { get; set; }
        public string insertSite { get; set; }
        public bool botcinstall { get; set; }
        public string defaultApplication { get; set; }
        public string baseLang { get; set; }
        public string firstname { get; set; }
        public string displayno { get; set; }
        public string city { get; set; }
        public string displayname { get; set; }
        public string defaultStoreroomSite { get; set; }
        public string baseCurrency { get; set; }
        public string baseCalendar { get; set; }
        public string defaultLanguage { get; set; }
        public string postalcode { get; set; }
        public string ibmId { get; set; }
        public bool inactiveSites { get; set; }
        public string defaultSite { get; set; }
        public string datetimeformat { get; set; }
        public bool isLocalSession { get; set; }
        public string calendarType { get; set; }
        public string timeformat { get; set; }
        public string userName { get; set; }
        public string insertOrg { get; set; }
        public string dateformat { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string languageChoose { get; set; }
        public string picture { get; set; }
        public string base64string { get; set; }
        public int themeChoose { get; set;  }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ResPerson
    {
        public List<UserProfile> member { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class Lbslocation
    {
        public string refobject { get; set; }
        public DateTime lastupdate { get; set; }
        public int lbslocationid { get; set; }
        public string key2 { get; set; }
        public string key1 { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string href { get; set; }
        public string wonum { get; set; }
        public string siteid { get; set; }
    }


}
