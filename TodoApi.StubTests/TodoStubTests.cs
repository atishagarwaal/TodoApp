using Moq; // Moq library for mocking
using Microsoft.EntityFrameworkCore; // EF Core namespace for DbContext and DbSet
using TodoApi.Controllers; // Namespace for the API controller
using TodoApi.Data; // Namespace for the data models
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TodoApi.StubTests
{
    [TestClass] // Indicates a test class for MSTest
    public class TodoControllerStubTests
    {
        [TestMethod] // Indicates a test method
        public async Task GetTodoItems_ReturnsAllItems()
        {
            // Arrange: Setup the mock data and context
            var data = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TodoItem>(data.Provider));
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TodoItem>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<TodoItem>(data.GetEnumerator()));

            var mockContext = new Mock<DBContext>(new DbContextOptions<DBContext>());
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);

            var controller = new TodoController(mockContext.Object);

            // Act: Call the method to be tested
            var result = await controller.GetTodoItems();

            // Assert: Verify the result
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value?.Count());
        }

        [TestMethod] // Indicates a test method
        public async Task GetTodoItem_ReturnsCorrectItem()
        {
            // Arrange: Setup the mock data and context
            var data = new List<TodoItem>().AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TodoItem>(data.Provider));
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TodoItem>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<TodoItem>(data.GetEnumerator()));

            var todoItem = new TodoItem { Id = 1, Title = "Test Todo 1", IsCompleted = false };
            mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(todoItem);

            var mockContext = new Mock<DBContext>(new DbContextOptions<DBContext>());
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);

            var controller = new TodoController(mockContext.Object);

            // Act: Call the method to be tested
            var result = await controller.GetTodoItem(1);

            // Assert: Verify the result
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value?.Id);
            Assert.AreEqual("Test Todo 1", result.Value?.Title);
        }

        [TestMethod] // Indicates a test method
        public async Task PostTodoItem_AddsNewItem()
        {
            // Arrange: Setup the mock data and context
            var data = new List<TodoItem>().AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TodoItem>(data.Provider));
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TodoItem>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<TodoItem>(data.GetEnumerator()));

            // Arrange: Setup the mock data and context
            var newItem = new TodoItem { Id = 3, Title = "New Todo", IsCompleted = false };

            var mockContext = new Mock<DBContext>(new DbContextOptions<DBContext>());
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);

            var controller = new TodoController(mockContext.Object);

            // Act: Call the method to be tested
            var result = await controller.PostTodoItem(newItem);

            // Assert: Verify the result
            Assert.IsNotNull((TodoItem)((CreatedAtActionResult)result.Result).Value);
            Assert.AreEqual("New Todo", ((TodoItem)((CreatedAtActionResult)result.Result)?.Value)?.Title);

            mockSet.Verify(m => m.Add(It.IsAny<TodoItem>()), Times.Once());
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [TestMethod] // Indicates a test method
        public async Task PutTodoItem_UpdatesItem()
        {
            // Arrange: Setup the mock data and context
            var data = new List<TodoItem>().AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TodoItem>(data.Provider));
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TodoItem>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<TodoItem>(data.GetEnumerator()));

            var mockContext = new Mock<DBContext>(new DbContextOptions<DBContext>());
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);

            var controller = new TodoController(mockContext.Object);

            // Arrange: Setup the mock data and context
            var existingItem = new TodoItem { Id = 1, Title = "Test Todo 1", IsCompleted = false };
            var itemToUpdate = new TodoItem { Id = 1, Title = "Updated Todo 1", IsCompleted = true };

            mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(existingItem);

            // Act: Call the method to be tested
            await controller.PutTodoItem(1, itemToUpdate);

            // Fetch the updated item from the in-memory database
            var updatedItem = await controller.GetTodoItem(1);

            // Assert that the updated item's title is "Updated Todo"
            Assert.AreEqual("Updated Todo 1", updatedItem.Value?.Title);

            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [TestMethod] // Indicates a test method
        public async Task DeleteTodoItem_DeletesItem()
        {
            // Arrange: Setup the mock data and context
            var data = new List<TodoItem>().AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TodoItem>(data.Provider));
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TodoItem>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<TodoItem>(data.GetEnumerator()));

            // Arrange: Setup the mock data and context
            var existingItem = new TodoItem { Id = 1, Title = "Existing Todo", IsCompleted = false };

            mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(existingItem);

            var mockContext = new Mock<DBContext>(new DbContextOptions<DBContext>());
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);

            var controller = new TodoController(mockContext.Object);

            // Act: Call the method to be tested
            var result = await controller.DeleteTodoItem(1);

            // Cast the result to NoContentResult
            var noContentResult = result as NoContentResult;

            // Assert that the result is not null
            Assert.IsNotNull(noContentResult);

            // Fetch the deleted item from the in-memory database
            var deletedItem = await controller.GetTodoItem(5);

            // Assert that the deleted item is null
            Assert.IsNull(deletedItem.Value);

            mockSet.Verify(m => m.Remove(It.IsAny<TodoItem>()), Times.Once());
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}
