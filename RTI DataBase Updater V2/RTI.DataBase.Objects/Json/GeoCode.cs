using Newtonsoft.Json;

namespace RTI.DataBase.Objects.Json
{
    public class GeoCode
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
        [JsonProperty("licence")]
        public string Licence { get; set; }
        [JsonProperty("osm_type")]
        public string OsmType { get; set; }
        [JsonProperty("osm_id")]
        public string OsmId { get; set; }
        [JsonProperty("lat")]
        public string Lat { get; set; }
        [JsonProperty("lon")]
        public string Lon { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("boundingbox")]
        public string[] Name { get; set; }
    }

    /// <summary>
    /// Address details of a GeoLocation.
    /// </summary>
    public class Address
    {
        [JsonProperty("house_number")]
        public string AddressNumber { get; set; }
        [JsonProperty("road")]
        public string Road { get; set; }
        [JsonProperty("neighbourhood")]
        public string Neighbourhood { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("postcode")]
        public string PostCode { get; set; }
    }
}
