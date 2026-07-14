using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationsForFinance.models;

namespace ApplicationsForFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppFinanceDbContext _context;

        public TransactionsController(AppFinanceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все транзакции (за всё время)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.Include(t => t.Article).ThenInclude(a => a.Category).ToListAsync();
        }

        /// <summary>
        /// Получить транзакции за конкретный день с цветовым стикером (зеленый/желтый/красный)
        /// </summary>
        [HttpGet("day/{date}")]
        public async Task<IActionResult> GetTransactionsByDay(DateOnly date)
        {
            var transactions = await _context.Transactions.Where(t => t.Date == date).Include(t => t.Article).ToListAsync();

            var totalSum = transactions.Sum(t => t.Sum);

            // Расчет стикеров
            string stickerColor;
            if (totalSum < 500)
            {
                stickerColor = "Зеленый (экономно)";
            }
            else if (totalSum <= 2000)
            { 
                stickerColor = "Желтый (в пределах обычного)";
            }
            else
            {
                stickerColor = "Красный (затратный день)";
            }

            return Ok(new
            {
                Date = date,
                TotalSum = totalSum,
                Sticker = stickerColor,
                Transactions = transactions
            });
        }

        /// <summary>
        /// Получить транзакции за конкретный месяц
        /// </summary>
        [HttpGet("month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByMonth(int year, int month)
        {
            var transactions = await _context.Transactions.Where(t => t.Date.Year == year && t.Date.Month == month).Include(t => t.Article).ToListAsync();

            return Ok(transactions);
        }

        /// <summary>
        /// Создать транзакцию
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            if (transaction.Sum <= 0)
            {
                return BadRequest("Сумма транзакции должна быть положительным числом.");
            }

            var article = await _context.Articles.FindAsync(transaction.IdArticle);
            if (article == null)
            {
                return BadRequest("Указанная статья расходов не существует.");
            }

            //Неактивные статьи нельзя выбрать при заведении транзакции
            if (!article.IsActive)
            {
                return BadRequest("Нельзя создать транзакцию по неактивной статье расходов.");
            }

            if (transaction.Date == default)
            {
                transaction.Date = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            //Запрещено суммарно тратить более 1000000 рублей в день
            var dailyTotal = await _context.Transactions.Where(t => t.Date == transaction.Date).SumAsync(t => t.Sum);

            if (dailyTotal + transaction.Sum > 1000000)
            {
                return BadRequest($"Превышен суточный лимит в 1 000 000 рублей. Сегодня уже потрачено: {dailyTotal} руб.");
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// Изменить существующую транзакцию
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction updatedTransaction)
        {
            if (id != updatedTransaction.Id)
                return BadRequest("ID в пути и в теле запроса не совпадают.");

            //Загружаем оригинальную транзакцию
            var existingTransaction = await _context.Transactions.Include(t => t.Article).FirstOrDefaultAsync(t => t.Id == id);

            if (existingTransaction == null)
                return NotFound("Транзакция не найдена.");

            if (existingTransaction.Article != null && !existingTransaction.Article.IsActive)
            {
                // Если статья неактивна
                if (existingTransaction.IdArticle != updatedTransaction.IdArticle)
                {
                    return BadRequest("Запрещено менять статью расходов у этой транзакции, так как текущая статья стала неактивной.");
                }
            }

            //Проверяем суточный лимит в 1000000 руб
            var dailyTotal = await _context.Transactions.Where(t => t.Date == updatedTransaction.Date && t.Id != id).SumAsync(t => t.Sum);

            if (dailyTotal + updatedTransaction.Sum > 1000000)
            {
                return BadRequest($"Изменение отклонено: Общая сумма за {updatedTransaction.Date} превысит лимит в 1000000 руб.");
            }

            //переносим обновленные свойства в отслеживаемый объект
            existingTransaction.Sum = updatedTransaction.Sum;
            existingTransaction.Comment = updatedTransaction.Comment;
            existingTransaction.Date = updatedTransaction.Date;
            existingTransaction.IdArticle = updatedTransaction.IdArticle;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Transactions.AnyAsync(t => t.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Удалить транзакцию
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound("Транзакция не найдена.");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}