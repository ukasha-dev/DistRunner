// DistRunner Dashboard - JavaScript Application
// =============================================

const API_BASE = '';
let refreshInterval = 10;
let autoRefreshTimer = null;

// Initialize the application
document.addEventListener('DOMContentLoaded', () => {
    initNavigation();
    initTheme();
    loadSettings();
    refreshData();
    startAutoRefresh();
});

// Navigation
function initNavigation() {
    const navItems = document.querySelectorAll('.nav-item[data-page]');
    const menuToggle = document.getElementById('menuToggle');
    const sidebar = document.getElementById('sidebar');

    navItems.forEach(item => {
        item.addEventListener('click', (e) => {
            e.preventDefault();
            const page = item.dataset.page;
            navigateTo(page);

            // Close sidebar on mobile
            if (window.innerWidth <= 1024) {
                sidebar.classList.remove('open');
            }
        });
    });

    menuToggle.addEventListener('click', () => {
        sidebar.classList.toggle('open');
    });

    // Close sidebar when clicking outside
    document.addEventListener('click', (e) => {
        if (window.innerWidth <= 1024 &&
            !sidebar.contains(e.target) &&
            !menuToggle.contains(e.target)) {
            sidebar.classList.remove('open');
        }
    });
}

function navigateTo(page) {
    // Update nav items
    document.querySelectorAll('.nav-item').forEach(item => {
        item.classList.remove('active');
        if (item.dataset.page === page) {
            item.classList.add('active');
        }
    });

    // Update pages
    document.querySelectorAll('.page').forEach(p => {
        p.classList.remove('active');
    });
    document.getElementById(`page-${page}`).classList.add('active');

    // Update title
    const titles = {
        dashboard: 'Dashboard',
        agents: 'Test Agents',
        runs: 'Test Runs',
        history: 'History',
        settings: 'Settings'
    };
    document.getElementById('pageTitle').textContent = titles[page] || 'Dashboard';
}

// Theme
function initTheme() {
    const savedTheme = localStorage.getItem('theme') || 'dark';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeToggle(savedTheme);
}

function toggleTheme() {
    const current = document.documentElement.getAttribute('data-theme');
    const newTheme = current === 'dark' ? 'light' : 'dark';
    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    updateThemeToggle(newTheme);
}

function updateThemeToggle(theme) {
    const toggle = document.querySelector('.theme-toggle');
    toggle.textContent = theme === 'dark' ? '🌙' : '☀️';
}

// Settings
function loadSettings() {
    const savedInterval = localStorage.getItem('refreshInterval');
    if (savedInterval) {
        refreshInterval = parseInt(savedInterval);
        document.getElementById('refreshInterval').value = refreshInterval;
    }

    const savedTheme = localStorage.getItem('theme') || 'dark';
    document.getElementById('themeSelect').value = savedTheme;
}

function saveSettings() {
    refreshInterval = parseInt(document.getElementById('refreshInterval').value) || 10;
    localStorage.setItem('refreshInterval', refreshInterval);

    const theme = document.getElementById('themeSelect').value;
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
    updateThemeToggle(theme);

    // Restart auto-refresh with new interval
    startAutoRefresh();

    showNotification('Settings saved successfully!');
}

function showNotification(message) {
    // Simple notification - could be enhanced
    alert(message);
}

// Auto Refresh
function startAutoRefresh() {
    if (autoRefreshTimer) {
        clearInterval(autoRefreshTimer);
    }
    autoRefreshTimer = setInterval(refreshData, refreshInterval * 1000);
}

// Data Fetching
async function fetchAgents() {
    try {
        const response = await fetch(`${API_BASE}/api/testagents`);
        if (!response.ok) throw new Error('Failed to fetch agents');
        return await response.json();
    } catch (error) {
        console.error('Error fetching agents:', error);
        return [];
    }
}

async function fetchTestRuns() {
    try {
        const response = await fetch(`${API_BASE}/api/testruns`);
        if (!response.ok) throw new Error('Failed to fetch test runs');
        return await response.json();
    } catch (error) {
        console.error('Error fetching test runs:', error);
        return [];
    }
}

async function fetchLogs() {
    try {
        const response = await fetch(`${API_BASE}/api/log`);
        if (!response.ok) throw new Error('Failed to fetch logs');
        return await response.json();
    } catch (error) {
        console.error('Error fetching logs:', error);
        return [];
    }
}

// Utilities
function formatDate(dateStr) {
    if (!dateStr) return 'N/A';
    const date = new Date(dateStr);
    return date.toLocaleString();
}

function formatRelativeTime(dateStr) {
    if (!dateStr) return 'N/A';
    const date = new Date(dateStr);
    const now = new Date();
    const diff = now - date;

    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (days > 0) return `${days}d ago`;
    if (hours > 0) return `${hours}h ago`;
    if (minutes > 0) return `${minutes}m ago`;
    return 'Just now';
}

function getStatusClass(status) {
    // Handle numeric status (enum values)
    if (typeof status === 'number') {
        // TestRunStatus: 0=InProgress, 1=Completed, 2=Aborted
        // TestAgentStatus: 0=Inactive, 1=Active, etc.
        switch (status) {
            case 0: return 'running';    // InProgress
            case 1: return 'active';     // Completed/Active
            case 2: return 'failed';     // Aborted
            default: return 'active';
        }
    }
    // Handle string status
    const statusLower = (status || '').toString().toLowerCase();
    if (statusLower.includes('completed') || statusLower.includes('passed') || statusLower.includes('active')) return 'active';
    if (statusLower.includes('running') || statusLower.includes('inprogress') || statusLower.includes('progress')) return 'running';
    if (statusLower.includes('idle') || statusLower.includes('waiting')) return 'idle';
    if (statusLower.includes('fail') || statusLower.includes('error') || statusLower.includes('abort')) return 'failed';
    return 'active';
}

function getStatusText(status) {
    // Handle numeric status (enum values)
    if (typeof status === 'number') {
        // TestRunStatus: 0=InProgress, 1=Completed, 2=Aborted
        switch (status) {
            case 0: return 'In Progress';
            case 1: return 'Completed';
            case 2: return 'Aborted';
            default: return 'Unknown';
        }
    }
    return status || 'Unknown';
}

// Refresh Data
async function refreshData() {
    try {
        console.log('Refreshing data...');
        const agents = await fetchAgents();
        const testRuns = await fetchTestRuns();

        console.log('Agents fetched:', agents);
        console.log('Test runs fetched:', testRuns);

        updateDashboard(agents, testRuns);
        updateAgentsPage(agents);
        updateTestRunsPage(testRuns);
        updateHistoryPage(testRuns);

        document.getElementById('lastUpdated').textContent = `Last updated: ${new Date().toLocaleTimeString()}`;
    } catch (error) {
        console.error('Error refreshing data:', error);
    }
}

function updateDashboard(agents, testRuns) {
    // Update stats
    document.getElementById('totalAgents').textContent = agents.length;
    document.getElementById('activeAgents').textContent = agents.filter(a =>
        getStatusClass(a.status) === 'active' || a.agentStatus === 1).length;
    document.getElementById('totalRuns').textContent = testRuns.length;
    document.getElementById('runningTests').textContent = testRuns.filter(r =>
        getStatusClass(r.status) === 'running').length;

    // Update agent count badge
    document.getElementById('agentCount').textContent = agents.length;
    document.getElementById('activityCount').textContent = testRuns.length;

    // Update agents list
    const agentsList = document.getElementById('agentsList');
    if (agents.length === 0) {
        agentsList.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">🖥️</div>
                <p>No agents connected</p>
                <p style="font-size: 13px; margin-top: 8px;">Start an agent to see it here</p>
            </div>
        `;
    } else {
        agentsList.innerHTML = agents.slice(0, 5).map(agent => `
            <div class="agent-item">
                <div class="agent-avatar">🖥️</div>
                <div class="agent-info">
                    <div class="agent-name">${agent.agentId || agent.testAgentId || 'Agent'}</div>
                    <div class="agent-machine">${agent.machineName || 'Unknown machine'}</div>
                </div>
                <span class="status ${getStatusClass(agent.status)}">${getStatusText(agent.status)}</span>
            </div>
        `).join('');
    }

    // Update activity list
    const activityList = document.getElementById('activityList');
    if (testRuns.length === 0) {
        activityList.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">📊</div>
                <p>No test runs yet</p>
                <p style="font-size: 13px; margin-top: 8px;">Run your first test to see activity</p>
            </div>
        `;
    } else {
        activityList.innerHTML = testRuns.slice(0, 5).map(run => `
            <div class="agent-item">
                <div class="agent-avatar" style="background: ${getStatusClass(run.status) === 'failed' ? 'linear-gradient(135deg, #f44336, #e91e63)' : 'linear-gradient(135deg, #00e676, #00c853)'}">
                    ${getStatusClass(run.status) === 'failed' ? '✕' : '✓'}
                </div>
                <div class="agent-info">
                    <div class="agent-name">Test Run #${run.testRunId || run.id}</div>
                    <div class="agent-machine">${formatRelativeTime(run.dateStarted)}</div>
                </div>
                <span class="status ${getStatusClass(run.status)}">${getStatusText(run.status)}</span>
            </div>
        `).join('');
    }
}

function updateAgentsPage(agents) {
    const tbody = document.getElementById('agentsTableBody');

    if (agents.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" class="empty-state">
                    <div class="empty-icon">🖥️</div>
                    <p>No agents connected</p>
                    <p style="font-size: 13px; margin-top: 8px;">
                        Run: <code>distrunner agent --tag="YourTag" --server="http://localhost:89"</code>
                    </p>
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = agents.map(agent => `
        <tr>
            <td><strong>${agent.agentId || agent.testAgentId || 'Agent'}</strong></td>
            <td>${agent.machineName || 'Unknown'}</td>
            <td><span class="tag">${agent.agentTag || 'default'}</span></td>
            <td><span class="status ${getStatusClass(agent.status)}">${getStatusText(agent.status)}</span></td>
            <td>${formatRelativeTime(agent.lastStatusUpdate)}</td>
            <td>
                <button class="btn btn-sm btn-secondary" onclick="viewAgentDetails('${agent.testAgentId}')">View</button>
            </td>
        </tr>
    `).join('');
}

function updateTestRunsPage(testRuns) {
    const tbody = document.getElementById('testRunsTableBody');

    if (testRuns.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" class="empty-state">
                    <div class="empty-icon">🧪</div>
                    <p>No test runs yet</p>
                    <p style="font-size: 13px; margin-top: 8px;">
                        Run: <code>distrunner runner --tag="YourTag" --testTechnology="MSTest" ...</code>
                    </p>
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = testRuns.map(run => {
        // Calculate duration if both dates are available
        let duration = '--';
        if (run.dateStarted && run.dateFinished) {
            const start = new Date(run.dateStarted);
            const end = new Date(run.dateFinished);
            const diffMs = end - start;
            const diffSec = Math.floor(diffMs / 1000);
            const diffMin = Math.floor(diffSec / 60);
            if (diffMin > 0) {
                duration = `${diffMin}m ${diffSec % 60}s`;
            } else {
                duration = `${diffSec}s`;
            }
        }

        return `
        <tr>
            <td><strong>#${run.testRunId || run.id}</strong></td>
            <td><span class="status ${getStatusClass(run.status)}">${getStatusText(run.status)}</span></td>
            <td>${run.testAssemblyName || '--'}</td>
            <td>${run.testTechnology || '--'}</td>
            <td>${formatDate(run.dateStarted)}</td>
            <td>${duration}</td>
        </tr>
    `;
    }).join('');
}

function updateHistoryPage(testRuns) {
    const timeline = document.getElementById('historyTimeline');

    if (testRuns.length === 0) {
        timeline.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">📜</div>
                <p>No history available</p>
            </div>
        `;
        return;
    }

    timeline.innerHTML = testRuns.slice(0, 20).map(run => {
        const isSuccess = getStatusClass(run.status) !== 'failed';
        return `
            <div class="timeline-item">
                <div class="timeline-icon ${isSuccess ? 'success' : 'error'}">
                    ${isSuccess ? '✓' : '✕'}
                </div>
                <div class="timeline-content">
                    <div class="timeline-title">Test Run #${run.testRunId || run.id} - ${getStatusText(run.status)}</div>
                    <div class="timeline-time">${formatDate(run.dateStarted)}</div>
                </div>
            </div>
        `;
    }).join('');
}

function viewAgentDetails(agentId) {
    alert(`Agent details for: ${agentId}\n\nThis feature will be expanded in future updates.`);
}

// Export for global access
window.refreshData = refreshData;
window.toggleTheme = toggleTheme;
window.saveSettings = saveSettings;
window.viewAgentDetails = viewAgentDetails;
