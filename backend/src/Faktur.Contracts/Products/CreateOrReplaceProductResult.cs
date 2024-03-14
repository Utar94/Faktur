namespace Faktur.Contracts.Products;

public record CreateOrReplaceProductResult(bool IsCreated, Product Product);
