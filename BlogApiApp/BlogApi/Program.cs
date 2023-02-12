using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using DataAccess;
using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Implementation;
using Services;
using Services.Extensions;
using Services.Options;
using BlogApi.Filters;
using Serilog;
using Services.Storage;

var Builder = WebApplication.CreateBuilder(args);

Builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging(true)
    );

Builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
Builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
Builder.Services.AddScoped<IAuthService, AuthService>();
Builder.Services.AddScoped<IStorageService, StorageService>();
Builder.Services.Configure<JWTOptions>(Builder.Configuration.GetSection("JWT"));
Builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
Builder.Services.AddHttpContextAccessor();

Builder.Services.AddControllers();
Builder.Services.AddScoped<SuspenededActionFilter>();
Builder.Services.AddRouting(options => options.LowercaseUrls = true);

// logging 
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
Builder.Logging.ClearProviders();
Builder.Logging.AddSerilog(logger);

// Identity configuration
Builder.AddCustomIdentity();

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
    ClockSkew = TimeSpan.Zero,

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

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), @"Resources/Images")),
    RequestPath = new PathString("/app-images")
});

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.SeedDataAsync();

app.Run();