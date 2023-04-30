namespace Models.Query
{
    public class PostFilterParams : PaginationQueryParams
    {
        public string? UsreId { get; set; } 
        public int? BlogId { get; set; } 
        public string? Tag { get; set; }
        public bool MostLiked { get; set; } = false;
    }
}
