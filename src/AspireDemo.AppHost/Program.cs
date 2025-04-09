#pragma warning disable CA2252

using Amazon.Lambda;

using Aspire.Hosting.AWS.Lambda;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAWSLambdaFunction<Projects.AspireDemo_DataDispatcherFunction>(
    "Casing",
    lambdaHandler: "AspireDemo.DataDispatcherFunction::AspireDemo.DataDispatcherFunction.Function::FunctionHandler",
    new LambdaFunctionOptions
    {
        LogFormat = LogFormat.JSON,
        ApplicationLogLevel = ApplicationLogLevel.DEBUG,
    });

builder.Build().Run();
