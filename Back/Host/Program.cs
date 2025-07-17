using Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using Model.Interfaces.Repositories;
using Model.Interfaces.Services;
using Model.Services;
using Todo.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories;
using AutoMapper;

namespace Host;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var mysqlDbSettings = new MysqlDbSettings
        {
            ConnectionString = "Server=localhost;Database=TodoList;User=root;Password=root;",
            DatabaseName = "TodoList"
        };

        builder.Services.AddMySqlDbCollections(mysqlDbSettings);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<ISubTaskService, SubTaskService>();
        builder.Services.AddScoped<ISubTaskRepository, SubTaskRepository>();
        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        var app = builder.Build();

        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
        //}
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
