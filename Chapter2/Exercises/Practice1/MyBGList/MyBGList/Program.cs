var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// resolve conflict dupliate route, which is always "bad practic" should delete duplicate code instead!
builder.Services.AddSwaggerGen(opts =>
    opts.ResolveConflictingActions(apiDesc => apiDesc.First())
);

var app = builder.Build();

// Configure the HTTP request pipeline.

// this is another solution for exercise
//if (app.Environment.IsDevelopment() || app.Environment.IsStaging()) 
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");


app.UseHttpsRedirection();

app.UseAuthorization();


// Minimal API
app.MapGet("/error", () => Results.Problem());
app.MapGet("/error/test", () => { throw new Exception("test"); });

// Controller
app.MapControllers();

app.Run();
