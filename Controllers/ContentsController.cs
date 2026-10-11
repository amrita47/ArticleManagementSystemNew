
using ArticleManagementSystemNew.Data;
using ArticleManagementSystemNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticleManagementSystemNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContentsController(AppDbContext context)
        {
            _context = context;
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Content>>> GetContents()
        {
            return await _context.Contents
                .AsNoTracking()
                .ToListAsync();
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<Content>> GetContent(int id)
        {
            var content = await _context.Contents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
                return NotFound();

            return content;
        }

      
        [HttpPost]
        public async Task<ActionResult<Content>> CreateContent(Content content)
        {
            content.Id = 0;
            content.CreatedAt = DateTime.UtcNow;

            if (!Enum.IsDefined(content.Status))
                return BadRequest("Invalid status.");

            if (!Enum.IsDefined(content.Language))
                return BadRequest("Invalid language.");

            var authorExists = await _context.Users
                .AnyAsync(u => u.Id == content.Author);

            if (!authorExists)
                return BadRequest("The specified author does not exist.");

            var articleExists = await _context.Articles
                .AnyAsync(a => a.Id == content.ArticleId);

            if (!articleExists)
                return BadRequest("The specified article does not exist.");

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetContent),
                new { id = content.Id },
                content);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContent(int id, Content content)
        {
            if (id != content.Id)
                return BadRequest(
                    "The ID in the URL must match the content ID.");

            var existingContent = await _context.Contents.FindAsync(id);

            if (existingContent == null)
                return NotFound();

            if (!Enum.IsDefined(content.Status))
                return BadRequest("Invalid status.");

            if (!Enum.IsDefined(content.Language))
                return BadRequest("Invalid language.");

            var authorExists = await _context.Users
                .AnyAsync(u => u.Id == content.Author);

            if (!authorExists)
                return BadRequest("The specified author does not exist.");

            var articleExists = await _context.Articles
                .AnyAsync(a => a.Id == content.ArticleId);

            if (!articleExists)
                return BadRequest("The specified article does not exist.");

            existingContent.Title = content.Title;
            existingContent.ContentData = content.ContentData;
            existingContent.Author = content.Author;
            existingContent.Status = content.Status;
            existingContent.Language = content.Language;
            existingContent.ArticleId = content.ArticleId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Contents.FindAsync(id);

            if (content == null)
                return NotFound();

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
