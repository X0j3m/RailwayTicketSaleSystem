import './TrainConnectionsMap.css';
import {MapContainer, TileLayer, useMapEvents} from 'react-leaflet';
import L from 'leaflet';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import TrainStationMarker from "./TrainStationMarker/TrainStationMarker.tsx";
import type {TrainStation} from "../../data/trainStations.ts";
import {useState} from "react";
import TrainStationContextMenu, {type ContextMenuState} from "./TrainStationContextMenu/TrainStationContextMenu.tsx";
import type {EndStationState, StartStationState} from "../TrainConnections/TrainConnections.tsx";
import type {TrainConnection} from "../../data/trainConnection.ts";
import TrainConnectionRouteLine from "./TrainConnectionRouteLine/TrainConnectionRouteLine.tsx";

const mapCenter: [number, number] = [52.0689, 19.4797];
const mapBounds = L.latLngBounds(
    [49.002, 14.122],
    [54.836, 24.145]
);

L.Icon.Default.mergeOptions({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
});

function MapClickHandler({onMapClick}: { onMapClick: (state: ContextMenuState | null) => void }) {
    useMapEvents({
        click() {
            onMapClick(null);
        },
        movestart() {
            onMapClick(null);
        }
    });

    return null;
}

interface TrainConnectionsMapProps {
    trainStations: TrainStation[];
    startStationState: StartStationState;
    endStationState: EndStationState;
    selectedTrainConnection: TrainConnection | null;
}

function TrainConnectionsMap({
                                 trainStations,
                                 startStationState,
                                 endStationState,
                                 selectedTrainConnection
                             }: TrainConnectionsMapProps) {
    const [contextMenu, setContextMenu] = useState<ContextMenuState | null>(null);
    const [startStationId, setStartStationId] = startStationState;
    const [endStationId, setEndStationId] = endStationState;

    return (
        <>
            <MapContainer
                center={mapCenter}
                zoom={7}
                minZoom={7}
                maxZoom={18}
                maxBounds={mapBounds}
                maxBoundsViscosity={1.0}
                style={{height: "80vh", width: "100%"}}
            >
                <MapClickHandler onMapClick={setContextMenu}/>
                <TileLayer
                    url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>'
                />

                {trainStations && trainStations.map((station) => (
                    <TrainStationMarker
                        key={station.id}
                        station={station}
                        startStationId={startStationId}
                        endStationId={endStationId}
                        setContextMenu={setContextMenu}
                    />
                ))}

            {selectedTrainConnection && <TrainConnectionRouteLine
                trainStations={trainStations}
                trainConnection={selectedTrainConnection}
            />}
            </MapContainer>

            {contextMenu && <TrainStationContextMenu
                contextMenuState={[contextMenu, setContextMenu]}
                trainStation={trainStations.find(s => s.id == contextMenu.markerId)}
                setStartStation={setStartStationId}
                setEndStation={setEndStationId}
            />}
        </>
    );
}

export default TrainConnectionsMap