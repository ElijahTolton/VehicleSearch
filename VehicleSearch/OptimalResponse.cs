using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleSearch;

public class OptimalResponse
{
    /// <summary>
    /// Identifier for the location.
    /// </summary>
    [JsonPropertyName("location_id")]
    public string LocationId { get; set; }

    /// <summary>
    /// List of associated listing identifiers.
    /// </summary>
    [JsonPropertyName("listing_ids")]
    public List<string> ListingIds { get; set; }

    /// <summary>
    /// Total price in cents for the listings.
    /// </summary>
    [JsonPropertyName("total_price_in_cents")]
    public int TotalPriceInCents { get; set; }

    public OptimalResponse(string locationId, List<string> listingIds, int totalPriceInCents)
    {
        this.LocationId = locationId;
        this.ListingIds = listingIds;
        this.TotalPriceInCents = totalPriceInCents;
    }

}
