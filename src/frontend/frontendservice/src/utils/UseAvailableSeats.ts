import type {HubConnection} from "@microsoft/signalr";
import type {AvailableSeatsMessage} from "../data/TrainComposition.ts";

export function sendAvailableSeatsQuery(
    connection: HubConnection,
    availableSeatsMessages: AvailableSeatsMessage[]) {
    if (!connection) return;

    if (connection.state === "Connected") {
        const args = {
            connectionId: connection?.connectionId,
            trainCompositionAvailableSeatsQueries: availableSeatsMessages,
        }
        connection.send("GetTrainCompositions", args)
        console.log("Sending AvailableSeats query")
    }
}


