using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Infrastructure.Data.Seeding;

namespace MSIS_HMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Relations
            builder.Entity<UserAccessMenu>(b =>
            {
                b.HasKey(nameof(UserAccessMenu.UserId), nameof(UserAccessMenu.MenuId));
                b.HasOne(uam => uam.User).WithMany(u => u.UserAccessMenus);
                b.HasOne(uam => uam.Menu).WithMany(u => u.UserAccessMenus);
            });

            builder.Entity<PackingUnit>(b =>
            {
                b.HasKey(nameof(PackingUnit.ItemId), nameof(PackingUnit.UnitId));
                b.HasOne(pu => pu.Item).WithMany(item => item.PackingUnits);
                b.HasOne(pu => pu.Unit).WithMany(unit => unit.PackingUnits);
            });

            builder.Entity<WarehouseItem>(b =>
            {
                b.HasKey(nameof(WarehouseItem.WarehouseId), nameof(WarehouseItem.ItemId), nameof(WarehouseItem.BatchId));
                b.HasOne(wi => wi.Warehouse).WithMany(warehouse => warehouse.WarehouseItems);
                b.HasOne(wi => wi.Item).WithMany(item => item.WarehouseItems);
                b.HasOne(wi => wi.Batch).WithMany(batch => batch.WarehouseItems);
            });

            builder.Entity<OutletItem>(b =>
            {
                b.HasKey(nameof(OutletItem.OutletId), nameof(OutletItem.ItemId));
                b.HasOne(wi => wi.Outlet).WithMany(outlet => outlet.OutletItems);
                b.HasOne(wi => wi.Item).WithMany(item => item.OutletItems);
            });

            builder.Entity<WarehouseTransfer>(b =>
            {
                b.HasOne(wt => wt.FromWarehouse).WithMany(w => w.FromWarehouseTransfers).HasForeignKey(wt => wt.FromWarehouseId);
                b.HasOne(wt => wt.ToWarehouse).WithMany(w => w.ToWarehouseTransfers).HasForeignKey(wt => wt.ToWarehouseId);
            });

            builder.Entity<LabResult>(b =>
            {
                b.HasOne(lr => lr.Technician).WithMany(lp => lp.LabResultsByTechnicians).HasForeignKey(lr => lr.TechnicianId);
                b.HasOne(lr => lr.Consultant).WithMany(lp => lp.LabResultsByConsultants).HasForeignKey(lr => lr.ConsultantId);
            });

            // Seeding
            builder.Entity<Module>().HasData(SeedData.preconfirguredModules);
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<UserAccessMenu> UserAccessMenus { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Referrer> Referrers { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Township> Townships { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<Diagnostic> Diagnostics { get; set; }
        public DbSet<Position> Positions { get; set; }

        //Inventory Module
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemLocation> ItemLocations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<PackingUnit> PackingUnits { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<OutletItem> OutletItems { get; set; }
        public DbSet<WarehouseTransfer> WarehouseTransfers { get; set; }
        public DbSet<WarehouseTransferItem> WarehouseTransferItems { get; set; }
        public DbSet<OutletTransfer> OutletTransfers { get; set; }
        public DbSet<OutletTransferItem> OutletTransferItems { get; set; }
        public DbSet<DeliverOrder> DeliverOrders { get; set; }
        public DbSet<DeliverOrderItem> DeliverOrderItems { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }
        public DbSet<Scratch> Scratches { get; set; }
        public DbSet<ScratchItem> ScratchItems { get; set; }
        public DbSet<Grounding> Groundings { get; set; }
        public DbSet<GroundingItem> GroundingItems { get; set; }

        //Pharmacy Module
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }

        //Patient Management Module
        public DbSet<Patient> Patients { get; set; }

        //OPD Module
        public DbSet<VisitType> VisitTypes { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<PatientSymptom> PatientSymptoms { get; set; }
        public DbSet<PatientDiagnostic> PatientDiagnostics { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<BedType> BedTypes { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<IPDStaff> IPDStaffs { get; set; }
        public DbSet<IPDRecord> IPDRecords { get; set; }
        public DbSet<IPDRound> IPDRounds { get; set; }
        public DbSet<IPDOrder> IPDOrders { get; set; }
        public DbSet<IPDOrderItem> IPDOrderItems { get; set; }
        public DbSet<IPDOrderService> IPDOrderServices { get; set; }
        public DbSet<IPDAllotment> IPDAllotments { get; set; }
        public DbSet<IPDFood> IPDFoods { get; set; }
        public DbSet<IPDPayment> IPDPayments { get; set; }

        //Lab Module
        public DbSet<LabPerson> LabPersons { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabTestDetail> LabTestDetails { get; set; }
        public DbSet<LabPerson_LabTest> LabPerson_LabTests { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabOrderTest> LabOrderTests { get; set; }
        public DbSet<LabResult> LabResults { get; set; }
        public DbSet<LabResultDetail> LabResultDetails { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<OperationRoom> OperationRooms { get; set; }
        public DbSet<OperationTreater> OperationTreaters { get; set; }
        public DbSet<OperationService> OperationServices { get; set; }
        public DbSet<OperationItem> OperationItems { get; set; }
        public DbSet<OT_Doctor> OT_Doctors { get; set; }
        public DbSet<OT_Staff> OT_Staffs { get; set; }
        public DbSet<PatientResultImage> patientResultImages { get; set; }
        public DbSet<LabResultFile> LabResultFiles { get; set; }
        public DbSet<OT_Anaesthetist> OT_Anaesthetists { get; set; }
    }
}
