export interface TrainConnection {
    DepartureTime?: string;
    ArrivalTime?: string;
    TotalTripTime?: number;
    RelationTypes?: string[];
    TransferDetails?: TransferDetail[];
    NumOfTransfers: number;
    StationIds?: string[];
}

export interface TransferDetail {
    StationId?: string;
    ArrivalTime?: string;
    DepartureTime?: string;
    TransferTime?: number;
}

export interface TrainConnectionsMessage {
    ConnectionId?: string;
    MessageItems?: TrainConnection[];
}