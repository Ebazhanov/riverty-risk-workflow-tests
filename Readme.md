## Risk Decision Workflow — SDET Test Automation Suite

[![CI Pipeline](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/api-integration-tests.yml/badge.svg)](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/api-integration-tests.yml)
[![Allure Report](https://img.shields.io/badge/Allure%20Report-GitHub%20Pages-1262B5?style=flat&logo=qameta&logoColor=white)](https://ebazhanov.github.io/riverty-risk-workflow-tests/)


### Prerequisites & Local Environment

Ensure Docker Daemon is running locally before executing integration tests:

```fish
# Check Docker status
docker ps

# Run full integration test suite
dotnet test --configuration Release
```

----

### 🧪 Risk Decision Workflow — Test Execution Matrix
##### TC-RISK-001: Low Risk Score Approval
- [x] **Passed** — Integration API & WireMock validation (`EvaluateRisk_LowRiskUser_ShouldApprove`)

##### TC-RISK-002: Hard Decline for Low Credit Score
- [ ] **Passed** — Automatic rejection for high-risk profiles (`EvaluateRisk_HighRiskUser_ShouldReject`)

##### TC-RISK-003: Boundary Score Evaluation
- [ ] Approval Test at Exact Score Boundary (Score: 700 / Amount Threshold)

##### TC-RISK-004: Credit Limit Exceeded
- [ ] Transaction Decline Due to Insufficient Credit Limit

##### TC-RISK-005: Downstream API Timeout & Resilience
- [ ] Graceful Fallback to Manual Review on External Service Latency (WireMock 504 Delay Simulation)

##### TC-RISK-006: Invalid Bank Details & IBAN Validation
- [ ] Input Rejection on Malformed Account Parameters (400 Bad Request)

---

## ⚡ Performance & SLA Validation (k6)

The suite includes load testing for the Risk Decision Workflow to ensure sub-second response times under concurrent load.

* **SLA Threshold:** 95% of API requests must complete in **< 500ms** (`p(95) < 500`).
* **Error Rate Target:** Less than **1%** failure rate under peak traffic.

### Local Execution

Run the performance test script using [k6](https://k6.io/):

```fish
k6 run performance/risk-load-test.js