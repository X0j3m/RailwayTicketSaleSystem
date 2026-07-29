import {useEffect, useState} from "react";
import {sendTrainConnectionsQuery} from "../../../utils/UseTrainConnections.ts";
import {useSignalR} from "../../../hooks/useSignalR.ts";
import type {
    TrainConnection,
    TransferDetail,
    TrainConnectionsMessage
} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import type {EndStationState, StartStationState} from "../TrainConnections.tsx";

export interface SearchConnectionProps {
    trainStations: Array<TrainStation>;
    setTrainConnections: React.Dispatch<React.SetStateAction<TrainConnection[]>>;
    startStationState: StartStationState;
    endStationState: EndStationState;
}

function SearchBar({trainStations, setTrainConnections, startStationState, endStationState}: SearchConnectionProps) {
    const {connection} = useSignalR();

    const [startStationId,] = startStationState;
    const [endStationId,] = endStationState;

    const [departureTime, setDepartureTime] = useState(new Date().toLocaleTimeString('pl-PL', {
        hour: '2-digit',
        minute: '2-digit',
    }));
    const [departureDate, setDepartureDate] = useState(new Date().toISOString().split('T')[0]);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveConnections = (data: string) => {
            try {
                const rawData: TrainConnectionsMessage = JSON.parse(data);

                const parsedData: TrainConnection[] = rawData?.MessageItems?.map((conn: TrainConnection) => ({
                    DepartureTime: conn?.DepartureTime,
                    ArrivalTime: conn?.ArrivalTime,
                    TotalTripTime: conn?.TotalTripTime,
                    RelationTypes: conn?.RelationTypes,
                    TransferDetails: (conn?.TransferDetails || []).map((detail: TransferDetail) => ({
                        StationId: detail?.StationId,
                        ArrivalTime: detail?.ArrivalTime,
                        DepartureTime: detail?.DepartureTime,
                        TransferTime: detail?.TransferTime,
                    })) || [],
                    NumOfTransfers: conn?.NumOfTransfers,
                    StationIds: conn?.StationIds,
                    TrainCompositionIds: conn?.TrainCompositionIds
                })) || [];

                if (setTrainConnections) {
                    setTrainConnections(parsedData);
                }

            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections);

        return () => {
            connection.off("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections);
        };
    }, [connection, setTrainConnections]);

    const handleClick = () => {
        if (!connection || !startStationId || !endStationId) return;
        setTrainConnections([]);
        if (connection.state === "Connected") {
            sendTrainConnectionsQuery(
                connection,
                startStationId,
                endStationId,
                departureDate,
                departureTime
            );
        }
    };

    const startStatinName = trainStations.find(s => s.id === startStationId)?.name ?? 'Start station';
    const endStatinName = trainStations.find(s => s.id === endStationId)?.name ?? 'End station';

    return (
        <form onSubmit={(e) => {
            e.preventDefault();
            handleClick();
        }}>
            <span>{startStatinName}</span>
            &nbsp;
            <span>{endStatinName}</span>
            &nbsp;
            <input
                value={departureTime}
                onChange={(e) => setDepartureTime(e.target.value)}
                type='time'
                name='departureTime'
            />
            &nbsp;
            <input
                value={departureDate}
                onChange={(e) => setDepartureDate(e.target.value)}
                type='date'
                name='departureDate'
            />
            &nbsp;
            <button
                disabled={!departureTime || !departureDate || !startStationId || !endStationId}
                type='submit'>
                Search
            </button>
        </form>
    );
}

export default SearchBar;