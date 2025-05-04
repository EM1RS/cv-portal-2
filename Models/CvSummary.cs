namespace CvAPI2.Models
{
    public class CvSummary
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string CvId { get; set; }
        public Cv Cv { get; set; }
        public string SummaryText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

}