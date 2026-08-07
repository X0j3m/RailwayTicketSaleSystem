import './TrainConnections.css'
import {type Dispatch, type SetStateAction, useState} from "react";
import type {TrainConnection, TrainConnectionQuery} from "../../data/trainConnection.ts";
import SearchBar from "./SearchBar/SearchBar.tsx";
import TrainConnectionBlock from "./TrainConnectionBlock/TrainConnectionBlock.tsx";
import TrainConnectionsMap from "../TrainConnectionsMap/TrainConnectionsMap.tsx";
import type {Page} from "../../data/Page.ts";
import ConnectionsSearchHandler from "./ConnectionsSearchHandler.tsx";
import {addDays, format, parseISO} from 'date-fns';
import {useTrainStations} from "../../utils/UseTrainStations.ts";
import TrainScheme from "../TrainScheme/TrainScheme.tsx";

export type StartStationState = [
        string | undefined,
    Dispatch<SetStateAction<string | undefined>>
];

export type EndStationState = [
        string | undefined,
    Dispatch<SetStateAction<string | undefined>>
];

function TrainConnections() {
    const [trainConnectionsQuery, setTrainConnectionsQuery] = useState<TrainConnectionQuery | null>(null);

    const [trainConnectionsMetadata, setTrainConnectionsMetadata] = useState<Page | null>(null);
    const [trainConnections, setTrainConnections] = useState<TrainConnection[] | null>([]);

    const [startStationId, setStartStationId] = useState<string | undefined>(undefined);
    const [endStationId, setEndStationId] = useState<string | undefined>(undefined);

    const [selectedConnectionsPageNumber, setSelectedConnectionsPageNumber] = useState<number>(0);

    const [selectedTrainConnection, setSelectedTrainConnection] = useState<TrainConnection | null>(null);
    const [mouseOverTrainConnection, setMouseOverTrainConnection] = useState<TrainConnection | null>(null);

    const trainStations = useTrainStations();

    function handleChangeDayClick(offset: number) {
        if (!trainConnectionsQuery) return;

        console.log(trainConnectionsQuery.DepartureTime);

        const newTrainConnectionsQuery: TrainConnectionQuery = {
            StartStation: trainConnectionsQuery.StartStation,
            EndStation: trainConnectionsQuery.EndStation,
            DepartureDate: format(addDays(parseISO(trainConnectionsQuery.DepartureDate), offset), 'yyyy-MM-dd'),
            DepartureTime: trainConnectionsQuery.DepartureTime,
            PageNumber: 0,
            PageSize: trainConnectionsQuery.PageSize
        };

        setTrainConnectionsQuery(newTrainConnectionsQuery);
        setSelectedConnectionsPageNumber(0);
    }

    function handleChangePageClick(offset: number) {
        if (!trainConnectionsQuery) return;
        setSelectedConnectionsPageNumber(selectedConnectionsPageNumber + offset);

        const newTrainConnectionsQuery: TrainConnectionQuery = {
            StartStation: trainConnectionsQuery.StartStation,
            EndStation: trainConnectionsQuery.EndStation,
            DepartureDate: trainConnectionsQuery.DepartureDate,
            DepartureTime: trainConnectionsQuery.DepartureTime,
            PageNumber: trainConnectionsQuery.PageNumber + offset,
            PageSize: trainConnectionsQuery.PageSize
        };

        setTrainConnectionsQuery(newTrainConnectionsQuery);
    }


    return (
        <>
            <TrainScheme trainStations={trainStations}
                         trainConnection={selectedTrainConnection}
                         trainConnectionQuery={trainConnectionsQuery}/>
            <>
                <ConnectionsSearchHandler
                    setTrainConnections={setTrainConnections}
                    setTrainConnectionsMetadata={setTrainConnectionsMetadata}
                    trainConnectionsQuery={trainConnectionsQuery}/>

                <div className={"container"}>
                    <div className={"box_left"}>
                        <TrainConnectionsMap
                            trainStations={trainStations}
                            startStationState={[startStationId, setStartStationId]}
                            endStationState={[endStationId, setEndStationId]}
                            selectedTrainConnection={mouseOverTrainConnection}/>
                    </div>

                    <div className={"box_right"}>
                        {trainConnectionsQuery ?
                            <div>
                                {trainStations.find(station => station.id == trainConnectionsQuery.StartStation)?.name ?? ""}
                                &nbsp;&rarr;&nbsp;
                                {trainStations.find(station => station.id == trainConnectionsQuery.EndStation)?.name ?? ""}
                                &nbsp;
                                <button
                                    onClick={() => {
                                        setTrainConnectionsQuery(null);
                                        setTrainConnections([]);
                                        setTrainConnectionsMetadata(null);
                                    }}>
                                    Change
                                </button>
                            </div>
                            :
                            <SearchBar
                                setTrainConnectionsQuery={setTrainConnectionsQuery}
                                trainStations={trainStations}
                                setTrainConnections={setTrainConnections}
                                startStationState={[startStationId, setStartStationId]}
                                endStationState={[endStationId, setEndStationId]}/>
                        }
                        <div>
                            {trainConnectionsQuery &&
                                <>
                                    <button
                                        disabled={trainConnectionsQuery?.DepartureDate == new Date().toISOString().split('T')[0]}
                                        onClick={() => handleChangeDayClick(-1)}>
                                        {format(addDays(parseISO(trainConnectionsQuery?.DepartureDate), -1), 'yyyy-MM-dd')}
                                    </button>
                                    &nbsp;
                                    <span>
                                        {trainConnectionsQuery?.DepartureDate}
                                    </span>
                                    &nbsp;
                                    <button
                                        onClick={() => handleChangeDayClick(1)}>
                                        {format(addDays(parseISO(trainConnectionsQuery?.DepartureDate), 1), 'yyyy-MM-dd')}
                                    </button>
                                </>
                            }
                        </div>
                        <div>
                            {trainConnectionsMetadata &&
                                <>
                                    <button
                                        disabled={
                                            selectedConnectionsPageNumber == 0
                                            ||
                                            trainConnectionsMetadata.NumberOfPages == 0
                                            ||
                                            !trainConnections
                                        }
                                        onClick={() => handleChangePageClick(-1)}>
                                        See before
                                    </button>
                                </>
                            }
                        </div>
                        {
                            trainConnections == null
                                ?
                                <span style={{userSelect: "none"}}>Loading</span>
                                :
                                trainConnections && trainConnections.length > 0 &&
                                trainConnections.map((trainConnection: TrainConnection) => {
                                    return <TrainConnectionBlock trainConnection={trainConnection}
                                                                 trainStations={trainStations}
                                                                 setSelectedTrainConnection={setSelectedTrainConnection}
                                                                 setMouseOverTrainConnection={setMouseOverTrainConnection}/>
                                })
                        }
                        <div>
                            {trainConnectionsMetadata &&
                                <>
                                    {selectedConnectionsPageNumber < trainConnectionsMetadata.NumberOfPages - 1 &&
                                        <button
                                            disabled={
                                                selectedConnectionsPageNumber == trainConnectionsMetadata.NumberOfPages - 1
                                                ||
                                                trainConnectionsMetadata.NumberOfPages == 0
                                                ||
                                                !trainConnections
                                            }
                                            onClick={() => handleChangePageClick(1)}>
                                            See later
                                        </button>
                                    }
                                    {selectedConnectionsPageNumber >= trainConnectionsMetadata.NumberOfPages - 1 &&
                                        <>
                                            <span>No more connections see</span>
                                            <button
                                                disabled={!trainConnections}
                                                onClick={() => handleChangeDayClick(1)}>
                                                next day
                                            </button>
                                        </>
                                    }
                                </>
                            }
                        </div>
                    </div>
                </div>
            </>
        </>);
}

export default TrainConnections;