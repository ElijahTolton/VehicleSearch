using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleSearch;

public class Listing
{
    /// <summary>
    /// Unique identifier for the listing.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Length of the listing in meters.
    /// </summary>
    [JsonPropertyName("length")]
    public int Length { get; set; }

    /// <summary>
    /// Width of the listing in meters.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Identifier for the location associated with the listing.
    /// </summary>
    [JsonPropertyName("location_id")]
    public string LocationId { get; set; }

    /// <summary>
    /// Price of the listing in cents.
    /// </summary>
    [JsonPropertyName("price_in_cents")]
    public int PriceInCents { get; set; }


    public override string ToString()
    {
        string output = $"id: {Id}," +
            $"location_id: {LocationId}" +
            $"length: {Length}" +
            $"width: {Width}" +
            $"price_in_cents: {PriceInCents}";

        return output;
    }
}
