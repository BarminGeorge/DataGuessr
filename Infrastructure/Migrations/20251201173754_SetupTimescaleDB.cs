using Microsoft.EntityFrameworkCore.Migrations;

public partial class SetupTimescaleDB : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Установить расширение
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");

        // Удалить индексы (потому что они конфликтуют с гипертаблицей)
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_rooms_Status\";");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_rooms_ClosedAt\";");

        // Создать гипертаблицу
        migrationBuilder.Sql(@"
            SELECT create_hypertable('rooms', 'Id', 
                if_not_exists => TRUE,
                migrate_data => TRUE
            );
        ");

        // Retention policy
        migrationBuilder.Sql(@"
            SELECT add_retention_policy('rooms', 
                INTERVAL '30 days',
                if_not_exists => TRUE
            );
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("SELECT remove_retention_policy('rooms', if_exists => TRUE);");
    }
}
