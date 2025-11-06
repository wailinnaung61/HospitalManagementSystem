using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Helpers;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Repositories.Base;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Services;
using MSIS_HMS.Models;
using Microsoft.Extensions.Logging;
using System;

namespace MSIS_HMS
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("MSIS_HMS.Infrastructure")));
     
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
            });
            services.AddCors();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<Pagination>(Configuration.GetSection("Pagination"));
            services.AddAutoMapper(typeof(Startup));

            // services.AddMvc(options =>
            // {
            //     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            // });

            // Enable Dual Authentication 
            //services.AddAuthentication(configureOptions =>
            //{
            //    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //  .AddCookie(cfg => cfg.SlidingExpiration = true)
            //  .AddJwtBearer(cfg =>
            //  {
            //      cfg.RequireHttpsMetadata = false;
            //      cfg.SaveToken = true;
            //      cfg.TokenValidationParameters = new TokenValidationParameters()
            //      {
            //          ValidIssuer = Configuration["AppSettings:Issuer"],
            //          ValidAudience = Configuration["AppSettings:Issuer"],
            //          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:Key"]))
            //      };
            //  });

            // configure DI for application services
            services.AddScoped<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IConfigService, ConfigService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IBranchService, BranchService>();
            services.AddTransient<IWarehouseService, WarehouseService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IBatchService, BatchService>();
            services.AddTransient<IReportService, ReportService>();

            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<INurseRepository, NurseRepository>();
            services.AddScoped<ISpecialityRepository, SpecialityRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<IServiceRepository,ServiceRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IReferrerRepository, ReferrerRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IItemTypeRepository, ItemTypeRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IBatchRepository, BatchRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IOutletRepository, OutletRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IWarehouseTransferRepository, WarehouseTransferRepository>();
            services.AddScoped<IOutletTransferRepository,OutletTransferRepository> ();
            services.AddScoped<IItemLocationRepository, ItemLocationRepository>();
            services.AddScoped<IDeliverOrderRepository,DeliverOrderRepository>();
            services.AddScoped<IReturnRepository, ReturnRepository>();
            services.AddScoped<IScratchRepository, ScratchRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IVisitTypeRepository, VisitTypeRepository>();
            services.AddScoped<IAppointmentTypeRepository, AppointmentTypeRepository>();
            services.AddScoped<IVisitRepository, VisitRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IGroundingRepository, GroundingRepository>();
            services.AddScoped<ISymptomRepository, SymptomRepository>();
            services.AddScoped<IDiagnosticRepository, DiagnosticRepository>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IWardRepository,WardRepository>();
            services.AddScoped<IRoomRepository,RoomRepository>();
            services.AddScoped<IRoomTypeRepository,RoomTypeRepository>();
            services.AddScoped<IBedRepository,BedRepository>();
            services.AddScoped<IBedTypeRepository,BedTypeRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IIPDRecordRepository, IPDRecordRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IIPDAllotmentRepository,IPDAllotmentRepository>();
            services.AddScoped<ILabPersonRepository, LabPersonRepository>();
            services.AddScoped<ILabTestRepository, LabTestRepository>();
            services.AddScoped<ILabOrderRepository, LabOrderRepository>();
            services.AddScoped<ILabResultRepository, LabResultRepository>();
            services.AddScoped<IOperationRoomRepository, OperationRoomRepository>();
            services.AddScoped<IOperationTypeRepository, OperationTypeRepository>();
            services.AddScoped<IOperationTreaterRepository, OperationTreaterRepository>();
            services.AddScoped<IPatientResultImageRepository, PatientResultImageRepository>();

            // Seeding Data
            services.AddTransient<ApplicationDbContextSeed>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:9000")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContextSeed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseCors();
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            // Seeding Data
            seeder.Seed();

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/echo",
                 context => context.Response.WriteAsync("echo"))
                 .RequireCors(MyAllowSpecificOrigins);

                endpoints.MapControllers()
                         .RequireCors(MyAllowSpecificOrigins);

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
