using Microsoft.EntityFrameworkCore;
using DOCOSoft.Controllers;
using DOCOSoft.Data;
using DOCOSoft.Models;

namespace DOCOSoft.Test
{
    public class UsersApiControllerTests
    {
       private DbContextOptions<AppDbContext> GetInMemoryOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        [Fact]
        public async Task GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var options = GetInMemoryOptions("GetUsers_ShouldReturnAllUsers");
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated(); // Garante que o banco de dados será criado
                context.Users.Add(new User { Name = "John Doe", Email = "john@docosoft.com" });
                context.Users.Add(new User { Name = "Jane Doe", Email = "jane@docosoft.com" });
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new UsersApiController(context);
                var result = await controller.GetUsers();
                Assert.Equal(2, result.Value.Count());
                Assert.Contains(result.Value, u => u.Name == "John Doe");
                Assert.Contains(result.Value, u => u.Name == "Jane Doe");
                context.Database.EnsureDeleted();
            }
            
        }

        [Fact]
        public async Task CreateUser_ShouldAddUser()
        {
            var options = GetInMemoryOptions("CreateUser_ShouldAddUser");
            using var context = new AppDbContext(options);
            var controller = new UsersApiController(context);
            var newUser = new User { Name = "New User", Email = "newuser@docosoft.com" };
            var result = await controller.CreateUser(newUser);
            ;
            Assert.Equal(1, context.Users.Count());
            var name = context.Users.First().Name;
            Assert.Equal("New User", name);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserById()
        {
            var options = GetInMemoryOptions("GetUser_ShouldReturnUserById");
            using var context = new AppDbContext(options);
            var user = new User { Name = "Test User", Email = "testuser@docosoft.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UsersApiController(context);
            var result = await controller.GetUserById(user.Id);
            Assert.NotNull(result.Value);
            Assert.Equal(user.Name, result.Value.Name);
            context.Database.EnsureDeleted();
        }
    }
}
