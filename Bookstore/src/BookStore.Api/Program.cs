using BookStore.Api.Models;
using BookStore.Api.Repositories;
using BookStore.Api.Rules;
using BookStore.Api.Services;
using BookStore.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookStoreDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("BookStore"));
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Service
builder.Services.AddScoped<BookService>();

// Rules
builder.Services.AddScoped<IBookDeletionRule, PremiumBookDeletionRule>();
builder.Services.AddScoped<IBookDeletionRule, LoanedBookDeletionRule>();
builder.Services.AddScoped<IBookDeletionRule, HistoricalBookDeletionRule>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();