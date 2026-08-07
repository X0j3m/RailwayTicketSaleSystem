import type {HubConnection} from "@microsoft/signalr";
import type {SeatReservation} from "../data/Reservation.ts";

export function sendReservationCommand(
    connection: HubConnection,
    seats: SeatReservation[]) {

    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            seatReservations: seats,
        }
        connection.send("CreateReservation", args);
        console.log("Sending Reservation command");
    }
}