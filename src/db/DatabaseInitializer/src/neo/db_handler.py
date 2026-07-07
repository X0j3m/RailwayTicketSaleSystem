import urllib
from typing import Type, List, Any
from neomodel import config, StructuredNode, db
from pydantic import BaseModel

from utils.logger import create_logger


class DatabaseConnection(BaseModel):
    ip: str
    port: str
    user: str = "neo4j"
    password: str = ""


def create_relations(query: str, batch_data):
    db.cypher_query(query, {"batch": batch_data})


def clear_database(db_connection):
    logger = create_logger()
    logger.info(f"Clearing database {db_connection.ip}:{db_connection.port}")
    password = urllib.parse.quote_plus(db_connection.password)
    config.DATABASE_URL = f'bolt://{db_connection.user}:{password}@{db_connection.ip}:{db_connection.port}'

    query = "MATCH (n) DETACH DELETE n"
    db.cypher_query(query)
    logger.info(f"Database {db_connection.ip}:{db_connection.port} cleared")


def clear_temp_properties():
    db.cypher_query("MATCH (n:Stop) REMOVE n.stop_id")
    db.cypher_query("MATCH (n:Stop) REMOVE n.station_id")


def bulk_save_entities(db_connection: DatabaseConnection, model_class: Type[StructuredNode],
                       entities: List[Any]):
    print(f"Saving entities to database {db_connection}")   
    password = urllib.parse.quote_plus(db_connection.password)
    config.DATABASE_URL = f'bolt://{db_connection.user}:{password}@{db_connection.ip}:{db_connection.port}'
    cleaned_entities = []
    for entity in entities:
        if hasattr(entity, '__properties__'):
            props = dict(entity.__properties__)
        else:
            props = dict(entity)
        cleaned_entities.append(props)

    return model_class.create_or_update(*cleaned_entities)
