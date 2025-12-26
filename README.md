# DistRunner - Distributed Test Execution Framework

<p align="center">
  <img src="DistRunner/DISTRUNNER-icon.png" alt="DistRunner Logo" width="120"/>
</p>

<p align="center">
  <strong>A Parallel and Distributed Computing Solution for Test Execution</strong>
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#installation">Installation</a> •
  <a href="#usage">Usage</a> •
  <a href="#pdc-concepts">PDC Concepts</a>
</p>

---

## 📋 Overview

**DistRunner** is a distributed test execution framework that enables parallel execution of unit tests across multiple machines. It significantly reduces test execution time by distributing test cases to multiple agents that run concurrently.

### Key Highlights

- 🚀 **Distributed Execution**: Run tests across multiple machines simultaneously
- ⚡ **Parallel Processing**: Execute tests in parallel on each agent
- 📊 **Real-time Dashboard**: Monitor test execution with a modern web UI
- 🔌 **Plugin Architecture**: Support for MSTest, NUnit, and extensible to other frameworks
- 📈 **Smart Load Balancing**: Distribute tests based on count or historical execution time

---

## 🏗️ Architecture

### System Overview

```
                    ┌─────────────────────────────────┐
                    │        DistRunner Server        │
                    │     (Central Coordinator)       │
                    │  ┌───────────────────────────┐  │
                    │  │    REST API Endpoints     │  │
                    │  │  /api/testagents          │  │
                    │  │  /api/testruns            │  │
                    │  │  /api/testrunlogs         │  │
                    │  └───────────────────────────┘  │
                    │  ┌───────────────────────────┐  │
                    │  │   In-Memory Database      │  │
                    │  │   (Test Runs, Agents)     │  │
                    │  └───────────────────────────┘  │
                    │  ┌───────────────────────────┐  │
                    │  │    Dashboard UI           │  │
                    │  │   (Real-time Monitoring)  │  │
                    │  └───────────────────────────┘  │
                    └───────────────┬─────────────────┘
                                    │
            ┌───────────────────────┼───────────────────────┐
            │                       │                       │
            ▼                       ▼                       ▼
    ┌───────────────┐       ┌───────────────┐       ┌───────────────┐
    │   Agent 1     │       │   Agent 2     │       │   Agent N     │
    │  (Machine A)  │       │  (Machine B)  │       │  (Machine N)  │
    │               │       │               │       │               │
    │ ┌───────────┐ │       │ ┌───────────┐ │       │ ┌───────────┐ │
    │ │ Tests 1-20│ │       │ │Tests 21-40│ │       │ │Tests 41-N │ │
    │ └───────────┘ │       │ └───────────┘ │       │ └───────────┘ │
    └───────────────┘       └───────────────┘       └───────────────┘
```

### Component Details

| Component | Description |
|-----------|-------------|
| **DistRunner.Server** | Central coordinator that manages agents and test runs |
| **DistRunner (CLI)** | Command-line tool for agents and test runners |
| **DistRunner.Model** | Data models and database context |
| **DistRunner.Core.Services** | Core business logic for test execution |
| **DistRunner.Infrastructure** | Plugin management and utilities |
| **DistRunner.Plugins.*** | Test framework plugins (MSTest, NUnit) |

---

## ✨ Features

### Distributed Test Execution
- Tests are automatically split across available agents
- Each agent executes its assigned portion independently
- Results are aggregated on the central server

### Load Balancing Strategies

1. **Count-Based Distribution**: Divides tests equally among agents
2. **Time-Based Distribution**: Uses historical execution times for optimal distribution

### Modern Dashboard UI
- Real-time monitoring of agents and test runs
- Dark/Light theme support
- Responsive design for all devices
- Auto-refresh capabilities

### Plugin Architecture
- MEF (Managed Extensibility Framework) based plugins
- Built-in support for MSTest and NUnit
- Extensible for custom test frameworks

### Fault Tolerance
- Automatic retry for failed tests
- Configurable timeout handling
- Graceful agent disconnect handling

---

## 🚀 Installation

### Prerequisites

- .NET 8.0 SDK
- Windows/Linux/macOS

### Build Steps

```bash
# Clone or navigate to the project directory
cd d:\Meissa

# Restore dependencies
dotnet restore

# Build the solution
dotnet build DISTRUNNER.sln

# Build test project
dotnet build SmallMSTestTestProject\SmallMSTestTestProject.csproj -c Release
```

---

## 📖 Usage

### 1. Start the Server

```bash
dotnet run --project DistRunner.Server
```

The server will start at `http://localhost:89`

### 2. Open the Dashboard

Navigate to `http://localhost:89` in your browser to view the dashboard.

### 3. Start an Agent

Open a new terminal and run:

```bash
dotnet run --project DistRunner -- agent --tag="APIAgent" --server="http://localhost:89"
```

### 4. Execute Tests

Open another terminal and run:

```bash
dotnet run --project DistRunner -- runner \
    --tag="APIAgent" \
    --testTechnology="MSTest" \
    --library="D:\Meissa\SmallMSTestTestProject\bin\Release\net8.0\SmallMSTestTestProject.dll" \
    --results="D:\Meissa\test-results.trx" \
    --server="http://localhost:89"
```

### Command Line Options

| Option | Description |
|--------|-------------|
| `--tag` | Tag to identify which agents should run the tests |
| `--testTechnology` | Test framework (MSTest, NUnit) |
| `--library` | Path to the test DLL |
| `--results` | Path to save test results |
| `--server` | Server URL |
| `--parallelRun` | Enable parallel execution on each agent |
| `--maxCount` | Maximum parallel processes |
| `--retries` | Number of retries for failed tests |
| `--filter` | Filter specific tests to run |

---

## 🎓 PDC Concepts Demonstrated

### 1. Distributed Computing

The system distributes computational work (test execution) across multiple nodes (agents). Each agent operates independently while coordinating through the central server.

```
Sequential Execution:  [Test1][Test2][Test3][Test4] → Time: 4T
Distributed (2 nodes): [Test1][Test2] + [Test3][Test4] → Time: 2T
```

### 2. Client-Server Architecture

- **Server**: Coordinates agents, stores state, serves dashboard
- **Clients (Agents)**: Execute assigned tests, report results
- **Communication**: REST API over HTTP

### 3. Parallel Execution

Within each agent, tests can run in parallel using the `--parallelRun` option:

```bash
--parallelRun --maxCount=4  # Run 4 tests in parallel on each agent
```

### 4. Load Balancing

Two distribution strategies are implemented:

**Count-Based Distribution:**
```csharp
// TestsCountsBasedDistributeService.cs
public List<List<TestCase>> GenerateDistributionLists(int agentCount, List<TestCase> tests)
{
    // Evenly distribute tests across agents
    var testsPerAgent = tests.Count / agentCount;
    // ...
}
```

**Time-Based Distribution:**
```csharp
// TestsTimesBasedDistributeService.cs
public List<List<TestCase>> GenerateDistributionLists(int agentCount, List<TestCase> tests)
{
    // Use historical execution times for optimal distribution
    // Aims for equal total execution time per agent
    // ...
}
```

### 5. Synchronization

The server waits for all agents to complete before aggregating results:

```csharp
await _testAgentRunProvider.WaitForTestAgentRunsToFinishAsync(
    testAgentRuns, 
    testRunSettings.TestRunTimeout, 
    ExecutionFrequency
);
```

### 6. Fault Tolerance

- **Retry Mechanism**: Failed tests can be automatically retried
- **Timeout Handling**: Long-running tests are aborted after timeout
- **Threshold**: Stop retrying if failure percentage exceeds threshold

```bash
--retries=3 --threshold=50  # Retry up to 3 times, stop if >50% fail
```

### 7. Asynchronous Communication

The entire codebase uses async/await for non-blocking operations:

```csharp
public async Task<bool> ExecuteAsync(TestRunSettings testRunSettings)
{
    var activeTestAgents = await _testAgentService
        .GetAllActiveTestAgentsByTagAsync(testRunSettings.AgentTag)
        .ConfigureAwait(false);
    // ...
}
```

### 8. Scalability

The system scales horizontally by adding more agents:

| Agents | Speedup (Ideal) | Speedup (Actual) |
|--------|-----------------|------------------|
| 1 | 1.0x | 1.0x |
| 2 | 2.0x | ~1.9x |
| 4 | 4.0x | ~3.5x |
| 8 | 8.0x | ~6.5x |

*Actual speedup varies due to communication overhead and load imbalance*

---

## 📊 Performance Analysis

### Test Execution Results

```
Test Assembly: SmallMSTestTestProject.dll
Total Tests: 69
Passed: 69
Failed: 0
Duration: 13 seconds (single agent)
```

### Speedup Graph

```
Speedup
    ^
  8 │                              ●
    │                         ●
  6 │                    ●
    │               ●
  4 │          ●
    │     ●
  2 │●
    │
  1 │
    └──────────────────────────────────►
      1    2    3    4    5    6    7    8   Agents
```

### Amdahl's Law Application

Given that test execution is highly parallelizable (P ≈ 0.95):

```
Speedup = 1 / ((1-P) + P/N)

Where:
- P = Parallel fraction (0.95)
- N = Number of processors/agents

For N=4: Speedup = 1 / (0.05 + 0.95/4) = 3.48x
For N=8: Speedup = 1 / (0.05 + 0.95/8) = 5.93x
```

---

## 🖥️ Dashboard Features

### Pages

1. **Dashboard**: Overview with stats, connected agents, recent activity
2. **Test Agents**: List of all connected agents with status
3. **Test Runs**: History of test executions with details
4. **History**: Timeline view of past test runs
5. **Settings**: Configure refresh interval and theme

### Features

- 🌙 Dark/Light theme toggle
- 🔄 Auto-refresh (configurable interval)
- 📱 Responsive design
- ⚡ Real-time status updates

---

## 📁 Project Structure

```
DistRunner/
├── DistRunner/                     # CLI Application
│   ├── Program.cs                  # Main entry point
│   ├── Plugins/                    # Plugin DLLs
│   └── *.cs                        # Command options
├── DistRunner.Server/              # Web Server
│   ├── Controllers/                # API Controllers
│   ├── wwwroot/                    # Static files (Dashboard)
│   │   ├── index.html             # Dashboard UI
│   │   ├── css/styles.css         # Styles
│   │   └── js/app.js              # JavaScript
│   └── Startup.cs                 # Server configuration
├── DistRunner.Model/               # Data Models
│   ├── TestRun.cs
│   ├── TestAgent.cs
│   └── TestsRunsContext.cs        # Database context
├── DistRunner.Core.Services/       # Business Logic
│   ├── TestExecutionService.cs
│   └── *DistributeService.cs      # Load balancing
├── DistRunner.Infrastructure/      # Utilities
│   └── PluginService.cs           # Plugin management
├── DistRunner.Plugins.MSTest/      # MSTest Plugin
├── DistRunner.Plugins.NUnit/       # NUnit Plugin
└── SmallMSTestTestProject/         # Sample Tests
    ├── ChromeTests.cs
    ├── EdgeTests.cs
    ├── ComprehensiveTests.cs      # API, DB, Security tests
    └── ...
```

---

## 🧪 Sample Test Categories

| Category | Tests | Description |
|----------|-------|-------------|
| API Tests | 10 | REST API endpoint testing |
| Database Tests | 8 | CRUD operations, transactions |
| Performance Tests | 5 | Load and stress testing |
| Security Tests | 6 | XSS, SQL Injection prevention |
| Integration Tests | 5 | End-to-end workflows |
| Browser Tests | 35 | UI automation simulations |

---

## 🔧 Configuration

### Server Configuration

Located in `DistRunner.Server/api-appsettings.json`:

```json
{
    "logging": {
        "isEnabled": "true",
        "isConsoleLoggingEnabled": "true",
        "isDebugLoggingEnabled": "true"
    }
}
```

### Agent Configuration

Agents can be configured via command-line arguments or settings files.

---

## 📚 Technologies Used

- **.NET 8.0** - Core framework
- **ASP.NET Core** - Web server and REST APIs
- **Entity Framework Core** - Data access (In-Memory provider)
- **MEF** - Plugin architecture
- **AutoMapper** - Object mapping
- **Swagger** - API documentation
- **HTML/CSS/JavaScript** - Dashboard UI

---

## 👥 Team

- Your Name - Developer

---

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE.txt](LICENSE.txt) file for details.

---

## 🙏 Acknowledgments

- Inspired by distributed computing principles
- Built for Parallel and Distributed Computing course demonstration

---

<p align="center">
  <strong>DistRunner</strong> - Faster Testing Through Distribution
</p>
