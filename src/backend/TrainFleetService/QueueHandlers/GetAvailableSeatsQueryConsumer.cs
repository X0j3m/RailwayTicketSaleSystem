using Contracts.Messages.Backend.Query;
using MassTransit;
using Models.Dto;

namespace TrainFleetService.QueueHandler
{
    internal class GetAvailableSeatsQueryConsumer : IConsumer<GetAvailableSeatsQuery>
    {
        private readonly ILogger<GetAvailableSeatsQueryConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public GetAvailableSeatsQueryConsumer(ILogger<GetAvailableSeatsQueryConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<GetAvailableSeatsQuery> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received GetAvailableSeatsQuery: TrainCompositionId={TrainCompositionId}, StartStation={StartStation}, EndStation={EndStation}",
                message.TrainCompositionId, message.StartStation, message.EndStation);

            await Task.Delay(3000);

            await _endpoint.Publish(new AvailableSeatsQueryResponse
            {
                Train = new TrainCompositionDto
                {
                    TrainType = "Sample Train Type",
                    TrainNumber = 123,
                    Cars = new List<CarDto>
                    {
                        new CarDto
                        {
                            Number = 1,
                            Seats = new List<SeatDto>
                            {
                                new SeatDto { Number = 1, XPosition = 0, YPosition = 0, Ocupied = false },
                                new SeatDto { Number = 2, XPosition = 1, YPosition = 0, Ocupied = true },
                                new SeatDto { Number = 3, XPosition = 0, YPosition = 1, Ocupied = false },
                                new SeatDto { Number = 4, XPosition = 1, YPosition = 1, Ocupied = true }
                            }
                        },
                        new CarDto
                        {
                            Number = 2,
                            Seats = new List<SeatDto>
                            {
                                new SeatDto { Number = 1, XPosition = 0, YPosition = 0, Ocupied = true },
                                new SeatDto { Number = 2, XPosition = 1, YPosition = 0, Ocupied = false },
                                new SeatDto { Number = 3, XPosition = 0, YPosition = 1, Ocupied = true },
                                new SeatDto { Number = 4, XPosition = 1, YPosition = 1, Ocupied = true }
                            }
                        }
                    }
                }
            });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
