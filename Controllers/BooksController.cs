using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll()
        {
            var books = await _context.Books
                .Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublishedYear = b.PublishedYear,
                    IsAvailable = b.IsAvailable
                }).ToListAsync();

            return Ok(books);
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            return Ok(new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                IsAvailable = book.IsAvailable
            });
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookResponseDto>> Create(BookDto dto)
        {
            bool isbnExists = await _context.Books
                .AnyAsync(b => b.ISBN == dto.ISBN);

            if (isbnExists)
                return Conflict(new { message = "A book with this ISBN already exists." });

            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                PublishedYear = dto.PublishedYear
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var response = new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                IsAvailable = book.IsAvailable
            };

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, response);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookDto dto)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            bool isbnTaken = await _context.Books
                .AnyAsync(b => b.ISBN == dto.ISBN && b.Id != id);

            if (isbnTaken)
                return Conflict(new { message = "Another book already has this ISBN." });

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.ISBN = dto.ISBN;
            book.PublishedYear = dto.PublishedYear;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found." });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
