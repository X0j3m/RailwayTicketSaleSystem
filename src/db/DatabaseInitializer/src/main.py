from neo.schedule.db_insert import insert_schedule
from sql.db_handler import DatabaseConnection
from sql.fleet.db_insert import insert_fleet


def main():
    sql_dbc = DatabaseConnection(
        ip="127.0.0.1",
        port="1433",
        password="RootP@ssword123",
    )
    insert_fleet(sql_dbc)

    neo4j_dbc = DatabaseConnection(
        ip="127.0.0.1",
        port="1433"
    )
    insert_schedule(neo4j_dbc)


if __name__ == "__main__":
    main()
