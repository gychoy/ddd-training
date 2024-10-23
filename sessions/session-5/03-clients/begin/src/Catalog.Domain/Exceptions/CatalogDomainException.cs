namespace eShop.Catalog.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public abstract class CatalogDomainException : Exception
{
    protected CatalogDomainException()
    { }

    protected  CatalogDomainException(string message)
        : base(message)
    { }

    protected  CatalogDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
