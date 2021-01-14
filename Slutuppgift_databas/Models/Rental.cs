using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slutuppgift_databas.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        [Required]
        public string Rented { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }

        public bool Returned
        {
            get
            {                
                return ReturnDate != null;
            }
        }
    }
}
