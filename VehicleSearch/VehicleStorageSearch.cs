///© Elijah Tolton 2025. All rights reserved.
///
/// Author: Elijah Tolton
/// Date: 10/14/2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace VehicleSearch;

/// <summary>
/// Loads vehicle storage listings and finds the most optimal (cheapest)
/// place I can store them.
/// </summary>
public class VehicleStorageSearch
{
    /// <summary>
    /// Map (Key,Pair) = (ListingLocationID,Listing)
    /// </summary>
    private Dictionary<string, List<Listing>> listingsMap;

    // Initializes listingsMap
    public VehicleStorageSearch()
    {
        listingsMap = new Dictionary<string, List<Listing>>();

        string listingJson = File.ReadAllText("listings.json");

        // Deserialilze all of the given property listings.
        List<Listing> listings = JsonSerializer.Deserialize<List<Listing>>(listingJson); 
        if(listings == null)
        {
            throw new Exception("Failed to deserialize Listings");
        }

        // Sort each listings with other listings with the same locationID.
        foreach (Listing listing in listings)
        {
            if (listingsMap.ContainsKey(listing.LocationId))
            {
                listingsMap[listing.LocationId].Add(listing);
            }
            else
            {
                listingsMap[listing.LocationId] = new List<Listing> { listing };
            }
        }
    }

    /// <summary>
    /// Given a list of cars returns a list of optimal responses.
    /// 
    /// And returns a response like:
    ///```json
    ///[
    ///    {
    ///        "location_id": "abc123",
    ///        "listing_ids": ["def456", "ghi789", "jkl012"],
    ///        "total_price_in_cents": 300
    ///    },
    ///    {
    ///        "location_id": "mno345",
    ///        "listing_ids": ["pqr678", "stu901"],
    ///        "total_price_in_cents": 305
    ///    }
    ///]
    ///```
    ///The results should:
    /// Include every possible location that could store all requested vehicles
    /// Include the cheapest possible combination of listings per location
    /// Include only one result per location_id
    /// Be sorted by the total price in cents, ascending
    ///
    /// </summary>
    /// <param name="cars">List of cars trying to be stored</param>
    /// <returns>A list of optimal places to store the cars.</returns>
    public List<OptimalResponse> OptimalStorageSolution(List<Car> cars)
    {
        var results = new List<OptimalResponse>();

        foreach (var location in listingsMap.Keys)
        {
            List<Listing> listings = listingsMap[location];

            // Calculate all possible vehicle subsets that can fit within listing.
            // Map listingID -> List of possible car combinations to fit in lot.
            Dictionary<Listing, List<List<Car>>> listingCombinations = [];
            foreach (var listing in listings)
            {
                listingCombinations[listing] = GetAllCarCombinations(listing, cars);
            }


            // Calculate the cheapest combination of listings.
            int minCost = int.MaxValue;
            List<Listing> bestCombination = null;
            List<Car> carsRemaining = new List<Car>(cars);
            List<Listing> selectedListings = new List<Listing>();

            FindCheapestCombination(listings, listingCombinations, carsRemaining, selectedListings, ref bestCombination, ref minCost);

            if( bestCombination == null || bestCombination.Count == 0)
            {
                // This location can't hold these vehicles.
                continue;
            }

            // Generate optimal response
            List<string> listingIDs = new List<string>(bestCombination.Select(listing => listing.Id));
            OptimalResponse op = new OptimalResponse(location, listingIDs, minCost);
            results.Add(op);
        }

        // Sort results based on min price.
        results.Sort((a,b) => a.TotalPriceInCents.CompareTo(b.TotalPriceInCents));

        return results;
    }

    /// <summary>
    /// Returns the set of all possible car combinations to be stored within this
    /// listing.
    /// </summary>
    /// <param name="listing">Lot where we are calculating to fit vehicles.</param>
    /// <param name="cars">List of vehicles which we are trying to fit.</param>
    /// <returns></returns>
    public static List<List<Car>> GetAllCarCombinations(Listing listing, List<Car> cars)
    {
        List<List<Car>> combinations = [];

        // Try orientation where car length along lot length
        int cols = listing.Width / 10;
        int colCapacity = listing.Length;
        // Space remaining in each col.
        int[] colRemaining = Enumerable.Repeat(colCapacity, cols).ToArray();

        // Try orientation where car length is along the width
        int rows = listing.Length / 10;
        int rowCapacity = listing.Width;
        // Space remaining in each col
        int[] rowRemaining = Enumerable.Repeat(rowCapacity, rows).ToArray();


        // Recursively calculates all the possible combinations of cars to be stored in this listing.
        // Not all combinations will have every car in it.
        void RecurseCombinations(int index, List<Car> currentCombo, int[] rowRemaining)
        {
            // Base case we have placed all of the cars.
            if(index == cars.Count)
            {
                if (currentCombo.Count > 0)
                    combinations.Add(new List<Car>(currentCombo));
                return;
            }

            Car car = cars[index];
            // Try placing this car in each row.
            for (int row = 0; row < rowRemaining.Length; row++)
            {
                if( car.Length <= rowRemaining[row])
                {
                    rowRemaining[row] -= car.Length;
                    currentCombo.Add(car);

                    // Place other cars after placement.
                    RecurseCombinations(index + 1, currentCombo, rowRemaining);

                    // Restore remaining and current to test placing this car in another row.
                    currentCombo.RemoveAt(currentCombo.Count - 1);
                    rowRemaining[row] += car.Length;
                }
            }

            RecurseCombinations(index + 1, currentCombo, rowRemaining);
        }

        RecurseCombinations(0, new List<Car>(), colRemaining);
        RecurseCombinations(0, new List<Car>(), rowRemaining);


        return combinations;
    }

    /// <summary>
    /// Given a list of all possible storage combinations for this location, combine all listings
    /// to cover all vehicles in the cheapest possible way. Recursively try combinations of all cars
    /// and track the cheapest.
    /// </summary>
    /// <param name="listings">All listings associated with this locationID</param>
    /// <param name="listingCombos">All possible combinations to store cars in each listing.</param>
    /// <param name="carsRemaining">Cars not placed yet.</param>
    /// <param name="selectedListings">Current listings with cars placed.</param>
    /// <param name="bestSelection">Cheapest combination of listings.</param>
    /// <param name="bestCost">Lowest cost to store vehicles.</param>
    private static void FindCheapestCombination(List<Listing> listings, Dictionary<Listing, List<List<Car>>> listingCombos, 
        List<Car> carsRemaining, List<Listing> selectedListings, ref List<Listing> bestSelection, ref int bestCost)
    {
        // Base case 1. We have placed all cars in a listing spot.
        if( carsRemaining.Count == 0)
        {
            int cost = selectedListings.Sum(listing => listing.PriceInCents);
            if(cost < bestCost)
            {
                bestCost = cost;
                bestSelection = new List<Listing>(selectedListings);
            }
            return;
        }

        // Base Case 2. If there are no more listings and we still have cars.
        // Likely a car could not be fit into a listing with the current combo.
        if (listings.Count == 0) return;

        Listing currentListing = listings[0];
        // Remove current
        var remainingListings = listings.Skip(1).ToList();

        foreach(var combination in listingCombos[currentListing])
        {
            // Copy so recursion doesn't mess with each other.
            var newCarsRemaining = new List<Car>(carsRemaining);

            // Keep track if combination actually helps store a car.
            bool carStored = false;

            foreach (var car in combination)
            {
                if (newCarsRemaining.Contains(car))
                {
                    newCarsRemaining.Remove(car);
                    carStored = true;
                }
            }

            // Skip combination
            if (!carStored) continue;

            selectedListings.Add(currentListing);
            FindCheapestCombination(remainingListings, listingCombos, newCarsRemaining, selectedListings, ref bestSelection, ref bestCost);

            // Remove current listing to test other possible combinations first.
            selectedListings.RemoveAt(selectedListings.Count - 1);
        }

        FindCheapestCombination(remainingListings, listingCombos, carsRemaining, selectedListings, ref bestSelection, ref bestCost);
    }
}
