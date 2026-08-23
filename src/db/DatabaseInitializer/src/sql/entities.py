import uuid
from typing import List, Optional
from datetime import datetime
from sqlmodel import Field, SQLModel, Relationship
from sqlalchemy import NVARCHAR, Column


class TrainStation(SQLModel, table=True):
    __tablename__ = "TrainStations"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    city: str = Field(sa_column=Column(NVARCHAR(255)))
    name: str = Field(sa_column=Column(NVARCHAR(255)))
    latitude: float
    longitude: float


class Train(SQLModel, table=True):
    __tablename__ = "Trains"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    type: str = Field(sa_column=Column(NVARCHAR(255)))
    number: int
    compositions: List["TrainComposition"] = Relationship(back_populates="train")


class TrainCompositionCar(SQLModel, table=True):
    __tablename__ = "TrainCompositions_Cars"
    composition_id: Optional[uuid.UUID] = Field(default=None, foreign_key="TrainCompositions.id", primary_key=True)
    car_id: Optional[uuid.UUID] = Field(default=None, foreign_key="Cars.id", primary_key=True)
    car_number: int


class TrainComposition(SQLModel, table=True):
    __tablename__ = "TrainCompositions"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    train_id: Optional[uuid.UUID] = Field(default=None, foreign_key="Trains.id")
    train: Optional[Train] = Relationship(back_populates="compositions")

    cars: List["Car"] = Relationship(
        back_populates="compositions",
        link_model=TrainCompositionCar
    )


class Car(SQLModel, table=True):
    __tablename__ = "Cars"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    seats: List["Seat"] = Relationship(back_populates="car")

    compositions: List["TrainComposition"] = Relationship(
        back_populates="cars",
        link_model=TrainCompositionCar
    )


class Seat(SQLModel, table=True):
    __tablename__ = "Seats"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    car_id: Optional[uuid.UUID] = Field(default=None, foreign_key="Cars.id")
    car: Optional[Car] = Relationship(back_populates="seats")
    number: int
    x_pos: int
    y_pos: int


class Ticket(SQLModel, table=True):
    __tablename__ = "Tickets"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    email: str
    status: str


class TicketSegments(SQLModel, table=True):
    __tablename__ = "TicketSegments"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    ticket_id: Optional[uuid.UUID] = Field(foreign_key="Tickets.id")
    segment_number: int
    train_composition_id: uuid.UUID
    car_number: int
    seat_number: int
    departure_time: datetime
    arrival_time: datetime
    start_station_id: uuid.UUID
    end_station_id: uuid.UUID