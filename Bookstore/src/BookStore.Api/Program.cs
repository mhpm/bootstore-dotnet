using BookStore.Api.Validators;
using BookStore.Application.Abstractions;
using BookStore.Application.Rules;
using BookStore.Application.Services;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Repositories;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// ProblemDetails
builder.Services.AddProblemDetails();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Application service
builder.Services.AddScoped<BookService>();

// Business rules
builder.Services.AddScoped<IBookDeletionRule, PremiumBookDeletionRule>();
builder.Services.AddScoped<IBookDeletionRule, LoanedBookDeletionRule>();
builder.Services.AddScoped<IBookDeletionRule, HistoricalBookDeletionRule>();

var app = builder.Build();

// HTTP pipeline
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