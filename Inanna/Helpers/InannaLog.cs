using Gaia.Models;
using Microsoft.Extensions.Logging;

namespace Inanna.Helpers;

public static partial class InannaLog
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Command exception")]
    public static partial void CommandException(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Command errors: {Errors}")]
    public static partial void CommandErrors(this ILogger logger, List<ValidationError> errors);
}
