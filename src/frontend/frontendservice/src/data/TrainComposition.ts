export interface TrainComposition {
    EndStationId: string
    StartStationId: string
    TrainCompositionId: string
    TrainNumber: number
    TrainType: string
    Cars: Car[]
}

export interface Car {
    Number: number
    Seats: Seat[]
}

export interface Seat {
    Number: number
    Occupied: boolean
    XPosition: number
    YPosition: number
}


export interface TrainCompositionsMessage {
    ConnectionId?: string;
    MessageItems?: TrainComposition[];
}