using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.PostgreSQL.Migrations
{
    public partial class CreateGuestCleanupTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION delete_orphaned_guests()
                RETURNS TRIGGER AS $$
                BEGIN
                    DELETE FROM users u
                    WHERE u.is_guest = true
                      AND NOT EXISTS (
                        SELECT 1 FROM players p 
                        WHERE p.user_id = u.id
                      );

                    RETURN OLD;
                END;
                $$ LANGUAGE plpgsql;

                DROP TRIGGER IF EXISTS cleanup_guests_on_room_delete ON rooms;
                CREATE TRIGGER cleanup_guests_on_room_delete
                BEFORE DELETE ON rooms
                FOR EACH ROW
                EXECUTE FUNCTION delete_orphaned_guests();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS cleanup_guests_on_room_delete ON rooms;
                DROP FUNCTION IF EXISTS delete_orphaned_guests();
            ");
        }
    }
}
