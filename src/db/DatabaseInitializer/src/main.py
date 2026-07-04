from sql.db_handler import DatabaseConnection, initialize_database, drop_database


def main():
    print("Hello World!")
    dbc = DatabaseConnection(
        ip="127.0.0.1",
        port="1433",
        password="RootP@ssword123",
    )
    db_name = "trains"
    drop_database(dbc, db_name)
    initialize_database(dbc, db_name, models_module="sql.trains.entities")


if __name__ == "__main__":
    main()
