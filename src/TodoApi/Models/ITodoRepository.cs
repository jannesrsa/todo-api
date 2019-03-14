using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public interface ITodoRepository
    {
        Task AddTodoItemAsync(TodoItem todoItem);

        void DeleteTodoItem(TodoItem todoItem);

        IAsyncEnumerable<TodoItem> GetTodoItemsAsync();

        Task<bool> SaveChangesAsync();

        Task<TodoItem> TodoItemsFindAsync(params object[] objects);

        void UpdateTodoItem(TodoItem todoItem);

        bool TodoItemsExist();
    }
}