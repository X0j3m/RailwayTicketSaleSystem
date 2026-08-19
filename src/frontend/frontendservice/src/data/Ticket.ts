export interface Ticket {
    TicketId: string
}

export interface TicketMessage {
    ConnectionId?: string;
    MessageItems?: Ticket[];
}

export interface TicketInfo {
    TicketId: string;
    DepartureTime: string;
    ArrivalTime: string;
    FromStationId: string;
    ToStationId: string;
}

export interface TicketsMessage {
    ConnectionId?: string;
    MessageItems?: TicketInfo[];
}