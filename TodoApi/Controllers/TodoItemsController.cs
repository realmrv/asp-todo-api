using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController(TodoContext context) : ControllerBase
{
    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
    {
        return await context.TodoItems.Select(x => ItemToDto(x)).ToListAsync();
    }

    // GET: api/TodoItems/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        return ItemToDto(todoItem);
    }

    // PUT: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItemDto todoDto)
    {
        if (id != todoDto.Id) return BadRequest();

        var todoItem = await context.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();

        todoItem.Name = todoDto.Name;
        todoItem.IsComplete = todoDto.IsComplete;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // PATCH: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> PatchTodoItem(long id, [FromBody] JsonPatchDocument<TodoItemDto> patchDoc)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        var dto = ItemToDto(todoItem);
        patchDoc.ApplyTo(dto, ModelState);

        todoItem.Name = dto.Name;
        todoItem.IsComplete = dto.IsComplete;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoItemExists(id))
                return NotFound();
            throw;
        }

        return !ModelState.IsValid ? BadRequest(ModelState) : new ObjectResult(ItemToDto(todoItem));
    }

    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItemDto todoDto)
    {
        var todoItem = DtoToItem(todoDto);

        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDto(todoItem));
    }

    // DELETE: api/TodoItems/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();

        context.TodoItems.Remove(todoItem);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id)
    {
        return context.TodoItems.Any(e => e.Id == id);
    }

    private static TodoItemDto ItemToDto(TodoItem todoItem)
    {
        return new TodoItemDto
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
    }

    private static TodoItem DtoToItem(TodoItemDto todoDto)
    {
        return new TodoItem
        {
            IsComplete = todoDto.IsComplete,
            Name = todoDto.Name
        };
    }
}