
using ArticleManagementSystemNew.Data;
using ArticleManagementSystemNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticleManagementSystemNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetArticles(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? sortBy = null,
            string? language = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "Page number must be positive and page size must be between 1 and 100.");
            }

            var query = _context.Articles.AsNoTracking().AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<Status>(status, true, out var parsedStatus)
                    || !Enum.IsDefined(parsedStatus))
                {
                    return BadRequest(
                        "Invalid status. Use Draft, Published, or Unpublished.");
                }

                query = query.Where(a => a.Status == parsedStatus);
            }

            Language? parsedLanguage = null;

            if (!string.IsNullOrWhiteSpace(language))
            {
                if (!Enum.TryParse<Language>(
                        language, true, out var selectedLanguage)
                    || !Enum.IsDefined(selectedLanguage))
                {
                    return BadRequest(
                        "Invalid language. Use English, French, or Spanish.");
                }

                parsedLanguage = selectedLanguage;
            }

            if (parsedLanguage.HasValue)
            {
  query = query.Where(a => a.Contents
                .Any(c => c.Language == parsedLanguage.Value));
            }

            if (string.Equals(
                    sortBy, "title", StringComparison.OrdinalIgnoreCase))
            {
                query = query
                    .OrderBy(a => a.Contents
                        .Where(c => c.Language == Language.English)
                        .Select(c => c.Title)
                        .FirstOrDefault() == null ? 1 : 0)
                    .ThenBy(a => a.Contents
                        .Where(c => c.Language == Language.English)
                        .Select(c => c.Title)
                   .FirstOrDefault())
                    .ThenBy(a => a.Id);
            }
            else
            {
                query = query
                    .OrderByDescending(a => a.CreatedAt)
                    .ThenBy(a => a.Id);
            }


            var totalCount = await query.CountAsync();
            Console.WriteLine(query.ToQueryString());
            var articles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.Id,
                    Title = a.Contents
                        .Where(c => parsedLanguage == null
                            || c.Language == parsedLanguage)
                        .OrderBy(c =>
                            c.Language == Language.English ? 0 : 1)
                        .ThenBy(c => c.Title)
                        .Select(c => c.Title)
                        .FirstOrDefault(),
                    Author = a.Contents
                        .Where(c => parsedLanguage == null
                            || c.Language == parsedLanguage)
                        .OrderBy(c =>
                            c.Language == Language.English ? 0 : 1)
                        .ThenBy(c => c.Title)
                        .Select(c => c.AuthorUser != null
                            ? c.AuthorUser.Username
                            : null)
                        .FirstOrDefault(),
                    a.Status,
                    a.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                articles
            });
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return NotFound();

            return article;
        }
      
        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle(Article article)
        {
            article.Id = 0;
            article.CreatedAt = DateTime.UtcNow;

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetArticle),
                new { id = article.Id },
                article);
        }

       
    [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(
            int id, Article article)
        {
            if (id != article.Id)
                return BadRequest(
 "The ID in the URL must match the article ID.");

            var existingArticle = await _context.Articles.FindAsync(id);

            if (existingArticle == null)
                return NotFound();

            existingArticle.Status = article.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }
   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
                return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetArticleDetails(int id)
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Status,
                    a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound(
                    new { message = "Article not found." });
            }

            var contents = await _context.Contents
                .AsNoTracking()
                .Where(c => c.ArticleId == id)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    Content = c.ContentData,
                    c.Author,
                    c.Status,
                    c.CreatedAt,
                    c.Language
                })
                .ToListAsync();

            return Ok(new
            {
                article.Id,
                article.Status,
                article.CreatedAt,
                Contents = contents
            });
        }
    }
}
