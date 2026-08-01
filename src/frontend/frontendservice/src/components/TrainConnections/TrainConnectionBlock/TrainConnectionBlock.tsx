import type {TrainConnection, Transit} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import {sendAvailableSeatsQuery} from "../../../utils/UseAvailableSeats.ts";
import {useSignalR} from "../../../hooks/useSignalR.ts";

interface TrainConnectionBlockProps {
    trainConnection: TrainConnection;
    trainStations: TrainStation[];
    setSelectedTrainConnection: React.Dispatch<React.SetStateAction<TrainConnection | null>>;
}

function TrainConnectionBlock({trainConnection, trainStations, setSelectedTrainConnection}: TrainConnectionBlockProps) {
    const {connection} = useSignalR();

    function handleRouteClick(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        setSelectedTrainConnection(trainConnection);
    }

    function handleSeatsClick(e: React.MouseEvent<HTMLButtonElement>) {
        if (!connection || !trainConnection || !trainConnection.Transits) return;

        e.preventDefault();

        trainConnection.Transits.forEach((transit: Transit) => {
            sendAvailableSeatsQuery(
                connection,
                transit.TrainCompositionId,
                transit.FromStationId,
                transit.ToStationId,
                ''
            )
        });
    }

    return (
        <div>
            Train Connection
            <br/>
            <button type='button'
                    onClick={handleRouteClick}>
                See route
            </button>
            <button type='button'
                    onClick={handleSeatsClick}>
                See available seats
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