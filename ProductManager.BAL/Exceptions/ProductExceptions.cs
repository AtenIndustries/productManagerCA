using ProductManager.BAL.DTO;
#pragma warning disable IDE0290 // Use primary constructor
using System;

namespace ProductManager.BAL.Exceptions;

[Serializable]
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int id) : base($"Product with Id {id} not found") { }

    public ProductNotFoundException() : base() { }

    public ProductNotFoundException(string? message) : base(message) { }

    public ProductNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class ProductConcurrencyException : Exception
{
    public ProductConcurrencyException(int id, Exception? innerException) : base(id.ToString(), innerException) { }

    public ProductConcurrencyException() : base() { }

    public ProductConcurrencyException(string? message) : base(message) { }

    public ProductConcurrencyException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class DuplicateProductException : Exception
{
    public DuplicateProductException(ProductReadDTO product, Exception? innerException) : base(product.Name.ToString(), innerException) { }
    public DuplicateProductException(ProductWriteDTO product, Exception? innerException) : base(product.Name.ToString(), innerException) { }
    public DuplicateProductException(ProductReadDTO product) : base(product.Name.ToString()) { }
    public DuplicateProductException(ProductWriteDTO product) : base(product.Name.ToString()) { }

    public DuplicateProductException() : base() { }

    public DuplicateProductException(string? message) : base(message) { }

    public DuplicateProductException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class ProductPersistenceException : Exception
{
    public ProductPersistenceException(int id, Exception? innerException) : base($"Product with Id {id} not saved", innerException) { }

    public ProductPersistenceException() : base() { }

    public ProductPersistenceException(string? message) : base(message) { }

    public ProductPersistenceException(string? message, Exception? innerException) : base(message, innerException) { }
}


#pragma warning restore IDE0290 // Use primary constructor