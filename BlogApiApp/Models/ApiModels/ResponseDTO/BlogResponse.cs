
namespace Models.ApiModels
{
    public class BlogResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int FollowersCount { get; set; }
        public AppUserResponse User { get; set; }
    }
}

