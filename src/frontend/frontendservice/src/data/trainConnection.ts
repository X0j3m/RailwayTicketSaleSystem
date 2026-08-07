export interface TrainConnection {
    DepartureTime?: string;
    ArrivalTime?: string;
    TotalTripTime?: number;
    Transits?: Transit[];
    TransferDetails?: TransferDetail[];
    NumOfTransfers: number;
    StationIds?: string[];
    TrainCompositionIds?: string[];
}

export interface TransferDetail {
    StationId: string;
    ArrivalTime: string;
    DepartureTime: string;
    TransferTime: number;
}

export interface Transit {
    FromStationId: string
    ToStationId: string
    ArrivalTime: string
    DepartureTime: string
    TravelTime: number
    TrainCompositionId: string
}

export interface TrainConnectionsMessage {
    ConnectionId?: string;
    NumberOfPages: number;
    PageNumber: number;
    PageSize: number;
    MessageItems?: TrainConnection[];
}

export interface TrainConnectionQuery {
    StartStation: string,
    EndStation: string,
    DepartureDate: string,
    DepartureTime: string,
    PageNumber: number,
    PageSize: number
}