import type {TrainConnection, TrainConnectionQuery, Transit} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import {sendAvailableSeatsQuery} from "../../../utils/UseAvailableSeats.ts";
import {useSignalR} from "../../../hooks/useSignalR.ts";
import type {AvailableSeatsMessage} from "../../../data/TrainComposition.ts";
import {toDateObjString, addToDateObjString} from "../../../utils/TimeCalculator.ts";
import {useNavigate} from "react-router-dom";


interface TrainConnectionBlockProps {
    trainConnection: TrainConnection;
    trainConnectionsQuery: TrainConnectionQuery
    trainStations: TrainStation[];
    setMouseOverTrainConnection: React.Dispatch<React.SetStateAction<TrainConnection | null>>;
}

function TrainConnectionBlock({
                                  trainConnection,
                                  trainConnectionsQuery,
                                  trainStations,
                                  setMouseOverTrainConnection
                              }: TrainConnectionBlockProps) {
    const {connection} = useSignalR();
    const navigate = useNavigate();

    function handleSeatsClick(e: React.MouseEvent<HTMLButtonElement>) {

        if (!connection || !trainConnection || !trainConnection.Transits) return;

        e.preventDefault();

        const args: AvailableSeatsMessage[] = []

        trainConnection.Transits.forEach((transit: Transit) => {
            const departureTime = toDateObjString(trainConnectionsQuery.DepartureDate, transit.DepartureTime);
            const arrivalTime = addToDateObjString(departureTime, transit.TravelTime);

            const availableSeatsMessage: AvailableSeatsMessage = {
                trainCompositionId: transit.TrainCompositionId,
                startStation: transit.FromStationId,
                endStation: transit.ToStationId,
                departureTime: departureTime,
                arrivalTime: arrivalTime
            }

            args.push(availableSeatsMessage)
        });

        sendAvailableSeatsQuery(connection, args)

        navigate("/seat-selection", {
            state: {
                trainConnection,
                trainConnectionsQuery
            }
        });
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