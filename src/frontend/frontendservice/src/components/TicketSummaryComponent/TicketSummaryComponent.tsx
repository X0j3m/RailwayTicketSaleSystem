import {sendReservationCommand} from "../../utils/UseReservationCommand.ts";
import {useLocation, useNavigate} from "react-router-dom";
import type {SeatReservation} from "../../data/Reservation.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";
import type {TrainStation} from "../../data/trainStations.ts";
import {useState} from "react";

export interface TicketSummaryProps {
    trainStations: Array<TrainStation>;
}

function TicketSummaryComponent({trainStations}: TicketSummaryProps) {
    const {connection} = useSignalR();
    const location = useLocation();
    const navigate = useNavigate();

    const seatReservations: SeatReservation[] = location.state?.seatReservations;
    const [email, setEmail] = useState<string>("");

    function handleCheckoutClick() {
        if (!connection || !email) return;

        sendReservationCommand(
            connection,
            email,
            seatReservations);

        navigate("/checkout-info", {
            state: {
                email
            }
        });
    }

    const formatDate = (isoDateString: string) => {
        return new Intl.DateTimeFormat("pl-PL", {
            day: "numeric",
            month: "long",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        }).format(new Date(isoDateString));
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
            <div>
                <input
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    type="email"
                    name="email"/>
            </div>
            <button
                onClick={handleCheckoutClick}
                disabled={!email}>
                Checkout
            </button>
        </>
    );
}

export default TicketSummaryComponent;