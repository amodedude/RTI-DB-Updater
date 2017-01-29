using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Objects
{
    public class State
    {
        public State(string ab, string name, string region)
        {
            Name = name;
            Abbreviation = ab;
            Region = region;
        }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            return $"{Abbreviation} - {Name}";
        }
    }

    public static class States
    {
      public static List<State> los = new List<State> {
      //us
      new State("AL", "Alabama","East South Central"),
      new State("AK", "Alaska","Non-Contiguous"),
      new State("AZ", "Arizona", "Mountain"),
      new State("AR", "Arkansas", "West South Central"),
      new State("CA", "California", "Pacific"),
      new State("CO", "Colorado", "Mountain"),
      new State("CT", "Connecticut", "New England"),
      new State("DE", "Delaware", "South Atlantic"),
      new State("DC", "District Of Columbia", "South Atlantic"),
      new State("FL", "Florida", "South Atlantic"),
      new State("GA", "Georgia", "South Atlantic"),
      new State("HI", "Hawaii","Non-Contiguous"),
      new State("ID", "Idaho", "Mountain"),
      new State("IL", "Illinois", "East North Central"),
      new State("IN", "Indiana", "East North Central"),
      new State("IA", "Iowa", "West North Central"),
      new State("KS", "Kansas", "West North Central"),
      new State("KY", "Kentucky","East South Central"),
      new State("LA", "Louisiana", "West South Central"),
      new State("ME", "Maine", "New England"),
      new State("MD", "Maryland", "South Atlantic"),
      new State("MA", "Massachusetts", "New England"),
      new State("MI", "Michigan", "East North Central"),
      new State("MN", "Minnesota", "West North Central"),
      new State("MS", "Mississippi","East South Central"),
      new State("MO", "Missouri", "West North Central"),
      new State("MT", "Montana", "Mountain"),
      new State("NE", "Nebraska", "West North Central"),
      new State("NV", "Nevada", "Mountain"),
      new State("NH", "New Hampshire", "New England"),
      new State("NJ", "New Jersey", "Middle Atlantic"),
      new State("NM", "New Mexico", "Mountain"),
      new State("NY", "New York", "Middle Atlantic"),
      new State("NC", "North Carolina", "South Atlantic"),
      new State("ND", "North Dakota", "West North Central"),
      new State("OH", "Ohio", "East North Central"),
      new State("OK", "Oklahoma", "West South Central"),
      new State("OR", "Oregon", "Pacific"),
      new State("PA", "Pennsylvania", "Middle Atlantic"),
      new State("RI", "Rhode Island", "New England"),
      new State("SC", "South Carolina", "South Atlantic"),
      new State("SD", "South Dakota", "West North Central"),
      new State("TN", "Tennessee","East South Central"),
      new State("TX", "Texas", "West South Central"),
      new State("UT", "Utah", "Mountain"),
      new State("VT", "Vermont", "New England"),
      new State("VA", "Virginia", "South Atlantic"),
      new State("WA", "Washington", "Pacific"),
      new State("WV", "West Virginia", "South Atlantic"),
      new State("WI", "Wisconsin", "East North Central"),
      new State("WY", "Wyoming", "Mountain"),
      //canada
      new State("AB", "Alberta", "Western Canada"),
      new State("BC", "British Columbia", "Western Canada"),
      new State("MB", "Manitoba", "Western Canada"),
      new State("NB", "New Brunswick", "Atlantic Canada"),
      new State("NL", "Newfoundland and Labrador", "Atlantic Canada"),
      new State("NS", "Nova Scotia", "Atlantic Canada"),
      new State("NT", "Northwest Territories", "Northern Canada"),
      new State("NU", "Nunavut", "Northern Canada"),
      new State("ON", "Ontario", "Central Canada"),
      new State("PE", "Prince Edward Island", "Atlantic Canada"),
      new State("QC", "Quebec", "Central Canada"),
      new State("SK", "Saskatchewan", "Western Canada"),
      new State("YT", "Yukon", "Northern Canada")
      };

        public static List<string> Abbreviations()
        {
            return los.Select(s => s.Abbreviation).ToList();
        }

        public static List<string> Names()
        {
            return los.Select(s => s.Name).ToList();
        }

        public static string GetName(string abbreviation)
        {
            return los.Where(s => s.Abbreviation.Equals(abbreviation, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.Name).FirstOrDefault();
        }

        public static string GetAbbreviation(string name)
        {
            return los.Where(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.Abbreviation).FirstOrDefault();
        }

        public static string GetRegion(string name)
        {
            return los.Where(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.Region).FirstOrDefault();
        }

        public static List<State> ToList()
        {
            return los;
        }
    }
}
