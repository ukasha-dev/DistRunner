# DistRunner - Project Viva Preparation Guide

This guide is designed to help you ace your lab project viva. It covers everything from the "elevator pitch" to the technical deep dive and live demonstration.

---

## 🚀 1. The Elevator Pitch (Introduction)
*When the examiner asks: "What is your project about?"*

"**DistRunner** is a distributed test execution framework designed to tackle the problem of long-running test suites. Instead of running hundreds of tests sequentially on one machine, DistRunner splits the workload across multiple 'Agents' (different machines or processes). This allows for **Parallel and Distributed Computing (PDC)**, significantly reducing the total execution time - often from minutes to seconds. It supports popular frameworks like MSTest and NUnit through a flexible plugin architecture."

---

## 🏗️ 2. Architecture & Components
*When asked: "Explain the architecture of your system."*

DistRunner uses a **Hub-and-Spoke (Client-Server)** architecture:

1.  **DistRunner Server (The Hub):** 
    - Acts as the central coordinator.
    - Manages the state of all 'Test Agents'.
    - Provides a **Real-time Dashboard** (Web UI) to monitor progress.
    - Exposes a REST API for communication.
2.  **DistRunner Agents (The Spokes):**
    - These are the workers that actually run the tests.
    - They 'register' themselves with the server using a **Tag** (e.g., "UI-Tests", "API-Tests").
    - They wait for the server to send them a batch of tests to execute.
3.  **DistRunner CLI (The Runner):**
    - The tool used to start a new 'Test Run'.
    - It sends the test DLL and configuration to the Server.

---

## 💻 3. Live Demo Script (Single Machine)
*Follow these steps for a flawless demonstration:*

### Step A: Start the Server (The Brain)
1. Open a terminal in the root directory.
2. Run: `dotnet run --project DistRunner.Server`
3. **Show the Examiner:** Open `http://localhost:89` in your browser. Point out that there are "0 Connected Agents".

### Step B: Start 2 Agents (The Workers)
1. Open **Terminal 2** and run:
   `dotnet run --project DistRunner -- agent --tag="DemoTag" --server="http://localhost:89"`
2. Open **Terminal 3** and run:
   `dotnet run --project DistRunner -- agent --tag="DemoTag" --server="http://localhost:89"`
3. **Show the Examiner:** Refresh the dashboard. You should now see **2 Agents** connected with the tag "DemoTag".

### Step C: Build the Sample Tests
1. In a terminal, run: `dotnet build SmallMSTestTestProject\SmallMSTestTestProject.csproj -c Release`

### Step D: Trigger the Test Run (The Spark)
1. Open **Terminal 4** and run the runner command:
   ```bash
   dotnet run --project DistRunner -- runner --tag="DemoTag" --testTechnology="MSTest" --library="c:\Users\HP\Downloads\DistRunner\DistRunner\SmallMSTestTestProject\bin\Release\net8.0\SmallMSTestTestProject.dll" --results="c:\Users\HP\Downloads\DistRunner\DistRunner\test-results.trx" --server="http://localhost:89"
   ```
2. **Show the Examiner:** Quickly switch to the Dashboard. You'll see the progress bars moving in real-time as the two agents share the 60+ tests!

---

## 🧠 4. Technical Deep Dive (PDC Concepts)
*Crucial for the "Theoretical" part of the viva:*

1.  **Amdahl's Law:** Explain that the total speedup is limited by the "sequential part" of the code (like the server setting up the run). 
    - *Theoretical Max Speedup = 1 / ((1-P) + P/N)* where P is the parallel fraction and N is agents.
2.  **Load Balancing:** Explain the two strategies you implemented:
    - **Count-Based:** Simple, splits tests evenly (e.g., 50 tests each for 2 agents).
    - **Time-Based:** Smarter, uses historical data to give more tests to faster agents or balance by expected duration.
3.  **Synchronization:** The server uses a "Wait Handle" logic. It doesn't finish the 'Test Run' until all Agents report back their results (Barrier Synchronization).
4.  **Fault Tolerance:** If a test fails, the agent can be configured to **Retry** it up to X times.

---

## ❓ 5. Common Viva Questions & Answers

*   **Q: Why use a Dashboard?**
    - A: In a distributed system, you can't see what's happening on 10 different machines at once. The dashboard provides a centralized, real-time "single source of truth."
*   **Q: How do agents know which tests to run?**
    - A: The Server calculates the distribution lists based on the selected strategy and sends a subset of test IDs/Names to each agent via a POST request.
*   **Q: What happens if an agent crashes during a run?**
    - A: The server has a timeout mechanism. If an agent doesn't report back within the `TestRunTimeout` limit, that batch of tests is marked as failed or timed out.
*   **Q: Why use MEF for plugins?**
    - A: It allows us to add support for new test frameworks (like PyTest or Playwright) without changing the core DistRunner code. We just drop a new DLL into the Plugins folder.

---

## 📝 6. Summary of Commands

| Action | Command |
| :--- | :--- |
| **Server** | `dotnet run --project DistRunner.Server` |
| **Agent** | `dotnet run --project DistRunner -- agent --tag="DemoTag" --server="http://localhost:89"` |
| **Runner** | `dotnet run --project DistRunner -- runner --tag="DemoTag" --testTechnology="MSTest" --library="..." --results="..." --server="http://localhost:89"` |
