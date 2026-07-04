from sql.db_handler import DatabaseConnection

from sql.trains.db_insert import insert_trains


def main():
    dbc = DatabaseConnection(
        ip="127.0.0.1",
        port="1433",
        password="RootP@ssword123",
    )
    insert_trains(dbc)


if __name__ == "__main__":
    main()
