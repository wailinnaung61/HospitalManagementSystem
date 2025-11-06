using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class ChangeTableNamePurchasesToPurchaseAndPurchaseItemsToPurchaseItemAndAddBatchIdToPurchaseItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Item_ItemId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Unit_UnitId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Branch_BranchId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Warehouse_WarehouseId",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchase");

            migrationBuilder.RenameTable(
                name: "PurchaseItems",
                newName: "PurchaseItem");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_WarehouseId",
                table: "Purchase",
                newName: "IX_Purchase_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_BranchId",
                table: "Purchase",
                newName: "IX_Purchase_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_UnitId",
                table: "PurchaseItem",
                newName: "IX_PurchaseItem_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_ItemId",
                table: "PurchaseItem",
                newName: "IX_PurchaseItem_ItemId");

            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "PurchaseItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItem",
                table: "PurchaseItem",
                columns: new[] { "PurchaseId", "ItemId", "UnitId", "BatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItem_BatchId",
                table: "PurchaseItem",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Branch_BranchId",
                table: "Purchase",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Warehouse_WarehouseId",
                table: "Purchase",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Batch_BatchId",
                table: "PurchaseItem",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Item_ItemId",
                table: "PurchaseItem",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Purchase_PurchaseId",
                table: "PurchaseItem",
                column: "PurchaseId",
                principalTable: "Purchase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItem_Unit_UnitId",
                table: "PurchaseItem",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Branch_BranchId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Warehouse_WarehouseId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Batch_BatchId",
                table: "PurchaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Item_ItemId",
                table: "PurchaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Purchase_PurchaseId",
                table: "PurchaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItem_Unit_UnitId",
                table: "PurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItem",
                table: "PurchaseItem");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItem_BatchId",
                table: "PurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "PurchaseItem");

            migrationBuilder.RenameTable(
                name: "PurchaseItem",
                newName: "PurchaseItems");

            migrationBuilder.RenameTable(
                name: "Purchase",
                newName: "Purchases");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItem_UnitId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItem_ItemId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_WarehouseId",
                table: "Purchases",
                newName: "IX_Purchases_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_BranchId",
                table: "Purchases",
                newName: "IX_Purchases_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems",
                columns: new[] { "PurchaseId", "ItemId", "UnitId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Item_ItemId",
                table: "PurchaseItems",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Unit_UnitId",
                table: "PurchaseItems",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Branch_BranchId",
                table: "Purchases",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Warehouse_WarehouseId",
                table: "Purchases",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
