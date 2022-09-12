namespace Models.ApiModels
{
    public class PostRequest
    {
        public int PostId { get; set; }
        public int BlogId { get; set; }
        public string HeadLine { get; set; }
        public string Content { get; set; }
        public bool CommentsAllowed { get; set; }
    }
}
