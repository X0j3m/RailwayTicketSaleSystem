export interface SeatReservation {
    TrainComposition: string;
    SegmentNumber: number;
    CarNumber: number;
    SeatNumber: number;
    FromStationId: string;
    ToStationId: string;
    DepartureTime: string;
    ArrivalTime: string;
}