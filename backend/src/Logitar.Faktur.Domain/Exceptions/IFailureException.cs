using FluentValidation.Results;

namespace Logitar.Faktur.Domain.Exceptions;

public interface IFailureException
{
  ValidationFailure Failure { get; }
}
