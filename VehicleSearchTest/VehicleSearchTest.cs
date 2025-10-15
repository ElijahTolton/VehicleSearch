// Contains multiple tests checking the validity of VehicleSearch.
using System.ComponentModel.DataAnnotations;
using VehicleSearch;

namespace VehicleSearchTest;

[TestClass]
public sealed class VehicleSearchTest
{
    [TestMethod]
    public void OptimalSolution_OneVehicleOptimalPrice_Valid()
    {
        Car car = new Car(10);
        List<Car> cars = new List<Car> { car };

        VehicleStorageSearch search = new VehicleStorageSearch();
        var response = search.OptimalStorageSolution(cars);

        Assert.IsTrue(response[0].LocationId == "42b8f068-2d13-4ed1-8eec-c98f1eef0850");
    }

    [TestMethod]
    public void OptimalSolution_MultipleVehicles_Valid()
    {
        Car car1 = new Car(20);
        Car car2 = new Car(40);
        List<Car> cars = new List<Car> { car1, car2 };

        VehicleStorageSearch search = new VehicleStorageSearch();
        var response = search.OptimalStorageSolution(cars);

        Assert.AreEqual(312, response.Count);
        Assert.IsTrue(response.Any(r => r.LocationId == "42b8f068-2d13-4ed1-8eec-c98f1eef0850"));
        Assert.IsTrue(response[1].ListingIds[0] == "e7d59481-b804-4565-b49b-d5beb7aec350");
    }


    [TestMethod]
    public void OptimalSolution_NoFit_ReturnsEmpty()
    {
        Car car = new Car(100); // Too large for any listing
        List<Car> cars = new List<Car> { car };

        VehicleStorageSearch search = new VehicleStorageSearch();
        var response = search.OptimalStorageSolution(cars);

        Assert.AreEqual(0, response.Count);
    }

}
