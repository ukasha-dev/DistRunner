// <copyright file="TestRunsCleanerController.cs" company="Your Company">
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
using System.Threading.Tasks;
using DISTRUNNER.Model;
using DISTRUNNER.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DISTRUNNER.Server.Controllers;

[Route("api/testrunscleaner")]
public class TestRunsCleanerController : Controller
{
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestRunsCleanerController(DISTRUNNERRepository repository) => _DISTRUNNERRepository = repository;

    [HttpDelete("testRun")]
    public async Task<IActionResult> DeleteOldTestRunDataByTestRunIdAsync([FromBody] Guid id)
    {
        try
        {
            var testRun = _DISTRUNNERRepository.GetAllQuery<TestRun>().FirstOrDefault(r => r.TestRunId.Equals(id));
            if (testRun != null)
            {
                var testRunOutputs = _DISTRUNNERRepository.GetAllQuery<TestRunOutput>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunOutputs);
                var testRunCustomArguments = _DISTRUNNERRepository.GetAllQuery<TestRunCustomArgument>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunCustomArguments);
                var testAgentRuns = _DISTRUNNERRepository.GetAllQuery<TestAgentRun>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                DeleteTestAgentRunAvailabilitiesForTestAgentRuns(testAgentRuns);
                _DISTRUNNERRepository.DeleteRange(testAgentRuns);
                var testRunLogs = _DISTRUNNERRepository.GetAllQuery<TestRunLog>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunLogs);
                _DISTRUNNERRepository.GetAllQuery<TestRunAvailability>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunLogs);
                await _DISTRUNNERRepository.DeleteWithSaveAsync(testRun).ConfigureAwait(false);
                await _DISTRUNNERRepository.SaveAsync().ConfigureAwait(false);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            // Ignore.
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOldTestRunsDataAsync()
    {
        try
        {
            var testRuns = (await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRun>().ConfigureAwait(false)).Where(r => r.Status != TestRunStatus.InProgress || (r.Status == TestRunStatus.InProgress && r.DateStarted < DateTime.Now.AddDays(1)));
            foreach (var testRun in testRuns)
            {
                var testRunOutputs = _DISTRUNNERRepository.GetAllQuery<TestRunOutput>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunOutputs);
                var testRunCustomArguments = _DISTRUNNERRepository.GetAllQuery<TestRunCustomArgument>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunCustomArguments);
                var testAgentRuns = _DISTRUNNERRepository.GetAllQuery<TestAgentRun>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                DeleteTestAgentRunAvailabilitiesForTestAgentRuns(testAgentRuns);

                _DISTRUNNERRepository.DeleteRange(testAgentRuns);
                var testRunLogs = _DISTRUNNERRepository.GetAllQuery<TestRunLog>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunLogs);

                _DISTRUNNERRepository.GetAllQuery<TestRunAvailability>().Where(x => x.TestRunId.Equals(testRun.TestRunId));
                _DISTRUNNERRepository.DeleteRange(testRunLogs);
            }

            _DISTRUNNERRepository.DeleteRange(testRuns);
            await _DISTRUNNERRepository.SaveAsync().ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Ignore.
        }

        return NoContent();
    }

    private void DeleteTestAgentRunAvailabilitiesForTestAgentRuns(IQueryable<TestAgentRun> testAgentRuns)
    {
        foreach (var testAgent in testAgentRuns)
        {
            var testAgentRunAvailabilities = _DISTRUNNERRepository.GetAllQuery<TestAgentRunAvailability>().Where(x => x.TestAgentRunId.Equals(testAgent.TestAgentId));
            _DISTRUNNERRepository.DeleteRange(testAgentRunAvailabilities);
        }
    }
}
