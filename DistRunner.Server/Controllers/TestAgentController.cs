// <copyright file="TestAgentController.cs" company="Your Company">
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

[Route("api/testagents")]
public class TestAgentController : Controller
{
    private readonly ILogger<TestAgentController> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestAgentController(ILogger<TestAgentController> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestAgent([FromBody] int id)
    {
        try
        {
            var testAgent = await _DISTRUNNERRepository.GetByIdAsync<TestAgent>(id).ConfigureAwait(false);
            if (testAgent == null)
            {
                _logger.LogInformation($"TestAgent with id {id} wasn't found.");
                return NotFound();
            }

            var testAgentDto = Mapper.Map<TestAgentDto>(testAgent);

            return Ok(testAgentDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestAgents()
    {
        try
        {
            var testAgents = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestAgent>().ConfigureAwait(false);
            var testAgentDtos = Mapper.Map<IEnumerable<TestAgentDto>>(testAgents);

            return Ok(testAgentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting test runs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestAgentAsync([FromBody] TestAgentDto testAgentDto)
    {
        if (testAgentDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testAgent = Mapper.Map<TestAgent>(testAgentDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testAgent).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestAgentDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestAgentAsync([FromBody] KeyValuePair<int, TestAgentDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestAgent>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestAgentAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestAgent>(id).ConfigureAwait(false);
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}
