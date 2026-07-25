import SearchConnection from "../SearchConnection/SearchConnection.tsx";
import {useState} from "react";
import type {TrainConnection, TrainConnectionSegment} from "../../data/trainConnection.ts";
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
            {trainConnections && trainConnections.length > 0 &&
                trainConnections.map((trainConnection: TrainConnection) => {
                    console.log(trainConnection);
                    return (
                        <div>
                            Train Connection
                            <br/>Train changes:
                            {trainConnection.trainChanges}
                            <br/>Segments:
                            {trainConnection.segments?.map((segment: TrainConnectionSegment) => {
                                return (<p>
                                    {segment.departureTime}
                                    {segment.arrivalTime}
                                </p>)
                            })}
                        </div>);
                })
            }
        </>);
}

export default TrainConnections;