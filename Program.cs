using System.Security.Claims;
using System.Text.Json.Serialization;
using CvAPI2.Models;
using CvAPI2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5005");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddDbContext<CvDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
        


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICvService, CvService>();
builder.Services.AddScoped<ICvRepository, CvRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
//}



// app.Use(async (context, next) =>
// {
//     try
//     {
//         await next.Invoke();
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine("🔥 Unhandled Exception:");
//         Console.WriteLine(ex.Message);
//         Console.WriteLine(ex.StackTrace);

//         context.Response.StatusCode = 500;
//         context.Response.ContentType = "application/json";

//         await context.Response.WriteAsJsonAsync(new
//         {
//             error = ex.Message,
//             //fullDetails = ex.ToString()
//             //stackTrace = ex.StackTrace
//         });
//     }
// });

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("AllowAll");
//app.UseHttpsRedirection();

app.UseRouting();

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
    await SeedData.InitializeAsync(services);
}


app.Run();





