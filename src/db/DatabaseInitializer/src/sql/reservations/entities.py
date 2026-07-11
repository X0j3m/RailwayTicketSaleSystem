import uuid
from typing import Optional

from sqlmodel import Field, SQLModel


class Ticket(SQLModel, table=True):
    __tablename__ = "Tickets"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    departure_date: str
    departure_time: str
    departure_station_id: uuid.UUID
    arrival_date: str
    arrival_time: str
    arrival_station_id: uuid.UUID


class TicketSegments(SQLModel, table=True):
    __tablename__ = "TicketSegments"
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    ticket_id: Optional[uuid.UUID] = Field(foreign_key="Tickets.id")
    train_composition_id: uuid.UUID
    car_number: int
    seat_number: int
    start_station_id: uuid.UUID
    end_station_id: uuid.UUID
