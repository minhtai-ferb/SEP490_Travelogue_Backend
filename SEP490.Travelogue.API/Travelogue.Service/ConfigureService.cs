using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using Travelogue.Service;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.SignalR;

namespace Travelogue.Service;

public static class ConfigureService
{
    public static IServiceCollection ConfigureServiceLayerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper();
        services.AddServices(configuration);

        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        PayOS payOS = new PayOS(configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
                            configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
                            configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
        //configuration["Environment:PAYOS_PARTNER_CODE"] ?? throw new Exception("Cannot find environment"));

        services.AddSingleton(payOS);
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        return services;
    }

    private static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var serviceInterfaces = types
            .Where(t => t.IsInterface && t.Name.EndsWith("Service"))
            .ToList();

        foreach (var serviceInterface in serviceInterfaces)
        {
            // Tìm class thực thi (implementation) có tên giống interface nhưng bỏ chữ "I" ở đầu
            var serviceImplementation = types.FirstOrDefault(t =>
                t.IsClass &&
                !t.IsAbstract &&
                serviceInterface.IsAssignableFrom(t));

            if (serviceImplementation != null)
            {
                services.AddScoped(serviceInterface, serviceImplementation);
            }
        }

        services.AddHttpContextAccessor();

        //services.AddScoped<ITimeService, TimeService>();
        //services.AddTransient<IEmailService, EmailService>();
        //services.AddScoped<IUserContextService, UserContextService>();
        //services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<IMediaCloudService, MediaCloudService>();
        //services.AddScoped<IEnumService, EnumService>();

        //services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        //services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IRoleService, RoleService>();
        //services.AddScoped<ILocationService, LocationService>();
        //services.AddScoped<ITypeLocationService, TypeLocationService>();
        //services.AddScoped<IExperienceService, ExperienceService>();
        //services.AddScoped<IEventService, EventService>();
        //services.AddScoped<ITypeEventService, TypeEventService>();
        //services.AddScoped<ICraftVillageService, CraftVillageService>();
        //services.AddScoped<ICuisineService, CuisineService>();
        //services.AddScoped<IDistrictService, DistrictService>();
    }
}