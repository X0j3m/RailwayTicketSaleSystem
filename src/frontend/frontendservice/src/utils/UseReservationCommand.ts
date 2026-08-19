import type {HubConnection} from "@microsoft/signalr";
import type {SeatReservation} from "../data/Reservation.ts";

export function sendReservationCommand(
    connection: HubConnection,
    email: string,
    seats: SeatReservation[]) {

    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            email: email,
            seatReservations: seats,
        }
        connection.send("CreateReservation", args);
        console.log("Sending Reservation command");
    }
}

export function sendCancelReservationCommand(
    connection: HubConnection,
    ticketId: string
){
    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            ticketId: ticketId,
        }

        connection.send("CancelReservation", args);
        console.log("Sending CancelReservation command");
    }
}