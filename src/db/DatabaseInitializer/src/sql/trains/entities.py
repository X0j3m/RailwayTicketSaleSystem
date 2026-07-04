import uuid

from typing import List, Optional
from sqlmodel import Field, SQLModel, Relationship


class TrainStation(SQLModel, table=True):
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    city: str
    name: str
    latitude: float
    longitude: float


class Train(SQLModel, table=True):
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    type: str
    number: int
    compositions: List["TrainComposition"] = Relationship(back_populates="train")


class TrainCompositionCar(SQLModel, table=True):
    composition_id: Optional[uuid.UUID] = Field(default=None, foreign_key="traincomposition.id", primary_key=True)
    car_id: Optional[uuid.UUID] = Field(default=None, foreign_key="car.id", primary_key=True)
    car_number: int


class TrainComposition(SQLModel, table=True):
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    train_id: Optional[uuid.UUID] = Field(default=None, foreign_key="train.id")
    train: Optional[Train] = Relationship(back_populates="compositions")

    cars: List["Car"] = Relationship(
        back_populates="compositions",
        link_model=TrainCompositionCar
    )


class Car(SQLModel, table=True):
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
    id: uuid.UUID | None = Field(default=None,
                                 primary_key=True,
                                 index=True,
                                 nullable=False)
    car_id: Optional[uuid.UUID] = Field(default=None, foreign_key="car.id")
    car: Optional[Car] = Relationship(back_populates="seats")
    number: int
    x_pos: int
    y_pos: int
