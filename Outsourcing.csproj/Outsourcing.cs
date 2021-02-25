using System;
using System.Dynamic;

namespace Outsourcing
{
    [Route("/cars")]
    public class Program : Controller
    {
        private ICarService carService;

        //Create
        [Route("{id}")]
        [HttpPost]
        public void Create(Request request)
        {
            if (!carService.CreateCar(request, out string errorMessage));
                throw new Exception(errorMessage);
        }

        //Update
        [Route("{id}")]
        [HttpPatch]
        public Car Update(string id, Request request)
        {
            var updatedCar = carService.UpdateCar(id, request.Name, request.Weight, out string errorMessage);
            if (updatedCar == null)
                throw new Exception(errorMessage);
            return updatedCar;
        }

        //Get
        [Route("{id}")]
        [HttpGet]
        public Car Get(string id)
        {
            var car = carService.GetCar(id, out string errorMessage);
            if (car == null)
                throw new Exception(errorMessage);
            return car;
        }

        //Delete
        [Route("{id}")]
        [HttpDelete]
        public void Delete(string id)
        {
            if (!carService.DeleteCar(id, out string errorMessage))
                throw new Exception(errorMessage);
        }
    }

    public class CarService : ICarService
    {
        private ICarRepository repository;

        private readonly double maxWeight = 1000;
        private readonly double minWeight = 0;

        public CarService(ICarRepository repository)
        {
            this.repository = repository;
        }

        public bool DeleteCar(string id, out string errorMessage)
        {
            var car = TryFoundCar(id, out errorMessage);
            if (car == null)
                return false;
            repository.DeleteCar(id);
            errorMessage = string.Empty;
            return true;
        }

        public Car GetCar(string id, out string errorMessage)
        {
            return TryFoundCar(id, out errorMessage);
        }

        public bool CreateCar(Request request, out string errorMessage)
        {
            if (CheckWeight(request.Weight, out errorMessage))
                return false;

            var car = new Car
            {
                Id = "1",
                Mark = request.Mark,
                Model = request.Model,
                Name = request.Name,
                Weight = request.Weight
            };
            repository.CreateCar(car);

            errorMessage = string.Empty;
            return true;
        }

        public Car UpdateCar(string id, string name, double weight, out string errorMessage)
        {
            if (CheckWeight(weight, out errorMessage))
                return null;

            var car = TryFoundCar(id, out errorMessage);
            if (car == null)
                return null;

            var updatedCar = repository.UpdateCar(car.Id, name, weight);
            errorMessage = string.Empty;
            return updatedCar;
        }

        private bool CheckWeight(double weight, out string errorMessage)
        {
            if (weight > maxWeight || weight < minWeight)
            {
                errorMessage = "Wrong format of weight parameter";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private Car TryFoundCar(string id, out string errorMessage)
        {
            var car = repository.GetCar(id);
            if (car == null)
            {
                errorMessage = "Car was not found";
                return null;
            }

            errorMessage = string.Empty;
            return car;
        }
    }

    public interface ICarRepository
    {
        void CreateCar(Car model);
        Car UpdateCar(string id, string name, double weight);
        Car GetCar(string id);
        void DeleteCar(string id);
    }

    public interface ICarService
    {
        bool CreateCar(Request request, out string errorMessage);
        Car UpdateCar(string id, string name, double weight, out string errorMessage);
        Car GetCar(string id, out string errorMessage);
        bool DeleteCar(string id, out string errorMessage);
    }

    public class Car
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
    }

    public class Request
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
    }
}