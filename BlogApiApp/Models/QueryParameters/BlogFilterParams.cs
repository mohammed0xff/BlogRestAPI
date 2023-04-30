namespace Models.Query
{
    public class BlogFilterParams : PaginationQueryParams
    {
        public string? Username { get; set; }
        public bool Popular { get; set; } = false;
    }
}
