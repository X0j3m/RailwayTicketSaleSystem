import type {HubConnection} from "@microsoft/signalr";
import type {TrainConnectionQuery} from "../data/trainConnection.ts";

export function sendTrainConnectionsQuery(
    connection: HubConnection,
    trainConnectionsQuery: TrainConnectionQuery) {

    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            startStation: trainConnectionsQuery.StartStation,
            endStation: trainConnectionsQuery.EndStation,
            departureDate: trainConnectionsQuery.DepartureDate,
            departureTime: trainConnectionsQuery.DepartureTime,
            pageNumber: trainConnectionsQuery.PageNumber,
            pageSize: trainConnectionsQuery.PageSize,
        }
        connection.send("GetTrainConnections", args);
        console.log("Sending TrainConnections query");
    }
}

