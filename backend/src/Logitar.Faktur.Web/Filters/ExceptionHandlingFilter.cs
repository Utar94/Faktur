using FluentValidation;
using Logitar.Faktur.Application.Articles;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Products;
using Logitar.Faktur.Domain.Exceptions;
using Logitar.Faktur.Domain.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Faktur.Web.Filters;

internal class ExceptionHandlingFilter : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> handlers = new()
  {
    [typeof(DepartmentNotFoundException)] = HandleNotFoundFailureException,
    [typeof(GtinAlreadyUsedException)] = HandleConflictFailureException,
    [typeof(ProductNotFoundException)] = HandleNotFoundFailureException,
    [typeof(SkuAlreadyUsedException)] = HandleConflictFailureException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is AggregateNotFoundException)
    {
      context.Result = HandleNotFoundFailureException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is IdentifierAlreadyUsedException)
    {
      context.Result = HandleConflictFailureException(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException)
    {
      context.Result = HandleBadRequestDetailException(context);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static IActionResult HandleBadRequestDetailException(ExceptionContext context)
  {
    return new BadRequestObjectResult(context.Exception.GetErrorDetail());
  }

  private static IActionResult HandleConflictFailureException(ExceptionContext context)
  {
    return new ConflictObjectResult(((IFailureException)context.Exception).Failure);
  }

  private static IActionResult HandleNotFoundFailureException(ExceptionContext context)
  {
    return new NotFoundObjectResult(((IFailureException)context.Exception).Failure);
  }

  private static IActionResult HandleValidationException(ExceptionContext context)
  {
    return new BadRequestObjectResult(new { ((ValidationException)context.Exception).Errors });
  }
}
