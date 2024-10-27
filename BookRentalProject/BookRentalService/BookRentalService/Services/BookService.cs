using BookRentalService.Controllers;
using BookRentalService.Data;
using BookRentalService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookRentalService.Services
{
    public class BookService
    {
        private readonly BookRentalContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookService> _logger; // Add logger

        public BookService(BookRentalContext context, IConfiguration configuration, IEmailService emailService, ILogger<BookService> logger)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger; // Initialize logger
        }

        /// <summary>
        ///  This method is used to search the books
        /// </summary>
        /// <param name="title"></param>
        /// <param name="genre"></param>
        /// <returns></returns>
        public async Task<List<Book>> SearchBooks(string title, string genre)
        {

            _logger.LogInformation($"Search for booking '{title}'.");


            return await _context.Books
                .Where(b => (string.IsNullOrEmpty(title) || b.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(genre) || b.Genre == genre))
                .ToListAsync();
        }

        /// <summary>
        /// This method is used to rent book
        /// </summary>
        /// <param name="username"></param>
        /// <param name="bookname"></param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, Rental Rental, string ErrorMessage)> RentBook(string username, string bookname)
        {

            _logger.LogInformation($"User '{username}' is attempting to rent the book '{bookname}'.");


            // Find the book by name
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == bookname);
            if (book == null)
            {
                _logger.LogWarning($"Book '{bookname}' not found.");
                return (false, null, _configuration["Errors:BookNotFound"]);
            }

            if (!book.IsAvailable)
            {
                _logger.LogWarning($"Book is not'{bookname}' available.");
                return (false, null, _configuration["Errors:BookUnavailable"]);
            }

            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
            {
                _logger.LogWarning($"User is not'{username}' available.");
                return (false, null, _configuration["Errors:UserNotFound"]);
            }

            // Mark the book as unavailable
            book.IsAvailable = false;

            // Create a new rental record
            var rental = new Rental
            {
                UserId = user.Id,
                BookId = book.Id,
                RentalDate = DateTime.Now,
                ReturnDate = null // Not returned yet
            };

            _context.Rentals.Add(rental);
            book.RentalCount++;
            await _context.SaveChangesAsync();
            return (true, rental, null);
        }


        /// <summary>
        /// This method is used to return book
        /// </summary>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, string ErrorMessage)> ReturnBook(string bookName)
        {
            var rental = await _context.Rentals
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Book.Title.ToLower() == bookName.ToLower() && r.ReturnDate == null);

            if (rental == null)
                return (false, _configuration["Errors:RentalNotFound"]);
                

            if (rental.ReturnDate != null)
                return (false, _configuration["Errors:BookAlreadyReturn"]);

            // Update the return details
            rental.ReturnDate = DateTime.Now;
            rental.Book.IsAvailable = true;

            if (DateTime.Now > rental.RentalDate.AddDays(14))
            {
                rental.IsOverdue = true;
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }


        /// <summary>
        /// This method is used to get the Rental History ByUser
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<List<RentalHistoryDto>> GetRentalHistoryByUser(string userName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == userName);

            if (user == null)
            {
                throw new KeyNotFoundException(_configuration["Errors:UserNotFound"]);
            }

            var rentalHistory = await _context.Rentals
         .Where(r => r.UserId == user.Id)
         .Include(r => r.Book)
         .Select(r => new RentalHistoryDto(
             r.RentalDate,
             r.ReturnDate,
             r.Book.Title,
             r.ReturnDate == null && r.RentalDate.AddDays(14) < DateTime.Now // Inline logic here
          )).ToListAsync();

            return rentalHistory;
        }

        /// <summary>
        /// This method is used to get the book statistics
        /// </summary>
        /// <returns></returns>
        public async Task<BookStatisticsResultDto> GetBookStatistics()
        {

            await NotifyOverdueRentalsAsync();

            var overdueBooks = await _context.Rentals
                .Where(r => r.ReturnDate == null && r.RentalDate.AddDays(14) < DateTime.Now)
                .Select(r => new
                {
                    r.BookId,
                    r.Book.Title,
                    r.RentalDate
                })
                .ToListAsync();

            var mostOverdueBook = overdueBooks
                .GroupBy(b => b.BookId)
                .Select(g => new BookStatisticsDto
                {
                    BookName = g.First().Title,
                    RentalCount = g.Count(),
                    IsOverdue = true
                })
                .OrderByDescending(b => b.RentalCount)
                .FirstOrDefault();

            var mostPopularBook = await _context.Books
                .OrderByDescending(b => b.RentalCount)
                .Select(b => new BookStatisticsDto
                {
                    BookName = b.Title,
                    RentalCount = b.RentalCount,
                    IsOverdue = false // Most popular books are not overdue by definition
                })
                .FirstOrDefaultAsync();

            var leastPopularBook = await _context.Books
                .OrderBy(b => b.RentalCount)
                .Select(b => new BookStatisticsDto
                {
                    BookName = b.Title,
                    RentalCount = b.RentalCount,
                    IsOverdue = false // Least popular books are not overdue by definition
                })
                .FirstOrDefaultAsync();


          

            return new BookStatisticsResultDto
            {
                MostOverdue = mostOverdueBook,
                MostPopular = mostPopularBook,
                LeastPopular = leastPopularBook
            };



        }


        /// <summary>
        /// This method is used to Notify Overdue Rental in email
        /// </summary>
        /// <returns></returns>
        public async Task NotifyOverdueRentalsAsync()
        {
            var overdueRentals = await _context.Rentals
                .Where(r => r.ReturnDate == null && r.RentalDate.AddDays(14) < DateTime.Now)
                .Include(r => r.User) // Include the User entity
                .Include(r => r.Book)  // Include the Book entity
                .ToListAsync();

            foreach (var rental in overdueRentals)
            {
                var subject = "Your Rental is Overdue!";
                var message = $"Dear {rental.User.Name},\n\n" +
                              $"Your rental for '{rental.Book.Title}' is overdue. " +
                              $"Please return it at your earliest convenience.\n\n" +
                              "Thank you,\nBook Rental Service";

                await _emailService.SendEmailAsync(rental.User.Email, subject, message);
            }
        }


    }
}
