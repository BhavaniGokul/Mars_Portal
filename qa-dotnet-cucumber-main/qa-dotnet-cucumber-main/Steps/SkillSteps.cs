using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qa_dotnet_cucumber.Pages;
using Reqnroll;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    [Scope(Feature = "Skill Functionality")]
    public class SkillSteps
    {
        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;
        private readonly SkillPage _skillPage;
        private readonly ScenarioContext _scenarioContext;
        public SkillSteps(LoginPage loginPage, NavigationHelper navigationHelper, SkillPage skillPage, ScenarioContext scenarioContext)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
            _skillPage = skillPage;
            _scenarioContext = scenarioContext;
        }

        [Given("I sign in to the profile page with valid username and password")]
        public void GivenISignInToTheProfilePageAsARegisteredUser()
        {
            _navigationHelper.NavigateTo("Home/");
            Assert.That(_loginPage.IsAtLoginPage(), Is.True, "Home");
            _loginPage.Login("bha@gmail.com", "bhavani");
            _skillPage.SkillsTab();
        }

        [When("I create a new {string} and {string} in my profile")]
        public void WhenICreateANewAndInMyProfile(string skill, string level)
        {
            
            _skillPage.CreateSkillLevel(skill, level);
        }

        [Then("The {string} and {string} should be created and listed successfully")]
        public void ThenTheAndShouldBeCreatedAndListedSuccessfully(string skill, string level)
        {
            string SavedSkillAddedMsg = _skillPage.SkillAddedSuccessMsg();
            string SavedSkill = _skillPage.SkillListing();
            string SavedLevel = _skillPage.LevelListing();

            Assert.That(SavedSkillAddedMsg == skill + " has been added to your skills", "Skill Added Message is not displayed successfully");
            Assert.That(SavedSkill == skill, "Skill has not been added successfully");
            Assert.That(SavedLevel == level, "Level has not been added successfully");

        }

        [When("I update an Existing Skill and Existing Level in my profile")]
        public void WhenIUpdateAnExistingSkillAndExistingLevelInMyProfile(Table updateSkilltable)
        {
            var updatedSkills = new List<(string newLang, string newLevel, string successMsg)>();

            foreach (var row in updateSkilltable.Rows)
            {
                var skill = row["Skill"];
                var newSkill = row["New Skill"];
                var newLevel = row["New Level"];

                _skillPage.UpdateSkillAndLevel(skill, newSkill, newLevel);
                string successMsg = _skillPage.SkillUpdatedSuccessMsg();
                updatedSkills.Add((newSkill, newLevel, successMsg));


            }
            _scenarioContext["updatedSkills"] = updatedSkills;
        }
        [Then("The New Skill and New Level should be updated and listed successfully")]
        public void ThenTheNewSkillAndNewLevelShouldBeUpdatedAndListedSuccessfully()
        {
            var updatedSkills = (List<(string newSkill, string newLevel, string successMsg)>)_scenarioContext["updatedSkills"];

            foreach (var (newSkill, newLevel, successMsg) in updatedSkills)
            {
                var isSkillPresent = _skillPage.IsSkillAndLevelPresent(newSkill, newLevel);
                Assert.That(isSkillPresent,
                    Is.True,
                    $"Expected to find '{newSkill}' with level '{newLevel}' in the skill list, but it was not found.");
                Assert.That(successMsg.Contains(newSkill),
                    $"Expected success message to contain '{newSkill}', but got: '{successMsg}'");
            }

        }

        [When("I delete all skills in my profile and successful message should appear")]
        public void WhenIDeleteAllSkillsInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _skillPage.DeleteAllSkills();
        }

        [Then("The deleted skills should not appear in the list")]
        public void ThenTheDeletedSkillsShouldNotAppearInTheList()
        {
            bool languagesExist = _skillPage.AreSkillsPresent();

            Assert.That(languagesExist, Is.False, "Languages are still present in the profile list. Expected all to be deleted.");

        }

    }
}

