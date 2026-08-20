import type {TrainStation} from "../../../data/trainStations.ts";
import type {Dispatch, SetStateAction} from "react";

export interface ContextMenuState {
    x: number;
    y: number;
    markerId: string;
}

export type ContextMenuStateTuple = [
    ContextMenuState,
    Dispatch<SetStateAction<ContextMenuState | null>>
];

interface TrainStationContextMenuProps {
    contextMenuState: ContextMenuStateTuple;
    trainStation: TrainStation | undefined
    setStartStation: React.Dispatch<React.SetStateAction<string | undefined>>;
    setEndStation: React.Dispatch<React.SetStateAction<string | undefined>>;
}

function TrainStationContextMenu({
                                     contextMenuState,
                                     trainStation,
                                     setStartStation,
                                     setEndStation
                                 }: TrainStationContextMenuProps) {
    const [contextMenu, setContextMenu] = contextMenuState;

    return <div
        style={{
            position: 'fixed',
            top: contextMenu.y,
            left: contextMenu.x,
            zIndex: 1000,
            backgroundColor: 'white',
            border: '1px solid #ccc',
            borderRadius: '4px',
            boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
            padding: '4px 0',
            minWidth: '140px',
        }}
        onClick={(e) => e.stopPropagation()}
    >
        {trainStation?.name}<br/>
        <button
            onClick={() => {
                setStartStation(trainStation?.id);
                setContextMenu(null);
            }}>
            Start
        </button>
        <button
            onClick={() => {
                setEndStation(trainStation?.id);
                setContextMenu(null);
            }}>
            End
        </button>
    </div>
}

export default TrainStationContextMenu;