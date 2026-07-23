import "./TrainStationMarkerComponent.css";
import 'leaflet/dist/leaflet.css';
import {CircleMarker, Tooltip} from "react-leaflet";
import type {TrainStation} from "../../data/trainStations.ts";
import {useState} from "react";
import type {LeafletMouseEvent, CircleMarker as LeafletCircleMarker} from "leaflet";

interface TrainStationMarkerProps {
    station: TrainStation;
}

function TrainStationMarkerComponent({station}: TrainStationMarkerProps) {
    const position: [number, number] = [station.latitude, station.longitude];

    const [isOpen, setIsOpen] = useState(false);

    const handleMouseDown = () => {
        console.log("Setting isOpen to %s", isOpen ? "false" : "true");
        setIsOpen(prev => !prev);
    }

    const handleContextMenu = (e: LeafletMouseEvent) => {
        e.originalEvent.preventDefault();

        const marker = e.target as LeafletCircleMarker;

        marker.setStyle({color: 'green', fillColor: 'green'})
    }

    return (
        <CircleMarker center={position}
                      radius={10}
                      pathOptions={{
                          fillColor: '#3b82f6',
                          fillOpacity: 1,
                          color: '#93c5fd',
                          weight: 5,
                          opacity: 0.6
                      }}
                      eventHandlers={
                          {
                              contextmenu: handleContextMenu,
                              mousedown: handleMouseDown
                          }
                      }>

            {isOpen && (
                <Tooltip
                    direction="top"
                    offset={[0, -5]}
                    opacity={0.9}
                    permanent={true}
                >
                    <span>
                        {station.id}
                        <br/>
                        {station.name}
                        <br/>
                        {station.latitude}
                        <br/>
                        {station.longitude}
                    </span>
                </Tooltip>
            )}
        </CircleMarker>
    );
}

export default TrainStationMarkerComponent;