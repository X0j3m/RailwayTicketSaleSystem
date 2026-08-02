import {useEffect} from "react";
import type {
    TrainConnection, TrainConnectionQuery,
    TrainConnectionsMessage,
    TransferDetail,
    Transit
} from "../../data/trainConnection.ts";
import type {Page} from "../../data/Page.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";
import {sendTrainConnectionsQuery} from "../../utils/UseTrainConnections.ts";

export interface ConnectionsSearchHandlerProps {
    setTrainConnections: React.Dispatch<React.SetStateAction<TrainConnection[] | null>>;
    setTrainConnectionsMetadata: React.Dispatch<React.SetStateAction<Page | null>>;
    trainConnectionsQuery: TrainConnectionQuery | null;
}

function ConnectionsSearchHandler({
                                      setTrainConnections,
                                      setTrainConnectionsMetadata,
                                      trainConnectionsQuery
                                  }: ConnectionsSearchHandlerProps) {
    const {connection} = useSignalR();

    useEffect(() => {
        if (!connection || !trainConnectionsQuery) return;
        setTrainConnections(null);
        if (connection.state === "Connected") {
            sendTrainConnectionsQuery(
                connection,
                trainConnectionsQuery
            );
        }
    }, [trainConnectionsQuery]);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveConnections = (data: string) => {
            try {
                const rawData: TrainConnectionsMessage = JSON.parse(data);

                const parsedData: TrainConnection[] = rawData?.MessageItems?.map((conn: TrainConnection) => ({
                    DepartureTime: conn?.DepartureTime,
                    ArrivalTime: conn?.ArrivalTime,
                    TotalTripTime: conn?.TotalTripTime,
                    Transits: (conn?.Transits || []).map((transit: Transit) => ({
                        FromStationId: transit?.FromStationId,
                        ToStationId: transit?.ToStationId,
                        ArrivalTime: transit?.ArrivalTime,
                        DepartureTime: transit?.DepartureTime,
                        TrainCompositionId: transit?.TrainCompositionId
                    })) || [],
                    TransferDetails: (conn?.TransferDetails || []).map((detail: TransferDetail) => ({
                        StationId: detail?.StationId,
                        ArrivalTime: detail?.ArrivalTime,
                        DepartureTime: detail?.DepartureTime,
                        TransferTime: detail?.TransferTime,
                    })) || [],
                    NumOfTransfers: conn?.NumOfTransfers,
                    StationIds: conn?.StationIds,
                    TrainCompositionIds: conn?.TrainCompositionIds
                })) || [];

                const page: Page = {
                    NumberOfPages: rawData.NumberOfPages,
                    PageNumber: rawData.PageNumber,
                    PageSize: rawData.PageSize
                }

                if (setTrainConnections && setTrainConnectionsMetadata) {
                    setTrainConnections(parsedData);
                    setTrainConnections(parsedData);
                    setTrainConnectionsMetadata(page);
                }

            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections);

        return () => {
            connection.off("ReceiveTrainConnectionsQueryResponse", handleReceiveConnections);
        };
    }, [connection, setTrainConnections, setTrainConnectionsMetadata]);

    return <></>
}

export default ConnectionsSearchHandler;