import {useSignalR} from "../hooks/useSignalR.ts";
import type {TrainStation, StationsMessage} from "../data/trainStations.ts";
import {useEffect, useState} from "react";

export function useTrainStations() {
    const {connection} = useSignalR();
    const [trainStations, setStations] = useState<TrainStation[]>([]);

    useEffect(() => {
        if (trainStations.length != 0) return;
        if (!connection) return;

        const handleReceiveStations = (data: string) => {
            try {
                const parsedData: StationsMessage = JSON.parse(data);

                const stations: TrainStation[] = parsedData?.MessageItems?.map((station: TrainStation) => ({
                    id: station.id,
                    city: station.city,
                    name: station.name,
                    latitude: station.latitude,
                    longitude: station.longitude
                }));

                setStations(stations);
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveStationsQueryResponse", handleReceiveStations);

        if (connection.state === "Connected") {
            const args = {
                connectionId: connection?.connectionId
            }
            connection.send("getAllStations", args)
                .catch(err => console.error("Error sending getAllStations: ", err));
        }

        return () => {
            connection.off("ReceiveStationsQueryResponse", handleReceiveStations);
        };
    }, [connection, connection?.state, trainStations.length]);

    return trainStations.sort((a, b) =>
        a.name.localeCompare(b.name)
    );
}