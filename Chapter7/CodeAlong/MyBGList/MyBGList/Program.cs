using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.Models;
using MyBGList.Swagger;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging
    .ClearProviders()
    .AddSimpleConsole()
    .AddDebug()
    .AddApplicationInsights(
        telemetary => telemetary.ConnectionString = builder.Configuration["Azure:ApplicationInsights:ConnectionString"],
        loggerOptions => { }
    );

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
    lc.Enrich.WithMachineName();
    lc.Enrich.WithThreadId();
    lc.WriteTo.File("Logs/log.txt",
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] [{MachineName} #{ThreadId}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day);
    lc.WriteTo.MSSqlServer(
        connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "LogEvents",
            AutoCreateSqlTable = true
        },
        columnOptions: new ColumnOptions()
        {
            AdditionalColumns = new SqlColumn[]
            {
                new SqlColumn()
                {
                    ColumnName = "SourceContext",
                    PropertyName = "SourceContext",
                    DataType = System.Data.SqlDbType.NVarChar
                }
            }
        });
},
    writeToProviders: true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin",
        cfg =>
        {
            cfg.AllowAnyOrigin();
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
        });
});

builder.Services.AddControllers(options =>
{
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"The value '{x}' is invalid.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => $"The field {x} must be a number");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"The value '{x}' is not valid for {y}.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => $"A value is required.");
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ParameterFilter<SortColumnFilter>();
    options.ParameterFilter<SortOrderFilter>();
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// This code is apply to all controller, which we replace by using [ManualValidationFilter] attribute affect only action method that apply. 
// This setting suppresses the filter that automatically return a `BadRequestObjectResult` when ModelState is invalid.
// builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(action =>
    {
        action.Run(async context =>
        {
            var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

            var details = new ProblemDetails();
            details.Detail = exceptionHandler?.Error.Message;
            details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            details.Status = StatusCodes.Status500InternalServerError;

            // Normally, when you return an object from a controller action, the framework automatically takes care of serializing it into JSON and writing it to the HTTP response.
            // However, in some cases�such as when handling exceptions globally or writing custom responses directly to the HTTP response stream�you have to manually handle serialization and response writing.
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(details));
        });
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();


// Minimal
app.MapGet("/error", 
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)]
    (HttpContext context) =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

        // TODO: logging, sending notification, and more
        var details = new ProblemDetails();
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
        details.Type =  "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = StatusCodes.Status500InternalServerError;

        app.Logger.LogError(CustomLogEvents.Error_Get, exceptionHandler?.Error, "An unhandled exception occurred.");

        return Results.Problem(details);
    });

app.MapGet("/error/test", 
    [EnableCors("AnyOrigin")] 
    [ResponseCache(NoStore = true)]
    () => { throw new Exception("test"); });
app.MapGet("/cod/test",
   [EnableCors("AnyOrigin")]
   [ResponseCache(NoStore = true)] () =>
   Results.Text("<script>" +
       "window.alert('Your client supports JavaScript!" +
       "\\r\\n\\r\\n" +
       $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
       "\\r\\n" +
       "Client time (UTC): ' + new Date().toISOString());" +
       "</script>" +
       "<noscript>Yor client does not support JavaScript</noscript>",
       "text/html"));

// Controller
app.MapControllers();

app.Run();
