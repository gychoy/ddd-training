using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Brands.Errors;
using eShop.Catalog.Entities.Brands;
using MediatR;

namespace eShop.Catalog.Application.Brands.Services;

public sealed record RenameBrandCommand(int Id, string NewName) : IRequest<Brand>;

public sealed class RenameBrandCommandHandler(
    IBrandRepository repository,
    IBrandByIdDataLoader brandById)
    : IRequestHandler<RenameBrandCommand, Brand>
{
    public async Task<Brand> Handle(
        RenameBrandCommand request,
        CancellationToken cancellationToken)
    {
        var (id, newName) = request;

        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var brand = await brandById.LoadAsync(id, cancellationToken);

        if (brand == null)
        {
            throw new BrandNotFoundException(id);
        }

        brand.Name = newName;
        repository.UpdateBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return brand;
    }
}