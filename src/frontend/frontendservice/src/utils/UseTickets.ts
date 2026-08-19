import type {HubConnection} from "@microsoft/signalr";

export function sendTicketsQuery(
    connection: HubConnection,
    email: string) {
    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            email: email,
        }
        connection.send("GetTickets", args);
        console.log("Sending GetTickets query");
    }
}