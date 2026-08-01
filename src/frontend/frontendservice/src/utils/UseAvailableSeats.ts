import type {HubConnection} from "@microsoft/signalr";

export function sendAvailableSeatsQuery(
    connection: HubConnection,
    trainCompositionId: string,
    startStation: string,
    endStation: string,
    departureDate: string) {

    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            trainCompositionId: trainCompositionId,
            startStation: startStation,
            endStation: endStation,
            departureDate: departureDate,
        }
        connection.send("GetTrainCompositions", args)
        console.log("Sending AvailableSeats query")
    }
}


