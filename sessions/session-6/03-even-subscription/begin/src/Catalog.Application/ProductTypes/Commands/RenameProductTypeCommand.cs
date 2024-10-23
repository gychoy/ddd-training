using eShop.Catalog.Application.ProductTypes.Models;
using eShop.Catalog.Entities.ProductTypes;
using eShop.Catalog.Entities.ProductTypes.Errors;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Commands;

public sealed record RenameProductTypeCommand(int Id, string NewName) : IRequest<ProductTypeDto>;

public sealed class RenameProductTypeCommandHandler(IProductTypeRepository repository)
    : IRequestHandler<RenameProductTypeCommand, ProductTypeDto>
{
    public async Task<ProductTypeDto> Handle(
        RenameProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var (id, newName) = request;

        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var type = await repository.GetProductTypeAsync(id, cancellationToken);

        if (type is null)
        {
            throw new ProductTypeNotFoundException(id);
        }

        type.Rename(newName);

        repository.UpdateProductType(type);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return type.ToReadModel();
    }
}