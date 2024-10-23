using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Application.ProductTypes.Errors;
using eShop.Catalog.Entities.ProductTypes;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Commands;

public sealed record RenameProductTypeCommand(int Id, string NewName) : IRequest<ProductType>;

public sealed class RenameProductTypeCommandHandler(
    IProductTypeRepository repository,
    IProductTypeByIdDataLoader productTypeById)
    : IRequestHandler<RenameProductTypeCommand, ProductType>
{
    public async Task<ProductType> Handle(
        RenameProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var (id, newName) = request;

        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var type = await productTypeById.LoadAsync(id, cancellationToken);

        if (type is null)
        {
            throw new ProductTypeNotFoundException(id);
        }

        type.Name = newName;
        repository.UpdateProductType(type);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return type;
    }
}