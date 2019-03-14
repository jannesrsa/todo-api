using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;

            if (_repository.TodoItemsExist())
            {
                // Create a new TodoItem if collection is empty,
                // which means you can't delete all TodoItems.
                _repository.AddTodoItemAsync(new TodoItem
                {
                    Name = "Item1"
                });
                _repository.SaveChangesAsync();
            }
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _repository.TodoItemsFindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _repository.DeleteTodoItem(todoItem);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _repository.TodoItemsFindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // GET: api/Todo
        [HttpGet]
        public ActionResult<IAsyncEnumerable<TodoItem>> GetTodoItems()
        {
            return Ok(_repository.GetTodoItemsAsync());
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            await _repository.AddTodoItemAsync(item);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _repository.UpdateTodoItem(item);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}