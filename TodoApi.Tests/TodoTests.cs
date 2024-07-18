using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

[TestClass] // Indicates that this class contains unit tests
public class TodoControllerTests
{
    private DBContext _context; // The in-memory database context
    private TodoController _controller; // The controller being tested

    [TestInitialize] // Method to set up the test environment
    public void Setup()
    {
        // Configure the in-memory database options
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseSqlite("Data Source=TodoListTest.db")
            .Options;

        // Initialize the database context with the in-memory database
        _context = new DBContext(options);
        _context.Database.EnsureCreated();
        // Initialize the controller with the in-memory database context
        _controller = new TodoController(_context);

        // Add test data to the in-memory database
        _context.TodoItems.AddRange(new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Test Todo 1", Description = "Test Todo 1", IsCompleted = false },
            new TodoItem { Id = 2, Title = "Test Todo 2", Description = "Test Todo 2", IsCompleted = true }
        });

        // Save changes to the in-memory database
        _context.SaveChanges();

        // Clear the change tracker to avoid tracking multiple instances of the same entity
        _context.ChangeTracker.Clear();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod] // Indicates that this method is a unit test
    public async Task GetTodoItems_ReturnsAllItems()
    {
        // Call the GetTodoItems method of the controller
        var result = await _controller.GetTodoItems();

        // Assert that the count of items is 2
        Assert.AreEqual(2, result.Value?.Count());
    }

    [TestMethod] // Indicates that this method is a unit test
    public async Task GetTodoItem_ReturnsCorrectItem()
    {
        // Call the GetTodoItem method of the controller with an ID of 1
        var result = await _controller.GetTodoItem(1);

        // Assert that the item's ID is 1
        Assert.AreEqual(1, result.Value?.Id);

        // Assert that the item's title is "Test Todo 1"
        Assert.AreEqual("Test Todo 1", result.Value?.Title);
    }

    [TestMethod] // Indicates that this method is a unit test
    public async Task PostTodoItem_AddsNewItem()
    {
        // Create a new TodoItem
        var newItem = new TodoItem { Id = 3, Title = "Test Todo 3", Description = "Test Todo 3", IsCompleted = false };

        // Call the PostTodoItem method of the controller
        var result = await _controller.PostTodoItem(newItem);

        // Assert that the item is not null
        Assert.IsNotNull((TodoItem)((CreatedAtActionResult)result.Result).Value);

        // Assert that the item's title is "New Todo"
        Assert.AreEqual("Test Todo 3", ((TodoItem)((CreatedAtActionResult)result.Result)?.Value)?.Title);
    }

    [TestMethod] // Indicates that this method is a unit test
    public async Task PutTodoItem_UpdatesItem()
    {
        // Create a TodoItem to update
        var itemToUpdate = new TodoItem { Id = 1, Title = "Updated Todo 1", Description = "Test Todo 1", IsCompleted = false };

        // Call the PutTodoItem method of the controller with the updated item
        await _controller.PutTodoItem(1, itemToUpdate);

        // Fetch the updated item from the in-memory database
        var updatedItem = await _context.TodoItems.FindAsync(1);

        // Assert that the updated item's title is "Updated Todo"
        Assert.AreEqual("Updated Todo 1", updatedItem?.Title);
    }

    [TestMethod] // Indicates that this method is a unit test
    public async Task DeleteTodoItem_DeletesItem()
    {
        // Add test data to the in-memory database
        _context.TodoItems.AddRange(new List<TodoItem>
        {
            new TodoItem { Id = 5, Title = "Test Todo 5", Description = "Test Todo 5", IsCompleted = false }
        });

        // Save changes to the in-memory database
        _context.SaveChanges();

        // Call the DeleteTodoItem method of the controller with an ID of 5
        var result = await _controller.DeleteTodoItem(5);

        // Cast the result to NoContentResult
        var noContentResult = result as NoContentResult;

        // Assert that the result is not null
        Assert.IsNotNull(noContentResult);

        // Fetch the deleted item from the in-memory database
        var deletedItem = await _context.TodoItems.FindAsync(5);

        // Assert that the deleted item is null
        Assert.IsNull(deletedItem);
    }
}
