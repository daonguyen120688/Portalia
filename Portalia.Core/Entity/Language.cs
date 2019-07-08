using System;
using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class Language: Repository.Pattern.Ef6.Entity
    {
        public Language()
        {
            this.CountryLanguages = new HashSet<CountryLanguage>();
        }

        public string ID { get; set; }
        public string Label { get; set; }
        public bool Active { get; set; }
        public bool Disabled { get; set; }
        public int Counter { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public virtual ICollection<CountryLanguage> CountryLanguages { get; set; }
    }
}
