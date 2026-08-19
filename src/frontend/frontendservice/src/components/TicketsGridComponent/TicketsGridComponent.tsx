import type {TrainStation} from "../../data/trainStations.ts";
import {useEffect, useState} from "react";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {TicketInfo, TicketsMessage} from "../../data/Ticket.ts";
import {sendCancelReservationCommand} from "../../utils/UseReservationCommand.ts";

export interface TicketsGridComponentProps {
    trainStations: Array<TrainStation>;
}

function TicketsGridComponent({trainStations}: TicketsGridComponentProps) {
    const {connection} = useSignalR();
    const [tickets, setTickets] = useState<Array<TicketInfo>>([]);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveGetTickets = (data: string) => {
            try {
                const rawData: TicketsMessage = JSON.parse(data);
                const parsedData: TicketInfo[] = rawData?.MessageItems?.map((ticket: TicketInfo) => ({
                    TicketId: ticket.TicketId,
                    DepartureTime: ticket.DepartureTime,
                    ArrivalTime: ticket.ArrivalTime,
                    FromStationId: ticket.FromStationId,
                    ToStationId: ticket.ToStationId,
                })) || [];
                console.log(parsedData);
                if (parsedData) {
                    setTickets(parsedData);
                }
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveGetTicketsQueryResponse", handleReceiveGetTickets);

        return () => {
            connection.off("ReceiveGetTicketsQueryResponse", handleReceiveGetTickets);
        };
    }, [connection]);

    function handleDeleteButton(ticketId: string) {
        if (!connection) return;

        sendCancelReservationCommand(
            connection,
            ticketId,
        )
    }

    return (
        <>
            <table>
                {tickets.map((ticket: TicketInfo) => {
                    const fromStation = trainStations.find((s: TrainStation) => s.id === ticket.FromStationId)?.name;
                    const toStation = trainStations.find((s: TrainStation) => s.id === ticket.ToStationId)?.name;
                    return (
                        <tr>
                            <td>{ticket.DepartureTime}</td>
                            <td>{ticket.ArrivalTime}</td>
                            <td>{fromStation} &rarr; {toStation}</td>
                            <td>
                                <button
                                    onClick={() => handleDeleteButton(ticket.TicketId)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    );
                })}
            </table>
        </>
    );
}

export default TicketsGridComponent;