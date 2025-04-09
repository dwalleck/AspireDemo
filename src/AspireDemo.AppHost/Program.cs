#pragma warning disable CA2252

using Amazon.Lambda;

using Aspire.Hosting.AWS.Lambda;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("database");

builder.AddAWSLambdaFunction<Projects.AspireDemo_DataDispatcherFunction>(
    "Casing",
    lambdaHandler: "AspireDemo.DataDispatcherFunction::AspireDemo.DataDispatcherFunction.Function::FunctionHandler",
    new LambdaFunctionOptions
    {
        LogFormat = LogFormat.JSON,
        ApplicationLogLevel = ApplicationLogLevel.DEBUG,
    })
        .WithReference(db)
        .WaitFor(db);

builder.Build().Run();
