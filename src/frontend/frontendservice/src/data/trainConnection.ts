export interface TrainConnection {
    segments: TrainConnectionSegment[];
    trainChanges: number;
}

export interface TrainConnectionSegment {
    trainCompositionId: string;
    startStationId: string;
    endStationId: string;
    departureTime: string;
    arrivalTime: string;
    duration: string;
}

export interface TrainConnectionsMessage {
    MessageTitle: string;
    MessageItems: TrainConnection[];
}