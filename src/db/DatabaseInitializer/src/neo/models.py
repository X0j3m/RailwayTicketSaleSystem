from pydantic import BaseModel


class StopModel(BaseModel):
    id: str | None = None
    start_station_time: str | None = None
    arrival_time_minutes: int | None = None
    departure_time_minutes: int | None = None
    station_id: str | None = None
    train_composition_id: str | None = None


class LeadsToRelationModel(BaseModel):
    id: str | None = None
    from_stop_id: str | None = None
    to_stop_id: str | None = None
    time: int | None = None


class TransferRelationModel(BaseModel):
    id: str | None = None
    from_stop_id: str | None = None
    to_stop_id: str | None = None
    time: int | None = None
