export interface TrainStation {
    id: string;
    city: string;
    name: string;
    latitude: number;
    longitude: number;
}

export interface StationsMessage {
    MessageTitle: string;
    MessageItems: TrainStation[];
}