import {sendReservationCommand} from "../../utils/UseReservationCommand.ts";
import {useLocation} from "react-router-dom";
import type {SeatReservation} from "../../data/Reservation.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {TrainStation} from "../../data/trainStations.ts";

export interface TicketSummaryProps {
    trainStations: Array<TrainStation>;
}

function TicketSummaryComponent({trainStations}: TicketSummaryProps) {
    const {connection} = useSignalR();
    const location = useLocation();

    const seatReservations: SeatReservation[] = location.state?.seatReservations;

    function handleCheckoutClick() {
        if (!connection) return;

        sendReservationCommand(
            connection,
            seatReservations);
    }

    const formatDate = (isoString: string) => {
        return new Intl.DateTimeFormat("pl-PL", {
            day: "numeric",
            month: "long",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        }).format(new Date(isoString));
    };

    return (
        <>
            <h1>Ticket</h1>
            {seatReservations.map((seatReservation: SeatReservation) => {
                const fromStation = trainStations.find((s: TrainStation) => s.id === seatReservation.FromStationId)?.name;
                const toStation = trainStations.find((s: TrainStation) => s.id === seatReservation.ToStationId)?.name;

                return (
                    <>
                        <p>
                            {formatDate(seatReservation.DepartureTime)} {fromStation} &rarr; {formatDate(seatReservation.ArrivalTime)} {toStation} Car: {seatReservation.CarNumber} Seat: {seatReservation.SeatNumber}
                        </p>
                    </>
                );
            })}
            <button onClick={handleCheckoutClick}>Checkout</button>
        </>
    );
}

export default TicketSummaryComponent;