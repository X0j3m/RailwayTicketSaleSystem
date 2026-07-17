import {useSignalR} from "../hooks/useSignalR.ts";
import type {TrainStation, StationsMessage} from "../data/trainStations.ts";
import {useEffect, useState} from "react";

export function useTrainStations() {
    const { connection } = useSignalR();
    const [trainStations, setStations] = useState<TrainStation[]>([]);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveStations = (data: string) => {
            try {
                const parsedData: StationsMessage = JSON.parse(data);

                const cleanStations: TrainStation[] = parsedData.MessageItems.map((item) => ({
                    ...item,
                    city: item.city.replace(/\s+/g, ' ').trim(),
                    name: item.name.replace(/\s+/g, ' ').trim(),
                }));

                setStations(cleanStations);
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveStationsQueryResponse", handleReceiveStations);

        // Wysyłamy żądanie tylko wtedy, gdy połączenie jest już aktywne
        if (connection.state === "Connected") {
            connection.send("getAllStations")
                .catch(err => console.error("Error sending getAllStations: ", err));
        }

        return () => {
            connection.off("ReceiveStationsQueryResponse", handleReceiveStations);
        };
    }, [connection, connection?.state]);

    return trainStations;
}