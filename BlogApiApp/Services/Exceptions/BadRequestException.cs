using System.Net;

namespace Services.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.NotFound;
        }
    }
}