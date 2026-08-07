using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Dtos
{
    public record OccupiedSeatDto
    {
        public required int CarNumber { get; set; }
        public required int SeatNumber { get; set; }
    }
}
