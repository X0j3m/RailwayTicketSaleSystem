import {useEffect, useState} from "react";
import {useTrainStations} from "../../utils/UseTrainStations.ts";
import {sendTrainConnectionsQuery} from "../../utils/UseTrainConnections.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {TrainConnection, TrainConnectionSegment, TrainConnectionsMessage} from "../../data/trainConnection.ts";

function SearchConnectionComponent() {
    const {connection} = useSignalR();

    const [trainConnections, setTrainConnections] = useState<TrainConnection[]>([]);

    const trainStations = useTrainStations().map((station) =>
        ({value: station.id, label: station.name}));

    const defaultStart = trainStations[0]?.value || "";
    const defaultEnd = trainStations[1]?.value || trainStations[0]?.value || "";

    const [startStation, setStartStation] = useState<string>(defaultStart);
    const [endStation, setEndStation] = useState<string>(defaultEnd);
    const [departureTime, setDepartureTime] = useState(new Date().toLocaleTimeString('pl-PL', {
        hour: '2-digit',
        minute: '2-digit',
    }));
    const [departureDate, setDepartureDate] = useState(new Date().toISOString().split('T')[0]);

    const activeStartStation = startStation || trainStations[0]?.value || "";
    const activeEndStation = endStation || trainStations[1]?.value || trainStations[0]?.value || "";

    useEffect(() => {
        if (!connection) return;

        const handleReceiveConnections = (data: string) => {
            console.log("Trying to parse TrainConnectionsQueryResponse");
            try {
                const rawData: TrainConnectionsMessage = JSON.parse(data);

                const parsedData = rawData?.MessageItems?.map((item: TrainConnection) => ({
                    trainChanges: item?.trainChanges,
                    segments: (item?.segments || []).map((seg: TrainConnectionSegment) => ({
                        trainCompositionId: seg.trainCompositionId,
                        startStationId: seg.startStationId,
                        endStationId: seg.endStationId,
                        departureTime: seg.departureTime,
                        arrivalTime: seg.arrivalTime,
                        duration: seg.duration,
                    }))
                })) || [];
                setTrainConnections(parsedData);
                console.log(parsedData);
                console.log(rawData);
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        }

        connection.on("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections)

        return () => {
            connection.off("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections);
        };
    }, [trainConnections, connection, connection?.state])

    const handleClick = () => {
        if (!connection) return;

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

export default SearchConnectionComponent;