using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(BookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await bookService.GetAllAsync();

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(
                title: result.Error!.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await bookService.GetByIdAsync(id);

        if (result.IsSuccess)
            return Ok(result.Value);

        return MapError(result.Error!);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request)
    {
        var result = await bookService.CreateAsync(request);

        if (!result.IsSuccess)
            return MapError(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBookRequest request)
    {
        var result = await bookService.UpdateAsync(id, request);

        if (!result.IsSuccess)
            return MapError(result.Error!);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await bookService.DeleteAsync(id);

        if (!result.IsSuccess)
            return MapError(result.Error!);

        return NoContent();
    }

    private IActionResult MapError(Error error)
    {
        return error.Code switch
        {
            "Book.NotFound" => NotFound(new
            {
                error.Code,
                error.Message
            }),

            _ => Problem(
                title: error.Code,
                detail: error.Message,
                statusCode: StatusCodes.Status400BadRequest)
        };
    }
}