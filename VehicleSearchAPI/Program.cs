using System.Collections.Generic;
using System.Text.Json;
using VehicleSearch;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

VehicleStorageSearch search = new VehicleStorageSearch();

// Define endpoint
app.MapPost("/", (List<Car> request) =>
{
    List<Car> cars = request.SelectMany(r => Enumerable.Repeat(new Car(r.Length), r.Quantity)).ToList();

    List<OptimalResponse> results = search.OptimalStorageSolution(cars);

    return Results.Json(results, new JsonSerializerOptions{WriteIndented = true});
});

app.Run();
