// Copyright 2024 Your Company
// Licensed under the Apache License, Version 2.0
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DISTRUNNER.Core.Contracts;
using DISTRUNNER.Infrastructure;
using DISTRUNNER.Model;
using DISTRUNNER.Server.Models;
using DISTRUNNER.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace DISTRUNNER.Server;

public class Startup
{
    public static IConfiguration Configuration { get; private set; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(a => a.EnableEndpointRouting = false).AddNewtonsoftJson();
        services.AddDbContext<TestsRunsContext>();

        // Add Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "DistRunner API",
                Version = "v1",
                Description = "Distributed Test Runner API - Monitor and control your test agents"
            });
        });

        services.AddTransient<DbRepository<TestsRunsContext>, DISTRUNNERRepository>();
        services.AddTransient<DISTRUNNERRepository>();
        services.AddTransient<TestCasesPersistsService>();

        services.AddTransient<IReflectionProvider, ReflectionProvider>();
        services.AddTransient<IDirectoryProvider, DirectoryProvider>();
        services.AddTransient<IPathProvider, PathProvider>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<DISTRUNNER.Core.Contracts.IFileProvider, FileProvider>();
        services.AddTransient<IGuidService, GuidService>();
        services.AddTransient<IXmlSerializer, XmlSerializer>();
        services.AddTransient<IJsonSerializer, JsonSerializer>();
        services.AddTransient<IAssemblyProvider, AssemblyProvider>();
        services.AddTransient<IDistributeLogger, EmptyDistributedLogger>();
        services.AddTransient<ITaskProvider, TaskProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        // Enable Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DistRunner API V1");
            c.RoutePrefix = "swagger";
        });

        // Serve static files for dashboard
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseStatusCodePages();
        app.UseMvc();

        AutoMapper.Mapper.Initialize(cfg =>
        {
            cfg.CreateMap<TestRun, TestRunDto>();
            cfg.CreateMap<TestAgent, TestAgentDto>();
            cfg.CreateMap<TestAgentRun, TestAgentRunDto>();
            cfg.CreateMap<TestRunLog, TestRunLogDto>();
            cfg.CreateMap<Log, LogDto>();
            cfg.CreateMap<TestRunCustomArgument, TestRunCustomArgumentDto>();
            cfg.CreateMap<TestRunOutput, TestRunOutputDto>();
            cfg.CreateMap<TestAgentRunAvailability, TestAgentRunAvailabilityDto>();
            cfg.CreateMap<TestRunAvailability, TestRunAvailabilityDto>();
            cfg.CreateMap<TestCaseHistoryEntry, TestCaseHistoryEntryDto>();
            cfg.CreateMap<TestCaseHistory, TestCaseHistoryDto>();

            cfg.CreateMap<TestRunDto, TestRun>();
            cfg.CreateMap<TestAgentDto, TestAgent>();
            cfg.CreateMap<TestAgentRunDto, TestAgentRun>();
            cfg.CreateMap<TestRunLogDto, TestRunLog>();
            cfg.CreateMap<LogDto, Log>();
            cfg.CreateMap<TestRunCustomArgumentDto, TestRunCustomArgument>();
            cfg.CreateMap<TestRunOutputDto, TestRunOutput>();
            cfg.CreateMap<TestAgentRunAvailabilityDto, TestAgentRunAvailability>();
            cfg.CreateMap<TestRunAvailabilityDto, TestRunAvailability>();
            cfg.CreateMap<TestCaseHistoryEntryDto, TestCaseHistoryEntry>();
            cfg.CreateMap<TestCaseHistoryDto, TestCaseHistory>();
        });
    }
}