using System;
using Domain.ValueTypes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnswerToJsonStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Конвертируем DateTime в текст, потом в jsonb
            migrationBuilder.Sql(@"
                ALTER TABLE questions 
                ALTER COLUMN ""RightAnswer"" TYPE jsonb 
                USING (jsonb_build_object('Value', ""RightAnswer""::text, '$type', 'DateTimeAnswer'));
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE player_answers 
                ALTER COLUMN ""Answer"" TYPE jsonb 
                USING (jsonb_build_object('Value', ""Answer""::text, '$type', 'DateTimeAnswer'));
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RightAnswer",
                table: "questions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(Answer),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Answer",
                table: "player_answers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(Answer),
                oldType: "jsonb");
        }
    }
}
