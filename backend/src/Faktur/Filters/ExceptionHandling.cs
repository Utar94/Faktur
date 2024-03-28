using Faktur.Application.Articles;
using Faktur.Application.Banners;
using Faktur.Application.Departments;
using Faktur.Application.Products;
using Faktur.Application.Stores;
using Faktur.Application.Taxes;
using Faktur.Contracts.Errors;
using Logitar;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Faktur.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Func<ExceptionContext, ActionResult>> _handlers = new()
  {
    [typeof(ArticleNotFoundException)] = HandleArticleNotFoundException,
    [typeof(BannerNotFoundException)] = HandleBannerNotFoundException,
    [typeof(DepartmentNotFoundException)] = HandleDepartmentNotFoundException,
    [typeof(GtinAlreadyUsedException)] = HandleGtinAlreadyUsedException,
    [typeof(SkuAlreadyUsedException)] = HandleSkuAlreadyUsedException,
    [typeof(StoreNotFoundException)] = HandleStoreNotFoundException,
    [typeof(TaxCodeAlreadyUsedException)] = HandleTaxCodeAlreadyUsedException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, ActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException tooManyResults)
    {
      Error error = new(tooManyResults.GetErrorCode(), TooManyResultsException.ErrorMessage);
      error.AddData(nameof(tooManyResults.ExpectedCount), tooManyResults.ExpectedCount.ToString());
      error.AddData(nameof(tooManyResults.ActualCount), tooManyResults.ActualCount.ToString());
      context.Result = new BadRequestObjectResult(error);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static NotFoundObjectResult HandleArticleNotFoundException(ExceptionContext context)
  {
    ArticleNotFoundException exception = (ArticleNotFoundException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), ArticleNotFoundException.ErrorMessage)
    {
      AttemptedValue = exception.ArticleId.ToString(),
      PropertyName = exception.PropertyName
    };
    return new NotFoundObjectResult(error);
  }

  private static NotFoundObjectResult HandleBannerNotFoundException(ExceptionContext context)
  {
    BannerNotFoundException exception = (BannerNotFoundException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), BannerNotFoundException.ErrorMessage)
    {
      AttemptedValue = exception.BannerId.ToString(),
      PropertyName = exception.PropertyName
    };
    return new NotFoundObjectResult(error);
  }

  private static NotFoundObjectResult HandleDepartmentNotFoundException(ExceptionContext context)
  {
    DepartmentNotFoundException exception = (DepartmentNotFoundException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), DepartmentNotFoundException.ErrorMessage)
    {
      AttemptedValue = exception.DepartmentNumber,
      PropertyName = exception.PropertyName
    };
    error.AddData(nameof(exception.StoreId), exception.StoreId.ToString());
    return new NotFoundObjectResult(error);
  }

  private static ConflictObjectResult HandleGtinAlreadyUsedException(ExceptionContext context)
  {
    GtinAlreadyUsedException exception = (GtinAlreadyUsedException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), GtinAlreadyUsedException.ErrorMessage)
    {
      AttemptedValue = exception.Gtin,
      PropertyName = exception.PropertyName
    };
    return new ConflictObjectResult(error);
  }

  private static ConflictObjectResult HandleSkuAlreadyUsedException(ExceptionContext context)
  {
    SkuAlreadyUsedException exception = (SkuAlreadyUsedException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), SkuAlreadyUsedException.ErrorMessage)
    {
      AttemptedValue = exception.Sku,
      PropertyName = exception.PropertyName
    };
    error.AddData(nameof(exception.StoreId), exception.StoreId.ToString());
    return new ConflictObjectResult(error);
  }

  private static NotFoundObjectResult HandleStoreNotFoundException(ExceptionContext context)
  {
    StoreNotFoundException exception = (StoreNotFoundException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), StoreNotFoundException.ErrorMessage)
    {
      AttemptedValue = exception.StoreId.ToString(),
      PropertyName = exception.PropertyName
    };
    return new NotFoundObjectResult(error);
  }

  private static ConflictObjectResult HandleTaxCodeAlreadyUsedException(ExceptionContext context)
  {
    TaxCodeAlreadyUsedException exception = (TaxCodeAlreadyUsedException)context.Exception;
    ValidationError error = new(exception.GetErrorCode(), TaxCodeAlreadyUsedException.ErrorMessage)
    {
      AttemptedValue = exception.TaxCode,
      PropertyName = exception.PropertyName
    };
    return new ConflictObjectResult(error);
  }
}
