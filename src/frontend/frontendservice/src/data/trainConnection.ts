// BACKEND
export interface RawTrainConnectionSegment {
    TrainCompositionId?: string;
    StartStation?: string;
    EndStation?: string;
    DepartureTime?: string;
    ArrivalTime?: string;
    Duration?: string;
}

export interface RawTrainConnection {
    Segments?: RawTrainConnectionSegment[];
    TrainChanges?: number;
}

export interface TrainConnectionsMessage {
    MessageTitle?: string;
    MessageItems?: RawTrainConnection[];
}

// FRONTEND
export interface TrainConnectionSegment {
    trainCompositionId?: string;
    startStationId?: string;
    endStationId?: string;
    departureTime?: string;
    arrivalTime?: string;
    duration?: string;
}

export interface TrainConnection {
    segments?: TrainConnectionSegment[];
    trainChanges?: number;
}