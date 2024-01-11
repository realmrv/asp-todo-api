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
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await context.TodoItems.ToListAsync();
    }

    // GET: api/TodoItems/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        return todoItem;
    }

    // PUT: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
    {
        if (id != todoItem.Id) return BadRequest();

        context.Entry(todoItem).State = EntityState.Modified;

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

        return NoContent();
    }

    // PATCH: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> PatchTodoItem(long id, [FromBody] JsonPatchDocument<TodoItem> patchDoc)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        patchDoc.ApplyTo(todoItem, ModelState);
        context.Update(todoItem);

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

        return !ModelState.IsValid ? BadRequest(ModelState) : new ObjectResult(todoItem);
    }

    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
    {
        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
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
}