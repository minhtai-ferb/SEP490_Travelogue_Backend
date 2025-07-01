using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Travelogue.API;

public static class ConfigureService
{
    public static IServiceCollection ConfigureApiLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new CamelCaseParameterTransformer()));
        });
        //services.ConfigCors();
        services.ConfigSwagger();

        //services.AddScoped<UserManager<User>>();
        //services.AddScoped<RoleManager<IdentityRole>>();

        //services.AddIdentity<User, IdentityRole>(options =>
        //{
        //    // Cấu hình các yêu cầu mật khẩu
        //    options.Password.RequireDigit = false; // Yêu cầu có số trong mật khẩu
        //    options.Password.RequireLowercase = false; // Yêu cầu có chữ thường
        //    options.Password.RequireUppercase = false; // Yêu cầu có chữ hoa
        //    options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
        //    options.Password.RequiredLength = 6; // Độ dài tối thiểu của mật khẩu
        //    options.Password.RequiredUniqueChars = 1; // Yêu cầu có ít nhất 1 ký tự đặc biệt duy nhất

        //    // Cấu hình khóa tài khoản
        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Thời gian khóa sau khi thất bại đăng nhập
        //    options.Lockout.MaxFailedAccessAttempts = 15; // Số lần thất bại đăng nhập trước khi khóa tài khoản
        //    options.Lockout.AllowedForNewUsers = true;

        //    // Cấu hình yêu cầu cho User
        //    options.User.RequireUniqueEmail = true; // Yêu cầu email phải là duy nhất
        //})
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>()
        //    .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = "1039660801087-kfmhq7htdg9pnr6eq6e1hdnnn35drrca.apps.googleusercontent.com";
            options.ClientSecret = "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";
            options.Scope.Add("email");  // Yêu cầu quyền truy cập email
            options.Scope.Add("profile");  // Yêu cầu quyền truy cập thông tin hồ sơ
            options.SaveTokens = true;  // Lưu token vào AuthenticationProperties
            options.CallbackPath = new PathString("/signin-google");
        });

        services.AddAuthenJwt(configuration);

        return services;
    }

    public static void ConfigCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.WithOrigins("*")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    public static void ConfigSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API"

            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header use scheme Bearer.",
                Type = SecuritySchemeType.Http,
                Name = "Authorization",
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
            {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
                }
            });
            //c.OperationFilter<MultipartMixedOperationFilter>();
        }
        );
    }

    public static void AddAuthenJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing in configuration.")))
            };
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.Events = new JwtBearerEvents();
        });
    }
}

public class CamelCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;

        // Convert to camelCase
        string input = value.ToString()!;
        return Regex.Replace(input, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }
}