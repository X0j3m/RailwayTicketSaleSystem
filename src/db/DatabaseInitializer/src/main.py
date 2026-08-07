from neo.db_insert import insert_schedule
from sql.db_handler import DatabaseConnection as SqlDatabaseConnection
from neo.db_handler import DatabaseConnection as Neo4jDatabaseConnection
from sql.db_insert import insert


def main():
    sql_dbc = SqlDatabaseConnection(
        ip="mssql2025",
        port="1433",
        password="RootP@ssword123",
    )
    insert(sql_dbc)

    neo4j_dbc = Neo4jDatabaseConnection(
        ip="neo4j",
        port="7687"
    )
    insert_schedule(neo4j_dbc)


if __name__ == "__main__":
    main()
