import type {TrainConnection} from "../../../data/trainConnection.ts";
import type {TrainStation} from "../../../data/trainStations.ts";
import {Polyline} from "react-leaflet";
import type {LatLngExpression} from "leaflet";

const colors: string[] = [
    '#047857',
    '#1D4ED8',
    '#F97316',
    '#7C3AED',
    '#E11D48',
]

interface RouteLineTuple {
    from: string;
    to: string;
    transitNumber: number;
}

interface TrainConnectionRouteLineProps {
    trainStations: TrainStation[];
    trainConnection: TrainConnection;
}

function TrainConnectionRouteLine({trainStations, trainConnection}: TrainConnectionRouteLineProps) {
    const routeLineTuples: RouteLineTuple[] = [];

    let transitNumber = 0;

    if (trainConnection.StationIds == undefined) {
        return
    } else {
        for (let i = 0; i < trainConnection.StationIds.length - 1; i += 1) {
            const first = trainConnection.StationIds[i];
            const second = trainConnection.StationIds[i + 1];

            if (first == second) {
                transitNumber++;
                continue;
            }

            const t: RouteLineTuple = {
                from: first,
                to: second,
                transitNumber: transitNumber
            }

            routeLineTuples.push(t)
        }
    }

    return (
        <>
            {
                routeLineTuples.map((routeLineTuple: RouteLineTuple) => {
                        const fromStation = trainStations.find(s => s.id == routeLineTuple.from);
                        const toStation = trainStations.find(s => s.id == routeLineTuple.to);

                        const pointA: LatLngExpression = [fromStation?.latitude ?? 0, fromStation?.longitude ?? 0];
                        const pointB: LatLngExpression = [toStation?.latitude ?? 0, toStation?.longitude ?? 0];

                        const col = colors[routeLineTuple.transitNumber % colors.length];

                        return <Polyline
                            positions={[pointA, pointB]}
                            pathOptions={{
                                color: col,
                                weight: 5,
                            }}
                        />
                    }
                )
            }
        </>
    );
}

export default TrainConnectionRouteLine