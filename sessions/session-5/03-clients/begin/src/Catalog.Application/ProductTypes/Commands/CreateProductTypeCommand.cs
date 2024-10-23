using eShop.Catalog.Application.ProductTypes.Models;
using eShop.Catalog.Entities.ProductTypes;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Commands;

public sealed record CreateProductTypeCommand(string Name) : IRequest<ProductTypeDto>;

public sealed class CreateProductTypeCommandHandler(IProductTypeRepository repository)
    : IRequestHandler<CreateProductTypeCommand, ProductTypeDto>
{
    public async Task<ProductTypeDto> Handle(
        CreateProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Name);

        var type = ProductType.Create(request.Name);

        repository.AddProductType(type);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return type.ToReadModel();
    }
}