using MediatR;

namespace eShop.Catalog.Application.Common.Contracts;

public interface IQuery;

public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>;