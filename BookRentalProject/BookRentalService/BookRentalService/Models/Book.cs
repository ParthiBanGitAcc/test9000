using System.ComponentModel.DataAnnotations;

namespace BookRentalService.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int RentalCount { get; set; } // To track the number of times rented

        public List<Rental> Rentals { get; set; } // Navigation property for rentals
    }
}
