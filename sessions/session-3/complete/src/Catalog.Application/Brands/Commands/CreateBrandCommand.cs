using eShop.Catalog.Application.Brands.Models;
using eShop.Catalog.Entities.Brands;
using MediatR;

namespace eShop.Catalog.Application.Brands.Commands;

public sealed record CreateBrandCommand(string Name) 
    : IRequest<BrandDto>;

public sealed class CreateBrandCommandHandler(IBrandRepository repository)
    : IRequestHandler<CreateBrandCommand, BrandDto>
{
    public async Task<BrandDto> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Name);

        var brand = Brand.Create(request.Name);

        repository.AddBrand(brand);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return brand.ToReadModel();
    }
}