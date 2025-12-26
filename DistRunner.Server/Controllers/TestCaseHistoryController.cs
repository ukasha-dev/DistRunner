// <copyright file="TestCaseHistoryController.cs" company="Your Company">
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

[Route("api/testcasesHistory")]
public class TestCaseHistoryController : Controller
{
    private readonly ILogger<TestCaseHistoryController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestCaseHistoryController(ILogger<TestCaseHistoryController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestCaseHistory([FromBody] int id)
    {
        try
        {
            var testCaseHistory = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistory>(id).ConfigureAwait(false);
            if (testCaseHistory == null)
            {
                _logger.LogInformation($"TestCaseHistory with id {id} wasn't found.");
                return NotFound();
            }

            var testCaseHistoryDto = Mapper.Map<TestCaseHistoryDto>(testCaseHistory);

            return Ok(testCaseHistoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestCaseHistorys()
    {
        try
        {
            var testCaseHistorys = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestCaseHistory>().ConfigureAwait(false);
            var testCaseHistoryDto = Mapper.Map<IEnumerable<TestCaseHistoryDto>>(testCaseHistorys);

            return Ok(testCaseHistoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting testCaseHistorys.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestCaseHistoryAsync([FromBody] TestCaseHistoryDto testCaseHistoryDto)
    {
        if (testCaseHistoryDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testCaseHistory = Mapper.Map<TestCaseHistory>(testCaseHistoryDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testCaseHistory).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestCaseHistoryDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestCaseHistoryAsync([FromBody] KeyValuePair<int, TestCaseHistoryDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistory>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestCaseHistoryAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestCaseHistory>(id).ConfigureAwait(false);
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}