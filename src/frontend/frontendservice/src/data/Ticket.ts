export interface Ticket {
    TicketId: string
}

export interface TicketMessage {
    ConnectionId?: string;
    MessageItems?: Ticket[];
}