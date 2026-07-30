import type {TrainConnection} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";

interface TrainConnectionBlockProps {
    trainConnection: TrainConnection;
    trainStations: TrainStation[];
    setSelectedTrainConnection: React.Dispatch<React.SetStateAction<TrainConnection | null>>;
}

function TrainConnectionBlock({ trainConnection, trainStations, setSelectedTrainConnection }: TrainConnectionBlockProps) {
    function handleClick(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        setSelectedTrainConnection(trainConnection);
    }

    return (
        <div>
            Train Connection
            <br/>
            <button type='button'
                    onClick={handleClick}>
                See route
            </button>
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