import {CircleMarker, Tooltip} from "react-leaflet";
import type {TrainStation} from "../../data/trainStations.ts";

interface TrainStationMarkerProps {
    station: TrainStation;
}

function TrainStationMarkerComponent({station}: TrainStationMarkerProps) {
    const position: [number, number] = [station.latitude, station.longitude];
    return (
        <CircleMarker center={position}
                      radius={10}
                      pathOptions={{
                          fillColor: '#3b82f6',     // Niebieski środek
                          fillOpacity: 1,
                          color: '#93c5fd',         // Jasnoniebieska poświata
                          weight: 6,                // Grube obramowanie
                          opacity: 0.6              // Półprzezroczysta poświata
                      }}>
            <Tooltip
                direction="top"     // Gdzie ma się pojawić dymek względem punktu ('top', 'bottom', 'left', 'right', 'center')
                offset={[0, -5]}    // Przesunięcie dymku w pikselach [x, y] (przydatne, by nie nachodził na marker)
                opacity={0.9}       // Przezroczystość dymku
                permanent={false}   // false = tylko po najechaniu; true = widoczny cały czas
            >
                <span>{station.name}</span>
            </Tooltip>
        </CircleMarker>
    );
}

export default TrainStationMarkerComponent;