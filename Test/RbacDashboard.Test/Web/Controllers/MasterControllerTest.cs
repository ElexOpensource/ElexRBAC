

using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.Web;

public class MasterControllerTest
{
    private Mock<IRbacMasterRepository> _masterRepositoryMock;
    private MasterController _masterController;

    [SetUp]
    public void SetUp()
    {
        _masterRepositoryMock = new Mock<IRbacMasterRepository>();
        _masterController = new MasterController(_masterRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _masterController.Dispose();
    }

    [Test]
    public async Task GenerateCustomerToken_ShouldReturnToken_WhenValidCustomerIdProvided()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var expectedToken = "generated-token";

        _masterRepositoryMock
            .Setup(repo => repo.GenetrateTokenByCustomer(customerId))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _masterController.GenerateCustomerToken(customerId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedToken));
    }

    [Test]
    public async Task GetTypeMasters_ShouldReturnTypeMasters()
    {
        // Arrange
        var expectedTypeMasters = new List<TypeMaster>
        {
            new TypeMaster { Id = Guid.NewGuid(), Name = "Type1" },
            new TypeMaster { Id = Guid.NewGuid(), Name = "Type2" }
        };

        _masterRepositoryMock
            .Setup(repo => repo.GetTypeMasters())
            .ReturnsAsync(expectedTypeMasters);

        // Act
        var result = await _masterController.GetTypeMasters();

        // Assert
        Assert.That(result, Is.EqualTo(expectedTypeMasters));
    }

    [Test]
    public async Task GetOptionsetMasters_ShouldReturnOptionsetMasters()
    {
        // Arrange
        var expectedOptionsetMasters = new List<OptionsetMaster>
        {
            new OptionsetMaster { Id = Guid.NewGuid(), Name = "OptionSet1" },
            new OptionsetMaster { Id = Guid.NewGuid(), Name = "OptionSet2" }
        };

        _masterRepositoryMock
            .Setup(repo => repo.GetOptionsetMasters())
            .ReturnsAsync(expectedOptionsetMasters);

        // Act
        var result = await _masterController.GetOptionsetMasters();

        // Assert
        Assert.That(result, Is.EqualTo(expectedOptionsetMasters));
    }

    [Test]
    public async Task GetPermissionSets_ShouldReturnPermissionSets()
    {
        // Arrange
        var expectedPermissionSets = new List<Permissionset>
        {
            new Permissionset { Id = Guid.NewGuid(), Name = "PermissionSet1" },
            new Permissionset { Id = Guid.NewGuid(), Name = "PermissionSet2" }
        };

        _masterRepositoryMock
            .Setup(repo => repo.GetPermissionSetList())
            .ReturnsAsync(expectedPermissionSets);

        // Act
        var result = await _masterController.GetPermissionSets();

        // Assert
        Assert.That(result, Is.EqualTo(expectedPermissionSets));
    }

    [Test]
    public async Task GetOptions_ShouldReturnOptions_WhenValidApplicationIdAndOptionNameProvided()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var optionName = "SomeOption";
        var expectedOptions = new List<Option>
        {
            new Option { Id = Guid.NewGuid(), Label = "Option1" },
            new Option { Id = Guid.NewGuid(), Label = "Option2" }
        };

        _masterRepositoryMock
            .Setup(repo => repo.GetOptions(applicationId, optionName))
            .ReturnsAsync(expectedOptions);

        // Act
        var result = await _masterController.GetOptions(applicationId, optionName);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOptions));
    }
}