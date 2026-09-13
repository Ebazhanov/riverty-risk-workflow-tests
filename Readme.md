## Risk Decision Workflow — SDET Test Automation Suite

[![CI Pipeline](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/api-integration-tests.yml/badge.svg)](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/api-integration-tests.yml)
[![Allure Report](https://img.shields.io/badge/Allure%20Report-GitHub%20Pages-1262B5?style=flat&logo=qameta&logoColor=white)](https://ebazhanov.github.io/riverty-risk-workflow-tests/)
[![YouTube Demo](https://img.shields.io/badge/YouTube-Video%20Walkthrough-FF0000?style=flat&logo=youtube&logoColor=white)](https://youtu.be/Nc3x9nabiqM)

### Key Technical Highlights
* **Code Quality & Roslyn Rules:** Maintained high code quality via zero-warning builds and Roslyn rules.
* **Test Resilience & Lifecycle:** Reliable WireMock & HttpClient lifecycle management.
* **Negative & Boundary Coverage:** Latency, negative scenario, and idempotency coverage.
* **Reporting & Traceability:** Allure reporting with full payload logs + Jira/Xray linking.

### Prerequisites & Local Environment

Ensure Docker Daemon is running locally before executing integration tests:

```fish
# Check Docker status
docker ps

# Run full integration test suite
dotnet test --configuration Release
```

----
```text
       /\
      /  \     End-to-End (BDD / Reqnroll)
     /----\    --------------------------------
    /      \   Integration (WireMock + Testcontainers)
   /--------\  ----------------------------------------
  /          \ Component (In-Memory / TestServer)
  
````
### Test Pyramid
* **Component Layer (`tests/Component/`):** Fast in-memory endpoint testing via `CustomWebApplicationFactory` and `Microsoft.AspNetCore.TestHost`.
* **Integration Layer (`tests/Integration/`):** Ephemeral PostgreSQL database validation via `Testcontainers`, downstream API mocking with `WireMock.Net`, and direct database assertions using `Dapper` & `Npgsql`.
* **E2E Layer (`tests/E2E/`):** Business acceptance scenarios in Gherkin (`.feature`) orchestrated by `Reqnroll` (SpecFlow).


#### Level 1: Component Tests (`tests/Component/`)
- [x] `PostEvaluateRisk_InMemoryCall_ReturnsCreatedStatus` — Fast in-memory endpoint validation via `TestServer`

#### Level 2: Integration Tests (`tests/Integration/`)
- [x] `TC-RISK-001` — Low risk score approval (`EvaluateRisk_LowRiskUser_ShouldApprove`)
- [x] `TC-RISK-002` — Hard decline for low credit score (`EvaluateRisk_HighRiskUser_ShouldReject`)
- [ ] `TC-RISK-003` — Boundary score evaluation at exact score limit (700)
- [ ] `TC-RISK-004` — Transaction decline due to exceeded credit limit
- [x] `TC-RISK-005` — Downstream API timeout & resilience (WireMock delay simulation via `EvaluateRisk_ExternalServiceDelay_ShouldHandleGracefully`)
- [ ] `TC-RISK-006` — Invalid bank details & IBAN input rejection (400 Bad Request)

#### Level 3: End-to-End Acceptance (`tests/E2E/`)
- [x] `ApproveLow_RiskTransaction` — Gherkin BDD scenario for approved transaction flow (`RiskDecision.feature`)
- [x] `Evaluate risk for negative amount transaction` — Boundary validation scenario (`RiskDecision.feature`)
- [x] `Ensure idempotency on repeated request execution` — Idempotency check scenario (`RiskDecision.feature`)
---

## Performance & SLA Validation (k6)
> The suite includes load testing for the Risk Decision Workflow to ensure sub-second response times under concurrent load.

* **SLA Threshold:** 95% of API requests must complete in **< 500ms** (`p(95) < 500`).
* **Error Rate Target:** Less than **1%** failure rate under peak traffic.

### Local Execution

Run the performance test script using [k6](https://k6.io/):

```fish
k6 run performance/risk-load-test.js
```

-----
### Directory Structure

```text
    tests/
    ├── Clients/        # HTTP API clients (RiskDecisionApiClient)
    ├── Component/      # Level 1: In-memory tests & WebApplicationFactory
    ├── E2E/            # Level 3: Gherkin feature specs & step bindings
    ├── Integration/    # Level 2: WireMock, Testcontainers & base setup
    └── Common/         # DTO models & cross-cutting logging handlers
```