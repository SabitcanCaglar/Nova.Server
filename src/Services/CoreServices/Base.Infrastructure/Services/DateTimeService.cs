using Base.Application.Common.Interfaces;

namespace Base.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
