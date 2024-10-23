using MediatR;

namespace eShop.Catalog.Application;

public sealed record HelloWorldCommand(string Name) : IRequest<string>;

public sealed class HelloWorldCommandHandler : IRequestHandler<HelloWorldCommand, string>
{
    public Task<string> Handle(HelloWorldCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Hello, {request.Name}!");
    }
}