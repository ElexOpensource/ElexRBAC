
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace RbacDashboard.Common.Test;

public class MediatorServiceTest
{
    private Mock<IMediator> _mediatorMock;
    private Mock<ILogger<MediatorService>> _loggerMock;
    private MediatorService _mediatorService;

    [SetUp]
    public void Setup()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<MediatorService>>();
        _mediatorService = new MediatorService(_mediatorMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task SendRequest_ShouldReturnResponse_WhenRequestIsHandledSuccessfully()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        var expectedResponse = "Success";

        _mediatorMock
            .Setup(m => m.Send(requestMock.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mediatorService.SendRequest(requestMock.Object);

        // Assert
        Assert.That(expectedResponse, Is.EqualTo(result));
        _mediatorMock.Verify(m => m.Send(requestMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void SendRequest_ShouldThrowInvalidOperationException_WhenHandlerIsNotRegistered()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        var exceptionMessage = "No service for type 'MediatR.IRequestHandler`2[SomeNamespace.SomeCommand,System.String]'";

        _mediatorMock
            .Setup(m => m.Send(requestMock.Object, It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException(exceptionMessage));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _mediatorService.SendRequest(requestMock.Object));

        Assert.That(exceptionMessage, Is.EqualTo(ex.Message));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void SendRequest_ShouldThrowInvalidOperationExceptionWithUnknownCommand_WhenHandlerIsNotRegistered()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        var exceptionMessage = "No service for type 'MediatR.IRequestHandler`";
            
        _mediatorMock
            .Setup(m => m.Send(requestMock.Object, It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException(exceptionMessage));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _mediatorService.SendRequest(requestMock.Object));

        Assert.That(exceptionMessage, Is.EqualTo(ex.Message));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void SendRequest_ShouldThrowApplicationException_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        var unexpectedException = new Exception("An unexpected error occurred.");

        _mediatorMock
            .Setup(m => m.Send(requestMock.Object, It.IsAny<CancellationToken>()))
            .Throws(unexpectedException);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(async () =>
            await _mediatorService.SendRequest(requestMock.Object));

        Assert.That(unexpectedException.Message, Is.EqualTo(ex.Message));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                unexpectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
