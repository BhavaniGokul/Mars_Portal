using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.BiDi.Modules.Log;
using qa_dotnet_cucumber.Pages;
using RazorEngine;
using Reqnroll;
using qa_dotnet_cucumber.Models;  // Import the models

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    [Scope(Feature = "Education Functionality")]
    public class EducationSteps
    {
        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;
        private readonly EducationPage _educationPage;
        private List<EducationData> _educationData;

        public EducationSteps(LoginPage loginPage, NavigationHelper navigationHelper, EducationPage educationPage)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
            _educationPage = educationPage;
            LoadTestData();
        }

        // Load test data from the JSON file
        private void LoadTestData()
        {
            // Get the base directory where the executable is located
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Build the full path to the Resources folder (relative to the executable)
            var filePath = Path.Combine(baseDirectory, "..", "..", "..", "Resources", "EducationTestData.json");

            // Normalize the path (optional, but recommended)
            filePath = Path.GetFullPath(filePath);

            // Read the JSON file
            var jsonData = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<TestData>(jsonData);
            _educationData = data.EducationData;
        }

        [Given("I sign in to the profile page with valid username and password")]
        public void GivenISignInToTheProfilePageWithValidUsernameAndPassword()
        {
            _navigationHelper.NavigateTo("Home/");
            Assert.That(_loginPage.IsAtLoginPage(), Is.True, "Home");
            _loginPage.Login("bha@gmail.com", "bhavani");
            _educationPage.EducationTab();
        }

        // Use JSON data instead of scenario outline examples
        [When("I enter education details from the JSON file and save")]
        public void WhenIEnterEducationDetailsFromTheJsonFileAndSave()
        {
            _educationPage.DeleteAllEducation();
            foreach (var data in _educationData)
            {
                _educationPage.AddEducation(data.University, data.Country, data.Title, data.Degree, data.Year);
            }
        }

        [Then("The Education should be created and listed successfully")]
        public void ThenTheEducationShouldBeCreatedAndListedSuccessfully()
        {
            foreach (var data in _educationData)
            {
                
                string savedMessage = _educationPage.EducationAddedSuccessMsg();
               
                // Validate success message
                Assert.That(savedMessage, Is.EqualTo("Education has been added"), "Message is not displayed successfully");

                // Get the data from the form and the data displayed in the application
                var displayedDataList = _educationPage.GetAllDisplayedEducationData();  // Now returning List<EducationData>

               // Assert that the number of rows displayed is the same as the number of rows in the input data
                Assert.That(displayedDataList.Count, Is.EqualTo(_educationData.Count), "Mismatch in number of education entries.");

                // Iterate over both the form data and the displayed data and compare each entry
                for (int i = 0; i < _educationData.Count; i++)
                {
                    var formData = _educationData[i];
                    var displayedData = displayedDataList[i];

                    // Compare data correctly (education data from JSON vs displayed data)
                    Assert.That(displayedData.university, Is.EqualTo(formData.University), $"University mismatch for row {i + 1}.");
                    Assert.That(displayedData.country, Is.EqualTo(formData.Country), $"Country mismatch for row {i + 1}.");
                    Assert.That(displayedData.title, Is.EqualTo(formData.Title), $"Title mismatch for row {i + 1}.");
                    Assert.That(displayedData.degree, Is.EqualTo(formData.Degree), $"Degree mismatch for row {i + 1}.");
                    Assert.That(displayedData.year, Is.EqualTo(formData.Year), $"Year mismatch for row {i + 1}.");
                }
            }
        }
        [When("I delete all education entries in my profile and successful message should appear")]
        public void WhenIDeleteAllEducationEntriesInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _educationPage.DeleteAllEducation();
        }

        [Then("The deleted education entries should not appear in the list")]
        public void ThenTheDeletedEducationEntriesShouldNotAppearInTheList()
        {
            bool educationExist = _educationPage.AreEducationEntriesPresent();

            Assert.That(educationExist, Is.False, "Education entries are still present in the profile list. Expected all to be deleted.");

        }

    }
}

