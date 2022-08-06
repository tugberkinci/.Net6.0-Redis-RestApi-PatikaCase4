using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PatikaHomework4.Data.Context;
using PatikaHomework4.Service.IServices;
using PatikaHomework4.Service.Mapper;
using PatikaHomework4.Service.Services;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Redis",
        Description = "Redis Distrubited Cache \nPlease contact for sql dump file. ",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Rasim Tuðberk ÝNCÝ",
            Email = "r.tugberkinci@gmail.com",
            Url = new Uri("https://twitter.com/tugberkinci"),
        },
        License = new OpenApiLicense
        {
            Name = "Open-source license",
            //Url = new Uri("https://example.com/license"),
        }
    });

    

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RedisDemo_";
});

//add db context
builder.Services.AddDbContext<EfContext>(k =>
k.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection"))
);

//injections
builder.Services.AddScoped<IPersonService, PersonService>();

//mapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());

//redis 
//builder.Services.AddStackExchangeRedisCache(options => {
//    options.Configuration = builder.Configuration.GetConnectionString("Redis");
//    options.InstanceName = "RedisDemo_";
//});
var configurationOptions = new ConfigurationOptions();
configurationOptions.EndPoints.Add(builder.Configuration["Redis:Host"], Convert.ToInt32(builder.Configuration["Redis:Port"]));
int.TryParse(builder.Configuration["Redis:DefaultDatabase"], out int defaultDatabase);
configurationOptions.DefaultDatabase = defaultDatabase;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = configurationOptions;
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
