using Contracts.Messages.Backend.Query;
using MassTransit;
using Models.Dto;

namespace TimetableService.QueueHandler
{
    internal class GetTrainConnectionsQueryConsumer : IConsumer<GetTrainConnectionsQuery>
    {

        private readonly ILogger<GetTrainConnectionsQueryConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public GetTrainConnectionsQueryConsumer(ILogger<GetTrainConnectionsQueryConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<GetTrainConnectionsQuery> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received GetTrainConnectionsQuery: StartStation={StartStation}, EndStation={EndStation}, DepartureTime={DepartureTime}",
                message.StartStation, message.EndStation, message.DepartureTime);

            await Task.Delay(3000);

            await _endpoint.Publish(new TrainConnectionsQueryResponse
            {
                Connections = new List<TrainConnectionDto>
                {
                    new TrainConnectionDto
                    {
                        TrainCompositionId = Guid.NewGuid(),
                        StartStation = message.StartStation,
                        EndStation = message.EndStation,
                        DepartureTime = message.DepartureTime.AddMinutes(10),
                        ArrivalTime = message.DepartureTime.AddMinutes(2*60),
                        Duration = TimeSpan.FromMinutes(2*60 - 10)
                    },
                    new TrainConnectionDto
                    {
                        TrainCompositionId = Guid.NewGuid(),
                        StartStation = message.StartStation,
                        EndStation = message.EndStation,
                        DepartureTime = message.DepartureTime.AddMinutes(20),
                        ArrivalTime = message.DepartureTime.AddMinutes(3*60),
                        Duration = TimeSpan.FromMinutes(3*60 - 20)
                    }
                }
            });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
