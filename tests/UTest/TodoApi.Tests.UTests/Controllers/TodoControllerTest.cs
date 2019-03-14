using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests.UTests.Controllers
{
    public class TodoControllerTest
    {
        private TodoController _controller;
        private ITodoRepository _service;

        public TodoControllerTest()
        {
            _service = MockTodoRepositoryFactory.CreateTodoReposity();
            _controller = new TodoController(_service);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Act
            var okResult = _controller.GetTodoItems().Result as OkObjectResult;

            // Assert
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.NotEmpty(items);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.GetTodoItems();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
    }
}