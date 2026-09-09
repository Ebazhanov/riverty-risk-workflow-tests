@allure.label.suite:Risk_Decision_E2E_Suite
Feature: Risk Assessment Workflow
As a risk system, I want to evaluate transaction requests 
so that fraudulent or high-risk payments are blocked.

@XRAY-1024
@allure.issue:XRAY-1024
Scenario: Approve low-risk transaction
    Given an external credit bureau returns a low risk score
    When a risk evaluation request is sent for amount 50.00 EUR
    Then the decision status should be "APPROVED"

@XRAY-1026
@allure.issue:XRAY-1026
Scenario: Evaluate risk for negative amount transaction
    Given an external credit bureau returns a low risk score
    When a risk evaluation request is sent for amount -50.00 EUR
    Then the decision status should be "APPROVED"

@XRAY-1027
@allure.issue:XRAY-1027
Scenario: Ensure idempotency on repeated request execution
    Given an external credit bureau returns a low risk score
    When a risk evaluation request is sent for amount 50.00 EUR
    And the same risk evaluation request is sent again
    Then the decision status should be "APPROVED"