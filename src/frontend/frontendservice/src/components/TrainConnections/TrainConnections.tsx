import {type Dispatch, type SetStateAction, useState} from "react";
import type {TrainConnection} from "../../data/trainConnection.ts";
import {useTrainStations} from "../../utils/UseTrainStations.ts";
import SearchBar from "./SearchBar/SearchBar.tsx";
import TrainConnectionBlock from "./TrainConnectionBlock/TrainConnectionBlock.tsx";
import TrainConnectionsMap from "../TrainConnectionsMap/TrainConnectionsMap.tsx";

export type StartStationState = [
        string | undefined,
    Dispatch<SetStateAction<string | undefined>>
];

export type EndStationState = [
        string | undefined,
    Dispatch<SetStateAction<string | undefined>>
];

function TrainConnections() {
    const [trainConnections, setTrainConnections] = useState<TrainConnection[]>([]);
    const [startStationId, setStartStationId] = useState<string | undefined>(undefined);
    const [endStationId, setEndStationId] = useState<string | undefined>(undefined);

    const trainStations = useTrainStations();

    console.log(trainStations);

    return (
        <>
            <SearchBar
                trainStations={trainStations}
                setTrainConnections={setTrainConnections}
                startStationState={[startStationId, setStartStationId]}
                endStationState={[endStationId, setEndStationId]}/>

            <TrainConnectionsMap
                trainStations={trainStations}
                startStationState={[startStationId, setStartStationId]}
                endStationState={[endStationId, setEndStationId]}/>

            {trainConnections && trainConnections.length > 0 &&
                trainConnections.map((trainConnection: TrainConnection) => {
                    return <TrainConnectionBlock trainConnection={trainConnection} trainStations={trainStations}/>
                })
            }
        </>);
}

export default TrainConnections;