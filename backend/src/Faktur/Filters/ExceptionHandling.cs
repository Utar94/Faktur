using Faktur.Application.Articles;
using Faktur.Contracts.Errors;
using Logitar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Faktur.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Func<ExceptionContext, ActionResult>> _handlers = new()
  {
    [typeof(GtinAlreadyUsedException)] = HandleGtinAlreadyUsedException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, ActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
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
}
