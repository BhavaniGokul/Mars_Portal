using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.Log;
using OpenQA.Selenium.Support.UI;
using RazorEngine;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class LanguagePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By AddNewButton = By.XPath("/html/body/div[1]/div/section[2]/div/div/div/div[3]/form/div[2]/div/div[2]/div/table/thead/tr/th[3]/div");
        //private readonly By AddNewButton = By.XPath("//table[@class='ui fixed table']//th[last()]");

        private readonly By LanguageField = By.XPath("(//input[@type='text'])[4]");
        private readonly By LanguageLevelField = By.XPath("//select[@class='ui dropdown']");
        private readonly By AddButton = By.XPath("(//input[@type='button'])[1]");
        private readonly By AddedLanguage = By.XPath("(//table[@class='ui fixed table']//tbody[last()]//tr/td[1])[1]");
        private readonly By AddedLevel = By.XPath("//table[@class='ui fixed table']//tbody[last()]//tr/td[2]");
        private readonly By LanguageAddedMsg = By.XPath("//div[contains(text(),'has been added to your languages')]");

        //Update Locators
        private readonly By LanguageEditButton = By.XPath("(//i[@class='outline write icon'])[2]");
        private readonly By LanguageRow = By.XPath("//table[@class='ui fixed table']/tbody/tr");
        private readonly By LanguageUpdatedMsg = By.XPath("//div[contains(text(),'has been updated to your languages')]");


        //Delete Locators
        private readonly By LanguageDeleteButton = By.XPath("(//i[@class='remove icon'])");
        private readonly By LanguageDeletedMsg = By.XPath("//div[contains(text(),'has been deleted from your languages')]");

        // Definining Constructor
        public LanguagePage(IWebDriver driver) // Inject IWebDriver directly
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // 10-second timeout
            // _driver.Navigate().GoToUrl(Hooks.Hooks.Settings.Environment.BaseUrl);

        }

        //******Adding New Language and Level
        public void CreateLanguageLevel(string language, string level)
        {   //Create language and level through Add New
            // Click Add New
            try
            {
                // Click on Add New button
                var addNewButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddNewButton));
                addNewButtonElement.Click();
            }
            catch (Exception ex)
            {
                Assert.Fail("Add New Button has not been found");
            }

            //Enter Language
            var LanguageElement = _wait.Until(ExpectedConditions.ElementIsVisible(LanguageField));
            LanguageElement.SendKeys(language);

            //Choose language level from dropdown
            var languageLevelElement = _wait.Until(d => d.FindElement(LanguageLevelField));
            SelectElement langLevel = new SelectElement(languageLevelElement);
            langLevel.SelectByText(level);

            //Click Add button

            var AddButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddButton));
            var AddButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
            AddButtonElementClickable.Click();
            Thread.Sleep(3000);

        }
        public string LanguageListing()
        {
            var SavedLanguage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedLanguage));
            return SavedLanguage.Text;
            //  var SavedUpdatedLanguage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(UpdatedLanguage));
            // return SavedUpdatedLanguage.Text;
        }
        public string LevelListing()
        {
            var SavedLevel = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedLevel));
            return SavedLevel.Text;
        }

        public string LangAddedSuccessMsg()
        {
            var LangAddedMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(LanguageAddedMsg));
            return LangAddedMsg.Text;
        }

        //******Updating existing Language and Level

        public void UpdateLanguageAndLevel(string language, string newLanguage, string newLevel)
        {
            try
            {
                // Find all rows in the table body
                var languageRows = _wait.Until(d => d.FindElements(LanguageRow));

                foreach (var row in languageRows)
                {
                    var languageText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                    if (languageText.Equals(language, StringComparison.OrdinalIgnoreCase))
                    {
                        // Click the edit icon in that row
                        var editButton = row.FindElement(By.XPath(".//i[contains(@class, 'outline write icon')]"));
                        editButton.Click();

                        Thread.Sleep(1000); // Wait for input to appear

                        // Update the language and level
                        var languageElement = _wait.Until(ExpectedConditions.ElementIsVisible(LanguageField));
                        languageElement.Clear();
                        languageElement.SendKeys(newLanguage);

                        // Choose language level from dropdown
                        var languageLevelElement = _wait.Until(d => d.FindElement(LanguageLevelField));
                        SelectElement langLevel = new SelectElement(languageLevelElement);
                        langLevel.SelectByText(newLevel);

                        // Click Add button
                        var addButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
                        addButtonElement.Click();

                        Thread.Sleep(3000);

                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed: " + ex.Message);
            }
        }
        public string LangUpdatedSuccessMsg()
        {
            var LangUpdatedMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(LanguageUpdatedMsg));
            return LangUpdatedMsg.Text;
        }

        public bool IsLanguageAndLevelPresent(string language, string level)
        {
            // Find all rows in the language table
            var rows = _driver.FindElements(LanguageRow);

            foreach (var row in rows)
            {
                var languageCell = row.FindElement(By.XPath("./td[1]"));  
                var levelCell = row.FindElement(By.XPath("./td[2]"));

                // Check if the language and level in the row match the provided values
                if (languageCell.Text.Trim() == language && levelCell.Text.Trim() == level)
                {
                    return true; // Found the matching language and level
                }
            }

            return false; 
        }



        //*******Deleting Language and Level
        public void DeleteAllLanguages()
        {
            try
            {
                while (true)
                {
                    var deleteButtons = _driver.FindElements(LanguageDeleteButton);

                    if (deleteButtons.Count == 0)
                    {
                        Console.WriteLine("All languages have been deleted.");
                        break;
                    }

                    // Click the first delete button
                    deleteButtons[0].Click();

                    Thread.Sleep(2000); 
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("An error occurred while trying to delete languages: " + ex.Message);
            }
        }

        public bool AreLanguagesPresent()
        {
            var languageRows = _wait.Until(d => d.FindElements(LanguageRow));
            return languageRows.Any(); 
        }

    }
}
