namespace Models.Query
{
    public class BlogParameters : QueryStringParameters
    {
        public string? Username { get; set; }
        public bool Popular { get; set; } = false;
    }
}
