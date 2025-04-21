using System;
using System.Reflection.Emit;
using OpenQA.Selenium.BiDi.Modules.Log;
using qa_dotnet_cucumber.Pages;
using RazorEngine;
using NUnit.Framework;
using Reqnroll;
using Reqnroll.Configuration.JsonConfig;

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

                Assert.That(actualMessage == expectedMessage,
                    $"Expected message '{expectedMessage}', but got '{actualMessage}' for {dupLanguage} with {secondLevel} level.");
            }
        }


        //[When("I try to add the same language and same level")]
        //public void WhenITryToAddTheSameLanguageAndSameLevel()
        //{
        //    _languagePage.DeleteAllLanguages();
        //    _languagePage.CreateLanguageLevel("English", "Basic");
        //    _languagePage.CreateLanguageLevel("English", "Basic");

        //}

        //[Then("This language is already exist in your language list should be displayed")]
        //public void ThenThisLanguageIsAlreadyExistInYourLanguageListShouldBeDisplayed()
        //{
        //    string DupLangMsg = _languagePage.DuplicateLanguageErrorMsg();
        //    Console.Write("Message displayed:" + DupLangMsg);
        //    Assert.That(DupLangMsg == "This language is already exist in your language list.", "Duplicate language is getting added");

        //    _languagePage.IsDupLanguageAndLevelPresent("English", "Basic");
        //}

        //[When("I try to add the same language and different level")]
        //public void WhenITryToAddTheSameLanguageAndDifferentLevel()
        //{
        //    _languagePage.DeleteAllLanguages();
        //    _languagePage.CreateLanguageLevel("English", "Basic");
        //    _languagePage.CreateLanguageLevel("English", "Conversational");

        //}

        //[Then("Error message duplicated language should be displayed")]
        //public void ThenErrorMessageDuplicatedLanguageShouldBeDisplayed()
        //{
        //    string DupLangMsg = _languagePage.DuplicateLanguageErrorMsg();
        //    Console.Write("Message displayed:" + DupLangMsg);
        //    Assert.That(DupLangMsg == "Duplicated data", "Duplicate language is getting added");
        //    _languagePage.IsDupLanguageAndLevelPresent("English", "Conversational");
        //}


        //[When("I enter the same language and level which editing language")]
        //public void WhenIEnterTheSameLanguageAndLevelWhichEditingLanguage()
        //{
        //    _languagePage.UpdateLanguageAndLevel("English", "English", "Basic");

        //}


    }
}
