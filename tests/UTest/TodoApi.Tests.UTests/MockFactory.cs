using System.Collections.Generic;
using System.Linq;
using Moq;
using TodoApi.Models;

namespace TodoApi.Tests.UTests
{
    public static class MockTodoRepositoryFactory
    {
        public static ITodoRepository CreateTodoReposity()
        {
            var mockService = new Mock<ITodoRepository>();
            var todoItems = new List<TodoItem>()
            {
                new TodoItem
                {
                    Name = "Item1"
                },
                new TodoItem
                {
                    Name = "Item2"
                },
                new TodoItem
                {
                    Name = "Item3"
                },
            };

            mockService
                .Setup(i => i.GetTodoItemsAsync())
                .Returns(todoItems.ToAsyncEnumerable());

            return mockService.Object;
        }
    }
}