﻿START TRANSACTION;

CREATE TABLE "Products" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "ArticleId" integer NOT NULL,
    "DepartmentId" integer NULL,
    "Description" text NULL,
    "Flags" character varying(10) NULL,
    "Label" character varying(100) NULL,
    "Sku" character varying(32) NULL,
    "StoreId" integer NOT NULL,
    "UnitPrice" money NULL,
    "UnitType" character varying(4) NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
    "CreatedById" uuid NOT NULL,
    "Deleted" boolean NOT NULL DEFAULT FALSE,
    "DeletedAt" timestamp with time zone NULL,
    "DeletedById" uuid NULL,
    "Key" uuid NOT NULL DEFAULT (uuid_generate_v4()),
    "UpdatedAt" timestamp with time zone NULL,
    "UpdatedById" uuid NULL,
    "Version" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Products_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Products_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id"),
    CONSTRAINT "FK_Products_Stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES "Stores" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Products_ArticleId" ON "Products" ("ArticleId");

CREATE INDEX "IX_Products_Deleted" ON "Products" ("Deleted");

CREATE INDEX "IX_Products_DepartmentId" ON "Products" ("DepartmentId");

CREATE UNIQUE INDEX "IX_Products_Key" ON "Products" ("Key");

CREATE INDEX "IX_Products_Label" ON "Products" ("Label");

CREATE UNIQUE INDEX "IX_Products_StoreId_ArticleId" ON "Products" ("StoreId", "ArticleId");

CREATE UNIQUE INDEX "IX_Products_StoreId_Sku" ON "Products" ("StoreId", "Sku");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220317152755_CreateProductTable', '6.0.3');

COMMIT;
