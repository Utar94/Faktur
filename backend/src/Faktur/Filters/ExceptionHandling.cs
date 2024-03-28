using Faktur.Application;
using Faktur.Contracts.Errors;
using FluentValidation;
using FluentValidation.Results;
using Logitar;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Faktur.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is ValidationException validation)
    {
      ValidationError error = new();
      foreach (ValidationFailure failure in validation.Errors)
      {
        error.Errors.Add(new PropertyError(failure.ErrorCode, failure.ErrorMessage, failure.AttemptedValue, failure.PropertyName));
      }
      context.Result = new BadRequestObjectResult(error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is ConflictException conflict)
    {
      context.Result = new ConflictObjectResult(conflict.Error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is NotFoundException notFound)
    {
      context.Result = new NotFoundObjectResult(notFound.Error);
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
}
