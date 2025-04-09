using Amazon.Lambda.Core;
//using Amazon.Runtime.Telemetry.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Trace;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AspireDemo.DataDispatcherFunction;

public class Function
{
    IHost _host;
    TracerProvider _traceProvider;

    public Function()
    {
        var builder = new HostApplicationBuilder();
        builder.AddServiceDefaults();
        _host = builder.Build();
        _traceProvider = _host.Services.GetRequiredService<TracerProvider>();
    }
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public Casing FunctionHandler(string input, ILambdaContext context)
    {
        return AWSLambdaWrapper.Trace(_traceProvider, (request, context) => new Casing(input.ToLower(), input.ToUpper()), input, context);
    }

}

public record Casing(string Lower, string Upper);