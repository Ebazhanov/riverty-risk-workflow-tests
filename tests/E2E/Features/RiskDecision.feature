@allure.label.suite:Risk_Decision_E2E_Suite
Feature: Risk Assessment Workflow
As a Risk Decision Engine
I want to evaluate payment requests
So that bad transactions are blocked

Scenario: Approve low-risk transaction
    Given an external credit bureau returns a low risk score
    When a risk evaluation request is sent for amount 50.00 EUR
    Then the decision status should be "APPROVED"