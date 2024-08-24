using Auth.Application.Interfaces.Application;

namespace Auth.Infrastructure.Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
