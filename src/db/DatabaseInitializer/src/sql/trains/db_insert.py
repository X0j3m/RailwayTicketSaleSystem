from sql.db_handler import DatabaseConnection, initialize_database, drop_database, bulk_save_entities
from sql.trains.entities import TrainStation, Train, TrainComposition, Car, Seat, TrainCompositionCar
from sql.trains.models import TrainModel, StationModel, TrainCompositionModel, CarModel, SeatModel, \
    TrainCompositionCarModel
from utils.json_handler import open_json_file


def insert_trains(db_connection: DatabaseConnection):
    db_name = "trains"
    drop_database(db_connection, db_name)
    initialize_database(db_connection, db_name, models_module="sql.trains.entities")

    stations = open_json_file("train_stations", StationModel)
    stations = [TrainStation(
        id=station.id,
        city=station.city,
        name=station.name,
        latitude=station.latitude,
        longitude=station.longitude,
    ) for station in stations]
    bulk_save_entities(db_connection, db_name, stations)

    trains = open_json_file("trains", TrainModel)
    trains = [Train(
        id=train.id,
        type=train.type,
        number=train.number,
    ) for train in trains]

    bulk_save_entities(db_connection, db_name, trains)

    train_compositions = open_json_file("compositions", TrainCompositionModel)
    train_compositions = [TrainComposition(
        id=train_composition.id,
        train_id=train_composition.train_id
    ) for train_composition in train_compositions]
    bulk_save_entities(db_connection, db_name, train_compositions)

    cars = open_json_file("cars", CarModel)
    cars = [Car(
        id=car.id
    ) for car in cars]
    bulk_save_entities(db_connection, db_name, cars)

    seats = open_json_file("seats", SeatModel)
    seats = [Seat(
        id=seat.id,
        car_id=seat.car_id,
        number=seat.number,
        x_pos=seat.x_pos,
        y_pos=seat.y_pos
    ) for seat in seats]
    bulk_save_entities(db_connection, db_name, seats)

    train_compositions_cars = open_json_file("compositions_cars", TrainCompositionCarModel)
    train_compositions_cars = [TrainCompositionCar(
        composition_id=train_composition_car.composition_id,
        car_id=train_composition_car.car_id,
        car_number=train_composition_car.car_number,
    ) for train_composition_car in train_compositions_cars]
    bulk_save_entities(db_connection, db_name, train_compositions_cars)
