using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Brands.Errors;
using MediatR;

namespace eShop.Catalog.Application.Brands.Services;

public sealed record RenameBrandCommand(int Id, string NewName) : IRequest<BrandDto>;

public sealed class RenameBrandCommandHandler(IBrandRepository repository)
    : IRequestHandler<RenameBrandCommand, BrandDto>
{
    public async Task<BrandDto> Handle(
        RenameBrandCommand request,
        CancellationToken cancellationToken)
    {
        var (id, newName) = request;

        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var brand = await repository.GetBrandAsync(id, cancellationToken);

        if (brand == null)
        {
            throw new BrandNotFoundException(id);
        }

        brand.Rename(newName);
        repository.UpdateBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return brand.ToReadModel();
    }
}