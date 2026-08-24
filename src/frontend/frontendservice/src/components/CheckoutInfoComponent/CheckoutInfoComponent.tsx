import {useSignalR} from "../../hooks/useSignalR.ts";
import {useEffect, useState} from "react";
import type {ReservationResponse, Ticket} from "../../data/Reservation.ts";
import {useNavigate} from "react-router-dom";

function CheckoutInfoComponent() {
    const {connection} = useSignalR();
    const navigate = useNavigate();

    const [ticket, setTicket] = useState<Ticket | null>(null);

    useEffect(() => {
        if (!connection) return;

        const handleReceiveTicketReservation = (data: string) => {
            try {
                const rawData: ReservationResponse = JSON.parse(data);

                const parsedData: Ticket[] = rawData?.MessageItems?.map((ticket: Ticket) => ({
                    TicketId: ticket.TicketId
                })) || [];

                if (parsedData.length > 0) {
                    setTicket(parsedData[0]);
                }
            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveTicketReservationCommandResponse", handleReceiveTicketReservation);

        return () => {
            connection.off("ReceiveTicketReservationCommandResponse", handleReceiveTicketReservation);
        };
    }, [connection, ticket]);

    function handleHomeClick() {
        navigate("/");
    }

    return (
        <>
            {
                ticket == null ?
                    <p>Processing</p>
                    :
                    ticket.TicketId == "00000000-0000-0000-0000-000000000000"
                        ?
                        <p>Fail</p>
                        :
                        <p>Success</p>
            }
            <button
                type="button"
                disabled={ticket == null}
                onClick={handleHomeClick}>
                Go Home
            </button>
        </>
    );
}

export default CheckoutInfoComponent;