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
    readonly IHost _host;
    readonly TracerProvider _traceProvider;

    public Function()
    {
        var builder = new HostApplicationBuilder();
        builder.AddServiceDefaults();
        builder.AddSqlServerClient(connectionName: "database");
        _host = builder.Build();
        _traceProvider = _host.Services.GetRequiredService<TracerProvider>();
    }
    
    public Casing FunctionHandler(string input, ILambdaContext context)
    {
        return AWSLambdaWrapper.Trace(_traceProvider, (request, context) => new Casing(input.ToLower(), input.ToUpper()), input, context);
    }

}

public record Casing(string Lower, string Upper);