import type {TrainConnection, Transit} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import {sendAvailableSeatsQuery} from "../../../utils/UseAvailableSeats.ts";
import {useSignalR} from "../../../hooks/useSignalR.ts";
import type {AvailableSeatsMessage} from "../../../data/TrainComposition.ts";

interface TrainConnectionBlockProps {
    trainConnection: TrainConnection;
    trainStations: TrainStation[];
    setSelectedTrainConnection: React.Dispatch<React.SetStateAction<TrainConnection | null>>;
    setMouseOverTrainConnection: React.Dispatch<React.SetStateAction<TrainConnection | null>>;
}

function TrainConnectionBlock({trainConnection, trainStations, setSelectedTrainConnection, setMouseOverTrainConnection}: TrainConnectionBlockProps) {
    const {connection} = useSignalR();

    function handleSeatsClick(e: React.MouseEvent<HTMLButtonElement>) {
        if (!connection || !trainConnection || !trainConnection.Transits) return;

        e.preventDefault();

        const args: AvailableSeatsMessage[] = []

        setSelectedTrainConnection(trainConnection);

        trainConnection.Transits.forEach((transit: Transit) => {
            const availableSeatsMessage: AvailableSeatsMessage = {
                trainCompositionId: transit.TrainCompositionId,
                startStation: transit.FromStationId,
                endStation: transit.ToStationId,
                departureDate: ''
            }

            args.push(availableSeatsMessage)
        });

        sendAvailableSeatsQuery(connection, args)
    }

    return (
        <div
            style={{userSelect: "none", border: "2px solid black"}}
            onMouseEnter={() => setMouseOverTrainConnection(trainConnection)}
            onMouseLeave={() => setMouseOverTrainConnection(null)}>
            <button type='button'
                    onClick={handleSeatsClick}>
                See available seats
            </button>
            {trainConnection.Transits?.map((transit: Transit) => {
                const fromStation = trainStations.find(station => station.id == transit.FromStationId);
                const toStation = trainStations.find(station => station.id == transit.ToStationId);
                return (
                    <>
                        <br/>
                        <span>
                            {fromStation?.name} ({transit.DepartureTime}) &rarr; {toStation?.name} ({transit.ArrivalTime})
                        </span>
                    </>);
            })}
        </div>);
}

export default TrainConnectionBlock;