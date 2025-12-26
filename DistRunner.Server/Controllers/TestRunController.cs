// <copyright file="TestRunController.cs" company="Your Company">
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
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DISTRUNNER.Model;
using DISTRUNNER.Server.Models;
using DISTRUNNER.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DISTRUNNER.Server.Controllers;

[Route("api/testruns")]
public class TestRunController : Controller
{
    private readonly ILogger<TestRunController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestRunController(ILogger<TestRunController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestRunAsync([FromBody] Guid id)
    {
        try
        {
            var testRun = (await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRun>().ConfigureAwait(false)).FirstOrDefault(x => x.TestRunId.Equals(id));
            if (testRun == null)
            {
                return NotFound();
            }

            var testRunDto = Mapper.Map<TestRunDto>(testRun);

            return Ok(testRunDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestRunsAsync()
    {
        try
        {
            var testRuns = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRun>().ConfigureAwait(false);
            var testRunDto = Mapper.Map<IEnumerable<TestRunDto>>(testRuns);

            return Ok(testRunDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting test runs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestRunAsync([FromBody] TestRunDto testRunDto)
    {
        if (testRunDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testRun = Mapper.Map<TestRun>(testRunDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testRun).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestRunDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestRunAsync([FromBody] KeyValuePair<Guid, TestRunDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = (await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRun>().ConfigureAwait(false)).FirstOrDefault(x => x.TestRunId.Equals(updateObject.Key));
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestRun([FromBody] Guid id)
    {
        var entityToBeRemoved = (await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRun>().ConfigureAwait(false)).FirstOrDefault(x => x.TestRunId.Equals(id));
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}
