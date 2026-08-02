using Models.Abstraction;

namespace Models.Dto
{
    public record TrainConnectionsDtoPage : IDto
    {
        public required int NumberOfPages { get; init; }
        public required int PageNumber { get; init; }
        public required int PageSize { get; init; }
        public required List<TrainConnectionDto> Connections { get; init; }
    }
}
