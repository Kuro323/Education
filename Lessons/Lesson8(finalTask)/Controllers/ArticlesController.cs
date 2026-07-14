using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationsForFinance.models;

namespace ApplicationsForFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppFinanceDbContext _context;

        public ArticlesController(AppFinanceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех статей расходов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.Include(a => a.Category).ToListAsync();
        }

        /// <summary>
        /// Получить статью по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id);
            if (article == null) return NotFound("Статья не найдена.");
            return article;
        }

        /// <summary>
        /// Создать новую статью расходов
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle(Article article)
        {
            // Провекар существует ли указанная категория
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == article.IdCategory);
            if (!categoryExists)
            {
                return BadRequest("Указанная категория расходов не существует.");
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        /// <summary>
        /// Изменить статью расходов
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Article article)
        {
            if (id != article.Id) return BadRequest("ID не совпадают.");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == article.IdCategory);
            if (!categoryExists) return BadRequest("Указанная категория расходов не существует.");

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Articles.AnyAsync(a => a.Id == id))
                    return NotFound("Статья не найдена.");
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Удалить статью расходов
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound("Статья не найдена.");

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}