import './TrainScheme.css';
import {useSignalR} from "../../hooks/useSignalR.ts";
import {useEffect, useState} from "react";
import type {Car, Seat, TrainComposition, TrainCompositionsMessage} from "../../data/TrainComposition.ts";
import type {TrainStation} from "../../data/trainStations.ts";
import type {SeatReservation} from "../../data/Reservation.ts";
import {sendReservationCommand} from "../../utils/UseReservationCommand.ts";
import type {TrainConnection, TrainConnectionQuery} from "../../data/trainConnection.ts";
import {toDateObjString, addToDateObjString} from "../../utils/TimeCalculator.ts";

interface SelectedSeat {
    Car: number | null;
    Seat: number | null;
}

export interface TrainSchemeProps {
    trainStations: Array<TrainStation>;
    trainConnection: TrainConnection | null;
    trainConnectionQuery: TrainConnectionQuery | null;
}

function TrainScheme({trainStations, trainConnection, trainConnectionQuery}: TrainSchemeProps) {
    const {connection} = useSignalR();
    const [trainCompositions, setTrainCompositions] = useState<TrainComposition[]>([]);
    const [showed, setShowed] = useState(false);

    const [activeTrainComposition, setActiveTrainComposition] = useState<TrainComposition | null>(null);
    const [activeCarNumber, setActiveCarNumber] = useState<number | null>(null);
    const [selectedSeats, setSelectedSeats] = useState<Map<string, SelectedSeat | null>>(new Map());

    useEffect(() => {
        console.log(trainConnection);
        console.log(trainConnectionQuery);
    }, [trainConnection, trainConnectionQuery]);

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

                if (parsedData.length > 0) {
                    setShowed(true);
                    setTrainCompositions(parsedData);
                    setActiveTrainComposition(parsedData[0]);
                    setActiveCarNumber(0);

                    const selectedSeatsMap = new Map<string, SelectedSeat | null>();
                    parsedData.forEach((composition: TrainComposition) => {
                        selectedSeatsMap.set(composition.TrainCompositionId, null);
                    });

                    console.log(parsedData);

                    
                    setSelectedSeats(selectedSeatsMap);
                }

            } catch (error) {
                console.error("Error parsing stations:", error);
            }
        };

        connection.on("ReceiveAvailableSeatsQueryResponse", handleReceiveAvailableSeats);

        return () => {
            connection.off("ReceiveAvailableSeatsQueryResponse", handleReceiveAvailableSeats);
        };
    }, [connection]);

    function onTrainCompositionClick(composition: TrainComposition) {
        if (activeTrainComposition == composition) {
            return;
        }

        setActiveTrainComposition(composition);
        setActiveCarNumber(0);
    }

    function onSeatClick(trainCompositionId: string, carNumber: number, seatNumber: number) {
        const selectedSeat: SelectedSeat = {
            Car: carNumber + 1,
            Seat: seatNumber
        };

        setSelectedSeats(prevMap => {
            const newMap = new Map(prevMap);
            newMap.set(trainCompositionId, selectedSeat);
            return newMap;
        });
    }

    function isCheckoutDisabled() {
        if (!selectedSeats || selectedSeats.size === 0) return true;

        for (const seat of selectedSeats.values()) {
            if (seat == null) {
                return true;
            }
        }
        return false;
    }



    function handleCheckoutClick() {
        if (!connection || !trainConnectionQuery || !trainConnection || !trainConnection.DepartureTime || !trainConnection.Transits) return;

        const seatReservations: SeatReservation[] = [];

        let departureTime = toDateObjString(trainConnectionQuery.DepartureDate, trainConnection.DepartureTime);

        for (let i = 0; i < trainConnection.Transits.length; i++) {
            const trainComposition = trainCompositions[i];

            const arrivalTime = addToDateObjString(departureTime, trainConnection.Transits[i].TravelTime)

            const seatReservation: SeatReservation = {
                TrainComposition: trainComposition.TrainCompositionId,
                SegmentNumber: i,
                CarNumber: selectedSeats.get(trainComposition.TrainCompositionId)?.Car ?? -1,
                SeatNumber: selectedSeats.get(trainComposition.TrainCompositionId)?.Seat ?? -1,
                FromStationId: trainComposition.StartStationId,
                ToStationId: trainComposition.EndStationId,
                DepartureTime: departureTime,
                ArrivalTime: arrivalTime
            }

            if (trainConnection.TransferDetails && trainConnection.TransferDetails.length > i) {
                departureTime = addToDateObjString(arrivalTime, trainConnection.TransferDetails[i].TransferTime);
            }

            seatReservations.push(seatReservation);
        }

        sendReservationCommand(
            connection,
            seatReservations);
    }

    return (
        <>
            {showed &&
                <div>
                    <button
                        onClick={() => setShowed(false)}>
                        X
                    </button>
                    <br/>
                    <br/>
                    <br/>
                    <br/>

                    {trainCompositions.map((composition: TrainComposition) => {
                        const startStation = trainStations.find(s => s.id === composition.StartStationId);
                        const endStation = trainStations.find(s => s.id === composition.EndStationId);
                        const seatInfo = selectedSeats?.get(composition.TrainCompositionId);

                        return (
                            <div key={composition.TrainCompositionId}>
                                <button onClick={() => onTrainCompositionClick(composition)}>
                                    {startStation?.name} &rarr; {endStation?.name}
                                </button>
                                {seatInfo && (
                                    <span>
                                        Car: {seatInfo.Car} Seat: {seatInfo.Seat}
                                    </span>
                                )}
                                <br/>
                            </div>
                        );
                    })}

                    <h2>
                        Active train composition:
                        &nbsp;
                        {activeTrainComposition &&
                            <>
                                <span>
                                    {trainStations.find(s => s.id === activeTrainComposition.StartStationId)?.name}
                                </span>
                                &nbsp;&rarr;&nbsp;
                                <span>
                                    {trainStations.find(s => s.id === activeTrainComposition.EndStationId)?.name}
                                </span>
                            </>
                        }
                    </h2>

                    <br/>
                    {activeTrainComposition && <button className="locomotive" disabled>Train</button>}
                    {activeTrainComposition &&
                        activeTrainComposition.Cars.map((car: Car) => (
                            <button
                                key={car.Number}
                                className={activeCarNumber === car.Number - 1 ? "car active" : "car"}
                                onClick={() => setActiveCarNumber(car.Number - 1)}>
                                Car {car.Number}
                            </button>
                        ))
                    }

                    <br/>
                    <div className="seat-map">
                        {activeCarNumber !== null && activeTrainComposition && (() => {
                            const currentSelected = selectedSeats.get(activeTrainComposition.TrainCompositionId);

                            return activeTrainComposition.Cars[activeCarNumber]?.Seats.map((seat: Seat) => {
                                const isSelected = currentSelected?.Car === activeCarNumber + 1 && currentSelected?.Seat === seat.Number;

                                const seatStatusClass = seat.Occupied
                                    ? "occupied"
                                    : (isSelected ? "selected" : "available");

                                return (
                                    <button
                                        key={seat.Number}
                                        className={`seat ${seatStatusClass}`}
                                        style={{
                                            gridColumn: seat.XPosition + 1,
                                            gridRow: seat.YPosition >= 2 ? seat.YPosition + 2 : seat.YPosition + 1
                                        }}
                                        disabled={seat.Occupied}
                                        onClick={() => onSeatClick(activeTrainComposition.TrainCompositionId, activeCarNumber, seat.Number)}
                                    >
                                        {seat.Number}
                                    </button>
                                );
                            });
                        })()}
                    </div>

                    {activeTrainComposition && (
                        <button
                            disabled={isCheckoutDisabled()}
                            onClick={handleCheckoutClick}>
                            Checkout
                        </button>
                    )}
                </div>
            }
        </>
    );
}

export default TrainScheme;