using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travelogue.Repository.Caching;
using Travelogue.Repository.Data;

namespace Travelogue.Repository;

public static class ConfigureService
{
    public static IServiceCollection ConfigureRepositoryLayerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //services.AddScoped<IUserRepository, UserRepository>();
        //services.AddScoped<IRoleRepository, RoleRepository>();
        //services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        //services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        //services.AddScoped<ILocationRepository, LocationRepository>();
        //services.AddScoped<IExperienceRepository, ExperienceRepository>();
        //services.AddScoped<IEventRepository, EventRepository>();
        //services.AddScoped<ICraftVillageRepository, CraftVillageRepository>();
        //services.AddScoped<ICuisineRepository, CuisineRepository>();
        //services.AddScoped<IMediaRepository, MediaRepository>();
        //services.AddScoped<IDistrictRepository, DistrictRepository>();
        //services.AddScoped<INewsRepository, NewsRepository>();
        //services.AddScoped<ITypeLocationRepository, TypeLocationRepository>();
        //services.AddScoped<ITypeEventRepository, TypeEventRepository>();
        //services.AddScoped<ITypeExperienceRepository, TypeExperienceRepository>();

        //var assembly = Assembly.GetExecutingAssembly();
        //var repositoryInterfaces = assembly.GetTypes()
        //    .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
        //    .ToList();

        //foreach (var repoInterface in repositoryInterfaces)
        //{
        //    var repoImplementation = assembly.GetTypes().FirstOrDefault(t =>
        //        t.IsClass && !t.IsAbstract && repoInterface.IsAssignableFrom(t));

        //    if (repoImplementation != null)
        //    {
        //        services.AddScoped(repoInterface, repoImplementation);
        //    }
        //}

        //services.AddRepositories(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddDatabase(configuration);

        AddCaching(services, configuration);

        return services;
        #region Identity Configuration (Removed)
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //services.AddIdentityCore<User>()
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();

        //services.AddIdentityCore<User>(options =>
        //{
        //    //options.Password.RequireDigit = true;
        //    //options.Password.RequireLowercase = true;
        //    //options.Password.RequireNonAlphanumeric = false;
        //    //options.Password.RequireUppercase = true;
        //    //options.Password.RequiredLength = 6;
        //    //options.Password.RequiredUniqueChars = 1;
        //})
        //.AddEntityFrameworkStores<ApplicationDbContext>();
        #endregion

    }

    public static void AddRepositories(this IServiceCollection services, Assembly assembly)
    {
        var repositoryTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IGenericRepository<>)));

        // filter out RepositoryBase<>
        var nonBaseRepos = repositoryTypes.Where(t => t != typeof(GenericRepository<>));

        foreach (var repositoryType in nonBaseRepos)
        {
            var interfaces = repositoryType.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IGenericRepository<>))
                .ToList();

            if (interfaces.Count != 1)
            {
                throw new InvalidOperationException($"Repository '{repositoryType.Name}' must implement only one interface that implements IRepositoryBase<T>.");
            }

            services.AddScoped(interfaces[0], repositoryType);
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContext<ApplicationDbContext>(options =>
        //{
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        //});

        //services.AddDbContext<ApplicationDbContext>(options =>
        //options.UseMySql(
        //    configuration.GetConnectionString("DefaultConnectionMySQL"),
        //    new MySqlServerVersion(new Version(8, 0, 41))
        //));

        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(
            configuration.GetConnectionString("DefaultConnectionMySQL"),
            ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnectionMySQL"))
        )
    );
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cache") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }
}
