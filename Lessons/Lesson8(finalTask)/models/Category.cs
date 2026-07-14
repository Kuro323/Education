using System.ComponentModel.DataAnnotations;

namespace ApplicationsForFinance.models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public float Budget { get; set; }
        public bool IsActive { get; set; }

        public List<Article> Articles { get; set; } = new();
    }
}
