import importlib
import urllib.parse
from sqlmodel import SQLModel, create_engine, Session
from sqlalchemy import text
from pydantic import BaseModel


class DatabaseConnection(BaseModel):
    ip: str
    port: str
    user: str = "sa"
    password: str


def create_database_engine(db_connection: DatabaseConnection, db_name: str, echo=True):
    params = "?driver=ODBC+Driver+18+for+SQL+Server&Encrypt=yes&TrustServerCertificate=yes"
    "&unicode_results=True"

    password = urllib.parse.quote_plus(db_connection.password)

    url = f"mssql+pyodbc://{db_connection.user}:{password}@{db_connection.ip}:{db_connection.port}/{db_name}{params}"

    engine = create_engine(url, echo=echo)
    return engine


def bulk_save_entities(db_connection: DatabaseConnection, db_name: str, entities: list[SQLModel]):
    engine = create_database_engine(db_connection, db_name, echo=True)
    with Session(engine) as session:
        session.add_all(entities)
        session.commit()


def save_entity(db_connection: DatabaseConnection, db_name: str, entity: SQLModel):
    engine = create_database_engine(db_connection, db_name, echo=True)
    with Session(engine) as session:
        session.add(entity)
        session.commit()


def drop_database(db_connection: DatabaseConnection, db_name: str):
    master_engine = create_database_engine(db_connection, "master")

    with master_engine.connect().execution_options(isolation_level="AUTOCOMMIT") as conn:
        result = conn.execute(text(f"SELECT database_id FROM sys.databases WHERE name = '{db_name}'"))
        if result.fetchone():
            conn.execute(text(f"ALTER DATABASE [{db_name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"))
            conn.execute(text(f"DROP DATABASE [{db_name}]"))

    master_engine.dispose()


def initialize_database(db_connection: DatabaseConnection, db_name: str, models_module: str = ""):
    if models_module != "":
        importlib.import_module(models_module)

    master_engine = create_database_engine(db_connection, "master")

    with master_engine.connect().execution_options(isolation_level="AUTOCOMMIT") as conn:
        result = conn.execute(text(f"SELECT database_id FROM sys.databases WHERE name = '{db_name}'"))
        if not result.fetchone():
            conn.execute(text(f"CREATE DATABASE {db_name}"))

    master_engine.dispose()

    engine = create_database_engine(db_connection, db_name, echo=True)
    SQLModel.metadata.create_all(engine)
    engine.dispose()
