from sql.db_handler import DatabaseConnection, initialize_database, drop_database, bulk_save_entities
from sql.fleet.entities import TrainStation, Train, TrainComposition, Car, Seat, TrainCompositionCar
from sql.fleet.models import TrainModel, StationModel, TrainCompositionModel, CarModel, SeatModel, \
    TrainCompositionCarModel
from utils.json_handler import open_json_file
from utils.logger import create_logger


def insert_fleet(db_connection: DatabaseConnection):
    logger = create_logger()

    db_name = "fleet"
    drop_database(db_connection, db_name)
    initialize_database(db_connection, db_name, models_module="sql.fleet.entities")

    logger.info("Formatting train_stations")
    stations = open_json_file("train_stations", StationModel)
    stations = [TrainStation(
        id=station.id,
        city=station.city,
        name=station.name,
        latitude=station.latitude,
        longitude=station.longitude,
    ) for station in stations]
    logger.info("Inserting train_stations")
    bulk_save_entities(db_connection, db_name, stations)

    logger.info("Formatting trains")
    trains = open_json_file("trains", TrainModel)
    trains = [Train(
        id=train.id,
        type=train.type,
        number=train.number,
    ) for train in trains]
    logger.info("Inserting trains")
    bulk_save_entities(db_connection, db_name, trains)

    logger.info("Formatting compositions")
    train_compositions = open_json_file("compositions", TrainCompositionModel)
    train_compositions = [TrainComposition(
        id=train_composition.id,
        train_id=train_composition.train_id
    ) for train_composition in train_compositions]
    logger.info("Inserting compositions")
    bulk_save_entities(db_connection, db_name, train_compositions)

    logger.info("Formatting cars")
    cars = open_json_file("cars", CarModel)
    cars = [Car(
        id=car.id
    ) for car in cars]
    logger.info("Inserting cars")
    bulk_save_entities(db_connection, db_name, cars)

    logger.info("Formatting seats")
    seats = open_json_file("seats", SeatModel)
    seats = [Seat(
        id=seat.id,
        car_id=seat.car_id,
        number=seat.number,
        x_pos=seat.x_pos,
        y_pos=seat.y_pos
    ) for seat in seats]
    logger.info("Inserting seats")
    bulk_save_entities(db_connection, db_name, seats)

    logger.info("Formatting compositions_cars")
    train_compositions_cars = open_json_file("compositions_cars", TrainCompositionCarModel)
    train_compositions_cars = [TrainCompositionCar(
        composition_id=train_composition_car.composition_id,
        car_id=train_composition_car.car_id,
        car_number=train_composition_car.car_number,
    ) for train_composition_car in train_compositions_cars]
    logger.info("Inserting compositions_cars")
    bulk_save_entities(db_connection, db_name, train_compositions_cars)
