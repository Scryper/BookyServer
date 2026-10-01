namespace BookyServer.Api.Middlewares;

internal static partial class ExceptionHandlerLog
{
    private const string UnhandledExceptionMessage = "An unhandled exception occurred while processing the request.";

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = UnhandledExceptionMessage)]
    internal static partial void UnhandledException(ILogger logger, Exception exception);
}
