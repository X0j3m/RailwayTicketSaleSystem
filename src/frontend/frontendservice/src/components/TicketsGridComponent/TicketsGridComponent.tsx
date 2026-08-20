import type {TrainStation} from "../../data/trainStations.ts";
import {useEffect, useState} from "react";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {TicketInfo, TicketsMessage} from "../../data/Ticket.ts";
import {sendCancelReservationCommand} from "../../utils/UseReservationCommand.ts";
import type {ReservationResponse, Ticket} from "../../data/Reservation.ts";

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

    useEffect(() => {
        if (!connection) return;

        const handleReceiveCancelTicketReservation = (data: string) => {
            try {
                const rawData: ReservationResponse = JSON.parse(data);
                const parsedData: Ticket[] = rawData?.MessageItems?.map((ticket: Ticket) => ({
                    TicketId: ticket.TicketId,
                })) || [];

                const canceledTicketId = parsedData[0].TicketId;

                setTickets(tickets.filter((ticket: TicketInfo) => ticket.TicketId != canceledTicketId));
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveCancelTicketReservationCommandResponse", handleReceiveCancelTicketReservation);

        return () => {
            connection.off("ReceiveCancelTicketReservationCommandResponse", handleReceiveCancelTicketReservation);
        };
    }, [connection, tickets]);

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
                                    Cancel
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