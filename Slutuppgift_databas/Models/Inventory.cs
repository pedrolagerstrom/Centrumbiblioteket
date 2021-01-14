using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slutuppgift_databas.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }        
        public List<Rental> Rentals { get; set; }

        public bool Available
        {
            get
            {
                if (Rentals == null)
                    return true;
                
                else if (Rentals.Count == 0)
                    return true;
                
                else if (Rentals.All(r => r.Returned))
                    return true;
                
                else
                {
                    return false;
                }
            }
        }
    }
}
