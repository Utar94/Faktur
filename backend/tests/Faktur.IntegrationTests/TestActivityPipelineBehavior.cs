using Faktur.Application.Activities;
using MediatR;

namespace Faktur;

internal class TestActivityPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  private readonly TestContext _context;

  public TestActivityPipelineBehavior(TestContext context)
  {
    _context = context;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (request is Activity activity)
    {
      ActivityContext context = new(_context.User);
      activity.Contextualize(context);
    }

    return await next();
  }
}
