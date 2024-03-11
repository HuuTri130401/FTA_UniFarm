using System.Security.Claims;
using Capstone.UniFarm.API.Configurations;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.API.MiddleWares;
using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Repositories.Repository;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //============ Connect DB ============//
    builder.Services.AddDbContext<FTAScript_V1Context>(options =>
    {
        options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

    //============ Config CORS =============//
    // Them CORS cho tat ca moi nguoi deu xai duoc apis
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });


    //============ Identity =============//
    builder.Services.AddIdentity<Account, IdentityRole<Guid>>(options =>
        {
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = false;
            /*
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.AllowedUserNameCharacters += " ";
            */
            options.User.AllowedUserNameCharacters = null;

            // SignIn settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<FTAScript_V1Context>()
        .AddDefaultTokenProviders()
        .AddUserManager<CustomUserManager>();

    /*============== Google authentication ============ */
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            /*options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;*/
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        });

    //============ Config Odata =============//
    /*builder.Services.AddControllers()
        .AddOData(options => options
            .AddRouteComponents("odata", GetEdmModel())
            .Count()
            .Filter()
            .Expand()
            .Select()
            .OrderBy()
            .SetMaxTop(null));*/

    //============ Add auto mapper ============//
    builder.Services.AddAutoMapper(typeof(AutoMapperService));

    //builder.Services.AddScoped<FTAScript_V1Context>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IFarmHubRepository, FarmHubRepository>();
    builder.Services.AddScoped<IAreaRepository, AreaRepository>();
    builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
    builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
    builder.Services.AddScoped<IWalletRepository, WalletRepository>();
    builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();
    builder.Services.AddScoped<IProductItemRepository, ProductItemRepository>();
    builder.Services.AddScoped<IStationRepository, StationRepository>();
    builder.Services.AddScoped<ICollectedHubRepository, CollectedHubRepository>();
    builder.Services.AddScoped<IProductItemInMenuRepository, ProductItemInMenuRepository>();
    builder.Services.AddScoped<IBusinessDayRepository, BusinessDayRepository>();

    
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IFarmHubService, FarmHubService>();
    builder.Services.AddScoped<IAreaService, AreaService>();
    builder.Services.AddScoped<IApartmentService, ApartmentService>();
    builder.Services.AddScoped<IAccountRoleService, AccountRoleService>();
    builder.Services.AddScoped<IWalletService, WalletService>();
    builder.Services.AddScoped<IProductImageService, ProductImageService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IProductItemService, ProductItemService>();
    builder.Services.AddScoped<IProductItemInMenuService, ProductItemInMenuService>();
    builder.Services.AddScoped<IStationService, StationService>();
    builder.Services.AddScoped<ICollectedHubService, CollectedHubService>();
    builder.Services.AddScoped<IManageUsersService, ManageUsersService>();
    builder.Services.AddScoped<IBusinessDayService, BusinessDayService>();

    //============Configure logging============//
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();

    //============register this middleware to ServiceCollection============//
    builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAuthentication();
    builder.Services.AddSwaggerGen(c =>
    {
        c.EnableAnnotations();
        c.SwaggerDoc("v1", new OpenApiInfo()
        {
            Title = "Farm To Apartments",
            Description = "Building an e-commerce system to buy and sell agricultural products from farms to apartment residents",
            Version = "Version - 01"
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Bearer Authentication with JWT Token",
            Type = SecuritySchemeType.Http
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });
    }).AddSwaggerGen();

    //============Configure CORS============//
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors();
    app.Run();

    /*static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<AreaRequestCreate>("Areas");
        return builder.GetEdmModel();
    }*/
}
catch (Exception exception)
{
    // NLog: Catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}