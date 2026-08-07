import {useState} from "react";
import {useSignalR} from "../../../hooks/useSignalR.ts";
import type {
    TrainConnection,
    TrainConnectionQuery
} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import type {EndStationState, StartStationState} from "../TrainConnections.tsx";

export interface SearchConnectionProps {
    setTrainConnectionsQuery: React.Dispatch<React.SetStateAction<TrainConnectionQuery | null>>;
    trainStations: Array<TrainStation>;
    setTrainConnections: React.Dispatch<React.SetStateAction<TrainConnection[] | null>>;
    startStationState: StartStationState;
    endStationState: EndStationState;
}

function SearchBar({
                       setTrainConnectionsQuery,
                       trainStations,
                       setTrainConnections,
                       startStationState,
                       endStationState
                   }: SearchConnectionProps) {
    const today: string = new Date().toISOString().split('T')[0];

    const {connection} = useSignalR();

    const [minTime, setMinTime] = useState<string>(new Date().toTimeString().slice(0, 5));

    const [startStationId,] = startStationState;
    const [endStationId,] = endStationState;

    const [departureTime, setDepartureTime] = useState(new Date().toLocaleTimeString('pl-PL', {
        hour: '2-digit',
        minute: '2-digit',
    }));
    const [departureDate, setDepartureDate] = useState(today);

    const handleSubmitClick = () => {
        if (!connection || !startStationId || !endStationId) return;

        setTrainConnections(null);

        const trainConnectionsQuery: TrainConnectionQuery = {
            StartStation: startStationId,
            EndStation: endStationId,
            DepartureDate: departureDate,
            DepartureTime: departureTime,
            PageNumber: 0,
            PageSize: 8
        }
        setTrainConnectionsQuery(trainConnectionsQuery);
    };

    const handleTimeChange = (selectedTime: string) => {
        if (selectedTime >= minTime) {
            setDepartureTime(selectedTime);
        }
    }

    const handleDateChange = (selectedDate: string) => {
        const newMinTime: string = selectedDate == today ? new Date().toTimeString().slice(0, 5) : "00:00";
        setDepartureDate(selectedDate);
        setMinTime(newMinTime);
        if (departureTime < newMinTime) {
            setDepartureTime(newMinTime);
        }
    }

    const startStationName = trainStations.find(s => s.id === startStationId)?.name ?? 'Start station';
    const endStationName = trainStations.find(s => s.id === endStationId)?.name ?? 'End station';

    return (
        <form onSubmit={(e) => {
            e.preventDefault();
            handleSubmitClick();
        }}>
            <span>{startStationName}</span>
            &nbsp;
            <span>{endStationName}</span>
            &nbsp;
            <input
                value={departureTime}
                onChange={(e) => handleTimeChange(e.target.value)}
                type='time'
                min={minTime}
                name='departureTime'
            />
            &nbsp;
            <input
                value={departureDate}
                onChange={(e) => handleDateChange(e.target.value)}
                type='date'
                min={today}
                name='departureDate'/>
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