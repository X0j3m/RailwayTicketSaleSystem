from neo.db_handler import DatabaseConnection, bulk_save_entities, clear_database, clear_temp_properties, \
    create_relations
from neo.schedule.models import StopModel, LeadsToRelationModel, TransferRelationModel
from neo.schedule.nodes import Stop, TrainStation
from sql.fleet.models import StationModel
from utils.json_handler import open_json_file
from datetime import datetime, timedelta

from utils.logger import create_logger


def add_minutes(time: str, minutes: int):
    if minutes is None:
        return None
    time_obj = datetime.strptime(time, '%H:%M:%S')
    new_time_obj = time_obj + timedelta(minutes=minutes)
    return new_time_obj.strftime('%H:%M:%S')


def insert_schedule(db_connection: DatabaseConnection):
    logger = create_logger()
    stops = open_json_file("stops", StopModel)
    stations = open_json_file("train_stations", StationModel)
    leads_to = open_json_file("leads_to_relations", LeadsToRelationModel)
    transfers = open_json_file("transfers", TransferRelationModel)

    logger.info("Formatting stations")
    stations = [TrainStation(
        station_id=station.id,
        city=station.city,
        name=station.name,
        latitude=station.latitude,
        longitude=station.longitude
    ) for station in stations]
    logger.info("Inserting stations")
    stations = bulk_save_entities(db_connection, TrainStation, stations)

    logger.info("Formatting stops")
    stops = [Stop(
        stop_id=stop.id,
        start_station_time=stop.start_station_time,
        arrival_time_minutes=stop.arrival_time_minutes,
        arrival_time=add_minutes(stop.start_station_time, stop.arrival_time_minutes),
        departure_time_minutes=stop.departure_time_minutes,
        departure_time=add_minutes(stop.start_station_time, stop.departure_time_minutes),
        station_id=stop.station_id,
        train_composition_id=stop.train_composition_id
    ) for stop in stops]
    logger.info("Inserting stops")
    stops = bulk_save_entities(db_connection, Stop, stops)

    located_at_relations = [
        {"stop_id": stop.stop_id, "station_id": stop.station_id}
        for stop in stops if stop.station_id
    ]
    query = """
    UNWIND $batch AS row
    MATCH (stop:Stop {stop_id: row.stop_id})
    MATCH (station:TrainStation {station_id: row.station_id})
    MERGE (stop)-[:LOCATED_AT]->(station)
    """
    # MERGE (station)-[:HAS]->(stop)
    # """
    logger.info("Inserting LOCATED_AT relations")
    create_relations(query, located_at_relations)

    leads_to_relations = [
        {"from_id": lead.from_stop_id, "to_id": lead.to_stop_id, "time": lead.time}
        for lead in leads_to
    ]
    query = """
    UNWIND $batch AS row
    MATCH (stop1:Stop {stop_id: row.from_id})
    MATCH (stop2:Stop {stop_id: row.to_id})
    MERGE (stop1)-[r:LEADS_TO]->(stop2)
    SET r.time = row.time
    """
    logger.info("Inserting LEADS_TO relations")
    create_relations(query, leads_to_relations)

    transfers_relations = [
        {"from_id": transfer.from_stop_id, "to_id": transfer.to_stop_id, "time": transfer.time}
        for transfer in transfers
    ]
    transfers_query = """
        UNWIND $batch AS row
        MATCH (stop1:Stop {stop_id: row.from_id})
        MATCH (stop2:Stop {stop_id: row.to_id})
        MERGE (stop1)-[r:TRANSFER]->(stop2)
        SET r.time = row.time
        """
    logger.info("Inserting TRANSFER relations")
    create_relations(transfers_query, transfers_relations)
    
    # logger.info("Clearing temporary properties")
    # clear_temp_properties()
