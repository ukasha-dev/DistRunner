// <copyright file="TestAgentRunController.cs" company="Your Company">
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
using DISTRUNNER.Server.Models;
using DISTRUNNER.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DISTRUNNER.Server.Controllers;

[Route("api/testagentruns")]
public class TestAgentRunController : Controller
{
    private readonly ILogger<TestAgentRunController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestAgentRunController(ILogger<TestAgentRunController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestAgentRunAsync([FromBody] int id)
    {
        try
        {
            var testAgentRun = await _DISTRUNNERRepository.GetByIdAsync<TestAgentRun>(id).ConfigureAwait(false);
            if (testAgentRun == null)
            {
                _logger.LogInformation($"TestAgentRun with id {id} wasn't found.");
                return NotFound();
            }

            var testAgentRunDto = Mapper.Map<TestAgentRunDto>(testAgentRun);

            return Ok(testAgentRunDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestAgentRunsAsync()
    {
        try
        {
            var logs = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestAgentRun>().ConfigureAwait(false);
            var logDto = Mapper.Map<IEnumerable<TestAgentRunDto>>(logs);

            return Ok(logDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting logs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestAgentRunAsync([FromBody] TestAgentRunDto logDto)
    {
        if (logDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var log = Mapper.Map<TestAgentRun>(logDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(log).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestAgentRunDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestAgentRunAsync([FromBody] KeyValuePair<int, TestAgentRunDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestAgentRun>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestAgentRunAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestAgentRun>(id).ConfigureAwait(false);

        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}
