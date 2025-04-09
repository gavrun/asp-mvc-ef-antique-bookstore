using System.ComponentModel.DataAnnotations.Schema;

namespace AntiqueBookstore.Domain.Entities
{
    public class Book
    {
        // Entity properties
        public int Id { get; set; } // NOTE: named PK
        public string Title { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public int PublicationDate { get; set; } // Year of publication

        // Explicitly set the column type over Fluent API
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? PurchasePrice { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal? RecommendedPrice { get; set; }

        // Cover placeholder if null
        public string? CoverImagePath { get; set; } 


        // Foreign key to BookCondition
        public int ConditionId { get; set; } // FK 

        public virtual BookCondition Condition { get; set; } = null!; // Required

        // Foreign key to BookStatus
        public int StatusId { get; set; } // FK 

        public virtual BookStatus Status { get; set; } = null!; // Required

        //
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        //
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();


        // NOTE: Optionally add unmapped property for easy access to Authors
        [NotMapped]
        public IEnumerable<Author> Authors => BookAuthors.Select(ba => ba.Author);
    }
}
