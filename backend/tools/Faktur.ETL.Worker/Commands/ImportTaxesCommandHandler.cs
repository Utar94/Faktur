using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportTaxesCommandHandler : IRequestHandler<ImportTaxesCommand, int>
{
  private readonly ITaxRepository _taxRepository;

  public ImportTaxesCommandHandler(ITaxRepository taxRepository)
  {
    _taxRepository = taxRepository;
  }

  public async Task<int> Handle(ImportTaxesCommand command, CancellationToken cancellationToken)
  {
    ActorId actorId = new(command.Actor.Id);

    Dictionary<TaxCodeUnit, TaxAggregate> taxes = (await _taxRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Code, x => x);

    int count = 0;

    TaxCodeUnit gstCode = new("GST");
    if (!taxes.TryGetValue(gstCode, out TaxAggregate? gst))
    {
      gst = new(gstCode, rate: 0.05, actorId);
      taxes[gstCode] = gst;
    }
    gst.Flags = new FlagsUnit("F");
    gst.Update(actorId);
    if (gst.HasChanges)
    {
      count++;
    }

    TaxCodeUnit qstCode = new("QST");
    if (!taxes.TryGetValue(qstCode, out TaxAggregate? qst))
    {
      qst = new(qstCode, rate: 0.09975, actorId);
      taxes[qstCode] = qst;
    }
    qst.Flags = new FlagsUnit("P");
    qst.Update(actorId);
    if (gst.HasChanges)
    {
      count++;
    }

    await _taxRepository.SaveAsync(taxes.Values, cancellationToken);

    return count;
  }
}
