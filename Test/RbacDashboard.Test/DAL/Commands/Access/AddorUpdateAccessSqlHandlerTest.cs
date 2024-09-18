using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class AddorUpdateAccessSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_AddNewAccess_AddsAccess()
    {
        // Arrange
        using var context = CreateContext();
        var newAccess = new Access
        {
            Id = Guid.Empty,
            Name = "New Access",
            IsActive = true,
            IsDeleted = false
        };

        var handler = new AddorUpdateAccessSqlHandler(context);
        var request = new AddorUpdateAccess(newAccess);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Name, Is.EqualTo("New Access"));
    }

    [Test]
    public async Task Handle_UpdateExistingAccess_UpdatesAccess()
    {
        // Arrange
        using var context = CreateContext();
        var existingAccessId = Guid.NewGuid();
        var existingAccess = new Access
        {
            Id = existingAccessId,
            Name = "Existing Access",
            IsActive = true,
            IsDeleted = false
        };
        context.Accesses.Add(existingAccess);
        await context.SaveChangesAsync();

        existingAccess.Name = "Updated Access";

        var handler = new AddorUpdateAccessSqlHandler(context);
        var request = new AddorUpdateAccess(existingAccess);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingAccessId));
        Assert.That(result.Name, Is.EqualTo("Updated Access"));
    }

}
