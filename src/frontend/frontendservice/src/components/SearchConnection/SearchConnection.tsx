import {useEffect, useState} from "react";
import {sendTrainConnectionsQuery} from "../../utils/UseTrainConnections.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {
    TrainConnection,
    TransferDetail,
    TrainConnectionsMessage
} from "../../data/trainConnection.ts";
import {type SearchConnectionProps} from "../TrainConnections/TrainConnections.tsx";

function SearchConnection({trainStations, setTrainConnections}: SearchConnectionProps) {
    const {connection} = useSignalR();

    // const trainStations = useTrainStations().map((station) => ({value: station.id, label: station.name}));

    const defaultStart = trainStations[8]?.value || "";
    const defaultEnd = trainStations[9]?.value || trainStations[8]?.value || "";

    const [startStation, setStartStation] = useState<string>(defaultStart);
    const [endStation, setEndStation] = useState<string>(defaultEnd);
    const [departureTime, setDepartureTime] = useState(new Date().toLocaleTimeString('pl-PL', {
        hour: '2-digit',
        minute: '2-digit',
    }));
    const [departureDate, setDepartureDate] = useState(new Date().toISOString().split('T')[0]);

    const activeStartStation = startStation || defaultStart;
    const activeEndStation = endStation || defaultEnd;

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
                    TransferDetails: (conn?.TransferDetails || []).map((detail: TransferDetail)=> ({
                        StationId: detail?.StationId,
                        ArrivalTime: detail?.ArrivalTime,
                        DepartureTime: detail?.DepartureTime,
                        TransferTime: detail?.TransferTime,
                    })) || [],
                    NumOfTransfers: conn?.NumOfTransfers,
                    StationIds: conn?.StationIds,
                })) || []

                console.log("Raw Data:", rawData);
                console.log("Parsed Data:", parsedData);

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
        if (!connection) return;
        setTrainConnections([]);
        if (connection.state === "Connected") {
            sendTrainConnectionsQuery(connection, activeStartStation, activeEndStation, departureDate, departureTime);
        }
        console.log("Start:", activeStartStation);
        console.log("End:", activeEndStation);
        console.log("Time:", departureTime);
        console.log("Date:", departureDate);
    }
    return (
        <>
            <form onSubmit={(e) => {
                e.preventDefault();
            }}>
                <div>
                    <label htmlFor="start_station">Choose a start station:</label>
                    <select name="start_station" id="start_station" value={activeStartStation}
                            onChange={(e) => setStartStation(e.target.value)}>
                        {trainStations && trainStations.map((station) => (
                            <option value={station.value}>{station.label}</option>
                        ))}
                    </select>
                </div>
                <div>
                    <label htmlFor="end_station">Choose a end station:</label>
                    <select name="end_station" id="end_station" value={activeEndStation}
                            onChange={(e) => setEndStation(e.target.value)}>
                        {trainStations && trainStations.map((station) => (
                            <option value={station.value}>{station.label}</option>
                        ))}
                    </select>
                </div>
                <div>
                    <input
                        defaultValue={departureTime}
                        onChange={(e) => {
                            setDepartureTime(e.target.value);
                        }}
                        type='time'
                        name='departureTime'
                        placeholder='Departure Time'
                    />
                </div>
                <div>
                    <input
                        defaultValue={departureDate}
                        onChange={(e) => {
                            setDepartureDate(e.target.value);
                        }}
                        type='date'
                        name='departureDate'
                        placeholder='Departure Date'
                    />
                </div>
                <button
                    disabled={
                        !departureTime && !departureDate
                    }
                    onClick={() => {
                        handleClick()
                    }}
                    type='submit'
                >
                    Search
                </button>
            </form>
        </>
    );
}

export default SearchConnection;