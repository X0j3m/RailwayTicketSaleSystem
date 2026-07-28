import SearchConnection from "../SearchConnection/SearchConnection.tsx";
import {useState} from "react";
import type {TrainConnection} from "../../data/trainConnection.ts";
import {useTrainStations} from "../../utils/UseTrainStations.ts";

export interface SearchConnectionProps {
    trainStations: Array<{ value: string; label: string }>;
    setTrainConnections: React.Dispatch<React.SetStateAction<TrainConnection[]>>;
}

function TrainConnections() {
    const [trainConnections, setTrainConnections] = useState<TrainConnection[]>([]);

    const trainStations = useTrainStations().map((station) =>
        ({value: station.id, label: station.name}));

    return (
        <>
            <SearchConnection trainStations={trainStations} setTrainConnections={setTrainConnections}/>

            {trainConnections.length == 0 && <p><strong>No connections</strong></p>}

            {trainConnections && trainConnections.length > 0 &&
                trainConnections.map((trainConnection: TrainConnection) => {
                    console.log(trainConnection);
                    return (
                        <div>
                            Train Connection
                            <br/>Departure:
                            {trainConnection.DepartureTime}
                            <br/>Arrival:
                            {trainConnection.ArrivalTime}
                            <br/>Train changes:
                            {trainConnection.NumOfTransfers}
                            <br/>Segments:
                            <ul>
                                {trainConnection.StationIds?.map((s: string) => {
                                    const station = trainStations.find(station => station.value == s);
                                    return (<li>{station?.label}</li>);
                                })}
                            </ul>
                        </div>);
                })
            }
        </>);
}

export default TrainConnections;