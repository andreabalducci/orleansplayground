using System.Threading.Tasks;
using GrainInterfaces;
using Grains.MyDomain;
using Microsoft.Extensions.Logging;
using NStore.Domain;
using Orleans;

namespace Grains
{
    public class Worker : Grain, IWorker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRepository _repository;
        private TicketAggregate _aggregate;

        public Worker(ILogger<Worker> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public Task<Result> ExecuteAsync(Command command)
        {
            _logger.LogInformation($"[{this.GetGrainIdentity().PrimaryKeyString}] {command.GetType().FullName}");
            return Task.FromResult(Result.Ok);
        }


        public override async Task OnActivateAsync()
        {
            this._aggregate = await _repository.GetByIdAsync<TicketAggregate>(this.GetGrainIdentity().PrimaryKeyString);
            _logger.LogInformation($"Activated {this.GetGrainIdentity().PrimaryKeyString} at version {_aggregate.Version}");
            
            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            _logger.LogInformation($"Deactivating {this.GetGrainIdentity().PrimaryKeyString}");
            return base.OnDeactivateAsync();
        }
    }
}