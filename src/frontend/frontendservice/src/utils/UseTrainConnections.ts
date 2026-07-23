import type {HubConnection} from "@microsoft/signalr";

export function sendTrainConnectionsQuery(
    connection: HubConnection,
    startStation: string,
    endStation: string,
    departureDate: string,
    departureTime: string) {

    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            startStation: startStation,
            endStation: endStation,
            departureDate: departureDate,
            departureTime: departureTime
        }
        connection.send("GetTrainConnections", args)
        console.log("Sending TrainConnections query")
    }
}

