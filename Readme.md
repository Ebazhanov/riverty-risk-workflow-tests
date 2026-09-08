## Riverty Risk Decision Workflow — SDET Test Automation Suite

[![CI Pipeline](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/ci.yml/badge.svg)](https://github.com/Ebazhanov/riverty-risk-workflow-tests/actions/workflows/ci.yml)

### 🧪 Risk Decision Workflow — Test Execution Matrix
##### [TC-RISK-001: Low Risk Score Approval](https://docs.riverty.com/bnpl/api_reference/#tag/Authorize)
- [x] **Passed** — Integration API & WireMock validation (`EvaluateRisk_LowRiskUser_ShouldApprove`)
- [x] **Passed** — Reqnroll BDD Gherkin scenario validation (`@XRAY-101`)

##### [TC-RISK-002: Hard Decline for Low Credit Score](https://docs.riverty.com/bnpl/api_reference/#tag/Authorize)
- [x] **Passed** — Automatic rejection for high-risk profiles (`EvaluateRisk_HighRiskUser_ShouldReject`)

##### [TC-RISK-003: Boundary Score Evaluation](https://docs.riverty.com/bnpl/api_reference/)
- [ ] Approval Test at Exact Score Boundary (Score: 700 / Amount Threshold)

##### [TC-RISK-004: Credit Limit Exceeded](https://docs.riverty.com/bnpl/api_reference/)
- [ ] Transaction Decline Due to Insufficient Credit Limit

##### [TC-RISK-005: Downstream API Timeout & Resilience](https://docs.riverty.com/bnpl/api_reference/#section/Errors)
- [ ] Graceful Fallback to Manual Review on External Service Latency (WireMock 504 Delay Simulation)

##### [TC-RISK-006: Invalid Bank Details & IBAN Validation](https://docs.riverty.com/bnpl/api_reference/#section/Errors)
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