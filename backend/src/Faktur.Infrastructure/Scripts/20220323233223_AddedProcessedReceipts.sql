START TRANSACTION;

ALTER TABLE "Receipts" ADD "Processed" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Receipts" ADD "ProcessedAt" timestamp with time zone NULL;

ALTER TABLE "Receipts" ADD "ProcessedById" uuid NULL;

CREATE INDEX "IX_Receipts_Processed" ON "Receipts" ("Processed");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220323233223_AddedProcessedReceipts', '6.0.3');

COMMIT;
