// <copyright file="TestRunLogController.cs" company="Your Company">
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

[Route("api/testrunlogs")]
public class TestRunLogController : Controller
{
    private readonly ILogger<TestRunLogController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestRunLogController(ILogger<TestRunLogController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestRunLogAsync([FromBody] int id)
    {
        try
        {
            var testRunLog = await _DISTRUNNERRepository.GetByIdAsync<TestRunLog>(id).ConfigureAwait(false);
            if (testRunLog == null)
            {
                return NotFound();
            }

            var testRunLogDto = Mapper.Map<TestRunLogDto>(testRunLog);

            return Ok(testRunLogDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAndDeleteNewTestRunLogsAsync()
    {
        try
        {
            var testRunLogs = (await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRunLog>().ConfigureAwait(false)).Where(x => x.Status == TestRunLogStatus.New);
            var testRunLogDtos = Mapper.Map<IEnumerable<TestRunLogDto>>(testRunLogs);
            await _DISTRUNNERRepository.DeleteRangeWithSaveAsync(testRunLogs).ConfigureAwait(false);

            return Ok(testRunLogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting testRunLogs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestRunLogAsync([FromBody] TestRunLogDto testRunLogDto)
    {
        if (testRunLogDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testRunLog = Mapper.Map<TestRunLog>(testRunLogDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testRunLog).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestRunLogDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestRunLogAsync([FromBody] KeyValuePair<int, TestRunLogDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestRunLog>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestRunLogAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestRunLog>(id).ConfigureAwait(false);
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}
