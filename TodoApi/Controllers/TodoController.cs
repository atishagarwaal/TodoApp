using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")] // Route attribute specifies the base route for all actions in this controller
    [ApiController] // Indicates that this controller responds to web API requests
    public class TodoController : ControllerBase
    {
        private readonly DBContext _context;

        // Constructor to initialize the database context
        public TodoController(DBContext context)
        {
            _context = context;
        }

        // GET: api/todo
        // Retrieves a list of all todo items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            // Asynchronously fetches all todo items from the database
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/todo/{id}
        // Retrieves a specific todo item by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            // Asynchronously fetches the todo item with the specified ID
            var todoItem = await _context.TodoItems.FindAsync(id);

            // If the item is not found, return a 404 Not Found response
            if (todoItem == null)
            {
                return NotFound();
            }

            // Return the found item
            return todoItem;
        }

        // POST: api/todo
        // Adds a new todo item
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            // Adds the new todo item to the database context
            _context.TodoItems.Add(todoItem);
            // Asynchronously saves the changes to the database
            await _context.SaveChangesAsync();

            // Returns a 201 Created response with the route to the new item
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // PUT: api/todo/{id}
        // Updates an existing todo item
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            // If the provided ID does not match the item's ID, return a 400 Bad Request response
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                // Asynchronously saves the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the item does not exist, return a 404 Not Found response
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Returns a 204 No Content response indicating successful update
            return NoContent();
        }

        // DELETE: api/todo/{id}
        // Deletes a specific todo item by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            // Asynchronously fetches the todo item with the specified ID
            var todoItem = await _context.TodoItems.FindAsync(id);
            // If the item is not found, return a 404 Not Found response
            if (todoItem == null)
            {
                return NotFound();
            }

            // Removes the item from the database context
            _context.TodoItems.Remove(todoItem);
            // Asynchronously saves the changes to the database
            await _context.SaveChangesAsync();

            // Returns a 204 No Content response indicating successful deletion
            return NoContent();
        }

        // Checks if a todo item with the specified ID exists in the database
        private bool TodoItemExists(int id)
        {
            // Returns true if any item with the specified ID exists, otherwise false
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
