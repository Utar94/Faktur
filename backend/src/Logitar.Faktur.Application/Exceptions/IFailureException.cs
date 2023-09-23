using FluentValidation.Results;

namespace Logitar.Faktur.Application.Exceptions;

public interface IFailureException
{
  ValidationFailure Failure { get; }
}
