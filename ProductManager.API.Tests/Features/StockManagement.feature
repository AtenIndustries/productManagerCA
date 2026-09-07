Feature: Stock Management

Scenario: Decrements stock with success
    Given exists product with name "Gadget" and quantity "5"
    When decrements "2" units of that product
    Then the answer is "OK"
    And product stock becomes "3"


Scenario: Increments stock with success
    Given exists product with name "Gadget" and quantity "5"
    When increments "2" units of that product
    Then the answer is "OK"
    And product stock becomes "7"