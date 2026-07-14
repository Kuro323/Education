using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationsForFinance.models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public float Sum { get; set; }
        public string Comment { get; set; }

        public int IdArticle { get; set; }

        [ForeignKey("IdArticle")]
        public Article? Article { get; set; }
    }
}
