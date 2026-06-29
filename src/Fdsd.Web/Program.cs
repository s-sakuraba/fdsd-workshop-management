using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Fdsd.Infrastructure.Persistence;
using Fdsd.Infrastructure.Persistence.Repositories;
using Fdsd.Infrastructure.Excel;
using Fdsd.Infrastructure.Files;
using Fdsd.Infrastructure;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Kenshu;
using Fdsd.Application.Attend;
using Fdsd.Application.Master;
using Fdsd.Application.Report;
using Fdsd.Web.Authentication;
using Fdsd.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Windows認証の切替設定:
// appsettings の Auth:EnableWindowsAuthentication で認証方式を選択する。
// true  = Windows認証 (Negotiate)
// false = 開発用バイパス認証 (DevBypassAuthenticationHandler)
var windowsAuthenticationEnabled = builder.Configuration.GetValue<bool?>("Auth:EnableWindowsAuthentication") ?? true;

if (windowsAuthenticationEnabled)
{
    // 本番/通常運用向け: Windows統合認証を使用
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();
}
else
{
    // 動作確認向け: 疑似ログインユーザーで認証を通す
    // 利用ユーザーは Auth:BypassAccount（未指定時はOSユーザー）で決まる
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = DevBypassAuthenticationHandler.SchemeName;
        options.DefaultChallengeScheme = DevBypassAuthenticationHandler.SchemeName;
    })
    .AddScheme<AuthenticationSchemeOptions, DevBypassAuthenticationHandler>(
        DevBypassAuthenticationHandler.SchemeName,
        _ => { });
}

builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.AddRequirements(new AdminRequirement()));
});

var connectionString = builder.Configuration.GetConnectionString("Fdsd")
    ?? throw new InvalidOperationException("Connection string 'Fdsd' not found.");

builder.Services.AddDbContext<FdsdDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());

// Infrastructure
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddSingleton<IClock, Fdsd.Infrastructure.SystemClock>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddSingleton<IExcelReportWriter, ExcelReportWriter>();

// Repositories
builder.Services.AddScoped<IKenshuRepository, KenshuRepository>();
builder.Services.AddScoped<IAttendRepository, AttendRepository>();
builder.Services.AddScoped<IKenshuGakkaRepository, KenshuGakkaRepository>();
builder.Services.AddScoped<IGakkaChangeRepository, GakkaChangeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGakkaRepository, GakkaRepository>();
builder.Services.AddScoped<IKenshuDocumentRepository, KenshuDocumentRepository>();
builder.Services.AddScoped<IWKenshuDocumentRepository, WKenshuDocumentRepository>();
builder.Services.AddScoped<IKenshuStyleRepository, KenshuStyleRepository>();
builder.Services.AddScoped<IGakkaOrderRepository, GakkaOrderRepository>();
builder.Services.AddScoped<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddScoped<ILeaveOfAbsenceRepository, LeaveOfAbsenceRepository>();
builder.Services.AddScoped<IEmpKubunRepository, EmpKubunRepository>();
builder.Services.AddScoped<IKCodeRepository, KCodeRepository>();
builder.Services.AddScoped<IM_IdManageRepository, M_IdManageRepository>();

// Services
builder.Services.AddScoped<KenshuService>();
builder.Services.AddScoped<AttendService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GakkaService>();
builder.Services.AddScoped<GakkaChangeService>();
builder.Services.AddScoped<StyleService>();
builder.Services.AddScoped<EmpKubunService>();
builder.Services.AddScoped<LeaveOfAbsenceService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ReportService>();

// 出欠登録など、1画面で多数の対象者(1人あたり複数の隠しフィールド)をPOSTする画面があるため、
// フォーム値数およびモデルバインドのコレクション上限(既定1024)を引き上げる。
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueCountLimit = 50000;
});

builder.Services.AddControllersWithViews(options =>
{
    options.MaxModelBindingCollectionSize = 50000;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=T_KENSHU}/{action=Menu}/{id?}");

app.Run();