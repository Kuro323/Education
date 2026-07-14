using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApplicationsForFinance.models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string NameArticle { get; set; }
        public bool IsActive { get; set; }

        public int IdCategory { get; set; }

        [ForeignKey("IdCategory")]
        public Category? Category { get; set; }

        public List<Transaction> Transactions { get; set; } = new();
    }
}
