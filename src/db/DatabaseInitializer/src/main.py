from neo.schedule.db_insert import insert_schedule
from sql.db_handler import DatabaseConnection as SqlDatabaseConnection
from neo.db_handler import DatabaseConnection as Neo4jDatabaseConnection
from sql.fleet.db_insert import insert_fleet


def main():
    sql_dbc = SqlDatabaseConnection(
        ip="127.0.0.1",
        port="1433",
        password="RootP@ssword123",
    )
    insert_fleet(sql_dbc)

    neo4j_dbc = Neo4jDatabaseConnection(
        ip="127.0.0.1",
        port="7687"
    )
    insert_schedule(neo4j_dbc)


if __name__ == "__main__":
    main()
