using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookRentalService.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime RentalDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
