using Faktur.Application.Activities;
using Faktur.Extensions;
using MediatR;

namespace Faktur;

internal class ActivityPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ActivityPipelineBehavior(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (request is Activity activity && _httpContextAccessor.HttpContext != null)
    {
      ActivityContext context = new(_httpContextAccessor.HttpContext.GetUser());
      activity.Contextualize(context);
    }

    return await next();
  }
}
