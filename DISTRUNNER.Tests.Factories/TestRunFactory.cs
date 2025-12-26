// <copyright file="TestRunFactory.cs" company="Your Company">
// Copyright 2024 Your Company
// Licensed under the Apache License, Version 2.0 (the "License");
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>Your Name</author>
// <site>https://Your Company.solutions/</site>
using System;
using System.Linq;
using AutoFixture;
using DISTRUNNER.Core.Model;
using DISTRUNNER.Server.Models;

namespace DISTRUNNER.Tests.Factories;

public static class TestRunFactory
{
    public static IQueryable<TestRunDto> CreateSingleInProgress(Guid testRunId)
    {
        var fixture = FixtureFactory.Create();

        fixture.Register(() => TestRunStatus.InProgress);
        fixture.Customize<TestRunDto>(tr => tr.With(x => x.TestRunId, testRunId));

        var result = fixture.CreateMany<TestRunDto>(1).AsQueryable();

        return result;
    }

    public static IQueryable<TestRunDto> CreateMany()
    {
        var fixture = FixtureFactory.Create();

        var result = fixture.CreateMany<TestRunDto>().AsQueryable();

        return result;
    }

    public static IQueryable<TestRunDto> CreateMany(Guid testRunId)
    {
        var fixture = FixtureFactory.Create();
        fixture.Customize<TestRunDto>(tr => tr.With(x => x.TestRunId, testRunId));

        var result = fixture.CreateMany<TestRunDto>().AsQueryable();

        return result;
    }
}