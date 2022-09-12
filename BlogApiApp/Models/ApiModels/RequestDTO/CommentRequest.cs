namespace Models.ApiModels
{
    public class CommentRequest
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
        public int PostId { get; set; }
    }
}