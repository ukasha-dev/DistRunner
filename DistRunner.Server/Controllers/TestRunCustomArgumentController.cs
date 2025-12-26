// <copyright file="TestRunCustomArgumentController.cs" company="Your Company">
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

[Route("api/testruncustomarguments")]
public class TestRunCustomArgumentController : Controller
{
    private readonly ILogger<TestRunCustomArgument> _logger;
    private readonly DISTRUNNERRepository _DISTRUNNERRepository;

    public TestRunCustomArgumentController(ILogger<TestRunCustomArgument> logger, DISTRUNNERRepository repository)
    {
        _logger = logger;
        _DISTRUNNERRepository = repository;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetTestRunCustomArgument([FromBody] int id)
    {
        try
        {
            var testRunCustomArgument = await _DISTRUNNERRepository.GetByIdAsync<TestRunCustomArgument>(id).ConfigureAwait(false);
            if (testRunCustomArgument == null)
            {
                return NotFound();
            }

            var testRunCustomArgumentDto = Mapper.Map<TestRunCustomArgumentDto>(testRunCustomArgument);

            return Ok(testRunCustomArgumentDto);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting test run with id {id}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestRunCustomArguments()
    {
        try
        {
            var testRunCustomArguments = await _DISTRUNNERRepository.GetAllQueryWithRefreshAsync<TestRunCustomArgument>().ConfigureAwait(false);
            var testRunCustomArgumentDtos = Mapper.Map<IEnumerable<TestRunCustomArgumentDto>>(testRunCustomArguments);

            return Ok(testRunCustomArgumentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting logs.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestRunCustomArgumentAsync([FromBody] TestRunCustomArgumentDto testRunCustomArgumentDto)
    {
        if (testRunCustomArgumentDto == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var testRunCustomArgument = Mapper.Map<TestRunCustomArgument>(testRunCustomArgumentDto);

        var result = await _DISTRUNNERRepository.InsertWithSaveAsync(testRunCustomArgument).ConfigureAwait(false);

        var resultDto = Mapper.Map<TestRunCustomArgumentDto>(result);

        return Ok(resultDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestRunCustomArgumentAsync([FromBody] KeyValuePair<int, TestRunCustomArgumentDto> updateObject)
    {
        if (updateObject.Value == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityToBeUpdated = await _DISTRUNNERRepository.GetByIdAsync<TestRunCustomArgument>(updateObject.Key).ConfigureAwait(false);
        if (entityToBeUpdated == null)
        {
            return NotFound();
        }

        entityToBeUpdated = Mapper.Map(updateObject.Value, entityToBeUpdated);
        await _DISTRUNNERRepository.UpdateWithSaveAsync(entityToBeUpdated).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTestRunCustomArgumentAsync([FromBody] int id)
    {
        var entityToBeRemoved = await _DISTRUNNERRepository.GetByIdAsync<TestRunCustomArgument>(id).ConfigureAwait(false);
        if (entityToBeRemoved == null)
        {
            return NotFound();
        }

        await _DISTRUNNERRepository.DeleteWithSaveAsync(entityToBeRemoved).ConfigureAwait(false);

        return NoContent();
    }
}
