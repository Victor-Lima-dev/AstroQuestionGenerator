using worker;
using Microsoft.EntityFrameworkCore;
using worker.context;
using worker.Models;


var build = Host.CreateDefaultBuilder(args);



build.ConfigureServices((context, services) =>
{
    string mySqlConnectionStr = context.Configuration.GetConnectionString("DefaultConnection");

    string rabbmitUrl = context.Configuration.GetConnectionString("RabbiMQ");

    var rabbitMQ = new Mensageiro(rabbmitUrl);

    services.AddSingleton(rabbitMQ);

    services.AddDbContextPool<AppDbContext>(options => options.UseMySql(mySqlConnectionStr,
     ServerVersion.AutoDetect(mySqlConnectionStr)));


    // //var requestServices = new RequestServices(new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseMySql(mySqlConnectionStr,
    //   //  ServerVersion.AutoDetect(mySqlConnectionStr)).Options));

     services.AddSingleton<RequestServices>();


    services.AddHostedService<Worker>();


});

var app = build.Build();




app.Run();
