using System;
using System.Reflection.Emit;
using OpenQA.Selenium.BiDi.Modules.Log;
using qa_dotnet_cucumber.Pages;
using RazorEngine;
using NUnit.Framework;
using Reqnroll;
using Reqnroll.Configuration.JsonConfig;
using System.Text;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    [Scope(Feature = "Language Functionality")]
    public class LanguageFunctionalityStepDefinitions
    {

        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;
        private readonly LanguagePage _languagePage;
        private readonly ScenarioContext _scenarioContext;
        public LanguageFunctionalityStepDefinitions(LoginPage loginPage, NavigationHelper navigationHelper, LanguagePage languagePage, ScenarioContext scenarioContext)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
            _languagePage = languagePage;
            _scenarioContext = scenarioContext;
        }

        [Given("I sign in to the profile page with valid username and password")]
        public void GivenISignInToTheProfilePageAsARegisteredUser()
        {
            _navigationHelper.NavigateTo("Home/");
            Assert.That(_loginPage.IsAtLoginPage(), Is.True, "Home");
            _loginPage.Login("bha@gmail.com", "bhavani");
        }

        [When("I create a new {string} and {string} in my profile")]
        public void WhenICreateANewAndInMyProfile(string language, string level)
        {
            _languagePage.CreateLanguageLevel(language, level);
        }

        [Then("The {string} and {string} should be created and listed successfully")]
        public void ThenTheAndShouldBeCreatedAndListedSuccessfully(string language, string level)
        {
            string SavedLangAddedMsg = _languagePage.LangAddedSuccessMsg();
            string SavedLanguage = _languagePage.LanguageListing();
            string SavedLevel = _languagePage.LevelListing();

            Assert.That(SavedLangAddedMsg == language + " has been added to your languages", "Language Added Message is not displayed successfully");
            Assert.That(SavedLanguage == language, "Language has not been added successfully");
            Assert.That(SavedLevel == level, "Level has not been added successfully");

        }

        [When("I update an Existing Language and Existing Level in my profile")]
        public void WhenIUpdateAnExistingLanguageAndExistingLevelInMyProfile(Table updateLangtable)
        {
            _languagePage.DeleteAllLanguages();
            _languagePage.CreateLanguageLevel("English","Basic");
            _languagePage.CreateLanguageLevel("Spanish","Fluent");

            var updatedLanguages = new List<(string newLang, string newLevel, string successMsg)>();

            foreach (var row in updateLangtable.Rows)
            {
                var language = row["Language"];
                var newLanguage = row["New Language"];
                var newLevel = row["New Level"];

                _languagePage.UpdateLanguageAndLevel(language, newLanguage, newLevel);
                string successMsg = _languagePage.LangUpdatedSuccessMsg();
                updatedLanguages.Add((newLanguage, newLevel, successMsg));                    
                
                
            }
            _scenarioContext["updatedLanguages"] = updatedLanguages;
        }
        [Then("The New Language and New Level should be updated and listed successfully")]
        public void ThenTheNewLanguageAndNewLevelShouldBeUpdatedAndListedSuccessfully()
        {
            var updatedLanguages = (List<(string newLang, string newLevel, string successMsg)>)_scenarioContext["updatedLanguages"];

            foreach (var (newLang, newLevel, successMsg) in updatedLanguages)
            {
                var isLanguagePresent = _languagePage.IsLanguageAndLevelPresent(newLang, newLevel);
                Assert.That(isLanguagePresent,
                    Is.True,
                    $"Expected to find '{newLang}' with level '{newLevel}' in the language list, but it was not found.");
                Assert.That(successMsg.Contains(newLang),
                    $"Expected success message to contain '{newLang}', but got: '{successMsg}'");
            }
            
        }

        [When("I delete all languages in my profile and successful message should appear")]
        public void WhenIDeleteAllLanguagesInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _languagePage.DeleteAllLanguages();
        }

        [Then("The deleted language should not appear in the list")]
        public void ThenTheDeletedLanguageShouldNotAppearInTheList()
        {
            bool languagesExist = _languagePage.AreLanguagesPresent();

            Assert.That(languagesExist, Is.False, "Languages are still present in the profile list. Expected all to be deleted.");

        }
        //**Checking duplicates while adding language
        //Validation #1 Same language and same level
        //Validation #2 Same language with different level
        //Validation #3 Same language with change of case

        [When("I try to add the following language entries:")]
        public void WhenITryToAddTheFollowingLanguageEntries(Table dupLangCheckTable)
        {
            
            foreach (var row in dupLangCheckTable.Rows)
            {
                string dupLanguage = row["DupLanguage"];
                string firstLevel = row["FirstLevel"];
                string secondLevel = row["SecondLevel"];
                string expectedMessage = row["ExpectedMessage"];

                _languagePage.DeleteAllLanguages();

                _languagePage.CreateLanguageLevel(dupLanguage, firstLevel);
                _languagePage.CreateLanguageLevel(dupLanguage, secondLevel);

                
                string actualMessage = _languagePage.DuplicateLanguageErrorMsg();
                _languagePage.clickCancelButton(); 
                Console.WriteLine("Message displayed: " + actualMessage);

                _scenarioContext["ActualMessage"] = actualMessage;
                _scenarioContext["ExpectedMessage"] = expectedMessage;

                }
        }

        [Then("ExpectedMessage should be displayed")]
        public void ThenExpectedMessageShouldBeDisplayed()
        {
            string actualMessage = _scenarioContext["ActualMessage"] as string;
            string expectedMessage = _scenarioContext["ExpectedMessage"] as string;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                $"Expected message '{expectedMessage}', but got '{actualMessage}'.");

        }


        [When("I try to add the same langauge with change of case")]
        public void WhenITryToAddTheSameLangaugeWithChangeOfCase()
        {
            _languagePage.DeleteAllLanguages();
            _languagePage.CreateLanguageLevel("English", "Basic");
            _languagePage.CreateLanguageLevel("english", "Basic");
            
            string actualMessage = _languagePage.DuplicateLanguageErrorMsg();
            _scenarioContext["ActualErrorMessage"] = actualMessage;
        }

        [Then("The language should not be added and listed")]
        public void ThenTheLanguageShouldNotBeAddedAndListed()
        {
            string actualMessage = _scenarioContext["ActualErrorMessage"] as string;
            bool isDuplicate = _languagePage.IsDupLanguageAndLevelPresent("english", "Basic");

            string errorMessage = actualMessage;

            // Combine both assertions with a custom message
            Assert.Multiple(() =>
            {
                Assert.That(isDuplicate, Is.False, "Duplicated language and level is getting added, but it shouldn't be.");
                Assert.That(errorMessage, Is.EqualTo("This language is already exist in your language list."),
                            "The error message for duplicate language is incorrect.");
            });

        }

        //**Checking duplicates while editing language
        //Validation #1 Same language and same level
        //Validation #2 Same language with different level
        //Validation #3 Same language with change of case

        
        [When("I try to add the following language entries whil editing:")]
        public void WhenITryToAddTheFollowingLanguageEntriesWhilEditing(DataTable dupLangEditTable)
        {
            _languagePage.DeleteAllLanguages();
            _languagePage.CreateLanguageLevel("English", "Basic");
            _languagePage.CreateLanguageLevel("Spanish", "Fluent");

            foreach (var row in dupLangEditTable.Rows)
            {
                string language = row["Language"];
                string dupLanguage = row["DupLanguage"].Trim(); // Trim to ensure empty string is correctly detected
                string level = row["Level"].Trim();
                string expectedMessage = row["ExpectedMessage"];

                _languagePage.UpdateLanguageAndLevel(language, dupLanguage, level);

                string actualMessage = _languagePage.DuplicateLanguageErrorMsg();
                Console.WriteLine(actualMessage);

                // Click Cancel only if the button is present
                if (_languagePage.IsCancelButtonPresent())
                {
                    _languagePage.clickCancelButton();
                }

                _scenarioContext["ActualMessage"] = actualMessage;
                _scenarioContext["ExpectedMessage"] = expectedMessage;
            }
        }


        //**Language and Level field validation
        [When("I try to add a language without language or level")]
        public void WhenITryToAddALanguageWithoutLanguageOrLevel(Table fieldvalidationTable)
        {
            foreach (var row in fieldvalidationTable.Rows)
            {
                string language = row["Language"];
                string level = row["Level"];

                _languagePage.CreateLanguageLevel(language, level);
               


                string actualMessage = _languagePage.LangLevelFieldValidationErrMsg();
                _scenarioContext["ActualErrorMessage"] = actualMessage;
               // Console.WriteLine("Message displayed: " + actualMessage);
                _languagePage.clickCancelButton();
                

            }
        }

        [Then("Please enter language and level should be displayed")]
        public void ThenPleaseEnterLanguageAndLevelShouldBeDisplayed()
        {
            string actualMessage = _scenarioContext["ActualErrorMessage"] as string;
            //bool isDuplicate = _languagePage.IsDupLanguageAndLevelPresent("english", "Basic");

            string errorMessage = actualMessage;
            Assert.That(errorMessage, Is.EqualTo("Please enter language and level"),
                            "The error message for field validation is incorrect.");

            

        }
        //**Non-alphabet characters in language field
        [When("I try to enter non-alphabet characters in language field")]
        public void WhenITryToEnterNon_AlphabetCharactersInLanguageField(Table invalidLangDataSet)
        {
            var invalidLanguages = new List<(string invalidLanguage, string level, string expectedMessage, string actualMessage, bool isPresentInList)>();

            foreach (var row in invalidLangDataSet.Rows)
            {
                string invalidLanguage = row["InvalidLanguage"];
                string level = row["Level"];
                string expectedMessage = row["ExpectedMessage"];

                _languagePage.DeleteAllLanguages();  // Clear any existing data
                _languagePage.CreateLanguageLevel(invalidLanguage, level);  // Attempt to create a new language

                // Get the actual error message displayed for the invalid input
                string actualMessage = _languagePage.LangLevelFieldValidationErrMsg();

                // Check if the invalid language and level were added to the table (which should NOT happen)
                bool isPresent = _languagePage.IsLanguageAndLevelPresent(invalidLanguage, level);

                // Add the test data and results to the list
                invalidLanguages.Add((invalidLanguage, level, expectedMessage, actualMessage, isPresent));

            }
            _scenarioContext["InvalidLanguages"] = invalidLanguages;
        }

            [Then("Error message should be displayed")]
        public void ThenErrorMessageShouldBeDisplayed()
        {
            var invalidLanguages = (List<(string invalidLanguage, string level, string expectedMessage, string actualMessage, bool isPresentInList)>)_scenarioContext["InvalidLanguages"];

            // Use StringBuilder for error messages to improve efficiency
            var errorBuilder = new StringBuilder();
            bool hasError = false;  // Flag to check if any error occurred

            // Loop through each invalid language entry
            foreach (var result in invalidLanguages)
            {
                // Check if the actual message matches the expected one
                if (result.actualMessage != result.expectedMessage)
                {
                    hasError = true;
                    errorBuilder.AppendLine(
                        $"Mismatch in error message for language '{result.invalidLanguage}' and level '{result.level}'. " +
                        $"Expected: '{result.expectedMessage}', but got: '{result.actualMessage}'");
                }

                // Check if the invalid language was incorrectly added to the list
                if (result.isPresentInList)
                {
                    hasError = true;
                    errorBuilder.AppendLine(
                        $"Invalid language '{result.invalidLanguage}' with level '{result.level}' was incorrectly added to the language list.");
                }
            }

            // If errors exist, fail the test and display them
            Assert.That(hasError, Is.False, errorBuilder.ToString());
        }


    }
}
