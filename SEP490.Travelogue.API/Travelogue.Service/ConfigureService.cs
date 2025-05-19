using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travelogue.Service;
using Travelogue.Service.Commons.Implementations;

namespace Travelogue.Service;

public static class ConfigureService
{
    public static IServiceCollection ConfigureServiceLayerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper();
        services.AddServices(configuration);

        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.AddScoped<ICloudinaryService, CloudinaryService>();

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
        //services.AddScoped<IHotelService, HotelService>();
        //services.AddScoped<IRestaurantService, RestaurantService>();
        //services.AddScoped<IDistrictService, DistrictService>();
    }
}