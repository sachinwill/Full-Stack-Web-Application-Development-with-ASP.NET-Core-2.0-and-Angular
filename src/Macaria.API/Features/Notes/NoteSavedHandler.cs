using Macaria.Core;
using Macaria.Core.DomainEvents;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Macaria.API.Features.Notes
{
    public class NoteSavedHandler : INotificationHandler<NoteSaved>
    {
        private readonly IHubContext<IntegrationEventsHub> _hubContext;

        public NoteSavedHandler(IHubContext<IntegrationEventsHub> hubContext)
            => _hubContext = hubContext;

        public async Task Handle(NoteSaved @event, CancellationToken cancellationToken)
            => await _hubContext.Clients.All
            .SendAsync("events", new
            {
                type = nameof(NoteSaved),
                payload = @event
            }, cancellationToken);
    }
}
