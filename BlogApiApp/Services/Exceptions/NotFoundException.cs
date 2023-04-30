using System.Net;

namespace Services.Exceptions
{
    public abstract class NotFoundException : CustomException
    {
        protected NotFoundException(string message)
        : base(message)
        { 
            StatusCode = HttpStatusCode.NotFound;
        }
    }
}
