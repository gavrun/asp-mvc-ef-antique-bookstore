using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class PositionHistoryViewModel
    {
        [Display(Name = "Position")]
        public string PositionTitle { get; set; } = string.Empty;

        [Display(Name = "Level")]
        public string LevelName { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)] // make it look like a date picker
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } // Nullable, if the position is current
    }
}
