// <copyright file="DumpModeOptions.cs" company="Your Company">
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
using CommandLine;

namespace DISTRUNNER;

[Verb("dump", HelpText = "Creates a file dump of the test execution logs from all test agents.")]
public class DumpModeOptions
{
    [Option('p', "path", HelpText = "Specifies the dump file's location.")]
    public string DumpPath { get; set; }

    [Option('u', "server", HelpText = "The test server URL with port that will be used by the test agents and runners to communicate between the machines.")]
    public string serverUrl { get; set; }
}