using System.Text;
using System.Text.Json.Serialization;
using BookStore.Api.Extensions;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Application.ValidationsAndAttributes;
using BookStore.Infrastructure.Constants;
using BookStore.Infrastructure.Persistance;
using BookStore.Infrastructure.Persistance.Interceptors;
using BookStore.Infrastructure.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddScoped<AuditLogInterceptor>();
builder.Services.AddScoped<IAuditLogSink, EfCoreAuditLogSink>();
builder.Services.AddDbContext<BookStoreDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).AddInterceptors(sp.GetRequiredService<AuditLogInterceptor>());
});
builder.Services.AddScoped<IBookStoreDbContext, BookStoreDbContext>();

builder.Services.AddHttpContextAccessor(); // Required for the Accessor to work

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BookRequestValidator>();

// builder.Services.AddSingleton<IBookService, InMemoryBookService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = AuthConstants.JwtScheme;
        options.DefaultChallengeScheme = AuthConstants.JwtScheme;
        options.DefaultScheme = AuthConstants.JwtScheme;
    })
    .AddJwtBearer(AuthConstants.JwtScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    })
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        AuthConstants.BasicScheme, 
        null
    );



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.DisplayRequestDuration());
}

app.UseHttpsRedirection();

// ORDER:
// 1. Authentication: "Who are you?" (Checks the token headers, parses claims)
app.UseAuthentication(); 

// 2. Authorization: "Are you allowed?" (Checks [Authorize] attributes against claims)
app.UseAuthorization();

app.MapControllers();

app.Run();