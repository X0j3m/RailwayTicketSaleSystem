from sql.db_handler import DatabaseConnection, initialize_database, drop_database, bulk_save_entities


def insert_tickets(db_connection: DatabaseConnection):
    db_name = "reservations"
    drop_database(db_connection, db_name)
    initialize_database(db_connection, db_name, models_module="sql.reservations.entities")
