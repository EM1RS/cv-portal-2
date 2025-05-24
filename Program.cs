using System.Security.Claims;
using System.Text.Json.Serialization;
using Amazon.S3;
using CvAPI2.Models;
using CvAPI2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using CvApi2.Service;
using DotNetEnv;



var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();


builder.WebHost.UseUrls("http://0.0.0.0:80");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
            new string[] {}
        }
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
    });



builder.Services.Configure<OpenAiSettings>(options =>
{
    builder.Configuration.GetSection("OpenAi").Bind(options);

    var fromEnv = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
    Console.WriteLine("🧪 OpenAI-nøkkel (kort): " + Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")?.Substring(0, 6));
    Console.WriteLine("✅ Konfigurerer OpenAiSettings ...");

    if (!string.IsNullOrWhiteSpace(fromEnv))
    {
        options.ApiKey = fromEnv;
    }

});


builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<CvDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrEmpty(jwtKey))
{   
    Console.WriteLine("JWT Key mangler i config!");
    throw new Exception("JWT Key is not configured.");
}


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role

    };
        options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            //Console.WriteLine("❌ Tokenvalidering feilet: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
           // Console.WriteLine("✅ Token validert!");
            //Console.WriteLine("🔑 Token brukt: " + context.SecurityToken);
            return Task.CompletedTask;
        }
    };

});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<AwsSettings>(options =>
{
    options.AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? "";
    options.SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? "";
    options.Region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "";
    options.BucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME") ?? "";
});



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (!string.IsNullOrWhiteSpace(envConn))
{
    connectionString = envConn;
}

builder.Services.AddDbContext<CvDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 42)),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
});





builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICvService, CvService>();
builder.Services.AddScoped<ICvRepository, CvRepository>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddSingleton<S3Service>();
builder.Services.AddAWSService<IAmazonS3>();




var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
//}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();
app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    Console.WriteLine($"➡️ {context.Request.Method} {context.Request.Path}");
    await next();
});

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    //Console.WriteLine($"🔍 Authorization Header: {authHeader}");
    await next();
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CvDbContext>();
        await context.Database.MigrateAsync(); 
        await SeedData.InitializeAsync(services); 
    }
    catch (Exception ex)
    {
        Console.WriteLine("Feil ved migrering/seed:");
        Console.WriteLine(ex);
    }
}


QuestPDF.Settings.License = LicenseType.Community;

app.Run();





