Feature: Education Functionality
  As a user, I want to log in to the Mars portal application to create, edit
  and delete education functionality. I would be able to show what education I have

  Background: 
    Given I sign in to the profile page with valid username and password

  # Adding New Education
  Scenario: Create new Education with valid data from JSON
    When I enter education details from the JSON file and save
    Then The Education should be created and listed successfully

  # Deleting All Education entries
  @Order3
  Scenario: Delete all existing education entries
  When I delete all education entries in my profile and successful message should appear
  Then The deleted education entries should not appear in the list