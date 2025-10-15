using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleSearch;

/// <summary>
/// Contains length, width and quantity of a car.
/// </summary>
public class Car
{
    /// <summary>
    /// Length of the car in meters.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Width of the car in meters.
    /// </summary>
    [JsonIgnore]
    public int Width { get; set; } = 10;

    /// <summary>
    /// Quantity of cars.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Length">Length of car in ft</param>
    /// <param name="Quantity">Number of cars to store</param>
    public Car(int Length)
    {
        this.Length = Length;
    }
}
