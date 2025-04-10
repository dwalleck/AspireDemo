#pragma warning disable CA2252

using Amazon.Lambda;

using Aspire.Hosting;
using Aspire.Hosting.AWS.Lambda;

var builder = DistributedApplication.CreateBuilder(args);

// Add LocalStack as an external resource
var localstack = builder.AddContainer("localstack", "localstack/localstack:latest")
                        .WithEnvironment("DEFAULT_REGION", "us-east-1")
                        .WithEnvironment("SERVICES", "s3,dynamodb,sqs,sns")
                        .WithEnvironment("DEBUG", "1")
                        .WithEnvironment("DOCKER_HOST", "unix:///var/run/podman/podman.sock") // Point to Podman socket
                        .WithEnvironment("USE_SINGLE_REGION", "true") // Simplify for Podman
                        .WithEnvironment("DISABLE_CORS_CHECKS", "1")  // Disable CORS for easier testing
                        .WithEnvironment("SKIP_SSL_CERT_DOWNLOAD", "1") // Skip cert download
                        .WithEnvironment("LOCALSTACK_HOST", "0.0.0.0") // Listen on all interfaces
                        .WithEndpoint(name: "edge", port: 4566, targetPort: 4566, scheme: "http")
                        .WithEndpoint(name: "dashboard", port: 8080, targetPort: 8080, scheme: "http")
                        //.WithEnvironment("LAMBDA_EXECUTOR", "local")
                        .WithEnvironment("DOCKER_CLIENT", "false");

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("database");

var lambda = builder.AddAWSLambdaFunction<Projects.AspireDemo_DataDispatcherFunction>(
    "Casing",
    lambdaHandler: "AspireDemo.DataDispatcherFunction::AspireDemo.DataDispatcherFunction.Function::FunctionHandler",
    new LambdaFunctionOptions
    {
        LogFormat = LogFormat.JSON,
        ApplicationLogLevel = ApplicationLogLevel.DEBUG,
    })
        .WithReference(db)
        .WaitFor(db)
        .WithEnvironment("ConnectionStrings__localstack", "http://${resource:localstack.bindings.edge.host}:${resource:localstack.bindings.edge.port}");

builder.Build().Run();
