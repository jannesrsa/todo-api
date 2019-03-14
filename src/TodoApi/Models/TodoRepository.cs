using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoRepository : ITodoRepository
    {
        private TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task AddTodoItemAsync(TodoItem todoItem)
        {
            await _context.TodoItems.AddAsync(todoItem);
        }

        public void DeleteTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Remove(todoItem);
        }

        public IAsyncEnumerable<TodoItem> GetTodoItemsAsync()
        {
            return _context.TodoItems.ToAsyncEnumerable();
        }

        public async Task<bool> SaveChangesAsync()
        {
            var returnVal = await _context.SaveChangesAsync();
            return returnVal >= 0;
        }

        public bool TodoItemsExist()
        {
            return _context.TodoItems.Any();
        }

        public async Task<TodoItem> TodoItemsFindAsync(params object[] objects)
        {
            return await _context.TodoItems.FindAsync(objects);
        }

        public void UpdateTodoItem(TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
        }
    }
}