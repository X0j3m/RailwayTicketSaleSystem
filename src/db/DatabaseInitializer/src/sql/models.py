from pydantic import BaseModel


class StationModel(BaseModel):
    id: str | None = None
    city: str | None = None
    name: str | None = None
    latitude: float | None = None
    longitude: float | None = None


class TrainModel(BaseModel):
    id: str | None = None
    type: str | None = None
    number: int | None = None
    velocity: int | None = None


class CarModel(BaseModel):
    id: str | None = None


class SeatModel(BaseModel):
    id: str | None = None
    car_id: str | None = None
    number: int | None = None
    x_pos: int | None = None
    y_pos: int | None = None


class TrainCompositionModel(BaseModel):
    id: str | None = None
    train_id: str | None = None


class TrainCompositionCarModel(BaseModel):
    composition_id: str | None = None
    car_id: str | None = None
    car_number: int | None = None
