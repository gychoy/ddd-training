using eShop.Catalog.Entities.Brands;
using MediatR;

namespace eShop.Catalog.Application.Brands.Commands;

public sealed record CreateBrandCommand(string Name) : IRequest<Brand>;

public sealed class CreateBrandCommandHandler(IBrandRepository repository)
    : IRequestHandler<CreateBrandCommand, Brand>
{
    public async Task<Brand> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Name);

        var brand = new Brand { Name = request.Name };

        repository.AddBrand(brand);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return brand;
    }
}