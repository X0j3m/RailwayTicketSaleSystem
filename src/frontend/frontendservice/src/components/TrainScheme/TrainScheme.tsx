import {useSignalR} from "../../hooks/useSignalR.ts";
import {useEffect} from "react";
import type {Car, TrainComposition, TrainCompositionsMessage} from "../../data/TrainComposition.ts";

function TrainScheme() {
    const {connection} = useSignalR();

    useEffect(() => {
        if (!connection) return;

        const handleReceiveAvailableSeats = (data: string) => {
            try {
                const rawData: TrainCompositionsMessage = JSON.parse(data);

                const parsedData: TrainComposition[] = rawData?.MessageItems?.map((composition: TrainComposition) => ({
                    TrainCompositionId: composition.TrainCompositionId,
                    StartStationId: composition.StartStationId,
                    EndStationId: composition.EndStationId,
                    TrainNumber: composition.TrainNumber,
                    TrainType: composition.TrainType,
                    Cars: (composition?.Cars || []).map((car: Car) => ({
                        Number: car.Number,
                        Seats: car.Seats
                    })) || [],
                })) || [];

                console.log(parsedData);

            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveAvailableSeatsQueryResponse", handleReceiveAvailableSeats);

        return () => {
            connection.off("ReceiveAvailableSeatsQueryResponse", handleReceiveAvailableSeats);
        }
    }, [connection]);

    return (
        <div>
            TrainScheme
        </div>
    );
}

export default TrainScheme;