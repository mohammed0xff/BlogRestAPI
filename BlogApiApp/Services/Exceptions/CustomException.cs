using System.Net;

namespace Services.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            List<string> errors = default) : base(message)
        {
            ErrorMessages = errors;
            StatusCode = statusCode;
        }

        public IEnumerable<string> ErrorMessages { get; protected set; }

        public HttpStatusCode StatusCode { get; protected set; }
    }
}