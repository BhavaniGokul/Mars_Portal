﻿Feature: Language Functionality
  As a user, I want to log in to the Mars portal application to create, edit
  and delete language functionality.  I would be able to show what languages I know

  Background: 
  Given I sign in to the profile page with valid username and password

  # Adding New Language and level
  @Order1
  Scenario Outline: Create new language and level record with valid data
    When I create a new '<Language>' and '<Level>' in my profile
    Then The '<Language>' and '<Level>' should be created and listed successfully
    Examples: 
    | Language| Level            |
    | Tamil   | Native/Bilingual |
    | English | Fluent           |
    | Chinese | Conversational   |
    | Spanish | Basic            |
  
  #Updating Existing language and level
  @Order2
  Scenario: Update the existing language and level with valid data
    When I update an Existing Language and Existing Level in my profile
    | Language          | New Language | New Level           |
    | English           | German       | Basic           |
    | Spanish           | Arab         | Conversational  |
    Then The New Language and New Level should be updated and listed successfully
   
  #   Deleting All language and level
  @Order3
  Scenario: Delete all existing language and level
  When I delete all languages in my profile and successful message should appear
  Then The deleted language should not appear in the list

  #Duplicate values check  while adding language and level
  Scenario: Duplicate language entries handling while adding language
    When I try to add the following language entries:
      | DupLanguage | FirstLevel | SecondLevel    | ExpectedMessage                                       |
      | English     | Basic      | Basic          | This language is already exist in your language list. |
      | English     | Basic      | Conversational | Duplicated data    |

   Then ExpectedMessage should be displayed

#Duplicate values check  while editing language and level
  Scenario: Duplicate language entries handling while editing language
    When I try to add the following language entries whil editing:
      | Language | DupLanguage | Level          | ExpectedMessage                                       |
      | English  | English     | Basic          | This language is already exist in your language list. |
      | Spanish  |             | Language Level | Please enter language and level                       |
      | Spanish  |             | Language Level | Please enter language and level                       |
      | Spanish  | Spanish     | Language Level | Please enter language and level                       |
      | Spanish  |             | Language Level | Please enter language and level                       |
      | Spanish  | english     | Basic          | This language is already exist in your language list. |
      
  Then ExpectedMessage should be displayed

 Scenario: Duplicate language check with change of case
    When I try to add the same langauge with change of case
    Then The language should not be added and listed
   
   ##Language Field Validation
 Scenario: Language or Level field should not be empty
 When I try to add a language without language or level
 | Language | Level                 |
 |          | Choose Language Level |
 | English  | Choose Language Level |
 |          | Basic                 |
 Then Please enter language and level should be displayed

 Scenario: Check whether language field accepts only alphabets
 When I try to enter non-alphabet characters in language field
 | InvalidLanguage                       | Level            | ExpectedMessage            |
 | 123456789012345678901234567890 | Basic            | Please enter only language |
 | !@#$%^&*(){}!@#$%^&*(){}!@#$%^ | Conversational   | Please enter only language |
 | %%%%%%%%%%%%$$$$$$$$$$$$$$$$$$ | Fluent           | Please enter only language |
 | %English%Spanish%Tamil%Greek%  | Native/Bilingual | Please enter only language |
 Then Error message should be displayed