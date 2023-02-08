using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Implementation;
using Services;
using Services.Helpers;
using Models.Entities;
using Serilog;
using BlogApi.Filters;



var Builder = WebApplication.CreateBuilder(args);

Builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(Builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging(true)
    );

Builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
Builder.Services.AddScoped<IAuthService, AuthService>();
Builder.Services.Configure<JWT>(Builder.Configuration.GetSection("JWT"));
Builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
Builder.Services.AddHttpContextAccessor();
Builder.Services.AddControllers();

// logging 
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
Builder.Logging.ClearProviders();
Builder.Logging.AddSerilog(logger);

// Identity configuration
Builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.AllowedUserNameCharacters =
     "abcdefghijklmnopqrstuvwxyz0123456789";
})
 .AddEntityFrameworkStores<AppDbContext>();

// jwt configuration
var key = Encoding.UTF8.GetBytes(Builder.Configuration["JWT:Key"]);
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = false,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero
};
Builder.Services.AddSingleton(tokenValidationParams);

// Authentication configuration
Builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});

// add swaggerGen 
Builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogRestApi", Version = "v1" });
    c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
        In = ParameterLocation.Header,
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.OperationFilter<AuthResponsesOperationFilter>();
});



var app = Builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();
AppDbInitializer.SeedDataAsync(app).Wait();

app.Run();