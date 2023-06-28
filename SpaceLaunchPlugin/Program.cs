using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

const string SPACEX_ROCKET_LAUNCH_URL = "https://api.spacexdata.com/v5/launches/latest";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:7").AllowAnyHeader().AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChatGPT.Spacelaunch",
        Version = "v1",
        Description = "Plugin to get wow latest space launch data"
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
    });
});
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ChatGPT.Spacelaunch v1");

});

/// <summary>
/// Gets the weather for a latitude and longitude
/// </summary>
app.MapGet("/getlaunchinfo", async(int id) =>
{
    using (var httpClient = new HttpClient())
    {
        var url = SPACEX_ROCKET_LAUNCH_URL;
        var result = await httpClient.GetStringAsync(url);
        var jsonDocument = JsonDocument.Parse(result);

      var response = new GetSpaceXResponse(jsonDocument);
       return response;
    }        
})
.WithName("GetInfo")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets latest rocket launch info";    
    return generatedOperation;
});

//first time to add plugin
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

internal class GetSpaceXResponse
{
    public string flightNumber { get; set; }

    public string webcast { get; set; }

    public GetSpaceXResponse() { }

    public GetSpaceXResponse(JsonDocument jsonDoc)
    {
        var linksJsonElement = jsonDoc.RootElement.GetProperty("links");
        var wcast = linksJsonElement.GetProperty("webcast").ToString();

        this.webcast = wcast;
        var flnumber = jsonDoc.RootElement.GetProperty("flight_number").ToString();
        this.flightNumber = flnumber;

    }

    public GetSpaceXResponse(string flnumber,string wcast)
    {
        this.webcast = wcast;
        this.flightNumber = flnumber;
    }
}