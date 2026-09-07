Feature: Stock Management

Scenario: Decrements stock with success
    Given exists product with name "Gadget" and quantity "5"
    When decrements "2" units of that product
    Then the answer is "OK"
    And product stock becomes "3"

Scenario: Decrements stock never reaches negative values
    Given exists product with name "Gadget" and quantity "5"
    When decrements "100" units of that product
    Then the answer is "OK"
    And product stock becomes "0"

Scenario: Increments stock with success
    Given exists product with name "Gadget" and quantity "5"
    When increments "2" units of that product
    Then the answer is "OK"
    And product stock becomes "7"

Scenario: Decrement stock of non existing product returns NotFound 
    When decrementing "10" units of product with id "999"
    Then the answer is "NotFound"

Scenario: Increment stock of non existing product returns NotFound 
    When incrementing "10" units of product with id "999"
    Then the answer is "NotFound"