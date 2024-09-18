
using Moq;
using RbacDashboard.Common.Interface;
using RbacDashboard.Common;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Commands;
using RbacDashboard.BAL;

namespace RbacDashboard.Test.BAL;

public class MasterRepositoryTest
{
    private Mock<IMediatorService> _mediatorMock;
    private Mock<IRbacTokenRepository> _tokenRepositoryMock;
    private MasterRepository _masterRepository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _tokenRepositoryMock = new Mock<IRbacTokenRepository>();
        _masterRepository = new MasterRepository(_mediatorMock.Object, _tokenRepositoryMock.Object);
    }

    [Test]
    public void GenetrateTokenByCustomer_ShouldThrowKeyNotFoundException_WhenCustomerNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
                     .ReturnsAsync((Customer)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _masterRepository.GenetrateTokenByCustomer(customerId));
        Assert.That(ex.Message, Is.EqualTo($"Customer with ID {customerId} was not found."));
    }

    [Test]
    public async Task GenetrateTokenByCustomer_ShouldReturnToken_WhenCustomerFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
                     .ReturnsAsync(customer);
        _tokenRepositoryMock.Setup(t => t.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns("jwt_token");

        // Act
        var result = await _masterRepository.GenetrateTokenByCustomer(customerId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetCustomerById>(req => req.Id == customerId)), Times.Once);
        _tokenRepositoryMock.Verify(t => t.GenerateJwtToken(customerId.ToString(), RbacConstants.CustomerId, 120), Times.Once);
        Assert.That(result, Is.EqualTo("jwt_token"));
    }

    [Test]
    public async Task GetOptions_ShouldReturnRoles_WhenOptionNameIsRole()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Role1" },
            new Role { Id = Guid.NewGuid(), Name = "Role2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
                     .ReturnsAsync(roles);

        // Act
        var result = await _masterRepository.GetOptions(applicationId, "ROLE");

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetRolesByApplicationId>(req => req.ApplicationId == applicationId)), Times.Once);
        Assert.That(result, Has.Count.EqualTo(roles.Count));
        Assert.That(result, Is.All.InstanceOf<Option>());
        Assert.That(result.Select(o => o.Label), Is.EquivalentTo(roles.Select(r => r.Name)));
    }

    [Test]
    public async Task GetOptions_ShouldReturnAccesses_WhenOptionNameIsAccess()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), Name = "Access1" },
            new Access { Id = Guid.NewGuid(), Name = "Access2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetAccessesByApplicationId>()))
                     .ReturnsAsync(accesses);

        // Act
        var result = await _masterRepository.GetOptions(applicationId, "ACCESS");

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetAccessesByApplicationId>(req => req.ApplicationId == applicationId)), Times.Once);
        Assert.That(result, Has.Count.EqualTo(accesses.Count));
        Assert.That(result, Is.All.InstanceOf<Option>());
        Assert.That(result.Select(o => o.Label), Is.EquivalentTo(accesses.Select(a => a.Name)));
    }

    [Test]
    public void GetOptions_ShouldThrowException_WhenOptionNameIsInvalid()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var invalidOptionName = "INVALID";

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => _masterRepository.GetOptions(applicationId, invalidOptionName));
        Assert.That(ex.Message, Is.EqualTo($"Invalid Option Name - {invalidOptionName} : it must be `ROLE` or `ACCESS`"));
    }

    [Test]
    public async Task GetOptionsetMasters_ShouldReturnOptionsets_WhenCalled()
    {
        // Arrange
        var optionsets = new List<OptionsetMaster>
        {
            new OptionsetMaster { Id = Guid.NewGuid(), Name = "Optionset1" },
            new OptionsetMaster { Id = Guid.NewGuid(), Name = "Optionset2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetOptionsetMasters>()))
                     .ReturnsAsync(optionsets);

        // Act
        var result = await _masterRepository.GetOptionsetMasters();

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetOptionsetMasters>()), Times.Once);
        Assert.That(result, Is.EqualTo(optionsets));
    }

    [Test]
    public async Task GetPermissionSetList_ShouldReturnPermissions_WhenCalled()
    {
        // Arrange
        var permissions = new List<Permissionset>
        {
            new Permissionset { Id = Guid.NewGuid(), Name = "Permission1" },
            new Permissionset { Id = Guid.NewGuid(), Name = "Permission2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetPermissionsets>()))
                     .ReturnsAsync(permissions);

        // Act
        var result = await _masterRepository.GetPermissionSetList();

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetPermissionsets>()), Times.Once);
        Assert.That(result, Is.EqualTo(permissions));
    }

    [Test]
    public async Task GetTypeMasters_ShouldReturnTypeMasters_WhenCalled()
    {
        // Arrange
        var types = new List<TypeMaster>
        {
            new TypeMaster { Id = Guid.NewGuid(), Name = "Type1" },
            new TypeMaster { Id = Guid.NewGuid(), Name = "Type2" }
        };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetAllTypeMaster>()))
                     .ReturnsAsync(types);

        // Act
        var result = await _masterRepository.GetTypeMasters();

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetAllTypeMaster>()), Times.Once);
        Assert.That(result, Is.EqualTo(types));
    }
}
