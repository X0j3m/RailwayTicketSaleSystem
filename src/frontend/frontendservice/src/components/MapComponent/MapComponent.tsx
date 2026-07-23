import './MapComponent.css';
import {MapContainer, TileLayer} from 'react-leaflet';
import L from 'leaflet';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import TrainStationMarkerComponent from "../TrainStationMarkerComponent/TrainStationMarkerComponent.tsx";
import {useTrainStations} from "../../utils/UseTrainStations.ts";

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


function MapComponent() {
    const trainStations = useTrainStations();

    return (
        <MapContainer
            center={mapCenter}
            zoom={7}
            minZoom={7}
            maxZoom={18}
            maxBounds={mapBounds}
            maxBoundsViscosity={1.0}
            style={{height: "80vh", width: "50%"}}
        >
            <TileLayer
                url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>'
            />

            {trainStations && trainStations.map((station) => (
                <TrainStationMarkerComponent
                    key={station.id}
                    station={station}
                />
            ))}
        </MapContainer>
    );
}

export default MapComponent