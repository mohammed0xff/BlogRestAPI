namespace Services.Authentication.Session
{
    public interface ISession
    {
        public string UserId { get; }
        public string Username { get; }
        public bool IsAuthenticated { get; }
        public bool IsAdmin { get; }
        public DateTime Now { get; }
    }
}
