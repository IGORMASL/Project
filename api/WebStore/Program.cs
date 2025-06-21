using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebStore.Data;
using WebStore.Repositories;
using WebStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
ConfigureServices(builder);

var app = builder.Build();

// Конфигурация middleware
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(WebApplicationBuilder webBuilder)
{
    // База данных с политикой повторных попыток
    webBuilder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(
            webBuilder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(webBuilder.Configuration.GetConnectionString("DefaultConnection")),
            mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

    // JWT Authentication
    var jwtSettings = webBuilder.Configuration.GetSection("JwtSettings");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

    webBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ClockSkew = TimeSpan.FromSeconds(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                        context.Response.Headers.Append("Token-Expired", "true");
                    return Task.CompletedTask;
                }
            };
        });

    // Регистрация сервисов
    webBuilder.Services.AddScoped<IUserRepository, UserRepository>();
    webBuilder.Services.AddScoped<IProductRepository, ProductRepository>();
    webBuilder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    webBuilder.Services.AddScoped<JwtService>();
    webBuilder.Services.AddScoped<AuthService>();
    webBuilder.Services.AddScoped<ProductService>();

    // Контроллеры с настройками JSON
    webBuilder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    // Swagger с JWT поддержкой
    webBuilder.Services.AddEndpointsApiExplorer();
    webBuilder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WebStore API",
            Version = "v1",
            Contact = new OpenApiContact { Name = "Support", Email = "support@webstore.com" }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // CORS политика
    var origins = webBuilder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
    webBuilder.Services.AddCors(options =>
    {
        options.AddPolicy("WebStoreCors", policy =>
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });
    });

    // Настройки загрузки файлов
    webBuilder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = webBuilder.Configuration.GetValue<long>("FileStorage:MaxFileSizeMB") * 1024 * 1024;
        options.ValueLengthLimit = int.MaxValue;
        options.MemoryBufferThreshold = int.MaxValue;
    });

    // Кэширование ответов
    webBuilder.Services.AddResponseCaching(options =>
    {
        options.MaximumBodySize = 1024 * 1024;
        options.UseCaseSensitivePaths = true;
    });
}

void ConfigureMiddleware(WebApplication webApp)
{
    // Development конфигурация
    if (webApp.Environment.IsDevelopment())
    {
        webApp.UseDeveloperExceptionPage();

        webApp.UseSwagger();
        webApp.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore API v1");
            c.DisplayRequestDuration();
            c.EnablePersistAuthorization();
            c.ConfigObject.AdditionalItems["syntaxHighlight"] = new { activated = false };
        });

        // Автомиграции в Development
        using var scope = webApp.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    else
    {
        webApp.UseExceptionHandler("/error");
        webApp.UseHsts();
    }

    // Базовые middleware
    webApp.UseHttpsRedirection();

    // Статические файлы с кэшированием
    webApp.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
        }
    });

    webApp.UseRouting();

    // Безопасность
    webApp.UseCors("WebStoreCors");
    webApp.UseResponseCaching();
    webApp.UseAuthentication();
    webApp.UseAuthorization();

    // Endpoints
    webApp.MapControllers();

    // Health check
    webApp.MapGet("/health", () => Results.Json(new { status = "Healthy" }));

    // Global error handler
    webApp.Map("/error", () => Results.Problem(
        title: "Server Error",
        detail: "An unexpected error occurred",
        statusCode: StatusCodes.Status500InternalServerError));
}