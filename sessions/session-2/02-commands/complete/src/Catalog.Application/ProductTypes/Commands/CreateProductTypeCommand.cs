using eShop.Catalog.Entities.ProductTypes;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Commands;

public sealed record CreateProductTypeCommand(string Name) : IRequest<ProductType>;

public sealed class CreateProductTypeCommandHandler(IProductTypeRepository repository)
    : IRequestHandler<CreateProductTypeCommand, ProductType>
{
    public async Task<ProductType> Handle(
        CreateProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Name);

        var type = new ProductType { Name = request.Name };

        repository.AddProductType(type);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return type;
    }
}