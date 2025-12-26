// <copyright file="ITestRunProvider.cs" company="Your Company">
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
using System.Collections.Generic;
using System.Threading.Tasks;
using DISTRUNNER.Core.Model;

namespace DISTRUNNER.Core.Contracts;

public interface ITestRunProvider
{
    Task CompleteTestRunAsync(Guid testRunId, TestRunStatus testRunStatus);

    Task<Guid> CreateNewTestRunAsync(
        string testAssemblyName,
        byte[] outputFilesZip,
        int retriesCount,
        double threshold,
        bool runInParallel,
        int maxParallelProcessesCount,
        string nativeArguments,
        string testTechnology,
        bool isTimeBasedBalance,
        bool sameMachineByClass,
        IEnumerable<string> customArgumentsPairs = null);
}