using System;

namespace Portalia.Core.Entity
{
    public class CountryLanguage: Repository.Pattern.Ef6.Entity
    {
        public string LanguageID { get; set; }
        public string CountryID { get; set; }
        public string Label { get; set; }
        public int Counter { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }

        public virtual Language Language { get; set; }
    }
}
