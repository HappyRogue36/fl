using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCatalog
{
    [Table("cars")]
    public class Car
    {
        [Key]
        [Column("car_id")]
        public int Id { get; set; }
        [Column("model")]
        public string? Name { get; set; }
    }

    // Model class for the "Driver" table
    [Table("drivers")]
    public class Driver
    {
        [Key]
        [Column("driver_id")]
        public int Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
    }

    // Model class for the "Crew" table
    [Table("crews")]
    public class Crew
    {
        [Key]
        [Column("crew_id")]
        public int Id { get; set; }
        [Column("driver_id")]
        public int DriverId { get; set; }
        [Column("car_id")]
        public int CarId { get; set; }
    }

    // Database context
    public class MyDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Crew> Crews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
               Config.Sql_con,
               ServerVersion.AutoDetect(Config.Sql_con)
            );
        }
    }

    public class BDManager
    {
        private readonly MyDbContext _context;

        public BDManager(MyDbContext context)
        {
            _context = context;
        }

        // Методы для таблицы Cars
        public List<Car> GetAllCars()
        {
            return _context.Cars.ToList();
        }

        public Car? GetCarById(int id)
        {
            return _context.Cars.FirstOrDefault(c => c.Id == id);
        }

        public void AddCar(Car car)
        {
            if (!_context.Cars.Any(c => c.Id == car.Id))
            {
                _context.Cars.Add(car);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Такой id уже присутствует");
            }
        }

        public void UpdateCar(Car car)
        {
            var existingCar = _context.Cars.FirstOrDefault(c => c.Id == car.Id);

            if (existingCar != null)
            {
                existingCar.Name = car.Name;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }

        public void DeleteCar(int id)
        {
            var carToDelete = _context.Cars.FirstOrDefault(c => c.Id == id);

            if (carToDelete != null)
            {
                _context.Cars.Remove(carToDelete);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }

        // Методы для таблицы Drivers
        public List<Driver> GetAllDrivers()
        {
            return _context.Drivers.ToList();
        }

        public Driver GetDriverById(int id)
        {
            return _context.Drivers.FirstOrDefault(d => d.Id == id);
        }

        public void AddDriver(Driver driver)
        {
            if (!_context.Drivers.Any(d => d.Id == driver.Id))
            {
                _context.Drivers.Add(driver);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Такой id уже присутствует");
            }
        }

        public void UpdateDriver(Driver driver)
        {
            var existingDriver = _context.Drivers.FirstOrDefault(d => d.Id == driver.Id);

            if (existingDriver != null)
            {
                existingDriver.Name = driver.Name;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }

        public void DeleteDriver(int id)
        {
            var driverToDelete = _context.Drivers.FirstOrDefault(d => d.Id == id);

            if (driverToDelete != null)
            {
                _context.Drivers.Remove(driverToDelete);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }

        // Методы для таблицы Crews
        public List<Crew> GetAllCrews()
        {
            return _context.Crews.ToList();
        }

        public Crew GetCrewById(int id)
        {
            return _context.Crews.FirstOrDefault(c => c.Id == id);
        }

        public void AddCrew(Crew crew)
        {
            if (!_context.Crews.Any(c => c.Id == crew.Id))
            {
                _context.Crews.Add(crew);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Такой id уже присутствует");
            }
        }

        public void UpdateCrew(Crew crew)
        {
            var existingCrew = _context.Crews.FirstOrDefault(c => c.Id == crew.Id);

            if (existingCrew != null)
            {
                existingCrew.DriverId = crew.DriverId;
                existingCrew.CarId = crew.CarId;
                // Обновите другие свойства при необходимости
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }

        public void DeleteCrew(int id)
        {
            var crewToDelete = _context.Crews.FirstOrDefault(c => c.Id == id);

            if (crewToDelete != null)
            {
                _context.Crews.Remove(crewToDelete);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Не удалось найти объект с таким id");
            }
        }
    }
}