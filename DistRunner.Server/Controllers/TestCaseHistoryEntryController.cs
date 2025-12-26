// <copyright file="TestCaseHistoryEntryController.cs" company="Your Company">
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
using AutoMapper;
using DISTRUNNER.Model;
using DISTRUNNER.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DISTRUNNER.API.Controllers;

[Route("api/testcaseHistoryEntries")]
public class TestCaseHistoryEntryController : Controller
{
    private readonly ILogger<TestCaseHistoryEntryController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestCaseHistoryEntryController(ILogger<TestCaseHistoryEntryController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestCaseHistoryEntry([FromBody] int id)
    {
        try
        {
            var testCaseHistoryEntry = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistoryEntry>(id).ConfigureAwait(false);
            if (testCaseHistoryEntry == null)
            {
                _logger.LogInformation($"TestCaseHistoryEntry with id {id} wasn't found.");
                return NotFound();
            }

            var testCaseHistoryEntryDto = Mapper.Map<TestCaseHistoryEntryDto>(testCaseHistoryEntry);

            return Ok(testCaseHistoryEntryDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestCaseHistoryEntrys()
    {
        try
        {
            var testCaseHistoryEntrys = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestCaseHistoryEntry>().ConfigureAwait(false);
            var testCaseHistoryEntryDtos = Mapper.Map<IEnumerable<TestCaseHistoryEntryDto>>(testCaseHistoryEntrys);

            return Ok(testCaseHistoryEntryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting test runs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestCaseHistoryEntryAsync([FromBody] TestCaseHistoryEntryDto testCaseHistoryEntryDto)
    {
        if (testCaseHistoryEntryDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testCaseHistoryEntry = Mapper.Map<TestCaseHistoryEntry>(testCaseHistoryEntryDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testCaseHistoryEntry).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestCaseHistoryEntryDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestCaseHistoryEntryAsync([FromBody] KeyValuePair<int, TestCaseHistoryEntryDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistoryEntry>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestCaseHistoryEntryAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistoryEntry>(id).ConfigureAwait(false);
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}