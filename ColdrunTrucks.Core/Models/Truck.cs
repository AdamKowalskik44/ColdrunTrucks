using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ColdrunTrucks.Core.Models
{
    public interface ITruck
    {
        string Code { get; }
        string Name { get; }
        TruckStatus Status { get; }
        string? Description { get; }
        public string GetTruckStatus();
        public void SetTruckStatus(TruckStatus status);
    }

    public class Truck : ITruck
    {
        private readonly Dictionary<TruckStatus, string> _statuses;

        public string Code { get; set; }

        public string Name { get; set; }

        public TruckStatus Status { get; private set; }

        public string? Description { get; set; }


        public Truck(string code, string name, TruckStatus status, string description)
        {
            Code = code;
            Name = name;
            Status = status;
            Description = description;

            _statuses = new Dictionary<TruckStatus, string>() 
            {
                { TruckStatus.OutOfService, "Out Of Service" }, 
                { TruckStatus.Loading, "Loading" },
                { TruckStatus.ToJob, "To Job" },
                { TruckStatus.AtJob, "At Job" },
                { TruckStatus.Returning, "Returning" },
            };
        }

        public string GetTruckStatus() => _statuses[Status];

        public void SetTruckStatus(TruckStatus newStatus)
        {
            if (Status == TruckStatus.OutOfService || newStatus == TruckStatus.OutOfService || newStatus - 1 == Status || Status == TruckStatus.Returning && newStatus == TruckStatus.Loading)
                Status = newStatus;
            else 
                throw new InvalidTruckStatusException(string.Format("Invalid truck status. Current status:{0}, given status:{1}", GetTruckStatus(), newStatus));
        }
    }

    public enum TruckStatus
    {
        OutOfService,
        Loading,
        ToJob,
        AtJob,
        Returning
    }

    public class InvalidTruckStatusException : Exception
    {
        public InvalidTruckStatusException(string message) : base(message)
        {
            
        }
    } 
}
 