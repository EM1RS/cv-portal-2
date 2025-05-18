namespace CvAPI2.Models.Tag
{    
    public class Tag

    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Value { get; set; } = string.Empty;
    }
}