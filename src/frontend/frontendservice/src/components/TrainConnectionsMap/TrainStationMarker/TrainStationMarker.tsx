import "./TrainStationMarker.css";
import 'leaflet/dist/leaflet.css';
import {CircleMarker, Tooltip} from "react-leaflet";
import type {TrainStation} from "../../../data/trainStations.ts";
import {useState} from "react";
import type {LeafletMouseEvent} from "leaflet";
import type {ContextMenuState} from "../TrainStationContextMenu/TrainStationContextMenu.tsx";

interface TrainStationMarkerProps {
    station: TrainStation;
    startStationId: string | undefined;
    endStationId: string | undefined;
    setContextMenu: React.Dispatch<React.SetStateAction<ContextMenuState | null>>;
}

function TrainStationMarker({station, startStationId, endStationId, setContextMenu}: TrainStationMarkerProps) {
    const position: [number, number] = [station.latitude, station.longitude];

    const [isOpen, setIsOpen] = useState(false);

    const handleMouseOver = () => {
        // console.log("Setting isOpen to %s", isOpen ? "false" : "true");
        setIsOpen(true);
    }

    const handleMouseOut = () => {
        // console.log("Setting isOpen to %s", isOpen ? "false" : "true");
        setIsOpen(false);
    }

    const handleContextMenu = (e: LeafletMouseEvent) => {
        e.originalEvent.preventDefault();

        const contextMenuState: ContextMenuState = {
            x: e.originalEvent.clientX,
            y: e.originalEvent.clientY,
            markerId: station.id,
        };

        setContextMenu(contextMenuState);
    }

    const markerColor = (): string => {
        if (startStationId == station.id) {
            return '#00ff19';
        }
        if (endStationId == station.id) {
            return '#ff0000';
        }
        return '#3b82f6';
    }

    return (
        <CircleMarker key={station.id}
                      center={position}
                      radius={10}
                      pathOptions={{
                          fillColor: markerColor(),
                          fillOpacity: 1,
                          color: '#ffffff',
                          weight: 5,
                          opacity: 0.6
                      }}
                      eventHandlers={
                          {
                              contextmenu: handleContextMenu,
                              mouseover: handleMouseOver,
                              mouseout: handleMouseOut
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
                        {station.name}
                    </span>
                </Tooltip>
            )}
        </CircleMarker>
    );
}

export default TrainStationMarker;