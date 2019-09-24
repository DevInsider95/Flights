using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public class Aircraft : BaseCRUD
    {

        [Required(ErrorMessage = "Le nom de l'avion est requis")]
        [Display(Name = "Nom de l'avion")]
        public string AircraftName { get; set; }

        /// <summary>
        /// Consommation de carburant pour 1 KM/h
        /// </summary>
        [Required(ErrorMessage = "La consommation du carburant est requise")]
        [Display(Name = "Consommation du carburant (l/km/h)")]
        public float FuelConsumption_Per_KM_Per_Hour { get; set; }

        /// <summary>
        /// Consommation de carburant au décollage
        /// </summary>
        [Required(ErrorMessage = "La consommation de carburant au décollage est requise")]
        [Display(Name = "Consommation du carburant au décollage (l/km/h)")]
        public float TakeOffEffort { get; set; }

        /// <summary>
        /// Réserve nécéssaire en cas de problème pendant le vol
        /// </summary>
        [Required(ErrorMessage = "La réserve de carburant est requise")]
        [Display(Name = "Réserve de carburant (l/km/h)")]
        public float RequiredFuelReserve { get; set; }

        /// <summary>
        /// Temps de préparation de l'avion
        /// </summary>
        [Required(ErrorMessage = "Le temps de préparation de l'avion est requis")]
        [Display(Name = "Temps de préparation de l'avion")]
        public TimeSpan PreparationTimeSpan { get; set; }

        /// <summary>
        /// Nombre de personnes maximales pour l'avion
        /// </summary>
        [Required(ErrorMessage = "La capacité totale des passagers est requise")]
        [Display(Name = "Capacité totale des passagers")]
        public int TotalCapacity { get; set; }

        public List<Flight> Flights { get; set; }

        /// <summary>
        /// Constructeur sans paramètres
        /// </summary>
        public Aircraft() : base()
        {
        }

        /// <summary>
        /// Constructeur d'un avion
        /// </summary>
        /// <param name="aircraftName">Nom de l'avion</param>
        /// <param name="fuelConsumption_Per_KM_Per_Hour">Consommation de carburant pour 1 KM/h</param>
        /// <param name="takeOffEffort">Consommation de l'avion au décollage</param>
        /// <param name="requiredFuelReserve">Réserve de carburant nécéssaire</param>
        /// <param name="preparationTimeSpan">Temps de préparation de l'avion</param>
        /// <param name="totalCapacity">Capacité totale de personnes</param>
        public Aircraft(string aircraftName, float fuelConsumption_Per_KM_Per_Hour, float takeOffEffort, float requiredFuelReserve, TimeSpan preparationTimeSpan, int totalCapacity)
        {
            this.Id = Guid.NewGuid();
            this.AircraftName = aircraftName;
            this.FuelConsumption_Per_KM_Per_Hour = fuelConsumption_Per_KM_Per_Hour;
            this.TakeOffEffort = takeOffEffort;
            this.RequiredFuelReserve = requiredFuelReserve;
            this.PreparationTimeSpan = preparationTimeSpan;
            this.TotalCapacity = totalCapacity;
            this.Status = EStatus.ACTIVE;
        }

        public override string ToString()
        {
            return AircraftName;
        }

        public bool CanBeReserved(DateTime dateTimeBegin, DateTime dateTimeEnd, IEnumerable<Flight> flights)
        {
            return flights
                .Where(s =>
                   (dateTimeBegin > s.DateTimeBegin && dateTimeBegin < s.DateTimeEnd) ||
                   (dateTimeEnd > s.DateTimeBegin && dateTimeEnd < s.DateTimeEnd) ||
                   (dateTimeBegin < s.DateTimeBegin && dateTimeEnd > s.DateTimeEnd))
                .Count() == 0;
        }

        /// <summary>
        /// Mise à jour du modèle
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override BaseCRUD Update(BaseCRUD originalEntity)
        {
            if (originalEntity is Aircraft aircraft)
            {
                this.Flights = aircraft.Flights;
                this.RowVersion = Guid.NewGuid();
            }

            return this;
        }
    }
}
