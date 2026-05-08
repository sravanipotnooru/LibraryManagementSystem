using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public MembersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetAll()
        {
            var members = await _context.Members
                .Select(m => new MemberResponseDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Phone = m.Phone,
                    MemberSince = m.MemberSince
                }).ToListAsync();

            return Ok(members);
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberResponseDto>> GetById(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
                return NotFound(new { message = $"Member with ID {id} not found." });

            return Ok(new MemberResponseDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                MemberSince = member.MemberSince
            });
        }

        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<MemberResponseDto>> Create(MemberDto dto)
        {
            bool emailExists = await _context.Members
                .AnyAsync(m => m.Email == dto.Email);

            if (emailExists)
                return Conflict(new { message = "A member with this email already exists." });

            var member = new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var response = new MemberResponseDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                MemberSince = member.MemberSince
            };

            return CreatedAtAction(nameof(GetById), new { id = member.Id }, response);
        }

        // PUT: api/members/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MemberDto dto)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
                return NotFound(new { message = $"Member with ID {id} not found." });

            bool emailTaken = await _context.Members
                .AnyAsync(m => m.Email == dto.Email && m.Id != id);

            if (emailTaken)
                return Conflict(new { message = "Another member already has this email." });

            member.Name = dto.Name;
            member.Email = dto.Email;
            member.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
                return NotFound(new { message = $"Member with ID {id} not found." });

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
