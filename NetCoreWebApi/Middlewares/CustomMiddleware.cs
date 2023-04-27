namespace NetCoreWebApi.Middlewares;
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomMiddleware> _logger;

    public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
    {
        _next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request data
        _logger.LogDebug("Request: {Method} {Path} {Query} {Headers} {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Request.Headers,
            await GetRequestBodyAsync(context.Request));

        // Save the response body to a memory stream
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        // Call the next middleware in the pipeline
        await _next(context);

        // Log response data
        _logger.LogDebug("Response: {StatusCode} {Headers} {Body}",
            context.Response.StatusCode,
            context.Response.Headers,
            await GetResponseBodyAsync(responseBodyStream));

        // Copy the memory stream to the original response body stream
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        context.Response.Body = originalResponseBodyStream;
    }

    private static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private static async Task<string> GetResponseBodyAsync(MemoryStream responseBodyStream)
    {
        responseBodyStream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(responseBodyStream, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        return body;
    }
}