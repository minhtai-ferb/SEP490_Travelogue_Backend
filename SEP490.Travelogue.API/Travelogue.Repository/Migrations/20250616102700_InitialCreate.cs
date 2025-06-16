using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_key = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<float>(type: "float", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interests", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "news_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiry_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_types", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "type_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_events", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "type_experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_experiences", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "type_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_location", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    password_hash = table.Column<byte[]>(type: "longblob", nullable: true),
                    password_salt = table.Column<byte[]>(type: "longblob", nullable: true),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    full_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    profile_picture_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sex = table.Column<int>(type: "int", nullable: false),
                    address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    google_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_email_verified = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    reset_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reset_token_expires = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    verification_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    verification_token_expires = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    lockout_end = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "district_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    district_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_district_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_district_medias_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_districts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    district_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_districts", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_districts_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_districts_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tours",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    tour_type_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours", x => x.id);
                    table.ForeignKey(
                        name: "FK_tours_tour_types_tour_type_id",
                        column: x => x.tour_type_id,
                        principalTable: "tour_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    heritage_rank = table.Column<int>(type: "int", nullable: false),
                    type_location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    district_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_locations_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_locations_type_location_type_location_id",
                        column: x => x.type_location_id,
                        principalTable: "type_location",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    receiver_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    sender_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sent_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_guides",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    rating = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    introduction = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_guides", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_guides_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plans", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plans_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    interest_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_interests_interests_interest_id",
                        column: x => x.interest_id,
                        principalTable: "interests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_interests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rating = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_groups_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    interest_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_interests_interests_interest_id",
                        column: x => x.interest_id,
                        principalTable: "interests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_interests_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vouchers", x => x.id);
                    table.ForeignKey(
                        name: "FK_vouchers_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vouchers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "craft_villages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price_per_night = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_craft_villages", x => x.id);
                    table.ForeignKey(
                        name: "FK_craft_villages_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cuisines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cuisine_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuisines", x => x.id);
                    table.ForeignKey(
                        name: "FK_cuisines_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_event_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    district_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    lunar_start_date = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lunar_end_date = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_recurring = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    recurrence_pattern = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_highlighted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_events_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_events_type_events_type_event_id",
                        column: x => x.type_event_id,
                        principalTable: "type_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "favorite_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_favorite_locations_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_locations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hotels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price_per_night = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<double>(type: "double", nullable: false),
                    longitude = table.Column<double>(type: "double", nullable: false),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotels", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotels_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    interest_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_interests_interests_interest_id",
                        column: x => x.interest_id,
                        principalTable: "interests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_interests_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_medias_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_plans", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_plans_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tour_plans_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_plans_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcements", x => x.id);
                    table.ForeignKey(
                        name: "FK_announcements_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_announcements_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_guide_availabilities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_guide_availabilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_guide_availabilities_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_exchange_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    final_status = table.Column<int>(type: "int", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plan_exchange_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_shares",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    permission = table.Column<int>(type: "int", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plan_shares", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_shares_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_shares_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    version_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    version_number = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plan_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_versions_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    review_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reported_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_reports_reviews_review_id",
                        column: x => x.review_id,
                        principalTable: "reviews",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_reports_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "craft_village_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    craft_village_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    interest_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_craft_village_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_craft_village_interests_craft_villages_craft_village_id",
                        column: x => x.craft_village_id,
                        principalTable: "craft_villages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_craft_village_interests_interests_interest_id",
                        column: x => x.interest_id,
                        principalTable: "interests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "craft_village_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    craft_village_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_craft_village_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_craft_village_media_craft_villages_craft_village_id",
                        column: x => x.craft_village_id,
                        principalTable: "craft_villages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_craft_village_suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    craft_village_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_craft_village_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_craft_village_suggestions_craft_villages_craft_vill~",
                        column: x => x.craft_village_id,
                        principalTable: "craft_villages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_craft_village_suggestions_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "withdrawal_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    craft_village_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    request_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    processed_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    processed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawal_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_withdrawal_requests_craft_villages_craft_village_id",
                        column: x => x.craft_village_id,
                        principalTable: "craft_villages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_withdrawal_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cuisine_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    cuisine_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    interest_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuisine_interests", x => x.id);
                    table.ForeignKey(
                        name: "FK_cuisine_interests_cuisines_cuisine_id",
                        column: x => x.cuisine_id,
                        principalTable: "cuisines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cuisine_interests_interests_interest_id",
                        column: x => x.interest_id,
                        principalTable: "interests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cuisine_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cuisine_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuisine_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_cuisine_medias_cuisines_cuisine_id",
                        column: x => x.cuisine_id,
                        principalTable: "cuisines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_cuisine_suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    cuisine_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_cuisine_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_cuisine_suggestions_cuisines_cuisine_id",
                        column: x => x.cuisine_id,
                        principalTable: "cuisines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_cuisine_suggestions_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "event_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    event_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_medias_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    event_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    type_experience_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    district_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_experiences", x => x.id);
                    table.ForeignKey(
                        name: "FK_experiences_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_experiences_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_experiences_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_experiences_type_experiences_type_experience_id",
                        column: x => x.type_experience_id,
                        principalTable: "type_experiences",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    event_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    news_category_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_highlighted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news", x => x.id);
                    table.ForeignKey(
                        name: "FK_news_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_news_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_news_news_categories_news_category_id",
                        column: x => x.news_category_id,
                        principalTable: "news_categories",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hotel_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    hotel_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotel_medias_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_hotel_suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    hotel_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_hotel_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_hotel_suggestions_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_hotel_suggestions_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_plan_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_plan_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_plan_locations_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_plan_locations_tour_plans_tour_plan_id",
                        column: x => x.tour_plan_id,
                        principalTable: "tour_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_announcements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    announcement_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    is_readed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_announcements", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_announcements_announcements_announcement_id",
                        column: x => x.announcement_id,
                        principalTable: "announcements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_announcements_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    order_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    scheduled_start_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    scheduled_end_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    cancelled_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    is_open_to_join = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    total_paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_exchanges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    suggested_trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    session_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    user_responded_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    user_response_message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plan_exchanges", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_exchange_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "trip_plan_exchange_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_versions_suggested_trip_plan_v~",
                        column: x => x.suggested_trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_plan_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_locations_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_locations_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "experience_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    experience_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_experience_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_experience_medias_experiences_experience_id",
                        column: x => x.experience_id,
                        principalTable: "experiences",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "news_medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_in_bytes = table.Column<float>(type: "float", nullable: false),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    news_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news_medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_news_medias_news_news_id",
                        column: x => x.news_id,
                        principalTable: "news",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_withdrawals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    withdrawal_request_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_withdrawals", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_withdrawals_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_withdrawals_withdrawal_requests_withdrawal_request_id",
                        column: x => x.withdrawal_request_id,
                        principalTable: "withdrawal_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "refund_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    request_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approved_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    refund_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refund_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_refund_requests_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refund_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_group_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_group_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    join_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_group_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_group_members_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_group_members_tour_groups_tour_group_id",
                        column: x => x.tour_group_id,
                        principalTable: "tour_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_group_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_join_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    from_order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    to_order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    request_note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    response_note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requested_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    responded_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_join_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_join_requests_orders_from_order_id",
                        column: x => x.from_order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_join_requests_orders_to_order_id",
                        column: x => x.to_order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_tour_guide_id",
                table: "announcements",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_tour_id",
                table: "announcements",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_craft_village_interests_craft_village_id",
                table: "craft_village_interests",
                column: "craft_village_id");

            migrationBuilder.CreateIndex(
                name: "IX_craft_village_interests_interest_id",
                table: "craft_village_interests",
                column: "interest_id");

            migrationBuilder.CreateIndex(
                name: "IX_craft_village_media_craft_village_id",
                table: "craft_village_media",
                column: "craft_village_id");

            migrationBuilder.CreateIndex(
                name: "IX_craft_villages_location_id",
                table: "craft_villages",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cuisine_interests_cuisine_id",
                table: "cuisine_interests",
                column: "cuisine_id");

            migrationBuilder.CreateIndex(
                name: "IX_cuisine_interests_interest_id",
                table: "cuisine_interests",
                column: "interest_id");

            migrationBuilder.CreateIndex(
                name: "IX_cuisine_medias_cuisine_id",
                table: "cuisine_medias",
                column: "cuisine_id");

            migrationBuilder.CreateIndex(
                name: "IX_cuisines_location_id",
                table: "cuisines",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_district_medias_district_id",
                table: "district_medias",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_medias_event_id",
                table: "event_medias",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_district_id",
                table: "events",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_location_id",
                table: "events",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_type_event_id",
                table: "events",
                column: "type_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_experience_medias_experience_id",
                table: "experience_medias",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_experiences_district_id",
                table: "experiences",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_experiences_event_id",
                table: "experiences",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_experiences_location_id",
                table: "experiences",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_experiences_type_experience_id",
                table: "experiences",
                column: "type_experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_locations_location_id",
                table: "favorite_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_locations_user_id",
                table: "favorite_locations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_medias_hotel_id",
                table: "hotel_medias",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotels_location_id",
                table: "hotels",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_location_craft_village_suggestions_craft_village_id",
                table: "location_craft_village_suggestions",
                column: "craft_village_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_craft_village_suggestions_location_id",
                table: "location_craft_village_suggestions",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_cuisine_suggestions_cuisine_id",
                table: "location_cuisine_suggestions",
                column: "cuisine_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_cuisine_suggestions_location_id",
                table: "location_cuisine_suggestions",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_hotel_suggestions_hotel_id",
                table: "location_hotel_suggestions",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_hotel_suggestions_location_id",
                table: "location_hotel_suggestions",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_interests_interest_id",
                table: "location_interests",
                column: "interest_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_interests_location_id",
                table: "location_interests",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_medias_location_id",
                table: "location_medias",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_locations_district_id",
                table: "locations",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_locations_type_location_id",
                table: "locations",
                column: "type_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_receiver_id",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_news_event_id",
                table: "news",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_news_location_id",
                table: "news",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_news_news_category_id",
                table: "news",
                column: "news_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_news_medias_news_id",
                table: "news_medias",
                column: "news_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_withdrawals_order_id",
                table: "order_withdrawals",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_withdrawals_withdrawal_request_id",
                table: "order_withdrawals",
                column: "withdrawal_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tour_guide_id",
                table: "orders",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tour_id",
                table: "orders",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_id",
                table: "orders",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_version_id",
                table: "orders",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refund_requests_order_id",
                table: "refund_requests",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_refund_requests_user_id",
                table: "refund_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_review_id",
                table: "reports",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_user_id",
                table: "reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_tour_id",
                table: "reviews",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_user_id",
                table: "reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_districts_district_id",
                table: "role_districts",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_districts_role_id",
                table: "role_districts",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_group_members_order_id",
                table: "tour_group_members",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_group_members_tour_group_id",
                table: "tour_group_members",
                column: "tour_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_group_members_user_id",
                table: "tour_group_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_groups_tour_id",
                table: "tour_groups",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_availabilities_tour_guide_id",
                table: "tour_guide_availabilities",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guides_user_id",
                table: "tour_guides",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_interests_interest_id",
                table: "tour_interests",
                column: "interest_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_interests_tour_id",
                table: "tour_interests",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_join_requests_from_order_id",
                table: "tour_join_requests",
                column: "from_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_join_requests_to_order_id",
                table: "tour_join_requests",
                column: "to_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_locations_location_id",
                table: "tour_plan_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_locations_tour_plan_id",
                table: "tour_plan_locations",
                column: "tour_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plans_location_id",
                table: "tour_plans",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plans_tour_id",
                table: "tour_plans",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plans_user_id",
                table: "tour_plans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tours_tour_type_id",
                table: "tours",
                column: "tour_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_order_id",
                table: "transactions",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_created_by_user_id",
                table: "trip_plan_exchange_sessions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_tour_guide_id",
                table: "trip_plan_exchange_sessions",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_trip_plan_id",
                table: "trip_plan_exchange_sessions",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_session_id",
                table: "trip_plan_exchanges",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_suggested_trip_plan_version_id",
                table: "trip_plan_exchanges",
                column: "suggested_trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_tour_guide_id",
                table: "trip_plan_exchanges",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_trip_plan_id",
                table: "trip_plan_exchanges",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_trip_plan_version_id",
                table: "trip_plan_exchanges",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_user_id",
                table: "trip_plan_exchanges",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_locations_location_id",
                table: "trip_plan_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_locations_trip_plan_version_id",
                table: "trip_plan_locations",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_shares_trip_plan_id",
                table: "trip_plan_shares",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_shares_user_id",
                table: "trip_plan_shares",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_versions_trip_plan_id",
                table: "trip_plan_versions",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plans_user_id",
                table: "trip_plans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_announcements_announcement_id",
                table: "user_announcements",
                column: "announcement_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_announcements_user_id",
                table: "user_announcements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_interest_id",
                table: "user_interests",
                column: "interest_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_user_id",
                table: "user_interests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_tour_id",
                table: "vouchers",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_user_id",
                table: "vouchers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_craft_village_id",
                table: "withdrawal_requests",
                column: "craft_village_id");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_user_id",
                table: "withdrawal_requests",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "craft_village_interests");

            migrationBuilder.DropTable(
                name: "craft_village_media");

            migrationBuilder.DropTable(
                name: "cuisine_interests");

            migrationBuilder.DropTable(
                name: "cuisine_medias");

            migrationBuilder.DropTable(
                name: "district_medias");

            migrationBuilder.DropTable(
                name: "event_medias");

            migrationBuilder.DropTable(
                name: "experience_medias");

            migrationBuilder.DropTable(
                name: "favorite_locations");

            migrationBuilder.DropTable(
                name: "hotel_medias");

            migrationBuilder.DropTable(
                name: "location_craft_village_suggestions");

            migrationBuilder.DropTable(
                name: "location_cuisine_suggestions");

            migrationBuilder.DropTable(
                name: "location_hotel_suggestions");

            migrationBuilder.DropTable(
                name: "location_interests");

            migrationBuilder.DropTable(
                name: "location_medias");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "news_medias");

            migrationBuilder.DropTable(
                name: "order_withdrawals");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "refund_requests");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "role_districts");

            migrationBuilder.DropTable(
                name: "system_settings");

            migrationBuilder.DropTable(
                name: "tour_group_members");

            migrationBuilder.DropTable(
                name: "tour_guide_availabilities");

            migrationBuilder.DropTable(
                name: "tour_interests");

            migrationBuilder.DropTable(
                name: "tour_join_requests");

            migrationBuilder.DropTable(
                name: "tour_plan_locations");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "trip_plan_exchanges");

            migrationBuilder.DropTable(
                name: "trip_plan_locations");

            migrationBuilder.DropTable(
                name: "trip_plan_shares");

            migrationBuilder.DropTable(
                name: "user_announcements");

            migrationBuilder.DropTable(
                name: "user_interests");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "experiences");

            migrationBuilder.DropTable(
                name: "cuisines");

            migrationBuilder.DropTable(
                name: "hotels");

            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "withdrawal_requests");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "tour_groups");

            migrationBuilder.DropTable(
                name: "tour_plans");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "trip_plan_exchange_sessions");

            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "interests");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "type_experiences");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "news_categories");

            migrationBuilder.DropTable(
                name: "craft_villages");

            migrationBuilder.DropTable(
                name: "trip_plan_versions");

            migrationBuilder.DropTable(
                name: "tour_guides");

            migrationBuilder.DropTable(
                name: "tours");

            migrationBuilder.DropTable(
                name: "type_events");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "trip_plans");

            migrationBuilder.DropTable(
                name: "tour_types");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "type_location");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
