import type {TrainConnection} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";

interface TrainConnectionBlockProps {
    trainConnection: TrainConnection;
    trainStations: TrainStation[];
}

function TrainConnectionBlock({ trainConnection, trainStations }: TrainConnectionBlockProps) {
    return (
        <div>
            Train Connection
            <br/>
            <button>See available seats</button>
            <br/>Departure:
            {trainConnection.DepartureTime}
            <br/>Arrival:
            {trainConnection.ArrivalTime}
            <br/>Train changes:
            {trainConnection.NumOfTransfers}
            <br/>Segments:
            <ul>
                {trainConnection.StationIds?.map((s: string) => {
                    const station = trainStations.find(station => station.id == s);
                    return (<li>{station?.name}</li>);
                })}
            </ul>
        </div>);
}

export default TrainConnectionBlock;