using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BorrowingsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/borrowings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowingResponseDto>>> GetAll()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Select(b => new BorrowingResponseDto
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    MemberId = b.MemberId,
                    MemberName = b.Member.Name,
                    BorrowedDate = b.BorrowedDate,
                    DueDate = b.DueDate,
                    ReturnedDate = b.ReturnedDate,
                    IsReturned = b.IsReturned
                }).ToListAsync();

            return Ok(borrowings);
        }

        // GET: api/borrowings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingResponseDto>> GetById(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
                return NotFound(new { message = $"Borrowing record with ID {id} not found." });

            return Ok(new BorrowingResponseDto
            {
                Id = borrowing.Id,
                BookId = borrowing.BookId,
                BookTitle = borrowing.Book.Title,
                MemberId = borrowing.MemberId,
                MemberName = borrowing.Member.Name,
                BorrowedDate = borrowing.BorrowedDate,
                DueDate = borrowing.DueDate,
                ReturnedDate = borrowing.ReturnedDate,
                IsReturned = borrowing.IsReturned
            });
        }

        // POST: api/borrowings
        [HttpPost]
        public async Task<IActionResult> BorrowBook(BorrowingDto dto)
        {
            var book = await _context.Books.FindAsync(dto.BookId);
            if (book == null)
                return NotFound(new { message = "Book not found." });

            if (!book.IsAvailable)
                return BadRequest(new { message = "Book is currently not available." });

            var member = await _context.Members.FindAsync(dto.MemberId);
            if (member == null)
                return NotFound(new { message = "Member not found." });

            var borrowing = new Borrowing
            {
                BookId = dto.BookId,
                MemberId = dto.MemberId,
                DueDate = dto.DueDate
            };

            book.IsAvailable = false;
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book borrowed successfully.", borrowingId = borrowing.Id });
        }

        // PUT: api/borrowings/5/return
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
                return NotFound(new { message = "Borrowing record not found." });

            if (borrowing.IsReturned)
                return BadRequest(new { message = "Book has already been returned." });

            borrowing.IsReturned = true;
            borrowing.ReturnedDate = DateTime.UtcNow;
            borrowing.Book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Book returned successfully." });
        }

        // DELETE: api/borrowings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var borrowing = await _context.Borrowings.FindAsync(id);

            if (borrowing == null)
                return NotFound(new { message = "Borrowing record not found." });

            _context.Borrowings.Remove(borrowing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
