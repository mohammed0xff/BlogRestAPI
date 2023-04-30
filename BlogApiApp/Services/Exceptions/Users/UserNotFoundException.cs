namespace Services.Exceptions.Users
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string username)
            : base($"User with username : {username} Not found!")
        {
        }
    }
}
