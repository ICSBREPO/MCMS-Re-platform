using SQLite;

namespace McmsApp.Models
{
    public class MxDomain
    {
        [PrimaryKey]
        public int alndomainid { get; set; }
        public string valueid { get; set; }
        public string domainid { get; set; }
        public bool defaults { get; set; }
        public string description { get; set; }
        public string maxvalue { get; set; }
        public string value { get; set; }
        public int synonymdomainid { get; set; }
    }

    public class Crossoverdomain
    {
        public string destfield { get; set; }
        public string _rowstamp { get; set; }
        public bool copyevenifsrcnull { get; set; }
        public bool copyonlyifdestnull { get; set; }
        public string localref { get; set; }
        public int crossoverdomainid { get; set; }
        public string href { get; set; }
        public string sourcefield { get; set; }
        public int? sequence { get; set; }
        public string destcondition { get; set; }

    }

    public class Maxtabledomain
    {
        public string objectname { get; set; }
        public string erroraccesskey { get; set; }
        public string _rowstamp { get; set; }
        public string errorresourcbundle { get; set; }
        public List<Crossoverdomain> crossoverdomain { get; set; }
        public int maxtabledomainid { get; set; }
        public string crossoverdomain_collectionref { get; set; }
        public string validtnwhereclause { get; set; }
        public string localref { get; set; }
        public string href { get; set; }
        public string listwhereclause { get; set; }

    }

    public class Synonymdomain
    {
        public bool defaults { get; set; }
        public string _rowstamp { get; set; }
        public string valueid { get; set; }
        public string description { get; set; }
        public string maxvalue { get; set; }
        public string value { get; set; }
        public int synonymdomainid { get; set; }
        public string localref { get; set; }
        public string href { get; set; }

    }

    public class Alndomain
    {
        public string _rowstamp { get; set; }
        public string valueid { get; set; }
        public int alndomainid { get; set; }
        public string description { get; set; }
        public string value { get; set; }
        public string localref { get; set; }
        public string href { get; set; }

    }

    public class Maxdomvalcond
    {
        public string _rowstamp { get; set; }
        public string valueid { get; set; }
        public int maxdomvalcondid { get; set; }
        public string localref { get; set; }
        public string conditionnum { get; set; }
        public string href { get; set; }

    }

    public class Numericdomain
    {
        public string _rowstamp { get; set; }
        public string valueid { get; set; }
        public string description { get; set; }
        public int numericdomainid { get; set; }
        public double value { get; set; }
        public string localref { get; set; }
        public string href { get; set; }

    }

    public class MemberDom
    {
        public string alndomain_collectionref { get; set; }
        public string maxdomvalcond_collectionref { get; set; }
        public string domainid { get; set; }
        public string maxtabledomain_collectionref { get; set; }
        public string synonymdomain_collectionref { get; set; }
        public string domaintype { get; set; }
        public string _rowstamp { get; set; }
        public string numericdomain_collectionref { get; set; }
        public int maxdomainid { get; set; }
        public string domaintype_description { get; set; }
        public string description { get; set; }
        public List<Maxtabledomain> maxtabledomain { get; set; }
        public string internal_description { get; set; }
        public string href { get; set; }
        public List<MxDomain> synonymdomain { get; set; }
        public int? length { get; set; }
        public string maxtype { get; set; }
        public List<MxDomain> alndomain { get; set; }
        public int? scale { get; set; }
        public List<Maxdomvalcond> maxdomvalcond { get; set; }
        public List<Numericdomain> numericdomain { get; set; }

    }


    public class MaxDomains
    {
        public List<MemberDom> member { get; set; }
        public double size { get; set; }
        public ResponseInfo responseInfo { get; set; }
        public string href { get; set; }

    }
}
