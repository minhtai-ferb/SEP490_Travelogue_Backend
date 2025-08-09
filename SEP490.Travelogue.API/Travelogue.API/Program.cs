using System.Net;
using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Travelogue.API;
using Travelogue.API.Middlewares;
using Travelogue.Repository;
using Travelogue.Repository.Data;
using Travelogue.Service;
using Travelogue.Service.Commons.SignalR;
using Travelogue.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Custom application service configuration

System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.ConfigureRepositoryLayerService(builder.Configuration);
builder.Services.ConfigureServiceLayerService(builder.Configuration);
builder.Services.ConfigureApiLayerServices(builder.Configuration);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnectionMySQL"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnectionMySQL"))
        )
    );

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion End of custom application service configuration

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddHttpClient<MailTemplateService>();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("Config/serviceAccountKey.json")
});

builder.Services.AddSingleton(FirebaseAuth.DefaultInstance);

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var serviceProvider = scope.ServiceProvider;
//     await DataSeeder.SeedDataAsync(serviceProvider);
// }

//await using var scope = app.Services.CreateAsyncScope();
//var serviceProvider = scope.ServiceProvider;
//await DataSeeder.SeedDataAsync(serviceProvider);
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    await DataSeeder.SeedDataAsync(serviceProvider);
}

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.MapHub<NotificationHub>("/notificationHub");

var uploadImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
var uploadDocumentsPath = Path.Combine(Directory.GetCurrentDirectory(), "documents");

if (!Directory.Exists(uploadImagesPath))
{
    Directory.CreateDirectory(uploadImagesPath);
}

if (!Directory.Exists(uploadDocumentsPath))
{
    Directory.CreateDirectory(uploadDocumentsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "images")),
    RequestPath = "/images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "documents")),
    RequestPath = "/documents"
});


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
