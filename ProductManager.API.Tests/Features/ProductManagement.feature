Feature: Product Management


Scenario: Unauthorized product registry when logged as non existing user
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "MOCK_USR" and password "NONE" 
    When adding a product with name "Gadget" and quantity "3"
    Then the answer is "Unauthorized"


Scenario: Duplicate product returns 409 Conflict
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    When adding a product with name "Gadget" and quantity "3"
    Then the answer is "Conflict"

Scenario: Decrements stock with success
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    When decrements "2" units of that product
    Then the answer is "OK"
    And product stock becomes "3"

Scenario: Decrements stock never reaches negative values
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Fruit Basket" and quantity "5"
    When decrements "100" units of that product
    Then the answer is "OK"
    And product stock becomes "0"

Scenario: Increments stock with success
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Chair" and quantity "5"
    When increments "2" units of that product
    Then the answer is "OK"
    And product stock becomes "7"

Scenario: Decrement stock of non existing product returns NotFound 
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Chair" and quantity "5"
    When decrementing "10" units of product with id "999"
    Then the answer is "NotFound"

Scenario: Increment stock of non existing product returns NotFound 
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Chair" and quantity "5"
    When incrementing "10" units of product with id "999"
    Then the answer is "NotFound"

Scenario: Valid search outputs results
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    And exists product with name "Smartwatch" and quantity "3"
    And exists product with name "Bag" and quantity "1"
    And exists product with name "Sonic screwdriver" and quantity "10"
    When searching for a min quantity of "3" and max quantity of "6"
    Then the answer is "OK"
    And has "2" results

Scenario: Valid search outputs no results
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    And exists product with name "Smartwatch" and quantity "3"
    And exists product with name "Bag" and quantity "1"
    And exists product with name "Sonic screwdriver" and quantity "10"
    When searching for a min quantity of "11" and max quantity of "19"
    Then the answer is "OK"
    And has "0" results



Scenario: Search with negative values outputs BadRequest
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    And exists product with name "Smartwatch" and quantity "3"
    And exists product with name "Bag" and quantity "1"
    And exists product with name "Sonic screwdriver" and quantity "10"
    When searching for a min quantity of "-30" and max quantity of "-9"
    Then the answer is "BadRequest"


Scenario: Search with min value bigger than negative value outputs BadRequest
    Given exists a registered user "UT_TEST" with password "UT_PASS"
    And user is logged in with username "UT_TEST" and password "UT_PASS"
    And exists product with name "Gadget" and quantity "5"
    And exists product with name "Smartwatch" and quantity "3"
    And exists product with name "Bag" and quantity "1"
    And exists product with name "Sonic screwdriver" and quantity "10"
    When searching for a min quantity of "10" and max quantity of "1"
    Then the answer is "BadRequest"